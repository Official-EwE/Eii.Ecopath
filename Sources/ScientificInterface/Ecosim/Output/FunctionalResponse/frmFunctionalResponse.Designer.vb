Imports ScientificInterfaceShared.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmFunctionalResponsePlot
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFunctionalResponsePlot))
            Me.m_plot = New ZedGraph.ZedGraphControl
            Me.m_lbPrey = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_hdrPrey = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.m_tsbnShowGroups = New System.Windows.Forms.ToolStripButton
            Me.m_sep = New System.Windows.Forms.ToolStripSeparator
            Me.m_tslConsumers = New System.Windows.Forms.ToolStripLabel
            Me.m_tscmConsumers = New System.Windows.Forms.ToolStripComboBox
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_plot
            '
            Me.m_plot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_plot, "m_plot")
            Me.m_plot.Name = "m_plot"
            Me.m_plot.ScrollGrace = 0
            Me.m_plot.ScrollMaxX = 0
            Me.m_plot.ScrollMaxY = 0
            Me.m_plot.ScrollMaxY2 = 0
            Me.m_plot.ScrollMinX = 0
            Me.m_plot.ScrollMinY = 0
            Me.m_plot.ScrollMinY2 = 0
            '
            'm_lbPrey
            '
            Me.m_lbPrey.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_lbPrey.AllGroupsItemText = "(All)"
            resources.ApplyResources(Me.m_lbPrey, "m_lbPrey")
            Me.m_lbPrey.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbPrey.FormattingEnabled = True
            Me.m_lbPrey.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayVisibleOnly
            Me.m_lbPrey.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.Manual
            Me.m_lbPrey.Name = "m_lbPrey"
            Me.m_lbPrey.SelectedGroup = Nothing
            Me.m_lbPrey.SelectedGroupIndex = -1
            Me.m_lbPrey.ShowAllGroupsItem = False
            Me.m_lbPrey.SortThreshold = -9999.0!
            Me.m_lbPrey.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.ValueDesc
            '
            'm_scMain
            '
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_plot)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_hdrPrey)
            Me.m_scMain.Panel2.Controls.Add(Me.m_lbPrey)
            '
            'm_hdrPrey
            '
            resources.ApplyResources(Me.m_hdrPrey, "m_hdrPrey")
            Me.m_hdrPrey.Name = "m_hdrPrey"
            '
            'm_ts
            '
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnShowGroups, Me.m_sep, Me.m_tslConsumers, Me.m_tscmConsumers})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            '
            'm_tsbnShowGroups
            '
            resources.ApplyResources(Me.m_tsbnShowGroups, "m_tsbnShowGroups")
            Me.m_tsbnShowGroups.Name = "m_tsbnShowGroups"
            '
            'm_sep
            '
            Me.m_sep.Name = "m_sep"
            resources.ApplyResources(Me.m_sep, "m_sep")
            '
            'm_tslConsumers
            '
            Me.m_tslConsumers.Name = "m_tslConsumers"
            resources.ApplyResources(Me.m_tslConsumers, "m_tslConsumers")
            '
            'm_tscmConsumers
            '
            resources.ApplyResources(Me.m_tscmConsumers, "m_tscmConsumers")
            Me.m_tscmConsumers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmConsumers.Name = "m_tscmConsumers"
            '
            'frmFunctionalResponsePlot
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_ts)
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "frmFunctionalResponsePlot"
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_plot As ZedGraph.ZedGraphControl
        Private WithEvents m_lbPrey As ScientificInterfaceShared.Controls.cGroupListBox
        Private WithEvents m_hdrPrey As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_tslConsumers As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tscmConsumers As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tsbnShowGroups As System.Windows.Forms.ToolStripButton
        Private WithEvents m_sep As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
    End Class

End Namespace
