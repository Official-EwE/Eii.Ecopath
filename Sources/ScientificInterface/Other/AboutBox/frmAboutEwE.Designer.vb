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


        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAboutEwE))
            Me.m_btnOK = New System.Windows.Forms.Button
            Me.m_tlpGeneral = New System.Windows.Forms.TableLayoutPanel
            Me.m_pbFish0 = New System.Windows.Forms.PictureBox
            Me.m_tlpDetails = New System.Windows.Forms.TableLayoutPanel
            Me.m_lbTitle = New System.Windows.Forms.Label
            Me.m_rtbDisclaimer = New System.Windows.Forms.RichTextBox
            Me.m_lbVersion = New System.Windows.Forms.Label
            Me.m_lbCopyright = New System.Windows.Forms.Label
            Me.m_rtbDistribution = New System.Windows.Forms.RichTextBox
            Me.m_pbSponsors = New System.Windows.Forms.PictureBox
            Me.m_tcMain = New System.Windows.Forms.TabControl
            Me.m_tpGeneral = New System.Windows.Forms.TabPage
            Me.m_tpCredits = New System.Windows.Forms.TabPage
            Me.m_tlpCredits = New System.Windows.Forms.TableLayoutPanel
            Me.m_rtbCredits = New System.Windows.Forms.RichTextBox
            Me.m_pbFish1 = New System.Windows.Forms.PictureBox
            Me.m_tpModules = New System.Windows.Forms.TabPage
            Me.m_tlpModules = New System.Windows.Forms.TableLayoutPanel
            Me.m_rtbModules = New System.Windows.Forms.RichTextBox
            Me.m_pbFish2 = New System.Windows.Forms.PictureBox
            Me.m_tpTechnical = New System.Windows.Forms.TabPage
            Me.m_tlpTechnical = New System.Windows.Forms.TableLayoutPanel
            Me.m_pbFish3 = New System.Windows.Forms.PictureBox
            Me.m_tlpTechnicalDetails = New System.Windows.Forms.TableLayoutPanel
            Me.m_lbTechnical = New System.Windows.Forms.Label
            Me.m_lblNetVersion = New System.Windows.Forms.Label
            Me.m_gridTechnical = New ScientificInterface.AboutEwEGrid
            Me.m_tlpGeneral.SuspendLayout()
            CType(Me.m_pbFish0, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpDetails.SuspendLayout()
            CType(Me.m_pbSponsors, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tcMain.SuspendLayout()
            Me.m_tpGeneral.SuspendLayout()
            Me.m_tpCredits.SuspendLayout()
            Me.m_tlpCredits.SuspendLayout()
            CType(Me.m_pbFish1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tpModules.SuspendLayout()
            Me.m_tlpModules.SuspendLayout()
            CType(Me.m_pbFish2, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tpTechnical.SuspendLayout()
            Me.m_tlpTechnical.SuspendLayout()
            CType(Me.m_pbFish3, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpTechnicalDetails.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnOK
            '
            resources.ApplyResources(Me.m_btnOK, "m_btnOK")
            Me.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnOK.Name = "m_btnOK"
            '
            'm_tlpGeneral
            '
            resources.ApplyResources(Me.m_tlpGeneral, "m_tlpGeneral")
            Me.m_tlpGeneral.Controls.Add(Me.m_pbFish0, 0, 0)
            Me.m_tlpGeneral.Controls.Add(Me.m_tlpDetails, 1, 0)
            Me.m_tlpGeneral.Name = "m_tlpGeneral"
            '
            'm_pbFish0
            '
            Me.m_pbFish0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_pbFish0.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            resources.ApplyResources(Me.m_pbFish0, "m_pbFish0")
            Me.m_pbFish0.Name = "m_pbFish0"
            Me.m_pbFish0.TabStop = False
            '
            'm_tlpDetails
            '
            resources.ApplyResources(Me.m_tlpDetails, "m_tlpDetails")
            Me.m_tlpDetails.Controls.Add(Me.m_lbTitle, 0, 0)
            Me.m_tlpDetails.Controls.Add(Me.m_rtbDisclaimer, 0, 4)
            Me.m_tlpDetails.Controls.Add(Me.m_lbVersion, 0, 1)
            Me.m_tlpDetails.Controls.Add(Me.m_lbCopyright, 0, 2)
            Me.m_tlpDetails.Controls.Add(Me.m_rtbDistribution, 0, 5)
            Me.m_tlpDetails.Controls.Add(Me.m_pbSponsors, 0, 6)
            Me.m_tlpDetails.Name = "m_tlpDetails"
            '
            'm_lbTitle
            '
            resources.ApplyResources(Me.m_lbTitle, "m_lbTitle")
            Me.m_lbTitle.Name = "m_lbTitle"
            '
            'm_rtbDisclaimer
            '
            Me.m_rtbDisclaimer.BackColor = System.Drawing.SystemColors.Control
            Me.m_rtbDisclaimer.BorderStyle = System.Windows.Forms.BorderStyle.None
            resources.ApplyResources(Me.m_rtbDisclaimer, "m_rtbDisclaimer")
            Me.m_rtbDisclaimer.Name = "m_rtbDisclaimer"
            '
            'm_lbVersion
            '
            resources.ApplyResources(Me.m_lbVersion, "m_lbVersion")
            Me.m_lbVersion.Name = "m_lbVersion"
            '
            'm_lbCopyright
            '
            resources.ApplyResources(Me.m_lbCopyright, "m_lbCopyright")
            Me.m_lbCopyright.Name = "m_lbCopyright"
            '
            'm_rtbDistribution
            '
            Me.m_rtbDistribution.BackColor = System.Drawing.SystemColors.Control
            Me.m_rtbDistribution.BorderStyle = System.Windows.Forms.BorderStyle.None
            resources.ApplyResources(Me.m_rtbDistribution, "m_rtbDistribution")
            Me.m_rtbDistribution.Name = "m_rtbDistribution"
            '
            'm_pbSponsors
            '
            Me.m_pbSponsors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_pbSponsors.Image = Global.ScientificInterface.My.Resources.Resources.Sponsors
            resources.ApplyResources(Me.m_pbSponsors, "m_pbSponsors")
            Me.m_pbSponsors.Name = "m_pbSponsors"
            Me.m_pbSponsors.TabStop = False
            '
            'm_tcMain
            '
            resources.ApplyResources(Me.m_tcMain, "m_tcMain")
            Me.m_tcMain.Controls.Add(Me.m_tpGeneral)
            Me.m_tcMain.Controls.Add(Me.m_tpCredits)
            Me.m_tcMain.Controls.Add(Me.m_tpModules)
            Me.m_tcMain.Controls.Add(Me.m_tpTechnical)
            Me.m_tcMain.Name = "m_tcMain"
            Me.m_tcMain.SelectedIndex = 0
            '
            'm_tpGeneral
            '
            Me.m_tpGeneral.Controls.Add(Me.m_tlpGeneral)
            resources.ApplyResources(Me.m_tpGeneral, "m_tpGeneral")
            Me.m_tpGeneral.Name = "m_tpGeneral"
            Me.m_tpGeneral.UseVisualStyleBackColor = True
            '
            'm_tpCredits
            '
            Me.m_tpCredits.Controls.Add(Me.m_tlpCredits)
            resources.ApplyResources(Me.m_tpCredits, "m_tpCredits")
            Me.m_tpCredits.Name = "m_tpCredits"
            Me.m_tpCredits.UseVisualStyleBackColor = True
            '
            'm_tlpCredits
            '
            resources.ApplyResources(Me.m_tlpCredits, "m_tlpCredits")
            Me.m_tlpCredits.Controls.Add(Me.m_rtbCredits, 1, 0)
            Me.m_tlpCredits.Controls.Add(Me.m_pbFish1, 0, 0)
            Me.m_tlpCredits.Name = "m_tlpCredits"
            '
            'm_rtbCredits
            '
            Me.m_rtbCredits.BackColor = System.Drawing.SystemColors.Control
            Me.m_rtbCredits.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.m_rtbCredits, "m_rtbCredits")
            Me.m_rtbCredits.Name = "m_rtbCredits"
            '
            'm_pbFish1
            '
            resources.ApplyResources(Me.m_pbFish1, "m_pbFish1")
            Me.m_pbFish1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_pbFish1.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.m_pbFish1.Name = "m_pbFish1"
            Me.m_pbFish1.TabStop = False
            '
            'm_tpModules
            '
            Me.m_tpModules.Controls.Add(Me.m_tlpModules)
            resources.ApplyResources(Me.m_tpModules, "m_tpModules")
            Me.m_tpModules.Name = "m_tpModules"
            Me.m_tpModules.UseVisualStyleBackColor = True
            '
            'm_tlpModules
            '
            resources.ApplyResources(Me.m_tlpModules, "m_tlpModules")
            Me.m_tlpModules.Controls.Add(Me.m_rtbModules, 1, 0)
            Me.m_tlpModules.Controls.Add(Me.m_pbFish2, 0, 0)
            Me.m_tlpModules.Name = "m_tlpModules"
            '
            'm_rtbModules
            '
            Me.m_rtbModules.BackColor = System.Drawing.SystemColors.Control
            Me.m_rtbModules.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.m_rtbModules, "m_rtbModules")
            Me.m_rtbModules.Name = "m_rtbModules"
            '
            'm_pbFish2
            '
            Me.m_pbFish2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.m_pbFish2, "m_pbFish2")
            Me.m_pbFish2.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.m_pbFish2.Name = "m_pbFish2"
            Me.m_pbFish2.TabStop = False
            '
            'm_tpTechnical
            '
            Me.m_tpTechnical.Controls.Add(Me.m_tlpTechnical)
            resources.ApplyResources(Me.m_tpTechnical, "m_tpTechnical")
            Me.m_tpTechnical.Name = "m_tpTechnical"
            Me.m_tpTechnical.UseVisualStyleBackColor = True
            '
            'm_tlpTechnical
            '
            resources.ApplyResources(Me.m_tlpTechnical, "m_tlpTechnical")
            Me.m_tlpTechnical.Controls.Add(Me.m_pbFish3, 0, 0)
            Me.m_tlpTechnical.Controls.Add(Me.m_tlpTechnicalDetails, 1, 0)
            Me.m_tlpTechnical.Name = "m_tlpTechnical"
            '
            'm_pbFish3
            '
            Me.m_pbFish3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.m_pbFish3, "m_pbFish3")
            Me.m_pbFish3.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.m_pbFish3.Name = "m_pbFish3"
            Me.m_pbFish3.TabStop = False
            '
            'm_tlpTechnicalDetails
            '
            resources.ApplyResources(Me.m_tlpTechnicalDetails, "m_tlpTechnicalDetails")
            Me.m_tlpTechnicalDetails.Controls.Add(Me.m_lbTechnical, 0, 0)
            Me.m_tlpTechnicalDetails.Controls.Add(Me.m_lblNetVersion, 0, 2)
            Me.m_tlpTechnicalDetails.Controls.Add(Me.m_gridTechnical, 0, 1)
            Me.m_tlpTechnicalDetails.Name = "m_tlpTechnicalDetails"
            '
            'm_lbTechnical
            '
            resources.ApplyResources(Me.m_lbTechnical, "m_lbTechnical")
            Me.m_lbTechnical.Name = "m_lbTechnical"
            '
            'm_lblNetVersion
            '
            resources.ApplyResources(Me.m_lblNetVersion, "m_lblNetVersion")
            Me.m_lblNetVersion.Name = "m_lblNetVersion"
            '
            'm_gridTechnical
            '
            resources.ApplyResources(Me.m_gridTechnical, "m_gridTechnical")
            Me.m_gridTechnical.AutoSizeMinHeight = 10
            Me.m_gridTechnical.AutoSizeMinWidth = 10
            Me.m_gridTechnical.AutoStretchColumnsToFitWidth = False
            Me.m_gridTechnical.AutoStretchRowsToFitHeight = False
            Me.m_gridTechnical.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_gridTechnical.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
            Me.m_gridTechnical.CustomSort = False
            Me.m_gridTechnical.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridTechnical.GridToolTipActive = True
            Me.m_gridTechnical.Name = "m_gridTechnical"
            Me.m_gridTechnical.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            '
            'frmAboutEwE
            '
            Me.AcceptButton = Me.m_btnOK
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnOK
            Me.ControlBox = False
            Me.Controls.Add(Me.m_tcMain)
            Me.Controls.Add(Me.m_btnOK)
            Me.DoubleBuffered = True
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmAboutEwE"
            Me.ShowInTaskbar = False
            Me.m_tlpGeneral.ResumeLayout(False)
            Me.m_tlpGeneral.PerformLayout()
            CType(Me.m_pbFish0, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpDetails.ResumeLayout(False)
            CType(Me.m_pbSponsors, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tcMain.ResumeLayout(False)
            Me.m_tpGeneral.ResumeLayout(False)
            Me.m_tpCredits.ResumeLayout(False)
            Me.m_tlpCredits.ResumeLayout(False)
            Me.m_tlpCredits.PerformLayout()
            CType(Me.m_pbFish1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tpModules.ResumeLayout(False)
            Me.m_tlpModules.ResumeLayout(False)
            Me.m_tlpModules.PerformLayout()
            CType(Me.m_pbFish2, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tpTechnical.ResumeLayout(False)
            Me.m_tlpTechnical.ResumeLayout(False)
            Me.m_tlpTechnical.PerformLayout()
            CType(Me.m_pbFish3, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpTechnicalDetails.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_pbFish3 As System.Windows.Forms.PictureBox
        Private WithEvents m_lbTechnical As System.Windows.Forms.Label
        Private WithEvents m_lblNetVersion As System.Windows.Forms.Label
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        'Private WithEvents m_gridTechnical As AboutEwEGrid
        Private WithEvents m_rtbModules As System.Windows.Forms.RichTextBox
        Private WithEvents m_pbFish2 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbFish1 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbFish0 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbSponsors As System.Windows.Forms.PictureBox
        Private WithEvents m_rtbDistribution As System.Windows.Forms.RichTextBox
        Private WithEvents m_rtbDisclaimer As System.Windows.Forms.RichTextBox
        Private WithEvents m_lbTitle As System.Windows.Forms.Label
        Private WithEvents m_lbVersion As System.Windows.Forms.Label
        Private WithEvents m_lbCopyright As System.Windows.Forms.Label
        Private WithEvents m_tcMain As System.Windows.Forms.TabControl
        Private WithEvents m_tlpDetails As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpTechnicalDetails As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_rtbCredits As System.Windows.Forms.RichTextBox
        Private WithEvents m_tlpGeneral As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpCredits As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpModules As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpTechnical As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tpGeneral As System.Windows.Forms.TabPage
        Private WithEvents m_tpCredits As System.Windows.Forms.TabPage
        Private WithEvents m_tpModules As System.Windows.Forms.TabPage
        Private WithEvents m_tpTechnical As System.Windows.Forms.TabPage
        Private WithEvents m_gridTechnical As ScientificInterface.AboutEwEGrid

    End Class
End Namespace

