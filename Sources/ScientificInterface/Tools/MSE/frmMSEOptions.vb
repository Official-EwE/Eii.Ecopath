' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
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


Public Class frmMSEOptions

    'ToDo_jb 19-April-2010 Change "Effort and regulatory option" to something Effort and evaluation type control type....
    Dim m_MSE As cMSEManager

    Private m_fpNTrials As cPropertyFormatProvider
    'Private m_fpUsePlugin As cPropertyFormatProvider
    Private m_fpSave As cPropertyFormatProvider

    '  Private m_fpKalman As cPropertyFormatProvider
    Private m_fpForecast As cPropertyFormatProvider
    Private m_fpSBPower As cPropertyFormatProvider

    Private m_fpMaxEffort As cPropertyFormatProvider


    Private m_fpUseQuotaRegs As cPropertyFormatProvider
    Private m_dctEffortControls As Dictionary(Of eMSERegulationMode, RadioButton)


    Private m_RegMode As eMSERegulationMode
    Private m_ControlType As eControlTypes

    Private Enum eControlTypes
        InputEffort
        OutputQuota
    End Enum


    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
            Me.m_gridFleetLPEffortBounds.UIContext = Me.UIContext
            Me.m_gridRegOptions.UIContext = Me.UIContext
        End Set
    End Property

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_MSE = Me.UIContext.Core.MSEManager

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

        '  Me.m_fpUsePlugin = New cPropertyFormatProvider(Me.UIContext, Me.m_ckPlugin, Me.m_MSE.ModelParameters, eVarNameFlags.MSEUseEconomicPlugin)

        'Me.m_fpForecast = New cPropertyFormatProvider(Me.UIContext, Me.txForecast, Me.m_MSE.ModelParameters, eVarNameFlags.MSEForcastGain)
        Me.m_fpSBPower = New cPropertyFormatProvider(Me.UIContext, Me.txSBPower, Me.m_MSE.ModelParameters, eVarNameFlags.MSEAssessPower)
        Me.m_fpMaxEffort = New cPropertyFormatProvider(Me.UIContext, Me.txMaxEffort, Me.m_MSE.ModelParameters, eVarNameFlags.MSEMaxEffort)


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

        Me.m_rbEffortControls.Tag = eControlTypes.InputEffort
        Me.m_rbQuotaControls.Tag = eControlTypes.OutputQuota

        Me.m_dctEffortControls = New Dictionary(Of eMSERegulationMode, RadioButton)


        m_RegMode = eMSERegulationMode.UseRegulations
        m_ControlType = eControlTypes.InputEffort

        ' Me.UpdateSelectedEffortMode()
        Me.UpdateControls()

        '  Me.updateControlTypes(eControlTypes.InputEffort)

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        '   Me.m_dctEffortControls.Clear()

        ' Me.m_fpUsePlugin.Release()
        Me.m_fpSBPower.Release()

        MyBase.OnFormClosed(e)
    End Sub

    Private Sub rbFTracking_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)


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
    Private Sub onAssessmentMethodChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

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


    'Private Sub UpdateSelectedEffortMode()
    '    Try
    '        ' m_dctEffortControls.Item(Me.m_MSE.ModelParameters.RegulatoryMode).Checked = True
    '    Catch ex As Exception

    '    End Try
    'End Sub


    Protected Overrides Sub UpdateControls()

        ' Me.pnlUseReg.Enabled = False
        Me.pnlFTracking.Enabled = False

        Me.m_MSE.ModelParameters.RegulatoryMode = Me.m_RegMode

        Me.m_panelEffortControls.Visible = False
        Me.m_panelNoReg.Visible = False
        Me.m_panelQuotaControls.Visible = False

        Select Case Me.m_RegMode

            Case eMSERegulationMode.UseRegulations
                'Fishery is regulated

                Select Case Me.m_ControlType


                    Case eControlTypes.InputEffort
                        Me.m_MSE.ModelParameters.UseLPSolution = True

                        Me.m_panelEffortControls.Visible = True

                    Case eControlTypes.OutputQuota
                        Me.m_MSE.ModelParameters.UseLPSolution = False
                        Me.m_panelQuotaControls.Visible = True


                End Select


            Case eMSERegulationMode.NoRegulations
                'No Regulations
                Me.pnlFTracking.Enabled = True

                Me.m_panelNoReg.Visible = True

        End Select

    End Sub


    Private Sub rbNoCap_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)


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

    Private Sub pnlUseReg_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs)

    End Sub

    Private Sub onControlTypeChanged_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles m_rbEffortControls.CheckedChanged, m_rbQuotaControls.CheckedChanged

        Try

            If Me.UIContext Is Nothing Then Return

            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            Debug.Assert(TypeOf sender Is RadioButton)
            Debug.Assert(rb.Tag IsNot Nothing)
            Debug.Assert(TypeOf rb.Tag Is eControlTypes)

            Me.m_ControlType = DirectCast(rb.Tag, eControlTypes)

            Me.UpdateControls()


            '  Me.updateControlTypes(DirectCast(rb.Tag, eControlTypes))

        Catch ex As Exception

        End Try

    End Sub


    Private Sub updateControlTypes(ByVal newControlType As eControlTypes)

        If newControlType = eControlTypes.InputEffort Then

            ' Me.m_SplitControls.Panel1.Enabled = True
            ' Me.m_SplitControls.Panel2.Enabled = False

            Me.m_MSE.ModelParameters.UseLPSolution = True



        ElseIf newControlType = eControlTypes.OutputQuota Then

            '  Me.m_SplitControls.Panel1.Enabled = False
            '  Me.m_SplitControls.Panel2.Enabled = True


            Me.m_MSE.ModelParameters.UseLPSolution = False

        End If

    End Sub
   
    Private Sub onChangeRegControls_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbUseRegs.CheckedChanged, rbNoRegs.CheckedChanged

        If Me.UIContext Is Nothing Then Return

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        Debug.Assert(TypeOf sender Is RadioButton)
        Debug.Assert(rb.Tag IsNot Nothing)
        Debug.Assert(TypeOf rb.Tag Is eMSERegulationMode)

        If rb.Checked = True Then
            Me.m_RegMode = DirectCast(rb.Tag, eMSERegulationMode)
            Me.m_MSE.ModelParameters.RegulatoryMode = m_RegMode
        End If

        Me.UpdateControls()

    End Sub

End Class