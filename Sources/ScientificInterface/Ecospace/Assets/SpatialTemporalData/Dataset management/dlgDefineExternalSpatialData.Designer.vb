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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms

Namespace Ecospace.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgDefineExternalSpatialData
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgDefineExternalSpatialData))
            Me.m_btnDelete = New System.Windows.Forms.Button()
            Me.m_btnOK = New System.Windows.Forms.Button()
            Me.m_btnConfigure = New System.Windows.Forms.Button()
            Me.m_cbEnableIndexing = New System.Windows.Forms.CheckBox()
            Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnSwitchConfig = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnExport = New System.Windows.Forms.ToolStripButton()
            Me.m_gridDatasets = New ScientificInterface.Ecospace.Controls.gridDefineExternalSpatialData()
            Me.m_btnCreate = New System.Windows.Forms.Button()
            Me.m_cmbTemplates = New System.Windows.Forms.ComboBox()
            Me.m_lblNew = New System.Windows.Forms.Label()
            Me.m_hdrExisting = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnDelete
            '
            resources.ApplyResources(Me.m_btnDelete, "m_btnDelete")
            Me.m_btnDelete.Name = "m_btnDelete"
            Me.m_btnDelete.UseVisualStyleBackColor = True
            '
            'm_btnOK
            '
            resources.ApplyResources(Me.m_btnOK, "m_btnOK")
            Me.m_btnOK.Name = "m_btnOK"
            Me.m_btnOK.UseVisualStyleBackColor = True
            '
            'm_btnConfigure
            '
            resources.ApplyResources(Me.m_btnConfigure, "m_btnConfigure")
            Me.m_btnConfigure.Name = "m_btnConfigure"
            Me.m_btnConfigure.UseVisualStyleBackColor = True
            '
            'm_cbEnableIndexing
            '
            resources.ApplyResources(Me.m_cbEnableIndexing, "m_cbEnableIndexing")
            Me.m_cbEnableIndexing.Name = "m_cbEnableIndexing"
            Me.m_cbEnableIndexing.UseVisualStyleBackColor = True
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnSwitchConfig, Me.ToolStripSeparator1, Me.m_tsbnExport})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnSwitchConfig
            '
            resources.ApplyResources(Me.m_tsbnSwitchConfig, "m_tsbnSwitchConfig")
            Me.m_tsbnSwitchConfig.Name = "m_tsbnSwitchConfig"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'm_tsbnExport
            '
            resources.ApplyResources(Me.m_tsbnExport, "m_tsbnExport")
            Me.m_tsbnExport.Name = "m_tsbnExport"
            '
            'm_gridDatasets
            '
            Me.m_gridDatasets.AllowBlockSelect = False
            resources.ApplyResources(Me.m_gridDatasets, "m_gridDatasets")
            Me.m_gridDatasets.AutoSizeMinHeight = 10
            Me.m_gridDatasets.AutoSizeMinWidth = 10
            Me.m_gridDatasets.AutoStretchColumnsToFitWidth = True
            Me.m_gridDatasets.AutoStretchRowsToFitHeight = False
            Me.m_gridDatasets.BackColor = System.Drawing.Color.White
            Me.m_gridDatasets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridDatasets.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridDatasets.CustomSort = False
            Me.m_gridDatasets.DataName = "grid content"
            Me.m_gridDatasets.FixedColumnWidths = False
            Me.m_gridDatasets.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridDatasets.GridToolTipActive = True
            Me.m_gridDatasets.IsLayoutSuspended = False
            Me.m_gridDatasets.Name = "m_gridDatasets"
            Me.m_gridDatasets.SelectedDataset = Nothing
            Me.m_gridDatasets.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                Or SourceGrid2.GridSpecialKeys.Delete) _
                Or SourceGrid2.GridSpecialKeys.Arrows) _
                Or SourceGrid2.GridSpecialKeys.Tab) _
                Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                Or SourceGrid2.GridSpecialKeys.Enter) _
                Or SourceGrid2.GridSpecialKeys.Escape) _
                Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridDatasets.UIContext = Nothing
            '
            'm_btnCreate
            '
            resources.ApplyResources(Me.m_btnCreate, "m_btnCreate")
            Me.m_btnCreate.Name = "m_btnCreate"
            Me.m_btnCreate.UseVisualStyleBackColor = True
            '
            'm_cmbTemplates
            '
            resources.ApplyResources(Me.m_cmbTemplates, "m_cmbTemplates")
            Me.m_cmbTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbTemplates.FormattingEnabled = True
            Me.m_cmbTemplates.Name = "m_cmbTemplates"
            '
            'm_lblNew
            '
            resources.ApplyResources(Me.m_lblNew, "m_lblNew")
            Me.m_lblNew.Name = "m_lblNew"
            '
            'm_hdrExisting
            '
            resources.ApplyResources(Me.m_hdrExisting, "m_hdrExisting")
            Me.m_hdrExisting.CanCollapseParent = False
            Me.m_hdrExisting.CollapsedParentHeight = 0
            Me.m_hdrExisting.IsCollapsed = False
            Me.m_hdrExisting.Name = "m_hdrExisting"
            '
            'dlgDefineExternalSpatialData
            '
            Me.AcceptButton = Me.m_btnOK
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ControlBox = False
            Me.Controls.Add(Me.m_hdrExisting)
            Me.Controls.Add(Me.m_lblNew)
            Me.Controls.Add(Me.m_cmbTemplates)
            Me.Controls.Add(Me.m_ts)
            Me.Controls.Add(Me.m_cbEnableIndexing)
            Me.Controls.Add(Me.m_btnOK)
            Me.Controls.Add(Me.m_gridDatasets)
            Me.Controls.Add(Me.m_btnCreate)
            Me.Controls.Add(Me.m_btnConfigure)
            Me.Controls.Add(Me.m_btnDelete)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgDefineExternalSpatialData"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_btnDelete As System.Windows.Forms.Button
        Private WithEvents m_gridDatasets As gridDefineExternalSpatialData
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        Private WithEvents m_btnConfigure As System.Windows.Forms.Button
        Private WithEvents m_cbEnableIndexing As System.Windows.Forms.CheckBox
        Private WithEvents m_ts As cEwEToolstrip
        Private WithEvents m_tsbnSwitchConfig As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnExport As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_btnCreate As System.Windows.Forms.Button
        Private WithEvents m_cmbTemplates As System.Windows.Forms.ComboBox
        Private WithEvents m_lblNew As System.Windows.Forms.Label
        Private WithEvents m_hdrExisting As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    End Class

End Namespace
