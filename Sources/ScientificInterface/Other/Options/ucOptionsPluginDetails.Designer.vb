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

Partial Class ucOptionsPluginDetails
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
        Me.m_lblContact = New System.Windows.Forms.Label()
        Me.m_lblAuthor = New System.Windows.Forms.Label()
        Me.m_lblName = New System.Windows.Forms.Label()
        Me.m_tbName = New System.Windows.Forms.TextBox()
        Me.m_tbDescription = New System.Windows.Forms.TextBox()
        Me.m_tbAuthor = New System.Windows.Forms.TextBox()
        Me.m_llContact = New System.Windows.Forms.LinkLabel()
        Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblDescription = New System.Windows.Forms.Label()
        Me.m_tlpContent.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblContact
        '
        Me.m_lblContact.AutoSize = True
        Me.m_lblContact.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblContact.Location = New System.Drawing.Point(3, 56)
        Me.m_lblContact.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblContact.Name = "m_lblContact"
        Me.m_lblContact.Size = New System.Drawing.Size(63, 13)
        Me.m_lblContact.TabIndex = 4
        Me.m_lblContact.Text = "Contact:"
        '
        'm_lblAuthor
        '
        Me.m_lblAuthor.AutoSize = True
        Me.m_lblAuthor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblAuthor.Location = New System.Drawing.Point(3, 31)
        Me.m_lblAuthor.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblAuthor.Name = "m_lblAuthor"
        Me.m_lblAuthor.Size = New System.Drawing.Size(63, 13)
        Me.m_lblAuthor.TabIndex = 2
        Me.m_lblAuthor.Text = "Author(s):"
        '
        'm_lblName
        '
        Me.m_lblName.AutoSize = True
        Me.m_lblName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblName.Location = New System.Drawing.Point(3, 6)
        Me.m_lblName.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblName.Name = "m_lblName"
        Me.m_lblName.Size = New System.Drawing.Size(63, 13)
        Me.m_lblName.TabIndex = 0
        Me.m_lblName.Text = "Name:"
        '
        'm_tbName
        '
        Me.m_tbName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbName.Location = New System.Drawing.Point(72, 6)
        Me.m_tbName.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbName.Name = "m_tbName"
        Me.m_tbName.ReadOnly = True
        Me.m_tbName.Size = New System.Drawing.Size(216, 13)
        Me.m_tbName.TabIndex = 1
        '
        'm_tbDescription
        '
        Me.m_tbDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbDescription.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbDescription.Cursor = System.Windows.Forms.Cursors.Default
        Me.m_tbDescription.Location = New System.Drawing.Point(72, 81)
        Me.m_tbDescription.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbDescription.Multiline = True
        Me.m_tbDescription.Name = "m_tbDescription"
        Me.m_tbDescription.ReadOnly = True
        Me.m_tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.m_tbDescription.Size = New System.Drawing.Size(216, 174)
        Me.m_tbDescription.TabIndex = 7
        '
        'm_tbAuthor
        '
        Me.m_tbAuthor.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbAuthor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbAuthor.Location = New System.Drawing.Point(72, 31)
        Me.m_tbAuthor.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_tbAuthor.Name = "m_tbAuthor"
        Me.m_tbAuthor.ReadOnly = True
        Me.m_tbAuthor.Size = New System.Drawing.Size(216, 13)
        Me.m_tbAuthor.TabIndex = 3
        '
        'm_llContact
        '
        Me.m_llContact.AutoSize = True
        Me.m_llContact.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_llContact.Location = New System.Drawing.Point(72, 56)
        Me.m_llContact.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_llContact.Name = "m_llContact"
        Me.m_llContact.Size = New System.Drawing.Size(216, 13)
        Me.m_llContact.TabIndex = 5
        '
        'm_tlpContent
        '
        Me.m_tlpContent.ColumnCount = 2
        Me.m_tlpContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.m_tlpContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpContent.Controls.Add(Me.m_lblDescription, 0, 3)
        Me.m_tlpContent.Controls.Add(Me.m_lblName, 0, 0)
        Me.m_tlpContent.Controls.Add(Me.m_tbDescription, 1, 3)
        Me.m_tlpContent.Controls.Add(Me.m_llContact, 1, 2)
        Me.m_tlpContent.Controls.Add(Me.m_tbName, 1, 0)
        Me.m_tlpContent.Controls.Add(Me.m_tbAuthor, 1, 1)
        Me.m_tlpContent.Controls.Add(Me.m_lblContact, 0, 2)
        Me.m_tlpContent.Controls.Add(Me.m_lblAuthor, 0, 1)
        Me.m_tlpContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpContent.Location = New System.Drawing.Point(0, 0)
        Me.m_tlpContent.Name = "m_tlpContent"
        Me.m_tlpContent.RowCount = 4
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpContent.Size = New System.Drawing.Size(291, 261)
        Me.m_tlpContent.TabIndex = 0
        '
        'm_lblDescription
        '
        Me.m_lblDescription.AutoSize = True
        Me.m_lblDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblDescription.Location = New System.Drawing.Point(3, 81)
        Me.m_lblDescription.Margin = New System.Windows.Forms.Padding(3, 6, 3, 6)
        Me.m_lblDescription.Name = "m_lblDescription"
        Me.m_lblDescription.Size = New System.Drawing.Size(63, 174)
        Me.m_lblDescription.TabIndex = 6
        Me.m_lblDescription.Text = "Description:"
        '
        'ucOptionsPluginDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.m_tlpContent)
        Me.Name = "ucOptionsPluginDetails"
        Me.Size = New System.Drawing.Size(291, 261)
        Me.m_tlpContent.ResumeLayout(False)
        Me.m_tlpContent.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_lblContact As System.Windows.Forms.Label
    Private WithEvents m_lblAuthor As System.Windows.Forms.Label
    Private WithEvents m_lblName As System.Windows.Forms.Label
    Private WithEvents m_tbName As System.Windows.Forms.TextBox
    Private WithEvents m_tbDescription As System.Windows.Forms.TextBox
    Private WithEvents m_tbAuthor As System.Windows.Forms.TextBox
    Private WithEvents m_llContact As System.Windows.Forms.LinkLabel
    Private WithEvents m_tlpContent As TableLayoutPanel
    Private WithEvents m_lblDescription As Label
End Class
