Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEPlots
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
        Me.components = New System.ComponentModel.Container
        Me.ZedGraph = New ZedGraph.ZedGraphControl
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.rbBioEst = New System.Windows.Forms.RadioButton
        Me.btShowHide = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.rbEffort = New System.Windows.Forms.RadioButton
        Me.rbFleetValue = New System.Windows.Forms.RadioButton
        Me.rbGroupCatch = New System.Windows.Forms.RadioButton
        Me.rbGroupBiomass = New System.Windows.Forms.RadioButton
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.lbType = New System.Windows.Forms.Label
        Me.rbValues = New System.Windows.Forms.RadioButton
        Me.rbHisto = New System.Windows.Forms.RadioButton
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ZedGraph
        '
        Me.ZedGraph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ZedGraph.Location = New System.Drawing.Point(12, 71)
        Me.ZedGraph.Name = "ZedGraph"
        Me.ZedGraph.ScrollGrace = 0
        Me.ZedGraph.ScrollMaxX = 0
        Me.ZedGraph.ScrollMaxY = 0
        Me.ZedGraph.ScrollMaxY2 = 0
        Me.ZedGraph.ScrollMinX = 0
        Me.ZedGraph.ScrollMinY = 0
        Me.ZedGraph.ScrollMinY2 = 0
        Me.ZedGraph.Size = New System.Drawing.Size(765, 498)
        Me.ZedGraph.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.rbBioEst)
        Me.Panel1.Controls.Add(Me.btShowHide)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.rbEffort)
        Me.Panel1.Controls.Add(Me.rbFleetValue)
        Me.Panel1.Controls.Add(Me.rbGroupCatch)
        Me.Panel1.Controls.Add(Me.rbGroupBiomass)
        Me.Panel1.Location = New System.Drawing.Point(123, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(654, 65)
        Me.Panel1.TabIndex = 5
        '
        'rbBioEst
        '
        Me.rbBioEst.AutoSize = True
        Me.rbBioEst.Location = New System.Drawing.Point(105, 24)
        Me.rbBioEst.Name = "rbBioEst"
        Me.rbBioEst.Size = New System.Drawing.Size(92, 17)
        Me.rbBioEst.TabIndex = 10
        Me.rbBioEst.TabStop = True
        Me.rbBioEst.Text = "B/B estimated"
        Me.rbBioEst.UseVisualStyleBackColor = True
        '
        'btShowHide
        '
        Me.btShowHide.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btShowHide.Location = New System.Drawing.Point(285, 21)
        Me.btShowHide.Name = "btShowHide"
        Me.btShowHide.Size = New System.Drawing.Size(120, 23)
        Me.btShowHide.TabIndex = 7
        Me.btShowHide.Text = "Show/hide items..."
        Me.btShowHide.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label1.Size = New System.Drawing.Size(654, 21)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Plots"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'rbEffort
        '
        Me.rbEffort.AutoSize = True
        Me.rbEffort.Location = New System.Drawing.Point(200, 48)
        Me.rbEffort.Name = "rbEffort"
        Me.rbEffort.Size = New System.Drawing.Size(75, 17)
        Me.rbEffort.TabIndex = 8
        Me.rbEffort.Text = "Fleet effort"
        Me.rbEffort.UseVisualStyleBackColor = True
        '
        'rbFleetValue
        '
        Me.rbFleetValue.AutoSize = True
        Me.rbFleetValue.Location = New System.Drawing.Point(200, 24)
        Me.rbFleetValue.Name = "rbFleetValue"
        Me.rbFleetValue.Size = New System.Drawing.Size(77, 17)
        Me.rbFleetValue.TabIndex = 7
        Me.rbFleetValue.Text = "Fleet value"
        Me.rbFleetValue.UseVisualStyleBackColor = True
        '
        'rbGroupCatch
        '
        Me.rbGroupCatch.AutoSize = True
        Me.rbGroupCatch.Location = New System.Drawing.Point(3, 48)
        Me.rbGroupCatch.Name = "rbGroupCatch"
        Me.rbGroupCatch.Size = New System.Drawing.Size(84, 17)
        Me.rbGroupCatch.TabIndex = 6
        Me.rbGroupCatch.Text = "Group catch"
        Me.rbGroupCatch.UseVisualStyleBackColor = True
        '
        'rbGroupBiomass
        '
        Me.rbGroupBiomass.AutoSize = True
        Me.rbGroupBiomass.Checked = True
        Me.rbGroupBiomass.Location = New System.Drawing.Point(3, 24)
        Me.rbGroupBiomass.Name = "rbGroupBiomass"
        Me.rbGroupBiomass.Size = New System.Drawing.Size(96, 17)
        Me.rbGroupBiomass.TabIndex = 5
        Me.rbGroupBiomass.TabStop = True
        Me.rbGroupBiomass.Text = "Group Biomass"
        Me.rbGroupBiomass.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lbType)
        Me.Panel2.Controls.Add(Me.rbValues)
        Me.Panel2.Controls.Add(Me.rbHisto)
        Me.Panel2.Location = New System.Drawing.Point(-1, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(118, 65)
        Me.Panel2.TabIndex = 6
        '
        'lbType
        '
        Me.lbType.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lbType.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbType.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lbType.Location = New System.Drawing.Point(12, 0)
        Me.lbType.Name = "lbType"
        Me.lbType.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lbType.Size = New System.Drawing.Size(103, 21)
        Me.lbType.TabIndex = 6
        Me.lbType.Text = "Plot type"
        Me.lbType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'rbValues
        '
        Me.rbValues.AutoSize = True
        Me.rbValues.Location = New System.Drawing.Point(15, 48)
        Me.rbValues.Name = "rbValues"
        Me.rbValues.Size = New System.Drawing.Size(57, 17)
        Me.rbValues.TabIndex = 5
        Me.rbValues.Tag = ""
        Me.rbValues.Text = "Values"
        Me.rbValues.UseVisualStyleBackColor = True
        '
        'rbHisto
        '
        Me.rbHisto.AutoSize = True
        Me.rbHisto.Checked = True
        Me.rbHisto.Location = New System.Drawing.Point(15, 24)
        Me.rbHisto.Name = "rbHisto"
        Me.rbHisto.Size = New System.Drawing.Size(72, 17)
        Me.rbHisto.TabIndex = 4
        Me.rbHisto.TabStop = True
        Me.rbHisto.Tag = ""
        Me.rbHisto.Text = "Histogram"
        Me.rbHisto.UseVisualStyleBackColor = True
        '
        'frmMSEPlots
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(789, 581)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ZedGraph)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSEPlots"
        Me.Text = "frmMSEPlots"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ZedGraph As ZedGraph.ZedGraphControl
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents rbEffort As System.Windows.Forms.RadioButton
    Friend WithEvents rbFleetValue As System.Windows.Forms.RadioButton
    Friend WithEvents rbGroupCatch As System.Windows.Forms.RadioButton
    Friend WithEvents rbGroupBiomass As System.Windows.Forms.RadioButton
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents rbValues As System.Windows.Forms.RadioButton
    Friend WithEvents rbHisto As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbType As System.Windows.Forms.Label
    Private WithEvents btShowHide As System.Windows.Forms.Button
    Friend WithEvents rbBioEst As System.Windows.Forms.RadioButton
End Class
