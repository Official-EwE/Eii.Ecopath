'==============================================================================
'
' $Log: frmNetworkAnalysis.vb,v $
' Revision 1.17  2009/05/01 17:44:47  jeroens
' Greatly simplified content management
'
' Revision 1.16  2009/04/28 19:00:31  jeroens
' Revamped to be able to use styleguide hide groups, rather than an isolated hidegroups interface
'
' Revision 1.15  2009/04/28 16:46:04  jeroens
' Removed obsolete class
'
' Revision 1.14  2009/04/28 16:36:00  jeroens
' Tree node navigation done based on node names, no longer node texts
'
' Revision 1.13  2009/04/22 22:29:23  joeh
' Check tsNetworkAnalysis has items before using tspgProgressBar
'
' Revision 1.12  2009/04/17 18:51:18  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.11  2009/04/17 01:08:00  joeh
' Remove MixedTrophicImpactUC when necessary
'
' Revision 1.10  2009/04/09 20:04:49  joeh
' Add "Bar graph" button to plot bar graph for MTI
'
' Revision 1.9  2008/12/10 20:56:19  joeh
' Finalize the Suitability Plot
'
' Revision 1.8  2008/12/04 01:14:16  joeh
' Add ucPlotOfMixedTrophicImpact
'
' Revision 1.7  2008/12/02 23:31:55  joeh
' Remove Zed graph control from the parameters of CreatePlot( )
'
' Revision 1.6  2008/12/02 03:06:24  joeh
' Incorporate Functional Response into Network Analysis
'
' Revision 1.5  2008/11/29 00:36:25  joeh
' Add a new node "Response Function" to Network Analysis - Take one
'
' Revision 1.4  2008/11/28 01:58:34  joeh
' Implement new MTI plot and save MTI plot as emf file
'
' Revision 1.3  2008/11/25 05:47:34  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.2  2008/11/17 13:08:31  jeroens
' Removed obsolete root node
'
' Revision 1.1  2008/09/26 07:30:57  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands

Public Class frmNetworkAnalysis
    Private WithEvents m_NetworkManager As cNetworkManager

    Private m_AlgorithmRunning As String
    Private m_strSelectedNodeName As String = ""
    Private m_SelectionOfComboBox1 As Integer
    Private m_SelectionOfComboBox2 As Integer
    Private m_FormActivatedCounter As Integer

    Public Sub New(ByRef theNetworkManager As cNetworkManager)
        Me.InitializeComponent()

        m_NetworkManager = theNetworkManager

        'm_NetworkManager.RunMainNetwork()
        'm_NetworkManager.RunRequiredPrimaryProd()

    End Sub

    Private m_contentmanager As cContentManager = Nothing

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_graph.Visible = False
        Me.m_graph.Dock = DockStyle.Fill

        Me.m_plot.Visible = False
        Me.m_plot.Dock = DockStyle.Fill

        Me.m_datagrid.Visible = False
        Me.m_datagrid.Dock = DockStyle.Fill

        Me.m_tsNetworkAnalysis.Visible = False
        Me.m_tsNetworkAnalysis.Dock = DockStyle.Top

        Me.m_tlpInfo.Visible = True
        Me.m_tlpInfo.Dock = DockStyle.Fill

    End Sub

    Private Sub tvNetworkAnalysis_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvNetworkAnalysis.AfterSelect

        If (Me.m_contentmanager IsNot Nothing) Then
            Me.m_contentmanager.Detach()
            Me.m_contentmanager = Nothing
            Me.m_tsNetworkAnalysis.Visible = False ' Hide toolstrip
        End If

        If Not Me.m_NetworkManager.IsMainNetworkRun Then
            Me.m_NetworkManager.RunMainNetwork()
        End If

        Select Case e.Node.Name

            Case "ndRelativeFlows"
                Me.m_contentmanager = New cRelativeFlows()

            Case "ndAbsoluteFlows"
                Me.m_contentmanager = New cAbsoluteFlows()

            Case "ndTransferEfficiency"
                Me.m_contentmanager = New cTransferEfficiency()

            Case "ndFlowPyramid"
                Me.m_contentmanager = New cFlowPyramid()

            Case "ndBiomassByTrophicLevel"
                Me.m_contentmanager = New cBiomassByTrophicLevel()

            Case "ndBiomassPyramid"
                Me.m_contentmanager = New cBiomassPyramid()

            Case "ndCatchByTrophicLevel"
                Me.m_contentmanager = New cCatchByTrophicLevel()

            Case "ndCatchPyramid"
                Me.m_contentmanager = New cCatchPyramid()

            Case "ndFromPrimaryProducers"
                Me.m_contentmanager = New cFromPrimaryProd()

            Case "ndFromDetritus"
                Me.m_contentmanager = New cFromDetritus()

            Case "ndFromAllCombined"
                Me.m_contentmanager = New cFromAllCombined()

            Case "ndForHarvestOfAllGroups"
                Me.m_NetworkManager.RunRequiredPrimaryProd()
                Me.m_contentmanager = New cForHarvestOfAllGp()

            Case "ndForConsumptionOfAllGroups"
                Me.m_NetworkManager.RunRequiredPrimaryProd()
                Me.m_contentmanager = New cForConsumpOfAllGp()

            Case "ndImpactData"
                Me.m_contentmanager = New cImpactData()

            Case "ndGraphOfMixedTrophicImpact"
                Me.m_contentmanager = New cPlotOfMixedTrophicImpact()

            Case "ndGraphOfMixedTrophicImpactEwE5"
                Me.m_contentmanager = New cGraphOfMixedTrophicImpact()

                ' JS 27apr09: discontinued, StyleGuide group/fleet visible flags should be used for this (if ever)
                'Case My.Resources.TREE_NODE_SHOW_HIDE_GRP
                '    m_HideGroupsClass = cHideGroups.GetInstance(scNetworkAnalysis.Panel2)
                '    m_HideGroupsClass.SetUpPanel()
                '    m_HideGroupsForm = frmHideGroups.GetInstance(m_NetworkManager)
                '    m_HideGroupsForm.ShowDialog()

            Case "ndTotal"
                Me.m_contentmanager = New cTotal()

            Case "ndByGroup"
                Me.m_contentmanager = New cByGroup()

            Case "ndFlowFromDetritus"
                Me.m_contentmanager = New cFlowFromDetritus()

            Case "ndPathway_cons_tl1"
                Me.m_contentmanager = New TL1ToConsumer.cPathways()

            Case "ndSummaryOfPathways_cons_tl1"
                Me.m_contentmanager = New TL1ToConsumer.cSummaryPathways()

            Case "ndPathway_cons_prey_tl1"
                Me.m_contentmanager = New TL1ToPreyToConsumer.cPathways()

            Case "ndSummaryOfPathways_cons_prey_tl1"
                Me.m_contentmanager = New TL1ToConsumer.cSummaryPathways()

            Case "ndPathway_pred_prey"
                Me.m_contentmanager = New PreyToPredator.cPathways()

            Case "ndSummaryOfPathways_pred_prey"
                Me.m_contentmanager = New PreyToPredator.cSummaryPathways()

            Case "ndPathway_living"
                Me.m_contentmanager = New CyclesLiving.cPathways()

            Case "ndSummaryOfPathways_living"
                Me.m_contentmanager = New CyclesLiving.cSummaryPathways()

            Case "ndPathway_all"
                If (MsgBox(My.Resources.PROMPT_COMPUTE_ALL_CYCLES, MsgBoxStyle.YesNo, My.Resources.CAPTION) = MsgBoxResult.Yes) Then
                    Me.m_NetworkManager.FindPathwaysCyclesAll()
                    Me.m_contentmanager = New CyclesAll.cPathways()
                End If

            Case "ndSummaryOfPathways_all"
                If (MsgBox(My.Resources.PROMPT_COMPUTE_ALL_CYCLES, MsgBoxStyle.YesNo, My.Resources.CAPTION) = MsgBoxResult.Yes) Then
                    Me.m_NetworkManager.FindPathwaysCyclesAll()
                    Me.m_contentmanager = New CyclesAll.cSummaryPathways()
                End If

            Case "ndCyclingAndPathLength"
                Me.m_contentmanager = New cCyclingAndPathLen()

            Case "ndWithoutPrimaryProductionRequiredEstimate"

                Me.m_NetworkManager.EcosimPPROn = False
                If Me.m_NetworkManager.RunEcosimNetwork() Then
                    Me.m_contentmanager = New cIndicesWithoutPPREst()
                End If

            Case "ndWithPrimaryProductionRequiredEstimate"

                ' Think positive
                Dim bRun As Boolean = True

                ' PPR not on yet?
                If (Me.m_NetworkManager.EcosimPPROn = False) Then
                    ' #Yes: prompt user if need to run
                    bRun = (MsgBox(My.Resources.PROMPT_ESTIMATE_PPR, MsgBoxStyle.YesNo, My.Resources.CAPTION) = MsgBoxResult.Yes)
                End If

                ' Need to run?
                If bRun Then
                    ' #Yes: run std PP
                    Me.m_NetworkManager.RunRequiredPrimaryProd()
                    ' Switch on PPR in Ecosim
                    m_NetworkManager.EcosimPPROn = True
                    ' Ecosim NA run succesful?
                    If m_NetworkManager.RunEcosimNetwork() = True Then
                        ' #Yes: update control
                        Me.m_contentmanager = New cIndicesWithPPREst()
                    End If
                End If

                'Case My.Resources.TREE_NODE_FUNCT_RESP
                '    If Not m_NetworkManager.IsMainNetworkRun Then
                '        m_NetworkAnalysis = cNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                '        m_NetworkAnalysis.RunNetworkAnalysis()
                '    End If
                '    If Not m_NetworkManager.IsEcosimNetworkWithoutPPREstRun Then
                '        m_EcosimNetworkAnalysis = cEcosimNetworkAnalysis.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                '        m_EcosimNetworkAnalysis.RunEcosimNetworkAnalysis(False) 'False->WithoutPPREst
                '    End If
                '    m_FunctionalResponse = cFunctionalResponse.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
                '    m_FunctionalResponse.SetUpPanel()
                '    If m_NetworkManager.IsEcosimNetworkWithoutPPREstRun = True Then
                '        m_FunctionalResponse.CreatePlot() ', zgcNetworkAnalysis)
                '        scNetworkAnalysis.Panel2.Refresh()
                '    End If
            Case Else
        End Select

        If Me.m_contentmanager IsNot Nothing Then
            ' Attach form
            Me.m_contentmanager.Attach(Me.m_NetworkManager, Me.m_datagrid, Me.m_graph, Me.m_plot)
            ' Get data
            Me.m_contentmanager.DisplayData()

            Me.m_contentmanager.SetupToolstrip(m_tsNetworkAnalysis)
            Me.m_tsNetworkAnalysis.Visible = Me.m_contentmanager.RequiresToolstrip

            ' Hide logo
            Me.m_tlpInfo.Visible = False

            ' Position content
            If Me.m_tsNetworkAnalysis.Visible Then
                Me.m_graph.Top = Me.m_tsNetworkAnalysis.Height
                Me.m_tlpInfo.Top = Me.m_tsNetworkAnalysis.Height
                Me.m_datagrid.Top = Me.m_tsNetworkAnalysis.Height
                Me.m_plot.Top = Me.m_tsNetworkAnalysis.Height
            Else
                Me.m_graph.Top = 0
                Me.m_tlpInfo.Top = 0
                Me.m_datagrid.Top = 0
                Me.m_plot.Top = 0
            End If
        Else
            ' Hide toolbar
            Me.m_tsNetworkAnalysis.Visible = False
            ' Show logo
            Me.m_tlpInfo.Visible = True
        End If

        ' Remember selected node name
        Me.m_strSelectedNodeName = e.Node.Name

    End Sub

    Private Sub tvNetworkAnalysis_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tvNetworkAnalysis.LostFocus

        tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.LightGray
    End Sub

    Private Sub tvNetworkAnalysis_NodeMouseClick(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs) _
        Handles tvNetworkAnalysis.NodeMouseClick

        tvNetworkAnalysis.SelectedNode.BackColor = Drawing.Color.MintCream
    End Sub

    ' JS 01may09: disabled 'Cancel' functionality; this only makes sense when running NA calcs in a separate thread from the UI
    'Private Sub tsbtnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    '    Handles tsbtnCancel.Click

    '    Select Case m_AlgorithmRunning
    '        Case My.Resources.LBL_PPR_CAL_PRGR
    '            m_NetworkManager.CancelRequiredPrimaryProdRun = True
    '    End Select
    'End Sub

    Private Sub tsbtnOutputIndicesCSV_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tsbtnOutputIndicesCSV.Click

        Dim cmdh As CommandHandler = CommandHandler.GetInstance()
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        If (Me.m_contentmanager Is Nothing) Then Return
        If (cmdFS Is Nothing) Then Return

        cmdFS.Invoke("CSV files (*.csv)|*.csv|text files (*.txt)|*.txt|All files (*.*)|*.*", 1)

        If (cmdFS.Result = DialogResult.OK) Then
            Try
                Me.m_contentmanager.SaveToCSV(cmdFS.FileName)
            Catch ex As Exception
                ' Woops
            End Try
        End If

    End Sub

    Private Sub tsbtnOutputGraphEMF_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tsbtnOutputGraphEMF.Click

        Dim cmdh As CommandHandler = CommandHandler.GetInstance()
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        If (Me.m_contentmanager Is Nothing) Then Return
        If (cmdFS Is Nothing) Then Return

        cmdFS.Invoke("Enhanced Metafile image files (*.emf)|*.emf|All files (*.*)|*.*", 1)

        If (cmdFS.Result = DialogResult.OK) Then
            Try
                Me.m_contentmanager.SaveToEMF(cmdFS.FileName)
            Catch ex As Exception
                ' Woops
            End Try
        End If

    End Sub

    Private Sub tsbtnGraphMTI_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tsbtnGraphMTI.Click

        ' ToDo: revamp this

        ''MTI graph with bars
        'm_GraphOfMixedTrophicImpact = cGraphOfMixedTrophicImpact.GetInstance(m_NetworkManager, scNetworkAnalysis.Panel2)
        'm_GraphOfMixedTrophicImpact.SetUpPanel()
        'm_GraphOfMixedTrophicImpact.CreatePlot()
    End Sub

    Private Sub tscmbSelection1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tscmbSelection1.SelectedIndexChanged

        Me.m_SelectionOfComboBox1 = tscmbSelection1.SelectedIndex + 1

        'Select Case m_strSelectedNodeName
        '    Case "ndPathway_cons_tl1" ' My.Resources.TREE_NODE_CONSUM_TL1
        '        m_TL1ToConsumerPathways.SetUpGridRow(intSelection1)
        '    Case "ndPathway_cons_prey_tl1" ' My.Resources.TREE_NODE_CONSUM_PREY_TL1
        '        If m_SelectionOfComboBox1 = 0 Then
        '            m_SelectionOfComboBox1 = intSelection1
        '        Else
        '             = intSelection1
        '            m_TL1ToPreyToConsumerPathways.SetUpGridRow(m_SelectionOfComboBox1, m_SelectionOfComboBox2)
        '        End If
        '    Case "ndPathway_pred_prey" ' My.Resources.TREE_NODE_PRED_PREY
        '        m_PreyToPredatorPathways.SetUpGridRow(intSelection1)
        '    Case Else
        'End Select

        If Me.m_contentmanager IsNot Nothing Then
            Me.m_contentmanager.UpdateData(Me.m_SelectionOfComboBox1, Me.m_SelectionOfComboBox2)
        End If

    End Sub

    Private Sub tscmbSelection2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tscmbSelection2.SelectedIndexChanged

        Me.m_SelectionOfComboBox2 = tscmbSelection2.SelectedIndex + 1

        If Me.m_contentmanager IsNot Nothing Then
            Me.m_contentmanager.UpdateData(Me.m_SelectionOfComboBox1, Me.m_SelectionOfComboBox2)
        End If

    End Sub

    'Private Sub m_NetworkManager_FindCyclesProgress(ByVal iCycle As Integer) Handles m_NetworkManager.FindCyclesProgress
    '    If Me.m_tsNetworkAnalysis.Items.Count > 0 Then
    '        tspgbProgressBar.Maximum = 50 '500
    '        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
    '            tspgbProgressBar.Value += 1
    '        Else
    '            tspgbProgressBar.Value = 0
    '        End If
    '    End If
    'End Sub

    'Private Sub m_NetworkManager_RunMainNetworkProgress(ByVal iProgress As Integer) Handles m_NetworkManager.RunMainNetworkProgress
    '    If Me.m_tsNetworkAnalysis.Items.Count > 0 Then
    '        tspgbProgressBar.Maximum = 10 '20000
    '        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
    '            tspgbProgressBar.Value += 1
    '        Else
    '            tspgbProgressBar.Value = 0
    '        End If
    '    End If
    'End Sub

    'Private Sub m_NetworkManager_CalculateRequiredPPProgress(ByVal nPaths As Integer) Handles m_NetworkManager.CalculateRequiredPPProgress
    '    If Me.m_tsNetworkAnalysis.Items.Count > 0 Then
    '        tspgbProgressBar.Maximum = 5000 '50000
    '        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
    '            tspgbProgressBar.Value += 1
    '        Else
    '            tspgbProgressBar.Value = 0
    '        End If

    '        m_AlgorithmRunning = My.Resources.LBL_PPR_CAL_PRGR
    '    End If
    '    'tsbtnCancel.PerformClick()
    'End Sub

    'Private Sub m_NetworkManager_EcosimNetworkProgress(ByVal iTime As Integer) Handles m_NetworkManager.EcosimNetworkProgress
    '    If Me.m_tsNetworkAnalysis.Items.Count > 0 Then
    '        tspgbProgressBar.Maximum = m_NetworkManager.nEcosimTimesteps  '100
    '        If tspgbProgressBar.Value < tspgbProgressBar.Maximum Then
    '            tspgbProgressBar.Value += 1
    '        Else
    '            tspgbProgressBar.Value = 0
    '        End If
    '    End If
    'End Sub

    Private Sub dgvNetworkAnalysis_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles m_datagrid.CellClick
        If e.RowIndex > 0 And e.ColumnIndex > 0 Then
            'highlight the cell
            m_datagrid.SelectionMode = DataGridViewSelectionMode.CellSelect
            m_datagrid.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
        ElseIf e.RowIndex > 0 And e.ColumnIndex = 0 Then
            'highlight the row
            m_datagrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            m_datagrid.Rows(e.RowIndex).Selected = True
        ElseIf e.RowIndex = 0 And e.ColumnIndex > 0 Then
            'highlight the column
            m_datagrid.SelectionMode = DataGridViewSelectionMode.FullColumnSelect
            m_datagrid.Columns(e.ColumnIndex).Selected = True
        ElseIf e.RowIndex = 0 And e.ColumnIndex = 0 Then
            'highlight the whole grid
            m_datagrid.SelectionMode = DataGridViewSelectionMode.CellSelect
            m_datagrid.SelectAll()
        End If
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