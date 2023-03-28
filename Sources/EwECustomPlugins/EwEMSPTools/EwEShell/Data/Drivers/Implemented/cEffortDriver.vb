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
Imports EwEShell
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Driver for inserting MSP pressure data into the <see cref="cEcospaceFleet.TotalEffMultiplier">effort multiplier</see>
''' of a single <see cref="cEcospaceFleet">Ecospace fleet</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEffortDriver
    Inherits cDriver

#Region " Private vars "

    Private m_fleet As cEcopathFleetInput = Nothing
    Private Const cTINY_NUM = 1.0E-20

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
        MyBase.New(core, game, cStringUtils.Localize(My.Resources.DRIVER_EFFORT_NAME, fleet.Name))
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

        If (data IsNot Nothing) Then
            data.SEmult(Me.m_fleet.Index) = Math.Max(cTINY_NUM, Math.Min(1, Math.Max(pressure.Scalar, 0)) * multiplier)
        Else
            Dim flt As cEcospaceFleetInput = Me.m_core.EcospaceFleetInputs(Me.m_fleet.Index)
            flt.TotalEffMultiplier = Math.Max(cTINY_NUM, Math.Min(1, Math.Max(pressure.Scalar, 0)) * multiplier)
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
    ''' Returns that this driver can only be driven by scalar data.
    ''' </summary>
    ''' <returns>The supported <see cref="cPressure.eDataTypes">pressure type</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function DataType() As cPressure.eDataTypes
        Return cPressure.eDataTypes.Scalar
    End Function

End Class
