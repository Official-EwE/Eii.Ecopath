<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucAppPluginDetails
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
        Me.m_lblContact = New System.Windows.Forms.Label
        Me.m_lblAuthor = New System.Windows.Forms.Label
        Me.m_lblName = New System.Windows.Forms.Label
        Me.m_tbName = New System.Windows.Forms.TextBox
        Me.m_tbDescription = New System.Windows.Forms.TextBox
        Me.m_tbAuthor = New System.Windows.Forms.TextBox
        Me.m_cbEnabled = New System.Windows.Forms.CheckBox
        Me.m_llContact = New System.Windows.Forms.LinkLabel
        Me.SuspendLayout()
        '
        'm_lblContact
        '
        Me.m_lblContact.AutoSize = True
        Me.m_lblContact.Location = New System.Drawing.Point(3, 41)
        Me.m_lblContact.Name = "m_lblContact"
        Me.m_lblContact.Size = New System.Drawing.Size(47, 13)
        Me.m_lblContact.TabIndex = 4
        Me.m_lblContact.Text = "Contact:"
        '
        'm_lblAuthor
        '
        Me.m_lblAuthor.AutoSize = True
        Me.m_lblAuthor.Location = New System.Drawing.Point(3, 22)
        Me.m_lblAuthor.Name = "m_lblAuthor"
        Me.m_lblAuthor.Size = New System.Drawing.Size(52, 13)
        Me.m_lblAuthor.TabIndex = 2
        Me.m_lblAuthor.Text = "Author(s):"
        '
        'm_lblName
        '
        Me.m_lblName.AutoSize = True
        Me.m_lblName.Location = New System.Drawing.Point(3, 3)
        Me.m_lblName.Name = "m_lblName"
        Me.m_lblName.Size = New System.Drawing.Size(38, 13)
        Me.m_lblName.TabIndex = 0
        Me.m_lblName.Text = "Name:"
        '
        'm_tbName
        '
        Me.m_tbName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbName.Location = New System.Drawing.Point(66, 3)
        Me.m_tbName.Name = "m_tbName"
        Me.m_tbName.ReadOnly = True
        Me.m_tbName.Size = New System.Drawing.Size(222, 13)
        Me.m_tbName.TabIndex = 1
        '
        'm_tbDescription
        '
        Me.m_tbDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbDescription.Cursor = System.Windows.Forms.Cursors.Default
        Me.m_tbDescription.Location = New System.Drawing.Point(6, 78)
        Me.m_tbDescription.Multiline = True
        Me.m_tbDescription.Name = "m_tbDescription"
        Me.m_tbDescription.ReadOnly = True
        Me.m_tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.m_tbDescription.Size = New System.Drawing.Size(282, 156)
        Me.m_tbDescription.TabIndex = 6
        Me.m_tbDescription.Text = "Description"
        '
        'm_tbAuthor
        '
        Me.m_tbAuthor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbAuthor.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_tbAuthor.Location = New System.Drawing.Point(66, 22)
        Me.m_tbAuthor.Name = "m_tbAuthor"
        Me.m_tbAuthor.ReadOnly = True
        Me.m_tbAuthor.Size = New System.Drawing.Size(222, 13)
        Me.m_tbAuthor.TabIndex = 3
        '
        'm_cbEnabled
        '
        Me.m_cbEnabled.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_cbEnabled.AutoSize = True
        Me.m_cbEnabled.Location = New System.Drawing.Point(6, 240)
        Me.m_cbEnabled.Name = "m_cbEnabled"
        Me.m_cbEnabled.Size = New System.Drawing.Size(149, 17)
        Me.m_cbEnabled.TabIndex = 10
        Me.m_cbEnabled.Text = "&Enable this plug-in module"
        Me.m_cbEnabled.UseVisualStyleBackColor = True
        '
        'm_llContact
        '
        Me.m_llContact.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_llContact.Location = New System.Drawing.Point(63, 41)
        Me.m_llContact.Name = "m_llContact"
        Me.m_llContact.Size = New System.Drawing.Size(225, 34)
        Me.m_llContact.TabIndex = 11
        '
        'ucAppPluginDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_llContact)
        Me.Controls.Add(Me.m_cbEnabled)
        Me.Controls.Add(Me.m_tbAuthor)
        Me.Controls.Add(Me.m_tbName)
        Me.Controls.Add(Me.m_tbDescription)
        Me.Controls.Add(Me.m_lblContact)
        Me.Controls.Add(Me.m_lblAuthor)
        Me.Controls.Add(Me.m_lblName)
        Me.Name = "ucAppPluginDetails"
        Me.Size = New System.Drawing.Size(291, 261)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lblContact As System.Windows.Forms.Label
    Private WithEvents m_lblAuthor As System.Windows.Forms.Label
    Private WithEvents m_lblName As System.Windows.Forms.Label
    Private WithEvents m_tbName As System.Windows.Forms.TextBox
    Private WithEvents m_tbDescription As System.Windows.Forms.TextBox
    Private WithEvents m_tbAuthor As System.Windows.Forms.TextBox
    Private WithEvents m_cbEnabled As System.Windows.Forms.CheckBox
    Private WithEvents m_llContact As System.Windows.Forms.LinkLabel

End Class
