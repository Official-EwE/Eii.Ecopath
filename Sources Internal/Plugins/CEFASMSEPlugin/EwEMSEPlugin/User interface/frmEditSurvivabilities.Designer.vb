
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditSurvivabilities
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditSurvivabilities))
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_grid = New EwEMSEPlugin.gridSurviveDistParameters()
        Me.SuspendLayout()
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = False
        resources.ApplyResources(Me.m_grid, "m_grid")
        Me.m_grid.AutoSizeMinHeight = 10
        Me.m_grid.AutoSizeMinWidth = 10
        Me.m_grid.AutoStretchColumnsToFitWidth = True
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
        'frmEditSurvivabilities
        '
        Me.AcceptButton = Me.m_btnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_grid)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEditSurvivabilities"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents m_btnOK As System.Windows.Forms.Button
    Friend WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_grid As EwEMSEPlugin.gridSurviveDistParameters
End Class
