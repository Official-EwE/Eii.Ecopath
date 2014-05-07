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

    Private m_plugin As cMSEPluginPoint = Nothing
    Private m_survivability As cSurvivability = Nothing

    Private m_fpArea As cEwEFormatProvider = Nothing
    Private m_fpNModelsToRun As cEwEFormatProvider = Nothing
    Private m_fpNTrials As cEwEFormatProvider = Nothing
    Private m_fpNYearsToProject As cEwEFormatProvider = Nothing
    Private m_fpMassBalanceTol As cEwEFormatProvider = Nothing
    Private m_fpMaxAttempts As cEwEFormatProvider = Nothing
    Private m_fpMaxTime As cEwEFormatProvider = Nothing

    Private m_bInUpdate As Boolean = False

#End Region ' Private vars

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="MSEPluginPoint">The <see cref="cMSEPluginPoint"/> this form 
    ''' is created for.</param>
    ''' <param name="uic">The <see cref="cUIContext"/> of the current EwE instance.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(MSEPluginPoint As cMSEPluginPoint, uic As cUIContext)

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_plugin = MSEPluginPoint
        Me.m_survivability = Me.MSE.Survivability

    End Sub

#End Region ' Construction

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_plugin Is Nothing) Then Return

        Me.m_bInUpdate = True

        Me.TabText = Me.Text

        ' -- Set up control interactions --

        ' Connect area UI control to live Ecopath data
        Me.m_fpArea = New cPropertyFormatProvider(Me.UIContext, m_tbxArea, Me.Core.EwEModel, eVarNameFlags.Area)
        ' Area can be made editable from here by not setting the format provider style:
        'Me.m_fpArea.Style = cStyleGuide.eStyleFlags.NotEditable

        Me.m_fpNModelsToRun = New cEwEFormatProvider(Me.UIContext, Me.m_tbxNModels2Run, GetType(Integer))
        Me.m_fpNModelsToRun.Value = Me.MSE.NModels2Run
        AddHandler Me.m_fpNModelsToRun.OnValueChanged, AddressOf OnNModels2RunChanged

        Me.m_fpNTrials = New cEwEFormatProvider(Me.UIContext, Me.m_tbxNTrials, GetType(Integer), New cVariableMetaData(0, Me.MSE.NumModelsAvailable, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpNTrials.Value = Me.MSE.NModels
        AddHandler Me.m_fpNTrials.OnValueChanged, AddressOf OnNTrialsChanged

        Me.m_fpNYearsToProject = New cEwEFormatProvider(Me.UIContext, m_tbxNYearsProject, GetType(Integer))
        Me.m_fpNYearsToProject.Value = Me.MSE.NYearsProject
        AddHandler Me.m_fpNYearsToProject.OnValueChanged, AddressOf OnNYearsToProjectChanged

        Me.m_fpMassBalanceTol = New cEwEFormatProvider(Me.UIContext, Me.m_tbxTolerance, GetType(Single), New cVariableMetaData(0, 0.1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpMassBalanceTol.Value = Me.MSE.MassBalanceTol
        AddHandler Me.m_fpMassBalanceTol.OnValueChanged, AddressOf OnMassBalanceTolChanged

        Me.m_fpMaxAttempts = New cEwEFormatProvider(Me.UIContext, Me.m_tbxMaxAttempts, GetType(Integer), New cVariableMetaData(1, 1000000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpMaxAttempts.Value = Me.MSE.NMaxAttempts
        AddHandler Me.m_fpMaxAttempts.OnValueChanged, AddressOf OnMaxAttemptsChanged

        Me.m_fpMaxTime = New cEwEFormatProvider(Me.UIContext, Me.m_tbxMaxTime, GetType(Single), New cVariableMetaData(0.08, 48, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        AddHandler Me.m_fpMaxTime.OnValueChanged, AddressOf OnMaxTimeChanged

        Me.m_rbEwEDefault.Checked = Me.MSE.UseEwEPath
        Me.m_rbCustomPath.Checked = Not Me.MSE.UseEwEPath

        Me.m_hdrStep2.IsCollapsed = True

        Me.m_bInUpdate = False

        Dim mon As cMSEStateMonitor = Me.m_plugin.Monitor
        AddHandler mon.OnInvalidated, AddressOf OnMSEStateChanged

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

            Dim mon As cMSEStateMonitor = Me.m_plugin.Monitor
            RemoveHandler mon.OnInvalidated, AddressOf OnMSEStateChanged

        End If

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If (Me.m_plugin Is Nothing) Then Return
        If (Me.IsDisposed) Then Return

        Dim mon As cMSEStateMonitor = Me.m_plugin.Monitor
        Dim img As Image = Nothing

        Me.m_plStep1.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.Idle)
        Me.m_plStep2.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_plStep3.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        Me.m_plStep4.Enabled = mon.IsStateAvailable(cMSEStateMonitor.eState.HasModels)

        Me.m_lblPathValue.Text = cStringUtils.CompactString(Me.MSE.DataPath, Me.m_lblPathValue.ClientRectangle.Width, Me.m_lblPathValue.Font, TextFormatFlags.PathEllipsis)
        cToolTipShared.GetInstance().SetToolTip(Me.m_lblPathValue, Me.MSE.DataPath)

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
            If String.IsNullOrWhiteSpace(Me.MSE.ModelCompatibilityInfo) Then
                Me.m_tbxNumAvailableModels.Text = CStr(Me.MSE.NumModelsAvailable)
            Else
                Me.m_tbxNumAvailableModels.Text = String.Format(SharedResources.GENERIC_LABEL_DETAILED, _
                                                                Me.MSE.NumModelsAvailable, _
                                                                Me.MSE.ModelCompatibilityInfo)
            End If
            Me.m_tbxNumAvailableFishingStrategies.Text = CStr(Me.MSE.NumStrategiesAvailable)
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
            If Not Me.MSE.CreateModels() Then
                'Failed to create the new models
                'better tell the user 
            End If
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE.frmMSE::OnCreateModels")
        End Try

    End Sub

    Private Sub OnPathClicked(sender As System.Object, e As System.EventArgs) _
        Handles m_lblPathValue.Click

        If (Me.m_plugin Is Nothing) Then Return

        Try
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.Invoke(Me.MSE.DataPath)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE.frmMSE::OnPathClicked(" & Me.MSE.DataPath & ")")
        End Try

    End Sub

    Private Sub OnPathPrefChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbEwEDefault.CheckedChanged, m_rbCustomPath.CheckedChanged

        If (Me.m_plugin Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Try
            Me.MSE.UseEwEPath = Me.m_rbEwEDefault.Checked
            Me.ResolveMSEPathConflicts()
        Catch ex As Exception
            cLog.Write(ex, "CEFASMSE:OnPathPrefChanged")
        End Try

    End Sub

    Private Sub OnRun(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnRun.Click
        Try
            Me.MSE.LoadSampledParams()
        Catch ex As Exception
            cLog.Write(ex, "CEFASMSE:OnRun")
        End Try
    End Sub

    Private Sub OnSelectDataPath(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnChangePath.Click

        ' JS 30Sep13: Use EwE dialog framework here
        Try
            If (Me.SelectDataPath()) Then
                Me.ResolveMSEPathConflicts()
            End If
        Catch ex As Exception
            cLog.Write(ex, "CEFASMSE:OnSelectDataPath")
        End Try

    End Sub

    Private Sub OnEditSurvivabilities(sender As System.Object, e As System.EventArgs) _
        Handles btnEditSurvivabilities.Click

        Try
            Dim frmSurvivabilities As New frmEditSurvivabilities(MSE)
            frmSurvivabilities.Init(Me.UIContext)
            frmSurvivabilities.ShowDialog(Me)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnShowTFM")
        End Try

    End Sub

    Private Sub OnShowTFM(sender As System.Object, e As System.EventArgs) _
        Handles m_btnReviewTFM.Click

        Try
            Dim frm As New frmTFMpolicy()
            frm.Init(Me.UIContext, Me.MSE)
            frm.ShowDialog(Me)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnShowTFM")
        End Try

    End Sub

    Private Sub OnReviewDistParams(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnReviewDistParms.Click

        Try
            Me.ReviewDistParams()
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnReviewDistParams")
        End Try

    End Sub

    Private Sub OnNModels2RunChanged(sender As Object, args As EventArgs)
        Try
            Me.MSE.NModels2Run = CInt(Me.m_fpNModelsToRun.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNModels2RunChanged")
        End Try
    End Sub

    Private Sub OnNTrialsChanged(sender As Object, args As EventArgs)
        Try
            Me.MSE.NModels = CInt(Me.m_fpNTrials.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNTrialsChanged")
        End Try
    End Sub

    Private Sub OnNYearsToProjectChanged(sender As Object, args As EventArgs)
        Try
            Me.MSE.NYearsProject = CInt(Me.m_fpNYearsToProject.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNYearsToProjectChanged")
        End Try
    End Sub

    Private Sub OnMassBalanceTolChanged(sender As Object, args As EventArgs)
        Try
            Me.MSE.MassBalanceTol = CSng(Me.m_fpMassBalanceTol.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnMassBalanceTolChanged")
        End Try
    End Sub

    Private Sub OnMaxAttemptsChanged(sender As Object, args As EventArgs)
        Try
            Me.MSE.NMaxAttempts = CInt(Me.m_fpMaxAttempts.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnMaxAttemptsChanged")
        End Try
    End Sub

    Private Sub OnMaxTimeChanged(sender As Object, args As EventArgs)
        Try
            Me.MSE.NMaxTime = CSng(Me.m_fpMaxTime.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnMaxAttemptsChanged")
        End Try
    End Sub

    Private Sub OnDecreaseEffort(sender As Object, e As System.EventArgs) Handles m_btnDecreaseEffort.Click

        Try
            Dim frmMaxDecreaseEfforts As New frmEditDecreaseEffort()
            frmMaxDecreaseEfforts.Init(Me.UIContext, Me.MSE)
            frmMaxDecreaseEfforts.ShowDialog(Me)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnDecreaseEffort")
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
    Private Sub OnMSEStateChanged(ByVal man As cMSEStateMonitor)
        Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
    End Sub

#End Region ' Plug-in callback

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cMSEPluginPoint"/> connected to this form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private ReadOnly Property Plugin As cMSEPluginPoint
        Get
            Return Me.m_plugin
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cMSE"/> connected to this form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private ReadOnly Property MSE As cMSE
        Get
            Return Me.m_plugin.MSE
        End Get
    End Property

#End Region ' Internals

#Region " Path / model validation "

    Private Function ReviewDistParams() As Boolean

        Dim frmDisParams As New frmDistributionParameters()
        frmDisParams.Init(Me.UIContext, Me.Plugin)

        If frmDisParams.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            Me.MSE.InvalidateData()
            Return True
        End If
        Return False

    End Function

    Private Function SelectDataPath() As Boolean

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim bNeedsResolving As Boolean = False

        If Me.MSE.UseEwEPath Then
            Dim cmd As cShowOptionsCommand = DirectCast(cmdh.GetCommand(cShowOptionsCommand.cCOMMAND_NAME), cShowOptionsCommand)
            cmd.Invoke(ScientificInterfaceShared.Definitions.eApplicationOptionTypes.FileLocations)
            bNeedsResolving = cmd.UserHandled
        Else
            Dim cmd As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
            cmd.Invoke(Me.MSE.CustomPath, My.Resources.PROMPT_DATAPATH)
            If (cmd.Result = Windows.Forms.DialogResult.OK) Then
                Me.MSE.CustomPath = cmd.Directory
                bNeedsResolving = True
            End If
        End If

        Return bNeedsResolving

    End Function

    ''' <summary>
    ''' Interactively resolve MSE folder conflicts.
    ''' </summary>
    Private Function ResolveMSEPathConflicts() As Boolean

        ' Assume the worst
        Dim bPathValid As Boolean = False
        ' .. and forget all we know
        Me.MSE.InvalidateData(False)

        While Not bPathValid

            ' Check if input structure is missing
            If Not Me.MSE.IsInputStructureAvailable(False) Then

                ' Ask user to create folder structure
                If Me.MSE.AskUser(String.Format(My.Resources.PROMPT_DATAPATH_MISSING, Me.MSE.DataPath), eMessageReplyStyle.YES_NO) <> eMessageReply.OK Then
                    ' #User abort: abandon process
                    Return False
                End If

                ' Try to create folder structure
                If Me.MSE.IsInputStructureAvailable(True) Then
                    ' #Created: force user to examine distribution params, and generate input files
                    If (Me.ReviewDistParams) Then
                        ' #User went along: create all other data files
                        ' - Survivabilities
                        MSE.GenerateSurvivabilities()
                        ' - Diets
                        MSE.GenerateEmptyDietCSVs()
                        ' Re-assess state
#If DEBUG Then
                        Me.MSE.InvalidateData(False)
                        Debug.Assert(Me.MSE.IsInputDataCompatible(), "Cefas MSE default data generation logic is not working")
#End If
                    Else
                        ' #Not created. Now we're stuck with a messy folder structure that may not be used. Pollution!
                        bPathValid = True
                    End If
                Else
                    ' #No luck? Panic, and make the user try again
                    Me.MSE.InformUser(String.Format(My.Resources.PROMPT_DATAPATH_INACCESSIBLE, Me.MSE.DataPath), eMessageImportance.Critical)
                    bPathValid = Me.SelectDataPath()
                End If
            Else
                ' Input structure is there, but may be meant for a different model
                If (Not Me.MSE.IsInputDataCompatible()) Then
                    Me.MSE.InformUser(String.Format(My.Resources.PROMPT_DATAPATH_INCOMPATIBLE, Me.MSE.DataPath), eMessageImportance.Warning)
                    bPathValid = False
                Else
                    bPathValid = True
                End If
            End If

            If (Not bPathValid) Then

                If Not Me.SelectDataPath() Then
                    Return False
                End If

                Me.MSE.InvalidateData()
                bPathValid = (Me.MSE.IsInputStructureAvailable(False) And Me.MSE.IsInputDataCompatible())

            End If

        End While

        Return True

    End Function

#End Region ' Path / model validation

    Private Sub btnSampleSurvivabilities_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSampleSurvivabilities.Click
        MSE.GenerateSurvivabilities()
    End Sub

    Private Sub btnCreateDiet_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCreateDiet.Click

    End Sub
End Class
