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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
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

#End Region ' Imports

''' -------------------------------------------------------------------
''' <summary>
''' Panic!
''' </summary>
''' -------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cAlertVisualizer
    Inherits VisualModels.Common

#Region " Overrides "

    Protected Overrides Sub DrawCell_ImageAndText(ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                                                  ByVal pos As SourceGrid2.Position, _
                                                  ByVal e As System.Windows.Forms.PaintEventArgs, _
                                                  ByVal rcClient As System.Drawing.Rectangle, _
                                                  ByVal status As SourceGrid2.DrawCellStatus)

        If Not CBool(cell.GetValue(pos)) Then
            Dim rc As New Rectangle(Math.Max(0, rcClient.X - 8 + CInt(rcClient.Width / 2)), rcClient.Y, 16, 16)
            e.Graphics.DrawImage(ScientificInterfaceShared.My.Resources.Critical, rc)
        End If

    End Sub

#End Region ' Overrides

End Class
