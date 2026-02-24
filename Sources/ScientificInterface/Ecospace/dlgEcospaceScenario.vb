' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports EwEUtils.Utilities
Imports ScientificInterface.Wizard
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Dialog implementing a <see cref="dlgScenario">scenario dialog</see> for
    ''' interacting with Ecospace scenarios.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgEcospaceScenario
        Inherits dlgScenario

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this dialog.
        ''' </summary>
        ''' <param name="mode"><see cref="eDialogModeType">Dialog interaction mode</see>.</param>
        ''' <param name="scenario"><see cref="cEcoSpaceScenario">Ecospace scenario</see> to save, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext,
                       mode As eDialogModeType,
                       Optional scenario As cEcospaceScenario = Nothing)
            MyBase.New(uic, mode, scenario)
        End Sub

        Protected Overrides Function GetIcon() As System.Drawing.Icon
            Return SharedResources.Ecospace
        End Function

        Protected Overrides Function GetAvailableScenarios() As List(Of cEwEScenario)
            Dim lscenarios As New List(Of cEwEScenario)

            For iScenario As Integer = 1 To Me.UIContext.Core.nEcospaceScenarios
                lscenarios.Add(Me.UIContext.Core.EcospaceScenarios(iScenario))
            Next
            Return lscenarios
        End Function

        Protected Overrides Function GetNewScenarioName() As String
            Return SharedResources.DEFAULT_NEWECOSPACESCENARIO
        End Function

        Protected Overrides Function GetDialogCaption(mode As Wizard.dlgScenario.eDialogModeType, strEwEModelName As String) As String
            Dim strCaption As String = ""
            Select Case mode
                Case eDialogModeType.CreateScenario
                    strCaption = My.Resources.ECOSPACE_SCENARIO_NEW_CAPTION
                Case eDialogModeType.DeleteScenario
                    strCaption = My.Resources.ECOSPACE_SCENARIO_DELETE_CAPTION
                Case eDialogModeType.LoadScenario
                    strCaption = My.Resources.ECOSPACE_SCENARIO_LOAD_CAPTION
                Case eDialogModeType.SaveScenario
                    strCaption = My.Resources.ECOSPACE_SCENARIO_SAVEAS_CAPTION
            End Select
            Return cStringUtils.Localize(strCaption, strEwEModelName)
        End Function

        Protected Overrides Function DeleteScenario(scenario As EwECore.cEwEScenario) As Boolean
            Return Me.UIContext.Core.RemoveEcospaceScenario(scenario.Index)
        End Function

    End Class

End Namespace