Namespace Ecopath.Controls.FlowDiagram

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Public Class FlowDiagramSettings
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FlowDiagramSettings))
            Me.FDPropertyGrid = New System.Windows.Forms.PropertyGrid
            Me.btnOk = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'FDPropertyGrid
            '
            resources.ApplyResources(Me.FDPropertyGrid, "FDPropertyGrid")
            Me.FDPropertyGrid.Name = "FDPropertyGrid"
            '
            'btnOk
            '
            resources.ApplyResources(Me.btnOk, "btnOk")
            Me.btnOk.Name = "btnOk"
            '
            'FlowDiagramSettings
            '
            Me.AcceptButton = Me.btnOk
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.btnOk)
            Me.Controls.Add(Me.FDPropertyGrid)
            Me.Name = "FlowDiagramSettings"
            Me.TopMost = True
            Me.ResumeLayout(False)

        End Sub
        Public WithEvents FDPropertyGrid As System.Windows.Forms.PropertyGrid
        Friend WithEvents btnOk As System.Windows.Forms.Button
    End Class
End Namespace