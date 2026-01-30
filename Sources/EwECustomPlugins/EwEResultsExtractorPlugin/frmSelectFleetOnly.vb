' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class frmSelectFleetOnly
    Inherits CreateCollectionForData

    Public Event FormExited()

    Public Sub New(i As cSelectionData, m_core As cCore)
        MyBase.New(i, m_core)

        ' This call is required by the Windows Form Designer.
        Me.InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Width = 380
        Me.chklstAttached.Hide()
        Me.btnAttachAll.Hide()
        Me.btnAttachNone.Hide()
        Me.btnOk.Left = 280
        Me.Show()

    End Sub
    Public Overrides Sub PopulateAttachedList(i As String)

    End Sub

    Private Sub frmSelectFleetOnly_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
    End Sub
End Class