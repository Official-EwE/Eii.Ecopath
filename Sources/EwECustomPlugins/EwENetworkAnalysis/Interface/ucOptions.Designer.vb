<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptions
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
        Me.m_hdrOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_cbCalcCyclesPathways = New System.Windows.Forms.CheckBox()
        Me.m_lblTimeout = New System.Windows.Forms.Label()
        Me.m_nudTimeOut = New System.Windows.Forms.NumericUpDown()
        Me.m_lblTimeOutUnit = New System.Windows.Forms.Label()
        CType(Me.m_nudTimeOut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_hdrOptions
        '
        Me.m_hdrOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrOptions.CanCollapseParent = False
        Me.m_hdrOptions.CollapsedParentHeight = 0
        Me.m_hdrOptions.IsCollapsed = False
        Me.m_hdrOptions.Location = New System.Drawing.Point(3, 4)
        Me.m_hdrOptions.Name = "m_hdrOptions"
        Me.m_hdrOptions.Size = New System.Drawing.Size(161, 18)
        Me.m_hdrOptions.TabIndex = 0
        Me.m_hdrOptions.Text = "Generic options"
        Me.m_hdrOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_cbCalcCyclesPathways
        '
        Me.m_cbCalcCyclesPathways.AutoSize = True
        Me.m_cbCalcCyclesPathways.Location = New System.Drawing.Point(6, 32)
        Me.m_cbCalcCyclesPathways.Name = "m_cbCalcCyclesPathways"
        Me.m_cbCalcCyclesPathways.Size = New System.Drawing.Size(103, 17)
        Me.m_cbCalcCyclesPathways.TabIndex = 1
        Me.m_cbCalcCyclesPathways.Text = "Calculate cycles"
        Me.m_cbCalcCyclesPathways.UseVisualStyleBackColor = True
        '
        'm_lblTimeout
        '
        Me.m_lblTimeout.AutoSize = True
        Me.m_lblTimeout.Location = New System.Drawing.Point(22, 57)
        Me.m_lblTimeout.Name = "m_lblTimeout"
        Me.m_lblTimeout.Size = New System.Drawing.Size(51, 13)
        Me.m_lblTimeout.TabIndex = 2
        Me.m_lblTimeout.Text = "Time out:"
        Me.m_lblTimeout.Visible = False
        '
        'm_nudTimeOut
        '
        Me.m_nudTimeOut.Location = New System.Drawing.Point(79, 55)
        Me.m_nudTimeOut.Maximum = New Decimal(New Integer() {360, 0, 0, 0})
        Me.m_nudTimeOut.Name = "m_nudTimeOut"
        Me.m_nudTimeOut.Size = New System.Drawing.Size(49, 20)
        Me.m_nudTimeOut.TabIndex = 3
        Me.m_nudTimeOut.Visible = False
        '
        'm_lblTimeOutUnit
        '
        Me.m_lblTimeOutUnit.AutoSize = True
        Me.m_lblTimeOutUnit.Location = New System.Drawing.Point(134, 57)
        Me.m_lblTimeOutUnit.Name = "m_lblTimeOutUnit"
        Me.m_lblTimeOutUnit.Size = New System.Drawing.Size(31, 13)
        Me.m_lblTimeOutUnit.TabIndex = 4
        Me.m_lblTimeOutUnit.Text = "mins."
        Me.m_lblTimeOutUnit.Visible = False
        '
        'ucOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_lblTimeOutUnit)
        Me.Controls.Add(Me.m_nudTimeOut)
        Me.Controls.Add(Me.m_lblTimeout)
        Me.Controls.Add(Me.m_cbCalcCyclesPathways)
        Me.Controls.Add(Me.m_hdrOptions)
        Me.Name = "ucOptions"
        Me.Size = New System.Drawing.Size(167, 54)
        CType(Me.m_nudTimeOut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_hdrOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_cbCalcCyclesPathways As System.Windows.Forms.CheckBox
    Private WithEvents m_lblTimeout As System.Windows.Forms.Label
    Private WithEvents m_nudTimeOut As System.Windows.Forms.NumericUpDown
    Private WithEvents m_lblTimeOutUnit As System.Windows.Forms.Label

End Class
