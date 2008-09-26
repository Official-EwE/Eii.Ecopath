<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcotrophGraph
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcotrophGraph))
        Me.zgZedGraph = New ZedGraph.ZedGraphControl
        Me.SuspendLayout()
        '
        'zgZedGraph
        '
        resources.ApplyResources(Me.zgZedGraph, "zgZedGraph")
        Me.zgZedGraph.Name = "zgZedGraph"
        Me.zgZedGraph.ScrollGrace = 0
        Me.zgZedGraph.ScrollMaxX = 0
        Me.zgZedGraph.ScrollMaxY = 0
        Me.zgZedGraph.ScrollMaxY2 = 0
        Me.zgZedGraph.ScrollMinX = 0
        Me.zgZedGraph.ScrollMinY = 0
        Me.zgZedGraph.ScrollMinY2 = 0
        '
        'frmEcotrophGraph
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.zgZedGraph)
        Me.Name = "frmEcotrophGraph"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents zgZedGraph As ZedGraph.ZedGraphControl
End Class
