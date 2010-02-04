Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucPolicyColorBlocks
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_pbFishingBlocks = New System.Windows.Forms.PictureBox
            Me.m_lblSequenceYear = New System.Windows.Forms.Label
            Me.m_lblYears = New System.Windows.Forms.Label
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.m_nudSeqEndYear = New System.Windows.Forms.NumericUpDown
            Me.m_lblEndYear = New System.Windows.Forms.Label
            Me.m_nudSeqStartYear = New System.Windows.Forms.NumericUpDown
            Me.m_lblStartYear = New System.Windows.Forms.Label
            Me.m_nudNumYearsPerBlock = New System.Windows.Forms.NumericUpDown
            Me.m_btnSetGear = New System.Windows.Forms.Button
            Me.m_blockCodes = New ScientificInterface.Ecosim.ucParmBlockCodes
            CType(Me.m_pbFishingBlocks, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudSeqEndYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudSeqStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudNumYearsPerBlock, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_pbFishingBlocks
            '
            Me.m_pbFishingBlocks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pbFishingBlocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_pbFishingBlocks.Location = New System.Drawing.Point(0, 72)
            Me.m_pbFishingBlocks.Margin = New System.Windows.Forms.Padding(0)
            Me.m_pbFishingBlocks.Name = "m_pbFishingBlocks"
            Me.m_pbFishingBlocks.Size = New System.Drawing.Size(694, 164)
            Me.m_pbFishingBlocks.TabIndex = 0
            Me.m_pbFishingBlocks.TabStop = False
            '
            'm_lblSequenceYear
            '
            Me.m_lblSequenceYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblSequenceYear.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblSequenceYear.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.m_lblSequenceYear.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblSequenceYear.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblSequenceYear.Location = New System.Drawing.Point(532, 0)
            Me.m_lblSequenceYear.Name = "m_lblSequenceYear"
            Me.m_lblSequenceYear.Size = New System.Drawing.Size(164, 18)
            Me.m_lblSequenceYear.TabIndex = 12
            Me.m_lblSequenceYear.Text = "Sequence year"
            Me.m_lblSequenceYear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblYears
            '
            Me.m_lblYears.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblYears.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblYears.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.m_lblYears.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblYears.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblYears.Location = New System.Drawing.Point(418, 0)
            Me.m_lblYears.Name = "m_lblYears"
            Me.m_lblYears.Size = New System.Drawing.Size(108, 18)
            Me.m_lblYears.TabIndex = 11
            Me.m_lblYears.Text = "Years/Block"
            Me.m_lblYears.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblInitializationHeader
            '
            Me.lblInitializationHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitializationHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitializationHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblInitializationHeader.Location = New System.Drawing.Point(0, 0)
            Me.lblInitializationHeader.Name = "lblInitializationHeader"
            Me.lblInitializationHeader.Size = New System.Drawing.Size(412, 18)
            Me.lblInitializationHeader.TabIndex = 10
            Me.lblInitializationHeader.Text = "Blocks"
            Me.lblInitializationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_nudSeqEndYear
            '
            Me.m_nudSeqEndYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudSeqEndYear.Location = New System.Drawing.Point(647, 21)
            Me.m_nudSeqEndYear.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudSeqEndYear.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudSeqEndYear.Name = "m_nudSeqEndYear"
            Me.m_nudSeqEndYear.Size = New System.Drawing.Size(47, 20)
            Me.m_nudSeqEndYear.TabIndex = 8
            Me.m_nudSeqEndYear.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'm_lblEndYear
            '
            Me.m_lblEndYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblEndYear.Location = New System.Drawing.Point(610, 23)
            Me.m_lblEndYear.Name = "m_lblEndYear"
            Me.m_lblEndYear.Size = New System.Drawing.Size(31, 16)
            Me.m_lblEndYear.TabIndex = 7
            Me.m_lblEndYear.Text = "End:"
            '
            'm_nudSeqStartYear
            '
            Me.m_nudSeqStartYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudSeqStartYear.Location = New System.Drawing.Point(565, 21)
            Me.m_nudSeqStartYear.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudSeqStartYear.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudSeqStartYear.Name = "m_nudSeqStartYear"
            Me.m_nudSeqStartYear.Size = New System.Drawing.Size(39, 20)
            Me.m_nudSeqStartYear.TabIndex = 6
            Me.m_nudSeqStartYear.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'm_lblStartYear
            '
            Me.m_lblStartYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblStartYear.Location = New System.Drawing.Point(532, 23)
            Me.m_lblStartYear.Name = "m_lblStartYear"
            Me.m_lblStartYear.Size = New System.Drawing.Size(36, 20)
            Me.m_lblStartYear.TabIndex = 5
            Me.m_lblStartYear.Text = "Start:"
            '
            'm_nudNumYearsPerBlock
            '
            Me.m_nudNumYearsPerBlock.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudNumYearsPerBlock.Location = New System.Drawing.Point(418, 21)
            Me.m_nudNumYearsPerBlock.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudNumYearsPerBlock.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudNumYearsPerBlock.Name = "m_nudNumYearsPerBlock"
            Me.m_nudNumYearsPerBlock.Size = New System.Drawing.Size(45, 20)
            Me.m_nudNumYearsPerBlock.TabIndex = 2
            Me.m_nudNumYearsPerBlock.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_btnSetGear
            '
            Me.m_btnSetGear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnSetGear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.m_btnSetGear.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_btnSetGear.Location = New System.Drawing.Point(469, 21)
            Me.m_btnSetGear.Name = "m_btnSetGear"
            Me.m_btnSetGear.Size = New System.Drawing.Size(57, 22)
            Me.m_btnSetGear.TabIndex = 3
            Me.m_btnSetGear.Text = "&Set gear"
            Me.m_btnSetGear.UseVisualStyleBackColor = True
            '
            'm_blockCodes
            '
            Me.m_blockCodes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_blockCodes.Location = New System.Drawing.Point(-2, 18)
            Me.m_blockCodes.Margin = New System.Windows.Forms.Padding(0)
            Me.m_blockCodes.Name = "m_blockCodes"
            Me.m_blockCodes.NumBlocks = 30
            Me.m_blockCodes.SelectedBlock = 15
            Me.m_blockCodes.Size = New System.Drawing.Size(417, 54)
            Me.m_blockCodes.TabIndex = 0
            '
            'ucPolicyColorBlocks
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_pbFishingBlocks)
            Me.Controls.Add(Me.m_lblSequenceYear)
            Me.Controls.Add(Me.m_lblYears)
            Me.Controls.Add(Me.lblInitializationHeader)
            Me.Controls.Add(Me.m_blockCodes)
            Me.Controls.Add(Me.m_nudSeqEndYear)
            Me.Controls.Add(Me.m_btnSetGear)
            Me.Controls.Add(Me.m_lblEndYear)
            Me.Controls.Add(Me.m_nudNumYearsPerBlock)
            Me.Controls.Add(Me.m_nudSeqStartYear)
            Me.Controls.Add(Me.m_lblStartYear)
            Me.Name = "ucPolicyColorBlocks"
            Me.Size = New System.Drawing.Size(694, 236)
            CType(Me.m_pbFishingBlocks, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudSeqEndYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudSeqStartYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudNumYearsPerBlock, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_pbFishingBlocks As System.Windows.Forms.PictureBox
        Private WithEvents m_blockCodes As ucParmBlockCodes
        Private WithEvents m_nudNumYearsPerBlock As System.Windows.Forms.NumericUpDown
        Private WithEvents m_btnSetGear As System.Windows.Forms.Button
        Private WithEvents m_nudSeqEndYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblEndYear As System.Windows.Forms.Label
        Private WithEvents m_nudSeqStartYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblStartYear As System.Windows.Forms.Label
        Private WithEvents m_lblYears As System.Windows.Forms.Label
        Private WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Private WithEvents m_lblSequenceYear As System.Windows.Forms.Label

    End Class

End Namespace