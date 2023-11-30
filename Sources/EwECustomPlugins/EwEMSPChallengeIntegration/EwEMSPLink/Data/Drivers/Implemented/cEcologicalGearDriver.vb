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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Imports EwECore
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Driver for defining if a fleet is "ecological", which changes all discard mortalities to 0.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcologicalGearDriver
    Inherits cDriver

#Region " Private vars "

    Private m_fleet As cEcopathFleetInput = Nothing
    Private m_bEcological As Boolean = False

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cEffortDriver"/> to drive the <see cref="cEcospaceFleet.TotalEffMultiplier">
    ''' Ecospace effort multiplier</see> of a single fleet.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to connect to.</param>
    ''' <param name="game">The <see cref="cGame"/> to connect to.</param>
    ''' <param name="fleet">The <see cref="cEcospaceFleet">fleet</see> this driver is connected to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, game As cGame, fleet As cEcopathFleetInput)
        MyBase.New(core, game, cStringUtils.Localize(My.Resources.DRIVER_EFFORTMULTIPLIER_NAME, fleet.Name))
        Me.m_fleet = fleet
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Applies the specified fishing effort multiplier.
    ''' </summary>
    ''' <param name="pressure">The MEL-derived fishing effort multiplier value to apply to the driver.</param>
    ''' <param name="data">Optional Ecospace data structures to apply pressures to.</param>
    ''' <param name="multiplier">The effort multiplier which translate a MEL fishing effort pressure value (0 to 1) to an Ecospace
    ''' effort multiplier (0 to inf).</param>
    ''' <returns>Always true. Happy.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Apply(pressure As cPressure, Optional data As cEcospaceDataStructures = Nothing, Optional multiplier As Double = 1.0!) As Boolean

        If (pressure.Scalar < 0) Then Return True

        Dim bIsEcological As Boolean = (pressure.Scalar > 0)
        Dim SimDS As cEcosimDatastructures = Me.m_core.EcosimDataStructures
        Dim PathDS As cEcopathDataStructures = Me.m_core.EcopathDataStructures

        Dim iFlt As Integer = Me.m_fleet.Index

        For iGrp As Integer = 1 To Me.m_core.nGroups

            If (bIsEcological <> Me.m_bEcological) Then
                If (bIsEcological) Then

                    SimDS.PropDiscardMortTime(iFlt, iGrp) = 0
                    SimDS.PropDiscardTime(iFlt, iGrp) = 0
                Else
                    SimDS.PropDiscardMortTime(iFlt, iGrp) = PathDS.PropDiscardMort(iFlt, iGrp)
                    SimDS.PropDiscardTime(iFlt, iGrp) = PathDS.PropDiscard(iFlt, iGrp) * SimDS.PropDiscardMortTime(iFlt, iGrp)
                End If

                Me.m_bEcological = bIsEcological
            End If
        Next
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
            Return CDbl(False)
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
    ''' <returns>The supported <see cref="cPressure.eDataTypes">pressure type</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function DataType() As cPressure.eDataTypes
        Return cPressure.eDataTypes.Scalar
    End Function

End Class
