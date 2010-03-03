
Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSY
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
        Me.btRunMSY = New System.Windows.Forms.Button
        Me.btStop = New System.Windows.Forms.Button
        Me.lbFleet = New System.Windows.Forms.Label
        Me.lbiter = New System.Windows.Forms.Label
        Me.lbEffort = New System.Windows.Forms.Label
        Me.txtMSYresults = New System.Windows.Forms.TextBox
        Me.btFleetTradeoffs = New System.Windows.Forms.Button
        Me.rbValue = New System.Windows.Forms.RadioButton
        Me.rbCatch = New System.Windows.Forms.RadioButton
        Me.lblMSY = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btRunMSY
        '
        Me.btRunMSY.Location = New System.Drawing.Point(12, 12)
        Me.btRunMSY.Name = "btRunMSY"
        Me.btRunMSY.Size = New System.Drawing.Size(101, 26)
        Me.btRunMSY.TabIndex = 0
        Me.btRunMSY.Text = "Run MSY search"
        Me.btRunMSY.UseVisualStyleBackColor = True
        '
        'btStop
        '
        Me.btStop.Enabled = False
        Me.btStop.Location = New System.Drawing.Point(140, 12)
        Me.btStop.Name = "btStop"
        Me.btStop.Size = New System.Drawing.Size(101, 26)
        Me.btStop.TabIndex = 1
        Me.btStop.Text = "Stop"
        Me.btStop.UseVisualStyleBackColor = True
        '
        'lbFleet
        '
        Me.lbFleet.AutoSize = True
        Me.lbFleet.Location = New System.Drawing.Point(22, 99)
        Me.lbFleet.Name = "lbFleet"
        Me.lbFleet.Size = New System.Drawing.Size(0, 13)
        Me.lbFleet.TabIndex = 2
        '
        'lbiter
        '
        Me.lbiter.AutoSize = True
        Me.lbiter.Location = New System.Drawing.Point(22, 131)
        Me.lbiter.Name = "lbiter"
        Me.lbiter.Size = New System.Drawing.Size(0, 13)
        Me.lbiter.TabIndex = 3
        '
        'lbEffort
        '
        Me.lbEffort.AutoSize = True
        Me.lbEffort.Location = New System.Drawing.Point(22, 163)
        Me.lbEffort.Name = "lbEffort"
        Me.lbEffort.Size = New System.Drawing.Size(0, 13)
        Me.lbEffort.TabIndex = 4
        '
        'txtMSYresults
        '
        Me.txtMSYresults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtMSYresults.Location = New System.Drawing.Point(396, 12)
        Me.txtMSYresults.Multiline = True
        Me.txtMSYresults.Name = "txtMSYresults"
        Me.txtMSYresults.Size = New System.Drawing.Size(612, 631)
        Me.txtMSYresults.TabIndex = 5
        '
        'btFleetTradeoffs
        '
        Me.btFleetTradeoffs.Enabled = False
        Me.btFleetTradeoffs.Location = New System.Drawing.Point(268, 12)
        Me.btFleetTradeoffs.Name = "btFleetTradeoffs"
        Me.btFleetTradeoffs.Size = New System.Drawing.Size(101, 26)
        Me.btFleetTradeoffs.TabIndex = 6
        Me.btFleetTradeoffs.Text = "Fleet tradeoffs"
        Me.btFleetTradeoffs.UseVisualStyleBackColor = True
        '
        'rbValue
        '
        Me.rbValue.AutoSize = True
        Me.rbValue.Location = New System.Drawing.Point(15, 65)
        Me.rbValue.Name = "rbValue"
        Me.rbValue.Size = New System.Drawing.Size(124, 17)
        Me.rbValue.TabIndex = 7
        Me.rbValue.TabStop = True
        Me.rbValue.Text = "MSY based on value"
        Me.rbValue.UseVisualStyleBackColor = True
        '
        'rbCatch
        '
        Me.rbCatch.AutoSize = True
        Me.rbCatch.Location = New System.Drawing.Point(145, 65)
        Me.rbCatch.Name = "rbCatch"
        Me.rbCatch.Size = New System.Drawing.Size(125, 17)
        Me.rbCatch.TabIndex = 8
        Me.rbCatch.TabStop = True
        Me.rbCatch.Text = "MSY based on catch"
        Me.rbCatch.UseVisualStyleBackColor = True
        '
        'lblMSY
        '
        Me.lblMSY.AutoSize = True
        Me.lblMSY.Location = New System.Drawing.Point(12, 41)
        Me.lblMSY.Name = "lblMSY"
        Me.lblMSY.Size = New System.Drawing.Size(380, 13)
        Me.lblMSY.TabIndex = 9
        Me.lblMSY.Text = "MSY is estimated fleet by fleet. Results are transferred to 'target fishing morta" & _
            "lity'"
        '
        'frmMSY
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1022, 655)
        Me.Controls.Add(Me.lblMSY)
        Me.Controls.Add(Me.rbCatch)
        Me.Controls.Add(Me.rbValue)
        Me.Controls.Add(Me.btFleetTradeoffs)
        Me.Controls.Add(Me.txtMSYresults)
        Me.Controls.Add(Me.lbEffort)
        Me.Controls.Add(Me.lbiter)
        Me.Controls.Add(Me.lbFleet)
        Me.Controls.Add(Me.btStop)
        Me.Controls.Add(Me.btRunMSY)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSY"
        Me.Text = "Run MSY search"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btRunMSY As System.Windows.Forms.Button
    Friend WithEvents btStop As System.Windows.Forms.Button
    Friend WithEvents lbFleet As System.Windows.Forms.Label
    Friend WithEvents lbiter As System.Windows.Forms.Label
    Friend WithEvents lbEffort As System.Windows.Forms.Label
    Friend WithEvents txtMSYresults As System.Windows.Forms.TextBox
    Friend WithEvents btFleetTradeoffs As System.Windows.Forms.Button
    Friend WithEvents rbValue As System.Windows.Forms.RadioButton
    Friend WithEvents rbCatch As System.Windows.Forms.RadioButton
    Friend WithEvents lblMSY As System.Windows.Forms.Label
End Class
