#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Wizard
Imports ScientificInterface.Ecospace

#End Region ' Imports

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Dialog implementing a <see cref="EwEScenarioDlg">EwEScenarioDlg</see> for
    ''' interacting with Ecospace scenarios.
    ''' </summary>
    ''' =======================================================================
    Public Class EcospaceScenarioDlg
        Inherits EwEScenarioDlg

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this dialog.
        ''' </summary>
        ''' <param name="mode"><see cref="eDialogModeType">Dialog interaction mode</see>.</param>
        ''' <param name="scenario"><see cref="cEcoSpaceScenario">Ecospace scenario</see> to save, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal mode As eDialogModeType, _
                       Optional ByVal scenario As cEcospaceScenario = Nothing)
            MyBase.New(uic, mode, scenario)
        End Sub

        Protected Overrides Function GetIcon() As System.Drawing.Icon
            Return My.Resources.Ecospace3
        End Function

        Protected Overrides Function GetAvailableScenarios() As List(Of cEwEScenario)
            Dim lscenarios As New List(Of cEwEScenario)

            For iScenario As Integer = 1 To Me.UIContext.Core.EcospaceScenarioCount
                lscenarios.Add(Me.UIContext.Core.EcospaceScenarios(iScenario))
            Next
            Return lscenarios
        End Function

        Protected Overrides Function GetNewScenarioName() As String
            Return My.Resources.DEFAULT_NEWECOSPACESCENARIO
        End Function

        Protected Overrides Function GetDialogCaption(ByVal mode As Wizard.EwEScenarioDlg.eDialogModeType, ByVal strEwEModelName As String) As String
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
            Return String.Format(strCaption, strEwEModelName)
        End Function

        Protected Overrides Function DeleteScenario(ByVal scenario As EwECore.cEwEScenario) As Boolean
            Return Me.UIContext.Core.RemoveEcospaceScenario(scenario.Index)
        End Function

    End Class

End Namespace