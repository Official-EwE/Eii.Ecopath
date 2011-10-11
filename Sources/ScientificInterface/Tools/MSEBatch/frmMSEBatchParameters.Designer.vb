
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEBatchParameters
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
        Me.chkSaveBiomass = New System.Windows.Forms.CheckBox()
        Me.chkCatch = New System.Windows.Forms.CheckBox()
        Me.chkFishingMort = New System.Windows.Forms.CheckBox()
        Me.chkPredMort = New System.Windows.Forms.CheckBox()
        Me.chkQB = New System.Windows.Forms.CheckBox()
        Me.chkFeedingTime = New System.Windows.Forms.CheckBox()
        Me.eweHdrSave = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.SuspendLayout()
        '
        'chkSaveBiomass
        '
        Me.chkSaveBiomass.AutoSize = True
        Me.chkSaveBiomass.Location = New System.Drawing.Point(18, 36)
        Me.chkSaveBiomass.Name = "chkSaveBiomass"
        Me.chkSaveBiomass.Size = New System.Drawing.Size(65, 17)
        Me.chkSaveBiomass.TabIndex = 0
        Me.chkSaveBiomass.Text = "Biomass"
        Me.chkSaveBiomass.UseVisualStyleBackColor = True
        '
        'chkCatch
        '
        Me.chkCatch.AutoSize = True
        Me.chkCatch.Location = New System.Drawing.Point(18, 59)
        Me.chkCatch.Name = "chkCatch"
        Me.chkCatch.Size = New System.Drawing.Size(54, 17)
        Me.chkCatch.TabIndex = 1
        Me.chkCatch.Text = "Catch"
        Me.chkCatch.UseVisualStyleBackColor = True
        '
        'chkFishingMort
        '
        Me.chkFishingMort.AutoSize = True
        Me.chkFishingMort.Location = New System.Drawing.Point(18, 82)
        Me.chkFishingMort.Name = "chkFishingMort"
        Me.chkFishingMort.Size = New System.Drawing.Size(85, 17)
        Me.chkFishingMort.TabIndex = 2
        Me.chkFishingMort.Text = "Fishing mort."
        Me.chkFishingMort.UseVisualStyleBackColor = True
        '
        'chkPredMort
        '
        Me.chkPredMort.AutoSize = True
        Me.chkPredMort.Location = New System.Drawing.Point(141, 59)
        Me.chkPredMort.Name = "chkPredMort"
        Me.chkPredMort.Size = New System.Drawing.Size(97, 17)
        Me.chkPredMort.TabIndex = 3
        Me.chkPredMort.Text = "Predation mort."
        Me.chkPredMort.UseVisualStyleBackColor = True
        '
        'chkQB
        '
        Me.chkQB.AutoSize = True
        Me.chkQB.Location = New System.Drawing.Point(141, 36)
        Me.chkQB.Name = "chkQB"
        Me.chkQB.Size = New System.Drawing.Size(131, 17)
        Me.chkQB.TabIndex = 4
        Me.chkQB.Text = "Consumption/Biomass"
        Me.chkQB.UseVisualStyleBackColor = True
        '
        'chkFeedingTime
        '
        Me.chkFeedingTime.AutoSize = True
        Me.chkFeedingTime.Location = New System.Drawing.Point(141, 82)
        Me.chkFeedingTime.Name = "chkFeedingTime"
        Me.chkFeedingTime.Size = New System.Drawing.Size(86, 17)
        Me.chkFeedingTime.TabIndex = 5
        Me.chkFeedingTime.Text = "Feeding time"
        Me.chkFeedingTime.UseVisualStyleBackColor = True
        '
        'eweHdrSave
        '
        Me.eweHdrSave.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.eweHdrSave.CanCollapseParent = False
        Me.eweHdrSave.CollapsedParentHeight = 0
        Me.eweHdrSave.IsCollapsed = False
        Me.eweHdrSave.Location = New System.Drawing.Point(12, 9)
        Me.eweHdrSave.Name = "eweHdrSave"
        Me.eweHdrSave.Size = New System.Drawing.Size(446, 24)
        Me.eweHdrSave.TabIndex = 6
        Me.eweHdrSave.Text = "Save"
        Me.eweHdrSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmMSEBatchParameters
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(470, 416)
        Me.Controls.Add(Me.eweHdrSave)
        Me.Controls.Add(Me.chkFeedingTime)
        Me.Controls.Add(Me.chkQB)
        Me.Controls.Add(Me.chkPredMort)
        Me.Controls.Add(Me.chkFishingMort)
        Me.Controls.Add(Me.chkCatch)
        Me.Controls.Add(Me.chkSaveBiomass)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSEBatchParameters"
        Me.Text = "MSE batch parameters"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chkSaveBiomass As System.Windows.Forms.CheckBox
    Friend WithEvents chkCatch As System.Windows.Forms.CheckBox
    Friend WithEvents chkFishingMort As System.Windows.Forms.CheckBox
    Friend WithEvents chkPredMort As System.Windows.Forms.CheckBox
    Friend WithEvents chkQB As System.Windows.Forms.CheckBox
    Friend WithEvents chkFeedingTime As System.Windows.Forms.CheckBox
    Friend WithEvents eweHdrSave As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
