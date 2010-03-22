Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RemarkPanel
    Inherits DockContent

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RemarkPanel))
        Me.m_tbRemark = New System.Windows.Forms.TextBox
        Me.m_lbVarName = New System.Windows.Forms.Label
        Me.m_btnSet = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'm_tbRemark
        '
        resources.ApplyResources(Me.m_tbRemark, "m_tbRemark")
        Me.m_tbRemark.Name = "m_tbRemark"
        '
        'm_lbVarName
        '
        resources.ApplyResources(Me.m_lbVarName, "m_lbVarName")
        Me.m_lbVarName.Name = "m_lbVarName"
        '
        'm_btnSet
        '
        resources.ApplyResources(Me.m_btnSet, "m_btnSet")
        Me.m_btnSet.Image = Global.ScientificInterface.My.Resources.Resources.NavForward
        Me.m_btnSet.Name = "m_btnSet"
        Me.m_btnSet.UseVisualStyleBackColor = True
        '
        'RemarkPanel
        '
        Me.AcceptButton = Me.m_btnSet
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_btnSet)
        Me.Controls.Add(Me.m_lbVarName)
        Me.Controls.Add(Me.m_tbRemark)
        Me.HideOnClose = True
        Me.Name = "RemarkPanel"
        Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_tbRemark As System.Windows.Forms.TextBox
    Friend WithEvents m_lbVarName As System.Windows.Forms.Label
    Friend WithEvents m_btnSet As System.Windows.Forms.Button
End Class
