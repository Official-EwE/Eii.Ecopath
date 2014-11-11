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


Public Class cRunParameters

    Public OutputFileName As String
    'Public RunTimes As cRunPeriods
    Public lstLayers As List(Of IEnviroInputMap)
    Public Delta As Single

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
        Me.OutputFileName = "C:\Users\Joe\Documents\Projects\EwE\Ecopath6\Sources Internal\Plugins\EcospaceSensitivityAnalysis\B_out.csv"
        Me.m_core = theCore
        Me.setDefaults()

    End Sub

    Private Sub setDefaults()

        'Use the current Ecospace configuration
        'as the default years
        'RunTimes = New cRunPeriods(Me.m_core)
        Delta = 0.9 ' 0.2
        Me.setDefaultMapLayers()

    End Sub

    Private Sub setDefaultMapLayers()
        Dim mapManager As cMapResponseInteractionManager = Me.m_core.CapacityMapInteractionManager
        Dim map As IEnviroInputMap = Nothing

        Me.lstLayers = New List(Of IEnviroInputMap)
        For iMap As Integer = 1 To mapManager.nMaps
            'Not Depth or Hard sediment
            If Not mapManager.Map(iMap).Layer.Name.Trim.ToLower.Contains("hard sediment") Then
                Me.lstLayers.Add(mapManager.Map(iMap))
            End If

        Next iMap
    End Sub

End Class

