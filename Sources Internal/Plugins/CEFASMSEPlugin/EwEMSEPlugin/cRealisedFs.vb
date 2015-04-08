Public Class cRealisedFs

    Inherits cResultsArrayGroups

    Private Const DefaultValue = -9999

    Public Sub New()

    End Sub

    Public Overrides Sub Initialise(MSE As cMSE)

        m_NumberOfTimeRecords = MSE.NYearsProject * MSE.EcosimData.NumStepsPerYear
        m_MSE = MSE
        SetSize(MSE.Strategies.Count, MSE.Core.nGroups, m_NumberOfTimeRecords)
        SetDefaults(DefaultValue)

    End Sub


    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Realised F's"
        End Get
    End Property


    Public Overrides Sub Populate()

        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To m_MSE.Core.nGroups
            For iTime = 1 To m_NumberOfTimeRecords
                Me.Value(StrategyIndex, igrp, iTime) = m_MSE.RealisedFs(igrp, iTime)
            Next
        Next

    End Sub


End Class
