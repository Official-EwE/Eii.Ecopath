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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Namespace Ecospace.Controls

    Partial Class ucConfigAdapter
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucConfigAdapter))
            Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plConnectionConverter = New System.Windows.Forms.Panel()
            Me.m_cmbConverter = New System.Windows.Forms.ComboBox()
            Me.m_btnSaveStats = New System.Windows.Forms.Button()
            Me.m_btnClearCache = New System.Windows.Forms.Button()
            Me.m_cmbNewDS = New System.Windows.Forms.ComboBox()
            Me.m_btnConfigureCV = New System.Windows.Forms.Button()
            Me.m_btnDeleteDS = New System.Windows.Forms.Button()
            Me.m_btnConfigDS = New System.Windows.Forms.Button()
            Me.m_btnCreateDS = New System.Windows.Forms.Button()
            Me.m_lblSelectCV = New System.Windows.Forms.Label()
            Me.m_lblNewDS = New System.Windows.Forms.Label()
            Me.m_plScalarAdapter = New System.Windows.Forms.Panel()
            Me.m_btnCalculate = New System.Windows.Forms.Button()
            Me.m_tbxScale = New System.Windows.Forms.TextBox()
            Me.m_rbRelative = New System.Windows.Forms.RadioButton()
            Me.m_rbAbsolute = New System.Windows.Forms.RadioButton()
            Me.m_hdrSource = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrScaling = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_gridDatasets = New ScientificInterface.Ecospace.Controls.gridDatasets()
            Me.m_tlpContent.SuspendLayout()
            Me.m_plConnectionConverter.SuspendLayout()
            Me.m_plScalarAdapter.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpContent
            '
            resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
            Me.m_tlpContent.Controls.Add(Me.m_plConnectionConverter, 0, 0)
            Me.m_tlpContent.Controls.Add(Me.m_plScalarAdapter, 0, 1)
            Me.m_tlpContent.Name = "m_tlpContent"
            '
            'm_plConnectionConverter
            '
            Me.m_plConnectionConverter.Controls.Add(Me.m_gridDatasets)
            Me.m_plConnectionConverter.Controls.Add(Me.m_cmbConverter)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnSaveStats)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnClearCache)
            Me.m_plConnectionConverter.Controls.Add(Me.m_cmbNewDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnConfigureCV)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnDeleteDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnConfigDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnCreateDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_lblSelectCV)
            Me.m_plConnectionConverter.Controls.Add(Me.m_lblNewDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_hdrSource)
            resources.ApplyResources(Me.m_plConnectionConverter, "m_plConnectionConverter")
            Me.m_plConnectionConverter.Name = "m_plConnectionConverter"
            '
            'm_cmbConverter
            '
            resources.ApplyResources(Me.m_cmbConverter, "m_cmbConverter")
            Me.m_cmbConverter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbConverter.FormattingEnabled = True
            Me.m_cmbConverter.Name = "m_cmbConverter"
            '
            'm_btnSaveStats
            '
            resources.ApplyResources(Me.m_btnSaveStats, "m_btnSaveStats")
            Me.m_btnSaveStats.Name = "m_btnSaveStats"
            Me.m_btnSaveStats.UseVisualStyleBackColor = True
            '
            'm_btnClearCache
            '
            resources.ApplyResources(Me.m_btnClearCache, "m_btnClearCache")
            Me.m_btnClearCache.Name = "m_btnClearCache"
            Me.m_btnClearCache.UseVisualStyleBackColor = True
            '
            'm_cmbNewDS
            '
            resources.ApplyResources(Me.m_cmbNewDS, "m_cmbNewDS")
            Me.m_cmbNewDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbNewDS.FormattingEnabled = True
            Me.m_cmbNewDS.Name = "m_cmbNewDS"
            '
            'm_btnConfigureCV
            '
            resources.ApplyResources(Me.m_btnConfigureCV, "m_btnConfigureCV")
            Me.m_btnConfigureCV.Name = "m_btnConfigureCV"
            Me.m_btnConfigureCV.UseVisualStyleBackColor = True
            '
            'm_btnDeleteDS
            '
            resources.ApplyResources(Me.m_btnDeleteDS, "m_btnDeleteDS")
            Me.m_btnDeleteDS.Name = "m_btnDeleteDS"
            Me.m_btnDeleteDS.UseVisualStyleBackColor = True
            '
            'm_btnConfigDS
            '
            resources.ApplyResources(Me.m_btnConfigDS, "m_btnConfigDS")
            Me.m_btnConfigDS.Name = "m_btnConfigDS"
            Me.m_btnConfigDS.UseVisualStyleBackColor = True
            '
            'm_btnCreateDS
            '
            resources.ApplyResources(Me.m_btnCreateDS, "m_btnCreateDS")
            Me.m_btnCreateDS.Name = "m_btnCreateDS"
            Me.m_btnCreateDS.UseVisualStyleBackColor = True
            '
            'm_lblSelectCV
            '
            resources.ApplyResources(Me.m_lblSelectCV, "m_lblSelectCV")
            Me.m_lblSelectCV.Name = "m_lblSelectCV"
            '
            'm_lblNewDS
            '
            resources.ApplyResources(Me.m_lblNewDS, "m_lblNewDS")
            Me.m_lblNewDS.Name = "m_lblNewDS"
            '
            'm_plScalarAdapter
            '
            Me.m_plScalarAdapter.Controls.Add(Me.m_hdrScaling)
            Me.m_plScalarAdapter.Controls.Add(Me.m_btnCalculate)
            Me.m_plScalarAdapter.Controls.Add(Me.m_tbxScale)
            Me.m_plScalarAdapter.Controls.Add(Me.m_rbRelative)
            Me.m_plScalarAdapter.Controls.Add(Me.m_rbAbsolute)
            resources.ApplyResources(Me.m_plScalarAdapter, "m_plScalarAdapter")
            Me.m_plScalarAdapter.Name = "m_plScalarAdapter"
            '
            'm_btnCalculate
            '
            resources.ApplyResources(Me.m_btnCalculate, "m_btnCalculate")
            Me.m_btnCalculate.Name = "m_btnCalculate"
            Me.m_btnCalculate.UseVisualStyleBackColor = True
            '
            'm_tbxScale
            '
            resources.ApplyResources(Me.m_tbxScale, "m_tbxScale")
            Me.m_tbxScale.Name = "m_tbxScale"
            '
            'm_rbRelative
            '
            resources.ApplyResources(Me.m_rbRelative, "m_rbRelative")
            Me.m_rbRelative.Name = "m_rbRelative"
            Me.m_rbRelative.TabStop = True
            Me.m_rbRelative.UseVisualStyleBackColor = True
            '
            'm_rbAbsolute
            '
            resources.ApplyResources(Me.m_rbAbsolute, "m_rbAbsolute")
            Me.m_rbAbsolute.Name = "m_rbAbsolute"
            Me.m_rbAbsolute.TabStop = True
            Me.m_rbAbsolute.UseVisualStyleBackColor = True
            '
            'm_hdrSource
            '
            resources.ApplyResources(Me.m_hdrSource, "m_hdrSource")
            Me.m_hdrSource.CanCollapseParent = False
            Me.m_hdrSource.CollapsedParentHeight = 0
            Me.m_hdrSource.IsCollapsed = False
            Me.m_hdrSource.Name = "m_hdrSource"
            '
            'm_hdrScaling
            '
            Me.m_hdrScaling.CanCollapseParent = False
            Me.m_hdrScaling.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrScaling, "m_hdrScaling")
            Me.m_hdrScaling.IsCollapsed = False
            Me.m_hdrScaling.Name = "m_hdrScaling"
            '
            'm_gridDatasets
            '
            Me.m_gridDatasets.AllowBlockSelect = True
            resources.ApplyResources(Me.m_gridDatasets, "m_gridDatasets")
            Me.m_gridDatasets.AutoSizeMinHeight = 10
            Me.m_gridDatasets.AutoSizeMinWidth = 10
            Me.m_gridDatasets.AutoStretchColumnsToFitWidth = False
            Me.m_gridDatasets.AutoStretchRowsToFitHeight = False
            Me.m_gridDatasets.BackColor = System.Drawing.Color.White
            Me.m_gridDatasets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridDatasets.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridDatasets.CustomSort = False
            Me.m_gridDatasets.FixedColumnWidths = True
            Me.m_gridDatasets.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridDatasets.GridToolTipActive = True
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
            'ucConfigAdapter
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpContent)
            Me.Name = "ucConfigAdapter"
            Me.m_tlpContent.ResumeLayout(False)
            Me.m_plConnectionConverter.ResumeLayout(False)
            Me.m_plConnectionConverter.PerformLayout()
            Me.m_plScalarAdapter.ResumeLayout(False)
            Me.m_plScalarAdapter.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plConnectionConverter As System.Windows.Forms.Panel
        Private WithEvents m_cmbConverter As System.Windows.Forms.ComboBox
        Private WithEvents m_btnClearCache As System.Windows.Forms.Button
        Private WithEvents m_cmbNewDS As System.Windows.Forms.ComboBox
        Private WithEvents m_btnConfigureCV As System.Windows.Forms.Button
        Private WithEvents m_btnDeleteDS As System.Windows.Forms.Button
        Private WithEvents m_btnConfigDS As System.Windows.Forms.Button
        Private WithEvents m_btnCreateDS As System.Windows.Forms.Button
        Private WithEvents m_lblSelectCV As System.Windows.Forms.Label
        Private WithEvents m_lblNewDS As System.Windows.Forms.Label
        Private WithEvents m_hdrSource As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_plScalarAdapter As System.Windows.Forms.Panel
        Private WithEvents m_btnCalculate As System.Windows.Forms.Button
        Private WithEvents m_tbxScale As System.Windows.Forms.TextBox
        Private WithEvents m_rbRelative As System.Windows.Forms.RadioButton
        Private WithEvents m_rbAbsolute As System.Windows.Forms.RadioButton
        Private WithEvents m_hdrScaling As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_btnSaveStats As System.Windows.Forms.Button
        Friend WithEvents m_gridDatasets As ScientificInterface.Ecospace.Controls.gridDatasets

    End Class

End Namespace
