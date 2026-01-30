' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.FishingPolicy
Imports EwEUtils.Logging
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    ''' <summary>
    ''' Form class for Fishing Policy Search form
    ''' </summary>
    Public Class frmFishingPolicySearch

        Private m_manager As cFishingPolicyManager = Nothing

        Private m_gridSystemObjectives As gridFPSResultSystemObjectives = Nothing
        Private m_gridSystemObjectivesMulti As gridFPSResultSystemObjectives = Nothing
        Private m_gridFleetValue As gridFPSResultFleetValue = Nothing

        Private m_fpDiscRate As cPropertyFormatProvider = Nothing
        Private m_fpGenDiscRate As cPropertyFormatProvider = Nothing
        Private m_fpUsePlugin As cPropertyFormatProvider = Nothing

        Private m_propBaseYear As cProperty = Nothing
        Private m_blockData As IPolicyColorBlockDataSource = Nothing

        Private m_lstOptEnabled As New List(Of cControlEnabler)
        ''' <summary>Results to be plotted</summary>
        Private m_lptsResults() As ResultPoints
        Private m_zghResults As cZedGraphHelper
        Private m_bInUpdate As Boolean = False
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmFishingPolicySearch)()

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#Region " Overrides "

        Public Overrides ReadOnly Property IsRunForm As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            'Initialize Fishing Policy Manager
            Me.m_manager = Me.Core.FishingPolicyManager
            Me.m_manager.Connect(AddressOf Me.RunStartedHandler, AddressOf Me.RunCompletedHandler,
                                AddressOf Me.SearchProgressHandler, AddressOf Me.SearchCompletedHandler)

            Me.m_gridObjWeights.UIContext = Me.UIContext
            Me.m_gridObjWeights.Manager = Me.Core.FishingPolicyManager

            Me.m_gridObjFleet.UIContext = Me.UIContext
            Me.m_gridObjFleet.Manager = Me.Core.FishingPolicyManager

            Me.m_gridObjGroup.UIContext = Me.UIContext
            Me.m_gridObjGroup.Manager = Me.Core.FishingPolicyManager

            Me.m_gridSystemObjectives = New gridFPSResultSystemObjectives()
            Me.m_gridSystemObjectives.UIContext = Me.UIContext
            Me.m_gridSystemObjectives.Dock = DockStyle.Fill

            Me.m_gridSystemObjectivesMulti = New gridFPSResultSystemObjectives()
            Me.m_gridSystemObjectivesMulti.UIContext = Me.UIContext
            Me.m_gridSystemObjectivesMulti.Dock = DockStyle.Fill

            Me.m_gridFleetValue = New gridFPSResultFleetValue()
            Me.m_gridFleetValue.UIContext = Me.UIContext
            Me.m_gridFleetValue.Dock = DockStyle.Fill

            Me.m_fpDiscRate = New cPropertyFormatProvider(Me.UIContext, Me.m_txtDiscountRate, Me.Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchDiscountRate)
            Me.m_fpGenDiscRate = New cPropertyFormatProvider(Me.UIContext, Me.m_txtGenDiscount, Me.Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchGenDiscRate)

            Me.m_fpUsePlugin = New cPropertyFormatProvider(Me.UIContext, Me.m_chkUsePlugin, Me.Core.FishingPolicyManager.ModelParameters, eVarNameFlags.FPSUseEconomicPlugin)

            Me.m_propBaseYear = Me.PropertyManager.GetProperty(Me.Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear)
            AddHandler Me.m_propBaseYear.PropertyChanged, AddressOf Me.OnBaseYearChanged

            Me.m_lstOptEnabled.Add(New cControlEnabler(Me.m_chkMaxPortUl, eOptimizeApproachTypes.SystemObjective))
            Me.m_lstOptEnabled.Add(New cControlEnabler(Me.m_chkPrevCE, eOptimizeApproachTypes.SystemObjective))
            Me.m_lstOptEnabled.Add(New cControlEnabler(Me.m_cmbSearchUsing, eOptimizeApproachTypes.SystemObjective))
            Me.m_lstOptEnabled.Add(New cControlEnabler(Me.m_lblSearchUsing, eOptimizeApproachTypes.SystemObjective))
            Me.m_lstOptEnabled.Add(New cControlEnabler(Me.m_chkUsePlugin, eOptimizeApproachTypes.SystemObjective))

            Me.m_lstOptEnabled.Add(New cControlEnabler(Me.m_chkIncludeCCosts, eOptimizeApproachTypes.FleetValues))

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.FishingPolicySearch, eCoreComponentType.SearchObjective, eCoreComponentType.TimeSeries, eCoreComponentType.Ecosim}

            Me.OnBaseYearChanged(Me.m_propBaseYear, cProperty.eChangeFlags.Value)

            Me.m_blockData = New cFPSearchColorBlockDataSource(Me.UIContext)
            Me.m_blocks.Attach(Me.m_blockData, New ucParmBlockCodes())
            Me.m_blocks.Invalidate()

            '     Me.m_blocks.DataSource = New cFPSearchColorBlockDataSource(Me.UIContext)
            Me.m_blocks.ParmBlockCodes.NumBlocks = Me.Core.nFleets
            Me.m_blocks.ParmBlockCodes.SelectedBlock = 1

            Me.InitRunParams()
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

            Me.m_manager.DisConnect()

            'jb remove the Results grids from the interface panels
            'this did not fix the gridFPSResultSystemObjectives memory leak...
            Me.m_scIterResultMultiRun.Panel1.Controls.Clear()
            Me.m_scIterResultMultiRun.Panel2.Controls.Clear()

            Me.m_lstOptEnabled.Clear()

            Me.m_blocks.Detach()
            Me.m_blocks.Dispose()

            RemoveHandler Me.m_propBaseYear.PropertyChanged, AddressOf Me.OnBaseYearChanged
            Me.m_propBaseYear = Nothing

            Me.m_fpDiscRate.Release()
            Me.m_fpGenDiscRate.Release()
            Me.m_fpUsePlugin.Release()
            Me.m_fpDiscRate = Nothing
            Me.m_fpUsePlugin = Nothing
            Me.m_fpGenDiscRate = Nothing

            Me.m_zghResults.Detach()
            Me.m_zghResults = Nothing

            MyBase.OnFormClosed(e)

        End Sub

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
                MyBase.UIContext = value
                Me.m_blocks.UIContext = value
            End Set
        End Property

#End Region ' Overrides

#Region " Internals "

        Private Sub InitRunParams()

            Me.m_nudNumberOfRuns.Value = CDec(Me.m_manager.ModelParameters.nRuns)
            Me.m_nudMaxNumEval.Value = CDec(Me.m_manager.ModelParameters.MaxNumEval)
            Select Case Me.m_manager.ModelParameters.InitOption
                Case eInitOption.EcopathBaseF
                    Me.m_cmbInitUsing.SelectedIndex = 0
                Case eInitOption.CurrentF
                    Me.m_cmbInitUsing.SelectedIndex = 1
                Case eInitOption.RandomF
                    Me.m_cmbInitUsing.SelectedIndex = 2
            End Select

            Select Case Me.m_manager.ModelParameters.SearchOption
                Case eSearchOptionTypes.Fletch
                    Me.m_cmbSearchUsing.SelectedIndex = 0
                Case eSearchOptionTypes.DFPmin
                    Me.m_cmbSearchUsing.SelectedIndex = 1
            End Select

            Select Case Me.m_manager.ModelParameters.OptimizeApproach
                Case eOptimizeApproachTypes.SystemObjective
                    Me.m_cmbOptmApproach.SelectedIndex = 0
                    Me.InitMaxSOParams()
                Case eOptimizeApproachTypes.FleetValues
                    Me.m_cmbOptmApproach.SelectedIndex = 1
                    Me.InitMaxFVParams()
            End Select

            ' Plot graph
            Me.InitResultsPlot()

            ' Controls
            Me.UpdateControls()

            ''set the enabled state of the Use Economic data checkbox
            'Me.updateEconomicDataAvailable()

            Me.btnSearch.Enabled = True
            Me.btnStop.Enabled = False

        End Sub

        Private Sub InitMaxSOParams()
            Me.m_chkMaxPortUl.Checked = Me.m_manager.ModelParameters.MaxPortUtil
            Me.m_chkPrevCE.Checked = Me.m_manager.ObjectiveParameters.PrevCostEarning
            Me.m_chkUsePlugin.Checked = Me.m_manager.ModelParameters.UseEconomicPlugin
        End Sub

        Private Sub InitMaxFVParams()
            Me.m_chkIncludeCCosts.Checked = Me.m_manager.ModelParameters.IncludeComp
            Me.m_nudMaxEffChg.Value = CDec(Me.m_manager.ModelParameters.MaxEffChange)
            Me.m_nudBaseYear.Value = CDec(Me.m_manager.ObjectiveParameters.BaseYear)
        End Sub

        Protected Overrides Sub UpdateControls()

            Dim optAproach As eOptimizeApproachTypes = Me.m_manager.ModelParameters.OptimizeApproach
            For Each ct As cControlEnabler In Me.m_lstOptEnabled
                ct.Enabled(optAproach)
            Next

            Me.m_chkUsePlugin.Enabled = (optAproach = eOptimizeApproachTypes.SystemObjective)

        End Sub

        Private Sub SendErrorMessage(theMessage As String)
            Me.Core.Messages.SendMessage(New cMessage(theMessage, eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Critical, eDataTypes.FishingPolicyManager))
        End Sub

#End Region ' Internals

#Region " Event handlers "

        Private Sub m_nudNumberOfRuns_ValueChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudNumberOfRuns.ValueChanged

            ' Check to discard premature NumericUpDown events
            If Me.m_manager Is Nothing Then Return

            Me.m_manager.ModelParameters.nRuns = CInt(Me.m_nudNumberOfRuns.Value)
            If Me.m_manager.ModelParameters.nRuns > 1 And Me.m_manager.ModelParameters.InitOption <> eInitOption.RandomF Then
                Me.m_manager.ModelParameters.InitOption = eInitOption.RandomF
                Me.InitRunParams()
            End If

        End Sub

        Private Sub m_nudMaxNumEval_ValueChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudMaxNumEval.ValueChanged

            ' Check to discard premature NumericUpDown events
            If Me.m_manager Is Nothing Then Return

            Me.m_manager.ModelParameters.MaxNumEval = CSng(Me.m_nudMaxNumEval.Value)

        End Sub

        Private Sub m_cmbInitUsing_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbInitUsing.SelectedIndexChanged

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return

            Select Case Me.m_cmbInitUsing.SelectedIndex
                Case 0
                    Me.m_manager.ModelParameters.InitOption = eInitOption.EcopathBaseF
                Case 1
                    Me.m_manager.ModelParameters.InitOption = eInitOption.CurrentF
                Case 2
                    Me.m_manager.ModelParameters.InitOption = eInitOption.RandomF
            End Select

        End Sub

        Private Sub m_cmbSearchUsing_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbSearchUsing.SelectedIndexChanged

            If Not Me.m_manager.ModelParameters Is Nothing Then

                Select Case Me.m_cmbSearchUsing.SelectedIndex
                    Case 0
                        Me.m_manager.ModelParameters.SearchOption = eSearchOptionTypes.Fletch
                    Case 1
                        Me.m_manager.ModelParameters.SearchOption = eSearchOptionTypes.DFPmin
                End Select

            End If

        End Sub

        Private Sub OnOptmApproach_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbOptmApproach.SelectedIndexChanged

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return

            Select Case Me.m_cmbOptmApproach.SelectedIndex
                Case 0
                    Me.m_manager.ModelParameters.OptimizeApproach = eOptimizeApproachTypes.SystemObjective
                    Me.InitMaxSOParams()
                    Me.m_gridObjFleet.IsMaximizeByFleetValue = False
                Case 1
                    Me.m_manager.ModelParameters.OptimizeApproach = eOptimizeApproachTypes.FleetValues
                    Me.InitMaxFVParams()
                    Me.m_gridObjFleet.IsMaximizeByFleetValue = True
            End Select

            Me.UpdateControls()

        End Sub

        Private Sub OnMaxEffValueChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudMaxEffChg.ValueChanged

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return

            Me.m_manager.ModelParameters.MaxEffChange = CSng(Me.m_nudMaxEffChg.Value)

        End Sub

        Private Sub btnSearch_Click(sender As System.Object, e As System.EventArgs) _
            Handles btnSearch.Click

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return

            Me.m_tcMain.SelectedIndex = 1

            Me.m_scIterResultMultiRun.Panel1.Controls.Clear()

            Me.m_scIterResultMultiRun.Panel1.Controls.Add(Me.m_gridSystemObjectives)
            Me.m_gridSystemObjectives.InsertColumns(Me.m_manager.nSearchBlocks)

            Select Case Me.m_manager.ModelParameters.OptimizeApproach
                Case eOptimizeApproachTypes.SystemObjective
                    Me.m_scIterResult.Panel2Collapsed = True
                Case eOptimizeApproachTypes.FleetValues
                    Me.m_scIterResult.Panel2Collapsed = False
                    Me.m_scIterResult.Panel2.Controls.Clear()
                    Me.m_scIterResult.Panel2.Controls.Add(Me.m_gridFleetValue)
            End Select

            If CInt(Me.m_nudNumberOfRuns.Value) > 1 Then
                Me.m_scIterResultMultiRun.Panel2Collapsed = False
                Me.m_scIterResultMultiRun.Panel2.Controls.Clear()
                Me.m_scIterResultMultiRun.Panel2.Controls.Add(Me.m_gridSystemObjectivesMulti)
                Me.m_gridSystemObjectivesMulti.InsertColumns(Me.m_manager.nSearchBlocks)
            Else
                Me.m_scIterResultMultiRun.Panel2Collapsed = True
            End If

            ' Init the Results plot
            Me.ReInitResultsPlot(Me.m_manager.nSearchBlocks, DirectCast(Me.m_blocks.ParmBlockCodes, ucParmBlockCodes))

            Me.m_manager.Run(Me)
            Me.btnSearch.Enabled = False
            Me.btnStop.Enabled = True

            Me.m_plRunParams.Enabled = False
            Me.m_blocks.Enabled = False

            cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_SEARCH_SEARCHING)

        End Sub

        Private Sub cbIncludeCCosts_CheckedChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_chkIncludeCCosts.CheckedChanged

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return
            Me.m_manager.ModelParameters.IncludeComp = Me.m_chkIncludeCCosts.Checked

        End Sub

        Private Sub cbMaxPortUl_CheckedChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_chkMaxPortUl.CheckedChanged

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return
            Me.m_manager.ModelParameters.MaxPortUtil = Me.m_chkMaxPortUl.Checked
            Me.m_gridObjWeights.ShowMaxPortUtil = Me.m_chkMaxPortUl.Checked

        End Sub

        Private Sub cbPrevCE_CheckedChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_chkPrevCE.CheckedChanged

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return
            Me.m_manager.ObjectiveParameters.PrevCostEarning = Me.m_chkPrevCE.Checked

        End Sub

        Private Sub btnStop_Click(sender As System.Object, e As System.EventArgs) Handles btnStop.Click
            Me.m_manager.StopRun(0)
        End Sub

#End Region ' Event handlers

#Region " Callbacks "

        ''' <summary>
        ''' Delegate for cFishingPolicyManager.SearchCompletedHandler. This sub will be called when cFishingPolicyManager.Run has completed.
        ''' </summary>
        Private Sub SearchCompletedHandler()

            Try

                Me.btnSearch.Enabled = True
                Me.btnStop.Enabled = False

                Me.m_plRunParams.Enabled = True
                Me.m_blocks.Enabled = True

                cApplicationStatusNotifier.EndProgress(Me.Core)

                Me.Core.Messages.SendMessage(New cMessage(My.Resources.SEARCH_STATUS_COMPLETED,
                        eMessageType.NotSet, eCoreComponentType.Ecosim, eMessageImportance.Information))

            Catch ex As Exception
                m_logger.LogError(ex, "Error in Fishing Policy search.")
                Me.SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        Private Sub RunStartedHandler()

            Try
                Me.m_gridSystemObjectives.RemoveDataRows()

                Me.Core.Messages.SendMessage(New cMessage(My.Resources.SEARCH_STATUS_STARTED,
                        eMessageType.NotSet, eCoreComponentType.Ecosim, eMessageImportance.Information))

            Catch ex As Exception
                m_logger.LogError(ex, "RunStartedHandler. Error in Fishing Policy search.")
                Me.SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' A Fishing Policy Search run has completed
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RunCompletedHandler()

            Try
                If CInt(Me.m_nudNumberOfRuns.Value) > 1 Then
                    Dim results As cFPSSearchResults = Me.m_manager.SearchResults
                    Me.m_gridSystemObjectivesMulti.InsertOneIterResult(results, Me.m_manager.nSearchBlocks, DirectCast(Me.m_blocks.ParmBlockCodes, ucParmBlockCodes))
                End If
            Catch ex As Exception
                m_logger.LogError(ex, "RunCompletedHandler. Error in Fishing Policy search.")
                Me.SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' Delegate for cFishingPolicyManager.ProgressHandler(). This will be called by the FishingPolicyManager to update the search progress.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SearchProgressHandler()

            Try
                'get the results object from the manager
                'cFishingPolicyManager.SearchResults will be populate with the results of the Search at the current iteration
                Dim results As cFPSSearchResults = Me.m_manager.SearchResults

                If Me.m_cmbOptmApproach.SelectedIndex = 1 Then
                    Me.m_gridFleetValue.InsertOneIterResult(results)
                End If

                Me.m_gridSystemObjectives.InsertOneIterResult(results, Me.m_manager.nSearchBlocks, DirectCast(Me.m_blocks.ParmBlockCodes, ucParmBlockCodes))

                Me.UpdateResultsGraph(results)


            Catch ex As Exception
                m_logger.LogError(ex, "SearchProgressHandler. Error in Fishing Policy search.")
                Me.SendErrorMessage("Error in Fishing Policy search. " & ex.Message)
            End Try

        End Sub

        Public Overrides Sub OnCoreMessage(msg As cMessage)

            Select Case msg.Source
                Case eCoreComponentType.TimeSeries
                    Me.OnBaseYearChanged(Me.m_propBaseYear, cProperty.eChangeFlags.All)
                Case eCoreComponentType.Ecosim
                    If (msg.Type = eMessageType.EcosimNYearsChanged) Then
                        ' HACK! Solves #1263
                        Me.m_blockData.Init()
                        Me.m_blocks.Refresh()
                    End If
            End Select

        End Sub

        Private Sub OnBaseYearChanged(prop As cProperty, cf As cProperty.eChangeFlags)
            Debug.Assert(ReferenceEquals(prop, Me.m_propBaseYear))

            If Me.m_bInUpdate Then Return

            'If (cf And cProperty.eChangeFlags.Value) = cProperty.eChangeFlags.Value Then
            Me.m_bInUpdate = True
            Me.m_nudBaseYear.Value = CInt(prop.GetValue()) + Me.Core.EcosimFirstYear
            Me.m_bInUpdate = False
            'End If

        End Sub

        Private Sub OnBaseYearChanged(sender As Object, e As EventArgs) _
            Handles m_nudBaseYear.ValueChanged

            ' Check to discard premature events
            If Me.m_manager Is Nothing Then Return

            Dim iStart As Integer = Me.Core.EcosimFirstYear
            Dim iEnd As Integer = iStart + Me.Core.nEcosimYears
            Dim iValue As Integer = iStart

            Try
                iValue = CInt(Val(Me.m_nudBaseYear.Value))
            Catch ex As Exception
                ' Whoops
            End Try

            iValue = Math.Max(Math.Min(iValue, iEnd), iStart)

            Me.m_propBaseYear.SetValue(iValue - iStart)

        End Sub

        Private Sub OnResultCursorPos(zgh As cZedGraphHelper, iPane As Integer, sPos As Single)
            Me.ShowIteration(CInt(Math.Round(Me.m_zghResults.CursorPos)))
        End Sub

#End Region ' Callbacks

#Region " Graphing Region "

        Private Sub InitResultsPlot()

            Me.m_zghResults = New cZedGraphHelper()
            Me.m_zghResults.Attach(Me.UIContext, Me.m_graphResults)
            Me.m_zghResults.ShowCursor = False

            AddHandler Me.m_zghResults.OnCursorPos, AddressOf Me.OnResultCursorPos
        End Sub

        Private Sub ReInitResultsPlot(nSearchBlocks As Integer, pbc As ucParmBlockCodes)
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
            ReDim Me.m_lptsResults(6) ' + nSearchBlocks) Will not plot blocks yet

            Me.m_lptsResults(1) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(SharedResources.HEADER_NET_ECONOMIC_VALUE, Me.m_lptsResults(1), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(2) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(SharedResources.HEADER_SOCIAL_VALUE_EMPLOYMENT, Me.m_lptsResults(2), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(3) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(SharedResources.HEADER_MANDATED_REBUILDING, Me.m_lptsResults(3), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(4) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(SharedResources.HEADER_ECOSYSTEM_STRUCTURE, Me.m_lptsResults(4), zgcr.NextColor, ZedGraph.SymbolType.None)

            Me.m_lptsResults(5) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(SharedResources.HEADER_BIODIVERSITY, Me.m_lptsResults(5), zgcr.NextColor, ZedGraph.SymbolType.None)

            ' Will not plot blocks for now
            'For i As Integer = 1 To nSearchBlocks
            '    Me.m_lptsResults(6) = New ResultPoints()
            '    Me.m_graphResults.GraphPane.AddCurve("Block " & i.ToString, Me.m_lptsResults(5 + i), pbc.BlockColor(i), ZedGraph.SymbolType.None)
            'Next

            Me.m_zghResults.AutoscalePane = True

        End Sub

        Private Sub UpdateResultsGraph(results As cFPSSearchResults)


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

        Private Sub ShowIteration(iIteration As Integer)

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

            Default Public ReadOnly Property Item(index As Integer) As ZedGraph.PointPair Implements ZedGraph.IPointList.Item
                Get
                    Return Me.m_list(index)
                End Get
            End Property

            Public Sub AddItem(yValue As Single, Optional xValue As Single = Nothing)
                If xValue = Nothing Then
                    Me.m_list.Add(New ZedGraph.PointPair(Me.Count, yValue))
                Else
                    Me.m_list.Add(New ZedGraph.PointPair(xValue, yValue))
                End If

            End Sub

        End Class

        ''' <summary>
        ''' Wrapper for a control that sets the enabled state of the control based on a eOptimizeApproachTypes 
        ''' </summary>
        Private Class cControlEnabler

            Private m_ct As System.Windows.Forms.Control
            Private m_EnabledState As eOptimizeApproachTypes

            Public Sub New(Control As System.Windows.Forms.Control, EnabledState As eOptimizeApproachTypes)
                Me.m_ct = Control
                Me.m_EnabledState = EnabledState
            End Sub

            Public Sub Enabled(OptAproach As eOptimizeApproachTypes)
                If OptAproach = Me.m_EnabledState Then
                    Me.m_ct.Enabled = True
                Else
                    Me.m_ct.Enabled = False
                End If
            End Sub

        End Class

#End Region ' Helper classes

    End Class ' frmFishingPolicySearch

#Region "DataSource implementation for ucPolicyColorBlocks"

    Public Class cFPSearchColorBlockDataSource
        Implements IPolicyColorBlockDataSource

        Private m_uic As cUIContext

        Private m_BlockCells(,) As Integer

        Private m_BlockSelector As IBlockSelector
        Private m_iTotalBlocks As Integer

        Public ReadOnly Property BlockCells() As Integer(,) Implements IPolicyColorBlockDataSource.BlockCells
            Get
                Return Me.m_BlockCells
            End Get
        End Property


        Public ReadOnly Property TotalBlocks() As Integer Implements IPolicyColorBlockDataSource.TotalBlocks
            Get
                Return Me.m_uic.Core.EcosimModelParameters.NumberYears
            End Get
        End Property

        Public Sub New(UIContext As cUIContext)
            Me.m_uic = UIContext
        End Sub

        Public Sub Atatch(Blocks As IBlockSelector) Implements IPolicyColorBlockDataSource.Attach
            Me.m_BlockSelector = Blocks
        End Sub

        Public Sub Init() Implements IPolicyColorBlockDataSource.Init

            Me.m_iTotalBlocks = Me.m_uic.Core.EcosimModelParameters.NumberYears

            ReDim Me.m_BlockCells(Me.m_uic.Core.nFleets, Me.m_iTotalBlocks)
            Dim fpFleetInput As cFishingPolicySearchBlock = Nothing

            For i As Integer = 1 To Me.m_BlockCells.GetLength(0) - 1
                fpFleetInput = Me.m_uic.Core.FishingPolicyManager.SearchBlocks(i)
                For j As Integer = 1 To Me.m_BlockCells.GetLength(1) - 1
                    Me.m_BlockCells(i, j) = fpFleetInput.SearchBlocks(j)
                Next
            Next

        End Sub

        Public Sub FillBlock(iRow As Integer, iCol As Integer) Implements IPolicyColorBlockDataSource.FillBlock

            ' Sanity checks
            If (iCol <= Me.m_uic.Core.FishingPolicyManager.ObjectiveParameters.BaseYear) Then Return

            If (iRow < 1) Then Return
            If (iRow > Me.m_BlockCells.GetLength(0) - 1) Then Return

            ' Fill single block
            Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iRow).SearchBlocks(iCol) = Me.m_BlockSelector.SelectedBlock
            Me.m_BlockCells(iRow, iCol) = Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iRow).SearchBlocks(iCol)

        End Sub

        Public Sub SetSeqColorCodes(startYear As Integer, endYear As Integer, yearPerBlock As Integer) Implements IPolicyColorBlockDataSource.SetSeqColorCodes

            ' If m_bIsFirstTimeLoaded Then Return
            If startYear > endYear Or startYear <= 0 Or endYear <= 0 Then Return
            If Me.m_BlockCells Is Nothing Then Return
            If endYear > Me.m_BlockCells.GetLength(1) - 1 Then endYear = Me.m_BlockCells.GetLength(1) - 1

            Dim nColors As Integer = Me.m_BlockSelector.NumBlocks - 1
            Dim yearSegment As Integer = CInt(Math.Ceiling(Me.m_iTotalBlocks / yearPerBlock))
            Dim totalClr As Integer = yearSegment * Me.m_uic.Core.nFleets

            If totalClr > nColors Then
                Me.m_BlockSelector.NumBlocks = totalClr + 1
            End If

            Dim cnt As Integer = 1
            Dim stepSize As Integer = CInt(Math.Floor(Me.m_BlockSelector.NumBlocks / totalClr))

            For i As Integer = 1 To Me.m_BlockCells.GetLength(0) - 1
                'Console.WriteLine("iColor = {0} selClr = {1}", cnt, selClr)

                For j As Integer = 0 To yearSegment - 1
                    cnt += stepSize
                    For l As Integer = 1 To yearPerBlock
                        Dim jIndex As Integer = j * yearPerBlock + l
                        If jIndex <= endYear AndAlso jIndex >= startYear Then
                            Me.m_BlockCells(i, jIndex) = cnt
                        End If
                    Next
                Next

                ' Black out blocks
                For j As Integer = 1 To startYear - 1
                    Me.m_BlockCells(i, j) = 0
                Next
                For j As Integer = endYear + 1 To Me.m_BlockCells.GetLength(1) - 1
                    Me.m_BlockCells(i, j) = 0
                Next
            Next

            For iflt As Integer = 1 To Me.m_uic.Core.nFleets
                'the batch edit flag stops the searchblocks from sending out any messages
                Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iflt).BatchEdit = True
                For iyr As Integer = 1 To Me.m_uic.Core.nEcosimYears
                    If iyr <= Me.m_uic.Core.FishingPolicyManager.ObjectiveParameters.BaseYear Then
                        'clear all blocks less than the baseyear
                        Me.m_BlockCells(iflt, iyr) = 0
                    End If
                    Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iflt).SearchBlocks(iyr) = Me.m_BlockCells(iflt, iyr)
                Next
                Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iflt).BatchEdit = False
            Next iflt

            'm_pbFishingBlocks.Invalidate()

        End Sub

        Public ReadOnly Property nRows() As Integer Implements IPolicyColorBlockDataSource.nRows
            Get
                Return Me.m_uic.Core.nFleets
            End Get
        End Property

        Public ReadOnly Property RowLabel(iRow As Integer) As String Implements IPolicyColorBlockDataSource.RowLabel
            Get
                Try
                    Return Me.m_uic.Core.EcopathFleetInputs(iRow).Name
                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".RowLabel() Exception: " & ex.Message)
                End Try
                Return String.Empty
            End Get
        End Property

        Public Property BatchEdit() As Boolean Implements IPolicyColorBlockDataSource.BatchEdit
            Get
                'no implementation of the group batch lock for FPS data
                Return False
            End Get
            Set(value As Boolean)
                'no implementation of the group batch lock for FPS data
            End Set
        End Property

        Public Sub Update() Implements IPolicyColorBlockDataSource.Update

            Try
                For iflt As Integer = 1 To Me.m_uic.Core.nFleets
                    'the batch edit flag stops the searchblocks from sending out any messages
                    Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iflt).BatchEdit = True
                    For iyr As Integer = 1 To Me.m_uic.Core.nEcosimYears
                        If iyr <= Me.m_uic.Core.FishingPolicyManager.ObjectiveParameters.BaseYear Then
                            'clear all blocks less than the baseyear
                            Me.m_BlockCells(iflt, iyr) = 0
                        End If
                        Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iflt).SearchBlocks(iyr) = Me.m_BlockCells(iflt, iyr)
                    Next
                    Me.m_uic.Core.FishingPolicyManager.SearchBlocks(iflt).BatchEdit = False
                Next iflt

            Catch ex As Exception
                System.Console.WriteLine(ex.Message)
            End Try
        End Sub

        Public ReadOnly Property isControlPanelVisible() As Boolean Implements IPolicyColorBlockDataSource.isControlPanelVisible
            Get
                Return True
            End Get
        End Property


        Public Function BlockToValue(iBlock As Integer) As Single Implements Ecosim.IPolicyColorBlockDataSource.BlockToValue
            'For the fishing policy block selector the iBlock is the value
            Return iBlock
        End Function
    End Class



#End Region

End Namespace
