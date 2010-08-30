Namespace Ecospace.Advection

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucMap
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
            Me.m_zoomctrl = New ScientificInterface.Ecospace.ucMapZoom
            Me.m_hdrTitle = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.SuspendLayout()
            '
            'm_zoomctrl
            '
            Me.m_zoomctrl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_zoomctrl.BackColor = System.Drawing.Color.White
            Me.m_zoomctrl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_zoomctrl.Location = New System.Drawing.Point(0, 18)
            Me.m_zoomctrl.Margin = New System.Windows.Forms.Padding(0)
            Me.m_zoomctrl.Name = "m_zoomctrl"
            Me.m_zoomctrl.Size = New System.Drawing.Size(360, 383)
            Me.m_zoomctrl.TabIndex = 0
            '
            'm_hdrTitle
            '
            Me.m_hdrTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrTitle.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrTitle.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrTitle.Name = "m_hdrTitle"
            Me.m_hdrTitle.Size = New System.Drawing.Size(360, 18)
            Me.m_hdrTitle.TabIndex = 1
            Me.m_hdrTitle.Text = "Map"
            Me.m_hdrTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'ucMap
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_hdrTitle)
            Me.Controls.Add(Me.m_zoomctrl)
            Me.Name = "ucMap"
            Me.Size = New System.Drawing.Size(360, 401)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_zoomctrl As ScientificInterface.Ecospace.ucMapZoom
        Private WithEvents m_hdrTitle As ScientificInterfaceShared.Controls.cEwEHeaderLabel

    End Class

End Namespace