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
    ''' <remarks>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cBiomassRelativeAdapter
        Inherits cSpatialScalarDataAdapterBase

#Region " Private vars "

        Private m_spaceData As cEcospaceDataStructures

        ''' <summary>Ragged array used to store the base map by Layer</summary>
        Private m_baseLayers()(,) As Single
        ''' <summary>Has the base map for this layer been initialized?</summary>
        Private m_IsBaseInitialized() As Boolean

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        Public Overrides Sub InitRun()
            MyBase.InitRun()

            'Called at the start of each run
            'Allocate arrays for the base layers and boolean flags 
            Dim n As Integer = Me.m_core.GetCoreCounter(m_coreCounter)
            'Just allocate the layers array 
            'Each map will be initialized once on the first call
            m_baseLayers = New Single(n)(,) {}
            m_IsBaseInitialized = New Boolean(n) {}

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Friend Overrides Sub Initialize()
            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcoSpaceData
            Dim n As Integer = Me.m_core.GetCoreCounter(eCoreCounterTypes.nGroups)

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
            Try
                'Debug.Assert(Me.DataScaleType(layer.Index, iConnection) = eScaleType.Relative, Me.ToString + ".SetCell() Warning scale type should be 'Relative'")

                If sValueAtT <> cCore.NULL_VALUE Then
                    'External data is the pattern of biomass distribution relative to the Ecospace base biomass
                    'B = [B base at t=zero] * [B external] * [1/mean B external at t=zero]
                    layer.Cell(iRow, iCol) = CDbl(Me.m_baseLayers(layer.Index)(iRow, iCol)) * sValueAtT * conn.Scale
                Else
                    layer.Cell(iRow, iCol) = sValueAtT
                End If

                Return True

            Catch ex As Exception
                Debug.Assert(False, Me.ToString + ".SetCell() Exception: " + ex.Message)
            End Try

            Return False

        End Function


        Protected Friend Overrides Function Adapt(ByVal bm As cEcospaceBasemap, ByVal layer As cEcospaceLayer,
                                                  ByVal conn As cSpatialDataConnection, ByVal iTime As Integer, ByVal dt As Date,
                                                  ByVal dataExternal As ISpatialRaster, ByVal dNoData As Double) As Boolean

            Try
                'This is a "First Chance" initialization of the base layers 
                If Not Me.m_IsBaseInitialized(layer.Index) Then
                    Me.InitializeBaseLayer(layer.Index)
                End If

            Catch ex As Exception
                'Ok what now....
                'I don't think this can happen. Really I promise...
                Debug.Assert(False, Me.ToString + ".Adapt() Exception: " + ex.Message)
                Return False
            End Try

            Return MyBase.Adapt(bm, layer, conn, iTime, dt, dataExternal, dNoData)

        End Function

#End Region ' Overrides

#Region "Internal methods"

        ''' <summary>
        ''' Copy the base map from the layer for this 
        ''' </summary>
        ''' <param name="iLayer"></param>
        ''' <remarks>First Chance initialization. This should only be called once.</remarks>
        Private Sub InitializeBaseLayer(iLayer As Integer)

            Debug.Assert(Me.m_IsBaseInitialized(iLayer) = False, Me.ToString + ".InitializeBaseLayer() already initialized! It should be be called again for this layer.")

            Try

                Dim n As Integer = Me.m_core.GetCoreCounter(m_coreCounter)
                Dim layer() As cEcospaceLayer = Me.m_core.EcospaceBasemap.Layers(Me.m_varName)

                'Allocate base map storeage for this layer
                m_baseLayers(iLayer) = New Single(m_spaceData.InRow, m_spaceData.InCol) {}
                'Copy the data from the layer into the base layers
                'used to scale the values relative to the Ecospace base
                For ir As Integer = 1 To m_spaceData.InRow
                    For ic As Integer = 1 To m_spaceData.InCol
                        m_baseLayers(iLayer)(ir, ic) = CSng(layer(iLayer - 1).Cell(ir, ic))
                    Next
                Next

                Me.m_IsBaseInitialized(iLayer) = True

            Catch ex As Exception
                Me.m_IsBaseInitialized(iLayer) = False
                cLog.Write(ex, Me.ToString + ".InitializeBaseLayer() Failed to save base map layer.")
            End Try

        End Sub

#End Region

    End Class

End Namespace
