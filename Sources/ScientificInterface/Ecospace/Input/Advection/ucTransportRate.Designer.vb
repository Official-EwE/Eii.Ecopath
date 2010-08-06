Namespace Ecospace.Advection

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucTransportRate
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
            Me.m_map = New ScientificInterface.Ecospace.ucBaseMap
            Me.m_btnReset = New System.Windows.Forms.Button
            Me.m_lblRate = New System.Windows.Forms.Label
            Me.m_nudRate = New System.Windows.Forms.NumericUpDown
            CType(Me.m_nudRate, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_map
            '
            Me.m_map.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_map.BackColor = System.Drawing.Color.White
            Me.m_map.Basemap = Nothing
            Me.m_map.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_map.Editable = False
            Me.m_map.Location = New System.Drawing.Point(3, 32)
            Me.m_map.Name = "m_map"
            Me.m_map.Size = New System.Drawing.Size(354, 366)
            Me.m_map.TabIndex = 0
            '
            'm_btnReset
            '
            Me.m_btnReset.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.m_btnReset.Location = New System.Drawing.Point(282, 3)
            Me.m_btnReset.Name = "m_btnReset"
            Me.m_btnReset.Size = New System.Drawing.Size(75, 23)
            Me.m_btnReset.TabIndex = 1
            Me.m_btnReset.Text = "&Reset"
            Me.m_btnReset.UseVisualStyleBackColor = True
            '
            'm_lblRate
            '
            Me.m_lblRate.AutoSize = True
            Me.m_lblRate.Location = New System.Drawing.Point(3, 8)
            Me.m_lblRate.Name = "m_lblRate"
            Me.m_lblRate.Size = New System.Drawing.Size(76, 13)
            Me.m_lblRate.TabIndex = 2
            Me.m_lblRate.Text = "&Transport rate:"
            '
            'm_nudRate
            '
            Me.m_nudRate.Location = New System.Drawing.Point(85, 6)
            Me.m_nudRate.Name = "m_nudRate"
            Me.m_nudRate.Size = New System.Drawing.Size(73, 20)
            Me.m_nudRate.TabIndex = 3
            '
            'ucTransportRate
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_nudRate)
            Me.Controls.Add(Me.m_lblRate)
            Me.Controls.Add(Me.m_btnReset)
            Me.Controls.Add(Me.m_map)
            Me.Name = "ucTransportRate"
            Me.Size = New System.Drawing.Size(360, 401)
            CType(Me.m_nudRate, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_map As ScientificInterface.Ecospace.ucBaseMap
        Private WithEvents m_btnReset As System.Windows.Forms.Button
        Private WithEvents m_lblRate As System.Windows.Forms.Label
        Private WithEvents m_nudRate As System.Windows.Forms.NumericUpDown

    End Class

End Namespace