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

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    Public Class cHabitatDataAdapter
        Inherits cCapacityDataAdapter

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cCapacityDataAdapter.Adapt(cEcospaceBasemap, cEcospaceLayer, cSpatialDataConnection, Integer, Date, ISpatialRaster, Double)"/>
        ''' <remarks>
        ''' Overridden to invalidate fishing area assessments, if any.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Friend Overrides Function Adapt(bm As cEcospaceBasemap, layer As cEcospaceLayer, conn As cSpatialDataConnection, iTime As Integer, dt As Date, dataExternal As ISpatialRaster, dNoData As Double) As Boolean

            If Not MyBase.Adapt(bm, layer, conn, iTime, dt, dataExternal, dNoData) Then Return False

            Dim ih As Integer = layer.Index
            Debug.Assert(ih >= 1)

            Dim bInvalidate As Boolean = False
            For ig As Integer = 1 To Me.m_spaceData.nFleets
                bInvalidate = bInvalidate Or Me.m_spaceData.GearHab(ig, ih)
            Next

            If bInvalidate Then
                Me.m_spaceData.isFishingHabitatChanged = True
            End If

            Return True

        End Function

#End Region ' Overrides

    End Class

End Namespace
