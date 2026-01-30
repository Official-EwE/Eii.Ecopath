' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace SpatialData

    Public Class cHabitatDataAdapter
        Inherits cCapacityDataAdapter

#Region " Constructor "

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
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
