#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim
Imports EwEUtils.Commands

#End Region


Public Class frmOptions

    'ToDo_jb 19-April-2010 Change "Effort and regulatory option" to something Effort and evaluation type control type....
    Dim m_MSE As cMSEManager

    Private m_fpNTrials As cPropertyFormatProvider
    Private m_fpUsePlugin As cPropertyFormatProvider
    Private m_fpSave As cPropertyFormatProvider

    Private m_fpKalman As cPropertyFormatProvider
    Private m_fpForecast As cPropertyFormatProvider
    Private m_fpSBPower As cPropertyFormatProvider

    Private m_fpUseQuotaRegs As cPropertyFormatProvider
    Private m_dctEffortControls As Dictionary(Of eMSERegulationMode, RadioButton)

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_MSE = Me.UIContext.Core.MSEManager

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

        Me.m_fpUsePlugin = New cPropertyFormatProvider(Me.UIContext, Me.m_ckPlugin, Me.m_MSE.ModelParameters, eVarNameFlags.MSEUseEconomicPlugin)
        ' Me.m_fpSave = New cPropertyFormatProvider(Me.UIContext, Me.ckSave, Me.m_MSE.ModelParameters, eVarNameFlags.MSESave)

        'Me.m_fpForecast = New cPropertyFormatProvider(Me.UIContext, Me.txForecast, Me.m_MSE.ModelParameters, eVarNameFlags.MSEForcastGain)
        Me.m_fpSBPower = New cPropertyFormatProvider(Me.UIContext, Me.txSBPower, Me.m_MSE.ModelParameters, eVarNameFlags.MSEAssessPower)
        Me.m_fpKalman = New cPropertyFormatProvider(Me.UIContext, Me.txKalmanGain, Me.m_MSE.ModelParameters, eVarNameFlags.MSEKalmanGain)


        'Assessment methods Catch Estimated Biomass and Direct Exploitation are stored in the tag property of the radio buttons
        'see the Changed event of the radio buttons for setting the parameters
        Me.rbCatchEstBio.Tag = eAssessmentMethods.CatchEstmBio
        Me.rbDirectExp.Tag = eAssessmentMethods.DirectExploitation
        Me.rbExact.Tag = eAssessmentMethods.Exact

        Me.rbEffortNoCap.Tag = eMSEEffortSource.NoCap
        Me.rbEffortEcosim.Tag = eMSEEffortSource.EcosimEffort
        Me.rbEffortPredicted.Tag = eMSEEffortSource.Predicted

        Me.rbNoRegs.Tag = eMSERegulationMode.NoRegulations
        Me.rbUseRegs.Tag = eMSERegulationMode.UseRegulations

        Me.m_dctEffortControls = New Dictionary(Of eMSERegulationMode, RadioButton)
        Me.m_dctEffortControls.Add(eMSERegulationMode.NoRegulations, Me.rbNoRegs)
        Me.m_dctEffortControls.Add(eMSERegulationMode.UseRegulations, Me.rbUseRegs)

        Me.UpdateSelectedEffortMode()
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        Me.m_dctEffortControls.Clear()
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub rbFTracking_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles rbNoRegs.CheckedChanged, rbUseRegs.CheckedChanged

        If Me.m_MSE Is Nothing Then Exit Sub

        Try
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            If rb.Checked = True Then
                Dim EffortMode As eMSERegulationMode = DirectCast(rb.Tag, eMSERegulationMode)
                Me.m_MSE.ModelParameters.RegulatoryMode = EffortMode
            End If

        Catch ex As Exception
            Debug.Assert(False, "Exception setting MSE Effort Mode. " & ex.Message)
        End Try

        Me.UpdateControls()
        Me.Refresh()

    End Sub


    ''' <summary>
    ''' Change the biomass assessment method based on the selected radio button
    ''' </summary>
    Private Sub onAssessmentMethodChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles rbCatchEstBio.CheckedChanged, rbDirectExp.CheckedChanged, rbExact.CheckedChanged
        Try

            If Me.m_MSE Is Nothing Then Exit Sub

            Debug.Assert(TypeOf sender Is RadioButton)
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            'This event handler is call for both radio buttons Changed events Checked and UnChecked
            'Use the tag of the Checked radio button to set the MSE.AssessmentMethod
            If rb.Checked = True Then
                Me.m_MSE.ModelParameters.AssessmentMethod = DirectCast(rb.Tag, eAssessmentMethods)

            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub UpdateSelectedEffortMode()
        Try
            m_dctEffortControls.Item(Me.m_MSE.ModelParameters.RegulatoryMode).Checked = True
        Catch ex As Exception

        End Try
    End Sub


    Private Sub UpdateControls()

        Me.pnlUseReg.Enabled = False
        Me.pnlFTracking.Enabled = False

        Select Case Me.m_MSE.ModelParameters.RegulatoryMode

            Case eMSERegulationMode.UseRegulations
                Me.pnlUseReg.Enabled = True

            Case eMSERegulationMode.NoRegulations
                Me.pnlFTracking.Enabled = True


        End Select

    End Sub


    Private Sub rbNoCap_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles rbEffortNoCap.CheckedChanged, rbEffortEcosim.CheckedChanged, rbEffortPredicted.CheckedChanged

        Try

            If Me.m_MSE Is Nothing Then Exit Sub

            Debug.Assert(TypeOf sender Is RadioButton)
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            'This event handler is call when the radio button is Checked or UnChecked
            'Use the tag of the Checked radio button to set the MSE.EffortSource
            If rb.Checked = True Then
                Me.m_MSE.ModelParameters.EffortSource = DirectCast(rb.Tag, eMSEEffortSource)
            End If

        Catch ex As Exception

        End Try

    End Sub

End Class