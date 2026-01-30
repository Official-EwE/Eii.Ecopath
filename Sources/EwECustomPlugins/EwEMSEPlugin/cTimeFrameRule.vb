' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On


Imports EwECore
Imports System.IO
Imports EwEUtils.Utilities
Imports EwEMSEPlugin.HCR_GroupNS

Public Class cTimeFrameRule
    'This object handles the application of a time frame rule.
    'A time frame rule is a rule that overrides the HCR and steps the F used to calc the quota down uniformly from the F during the last year of hindcast
    'to the maximum F (Fmsy) of the harvest control rule over a user specified number of years
    'This object is created for each HCR but only does anything if the number of years it is specified for is > 0

    Private m_F() As Single
    Private m_nTimeStepsInHindcast As Integer
    Private m_EcosimData As cEcosimDatastructures
    Private m_HCR As HCR_Group
    'Private FGreaterThanHCRF As Boolean
    Private m_MSE As cMSE
    Private init_F As Single 'This is the real average F in the last year of the hindcast

    Public Property NYears As Integer

    Public Sub New(ByRef EcosimDatastructures As cEcosimDatastructures, ByRef HCR As HCR_Group, ByRef MSE As cMSE)
        Me.m_EcosimData = EcosimDatastructures
        Me.m_HCR = HCR
        Me.m_nTimeStepsInHindcast = EcosimDatastructures.NTimes
        Me.m_MSE = MSE
    End Sub

    Public Function F(iCurrentTimeStep As Integer, iYearProjecting As Integer, HCR_F As Single) As Single

        If iYearProjecting = 1 Then
            Me.init_F = Me.calcAveragePrevYearF(iCurrentTimeStep)
            If Me.init_F < HCR_F Then Me.init_F = HCR_F
        End If

        'Dim MeanPrevYearF As Double = calcAveragePrevYearF(iCurrentTimeStep)

#If DEBUG Then
        If iYearProjecting = 1 Then
            Dim strmWriter As StreamWriter
            Dim strFile As String = cFileUtils.ToValidFileName("Diagnostics_F_Steps.csv", False)
            strmWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(Me.m_MSE.DataPath, cMSEUtils.eMSEPaths.Results, strFile), True)
            strmWriter.WriteLine(Me.m_MSE.CurrentModelID & "," & Me.m_MSE.currentStrategy.Name & "," & Me.m_HCR.GroupF.Name & "," & Me.init_F)
            cMSEUtils.ReleaseWriter(strmWriter)
        End If
#End If

        'Dim Distance_From_HCR_F As Single = MeanPrevYearF - HCR_F

        Return CSng(Me.init_F - (iYearProjecting / (Me.NYears + 1)) * (Me.init_F - HCR_F))

        'Return HCR_F + ((NYears + 1 - iYearProjecting) / (NYears + 1)) * Distance_From_HCR_F


    End Function

    Public Function CheckValidRule(iYearProjecting As Integer, HCR_F As Single, CurrentTimeStep As Integer) As Boolean
        'A time frame rule is valid only if the number of years field for it is >0 and the year into projection is 1 upto that number
        'and also the F during the last year of the hindcast is greater than the Fmsy

        'If NYears > 0 And iYearProjecting >= 1 And iYearProjecting <= NYears And FGreaterThanHCRF(HCR_F, CurrentTimeStep) Then
        If Me.NYears > 0 And iYearProjecting >= 1 And iYearProjecting <= Me.NYears Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function calcAveragePrevYearF(iCurrentTimestep As Integer) As Single

        Dim MeanF As Single
        Dim BiomassAtT As Single
        Dim Q As Single
        Dim GroupIndex As Integer = Me.m_HCR.GroupF.Index

        Debug.Assert(iCurrentTimestep > 12, "TimeFrameRules must have a hind cast period > 12 months. See cTimeFrameRule.calcAverageFLastYearHindCast()")

        'Get the average from the last year of the hindcast
        'iCurrentTimestep is the first time step of the forecast
        For iTimeStep = (iCurrentTimestep - 12) To (iCurrentTimestep - 1)
            BiomassAtT = Me.m_EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, GroupIndex, iTimeStep)

            'time series include density dependent q implicity so dont need to multiply by it here (set Q =1). In all other cases do so.
            If Me.m_EcosimData.FisForced(GroupIndex) Then
                Q = 1
            Else
                Q = Me.m_EcosimData.QmQo(GroupIndex) / (1 + (Me.m_EcosimData.QmQo(GroupIndex) - 1) * BiomassAtT / Me.m_EcosimData.StartBiomass(GroupIndex))
            End If
            MeanF += Me.m_EcosimData.FishRateNo(Me.m_HCR.GroupF.Index, iTimeStep) * Q
        Next
        MeanF /= 12

        Return MeanF

    End Function

    Private Function FGreaterThanHCRF(HCR_F As Double, iCurrentTime As Integer) As Boolean

        Dim MeanPrevYearF As Double = Me.calcAveragePrevYearF(iCurrentTime)

        If MeanPrevYearF > HCR_F Then
            Return True
        Else
            Return False
        End If


    End Function

    '    Public Sub calcFsfromTimeFrameRules(iCurrentTimestep As Integer)
    '        'It performs this during the very first timestep of the projection and saves the results to an array for 
    '        ' extraction at the beginning of each year

    '        Dim MeanHindcastF As Double
    '        Dim Fmsy As Double = m_HCR.MaxF
    '        Dim Interval As Double

    '        MeanHindcastF = calcAveragePrevYearF(iCurrentTimestep)

    '#If DEBUG Then
    '        Dim strmWriter As StreamWriter
    '        Dim strFile As String = cFileUtils.ToValidFileName("Diagnostics_F_Steps.csv", False)
    '        strmWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(m_MSE.DataPath, cMSEUtils.eMSEPaths.Results, strFile), True)
    '        strmWriter.WriteLine(m_MSE.CurrentModelID & "," & m_MSE.currentStrategy.Name & "," & Me.m_HCR.GroupF.Name & "," & MeanHindcastF)
    '        strmWriter.Close()
    '        strmWriter.Dispose()
    '#End If

    '        If MeanHindcastF > Fmsy Then
    '            FGreaterThanHCRF = True
    '            If NYears > 0 Then
    '                Interval = (MeanHindcastF - Fmsy) / NYears
    '                ReDim m_F(NYears - 1)
    '                For iYear = 1 To NYears
    '                    m_F(iYear - 1) = MeanHindcastF - iYear * Interval
    '                Next iYear

    '            End If
    '        End If
    '    End Sub


End Class
