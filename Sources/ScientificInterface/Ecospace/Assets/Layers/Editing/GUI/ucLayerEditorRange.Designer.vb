Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorRange
        Inherits ucLayerEditorDefault

        'UserControl overrides dispose to clean up the component list.
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
            Me.m_lbValue = New System.Windows.Forms.Label
            Me.m_nudValue = New System.Windows.Forms.NumericUpDown
            CType(Me.m_nudValue, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lbValue
            '
            Me.m_lbValue.AutoSize = True
            Me.m_lbValue.Location = New System.Drawing.Point(3, 44)
            Me.m_lbValue.Name = "m_lbValue"
            Me.m_lbValue.Size = New System.Drawing.Size(37, 13)
            Me.m_lbValue.TabIndex = 2
            Me.m_lbValue.Text = "Value:"
            '
            'm_nudValue
            '
            Me.m_nudValue.Location = New System.Drawing.Point(46, 42)
            Me.m_nudValue.Name = "m_nudValue"
            Me.m_nudValue.Size = New System.Drawing.Size(86, 20)
            Me.m_nudValue.TabIndex = 3
            '
            'ucLayerEditorRange
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_nudValue)
            Me.Controls.Add(Me.m_lbValue)
            Me.Name = "ucLayerEditorRange"
            Me.Size = New System.Drawing.Size(200, 65)
            Me.Controls.SetChildIndex(Me.m_lbValue, 0)
            Me.Controls.SetChildIndex(Me.m_nudValue, 0)
            CType(Me.m_nudValue, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lbValue As System.Windows.Forms.Label
        Private WithEvents m_nudValue As System.Windows.Forms.NumericUpDown

    End Class

End Namespace
