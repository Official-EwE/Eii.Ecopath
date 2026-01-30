' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class frmMSEOptions

#Region " Private vars "

    'ToDo_jb 19-April-2010 Change "Effort and regulatory option" to something Effort and evaluation type control type....
    Dim m_MSE As cMSEManager

    Private m_fpNTrials As cPropertyFormatProvider
    Private m_fpSave As cPropertyFormatProvider

    Private m_fpForecast As cPropertyFormatProvider
    Private m_fpSBPower As cPropertyFormatProvider

    Private m_fpMaxEffort As cPropertyFormatProvider
    Private m_fpUseQuotaRegs As cPropertyFormatProvider

    Private m_RegMode As eMSERegulationMode
    Private m_ControlType As eControlTypes

    Private Enum eControlTypes
        InputEffort
        OutputQuota
    End Enum

    Private Enum eRegOptions As Integer
        Effort
        Quota
        NoReg
    End Enum
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmMSEOptions)()

#End Region ' Private vars

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

#Region " Overrides "

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
            Me.m_gridEffortControls.UIContext = Me.UIContext
            Me.m_gridQuotaControls.UIContext = Me.UIContext
        End Set
    End Property

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_MSE = Me.UIContext.Core.MSEManager

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

        '  Me.m_fpUsePlugin = New cPropertyFormatProvider(Me.UIContext, Me.m_ckPlugin, Me.m_MSE.ModelParameters, eVarNameFlags.MSEUseEconomicPlugin)

        'Me.m_fpForecast = New cPropertyFormatProvider(Me.UIContext, Me.txForecast, Me.m_MSE.ModelParameters, eVarNameFlags.MSEForcastGain)
        Me.m_fpSBPower = New cPropertyFormatProvider(Me.UIContext, Me.m_txSBPower, Me.m_MSE.ModelParameters, eVarNameFlags.MSEAssessPower)
        Me.m_fpMaxEffort = New cPropertyFormatProvider(Me.UIContext, Me.m_txMaxEffort, Me.m_MSE.ModelParameters, eVarNameFlags.MSEMaxEffort)


        'Assessment methods Catch Estimated Biomass and Direct Exploitation are stored in the tag property of the radio buttons
        'see the Changed event of the radio buttons for setting the parameters
        Me.m_rbCatchEstBio.Tag = eAssessmentMethods.CatchEstmBio
        Me.m_rbDirectExp.Tag = eAssessmentMethods.DirectExploitation
        Me.m_rbExact.Tag = eAssessmentMethods.Exact

        Me.m_rbEffortNoCap.Tag = eMSEEffortSource.NoCap
        Me.m_rbEffortEcosim.Tag = eMSEEffortSource.EcosimEffort
        Me.m_rbEffortPredicted.Tag = eMSEEffortSource.Predicted

        Me.m_rbNoRegs.Tag = eMSERegulationMode.NoRegulations
        Me.m_rbUseRegs.Tag = eMSERegulationMode.UseRegulations

        Me.m_rbEffortControls.Tag = eControlTypes.InputEffort
        Me.m_rbQuotaControls.Tag = eControlTypes.OutputQuota

        Me.m_RegMode = eMSERegulationMode.UseRegulations
        Me.m_ControlType = eControlTypes.OutputQuota

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Me.m_fpSBPower.Release()
        MyBase.OnFormClosed(e)
    End Sub

    Private m_bInUpdate As Boolean = False

    Protected Overrides Sub UpdateControls()

        If Me.m_bInUpdate Then Return
        Me.m_bInUpdate = True

        ' 0 = Effort, 1 = Quota, 2 = NoReg
        Dim [option] As eRegOptions = eRegOptions.Effort

        Me.m_MSE.ModelParameters.RegulatoryMode = Me.m_RegMode

        ' This is some awful logic...
        ' In EwE, controls are not hidden, only disabled / enabled. Fixed this UI up properly
        ' Also, control anchoring is not working reliably anymore. Panels have been reorganized which is unfortunate.

        Select Case Me.m_RegMode
            Case eMSERegulationMode.UseRegulations
                Me.m_rbUseRegs.Checked = True
                Select Case Me.m_ControlType
                    Case eControlTypes.InputEffort
                        Me.m_rbEffortControls.Checked = True
                        [option] = eRegOptions.Effort
                    Case eControlTypes.OutputQuota
                        Me.m_rbQuotaControls.Checked = True
                        [option] = eRegOptions.Quota
                End Select
            Case eMSERegulationMode.NoRegulations
                Me.m_rbNoRegs.Checked = True
                [option] = eRegOptions.NoReg
        End Select

        Me.m_MSE.ModelParameters.UseLPSolution = ([option] = eRegOptions.Effort)
        Me.m_gridEffortControls.Enabled = ([option] = eRegOptions.Effort)
        Me.m_panelQuotaControls.Enabled = ([option] = eRegOptions.Quota)
        Me.m_gridQuotaControls.Enabled = ([option] = eRegOptions.Quota)
        Me.m_panelNoReg.Enabled = ([option] = eRegOptions.NoReg)

        ' Do not disable master controls
        'Me.m_panelRegControls.Enabled = ([option] <> eRegOptions.NoReg)

        Me.m_bInUpdate = False

    End Sub

#End Region ' Overrides

#Region " Event handlers "

    Private Sub rbFTracking_CheckedChanged(sender As System.Object, e As System.EventArgs)

        If (Me.UIContext Is Nothing) Then Return

        Try
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            If rb.Checked = True Then
                Dim EffortMode As eMSERegulationMode = DirectCast(rb.Tag, eMSERegulationMode)
                Me.m_MSE.ModelParameters.RegulatoryMode = EffortMode
                Me.UpdateControls()
            End If

        Catch ex As Exception
            Debug.Assert(False, "Exception setting MSE Effort Mode. " & ex.Message)
        End Try


    End Sub

    Private Sub rbNoCap_CheckedChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbEffortNoCap.CheckedChanged, m_rbEffortEcosim.CheckedChanged, m_rbEffortPredicted.CheckedChanged

        If (Me.UIContext Is Nothing) Then Return

        Try
            Debug.Assert(TypeOf sender Is RadioButton)
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            'This event handler is call when the radio button is Checked or UnChecked
            'Use the tag of the Checked radio button to set the MSE.EffortSource
            If rb.Checked = True Then
                Me.m_MSE.ModelParameters.EffortSource = DirectCast(rb.Tag, eMSEEffortSource)
                Me.UpdateControls()
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnControlTypeCheckChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbEffortControls.CheckedChanged, m_rbQuotaControls.CheckedChanged

        If (Me.UIContext Is Nothing) Then Return

        Try
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            Debug.Assert(TypeOf sender Is RadioButton)
            Debug.Assert(rb.Tag IsNot Nothing)
            Debug.Assert(TypeOf rb.Tag Is eControlTypes)

            If (rb.Checked) Then
                Me.m_RegMode = eMSERegulationMode.UseRegulations
                Me.m_ControlType = DirectCast(rb.Tag, eControlTypes)
                Me.UpdateControls()
            End If
        Catch ex As Exception
            m_logger.LogError(ex, "frmMSEOptions::OnControlTypeCheckChanged")
        End Try

    End Sub

    Private Sub OnRegControlsCheckChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbUseRegs.CheckedChanged, m_rbNoRegs.CheckedChanged

        If (Me.UIContext Is Nothing) Then Return

        Try

            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            Debug.Assert(TypeOf sender Is RadioButton)
            Debug.Assert(rb.Tag IsNot Nothing)
            Debug.Assert(TypeOf rb.Tag Is eMSERegulationMode)

            If rb.Checked = True Then
                Me.m_RegMode = DirectCast(rb.Tag, eMSERegulationMode)
                Me.m_MSE.ModelParameters.RegulatoryMode = Me.m_RegMode
                Me.UpdateControls()
            End If

        Catch ex As Exception

        End Try

    End Sub

#End Region ' Event handlers

End Class