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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
Option Strict On
Imports System.IO
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class frmMSE

    Private mCore As cCore
    Private m_plugin As cMSE
    Private frmTargetF As frmTFMpolicy

    Private m_fpArea As cEwEFormatProvider = Nothing
    Private m_fpNModelsToRun As cEwEFormatProvider = Nothing
    Private m_fpNTrials As cEwEFormatProvider = Nothing
    Private m_fpNYearsToProject As cEwEFormatProvider = Nothing
    Private m_fpMassBalanceTol As cEwEFormatProvider = Nothing
    Private m_fpMaxAttempts As cEwEFormatProvider = Nothing

    Private m_bInUpdate As Boolean = False

    Public Sub New(MSE As cMSE, uic As cUIContext)

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_plugin = MSE
        Me.mCore = uic.Core

    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_bInUpdate = True

        ' -- Set up control interactions --

        ' Connect area UI control to live Ecopath data
        Me.m_fpArea = New cPropertyFormatProvider(Me.UIContext, m_tbxArea, Me.Core.EwEModel, eVarNameFlags.Area)
        ' Area can be made editable from here by not setting the format provider style:
        'Me.m_fpArea.Style = ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags.NotEditable

        Me.m_fpNModelsToRun = New cEwEFormatProvider(Me.UIContext, Me.m_tbxNModels2Run, GetType(Integer))
        Me.m_fpNModelsToRun.Value = Me.m_plugin.NModels2Run
        AddHandler Me.m_fpNModelsToRun.OnValueChanged, AddressOf OnNModels2RunChanged

        Me.m_fpNTrials = New cEwEFormatProvider(Me.UIContext, Me.m_tbxNTrials, GetType(Integer), New cVariableMetaData(0, Me.m_plugin.NumModelsAvailable, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpNTrials.Value = Me.m_plugin.NTrials
        AddHandler Me.m_fpNTrials.OnValueChanged, AddressOf OnNTrialsChanged

        Me.m_fpNYearsToProject = New cEwEFormatProvider(Me.UIContext, m_tbxNYearsProject, GetType(Integer))
        Me.m_fpNYearsToProject.Value = Me.m_plugin.NYearsProject
        AddHandler Me.m_fpNYearsToProject.OnValueChanged, AddressOf OnNYearsToProjectChanged

        Me.m_fpMassBalanceTol = New cEwEFormatProvider(Me.UIContext, Me.m_tbxTolerance, GetType(Single), New cVariableMetaData(0, 0.1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpMassBalanceTol.Value = Me.m_plugin.MassBalanceTol
        AddHandler Me.m_fpMassBalanceTol.OnValueChanged, AddressOf OnMassBalanceTolChanged

        Me.m_fpMaxAttempts = New cEwEFormatProvider(Me.UIContext, Me.m_tbxMaxAttempts, GetType(Integer), New cVariableMetaData(1, 1000000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpMaxAttempts.Value = Me.m_plugin.NMaxAttempts
        AddHandler Me.m_fpMaxAttempts.OnValueChanged, AddressOf OnMaxAttemptsChanged

        Me.m_rbEwEDefault.Checked = Me.m_plugin.UseEwEPath
        Me.m_rbCustomPath.Checked = Not Me.m_plugin.UseEwEPath

        Me.m_hdrStep2.IsCollapsed = True

        Me.m_bInUpdate = False

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.UIContext IsNot Nothing) Then

            Me.m_fpArea.Release()

            RemoveHandler Me.m_fpNModelsToRun.OnValueChanged, AddressOf OnNModels2RunChanged
            Me.m_fpNModelsToRun.Release()

            RemoveHandler Me.m_fpNTrials.OnValueChanged, AddressOf OnNTrialsChanged
            Me.m_fpNTrials.Release()

            RemoveHandler Me.m_fpNYearsToProject.OnValueChanged, AddressOf OnNYearsToProjectChanged
            Me.m_fpNYearsToProject.Release()

            RemoveHandler Me.m_fpMassBalanceTol.OnValueChanged, AddressOf OnMassBalanceTolChanged
            Me.m_fpMassBalanceTol.Release()

            RemoveHandler Me.m_fpMaxAttempts.OnValueChanged, AddressOf OnMaxAttemptsChanged
            Me.m_fpMaxAttempts.Release()

        End If

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Dim mon As cMSEStateMonitor = Me.m_plugin.Controller
        Dim img As Image = Nothing
        Dim bCanCreateModels As Boolean = Me.m_plugin.IsInputDataCompatible

        Me.m_plStep1.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.Idle)
        Me.m_plStep2.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_plStep3.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_plStep4.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_plStep5.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasModels)

        Me.m_tbxPath.Text = Me.m_plugin.DataPath

        If bCanCreateModels Then
            img = SharedResources.OK
        Else
            img = SharedResources.Critical
        End If
        Me.m_pbCompatible.Image = img

        ' Update trial buttons
        Me.m_fpNTrials.Enabled = bCanCreateModels
        Me.m_fpMassBalanceTol.Enabled = bCanCreateModels
        Me.m_btnCreateModels.Enabled = bCanCreateModels

        ' Provide feedback about available models
        If mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams) Then
            If String.IsNullOrWhiteSpace(Me.m_plugin.ModelCompatibilityInfo) Then
                Me.m_tbxNumAvailableModels.Text = CStr(Me.m_plugin.NumModelsAvailable)
            Else
                Me.m_tbxNumAvailableModels.Text = String.Format(SharedResources.GENERIC_LABEL_DETAILED, _
                                                                Me.m_plugin.NumModelsAvailable, _
                                                                Me.m_plugin.ModelCompatibilityInfo)
            End If
            Me.m_tbxNumAvailableFishingStrategies.Text = CStr(Me.m_plugin.NumStrategiesAvailable)
        Else
            Me.m_tbxNumAvailableModels.Text = ""
            Me.m_tbxNumAvailableFishingStrategies.Text = ""
        End If

    End Sub

#End Region ' Form overrides

#Region " Control events "

    Private Sub OnCreateModels(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnCreateModels.Click

        m_plugin.Create1DimParams("MaxRelFeedingTime")
        m_plugin.Create1DimParams("FeedingTimeAdjustRate")
        m_plugin.Create1DimParams("OtherMortFeedingTime")
        m_plugin.Create1DimParams("PredEffectFeedingTime")
        m_plugin.Create1DimParams("DenDepCatchability")
        m_plugin.Create1DimParams("QBMaxxQBio")
        m_plugin.Create1DimParams("SwitchingPower")
        m_plugin.CreateVulnerabilities()
        m_plugin.GenerateEcopathParamaters()

    End Sub

    Private Sub OnPathPrefChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbEwEDefault.CheckedChanged, m_rbCustomPath.CheckedChanged

        If Me.m_bInUpdate Then Return

        Try
            Me.m_plugin.UseEwEPath = Me.m_rbEwEDefault.Checked
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub btnLoadSampled_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnRun.Click
        Try
            Me.m_plugin.LoadSampledParams()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnSelectDataPath(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnChangePath.Click

        ' JS 30Sep13: Use EwE dialog framework here
        Try
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler

            If Me.m_plugin.UseEwEPath Then
                Dim cmd As cShowOptionsCommand = DirectCast(cmdh.GetCommand(cShowOptionsCommand.cCOMMAND_NAME), cShowOptionsCommand)
                cmd.Invoke(ScientificInterfaceShared.Definitions.eApplicationOptionTypes.FileLocations)
                ' Do not set path; let core deal with it
            Else
                Dim cmd As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
                cmd.Invoke(Me.m_plugin.CustomPath, My.Resources.PROMPT_DATAPATH)
                If (cmd.Result = Windows.Forms.DialogResult.OK) Then
                    Me.m_plugin.CustomPath = cmd.Directory
                End If
            End If

            Me.UpdateControls()

        Catch ex As Exception
            cLog.Write(ex, "CEFASMSE:OnSelectDataPath")
        End Try

    End Sub

    Private Sub btShowTFMForm_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnShowTFMForm.Click

        Dim bhasForm As Boolean

        'First make sure the Harvest Controls Rules have been loaded
        'this is so the interface has some data

        ' JS 02Oct13: Moved Strategies extraction test flag to the plug-in, which does the actual work
        '             From the UI point of view, we just want strategies. The plug-in does the optimizating
        m_plugin.ExtractHCR()

        'Ok now the interface
        If Me.frmTargetF IsNot Nothing Then
            bhasForm = Not frmTargetF.IsDisposed
        End If
        If Not bhasForm Then
            frmTargetF = New frmTFMpolicy()
            frmTargetF.Init(Me.UIContext, Me.m_plugin)
        End If

        frmTargetF.Show()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btn2.Click

        Try
            m_plugin.Create1DimParams("MaxRelFeedingTime")
            m_plugin.Create1DimParams("FeedingTimeAdjustRate")
            m_plugin.Create1DimParams("OtherMortFeedingTime")
            m_plugin.Create1DimParams("PredEffectFeedingTime")
            m_plugin.Create1DimParams("DenDepCatchability")
            m_plugin.Create1DimParams("QBMaxxQBio")
            m_plugin.Create1DimParams("SwitchingPower")
            'mMSE.Create2DimParams("DietComposition")
            m_plugin.CreateVulnerabilities()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnDistParams_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnDistParams.Click

        Try
            Dim frmDisParams As New frmDistributionParameters()
            frmDisParams.Init(Me.UIContext, Me.m_plugin)
            frmDisParams.ShowDialog(Me)
        Catch ex As Exception

        End Try

        ' Perhaps the user has made a useful contribution ;)
        Me.UpdateControls()

    End Sub

    Private Sub OnNModels2RunChanged(sender As Object, args As EventArgs)
        Try
            Me.m_plugin.NModels2Run = CInt(Me.m_fpNModelsToRun.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNModels2RunChanged")
        End Try
    End Sub

    Private Sub OnNTrialsChanged(sender As Object, args As EventArgs)
        Try
            Me.m_plugin.NTrials = CInt(Me.m_fpNTrials.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNTrialsChanged")
        End Try
    End Sub

    Private Sub OnNYearsToProjectChanged(sender As Object, args As EventArgs)
        Try
            Me.m_plugin.NYearsProject = CInt(Me.m_fpNYearsToProject.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNYearsToProjectChanged")
        End Try
    End Sub

    Private Sub OnMassBalanceTolChanged(sender As Object, args As EventArgs)
        Try
            Me.m_plugin.MassBalanceTol = CSng(Me.m_fpMassBalanceTol.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnMassBalanceTolChanged")
        End Try
    End Sub

    Private Sub OnMaxAttemptsChanged(sender As Object, args As EventArgs)
        Try
            Me.m_plugin.NMaxAttempts = CInt(Me.m_fpMaxAttempts.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnMaxAttemptsChanged")
        End Try
    End Sub

#End Region ' Control events


End Class
