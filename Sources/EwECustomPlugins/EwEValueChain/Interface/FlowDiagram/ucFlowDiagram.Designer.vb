Imports ScientificInterfaceShared.Controls

Partial Class ucFlowDiagram
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucFlowDiagram))
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tslData = New System.Windows.Forms.ToolStripLabel()
        Me.m_tscbmValue = New System.Windows.Forms.ToolStripComboBox()
        Me.m_tsbnOptions = New System.Windows.Forms.ToolStripButton()
        Me.m_scFD = New System.Windows.Forms.SplitContainer()
        Me.m_pgFD = New System.Windows.Forms.PropertyGrid()
        Me.m_pbFlowDiagram = New ucSmoothPanel()
        Me.m_tsMain.SuspendLayout()
        CType(Me.m_scFD, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scFD.Panel1.SuspendLayout()
        Me.m_scFD.Panel2.SuspendLayout()
        Me.m_scFD.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslData, Me.m_tscbmValue, Me.m_tsbnOptions})
        Me.m_tsMain.Location = New System.Drawing.Point(0, 0)
        Me.m_tsMain.Name = "m_tsMain"
        Me.m_tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.m_tsMain.Size = New System.Drawing.Size(559, 25)
        Me.m_tsMain.TabIndex = 0
        '
        'm_tslData
        '
        Me.m_tslData.Name = "m_tslData"
        Me.m_tslData.Size = New System.Drawing.Size(34, 22)
        Me.m_tslData.Text = "&Data:"
        '
        'm_tscbmValue
        '
        Me.m_tscbmValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscbmValue.Name = "m_tscbmValue"
        Me.m_tscbmValue.Size = New System.Drawing.Size(121, 25)
        '
        'm_tsbnOptions
        '
        Me.m_tsbnOptions.CheckOnClick = True
        Me.m_tsbnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbnOptions.Image = CType(resources.GetObject("m_tsbnOptions.Image"), System.Drawing.Image)
        Me.m_tsbnOptions.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnOptions.Name = "m_tsbnOptions"
        Me.m_tsbnOptions.Size = New System.Drawing.Size(53, 22)
        Me.m_tsbnOptions.Text = "Options"
        '
        'm_scFD
        '
        Me.m_scFD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_scFD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_scFD.IsSplitterFixed = True
        Me.m_scFD.Location = New System.Drawing.Point(0, 25)
        Me.m_scFD.Name = "m_scFD"
        '
        'm_scFD.Panel1
        '
        Me.m_scFD.Panel1.Controls.Add(Me.m_pbFlowDiagram)
        '
        'm_scFD.Panel2
        '
        Me.m_scFD.Panel2.Controls.Add(Me.m_pgFD)
        Me.m_scFD.Size = New System.Drawing.Size(559, 336)
        Me.m_scFD.SplitterDistance = 414
        Me.m_scFD.TabIndex = 2
        '
        'm_pgFD
        '
        Me.m_pgFD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pgFD.Location = New System.Drawing.Point(0, 0)
        Me.m_pgFD.Name = "m_pgFD"
        Me.m_pgFD.Size = New System.Drawing.Size(137, 332)
        Me.m_pgFD.TabIndex = 0
        '
        'm_pbFlowDiagram
        '
        Me.m_pbFlowDiagram.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbFlowDiagram.Location = New System.Drawing.Point(0, 0)
        Me.m_pbFlowDiagram.Name = "m_pbFlowDiagram"
        Me.m_pbFlowDiagram.Size = New System.Drawing.Size(410, 332)
        Me.m_pbFlowDiagram.TabIndex = 0
        '
        'ucFlowDiagram
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_scFD)
        Me.Controls.Add(Me.m_tsMain)
        Me.Name = "ucFlowDiagram"
        Me.Size = New System.Drawing.Size(559, 361)
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        Me.m_scFD.Panel1.ResumeLayout(False)
        Me.m_scFD.Panel2.ResumeLayout(False)
        CType(Me.m_scFD, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scFD.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_tsMain As cEwEToolstrip
    Private WithEvents m_tslData As System.Windows.Forms.ToolStripLabel
    Private WithEvents m_tscbmValue As System.Windows.Forms.ToolStripComboBox
    Private WithEvents m_scFD As System.Windows.Forms.SplitContainer
    Private WithEvents m_pgFD As System.Windows.Forms.PropertyGrid
    Private WithEvents m_tsbnOptions As System.Windows.Forms.ToolStripButton
    Private WithEvents m_pbFlowDiagram As ucSmoothPanel

End Class
