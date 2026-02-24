' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cResultsCollector_TotalCatch_Yearly
    Inherits cResultsCollector_Catch

    Public Overloads Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Dim TempTotalGroupFleetDiscardRate As Double = 0
            For iMonth = 1 To 12
                TempTotalGroupFleetDiscardRate += Me.m_MSE.CatchesThroughoutProjection(iGrp, iFleet, (iTime - 1) * 12 + iMonth)
            Next
            TempTotalGroupFleetDiscardRate /= 12
            Return TempTotalGroupFleetDiscardRate
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Dim fmt As New EwECore.Style.cCurrencyUnitFormatter("")
            Return "Catch Rate (" & fmt.ToString(eUnitCurrencyType.WetWeight) & "/year)"
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "TotalCatchYearly_"
        End Get
    End Property

End Class
