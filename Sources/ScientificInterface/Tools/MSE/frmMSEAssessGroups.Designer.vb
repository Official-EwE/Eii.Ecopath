<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEAssessGroups
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.GridBioCV1 = New ScientificInterface.gridBioCV
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.GridBioCV1)
        Me.SplitContainer1.Size = New System.Drawing.Size(652, 483)
        Me.SplitContainer1.SplitterDistance = 217
        Me.SplitContainer1.TabIndex = 0
        '
        'GridBioCV1
        '
        Me.GridBioCV1.AutoSizeMinHeight = 10
        Me.GridBioCV1.AutoSizeMinWidth = 10
        Me.GridBioCV1.AutoStretchColumnsToFitWidth = False
        Me.GridBioCV1.AutoStretchRowsToFitHeight = False
        Me.GridBioCV1.BackColor = System.Drawing.Color.White
        Me.GridBioCV1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.GridBioCV1.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.GridBioCV1.CustomSort = False
        Me.GridBioCV1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridBioCV1.FixedColumnWidths = True
        Me.GridBioCV1.FocusStyle = SourceGrid2.FocusStyle.None
        Me.GridBioCV1.GridToolTipActive = True
        Me.GridBioCV1.Location = New System.Drawing.Point(0, 0)
        Me.GridBioCV1.Name = "GridBioCV1"
        Me.GridBioCV1.Size = New System.Drawing.Size(652, 262)
        Me.GridBioCV1.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.GridBioCV1.TabIndex = 0
        Me.GridBioCV1.TrackPropertySelection = True
        '
        'frmMSEAssessGroups
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(652, 483)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSEAssessGroups"
        Me.Text = "frmMSEAssessGroups"
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents GridBioCV1 As ScientificInterface.gridBioCV
End Class
