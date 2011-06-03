#Region " Imports "

Option Strict On
Imports SourceGrid2
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports System.Drawing
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' <see cref="SourceGrid2.VisualModels.Common">visual model</see> to show a
''' <see cref="cShapeData">shape</see> as a thumbnail.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cVisualModelThumbnail
    Inherits VisualModels.MultiImages

    Private m_handler As cShapeGUIHandler = Nothing

    Public Sub New(ByVal handler As cShapeGUIHandler)
        Me.m_handler = handler
    End Sub

    Protected Overrides Sub DrawCell_ImageAndText(ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                                                  ByVal pos As SourceGrid2.Position, _
                                                  ByVal e As System.Windows.Forms.PaintEventArgs, _
                                                  ByVal rcClient As System.Drawing.Rectangle, _
                                                  ByVal status As SourceGrid2.DrawCellStatus)

        Dim shape As cShapeData = DirectCast(cell.GetValue(pos), cShapeData)
        Dim grid As EwEGrid = DirectCast(cell.Grid, EwEGrid)
        Dim rcBmp As New Rectangle(0, 0, rcClient.Width, rcClient.Height)
        Dim img As New Bitmap(rcClient.Width, rcClient.Height)

        Using g As Graphics = Graphics.FromImage(img)
            cShapeImage.DrawShape(grid.UIContext, shape, _
                                  rcBmp, g, _
                                  Me.m_handler.Color, Me.m_handler.SketchDrawMode)
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
        Using br As New SolidBrush(grid.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND))
            e.Graphics.FillRectangle(br, rcClient)
        End Using

    End Sub

End Class
