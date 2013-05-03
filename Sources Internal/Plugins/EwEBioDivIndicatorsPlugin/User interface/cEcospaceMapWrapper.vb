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
Imports System.Drawing
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to update the map that reflects Ecospace biodiversity indicators.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceMapWrapper

#Region " Private variables "

    ''' <summary>UIContext to operate onto.</summary>
    Private m_uic As cUIContext = Nothing
    ''' <summary>Map to operate onto.</summary>
    Private m_map As ucMapZoom = Nothing
    ''' <summary>Map toolbar that may make our life interesting.</summary>
    Private m_toolbar As ucMapZoomToolbar = Nothing

    ''' <summary>Settings to use in the map.</summary>
    Private m_settings As cIndicatorSettings = Nothing
    ''' <summary>Computed Ecospace indicators, organized by point (col, row).</summary>
    Private m_dtIndicators As Dictionary(Of Point, cEcospaceIndicators)

    ''' <summary>Ecospace depth layer for finding water cells and for showing context.</summary>
    Private m_layerDepth As cLayer = Nothing
    ''' <summary>Indicator layer data.</summary>
    Private m_layerData As cLayer = Nothing

#End Region ' Private variables

#Region " Attach + detach "

    Public Sub Attach(ByVal uic As cUIContext, _
                      ByVal indicators As Dictionary(Of Point, cEcospaceIndicators), _
                      ByVal settings As cIndicatorSettings, _
                      ByVal toolbar As ucMapZoomToolbar, _
                      ByVal map As ucMapZoom)

        Me.m_uic = uic
        Me.m_settings = settings
        Me.m_dtIndicators = indicators
        Me.m_map = map
        Me.m_toolbar = toolbar

        Me.m_map.UIContext = Me.m_uic
        Me.m_toolbar.UIContext = Me.m_uic

        Me.m_toolbar.AddZoomContainer(Me.m_map)

    End Sub

    Public Sub Detach()

        Me.m_map.Map.Clear()

        Me.m_toolbar.RemoveZoomContainer(Me.m_map)

        Me.m_map.UIContext = Nothing
        Me.m_toolbar.UIContext = Nothing

        Me.m_settings = Nothing
        Me.m_dtIndicators = Nothing
        Me.m_map = Nothing
        Me.m_toolbar = Nothing
        Me.m_uic = Nothing

    End Sub

#End Region ' Attach + detach

#Region " Refreshing "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show an indicator in the map.
    ''' </summary>
    ''' <param name="info">The <see cref="cIndicatorSettings.cIndicatorInfo"/> to show.</param>
    ''' <param name="indEcopath">The calculated Ecopath indicators that may be used as a baseline. Not used
    ''' right now, but may come in pretty handy in the future if we decide to show indicators relative to
    ''' an Ecopath baseline.</param>
    ''' -----------------------------------------------------------------------
    Public Sub RefreshContent(ByVal info As cIndicatorSettings.cIndicatorInfo, indEcopath As cIndicators)

        ' Sanity check
        If (Me.m_uic Is Nothing) Then Return

        Dim bHasEcospace As Boolean = Me.m_uic.Core.StateMonitor.HasEcospaceLoaded

        ' Detach from previously loaded data
        If (Me.m_layerData IsNot Nothing) Then
            Me.m_map.Map.RemoveLayer(Me.m_layerData)
            Me.m_layerData.Dispose()
            Me.m_layerData = Nothing
        End If

        ' Ecospace may have been unloaded
        If (Not bHasEcospace) Then
            ' Clean up
            Me.m_map.Map.RemoveLayer(Me.m_layerDepth)
            Me.m_layerDepth = Nothing
            Me.m_map.Map.Visible = False
            ' Done
            Return
        End If

        ' Create new layer and populate it
        Dim fact As New cEcospaceLayerFactory()
        Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
        Dim asData(bm.InRow, bm.InCol) As Single
        Dim sValue As Single = 0
        Dim ind As cIndicators = Nothing

        ' Add depth layer
        If (Me.m_layerDepth Is Nothing) Then
            Me.m_layerDepth = fact.GetLayers(Me.m_uic, eVarNameFlags.LayerDepth)(0)
            Me.m_map.Map.AddLayer(Me.m_layerDepth)
            Me.m_map.Refresh()
        End If

        If (info IsNot Nothing) Then

            ' Populate result array from computed indicators
            For Each pt As Point In Me.m_dtIndicators.Keys
                ind = Me.m_dtIndicators(pt)
                sValue = info.GetValue(ind)
                asData(pt.Y, pt.X) = sValue
            Next
            ' Build wrapper layer
            Me.m_layerData = fact.GetLayer(Me.m_uic, info, asData)
            ' Add
            Me.m_map.Map.AddLayer(Me.m_layerData, Me.m_layerDepth)

        End If

        ' Done, update map
        Me.m_map.Map.Refresh()
        Me.m_map.Map.Visible = True

    End Sub

#End Region ' Refreshing

End Class
