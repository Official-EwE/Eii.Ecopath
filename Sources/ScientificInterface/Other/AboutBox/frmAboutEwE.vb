' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.SystemUtilities
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports 

Namespace Other

    ''' =======================================================================
    ''' <summary>
    ''' EwE about box form.
    ''' </summary>
    ''' =======================================================================
    Public Class frmAboutEwE

        Private m_uic As cUIContext = Nothing

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Me.m_uic = uic
            Me.m_gridTechnical.UIContext = uic
            Me.m_gridDatabase.UIContext = uic
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return

            Dim strTitle As String = My.Resources.GENERIC_CAPTION
            Dim strBitApp As String = cSystemUtils.IIF(cSystemUtils.Is64Bit, SharedResources.ABOUT_64BIT, SharedResources.ABOUT_32BIT)

            ' Format generic page
            Me.Text = String.Format(My.Resources.ABOUT_CAPTION, strTitle)
            Me.m_lbTitle.Text = strTitle
            Me.m_lbVersion.Text = String.Format(My.Resources.ABOUT_VERSION, cCore.Version, strBitApp)
            Me.m_lbCopyright.Text = String.Format(My.Resources.ABOUT_COPYRIGHT, My.Application.Info.Copyright, My.Application.Info.CompanyName)

            ' Format technical page
            Me.m_lblOSVersion.Text = cSysConfig.OSVersion()
            Me.m_lblNetVersion.Text = cSysConfig.NETVersion()

            Dim strFont As String = Me.Font.Name

            Me.m_rtbTeam.Rtf = StyleRTF(My.Resources.team)
            Me.m_rtbLicense.Rtf = StyleRTF(My.Resources.license)
            Me.m_rtbAcknowledgements.Rtf = StyleRTF(My.Resources.acknowledgements)

            If Not Me.m_uic.Core.StateMonitor.HasEcopathLoaded Then
                Me.m_tcMain.TabPages.Remove(Me.m_tpDatabase)
            End If

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.Close()
        End Sub

        Private Sub OnURLClicked(sender As Object, e As System.Windows.Forms.LinkClickedEventArgs) _
            Handles m_rtbLicense.LinkClicked, m_rtbDistribution.LinkClicked

            Try
                Dim cmd As cBrowserCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                If cmd IsNot Nothing Then
                    cmd.Invoke(e.LinkText)
                End If
            Catch ex As Exception
                ' Aargh
                cLog.Write(ex, "frmAboutEwE::OnURLClicked")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Switch RTF text from 9 pt Arial to local form font and size.
        ''' </summary>
        ''' <param name="strRTF">Text to convert, needing to specify Arial font at 18 pt</param>
        ''' <returns>A transmogrified text.</returns>
        ''' -------------------------------------------------------------------
        Private Function StyleRTF(strRTF As String) As String

            Dim strFont As String = Me.Font.FontFamily.Name
            Dim szFont As Single = Me.Font.Size

            strRTF = strRTF.Replace("Arial;", strFont & ";")
            strRTF = strRTF.Replace("\fs18", "\fs" & CInt(szFont * 2))

            Return strRTF
        End Function

    End Class

End Namespace

