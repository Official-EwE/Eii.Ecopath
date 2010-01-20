Namespace Import

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucImportPageProgress
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
            Me.m_lbProgress = New System.Windows.Forms.Label
            Me.m_lblSummary = New System.Windows.Forms.Label
            Me.m_pb = New System.Windows.Forms.ProgressBar
            Me.m_hdr = New ScientificInterface.Import.ucImportHeader
            Me.m_lbSummary = New System.Windows.Forms.ListBox
            Me.SuspendLayout()
            '
            'm_lbProgress
            '
            Me.m_lbProgress.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lbProgress.AutoSize = True
            Me.m_lbProgress.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lbProgress.Location = New System.Drawing.Point(3, 238)
            Me.m_lbProgress.Name = "m_lbProgress"
            Me.m_lbProgress.Size = New System.Drawing.Size(82, 13)
            Me.m_lbProgress.TabIndex = 8
            Me.m_lbProgress.Text = "Import progress:"
            '
            'm_lblSummary
            '
            Me.m_lblSummary.AutoSize = True
            Me.m_lblSummary.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblSummary.Location = New System.Drawing.Point(3, 73)
            Me.m_lblSummary.Name = "m_lblSummary"
            Me.m_lblSummary.Size = New System.Drawing.Size(53, 13)
            Me.m_lblSummary.TabIndex = 6
            Me.m_lblSummary.Text = "&Summary:"
            '
            'm_pb
            '
            Me.m_pb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pb.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_pb.Location = New System.Drawing.Point(0, 254)
            Me.m_pb.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
            Me.m_pb.Name = "m_pb"
            Me.m_pb.Size = New System.Drawing.Size(420, 23)
            Me.m_pb.Style = System.Windows.Forms.ProgressBarStyle.Continuous
            Me.m_pb.TabIndex = 9
            '
            'm_hdr
            '
            Me.m_hdr.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdr.BackColor = System.Drawing.Color.White
            Me.m_hdr.Location = New System.Drawing.Point(0, 0)
            Me.m_hdr.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdr.Name = "m_hdr"
            Me.m_hdr.Size = New System.Drawing.Size(420, 64)
            Me.m_hdr.SubText = ""
            Me.m_hdr.TabIndex = 10
            Me.m_hdr.Text = "Importing, please wait"
            '
            'm_lbSummary
            '
            Me.m_lbSummary.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lbSummary.Enabled = False
            Me.m_lbSummary.FormattingEnabled = True
            Me.m_lbSummary.IntegralHeight = False
            Me.m_lbSummary.Location = New System.Drawing.Point(0, 89)
            Me.m_lbSummary.Name = "m_lbSummary"
            Me.m_lbSummary.Size = New System.Drawing.Size(420, 146)
            Me.m_lbSummary.TabIndex = 11
            '
            'ucImportPageProgress
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lbSummary)
            Me.Controls.Add(Me.m_hdr)
            Me.Controls.Add(Me.m_lbProgress)
            Me.Controls.Add(Me.m_lblSummary)
            Me.Controls.Add(Me.m_pb)
            Me.Name = "ucImportPageProgress"
            Me.Size = New System.Drawing.Size(420, 277)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_hdr As ucImportHeader
        Private WithEvents m_lbProgress As System.Windows.Forms.Label
        Private WithEvents m_lblSummary As System.Windows.Forms.Label
        Private WithEvents m_pb As System.Windows.Forms.ProgressBar
        Private WithEvents m_lbSummary As System.Windows.Forms.ListBox

    End Class

End Namespace