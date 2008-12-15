'==============================================================================
'
' $Log: EcotracerScenarioDlg.vb,v $
' Revision 1.2  2008/12/15 15:52:27  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:32:03  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Wizard
Imports ScientificInterface.Ecotracer

#End Region ' Imports

Namespace Ecotracer

    ''' <summary>
    ''' Dialog implementing a <see cref="EwEScenarioDlg">EwEScenarioDlg</see> for
    ''' interacting with Ecotracer scenarios.
    ''' </summary>
    Public Class EcotracerScenarioDlg
        Inherits EwEScenarioDlg

        ''' <summary>
        ''' Constructor, initializes a new instance of this dialog.
        ''' </summary>
        ''' <param name="mode"><see cref="eDialogModeType">Dialog interaction mode</see>.</param>
        ''' <param name="scenario"><see cref="cEcoSpaceScenario">Ecotracer scenario</see> to save, if any.</param>
        Public Sub New(ByVal mode As eDialogModeType, _
                Optional ByVal scenario As cEcotracerScenario = Nothing)
            MyBase.New(mode, scenario)
        End Sub

        Protected Overrides Function GetIcon() As System.Drawing.Icon
            Return My.Resources.Ecotracer
        End Function

        Protected Overrides Function GetAvailableScenarios() As List(Of cEwEScenario)
            Dim lscenarios As New List(Of cEwEScenario)

            For iScenario As Integer = 1 To Me.m_core.EcotracerScenarioCount
                lscenarios.Add(Me.m_core.EcotracerScenarios(iScenario))
            Next
            Return lscenarios
        End Function

        Protected Overrides Function GetNewScenarioName() As String
            Return My.Resources.DEFAULT_NEWECOTRACERSCENARIO
        End Function

        Protected Overrides Function GetDialogCaption(ByVal mode As Wizard.EwEScenarioDlg.eDialogModeType, ByVal strEwEModelName As String) As String
            Dim strCaption As String = ""
            Select Case mode
                Case eDialogModeType.CreateScenario
                    strCaption = My.Resources.ECOTRACER_SCENARIO_NEW_CAPTION
                Case eDialogModeType.DeleteScenario
                    strCaption = My.Resources.ECOTRACER_SCENARIO_DELETE_CAPTION
                Case eDialogModeType.LoadScenario
                    strCaption = My.Resources.ECOTRACER_SCENARIO_LOAD_CAPTION
                Case eDialogModeType.SaveScenario
                    strCaption = My.Resources.ECOTRACER_SCENARIO_SAVEAS_CAPTION
            End Select
            Return String.Format(strCaption, strEwEModelName)
        End Function

        Protected Overrides Function DeleteScenario(ByVal scenario As EwECore.cEwEScenario) As Boolean
            Return Me.m_core.RemoveEcotracerScenario(scenario.Index)
        End Function

    End Class

End Namespace