' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwEUtils.Utilities

Public Class cSFPGroupsWithTimeSeries
    Inherits cSFPGenericIteration

    Public Sub New(baseSearchMode As ISFPIteration.eBaseSearchMode)
        Me.BaseSearchMode = baseSearchMode
    End Sub

    Public Overrides Function Load(core As cCore) As Boolean

        Dim bOK As Boolean = False
        If Not MyBase.Load(core) Then Return bOK

        'Enable specific time series for Baseline or Fishing
        If Me.EnableTimeSeries(core) Then
            'Reset vunerabilities
            If MyBase.ResetVs(core) And MyBase.ResetFF(core) Then
                'Run a sensitivity of SS to V search for baseline
                Dim man As cF2TSManager = core.EcosimFitToTimeSeries
                man.SetNBlocksForGroupsWithTimeSeries()
                Me.Report(cStringUtils.Localize(My.Resources.REPORT_V_FOR_GROUPS_WITH_TIMESERIES, man.nBlockCodes), eReportState.Success)
                bOK = True
            End If
        End If

        Return bOK

    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return cStringUtils.Localize(My.Resources.NAME_WITH_TS_ONLY, MyBase.Name)
        End Get
    End Property

    Public Overrides ReadOnly Property IsGroupsWithTimeSeriesOnly As Boolean
        Get
            Return True
        End Get
    End Property

End Class
