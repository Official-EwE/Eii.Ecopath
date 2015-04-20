Imports EwEUtils.Utilities

Public MustInherit Class cResultsCollector_RealisedFs

    Inherits cResultsCollector_1DArray

    Protected MustOverride ReadOnly Property RealisedF(iGrp As Integer, iTime As Integer) As Double

    Public Sub New()
        MyBase.new()
    End Sub

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return -9999
        End Get
    End Property

    Public Overrides Sub Initialise(MSE As cMSE)

        m_MSE = MSE
        SetSize(MSE.Strategies.Count, MSE.Core.nGroups, NumberOfTimeRecords)

    End Sub

    Public Overrides ReadOnly Property Dim_Name As String
        Get
            Return "Group"
        End Get
    End Property

    Public Overrides ReadOnly Property nElements As Integer
        Get
            Return m_MSE.Core.nGroups
        End Get
    End Property

    Public Overrides ReadOnly Property ElementName(iElement As Integer) As String
        Get
            Return m_MSE.Core.EcoPathGroupInputs(iElement).Name
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.FormatNumber(GetValue(iStrategy, iElement, iTime))
        End Get
    End Property

    Public Overrides Sub Populate()

        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To m_MSE.Core.nGroups
            For iTime = 1 To NumberOfTimeRecords
                Me.SetValue(StrategyIndex, igrp, iTime) = RealisedF(igrp, iTime)
            Next
        Next
    End Sub

End Class
