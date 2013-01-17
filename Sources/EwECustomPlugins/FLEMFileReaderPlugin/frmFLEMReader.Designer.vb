<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFLEMReader
    Inherits ScientificInterfaceShared.Forms.frmEwE

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
        Me.chkForcePP = New System.Windows.Forms.CheckBox()
        Me.btForcingFile = New System.Windows.Forms.Button()
        Me.chkForceHabCap = New System.Windows.Forms.CheckBox()
        Me.lbForceFile = New System.Windows.Forms.Label()
        Me.cbHabCap = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'chkForcePP
        '
        Me.chkForcePP.AutoSize = True
        Me.chkForcePP.Location = New System.Drawing.Point(12, 51)
        Me.chkForcePP.Name = "chkForcePP"
        Me.chkForcePP.Size = New System.Drawing.Size(197, 17)
        Me.chkForcePP.TabIndex = 0
        Me.chkForcePP.Text = "Force primary production and salinity"
        Me.chkForcePP.UseVisualStyleBackColor = True
        '
        'btForcingFile
        '
        Me.btForcingFile.Location = New System.Drawing.Point(12, 12)
        Me.btForcingFile.Name = "btForcingFile"
        Me.btForcingFile.Size = New System.Drawing.Size(116, 21)
        Me.btForcingFile.TabIndex = 2
        Me.btForcingFile.Text = "Select forcing file..."
        Me.btForcingFile.UseVisualStyleBackColor = True
        '
        'chkForceHabCap
        '
        Me.chkForceHabCap.AutoSize = True
        Me.chkForceHabCap.Location = New System.Drawing.Point(12, 82)
        Me.chkForceHabCap.Name = "chkForceHabCap"
        Me.chkForceHabCap.Size = New System.Drawing.Size(135, 17)
        Me.chkForceHabCap.TabIndex = 3
        Me.chkForceHabCap.Text = "Modify habitat capacity"
        Me.chkForceHabCap.UseVisualStyleBackColor = True
        '
        'lbForceFile
        '
        Me.lbForceFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbForceFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lbForceFile.Location = New System.Drawing.Point(134, 12)
        Me.lbForceFile.Name = "lbForceFile"
        Me.lbForceFile.Size = New System.Drawing.Size(459, 21)
        Me.lbForceFile.TabIndex = 5
        '
        'cbHabCap
        '
        Me.cbHabCap.FormattingEnabled = True
        Me.cbHabCap.Location = New System.Drawing.Point(215, 102)
        Me.cbHabCap.Name = "cbHabCap"
        Me.cbHabCap.Size = New System.Drawing.Size(225, 21)
        Me.cbHabCap.TabIndex = 6
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(48, 102)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(161, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Group modifying habitat capacity"
        '
        'frmFLEMReader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(606, 262)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbHabCap)
        Me.Controls.Add(Me.lbForceFile)
        Me.Controls.Add(Me.chkForceHabCap)
        Me.Controls.Add(Me.btForcingFile)
        Me.Controls.Add(Me.chkForcePP)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmFLEMReader"
        Me.Text = "FLEM File Reader"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chkForcePP As System.Windows.Forms.CheckBox
    Friend WithEvents btForcingFile As System.Windows.Forms.Button
    Friend WithEvents chkForceHabCap As System.Windows.Forms.CheckBox
    Friend WithEvents lbForceFile As System.Windows.Forms.Label
    Friend WithEvents cbHabCap As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
