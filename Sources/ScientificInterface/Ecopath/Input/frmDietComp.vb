' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecopath Diet Composition interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmDietComp

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Private Sub tsSumtoOneBtn_Click(sender As System.Object, e As System.EventArgs) _
            Handles tsSumtoOneBtn.Click
            Me.Core.NormalizeDietInput()
        End Sub
    End Class

End Namespace

