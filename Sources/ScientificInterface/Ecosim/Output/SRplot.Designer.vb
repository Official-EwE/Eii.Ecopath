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
            Me.zgSRPlot = New ZedGraph.ZedGraphControl
            Me.tvGroups = New System.Windows.Forms.TreeView
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.lblPt = New System.Windows.Forms.Label
            Me.btnRun = New System.Windows.Forms.Button
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.SuspendLayout()
            '
            'zgSRPlot
            '
            resources.ApplyResources(Me.zgSRPlot, "zgSRPlot")
            Me.zgSRPlot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.zgSRPlot.Name = "zgSRPlot"
            Me.zgSRPlot.ScrollGrace = 0
            Me.zgSRPlot.ScrollMaxX = 0
            Me.zgSRPlot.ScrollMaxY = 0
            Me.zgSRPlot.ScrollMaxY2 = 0
            Me.zgSRPlot.ScrollMinX = 0
            Me.zgSRPlot.ScrollMinY = 0
            Me.zgSRPlot.ScrollMinY2 = 0
            '
            'tvGroups
            '
            resources.ApplyResources(Me.tvGroups, "tvGroups")
            Me.tvGroups.BackColor = System.Drawing.SystemColors.Window
            Me.tvGroups.HideSelection = False
            Me.tvGroups.Name = "tvGroups"
            '
            'SplitContainer1
            '
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.tvGroups)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.lblPt)
            Me.SplitContainer1.Panel2.Controls.Add(Me.btnRun)
            Me.SplitContainer1.Panel2.Controls.Add(Me.zgSRPlot)
            '
            'lblPt
            '
            resources.ApplyResources(Me.lblPt, "lblPt")
            Me.lblPt.Name = "lblPt"
            '
            'btnRun
            '
            resources.ApplyResources(Me.btnRun, "btnRun")
            Me.btnRun.Name = "btnRun"
            Me.btnRun.UseVisualStyleBackColor = True
            '
            'SRplot
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "SRplot"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents zgSRPlot As ZedGraph.ZedGraphControl
        Friend WithEvents tvGroups As System.Windows.Forms.TreeView
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents btnRun As System.Windows.Forms.Button
        Friend WithEvents lblPt As System.Windows.Forms.Label
    End Class
End Namespace

