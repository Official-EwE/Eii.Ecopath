<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSuitabilityPlot
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
        Me.components = New System.ComponentModel.Container
        Me.m_graph = New ZedGraph.ZedGraphControl
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.m_tslblPlotType = New System.Windows.Forms.ToolStripLabel
        Me.m_tscmbPlotType = New System.Windows.Forms.ToolStripComboBox
        Me.m_tslblPredator = New System.Windows.Forms.ToolStripLabel
        Me.m_tscmbPredator = New System.Windows.Forms.ToolStripComboBox
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_graph
        '
        Me.m_graph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_graph.Location = New System.Drawing.Point(0, 28)
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0
        Me.m_graph.ScrollMaxX = 0
        Me.m_graph.ScrollMaxY = 0
        Me.m_graph.ScrollMaxY2 = 0
        Me.m_graph.ScrollMinX = 0
        Me.m_graph.ScrollMinY = 0
        Me.m_graph.ScrollMinY2 = 0
        Me.m_graph.Size = New System.Drawing.Size(697, 409)
        Me.m_graph.TabIndex = 4
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslblPlotType, Me.m_tscmbPlotType, Me.m_tslblPredator, Me.m_tscmbPredator})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(700, 25)
        Me.ToolStrip1.TabIndex = 5
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'm_tslblPlotType
        '
        Me.m_tslblPlotType.Name = "m_tslblPlotType"
        Me.m_tslblPlotType.Size = New System.Drawing.Size(53, 22)
        Me.m_tslblPlotType.Text = "PlotType:"
        '
        'm_tscmbPlotType
        '
        Me.m_tscmbPlotType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmbPlotType.Items.AddRange(New Object() {"Electivity", "Functional response", "Suitability"})
        Me.m_tscmbPlotType.Name = "m_tscmbPlotType"
        Me.m_tscmbPlotType.Size = New System.Drawing.Size(121, 25)
        '
        'm_tslblPredator
        '
        Me.m_tslblPredator.Name = "m_tslblPredator"
        Me.m_tslblPredator.Size = New System.Drawing.Size(53, 22)
        Me.m_tslblPredator.Text = "Predator:"
        '
        'm_tscmbPredator
        '
        Me.m_tscmbPredator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmbPredator.Name = "m_tscmbPredator"
        Me.m_tscmbPredator.Size = New System.Drawing.Size(121, 25)
        '
        'ucSuitabilityPlot
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.m_graph)
        Me.Name = "ucSuitabilityPlot"
        Me.Size = New System.Drawing.Size(700, 440)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_graph As ZedGraph.ZedGraphControl
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents m_tslblPlotType As System.Windows.Forms.ToolStripLabel
    Friend WithEvents m_tscmbPlotType As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents m_tslblPredator As System.Windows.Forms.ToolStripLabel
    Friend WithEvents m_tscmbPredator As System.Windows.Forms.ToolStripComboBox

End Class
