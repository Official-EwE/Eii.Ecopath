Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated(), CLSCompliant(False)> _
    Partial Class SRplot
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SRplot))
            Me.m_plot = New ZedGraph.ZedGraphControl
            Me.m_tvGroups = New System.Windows.Forms.TreeView
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.m_lblPt = New System.Windows.Forms.Label
            Me.m_btnRun = New System.Windows.Forms.Button
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_plot
            '
            resources.ApplyResources(Me.m_plot, "m_plot")
            Me.m_plot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plot.Name = "m_plot"
            Me.m_plot.ScrollGrace = 0
            Me.m_plot.ScrollMaxX = 0
            Me.m_plot.ScrollMaxY = 0
            Me.m_plot.ScrollMaxY2 = 0
            Me.m_plot.ScrollMinX = 0
            Me.m_plot.ScrollMinY = 0
            Me.m_plot.ScrollMinY2 = 0
            '
            'm_tvGroups
            '
            resources.ApplyResources(Me.m_tvGroups, "m_tvGroups")
            Me.m_tvGroups.BackColor = System.Drawing.SystemColors.Window
            Me.m_tvGroups.HideSelection = False
            Me.m_tvGroups.Name = "m_tvGroups"
            '
            'SplitContainer1
            '
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_tvGroups)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_lblPt)
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_btnRun)
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_plot)
            '
            'm_lblPt
            '
            resources.ApplyResources(Me.m_lblPt, "m_lblPt")
            Me.m_lblPt.Name = "m_lblPt"
            '
            'm_btnRun
            '
            resources.ApplyResources(Me.m_btnRun, "m_btnRun")
            Me.m_btnRun.Name = "m_btnRun"
            Me.m_btnRun.UseVisualStyleBackColor = True
            '
            'SRplot
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "SRplot"
            Me.TabText = "S/R plot"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents m_plot As ZedGraph.ZedGraphControl
        Private WithEvents m_tvGroups As System.Windows.Forms.TreeView
        Private WithEvents m_btnRun As System.Windows.Forms.Button
        Private WithEvents m_lblPt As System.Windows.Forms.Label
    End Class
End Namespace

