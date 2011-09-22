Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorGroup
        Inherits ucLayerEditor

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorGroup))
            Me.m_lblFleet = New System.Windows.Forms.Label()
            Me.m_cmbGroup = New System.Windows.Forms.ComboBox()
            Me.SuspendLayout()
            '
            'm_lblFleet
            '
            resources.ApplyResources(Me.m_lblFleet, "m_lblFleet")
            Me.m_lblFleet.Name = "m_lblFleet"
            '
            'm_cmbGroup
            '
            resources.ApplyResources(Me.m_cmbGroup, "m_cmbGroup")
            Me.m_cmbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGroup.FormattingEnabled = True
            Me.m_cmbGroup.Name = "m_cmbGroup"
            '
            'ucLayerEditorGroup
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cmbGroup)
            Me.Controls.Add(Me.m_lblFleet)
            Me.Name = "ucLayerEditorGroup"
            Me.Controls.SetChildIndex(Me.m_lblFleet, 0)
            Me.Controls.SetChildIndex(Me.m_cmbGroup, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblFleet As System.Windows.Forms.Label
        Private WithEvents m_cmbGroup As System.Windows.Forms.ComboBox

    End Class

End Namespace
