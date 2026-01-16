' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'







Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A visualizer that renders cell values [0, 1] as a progress bar.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    
    Public Class cEwEGridProportionVisualizer
        Inherits cEwEGridVisualizerBase

        Protected Overrides Sub DrawCell_ImageAndText(cell As SourceGrid2.Cells.ICellVirtual, _
                                                      pos As SourceGrid2.Position, _
                                                      e As System.Windows.Forms.PaintEventArgs, _
                                                      rc As System.Drawing.Rectangle, _
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
