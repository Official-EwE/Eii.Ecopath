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

Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core
Imports System.Drawing

''' ---------------------------------------------------------------------------
''' <summary>
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)>
Friend Class cAvgHighlightVisualizer
    Inherits cEwEGridVisualizerBase

    Public Sub New(sg As cStyleGuide)
        Me.SG = sg
    End Sub

    Public Property Min As Single = 0
    Public Property Max As Single = 0
    Public ReadOnly Property SG As cStyleGuide = Nothing

    Protected Overrides Sub DrawCell_Background(p_Cell As SourceGrid2.Cells.ICellVirtual, p_CellPosition As SourceGrid2.Position, e As System.Windows.Forms.PaintEventArgs, p_ClientRectangle As System.Drawing.Rectangle, p_Status As SourceGrid2.DrawCellStatus)

        Dim sValue As Single = CSng(p_Cell.GetValue(p_CellPosition))
        Dim ramp As cColorRamp = Me.SG.DefaultColorRamp

        ' Render back colour
        Dim clrBack As Color = ramp.GetColor(sValue - Min, Max - Min)
        ' Draw the background
        Using br As New SolidBrush(clrBack)
            e.Graphics.FillRectangle(br, p_ClientRectangle)
        End Using
        ' Done
        Return

        ' Rever to default
        MyBase.DrawCell_Background(p_Cell, p_CellPosition, e, p_ClientRectangle, p_Status)
    End Sub
End Class
