Option Explicit On


Imports EwECore
Imports System.IO
Imports EwEUtils.Utilities

Public Class cTimeFrameRule
    'This object handles the application of a time frame rule.
    'A time frame rule is a rule that overrides the HCR and steps the F used to calc the quota down uniformly from the F during the last year of hindcast
    'to the maximum F (Fmsy) of the harvest control rule over a user specified number of years
    'This object is created for each HCR but only does anything if the number of years it is specified for is > 0

    Private m_F() As Double
    Private m_nTimeStepsInHindcast As Integer
    Private m_EcosimData As cEcosimDatastructures
    Private m_HCR As HCR_Group
    Private FGreaterThanFmsy As Boolean
    Private m_MSE As cMSE

    Public Property NYears As Integer

    Public Sub New(ByRef EcosimDatastructures As cEcosimDatastructures, ByRef HCR As HCR_Group, ByRef MSE As cMSE)
        m_EcosimData = EcosimDatastructures
        m_HCR = HCR
        m_nTimeStepsInHindcast = EcosimDatastructures.NTimes
        m_MSE = MSE
    End Sub


    Public Property ExtractF(iYearProjecting) As Double
        Get
            Return m_F(iYearProjecting - 1)
        End Get
        Set(value As Double)
            m_F(iYearProjecting - 1) = value
        End Set
    End Property

    Public Function CheckValidRule(iYearProjecting As Integer) As Boolean
        'A time frame rule is valid only if the number of years field for it is >0 and the year into projection is 1 upto that number
        'and also the F during the last year of the hindcast is greater than the Fmsy

        If NYears > 0 And iYearProjecting >= 1 And iYearProjecting <= NYears And FGreaterThanFmsy Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function calcAverageFLastYearHindCast(iCurrentTimestep As Integer)

        Dim MeanF As Double
        Dim BiomassAtT As Double
        Dim Q As Double
        Dim GroupIndex As Integer = m_HCR.GroupF.Index

        Debug.Assert(iCurrentTimestep > 12, "TimeFrameRules must have a hind cast period > 12 months. See cTimeFrameRule.calcAverageFLastYearHindCast()")

        'Get the average from the last year of the hindcast
        'iCurrentTimestep is the first time step of the forecast
        For iTimeStep = (iCurrentTimestep - 12) To (iCurrentTimestep - 1)
            BiomassAtT = m_EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, GroupIndex, iTimeStep)
            Q = m_EcosimData.QmQo(GroupIndex) / (1 + (m_EcosimData.QmQo(GroupIndex) - 1) * BiomassAtT / m_EcosimData.StartBiomass(GroupIndex))
            MeanF += m_EcosimData.FishRateNo(m_HCR.GroupF.Index, iTimeStep) ' * Q
        Next
        MeanF /= 12

        Return MeanF

    End Function

    Public Sub calcFsfromTimeFrameRules(iCurrentTimestep As Integer)
        'It performs this during the very first timestep of the projection and saves the results to an array for 
        ' extraction at the beginning of each year

        Dim MeanHindcastF As Double
        Dim Fmsy As Double = m_HCR.MaxF
        Dim Interval As Double

        MeanHindcastF = calcAverageFLastYearHindCast(iCurrentTimestep)

#If DEBUG Then
        Dim strmWriter As StreamWriter
        Dim strFile As String = cFileUtils.ToValidFileName("Diagnostics_F_Steps.csv", False)
        strmWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(m_MSE.DataPath, cMSEUtils.eMSEPaths.Results, strFile), True)
        strmWriter.WriteLine(m_MSE.CurrentModelID & "," & m_MSE.currentStrategy.Name & "," & Me.m_HCR.GroupF.Name & "," & MeanHindcastF)
        strmWriter.Close()
        strmWriter.Dispose()
#End If

        If MeanHindcastF > Fmsy Then
            FGreaterThanFmsy = True
            If NYears > 0 Then
                Interval = (MeanHindcastF - Fmsy) / NYears
                ReDim m_F(NYears - 1)
                For iYear = 1 To NYears
                    m_F(iYear - 1) = MeanHindcastF - iYear * Interval
                Next iYear

            End If
        End If
    End Sub


End Class
