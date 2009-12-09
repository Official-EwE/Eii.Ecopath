#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports SAUPUtil.SAUPData.Mapping
Imports EwEUtils.Core
Imports ScientificInterface.Other
Imports Microsoft.VisualBasic
Imports ScientificInterfaceShared
Imports ZedGraph
Imports EwEUtils.Commands

#End Region

Namespace Ecosim

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RunEcosim

        Private Enum eSelectionModeType
            NotSet = 0
            Fleets
            Groups
        End Enum

        Private m_selectionMode As eSelectionModeType = eSelectionModeType.NotSet

#Region " Variables "

        Private m_coreStateMonitor As cCoreStateMonitor = Nothing
        Private m_core As cCore = Nothing
        Private m_shapeGUIHandler As cForcingShapeGUIHandler = Nothing
        Private m_params As cEcoSimModelParameters = Nothing
        Private m_iTimeSteps As Integer = 0
        Private m_sChangeTrackSize As Single = 0.1!
        Private m_zgp As cEcosimOutputPlotHelper = Nothing
        Private m_sg As cStyleGuide = cStyleGuide.GetInstance()
        ''' <summary>
        ''' True when this interface is running ecosim. False otherwise
        ''' </summary>
        ''' <remarks>This is to stop this interface from responding to Ecosim messages if it did not start the ecosim run </remarks>
        Private m_bEcosimRunning As Boolean = False

        Private m_simStats As cEcosimStats

        Private m_bInUpdate As Boolean = False

        Private m_ccb As cCustomComboBoxFleetGroupTree = Nothing

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance()
            Me.m_coreStateMonitor = Me.m_core.StateMonitor
            Me.m_params = m_core.EcoSimModelParameters()
            Me.m_simStats = Me.m_core.EcosimStats

        End Sub

#End Region ' Constructors

#Region " Framework overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmd As cCommand = Nothing

            If cmdh Is Nothing Then Return

            Me.m_ccb = New cCustomComboBoxFleetGroupTree(Me.m_core, Me.tscbTarget)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.ShapesManager}

            Me.m_lbGroups.Attach(Me.m_core, Me.m_sg)

            Me.m_zgp = New cEcosimOutputPlotHelper()
            Me.m_zgp.Attach(Me.m_core, Me.m_graph)

            Me.m_zgp.ConfigurePane(My.Resources.HEADER_RELATIVEBIOMASS, My.Resources.HEADER_YEAR, My.Resources.HEADER_RELATIVEBIOMASS, False)
            Me.m_zgp.ShowMultipleRuns = Me.m_tsmShowMultipleRuns.Selected

            Me.m_zgp.AutoScaleOption = cZedGraphHelper.ScaleOptions.Both
            Me.m_zgp.YScaleMin = 0.0!
            Me.m_zgp.ShowPointValue = True

            ' Set the axis
            Me.m_graph.GraphPane.XAxis.Scale.Min = m_core.EcosimFirstYear
            Me.m_graph.GraphPane.XAxis.Scale.Max = m_core.EcoSimModelParameters.NumberYears + m_core.EcosimFirstYear
            Me.m_graph.AxisChange()

            AddHandler Me.m_zgp.OnCursorPos, AddressOf OnSyncCursor

            Me.UpdateControls()

            ' Display Groups
            cmd = cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.AddControl(Me.m_tsbtnShowHideGroups)
            End If

            ' Track core monitor changes
            AddHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            Me.PopulateGraph()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            RemoveHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            ' Show/Hide Groups
            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmd As cCommand = cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.RemoveControl(Me.m_tsbtnShowHideGroups)
            End If

            Me.m_lbGroups.Detach()

            Me.m_coreStateMonitor = Nothing
            Me.CoreComponents = Nothing

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

#End Region ' Framework overrides

#Region " Events "

#Region " Controls "

        Private Sub btnRunOrStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnRunOrStop.Click

            If Not Me.m_bEcosimRunning Then
                Me.m_iTimeSteps = Me.m_core.nEcosimTimeSteps
                Me.m_graph.Refresh()
                Me.m_core.RunEcoSim(AddressOf TimeStepFromEcoSim_handler)
            Else
                Me.m_core.StopEcoSim()
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
            Me.m_tsmiShowAnnualOutput.Checked = Not Me.m_tsmiShowAnnualOutput.Checked
            Me.m_zgp.Clear()
            Me.PopulateGroupBox()
            Me.m_zgp.RescaleAndRedraw()
        End Sub

        Private Sub ShowLegendToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiShowLegend.Click
            Me.m_tsmiShowLegend.Checked = Not Me.m_tsmiShowLegend.Checked
            Me.m_zgp.ShowLegend = Me.m_tsmiShowLegend.Checked
            Me.m_zgp.RescaleAndRedraw()
        End Sub

        Private Sub CumulativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiCumulative.Click
            Me.m_tsmiRelative.Checked = Not Me.m_tsmiCumulative.Checked
            Me.PopulateGraph()
        End Sub

        Private Sub RelativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiRelative.Click
            Me.m_tsmiCumulative.Checked = Not Me.m_tsmiRelative.Checked
            Me.PopulateGraph()
        End Sub

        Private Sub BiomassToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiBiomass.Click
            Me.m_tsmiCatch.Checked = Not Me.m_tsmiBiomass.Checked
            'Set default plot type to relative
            Me.m_tsmiRelative.Checked = True
            Me.PopulateGroupBox()
            Me.PopulateGraph()
        End Sub

        Private Sub CatchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiCatch.Click
            Me.m_tsmiBiomass.Checked = Not Me.m_tsmiCatch.Checked
            'Set default plot type to relative
            Me.m_tsmiRelative.Checked = True
            Me.PopulateGroupBox()
            Me.PopulateGraph()
        End Sub

        Private Sub AutoScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiAutoscale.Click
            Me.m_zgp.AutoScaleOption = cZedGraphHelper.ScaleOptions.MaxOnly
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
            Me.m_zgp.AutoScaleOption = cZedGraphHelper.ScaleOptions.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSetMax_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbMax.LostFocus
            Double.TryParse(TryCast(sender, ToolStripTextBox).Text, Me.m_zgp.YScaleMax)
            Me.m_zgp.AutoScaleOption = cZedGraphHelper.ScaleOptions.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSetMin_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbMin.LostFocus
            Double.TryParse(TryCast(sender, ToolStripTextBox).Text, Me.m_zgp.YScaleMin)
            Me.m_zgp.AutoScaleOption = cZedGraphHelper.ScaleOptions.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tsmiSortMostChanged_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiSortMostChanged.Click, m_tssbExplore.Click

            ' Show or hide cursor
            Me.m_zgp.ShowCursor = Not Me.m_zgp.ShowCursor
            Me.m_tsmiSortMostChanged.Checked = Me.m_zgp.ShowCursor
            Me.UpdateControls()

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
            If Me.m_tsmiSortMostChanged.Checked Then
                Me.SortGroupsAtTimestep(CInt(Math.Round(sPos * cCore.N_MONTHS)))
            End If
        End Sub

#End Region ' Controls

#Region " Core "

        Private Sub TimeStepFromEcoSim_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)

            Dim asn As cApplicationStatusNotifier = cApplicationStatusNotifier.GetInstance()
            asn.SetStatusText(My.Resources.STATUS_ECOSIM_RUNNING, TriState.UseDefault, CSng(iTime / m_iTimeSteps))

        End Sub

        Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)

            ' Could be that we're closing
            If (Me.IsDisposed) Then Return

            Dim bEcosimRunning As Boolean = m_coreStateMonitor.IsEcosimRunning
            Dim bHasEcosimResults As Boolean = m_coreStateMonitor.HasEcosimRan
            Dim strRunLabel As String = String.Format(My.Resources.ECOSIM_LABEL_RUN, (Me.m_zgp.NumRuns + 1))

            ' Does not have ecosim results?
            If (Not m_coreStateMonitor.HasEcopathRan) Then
                ' #Yes: clear run results
                Me.ResetGraph()
            End If

            ' Check whether ecosim is running
            ' Is this a state change?
            If (bEcosimRunning <> Me.m_bEcosimRunning) Then
                ' #Yes: update to new state
                Me.m_bEcosimRunning = bEcosimRunning
                If Me.m_bEcosimRunning Then
                    AppLauncher.GetInstance().SetStatusText(My.Resources.STATUS_ECOSIM_RUNNING, TriState.True, 0)
                Else
                    AppLauncher.GetInstance().SetStatusText("", TriState.False, 0)
                    Me.m_zgp.CreateRun(strRunLabel)
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
                            Me.tslblSSValue.Text = cStyleGuide.GetInstance().FormatNumber(Me.m_core.EcosimStats.SS)
                            Me.m_iTimeSteps = 0
                        End If

                        ' Plot the graph
                        Me.PopulateGraph()

                    Case eMessageType.EcosimNYearsChanged

                        'set the xaxis this is the number of time steps the model will run for
                        'm_ucBPlots.Plot.XAxis = m_Core.nEcosimTimeSteps
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
            Me.m_core.FishingEffortShapeManager.ResetToDefaults()
            Me.m_core.FishMortShapeManager.ResetToDefaults()

            ' JS 16Apr09: also disable time series
            For iTS As Integer = 1 To Me.m_core.nTimeSeries
                Me.m_core.EcosimTimeSeries(iTS).Enabled = False
            Next
            Me.m_core.UpdateTimeSeries()

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

            'Me.m_lbGroups.Items.Clear()
            'Me.m_lbGroups.Items.Add(New cGroupListBox.cGroupItem(Nothing))

            'For iGroup As Integer = 1 To m_core.nGroups

            '    ' Include visible groups only
            '    bIncludeGroup = Me.m_sg.GroupVisible(iGroup)

            '    ' Displaying catch and discards?
            '    If m_tsmiCatch.Checked Then

            '        ' Get sum of landings and discards for this group
            '        sSumDiscardsLandings = 0
            '        For f As Integer = 1 To m_core.nFleets
            '            sSumDiscardsLandings += (Me.m_core.FleetInputs(f).Discards(iGroup) + Me.m_core.FleetInputs(f).Landings(iGroup))
            '        Next f

            '        ' Include when group has landings and/or discards
            '        bIncludeGroup = bIncludeGroup And (sSumDiscardsLandings > 0)
            '    End If

            '    ' Include group?
            '    If bIncludeGroup Then
            '        ' #Yes: add group to the list of options
            '        group = Me.m_core.EcoPathGroupInputs(iGroup)
            '        gi = New cGroupListBox.cGroupItem(group)
            '        Me.m_lbGroups.Items.Add(gi)

            '        If Object.ReferenceEquals(group, groupSelected) Then
            '            Me.m_lbGroups.SelectedItem = gi
            '        End If
            '    End If

            'Next

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
        Public Sub PopulateGraph()

            Dim pplData As New PointPairList()
            Dim pplSum As New PointPairList()

            ' Safety check
            If Me.m_zgp Is Nothing Then Return

            'jb added if ecosim has not run then the Ecosim EcoSimGroupOutputs will not be populated and can not be plotted
            If Not Me.m_core.StateMonitor.HasEcosimRan Then
                Return
            End If

            ' Clear curves out of current run, if applicable
            Me.m_zgp.ResetRun()

            If Not Me.m_zgp.isReady Then
                'The graph has not been initialized don't try to draw the data
                'this can happen is some other process ran Ecosim and PopulateGraph() gets called in response
                Return
            End If

            ' === Cumulative plot ===

            If m_tsmiCumulative.Checked Then
                If m_tsmiBiomass.Checked Then
                    'Biomass
                    m_zgp.DataName = My.Resources.HEADER_BIOMASS_CUMULATIVE
                ElseIf m_tsmiCatch.Checked Then
                    'Catch
                    m_zgp.DataName = My.Resources.HEADER_CATCH_CUMULATIVE
                Else
                    Debug.Assert(False)
                End If

                'Initialize listSum.Y=0
                pplSum.Add(0, 0)
                For t As Integer = 1 To m_core.nEcosimTimeSteps
                    If m_tsmiShowAnnualOutput.Checked Then
                        If t Mod cCore.N_MONTHS = 0 Then
                            pplSum.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, 0.0)
                        End If
                    Else
                        ' 2) Add a single point to temp list
                        pplSum.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, 0.0)
                    End If
                Next t

                For iLBItem As Integer = 1 To Me.m_lbGroups.Items.Count - 1

                    'Dim i As Integer = DirectCast(Me.m_lbGroups.Items(iLBItem), cGroupListBox.cGroupItem).Group.Index
                    Dim i As Integer = Me.m_lbGroups.GetGroupIndexAt(iLBItem)

                    'Catch
                    If m_tsmiCatch.Checked Then
                        'Find the sum of discard and landing of the group
                        Dim dblSumDiscardsLandings As Double = 0.0
                        For f As Integer = 1 To m_core.nFleets
                            dblSumDiscardsLandings = dblSumDiscardsLandings + m_core.FleetInputs(f).Discards(i) + m_core.FleetInputs(f).Landings(i)
                        Next f
                        'If sum=0 then skip this group
                        If Not dblSumDiscardsLandings > 0 Then Continue For
                    End If

                    pplData = New PointPairList
                    If m_tsmiBiomass.Checked Then
                        'Biomass
                        pplData.Add(0, m_core.EcoPathGroupOutputs(i).Biomass())
                    ElseIf m_tsmiCatch.Checked Then
                        'Catch
                        pplData.Add(0, m_core.EcoPathGroupOutputs(i).Biomass() * m_core.EcoPathGroupOutputs(i).MortCoFishRate())
                    Else
                        Debug.Assert(False)
                    End If

                    For t As Integer = 1 To m_core.nEcosimTimeSteps
                        If m_tsmiShowAnnualOutput.Checked Then
                            If t Mod cCore.N_MONTHS = 0 Then
                                If m_tsmiBiomass.Checked Then
                                    'Biomass
                                    pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t)))
                                ElseIf m_tsmiCatch.Checked Then
                                    'Catch
                                    'jb changed to use Yield computed by Ecosim 
                                    'pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) * _
                                    '  (m_core.EcoSimGroupOutputs(i).FishMort(t) - m_core.EcoSimGroupOutputs(i).PredMort(t))))
                                    pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Yield(t)))

                                Else
                                    Debug.Assert(False)
                                End If
                            End If
                        Else
                            ' 2) Add a single point to temp list
                            If m_tsmiBiomass.Checked Then
                                'Biomass
                                pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t)))
                            ElseIf m_tsmiCatch.Checked Then
                                'Catch
                                pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Yield(t)))
                            Else
                                Debug.Assert(False)
                            End If
                        End If
                    Next t

                    'listSum=listSum+list1
                    For j As Integer = 0 To pplSum.Count - 1
                        pplSum(j).Y = pplSum(j).Y + pplData(j).Y
                    Next

                    For j As Integer = 0 To pplSum.Count - 1
                        pplData(j).Y = pplSum(j).Y
                    Next

                    ' 3) Store the line
                    If m_tsmiBiomass.Checked Then
                        'Biomass
                        If m_tsmiCumulative.Checked Then
                            'Cumulative highlight
                            m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, _
                                          i, cEcosimOutputPlotHelper.eLineType.CumulativeBiomass, pplData)
                        Else
                            'Cumulative selected
                            m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, _
                                          i, cEcosimOutputPlotHelper.eLineType.CumulativeSelectedBiomass, pplData)
                        End If
                    ElseIf m_tsmiCatch.Checked Then
                        'Catch
                        m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, _
                                      i, cEcosimOutputPlotHelper.eLineType.CumulativeCatch, pplData)
                    Else
                        Debug.Assert(False)
                    End If

                Next
            End If

            ' === Relative plot ===

            If m_tsmiRelative.Checked Then
                If m_tsmiBiomass.Checked Then
                    'Biomass
                    m_zgp.DataName = My.Resources.HEADER_RELATIVEBIOMASS
                ElseIf m_tsmiCatch.Checked Then
                    'Catch
                    m_zgp.DataName = My.Resources.HEADER_RELATIVE_CATCH
                Else
                    Debug.Assert(False)
                End If

                ' todo: change to groups that listed in group box
                For j As Integer = 1 To Me.m_lbGroups.Items.Count - 1

                    'Dim i As Integer = DirectCast(Me.m_lbGroups.Items(j), cGroupListBox.cGroupItem).Group.Index
                    Dim i As Integer = Me.m_lbGroups.GetGroupIndexAt(j)

                    If (i > -1) Then

                        ' No need to check; group would not be available otherwise

                        ''Catch
                        'If CatchToolStripMenuItem.Checked Then
                        '    'Find the sum of discard and landing of the group
                        '    Dim dblSumDiscardsLandings As Double = 0.0
                        '    For f As Integer = 1 To m_core.nFleets
                        '        dblSumDiscardsLandings = dblSumDiscardsLandings + m_core.FleetInputs(f).Discards(i) + m_core.FleetInputs(f).Landings(i)
                        '    Next f
                        '    'If sum=0 then skip this group
                        '    If Not dblSumDiscardsLandings > 0 Then Continue For
                        'End If

                        pplData = New PointPairList
                        pplData.Add(0, 1) ' Brute force to make 0 TS 1
                        For t As Integer = 1 To m_core.nEcosimTimeSteps
                            If m_tsmiShowAnnualOutput.Checked Then
                                If t Mod cCore.N_MONTHS = 0 Then



                                    If m_tsmiBiomass.Checked Then
                                        'Biomass
                                        pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).BiomassRel(t)))
                                    ElseIf m_tsmiCatch.Checked Then
                                        'Catch
                                        pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).YieldRel(t)))
                                    Else
                                        Debug.Assert(False)
                                    End If

                                End If
                            Else

                                ' 2) Add a single point to temp list
                                If m_tsmiBiomass.Checked Then
                                    'Biomass
                                    pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).BiomassRel(t)))
                                ElseIf m_tsmiCatch.Checked Then
                                    'Catch
                                    'jb changed to use relative values computed by Ecosim original code left for reference until this has been vetted
                                    'pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) * _
                                    '    (m_core.EcoSimGroupOutputs(i).FishMort(t) - m_core.EcoSimGroupOutputs(i).PredMort(t)) / (m_core.EcoPathGroupOutputs(i).Biomass() * m_core.EcoPathGroupOutputs(i).MortCoFishRate())))
                                    pplData.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).YieldRel(t)))
                                Else
                                    Debug.Assert(False)
                                End If

                            End If
                        Next t

                        ' 3) Store the line
                        If m_tsmiBiomass.Checked Then
                            'Biomass
                            m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, _
                                          i, cEcosimOutputPlotHelper.eLineType.RelativeBiomass, pplData)
                        ElseIf m_tsmiCatch.Checked Then
                            'Catch
                            m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, _
                                          i, cEcosimOutputPlotHelper.eLineType.RelativeCatch, pplData)
                        Else
                            Debug.Assert(False)
                        End If

                    End If

                Next j
            End If

            Me.m_graph.GraphPane.XAxis.Scale.Min = m_core.EcosimFirstYear
            Me.m_graph.GraphPane.XAxis.Scale.Max = m_core.EcoSimModelParameters.NumberYears + m_core.EcosimFirstYear

            ' Draw timeseries 
            Me.PopulateGraphTimeSeries()

            ' Calculate the Axis Scale Ranges
            Me.UpdateControls()
            Me.UpdateGraphHighlights()
            Me.m_zgp.RescaleAndRedraw()

        End Sub

        Public Sub PopulateGraphTimeSeries()

            Dim sg As cStyleGuide = cStyleGuide.GetInstance()
            Dim ppl As New PointPairList()
            Dim ts As cTimeSeries = Nothing

            If (Me.m_tsmiBiomass.Checked = False) Then Return
            If (Me.m_tsmiRelative.Checked = False) Then Return

            For i As Integer = 1 To m_core.nTimeSeries
                ts = m_core.EcosimTimeSeries(i)
                If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                    If TypeOf ts Is cGroupTimeSeries Then
                        Dim gts As cGroupTimeSeries = DirectCast(ts, cGroupTimeSeries)
                        If gts.Enabled() Then
                            'm_abHasTSData(gts.GroupIndex) = True
                            Dim da() As Single = gts.ShapeData()
                            Dim group As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(gts.GroupIndex)
                            ppl = New PointPairList

                            'Scaling values for relative and actual observed biomass values (reference data)
                            'BiomassRel (relative value)scale values by exp(DataQ) DataQ = mle mean(sumof(log(observed/predicted))
                            'BiomassAbs (actual value) scale to relative [b(t)]/[b(0)] no statistical scaling
                            Dim startBio As Single = m_core.StartBiomass(gts.GroupIndex)
                            Dim eDataQ As Single = gts.eDataQ
                            If ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                                'don't use the stat scaler for actual values
                                eDataQ = 1
                            End If

                            For j As Integer = 1 To m_core.EcoSimModelParameters.NumberYears
                                If j < da.Length Then
                                    If da(j) > 0 Then
                                        ' Minus 1 because it should start with the first year
                                        ppl.Add(j + m_core.EcosimFirstYear - 1, (da(j) / eDataQ) / startBio)
                                    End If
                                End If
                            Next

                            ' Add line to graph.
                            m_zgp.AddLine(String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, ts.Name, group.Name), _
                                          gts.GroupIndex, cEcosimOutputPlotHelper.eLineType.TimeSeries, ppl)

                        End If

                    Else
                        Debug.Assert(True, "Relative Biomass TS should be cGroupTimeSeries object, check for import")
                    End If
                End If

            Next
        End Sub

        Private Sub SortGroupsAtTimestep(ByVal iTimeStep As Integer)

            Dim iGroup As Integer = 0
            Dim sValue As Single = 0.0

            If Me.m_zgp.NumRuns < 1 Then Return

            Me.m_lbGroups.SortThreshold = Me.m_sChangeTrackSize

            For i As Integer = 0 To Me.m_lbGroups.Items.Count
                iGroup = Me.m_lbGroups.GetGroupIndexAt(i)
                If (i > 0) Then
                    sValue = CSng(Me.m_zgp.GetValueAt(iGroup, Me.m_zgp.NumRuns - 1, iTimeStep))
                    ' ToDo: Handle value depending on what is being displayed
                    Me.m_lbGroups.SortValue(iGroup) = CSng(Math.Abs(sValue - 1.0))
                End If
            Next

            Me.m_lbGroups.Sorted = (Me.m_zgp.ShowCursor = True)
            Me.m_lbGroups.Refresh()

        End Sub

        Private Function GetSelectedGroupOrFleet() As ICoreInterface
            Dim tv As cCustomComboBoxFleetGroupTree = DirectCast(Me.tscbTarget.DropdownControl, cCustomComboBoxFleetGroupTree)
            Return tv.SelectedItem()
        End Function

        ''' <summary>
        ''' Load fishing effort data from the Fishing Rate manager 
        ''' </summary>
        ''' <remarks>Right now, it is zero based</remarks>
        Private Sub LoadFishingRateShape()
            Dim item As ICoreInterface = Me.GetSelectedGroupOrFleet()

            Me.m_shapeGUIHandler = New cFishingEffortShapeGUIHandler(Me.m_core, Nothing, Me.m_sketchPad)
            Me.m_shapeGUIHandler.SelectedShape = DirectCast(item, cFishingRateShape)
            Me.m_sketchPad.Style = cStyleGuide.eStyleFlags.OK
            Me.UpdateControls()
        End Sub

        'Fish Rate (Y/B)
        Private Sub LoadFishMortShape()
            Dim item As ICoreInterface = Me.GetSelectedGroupOrFleet()
            Dim shape As cShapeData = Nothing

            ' Mortality shapes are 0-base indexed, groups are 1-base indexed
            shape = m_core.FishMortShapeManager.Item(item.Index - 1)

            Me.m_shapeGUIHandler = New cFishingMortalityShapeGUIHandler(Me.m_core, Nothing, Me.m_sketchPad)
            Me.m_shapeGUIHandler.SelectedShape = shape
            ' Cannot edit Fs anymore
            Me.m_sketchPad.Style = cStyleGuide.eStyleFlags.NotEditable
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

            ' Configure run/stop button
            Me.btnRunOrStop.Text = CStr(IIf(Me.m_bEcosimRunning, My.Resources.LABEL_STOP, My.Resources.LABEL_RUN))
            Me.btnRunOrStop.Enabled = Me.m_coreStateMonitor.HasEcosimLoaded
            ' Reflect change immediately
            Me.btnRunOrStop.Update()

            ' Reset buttons
            Me.tsbSetToValue.Enabled = (Me.m_sketchPad.Shape IsNot Nothing)
            Me.tsbSetTo0.Enabled = (Me.m_sketchPad.Shape IsNot Nothing)
            Me.tsbResetFs.Enabled = True

            If Me.m_zgp Is Nothing Then Return

            Me.m_tsmiAutoscale.Checked = (Me.m_zgp.AutoScaleOption = cZedGraphHelper.ScaleOptions.MaxOnly)
            Me.m_tsmiCustomScaleLabel.Checked = Not m_tsmiAutoscale.Checked
            Me.m_tstbMax.Text = CStr(Me.m_zgp.YScaleMax)
            Me.m_tstbMin.Text = CStr(Me.m_zgp.YScaleMin)

            If Me.m_tsmiSortMostChanged.Checked Then
                Me.m_lbGroups.SortType = cGroupListBox.eSortType.ValueDesc
            Else
                Me.m_lbGroups.SortThreshold = cCore.NULL_VALUE
                Me.m_lbGroups.SortType = cGroupListBox.eSortType.GroupIndexAsc
            End If

            Me.m_scOptions.Panel1Collapsed = Not Me.m_tsmShowMultipleRuns.Checked
            Me.m_tstbChangeAmount.Text = CStr(Me.m_sChangeTrackSize)

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
                'For Each groupitem As Object In Me.m_lbGroups.SelectedItems
                '    If TypeOf (groupitem) Is cGroupListBox.cGroupItem Then
                '        gi = DirectCast(groupitem, cGroupListBox.cGroupItem)
                '        If (gi.Group IsNot Nothing) Then
                '            iGroup = gi.Group.Index
                '        Else
                '            iGroup = 0
                '        End If
                '        Me.m_zgp.Highlight(iGroup, iRun - 1)
                '    End If
                'Next groupitem
            Next iRun

            Me.m_graph.Invalidate()

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
