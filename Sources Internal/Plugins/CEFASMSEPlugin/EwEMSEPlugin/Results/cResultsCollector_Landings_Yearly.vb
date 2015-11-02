Public Class cResultsCollector_Landings_Yearly
    Inherits cResultsCollector_Catch

    Public Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Dim TempTotalGroupFleetLandingsRate As Double = 0
            For iMonth = 1 To 12
                TempTotalGroupFleetLandingsRate += m_MSE.LandingsThroughoutProjection(iGrp, iFleet, (iTime - 1) * 12 + iMonth)
            Next
            TempTotalGroupFleetLandingsRate /= 12
            Return TempTotalGroupFleetLandingsRate
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Landings Rate (" & ScientificInterfaceShared.My.Resources.UNIT_CURRENCY_WETWEIGHT & "/year)"
        End Get
    End Property

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

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "LandingsYearly_"
        End Get
    End Property

End Class
