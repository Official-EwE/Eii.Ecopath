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
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace

    Partial Class frmSpatialTimeSeries
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
            Dim sep1 As System.Windows.Forms.ToolStripSeparator
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSpatialTimeSeries))
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.m_tcMain = New System.Windows.Forms.TabControl()
            Me.m_tpConnections = New System.Windows.Forms.TabPage()
            Me.m_gridApply = New ScientificInterface.Ecospace.gridExternalSpatialData()
            Me.m_tpMap = New System.Windows.Forms.TabPage()
            Me.m_tsMap = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tslZoom = New System.Windows.Forms.ToolStripLabel()
            Me.m_tsbnZoomMap = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnZoomData = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnZoomBoth = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnShowGrid = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnShowRefMap = New System.Windows.Forms.ToolStripButton()
            Me.m_map = New ScientificInterface.Ecospace.ucSpatialTimeSeriesMap()
            Me.m_tsDatasets = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnConnections = New System.Windows.Forms.ToolStripButton()
            Me.m_tslbFilter = New System.Windows.Forms.ToolStripLabel()
            Me.m_tscmbLayerVariable = New System.Windows.Forms.ToolStripComboBox()
            Me.m_tsbnOnlyShowConnectedLayers = New System.Windows.Forms.ToolStripButton()
            Me.m_toolbox = New ScientificInterface.Ecospace.Controls.ucSpatialTimeSeriesToolbox()
            sep1 = New System.Windows.Forms.ToolStripSeparator()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.m_tcMain.SuspendLayout()
            Me.m_tpConnections.SuspendLayout()
            Me.m_tpMap.SuspendLayout()
            Me.m_tsMap.SuspendLayout()
            Me.m_tsDatasets.SuspendLayout()
            Me.SuspendLayout()
            '
            'sep1
            '
            sep1.Name = "sep1"
            resources.ApplyResources(sep1, "sep1")
            '
            'm_scMain
            '
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.TableLayoutPanel1)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_toolbox)
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.m_tcMain, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.m_tsDatasets, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'm_tcMain
            '
            Me.m_tcMain.Controls.Add(Me.m_tpConnections)
            Me.m_tcMain.Controls.Add(Me.m_tpMap)
            resources.ApplyResources(Me.m_tcMain, "m_tcMain")
            Me.m_tcMain.Name = "m_tcMain"
            Me.m_tcMain.SelectedIndex = 0
            '
            'm_tpConnections
            '
            Me.m_tpConnections.Controls.Add(Me.m_gridApply)
            resources.ApplyResources(Me.m_tpConnections, "m_tpConnections")
            Me.m_tpConnections.Name = "m_tpConnections"
            Me.m_tpConnections.UseVisualStyleBackColor = True
            '
            'm_gridApply
            '
            Me.m_gridApply.AllowBlockSelect = False
            Me.m_gridApply.AutoSizeMinHeight = 10
            Me.m_gridApply.AutoSizeMinWidth = 10
            Me.m_gridApply.AutoStretchColumnsToFitWidth = False
            Me.m_gridApply.AutoStretchRowsToFitHeight = False
            Me.m_gridApply.BackColor = System.Drawing.Color.White
            Me.m_gridApply.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridApply.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridApply.CustomSort = False
            Me.m_gridApply.DataName = "grid content"
            resources.ApplyResources(Me.m_gridApply, "m_gridApply")
            Me.m_gridApply.Filter = EwEUtils.Core.eVarNameFlags.NotSet
            Me.m_gridApply.FixedColumnWidths = True
            Me.m_gridApply.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridApply.GridToolTipActive = True
            Me.m_gridApply.IsLayoutSuspended = False
            Me.m_gridApply.Name = "m_gridApply"
            Me.m_gridApply.OnlyShowConnected = True
            Me.m_gridApply.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                Or SourceGrid2.GridSpecialKeys.Delete) _
                Or SourceGrid2.GridSpecialKeys.Arrows) _
                Or SourceGrid2.GridSpecialKeys.Tab) _
                Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                Or SourceGrid2.GridSpecialKeys.Enter) _
                Or SourceGrid2.GridSpecialKeys.Escape) _
                Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridApply.UIContext = Nothing
            '
            'm_tpMap
            '
            Me.m_tpMap.Controls.Add(Me.m_tsMap)
            Me.m_tpMap.Controls.Add(Me.m_map)
            resources.ApplyResources(Me.m_tpMap, "m_tpMap")
            Me.m_tpMap.Name = "m_tpMap"
            Me.m_tpMap.UseVisualStyleBackColor = True
            '
            'm_tsMap
            '
            Me.m_tsMap.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsMap.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslZoom, Me.m_tsbnZoomMap, Me.m_tsbnZoomData, Me.m_tsbnZoomBoth, Me.ToolStripSeparator1, Me.m_tsbnShowGrid, Me.m_tsbnShowRefMap})
            resources.ApplyResources(Me.m_tsMap, "m_tsMap")
            Me.m_tsMap.Name = "m_tsMap"
            Me.m_tsMap.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tslZoom
            '
            Me.m_tslZoom.Name = "m_tslZoom"
            resources.ApplyResources(Me.m_tslZoom, "m_tslZoom")
            '
            'm_tsbnZoomMap
            '
            Me.m_tsbnZoomMap.CheckOnClick = True
            Me.m_tsbnZoomMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnZoomMap, "m_tsbnZoomMap")
            Me.m_tsbnZoomMap.Name = "m_tsbnZoomMap"
            '
            'm_tsbnZoomData
            '
            Me.m_tsbnZoomData.CheckOnClick = True
            Me.m_tsbnZoomData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnZoomData, "m_tsbnZoomData")
            Me.m_tsbnZoomData.Name = "m_tsbnZoomData"
            '
            'm_tsbnZoomBoth
            '
            Me.m_tsbnZoomBoth.Checked = True
            Me.m_tsbnZoomBoth.CheckOnClick = True
            Me.m_tsbnZoomBoth.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsbnZoomBoth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnZoomBoth, "m_tsbnZoomBoth")
            Me.m_tsbnZoomBoth.Name = "m_tsbnZoomBoth"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'm_tsbnShowGrid
            '
            Me.m_tsbnShowGrid.CheckOnClick = True
            Me.m_tsbnShowGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnShowGrid, "m_tsbnShowGrid")
            Me.m_tsbnShowGrid.Name = "m_tsbnShowGrid"
            '
            'm_tsbnShowRefMap
            '
            Me.m_tsbnShowRefMap.CheckOnClick = True
            Me.m_tsbnShowRefMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnShowRefMap, "m_tsbnShowRefMap")
            Me.m_tsbnShowRefMap.Name = "m_tsbnShowRefMap"
            '
            'm_map
            '
            resources.ApplyResources(Me.m_map, "m_map")
            Me.m_map.MapExtent = CType(resources.GetObject("m_map.MapExtent"), System.Drawing.RectangleF)
            Me.m_map.MapSize = New System.Drawing.Size(10, 10)
            Me.m_map.Name = "m_map"
            Me.m_map.SelectedDataset = Nothing
            Me.m_map.SelectedTimeStep = 1
            Me.m_map.ShowGrid = False
            Me.m_map.ShowLabels = False
            Me.m_map.ShowReferenceMap = False
            Me.m_map.UIContext = Nothing
            Me.m_map.UseBuiltInReferenceMap = False
            Me.m_map.ZoomLevel = ScientificInterface.Ecospace.ucSpatialTimeSeriesMap.eZoomLevel.Both
            '
            'm_tsDatasets
            '
            Me.m_tsDatasets.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsDatasets.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnConnections, sep1, Me.m_tslbFilter, Me.m_tscmbLayerVariable, Me.m_tsbnOnlyShowConnectedLayers})
            resources.ApplyResources(Me.m_tsDatasets, "m_tsDatasets")
            Me.m_tsDatasets.Name = "m_tsDatasets"
            Me.m_tsDatasets.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnConnections
            '
            resources.ApplyResources(Me.m_tsbnConnections, "m_tsbnConnections")
            Me.m_tsbnConnections.Name = "m_tsbnConnections"
            '
            'm_tslbFilter
            '
            Me.m_tslbFilter.Name = "m_tslbFilter"
            resources.ApplyResources(Me.m_tslbFilter, "m_tslbFilter")
            '
            'm_tscmbLayerVariable
            '
            Me.m_tscmbLayerVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmbLayerVariable.Name = "m_tscmbLayerVariable"
            resources.ApplyResources(Me.m_tscmbLayerVariable, "m_tscmbLayerVariable")
            Me.m_tscmbLayerVariable.Sorted = True
            '
            'm_tsbnOnlyShowConnectedLayers
            '
            Me.m_tsbnOnlyShowConnectedLayers.CheckOnClick = True
            Me.m_tsbnOnlyShowConnectedLayers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnOnlyShowConnectedLayers, "m_tsbnOnlyShowConnectedLayers")
            Me.m_tsbnOnlyShowConnectedLayers.Name = "m_tsbnOnlyShowConnectedLayers"
            '
            'm_toolbox
            '
            Me.m_toolbox.BackColor = System.Drawing.SystemColors.Window
            resources.ApplyResources(Me.m_toolbox, "m_toolbox")
            Me.m_toolbox.Filter = EwEUtils.Core.eVarNameFlags.NotSet
            Me.m_toolbox.Name = "m_toolbox"
            Me.m_toolbox.SelectedDatasetIndex = -1
            Me.m_toolbox.SelectedTimeStep = -1
            Me.m_toolbox.UIContext = Nothing
            '
            'frmSpatialTimeSeries
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "frmSpatialTimeSeries"
            Me.ShowInTaskbar = False
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.m_tcMain.ResumeLayout(False)
            Me.m_tpConnections.ResumeLayout(False)
            Me.m_tpMap.ResumeLayout(False)
            Me.m_tpMap.PerformLayout()
            Me.m_tsMap.ResumeLayout(False)
            Me.m_tsMap.PerformLayout()
            Me.m_tsDatasets.ResumeLayout(False)
            Me.m_tsDatasets.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_toolbox As ScientificInterface.Ecospace.Controls.ucSpatialTimeSeriesToolbox
        Private WithEvents m_tsDatasets As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_tscmbLayerVariable As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tslbFilter As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_map As ScientificInterface.Ecospace.ucSpatialTimeSeriesMap
        Private WithEvents m_tsMap As cEwEToolstrip
        Private WithEvents m_tsbnZoomMap As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomData As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomBoth As System.Windows.Forms.ToolStripButton
        Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbnShowRefMap As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tslZoom As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsbnShowGrid As System.Windows.Forms.ToolStripButton
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tcMain As System.Windows.Forms.TabControl
        Private WithEvents m_tpConnections As System.Windows.Forms.TabPage
        Private WithEvents m_tpMap As System.Windows.Forms.TabPage
        Private WithEvents m_gridApply As gridExternalSpatialData
        Private WithEvents m_tsbnOnlyShowConnectedLayers As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnConnections As System.Windows.Forms.ToolStripButton
    End Class

End Namespace
