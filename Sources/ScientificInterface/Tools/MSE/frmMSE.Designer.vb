Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSE
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
        Me.btRun = New System.Windows.Forms.Button
        Me.prgProgress = New System.Windows.Forms.ProgressBar
        Me.txNTrials = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lbRun = New System.Windows.Forms.Label
        Me.spInputOutput = New System.Windows.Forms.SplitContainer
        Me.tbObjectives = New System.Windows.Forms.TabControl
        Me.pgObjective = New System.Windows.Forms.TabPage
        Me.pgEcoObjectives = New System.Windows.Forms.TabPage
        Me.pgCV = New System.Windows.Forms.TabPage
        Me.Label4 = New System.Windows.Forms.Label
        Me.panelCV = New ScientificInterface.gridBioCV
        Me.pgFleetWeight = New System.Windows.Forms.TabPage
        Me.Label3 = New System.Windows.Forms.Label
        Me.PanelFleetWeight = New ScientificInterface.gridFishingWeights
        Me.pgCatchabiltiy = New System.Windows.Forms.TabPage
        Me.pgRiskBounds = New System.Windows.Forms.TabPage
        Me.tbOutput = New System.Windows.Forms.TabControl
        Me.pgGraphs = New System.Windows.Forms.TabPage
        Me.zdGraph = New ZedGraph.ZedGraphControl
        Me.pgRisk = New System.Windows.Forms.TabPage
        Me.pgPerformance = New System.Windows.Forms.TabPage
        Me.Label2 = New System.Windows.Forms.Label
        Me.spInputOutput.Panel1.SuspendLayout()
        Me.spInputOutput.Panel2.SuspendLayout()
        Me.spInputOutput.SuspendLayout()
        Me.tbObjectives.SuspendLayout()
        Me.pgCV.SuspendLayout()
        Me.pgFleetWeight.SuspendLayout()
        Me.tbOutput.SuspendLayout()
        Me.pgGraphs.SuspendLayout()
        Me.SuspendLayout()
        '
        'btRun
        '
        Me.btRun.Location = New System.Drawing.Point(250, 43)
        Me.btRun.Name = "btRun"
        Me.btRun.Size = New System.Drawing.Size(93, 20)
        Me.btRun.TabIndex = 0
        Me.btRun.Text = "Run"
        Me.btRun.UseVisualStyleBackColor = True
        '
        'prgProgress
        '
        Me.prgProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.prgProgress.Location = New System.Drawing.Point(4, 68)
        Me.prgProgress.Name = "prgProgress"
        Me.prgProgress.Size = New System.Drawing.Size(1012, 19)
        Me.prgProgress.TabIndex = 1
        '
        'txNTrials
        '
        Me.txNTrials.Location = New System.Drawing.Point(111, 42)
        Me.txNTrials.Name = "txNTrials"
        Me.txNTrials.Size = New System.Drawing.Size(63, 20)
        Me.txNTrials.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 45)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Number of trials"
        '
        'lbRun
        '
        Me.lbRun.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbRun.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lbRun.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbRun.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lbRun.Location = New System.Drawing.Point(1, 0)
        Me.lbRun.Name = "lbRun"
        Me.lbRun.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lbRun.Size = New System.Drawing.Size(1024, 21)
        Me.lbRun.TabIndex = 4
        Me.lbRun.Text = "Run"
        Me.lbRun.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'spInputOutput
        '
        Me.spInputOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.spInputOutput.Location = New System.Drawing.Point(4, 121)
        Me.spInputOutput.Name = "spInputOutput"
        '
        'spInputOutput.Panel1
        '
        Me.spInputOutput.Panel1.Controls.Add(Me.tbObjectives)
        '
        'spInputOutput.Panel2
        '
        Me.spInputOutput.Panel2.Controls.Add(Me.tbOutput)
        Me.spInputOutput.Size = New System.Drawing.Size(1025, 478)
        Me.spInputOutput.SplitterDistance = 529
        Me.spInputOutput.TabIndex = 7
        '
        'tbObjectives
        '
        Me.tbObjectives.Controls.Add(Me.pgObjective)
        Me.tbObjectives.Controls.Add(Me.pgEcoObjectives)
        Me.tbObjectives.Controls.Add(Me.pgCV)
        Me.tbObjectives.Controls.Add(Me.pgFleetWeight)
        Me.tbObjectives.Controls.Add(Me.pgCatchabiltiy)
        Me.tbObjectives.Controls.Add(Me.pgRiskBounds)
        Me.tbObjectives.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbObjectives.Location = New System.Drawing.Point(0, 0)
        Me.tbObjectives.MinimumSize = New System.Drawing.Size(10, 0)
        Me.tbObjectives.Name = "tbObjectives"
        Me.tbObjectives.SelectedIndex = 0
        Me.tbObjectives.Size = New System.Drawing.Size(529, 478)
        Me.tbObjectives.TabIndex = 7
        '
        'pgObjective
        '
        Me.pgObjective.Location = New System.Drawing.Point(4, 22)
        Me.pgObjective.Name = "pgObjective"
        Me.pgObjective.Padding = New System.Windows.Forms.Padding(3)
        Me.pgObjective.Size = New System.Drawing.Size(521, 452)
        Me.pgObjective.TabIndex = 0
        Me.pgObjective.Text = "Objectives"
        Me.pgObjective.UseVisualStyleBackColor = True
        '
        'pgEcoObjectives
        '
        Me.pgEcoObjectives.Location = New System.Drawing.Point(4, 22)
        Me.pgEcoObjectives.Name = "pgEcoObjectives"
        Me.pgEcoObjectives.Padding = New System.Windows.Forms.Padding(3)
        Me.pgEcoObjectives.Size = New System.Drawing.Size(521, 452)
        Me.pgEcoObjectives.TabIndex = 1
        Me.pgEcoObjectives.Text = "Eco Objectives"
        Me.pgEcoObjectives.UseVisualStyleBackColor = True
        '
        'pgCV
        '
        Me.pgCV.Controls.Add(Me.Label4)
        Me.pgCV.Controls.Add(Me.panelCV)
        Me.pgCV.Location = New System.Drawing.Point(4, 22)
        Me.pgCV.Name = "pgCV"
        Me.pgCV.Size = New System.Drawing.Size(521, 452)
        Me.pgCV.TabIndex = 2
        Me.pgCV.Text = "C.V. Fishing Rate"
        Me.pgCV.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label4.Location = New System.Drawing.Point(3, 3)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(515, 21)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "C.V in annual direct estimate of fishing rate"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'panelCV
        '
        Me.panelCV.AutoSizeMinHeight = 10
        Me.panelCV.AutoSizeMinWidth = 10
        Me.panelCV.AutoStretchColumnsToFitWidth = False
        Me.panelCV.AutoStretchRowsToFitHeight = False
        Me.panelCV.BackColor = System.Drawing.Color.White
        Me.panelCV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panelCV.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.panelCV.CustomSort = False
        Me.panelCV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panelCV.FixedColumnWidths = True
        Me.panelCV.FocusStyle = SourceGrid2.FocusStyle.None
        Me.panelCV.GridToolTipActive = True
        Me.panelCV.Location = New System.Drawing.Point(0, 0)
        Me.panelCV.Name = "panelCV"
        Me.panelCV.Size = New System.Drawing.Size(521, 452)
        Me.panelCV.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.panelCV.TabIndex = 0
        '
        'pgFleetWeight
        '
        Me.pgFleetWeight.Controls.Add(Me.Label3)
        Me.pgFleetWeight.Controls.Add(Me.PanelFleetWeight)
        Me.pgFleetWeight.Location = New System.Drawing.Point(4, 22)
        Me.pgFleetWeight.Name = "pgFleetWeight"
        Me.pgFleetWeight.Size = New System.Drawing.Size(521, 452)
        Me.pgFleetWeight.TabIndex = 3
        Me.pgFleetWeight.Text = "Fleet Weight"
        Me.pgFleetWeight.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label3.Location = New System.Drawing.Point(-1, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(522, 21)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Importance weight of fishing on group"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelFleetWeight
        '
        Me.PanelFleetWeight.AutoSizeMinHeight = 10
        Me.PanelFleetWeight.AutoSizeMinWidth = 10
        Me.PanelFleetWeight.AutoStretchColumnsToFitWidth = False
        Me.PanelFleetWeight.AutoStretchRowsToFitHeight = False
        Me.PanelFleetWeight.BackColor = System.Drawing.Color.White
        Me.PanelFleetWeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelFleetWeight.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.PanelFleetWeight.CustomSort = False
        Me.PanelFleetWeight.FixedColumnWidths = True
        Me.PanelFleetWeight.FocusStyle = SourceGrid2.FocusStyle.None
        Me.PanelFleetWeight.GridToolTipActive = True
        Me.PanelFleetWeight.Location = New System.Drawing.Point(0, 24)
        Me.PanelFleetWeight.Name = "PanelFleetWeight"
        Me.PanelFleetWeight.Size = New System.Drawing.Size(521, 428)
        Me.PanelFleetWeight.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.PanelFleetWeight.TabIndex = 0
        '
        'pgCatchabiltiy
        '
        Me.pgCatchabiltiy.Location = New System.Drawing.Point(4, 22)
        Me.pgCatchabiltiy.Name = "pgCatchabiltiy"
        Me.pgCatchabiltiy.Size = New System.Drawing.Size(521, 452)
        Me.pgCatchabiltiy.TabIndex = 4
        Me.pgCatchabiltiy.Text = "Catchability Increase"
        Me.pgCatchabiltiy.UseVisualStyleBackColor = True
        '
        'pgRiskBounds
        '
        Me.pgRiskBounds.Location = New System.Drawing.Point(4, 22)
        Me.pgRiskBounds.Name = "pgRiskBounds"
        Me.pgRiskBounds.Size = New System.Drawing.Size(521, 452)
        Me.pgRiskBounds.TabIndex = 5
        Me.pgRiskBounds.Text = "Risk Bounds"
        Me.pgRiskBounds.UseVisualStyleBackColor = True
        '
        'tbOutput
        '
        Me.tbOutput.Controls.Add(Me.pgGraphs)
        Me.tbOutput.Controls.Add(Me.pgRisk)
        Me.tbOutput.Controls.Add(Me.pgPerformance)
        Me.tbOutput.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbOutput.Location = New System.Drawing.Point(0, 0)
        Me.tbOutput.MinimumSize = New System.Drawing.Size(10, 0)
        Me.tbOutput.Name = "tbOutput"
        Me.tbOutput.SelectedIndex = 0
        Me.tbOutput.Size = New System.Drawing.Size(492, 478)
        Me.tbOutput.TabIndex = 6
        '
        'pgGraphs
        '
        Me.pgGraphs.Controls.Add(Me.zdGraph)
        Me.pgGraphs.Location = New System.Drawing.Point(4, 22)
        Me.pgGraphs.Name = "pgGraphs"
        Me.pgGraphs.Padding = New System.Windows.Forms.Padding(3)
        Me.pgGraphs.Size = New System.Drawing.Size(484, 452)
        Me.pgGraphs.TabIndex = 0
        Me.pgGraphs.Text = "Graphs"
        Me.pgGraphs.UseVisualStyleBackColor = True
        '
        'zdGraph
        '
        Me.zdGraph.Dock = System.Windows.Forms.DockStyle.Fill
        Me.zdGraph.Location = New System.Drawing.Point(3, 3)
        Me.zdGraph.Name = "zdGraph"
        Me.zdGraph.ScrollGrace = 0
        Me.zdGraph.ScrollMaxX = 0
        Me.zdGraph.ScrollMaxY = 0
        Me.zdGraph.ScrollMaxY2 = 0
        Me.zdGraph.ScrollMinX = 0
        Me.zdGraph.ScrollMinY = 0
        Me.zdGraph.ScrollMinY2 = 0
        Me.zdGraph.Size = New System.Drawing.Size(478, 446)
        Me.zdGraph.TabIndex = 0
        '
        'pgRisk
        '
        Me.pgRisk.Location = New System.Drawing.Point(4, 22)
        Me.pgRisk.Name = "pgRisk"
        Me.pgRisk.Padding = New System.Windows.Forms.Padding(3)
        Me.pgRisk.Size = New System.Drawing.Size(484, 452)
        Me.pgRisk.TabIndex = 1
        Me.pgRisk.Text = "Risk"
        Me.pgRisk.UseVisualStyleBackColor = True
        '
        'pgPerformance
        '
        Me.pgPerformance.Location = New System.Drawing.Point(4, 22)
        Me.pgPerformance.Name = "pgPerformance"
        Me.pgPerformance.Size = New System.Drawing.Size(484, 452)
        Me.pgPerformance.TabIndex = 2
        Me.pgPerformance.Text = "Performance"
        Me.pgPerformance.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label2.Location = New System.Drawing.Point(4, 97)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(1021, 21)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Inputs and Outputs"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1028, 595)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.spInputOutput)
        Me.Controls.Add(Me.lbRun)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txNTrials)
        Me.Controls.Add(Me.prgProgress)
        Me.Controls.Add(Me.btRun)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "frmMSE"
        Me.spInputOutput.Panel1.ResumeLayout(False)
        Me.spInputOutput.Panel2.ResumeLayout(False)
        Me.spInputOutput.ResumeLayout(False)
        Me.tbObjectives.ResumeLayout(False)
        Me.pgCV.ResumeLayout(False)
        Me.pgFleetWeight.ResumeLayout(False)
        Me.tbOutput.ResumeLayout(False)
        Me.pgGraphs.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btRun As System.Windows.Forms.Button
    Friend WithEvents prgProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents txNTrials As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbRun As System.Windows.Forms.Label
    Friend WithEvents spInputOutput As System.Windows.Forms.SplitContainer
    Friend WithEvents tbObjectives As System.Windows.Forms.TabControl
    Friend WithEvents pgObjective As System.Windows.Forms.TabPage
    Friend WithEvents pgEcoObjectives As System.Windows.Forms.TabPage
    Friend WithEvents tbOutput As System.Windows.Forms.TabControl
    Friend WithEvents pgGraphs As System.Windows.Forms.TabPage
    Friend WithEvents zdGraph As ZedGraph.ZedGraphControl
    Friend WithEvents pgRisk As System.Windows.Forms.TabPage
    Friend WithEvents pgPerformance As System.Windows.Forms.TabPage
    Friend WithEvents pgCV As System.Windows.Forms.TabPage
    Friend WithEvents pgFleetWeight As System.Windows.Forms.TabPage
    Friend WithEvents pgCatchabiltiy As System.Windows.Forms.TabPage
    Friend WithEvents pgRiskBounds As System.Windows.Forms.TabPage
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents panelCV As gridBioCV
    Friend WithEvents PanelFleetWeight As gridFishingWeights
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
End Class
