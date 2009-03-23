Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

Namespace Ecopath.Output

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RunPSD
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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunPSD))
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
            Me.drpDwnBtnTotalMortality = New System.Windows.Forms.ToolStripDropDownButton
            Me.mnuItmGroupPB = New System.Windows.Forms.ToolStripMenuItem
            Me.mnuItmLorenzen = New System.Windows.Forms.ToolStripMenuItem
            Me.mnuItmDummy1 = New System.Windows.Forms.ToolStripMenuItem
            Me.tbxNWLat = New System.Windows.Forms.ToolStripTextBox
            Me.mnuItmDummy2 = New System.Windows.Forms.ToolStripMenuItem
            Me.tbxSELat = New System.Windows.Forms.ToolStripTextBox
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.btnShowHideGroups = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator
            Me.lblNoOfPointsPSD = New System.Windows.Forms.ToolStripLabel
            Me.tbxNoOfPointsPSD = New System.Windows.Forms.ToolStripTextBox
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.lblMinWeight = New System.Windows.Forms.ToolStripLabel
            Me.tbxMinWeight = New System.Windows.Forms.ToolStripTextBox
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
            Me.lblNoOfPointsMovAvg = New System.Windows.Forms.ToolStripLabel
            Me.tbxNoOfPointsMovAvg = New System.Windows.Forms.ToolStripTextBox
            Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
            Me.btnRun = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
            Me.zgcZedGraphCntl = New ZedGraph.ZedGraphControl
            Me.ToolStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'ToolStrip1
            '
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.drpDwnBtnTotalMortality, Me.ToolStripSeparator1, Me.btnShowHideGroups, Me.ToolStripSeparator6, Me.lblNoOfPointsPSD, Me.tbxNoOfPointsPSD, Me.ToolStripSeparator2, Me.lblMinWeight, Me.tbxMinWeight, Me.ToolStripSeparator3, Me.lblNoOfPointsMovAvg, Me.tbxNoOfPointsMovAvg, Me.ToolStripSeparator4, Me.btnRun, Me.ToolStripSeparator5})
            Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
            Me.ToolStrip1.Name = "ToolStrip1"
            Me.ToolStrip1.Size = New System.Drawing.Size(869, 25)
            Me.ToolStrip1.TabIndex = 0
            Me.ToolStrip1.Text = "ToolStrip1"
            '
            'drpDwnBtnTotalMortality
            '
            Me.drpDwnBtnTotalMortality.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuItmGroupPB, Me.mnuItmLorenzen, Me.mnuItmDummy1, Me.tbxNWLat, Me.mnuItmDummy2, Me.tbxSELat})
            Me.drpDwnBtnTotalMortality.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.drpDwnBtnTotalMortality.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.drpDwnBtnTotalMortality.Name = "drpDwnBtnTotalMortality"
            Me.drpDwnBtnTotalMortality.Size = New System.Drawing.Size(105, 22)
            Me.drpDwnBtnTotalMortality.Text = "Total mortality"
            '
            'mnuItmGroupPB
            '
            Me.mnuItmGroupPB.CheckOnClick = True
            Me.mnuItmGroupPB.Name = "mnuItmGroupPB"
            Me.mnuItmGroupPB.Size = New System.Drawing.Size(184, 22)
            Me.mnuItmGroupPB.Text = "Group P/B "
            '
            'mnuItmLorenzen
            '
            Me.mnuItmLorenzen.CheckOnClick = True
            Me.mnuItmLorenzen.Name = "mnuItmLorenzen"
            Me.mnuItmLorenzen.Size = New System.Drawing.Size(184, 22)
            Me.mnuItmLorenzen.Text = "Lorenzen-variable "
            '
            'mnuItmDummy1
            '
            Me.mnuItmDummy1.Margin = New System.Windows.Forms.Padding(10, 0, 0, 0)
            Me.mnuItmDummy1.Name = "mnuItmDummy1"
            Me.mnuItmDummy1.Size = New System.Drawing.Size(184, 22)
            Me.mnuItmDummy1.Text = "NW corner latitudes:"
            '
            'tbxNWLat
            '
            Me.tbxNWLat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.tbxNWLat.Margin = New System.Windows.Forms.Padding(115, -20, 1, 1)
            Me.tbxNWLat.Name = "tbxNWLat"
            Me.tbxNWLat.Size = New System.Drawing.Size(35, 21)
            '
            'mnuItmDummy2
            '
            Me.mnuItmDummy2.Margin = New System.Windows.Forms.Padding(10, -1, 0, 0)
            Me.mnuItmDummy2.Name = "mnuItmDummy2"
            Me.mnuItmDummy2.Size = New System.Drawing.Size(184, 22)
            Me.mnuItmDummy2.Text = "SE corner latitudes:"
            '
            'tbxSELat
            '
            Me.tbxSELat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.tbxSELat.Margin = New System.Windows.Forms.Padding(115, -21, 1, 1)
            Me.tbxSELat.Name = "tbxSELat"
            Me.tbxSELat.Size = New System.Drawing.Size(35, 21)
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
            '
            'btnShowHideGroups
            '
            Me.btnShowHideGroups.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
            Me.btnShowHideGroups.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.btnShowHideGroups.Name = "btnShowHideGroups"
            Me.btnShowHideGroups.Size = New System.Drawing.Size(107, 22)
            Me.btnShowHideGroups.Text = "Select groups ..."
            '
            'ToolStripSeparator6
            '
            Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
            Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
            '
            'lblNoOfPointsPSD
            '
            Me.lblNoOfPointsPSD.Name = "lblNoOfPointsPSD"
            Me.lblNoOfPointsPSD.Size = New System.Drawing.Size(108, 22)
            Me.lblNoOfPointsPSD.Text = "No. of points for PSD"
            '
            'tbxNoOfPointsPSD
            '
            Me.tbxNoOfPointsPSD.Name = "tbxNoOfPointsPSD"
            Me.tbxNoOfPointsPSD.Size = New System.Drawing.Size(50, 25)
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'lblMinWeight
            '
            Me.lblMinWeight.Name = "lblMinWeight"
            Me.lblMinWeight.Size = New System.Drawing.Size(59, 22)
            Me.lblMinWeight.Text = "Min. wt (g)"
            '
            'tbxMinWeight
            '
            Me.tbxMinWeight.Name = "tbxMinWeight"
            Me.tbxMinWeight.Size = New System.Drawing.Size(50, 25)
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
            '
            'lblNoOfPointsMovAvg
            '
            Me.lblNoOfPointsMovAvg.Name = "lblNoOfPointsMovAvg"
            Me.lblNoOfPointsMovAvg.Size = New System.Drawing.Size(130, 22)
            Me.lblNoOfPointsMovAvg.Text = "No. of points for mov avg"
            '
            'tbxNoOfPointsMovAvg
            '
            Me.tbxNoOfPointsMovAvg.Name = "tbxNoOfPointsMovAvg"
            Me.tbxNoOfPointsMovAvg.Size = New System.Drawing.Size(50, 25)
            '
            'ToolStripSeparator4
            '
            Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
            Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 25)
            '
            'btnRun
            '
            Me.btnRun.AutoSize = False
            Me.btnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.btnRun.Image = CType(resources.GetObject("btnRun.Image"), System.Drawing.Image)
            Me.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.btnRun.Name = "btnRun"
            Me.btnRun.Size = New System.Drawing.Size(50, 22)
            Me.btnRun.Text = "Run"
            '
            'ToolStripSeparator5
            '
            Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
            Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 25)
            '
            'zgcZedGraphCntl
            '
            Me.zgcZedGraphCntl.Dock = System.Windows.Forms.DockStyle.Fill
            Me.zgcZedGraphCntl.Location = New System.Drawing.Point(0, 25)
            Me.zgcZedGraphCntl.Name = "zgcZedGraphCntl"
            Me.zgcZedGraphCntl.ScrollGrace = 0
            Me.zgcZedGraphCntl.ScrollMaxX = 0
            Me.zgcZedGraphCntl.ScrollMaxY = 0
            Me.zgcZedGraphCntl.ScrollMaxY2 = 0
            Me.zgcZedGraphCntl.ScrollMinX = 0
            Me.zgcZedGraphCntl.ScrollMinY = 0
            Me.zgcZedGraphCntl.ScrollMinY2 = 0
            Me.zgcZedGraphCntl.Size = New System.Drawing.Size(869, 241)
            Me.zgcZedGraphCntl.TabIndex = 1
            '
            'RunPSD
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(869, 266)
            Me.Controls.Add(Me.zgcZedGraphCntl)
            Me.Controls.Add(Me.ToolStrip1)
            Me.Name = "RunPSD"
            Me.Text = "RunParticleSizeDistribution"
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents drpDwnBtnTotalMortality As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents mnuItmGroupPB As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuItmLorenzen As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents lblNoOfPointsPSD As System.Windows.Forms.ToolStripLabel
        Friend WithEvents tbxNoOfPointsPSD As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents lblMinWeight As System.Windows.Forms.ToolStripLabel
        Friend WithEvents tbxMinWeight As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents lblNoOfPointsMovAvg As System.Windows.Forms.ToolStripLabel
        Friend WithEvents tbxNoOfPointsMovAvg As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents btnRun As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents zgcZedGraphCntl As ZedGraph.ZedGraphControl
        Friend WithEvents btnShowHideGroups As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents tbxNWLat As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents mnuItmDummy1 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuItmDummy2 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents tbxSELat As System.Windows.Forms.ToolStripTextBox
    End Class

End Namespace
