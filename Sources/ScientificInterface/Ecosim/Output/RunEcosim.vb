#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ScientificInterfaceShared
Imports ScientificInterface.Other
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SAUPUtil.SAUPData.Mapping
Imports ZedGraph
Imports Microsoft.VisualBasic

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form, implementing the Run Ecosim interface.
    ''' </summary>
    ''' =======================================================================
    Public Class RunEcosim

#Region " Variables "

        ''' <summary>
        ''' Enumerated type, indicating whether the user is viewing fleet or
        ''' group related fishing shapes underneath the Ecosim plot.
        ''' </summary>
        Private Enum eSelectionModeType
            ''' <summary>User has not made any selection (yet).</summary>
            NotSet = 0
            ''' <summary>User is viewing Fleet fishing shapes.</summary>
            Fleets
            ''' <summary>User is viewing Group fishing shapes.</summary>
            Groups
        End Enum

        Private m_selectionMode As eSelectionModeType = eSelectionModeType.NotSet
        Private m_ccb As cCustomComboBoxFleetGroupTree = Nothing

        Private m_shapeGUIHandler As cForcingShapeGUIHandler = Nothing
        Private m_params As cEcoSimModelParameters = Nothing
        Private m_iTimeSteps As Integer = 0
        Private m_sChangeTrackSize As Single = 0.1!
        Private m_zgp As cEcosimOutputPlotHelper = Nothing

        ''' <summary>
        ''' True when this interface is running ecosim. False otherwise
        ''' </summary>
        ''' <remarks>This is to stop this interface from responding to Ecosim messages if it did not start the ecosim run </remarks>
        Private m_bEcosimRunning As Boolean = False

        Private m_simStats As cEcosimStats

        Private m_bInUpdate As Boolean = False

        ' === plot data ==
        Private m_varname As ePlotData = ePlotData.Biomass
        Private m_bIsAnnual As Boolean = False
        Private m_bIsCumulative As Boolean = False
        Private m_bIsExploring As Boolean = False
        Private m_bIsEffortSelected As Boolean = False

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Constructors

#Region " Framework overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmd As cCommand = Nothing

            Me.m_params = Core.EcoSimModelParameters()
            Me.m_simStats = Me.Core.EcosimStats

            Me.m_ccb = New cCustomComboBoxFleetGroupTree(Me.Core, Me.tscbTarget)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.ShapesManager}

            Me.m_lbGroups.Attach(Me.UIContext)

            Me.m_zgp = New cEcosimOutputPlotHelper()
            Me.m_zgp.Attach(Me.UIContext, Me.m_graph)

            Me.m_zgp.ConfigurePane(My.Resources.HEADER_RELATIVEBIOMASS, My.Resources.HEADER_YEAR, My.Resources.HEADER_RELATIVEBIOMASS, False)
            Me.m_zgp.ShowMultipleRuns = Me.m_tsmShowMultipleRuns.Selected

            Me.m_zgp.AutoScaleYOption = cZedGraphHelper.eScaleOptionTypes.MaxOnly

            Me.m_zgp.YScaleMin = 0.0!
            Me.m_zgp.ShowPointValue = True

            ' Set the axis
            Me.m_graph.GraphPane.XAxis.Scale.Min = Core.EcosimFirstYear
            Me.m_graph.GraphPane.XAxis.Scale.Max = Core.EcoSimModelParameters.NumberYears + Core.EcosimFirstYear
            Me.m_graph.AxisChange()

            AddHandler Me.m_zgp.OnCursorPos, AddressOf OnSyncCursor

            Me.UpdateControls()

            ' Display Groups
            cmd = cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.AddControl(Me.m_tsbtnShowHideGroups)
            End If

            ' Track core monitor changes
            AddHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
            ' Track styleguide changes
            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            Me.PopulateGraph()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            RemoveHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            ' Unplug
            Me.IsExploring = False

            If Me.UIContext Is Nothing Then Return

            ' Clean up
            If Me.m_shapeGUIHandler IsNot Nothing Then
                Me.m_shapeGUIHandler.Detach()
                Me.m_shapeGUIHandler = Nothing
            End If

            ' Show/Hide Groups
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.RemoveControl(Me.m_tsbtnShowHideGroups)
            End If

            Me.m_lbGroups.Detach()

            RemoveHandler Me.m_zgp.OnCursorPos, AddressOf OnSyncCursor
            Me.m_zgp.Detach()

            MyBase.OnFormClosed(e)
        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            Select Case msg.Source

                Case eCoreComponentType.EcoSim
                    'handle ecosim messages
                    EcosimMessageHandler(msg)

                Case eCoreComponentType.ShapesManager
                    ' Respond to relevant shape changes
                    If (Me.m_shapeGUIHandler Is Nothing) Then Return

                    If (((Me.SelectionMode = eSelectionModeType.Fleets) And (msg.DataType = eDataTypes.FishingEffort)) Or _
                        ((Me.SelectionMode = eSelectionModeType.Groups) And (msg.DataType = eDataTypes.FishMort))) Then

                        Me.m_shapeGUIHandler.Refresh()

                    End If

            End Select

        End Sub

        Public Overrides ReadOnly Property IsRunForm() As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.GroupVisibility) > 0 Then
                Me.PopulateGraph()
            End If
        End Sub

#End Region ' Framework overrides

#Region " Events "

#Region " Controls "

        Private Sub btnRunOrStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnRunOrStop.Click

            If Not Me.m_bEcosimRunning Then
                Me.m_iTimeSteps = Me.Core.nEcosimTimeSteps
                Me.m_graph.Refresh()
                Me.Core.RunEcoSim(AddressOf TimeStepFromEcoSim_handler)
            Else
                Me.Core.StopEcoSim()
            End If

        End Sub

        Private Sub m_tsmShowMultipleRuns_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmShowMultipleRuns.Click

            Me.m_tsmShowMultipleRuns.Checked = Not Me.m_tsmShowMultipleRuns.Checked
            Me.m_zgp.ShowMultipleRuns = Me.m_tsmShowMultipleRuns.Checked
            Me.PopulateRunsBox()
            Me.m_zgp.RescaleAndRedraw()
            Me.UpdateControls()
        End Sub

        Private Sub AnnualOutputToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiShowAnnualOutput.Click

            If Me.m_bInUpdate Then Return

            Me.m_tsmiShowAnnualOutput.Checked = Not Me.m_tsmiShowAnnualOutput.Checked
            Me.IsAnnualPlot = Me.m_tsmiShowAnnualOutput.Checked
        End Sub

        Private Sub ShowLegendToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiShowLegend.Click

            Me.m_tsmiShowLegend.Checked = Not Me.m_tsmiShowLegend.Checked
            Me.m_zgp.IsLegendVisible = Me.m_tsmiShowLegend.Checked
            Me.m_zgp.RescaleAndRedraw()

        End Sub

        Private Sub CumulativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiCumulative.Click

            If Me.m_bInUpdate Then Return
            Me.m_tsmiRelative.Checked = Not Me.m_tsmiCumulative.Checked
            Me.IsCumulativePlot = Me.m_tsmiCumulative.Checked

        End Sub

        Private Sub RelativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiRelative.Click

            If Me.m_bInUpdate Then Return
            Me.m_tsmiCumulative.Checked = Not Me.m_tsmiRelative.Checked
            Me.IsCumulativePlot = (Me.m_tsmiRelative.Checked = False)

        End Sub

        Private Sub BiomassToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiBiomass.Click
            Me.PlotDataType = ePlotData.Biomass
        End Sub

        Private Sub CatchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiCatch.Click
            Me.PlotDataType = ePlotData.GroupCatch
        End Sub

        Private Sub AutoScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiAutoscale.Click
            Me.m_zgp.AutoScaleYOption = cZedGraphHelper.eScaleOptionTypes.MaxOnly
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbMax.Click
            m_tsmiAutoscale.Checked = False
            m_tsmiCustomScaleLabel.Checked = True
        End Sub

        Private Sub OnCustomScale(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiCustomScaleLabel.Click
            Double.TryParse(Me.m_tstbMax.Text, Me.m_zgp.YScaleMax)
            Double.TryParse(Me.m_tstbMin.Text, Me.m_zgp.YScaleMin)
            Me.m_zgp.AutoScaleYOption = cZedGraphHelper.eScaleOptionTypes.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSetMax_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbMax.LostFocus
            Double.TryParse(TryCast(sender, ToolStripTextBox).Text, Me.m_zgp.YScaleMax)
            Me.m_zgp.AutoScaleYOption = cZedGraphHelper.eScaleOptionTypes.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSetMin_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbMin.LostFocus
            Double.TryParse(TryCast(sender, ToolStripTextBox).Text, Me.m_zgp.YScaleMin)
            Me.m_zgp.AutoScaleYOption = cZedGraphHelper.eScaleOptionTypes.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tsmiSortMostChanged_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiSortMostChanged.Click, m_tssbExplore.Click

            If Me.m_bInUpdate Then Return

            Me.m_tsmiSortMostChanged.Checked = Not Me.m_tsmiSortMostChanged.Checked
            Me.IsExploring = Me.m_tsmiSortMostChanged.Checked

        End Sub

        Private Sub m_tstbChangeAmount_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbChangeAmount.LostFocus

            Single.TryParse(Me.m_tstbChangeAmount.Text, Me.m_sChangeTrackSize)
            Me.m_sChangeTrackSize = Math.Max(0, Me.m_sChangeTrackSize)
            Me.m_tstbChangeAmount.Text = CStr(Me.m_sChangeTrackSize)

            Me.UpdateControls()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Listbox selected index change handler
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub lb_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_lbRuns.SelectedIndexChanged, m_lbGroups.SelectedIndexChanged

            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True

            Try
                Dim lb As ListBox = DirectCast(sender, ListBox)
                ' Clear selection of all other groups when 'all' is clicked
                If (lb.GetSelected(0) = True) And (lb.SelectedIndices.Count > 1) Then
                    For i As Integer = 0 To lb.SelectedIndices.Count - 1
                        lb.SetSelected(i, (i = 0))
                    Next
                End If

                Me.UpdateGraphHighlights()
            Catch ex As Exception

            End Try
            Me.m_bInUpdate = False
        End Sub

        Private Sub OnSyncCursor(ByVal zgh As cZedGraphHelper, ByVal iPane As Integer, ByVal sPos As Single)
            If Me.IsExploring Then
                Try
                    ' Hmm, this logic fails when time series are loaded; in that case
                    ' the X value is corrected by the Ecosim year. Let's hack around this
                    ' for now.
                    Me.SortGroupsAtTimestep(CInt(Math.Round((sPos - Me.Core.EcosimFirstYear) * cCore.N_MONTHS)))
                Catch ex As Exception

                End Try
            End If
        End Sub

#End Region ' Controls

#Region " Core "

        Private Sub TimeStepFromEcoSim_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_ECOSIM_RUNNING, TriState.UseDefault, CSng(iTime / m_iTimeSteps))

        End Sub

        Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)

            ' Could be that we're closing
            If (Me.IsDisposed) Then Return

            Dim bEcosimRunning As Boolean = Core.StateMonitor.IsEcosimRunning
            Dim bHasEcosimResults As Boolean = Core.StateMonitor.HasEcosimRan

            ' Does not have ecosim results?
            If (Not Core.StateMonitor.HasEcopathRan) Then
                ' #Yes: clear run results
                Me.ResetGraph()
            End If

            ' Check whether ecosim is running
            ' Is this a state change?
            If (bEcosimRunning <> Me.m_bEcosimRunning) Then
                ' #Yes: update to new state
                Me.m_bEcosimRunning = bEcosimRunning
                If Me.m_bEcosimRunning Then
                    cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_ECOSIM_RUNNING, TriState.True, 0)
                Else
                    cApplicationStatusNotifier.SetStatusText("", TriState.False, 0)
                    If Not Me.m_zgp.ShowMultipleRuns Then
                        Me.m_zgp.Clear()
                    End If
                    Me.m_zgp.CreateRun(String.Format(My.Resources.ECOSIM_LABEL_RUN, (Me.m_zgp.NumRuns + 1)))
                    Me.PopulateRunsBox()
                    Me.PopulateGroupBox()
                End If
                Me.UpdateControls()

            End If

        End Sub

        Private Sub EcosimMessageHandler(ByRef msg As cMessage)

            Try
                Select Case msg.Type
                    Case eMessageType.EcosimRunCompleted

                        'jb if Ecosim was not run by this interface ignore this message
                        If (Me.m_iTimeSteps > 0) Then
                            Me.tslblSSValue.Text = Me.StyleGuide.FormatNumber(Me.Core.EcosimStats.SS)
                            Me.m_iTimeSteps = 0
                        End If

                        ' Plot the graph
                        Me.PopulateGraph()

                    Case eMessageType.EcosimNYearsChanged

                        'set the xaxis this is the number of time steps the model will run for
                        'm_ucBPlots.Plot.XAxis = Core.nEcosimTimeSteps
                        'now what..... hope it draws right next time!
                        'm_ucBPlots.Plot.GenerateOutputImage()

                    Case eMessageType.DataModified

                        For Each var As cVariableStatus In msg.Variables
                            If var.VarName = eVarNameFlags.EcosimSumEnd Or var.VarName = eVarNameFlags.EcosimSumStart Then
                                'the summary time periods has changed
                                'redraw the lines on the graph
                                'Me.m_ucBPlots.DrawSummaryLines(m_EcosimModelParams.StartSummaryTime, m_EcosimModelParams.EndSummaryTime)
                                Exit For
                            End If
                        Next

                End Select

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

#End Region ' Core

#Region " Forcing function "

        Private Sub tscbTarget_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscbTarget.SelectedIndexChanged
            Dim obj As ICoreInterface = GetSelectedGroupOrFleet()

            If TypeOf obj Is cFishingRateShape Then
                Me.SelectionMode = eSelectionModeType.Fleets
                Me.LoadFishingRateShape()
                Return
            End If

            If TypeOf obj Is cEcoPathGroupInput Then
                Me.SelectionMode = eSelectionModeType.Groups
                Me.LoadFishMortShape()
                Return
            End If

            Me.SelectionMode = eSelectionModeType.NotSet
            Me.ClearShape()

            Return
        End Sub

        Private Sub OnFValue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSetToValue.Click

            Dim strCaption As String = My.Resources.RUN_ECOSIM_F_VALUE_CAPTION
            Dim strMessage As String = My.Resources.RUN_ECOSIM_F_VALUE_MSG
            Dim strDefault As String = "1"
            Dim strValue As String = String.Empty

            ' Sanity check
            If Me.m_sketchPad.Shape Is Nothing Then Return

            strValue = Interaction.InputBox(strMessage, strCaption, strDefault)

            'User clicks OK
            If strValue.Length <> 0 Then

                Dim astrEntered As String() = strValue.Split(CChar(" "))

                ' One character entered?
                If astrEntered.Length = 1 Then
                    ' #Yes: duplicate this char over the entire shape
                    Try
                        If (Me.m_shapeGUIHandler IsNot Nothing) Then
                            Me.m_shapeGUIHandler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset, _
                                        New cShapeData() {Me.m_sketchPad.Shape}, CSng(Val(astrEntered(0))))
                        End If
                    Catch ex As Exception
                    End Try

                ElseIf astrEntered.Length > 1 Then

                    Dim shape As cShapeData = Me.m_sketchPad.Shape

                    ' Translate individual values
                    Dim asValues(shape.XMax) As Single
                    Dim sValue As Single = 0.0!

                    For i As Integer = 0 To shape.XMax
                        If (i < (astrEntered.Length - 1)) Then
                            Try
                                sValue = CSng(Val(astrEntered(i)))
                            Catch ex As Exception
                                sValue = -1
                            End Try
                        End If
                        asValues(i) = sValue
                    Next

                    shape.LockUpdates()
                    shape.ShapeData = asValues
                    shape.UnlockUpdates()

                End If
            End If
        End Sub

        Private Sub OnFReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbResetFs.Click

            Dim ts As cTimeSeries = Nothing

            ' JS 16May08: bypassed shape handler (which may be 0) to do a mass change
            Me.Core.FishingEffortShapeManager.ResetToDefaults()
            Me.Core.FishMortShapeManager.ResetToDefaults()

            ' JS 16Apr09: also disable time series
            For iTS As Integer = 1 To Me.Core.nTimeSeries
                Me.Core.EcosimTimeSeries(iTS).Enabled = False
            Next
            Me.Core.UpdateTimeSeries()

        End Sub

        Private Sub OnFZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSetTo0.Click
            If Me.m_shapeGUIHandler IsNot Nothing Then
                Me.m_shapeGUIHandler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset, _
                                                    New cShapeData() {Me.m_sketchPad.Shape}, 0.0!)
            End If
        End Sub

#End Region ' Forcing function

#End Region ' Events

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether plot is cumulative (True) or relative (False)
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property IsCumulativePlot() As Boolean
            Get
                Return Me.m_bIsCumulative
            End Get
            Set(ByVal value As Boolean)
                If (value = Me.m_bIsCumulative) Then Return

                Me.m_bIsCumulative = value
                Me.UpdateControls()
                Me.PopulateGraph()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether plot displays annual values.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property IsAnnualPlot() As Boolean
            Get
                Return Me.m_bIsAnnual
            End Get
            Set(ByVal value As Boolean)
                If (value = Me.m_bIsAnnual) Then Return

                Me.m_bIsAnnual = value
                Me.UpdateControls()
                Me.PopulateGraph()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the user is exploring values with the cursor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property IsExploring() As Boolean
            Get
                Return Me.m_bIsExploring
            End Get
            Set(ByVal value As Boolean)
                If (value = Me.m_bIsExploring) Then Return

                Me.m_bIsExploring = value

                ' Show or hide cursor
                Me.m_zgp.ShowCursor = Me.m_bIsExploring

                Me.UpdateControls()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set type of data to plot.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property PlotDataType() As ePlotData
            Get
                Return Me.m_varname
            End Get
            Set(ByVal value As ePlotData)
                If (value = Me.m_varname) Then Return

                Me.m_varname = value
                Me.m_bIsCumulative = False ' Switch to relative view
                Me.UpdateControls()
                Me.PopulateGraph()
            End Set
        End Property

        Private Sub PopulateRunsBox()

            Me.m_lbRuns.SuspendLayout()

            Me.m_lbRuns.Items.Clear()
            Me.m_lbRuns.Items.Add(My.Resources.GENERIC_VALUE_ALL)
            For iRun As Integer = 1 To Me.m_zgp.NumRuns
                Me.m_lbRuns.Items.Add(Me.m_zgp.RunLabel(iRun - 1))
            Next
            Me.m_lbRuns.SelectedIndex = 0
            Me.m_lbRuns.ResumeLayout()

        End Sub

        Private Sub PopulateGroupBox()

            Dim sSumDiscardsLandings As Double = 0.0
            Dim group As cCoreGroupBase = Nothing
            'Dim gi As cGroupListBox.cGroupItem = Nothing
            'Dim bIncludeGroup As Boolean = False
            Dim groupSelected As cCoreGroupBase = Nothing

            If (Me.m_lbGroups.SelectedIndex > 0) Then
                groupSelected = Me.m_lbGroups.SelectedGroup
            End If

            Me.m_lbGroups.SuspendLayout()

            Me.m_lbGroups.Sorted = False
            Me.m_lbGroups.ShowAllGroupsItem = True
            Me.m_lbGroups.Populate()

            If Me.m_lbGroups.SelectedItem Is Nothing Then
                Me.m_lbGroups.SelectedIndex = 0
            End If

            Me.m_lbGroups.Sorted = True
            Me.m_lbGroups.ResumeLayout()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the graph.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub PopulateGraph()

            Dim pplData As New PointPairList()
            Dim src As cEcosimGroupOutput = Nothing
            Dim dValue As Double = 0.0#
            Dim iGroup As Integer = 0
            Dim bIncludeDataPoint As Boolean = True
            Dim lLines As New List(Of LineItem)

            ' Safety checks
            If (Me.m_zgp Is Nothing) Then Return
            If (Not Me.m_zgp.isReady) Then Return
            If (Not Me.Core.StateMonitor.HasEcosimRan) Then Return

            ' Clear curves out of current run, if applicable
            Me.m_zgp.ResetRun()

            ' Set title
            Select Case Me.PlotDataType
                Case ePlotData.Biomass
                    If Me.m_bIsCumulative Then
                        m_zgp.DataName = My.Resources.HEADER_BIOMASS_CUMULATIVE
                    Else
                        m_zgp.DataName = My.Resources.HEADER_RELATIVEBIOMASS
                    End If
                Case ePlotData.GroupCatch
                    If Me.m_bIsCumulative Then
                        m_zgp.DataName = My.Resources.HEADER_CATCH_CUMULATIVE
                    Else
                        m_zgp.DataName = My.Resources.HEADER_RELATIVE_CATCH
                    End If
                Case Else
                    Debug.Assert(False, "Data " & Me.PlotDataType.ToString & " not supported by this graph")
            End Select

            ' For all groups in the group list box
            For iGroupItem As Integer = 1 To Me.m_lbGroups.Items.Count - 1
                ' Get actual group index
                iGroup = Me.m_lbGroups.GetGroupIndexAt(iGroupItem)
                ' Is a group?
                If (iGroup > 0) Then

                    ' Yes: Create data list
                    pplData = New PointPairList
                    pplData.Add(0, 1) ' Brute force to make 0 TS 1
                    src = Me.Core.EcoSimGroupOutputs(iGroup)

                    For iTimeStep As Integer = 1 To Core.nEcosimTimeSteps

                        dValue = CDbl(Me.GetEcosimValue(iGroup, iTimeStep))

                        ' Determine if datapoint should be displayed
                        bIncludeDataPoint = (Me.IsAnnualPlot = False) Or (iTimeStep Mod cCore.N_MONTHS = 0)

                        ' Should datapoint be displayed?
                        If bIncludeDataPoint Then
                            ' #Yes: display it
                            pplData.Add(CDbl(iTimeStep / cCore.N_MONTHS) + Core.EcosimFirstYear, dValue)
                        End If

                    Next iTimeStep

                    ' Add line
                    lLines.Add(Me.m_zgp.CreateLine(Me.Core.EcoPathGroupInputs(iGroup), pplData))

                End If

            Next iGroupItem

            Me.m_graph.GraphPane.XAxis.Scale.Min = Core.EcosimFirstYear
            Me.m_graph.GraphPane.XAxis.Scale.Max = Core.EcoSimModelParameters.NumberYears + Core.EcosimFirstYear

            ' Draw timeseries 
            Me.AddTimeSeriesLines(lLines)

            ' Calculate the Axis Scale Ranges
            Me.UpdateControls()
            Me.UpdateGraphHighlights()

            Me.m_zgp.PlotLines(lLines.ToArray(), 1, True, _
                               Me.m_zgp.ShowMultipleRuns = False, _
                               Me.IsCumulativePlot)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Place all available time series on the graph for the current data
        ''' plot type.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub AddTimeSeriesLines(ByVal lLines As List(Of LineItem))

            Dim ppl As New PointPairList()
            Dim ts As cTimeSeries = Nothing
            Dim gts As cGroupTimeSeries = Nothing
            Dim group As cEcoPathGroupInput = Nothing
            Dim StartBio As Single = 0.0!
            Dim EDataQ As Single = 0.0!

            ' Only plot time series for biomass 
            If (Me.PlotDataType <> ePlotData.Biomass) Then Return
            ' Only plot data when NOT showing cumulative data
            If (Me.IsCumulativePlot) Then Return

            ' For all time series
            For iTS As Integer = 1 To Core.nTimeSeries
                ' Get TS
                ts = Core.EcosimTimeSeries(iTS)
                ' Is ts usable?
                If ((ts.TimeSeriesType = eTimeSeriesType.BiomassRel) Or _
                    (ts.TimeSeriesType = eTimeSeriesType.BiomassAbs)) And _
                   (ts.Enabled = True) Then

                    ' Sanity check
                    Debug.Assert(TypeOf ts Is cGroupTimeSeries, "Relative Biomass TS should be cGroupTimeSeries object, check for import")

                    gts = DirectCast(ts, cGroupTimeSeries)
                    group = Me.Core.EcoPathGroupInputs(gts.GroupIndex)
                    ppl = New PointPairList()

                    'Scaling values for relative and actual observed biomass values (reference data)
                    'BiomassRel (relative value)scale values by exp(DataQ) DataQ = mle mean(sumof(log(observed/predicted))
                    'BiomassAbs (actual value) scale to relative [b(t)]/[b(0)] no statistical scaling
                    If (ts.TimeSeriesType = eTimeSeriesType.BiomassAbs) Then
                        'don't use the stat scaler for actual values
                        EDataQ = 1
                    Else
                        EDataQ = gts.eDataQ
                    End If
                    StartBio = Core.StartBiomass(gts.GroupIndex)

                    For iYear As Integer = 1 To Core.EcoSimModelParameters.NumberYears
                        If iYear < gts.ShapeData().Length Then
                            If gts.ShapeData(iYear) > 0 Then
                                ' Minus 1 because it should start with the first year
                                ppl.Add(iYear + Core.EcosimFirstYear - 0.5, (gts.ShapeData()(iYear) / EDataQ) / StartBio)
                            End If
                        End If
                    Next

                    ' Add line to graph.
                    lLines.Add(Me.m_zgp.CreateLine(gts, ppl, String.Format(sharedresources.GENERIC_LABEL_DETAILEDLABEL, ts.Name, group.Name)))
                End If
            Next iTS

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get an Ecosim value for a given group and time step.
        ''' </summary>
        ''' <param name="iGroup"></param>
        ''' <param name="iTimeStep"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function GetEcosimValue(ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Single

            Dim src As cEcosimGroupOutput = Me.Core.EcoSimGroupOutputs(iGroup)

            ' Get data point value
            Select Case Me.PlotDataType
                Case ePlotData.Biomass
                    Return CSng(IIf(Me.IsCumulativePlot, src.Biomass(iTimeStep), src.BiomassRel(iTimeStep)))
                Case ePlotData.GroupCatch
                    Return CSng(IIf(Me.IsCumulativePlot, src.Yield(iTimeStep), src.YieldRel(iTimeStep)))
            End Select

            Return cCore.NULL_VALUE

        End Function

        Private Sub SortGroupsAtTimestep(ByVal iTimeStep As Integer)

            Dim iGroup As Integer = 0
            Dim sValue As Single = 0.0

            If (Me.m_zgp.NumRuns < 1) Then Return
            Debug.Assert(Me.IsExploring = True)

            'Me.m_lbGroups.Sorted = False

            For i As Integer = 0 To Me.m_lbGroups.Items.Count
                iGroup = Me.m_lbGroups.GetGroupIndexAt(i)
                If (iGroup > 0) Then
                    ' Grab value from data
                    'sValue = Me.GetDataPoint(iGroup, iTimeStep)
                    sValue = CSng(Me.m_zgp.GetValueAt(iGroup, Me.m_zgp.NumRuns - 1, iTimeStep))
                    ' Set sort value
                    If Me.IsCumulativePlot Then
                        ' Set this to sort value
                        Me.m_lbGroups.SortValue(iGroup) = sValue
                    Else
                        ' Set this to sort value
                        Me.m_lbGroups.SortValue(iGroup) = CSng(Math.Abs(sValue - 1.0))
                    End If
                End If
            Next

            'Me.m_lbGroups.Sorted = True

            Me.m_lbGroups.Refresh()

        End Sub

        Private Function GetSelectedGroupOrFleet() As ICoreInterface
            Dim tv As cCustomComboBoxFleetGroupTree = DirectCast(Me.tscbTarget.DropdownControl, cCustomComboBoxFleetGroupTree)
            Return tv.SelectedItem()
        End Function

        ''' <summary>
        ''' Load fishing effort data from the Fishing Rate manager 
        ''' </summary>
        Private Sub LoadFishingRateShape()

            Dim item As ICoreInterface = Me.GetSelectedGroupOrFleet()

            If (Not TypeOf Me.m_shapeGUIHandler Is cFishingEffortShapeGUIHandler) Then
                If (Not Me.m_shapeGUIHandler Is Nothing) Then
                    Me.m_shapeGUIHandler.Detach()
                    Me.m_shapeGUIHandler = Nothing
                End If
                Me.m_shapeGUIHandler = New cFishingEffortShapeGUIHandler()
                Me.m_shapeGUIHandler.Attach(Me.UIContext, Nothing, Nothing, Me.m_sketchPad, Nothing)
            End If

            Me.m_shapeGUIHandler.SelectedShape = DirectCast(item, cFishingRateShape)
            Me.m_sketchPad.Editable = True
            Me.m_bIsEffortSelected = True
            Me.UpdateControls()

        End Sub

        'Fish Rate (Y/B)
        Private Sub LoadFishMortShape()

            Dim item As ICoreInterface = Me.GetSelectedGroupOrFleet()
            Dim shape As cShapeData = Nothing

            ' Mortality shapes are 0-base indexed, groups are 1-base indexed
            shape = Core.FishMortShapeManager.Item(item.Index - 1)

            If (Not TypeOf Me.m_shapeGUIHandler Is cFishingMortalityShapeGUIHandler) Then
                If (Not Me.m_shapeGUIHandler Is Nothing) Then
                    Me.m_shapeGUIHandler.Detach()
                    Me.m_shapeGUIHandler = Nothing
                End If
                Me.m_shapeGUIHandler = New cFishingMortalityShapeGUIHandler()
                Me.m_shapeGUIHandler.Attach(Me.UIContext, Nothing, Nothing, Me.m_sketchPad, Nothing)
            End If

            Me.m_shapeGUIHandler.SelectedShape = shape
            ' Cannot edit Fs anymore
            Me.m_sketchPad.Editable = False
            Me.m_bIsEffortSelected = False
            Me.UpdateControls()
        End Sub

        Private Sub ClearShape()
            Me.m_sketchPad.Shape = Nothing
            Me.UpdateControls()
        End Sub

        Private Property SelectionMode() As eSelectionModeType
            Get
                Return Me.m_selectionMode
            End Get
            Set(ByVal value As eSelectionModeType)
                Me.m_selectionMode = value
                Me.UpdateControls()
            End Set
        End Property

        Private Sub UpdateControls()

            If (Me.m_zgp Is Nothing) Then Return

            Me.m_bInUpdate = True

            ' Configure run/stop button
            Me.btnRunOrStop.Text = CStr(IIf(Me.m_bEcosimRunning, My.Resources.LABEL_STOP, My.Resources.LABEL_RUN))
            Me.btnRunOrStop.Enabled = Me.Core.StateMonitor.HasEcosimLoaded
            ' Reflect change immediately
            Me.btnRunOrStop.Update()

            ' Reset buttons
            Me.tsbSetToValue.Enabled = (Me.m_sketchPad.Shape IsNot Nothing)
            Me.tsbSetTo0.Enabled = (Me.m_sketchPad.Shape IsNot Nothing)
            Me.tsbResetFs.Enabled = True

            Me.m_tsmiCumulative.Checked = (Me.m_bIsCumulative = True)
            Me.m_tsmiRelative.Checked = (Me.m_bIsCumulative = False)
            Me.m_tsmiShowAnnualOutput.Checked = Me.m_bIsAnnual

            Me.m_tsmiBiomass.Checked = (Me.PlotDataType = ePlotData.Biomass)
            Me.m_tsmiCatch.Checked = (Me.PlotDataType = ePlotData.GroupCatch)

            Me.m_tsmiAutoscale.Checked = (Me.m_zgp.AutoScaleYOption = cZedGraphHelper.eScaleOptionTypes.MaxOnly)
            Me.m_tsmiCustomScaleLabel.Checked = Not m_tsmiAutoscale.Checked
            Me.m_tstbMax.Text = CStr(Me.m_zgp.YScaleMax)
            Me.m_tstbMin.Text = CStr(Me.m_zgp.YScaleMin)

            If Me.IsExploring Then
                Me.m_lbGroups.SortThreshold = Me.m_sChangeTrackSize
                Me.m_lbGroups.SortType = cGroupListBox.eSortType.ValueDesc
                Me.m_tsmiSortMostChanged.Checked = True
            Else
                Me.m_lbGroups.SortThreshold = cCore.NULL_VALUE
                Me.m_lbGroups.SortType = cGroupListBox.eSortType.GroupIndexAsc
                Me.m_tsmiSortMostChanged.Checked = False
            End If

            Me.m_scOptions.Panel1Collapsed = Not Me.m_tsmShowMultipleRuns.Checked
            Me.m_tstbChangeAmount.Text = CStr(Me.m_sChangeTrackSize)

            Me.tsbSetToValue.Enabled = Me.m_bIsEffortSelected
            Me.tsbSetTo0.Enabled = Me.m_bIsEffortSelected

            Me.m_bInUpdate = False

        End Sub

        Public Sub ResetGraph()
            Me.m_zgp.Clear()
            Me.PopulateRunsBox()
            Me.PopulateGroupBox()
        End Sub

        ''' <summary>
        ''' Highlight selected groups
        ''' </summary>
        Private Sub UpdateGraphHighlights()

            Dim iItem As Integer = 0
            Dim iGroup As Integer = 0
            Dim iRun As Integer = 0

            Me.m_zgp.ClearHighlights()
            For Each iRun In Me.m_lbRuns.SelectedIndices
                For Each iItem In Me.m_lbGroups.SelectedIndices
                    iGroup = Math.Max(0, Me.m_lbGroups.GetGroupIndexAt(iItem))
                    Me.m_zgp.Highlight(iGroup, iRun - 1)
                Next
            Next iRun

            Me.m_graph.Invalidate()

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
