' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Utilities
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ZedGraph
Imports SharedResources = ScientificInterfaceShared.My.Resources

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

            Default Public ReadOnly Property Item(index As Integer) As ZedGraph.PointPair Implements ZedGraph.IPointList.Item
                Get
                    Return Me.m_list(index)
                End Get
            End Property

            Public Sub AddItem(sValue As Single)
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

        ' == Data ==

        Private m_manager As cMPAOptManager = Nothing
        Private m_basemap As cEcospaceBasemap = Nothing

        ' == Layer cache ==

        ''' <summary>All layers in the basemap.</summary>
        Private m_layers As New List(Of cDisplayLayer)
        ''' <summary>All layers that reflect search progress.</summary>
        ''' <remarks>The data for these layers orginates from the core.</remarks>
        Private m_feedbackLayers() As cDisplayLayerRaster = Nothing
        Private m_ecoseedLayer As cDisplayLayerRaster = Nothing
        Private m_mpaLayers() As cDisplayLayerRaster = Nothing
        ''' <summary>Map data to provide feedback to the user.</summary>
        Private m_mapFeedback As Integer(,) = Nothing

        ' == Parameter IO ==

        Private m_fpStartYear As cEwEFormatProvider = Nothing
        Private m_fpEndYear As cEwEFormatProvider = Nothing
        Private m_fpMinArea As cEwEFormatProvider = Nothing
        Private m_fpMaxArea As cEwEFormatProvider = Nothing
        Private m_fpStepSize As cEwEFormatProvider = Nothing
        Private m_fpRegions As cEwEFormatProvider = Nothing
        Private m_fpIterations As cEwEFormatProvider = Nothing
        Private m_fpBestPercentile As cEwEFormatProvider = Nothing
        Private m_fpMPA As cEwEFormatProvider = Nothing
        Private m_propSearchType As cIntegerProperty = Nothing
        Private m_fpDiscRate As cPropertyFormatProvider = Nothing
        Private m_fpGenDiscRate As cPropertyFormatProvider = Nothing
        Private m_fpBaseYear As cPropertyFormatProvider = Nothing

        ' == UI components ==

        ''' <summary>The one and only control that provides the layers interface.</summary>
        Private m_ucLayers As ucLayersControl = Nothing

        ''' <summary>Progress graph helper.</summary>
        Private m_zghProgress As cZedGraphHelper = Nothing
        ''' <summary>Progress graph data.</summary>
        Private m_progress(5) As ResultPoints

        ''' <summary>Results graph helper.</summary>
        Private m_zghResults As cZedGraphHelper = Nothing
        ''' <summary>Results graph data.</summary>
        Private m_results(6) As ResultPoints

        ''' <summary>The mode that this form is in.</summary>
        Private m_mode As eFormModeTypes = eFormModeTypes.Prepare

        ''' <summary>The layer currently selected by the user.</summary>
        Private m_layerSelected As cDisplayLayer = Nothing
        ''' <summary>The editor belonging to the selected layer, if any.</summary>
        Private m_editorGUISelected As ucLayerEditor = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Constructor

#Region " Events "

#Region " Form "

        Public Overrides ReadOnly Property IsRunForm As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim SpaceOpt As cCoreInputOutputBase = Me.UIContext.Core.EcospaceModelParameters
            Dim MPAOpt As cMPAOptParameters = Nothing

            Me.m_manager = Me.UIContext.Core.MPAOptimizationManager
            Me.m_manager.Connect(Me, AddressOf Me.OnHandleSeedCellCallback, AddressOf Me.OnRunStateChanged)

            MPAOpt = Me.m_manager.MPAOptimizationParameters

            ' Add LayersControl
            Me.m_ucLayers = New ucLayersControl()
            Me.m_ucLayers.UIContext = Me.UIContext
            Me.m_ucLayers.Dock = DockStyle.Fill
            Me.m_plLayers.Controls.Add(Me.m_ucLayers)

            ' Configure objective grids
            Me.m_gridObjectives.ShowMPAOptParams = True
            Me.m_gridObjectives.Manager = Me.m_manager
            Me.m_gridObjectives.UIContext = Me.UIContext

            Me.m_gridFleet.Manager = Me.m_manager
            Me.m_gridFleet.UIContext = Me.UIContext

            Me.m_gridGroup.Manager = Me.m_manager
            Me.m_gridGroup.UIContext = Me.UIContext

            Me.m_gridProgress.UIContext = Me.UIContext
            Me.m_gridResults.UIContext = Me.UIContext

            ' Configure map
            Me.m_ucZoom.UIContext = Me.UIContext

            Me.m_propSearchType = New cIntegerProperty(MPAOpt, eVarNameFlags.MPAOptSearchType)
            AddHandler Me.m_propSearchType.PropertyChanged, AddressOf Me.OnSearchTypeChanged

            ' Connect to controls
            Me.m_fpStartYear = New cPropertyFormatProvider(Me.UIContext, Me.m_nudStartYear, MPAOpt, eVarNameFlags.MPAOptStartYear)
            Me.m_fpEndYear = New cPropertyFormatProvider(Me.UIContext, Me.m_nudEndYear, MPAOpt, eVarNameFlags.MPAOptEndYear)
            Me.m_fpBaseYear = New cPropertyFormatProvider(Me.UIContext, Me.m_nudBaseYear, Me.m_manager.ObjectiveParameters, eVarNameFlags.SearchBaseYear)
            'Me.m_fpStartYear.Value = Math.Max(CSng(Me.m_fpStartYear.Value), 3)
            'Me.m_fpEndYear.Value = Math.Max(CSng(Me.m_fpEndYear.Value), 5)

            Me.m_fpMinArea = New cPropertyFormatProvider(Me.UIContext, Me.m_nudMinArea, MPAOpt, eVarNameFlags.MPAOptMinArea)
            Me.m_fpMaxArea = New cPropertyFormatProvider(Me.UIContext, Me.m_nudMaxArea, MPAOpt, eVarNameFlags.MPAOptMaxArea)
            Me.m_fpStepSize = New cPropertyFormatProvider(Me.UIContext, Me.m_nudStep, MPAOpt, eVarNameFlags.MPAOptStepSize)
            Me.m_fpIterations = New cPropertyFormatProvider(Me.UIContext, Me.m_nudIterations, MPAOpt, eVarNameFlags.MPAOptIterations)
            Me.m_fpBestPercentile = New cEwEFormatProvider(Me.UIContext, Me.m_nudBestPercentile, GetType(Single))
            Me.m_fpDiscRate = New cPropertyFormatProvider(Me.UIContext, Me.m_nudDiscRate, Me.m_manager.ObjectiveParameters, eVarNameFlags.SearchDiscountRate)
            Me.m_fpGenDiscRate = New cPropertyFormatProvider(Me.UIContext, Me.m_nudGenDiscRate, Me.m_manager.ObjectiveParameters, eVarNameFlags.SearchGenDiscRate)
            Me.m_fpMPA = New cPropertyFormatProvider(Me.UIContext, Me.m_cmbMPA, MPAOpt, eVarNameFlags.iMPAOptToUse)
            Me.m_fpRegions = New cPropertyFormatProvider(Me.UIContext, Me.m_cbUseRegions, MPAOpt, eVarNameFlags.MPAOptUseRegions)

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Ecospace, eCoreComponentType.Core}

            Me.m_plEditor.Visible = False

            ' -- Sponsors --
            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
            cmd.AddControl(Me.m_pbLenfest, New Object() {"http://www.lenfestocean.org/"})
            cmd.AddControl(Me.m_pbDuke, New Object() {"http://mgel.env.duke.edu/"})

            ' Configure graphs
            Me.InitProgressGraph()
            Me.InitOutputGraph()

            Me.ReloadMPAChoices()

            ' Kick off
            Me.Reload()
            Me.OnSearchTypeChanged(Me.m_propSearchType, cProperty.eChangeFlags.All)

            ' Respond to current run state
            Me.OnRunStateChanged(Me.m_manager.RunState)

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            Me.SelectedLayer = Nothing
            Me.m_manager.Disconnect()

            ' Terminate any run state feedback
            Me.ExitMode()

            ' -- Sponsors --
            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
            cmd.RemoveControl(Me.m_pbLenfest)
            cmd.RemoveControl(Me.m_pbDuke)

            Dim alays As cDisplayLayer() = Me.m_layers.ToArray

            For Each l As cDisplayLayer In alays
                Me.RemoveLayer(l)
            Next
            Me.m_layers = Nothing

            RemoveHandler Me.m_zghResults.OnCursorPos, AddressOf Me.OnResultCursorPos
            Me.m_zghResults.Detach()
            Me.m_zghProgress.Detach()

            RemoveHandler Me.m_propSearchType.PropertyChanged, AddressOf Me.OnSearchTypeChanged
            Me.m_propSearchType = Nothing

            Me.CoreComponents = Nothing

            Me.m_fpBaseYear.Release()
            Me.m_fpBestPercentile.Release()
            Me.m_fpDiscRate.Release()
            Me.m_fpEndYear.Release()
            Me.m_fpGenDiscRate.Release()
            Me.m_fpIterations.Release()
            Me.m_fpMaxArea.Release()
            Me.m_fpMinArea.Release()
            Me.m_fpMPA.Release()
            Me.m_fpStartYear.Release()
            Me.m_fpStepSize.Release()

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form

#Region " Controls "

        Private Sub OnRun(sender As System.Object, e As System.EventArgs) _
                Handles m_btnRun.Click

            ' Abort if not all inputs valid
            If Not Me.ValidateInputs Then Return
            ' Start run
            Me.m_manager.Run()
        End Sub

        Private Sub OnStop(sender As System.Object, e As System.EventArgs) _
                Handles m_btnStop.Click

            Me.RunMode = eFormModeTypes.Stopping
            Me.m_manager.StopRun()

        End Sub

        Private Sub OnClearSeedCells(sender As System.Object, e As System.EventArgs) _
                Handles m_tsmClearSeed.Click
            Me.m_manager.clearSeedCells()
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnClearMPACells(sender As System.Object, e As System.EventArgs) _
                Handles m_tsmClearMPA.Click
            Me.m_manager.clearMPAs()
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnSetAllSeedCells(sender As System.Object, e As System.EventArgs) _
                Handles m_tsmSetAllSeed.Click
            Me.m_manager.setAllCellsToSeed(Me.SelectedMPA())
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnSetAllMPACells(sender As System.Object, e As System.EventArgs) _
                Handles m_tsmSetAllMPA.Click
            Me.m_manager.setAllCellsToMPA(Me.SelectedMPA())
            ' Re-render the map
            Me.m_ucZoom.Map.Refresh()
        End Sub

        Private Sub OnEditLayers(sender As Object, e As System.EventArgs) _
                Handles m_tsbEditLayers.Click

            ' Note that the command is invoked manually here because in THIS FORM only the command will be enabled when
            ' preparing Ecoseed. Yes, it's a half-ass solution while in fact the entire GUI should become aware the 
            ' running of a model by blocking out any possibility to enter/edit data.
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand("EditImportanceMaps")
            If cmd IsNot Nothing Then cmd.Invoke()

        End Sub

        Private Sub OnModeEcoseed(sender As System.Object, e As System.EventArgs) _
                Handles m_rbEcoseed.CheckedChanged

            If Me.m_rbEcoseed.Checked Then
                Me.SearchType = eMPAOptimizationModels.EcoSeed
                Me.UpdateControls()
            End If

        End Sub

        Private Sub OnModeRandom(sender As System.Object, e As System.EventArgs) _
                Handles m_rbRandom.CheckedChanged

            If Me.m_rbRandom.Checked Then
                Me.SearchType = eMPAOptimizationModels.RandomSearch
                Me.UpdateControls()
            End If

        End Sub

        Private Sub OnResetMPAs(sender As System.Object, e As System.EventArgs) _
                Handles m_btnResetMPAs.Click

            Me.m_ucZoom.SuspendLayout()

            Try
                ' Set the layer
                For i As Integer = 1 To Me.Core.nMPAs
                    Me.SetLayer(Me.m_manager.OrgMPA(i), Me.m_basemap.LayerMPA(i))
                Next i

                ' Update MPAs (JS: is this necessary?)
                For Each l As cDisplayLayer In Me.m_mpaLayers
                    l.Update(cDisplayLayer.eChangeFlags.Map)
                Next
            Catch ex As Exception
                ' NOP
            End Try
            Me.m_ucZoom.ResumeLayout()

        End Sub

        Private Sub OnReset(sender As System.Object, e As System.EventArgs)


            Me.RunMode = eFormModeTypes.Prepare

        End Sub

        Private Sub OnSelectAreaClosed(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbAreaClosed.SelectedIndexChanged

            If (Me.m_propSearchType Is Nothing) Then Return
            Try
                Me.UpdateBestCountMap()
                Me.UpdateResultsGraph()
            Catch ex As Exception
                ' nop
            End Try

        End Sub

        Private Sub OnBestPercentileChanged(sender As System.Object, e As System.EventArgs) _
                Handles m_nudBestPercentile.ValueChanged

            If (Me.m_propSearchType Is Nothing) Then Return

            Try
                Me.ShowBestPercentage()
                Me.UpdateResultsGraph()
            Catch ex As Exception
                ' NOP
            End Try

        End Sub

        ''' <summary>
        ''' Event handler, responds to the user exploring the progress graph.
        ''' </summary>
        Private Sub OnResultCursorPos(zgh As cZedGraphHelper, iPane As Integer, sPos As Single)
            Try
                Me.ShowIteration(CInt(Math.Round(Me.m_zghResults.CursorPos)))
            Catch ex As Exception
                ' NOP
            End Try
        End Sub

        Private Sub OnConvertToMPA(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConvertToMpa.Click

            Dim map As Integer(,) = Nothing
            Dim iNumResults As Integer = 0

            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed
                    Me.SetLayer(Me.m_mapFeedback, Me.m_basemap.LayerMPA(Me.SelectedMPA()), Me.SelectedMPA())

                Case eMPAOptimizationModels.RandomSearch
                    Me.m_manager.ConvertResultsToMPA(Me.SelectedBestPercentile(), Me.SelectedClosedPercentage())

            End Select

            ' Refresh the MPA layer that has been affected
            For Each l As cDisplayLayer In Me.m_mpaLayers
                'If l.Data.Index = Me.SelectedMPA Then
                l.Update(cDisplayLayer.eChangeFlags.Map)
                ' End If
            Next

            Me.InvaldiateMap()

        End Sub

        Private Sub OnSave(sender As System.Object, e As System.EventArgs) _
            Handles m_btnSave.Click

            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand("ExportLayerData")
            Dim lLayers As New List(Of cDisplayLayer)
            Dim layerTmp As cDisplayLayer = Nothing
            Dim ldataTmp As cEcospaceLayerInteger = Nothing
            Dim iAreaClosed As Integer = 0
            Dim iNumResults As Integer = 0

            If cmd Is Nothing Then Return

            ' Conjure a best 100% layer for every AreaPercClosed level
            For iLevel As Integer = 0 To Me.m_cmbAreaClosed.Items.Count - 1

                ' Get area closed
                iAreaClosed = CInt(Me.m_cmbAreaClosed.Items(iLevel))
                ' Wrap this in a core map layer to handle projections
                ldataTmp = New cEcospaceLayerInteger(Me.UIContext.Core,
                                                     Me.m_manager.CellSelectedMap(100, iAreaClosed, iNumResults),
                                                     cStringUtils.Localize(My.Resources.ECOSPACE_LAYER_MPABESTCOUNT, iAreaClosed))
                ' Wrap THIS in turn in a GUI layer, required by the exporter
                layerTmp = New cDisplayLayerRaster(Me.UIContext, ldataTmp, Nothing, Nothing)
                ' Add the layer to the stash to save
                lLayers.Add(layerTmp)

            Next iLevel

            cmd.Tag = lLayers.ToArray()
            cmd.Invoke()

        End Sub

        Private Sub OnAutoSaveOutputChecked(sender As System.Object, e As System.EventArgs) _
            Handles m_cbAutoSave.CheckedChanged, m_cbUseRegions.CheckedChanged
            Try
                Me.Core.Autosave(eAutosaveTypes.MPAOpt) = Me.m_cbAutoSave.Checked
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Controls

#Region " Search manager "

        Private Sub OnRunStateChanged(runstate As cMPAOptManager.eRunStates)

            Try
                Select Case runstate

                    Case cMPAOptManager.eRunStates.Initializing
                        Me.RunMode = eFormModeTypes.Initializing

                    Case cMPAOptManager.eRunStates.Searching
                        Me.RunMode = eFormModeTypes.Searching

                    Case cMPAOptManager.eRunStates.Completed
                        Me.RunMode = eFormModeTypes.Results

                    Case cMPAOptManager.eRunStates.NewCellSelected
                        Me.HandleNewCellSelected()

                    Case cMPAOptManager.eRunStates.NewBestResultFound
                        Me.HandleNewBestResultFound()

                End Select
            Catch ex As Exception
                ' Protect calling process from potential UI madness
            End Try

        End Sub

#End Region ' Search manager

#Region " Core "

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)

            Select Case msg.Source

                Case eCoreComponentType.Ecospace
                    If (Me.RunMode = eFormModeTypes.Searching Or Me.RunMode = eFormModeTypes.Stopping) Then
                        Return
                    End If

                    If (msg.Type = eMessageType.DataAddedOrRemoved) Then
                        ' Reload data
                        Me.Reload()
                        ' Cascade mode down
                        Me.RunMode = eFormModeTypes.Prepare
                    End If

                Case eCoreComponentType.Core
                    If (msg.Type = eMessageType.GlobalSettingsChanged) Then
                        Me.UpdateControls()
                    End If

            End Select

        End Sub

#End Region ' Core

#Region " Properties "

        Private m_bInUpdate As Boolean = False

        Private Sub OnSearchTypeChanged(prop As cProperty, change As cProperty.eChangeFlags)
            Debug.Assert(Object.ReferenceEquals(prop, Me.m_propSearchType))

            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            Select Case CInt(prop.GetValue())
                Case eMPAOptimizationModels.EcoSeed
                    Me.m_rbEcoseed.Checked = True
                Case eMPAOptimizationModels.RandomSearch
                    Me.m_rbRandom.Checked = True
                Case Else
                    Debug.Assert(False, cStringUtils.Localize("Unsupported search type selected {0}", CInt(prop.GetValue())))
            End Select
            Me.m_bInUpdate = False
        End Sub

#End Region ' Properties

#Region " Map "

        Private Sub OnLayerChanged(l As cDisplayLayer, changeFlags As cDisplayLayer.eChangeFlags)

            If (Me.m_bInUpdate) Then Return

            If ((changeFlags And cDisplayLayer.eChangeFlags.Selected) > 0) Then
                Me.UpdateControls()
            End If
            Me.SelectedLayer = l

        End Sub

#End Region ' Map

#End Region ' Events

#Region " Internals "

#Region " One-time initialization "

        Private Sub InitProgressGraph()

            Dim zgcr As New ZedGraph.ColorSymbolRotator

            ' Flush first color to make sure that the two graps (progress and output) use the same colour scheme
            Dim clrFlush As Color = zgcr.NextColor
            Dim gp As GraphPane = Nothing

            For i As Integer = 0 To 5
                Me.m_progress(i) = New ResultPoints()
            Next

            Me.m_zghProgress = New cZedGraphHelper()
            Me.m_zghProgress.Attach(Me.UIContext, Me.m_graphProgress)
            gp = Me.m_zghProgress.GetPane(1)

            With gp

                .Legend.Position = ZedGraph.LegendPos.Right
                .Title.IsVisible = False
                .XAxis.Title.Text = "" ' Config with form mode
                .YAxis.Title.Text = "" ' Config with form mode

                ' JS 19nov08: let graph figure out the ticks
                '' Only show major ticks
                'Me.m_graphProgress.GraphPane.XAxis.Scale.MajorStep = 5
                'Me.m_graphProgress.GraphPane.XAxis.Scale.MinorStep = 1

                .AddCurve(SharedResources.HEADER_NET_ECONOMIC_VALUE, Me.m_progress(0), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_SOCIAL_VALUE_EMPLOYMENT, Me.m_progress(1), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_MANDATED_REBUILDING, Me.m_progress(2), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_ECOSYSTEM_STRUCTURE, Me.m_progress(3), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_BIODIVERSITY, Me.m_progress(4), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_BOUNDARYWEIGHT, Me.m_progress(5), zgcr.NextColor, ZedGraph.SymbolType.None)

            End With

            Me.m_zghProgress.AutoscalePane = True

        End Sub

        Private Sub InitOutputGraph()

            Dim zgcr As New ZedGraph.ColorSymbolRotator
            Dim gp As GraphPane = Nothing

            Me.m_zghResults = New cZedGraphHelper()
            Me.m_zghResults.Attach(Me.UIContext, Me.m_graphResults)
            Me.m_zghResults.ShowCursor = True

            For i As Integer = 0 To 6
                Me.m_results(i) = New ResultPoints()
            Next

            gp = Me.m_zghResults.GetPane(1)
            With gp

                .Legend.Position = ZedGraph.LegendPos.Right
                .Title.IsVisible = False
                .Title.Text = "" ' Config with form mode
                .Title.Text = "" ' Config with form mode

                .AddCurve(SharedResources.HEADER_NET_ECONOMIC_VALUE, Me.m_results(1), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_SOCIAL_VALUE_EMPLOYMENT, Me.m_results(2), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_MANDATED_REBUILDING, Me.m_results(3), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_ECOSYSTEM_STRUCTURE, Me.m_results(4), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_BIODIVERSITY, Me.m_results(5), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(SharedResources.HEADER_BOUNDARYWEIGHT, Me.m_results(6), zgcr.NextColor, ZedGraph.SymbolType.None)
                .AddCurve(My.Resources.SEARCH_LABEL_TOTAL_WEIGHTED, Me.m_results(0), zgcr.NextColor, ZedGraph.SymbolType.None)

            End With

            Me.m_zghResults.AutoscalePane = True
            AddHandler Me.m_zghResults.OnCursorPos, AddressOf Me.OnResultCursorPos

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
            Set(value As eFormModeTypes)
                ' Switching?
                If value <> Me.m_mode Then
                    ' Exit previous mode
                    Me.ExitMode()
                    ' Store mode
                    Me.m_mode = value
                    ' Enter new mode
                    Me.EnterMode()
                    ' Reflect changes 
                    Me.UpdateControls()
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
            Set(value As eMPAOptimizationModels)
                ' Only valid while preparing a run
                If (Me.RunMode <> eFormModeTypes.Prepare) Then Return

                ' Clean up
                Me.ClearMapFeedback()
                Me.ClearLastRun()

                Dim factory As New cLayerFactoryInternal()

                ' Set search type
                If (Me.m_propSearchType IsNot Nothing) Then Me.m_propSearchType.SetValue(value)

                ' Polute again
                Me.InitMapFeedback()

                ' Update visible state of existing layers
                Me.ShowLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerMPASeed),
                    Me.SearchType = eMPAOptimizationModels.EcoSeed, Me.SearchType = eMPAOptimizationModels.EcoSeed)
                Me.ShowLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerMPARandom),
                    Me.SearchType = eMPAOptimizationModels.RandomSearch, Me.SearchType = eMPAOptimizationModels.RandomSearch)
                Me.ShowLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerImportance),
                     Me.SearchType = eMPAOptimizationModels.RandomSearch, Me.SearchType = eMPAOptimizationModels.RandomSearch)

                ' Update graph labels
                Select Case Me.SearchType
                    Case eMPAOptimizationModels.EcoSeed
                        Me.m_graphProgress.GraphPane.XAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_ECOSEED
                        Me.m_graphResults.GraphPane.XAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_ECOSEED
                    Case eMPAOptimizationModels.RandomSearch
                        Me.m_graphProgress.GraphPane.XAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_RANDOMSEARCH
                        Me.m_graphResults.GraphPane.XAxis.Title.Text = My.Resources.MPAOPT_AXISLABEL_BESTITERATIONS
                End Select

                Me.m_zghProgress.RescaleAndRedraw()

            End Set
        End Property

        Private Function SelectedClosedPercentage() As Integer
            Dim iPerc As Integer = 20
            Try
                If (Me.m_cmbAreaClosed.SelectedIndex >= 0) Then
                    iPerc = CInt(Me.m_cmbAreaClosed.Items(Me.m_cmbAreaClosed.SelectedIndex))
                End If
            Catch ex As Exception
                ' Wow
            End Try
            Return iPerc
        End Function

        Private Function SelectedBestPercentile() As Single
            Return CSng(Me.m_nudBestPercentile.Value)
        End Function

        Private Function SelectedMPA() As Integer
            If (Me.m_fpMPA Is Nothing) Then Return 0
            Return CInt(Me.m_fpMPA.Value())
        End Function

        Private Sub EnterMode()

            Select Case Me.m_mode
                Case eFormModeTypes.Prepare
                    ' User is about to start entering data

                Case eFormModeTypes.Initializing
                    ' Set stop delegate
                    Me.Core.SetStopRunDelegate(New cCore.StopRunDelegate(AddressOf Me.m_manager.StopRun))
                    ' Set running status text
                    cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_SEARCH_INITIALIZING, -1)
                    ' Switch to 'Results' page
                    Me.m_tcResults.SelectedIndex = 0
                    ' Clear best cells
                    For iRow As Integer = 0 To Me.m_basemap.InRow
                        For iCol As Integer = 0 To Me.m_basemap.InCol
                            Me.m_mapFeedback(iRow, iCol) = 0
                        Next iCol
                    Next iRow
                    Me.m_feedbackLayers(0).Update(cDisplayLayer.eChangeFlags.Map)

                Case eFormModeTypes.Searching
                    ' Set stop delegate
                    Me.Core.SetStopRunDelegate(New cCore.StopRunDelegate(AddressOf Me.m_manager.StopRun))
                    ' Set running status text
                    cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_SEARCH_SEARCHING, -1)

                Case eFormModeTypes.Stopping
                    ' Set running status text
                    cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_SEARCH_STOPPING, -1)

                Case eFormModeTypes.Results
                    ' Switch to 'Results' page
                    Me.m_tcResults.SelectedIndex = 1
                    ' Show results if possible
                    If Me.m_cmbAreaClosed.Items.Count > 0 Then
                        Me.m_cmbAreaClosed.SelectedIndex = 0
                    End If

            End Select

        End Sub

        Private Sub ExitMode()
            Select Case Me.m_mode

                Case eFormModeTypes.Prepare ' Prepare for running mode
                    Me.ClearLastRun()

                Case eFormModeTypes.Searching
                    ' Cancel running status text
                    Me.Core.SetStopRunDelegate(Nothing)
                    cApplicationStatusNotifier.EndProgress(Me.Core)

                Case eFormModeTypes.Initializing
                    ' Cancel running status text
                    Me.Core.SetStopRunDelegate(Nothing)
                    cApplicationStatusNotifier.EndProgress(Me.Core)

                Case eFormModeTypes.Stopping
                    ' Cancel running status text
                    cApplicationStatusNotifier.EndProgress(Me.Core)

                Case eFormModeTypes.Results ' Show results
                    ' Clear results
                    Me.ClearLastRun()

            End Select

        End Sub

        Private Sub Reload()
            ' Store ref
            Me.m_basemap = Me.UIContext.Core.EcospaceBasemap
            Me.ReloadMap()
            Me.ReloadMPAChoices()
            Me.Invalidate(True)
        End Sub

        Private Sub ReloadMap()

            Me.m_ucZoom.Map.SuspendLayout()
            Me.m_ucLayers.LockUpdates()

            Me.m_ucZoom.Map.Clear()

            Me.m_mpaLayers = Me.AddBaseLayers(eVarNameFlags.LayerMPA)
            Me.m_ecoseedLayer = Me.AddBaseLayers(eVarNameFlags.LayerMPASeed)(0)
            Me.AddBaseLayers(eVarNameFlags.LayerMPARandom)
            Me.AddBaseLayers(eVarNameFlags.LayerImportance)
            Me.AddBaseLayers(eVarNameFlags.LayerRegion)
            Me.AddBaseLayers(eVarNameFlags.LayerHabitat)
            Me.AddBaseLayers(eVarNameFlags.LayerDepth)

            ' Hide habitat layers at startup
            Dim factory As New cLayerFactoryInternal()
            Me.ShowLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerHabitat), False, True)

            Me.m_ucLayers.UnlockUpdates()
            Me.m_ucZoom.Map.ResumeLayout()

            Me.m_ecoseedLayer.IsSelected = True

        End Sub

        Private Sub ReloadMPAChoices()

            ' Get MPA optimization params to connect start MPA to
            Dim MPAOpt As cMPAOptParameters = Me.UIContext.Core.MPAOptimizationManager.MPAOptimizationParameters
            ' Create list of available MPAs
            Dim mpas As New List(Of cCoreInputOutputBase)

            ' Build list of MPAs
            For iMPA As Integer = 1 To Me.UIContext.Core.nMPAs
                mpas.Add(Me.UIContext.Core.EcospaceMPAs(iMPA))
            Next

            Me.m_fpMPA.Items = mpas.ToArray

            ' Only one MPA available?
            If mpas.Count = 1 Then
                ' #Yes: select first MPA
                Me.m_fpMPA.Value = mpas(0).Index
            End If

        End Sub

        Private Sub InvaldiateMap()
            Me.m_ucZoom.Map.Invalidate()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, called when a new seed cell has been selected in the
        ''' MPA optimization engine.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnHandleSeedCellCallback()

            ' Sanity check
            If Not (Me.m_manager.IsRunning()) Then Return

            Dim output As cMPAOptOutput = Me.m_manager.CurrentRowColResults

            Try

                ' Perform search specific updates
                Select Case Me.SearchType

                    Case eMPAOptimizationModels.EcoSeed
                        ' Ecoseed: the seed cell configuration has changed. 
                        ' The seed cell map has to be updated, which is done in the GUI
                        ' Populate run state layer with current row/col results
                        For iRow As Integer = 0 To Me.m_basemap.InRow
                            For iCol As Integer = 0 To Me.m_basemap.InCol
                                Me.m_mapFeedback(iRow, iCol) = cLayerFactoryInternal.cECOSEED_LAYER_NOVALUE
                            Next iCol
                        Next iRow

                        If output.CurRow > 0 And output.CurCol > 0 Then
                            Me.m_mapFeedback(output.CurRow, output.CurCol) = cLayerFactoryInternal.cECOSEED_LAYER_CURRENTVALUE
                        End If
                        If output.BestRow > 0 And output.BestCol > 0 Then
                            Me.m_mapFeedback(output.BestRow, output.BestCol) = cLayerFactoryInternal.cECOSEED_LAYER_BESTVALUE
                        End If

                    Case eMPAOptimizationModels.RandomSearch

                        ' No specific handling
                        Me.m_mpaLayers(0).Update(cDisplayLayer.eChangeFlags.Map)

                End Select

                Me.LogProgress(output.EconomicValue, output.SocialValue,
                               output.MandatedValue, output.EcologicalValue,
                               output.BiomassDiversityValue, output.AreaBoundaryValue,
                               output.TotalValue, output.PercentageClosed)

                ' Trigger feedback layer update
                Me.m_feedbackLayers(0).Update(cDisplayLayer.eChangeFlags.Map)

            Catch ex As Exception

            End Try


        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, called when a new cell has been selected.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub HandleNewCellSelected()

            ' Sanity check
            Debug.Assert(Me.m_manager.IsRunning())

            Try
                Dim output As cMPAOptOutput = Me.m_manager.CurrentRowColResults

                Select Case Me.SearchType

                    Case eMPAOptimizationModels.EcoSeed
                        ' A new MPA cell has been selected out of the seed cells
                        ' Redraw MPA map
                        For Each l As cDisplayLayerRaster In Me.m_feedbackLayers
                            l.IsModified = True
                            l.Update(cDisplayLayer.eChangeFlags.Map)
                        Next

                        ' Show this in the graph
                        Me.LogProgress(output.EconomicValue, output.SocialValue,
                                       output.MandatedValue, output.EcologicalValue,
                                       output.BiomassDiversityValue, output.AreaBoundaryValue,
                                       output.TotalValue, output.PercentageClosed)

                    Case eMPAOptimizationModels.RandomSearch
                        ' Does not apply to Random search
                        For Each l As cDisplayLayerRaster In Me.m_mpaLayers
                            l.IsModified = True
                            l.Update(cDisplayLayer.eChangeFlags.Map)
                        Next

                End Select
                Me.InvaldiateMap()

            Catch ex As Exception

            End Try

        End Sub

        Private Sub HandleNewBestResultFound()

            Dim output As cMPAOptOutput = Me.m_manager.CurrentRowColResults

            ' Sanity check
            Debug.Assert(Me.m_manager.IsRunning())

            Try

                Select Case Me.SearchType

                    Case eMPAOptimizationModels.EcoSeed

                        ' Ecoseed: the seed cell configuration has changed. 
                        ' The seed cell map has to be updated, which is done in the GUI
                        ' Populate run state layer with current row/col results
                        For iRow As Integer = 0 To Me.m_basemap.InRow
                            For iCol As Integer = 0 To Me.m_basemap.InCol
                                Me.m_mapFeedback(iRow, iCol) = cLayerFactoryInternal.cECOSEED_LAYER_NOVALUE
                            Next iCol
                        Next iRow

                        If output.CurRow > 0 And output.CurCol > 0 Then
                            Me.m_mapFeedback(output.CurRow, output.CurCol) = cLayerFactoryInternal.cECOSEED_LAYER_CURRENTVALUE
                        End If
                        If output.BestRow > 0 And output.BestCol > 0 Then
                            Me.m_mapFeedback(output.BestRow, output.BestCol) = cLayerFactoryInternal.cECOSEED_LAYER_BESTVALUE
                        End If

                        Me.m_feedbackLayers(0).Update(cDisplayLayer.eChangeFlags.Map)

                    Case eMPAOptimizationModels.RandomSearch
                        ' NOP

                End Select

            Catch ex As Exception

            End Try

        End Sub

#End Region ' Run-mode specific updates

#Region " Layer editor "

        Private Property SelectedLayer() As cDisplayLayer
            Get
                Return Me.m_layerSelected
            End Get
            Set(ByVal layer As cDisplayLayer)

                If ReferenceEquals(layer, Me.m_layerSelected) Then Return

                Me.SuspendLayout()

                If (Me.m_layerSelected IsNot Nothing) Then
                    ' Has editor GUI?
                    If (Me.m_editorGUISelected IsNot Nothing) Then
                        ' #Yes: remove layer editor GUI
                        Me.m_plEditor.Controls.Remove(Me.m_editorGUISelected)
                        Me.m_editorGUISelected = Nothing
                    End If

                    If (TypeOf Me.m_layerSelected Is cDisplayLayerRaster) Then
                        DirectCast(Me.m_layerSelected, cDisplayLayerRaster).Editor.DestroyEditorControl()
                    End If
                End If

                Me.m_layerSelected = layer

                If (Me.m_layerSelected IsNot Nothing) Then

                    ' Add layer editor GUI
                    If (TypeOf Me.m_layerSelected Is cDisplayLayerRaster) And (Me.RunMode = eFormModeTypes.Prepare) Then
                        Me.m_editorGUISelected = DirectCast(Me.m_layerSelected, cDisplayLayerRaster).Editor.CreateEditorControl()
                    End If

                    If (Me.m_editorGUISelected IsNot Nothing) Then
                        Me.m_plEditor.Height = Me.m_editorGUISelected.Height
                        Me.m_editorGUISelected.Dock = DockStyle.Fill
                        Me.m_plEditor.Controls.Add(Me.m_editorGUISelected)

                    End If
                End If

                Me.ResumeLayout()

                Me.m_plEditor.Visible = (Me.m_editorGUISelected IsNot Nothing)

            End Set
        End Property

#End Region ' Layer editor

#Region " Map updating "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper function to create layer(s) for a given varname.
        ''' </summary>
        ''' <param name="varName">The core variable to load basemap data for.</param>
        ''' -------------------------------------------------------------------
        Private Function AddBaseLayers(varName As eVarNameFlags) As cDisplayLayerRaster()

            Dim factory As New cLayerFactoryInternal()
            Dim strGroup As String = factory.GetLayerGroup(varName)
            Dim alayers As cDisplayLayerRaster() = factory.GetLayers(Me.UIContext, varName)
            Dim l As cDisplayLayer = Nothing

            ' Add group, and collapse and hide habitat layers
            Me.m_ucLayers.AddGroup(strGroup, "", varName <> eVarNameFlags.LayerHabitat)

            ' Add individual layers
            For iLayer As Integer = 0 To alayers.Length - 1
                l = alayers(iLayer)
                If (TypeOf (l) Is cDisplayLayerRaster) Then
                    ' Add the layer to the control(s)
                    Dim rl As cDisplayLayerRaster = DirectCast(l, cDisplayLayerRaster)
                    ' Do not block out key layers
                    Dim bCanEdit As Boolean = (varName = eVarNameFlags.LayerMPASeed) Or (varName = eVarNameFlags.LayerImportance)
                    rl.Editor.IsReadOnly = Not bCanEdit
                    Me.AddLayer(l, strGroup)
                End If
            Next

            Return alayers

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub InitMapFeedback()

            Dim strGroup As String = ""
            Dim l As cDisplayLayerRaster = Nothing
            Dim alayers As cDisplayLayerRaster() = Nothing
            Dim lRunStateLayers As New List(Of cDisplayLayerRaster)
            Dim factory As New cLayerFactoryInternal()
            Dim datalayerTemp As cEcospaceLayerInteger = Nothing

            Me.m_ucLayers.LockUpdates()

            Try

                ' Redim data
                ReDim Me.m_mapFeedback(Me.m_basemap.InRow, Me.m_basemap.InCol)

                Select Case Me.SearchType

                    Case eMPAOptimizationModels.EcoSeed

                        ' Get group
                        strGroup = factory.GetLayerGroup(eVarNameFlags.LayerMPASeed)

                        ' DO NOT CHANGE THE ORDER OF LAYERS HERE TO ENSURE THAT THE 
                        ' SEED PROGRESS INDICATORS SHOW UP ON TOP OF THE RUNNING SEED CELLS,
                        ' AND THAT THE BEST CELL SHOWS UP ON TOP OF THE CURRENT CELL

                        ' Create best cell layer
                        datalayerTemp = New cEcospaceLayerInteger(Me.UIContext.Core, Me.m_mapFeedback, My.Resources.ECOSPACE_LAYER_SEEDBEST, Nothing, eVarNameFlags.LayerMPASeedBest)
                        alayers = factory.GetLayers(Me.UIContext, eVarNameFlags.LayerMPASeedBest, datalayerTemp)
                        For iLayer As Integer = 0 To alayers.Length - 1
                            l = alayers(iLayer)
                            l.Editor.IsReadOnly = True
                            Me.AddLayer(l, strGroup, Me.m_ecoseedLayer)
                        Next
                        lRunStateLayers.AddRange(alayers)

                        ' Create current cell layer(s)
                        datalayerTemp = New cEcospaceLayerInteger(Me.UIContext.Core, Me.m_mapFeedback, My.Resources.ECOSPACE_LAYER_SEEDCURRENT, Nothing, eVarNameFlags.LayerMPASeedCurrent)
                        alayers = factory.GetLayers(Me.UIContext, eVarNameFlags.LayerMPASeedCurrent, datalayerTemp)
                        For iLayer As Integer = 0 To alayers.Length - 1
                            l = alayers(iLayer)
                            l.Editor.IsReadOnly = True
                            Me.AddLayer(l, strGroup, Me.m_ecoseedLayer)
                        Next

                        lRunStateLayers.AddRange(alayers)

                    Case eMPAOptimizationModels.RandomSearch

                        ' Create random output layer
                        strGroup = factory.GetLayerGroup(eVarNameFlags.LayerMPARandom)
                        datalayerTemp = New cEcospaceLayerInteger(Me.UIContext.Core, Me.m_mapFeedback, My.Resources.ECOSPACE_LAYER_RANDOMBEST, Nothing, eVarNameFlags.LayerMPARandom)

                        ' Create current cell layer(s)
                        alayers = factory.GetLayers(Me.UIContext, eVarNameFlags.LayerMPARandom, datalayerTemp)
                        For iLayer As Integer = 0 To alayers.Length - 1
                            l = alayers(iLayer)
                            l.Editor.IsReadOnly = True
                            Me.AddLayer(l, strGroup)
                        Next
                        lRunStateLayers.AddRange(alayers)

                End Select

                Me.m_feedbackLayers = lRunStateLayers.ToArray()

            Catch ex As Exception

            End Try

            Me.m_ucLayers.UnlockUpdates()

        End Sub

        Private Sub ClearMapFeedback()
            If Me.m_feedbackLayers IsNot Nothing Then
                For Each l As cDisplayLayer In Me.m_feedbackLayers
                    Me.RemoveLayer(l)
                Next
                Me.m_feedbackLayers = Nothing
            End If
            Me.m_mapFeedback = Nothing
        End Sub

        Private Sub UpdateBestCountMap()

            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed

                Case eMPAOptimizationModels.RandomSearch

                    Dim iNumResults As Integer = 0
                    Dim cells(,) As Integer = Me.m_manager.CellSelectedMap(Me.SelectedBestPercentile,
                                                                             Me.SelectedClosedPercentage,
                                                                             iNumResults)

                    For iRow As Integer = 1 To Me.m_basemap.InRow
                        For iCol As Integer = 1 To Me.m_basemap.InCol
                            Me.m_mapFeedback(iRow, iCol) = cells(iRow, iCol)
                        Next
                    Next

                    ' In Random MPA, layer(0) is the only feedback layer
                    ' Invalidate to recalc min, max
                    Me.m_feedbackLayers(0).IsModified = True
                    ' Trigger redraw
                    For Each l As cDisplayLayerRaster In Me.m_feedbackLayers
                        l.IsModified = True
                    Next
                    Me.InvaldiateMap()

            End Select

        End Sub

#End Region ' Map updating

#Region " Progress "

        Private Sub LogProgress(sEconomicValue As Single, sSocialValue As Single,
                                     sMandatedValue As Single, sEcologicalValue As Single,
                                     sBiomassDiversityValue As Single, sBoundaryWeightValue As Single,
                                     sTotalValue As Single, sAreaPercentageClosed As Single)

            ' Show this in the graph
            Dim strPerc As String = CStr(Math.Round(sAreaPercentageClosed))
            Dim gp As GraphPane = Me.m_zghProgress.GetPane(1)

            ' All 0: do not log
            If (sEconomicValue + sSocialValue + sMandatedValue + sEcologicalValue + sBiomassDiversityValue) = 0.0 Then Return

            For iResult As Integer = 0 To Me.m_progress.Length - 1
                Dim rp As ResultPoints = Me.m_progress(iResult)
                Select Case iResult
                    Case 0 : rp.AddItem(sEconomicValue)
                    Case 1 : rp.AddItem(sSocialValue)
                    Case 2 : rp.AddItem(sMandatedValue)
                    Case 3 : rp.AddItem(sEcologicalValue)
                    Case 4 : rp.AddItem(sBiomassDiversityValue)
                    Case 5 : rp.AddItem(sBoundaryWeightValue)
                End Select
            Next

            Me.m_zghProgress.RescaleAndRedraw()

            Me.m_gridProgress.LogResult(sEconomicValue, sSocialValue,
                                        sMandatedValue, sEcologicalValue,
                                        sBiomassDiversityValue, sBoundaryWeightValue,
                                        sTotalValue, sAreaPercentageClosed)

            If (Me.m_cmbAreaClosed.FindStringExact(strPerc) = -1) Then
                Me.m_cmbAreaClosed.Items.Add(strPerc)
            End If

        End Sub

#End Region ' Progress

#Region " Results "

        Private Class cObjectiveResultComparer
            Implements IComparer(Of cObjectiveResult)

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Sorts objective results in DESCENDING order!
            ''' </summary>
            ''' <param name="x"></param>
            ''' <param name="y"></param>
            ''' <returns></returns>
            ''' ---------------------------------------------------------------
            Public Function Compare(x As EwECore.cObjectiveResult,
                                    y As EwECore.cObjectiveResult) As Integer _
                                    Implements IComparer(Of EwECore.cObjectiveResult).Compare
                ' DESCENDING ORDER! < 1, = 0, > -1 (instead of customary ascending order < -1, = 0, > 1)
                If x.objFuncTotal > y.objFuncTotal Then Return -1
                If x.objFuncTotal < y.objFuncTotal Then Return 1
                Return 0
            End Function

        End Class

        Private Sub UpdateResultsGraph()

            Dim lResults As List(Of cObjectiveResult) = Nothing

            Select Case Me.SearchType
                Case eMPAOptimizationModels.EcoSeed
                    ' Get all results
                    lResults = Me.m_manager.Results
                Case eMPAOptimizationModels.RandomSearch
                    ' Get results, filtered by selected percentage area closed
                    lResults = Me.FilteredResults(Me.m_manager.Results, Me.SelectedClosedPercentage)
                    ' Sort the results
                    lResults.Sort(New cObjectiveResultComparer())
                    ' Only show top percentile
                    If lResults.Count > 0 Then
                        ' Strip off anything past top x %
                        Dim iIndex As Integer = CInt(Math.Ceiling(lResults.Count * Me.SelectedBestPercentile / 100.0!))
                        lResults.RemoveRange(iIndex, lResults.Count - iIndex)
                    End If

            End Select

            Try
                ' Fill output graph
                For iResult As Integer = 0 To lResults.Count - 1
                    Dim result As cObjectiveResult = lResults(iResult)
                    Me.m_results(0).AddItem(result.objFuncTotal)
                    Me.m_results(1).AddItem(result.objFuncEconomicValue)
                    Me.m_results(2).AddItem(result.objFuncSocialValue)
                    Me.m_results(3).AddItem(result.objFuncMandatedValue)
                    Me.m_results(4).AddItem(result.objFuncEcologicalValue)
                    Me.m_results(5).AddItem(result.objBiomassDiversity)
                    Me.m_results(6).AddItem(result.objFuncAreaBorder)
                Next
                Me.m_graphResults.GraphPane.XAxis.Scale.Max = lResults.Count - 1

                Me.m_zghResults.CursorPos = 0.0
                Me.m_zghResults.RescaleAndRedraw()

            Catch ex As Exception

            End Try

        End Sub

        Private Sub ShowIteration(iIteration As Integer)

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_manager Is Nothing) Then Return

            Dim lResults As List(Of cObjectiveResult) = Nothing
            Dim res As cObjectiveResult = Nothing
            Dim cell As cMPACell = Nothing
            Dim mpaMap(Me.m_basemap.InRow, Me.m_basemap.InCol) As Integer

            Select Case Me.SearchType

                Case eMPAOptimizationModels.EcoSeed
                    lResults = Me.m_manager.Results()

                Case eMPAOptimizationModels.RandomSearch
                    lResults = Me.FilteredResults(Me.m_manager.Results, Me.SelectedClosedPercentage())

            End Select

            ' Truncate iteration index
            iIteration = Math.Max(0, Math.Min(lResults.Count - 1, iIteration))
            ' Get results
            If (iIteration < lResults.Count) Then
                res = lResults(iIteration)
                For iCell As Integer = 0 To res.Cells.Count - 1
                    cell = res.Cells(iCell)
                    mpaMap(cell.Row, cell.Col) = cell.iMPA
                Next iCell
            End If

            Me.SetLayer(mpaMap, Me.m_basemap.LayerMPA(Me.SelectedMPA()), Me.SelectedMPA())

            If (res IsNot Nothing) Then
                ' Update indicators
                Me.m_gridResults.LogResult(res.objFuncEconomicValue, res.objFuncSocialValue,
                                           res.objFuncMandatedValue, res.objFuncEcologicalValue,
                                           res.objBiomassDiversity, res.objFuncAreaBorder,
                                           res.objFuncTotal, res.PercentageClosed)
            End If

            Me.m_ucZoom.Map.Invalidate()

        End Sub

        Private Sub ShowBestPercentage()
            Me.UpdateBestCountMap()
        End Sub

        Private Function FilteredResults(lIn As List(Of cObjectiveResult),
                                         Optional iPercAreaClosed As Integer = -1) As List(Of cObjectiveResult)

            If iPercAreaClosed = -1 Then Return lIn
            Dim lOut As New List(Of cObjectiveResult)

            For iResult As Integer = 0 To lIn.Count - 1
                If lIn(iResult).PercentageClosed = iPercAreaClosed Then
                    lOut.Add(lIn(iResult))
                End If
            Next

            lOut.Sort(New cObjectiveResultComparer)

            Return lOut
        End Function

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
        Private Sub AddLayer(l As cDisplayLayer, strGroup As String, Optional layerPosition As cDisplayLayer = Nothing)
            Me.m_layers.Add(l)
            Me.m_ucZoom.Map.AddLayer(l, layerPosition)
            Me.m_ucLayers.AddLayer(l, strGroup, "", layerPosition)
            AddHandler l.LayerChanged, AddressOf Me.OnLayerChanged
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remmove a layer from the map.
        ''' </summary>
        ''' <param name="l">Layer to remove.</param>
        ''' -------------------------------------------------------------------
        Private Sub RemoveLayer(l As cDisplayLayer)
            Me.m_layers.Remove(l)
            Me.m_ucZoom.Map.RemoveLayer(l)
            Me.m_ucLayers.RemoveLayer(l)
            RemoveHandler l.LayerChanged, AddressOf Me.OnLayerChanged
        End Sub

        Private Sub ShowLayerGroup(strGroup As String, bShowLayers As Boolean, bShowGroup As Boolean)
            Me.m_ucLayers.ShowGroup(strGroup, bShowLayers, bShowGroup)
        End Sub

        Private Sub EnableLayerGroup(strGroup As String, bEditable As Boolean)
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
        Private Sub SetLayer(src As Integer(,), lDest As cEcospaceLayer,
            Optional iConvertTo As Integer = cCore.NULL_VALUE)

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
                        iValue = If(iValue = 0, iValue, iConvertTo)
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
        Private Sub SetLayer(src As Single(,), lDest As cEcospaceLayer,
            Optional iConvertTo As Integer = cCore.NULL_VALUE)

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
                        sValue = CInt(If(sValue = 0, sValue, iConvertTo))
                    End If
                    ' Apply!
                    lDest.Cell(iRow, iCol) = sValue
                Next iCol
            Next iRow

        End Sub

#End Region ' Map

#Region " Generic "

        Protected Overrides Sub UpdateControls()

            ' The %^@#$^#@$ check boxes throw events even before the form OnLoad has been called. Nice.
            ' Added sanity check to prevent premature control handling
            If (Me.m_manager Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return
            Me.m_bInUpdate = True

            Dim bIsInputMode As Boolean = (Me.RunMode = eFormModeTypes.Prepare) Or (Me.RunMode = eFormModeTypes.Results)
            Dim bIsRunning As Boolean = (Me.RunMode = eFormModeTypes.Searching Or Me.RunMode = eFormModeTypes.Initializing Or Me.RunMode = eFormModeTypes.Stopping)
            Dim bIsResults As Boolean = (Me.RunMode = eFormModeTypes.Results)
            Dim bIsEcoseed As Boolean = (Me.SearchType = eMPAOptimizationModels.EcoSeed)
            Dim bIsRandom As Boolean = (Me.SearchType = eMPAOptimizationModels.RandomSearch)
            Dim bHasRegions As Boolean = (Me.Core.nRegions > 0)
            Dim bMPALayerSelected As Boolean = (Me.SelectedMPA() > 0)
            Dim factory As New cLayerFactoryInternal()

            ' Update input controls
            Me.m_rbEcoseed.Enabled = (bIsInputMode)
            Me.m_rbRandom.Enabled = (bIsInputMode)
            Me.m_fpStartYear.Enabled = bIsInputMode
            Me.m_fpEndYear.Enabled = bIsInputMode
            Me.m_fpBaseYear.Enabled = bIsInputMode
            Me.m_fpMinArea.Enabled = (bIsInputMode And bIsRandom)
            Me.m_fpMaxArea.Enabled = (bIsInputMode And bIsRandom)
            Me.m_fpStepSize.Enabled = (bIsInputMode And bIsRandom)
            Me.m_fpDiscRate.Enabled = bIsInputMode
            Me.m_fpGenDiscRate.Enabled = bIsInputMode
            Me.m_fpIterations.Enabled = (bIsInputMode And bIsRandom)
            Me.m_fpMPA.Enabled = bIsInputMode

            Me.m_gridObjectives.Enabled = (bIsInputMode)
            Me.m_gridFleet.Enabled = (bIsInputMode)
            Me.m_gridGroup.Enabled = (bIsInputMode)

            Me.m_fpRegions.Enabled = bIsInputMode And bIsRandom And bHasRegions

            ' Results
            Me.m_graphResults.Enabled = bIsResults
            Me.m_cmbAreaClosed.Enabled = (bIsResults And bIsRandom)
            Me.m_nudBestPercentile.Enabled = (bIsResults And bIsRandom)
            Me.m_btnResetMPAs.Enabled = bIsResults

            ' Update run control buttons
            Me.m_btnRun.Enabled = (bIsInputMode Or bIsResults) And Not bIsRunning
            Me.m_btnStop.Enabled = bIsRunning
            Me.m_btnConvertToMpa.Enabled = bIsResults
            Me.m_btnSave.Enabled = (bIsResults And bIsRandom)

            ' Toggle toolbar controls
            Me.m_tsbMPA.Enabled = bIsInputMode And bMPALayerSelected
            Me.m_tsbSeed.Enabled = bIsInputMode And bMPALayerSelected And bIsEcoseed
            Me.m_tsbEditLayers.Enabled = bIsInputMode And bIsRandom

            ' Layers enabled state
            Me.EnableLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerDepth), Not bIsRunning)
            Me.EnableLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerMPA), Not bIsRunning)
            Me.EnableLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerHabitat), Not bIsRunning)
            Me.EnableLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerMPASeed), Not bIsRunning)
            Me.EnableLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerMPARandom), Not bIsRunning)
            Me.EnableLayerGroup(factory.GetLayerGroup(eVarNameFlags.LayerImportance), Not bIsRunning)

            ' Update map
            Me.m_ucZoom.Map.Editable = bIsInputMode

            Me.m_cbAutoSave.Checked = Me.Core.Autosave(eAutosaveTypes.MPAOpt)

            Me.m_bInUpdate = False

        End Sub

        Private Function ValidateInputs() As Boolean

            Dim parms As cMPAOptParameters = Me.m_manager.MPAOptimizationParameters
            Dim valweights As cCoreInputOutputBase = Me.m_manager.ValueWeights
            Dim bOk As Boolean = True

            ' Check MPA selection
            If Me.m_cmbMPA.SelectedIndex = -1 Then
                Me.UIContext.Core.Messages.SendMessage(New cMessage(My.Resources.PROMPT_MPAOPT_SELECTION,
                                                                    eMessageType.Any, eCoreComponentType.MPAOptimization,
                                                                    eMessageImportance.Warning))
                Return False
            End If

            ' Check min / max area
            Dim iMinArea As Integer = parms.MinArea
            Dim iMaxArea As Integer = parms.MaxArea

            If iMaxArea < iMinArea Then
                parms.MaxArea = iMinArea
                parms.MinArea = iMaxArea
            End If

            ' Check mandated rebuilding
            If CSng(valweights.GetVariable(eVarNameFlags.FPSMandatedRebuildingWeight)) > 0.0 Then
                bOk = False
                For iGroup As Integer = 1 To Me.UIContext.Core.nGroups
                    valweights = Me.m_manager.GroupObjectives(iGroup)
                    bOk = bOk Or (CSng(valweights.GetVariable(eVarNameFlags.FPSGroupMandRelBiom)) > 0.0)
                Next
                If bOk = False Then
                    Me.UIContext.Core.Messages.SendMessage(New cMessage(My.Resources.PROMPT_MPAOPT_MANDATEDB,
                                                                        eMessageType.Any, eCoreComponentType.MPAOptimization,
                                                                        eMessageImportance.Warning))
                    Return False
                End If
            End If

            Return True

        End Function

        Private Sub ClearLastRun()

            For Each rp As ResultPoints In Me.m_progress
                If rp IsNot Nothing Then rp.Clear()
            Next
            For Each rp As ResultPoints In Me.m_results
                If rp IsNot Nothing Then rp.Clear()
            Next

            Me.m_graphProgress.GraphPane.XAxis.Scale.MaxAuto = True
            Me.m_graphResults.GraphPane.XAxis.Scale.MaxAuto = True

            Me.m_graphProgress.Refresh()
            Me.m_graphResults.Refresh()

            Me.m_cmbAreaClosed.Items.Clear()
        End Sub

#End Region ' Generic

#End Region ' Helper methods

#End Region ' Internals

    End Class

End Namespace