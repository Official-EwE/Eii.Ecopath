Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRemarkPanel
    Inherits frmEwEDockContent

    Private components As System.ComponentModel.IContainer

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRemarkPanel))
        Me.m_tbRemark = New System.Windows.Forms.TextBox
        Me.m_lbVarName = New System.Windows.Forms.Label
        Me.m_btnApply = New System.Windows.Forms.Button
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
        'm_btnApply
        '
        resources.ApplyResources(Me.m_btnApply, "m_btnApply")
        Me.m_btnApply.Name = "m_btnApply"
        Me.m_btnApply.UseVisualStyleBackColor = True
        '
        'frmRemarkPanel
        '
        Me.AcceptButton = Me.m_btnApply
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_btnApply)
        Me.Controls.Add(Me.m_lbVarName)
        Me.Controls.Add(Me.m_tbRemark)
        Me.DockAreas = CType((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) _
                    Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) _
                    Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom), WeifenLuo.WinFormsUI.Docking.DockAreas)
        Me.HideOnClose = True
        Me.Name = "frmRemarkPanel"
        Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TabText = "Remarks"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_tbRemark As System.Windows.Forms.TextBox
    Private WithEvents m_lbVarName As System.Windows.Forms.Label
    Private WithEvents m_btnApply As System.Windows.Forms.Button
End Class
