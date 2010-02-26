Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.IO
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands
Imports ScientificInterfaceShared

Public Class frmNetworkAnalysis

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

    Private m_uic As cUIContext = Nothing

    Public Sub New(ByVal strText As String, ByVal networkmanager As cNetworkManager, ByVal uic As cUIContext)

        Me.m_networkmanager = networkmanager
        Me.m_uic = uic

        Debug.Assert(uic IsNot Nothing, "Essential data missing")

        Me.InitializeComponent()
        Me.Text = strText
        Me.TabText = strText

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

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHander
        Me.m_cmdDisplayGroups = DirectCast(cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME), cDisplayGroupsCommand)
        If (Me.m_cmdDisplayGroups IsNot Nothing) Then
            Me.m_cmdDisplayGroups.AddControl(Me.tsmiDisplayGroups)
            AddHandler Me.m_cmdDisplayGroups.OnPreInvoke, AddressOf OnPreInvokeDisplayGroups
            AddHandler Me.m_cmdDisplayGroups.OnPostInvoke, AddressOf OnPostInvokeDisplayGroups
        End If

    End Sub

    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

        If (Me.m_cmdDisplayGroups IsNot Nothing) Then
            Me.m_cmdDisplayGroups.RemoveControl(Me.tsmiDisplayGroups)
            RemoveHandler Me.m_cmdDisplayGroups.OnPreInvoke, AddressOf OnPreInvokeDisplayGroups
            RemoveHandler Me.m_cmdDisplayGroups.OnPostInvoke, AddressOf OnPostInvokeDisplayGroups
            Me.m_cmdDisplayGroups = Nothing
        End If

        MyBase.OnFormClosing(e)
    End Sub

    Private Sub tvNetworkAnalysis_AfterSelect(ByVal sender As System.Object, ByVal e As TreeViewEventArgs) _
        Handles tvNetworkAnalysis.AfterSelect

        Me.SuspendLayout()

        If (Me.m_contentmanager IsNot Nothing) Then
            Me.m_contentmanager.Detach()
            Me.m_contentmanager = Nothing
        End If

        ' Make sure main network has ran
        Me.m_networkmanager.RunMainNetwork()

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
                Me.m_contentmanager = New cForHarvestOfAllGp()

            Case "ndForConsumptionOfAllGroups"
                Me.m_contentmanager = New cForConsumpOfAllGp()

            Case "ndImpactData"
                Me.m_contentmanager = New cImpactData()

            Case "ndGraphOfMixedTrophicImpact"
                Me.m_contentmanager = New cPlotOfMixedTrophicImpact()

            Case "ndGraphOfMixedTrophicImpactEwE5"
                Me.m_contentmanager = New cGraphOfMixedTrophicImpact()

            Case "ndKeystonenessTable"
                Me.m_contentmanager = New cKeystonenessTable()

            Case "ndKeystonenessGraph"
                Me.m_contentmanager = New cKeystonenessGraph()

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
                Me.m_contentmanager = New CyclesAll.cPathways()

            Case "ndSummaryOfPathways_all"
                Me.m_contentmanager = New CyclesAll.cSummaryPathways()

            Case "ndCyclingAndPathLength"
                Me.m_contentmanager = New cCyclingAndPathLen()

            Case "ndLindemanSpine"
                Me.m_contentmanager = New cLindemanSpine()

            Case "ndWithoutPrimaryProductionRequiredEstimate"
                Me.m_contentmanager = New cIndicesWithoutPPREst()

            Case "ndWithPrimaryProductionRequiredEstimate"
                Me.m_contentmanager = New cIndicesWithPPREst()

                ' JS 27apr09: discontinued, StyleGuide group/fleet visible flags should be used for this (if ever)
                ' JS 01may09: besides, the interface will be triggered from a toolbar btn instead of a tree node.
                '             Changing viz items will need to cause toolbars to refresh, grid population code to 
                '             change, etc... It's not straight-forward at all!

                'Case My.Resources.TREE_NODE_SHOW_HIDE_GRP
                '    m_HideGroupsClass = cHideGroups.GetInstance(scNetworkAnalysis.Panel2)
                '    m_HideGroupsClass.SetUpPanel()
                '    m_HideGroupsForm = frmHideGroups.GetInstance(m_NetworkManager)
                '    m_HideGroupsForm.ShowDialog()

            Case Else
        End Select

        cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_UPDATING_UI, TriState.True)

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

                    Me.m_bInUpdate = True

                    Me.tscmbSelection1.Items.Clear()
                    Me.tscmbSelection2.Items.Clear()
                    For iGroup As Integer = 1 To Me.m_networkmanager.nLivingGroups
                        Me.tscmbSelection1.Items.Add(String.Format(My.Resources.LBL_INDEXED, iGroup, Me.m_networkmanager.GroupName(iGroup)))
                        Me.tscmbSelection2.Items.Add(String.Format(My.Resources.LBL_INDEXED, iGroup, Me.m_networkmanager.GroupName(iGroup)))
                    Next
                    Me.m_toolstrip.Refresh()

                    Me.tscmbSelection1.SelectedIndex = 0
                    Me.tscmbSelection2.SelectedIndex = 0

                    Me.m_bInUpdate = False

                    Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)

                End If

            End If

            ' Hide info panel
            Me.m_tlpInfo.Visible = False
        Else
            ' Hide toolbar
            Me.m_toolstrip.Visible = False
            ' Show logo
            Me.m_tlpInfo.Visible = True
        End If

        ' Position content
        If Me.m_toolstrip.Visible Then
            Me.m_graph.Top = Me.m_toolstrip.Height
            Me.m_tlpInfo.Top = Me.m_toolstrip.Height
            Me.m_datagrid.Top = Me.m_toolstrip.Height
            Me.m_plot.Top = Me.m_toolstrip.Height
        Else
            Me.m_graph.Top = 0
            Me.m_tlpInfo.Top = 0
            Me.m_datagrid.Top = 0
            Me.m_plot.Top = 0
        End If

        cApplicationStatusNotifier.SetStatusText("", TriState.False)
        Me.ResumeLayout()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic save-to-csv command handler. Invokes the EwE6 File Save interface
    ''' and informs the current control manager to save to the selected file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub tsbtnOutputIndicesCSV_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tsbtnOutputIndicesCSV.Click

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHander
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

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic save-to-emf command handler. Invokes the EwE6 File Save interface
    ''' and informs the current control manager to save to the selected file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub tsbtnOutputGraphEMF_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tsbtnOutputGraphEMF.Click

        ' ToDo: localize this

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHander
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

    End Sub

    Private Sub tscmbSelection1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tscmbSelection1.SelectedIndexChanged

        Me.m_iSelectedGroup1 = tscmbSelection1.SelectedIndex + 1

        If Me.m_bInUpdate Then Return

        If Me.m_contentmanager IsNot Nothing Then
            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_UPDATING_UI, TriState.True)
            Try
                Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)
            Catch ex As Exception
                ' Woops
            End Try
            cApplicationStatusNotifier.SetStatusText("", TriState.False)
        End If

    End Sub

    Private Sub tscmbSelection2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tscmbSelection2.SelectedIndexChanged

        Me.m_iSelectedGroup2 = tscmbSelection2.SelectedIndex + 1

        If Me.m_bInUpdate Then Return

        If Me.m_contentmanager IsNot Nothing Then
            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_UPDATING_UI, TriState.True)
            Try
                Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)
            Catch ex As Exception
                ' Woops
            End Try
            cApplicationStatusNotifier.SetStatusText("", TriState.False)
        End If

    End Sub

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, triggered before 'DisplayGroups' command has been invoked.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnPreInvokeDisplayGroups(ByVal cmd As cCommand)
        Me.m_cmdDisplayGroups.ShowGroups = True
        Me.m_cmdDisplayGroups.ShowTotals = False
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, triggered after 'DisplayGroups' command has been invoked.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnPostInvokeDisplayGroups(ByVal cmd As cCommand)
        If Me.m_contentmanager IsNot Nothing Then
            Me.m_contentmanager.DisplayData()
        End If
    End Sub

End Class