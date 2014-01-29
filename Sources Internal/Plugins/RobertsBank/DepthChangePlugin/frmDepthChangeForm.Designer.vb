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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEwEPlugin
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btLoadDepthDataset = New System.Windows.Forms.Button()
        Me.m_btConfigDepth = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.m_lbConfigFile = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_lstDatasets = New System.Windows.Forms.ListBox()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.AutoSize = True
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.CEwEHeaderLabel2)
        Me.Panel1.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Panel1.Controls.Add(Me.m_btLoadDepthDataset)
        Me.Panel1.Controls.Add(Me.m_btConfigDepth)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.m_lbConfigFile)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.m_lstDatasets)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(713, 339)
        Me.Panel1.TabIndex = 6
        '
        'CEwEHeaderLabel2
        '
        Me.CEwEHeaderLabel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel2.CanCollapseParent = False
        Me.CEwEHeaderLabel2.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel2.IsCollapsed = False
        Me.CEwEHeaderLabel2.Location = New System.Drawing.Point(12, 133)
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        Me.CEwEHeaderLabel2.Size = New System.Drawing.Size(691, 22)
        Me.CEwEHeaderLabel2.TabIndex = 7
        Me.CEwEHeaderLabel2.Text = "Pick depth data set"
        Me.CEwEHeaderLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(691, 24)
        Me.CEwEHeaderLabel1.TabIndex = 6
        Me.CEwEHeaderLabel1.Text = "Load spatial configuration file"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_btLoadDepthDataset
        '
        Me.m_btLoadDepthDataset.Location = New System.Drawing.Point(20, 245)
        Me.m_btLoadDepthDataset.Name = "m_btLoadDepthDataset"
        Me.m_btLoadDepthDataset.Size = New System.Drawing.Size(219, 23)
        Me.m_btLoadDepthDataset.TabIndex = 5
        Me.m_btLoadDepthDataset.Text = "Use selected dataset for Depth"
        Me.m_btLoadDepthDataset.UseVisualStyleBackColor = True
        '
        'm_btConfigDepth
        '
        Me.m_btConfigDepth.Location = New System.Drawing.Point(20, 36)
        Me.m_btConfigDepth.Name = "m_btConfigDepth"
        Me.m_btConfigDepth.Size = New System.Drawing.Size(219, 23)
        Me.m_btConfigDepth.TabIndex = 0
        Me.m_btConfigDepth.Text = "Select Spatial Configuration file..."
        Me.m_btConfigDepth.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 167)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Data sets"
        '
        'm_lbConfigFile
        '
        Me.m_lbConfigFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbConfigFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_lbConfigFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.m_lbConfigFile.Location = New System.Drawing.Point(20, 85)
        Me.m_lbConfigFile.Name = "m_lbConfigFile"
        Me.m_lbConfigFile.Size = New System.Drawing.Size(675, 23)
        Me.m_lbConfigFile.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 72)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(167, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Currently loaded spatial config. file"
        '
        'm_lstDatasets
        '
        Me.m_lstDatasets.FormattingEnabled = True
        Me.m_lstDatasets.Location = New System.Drawing.Point(20, 183)
        Me.m_lstDatasets.Name = "m_lstDatasets"
        Me.m_lstDatasets.Size = New System.Drawing.Size(219, 56)
        Me.m_lstDatasets.TabIndex = 2
        '
        'frmEwEPlugin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(713, 339)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEwEPlugin"
        Me.ShowInTaskbar = False
        Me.TabText = "Basic plug-in"
        Me.Text = "Configure Roberts Bank data"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_btConfigDepth As System.Windows.Forms.Button
    Friend WithEvents m_lbConfigFile As System.Windows.Forms.Label
    Friend WithEvents m_lstDatasets As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents m_btLoadDepthDataset As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
