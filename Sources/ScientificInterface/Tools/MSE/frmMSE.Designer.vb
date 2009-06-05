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
        Me.lbParams = New System.Windows.Forms.Label
        Me.tbOutput = New System.Windows.Forms.TabControl
        Me.pgGraphs = New System.Windows.Forms.TabPage
        Me.pgRisk = New System.Windows.Forms.TabPage
        Me.pgPerformance = New System.Windows.Forms.TabPage
        Me.zdGraph = New ZedGraph.ZedGraphControl
        Me.tbOutput.SuspendLayout()
        Me.pgGraphs.SuspendLayout()
        Me.SuspendLayout()
        '
        'btRun
        '
        Me.btRun.Location = New System.Drawing.Point(12, 88)
        Me.btRun.Name = "btRun"
        Me.btRun.Size = New System.Drawing.Size(93, 20)
        Me.btRun.TabIndex = 0
        Me.btRun.Text = "Run"
        Me.btRun.UseVisualStyleBackColor = True
        '
        'prgProgress
        '
        Me.prgProgress.Location = New System.Drawing.Point(12, 125)
        Me.prgProgress.Name = "prgProgress"
        Me.prgProgress.Size = New System.Drawing.Size(195, 18)
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
        'lbParams
        '
        Me.lbParams.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lbParams.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbParams.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lbParams.Location = New System.Drawing.Point(1, 0)
        Me.lbParams.Name = "lbParams"
        Me.lbParams.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lbParams.Size = New System.Drawing.Size(243, 21)
        Me.lbParams.TabIndex = 4
        Me.lbParams.Text = "Parameters"
        Me.lbParams.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbOutput
        '
        Me.tbOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbOutput.Controls.Add(Me.pgGraphs)
        Me.tbOutput.Controls.Add(Me.pgRisk)
        Me.tbOutput.Controls.Add(Me.pgPerformance)
        Me.tbOutput.Location = New System.Drawing.Point(259, 0)
        Me.tbOutput.Name = "tbOutput"
        Me.tbOutput.SelectedIndex = 0
        Me.tbOutput.Size = New System.Drawing.Size(769, 597)
        Me.tbOutput.TabIndex = 5
        '
        'pgGraphs
        '
        Me.pgGraphs.Controls.Add(Me.zdGraph)
        Me.pgGraphs.Location = New System.Drawing.Point(4, 22)
        Me.pgGraphs.Name = "pgGraphs"
        Me.pgGraphs.Padding = New System.Windows.Forms.Padding(3)
        Me.pgGraphs.Size = New System.Drawing.Size(761, 571)
        Me.pgGraphs.TabIndex = 0
        Me.pgGraphs.Text = "Graphs"
        Me.pgGraphs.UseVisualStyleBackColor = True
        '
        'pgRisk
        '
        Me.pgRisk.Location = New System.Drawing.Point(4, 22)
        Me.pgRisk.Name = "pgRisk"
        Me.pgRisk.Padding = New System.Windows.Forms.Padding(3)
        Me.pgRisk.Size = New System.Drawing.Size(761, 571)
        Me.pgRisk.TabIndex = 1
        Me.pgRisk.Text = "Risk"
        Me.pgRisk.UseVisualStyleBackColor = True
        '
        'pgPerformance
        '
        Me.pgPerformance.Location = New System.Drawing.Point(4, 22)
        Me.pgPerformance.Name = "pgPerformance"
        Me.pgPerformance.Size = New System.Drawing.Size(761, 571)
        Me.pgPerformance.TabIndex = 2
        Me.pgPerformance.Text = "Performance"
        Me.pgPerformance.UseVisualStyleBackColor = True
        '
        'zdGraph
        '
        Me.zdGraph.Location = New System.Drawing.Point(-4, 0)
        Me.zdGraph.Name = "zdGraph"
        Me.zdGraph.ScrollGrace = 0
        Me.zdGraph.ScrollMaxX = 0
        Me.zdGraph.ScrollMaxY = 0
        Me.zdGraph.ScrollMaxY2 = 0
        Me.zdGraph.ScrollMinX = 0
        Me.zdGraph.ScrollMinY = 0
        Me.zdGraph.ScrollMinY2 = 0
        Me.zdGraph.Size = New System.Drawing.Size(764, 570)
        Me.zdGraph.TabIndex = 0
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1028, 595)
        Me.Controls.Add(Me.tbOutput)
        Me.Controls.Add(Me.lbParams)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txNTrials)
        Me.Controls.Add(Me.prgProgress)
        Me.Controls.Add(Me.btRun)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "frmMSE"
        Me.tbOutput.ResumeLayout(False)
        Me.pgGraphs.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btRun As System.Windows.Forms.Button
    Friend WithEvents prgProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents txNTrials As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbParams As System.Windows.Forms.Label
    Friend WithEvents tbOutput As System.Windows.Forms.TabControl
    Friend WithEvents pgGraphs As System.Windows.Forms.TabPage
    Friend WithEvents pgRisk As System.Windows.Forms.TabPage
    Friend WithEvents pgPerformance As System.Windows.Forms.TabPage
    Friend WithEvents zdGraph As ZedGraph.ZedGraphControl
End Class
