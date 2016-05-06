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
' EwE Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' GOTMLink plug-in Copyright 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormGotmPluggin
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
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.m_tbxLinkFile = New System.Windows.Forms.TextBox()
        Me.m_btnChooseLinkFile = New System.Windows.Forms.Button()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.m_tbxEwEStatus = New System.Windows.Forms.TextBox()
        Me.m_tbxGOTMStatus = New System.Windows.Forms.TextBox()
        Me.m_lblGOTMStatus = New System.Windows.Forms.Label()
        Me.m_lblEwEStatus = New System.Windows.Forms.Label()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.RadioButton1 = New System.Windows.Forms.RadioButton()
        Me.RadioButton2 = New System.Windows.Forms.RadioButton()
        Me.TextBox6 = New System.Windows.Forms.TextBox()
        Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpDown3 = New System.Windows.Forms.NumericUpDown()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.m_lblLinkFile = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TextBox12 = New System.Windows.Forms.TextBox()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.CheckBox2 = New System.Windows.Forms.CheckBox()
        Me.DomainUpDown1 = New System.Windows.Forms.DomainUpDown()
        Me.DateTimePicker1 = New System.Windows.Forms.DateTimePicker()
        Me.DateTimePicker2 = New System.Windows.Forms.DateTimePicker()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 66)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(146, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Load Scenario"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'm_tbxLinkFile
        '
        Me.m_tbxLinkFile.Location = New System.Drawing.Point(91, 32)
        Me.m_tbxLinkFile.Name = "m_tbxLinkFile"
        Me.m_tbxLinkFile.Size = New System.Drawing.Size(246, 20)
        Me.m_tbxLinkFile.TabIndex = 1
        '
        'm_btnChooseLinkFile
        '
        Me.m_btnChooseLinkFile.Location = New System.Drawing.Point(343, 30)
        Me.m_btnChooseLinkFile.Name = "m_btnChooseLinkFile"
        Me.m_btnChooseLinkFile.Size = New System.Drawing.Size(75, 23)
        Me.m_btnChooseLinkFile.TabIndex = 2
        Me.m_btnChooseLinkFile.Text = "Choose..."
        Me.m_btnChooseLinkFile.UseVisualStyleBackColor = True
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(12, 211)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(405, 20)
        Me.TextBox2.TabIndex = 5
        '
        'm_tbxEwEStatus
        '
        Me.m_tbxEwEStatus.Location = New System.Drawing.Point(277, 6)
        Me.m_tbxEwEStatus.Name = "m_tbxEwEStatus"
        Me.m_tbxEwEStatus.ReadOnly = True
        Me.m_tbxEwEStatus.Size = New System.Drawing.Size(141, 20)
        Me.m_tbxEwEStatus.TabIndex = 6
        '
        'm_tbxGOTMStatus
        '
        Me.m_tbxGOTMStatus.Location = New System.Drawing.Point(91, 6)
        Me.m_tbxGOTMStatus.Name = "m_tbxGOTMStatus"
        Me.m_tbxGOTMStatus.ReadOnly = True
        Me.m_tbxGOTMStatus.Size = New System.Drawing.Size(112, 20)
        Me.m_tbxGOTMStatus.TabIndex = 7
        '
        'm_lblGOTMStatus
        '
        Me.m_lblGOTMStatus.AutoSize = True
        Me.m_lblGOTMStatus.Location = New System.Drawing.Point(12, 9)
        Me.m_lblGOTMStatus.Name = "m_lblGOTMStatus"
        Me.m_lblGOTMStatus.Size = New System.Drawing.Size(73, 13)
        Me.m_lblGOTMStatus.TabIndex = 8
        Me.m_lblGOTMStatus.Text = "GOTM status:"
        '
        'm_lblEwEStatus
        '
        Me.m_lblEwEStatus.AutoSize = True
        Me.m_lblEwEStatus.Location = New System.Drawing.Point(209, 9)
        Me.m_lblEwEStatus.Name = "m_lblEwEStatus"
        Me.m_lblEwEStatus.Size = New System.Drawing.Size(63, 13)
        Me.m_lblEwEStatus.TabIndex = 9
        Me.m_lblEwEStatus.Text = "EwE status:"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(12, 95)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(146, 23)
        Me.Button3.TabIndex = 10
        Me.Button3.Text = "Edit Scenario"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(12, 124)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(146, 23)
        Me.Button4.TabIndex = 11
        Me.Button4.Text = "Run Scenario"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(12, 153)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(146, 23)
        Me.Button5.TabIndex = 12
        Me.Button5.Text = "View Results"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.Location = New System.Drawing.Point(190, 92)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
        Me.NumericUpDown1.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(61, 20)
        Me.NumericUpDown1.TabIndex = 15
        Me.NumericUpDown1.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Checked = True
        Me.RadioButton1.Location = New System.Drawing.Point(190, 118)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(113, 17)
        Me.RadioButton1.TabIndex = 16
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "Managed coupling"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'RadioButton2
        '
        Me.RadioButton2.AutoSize = True
        Me.RadioButton2.Location = New System.Drawing.Point(190, 141)
        Me.RadioButton2.Name = "RadioButton2"
        Me.RadioButton2.Size = New System.Drawing.Size(108, 17)
        Me.RadioButton2.TabIndex = 17
        Me.RadioButton2.Text = "Network coupling"
        Me.RadioButton2.UseVisualStyleBackColor = True
        '
        'TextBox6
        '
        Me.TextBox6.Location = New System.Drawing.Point(13, 289)
        Me.TextBox6.Multiline = True
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(405, 319)
        Me.TextBox6.TabIndex = 18
        '
        'NumericUpDown2
        '
        Me.NumericUpDown2.Location = New System.Drawing.Point(283, 92)
        Me.NumericUpDown2.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown2.Name = "NumericUpDown2"
        Me.NumericUpDown2.Size = New System.Drawing.Size(54, 20)
        Me.NumericUpDown2.TabIndex = 19
        '
        'NumericUpDown3
        '
        Me.NumericUpDown3.Location = New System.Drawing.Point(357, 92)
        Me.NumericUpDown3.Maximum = New Decimal(New Integer() {11, 0, 0, 0})
        Me.NumericUpDown3.Name = "NumericUpDown3"
        Me.NumericUpDown3.Size = New System.Drawing.Size(55, 20)
        Me.NumericUpDown3.TabIndex = 20
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(370, 71)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(42, 13)
        Me.Label3.TabIndex = 21
        Me.Label3.Text = "Months"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(303, 71)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(34, 13)
        Me.Label4.TabIndex = 22
        Me.Label4.Text = "Years"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(343, 58)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(59, 13)
        Me.Label5.TabIndex = 23
        Me.Label5.Text = "Delay Start"
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(12, 182)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(146, 23)
        Me.Button6.TabIndex = 24
        Me.Button6.Text = "Reset"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label6.Location = New System.Drawing.Point(187, 58)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(78, 13)
        Me.Label6.TabIndex = 25
        Me.Label6.Text = "Timestep Ratio"
        '
        'm_lblLinkFile
        '
        Me.m_lblLinkFile.AutoSize = True
        Me.m_lblLinkFile.Location = New System.Drawing.Point(12, 35)
        Me.m_lblLinkFile.Name = "m_lblLinkFile"
        Me.m_lblLinkFile.Size = New System.Drawing.Size(46, 13)
        Me.m_lblLinkFile.TabIndex = 26
        Me.m_lblLinkFile.Text = "Link file:"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Enabled = False
        Me.Label12.Location = New System.Drawing.Point(319, 164)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(61, 13)
        Me.Label12.TabIndex = 40
        Me.Label12.Text = "NetCDFFile"
        '
        'TextBox12
        '
        Me.TextBox12.Enabled = False
        Me.TextBox12.Location = New System.Drawing.Point(164, 184)
        Me.TextBox12.Name = "TextBox12"
        Me.TextBox12.Size = New System.Drawing.Size(253, 20)
        Me.TextBox12.TabIndex = 41
        Me.TextBox12.Text = "e:/ersemewecoupler/testdata/PML.NC"
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(321, 139)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(58, 17)
        Me.CheckBox1.TabIndex = 42
        Me.CheckBox1.Text = "Spatial"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Checked = True
        Me.CheckBox2.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox2.Location = New System.Drawing.Point(190, 160)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(71, 17)
        Me.CheckBox2.TabIndex = 44
        Me.CheckBox2.Text = "3D model"
        Me.CheckBox2.UseVisualStyleBackColor = True
        '
        'DomainUpDown1
        '
        Me.DomainUpDown1.Location = New System.Drawing.Point(12, 237)
        Me.DomainUpDown1.Name = "DomainUpDown1"
        Me.DomainUpDown1.Size = New System.Drawing.Size(120, 20)
        Me.DomainUpDown1.TabIndex = 45
        Me.DomainUpDown1.Text = "Connection"
        '
        'DateTimePicker1
        '
        Me.DateTimePicker1.Location = New System.Drawing.Point(12, 263)
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Size = New System.Drawing.Size(191, 20)
        Me.DateTimePicker1.TabIndex = 46
        '
        'DateTimePicker2
        '
        Me.DateTimePicker2.Location = New System.Drawing.Point(241, 263)
        Me.DateTimePicker2.Name = "DateTimePicker2"
        Me.DateTimePicker2.Size = New System.Drawing.Size(177, 20)
        Me.DateTimePicker2.TabIndex = 47
        '
        'FormGotmPluggin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(429, 620)
        Me.Controls.Add(Me.DateTimePicker2)
        Me.Controls.Add(Me.DateTimePicker1)
        Me.Controls.Add(Me.DomainUpDown1)
        Me.Controls.Add(Me.CheckBox2)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.TextBox12)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.m_lblLinkFile)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.NumericUpDown3)
        Me.Controls.Add(Me.NumericUpDown2)
        Me.Controls.Add(Me.TextBox6)
        Me.Controls.Add(Me.RadioButton2)
        Me.Controls.Add(Me.RadioButton1)
        Me.Controls.Add(Me.NumericUpDown1)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.m_lblEwEStatus)
        Me.Controls.Add(Me.m_lblGOTMStatus)
        Me.Controls.Add(Me.m_tbxGOTMStatus)
        Me.Controls.Add(Me.m_tbxEwEStatus)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.m_btnChooseLinkFile)
        Me.Controls.Add(Me.m_tbxLinkFile)
        Me.Controls.Add(Me.Button1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "FormGotmPluggin"
        Me.Text = "GOTM Launcher"
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents TextBox6 As System.Windows.Forms.TextBox
    Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
    Friend WithEvents NumericUpDown3 As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents TextBox12 As System.Windows.Forms.TextBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
    Friend WithEvents DomainUpDown1 As System.Windows.Forms.DomainUpDown
    Friend WithEvents DateTimePicker1 As System.Windows.Forms.DateTimePicker
    Friend WithEvents DateTimePicker2 As System.Windows.Forms.DateTimePicker
    Private WithEvents m_lblGOTMStatus As System.Windows.Forms.Label
    Private WithEvents m_tbxGOTMStatus As System.Windows.Forms.TextBox
    Private WithEvents m_lblEwEStatus As System.Windows.Forms.Label
    Private WithEvents m_tbxEwEStatus As System.Windows.Forms.TextBox
    Private WithEvents m_lblLinkFile As System.Windows.Forms.Label
    Private WithEvents m_tbxLinkFile As System.Windows.Forms.TextBox
    Private WithEvents m_btnChooseLinkFile As System.Windows.Forms.Button
End Class
