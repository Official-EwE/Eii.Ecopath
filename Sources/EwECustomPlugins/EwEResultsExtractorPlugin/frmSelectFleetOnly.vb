' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

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