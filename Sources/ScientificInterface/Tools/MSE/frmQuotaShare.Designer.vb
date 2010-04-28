Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmQuotaShare
        Inherits frmEwEGrid

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmQuotaShare))
            Me.m_tss = New System.Windows.Forms.ToolStrip
            Me.m_tsbnDefaults = New System.Windows.Forms.ToolStripButton
            Me.m_tsSumtoOneBtn = New System.Windows.Forms.ToolStripButton
            Me.m_grid = New ScientificInterface.Ecosim.gridQuotaShare
            Me.m_tss.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tss
            '
            Me.m_tss.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnDefaults, Me.m_tsSumtoOneBtn})
            Me.m_tss.Location = New System.Drawing.Point(0, 0)
            Me.m_tss.Name = "m_tss"
            Me.m_tss.Size = New System.Drawing.Size(327, 25)
            Me.m_tss.TabIndex = 2
            Me.m_tss.Text = "ToolStrip1"
            '
            'm_tsbnDefaults
            '
            Me.m_tsbnDefaults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbnDefaults.Image = CType(resources.GetObject("m_tsbnDefaults.Image"), System.Drawing.Image)
            Me.m_tsbnDefaults.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnDefaults.Name = "m_tsbnDefaults"
            Me.m_tsbnDefaults.Size = New System.Drawing.Size(82, 22)
            Me.m_tsbnDefaults.Text = "Set to &defaults"
            '
            'm_tsSumtoOneBtn
            '
            Me.m_tsSumtoOneBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsSumtoOneBtn.Image = Global.ScientificInterface.My.Resources.Resources.OptionsIconSM
            Me.m_tsSumtoOneBtn.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsSumtoOneBtn.Name = "m_tsSumtoOneBtn"
            Me.m_tsSumtoOneBtn.Size = New System.Drawing.Size(65, 22)
            Me.m_tsSumtoOneBtn.Text = "Sum to &one"
            '
            'm_grid
            '
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
            Me.m_grid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.Location = New System.Drawing.Point(0, 25)
            Me.m_grid.Name = "m_grid"
            Me.m_grid.Size = New System.Drawing.Size(327, 132)
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_grid.TabIndex = 3
            Me.m_grid.TrackPropertySelection = True
            Me.m_grid.UIContext = Nothing
            '
            'frmQuotaShare
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(327, 157)
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_tss)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmQuotaShare"
            Me.Text = "MSE quota share"
            Me.m_tss.ResumeLayout(False)
            Me.m_tss.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_grid As gridQuotaShare
        Private WithEvents m_tss As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsSumtoOneBtn As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnDefaults As System.Windows.Forms.ToolStripButton
    End Class

End Namespace