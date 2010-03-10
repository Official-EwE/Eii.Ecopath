#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim
Imports EwEUtils.Commands
Imports ZedGraph

#End Region

Public Class frmMSE

    'ToDo_jb 15-Jan-2010 MSE looks like the min and max are wrong

#Region "Private Enum definitions"

    ''' <summary>
    ''' MSE state enumerators use by the interface to set control states
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum eMSEStates
        InActive
        Running
        Completed
    End Enum

#End Region

#Region "Private variables"

    Dim m_MSE As cMSEManager

    Private m_fpNTrials As cPropertyFormatProvider
    Private m_fpUsePlugin As cPropertyFormatProvider
    Private m_fpSave As cPropertyFormatProvider

    Private m_fpKalman As cPropertyFormatProvider
    Private m_fpForecast As cPropertyFormatProvider
    Private m_fpSBPower As cPropertyFormatProvider

    Private m_fpUseQuotaRegs As cPropertyFormatProvider

    Private m_paneMaster As MasterPane = Nothing
    Private m_zgh As cZedGraphHelper = Nothing
    Private m_curState As eMSEStates

    Private m_dctEffortControls As Dictionary(Of eMSEEffortMode, RadioButton)

    Private m_plotter As cMSEPlotter
    Private m_coreMessage As cMSEEventSource

#End Region

#Region "Construction Initialization and Destruction"

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        ' Show/Hide Groups
        Dim cmdh As cCommandHandler = Me.CommandHandler
        Dim cmd As cCommand = cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        If Not Object.ReferenceEquals(cmd, Nothing) Then
            cmd.RemoveControl(Me.btShowHide)
        End If

        RemoveHandler cmd.OnPostInvoke, AddressOf Me.OnShowHideGroups
        RemoveHandler Me.m_coreMessage.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged

        Me.CoreComponents = Nothing
        Me.m_MSE.Disconnect()
        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_MSE = Me.UIContext.Core.MSEManager

        Me.m_fpNTrials = New cPropertyFormatProvider(Me.UIContext, Me.txNTrials, Me.m_MSE.ModelParameters, eVarNameFlags.MSENTrials)
        Me.m_fpUsePlugin = New cPropertyFormatProvider(Me.UIContext, Me.ckPlugin, Me.m_MSE.ModelParameters, eVarNameFlags.MSEUseEconomicPlugin)
        Me.m_fpSave = New cPropertyFormatProvider(Me.UIContext, Me.ckSave, Me.m_MSE.ModelParameters, eVarNameFlags.MSESave)

        Me.m_fpForecast = New cPropertyFormatProvider(Me.UIContext, Me.txForecast, Me.m_MSE.ModelParameters, eVarNameFlags.MSEForcastGain)
        Me.m_fpSBPower = New cPropertyFormatProvider(Me.UIContext, Me.txSBPower, Me.m_MSE.ModelParameters, eVarNameFlags.MSEAssessPower)
        Me.m_fpKalman = New cPropertyFormatProvider(Me.UIContext, Me.txKalmanGain, Me.m_MSE.ModelParameters, eVarNameFlags.MSEKalmanGain)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

        m_coreMessage = New cMSEEventSource

        'Assessment methods Catch Estimated Biomass and Direct Exploitation are stored in the tag property of the radio buttons
        'see the Changed event of the radio buttons for setting the parameters
        Me.rbCatchEstBio.Tag = eAssessmentMethods.CatchEstmBio
        Me.rbDirectExp.Tag = eAssessmentMethods.DirectExploitation
        Me.rbExact.Tag = eAssessmentMethods.Exact

        Me.rbFTracking.Tag = eMSEEffortMode.Tracking
        Me.rbPredictEffort.Tag = eMSEEffortMode.PredictUseQuota
        Me.rbTrackUseQuota.Tag = eMSEEffortMode.TrackUseQuota

        m_dctEffortControls = New Dictionary(Of eMSEEffortMode, RadioButton)
        m_dctEffortControls.Add(eMSEEffortMode.Tracking, Me.rbFTracking)
        m_dctEffortControls.Add(eMSEEffortMode.PredictUseQuota, Me.rbPredictEffort)
        m_dctEffortControls.Add(eMSEEffortMode.TrackUseQuota, Me.rbTrackUseQuota)

        ' Display Groups
        Dim cmd As cCommand = Me.UIContext.CommandHander.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        If Not Object.ReferenceEquals(cmd, Nothing) Then
            cmd.AddControl(Me.btShowHide)
        End If

        AddHandler cmd.OnPostInvoke, AddressOf Me.OnShowHideGroups
        AddHandler Me.m_coreMessage.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged

        Me.m_paneMaster = Me.zdGraph.MasterPane
        Me.m_zgh = New cZedGraphHelper()

        Me.m_plotter = New cMSEPlotter

        Me.m_plotter.Init(Me.UIContext, Me.m_MSE, Me.m_zgh, Me.zdGraph)
        Me.m_plotter.PlotType = ePlotTypes.Line
        Me.m_plotter.DataType = ePlotData.Biomass

        Me.UpdateSelectedEffortMode()

        Me.UpdateControls(eMSEStates.InActive)

        Me.initGraphs()

    End Sub

    Private Sub OnShowHideGroups(ByVal cmd As cCommand)

        'Just clear the graphs and add the reference lines
        'there is no interation data available for the graph
        'that has to be added via AddLineToGraph()
        Me.m_plotter.Clear()
        '  Me.AddRefToGraph()
        'Me.m_plotter.Draw()

    End Sub

    ''' <summary>
    ''' Reference levels have changed! For now just redraw the graphs
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub onRefLevelsChanged()
        Try
            'this still needs sorting out
            Me.m_plotter.AddReference()
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "Core interactions"

    Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)

        Me.m_coreMessage.HandleCoreMessage(msg)

    End Sub

#End Region

#Region "MSE interactions"

    Private Sub runMSE()

        Try

            Me.m_MSE.Connect(AddressOf Me.onMSECallBack, Nothing)

            Me.m_MSE.ValidateRun()

            cApplicationStatusNotifier.SetStatusText("Initializing MSE...")

            'init the graphs for a new run
            Me.m_plotter.Clear()

            Me.m_MSE.Run()

        Catch ex As Exception

        End Try

    End Sub


    Private Sub onMSECompleted()

        cApplicationStatusNotifier.SetStatusText("")
        Me.m_MSE.Disconnect()

    End Sub

    Private Sub onMSECallBack(ByVal CallBackType As MSE.eCallBackTypes)

        Dim state As eMSEStates
        Select Case CallBackType

            Case eCallBackTypes.Started
                state = eMSEStates.Running

            Case eCallBackTypes.IterationStarted
                state = eMSEStates.Running

            Case eCallBackTypes.IterationCompleted
                Me.onMSEProgress()
                Me.AddLineToGraph()
                state = eMSEStates.Running

            Case eCallBackTypes.RunCompleted
                Me.AddMeanLineToGraph()
                Me.onMSECompleted()
                state = eMSEStates.Completed

        End Select

        Me.UpdateControls(state)

    End Sub

    Private Sub onMSEProgress()
        Dim sProgress As Single = CSng(Me.m_MSE.Output.TrialNumber / Me.m_MSE.ModelParameters.NTrials)
        cApplicationStatusNotifier.SetStatusText("Running MSE...", TriState.UseDefault, sProgress)
    End Sub

#End Region

#Region "Interface events"

    Private Sub rbFTracking_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbFTracking.CheckedChanged, rbPredictEffort.CheckedChanged, rbTrackUseQuota.CheckedChanged

        If Me.m_MSE Is Nothing Then Exit Sub

        Try
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            If rb.Checked = True Then
                Dim EffortMode As eMSEEffortMode = DirectCast(rb.Tag, eMSEEffortMode)
                Me.m_MSE.ModelParameters.EffortMode = EffortMode
            End If

        Catch ex As Exception
            Debug.Assert(False, "Exception setting MSE Effort Mode. " & ex.Message)
        End Try

        Me.UpdateControls(Me.m_curState)
        Me.Refresh()

    End Sub


    Private Sub onRunClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRun.Click

        Try
            Me.runMSE()
        Catch ex As Exception

        End Try

    End Sub


    ''' <summary>
    ''' Change the biomass assessment method based on the selected radio button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub onAssessmentMethodChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbCatchEstBio.CheckedChanged, rbDirectExp.CheckedChanged, rbExact.CheckedChanged
        Try

            If Me.m_MSE Is Nothing Then Exit Sub

            Debug.Assert(TypeOf sender Is RadioButton)
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            'This event handler is call for both radio buttons Changed events Checked and UnChecked
            'Use the tag of the Checked radio button to set the MSE.AssessmentMethod
            If rb.Checked = True Then
                Me.m_MSE.ModelParameters.AssessmentMethod = DirectCast(rb.Tag, eAssessmentMethods)
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub btStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btStop.Click
        Me.m_MSE.ModelParameters.StopRun = True
    End Sub

#End Region

#Region "Interface objects"

#Region "Graphs"

    Private Sub AddLineToGraph()
        Try
            Dim lstData As New List(Of cCoreGroupBase)
            For Each grp As cMSEGroupOutput In Me.m_MSE.GroupOutputs
                If Me.UIContext.StyleGuide.GroupVisible(grp.Index) Then
                    lstData.Add(grp)
                End If
            Next

            Me.m_plotter.AddData(lstData)
            Me.m_plotter.Draw()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".AddLineToGraph() Error: " & ex.Message)
        End Try
    End Sub

    Private Sub AddMeanLineToGraph()
        Try

            Me.m_plotter.AddMean()
            Me.m_plotter.Draw()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".AddMeanLineToGraph() Error: " & ex.Message)
        End Try
    End Sub



    Private Function nVisGroups() As Integer
        Dim n As Integer
        For igrp As Integer = 1 To Me.UIContext.Core.nGroups
            If Me.UIContext.StyleGuide.GroupVisible(igrp) Then
                n += 1
            End If
        Next
        Return n
    End Function

    Private Sub initGraphs()
        Me.m_plotter.Clear()
        Me.m_plotter.Draw()
    End Sub

#End Region

#Region "Controls"


    Private Sub UpdateSelectedEffortMode()

        Try
            m_dctEffortControls.Item(Me.m_MSE.ModelParameters.EffortMode).Checked = True
        Catch ex As Exception

        End Try

    End Sub

    Private Sub UpdateControls(ByVal State As eMSEStates)

        Try

            Select Case State

                Case eMSEStates.InActive
                    Me.btRun.Enabled = True
                    Me.btStop.Enabled = False
                    Me.btShowHide.Enabled = True
                    Me.pnlFTracking.Enabled = True
                    Me.pnlRunOpt.Enabled = True
                    Me.pnlRegOpt.Enabled = True

                Case eMSEStates.Running
                    Me.btRun.Enabled = False
                    Me.btStop.Enabled = True
                    Me.btShowHide.Enabled = False
                    Me.pnlFTracking.Enabled = False
                    Me.pnlRunOpt.Enabled = False
                    Me.pnlFTracking.Enabled = False
                    Me.pnlRegOpt.Enabled = False

                Case eMSEStates.Completed
                    Me.btRun.Enabled = True
                    Me.btStop.Enabled = False
                    Me.btShowHide.Enabled = True
                    Me.pnlRunOpt.Enabled = True
                    Me.pnlRegOpt.Enabled = True

            End Select

            If State <> eMSEStates.Running Then
                If Me.m_MSE.ModelParameters.EffortMode = eMSEEffortMode.Tracking Then
                    Me.pnlFTracking.Enabled = True
                Else
                    Me.pnlFTracking.Enabled = False
                End If
            End If

            m_curState = State

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".UpdateControls(): " & ex.Message)
        End Try

    End Sub

#End Region

#End Region

End Class

