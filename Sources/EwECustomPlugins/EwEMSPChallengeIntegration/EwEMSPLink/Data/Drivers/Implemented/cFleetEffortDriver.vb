' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwEUtils.Utilities

''' ---------------------------------------------------------------------------
''' <summary>
''' Driver for inserting MSP fishing pressure data into the running EwE model for 
''' a single <see cref="cEcospaceFleet">Ecospace fleet</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cFleetEffortDriver
    Inherits cDriver

#Region " Private vars "

    Private m_fleet As cEcopathFleetInput = Nothing
    Private Const cTINY_NUM = 1.0E-20

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cFleetEffortDriver"/> to drive the <see cref="cEcospaceFleet.TotalEffMultiplier">
    ''' Ecospace effort multiplier</see> of a single fleet.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to connect to.</param>
    ''' <param name="game">The <see cref="cGame"/> to connect to.</param>
    ''' <param name="fleet">The <see cref="cEcospaceFleet">fleet</see> this driver is connected to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, game As cGame, fleet As cEcopathFleetInput)
        MyBase.New(core, game, cStringUtils.Localize(My.Resources.DRIVER_EFFORT, fleet.Name))
        Me.m_fleet = fleet
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Applies the specified fishing effort multiplier.
    ''' </summary>
    ''' <param name="pressure">The MEL-derived fishing effort multiplier value to apply to the driver.</param>
    ''' <param name="bDirect">Flag, indicating whether a value needs to be injected directly into the 
    ''' EwE data structures (true) or into the EwE input/output objects (false).</param>
    ''' <param name="multiplier">The effort multiplier which translate a MEL fishing effort pressure value (0 to 1) to an Ecospace
    ''' effort multiplier (0 to inf).</param>
    ''' <returns>Always true. Happy.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Apply(pressure As cPressure, bDirect As Boolean, Optional multiplier As Double = 1.0!) As Boolean

        If (TypeOf (pressure) IsNot cFishingEffortPressure) Then Return False

        Dim fp As cFishingEffortPressure = DirectCast(pressure, cFishingEffortPressure)

        If (bDirect) Then
            Me.m_core.EcospaceDataStructures.SEmult(Me.m_fleet.Index) = Math.Max(cTINY_NUM, Math.Min(1, Math.Max(fp.EffortScalar, 0)) * multiplier)
        Else
            Me.m_core.EcospaceFleetInputs(Me.m_fleet.Index).TotalEffMultiplier = Math.Max(cTINY_NUM, Math.Min(1, Math.Max(fp.EffortScalar, 0)) * multiplier)
        End If

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the effort multiplier configured in the base Ecospace model.
    ''' </summary>
    ''' <returns>The effort multiplier configured in the base Ecospace model.</returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property StartValue As Double
        Get
            Dim flt As cEcospaceFleetInput = Me.m_core.EcospaceFleetInputs(Me.m_fleet.Index)
            Return flt.TotalEffMultiplier
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the unique ID for the Ecospace <see cref="cEcospaceFleetInput">fleet</see>.
    ''' </summary>
    ''' <returns>The unique ID for the Ecospace <see cref="cEcospaceFleetInput">fleet</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ValueID() As String
        Return Me.m_fleet.GetID()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns that this driver can only be driven by fishing pressure data.
    ''' </summary>
    ''' <returns>The supported pressure type.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function PressureType() As Type
        Return GetType(cFishingEffortPressure)
    End Function

End Class
