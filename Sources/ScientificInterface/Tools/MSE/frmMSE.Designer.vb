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
        Me.txNTrials = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.btStop = New System.Windows.Forms.Button
        Me.zdGraph = New ZedGraph.ZedGraphControl
        Me.btShowHide = New System.Windows.Forms.Button
        Me.ckSave = New System.Windows.Forms.CheckBox
        Me.txStartYear = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btRun
        '
        Me.btRun.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btRun.Location = New System.Drawing.Point(5, 9)
        Me.btRun.Margin = New System.Windows.Forms.Padding(0)
        Me.btRun.Name = "btRun"
        Me.btRun.Size = New System.Drawing.Size(146, 22)
        Me.btRun.TabIndex = 0
        Me.btRun.Text = "&Run"
        Me.btRun.UseVisualStyleBackColor = True
        '
        'txNTrials
        '
        Me.txNTrials.Location = New System.Drawing.Point(660, 10)
        Me.txNTrials.Name = "txNTrials"
        Me.txNTrials.Size = New System.Drawing.Size(63, 20)
        Me.txNTrials.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(576, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Number of trials:"
        '
        'Label3
        '
        Me.Label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label3.Location = New System.Drawing.Point(5, 34)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(892, 22)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Outputs"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btStop
        '
        Me.btStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btStop.Location = New System.Drawing.Point(154, 9)
        Me.btStop.Name = "btStop"
        Me.btStop.Size = New System.Drawing.Size(146, 22)
        Me.btStop.TabIndex = 26
        Me.btStop.Text = "Stop"
        Me.btStop.UseVisualStyleBackColor = True
        '
        'zdGraph
        '
        Me.zdGraph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.zdGraph.Location = New System.Drawing.Point(5, 56)
        Me.zdGraph.Margin = New System.Windows.Forms.Padding(0)
        Me.zdGraph.Name = "zdGraph"
        Me.zdGraph.ScrollGrace = 0
        Me.zdGraph.ScrollMaxX = 0
        Me.zdGraph.ScrollMaxY = 0
        Me.zdGraph.ScrollMaxY2 = 0
        Me.zdGraph.ScrollMinX = 0
        Me.zdGraph.ScrollMinY = 0
        Me.zdGraph.ScrollMinY2 = 0
        Me.zdGraph.Size = New System.Drawing.Size(892, 711)
        Me.zdGraph.TabIndex = 27
        '
        'btShowHide
        '
        Me.btShowHide.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btShowHide.Location = New System.Drawing.Point(316, 10)
        Me.btShowHide.Name = "btShowHide"
        Me.btShowHide.Size = New System.Drawing.Size(145, 21)
        Me.btShowHide.TabIndex = 32
        Me.btShowHide.Text = "Show/hide items..."
        Me.btShowHide.UseVisualStyleBackColor = True
        '
        'ckSave
        '
        Me.ckSave.AutoSize = True
        Me.ckSave.Enabled = False
        Me.ckSave.Location = New System.Drawing.Point(467, 13)
        Me.ckSave.Name = "ckSave"
        Me.ckSave.Size = New System.Drawing.Size(84, 17)
        Me.ckSave.TabIndex = 36
        Me.ckSave.Text = "Save output"
        Me.ckSave.UseVisualStyleBackColor = True
        '
        'txStartYear
        '
        Me.txStartYear.Location = New System.Drawing.Point(813, 11)
        Me.txStartYear.Name = "txStartYear"
        Me.txStartYear.Size = New System.Drawing.Size(66, 20)
        Me.txStartYear.TabIndex = 37
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(752, 14)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 13)
        Me.Label2.TabIndex = 38
        Me.Label2.Text = "Start year:"
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(906, 776)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txStartYear)
        Me.Controls.Add(Me.btShowHide)
        Me.Controls.Add(Me.ckSave)
        Me.Controls.Add(Me.zdGraph)
        Me.Controls.Add(Me.btStop)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txNTrials)
        Me.Controls.Add(Me.btRun)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "Run MSE"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents btRun As System.Windows.Forms.Button
    Private WithEvents txNTrials As System.Windows.Forms.TextBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btStop As System.Windows.Forms.Button
    Private WithEvents zdGraph As ZedGraph.ZedGraphControl
    Friend WithEvents btShowHide As System.Windows.Forms.Button
    Friend WithEvents ckSave As System.Windows.Forms.CheckBox
    Friend WithEvents txStartYear As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
