Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

'<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>

Partial Class frmBiomassLimits
    Inherits frmEwEGrid

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBiomassLimits))
        Me.m_grid = New EwEMSEPlugin.gridBiomassLimits()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnSave = New System.Windows.Forms.Button()
        Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.SuspendLayout()
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = False
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
        Me.m_grid.Data = Nothing
        Me.m_grid.DataName = "grid content"
        Me.m_grid.FixedColumnWidths = False
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.IsLayoutSuspended = False
        Me.m_grid.IsOutputGrid = True
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
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnSave
        '
        resources.ApplyResources(Me.m_btnSave, "m_btnSave")
        Me.m_btnSave.Name = "m_btnSave"
        Me.m_btnSave.UseVisualStyleBackColor = True
        '
        'm_ts
        '
        Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        resources.ApplyResources(Me.m_ts, "m_ts")
        Me.m_ts.Name = "m_ts"
        Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'frmBiomassLimits
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_ts)
        Me.Controls.Add(Me.m_btnSave)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_grid)
        Me.Name = "frmBiomassLimits"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents m_grid As gridBiomassLimits
    Friend WithEvents m_btnCancel As Button
    Friend WithEvents m_btnSave As Button
    Friend WithEvents m_ts As cEwEToolstrip
End Class
