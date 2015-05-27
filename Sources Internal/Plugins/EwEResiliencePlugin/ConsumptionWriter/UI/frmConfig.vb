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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands

#End Region ' Imports

Public Class frmConfig

    Private m_uic As cUIContext = Nothing

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic
        Me.InitializeComponent()
    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_cbAutosave.Checked = My.Settings.ConsAutosave
        Me.m_cbIncludeDetritus.Checked = My.Settings.ConsIncludeDetritus
        Me.m_cbIncludeImportAndSum.Checked = My.Settings.ConsIncludeImportAndSum

        Dim cmd As cCommand = Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
        cmd.AddControl(Me.m_pbIPN, "http://www.ipn.mx")
        cmd.AddControl(Me.m_pbCicimar, "http://www.cicimar.ipn.mx")
        cmd.AddControl(Me.m_pbConacyt, "http://www.conacyt.mx")

        Me.UpdateControls()
    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Dim cmd As cCommand = Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
        cmd.RemoveControl(Me.m_pbIPN)
        cmd.RemoveControl(Me.m_pbCicimar)
        cmd.RemoveControl(Me.m_pbConacyt)
        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Overrides

#Region " Event handlers "

    Private Sub OnCheckChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbAutosave.CheckedChanged, _
                m_cbIncludeDetritus.CheckedChanged, _
                m_cbIncludeImportAndSum.CheckedChanged
        Try
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        My.Settings.ConsAutosave = Me.m_cbAutosave.Checked
        My.Settings.ConsIncludeDetritus = Me.m_cbIncludeDetritus.Checked
        My.Settings.ConsIncludeImportAndSum = Me.m_cbIncludeImportAndSum.Checked
        My.Settings.Save()

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

#End Region ' Event handlers

#Region " Internals "

    Private Sub ShowSponsor(strURL As String)

        If (Me.m_uic Is Nothing) Then Return

        Try
            Dim cmd As cBrowserCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.Invoke(strURL)
        Catch ex As Exception
            cLog.Write(ex, "ConsumptionWriterPlugin::frmConfig.ShowSponsor(" & strURL & ")")
        End Try

    End Sub

    Private Sub UpdateControls()
        Me.m_cbIncludeDetritus.Enabled = Me.m_cbAutosave.Checked
        Me.m_cbIncludeImportAndSum.Enabled = Me.m_cbAutosave.Checked
    End Sub

#End Region ' Internals

End Class
