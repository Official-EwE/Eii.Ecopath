'==============================================================================
'
' $Log: frmNetworkAnalysis.vb,v $
' Revision 1.1  2008/09/26 07:30:57  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.39  2008/07/01 19:13:11  sherman
' Merged branch - Fix_Ecopat_EcosimUpdateBug
'
' Revision 1.38  2008/06/30 23:53:25  joeh
' Call RunRequiredPrimaryProd() of cRequiredPrimaryProduction when computing Ecosim NA indices with PPR
'
' Revision 1.37  2008/06/25 01:53:40  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.36  2008/06/24 18:08:38  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.35  2008/06/24 00:52:28  joeh
' Ecosim NA indice plots are no longer displayed in a pop up form, rather they are displayed in the same form where  we have the NA tree view
'
' Revision 1.34  2007/09/13 04:15:19  sherman
' Updated sponsors, try #2
'
' Revision 1.32  2007/07/09 23:05:48  joeh
' Move hard coded strings to resource file
'
' Revision 1.31  2007/07/09 19:44:45  joeh
' Move hard coded strings to resource file
'
' Revision 1.30  2007/06/28 19:28:30  joeh
' Add tool strip button Cancel
'
' Revision 1.29  2007/06/22 00:35:32  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.28  2007/06/21 00:14:38  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.27  2007/06/20 23:33:13  joeh
' Add cEcosimNetworkAnalysis
'
' Revision 1.26  2007/06/20 18:52:56  joeh
' Rename SetUpPanel() to RunRequiredPrimaryProd()
'
' Revision 1.25  2007/06/20 18:49:08  joeh
' Rename SetUpPanel() to RunFindPathwaysCyclesAll()
'
' Revision 1.24  2007/06/20 18:45:40  joeh
' Rename SetUpPanel() to RunNetworkAnalysis()
'
' Revision 1.23  2007/06/20 18:13:56  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================

Option Strict On
Option Explicit On

Public Class frmNetworkAnalysis
    Private WithEvents m_NetworkManager As cNetworkManager

    Private WithEvents m_NetworkAnalysis As cNetworkAnalysis
    Private WithEvents m_FindPathwaysCyclesAll As cFindPathwaysCyclesAll
    Private WithEvents m_RequiredPrimaryProduction As cRequiredPrimaryProduction
    Private WithEvents m_EcosimNetworkAnalysis As cEcosimNetworkAnalysis

    Private m_AbsoluteFlows As cAbsoluteFlows
    Private m_RelativeFlows As cRelativeFlows
    Private m_TransferEfficiency As cTransferEfficiency
    Private m_FlowPyramid As cFlowPyramid
    Private m_BiomassByTrophicLevel As cBiomassByTrophicLevel
    Private m_BiomassPyramid As cBiomassPyramid
    Private m_CatchByTrophicLevel As cCatchByTrophicLevel
    Private m_CatchPyramid As cCatchPyramid

    Private m_FromAllCombined As cFromAllCombined
    Private m_FromDetritus As cFromDetritus
    Private m_FromPrimaryProd As cFromPrimaryProd

    Private m_ForConsumpOfAllGp As cForConsumpOfAllGp
    Private m_ForHarvestOfAllGp As cForHarvestOfAllGp

    Private m_ImpactData As cImpactData
    Private m_GraphOfMixedTrophicImpact As cGraphOfMixedTrophicImpact
    Private m_HideGroupsClass As cHideGroups
    Private m_HideGroupsForm As frmHideGroups

    Private m_AscendencyByGroup As cByGroup
    Private m_AscendencyTotal As cTotal

    Private m_FlowFromDetritus As cFlowFromDetritus

    Private WithEvents m_TL1ToConsumerPathways As TL1ToConsumer.cPathways
    Private m_TL1ToConsumerSummaryPathways As TL1ToConsumer.cSummaryPathways
    Private WithEvents m_TL1ToPreyToConsumerPathways As TL1ToPreyToConsumer.cPathways
    Private m_TL1ToPreyToConsumerSummaryPathways As TL1ToPreyToConsumer.cSummaryPathways
    Private WithEvents m_PreyToPredatorPathways As PreyToPredator.cPathways
    Private m_PreyToPredatorSummaryPathways As PreyToPredator.cSummaryPathways
    Private m_CyclesLivingPathways As CyclesLiving.cPathways
    Private m_CyclesLivingSummaryPathways As CyclesLiving.cSummaryPathways
    Private m_CyclesAllPathways As CyclesAll.cPathways
    Private m_CyclesAllSummaryPathways As CyclesAll.cSummaryPathways
    Private m_CyclingAndPathLen As cCyclingAndPathLen

    Private WithEvents m_IndicesWithoutPPREstClass As cIndicesWithoutPPREst
    'Private m_IndicesWithoutPPREstForm As frmIndicesWithoutPPREst
    Private WithEvents m_IndicesWithPPREstClass As cIndicesWithPPREst
    'Private m_IndicesWithPPREstForm As frmIndicesWithPPREst

    Private m_AlgorithmRunning As String
    Private m_ParentOfPathwayNode As String
    Private m_SelectionOfComboBox1 As Integer
    Private m_SelectionOfComboBox2 As Integer
    Private m_FormActivatedCounter As Integer

    Public Sub New(ByRef theNetworkManager As cNetworkManager)
        Me.InitializeComponent()

        m_NetworkManager = theNetworkManager
        'm_NetworkManager.RunMainNetwork()
        'm_NetworkManager.RunRequiredPrimaryProd()

    End Sub

    Private Sub frmNetworkNav_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        m_FormActivatedCounter = m_FormActivatedCounter + 1
        'If Not ndRelativeFlows Is Nothing Then
        '    tvNetworkAnalysis.SelectedNode = ndRelativeFlows
        '    tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.SkyBlue
        'End If
        If m_FormActivatedCounter = 1 Then
            scNetworkAnalysis.Panel2.Controls.RemoveByKey("tsNetworkAnalysis")
            dgvNetworkAnalysis.Visible = False
            zgcNetworkAnalysis.Visible = False
            tlpNetworkAnalysis.Visible = True
            'tvNetworkAnalysis.Nodes("ndEwENetworkAnalysisPlugin").Expand()
        End If
    End Sub

    Private Sub frmNetworkAnalysis_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        tvNetworkAnalysis.Nodes("ndEwENetworkAnalysisPlugin").Expand()
    End Sub

    Private Function FindNode(ByVal root As Windows.Forms.TreeNodeCollection, ByVal strText As String) As Windows.Forms.TreeNode
        Dim ret As Windows.Forms.TreeNode = Nothing

        For Each nd As Windows.Forms.TreeNode In root
            If nd.Text.Equals(strText) Then
                ret = nd
                Exit For
            Else
                If nd.GetNodeCount(False) <> 0 Then
                    ret = FindNode(nd.Nodes, strText)
                    If Not ret Is Nothing Then Exit For
                End If
            End If
        Next

        Return ret
    End Function

    'Private Function FindNode(ByVal root As Windows.Forms.TreeNodeCollection, ByVal intIndex As Integer) As Windows.Forms.TreeNode
    '    Dim ret As Windows.Forms.TreeNode = Nothing

    '    For Each nd As Windows.Forms.TreeNode In root
    '        If nd.Index.Equals(intIndex) Then
    '            ret = nd
    '            Exit For
    '        Else
    '            If nd.GetNodeCount(False) <> 0 Then
    '                ret = FindNode(nd.Nodes, intIndex)
    '                If Not ret Is Nothing Then Exit For
    '            End If
    '        End If
    '    Next

    '    Return ret
    'End Function


    Private Sub tvNetworkAnalysis_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvNetworkAnalysis.AfterSelect
        Select Case e.Node.Text
            Case My.Resources.TREE_NODE_REL_FLOWS
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_RelativeFlows = cRelativeFlows.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_RelativeFlows.DisplayData()
            Case My.Resources.TREE_NODE_ABS_FLOWS
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_AbsoluteFlows = cAbsoluteFlows.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_AbsoluteFlows.DisplayData()
            Case My.Resources.TREE_NODE_TRANSFER_EFF
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_TransferEfficiency = cTransferEfficiency.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_TransferEfficiency.DisplayData()
            Case My.Resources.TREE_NODE_FLOW_PYR
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_HideGroupsForm = frmHideGroups.GetInstance(m_NetworkManager)
                m_FlowPyramid = cFlowPyramid.GetInstance(m_NetworkManager, m_HideGroupsForm, scNetworkAnalysis.Panel2)
                m_FlowPyramid.SetUpPanel()
                m_FlowPyramid.CreatePlot()
            Case My.Resources.TREE_NODE_BIOMASS_TRP_LVL
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_HideGroupsForm = frmHideGroups.GetInstance(m_NetworkManager)
                m_BiomassByTrophicLevel = cBiomassByTrophicLevel.GetInstance(m_NetworkManager, m_HideGroupsForm, scNetworkAnalysis.Panel2)
                m_BiomassByTrophicLevel.DisplayData()
            Case My.Resources.TREE_NODE_BIOMASS_PYR
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_BiomassPyramid = cBiomassPyramid.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_BiomassPyramid.SetUpPanel()
                m_BiomassPyramid.CreatePlot()
            Case My.Resources.TREE_NODE_CATCH_TRP_LVL
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_HideGroupsForm = frmHideGroups.GetInstance(m_NetworkManager)
                m_CatchByTrophicLevel = cCatchByTrophicLevel.GetInstance(m_NetworkManager, m_HideGroupsForm, scNetworkAnalysis.Panel2)
                m_CatchByTrophicLevel.DisplayData()
            Case My.Resources.TREE_NODE_CATCH_PYR
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_CatchPyramid = cCatchPyramid.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_CatchPyramid.SetUpPanel()
                m_CatchPyramid.CreatePlot()
            Case My.Resources.TREE_NODE_FROM_PRIM_PRODUCER
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_FromPrimaryProd = cFromPrimaryProd.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_FromPrimaryProd.DisplayData()
            Case My.Resources.TREE_NODE_FROM_DET
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_FromDetritus = cFromDetritus.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_FromDetritus.DisplayData()
            Case My.Resources.TREE_NODE_FROM_ALL_COMB
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_FromAllCombined = cFromAllCombined.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_FromAllCombined.DisplayData()
            Case My.Resources.TREE_NODE_FOR_HARV_ALL_GRP
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                If Not m_NetworkManager.IsRequiredPrimaryProdRun Then
                    m_RequiredPrimaryProduction = cRequiredPrimaryProduction.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_RequiredPrimaryProduction.RunRequiredPrimaryProd()
                End If
                m_ForHarvestOfAllGp = cForHarvestOfAllGp.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_ForHarvestOfAllGp.DisplayData()
            Case My.Resources.TREE_NODE_FOR_CONSUM_ALL_GRP
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                If Not m_NetworkManager.IsRequiredPrimaryProdRun Then
                    m_RequiredPrimaryProduction = cRequiredPrimaryProduction.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_RequiredPrimaryProduction.RunRequiredPrimaryProd()
                End If
                m_ForConsumpOfAllGp = cForConsumpOfAllGp.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_ForConsumpOfAllGp.DisplayData()
            Case My.Resources.TREE_NODE_IMPACT_DATA  'Mixed trophic impact data
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_ImpactData = cImpactData.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_ImpactData.DisplayData()
            Case My.Resources.TREE_NODE_GRAPH_MTI
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_HideGroupsForm = frmHideGroups.GetInstance(m_NetworkManager)
                m_GraphOfMixedTrophicImpact = cGraphOfMixedTrophicImpact.GetInstance(m_NetworkManager, m_HideGroupsForm, scNetworkAnalysis.Panel2)
                m_GraphOfMixedTrophicImpact.SetUpPanel()
                m_GraphOfMixedTrophicImpact.CreatePlot()
            Case My.Resources.TREE_NODE_SHOW_HIDE_GRP
                m_HideGroupsClass = cHideGroups.GetInstance(scNetworkAnalysis.Panel2)
                m_HideGroupsClass.SetUpPanel()
                m_HideGroupsForm = frmHideGroups.GetInstance(m_NetworkManager)
                m_HideGroupsForm.ShowDialog()
            Case My.Resources.TREE_NODE_TOTAL
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_AscendencyTotal = cTotal.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_AscendencyTotal.DisplayData()
            Case My.Resources.TREE_NODE_BY_GRP
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_AscendencyByGroup = cByGroup.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_AscendencyByGroup.DisplayData()
            Case My.Resources.TREE_NODE_FLOW_DET
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_FlowFromDetritus = cFlowFromDetritus.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_FlowFromDetritus.DisplayData()
            Case My.Resources.TREE_NODE_PATH
                Select Case e.Node.Parent.Text
                    Case My.Resources.TREE_NODE_CONSUM_TL1
                        m_ParentOfPathwayNode = e.Node.Parent.Text
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        m_TL1ToConsumerPathways = TL1ToConsumer.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_TL1ToConsumerPathways.DisplayData()
                    Case My.Resources.TREE_NODE_CONSUM_PREY_TL1
                        m_ParentOfPathwayNode = e.Node.Parent.Text
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        m_TL1ToPreyToConsumerPathways = TL1ToPreyToConsumer.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_TL1ToPreyToConsumerPathways.DisplayData()
                    Case My.Resources.TREE_NODE_PRED_PREY
                        m_ParentOfPathwayNode = e.Node.Parent.Text
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        m_PreyToPredatorPathways = PreyToPredator.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_PreyToPredatorPathways.DisplayData()
                    Case My.Resources.TREE_NODE_CYC_LIVING
                        m_ParentOfPathwayNode = e.Node.Parent.Text
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        m_CyclesLivingPathways = CyclesLiving.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_CyclesLivingPathways.DisplayData()
                    Case My.Resources.TREE_NODE_CYC_ALL
                        Dim Answer As String
                        Answer = CStr(MsgBox(My.Resources.MSG_BOX_CYC_ALL, MsgBoxStyle.YesNo, My.Resources.MSG_BOX_EWE_NA_PLUGIN))
                        If Answer = CStr(vbYes) Then
                            m_ParentOfPathwayNode = e.Node.Parent.Text
                            If Not m_NetworkManager.IsMainNetworkRun Then
                                m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                                m_NetworkAnalysis.RunNetworkAnalysis()
                            End If
                            m_FindPathwaysCyclesAll = cFindPathwaysCyclesAll.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_FindPathwaysCyclesAll.RunFindPathwaysCyclesAll()
                            m_CyclesAllPathways = CyclesAll.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_CyclesAllPathways.DisplayData()
                        Else
                            scNetworkAnalysis.Panel2.Controls.RemoveByKey("tsNetworkAnalysis")
                            dgvNetworkAnalysis.Visible = False
                            zgcNetworkAnalysis.Visible = False
                        End If
                    Case Else
                End Select
            Case My.Resources.TREE_NODE_SUM_PATH
                Select Case e.Node.Parent.Text
                    Case My.Resources.TREE_NODE_CONSUM_TL1
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        If m_ParentOfPathwayNode <> e.Node.Parent.Text Then
                            m_ParentOfPathwayNode = e.Node.Parent.Text
                            m_TL1ToConsumerPathways = TL1ToConsumer.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_TL1ToConsumerPathways.DisplayData()
                        End If
                        m_TL1ToConsumerSummaryPathways = TL1ToConsumer.cSummaryPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_TL1ToConsumerSummaryPathways.DisplayData()
                    Case My.Resources.TREE_NODE_CONSUM_PREY_TL1
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        If m_ParentOfPathwayNode <> e.Node.Parent.Text Then
                            m_ParentOfPathwayNode = e.Node.Parent.Text
                            m_TL1ToPreyToConsumerPathways = TL1ToPreyToConsumer.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_TL1ToPreyToConsumerPathways.DisplayData()
                        End If
                        m_TL1ToPreyToConsumerSummaryPathways = TL1ToPreyToConsumer.cSummaryPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_TL1ToPreyToConsumerSummaryPathways.DisplayData()
                    Case My.Resources.TREE_NODE_PRED_PREY
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        If m_ParentOfPathwayNode <> e.Node.Parent.Text Then
                            m_ParentOfPathwayNode = e.Node.Parent.Text
                            m_PreyToPredatorPathways = PreyToPredator.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_PreyToPredatorPathways.DisplayData()
                        End If
                        m_PreyToPredatorSummaryPathways = PreyToPredator.cSummaryPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_PreyToPredatorSummaryPathways.DisplayData()
                    Case My.Resources.TREE_NODE_CYC_LIVING
                        If Not m_NetworkManager.IsMainNetworkRun Then
                            m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_NetworkAnalysis.RunNetworkAnalysis()
                        End If
                        If m_ParentOfPathwayNode <> e.Node.Parent.Text Then
                            m_CyclesLivingPathways = CyclesLiving.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                            m_CyclesLivingPathways.DisplayData()
                        End If
                        m_CyclesLivingSummaryPathways = CyclesLiving.cSummaryPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_CyclesLivingSummaryPathways.DisplayData()
                    Case My.Resources.TREE_NODE_CYC_ALL
                        If m_ParentOfPathwayNode <> e.Node.Parent.Text Then
                            Dim Answer As String
                            Answer = CStr(MsgBox(My.Resources.MSG_BOX_CYC_ALL, MsgBoxStyle.YesNo, My.Resources.MSG_BOX_EWE_NA_PLUGIN))
                            If Answer = CStr(vbYes) Then
                                If Not m_NetworkManager.IsMainNetworkRun Then
                                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                                    m_NetworkAnalysis.RunNetworkAnalysis()
                                End If
                                m_FindPathwaysCyclesAll = cFindPathwaysCyclesAll.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                                m_FindPathwaysCyclesAll.RunFindPathwaysCyclesAll()
                                m_CyclesAllPathways = CyclesAll.cPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                                m_CyclesAllPathways.DisplayData()
                            Else
                                scNetworkAnalysis.Panel2.Controls.RemoveByKey("tsNetworkAnalysis")
                                dgvNetworkAnalysis.Visible = False
                                zgcNetworkAnalysis.Visible = False
                                Exit Select
                            End If
                        End If
                        m_CyclesAllSummaryPathways = CyclesAll.cSummaryPathways.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_CyclesAllSummaryPathways.DisplayData()
                    Case Else
                End Select
            Case My.Resources.TREE_NODE_CYC_PATH_LEN
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                m_CyclingAndPathLen = cCyclingAndPathLen.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_CyclingAndPathLen.DisplayData()
            Case My.Resources.TREE_NODE_WO_PPR_EST
                If Not m_NetworkManager.IsMainNetworkRun Then
                    m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_NetworkAnalysis.RunNetworkAnalysis()
                End If
                'If Not m_NetworkManager.IsEcosimNetworkWithoutPPREstRun And _
                'Not m_NetworkManager.IsEcosimNetworkWithPPREstRun Then
                If Not m_NetworkManager.IsEcosimNetworkWithoutPPREstRun Then
                    m_EcosimNetworkAnalysis = cEcosimNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_EcosimNetworkAnalysis.RunEcosimNetworkAnalysis(False) 'False->WithoutPPREst
                End If
                m_IndicesWithoutPPREstClass = cIndicesWithoutPPREst.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_IndicesWithoutPPREstClass.SetUpPanel(m_NetworkManager.IsEcosimNetworkWithoutPPREstRun)
                'm_EcosimNetworkAnalysis.RunEcosimNetworkAnalysis() might not be successfully run because Ecosim scenario has not been loaded
                If m_NetworkManager.IsEcosimNetworkWithoutPPREstRun = True Then
                    m_IndicesWithoutPPREstClass.CreatePlot(Me, zgcNetworkAnalysis)
                    scNetworkAnalysis.Panel2.Refresh()
                    'm_IndicesWithoutPPREstForm = frmIndicesWithoutPPREst.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    'm_IndicesWithoutPPREstForm.ShowDialog()
                End If
            Case My.Resources.TREE_NODE_W_PPR_EST
                Dim Answer As String
                If Not m_NetworkManager.IsEcosimNetworkWithPPREstRun Then
                    Answer = CStr(MsgBox(My.Resources.MSG_BOX_EST_PPR, MsgBoxStyle.YesNo, My.Resources.MSG_BOX_EWE_NA_PLUGIN))
                Else
                    Answer = CStr(vbYes)
                End If
                If Answer = CStr(vbYes) Then
                    If Not m_NetworkManager.IsMainNetworkRun Then
                        m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_NetworkAnalysis.RunNetworkAnalysis()
                    End If
                    If Not m_NetworkManager.IsRequiredPrimaryProdRun Then
                        m_RequiredPrimaryProduction = cRequiredPrimaryProduction.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_RequiredPrimaryProduction.RunRequiredPrimaryProd()
                    End If
                    If Not m_NetworkManager.IsEcosimNetworkWithPPREstRun Then
                        m_EcosimNetworkAnalysis = cEcosimNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        m_EcosimNetworkAnalysis.RunEcosimNetworkAnalysis(True) 'True->WithPPREst
                    End If
                    m_IndicesWithPPREstClass = cIndicesWithPPREst.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                    m_IndicesWithPPREstClass.SetUpPanel(m_NetworkManager.IsEcosimNetworkWithPPREstRun)
                    'm_EcosimNetworkAnalysis.RunEcosimNetworkAnalysis() might not be successfully run because Ecosim scenario has not been loaded
                    If m_NetworkManager.IsEcosimNetworkWithPPREstRun = True Then
                        m_IndicesWithPPREstClass.CreatePlot(Me, zgcNetworkAnalysis)
                        scNetworkAnalysis.Panel2.Refresh()
                        'm_IndicesWithPPREstForm = frmIndicesWithPPREst.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                        'm_IndicesWithPPREstForm.ShowDialog()
                    End If
                Else
                    scNetworkAnalysis.Panel2.Controls.RemoveByKey("tsNetworkAnalysis")
                    dgvNetworkAnalysis.Visible = False
                    zgcNetworkAnalysis.Visible = False
                End If
            Case Else
        End Select

    End Sub

    Private Sub tvNetworkAnalysis_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tvNetworkAnalysis.LostFocus
        tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.LightGray
    End Sub

    Private Sub tvNetworkAnalysis_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvNetworkAnalysis.NodeMouseClick
        tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.MintCream
    End Sub

    Private Sub m_TL1ToConsumerPathways_AddToolStrip() Handles m_TL1ToConsumerPathways.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub m_TL1ToPreyToConsumerPathways_AddToolStrip() Handles m_TL1ToPreyToConsumerPathways.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub m_PreyToPredatorPathways_AddToolStrip() Handles m_PreyToPredatorPathways.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub tsbtnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbtnCancel.Click
        Select Case m_AlgorithmRunning
            Case My.Resources.LBL_PPR_CAL_PRGR
                m_NetworkManager.CancelRequiredPrimaryProdRun = True
        End Select
    End Sub

    Private Sub tsbtnOutputIndicesCSV_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbtnOutputIndicesCSV.Click
        Select Case tvNetworkAnalysis.SelectedNode.Text
            Case My.Resources.TREE_NODE_WO_PPR_EST
                'm_IndicesWithoutPPREstClass = cIndicesWithoutPPREst.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_IndicesWithoutPPREstClass.ExtractToCSV()
            Case My.Resources.TREE_NODE_W_PPR_EST
                'm_IndicesWithPPREstClass = cIndicesWithPPREst.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                m_IndicesWithPPREstClass.ExtractToCSV()
        End Select
    End Sub

    Private Sub tscmbSelection1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscmbSelection1.SelectedIndexChanged
        Dim strSelection1 As String
        Dim intSelection1 As Integer

        strSelection1 = CStr(tscmbSelection1.SelectedItem)
        intSelection1 = CInt(strSelection1.Substring(0, InStr(strSelection1, ",") - 1))

        Select Case m_ParentOfPathwayNode
            Case My.Resources.TREE_NODE_CONSUM_TL1
                m_TL1ToConsumerPathways.SetUpGridRow(intSelection1)
            Case My.Resources.TREE_NODE_CONSUM_PREY_TL1
                If m_SelectionOfComboBox1 = 0 Then
                    m_SelectionOfComboBox1 = intSelection1
                Else
                    m_SelectionOfComboBox1 = intSelection1
                    m_TL1ToPreyToConsumerPathways.SetUpGridRow(m_SelectionOfComboBox1, m_SelectionOfComboBox2)
                End If
            Case My.Resources.TREE_NODE_PRED_PREY
                m_PreyToPredatorPathways.SetUpGridRow(intSelection1)
            Case Else
        End Select
    End Sub

    Private Sub tscmbSelection2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscmbSelection2.SelectedIndexChanged
        Dim strSelection2 As String

        strSelection2 = CStr(tscmbSelection2.SelectedItem)
        m_SelectionOfComboBox2 = CInt(strSelection2.Substring(0, InStr(strSelection2, ",") - 1))

        m_TL1ToPreyToConsumerPathways.SetUpGridRow(m_SelectionOfComboBox1, m_SelectionOfComboBox2)

    End Sub

    Private Sub m_NetworkManager_FindCyclesProgress(ByVal iCycle As Integer) Handles m_NetworkManager.FindCyclesProgress
        tspgbProgressBar.Maximum = 50 '500
        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
            tspgbProgressBar.Value += 1
        Else
            tspgbProgressBar.Value = 0
        End If
    End Sub

    Private Sub m_NetworkManager_RunMainNetworkProgress(ByVal iProgress As Integer) Handles m_NetworkManager.RunMainNetworkProgress
        tspgbProgressBar.Maximum = 10 '20000
        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
            tspgbProgressBar.Value += 1
        Else
            tspgbProgressBar.Value = 0
        End If
    End Sub

    Private Sub m_NetworkManager_CalculateRequiredPPProgress(ByVal nPaths As Integer) Handles m_NetworkManager.CalculateRequiredPPProgress
        tspgbProgressBar.Maximum = 5000 '50000
        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
            tspgbProgressBar.Value += 1
        Else
            tspgbProgressBar.Value = 0
        End If

        m_AlgorithmRunning = My.Resources.LBL_PPR_CAL_PRGR
        'tsbtnCancel.PerformClick()
    End Sub

    Private Sub m_NetworkManager_EcosimNetworkProgress(ByVal iTime As Integer) Handles m_NetworkManager.EcosimNetworkProgress
        'pbProgress.PerformStep()
        tspgbProgressBar.Maximum = m_NetworkManager.nEcosimTimesteps  '100
        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
            tspgbProgressBar.Value += 1
        Else
            tspgbProgressBar.Value = 0
        End If
    End Sub

    Private Sub m_EcosimNetworkAnalysis_AddToolStrip() Handles m_EcosimNetworkAnalysis.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub m_NetworkAnalysis_AddToolStrip() Handles m_NetworkAnalysis.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub m_FindPathwaysCyclesAll_AddToolStrip() Handles m_FindPathwaysCyclesAll.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub m_RequiredPrimaryProduction_AddToolStrip() Handles m_RequiredPrimaryProduction.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub m_IndicesWithoutPPREstClass_AddToolStrip() Handles m_IndicesWithoutPPREstClass.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub m_IndicesWithPPREstClass_AddToolStrip() Handles m_IndicesWithPPREstClass.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub
    'Private Sub btRunEcosimNetwork_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim iTime As Integer

    '    'progress of ecosim network
    '    Me.pbProgress.Maximum = m_NetworkManager.nEcosimTimesteps
    '    pbProgress.Value = 0

    '    'bEcosimPPR will make the model run a lot slower
    '    'in EwE5 the user is asked if they want to turn this flag on when they select the 'Indices' option button
    '    m_NetworkManager.bEcosimPPR = True 'Primary production required

    '    m_NetworkManager.RunEcosimNetwork()

    '    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    '    'EwE5 plot from Ecosim 'Indices' option button
    '    System.Console.WriteLine("FIB index, Total Catch, Kemptons Q, TL of catch")
    '    For iTime = 1 To m_NetworkManager.nEcosimTimesteps
    '        System.Console.WriteLine("Time = " & iTime.ToString)
    '        System.Console.WriteLine(m_NetworkManager.FIB(iTime).ToString & ", " _
    '                                & m_NetworkManager.RelativeSumOfCatchPlot(iTime).ToString & ", " _
    '                                & m_NetworkManager.RelativeKemptonsPlot(iTime).ToString & ", " _
    '                                & m_NetworkManager.TLCatchPlot(iTime).ToString)
    '    Next iTime

    '    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    '    'plot Primary production required results
    '    If m_NetworkManager.bEcosimPPR Then

    '        System.Console.WriteLine("Catch PPR, Catch detritus req.")
    '        For iTime = 1 To m_NetworkManager.nEcosimTimesteps
    '            System.Console.WriteLine("Time = " & iTime.ToString)
    '            System.Console.WriteLine(m_NetworkManager.RelativeCatchPPRPlot(iTime).ToString & ", " _
    '                                    & m_NetworkManager.RelativeDetritusReqPlot(iTime).ToString)
    '        Next iTime

    '    End If

    '    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    '    'EwE5 plot from Ecosim "TL's" option button
    '    System.Console.WriteLine("TL of catch")
    '    For iTime = 1 To m_NetworkManager.nEcosimTimesteps
    '        System.Console.WriteLine("Time = " & iTime.ToString)
    '        For iGrp As Integer = 1 To m_NetworkManager.nGroups
    '            System.Console.Write(m_NetworkManager.TLSimPlot(iGrp, iTime).ToString & ", ")
    '        Next iGrp
    '        System.Console.WriteLine()
    '    Next iTime

    'End Sub

End Class