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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class dlgImportTraits
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.m_tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.m_hdrSource = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tbImportSeparator = New ScientificInterfaceShared.Controls.ucCharacterTextBox()
        Me.m_tbImportDelimiter = New ScientificInterfaceShared.Controls.ucCharacterTextBox()
        Me.m_tbImportFileName = New System.Windows.Forms.TextBox()
        Me.m_lblImportDecimalSeparator = New System.Windows.Forms.Label()
        Me.m_lblImportDelimiter = New System.Windows.Forms.Label()
        Me.m_rbImportSourceClipboard = New System.Windows.Forms.RadioButton()
        Me.m_btnImportBrowse = New System.Windows.Forms.Button()
        Me.m_rbImportSourceTextFile = New System.Windows.Forms.RadioButton()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.m_plOK = New System.Windows.Forms.Panel()
        Me.m_btnOk = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_tlpMain.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_plOK.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tlpMain
        '
        Me.m_tlpMain.ColumnCount = 1
        Me.m_tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpMain.Controls.Add(Me.Panel1, 0, 0)
        Me.m_tlpMain.Controls.Add(Me.DataGridView1, 0, 1)
        Me.m_tlpMain.Controls.Add(Me.m_plOK, 0, 2)
        Me.m_tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.m_tlpMain.Name = "m_tlpMain"
        Me.m_tlpMain.RowCount = 3
        Me.m_tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.00932!))
        Me.m_tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75.99068!))
        Me.m_tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.m_tlpMain.Size = New System.Drawing.Size(800, 450)
        Me.m_tlpMain.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.m_hdrSource)
        Me.Panel1.Controls.Add(Me.m_tbImportSeparator)
        Me.Panel1.Controls.Add(Me.m_tbImportDelimiter)
        Me.Panel1.Controls.Add(Me.m_tbImportFileName)
        Me.Panel1.Controls.Add(Me.m_lblImportDecimalSeparator)
        Me.Panel1.Controls.Add(Me.m_lblImportDelimiter)
        Me.Panel1.Controls.Add(Me.m_rbImportSourceClipboard)
        Me.Panel1.Controls.Add(Me.m_btnImportBrowse)
        Me.Panel1.Controls.Add(Me.m_rbImportSourceTextFile)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(794, 95)
        Me.Panel1.TabIndex = 0
        '
        'm_hdrSource
        '
        Me.m_hdrSource.CanCollapseParent = False
        Me.m_hdrSource.CollapsedParentHeight = 0
        Me.m_hdrSource.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdrSource.IsCollapsed = False
        Me.m_hdrSource.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrSource.Name = "m_hdrSource"
        Me.m_hdrSource.Size = New System.Drawing.Size(794, 18)
        Me.m_hdrSource.TabIndex = 0
        Me.m_hdrSource.Text = "Source"
        Me.m_hdrSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tbImportSeparator
        '
        Me.m_tbImportSeparator.AcceptsReturn = True
        Me.m_tbImportSeparator.AcceptsTab = True
        Me.m_tbImportSeparator.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbImportSeparator.Character = Global.Microsoft.VisualBasic.ChrW(46)
        Me.m_tbImportSeparator.CharacterMask = ""
        Me.m_tbImportSeparator.CharCode = 46
        Me.m_tbImportSeparator.Location = New System.Drawing.Point(338, 74)
        Me.m_tbImportSeparator.MaskInclusive = False
        Me.m_tbImportSeparator.Multiline = True
        Me.m_tbImportSeparator.Name = "m_tbImportSeparator"
        Me.m_tbImportSeparator.ShortcutsEnabled = False
        Me.m_tbImportSeparator.Size = New System.Drawing.Size(90, 20)
        Me.m_tbImportSeparator.TabIndex = 8
        Me.m_tbImportSeparator.Text = ". (period)"
        '
        'm_tbImportDelimiter
        '
        Me.m_tbImportDelimiter.AcceptsReturn = True
        Me.m_tbImportDelimiter.AcceptsTab = True
        Me.m_tbImportDelimiter.Character = Global.Microsoft.VisualBasic.ChrW(44)
        Me.m_tbImportDelimiter.CharacterMask = ""
        Me.m_tbImportDelimiter.CharCode = 44
        Me.m_tbImportDelimiter.Location = New System.Drawing.Point(83, 74)
        Me.m_tbImportDelimiter.MaskInclusive = False
        Me.m_tbImportDelimiter.MaxLength = 10
        Me.m_tbImportDelimiter.Multiline = True
        Me.m_tbImportDelimiter.Name = "m_tbImportDelimiter"
        Me.m_tbImportDelimiter.ShortcutsEnabled = False
        Me.m_tbImportDelimiter.Size = New System.Drawing.Size(90, 20)
        Me.m_tbImportDelimiter.TabIndex = 6
        Me.m_tbImportDelimiter.Text = ", (comma)"
        '
        'm_tbImportFileName
        '
        Me.m_tbImportFileName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbImportFileName.Location = New System.Drawing.Point(84, 27)
        Me.m_tbImportFileName.Name = "m_tbImportFileName"
        Me.m_tbImportFileName.Size = New System.Drawing.Size(620, 20)
        Me.m_tbImportFileName.TabIndex = 2
        '
        'm_lblImportDecimalSeparator
        '
        Me.m_lblImportDecimalSeparator.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lblImportDecimalSeparator.AutoSize = True
        Me.m_lblImportDecimalSeparator.Location = New System.Drawing.Point(237, 77)
        Me.m_lblImportDecimalSeparator.Name = "m_lblImportDecimalSeparator"
        Me.m_lblImportDecimalSeparator.Size = New System.Drawing.Size(95, 13)
        Me.m_lblImportDecimalSeparator.TabIndex = 7
        Me.m_lblImportDecimalSeparator.Text = "D&ecimal separator:"
        '
        'm_lblImportDelimiter
        '
        Me.m_lblImportDelimiter.AutoSize = True
        Me.m_lblImportDelimiter.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblImportDelimiter.Location = New System.Drawing.Point(12, 77)
        Me.m_lblImportDelimiter.Name = "m_lblImportDelimiter"
        Me.m_lblImportDelimiter.Size = New System.Drawing.Size(50, 13)
        Me.m_lblImportDelimiter.TabIndex = 5
        Me.m_lblImportDelimiter.Text = "&Delimiter:"
        '
        'm_rbImportSourceClipboard
        '
        Me.m_rbImportSourceClipboard.AutoSize = True
        Me.m_rbImportSourceClipboard.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_rbImportSourceClipboard.Location = New System.Drawing.Point(15, 51)
        Me.m_rbImportSourceClipboard.Name = "m_rbImportSourceClipboard"
        Me.m_rbImportSourceClipboard.Size = New System.Drawing.Size(69, 17)
        Me.m_rbImportSourceClipboard.TabIndex = 4
        Me.m_rbImportSourceClipboard.Text = "&Clipboard"
        Me.m_rbImportSourceClipboard.UseVisualStyleBackColor = True
        '
        'm_btnImportBrowse
        '
        Me.m_btnImportBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnImportBrowse.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_btnImportBrowse.Location = New System.Drawing.Point(710, 25)
        Me.m_btnImportBrowse.Name = "m_btnImportBrowse"
        Me.m_btnImportBrowse.Size = New System.Drawing.Size(75, 23)
        Me.m_btnImportBrowse.TabIndex = 3
        Me.m_btnImportBrowse.Text = "&Browse..."
        Me.m_btnImportBrowse.UseVisualStyleBackColor = True
        '
        'm_rbImportSourceTextFile
        '
        Me.m_rbImportSourceTextFile.AutoSize = True
        Me.m_rbImportSourceTextFile.Checked = True
        Me.m_rbImportSourceTextFile.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_rbImportSourceTextFile.Location = New System.Drawing.Point(15, 28)
        Me.m_rbImportSourceTextFile.Name = "m_rbImportSourceTextFile"
        Me.m_rbImportSourceTextFile.Size = New System.Drawing.Size(65, 17)
        Me.m_rbImportSourceTextFile.TabIndex = 1
        Me.m_rbImportSourceTextFile.TabStop = True
        Me.m_rbImportSourceTextFile.Text = "Text &file:"
        Me.m_rbImportSourceTextFile.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(3, 104)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.Size = New System.Drawing.Size(794, 314)
        Me.DataGridView1.TabIndex = 0
        '
        'm_plOK
        '
        Me.m_plOK.Controls.Add(Me.m_btnCancel)
        Me.m_plOK.Controls.Add(Me.m_btnOk)
        Me.m_plOK.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plOK.Location = New System.Drawing.Point(3, 424)
        Me.m_plOK.Name = "m_plOK"
        Me.m_plOK.Size = New System.Drawing.Size(794, 23)
        Me.m_plOK.TabIndex = 1
        '
        'm_btnOk
        '
        Me.m_btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnOk.Location = New System.Drawing.Point(638, 0)
        Me.m_btnOk.Name = "m_btnOk"
        Me.m_btnOk.Size = New System.Drawing.Size(75, 23)
        Me.m_btnOk.TabIndex = 0
        Me.m_btnOk.Text = "OK"
        Me.m_btnOk.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        Me.m_btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnCancel.Location = New System.Drawing.Point(716, 0)
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.m_btnCancel.TabIndex = 1
        Me.m_btnCancel.Text = "Cancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'dlgImportTraits
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.m_tlpMain)
        Me.MinimizeBox = False
        Me.Name = "dlgImportTraits"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Import traits"
        Me.m_tlpMain.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_plOK.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As Panel
    Private WithEvents m_hdrSource As cEwEHeaderLabel
    Private WithEvents m_tbImportSeparator As ucCharacterTextBox
    Private WithEvents m_tbImportDelimiter As ucCharacterTextBox
    Private WithEvents m_tbImportFileName As TextBox
    Private WithEvents m_lblImportDecimalSeparator As Label
    Private WithEvents m_lblImportDelimiter As Label
    Private WithEvents m_rbImportSourceClipboard As RadioButton
    Private WithEvents m_btnImportBrowse As Button
    Private WithEvents m_rbImportSourceTextFile As RadioButton
    Private WithEvents m_tlpMain As TableLayoutPanel
    Private WithEvents DataGridView1 As DataGridView
    Private WithEvents m_plOK As Panel
    Private WithEvents m_btnCancel As Button
    Private WithEvents m_btnOk As Button
End Class
