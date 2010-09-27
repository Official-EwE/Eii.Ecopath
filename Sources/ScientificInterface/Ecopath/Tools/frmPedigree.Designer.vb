Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

Namespace Ecopath.Tools

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmPedigree
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
            Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.Label2 = New System.Windows.Forms.Label
            Me.m_cmbViewAs = New System.Windows.Forms.ComboBox
            Me.m_lblViewAs = New System.Windows.Forms.Label
            Me.m_cmbCategory = New System.Windows.Forms.ComboBox
            Me.Label1 = New System.Windows.Forms.Label
            Me.m_lbLevels = New System.Windows.Forms.ListBox
            Me.m_hdrPedigree = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_grid = New ScientificInterface.Ecopath.Tools.gridPedigree
            Me.m_hdrGrid = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsMain
            '
            Me.m_tsMain.Location = New System.Drawing.Point(0, 0)
            Me.m_tsMain.Name = "m_tsMain"
            Me.m_tsMain.Size = New System.Drawing.Size(724, 25)
            Me.m_tsMain.TabIndex = 0
            Me.m_tsMain.Text = "ToolStrip1"
            '
            'm_scMain
            '
            Me.m_scMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMain.Location = New System.Drawing.Point(0, 25)
            Me.m_scMain.Margin = New System.Windows.Forms.Padding(0)
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.Label2)
            Me.m_scMain.Panel1.Controls.Add(Me.m_cmbViewAs)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblViewAs)
            Me.m_scMain.Panel1.Controls.Add(Me.m_cmbCategory)
            Me.m_scMain.Panel1.Controls.Add(Me.Label1)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lbLevels)
            Me.m_scMain.Panel1.Controls.Add(Me.m_hdrPedigree)
            Me.m_scMain.Panel1MinSize = 108
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_grid)
            Me.m_scMain.Panel2.Controls.Add(Me.m_hdrGrid)
            Me.m_scMain.Size = New System.Drawing.Size(724, 419)
            Me.m_scMain.SplitterDistance = 173
            Me.m_scMain.TabIndex = 1
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(3, 78)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(41, 13)
            Me.Label2.TabIndex = 5
            Me.Label2.Text = "&Levels:"
            '
            'm_cmbViewAs
            '
            Me.m_cmbViewAs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbViewAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbViewAs.FormattingEnabled = True
            Me.m_cmbViewAs.Items.AddRange(New Object() {"Color", "Indices", "Values", "Confidence Interval"})
            Me.m_cmbViewAs.Location = New System.Drawing.Point(61, 51)
            Me.m_cmbViewAs.Name = "m_cmbViewAs"
            Me.m_cmbViewAs.Size = New System.Drawing.Size(112, 21)
            Me.m_cmbViewAs.TabIndex = 4
            '
            'm_lblViewAs
            '
            Me.m_lblViewAs.AutoSize = True
            Me.m_lblViewAs.Location = New System.Drawing.Point(3, 54)
            Me.m_lblViewAs.Name = "m_lblViewAs"
            Me.m_lblViewAs.Size = New System.Drawing.Size(47, 13)
            Me.m_lblViewAs.TabIndex = 3
            Me.m_lblViewAs.Text = "&View as:"
            '
            'm_cmbCategory
            '
            Me.m_cmbCategory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbCategory.FormattingEnabled = True
            Me.m_cmbCategory.Location = New System.Drawing.Point(61, 21)
            Me.m_cmbCategory.Name = "m_cmbCategory"
            Me.m_cmbCategory.Size = New System.Drawing.Size(112, 21)
            Me.m_cmbCategory.TabIndex = 2
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(3, 24)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(52, 13)
            Me.Label1.TabIndex = 1
            Me.Label1.Text = "&Category:"
            '
            'm_lbLevels
            '
            Me.m_lbLevels.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lbLevels.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbLevels.FormattingEnabled = True
            Me.m_lbLevels.IntegralHeight = False
            Me.m_lbLevels.ItemHeight = 15
            Me.m_lbLevels.Location = New System.Drawing.Point(0, 94)
            Me.m_lbLevels.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
            Me.m_lbLevels.Name = "m_lbLevels"
            Me.m_lbLevels.Size = New System.Drawing.Size(173, 325)
            Me.m_lbLevels.TabIndex = 6
            '
            'm_hdrPedigree
            '
            Me.m_hdrPedigree.Dock = System.Windows.Forms.DockStyle.Top
            Me.m_hdrPedigree.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrPedigree.Name = "m_hdrPedigree"
            Me.m_hdrPedigree.Size = New System.Drawing.Size(173, 18)
            Me.m_hdrPedigree.TabIndex = 0
            Me.m_hdrPedigree.Text = "Pedigree"
            Me.m_hdrPedigree.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
            Me.m_grid.FixedColumnWidths = True
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.Location = New System.Drawing.Point(0, 18)
            Me.m_grid.Margin = New System.Windows.Forms.Padding(0)
            Me.m_grid.Name = "m_grid"
            Me.m_grid.Size = New System.Drawing.Size(547, 401)
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_grid.TabIndex = 1
            Me.m_grid.UIContext = Nothing
            Me.m_grid.PedigreeStyleGuide = Nothing
            '
            'm_hdrGrid
            '
            Me.m_hdrGrid.Dock = System.Windows.Forms.DockStyle.Top
            Me.m_hdrGrid.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrGrid.Name = "m_hdrGrid"
            Me.m_hdrGrid.Size = New System.Drawing.Size(547, 18)
            Me.m_hdrGrid.TabIndex = 0
            Me.m_hdrGrid.Text = "Assignment"
            Me.m_hdrGrid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'frmPedigree
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(724, 444)
            Me.ControlBox = False
            Me.Controls.Add(Me.m_scMain)
            Me.Controls.Add(Me.m_tsMain)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmPedigree"
            Me.ShowInTaskbar = False
            Me.Text = "frmPedigree"
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel1.PerformLayout()
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsMain As cEwEToolstrip
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_lbLevels As System.Windows.Forms.ListBox
        Private WithEvents m_hdrPedigree As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_grid As ScientificInterface.Ecopath.Tools.gridPedigree
        Private WithEvents m_hdrGrid As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Private WithEvents m_cmbViewAs As System.Windows.Forms.ComboBox
        Private WithEvents m_lblViewAs As System.Windows.Forms.Label
        Private WithEvents m_cmbCategory As System.Windows.Forms.ComboBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
    End Class

End Namespace
