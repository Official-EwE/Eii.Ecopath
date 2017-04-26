Option Strict On
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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Imports System.Drawing
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

Public Class cTransectRasterDisplay
    Inherits cDisplayLayerRaster

    Private Shared m_vs As New cVisualStyle()

    Public Sub New(ByVal uic As cUIContext, ByVal data As cTransectLayer)
        MyBase.New(uic, data, New cLayerRendererHatch(m_vs), Nothing)
        m_vs.ForeColour = Color.FromArgb(128, Color.Orange)
        m_vs.BackColour = Color.FromArgb(128, Color.Black)
        m_vs.HatchStyle = Drawing2D.HatchStyle.Percent50
        Me.RenderMode = eLayerRenderType.Always
    End Sub

End Class
