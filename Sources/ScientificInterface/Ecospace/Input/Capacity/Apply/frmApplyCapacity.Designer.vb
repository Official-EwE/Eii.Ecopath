Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    <CLSCompliant(False)> _
Partial Class frmApplyCapacity
    Inherits Ecosim.frmApplyShapeBase

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
        Me.m_grid = New ScientificInterface.ucApplyMapResponseGrid(Me.components)
        Me.SuspendLayout()
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
        Me.m_grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_grid.FixedColumnWidths = False
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.Location = New System.Drawing.Point(0, 0)
        Me.m_grid.Name = "m_grid"
        Me.m_grid.Size = New System.Drawing.Size(292, 273)
        Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_grid.TabIndex = 0
        Me.m_grid.UIContext = Nothing
        '
        'frmApplyCapacity
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 273)
        Me.Controls.Add(Me.m_grid)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmApplyCapacity"
        Me.Text = "Apply Capacity"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents m_grid As ucApplyMapResponseGrid

End Class
