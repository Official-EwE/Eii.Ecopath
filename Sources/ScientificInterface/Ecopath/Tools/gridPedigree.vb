#Region " Imports "

Option Strict On

Imports EwECore

#End Region 'Imports

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

        Public Sub New(ByVal viz As frmPedigree.cPedigreeVisualizer)

        End Sub
        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to render cell value as a color
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_ImageAndText( _
                ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                ByVal p_CellPosition As SourceGrid2.Position, _
                ByVal e As System.Windows.Forms.PaintEventArgs, _
                ByVal p_ClientRectangle As System.Drawing.Rectangle, _
                ByVal p_Status As SourceGrid2.DrawCellStatus)

            Dim value As Object = p_Cell.GetValue(p_CellPosition)

            If Not (TypeOf value Is Color) Then Return

            Dim clr As Color = DirectCast(value, Color)
            Dim rcColor As New Rectangle(p_ClientRectangle.X + 2, p_ClientRectangle.Y + 2, p_ClientRectangle.Width - 4, p_ClientRectangle.Height - 4)

            ' Draw the background
            Using br As New SolidBrush(clr)
                e.Graphics.FillRectangle(br, rcColor)
            End Using

        End Sub

    End Class

    'Public Class cPedigreeCell
    '    Inherits EwECell

    '    Public Sub New(ByVal lvl As cPedigreeLevel, _
    '                   ByVal vis As frmPedigree.cPedigreeVisualizer)
    '        MyBase.New(
    '    End Sub

    'End Class

    'Protected Overrides Sub FillData()

    'End Sub

    Protected Overrides Sub FillData()

    End Sub

End Class
