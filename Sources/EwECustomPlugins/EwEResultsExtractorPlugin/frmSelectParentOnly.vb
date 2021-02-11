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

Imports EwECore
Imports System.Drawing

Public Class frmSelectParentOnly
    Inherits CreateCollectionForData

    Private Shared theInstance As frmSelectParentOnly
    Public Event FormExited()

    Public Sub New(i As cSelectionData, p As cCore)
        MyBase.New(i, p)

        ' This call is required by the Windows Form Designer.
        Me.InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Width = 380
        Me.chklstAttached.Hide()
        Me.btnAttachAll.Hide()
        Me.btnAttachNone.Hide()
        Me.btnOk.Left = 280

    End Sub

    Public Overrides Sub PopulateAttachedList(i As String)

    End Sub

    Public Shared ReadOnly Property GetInstance(i As cSelectionData, p As cCore) As frmSelectParentOnly
        Get
            If theInstance Is Nothing Then
                theInstance = New frmSelectParentOnly(i, p)
            End If
            Return theInstance
        End Get
    End Property

    Private Sub frmSelectParentOnly_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
        theInstance = Nothing
    End Sub

End Class