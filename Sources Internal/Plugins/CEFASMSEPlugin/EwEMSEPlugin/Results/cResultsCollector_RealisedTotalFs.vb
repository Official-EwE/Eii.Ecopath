Public Class cResultsCollector_RealisedTotalFs
    Inherits cResultsCollector_RealisedFs

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Realised Total F"
        End Get
    End Property

    'Public Overrides Sub Populate()

    '    Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
    '    For igrp = 1 To m_MSE.Core.nGroups
    '        For iTime = 1 To NumberOfTimeRecords
    '            Me.SetValue(StrategyIndex, igrp, iTime) = Me.RealisedF(igrp, iTime)
    '            'Me.SetValue(StrategyIndex, igrp, iTime) = m_MSE.RealisedFs(igrp, iTime)
    '        Next
    '    Next

    'End Sub

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject * m_MSE.EcosimData.NumStepsPerYear
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides ReadOnly Property RealisedF(iGrp As Integer, iTime As Integer) As Double
        Get
            Return m_MSE.RealisedFs(iGrp, iTime)
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "RealisedTotalFs_"
        End Get
    End Property

End Class
