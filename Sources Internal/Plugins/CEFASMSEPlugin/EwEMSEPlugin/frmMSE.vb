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
    Private StrategiesExtracted As Boolean 'this is a flag used to determine whether the strategies have already been loads and if so not to load them again
    Private frmTargetF As frmTFMpolicy
    Private m_bAdvanced As Boolean = False

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
        Me.StrategiesExtracted = False

    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        ' -- Set up control interactions --

        Me.m_lblDataDirectoryPath.Text = mMSE.DataPath

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

        ' ToDo_JS: Update the state of controls depending on the run state of the plug-in
        ' - Are all input files generated?

        m_lblMassBalanceTol.Visible = Me.m_bAdvanced
        m_txtTolerance.Visible = Me.m_bAdvanced
        m_lblGenDC.Visible = Me.m_bAdvanced
        m_plGamma.Visible = Me.m_bAdvanced
        m_btnGamma.Visible = Me.m_bAdvanced
        m_btnEcopathParams2.Visible = Me.m_bAdvanced
        m_btn2.Visible = Me.m_bAdvanced

    End Sub

#End Region ' Form overrides

#Region " Control events "

    Private Sub btnSample_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSample.Click
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

    Private Sub btnGamma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnGamma.Click
        Try
            GenerateEmptyDietcsv()
        Catch ex As Exception
            cLog.Write(ex, "CefasMSE:GenerateEmptyDietcsv")
        End Try
    End Sub

    Private Sub btnLoadSampled_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnLoadSampled.Click
        Try
            If StrategiesExtracted = False Then 'This is to prevent it loading the strategies more than once
                mMSE.ExtractHCR()
                StrategiesExtracted = True
            End If
            mMSE.ChangeEffortFlag = True
            mMSE.LoadSampledParams()
            mMSE.ChangeEffortFlag = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnSelectDataPath(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnChangeDataDir.Click

        ' JS 30Sep13: Use EwE dialog framework here
        Try

            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cShowOptionsCommand = DirectCast(cmdh.GetCommand(cShowOptionsCommand.cCOMMAND_NAME), cShowOptionsCommand)

            cmd.Invoke(ScientificInterfaceShared.Definitions.eApplicationOptionTypes.FileLocations)

        Catch ex As Exception
            cLog.Write(ex, "CEFASMSE:OnSelectDataPath")
        End Try

    End Sub

    Private Sub btShowTFMForm_Click(sender As System.Object, e As System.EventArgs) _
        Handles btShowTFMForm.Click

        Dim bhasForm As Boolean

        'First make sure the Harvest Controls Rules have been loaded
        'this is so the interface has some data
        If StrategiesExtracted = False Then 'This is to prevent it loading the strategies more than once
            mMSE.ExtractHCR()
            StrategiesExtracted = True
        End If

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


    Private Sub OnToggleAdvancedView(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAdvancedSettings.Click

        Me.m_bAdvanced = Not Me.m_bAdvanced
        Me.UpdateControls()

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

    ' This method really should move to the main engine: the plug-in
    Private Sub GenerateEmptyDietcsv()

        Dim sPath As String = cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "DietComposition.csv", True)
        Dim diet_csvout As StreamWriter = cMSEUtils.GetWriter(sPath, False)
        Dim mean As Single

        If (diet_csvout Is Nothing) Then Return

        diet_csvout.Write("Predator,Prey,PredIndex,PreyIndex,Interacts,Mean")
        diet_csvout.WriteLine()

        For iPred As Integer = 1 To mCore.nLivingGroups
            If mCore.EcoPathGroupInputs(iPred).ImpDiet > 0 Then
                mean = mCore.EcoPathGroupInputs(iPred).ImpDiet
                diet_csvout.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & ",Imports," & iPred & ",0,1," & cStringUtils.ToCSVField(mean))
            Else
                diet_csvout.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & ",Imports," & iPred & ",0,0,0")
            End If

            For iPrey As Integer = 1 To mCore.nGroups
                If mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) > 0 Then
                    mean = mCore.EcoPathGroupInputs(iPred).DietComp(iPrey)
                    diet_csvout.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & "," & cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPrey).Name) & "," & iPred & "," & iPrey & ",1," & cStringUtils.ToCSVField(mean))
                Else
                    diet_csvout.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & "," & cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPrey).Name) & "," & iPred & "," & iPrey & ",0,0")
                End If
            Next
        Next

        cMSEUtils.ReleaseWriter(diet_csvout)

    End Sub

End Class
