<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucDriverResponseView
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.m_lblXMin = New System.Windows.Forms.Label()
        Me.m_tbxXMin = New System.Windows.Forms.TextBox()
        Me.m_lblXMax = New System.Windows.Forms.Label()
        Me.m_tbxXMax = New System.Windows.Forms.TextBox()
        Me.m_lblMean = New System.Windows.Forms.Label()
        Me.m_graph = New ZedGraph.ZedGraphControl()
        Me.m_tbxMean = New System.Windows.Forms.TextBox()
        Me.m_btnDefaultMinMax = New System.Windows.Forms.Button()
        Me.m_btChangeShape = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'm_lblXMin
        '
        Me.m_lblXMin.AutoSize = True
        Me.m_lblXMin.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblXMin.Location = New System.Drawing.Point(0, 8)
        Me.m_lblXMin.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblXMin.Name = "m_lblXMin"
        Me.m_lblXMin.Size = New System.Drawing.Size(42, 13)
        Me.m_lblXMin.TabIndex = 19
        Me.m_lblXMin.Text = "X m&in:  "
        Me.m_lblXMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'm_tbxXMin
        '
        Me.m_tbxXMin.Location = New System.Drawing.Point(48, 5)
        Me.m_tbxXMin.Name = "m_tbxXMin"
        Me.m_tbxXMin.Size = New System.Drawing.Size(50, 20)
        Me.m_tbxXMin.TabIndex = 20
        '
        'm_lblXMax
        '
        Me.m_lblXMax.AutoSize = True
        Me.m_lblXMax.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblXMax.Location = New System.Drawing.Point(108, 8)
        Me.m_lblXMax.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblXMax.Name = "m_lblXMax"
        Me.m_lblXMax.Size = New System.Drawing.Size(45, 13)
        Me.m_lblXMax.TabIndex = 21
        Me.m_lblXMax.Text = "X m&ax:  "
        Me.m_lblXMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'm_tbxXMax
        '
        Me.m_tbxXMax.Location = New System.Drawing.Point(159, 5)
        Me.m_tbxXMax.Name = "m_tbxXMax"
        Me.m_tbxXMax.Size = New System.Drawing.Size(50, 20)
        Me.m_tbxXMax.TabIndex = 22
        '
        'm_lblMean
        '
        Me.m_lblMean.AutoSize = True
        Me.m_lblMean.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblMean.Location = New System.Drawing.Point(0, 37)
        Me.m_lblMean.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblMean.Name = "m_lblMean"
        Me.m_lblMean.Size = New System.Drawing.Size(37, 13)
        Me.m_lblMean.TabIndex = 24
        Me.m_lblMean.Text = "Mean:"
        Me.m_lblMean.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_graph
        '
        Me.m_graph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_graph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_graph.Location = New System.Drawing.Point(0, 61)
        Me.m_graph.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0R
        Me.m_graph.ScrollMaxX = 0R
        Me.m_graph.ScrollMaxY = 0R
        Me.m_graph.ScrollMaxY2 = 0R
        Me.m_graph.ScrollMinX = 0R
        Me.m_graph.ScrollMinY = 0R
        Me.m_graph.ScrollMinY2 = 0R
        Me.m_graph.Size = New System.Drawing.Size(367, 274)
        Me.m_graph.TabIndex = 27
        '
        'm_tbxMean
        '
        Me.m_tbxMean.Location = New System.Drawing.Point(48, 34)
        Me.m_tbxMean.Name = "m_tbxMean"
        Me.m_tbxMean.Size = New System.Drawing.Size(50, 20)
        Me.m_tbxMean.TabIndex = 25
        '
        'm_btnDefaultMinMax
        '
        Me.m_btnDefaultMinMax.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnDefaultMinMax.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_btnDefaultMinMax.Location = New System.Drawing.Point(225, 3)
        Me.m_btnDefaultMinMax.Name = "m_btnDefaultMinMax"
        Me.m_btnDefaultMinMax.Size = New System.Drawing.Size(142, 23)
        Me.m_btnDefaultMinMax.TabIndex = 23
        Me.m_btnDefaultMinMax.Text = "&Default X axis"
        Me.m_btnDefaultMinMax.UseVisualStyleBackColor = True
        '
        'm_btChangeShape
        '
        Me.m_btChangeShape.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btChangeShape.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_btChangeShape.Location = New System.Drawing.Point(225, 32)
        Me.m_btChangeShape.Name = "m_btChangeShape"
        Me.m_btChangeShape.Size = New System.Drawing.Size(142, 23)
        Me.m_btChangeShape.TabIndex = 26
        Me.m_btChangeShape.Text = "&Edit response function..."
        Me.m_btChangeShape.UseVisualStyleBackColor = True
        '
        'ucDriverResponseView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_lblXMin)
        Me.Controls.Add(Me.m_tbxXMin)
        Me.Controls.Add(Me.m_lblXMax)
        Me.Controls.Add(Me.m_tbxXMax)
        Me.Controls.Add(Me.m_lblMean)
        Me.Controls.Add(Me.m_graph)
        Me.Controls.Add(Me.m_tbxMean)
        Me.Controls.Add(Me.m_btnDefaultMinMax)
        Me.Controls.Add(Me.m_btChangeShape)
        Me.Name = "ucDriverResponseView"
        Me.Size = New System.Drawing.Size(367, 335)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_lblXMin As Label
    Private WithEvents m_tbxXMin As TextBox
    Private WithEvents m_lblXMax As Label
    Private WithEvents m_tbxXMax As TextBox
    Private WithEvents m_lblMean As Label
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_tbxMean As TextBox
    Private WithEvents m_btnDefaultMinMax As Button
    Private WithEvents m_btChangeShape As Button
End Class
