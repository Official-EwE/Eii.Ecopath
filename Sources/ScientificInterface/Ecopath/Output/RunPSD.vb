#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Properties
Imports ZedGraph

#End Region 'Imports

Namespace Ecopath.Output

    Public Class RunPSD

#Region " Variables "

        ' -- Core connection
        Private m_coreStateMonitor As cCoreStateMonitor = Nothing

        ' -- To make life easier and a more fun place to be
        Private m_zgh As cZedGraphHelper = Nothing

        ' -- Format providers --
        Private m_fpNoOfPointsPSD As cEwEFormatProvider = Nothing
        Private m_fpMinWeight As cEwEFormatProvider = Nothing
        Private m_fpNoOfPointsMovAvg As cEwEFormatProvider = Nothing

        ' -- Internal admin --
        ''' <summary>Flag stating whether the current Ecopath results have been plotted.</summary>
        Private m_bEcopathResultsPlotted As Boolean = False

        Private m_cmdShowGroups As cDisplayGroupsCommand = Nothing

#End Region ' Variables

#Region " Constructor/Destructor "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Constructor/Destructor

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim parms As cPSDParameters = Nothing
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim pm As cPropertyManager = Me.UIContext.PropertyManager

            Me.m_coreStateMonitor = Me.UIContext.Core.StateMonitor
            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_zedgraph)

            ' Connect to show/hide groups command
            Me.m_cmdShowGroups = DirectCast(cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME), cDisplayGroupsCommand)
            If Not Object.ReferenceEquals(Me.m_cmdShowGroups, Nothing) Then
                Me.m_cmdShowGroups.AddControl(Me.m_tsbnShowHideGroups)
                AddHandler Me.m_cmdShowGroups.OnPostInvoke, AddressOf OnAfterShowGroups
            End If

            ' Connect format providers
            parms = Me.UIContext.Core.ParticleSizeDistributionParameters
            Me.m_fpNoOfPointsPSD = New cPropertyFormatProvider(pm, Me.m_tstbxNoOfPointsPSD.Control, parms, eVarNameFlags.PSDNumWeightClasses)
            Me.m_fpMinWeight = New cPropertyFormatProvider(pm, Me.m_tstbxMinWeight.Control, parms, eVarNameFlags.PSDFirstWeightClass)
            Me.m_fpNoOfPointsMovAvg = New cPropertyFormatProvider(pm, Me.m_tstbxNoOfPointsMovAvg.Control, parms, eVarNameFlags.NumPtsMovAvg)

            ' Connect to core state monitor events
            AddHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            ' Sync controls
            Me.UpdateToolstrip()
            ' Neatify
            cToolstripUtils.HideRepeatingSeparators(Me.m_tsRunPSD)

            ' Synchronize plot with Ecopath results
            Me.SynchronizePlot()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters

            ' Detach format providers
            Me.m_fpNoOfPointsPSD.Release()
            Me.m_fpMinWeight.Release()
            Me.m_fpNoOfPointsMovAvg.Release()

            Me.m_zgh.Detach()
            Me.m_zgh = Nothing

            ' Detach from show/hide groups command
            If Not Object.ReferenceEquals(Me.m_cmdShowGroups, Nothing) Then
                RemoveHandler Me.m_cmdShowGroups.OnPostInvoke, AddressOf OnAfterShowGroups
                Me.m_cmdShowGroups.RemoveControl(Me.m_tsbnShowHideGroups)
            End If

            ' Detach from core state monitor events
            RemoveHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub MenuItmGroupPB_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tsmiGroupPB.Click

            ' Make sure one checkbox is checked exclusively
            Me.m_tsmiGroupPB.Checked = True
            Me.m_tsmiLorenzen.Checked = Not Me.m_tsmiGroupPB.Checked
            'Disable MeanLat label and combo box
            m_tsmiMeanLat.Enabled = Me.m_tsmiLorenzen.Checked
            m_tscbxMeanLat.Enabled = Me.m_tsmiLorenzen.Checked

            ' JS to JH: on this event, please update the core PSD params immediately to 
            '           let the core know that PSD needs to re-run
            Me.UpdateVariables()

        End Sub

        Private Sub MenuItmLorenzen_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tsmiLorenzen.Click

            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters

            ' Make sure one checkbox is checked exclusively
            Me.m_tsmiLorenzen.Checked = True
            Me.m_tsmiGroupPB.Checked = Not Me.m_tsmiLorenzen.Checked
            'Enable MeanLat label and combo box
            m_tsmiMeanLat.Enabled = Me.m_tsmiLorenzen.Checked
            m_tscbxMeanLat.Enabled = Me.m_tsmiLorenzen.Checked

            ' JS to JH: on this event, please update the core PSD params immediately to 
            '           let the core know that PSD needs to re-run
            Me.UpdateVariables()

        End Sub

        Private Sub BtnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbtnRun.Click

            ' Grab PSD settings from the GUI and stick them in the core
            Me.UpdateVariables()
            ' Run Ecopath
            Me.UIContext.Core.RunEcoPath()

        End Sub

        Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)
            Me.SynchronizePlot()
        End Sub

        Private Sub OnAfterShowGroups(ByVal cmd As cCommand)
            ' JS to JH: on this event, please update the core PSD params immediately to 
            '           let the core know that PSD needs to re-run
            Me.UpdateVariables()
        End Sub

        Private Sub m_tscbxMeanLat_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tscbxMeanLat.SelectedIndexChanged
            ' JS to JH: on this event, please update the core PSD params immediately to 
            '           let the core know that PSD needs to re-run
            Me.UpdateVariables()
        End Sub

#End Region ' Event handlers

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Synchronize the plot area with Ecopath results.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SynchronizePlot()

            ' This code is optimized to only plot when new results are available
            ' Are Ecopath results available?
            If Me.m_coreStateMonitor.HasPSDRan Then
                ' #Yes: are these results not plotted yet?
                If Me.m_bEcopathResultsPlotted = False Then
                    ' #Yes: Plot the curves
                    Me.PlotCurves()
                    ' Set flag to remind ourselves that these results are plotted
                    Me.m_bEcopathResultsPlotted = True
                End If
            Else
                '#No: Ecopath results have disappeared (or are not yet available)
                'Is the plot populated?
                If Me.m_bEcopathResultsPlotted = True Then
                    ' #Yes: clear the plot
                    Me.InitializePane()
                    ' Set local flag to remind ourselves that the plot is empty
                    Me.m_bEcopathResultsPlotted = False
                End If
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure the graph pane to look pretty and dandy.
        ''' </summary>
        ''' <returns>
        ''' The graph pane that was initialized.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function InitializePane() As GraphPane

            Dim pane As GraphPane = Me.m_zedgraph.GraphPane
            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters

            pane.CurveList.Clear()

            'JS 23Mar09: Zedgraph helper performs standardized label, axis styling
            Me.m_zgh.ConfigurePane(My.Resources.PSD_PLOTCAPTION_PSD, _
                                   My.Resources.PSD_XAXISLABEL_BODYWEIGHT, _
                                   My.Resources.PSD_YAXISLABEL_BIOMASS, _
                                   True)

            'JS 15Oct09: Fonts are set via StyleGuide
            'pane.Title.FontSpec.Size = 16
            'pane.Legend.FontSpec.Size = 14
            'pane.XAxis.Title.FontSpec.Size = 14
            'pane.YAxis.Title.FontSpec.Size = 14

            pane.XAxis.Scale.Min = Int(Math.Log10(parms.FirstWeightClass))
            pane.XAxis.Scale.Max = Math.Round(Math.Log10(parms.FirstWeightClass * 2 ^ (Me.UIContext.Core.nWeightClasses - 1)) + 0.4, 0, MidpointRounding.AwayFromZero)
            pane.YAxis.Scale.Min = 0

            pane.YAxis.Cross = 0
            pane.YAxis.CrossAuto = False

            Return pane

        End Function

        Private Sub AddCurves(ByVal pane As GraphPane)

            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim sSystemPSD(Me.UIContext.Core.nWeightClasses) As Single
            Dim sSlope As Single
            Dim sSlopeStdErr As Single
            Dim sIntercept As Single
            Dim sInterceptStdErr As Single
            Dim sCorrelation As Single
            Dim sLowWtClass As Single
            Dim sHighWtClass As Single
            Dim iSampleSize As Integer
            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()
            Dim strLabel As String = ""

            Me.InitLists(resultLists, 2)

            'Find system PSD by summing the group PSD
            Me.FindSystemPSD(sSystemPSD)

            'Find regression of the system PSD
            Me.FindRegression(sSlope, sSlopeStdErr, sIntercept, sInterceptStdErr, sCorrelation, _
                              sLowWtClass, sHighWtClass, iSampleSize, sSystemPSD)

            For iWtClass As Integer = 1 To Me.UIContext.Core.nWeightClasses
                If sSystemPSD(iWtClass) * 1000000000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    'PSD data
                    resultLists(0).Add(Math.Log10(sXValue), Math.Log10(sSystemPSD(iWtClass) * 1000000000)) '* 1000000000 for plotting purpose
                    'PSD regression data
                    resultLists(1).Add(Math.Log10(sXValue), sSlope * Math.Log10(sXValue) + sIntercept)

                End If
            Next

            'PSD plot
            Me.AddCurveToGraphPane(pane, resultLists(0), "", Color.Transparent)
            'PSD regression plot
            If iSampleSize = 2 Then
                'Without std err
                strLabel = String.Format(My.Resources.PSD_GRAPH_REGRESSION_LABEL_WO_STDERR, sg.FormatNumber(sSlope), _
                                    sg.FormatNumber(sIntercept), sg.FormatNumber(sCorrelation) & vbCrLf, _
                                    sg.FormatNumber(sLowWtClass), sg.FormatNumber(sHighWtClass), sg.FormatNumber(iSampleSize))
            Else
                'With std err
                strLabel = String.Format(My.Resources.PSD_GRAPH_REGRESSION_LABEL_W_STDERR, sg.FormatNumber(sSlope), sg.FormatNumber(sSlopeStdErr), _
                                    sg.FormatNumber(sIntercept), sg.FormatNumber(sInterceptStdErr), sg.FormatNumber(sCorrelation) & vbCrLf, _
                                    sg.FormatNumber(sLowWtClass), sg.FormatNumber(sHighWtClass), sg.FormatNumber(iSampleSize))
            End If
            Me.AddCurveToGraphPane(pane, resultLists(1), strLabel, Color.Black)
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(ByVal pane As GraphPane, ByVal list As PointPairList, _
                                        ByVal strLabel As String, ByVal lineClr As Color)
            Dim lnItem As LineItem

            lnItem = pane.AddCurve(strLabel, list, lineClr)

            If lineClr = Color.Transparent Then
                lnItem.Line.IsVisible = False
                lnItem.Symbol.Type = SymbolType.Circle
                lnItem.Symbol.Border.IsVisible = False
                lnItem.Symbol.Fill.IsVisible = True
                lnItem.Symbol.Fill.Brush = Brushes.Black
            Else
                lnItem.Line.IsVisible = True
                lnItem.Symbol.Type = SymbolType.None
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update PSD variables from user settings.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateVariables()

            ' JS to JH: This method is now abused to update core PSD params with
            '           UI changed vars in order to let the core know that PSD 
            '           needs to re-run. UpdateVariables may not be the best way
            '           to do this, I'm not sure. It seems that two calls to
            '           UpdateVariables are necessary to make new values reach
            '           the actual core PSD computations.

            Dim grpInput As cEcoPathGroupInput = Nothing
            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()

            'Mortality type
            If m_tsmiGroupPB.Checked Then
                parms.MortalityType = ePSDMortalityTypes.GroupZ
            ElseIf m_tsmiLorenzen.Checked Then
                parms.MortalityType = ePSDMortalityTypes.Lorenzen
            End If

            'Climate type
            If m_tsmiLorenzen.Checked Then
                parms.ClimateType = DirectCast(m_tscbxMeanLat.SelectedIndex, eClimateTypes)
            End If

            'Group included in PSD 
            For iGroup As Integer = 1 To Me.UIContext.Core.nLivingGroups
                grpInput = Me.UIContext.Core.EcoPathGroupInputs(iGroup)
                parms.GroupIncluded(iGroup) = sg.GroupVisible(iGroup)
            Next

        End Sub

        Private Sub UpdatePlot()
            Me.m_zedgraph.AxisChange()
            Me.m_zedgraph.Refresh()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update toolstrip item states.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateToolstrip()

            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters
            Dim grpInput As cEcoPathGroupInput = Nothing
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()

            'Mortality type
            Select Case parms.MortalityType
                Case ePSDMortalityTypes.GroupZ
                    Me.m_tsmiGroupPB.Checked = True
                Case ePSDMortalityTypes.Lorenzen
                    Me.m_tsmiLorenzen.Checked = True
            End Select

            'Climate type
            If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                Me.m_tscbxMeanLat.Enabled = True
                Me.m_tscbxMeanLat.SelectedIndex = parms.ClimateType
            End If

            'Group included in PSD
            ' ToDo_JS or ToDo_JH: The groupvisible flags are meant solely for showing/hiding groups in graphs.
            ' The PSD group inclusion settings is separate behaviour, and should perhaps not alter the EwE group 
            ' visibility settings. 
            '
            ' We can solve this by either:
            '    1. building a different show/hide group interface solely for use with PSD, 
            '    2. reuse the current show/hide group interface but make it operate on different data,
            '    3. reuse show/hide groups but do NOT store PSD group inclusion settings in the database.
            '
            ' 1) is quite a bit of work
            ' 2) is possible by passing an array of show/hide flags into the command.Tag, which we could make
            '    the show/hide dialog operate on. This is pretty hack. Additionally, the hijacked dialog needs to 
            '    display an alternate caption to distinguish its behaviour from regular show/hide group behaviour.
            ' 3) is probably the best option
            'For iGroup As Integer = 1 To m_core.nLivingGroups
            '    grpInput = m_core.EcoPathGroupInputs(iGroup)
            '    sg.GroupVisible(iGroup) = grpInput.PSDIncluded
            'Next

        End Sub

        Private Sub PlotCurves()
            Me.AddCurves(Me.InitializePane())
            Me.UpdatePlot()
        End Sub

        Private Sub FindSystemPSD(ByVal sSystemPSD() As Single)

            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            'Find the system PSD by summing the group PSD
            For iGroup As Integer = 1 To Me.UIContext.Core.nLivingGroups
                If parms.GroupIncluded(iGroup) Then
                    grpOutput = Me.UIContext.Core.EcoPathGroupOutputs(iGroup)
                    For iWtClass As Integer = 1 To Me.UIContext.Core.nWeightClasses
                        sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                    Next
                End If
            Next

        End Sub

        Private Sub FindRegression(ByRef sSlope As Single, ByRef sSlopeStdErr As Single, _
                                   ByRef sIntercept As Single, ByRef sInterceptStdErr As Single, _
                                   ByRef sCorrelation As Single, ByRef sLowWtClass As Single, ByRef sHighWtClass As Single, _
                                   ByRef iSampleSize As Integer, ByVal sSystemPSD() As Single)

            Dim sXValue As Single = 0
            Dim dSumX As Double = 0
            Dim dSumY As Double = 0
            Dim dSumXSq As Double = 0
            Dim dSumYSq As Double = 0
            Dim dSumXY As Double = 0
            Dim iNum As Integer = 0
            Dim sXMin As Single = -1
            Dim sXMax As Single
            Dim dXMean As Double
            Dim dYMean As Double
            Dim dSumXdevYdev As Double = 0
            Dim dSumXdevSq As Double = 0
            Dim dSumYdevSq As Double = 0
            Dim dXStdDev As Double
            Dim dYStdDev As Double
            Dim dEstStdErr As Double
            Dim parms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters

            For iWtClass As Integer = 1 To Me.UIContext.Core.nWeightClasses
                If sSystemPSD(iWtClass) * 1000000000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    dSumX = dSumX + Math.Log10(sXValue)
                    dSumY = dSumY + Math.Log10(sSystemPSD(iWtClass) * 1000000000.0)
                    dSumXSq = dSumXSq + Math.Log10(sXValue) ^ 2
                    dSumYSq = dSumYSq + Math.Log10(sSystemPSD(iWtClass) * 1000000000.0) ^ 2
                    dSumXY = dSumXY + Math.Log10(sXValue) * Math.Log10(sSystemPSD(iWtClass) * 1000000000.0)

                    'v.5
                    'sXValue = iWtClass

                    'dSumX = dSumX + sXValue
                    'dSumY = dSumY + Math.Log10(sSystemPSD(iWtClass))
                    'End v.5
                    If sXMin < 0 Then sXMin = sXValue
                    sXMax = sXValue
                    iNum = iNum + 1
                End If
            Next
            dXMean = dSumX / iNum
            dYMean = dSumY / iNum

            For iWtClass As Integer = 1 To Me.UIContext.Core.nWeightClasses
                If sSystemPSD(iWtClass) * 1000000000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    dSumXdevYdev = dSumXdevYdev + (Math.Log10(sXValue) - dXMean) * (Math.Log10(sSystemPSD(iWtClass) * 1000000000) - dYMean)
                    dSumXdevSq = dSumXdevSq + (Math.Log10(sXValue) - dXMean) ^ 2
                    dSumYdevSq = dSumYdevSq + (Math.Log10(sSystemPSD(iWtClass) * 1000000000) - dYMean) ^ 2

                    'v.5
                    'sXValue = iWtClass

                    'dSumXdevYdev = dSumXdevYdev + ((sXValue) - dXMean) * (Math.Log10(sSystemPSD(iWtClass)) - dYMean)
                    'dSumXdevSq = dSumXdevSq + ((sXValue) - dXMean) ^ 2
                    'End v.5
                End If
            Next

            sSlope = CSng(dSumXdevYdev / dSumXdevSq)
            sIntercept = CSng(dYMean - sSlope * dXMean)

            dXStdDev = Math.Sqrt(dSumXdevSq / (iNum - 1))
            dYStdDev = Math.Sqrt(dSumYdevSq / (iNum - 1))
            dEstStdErr = Math.Sqrt((iNum - 1) * (dYStdDev ^ 2 - sSlope ^ 2 * dXStdDev ^ 2) / (iNum - 2))
            sSlopeStdErr = CSng(dEstStdErr / (Math.Sqrt(iNum - 1) * dXStdDev))
            sInterceptStdErr = CSng(dEstStdErr * Math.Sqrt((1 / iNum) + (dXMean ^ 2 / ((iNum - 1) * dXStdDev ^ 2))))

            sCorrelation = CSng((iNum * dSumXY - dSumX * dSumY) / _
                           (Math.Sqrt(iNum * dSumXSq - dSumX ^ 2) * Math.Sqrt(iNum * dSumYSq - dSumY ^ 2)))
            sLowWtClass = sXMin
            sHighWtClass = sXMax
            iSampleSize = iNum

        End Sub

#End Region ' Helper methods

    End Class

End Namespace