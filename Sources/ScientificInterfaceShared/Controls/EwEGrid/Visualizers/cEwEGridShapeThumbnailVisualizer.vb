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

''' -------------------------------------------------------------------
''' <summary>
''' A cell visualizer that renders a <see cref="cShapeData">shape</see>
''' into the cell.
''' </summary>
''' -------------------------------------------------------------------
<CLSCompliant(False)> _
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

    Protected Overrides Sub DrawCell_ImageAndText(cell As SourceGrid2.Cells.ICellVirtual, _
                                                  pos As SourceGrid2.Position, _
                                                  e As System.Windows.Forms.PaintEventArgs, _
                                                  rcClient As System.Drawing.Rectangle, _
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

    Protected Overrides Sub DrawCell_Background(cell As SourceGrid2.Cells.ICellVirtual, _
                                                pos As SourceGrid2.Position, _
                                                e As System.Windows.Forms.PaintEventArgs, _
                                                rcClient As System.Drawing.Rectangle, _
                                                status As SourceGrid2.DrawCellStatus)
        Dim grid As cEwEGrid = DirectCast(cell.Grid, cEwEGrid)
        Using br As New SolidBrush(grid.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND))
            e.Graphics.FillRectangle(br, rcClient)
        End Using

    End Sub

#End Region ' Overrides

End Class
