#Region " Imports "

Option Strict On
Imports System.Collections.Generic
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Interface IGraphView

    Sub SetData(ByVal strGraphTitle As String, _
                ByVal strXAxisLabel As String, ByVal aUnitsXAxis() As cStyleGuide.eUnitType, _
                ByVal strYAxisLabel As String, ByVal aUnitsYAxis() As cStyleGuide.eUnitType, _
                ByVal aVars() As cResults.eVariableType)

End Interface
