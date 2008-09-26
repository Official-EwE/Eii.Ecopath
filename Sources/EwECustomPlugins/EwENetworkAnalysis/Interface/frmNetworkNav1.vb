Imports EwECore

Public Class frmNetworkNav1
    Private WithEvents m_NetworkManager As cNetworkManager
    Private m_TrophicLevelDecomp As cTrophicLevelDecomp
    Private m_FlowsAndBiomasses As cFlowAndBiomasses
    Private m_TransferEfficiency As cTransferEfficiency
    Private m_BiomassByTrophicLevel As cBiomassByTrophicLevel
    Private m_CatchByTrophicLevel As cCatchByTrophicLevel
    Private m_PrimaryProdRequired As cPrimaryProdRequired
    Private m_MixedTrophicImpact As cMixedTrophicImpact
    Private m_Ascendency As cAscendency
    Private m_FlowFromDetritus As cFlowFromDetritus
    Private WithEvents m_CyclesAndPathways As cCyclesAndPathways

    Private m_ParentOfPathwayNode As String
    Private m_SelectionOfComboBox1 As Integer
    Private m_SelectionOfComboBox2 As Integer

    Public Sub New(ByRef theNetworkManager As cNetworkManager)
        Me.InitializeComponent()

        m_NetworkManager = theNetworkManager
        m_NetworkManager.RunMainNetwork()

    End Sub

    'Private Sub frmNetworkNav1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    'm_TrophicLevelDecomp = New cTrophicLevelDecomp(m_NetworkManager, scNetworkAnalysis.Panel2)
    'm_FlowsAndBiomasses = New cFlowAndBiomasses(m_NetworkManager, scNetworkAnalysis.Panel2)
    'm_TransferEfficiency = New cTransferEfficiency(m_NetworkManager, scNetworkAnalysis.Panel2)
    'm_MixedTrophicImpact = New cMixedTrophicImpact(m_NetworkManager, scNetworkAnalysis.Panel2)
    'm_Ascendency = New cAscendency(m_NetworkManager, scNetworkAnalysis.Panel2)
    'm_FlowFromDetritus = New cFlowFromDetritus(m_NetworkManager, scNetworkAnalysis.Panel2)
    'm_CyclesAndPathways = New cCyclesAndPathways(m_NetworkManager, scNetworkAnalysis.Panel2)

    'tscmbSelection1.Items.Clear()
    'tscmbSelection2.Items.Clear()
    'For intIndex As Integer = 0 To m_CyclesAndPathways.NGroups - 1
    '    tscmbSelection1.Items.Add(CStr(intIndex + 1) + ", " + m_CyclesAndPathways.GroupNames(intIndex))
    '    tscmbSelection2.Items.Add(CStr(intIndex + 1) + ", " + m_CyclesAndPathways.GroupNames(intIndex))
    'Next
    'tscmbSelection1.Text = CStr(1) + ", " + m_CyclesAndPathways.GroupNames(0)
    'tscmbSelection2.Text = CStr(1) + ", " + m_CyclesAndPathways.GroupNames(0)

    'End Sub

    Private Sub frmNetworkNav_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        'Select the "Relative flows" node
        Dim ndRelativeFlows As Windows.Forms.TreeNode = FindNode(tvNetworkAnalysis.Nodes, "Relative flows")

        'scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
        'scNetworkAnalysis.Panel2.Controls.Add(dgvNetworkAnalysis)

        m_TrophicLevelDecomp = New cTrophicLevelDecomp(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_FlowsAndBiomasses = New cFlowAndBiomasses(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_TransferEfficiency = New cTransferEfficiency(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_BiomassByTrophicLevel = New cBiomassByTrophicLevel(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_CatchByTrophicLevel = New cCatchByTrophicLevel(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_PrimaryProdRequired = New cPrimaryProdRequired(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_MixedTrophicImpact = New cMixedTrophicImpact(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_Ascendency = New cAscendency(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_FlowFromDetritus = New cFlowFromDetritus(m_NetworkManager, scNetworkAnalysis.Panel2)
        m_CyclesAndPathways = New cCyclesAndPathways(m_NetworkManager, scNetworkAnalysis.Panel2)

        If Not ndRelativeFlows Is Nothing Then
            tvNetworkAnalysis.SelectedNode = ndRelativeFlows
            tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.SkyBlue
        End If

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
        'Dim objTrophicLevelDecomp As cTrophicLevelDecomp

        Select Case e.Node.Text
            Case "Relative flows"
                'objTrophicLevelDecomp = New cTrophicLevelDecomp(m_NetworkManager, plNetworkAnalysis)
                'objTrophicLevelDecomp.SetUpRelativeFlowsPanel()
                m_TrophicLevelDecomp.SetUpRelativeFlowsPanel()
            Case "Absolute flows"
                'objTrophicLevelDecomp = New cTrophicLevelDecomp(m_NetworkManager, plNetworkAnalysis)
                'objTrophicLevelDecomp.SetUpAbsoluteFlowsPanel()
                m_TrophicLevelDecomp.SetUpAbsoluteFlowsPanel()
            Case "Transfer efficiency"
                m_TransferEfficiency.SetUpPanel()
            Case "Biomass by trophic level"
                m_BiomassByTrophicLevel.SetUpPanel()
            Case "Catch by trophic level"
                m_CatchByTrophicLevel.SetUpPanel()
            Case "From primary producers"
                m_FlowsAndBiomasses.SetUpFromPrimaryProducersPanel()
            Case "From detritus"
                m_FlowsAndBiomasses.SetUpFromDetritusPanel()
            Case "From all combined"
                m_FlowsAndBiomasses.SetUpFromAllCombinedPanel()
            Case "For harvest of all groups"
                m_PrimaryProdRequired.SetUpHarvestOfAllGroupsPanel()
            Case "For consumption of all groups"
                m_PrimaryProdRequired.SetUpConsumptionOfAllGroupsPanel()
            Case "Impact data"  'Mixed trophic impact data
                m_MixedTrophicImpact.SetUpPanel()
            Case "Total"
                m_Ascendency.SetUpTotalPanel()
            Case "By group"
                m_Ascendency.SetUpByGroupPanel()
            Case "Flow from detritus"
                m_FlowFromDetritus.SetUpPanel()
            Case "Pathway"
                Select Case e.Node.Parent.Text
                    Case "Consumer <- TL1"
                        m_ParentOfPathwayNode = e.Node.Parent.Text
                        m_CyclesAndPathways.SetUpTL1ToConsumerPanel()
                    Case "Consumer <- prey <- TL1"
                        m_ParentOfPathwayNode = e.Node.Parent.Text
                        m_CyclesAndPathways.SetUpTL1ToPreyToConsumerPanel()
                    Case "Top predator <- prey"
                        m_ParentOfPathwayNode = e.Node.Parent.Text
                        m_CyclesAndPathways.SetUpPreyToTopPredatorPanel()
                    Case "Cycles (living)"
                        m_CyclesAndPathways.SetUpCyclesLivingPanel()
                    Case "Cycles (all)"
                        m_CyclesAndPathways.SetUpCyclesAllPanel()
                    Case Else
                End Select
            Case "Summary of pathways"
                Select Case e.Node.Parent.Text
                    Case "Consumer <- TL1"
                        m_CyclesAndPathways.SetUpTL1ToConsumerSummaryPanel()
                    Case "Consumer <- prey <- TL1"

                    Case "Top predator <- prey"

                    Case "Cycles (living)"

                    Case "Cycles (all)"

                    Case Else
                End Select
            Case "Cycling and path length"
                m_CyclesAndPathways.SetUpCyclingAndPathLengthPanel()
            Case Else
        End Select

    End Sub

    Private Sub tvNetworkAnalysis_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tvNetworkAnalysis.LostFocus
        tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.LightGray

    End Sub

    Private Sub tvNetworkAnalysis_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvNetworkAnalysis.NodeMouseClick
        tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.MintCream
    End Sub

    Private Sub m_CyclesAndPathways_AddToolStrip() Handles m_CyclesAndPathways.AddToolStrip
        scNetworkAnalysis.Panel2.Controls.Add(tsNetworkAnalysis)
    End Sub

    Private Sub tscmbSelection1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscmbSelection1.SelectedIndexChanged
        'MsgBox(tscmbSelection1.SelectedItem, MsgBoxStyle.Information)
        Dim strSelection1 As String
        Dim intSelection1 As Integer

        strSelection1 = CStr(tscmbSelection1.SelectedItem)
        intSelection1 = CInt(strSelection1.Substring(0, InStr(strSelection1, ",") - 1))

        Select Case m_ParentOfPathwayNode
            Case "Consumer <- TL1"
                m_CyclesAndPathways.SetUpTL1ToConsumerRow(intSelection1)
            Case "Consumer <- prey <- TL1"
                If m_SelectionOfComboBox1 = 0 Then
                    m_SelectionOfComboBox1 = intSelection1
                Else
                    m_SelectionOfComboBox1 = intSelection1
                    m_CyclesAndPathways.SetUpTL1ToPreyToConsumerRow(m_SelectionOfComboBox1, m_SelectionOfComboBox2)
                End If
            Case "Top predator <- prey"
                m_CyclesAndPathways.SetUpPreyToTopPredatorRow(intSelection1)
            Case Else
        End Select
    End Sub

    Private Sub tscmbSelection2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscmbSelection2.SelectedIndexChanged
        'MsgBox(tscmbSelection2.SelectedItem, MsgBoxStyle.Information)
        Dim strSelection2 As String

        strSelection2 = CStr(tscmbSelection2.SelectedItem)
        m_SelectionOfComboBox2 = CInt(strSelection2.Substring(0, InStr(strSelection2, ",") - 1))

        m_CyclesAndPathways.SetUpTL1ToPreyToConsumerRow(m_SelectionOfComboBox1, m_SelectionOfComboBox2)

    End Sub

End Class