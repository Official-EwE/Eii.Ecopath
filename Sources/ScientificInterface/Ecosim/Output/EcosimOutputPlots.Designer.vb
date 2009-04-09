Imports ScientificInterfaceShared.Controls

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcosimOutputPlots
        Inherits frmEwE

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
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcosimOutputPlots))
            Me.zgcPlots = New ZedGraph.ZedGraphControl
            Me.btnShowAllFits = New System.Windows.Forms.Button
            Me.lbGroups = New cGroupListBox
            Me.btnSave = New System.Windows.Forms.Button
            Me.lbPredRanks = New cGroupListBox
            Me.lbPreyRanks = New cGroupListBox
            Me.scMain = New System.Windows.Forms.SplitContainer
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.Panel2 = New System.Windows.Forms.Panel
            Me.m_lblGroup = New System.Windows.Forms.Label
            Me.Panel1 = New System.Windows.Forms.Panel
            Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel
            Me.Panel3 = New System.Windows.Forms.Panel
            Me.m_lblPredRank = New System.Windows.Forms.Label
            Me.Panel4 = New System.Windows.Forms.Panel
            Me.m_lblPreyRank = New System.Windows.Forms.Label
            Me.scMain.Panel1.SuspendLayout()
            Me.scMain.Panel2.SuspendLayout()
            Me.scMain.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.Panel2.SuspendLayout()
            Me.Panel1.SuspendLayout()
            Me.TableLayoutPanel2.SuspendLayout()
            Me.Panel3.SuspendLayout()
            Me.Panel4.SuspendLayout()
            Me.SuspendLayout()
            '
            'zgcPlots
            '
            Me.zgcPlots.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.zgcPlots, "zgcPlots")
            Me.zgcPlots.Name = "zgcPlots"
            Me.zgcPlots.ScrollGrace = 0
            Me.zgcPlots.ScrollMaxX = 0
            Me.zgcPlots.ScrollMaxY = 0
            Me.zgcPlots.ScrollMaxY2 = 0
            Me.zgcPlots.ScrollMinX = 0
            Me.zgcPlots.ScrollMinY = 0
            Me.zgcPlots.ScrollMinY2 = 0
            '
            'btnShowAllFits
            '
            resources.ApplyResources(Me.btnShowAllFits, "btnShowAllFits")
            Me.btnShowAllFits.Name = "btnShowAllFits"
            Me.btnShowAllFits.UseVisualStyleBackColor = True
            '
            'lbGroups
            '
            resources.ApplyResources(Me.lbGroups, "lbGroups")
            Me.lbGroups.FormattingEnabled = True
            Me.lbGroups.Name = "lbGroups"
            '
            'btnSave
            '
            resources.ApplyResources(Me.btnSave, "btnSave")
            Me.btnSave.Name = "btnSave"
            Me.btnSave.UseVisualStyleBackColor = True
            '
            'lbPredRanks
            '
            resources.ApplyResources(Me.lbPredRanks, "lbPredRanks")
            Me.lbPredRanks.BackColor = System.Drawing.SystemColors.Window
            Me.lbPredRanks.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.lbPredRanks.FormattingEnabled = True
            Me.lbPredRanks.Name = "lbPredRanks"
            '
            'lbPreyRanks
            '
            resources.ApplyResources(Me.lbPreyRanks, "lbPreyRanks")
            Me.lbPreyRanks.BackColor = System.Drawing.SystemColors.Window
            Me.lbPreyRanks.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.lbPreyRanks.FormattingEnabled = True
            Me.lbPreyRanks.Name = "lbPreyRanks"
            '
            'scMain
            '
            resources.ApplyResources(Me.scMain, "scMain")
            Me.scMain.MinimumSize = New System.Drawing.Size(400, 400)
            Me.scMain.Name = "scMain"
            '
            'scMain.Panel1
            '
            Me.scMain.Panel1.Controls.Add(Me.zgcPlots)
            '
            'scMain.Panel2
            '
            Me.scMain.Panel2.Controls.Add(Me.TableLayoutPanel1)
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.Panel2, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.Panel3, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Panel4, 0, 2)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'Panel2
            '
            Me.Panel2.Controls.Add(Me.m_lblGroup)
            Me.Panel2.Controls.Add(Me.lbGroups)
            resources.ApplyResources(Me.Panel2, "Panel2")
            Me.Panel2.Name = "Panel2"
            '
            'm_lblGroup
            '
            resources.ApplyResources(Me.m_lblGroup, "m_lblGroup")
            Me.m_lblGroup.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblGroup.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblGroup.Name = "m_lblGroup"
            '
            'Panel1
            '
            Me.Panel1.Controls.Add(Me.TableLayoutPanel2)
            resources.ApplyResources(Me.Panel1, "Panel1")
            Me.Panel1.Name = "Panel1"
            '
            'TableLayoutPanel2
            '
            resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
            Me.TableLayoutPanel2.Controls.Add(Me.btnSave, 0, 0)
            Me.TableLayoutPanel2.Controls.Add(Me.btnShowAllFits, 1, 0)
            Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
            '
            'Panel3
            '
            Me.Panel3.Controls.Add(Me.m_lblPredRank)
            Me.Panel3.Controls.Add(Me.lbPredRanks)
            resources.ApplyResources(Me.Panel3, "Panel3")
            Me.Panel3.Name = "Panel3"
            '
            'm_lblPredRank
            '
            resources.ApplyResources(Me.m_lblPredRank, "m_lblPredRank")
            Me.m_lblPredRank.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblPredRank.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblPredRank.Name = "m_lblPredRank"
            '
            'Panel4
            '
            Me.Panel4.Controls.Add(Me.m_lblPreyRank)
            Me.Panel4.Controls.Add(Me.lbPreyRanks)
            resources.ApplyResources(Me.Panel4, "Panel4")
            Me.Panel4.Name = "Panel4"
            '
            'm_lblPreyRank
            '
            resources.ApplyResources(Me.m_lblPreyRank, "m_lblPreyRank")
            Me.m_lblPreyRank.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblPreyRank.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblPreyRank.Name = "m_lblPreyRank"
            '
            'EcosimOutputPlots
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.scMain)
            Me.Name = "EcosimOutputPlots"
            Me.ShowIcon = False
            Me.scMain.Panel1.ResumeLayout(False)
            Me.scMain.Panel2.ResumeLayout(False)
            Me.scMain.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.Panel2.ResumeLayout(False)
            Me.Panel1.ResumeLayout(False)
            Me.TableLayoutPanel2.ResumeLayout(False)
            Me.Panel3.ResumeLayout(False)
            Me.Panel4.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents zgcPlots As ZedGraph.ZedGraphControl
        Private WithEvents btnShowAllFits As System.Windows.Forms.Button
        Private WithEvents lbGroups As cGroupListBox
        Private WithEvents btnSave As System.Windows.Forms.Button
        Private WithEvents lbPredRanks As cGroupListBox
        Private WithEvents lbPreyRanks As cGroupListBox
        Private WithEvents scMain As System.Windows.Forms.SplitContainer
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents Panel2 As System.Windows.Forms.Panel
        Private WithEvents Panel1 As System.Windows.Forms.Panel
        Private WithEvents Panel3 As System.Windows.Forms.Panel
        Private WithEvents Panel4 As System.Windows.Forms.Panel
        Private WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblGroup As System.Windows.Forms.Label
        Private WithEvents m_lblPredRank As System.Windows.Forms.Label
        Private WithEvents m_lblPreyRank As System.Windows.Forms.Label
    End Class

End Namespace

