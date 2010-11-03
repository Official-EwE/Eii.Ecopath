Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Forms

Namespace Ecopath.Input

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmFisheryBasicInput
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
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.m_tsbnEditFleets = New System.Windows.Forms.ToolStripButton
            Me.FisheryInputFleetDefinitionEwEGrid1 = New ScientificInterface.Ecopath.Input.FisheryInputFleetDefinitionEwEGrid
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnEditFleets})
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.Size = New System.Drawing.Size(292, 25)
            Me.m_ts.TabIndex = 0
            Me.m_ts.Text = "ToolStrip1"
            '
            'm_tsbnEditFleets
            '
            Me.m_tsbnEditFleets.Image = Global.ScientificInterface.My.Resources.Resources.EditGroup
            Me.m_tsbnEditFleets.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnEditFleets.Name = "m_tsbnEditFleets"
            Me.m_tsbnEditFleets.Size = New System.Drawing.Size(87, 22)
            Me.m_tsbnEditFleets.Text = "&Edit fleets..."
            Me.m_tsbnEditFleets.ToolTipText = "Create or delete fleet definitions..."
            '
            'FisheryInputFleetDefinitionEwEGrid1
            '
            Me.FisheryInputFleetDefinitionEwEGrid1.AllowBlockSelect = True
            Me.FisheryInputFleetDefinitionEwEGrid1.AutoSizeMinHeight = 10
            Me.FisheryInputFleetDefinitionEwEGrid1.AutoSizeMinWidth = 10
            Me.FisheryInputFleetDefinitionEwEGrid1.AutoStretchColumnsToFitWidth = False
            Me.FisheryInputFleetDefinitionEwEGrid1.AutoStretchRowsToFitHeight = False
            Me.FisheryInputFleetDefinitionEwEGrid1.BackColor = System.Drawing.Color.White
            Me.FisheryInputFleetDefinitionEwEGrid1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.FisheryInputFleetDefinitionEwEGrid1.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.FisheryInputFleetDefinitionEwEGrid1.CustomSort = False
            Me.FisheryInputFleetDefinitionEwEGrid1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.FisheryInputFleetDefinitionEwEGrid1.FixedColumnWidths = True
            Me.FisheryInputFleetDefinitionEwEGrid1.FocusStyle = SourceGrid2.FocusStyle.None
            Me.FisheryInputFleetDefinitionEwEGrid1.GridToolTipActive = True
            Me.FisheryInputFleetDefinitionEwEGrid1.Location = New System.Drawing.Point(0, 25)
            Me.FisheryInputFleetDefinitionEwEGrid1.Name = "FisheryInputFleetDefinitionEwEGrid1"
            Me.FisheryInputFleetDefinitionEwEGrid1.Size = New System.Drawing.Size(292, 248)
            Me.FisheryInputFleetDefinitionEwEGrid1.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.FisheryInputFleetDefinitionEwEGrid1.TabIndex = 1
            Me.FisheryInputFleetDefinitionEwEGrid1.UIContext = Nothing
            '
            'frmFisheryBasicInput
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(292, 273)
            Me.Controls.Add(Me.FisheryInputFleetDefinitionEwEGrid1)
            Me.Controls.Add(Me.m_ts)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmFisheryBasicInput"
            Me.Text = "Definition of fleets"
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbnEditFleets As System.Windows.Forms.ToolStripButton
        Friend WithEvents FisheryInputFleetDefinitionEwEGrid1 As ScientificInterface.Ecopath.Input.FisheryInputFleetDefinitionEwEGrid
    End Class

End Namespace
