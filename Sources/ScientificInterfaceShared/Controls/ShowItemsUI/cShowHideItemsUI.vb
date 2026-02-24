' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Style

Namespace Controls

    Public Class cShowHideItemsUI

        Private m_uic As cUIContext = Nothing
        Private WithEvents m_ctrl As ToolStripDropDownButton = Nothing

        Public Sub New(uic As cUIContext)
            Me.m_uic = uic
        End Sub

        Public Sub Attach(ctrl As ToolStripDropDownButton)
            Me.Detach()
            Me.m_ctrl = ctrl
        End Sub

        Public Sub Detach()
            Me.m_ctrl = Nothing
        End Sub

        Private Sub OnButtonClicked(sender As Object, e As EventArgs) Handles m_ctrl.Click
            Me.m_uic.CommandHandler.GetCommand(cShowHideItemsCommand.COMMAND_NAME).Invoke()
        End Sub

        Private Sub OnDropDownOpening(sender As Object, e As EventArgs) Handles m_ctrl.DropDownOpening
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            For i As Integer = 0 To sg.ItemVisibilityPresetNames.Length - 1
                Me.m_ctrl.DropDownItems.Add(sg.ItemVisibilityPresetNames(i))
            Next
        End Sub

        Private Sub OnDropDownClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles m_ctrl.DropDownItemClicked
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            sg.SelectedItemVisibilityPresetName = e.ClickedItem.Text
        End Sub

        Private Sub OnDropDownClosed(sender As Object, e As EventArgs) Handles m_ctrl.DropDownClosed
            Me.m_ctrl.DropDownItems.Clear()
        End Sub

    End Class

End Namespace
