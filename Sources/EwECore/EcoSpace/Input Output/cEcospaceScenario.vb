Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceScenario
    Inherits cEwEScenario

#Region " Constructor "

    Sub New(ByVal theCore As cCore)
        MyBase.New(theCore)
        Me.m_dataType = eDataTypes.EcoSpaceScenario
        Me.m_ValidationStatus.DataType = Me.m_dataType
    End Sub

#End Region ' Constructor

    Public Overrides Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveEcospaceScenarioIndex = Me.Index)
    End Function

End Class
