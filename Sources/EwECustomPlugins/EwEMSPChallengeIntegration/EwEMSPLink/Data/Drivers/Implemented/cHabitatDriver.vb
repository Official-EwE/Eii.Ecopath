' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Imports EwECore
Imports EwEUtils.Utilities



''' ---------------------------------------------------------------------------
''' <summary>
''' Driver for inserting MSP pressure data into the <see cref="cEcospaceLayerHabitat">map</see>
''' of a single Ecospace <see cref="cEcospaceHabitat">Ecospace habitat</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cHabitatDriver
    Inherits cDriver

#Region " Private vars "

    Private m_hab As cEcospaceHabitat = Nothing

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cHabitatDriver"/> to drive the <see cref="cEcospaceLayerHabitat">
    ''' habitat map</see> of a given <see cref="cEcospaceHabitat">Ecospace habitat</see>.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to connect to.</param>
    ''' <param name="game">The <see cref="cGame"/> to connect to.</param>
    ''' <param name="hab">The <see cref="cEcospaceHabitat">habitat</see> this driver is connected to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, game As cGame, hab As cEcospaceHabitat)
        MyBase.New(core, game, cStringUtils.Localize(My.Resources.DRIVER_HAB_NAME, hab.Name))
        Me.m_hab = hab
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Insert pressure data into the <see cref="cEcospaceLayerHabitat">habitat map</see>.
    ''' </summary>
    ''' <param name="pressure">The MEL-derived habitat map value to apply to the driver.</param>
    ''' <param name="bDirect">Flag, indicating whether a value needs to be 
    ''' injected directly into the EwE data structures (true) or into the EwE 
    ''' input/output objects (false).</param>
    ''' <param name="multiplier">Ignored.</param>
    ''' <returns>True if applied correctly, False if an error occurred.</returns>
    ''' <exception cref="cMELException">A MEL exception will be thrown if something went wrong.</exception>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Apply(pressure As cPressure, bDirect As Boolean, Optional multiplier As Double = 1.0!) As Boolean

        If (TypeOf pressure IsNot cEnvironmentalPressure) Then Return False
        Dim ep As cEnvironmentalPressure = DirectCast(pressure, cEnvironmentalPressure)

        Try
            Dim nRows As Integer = ep.Grid.Height
            Dim nCols As Integer = ep.Grid.Width
            Dim total As Double = 0

            If (bDirect) Then
                Dim map As Single(,) = Me.m_core.EcospaceDataStructures.PHabType(Me.m_hab.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        map(iRow + 1, iCol + 1) = Math.Max(0, Math.Min(1, ep.Grid.Cell(iCol, iRow)))
                    Next iCol
                Next iRow
            Else
                Dim layer As cEcospaceLayerHabitat = Me.m_core.EcospaceBasemap.LayerHabitat(Me.m_hab.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        Dim val As Double = ep.Grid.Cell(iCol, iRow)
                        layer.Cell(iRow + 1, iCol + 1) = val
                        total += val
                    Next iCol
                Next iRow

                layer.Invalidate()
                Me.m_core.onChanged(layer)

                Console.WriteLine("Hab pressure " & pressure.Name & " total is " & total)

            End If

        Catch ex As Exception
            cEwEMSPLink.RaiseException("Exception applying pressure " & pressure.Name & " to habitat layer " & Me.m_hab.Name & ", driver " & Me.Name & ".", False)
            Return False
        End Try

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the unique ID for the Ecospace <see cref="cEcospaceHabitat">habitat</see>.
    ''' </summary>
    ''' <returns>The unique ID for the Ecospace <see cref="cEcospaceHabitat">habitat</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ValueID() As String
        Return Me.m_hab.GetID()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns that this driver can only be driven by gridded map data.
    ''' </summary>
    ''' <returns>The supported pressure type.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function PressureType() As Type
        Return GetType(cEnvironmentalPressure)
    End Function

End Class
