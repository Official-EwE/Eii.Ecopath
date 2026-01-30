' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Style
Imports SourceGrid2



''' -------------------------------------------------------------------
''' <summary>
''' A cell visualizer that renders a <see cref="cShapeData">shape</see>
''' into the cell.
''' </summary>
''' -------------------------------------------------------------------

Public Class cEwEGridShapeThumbnailVisualizer
    Inherits VisualModels.Common

#Region " Private vars "

    ''' <summary>We have to start somewhere, no?</summary>
    Private m_clr As Color = Color.Cornsilk

#End Region ' Private vars

#Region " Constructor "

    Public Sub New(clr As Color)
        Me.m_clr = clr
    End Sub

#End Region ' Constructor

#Region " Overrides "

    Protected Overrides Sub DrawCell_ImageAndText(cell As SourceGrid2.Cells.ICellVirtual,
                                                  pos As SourceGrid2.Position,
                                                  e As System.Windows.Forms.PaintEventArgs,
                                                  rcClient As System.Drawing.Rectangle,
                                                  status As SourceGrid2.DrawCellStatus)


        Dim shape As cShapeData = DirectCast(cell.GetValue(pos), cShapeData)

        If (shape Is Nothing) Then Return

        Dim grid As cEwEGrid = DirectCast(cell.Grid, cEwEGrid)
        Dim rcBmp As New Rectangle(0, 0, rcClient.Width, rcClient.Height)
        Dim img As New Bitmap(rcClient.Width, rcClient.Height)

        Using g As Graphics = Graphics.FromImage(img)
            cShapeImage.DrawShape(grid.UIContext, shape, rcBmp, g, Me.m_clr, eSketchDrawModeTypes.Line)
        End Using

        e.Graphics.DrawImage(img, rcClient.Location)
        img.Dispose()

    End Sub

    Protected Overrides Sub DrawCell_Background(cell As SourceGrid2.Cells.ICellVirtual,
                                                pos As SourceGrid2.Position,
                                                e As System.Windows.Forms.PaintEventArgs,
                                                rcClient As System.Drawing.Rectangle,
                                                status As SourceGrid2.DrawCellStatus)
        Dim grid As cEwEGrid = DirectCast(cell.Grid, cEwEGrid)
        Using br As New SolidBrush(grid.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND))
            e.Graphics.FillRectangle(br, rcClient)
        End Using

    End Sub

#End Region ' Overrides

End Class
