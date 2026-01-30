' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources



Namespace Ecospace

    Public Class frmEcospaceMPAEnforcement

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
            Me.TabText = Me.Text
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)

            Dim cmd As cCommand = Nothing
            MyBase.OnLoad(e)

            If (Me.CommandHandler Is Nothing) Then Return

            cmd = Me.CommandHandler.GetCommand(cEditMPAsCommand.cCOMMAND_NAME)
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnDefineMPAs)

            Me.m_tsbnDefineMPAs.Image = SharedResources.MPA
            Me.m_tsbnQuickHelp.Image = SharedResources.Info
            Me.m_lblInfo.Visible = False

        End Sub

        Private Sub OnShowQuickHelp(sender As Object, e As EventArgs) Handles m_tsbnQuickHelp.MouseDown
            Me.m_lblInfo.Visible = True
        End Sub

        Private Sub OnHideQuickHelp(sender As Object, e As MouseEventArgs) Handles m_tsbnQuickHelp.MouseUp
            Me.m_lblInfo.Visible = False
        End Sub

    End Class

End Namespace
