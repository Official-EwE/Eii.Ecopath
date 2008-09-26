Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgDisplayGroups
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgDisplayGroups))
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_clbGroups = New System.Windows.Forms.CheckedListBox
            Me.m_btnAll = New System.Windows.Forms.Button
            Me.m_btnNone = New System.Windows.Forms.Button
            Me.m_btnDefault = New System.Windows.Forms.Button
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
            'm_clbGroups
            '
            resources.ApplyResources(Me.m_clbGroups, "m_clbGroups")
            Me.m_clbGroups.CheckOnClick = True
            Me.m_clbGroups.FormattingEnabled = True
            Me.m_clbGroups.Name = "m_clbGroups"
            '
            'm_btnAll
            '
            resources.ApplyResources(Me.m_btnAll, "m_btnAll")
            Me.m_btnAll.Name = "m_btnAll"
            '
            'm_btnNone
            '
            resources.ApplyResources(Me.m_btnNone, "m_btnNone")
            Me.m_btnNone.Name = "m_btnNone"
            '
            'm_btnDefault
            '
            resources.ApplyResources(Me.m_btnDefault, "m_btnDefault")
            Me.m_btnDefault.Name = "m_btnDefault"
            '
            'dlgDisplayGroups
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.ControlBox = False
            Me.Controls.Add(Me.m_clbGroups)
            Me.Controls.Add(Me.m_btnDefault)
            Me.Controls.Add(Me.m_btnNone)
            Me.Controls.Add(Me.m_btnAll)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.DoubleBuffered = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgDisplayGroups"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents OK_Button As System.Windows.Forms.Button
        Friend WithEvents Cancel_Button As System.Windows.Forms.Button
        Friend WithEvents m_clbGroups As System.Windows.Forms.CheckedListBox
        Friend WithEvents m_btnAll As System.Windows.Forms.Button
        Friend WithEvents m_btnNone As System.Windows.Forms.Button
        Friend WithEvents m_btnDefault As System.Windows.Forms.Button
    End Class

End Namespace

