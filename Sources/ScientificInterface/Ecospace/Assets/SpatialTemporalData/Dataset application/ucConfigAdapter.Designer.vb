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
            Me.m_plConnection = New System.Windows.Forms.Panel()
            Me.m_gridDatasets = New ScientificInterface.Ecospace.Controls.gridDatasets()
            Me.m_lblNewDS = New System.Windows.Forms.Label()
            Me.m_btnConfigDS = New System.Windows.Forms.Button()
            Me.m_hdrSource = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_plScalarAdapter = New System.Windows.Forms.Panel()
            Me.m_btnCalculate = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.m_tbxScale = New System.Windows.Forms.TextBox()
            Me.m_rbRelative = New System.Windows.Forms.RadioButton()
            Me.m_rbAbsolute = New System.Windows.Forms.RadioButton()
            Me.m_plConversion = New System.Windows.Forms.Panel()
            Me.m_cmbConverter = New System.Windows.Forms.ComboBox()
            Me.m_lblSelectCV = New System.Windows.Forms.Label()
            Me.m_btnConfigCV = New System.Windows.Forms.Button()
            Me.m_lbSlots = New System.Windows.Forms.ListBox()
            Me.m_hdrConnections = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_plConnections = New System.Windows.Forms.Panel()
            Me.m_tlpContent.SuspendLayout()
            Me.m_plConnection.SuspendLayout()
            Me.m_plScalarAdapter.SuspendLayout()
            Me.m_plConversion.SuspendLayout()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_plConnections.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpContent
            '
            resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
            Me.m_tlpContent.Controls.Add(Me.m_plConnection, 0, 0)
            Me.m_tlpContent.Controls.Add(Me.m_plScalarAdapter, 0, 2)
            Me.m_tlpContent.Controls.Add(Me.m_plConversion, 0, 1)
            Me.m_tlpContent.Name = "m_tlpContent"
            '
            'm_plConnection
            '
            Me.m_plConnection.Controls.Add(Me.m_gridDatasets)
            Me.m_plConnection.Controls.Add(Me.m_lblNewDS)
            Me.m_plConnection.Controls.Add(Me.m_btnConfigDS)
            Me.m_plConnection.Controls.Add(Me.m_hdrSource)
            resources.ApplyResources(Me.m_plConnection, "m_plConnection")
            Me.m_plConnection.Name = "m_plConnection"
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
            'm_lblNewDS
            '
            resources.ApplyResources(Me.m_lblNewDS, "m_lblNewDS")
            Me.m_lblNewDS.Name = "m_lblNewDS"
            '
            'm_btnConfigDS
            '
            resources.ApplyResources(Me.m_btnConfigDS, "m_btnConfigDS")
            Me.m_btnConfigDS.Name = "m_btnConfigDS"
            Me.m_btnConfigDS.UseVisualStyleBackColor = True
            '
            'm_hdrSource
            '
            Me.m_hdrSource.CanCollapseParent = False
            Me.m_hdrSource.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrSource, "m_hdrSource")
            Me.m_hdrSource.IsCollapsed = False
            Me.m_hdrSource.Name = "m_hdrSource"
            '
            'm_plScalarAdapter
            '
            Me.m_plScalarAdapter.Controls.Add(Me.m_btnCalculate)
            Me.m_plScalarAdapter.Controls.Add(Me.Label1)
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
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
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
            'm_plConversion
            '
            Me.m_plConversion.Controls.Add(Me.m_cmbConverter)
            Me.m_plConversion.Controls.Add(Me.m_lblSelectCV)
            Me.m_plConversion.Controls.Add(Me.m_btnConfigCV)
            resources.ApplyResources(Me.m_plConversion, "m_plConversion")
            Me.m_plConversion.Name = "m_plConversion"
            '
            'm_cmbConverter
            '
            resources.ApplyResources(Me.m_cmbConverter, "m_cmbConverter")
            Me.m_cmbConverter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbConverter.FormattingEnabled = True
            Me.m_cmbConverter.Name = "m_cmbConverter"
            '
            'm_lblSelectCV
            '
            resources.ApplyResources(Me.m_lblSelectCV, "m_lblSelectCV")
            Me.m_lblSelectCV.Name = "m_lblSelectCV"
            '
            'm_btnConfigCV
            '
            resources.ApplyResources(Me.m_btnConfigCV, "m_btnConfigCV")
            Me.m_btnConfigCV.Name = "m_btnConfigCV"
            Me.m_btnConfigCV.UseVisualStyleBackColor = True
            '
            'm_lbSlots
            '
            resources.ApplyResources(Me.m_lbSlots, "m_lbSlots")
            Me.m_lbSlots.FormattingEnabled = True
            Me.m_lbSlots.Items.AddRange(New Object() {resources.GetString("m_lbSlots.Items"), resources.GetString("m_lbSlots.Items1"), resources.GetString("m_lbSlots.Items2"), resources.GetString("m_lbSlots.Items3"), resources.GetString("m_lbSlots.Items4"), resources.GetString("m_lbSlots.Items5")})
            Me.m_lbSlots.Name = "m_lbSlots"
            '
            'm_hdrConnections
            '
            Me.m_hdrConnections.CanCollapseParent = False
            Me.m_hdrConnections.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrConnections, "m_hdrConnections")
            Me.m_hdrConnections.IsCollapsed = False
            Me.m_hdrConnections.Name = "m_hdrConnections"
            '
            'm_scMain
            '
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_plConnections)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tlpContent)
            '
            'm_plConnections
            '
            Me.m_plConnections.Controls.Add(Me.m_hdrConnections)
            Me.m_plConnections.Controls.Add(Me.m_lbSlots)
            resources.ApplyResources(Me.m_plConnections, "m_plConnections")
            Me.m_plConnections.Name = "m_plConnections"
            '
            'ucConfigAdapter
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "ucConfigAdapter"
            Me.m_tlpContent.ResumeLayout(False)
            Me.m_plConnection.ResumeLayout(False)
            Me.m_plConnection.PerformLayout()
            Me.m_plScalarAdapter.ResumeLayout(False)
            Me.m_plScalarAdapter.PerformLayout()
            Me.m_plConversion.ResumeLayout(False)
            Me.m_plConversion.PerformLayout()
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.m_plConnections.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plConnection As System.Windows.Forms.Panel
        Private WithEvents m_cmbConverter As System.Windows.Forms.ComboBox
        Private WithEvents m_btnConfigCV As System.Windows.Forms.Button
        Private WithEvents m_lblSelectCV As System.Windows.Forms.Label
        Private WithEvents m_lblNewDS As System.Windows.Forms.Label
        Private WithEvents m_hdrSource As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_plScalarAdapter As System.Windows.Forms.Panel
        Private WithEvents m_btnCalculate As System.Windows.Forms.Button
        Private WithEvents m_tbxScale As System.Windows.Forms.TextBox
        Private WithEvents m_rbRelative As System.Windows.Forms.RadioButton
        Private WithEvents m_rbAbsolute As System.Windows.Forms.RadioButton
        Friend WithEvents m_gridDatasets As ScientificInterface.Ecospace.Controls.gridDatasets
        Private WithEvents m_lbSlots As System.Windows.Forms.ListBox
        Private WithEvents m_hdrConnections As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_btnConfigDS As System.Windows.Forms.Button
        Private WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_plConversion As System.Windows.Forms.Panel
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_plConnections As System.Windows.Forms.Panel

    End Class

End Namespace
