' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwEUtils.Utilities



' For convenience reasons, effort intensity and ecological fishing is handled in the same class
' This poses problems for the emulator, where only one value can be passed in. Bugger.

' The most proper solution is to have two pressures operating on the same fleet: FleetEffort and FleetEcological. 
' Clunky but consistent, and without having to make any other inconsistent changes.

''' ---------------------------------------------------------------------------
''' <summary>
''' Driver for inserting MSP fishing pressure data into the running EwE model for 
''' a single <see cref="cEcospaceFleet">Ecospace fleet</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cFleetEcoDriver
    Inherits cDriver

#Region " Private vars "

    Private m_fleet As cEcopathFleetInput = Nothing
    Private Const cTINY_NUM = 1.0E-20
    Private m_penaltyvalue As Single = 0

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cFleetEcoDriver"/> to drive ecological fishing.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to connect to.</param>
    ''' <param name="game">The <see cref="cGame"/> to connect to.</param>
    ''' <param name="fleet">The <see cref="cEcospaceFleet">fleet</see> this driver is connected to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, game As cGame, fleet As cEcopathFleetInput)
        MyBase.New(core, game, cStringUtils.Localize(My.Resources.DRIVER_ECOLOGICAL, fleet.Name))
        Me.m_fleet = fleet

        ' Calculate cost penalty to impose on bycatch when a fleet is fishing ecologically
        ' The penalty value is determined as a multipler of the combined off-vessel value for all targeted species
        Dim sTotOffVesselValue As Single = 0

        For igrp As Integer = 1 To Me.m_core.nGroups
            If ((fleet.Landings(igrp) + fleet.Discards(igrp)) > 0) And (fleet.OffVesselValue(igrp) > 0) Then
                sTotOffVesselValue += fleet.OffVesselValue(igrp)
            End If
        Next
        Me.m_penaltyvalue = -1 * Math.Abs((Me.m_game.BycatchCostMultiplier * sTotOffVesselValue))

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

        If (TypeOf (pressure) IsNot cFishingEcoPressure) Then Return False

        Dim fep As cFishingEcoPressure = DirectCast(pressure, cFishingEcoPressure)

        If (bDirect) Then
            For Each i As Integer In Me.BycatchGroups
                Me.m_core.EcopathDataStructures.Market(Me.m_fleet.Index, i) = If(fep.bIsEcological, Me.m_penaltyvalue, 0)
            Next
        Else
            For Each i As Integer In Me.BycatchGroups
                ' JS31Jan24: Cannot insert a negative price via the Core IO ojects; the metadata prevents this. Let's discuss with Joe and Villy
                ' Me.m_core.EcopathFleetInputs(Me.m_fleet.Index).OffVesselValue(i) = If(fp.bIsEcological, Me.m_penaltyvalue, 0)
                Me.m_core.EcopathDataStructures.Market(Me.m_fleet.Index, i) = If(fep.bIsEcological, Me.m_penaltyvalue, 0)
            Next
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
    ''' Returns the groups that this fleet is bycatching (e.g, with no off-vessel value).
    ''' </summary>
    ''' <returns>An array with <see cref="ICoreGroup.Index">indices</see> representing
    ''' the groups that this fleet is bycatching.</returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property BycatchGroups As Integer()
        Get
            Dim flt As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(Me.m_fleet.Index)
            Dim lGrps As New List(Of Integer)
            For igrp As Integer = 1 To Me.m_core.nGroups
                If ((flt.Landings(igrp) + flt.Discards(igrp)) > 0) And (flt.OffVesselValue(igrp) <= 0) Then
                    lGrps.Add(igrp)
                End If
            Next
            Return lGrps.ToArray()
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
    ''' Returns that this driver can only be driven by scalar data.
    ''' </summary>
    ''' <returns>The supported pressure type.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function PressureType() As Type
        Return GetType(cFishingEcoPressure)
    End Function

End Class
