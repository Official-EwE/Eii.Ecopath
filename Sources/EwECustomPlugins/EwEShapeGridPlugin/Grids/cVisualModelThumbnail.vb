' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports SourceGrid2
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports System.Drawing
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions



''' ---------------------------------------------------------------------------
''' <summary>
''' <see cref="SourceGrid2.VisualModels.Common">visual model</see> to show a
''' <see cref="cShapeData">shape</see> as a thumbnail.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cVisualModelThumbnail
    Inherits VisualModels.MultiImages

    Private m_handler As cShapeGUIHandler = Nothing

    Public Sub New(handler As cShapeGUIHandler)
        Me.m_handler = handler
    End Sub

    Protected Overrides Sub DrawCell_ImageAndText(cell As SourceGrid2.Cells.ICellVirtual, _
                                                  pos As SourceGrid2.Position, _
                                                  e As System.Windows.Forms.PaintEventArgs, _
                                                  rcClient As System.Drawing.Rectangle, _
                                                  status As SourceGrid2.DrawCellStatus)

        Dim shape As cShapeData = DirectCast(cell.GetValue(pos), cShapeData)
        Dim grid As cEwEGrid = DirectCast(cell.Grid, cEwEGrid)
        Dim rcBmp As New Rectangle(0, 0, rcClient.Width, rcClient.Height)
        Dim img As New Bitmap(rcClient.Width, rcClient.Height)

        Using g As Graphics = Graphics.FromImage(img)
            cShapeImage.DrawShape(grid.UIContext, shape,
                                  rcBmp, g,
                                  Me.m_handler.Color, Me.m_handler.SketchDrawMode(shape), Me.m_handler.XAxisMaxValue)
        End Using

        e.Graphics.DrawImage(img, rcClient.Location)
        img.Dispose()

    End Sub

    Protected Overrides Sub DrawCell_Background(cell As SourceGrid2.Cells.ICellVirtual, _
                                                pos As SourceGrid2.Position, _
                                                e As System.Windows.Forms.PaintEventArgs, _
                                                rcClient As System.Drawing.Rectangle, _
                                                status As SourceGrid2.DrawCellStatus)

        Dim grid As cEwEGrid = DirectCast(cell.Grid, cEwEGrid)
        Using br As New SolidBrush(grid.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND))
            e.Graphics.FillRectangle(br, rcClient)
        End Using

    End Sub

End Class
