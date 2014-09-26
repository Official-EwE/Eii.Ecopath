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

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data Adapter specific to Biomass forcing.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cBiomassForcingAdapter
        Inherits cSpatialScalarDataAdapterBase

#Region " Private vars "

        Private m_spaceData As cEcospaceDataStructures

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Warning this hardwires the scale value
        'so changing the scale in the interface has no affect
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
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

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.SetCell"/>.
        ''' <remarks>Overridden to scale values prior to being set in the 
        ''' Ecospace data structures.</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function SetCell(ByVal layer As cEcospaceLayer, _
                                             ByVal conn As cSpatialDataConnection, _
                                             ByVal iRow As Integer, _
                                             ByVal iCol As Integer, _
                                             ByVal sValueAtT As Double) As Boolean

            If (conn.ScaleType = eScaleType.Relative) Then
                sValueAtT *= conn.Scale
            End If
            Return MyBase.SetCell(layer, conn, iRow, iCol, sValueAtT)

        End Function

        Protected Overrides Function NewConnection() As cSpatialDataConnection
            Dim conn As New cSpatialDataConnection()
            'WARNING: These values get overwritten by the loading 
            'For now you can't hardwire an initial scaler value into an Adapter
            conn.Scale = Me.molesm2_to_kgkm2
            conn.ScaleType = eScaleType.Relative
            Return MyBase.NewConnection()
        End Function

#End Region ' Overrides

    End Class

End Namespace
