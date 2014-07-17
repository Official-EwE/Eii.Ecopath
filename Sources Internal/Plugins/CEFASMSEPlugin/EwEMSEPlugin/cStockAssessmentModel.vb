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
    Implements IMSEData

    'ToDo Implement the stock recruitment model
    'ToDo Figure out how to get Troschuetz.Random.NormalDistribution() to return a normal distribution with a mean=0 sd=1
    'as is it looks like its mean=1 sd=1


#Region "Private data"

    Private m_MSE As cMSE
    Private m_core As cCore

    Private Bestimate() As Single
    Private BestimateLast() As Single
    Private KalmanGain() As Single

    Private m_RandNormal As Troschuetz.Random.NormalDistribution

#End Region

#Region "Pubic data"

    Public Rmax() As Single
    Public BhalfT() As Single
    Public CVbiomEst() As Single
    Public GstockPred() As Single
    Public RstockRatio() As Single
    Public RStock0() As Single
    Public RHalfB0Ratio() As Single
    Public cvRec() As Single

#End Region

#Region "Construction and Initialization"


    Public Sub New(ByVal MSE As cMSE)
        Me.m_MSE = MSE
        Me.m_core = m_MSE.Core
        Debug.Assert(Me.m_MSE IsNot Nothing, "cStockAssessmentModel must have a valid cMSE object during initialization!")
        Debug.Assert(Me.m_core IsNot Nothing, "cStockAssessmentModel must have a valid cCore object during initialization!")
    End Sub


    Public Sub Init()
        Dim BaB As Single
        ' Dim MSEData As MSE.cMSEDataStructures = Me.MSE.CoreMSEData
        Dim simdata As cEcosimDatastructures = Me.MSE.EcosimData
        Dim pathdata As cEcopathDataStructures = Me.MSE.EcopathData

        Bestimate = New Single(Me.Core.nLivingGroups) {}
        BestimateLast = New Single(Me.Core.nLivingGroups) {}
        m_RandNormal = New Troschuetz.Random.NormalDistribution()

        'Init Bestimate() and BestimateLast() to the start biomass with some error
        For igrp As Integer = 1 To Me.Core.nLivingGroups
            Bestimate(igrp) = simdata.StartBiomass(igrp) * CSng(Math.Exp(CVbiomEst(igrp) * (1 - m_RandNormal.NextDouble())))
            BestimateLast(igrp) = Bestimate(igrp)
        Next igrp

        'init RstockPred from GstockPred
        'GstockPred could have been altered by an interface
        For igrp = 1 To Me.Core.nLivingGroups
            'BaB is correct for Stanza groups because Ecopath.BA() gets updated with Stanza.BaBsplit()
            BaB = pathdata.BA(igrp) / pathdata.B(igrp)
            'gstockpred=exp(bab)-rstockratio, rather than 1-rstockratio.  Check to insure gstockpred>0

            'Me.m_data.GstockPred(igrp) = 1 - Me.m_data.RstockRatio(igrp)
            GstockPred(igrp) = CSng(Math.Exp(BaB) - RstockRatio(igrp))
            If GstockPred(igrp) < 0 Then GstockPred(igrp) = 0
            BhalfT(igrp) = RHalfB0Ratio(igrp) * pathdata.B(igrp)

            RStock0(igrp) = RstockRatio(igrp) * simdata.StartBiomass(igrp)
            Rmax(igrp) = RStock0(igrp) * (RHalfB0Ratio(igrp) + 1)

        Next

    End Sub

#End Region

#Region "Public Methods"


    Public Function DoAnnualStockAssessment(iTimestep As Integer, Biomass() As Single) As Single()
        Try

            Dim nGrps As Integer = Me.Core.nLivingGroups
            'Use the MSE Stock Recruitment Parameters from the EwE6 interface until we get our own interface
            Dim MSEData As MSE.cMSEDataStructures = Me.MSE.CoreMSEData
            Dim Bobs() As Single = New Single(nGrps) {}

            'get average biomass for the last year
            Dim Bavg() As Single = Me.getAvgB(iTimestep)

            For igrp As Integer = 1 To nGrps
                'Get the Observed Biomass from the last year
                'Average biomass with variation for the last year
                '1 - rand.NextDouble() ??? is this a mean=0 sd=1
                Bobs(igrp) = Bavg(igrp) * CSng(Math.Exp(CVbiomEst(igrp) * (1 - m_RandNormal.NextDouble())))

                'Get the estimated biomass base on the observed biomass plus variation
                'Using the stock recruitment curve from the EwE6 MSE interface
                Me.Bestimate(igrp) = Me.stockRecruitment(iTimestep, igrp, Bobs(igrp), Me.Bestimate(igrp))
            Next igrp

            'For debugging dump BioEst/B to console window
            Me.dumpBioEstOverB(Me.Bestimate, Biomass)

            Return Me.Bestimate

        Catch ex As Exception
            Debug.Assert(False, "Opps Exception in DoAnnualStockAssessment(). " + ex.Message)
        End Try

        Return Biomass

    End Function


#End Region

#Region "Private Properties and  Methods"

    Private Sub dumpBioEstOverB(BioEst() As Single, B() As Single)

        For i As Integer = 1 To Me.Core.nLivingGroups
            System.Console.Write(i.ToString + "," + (BioEst(i) / B(i)).ToString + " | ")
        Next
        System.Console.WriteLine()

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

    Private Function stockRecruitment(ByVal iTime As Integer, ByVal iGroup As Integer, ByVal BioEst As Single, ByVal Blast As Single) As Single
        'B is the biomass calculated by Ecosim
        'BioEst is the observed biomass(Ecosim biomass + random variation)
        'Blast is the biomass predicted for the last timestep ( Blast = stockRecruitment(t-1) )

        Dim RstockPred As Single
        Dim vPred As Single
        Dim Best As Single

        Dim simdata As cEcosimDatastructures = Me.MSE.EcosimData
        'Dim MSEData As MSE.cMSEDataStructures = Me.MSE.CoreMSEData

        Dim CatchYear() As Single = Me.getCatch(iTime)
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        ' What this correction basically does is to increase the year-to-year Biomass gain factor in the delaydifference model (effective GstockPred by year)
        ' for situations where F has been reduced relative to ecopath base, and reduce the factor for years when F is higher than ecopath base.  
        'In the original code, we were just doing a factor reduction based on current F (catchyeargroup/Blast), without correcting relative to the ecopath base value of GstockPred.
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        Me.BestimateLast(iGroup) = Blast * CSng(Math.Exp(-CatchYear(iGroup) / Blast + simdata.Fish1(iGroup)))

        RstockPred = CSng(Rmax(iGroup) * Me.BestimateLast(iGroup) / (BhalfT(iGroup) + Me.BestimateLast(iGroup)))
        vPred = CSng((RstockRatio(iGroup) * cvRec(iGroup)) ^ 2 / (1 - GstockPred(iGroup) ^ 2))
        KalmanGain(iGroup) = CSng(vPred / (vPred + CVbiomEst(iGroup) ^ 2))

        Best = KalmanGain(iGroup) * BioEst + (1 - KalmanGain(iGroup)) * (GstockPred(iGroup) * Me.BestimateLast(iGroup) + RstockPred)

        Return Best

    End Function

    Private Function getCatch(itime As Integer) As Single()
        Dim ngrps As Integer = Me.Core.nLivingGroups
        Dim simdata As cEcosimDatastructures = Me.MSE.EcosimData
        Dim avgCatch() As Single = New Single(ngrps) {}

        Debug.Assert(itime > 12, Me.ToString + ".getCatch() Can only be called after the end of the first year!")
        Dim StartT As Integer = itime - 12

        'Sum the biomass from the previous year
        For it As Integer = StartT To StartT + 12
            For igrp As Integer = 1 To ngrps
                avgCatch(igrp) += Me.m_MSE.EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, it)
            Next igrp
        Next it

        'Get the average
        For igrp As Integer = 1 To ngrps
            avgCatch(igrp) /= 12
        Next igrp

        Return avgCatch

    End Function

    Private Sub InitBestimated()
        If Bestimate Is Nothing Then
            Bestimate = New Single(Me.Core.nLivingGroups) {}
        End If
        If Bestimate.Length <> Me.Core.nLivingGroups Then
            Bestimate = New Single(Me.Core.nLivingGroups) {}
        End If
    End Sub


    Private Sub InitToCoreData()

        Dim MSEData As MSE.cMSEDataStructures = Me.MSE.CoreMSEData

        Rmax = New Single(Me.Core.nLivingGroups) {}
        BhalfT = New Single(Me.Core.nLivingGroups) {}
        CVbiomEst = New Single(Me.Core.nLivingGroups) {}
        GstockPred = New Single(Me.Core.nLivingGroups) {}
        RstockRatio = New Single(Me.Core.nLivingGroups) {}
        RStock0 = New Single(Me.Core.nLivingGroups) {}
        RHalfB0Ratio = New Single(Me.Core.nLivingGroups) {}
        cvRec = New Single(Me.Core.nLivingGroups) {}
        KalmanGain = New Single(Me.Core.nLivingGroups) {}

        Array.Copy(MSEData.Rmax, Rmax, Me.Core.nLivingGroups)
        Array.Copy(MSEData.BhalfT, BhalfT, Me.Core.nLivingGroups)
        Array.Copy(MSEData.CVbiomEst, CVbiomEst, Me.Core.nLivingGroups)
        Array.Copy(MSEData.GstockPred, GstockPred, Me.Core.nLivingGroups)
        Array.Copy(MSEData.RstockRatio, RstockRatio, Me.Core.nLivingGroups)
        Array.Copy(MSEData.RStock0, RStock0, Me.Core.nLivingGroups)
        Array.Copy(MSEData.RHalfB0Ratio, RHalfB0Ratio, Me.Core.nLivingGroups)
        Array.Copy(MSEData.cvRec, cvRec, Me.Core.nLivingGroups)

    End Sub


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

#End Region

#Region "IMSEData Implementation"


    Public Sub Defaults() Implements IMSEData.Defaults

    End Sub

    Public Function IsChanged() As Boolean Implements IMSEData.IsChanged

        Return False
    End Function

    Public Function Load(Optional strFilename As String = "") As Boolean Implements IMSEData.Load

        'For now just copy the data from the core into local arrays
        Me.InitToCoreData()

        Return True
    End Function

    Public Function Save(Optional strFilename As String = "") As Boolean Implements IMSEData.Save

        Return True
    End Function

#End Region

End Class
