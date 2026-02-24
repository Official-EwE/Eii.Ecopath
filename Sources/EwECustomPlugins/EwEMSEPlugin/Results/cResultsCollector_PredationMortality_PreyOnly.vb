' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwEUtils.Utilities

Public Class cResultsCollector_PredationMortality_PreyOnly
    Inherits cResultsCollector_1DArray

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "PredationMortality"
        End Get
    End Property

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return 0
        End Get
    End Property

    Public Overrides Sub Populate()

        Dim StrategyIndex = Me.m_MSE.Strategies.IndexOf(Me.m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index

        For iPrey = 1 To Me.m_MSE.Core.nGroups
            For iTime = 1 To Me.NumberOfTimeRecords
                Me.SetValue(StrategyIndex, iPrey, iTime) = Me.m_MSE.EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.PredMort, iPrey, iTime)
            Next
        Next

    End Sub

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.NYearsProject * Me.m_MSE.EcosimData.NumStepsPerYear
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "PredationMortalityPreyOnly_"
        End Get
    End Property

    Public Overrides ReadOnly Property Dim_Name As String
        Get
            Return "Prey"
        End Get
    End Property

    Public Overrides ReadOnly Property nElements As Integer
        Get
            Return Me.m_MSE.Core.nGroups
        End Get
    End Property

    Public Overrides ReadOnly Property ElementName(iElement As Integer) As String
        Get
            Return Me.m_MSE.Core.EcopathGroupInputs(iElement).Name
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.FormatNumber(Me.GetValue(iStrategy, iElement, iTime))
        End Get
    End Property
End Class
