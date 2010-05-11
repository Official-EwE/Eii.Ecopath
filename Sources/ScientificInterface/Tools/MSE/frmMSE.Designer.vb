Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared.Controls

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
        Me.m_btRun = New System.Windows.Forms.Button
        Me.m_lblNumTrials = New System.Windows.Forms.Label
        Me.m_hdrOutputs = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_btnStop = New System.Windows.Forms.Button
        Me.m_zgc = New ZedGraph.ZedGraphControl
        Me.m_btnShowHide = New System.Windows.Forms.Button
        Me.m_ckSave = New System.Windows.Forms.CheckBox
        Me.m_lblStartYear = New System.Windows.Forms.Label
        Me.m_tlpTop = New System.Windows.Forms.TableLayoutPanel
        Me.m_nudStartYear = New System.Windows.Forms.NumericUpDown
        Me.m_nudNumTrials = New System.Windows.Forms.NumericUpDown
        Me.m_tlpTop.SuspendLayout()
        CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_btRun
        '
        Me.m_btRun.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btRun.Location = New System.Drawing.Point(0, 0)
        Me.m_btRun.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.m_btRun.Name = "m_btRun"
        Me.m_btRun.Size = New System.Drawing.Size(75, 23)
        Me.m_btRun.TabIndex = 0
        Me.m_btRun.Text = "&Run"
        Me.m_btRun.UseVisualStyleBackColor = True
        '
        'm_lblNumTrials
        '
        Me.m_lblNumTrials.AutoSize = True
        Me.m_lblNumTrials.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblNumTrials.Location = New System.Drawing.Point(306, 0)
        Me.m_lblNumTrials.Name = "m_lblNumTrials"
        Me.m_lblNumTrials.Size = New System.Drawing.Size(83, 23)
        Me.m_lblNumTrials.TabIndex = 4
        Me.m_lblNumTrials.Text = "&Number of trials:"
        Me.m_lblNumTrials.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_hdrOutputs
        '
        Me.m_hdrOutputs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrOutputs.Location = New System.Drawing.Point(5, 31)
        Me.m_hdrOutputs.Name = "m_hdrOutputs"
        Me.m_hdrOutputs.Size = New System.Drawing.Size(635, 18)
        Me.m_hdrOutputs.TabIndex = 1
        Me.m_hdrOutputs.Text = "Outputs"
        Me.m_hdrOutputs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_btnStop
        '
        Me.m_btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnStop.Location = New System.Drawing.Point(81, 0)
        Me.m_btnStop.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.m_btnStop.Name = "m_btnStop"
        Me.m_btnStop.Size = New System.Drawing.Size(75, 23)
        Me.m_btnStop.TabIndex = 1
        Me.m_btnStop.Text = "Stop"
        Me.m_btnStop.UseVisualStyleBackColor = True
        '
        'm_zgc
        '
        Me.m_zgc.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_zgc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_zgc.Location = New System.Drawing.Point(5, 49)
        Me.m_zgc.Margin = New System.Windows.Forms.Padding(0)
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0
        Me.m_zgc.ScrollMaxX = 0
        Me.m_zgc.ScrollMaxY = 0
        Me.m_zgc.ScrollMaxY2 = 0
        Me.m_zgc.ScrollMinX = 0
        Me.m_zgc.ScrollMinY = 0
        Me.m_zgc.ScrollMinY2 = 0
        Me.m_zgc.Size = New System.Drawing.Size(635, 412)
        Me.m_zgc.TabIndex = 2
        '
        'm_btnShowHide
        '
        Me.m_btnShowHide.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnShowHide.Location = New System.Drawing.Point(162, 0)
        Me.m_btnShowHide.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.m_btnShowHide.Name = "m_btnShowHide"
        Me.m_btnShowHide.Size = New System.Drawing.Size(110, 23)
        Me.m_btnShowHide.TabIndex = 2
        Me.m_btnShowHide.Text = "&Show/hide items..."
        Me.m_btnShowHide.UseVisualStyleBackColor = True
        '
        'm_ckSave
        '
        Me.m_ckSave.AutoSize = True
        Me.m_ckSave.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_ckSave.Enabled = False
        Me.m_ckSave.Location = New System.Drawing.Point(247, 3)
        Me.m_ckSave.Name = "m_ckSave"
        Me.m_ckSave.Size = New System.Drawing.Size(84, 17)
        Me.m_ckSave.TabIndex = 3
        Me.m_ckSave.Text = "Save &output"
        Me.m_ckSave.UseVisualStyleBackColor = True
        '
        'm_lblStartYear
        '
        Me.m_lblStartYear.AutoSize = True
        Me.m_lblStartYear.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblStartYear.Location = New System.Drawing.Point(470, 0)
        Me.m_lblStartYear.Name = "m_lblStartYear"
        Me.m_lblStartYear.Size = New System.Drawing.Size(55, 23)
        Me.m_lblStartYear.TabIndex = 6
        Me.m_lblStartYear.Text = "Start &year:"
        Me.m_lblStartYear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tlpTop
        '
        Me.m_tlpTop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tlpTop.ColumnCount = 11
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpTop.Controls.Add(Me.m_lblStartYear, 9, 0)
        Me.m_tlpTop.Controls.Add(Me.m_btnStop, 1, 0)
        Me.m_tlpTop.Controls.Add(Me.m_btnShowHide, 2, 0)
        Me.m_tlpTop.Controls.Add(Me.m_btRun, 0, 0)
        Me.m_tlpTop.Controls.Add(Me.m_nudStartYear, 10, 0)
        Me.m_tlpTop.Controls.Add(Me.m_lblNumTrials, 6, 0)
        Me.m_tlpTop.Controls.Add(Me.m_ckSave, 4, 0)
        Me.m_tlpTop.Controls.Add(Me.m_nudNumTrials, 7, 0)
        Me.m_tlpTop.Location = New System.Drawing.Point(5, 5)
        Me.m_tlpTop.Name = "m_tlpTop"
        Me.m_tlpTop.RowCount = 1
        Me.m_tlpTop.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpTop.Size = New System.Drawing.Size(635, 23)
        Me.m_tlpTop.TabIndex = 0
        '
        'm_nudStartYear
        '
        Me.m_nudStartYear.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_nudStartYear.Location = New System.Drawing.Point(531, 0)
        Me.m_nudStartYear.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.m_nudStartYear.Name = "m_nudStartYear"
        Me.m_nudStartYear.Size = New System.Drawing.Size(103, 20)
        Me.m_nudStartYear.TabIndex = 7
        '
        'm_nudNumTrials
        '
        Me.m_nudNumTrials.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_nudNumTrials.Location = New System.Drawing.Point(395, 0)
        Me.m_nudNumTrials.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.m_nudNumTrials.Name = "m_nudNumTrials"
        Me.m_nudNumTrials.Size = New System.Drawing.Size(100, 20)
        Me.m_nudNumTrials.TabIndex = 5
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(649, 470)
        Me.Controls.Add(Me.m_tlpTop)
        Me.Controls.Add(Me.m_zgc)
        Me.Controls.Add(Me.m_hdrOutputs)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Name = "frmMSE"
        Me.Text = "Run MSE"
        Me.m_tlpTop.ResumeLayout(False)
        Me.m_tlpTop.PerformLayout()
        CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btRun As System.Windows.Forms.Button
    Private WithEvents m_lblNumTrials As System.Windows.Forms.Label
    Private WithEvents m_hdrOutputs As cEwEHeaderLabel
    Private WithEvents m_btnStop As System.Windows.Forms.Button
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_btnShowHide As System.Windows.Forms.Button
    Private WithEvents m_lblStartYear As System.Windows.Forms.Label
    Private WithEvents m_tlpTop As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_ckSave As System.Windows.Forms.CheckBox
    Private WithEvents m_nudNumTrials As System.Windows.Forms.NumericUpDown
    Private WithEvents m_nudStartYear As System.Windows.Forms.NumericUpDown
End Class
