' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcotracerScenario
    Inherits cEwEScenario

#Region " Constructor "

    Sub New(core As cCore)
        MyBase.New(core)
        Me.m_dataType = eDataTypes.EcotracerScenario
        Me.m_ValidationStatus.DataType = Me.m_dataType
    End Sub

#End Region ' Constructor

    Public Overrides Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveEcotracerScenarioIndex = Me.Index)
    End Function

End Class
