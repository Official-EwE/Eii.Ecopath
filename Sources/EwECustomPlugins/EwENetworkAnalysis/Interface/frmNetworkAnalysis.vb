'==============================================================================
'
' $Log: frmNetworkAnalysis.vb,v $
' Revision 1.22  2009/05/11 02:12:40  jeroens
' Simplified default file name use for CSV files
' Uses new cDirectoryOpenCommand
'
' Revision 1.21  2009/05/11 01:50:57  jeroens
' Renamed command classes
'
' Revision 1.20  2009/05/04 02:12:49  jeroens
' NA Sim off unless initiated from NA nav tree
'
' Revision 1.19  2009/05/02 18:59:33  jeroens
' Added UI refresh status feedback
'
' Revision 1.18  2009/05/02 03:06:19  jeroens
' Cleaned up
' Uses content manager provided file names when handling file-based commands
'
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
Imports System.IO
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands

Public Class frmNetworkAnalysis

    ''' <summary>NA manager in charge of computations.</summary>
    Private m_networkmanager As cNetworkManager = Nothing
    ''' <summary>Control manager in charge of UI elements.</summary>
    Private m_contentmanager As cContentManager = Nothing
    ''' <summary>Current selected group in toolbar combo 1.</summary>
    Private m_iSelectedGroup1 As Integer = 0
    ''' <summary>Current selected group in toolbar combo 2.</summary>
    Private m_iSelectedGroup2 As Integer = 0

    Public Sub New(ByRef networkmanager As cNetworkManager)
        Me.InitializeComponent()
        Me.m_networkmanager = networkmanager
    End Sub

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

    Private Sub tvNetworkAnalysis_AfterSelect(ByVal sender As System.Object, ByVal e As TreeViewEventArgs) _
        Handles tvNetworkAnalysis.AfterSelect

        Dim asn As cApplicationStatusNotifier = cApplicationStatusNotifier.GetInstance()

        Me.SuspendLayout()

        If (Me.m_contentmanager IsNot Nothing) Then
            Me.m_contentmanager.Detach()
            Me.m_contentmanager = Nothing
            Me.m_tsNetworkAnalysis.Visible = False ' Hide toolstrip
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
                Me.m_networkmanager.RunRequiredPrimaryProd()
                Me.m_contentmanager = New cForHarvestOfAllGp()

            Case "ndForConsumptionOfAllGroups"
                Me.m_networkmanager.RunRequiredPrimaryProd()
                Me.m_contentmanager = New cForConsumpOfAllGp()

            Case "ndImpactData"
                Me.m_contentmanager = New cImpactData()

            Case "ndGraphOfMixedTrophicImpact"
                Me.m_contentmanager = New cPlotOfMixedTrophicImpact()

            Case "ndGraphOfMixedTrophicImpactEwE5"
                Me.m_contentmanager = New cGraphOfMixedTrophicImpact()

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
                    Me.m_networkmanager.FindPathwaysCyclesAll()
                    Me.m_contentmanager = New CyclesAll.cPathways()
                End If

            Case "ndSummaryOfPathways_all"
                If (MsgBox(My.Resources.PROMPT_COMPUTE_ALL_CYCLES, MsgBoxStyle.YesNo, My.Resources.CAPTION) = MsgBoxResult.Yes) Then
                    Me.m_networkmanager.FindPathwaysCyclesAll()
                    Me.m_contentmanager = New CyclesAll.cSummaryPathways()
                End If

            Case "ndCyclingAndPathLength"
                Me.m_contentmanager = New cCyclingAndPathLen()

            Case "ndWithoutPrimaryProductionRequiredEstimate"

                Me.m_networkmanager.UseEcosimNetwork = True
                Me.m_networkmanager.EcosimPPROn = False
                If Me.m_networkmanager.RunEcosimNetwork() Then
                    Me.m_contentmanager = New cIndicesWithoutPPREst()
                End If
                Me.m_networkmanager.UseEcosimNetwork = False

            Case "ndWithPrimaryProductionRequiredEstimate"

                ' Think positive
                Dim bRun As Boolean = True

                ' PPR not on yet?
                If (Me.m_networkmanager.EcosimPPROn = False) Then
                    ' #Yes: prompt user if need to run
                    bRun = (MsgBox(My.Resources.PROMPT_ESTIMATE_PPR, MsgBoxStyle.YesNo, My.Resources.CAPTION) = MsgBoxResult.Yes)
                End If

                ' Need to run?
                If bRun Then
                    ' #Yes: run std PP
                    Me.m_networkmanager.RunRequiredPrimaryProd()
                    ' Switch on PPR in Ecosim
                    Me.m_networkmanager.UseEcosimNetwork = True
                    m_networkmanager.EcosimPPROn = True
                    ' Ecosim NA run succesful?
                    If m_networkmanager.RunEcosimNetwork() = True Then
                        ' #Yes: update control
                        Me.m_contentmanager = New cIndicesWithPPREst()
                    End If
                    Me.m_networkmanager.UseEcosimNetwork = False
                End If

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

        asn.SetStatusText("Updating UI...", TriState.True)

        ' Put content manager to work
        If Me.m_contentmanager IsNot Nothing Then

            ' Attach form
            Me.m_contentmanager.Attach(Me.m_networkmanager, Me.m_datagrid, Me.m_graph, Me.m_plot)
            Try
                ' Get data
                Me.m_contentmanager.DisplayData()
            Catch ex As Exception

            End Try

            ' Fix toolstrip
            Me.m_contentmanager.SetupToolstrip(m_tsNetworkAnalysis)
            Me.m_tsNetworkAnalysis.Visible = Me.m_contentmanager.RequiresToolstrip
            ' Hide info panel
            Me.m_tlpInfo.Visible = False
        Else
            ' Hide toolbar
            Me.m_tsNetworkAnalysis.Visible = False
            ' Show logo
            Me.m_tlpInfo.Visible = True
        End If

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

        asn.SetStatusText("", TriState.False)
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

        ' ToDo: localize this

        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        Dim cmdDOC As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
        Dim strFileName As String = ""

        If (Me.m_contentmanager Is Nothing) Then Return
        If (cmdDOC Is Nothing) Then Return

        cmdDOC.Invoke("", "Select folder to save Network Analysis CSV results")

        If (cmdDOC.Result = DialogResult.OK) Then
            Try
                strFileName = Path.GetFileNameWithoutExtension(Me.m_contentmanager.Filename) & ".csv"
                Me.m_contentmanager.SaveToCSV(Path.Combine(cmdDOC.Directory, strFileName))
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

        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        If (Me.m_contentmanager Is Nothing) Then Return
        If (cmdFS Is Nothing) Then Return

        cmdFS.Invoke(Me.m_contentmanager.Filename, _
                     "EMF image files (*.emf)|*.emf|All files (*.*)|*.*", _
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

        Dim asn As cApplicationStatusNotifier = cApplicationStatusNotifier.GetInstance()

        Me.m_iSelectedGroup1 = tscmbSelection1.SelectedIndex + 1

        If Me.m_contentmanager IsNot Nothing Then
            asn.SetStatusText("Updating UI...", TriState.True)
            Try
                Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)
            Catch ex As Exception
                ' Woops
            End Try
            asn.SetStatusText("", TriState.False)
        End If

    End Sub

    Private Sub tscmbSelection2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tscmbSelection2.SelectedIndexChanged

        Dim asn As cApplicationStatusNotifier = cApplicationStatusNotifier.GetInstance()

        Me.m_iSelectedGroup2 = tscmbSelection2.SelectedIndex + 1

        If Me.m_contentmanager IsNot Nothing Then
            asn.SetStatusText("Updating UI...", TriState.True)
            Try
                Me.m_contentmanager.UpdateData(Me.m_iSelectedGroup1, Me.m_iSelectedGroup2)
            Catch ex As Exception
                ' Woops
            End Try
            asn.SetStatusText("", TriState.False)
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

End Class