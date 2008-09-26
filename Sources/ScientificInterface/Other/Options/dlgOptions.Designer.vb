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
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.tvOptions = New System.Windows.Forms.TreeView
            Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
            Me.plOption = New System.Windows.Forms.Panel
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'OK_Button
            '
            resources.ApplyResources(Me.OK_Button, "OK_Button")
            Me.OK_Button.Name = "OK_Button"
            '
            'Cancel_Button
            '
            resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Name = "Cancel_Button"
            '
            'tvOptions
            '
            resources.ApplyResources(Me.tvOptions, "tvOptions")
            Me.tvOptions.HideSelection = False
            Me.tvOptions.Name = "tvOptions"
            Me.tvOptions.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("tvOptions.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvOptions.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvOptions.Nodes2"), System.Windows.Forms.TreeNode)})
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
            'dlgOptions
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.plOption)
            Me.Controls.Add(Me.tvOptions)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.DoubleBuffered = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgOptions"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents OK_Button As System.Windows.Forms.Button
        Friend WithEvents Cancel_Button As System.Windows.Forms.Button
        Friend WithEvents tvOptions As System.Windows.Forms.TreeView
        Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
        Friend WithEvents plOption As System.Windows.Forms.Panel
        Friend WithEvents tnModel As System.Windows.Forms.TreeNode
    End Class

End Namespace