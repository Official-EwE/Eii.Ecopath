Public Class cResultsCollector_RealisedDiscardedFs
    Inherits cResultsCollector_RealisedFs

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Realised Discarded F"
        End Get
    End Property



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
            Return m_MSE.RealisedDiscardFs(iGrp, iTime)
        End Get
    End Property
End Class
