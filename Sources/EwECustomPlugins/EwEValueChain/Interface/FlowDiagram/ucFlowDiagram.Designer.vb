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
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tslData = New System.Windows.Forms.ToolStripLabel()
        Me.m_tscbmValue = New System.Windows.Forms.ToolStripComboBox()
        Me.m_pbFlowDiagram = New System.Windows.Forms.PictureBox()
        Me.m_tsMain.SuspendLayout()
        CType(Me.m_pbFlowDiagram, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslData, Me.m_tscbmValue})
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
        'm_pbFlowDiagram
        '
        Me.m_pbFlowDiagram.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbFlowDiagram.Location = New System.Drawing.Point(0, 25)
        Me.m_pbFlowDiagram.Name = "m_pbFlowDiagram"
        Me.m_pbFlowDiagram.Size = New System.Drawing.Size(559, 336)
        Me.m_pbFlowDiagram.TabIndex = 1
        Me.m_pbFlowDiagram.TabStop = False
        '
        'ucFlowDiagram
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_pbFlowDiagram)
        Me.Controls.Add(Me.m_tsMain)
        Me.Name = "ucFlowDiagram"
        Me.Size = New System.Drawing.Size(559, 361)
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        CType(Me.m_pbFlowDiagram, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_tsMain As cEwEToolstrip
    Private WithEvents m_pbFlowDiagram As System.Windows.Forms.PictureBox
    Private WithEvents m_tslData As System.Windows.Forms.ToolStripLabel
    Private WithEvents m_tscbmValue As System.Windows.Forms.ToolStripComboBox

End Class
