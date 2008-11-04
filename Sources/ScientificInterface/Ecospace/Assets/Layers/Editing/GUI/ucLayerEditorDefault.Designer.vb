Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorDefault
        Inherits ucLayerEditor

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
            Me.m_ucSlider = New ScientificInterfaceShared.Controls.ucSlider
            Me.SuspendLayout()
            '
            'm_ucSlider
            '
            Me.m_ucSlider.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_ucSlider.Location = New System.Drawing.Point(46, 19)
            Me.m_ucSlider.Margin = New System.Windows.Forms.Padding(0)
            Me.m_ucSlider.Maximum = 6
            Me.m_ucSlider.Minimum = 1
            Me.m_ucSlider.Name = "m_ucSlider"
            Me.m_ucSlider.Size = New System.Drawing.Size(154, 20)
            Me.m_ucSlider.TabIndex = 6
            Me.m_ucSlider.Value = 1
            '
            'ucLayerEditorDefault
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_ucSlider)
            Me.Name = "ucLayerEditorDefault"
            Me.Size = New System.Drawing.Size(200, 42)
            Me.Controls.SetChildIndex(Me.m_ucSlider, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_ucSlider As ScientificInterfaceShared.Controls.ucSlider

    End Class

End Namespace