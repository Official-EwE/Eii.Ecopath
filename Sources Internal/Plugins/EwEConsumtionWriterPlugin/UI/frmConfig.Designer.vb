<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConfig
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConfig))
        Me.m_cbIncludeDetritus = New System.Windows.Forms.CheckBox()
        Me.m_cbIncludeImportAndSum = New System.Windows.Forms.CheckBox()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'm_cbIncludeDetritus
        '
        resources.ApplyResources(Me.m_cbIncludeDetritus, "m_cbIncludeDetritus")
        Me.m_cbIncludeDetritus.Name = "m_cbIncludeDetritus"
        Me.m_cbIncludeDetritus.UseVisualStyleBackColor = True
        '
        'm_cbIncludeImportAndSum
        '
        resources.ApplyResources(Me.m_cbIncludeImportAndSum, "m_cbIncludeImportAndSum")
        Me.m_cbIncludeImportAndSum.Name = "m_cbIncludeImportAndSum"
        Me.m_cbIncludeImportAndSum.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'frmConfig
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_cbIncludeImportAndSum)
        Me.Controls.Add(Me.m_cbIncludeDetritus)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "frmConfig"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_cbIncludeImportAndSum As System.Windows.Forms.CheckBox
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Friend WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_cbIncludeDetritus As System.Windows.Forms.CheckBox
End Class
