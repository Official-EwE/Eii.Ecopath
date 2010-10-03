#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Drawing
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A visualizer that renders cell values [0, 1] as a progress bar.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cEwEGridProportionVisualizer
        Inherits cEwEGridVisualizerBase

        Protected Overrides Sub DrawCell_ImageAndText(ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                                                      ByVal pos As SourceGrid2.Position, _
                                                      ByVal e As System.Windows.Forms.PaintEventArgs, _
                                                      ByVal rc As System.Drawing.Rectangle, _
                                                      ByVal status As SourceGrid2.DrawCellStatus)

            Dim objVal As Object = cell.GetValue(pos)
            If Not (TypeOf objVal Is Single) Then Return

            Dim sVal As Single = CSng(objVal)
            Dim rcBox As New Rectangle(rc.Left + 3, rc.Top + 2, rc.Width - 6, rc.Height - 4)
            Dim rcFill As New Rectangle(rcBox.Left, rcBox.Top, CInt(Math.Min(sVal, 1) * rcBox.Width), rcBox.Height)

            e.Graphics.FillRectangle(SystemBrushes.Window, rcBox)
            e.Graphics.FillRectangle(SystemBrushes.Highlight, rcFill)
            e.Graphics.DrawRectangle(SystemPens.ControlDark, rcBox)

        End Sub

    End Class

End Namespace
