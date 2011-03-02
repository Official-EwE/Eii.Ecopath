Namespace Import

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucImportPageModels
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
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_btnBrowse = New System.Windows.Forms.Button()
            Me.m_tbxOutputFolder = New System.Windows.Forms.TextBox()
            Me.m_lblOutputFolder = New System.Windows.Forms.Label()
            Me.m_lblModels = New System.Windows.Forms.Label()
            Me.m_grid = New ScientificInterface.Import.cImportGrid()
            Me.m_hdr = New ScientificInterface.Import.ucImportHeader()
            Me.m_lblFormat = New System.Windows.Forms.Label()
            Me.m_cmbDatabaseFormat = New System.Windows.Forms.ComboBox()
            Me.m_scModels = New System.Windows.Forms.SplitContainer()
            Me.m_lblComments = New System.Windows.Forms.Label()
            Me.m_scModels.Panel1.SuspendLayout()
            Me.m_scModels.Panel2.SuspendLayout()
            Me.m_scModels.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnBrowse
            '
            Me.m_btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.m_btnBrowse.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_btnBrowse.Location = New System.Drawing.Point(446, 427)
            Me.m_btnBrowse.Name = "m_btnBrowse"
            Me.m_btnBrowse.Size = New System.Drawing.Size(64, 23)
            Me.m_btnBrowse.TabIndex = 5
            Me.m_btnBrowse.Text = "&Browse..."
            Me.m_btnBrowse.UseVisualStyleBackColor = True
            '
            'm_tbxOutputFolder
            '
            Me.m_tbxOutputFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxOutputFolder.Location = New System.Drawing.Point(97, 429)
            Me.m_tbxOutputFolder.Name = "m_tbxOutputFolder"
            Me.m_tbxOutputFolder.Size = New System.Drawing.Size(343, 20)
            Me.m_tbxOutputFolder.TabIndex = 4
            '
            'm_lblOutputFolder
            '
            Me.m_lblOutputFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblOutputFolder.AutoSize = True
            Me.m_lblOutputFolder.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
            Me.m_lblOutputFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblOutputFolder.Location = New System.Drawing.Point(3, 432)
            Me.m_lblOutputFolder.Name = "m_lblOutputFolder"
            Me.m_lblOutputFolder.Size = New System.Drawing.Size(71, 13)
            Me.m_lblOutputFolder.TabIndex = 3
            Me.m_lblOutputFolder.Text = "&Output folder:"
            '
            'm_lblModels
            '
            Me.m_lblModels.AutoSize = True
            Me.m_lblModels.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
            Me.m_lblModels.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblModels.Location = New System.Drawing.Point(3, 75)
            Me.m_lblModels.Name = "m_lblModels"
            Me.m_lblModels.Size = New System.Drawing.Size(178, 13)
            Me.m_lblModels.TabIndex = 1
            Me.m_lblModels.Text = "&Select one or more models to import:"
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
            Me.m_grid.Margin = New System.Windows.Forms.Padding(0)
            Me.m_grid.Name = "m_grid"
            Me.m_grid.Size = New System.Drawing.Size(510, 250)
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_grid.TabIndex = 2
            Me.m_grid.TrackPropertySelection = False
            Me.m_grid.UIContext = Nothing
            '
            'm_hdr
            '
            Me.m_hdr.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdr.BackColor = System.Drawing.Color.White
            Me.m_hdr.Location = New System.Drawing.Point(0, 0)
            Me.m_hdr.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdr.Name = "m_hdr"
            Me.m_hdr.Size = New System.Drawing.Size(510, 64)
            Me.m_hdr.SubText = "<database name goes here>"
            Me.m_hdr.TabIndex = 0
            Me.m_hdr.Text = "Selected model"
            '
            'm_lblFormat
            '
            Me.m_lblFormat.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblFormat.AutoSize = True
            Me.m_lblFormat.Location = New System.Drawing.Point(3, 458)
            Me.m_lblFormat.Name = "m_lblFormat"
            Me.m_lblFormat.Size = New System.Drawing.Size(88, 13)
            Me.m_lblFormat.TabIndex = 6
            Me.m_lblFormat.Text = "&Database format:"
            '
            'm_cmbDatabaseFormat
            '
            Me.m_cmbDatabaseFormat.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_cmbDatabaseFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbDatabaseFormat.FormattingEnabled = True
            Me.m_cmbDatabaseFormat.Location = New System.Drawing.Point(97, 455)
            Me.m_cmbDatabaseFormat.Name = "m_cmbDatabaseFormat"
            Me.m_cmbDatabaseFormat.Size = New System.Drawing.Size(189, 21)
            Me.m_cmbDatabaseFormat.TabIndex = 7
            '
            'm_scModels
            '
            Me.m_scModels.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_scModels.Location = New System.Drawing.Point(0, 92)
            Me.m_scModels.Margin = New System.Windows.Forms.Padding(0)
            Me.m_scModels.Name = "m_scModels"
            Me.m_scModels.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_scModels.Panel1
            '
            Me.m_scModels.Panel1.Controls.Add(Me.m_grid)
            '
            'm_scModels.Panel2
            '
            Me.m_scModels.Panel2.Controls.Add(Me.m_lblComments)
            Me.m_scModels.Size = New System.Drawing.Size(510, 332)
            Me.m_scModels.SplitterDistance = 250
            Me.m_scModels.TabIndex = 8
            '
            'm_lblComments
            '
            Me.m_lblComments.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblComments.Location = New System.Drawing.Point(0, 0)
            Me.m_lblComments.Name = "m_lblComments"
            Me.m_lblComments.Size = New System.Drawing.Size(510, 78)
            Me.m_lblComments.TabIndex = 0
            Me.m_lblComments.Text = "<comments>"
            '
            'ucImportPageModels
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scModels)
            Me.Controls.Add(Me.m_cmbDatabaseFormat)
            Me.Controls.Add(Me.m_lblFormat)
            Me.Controls.Add(Me.m_btnBrowse)
            Me.Controls.Add(Me.m_tbxOutputFolder)
            Me.Controls.Add(Me.m_lblOutputFolder)
            Me.Controls.Add(Me.m_lblModels)
            Me.Controls.Add(Me.m_hdr)
            Me.Name = "ucImportPageModels"
            Me.Size = New System.Drawing.Size(510, 476)
            Me.m_scModels.Panel1.ResumeLayout(False)
            Me.m_scModels.Panel2.ResumeLayout(False)
            Me.m_scModels.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_grid As cImportGrid
        Private WithEvents m_btnBrowse As System.Windows.Forms.Button
        Private WithEvents m_tbxOutputFolder As System.Windows.Forms.TextBox
        Private WithEvents m_lblOutputFolder As System.Windows.Forms.Label
        Private WithEvents m_lblModels As System.Windows.Forms.Label
        Private WithEvents m_hdr As ucImportHeader
        Private WithEvents m_lblFormat As System.Windows.Forms.Label
        Friend WithEvents m_cmbDatabaseFormat As System.Windows.Forms.ComboBox
        Private WithEvents m_scModels As System.Windows.Forms.SplitContainer
        Private WithEvents m_lblComments As System.Windows.Forms.Label

    End Class

End Namespace
