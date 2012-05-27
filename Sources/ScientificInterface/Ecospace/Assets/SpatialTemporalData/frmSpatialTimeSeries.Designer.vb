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
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSpatialTimeSeries))
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_map = New ScientificInterface.Ecospace.ucSpatialTimeSeriesMap()
            Me.m_tsMap = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnZoomMap = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnZoomData = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnZoomBoth = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnShowRefMap = New System.Windows.Forms.ToolStripButton()
            Me.m_toolbox = New ScientificInterface.Ecospace.Controls.ucSpatialTimeSeriesToolbox()
            Me.m_tsDatasets = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tscmTypes = New System.Windows.Forms.ToolStripComboBox()
            Me.m_tslData = New System.Windows.Forms.ToolStripLabel()
            Me.m_tsbnConnections = New System.Windows.Forms.ToolStripButton()
            Me.m_tslZoom = New System.Windows.Forms.ToolStripLabel()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tsMap.SuspendLayout()
            Me.m_tsDatasets.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scMain
            '
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_map)
            Me.m_scMain.Panel1.Controls.Add(Me.m_tsMap)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_toolbox)
            Me.m_scMain.Panel2.Controls.Add(Me.m_tsDatasets)
            '
            'm_map
            '
            resources.ApplyResources(Me.m_map, "m_map")
            Me.m_map.Name = "m_map"
            Me.m_map.SelectedDataset = Nothing
            Me.m_map.SelectedTimeStep = -1
            Me.m_map.ShowReferenceMap = False
            Me.m_map.UIContext = Nothing
            Me.m_map.ZoomLevel = ScientificInterface.Ecospace.ucSpatialTimeSeriesMap.eZoomLevel.Both
            '
            'm_tsMap
            '
            Me.m_tsMap.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsMap.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslZoom, Me.m_tsbnZoomMap, Me.m_tsbnZoomData, Me.m_tsbnZoomBoth, Me.ToolStripSeparator1, Me.m_tsbnShowRefMap})
            resources.ApplyResources(Me.m_tsMap, "m_tsMap")
            Me.m_tsMap.Name = "m_tsMap"
            Me.m_tsMap.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
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
            'm_tsbnShowRefMap
            '
            Me.m_tsbnShowRefMap.CheckOnClick = True
            Me.m_tsbnShowRefMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnShowRefMap.Image = Global.ScientificInterface.My.Resources.Resources.Basemap
            resources.ApplyResources(Me.m_tsbnShowRefMap, "m_tsbnShowRefMap")
            Me.m_tsbnShowRefMap.Name = "m_tsbnShowRefMap"
            '
            'm_toolbox
            '
            Me.m_toolbox.BackColor = System.Drawing.SystemColors.Window
            resources.ApplyResources(Me.m_toolbox, "m_toolbox")
            Me.m_toolbox.Name = "m_toolbox"
            Me.m_toolbox.SelectedIndex = -1
            Me.m_toolbox.SelectedTimeStep = -1
            Me.m_toolbox.UIContext = Nothing
            Me.m_toolbox.VarName = EwEUtils.Core.eVarNameFlags.NotSet
            '
            'm_tsDatasets
            '
            Me.m_tsDatasets.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsDatasets.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tscmTypes, Me.m_tslData, Me.m_tsbnConnections})
            resources.ApplyResources(Me.m_tsDatasets, "m_tsDatasets")
            Me.m_tsDatasets.Name = "m_tsDatasets"
            Me.m_tsDatasets.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tscmTypes
            '
            Me.m_tscmTypes.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tscmTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmTypes.Name = "m_tscmTypes"
            resources.ApplyResources(Me.m_tscmTypes, "m_tscmTypes")
            Me.m_tscmTypes.Sorted = True
            '
            'm_tslData
            '
            Me.m_tslData.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tslData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tslData.Name = "m_tslData"
            resources.ApplyResources(Me.m_tslData, "m_tslData")
            '
            'm_tsbnConnections
            '
            resources.ApplyResources(Me.m_tsbnConnections, "m_tsbnConnections")
            Me.m_tsbnConnections.Name = "m_tsbnConnections"
            '
            'm_tslZoom
            '
            Me.m_tslZoom.Name = "m_tslZoom"
            resources.ApplyResources(Me.m_tslZoom, "m_tslZoom")
            '
            'frmSpatialTimeSeries
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "frmSpatialTimeSeries"
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel1.PerformLayout()
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.Panel2.PerformLayout()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.m_tsMap.ResumeLayout(False)
            Me.m_tsMap.PerformLayout()
            Me.m_tsDatasets.ResumeLayout(False)
            Me.m_tsDatasets.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_toolbox As ScientificInterface.Ecospace.Controls.ucSpatialTimeSeriesToolbox
        Private WithEvents m_tsDatasets As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_tscmTypes As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tslData As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsbnConnections As System.Windows.Forms.ToolStripButton
        Private WithEvents m_map As ScientificInterface.Ecospace.ucSpatialTimeSeriesMap
        Private WithEvents m_tsMap As cEwEToolstrip
        Private WithEvents m_tsbnZoomMap As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomData As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomBoth As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbnShowRefMap As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tslZoom As System.Windows.Forms.ToolStripLabel
    End Class

End Namespace
