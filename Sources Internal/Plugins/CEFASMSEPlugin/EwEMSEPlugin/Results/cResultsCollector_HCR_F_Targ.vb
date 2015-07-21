Public Class cResultsCollector_HCR_F_Targ
    Inherits cResultsCollector_HCR_F

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Target F"
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To m_MSE.Core.nGroups
            For iTime = 1 To NumberOfTimeRecords
                Me.SetValue(StrategyIndex, igrp, iTime) = m_MSE.HCR_F_Target(igrp, iTime)
            Next
        Next
    End Sub

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property
End Class
