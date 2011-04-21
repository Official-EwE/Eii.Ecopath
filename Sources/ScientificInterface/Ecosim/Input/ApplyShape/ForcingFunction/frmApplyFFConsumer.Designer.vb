Imports ScientificInterfaceShared.Forms

Namespace Ecosim
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    <CLSCompliant(False)> _
    Partial Class frmApplyFFConsumer
        Inherits frmApplyShapeBase

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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmApplyFFConsumer))
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.tsBtnClearAll = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnSetAll = New System.Windows.Forms.ToolStripButton
            Me.m_grid = New ScientificInterface.Ecosim.ApplyPredPreyShapeGrid
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnClearAll, Me.ToolStripSeparator1, Me.tsBtnSetAll})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'tsBtnClearAll
            '
            Me.tsBtnClearAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnClearAll, "tsBtnClearAll")
            Me.tsBtnClearAll.Name = "tsBtnClearAll"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'tsBtnSetAll
            '
            Me.tsBtnSetAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnSetAll, "tsBtnSetAll")
            Me.tsBtnSetAll.Name = "tsBtnSetAll"
            '
            'm_grid
            '
            Me.m_grid.AllowBlockSelect = True
            Me.m_grid.ApplyShapeMode = ScientificInterfaceShared.Definitions.eApplyShapeTypes.Forcing
            Me.m_grid.ApplyTargetMode = ScientificInterfaceShared.Definitions.eApplyTargetTypes.Consumer
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
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
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
            Me.m_grid.UIContext = Nothing
            '
            'frmApplyFFConsumer
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_ts)
            Me.Name = "frmApplyFFConsumer"
            Me.TabText = "Apply FF (cons)"
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents tsBtnClearAll As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsBtnSetAll As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents m_grid As ScientificInterface.Ecosim.ApplyPredPreyShapeGrid

    End Class
End Namespace

