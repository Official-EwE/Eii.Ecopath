Namespace Ecotracer

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmEcotracerInput
        Inherits frmEwEGrid

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
            Me.cmbEnvInflowFF = New System.Windows.Forms.ComboBox
            Me.m_lbFFEnv = New System.Windows.Forms.Label
            Me.m_tbCLossEnv = New System.Windows.Forms.TextBox
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.m_tbCInflowEnv = New System.Windows.Forms.TextBox
            Me.m_lblCDecay = New System.Windows.Forms.Label
            Me.m_lblCInflowEnv = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.m_plGrid = New System.Windows.Forms.Panel
            Me.m_tbCZeroEnv = New System.Windows.Forms.TextBox
            Me.m_tbCDecayRateEnv = New System.Windows.Forms.TextBox
            Me.m_lbCZeroEnv = New System.Windows.Forms.Label
            Me.m_lbCDecayRateEnv = New System.Windows.Forms.Label
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.SuspendLayout()
            '
            'cmbEnvInflowFF
            '
            Me.cmbEnvInflowFF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbEnvInflowFF.FormattingEnabled = True
            Me.cmbEnvInflowFF.Location = New System.Drawing.Point(201, 108)
            Me.cmbEnvInflowFF.Name = "cmbEnvInflowFF"
            Me.cmbEnvInflowFF.Size = New System.Drawing.Size(194, 21)
            Me.cmbEnvInflowFF.TabIndex = 10
            '
            'm_lbFFEnv
            '
            Me.m_lbFFEnv.AutoSize = True
            Me.m_lbFFEnv.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lbFFEnv.Location = New System.Drawing.Point(12, 111)
            Me.m_lbFFEnv.Name = "m_lbFFEnv"
            Me.m_lbFFEnv.Size = New System.Drawing.Size(183, 13)
            Me.m_lbFFEnv.TabIndex = 9
            Me.m_lbFFEnv.Text = "&Environmental inflow forcing function:"
            '
            'm_tbCLossEnv
            '
            Me.m_tbCLossEnv.Location = New System.Drawing.Point(500, 82)
            Me.m_tbCLossEnv.Name = "m_tbCLossEnv"
            Me.m_tbCLossEnv.Size = New System.Drawing.Size(80, 20)
            Me.m_tbCLossEnv.TabIndex = 8
            '
            'lblInitializationHeader
            '
            Me.lblInitializationHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitializationHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitializationHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblInitializationHeader.Location = New System.Drawing.Point(12, 35)
            Me.lblInitializationHeader.Name = "lblInitializationHeader"
            Me.lblInitializationHeader.Size = New System.Drawing.Size(568, 18)
            Me.lblInitializationHeader.TabIndex = 0
            Me.lblInitializationHeader.Text = "Environment"
            Me.lblInitializationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tbCInflowEnv
            '
            Me.m_tbCInflowEnv.Location = New System.Drawing.Point(500, 56)
            Me.m_tbCInflowEnv.Name = "m_tbCInflowEnv"
            Me.m_tbCInflowEnv.Size = New System.Drawing.Size(80, 20)
            Me.m_tbCInflowEnv.TabIndex = 4
            '
            'm_lblCDecay
            '
            Me.m_lblCDecay.AutoSize = True
            Me.m_lblCDecay.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblCDecay.Location = New System.Drawing.Point(318, 85)
            Me.m_lblCDecay.Name = "m_lblCDecay"
            Me.m_lblCDecay.Size = New System.Drawing.Size(176, 13)
            Me.m_lblCDecay.TabIndex = 7
            Me.m_lblCDecay.Text = "Base &volume exchange loss (/year):"
            '
            'm_lblCInflowEnv
            '
            Me.m_lblCInflowEnv.AutoSize = True
            Me.m_lblCInflowEnv.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblCInflowEnv.Location = New System.Drawing.Point(318, 59)
            Me.m_lblCInflowEnv.Name = "m_lblCInflowEnv"
            Me.m_lblCInflowEnv.Size = New System.Drawing.Size(147, 13)
            Me.m_lblCInflowEnv.TabIndex = 3
            Me.m_lblCInflowEnv.Text = "&Base inflow rate (t/km2/year):"
            '
            'Label1
            '
            Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.Label1.Location = New System.Drawing.Point(12, 142)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(568, 18)
            Me.Label1.TabIndex = 11
            Me.Label1.Text = "Groups"
            Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_plGrid
            '
            Me.m_plGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_plGrid.Location = New System.Drawing.Point(12, 164)
            Me.m_plGrid.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plGrid.Name = "m_plGrid"
            Me.m_plGrid.Size = New System.Drawing.Size(568, 260)
            Me.m_plGrid.TabIndex = 12
            '
            'm_tbCZeroEnv
            '
            Me.m_tbCZeroEnv.Location = New System.Drawing.Point(201, 56)
            Me.m_tbCZeroEnv.Name = "m_tbCZeroEnv"
            Me.m_tbCZeroEnv.Size = New System.Drawing.Size(80, 20)
            Me.m_tbCZeroEnv.TabIndex = 2
            '
            'm_tbCDecayRateEnv
            '
            Me.m_tbCDecayRateEnv.Location = New System.Drawing.Point(201, 82)
            Me.m_tbCDecayRateEnv.Name = "m_tbCDecayRateEnv"
            Me.m_tbCDecayRateEnv.Size = New System.Drawing.Size(80, 20)
            Me.m_tbCDecayRateEnv.TabIndex = 6
            '
            'm_lbCZeroEnv
            '
            Me.m_lbCZeroEnv.AutoSize = True
            Me.m_lbCZeroEnv.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lbCZeroEnv.Location = New System.Drawing.Point(12, 59)
            Me.m_lbCZeroEnv.Name = "m_lbCZeroEnv"
            Me.m_lbCZeroEnv.Size = New System.Drawing.Size(139, 13)
            Me.m_lbCZeroEnv.TabIndex = 1
            Me.m_lbCZeroEnv.Text = "&Initial concentration (t/km2):"
            '
            'm_lbCDecayRateEnv
            '
            Me.m_lbCDecayRateEnv.AutoSize = True
            Me.m_lbCDecayRateEnv.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lbCDecayRateEnv.Location = New System.Drawing.Point(12, 85)
            Me.m_lbCDecayRateEnv.Name = "m_lbCDecayRateEnv"
            Me.m_lbCDecayRateEnv.Size = New System.Drawing.Size(96, 13)
            Me.m_lbCDecayRateEnv.TabIndex = 5
            Me.m_lbCDecayRateEnv.Text = "&Decay rate (/year):"
            '
            'm_ts
            '
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.Size = New System.Drawing.Size(592, 25)
            Me.m_ts.TabIndex = 13
            '
            'frmEcotracerInput
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(592, 436)
            Me.Controls.Add(Me.m_ts)
            Me.Controls.Add(Me.m_plGrid)
            Me.Controls.Add(Me.cmbEnvInflowFF)
            Me.Controls.Add(Me.m_lbFFEnv)
            Me.Controls.Add(Me.m_tbCDecayRateEnv)
            Me.Controls.Add(Me.m_tbCLossEnv)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.lblInitializationHeader)
            Me.Controls.Add(Me.m_tbCZeroEnv)
            Me.Controls.Add(Me.m_tbCInflowEnv)
            Me.Controls.Add(Me.m_lbCDecayRateEnv)
            Me.Controls.Add(Me.m_lblCDecay)
            Me.Controls.Add(Me.m_lbCZeroEnv)
            Me.Controls.Add(Me.m_lblCInflowEnv)
            Me.MinimumSize = New System.Drawing.Size(600, 250)
            Me.Name = "frmEcotracerInput"
            Me.Text = "frmEcotracerInput"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_plGrid As System.Windows.Forms.Panel
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents m_tbCLossEnv As System.Windows.Forms.TextBox
        Private WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Private WithEvents m_tbCInflowEnv As System.Windows.Forms.TextBox
        Private WithEvents m_lblCDecay As System.Windows.Forms.Label
        Private WithEvents m_lblCInflowEnv As System.Windows.Forms.Label
        Private WithEvents m_tbCZeroEnv As System.Windows.Forms.TextBox
        Private WithEvents m_tbCDecayRateEnv As System.Windows.Forms.TextBox
        Private WithEvents m_lbCZeroEnv As System.Windows.Forms.Label
        Private WithEvents m_lbCDecayRateEnv As System.Windows.Forms.Label
        Private WithEvents cmbEnvInflowFF As System.Windows.Forms.ComboBox
        Private WithEvents m_lbFFEnv As System.Windows.Forms.Label
        Private WithEvents Label1 As System.Windows.Forms.Label
    End Class

End Namespace
