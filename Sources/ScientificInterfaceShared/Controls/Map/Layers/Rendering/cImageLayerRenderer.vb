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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for rendering a <see cref="cDisplayLayer">display layer</see>
    ''' onto the base map.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cImageLayerRenderer
        Inherits cVectorLayerRenderer

#Region " Construction / destruction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vs"></param>
        ''' <param name="layerStyleFlags"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal vs As cVisualStyle, _
                       Optional ByVal layerStyleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet)
            MyBase.New(vs, layerStyleFlags)
        End Sub

#End Region ' Construction / destruction

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render a cell of a layer.
        ''' </summary>
        ''' <param name="g">The graphics to render onto.</param>
        ''' <param name="rc">Device area to render cell onto.</param>
        ''' <param name="layer">Layer to render.</param>
        ''' <param name="ptfTL">Top-left corner (lon, lat) represented by <paramref name="rc"/>.</param>
        ''' <param name="ptfBR">Bottom-right corner (lon, lat) represented by <paramref name="rc"/>.</param>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub Render(ByVal g As Graphics, _
                                       ByVal layer As cDisplayLayer, _
                                       ByVal rc As Rectangle, _
                                       ByVal ptfTL As PointF, _
                                       ByVal ptfBR As PointF, _
                                       ByVal style As cStyleGuide.eStyleFlags)

            If (Not TypeOf layer Is cDisplayImageLayer) Then Return

            Dim bml As cDisplayImageLayer = DirectCast(layer, cDisplayImageLayer)
            Dim img As Image = bml.Image
            Dim imgTL As PointF = bml.ImageTL
            Dim imgBR As PointF = bml.ImageBR
            Dim sScaleX As Single = (rc.Width / (ptfBR.X - ptfTL.X))
            Dim sScaleY As Single = (rc.Height / (ptfTL.Y - ptfBR.Y))

            If (img Is Nothing) Then Return

            g.ScaleTransform(sScaleX, sScaleY)
            g.DrawImage(img, (imgTL.X - ptfTL.X), (ptfTL.Y - imgTL.Y), Math.Abs(imgBR.X - imgTL.X), Math.Abs(imgBR.Y - imgTL.Y))
            g.ResetTransform()

        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics,
                                           ByVal rc As Rectangle,
                                           Optional iSymbol As Integer = 0)
            g.DrawImage(My.Resources.map, rc)
        End Sub

        Public Overrides Function GetDisplayText(value As Object) As String
            Return ""
        End Function

        Public Overrides ReadOnly Property nExtraSymbols As Integer
            Get
                Return 0
            End Get
        End Property

        Public Overrides ReadOnly Property SymbolName(iSymbol As Integer) As String
            Get
                Return ""
            End Get
        End Property

    End Class

End Namespace
