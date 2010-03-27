Imports WeifenLuo.WinFormsUI.Docking

<CLSCompliant(False)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWebBrowser
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWebBrowser))
        Me.m_tlp = New System.Windows.Forms.TableLayoutPanel
        Me.m_browser = New System.Windows.Forms.WebBrowser
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.m_tsbnHome = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsbnBack = New System.Windows.Forms.ToolStripButton
        Me.m_tsbnForward = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsbnRefresh = New System.Windows.Forms.ToolStripButton
        Me.m_tlp.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tlp
        '
        resources.ApplyResources(Me.m_tlp, "m_tlp")
        Me.m_tlp.Controls.Add(Me.m_browser, 0, 1)
        Me.m_tlp.Controls.Add(Me.ToolStrip1, 0, 0)
        Me.m_tlp.Name = "m_tlp"
        '
        'm_browser
        '
        Me.m_browser.AllowWebBrowserDrop = False
        resources.ApplyResources(Me.m_browser, "m_browser")
        Me.m_browser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.m_browser.Name = "m_browser"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnHome, Me.ToolStripSeparator2, Me.m_tsbnBack, Me.m_tsbnForward, Me.ToolStripSeparator1, Me.m_tsbnRefresh})
        resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
        Me.ToolStrip1.Name = "ToolStrip1"
        '
        'm_tsbnHome
        '
        Me.m_tsbnHome.Image = Global.ScientificInterface.My.Resources.Resources.HomeHS
        resources.ApplyResources(Me.m_tsbnHome, "m_tsbnHome")
        Me.m_tsbnHome.Name = "m_tsbnHome"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'm_tsbnBack
        '
        Me.m_tsbnBack.Image = Global.ScientificInterface.My.Resources.Resources.NavBack
        resources.ApplyResources(Me.m_tsbnBack, "m_tsbnBack")
        Me.m_tsbnBack.Name = "m_tsbnBack"
        '
        'm_tsbnForward
        '
        Me.m_tsbnForward.Image = Global.ScientificInterface.My.Resources.Resources.NavForward
        resources.ApplyResources(Me.m_tsbnForward, "m_tsbnForward")
        Me.m_tsbnForward.Name = "m_tsbnForward"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'm_tsbnRefresh
        '
        Me.m_tsbnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnRefresh, "m_tsbnRefresh")
        Me.m_tsbnRefresh.Name = "m_tsbnRefresh"
        '
        'WebBrowserDC
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlp)
        Me.HideOnClose = True
        Me.Name = "WebBrowserDC"
        Me.TabText = "Home"
        Me.m_tlp.ResumeLayout(False)
        Me.m_tlp.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_browser As System.Windows.Forms.WebBrowser
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Private WithEvents m_tsbnBack As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tlp As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_tsbnForward As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbnRefresh As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnHome As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator


End Class
