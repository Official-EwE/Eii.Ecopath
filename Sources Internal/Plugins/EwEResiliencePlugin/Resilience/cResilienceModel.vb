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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

Public Class cResilienceModel
    Implements IDisposable

    ''' <summary>EwE core to use</summary>
    Private m_core As cCore = Nothing
    Private m_data As cResilienceData = Nothing

    ' -- Internals --
    Private Property EatenByYear As Single()
    Private Property EatenOfYear As Single()

    Public Sub New(core As cCore)
        Me.m_core = core
        Me.m_data = New cResilienceData()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Me.m_data = Nothing
        GC.SuppressFinalize(Me)
    End Sub

    Public Event OnUpdated(sender As cResilienceData, time As Integer, bDone As Boolean)

    Public Sub Compute(iTime As Integer, simds As cEcosimDatastructures)

        If (iTime = 1) Then
            Me.m_data.Resize(simds.nGroups, simds.NTimes, simds.NumYears)
            ReDim EatenByYear(simds.nGroups)
            ReDim EatenOfYear(simds.nGroups)

            For iGroup As Integer = 1 To Me.m_core.nGroups
                Me.m_data.IsConsumer(iGroup) = (Me.m_core.EcoPathGroupInputs(iGroup).IsConsumer)
            Next
        End If

        Dim iYear As Integer = 1 + CInt(Math.Floor((iTime - 1) / cCore.N_MONTHS))

        For i As Integer = 1 To Me.m_core.nGroups
            If (Me.m_data.IsConsumer(i)) Then
                EatenByYear(i) += simds.Eatenby(i)
                EatenOfYear(i) += simds.Eatenof(i)

                Me.m_data.GroupDemandAtT(i, iTime) = If(simds.Eatenby(i) = 0, 0, CSng(Math.Log10(simds.Eatenby(i))))
                Me.m_data.GroupSupplyAtT(i, iTime) = If(simds.Eatenof(i) = 0, 0, CSng(Math.Log10(simds.Eatenof(i))))

                If ((iTime Mod cCore.N_MONTHS) = 0) Then
                    Me.m_data.GroupDemandAtY(i, iYear) = If(EatenByYear(i) = 0, 0, CSng(Math.Log10(EatenByYear(i) / cCore.N_MONTHS)))
                    Me.m_data.GroupSupplyAtY(i, iYear) = If(EatenOfYear(i) = 0, 0, CSng(Math.Log10(EatenOfYear(i) / cCore.N_MONTHS)))
                    EatenOfYear(i) = 0
                    EatenByYear(i) = 0
                End If
            End If
        Next

        Me.CalculateRegression(Me.m_data.GroupDemandAtT, Me.m_data.GroupSupplyAtT, iTime, Me.m_data.SlopeAtT, Me.m_data.InterceptAtT)
        If ((iTime Mod cCore.N_MONTHS) = 0) Then
            Me.CalculateRegression(Me.m_data.GroupDemandAtY, Me.m_data.GroupSupplyAtY, iYear, Me.m_data.SlopeAtY, Me.m_data.InterceptAtY)
        End If

        If (iTime = simds.NTimes) Then
            Me.m_data.CalculateBounds()
        End If

        Me.RaiseUpdate(iTime, Me.m_data.Calculated)

    End Sub

    Public ReadOnly Property Data As cResilienceData
        Get
            Return Me.m_data
        End Get
    End Property

#Region " Internals "

    Private Sub RaiseUpdate(time As Integer, bDone As Boolean)
        Try
            RaiseEvent OnUpdated(Me.m_data, time, bDone)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub CalculateRegression(ByVal Supply As Single(,), ByVal Demand As Single(,), ByVal Time As Integer, ByVal Slope As Single(), ByVal Intercept As Single())

        Dim s0 As Integer = 0
        Dim s1, s2, t0, t1 As Double

        For i As Integer = 1 To Me.m_core.nGroups
            If Me.m_data.IsConsumer(i) Then
                If (Demand(i, Time) <> 0) And (Supply(i, Time) <> 0) Then
                    s0 += 1
                    s1 = s1 + Demand(i, Time)
                    s2 = s2 + Demand(i, Time) * Demand(i, Time)
                    t0 = t0 + Supply(i, Time)
                    t1 = t1 + Demand(i, Time) * Supply(i, Time)
                End If
            End If
        Next

        Slope(Time) = CSng((s0 * t1 - s1 * t0) / (s0 * s2 - s1 * s1))
        Intercept(Time) = CSng((s2 * t0 - s1 * t1) / (s0 * s2 - s1 * s1))

    End Sub

#End Region ' Internals

End Class
