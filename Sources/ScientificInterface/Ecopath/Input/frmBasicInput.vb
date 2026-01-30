' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecopath Basic Input interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmBasicInput

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            Dim cmd As cCommand = Nothing
            MyBase.OnLoad(e)

            If (Me.CommandHandler Is Nothing) Then Return

            cmd = Me.CommandHandler.GetCommand("EditGroups")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnEditGroups)
            cmd = Me.CommandHandler.GetCommand("EditMultiStanza")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnEditMultiStanza)
        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            If (Me.CommandHandler IsNot Nothing) Then
                Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditGroups")
                If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnEditGroups)
                cmd = Me.CommandHandler.GetCommand("EditMultiStanza")
                If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnEditMultiStanza)
            End If
            MyBase.OnFormClosed(e)

        End Sub

    End Class

End Namespace
