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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================

Option Strict On
Option Explicit On

#Region "Imports"

Imports EwECore

Imports Troschuetz.Random

#End Region


Public Class cStockAssessmentModel

    'ToDo Implement the stock recruitment model
    'ToDo Figure out how to get Troschuetz.Random.NormalDistribution() to return a normal distribution with a mean=0 sd=1
    'as is it looks like its mean=1 sd=1

    Private m_MSE As cMSE
    Private m_core As cCore

    Private Bestimate() As Single

    Public Sub New(ByVal MSE As cMSE)
        Me.m_MSE = MSE
        Me.m_core = m_MSE.Core
        Debug.Assert(Me.m_MSE IsNot Nothing, "cStockAssessmentModel must have a valid cMSE object during initialization!")
        Debug.Assert(Me.m_core IsNot Nothing, "cStockAssessmentModel must have a valid cCore object during initialization!")
    End Sub


    Private Function getAvgB(iModelTimeStep As Integer) As Single()
        Dim ngrps As Integer = Me.Core.nLivingGroups
        Dim simdata As cEcosimDatastructures = Me.MSE.EcosimData
        Dim avgB() As Single = New Single(ngrps) {}

        Debug.Assert(iModelTimeStep > 12, Me.ToString + ".getAvgB() Can only be called after the end of the first year!")
        Dim StartT As Integer = iModelTimeStep - 12

        'Sum the biomass from the previous year
        For it As Integer = StartT To StartT + 12
            For igrp As Integer = 1 To ngrps
                avgB(igrp) += Me.m_MSE.EcosimData.ResultsOverTime(0, igrp, it)
            Next igrp
        Next it

        'Get the average
        For igrp As Integer = 1 To ngrps
            avgB(igrp) /= 12
        Next igrp

        Return avgB

    End Function

    Public Function DoAnnualStockAssessment(iTimestep As Integer) As Single()

        Dim nGrps As Integer = Me.Core.nLivingGroups
        'Use the MSE Stock Recruitment Parameters from the EwE6 interface until we get our own interface
        Dim MSEData As MSE.cMSEDataStructures = Me.MSE.CoreMSEData
        Dim Bobs() As Single = New Single(nGrps) {}

        'get average biomass for the last year
        Dim Bavg() As Single = Me.getAvgB(iTimestep)

        Dim rand As New Troschuetz.Random.NormalDistribution()
        'Make sure the Bestimate() array has been dimensioned
        Me.InitBestimated()

        For i As Integer = 1 To nGrps
            'Get the Observed Biomass from the last year
            'Average biomass with variation for the last year
            '1 - rand.NextDouble() ??? is this a mean=0 sd=1
            Bobs(i) = Bavg(i) * CSng(Math.Exp(MSEData.CVbiomEst(i) * (1 - rand.NextDouble())))
            'For now just set the Bestimate() to observed biomass
            Me.Bestimate(i) = Bobs(i)

            'Get the estimated biomass base on the observed biomass plus to variation
            'Using the stock recruitment curve from the EwE6 MSE interface
            'Me.m_data.Bestimate(i) = Me.stockRecruitment(i, Biomass(i), Bobs(i), Me.m_data.Bestimate(i))
        Next i

        'Just returns Bobs for now
        Return Me.Bestimate

    End Function

    Private ReadOnly Property Core As cCore
        Get
            Return Me.m_core
        End Get
    End Property

    Private ReadOnly Property MSE As cMSE
        Get
            Return Me.m_MSE
        End Get
    End Property


    Private Sub InitBestimated()
        If Bestimate Is Nothing Then
            Bestimate = New Single(Me.Core.nLivingGroups) {}
        End If
        If Bestimate.Length <> Me.Core.nLivingGroups Then
            Bestimate = New Single(Me.Core.nLivingGroups) {}
        End If
    End Sub


End Class
