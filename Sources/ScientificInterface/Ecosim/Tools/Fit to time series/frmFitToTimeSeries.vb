'==============================================================================
'
' $Log: frmFitToTimeSeries.vb,v $
' Revision 1.13  2009/05/11 01:50:56  jeroens
' Renamed command classes
'
' Revision 1.12  2009/04/24 16:04:19  jeroens
' Anomaly search added year check
'
' Revision 1.11  2009/04/24 15:47:36  jeroens
' Anomaly search will not run if no FF selected
'
' Revision 1.10  2009/04/23 23:06:16  joeb
' Fix bug when updating from AnomalySearch() shape was null
'
' Revision 1.9  2009/03/23 20:19:58  jeroens
' Fixed InitializeComponent crash, ugh
'
' Revision 1.8  2009/03/20 17:55:40  jeroens
' Shape controls are multiple selection
'
' Revision 1.7  2009/03/02 01:52:35  jeroens
' Properly named handlers
'
' Revision 1.6  2009/02/05 17:48:38  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.5  2009/01/16 18:30:41  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.4  2008/12/15 16:01:38  jeroens
' no message
'
' Revision 1.3  2008/11/27 20:56:11  sherman
' Switched MaxFishing Mortality to Search routines.
'
' Revision 1.2  2008/11/26 23:19:52  jeroens
' Weight! Weight, dude
'
' Revision 1.1  2008/11/19 14:40:55  jeroens
' Moved and renamed
'
' Revision 1.1  2008/09/26 07:31:51  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "
Option Explicit On
Option Strict On

Imports EwECore
Imports EwECore.FitToTimeSeries
Imports EwEUtils.Commands
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    Public Class frmFitToTimeSeries

#Region " Private variables "

        Private m_core As EwECore.cCore = EwECore.cCore.GetInstance

        Private m_F2TSManager As cF2TSManager = Nothing
        Private m_shapeHandler As AppliedFFGUIHandler = Nothing
        Private m_dlgSensOfSS As dlgSensitivityOfSStoV = Nothing
        Private m_SensitivityByPredatorResults As cSensitivityToVulResults = Nothing
        Private m_cmdTSWeights As cCommand = Nothing
        Private m_gridGroupMaxFishingMortality As gridFitToTimeSeriesGroup = Nothing

#End Region 'Private variables

#Region " Helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class AppliedFFGUIHandler
            Inherits cForcingShapeGUIHandler

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="core"></param>
            ''' <param name="stb"></param>
            ''' <param name="sp"></param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal core As cCore, ByVal stb As ucShapeToolbox, ByVal sp As ucSketchPad)
                MyBase.New(core, stb, Nothing, sp, Nothing)
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Overrides Property SketchPad() As ucSketchPad
                Get
                    Return MyBase.SketchPad
                End Get
                Set(ByVal value As ucSketchPad)
                    MyBase.SketchPad = value

                    If value IsNot Nothing Then
                        ' No doodling allowed here
                        value.Editable = False
                    End If
                End Set
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="shape"></param>
            ''' <returns></returns>
            ''' ---------------------------------------------------------------
            Protected Overrides Function IncludeShape(ByVal shape As EwECore.cShapeData) As Boolean
                Dim ppi As cPPIManager = Me.m_core.PPInteractionManager
                If Not (TypeOf shape Is cForcingFunction) Then Return False
                If (ppi Is Nothing) Then Return False
                Return ppi.IsApplied(DirectCast(shape, cForcingFunction))
            End Function

        End Class

#End Region ' Helper clases

#Region " Constructor "

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Private form event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            Me.m_F2TSManager = Me.m_core.EcosimFitToTimeSeries
            Me.m_cbAnomalySearch.Checked = Me.m_F2TSManager.AnomalySearch
            Me.m_cbVulnerabilitySearch.Checked = Me.m_F2TSManager.VulnerabilitySearch
            Me.m_cbFishingMortalityPenalty.Checked = Me.m_F2TSManager.ObjectiveParameters.FishingMortalityPenalty

            ''set the max number of year to the same as the time series data
            'Me.m_nudFirstYear.Maximum = Me.m_F2TSManager.nTimeSeriesYears
            'Me.m_nudLastYear.Maximum = Me.m_F2TSManager.nTimeSeriesYears

            Me.m_nudSplinePts.Value = Me.m_F2TSManager.NumSplinePoints
            Me.m_tbxVariance.Text = CStr(Me.m_F2TSManager.VulnerabilityVariance)
            Me.m_tbVariancePrimaryProd.Text = CStr(Me.m_F2TSManager.PPVariance)
            Me.m_vulnerabilityBlockCodeSelector.SelectedBlockNum = 1
            Me.m_vulnerabilityBlockMatrix.Init(m_core)
            Me.m_vulnerabilityBlockMatrix.BlockColors = Me.m_vulnerabilityBlockCodeSelector.BlockColors
            Me.m_vulnerabilityBlockMatrix.SelectedBlockNum = Me.m_vulnerabilityBlockCodeSelector.SelectedBlockNum

            Me.m_gridGroupMaxFishingMortality = New gridFitToTimeSeriesGroup(Me.m_core.FishingPolicyManager)

            Me.m_plGrid.Controls.Clear()
            Me.m_plGrid.Controls.Add(m_gridGroupMaxFishingMortality)
            Me.m_gridGroupMaxFishingMortality.Dock = DockStyle.Fill

            Me.m_shapeHandler = New AppliedFFGUIHandler(Me.m_core, Me.m_shapeToolBox, Me.m_sketchPad)

            Me.m_cmdTSWeights = cCommandHandler.GetInstance().GetCommand("WeightTimeSeries")
            If (Me.m_cmdTSWeights IsNot Nothing) Then
                AddHandler Me.m_cmdTSWeights.OnUpdate, AddressOf OnUpdateTSCommand
            End If

            If Me.m_F2TSManager.LastYear > Me.m_F2TSManager.FirstYear Then
                Me.ReloadControls()
            End If

            Me.m_F2TSManager.Connect(Me, AddressOf OnRunStarted, AddressOf OnRunStep, AddressOf OnRunStopped, AddressOf OnModelRun)

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries, eCoreComponentType.EcoPath, eCoreComponentType.ShapesManager, eCoreComponentType.PPIManager}
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.m_F2TSManager.Disconnect(AddressOf OnRunStarted, AddressOf OnRunStep, _
                                        AddressOf OnRunStopped, AddressOf Me.OnModelRun)

            ' Detach from event handlers
            Me.m_vulnerabilityBlockCodeSelector = Nothing
            Me.m_F2TSManager = Nothing

            Me.m_shapeHandler = Nothing

            If (Me.m_cmdTSWeights IsNot Nothing) Then
                RemoveHandler Me.m_cmdTSWeights.OnUpdate, AddressOf OnUpdateTSCommand
                Me.m_cmdTSWeights = Nothing
            End If

            MyBase.OnFormClosed(e)

        End Sub

        Private Sub ReloadControls()
            'set the max number of year to the same as the time series data
            Me.m_nudFirstYear.Maximum = Me.m_F2TSManager.nTimeSeriesYears
            Me.m_nudLastYear.Maximum = Me.m_F2TSManager.nTimeSeriesYears

            Me.m_nudSplinePts.Value = Me.m_F2TSManager.NumSplinePoints
            Me.m_nudFirstYear.Value = Math.Max(0, Me.m_F2TSManager.FirstYear - 1)
            Me.m_nudLastYear.Value = Me.m_F2TSManager.LastYear
            Me.m_tbxVariance.Text = CStr(Me.m_F2TSManager.VulnerabilityVariance)
            Me.m_tbVariancePrimaryProd.Text = CStr(Me.m_F2TSManager.PPVariance)

            Me.UpdateControls()
        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            Select Case msg.Source

                Case eCoreComponentType.EcoPath
                    ' Hmm, this can be quite disastrous. Consider what to do here!
                    If ((msg.Type = eMessageType.DataAddedOrRemoved) And (msg.DataType = eDataTypes.EcoPathGroupInput)) Then
                        ' Make the vul control update itself
                        Me.m_vulnerabilityBlockMatrix.RefreshContent()
                        ' Etc...
                    End If

                Case eCoreComponentType.TimeSeries
                    Me.m_sketchPad.NumTSYears = Me.m_core.nTimeSeriesYears

                Case eCoreComponentType.ShapesManager
                    ' Refresh the Anomaly search content
                    If ((msg.DataType = eDataTypes.Forcing) Or (msg.DataType = eDataTypes.EggProd) Or (msg.DataType = eDataTypes.Mediation)) Then
                        Me.m_shapeHandler.Refresh()
                        Me.ReloadControls()
                    End If

                Case eCoreComponentType.PPIManager
                    ' Refresh on shape assignment changes
                    Me.m_shapeHandler.Refresh()

            End Select
        End Sub

#End Region ' Private form event handlers

#Region " Private control event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' -------------------------------------------------------------------
        Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSearch.Click

            Dim shapeSelected As cShapeData = Nothing

            ' Try to make sure TS are loaded
            If (Me.m_core.nTimeSeriesEnabled = 0) And (Me.m_cmdTSWeights IsNot Nothing) Then
                Me.m_cmdTSWeights.Invoke()
            End If

            If (Me.m_core.nTimeSeriesEnabled = 0) Then Return

            ' Update TS
            Me.m_core.UpdateTimeSeries()

            shapeSelected = Me.m_shapeHandler.SelectedShape
            If shapeSelected Is Nothing Then
                Me.m_F2TSManager.AnomalySearchShapeNumber = 0
            Else
                Me.m_F2TSManager.AnomalySearchShapeNumber = shapeSelected.Index
            End If
            Me.m_F2TSManager.AnomalySearch = Me.m_cbAnomalySearch.Checked
            Me.m_F2TSManager.VulnerabilitySearch = Me.m_cbVulnerabilitySearch.Checked
            Me.m_F2TSManager.ObjectiveParameters.FishingMortalityPenalty = Me.m_cbFishingMortalityPenalty.Checked

            Me.m_F2TSManager.FirstYear = CInt(Me.m_nudFirstYear.Text) + 1
            Me.m_F2TSManager.LastYear = CInt(Me.m_nudLastYear.Text)
            Me.m_F2TSManager.NumSplinePoints = CInt(Me.m_nudSplinePts.Text)
            Try
                Me.m_F2TSManager.PPVariance = Single.Parse(Me.m_tbVariancePrimaryProd.Text)
            Catch ex As Exception
                Me.m_F2TSManager.PPVariance = 0.1!
            End Try
            Try
                Me.m_F2TSManager.VulnerabilityVariance = CSng(Me.m_tbxVariance.Text)
            Catch ex As Exception
                Me.m_F2TSManager.VulnerabilityVariance = 10.0!
            End Try
            Me.m_F2TSManager.VulnerabilityBlocks = Me.m_vulnerabilityBlockMatrix.Vulblocks
            Me.m_F2TSManager.nBlockCodes = Me.m_vulnerabilityBlockCodeSelector.nBlockCodes

            m_F2TSManager.RunSearch()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' -------------------------------------------------------------------
        Private Sub btStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnStop.Click
            'this will stop any running model Search or Sensitivity
            Me.m_F2TSManager.StopRun()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' -------------------------------------------------------------------
        Private Sub m_tsbSensOfSS2V_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbSensOfSS2V.Click

            If (m_dlgSensOfSS IsNot Nothing) Then Return

            Me.m_dlgSensOfSS = New dlgSensitivityOfSStoV(Me.m_core, Me.m_F2TSManager)
            Me.m_dlgSensOfSS.NumBlocks = Me.m_vulnerabilityBlockCodeSelector.nBlockCodes

            m_F2TSManager.VulnerabilityBlocks = Me.m_vulnerabilityBlockMatrix.Vulblocks
            m_F2TSManager.nBlockCodes = m_vulnerabilityBlockCodeSelector.nBlockCodes


            If Me.m_dlgSensOfSS.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then

                ' Transfer values from the Sensitivity form to this form
                ' Number of blocks, colors on the main form should match those set by the user on the Sensitivity form
                Me.m_vulnerabilityBlockCodeSelector.nBlockCodes = Me.m_dlgSensOfSS.NumBlocks

                ' Transfer vulnerabiltiy blocks
                For iPred As Integer = 1 To Me.m_core.nGroups
                    For iPrey As Integer = 1 To Me.m_core.nGroups
                        Me.m_vulnerabilityBlockMatrix.Vulblocks(iPred, iPrey) = Me.m_dlgSensOfSS.VulnerabilityBlocks(iPred, iPrey)
                    Next iPrey
                Next iPred

                ' Adjust numblocks
                Me.m_vulnerabilityBlockMatrix.Invalidate()
            End If

            Me.m_dlgSensOfSS = Nothing

        End Sub

        Private Sub m_VulnerabilityBlockCodeSelector_OnBlockSelected(ByVal sender As ucParmBlockCodes) Handles m_vulnerabilityBlockCodeSelector.OnBlockSelected
            Me.m_vulnerabilityBlockMatrix.SelectedBlockNum = sender.SelectedBlockNum
            Me.UpdateControls()
        End Sub

        Private Sub m_VulnerabilityBlockCodeSelector_OnNumBlocksChanged(ByVal sender As ucParmBlockCodes) Handles m_vulnerabilityBlockCodeSelector.OnNumBlocksChanged
            Me.m_vulnerabilityBlockMatrix.BlockColors = sender.BlockColors
            Me.UpdateControls()
        End Sub

        Private Sub m_nudFirstYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_nudFirstYear.ValueChanged
            If (Not Me.m_bInUpdate) Then Me.m_sketchPad.FirstYear = CInt(Me.m_nudFirstYear.Value)
            Me.UpdateControls()
        End Sub

        Private Sub m_nudLastYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudLastYear.ValueChanged

            If (Not Me.m_bInUpdate) Then Me.m_sketchPad.LastYear = CInt(Me.m_nudLastYear.Value)
            Me.UpdateControls()

        End Sub

        Private Sub m_nudSplinePts_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudSplinePts.ValueChanged

            If (Not Me.m_bInUpdate) Then Me.m_sketchPad.NumSplinePoints = CInt(Me.m_nudSplinePts.Value)
            Me.UpdateControls()

        End Sub

        Dim m_shapeSelected As cShapeData = Nothing

        Private Sub m_shapeToolBox_OnSelectionChanged(ByVal ashapes As EwECore.cShapeData()) _
            Handles m_shapeToolBox.OnSelectionChanged

            Dim iMax As Integer = Me.m_core.nEcosimYears

            ' Initialize Component will cause this event to be triggered when the form is
            ' not up and running yet. Here's a sanity check:
            If (Me.m_shapeHandler Is Nothing) Then Return

            Dim shape As cShapeData = Me.m_shapeHandler.SelectedShape

            ' Reset year range when new shape selected
            If (Not Object.ReferenceEquals(m_shapeSelected, shape)) Then

                Me.m_bInUpdate = True

                ' Remember newly selected shape
                Me.m_shapeSelected = shape

                If shape IsNot Nothing Then iMax = CInt(shape.XMax / cCore.N_MONTHS)

                Me.m_nudLastYear.Maximum = iMax
                Me.m_nudFirstYear.Value = 0
                Me.m_nudLastYear.Value = Me.m_nudLastYear.Maximum

                ' Update sketchpad
                Me.m_sketchPad.FirstYear = CInt(Me.m_nudFirstYear.Value)
                Me.m_sketchPad.LastYear = CInt(Me.m_nudLastYear.Value)
                Me.m_sketchPad.NumSplinePoints = CInt(Me.m_nudSplinePts.Value)

                Me.m_bInUpdate = False

            End If

            Me.UpdateControls()

        End Sub

        Private m_bInUpdate As Boolean = True

        Private Sub m_sketchPad_OnYearRangeChanged(ByVal sender As ucAnomalySearchSketchPad) Handles m_sketchPad.OnYearRangeChanged
            Me.m_bInUpdate = True
            Me.m_nudFirstYear.Value = Math.Min(Math.Max(Me.m_nudFirstYear.Minimum, sender.FirstYear), Me.m_nudFirstYear.Maximum)
            Me.m_nudLastYear.Value = Math.Min(Math.Max(Me.m_nudLastYear.Minimum, sender.LastYear), Me.m_nudLastYear.Maximum)
            Me.m_bInUpdate = False
        End Sub

        Private Sub m_btnTimeSeries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnTimeSeriesWeights.Click
            If (Me.m_cmdTSWeights IsNot Nothing) Then
                Me.m_cmdTSWeights.Invoke()
            End If
        End Sub

        Private Sub OnAnomalySearchChecked(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_cbAnomalySearch.CheckedChanged
            Me.UpdateControls()
        End Sub

#End Region ' Private control event handlers

#Region " Private manager event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="runType"></param>
        ''' <param name="nSteps"></param>
        ''' -------------------------------------------------------------------
        Private Sub OnRunStarted(ByVal runType As eRunType, ByVal nSteps As Integer)

            Dim data As cF2TSResults = Me.m_F2TSManager.Results
            Me.LogProgress(String.Format(My.Resources.FIT2TS_PROGRESS_RUNSTARTED, data.BaseSS), False)

            If (Me.m_dlgSensOfSS IsNot Nothing) Then
                Me.m_dlgSensOfSS.OnRunStarted(runType, nSteps)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnRunStep()

            'get the results of this iteration from the manager
            Dim data As cF2TSResults = Me.m_F2TSManager.Results
            Dim runtype As eRunType = data.RunType

            Select Case runtype

                Case eRunType.Search
                    'retrieve search analysis result
                    Dim rsltSearch As cSearchResults = CType(data, cSearchResults)
                    Me.LogProgress(String.Format(My.Resources.FIT2TS_PROGRESS_RUNSTEP, rsltSearch.iStep, rsltSearch.IterSS))

                    ' Reload shape
                    If Me.m_F2TSManager.AnomalySearch Then
                        Me.m_core.ForcingShapeManager.Load()
                        ' Ugh, there must be a better way to do this
                        For Each shape As cShapeData In Me.m_shapeHandler.SelectedShapes
                            shape.Update()
                        Next
                    End If

                Case eRunType.SensitivitySS2VByPredPrey, eRunType.SensitivitySS2VByPredator
                    If (Me.m_dlgSensOfSS IsNot Nothing) Then
                        Dim results As cSensitivityToVulResults = DirectCast(Me.m_F2TSManager.Results, cSensitivityToVulResults)
                        Me.m_dlgSensOfSS.OnRunStep(runtype, results.iPred, results.iPrey, results.SSen)
                    End If

            End Select

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The model run has completed
        ''' </summary>
        ''' <param name="runType"></param>
        ''' -------------------------------------------------------------------
        Private Sub OnRunStopped(ByVal runType As eRunType)
            Me.LogProgress(String.Format(My.Resources.FIT2TS_PROGRESS_RUNCOMPLETED, Date.Now().ToShortTimeString))
            If (Me.m_dlgSensOfSS IsNot Nothing) Then
                Me.m_dlgSensOfSS.OnRunStopped(runType)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="runType"></param>
        ''' <param name="iCurrentIterationStep"></param>
        ''' <param name="nTotalIterationSteps"></param>
        ''' -------------------------------------------------------------------
        Private Sub OnModelRun(ByVal runType As eRunType, ByVal iCurrentIterationStep As Integer, ByVal nTotalIterationSteps As Integer)
            '    System.Console.WriteLine("Ecosim run " & iCurrentIterationStep.ToString & " of " & nTotalIterationSteps.ToString)
        End Sub

        Private Sub m_cbFishingMortalityPenalty_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbFishingMortalityPenalty.CheckedChanged
            Me.m_plGrid.Enabled = Me.m_cbFishingMortalityPenalty.Checked
            Me.UpdateControls()
        End Sub

#End Region ' Private search event handlers

#Region " Private command handler "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command handler, invoked after the user has changed the enabled
        ''' time series configuration.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnUpdateTSCommand(ByVal cmd As cCommand)
            Me.UpdateControls()
        End Sub

#End Region ' Private command handler

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update state of main controls based on user selections.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            If (Me.m_F2TSManager Is Nothing) Then Return
            If (Me.m_sketchPad Is Nothing) Then Return

            Dim bInputsValid As Boolean = Me.m_core.HasAppliedTimeSeries()
            Dim bIsRunning As Boolean = Me.m_F2TSManager.IsRunning()

            If Me.m_cbAnomalySearch.Checked Then
                bInputsValid = bInputsValid And _
                               (Me.m_shapeHandler.SelectedShape IsNot Nothing) And _
                               (Me.m_nudLastYear.Value > Me.m_nudFirstYear.Value)
            Else
                'bInputsValid = True
            End If

            ' Search button enabled when ts loaded and not running
            Me.m_btnStop.Enabled = bIsRunning
            Me.m_btnSearch.Enabled = (Not bIsRunning) And bInputsValid

            'constrain the number of years to the number of years in the time series data
            If Me.m_nudLastYear.Value > Me.m_F2TSManager.nTimeSeriesYears Then
                Me.m_nudLastYear.Value = Me.m_F2TSManager.nTimeSeriesYears
            End If

            Me.m_nudFirstYear.Maximum = Me.m_F2TSManager.nTimeSeriesYears
            Me.m_nudLastYear.Maximum = Me.m_F2TSManager.nTimeSeriesYears

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strEntry"></param>
        ''' <param name="bAppend"></param>
        ''' -------------------------------------------------------------------
        Private Sub LogProgress(ByVal strEntry As String, Optional ByVal bAppend As Boolean = True)
            Dim strLog As String = strEntry & vbNewLine & Me.m_tbResults.Text
            Me.m_tbResults.Text = strLog
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace