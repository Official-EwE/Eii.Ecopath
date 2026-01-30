' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecopath Fisheries fleet definitions interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmFisheryBasicInput

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            If (Me.CommandHandler Is Nothing) Then Return

            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditFleets")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnEditFleets)
        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            If (Me.CommandHandler IsNot Nothing) Then
                Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditFleets")
                If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnEditFleets)
            End If
            MyBase.OnFormClosed(e)
        End Sub

    End Class

End Namespace
