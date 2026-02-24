' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cEcospaceScenario
    Inherits cEwEScenario

#Region " Constructor "

    Sub New(theCore As cCore)
        MyBase.New(theCore)
        Me.m_dataType = eDataTypes.EcoSpaceScenario
        Me.m_ValidationStatus.DataType = Me.m_dataType
    End Sub

#End Region ' Constructor

    Public Overrides Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveEcospaceScenarioIndex = Me.Index)
    End Function

End Class
