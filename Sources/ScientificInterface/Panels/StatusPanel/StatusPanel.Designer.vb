Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class StatusPanel
    Inherits frmEwEDockContent

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


