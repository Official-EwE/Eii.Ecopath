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
Imports ScientificInterfaceShared.Controls

Namespace Ecosim

    Partial Class frmEcosimOutputPlotsByParm
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
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcosimOutputPlotsByParm))
            Me.m_graph = New ZedGraph.ZedGraphControl()
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox()
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_tlpMain = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plGroups = New System.Windows.Forms.Panel()
            Me.m_hdrGroup = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_plFleets = New System.Windows.Forms.Panel()
            Me.m_plPredators = New System.Windows.Forms.Panel()
            Me.m_lbParameter = New System.Windows.Forms.ListBox()
            Me.m_hdrPredators = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_plPrey = New System.Windows.Forms.Panel()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tlpMain.SuspendLayout()
            Me.m_plGroups.SuspendLayout()
            Me.m_plPredators.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_graph
            '
            Me.m_graph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_graph, "m_graph")
            Me.m_graph.Name = "m_graph"
            Me.m_graph.ScrollGrace = 0.0R
            Me.m_graph.ScrollMaxX = 0.0R
            Me.m_graph.ScrollMaxY = 0.0R
            Me.m_graph.ScrollMaxY2 = 0.0R
            Me.m_graph.ScrollMinX = 0.0R
            Me.m_graph.ScrollMinY = 0.0R
            Me.m_graph.ScrollMinY2 = 0.0R
            '
            'm_lbGroups
            '
            Me.m_lbGroups.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_lbGroups.AllGroupsItemText = "(All)"
            resources.ApplyResources(Me.m_lbGroups, "m_lbGroups")
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.LivingGroups
            Me.m_lbGroups.IsAllGroupsItemSelected = False
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.SelectedGroup = Nothing
            Me.m_lbGroups.SelectedGroupIndex = -1
            Me.m_lbGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            Me.m_lbGroups.ShowAllGroupsItem = False
            Me.m_lbGroups.SortThreshold = -9999.0!
            '
            'm_scMain
            '
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_graph)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tlpMain)
            '
            'm_tlpMain
            '
            resources.ApplyResources(Me.m_tlpMain, "m_tlpMain")
            Me.m_tlpMain.Controls.Add(Me.m_plGroups, 0, 1)
            Me.m_tlpMain.Controls.Add(Me.m_plFleets, 0, 4)
            Me.m_tlpMain.Controls.Add(Me.m_plPredators, 0, 2)
            Me.m_tlpMain.Controls.Add(Me.m_plPrey, 0, 3)
            Me.m_tlpMain.Name = "m_tlpMain"
            '
            'm_plGroups
            '
            Me.m_plGroups.Controls.Add(Me.m_hdrGroup)
            Me.m_plGroups.Controls.Add(Me.m_lbGroups)
            resources.ApplyResources(Me.m_plGroups, "m_plGroups")
            Me.m_plGroups.Name = "m_plGroups"
            '
            'm_hdrGroup
            '
            resources.ApplyResources(Me.m_hdrGroup, "m_hdrGroup")
            Me.m_hdrGroup.CanCollapseParent = False
            Me.m_hdrGroup.CollapsedParentHeight = 0
            Me.m_hdrGroup.IsCollapsed = False
            Me.m_hdrGroup.Name = "m_hdrGroup"
            '
            'm_plFleets
            '
            resources.ApplyResources(Me.m_plFleets, "m_plFleets")
            Me.m_plFleets.Name = "m_plFleets"
            '
            'm_plPredators
            '
            Me.m_plPredators.Controls.Add(Me.m_lbParameter)
            Me.m_plPredators.Controls.Add(Me.m_hdrPredators)
            resources.ApplyResources(Me.m_plPredators, "m_plPredators")
            Me.m_plPredators.Name = "m_plPredators"
            '
            'm_lbParameter
            '
            resources.ApplyResources(Me.m_lbParameter, "m_lbParameter")
            Me.m_lbParameter.FormattingEnabled = True
            Me.m_lbParameter.Name = "m_lbParameter"
            '
            'm_hdrPredators
            '
            resources.ApplyResources(Me.m_hdrPredators, "m_hdrPredators")
            Me.m_hdrPredators.CanCollapseParent = False
            Me.m_hdrPredators.CollapsedParentHeight = 0
            Me.m_hdrPredators.IsCollapsed = False
            Me.m_hdrPredators.Name = "m_hdrPredators"
            '
            'm_plPrey
            '
            resources.ApplyResources(Me.m_plPrey, "m_plPrey")
            Me.m_plPrey.Name = "m_plPrey"
            '
            'frmEcosimOutputPlotsByParm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "frmEcosimOutputPlotsByParm"
            Me.ShowIcon = False
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.m_tlpMain.ResumeLayout(False)
            Me.m_plGroups.ResumeLayout(False)
            Me.m_plPredators.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
        Private WithEvents m_lbGroups As cGroupListBox
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_tlpMain As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plGroups As System.Windows.Forms.Panel
        Private WithEvents m_plFleets As System.Windows.Forms.Panel
        Private WithEvents m_plPredators As System.Windows.Forms.Panel
        Private WithEvents m_plPrey As System.Windows.Forms.Panel
        Private WithEvents m_hdrGroup As cEwEHeaderLabel
        Private WithEvents m_hdrPredators As cEwEHeaderLabel
        Private WithEvents m_lbParameter As System.Windows.Forms.ListBox
    End Class

End Namespace

