' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports EwECore.Auxiliary
Imports EwEUtils.UserInterface
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Style

Public Class cTransectRasterDisplay
    Inherits cDisplayLayerRaster

    ' ToDo: update visual style fore colour when styleguide changes

    Private Shared s_vs As New cVisualStyle()

    Public Sub New(uic As cUIContext, data As cTransectLayer)
        MyBase.New(uic, data, New cLayerRendererHatch(uic, s_vs), Nothing)

        Dim sg As cStyleGuide = uic.StyleGuide

        s_vs.HatchStyle = VisualHatchStyle.Percent50
        s_vs.ForeColour = VisualColor.FromArgb(128, sg.ApplicationColorInvariant(cStyleGuide.eApplicationColorType.HIGHLIGHT))
        s_vs.BackColour = VisualColor.FromArgb(128, VisualColor.FromArgb(0))

        Me.RenderMode = eLayerRenderType.Always
    End Sub

End Class
