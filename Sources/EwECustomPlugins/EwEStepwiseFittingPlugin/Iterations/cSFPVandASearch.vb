' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.FitToTimeSeries



Public Class cSFPVandASearch
    Inherits cSFPGenericIteration

    Public Sub New(BOrF As ISFPIteration.eBaseSearchMode, estimatedParameters As Integer, sps As Integer)
        Me.BaseSearchMode = BOrF
        Me.k = estimatedParameters + sps
        Me.EstimatedV = estimatedParameters
        Me.SplinePoints = sps
    End Sub

    Public Overrides Function Load(core As cCore) As Boolean

        Dim bOK As Boolean = False
        If Not MyBase.Load(core) Then Return bOK

        'Enable specific time series for Baseline or Fishing
        If Me.EnableTimeSeries(core) Then
            'Reset vunerabilities
            If Me.ResetVs(core) And Me.ResetFF(core) Then
                'Run a sensitivity of SS to V search for baseline
                If Me.RunSensitivityOfSSToV(core) Then
                    bOK = True
                End If
            End If
        End If

        Return bOK

    End Function

    Public Overrides Function Run(core As cCore) As Boolean
        If Me.RunVandASearch(core) Then
            Return MyBase.Run(core)
        End If
        Return False
    End Function

End Class
