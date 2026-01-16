' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucConfigureCConcDriver
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.m_lblGroup = New System.Windows.Forms.Label()
        Me.m_tlpControls = New System.Windows.Forms.TableLayoutPanel()
        Me.m_cmbGroups = New System.Windows.Forms.ComboBox()
        Me.m_tlpControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblGroup
        '
        Me.m_lblGroup.AutoSize = True
        Me.m_lblGroup.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblGroup.Location = New System.Drawing.Point(3, 0)
        Me.m_lblGroup.Name = "m_lblGroup"
        Me.m_lblGroup.Size = New System.Drawing.Size(39, 27)
        Me.m_lblGroup.TabIndex = 0
        Me.m_lblGroup.Text = "&Group:"
        Me.m_lblGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tlpControls
        '
        Me.m_tlpControls.ColumnCount = 2
        Me.m_tlpControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.m_tlpControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpControls.Controls.Add(Me.m_lblGroup, 0, 0)
        Me.m_tlpControls.Controls.Add(Me.m_cmbGroups, 1, 0)
        Me.m_tlpControls.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpControls.Location = New System.Drawing.Point(0, 0)
        Me.m_tlpControls.Name = "m_tlpControls"
        Me.m_tlpControls.RowCount = 2
        Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpControls.Size = New System.Drawing.Size(171, 74)
        Me.m_tlpControls.TabIndex = 1
        '
        'm_cmbGroups
        '
        Me.m_cmbGroups.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_cmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbGroups.FormattingEnabled = True
        Me.m_cmbGroups.Location = New System.Drawing.Point(48, 3)
        Me.m_cmbGroups.Name = "m_cmbGroups"
        Me.m_cmbGroups.Size = New System.Drawing.Size(120, 21)
        Me.m_cmbGroups.TabIndex = 1
        '
        'ucConfigureCConcDriver
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlpControls)
        Me.Name = "ucConfigureCConcDriver"
        Me.Size = New System.Drawing.Size(171, 74)
        Me.m_tlpControls.ResumeLayout(False)
        Me.m_tlpControls.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tlpControls As Windows.Forms.TableLayoutPanel
    Private WithEvents m_cmbGroups As Windows.Forms.ComboBox
    Private WithEvents m_lblGroup As Windows.Forms.Label
End Class
