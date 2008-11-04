Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorMigration
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
            Me.m_lbMonth = New System.Windows.Forms.Label
            Me.m_cmbMonth = New System.Windows.Forms.ComboBox
            Me.m_chkAutoRotate = New System.Windows.Forms.CheckBox
            Me.m_lblGroup = New System.Windows.Forms.Label
            Me.m_cmbGroup = New System.Windows.Forms.ComboBox
            Me.SuspendLayout()
            '
            'm_lbMonth
            '
            Me.m_lbMonth.AutoSize = True
            Me.m_lbMonth.Location = New System.Drawing.Point(4, 52)
            Me.m_lbMonth.Name = "m_lbMonth"
            Me.m_lbMonth.Size = New System.Drawing.Size(40, 13)
            Me.m_lbMonth.TabIndex = 3
            Me.m_lbMonth.Text = "&Month:"
            '
            'm_cmbMonth
            '
            Me.m_cmbMonth.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbMonth.FormattingEnabled = True
            Me.m_cmbMonth.Items.AddRange(New Object() {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"})
            Me.m_cmbMonth.Location = New System.Drawing.Point(49, 49)
            Me.m_cmbMonth.MaxDropDownItems = 12
            Me.m_cmbMonth.Name = "m_cmbMonth"
            Me.m_cmbMonth.Size = New System.Drawing.Size(151, 21)
            Me.m_cmbMonth.TabIndex = 4
            '
            'm_chkAutoRotate
            '
            Me.m_chkAutoRotate.AutoSize = True
            Me.m_chkAutoRotate.Location = New System.Drawing.Point(49, 76)
            Me.m_chkAutoRotate.Name = "m_chkAutoRotate"
            Me.m_chkAutoRotate.Size = New System.Drawing.Size(110, 17)
            Me.m_chkAutoRotate.TabIndex = 5
            Me.m_chkAutoRotate.Text = "&Auto-rotate month"
            Me.m_chkAutoRotate.UseVisualStyleBackColor = True
            Me.m_chkAutoRotate.Checked = True
            '
            'm_lblGroup
            '
            Me.m_lblGroup.AutoSize = True
            Me.m_lblGroup.Location = New System.Drawing.Point(4, 24)
            Me.m_lblGroup.Name = "m_lblGroup"
            Me.m_lblGroup.Size = New System.Drawing.Size(39, 13)
            Me.m_lblGroup.TabIndex = 1
            Me.m_lblGroup.Text = "&Group:"
            '
            'm_cmbGroup
            '
            Me.m_cmbGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGroup.FormattingEnabled = True
            Me.m_cmbGroup.Items.AddRange(New Object() {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"})
            Me.m_cmbGroup.Location = New System.Drawing.Point(49, 21)
            Me.m_cmbGroup.MaxDropDownItems = 12
            Me.m_cmbGroup.Name = "m_cmbGroup"
            Me.m_cmbGroup.Size = New System.Drawing.Size(151, 21)
            Me.m_cmbGroup.TabIndex = 2
            '
            'ucLayerEditorMigration
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cmbMonth)
            Me.Controls.Add(Me.m_chkAutoRotate)
            Me.Controls.Add(Me.m_cmbGroup)
            Me.Controls.Add(Me.m_lbMonth)
            Me.Controls.Add(Me.m_lblGroup)
            Me.Name = "ucLayerEditorMigration"
            Me.Size = New System.Drawing.Size(200, 97)
            Me.Controls.SetChildIndex(Me.m_lblGroup, 0)
            Me.Controls.SetChildIndex(Me.m_lbMonth, 0)
            Me.Controls.SetChildIndex(Me.m_cmbGroup, 0)
            Me.Controls.SetChildIndex(Me.m_chkAutoRotate, 0)
            Me.Controls.SetChildIndex(Me.m_cmbMonth, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lbMonth As System.Windows.Forms.Label
        Friend WithEvents m_cmbMonth As System.Windows.Forms.ComboBox
        Friend WithEvents m_chkAutoRotate As System.Windows.Forms.CheckBox
        Private WithEvents m_lblGroup As System.Windows.Forms.Label
        Friend WithEvents m_cmbGroup As System.Windows.Forms.ComboBox

    End Class

End Namespace
