<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcotracerOutput
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
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
        Me.components = New System.ComponentModel.Container
        Me.m_zgc = New ZedGraph.ZedGraphControl
        Me.lbGroups = New System.Windows.Forms.ListBox
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.ckSorted = New System.Windows.Forms.CheckBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.lbCommands = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.cmbRegions = New System.Windows.Forms.ComboBox
        Me.rbCB = New System.Windows.Forms.RadioButton
        Me.rbConc = New System.Windows.Forms.RadioButton
        Me.pbProgress = New System.Windows.Forms.ProgressBar
        Me.btRunSpace = New System.Windows.Forms.Button
        Me.btRunSim = New System.Windows.Forms.Button
        Me.m_btnShowHideGroups = New System.Windows.Forms.Button
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_zgc
        '
        Me.m_zgc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_zgc.Location = New System.Drawing.Point(0, 0)
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0
        Me.m_zgc.ScrollMaxX = 0
        Me.m_zgc.ScrollMaxY = 0
        Me.m_zgc.ScrollMaxY2 = 0
        Me.m_zgc.ScrollMinX = 0
        Me.m_zgc.ScrollMinY = 0
        Me.m_zgc.ScrollMinY2 = 0
        Me.m_zgc.Size = New System.Drawing.Size(762, 700)
        Me.m_zgc.TabIndex = 0
        '
        'lbGroups
        '
        Me.lbGroups.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.lbGroups.FormattingEnabled = True
        Me.lbGroups.IntegralHeight = False
        Me.lbGroups.Location = New System.Drawing.Point(0, 0)
        Me.lbGroups.Name = "lbGroups"
        Me.lbGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lbGroups.Size = New System.Drawing.Size(180, 425)
        Me.lbGroups.TabIndex = 0
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 1)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.m_btnShowHideGroups)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cmbRegions)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ckSorted)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lbCommands)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.rbCB)
        Me.SplitContainer1.Panel1.Controls.Add(Me.rbConc)
        Me.SplitContainer1.Panel1.Controls.Add(Me.pbProgress)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btRunSpace)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btRunSim)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lbGroups)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.m_zgc)
        Me.SplitContainer1.Size = New System.Drawing.Size(946, 700)
        Me.SplitContainer1.SplitterDistance = 180
        Me.SplitContainer1.TabIndex = 2
        '
        'ckSorted
        '
        Me.ckSorted.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ckSorted.AutoSize = True
        Me.ckSorted.Location = New System.Drawing.Point(3, 560)
        Me.ckSorted.Name = "ckSorted"
        Me.ckSorted.Size = New System.Drawing.Size(80, 17)
        Me.ckSorted.TabIndex = 6
        Me.ckSorted.Text = "&Sort groups"
        Me.ckSorted.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label2.Location = New System.Drawing.Point(0, 508)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(180, 20)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Plot Options"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbCommands
        '
        Me.lbCommands.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbCommands.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lbCommands.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbCommands.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lbCommands.Location = New System.Drawing.Point(0, 428)
        Me.lbCommands.Name = "lbCommands"
        Me.lbCommands.Size = New System.Drawing.Size(180, 20)
        Me.lbCommands.TabIndex = 1
        Me.lbCommands.Text = "Commands"
        Me.lbCommands.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(0, 640)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 13)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Select &region(s):"
        '
        'cmbRegions
        '
        Me.cmbRegions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbRegions.FormattingEnabled = True
        Me.cmbRegions.Location = New System.Drawing.Point(3, 657)
        Me.cmbRegions.Name = "cmbRegions"
        Me.cmbRegions.Size = New System.Drawing.Size(177, 21)
        Me.cmbRegions.TabIndex = 10
        '
        'rbCB
        '
        Me.rbCB.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.rbCB.AutoSize = True
        Me.rbCB.Checked = True
        Me.rbCB.Location = New System.Drawing.Point(3, 587)
        Me.rbCB.Name = "rbCB"
        Me.rbCB.Size = New System.Drawing.Size(141, 17)
        Me.rbCB.TabIndex = 7
        Me.rbCB.TabStop = True
        Me.rbCB.Text = "Concentration / &Biomass"
        Me.rbCB.UseVisualStyleBackColor = True
        '
        'rbConc
        '
        Me.rbConc.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.rbConc.AutoSize = True
        Me.rbConc.Location = New System.Drawing.Point(3, 610)
        Me.rbConc.Name = "rbConc"
        Me.rbConc.Size = New System.Drawing.Size(91, 17)
        Me.rbConc.TabIndex = 8
        Me.rbConc.Text = "&Concentration"
        Me.rbConc.UseVisualStyleBackColor = True
        '
        'pbProgress
        '
        Me.pbProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbProgress.Location = New System.Drawing.Point(0, 684)
        Me.pbProgress.Name = "pbProgress"
        Me.pbProgress.Size = New System.Drawing.Size(180, 16)
        Me.pbProgress.TabIndex = 11
        '
        'btRunSpace
        '
        Me.btRunSpace.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btRunSpace.Location = New System.Drawing.Point(3, 478)
        Me.btRunSpace.Name = "btRunSpace"
        Me.btRunSpace.Size = New System.Drawing.Size(177, 22)
        Me.btRunSpace.TabIndex = 3
        Me.btRunSpace.Text = "Run Ecosp&ace"
        Me.btRunSpace.UseVisualStyleBackColor = True
        '
        'btRunSim
        '
        Me.btRunSim.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btRunSim.Location = New System.Drawing.Point(3, 451)
        Me.btRunSim.Name = "btRunSim"
        Me.btRunSim.Size = New System.Drawing.Size(177, 21)
        Me.btRunSim.TabIndex = 2
        Me.btRunSim.Text = "Run Ecos&im"
        Me.btRunSim.UseVisualStyleBackColor = True
        '
        'm_btnShowHideGroups
        '
        Me.m_btnShowHideGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnShowHideGroups.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
        Me.m_btnShowHideGroups.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.m_btnShowHideGroups.Location = New System.Drawing.Point(0, 531)
        Me.m_btnShowHideGroups.Name = "m_btnShowHideGroups"
        Me.m_btnShowHideGroups.Size = New System.Drawing.Size(180, 23)
        Me.m_btnShowHideGroups.TabIndex = 5
        Me.m_btnShowHideGroups.Text = "Show &groups..."
        Me.m_btnShowHideGroups.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.m_btnShowHideGroups.UseVisualStyleBackColor = True
        '
        'frmEcotracerOutput
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(946, 701)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "frmEcotracerOutput"
        Me.Text = "frmEcotracerOutput"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_zgc As ZedGraph.ZedGraphControl
    Friend WithEvents lbGroups As System.Windows.Forms.ListBox
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents btRunSim As System.Windows.Forms.Button
    Friend WithEvents btRunSpace As System.Windows.Forms.Button
    Friend WithEvents pbProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents rbCB As System.Windows.Forms.RadioButton
    Friend WithEvents rbConc As System.Windows.Forms.RadioButton
    Friend WithEvents cmbRegions As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbCommands As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ckSorted As System.Windows.Forms.CheckBox
    Friend WithEvents m_btnShowHideGroups As System.Windows.Forms.Button
End Class
