Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorFleet
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
            Me.m_lblFleet = New System.Windows.Forms.Label
            Me.m_cmbFleet = New System.Windows.Forms.ComboBox
            Me.SuspendLayout()
            '
            'm_lblFleet
            '
            Me.m_lblFleet.AutoSize = True
            Me.m_lblFleet.Location = New System.Drawing.Point(4, 24)
            Me.m_lblFleet.Name = "m_lblFleet"
            Me.m_lblFleet.Size = New System.Drawing.Size(33, 13)
            Me.m_lblFleet.TabIndex = 1
            Me.m_lblFleet.Text = "&Fleet:"
            '
            'm_cmbFleet
            '
            Me.m_cmbFleet.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbFleet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbFleet.FormattingEnabled = True
            Me.m_cmbFleet.Items.AddRange(New Object() {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"})
            Me.m_cmbFleet.Location = New System.Drawing.Point(49, 21)
            Me.m_cmbFleet.MaxDropDownItems = 12
            Me.m_cmbFleet.Name = "m_cmbFleet"
            Me.m_cmbFleet.Size = New System.Drawing.Size(151, 21)
            Me.m_cmbFleet.TabIndex = 2
            '
            'ucLayerEditorPort
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cmbFleet)
            Me.Controls.Add(Me.m_lblFleet)
            Me.Name = "ucLayerEditorPort"
            Me.Size = New System.Drawing.Size(200, 59)
            Me.Controls.SetChildIndex(Me.m_lblFleet, 0)
            Me.Controls.SetChildIndex(Me.m_cmbFleet, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblFleet As System.Windows.Forms.Label
        Private WithEvents m_cmbFleet As System.Windows.Forms.ComboBox

    End Class

End Namespace
