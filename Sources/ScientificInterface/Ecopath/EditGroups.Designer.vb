Imports ScientificInterfaceShared

Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EditGroups
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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditGroups))
            Me.m_grid = New ScientificInterface.gridEditGroups
            Me.m_btnInsert = New System.Windows.Forms.Button
            Me.m_btnMoveUp = New System.Windows.Forms.Button
            Me.m_btnMoveDown = New System.Windows.Forms.Button
            Me.m_btnDelete = New System.Windows.Forms.Button
            Me.m_btnKeep = New System.Windows.Forms.Button
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_bntColorDefaultAll = New System.Windows.Forms.Button
            Me.m_btnColorAlternateAll = New System.Windows.Forms.Button
            Me.m_hdrColours = New cEwEHeaderLabel
            Me.m_hdrEdit = New cEwEHeaderLabel
            Me.m_hdrOrder = New cEwEHeaderLabel
            Me.m_btnColourCustomCurrent = New System.Windows.Forms.Button
            Me.m_btnColourDefaultCurrent = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'm_grid
            '
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
            Me.m_grid.FixedColumnWidths = True
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
            Me.m_grid.TrackPropertySelection = True
            Me.m_grid.UIContext = Nothing
            '
            'm_btnInsert
            '
            resources.ApplyResources(Me.m_btnInsert, "m_btnInsert")
            Me.m_btnInsert.Name = "m_btnInsert"
            Me.m_btnInsert.UseVisualStyleBackColor = True
            '
            'm_btnMoveUp
            '
            resources.ApplyResources(Me.m_btnMoveUp, "m_btnMoveUp")
            Me.m_btnMoveUp.Name = "m_btnMoveUp"
            Me.m_btnMoveUp.UseVisualStyleBackColor = True
            '
            'm_btnMoveDown
            '
            resources.ApplyResources(Me.m_btnMoveDown, "m_btnMoveDown")
            Me.m_btnMoveDown.Name = "m_btnMoveDown"
            Me.m_btnMoveDown.UseVisualStyleBackColor = True
            '
            'm_btnDelete
            '
            resources.ApplyResources(Me.m_btnDelete, "m_btnDelete")
            Me.m_btnDelete.Name = "m_btnDelete"
            Me.m_btnDelete.UseVisualStyleBackColor = True
            '
            'm_btnKeep
            '
            resources.ApplyResources(Me.m_btnKeep, "m_btnKeep")
            Me.m_btnKeep.Name = "m_btnKeep"
            Me.m_btnKeep.UseVisualStyleBackColor = True
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
            'm_bntColorDefaultAll
            '
            resources.ApplyResources(Me.m_bntColorDefaultAll, "m_bntColorDefaultAll")
            Me.m_bntColorDefaultAll.Name = "m_bntColorDefaultAll"
            Me.m_bntColorDefaultAll.UseVisualStyleBackColor = True
            '
            'm_btnColorAlternateAll
            '
            resources.ApplyResources(Me.m_btnColorAlternateAll, "m_btnColorAlternateAll")
            Me.m_btnColorAlternateAll.Name = "m_btnColorAlternateAll"
            Me.m_btnColorAlternateAll.UseVisualStyleBackColor = True
            '
            'm_hdrColours
            '
            resources.ApplyResources(Me.m_hdrColours, "m_hdrColours")
            Me.m_hdrColours.Name = "m_hdrColours"
            '
            'm_hdrEdit
            '
            resources.ApplyResources(Me.m_hdrEdit, "m_hdrEdit")
            Me.m_hdrEdit.Name = "m_hdrEdit"
            '
            'm_hdrOrder
            '
            resources.ApplyResources(Me.m_hdrOrder, "m_hdrOrder")
            Me.m_hdrOrder.Name = "m_hdrOrder"
            '
            'm_btnColourCustomCurrent
            '
            resources.ApplyResources(Me.m_btnColourCustomCurrent, "m_btnColourCustomCurrent")
            Me.m_btnColourCustomCurrent.Name = "m_btnColourCustomCurrent"
            Me.m_btnColourCustomCurrent.UseVisualStyleBackColor = True
            '
            'm_btnColourDefaultCurrent
            '
            resources.ApplyResources(Me.m_btnColourDefaultCurrent, "m_btnColourDefaultCurrent")
            Me.m_btnColourDefaultCurrent.Name = "m_btnColourDefaultCurrent"
            Me.m_btnColourDefaultCurrent.UseVisualStyleBackColor = True
            '
            'EditGroups
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.Cancel_Button)
            Me.Controls.Add(Me.OK_Button)
            Me.Controls.Add(Me.m_btnColourDefaultCurrent)
            Me.Controls.Add(Me.m_btnColourCustomCurrent)
            Me.Controls.Add(Me.m_btnColorAlternateAll)
            Me.Controls.Add(Me.m_bntColorDefaultAll)
            Me.Controls.Add(Me.m_hdrOrder)
            Me.Controls.Add(Me.m_hdrEdit)
            Me.Controls.Add(Me.m_hdrColours)
            Me.Controls.Add(Me.m_btnKeep)
            Me.Controls.Add(Me.m_btnDelete)
            Me.Controls.Add(Me.m_btnMoveDown)
            Me.Controls.Add(Me.m_btnMoveUp)
            Me.Controls.Add(Me.m_btnInsert)
            Me.Controls.Add(Me.m_grid)
            Me.Name = "EditGroups"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.ResumeLayout(False)

        End Sub

        Private WithEvents m_grid As gridEditGroups
        Private WithEvents m_btnInsert As System.Windows.Forms.Button
        Private WithEvents m_btnMoveUp As System.Windows.Forms.Button
        Private WithEvents m_btnMoveDown As System.Windows.Forms.Button
        Private WithEvents m_btnDelete As System.Windows.Forms.Button
        Private WithEvents m_btnKeep As System.Windows.Forms.Button
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_bntColorDefaultAll As System.Windows.Forms.Button
        Private WithEvents m_btnColorAlternateAll As System.Windows.Forms.Button
        Private m_hdrColours As cEwEHeaderLabel
        Private m_hdrEdit As cEwEHeaderLabel
        Private m_hdrOrder As cEwEHeaderLabel
        Private WithEvents m_btnColourCustomCurrent As System.Windows.Forms.Button
        Private WithEvents m_btnColourDefaultCurrent As System.Windows.Forms.Button

    End Class

End Namespace

