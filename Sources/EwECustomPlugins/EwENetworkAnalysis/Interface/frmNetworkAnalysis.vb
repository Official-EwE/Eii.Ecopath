#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Windows.Forms
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmNetworkAnalysis

    Public Enum eNetworkAnalysisPageTypes As Integer
        NotSet = 0
        Credits
        RelativeFlows
        AbsoluteFlows
        TransferEfficiency
        FlowPyramid
        BiomassByTrophicLevel
        BiomassPyramid
        CatchByTrophicLevel
        CatchPyramid
        FromPrimaryProducers
        FromDetritus
        FromAllCombined
        ForHarvestOfAllGroups
        ForConsumptionOfAllGroups
        ImpactData
        GraphOfMixedTrophicImpact
        GraphOfMixedTrophicImpactEwE5
        KeystonenessTable
        KeystonenessGraph
        Total
        ByGroup
        FlowFromDetritus
        Pathway_cons_tl1
        SummaryOfPathways_cons_tl1
        Pathway_cons_prey_tl1
        SummaryOfPathways_cons_prey_tl1
        Pathway_pred_prey
        SummaryOfPathways_pred_prey
        Pathway_living
        SummaryOfPathways_living
        Pathway_all
        SummaryOfPathways_all
        CyclingAndPathLength
        LindemanSpine
        WithoutPrimaryProductionRequiredEstimate
        WithPrimaryProductionRequiredEstimate
    End Enum

    Private m_pageCurrent As eNetworkAnalysisPageTypes = eNetworkAnalysisPageTypes.NotSet

    Private m_networkmanager As cNetworkManager = Nothing
    ''' <summary>Control manager in charge of UI elements.</summary>
    Private m_contentmanager As cContentManager = Nothing
    ''' <summary>Current selected group in toolbar combo 1.</summary>
    Private m_iSelectedGroup1 As Integer = 0
    ''' <summary>Current selected group in toolbar combo 2.</summary>
    Private m_iSelectedGroup2 As Integer = 0
    ''' <summary>Update feedback loop prevention.</summary>
    Private m_bInUpdate As Boolean = False
    ''' <summary></summary>
    Private m_cmdDisplayGroups As cDisplayGroupsCommand = Nothing
    ''' <summary>UI context for UI to use.</summary>
    Private m_uic As cUIContext = Nothing

    Public Sub New(ByVal networkmanager As cNetworkManager, ByVal uic As cUIContext)

        Me.m_networkmanager = networkmanager
        Me.m_uic = uic

        Debug.Assert(uic IsNot Nothing, "Essential data missing")

        Me.InitializeComponent()
        Me.Text = My.Resources.CAPTION
        Me.TabText = My.Resources.CAPTION

    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_graph.Visible = False
        Me.m_graph.Dock = DockStyle.Fill

        Me.m_plot.Visible = False
        Me.m_plot.Dock = DockStyle.Fill

        Me.m_datagrid.Visible = False
        Me.m_datagrid.Dock = DockStyle.Fill

        Me.m_toolstrip.Visible = False
        Me.m_toolstrip.Dock = DockStyle.Top

        Me.m_tlpInfo.Visible = True
        Me.m_tlpInfo.Dock = DockStyle.Fill

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Me.m_cmdDisplayGroups = DirectCast(cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME), cDisplayGroupsCommand)
        If (Me.m_cmdDisplayGroups IsNot Nothing) Then
            Me.m_cmdDisplayGroups.AddControl(Me.tsmiDisplayGroups)
            AddHandler Me.m_cmdDisplayGroups.OnPreInvoke, AddressOf OnPreInvokeDisplayGroups
            AddHandler Me.m_cmdDisplayGroups.OnPostInvoke, AddressOf OnPostInvokeDisplayGroups
        End If

        AddHandler Me.m_networkmanager.OnRunStateChanged, AddressOf OnRunStateChanged

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.m_cmdDisplayGroups IsNot Nothing) Then
            Me.m_cmdDisplayGroups.RemoveControl(Me.tsmiDisplayGroups)
            RemoveHandler Me.m_cmdDisplayGroups.OnPreInvoke, AddressOf OnPreInvokeDisplayGroups
            RemoveHandler Me.m_cmdDisplayGroups.OnPostInvoke, AddressOf OnPostInvokeDisplayGroups
            Me.m_cmdDisplayGroups = Nothing
        End If

        RemoveHandler Me.m_networkmanager.OnRunStateChanged, AddressOf OnRunStateChanged

        MyBase.OnFormClosed(e)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Re-run Network Analysis bit.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_tsbnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles tsmiRun.Click
        ' Shazaam
        Me.ShowPage(Me.m_pageCurrent)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic save-to-csv command handler. Invokes the EwE6 File Save interface
    ''' and informs the current control manager to save to the selected file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub tsbtnOutputIndicesCSV_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tsbtnOutputIndicesCSV.Click

        Try

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmdDOC As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
            Dim strFileName As String = ""
            Dim bAnnual As Boolean = False

            If (Me.m_contentmanager Is Nothing) Then Return
            If (cmdDOC Is Nothing) Then Return

            If (Me.m_contentmanager.IsDataOverTime) Then
                Select Case MsgBox(My.Resources.PROMPT_SAVE_ANNUAL_AVERAGES, MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question)

                    Case MsgBoxResult.Yes
                        bAnnual = True

                    Case MsgBoxResult.No
                        bAnnual = False

                    Case Else
                        Return

                End Select
            End If

            cmdDOC.Invoke("", My.Resources.PROMPT_SAVE_DESTINATION)

            If (cmdDOC.Result = DialogResult.OK) Then
                Try
                    Dim writer As New cResultWriter(Me.m_networkmanager)
                    writer.WriteCurrentResults(cmdDOC.Directory, bAnnual)
                Catch ex As Exception
                    ' Woops
                End Try
            End If

        Catch ex As Exception

        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic save-to-emf command handler. Invokes the EwE6 File Save interface
    ''' and informs the current control manager to save to the selected file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub tsbtnOutputGraphEMF_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tsbtnOutputGraphEMF.Click

        Try

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim bAnnual As Boolean = False

            If (Me.m_contentmanager Is Nothing) Then Return
            If (cmdFS Is Nothing) Then Return

            cmdFS.Invoke(Me.m_contentmanager.Filename(bAnnual), _
                         My.Resources.FILEFILTER_EMF, _
                         1)

            If (cmdFS.Result = DialogResult.OK) Then
                Try
                    Me.m_contentmanager.SaveToEMF(cmdFS.FileName)
                Catch ex As Exception
                    ' Woops
                End Try
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub tscmbSelection1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tscmbSelection1.SelectedIndexChanged

        Try

            Me.m_iSelectedGroup1 = tscmbSelection1.SelectedIndex + 1

            If Me.m_bInUpdate Then Return

            If Me.m_contentmanager IsNot Nothing Then
                cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, My.Resources.STATUS_UPDATING_UI)
                Try
                    Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)
                Catch ex As Exception
                    ' Woops
                End Try
                cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub tscmbSelection2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tscmbSelection2.SelectedIndexChanged

        Try

            Me.m_iSelectedGroup2 = tscmbSelection2.SelectedIndex + 1

            If Me.m_bInUpdate Then Return

            If Me.m_contentmanager IsNot Nothing Then
                cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, My.Resources.STATUS_UPDATING_UI)
                Try
                    Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)
                Catch ex As Exception
                    ' Woops
                End Try
                cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub dgvNetworkAnalysis_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles m_datagrid.CellClick

        Try

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

        Catch ex As Exception

        End Try

    End Sub

    Private Sub tsbtnOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles tsbtnOptions.Click
        Me.ShowOptions(tsbtnOptions.Checked)
    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, triggered before 'DisplayGroups' command has been invoked.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnPreInvokeDisplayGroups(ByVal cmd As cCommand)
        Try
            Me.m_cmdDisplayGroups.ShowGroups = True
            Me.m_cmdDisplayGroups.ShowTotals = False
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, triggered after 'DisplayGroups' command has been invoked.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnPostInvokeDisplayGroups(ByVal cmd As cCommand)
        If (Me.m_contentmanager IsNot Nothing) Then
            Try
                Me.m_contentmanager.DisplayData()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub OnRunStateChanged()

        If (Me.tsmiRun Is Nothing) Then Return
        If (Me.m_contentmanager Is Nothing) Then Return
        If (Me.m_networkmanager Is Nothing) Then Return

        Me.tsmiRun.Enabled = (Me.m_networkmanager.IsMainNetworkRun = False) Or _
                             ((Me.m_networkmanager.IsEcosimNetworkRun = False) And (Me.m_contentmanager.UsesEcosim = True))

        ' Fixes bug 937
        Me.ShowPage(Me.m_pageCurrent)

    End Sub

    Public Sub ShowPage(ByVal page As eNetworkAnalysisPageTypes)

        If Me.m_bInUpdate Then Return
        Me.m_bInUpdate = True

        Me.m_pageCurrent = page

        Me.SuspendLayout()

        If (Me.m_contentmanager IsNot Nothing) Then
            Me.ShowOptions(False)
            Try
                Me.m_contentmanager.Detach()
            Catch ex As Exception
                ' Hmm
            End Try
            Me.m_contentmanager = Nothing
        End If

        ' Make sure main network has ran
        ' Fixes bug 937
        If Me.m_networkmanager.RunMainNetwork() Then

            Select Case page

                Case eNetworkAnalysisPageTypes.Credits
                    Me.m_contentmanager = Nothing

                Case eNetworkAnalysisPageTypes.RelativeFlows
                    Me.m_contentmanager = New cRelativeFlows()

                Case eNetworkAnalysisPageTypes.AbsoluteFlows
                    Me.m_contentmanager = New cAbsoluteFlows()

                Case eNetworkAnalysisPageTypes.TransferEfficiency
                    Me.m_contentmanager = New cTransferEfficiency()

                Case eNetworkAnalysisPageTypes.FlowPyramid
                    Me.m_contentmanager = New cFlowPyramid()

                Case eNetworkAnalysisPageTypes.BiomassByTrophicLevel
                    Me.m_contentmanager = New cBiomassByTrophicLevel()

                Case eNetworkAnalysisPageTypes.BiomassPyramid
                    Me.m_contentmanager = New cBiomassPyramid()

                Case eNetworkAnalysisPageTypes.CatchByTrophicLevel
                    Me.m_contentmanager = New cCatchByTrophicLevel()

                Case eNetworkAnalysisPageTypes.CatchPyramid
                    Me.m_contentmanager = New cCatchPyramid()

                Case eNetworkAnalysisPageTypes.FromPrimaryProducers
                    Me.m_contentmanager = New cFromPrimaryProd()

                Case eNetworkAnalysisPageTypes.FromDetritus
                    Me.m_contentmanager = New cFromDetritus()

                Case eNetworkAnalysisPageTypes.FromAllCombined
                    Me.m_contentmanager = New cFromAllCombined()

                Case eNetworkAnalysisPageTypes.ForHarvestOfAllGroups
                    Me.m_contentmanager = New cForHarvestOfAllGp()

                Case eNetworkAnalysisPageTypes.ForConsumptionOfAllGroups
                    Me.m_contentmanager = New cForConsumpOfAllGp()

                Case eNetworkAnalysisPageTypes.ImpactData
                    Me.m_contentmanager = New cImpactData()

                Case eNetworkAnalysisPageTypes.GraphOfMixedTrophicImpact
                    Me.m_contentmanager = New cPlotOfMixedTrophicImpact()

                Case eNetworkAnalysisPageTypes.GraphOfMixedTrophicImpactEwE5
                    Me.m_contentmanager = New cGraphOfMixedTrophicImpact()

                Case eNetworkAnalysisPageTypes.KeystonenessTable
                    Me.m_contentmanager = New cKeystonenessTable()

                Case eNetworkAnalysisPageTypes.KeystonenessGraph
                    Me.m_contentmanager = New cKeystonenessGraph()

                Case eNetworkAnalysisPageTypes.Total
                    Me.m_contentmanager = New cTotal()

                Case eNetworkAnalysisPageTypes.ByGroup
                    Me.m_contentmanager = New cByGroup()

                Case eNetworkAnalysisPageTypes.FlowFromDetritus
                    Me.m_contentmanager = New cFlowFromDetritus()

                Case eNetworkAnalysisPageTypes.Pathway_cons_tl1
                    Me.m_contentmanager = New TL1ToConsumer.cCyclesPathways()

                Case eNetworkAnalysisPageTypes.SummaryOfPathways_cons_tl1
                    Me.m_contentmanager = New TL1ToConsumer.cCyclesPathwaysSummary()

                Case eNetworkAnalysisPageTypes.Pathway_cons_prey_tl1
                    Me.m_contentmanager = New TL1ToPreyToConsumer.cCyclesPathways()

                Case eNetworkAnalysisPageTypes.SummaryOfPathways_cons_prey_tl1
                    Me.m_contentmanager = New TL1ToConsumer.cCyclesPathwaysSummary()

                Case eNetworkAnalysisPageTypes.Pathway_pred_prey
                    Me.m_contentmanager = New PreyToPredator.cCyclesPathways()

                Case eNetworkAnalysisPageTypes.SummaryOfPathways_pred_prey
                    Me.m_contentmanager = New PreyToPredator.cSummaryCyclesPathways()

                Case eNetworkAnalysisPageTypes.Pathway_living
                    Me.m_contentmanager = New CyclesLiving.cCyclesPathways()

                Case eNetworkAnalysisPageTypes.SummaryOfPathways_living
                    Me.m_contentmanager = New CyclesLiving.cSummaryPathways()

                Case eNetworkAnalysisPageTypes.Pathway_all
                    Me.m_contentmanager = New CyclesAll.cCyclesPathways()

                Case eNetworkAnalysisPageTypes.SummaryOfPathways_all
                    Me.m_contentmanager = New CyclesAll.cSummaryCyclesPathways()

                Case eNetworkAnalysisPageTypes.CyclingAndPathLength
                    Me.m_contentmanager = New cCyclingAndPathLen()

                Case eNetworkAnalysisPageTypes.LindemanSpine
                    Me.m_contentmanager = New cLindemanSpine()

                Case eNetworkAnalysisPageTypes.WithoutPrimaryProductionRequiredEstimate
                    Me.m_contentmanager = New cIndicesWithoutPPREst()

                Case eNetworkAnalysisPageTypes.WithPrimaryProductionRequiredEstimate
                    Me.m_contentmanager = New cIndicesWithPPREst()

                Case Else
            End Select
        End If

        cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, My.Resources.STATUS_UPDATING_UI)

        ' Put content manager to work
        If (Me.m_contentmanager IsNot Nothing) Then

            ' Try to attach content manager
            If Me.m_contentmanager.Attach(Me.m_networkmanager, _
                                          Me.m_datagrid, Me.m_graph, Me.m_plot, Me.m_toolstrip, _
                                          Me.m_uic) Then

                Try
                    ' Display data if succesful
                    Me.m_contentmanager.DisplayData()
                Catch ex As Exception

                End Try

                ' Need to populate group combos?
                If Me.m_toolstrip.Visible Then

                    Me.tscmbSelection1.Items.Clear()
                    Me.tscmbSelection2.Items.Clear()
                    For iGroup As Integer = 1 To Me.m_networkmanager.nGroups
                        If (Me.m_contentmanager.GroupFilter1 = eGroupFilterTypes.Living) Or _
                           (iGroup < Me.m_networkmanager.nLivingGroups) Then
                            Me.tscmbSelection1.Items.Add(String.Format(My.Resources.LBL_INDEXED, iGroup, Me.m_networkmanager.GroupName(iGroup)))
                        End If
                        If (Me.m_contentmanager.GroupFilter2 = eGroupFilterTypes.Living) Or _
                           (iGroup < Me.m_networkmanager.nLivingGroups) Then
                            Me.tscmbSelection2.Items.Add(String.Format(My.Resources.LBL_INDEXED, iGroup, Me.m_networkmanager.GroupName(iGroup)))
                        End If
                    Next
                    Me.m_toolstrip.Refresh()

                    Me.tscmbSelection1.SelectedIndex = 0
                    Me.tscmbSelection2.SelectedIndex = 0

                    Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)

                End If

                Me.m_hdrPage.Text = Me.m_contentmanager.PageTitle
            End If

            ' Hide info panel
            Me.m_tlpInfo.Visible = False
        Else
            ' Hide toolbar
            Me.m_toolstrip.Visible = False
            ' Show credits
            Me.m_tlpInfo.Visible = True
            Me.m_hdrPage.Text = My.Resources.PAGE_CREDITS
        End If

        ' Position content
        Me.m_graph.Top = 0
        Me.m_tlpInfo.Top = 0
        Me.m_datagrid.Top = 0
        Me.m_plot.Top = 0

        cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)
        Me.ResumeLayout()

        Me.m_bInUpdate = False

    End Sub

    Private Sub ShowOptions(ByVal bShow As Boolean)

        Dim ctrlOptions As Control = Nothing

        Me.m_scMain.Panel2.Controls.Clear()

        If (bShow = True) And (Me.m_contentmanager IsNot Nothing) Then
            ctrlOptions = Me.m_contentmanager.OptionsControl
        End If

        If (ctrlOptions IsNot Nothing) Then
            Me.m_scMain.Panel2Collapsed = False
            Me.m_scMain.SplitterDistance = Me.m_scMain.Width - Me.m_scMain.SplitterWidth - ctrlOptions.Width
            Me.m_scMain.Panel2.Controls.Add(ctrlOptions)
            ctrlOptions.Dock = DockStyle.Fill
            Me.tsbtnOptions.Checked = True
        Else
            Me.m_scMain.Panel2Collapsed = True
            Me.tsbtnOptions.Checked = False
        End If

    End Sub

End Class