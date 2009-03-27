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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StatusPanel))
        Me.cmenuListBox = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.Item_Remove = New System.Windows.Forms.ToolStripMenuItem
        Me.Item_RemoveAll = New System.Windows.Forms.ToolStripMenuItem
        Me.tvStatus = New System.Windows.Forms.TreeView
        Me.cmenuListBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmenuListBox
        '
        Me.cmenuListBox.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Item_Remove, Me.Item_RemoveAll})
        Me.cmenuListBox.Name = "cmenuListBox"
        resources.ApplyResources(Me.cmenuListBox, "cmenuListBox")
        '
        'Item_Remove
        '
        Me.Item_Remove.Name = "Item_Remove"
        resources.ApplyResources(Me.Item_Remove, "Item_Remove")
        '
        'Item_RemoveAll
        '
        Me.Item_RemoveAll.Name = "Item_RemoveAll"
        resources.ApplyResources(Me.Item_RemoveAll, "Item_RemoveAll")
        '
        'tvStatus
        '
        resources.ApplyResources(Me.tvStatus, "tvStatus")
        Me.tvStatus.Name = "tvStatus"
        Me.tvStatus.ShowLines = False
        '
        'StatusPanel
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CloseButton = False
        Me.ContextMenuStrip = Me.cmenuListBox
        Me.Controls.Add(Me.tvStatus)
        Me.DoubleBuffered = True
        Me.HideOnClose = True
        Me.Name = "StatusPanel"
        Me.cmenuListBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents cmenuListBox As System.Windows.Forms.ContextMenuStrip
    Private WithEvents Item_Remove As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents Item_RemoveAll As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents tvStatus As System.Windows.Forms.TreeView

End Class


