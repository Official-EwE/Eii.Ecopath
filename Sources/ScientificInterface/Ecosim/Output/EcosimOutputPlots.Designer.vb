Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcosimOutputPlots
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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcosimOutputPlots))
            Me.m_graph = New ZedGraph.ZedGraphControl
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_btnSaveData = New System.Windows.Forms.Button
            Me.m_lbPredators = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_lbPrey = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_tlpMain = New System.Windows.Forms.TableLayoutPanel
            Me.m_plGroups = New System.Windows.Forms.Panel
            Me.m_hdrGroup = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_plFleets = New System.Windows.Forms.Panel
            Me.m_lbFleets = New ScientificInterfaceShared.Controls.cFleetListBox
            Me.m_hdrFleets = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_plPredators = New System.Windows.Forms.Panel
            Me.m_hdrPredators = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_plPrey = New System.Windows.Forms.Panel
            Me.m_hdrPrey = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tlpMain.SuspendLayout()
            Me.m_plGroups.SuspendLayout()
            Me.m_plFleets.SuspendLayout()
            Me.m_plPredators.SuspendLayout()
            Me.m_plPrey.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_graph
            '
            Me.m_graph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_graph, "m_graph")
            Me.m_graph.Name = "m_graph"
            Me.m_graph.ScrollGrace = 0
            Me.m_graph.ScrollMaxX = 0
            Me.m_graph.ScrollMaxY = 0
            Me.m_graph.ScrollMaxY2 = 0
            Me.m_graph.ScrollMinX = 0
            Me.m_graph.ScrollMinY = 0
            Me.m_graph.ScrollMinY2 = 0
            '
            'm_lbGroups
            '
            Me.m_lbGroups.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_lbGroups.AllGroupsItemText = "(All)"
            resources.ApplyResources(Me.m_lbGroups, "m_lbGroups")
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayAlways
            Me.m_lbGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.LivingGroups
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.SelectedGroup = Nothing
            Me.m_lbGroups.SelectedGroupIndex = -1
            Me.m_lbGroups.ShowAllGroupsItem = False
            Me.m_lbGroups.SortThreshold = -9999.0!
            Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.GroupIndexAsc
            '
            'm_btnSaveData
            '
            resources.ApplyResources(Me.m_btnSaveData, "m_btnSaveData")
            Me.m_btnSaveData.Name = "m_btnSaveData"
            Me.m_btnSaveData.UseVisualStyleBackColor = True
            '
            'm_lbPredators
            '
            Me.m_lbPredators.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_lbPredators.AllGroupsItemText = "(All)"
            resources.ApplyResources(Me.m_lbPredators, "m_lbPredators")
            Me.m_lbPredators.BackColor = System.Drawing.SystemColors.Window
            Me.m_lbPredators.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbPredators.FormattingEnabled = True
            Me.m_lbPredators.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayAlways
            Me.m_lbPredators.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.AllGroups
            Me.m_lbPredators.Name = "m_lbPredators"
            Me.m_lbPredators.SelectedGroup = Nothing
            Me.m_lbPredators.SelectedGroupIndex = -1
            Me.m_lbPredators.ShowAllGroupsItem = False
            Me.m_lbPredators.SortThreshold = -9999.0!
            Me.m_lbPredators.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.ValueAsc
            '
            'm_lbPrey
            '
            Me.m_lbPrey.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_lbPrey.AllGroupsItemText = "(All)"
            resources.ApplyResources(Me.m_lbPrey, "m_lbPrey")
            Me.m_lbPrey.BackColor = System.Drawing.SystemColors.Window
            Me.m_lbPrey.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbPrey.FormattingEnabled = True
            Me.m_lbPrey.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayAlways
            Me.m_lbPrey.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.AllGroups
            Me.m_lbPrey.Name = "m_lbPrey"
            Me.m_lbPrey.SelectedGroup = Nothing
            Me.m_lbPrey.SelectedGroupIndex = -1
            Me.m_lbPrey.ShowAllGroupsItem = False
            Me.m_lbPrey.SortThreshold = -9999.0!
            Me.m_lbPrey.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.ValueAsc
            '
            'm_scMain
            '
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.MinimumSize = New System.Drawing.Size(400, 400)
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
            Me.m_tlpMain.Controls.Add(Me.m_btnSaveData, 0, 4)
            Me.m_tlpMain.Controls.Add(Me.m_plGroups, 0, 0)
            Me.m_tlpMain.Controls.Add(Me.m_plFleets, 0, 3)
            Me.m_tlpMain.Controls.Add(Me.m_plPredators, 0, 1)
            Me.m_tlpMain.Controls.Add(Me.m_plPrey, 0, 2)
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
            Me.m_hdrGroup.Name = "m_hdrGroup"
            '
            'm_plFleets
            '
            Me.m_plFleets.Controls.Add(Me.m_lbFleets)
            Me.m_plFleets.Controls.Add(Me.m_hdrFleets)
            resources.ApplyResources(Me.m_plFleets, "m_plFleets")
            Me.m_plFleets.Name = "m_plFleets"
            '
            'm_lbFleets
            '
            resources.ApplyResources(Me.m_lbFleets, "m_lbFleets")
            Me.m_lbFleets.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbFleets.FleetListTracking = ScientificInterfaceShared.Controls.cFleetListBox.eFleetTrackingType.Manual
            Me.m_lbFleets.FormattingEnabled = True
            Me.m_lbFleets.Name = "m_lbFleets"
            Me.m_lbFleets.SelectedFleet = Nothing
            Me.m_lbFleets.SelectedFleetIndex = -1
            Me.m_lbFleets.SortThreshold = -9999.0!
            Me.m_lbFleets.SortType = ScientificInterfaceShared.Controls.cFleetListBox.eSortType.FleetIndexAsc
            '
            'm_hdrFleets
            '
            resources.ApplyResources(Me.m_hdrFleets, "m_hdrFleets")
            Me.m_hdrFleets.Name = "m_hdrFleets"
            '
            'm_plPredators
            '
            Me.m_plPredators.Controls.Add(Me.m_hdrPredators)
            Me.m_plPredators.Controls.Add(Me.m_lbPredators)
            resources.ApplyResources(Me.m_plPredators, "m_plPredators")
            Me.m_plPredators.Name = "m_plPredators"
            '
            'm_hdrPredators
            '
            resources.ApplyResources(Me.m_hdrPredators, "m_hdrPredators")
            Me.m_hdrPredators.Name = "m_hdrPredators"
            '
            'm_plPrey
            '
            Me.m_plPrey.Controls.Add(Me.m_hdrPrey)
            Me.m_plPrey.Controls.Add(Me.m_lbPrey)
            resources.ApplyResources(Me.m_plPrey, "m_plPrey")
            Me.m_plPrey.Name = "m_plPrey"
            '
            'm_hdrPrey
            '
            resources.ApplyResources(Me.m_hdrPrey, "m_hdrPrey")
            Me.m_hdrPrey.Name = "m_hdrPrey"
            '
            'EcosimOutputPlots
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "EcosimOutputPlots"
            Me.ShowIcon = False
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            Me.m_tlpMain.ResumeLayout(False)
            Me.m_plGroups.ResumeLayout(False)
            Me.m_plFleets.ResumeLayout(False)
            Me.m_plPredators.ResumeLayout(False)
            Me.m_plPrey.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
        Private WithEvents m_lbGroups As cGroupListBox
        Private WithEvents m_btnSaveData As System.Windows.Forms.Button
        Private WithEvents m_lbPredators As cGroupListBox
        Private WithEvents m_lbPrey As cGroupListBox
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_tlpMain As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plGroups As System.Windows.Forms.Panel
        Private WithEvents m_plFleets As System.Windows.Forms.Panel
        Private WithEvents m_plPredators As System.Windows.Forms.Panel
        Private WithEvents m_plPrey As System.Windows.Forms.Panel
        Private WithEvents m_hdrGroup As cEwEHeaderLabel
        Private WithEvents m_hdrPredators As cEwEHeaderLabel
        Private WithEvents m_hdrPrey As cEwEHeaderLabel
        Private WithEvents m_lbFleets As cFleetListBox
        Private WithEvents m_hdrFleets As cEwEHeaderLabel
    End Class

End Namespace

