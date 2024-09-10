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

Partial Class ucOptionsPluginAssemblyDetails
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Me.m_lblCopyright = New System.Windows.Forms.Label()
        Me.m_lblCompany = New System.Windows.Forms.Label()
        Me.m_lblVersion = New System.Windows.Forms.Label()
        Me.m_lblFile = New System.Windows.Forms.Label()
        Me.m_tbFile = New System.Windows.Forms.TextBox()
        Me.m_tbVersion = New System.Windows.Forms.TextBox()
        Me.m_tbCompany = New System.Windows.Forms.TextBox()
        Me.m_tbCopyright = New System.Windows.Forms.TextBox()
        Me.m_tbDescription = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblDescription = New System.Windows.Forms.Label()
        Me.m_lbLicense = New System.Windows.Forms.Label()
        Me.m_tbxLicense = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblCopyright
        '
        Me.m_lblCopyright.AutoSize = True
        Me.m_lblCopyright.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblCopyright.Location = New System.Drawing.Point(3, 81)
        Me.m_lblCopyright.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblCopyright.Name = "m_lblCopyright"
        Me.m_lblCopyright.Size = New System.Drawing.Size(63, 13)
        Me.m_lblCopyright.TabIndex = 6
        Me.m_lblCopyright.Text = "Copyright:"
        Me.m_lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblCompany
        '
        Me.m_lblCompany.AutoSize = True
        Me.m_lblCompany.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblCompany.Location = New System.Drawing.Point(3, 56)
        Me.m_lblCompany.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblCompany.Name = "m_lblCompany"
        Me.m_lblCompany.Size = New System.Drawing.Size(63, 13)
        Me.m_lblCompany.TabIndex = 4
        Me.m_lblCompany.Text = "Company:"
        Me.m_lblCompany.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblVersion
        '
        Me.m_lblVersion.AutoSize = True
        Me.m_lblVersion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblVersion.Location = New System.Drawing.Point(3, 31)
        Me.m_lblVersion.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblVersion.Name = "m_lblVersion"
        Me.m_lblVersion.Size = New System.Drawing.Size(63, 13)
        Me.m_lblVersion.TabIndex = 2
        Me.m_lblVersion.Text = "Version:"
        Me.m_lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblFile
        '
        Me.m_lblFile.AutoSize = True
        Me.m_lblFile.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblFile.Location = New System.Drawing.Point(3, 6)
        Me.m_lblFile.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblFile.Name = "m_lblFile"
        Me.m_lblFile.Size = New System.Drawing.Size(63, 13)
        Me.m_lblFile.TabIndex = 0
        Me.m_lblFile.Text = "File:"
        Me.m_lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tbFile
        '
        Me.m_tbFile.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbFile.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbFile.Location = New System.Drawing.Point(72, 6)
        Me.m_tbFile.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbFile.Name = "m_tbFile"
        Me.m_tbFile.ReadOnly = True
        Me.m_tbFile.Size = New System.Drawing.Size(368, 13)
        Me.m_tbFile.TabIndex = 1
        Me.m_tbFile.TabStop = False
        '
        'm_tbVersion
        '
        Me.m_tbVersion.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbVersion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbVersion.Location = New System.Drawing.Point(72, 31)
        Me.m_tbVersion.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbVersion.Name = "m_tbVersion"
        Me.m_tbVersion.ReadOnly = True
        Me.m_tbVersion.Size = New System.Drawing.Size(368, 13)
        Me.m_tbVersion.TabIndex = 3
        Me.m_tbVersion.TabStop = False
        Me.m_tbVersion.Text = "1.2.3.4"
        '
        'm_tbCompany
        '
        Me.m_tbCompany.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbCompany.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbCompany.Location = New System.Drawing.Point(72, 56)
        Me.m_tbCompany.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbCompany.Name = "m_tbCompany"
        Me.m_tbCompany.ReadOnly = True
        Me.m_tbCompany.Size = New System.Drawing.Size(368, 13)
        Me.m_tbCompany.TabIndex = 5
        Me.m_tbCompany.TabStop = False
        '
        'm_tbCopyright
        '
        Me.m_tbCopyright.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbCopyright.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbCopyright.Location = New System.Drawing.Point(72, 81)
        Me.m_tbCopyright.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbCopyright.Name = "m_tbCopyright"
        Me.m_tbCopyright.ReadOnly = True
        Me.m_tbCopyright.Size = New System.Drawing.Size(368, 13)
        Me.m_tbCopyright.TabIndex = 7
        Me.m_tbCopyright.TabStop = False
        '
        'm_tbDescription
        '
        Me.m_tbDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbDescription.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbDescription.Location = New System.Drawing.Point(72, 131)
        Me.m_tbDescription.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbDescription.Multiline = True
        Me.m_tbDescription.Name = "m_tbDescription"
        Me.m_tbDescription.ReadOnly = True
        Me.m_tbDescription.Size = New System.Drawing.Size(368, 124)
        Me.m_tbDescription.TabIndex = 11
        Me.m_tbDescription.TabStop = False
        Me.m_tbDescription.Text = "Description"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblDescription, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblFile, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbDescription, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbFile, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbCopyright, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblVersion, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbCompany, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbVersion, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblCopyright, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblCompany, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbLicense, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbxLicense, 1, 4)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(443, 261)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'm_lblDescription
        '
        Me.m_lblDescription.AutoSize = True
        Me.m_lblDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblDescription.Location = New System.Drawing.Point(3, 131)
        Me.m_lblDescription.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblDescription.Name = "m_lblDescription"
        Me.m_lblDescription.Size = New System.Drawing.Size(63, 124)
        Me.m_lblDescription.TabIndex = 10
        Me.m_lblDescription.Text = "Description:"
        '
        'm_lbLicense
        '
        Me.m_lbLicense.AutoSize = True
        Me.m_lbLicense.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbLicense.Location = New System.Drawing.Point(3, 106)
        Me.m_lbLicense.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lbLicense.Name = "m_lbLicense"
        Me.m_lbLicense.Size = New System.Drawing.Size(63, 13)
        Me.m_lbLicense.TabIndex = 8
        Me.m_lbLicense.Text = "License:"
        Me.m_lbLicense.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tbxLicense
        '
        Me.m_tbxLicense.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbxLicense.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbxLicense.Location = New System.Drawing.Point(72, 106)
        Me.m_tbxLicense.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbxLicense.Name = "m_tbxLicense"
        Me.m_tbxLicense.ReadOnly = True
        Me.m_tbxLicense.Size = New System.Drawing.Size(368, 13)
        Me.m_tbxLicense.TabIndex = 9
        Me.m_tbxLicense.TabStop = False
        '
        'ucOptionsPluginAssemblyDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ucOptionsPluginAssemblyDetails"
        Me.Size = New System.Drawing.Size(443, 261)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_lblCopyright As System.Windows.Forms.Label
    Friend WithEvents m_lblCompany As System.Windows.Forms.Label
    Friend WithEvents m_lblVersion As System.Windows.Forms.Label
    Friend WithEvents m_lblFile As System.Windows.Forms.Label
    Friend WithEvents m_tbFile As System.Windows.Forms.TextBox
    Friend WithEvents m_tbVersion As System.Windows.Forms.TextBox
    Friend WithEvents m_tbCompany As System.Windows.Forms.TextBox
    Friend WithEvents m_tbCopyright As System.Windows.Forms.TextBox
    Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Private WithEvents m_lblDescription As Label
    Private WithEvents m_lbLicense As Label
    Private WithEvents m_tbxLicense As TextBox
End Class
