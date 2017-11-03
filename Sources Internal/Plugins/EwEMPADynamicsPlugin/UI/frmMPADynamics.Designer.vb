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
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Forms

#End Region ' Imports

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMPADynamics
    Inherits frmEwE

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMPADynamics))
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tsbnLoadCSV = New System.Windows.Forms.ToolStripButton()
        Me.m_sep = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbnShowMonths = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnShowFleets = New System.Windows.Forms.ToolStripButton()
        Me.m_tscmbFleets = New System.Windows.Forms.ToolStripComboBox()
        Me.m_dgvStates = New System.Windows.Forms.DataGridView()
        Me.m_colTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colMPA = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_tsMain.SuspendLayout()
        CType(Me.m_dgvStates, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnLoadCSV, Me.m_sep, Me.m_tsbnShowMonths, Me.m_tsbnShowFleets, Me.m_tscmbFleets})
        resources.ApplyResources(Me.m_tsMain, "m_tsMain")
        Me.m_tsMain.Name = "m_tsMain"
        Me.m_tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'm_tsbnLoadCSV
        '
        Me.m_tsbnLoadCSV.AutoToolTip = False
        resources.ApplyResources(Me.m_tsbnLoadCSV, "m_tsbnLoadCSV")
        Me.m_tsbnLoadCSV.Name = "m_tsbnLoadCSV"
        '
        'm_sep
        '
        Me.m_sep.Name = "m_sep"
        resources.ApplyResources(Me.m_sep, "m_sep")
        '
        'm_tsbnShowMonths
        '
        Me.m_tsbnShowMonths.CheckOnClick = True
        Me.m_tsbnShowMonths.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnShowMonths, "m_tsbnShowMonths")
        Me.m_tsbnShowMonths.Name = "m_tsbnShowMonths"
        '
        'm_tsbnShowFleets
        '
        Me.m_tsbnShowFleets.CheckOnClick = True
        Me.m_tsbnShowFleets.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnShowFleets, "m_tsbnShowFleets")
        Me.m_tsbnShowFleets.Name = "m_tsbnShowFleets"
        '
        'm_tscmbFleets
        '
        Me.m_tscmbFleets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmbFleets.Name = "m_tscmbFleets"
        resources.ApplyResources(Me.m_tscmbFleets, "m_tscmbFleets")
        '
        'm_dgvStates
        '
        Me.m_dgvStates.AllowDrop = True
        Me.m_dgvStates.AllowUserToAddRows = False
        Me.m_dgvStates.AllowUserToDeleteRows = False
        Me.m_dgvStates.AllowUserToResizeRows = False
        Me.m_dgvStates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvStates.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colTime, Me.m_colMPA})
        resources.ApplyResources(Me.m_dgvStates, "m_dgvStates")
        Me.m_dgvStates.MultiSelect = False
        Me.m_dgvStates.Name = "m_dgvStates"
        Me.m_dgvStates.ReadOnly = True
        Me.m_dgvStates.RowHeadersVisible = False
        '
        'm_colTime
        '
        Me.m_colTime.Frozen = True
        resources.ApplyResources(Me.m_colTime, "m_colTime")
        Me.m_colTime.MaxInputLength = 12
        Me.m_colTime.Name = "m_colTime"
        Me.m_colTime.ReadOnly = True
        '
        'm_colMPA
        '
        Me.m_colMPA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.m_colMPA.Frozen = True
        resources.ApplyResources(Me.m_colMPA, "m_colMPA")
        Me.m_colMPA.Name = "m_colMPA"
        Me.m_colMPA.ReadOnly = True
        '
        'frmMPADynamics
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_dgvStates)
        Me.Controls.Add(Me.m_tsMain)
        Me.Name = "frmMPADynamics"
        Me.TabText = ""
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        CType(Me.m_dgvStates, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_tsMain As ScientificInterfaceShared.Controls.cEwEToolstrip
    Private WithEvents m_tsbnLoadCSV As Windows.Forms.ToolStripButton
    Private WithEvents m_dgvStates As Windows.Forms.DataGridView
    Private WithEvents m_colTime As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_colMPA As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_sep As Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbnShowMonths As Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnShowFleets As Windows.Forms.ToolStripButton
    Private WithEvents m_tscmbFleets As Windows.Forms.ToolStripComboBox
End Class
