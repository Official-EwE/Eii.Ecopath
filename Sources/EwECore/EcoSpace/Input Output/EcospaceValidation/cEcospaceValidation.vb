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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Statistics to aid the validation and calibration of Ecospace.
''' </summary>
Public Class cEcospaceValidation

#Region " Internal vars "

    Private m_meanBwPrey As New Dictionary(Of Integer, Double(,))

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
    ''' Calculate mean weighted prey biomass.
    ''' </summary>
    ''' <param name="bcell">Predicted biomass (row x col x grp)</param>
    ''' <returns>Mean weighted prey biomass (prey x pred), or Nothing if an 
    ''' error occurred.</returns>
    ''' <remarks>
    ''' ToDo: add support for regions
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function MeanBwPrey(bcell(,,) As Single) As Double(,)

        Dim result(EcopathData.NumLiving, EcopathData.NumGroups) As Double
        Dim Btot(EcopathData.NumGroups) As Double

        Try
            'get total biomasses for all groups
            For iGrp As Integer = 1 To EcopathData.NumGroups
                For row As Integer = 1 To EcospaceData.InRow
                    For col As Integer = 1 To EcospaceData.InCol
                        If EcospaceData.Depth(row, col) > 0 Then
                            Btot(iGrp) += bcell(row, col, iGrp)
                        End If
                    Next col
                Next row
            Next iGrp

            'loop over predators and prey types
            For iPred As Integer = 1 To EcopathData.NumLiving
                For iPrey As Integer = 1 To EcopathData.NumGroups
                    If EcopathData.DC(iPred, iPrey) > 0 Then 'only look at active diet composition cases

                        Dim BxB As Double = 0

                        For row As Integer = 1 To EcospaceData.InRow
                            For col As Integer = 1 To EcospaceData.InCol
                                If EcospaceData.Depth(row, col) > 0 Then
                                    BxB += bcell(row, col, iPred) * bcell(row, col, iPrey)
                                End If
                            Next
                        Next

                        'predator biomass weighted mean prey biomass per area
                        Dim meanprey As Double = BxB / Btot(iPred)
                        'scale to ecopath base prey biomass B(iprey)		
                        result(iPred, iPrey) = meanprey / Me.EcopathData.B(iPrey)
                    End If
                Next
            Next iPred
        Catch ex As Exception
            Debug.Assert(False, "Ecospace validation failed to calculate MeanBwPrey")
            cLog.Write(ex)
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
    Public Function CalculateStats(bcell(,,) As Single, iTimeStep As Integer) As Boolean
        Me.m_meanBwPrey(iTimeStep) = Me.MeanBwPrey(bcell)
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain the weighted mean prey b (pred x prey) for a given time step.
    ''' </summary>
    ''' <param name="iTimestep"></param>
    ''' <returns>The weighted mean prey b (pred x prey) at <paramref name="iTimestep"/>.
    ''' If there are no results for the time step, this will return Nothing.</returns>
    ''' -----------------------------------------------------------------------
    Public Function MeanBwPrey(iTimestep As Integer) As Double(,)
        If (Me.m_meanBwPrey.ContainsKey(iTimestep)) Then
            Return Me.m_meanBwPrey(iTimestep)
        End If
        Return Nothing
    End Function

#End Region ' Public bits

End Class
