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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmConfig

    Public Sub New(uic As cUIContext)
        Me.InitializeComponent()
        Me.m_ack.UIContext = uic
    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_cbAutosave.Checked = My.Settings.ConsAutosave
        Me.m_cbIncludeDetritus.Checked = My.Settings.ConsIncludeDetritus
        Me.m_cbIncludeImportAndSum.Checked = My.Settings.ConsIncludeImportAndSum

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

    Private Sub UpdateControls()
        Me.m_cbIncludeDetritus.Enabled = Me.m_cbAutosave.Checked
        Me.m_cbIncludeImportAndSum.Enabled = Me.m_cbAutosave.Checked
    End Sub

#End Region ' Internals

End Class
