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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgOptions))
            Me.m_btnOk = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_tvOptions = New System.Windows.Forms.TreeView
            Me.m_btnApply = New System.Windows.Forms.Button
            Me.m_scContent = New System.Windows.Forms.SplitContainer
            Me.m_scContent.Panel1.SuspendLayout()
            Me.m_scContent.SuspendLayout()
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
            'm_tvOptions
            '
            resources.ApplyResources(Me.m_tvOptions, "m_tvOptions")
            Me.m_tvOptions.FullRowSelect = True
            Me.m_tvOptions.HideSelection = False
            Me.m_tvOptions.LineColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
            Me.m_tvOptions.Name = "m_tvOptions"
            Me.m_tvOptions.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("m_tvOptions.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvOptions.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvOptions.Nodes2"), System.Windows.Forms.TreeNode)})
            '
            'm_btnApply
            '
            resources.ApplyResources(Me.m_btnApply, "m_btnApply")
            Me.m_btnApply.Name = "m_btnApply"
            '
            'm_scContent
            '
            resources.ApplyResources(Me.m_scContent, "m_scContent")
            Me.m_scContent.Name = "m_scContent"
            '
            'm_scContent.Panel1
            '
            Me.m_scContent.Panel1.Controls.Add(Me.m_tvOptions)
            '
            'dlgOptions
            '
            Me.AcceptButton = Me.m_btnOk
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.Controls.Add(Me.m_scContent)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_btnApply)
            Me.Controls.Add(Me.m_btnOk)
            Me.DoubleBuffered = True
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgOptions"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.m_scContent.Panel1.ResumeLayout(False)
            Me.m_scContent.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_btnOk As System.Windows.Forms.Button
        Private WithEvents m_btnApply As System.Windows.Forms.Button
        Private WithEvents m_tvOptions As System.Windows.Forms.TreeView
        Private WithEvents m_scContent As System.Windows.Forms.SplitContainer
    End Class

End Namespace