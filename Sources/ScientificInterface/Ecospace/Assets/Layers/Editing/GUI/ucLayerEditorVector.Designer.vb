Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorVector
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
            Me.m_lblScale = New System.Windows.Forms.Label
            Me.m_nudValue = New System.Windows.Forms.NumericUpDown
            Me.m_lblUnits = New System.Windows.Forms.Label
            CType(Me.m_nudValue, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblScale
            '
            Me.m_lblScale.AutoSize = True
            Me.m_lblScale.Location = New System.Drawing.Point(3, 44)
            Me.m_lblScale.Name = "m_lblScale"
            Me.m_lblScale.Size = New System.Drawing.Size(37, 13)
            Me.m_lblScale.TabIndex = 2
            Me.m_lblScale.Text = "&Scale:"
            '
            'm_nudValue
            '
            Me.m_nudValue.Location = New System.Drawing.Point(46, 42)
            Me.m_nudValue.Name = "m_nudValue"
            Me.m_nudValue.Size = New System.Drawing.Size(85, 20)
            Me.m_nudValue.TabIndex = 3
            '
            'm_lblUnits
            '
            Me.m_lblUnits.AutoSize = True
            Me.m_lblUnits.Location = New System.Drawing.Point(137, 44)
            Me.m_lblUnits.Name = "m_lblUnits"
            Me.m_lblUnits.Size = New System.Drawing.Size(41, 13)
            Me.m_lblUnits.TabIndex = 4
            Me.m_lblUnits.Text = "<units>"
            Me.m_lblUnits.Visible = False
            '
            'ucLayerEditorVector
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_nudValue)
            Me.Controls.Add(Me.m_lblScale)
            Me.Controls.Add(Me.m_lblUnits)
            Me.Name = "ucLayerEditorVector"
            Me.Size = New System.Drawing.Size(200, 69)
            Me.Controls.SetChildIndex(Me.m_lblUnits, 0)
            Me.Controls.SetChildIndex(Me.m_lblScale, 0)
            Me.Controls.SetChildIndex(Me.m_nudValue, 0)
            CType(Me.m_nudValue, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_nudValue As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblUnits As System.Windows.Forms.Label
        Private WithEvents m_lblScale As System.Windows.Forms.Label

    End Class

End Namespace
