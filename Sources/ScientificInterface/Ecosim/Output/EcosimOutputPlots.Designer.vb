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
            Me.m_btnShowAllFits = New System.Windows.Forms.Button
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_btnSaveData = New System.Windows.Forms.Button
            Me.m_lbPredators = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_lbPrey = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_tlpMain = New System.Windows.Forms.TableLayoutPanel
            Me.m_plGroups = New System.Windows.Forms.Panel
            Me.m_lblGroup = New System.Windows.Forms.Label
            Me.m_plFleets = New System.Windows.Forms.Panel
            Me.m_tlbControllers = New System.Windows.Forms.TableLayoutPanel
            Me.m_plPredators = New System.Windows.Forms.Panel
            Me.m_lblPredators = New System.Windows.Forms.Label
            Me.m_plPrey = New System.Windows.Forms.Panel
            Me.m_lblPrey = New System.Windows.Forms.Label
            Me.m_lblFleets = New System.Windows.Forms.Label
            Me.m_lbFleets = New System.Windows.Forms.ListBox
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tlpMain.SuspendLayout()
            Me.m_plGroups.SuspendLayout()
            Me.m_plFleets.SuspendLayout()
            Me.m_tlbControllers.SuspendLayout()
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
            'm_btnShowAllFits
            '
            resources.ApplyResources(Me.m_btnShowAllFits, "m_btnShowAllFits")
            Me.m_btnShowAllFits.Name = "m_btnShowAllFits"
            Me.m_btnShowAllFits.UseVisualStyleBackColor = True
            '
            'm_lbGroups
            '
            resources.ApplyResources(Me.m_lbGroups, "m_lbGroups")
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.SortThreshold = -9999.0!
            Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.[Default]
            '
            'm_btnSaveData
            '
            resources.ApplyResources(Me.m_btnSaveData, "m_btnSaveData")
            Me.m_btnSaveData.Name = "m_btnSaveData"
            Me.m_btnSaveData.UseVisualStyleBackColor = True
            '
            'm_lbPredators
            '
            resources.ApplyResources(Me.m_lbPredators, "m_lbPredators")
            Me.m_lbPredators.BackColor = System.Drawing.SystemColors.Window
            Me.m_lbPredators.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbPredators.FormattingEnabled = True
            Me.m_lbPredators.Name = "m_lbPredators"
            Me.m_lbPredators.SortThreshold = -9999.0!
            Me.m_lbPredators.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.[Default]
            '
            'm_lbPrey
            '
            resources.ApplyResources(Me.m_lbPrey, "m_lbPrey")
            Me.m_lbPrey.BackColor = System.Drawing.SystemColors.Window
            Me.m_lbPrey.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbPrey.FormattingEnabled = True
            Me.m_lbPrey.Name = "m_lbPrey"
            Me.m_lbPrey.SortThreshold = -9999.0!
            Me.m_lbPrey.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.[Default]
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
            Me.m_tlpMain.Controls.Add(Me.m_tlbControllers, 0, 4)
            Me.m_tlpMain.Controls.Add(Me.m_plGroups, 0, 0)
            Me.m_tlpMain.Controls.Add(Me.m_plFleets, 0, 3)
            Me.m_tlpMain.Controls.Add(Me.m_plPredators, 0, 1)
            Me.m_tlpMain.Controls.Add(Me.m_plPrey, 0, 2)
            Me.m_tlpMain.Name = "m_tlpMain"
            '
            'm_plGroups
            '
            Me.m_plGroups.Controls.Add(Me.m_lblGroup)
            Me.m_plGroups.Controls.Add(Me.m_lbGroups)
            resources.ApplyResources(Me.m_plGroups, "m_plGroups")
            Me.m_plGroups.Name = "m_plGroups"
            '
            'm_lblGroup
            '
            resources.ApplyResources(Me.m_lblGroup, "m_lblGroup")
            Me.m_lblGroup.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblGroup.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblGroup.Name = "m_lblGroup"
            '
            'm_plFleets
            '
            Me.m_plFleets.Controls.Add(Me.m_lbFleets)
            Me.m_plFleets.Controls.Add(Me.m_lblFleets)
            resources.ApplyResources(Me.m_plFleets, "m_plFleets")
            Me.m_plFleets.Name = "m_plFleets"
            '
            'm_tlbControllers
            '
            resources.ApplyResources(Me.m_tlbControllers, "m_tlbControllers")
            Me.m_tlbControllers.Controls.Add(Me.m_btnSaveData, 0, 0)
            Me.m_tlbControllers.Controls.Add(Me.m_btnShowAllFits, 1, 0)
            Me.m_tlbControllers.Name = "m_tlbControllers"
            '
            'm_plPredators
            '
            Me.m_plPredators.Controls.Add(Me.m_lblPredators)
            Me.m_plPredators.Controls.Add(Me.m_lbPredators)
            resources.ApplyResources(Me.m_plPredators, "m_plPredators")
            Me.m_plPredators.Name = "m_plPredators"
            '
            'm_lblPredators
            '
            resources.ApplyResources(Me.m_lblPredators, "m_lblPredators")
            Me.m_lblPredators.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblPredators.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblPredators.Name = "m_lblPredators"
            '
            'm_plPrey
            '
            Me.m_plPrey.Controls.Add(Me.m_lblPrey)
            Me.m_plPrey.Controls.Add(Me.m_lbPrey)
            resources.ApplyResources(Me.m_plPrey, "m_plPrey")
            Me.m_plPrey.Name = "m_plPrey"
            '
            'm_lblPrey
            '
            resources.ApplyResources(Me.m_lblPrey, "m_lblPrey")
            Me.m_lblPrey.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblPrey.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblPrey.Name = "m_lblPrey"
            '
            'm_lblFleets
            '
            resources.ApplyResources(Me.m_lblFleets, "m_lblFleets")
            Me.m_lblFleets.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblFleets.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblFleets.Name = "m_lblFleets"
            '
            'm_lbFleets
            '
            resources.ApplyResources(Me.m_lbFleets, "m_lbFleets")
            Me.m_lbFleets.FormattingEnabled = True
            Me.m_lbFleets.Name = "m_lbFleets"
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
            Me.m_tlbControllers.ResumeLayout(False)
            Me.m_plPredators.ResumeLayout(False)
            Me.m_plPrey.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
        Private WithEvents m_btnShowAllFits As System.Windows.Forms.Button
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
        Private WithEvents m_tlbControllers As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblGroup As System.Windows.Forms.Label
        Private WithEvents m_lblPredators As System.Windows.Forms.Label
        Private WithEvents m_lblPrey As System.Windows.Forms.Label
        Private WithEvents m_lbFleets As System.Windows.Forms.ListBox
        Private WithEvents m_lblFleets As System.Windows.Forms.Label
    End Class

End Namespace

