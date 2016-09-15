Partial Class frmUI

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUI))
        Me.m_ts = New System.Windows.Forms.ToolStrip()
        Me.m_tsbnPopulate = New System.Windows.Forms.ToolStripButton()
        Me.m_grid = New FishMIRresultWriter.gridUI()
        Me.m_tsbnAutosave = New System.Windows.Forms.ToolStripButton()
        Me.m_ts.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_ts
        '
        Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnAutosave, Me.m_tsbnPopulate})
        resources.ApplyResources(Me.m_ts, "m_ts")
        Me.m_ts.Name = "m_ts"
        '
        'm_tsbnPopulate
        '
        Me.m_tsbnPopulate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnPopulate, "m_tsbnPopulate")
        Me.m_tsbnPopulate.Name = "m_tsbnPopulate"
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = True
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
        Me.m_grid.DataName = "grid content"
        resources.ApplyResources(Me.m_grid, "m_grid")
        Me.m_grid.FixedColumnWidths = False
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.IsLayoutSuspended = False
        Me.m_grid.IsOutputGrid = True
        Me.m_grid.Name = "m_grid"
        Me.m_grid.Plugin = Nothing
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
        'm_tsbnAutosave
        '
        Me.m_tsbnAutosave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnAutosave, "m_tsbnAutosave")
        Me.m_tsbnAutosave.Name = "m_tsbnAutosave"
        '
        'frmUI
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_grid)
        Me.Controls.Add(Me.m_ts)
        Me.Name = "frmUI"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.m_ts.ResumeLayout(False)
        Me.m_ts.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_ts As Windows.Forms.ToolStrip
    Private WithEvents m_tsbnPopulate As Windows.Forms.ToolStripButton
    Private WithEvents m_grid As gridUI
    Private WithEvents m_tsbnAutosave As Windows.Forms.ToolStripButton
End Class
