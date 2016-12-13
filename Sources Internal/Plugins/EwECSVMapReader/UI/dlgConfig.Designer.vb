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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class dlgConfig
    Inherits System.Windows.Forms.UserControl

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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_tbxFolder = New System.Windows.Forms.TextBox()
        Me.m_btnChooseFolder = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.m_tbxDescription = New System.Windows.Forms.TextBox()
        Me.m_tbxName = New System.Windows.Forms.TextBox()
        Me.m_lbxFiles = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 64)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "&Folder:"
        '
        'm_tbxFolder
        '
        Me.m_tbxFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxFolder.Location = New System.Drawing.Point(81, 61)
        Me.m_tbxFolder.Name = "m_tbxFolder"
        Me.m_tbxFolder.ReadOnly = True
        Me.m_tbxFolder.Size = New System.Drawing.Size(110, 20)
        Me.m_tbxFolder.TabIndex = 5
        '
        'm_btnChooseFolder
        '
        Me.m_btnChooseFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnChooseFolder.Location = New System.Drawing.Point(197, 59)
        Me.m_btnChooseFolder.Name = "m_btnChooseFolder"
        Me.m_btnChooseFolder.Size = New System.Drawing.Size(75, 23)
        Me.m_btnChooseFolder.TabIndex = 6
        Me.m_btnChooseFolder.Text = "&Choose..."
        Me.m_btnChooseFolder.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 36)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "&Description:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "&Name:"
        '
        'm_tbxDescription
        '
        Me.m_tbxDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxDescription.Location = New System.Drawing.Point(81, 33)
        Me.m_tbxDescription.Name = "m_tbxDescription"
        Me.m_tbxDescription.Size = New System.Drawing.Size(191, 20)
        Me.m_tbxDescription.TabIndex = 3
        '
        'm_tbxName
        '
        Me.m_tbxName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxName.Location = New System.Drawing.Point(81, 6)
        Me.m_tbxName.Name = "m_tbxName"
        Me.m_tbxName.Size = New System.Drawing.Size(191, 20)
        Me.m_tbxName.TabIndex = 1
        '
        'm_lbxFiles
        '
        Me.m_lbxFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbxFiles.FormattingEnabled = True
        Me.m_lbxFiles.IntegralHeight = False
        Me.m_lbxFiles.Location = New System.Drawing.Point(81, 88)
        Me.m_lbxFiles.Name = "m_lbxFiles"
        Me.m_lbxFiles.Size = New System.Drawing.Size(191, 232)
        Me.m_lbxFiles.TabIndex = 7
        '
        'dlgConfig
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_lbxFiles)
        Me.Controls.Add(Me.m_tbxName)
        Me.Controls.Add(Me.m_tbxDescription)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.m_btnChooseFolder)
        Me.Controls.Add(Me.m_tbxFolder)
        Me.Controls.Add(Me.Label1)
        Me.Name = "dlgConfig"
        Me.Size = New System.Drawing.Size(284, 329)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents m_tbxFolder As System.Windows.Forms.TextBox
    Friend WithEvents m_btnChooseFolder As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents m_tbxDescription As System.Windows.Forms.TextBox
    Friend WithEvents m_tbxName As System.Windows.Forms.TextBox
    Friend WithEvents m_lbxFiles As System.Windows.Forms.ListBox
End Class
