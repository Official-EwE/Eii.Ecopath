Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditor
        Inherits System.Windows.Forms.UserControl

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditor))
            Me.m_lbImage = New System.Windows.Forms.Label
            Me.m_lbCaption = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'm_lbImage
            '
            Me.m_lbImage.BackColor = System.Drawing.SystemColors.ControlDark
            resources.ApplyResources(Me.m_lbImage, "m_lbImage")
            Me.m_lbImage.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lbImage.Image = Global.ScientificInterface.My.Resources.Resources.Editable
            Me.m_lbImage.Name = "m_lbImage"
            '
            'm_lbCaption
            '
            resources.ApplyResources(Me.m_lbCaption, "m_lbCaption")
            Me.m_lbCaption.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lbCaption.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lbCaption.Name = "m_lbCaption"
            '
            'ucLayerEditor
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lbCaption)
            Me.Controls.Add(Me.m_lbImage)
            Me.Name = "ucLayerEditor"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_lbImage As System.Windows.Forms.Label
        Private WithEvents m_lbCaption As System.Windows.Forms.Label

    End Class

End Namespace
