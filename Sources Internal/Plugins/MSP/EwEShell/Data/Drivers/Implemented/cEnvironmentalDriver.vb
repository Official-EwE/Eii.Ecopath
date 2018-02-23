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
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Driver for inserting MSP pressure data into a single 
''' <see cref="cEcospaceLayerDriver">Ecospace environmental driver</see> map.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEnvironmentalDriver
    Inherits cDriver

#Region " Private vars "

    Private m_layer As cEcospaceLayerDriver = Nothing

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cMPADriver"/> to drive the <see cref="cEcospaceLayerMPA">
    ''' Marine Protected Area map</see> of a given <see cref="cEcospaceMPA">Ecospace MPA</see>.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to connect to.</param>
    ''' <param name="game">The <see cref="cGame"/> to connect to.</param>
    ''' <param name="layer">The <see cref="cEcospaceLayerDriver">environmental driver
    ''' layer</see> this driver is connected to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, game As cGame, layer As cEcospaceLayerDriver)
        MyBase.New(core, game, cStringUtils.Localize(My.Resources.DRIVER_ENV_NAME, layer.Name))
        Me.m_layer = layer
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Insert pressure data into the <see cref="cEcospaceLayerDriver">environmental driver
    ''' layer map</see>.
    ''' </summary>
    ''' <param name="pressure">The MEL-derived protection map value to apply to the driver.</param>
    ''' <param name="data">Optional Ecospace data structures to apply pressures to.</param>
    ''' <param name="multiplier">Ignored.</param>
    ''' <returns>True if applied correctly, False if an error occurred.</returns>
    ''' <exception cref="cMELException">A MEL exception will be thrown if something went wrong.</exception>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Apply(pressure As cPressure, Optional data As cEcospaceDataStructures = Nothing, Optional multiplier As Double = 1.0!) As Boolean

        Try
            Dim nRows As Integer = pressure.Grid.Height
            Dim nCols As Integer = pressure.Grid.Width
            Dim total As Double = 0

            If (data IsNot Nothing) Then
                Dim map As Single(,) = data.EnvironmentalLayerMap(Me.m_layer.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        map(iRow + 1, iCol + 1) = pressure.Grid.Cell(iRow, iCol)
                    Next iCol
                Next iRow
            Else
                Dim layer As cEcospaceLayerDriver = Me.m_core.EcospaceBasemap.LayerDriver(Me.m_layer.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        Dim val As Double = pressure.Grid.Cell(iCol, iRow)
                        layer.Cell(iRow + 1, iCol + 1) = val
                        If (val > 0) Then
                            total += val
                        End If
                    Next iCol
                Next iRow
                Console.WriteLine("Env driver pressure " & pressure.Name & " total is " & total)
                layer.Invalidate()
                Me.m_core.onChanged(layer)
            End If

        Catch ex As Exception
            cEwEShell.RaiseException("Exception applying pressure " & pressure.Name & " to layer " & Me.m_layer.Name & ", driver " & Me.Name & ".", False)
            Return False
        End Try

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the unique ID for the <see cref="cEcospaceMPA">environmental driver layer</see>.
    ''' </summary>
    ''' <returns>The unique ID for the <see cref="cEcospaceMPA">environmental driver layer</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ValueID() As String
        Return Me.m_layer.GetID()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns that this driver can only be driven by gridded map data.
    ''' </summary>
    ''' <returns>The supported <see cref="cPressure.eDataTypes">pressure type</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function DataType() As cPressure.eDataTypes
        Return cPressure.eDataTypes.Grid
    End Function

End Class
