Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Class to encapsulate scenario parameters for a single scenario in the cEcoSim Model
''' </summary>
Public Class cEcoSimScenario
    Inherits cEwEScenario

#Region "Constructor"

    Sub New(ByVal theCore As cCore)
        MyBase.New(theCore)
        Me.m_dataType = eDataTypes.EcoSimScenario
        Me.m_ValidationStatus.DataType = Me.m_dataType
    End Sub

#End Region

    Public Overrides Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveEcosimScenarioIndex = Me.Index)
    End Function

End Class
