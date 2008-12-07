Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAppPlugins
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAppPlugins))
            Me.lblTitle = New System.Windows.Forms.Label
            Me.m_tvPlugins = New System.Windows.Forms.TreeView
            Me.m_ilPlugins = New System.Windows.Forms.ImageList(Me.components)
            Me.m_split = New System.Windows.Forms.SplitContainer
            Me.m_split.Panel1.SuspendLayout()
            Me.m_split.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblTitle
            '
            Me.lblTitle.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Top
            Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblTitle.Location = New System.Drawing.Point(0, 0)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(414, 18)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Plugins"
            Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tvPlugins
            '
            Me.m_tvPlugins.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.m_tvPlugins.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tvPlugins.ImageIndex = 0
            Me.m_tvPlugins.ImageList = Me.m_ilPlugins
            Me.m_tvPlugins.Location = New System.Drawing.Point(0, 0)
            Me.m_tvPlugins.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tvPlugins.Name = "m_tvPlugins"
            Me.m_tvPlugins.SelectedImageIndex = 0
            Me.m_tvPlugins.Size = New System.Drawing.Size(134, 324)
            Me.m_tvPlugins.TabIndex = 2
            '
            'm_ilPlugins
            '
            Me.m_ilPlugins.ImageStream = CType(resources.GetObject("m_ilPlugins.ImageStream"), System.Windows.Forms.ImageListStreamer)
            Me.m_ilPlugins.TransparentColor = System.Drawing.Color.Transparent
            Me.m_ilPlugins.Images.SetKeyName(0, "Ecopath.ico")
            Me.m_ilPlugins.Images.SetKeyName(1, "NavForward.png")
            Me.m_ilPlugins.Images.SetKeyName(2, "BreakpointHS.png")
            Me.m_ilPlugins.Images.SetKeyName(3, "ConflictHS.png")
            '
            'm_split
            '
            Me.m_split.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_split.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_split.Location = New System.Drawing.Point(0, 25)
            Me.m_split.Margin = New System.Windows.Forms.Padding(0)
            Me.m_split.Name = "m_split"
            '
            'm_split.Panel1
            '
            Me.m_split.Panel1.Controls.Add(Me.m_tvPlugins)
            Me.m_split.Size = New System.Drawing.Size(414, 328)
            Me.m_split.SplitterDistance = 138
            Me.m_split.TabIndex = 5
            '
            'ucAppPlugins
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_split)
            Me.Controls.Add(Me.lblTitle)
            Me.Margin = New System.Windows.Forms.Padding(0)
            Me.Name = "ucAppPlugins"
            Me.Size = New System.Drawing.Size(414, 353)
            Me.m_split.Panel1.ResumeLayout(False)
            Me.m_split.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents m_tvPlugins As System.Windows.Forms.TreeView
        Friend WithEvents m_split As System.Windows.Forms.SplitContainer
        Private WithEvents m_ilPlugins As System.Windows.Forms.ImageList

    End Class

End Namespace
