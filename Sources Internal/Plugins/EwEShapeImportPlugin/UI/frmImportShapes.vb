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
Imports EwEPlugin
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmImportShapes

    Private m_uic As cUIContext = Nothing

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        ' Load all available shape functions
        ' Load all available shape creation destinations (e.g., shape managers)
        ' - GUI shape handlers are probably the easiest vehicle, because they already know which shape type can be created
        ' - Stick these into the target combo

        Me.CenterToScreen()
        Me.UpdateControls()

    End Sub

#Region " Events "

    Private Sub m_btnImportBrowse_Click(sender As System.Object, e As System.EventArgs) Handles m_btnImportBrowse.Click

    End Sub

    Private Sub m_tbImportDelimiter_TextChanged(sender As System.Object, e As System.EventArgs) Handles m_tbImportDelimiter.TextChanged

    End Sub

    Private Sub m_rbImportSourceClipboard_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles m_rbImportSourceClipboard.CheckedChanged

    End Sub

    Private Sub m_rbImportSourceTextFile_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles m_rbImportSourceTextFile.CheckedChanged

    End Sub

    Private Sub m_cmbTarget_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles m_cmbTarget.SelectedIndexChanged

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) Handles m_btnOk.Click

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) Handles m_btnCancel.Click

    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()

        Dim bCanImport As Boolean = True

        Me.m_btnOk.Enabled = bCanImport

    End Sub

    Private Sub ReadPreview(strText As String)

    End Sub

#End Region ' Internals

End Class