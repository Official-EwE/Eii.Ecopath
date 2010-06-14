<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSearchResults
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSearchResults))
        Me.m_lblStatus = New System.Windows.Forms.Label
        Me.m_btnSearch = New System.Windows.Forms.Button
        Me.m_btnCancel = New System.Windows.Forms.Button
        Me.m_btnUse = New System.Windows.Forms.Button
        Me.m_tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.m_grid = New ScientificInterface.gridTaxonSearchResults
        Me.m_tlpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblStatus
        '
        resources.ApplyResources(Me.m_lblStatus, "m_lblStatus")
        Me.m_lblStatus.Name = "m_lblStatus"
        '
        'm_btnSearch
        '
        resources.ApplyResources(Me.m_btnSearch, "m_btnSearch")
        Me.m_btnSearch.Name = "m_btnSearch"
        Me.m_btnSearch.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnUse
        '
        resources.ApplyResources(Me.m_btnUse, "m_btnUse")
        Me.m_btnUse.Name = "m_btnUse"
        Me.m_btnUse.UseVisualStyleBackColor = True
        '
        'm_tlpButtons
        '
        resources.ApplyResources(Me.m_tlpButtons, "m_tlpButtons")
        Me.m_tlpButtons.Controls.Add(Me.m_btnUse, 1, 0)
        Me.m_tlpButtons.Controls.Add(Me.m_btnCancel, 3, 0)
        Me.m_tlpButtons.Controls.Add(Me.m_btnSearch, 2, 0)
        Me.m_tlpButtons.Name = "m_tlpButtons"
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
        Me.m_grid.CustomSort = True
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
        Me.m_grid.TrackPropertySelection = False
        Me.m_grid.UIContext = Nothing
        '
        'frmSearchResults
        '
        Me.AcceptButton = Me.m_btnUse
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_grid)
        Me.Controls.Add(Me.m_tlpButtons)
        Me.Controls.Add(Me.m_lblStatus)
        Me.Name = "frmSearchResults"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.m_tlpButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_btnUse As System.Windows.Forms.Button
    Private WithEvents m_tlpButtons As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_lblStatus As System.Windows.Forms.Label
    Private WithEvents m_btnSearch As System.Windows.Forms.Button
    Private WithEvents m_grid As ScientificInterface.gridTaxonSearchResults
End Class
