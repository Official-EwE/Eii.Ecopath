'==============================================================================
'
' $Log: frmMPAOptimizations.vb,v $
' Revision 1.7  2008/11/11 07:31:31  jeroens
' Implemented export
'
' Revision 1.6  2008/11/08 23:57:29  jeroens
' Built basis for export
'
' Revision 1.5  2008/11/07 23:51:28  jeroens
' Looking prettier
'
' Revision 1.4  2008/11/06 23:57:49  jeroens
' Pretty
'
' Revision 1.3  2008/10/15 23:58:10  jeroens
' All layers added by varname, no longer by string
'
' Revision 1.2  2008/10/10 18:04:03  jeroens
' Updated to renamed layers classes
'
' Revision 1.1  2008/09/26 07:32:03  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPFile

#End Region ' Import

Namespace Ecospace

    Public Class frmMPAOptimizations

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

            Public Sub AddItem(ByVal sValue As Single)
                Me.m_list.Add(New ZedGraph.PointPair(Me.Count, sValue))
            End Sub

        End Class

#End Region ' Helper classes

#Region " Private vars "

        Private Enum eFormModeTypes As Integer
            ''' <summary>User is entering values for a new search.</summary>
            Prepare
            ''' <summary>Search has been started.</summary>
            Searching
            ''' <summary>Search is running.</summary>
            Initializing
            ''' <summary>Search is stopping.</summary>
            Stopping
            ''' <summary>Search is done, results are available.</summary>
            Results
        End Enum

        ' The three stooges
        Private m_core As cCore = Nothing
        Private m_MPAOptManager As cMPAOptManager = Nothing
        Private m_basemap As cEcospaceBasemap = Nothing

        ' Layer cache
        ' - All layers in the basemap
        Private m_lLayers As New List(Of cLayer)
        ' - Collections of layers that reflect the run states
        Private m_alayerFeedback() As cLayer = Nothing
        ' - Collections of layers that reflect the ecoseed bits
        Private m_alayerSeed() As cLayer = Nothing
        ' - Temp data structure to feed the run state basemap layer
        Private m_dataRunState As Integer(,)

        ' Parameter IO
        Private m_fpStartYear As cEwEFormatProvider = Nothing
        Private m_fpEndYear As cEwEFormatProvider = Nothing
        Private m_fpBoundaryWeight As cEwEFormatProvider = Nothing
        Private m_fpMinArea As cEwEFormatProvider = Nothing
        Private m_fpMaxArea As cEwEFormatProvider = Nothing
        Private m_fpStepSize As cEwEFormatProvider = Nothing
        Private m_fpIterations As cEwEFormatProvider = Nothing
        Private m_fpBestPercentile As cEwEFormatProvider = Nothing
        Private m_fpMPA As cEwEFormatProvider = Nothing

        Private m_propSearchType As cIntegerProperty = Nothing

        ''' <summary>The one and only control that provides the layers interface.</summary>
        Private m_ucLayers As ucLayersControl = Nothing
        ''' <summary>Grid that allows configuration of objective weights (shared with Ecosim FPS)</summary>
        Private m_ucWeightOPGrid As ValueComponentGrid = Nothing
        ''' <summary>Grid that allows configuration of fleet opt params (shared with Ecosim FPS)</summary>
        Private m_ucFleetOPGrid As FleetOptmParamGrid = Nothing
        ''' <summary>Grid that allows configuration of group opt params (shared with Ecosim FPS)</summary>
        Private m_ucGroupOPGrid As GroupOptmParamGrid = Nothing

        ' Graph helper
        Private m_zghProgress As ZedGraphHelper = Nothing
        Private m_lptsProgress(3) As ResultPoints

        Private m_zghResults As ZedGraphHelper = Nothing
        Private m_lptsResults(4) As ResultPoints

        ' The mode this form is in
        Private m_mode As eFormModeTypes = eFormModeTypes.Prepare

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()

            Me.InitializeComponent()
            Me.m_core = cCore.GetInstance

        End Sub

#End Region ' Constructor

#Region " Events "

#Region " Form "

        Private Sub Ecoseed_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim SpaceOpt As cCoreInputOutputBase = Me.m_core.EcospaceModelParameters
            Dim MPAOpt As cMPAOptParameters = Me.m_core.MPAOptimizationManager.MPAOptimizationParamters

            Me.m_MPAOptManager = m_core.MPAOptimizationManager
            Me.m_MPAOptManager.Connect(Me, AddressOf Me.OnSeedCellCallback, AddressOf OnSeedRunStateCallback)

            ' Add LayersControl
            Me.m_ucLayers = New ucLayersControl()
            m_plLayers.Controls.Add(Me.m_ucLayers)

            ' Add objective grids
            Me.m_ucWeightOPGrid = New ValueComponentGrid(Me.m_MPAOptManager)
            Me.m_ucWeightOPGrid.FixedColumnWidths = False
            Me.m_plWeights.Controls.Add(Me.m_ucWeightOPGrid)

            Me.m_ucFleetOPGrid = New FleetOptmParamGrid(Me.m_MPAOptManager)
            Me.m_ucFleetOPGrid.FixedColumnWidths = False
            Me.m_plFleet.Controls.Add(Me.m_ucFleetOPGrid)

            Me.m_ucGroupOPGrid = New GroupOptmParamGrid(Me.m_MPAOptManager)
            Me.m_ucGroupOPGrid.FixedColumnWidths = False
            Me.m_plGroup.Controls.Add(Me.m_ucGroupOPGrid)

            ' Configure graphs
            Me.InitProgressGraph()
            Me.InitOutputGraph()

            ' Connect to controls
            Me.m_fpStartYear = New cPropertyFormatProvider(Me.m_nudStartYear, MPAOpt, eVarNameFlags.MPAOptStartYear)
            Me.m_fpEndYear = New cPropertyFormatProvider(Me.m_nudEndYear, MPAOpt, eVarNameFlags.MPAOptEndYear)
            Me.m_fpStartYear.Value = Math.Max(CSng(Me.m_fpStartYear.Value), 3)
            Me.m_fpEndYear.Value = Math.Max(CSng(Me.m_fpEndYear.Value), 5)
            Me.m_fpBoundaryWeight = New cPropertyFormatProvider(Me.m_nudBoundaryWeight, MPAOpt, eVarNameFlags.MPAOptBoundaryWeight)

            Me.m_propSearchType = New cIntegerProperty(MPAOpt, eVarNameFlags.MPAOptSearchType)
            AddHandler Me.m_propSearchType.PropertyChanged, AddressOf OnSearchTypeChanged

            Me.m_fpMinArea = New cPropertyFormatProvider(Me.m_nudMinArea, MPAOpt, eVarNameFlags.MPAOptMinArea)
            Me.m_fpMaxArea = New cPropertyFormatProvider(Me.m_nudMaxArea, MPAOpt, eVarNameFlags.MPAOptMaxArea)
            Me.m_fpStepSize = New cPropertyFormatProvider(Me.m_nudStep, MPAOpt, eVarNameFlags.MPAOptStepSize)
            Me.m_fpIterations = New cPropertyFormatProvider(Me.m_nudIterations, MPAOpt, eVarNameFlags.MPAOptIterations)
            Me.m_fpBestPercentile = New cEwEFormatProvider(Me.m_nudBestPercentile, GetType(Integer))

            Me.MessageSources = New eMessageSource() {eMessageSource.EcoSpace}

            ' Kick off
            Me.Reload()
            Me.OnSearchTypeChanged(Me.m_propSearchType, cProperty.eChangeFlags.All)

        End Sub

        ''' <summary>
        ''' Cleanup
        ''' </summary>
        Private Sub frmEcoseed_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed

            Dim alays As cLayer() = Me.m_lLayers.ToArray

            For Each l As cLayer In alays
                Me.RemoveLayer(l)
            Next
            Me.m_lLayers = Nothing

            RemoveHandler Me.m_zghResults.OnCursorPos, AddressOf OnResultCursorPos

            RemoveHandler Me.m_propSearchType.PropertyChanged, AddressOf OnSearchTypeChanged
            Me.m_propSearchType = Nothing

            Me.MessageSources = Nothing

        End Sub

#End Region ' Form

#Region " Controls "

        Private Sub OnRun(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_btnRun.Click
            ' Abort if not all inputs valid
            If Not Me.ValidateInputs Then Return
            ' Start run
            Me.m_MPAOptManager.Run()
        End Sub

        Private Sub OnStop(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_btnStop.Click
            Me.m_MPAOptManager.StopRun()
            ' Ho!
            Me.RunMode = eFormModeTypes.Stopping
        End Sub

        Private Sub OnClearSeedCells(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_tsmClearSeed.Click
            Me.m_MPAOptManager.clearSeedCells()
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnClearMPACells(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_tsmClearMPA.Click
            Me.m_MPAOptManager.clearMPAs()
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnSetAllSeedCells(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_tsmSetAllSeed.Click
            Me.m_MPAOptManager.setAllCellsToSeed(Me.GetSelectedMPA())
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnSetAllMPACells(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_tsmSetAllMPA.Click
            Me.m_MPAOptManager.setAllCellsToMPA(Me.GetSelectedMPA())
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnEditLayers(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles m_tsbEditLayers.Click

            ' Note that the command is invoked manually here because in THIS FORM only the command will be enabled when
            ' preparing Ecoseed. Yes, it's a half-ass solution while in fact the entire GUI should become aware the 
            ' running of a model by blocking out any possibility to enter/edit data.
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("EditImportanceLayers")
            If cmd IsNot Nothing Then cmd.Invoke()

        End Sub

        Private Sub OnModeEcoseed(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_rbEcoseed.CheckedChanged

            If Me.m_rbEcoseed.Checked Then
                Me.SearchType = eMPAOptimizationModels.EcoSeed
                Me.UpdateControls()
            End If

        End Sub

        Private Sub OnModeRandom(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_rbRandom.CheckedChanged

            If Me.m_rbRandom.Checked Then
                Me.SearchType = eMPAOptimizationModels.RandomSearch
                Me.UpdateControls()
            End If

        End Sub

        Private Sub OnResetMPAs(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_btnResetMPAs.Click
            Try

                Me.m_ucZoom.SuspendLayout()

                ' Set the layer
                Me.SetLayer(Me.m_MPAOptManager.OrgMPA, Me.m_basemap.LayerMPA)
                Me.m_ucZoom.Map.Refresh()

                Me.m_ucZoom.ResumeLayout()

            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnNewSearch(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_btnNewSearch.Click
            Me.RunMode = eFormModeTypes.Prepare
        End Sub

        Private Sub OnUpdateBestPercentile(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_nudBestPercentile.ValueChanged
            Me.ShowBestPercentage()
        End Sub

        Private Sub OnResultCursorPos(ByVal zgh As ZedGraphHelper, ByVal iPane As Integer, ByVal sPos As Single)
            ' Sanity checks
            If (sPos < 0 Or sPos > Me.m_MPAOptManager.Results.Count - 1) Then Return
            Me.ShowIteration()
        End Sub

        Private Sub OnApply(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnConvertToMpa.Click

            ' Sanity check
            If (Me.SearchType = eMPAOptimizationModels.RandomSearch) And (Me.m_alayerFeedback.Length = 1) Then
                Me.SetLayer(Me.m_dataRunState, Me.m_basemap.LayerMPA, Me.GetSelectedMPA())
                Me.m_ucZoom.Map.Refresh()
            End If

        End Sub

        Private Sub OnExport(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnExport.Click

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("ExportLayerData")
            Dim lyrExport As cLayer = Nothing
            Dim aiCells(,) As Integer = Nothing
            Dim iNumResults As Integer = 0

            If cmd Is Nothing Then Return

            ' Copy layer
            lyrExport = New cLayer(Me.m_alayerFeedback(0))
            aiCells = Me.m_MPAOptManager.CellSelectedMap(CInt(Me.m_nudBestPercentile.Value), iNumResults)
            lyrExport.Name = "HitCount"
            Me.SetLayer(aiCells, lyrExport.Data)

            cmd.Tag = New cLayer() {lyrExport}
            cmd.Invoke()
        End Sub

#End Region ' Controls

#Region " Search manager "

        Private Sub OnSeedCellCallback()
            Me.HandleSeedCellCallback()
        End Sub

        Private Sub OnSeedRunStateCallback(ByVal runstate As eRunStates)

            Select Case runstate

                Case eRunStates.Initializing
                    Me.RunMode = eFormModeTypes.Initializing

                Case eRunStates.Searching
                    Me.RunMode = eFormModeTypes.Searching

                Case eRunStates.Completed
                    Me.RunMode = eFormModeTypes.Results

                Case eRunStates.NewCellSelected
                    Me.HandleNewCellSelected()

                Case eRunStates.NewBestResultFound
                    Me.HandleNewBestResultFound()

            End Select

        End Sub

#End Region ' Search manager

#Region " Core "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)

            If (msg.Source = eMessageSource.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved) Then
                ' Reload data
                Me.Reload()
                ' Cascade mode down
                Me.RunMode = eFormModeTypes.Prepare
            End If

        End Sub

#End Region ' Core

#Region " Properties "

        Private m_bInUpdate As Boolean = False

        Private Sub OnSearchTypeChanged(ByVal prop As cProperty, ByVal change As cProperty.eChangeFlags)
            Debug.Assert(Object.ReferenceEquals(prop, Me.m_propSearchType))

            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            Select Case CInt(prop.GetValue())
                Case eMPAOptimizationModels.EcoSeed
                    Me.m_rbEcoseed.Checked = True
                Case eMPAOptimizationModels.RandomSearch
                    Me.m_rbRandom.Checked = True
                Case Else
                    Debug.Assert(False, String.Format("Unsupported search type selected {0}", CInt(prop.GetValue())))
            End Select
            Me.m_bInUpdate = False
        End Sub

#End Region ' Properties

#Region " Map "

        Private Sub OnLayerChanged(ByVal l As cLayer, ByVal changeFlags As cLayer.eChangeFlags)
            If ((changeFlags And cLayer.eChangeFlags.Selected) > 0) Then Me.UpdateControls()
        End Sub

#End Region ' Map

#End Region ' Events

#Region " Internals "

#Region " One-time initialization "

        Private Sub InitProgressGraph()

            Dim zgcr As New ZedGraph.ColorSymbolRotator

            ' Flush first color to make sure that the two graps (progress and output) use the same colour scheme
            Dim clrFlush As Color = zgcr.NextColor

            Me.m_zghProgress = New ZedGraphHelper(Me.m_graphProgress)

            Me.m_graphProgress.GraphPane.Legend.Position = ZedGraph.LegendPos.Right
            Me.m_graphProgress.GraphPane.Title.IsVisible = False
            Me.m_graphProgress.GraphPane.XAxis.Title.Text = "" ' Config with form mode
            Me.m_graphProgress.GraphPane.YAxis.Title.Text = "" ' Config with form mode

            ' Only show major ticks
            Me.m_graphProgress.GraphPane.XAxis.Scale.MajorStep = 5
            Me.m_graphProgress.GraphPane.XAxis.Scale.MinorStep = 1
            Me.m_graphProgress.GraphPane.YAxis.Scale.MaxAuto = True
            Me.m_graphProgress.GraphPane.YAxis.Scale.MinAuto = True

            Me.m_lptsProgress(0) = New ResultPoints()
            Me.m_graphProgress.GraphPane.AddCurve(My.Resources.HEADER_NETECONOMICVALUE, Me.m_lptsProgress(0), zgcr.NextColor, ZedGraph.SymbolType.None)
            Me.m_lptsProgress(1) = New ResultPoints()
            Me.m_graphProgress.GraphPane.AddCurve(My.Resources.FPS_VC_NET_SOCIAL_VALUE, Me.m_lptsProgress(1), zgcr.NextColor, ZedGraph.SymbolType.None)
            Me.m_lptsProgress(2) = New ResultPoints()
            Me.m_graphProgress.GraphPane.AddCurve(My.Resources.FPS_VC_NET_MANDATED_REBUILDING, Me.m_lptsProgress(2), zgcr.NextColor, ZedGraph.SymbolType.None)
            Me.m_lptsProgress(3) = New ResultPoints()
            Me.m_graphProgress.GraphPane.AddCurve(My.Resources.FPS_VC_NET_ECOSYSTEM_STRUCTURE, Me.m_lptsProgress(3), zgcr.NextColor, ZedGraph.SymbolType.None)

        End Sub

        Private Sub InitOutputGraph()

            Dim zgcr As New ZedGraph.ColorSymbolRotator

            Me.m_zghResults = New ZedGraphHelper(Me.m_graphResults)
            Me.m_zghResults.ShowCursor = True
            AddHandler Me.m_zghResults.OnCursorPos, AddressOf OnResultCursorPos

            Me.m_graphResults.GraphPane.Legend.Position = ZedGraph.LegendPos.Right
            Me.m_graphResults.GraphPane.Title.IsVisible = False
            Me.m_graphResults.GraphPane.XAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_BESTITERATIONS
            Me.m_graphResults.GraphPane.YAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_OBJECTIVEVALUE

            ' Only show major ticks
            Me.m_graphResults.GraphPane.XAxis.Scale.MajorStep = 5
            Me.m_graphResults.GraphPane.XAxis.Scale.MinorStep = 1

            Me.m_lptsResults(0) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve("Total weighted", Me.m_lptsResults(0), zgcr.NextColor, ZedGraph.SymbolType.None)
            Me.m_lptsResults(1) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.HEADER_NETECONOMICVALUE, Me.m_lptsResults(1), zgcr.NextColor, ZedGraph.SymbolType.None)
            Me.m_lptsResults(2) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.FPS_VC_NET_SOCIAL_VALUE, Me.m_lptsResults(2), zgcr.NextColor, ZedGraph.SymbolType.None)
            Me.m_lptsResults(3) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.FPS_VC_NET_MANDATED_REBUILDING, Me.m_lptsResults(3), zgcr.NextColor, ZedGraph.SymbolType.None)
            Me.m_lptsResults(4) = New ResultPoints()
            Me.m_graphResults.GraphPane.AddCurve(My.Resources.FPS_VC_NET_ECOSYSTEM_STRUCTURE, Me.m_lptsResults(4), zgcr.NextColor, ZedGraph.SymbolType.None)

        End Sub

#End Region ' One-time initialization

#Region " Run mode specific updates "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The one controller that determines what is displayed in the form.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property RunMode() As eFormModeTypes
            Get
                Return Me.m_mode
            End Get
            Set(ByVal value As eFormModeTypes)
                ' Switching?
                If value <> Me.m_mode Then
                    ' Exit previous mode
                    Me.ExitMode()
                    ' Store mode
                    Me.m_mode = value
                    ' Enter new mode
                    Me.EnterMode()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Toggle search type, only valid in <see cref="eFormModeTypes.Prepare">Prepare</see> mode.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property SearchType() As eMPAOptimizationModels
            Get
                Return DirectCast(Me.m_propSearchType.GetValue(), eMPAOptimizationModels)
            End Get
            Set(ByVal value As eMPAOptimizationModels)
                ' Only valid while preparing a run
                If (Me.RunMode <> eFormModeTypes.Prepare) Then Return

                ' Clean up
                Me.ClearRunModeFeedback()

                ' Set search type
                If (Me.m_propSearchType IsNot Nothing) Then Me.m_propSearchType.SetValue(value)

                ' Pollute again
                Me.SetRunModeFeedback()

                ' Update visible state of existing layers
                Me.ShowLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerMPASeed), _
                    SearchType = eMPAOptimizationModels.EcoSeed, SearchType = eMPAOptimizationModels.EcoSeed)
                Me.ShowLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerMPARandom), _
                    SearchType = eMPAOptimizationModels.RandomSearch, SearchType = eMPAOptimizationModels.RandomSearch)
                Me.ShowLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerImportance), _
                     SearchType = eMPAOptimizationModels.RandomSearch, SearchType = eMPAOptimizationModels.RandomSearch)

                ' Update graph labels
                Select Case SearchType
                    Case eMPAOptimizationModels.EcoSeed
                        Me.m_graphProgress.GraphPane.XAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_ECOSEED
                    Case eMPAOptimizationModels.RandomSearch
                        Me.m_graphProgress.GraphPane.XAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_RANDOMSEARCH
                End Select
                Me.m_graphProgress.Invalidate()

            End Set
        End Property

        Private Sub EnterMode()

            Select Case Me.m_mode
                Case eFormModeTypes.Prepare
                    ' User is about to start entering data

                Case eFormModeTypes.Initializing
                    ' Set running status text
                    AppLauncher.GetInstance().SetStatusText(My.Resources.STATUS_SEARCH_INITIALIZING, TriState.UseDefault, -1)
                    ' Switch to 'Results' page
                    Me.m_tcResults.SelectedIndex = 0

                Case eFormModeTypes.Searching
                    ' Set running status text
                    AppLauncher.GetInstance().SetStatusText(My.Resources.STATUS_SEARCH_SEARCHING, TriState.UseDefault, -1)

                Case eFormModeTypes.Stopping
                    ' Set running status text
                    AppLauncher.GetInstance().SetStatusText(My.Resources.STATUS_SEARCH_STOPPING, TriState.UseDefault, -1)

                Case eFormModeTypes.Results
                    ' Switch to 'Apply' page
                    Me.m_tcResults.SelectedIndex = 1

            End Select

            ' Create run state layers
            Me.SetRunModeFeedback()
            Me.UpdateControls()

        End Sub

        Private Sub ExitMode()
            Select Case Me.m_mode

                Case eFormModeTypes.Prepare ' Prepare for running mode

                Case eFormModeTypes.Searching
                    ' Cancel running status text
                    AppLauncher.GetInstance().SetStatusText("", TriState.UseDefault, 0)

                Case eFormModeTypes.Initializing
                    ' Cancel running status text
                    AppLauncher.GetInstance().SetStatusText("", TriState.UseDefault, 0)

                Case eFormModeTypes.Stopping
                    ' Cancel running status text
                    AppLauncher.GetInstance().SetStatusText("", TriState.UseDefault, 0)

                Case eFormModeTypes.Results ' Show results
                    ' Clear the graph
                    Me.ClearProgressGraph()

            End Select

            ' Remove run state layers
            Me.ClearRunModeFeedback()

        End Sub

        Private Sub Reload()
            ' Store ref
            Me.m_basemap = Me.m_core.EcospaceBasemap
            ' Initalize the m_ucBasemap
            Me.m_ucZoom.m_map.Basemap = Me.m_basemap
            Me.ReloadMap()
            Me.ReloadMPAChoices()
        End Sub

        Private Sub ReloadMap()

            Me.m_ucZoom.Map.SuspendLayout()
            Me.m_ucLayers.LockUpdates()

            Me.m_ucZoom.Map.Clear()

            Me.m_alayerSeed = Me.AddBaseLayers(eVarNameFlags.LayerMPASeed)
            Me.AddBaseLayers(eVarNameFlags.LayerMPARandom)
            Me.AddBaseLayers(eVarNameFlags.LayerImportance)
            Me.AddBaseLayers(eVarNameFlags.LayerMPA)
            Me.AddBaseLayers(eVarNameFlags.LayerHabitat)
            Me.AddBaseLayers(eVarNameFlags.LayerDepth)
            ' Hide habitat layers but show group at startup
            Me.ShowLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerHabitat), False, True)

            Me.m_ucLayers.UnlockUpdates()
            Me.m_ucZoom.Map.ResumeLayout()

        End Sub

        Private Sub ReloadMPAChoices()

            ' Get MPA optimization params to connect start MPA to
            Dim MPAOpt As cMPAOptParameters = Me.m_core.MPAOptimizationManager.MPAOptimizationParamters
            ' Create list of available MPAs
            Dim alMPAs As New List(Of cCoreInputOutputBase)

            ' Build list of MPAs
            For iMPA As Integer = 1 To Me.m_core.nMPAs
                alMPAs.Add(Me.m_core.EcospaceMPAs(iMPA))
            Next

            ' Connect MPA optimization property to MPA control
            Me.m_fpMPA = New cPropertyFormatProvider(Me.m_cmbMPA, MPAOpt, eVarNameFlags.iMPAOptToUse, Nothing, alMPAs.ToArray)

            ' Only one MPA available?
            If alMPAs.Count = 1 Then
                ' #Yes: select first MPA
                Me.m_fpMPA.Value = alMPAs(0).Index
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, called when a new seed cell has been selected.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub HandleSeedCellCallback()

            ' Sanity check
            If Not (Me.m_MPAOptManager.isRunning()) Then Return

            Dim output As cMPAOptOutput = Me.m_MPAOptManager.CurrentRowColResults

            ' Perform search specific updates
            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed
                    Try
                        ' Ecoseed: the seed cell configuration has changed. 
                        ' The seed cell map has to be updated, which is done in the GUI
                        ' Populate run state layer with current row/col results
                        For iRow As Integer = 0 To Me.m_basemap.InRow
                            For iCol As Integer = 0 To Me.m_basemap.InCol
                                Me.m_dataRunState(iRow, iCol) = cLayerFactory.cECOSEED_LAYER_NOVALUE
                            Next iCol
                        Next iRow

                        If output.CurRow > 0 And output.CurCol > 0 Then
                            Me.m_dataRunState(output.CurRow, output.CurCol) = cLayerFactory.cECOSEED_LAYER_CURRENTVALUE
                        End If
                        If output.BestRow > 0 And output.BestCol > 0 Then
                            Me.m_dataRunState(output.BestRow, output.BestCol) = cLayerFactory.cECOSEED_LAYER_BESTVALUE
                        End If

                        ' Make the map redraw itself
                        Me.m_ucZoom.Map.Refresh()
                    Catch ex As Exception

                    End Try

                    Me.m_gridProgress.LogResult(output.EconomicValue, output.SocialValue, _
                        output.MandatedValue, output.EcologicalValue, _
                        output.TotalValue, output.PercentageClosed)

                Case eMPAOptimizationModels.RandomSearch
                    ' MPA layout has changed
                    Me.m_ucZoom.Map.Refresh()

                    Me.LogProgressGraph(output.EconomicValue, output.SocialValue, output.MandatedValue, output.EcologicalValue)

                    Me.m_gridProgress.LogResult(output.EconomicValue, output.SocialValue, _
                        output.MandatedValue, output.EcologicalValue, _
                        output.TotalValue, output.PercentageClosed)

            End Select


        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, called when a new cell has been selected.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub HandleNewCellSelected()

            ' Sanity check
            Debug.Assert(Me.m_MPAOptManager.isRunning())

            Dim output As cMPAOptOutput = Me.m_MPAOptManager.CurrentRowColResults

            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed
                    ' A new MPA cell has been selected out of the seed cells
                    ' Redraw MPA map
                    Me.m_ucZoom.Map.Refresh()
                    ' Show this in the graph
                    Me.LogProgressGraph(output.EconomicValue, output.SocialValue, output.MandatedValue, output.EcologicalValue)
                    ' Always update the progress grid
                    Me.m_gridProgress.LogResult(output.EconomicValue, output.SocialValue, _
                        output.MandatedValue, output.EcologicalValue, _
                        output.TotalValue, output.PercentageClosed)

                Case eMPAOptimizationModels.RandomSearch
                    ' Does not apply to Random search

            End Select

        End Sub

        Private Sub HandleNewBestResultFound()

            Dim output As cMPAOptOutput = Me.m_MPAOptManager.CurrentRowColResults

            ' Sanity check
            Debug.Assert(Me.m_MPAOptManager.isRunning())

            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed
                    Try

                        ' Ecoseed: the seed cell configuration has changed. 
                        ' The seed cell map has to be updated, which is done in the GUI
                        ' Populate run state layer with current row/col results
                        For iRow As Integer = 0 To Me.m_basemap.InRow
                            For iCol As Integer = 0 To Me.m_basemap.InCol
                                Me.m_dataRunState(iRow, iCol) = cLayerFactory.cECOSEED_LAYER_NOVALUE
                            Next iCol
                        Next iRow

                        If output.CurRow > 0 And output.CurCol > 0 Then
                            Me.m_dataRunState(output.CurRow, output.CurCol) = cLayerFactory.cECOSEED_LAYER_CURRENTVALUE
                        End If
                        If output.BestRow > 0 And output.BestCol > 0 Then
                            Me.m_dataRunState(output.BestRow, output.BestCol) = cLayerFactory.cECOSEED_LAYER_BESTVALUE
                        End If

                        ' Make the map redraw itself
                        Me.m_ucZoom.Map.Refresh()

                    Catch ex As Exception

                    End Try

                Case eMPAOptimizationModels.RandomSearch

                    Me.LogProgressGraph(output.EconomicValue, output.SocialValue, output.MandatedValue, output.EcologicalValue)

            End Select

        End Sub

#End Region ' Run-mode specific updates

#Region " Map updating "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper function to create layer(s) for a given varname.
        ''' </summary>
        ''' <param name="varName">The core variable to load basemap data for.</param>
        ''' -------------------------------------------------------------------
        Private Function AddBaseLayers(ByVal varName As eVarNameFlags) As cLayer()

            Dim strGroup As String = cLayerFactory.GetLayerGroup(varName)
            Dim alayers As cLayer() = cLayerFactory.GetLayers(Me.m_core, varName)
            Dim l As cLayer = Nothing

            ' Add group, and collapse and hide habitat layers
            Me.m_ucLayers.AddGroup(strGroup, varName <> eVarNameFlags.LayerHabitat)

            ' Add individual layers
            For iLayer As Integer = 0 To alayers.Length - 1
                l = alayers(iLayer)
                ' Add the layer to the control(s)
                Me.AddLayer(l, strGroup)
            Next

            Return alayers

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper function to create the standard core layers.  
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function SetRunModeFeedback() As Boolean

            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed

                    Select Case Me.RunMode
                        Case eFormModeTypes.Prepare, eFormModeTypes.Searching, eFormModeTypes.Initializing
                            Try

                                Dim strGroup As String = ""
                                Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
                                Dim layWrapped As cEcospaceIntegerNxNLayer
                                Dim l As cLayer = Nothing
                                Dim alayers As cLayer() = Nothing
                                Dim lRunStateLayers As New List(Of cLayer)

                                Me.m_ucLayers.LockUpdates()

                                ' Redim data
                                ReDim Me.m_dataRunState(Me.m_basemap.InRow, Me.m_basemap.InCol)

                                ' AFTER redimming create a temp wrapper layer
                                layWrapped = New cEcospaceIntegerNxNLayer(Me.m_core, Me.m_dataRunState, bm.InRow, bm.InCol, bm.CellLength, bm.Latitude, bm.Longitude)

                                strGroup = cLayerFactory.GetLayerGroup(eVarNameFlags.LayerMPASeed)
                                ' Create current cell layer(s)
                                alayers = cLayerFactory.GetLayers(Me.m_core, eVarNameFlags.LayerMPASeedCurrent, layWrapped)
                                For iLayer As Integer = 0 To alayers.Length - 1
                                    l = alayers(iLayer)
                                    l.Editor.IsReadOnly = True
                                    Me.AddLayer(l, strGroup, Me.m_alayerSeed(0))
                                Next
                                lRunStateLayers.AddRange(alayers)

                                ' Create best cell layer
                                alayers = cLayerFactory.GetLayers(Me.m_core, eVarNameFlags.LayerMPASeedBest, layWrapped)
                                For iLayer As Integer = 0 To alayers.Length - 1
                                    l = alayers(iLayer)
                                    l.Editor.IsReadOnly = True
                                    Me.AddLayer(l, strGroup, Me.m_alayerSeed(0))
                                Next
                                lRunStateLayers.AddRange(alayers)
                                Me.m_alayerFeedback = lRunStateLayers.ToArray()
                                Me.m_ucLayers.UnlockUpdates()

                            Catch ex As Exception

                            End Try

                        Case eFormModeTypes.Results
                    End Select

                Case eMPAOptimizationModels.RandomSearch

                    Select Case Me.RunMode

                        Case eFormModeTypes.Results
                            Try
                                ' Create random output layer
                                Dim strGroup As String = ""
                                Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
                                Dim layWrapped As cEcospaceIntegerNxNLayer
                                Dim l As cLayer = Nothing
                                Dim alayers As cLayer() = Nothing
                                Dim lRunStateLayers As New List(Of cLayer)

                                Me.m_ucLayers.LockUpdates()

                                ' Redim data
                                ReDim Me.m_dataRunState(Me.m_basemap.InRow, Me.m_basemap.InCol)

                                ' AFTER redimming create a temp wrapper layer
                                layWrapped = New cEcospaceIntegerNxNLayer(Me.m_core, Me.m_dataRunState, bm.InRow, bm.InCol, bm.CellLength, bm.Latitude, bm.Longitude)

                                strGroup = cLayerFactory.GetLayerGroup(eVarNameFlags.LayerMPARandom)

                                ' Create current cell layer(s)
                                alayers = cLayerFactory.GetLayers(Me.m_core, eVarNameFlags.LayerMPARandom, layWrapped)
                                For iLayer As Integer = 0 To alayers.Length - 1
                                    l = alayers(iLayer)
                                    l.Editor.IsReadOnly = True
                                    Me.AddLayer(l, strGroup)
                                Next
                                lRunStateLayers.AddRange(alayers)

                                Me.m_alayerFeedback = lRunStateLayers.ToArray()
                                Me.m_ucLayers.UnlockUpdates()

                            Catch ex As Exception

                            End Try

                    End Select
            End Select

            Try
                ' Fill output graph
                Dim lResults As List(Of cObjectiveResult) = Me.m_MPAOptManager.Results
                For iResult As Integer = 0 To lResults.Count - 1
                    Dim result As cObjectiveResult = lResults(iResult)
                    Me.m_lptsResults(0).AddItem(result.objFuncTotal)
                    Me.m_lptsResults(1).AddItem(result.objFuncEconomicValue)
                    Me.m_lptsResults(2).AddItem(result.objFuncSocialValue)
                    Me.m_lptsResults(3).AddItem(result.objFuncMandatedValue)
                    Me.m_lptsResults(4).AddItem(result.objFuncEcologicalValue)
                Next
                Me.m_graphResults.GraphPane.XAxis.Scale.Max = lResults.Count - 1
                Me.m_graphResults.GraphPane.YAxis.Scale.MaxAuto = True
                Me.m_zghResults.CursorPos = 0.0
                Me.m_graphResults.Invalidate()

            Catch ex As Exception

            End Try

            Return True

        End Function

        Private Function ClearRunModeFeedback() As Boolean

            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed
                    If Me.m_alayerFeedback IsNot Nothing Then
                        For Each l As cLayer In Me.m_alayerFeedback
                            Me.RemoveLayer(l)
                        Next

                        Me.m_alayerFeedback = Nothing
                    End If
                    Me.m_dataRunState = Nothing

                Case eMPAOptimizationModels.RandomSearch

                    Select Case Me.RunMode
                        Case eFormModeTypes.Results
                            If Me.m_alayerFeedback IsNot Nothing Then
                                For Each l As cLayer In Me.m_alayerFeedback
                                    Me.RemoveLayer(l)
                                Next

                                Me.m_alayerFeedback = Nothing
                            End If
                            Me.m_dataRunState = Nothing

                    End Select

            End Select

            ' Clear results
            For i As Integer = 0 To 4
                Me.m_lptsResults(i).Clear()
            Next

            Return True

        End Function

#End Region ' Map updating

#Region " Progress "

        Private Sub LogProgressGraph(ByVal sEconomicValue As Single, ByVal sSocialValue As Single, ByVal sMandatedValue As Single, ByVal sEcologicalValue As Single)

            ' Show this in the graph
            Dim iXMax As Integer = 0

            ' All 0: do not log
            If (sEconomicValue + sSocialValue + sMandatedValue + sEcologicalValue) = 0.0 Then Return

            For iResult As Integer = 0 To Me.m_lptsProgress.Length - 1
                Dim rp As ResultPoints = Me.m_lptsProgress(iResult)
                Select Case iResult
                    Case 0 : rp.AddItem(sEconomicValue)
                    Case 1 : rp.AddItem(sSocialValue)
                    Case 2 : rp.AddItem(sMandatedValue)
                    Case 3 : rp.AddItem(sEcologicalValue)
                End Select
                iXMax = Math.Max(iXMax, rp.Count)
            Next

            Me.m_graphProgress.GraphPane.XAxis.Scale.Max = iXMax
            Me.m_graphProgress.Invalidate()

        End Sub

#End Region ' Progress

#Region " Results "

        Private Sub ShowIteration()

            Dim aiCells(Me.m_basemap.InRow, Me.m_basemap.InCol) As Integer

            ' Update map
            ReDim aiCells(Me.m_basemap.InRow, Me.m_basemap.InCol)

            Dim res As cObjectiveResult = Me.m_MPAOptManager.Results(CInt(Math.Round(Me.m_zghResults.CursorPos)))
            For iCell As Integer = 0 To res.Cells.Count - 1
                Dim cell As cMPACell = res.Cells(iCell)
                aiCells(cell.Row, cell.Col) = cell.iMPA
            Next iCell

            ' Update indicators
            Me.m_gridResults.LogResult(res.objFuncEconomicValue, res.objFuncSocialValue, res.objFuncMandatedValue, res.objFuncEcologicalValue, res.objFuncTotal, res.PercentageClosed)
            Me.SetLayer(aiCells, Me.m_basemap.LayerMPA, Me.GetSelectedMPA())
            Me.m_ucZoom.Map.Refresh()

        End Sub

        Private Sub ShowBestPercentage()

            If (Me.SearchType = eMPAOptimizationModels.RandomSearch) And (Me.m_alayerFeedback.Length = 1) Then
                ' Update map
                Dim iNumResults As Integer = 0
                Dim aiCells(,) As Integer = Me.m_MPAOptManager.CellSelectedMap(CInt(Me.m_nudBestPercentile.Value), iNumResults)

                Me.SetLayer(aiCells, Me.m_alayerFeedback(0).Data)
                Me.m_alayerFeedback(0).Update(cLayer.eChangeFlags.Map)
            End If

        End Sub

#End Region ' Results

#Region " Helper methods "

#Region " Map "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a layer to the map.
        ''' </summary>
        ''' <param name="l">Layer to add.</param>
        ''' <param name="strGroup">Group to add the layer to.</param>
        ''' <param name="layerPosition">Layer to position this layer before, if any.</param>
        ''' -------------------------------------------------------------------
        Private Sub AddLayer(ByVal l As cLayer, ByVal strGroup As String, Optional ByVal layerPosition As cLayer = Nothing)
            Me.m_lLayers.Add(l)
            Me.m_ucZoom.Map.AddLayer(l, layerPosition)
            Me.m_ucLayers.AddLayer(l, strGroup, layerPosition)
            AddHandler l.LayerChanged, AddressOf OnLayerChanged
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remmove a layer from the map.
        ''' </summary>
        ''' <param name="l">Layer to remove.</param>
        ''' -------------------------------------------------------------------
        Private Sub RemoveLayer(ByVal l As cLayer)
            Me.m_lLayers.Remove(l)
            Me.m_ucZoom.Map.RemoveLayer(l)
            Me.m_ucLayers.RemoveLayer(l)
            RemoveHandler l.LayerChanged, AddressOf OnLayerChanged
        End Sub

        Private Sub ShowLayerGroup(ByVal strGroup As String, ByVal bShowLayers As Boolean, ByVal bShowGroup As Boolean)
            Me.m_ucLayers.ShowGroup(strGroup, bShowLayers, bShowGroup)
        End Sub

        Private Sub EnableLayerGroup(ByVal strGroup As String, ByVal bEditable As Boolean)
            Me.m_ucLayers.EnableGroup(strGroup, bEditable)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets a layer to a grid of values.
        ''' </summary>
        ''' <param name="src">NxN array of integer to copy from.</param>
        ''' <param name="lDest">Layer to copy to.</param>
        ''' <param name="iConvertTo">Variable to convert non-negative values
        ''' to, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> to 
        ''' directly copy the values.</param>
        ''' -------------------------------------------------------------------
        Private Sub SetLayer(ByVal src As Integer(,), ByVal lDest As cEcospaceLayer, _
            Optional ByVal iConvertTo As Integer = cCore.NULL_VALUE)

            Dim iValue As Integer = 0
            ' For all rows
            For iRow As Integer = 1 To Me.m_basemap.InRow
                ' For all cols
                For iCol As Integer = 1 To Me.m_basemap.InCol
                    ' Get value
                    iValue = src(iRow, iCol)
                    ' Must convert?
                    If iConvertTo <> cCore.NULL_VALUE Then
                        ' #Yes: transmogrify non-zero values
                        iValue = CInt(IIf(iValue = 0, iValue, iConvertTo))
                    End If
                    ' Apply!
                    lDest.Cell(iRow, iCol) = iValue
                Next iCol
            Next iRow

            ' Invalidate min/max
            lDest.Invalidate()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets a layer to a grid of values.
        ''' </summary>
        ''' <param name="src">NxN array of single to copy from.</param>
        ''' <param name="lDest">Layer to copy to.</param>
        ''' <param name="iConvertTo">Variable to convert non-negative values
        ''' to, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> to 
        ''' directly copy the values.</param>
        ''' -------------------------------------------------------------------
        Private Sub SetLayer(ByVal src As Single(,), ByVal lDest As cEcospaceLayer, _
            Optional ByVal iConvertTo As Integer = cCore.NULL_VALUE)

            Dim sValue As Single = 0
            ' For all rows
            For iRow As Integer = 1 To Me.m_basemap.InRow
                ' For all cols
                For iCol As Integer = 1 To Me.m_basemap.InCol
                    ' Get value
                    sValue = src(iRow, iCol)
                    ' Must convert?
                    If iConvertTo <> cCore.NULL_VALUE Then
                        ' #Yes: ognotrizarp non-zero values
                        sValue = CInt(IIf(sValue = 0, sValue, iConvertTo))
                    End If
                    ' Apply!
                    lDest.Cell(iRow, iCol) = sValue
                Next iCol
            Next iRow

        End Sub

#End Region ' Map

#Region " Generic "

        Private Sub UpdateControls()

            ' The %^@#$^#@$ check boxes throw events even before the form OnLoad has been called. Nice.
            ' Added sanity check to prevent premature control handling
            If (Object.ReferenceEquals(Me.m_MPAOptManager, Nothing)) Then Return

            Dim bIsPreparing As Boolean = (Me.RunMode = eFormModeTypes.Prepare)
            Dim bIsRunning As Boolean = (Me.RunMode = eFormModeTypes.Searching Or Me.RunMode = eFormModeTypes.Initializing Or Me.RunMode = eFormModeTypes.Stopping)
            Dim bIsResults As Boolean = (Me.RunMode = eFormModeTypes.Results)
            Dim bIsEcoseed As Boolean = (Me.SearchType = eMPAOptimizationModels.EcoSeed)
            Dim bIsRandom As Boolean = (Me.SearchType = eMPAOptimizationModels.RandomSearch)
            Dim bMPALayerSelected As Boolean = (Me.GetSelectedMPA() > 0)

            ' Update input controls
            Me.m_nudStartYear.Enabled = bIsPreparing
            Me.m_lblStartYear.Enabled = bIsPreparing
            Me.m_nudEndYear.Enabled = bIsPreparing
            Me.m_lblEndYear.Enabled = bIsPreparing
            Me.m_nudMinArea.Enabled = (bIsPreparing And bIsRandom)
            Me.m_lblMinArea.Enabled = (bIsPreparing And bIsRandom)
            Me.m_nudMaxArea.Enabled = (bIsPreparing And bIsRandom)
            Me.m_lblMaxArea.Enabled = (bIsPreparing And bIsRandom)
            Me.m_nudStep.Enabled = (bIsPreparing And bIsRandom)
            Me.m_lblStep.Enabled = (bIsPreparing And bIsRandom)
            Me.m_nudIterations.Enabled = (bIsPreparing And bIsRandom)
            Me.m_lblIterations.Enabled = (bIsPreparing And bIsRandom)
            Me.m_nudBoundaryWeight.Enabled = (bIsPreparing)
            Me.m_lblBoundaryWeight.Enabled = (bIsPreparing)
            Me.m_ucWeightOPGrid.Enabled = (bIsPreparing)
            Me.m_ucFleetOPGrid.Enabled = (bIsPreparing)
            Me.m_ucGroupOPGrid.Enabled = (bIsPreparing)
            Me.m_rbEcoseed.Enabled = (bIsPreparing)
            Me.m_rbRandom.Enabled = (bIsPreparing)
            Me.m_lbMPA.Enabled = (bIsPreparing)
            Me.m_cmbMPA.Enabled = (bIsPreparing)
            ' Results
            Me.m_graphResults.Enabled = bIsResults
            Me.m_nudBestPercentile.Enabled = (bIsResults And bIsRandom)
            Me.m_btnResetMPAs.Enabled = bIsResults

            ' Run buttons
            Me.m_btnNewSearch.Enabled = bIsResults
            Me.m_btnConvertToMpa.Enabled = bIsResults
            Me.m_btnExport.Enabled = (bIsResults And bIsRandom)

            ' Update run control buttons
            Me.m_btnRun.Enabled = (Not bIsRunning)
            Me.m_btnStop.Enabled = bIsRunning

            ' Toggle toolbar controls
            Me.m_tsbMPA.Enabled = bIsPreparing And bMPALayerSelected
            Me.m_tsbSeed.Enabled = bIsPreparing And bMPALayerSelected And bIsEcoseed
            Me.m_tsbEditLayers.Enabled = bIsPreparing And bIsRandom

            ' Layers enabled state
            Me.EnableLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerDepth), Not bIsRunning)
            Me.EnableLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerMPA), Not bIsRunning)
            Me.EnableLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerHabitat), Not bIsRunning)
            Me.EnableLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerMPASeed), Not bIsRunning)
            Me.EnableLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerMPARandom), Not bIsRunning)
            Me.EnableLayerGroup(cLayerFactory.GetLayerGroup(eVarNameFlags.LayerImportance), Not bIsRunning)

            ' Update map
            Me.m_ucZoom.Map.Editable = bIsPreparing

        End Sub

        Private Function ValidateInputs() As Boolean

            Dim source As cCoreInputOutputBase = Me.m_MPAOptManager.ValueWeights
            Dim bOk As Boolean = True

            ' Check MPA selection
            If Me.m_cmbMPA.SelectedIndex = -1 Then
                ' ToDo_JS: Globalize this
                Me.m_core.Messages.SendMessage(New cMessage("MPA selection required", eMessageType.Any, eMessageSource.MPAOptimization, eMessageImportance.Warning))
                Return False
            End If

            ' Check mandated rebuilding
            If CSng(source.GetVariable(eVarNameFlags.FPSMandatedRebuildingWeight)) > 0.0 Then
                bOk = False
                For iGroup As Integer = 1 To Me.m_core.nGroups
                    source = Me.m_MPAOptManager.GroupObjectives(iGroup)
                    bOk = bOk Or (CSng(source.GetVariable(eVarNameFlags.FPSGroupMandRelBiom)) > 0.0)
                Next
                If bOk = False Then
                    ' ToDo_JS: Globalize this
                    Me.m_core.Messages.SendMessage(New cMessage("No mandated biomasses specified", eMessageType.Any, eMessageSource.MPAOptimization, eMessageImportance.Warning))
                    Return False
                End If
            End If

            '' At least one objective weight should exceed 0
            'If CSng(source.GetVariable(eVarNameFlags.FPSEconomicWeight)) = 0.0 And _
            '   CSng(source.GetVariable(eVarNameFlags.FPSSocialWeight)) = 0.0 And _
            '   CSng(source.GetVariable(eVarNameFlags.FPSMandatedRebuildingWeight)) = 0.0 And _
            '   CSng(source.GetVariable(eVarNameFlags.FPSEcoSystemWeight)) = 0.0 Then
            '    ' ToDo_JS: Globalize this
            '    Me.m_core.Messages.SendMessage(New cMessage("All objective weights are 0, there is nothing to search.", eMessageType.Any, eMessageSource.MPAOptimization, eMessageImportance.Warning))
            '    Return False
            'End If

            Return True
        End Function

        Private Function GetSelectedMPA() As Integer
            Return CInt(Me.m_fpMPA.Value())
        End Function

        Private Sub ClearProgressGraph()
            For Each rp As ResultPoints In Me.m_lptsProgress
                rp.Clear()
            Next
            Me.m_graphProgress.Refresh()
        End Sub

#End Region ' Generic

#End Region ' Helper methods

#End Region ' Internals

    End Class

End Namespace