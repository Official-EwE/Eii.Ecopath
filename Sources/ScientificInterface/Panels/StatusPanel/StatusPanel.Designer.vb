Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class StatusPanel
    Inherits DockContent

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StatusPanel))
        Me.m_tvStatus = New System.Windows.Forms.TreeView
        Me.SuspendLayout()
        '
        'm_tvStatus
        '
        resources.ApplyResources(Me.m_tvStatus, "m_tvStatus")
        Me.m_tvStatus.Name = "m_tvStatus"
        Me.m_tvStatus.ShowLines = False
        '
        'StatusPanel
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CloseButton = False
        Me.Controls.Add(Me.m_tvStatus)
        Me.DoubleBuffered = True
        Me.HideOnClose = True
        Me.Name = "StatusPanel"
        Me.TabText = "Status"
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tvStatus As System.Windows.Forms.TreeView

End Class


