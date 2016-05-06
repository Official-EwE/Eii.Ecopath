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
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrConfig = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btLoadDepthDataset = New System.Windows.Forms.Button()
        Me.m_btConfigDepth = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.m_lbConfigFile = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_lstDatasets = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'CEwEHeaderLabel2
        '
        Me.CEwEHeaderLabel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel2.CanCollapseParent = False
        Me.CEwEHeaderLabel2.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel2.IsCollapsed = False
        Me.CEwEHeaderLabel2.Location = New System.Drawing.Point(12, 138)
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        Me.CEwEHeaderLabel2.Size = New System.Drawing.Size(442, 22)
        Me.CEwEHeaderLabel2.TabIndex = 7
        Me.CEwEHeaderLabel2.Text = "Pick depth data set"
        Me.CEwEHeaderLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_hdrConfig
        '
        Me.m_hdrConfig.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrConfig.CanCollapseParent = False
        Me.m_hdrConfig.CollapsedParentHeight = 0
        Me.m_hdrConfig.IsCollapsed = False
        Me.m_hdrConfig.Location = New System.Drawing.Point(12, 9)
        Me.m_hdrConfig.Name = "m_hdrConfig"
        Me.m_hdrConfig.Size = New System.Drawing.Size(442, 24)
        Me.m_hdrConfig.TabIndex = 6
        Me.m_hdrConfig.Text = "Load spatial configuration file"
        Me.m_hdrConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_btLoadDepthDataset
        '
        Me.m_btLoadDepthDataset.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_btLoadDepthDataset.Location = New System.Drawing.Point(12, 328)
        Me.m_btLoadDepthDataset.Name = "m_btLoadDepthDataset"
        Me.m_btLoadDepthDataset.Size = New System.Drawing.Size(219, 23)
        Me.m_btLoadDepthDataset.TabIndex = 5
        Me.m_btLoadDepthDataset.Text = "Use selected dataset for Depth"
        Me.m_btLoadDepthDataset.UseVisualStyleBackColor = True
        '
        'm_btConfigDepth
        '
        Me.m_btConfigDepth.Location = New System.Drawing.Point(12, 36)
        Me.m_btConfigDepth.Name = "m_btConfigDepth"
        Me.m_btConfigDepth.Size = New System.Drawing.Size(219, 23)
        Me.m_btConfigDepth.TabIndex = 0
        Me.m_btConfigDepth.Text = "Select spatial configuration file..."
        Me.m_btConfigDepth.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 171)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(99, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Available data sets:"
        '
        'm_lbConfigFile
        '
        Me.m_lbConfigFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbConfigFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_lbConfigFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.m_lbConfigFile.Location = New System.Drawing.Point(12, 96)
        Me.m_lbConfigFile.Name = "m_lbConfigFile"
        Me.m_lbConfigFile.Size = New System.Drawing.Size(442, 23)
        Me.m_lbConfigFile.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 73)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(167, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Currently loaded spatial config. file"
        '
        'm_lstDatasets
        '
        Me.m_lstDatasets.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lstDatasets.FormattingEnabled = True
        Me.m_lstDatasets.IntegralHeight = False
        Me.m_lstDatasets.Location = New System.Drawing.Point(12, 187)
        Me.m_lstDatasets.Name = "m_lstDatasets"
        Me.m_lstDatasets.Size = New System.Drawing.Size(442, 135)
        Me.m_lstDatasets.TabIndex = 2
        '
        'frmEwEPlugin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(466, 363)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_btLoadDepthDataset)
        Me.Controls.Add(Me.CEwEHeaderLabel2)
        Me.Controls.Add(Me.m_lstDatasets)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.m_hdrConfig)
        Me.Controls.Add(Me.m_btConfigDepth)
        Me.Controls.Add(Me.m_lbConfigFile)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEwEPlugin"
        Me.ShowInTaskbar = False
        Me.TabText = "Basic plug-in"
        Me.Text = "Configure Roberts Bank data"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btConfigDepth As System.Windows.Forms.Button
    Private WithEvents m_lbConfigFile As System.Windows.Forms.Label
    Private WithEvents m_lstDatasets As System.Windows.Forms.ListBox
    Private WithEvents m_btLoadDepthDataset As System.Windows.Forms.Button
    Private WithEvents m_hdrConfig As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
