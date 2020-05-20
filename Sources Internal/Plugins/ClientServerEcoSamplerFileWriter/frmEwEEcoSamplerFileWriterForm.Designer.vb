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

Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmEwEEcoSamplerFileWriterForm
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEwEEcoSamplerFileWriterForm))
        Me.m_txtFileName = New System.Windows.Forms.TextBox()
        Me.m_btSelectFile = New System.Windows.Forms.Button()
        Me.m_btSave = New System.Windows.Forms.Button()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.SuspendLayout()
        '
        'm_txtFileName
        '
        Me.m_txtFileName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_txtFileName.Location = New System.Drawing.Point(186, 119)
        Me.m_txtFileName.Name = "m_txtFileName"
        Me.m_txtFileName.Size = New System.Drawing.Size(1040, 29)
        Me.m_txtFileName.TabIndex = 0
        '
        'm_btSelectFile
        '
        Me.m_btSelectFile.Location = New System.Drawing.Point(12, 117)
        Me.m_btSelectFile.Name = "m_btSelectFile"
        Me.m_btSelectFile.Size = New System.Drawing.Size(168, 35)
        Me.m_btSelectFile.TabIndex = 1
        Me.m_btSelectFile.Text = "Select file..."
        Me.m_btSelectFile.UseVisualStyleBackColor = True
        '
        'm_btSave
        '
        Me.m_btSave.Location = New System.Drawing.Point(12, 76)
        Me.m_btSave.Name = "m_btSave"
        Me.m_btSave.Size = New System.Drawing.Size(168, 35)
        Me.m_btSave.TabIndex = 2
        Me.m_btSave.Text = "Save File"
        Me.m_btSave.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel1
        '
        Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(12, 9)
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(1214, 52)
        Me.CEwEHeaderLabel1.TabIndex = 3
        Me.CEwEHeaderLabel1.Text = "Save current EcoSampler data to .csv file"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmEwEEcoSamplerFileWriterForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(168.0!, 168.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1238, 786)
        Me.ControlBox = False
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.m_btSave)
        Me.Controls.Add(Me.m_btSelectFile)
        Me.Controls.Add(Me.m_txtFileName)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.Name = "frmEwEEcoSamplerFileWriterForm"
        Me.ShowInTaskbar = False
        Me.TabText = "EcoSampler File Writer"
        Me.Text = "EcoSampler File Writer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents m_txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents m_btSelectFile As System.Windows.Forms.Button
    Friend WithEvents m_btSave As System.Windows.Forms.Button
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
