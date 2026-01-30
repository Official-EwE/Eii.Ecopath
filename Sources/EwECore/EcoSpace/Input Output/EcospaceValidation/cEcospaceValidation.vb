' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



''' <summary>
''' Statistics to aid the validation and calibration of Ecospace.
''' </summary>
Public Class cEcospaceValidation

#Region " Internal vars "

    ''' <summary>
    ''' Calculated region x pred x prey overlap by timestep (timestep -> region x pred x prey)
    ''' </summary>
    Private m_meanBwPrey As New Dictionary(Of Integer, Double(,,))
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceValidation)()

#End Region ' Internal vars

#Region " Construction / destruction "

    Public Sub New(core As cCore, ecopathData As cEcopathDataStructures, ecospaceData As cEcospaceDataStructures)

        Me.Core = core
        Me.EcopathData = ecopathData
        Me.EcospaceData = ecospaceData

    End Sub

#End Region ' Construction / destruction

#Region " Public bits "

    ''' <summary>Get the attached core.</summary>
    Friend ReadOnly Property Core As cCore
    ''' <summary>Get the attached Ecopath data structures.</summary>
    Friend ReadOnly Property EcopathData As cEcopathDataStructures
    ''' <summary>Get the attached Ecospace data structures.</summary>
    Friend ReadOnly Property EcospaceData As cEcospaceDataStructures

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear the stats in preparation for new computations.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Clear()
        Me.m_meanBwPrey.Clear()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate mean weighted prey biomass by region.
    ''' </summary>
    ''' <param name="bcell">Predicted biomass (row x col x grp)</param>
    ''' <param name="nRegions">Optional number of regions to consider.</param>
    ''' <param name="regions">Optional region map.</param>
    ''' <returns>
    ''' Mean weighted prey biomass (prey x pred x region), or Nothing if an 
    ''' error occurred.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function CalculateMeanBwPrey(bcell(,,) As Single,
                                        Optional nRegions As Integer = 0,
                                        Optional regions(,) As Integer = Nothing) As Double(,,)

        Dim result(EcopathData.NumLiving, EcopathData.NumGroups, nRegions) As Double
        Dim Btot(nRegions, EcopathData.NumGroups) As Double
        Dim iRegion As Integer = 0

        Try
            'get total biomasses for all groups
            For row As Integer = 1 To EcospaceData.InRow
                For col As Integer = 1 To EcospaceData.InCol
                    If EcospaceData.Depth(row, col) > 0 Then
                        For iGrp As Integer = 1 To EcopathData.NumGroups
                            If (regions IsNot Nothing) Then iRegion = regions(row, col)
                            ' Add to specific region
                            If (iRegion > 0 And iRegion <= nRegions) Then Btot(iRegion, iGrp) += bcell(row, col, iGrp)
                            ' Add to total region
                            Btot(0, iGrp) += bcell(row, col, iGrp)
                        Next iGrp
                    End If
                Next col
            Next row

            'loop over predators and prey types
            For iPred As Integer = 1 To EcopathData.NumLiving
                For iPrey As Integer = 1 To EcopathData.NumGroups
                    If EcopathData.DC(iPred, iPrey) > 0 Then 'only look at active diet composition cases

                        Dim BxB(nRegions) As Double

                        For row As Integer = 1 To EcospaceData.InRow
                            For col As Integer = 1 To EcospaceData.InCol
                                If EcospaceData.Depth(row, col) > 0 Then
                                    If (regions IsNot Nothing) Then iRegion = regions(row, col)
                                    ' Account for specific region
                                    If (iRegion > 0 And iRegion <= nRegions) Then BxB(iRegion) += bcell(row, col, iPred) * bcell(row, col, iPrey)
                                    ' Account for total region
                                    BxB(0) += bcell(row, col, iPred) * bcell(row, col, iPrey)
                                End If
                            Next
                        Next

                        For iRegion = 0 To nRegions
                            If (Btot(iRegion, iPred) > 0) Then
                                'predator biomass weighted mean prey biomass per area
                                Dim meanprey As Double = BxB(iRegion) / Btot(iRegion, iPred)
                                'scale to ecopath base prey biomass B(iprey)		
                                result(iPred, iPrey, iRegion) = meanprey / Me.EcopathData.B(iPrey)
                            End If
                        Next
                    End If
                Next
            Next iPred
        Catch ex As Exception
            Debug.Assert(False, "Ecospace validation failed to calculate MeanBwPrey")
            m_logger.LogError(ex, "Ecospace validation failed to calculate MeanBwPrey")
            Return Nothing
        End Try

        Return result

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate stats for a given time step.
    ''' </summary>
    ''' <param name="bcell">The biomass (row x col x group) to calculate stats from.</param>
    ''' <param name="iTimeStep">The time step that <paramref name="bcell"/> corresponds to.</param>
    ''' <returns>Always true. This is a very happy function.</returns>
    ''' -----------------------------------------------------------------------
    Public Function CalculateStats(iTimeStep As Integer, bcell(,,) As Single,
                                   Optional iRegions As Integer = 0, Optional regions(,) As Integer = Nothing) As Boolean
        Me.m_meanBwPrey(iTimeStep) = Me.CalculateMeanBwPrey(bcell, iRegions, regions)
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain the weighted mean prey b (pred x prey x region) for a given time step.
    ''' </summary>
    ''' <param name="iTimestep"></param>
    ''' <returns>The weighted mean prey b (pred x prey x region) at <paramref name="iTimestep"/>.
    ''' If there are no results for the time step, this will return Nothing.</returns>
    ''' -----------------------------------------------------------------------
    Public Function MeanBwPrey(iTimestep As Integer) As Double(,,)
        If (Me.m_meanBwPrey.ContainsKey(iTimestep)) Then
            Return Me.m_meanBwPrey(iTimestep)
        End If
        Return Nothing
    End Function

#End Region ' Public bits

End Class
