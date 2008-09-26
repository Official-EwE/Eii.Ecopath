Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcosimOutputLineGraph
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
            Me.ttLineGraph = New System.Windows.Forms.ToolTip(Me.components)
            Me.lblGrpName = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'lblGrpName
            '
            Me.lblGrpName.AutoSize = True
            Me.lblGrpName.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblGrpName.Location = New System.Drawing.Point(32, 24)
            Me.lblGrpName.Name = "lblGrpName"
            Me.lblGrpName.Size = New System.Drawing.Size(49, 16)
            Me.lblGrpName.TabIndex = 0
            Me.lblGrpName.Text = "Label1"
            Me.lblGrpName.Visible = False
            '
            'LineGraph
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.lblGrpName)
            Me.Name = "LineGraph"
            Me.Size = New System.Drawing.Size(706, 471)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ttLineGraph As System.Windows.Forms.ToolTip
        Friend WithEvents lblGrpName As System.Windows.Forms.Label

    End Class

End Namespace
