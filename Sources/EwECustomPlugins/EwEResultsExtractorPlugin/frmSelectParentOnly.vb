' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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