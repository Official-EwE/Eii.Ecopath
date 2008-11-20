<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PedigreePanel
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
        Me.m_lblShow = New System.Windows.Forms.Label
        Me.m_lblRating = New System.Windows.Forms.Label
        Me.m_lblOutput = New System.Windows.Forms.Label
        Me.m_rbBars = New System.Windows.Forms.RadioButton
        Me.m_rbColors = New System.Windows.Forms.RadioButton
        Me.m_rbOff = New System.Windows.Forms.RadioButton
        Me.m_btnEditRating = New System.Windows.Forms.Button
        Me.m_lbRatingSelection = New System.Windows.Forms.ListBox
        Me.m_rbSample = New System.Windows.Forms.RadioButton
        Me.m_btnSave = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'm_lblShow
        '
        Me.m_lblShow.BackColor = System.Drawing.SystemColors.ControlDark
        Me.m_lblShow.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_lblShow.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_lblShow.Location = New System.Drawing.Point(4, 3)
        Me.m_lblShow.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lblShow.Name = "m_lblShow"
        Me.m_lblShow.Size = New System.Drawing.Size(157, 18)
        Me.m_lblShow.TabIndex = 1
        Me.m_lblShow.Text = "Show"
        Me.m_lblShow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblRating
        '
        Me.m_lblRating.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lblRating.BackColor = System.Drawing.SystemColors.ControlDark
        Me.m_lblRating.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_lblRating.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_lblRating.Location = New System.Drawing.Point(168, 3)
        Me.m_lblRating.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lblRating.Name = "m_lblRating"
        Me.m_lblRating.Size = New System.Drawing.Size(273, 18)
        Me.m_lblRating.TabIndex = 2
        Me.m_lblRating.Text = "Rating"
        Me.m_lblRating.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblOutput
        '
        Me.m_lblOutput.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lblOutput.BackColor = System.Drawing.SystemColors.ControlDark
        Me.m_lblOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_lblOutput.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_lblOutput.Location = New System.Drawing.Point(447, 3)
        Me.m_lblOutput.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lblOutput.Name = "m_lblOutput"
        Me.m_lblOutput.Size = New System.Drawing.Size(102, 18)
        Me.m_lblOutput.TabIndex = 3
        Me.m_lblOutput.Text = "Output"
        Me.m_lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_rbBars
        '
        Me.m_rbBars.AutoSize = True
        Me.m_rbBars.Location = New System.Drawing.Point(13, 25)
        Me.m_rbBars.Name = "m_rbBars"
        Me.m_rbBars.Size = New System.Drawing.Size(46, 17)
        Me.m_rbBars.TabIndex = 4
        Me.m_rbBars.TabStop = True
        Me.m_rbBars.Text = "Bars"
        Me.m_rbBars.UseVisualStyleBackColor = True
        '
        'm_rbColors
        '
        Me.m_rbColors.AutoSize = True
        Me.m_rbColors.Location = New System.Drawing.Point(13, 41)
        Me.m_rbColors.Name = "m_rbColors"
        Me.m_rbColors.Size = New System.Drawing.Size(54, 17)
        Me.m_rbColors.TabIndex = 5
        Me.m_rbColors.TabStop = True
        Me.m_rbColors.Text = "Colors"
        Me.m_rbColors.UseVisualStyleBackColor = True
        '
        'm_rbOff
        '
        Me.m_rbOff.AutoSize = True
        Me.m_rbOff.Location = New System.Drawing.Point(13, 58)
        Me.m_rbOff.Name = "m_rbOff"
        Me.m_rbOff.Size = New System.Drawing.Size(39, 17)
        Me.m_rbOff.TabIndex = 6
        Me.m_rbOff.TabStop = True
        Me.m_rbOff.Text = "Off"
        Me.m_rbOff.UseVisualStyleBackColor = True
        '
        'm_btnEditRating
        '
        Me.m_btnEditRating.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnEditRating.Font = New System.Drawing.Font("Microsoft Sans Serif", 5.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_btnEditRating.Location = New System.Drawing.Point(385, 5)
        Me.m_btnEditRating.Name = "m_btnEditRating"
        Me.m_btnEditRating.Size = New System.Drawing.Size(52, 14)
        Me.m_btnEditRating.TabIndex = 7
        Me.m_btnEditRating.Text = "Edit"
        Me.m_btnEditRating.UseVisualStyleBackColor = True
        '
        'm_lbRatingSelection
        '
        Me.m_lbRatingSelection.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbRatingSelection.BackColor = System.Drawing.SystemColors.Control
        Me.m_lbRatingSelection.FormattingEnabled = True
        Me.m_lbRatingSelection.Location = New System.Drawing.Point(169, 43)
        Me.m_lbRatingSelection.Name = "m_lbRatingSelection"
        Me.m_lbRatingSelection.Size = New System.Drawing.Size(268, 82)
        Me.m_lbRatingSelection.TabIndex = 8
        '
        'm_rbSample
        '
        Me.m_rbSample.AutoSize = True
        Me.m_rbSample.Location = New System.Drawing.Point(169, 25)
        Me.m_rbSample.Name = "m_rbSample"
        Me.m_rbSample.Size = New System.Drawing.Size(203, 17)
        Me.m_rbSample.TabIndex = 9
        Me.m_rbSample.TabStop = True
        Me.m_rbSample.Text = "|==| (ConfInt)(IndexVal) Sample Option"
        Me.m_rbSample.UseVisualStyleBackColor = True
        '
        'm_btnSave
        '
        Me.m_btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnSave.Location = New System.Drawing.Point(457, 25)
        Me.m_btnSave.Name = "m_btnSave"
        Me.m_btnSave.Size = New System.Drawing.Size(82, 22)
        Me.m_btnSave.TabIndex = 10
        Me.m_btnSave.Text = "Save grid"
        Me.m_btnSave.UseVisualStyleBackColor = True
        '
        'PedigreePanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(551, 139)
        Me.Controls.Add(Me.m_btnSave)
        Me.Controls.Add(Me.m_rbSample)
        Me.Controls.Add(Me.m_lbRatingSelection)
        Me.Controls.Add(Me.m_btnEditRating)
        Me.Controls.Add(Me.m_rbOff)
        Me.Controls.Add(Me.m_rbColors)
        Me.Controls.Add(Me.m_rbBars)
        Me.Controls.Add(Me.m_lblOutput)
        Me.Controls.Add(Me.m_lblRating)
        Me.Controls.Add(Me.m_lblShow)
        Me.Name = "PedigreePanel"
        Me.Text = "PedigreePanel"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lblShow As System.Windows.Forms.Label
    Private WithEvents m_lblRating As System.Windows.Forms.Label
    Private WithEvents m_lblOutput As System.Windows.Forms.Label
    Friend WithEvents m_rbBars As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbColors As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbOff As System.Windows.Forms.RadioButton
    Friend WithEvents m_btnEditRating As System.Windows.Forms.Button
    Friend WithEvents m_lbRatingSelection As System.Windows.Forms.ListBox
    Friend WithEvents m_rbSample As System.Windows.Forms.RadioButton
    Friend WithEvents m_btnSave As System.Windows.Forms.Button
End Class
