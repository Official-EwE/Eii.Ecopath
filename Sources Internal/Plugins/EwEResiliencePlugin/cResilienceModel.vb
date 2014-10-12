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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
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
    Private Property YearEatenBy As Single
    Private Property YearEatenOf As Single

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
            Me.m_data.Resize(simds.NTimes - 1, simds.NumYears - 1)
        End If

        Dim SumEatenBy As Single = 0
        Dim SumEatenOf As Single = 0
        Dim iYear As Integer = 1 + CInt((iTime - 1) / cCore.N_MONTHS)

        For i As Integer = 1 To Me.m_core.nGroups
            SumEatenBy += simds.Eatenby(i)
            SumEatenOf += simds.Eatenof(i)
        Next
        YearEatenBy += SumEatenBy
        YearEatenOf += SumEatenOf

        Try
            Me.m_data.DemandAtT(iTime - 1) = CSng(Math.Log10(SumEatenOf))
            Me.m_data.SupplyAtT(iTime - 1) = -CSng(Math.Log10(SumEatenBy))

            If ((iTime Mod cCore.N_MONTHS) = 0) Then
                Me.m_data.DemandAtY(iYear - 1) = CSng(Math.Log10(YearEatenOf / cCore.N_MONTHS))
                Me.m_data.SupplyAtY(iYear - 1) = -CSng(Math.Log10(YearEatenBy / cCore.N_MONTHS))
                YearEatenOf = 0
                YearEatenBy = 0
            End If
        Catch ex As Exception
            'Whoah!
        End Try

        Me.RaiseUpdate(iTime, iTime = simds.NTimes)

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

#End Region ' Internals

End Class
