' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.FitToTimeSeries



Public Class cSFPAnomalySearch
    Inherits cSFPGenericIteration

    Public Sub New(baseSearchMode As ISFPIteration.eBaseSearchMode, sps As Integer)
        Me.BaseSearchMode = baseSearchMode
        Me.k = sps
        Me.SplinePoints = Me.k
    End Sub

    Public Overrides Function Load(core As cCore) As Boolean

        If Not MyBase.Load(core) Then Return False

        'Enable specific time series for Baseline or Fishing
        If Not Me.EnableTimeSeries(core) Then Return False

        'Reset vunerabilities
        Return Me.ResetVs(core) And Me.ResetFF(core)

    End Function

    Public Overrides Function Run(core As cCore) As Boolean
        If Not Me.RunAnomalySearch(core) Then Return False
        Return MyBase.Run(core)
    End Function

End Class
