<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSelector
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.m_btnPrev = New System.Windows.Forms.Button
        Me.m_btnRight = New System.Windows.Forms.Button
        Me.m_lblInfo = New System.Windows.Forms.Label
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel1.Controls.Add(Me.m_btnPrev, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.m_btnRight, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblInfo, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(150, 23)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'm_btnPrev
        '
        Me.m_btnPrev.Location = New System.Drawing.Point(0, 0)
        Me.m_btnPrev.Margin = New System.Windows.Forms.Padding(0)
        Me.m_btnPrev.Name = "m_btnPrev"
        Me.m_btnPrev.Size = New System.Drawing.Size(23, 23)
        Me.m_btnPrev.TabIndex = 0
        Me.m_btnPrev.Text = "<"
        Me.m_btnPrev.UseVisualStyleBackColor = True
        '
        'm_btnRight
        '
        Me.m_btnRight.Location = New System.Drawing.Point(127, 0)
        Me.m_btnRight.Margin = New System.Windows.Forms.Padding(0)
        Me.m_btnRight.Name = "m_btnRight"
        Me.m_btnRight.Size = New System.Drawing.Size(23, 23)
        Me.m_btnRight.TabIndex = 0
        Me.m_btnRight.Text = ">"
        Me.m_btnRight.UseVisualStyleBackColor = True
        '
        'm_lblInfo
        '
        Me.m_lblInfo.AutoSize = True
        Me.m_lblInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblInfo.Location = New System.Drawing.Point(26, 0)
        Me.m_lblInfo.Name = "m_lblInfo"
        Me.m_lblInfo.Size = New System.Drawing.Size(98, 23)
        Me.m_lblInfo.TabIndex = 1
        Me.m_lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ucSelectLeftRight
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ucSelectLeftRight"
        Me.Size = New System.Drawing.Size(150, 23)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_btnPrev As System.Windows.Forms.Button
    Private WithEvents m_btnRight As System.Windows.Forms.Button
    Private WithEvents m_lblInfo As System.Windows.Forms.Label

End Class
