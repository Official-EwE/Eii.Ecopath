Namespace Other
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Public Class frmAboutEwE
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Friend WithEvents OKButton As System.Windows.Forms.Button

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAboutEwE))
            Me.OKButton = New System.Windows.Forms.Button
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.PictureBox1 = New System.Windows.Forms.PictureBox
            Me.tlpDetails = New System.Windows.Forms.TableLayoutPanel
            Me.lbTitle = New System.Windows.Forms.Label
            Me.rtbDisclaimer = New System.Windows.Forms.RichTextBox
            Me.lbVersion = New System.Windows.Forms.Label
            Me.lbCopyright = New System.Windows.Forms.Label
            Me.rtbDistribution = New System.Windows.Forms.RichTextBox
            Me.PictureBox2 = New System.Windows.Forms.PictureBox
            Me.TabControl1 = New System.Windows.Forms.TabControl
            Me.tpGeneral = New System.Windows.Forms.TabPage
            Me.tpCredits = New System.Windows.Forms.TabPage
            Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel
            Me.PictureBox4 = New System.Windows.Forms.PictureBox
            Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel
            Me.rtbCredits = New System.Windows.Forms.RichTextBox
            Me.tpModules = New System.Windows.Forms.TabPage
            Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel
            Me.PictureBox5 = New System.Windows.Forms.PictureBox
            Me.TableLayoutPanel7 = New System.Windows.Forms.TableLayoutPanel
            Me.rtbModules = New System.Windows.Forms.RichTextBox
            Me.tpTechnical = New System.Windows.Forms.TabPage
            Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel
            Me.PictureBox3 = New System.Windows.Forms.PictureBox
            Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel
            Me.pGrid = New System.Windows.Forms.Panel
            Me.lbTechnical = New System.Windows.Forms.Label
            Me.m_lblNetVersion = New System.Windows.Forms.Label
            Me.TableLayoutPanel1.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tlpDetails.SuspendLayout()
            CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TabControl1.SuspendLayout()
            Me.tpGeneral.SuspendLayout()
            Me.tpCredits.SuspendLayout()
            Me.TableLayoutPanel4.SuspendLayout()
            CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel5.SuspendLayout()
            Me.tpModules.SuspendLayout()
            Me.TableLayoutPanel6.SuspendLayout()
            CType(Me.PictureBox5, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel7.SuspendLayout()
            Me.tpTechnical.SuspendLayout()
            Me.TableLayoutPanel2.SuspendLayout()
            CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel3.SuspendLayout()
            Me.SuspendLayout()
            '
            'OKButton
            '
            resources.ApplyResources(Me.OKButton, "OKButton")
            Me.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.OKButton.Name = "OKButton"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.PictureBox1, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.tlpDetails, 1, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'PictureBox1
            '
            Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.PictureBox1.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            resources.ApplyResources(Me.PictureBox1, "PictureBox1")
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.TabStop = False
            '
            'tlpDetails
            '
            resources.ApplyResources(Me.tlpDetails, "tlpDetails")
            Me.tlpDetails.Controls.Add(Me.lbTitle, 0, 0)
            Me.tlpDetails.Controls.Add(Me.rtbDisclaimer, 0, 4)
            Me.tlpDetails.Controls.Add(Me.lbVersion, 0, 1)
            Me.tlpDetails.Controls.Add(Me.lbCopyright, 0, 2)
            Me.tlpDetails.Controls.Add(Me.rtbDistribution, 0, 5)
            Me.tlpDetails.Controls.Add(Me.PictureBox2, 0, 6)
            Me.tlpDetails.Name = "tlpDetails"
            '
            'lbTitle
            '
            resources.ApplyResources(Me.lbTitle, "lbTitle")
            Me.lbTitle.Name = "lbTitle"
            '
            'rtbDisclaimer
            '
            Me.rtbDisclaimer.BackColor = System.Drawing.SystemColors.Control
            Me.rtbDisclaimer.BorderStyle = System.Windows.Forms.BorderStyle.None
            resources.ApplyResources(Me.rtbDisclaimer, "rtbDisclaimer")
            Me.rtbDisclaimer.Name = "rtbDisclaimer"
            '
            'lbVersion
            '
            resources.ApplyResources(Me.lbVersion, "lbVersion")
            Me.lbVersion.Name = "lbVersion"
            '
            'lbCopyright
            '
            resources.ApplyResources(Me.lbCopyright, "lbCopyright")
            Me.lbCopyright.Name = "lbCopyright"
            '
            'rtbDistribution
            '
            Me.rtbDistribution.BackColor = System.Drawing.SystemColors.Control
            Me.rtbDistribution.BorderStyle = System.Windows.Forms.BorderStyle.None
            resources.ApplyResources(Me.rtbDistribution, "rtbDistribution")
            Me.rtbDistribution.Name = "rtbDistribution"
            '
            'PictureBox2
            '
            Me.PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.PictureBox2.Image = Global.ScientificInterface.My.Resources.Resources.Sponsors
            resources.ApplyResources(Me.PictureBox2, "PictureBox2")
            Me.PictureBox2.Name = "PictureBox2"
            Me.PictureBox2.TabStop = False
            '
            'TabControl1
            '
            resources.ApplyResources(Me.TabControl1, "TabControl1")
            Me.TabControl1.Controls.Add(Me.tpGeneral)
            Me.TabControl1.Controls.Add(Me.tpCredits)
            Me.TabControl1.Controls.Add(Me.tpModules)
            Me.TabControl1.Controls.Add(Me.tpTechnical)
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.SelectedIndex = 0
            '
            'tpGeneral
            '
            Me.tpGeneral.Controls.Add(Me.TableLayoutPanel1)
            resources.ApplyResources(Me.tpGeneral, "tpGeneral")
            Me.tpGeneral.Name = "tpGeneral"
            Me.tpGeneral.UseVisualStyleBackColor = True
            '
            'tpCredits
            '
            Me.tpCredits.Controls.Add(Me.TableLayoutPanel4)
            resources.ApplyResources(Me.tpCredits, "tpCredits")
            Me.tpCredits.Name = "tpCredits"
            Me.tpCredits.UseVisualStyleBackColor = True
            '
            'TableLayoutPanel4
            '
            resources.ApplyResources(Me.TableLayoutPanel4, "TableLayoutPanel4")
            Me.TableLayoutPanel4.Controls.Add(Me.PictureBox4, 0, 0)
            Me.TableLayoutPanel4.Controls.Add(Me.TableLayoutPanel5, 1, 0)
            Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
            '
            'PictureBox4
            '
            Me.PictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.PictureBox4, "PictureBox4")
            Me.PictureBox4.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.PictureBox4.Name = "PictureBox4"
            Me.PictureBox4.TabStop = False
            '
            'TableLayoutPanel5
            '
            resources.ApplyResources(Me.TableLayoutPanel5, "TableLayoutPanel5")
            Me.TableLayoutPanel5.Controls.Add(Me.rtbCredits, 0, 0)
            Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
            '
            'rtbCredits
            '
            Me.rtbCredits.BackColor = System.Drawing.SystemColors.Control
            Me.rtbCredits.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.rtbCredits, "rtbCredits")
            Me.rtbCredits.Name = "rtbCredits"
            '
            'tpModules
            '
            Me.tpModules.Controls.Add(Me.TableLayoutPanel6)
            resources.ApplyResources(Me.tpModules, "tpModules")
            Me.tpModules.Name = "tpModules"
            Me.tpModules.UseVisualStyleBackColor = True
            '
            'TableLayoutPanel6
            '
            resources.ApplyResources(Me.TableLayoutPanel6, "TableLayoutPanel6")
            Me.TableLayoutPanel6.Controls.Add(Me.PictureBox5, 0, 0)
            Me.TableLayoutPanel6.Controls.Add(Me.TableLayoutPanel7, 1, 0)
            Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
            '
            'PictureBox5
            '
            Me.PictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.PictureBox5, "PictureBox5")
            Me.PictureBox5.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.PictureBox5.Name = "PictureBox5"
            Me.PictureBox5.TabStop = False
            '
            'TableLayoutPanel7
            '
            resources.ApplyResources(Me.TableLayoutPanel7, "TableLayoutPanel7")
            Me.TableLayoutPanel7.Controls.Add(Me.rtbModules, 0, 0)
            Me.TableLayoutPanel7.Name = "TableLayoutPanel7"
            '
            'rtbModules
            '
            Me.rtbModules.BackColor = System.Drawing.SystemColors.Control
            Me.rtbModules.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.rtbModules, "rtbModules")
            Me.rtbModules.Name = "rtbModules"
            '
            'tpTechnical
            '
            Me.tpTechnical.Controls.Add(Me.TableLayoutPanel2)
            resources.ApplyResources(Me.tpTechnical, "tpTechnical")
            Me.tpTechnical.Name = "tpTechnical"
            Me.tpTechnical.UseVisualStyleBackColor = True
            '
            'TableLayoutPanel2
            '
            resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
            Me.TableLayoutPanel2.Controls.Add(Me.PictureBox3, 0, 0)
            Me.TableLayoutPanel2.Controls.Add(Me.TableLayoutPanel3, 1, 0)
            Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
            '
            'PictureBox3
            '
            Me.PictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.PictureBox3, "PictureBox3")
            Me.PictureBox3.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.PictureBox3.Name = "PictureBox3"
            Me.PictureBox3.TabStop = False
            '
            'TableLayoutPanel3
            '
            resources.ApplyResources(Me.TableLayoutPanel3, "TableLayoutPanel3")
            Me.TableLayoutPanel3.Controls.Add(Me.pGrid, 0, 1)
            Me.TableLayoutPanel3.Controls.Add(Me.lbTechnical, 0, 0)
            Me.TableLayoutPanel3.Controls.Add(Me.m_lblNetVersion, 0, 2)
            Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
            '
            'pGrid
            '
            resources.ApplyResources(Me.pGrid, "pGrid")
            Me.pGrid.Name = "pGrid"
            '
            'lbTechnical
            '
            resources.ApplyResources(Me.lbTechnical, "lbTechnical")
            Me.lbTechnical.Name = "lbTechnical"
            '
            'm_lblNetVersion
            '
            resources.ApplyResources(Me.m_lblNetVersion, "m_lblNetVersion")
            Me.m_lblNetVersion.Name = "m_lblNetVersion"
            '
            'frmAboutEwE
            '
            Me.AcceptButton = Me.OKButton
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.OKButton
            Me.ControlBox = False
            Me.Controls.Add(Me.TabControl1)
            Me.Controls.Add(Me.OKButton)
            Me.DoubleBuffered = True
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmAboutEwE"
            Me.ShowInTaskbar = False
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tlpDetails.ResumeLayout(False)
            CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TabControl1.ResumeLayout(False)
            Me.tpGeneral.ResumeLayout(False)
            Me.tpCredits.ResumeLayout(False)
            Me.TableLayoutPanel4.ResumeLayout(False)
            Me.TableLayoutPanel4.PerformLayout()
            CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel5.ResumeLayout(False)
            Me.tpModules.ResumeLayout(False)
            Me.TableLayoutPanel6.ResumeLayout(False)
            Me.TableLayoutPanel6.PerformLayout()
            CType(Me.PictureBox5, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel7.ResumeLayout(False)
            Me.tpTechnical.ResumeLayout(False)
            Me.TableLayoutPanel2.ResumeLayout(False)
            Me.TableLayoutPanel2.PerformLayout()
            CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel3.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents tlpDetails As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents lbTitle As System.Windows.Forms.Label
        Friend WithEvents lbVersion As System.Windows.Forms.Label
        Friend WithEvents lbCopyright As System.Windows.Forms.Label
        Friend WithEvents rtbDisclaimer As System.Windows.Forms.RichTextBox
        Friend WithEvents rtbDistribution As System.Windows.Forms.RichTextBox
        Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
        Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
        Friend WithEvents tpGeneral As System.Windows.Forms.TabPage
        Friend WithEvents tpTechnical As System.Windows.Forms.TabPage
        Friend WithEvents pGrid As System.Windows.Forms.Panel
        Friend WithEvents tpCredits As System.Windows.Forms.TabPage
        Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents PictureBox3 As System.Windows.Forms.PictureBox
        Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents lbTechnical As System.Windows.Forms.Label
        Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents PictureBox4 As System.Windows.Forms.PictureBox
        Friend WithEvents TableLayoutPanel5 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents rtbCredits As System.Windows.Forms.RichTextBox
        Friend WithEvents tpModules As System.Windows.Forms.TabPage
        Friend WithEvents TableLayoutPanel6 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents PictureBox5 As System.Windows.Forms.PictureBox
        Friend WithEvents TableLayoutPanel7 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents rtbModules As System.Windows.Forms.RichTextBox
        Friend WithEvents m_lblNetVersion As System.Windows.Forms.Label

    End Class
End Namespace

