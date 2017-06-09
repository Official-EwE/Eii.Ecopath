Imports EwECore

Public Class cResultsCollector_PredationMortality_PreyOnly_Yearly
    Inherits cResultsCollector_PredationMortality_PreyOnly

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim TempTotalPredationMortality As Double

        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For iPrey = 1 To m_MSE.Core.nGroups
            For iTime = 1 To NumberOfTimeRecords
                TempTotalPredationMortality = 0
                For iMonth = 1 To 12
                    TempTotalPredationMortality = TempTotalPredationMortality + m_MSE.EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.PredMort, iPrey, (iTime - 1) * 12 + iMonth)
                Next

                TempTotalPredationMortality /= 12

                Me.SetValue(StrategyIndex, iPrey, iTime) = TempTotalPredationMortality
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
            Return "PredationMortalityPreyOnly_Yearly_"
        End Get
    End Property


End Class
