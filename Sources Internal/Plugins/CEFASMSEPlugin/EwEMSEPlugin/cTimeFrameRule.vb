Imports EwECore

Public Class cTimeFrameRule

    Private m_F() As Double
    Private m_nTimeStepsInHindcast As Integer
    Private m_EcosimData As cEcosimDatastructures
    Private m_HCR As HCR_Group

    Public Property NYears As Integer

    Public ReadOnly Property F(ByVal iYear As Integer)
        Get
            Return m_F(iYear - 1)
        End Get
    End Property

    Public Sub New(ByRef EcosimDatastructures As cEcosimDatastructures, ByRef HCR As HCR_Group, ByVal nTimeStepsInHindcast As Integer)
        m_EcosimData = EcosimDatastructures
        m_HCR = HCR
        m_nTimeStepsInHindcast = nTimeStepsInHindcast
    End Sub


    Public Property ExtractF(iTime) As Double
        Get
            Return m_F((iTime - 1 - m_nTimeStepsInHindcast) \ 12)
        End Get
        Set(value As Double)
            m_F((iTime - 1 - m_nTimeStepsInHindcast) \ 12) = value
        End Set
    End Property

    Public Function CheckValidRule(iTimeStep As Integer) As Boolean

        Dim TimeFrameRuleTimeStepBegin As Integer = m_nTimeStepsInHindcast + 1
        Dim TimeFrameRuleTimeStepEnd As Integer = m_nTimeStepsInHindcast + 12 * NYears

        If NYears > 0 And iTimeStep >= TimeFrameRuleTimeStepBegin And iTimeStep <= TimeFrameRuleTimeStepEnd Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function calcAverageFLastYearHindCast()

        Dim MeanF As Double

        For iTimeStep = 1 To 12
            MeanF += m_EcosimData.FishRateNo(m_HCR.GroupF.Index, m_nTimeStepsInHindcast - iTimeStep + 1) / 12
        Next

        Return MeanF

    End Function

    Public Sub calcFsfromTimeFrameRules()

        Dim MeanHindcastF As Double
        Dim Fmsy As Double = m_HCR.MaxF
        Dim Interval As Double

        MeanHindcastF = calcAverageFLastYearHindCast()

        If MeanHindcastF > Fmsy And NYears > 0 Then
            Interval = (MeanHindcastF - Fmsy) / NYears
            ReDim m_F(NYears - 1)
            For iYear = 1 To NYears
                m_F(iYear - 1) = MeanHindcastF - iYear * Interval
            Next iYear

        End If

    End Sub


End Class
