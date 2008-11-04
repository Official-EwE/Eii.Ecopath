Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorAdvection
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
            Me.m_lblAngle = New System.Windows.Forms.Label
            Me.m_lblVelocity = New System.Windows.Forms.Label
            Me.m_nudAngle = New System.Windows.Forms.NumericUpDown
            Me.m_nudVelocity = New System.Windows.Forms.NumericUpDown
            Me.m_pbSample = New System.Windows.Forms.PictureBox
            CType(Me.m_nudAngle, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudVelocity, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbSample, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblAngle
            '
            Me.m_lblAngle.AutoSize = True
            Me.m_lblAngle.Location = New System.Drawing.Point(3, 28)
            Me.m_lblAngle.Name = "m_lblAngle"
            Me.m_lblAngle.Size = New System.Drawing.Size(37, 13)
            Me.m_lblAngle.TabIndex = 0
            Me.m_lblAngle.Text = "&Angle:"
            '
            'm_lblVelocity
            '
            Me.m_lblVelocity.AutoSize = True
            Me.m_lblVelocity.Location = New System.Drawing.Point(3, 54)
            Me.m_lblVelocity.Name = "m_lblVelocity"
            Me.m_lblVelocity.Size = New System.Drawing.Size(47, 13)
            Me.m_lblVelocity.TabIndex = 2
            Me.m_lblVelocity.Text = "&Velocity:"
            '
            'm_nudAngle
            '
            Me.m_nudAngle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudAngle.Location = New System.Drawing.Point(56, 26)
            Me.m_nudAngle.Maximum = New Decimal(New Integer() {359, 0, 0, 0})
            Me.m_nudAngle.Name = "m_nudAngle"
            Me.m_nudAngle.Size = New System.Drawing.Size(89, 20)
            Me.m_nudAngle.TabIndex = 1
            '
            'm_nudVelocity
            '
            Me.m_nudVelocity.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudVelocity.Location = New System.Drawing.Point(56, 52)
            Me.m_nudVelocity.Name = "m_nudVelocity"
            Me.m_nudVelocity.Size = New System.Drawing.Size(89, 20)
            Me.m_nudVelocity.TabIndex = 3
            '
            'm_pbSample
            '
            Me.m_pbSample.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pbSample.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbSample.Location = New System.Drawing.Point(151, 26)
            Me.m_pbSample.Name = "m_pbSample"
            Me.m_pbSample.Size = New System.Drawing.Size(46, 46)
            Me.m_pbSample.TabIndex = 5
            Me.m_pbSample.TabStop = False
            '
            'ucLayerEditorAdvection
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblAngle)
            Me.Controls.Add(Me.m_nudVelocity)
            Me.Controls.Add(Me.m_nudAngle)
            Me.Controls.Add(Me.m_lblVelocity)
            Me.Controls.Add(Me.m_pbSample)
            Me.Name = "ucLayerEditorAdvection"
            Me.Size = New System.Drawing.Size(200, 76)
            Me.Controls.SetChildIndex(Me.m_pbSample, 0)
            Me.Controls.SetChildIndex(Me.m_lblVelocity, 0)
            Me.Controls.SetChildIndex(Me.m_nudAngle, 0)
            Me.Controls.SetChildIndex(Me.m_nudVelocity, 0)
            Me.Controls.SetChildIndex(Me.m_lblAngle, 0)
            CType(Me.m_nudAngle, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudVelocity, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbSample, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_nudAngle As System.Windows.Forms.NumericUpDown
        Private WithEvents m_pbSample As System.Windows.Forms.PictureBox
        Private WithEvents m_lblAngle As System.Windows.Forms.Label
        Private WithEvents m_lblVelocity As System.Windows.Forms.Label
        Private WithEvents m_nudVelocity As System.Windows.Forms.NumericUpDown

    End Class

End Namespace
