'==============================================================================
'
' $Log: RunEcosim.vb,v $
' Revision 1.13  2009/03/26 17:41:39  jeroens
' Fixed confusion between rate and effort shape names
'
' Revision 1.12  2009/03/22 14:01:38  jeroens
' Core state monitor exec event parameters simplified
'
' Revision 1.11  2009/03/20 17:55:41  jeroens
' Shape controls are multiple selection
'
' Revision 1.10  2009/03/17 17:18:10  jeroens
' EcosimCompleteDelegate -> Populate
'
' Revision 1.9  2009/03/02 01:53:18  jeroens
' Properly named handlers
'
' Revision 1.8  2009/02/24 04:07:11  jeroens
' Renamed combo class
'
' Revision 1.7  2009/02/05 17:48:38  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.6  2009/01/16 18:30:38  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.5  2008/12/15 15:56:20  jeroens
' no message
'
' Revision 1.4  2008/12/03 00:07:58  jeroens
' Fixed bug 580
'
' Revision 1.3  2008/11/26 21:18:57  sherman
' Removed Group Boxes
'
' Revision 1.2  2008/11/26 16:00:22  jeroens
' Fixed issue 571
'
' Revision 1.1  2008/09/26 07:31:48  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports SAUPUtil.SAUPData.Mapping
Imports EwEUtils.Core
Imports ScientificInterface.Other
Imports Microsoft.VisualBasic
Imports ScientificInterfaceShared

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
        Private m_Core As cCore = Nothing
        Private m_shapeGUIHandler As cForcingShapeGUIHandler = Nothing
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

        Private m_ccb As cCustomComboBoxFleetGroupTree = Nothing

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

            Me.m_ccb = New cCustomComboBoxFleetGroupTree(Me.m_Core, Me.tscbTarget)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.ShapesManager}

            ' Track core monitor changes
            AddHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            Me.UpdateControls()

        End Sub

        Private Sub RunEcosim_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            RemoveHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            Me.m_coreStateMonitor = Nothing
            Me.CoreComponents = Nothing
        End Sub

        Private Sub btnRunOrStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunOrStop.Click

            If Not m_bEcosimRunning Then

                m_TimeSteps = m_Core.nEcosimTimeSteps

                'jb clear the graph
                'Me.m_ucBPlots.Plot.Clear()
                'm_ucBPlots.ProgressVisible = True
                Me.m_graph.Refresh()

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
                                Me.m_graph.SSValue = Me.m_Core.EcosimStats.SS


                            End If

                            ' Now plot the graphs.
                            Me.m_graph.Populate()

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

        Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)

            Dim bEcosimRunning As Boolean = m_coreStateMonitor.IsEcosimRunning
            Dim bHasEcosimResults As Boolean = m_coreStateMonitor.HasEcosimRan

            ' Does not have ecosim results?
            If (Not bHasEcosimResults) Then
                ' #Yes: clear run results
                Me.m_graph.OnCoreExecutionStateChanged()
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
            ' JS 16May08: bypassed shape handler (which may be 0) to do a mass change
            Me.m_Core.FishingEffortShapeManager.ResetToDefaults()
            Me.m_Core.FishMortShapeManager.ResetToDefaults()
        End Sub

        Private Sub OnFZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSetTo0.Click
            If Me.m_shapeGUIHandler IsNot Nothing Then
                Me.m_shapeGUIHandler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset, _
                            New cShapeData() {Me.m_sketchPad.Shape}, 0.0!)
            End If
        End Sub

#End Region ' FF

#End Region ' Events

#Region " Internal implementation "

        Private Function GetSelectedTarget() As ICoreInterface
            Dim tv As cCustomComboBoxFleetGroupTree = DirectCast(Me.tscbTarget.DropdownControl, cCustomComboBoxFleetGroupTree)
            Return tv.SelectedItem()
        End Function

        ''' <summary>
        ''' Load fishing effort data from the Fishing Rate manager 
        ''' </summary>
        ''' <remarks>Right now, it is zero based</remarks>
        Private Sub LoadFishingRateShape()
            Dim item As ICoreInterface = Me.GetSelectedTarget()

            Me.m_shapeGUIHandler = New cFishingEffortShapeGUIHandler(Me.m_Core, Nothing, Me.m_sketchPad)
            Me.m_shapeGUIHandler.SelectedShape = DirectCast(item, cFishingRateShape)
            Me.UpdateControls()
        End Sub

        'Fish Rate (Y/B)
        Private Sub LoadFishMortShape()
            Dim item As ICoreInterface = Me.GetSelectedTarget()
            Dim shape As cShapeData = Nothing

            ' Mortality shapes are 0-base indexed, groups are 1-base indexed
            shape = m_Core.FishMortShapeManager.Item(item.Index - 1)

            m_shapeGUIHandler = New cFishingMortalityShapeGUIHandler(Me.m_Core, Nothing, Me.m_sketchPad)
            m_shapeGUIHandler.SelectedShape = shape
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
                Case eCoreComponentType.EcoPath
                    'DataAddedOrRemoved for a Group or a Fleet 
                    If msg.Type = eMessageType.DataAddedOrRemoved And _
                                ((msg.DataType = eDataTypes.EcoPathGroupInput) Or (msg.DataType = eDataTypes.FleetInput)) Then
                        'Then update the interface to the new number of groups and or fleets
                        Me.m_ccb.UpdateContent()

                    End If

                Case eCoreComponentType.EcoSim
                    'handle ecosim messages
                    EcosimMessageHandler(msg)

                Case eCoreComponentType.ShapesManager
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
