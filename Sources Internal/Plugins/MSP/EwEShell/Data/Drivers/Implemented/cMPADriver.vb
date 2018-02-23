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
''' Driver for inserting MSP pressure data into the <see cref="cEcospaceLayerMPA">map</see>
''' of a single Ecospace <see cref="cEcospaceMPA">Marine Protected Area</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cMPADriver
    Inherits cDriver

#Region " Private vars "

    Private m_mpa As cEcospaceMPA = Nothing

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cMPADriver"/> to drive the <see cref="cEcospaceLayerMPA">
    ''' Marine Protected Area map</see> of a given <see cref="cEcospaceMPA">Ecospace MPA</see>.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to connect to.</param>
    ''' <param name="game">The <see cref="cGame"/> to connect to.</param>
    ''' <param name="MPA">The <see cref="cEcospaceMPA">MPA</see> this driver is connected to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, game As cGame, MPA As cEcospaceMPA)
        MyBase.New(core, game, cStringUtils.Localize(My.Resources.DRIVER_MPA_NAME, MPA.Name))
        Me.m_mpa = MPA
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Insert pressure data into the <see cref="cEcospaceLayerMPA">MPA map</see>.
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

            If (data IsNot Nothing) Then
                Dim map As Boolean(,) = data.MPA(Me.m_mpa.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        map(iRow + 1, iCol + 1) = (pressure.Grid.Cell(iRow, iCol) >= Me.m_game.MPACellClosureRatio)
                    Next iCol
                Next iRow
            Else
                Dim layer As cEcospaceLayerMPA = Me.m_core.EcospaceBasemap.LayerMPA(Me.m_mpa.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        layer.Cell(iRow + 1, iCol + 1) = (pressure.Grid.Cell(iCol, iRow) >= Me.m_game.MPACellClosureRatio)
                    Next iCol
                Next iRow
                layer.Invalidate()

                Me.m_core.onChanged(layer)
            End If

        Catch ex As Exception
            cEwEShell.RaiseException("Exception applying pressure " & pressure.Name & " to MPA layer " & Me.m_mpa.Name & ", driver " & Me.Name & ".", False)
            Return False
        End Try

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the unique ID for the Ecospace <see cref="cEcospaceMPA">MPA</see>.
    ''' </summary>
    ''' <returns>The unique ID for the Ecospace <see cref="cEcospaceMPA">MPA</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ValueID() As String
        Return Me.m_mpa.GetID()
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
