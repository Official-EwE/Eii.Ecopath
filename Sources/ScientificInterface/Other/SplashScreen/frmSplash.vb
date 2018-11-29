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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Friend Class frmSplash

    Private m_expired As TriState = TriState.UseDefault
    Private m_mode As eReleaseMode = eReleaseMode.Dev
    Private m_dtServer As DateTime = DateTime.MinValue
    Private m_dtExpiry As DateTime = DateTime.MinValue
    Private m_bCanAutoClose As Boolean = True

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_pbIcon.BackgroundImage = cDrawingUtils.BitmapFromIcon(cEwEIcon.Current())
        Me.m_pbIcon.BackgroundImageLayout = ImageLayout.Zoom

        Me.m_lblEwE.Text = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.GENERIC_CAPTION, cCore.Version(False))

        Me.m_btnOK.Visible = False

        Me.m_mode = frmEwE6.ReleaseMode()
        Select Case Me.m_mode
            Case eReleaseMode.Beta, eReleaseMode.Pro
                Me.m_dtExpiry = cCore.BestBefore(eReleaseMode.Beta)
                Me.m_lblDetails.Text = My.Resources.STATUS_CHECKING_LICENSE
                Me.m_bCanAutoClose = False
                Me.m_chugchug.RunWorkerAsync()
            Case Else
                Me.m_dtExpiry = cCore.BestBefore(eReleaseMode.Free)
                Me.m_dtServer = Me.m_dtExpiry
                Me.m_lblDetails.Text = "Loading..."
        End Select

        Me.CenterToScreen()
        Me.TopMost = True

    End Sub

    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
        ' NOP
    End Sub

    Public Function CanAutoClose() As Boolean
        Return m_bCanAutoClose
    End Function

    Public Function Expired() As TriState
        Return Me.m_expired
    End Function

    Public Sub PleaseClose()
        If Me.InvokeRequired() Then
            Me.Invoke(New MethodInvoker(AddressOf Close))
        Else
            Me.Close()
        End If
    End Sub

    Private Sub DoWork(sender As Object, args As DoWorkEventArgs) Handles m_chugchug.DoWork
        Me.m_dtServer = cDateUtils.GetNetworkTime()
    End Sub

    Private Sub OnChuggedOut(sender As Object, e As RunWorkerCompletedEventArgs) Handles m_chugchug.RunWorkerCompleted
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf UpdateControls))
        Else
            Me.UpdateControls()
        End If
    End Sub

    Private Sub OnClose(sender As Object, e As EventArgs) Handles m_btnOK.Click
        Me.Close()
    End Sub

    Private Sub UpdateControls()

        Select Case Me.m_mode
            Case eReleaseMode.Beta, eReleaseMode.Pro
                If Me.m_dtServer <= Me.m_dtExpiry Then
                    Me.m_lblDetails.Text = cStringUtils.Localize(My.Resources.ABOUT_EXPIRY, Me.m_dtExpiry.ToShortDateString)
                    Me.m_expired = TriState.False
                Else
                    Me.m_lblDetails.Text = My.Resources.ABOUT_EXPIRED
                    Me.m_expired = TriState.True
                End If
                Me.m_btnOK.Visible = True
            Case Else
                Me.m_btnOK.Visible = False
        End Select

    End Sub

End Class