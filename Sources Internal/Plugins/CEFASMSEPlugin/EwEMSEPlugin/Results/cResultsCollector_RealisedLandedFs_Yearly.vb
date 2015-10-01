Public Class cResultsCollector_RealisedLandedFs_Yearly
    Inherits cResultsCollector_RealisedLandedFs

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject
        End Get
    End Property

    Protected Overrides ReadOnly Property RealisedF(iGrp As Integer, iTime As Integer) As Double
        Get
            Dim TempTotalGroupRealisedF As Double = 0
            For iMonth = 1 To 12
                TempTotalGroupRealisedF += m_MSE.RealisedLandedFs(iGrp, (iTime - 1) * 12 + iMonth)
            Next
            TempTotalGroupRealisedF /= 12
            Return TempTotalGroupRealisedF
        End Get
    End Property

End Class
