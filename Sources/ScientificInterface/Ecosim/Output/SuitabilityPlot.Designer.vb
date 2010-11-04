Imports ScientificInterfaceShared.Forms

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated(), CLSCompliant(False)> _
Partial Class SuitabilityPlot
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SuitabilityPlot))
            Me.m_plot = New ScientificInterface.ucSuitabilityPlot
            Me.SuspendLayout()
            '
            'm_plot
            '
            resources.ApplyResources(Me.m_plot, "m_plot")
            Me.m_plot.Name = "m_plot"
            Me.m_plot.UIContext = Nothing
            '
            'SuitabilityPlot
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ControlBox = False
            Me.Controls.Add(Me.m_plot)
            Me.Name = "SuitabilityPlot"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_plot As ScientificInterface.ucSuitabilityPlot
    End Class

End Namespace
