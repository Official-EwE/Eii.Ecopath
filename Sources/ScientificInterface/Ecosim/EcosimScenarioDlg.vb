'==============================================================================
'
' $Log: EcosimScenarioDlg.vb,v $
' Revision 1.2  2008/12/15 15:52:26  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:34  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Wizard
Imports ScientificInterface.Ecosim

#End Region ' Imports

Namespace Ecosim

    ''' <summary>
    ''' Dialog implementing a <see cref="EwEScenarioDlg">EwEScenarioDlg</see> for
    ''' interacting with Ecosim scenarios.
    ''' </summary>
    Public Class EcosimScenarioDlg
        Inherits EwEScenarioDlg

        ''' <summary>
        ''' Constructor, initializes a new instance of this dialog.
        ''' </summary>
        ''' <param name="mode"><see cref="eDialogModeType">Dialog interaction mode</see>.</param>
        ''' <param name="scenario"><see cref="cEcoSpaceScenario">Ecosim scenario</see> to save, if any.</param>
        Public Sub New(ByVal mode As eDialogModeType, _
                Optional ByVal scenario As cEcoSimScenario = Nothing)
            MyBase.New(mode, scenario)
        End Sub

        Protected Overrides Function GetIcon() As System.Drawing.Icon
            Return My.Resources.Ecosim3
        End Function

        Protected Overrides Function GetAvailableScenarios() As List(Of cEwEScenario)
            Dim lscenarios As New List(Of cEwEScenario)

            For iScenario As Integer = 1 To Me.m_core.EcosimScenarioCount
                lscenarios.Add(Me.m_core.EcosimScenarios(iScenario))
            Next
            Return lscenarios
        End Function

        Protected Overrides Function GetNewScenarioName() As String
            Return My.Resources.DEFAULT_NEWECOSIMSCENARIO
        End Function

        Protected Overrides Function GetDialogCaption(ByVal mode As Wizard.EwEScenarioDlg.eDialogModeType, ByVal strEwEModelName As String) As String
            Dim strCaption As String = ""
            Select Case mode
                Case eDialogModeType.CreateScenario
                    strCaption = My.Resources.ECOSIM_SCENARIO_NEW_CAPTION
                Case eDialogModeType.DeleteScenario
                    strCaption = My.Resources.ECOSIM_SCENARIO_DELETE_CAPTION
                Case eDialogModeType.LoadScenario
                    strCaption = My.Resources.ECOSIM_SCENARIO_LOAD_CAPTION
                Case eDialogModeType.SaveScenario
                    strCaption = My.Resources.ECOSIM_SCENARIO_SAVEAS_CAPTION
            End Select
            Return String.Format(strCaption, strEwEModelName)
        End Function

        Protected Overrides Function DeleteScenario(ByVal scenario As EwECore.cEwEScenario) As Boolean
            Return Me.m_core.RemoveEcosimScenario(scenario.Index)
        End Function

    End Class

End Namespace