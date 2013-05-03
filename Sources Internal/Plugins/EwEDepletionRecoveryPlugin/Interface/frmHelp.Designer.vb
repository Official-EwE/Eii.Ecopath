#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls
Imports WeifenLuo.WinFormsUI.Docking

#End Region ' Imports

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHelp
    Inherits DockContent

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
        Me.m_tlpHelp = New System.Windows.Forms.TableLayoutPanel
        Me.m_tbHelp = New System.Windows.Forms.Label
        Me.m_tlpHelp.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tlpHelp
        '
        Me.m_tlpHelp.ColumnCount = 3
        Me.m_tlpHelp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpHelp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.m_tlpHelp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpHelp.Controls.Add(Me.m_tbHelp, 1, 1)
        Me.m_tlpHelp.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpHelp.Location = New System.Drawing.Point(0, 0)
        Me.m_tlpHelp.Name = "m_tlpHelp"
        Me.m_tlpHelp.RowCount = 3
        Me.m_tlpHelp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpHelp.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.m_tlpHelp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpHelp.Size = New System.Drawing.Size(292, 273)
        Me.m_tlpHelp.TabIndex = 0
        '
        'm_tbHelp
        '
        Me.m_tbHelp.AutoSize = True
        Me.m_tbHelp.Location = New System.Drawing.Point(71, 130)
        Me.m_tbHelp.Name = "m_tbHelp"
        Me.m_tbHelp.Size = New System.Drawing.Size(149, 13)
        Me.m_tbHelp.TabIndex = 0
        Me.m_tbHelp.Text = "Help and credits planned here"
        '
        'frmHelp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 273)
        Me.Controls.Add(Me.m_tlpHelp)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmHelp"
        Me.Text = "frmHelp"
        Me.m_tlpHelp.ResumeLayout(False)
        Me.m_tlpHelp.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_tlpHelp As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_tbHelp As System.Windows.Forms.Label
End Class
