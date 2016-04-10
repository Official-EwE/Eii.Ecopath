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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Option Explicit On

#Region "Imports"

Imports EwEPlugin
Imports EwECore
Imports EwEUtils

#End Region


Public Class cFitStats
    Public bSSGroup() As Double
    Public SS As Double
    Public nGroups As Integer
    Public nTimeSteps As Integer
    Public MSE As Double

    Private n As Integer

    Public Sub New(NumberOfGroups As Integer)
        Try
            nGroups = NumberOfGroups
            nTimeSteps = 0
            SS = 0
            n = 0
            bSSGroup = New Double(nGroups) {}
        Catch ex As Exception

        End Try
    End Sub

    Public Sub InitTimeStep(iTimeStep As Integer)
        nTimeSteps += 1
    End Sub


    Public Sub AddValues(GroupIndex As Integer, PredictedValue As Single, ObservedValue As Double)
        Dim rle As Double = 0

        n += 1
        'square relative log error
        If PredictedValue <> ObservedValue Then
            rle = Math.Log(PredictedValue / ObservedValue) ^ 2
        End If

        bSSGroup(GroupIndex) += rle
        SS += rle
        MSE = SS / n

    End Sub

End Class


Public Class cEcospaceFit

    Public Event onRunStarted()
    Public Event onRunCompleted()

    Private m_core As cCore
    Private m_PathData As cEcopathDataStructures
    Private m_SpaceData As cEcospaceDataStructures

    Private bSS As Double
    Private nSS As Integer

    Private m_lstStats As List(Of cFitStats)
    Private m_curFit As cFitStats


    Public Sub New()

    End Sub


    Public Function Init(theCore As cCore, EcopathData As cEcopathDataStructures, EcospaceData As cEcospaceDataStructures) As Boolean
        Dim bReturn As Boolean = True
        Try
            m_core = theCore
            m_PathData = EcopathData
            m_SpaceData = EcospaceData

            m_lstStats = New List(Of cFitStats)
        Catch ex As Exception
            bReturn = False
        End Try

        Return bReturn

    End Function


    Public Sub RunInitialized()
        m_curFit = New cFitStats(Me.m_SpaceData.NGroups)
        Me.fireOnRunStarted()
    End Sub

    Public Sub RunCompleted()
        Me.m_lstStats.Add(Me.m_curFit)
        Me.fireOnRunCompleted()
    End Sub


    Public Function EcospaceTimeStep(iTime As Integer) As Boolean
        Return CalcFitStats(iTime)
    End Function

    Private Function CalcFitStats(iTime As Integer) As Boolean
        Dim bReturn As Boolean = True
        Try
            Me.m_curFit.InitTimeStep(iTime)

            For igrp As Integer = 1 To Me.m_SpaceData.NGroups
                Me.m_curFit.AddValues(igrp, Me.m_SpaceData.ResultsByGroup(0, igrp, iTime), m_PathData.B(igrp))
            Next

        Catch ex As Exception
            bReturn = False
        End Try

        Return bReturn

    End Function


    Private Sub fireOnRunStarted()
        Try
            RaiseEvent onRunStarted()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub fireOnRunCompleted()
        Try
            RaiseEvent onRunCompleted()
        Catch ex As Exception

        End Try
    End Sub


    Public ReadOnly Property FitStats As List(Of cFitStats)
        Get
            Return Me.m_lstStats
        End Get
    End Property


    Public Sub Clear()
        Try
            m_lstStats.Clear()
        Catch ex As Exception

        End Try
    End Sub

End Class
