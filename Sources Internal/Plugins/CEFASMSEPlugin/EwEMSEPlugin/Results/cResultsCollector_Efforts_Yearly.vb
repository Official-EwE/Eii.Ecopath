Public Class cResultsCollector_Efforts_Yearly
    Inherits cResultsCollector_Efforts

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.OriginalNTimesteps / m_MSE.EcosimData.NumStepsPerYear + m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim TempTotalEffort As Double

        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For iFleet = 1 To m_MSE.Core.nFleets
            For iTime = 1 To NumberOfTimeRecords
                TempTotalEffort = 0
                For iMonth = 1 To 12
                    TempTotalEffort += m_MSE.EcosimData.ResultsEffort(iFleet, (iTime - 1) * 12 + iMonth)
                Next
                TempTotalEffort /= 12
                Me.SetValue(StrategyIndex, iFleet, iTime) = TempTotalEffort
            Next
        Next

    End Sub

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "EffortsYearly_"
        End Get
    End Property

End Class
