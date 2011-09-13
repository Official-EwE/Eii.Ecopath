Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgDefineCapacityMaps
        Inherits System.Windows.Forms.Form

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
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgDefineCapacityMaps))
            Me.m_grid = New ScientificInterface.Ecospace.gridDefineCapacityMaps
            Me.m_btnAdd = New System.Windows.Forms.Button
            Me.m_btnRemove = New System.Windows.Forms.Button
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_btnKeep = New System.Windows.Forms.Button
            Me.m_hdrDefaults = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_btnCreateDefaults = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'm_grid
            '
            Me.m_grid.AllowBlockSelect = True
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
            Me.m_grid.CustomSort = False
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
            'm_btnAdd
            '
            resources.ApplyResources(Me.m_btnAdd, "m_btnAdd")
            Me.m_btnAdd.Name = "m_btnAdd"
            Me.m_btnAdd.UseVisualStyleBackColor = True
            '
            'm_btnRemove
            '
            resources.ApplyResources(Me.m_btnRemove, "m_btnRemove")
            Me.m_btnRemove.Name = "m_btnRemove"
            Me.m_btnRemove.UseVisualStyleBackColor = True
            '
            'OK_Button
            '
            resources.ApplyResources(Me.OK_Button, "OK_Button")
            Me.OK_Button.Name = "OK_Button"
            '
            'Cancel_Button
            '
            resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Name = "Cancel_Button"
            '
            'm_btnKeep
            '
            resources.ApplyResources(Me.m_btnKeep, "m_btnKeep")
            Me.m_btnKeep.Name = "m_btnKeep"
            Me.m_btnKeep.UseVisualStyleBackColor = True
            '
            'm_hdrDefaults
            '
            resources.ApplyResources(Me.m_hdrDefaults, "m_hdrDefaults")
            Me.m_hdrDefaults.CanCollapseParent = False
            Me.m_hdrDefaults.CollapsedParentHeight = 0
            Me.m_hdrDefaults.IsCollapsed = False
            Me.m_hdrDefaults.Name = "m_hdrDefaults"
            '
            'm_btnCreateDefaults
            '
            resources.ApplyResources(Me.m_btnCreateDefaults, "m_btnCreateDefaults")
            Me.m_btnCreateDefaults.Name = "m_btnCreateDefaults"
            Me.m_btnCreateDefaults.UseVisualStyleBackColor = True
            '
            'dlgDefineCapacityMaps
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.OK_Button)
            Me.Controls.Add(Me.m_hdrDefaults)
            Me.Controls.Add(Me.Cancel_Button)
            Me.Controls.Add(Me.m_btnKeep)
            Me.Controls.Add(Me.m_btnRemove)
            Me.Controls.Add(Me.m_btnCreateDefaults)
            Me.Controls.Add(Me.m_btnAdd)
            Me.Controls.Add(Me.m_grid)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgDefineCapacityMaps"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_grid As gridDefineCapacityMaps
        Private WithEvents m_btnRemove As System.Windows.Forms.Button
        Private WithEvents m_btnAdd As System.Windows.Forms.Button
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_btnKeep As System.Windows.Forms.Button
        Private WithEvents m_hdrDefaults As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_btnCreateDefaults As System.Windows.Forms.Button

    End Class

End Namespace

