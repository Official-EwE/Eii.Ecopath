
Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEResults
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
        Me.rbGroup = New System.Windows.Forms.RadioButton
        Me.rbFleet = New System.Windows.Forms.RadioButton
        Me.Grid = New ScientificInterface.gridRiskResults
        Me.pnlGrid = New System.Windows.Forms.Panel
        Me.pnlGrid.SuspendLayout()
        Me.SuspendLayout()
        '
        'rbGroup
        '
        Me.rbGroup.AutoSize = True
        Me.rbGroup.Checked = True
        Me.rbGroup.Location = New System.Drawing.Point(15, 10)
        Me.rbGroup.Name = "rbGroup"
        Me.rbGroup.Size = New System.Drawing.Size(54, 17)
        Me.rbGroup.TabIndex = 0
        Me.rbGroup.TabStop = True
        Me.rbGroup.Text = "Group"
        Me.rbGroup.UseVisualStyleBackColor = True
        '
        'rbFleet
        '
        Me.rbFleet.AutoSize = True
        Me.rbFleet.Location = New System.Drawing.Point(75, 10)
        Me.rbFleet.Name = "rbFleet"
        Me.rbFleet.Size = New System.Drawing.Size(48, 17)
        Me.rbFleet.TabIndex = 3
        Me.rbFleet.Text = "Fleet"
        Me.rbFleet.UseVisualStyleBackColor = True
        '
        'Grid
        '
        Me.Grid.AutoSizeMinHeight = 10
        Me.Grid.AutoSizeMinWidth = 10
        Me.Grid.AutoStretchColumnsToFitWidth = False
        Me.Grid.AutoStretchRowsToFitHeight = False
        Me.Grid.BackColor = System.Drawing.Color.White
        Me.Grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.Grid.CustomSort = False
        Me.Grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Grid.FixedColumnWidths = True
        Me.Grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.Grid.GridToolTipActive = True
        Me.Grid.Location = New System.Drawing.Point(0, 0)
        Me.Grid.Name = "Grid"
        Me.Grid.Size = New System.Drawing.Size(685, 394)
        Me.Grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.Grid.TabIndex = 4
        Me.Grid.TrackPropertySelection = True
        '
        'pnlGrid
        '
        Me.pnlGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlGrid.Controls.Add(Me.Grid)
        Me.pnlGrid.Location = New System.Drawing.Point(1, 33)
        Me.pnlGrid.Name = "pnlGrid"
        Me.pnlGrid.Size = New System.Drawing.Size(685, 394)
        Me.pnlGrid.TabIndex = 5
        '
        'frmMSEResults
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(684, 426)
        Me.Controls.Add(Me.pnlGrid)
        Me.Controls.Add(Me.rbFleet)
        Me.Controls.Add(Me.rbGroup)
        Me.Name = "frmMSEResults"
        Me.Text = "frmMSEResults"
        Me.pnlGrid.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rbGroup As System.Windows.Forms.RadioButton
    Friend WithEvents rbFleet As System.Windows.Forms.RadioButton
    Friend WithEvents Grid As ScientificInterface.gridRiskResults
    Friend WithEvents pnlGrid As System.Windows.Forms.Panel
End Class
