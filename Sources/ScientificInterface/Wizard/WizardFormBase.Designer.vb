Namespace Wizard
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class WizardFormBase
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WizardFormBase))
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.btnBack = New System.Windows.Forms.Button
            Me.btnCancel = New System.Windows.Forms.Button
            Me.btnNext = New System.Windows.Forms.Button
            Me.btnFinish = New System.Windows.Forms.Button
            Me.tcMain = New System.Windows.Forms.TabControl
            Me.Separator1 = New ucFormSeparator
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.btnBack, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.btnCancel, 3, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.btnNext, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.btnFinish, 2, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'btnBack
            '
            resources.ApplyResources(Me.btnBack, "btnBack")
            Me.btnBack.Name = "btnBack"
            Me.btnBack.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            resources.ApplyResources(Me.btnCancel, "btnCancel")
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Name = "btnCancel"
            '
            'btnNext
            '
            resources.ApplyResources(Me.btnNext, "btnNext")
            Me.btnNext.Name = "btnNext"
            Me.btnNext.UseVisualStyleBackColor = True
            '
            'btnFinish
            '
            resources.ApplyResources(Me.btnFinish, "btnFinish")
            Me.btnFinish.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnFinish.Name = "btnFinish"
            '
            'tcMain
            '
            resources.ApplyResources(Me.tcMain, "tcMain")
            Me.tcMain.Name = "tcMain"
            Me.tcMain.SelectedIndex = 0
            '
            'Separator1
            '
            resources.ApplyResources(Me.Separator1, "Separator1")
            Me.Separator1.Name = "Separator1"
            '
            'WizardFormBase
            '
            Me.AcceptButton = Me.btnNext
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.btnCancel
            Me.Controls.Add(Me.tcMain)
            Me.Controls.Add(Me.Separator1)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "WizardFormBase"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents btnBack As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents btnNext As System.Windows.Forms.Button
        Friend WithEvents btnFinish As System.Windows.Forms.Button
        Friend WithEvents Separator1 As ucFormSeparator
        Protected WithEvents tcMain As System.Windows.Forms.TabControl
    End Class

End Namespace
