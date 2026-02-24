' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict Off

Public Class cResultsCollector_Efforts_Yearly
    Inherits cResultsCollector_Efforts

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.OriginalNTimesteps / Me.m_MSE.EcosimData.NumStepsPerYear + Me.m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim TempTotalEffort As Double

        Dim StrategyIndex = Me.m_MSE.Strategies.IndexOf(Me.m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For iFleet = 1 To Me.m_MSE.Core.nFleets
            For iTime = 1 To Me.NumberOfTimeRecords
                TempTotalEffort = 0
                For iMonth = 1 To 12
                    TempTotalEffort += Me.m_MSE.EcosimData.ResultsEffort(iFleet, (iTime - 1) * 12 + iMonth)
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
