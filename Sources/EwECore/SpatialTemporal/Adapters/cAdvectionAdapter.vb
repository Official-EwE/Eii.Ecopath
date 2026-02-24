' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace SpatialData

    ''' =======================================================================
    ''' <summary>
    ''' Adapter to populate the Advection core layer (not the monthly maps!)
    ''' Note that this adapter disables the use of monthly advection files when
    ''' external data is connected!
    ''' </summary>
    ''' <remarks>
    ''' A scalar is needed to have the ability to reverse or scale advection 
    ''' vectors.
    ''' </remarks>
    ''' =======================================================================
    Public Class cAdvectionAdapter
        Inherits cSpatialScalarDataAdapter

        Private m_spaceData As cEcospaceDataStructures

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Public Overrides Sub Initialize()
            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcospaceData
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States that this adapter cannot scale to a base value, as there is
        ''' no base velocity to scale to.
        ''' </summary>
        ''' <returns></returns>
        ''' <seealso cref="CalculateScalar(Double, Double)" />
        ''' -------------------------------------------------------------------
        Public Overrides Function CanCalculateScalar() As Boolean
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the run, overridden to disable some core logic if 
        ''' advection is connected to external data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub InitRun(bPreserveLayerData As Boolean)
            ' Is connected to external data?
            If Me.IsConnected(0) Or Me.IsConnected(1) Then
                ' #Yes: block the use of monthly advection vectors
                Me.m_spaceData.isAdvectionForced = True
            End If
            MyBase.InitRun(bPreserveLayerData)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' End the run, overridden to restore some core logic if 
        ''' advection is connected to external data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub EndRun()
            MyBase.EndRun()
            ' Is connected to external data?
            If Me.IsConnected(0) Or Me.IsConnected(1) Then
                ' #Yes: unblock the use of monthly advection vectors
                Me.m_spaceData.isAdvectionForced = False
            End If
        End Sub

    End Class

End Namespace
