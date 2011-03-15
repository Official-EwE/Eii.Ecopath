Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorRange
        Inherits ucLayerEditorDefault

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorRange))
            Me.m_lbValue = New System.Windows.Forms.Label
            Me.m_nudValue = New System.Windows.Forms.NumericUpDown
            Me.m_pbPreview = New System.Windows.Forms.PictureBox
            CType(Me.m_nudValue, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbPreview, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lbValue
            '
            resources.ApplyResources(Me.m_lbValue, "m_lbValue")
            Me.m_lbValue.Name = "m_lbValue"
            '
            'm_nudValue
            '
            resources.ApplyResources(Me.m_nudValue, "m_nudValue")
            Me.m_nudValue.Name = "m_nudValue"
            '
            'm_pbPreview
            '
            resources.ApplyResources(Me.m_pbPreview, "m_pbPreview")
            Me.m_pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbPreview.Name = "m_pbPreview"
            Me.m_pbPreview.TabStop = False
            '
            'ucLayerEditorRange
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_pbPreview)
            Me.Controls.Add(Me.m_nudValue)
            Me.Controls.Add(Me.m_lbValue)
            Me.Name = "ucLayerEditorRange"
            Me.Controls.SetChildIndex(Me.m_lbValue, 0)
            Me.Controls.SetChildIndex(Me.m_nudValue, 0)
            Me.Controls.SetChildIndex(Me.m_pbPreview, 0)
            CType(Me.m_nudValue, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbPreview, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lbValue As System.Windows.Forms.Label
        Private WithEvents m_nudValue As System.Windows.Forms.NumericUpDown
        Protected WithEvents m_pbPreview As System.Windows.Forms.PictureBox

    End Class

End Namespace
