Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

Namespace Ecopath.Output

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class PSDContributionPlot
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
            Me.UcGrowthPlotzgc1 = New ScientificInterface.ucPSDPlotzgc
            Me.SuspendLayout()
            '
            'UcGrowthPlotzgc1
            '
            Me.UcGrowthPlotzgc1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.UcGrowthPlotzgc1.Location = New System.Drawing.Point(0, 0)
            Me.UcGrowthPlotzgc1.Name = "UcGrowthPlotzgc1"
            Me.UcGrowthPlotzgc1.Size = New System.Drawing.Size(534, 340)
            Me.UcGrowthPlotzgc1.TabIndex = 0
            '
            'PSDContributionPlot
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(534, 340)
            Me.Controls.Add(Me.UcGrowthPlotzgc1)
            Me.Name = "PSDContributionPlot"
            Me.Text = "PSDContributionPlot"
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents UcGrowthPlotzgc1 As ScientificInterface.ucPSDPlotzgc
    End Class

End Namespace
