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

#End Region ' Imports

Public Class frmConfig

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.m_cbIncludeDetritus.Checked = My.Settings.IncludeDetritus
        Me.m_cbIncludeImportAndSum.Checked = My.Settings.IncludeImportAndSum
    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) Handles m_btnOK.Click
        My.Settings.IncludeDetritus = Me.m_cbIncludeDetritus.Checked
        My.Settings.IncludeImportAndSum = Me.m_cbIncludeImportAndSum.Checked
        My.Settings.Save()
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) Handles m_btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

End Class