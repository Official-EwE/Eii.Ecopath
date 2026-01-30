' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cResultsCollector_RealisedLandedFs_Yearly
    Inherits cResultsCollector_RealisedLandedFs

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.NYearsProject
        End Get
    End Property

    Protected Overrides ReadOnly Property RealisedF(iGrp As Integer, iTime As Integer) As Double
        Get
            Dim TempTotalGroupRealisedF As Double = 0
            For iMonth = 1 To 12
                TempTotalGroupRealisedF += Me.m_MSE.RealisedLandedFs(iGrp, (iTime - 1) * 12 + iMonth)
            Next
            TempTotalGroupRealisedF /= 12
            Return TempTotalGroupRealisedF
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "RealisedLandedFsYearly_"
        End Get
    End Property

End Class
