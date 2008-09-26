Namespace Wizard

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class DatabaseConversionWizard
        Inherits ScientificInterface.Wizard.WizardFormBase

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.tbpStep1 = New System.Windows.Forms.TabPage
            Me.Label2 = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.PictureBox1 = New System.Windows.Forms.PictureBox
            Me.tbpStep2 = New System.Windows.Forms.TabPage
            Me.btnBrowseTargetDirectory = New System.Windows.Forms.Button
            Me.txbSaveModelName = New System.Windows.Forms.TextBox
            Me.Label6 = New System.Windows.Forms.Label
            Me.Panel2 = New System.Windows.Forms.Panel
            Me.PictureBox4 = New System.Windows.Forms.PictureBox
            Me.PictureBox2 = New System.Windows.Forms.PictureBox
            Me.Label4 = New System.Windows.Forms.Label
            Me.lblDatabaseName = New System.Windows.Forms.Label
            Me.lbModels = New System.Windows.Forms.ListBox
            Me.Label5 = New System.Windows.Forms.Label
            Me.tbpStep3 = New System.Windows.Forms.TabPage
            Me.m_lbProgress = New System.Windows.Forms.Label
            Me.m_pb = New System.Windows.Forms.ProgressBar
            Me.Label9 = New System.Windows.Forms.Label
            Me.txbSummary = New System.Windows.Forms.TextBox
            Me.Panel3 = New System.Windows.Forms.Panel
            Me.PictureBox5 = New System.Windows.Forms.PictureBox
            Me.PictureBox3 = New System.Windows.Forms.PictureBox
            Me.Label7 = New System.Windows.Forms.Label
            Me.Label8 = New System.Windows.Forms.Label
            Me.tcMain.SuspendLayout()
            Me.tbpStep1.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tbpStep2.SuspendLayout()
            Me.Panel2.SuspendLayout()
            CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tbpStep3.SuspendLayout()
            Me.Panel3.SuspendLayout()
            CType(Me.PictureBox5, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'tcMain
            '
            Me.tcMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tcMain.Controls.Add(Me.tbpStep1)
            Me.tcMain.Controls.Add(Me.tbpStep2)
            Me.tcMain.Controls.Add(Me.tbpStep3)
            Me.tcMain.Dock = System.Windows.Forms.DockStyle.None
            Me.tcMain.Size = New System.Drawing.Size(492, 306)
            '
            'tbpStep1
            '
            Me.tbpStep1.Controls.Add(Me.Label2)
            Me.tbpStep1.Controls.Add(Me.Label1)
            Me.tbpStep1.Controls.Add(Me.PictureBox1)
            Me.tbpStep1.Location = New System.Drawing.Point(4, 22)
            Me.tbpStep1.Name = "tbpStep1"
            Me.tbpStep1.Size = New System.Drawing.Size(484, 280)
            Me.tbpStep1.TabIndex = 0
            Me.tbpStep1.Text = "Ecopath Database Conversion Wizard"
            Me.tbpStep1.UseVisualStyleBackColor = True
            '
            'Label2
            '
            Me.Label2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label2.Location = New System.Drawing.Point(163, 100)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(313, 170)
            Me.Label2.TabIndex = 2
            Me.Label2.Text = "The database you are opening was created in a previous version of Ecopath with Ec" & _
                "osim.  It must be converted to the format used by this version." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Click Next to" & _
                " proceed."
            '
            'Label1
            '
            Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label1.Location = New System.Drawing.Point(159, 33)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(314, 46)
            Me.Label1.TabIndex = 1
            Me.Label1.Text = "Welcome to Ecopath Database Conversion Wizard"
            '
            'PictureBox1
            '
            Me.PictureBox1.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.PictureBox1.Location = New System.Drawing.Point(8, 3)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(145, 303)
            Me.PictureBox1.TabIndex = 0
            Me.PictureBox1.TabStop = False
            '
            'tbpStep2
            '
            Me.tbpStep2.Controls.Add(Me.btnBrowseTargetDirectory)
            Me.tbpStep2.Controls.Add(Me.txbSaveModelName)
            Me.tbpStep2.Controls.Add(Me.Label6)
            Me.tbpStep2.Controls.Add(Me.Panel2)
            Me.tbpStep2.Controls.Add(Me.lbModels)
            Me.tbpStep2.Controls.Add(Me.Label5)
            Me.tbpStep2.Location = New System.Drawing.Point(4, 22)
            Me.tbpStep2.Name = "tbpStep2"
            Me.tbpStep2.Size = New System.Drawing.Size(484, 280)
            Me.tbpStep2.TabIndex = 1
            Me.tbpStep2.Text = "Ecopath Database Conversion Wizard"
            Me.tbpStep2.UseVisualStyleBackColor = True
            '
            'btnBrowseTargetDirectory
            '
            Me.btnBrowseTargetDirectory.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnBrowseTargetDirectory.Location = New System.Drawing.Point(416, 252)
            Me.btnBrowseTargetDirectory.Name = "btnBrowseTargetDirectory"
            Me.btnBrowseTargetDirectory.Size = New System.Drawing.Size(64, 23)
            Me.btnBrowseTargetDirectory.TabIndex = 5
            Me.btnBrowseTargetDirectory.Text = "&Browse..."
            Me.btnBrowseTargetDirectory.UseVisualStyleBackColor = True
            '
            'txbSaveModelName
            '
            Me.txbSaveModelName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txbSaveModelName.Location = New System.Drawing.Point(6, 254)
            Me.txbSaveModelName.Name = "txbSaveModelName"
            Me.txbSaveModelName.Size = New System.Drawing.Size(404, 20)
            Me.txbSaveModelName.TabIndex = 4
            '
            'Label6
            '
            Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.Label6.AutoSize = True
            Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label6.Location = New System.Drawing.Point(3, 238)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(83, 13)
            Me.Label6.TabIndex = 3
            Me.Label6.Text = "Save model &as.."
            '
            'Panel2
            '
            Me.Panel2.BackColor = System.Drawing.Color.White
            Me.Panel2.Controls.Add(Me.PictureBox4)
            Me.Panel2.Controls.Add(Me.PictureBox2)
            Me.Panel2.Controls.Add(Me.Label4)
            Me.Panel2.Controls.Add(Me.lblDatabaseName)
            Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
            Me.Panel2.Location = New System.Drawing.Point(0, 0)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(484, 66)
            Me.Panel2.TabIndex = 0
            '
            'PictureBox4
            '
            Me.PictureBox4.Image = Global.ScientificInterface.My.Resources.Resources.ecopath_256x256
            Me.PictureBox4.Location = New System.Drawing.Point(416, 0)
            Me.PictureBox4.Name = "PictureBox4"
            Me.PictureBox4.Size = New System.Drawing.Size(64, 64)
            Me.PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
            Me.PictureBox4.TabIndex = 0
            Me.PictureBox4.TabStop = False
            '
            'PictureBox2
            '
            Me.PictureBox2.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo
            Me.PictureBox2.Location = New System.Drawing.Point(3, 0)
            Me.PictureBox2.Name = "PictureBox2"
            Me.PictureBox2.Size = New System.Drawing.Size(77, 64)
            Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
            Me.PictureBox2.TabIndex = 0
            Me.PictureBox2.TabStop = False
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label4.Location = New System.Drawing.Point(86, 9)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(117, 16)
            Me.Label4.TabIndex = 0
            Me.Label4.Text = "Selected model"
            '
            'lblDatabaseName
            '
            Me.lblDatabaseName.AutoSize = True
            Me.lblDatabaseName.Location = New System.Drawing.Point(92, 38)
            Me.lblDatabaseName.Name = "lblDatabaseName"
            Me.lblDatabaseName.Size = New System.Drawing.Size(0, 13)
            Me.lblDatabaseName.TabIndex = 1
            '
            'lbModels
            '
            Me.lbModels.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lbModels.FormattingEnabled = True
            Me.lbModels.IntegralHeight = False
            Me.lbModels.Location = New System.Drawing.Point(6, 84)
            Me.lbModels.Name = "lbModels"
            Me.lbModels.Size = New System.Drawing.Size(474, 142)
            Me.lbModels.Sorted = True
            Me.lbModels.TabIndex = 2
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label5.Location = New System.Drawing.Point(3, 69)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(131, 13)
            Me.Label5.TabIndex = 1
            Me.Label5.Text = "&Select a model to convert:"
            '
            'tbpStep3
            '
            Me.tbpStep3.Controls.Add(Me.m_lbProgress)
            Me.tbpStep3.Controls.Add(Me.m_pb)
            Me.tbpStep3.Controls.Add(Me.Label9)
            Me.tbpStep3.Controls.Add(Me.txbSummary)
            Me.tbpStep3.Controls.Add(Me.Panel3)
            Me.tbpStep3.Location = New System.Drawing.Point(4, 22)
            Me.tbpStep3.Name = "tbpStep3"
            Me.tbpStep3.Size = New System.Drawing.Size(484, 280)
            Me.tbpStep3.TabIndex = 2
            Me.tbpStep3.Text = "Ecopath Database Conversion Wizard"
            Me.tbpStep3.UseVisualStyleBackColor = True
            '
            'm_lbProgress
            '
            Me.m_lbProgress.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lbProgress.AutoSize = True
            Me.m_lbProgress.Location = New System.Drawing.Point(3, 236)
            Me.m_lbProgress.Name = "m_lbProgress"
            Me.m_lbProgress.Size = New System.Drawing.Size(106, 13)
            Me.m_lbProgress.TabIndex = 3
            Me.m_lbProgress.Text = "Conversion progress:"
            '
            'm_pb
            '
            Me.m_pb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pb.Location = New System.Drawing.Point(3, 252)
            Me.m_pb.Name = "m_pb"
            Me.m_pb.Size = New System.Drawing.Size(477, 23)
            Me.m_pb.Style = System.Windows.Forms.ProgressBarStyle.Continuous
            Me.m_pb.TabIndex = 4
            '
            'Label9
            '
            Me.Label9.AutoSize = True
            Me.Label9.Location = New System.Drawing.Point(3, 69)
            Me.Label9.Name = "Label9"
            Me.Label9.Size = New System.Drawing.Size(53, 13)
            Me.Label9.TabIndex = 1
            Me.Label9.Text = "&Summary:"
            '
            'txbSummary
            '
            Me.txbSummary.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txbSummary.Cursor = System.Windows.Forms.Cursors.Default
            Me.txbSummary.Location = New System.Drawing.Point(3, 85)
            Me.txbSummary.Multiline = True
            Me.txbSummary.Name = "txbSummary"
            Me.txbSummary.ReadOnly = True
            Me.txbSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.txbSummary.Size = New System.Drawing.Size(477, 148)
            Me.txbSummary.TabIndex = 2
            '
            'Panel3
            '
            Me.Panel3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Panel3.BackColor = System.Drawing.Color.White
            Me.Panel3.Controls.Add(Me.PictureBox5)
            Me.Panel3.Controls.Add(Me.PictureBox3)
            Me.Panel3.Controls.Add(Me.Label7)
            Me.Panel3.Controls.Add(Me.Label8)
            Me.Panel3.Location = New System.Drawing.Point(0, 0)
            Me.Panel3.Name = "Panel3"
            Me.Panel3.Size = New System.Drawing.Size(483, 66)
            Me.Panel3.TabIndex = 0
            '
            'PictureBox5
            '
            Me.PictureBox5.Image = Global.ScientificInterface.My.Resources.Resources.ecopath_256x256
            Me.PictureBox5.Location = New System.Drawing.Point(416, 0)
            Me.PictureBox5.Name = "PictureBox5"
            Me.PictureBox5.Size = New System.Drawing.Size(64, 64)
            Me.PictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
            Me.PictureBox5.TabIndex = 2
            Me.PictureBox5.TabStop = False
            '
            'PictureBox3
            '
            Me.PictureBox3.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo
            Me.PictureBox3.Location = New System.Drawing.Point(3, 0)
            Me.PictureBox3.Name = "PictureBox3"
            Me.PictureBox3.Size = New System.Drawing.Size(77, 64)
            Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
            Me.PictureBox3.TabIndex = 0
            Me.PictureBox3.TabStop = False
            '
            'Label7
            '
            Me.Label7.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label7.Location = New System.Drawing.Point(86, 9)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(324, 55)
            Me.Label7.TabIndex = 0
            Me.Label7.Text = "Converting database, please wait"
            '
            'Label8
            '
            Me.Label8.AutoSize = True
            Me.Label8.Location = New System.Drawing.Point(92, 38)
            Me.Label8.Name = "Label8"
            Me.Label8.Size = New System.Drawing.Size(0, 13)
            Me.Label8.TabIndex = 1
            '
            'DatabaseConversionWizard
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(492, 363)
            Me.MinimumSize = New System.Drawing.Size(500, 390)
            Me.Name = "DatabaseConversionWizard"
            Me.Text = "DatabaseConversionWizard"
            Me.tcMain.ResumeLayout(False)
            Me.tbpStep1.ResumeLayout(False)
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tbpStep2.ResumeLayout(False)
            Me.tbpStep2.PerformLayout()
            Me.Panel2.ResumeLayout(False)
            Me.Panel2.PerformLayout()
            CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tbpStep3.ResumeLayout(False)
            Me.tbpStep3.PerformLayout()
            Me.Panel3.ResumeLayout(False)
            Me.Panel3.PerformLayout()
            CType(Me.PictureBox5, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents tbpStep1 As System.Windows.Forms.TabPage
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents tbpStep2 As System.Windows.Forms.TabPage
        Friend WithEvents lbModels As System.Windows.Forms.ListBox
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents lblDatabaseName As System.Windows.Forms.Label
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents btnBrowseTargetDirectory As System.Windows.Forms.Button
        Friend WithEvents txbSaveModelName As System.Windows.Forms.TextBox
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents tbpStep3 As System.Windows.Forms.TabPage
        Friend WithEvents Panel3 As System.Windows.Forms.Panel
        Friend WithEvents PictureBox3 As System.Windows.Forms.PictureBox
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents Label8 As System.Windows.Forms.Label
        Friend WithEvents txbSummary As System.Windows.Forms.TextBox
        Friend WithEvents Label9 As System.Windows.Forms.Label
        Friend WithEvents m_lbProgress As System.Windows.Forms.Label
        Friend WithEvents m_pb As System.Windows.Forms.ProgressBar
        Friend WithEvents PictureBox4 As System.Windows.Forms.PictureBox
        Friend WithEvents PictureBox5 As System.Windows.Forms.PictureBox
    End Class

End Namespace