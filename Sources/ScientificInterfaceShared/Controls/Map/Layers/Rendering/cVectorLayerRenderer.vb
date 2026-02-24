' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for rendering a <see cref="cDisplayLayer">display layer</see>
    ''' onto the base map.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cVectorLayerRenderer
        Inherits cLayerRenderer

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vs"></param>
        ''' <param name="layerStyleFlags"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext, vs As cVisualStyle,
                       Optional layerStyleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet)
            MyBase.New(uic, vs, layerStyleFlags)
        End Sub

#End Region ' Construction / destruction

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Render vector data.
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="layer"></param>
        ''' <param name="ClipRectangle">Screen rectangle to draw onto</param>
        ''' <param name="ClipTLLonLat">Top-left coordinate, in actual basemap coordinates, of the screen rectangle.</param>
        ''' <param name="ClipBRLonLat">Bottom-right coordinate, in actual basemap coordinates, of the screen rectangle.</param>
        ''' <param name="style"></param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Render(g As Graphics, layer As cDisplayLayer, ClipRectangle As RectangleF, ClipTLLonLat As PointF, ClipBRLonLat As PointF, style As cStyleGuide.eStyleFlags)
            ' NOP
        End Sub

    End Class

End Namespace
