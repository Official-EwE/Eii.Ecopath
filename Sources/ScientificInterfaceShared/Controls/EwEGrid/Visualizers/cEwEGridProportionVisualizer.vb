' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A visualizer that renders cell values [0, 1] as a progress bar.
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class cEwEGridProportionVisualizer
        Inherits cEwEGridVisualizerBase

        Protected Overrides Sub DrawCell_ImageAndText(cell As SourceGrid2.Cells.ICellVirtual,
                                                      pos As SourceGrid2.Position,
                                                      e As System.Windows.Forms.PaintEventArgs,
                                                      rc As System.Drawing.Rectangle,
                                                      status As SourceGrid2.DrawCellStatus)

            Dim objVal As Object = cell.GetValue(pos)
            If Not (TypeOf objVal Is Single) Then Return

            Dim sVal As Single = CSng(objVal)
            Dim rcBox As New Rectangle(rc.Left + 3, rc.Top + 4, Math.Max(0, rc.Width - 6), Math.Max(0, rc.Height - 9))
            Dim rcFill As New Rectangle(rcBox.Left, rcBox.Top, CInt(Math.Min(sVal, 1) * rcBox.Width), rcBox.Height)

            e.Graphics.FillRectangle(SystemBrushes.Window, rcBox)
            e.Graphics.FillRectangle(SystemBrushes.Highlight, rcFill)
            e.Graphics.DrawRectangle(SystemPens.ControlDark, rcBox)

        End Sub

    End Class

End Namespace
