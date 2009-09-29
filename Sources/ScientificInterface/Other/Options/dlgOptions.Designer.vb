Namespace Other
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgOptions
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgOptions))
            Me.m_btnOk = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.tvOptions = New System.Windows.Forms.TreeView
            Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
            Me.plOption = New System.Windows.Forms.Panel
            Me.m_btnApply = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'm_btnOk
            '
            resources.ApplyResources(Me.m_btnOk, "m_btnOk")
            Me.m_btnOk.Name = "m_btnOk"
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            '
            'tvOptions
            '
            resources.ApplyResources(Me.tvOptions, "tvOptions")
            Me.tvOptions.HideSelection = False
            Me.tvOptions.Name = "tvOptions"
            Me.tvOptions.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("tvOptions.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvOptions.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvOptions.Nodes2"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvOptions.Nodes3"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvOptions.Nodes4"), System.Windows.Forms.TreeNode)})
            '
            'ImageList1
            '
            Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
            Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
            Me.ImageList1.Images.SetKeyName(0, "application.png")
            Me.ImageList1.Images.SetKeyName(1, "color_wheel.png")
            '
            'plOption
            '
            resources.ApplyResources(Me.plOption, "plOption")
            Me.plOption.Name = "plOption"
            '
            'm_btnApply
            '
            resources.ApplyResources(Me.m_btnApply, "m_btnApply")
            Me.m_btnApply.Name = "m_btnApply"
            '
            'dlgOptions
            '
            Me.AcceptButton = Me.m_btnOk
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_btnApply)
            Me.Controls.Add(Me.m_btnOk)
            Me.Controls.Add(Me.plOption)
            Me.Controls.Add(Me.tvOptions)
            Me.DoubleBuffered = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgOptions"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_btnCancel As System.Windows.Forms.Button
        Friend WithEvents tvOptions As System.Windows.Forms.TreeView
        Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
        Friend WithEvents plOption As System.Windows.Forms.Panel
        Friend WithEvents tnModel As System.Windows.Forms.TreeNode
        Private WithEvents m_btnOk As System.Windows.Forms.Button
        Private WithEvents m_btnApply As System.Windows.Forms.Button
    End Class

End Namespace