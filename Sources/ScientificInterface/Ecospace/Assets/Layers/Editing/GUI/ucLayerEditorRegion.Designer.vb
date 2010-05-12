Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorRegion
        Inherits ucLayerEditorDefault

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorRegion))
            Me.m_lbRegion = New System.Windows.Forms.Label
            Me.m_cmbRegion = New System.Windows.Forms.ComboBox
            Me.SuspendLayout()
            '
            'm_lbRegion
            '
            resources.ApplyResources(Me.m_lbRegion, "m_lbRegion")
            Me.m_lbRegion.Name = "m_lbRegion"
            '
            'm_cmbRegion
            '
            resources.ApplyResources(Me.m_cmbRegion, "m_cmbRegion")
            Me.m_cmbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbRegion.FormattingEnabled = True
            Me.m_cmbRegion.Name = "m_cmbRegion"
            '
            'ucLayerEditorRegion
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cmbRegion)
            Me.Controls.Add(Me.m_lbRegion)
            Me.Name = "ucLayerEditorRegion"
            Me.Controls.SetChildIndex(Me.m_lbRegion, 0)
            Me.Controls.SetChildIndex(Me.m_cmbRegion, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lbRegion As System.Windows.Forms.Label
        Private WithEvents m_cmbRegion As System.Windows.Forms.ComboBox

    End Class

End Namespace
