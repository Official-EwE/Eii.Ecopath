' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cResultsCollector_RealisedDiscardedFs
    Inherits cResultsCollector_RealisedFs

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Realised Discarded F"
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.NYearsProject * Me.m_MSE.EcosimData.NumStepsPerYear
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides ReadOnly Property RealisedF(iGrp As Integer, iTime As Integer) As Double
        Get
            Return Me.m_MSE.RealisedDiscardFs(iGrp, iTime)
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "RealisedDiscardFs_"
        End Get
    End Property

End Class
