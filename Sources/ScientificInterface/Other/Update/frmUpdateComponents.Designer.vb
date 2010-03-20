<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUpdateComponents
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUpdateComponents))
        Me.m_lblInfo = New System.Windows.Forms.Label
        Me.m_pbProgress = New System.Windows.Forms.ProgressBar
        Me.m_btnAbort = New System.Windows.Forms.Button
        Me.m_tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.m_tlpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblInfo
        '
        resources.ApplyResources(Me.m_lblInfo, "m_lblInfo")
        Me.m_lblInfo.Name = "m_lblInfo"
        '
        'm_pbProgress
        '
        resources.ApplyResources(Me.m_pbProgress, "m_pbProgress")
        Me.m_pbProgress.Name = "m_pbProgress"
        Me.m_pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'm_btnAbort
        '
        resources.ApplyResources(Me.m_btnAbort, "m_btnAbort")
        Me.m_btnAbort.Name = "m_btnAbort"
        Me.m_btnAbort.UseVisualStyleBackColor = True
        '
        'm_tlpButtons
        '
        resources.ApplyResources(Me.m_tlpButtons, "m_tlpButtons")
        Me.m_tlpButtons.Controls.Add(Me.m_btnAbort, 1, 0)
        Me.m_tlpButtons.Name = "m_tlpButtons"
        '
        'frmUpdateComponents
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_tlpButtons)
        Me.Controls.Add(Me.m_pbProgress)
        Me.Controls.Add(Me.m_lblInfo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "frmUpdateComponents"
        Me.TopMost = True
        Me.m_tlpButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_pbProgress As System.Windows.Forms.ProgressBar
    Private WithEvents m_lblInfo As System.Windows.Forms.Label
    Private WithEvents m_btnAbort As System.Windows.Forms.Button
    Private WithEvents m_tlpButtons As System.Windows.Forms.TableLayoutPanel
End Class
