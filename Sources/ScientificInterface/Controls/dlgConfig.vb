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

Imports EwEUtils
Imports EwECore

Public Class dlgConfig
    Implements IUIElement

    Private m_ctrl As Control = Nothing
    Private m_uic As cUIContext = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
        Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Set(value As ScientificInterfaceShared.Controls.cUIContext)
            Me.m_uic = value
        End Set
    End Property

    Public Shadows Function ShowDialog(owner As IWin32Window, strTitle As String, ctrl As Control) As DialogResult

        ' Set window text
        Me.Text = strTitle
        ' Store control
        Me.m_ctrl = ctrl
        ' Base, do your work
        Return MyBase.ShowDialog(owner)

    End Function

    Public Shadows Function ShowDialog(strTitle As String, ctrl As Control) As DialogResult
        Return Me.ShowDialog(Nothing, strTitle, ctrl)
    End Function

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        ' Resize to page control size
        Dim szPanel As Size = Me.m_plContent.Size
        Dim szPage As Size = Me.m_ctrl.Size

        Me.Size = New Size(Me.Width + szPage.Width - szPanel.Width, _
                           Me.Height + szPage.Height - szPanel.Height)

        Me.MinimumSize = Size

        Me.m_ctrl.Dock = DockStyle.Fill
        Me.m_plContent.Controls.Add(Me.m_ctrl)

        Me.CenterToParent()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Me.m_plContent.Controls.Remove(Me.m_ctrl)
        Me.m_ctrl.Dispose()
        Me.UIContext = Nothing
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        Dim uic As cUIContext = Me.UIContext

        Try
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        Catch ex As Exception
            cLog.Write(ex, "dlgConfig::OnOK")
        End Try

        If (uic IsNot Nothing) Then
            cApplicationStatusNotifier.StartProgress(uic.Core, ScientificInterfaceShared.My.Resources.STATUS_APPLYVALUES)
        End If

    End Sub

End Class