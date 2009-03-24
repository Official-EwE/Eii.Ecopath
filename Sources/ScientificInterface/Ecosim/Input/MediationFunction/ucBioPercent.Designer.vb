Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucBioPercent
        Inherits System.Windows.Forms.UserControl

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucBioPercent))
            Me.m_zedgraph = New ZedGraph.ZedGraphControl
            Me.SuspendLayout()
            '
            'zgBP
            '
            resources.ApplyResources(Me.m_zedgraph, "zgBP")
            Me.m_zedgraph.Name = "zgBP"
            Me.m_zedgraph.ScrollGrace = 0
            Me.m_zedgraph.ScrollMaxX = 0
            Me.m_zedgraph.ScrollMaxY = 0
            Me.m_zedgraph.ScrollMaxY2 = 0
            Me.m_zedgraph.ScrollMinX = 0
            Me.m_zedgraph.ScrollMinY = 0
            Me.m_zedgraph.ScrollMinY2 = 0
            '
            'ucBioPercent
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_zedgraph)
            Me.Name = "ucBioPercent"
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_zedgraph As ZedGraph.ZedGraphControl

    End Class

End Namespace



