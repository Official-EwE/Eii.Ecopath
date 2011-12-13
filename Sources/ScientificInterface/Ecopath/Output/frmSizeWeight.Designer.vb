Imports ScientificInterfaceShared.Forms

Namespace Ecopath.Output

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class SizeWeightPlot
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SizeWeightPlot))
            Me.zgcZedGraphCntl = New ZedGraph.ZedGraphControl
            Me.SuspendLayout()
            '
            'zgcZedGraphCntl
            '
            resources.ApplyResources(Me.zgcZedGraphCntl, "zgcZedGraphCntl")
            Me.zgcZedGraphCntl.Name = "zgcZedGraphCntl"
            Me.zgcZedGraphCntl.ScrollGrace = 0
            Me.zgcZedGraphCntl.ScrollMaxX = 0
            Me.zgcZedGraphCntl.ScrollMaxY = 0
            Me.zgcZedGraphCntl.ScrollMaxY2 = 0
            Me.zgcZedGraphCntl.ScrollMinX = 0
            Me.zgcZedGraphCntl.ScrollMinY = 0
            Me.zgcZedGraphCntl.ScrollMinY2 = 0
            '
            'SizeWeightPlot
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.zgcZedGraphCntl)
            Me.Name = "SizeWeightPlot"
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents zgcZedGraphCntl As ZedGraph.ZedGraphControl
    End Class

End Namespace
