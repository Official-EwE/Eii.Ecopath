Namespace Ecotracer

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmEcotracerParameters
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcotracerParameters))
            Me.gbDetails = New System.Windows.Forms.GroupBox
            Me.m_tbContact = New System.Windows.Forms.TextBox
            Me.m_tbAuthor = New System.Windows.Forms.TextBox
            Me.m_lbContact = New System.Windows.Forms.Label
            Me.m_lbAuthor = New System.Windows.Forms.Label
            Me.m_tbName = New System.Windows.Forms.TextBox
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.lblDescription = New System.Windows.Forms.Label
            Me.lbScenarioName = New System.Windows.Forms.Label
            Me.lblScenario = New System.Windows.Forms.Label
            Me.lblSponsors = New System.Windows.Forms.Label
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.PictureBox1 = New System.Windows.Forms.PictureBox
            Me.PictureBox2 = New System.Windows.Forms.PictureBox
            Me.PictureBox3 = New System.Windows.Forms.PictureBox
            Me.PictureBox4 = New System.Windows.Forms.PictureBox
            Me.lblInitialization = New System.Windows.Forms.Label
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.rbSpace = New System.Windows.Forms.RadioButton
            Me.rbSim = New System.Windows.Forms.RadioButton
            Me.rbDisabled = New System.Windows.Forms.RadioButton
            Me.gbDetails.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox2.SuspendLayout()
            Me.SuspendLayout()
            '
            'gbDetails
            '
            resources.ApplyResources(Me.gbDetails, "gbDetails")
            Me.gbDetails.Controls.Add(Me.m_tbContact)
            Me.gbDetails.Controls.Add(Me.m_tbAuthor)
            Me.gbDetails.Controls.Add(Me.m_lbContact)
            Me.gbDetails.Controls.Add(Me.m_lbAuthor)
            Me.gbDetails.Controls.Add(Me.m_tbName)
            Me.gbDetails.Controls.Add(Me.m_tbDescription)
            Me.gbDetails.Controls.Add(Me.lblDescription)
            Me.gbDetails.Controls.Add(Me.lbScenarioName)
            Me.gbDetails.Name = "gbDetails"
            Me.gbDetails.TabStop = False
            '
            'm_tbContact
            '
            resources.ApplyResources(Me.m_tbContact, "m_tbContact")
            Me.m_tbContact.Name = "m_tbContact"
            '
            'm_tbAuthor
            '
            resources.ApplyResources(Me.m_tbAuthor, "m_tbAuthor")
            Me.m_tbAuthor.Name = "m_tbAuthor"
            '
            'm_lbContact
            '
            resources.ApplyResources(Me.m_lbContact, "m_lbContact")
            Me.m_lbContact.Name = "m_lbContact"
            '
            'm_lbAuthor
            '
            resources.ApplyResources(Me.m_lbAuthor, "m_lbAuthor")
            Me.m_lbAuthor.Name = "m_lbAuthor"
            '
            'm_tbName
            '
            resources.ApplyResources(Me.m_tbName, "m_tbName")
            Me.m_tbName.Name = "m_tbName"
            '
            'm_tbDescription
            '
            resources.ApplyResources(Me.m_tbDescription, "m_tbDescription")
            Me.m_tbDescription.Name = "m_tbDescription"
            '
            'lblDescription
            '
            resources.ApplyResources(Me.lblDescription, "lblDescription")
            Me.lblDescription.Name = "lblDescription"
            '
            'lbScenarioName
            '
            resources.ApplyResources(Me.lbScenarioName, "lbScenarioName")
            Me.lbScenarioName.Name = "lbScenarioName"
            '
            'lblScenario
            '
            resources.ApplyResources(Me.lblScenario, "lblScenario")
            Me.lblScenario.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblScenario.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblScenario.Name = "lblScenario"
            '
            'lblSponsors
            '
            resources.ApplyResources(Me.lblSponsors, "lblSponsors")
            Me.lblSponsors.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblSponsors.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblSponsors.Name = "lblSponsors"
            '
            'GroupBox1
            '
            resources.ApplyResources(Me.GroupBox1, "GroupBox1")
            Me.GroupBox1.Controls.Add(Me.TableLayoutPanel1)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.TabStop = False
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.BackColor = System.Drawing.Color.White
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.PictureBox1, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.PictureBox2, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.PictureBox3, 2, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.PictureBox4, 3, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'PictureBox1
            '
            resources.ApplyResources(Me.PictureBox1, "PictureBox1")
            Me.PictureBox1.Image = Global.ScientificInterface.My.Resources.Resources.fimr_logo_50px
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.TabStop = False
            '
            'PictureBox2
            '
            resources.ApplyResources(Me.PictureBox2, "PictureBox2")
            Me.PictureBox2.Image = Global.ScientificInterface.My.Resources.Resources.EU_50px
            Me.PictureBox2.Name = "PictureBox2"
            Me.PictureBox2.TabStop = False
            '
            'PictureBox3
            '
            resources.ApplyResources(Me.PictureBox3, "PictureBox3")
            Me.PictureBox3.Image = Global.ScientificInterface.My.Resources.Resources.Lenfest_Logo_50px
            Me.PictureBox3.Name = "PictureBox3"
            Me.PictureBox3.TabStop = False
            '
            'PictureBox4
            '
            resources.ApplyResources(Me.PictureBox4, "PictureBox4")
            Me.PictureBox4.Image = Global.ScientificInterface.My.Resources.Resources.sautxt_50px
            Me.PictureBox4.Name = "PictureBox4"
            Me.PictureBox4.TabStop = False
            '
            'lblInitialization
            '
            resources.ApplyResources(Me.lblInitialization, "lblInitialization")
            Me.lblInitialization.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitialization.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitialization.Name = "lblInitialization"
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.rbSpace)
            Me.GroupBox2.Controls.Add(Me.rbSim)
            Me.GroupBox2.Controls.Add(Me.rbDisabled)
            resources.ApplyResources(Me.GroupBox2, "GroupBox2")
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.TabStop = False
            '
            'rbSpace
            '
            resources.ApplyResources(Me.rbSpace, "rbSpace")
            Me.rbSpace.Name = "rbSpace"
            Me.rbSpace.TabStop = True
            Me.rbSpace.UseVisualStyleBackColor = True
            '
            'rbSim
            '
            resources.ApplyResources(Me.rbSim, "rbSim")
            Me.rbSim.Name = "rbSim"
            Me.rbSim.TabStop = True
            Me.rbSim.UseVisualStyleBackColor = True
            '
            'rbDisabled
            '
            resources.ApplyResources(Me.rbDisabled, "rbDisabled")
            Me.rbDisabled.Checked = True
            Me.rbDisabled.Name = "rbDisabled"
            Me.rbDisabled.TabStop = True
            Me.rbDisabled.UseVisualStyleBackColor = True
            '
            'frmEcotracerParameters
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ControlBox = False
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.lblInitialization)
            Me.Controls.Add(Me.lblSponsors)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.gbDetails)
            Me.Controls.Add(Me.lblScenario)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmEcotracerParameters"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.gbDetails.ResumeLayout(False)
            Me.gbDetails.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents gbDetails As System.Windows.Forms.GroupBox
        Friend WithEvents m_tbContact As System.Windows.Forms.TextBox
        Friend WithEvents m_tbAuthor As System.Windows.Forms.TextBox
        Friend WithEvents m_lbContact As System.Windows.Forms.Label
        Friend WithEvents m_lbAuthor As System.Windows.Forms.Label
        Friend WithEvents m_tbName As System.Windows.Forms.TextBox
        Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Friend WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents lbScenarioName As System.Windows.Forms.Label
        Friend WithEvents lblScenario As System.Windows.Forms.Label
        Friend WithEvents lblSponsors As System.Windows.Forms.Label
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
        Friend WithEvents PictureBox3 As System.Windows.Forms.PictureBox
        Friend WithEvents PictureBox4 As System.Windows.Forms.PictureBox
        Friend WithEvents lblInitialization As System.Windows.Forms.Label
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents rbDisabled As System.Windows.Forms.RadioButton
        Friend WithEvents rbSpace As System.Windows.Forms.RadioButton
        Friend WithEvents rbSim As System.Windows.Forms.RadioButton
    End Class

End Namespace
