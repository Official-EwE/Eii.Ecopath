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
