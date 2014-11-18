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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Option Explicit On
Option Strict On


Imports EwECore
Imports System.IO
Imports System.Threading


Public Class cLayerFilePair
    Public MapLayer As IEnviroInputMap
    Public File As String

    Public Sub New(layer As IEnviroInputMap, FileName As String)
        MapLayer = layer
        File = FileName
    End Sub

End Class


Public Class cRunParameters

    Public BoundsOutput As String
    Public RemovalOutput As String
    Public lstRemovalLayers As List(Of IEnviroInputMap)
    Public lstBoundsFiles As List(Of cLayerFilePair)
    Public Delta As Single

    Public Const DEPTH_FILE As String = "depth_after.asc"
    Public Const SALINITY_FILE As String = "sal_50p_avg_after.asc"
    Public Const WAVE_FILE As String = "Waveheight 90p_avg.asc"
    Public Const CURRENT_FILE As String = "Ubot 90p_avg.asc"

    Public ReadOnly Property LowerBound As Single
        Get
            Dim temp As Single = 1 - Delta
            If temp < 0 Then temp = 0
            Return temp
        End Get
    End Property

    Public ReadOnly Property UpperBound As Single
        Get
            Return 1 + Delta
        End Get
    End Property


    Private m_core As cCore

    Public Sub New(theCore As cCore)
        Me.BoundsOutput = "Ecospace_Avg_Biomass_InputFactors.csv"
        Me.RemovalOutput = "Ecospace_Avg_Bomass_GroupResponses.csv"
        Me.m_core = theCore
        Me.setDefaults()

    End Sub

    Private Sub setDefaults()

        'Use the current Ecospace configuration
        'as the default years
        'RunTimes = New cRunPeriods(Me.m_core)
        Delta = 0.2 ' 0.2
        Me.setDefaultMapLayers()
        Me.setDefaultLayerFiles()
        Me.HardwireFileNames()

    End Sub

    Private Sub setDefaultMapLayers()
        Dim mapManager As cMapResponseInteractionManager = Me.m_core.CapacityMapInteractionManager
        Dim map As IEnviroInputMap = Nothing

        Me.lstRemovalLayers = New List(Of IEnviroInputMap)
        For iMap As Integer = 1 To mapManager.nMaps
            'Not Depth or Hard sediment
            If Not mapManager.Map(iMap).Layer.Name.Trim.ToLower.Contains("hard sediment") Then
                Me.lstRemovalLayers.Add(mapManager.Map(iMap))
            End If

        Next iMap

    End Sub


    Private Sub setDefaultLayerFiles()

        Me.lstBoundsFiles = New List(Of cLayerFilePair)
        For Each layer As IEnviroInputMap In Me.lstRemovalLayers
            Me.lstBoundsFiles.Add(New cLayerFilePair(layer, Nothing))
        Next

    End Sub


    Private Sub HardwireFileNames()
        For Each pair As cLayerFilePair In Me.lstBoundsFiles

            If pair.MapLayer.Layer.Name.Contains("Salinity") Then
                pair.File = SALINITY_FILE

            ElseIf pair.MapLayer.Layer.Name.Contains("Wave") Then
                pair.File = WAVE_FILE

            ElseIf pair.MapLayer.Layer.Name.Contains("Depth") Then
                pair.File = DEPTH_FILE

            ElseIf pair.MapLayer.Layer.Name.Contains("Ubot") Then
                pair.File = CURRENT_FILE

            End If

        Next
    End Sub

End Class

