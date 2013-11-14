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
        Me.m_btConfigDepth = New System.Windows.Forms.Button()
        Me.m_lbConfigFile = New System.Windows.Forms.Label()
        Me.m_lstDatasets = New System.Windows.Forms.ListBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.m_btLoadDepthDataset = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'm_btConfigDepth
        '
        Me.m_btConfigDepth.Location = New System.Drawing.Point(12, 22)
        Me.m_btConfigDepth.Name = "m_btConfigDepth"
        Me.m_btConfigDepth.Size = New System.Drawing.Size(219, 23)
        Me.m_btConfigDepth.TabIndex = 0
        Me.m_btConfigDepth.Text = "Select Spatial Configuration file..."
        Me.m_btConfigDepth.UseVisualStyleBackColor = True
        '
        'm_lbConfigFile
        '
        Me.m_lbConfigFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbConfigFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lbConfigFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.m_lbConfigFile.Location = New System.Drawing.Point(12, 71)
        Me.m_lbConfigFile.Name = "m_lbConfigFile"
        Me.m_lbConfigFile.Size = New System.Drawing.Size(571, 23)
        Me.m_lbConfigFile.TabIndex = 1
        '
        'm_lstDatasets
        '
        Me.m_lstDatasets.FormattingEnabled = True
        Me.m_lstDatasets.Location = New System.Drawing.Point(12, 135)
        Me.m_lstDatasets.Name = "m_lstDatasets"
        Me.m_lstDatasets.Size = New System.Drawing.Size(219, 56)
        Me.m_lstDatasets.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 58)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(167, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Currently loaded spatial config. file"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 119)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Data sets"
        '
        'm_btLoadDepthDataset
        '
        Me.m_btLoadDepthDataset.Location = New System.Drawing.Point(247, 135)
        Me.m_btLoadDepthDataset.Name = "m_btLoadDepthDataset"
        Me.m_btLoadDepthDataset.Size = New System.Drawing.Size(174, 25)
        Me.m_btLoadDepthDataset.TabIndex = 5
        Me.m_btLoadDepthDataset.Text = "Load selected dataset as Depth"
        Me.m_btLoadDepthDataset.UseVisualStyleBackColor = True
        '
        'frmEwEPlugin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(595, 334)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_btLoadDepthDataset)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_lstDatasets)
        Me.Controls.Add(Me.m_lbConfigFile)
        Me.Controls.Add(Me.m_btConfigDepth)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEwEPlugin"
        Me.ShowInTaskbar = False
        Me.TabText = "Basic plug-in"
        Me.Text = "Configure Roberts Bank data"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_btConfigDepth As System.Windows.Forms.Button
    Friend WithEvents m_lbConfigFile As System.Windows.Forms.Label
    Friend WithEvents m_lstDatasets As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents m_btLoadDepthDataset As System.Windows.Forms.Button
End Class
