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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

#End Region 'Imports

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells as a coloured symbol, using the 
    ''' foreground colour in the attaced <see cref="cLayerRenderer.VisualStyle">visual style</see>
    ''' to fill the symbol.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererSymbol
        Inherits cRasterLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle)

            If Me.IsStyleValid() Then
                rc.Inflate(CInt(-rc.Width * 0.75), CInt(-rc.Height * 0.75))
                Using p As New Pen(Color.White, 3)
                    g.DrawEllipse(p, rc)
                End Using
                Using br As New SolidBrush(Me.VisualStyle.ForeColour)
                    g.FillEllipse(br, rc)
                End Using
                g.DrawEllipse(Pens.Black, rc)
            Else
                Me.RenderError(g, rc)
            End If

        End Sub

        Public Overrides Sub RenderCell(ByVal g As System.Drawing.Graphics, _
                                        ByVal rc As System.Drawing.Rectangle, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal value As Object, _
                                        ByVal style As cStyleGuide.eStyleFlags)
            Me.RenderPreview(g, rc)
        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            Return True
        End Function

    End Class

End Namespace