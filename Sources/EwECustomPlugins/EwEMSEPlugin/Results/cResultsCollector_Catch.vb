' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict Off

Public MustInherit Class cResultsCollector_Catch
    Inherits cResultsCollector_2DArray

    Public MustOverride ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return 0
        End Get
    End Property

    Public Overrides Sub Populate()

        Dim StrategyIndex = Me.m_MSE.Strategies.IndexOf(Me.m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        Dim TempTotalGroupFleetDiscardRate As Double

        For iTime = 1 To Me.NumberOfTimeRecords
            For igrp = 1 To Me.m_MSE.Core.nGroups
                For iFleet = 1 To Me.m_MSE.Core.nFleets
                    TempTotalGroupFleetDiscardRate = 0
                    Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = Me.ResultsThroughProjection(igrp, iFleet, iTime)
                    Me.SetValue(StrategyIndex, igrp, 0, iTime) = Me.GetValue(StrategyIndex, igrp, 0, iTime) + Me.ResultsThroughProjection(igrp, iFleet, iTime) 'Summing across fleets
                    Me.SetValue(StrategyIndex, 0, iFleet, iTime) = Me.GetValue(StrategyIndex, 0, iFleet, iTime) + Me.ResultsThroughProjection(igrp, iFleet, iTime) 'Summing across groups
                    Me.SetValue(StrategyIndex, 0, 0, iTime) = Me.GetValue(StrategyIndex, 0, 0, iTime) + Me.ResultsThroughProjection(igrp, iFleet, iTime) ' summ across both fleets and groups
                Next
            Next
        Next

    End Sub

    Public Overrides ReadOnly Property TotalAcrossFleets As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TotalAcrossGroups As Boolean
        Get
            Return True
        End Get
    End Property

End Class
