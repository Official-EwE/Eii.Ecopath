Imports System.Windows.Forms

<CLSCompliant(False)> _
Public Class ucPlotOfMTIOptions
    Inherits usercontrol

    Private WithEvents m_hdrOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_rbCircles As System.Windows.Forms.RadioButton
    Private WithEvents m_rbRectangles As System.Windows.Forms.RadioButton
    Private WithEvents m_cbShowGrid As System.Windows.Forms.CheckBox
    Private WithEvents m_cbSlantingLabels As System.Windows.Forms.CheckBox
    Private m_content As cPlotOfMixedTrophicImpact = Nothing

    Public Sub New(ByVal content As cPlotOfMixedTrophicImpact)
        Me.InitializeComponent()
        Me.m_content = content
    End Sub

    Private Sub InitializeComponent()
        Me.m_hdrOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_rbCircles = New System.Windows.Forms.RadioButton
        Me.m_rbRectangles = New System.Windows.Forms.RadioButton
        Me.m_cbShowGrid = New System.Windows.Forms.CheckBox
        Me.m_cbSlantingLabels = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'm_hdrOptions
        '
        Me.m_hdrOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrOptions.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrOptions.Margin = New System.Windows.Forms.Padding(0)
        Me.m_hdrOptions.Name = "m_hdrOptions"
        Me.m_hdrOptions.Size = New System.Drawing.Size(125, 18)
        Me.m_hdrOptions.TabIndex = 0
        Me.m_hdrOptions.Text = "Plot options"
        Me.m_hdrOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_rbCircles
        '
        Me.m_rbCircles.AutoSize = True
        Me.m_rbCircles.Checked = True
        Me.m_rbCircles.Location = New System.Drawing.Point(6, 27)
        Me.m_rbCircles.Name = "m_rbCircles"
        Me.m_rbCircles.Size = New System.Drawing.Size(56, 17)
        Me.m_rbCircles.TabIndex = 1
        Me.m_rbCircles.TabStop = True
        Me.m_rbCircles.Text = "&Circles"
        Me.m_rbCircles.UseVisualStyleBackColor = True
        '
        'm_rbRectangles
        '
        Me.m_rbRectangles.AutoSize = True
        Me.m_rbRectangles.Location = New System.Drawing.Point(6, 50)
        Me.m_rbRectangles.Name = "m_rbRectangles"
        Me.m_rbRectangles.Size = New System.Drawing.Size(79, 17)
        Me.m_rbRectangles.TabIndex = 1
        Me.m_rbRectangles.TabStop = True
        Me.m_rbRectangles.Text = "&Rectangles"
        Me.m_rbRectangles.UseVisualStyleBackColor = True
        '
        'm_cbShowGrid
        '
        Me.m_cbShowGrid.AutoSize = True
        Me.m_cbShowGrid.Location = New System.Drawing.Point(6, 89)
        Me.m_cbShowGrid.Name = "m_cbShowGrid"
        Me.m_cbShowGrid.Size = New System.Drawing.Size(95, 17)
        Me.m_cbShowGrid.TabIndex = 2
        Me.m_cbShowGrid.Text = "Draw &grid lines"
        Me.m_cbShowGrid.UseVisualStyleBackColor = True
        '
        'm_cbSlantingLabels
        '
        Me.m_cbSlantingLabels.AutoSize = True
        Me.m_cbSlantingLabels.Checked = True
        Me.m_cbSlantingLabels.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_cbSlantingLabels.Location = New System.Drawing.Point(6, 112)
        Me.m_cbSlantingLabels.Name = "m_cbSlantingLabels"
        Me.m_cbSlantingLabels.Size = New System.Drawing.Size(120, 17)
        Me.m_cbSlantingLabels.TabIndex = 2
        Me.m_cbSlantingLabels.Text = "Draw slanting &labels"
        Me.m_cbSlantingLabels.UseVisualStyleBackColor = True
        Me.m_cbSlantingLabels.Visible = False
        '
        'ucPlotOfMTIOptions
        '
        Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Controls.Add(Me.m_cbSlantingLabels)
        Me.Controls.Add(Me.m_cbShowGrid)
        Me.Controls.Add(Me.m_rbRectangles)
        Me.Controls.Add(Me.m_rbCircles)
        Me.Controls.Add(Me.m_hdrOptions)
        Me.Name = "ucPlotOfMTIOptions"
        Me.Size = New System.Drawing.Size(125, 133)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If Me.m_content.DrawCircles Then
            Me.m_rbCircles.Checked = True
        Else
            Me.m_rbRectangles.Checked = True
        End If

        Me.m_cbShowGrid.Checked = Me.m_content.DrawGrid
        Me.m_cbSlantingLabels.Checked = Me.m_content.DrawSlanted

    End Sub

    Private Sub OnDrawModeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_rbCircles.CheckedChanged, m_rbRectangles.CheckedChanged

        If (Me.m_content Is Nothing) Then Return

        If Me.m_rbCircles.Checked Then
            Me.m_content.DrawCircles = True
        Else
            Me.m_content.DrawRectangles = True
        End If

    End Sub

    Private Sub OnShowGridChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cbShowGrid.CheckedChanged

        If (Me.m_content Is Nothing) Then Return

        Me.m_content.DrawGrid = Me.m_cbShowGrid.Checked
    End Sub

    Private Sub OnSlantLabelsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cbSlantingLabels.CheckedChanged

        If (Me.m_content Is Nothing) Then Return

        Me.m_content.DrawSlanted = Me.m_cbSlantingLabels.Checked
    End Sub

End Class
