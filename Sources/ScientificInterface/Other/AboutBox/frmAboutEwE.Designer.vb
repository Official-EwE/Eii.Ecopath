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
            Me.m_tcMain = New System.Windows.Forms.TabControl
            Me.m_tpGeneral = New System.Windows.Forms.TabPage
            Me.m_tpTeam = New System.Windows.Forms.TabPage
            Me.m_tlpCredits = New System.Windows.Forms.TableLayoutPanel
            Me.m_rtbTeam = New System.Windows.Forms.RichTextBox
            Me.m_pbFish1 = New System.Windows.Forms.PictureBox
            Me.m_tpacknowledgements = New System.Windows.Forms.TabPage
            Me.m_tlpModules = New System.Windows.Forms.TableLayoutPanel
            Me.m_rtbAcknowledgements = New System.Windows.Forms.RichTextBox
            Me.m_pbFish2 = New System.Windows.Forms.PictureBox
            Me.m_tpTechnical = New System.Windows.Forms.TabPage
            Me.m_tlpTechnical = New System.Windows.Forms.TableLayoutPanel
            Me.m_pbFish3 = New System.Windows.Forms.PictureBox
            Me.m_tlpTechnicalDetails = New System.Windows.Forms.TableLayoutPanel
            Me.m_lbTechnical = New System.Windows.Forms.Label
            Me.m_lblNetVersion = New System.Windows.Forms.Label
            Me.m_gridTechnical = New ScientificInterface.AboutEwEGrid
            Me.m_tpDatabase = New System.Windows.Forms.TabPage
            Me.m_tlpDatabase = New System.Windows.Forms.TableLayoutPanel
            Me.m_pbFish4 = New System.Windows.Forms.PictureBox
            Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel
            Me.m_lblDatabase = New System.Windows.Forms.Label
            Me.m_gridDatabase = New ScientificInterface.DatabaseGrid
            Me.m_pbSponsors = New System.Windows.Forms.PictureBox
            Me.m_tlpGeneral.SuspendLayout()
            CType(Me.m_pbFish0, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpDetails.SuspendLayout()
            Me.m_tcMain.SuspendLayout()
            Me.m_tpGeneral.SuspendLayout()
            Me.m_tpTeam.SuspendLayout()
            Me.m_tlpCredits.SuspendLayout()
            CType(Me.m_pbFish1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tpacknowledgements.SuspendLayout()
            Me.m_tlpModules.SuspendLayout()
            CType(Me.m_pbFish2, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tpTechnical.SuspendLayout()
            Me.m_tlpTechnical.SuspendLayout()
            CType(Me.m_pbFish3, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpTechnicalDetails.SuspendLayout()
            Me.m_tpDatabase.SuspendLayout()
            Me.m_tlpDatabase.SuspendLayout()
            CType(Me.m_pbFish4, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel2.SuspendLayout()
            CType(Me.m_pbSponsors, System.ComponentModel.ISupportInitialize).BeginInit()
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
            'm_tcMain
            '
            resources.ApplyResources(Me.m_tcMain, "m_tcMain")
            Me.m_tcMain.Controls.Add(Me.m_tpGeneral)
            Me.m_tcMain.Controls.Add(Me.m_tpTeam)
            Me.m_tcMain.Controls.Add(Me.m_tpacknowledgements)
            Me.m_tcMain.Controls.Add(Me.m_tpTechnical)
            Me.m_tcMain.Controls.Add(Me.m_tpDatabase)
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
            'm_tpTeam
            '
            Me.m_tpTeam.Controls.Add(Me.m_tlpCredits)
            resources.ApplyResources(Me.m_tpTeam, "m_tpTeam")
            Me.m_tpTeam.Name = "m_tpTeam"
            Me.m_tpTeam.UseVisualStyleBackColor = True
            '
            'm_tlpCredits
            '
            resources.ApplyResources(Me.m_tlpCredits, "m_tlpCredits")
            Me.m_tlpCredits.Controls.Add(Me.m_rtbTeam, 1, 0)
            Me.m_tlpCredits.Controls.Add(Me.m_pbFish1, 0, 0)
            Me.m_tlpCredits.Name = "m_tlpCredits"
            '
            'm_rtbTeam
            '
            Me.m_rtbTeam.BackColor = System.Drawing.SystemColors.Control
            Me.m_rtbTeam.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.m_rtbTeam, "m_rtbTeam")
            Me.m_rtbTeam.Name = "m_rtbTeam"
            '
            'm_pbFish1
            '
            resources.ApplyResources(Me.m_pbFish1, "m_pbFish1")
            Me.m_pbFish1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_pbFish1.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.m_pbFish1.Name = "m_pbFish1"
            Me.m_pbFish1.TabStop = False
            '
            'm_tpacknowledgements
            '
            Me.m_tpacknowledgements.Controls.Add(Me.m_tlpModules)
            resources.ApplyResources(Me.m_tpacknowledgements, "m_tpacknowledgements")
            Me.m_tpacknowledgements.Name = "m_tpacknowledgements"
            Me.m_tpacknowledgements.UseVisualStyleBackColor = True
            '
            'm_tlpModules
            '
            resources.ApplyResources(Me.m_tlpModules, "m_tlpModules")
            Me.m_tlpModules.Controls.Add(Me.m_rtbAcknowledgements, 1, 0)
            Me.m_tlpModules.Controls.Add(Me.m_pbFish2, 0, 0)
            Me.m_tlpModules.Name = "m_tlpModules"
            '
            'm_rtbAcknowledgements
            '
            Me.m_rtbAcknowledgements.BackColor = System.Drawing.SystemColors.Control
            Me.m_rtbAcknowledgements.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.m_rtbAcknowledgements, "m_rtbAcknowledgements")
            Me.m_rtbAcknowledgements.Name = "m_rtbAcknowledgements"
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
            Me.m_gridTechnical.AllowBlockSelect = False
            resources.ApplyResources(Me.m_gridTechnical, "m_gridTechnical")
            Me.m_gridTechnical.AutoSizeMinHeight = 10
            Me.m_gridTechnical.AutoSizeMinWidth = 10
            Me.m_gridTechnical.AutoStretchColumnsToFitWidth = False
            Me.m_gridTechnical.AutoStretchRowsToFitHeight = False
            Me.m_gridTechnical.BackColor = System.Drawing.Color.White
            Me.m_gridTechnical.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridTechnical.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridTechnical.CustomSort = False
            Me.m_gridTechnical.FixedColumnWidths = False
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
            Me.m_gridTechnical.UIContext = Nothing
            '
            'm_tpDatabase
            '
            Me.m_tpDatabase.Controls.Add(Me.m_tlpDatabase)
            resources.ApplyResources(Me.m_tpDatabase, "m_tpDatabase")
            Me.m_tpDatabase.Name = "m_tpDatabase"
            Me.m_tpDatabase.UseVisualStyleBackColor = True
            '
            'm_tlpDatabase
            '
            resources.ApplyResources(Me.m_tlpDatabase, "m_tlpDatabase")
            Me.m_tlpDatabase.Controls.Add(Me.m_pbFish4, 0, 0)
            Me.m_tlpDatabase.Controls.Add(Me.TableLayoutPanel2, 1, 0)
            Me.m_tlpDatabase.Name = "m_tlpDatabase"
            '
            'm_pbFish4
            '
            Me.m_pbFish4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.m_pbFish4, "m_pbFish4")
            Me.m_pbFish4.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.m_pbFish4.Name = "m_pbFish4"
            Me.m_pbFish4.TabStop = False
            '
            'TableLayoutPanel2
            '
            resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
            Me.TableLayoutPanel2.Controls.Add(Me.m_lblDatabase, 0, 0)
            Me.TableLayoutPanel2.Controls.Add(Me.m_gridDatabase, 0, 1)
            Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
            '
            'm_lblDatabase
            '
            resources.ApplyResources(Me.m_lblDatabase, "m_lblDatabase")
            Me.m_lblDatabase.Name = "m_lblDatabase"
            '
            'm_gridDatabase
            '
            Me.m_gridDatabase.AllowBlockSelect = False
            resources.ApplyResources(Me.m_gridDatabase, "m_gridDatabase")
            Me.m_gridDatabase.AutoSizeMinHeight = 10
            Me.m_gridDatabase.AutoSizeMinWidth = 10
            Me.m_gridDatabase.AutoStretchColumnsToFitWidth = False
            Me.m_gridDatabase.AutoStretchRowsToFitHeight = False
            Me.m_gridDatabase.BackColor = System.Drawing.Color.White
            Me.m_gridDatabase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridDatabase.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridDatabase.CustomSort = False
            Me.m_gridDatabase.FixedColumnWidths = False
            Me.m_gridDatabase.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridDatabase.GridToolTipActive = True
            Me.m_gridDatabase.Name = "m_gridDatabase"
            Me.m_gridDatabase.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridDatabase.UIContext = Nothing
            '
            'm_pbSponsors
            '
            Me.m_pbSponsors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_pbSponsors.Image = Global.ScientificInterface.My.Resources.Resources.Sponsors
            resources.ApplyResources(Me.m_pbSponsors, "m_pbSponsors")
            Me.m_pbSponsors.Name = "m_pbSponsors"
            Me.m_pbSponsors.TabStop = False
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
            Me.m_tcMain.ResumeLayout(False)
            Me.m_tpGeneral.ResumeLayout(False)
            Me.m_tpTeam.ResumeLayout(False)
            Me.m_tlpCredits.ResumeLayout(False)
            Me.m_tlpCredits.PerformLayout()
            CType(Me.m_pbFish1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tpacknowledgements.ResumeLayout(False)
            Me.m_tlpModules.ResumeLayout(False)
            Me.m_tlpModules.PerformLayout()
            CType(Me.m_pbFish2, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tpTechnical.ResumeLayout(False)
            Me.m_tlpTechnical.ResumeLayout(False)
            Me.m_tlpTechnical.PerformLayout()
            CType(Me.m_pbFish3, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpTechnicalDetails.ResumeLayout(False)
            Me.m_tpDatabase.ResumeLayout(False)
            Me.m_tlpDatabase.ResumeLayout(False)
            Me.m_tlpDatabase.PerformLayout()
            CType(Me.m_pbFish4, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel2.ResumeLayout(False)
            CType(Me.m_pbSponsors, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_pbFish3 As System.Windows.Forms.PictureBox
        Private WithEvents m_lbTechnical As System.Windows.Forms.Label
        Private WithEvents m_lblNetVersion As System.Windows.Forms.Label
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        Private WithEvents m_rtbAcknowledgements As System.Windows.Forms.RichTextBox
        Private WithEvents m_pbFish2 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbFish1 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbFish0 As System.Windows.Forms.PictureBox
        Private WithEvents m_rtbDistribution As System.Windows.Forms.RichTextBox
        Private WithEvents m_rtbDisclaimer As System.Windows.Forms.RichTextBox
        Private WithEvents m_lbTitle As System.Windows.Forms.Label
        Private WithEvents m_lbVersion As System.Windows.Forms.Label
        Private WithEvents m_lbCopyright As System.Windows.Forms.Label
        Private WithEvents m_tcMain As System.Windows.Forms.TabControl
        Private WithEvents m_tlpDetails As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpTechnicalDetails As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_rtbTeam As System.Windows.Forms.RichTextBox
        Private WithEvents m_tlpGeneral As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpCredits As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpModules As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tlpTechnical As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tpGeneral As System.Windows.Forms.TabPage
        Private WithEvents m_tpTeam As System.Windows.Forms.TabPage
        Private WithEvents m_tpacknowledgements As System.Windows.Forms.TabPage
        Private WithEvents m_tpTechnical As System.Windows.Forms.TabPage
        Private WithEvents m_gridTechnical As ScientificInterface.AboutEwEGrid
        Private WithEvents m_tpDatabase As System.Windows.Forms.TabPage
        Private WithEvents m_tlpDatabase As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_pbFish4 As System.Windows.Forms.PictureBox
        Private WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblDatabase As System.Windows.Forms.Label
        Private WithEvents m_gridDatabase As DatabaseGrid
        Private WithEvents m_pbSponsors As System.Windows.Forms.PictureBox

    End Class
End Namespace

