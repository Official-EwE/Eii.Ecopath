Namespace Import

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucImportHeader
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
            Me.m_pbEwE6Logo = New System.Windows.Forms.PictureBox
            Me.m_pbEwE5logo = New System.Windows.Forms.PictureBox
            Me.m_lblHeader = New System.Windows.Forms.Label
            Me.m_lblSubheader = New System.Windows.Forms.Label
            CType(Me.m_pbEwE6Logo, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbEwE5logo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_pbEwE6Logo
            '
            Me.m_pbEwE6Logo.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pbEwE6Logo.Image = Global.ScientificInterface.My.Resources.Resources.ecopath_256x256
            Me.m_pbEwE6Logo.Location = New System.Drawing.Point(394, 0)
            Me.m_pbEwE6Logo.Name = "m_pbEwE6Logo"
            Me.m_pbEwE6Logo.Size = New System.Drawing.Size(64, 64)
            Me.m_pbEwE6Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
            Me.m_pbEwE6Logo.TabIndex = 0
            Me.m_pbEwE6Logo.TabStop = False
            '
            'm_pbEwE5logo
            '
            Me.m_pbEwE5logo.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo
            Me.m_pbEwE5logo.Location = New System.Drawing.Point(0, 0)
            Me.m_pbEwE5logo.Name = "m_pbEwE5logo"
            Me.m_pbEwE5logo.Size = New System.Drawing.Size(77, 64)
            Me.m_pbEwE5logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
            Me.m_pbEwE5logo.TabIndex = 0
            Me.m_pbEwE5logo.TabStop = False
            '
            'm_lblHeader
            '
            Me.m_lblHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblHeader.Location = New System.Drawing.Point(83, 1)
            Me.m_lblHeader.Name = "m_lblHeader"
            Me.m_lblHeader.Size = New System.Drawing.Size(305, 26)
            Me.m_lblHeader.TabIndex = 0
            Me.m_lblHeader.Text = "Header"
            Me.m_lblHeader.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            '
            'm_lblSubheader
            '
            Me.m_lblSubheader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblSubheader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblSubheader.Location = New System.Drawing.Point(83, 31)
            Me.m_lblSubheader.Name = "m_lblSubheader"
            Me.m_lblSubheader.Size = New System.Drawing.Size(305, 33)
            Me.m_lblSubheader.TabIndex = 0
            Me.m_lblSubheader.Text = "Sub header"
            Me.m_lblSubheader.TextAlign = System.Drawing.ContentAlignment.TopCenter
            '
            'ucImportHeader
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.White
            Me.Controls.Add(Me.m_lblSubheader)
            Me.Controls.Add(Me.m_lblHeader)
            Me.Controls.Add(Me.m_pbEwE6Logo)
            Me.Controls.Add(Me.m_pbEwE5logo)
            Me.Margin = New System.Windows.Forms.Padding(0)
            Me.Name = "ucImportHeader"
            Me.Size = New System.Drawing.Size(458, 64)
            CType(Me.m_pbEwE6Logo, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbEwE5logo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_pbEwE6Logo As System.Windows.Forms.PictureBox
        Private WithEvents m_pbEwE5logo As System.Windows.Forms.PictureBox
        Private WithEvents m_lblHeader As System.Windows.Forms.Label
        Private WithEvents m_lblSubheader As System.Windows.Forms.Label

    End Class

End Namespace