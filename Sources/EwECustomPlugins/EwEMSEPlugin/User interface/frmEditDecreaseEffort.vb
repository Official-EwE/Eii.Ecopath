' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports ScientificInterfaceShared.Controls
Imports LumenWorks.Framework.IO.Csv
Imports EwEUtils.Utilities

Public Class frmEditDecreaseEffort

    Private m_mse As cMSE = Nothing
    Private m_data As cEffortLimits = Nothing
    Private m_bInitialized As Boolean

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(uic As cUIContext, mse As cMSE)
        Me.m_mse = mse
        Me.m_data = New cEffortLimits(mse, mse.Core)
        Me.m_data.Load()
        Me.Grid = Me.m_grid
        Me.UIContext = uic
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.QuickEditHandler.ShowImportExport = False
        Me.QuickEditHandler.Attach(Me.m_grid, Me.UIContext, Me.m_ts)

        Me.m_grid.Init(Me.m_data)

        Me.rbDecaying.Checked = Me.m_data.decaying_max_effort
        Me.rbProportion.Checked = Not Me.m_data.decaying_max_effort

        Me.m_bInitialized = True

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Me.QuickEditHandler.Detach()
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Try
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSave.Click

        Try
            ' Save to default location
            If Me.m_data.Save("") Then
                Me.DialogResult = System.Windows.Forms.DialogResult.OK
                Me.Close()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub rbDecaying_CheckedChanged(sender As Object, e As EventArgs) Handles rbDecaying.CheckedChanged
        'If Not Me.m_bInitialized Then Return
        If Not Me.m_bInitialized Then Return
        Me.m_data.decaying_max_effort = Me.rbDecaying.Checked
        'Me.UpdateControls()
    End Sub

    Private Sub rbProportion_CheckedChanged(sender As Object, e As EventArgs) Handles rbProportion.CheckedChanged
        If Not Me.m_bInitialized Then Return
        Me.m_data.decaying_max_effort = Me.rbDecaying.Checked
    End Sub

End Class