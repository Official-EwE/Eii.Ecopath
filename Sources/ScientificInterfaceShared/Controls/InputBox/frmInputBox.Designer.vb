Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmInputBox
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmInputBox))
            Me.m_lblPrompt = New System.Windows.Forms.Label()
            Me.m_tbxValue = New System.Windows.Forms.TextBox()
            Me.m_btnOk = New System.Windows.Forms.Button()
            Me.m_btnCancel = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'm_lblPrompt
            '
            resources.ApplyResources(Me.m_lblPrompt, "m_lblPrompt")
            Me.m_lblPrompt.Name = "m_lblPrompt"
            '
            'm_tbxValue
            '
            resources.ApplyResources(Me.m_tbxValue, "m_tbxValue")
            Me.m_tbxValue.Name = "m_tbxValue"
            '
            'm_btnOk
            '
            resources.ApplyResources(Me.m_btnOk, "m_btnOk")
            Me.m_btnOk.Name = "m_btnOk"
            Me.m_btnOk.UseVisualStyleBackColor = True
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.Name = "m_btnCancel"
            Me.m_btnCancel.UseVisualStyleBackColor = True
            '
            'frmInput
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ControlBox = False
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_btnOk)
            Me.Controls.Add(Me.m_tbxValue)
            Me.Controls.Add(Me.m_lblPrompt)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmInput"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblPrompt As System.Windows.Forms.Label
        Private WithEvents m_tbxValue As System.Windows.Forms.TextBox
        Private WithEvents m_btnOk As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
    End Class

End Namespace
