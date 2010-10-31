Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Forms

<CLSCompliant(False)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWebBrowser
    Inherits frmEwE

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
        Me.m_tsbnBack = New System.Windows.Forms.ToolStripButton
        Me.m_tsbnForward = New System.Windows.Forms.ToolStripButton
        Me.m_sep2 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsbnRefresh = New System.Windows.Forms.ToolStripButton
        Me.m_sep1 = New System.Windows.Forms.ToolStripSeparator
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
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnHome, Me.m_sep1, Me.m_tsbnBack, Me.m_tsbnForward, Me.m_sep2, Me.m_tsbnRefresh})
        resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
        Me.ToolStrip1.Name = "ToolStrip1"
        '
        'm_tsbnHome
        '
        Me.m_tsbnHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsbnHome.Image = SharedResources.HomeHS
        resources.ApplyResources(Me.m_tsbnHome, "m_tsbnHome")
        Me.m_tsbnHome.Name = "m_tsbnHome"
        '
        'm_tsbnBack
        '
        Me.m_tsbnBack.Image = SharedResources.NavBack
        resources.ApplyResources(Me.m_tsbnBack, "m_tsbnBack")
        Me.m_tsbnBack.Name = "m_tsbnBack"
        '
        'm_tsbnForward
        '
        Me.m_tsbnForward.Image = SharedResources.NavForward
        resources.ApplyResources(Me.m_tsbnForward, "m_tsbnForward")
        Me.m_tsbnForward.Name = "m_tsbnForward"
        '
        'm_sep2
        '
        Me.m_sep2.Name = "m_sep2"
        resources.ApplyResources(Me.m_sep2, "m_sep2")
        '
        'm_tsbnRefresh
        '
        resources.ApplyResources(Me.m_tsbnRefresh, "m_tsbnRefresh")
        Me.m_tsbnRefresh.Name = "m_tsbnRefresh"
        '
        'm_sep1
        '
        Me.m_sep1.Name = "m_sep1"
        resources.ApplyResources(Me.m_sep1, "m_sep1")
        '
        'frmWebBrowser
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlp)
        Me.HideOnClose = True
        Me.Name = "frmWebBrowser"
        Me.TabText = "Home"
        Me.m_tlp.ResumeLayout(False)
        Me.m_tlp.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_browser As System.Windows.Forms.WebBrowser
    Private WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Private WithEvents m_tsbnBack As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tlp As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_tsbnForward As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnRefresh As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnHome As System.Windows.Forms.ToolStripButton
    Private WithEvents m_sep1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_sep2 As System.Windows.Forms.ToolStripSeparator


End Class
