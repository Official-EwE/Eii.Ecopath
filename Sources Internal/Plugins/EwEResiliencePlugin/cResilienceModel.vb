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
Imports EwEUtils.SystemUtilities

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
            Me.m_data.Resize(simds.nGroups, simds.NTimes - 1, simds.NumYears - 1)
            ReDim EatenByYear(simds.nGroups)
            ReDim EatenOfYear(simds.nGroups)
        End If

        Dim iYear As Integer = 1 + CInt((iTime - 1) / cCore.N_MONTHS)

        For i As Integer = 1 To Me.m_core.nGroups
            EatenByYear(i) += simds.Eatenby(i)
            EatenOfYear(i) += simds.Eatenof(i)

            Me.m_data.GroupSupplyAtT(i, iTime - 1) = -cSystemUtils.IIF(simds.Eatenby(i) = 0, 0, CSng(Math.Log10(simds.Eatenby(i))))
            Me.m_data.GroupDemandAtT(i, iTime - 1) = cSystemUtils.IIF(simds.Eatenof(i) = 0, 0, CSng(Math.Log10(simds.Eatenof(i))))

            Try
                If ((iTime Mod cCore.N_MONTHS) = 0) Then
                    Me.m_data.GroupSupplyAtY(i, iYear - 1) = -cSystemUtils.IIF(EatenByYear(i) = 0, 0, CSng(Math.Log10(EatenByYear(i) / cCore.N_MONTHS)))
                    Me.m_data.GroupDemandAtY(i, iYear - 1) = cSystemUtils.IIF(EatenOfYear(i) = 0, 0, CSng(Math.Log10(EatenOfYear(i) / cCore.N_MONTHS)))
                    EatenOfYear(i) = 0
                    EatenByYear(i) = 0
                End If
            Catch ex As Exception
                'Whoah!
            End Try
        Next

        If (iTime = simds.NTimes) Then
            Me.m_data.CalculateStats()
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

#End Region ' Internals

End Class
