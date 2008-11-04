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
            Me.m_lbImage = New System.Windows.Forms.Label
            Me.m_lbCaption = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'm_lbImage
            '
            Me.m_lbImage.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lbImage.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lbImage.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lbImage.Image = Global.ScientificInterface.My.Resources.Resources.Editable
            Me.m_lbImage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.m_lbImage.Location = New System.Drawing.Point(0, 0)
            Me.m_lbImage.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lbImage.Name = "m_lbImage"
            Me.m_lbImage.Size = New System.Drawing.Size(21, 18)
            Me.m_lbImage.TabIndex = 0
            Me.m_lbImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'm_lbCaption
            '
            Me.m_lbCaption.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lbCaption.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lbCaption.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lbCaption.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lbCaption.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.m_lbCaption.Location = New System.Drawing.Point(21, 0)
            Me.m_lbCaption.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lbCaption.Name = "m_lbCaption"
            Me.m_lbCaption.Size = New System.Drawing.Size(179, 18)
            Me.m_lbCaption.TabIndex = 0
            Me.m_lbCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'ucLayerEditor
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lbCaption)
            Me.Controls.Add(Me.m_lbImage)
            Me.Name = "ucLayerEditor"
            Me.Size = New System.Drawing.Size(200, 19)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_lbImage As System.Windows.Forms.Label
        Private WithEvents m_lbCaption As System.Windows.Forms.Label

    End Class

End Namespace
