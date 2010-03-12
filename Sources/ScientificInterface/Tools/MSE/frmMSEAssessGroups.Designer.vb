<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEAssessGroups
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
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
        Me.m_blocks = New ScientificInterface.Ecosim.ucPolicyColorBlocks
        Me.SuspendLayout()
        '
        'm_blocks
        '
        Me.m_blocks.CurColor = System.Drawing.Color.Empty
        Me.m_blocks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_blocks.Location = New System.Drawing.Point(0, 0)
        Me.m_blocks.Margin = New System.Windows.Forms.Padding(0)
        Me.m_blocks.Name = "m_blocks"
        Me.m_blocks.ParmBlockCodes = Nothing
        Me.m_blocks.Size = New System.Drawing.Size(652, 483)
        Me.m_blocks.TabIndex = 1
        Me.m_blocks.UIContext = Nothing
        '
        'frmMSEAssessGroups
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(652, 483)
        Me.Controls.Add(Me.m_blocks)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSEAssessGroups"
        Me.Text = "frmMSEAssessGroups"
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_blocks As ScientificInterface.Ecosim.ucPolicyColorBlocks
End Class
