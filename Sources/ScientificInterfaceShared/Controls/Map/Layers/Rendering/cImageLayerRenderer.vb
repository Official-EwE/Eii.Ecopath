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
        Public Sub New(uic As cUIContext, vs As cVisualStyle,
                       Optional layerStyleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet)
            MyBase.New(uic, vs, layerStyleFlags)
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
        Public Overrides Sub Render(g As Graphics,
                                    layer As cDisplayLayer,
                                    rc As RectangleF,
                                    ptfTL As PointF,
                                    ptfBR As PointF,
                                    style As cStyleGuide.eStyleFlags)

            If (Not TypeOf layer Is cDisplayLayerImage) Then Return

            Dim bml As cDisplayLayerImage = DirectCast(layer, cDisplayLayerImage)
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

        Public Overrides Sub RenderPreview(g As Graphics,
                                           rc As RectangleF,
                                           Optional iSymbol As Integer = 0)
            g.DrawImage(My.Resources.map, rc)
        End Sub

        Public Overrides Function GetDisplayText(value As Object) As String
            Return ""
        End Function

    End Class

End Namespace
