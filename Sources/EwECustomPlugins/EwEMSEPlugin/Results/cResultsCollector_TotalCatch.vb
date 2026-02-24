' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Public Class cResultsCollector_TotalCatch
    Inherits cResultsCollector_Catch

    Public Overrides ReadOnly Property DataName As String
        Get
            Dim fmt As New EwECore.Style.cCurrencyUnitFormatter("")
            Return "Catch Rate (" & fmt.ToString(eUnitCurrencyType.WetWeight) & "/year)"
        End Get
    End Property

    Public Overloads Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Return Me.m_MSE.CatchesThroughoutProjection(iGrp, iFleet, iTime)
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return (Me.m_MSE.NYearsProject * Me.m_MSE.EcosimData.NumStepsPerYear)
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "TotalCatch_"
        End Get
    End Property

End Class
