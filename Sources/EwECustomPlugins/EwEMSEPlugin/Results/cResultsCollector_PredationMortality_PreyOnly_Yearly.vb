' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class cResultsCollector_PredationMortality_PreyOnly_Yearly
    Inherits cResultsCollector_PredationMortality_PreyOnly

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim TempTotalPredationMortality As Double

        Dim StrategyIndex = Me.m_MSE.Strategies.IndexOf(Me.m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For iPrey = 1 To Me.m_MSE.Core.nGroups
            For iTime = 1 To Me.NumberOfTimeRecords
                TempTotalPredationMortality = 0
                For iMonth = 1 To 12
                    TempTotalPredationMortality = TempTotalPredationMortality + Me.m_MSE.EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.PredMort, iPrey, (iTime - 1) * 12 + iMonth)
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
