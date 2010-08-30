Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerGroup
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerGroup))
            Me.fpItems = New System.Windows.Forms.FlowLayoutPanel
            Me.SuspendLayout()
            '
            'fpItems
            '
            resources.ApplyResources(Me.fpItems, "fpItems")
            Me.fpItems.Name = "fpItems"
            '
            'ucLayerGroup
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.Controls.Add(Me.fpItems)
            Me.Name = "ucLayerGroup"
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents fpItems As System.Windows.Forms.FlowLayoutPanel

    End Class
End Namespace