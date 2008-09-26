'==============================================================================
'
' $Log: frmEcotrophGrid.vb,v $
' Revision 1.1  2008/09/26 07:30:44  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.116  2008/09/09 14:44:52  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.115  2008/06/05 19:43:47  joeh
' no message
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On
Imports System.Windows.Forms
Imports ZedGraph
Imports EwEUtils.Commands

#End Region ' Imports

Public Class frmEcotrophGrid

#Region "Private fields"
    Private Const PRGRS_BAR_MAX As Integer = 10

    Private WithEvents m_EcotrophManager As cEcotrophManager
    'Private WithEvents m_Transpose As ConnectToComputation.cTranspose 'temporary

    Private m_FormActivatedCounter As Integer
    Private m_PrevSelectedNode As TreeNode
    Private m_CTSACatchesFilePath As String
    Private m_CatchPastAnalysisFilePath As String
#End Region 'Private fields

#Region "Constructors"
    Public Sub New(ByRef EcotrophManager As cEcotrophManager)
        Me.InitializeComponent()

        m_EcotrophManager = EcotrophManager
    End Sub
#End Region 'Constructors

#Region "Private Events" 'Private Events

    Private Sub frmEcotroph_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        m_FormActivatedCounter = m_FormActivatedCounter + 1
        If m_FormActivatedCounter = 1 Then
            tsEcotroph.Visible = False
            tcEcotroph.Visible = False
            zgEcotroph.Visible = False

            tscbxMainDiagnosis.Text = CStr(tscbxMainDiagnosis.Items.Item(0))
            tscbxMainDynamics.Text = CStr(tscbxMainDynamics.Items.Item(0))
            'sc1Ecotroph.Panel2.Controls.RemoveByKey("tsEcotroph")
            'sc2Ecotroph.Panel1.Controls.RemoveByKey("tcEcotroph")
            'sc2Ecotroph.Panel2.Controls.RemoveByKey("zgEcotroph")
            ConnectToComputation.cTranspose.Transpose.m_ToolStrip = tsEcotroph
            UserInterface.cTranspose.Transpose.m_ToolStrip = tsEcotroph
            UserInterface.cTranspose.Transpose.m_TabPages(1) = tpEcotroph1
            UserInterface.cTranspose.Transpose.m_TabPages(2) = tpEcotroph2
            UserInterface.cTranspose.Transpose.m_TabPages(3) = tpEcotroph3
            UserInterface.cTranspose.Transpose.m_TabPages(4) = tpEcotroph4
            UserInterface.cTranspose.Transpose.m_TabPages(5) = tpEcotroph5
            UserInterface.cTranspose.Transpose.m_TabPages(6) = tpEcotroph6
            UserInterface.cTranspose.Transpose.m_TabPages(7) = tpEcotroph7
            UserInterface.cTranspose.Transpose.m_TabPages(8) = tpEcotroph8
            UserInterface.cTranspose.Transpose.m_TabPages(9) = tpEcotroph9
            UserInterface.cTranspose.Transpose.m_TabPages(10) = tpEcotroph10
            UserInterface.cTranspose.Transpose.m_TabPages(11) = tpEcotroph11
            UserInterface.cTranspose.Transpose.m_TabPages(12) = tpEcotroph12
            UserInterface.cTranspose.Transpose.m_TabPages(13) = tpEcotroph13

            ConnectToComputation.cCTSA.CTSA.m_ToolStrip = tsEcotroph
            UserInterface.cCTSA.CTSA.m_ToolStrip = tsEcotroph
            'UserInterface.cCTSA.CTSA.m_TabCntl = tcEcotroph
            UserInterface.cCTSA.CTSA.m_TabPages(1) = tpEcotroph1
            UserInterface.cCTSA.CTSA.m_TabPages(2) = tpEcotroph2
            UserInterface.cCTSA.CTSA.m_TabPages(3) = tpEcotroph3
            UserInterface.cCTSA.CTSA.m_TabPages(4) = tpEcotroph4
            UserInterface.cCTSA.CTSA.m_TabPages(5) = tpEcotroph5
            UserInterface.cCTSA.CTSA.m_TabPages(6) = tpEcotroph6
            UserInterface.cCTSA.CTSA.m_TabPages(7) = tpEcotroph7
            UserInterface.cCTSA.CTSA.m_TabPages(8) = tpEcotroph8
            UserInterface.cCTSA.CTSA.m_TabPages(9) = tpEcotroph9
            UserInterface.cCTSA.CTSA.m_TabPages(10) = tpEcotroph10
            UserInterface.cCTSA.CTSA.m_TabPages(11) = tpEcotroph11
            UserInterface.cCTSA.CTSA.m_TabPages(12) = tpEcotroph12
            UserInterface.cCTSA.CTSA.m_TabPages(13) = tpEcotroph13

            ConnectToComputation.cDiagnosis.Diagnosis.m_ToolStrip = tsEcotroph
            UserInterface.cDiagnosis.Diagnosis.m_ToolStrip = tsEcotroph
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(1) = tpEcotroph1
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(2) = tpEcotroph2
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(3) = tpEcotroph3
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(4) = tpEcotroph4
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(5) = tpEcotroph5
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(6) = tpEcotroph6
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(7) = tpEcotroph7
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(8) = tpEcotroph8
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(9) = tpEcotroph9
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(10) = tpEcotroph10
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(11) = tpEcotroph11
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(12) = tpEcotroph12
            UserInterface.cDiagnosis.Diagnosis.m_TabPages(13) = tpEcotroph13

            ConnectToComputation.cDynamics.Dynamics.m_ToolStrip = tsEcotroph
            UserInterface.cDynamics.Dynamics.m_ToolStrip = tsEcotroph
            UserInterface.cDynamics.Dynamics.m_TabPages(1) = tpEcotroph1
            UserInterface.cDynamics.Dynamics.m_TabPages(2) = tpEcotroph2
            UserInterface.cDynamics.Dynamics.m_TabPages(3) = tpEcotroph3
            UserInterface.cDynamics.Dynamics.m_TabPages(4) = tpEcotroph4
            UserInterface.cDynamics.Dynamics.m_TabPages(5) = tpEcotroph5
            UserInterface.cDynamics.Dynamics.m_TabPages(6) = tpEcotroph6
            UserInterface.cDynamics.Dynamics.m_TabPages(7) = tpEcotroph7
            UserInterface.cDynamics.Dynamics.m_TabPages(8) = tpEcotroph8
            UserInterface.cDynamics.Dynamics.m_TabPages(9) = tpEcotroph9
            UserInterface.cDynamics.Dynamics.m_TabPages(10) = tpEcotroph10
            UserInterface.cDynamics.Dynamics.m_TabPages(11) = tpEcotroph11
            UserInterface.cDynamics.Dynamics.m_TabPages(12) = tpEcotroph12
            UserInterface.cDynamics.Dynamics.m_TabPages(13) = tpEcotroph13
        End If
    End Sub

    Private Sub frmEcotroph_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        tvEcotroph.Nodes("ndEwEEcotrophPlugin").Expand()
    End Sub

    Private Sub tvEcotroph_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvEcotroph.AfterSelect

        Dim cmdh As CommandHandler = CommandHandler.GetInstance()
        Dim cmdFO As FileOpenCommand = DirectCast(cmdh.GetCommand(FileOpenCommand.COMMAND_NAME), FileOpenCommand)

        Select Case e.Node.Text
            Case My.Resources.TREE_NODE_AUTO_SMOOTH
                'If m_EcotrophManager.IsAEFRun = True Then Exit Sub
                ConnectToComputation.cTranspose.Transpose.m_EcotrophManager = m_EcotrophManager
                ConnectToComputation.cTranspose.Transpose.m_PanelToolStrip = scEcotroph1.Panel2
                ConnectToComputation.cTranspose.Transpose.m_PanelTabCntl = scEcotroph2.Panel1
                ConnectToComputation.cTranspose.RunTransposeAEF()
                UserInterface.cTranspose.Transpose.m_EcotrophManager = m_EcotrophManager
                UserInterface.cTranspose.Transpose.m_PanelToolStrip = scEcotroph1.Panel2
                UserInterface.cTranspose.Transpose.m_PanelTabCntl = scEcotroph2.Panel1
                UserInterface.cTranspose.Transpose.m_Tree = tvEcotroph
                UserInterface.cTranspose.DisplayTransposeAEF()
            Case My.Resources.TREE_NODE_OMNI_IDX
                'If m_EcotrophManager.IsOmniIdxRun = True Then Exit Sub
                ConnectToComputation.cTranspose.Transpose.m_EcotrophManager = m_EcotrophManager
                ConnectToComputation.cTranspose.Transpose.m_PanelToolStrip = scEcotroph1.Panel2
                ConnectToComputation.cTranspose.Transpose.m_PanelTabCntl = scEcotroph2.Panel1
                ConnectToComputation.cTranspose.RunTransposeOmniIdx()
                UserInterface.cTranspose.Transpose.m_EcotrophManager = m_EcotrophManager
                UserInterface.cTranspose.Transpose.m_PanelToolStrip = scEcotroph1.Panel2
                UserInterface.cTranspose.Transpose.m_PanelTabCntl = scEcotroph2.Panel1
                UserInterface.cTranspose.Transpose.m_Tree = tvEcotroph
                UserInterface.cTranspose.DisplayTransposeOmniIdx()
            Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                'If m_EcotrophManager.IsUserDefValRun = True Then Exit Sub
                ConnectToComputation.cTranspose.Transpose.m_EcotrophManager = m_EcotrophManager
                ConnectToComputation.cTranspose.Transpose.m_PanelToolStrip = scEcotroph1.Panel2
                ConnectToComputation.cTranspose.Transpose.m_PanelTabCntl = scEcotroph2.Panel1
                ConnectToComputation.cTranspose.RunTransposeUserDefVal()
                UserInterface.cTranspose.Transpose.m_EcotrophManager = m_EcotrophManager
                UserInterface.cTranspose.Transpose.m_PanelToolStrip = scEcotroph1.Panel2
                UserInterface.cTranspose.Transpose.m_PanelTabCntl = scEcotroph2.Panel1
                UserInterface.cTranspose.Transpose.m_Tree = tvEcotroph
                UserInterface.cTranspose.DisplayTransposeUserDefVal()
            Case My.Resources.TREE_NODE_BASIC_PARAM
                Select Case e.Node.Parent.Text
                    Case My.Resources.TREE_NODE_CTSA
                        'If m_EcotrophManager.IsCTSAParameterRun = False Then
                        ConnectToComputation.cCTSA.CTSA.m_EcotrophManager = m_EcotrophManager
                        ConnectToComputation.cCTSA.CTSA.m_PanelToolStrip = scEcotroph1.Panel2
                        ConnectToComputation.cCTSA.CTSA.m_PanelTabCntl = scEcotroph2.Panel1
                        ConnectToComputation.cCTSA.RunCTSAParameter()
                        'End If
                        UserInterface.cCTSA.CTSA.m_EcotrophManager = m_EcotrophManager
                        UserInterface.cCTSA.CTSA.m_PanelToolStrip = scEcotroph1.Panel2
                        UserInterface.cCTSA.CTSA.m_PanelTabCntl = scEcotroph2.Panel1
                        UserInterface.cCTSA.CTSA.m_Tree = tvEcotroph
                        UserInterface.cCTSA.DisplayCTSAParameter()
                    Case My.Resources.TREE_NODE_DIAGNOSIS
                        'If m_EcotrophManager.IsDiagnosisParameterRun = True Then Exit Sub
                        ConnectToComputation.cDiagnosis.Diagnosis.m_EcotrophManager = m_EcotrophManager
                        ConnectToComputation.cDiagnosis.Diagnosis.m_PanelToolStrip = scEcotroph1.Panel2
                        ConnectToComputation.cDiagnosis.Diagnosis.m_PanelTabCntl = scEcotroph2.Panel1
                        ConnectToComputation.cDiagnosis.RunDiagnosisParameter(tscbxMainDiagnosis.Text)
                        UserInterface.cDiagnosis.Diagnosis.m_EcotrophManager = m_EcotrophManager
                        UserInterface.cDiagnosis.Diagnosis.m_PanelToolStrip = scEcotroph1.Panel2
                        UserInterface.cDiagnosis.Diagnosis.m_PanelTabCntl = scEcotroph2.Panel1
                        UserInterface.cDiagnosis.Diagnosis.m_Tree = tvEcotroph
                        UserInterface.cDiagnosis.DisplayDiagnosisParameter(tscbxMainDiagnosis.Text)
                    Case My.Resources.TREE_NODE_DYNAMICS
                        ConnectToComputation.cDynamics.Dynamics.m_EcotrophManager = m_EcotrophManager
                        ConnectToComputation.cDynamics.Dynamics.m_PanelToolStrip = scEcotroph1.Panel2
                        ConnectToComputation.cDynamics.Dynamics.m_PanelTabCntl = scEcotroph2.Panel1
                        ConnectToComputation.cDynamics.RunDynamicsParameter(tscbxMainDynamics.Text)
                        UserInterface.cDynamics.Dynamics.m_EcotrophManager = m_EcotrophManager
                        UserInterface.cDynamics.Dynamics.m_PanelToolStrip = scEcotroph1.Panel2
                        UserInterface.cDynamics.Dynamics.m_PanelTabCntl = scEcotroph2.Panel1
                        UserInterface.cDynamics.Dynamics.m_Tree = tvEcotroph
                        UserInterface.cDynamics.DisplayDynamicsParameter(tscbxMainDynamics.Text)
                End Select
            Case My.Resources.TREE_NODE_FWD_CAL
                If m_EcotrophManager.IsCTSAParameterRun = True Then
                    'If m_EcotrophManager.IsFwdCalRun = False Then
                    ConnectToComputation.cCTSA.CTSA.m_EcotrophManager = m_EcotrophManager
                    ConnectToComputation.cCTSA.CTSA.m_PanelToolStrip = scEcotroph1.Panel2
                    ConnectToComputation.cCTSA.CTSA.m_PanelTabCntl = scEcotroph2.Panel1
                    ConnectToComputation.cCTSA.RunCTSAFwdCal()
                    'End If
                    UserInterface.cCTSA.CTSA.m_EcotrophManager = m_EcotrophManager
                    UserInterface.cCTSA.CTSA.m_PanelToolStrip = scEcotroph1.Panel2
                    UserInterface.cCTSA.CTSA.m_PanelTabCntl = scEcotroph2.Panel1
                    UserInterface.cCTSA.CTSA.m_Tree = tvEcotroph
                    UserInterface.cCTSA.DisplayCTSAFwdCal()
                Else
                    MsgBox(My.Resources.ERR_MSG_RUN_CTSA_PARAM, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                    tsEcotroph.Visible = False
                    tcEcotroph.Visible = False
                    'e.Node.BackColor = Drawing.Color.LightGoldenrodYellow
                    'tvEcotroph.SelectedNode = m_PrevSelectedNode
                End If
            Case My.Resources.TREE_NODE_BWD_CAL
                If m_EcotrophManager.IsCTSAParameterRun = True Then
                    'If m_EcotrophManager.IsBwdCalRun = False Then
                    ConnectToComputation.cCTSA.CTSA.m_EcotrophManager = m_EcotrophManager
                    ConnectToComputation.cCTSA.CTSA.m_PanelToolStrip = scEcotroph1.Panel2
                    ConnectToComputation.cCTSA.CTSA.m_PanelTabCntl = scEcotroph2.Panel1
                    ConnectToComputation.cCTSA.RunCTSABwdCal()
                    'End If
                    UserInterface.cCTSA.CTSA.m_EcotrophManager = m_EcotrophManager
                    UserInterface.cCTSA.CTSA.m_PanelToolStrip = scEcotroph1.Panel2
                    UserInterface.cCTSA.CTSA.m_PanelTabCntl = scEcotroph2.Panel1
                    UserInterface.cCTSA.CTSA.m_Tree = tvEcotroph
                    UserInterface.cCTSA.DisplayCTSABwdCal()
                Else
                    MsgBox(My.Resources.ERR_MSG_RUN_CTSA_PARAM, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                    tsEcotroph.Visible = False
                    tcEcotroph.Visible = False
                End If
            Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR, My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                If m_EcotrophManager.IsDiagnosisParameterRun = True Then
                    ConnectToComputation.cDiagnosis.Diagnosis.m_EcotrophManager = m_EcotrophManager
                    ConnectToComputation.cDiagnosis.Diagnosis.m_PanelToolStrip = scEcotroph1.Panel2
                    ConnectToComputation.cDiagnosis.Diagnosis.m_PanelTabCntl = scEcotroph2.Panel1
                    ConnectToComputation.cDiagnosis.RunDiagnosis(e.Node.Text)
                    UserInterface.cDiagnosis.Diagnosis.m_EcotrophManager = m_EcotrophManager
                    UserInterface.cDiagnosis.Diagnosis.m_PanelToolStrip = scEcotroph1.Panel2
                    UserInterface.cDiagnosis.Diagnosis.m_PanelTabCntl = scEcotroph2.Panel1
                    UserInterface.cDiagnosis.Diagnosis.m_Tree = tvEcotroph
                    UserInterface.cDiagnosis.DisplayDiagnosis(e.Node.Text)
                Else
                    MsgBox(My.Resources.ERR_MSG_RUN_DIAGNOSIS_PARAM, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                    tsEcotroph.Visible = False
                    tcEcotroph.Visible = False
                End If
            Case My.Resources.TREE_NODE_CATCH_FORECAST, My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                If m_EcotrophManager.IsDynamicsParameterRun = True Then
                    ConnectToComputation.cDynamics.Dynamics.m_EcotrophManager = m_EcotrophManager
                    ConnectToComputation.cDynamics.Dynamics.m_PanelToolStrip = scEcotroph1.Panel2
                    ConnectToComputation.cDynamics.Dynamics.m_PanelTabCntl = scEcotroph2.Panel1
                    Select Case e.Node.Text
                        Case My.Resources.TREE_NODE_CATCH_FORECAST
                            ConnectToComputation.cDynamics.RunDynamics(e.Node.Text)
                            UserInterface.cDynamics.Dynamics.m_EcotrophManager = m_EcotrophManager
                            UserInterface.cDynamics.Dynamics.m_PanelToolStrip = scEcotroph1.Panel2
                            UserInterface.cDynamics.Dynamics.m_PanelTabCntl = scEcotroph2.Panel1
                            UserInterface.cDynamics.Dynamics.m_Tree = tvEcotroph
                            UserInterface.cDynamics.DisplayDynamics(e.Node.Text)
                        Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS

                            cmdFO.Invoke("txt files (*.txt)|*.txt|All files (*.*)|*.*")

                            'FileDialog.RestoreDirectory = True
                            If (cmdFO.Result = Windows.Forms.DialogResult.OK) Then
                                m_CatchPastAnalysisFilePath = cmdFO.FileName
                                tvEcotroph.Refresh()
                                tcEcotroph.Refresh()
                                dgvEcotroph1.Refresh()
                                ConnectToComputation.cDynamics.RunDynamics(e.Node.Text, m_CatchPastAnalysisFilePath)

                                If m_EcotrophManager.IsDynamicsRun = True Then
                                    UserInterface.cDynamics.Dynamics.m_EcotrophManager = m_EcotrophManager
                                    UserInterface.cDynamics.Dynamics.m_PanelToolStrip = scEcotroph1.Panel2
                                    UserInterface.cDynamics.Dynamics.m_PanelTabCntl = scEcotroph2.Panel1
                                    UserInterface.cDynamics.Dynamics.m_Tree = tvEcotroph
                                    UserInterface.cDynamics.DisplayDynamics(e.Node.Text)
                                Else 'Dynamics not run because value in CatchPastAnalysis file has error
                                    tsEcotroph.Visible = False
                                    tcEcotroph.Visible = False
                                End If
                            Else 'Cancel in the file dialog box
                                tsEcotroph.Visible = False
                                tcEcotroph.Visible = False
                                Exit Select
                            End If
                    End Select
                Else
                    MsgBox(My.Resources.ERR_MSG_RUN_DYNAMICS_PARAM, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                    tsEcotroph.Visible = False
                    tcEcotroph.Visible = False
                End If
            Case My.Resources.TREE_NODE_ECOTROPH_PLUGIN, My.Resources.TREE_NODE_TRANSPOSE, My.Resources.TREE_NODE_CTSA, _
              My.Resources.TREE_NODE_DIAGNOSIS, My.Resources.TREE_NODE_DYNAMICS
                tsEcotroph.Visible = False
                tcEcotroph.Visible = False
        End Select
        m_PrevSelectedNode = e.Node
    End Sub

    Private Sub tvEcotroph_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tvEcotroph.LostFocus
        tvEcotroph.SelectedNode.BackColor = Drawing.Color.LightGray
    End Sub

    Private Sub tvEcotroph_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvEcotroph.NodeMouseClick
        tvEcotroph.SelectedNode.BackColor = Drawing.Color.LightGoldenrodYellow
    End Sub

    Private Sub tscbxInitializationFwdCal_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscbxInitializationFwdCal.SelectedIndexChanged
        LocateInitializationCellFwdCal()
    End Sub

    Private Sub tscbxTerminalTL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscbxTerminalTL.SelectedIndexChanged
        LocateInitializationCellBwdCal()
    End Sub

    Private Sub tscbxInitializationBwdCal_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscbxInitializationBwdCal.SelectedIndexChanged
        LocateInitializationCellBwdCal()
    End Sub

    Private Sub tsbtnCalculate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbtnCalculate.Click
        Dim SelTabPg As TabPage
        Dim SelTabPgIdx As Integer
        Dim DataGrid As DataGridView
        Dim IsValidSmoothFactor As cUtility.Valid
        Dim IsValidSigma As cUtility.Valid
        Dim IsValidAccess As cUtility.Valid
        Dim IsValidCTSAParameter As cUtility.Valid
        Dim IsValidFwdCalParameter As cUtility.Valid
        Dim IsValidBwdCalParameter As cUtility.Valid
        Dim IsValidDiagnosisParameter As cUtility.Valid
        Dim IsValidEffortMultiplier As cUtility.Valid
        Dim IsValidDynamicsParameter As cUtility.Valid
        Dim IsValidForecastYear As cUtility.Valid
        Dim IsValidCatchMultiplier As cUtility.Valid
        Dim IsValidIndexPPForecast As cUtility.Valid
        Dim IsValidIndexPPPastAnalysis As cUtility.Valid

        Select Case tvEcotroph.SelectedNode.Text
            Case My.Resources.TREE_NODE_AUTO_SMOOTH
                'Update SmoothFactor in memory and file by the latest user inputs if valid
                UserInterface.cTranspose.UpdateSmoothFactor(tstbxSmoothFactor, IsValidSmoothFactor)

                'Update Access values in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cTranspose.UpdateAccessAry(tvEcotroph.SelectedNode.Text, DataGrid, IsValidAccess)

                'Run TransposeAEF again if valid
                HandleIsValidSFAccess(IsValidSmoothFactor, IsValidAccess)
            Case My.Resources.TREE_NODE_OMNI_IDX
                'Update Access values in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cTranspose.UpdateAccessAry(tvEcotroph.SelectedNode.Text, DataGrid, IsValidAccess)

                'Run TransposeOmniIdx again if valid
                HandleIsValidAccess(IsValidAccess)
            Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                'Update Sigma values in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cTranspose.UpdateSigmaAry(DataGrid, IsValidSigma)

                'Update Access values in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                UserInterface.cTranspose.UpdateAccessAry(tvEcotroph.SelectedNode.Text, DataGrid, IsValidAccess)

                'Run TransposeUserDefVal again if valid
                HandleIsValidSigmaAccess(IsValidSigma, IsValidAccess)
            Case My.Resources.TREE_NODE_BASIC_PARAM
                Select Case tvEcotroph.SelectedNode.Parent.Text
                    Case My.Resources.TREE_NODE_CTSA
                        'Update CTSAParameter in memory and file by the latest user inputs if valid
                        SelTabPg = tcEcotroph.SelectedTab
                        SelTabPgIdx = tcEcotroph.SelectedIndex
                        DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                        DataGrid.EndEdit()
                        UserInterface.cCTSA.UpdateCTSAParameter(tstbxWaterTemp, tstbxTETL12, tstbxTETL2, _
                          tstbxAsymptote, tstbxTL50, tstbxSlope, DataGrid, IsValidCTSAParameter)

                        'Run CTSAParameter again if valid
                        HandleIsValidCTSAParameter(IsValidCTSAParameter)
                    Case My.Resources.TREE_NODE_DIAGNOSIS
                        'Update Diagnosis Parameter in memory and file by the latest user inputs if valid
                        SelTabPg = tcEcotroph.SelectedTab
                        SelTabPgIdx = tcEcotroph.SelectedIndex
                        DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                        DataGrid.EndEdit()
                        UserInterface.cDiagnosis.UpdateDiagnosisParameter(tscbxMainDiagnosis, tstbxBeta, DataGrid, _
                          IsValidDiagnosisParameter)

                        'Run Diagnosis Parameter again if valid
                        HandleIsValidDiagnosisParameter(IsValidDiagnosisParameter)
                    Case My.Resources.TREE_NODE_DYNAMICS
                        'Update Dynamics Parameter in memory and file by the latest user inputs if valid
                        SelTabPg = tcEcotroph.SelectedTab
                        SelTabPgIdx = tcEcotroph.SelectedIndex
                        DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                        DataGrid.EndEdit()
                        UserInterface.cDynamics.UpdateDynamicsParameter(tscbxMainDynamics, tstbxBeta, SelTabPg, DataGrid, _
                          IsValidDynamicsParameter)

                        'Run Dynamics Parameter again if valid
                        HandleIsValidDynamicsParameter(IsValidDynamicsParameter)
                End Select
            Case My.Resources.TREE_NODE_FWD_CAL
                'Update Forward Calculation Parameter in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cCTSA.UpdateFwdCalParameter(tscbxInitializationFwdCal, DataGrid, IsValidFwdCalParameter)

                'Run Forward Calculation again if valid
                HandleIsValidFwdCalParameter(IsValidFwdCalParameter)
            Case My.Resources.TREE_NODE_BWD_CAL
                'Update CTSA Backward Calculation Parameter in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cCTSA.UpdateBwdCalParameter(tscbxTerminalTL, tscbxInitializationBwdCal, DataGrid, IsValidBwdCalParameter)

                'Run CTSA Backward Calculation again if valid
                HandleIsValidBwdCalParameter(IsValidBwdCalParameter)
            Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                'Update Effort Multiplier in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cDiagnosis.UpdateEffortMultiplierAry(DataGrid, IsValidEffortMultiplier)

                'Run Diagnosis again if valid
                HandleIsValidEffortMultiplier(IsValidEffortMultiplier)
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                'Update ForecastYear in memory and file by the latest user inputs if valid
                UserInterface.cDynamics.UpdateForecastYear(tstbxRefYear, tstbxNumYear, IsValidForecastYear)

                'Update Catch Multiplier values in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cDynamics.UpdateCatchMultiplierAry(DataGrid, IsValidCatchMultiplier)

                'Update Index PP values in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                UserInterface.cDynamics.UpdateIndexPPForecastAry(DataGrid, IsValidIndexPPForecast)

                'Run Dynamics again if valid
                HandleIsValidForecastYrCatchMtplrIdxPP(IsValidForecastYear, IsValidCatchMultiplier, IsValidIndexPPForecast)
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                'Update Index PP values in memory and file by the latest user inputs if valid
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                DataGrid.EndEdit()
                UserInterface.cDynamics.UpdateIndexPPPastAnalysisAry(DataGrid, IsValidIndexPPPastAnalysis)

                'Run Dynamics again if valid
                HandleIsValidIndexPPPastAnalysis(IsValidIndexPPPastAnalysis)
        End Select
    End Sub

    Private Sub tsbtnPlot_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbtnPlot.Click
        Dim SelTabPg As TabPage
        Dim SelTabPgIdx As Integer
        Dim DataGrid As DataGridView
        Dim EcotrophGraph As frmEcotrophGraph

        SelTabPg = tcEcotroph.SelectedTab
        SelTabPgIdx = tcEcotroph.SelectedIndex
        DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)

        EcotrophGraph = New frmEcotrophGraph()
        EcotrophGraph.m_NodeText = tvEcotroph.SelectedNode.Text
        EcotrophGraph.m_TabPageText = tcEcotroph.SelectedTab.Text
        EcotrophGraph.m_DataGrid = DataGrid
        EcotrophGraph.m_EcotrophManager = m_EcotrophManager
        EcotrophGraph.InitializeGraphClass()
    End Sub

    Private Sub tsbtnImportCatches_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbtnImportCatches.Click

        Dim SelTabPg As TabPage
        Dim SelTabPgIdx As Integer
        Dim DataGrid As DataGridView
        Dim cmdh As CommandHandler = CommandHandler.GetInstance()
        Dim cmdFO As FileOpenCommand = DirectCast(cmdh.GetCommand(FileOpenCommand.COMMAND_NAME), FileOpenCommand)

        ConnectToComputation.cTranspose.Transpose.m_EcotrophManager = m_EcotrophManager
        ConnectToComputation.cTranspose.Transpose.m_PanelToolStrip = scEcotroph1.Panel2
        ConnectToComputation.cTranspose.Transpose.m_PanelTabCntl = scEcotroph2.Panel1

        cmdFO.Invoke("txt files (*.txt)|*.txt|All files (*.*)|*.*", 2)
        If (cmdFO.Result = Windows.Forms.DialogResult.OK) Then
            m_CTSACatchesFilePath = cmdFO.FileName
            If m_EcotrophManager.InputData.ReadFile("CTSACatches", m_EcotrophManager, m_CTSACatchesFilePath) = False Then
                m_EcotrophManager_RunTransposePrgrs(tsEcotroph, PRGRS_BAR_MAX)

                MsgBox(My.Resources.ERR_MSG_CATCH_FILE, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
            Else
                m_EcotrophManager_RunTransposePrgrs(tsEcotroph, PRGRS_BAR_MAX)

                Select Case m_EcotrophManager.InputData.TransposeAlgorImport
                    Case My.Resources.TREE_NODE_AUTO_SMOOTH
                        ConnectToComputation.cTranspose.RunTransposeAEFCatches()
                    Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                        ConnectToComputation.cTranspose.RunTransposeUserDefValCatches()
                    Case Else
                        MsgBox(My.Resources.ERR_MSG_CATCH_FILE, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                        tsEcotroph.Visible = False
                        tcEcotroph.Visible = False
                        Exit Sub
                End Select

                'Re-display CTSA Parameter user interface including the tool strip
                UserInterface.cCTSA.CTSA.m_EcotrophManager = m_EcotrophManager
                UserInterface.cCTSA.CTSA.m_PanelToolStrip = scEcotroph1.Panel2
                UserInterface.cCTSA.CTSA.m_PanelTabCntl = scEcotroph2.Panel1
                UserInterface.cCTSA.CTSA.m_Tree = tvEcotroph
                UserInterface.cCTSA.DisplayCTSAParameter()
                'The catch column is re-populated with the transposed (AEF or user def sigma)catches
                SelTabPg = tcEcotroph.SelectedTab
                SelTabPgIdx = tcEcotroph.SelectedIndex
                DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)
                Select Case m_EcotrophManager.InputData.TransposeAlgorImport
                    Case My.Resources.TREE_NODE_AUTO_SMOOTH
                        For Row As Integer = 1 To DataGrid.RowCount
                            DataGrid.Item(3 - 1, Row - 1).Value = m_EcotrophManager.AEFCatches(Row).ToString("F4")
                            DataGrid.EndEdit()
                        Next
                    Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                        For Row As Integer = 1 To DataGrid.RowCount
                            DataGrid.Item(3 - 1, Row - 1).Value = m_EcotrophManager.UserDefValCatches(Row).ToString("F4")
                            DataGrid.EndEdit()
                        Next
                End Select
            End If
        End If
    End Sub

    Private Sub tsbtnSetDefault_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbtnSetDefault.Click
        Dim SelTabPg As TabPage
        Dim SelTabPgIdx As Integer
        Dim DataGrid As DataGridView

        SelTabPg = tcEcotroph.SelectedTab
        SelTabPgIdx = tcEcotroph.SelectedIndex
        DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)

        Select Case tvEcotroph.SelectedNode.Text
            Case My.Resources.TREE_NODE_BASIC_PARAM
                Select Case tvEcotroph.SelectedNode.Parent.Text
                    Case My.Resources.TREE_NODE_CTSA
                        tstbxWaterTemp.Text = CStr(cUtility.DEFAULT_CTSA_WATER_TEMP)
                        tstbxTETL12.Text = CStr(cUtility.DEFAULT_CTSA_TE_TL12)
                        tstbxTETL2.Text = CStr(cUtility.DEFAULT_CTSA_TE_TL2)
                        tstbxAsymptote.Text = CStr(cUtility.DEFAULT_CTSA_ASYMPTOTE)
                        tstbxTL50.Text = CStr(cUtility.DEFAULT_CTSA_TL50)
                        tstbxSlope.Text = CStr(cUtility.DEFAULT_CTSA_SLOPE)
                        For Row As Integer = 1 To DataGrid.RowCount
                            DataGrid.Item(3 - 1, Row - 1).Value = cUtility.DEFAULT_CTSA_CATCHES.ToString("F4")
                            DataGrid.Item(7 - 1, Row - 1).Value = cUtility.DEFAULT_CTSA_TOPD.ToString("F4")
                            DataGrid.Item(8 - 1, Row - 1).Value = cUtility.DEFAULT_CTSA_FORMD.ToString("F4")
                            DataGrid.EndEdit()
                        Next
                    Case My.Resources.TREE_NODE_DIAGNOSIS
                        tstbxBeta.Text = CStr(cUtility.DEFAULT_DIAGNOSIS_BETA)
                        For Row As Integer = 1 To DataGrid.RowCount
                            DataGrid.Item(15 - 1, Row - 1).Value = cUtility.DEFAULT_DIAGNOSIS_TOPD.ToString("F4")
                            DataGrid.Item(16 - 1, Row - 1).Value = cUtility.DEFAULT_DIAGNOSIS_FORMD.ToString("F4")
                            DataGrid.EndEdit()
                        Next
                    Case My.Resources.TREE_NODE_DYNAMICS
                        tstbxBeta.Text = CStr(cUtility.DEFAULT_DYNAMICS_BETA)
                        For Row As Integer = 1 To DataGrid.RowCount
                            DataGrid.Item(15 - 1, Row - 1).Value = cUtility.DEFAULT_DYNAMICS_TOPD.ToString("F4")
                            DataGrid.Item(16 - 1, Row - 1).Value = cUtility.DEFAULT_DYNAMICS_FORMD.ToString("F4")
                            DataGrid.EndEdit()
                        Next
                End Select
        End Select
    End Sub

    Private Sub tcEcotroph_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tcEcotroph.SelectedIndexChanged
        Dim TabPg As TabPage

        TabPg = tcEcotroph.SelectedTab
        If TabPg Is Nothing Then Return
        cUtility.DisplayToolStripData(scEcotroph1.Panel2, scEcotroph2.Panel1, tsEcotroph, tvEcotroph.SelectedNode.Text, _
          tvEcotroph.SelectedNode.Parent.Text, TabPg.Text)
    End Sub

    'Private Sub m_Transpose_AddToolStrip() Handles m_Transpose.AddToolStrip
    '    scEcotroph1.Panel2.Controls.Add(tsEcotroph)
    'End Sub

    Private Sub m_EcotrophManager_RunTransposePrgrs(ByVal ToolStp As ToolStrip, ByVal BarMax As Integer) Handles m_EcotrophManager.RunTransposePrgrs
        Dim ToolStpPrgBar As ToolStripProgressBar

        ToolStpPrgBar = CType(ToolStp.Items("tspgbProgressBar"), ToolStripProgressBar)
        ToolStpPrgBar.Maximum = BarMax
        If ToolStpPrgBar.Value < ToolStpPrgBar.Maximum Then
            ToolStpPrgBar.Value = ToolStpPrgBar.Value + 1
        Else
            ToolStpPrgBar.Value = 0
        End If
        'tspgbProgressBar.Maximum = BarMax
        'If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
        '    tspgbProgressBar.Value = tspgbProgressBar.Value + 1
        'Else
        '    tspgbProgressBar.Value = 0
        'End If
    End Sub

    Private Sub m_EcotrophManager_CTSAFwdCalIterationInfo(ByVal KineticCriteria As Double) Handles m_EcotrophManager.CTSAFwdCalIterationInfo
        If MsgBox(My.Resources.INFO_MSG_KINETIC_CRITERIA & KineticCriteria.ToString("F6") & Chr(13) & Chr(10) & _
          My.Resources.INFO_MSG_CONTIN_ITER, MsgBoxStyle.YesNo, My.Resources.INFO_TITLE_ITER_NOT_CONVERGE) = MsgBoxResult.Yes Then
            m_EcotrophManager.IsFwdCalIterationContinue = True
        Else
            m_EcotrophManager.IsFwdCalIterationContinue = False
        End If
    End Sub

    Private Sub m_EcotrophManager_CTSABwdCalIterationInfo(ByVal KineticCriteria As Double) Handles m_EcotrophManager.CTSABwdCalIterationInfo
        If MsgBox(My.Resources.INFO_MSG_KINETIC_CRITERIA & KineticCriteria.ToString("F6") & Chr(13) & Chr(10) & _
                  My.Resources.INFO_MSG_CONTIN_ITER, MsgBoxStyle.YesNo, My.Resources.INFO_TITLE_ITER_NOT_CONVERGE) = MsgBoxResult.Yes Then
            m_EcotrophManager.IsBwdCalIterationContinue = True
        Else
            m_EcotrophManager.IsBwdCalIterationContinue = False
        End If
    End Sub

    Private Sub m_EcotrophManager_DiagnosisIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double) Handles m_EcotrophManager.DiagnosisIterationInfo
        If MsgBox(My.Resources.INFO_MSG_PROD_CRITERIA & FlowCriteria.ToString("F6") & Chr(13) & Chr(10) & _
                              My.Resources.INFO_MSG_KINETIC_CRITERIA & KineticCriteria.ToString("F6") & Chr(13) & Chr(10) & _
                              My.Resources.INFO_MSG_CONTIN_ITER, MsgBoxStyle.YesNo, My.Resources.INFO_TITLE_ITER_NOT_CONVERGE) = MsgBoxResult.Yes Then
            m_EcotrophManager.IsDiagnosisIterationContinue = True
        Else
            m_EcotrophManager.IsDiagnosisIterationContinue = False
        End If
    End Sub

    Private Sub m_EcotrophManager_DynamicsIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double) Handles m_EcotrophManager.DynamicsIterationInfo
        If MsgBox(My.Resources.INFO_MSG_PROD_CRITERIA & FlowCriteria.ToString("F6") & Chr(13) & Chr(10) & _
                      My.Resources.INFO_MSG_KINETIC_CRITERIA & KineticCriteria.ToString("F6") & Chr(13) & Chr(10) & _
                      My.Resources.INFO_MSG_CONTIN_ITER, MsgBoxStyle.YesNo, My.Resources.INFO_TITLE_ITER_NOT_CONVERGE) = MsgBoxResult.Yes Then
            m_EcotrophManager.IsDynamicsIterationContinue = True
        Else
            m_EcotrophManager.IsDynamicsIterationContinue = False
        End If
    End Sub

    Private Sub m_EcotrophManager_CatchPastAnalysisErr() Handles m_EcotrophManager.CatchPastAnalysisErr
        MsgBox(My.Resources.ERR_MSG_CATCH_PAST_ANALYSIS_FILE, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
        'tvEcotroph_AfterSelect() will handle tsEcotroph.Visible and tcEcotroph.Visible
    End Sub
#End Region 'Private Events

#Region "Helper methods"
    Private Sub LocateInitializationCellFwdCal()
        Dim SelTabPg As TabPage
        Dim SelTabPgIdx As Integer
        Dim DataGrid As DataGridView
        Dim CellStyle As DataGridViewCellStyle

        SelTabPg = tcEcotroph.SelectedTab
        SelTabPgIdx = tcEcotroph.SelectedIndex
        DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)

        Windows.Forms.Cursor.Current = Cursors.WaitCursor
        cUtility.SetGridColumnPropertyDefault(DataGrid)
        cUtility.SetGridRowPropertyDefault(DataGrid)
        cUtility.SetGridCellPropertyDefault(DataGrid)

        'Set up grid columns
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH
        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.ReadOnly = False

        For Col As Integer = 0 To 1
            For Row As Integer = 0 To m_EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next
        For col As Integer = 2 To 3
            For Row As Integer = 2 To m_EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(col, Row).ReadOnly = True
            Next
        Next
        Select Case tscbxInitializationFwdCal.Text
            Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL1
                DataGrid.Item(2, 0).ReadOnly = True
                DataGrid.Item(2, 1).ReadOnly = True
                DataGrid.Item(3, 1).ReadOnly = True
            Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL2
                DataGrid.Item(2, 0).ReadOnly = True
                DataGrid.Item(2, 1).ReadOnly = True
                DataGrid.Item(3, 0).ReadOnly = True
            Case My.Resources.DROP_DWN_LST_ITM_PROD_TL1
                DataGrid.Item(3, 0).ReadOnly = True
                DataGrid.Item(2, 1).ReadOnly = True
                DataGrid.Item(3, 1).ReadOnly = True
            Case My.Resources.DROP_DWN_LST_ITM_PROD_TL2
                DataGrid.Item(2, 0).ReadOnly = True
                DataGrid.Item(3, 0).ReadOnly = True
                DataGrid.Item(3, 1).ReadOnly = True
        End Select
        For Col As Integer = 4 To DataGrid.ColumnCount - 1
            For Row As Integer = 0 To m_EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next

        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.LightGreen
        Select Case tscbxInitializationFwdCal.Text
            Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL1
                DataGrid.Item(3, 0).Style = CellStyle
            Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL2
                DataGrid.Item(3, 1).Style = CellStyle
            Case My.Resources.DROP_DWN_LST_ITM_PROD_TL1
                DataGrid.Item(2, 0).Style = CellStyle
            Case My.Resources.DROP_DWN_LST_ITM_PROD_TL2
                DataGrid.Item(2, 1).Style = CellStyle
        End Select
        Windows.Forms.Cursor.Current = Cursors.Default
    End Sub

    Private Sub LocateInitializationCellBwdCal()
        Dim SelTabPg As TabPage
        Dim SelTabPgIdx As Integer
        Dim DataGrid As DataGridView
        Dim RowTTL As Integer
        Dim CellStyle As DataGridViewCellStyle

        SelTabPg = tcEcotroph.SelectedTab
        SelTabPgIdx = tcEcotroph.SelectedIndex
        DataGrid = CType(SelTabPg.Controls("dgvEcotroph" & CStr(SelTabPgIdx + 1)), DataGridView)

        Windows.Forms.Cursor.Current = Cursors.WaitCursor
        cUtility.SetGridColumnPropertyDefault(DataGrid)
        cUtility.SetGridRowPropertyDefault(DataGrid)
        cUtility.SetGridCellPropertyDefault(DataGrid)

        'Set up grid columns
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH
        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.ReadOnly = False

        For Col As Integer = 0 To 3
            For Row As Integer = 0 To m_EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next
        RowTTL = CInt((Int(CSng(tscbxTerminalTL.Text)) - 2) * 10 + CSng((CDbl(tscbxTerminalTL.Text) - Int(CSng(tscbxTerminalTL.Text))) * 10) + 2) _
          - 1
        For col As Integer = 4 To 5
            For Row As Integer = 0 To m_EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                If Row <> RowTTL Then DataGrid.Item(col, Row).ReadOnly = True
            Next
        Next
        Select Case tscbxInitializationBwdCal.Text
            Case My.Resources.DROP_DWN_LST_ITM_FISH_LOSS_RATE_TLL
                DataGrid.Item(5, RowTTL).ReadOnly = True
            Case My.Resources.DROP_DWN_LST_ITM_ACCESS_FISH_MORTALITY_TTL
                DataGrid.Item(4, RowTTL).ReadOnly = True
        End Select
        For Col As Integer = 6 To DataGrid.ColumnCount - 1
            For Row As Integer = 0 To m_EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next

        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.LightGreen
        Select Case tscbxInitializationBwdCal.Text
            Case My.Resources.DROP_DWN_LST_ITM_FISH_LOSS_RATE_TLL
                DataGrid.Item(4, RowTTL).Style = CellStyle
            Case My.Resources.DROP_DWN_LST_ITM_ACCESS_FISH_MORTALITY_TTL
                DataGrid.Item(5, RowTTL).Style = CellStyle
        End Select
        Windows.Forms.Cursor.Current = Cursors.Default
    End Sub

    Private Sub HandleIsValidSFAccess(ByVal IsValidSmoothFactor As cUtility.Valid, ByVal IsValidAccess As cUtility.Valid)
        Select Case IsValidSmoothFactor * IsValidAccess
            Case cUtility.Valid.T, cUtility.Valid.NA
                'Run and display Transpose again
                ConnectToComputation.cTranspose.RunTransposeAEF()
                UserInterface.cTranspose.DisplayTransposeAEF()
            Case cUtility.Valid.F
                Select Case IsValidSmoothFactor
                    Case cUtility.Valid.F
                        MsgBox(My.Resources.ERR_MSG_SMOOTH_FACTOR, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                        tstbxSmoothFactor.Focus()
                        'Exit Sub
                End Select
                Select Case IsValidAccess
                    Case cUtility.Valid.F
                        MsgBox(My.Resources.ERR_MSG_ACCESS, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                        'Exit Sub
                    Case cUtility.Valid.NA
                        'Exit Sub
                End Select
        End Select
    End Sub

    Private Sub HandleIsValidAccess(ByVal IsValidAccess As cUtility.Valid)
        Select Case IsValidAccess
            Case cUtility.Valid.T
                'Run and display Transpose again
                ConnectToComputation.cTranspose.RunTransposeOmniIdx()
                UserInterface.cTranspose.DisplayTransposeOmniIdx()
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_ACCESS, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                'Exit Sub
            Case cUtility.Valid.NA
                'Exit Sub
        End Select
    End Sub

    Private Sub HandleIsValidSigmaAccess(ByVal IsValidSigma As cUtility.Valid, ByVal IsValidAccess As cUtility.Valid)
        Select Case IsValidSigma * IsValidAccess
            Case cUtility.Valid.T, cUtility.Valid.NA
                'Run and display Transpose again
                ConnectToComputation.cTranspose.RunTransposeUserDefVal()
                UserInterface.cTranspose.DisplayTransposeUserDefVal()
            Case cUtility.Valid.F, Is > cUtility.Valid.NA
                Select Case IsValidSigma
                    Case cUtility.Valid.F
                        MsgBox(My.Resources.ERR_MSG_SIGMA, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                        'Exit Sub
                    Case cUtility.Valid.NA
                        'Exit Sub
                End Select
                Select Case IsValidAccess
                    Case cUtility.Valid.F
                        MsgBox(My.Resources.ERR_MSG_ACCESS, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                        'Exit Sub
                    Case cUtility.Valid.NA
                        'Exit Sub
                End Select
        End Select
    End Sub

    Private Sub HandleIsValidCTSAParameter(ByVal IsValidCTSAParameter As cUtility.Valid)
        Select Case IsValidCTSAParameter
            Case cUtility.Valid.T
                'Run and display CTSA again
                ConnectToComputation.cCTSA.RunCTSAParameter()
                UserInterface.cCTSA.DisplayCTSAParameter()
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_CTSAPARAMETER, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                'Exit Sub
        End Select
    End Sub

    Private Sub HandleIsValidFwdCalParameter(ByVal IsValidFwdCalParameter As cUtility.Valid)
        Select Case IsValidFwdCalParameter
            Case cUtility.Valid.T
                'Run and display CTSA Backward Calculation again
                ConnectToComputation.cCTSA.RunCTSAFwdCal()
                UserInterface.cCTSA.DisplayCTSAFwdCal()
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_FWDCALPARAMETER, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                'Exit Sub
        End Select
    End Sub

    Private Sub HandleIsValidBwdCalParameter(ByVal IsValidBwdCalParameter As cUtility.Valid)
        Select Case IsValidBwdCalParameter
            Case cUtility.Valid.T
                'Run and display CTSA Backward Calculation again
                ConnectToComputation.cCTSA.RunCTSABwdCal()
                UserInterface.cCTSA.DisplayCTSABwdCal()
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_BWDCALPARAMETER, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                'Exit Sub
        End Select
    End Sub

    Private Sub HandleIsValidDiagnosisParameter(ByVal IsValidDiagnosisParameter As cUtility.Valid)
        Select Case IsValidDiagnosisParameter
            Case cUtility.Valid.T
                Select Case tscbxMainDiagnosis.Text
                    Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                        If m_EcotrophManager.IsAEFRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_TP_AUTO_EMPIR_FUNCT, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDiagnosis.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                        If m_EcotrophManager.IsOmniIdxRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_TP_OMNI_IDX, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDiagnosis.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                        If m_EcotrophManager.IsUserDefValRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_TP_USER_DEF_SIGMA, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDiagnosis.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                        If m_EcotrophManager.IsFwdCalRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_CTSA_FWD_CAL, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDiagnosis.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                        If m_EcotrophManager.IsBwdCalRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_CTSA_BWD_CAL, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDiagnosis.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                End Select
                'Run and display Diagnosis Parameter again
                ConnectToComputation.cDiagnosis.RunDiagnosisParameter(tscbxMainDiagnosis.Text)
                UserInterface.cDiagnosis.DisplayDiagnosisParameter(tscbxMainDiagnosis.Text)
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_DIAGNOSISPARAMETER, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                'Exit Sub
        End Select
    End Sub

    Private Sub HandleIsValidEffortMultiplier(ByVal IsValidEffortMultiplier As cUtility.Valid)
        Select Case IsValidEffortMultiplier
            Case cUtility.Valid.T
                'Run and display Diagnosis again
                ConnectToComputation.cDiagnosis.RunDiagnosis(My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR)
                UserInterface.cDiagnosis.DisplayDiagnosis(tvEcotroph.SelectedNode.Text)
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_EFF_MTPLR, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
        End Select
    End Sub

    Private Sub HandleIsValidDynamicsParameter(ByVal IsValidDynamicsParameter As cUtility.Valid)
        Select Case IsValidDynamicsParameter
            Case cUtility.Valid.T
                Select Case tscbxMainDynamics.Text
                    Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                        If m_EcotrophManager.IsAEFRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_TP_AUTO_EMPIR_FUNCT, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDynamics.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                        If m_EcotrophManager.IsOmniIdxRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_TP_OMNI_IDX, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDynamics.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                        If m_EcotrophManager.IsUserDefValRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_TP_USER_DEF_SIGMA, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDynamics.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                        If m_EcotrophManager.IsFwdCalRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_CTSA_FWD_CAL, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDynamics.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                    Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                        If m_EcotrophManager.IsBwdCalRun = False Then
                            MsgBox(My.Resources.ERR_MSG_RUN_CTSA_BWD_CAL, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_RUN_SEQ)
                            tscbxMainDynamics.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                            tsEcotroph.Visible = False
                            tcEcotroph.Visible = False
                            Exit Sub
                        End If
                End Select
                'Run and display Dynamics Parameter again
                ConnectToComputation.cDynamics.RunDynamicsParameter(tscbxMainDynamics.Text)
                UserInterface.cDynamics.DisplayDynamicsParameter(tscbxMainDynamics.Text)
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_DYNAMICSPARAMETER, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                'Exit Sub
        End Select
    End Sub

    Private Sub HandleIsValidForecastYrCatchMtplrIdxPP(ByVal IsValidForecastYear As cUtility.Valid, ByVal IsValidCatchMultiplier As cUtility.Valid, _
      ByVal IsValidIndexPPForecast As cUtility.Valid)
        'Select Case IsValidForecastYear
        '    Case cUtility.Valid.T
        '        'Run and display Dynamics again
        '        ConnectToComputation.cDynamics.RunDynamics(My.Resources.TREE_NODE_CATCH_FORECAST)
        '        UserInterface.cDynamics.DisplayDynamics(My.Resources.TREE_NODE_CATCH_FORECAST)
        '    Case cUtility.Valid.F
        '        MsgBox(My.Resources.ERR_MSG_FORECASTYEAR, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
        'End Select
        Select Case IsValidForecastYear * IsValidCatchMultiplier * IsValidIndexPPForecast
            Case cUtility.Valid.T
                'Run and display Dynamics again
                ConnectToComputation.cDynamics.RunDynamics(My.Resources.TREE_NODE_CATCH_FORECAST)
                UserInterface.cDynamics.DisplayDynamics(My.Resources.TREE_NODE_CATCH_FORECAST)
            Case cUtility.Valid.F
                Select Case IsValidForecastYear
                    Case cUtility.Valid.F
                        MsgBox(My.Resources.ERR_MSG_FORECASTYEAR, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                End Select
                Select Case IsValidCatchMultiplier
                    Case cUtility.Valid.F
                        MsgBox(My.Resources.ERR_MSG_CATCH_MTPLR, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                End Select
                Select Case IsValidIndexPPForecast
                    Case cUtility.Valid.F
                        MsgBox(My.Resources.ERR_MSG_IDX_PP, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
                End Select
        End Select
    End Sub

    Private Sub HandleIsValidIndexPPPastAnalysis(ByVal IsValidIndexPP As cUtility.Valid)
        Select Case IsValidIndexPP
            Case cUtility.Valid.T
                'Run and display Dynamics again
                ConnectToComputation.cDynamics.RunDynamics(My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS, m_CatchPastAnalysisFilePath)
                UserInterface.cDynamics.DisplayDynamics(My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS)
            Case cUtility.Valid.F
                MsgBox(My.Resources.ERR_MSG_IDX_PP, MsgBoxStyle.OkOnly, My.Resources.ERR_TITLE_INPUT)
        End Select
    End Sub
#End Region 'Helper methods

End Class