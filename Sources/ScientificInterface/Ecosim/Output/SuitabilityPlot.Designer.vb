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
            Me.m_plot = New ScientificInterface.ucSuitabilityPlot
            Me.SuspendLayout()
            '
            'm_plot
            '
            Me.m_plot.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plot.Location = New System.Drawing.Point(0, 0)
            Me.m_plot.Name = "m_plot"
            Me.m_plot.Size = New System.Drawing.Size(416, 266)
            Me.m_plot.TabIndex = 0
            '
            'SuitabilityPlot
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(416, 266)
            Me.Controls.Add(Me.m_plot)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "SuitabilityPlot"
            Me.Text = "SuitabilityPlot"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_plot As ScientificInterface.ucSuitabilityPlot
    End Class

End Namespace
