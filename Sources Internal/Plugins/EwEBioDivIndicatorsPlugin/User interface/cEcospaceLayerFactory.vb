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

Imports System.Drawing
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to generate and style the <see cref="cDisplayLayer">display layer</see> that 
''' will be used to display Ecospace indicators.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerFactory
    Inherits cLayerFactoryBase

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the layer for displaying Ecospace indicators in this plug-in.
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext"/> to operate onto.</param>
    ''' <param name="ind">The <see cref="cIndicatorSettings.cIndicatorInfo">indicator information</see>
    ''' to create the layer for.</param>
    ''' <param name="data">The data that holds the computed values for the indicator.</param>
    ''' <returns>A <see cref="cDisplayLayer"/>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overloads Function GetLayer(ByVal uic As cUIContext, _
                                       ByVal ind As cIndicatorSettings.cIndicatorInfo, ByVal data As Single(,)) As  _
                                   ScientificInterfaceShared.Controls.Map.Layers.cDisplayLayer

        Dim lLayers As New List(Of cDisplayLayer)

        Dim layer As cDisplayLayer = Nothing
        Dim layerData As New cEcospaceLayerSingle(uic.Core, data, ind.Name)
        Dim vs As New cVisualStyle()
        Dim renderer As New cLayerRendererValue(vs)

        vs.ForeColour = Color.Black
        vs.BackColour = Color.Transparent
        renderer.DrawAlways = True

        Return New cDisplayRasterLayer(uic, layerData, renderer, Nothing)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cLayerFactoryBase.GetLayerGroup"/>
    ''' -----------------------------------------------------------------------
    Public Overloads Function GetLayerGroup(ByVal ind As cIndicatorSettings.cIndicatorInfoGroup) As String
        Return ind.Name
    End Function

End Class
