Public Class cResultsCollector_TotalCatch
    Inherits cResultsCollector_Catch

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Catch Rate (" & ScientificInterfaceShared.My.Resources.UNIT_CURRENCY_WETWEIGHT & "/year)"
        End Get
    End Property

    Public Overloads Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Return m_MSE.CatchesThroughoutProjection(iGrp, iFleet, iTime)
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return (m_MSE.NYearsProject * m_MSE.EcosimData.NumStepsPerYear)
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "TotalCatch_"
        End Get
    End Property

End Class
