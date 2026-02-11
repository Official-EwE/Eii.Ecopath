' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.FitToTimeSeries

Public Class cSFPEcosimRun
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
                If MyBase.RunSensitivityOfSSToV(core) Then
                    bOK = True
                End If
            End If
        End If

        Return bOK

    End Function

End Class
