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
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSE
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSE))
        Me.m_tbxNModels2Run = New System.Windows.Forms.TextBox()
        Me.m_lblNTrials = New System.Windows.Forms.Label()
        Me.m_btnRun = New System.Windows.Forms.Button()
        Me.m_tbxNYearsProject = New System.Windows.Forms.TextBox()
        Me.m_lblNYears = New System.Windows.Forms.Label()
        Me.m_lblMassBalanceTol = New System.Windows.Forms.Label()
        Me.m_tbxTolerance = New System.Windows.Forms.TextBox()
        Me.m_btnCreateModels = New System.Windows.Forms.Button()
        Me.m_btnReviewTFM = New System.Windows.Forms.Button()
        Me.m_plStep2 = New System.Windows.Forms.Panel()
        Me.m_pbCompatible = New System.Windows.Forms.PictureBox()
        Me.m_hdrStep2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblAvailableModels = New System.Windows.Forms.Label()
        Me.m_tbxNumAvailableModels = New System.Windows.Forms.TextBox()
        Me.m_tbxNTrials = New System.Windows.Forms.TextBox()
        Me.m_lblMaxAttempts = New System.Windows.Forms.Label()
        Me.m_lblMaxTime = New System.Windows.Forms.Label()
        Me.m_tbxMaxAttempts = New System.Windows.Forms.TextBox()
        Me.m_tbxMaxTime = New System.Windows.Forms.TextBox()
        Me.m_plStep4 = New System.Windows.Forms.Panel()
        Me.m_hdrStep4 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblNModels = New System.Windows.Forms.Label()
        Me.m_plStep3 = New System.Windows.Forms.Panel()
        Me.m_hdrStep3 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblAvailableStrategies = New System.Windows.Forms.Label()
        Me.m_tbxNumAvailableFishingStrategies = New System.Windows.Forms.TextBox()
        Me.m_tbxArea = New System.Windows.Forms.TextBox()
        Me.m_lblArea = New System.Windows.Forms.Label()
        Me.m_pbCefas = New System.Windows.Forms.PictureBox()
        Me.m_lblAreaUnit = New System.Windows.Forms.Label()
        Me.m_btnReviewDistParms = New System.Windows.Forms.Button()
        Me.m_tlpLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plStep1 = New System.Windows.Forms.Panel()
        Me.m_lblPathValue = New System.Windows.Forms.Label()
        Me.m_lblDataPath = New System.Windows.Forms.Label()
        Me.m_rbCustomPath = New System.Windows.Forms.RadioButton()
        Me.m_lblInputParams = New System.Windows.Forms.Label()
        Me.m_rbEwEDefault = New System.Windows.Forms.RadioButton()
        Me.m_tbxParamStatus = New System.Windows.Forms.TextBox()
        Me.m_hdrStep1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnChangePath = New System.Windows.Forms.Button()
        Me.btnDecreaseEffort = New System.Windows.Forms.Button()
        Me.m_plStep2.SuspendLayout()
        CType(Me.m_pbCompatible, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_plStep4.SuspendLayout()
        Me.m_plStep3.SuspendLayout()
        CType(Me.m_pbCefas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpLayout.SuspendLayout()
        Me.m_plStep1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tbxNModels2Run
        '
        resources.ApplyResources(Me.m_tbxNModels2Run, "m_tbxNModels2Run")
        Me.m_tbxNModels2Run.Name = "m_tbxNModels2Run"
        '
        'm_lblNTrials
        '
        resources.ApplyResources(Me.m_lblNTrials, "m_lblNTrials")
        Me.m_lblNTrials.Name = "m_lblNTrials"
        '
        'm_btnRun
        '
        resources.ApplyResources(Me.m_btnRun, "m_btnRun")
        Me.m_btnRun.Name = "m_btnRun"
        Me.m_btnRun.UseVisualStyleBackColor = True
        '
        'm_tbxNYearsProject
        '
        resources.ApplyResources(Me.m_tbxNYearsProject, "m_tbxNYearsProject")
        Me.m_tbxNYearsProject.Name = "m_tbxNYearsProject"
        '
        'm_lblNYears
        '
        resources.ApplyResources(Me.m_lblNYears, "m_lblNYears")
        Me.m_lblNYears.Name = "m_lblNYears"
        '
        'm_lblMassBalanceTol
        '
        resources.ApplyResources(Me.m_lblMassBalanceTol, "m_lblMassBalanceTol")
        Me.m_lblMassBalanceTol.Name = "m_lblMassBalanceTol"
        '
        'm_tbxTolerance
        '
        resources.ApplyResources(Me.m_tbxTolerance, "m_tbxTolerance")
        Me.m_tbxTolerance.Name = "m_tbxTolerance"
        '
        'm_btnCreateModels
        '
        resources.ApplyResources(Me.m_btnCreateModels, "m_btnCreateModels")
        Me.m_btnCreateModels.Name = "m_btnCreateModels"
        Me.m_btnCreateModels.UseVisualStyleBackColor = True
        '
        'm_btnReviewTFM
        '
        resources.ApplyResources(Me.m_btnReviewTFM, "m_btnReviewTFM")
        Me.m_btnReviewTFM.Name = "m_btnReviewTFM"
        Me.m_btnReviewTFM.UseVisualStyleBackColor = True
        '
        'm_plStep2
        '
        Me.m_plStep2.Controls.Add(Me.m_pbCompatible)
        Me.m_plStep2.Controls.Add(Me.m_lblMassBalanceTol)
        Me.m_plStep2.Controls.Add(Me.m_tbxTolerance)
        Me.m_plStep2.Controls.Add(Me.m_hdrStep2)
        Me.m_plStep2.Controls.Add(Me.m_btnCreateModels)
        Me.m_plStep2.Controls.Add(Me.m_lblAvailableModels)
        Me.m_plStep2.Controls.Add(Me.m_lblNTrials)
        Me.m_plStep2.Controls.Add(Me.m_tbxNumAvailableModels)
        Me.m_plStep2.Controls.Add(Me.m_tbxNTrials)
        Me.m_plStep2.Controls.Add(Me.m_lblMaxAttempts)
        Me.m_plStep2.Controls.Add(Me.m_lblMaxTime)
        Me.m_plStep2.Controls.Add(Me.m_tbxMaxAttempts)
        Me.m_plStep2.Controls.Add(Me.m_tbxMaxTime)
        resources.ApplyResources(Me.m_plStep2, "m_plStep2")
        Me.m_plStep2.Name = "m_plStep2"
        '
        'm_pbCompatible
        '
        resources.ApplyResources(Me.m_pbCompatible, "m_pbCompatible")
        Me.m_pbCompatible.Name = "m_pbCompatible"
        Me.m_pbCompatible.TabStop = False
        '
        'm_hdrStep2
        '
        Me.m_hdrStep2.CanCollapseParent = True
        Me.m_hdrStep2.CollapsedParentHeight = 72
        resources.ApplyResources(Me.m_hdrStep2, "m_hdrStep2")
        Me.m_hdrStep2.IsCollapsed = False
        Me.m_hdrStep2.Name = "m_hdrStep2"
        '
        'm_lblAvailableModels
        '
        resources.ApplyResources(Me.m_lblAvailableModels, "m_lblAvailableModels")
        Me.m_lblAvailableModels.Name = "m_lblAvailableModels"
        '
        'm_tbxNumAvailableModels
        '
        resources.ApplyResources(Me.m_tbxNumAvailableModels, "m_tbxNumAvailableModels")
        Me.m_tbxNumAvailableModels.Name = "m_tbxNumAvailableModels"
        Me.m_tbxNumAvailableModels.ReadOnly = True
        '
        'm_tbxNTrials
        '
        resources.ApplyResources(Me.m_tbxNTrials, "m_tbxNTrials")
        Me.m_tbxNTrials.Name = "m_tbxNTrials"
        '
        'm_lblMaxAttempts
        '
        resources.ApplyResources(Me.m_lblMaxAttempts, "m_lblMaxAttempts")
        Me.m_lblMaxAttempts.Name = "m_lblMaxAttempts"
        '
        'm_lblMaxTime
        '
        resources.ApplyResources(Me.m_lblMaxTime, "m_lblMaxTime")
        Me.m_lblMaxTime.Name = "m_lblMaxTime"
        '
        'm_tbxMaxAttempts
        '
        resources.ApplyResources(Me.m_tbxMaxAttempts, "m_tbxMaxAttempts")
        Me.m_tbxMaxAttempts.Name = "m_tbxMaxAttempts"
        '
        'm_tbxMaxTime
        '
        resources.ApplyResources(Me.m_tbxMaxTime, "m_tbxMaxTime")
        Me.m_tbxMaxTime.Name = "m_tbxMaxTime"
        '
        'm_plStep4
        '
        Me.m_plStep4.Controls.Add(Me.m_hdrStep4)
        Me.m_plStep4.Controls.Add(Me.m_tbxNModels2Run)
        Me.m_plStep4.Controls.Add(Me.m_lblNYears)
        Me.m_plStep4.Controls.Add(Me.m_lblNModels)
        Me.m_plStep4.Controls.Add(Me.m_btnRun)
        Me.m_plStep4.Controls.Add(Me.m_tbxNYearsProject)
        resources.ApplyResources(Me.m_plStep4, "m_plStep4")
        Me.m_plStep4.Name = "m_plStep4"
        '
        'm_hdrStep4
        '
        Me.m_hdrStep4.CanCollapseParent = False
        Me.m_hdrStep4.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrStep4, "m_hdrStep4")
        Me.m_hdrStep4.IsCollapsed = False
        Me.m_hdrStep4.Name = "m_hdrStep4"
        '
        'm_lblNModels
        '
        resources.ApplyResources(Me.m_lblNModels, "m_lblNModels")
        Me.m_lblNModels.Name = "m_lblNModels"
        '
        'm_plStep3
        '
        Me.m_plStep3.Controls.Add(Me.m_hdrStep3)
        Me.m_plStep3.Controls.Add(Me.m_btnReviewTFM)
        Me.m_plStep3.Controls.Add(Me.m_lblAvailableStrategies)
        Me.m_plStep3.Controls.Add(Me.m_tbxNumAvailableFishingStrategies)
        resources.ApplyResources(Me.m_plStep3, "m_plStep3")
        Me.m_plStep3.Name = "m_plStep3"
        '
        'm_hdrStep3
        '
        Me.m_hdrStep3.CanCollapseParent = False
        Me.m_hdrStep3.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrStep3, "m_hdrStep3")
        Me.m_hdrStep3.IsCollapsed = False
        Me.m_hdrStep3.Name = "m_hdrStep3"
        '
        'm_lblAvailableStrategies
        '
        resources.ApplyResources(Me.m_lblAvailableStrategies, "m_lblAvailableStrategies")
        Me.m_lblAvailableStrategies.Name = "m_lblAvailableStrategies"
        '
        'm_tbxNumAvailableFishingStrategies
        '
        resources.ApplyResources(Me.m_tbxNumAvailableFishingStrategies, "m_tbxNumAvailableFishingStrategies")
        Me.m_tbxNumAvailableFishingStrategies.Name = "m_tbxNumAvailableFishingStrategies"
        Me.m_tbxNumAvailableFishingStrategies.ReadOnly = True
        '
        'm_tbxArea
        '
        resources.ApplyResources(Me.m_tbxArea, "m_tbxArea")
        Me.m_tbxArea.Name = "m_tbxArea"
        '
        'm_lblArea
        '
        resources.ApplyResources(Me.m_lblArea, "m_lblArea")
        Me.m_lblArea.MaximumSize = New System.Drawing.Size(100, 0)
        Me.m_lblArea.Name = "m_lblArea"
        '
        'm_pbCefas
        '
        Me.m_pbCefas.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.m_pbCefas, "m_pbCefas")
        Me.m_pbCefas.Name = "m_pbCefas"
        Me.m_pbCefas.TabStop = False
        '
        'm_lblAreaUnit
        '
        resources.ApplyResources(Me.m_lblAreaUnit, "m_lblAreaUnit")
        Me.m_lblAreaUnit.MaximumSize = New System.Drawing.Size(100, 0)
        Me.m_lblAreaUnit.Name = "m_lblAreaUnit"
        '
        'm_btnReviewDistParms
        '
        resources.ApplyResources(Me.m_btnReviewDistParms, "m_btnReviewDistParms")
        Me.m_btnReviewDistParms.Name = "m_btnReviewDistParms"
        Me.m_btnReviewDistParms.UseVisualStyleBackColor = True
        '
        'm_tlpLayout
        '
        resources.ApplyResources(Me.m_tlpLayout, "m_tlpLayout")
        Me.m_tlpLayout.Controls.Add(Me.m_plStep2, 0, 1)
        Me.m_tlpLayout.Controls.Add(Me.m_plStep3, 0, 2)
        Me.m_tlpLayout.Controls.Add(Me.m_plStep4, 0, 3)
        Me.m_tlpLayout.Controls.Add(Me.m_pbCefas, 0, 4)
        Me.m_tlpLayout.Controls.Add(Me.m_plStep1, 0, 0)
        Me.m_tlpLayout.MinimumSize = New System.Drawing.Size(450, 500)
        Me.m_tlpLayout.Name = "m_tlpLayout"
        '
        'm_plStep1
        '
        Me.m_plStep1.Controls.Add(Me.m_lblPathValue)
        Me.m_plStep1.Controls.Add(Me.m_lblDataPath)
        Me.m_plStep1.Controls.Add(Me.m_lblAreaUnit)
        Me.m_plStep1.Controls.Add(Me.m_rbCustomPath)
        Me.m_plStep1.Controls.Add(Me.m_lblArea)
        Me.m_plStep1.Controls.Add(Me.m_tbxArea)
        Me.m_plStep1.Controls.Add(Me.m_lblInputParams)
        Me.m_plStep1.Controls.Add(Me.m_rbEwEDefault)
        Me.m_plStep1.Controls.Add(Me.m_tbxParamStatus)
        Me.m_plStep1.Controls.Add(Me.m_hdrStep1)
        Me.m_plStep1.Controls.Add(Me.m_btnReviewDistParms)
        Me.m_plStep1.Controls.Add(Me.m_btnChangePath)
        resources.ApplyResources(Me.m_plStep1, "m_plStep1")
        Me.m_plStep1.Name = "m_plStep1"
        '
        'm_lblPathValue
        '
        resources.ApplyResources(Me.m_lblPathValue, "m_lblPathValue")
        Me.m_lblPathValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_lblPathValue.Cursor = System.Windows.Forms.Cursors.Hand
        Me.m_lblPathValue.Name = "m_lblPathValue"
        '
        'm_lblDataPath
        '
        resources.ApplyResources(Me.m_lblDataPath, "m_lblDataPath")
        Me.m_lblDataPath.Name = "m_lblDataPath"
        '
        'm_rbCustomPath
        '
        resources.ApplyResources(Me.m_rbCustomPath, "m_rbCustomPath")
        Me.m_rbCustomPath.Name = "m_rbCustomPath"
        Me.m_rbCustomPath.TabStop = True
        Me.m_rbCustomPath.UseVisualStyleBackColor = True
        '
        'm_lblInputParams
        '
        resources.ApplyResources(Me.m_lblInputParams, "m_lblInputParams")
        Me.m_lblInputParams.MaximumSize = New System.Drawing.Size(100, 0)
        Me.m_lblInputParams.Name = "m_lblInputParams"
        '
        'm_rbEwEDefault
        '
        resources.ApplyResources(Me.m_rbEwEDefault, "m_rbEwEDefault")
        Me.m_rbEwEDefault.Name = "m_rbEwEDefault"
        Me.m_rbEwEDefault.TabStop = True
        Me.m_rbEwEDefault.UseVisualStyleBackColor = True
        '
        'm_tbxParamStatus
        '
        resources.ApplyResources(Me.m_tbxParamStatus, "m_tbxParamStatus")
        Me.m_tbxParamStatus.Name = "m_tbxParamStatus"
        Me.m_tbxParamStatus.ReadOnly = True
        '
        'm_hdrStep1
        '
        Me.m_hdrStep1.CanCollapseParent = False
        Me.m_hdrStep1.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrStep1, "m_hdrStep1")
        Me.m_hdrStep1.IsCollapsed = False
        Me.m_hdrStep1.Name = "m_hdrStep1"
        '
        'm_btnChangePath
        '
        resources.ApplyResources(Me.m_btnChangePath, "m_btnChangePath")
        Me.m_btnChangePath.Name = "m_btnChangePath"
        Me.m_btnChangePath.UseVisualStyleBackColor = True
        '
        'btnDecreaseEffort
        '
        resources.ApplyResources(Me.btnDecreaseEffort, "btnDecreaseEffort")
        Me.btnDecreaseEffort.Name = "btnDecreaseEffort"
        Me.btnDecreaseEffort.UseVisualStyleBackColor = True
        '
        'frmMSE
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.btnDecreaseEffort)
        Me.Controls.Add(Me.m_tlpLayout)
        Me.Name = "frmMSE"
        Me.m_plStep2.ResumeLayout(False)
        Me.m_plStep2.PerformLayout()
        CType(Me.m_pbCompatible, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_plStep4.ResumeLayout(False)
        Me.m_plStep4.PerformLayout()
        Me.m_plStep3.ResumeLayout(False)
        Me.m_plStep3.PerformLayout()
        CType(Me.m_pbCefas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpLayout.ResumeLayout(False)
        Me.m_plStep1.ResumeLayout(False)
        Me.m_plStep1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tbxNModels2Run As System.Windows.Forms.TextBox
    Private WithEvents m_lblNTrials As System.Windows.Forms.Label
    Private WithEvents m_btnRun As System.Windows.Forms.Button
    Private WithEvents m_tbxNYearsProject As System.Windows.Forms.TextBox
    Private WithEvents m_lblNYears As System.Windows.Forms.Label
    Private WithEvents m_tbxTolerance As System.Windows.Forms.TextBox
    Private WithEvents m_btnCreateModels As System.Windows.Forms.Button
    Private WithEvents m_lblMassBalanceTol As System.Windows.Forms.Label
    Private WithEvents m_btnReviewTFM As System.Windows.Forms.Button
    Private WithEvents m_plStep2 As System.Windows.Forms.Panel
    Private WithEvents m_plStep4 As System.Windows.Forms.Panel
    Private WithEvents m_lblNModels As System.Windows.Forms.Label
    Private WithEvents m_plStep3 As System.Windows.Forms.Panel
    Private WithEvents m_pbCefas As System.Windows.Forms.PictureBox
    Private WithEvents m_lblArea As System.Windows.Forms.Label
    Private WithEvents m_btnReviewDistParms As System.Windows.Forms.Button
    Private WithEvents m_tbxArea As System.Windows.Forms.TextBox
    Private WithEvents m_tbxNTrials As System.Windows.Forms.TextBox
    Private WithEvents m_hdrStep2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrStep3 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_tlpLayout As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_hdrStep4 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_plStep1 As System.Windows.Forms.Panel
    Private WithEvents m_hdrStep1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_rbCustomPath As System.Windows.Forms.RadioButton
    Private WithEvents m_lblInputParams As System.Windows.Forms.Label
    Private WithEvents m_rbEwEDefault As System.Windows.Forms.RadioButton
    Private WithEvents m_btnChangePath As System.Windows.Forms.Button
    Friend WithEvents m_lblDataPath As System.Windows.Forms.Label
    Private WithEvents m_lblAvailableModels As System.Windows.Forms.Label
    Private WithEvents m_tbxNumAvailableModels As System.Windows.Forms.TextBox
    Private WithEvents m_lblAvailableStrategies As System.Windows.Forms.Label
    Private WithEvents m_tbxNumAvailableFishingStrategies As System.Windows.Forms.TextBox
    Private WithEvents m_pbCompatible As System.Windows.Forms.PictureBox
    Private WithEvents m_lblMaxAttempts As System.Windows.Forms.Label
    Private WithEvents m_tbxMaxAttempts As System.Windows.Forms.TextBox
    Private WithEvents m_lblAreaUnit As System.Windows.Forms.Label
    Private WithEvents m_tbxParamStatus As System.Windows.Forms.TextBox
    Private WithEvents m_lblMaxTime As System.Windows.Forms.Label
    Private WithEvents m_tbxMaxTime As System.Windows.Forms.TextBox
    Friend WithEvents btnDecreaseEffort As System.Windows.Forms.Button
    Private WithEvents m_lblPathValue As System.Windows.Forms.Label
End Class
