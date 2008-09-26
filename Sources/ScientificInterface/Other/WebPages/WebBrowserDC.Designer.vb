Imports WeifenLuo.WinFormsUI.Docking

<CLSCompliant(False)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WebBrowserDC
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WebBrowserDC))
        Me.m_browser = New System.Windows.Forms.WebBrowser
        Me.SuspendLayout()
        '
        'WebBrowser1
        '
        resources.ApplyResources(Me.m_browser, "WebBrowser1")
        Me.m_browser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.m_browser.Name = "WebBrowser1"
        '
        'WebBrowserDC
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_browser)
        Me.HideOnClose = True
        Me.Name = "WebBrowserDC"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents m_browser As System.Windows.Forms.WebBrowser

End Class
