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
        Me.btnSample = New System.Windows.Forms.Button()
        Me.m_btnGamma = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.m_btnChangeDataDir = New System.Windows.Forms.Button()
        Me.btShowTFMForm = New System.Windows.Forms.Button()
        Me.m_btnEcopathParams2 = New System.Windows.Forms.Button()
        Me.m_plGamma = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.m_tbNTrials = New System.Windows.Forms.TextBox()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.m_lblNModels = New System.Windows.Forms.Label()
        Me.m_lblGenDC = New System.Windows.Forms.Label()
        Me.m_btnAdvancedSettings = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.m_tbArea = New System.Windows.Forms.TextBox()
        Me.m_lblArea = New System.Windows.Forms.Label()
        Me.m_lblStep1 = New System.Windows.Forms.Label()
        Me.m_lblStep2 = New System.Windows.Forms.Label()
        Me.m_lblStep3 = New System.Windows.Forms.Label()
        Me.m_pbCefas = New System.Windows.Forms.PictureBox()
        Me.m_btn2 = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.m_lblAreaInfo = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.m_lblStep4 = New System.Windows.Forms.Label()
        Me.m_lblDataDirectoryPath = New System.Windows.Forms.Label()
        Me.m_btnDistParams = New System.Windows.Forms.Button()
        Me.m_plGamma.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.m_pbCefas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
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
        'btnSample
        '
        resources.ApplyResources(Me.btnSample, "btnSample")
        Me.btnSample.Name = "btnSample"
        Me.btnSample.UseVisualStyleBackColor = True
        '
        'm_btnGamma
        '
        resources.ApplyResources(Me.m_btnGamma, "m_btnGamma")
        Me.m_btnGamma.Name = "m_btnGamma"
        Me.m_btnGamma.UseVisualStyleBackColor = True
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'm_btnChangeDataDir
        '
        resources.ApplyResources(Me.m_btnChangeDataDir, "m_btnChangeDataDir")
        Me.m_btnChangeDataDir.Name = "m_btnChangeDataDir"
        Me.m_btnChangeDataDir.UseVisualStyleBackColor = True
        '
        'btShowTFMForm
        '
        resources.ApplyResources(Me.btShowTFMForm, "btShowTFMForm")
        Me.btShowTFMForm.Name = "btShowTFMForm"
        Me.btShowTFMForm.UseVisualStyleBackColor = True
        '
        'm_btnEcopathParams2
        '
        resources.ApplyResources(Me.m_btnEcopathParams2, "m_btnEcopathParams2")
        Me.m_btnEcopathParams2.Name = "m_btnEcopathParams2"
        Me.m_btnEcopathParams2.UseVisualStyleBackColor = True
        '
        'm_plGamma
        '
        Me.m_plGamma.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_plGamma.Controls.Add(Me.m_btnGamma)
        resources.ApplyResources(Me.m_plGamma, "m_plGamma")
        Me.m_plGamma.Name = "m_plGamma"
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.btnSample)
        Me.Panel4.Controls.Add(Me.m_lblNTrials)
        Me.Panel4.Controls.Add(Me.m_tbNTrials)
        resources.ApplyResources(Me.Panel4, "Panel4")
        Me.Panel4.Name = "Panel4"
        '
        'm_tbNTrials
        '
        resources.ApplyResources(Me.m_tbNTrials, "m_tbNTrials")
        Me.m_tbNTrials.Name = "m_tbNTrials"
        '
        'Panel6
        '
        Me.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel6.Controls.Add(Me.m_tbNModels2Run)
        Me.Panel6.Controls.Add(Me.m_lblNYears)
        Me.Panel6.Controls.Add(Me.m_lblNModels)
        Me.Panel6.Controls.Add(Me.btnLoadSampled)
        Me.Panel6.Controls.Add(Me.m_tbNYearsProject)
        resources.ApplyResources(Me.Panel6, "Panel6")
        Me.Panel6.Name = "Panel6"
        '
        'm_lblNModels
        '
        resources.ApplyResources(Me.m_lblNModels, "m_lblNModels")
        Me.m_lblNModels.Name = "m_lblNModels"
        '
        'm_lblGenDC
        '
        resources.ApplyResources(Me.m_lblGenDC, "m_lblGenDC")
        Me.m_lblGenDC.Name = "m_lblGenDC"
        '
        'm_btnAdvancedSettings
        '
        resources.ApplyResources(Me.m_btnAdvancedSettings, "m_btnAdvancedSettings")
        Me.m_btnAdvancedSettings.Name = "m_btnAdvancedSettings"
        Me.m_btnAdvancedSettings.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.btShowTFMForm)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
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
        'm_lblStep1
        '
        resources.ApplyResources(Me.m_lblStep1, "m_lblStep1")
        Me.m_lblStep1.Name = "m_lblStep1"
        '
        'm_lblStep2
        '
        resources.ApplyResources(Me.m_lblStep2, "m_lblStep2")
        Me.m_lblStep2.Name = "m_lblStep2"
        '
        'm_lblStep3
        '
        resources.ApplyResources(Me.m_lblStep3, "m_lblStep3")
        Me.m_lblStep3.Name = "m_lblStep3"
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
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.m_lblAreaInfo)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.m_lblArea)
        Me.Panel2.Controls.Add(Me.m_tbArea)
        resources.ApplyResources(Me.Panel2, "Panel2")
        Me.Panel2.Name = "Panel2"
        '
        'm_lblAreaInfo
        '
        resources.ApplyResources(Me.m_lblAreaInfo, "m_lblAreaInfo")
        Me.m_lblAreaInfo.Name = "m_lblAreaInfo"
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'm_lblStep4
        '
        resources.ApplyResources(Me.m_lblStep4, "m_lblStep4")
        Me.m_lblStep4.Name = "m_lblStep4"
        '
        'm_lblDataDirectoryPath
        '
        resources.ApplyResources(Me.m_lblDataDirectoryPath, "m_lblDataDirectoryPath")
        Me.m_lblDataDirectoryPath.Name = "m_lblDataDirectoryPath"
        '
        'm_btnDistParams
        '
        resources.ApplyResources(Me.m_btnDistParams, "m_btnDistParams")
        Me.m_btnDistParams.Name = "m_btnDistParams"
        Me.m_btnDistParams.UseVisualStyleBackColor = True
        '
        'frmMSE
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_btnDistParams)
        Me.Controls.Add(Me.m_lblDataDirectoryPath)
        Me.Controls.Add(Me.m_lblStep4)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.m_btn2)
        Me.Controls.Add(Me.m_pbCefas)
        Me.Controls.Add(Me.m_lblStep3)
        Me.Controls.Add(Me.m_lblStep2)
        Me.Controls.Add(Me.m_lblStep1)
        Me.Controls.Add(Me.m_lblMassBalanceTol)
        Me.Controls.Add(Me.m_txtTolerance)
        Me.Controls.Add(Me.m_btnChangeDataDir)
        Me.Controls.Add(Me.m_btnAdvancedSettings)
        Me.Controls.Add(Me.m_lblGenDC)
        Me.Controls.Add(Me.Panel6)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.m_plGamma)
        Me.Controls.Add(Me.m_btnEcopathParams2)
        Me.Controls.Add(Me.Label3)
        Me.Name = "frmMSE"
        Me.m_plGamma.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        CType(Me.m_pbCefas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_tbNModels2Run As System.Windows.Forms.TextBox
    Private WithEvents m_lblNTrials As System.Windows.Forms.Label
    Private WithEvents btnLoadSampled As System.Windows.Forms.Button
    Private WithEvents m_tbNYearsProject As System.Windows.Forms.TextBox
    Private WithEvents m_lblNYears As System.Windows.Forms.Label
    Private WithEvents m_txtTolerance As System.Windows.Forms.TextBox
    Private WithEvents btnSample As System.Windows.Forms.Button
    Private WithEvents m_btnGamma As System.Windows.Forms.Button
    Private WithEvents m_lblMassBalanceTol As System.Windows.Forms.Label
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents m_btnChangeDataDir As System.Windows.Forms.Button
    Private WithEvents btShowTFMForm As System.Windows.Forms.Button
    Private WithEvents m_btnEcopathParams2 As System.Windows.Forms.Button
    Private WithEvents m_plGamma As System.Windows.Forms.Panel
    Private WithEvents Panel4 As System.Windows.Forms.Panel
    Private WithEvents Panel6 As System.Windows.Forms.Panel
    Private WithEvents m_lblGenDC As System.Windows.Forms.Label
    Private WithEvents m_btnAdvancedSettings As System.Windows.Forms.Button
    Private WithEvents m_lblNModels As System.Windows.Forms.Label
    Private WithEvents Panel1 As System.Windows.Forms.Panel
    Private WithEvents m_lblStep1 As System.Windows.Forms.Label
    Private WithEvents m_lblStep2 As System.Windows.Forms.Label
    Private WithEvents m_lblStep3 As System.Windows.Forms.Label
    Private WithEvents m_pbCefas As System.Windows.Forms.PictureBox
    Private WithEvents m_lblArea As System.Windows.Forms.Label
    Private WithEvents m_btn2 As System.Windows.Forms.Button
    Private WithEvents Panel2 As System.Windows.Forms.Panel
    Private WithEvents m_lblAreaInfo As System.Windows.Forms.Label
    Private WithEvents Label10 As System.Windows.Forms.Label
    Private WithEvents m_lblStep4 As System.Windows.Forms.Label
    Private WithEvents m_lblDataDirectoryPath As System.Windows.Forms.Label
    Private WithEvents m_btnDistParams As System.Windows.Forms.Button
    Private WithEvents m_tbArea As System.Windows.Forms.TextBox
    Private WithEvents m_tbNTrials As System.Windows.Forms.TextBox
End Class
