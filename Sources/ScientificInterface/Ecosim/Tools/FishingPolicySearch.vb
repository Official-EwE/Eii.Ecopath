'==============================================================================
'
' $Log: FishingPolicySearch.vb,v $
' Revision 1.2  2008/11/12 21:36:19  jeroens
' Resources!
'
' Revision 1.1  2008/09/26 07:31:51  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.36  2008/09/25 02:31:47  jeroens
' Moved max fishing mortaility from search datastructures to Ecosim
'
' Revision 1.35  2008/09/23 16:16:11  jeroens
' Added different usage modes for GroupOptGrid
'
' Revision 1.34  2008/08/15 21:04:29  jeroens
' Fixing search status resources
'
' Revision 1.33  2008/08/10 01:43:08  jeroens
' Renamed PropertyFormatProvider
'
' Revision 1.32  2008/08/05 17:44:14  jeroens
' Uses AppLauncher status bar for feedback
'
' Revision 1.31  2008/08/04 17:55:54  jeroens
' Fixed issue 454
'
' Revision 1.30  2008/07/29 21:40:20  joeh
' Add a marquee style progress bar to indicate search progress
'
' Revision 1.29  2008/07/04 19:59:43  jeroens
' Fixed issue 332
'
' Revision 1.28  2008/06/04 15:42:31  jeroens
' Wow, intense!
'
' Revision 1.27  2008/05/29 22:23:01  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.26  2008/05/16 17:06:09  joeb
' Added SearchObjective to message source
'
' Revision 1.25  2008/05/12 19:03:43  joeb
' Changes to search objects to support ISearchObjective interface
'
' Revision 1.24  2008/04/15 15:24:29  joeb
' Moved handling of core messages to PolicyColorBlocks
'
' Revision 1.23  2008/04/11 15:10:15  joeb
' Added Connect method to FishingPolicyManager
'
' Revision 1.22  2008/02/27 19:31:01  joeb
' Set Base Year
'
' Revision 1.21  2008/02/27 15:30:33  joeb
' Changed the default number of blocks to the number of fleets to make it easier to see the difference between parameter block is the selected blocks grid see bug 395
'
' Revision 1.20  2008/02/06 16:42:27  jeroens
' Fixed issue 405
'
' Revision 1.19  2007/11/21 14:39:32  jeroens
' * Fixed enums
'
' Revision 1.18  2007/11/21 01:13:41  jeroens
' * Cleaned up
'
'==============================================================================

#Region "Imports directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    Public Class FishingPolicySearch

        Private m_PolicyClrBlocks As PolicyColorBlocks
        Private m_Core As cCore
        Private m_FPManager As cFishingPolicyManager
        Private m_FPParams As cFishingPolicyParameters

        Private m_VCGrid As gridSearchObjectivesWeight
        Private m_FleetOPGrid As gridSearchObjectivesFleet
        Private m_GroupOPGrid As gridSearchObjectivesGroup
        Private m_IterResultSOGrid As IterResultSOGrid
        Private m_IterResultMultiRunSOGrid As IterResultSOGrid
        Private m_IterResultFVGrid As IterResultFVGrid

        Private m_fpDiscRate As cPropertyFormatProvider
        Private m_fpGenDiscRate As cPropertyFormatProvider

        Private m_propBaseYear As cProperty = Nothing
        'Private m_fpBaseYear As PropertyFormatProvider

        Private m_lstOptVisControls As New List(Of cControlVisContainer)

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            m_Core = cCore.GetInstance()

            'Initialize Fishing Policy Manager
            m_FPManager = m_Core.FishingPolicyManager
            m_FPParams = m_FPManager.ModelParameters

            m_FPManager.Connect(AddressOf Me.RunStartedHandler, AddressOf Me.RunCompletedHandler, _
                                AddressOf Me.SearchProgressHandler, AddressOf Me.SearchCompletedHandler)

            'AddressOf Me.SearchCompletedHandler
            'A Fishing Policy search has completed all it's runs.

            'AddressOf Me.SearchProgressHandler
            'Progress of the current Fishing Policy run
            'The cFishingPolicyManager.SearchResults object will contain results of the current iteration

            'AddressOf Me.RunStartedHandler
            'A Fishing Policy Search run has started.
            'When this is called there cFishingPolicyManager.SearchResults will not contain any results
            'The results arrays will be dimensioned by the number of blocks and/or fleets

            'AddressOf Me.RunCompletedHandler
            'A run of the Fishing Policy search has completed.
            'The cFishingPolicyManager.SearchResults object will contain the results of the search run



            m_PolicyClrBlocks = New PolicyColorBlocks
            m_VCGrid = New gridSearchObjectivesWeight(m_Core.FishingPolicyManager)
            m_FleetOPGrid = New gridSearchObjectivesFleet(m_Core.FishingPolicyManager)
            m_GroupOPGrid = New gridSearchObjectivesGroup(m_Core.FishingPolicyManager)
            m_IterResultSOGrid = New IterResultSOGrid
            m_IterResultMultiRunSOGrid = New IterResultSOGrid
            m_IterResultFVGrid = New IterResultFVGrid

            Me.m_fpDiscRate = New cPropertyFormatProvider(Me.txDiscountRate, m_Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchDiscountRate)
            Me.m_fpGenDiscRate = New cPropertyFormatProvider(Me.txGenDiscRate, m_Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchGenDiscRate)

            'Me.m_fpBaseYear = New PropertyFormatProvider(Me.nudBaseYear, m_Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear)
            Me.m_propBaseYear = cPropertyManager.GetInstance().GetProperty(m_Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear)
            AddHandler Me.m_propBaseYear.PropertyChanged, AddressOf OnBaseYearChanged

            m_lstOptVisControls.Add(New cControlVisContainer(Me.cbMaxPortUl, eOptimizeApproachTypes.SystemObjective))
            m_lstOptVisControls.Add(New cControlVisContainer(Me.cbPrevCE, eOptimizeApproachTypes.SystemObjective))
            m_lstOptVisControls.Add(New cControlVisContainer(Me.cmbSearchUsing, eOptimizeApproachTypes.SystemObjective))
            m_lstOptVisControls.Add(New cControlVisContainer(Me.lblSearchUsing, eOptimizeApproachTypes.SystemObjective))
            'blSearchUsing

            m_lstOptVisControls.Add(New cControlVisContainer(Me.cbIncludeCCosts, eOptimizeApproachTypes.FleetValues))

            Me.MessageSources = New eMessageSource() {eMessageSource.FishingPolicySearch, eMessageSource.SearchObjective, eMessageSource.TimeSeries}

            Me.OnBaseYearChanged(Me.m_propBaseYear, cProperty.eChangeFlags.Value)

        End Sub

        Private Sub FishingPolicySearch_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            RemoveHandler Me.m_propBaseYear.PropertyChanged, AddressOf OnBaseYearChanged
            Me.m_propBaseYear = Nothing

            Me.MessageSources = Nothing
        End Sub

        Private Sub setVisibleControls()

            Dim optAproach As eOptimizeApproachTypes = m_FPParams.OptimizeApproach
            For Each ct As cControlVisContainer In m_lstOptVisControls
                ct.Visible(optAproach)
            Next

        End Sub

        Private Sub FishingPolicySearch_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            plBlocks.Controls.Clear()
            plBlocks.Controls.Add(m_PolicyClrBlocks)
            m_PolicyClrBlocks.Dock = DockStyle.Fill

            SplitContainer2.Panel1.Controls.Clear()
            SplitContainer2.Panel1.Controls.Add(m_VCGrid)
            m_VCGrid.Dock = DockStyle.Fill

            SplitContainer3.Panel1.Controls.Clear()
            SplitContainer3.Panel1.Controls.Add(m_FleetOPGrid)
            m_FleetOPGrid.Dock = DockStyle.Fill

            SplitContainer3.Panel2.Controls.Clear()
            SplitContainer3.Panel2.Controls.Add(m_GroupOPGrid)
            m_GroupOPGrid.Dock = DockStyle.Fill

            m_PolicyClrBlocks.ParmBlockCodes.nBlockCodes = m_Core.nFleets
            m_PolicyClrBlocks.ParmBlockCodes.SelectedBlockNum = 1

            InitRunParams()

        End Sub

        Private Sub InitRunParams()

            nupNumOfRuns.Value = CDec(m_FPParams.nRuns)
            nupMaxNumEval.Value = CDec(m_FPParams.MaxNumEval)
            Select Case m_FPParams.InitOption
                Case eInitOption.EcopathBaseF
                    cmbInitUsing.SelectedIndex = 0
                Case eInitOption.CurrentF
                    cmbInitUsing.SelectedIndex = 1
                Case eInitOption.RandomF
                    cmbInitUsing.SelectedIndex = 2
            End Select

            Select Case m_FPParams.SearchOption
                Case eSearchOptionTypes.Fletch
                    cmbSearchUsing.SelectedIndex = 0
                Case eSearchOptionTypes.DFPmin
                    cmbSearchUsing.SelectedIndex = 1
            End Select

            Select Case m_FPParams.OptimizeApproach
                Case eOptimizeApproachTypes.SystemObjective
                    cmbOptmApproach.SelectedIndex = 0
                    InitMaxSOParams()
                Case eOptimizeApproachTypes.FleetValues
                    cmbOptmApproach.SelectedIndex = 1
                    InitMaxFVParams()
            End Select

            setVisibleControls()

            Me.btnSearch.Enabled = True
            Me.btnStop.Enabled = False

        End Sub

        Private Sub InitMaxSOParams()

            '     cbBatchRun.Checked = m_FPParams.BatchRun
            cbMaxPortUl.Checked = m_FPParams.MaxPortUtil
            cbPrevCE.Checked = Me.m_FPManager.ObjectiveParameters.PrevCostEarning
            '    cbUseEcospace.Checked = m_FPParams.UseEcospace

        End Sub

        Private Sub InitMaxFVParams()
            cbIncludeCCosts.Checked = m_FPParams.IncludeComp
            nupMaxEffChg.Value = CDec(m_FPParams.MaxEffChange)
            nudBaseYear.Value = CDec(Me.m_FPManager.ObjectiveParameters.BaseYear)
        End Sub

        Private Sub nupNumOfRuns_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupNumOfRuns.ValueChanged

            If Not m_FPParams Is Nothing Then
                m_FPParams.nRuns = CInt(nupNumOfRuns.Value)
                If m_FPParams.nRuns > 1 And m_FPParams.InitOption <> eInitOption.RandomF Then
                    m_FPParams.InitOption = eInitOption.RandomF
                    InitRunParams()
                End If
            End If

        End Sub

        Private Sub nupMaxNumEval_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupMaxNumEval.ValueChanged

            If Not m_FPParams Is Nothing Then
                m_FPParams.MaxNumEval = CSng(nupMaxNumEval.Value)
            End If

        End Sub

        Private Sub cbInitUsing_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbInitUsing.SelectedIndexChanged

            If Not m_FPParams Is Nothing Then

                Select Case cmbInitUsing.SelectedIndex
                    Case 0
                        m_FPParams.InitOption = eInitOption.EcopathBaseF
                    Case 1
                        m_FPParams.InitOption = eInitOption.CurrentF
                    Case 2
                        m_FPParams.InitOption = eInitOption.RandomF
                End Select

            End If

        End Sub

        Private Sub cbSearchUsing_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSearchUsing.SelectedIndexChanged

            If Not m_FPParams Is Nothing Then

                Select Case cmbSearchUsing.SelectedIndex
                    Case 0
                        m_FPParams.SearchOption = eSearchOptionTypes.Fletch
                    Case 1
                        m_FPParams.SearchOption = eSearchOptionTypes.DFPmin
                End Select

            End If

        End Sub

        Private Sub cbOptmApproach_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbOptmApproach.SelectedIndexChanged
            If Not m_FPParams Is Nothing Then

                Select Case cmbOptmApproach.SelectedIndex
                    Case 0
                        m_FPParams.OptimizeApproach = eOptimizeApproachTypes.SystemObjective
                        InitMaxSOParams()
                        m_FleetOPGrid.IsMaximizeByFleetValue = False
                    Case 1
                        m_FPParams.OptimizeApproach = eOptimizeApproachTypes.FleetValues
                        InitMaxFVParams()
                        m_FleetOPGrid.IsMaximizeByFleetValue = True
                End Select

            End If

            setVisibleControls()

        End Sub

        Private Sub nupMaxEffChg_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupMaxEffChg.ValueChanged

            If Not m_FPParams Is Nothing Then
                m_FPParams.MaxEffChange = CSng(nupMaxEffChg.Value)
            End If

        End Sub

        Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

            tcMain.SelectedIndex = 1

            scIterResultMultiRun.Panel1.Controls.Clear()

            scIterResultMultiRun.Panel1.Controls.Add(m_IterResultSOGrid)
            m_IterResultSOGrid.InsertColumns(m_FPManager.nSearchBlocks)

            Select Case m_FPParams.OptimizeApproach
                Case eOptimizeApproachTypes.SystemObjective
                    scIterResult.Panel2Collapsed = True
                Case eOptimizeApproachTypes.FleetValues
                    scIterResult.Panel2Collapsed = False
                    scIterResult.Panel2.Controls.Clear()
                    scIterResult.Panel2.Controls.Add(m_IterResultFVGrid)
            End Select

            If CInt(nupNumOfRuns.Value) > 1 Then
                scIterResultMultiRun.Panel2Collapsed = False
                scIterResultMultiRun.Panel2.Controls.Clear()
                scIterResultMultiRun.Panel2.Controls.Add(m_IterResultMultiRunSOGrid)
                m_IterResultMultiRunSOGrid.InsertColumns(m_FPManager.nSearchBlocks)
            Else
                scIterResultMultiRun.Panel2Collapsed = True
            End If

            m_FPManager.Run(Me)
            Me.btnSearch.Enabled = False
            Me.btnStop.Enabled = True

            Me.plRunParams.Enabled = False
            Me.plBlocks.Enabled = False

            AppLauncher.GetInstance().SetStatusText(My.Resources.STATUS_SEARCH_SEARCHING, TriState.UseDefault, -1.0!)

        End Sub

        ''' <summary>
        ''' Delegate for cFishingPolicyManager.SearchCompletedHandler. This sub will be called when cFishingPolicyManager.Run has completed.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SearchCompletedHandler()

            Try

                Me.btnSearch.Enabled = True
                Me.btnStop.Enabled = False

                Me.plRunParams.Enabled = True
                Me.plBlocks.Enabled = True

                AppLauncher.GetInstance().SetStatusText("", TriState.UseDefault)

                Me.m_Core.Messages.SendMessage(New cMessage(My.Resources.SEARCH_STATUS_COMPLETED, _
                        eMessageType.NotSet, eMessageSource.EcoSim, eMessageImportance.Information))

            Catch ex As Exception
                cLog.Write(ex)
                SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        Private Sub RunStartedHandler()

            Try
                Me.m_IterResultSOGrid.ClearData()

                Me.m_Core.Messages.SendMessage(New cMessage(My.Resources.SEARCH_STATUS_STARTED, _
                        eMessageType.NotSet, eMessageSource.EcoSim, eMessageImportance.Information))

            Catch ex As Exception
                cLog.Write(ex)
                SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' A Fishing Policy Search run has completed
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RunCompletedHandler()

            Try
                If CInt(nupNumOfRuns.Value) > 1 Then
                    Dim results As cFPSSearchResults = m_FPManager.SearchResults
                    m_IterResultMultiRunSOGrid.InsertOneIterResult(results, m_FPManager.nSearchBlocks, m_PolicyClrBlocks.ParmBlockCodes)
                End If
            Catch ex As Exception
                cLog.Write(ex)
                SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' Delegate for cFishingPolicyManager.ProgressHandler(). This sub will be called the the FishingPolicyManager to update the search progress.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SearchProgressHandler()

            Try
                'get the results object from the manager
                'cFishingPolicyManager.SearchResults will be populate with the results of the Search at the current interation
                Dim results As cFPSSearchResults = m_FPManager.SearchResults

                If cmbOptmApproach.SelectedIndex = 1 Then
                    m_IterResultFVGrid.InsertOneIterResult(results)
                End If

                m_IterResultSOGrid.InsertOneIterResult(results, m_FPManager.nSearchBlocks, m_PolicyClrBlocks.ParmBlockCodes)
            Catch ex As Exception
                cLog.Write(ex)
                SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        Private Sub cbIncludeCCosts_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbIncludeCCosts.CheckedChanged
            If Not m_FPParams Is Nothing Then
                m_FPParams.IncludeComp = cbIncludeCCosts.Checked
            End If
        End Sub

        Private Sub cbMaxPortUl_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbMaxPortUl.CheckedChanged
            If Not m_FPParams Is Nothing Then
                m_FPParams.MaxPortUtil = cbMaxPortUl.Checked
                m_VCGrid.ShowMaxPortUtil = cbMaxPortUl.Checked
            End If
        End Sub

        Private Sub cbPrevCE_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPrevCE.CheckedChanged
            ' If Not m_FPParams Is Nothing Then
            Me.m_FPManager.ObjectiveParameters.PrevCostEarning = cbPrevCE.Checked
            '  End If
        End Sub

        Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
            m_FPManager.StopRun()
        End Sub

        'send a generic error message
        Private Sub SendErrorMessage(ByVal theMessage As String)
            m_Core.Messages.SendMessage(New cMessage(theMessage, eMessageType.ErrorEncountered, eMessageSource.EcoSim, eMessageImportance.Critical, eDataTypes.FishingPolicyManager))
        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)
            If msg.Source = eMessageSource.TimeSeries Then
                Me.OnBaseYearChanged(Me.m_propBaseYear, cProperty.eChangeFlags.All)
            End If
        End Sub

        Private m_bInUpdate As Boolean = False

        Private Sub OnBaseYearChanged(ByVal prop As cProperty, ByVal cf As cProperty.eChangeFlags)
            Debug.Assert(Object.ReferenceEquals(prop, Me.m_propBaseYear))

            If Me.m_bInUpdate Then Return

            'If (cf And cProperty.eChangeFlags.Value) = cProperty.eChangeFlags.Value Then
            Me.m_bInUpdate = True
            Me.nudBaseYear.Value = CInt(prop.GetValue()) + Me.m_Core.EcosimFirstYear
            Me.m_bInUpdate = False
            'End If

        End Sub

        Private Sub OnBaseYearChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles nudBaseYear.ValueChanged

            Dim iStart As Integer = Me.m_Core.EcosimFirstYear
            Dim iEnd As Integer = iStart + Me.m_Core.nEcosimYears
            Dim iValue As Integer = iStart

            Try
                iValue = CInt(Val(Me.nudBaseYear.Value))
            Catch ex As Exception
                ' Whoops
            End Try

            iValue = Math.Max(Math.Min(iValue, iEnd), iStart)

            Me.m_propBaseYear.SetValue(iValue - iStart)

        End Sub

    End Class

    ''' <summary>
    ''' Set the visibility of a control based on an optimize approach value
    ''' </summary>
    Friend Class cControlVisContainer

        Private m_ct As Windows.Forms.Control
        Private m_visState As eOptimizeApproachTypes

        Public Sub New(ByVal Control As Windows.Forms.Control, ByVal VisibleState As eOptimizeApproachTypes)
            m_ct = Control
            m_visState = VisibleState
        End Sub

        Public Sub Visible(ByVal OptAproach As eOptimizeApproachTypes)
            If OptAproach = m_visState Then
                m_ct.Visible = True
            Else
                m_ct.Visible = False
            End If
        End Sub

    End Class

End Namespace
