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
        Me.m_tbNModels2Run = New System.Windows.Forms.TextBox()
        Me.m_lblNTrials = New System.Windows.Forms.Label()
        Me.btnLoadSampled = New System.Windows.Forms.Button()
        Me.m_tbNYearsProject = New System.Windows.Forms.TextBox()
        Me.m_lblNYears = New System.Windows.Forms.Label()
        Me.m_lblMassBalanceTol = New System.Windows.Forms.Label()
        Me.m_txtTolerance = New System.Windows.Forms.TextBox()
        Me.btnCreateModels = New System.Windows.Forms.Button()
        Me.m_btnGenDC = New System.Windows.Forms.Button()
        Me.m_btnShowTFMForm = New System.Windows.Forms.Button()
        Me.m_btnEcopathParams2 = New System.Windows.Forms.Button()
        Me.m_plStep2 = New System.Windows.Forms.Panel()
        Me.m_hrdStep2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_tbxNumAvailableModels = New System.Windows.Forms.TextBox()
        Me.m_tbNTrials = New System.Windows.Forms.TextBox()
        Me.m_plStep5 = New System.Windows.Forms.Panel()
        Me.m_hdrStep5 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblNModels = New System.Windows.Forms.Label()
        Me.m_plStep4 = New System.Windows.Forms.Panel()
        Me.m_hdrStep4 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblNumAvailableStrategies = New System.Windows.Forms.Label()
        Me.m_tbArea = New System.Windows.Forms.TextBox()
        Me.m_lblArea = New System.Windows.Forms.Label()
        Me.m_pbCefas = New System.Windows.Forms.PictureBox()
        Me.m_btn2 = New System.Windows.Forms.Button()
        Me.m_plStep3 = New System.Windows.Forms.Panel()
        Me.m_hdrStep3 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnDistParams = New System.Windows.Forms.Button()
        Me.m_tlpLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plStep1 = New System.Windows.Forms.Panel()
        Me.m_lblDataPath = New System.Windows.Forms.Label()
        Me.m_rbCustomPath = New System.Windows.Forms.RadioButton()
        Me.m_tbxPath = New System.Windows.Forms.TextBox()
        Me.m_lblInputParams = New System.Windows.Forms.Label()
        Me.m_rbEwEDefault = New System.Windows.Forms.RadioButton()
        Me.m_hdrStep1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnChangePath = New System.Windows.Forms.Button()
        Me.m_plStep2.SuspendLayout()
        Me.m_plStep5.SuspendLayout()
        Me.m_plStep4.SuspendLayout()
        CType(Me.m_pbCefas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_plStep3.SuspendLayout()
        Me.m_tlpLayout.SuspendLayout()
        Me.m_plStep1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tbNModels2Run
        '
        resources.ApplyResources(Me.m_tbNModels2Run, "m_tbNModels2Run")
        Me.m_tbNModels2Run.Name = "m_tbNModels2Run"
        '
        'm_lblNTrials
        '
        resources.ApplyResources(Me.m_lblNTrials, "m_lblNTrials")
        Me.m_lblNTrials.Name = "m_lblNTrials"
        '
        'btnLoadSampled
        '
        resources.ApplyResources(Me.btnLoadSampled, "btnLoadSampled")
        Me.btnLoadSampled.Name = "btnLoadSampled"
        Me.btnLoadSampled.UseVisualStyleBackColor = True
        '
        'm_tbNYearsProject
        '
        resources.ApplyResources(Me.m_tbNYearsProject, "m_tbNYearsProject")
        Me.m_tbNYearsProject.Name = "m_tbNYearsProject"
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
        'm_txtTolerance
        '
        resources.ApplyResources(Me.m_txtTolerance, "m_txtTolerance")
        Me.m_txtTolerance.Name = "m_txtTolerance"
        '
        'btnCreateModels
        '
        resources.ApplyResources(Me.btnCreateModels, "btnCreateModels")
        Me.btnCreateModels.Name = "btnCreateModels"
        Me.btnCreateModels.UseVisualStyleBackColor = True
        '
        'm_btnGenDC
        '
        resources.ApplyResources(Me.m_btnGenDC, "m_btnGenDC")
        Me.m_btnGenDC.Name = "m_btnGenDC"
        Me.m_btnGenDC.UseVisualStyleBackColor = True
        '
        'm_btnShowTFMForm
        '
        resources.ApplyResources(Me.m_btnShowTFMForm, "m_btnShowTFMForm")
        Me.m_btnShowTFMForm.Name = "m_btnShowTFMForm"
        Me.m_btnShowTFMForm.UseVisualStyleBackColor = True
        '
        'm_btnEcopathParams2
        '
        resources.ApplyResources(Me.m_btnEcopathParams2, "m_btnEcopathParams2")
        Me.m_btnEcopathParams2.Name = "m_btnEcopathParams2"
        Me.m_btnEcopathParams2.UseVisualStyleBackColor = True
        '
        'm_plStep2
        '
        Me.m_plStep2.Controls.Add(Me.m_hrdStep2)
        Me.m_plStep2.Controls.Add(Me.btnCreateModels)
        Me.m_plStep2.Controls.Add(Me.Label1)
        Me.m_plStep2.Controls.Add(Me.m_lblNTrials)
        Me.m_plStep2.Controls.Add(Me.m_tbxNumAvailableModels)
        Me.m_plStep2.Controls.Add(Me.m_tbNTrials)
        Me.m_plStep2.Controls.Add(Me.m_lblMassBalanceTol)
        Me.m_plStep2.Controls.Add(Me.m_txtTolerance)
        resources.ApplyResources(Me.m_plStep2, "m_plStep2")
        Me.m_plStep2.Name = "m_plStep2"
        '
        'm_hrdStep2
        '
        Me.m_hrdStep2.CanCollapseParent = True
        Me.m_hrdStep2.CollapsedParentHeight = 49
        resources.ApplyResources(Me.m_hrdStep2, "m_hrdStep2")
        Me.m_hrdStep2.IsCollapsed = False
        Me.m_hrdStep2.Name = "m_hrdStep2"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'm_tbxNumAvailableModels
        '
        resources.ApplyResources(Me.m_tbxNumAvailableModels, "m_tbxNumAvailableModels")
        Me.m_tbxNumAvailableModels.Name = "m_tbxNumAvailableModels"
        Me.m_tbxNumAvailableModels.ReadOnly = True
        '
        'm_tbNTrials
        '
        resources.ApplyResources(Me.m_tbNTrials, "m_tbNTrials")
        Me.m_tbNTrials.Name = "m_tbNTrials"
        '
        'm_plStep5
        '
        Me.m_plStep5.Controls.Add(Me.m_hdrStep5)
        Me.m_plStep5.Controls.Add(Me.m_tbNModels2Run)
        Me.m_plStep5.Controls.Add(Me.m_lblNYears)
        Me.m_plStep5.Controls.Add(Me.m_lblNModels)
        Me.m_plStep5.Controls.Add(Me.btnLoadSampled)
        Me.m_plStep5.Controls.Add(Me.m_tbNYearsProject)
        resources.ApplyResources(Me.m_plStep5, "m_plStep5")
        Me.m_plStep5.Name = "m_plStep5"
        '
        'm_hdrStep5
        '
        Me.m_hdrStep5.CanCollapseParent = False
        Me.m_hdrStep5.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrStep5, "m_hdrStep5")
        Me.m_hdrStep5.IsCollapsed = False
        Me.m_hdrStep5.Name = "m_hdrStep5"
        '
        'm_lblNModels
        '
        resources.ApplyResources(Me.m_lblNModels, "m_lblNModels")
        Me.m_lblNModels.Name = "m_lblNModels"
        '
        'm_plStep4
        '
        Me.m_plStep4.Controls.Add(Me.m_hdrStep4)
        Me.m_plStep4.Controls.Add(Me.m_btnShowTFMForm)
        Me.m_plStep4.Controls.Add(Me.m_lblNumAvailableStrategies)
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
        'm_lblNumAvailableStrategies
        '
        resources.ApplyResources(Me.m_lblNumAvailableStrategies, "m_lblNumAvailableStrategies")
        Me.m_lblNumAvailableStrategies.Name = "m_lblNumAvailableStrategies"
        '
        'm_tbArea
        '
        resources.ApplyResources(Me.m_tbArea, "m_tbArea")
        Me.m_tbArea.Name = "m_tbArea"
        '
        'm_lblArea
        '
        resources.ApplyResources(Me.m_lblArea, "m_lblArea")
        Me.m_lblArea.Name = "m_lblArea"
        '
        'm_pbCefas
        '
        Me.m_pbCefas.BackColor = System.Drawing.Color.White
        Me.m_pbCefas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.m_pbCefas, "m_pbCefas")
        Me.m_pbCefas.Name = "m_pbCefas"
        Me.m_pbCefas.TabStop = False
        '
        'm_btn2
        '
        resources.ApplyResources(Me.m_btn2, "m_btn2")
        Me.m_btn2.Name = "m_btn2"
        Me.m_btn2.UseVisualStyleBackColor = True
        '
        'm_plStep3
        '
        Me.m_plStep3.Controls.Add(Me.m_hdrStep3)
        Me.m_plStep3.Controls.Add(Me.m_lblArea)
        Me.m_plStep3.Controls.Add(Me.m_tbArea)
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
        'm_btnDistParams
        '
        resources.ApplyResources(Me.m_btnDistParams, "m_btnDistParams")
        Me.m_btnDistParams.Name = "m_btnDistParams"
        Me.m_btnDistParams.UseVisualStyleBackColor = True
        '
        'm_tlpLayout
        '
        resources.ApplyResources(Me.m_tlpLayout, "m_tlpLayout")
        Me.m_tlpLayout.Controls.Add(Me.m_plStep2, 0, 1)
        Me.m_tlpLayout.Controls.Add(Me.m_plStep3, 0, 2)
        Me.m_tlpLayout.Controls.Add(Me.m_plStep4, 0, 3)
        Me.m_tlpLayout.Controls.Add(Me.m_plStep5, 0, 4)
        Me.m_tlpLayout.Controls.Add(Me.m_pbCefas, 0, 5)
        Me.m_tlpLayout.Controls.Add(Me.m_plStep1, 0, 0)
        Me.m_tlpLayout.Name = "m_tlpLayout"
        '
        'm_plStep1
        '
        Me.m_plStep1.Controls.Add(Me.m_lblDataPath)
        Me.m_plStep1.Controls.Add(Me.m_rbCustomPath)
        Me.m_plStep1.Controls.Add(Me.m_tbxPath)
        Me.m_plStep1.Controls.Add(Me.m_lblInputParams)
        Me.m_plStep1.Controls.Add(Me.m_rbEwEDefault)
        Me.m_plStep1.Controls.Add(Me.m_hdrStep1)
        Me.m_plStep1.Controls.Add(Me.m_btnDistParams)
        Me.m_plStep1.Controls.Add(Me.m_btnChangePath)
        resources.ApplyResources(Me.m_plStep1, "m_plStep1")
        Me.m_plStep1.Name = "m_plStep1"
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
        'm_tbxPath
        '
        resources.ApplyResources(Me.m_tbxPath, "m_tbxPath")
        Me.m_tbxPath.Name = "m_tbxPath"
        Me.m_tbxPath.ReadOnly = True
        '
        'm_lblInputParams
        '
        resources.ApplyResources(Me.m_lblInputParams, "m_lblInputParams")
        Me.m_lblInputParams.Name = "m_lblInputParams"
        '
        'm_rbEwEDefault
        '
        resources.ApplyResources(Me.m_rbEwEDefault, "m_rbEwEDefault")
        Me.m_rbEwEDefault.Name = "m_rbEwEDefault"
        Me.m_rbEwEDefault.TabStop = True
        Me.m_rbEwEDefault.UseVisualStyleBackColor = True
        '
        'm_hdrStep1
        '
        Me.m_hdrStep1.CanCollapseParent = True
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
        'frmMSE
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_btnGenDC)
        Me.Controls.Add(Me.m_tlpLayout)
        Me.Controls.Add(Me.m_btn2)
        Me.Controls.Add(Me.m_btnEcopathParams2)
        Me.Name = "frmMSE"
        Me.m_plStep2.ResumeLayout(False)
        Me.m_plStep2.PerformLayout()
        Me.m_plStep5.ResumeLayout(False)
        Me.m_plStep5.PerformLayout()
        Me.m_plStep4.ResumeLayout(False)
        CType(Me.m_pbCefas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_plStep3.ResumeLayout(False)
        Me.m_plStep3.PerformLayout()
        Me.m_tlpLayout.ResumeLayout(False)
        Me.m_plStep1.ResumeLayout(False)
        Me.m_plStep1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tbNModels2Run As System.Windows.Forms.TextBox
    Private WithEvents m_lblNTrials As System.Windows.Forms.Label
    Private WithEvents btnLoadSampled As System.Windows.Forms.Button
    Private WithEvents m_tbNYearsProject As System.Windows.Forms.TextBox
    Private WithEvents m_lblNYears As System.Windows.Forms.Label
    Private WithEvents m_txtTolerance As System.Windows.Forms.TextBox
    Private WithEvents btnCreateModels As System.Windows.Forms.Button
    Private WithEvents m_btnGenDC As System.Windows.Forms.Button
    Private WithEvents m_lblMassBalanceTol As System.Windows.Forms.Label
    Private WithEvents m_btnShowTFMForm As System.Windows.Forms.Button
    Private WithEvents m_btnEcopathParams2 As System.Windows.Forms.Button
    Private WithEvents m_plStep2 As System.Windows.Forms.Panel
    Private WithEvents m_plStep5 As System.Windows.Forms.Panel
    Private WithEvents m_lblNModels As System.Windows.Forms.Label
    Private WithEvents m_plStep4 As System.Windows.Forms.Panel
    Private WithEvents m_pbCefas As System.Windows.Forms.PictureBox
    Private WithEvents m_lblArea As System.Windows.Forms.Label
    Private WithEvents m_btn2 As System.Windows.Forms.Button
    Private WithEvents m_plStep3 As System.Windows.Forms.Panel
    Private WithEvents m_btnDistParams As System.Windows.Forms.Button
    Private WithEvents m_tbArea As System.Windows.Forms.TextBox
    Private WithEvents m_tbNTrials As System.Windows.Forms.TextBox
    Private WithEvents m_hrdStep2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrStep4 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrStep3 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_tlpLayout As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_hdrStep5 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_plStep1 As System.Windows.Forms.Panel
    Private WithEvents m_hdrStep1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_rbCustomPath As System.Windows.Forms.RadioButton
    Private WithEvents m_lblInputParams As System.Windows.Forms.Label
    Private WithEvents m_rbEwEDefault As System.Windows.Forms.RadioButton
    Private WithEvents m_btnChangePath As System.Windows.Forms.Button
    Private WithEvents m_lblNumAvailableStrategies As System.Windows.Forms.Label
    Friend WithEvents m_lblDataPath As System.Windows.Forms.Label
    Private WithEvents m_tbxPath As System.Windows.Forms.TextBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents m_tbxNumAvailableModels As System.Windows.Forms.TextBox
End Class
