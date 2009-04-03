'==============================================================================
'
' $Log: frmFishingPolicySearch.vb,v $
' Revision 1.7  2009/04/03 18:21:54  jeroens
' Deliberately detached zedgraphhelper
'
' Revision 1.6  2009/03/26 22:48:05  jeroens
' updated to objective grid changes
'
' Revision 1.5  2009/02/05 17:48:38  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.4  2009/01/16 18:30:41  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.3  2008/12/15 16:03:01  jeroens
' Shape controls moved to ScIntShared
'
' Revision 1.2  2008/11/26 18:19:44  sherman
' Added Results Plots to FPS
'
' Revision 1.1  2008/11/19 14:40:35  jeroens
' Moved and renamed
'
' Revision 1.3  2008/11/17 18:27:03  jeroens
' Hooked up discount rates, base year
'
' Revision 1.2  2008/11/12 21:36:19  jeroens
' Resources!
'
' Revision 1.1  2008/09/26 07:31:51  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    Public Class frmFishingPolicySearch

        Private m_blocks As ucPolicyColorBlocks = Nothing
        Private m_core As cCore = Nothing
        Private m_manager As cFishingPolicyManager = Nothing
        Private m_params As cFishingPolicyParameters = Nothing

        Private m_gridObjectiveWeights As gridSearchObjectivesWeight = Nothing
        Private m_gridFleetObjectives As gridSearchObjectivesFleet = Nothing
        Private m_gridGroupObjectives As gridSearchObjectivesGroup = Nothing
        Private m_gridSystemObjectives As gridFPSResultSystemObjectives = Nothing
        Private m_gridSystemObjectivesMulti As gridFPSResultSystemObjectives = Nothing
        Private m_gridFleetValue As gridFPSResultFleetValue = Nothing

        Private m_fpDiscRate As cPropertyFormatProvider = Nothing
        Private m_fpGenDiscRate As cPropertyFormatProvider = Nothing

        Private m_propBaseYear As cProperty = Nothing

        Private m_lstOptVisControls As New List(Of cControlVisContainer)

        ''' <summary>Results to be plotted</summary>
        ''' <remarks></remarks>
        Private m_lptsResults() As ResultPoints
        Private m_zghResults As ZedGraphHelper

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance()

            'Initialize Fishing Policy Manager
            Me.m_manager = Me.m_core.FishingPolicyManager
            Me.m_params = Me.m_manager.ModelParameters

            Me.m_manager.Connect(AddressOf Me.RunStartedHandler, AddressOf Me.RunCompletedHandler, _
                                AddressOf Me.SearchProgressHandler, AddressOf Me.SearchCompletedHandler)


            Me.m_blocks = New ucPolicyColorBlocks()
            Me.m_gridObjectiveWeights = New gridSearchObjectivesWeight(Me.m_core.FishingPolicyManager)
            Me.m_gridFleetObjectives = New gridSearchObjectivesFleet(Me.m_core.FishingPolicyManager)
            Me.m_gridGroupObjectives = New gridSearchObjectivesGroup(Me.m_core.FishingPolicyManager)
            Me.m_gridSystemObjectives = New gridFPSResultSystemObjectives()
            Me.m_gridSystemObjectivesMulti = New gridFPSResultSystemObjectives()
            Me.m_gridFleetValue = New gridFPSResultFleetValue()

            Me.m_fpDiscRate = New cPropertyFormatProvider(Me.txDiscountRate, m_core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchDiscountRate)
            Me.m_fpGenDiscRate = New cPropertyFormatProvider(Me.txGenDiscRate, m_core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchGenDiscRate)

            Me.m_propBaseYear = cPropertyManager.GetInstance().GetProperty(m_core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear)
            AddHandler Me.m_propBaseYear.PropertyChanged, AddressOf OnBaseYearChanged

            Me.m_lstOptVisControls.Add(New cControlVisContainer(Me.cbMaxPortUl, eOptimizeApproachTypes.SystemObjective))
            Me.m_lstOptVisControls.Add(New cControlVisContainer(Me.cbPrevCE, eOptimizeApproachTypes.SystemObjective))
            Me.m_lstOptVisControls.Add(New cControlVisContainer(Me.cmbSearchUsing, eOptimizeApproachTypes.SystemObjective))
            Me.m_lstOptVisControls.Add(New cControlVisContainer(Me.lblSearchUsing, eOptimizeApproachTypes.SystemObjective))

            Me.m_lstOptVisControls.Add(New cControlVisContainer(Me.cbIncludeCCosts, eOptimizeApproachTypes.FleetValues))

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.FishingPolicySearch, eCoreComponentType.SearchObjective, eCoreComponentType.TimeSeries}

            Me.OnBaseYearChanged(Me.m_propBaseYear, cProperty.eChangeFlags.Value)

        End Sub

        Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
            MyBase.OnFormClosing(e)

            RemoveHandler Me.m_propBaseYear.PropertyChanged, AddressOf OnBaseYearChanged
            Me.m_propBaseYear = Nothing

            Me.m_zghResults.Detach()
            Me.m_zghResults = Nothing

            Me.CoreComponents = Nothing
        End Sub

        Private Sub setVisibleControls()

            Dim optAproach As eOptimizeApproachTypes = m_params.OptimizeApproach
            For Each ct As cControlVisContainer In m_lstOptVisControls
                ct.Visible(optAproach)
            Next

        End Sub

        Private Sub FishingPolicySearch_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            plBlocks.Controls.Clear()
            plBlocks.Controls.Add(m_blocks)
            m_blocks.Dock = DockStyle.Fill

            SplitContainer2.Panel1.Controls.Clear()
            SplitContainer2.Panel1.Controls.Add(m_gridObjectiveWeights)
            m_gridObjectiveWeights.Dock = DockStyle.Fill

            SplitContainer3.Panel1.Controls.Clear()
            SplitContainer3.Panel1.Controls.Add(m_gridFleetObjectives)
            m_gridFleetObjectives.Dock = DockStyle.Fill

            SplitContainer3.Panel2.Controls.Clear()
            SplitContainer3.Panel2.Controls.Add(m_gridGroupObjectives)
            m_gridGroupObjectives.Dock = DockStyle.Fill

            m_blocks.ParmBlockCodes.nBlockCodes = m_core.nFleets
            m_blocks.ParmBlockCodes.SelectedBlockNum = 1

            InitRunParams()

        End Sub

        Private Sub InitRunParams()

            nupNumOfRuns.Value = CDec(m_params.nRuns)
            nupMaxNumEval.Value = CDec(m_params.MaxNumEval)
            Select Case m_params.InitOption
                Case eInitOption.EcopathBaseF
                    cmbInitUsing.SelectedIndex = 0
                Case eInitOption.CurrentF
                    cmbInitUsing.SelectedIndex = 1
                Case eInitOption.RandomF
                    cmbInitUsing.SelectedIndex = 2
            End Select

            Select Case m_params.SearchOption
                Case eSearchOptionTypes.Fletch
                    cmbSearchUsing.SelectedIndex = 0
                Case eSearchOptionTypes.DFPmin
                    cmbSearchUsing.SelectedIndex = 1
            End Select

            Select Case m_params.OptimizeApproach
                Case eOptimizeApproachTypes.SystemObjective
                    cmbOptmApproach.SelectedIndex = 0
                    InitMaxSOParams()
                Case eOptimizeApproachTypes.FleetValues
                    cmbOptmApproach.SelectedIndex = 1
                    InitMaxFVParams()
            End Select

            ' Plot graph
            InitResultsPlot()

            ' Controls
            setVisibleControls()

            Me.btnSearch.Enabled = True
            Me.btnStop.Enabled = False

        End Sub

        Private Sub InitMaxSOParams()
            cbMaxPortUl.Checked = m_params.MaxPortUtil
            cbPrevCE.Checked = Me.m_manager.ObjectiveParameters.PrevCostEarning
        End Sub

        Private Sub InitMaxFVParams()
            cbIncludeCCosts.Checked = m_params.IncludeComp
            nupMaxEffChg.Value = CDec(m_params.MaxEffChange)
            nudBaseYear.Value = CDec(Me.m_manager.ObjectiveParameters.BaseYear)
        End Sub

        Private Sub nupNumOfRuns_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupNumOfRuns.ValueChanged

            If Not m_params Is Nothing Then
                m_params.nRuns = CInt(nupNumOfRuns.Value)
                If m_params.nRuns > 1 And m_params.InitOption <> eInitOption.RandomF Then
                    m_params.InitOption = eInitOption.RandomF
                    InitRunParams()
                End If
            End If

        End Sub

        Private Sub nupMaxNumEval_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupMaxNumEval.ValueChanged

            If Not m_params Is Nothing Then
                m_params.MaxNumEval = CSng(nupMaxNumEval.Value)
            End If

        End Sub

        Private Sub cbInitUsing_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbInitUsing.SelectedIndexChanged

            If Not m_params Is Nothing Then

                Select Case cmbInitUsing.SelectedIndex
                    Case 0
                        m_params.InitOption = eInitOption.EcopathBaseF
                    Case 1
                        m_params.InitOption = eInitOption.CurrentF
                    Case 2
                        m_params.InitOption = eInitOption.RandomF
                End Select

            End If

        End Sub

        Private Sub cbSearchUsing_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSearchUsing.SelectedIndexChanged

            If Not m_params Is Nothing Then

                Select Case cmbSearchUsing.SelectedIndex
                    Case 0
                        m_params.SearchOption = eSearchOptionTypes.Fletch
                    Case 1
                        m_params.SearchOption = eSearchOptionTypes.DFPmin
                End Select

            End If

        End Sub

        Private Sub cbOptmApproach_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbOptmApproach.SelectedIndexChanged
            If Not m_params Is Nothing Then

                Select Case cmbOptmApproach.SelectedIndex
                    Case 0
                        m_params.OptimizeApproach = eOptimizeApproachTypes.SystemObjective
                        InitMaxSOParams()
                        m_gridFleetObjectives.IsMaximizeByFleetValue = False
                    Case 1
                        m_params.OptimizeApproach = eOptimizeApproachTypes.FleetValues
                        InitMaxFVParams()
                        m_gridFleetObjectives.IsMaximizeByFleetValue = True
                End Select

            End If

            setVisibleControls()

        End Sub

        Private Sub nupMaxEffChg_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupMaxEffChg.ValueChanged

            If Not m_params Is Nothing Then
                m_params.MaxEffChange = CSng(nupMaxEffChg.Value)
            End If

        End Sub

        Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

            tcMain.SelectedIndex = 1

            scIterResultMultiRun.Panel1.Controls.Clear()

            scIterResultMultiRun.Panel1.Controls.Add(m_gridSystemObjectives)
            m_gridSystemObjectives.InsertColumns(m_manager.nSearchBlocks)

            Select Case m_params.OptimizeApproach
                Case eOptimizeApproachTypes.SystemObjective
                    scIterResult.Panel2Collapsed = True
                Case eOptimizeApproachTypes.FleetValues
                    scIterResult.Panel2Collapsed = False
                    scIterResult.Panel2.Controls.Clear()
                    scIterResult.Panel2.Controls.Add(m_gridFleetValue)
            End Select

            If CInt(nupNumOfRuns.Value) > 1 Then
                scIterResultMultiRun.Panel2Collapsed = False
                scIterResultMultiRun.Panel2.Controls.Clear()
                scIterResultMultiRun.Panel2.Controls.Add(m_gridSystemObjectivesMulti)
                m_gridSystemObjectivesMulti.InsertColumns(m_manager.nSearchBlocks)
            Else
                scIterResultMultiRun.Panel2Collapsed = True
            End If

            ' Init the Results plot
            ReInitResultsPlot(m_manager.nSearchBlocks, m_blocks.ParmBlockCodes)

            m_manager.Run(Me)
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

                Me.m_core.Messages.SendMessage(New cMessage(My.Resources.SEARCH_STATUS_COMPLETED, _
                        eMessageType.NotSet, eCoreComponentType.EcoSim, eMessageImportance.Information))

            Catch ex As Exception
                cLog.Write(ex)
                SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        Private Sub RunStartedHandler()

            Try
                Me.m_gridSystemObjectives.RemoveDataRows()

                Me.m_core.Messages.SendMessage(New cMessage(My.Resources.SEARCH_STATUS_STARTED, _
                        eMessageType.NotSet, eCoreComponentType.EcoSim, eMessageImportance.Information))

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
                    Dim results As cFPSSearchResults = m_manager.SearchResults
                    m_gridSystemObjectivesMulti.InsertOneIterResult(results, m_manager.nSearchBlocks, m_blocks.ParmBlockCodes)
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
                Dim results As cFPSSearchResults = m_manager.SearchResults

                If cmbOptmApproach.SelectedIndex = 1 Then
                    m_gridFleetValue.InsertOneIterResult(results)
                End If

                m_gridSystemObjectives.InsertOneIterResult(results, m_manager.nSearchBlocks, m_blocks.ParmBlockCodes)

                UpdateResultsGraph(results)


            Catch ex As Exception
                cLog.Write(ex)
                SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        Private Sub cbIncludeCCosts_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbIncludeCCosts.CheckedChanged
            If Not m_params Is Nothing Then
                m_params.IncludeComp = cbIncludeCCosts.Checked
            End If
        End Sub

        Private Sub cbMaxPortUl_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbMaxPortUl.CheckedChanged
            If Not m_params Is Nothing Then
                m_params.MaxPortUtil = cbMaxPortUl.Checked
                m_gridObjectiveWeights.ShowMaxPortUtil = cbMaxPortUl.Checked
            End If
        End Sub

        Private Sub cbPrevCE_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPrevCE.CheckedChanged
            ' If Not m_FPParams Is Nothing Then
            Me.m_manager.ObjectiveParameters.PrevCostEarning = cbPrevCE.Checked
            '  End If
        End Sub

        Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
            m_manager.StopRun()
        End Sub

        'send a generic error message
        Private Sub SendErrorMessage(ByVal theMessage As String)
            m_core.Messages.SendMessage(New cMessage(theMessage, eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Critical, eDataTypes.FishingPolicyManager))
        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)
            If msg.Source = eCoreComponentType.TimeSeries Then
                Me.OnBaseYearChanged(Me.m_propBaseYear, cProperty.eChangeFlags.All)
            End If
        End Sub

        Private m_bInUpdate As Boolean = False

        Private Sub OnBaseYearChanged(ByVal prop As cProperty, ByVal cf As cProperty.eChangeFlags)
            Debug.Assert(Object.ReferenceEquals(prop, Me.m_propBaseYear))

            If Me.m_bInUpdate Then Return

            'If (cf And cProperty.eChangeFlags.Value) = cProperty.eChangeFlags.Value Then
            Me.m_bInUpdate = True
            Me.nudBaseYear.Value = CInt(prop.GetValue()) + Me.m_core.EcosimFirstYear
            Me.m_bInUpdate = False
            'End If

        End Sub

        Private Sub OnBaseYearChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles nudBaseYear.ValueChanged

            Dim iStart As Integer = Me.m_core.EcosimFirstYear
            Dim iEnd As Integer = iStart + Me.m_core.nEcosimYears
            Dim iValue As Integer = iStart

            Try
                iValue = CInt(Val(Me.nudBaseYear.Value))
            Catch ex As Exception
                ' Whoops
            End Try

            iValue = Math.Max(Math.Min(iValue, iEnd), iStart)

            Me.m_propBaseYear.SetValue(iValue - iStart)

        End Sub

        Private Sub OnResultCursorPos(ByVal zgh As ZedGraphHelper, ByVal iPane As Integer, ByVal sPos As Single)
            Me.ShowIteration(CInt(Math.Round(Me.m_zghResults.CursorPos)))
        End Sub

#Region " Graphing Region "

        Private Sub InitResultsPlot()

            Me.m_zghResults = New ZedGraphHelper(Me.m_graphResults)
            Me.m_zghResults.ShowCursor = False

            AddHandler Me.m_zghResults.OnCursorPos, AddressOf OnResultCursorPos
        End Sub

        Private Sub ReInitResultsPlot(ByVal nSearchBlocks As Integer, ByRef pbc As ucParmBlockCodes)
            Dim zgcr As New ZedGraph.ColorSymbolRotator

            Me.m_graphResults.GraphPane.Legend.Position = ZedGraph.LegendPos.Right
            Me.m_graphResults.GraphPane.Title.IsVisible = False
            Me.m_graphResults.GraphPane.XAxis.Title.Text = "Iterations"
            Me.m_graphResults.GraphPane.YAxis.Title.Text = "Objective value"

            Me.m_graphResults.GraphPane.CurveList.Clear()

            ' JS 19nov08: let graph figure out the ticks
            '' Only show major ticks
            'Me.m_graphResults.GraphPane.XAxis.Scale.MajorStep = 5
            'Me.m_graphResults.GraphPane.XAxis.Scale.MinorStep = 1
            ReDim m_lptsResults(6) ' + nSearchBlocks) Will not plot blocks yet

            Me.m_lptsResults(1) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.HEADER_NET_ECONOMIC_VALUE, Me.m_lptsResults(1), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(2) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.HEADER_SOCIAL_VALUE_EMPLOYMENT, Me.m_lptsResults(2), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(3) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.HEADER_MANDATED_REBUILDING, Me.m_lptsResults(3), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(4) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.HEADER_ECOSYSTEM_STRUCTURE, Me.m_lptsResults(4), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(5) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.HEADER_BIOMASS_DIVERSITY, Me.m_lptsResults(5), zgcr.NextColor, ZedGraph.SymbolType.None)

            ' Will not plot blocks for now
            'For i As Integer = 1 To nSearchBlocks
            '    Me.m_lptsResults(6) = New ResultPoints()
            '    Me.m_graphResults.GraphPane.AddCurve("Block " & i.ToString, Me.m_lptsResults(5 + i), pbc.BlockColor(i), ZedGraph.SymbolType.None)
            'Next

            Me.m_zghResults.AutoscalePane = True

        End Sub

        Private Sub UpdateResultsGraph(ByVal results As cFPSSearchResults)


            Dim aiBlocks() As Integer = results.BlockNumber
            Dim asResults() As Single = results.BlockResults

            Try
                ' Fill output graph
                For iResult As Integer = 1 To results.CriteriaValues.Length - 1
                    Me.m_lptsResults(iResult).AddItem(results.CriteriaValues(iResult), CSng(results.nCalls))
                Next

                'Me.m_graphResults.GraphPane.XAxis.Scale.Max = m_lptsResults.Count - 1

                Me.m_zghResults.CursorPos = 0.0
                Me.m_zghResults.RescaleAndRedraw()

            Catch ex As Exception
                Debug.Assert(False, "Failed to add Items to results")
            End Try

        End Sub

        Private Sub ShowIteration(ByVal iIteration As Integer)

            Dim lResults As List(Of cObjectiveResult) = Nothing
            Dim res As cObjectiveResult = Nothing


            '' Get the results
            'lResults = Me.m_manager.Results()


            'iIteration = Math.Max(0, Math.Min(lResults.Count - 1, iIteration))

            'If iIteration = -1 Then Return

            '' Update indicators
            'Me.m_gridResults.LogResult(res.objFuncEconomicValue, res.objFuncSocialValue, _
            '                           res.objFuncMandatedValue, res.objFuncEcologicalValue, _
            '                           res.objBiomassDiversity, res.objFuncAreaBorder, _
            '                           res.objFuncTotal, res.PercentageClosed)


        End Sub
#End Region ' Graphing region

#Region " Helper classes "

        ''' <summary>
        ''' Utility class for maintaining a list of results in the output (zed)graph
        ''' </summary>
        Private Class ResultPoints
            Implements ZedGraph.IPointList

            Private m_list As New List(Of ZedGraph.PointPair)

            Public Sub Clear()
                Me.m_list.Clear()
            End Sub

            Public Function Clone() As Object Implements System.ICloneable.Clone
                Return Nothing
            End Function

            Public ReadOnly Property Count() As Integer Implements ZedGraph.IPointList.Count
                Get
                    Return Me.m_list.Count
                End Get
            End Property

            Default Public ReadOnly Property Item(ByVal index As Integer) As ZedGraph.PointPair Implements ZedGraph.IPointList.Item
                Get
                    Return Me.m_list(index)
                End Get
            End Property

            Public Sub AddItem(ByVal yValue As Single, Optional ByVal xValue As Single = Nothing)
                If xValue = Nothing Then
                    Me.m_list.Add(New ZedGraph.PointPair(Me.Count, yValue))
                Else
                    Me.m_list.Add(New ZedGraph.PointPair(xValue, yValue))
                End If

            End Sub

        End Class

#End Region ' Helper classes

    End Class ' frmFishingPolicySearch

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
