<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSelector2
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
        Me.m_lbxBits = New System.Windows.Forms.ListBox()
        Me.m_tlpBits = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plControls = New System.Windows.Forms.Panel()
        Me.m_btnAdd = New System.Windows.Forms.Button()
        Me.m_btnRemove = New System.Windows.Forms.Button()
        Me.m_tlpBits.SuspendLayout()
        Me.m_plControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lbxBits
        '
        Me.m_lbxBits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbxBits.FormattingEnabled = True
        Me.m_lbxBits.IntegralHeight = False
        Me.m_lbxBits.Location = New System.Drawing.Point(0, 0)
        Me.m_lbxBits.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.m_lbxBits.Name = "m_lbxBits"
        Me.m_lbxBits.Size = New System.Drawing.Size(181, 258)
        Me.m_lbxBits.TabIndex = 0
        '
        'm_tlpBits
        '
        Me.m_tlpBits.ColumnCount = 2
        Me.m_tlpBits.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpBits.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.m_tlpBits.Controls.Add(Me.m_lbxBits, 0, 0)
        Me.m_tlpBits.Controls.Add(Me.m_plControls, 1, 0)
        Me.m_tlpBits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpBits.Location = New System.Drawing.Point(0, 0)
        Me.m_tlpBits.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tlpBits.Name = "m_tlpBits"
        Me.m_tlpBits.RowCount = 1
        Me.m_tlpBits.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpBits.Size = New System.Drawing.Size(266, 258)
        Me.m_tlpBits.TabIndex = 1
        '
        'm_plControls
        '
        Me.m_plControls.Controls.Add(Me.m_btnRemove)
        Me.m_plControls.Controls.Add(Me.m_btnAdd)
        Me.m_plControls.Location = New System.Drawing.Point(184, 0)
        Me.m_plControls.Margin = New System.Windows.Forms.Padding(0)
        Me.m_plControls.Name = "m_plControls"
        Me.m_plControls.Size = New System.Drawing.Size(82, 58)
        Me.m_plControls.TabIndex = 1
        '
        'm_btnAdd
        '
        Me.m_btnAdd.Location = New System.Drawing.Point(3, 3)
        Me.m_btnAdd.Name = "m_btnAdd"
        Me.m_btnAdd.Size = New System.Drawing.Size(75, 23)
        Me.m_btnAdd.TabIndex = 0
        Me.m_btnAdd.Text = "&Add"
        Me.m_btnAdd.UseVisualStyleBackColor = True
        '
        'm_btnRemove
        '
        Me.m_btnRemove.Location = New System.Drawing.Point(3, 32)
        Me.m_btnRemove.Name = "m_btnRemove"
        Me.m_btnRemove.Size = New System.Drawing.Size(75, 23)
        Me.m_btnRemove.TabIndex = 0
        Me.m_btnRemove.Text = "&Remove"
        Me.m_btnRemove.UseVisualStyleBackColor = True
        '
        'ucSelector2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlpBits)
        Me.Name = "ucSelector2"
        Me.Size = New System.Drawing.Size(266, 258)
        Me.m_tlpBits.ResumeLayout(False)
        Me.m_plControls.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_lbxBits As System.Windows.Forms.ListBox
    Private WithEvents m_tlpBits As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_plControls As System.Windows.Forms.Panel
    Private WithEvents m_btnRemove As System.Windows.Forms.Button
    Private WithEvents m_btnAdd As System.Windows.Forms.Button

End Class
