Imports EwECore
Imports EwEUtils.Utilities

Public Class cResultsCollector_Biomass
    Inherits cResultsCollector_1DArray

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Biomass"
        End Get
    End Property

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return -9999
        End Get
    End Property

    Public Overrides ReadOnly Property Dim_Name As String
        Get
            Return "Group"
        End Get
    End Property

    Public Overrides ReadOnly Property ElementName(iElement As Integer) As String
        Get
            Return m_MSE.Core.EcoPathGroupInputs(iElement).Name
        End Get
    End Property

    Public Overrides ReadOnly Property nElements As Integer
        Get
            Return m_MSE.Core.nGroups
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To m_MSE.Core.nGroups
            For iTime = 1 To NumberOfTimeRecords
                Me.SetValue(StrategyIndex, igrp, iTime) = m_MSE.EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime)
            Next
        Next
    End Sub

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.OriginalNTimesteps + m_MSE.NYearsProject * m_MSE.EcosimData.NumStepsPerYear
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.FormatNumber(GetValue(iStrategy, iElement, iTime))
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "Biomass_"
        End Get
    End Property
End Class
