' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Utilities
Imports SourceGrid2

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A visualizer for rendering EwE column header cells.
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cEwEGridColumnHeaderVisualizer
        Inherits SourceGrid2.VisualModels.Header

        Public Sub New(Optional alignment As ContentAlignment = ContentAlignment.MiddleCenter)
            MyBase.New(False)
            Me.TextAlignment = alignment
            Me.WordWrap = True
            Me.AlignTextToImage = True
        End Sub

        Protected Overrides Sub DrawCell_Border(p_Cell As SourceGrid2.Cells.ICellVirtual, p_CellPosition As SourceGrid2.Position, e As System.Windows.Forms.PaintEventArgs, p_ClientRectangle As System.Drawing.Rectangle, p_Status As SourceGrid2.DrawCellStatus)

            Dim border As RectangleBorder = Me.Border
            Dim rc As Rectangle = p_ClientRectangle
            Dim l_BackColor As Color = Me.BackColor

            If (p_Status = DrawCellStatus.Focus) Then
                l_BackColor = Me.FocusBackColor
            ElseIf (p_Status = DrawCellStatus.Selected) Then
                l_BackColor = Me.SelectionBackColor
                l_BackColor = Me.BackColor
            End If

            ' Draw the border
            ControlPaint.DrawBorder(e.Graphics, rc,
                SystemColors.ButtonHighlight, 1, ButtonBorderStyle.Solid,
                Color.Transparent, 0, ButtonBorderStyle.Solid,
                SystemColors.ButtonShadow, 1, ButtonBorderStyle.Solid,
                SystemColors.ButtonShadow, 1, ButtonBorderStyle.Solid)

        End Sub

        Protected Overrides Sub DrawCell_ImageAndText(cell As SourceGrid2.Cells.ICellVirtual, pos As SourceGrid2.Position, e As System.Windows.Forms.PaintEventArgs, rc As System.Drawing.Rectangle, p_Status As SourceGrid2.DrawCellStatus)
            If cell.Grid.Enabled Then
                Me.ForeColor = SystemColors.ControlText
            Else
                Me.ForeColor = cColorUtils.GetVariant(SystemColors.ControlText, 0.5)
            End If
            MyBase.DrawCell_ImageAndText(cell, pos, e, rc, p_Status)
        End Sub

    End Class

End Namespace