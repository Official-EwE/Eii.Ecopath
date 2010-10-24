Imports ScientificInterfaceShared.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports WeifenLuo.WinFormsUI.Docking

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
            Dim m_sep1 As System.Windows.Forms.ToolStripSeparator
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunPSD))
            Dim m_sep666 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep2 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep3 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep4 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep5 As System.Windows.Forms.ToolStripSeparator
            Me.m_tsRunPSD = New ScientificInterfaceShared.Controls.cEwEToolstrip
            Me.m_tsddTotalMortality = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiGroupPB = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiLorenzen = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiMeanLat = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tscbxMeanLat = New System.Windows.Forms.ToolStripComboBox
            Me.m_tsbnShowHideGroups = New System.Windows.Forms.ToolStripButton
            Me.m_tslblNoOfPointsPSD = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbxNoOfPointsPSD = New System.Windows.Forms.ToolStripTextBox
            Me.m_tslblMinWeight = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbxMinWeight = New System.Windows.Forms.ToolStripTextBox
            Me.m_tslblNoOfPointsMovAvg = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbxNoOfPointsMovAvg = New System.Windows.Forms.ToolStripTextBox
            Me.m_tsbtnRun = New System.Windows.Forms.ToolStripButton
            Me.m_zedgraph = New ZedGraph.ZedGraphControl
            m_sep1 = New System.Windows.Forms.ToolStripSeparator
            m_sep666 = New System.Windows.Forms.ToolStripSeparator
            m_sep2 = New System.Windows.Forms.ToolStripSeparator
            m_sep3 = New System.Windows.Forms.ToolStripSeparator
            m_sep4 = New System.Windows.Forms.ToolStripSeparator
            m_sep5 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsRunPSD.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_sep1
            '
            m_sep1.Name = "m_sep1"
            resources.ApplyResources(m_sep1, "m_sep1")
            '
            'm_sep666
            '
            m_sep666.Name = "m_sep666"
            resources.ApplyResources(m_sep666, "m_sep666")
            '
            'm_sep2
            '
            m_sep2.Name = "m_sep2"
            resources.ApplyResources(m_sep2, "m_sep2")
            '
            'm_sep3
            '
            m_sep3.Name = "m_sep3"
            resources.ApplyResources(m_sep3, "m_sep3")
            '
            'm_sep4
            '
            m_sep4.Name = "m_sep4"
            resources.ApplyResources(m_sep4, "m_sep4")
            '
            'm_sep5
            '
            m_sep5.Name = "m_sep5"
            resources.ApplyResources(m_sep5, "m_sep5")
            '
            'm_tsRunPSD
            '
            Me.m_tsRunPSD.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsddTotalMortality, m_sep1, Me.m_tsbnShowHideGroups, m_sep666, Me.m_tslblNoOfPointsPSD, Me.m_tstbxNoOfPointsPSD, m_sep2, Me.m_tslblMinWeight, Me.m_tstbxMinWeight, m_sep3, Me.m_tslblNoOfPointsMovAvg, Me.m_tstbxNoOfPointsMovAvg, m_sep4, Me.m_tsbtnRun, m_sep5})
            resources.ApplyResources(Me.m_tsRunPSD, "m_tsRunPSD")
            Me.m_tsRunPSD.Name = "m_tsRunPSD"
            '
            'm_tsddTotalMortality
            '
            Me.m_tsddTotalMortality.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiGroupPB, Me.m_tsmiLorenzen, Me.m_tsmiMeanLat, Me.m_tscbxMeanLat})
            Me.m_tsddTotalMortality.Image = SharedResources.OptionsHS
            resources.ApplyResources(Me.m_tsddTotalMortality, "m_tsddTotalMortality")
            Me.m_tsddTotalMortality.Name = "m_tsddTotalMortality"
            '
            'm_tsmiGroupPB
            '
            Me.m_tsmiGroupPB.CheckOnClick = True
            Me.m_tsmiGroupPB.Name = "m_tsmiGroupPB"
            resources.ApplyResources(Me.m_tsmiGroupPB, "m_tsmiGroupPB")
            '
            'm_tsmiLorenzen
            '
            Me.m_tsmiLorenzen.CheckOnClick = True
            Me.m_tsmiLorenzen.Name = "m_tsmiLorenzen"
            resources.ApplyResources(Me.m_tsmiLorenzen, "m_tsmiLorenzen")
            '
            'm_tsmiMeanLat
            '
            Me.m_tsmiMeanLat.Margin = New System.Windows.Forms.Padding(5, 0, 0, 0)
            Me.m_tsmiMeanLat.Name = "m_tsmiMeanLat"
            resources.ApplyResources(Me.m_tsmiMeanLat, "m_tsmiMeanLat")
            '
            'm_tscbxMeanLat
            '
            resources.ApplyResources(Me.m_tscbxMeanLat, "m_tscbxMeanLat")
            Me.m_tscbxMeanLat.Items.AddRange(New Object() {resources.GetString("m_tscbxMeanLat.Items"), resources.GetString("m_tscbxMeanLat.Items1"), resources.GetString("m_tscbxMeanLat.Items2")})
            Me.m_tscbxMeanLat.Margin = New System.Windows.Forms.Padding(55, -22, 2, 2)
            Me.m_tscbxMeanLat.Name = "m_tscbxMeanLat"
            '
            'm_tsbnShowHideGroups
            '
            Me.m_tsbnShowHideGroups.Image = SharedResources.Eye_open
            resources.ApplyResources(Me.m_tsbnShowHideGroups, "m_tsbnShowHideGroups")
            Me.m_tsbnShowHideGroups.Name = "m_tsbnShowHideGroups"
            '
            'm_tslblNoOfPointsPSD
            '
            Me.m_tslblNoOfPointsPSD.Name = "m_tslblNoOfPointsPSD"
            resources.ApplyResources(Me.m_tslblNoOfPointsPSD, "m_tslblNoOfPointsPSD")
            '
            'm_tstbxNoOfPointsPSD
            '
            Me.m_tstbxNoOfPointsPSD.Name = "m_tstbxNoOfPointsPSD"
            resources.ApplyResources(Me.m_tstbxNoOfPointsPSD, "m_tstbxNoOfPointsPSD")
            '
            'm_tslblMinWeight
            '
            Me.m_tslblMinWeight.Name = "m_tslblMinWeight"
            resources.ApplyResources(Me.m_tslblMinWeight, "m_tslblMinWeight")
            '
            'm_tstbxMinWeight
            '
            Me.m_tstbxMinWeight.Name = "m_tstbxMinWeight"
            resources.ApplyResources(Me.m_tstbxMinWeight, "m_tstbxMinWeight")
            '
            'm_tslblNoOfPointsMovAvg
            '
            Me.m_tslblNoOfPointsMovAvg.Name = "m_tslblNoOfPointsMovAvg"
            resources.ApplyResources(Me.m_tslblNoOfPointsMovAvg, "m_tslblNoOfPointsMovAvg")
            '
            'm_tstbxNoOfPointsMovAvg
            '
            Me.m_tstbxNoOfPointsMovAvg.Name = "m_tstbxNoOfPointsMovAvg"
            resources.ApplyResources(Me.m_tstbxNoOfPointsMovAvg, "m_tstbxNoOfPointsMovAvg")
            '
            'm_tsbtnRun
            '
            resources.ApplyResources(Me.m_tsbtnRun, "m_tsbtnRun")
            Me.m_tsbtnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbtnRun.Name = "m_tsbtnRun"
            '
            'm_zedgraph
            '
            resources.ApplyResources(Me.m_zedgraph, "m_zedgraph")
            Me.m_zedgraph.Name = "m_zedgraph"
            Me.m_zedgraph.ScrollGrace = 0
            Me.m_zedgraph.ScrollMaxX = 0
            Me.m_zedgraph.ScrollMaxY = 0
            Me.m_zedgraph.ScrollMaxY2 = 0
            Me.m_zedgraph.ScrollMinX = 0
            Me.m_zedgraph.ScrollMinY = 0
            Me.m_zedgraph.ScrollMinY2 = 0
            '
            'RunPSD
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_zedgraph)
            Me.Controls.Add(Me.m_tsRunPSD)
            Me.Name = "RunPSD"
            Me.ShowInTaskbar = False
            Me.m_tsRunPSD.ResumeLayout(False)
            Me.m_tsRunPSD.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsRunPSD As cEwEToolstrip
        Private WithEvents m_tstbxNoOfPointsMovAvg As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsbtnRun As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsddTotalMortality As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiGroupPB As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiLorenzen As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsbnShowHideGroups As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tslblNoOfPointsPSD As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tstbxNoOfPointsPSD As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tslblMinWeight As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tstbxMinWeight As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tslblNoOfPointsMovAvg As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_zedgraph As ZedGraph.ZedGraphControl
        Private WithEvents m_tsmiMeanLat As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tscbxMeanLat As System.Windows.Forms.ToolStripComboBox
    End Class

End Namespace
