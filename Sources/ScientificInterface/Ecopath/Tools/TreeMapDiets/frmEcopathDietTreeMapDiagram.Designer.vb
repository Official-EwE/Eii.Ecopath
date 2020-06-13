Partial Class frmEcopathDietTreeMapDiagram
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcopathDietTreeMapDiagram))
        Me.m_ts = New System.Windows.Forms.ToolStrip()
        Me.m_tsmiSettings = New System.Windows.Forms.ToolStripButton()
        Me.m_tss1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsmiSaveToImage = New System.Windows.Forms.ToolStripButton()
        Me.m_scContent = New System.Windows.Forms.SplitContainer()
        Me.m_pbDiagram = New System.Windows.Forms.PictureBox()
        Me.m_pgFlowDiagram = New System.Windows.Forms.PropertyGrid()
        Me.m_ts.SuspendLayout()
        CType(Me.m_scContent, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scContent.Panel1.SuspendLayout()
        Me.m_scContent.Panel2.SuspendLayout()
        Me.m_scContent.SuspendLayout()
        CType(Me.m_pbDiagram, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_ts
        '
        Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiSettings, Me.m_tss1, Me.m_tsmiSaveToImage})
        Me.m_ts.Location = New System.Drawing.Point(0, 0)
        Me.m_ts.Name = "m_ts"
        Me.m_ts.Size = New System.Drawing.Size(800, 25)
        Me.m_ts.TabIndex = 0
        Me.m_ts.Text = "ToolStrip1"
        '
        'm_tsmiSettings
        '
        Me.m_tsmiSettings.CheckOnClick = True
        Me.m_tsmiSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsmiSettings.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsmiSettings.Name = "m_tsmiSettings"
        Me.m_tsmiSettings.Size = New System.Drawing.Size(53, 22)
        Me.m_tsmiSettings.Text = "&Options"
        '
        'm_tss1
        '
        Me.m_tss1.Name = "m_tss1"
        Me.m_tss1.Size = New System.Drawing.Size(6, 25)
        '
        'm_tsmiSaveToImage
        '
        Me.m_tsmiSaveToImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsmiSaveToImage.Image = CType(resources.GetObject("m_tsmiSaveToImage.Image"), System.Drawing.Image)
        Me.m_tsmiSaveToImage.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsmiSaveToImage.Name = "m_tsmiSaveToImage"
        Me.m_tsmiSaveToImage.Size = New System.Drawing.Size(23, 22)
        Me.m_tsmiSaveToImage.Text = "Save to image"
        '
        'm_scContent
        '
        Me.m_scContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_scContent.Location = New System.Drawing.Point(0, 25)
        Me.m_scContent.Margin = New System.Windows.Forms.Padding(0)
        Me.m_scContent.Name = "m_scContent"
        '
        'm_scContent.Panel1
        '
        Me.m_scContent.Panel1.Controls.Add(Me.m_pbDiagram)
        '
        'm_scContent.Panel2
        '
        Me.m_scContent.Panel2.Controls.Add(Me.m_pgFlowDiagram)
        Me.m_scContent.Size = New System.Drawing.Size(800, 425)
        Me.m_scContent.SplitterDistance = 597
        Me.m_scContent.TabIndex = 6
        '
        'm_pbDiagram
        '
        Me.m_pbDiagram.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_pbDiagram.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbDiagram.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_pbDiagram.Location = New System.Drawing.Point(0, 0)
        Me.m_pbDiagram.Name = "m_pbDiagram"
        Me.m_pbDiagram.Size = New System.Drawing.Size(597, 425)
        Me.m_pbDiagram.TabIndex = 4
        Me.m_pbDiagram.TabStop = False
        '
        'm_pgFlowDiagram
        '
        Me.m_pgFlowDiagram.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pgFlowDiagram.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.m_pgFlowDiagram.Location = New System.Drawing.Point(0, 0)
        Me.m_pgFlowDiagram.Name = "m_pgFlowDiagram"
        Me.m_pgFlowDiagram.Size = New System.Drawing.Size(199, 425)
        Me.m_pgFlowDiagram.TabIndex = 1
        '
        'frmEcopathDietTreeMapDiagram
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.m_scContent)
        Me.Controls.Add(Me.m_ts)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmEcopathDietTreeMapDiagram"
        Me.TabText = ""
        Me.Text = "frmEcopathDietTreeMapDiagram"
        Me.m_ts.ResumeLayout(False)
        Me.m_ts.PerformLayout()
        Me.m_scContent.Panel1.ResumeLayout(False)
        Me.m_scContent.Panel2.ResumeLayout(False)
        CType(Me.m_scContent, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scContent.ResumeLayout(False)
        CType(Me.m_pbDiagram, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_ts As ToolStrip
    Private WithEvents m_scContent As SplitContainer
    Private WithEvents m_pbDiagram As PictureBox
    Private WithEvents m_pgFlowDiagram As PropertyGrid
    Private WithEvents m_tsmiSettings As ToolStripButton
    Private WithEvents m_tss1 As ToolStripSeparator
    Private WithEvents m_tsmiSaveToImage As ToolStripButton
End Class
