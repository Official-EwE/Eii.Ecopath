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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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
        Me.m_lblSCOR = New System.Windows.Forms.Label()
        Me.m_tbxSCOR = New System.Windows.Forms.TextBox()
        Me.m_btnChooseSCOR = New System.Windows.Forms.Button()
        Me.m_tbxPlaceholder = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_tcDebug = New System.Windows.Forms.TabControl()
        Me.m_tpgScript = New System.Windows.Forms.TabPage()
        Me.m_lbxScript = New System.Windows.Forms.ListBox()
        Me.m_tpgOutput = New System.Windows.Forms.TabPage()
        Me.m_lbxOutput = New System.Windows.Forms.ListBox()
        Me.m_tpgErrors = New System.Windows.Forms.TabPage()
        Me.m_lbxError = New System.Windows.Forms.ListBox()
        Me.m_tcDebug.SuspendLayout()
        Me.m_tpgScript.SuspendLayout()
        Me.m_tpgOutput.SuspendLayout()
        Me.m_tpgErrors.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblR
        '
        Me.m_lblR.AutoSize = True
        Me.m_lblR.Location = New System.Drawing.Point(12, 17)
        Me.m_lblR.Name = "m_lblR"
        Me.m_lblR.Size = New System.Drawing.Size(59, 13)
        Me.m_lblR.TabIndex = 0
        Me.m_lblR.Text = "R &program:"
        '
        'm_tbxR
        '
        Me.m_tbxR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxR.Location = New System.Drawing.Point(131, 14)
        Me.m_tbxR.Name = "m_tbxR"
        Me.m_tbxR.Size = New System.Drawing.Size(215, 20)
        Me.m_tbxR.TabIndex = 1
        '
        'm_btnChooseR
        '
        Me.m_btnChooseR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseR.Location = New System.Drawing.Point(352, 12)
        Me.m_btnChooseR.Name = "m_btnChooseR"
        Me.m_btnChooseR.Size = New System.Drawing.Size(70, 23)
        Me.m_btnChooseR.TabIndex = 2
        Me.m_btnChooseR.Text = "Choose..."
        Me.m_btnChooseR.UseVisualStyleBackColor = True
        '
        'm_lblScript
        '
        Me.m_lblScript.AutoSize = True
        Me.m_lblScript.Location = New System.Drawing.Point(12, 46)
        Me.m_lblScript.Name = "m_lblScript"
        Me.m_lblScript.Size = New System.Drawing.Size(46, 13)
        Me.m_lblScript.TabIndex = 3
        Me.m_lblScript.Text = "R &script:"
        '
        'm_tbxScript
        '
        Me.m_tbxScript.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxScript.Location = New System.Drawing.Point(131, 43)
        Me.m_tbxScript.Name = "m_tbxScript"
        Me.m_tbxScript.Size = New System.Drawing.Size(215, 20)
        Me.m_tbxScript.TabIndex = 4
        '
        'm_btnChooseScript
        '
        Me.m_btnChooseScript.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseScript.Location = New System.Drawing.Point(352, 41)
        Me.m_btnChooseScript.Name = "m_btnChooseScript"
        Me.m_btnChooseScript.Size = New System.Drawing.Size(70, 23)
        Me.m_btnChooseScript.TabIndex = 5
        Me.m_btnChooseScript.Text = "Choose..."
        Me.m_btnChooseScript.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        Me.m_btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnOK.Location = New System.Drawing.Point(352, 122)
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.Size = New System.Drawing.Size(70, 23)
        Me.m_btnOK.TabIndex = 6
        Me.m_btnOK.Text = "Run"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_lblSCOR
        '
        Me.m_lblSCOR.AutoSize = True
        Me.m_lblSCOR.Location = New System.Drawing.Point(12, 98)
        Me.m_lblSCOR.Name = "m_lblSCOR"
        Me.m_lblSCOR.Size = New System.Drawing.Size(56, 13)
        Me.m_lblSCOR.TabIndex = 3
        Me.m_lblSCOR.Text = "SCOR file:"
        '
        'm_tbxSCOR
        '
        Me.m_tbxSCOR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxSCOR.Location = New System.Drawing.Point(131, 96)
        Me.m_tbxSCOR.Name = "m_tbxSCOR"
        Me.m_tbxSCOR.Size = New System.Drawing.Size(215, 20)
        Me.m_tbxSCOR.TabIndex = 4
        '
        'm_btnChooseSCOR
        '
        Me.m_btnChooseSCOR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseSCOR.Location = New System.Drawing.Point(352, 93)
        Me.m_btnChooseSCOR.Name = "m_btnChooseSCOR"
        Me.m_btnChooseSCOR.Size = New System.Drawing.Size(70, 23)
        Me.m_btnChooseSCOR.TabIndex = 5
        Me.m_btnChooseSCOR.Text = "Choose..."
        Me.m_btnChooseSCOR.UseVisualStyleBackColor = True
        '
        'm_tbxPlaceholder
        '
        Me.m_tbxPlaceholder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxPlaceholder.Location = New System.Drawing.Point(131, 69)
        Me.m_tbxPlaceholder.Name = "m_tbxPlaceholder"
        Me.m_tbxPlaceholder.Size = New System.Drawing.Size(215, 20)
        Me.m_tbxPlaceholder.TabIndex = 4
        Me.m_tbxPlaceholder.Text = "%FILENAME%"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 72)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(114, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "SCOR file placeholder:"
        '
        'm_tcDebug
        '
        Me.m_tcDebug.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tcDebug.Controls.Add(Me.m_tpgScript)
        Me.m_tcDebug.Controls.Add(Me.m_tpgOutput)
        Me.m_tcDebug.Controls.Add(Me.m_tpgErrors)
        Me.m_tcDebug.Location = New System.Drawing.Point(12, 151)
        Me.m_tcDebug.Name = "m_tcDebug"
        Me.m_tcDebug.SelectedIndex = 0
        Me.m_tcDebug.Size = New System.Drawing.Size(410, 304)
        Me.m_tcDebug.TabIndex = 7
        '
        'm_tpgScript
        '
        Me.m_tpgScript.Controls.Add(Me.m_lbxScript)
        Me.m_tpgScript.Location = New System.Drawing.Point(4, 22)
        Me.m_tpgScript.Name = "m_tpgScript"
        Me.m_tpgScript.Size = New System.Drawing.Size(402, 278)
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
        Me.m_lbxScript.Size = New System.Drawing.Size(402, 278)
        Me.m_lbxScript.TabIndex = 0
        '
        'm_tpgOutput
        '
        Me.m_tpgOutput.Controls.Add(Me.m_lbxOutput)
        Me.m_tpgOutput.Location = New System.Drawing.Point(4, 22)
        Me.m_tpgOutput.Name = "m_tpgOutput"
        Me.m_tpgOutput.Size = New System.Drawing.Size(402, 278)
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
        Me.m_lbxOutput.Size = New System.Drawing.Size(402, 278)
        Me.m_lbxOutput.TabIndex = 1
        '
        'm_tpgErrors
        '
        Me.m_tpgErrors.Controls.Add(Me.m_lbxError)
        Me.m_tpgErrors.Location = New System.Drawing.Point(4, 22)
        Me.m_tpgErrors.Name = "m_tpgErrors"
        Me.m_tpgErrors.Size = New System.Drawing.Size(402, 278)
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
        Me.m_lbxError.Size = New System.Drawing.Size(402, 278)
        Me.m_lbxError.TabIndex = 1
        '
        'frmInvokeR
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(434, 467)
        Me.Controls.Add(Me.m_tcDebug)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_btnChooseSCOR)
        Me.Controls.Add(Me.m_btnChooseScript)
        Me.Controls.Add(Me.m_btnChooseR)
        Me.Controls.Add(Me.m_tbxSCOR)
        Me.Controls.Add(Me.m_tbxPlaceholder)
        Me.Controls.Add(Me.m_tbxScript)
        Me.Controls.Add(Me.m_lblSCOR)
        Me.Controls.Add(Me.m_tbxR)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_lblScript)
        Me.Controls.Add(Me.m_lblR)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmInvokeR"
        Me.Text = "Invoke R"
        Me.m_tcDebug.ResumeLayout(False)
        Me.m_tpgScript.ResumeLayout(False)
        Me.m_tpgOutput.ResumeLayout(False)
        Me.m_tpgErrors.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_tbxR As System.Windows.Forms.TextBox
    Private WithEvents m_btnChooseR As System.Windows.Forms.Button
    Private WithEvents m_btnChooseScript As System.Windows.Forms.Button
    Private WithEvents m_tbxSCOR As System.Windows.Forms.TextBox
    Private WithEvents m_btnChooseSCOR As System.Windows.Forms.Button
    Private WithEvents m_tbxScript As System.Windows.Forms.TextBox
    Private WithEvents m_lblSCOR As System.Windows.Forms.Label
    Private WithEvents m_lblScript As System.Windows.Forms.Label
    Private WithEvents m_lblR As System.Windows.Forms.Label
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_tbxPlaceholder As System.Windows.Forms.TextBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents m_tcDebug As System.Windows.Forms.TabControl
    Private WithEvents m_tpgScript As System.Windows.Forms.TabPage
    Private WithEvents m_lbxScript As System.Windows.Forms.ListBox
    Private WithEvents m_tpgOutput As System.Windows.Forms.TabPage
    Private WithEvents m_lbxOutput As System.Windows.Forms.ListBox
    Private WithEvents m_tpgErrors As System.Windows.Forms.TabPage
    Private WithEvents m_lbxError As System.Windows.Forms.ListBox
End Class
