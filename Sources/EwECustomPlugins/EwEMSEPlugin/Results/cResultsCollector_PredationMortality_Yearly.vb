Imports EwECore

Public Class cResultsCollector_PredationMortality_Yearly
    Inherits cResultsCollector_PredationMortality

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim TempTotalPredationMortality As Double

        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For iPred = 1 To m_MSE.Core.nGroups
            For iPrey = 1 To m_MSE.Core.nGroups
                For iTime = 1 To NumberOfTimeRecords
                    TempTotalPredationMortality = 0
                    For iMonth = 1 To 12
                        TempTotalPredationMortality += m_MSE.EcosimData.PredPreyResultsOverTime(cEcosimDatastructures.eEcosimPreyPredResults.Consumption, iPrey, iPred, (iTime - 1) * 12 + iMonth) /
                                                            m_MSE.EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iPrey, (iTime - 1) * 12 + iMonth)
                    Next

                    TempTotalPredationMortality /= 12

                    Me.SetValue(StrategyIndex, iPred, iPrey, iTime) = TempTotalPredationMortality
                    Me.SetValue(StrategyIndex, iPred, 0, iTime) = Me.GetValue(StrategyIndex, iPred, 0, iTime) + TempTotalPredationMortality 'Summing across prey
                    Me.SetValue(StrategyIndex, 0, iPrey, iTime) = Me.GetValue(StrategyIndex, 0, iPrey, iTime) + TempTotalPredationMortality 'Summing across predators
                    Me.SetValue(StrategyIndex, 0, 0, iTime) = Me.GetValue(StrategyIndex, 0, 0, iTime) + TempTotalPredationMortality ' sum across both predators and prey
                Next
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
            Return "PredationMortalityYearly_"
        End Get
    End Property

End Class
