' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

<HideModuleName()>
Public Module modDefinitions

    Public Enum eTrendType As Integer
        TimeSeries = 0
        Rule
    End Enum

    Public Enum eTargetType As Integer
        Region = 0
        MPA
        Habitat
    End Enum

    Public Enum eApplicationType As Integer
        Relative = 0
        Absolute
        Additive
    End Enum

    Public Enum eProtectionType As Integer
        Full = 0
        High
        Moderate
        Poor
        None
        '''' <summary>Let EwE decide the type of protection</summary>
        'Automatic
    End Enum

End Module
