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

Public Class frmMSE

    Private mCore As cCore
    Private mMSE As cMSE
    Private frmTargetF As frmTFMpolicy

    Private m_fpArea As cEwEFormatProvider = Nothing
    Private m_fpNModelsToRun As cEwEFormatProvider = Nothing
    Private m_fpNTrials As cEwEFormatProvider = Nothing
    Private m_fpNYearsToProject As cEwEFormatProvider = Nothing
    Private m_fpMassBalanceTol As cEwEFormatProvider = Nothing

    Public Sub New(MSE As cMSE, uic As cUIContext)

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.mMSE = MSE
        Me.mCore = uic.Core
 
    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        ' -- Set up control interactions --

        ' Connect area UI control to live Ecopath data
        Me.m_fpArea = New cPropertyFormatProvider(Me.UIContext, m_tbArea, Me.Core.EwEModel, eVarNameFlags.Area)
        ' Area can be made editable from here by not setting the format provider style:
        Me.m_fpArea.Style = ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags.NotEditable

        Me.m_fpNModelsToRun = New cEwEFormatProvider(Me.UIContext, Me.m_tbNModels2Run, GetType(Integer))
        Me.m_fpNModelsToRun.Value = Me.mMSE.NModels2Run
        AddHandler Me.m_fpNModelsToRun.OnValueChanged, AddressOf OnNModels2RunChanged

        Me.m_fpNTrials = New cEwEFormatProvider(Me.UIContext, Me.m_tbNTrials, GetType(Integer))
        Me.m_fpNTrials.Value = Me.mMSE.NTrials
        AddHandler Me.m_fpNTrials.OnValueChanged, AddressOf OnNTrialsChanged

        Me.m_fpNYearsToProject = New cEwEFormatProvider(Me.UIContext, m_tbNYearsProject, GetType(Integer))
        Me.m_fpNYearsToProject.Value = Me.mMSE.NYearsProject
        AddHandler Me.m_fpNYearsToProject.OnValueChanged, AddressOf OnNYearsToProjectChanged

        Me.m_fpMassBalanceTol = New cEwEFormatProvider(Me.UIContext, Me.m_txtTolerance, GetType(Single), New cVariableMetaData(0, 0.1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_fpMassBalanceTol.Value = Me.mMSE.MassBalanceTol
        AddHandler Me.m_fpMassBalanceTol.OnValueChanged, AddressOf OnMassBalanceTolChanged

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.UIContext IsNot Nothing) Then

            Me.m_fpArea.Release()

            RemoveHandler Me.m_fpNModelsToRun.OnValueChanged, AddressOf OnNModels2RunChanged
            Me.m_fpNModelsToRun.Release()

            Me.m_fpNTrials.Release()
            RemoveHandler Me.m_fpNTrials.OnValueChanged, AddressOf OnNTrialsChanged

            Me.m_fpNYearsToProject.Release()
            RemoveHandler Me.m_fpNYearsToProject.OnValueChanged, AddressOf OnNYearsToProjectChanged

            Me.m_fpMassBalanceTol.Release()
            RemoveHandler Me.m_fpMassBalanceTol.OnValueChanged, AddressOf OnMassBalanceTolChanged

        End If

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        'Dim controller As cMSEStateMonitor = Me.mMSE.Controller

        'Me.m_plStep1.Enabled = controller.IsStateAvailable(cMSEStateMonitor.eState.Idle)
        'Me.m_plStep2.Enabled = controller.IsStateAvailable(cMSEStateMonitor.eState.HasParams)
        'Me.m_plStep3.Enabled = controller.IsStateAvailable(cMSEStateMonitor.eState.HasModels)
        'Me.m_plStep4.Enabled = controller.IsStateAvailable(cMSEStateMonitor.eState.HasModels)

        Me.m_lblDataDirectoryPath.Text = Me.mMSE.DataPath

    End Sub

#End Region ' Form overrides

#Region " Control events "

    Private Sub btnSample_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateModels.Click

        mMSE.Create1DimParams("MaxRelFeedingTime")
        mMSE.Create1DimParams("FeedingTimeAdjustRate")
        mMSE.Create1DimParams("OtherMortFeedingTime")
        mMSE.Create1DimParams("PredEffectFeedingTime")
        mMSE.Create1DimParams("DenDepCatchability")
        mMSE.Create1DimParams("QBMaxxQBio")
        mMSE.Create1DimParams("SwitchingPower")
        mMSE.CreateVulnerabilities()
        mMSE.GenerateEcopathParamaters()

    End Sub

    Private Sub OnPathPrefChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbEwEDefault.CheckedChanged, m_rbCustomPath.CheckedChanged
        Try
            Me.mMSE.UseEwEPath = Me.m_rbEwEDefault.Checked
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub btnLoadSampled_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnLoadSampled.Click
        Try
            ' JS 02Oct13: Moved Strategies extraction test flag to the plug-in, which does the actual work
            '             From the UI point of view, we just want strategies. The plug-in does the optimizating
            mMSE.ExtractHCR()
            mMSE.ChangeEffortFlag = True
            mMSE.LoadSampledParams()
            mMSE.ChangeEffortFlag = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnSelectDataPath(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnChangePath.Click

        ' JS 30Sep13: Use EwE dialog framework here
        Try
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler

            If Me.mMSE.UseEwEPath Then
                Dim cmd As cShowOptionsCommand = DirectCast(cmdh.GetCommand(cShowOptionsCommand.cCOMMAND_NAME), cShowOptionsCommand)
                cmd.Invoke(ScientificInterfaceShared.Definitions.eApplicationOptionTypes.FileLocations)
            Else
                Dim cmd As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
                cmd.Invoke(Me.mMSE.CustomPath, My.Resources.PROMPT_DATAPATH)
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
        mMSE.ExtractHCR()

        'Ok now the interface
        If Me.frmTargetF IsNot Nothing Then
            bhasForm = Not frmTargetF.IsDisposed
        End If
        If Not bhasForm Then
            frmTargetF = New frmTFMpolicy()
            frmTargetF.Init(Me.UIContext, Me.mMSE)
        End If

        frmTargetF.Show()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btn2.Click

        Try
            mMSE.Create1DimParams("MaxRelFeedingTime")
            mMSE.Create1DimParams("FeedingTimeAdjustRate")
            mMSE.Create1DimParams("OtherMortFeedingTime")
            mMSE.Create1DimParams("PredEffectFeedingTime")
            mMSE.Create1DimParams("DenDepCatchability")
            mMSE.Create1DimParams("QBMaxxQBio")
            mMSE.Create1DimParams("SwitchingPower")
            'mMSE.Create2DimParams("DietComposition")
            mMSE.CreateVulnerabilities()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnDistParams_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnDistParams.Click

        Try
            Dim frmDisParams As New frmDistributionParameters()
            frmDisParams.Init(Me.UIContext, Me.mMSE)
            frmDisParams.Show(Me)
        Catch ex As Exception

        End Try

        ' Perhaps the user has made a useful contribution ;)
        Me.UpdateControls()

    End Sub

    Private Sub OnNModels2RunChanged(sender As Object, args As EventArgs)
        Try
            Me.mMSE.NModels2Run = CInt(Me.m_fpNModelsToRun.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNModels2RunChanged")
        End Try
    End Sub

    Private Sub OnNTrialsChanged(sender As Object, args As EventArgs)
        Try
            Me.mMSE.NTrials = CInt(Me.m_fpNTrials.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNTrialsChanged")
        End Try
    End Sub

    Private Sub OnNYearsToProjectChanged(sender As Object, args As EventArgs)
        Try
            Me.mMSE.NYearsProject = CInt(Me.m_fpNYearsToProject.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnNYearsToProjectChanged")
        End Try
    End Sub

    Private Sub OnMassBalanceTolChanged(sender As Object, args As EventArgs)
        Try
            Me.mMSE.MassBalanceTol = CSng(Me.m_fpMassBalanceTol.Value)
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:OnMassBalanceTolChanged")
        End Try
    End Sub

#End Region ' Control events

End Class
