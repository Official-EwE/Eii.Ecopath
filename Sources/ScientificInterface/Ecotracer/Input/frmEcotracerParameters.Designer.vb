Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

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
            Me.m_gbDetails = New System.Windows.Forms.GroupBox
            Me.m_tbContact = New System.Windows.Forms.TextBox
            Me.m_tbAuthor = New System.Windows.Forms.TextBox
            Me.m_lbContact = New System.Windows.Forms.Label
            Me.m_lbAuthor = New System.Windows.Forms.Label
            Me.m_tbName = New System.Windows.Forms.TextBox
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_lbScenario = New System.Windows.Forms.Label
            Me.m_hdrScenario = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_hdrSponsors = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_tlpSponsors = New System.Windows.Forms.TableLayoutPanel
            Me.m_pbSponsor1 = New System.Windows.Forms.PictureBox
            Me.m_pbSponsor2 = New System.Windows.Forms.PictureBox
            Me.m_pbSponsor3 = New System.Windows.Forms.PictureBox
            Me.m_pbSponsor4 = New System.Windows.Forms.PictureBox
            Me.m_hdrInitialization = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_gbInit = New System.Windows.Forms.GroupBox
            Me.m_rbSpace = New System.Windows.Forms.RadioButton
            Me.m_rbSim = New System.Windows.Forms.RadioButton
            Me.m_rbDisabled = New System.Windows.Forms.RadioButton
            Me.m_gbDetails.SuspendLayout()
            Me.m_tlpSponsors.SuspendLayout()
            CType(Me.m_pbSponsor1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbSponsor2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbSponsor3, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbSponsor4, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbInit.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_gbDetails
            '
            resources.ApplyResources(Me.m_gbDetails, "m_gbDetails")
            Me.m_gbDetails.Controls.Add(Me.m_tbContact)
            Me.m_gbDetails.Controls.Add(Me.m_tbAuthor)
            Me.m_gbDetails.Controls.Add(Me.m_lbContact)
            Me.m_gbDetails.Controls.Add(Me.m_lbAuthor)
            Me.m_gbDetails.Controls.Add(Me.m_tbName)
            Me.m_gbDetails.Controls.Add(Me.m_tbDescription)
            Me.m_gbDetails.Controls.Add(Me.m_lblDescription)
            Me.m_gbDetails.Controls.Add(Me.m_lbScenario)
            Me.m_gbDetails.Name = "m_gbDetails"
            Me.m_gbDetails.TabStop = False
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
            'm_lblDescription
            '
            resources.ApplyResources(Me.m_lblDescription, "m_lblDescription")
            Me.m_lblDescription.Name = "m_lblDescription"
            '
            'm_lbScenario
            '
            resources.ApplyResources(Me.m_lbScenario, "m_lbScenario")
            Me.m_lbScenario.Name = "m_lbScenario"
            '
            'm_hdrScenario
            '
            resources.ApplyResources(Me.m_hdrScenario, "m_hdrScenario")
            Me.m_hdrScenario.Name = "m_hdrScenario"
            '
            'm_hdrSponsors
            '
            resources.ApplyResources(Me.m_hdrSponsors, "m_hdrSponsors")
            Me.m_hdrSponsors.Name = "m_hdrSponsors"
            '
            'm_tlpSponsors
            '
            resources.ApplyResources(Me.m_tlpSponsors, "m_tlpSponsors")
            Me.m_tlpSponsors.BackColor = System.Drawing.Color.White
            Me.m_tlpSponsors.Controls.Add(Me.m_pbSponsor1, 0, 0)
            Me.m_tlpSponsors.Controls.Add(Me.m_pbSponsor2, 1, 0)
            Me.m_tlpSponsors.Controls.Add(Me.m_pbSponsor3, 2, 0)
            Me.m_tlpSponsors.Controls.Add(Me.m_pbSponsor4, 3, 0)
            Me.m_tlpSponsors.Name = "m_tlpSponsors"
            '
            'm_pbSponsor1
            '
            resources.ApplyResources(Me.m_pbSponsor1, "m_pbSponsor1")
            Me.m_pbSponsor1.Image = Global.ScientificInterface.My.Resources.Resources.fimr_logo_50px
            Me.m_pbSponsor1.Name = "m_pbSponsor1"
            Me.m_pbSponsor1.TabStop = False
            '
            'm_pbSponsor2
            '
            resources.ApplyResources(Me.m_pbSponsor2, "m_pbSponsor2")
            Me.m_pbSponsor2.Image = Global.ScientificInterface.My.Resources.Resources.EU_50px
            Me.m_pbSponsor2.Name = "m_pbSponsor2"
            Me.m_pbSponsor2.TabStop = False
            '
            'm_pbSponsor3
            '
            resources.ApplyResources(Me.m_pbSponsor3, "m_pbSponsor3")
            Me.m_pbSponsor3.Image = Global.ScientificInterface.My.Resources.Resources.Lenfest_Logo_50px
            Me.m_pbSponsor3.Name = "m_pbSponsor3"
            Me.m_pbSponsor3.TabStop = False
            '
            'm_pbSponsor4
            '
            resources.ApplyResources(Me.m_pbSponsor4, "m_pbSponsor4")
            Me.m_pbSponsor4.Image = Global.ScientificInterface.My.Resources.Resources.sautxt_50px
            Me.m_pbSponsor4.Name = "m_pbSponsor4"
            Me.m_pbSponsor4.TabStop = False
            '
            'm_hdrInitialization
            '
            resources.ApplyResources(Me.m_hdrInitialization, "m_hdrInitialization")
            Me.m_hdrInitialization.Name = "m_hdrInitialization"
            '
            'm_gbInit
            '
            resources.ApplyResources(Me.m_gbInit, "m_gbInit")
            Me.m_gbInit.Controls.Add(Me.m_rbSpace)
            Me.m_gbInit.Controls.Add(Me.m_rbSim)
            Me.m_gbInit.Controls.Add(Me.m_rbDisabled)
            Me.m_gbInit.Name = "m_gbInit"
            Me.m_gbInit.TabStop = False
            '
            'm_rbSpace
            '
            resources.ApplyResources(Me.m_rbSpace, "m_rbSpace")
            Me.m_rbSpace.Name = "m_rbSpace"
            Me.m_rbSpace.TabStop = True
            Me.m_rbSpace.UseVisualStyleBackColor = True
            '
            'm_rbSim
            '
            resources.ApplyResources(Me.m_rbSim, "m_rbSim")
            Me.m_rbSim.Name = "m_rbSim"
            Me.m_rbSim.TabStop = True
            Me.m_rbSim.UseVisualStyleBackColor = True
            '
            'm_rbDisabled
            '
            resources.ApplyResources(Me.m_rbDisabled, "m_rbDisabled")
            Me.m_rbDisabled.Checked = True
            Me.m_rbDisabled.Name = "m_rbDisabled"
            Me.m_rbDisabled.TabStop = True
            Me.m_rbDisabled.UseVisualStyleBackColor = True
            '
            'frmEcotracerParameters
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ControlBox = False
            Me.Controls.Add(Me.m_tlpSponsors)
            Me.Controls.Add(Me.m_gbInit)
            Me.Controls.Add(Me.m_hdrInitialization)
            Me.Controls.Add(Me.m_hdrSponsors)
            Me.Controls.Add(Me.m_gbDetails)
            Me.Controls.Add(Me.m_hdrScenario)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmEcotracerParameters"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.m_gbDetails.ResumeLayout(False)
            Me.m_gbDetails.PerformLayout()
            Me.m_tlpSponsors.ResumeLayout(False)
            CType(Me.m_pbSponsor1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbSponsor2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbSponsor3, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbSponsor4, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gbInit.ResumeLayout(False)
            Me.m_gbInit.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_gbDetails As System.Windows.Forms.GroupBox
        Private WithEvents m_tbContact As System.Windows.Forms.TextBox
        Private WithEvents m_tbAuthor As System.Windows.Forms.TextBox
        Private WithEvents m_lbContact As System.Windows.Forms.Label
        Private WithEvents m_lbAuthor As System.Windows.Forms.Label
        Private WithEvents m_tbName As System.Windows.Forms.TextBox
        Private WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Private WithEvents m_lblDescription As System.Windows.Forms.Label
        Private WithEvents m_lbScenario As System.Windows.Forms.Label
        Private WithEvents m_hdrScenario As cEwEHeaderLabel
        Private WithEvents m_hdrSponsors As cEwEHeaderLabel
        Private WithEvents m_tlpSponsors As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_pbSponsor1 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbSponsor2 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbSponsor3 As System.Windows.Forms.PictureBox
        Private WithEvents m_pbSponsor4 As System.Windows.Forms.PictureBox
        Private WithEvents m_hdrInitialization As cEwEHeaderLabel
        Private WithEvents m_rbDisabled As System.Windows.Forms.RadioButton
        Private WithEvents m_rbSpace As System.Windows.Forms.RadioButton
        Private WithEvents m_rbSim As System.Windows.Forms.RadioButton
        Private WithEvents m_gbInit As System.Windows.Forms.GroupBox
    End Class

End Namespace
