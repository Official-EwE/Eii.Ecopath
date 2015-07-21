Public Class cResultsCollector_TotalCatch_Yearly
    Inherits cResultsCollector_Catch

    Public Overloads Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Dim TempTotalGroupFleetDiscardRate As Double = 0
            For iMonth = 1 To 12
                TempTotalGroupFleetDiscardRate += m_MSE.CatchesThroughoutProjection(iGrp, iFleet, (iTime - 1) * 12 + iMonth)
            Next
            TempTotalGroupFleetDiscardRate /= 12
            Return TempTotalGroupFleetDiscardRate
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Catch Rate (" & ScientificInterfaceShared.My.Resources.UNIT_CURRENCY_WETWEIGHT & "/year)"
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
End Class
