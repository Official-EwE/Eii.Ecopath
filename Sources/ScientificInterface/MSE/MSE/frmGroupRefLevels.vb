' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the MSE Group Reference Levels interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmGroupRefLevels

        Public Sub New()
            MyBase.New(New gridGroupRefLevels())
            Me.InitializeComponent()
        End Sub

        Private Sub OnReset(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnReset.Click
            Me.Core.ResetMSEGroupRefLevels()
        End Sub

    End Class

End Namespace
