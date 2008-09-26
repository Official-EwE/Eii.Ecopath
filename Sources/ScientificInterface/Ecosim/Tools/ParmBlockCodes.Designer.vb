Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ParmBlockCodes
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
            Me.pbxBlockCodes = New System.Windows.Forms.PictureBox
            Me.nudNumBlockCodes = New System.Windows.Forms.NumericUpDown
            Me.nudSelectedBlockCode = New System.Windows.Forms.NumericUpDown
            Me.lblNumBlocks = New System.Windows.Forms.Label
            Me.lblSelectedBlock = New System.Windows.Forms.Label
            Me.slSelectedBlockCode = New ucSlider
            CType(Me.pbxBlockCodes, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nudNumBlockCodes, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nudSelectedBlockCode, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'pbxBlockCodes
            '
            Me.pbxBlockCodes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pbxBlockCodes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.pbxBlockCodes.Location = New System.Drawing.Point(139, 3)
            Me.pbxBlockCodes.Name = "pbxBlockCodes"
            Me.pbxBlockCodes.Size = New System.Drawing.Size(371, 20)
            Me.pbxBlockCodes.TabIndex = 0
            Me.pbxBlockCodes.TabStop = False
            '
            'nudNumBlockCodes
            '
            Me.nudNumBlockCodes.Location = New System.Drawing.Point(82, 3)
            Me.nudNumBlockCodes.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
            Me.nudNumBlockCodes.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.nudNumBlockCodes.Name = "nudNumBlockCodes"
            Me.nudNumBlockCodes.Size = New System.Drawing.Size(51, 20)
            Me.nudNumBlockCodes.TabIndex = 1
            Me.nudNumBlockCodes.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'nudSelectedBlockCode
            '
            Me.nudSelectedBlockCode.Location = New System.Drawing.Point(82, 29)
            Me.nudSelectedBlockCode.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.nudSelectedBlockCode.Name = "nudSelectedBlockCode"
            Me.nudSelectedBlockCode.Size = New System.Drawing.Size(51, 20)
            Me.nudSelectedBlockCode.TabIndex = 3
            Me.nudSelectedBlockCode.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'lblNumBlocks
            '
            Me.lblNumBlocks.AutoSize = True
            Me.lblNumBlocks.Location = New System.Drawing.Point(3, 5)
            Me.lblNumBlocks.Name = "lblNumBlocks"
            Me.lblNumBlocks.Size = New System.Drawing.Size(73, 13)
            Me.lblNumBlocks.TabIndex = 0
            Me.lblNumBlocks.Text = "&No. of blocks:"
            '
            'lblSelectedBlock
            '
            Me.lblSelectedBlock.AutoSize = True
            Me.lblSelectedBlock.Location = New System.Drawing.Point(3, 31)
            Me.lblSelectedBlock.Name = "lblSelectedBlock"
            Me.lblSelectedBlock.Size = New System.Drawing.Size(52, 13)
            Me.lblSelectedBlock.TabIndex = 2
            Me.lblSelectedBlock.Text = "&Selected:"
            '
            'slSelectedBlockCode
            '
            Me.slSelectedBlockCode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.slSelectedBlockCode.Location = New System.Drawing.Point(139, 27)
            Me.slSelectedBlockCode.Maximum = 100
            Me.slSelectedBlockCode.Minimum = 0
            Me.slSelectedBlockCode.Name = "slSelectedBlockCode"
            Me.slSelectedBlockCode.Size = New System.Drawing.Size(371, 23)
            Me.slSelectedBlockCode.TabIndex = 4
            Me.slSelectedBlockCode.Value = 0
            '
            'ParmBlockCodes
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.lblSelectedBlock)
            Me.Controls.Add(Me.lblNumBlocks)
            Me.Controls.Add(Me.nudSelectedBlockCode)
            Me.Controls.Add(Me.nudNumBlockCodes)
            Me.Controls.Add(Me.pbxBlockCodes)
            Me.Controls.Add(Me.slSelectedBlockCode)
            Me.Margin = New System.Windows.Forms.Padding(0)
            Me.Name = "ParmBlockCodes"
            Me.Size = New System.Drawing.Size(513, 55)
            CType(Me.pbxBlockCodes, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nudNumBlockCodes, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nudSelectedBlockCode, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents pbxBlockCodes As System.Windows.Forms.PictureBox
        Friend WithEvents nudNumBlockCodes As System.Windows.Forms.NumericUpDown
        Friend WithEvents nudSelectedBlockCode As System.Windows.Forms.NumericUpDown
        Friend WithEvents slSelectedBlockCode As ucSlider
        Friend WithEvents lblNumBlocks As System.Windows.Forms.Label
        Friend WithEvents lblSelectedBlock As System.Windows.Forms.Label

    End Class

End Namespace
