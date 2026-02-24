' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict Off

Public Class cResultsCollector_HCR_Quota_Targ
    Inherits cResultsCollector_HCR_Quota

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Target Quota"
        End Get
    End Property

    Public Overrides Sub Populate()

        Dim StrategyIndex = Me.m_MSE.Strategies.IndexOf(Me.m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To Me.m_MSE.Core.nGroups
            For iFleet = 1 To Me.m_MSE.Core.nFleets
                For iTime = 1 To Me.NumberOfTimeRecords
                    Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = Me.m_MSE.HCR_Quota_Target(igrp, iFleet, iTime)
                    If Me.GetValue(StrategyIndex, igrp, 0, iTime) = Me.DefaultValue And Me.m_MSE.HCR_Quota_Target(igrp, iFleet, iTime) <> Me.DefaultValue Then
                        Me.SetValue(StrategyIndex, igrp, 0, iTime) = 0
                    End If
                    If Me.GetValue(StrategyIndex, igrp, 0, iTime) <> Me.DefaultValue And Me.m_MSE.HCR_Quota_Target(igrp, iFleet, iTime) <> Me.DefaultValue Then
                        Me.SetValue(StrategyIndex, igrp, 0, iTime) = Me.GetValue(StrategyIndex, igrp, 0, iTime) + Me.m_MSE.HCR_Quota_Target(igrp, iFleet, iTime) 'Summing across fleets
                    End If
                Next
            Next
        Next

    End Sub

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
            Return "HCR_Quota_Targ_"
        End Get
    End Property

End Class
