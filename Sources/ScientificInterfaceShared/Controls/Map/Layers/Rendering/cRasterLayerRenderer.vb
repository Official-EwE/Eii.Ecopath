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
    Public MustInherit Class cRasterLayerRenderer
        Inherits cLayerRenderer

#Region " Construction / destruction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uic"></param>
        ''' <param name="vs"></param>
        ''' <param name="layerStyleFlags"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(uic As cUIContext, vs As cVisualStyle, Optional layerStyleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet)
            MyBase.New(uic, vs, layerStyleFlags)
        End Sub

#End Region ' Construction / destruction

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cLayerRenderer.Render"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub Render(g As System.Drawing.Graphics,
                                    layer As cDisplayLayer,
                                    rc As System.Drawing.RectangleF,
                                    ptfTL As System.Drawing.PointF,
                                    ptfBR As System.Drawing.PointF,
                                    style As Style.cStyleGuide.eStyleFlags)

            Throw New NotImplementedException("Invalid render mode")

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render a cell of a layer.
        ''' </summary>
        ''' <param name="g">The graphics to render onto.</param>
        ''' <param name="rc">Device area to render cell onto.</param>
        ''' <param name="layer">Layer to render from</param>
        ''' <param name="value">The value to render.</param>
        ''' -----------------------------------------------------------------------
        Public MustOverride Sub RenderCell(g As Graphics,
                                           rc As RectangleF,
                                           layer As cEcospaceLayer,
                                           value As Object,
                                           style As cStyleGuide.eStyleFlags)

        Public Property SuppressZero As Boolean

    End Class

End Namespace
