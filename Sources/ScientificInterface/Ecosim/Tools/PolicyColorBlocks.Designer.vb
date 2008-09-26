Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class PolicyColorBlocks
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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.pbFishingBlocks = New System.Windows.Forms.PictureBox
            Me.Label1 = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.nupSeqEndYear = New System.Windows.Forms.NumericUpDown
            Me.lblEndYear = New System.Windows.Forms.Label
            Me.nupSeqStartYear = New System.Windows.Forms.NumericUpDown
            Me.lblStartYear = New System.Windows.Forms.Label
            Me.nupYearBlockNum = New System.Windows.Forms.NumericUpDown
            Me.btnSetEveryGear = New System.Windows.Forms.Button
            Me.m_blockCodes = New ScientificInterface.Ecosim.ParmBlockCodes
            CType(Me.pbFishingBlocks, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nupSeqEndYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nupSeqStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nupYearBlockNum, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'pbFishingBlocks
            '
            Me.pbFishingBlocks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pbFishingBlocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pbFishingBlocks.Location = New System.Drawing.Point(0, 72)
            Me.pbFishingBlocks.Margin = New System.Windows.Forms.Padding(0)
            Me.pbFishingBlocks.Name = "pbFishingBlocks"
            Me.pbFishingBlocks.Size = New System.Drawing.Size(694, 164)
            Me.pbFishingBlocks.TabIndex = 0
            Me.pbFishingBlocks.TabStop = False
            '
            'Label1
            '
            Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.Label1.Location = New System.Drawing.Point(532, 0)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(164, 18)
            Me.Label1.TabIndex = 12
            Me.Label1.Text = "Sequence year"
            Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Label2
            '
            Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label2.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.Label2.Location = New System.Drawing.Point(418, 0)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(108, 18)
            Me.Label2.TabIndex = 11
            Me.Label2.Text = "Years/Block"
            Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
            'nupSeqEndYear
            '
            Me.nupSeqEndYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.nupSeqEndYear.Location = New System.Drawing.Point(647, 21)
            Me.nupSeqEndYear.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.nupSeqEndYear.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.nupSeqEndYear.Name = "nupSeqEndYear"
            Me.nupSeqEndYear.Size = New System.Drawing.Size(47, 20)
            Me.nupSeqEndYear.TabIndex = 8
            Me.nupSeqEndYear.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'lblEndYear
            '
            Me.lblEndYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblEndYear.Location = New System.Drawing.Point(610, 23)
            Me.lblEndYear.Name = "lblEndYear"
            Me.lblEndYear.Size = New System.Drawing.Size(31, 16)
            Me.lblEndYear.TabIndex = 7
            Me.lblEndYear.Text = "End:"
            '
            'nupSeqStartYear
            '
            Me.nupSeqStartYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.nupSeqStartYear.Location = New System.Drawing.Point(565, 21)
            Me.nupSeqStartYear.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.nupSeqStartYear.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.nupSeqStartYear.Name = "nupSeqStartYear"
            Me.nupSeqStartYear.Size = New System.Drawing.Size(39, 20)
            Me.nupSeqStartYear.TabIndex = 6
            Me.nupSeqStartYear.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'lblStartYear
            '
            Me.lblStartYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblStartYear.Location = New System.Drawing.Point(532, 23)
            Me.lblStartYear.Name = "lblStartYear"
            Me.lblStartYear.Size = New System.Drawing.Size(36, 20)
            Me.lblStartYear.TabIndex = 5
            Me.lblStartYear.Text = "Start:"
            '
            'nupYearBlockNum
            '
            Me.nupYearBlockNum.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.nupYearBlockNum.Location = New System.Drawing.Point(418, 21)
            Me.nupYearBlockNum.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.nupYearBlockNum.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.nupYearBlockNum.Name = "nupYearBlockNum"
            Me.nupYearBlockNum.Size = New System.Drawing.Size(45, 20)
            Me.nupYearBlockNum.TabIndex = 2
            Me.nupYearBlockNum.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'btnSetEveryGear
            '
            Me.btnSetEveryGear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSetEveryGear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnSetEveryGear.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.btnSetEveryGear.Location = New System.Drawing.Point(469, 21)
            Me.btnSetEveryGear.Name = "btnSetEveryGear"
            Me.btnSetEveryGear.Size = New System.Drawing.Size(57, 22)
            Me.btnSetEveryGear.TabIndex = 3
            Me.btnSetEveryGear.Text = "&Set gear"
            Me.btnSetEveryGear.UseVisualStyleBackColor = True
            '
            'm_blockCodes
            '
            Me.m_blockCodes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_blockCodes.Location = New System.Drawing.Point(-2, 18)
            Me.m_blockCodes.Margin = New System.Windows.Forms.Padding(0)
            Me.m_blockCodes.Name = "m_blockCodes"
            Me.m_blockCodes.nBlockCodes = 30
            Me.m_blockCodes.SelectedBlockNum = 15
            Me.m_blockCodes.Size = New System.Drawing.Size(417, 54)
            Me.m_blockCodes.TabIndex = 0
            '
            'PolicyColorBlocks
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pbFishingBlocks)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.lblInitializationHeader)
            Me.Controls.Add(Me.m_blockCodes)
            Me.Controls.Add(Me.nupSeqEndYear)
            Me.Controls.Add(Me.btnSetEveryGear)
            Me.Controls.Add(Me.lblEndYear)
            Me.Controls.Add(Me.nupYearBlockNum)
            Me.Controls.Add(Me.nupSeqStartYear)
            Me.Controls.Add(Me.lblStartYear)
            Me.Name = "PolicyColorBlocks"
            Me.Size = New System.Drawing.Size(694, 236)
            CType(Me.pbFishingBlocks, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nupSeqEndYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nupSeqStartYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nupYearBlockNum, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents pbFishingBlocks As System.Windows.Forms.PictureBox
        Friend WithEvents m_blockCodes As ParmBlockCodes
        Friend WithEvents nupYearBlockNum As System.Windows.Forms.NumericUpDown
        Friend WithEvents btnSetEveryGear As System.Windows.Forms.Button
        Friend WithEvents nupSeqEndYear As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblEndYear As System.Windows.Forms.Label
        Friend WithEvents nupSeqStartYear As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblStartYear As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label

    End Class

End Namespace