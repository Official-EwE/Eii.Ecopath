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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwECore
Imports EwEUtils.SystemUtilities
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports ScientificInterfaceShared.Commands

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
            Dim strBit As String = IIF(cSystemUtils.Is64Bit, My.Resources.ABOUT_64BIT, My.Resources.ABOUT_32BIT)

            ' Format generic page
            Me.Text = String.Format(My.Resources.ABOUT_CAPTION, strTitle)
            Me.m_lbTitle.Text = strTitle
            Me.m_lbVersion.Text = String.Format(My.Resources.ABOUT_VERSION, cCore.Version, strBit)
            Me.m_lbCopyright.Text = String.Format(My.Resources.ABOUT_COPYRIGHT, My.Application.Info.Copyright, My.Application.Info.CompanyName)

            ' Format technical page
            Me.m_lblNetVersion.Text = String.Format(My.Resources.ABOUT_VERSION, System.Environment.Version.ToString(), strBit)

            Me.m_rtbTeam.Rtf = My.Resources.team
            Me.m_rtbLicense.Rtf = My.Resources.license
            Me.m_rtbAcknowledgements.Rtf = My.Resources.acknowledgements

            If Not Me.m_uic.Core.StateMonitor.HasEcopathLoaded Then
                Me.m_tcMain.TabPages.Remove(Me.m_tpDatabase)
            End If

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.Close()
        End Sub

        Private Sub OnURLClicked(sender As Object, e As System.Windows.Forms.LinkClickedEventArgs) _
            Handles m_rtbLicense.LinkClicked

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

    End Class

End Namespace

