Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmMSERecruitment
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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSERecruitment))
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_graph = New ZedGraph.ZedGraphControl
            Me.tsToolStrip = New System.Windows.Forms.ToolStrip
            Me.tsbtDefaults = New System.Windows.Forms.ToolStripButton
            Me.m_grid = New ScientificInterface.Ecosim.gridMSERecruitment
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.tsToolStrip.SuspendLayout()
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
            Me.m_scMain.Panel1.Controls.Add(Me.m_graph)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.tsToolStrip)
            Me.m_scMain.Panel2.Controls.Add(Me.m_grid)
            '
            'm_graph
            '
            resources.ApplyResources(Me.m_graph, "m_graph")
            Me.m_graph.EditModifierKeys = System.Windows.Forms.Keys.None
            Me.m_graph.Name = "m_graph"
            Me.m_graph.ScrollGrace = 0
            Me.m_graph.ScrollMaxX = 0
            Me.m_graph.ScrollMaxY = 0
            Me.m_graph.ScrollMaxY2 = 0
            Me.m_graph.ScrollMinX = 0
            Me.m_graph.ScrollMinY = 0
            Me.m_graph.ScrollMinY2 = 0
            Me.m_graph.ZoomButtons = System.Windows.Forms.MouseButtons.None
            '
            'tsToolStrip
            '
            Me.tsToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbtDefaults})
            resources.ApplyResources(Me.tsToolStrip, "tsToolStrip")
            Me.tsToolStrip.Name = "tsToolStrip"
            '
            'tsbtDefaults
            '
            Me.tsbtDefaults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbtDefaults, "tsbtDefaults")
            Me.tsbtDefaults.Name = "tsbtDefaults"
            '
            'm_grid
            '
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_grid.CustomSort = False
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.Group = Nothing
            Me.m_grid.Name = "m_grid"
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_grid.TrackPropertySelection = True
            Me.m_grid.UIContext = Nothing
            '
            'frmMSERecruitment
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "frmMSERecruitment"
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.Panel2.PerformLayout()
            Me.m_scMain.ResumeLayout(False)
            Me.tsToolStrip.ResumeLayout(False)
            Me.tsToolStrip.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Private WithEvents m_grid As Ecosim.gridMSERecruitment
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
        Friend WithEvents tsToolStrip As System.Windows.Forms.ToolStrip
        Friend WithEvents tsbtDefaults As System.Windows.Forms.ToolStripButton

    End Class

End Namespace
