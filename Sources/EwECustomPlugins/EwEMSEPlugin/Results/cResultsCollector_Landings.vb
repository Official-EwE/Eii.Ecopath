' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class cResultsCollector_Landings
    Inherits cResultsCollector_Catch


    Public Overrides ReadOnly Property DataName As String
        Get
            Dim fmt As New EwECore.Style.cCurrencyUnitFormatter("")
            Return "Landings Rate (" & fmt.ToString(eUnitCurrencyType.WetWeight) & "/year)"
        End Get
    End Property

    'Public Overrides Sub Populate()

    '    Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
    '    For igrp = 1 To m_MSE.Core.nGroups
    '        For iFleet = 1 To m_MSE.Core.nFleets
    '            For iTime = 1 To m_nTimeRecords
    '                Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime)
    '                Me.SetValue(StrategyIndex, igrp, 0, iTime) = Me.GetValue(StrategyIndex, igrp, 0, iTime) + m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime) 'Summing across fleets
    '                Me.SetValue(StrategyIndex, 0, iFleet, iTime) = Me.GetValue(StrategyIndex, 0, iFleet, iTime) + m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime) 'Summing across groups
    '                Me.SetValue(StrategyIndex, 0, 0, iTime) = Me.GetValue(StrategyIndex, 0, 0, iTime) + m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime) ' summ across both fleets and groups
    '            Next
    '        Next
    '    Next

    'End Sub

    Public Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Return Me.m_MSE.LandingsThroughoutProjection(iGrp, iFleet, iTime)
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return (Me.m_MSE.NYearsProject * Me.m_MSE.EcosimData.NumStepsPerYear)
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "Landings_"
        End Get
    End Property

End Class
