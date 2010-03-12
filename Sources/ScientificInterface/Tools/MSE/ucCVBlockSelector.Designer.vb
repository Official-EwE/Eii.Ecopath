<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucCVBlockSelector
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.gridSelector = New ScientificInterface.gridSelectColorBlock
        Me.SuspendLayout()
        '
        'gridSelector
        '
        Me.gridSelector.AutoSizeMinHeight = 10
        Me.gridSelector.AutoSizeMinWidth = 10
        Me.gridSelector.AutoStretchColumnsToFitWidth = False
        Me.gridSelector.AutoStretchRowsToFitHeight = False
        Me.gridSelector.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
        Me.gridSelector.CustomSort = False
        Me.gridSelector.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridSelector.FixedColumnWidths = True
        Me.gridSelector.FocusStyle = SourceGrid2.FocusStyle.None
        Me.gridSelector.GridToolTipActive = True
        Me.gridSelector.Location = New System.Drawing.Point(0, 0)
        Me.gridSelector.Name = "gridSelector"
        Me.gridSelector.Size = New System.Drawing.Size(539, 67)
        Me.gridSelector.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.gridSelector.TabIndex = 4
        Me.gridSelector.TrackPropertySelection = True
        Me.gridSelector.UIContext = Nothing
        '
        'ucCVBlockSelector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.gridSelector)
        Me.Name = "ucCVBlockSelector"
        Me.Size = New System.Drawing.Size(539, 67)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gridSelector As ScientificInterface.gridSelectColorBlock

End Class
