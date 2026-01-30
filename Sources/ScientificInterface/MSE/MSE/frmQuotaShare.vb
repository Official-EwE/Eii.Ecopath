' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the MSE Quota Shared interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmQuotaShare
        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Private Sub OnSumSharesToOne(sender As System.Object, e As System.EventArgs) _
            Handles m_tsSumtoOneBtn.Click
            Me.Core.NormalizeQuotaShare()
        End Sub

        Private Sub OnDefaultShares(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnDefaults.Click
            Me.Core.SetDefaultQuotaShare()
        End Sub
    End Class

End Namespace
