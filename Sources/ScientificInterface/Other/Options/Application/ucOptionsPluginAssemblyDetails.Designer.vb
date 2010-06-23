<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsPluginAssemblyDetails
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
        Me.m_lblCopyright = New System.Windows.Forms.Label
        Me.m_lblCompany = New System.Windows.Forms.Label
        Me.m_lblVersion = New System.Windows.Forms.Label
        Me.m_lblFile = New System.Windows.Forms.Label
        Me.m_tbFile = New System.Windows.Forms.TextBox
        Me.m_tbVersion = New System.Windows.Forms.TextBox
        Me.m_tbCompany = New System.Windows.Forms.TextBox
        Me.m_tbCopyright = New System.Windows.Forms.TextBox
        Me.m_tbDescription = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'm_lblCopyright
        '
        Me.m_lblCopyright.AutoSize = True
        Me.m_lblCopyright.Location = New System.Drawing.Point(3, 60)
        Me.m_lblCopyright.Name = "m_lblCopyright"
        Me.m_lblCopyright.Size = New System.Drawing.Size(54, 13)
        Me.m_lblCopyright.TabIndex = 6
        Me.m_lblCopyright.Text = "Copyright:"
        '
        'm_lblCompany
        '
        Me.m_lblCompany.AutoSize = True
        Me.m_lblCompany.Location = New System.Drawing.Point(3, 41)
        Me.m_lblCompany.Name = "m_lblCompany"
        Me.m_lblCompany.Size = New System.Drawing.Size(54, 13)
        Me.m_lblCompany.TabIndex = 4
        Me.m_lblCompany.Text = "Company:"
        '
        'm_lblVersion
        '
        Me.m_lblVersion.AutoSize = True
        Me.m_lblVersion.Location = New System.Drawing.Point(3, 22)
        Me.m_lblVersion.Name = "m_lblVersion"
        Me.m_lblVersion.Size = New System.Drawing.Size(45, 13)
        Me.m_lblVersion.TabIndex = 2
        Me.m_lblVersion.Text = "Version:"
        '
        'm_lblFile
        '
        Me.m_lblFile.AutoSize = True
        Me.m_lblFile.Location = New System.Drawing.Point(3, 3)
        Me.m_lblFile.Name = "m_lblFile"
        Me.m_lblFile.Size = New System.Drawing.Size(26, 13)
        Me.m_lblFile.TabIndex = 0
        Me.m_lblFile.Text = "File:"
        '
        'm_tbFile
        '
        Me.m_tbFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbFile.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbFile.Location = New System.Drawing.Point(66, 3)
        Me.m_tbFile.Name = "m_tbFile"
        Me.m_tbFile.ReadOnly = True
        Me.m_tbFile.Size = New System.Drawing.Size(374, 13)
        Me.m_tbFile.TabIndex = 1
        '
        'm_tbVersion
        '
        Me.m_tbVersion.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbVersion.Location = New System.Drawing.Point(66, 22)
        Me.m_tbVersion.Name = "m_tbVersion"
        Me.m_tbVersion.ReadOnly = True
        Me.m_tbVersion.Size = New System.Drawing.Size(93, 13)
        Me.m_tbVersion.TabIndex = 3
        Me.m_tbVersion.Text = "1.2.3.4"
        '
        'm_tbCompany
        '
        Me.m_tbCompany.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbCompany.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbCompany.Location = New System.Drawing.Point(66, 41)
        Me.m_tbCompany.Name = "m_tbCompany"
        Me.m_tbCompany.ReadOnly = True
        Me.m_tbCompany.Size = New System.Drawing.Size(374, 13)
        Me.m_tbCompany.TabIndex = 5
        '
        'm_tbCopyright
        '
        Me.m_tbCopyright.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbCopyright.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbCopyright.Location = New System.Drawing.Point(66, 60)
        Me.m_tbCopyright.Name = "m_tbCopyright"
        Me.m_tbCopyright.ReadOnly = True
        Me.m_tbCopyright.Size = New System.Drawing.Size(374, 13)
        Me.m_tbCopyright.TabIndex = 7
        '
        'm_tbDescription
        '
        Me.m_tbDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbDescription.Location = New System.Drawing.Point(6, 78)
        Me.m_tbDescription.Multiline = True
        Me.m_tbDescription.Name = "m_tbDescription"
        Me.m_tbDescription.ReadOnly = True
        Me.m_tbDescription.Size = New System.Drawing.Size(434, 180)
        Me.m_tbDescription.TabIndex = 8
        Me.m_tbDescription.Text = "Description"
        '
        'ucOptionsPluginAssemblyDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tbCopyright)
        Me.Controls.Add(Me.m_tbCompany)
        Me.Controls.Add(Me.m_tbVersion)
        Me.Controls.Add(Me.m_tbDescription)
        Me.Controls.Add(Me.m_tbFile)
        Me.Controls.Add(Me.m_lblCopyright)
        Me.Controls.Add(Me.m_lblCompany)
        Me.Controls.Add(Me.m_lblVersion)
        Me.Controls.Add(Me.m_lblFile)
        Me.Name = "ucOptionsPluginAssemblyDetails"
        Me.Size = New System.Drawing.Size(443, 261)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_lblCopyright As System.Windows.Forms.Label
    Friend WithEvents m_lblCompany As System.Windows.Forms.Label
    Friend WithEvents m_lblVersion As System.Windows.Forms.Label
    Friend WithEvents m_lblFile As System.Windows.Forms.Label
    Friend WithEvents m_tbFile As System.Windows.Forms.TextBox
    Friend WithEvents m_tbVersion As System.Windows.Forms.TextBox
    Friend WithEvents m_tbCompany As System.Windows.Forms.TextBox
    Friend WithEvents m_tbCopyright As System.Windows.Forms.TextBox
    Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox

End Class
