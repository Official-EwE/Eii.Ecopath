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

Public Class cResilienceData

    ''' <summary>EwE core to use</summary>
    Private m_core As cCore = Nothing
    Private m_supply() As Single
    Private m_demand() As Single

    Public Event OnUpdated(sender As cResilienceData, time As Integer)

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Sub Compute(iTime As Integer, simds As cEcosimDatastructures)

        If (iTime = 1) Then
            ReDim m_supply(simds.NTimes)
            ReDim m_demand(simds.NTimes)
        End If

        Dim SumEatenBy As Single = 0
        Dim SumEatenOf As Single = 0

        For i As Integer = 1 To Me.m_core.nGroups
            SumEatenBy += simds.Eatenby(i)
            SumEatenOf += simds.Eatenof(i)
        Next

        Try
            Me.m_demand(iTime) = CSng(Math.Log(SumEatenOf))
            Me.m_supply(iTime) = -CSng(Math.Log(SumEatenBy))
        Catch ex As Exception
            Me.m_demand(iTime) = 0
            Me.m_supply(iTime) = 0
        End Try

        Me.RaiseUpdate(iTime)

    End Sub

    Public ReadOnly Property Demand(iTime As Integer) As Single
        Get
            Return Me.m_demand(iTime)
        End Get
    End Property

    Public ReadOnly Property Supply(iTime As Integer) As Single
        Get
            Return Me.m_supply(iTime)
        End Get
    End Property

#Region " Internals "

    Private Sub RaiseUpdate(time As Integer)
        Try
            RaiseEvent OnUpdated(Me, time)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Internals

End Class
