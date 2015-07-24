Imports EwEUtils.Utilities

Public Class cResultsCollector_HighestValueGroup
    Inherits cResultsCollector_1DArray

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Highest Value Group"
        End Get
    End Property

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return "NA"
        End Get
    End Property

    Public Overrides ReadOnly Property Dim_Name As String
        Get
            Return "Fleet"
        End Get
    End Property

    Public Overrides ReadOnly Property ElementName(iElement As Integer) As String
        Get
            Return m_MSE.Core.FleetInputs(iElement).Name
        End Get
    End Property

    Public Overrides ReadOnly Property nElements As Integer
        Get
            Return m_MSE.Core.nFleets
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For iFleet = 1 To m_MSE.Core.nFleets
            For iTime = 1 To NumberOfTimeRecords
                Me.SetValue(StrategyIndex, iFleet, iTime) = m_MSE.HighestValueGroup(iFleet, iTime)
            Next
        Next
    End Sub

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.ToCSVField(GetValue(iStrategy, iElement, iTime))
        End Get
    End Property

End Class
