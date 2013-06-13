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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data Adapter specific to Capacity layers .
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cBiomassForcingAdapter
        Inherits cSpatialScalarDataAdapterBase


#Region " Private vars "

        Private m_spaceData As cEcospaceDataStructures

        '12 grams of carbon per mol 
        '9x for conversion of C to wet weight
        Dim molesm2_to_kgkm2 As Single = 12 * 9


#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Friend Overrides Sub Initialize()
            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcoSpaceData

        End Sub


        ' ''' -------------------------------------------------------------------
        ' ''' <inheritdocs cref="cSpatialScalarDataAdapter.Adapt"/>
        ' ''' <remarks>
        ' ''' Called before data from an external source is copied into <see cref="cEcospaceDataStructures.RelPP"/>
        ' ''' EcoSpace uses an internal scaler to scale PP data to Ecopath levels. <see cref="cEcospaceDataStructures.PPScale"/>
        ' ''' This is the mean relative PP across all water cells computed from the currently loaded  <see cref="cEcospaceDataStructures.RelPP"/> array.
        ' ''' <see cref="cSpatialScalarDataAdapter.SetCell"/> will scale external data to a the first timestep or a user defined value.
        ' ''' </remarks>
        ' ''' -------------------------------------------------------------------
        'Protected Friend Overrides Function Adapt(ByVal bm As cEcospaceBasemap, _
        '                                          ByVal layer As cEcospaceLayer, _
        '                                          ByVal iTime As Integer, _
        '                                          ByVal dt As Date, _
        '                                          ByVal dataExternal As ISpatialRaster) As Boolean



        '    'Return True
        '    Return MyBase.Adapt(bm, layer, iTime, dt, dataExternal)

        '    'Dim breturnVal As Boolean
        '    'Try
        '    '    '12 grams of carbon per mol 
        '    '    '9x for conversion of C to wet weight
        '    '    Dim molesm2_to_kgkm2 As Single = 12 * 9
        '    '    Dim igrp As Integer = layer.Index
        '    '    For ir As Integer = 1 To Me.m_spaceData.InRow
        '    '        For ic As Integer = 1 To Me.m_spaceData.InCol
        '    '            Me.m_spaceData.Bcell(ir, ic, igrp) = CSng(dataExternal.Cell(ir, ic)) * molesm2_to_kgkm2
        '    '        Next ic
        '    '    Next ir
        '    '    breturnVal = True

        '    'Catch ex As Exception
        '    '    System.Console.WriteLine("Exception in cBiomassForcingAdapter.Adapt() " + ex.Message)
        '    '    breturnVal = False
        '    'End Try

        '    'Return breturnVal

        'End Function


        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.SetCell"/>.
        ''' <remarks>Overridden to scale values prior to being set in the 
        ''' Ecospace data structures.</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function SetCell(ByVal layer As cEcospaceLayer, _
                                             ByVal iRow As Integer, _
                                             ByVal iCol As Integer, _
                                             ByVal sValueAtT As Double) As Boolean
            'For now the conversion is fixed at mol to kg
            'If (Me.DataScaleType(layer.Index) = eScaleType.Relative) Then
            '    sValueAtT /= Me.DataScale(layer.Index)
            'End If

            'convert from mol C /m2 to kg/km2
            sValueAtT *= Me.molesm2_to_kgkm2
            Return MyBase.SetCell(layer, iRow, iCol, sValueAtT)

        End Function

#End Region ' Overrides

    End Class

End Namespace
