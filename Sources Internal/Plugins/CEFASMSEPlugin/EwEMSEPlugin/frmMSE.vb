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
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmMSE

#Region " Private vars "

    Private m_plugin As cMSE = Nothing

    Private m_fpArea As cEwEFormatProvider = Nothing
    Private m_fpNModelsToRun As cEwEFormatProvider = Nothing
    Private m_fpNTrials As cEwEFormatProvider = Nothing
    Private m_fpNYearsToProject As cEwEFormatProvider = Nothing
    Private m_fpMassBalanceTol As cEwEFormatProvider = Nothing
    Private m_fpMaxAttempts As cEwEFormatProvider = Nothing
    Private m_fpMaxTime As cEwEFormatProvider = Nothing

    Private m_bInUpdate As Boolean = False

#End Region ' Private vars

    Public Sub New(MSE As cMSE, uic As cUIContext)

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_plugin = MSE

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
        'Me.m_fpArea.Style = cStyleGuide.eStyleFlags.NotEditable

        Me.m_fpNModelsToRun = New cEwEFormatProvider(Me.UIContext, Me.m_tbxNModels2Run, GetType(Integer))
        Me.m_fpNModelsToRun.Value = Me.m_plugin.NModels2Run
        AddHandler Me.m_fpNModelsToRun.OnValueChanged, AddressOf OnNModels2RunChanged

        Me.m_fpNTrials = New cEwEFormatProvider(Me.UIContext, Me.m_tbxNTrials, GetType(Integer), New cVariableMetaData(0, Me.m_plugin.NumModelsAvailable, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpNTrials.Value = Me.m_plugin.NModels
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

        Me.m_fpMaxTime = New cEwEFormatProvider(Me.UIContext, Me.m_tbxMaxTime, GetType(Single), New cVariableMetaData(0.08, 48, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        AddHandler Me.m_fpMaxTime.OnValueChanged, AddressOf OnMaxTimeChanged

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

            RemoveHandler Me.m_fpMaxTime.OnValueChanged, AddressOf OnMaxTimeChanged
            Me.m_fpMaxTime.Release()

        End If

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If (Me.m_plugin Is Nothing) Then Return
        If (Me.IsDisposed) Then Return

        Dim mon As cMSEStateMonitor = Me.m_plugin.Controller
        Dim img As Image = Nothing

        Me.m_plStep1.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.Idle)
        Me.m_plStep2.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_plStep3.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_plStep4.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasModels)

        Me.m_lblPathValue.Text = cStringUtils.CompactString(Me.m_plugin.DataPath, Me.m_lblPathValue.ClientRectangle.Width, Me.m_lblPathValue.Font, TextFormatFlags.PathEllipsis)
        cToolTipShared.GetInstance().SetToolTip(Me.m_lblPathValue, Me.m_plugin.DataPath)

        If mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams) Then
            Me.m_tbxParamStatus.Text = My.Resources.STATUS_AVAILABLE
            If Not mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams) Then
                img = SharedResources.Critical
            End If
        Else
            Me.m_tbxParamStatus.Text = My.Resources.STATUS_NOTAVAILABLE
        End If
        Me.m_pbCompatible.Image = img

        ' Update trial buttons
        Me.m_fpNTrials.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_fpMassBalanceTol.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_btnDecreaseEffort.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_btnCreateModels.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)

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

        If (Me.m_plugin Is Nothing) Then Return

        Try
            Me.m_plugin.GenerateEcosimParameters("MaxRelFeedingTime")
            Me.m_plugin.GenerateEcosimParameters("FeedingTimeAdjustRate")
            Me.m_plugin.GenerateEcosimParameters("OtherMortFeedingTime")
            Me.m_plugin.GenerateEcosimParameters("PredEffectFeedingTime")
            Me.m_plugin.GenerateEcosimParameters("DenDepCatchability")
            Me.m_plugin.GenerateEcosimParameters("QBMaxxQBio")
            Me.m_plugin.GenerateEcosimParameters("SwitchingPower")
            Me.m_plugin.CreateVulnerabilities()
            Me.m_plugin.GenerateEcopathParamaters()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnPathClicked(sender As System.Object, e As System.EventArgs) _
        Handles m_lblPathValue.Click

        If (Me.m_plugin Is Nothing) Then Return

        Try
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.Invoke(Me.m_plugin.DataPath)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE.frmMSE::OnPathClicked(" & Me.m_plugin.DataPath & ")")
        End Try

    End Sub

    Private Sub OnPathPrefChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbEwEDefault.CheckedChanged, m_rbCustomPath.CheckedChanged

        If Me.m_bInUpdate Then Return

        Try
            Me.m_plugin.UseEwEPath = Me.m_rbEwEDefault.Checked
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnRun(ByVal sender As System.Object, ByVal e As System.EventArgs) _
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

        Catch ex As Exception
            cLog.Write(ex, "CEFASMSE:OnSelectDataPath")
        End Try

    End Sub

    Private Sub OnShowTFM(sender As System.Object, e As System.EventArgs) _
        Handles m_btnReviewTFM.Click

        'First make sure the Harvest Controls Rules have been loaded
        'this is so the interface has some data

        ' JS 02Oct13: Moved Strategies extraction test flag to the plug-in, which does the actual work
        '             From the UI point of view, we just want strategies. The plug-in does the optimizating
        m_plugin.ExtractHCR()

        Dim frm As New frmTFMpolicy()
        frm.Init(Me.UIContext, Me.m_plugin)
        frm.ShowDialog(Me)

    End Sub

    Private Sub OnReviewDistParams(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnReviewDistParms.Click

        Try
            Dim frmDisParams As New frmDistributionParameters()
            frmDisParams.Init(Me.UIContext, Me.m_plugin)
            frmDisParams.ShowDialog(Me)
        Catch ex As Exception

        End Try

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
            Me.m_plugin.NModels = CInt(Me.m_fpNTrials.Value)
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

    Private Sub OnMaxTimeChanged(sender As Object, args As EventArgs)
        Try
            Me.m_plugin.NMaxTime = CSng(Me.m_fpMaxTime.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnMaxAttemptsChanged")
        End Try
    End Sub

    Private Sub OnDecreaseEffort(sender As Object, e As System.EventArgs) Handles m_btnDecreaseEffort.Click

        Try
            Dim frmMaxDecreaseEfforts As New frmEditDecreaseEffort()
            frmMaxDecreaseEfforts.Init(Me.UIContext, Me.m_plugin)
            frmMaxDecreaseEfforts.ShowDialog(Me)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Control events

#Region " Plug-in callback "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Handle a (remote) request to update the form state. The request is handled
    ''' in idle time.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub UpdateState()
        Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
    End Sub

#End Region ' Plug-in callback

End Class
