#Region " Imports "

Option Strict On
Imports SourceGrid2
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports System.Drawing
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' -------------------------------------------------------------------
''' <summary>
''' A cell visualizer that renders a <see cref="cShapeData">shape</see>
''' into the cell.
''' </summary>
''' -------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cEwEGridShapeThumbnailVisualizer
    Inherits VisualModels.MultiImages

#Region " Private vars "

    ''' <summary>We have to start somewhere, no?</summary>
    Private m_clr As Color = Color.Cornsilk

#End Region ' Private vars

#Region " Constructor "

    Public Sub New(ByVal clr As Color)
        Me.m_clr = clr
    End Sub

#End Region ' Constructor

#Region " Overrides "

    Protected Overrides Sub DrawCell_ImageAndText(ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                                                  ByVal pos As SourceGrid2.Position, _
                                                  ByVal e As System.Windows.Forms.PaintEventArgs, _
                                                  ByVal rcClient As System.Drawing.Rectangle, _
                                                  ByVal status As SourceGrid2.DrawCellStatus)


        Dim shape As cShapeData = DirectCast(cell.GetValue(pos), cShapeData)

        If (shape Is Nothing) Then Return

        Dim grid As EwEGrid = DirectCast(cell.Grid, EwEGrid)
        Dim rcBmp As New Rectangle(0, 0, rcClient.Width, rcClient.Height)
        Dim img As New Bitmap(rcClient.Width, rcClient.Height)

        Using g As Graphics = Graphics.FromImage(img)
            cShapeImage.DrawShape(grid.UIContext, shape, rcBmp, g, Me.m_clr, ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.LineSelective)
        End Using

        e.Graphics.DrawImage(img, rcClient.Location)
        img.Dispose()

    End Sub

    Protected Overrides Sub DrawCell_Background(ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                                                ByVal pos As SourceGrid2.Position, _
                                                ByVal e As System.Windows.Forms.PaintEventArgs, _
                                                ByVal rcClient As System.Drawing.Rectangle, _
                                                ByVal status As SourceGrid2.DrawCellStatus)
        Dim grid As EwEGrid = DirectCast(cell.Grid, EwEGrid)
        Using br As New SolidBrush(grid.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND))
            e.Graphics.FillRectangle(br, rcClient)
        End Using

    End Sub

#End Region ' Overrides

End Class
