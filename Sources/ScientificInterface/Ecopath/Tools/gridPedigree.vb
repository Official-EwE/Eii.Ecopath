#Region " Imports "

Option Strict On

Imports EwECore

#End Region 'Imports

Namespace Ecopath.Tools

    <CLSCompliant(False)> _
    Public Class gridPedigree
        Inherits EwEGrid

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' cColorCellVisualizer is a cell visualizer that provides color feedback.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <CLSCompliant(False)> _
        Public Class cColorCellVisualizer
            Inherits SourceGrid2.VisualModels.Common

            Private m_viz As frmPedigree.cPedigreeVisualizer = Nothing

            Public Sub New(ByVal viz As frmPedigree.cPedigreeVisualizer)
                Me.m_viz = viz
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Overidden to render cell value as a color
            ''' </summary>
            ''' -------------------------------------------------------------------
            Protected Overrides Sub DrawCell_ImageAndText( _
                    ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                    ByVal pos As SourceGrid2.Position, _
                    ByVal e As System.Windows.Forms.PaintEventArgs, _
                    ByVal rcClient As System.Drawing.Rectangle, _
                    ByVal status As SourceGrid2.DrawCellStatus)

                'If Not (TypeOf value Is Color) Then Return

                'Dim clr As Color = DirectCast(value, Color)
                'Dim rcColor As New Rectangle(rcClient.X + 2, rcClient.Y + 2, rcClient.Width - 4, rcClient.Height - 4)

                '' Draw the background
                'Using br As New SolidBrush(clr)
                '    e.Graphics.FillRectangle(br, rcColor)
                'End Using

            End Sub

        End Class


        Protected Overrides Sub FillData()

            ' Populate grid with cells of different types based on settings:
            '   - ColorCell
            '   - Single cell
            '   - Integer cell

        End Sub

    End Class

End Namespace
