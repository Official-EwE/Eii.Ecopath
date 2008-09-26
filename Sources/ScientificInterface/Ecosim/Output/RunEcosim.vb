'==============================================================================
'
' $Log: RunEcosim.vb,v $
' Revision 1.1  2008/09/26 07:31:48  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.79  2008/07/30 20:00:36  jeroens
' Fixed Sim runstate feedback bug
'
' Revision 1.78  2008/07/29 19:35:16  sherman
' Bug fixes
' - clear lines when core state changed
' - fixed year change bugs
'
' Revision 1.77  2008/07/29 16:14:48  sherman
' Added sum of squares
'
' Revision 1.76  2008/07/25 20:59:32  sherman
' Ported BiomassPlots to zedgraph in RunEcosim
'
' Revision 1.75  2008/05/18 02:12:58  jeroens
' Fixed issue 361
'
' Revision 1.74  2008/05/05 22:21:26  jeroens
' Shared progress bar
'
' Revision 1.73  2008/02/05 16:04:14  jeroens
' Fixed bug 401
'
' Revision 1.72  2008/01/08 20:15:02  jeroens
' Last manual value repeated across shape - fixes bug 361
'
' Revision 1.71  2008/01/07 16:45:58  jeroens
' Fixed number interpretation bug
'
' Revision 1.70  2007/12/31 15:53:51  jeroens
' * Fixed bug 365
'
' Revision 1.69  2007/12/14 15:48:09  jeroens
' * Fixed hokey layout, uses toolbars instead
'
' Revision 1.68  2007/12/10 02:30:13  sherman
' Re-organized Ecosim Plots and Ecosim Results.  Moved monte carlo run to tools.
'
' Revision 1.67  2007/12/05 03:46:16  jeroens
' - Removed links to specialized core state events; generic core state event suffices
'
' Revision 1.66  2007/11/22 18:40:54  jeroens
' * Uses command
'
' Revision 1.65  2007/11/02 16:23:02  joeb
' Redraw summary graph lines in response to core message
'
' Revision 1.64  2007/10/31 16:04:45  jeroens
' * Respond to relevant shape manager messages
'
' Revision 1.63  2007/10/30 22:53:44  jeroens
' + Reconnected reset commands
'
' Revision 1.62  2007/10/29 16:38:42  jeroens
' * Uses new revamped shape controls layout
'
' Revision 1.61  2007/10/29 14:06:34  jeroens
' * Updated to reworked shape controls
'
' Revision 1.60  2007/10/18 22:04:50  joeb
' release core state monitor
'
' Revision 1.59  2007/10/15 20:04:10  joeb
' Removed some commented out code
'
' Revision 1.58  2007/10/15 16:48:07  joeb
' Removed core message handler and put all message handling in OnCoreMessage
'
' Revision 1.57  2007/10/15 15:26:13  jeroens
' * Updated to renamed override
'
' Revision 1.56  2007/10/14 17:01:08  joeb
' Changes to Dispose
'
' Revision 1.55  2007/10/14 16:45:13  jeroens
' - Released message sources
'
' Revision 1.54  2007/10/13 22:38:28  jeroens
' * Fixed bug 282
'
' Revision 1.53  2007/10/12 15:20:50  joeb
' Changes for Results forms
'
' Revision 1.52  2007/10/09 18:58:57  joeb
' Progress bar
'
' Revision 1.51  2007/10/05 18:15:15  joeb
' Added BatchMode to biomass graph
'
' Revision 1.50  2007/09/29 01:17:01  joeb
' Bug Fixes
'
' Revision 1.49  2007/09/28 18:55:18  joeb
' changed number of years
'
' Revision 1.48  2007/09/13 17:18:25  jeroens
' * Re-enabled group/fleet tree view. Why was this commented out?
'
' Revision 1.47  2007/09/11 21:13:44  fgao
' More FPS stuff
'
' Revision 1.46  2007/09/11 12:25:17  jeroens
' * Fixed group selection to mortality shape index offset bug
'
' Revision 1.45  2007/09/07 13:34:15  jeroens
' * Replaced fleet and group combo's with single drop down tree
'
' Revision 1.44  2007/09/04 18:39:40  jeroens
' * Reset F's updated to screen immediately
'
' Revision 1.43  2007/08/24 22:53:43  fgao
' Add a progress bar
'
' Revision 1.42  2007/08/21 19:50:06  jeroens
' * Simplified fleet/group selection handling
'
' Revision 1.41  2007/08/10 23:23:41  fgao
' Finish ucBiomassPlot, make them work for both MCRun and RunEcosim UI,
' Add annual plot options etc.
'
' Revision 1.40  2007/08/07 16:41:34  jeroens
' * Fixed several layout issues
'
' Revision 1.39  2007/08/03 23:46:49  fgao
' Improved a lot in biomass rendering speed.
'
' Revision 1.38  2007/08/03 17:59:29  jeroens
' - Removed dead logic
'
' Revision 1.37  2007/08/03 16:31:13  jeroens
' + Localized
' + Properly uses m_selectionMode
'
' Revision 1.36  2007/08/01 23:42:44  fgao
' Add MC Run plot now....
'
' Revision 1.35  2007/08/01 23:09:36  joeb
' Fixxed UpdateCoreShapeData bug it was not calling the correct manager
'
' Revision 1.34  2007/08/01 21:00:39  fgao
' Refactoring out the Biomass plot control, so it can be used for MCRun.
'
'==============================================================================

#Region "Imports Directive"

Option Explicit On
Option Strict On

Imports EwECore
Imports SAUPUtil.SAUPData.Mapping
Imports EwEUtils.Core
Imports ScientificInterface.Other
Imports Microsoft.VisualBasic

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

        Private WithEvents m_coreStateMonitor As cCoreStateMonitor = Nothing

        Private m_ucBPlots As New ucBiomassPlotzgc

        Private m_Core As cCore = Nothing
        'Private m_FishingRateManager As cFishingRateManger = Nothing
        'Private m_FishMortalityManager As cFishMortalityManger = Nothing
        Private m_shapeGUIHandler As ForcingShapeGUIHandler = Nothing

        Private m_BiomassResults(,) As Single
        Private m_EcosimModelParams As cEcoSimModelParameters = Nothing
        Private m_TimeSteps As Integer

        ''' <summary>
        ''' True when this interface is running ecosim. False otherwise
        ''' </summary>
        ''' <remarks>This is to stop this interface from responding to Ecosim messages if it did not start the ecosim run </remarks>
        Private m_bEcosimRunning As Boolean = False
        Private m_iRenderSpeed As Integer = 60

        Private m_simStats As cEcosimStats

        Private m_ccb As CustomComboBoxFleetGroupTree = Nothing

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_Core = cCore.GetInstance()
            Me.m_coreStateMonitor = Me.m_Core.StateMonitor
            Me.m_EcosimModelParams = m_Core.EcoSimModelParameters()

            '' Get the fishing rate shape manager 
            'm_FishingRateManager = m_Core.FishingRateShapeManager

            '' Get the fish mortality manager
            'm_FishMortalityManager = m_Core.FishMortShapeManager

            m_simStats = m_Core.EcosimStats

        End Sub

        Public Sub New(ByVal text As String)
            Me.New()
            'Set tab text
            Me.TabText = text
            ' Set the windows text
            Me.Text = text

        End Sub

#End Region ' Constructors

#Region " Events "

#Region " Generic "

        Private Sub RunEcosim_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            plBPlot.Controls.Clear()

            'm_ucBPlots.BatchMode = False
            'm_ucBPlots.ProgressVisible = False
            m_ucBPlots.Dock = DockStyle.Fill

            plBPlot.Controls.Add(m_ucBPlots)

            Me.m_ccb = New CustomComboBoxFleetGroupTree(Me.m_Core, Me.tscbTarget)

            Me.MessageSources = New eMessageSource() {eMessageSource.EcoPath, eMessageSource.EcoSim, eMessageSource.ShapesManager}

            Me.UpdateControls()

        End Sub

        Private Sub RunEcosim_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.m_coreStateMonitor = Nothing
            Me.MessageSources = Nothing
        End Sub

        Private Sub btnRunOrStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunOrStop.Click

            If Not m_bEcosimRunning Then

                m_TimeSteps = m_Core.nEcosimTimeSteps

                'jb clear the graph
                'Me.m_ucBPlots.Plot.Clear()
                'm_ucBPlots.ProgressVisible = True
                Me.m_ucBPlots.Refresh()

                ReDim m_BiomassResults(m_Core.nGroups, m_TimeSteps)
                m_Core.RunEcoSim(AddressOf TimeStepFromEcoSim_handler)
            Else
                m_Core.StopEcoSim()
            End If

        End Sub

        Private Sub btnPlots_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

            Dim plotsDlg As New EcosimOutputPlots
            plotsDlg.ShowDialog()

        End Sub

        Private Sub btnResults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

            Dim resultsDlg As New EcosimResults
            resultsDlg.ShowDialog()

        End Sub

        Private Sub EcosimMessageHandler(ByRef msg As cMessage)

            Try
                Select Case msg.Type
                    Case eMessageType.EcosimRunCompleted

                        Try
                            'jb if Ecosim was not run by this interface ignore this message
                            If m_BiomassResults IsNot Nothing Then
                                'm_ucBPlots.ProgressVisible = False
                                'm_ucBPlots.AddValues(m_BiomassResults)
                                m_ucBPlots.SSValue = Me.m_Core.EcosimStats.SS


                            End If

                            ' Now plot the graphs.
                            Me.m_ucBPlots.EcosimCompleteDelegate()

                        Catch ex As Exception
                            'make sure the model can be rerun if something goes wrong
                            'm_ucBPlots.ProgressVisible = False
                        End Try


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

#End Region ' Generic

#Region " Biomass plot events "

        Private Sub TimeStepFromEcoSim_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)

            Try

                For groupIndex As Integer = 1 To results.nGroups
                    m_BiomassResults(groupIndex, CInt(iTime)) = results.Biomass(groupIndex)
                Next



                AppLauncher.GetInstance().SetStatusText("Running Ecosim...", TriState.UseDefault, CSng(iTime / m_TimeSteps))
                'If iTime Mod m_iRenderSpeed = 0 Then
                '    m_ucBPlots.RenderSpeed = CInt(iTime * 100 / m_TimeSteps)
                'End If

            Catch ex As Exception
                'jb write this to the console instead of the log so that it does not flood the log if something goes wrong
                System.Console.WriteLine(Me.ToString & ".TimeStepFromEcoSim_handler(" & iTime.ToString & ") Error: " & ex.Message)

            End Try

        End Sub

        Private Sub OnCoreExecutionStateChanged(ByVal core As EwECore.cCore, ByVal iState As eCoreExecutionState) Handles m_coreStateMonitor.CoreExecutionStateEvent

            Dim bEcosimRunning As Boolean = m_coreStateMonitor.IsEcosimRunning

            ' Ecosim back to loaded state?
            If (iState = eCoreExecutionState.EcosimLoaded) Then
                ' #Yes: clear run results
                'Me.m_ucBPlots.Plot.Clear()
                Me.m_ucBPlots.OnCoreExecutionStateChanged()
            End If

            ' Check whether ecosim is running
            ' Is this a state change?
            If (bEcosimRunning <> Me.m_bEcosimRunning) Then
                ' #Yes: update to new state
                Me.m_bEcosimRunning = bEcosimRunning
                If Me.m_bEcosimRunning Then
                    AppLauncher.GetInstance().SetStatusText("Running Ecosim", TriState.True, 0)
                Else
                    AppLauncher.GetInstance().SetStatusText("", TriState.False, 0)
                End If
                Me.UpdateControls()

            End If

        End Sub

#End Region ' Biomass plot events

#Region "Forcing function related"

        Private Sub tscbTarget_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tscbTarget.SelectedIndexChanged
            Dim obj As ICoreInterface = GetSelectedTarget()

            If TypeOf obj Is cFishingRateShape Then
                Me.SelectionMode = eSelectionModeType.Fleets
                LoadFishingRateShape()
                Return
            End If

            If TypeOf obj Is cEcoPathGroupInput Then
                Me.SelectionMode = eSelectionModeType.Groups
                LoadFishMortShape()
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
                            Me.m_shapeGUIHandler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Reset, Me.m_sketchPad.Shape, CSng(Val(astrEntered(0))))
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
            ' JS 16May08: bypassed shape handler (which may be 0) to do a mass change
            Me.m_Core.FishingRateShapeManager.ResetToDefaults()
            Me.m_Core.FishMortShapeManager.ResetToDefaults()
        End Sub

        Private Sub OnFZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSetTo0.Click
            If Me.m_shapeGUIHandler IsNot Nothing Then
                Me.m_shapeGUIHandler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Reset, Me.m_sketchPad.Shape, 0.0!)
            End If
        End Sub

#End Region ' FF

#End Region ' Events

#Region " Internal implementation "

        Private Function GetSelectedTarget() As ICoreInterface
            Dim tv As CustomComboBoxFleetGroupTree = DirectCast(Me.tscbTarget.DropdownControl, CustomComboBoxFleetGroupTree)
            Return tv.SelectedItem()
        End Function

        ''' <summary>
        ''' Load fishing effort data from the Fishing Rate manager 
        ''' </summary>
        ''' <remarks>Right now, it is zero based</remarks>
        Private Sub LoadFishingRateShape()
            Dim item As ICoreInterface = Me.GetSelectedTarget()

            Me.m_shapeGUIHandler = New FishingRateShapeGUIHandler(Me.m_Core, Nothing, Me.m_sketchPad)
            Me.m_shapeGUIHandler.Selection = DirectCast(item, cFishingRateShape)
            Me.UpdateControls()
        End Sub

        'Fish Rate (Y/B)
        Private Sub LoadFishMortShape()
            Dim item As ICoreInterface = Me.GetSelectedTarget()
            Dim shape As cShapeData = Nothing

            ' Mortality shapes are 0-base indexed, groups are 1-base indexed
            shape = m_Core.FishMortShapeManager.Item(item.Index - 1)

            m_shapeGUIHandler = New FishingMortalityShapeGUIHandler(Me.m_Core, Nothing, Me.m_sketchPad)
            m_shapeGUIHandler.Selection = shape
            Me.UpdateControls()
        End Sub

        Private Sub ClearShape()
            Me.m_sketchPad.Shape = Nothing
            Me.UpdateControls()
        End Sub

        ''' <summary>
        ''' This helper methods converts the data type, cShapeData, returned by ForcingShapeManager
        ''' into an array of singles used by Forcing Sketchpad interface.
        ''' </summary>
        Private Function GetForcingDataArray(ByRef xData As cShapeData) As Single()

            Dim tmpList As New List(Of Single)
            tmpList.Add(0)
            For i As Integer = 1 To xData.XMax
                tmpList.Add(xData.ShapeData(i))
            Next
            Return tmpList.ToArray

        End Function

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
            ' ToDo_JS: globalize this
            Me.btnRunOrStop.Text = CStr(IIf(Me.m_bEcosimRunning, "&Stop", "&Run"))
            Me.btnRunOrStop.Enabled = Me.m_coreStateMonitor.HasEcosimLoaded
            ' Reflect change immediately
            Me.btnRunOrStop.Update()

            Select Case Me.SelectionMode

                Case eSelectionModeType.Fleets
                    'Me.cbGroups.SelectedIndex = -1
                    gpbFF.Text = My.Resources.ECOSIM_RUN_MODEFLEET

                Case eSelectionModeType.Groups
                    'Me.cbFleets.SelectedIndex = -1
                    gpbFF.Text = My.Resources.ECOSIM_RUN_MODEGROUP

            End Select

            ' Reset buttons
            Me.tsbSetToValue.Enabled = (Me.m_sketchPad.Shape IsNot Nothing)
            Me.tsbSetTo0.Enabled = (Me.m_sketchPad.Shape IsNot Nothing)
            Me.tsbResetFs.Enabled = True

        End Sub

#End Region ' Internal implementation

#Region " Mandatory overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            ' Update group/fleet tree when ecopath #groups or #fleets has changed
            Select Case msg.Source

                ' Is Ecopath 'data added or removed' message?
                Case eMessageSource.EcoPath
                    'DataAddedOrRemoved for a Group or a Fleet 
                    If msg.Type = eMessageType.DataAddedOrRemoved And _
                                ((msg.DataType = eDataTypes.EcoPathGroupInput) Or (msg.DataType = eDataTypes.FleetInput)) Then
                        'Then update the interface to the new number of groups and or fleets
                        Me.m_ccb.UpdateContent()

                    End If

                Case eMessageSource.EcoSim
                    'handle ecosim messages
                    EcosimMessageHandler(msg)

                Case eMessageSource.ShapesManager
                    ' Respond to relevant shape changes
                    If (Me.m_shapeGUIHandler Is Nothing) Then Return

                    If (((Me.SelectionMode = eSelectionModeType.Fleets) And (msg.DataType = eDataTypes.FishingRate)) Or _
                        ((Me.SelectionMode = eSelectionModeType.Groups) And (msg.DataType = eDataTypes.FishMort))) Then

                        Me.m_shapeGUIHandler.Refresh()

                    End If

            End Select

        End Sub

#End Region ' Mandatory overrides

    End Class

End Namespace
