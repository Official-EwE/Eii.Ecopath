Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucPolicyColorBlocks
        Inherits System.Windows.Forms.UserControl


        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_pbFishingBlocks = New System.Windows.Forms.PictureBox
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.m_nudSeqEndYear = New System.Windows.Forms.NumericUpDown
            Me.m_lblEndYear = New System.Windows.Forms.Label
            Me.m_nudSeqStartYear = New System.Windows.Forms.NumericUpDown
            Me.m_lblStartYear = New System.Windows.Forms.Label
            Me.m_nudNumYearsPerBlock = New System.Windows.Forms.NumericUpDown
            Me.m_btnSetGear = New System.Windows.Forms.Button
            Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
            Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel
            Me.pnlControls = New System.Windows.Forms.Panel
            Me.Label1 = New System.Windows.Forms.Label
            CType(Me.m_pbFishingBlocks, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudSeqEndYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudSeqStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudNumYearsPerBlock, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tlpMain.SuspendLayout()
            Me.pnlControls.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_pbFishingBlocks
            '
            Me.m_pbFishingBlocks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pbFishingBlocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_pbFishingBlocks.Location = New System.Drawing.Point(0, 95)
            Me.m_pbFishingBlocks.Margin = New System.Windows.Forms.Padding(0)
            Me.m_pbFishingBlocks.Name = "m_pbFishingBlocks"
            Me.m_pbFishingBlocks.Size = New System.Drawing.Size(870, 493)
            Me.m_pbFishingBlocks.TabIndex = 0
            Me.m_pbFishingBlocks.TabStop = False
            '
            'lblInitializationHeader
            '
            Me.lblInitializationHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitializationHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitializationHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblInitializationHeader.Location = New System.Drawing.Point(3, 0)
            Me.lblInitializationHeader.Name = "lblInitializationHeader"
            Me.lblInitializationHeader.Size = New System.Drawing.Size(583, 17)
            Me.lblInitializationHeader.TabIndex = 10
            Me.lblInitializationHeader.Text = "Blocks"
            Me.lblInitializationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_nudSeqEndYear
            '
            Me.m_nudSeqEndYear.Location = New System.Drawing.Point(230, 6)
            Me.m_nudSeqEndYear.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudSeqEndYear.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudSeqEndYear.Name = "m_nudSeqEndYear"
            Me.m_nudSeqEndYear.Size = New System.Drawing.Size(36, 20)
            Me.m_nudSeqEndYear.TabIndex = 8
            Me.m_nudSeqEndYear.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'm_lblEndYear
            '
            Me.m_lblEndYear.AutoSize = True
            Me.m_lblEndYear.Location = New System.Drawing.Point(203, 10)
            Me.m_lblEndYear.Name = "m_lblEndYear"
            Me.m_lblEndYear.Size = New System.Drawing.Size(29, 13)
            Me.m_lblEndYear.TabIndex = 7
            Me.m_lblEndYear.Text = "End:"
            '
            'm_nudSeqStartYear
            '
            Me.m_nudSeqStartYear.Location = New System.Drawing.Point(158, 6)
            Me.m_nudSeqStartYear.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudSeqStartYear.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudSeqStartYear.Name = "m_nudSeqStartYear"
            Me.m_nudSeqStartYear.Size = New System.Drawing.Size(36, 20)
            Me.m_nudSeqStartYear.TabIndex = 6
            Me.m_nudSeqStartYear.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'm_lblStartYear
            '
            Me.m_lblStartYear.AutoSize = True
            Me.m_lblStartYear.Location = New System.Drawing.Point(127, 10)
            Me.m_lblStartYear.Name = "m_lblStartYear"
            Me.m_lblStartYear.Size = New System.Drawing.Size(32, 13)
            Me.m_lblStartYear.TabIndex = 5
            Me.m_lblStartYear.Text = "Start:"
            '
            'm_nudNumYearsPerBlock
            '
            Me.m_nudNumYearsPerBlock.Location = New System.Drawing.Point(7, 6)
            Me.m_nudNumYearsPerBlock.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudNumYearsPerBlock.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudNumYearsPerBlock.Name = "m_nudNumYearsPerBlock"
            Me.m_nudNumYearsPerBlock.Size = New System.Drawing.Size(36, 20)
            Me.m_nudNumYearsPerBlock.TabIndex = 2
            Me.m_nudNumYearsPerBlock.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_btnSetGear
            '
            Me.m_btnSetGear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.m_btnSetGear.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_btnSetGear.Location = New System.Drawing.Point(46, 5)
            Me.m_btnSetGear.Name = "m_btnSetGear"
            Me.m_btnSetGear.Size = New System.Drawing.Size(66, 22)
            Me.m_btnSetGear.TabIndex = 3
            Me.m_btnSetGear.Text = "&Set gear"
            Me.m_btnSetGear.UseVisualStyleBackColor = True
            '
            'tlpMain
            '
            Me.tlpMain.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tlpMain.ColumnCount = 2
            Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 281.0!))
            Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.tlpMain.Controls.Add(Me.Label1, 1, 0)
            Me.tlpMain.Controls.Add(Me.pnlControls, 1, 1)
            Me.tlpMain.Controls.Add(Me.lblInitializationHeader, 0, 0)
            Me.tlpMain.Location = New System.Drawing.Point(0, 0)
            Me.tlpMain.Name = "tlpMain"
            Me.tlpMain.RowCount = 2
            Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.94737!))
            Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 81.05263!))
            Me.tlpMain.Size = New System.Drawing.Size(870, 92)
            Me.tlpMain.TabIndex = 13
            '
            'TableLayoutPanel3
            '
            Me.TableLayoutPanel3.ColumnCount = 4
            Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TableLayoutPanel3.Location = New System.Drawing.Point(0, 0)
            Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
            Me.TableLayoutPanel3.RowCount = 1
            Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel3.Size = New System.Drawing.Size(200, 100)
            Me.TableLayoutPanel3.TabIndex = 0
            '
            'pnlControls
            '
            Me.pnlControls.Controls.Add(Me.m_nudSeqEndYear)
            Me.pnlControls.Controls.Add(Me.m_lblEndYear)
            Me.pnlControls.Controls.Add(Me.m_nudSeqStartYear)
            Me.pnlControls.Controls.Add(Me.m_btnSetGear)
            Me.pnlControls.Controls.Add(Me.m_lblStartYear)
            Me.pnlControls.Controls.Add(Me.m_nudNumYearsPerBlock)
            Me.pnlControls.Location = New System.Drawing.Point(592, 20)
            Me.pnlControls.Name = "pnlControls"
            Me.pnlControls.Size = New System.Drawing.Size(274, 69)
            Me.pnlControls.TabIndex = 14
            '
            'Label1
            '
            Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.Label1.Location = New System.Drawing.Point(592, 0)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(275, 17)
            Me.Label1.TabIndex = 14
            Me.Label1.Text = "Set block years and sequence"
            Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'ucPolicyColorBlocks
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.tlpMain)
            Me.Controls.Add(Me.m_pbFishingBlocks)
            Me.Name = "ucPolicyColorBlocks"
            Me.Size = New System.Drawing.Size(870, 588)
            CType(Me.m_pbFishingBlocks, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudSeqEndYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudSeqStartYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudNumYearsPerBlock, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tlpMain.ResumeLayout(False)
            Me.pnlControls.ResumeLayout(False)
            Me.pnlControls.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_pbFishingBlocks As System.Windows.Forms.PictureBox
        Private WithEvents m_nudNumYearsPerBlock As System.Windows.Forms.NumericUpDown
        Private WithEvents m_btnSetGear As System.Windows.Forms.Button
        Private WithEvents m_nudSeqEndYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblEndYear As System.Windows.Forms.Label
        Private WithEvents m_nudSeqStartYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblStartYear As System.Windows.Forms.Label
        Private WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents pnlControls As System.Windows.Forms.Panel
        Private WithEvents Label1 As System.Windows.Forms.Label

    End Class

End Namespace