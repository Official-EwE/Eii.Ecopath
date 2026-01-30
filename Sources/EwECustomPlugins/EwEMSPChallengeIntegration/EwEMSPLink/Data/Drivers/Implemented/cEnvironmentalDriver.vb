' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwEUtils.Utilities



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
                Dim map As Single(,) = Me.m_core.EcospaceDataStructures.EnvironmentalLayerMap(Me.m_layer.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        map(iRow + 1, iCol + 1) = ep.Grid.Cell(iCol, iRow)
                    Next iCol
                Next iRow
            Else
                Dim layer As cEcospaceLayerDriver = Me.m_core.EcospaceBasemap.LayerDriver(Me.m_layer.Index)
                For iRow As Integer = 0 To nRows - 1
                    For iCol As Integer = 0 To nCols - 1
                        Dim val As Double = ep.Grid.Cell(iCol, iRow)
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
            cEwEMSPLink.RaiseException("Exception applying pressure " & pressure.Name & " to layer " & Me.m_layer.Name & ", driver " & Me.Name & ".", False)
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
    ''' <returns>The supported pressure type.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function PressureType() As Type
        Return GetType(cEnvironmentalPressure)
    End Function

End Class
