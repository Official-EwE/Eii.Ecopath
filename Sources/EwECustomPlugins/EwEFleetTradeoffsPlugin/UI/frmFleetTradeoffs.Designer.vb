Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmFleetTradeoffs
    Inherits System.Windows.Forms.Form

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFleetTradeoffs))
        Me.m_btnRun = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_lblInfo = New System.Windows.Forms.Label()
        Me.m_progress = New System.Windows.Forms.ProgressBar()
        Me.SuspendLayout()
        '
        'm_btnRun
        '
        resources.ApplyResources(Me.m_btnRun, "m_btnRun")
        Me.m_btnRun.Name = "m_btnRun"
        Me.m_btnRun.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_lblInfo
        '
        resources.ApplyResources(Me.m_lblInfo, "m_lblInfo")
        Me.m_lblInfo.Name = "m_lblInfo"
        '
        'm_progress
        '
        resources.ApplyResources(Me.m_progress, "m_progress")
        Me.m_progress.Name = "m_progress"
        Me.m_progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'frmFleetTradeoffs
        '
        Me.AcceptButton = Me.m_btnRun
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.m_progress)
        Me.Controls.Add(Me.m_lblInfo)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnRun)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFleetTradeoffs"
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents m_btnRun As Windows.Forms.Button
    Friend WithEvents m_btnCancel As Windows.Forms.Button
    Private WithEvents m_lblInfo As Windows.Forms.Label
    Private WithEvents m_progress As Windows.Forms.ProgressBar
End Class
