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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
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
        Me.m_tbxR = New System.Windows.Forms.TextBox()
        Me.m_btnChooseR = New System.Windows.Forms.Button()
        Me.m_lblScript = New System.Windows.Forms.Label()
        Me.m_tbxScript = New System.Windows.Forms.TextBox()
        Me.m_btnChooseScript = New System.Windows.Forms.Button()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_tbxSCOR = New System.Windows.Forms.TextBox()
        Me.m_btnChooseSCOR = New System.Windows.Forms.Button()
        Me.m_tbxPlaceholder = New System.Windows.Forms.TextBox()
        Me.m_lblPlaceholder = New System.Windows.Forms.Label()
        Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plOptions = New System.Windows.Forms.Panel()
        Me.m_hdrSCOR = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_rbCustomSCOR = New System.Windows.Forms.RadioButton()
        Me.m_rbManagedSCOR = New System.Windows.Forms.RadioButton()
        Me.m_hdrSettings = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_plRun = New System.Windows.Forms.Panel()
        Me.m_plResult = New System.Windows.Forms.Panel()
        Me.m_tcDebug = New System.Windows.Forms.TabControl()
        Me.m_tpgScript = New System.Windows.Forms.TabPage()
        Me.m_lbxScript = New System.Windows.Forms.ListBox()
        Me.m_tpgOutput = New System.Windows.Forms.TabPage()
        Me.m_lbxOutput = New System.Windows.Forms.ListBox()
        Me.m_tpgErrors = New System.Windows.Forms.TabPage()
        Me.m_lbxError = New System.Windows.Forms.ListBox()
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
        Me.m_lblR.Location = New System.Drawing.Point(4, 26)
        Me.m_lblR.Name = "m_lblR"
        Me.m_lblR.Size = New System.Drawing.Size(59, 13)
        Me.m_lblR.TabIndex = 1
        Me.m_lblR.Text = "R &program:"
        '
        'm_tbxR
        '
        Me.m_tbxR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxR.Location = New System.Drawing.Point(95, 23)
        Me.m_tbxR.Name = "m_tbxR"
        Me.m_tbxR.ReadOnly = True
        Me.m_tbxR.Size = New System.Drawing.Size(235, 20)
        Me.m_tbxR.TabIndex = 2
        '
        'm_btnChooseR
        '
        Me.m_btnChooseR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseR.Location = New System.Drawing.Point(336, 21)
        Me.m_btnChooseR.Name = "m_btnChooseR"
        Me.m_btnChooseR.Size = New System.Drawing.Size(70, 23)
        Me.m_btnChooseR.TabIndex = 3
        Me.m_btnChooseR.Text = "Choose..."
        Me.m_btnChooseR.UseVisualStyleBackColor = True
        '
        'm_lblScript
        '
        Me.m_lblScript.AutoSize = True
        Me.m_lblScript.Location = New System.Drawing.Point(4, 55)
        Me.m_lblScript.Name = "m_lblScript"
        Me.m_lblScript.Size = New System.Drawing.Size(46, 13)
        Me.m_lblScript.TabIndex = 4
        Me.m_lblScript.Text = "R &script:"
        '
        'm_tbxScript
        '
        Me.m_tbxScript.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxScript.Location = New System.Drawing.Point(95, 52)
        Me.m_tbxScript.Name = "m_tbxScript"
        Me.m_tbxScript.ReadOnly = True
        Me.m_tbxScript.Size = New System.Drawing.Size(235, 20)
        Me.m_tbxScript.TabIndex = 5
        '
        'm_btnChooseScript
        '
        Me.m_btnChooseScript.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseScript.Location = New System.Drawing.Point(336, 50)
        Me.m_btnChooseScript.Name = "m_btnChooseScript"
        Me.m_btnChooseScript.Size = New System.Drawing.Size(70, 23)
        Me.m_btnChooseScript.TabIndex = 6
        Me.m_btnChooseScript.Text = "Choose..."
        Me.m_btnChooseScript.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        Me.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.m_btnOK.Location = New System.Drawing.Point(336, 3)
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.Size = New System.Drawing.Size(70, 23)
        Me.m_btnOK.TabIndex = 0
        Me.m_btnOK.Text = "Run"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_tbxSCOR
        '
        Me.m_tbxSCOR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxSCOR.Location = New System.Drawing.Point(95, 97)
        Me.m_tbxSCOR.Name = "m_tbxSCOR"
        Me.m_tbxSCOR.ReadOnly = True
        Me.m_tbxSCOR.Size = New System.Drawing.Size(235, 20)
        Me.m_tbxSCOR.TabIndex = 9
        '
        'm_btnChooseSCOR
        '
        Me.m_btnChooseSCOR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseSCOR.Location = New System.Drawing.Point(336, 97)
        Me.m_btnChooseSCOR.Name = "m_btnChooseSCOR"
        Me.m_btnChooseSCOR.Size = New System.Drawing.Size(70, 23)
        Me.m_btnChooseSCOR.TabIndex = 10
        Me.m_btnChooseSCOR.Text = "Choose..."
        Me.m_btnChooseSCOR.UseVisualStyleBackColor = True
        '
        'm_tbxPlaceholder
        '
        Me.m_tbxPlaceholder.Location = New System.Drawing.Point(95, 144)
        Me.m_tbxPlaceholder.MaxLength = 24
        Me.m_tbxPlaceholder.Name = "m_tbxPlaceholder"
        Me.m_tbxPlaceholder.Size = New System.Drawing.Size(120, 20)
        Me.m_tbxPlaceholder.TabIndex = 13
        Me.m_tbxPlaceholder.Text = "%FILENAME%"
        '
        'm_lblPlaceholder
        '
        Me.m_lblPlaceholder.AutoSize = True
        Me.m_lblPlaceholder.Location = New System.Drawing.Point(7, 148)
        Me.m_lblPlaceholder.Name = "m_lblPlaceholder"
        Me.m_lblPlaceholder.Size = New System.Drawing.Size(78, 13)
        Me.m_lblPlaceholder.TabIndex = 12
        Me.m_lblPlaceholder.Text = "SCOR file field:"
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
        Me.m_tlpContent.Location = New System.Drawing.Point(12, 12)
        Me.m_tlpContent.Name = "m_tlpContent"
        Me.m_tlpContent.RowCount = 3
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29.0!))
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpContent.Size = New System.Drawing.Size(410, 443)
        Me.m_tlpContent.TabIndex = 0
        '
        'm_plOptions
        '
        Me.m_plOptions.Controls.Add(Me.m_hdrSCOR)
        Me.m_plOptions.Controls.Add(Me.m_rbManagedSCOR)
        Me.m_plOptions.Controls.Add(Me.m_rbCustomSCOR)
        Me.m_plOptions.Controls.Add(Me.m_hdrSettings)
        Me.m_plOptions.Controls.Add(Me.m_tbxSCOR)
        Me.m_plOptions.Controls.Add(Me.m_btnChooseSCOR)
        Me.m_plOptions.Controls.Add(Me.m_lblR)
        Me.m_plOptions.Controls.Add(Me.m_btnChooseScript)
        Me.m_plOptions.Controls.Add(Me.m_tbxPlaceholder)
        Me.m_plOptions.Controls.Add(Me.m_tbxR)
        Me.m_plOptions.Controls.Add(Me.m_lblPlaceholder)
        Me.m_plOptions.Controls.Add(Me.m_tbxScript)
        Me.m_plOptions.Controls.Add(Me.m_btnChooseR)
        Me.m_plOptions.Controls.Add(Me.m_lblScript)
        Me.m_plOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plOptions.Location = New System.Drawing.Point(0, 0)
        Me.m_plOptions.Margin = New System.Windows.Forms.Padding(0)
        Me.m_plOptions.Name = "m_plOptions"
        Me.m_plOptions.Size = New System.Drawing.Size(410, 170)
        Me.m_plOptions.TabIndex = 0
        '
        'm_hdrSCOR
        '
        Me.m_hdrSCOR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrSCOR.CanCollapseParent = False
        Me.m_hdrSCOR.CollapsedParentHeight = 0
        Me.m_hdrSCOR.IsCollapsed = False
        Me.m_hdrSCOR.Location = New System.Drawing.Point(7, 76)
        Me.m_hdrSCOR.Margin = New System.Windows.Forms.Padding(0)
        Me.m_hdrSCOR.Name = "m_hdrSCOR"
        Me.m_hdrSCOR.Size = New System.Drawing.Size(399, 18)
        Me.m_hdrSCOR.TabIndex = 7
        Me.m_hdrSCOR.Text = "SCOR file"
        Me.m_hdrSCOR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_rbCustomSCOR
        '
        Me.m_rbCustomSCOR.AutoSize = True
        Me.m_rbCustomSCOR.Location = New System.Drawing.Point(10, 98)
        Me.m_rbCustomSCOR.Name = "m_rbCustomSCOR"
        Me.m_rbCustomSCOR.Size = New System.Drawing.Size(79, 17)
        Me.m_rbCustomSCOR.TabIndex = 8
        Me.m_rbCustomSCOR.TabStop = True
        Me.m_rbCustomSCOR.Text = "&Custom file:"
        Me.m_rbCustomSCOR.UseVisualStyleBackColor = True
        '
        'm_rbManagedSCOR
        '
        Me.m_rbManagedSCOR.AutoSize = True
        Me.m_rbManagedSCOR.Location = New System.Drawing.Point(10, 121)
        Me.m_rbManagedSCOR.Name = "m_rbManagedSCOR"
        Me.m_rbManagedSCOR.Size = New System.Drawing.Size(106, 17)
        Me.m_rbManagedSCOR.TabIndex = 11
        Me.m_rbManagedSCOR.TabStop = True
        Me.m_rbManagedSCOR.Text = "&System managed"
        Me.m_rbManagedSCOR.UseVisualStyleBackColor = True
        '
        'm_hdrSettings
        '
        Me.m_hdrSettings.CanCollapseParent = True
        Me.m_hdrSettings.CollapsedParentHeight = 76
        Me.m_hdrSettings.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdrSettings.IsCollapsed = False
        Me.m_hdrSettings.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrSettings.Name = "m_hdrSettings"
        Me.m_hdrSettings.Size = New System.Drawing.Size(410, 18)
        Me.m_hdrSettings.TabIndex = 0
        Me.m_hdrSettings.Text = "Settings"
        Me.m_hdrSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_plRun
        '
        Me.m_plRun.Controls.Add(Me.m_btnOK)
        Me.m_plRun.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plRun.Location = New System.Drawing.Point(0, 170)
        Me.m_plRun.Margin = New System.Windows.Forms.Padding(0)
        Me.m_plRun.Name = "m_plRun"
        Me.m_plRun.Size = New System.Drawing.Size(410, 29)
        Me.m_plRun.TabIndex = 1
        '
        'm_plResult
        '
        Me.m_plResult.Controls.Add(Me.m_tcDebug)
        Me.m_plResult.Controls.Add(Me.m_hdrResults)
        Me.m_plResult.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plResult.Location = New System.Drawing.Point(3, 202)
        Me.m_plResult.Name = "m_plResult"
        Me.m_plResult.Size = New System.Drawing.Size(404, 238)
        Me.m_plResult.TabIndex = 9
        '
        'm_tcDebug
        '
        Me.m_tcDebug.Controls.Add(Me.m_tpgScript)
        Me.m_tcDebug.Controls.Add(Me.m_tpgOutput)
        Me.m_tcDebug.Controls.Add(Me.m_tpgErrors)
        Me.m_tcDebug.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tcDebug.Location = New System.Drawing.Point(0, 18)
        Me.m_tcDebug.Name = "m_tcDebug"
        Me.m_tcDebug.SelectedIndex = 0
        Me.m_tcDebug.Size = New System.Drawing.Size(404, 220)
        Me.m_tcDebug.TabIndex = 1
        '
        'm_tpgScript
        '
        Me.m_tpgScript.Controls.Add(Me.m_lbxScript)
        Me.m_tpgScript.Location = New System.Drawing.Point(4, 22)
        Me.m_tpgScript.Name = "m_tpgScript"
        Me.m_tpgScript.Size = New System.Drawing.Size(396, 194)
        Me.m_tpgScript.TabIndex = 0
        Me.m_tpgScript.Text = "Script"
        Me.m_tpgScript.UseVisualStyleBackColor = True
        '
        'm_lbxScript
        '
        Me.m_lbxScript.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbxScript.FormattingEnabled = True
        Me.m_lbxScript.IntegralHeight = False
        Me.m_lbxScript.Location = New System.Drawing.Point(0, 0)
        Me.m_lbxScript.Name = "m_lbxScript"
        Me.m_lbxScript.Size = New System.Drawing.Size(396, 194)
        Me.m_lbxScript.TabIndex = 0
        '
        'm_tpgOutput
        '
        Me.m_tpgOutput.Controls.Add(Me.m_lbxOutput)
        Me.m_tpgOutput.Location = New System.Drawing.Point(4, 22)
        Me.m_tpgOutput.Name = "m_tpgOutput"
        Me.m_tpgOutput.Size = New System.Drawing.Size(396, 194)
        Me.m_tpgOutput.TabIndex = 1
        Me.m_tpgOutput.Text = "Output"
        Me.m_tpgOutput.UseVisualStyleBackColor = True
        '
        'm_lbxOutput
        '
        Me.m_lbxOutput.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbxOutput.FormattingEnabled = True
        Me.m_lbxOutput.IntegralHeight = False
        Me.m_lbxOutput.Location = New System.Drawing.Point(0, 0)
        Me.m_lbxOutput.Name = "m_lbxOutput"
        Me.m_lbxOutput.Size = New System.Drawing.Size(396, 194)
        Me.m_lbxOutput.TabIndex = 1
        '
        'm_tpgErrors
        '
        Me.m_tpgErrors.Controls.Add(Me.m_lbxError)
        Me.m_tpgErrors.Location = New System.Drawing.Point(4, 22)
        Me.m_tpgErrors.Name = "m_tpgErrors"
        Me.m_tpgErrors.Size = New System.Drawing.Size(396, 194)
        Me.m_tpgErrors.TabIndex = 2
        Me.m_tpgErrors.Text = "Errors"
        Me.m_tpgErrors.UseVisualStyleBackColor = True
        '
        'm_lbxError
        '
        Me.m_lbxError.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbxError.FormattingEnabled = True
        Me.m_lbxError.IntegralHeight = False
        Me.m_lbxError.Location = New System.Drawing.Point(0, 0)
        Me.m_lbxError.Name = "m_lbxError"
        Me.m_lbxError.Size = New System.Drawing.Size(396, 194)
        Me.m_lbxError.TabIndex = 1
        '
        'm_hdrResults
        '
        Me.m_hdrResults.CanCollapseParent = False
        Me.m_hdrResults.CollapsedParentHeight = 0
        Me.m_hdrResults.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdrResults.IsCollapsed = False
        Me.m_hdrResults.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrResults.Name = "m_hdrResults"
        Me.m_hdrResults.Size = New System.Drawing.Size(404, 18)
        Me.m_hdrResults.TabIndex = 0
        Me.m_hdrResults.Text = "Results"
        Me.m_hdrResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmInvokeR
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(434, 467)
        Me.Controls.Add(Me.m_tlpContent)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmInvokeR"
        Me.Text = "Invoke R NETWRK"
        Me.m_tlpContent.ResumeLayout(False)
        Me.m_plOptions.ResumeLayout(False)
        Me.m_plOptions.PerformLayout()
        Me.m_plRun.ResumeLayout(False)
        Me.m_plResult.ResumeLayout(False)
        Me.m_tcDebug.ResumeLayout(False)
        Me.m_tpgScript.ResumeLayout(False)
        Me.m_tpgOutput.ResumeLayout(False)
        Me.m_tpgErrors.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_tbxR As System.Windows.Forms.TextBox
    Private WithEvents m_btnChooseR As System.Windows.Forms.Button
    Private WithEvents m_btnChooseScript As System.Windows.Forms.Button
    Private WithEvents m_tbxSCOR As System.Windows.Forms.TextBox
    Private WithEvents m_btnChooseSCOR As System.Windows.Forms.Button
    Private WithEvents m_tbxScript As System.Windows.Forms.TextBox
    Private WithEvents m_lblScript As System.Windows.Forms.Label
    Private WithEvents m_lblR As System.Windows.Forms.Label
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_tbxPlaceholder As System.Windows.Forms.TextBox
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
    Private WithEvents m_lbxScript As System.Windows.Forms.ListBox
    Private WithEvents m_tpgOutput As System.Windows.Forms.TabPage
    Private WithEvents m_lbxOutput As System.Windows.Forms.ListBox
    Private WithEvents m_tpgErrors As System.Windows.Forms.TabPage
    Private WithEvents m_lbxError As System.Windows.Forms.ListBox
    Private WithEvents m_hdrResults As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_rbManagedSCOR As System.Windows.Forms.RadioButton
End Class
