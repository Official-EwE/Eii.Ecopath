Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms


<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEBatchTFM
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
        Me.m_grid = New ScientificInterface.gridMSEBatchTFM()
        Me.UpDwnIter = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btCalcIters = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txNTFM = New System.Windows.Forms.TextBox()
        Me.rbCalcTypePercent = New System.Windows.Forms.RadioButton()
        Me.rbCalcTypeValue = New System.Windows.Forms.RadioButton()
        CType(Me.UpDwnIter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = True
        Me.m_grid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        Me.m_grid.FixedColumnWidths = False
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.iCurIter = 1
        Me.m_grid.Location = New System.Drawing.Point(2, 83)
        Me.m_grid.Name = "m_grid"
        Me.m_grid.Size = New System.Drawing.Size(782, 389)
        Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_grid.TabIndex = 5
        Me.m_grid.UIContext = Nothing
        '
        'UpDwnIter
        '
        Me.UpDwnIter.Location = New System.Drawing.Point(92, 57)
        Me.UpDwnIter.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.UpDwnIter.Name = "UpDwnIter"
        Me.UpDwnIter.Size = New System.Drawing.Size(51, 20)
        Me.UpDwnIter.TabIndex = 4
        Me.UpDwnIter.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 59)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Show iteration "
        '
        'btCalcIters
        '
        Me.btCalcIters.Location = New System.Drawing.Point(222, 5)
        Me.btCalcIters.Name = "btCalcIters"
        Me.btCalcIters.Size = New System.Drawing.Size(212, 20)
        Me.btCalcIters.TabIndex = 2
        Me.btCalcIters.Text = "Calculate iteration values"
        Me.btCalcIters.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(151, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Number of parameter iterations"
        '
        'txNTFM
        '
        Me.txNTFM.Location = New System.Drawing.Point(166, 4)
        Me.txNTFM.Name = "txNTFM"
        Me.txNTFM.Size = New System.Drawing.Size(50, 20)
        Me.txNTFM.TabIndex = 1
        '
        'rbCalcTypePercent
        '
        Me.rbCalcTypePercent.AutoSize = True
        Me.rbCalcTypePercent.Checked = True
        Me.rbCalcTypePercent.Location = New System.Drawing.Point(222, 30)
        Me.rbCalcTypePercent.Name = "rbCalcTypePercent"
        Me.rbCalcTypePercent.Size = New System.Drawing.Size(80, 17)
        Me.rbCalcTypePercent.TabIndex = 6
        Me.rbCalcTypePercent.TabStop = True
        Me.rbCalcTypePercent.Text = "Percentage"
        Me.rbCalcTypePercent.UseVisualStyleBackColor = True
        '
        'rbCalcTypeValue
        '
        Me.rbCalcTypeValue.AutoSize = True
        Me.rbCalcTypeValue.Location = New System.Drawing.Point(324, 30)
        Me.rbCalcTypeValue.Name = "rbCalcTypeValue"
        Me.rbCalcTypeValue.Size = New System.Drawing.Size(120, 17)
        Me.rbCalcTypeValue.TabIndex = 7
        Me.rbCalcTypeValue.TabStop = True
        Me.rbCalcTypeValue.Text = "Upper lower bounds"
        Me.rbCalcTypeValue.UseVisualStyleBackColor = True
        '
        'frmMSEBatchTFM
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(787, 475)
        Me.Controls.Add(Me.rbCalcTypeValue)
        Me.Controls.Add(Me.rbCalcTypePercent)
        Me.Controls.Add(Me.txNTFM)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btCalcIters)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.UpDwnIter)
        Me.Controls.Add(Me.m_grid)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSEBatchTFM"
        Me.Text = "frmMSEBatchTFM"
        CType(Me.UpDwnIter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents UpDwnIter As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btCalcIters As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txNTFM As System.Windows.Forms.TextBox
    Private WithEvents m_grid As ScientificInterface.gridMSEBatchTFM
    Friend WithEvents rbCalcTypePercent As System.Windows.Forms.RadioButton
    Friend WithEvents rbCalcTypeValue As System.Windows.Forms.RadioButton
End Class
