' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict Off
Imports EwECore
Imports EwEUtils.Utilities

Public Class cResultsCollector_Biomass_Yearly
    Inherits cResultsCollector_Biomass

    Public Overrides Sub Populate()
        Dim Temp As Double

        Dim StrategyIndex = Me.m_MSE.Strategies.IndexOf(Me.m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To Me.m_MSE.Core.nGroups
            For iTime = 1 To Me.NumberOfTimeRecords
                Temp = 0
                For iMonth = 1 To 12
                    Temp += Me.m_MSE.EcosimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, (iTime - 1) * 12 + iMonth)
                Next
                Temp /= 12
                Me.SetValue(StrategyIndex, igrp, iTime) = Temp
            Next
        Next

    End Sub

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.OriginalNTimesteps / Me.m_MSE.EcosimData.NumStepsPerYear + Me.m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.FormatNumber(Me.GetValue(iStrategy, iElement, iTime))
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "BiomassYearly_"
        End Get
    End Property

End Class
