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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInvokeR
    Inherits System.Windows.Forms.Form

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
        Me.m_lblR = New System.Windows.Forms.Label()
        Me.m_btnChooseR = New System.Windows.Forms.Button()
        Me.m_lblScript = New System.Windows.Forms.Label()
        Me.m_tbxScriptFile = New System.Windows.Forms.TextBox()
        Me.m_btnChooseScript = New System.Windows.Forms.Button()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_tbxSCORFile = New System.Windows.Forms.TextBox()
        Me.m_btnChooseSCOR = New System.Windows.Forms.Button()
        Me.m_tbxOutFile = New System.Windows.Forms.TextBox()
        Me.m_lblPlaceholder = New System.Windows.Forms.Label()
        Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plOptions = New System.Windows.Forms.Panel()
        Me.m_cmbR = New System.Windows.Forms.ComboBox()
        Me.m_hdrSCOR = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_rbManagedSCOR = New System.Windows.Forms.RadioButton()
        Me.m_rbCustomSCOR = New System.Windows.Forms.RadioButton()
        Me.m_hdrSettings = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_plRun = New System.Windows.Forms.Panel()
        Me.m_plResult = New System.Windows.Forms.Panel()
        Me.m_tcDebug = New System.Windows.Forms.TabControl()
        Me.m_tpgScript = New System.Windows.Forms.TabPage()
        Me.m_tbxScriptOut = New System.Windows.Forms.TextBox()
        Me.m_tpgOutput = New System.Windows.Forms.TabPage()
        Me.m_tbxOutput = New System.Windows.Forms.TextBox()
        Me.m_tpgErrors = New System.Windows.Forms.TabPage()
        Me.m_tbxErrors = New System.Windows.Forms.TextBox()
        Me.m_hdrResults = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tlpContent.SuspendLayout()
        Me.m_plOptions.SuspendLayout()
        Me.m_plRun.SuspendLayout()
        Me.m_plResult.SuspendLayout()
        Me.m_tcDebug.SuspendLayout()
        Me.m_tpgScript.SuspendLayout()
        Me.m_tpgOutput.SuspendLayout()
        Me.m_tpgErrors.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblR
        '
        Me.m_lblR.AutoSize = True
        Me.m_lblR.Location = New System.Drawing.Point(5, 32)
        Me.m_lblR.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.m_lblR.Name = "m_lblR"
        Me.m_lblR.Size = New System.Drawing.Size(79, 17)
        Me.m_lblR.TabIndex = 1
        Me.m_lblR.Text = "R &program:"
        '
        'm_btnChooseR
        '
        Me.m_btnChooseR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseR.Location = New System.Drawing.Point(361, 26)
        Me.m_btnChooseR.Margin = New System.Windows.Forms.Padding(4)
        Me.m_btnChooseR.Name = "m_btnChooseR"
        Me.m_btnChooseR.Size = New System.Drawing.Size(29, 28)
        Me.m_btnChooseR.TabIndex = 3
        Me.m_btnChooseR.Text = ".."
        Me.m_btnChooseR.UseVisualStyleBackColor = True
        '
        'm_lblScript
        '
        Me.m_lblScript.AutoSize = True
        Me.m_lblScript.Location = New System.Drawing.Point(5, 68)
        Me.m_lblScript.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.m_lblScript.Name = "m_lblScript"
        Me.m_lblScript.Size = New System.Drawing.Size(60, 17)
        Me.m_lblScript.TabIndex = 4
        Me.m_lblScript.Text = "R &script:"
        '
        'm_tbxScriptFile
        '
        Me.m_tbxScriptFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxScriptFile.Location = New System.Drawing.Point(127, 64)
        Me.m_tbxScriptFile.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tbxScriptFile.Name = "m_tbxScriptFile"
        Me.m_tbxScriptFile.Size = New System.Drawing.Size(225, 22)
        Me.m_tbxScriptFile.TabIndex = 5
        '
        'm_btnChooseScript
        '
        Me.m_btnChooseScript.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseScript.Location = New System.Drawing.Point(361, 62)
        Me.m_btnChooseScript.Margin = New System.Windows.Forms.Padding(4)
        Me.m_btnChooseScript.Name = "m_btnChooseScript"
        Me.m_btnChooseScript.Size = New System.Drawing.Size(29, 28)
        Me.m_btnChooseScript.TabIndex = 6
        Me.m_btnChooseScript.Text = ".."
        Me.m_btnChooseScript.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        Me.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.m_btnOK.Location = New System.Drawing.Point(276, 4)
        Me.m_btnOK.Margin = New System.Windows.Forms.Padding(4)
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.Size = New System.Drawing.Size(115, 28)
        Me.m_btnOK.TabIndex = 0
        Me.m_btnOK.Text = "Run"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_tbxSCORFile
        '
        Me.m_tbxSCORFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxSCORFile.Location = New System.Drawing.Point(123, 155)
        Me.m_tbxSCORFile.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tbxSCORFile.Name = "m_tbxSCORFile"
        Me.m_tbxSCORFile.ReadOnly = True
        Me.m_tbxSCORFile.Size = New System.Drawing.Size(229, 22)
        Me.m_tbxSCORFile.TabIndex = 11
        '
        'm_btnChooseSCOR
        '
        Me.m_btnChooseSCOR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseSCOR.Location = New System.Drawing.Point(361, 155)
        Me.m_btnChooseSCOR.Margin = New System.Windows.Forms.Padding(4)
        Me.m_btnChooseSCOR.Name = "m_btnChooseSCOR"
        Me.m_btnChooseSCOR.Size = New System.Drawing.Size(29, 28)
        Me.m_btnChooseSCOR.TabIndex = 12
        Me.m_btnChooseSCOR.Text = ".."
        Me.m_btnChooseSCOR.UseVisualStyleBackColor = True
        '
        'm_tbxOutFile
        '
        Me.m_tbxOutFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxOutFile.Location = New System.Drawing.Point(127, 96)
        Me.m_tbxOutFile.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tbxOutFile.MaxLength = 24
        Me.m_tbxOutFile.Name = "m_tbxOutFile"
        Me.m_tbxOutFile.Size = New System.Drawing.Size(263, 22)
        Me.m_tbxOutFile.TabIndex = 8
        Me.m_tbxOutFile.Text = "%FILENAME%"
        '
        'm_lblPlaceholder
        '
        Me.m_lblPlaceholder.AutoSize = True
        Me.m_lblPlaceholder.Location = New System.Drawing.Point(7, 100)
        Me.m_lblPlaceholder.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.m_lblPlaceholder.Name = "m_lblPlaceholder"
        Me.m_lblPlaceholder.Size = New System.Drawing.Size(77, 17)
        Me.m_lblPlaceholder.TabIndex = 7
        Me.m_lblPlaceholder.Text = "&Output file:"
        '
        'm_tlpContent
        '
        Me.m_tlpContent.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tlpContent.ColumnCount = 1
        Me.m_tlpContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpContent.Controls.Add(Me.m_plOptions, 0, 0)
        Me.m_tlpContent.Controls.Add(Me.m_plRun, 0, 1)
        Me.m_tlpContent.Controls.Add(Me.m_plResult, 0, 2)
        Me.m_tlpContent.Location = New System.Drawing.Point(16, 15)
        Me.m_tlpContent.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tlpContent.Name = "m_tlpContent"
        Me.m_tlpContent.RowCount = 3
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpContent.Size = New System.Drawing.Size(400, 425)
        Me.m_tlpContent.TabIndex = 0
        '
        'm_plOptions
        '
        Me.m_plOptions.Controls.Add(Me.m_cmbR)
        Me.m_plOptions.Controls.Add(Me.m_tbxOutFile)
        Me.m_plOptions.Controls.Add(Me.m_hdrSCOR)
        Me.m_plOptions.Controls.Add(Me.m_lblPlaceholder)
        Me.m_plOptions.Controls.Add(Me.m_rbManagedSCOR)
        Me.m_plOptions.Controls.Add(Me.m_rbCustomSCOR)
        Me.m_plOptions.Controls.Add(Me.m_hdrSettings)
        Me.m_plOptions.Controls.Add(Me.m_tbxSCORFile)
        Me.m_plOptions.Controls.Add(Me.m_btnChooseSCOR)
        Me.m_plOptions.Controls.Add(Me.m_lblR)
        Me.m_plOptions.Controls.Add(Me.m_btnChooseScript)
        Me.m_plOptions.Controls.Add(Me.m_tbxScriptFile)
        Me.m_plOptions.Controls.Add(Me.m_btnChooseR)
        Me.m_plOptions.Controls.Add(Me.m_lblScript)
        Me.m_plOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plOptions.Location = New System.Drawing.Point(0, 0)
        Me.m_plOptions.Margin = New System.Windows.Forms.Padding(0)
        Me.m_plOptions.Name = "m_plOptions"
        Me.m_plOptions.Size = New System.Drawing.Size(400, 209)
        Me.m_plOptions.TabIndex = 0
        '
        'm_cmbR
        '
        Me.m_cmbR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_cmbR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbR.FormattingEnabled = True
        Me.m_cmbR.Location = New System.Drawing.Point(127, 28)
        Me.m_cmbR.Margin = New System.Windows.Forms.Padding(4)
        Me.m_cmbR.Name = "m_cmbR"
        Me.m_cmbR.Size = New System.Drawing.Size(225, 24)
        Me.m_cmbR.TabIndex = 2
        '
        'm_hdrSCOR
        '
        Me.m_hdrSCOR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrSCOR.CanCollapseParent = False
        Me.m_hdrSCOR.CollapsedParentHeight = 0
        Me.m_hdrSCOR.IsCollapsed = False
        Me.m_hdrSCOR.Location = New System.Drawing.Point(5, 129)
        Me.m_hdrSCOR.Margin = New System.Windows.Forms.Padding(0)
        Me.m_hdrSCOR.Name = "m_hdrSCOR"
        Me.m_hdrSCOR.Size = New System.Drawing.Size(385, 22)
        Me.m_hdrSCOR.TabIndex = 9
        Me.m_hdrSCOR.Text = "SCOR file for R"
        Me.m_hdrSCOR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_rbManagedSCOR
        '
        Me.m_rbManagedSCOR.AutoSize = True
        Me.m_rbManagedSCOR.Location = New System.Drawing.Point(9, 185)
        Me.m_rbManagedSCOR.Margin = New System.Windows.Forms.Padding(4)
        Me.m_rbManagedSCOR.Name = "m_rbManagedSCOR"
        Me.m_rbManagedSCOR.Size = New System.Drawing.Size(138, 21)
        Me.m_rbManagedSCOR.TabIndex = 13
        Me.m_rbManagedSCOR.TabStop = True
        Me.m_rbManagedSCOR.Text = "&System managed"
        Me.m_rbManagedSCOR.UseVisualStyleBackColor = True
        '
        'm_rbCustomSCOR
        '
        Me.m_rbCustomSCOR.AutoSize = True
        Me.m_rbCustomSCOR.Location = New System.Drawing.Point(9, 156)
        Me.m_rbCustomSCOR.Margin = New System.Windows.Forms.Padding(4)
        Me.m_rbCustomSCOR.Name = "m_rbCustomSCOR"
        Me.m_rbCustomSCOR.Size = New System.Drawing.Size(102, 21)
        Me.m_rbCustomSCOR.TabIndex = 10
        Me.m_rbCustomSCOR.TabStop = True
        Me.m_rbCustomSCOR.Text = "&Custom file:"
        Me.m_rbCustomSCOR.UseVisualStyleBackColor = True
        '
        'm_hdrSettings
        '
        Me.m_hdrSettings.CanCollapseParent = True
        Me.m_hdrSettings.CollapsedParentHeight = 76
        Me.m_hdrSettings.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdrSettings.IsCollapsed = False
        Me.m_hdrSettings.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrSettings.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.m_hdrSettings.Name = "m_hdrSettings"
        Me.m_hdrSettings.Size = New System.Drawing.Size(400, 22)
        Me.m_hdrSettings.TabIndex = 0
        Me.m_hdrSettings.Text = "Settings"
        Me.m_hdrSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_plRun
        '
        Me.m_plRun.Controls.Add(Me.m_btnOK)
        Me.m_plRun.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plRun.Location = New System.Drawing.Point(0, 209)
        Me.m_plRun.Margin = New System.Windows.Forms.Padding(0)
        Me.m_plRun.Name = "m_plRun"
        Me.m_plRun.Size = New System.Drawing.Size(400, 36)
        Me.m_plRun.TabIndex = 1
        '
        'm_plResult
        '
        Me.m_plResult.Controls.Add(Me.m_tcDebug)
        Me.m_plResult.Controls.Add(Me.m_hdrResults)
        Me.m_plResult.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plResult.Location = New System.Drawing.Point(4, 249)
        Me.m_plResult.Margin = New System.Windows.Forms.Padding(4)
        Me.m_plResult.Name = "m_plResult"
        Me.m_plResult.Size = New System.Drawing.Size(392, 172)
        Me.m_plResult.TabIndex = 2
        '
        'm_tcDebug
        '
        Me.m_tcDebug.Controls.Add(Me.m_tpgScript)
        Me.m_tcDebug.Controls.Add(Me.m_tpgOutput)
        Me.m_tcDebug.Controls.Add(Me.m_tpgErrors)
        Me.m_tcDebug.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tcDebug.Location = New System.Drawing.Point(0, 22)
        Me.m_tcDebug.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tcDebug.Name = "m_tcDebug"
        Me.m_tcDebug.SelectedIndex = 0
        Me.m_tcDebug.Size = New System.Drawing.Size(392, 150)
        Me.m_tcDebug.TabIndex = 1
        '
        'm_tpgScript
        '
        Me.m_tpgScript.Controls.Add(Me.m_tbxScriptOut)
        Me.m_tpgScript.Location = New System.Drawing.Point(4, 25)
        Me.m_tpgScript.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tpgScript.Name = "m_tpgScript"
        Me.m_tpgScript.Size = New System.Drawing.Size(487, 149)
        Me.m_tpgScript.TabIndex = 0
        Me.m_tpgScript.Text = "Script"
        Me.m_tpgScript.UseVisualStyleBackColor = True
        '
        'm_tbxScriptOut
        '
        Me.m_tbxScriptOut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbxScriptOut.Location = New System.Drawing.Point(0, 0)
        Me.m_tbxScriptOut.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tbxScriptOut.Multiline = True
        Me.m_tbxScriptOut.Name = "m_tbxScriptOut"
        Me.m_tbxScriptOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.m_tbxScriptOut.Size = New System.Drawing.Size(487, 149)
        Me.m_tbxScriptOut.TabIndex = 0
        Me.m_tbxScriptOut.WordWrap = False
        '
        'm_tpgOutput
        '
        Me.m_tpgOutput.Controls.Add(Me.m_tbxOutput)
        Me.m_tpgOutput.Location = New System.Drawing.Point(4, 25)
        Me.m_tpgOutput.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tpgOutput.Name = "m_tpgOutput"
        Me.m_tpgOutput.Size = New System.Drawing.Size(487, 149)
        Me.m_tpgOutput.TabIndex = 1
        Me.m_tpgOutput.Text = "Output"
        Me.m_tpgOutput.UseVisualStyleBackColor = True
        '
        'm_tbxOutput
        '
        Me.m_tbxOutput.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbxOutput.Location = New System.Drawing.Point(0, 0)
        Me.m_tbxOutput.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tbxOutput.Multiline = True
        Me.m_tbxOutput.Name = "m_tbxOutput"
        Me.m_tbxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.m_tbxOutput.Size = New System.Drawing.Size(487, 149)
        Me.m_tbxOutput.TabIndex = 1
        Me.m_tbxOutput.WordWrap = False
        '
        'm_tpgErrors
        '
        Me.m_tpgErrors.Controls.Add(Me.m_tbxErrors)
        Me.m_tpgErrors.Location = New System.Drawing.Point(4, 25)
        Me.m_tpgErrors.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tpgErrors.Name = "m_tpgErrors"
        Me.m_tpgErrors.Size = New System.Drawing.Size(384, 121)
        Me.m_tpgErrors.TabIndex = 2
        Me.m_tpgErrors.Text = "Errors"
        Me.m_tpgErrors.UseVisualStyleBackColor = True
        '
        'm_tbxErrors
        '
        Me.m_tbxErrors.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbxErrors.Location = New System.Drawing.Point(0, 0)
        Me.m_tbxErrors.Margin = New System.Windows.Forms.Padding(4)
        Me.m_tbxErrors.Multiline = True
        Me.m_tbxErrors.Name = "m_tbxErrors"
        Me.m_tbxErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.m_tbxErrors.Size = New System.Drawing.Size(384, 121)
        Me.m_tbxErrors.TabIndex = 1
        Me.m_tbxErrors.WordWrap = False
        '
        'm_hdrResults
        '
        Me.m_hdrResults.CanCollapseParent = False
        Me.m_hdrResults.CollapsedParentHeight = 0
        Me.m_hdrResults.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdrResults.IsCollapsed = False
        Me.m_hdrResults.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrResults.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.m_hdrResults.Name = "m_hdrResults"
        Me.m_hdrResults.Size = New System.Drawing.Size(392, 22)
        Me.m_hdrResults.TabIndex = 0
        Me.m_hdrResults.Text = "Results"
        Me.m_hdrResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmInvokeR
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(432, 455)
        Me.Controls.Add(Me.m_tlpContent)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(370, 400)
        Me.Name = "frmInvokeR"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Invoke R NETWRK"
        Me.m_tlpContent.ResumeLayout(False)
        Me.m_plOptions.ResumeLayout(False)
        Me.m_plOptions.PerformLayout()
        Me.m_plRun.ResumeLayout(False)
        Me.m_plResult.ResumeLayout(False)
        Me.m_tcDebug.ResumeLayout(False)
        Me.m_tpgScript.ResumeLayout(False)
        Me.m_tpgScript.PerformLayout()
        Me.m_tpgOutput.ResumeLayout(False)
        Me.m_tpgOutput.PerformLayout()
        Me.m_tpgErrors.ResumeLayout(False)
        Me.m_tpgErrors.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btnChooseR As System.Windows.Forms.Button
    Private WithEvents m_btnChooseScript As System.Windows.Forms.Button
    Private WithEvents m_tbxSCORFile As System.Windows.Forms.TextBox
    Private WithEvents m_btnChooseSCOR As System.Windows.Forms.Button
    Private WithEvents m_tbxScriptFile As System.Windows.Forms.TextBox
    Private WithEvents m_lblScript As System.Windows.Forms.Label
    Private WithEvents m_lblR As System.Windows.Forms.Label
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_tbxOutFile As System.Windows.Forms.TextBox
    Private WithEvents m_lblPlaceholder As System.Windows.Forms.Label
    Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_plOptions As System.Windows.Forms.Panel
    Private WithEvents m_hdrSettings As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_plRun As System.Windows.Forms.Panel
    Private WithEvents m_hdrSCOR As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_rbCustomSCOR As System.Windows.Forms.RadioButton
    Private WithEvents m_plResult As System.Windows.Forms.Panel
    Private WithEvents m_tcDebug As System.Windows.Forms.TabControl
    Private WithEvents m_tpgScript As System.Windows.Forms.TabPage
    Private WithEvents m_tpgOutput As System.Windows.Forms.TabPage
    Private WithEvents m_tpgErrors As System.Windows.Forms.TabPage
    Private WithEvents m_hdrResults As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_rbManagedSCOR As System.Windows.Forms.RadioButton
    Private WithEvents m_tbxScriptOut As System.Windows.Forms.TextBox
    Private WithEvents m_tbxOutput As System.Windows.Forms.TextBox
    Private WithEvents m_tbxErrors As System.Windows.Forms.TextBox
    Private WithEvents m_cmbR As System.Windows.Forms.ComboBox
End Class
