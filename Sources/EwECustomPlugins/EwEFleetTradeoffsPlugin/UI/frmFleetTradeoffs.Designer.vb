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
        Me.m_lblStart = New System.Windows.Forms.Label()
        Me.m_ = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_tbxFrom = New System.Windows.Forms.TextBox()
        Me.m_tbxTo = New System.Windows.Forms.TextBox()
        Me.m_tbxStep = New System.Windows.Forms.TextBox()
        Me.m_lblOutput = New System.Windows.Forms.Label()
        Me.m_tbxOutput = New System.Windows.Forms.TextBox()
        Me.m_btnChangeOutput = New System.Windows.Forms.Button()
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
        'm_lblStart
        '
        resources.ApplyResources(Me.m_lblStart, "m_lblStart")
        Me.m_lblStart.Name = "m_lblStart"
        '
        'm_
        '
        resources.ApplyResources(Me.m_, "m_")
        Me.m_.Name = "m_"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'm_tbxFrom
        '
        resources.ApplyResources(Me.m_tbxFrom, "m_tbxFrom")
        Me.m_tbxFrom.Name = "m_tbxFrom"
        '
        'm_tbxTo
        '
        resources.ApplyResources(Me.m_tbxTo, "m_tbxTo")
        Me.m_tbxTo.Name = "m_tbxTo"
        '
        'm_tbxStep
        '
        resources.ApplyResources(Me.m_tbxStep, "m_tbxStep")
        Me.m_tbxStep.Name = "m_tbxStep"
        '
        'm_lblOutput
        '
        resources.ApplyResources(Me.m_lblOutput, "m_lblOutput")
        Me.m_lblOutput.Name = "m_lblOutput"
        '
        'm_tbxOutput
        '
        resources.ApplyResources(Me.m_tbxOutput, "m_tbxOutput")
        Me.m_tbxOutput.Name = "m_tbxOutput"
        Me.m_tbxOutput.ReadOnly = True
        '
        'm_btnChangeOutput
        '
        resources.ApplyResources(Me.m_btnChangeOutput, "m_btnChangeOutput")
        Me.m_btnChangeOutput.Name = "m_btnChangeOutput"
        Me.m_btnChangeOutput.UseVisualStyleBackColor = True
        '
        'frmFleetTradeoffs
        '
        Me.AcceptButton = Me.m_btnRun
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.m_btnChangeOutput)
        Me.Controls.Add(Me.m_tbxOutput)
        Me.Controls.Add(Me.m_lblOutput)
        Me.Controls.Add(Me.m_tbxStep)
        Me.Controls.Add(Me.m_tbxTo)
        Me.Controls.Add(Me.m_tbxFrom)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_)
        Me.Controls.Add(Me.m_lblStart)
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
        Me.PerformLayout()

    End Sub

    Friend WithEvents m_btnRun As Windows.Forms.Button
    Friend WithEvents m_btnCancel As Windows.Forms.Button
    Private WithEvents m_lblInfo As Windows.Forms.Label
    Private WithEvents m_progress As Windows.Forms.ProgressBar
    Friend WithEvents m_lblStart As Windows.Forms.Label
    Friend WithEvents m_ As Windows.Forms.Label
    Friend WithEvents Label1 As Windows.Forms.Label
    Private WithEvents m_tbxFrom As Windows.Forms.TextBox
    Private WithEvents m_tbxTo As Windows.Forms.TextBox
    Private WithEvents m_tbxStep As Windows.Forms.TextBox
    Private WithEvents m_lblOutput As Windows.Forms.Label
    Private WithEvents m_tbxOutput As Windows.Forms.TextBox
    Private WithEvents m_btnChangeOutput As Windows.Forms.Button
End Class
