' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Time series for a specific MPA. A time series can hold relative B data points
''' for a specific group and target area (MPA or region), or can determine local
''' biomass fluctuations from emprical rules.
''' </summary>
Public MustInherit Class cEmission

    Public Sub New(data As cData)
        Me.Data = data
    End Sub

    Public ReadOnly Property Data As cData = Nothing
    Public MustOverride Property Enable As Boolean
    Public MustOverride Function IsValid() As Boolean

End Class
