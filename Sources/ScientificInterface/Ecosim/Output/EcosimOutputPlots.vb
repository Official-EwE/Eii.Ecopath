#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Controls
Imports ScientificInterface.Other
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ZedGraph
Imports System.IO
Imports System.Text

#End Region

Namespace Ecosim

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class EcosimOutputPlots

#Region " Variables "

        Private m_core As cCore
        Private m_parms As cEcoSimModelParameters
        Private m_paneMaster As MasterPane = Nothing
        Private m_sg As cStyleGuide = Nothing
        Private m_zgh As cZedGraphHelper = Nothing

        Private Enum ePaneTypes As Integer
            Biomass = 1
            ConsumptionBiomass
            PredationMortality
            Mortality
            FeedingTime
            Prey
            Yield
            AvgWeightOrProdCons
        End Enum

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            Me.InitializeComponent()

            Me.m_core = cCore.GetInstance()
            Me.m_parms = m_core.EcoSimModelParameters()
            Me.m_sg = cStyleGuide.GetInstance()

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            Dim group As cCoreGroupBase = Nothing

            ' Add group names into listbox
            For i As Integer = 1 To Me.m_core.nGroups
                group = Me.m_core.EcoSimGroupInputs(i)
                lbGroups.Items.Add(New cGroupListBox.cGroupItem(group))
            Next
            Me.lbGroups.Refresh()

            Me.m_paneMaster = Me.zgcPlots.MasterPane
            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.m_core, Me.zgcPlots, [Enum].GetValues(GetType(ePaneTypes)).Length)
            Me.m_zgh.ShowPointValue = True

            Me.ConfigurePane(ePaneTypes.Biomass, My.Resources.HEADER_BIOMASS)
            Me.ConfigurePane(ePaneTypes.ConsumptionBiomass, My.Resources.HEADER_CONSUMPTIONBIOMASS)
            Me.ConfigurePane(ePaneTypes.PredationMortality, My.Resources.ECOSIM_PLOT_CAPTION_PREDMORT)
            Me.ConfigurePane(ePaneTypes.Mortality, My.Resources.ECOSIM_PLOT_CAPTION_MORT)
            Me.ConfigurePane(ePaneTypes.FeedingTime, My.Resources.HEADER_FEEDINGTIME)
            Me.ConfigurePane(ePaneTypes.Prey, My.Resources.ECOSIM_PLOT_CAPTION_PREYPERC)
            Me.ConfigurePane(ePaneTypes.Yield, My.Resources.HEADER_YIELD)
            ' Need to test StanZaGroup..Sometimes displayed as Average weight
            ' update it in the actual rendering.
            Me.ConfigurePane(ePaneTypes.AvgWeightOrProdCons, My.Resources.ECOSIM_PLOT_CAPTION_PRODCONS)

            'Me.m_zgh = New ZedGraphHelper(Me.zgcPlots, 8)
            lbGroups.SelectedIndex = 0

            Me.UpdateControls()
            Me.UpdateColors()

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.CoreComponents = Nothing

            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.m_sg = Nothing

            Me.m_paneMaster = Nothing
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
            If ((changeType And cStyleGuide.eChangeType.Colours) = cStyleGuide.eChangeType.Colours) Then
                Me.UpdateColors()
            End If
        End Sub

        ''' <summary>
        ''' Event hander to display results for another group
        ''' </summary>
        Private Sub lbGroups_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles lbGroups.SelectedIndexChanged

            Me.AddCurves()
            Me.m_zgh.RescaleAndRedraw()

            'Display pred and prey ranks
            Me.DisplayRanks()

        End Sub

        ''' <summary>
        ''' Event handler for closing the form
        ''' </summary>
        Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        ''' <summary>
        ''' Event handler for saving the value to .csv file
        ''' </summary>
        Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click

            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmd As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
            Dim bSaveAnnual As Boolean = False 'Save each time steps
            Dim writer As New cEcosimResultWriter(Me.m_core)

            cmd.Invoke("", My.Resources.ECOSIM_PROMPT_SAVEDESTINATION)

            If (cmd.Result <> Windows.Forms.DialogResult.OK) Then Return
            If (String.IsNullOrEmpty(cmd.Directory)) Then Return

            Select Case MsgBox(My.Resources.ECOSIM_PROMPT_SAVEANNUAL, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    bSaveAnnual = True
                Case MsgBoxResult.No
                    bSaveAnnual = False
                Case MsgBoxResult.Cancel
                    Return
            End Select

            writer.WriteResults(cmd.Directory, bSaveAnnual, Me.lbGroups.SelectedIndex + 1)

        End Sub

        Private Sub btnTimeSeries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmd As cCommand = cmdh.GetCommand("LoadTimeSeries")
            If (cmd IsNot Nothing) Then cmd.Invoke()
        End Sub

        Private Sub btnShowAllFits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowAllFits.Click
            Dim showAllFitsDlg As New frmShowAllFits
            showAllFitsDlg.ShowDialog()
        End Sub

#End Region ' Event handlers

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If (msg.Source = eCoreComponentType.TimeSeries) Then
                Me.UpdateControls()
            End If
        End Sub

#End Region ' Overrides

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a plot on the main graph
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub ConfigurePane(ByVal iPane As ePaneTypes, ByVal strTitle As String)

            Me.m_zgh.ConfigurePane(strTitle, _
                       "", CDbl(Me.m_core.EcosimFirstYear), CDbl(Me.m_core.EcosimFirstYear + (m_core.nEcosimTimeSteps / cCore.N_MONTHS)), _
                       "", 0, 0, _
                       False, LegendPos.Top, CInt(iPane))

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, creates a ready-to-eat list of PointPairList instances.
        ''' </summary>
        ''' <param name="iNumLists">Number of lists to create.</param>
        ''' -------------------------------------------------------------------
        Private Function InitLists(ByVal iNumLists As Integer) As List(Of PointPairList)

            Dim lPPL As New List(Of PointPairList)
            For i As Integer = 1 To iNumLists
                lPPL.Add(New PointPairList())
            Next
            Return lPPL

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the values from core and add them into graph
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub AddCurves()

            ' ToDo_JS: Find way to update colours in existing curves [CurveList.Item(x).Color = ...]
            '          Use zedgraphhelper for this

            Dim iCount As Integer = 0
            Dim dXValue As Double = 0
            Dim item As cGroupListBox.cGroupItem = DirectCast(lbGroups.SelectedItem, cGroupListBox.cGroupItem)
            Dim iGroup As Integer = item.Group.Index
            Dim groupSimOut As cEcosimGroupOutput = Me.m_core.EcoSimGroupOutputs(iGroup)

            Dim pplB As New PointPairList()
            Dim pplConsB As New PointPairList()
            Dim pplFeedTime As New PointPairList()
            Dim pplYield As New PointPairList()
            Dim pplAvgWorProdCons As New PointPairList()

            Dim pplMortTotal As New PointPairList()
            Dim pplMortPredation As New PointPairList()
            Dim pplMortFishing As New PointPairList()

            'Set the master pane title
            Me.m_zgh.Configure(item.Group.Name)

            ' Clear all panes
            For Each pt As ePaneTypes In [Enum].GetValues(GetType(ePaneTypes))
                With Me.m_zgh.GetPane(CInt(pt))
                    .CurveList.Clear()
                    .AxisChange()
                End With
            Next

            ' Do not render when sim has not ran
            If Not Me.m_core.StateMonitor.HasEcosimRan Then Return

            For i As Integer = 1 To m_core.nEcosimTimeSteps
                ' Time
                dXValue = Me.m_core.EcosimFirstYear + (i / cCore.N_MONTHS)
                ' Get sim results
                pplB.Add(dXValue, groupSimOut.Biomass(i))
                pplConsB.Add(dXValue, groupSimOut.ConsumpBiomass(i))
                pplFeedTime.Add(dXValue, groupSimOut.FeedingTime(i))
                pplYield.Add(dXValue, groupSimOut.Yield(i))
                If groupSimOut.isMultiStanza() Then
                    pplAvgWorProdCons.Add(dXValue, groupSimOut.AvgWeight(i))
                Else
                    pplAvgWorProdCons.Add(dXValue, groupSimOut.ProdConsump(i))
                End If
                pplMortTotal.Add(dXValue, groupSimOut.TotalMort(i))
                pplMortPredation.Add(dXValue, groupSimOut.PredMort(i))
                pplMortFishing.Add(dXValue, groupSimOut.FishMort(i))
            Next

            Me.AddCurveToGraphPane(ePaneTypes.Biomass, Me.m_zgh.CreateLineItem("", cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Black, pplB))
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassRel, iGroup, Color.Blue)
                Me.AddCurveToGraphPane(ePaneTypes.Biomass, li, False)
            Next li
            ' Fixes issue 604:
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassAbs, iGroup, Color.Green)
                Me.AddCurveToGraphPane(ePaneTypes.Biomass, li, False)
            Next li

            Me.AddCurveToGraphPane(ePaneTypes.ConsumptionBiomass, Me.m_zgh.CreateLineItem("", cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Black, pplConsB))
            Me.AddCurveToGraphPane(ePaneTypes.FeedingTime, Me.m_zgh.CreateLineItem("", cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Black, pplFeedTime))

            Me.AddCurveToGraphPane(ePaneTypes.Yield, Me.m_zgh.CreateLineItem("", cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Black, pplYield))
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.Catches, iGroup, Color.Red)
                Me.AddCurveToGraphPane(ePaneTypes.Yield, li, False)
            Next li
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.CatchesForcing, iGroup, Color.Blue)
                Me.AddCurveToGraphPane(ePaneTypes.Yield, li, False)
            Next li

            If groupSimOut.isMultiStanza() Then

                Me.UpdateGraphPaneTitle(ePaneTypes.AvgWeightOrProdCons, My.Resources.HEADER_AVGERAGEWEIGHT)

                Me.AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, Me.m_zgh.CreateLineItem("", cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Black, pplAvgWorProdCons))
                For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.AverageWeight, iGroup, Color.Blue)
                    Me.AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, li, False)
                Next li

            Else

                Me.UpdateGraphPaneTitle(ePaneTypes.AvgWeightOrProdCons, My.Resources.ECOSIM_PLOT_CAPTION_PRODCONS)
                Me.AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, Me.m_zgh.CreateLineItem("", cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Black, pplAvgWorProdCons))

            End If

            Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem(My.Resources.HEADER_TOTAL, cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Black, pplMortTotal), True)
            Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem(My.Resources.HEADER_PREDATION, cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Red, pplMortPredation), False)
            Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem(My.Resources.HEADER_FISHING, cZedGraphHelper.eCurveTypes.EcosimOutput, Color.Blue, pplMortFishing), False)
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.TotalMortality, iGroup, Color.Green)
                Me.AddCurveToGraphPane(ePaneTypes.Mortality, li, False)
            Next li

            'VC 07apr09: F values (type = 4) should not be plotted as they are used to drive the model
            'For Each ppl As PointPairList In Me.GetTSData(eTimeSeriesType.FishingMortality)
            '    Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem("Fishing", ZedGraphHelper.eCurveTypes.TimeSeries, Color.Red, ppl), False)
            'Next ppl

            'Predation mortality 
            iCount = 0
            For i As Integer = 1 To m_core.nLivingGroups
                If groupSimOut.isPred(i) Then
                    Dim ppl As New PointPairList
                    For j As Integer = 1 To m_core.nEcosimTimeSteps
                        dXValue = Me.m_core.EcosimFirstYear + (j / cCore.N_MONTHS)
                        ppl.Add(dXValue, groupSimOut.Predation(i, j))
                    Next
                    Me.AddCurveToGraphPane(ePaneTypes.PredationMortality, Me.m_zgh.CreateLineItem(cZedGraphHelper.eCurveTypes.EcosimOutput, i, ppl), (iCount = 0))
                    iCount += 1
                End If
            Next

            'Prey %
            iCount = 0
            For i As Integer = 1 To m_core.nLivingGroups
                If groupSimOut.isPrey(i) Then
                    Dim ppl As New PointPairList
                    For j As Integer = 1 To m_core.nEcosimTimeSteps
                        dXValue = Me.m_core.EcosimFirstYear + (j / cCore.N_MONTHS)
                        ppl.Add(dXValue, groupSimOut.PreyPercentage(i, j) * 100)
                    Next
                    Me.AddCurveToGraphPane(ePaneTypes.Prey, Me.m_zgh.CreateLineItem(cZedGraphHelper.eCurveTypes.EcosimOutput, i, ppl), (iCount = 0))
                    iCount += 1
                End If
            Next

        End Sub

#Region " Time series "

        Private Function GetTimeSeriesLineItems(ByVal TSType As eTimeSeriesType, ByVal iGroup As Integer, ByVal clr As Color) As List(Of LineItem)

            Dim lli As New List(Of LineItem)
            Dim ppt As PointPairList = Nothing
            Dim ts As cTimeSeries = Nothing
            Dim gts As cGroupTimeSeries = Nothing

            For i As Integer = 1 To m_core.nTimeSeries
                ts = Me.m_core.EcosimTimeSeries(i)
                If ts.TimeSeriesType = TSType Then
                    If TypeOf ts Is cGroupTimeSeries Then
                        gts = DirectCast(ts, cGroupTimeSeries)
                        If (gts.GroupIndex = iGroup) And gts.Enabled() Then
                            lli.Add(Me.ToTimeSeriesLineItem(gts, clr))
                        End If
                    End If
                End If
            Next

            Return lli

        End Function

        Private Function ToTimeSeriesLineItem(ByVal gts As cGroupTimeSeries, ByVal clr As Color) As LineItem

            Dim ppt As New PointPairList
            Dim dScale As Single = 1.0F

            If gts.TimeSeriesType = eTimeSeriesType.BiomassRel Or _
                 gts.TimeSeriesType = eTimeSeriesType.TotalMortality Or _
                    gts.TimeSeriesType = eTimeSeriesType.AverageWeight Then
                If gts.eDataQ > 0 Then
                    dScale = 1.0F / gts.eDataQ
                End If
            End If

            Dim da() As Single = gts.ShapeData()
            For j As Integer = 1 To da.Length - 1
                If (da(j) > 0) Then
                    ppt.Add(Me.m_core.EcosimFirstYear + j - 1, da(j) * dScale)
                End If
            Next
            Return Me.m_zgh.CreateLineItem(gts.Name, cZedGraphHelper.eCurveTypes.TimeSeries, clr, ppt)

        End Function

#End Region ' Time series

        Private Sub DisplayRanks()

            Dim iGroup As Integer = lbGroups.SelectedIndex + 1
            Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(iGroup)

            Dim avgPredConsumption As New List(Of Single)
            Dim avgPredIndex As New List(Of Integer)

            Dim avgPreyConsumption As New List(Of Single)
            Dim avgpreyIndex As New List(Of Integer)

            For i As Integer = 1 To m_core.nLivingGroups

                If grpOutput.isPred(i) Then
                    avgPredConsumption.Add(grpOutput.AvgPredConsumption(i))
                    avgPredIndex.Add(i)
                End If

                If grpOutput.isPrey(i) Then
                    avgPreyConsumption.Add(grpOutput.AvgPreyConsumption(i))
                    avgpreyIndex.Add(i)
                End If

            Next

            Dim iAvgPred() As Integer = avgPredIndex.ToArray
            Me.SortRanks(avgPredConsumption.ToArray, iAvgPred)
            Me.PopulateGroupListBox(Me.lbPredRanks, iAvgPred)

            Dim aiAvgPrey() As Integer = avgpreyIndex.ToArray
            Me.SortRanks(avgPreyConsumption.ToArray, aiAvgPrey)
            Me.PopulateGroupListBox(Me.lbPreyRanks, aiAvgPrey)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="asPredConsumption"></param>
        ''' <param name="aiGroups"></param>
        ''' -------------------------------------------------------------------
        Private Sub SortRanks(ByVal asPredConsumption() As Single, ByVal aiGroups() As Integer)

            'Use a simple bubblesort algorithm here
            For i As Integer = 0 To asPredConsumption.Length - 2
                For j As Integer = asPredConsumption.Length - 1 To i + 1 Step -1
                    If asPredConsumption(j) > asPredConsumption(j - 1) Then
                        ' swap 
                        Dim sTmp As Single = asPredConsumption(j)
                        asPredConsumption(j) = asPredConsumption(j - 1)
                        asPredConsumption(j - 1) = sTmp
                        Dim iTmp As Integer = aiGroups(j)
                        aiGroups(j) = aiGroups(j - 1)
                        aiGroups(j - 1) = iTmp
                    End If
                Next
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate a group list box with Ecopath groups.
        ''' </summary>
        ''' <param name="l">Listbox to add items to.</param>
        ''' <param name="aiGroupIndex">Array of group index values.</param>
        ''' -------------------------------------------------------------------
        Private Sub PopulateGroupListBox(ByVal l As cGroupListBox, ByVal aiGroupIndex() As Integer)

            Dim group As cEcoPathGroupInput = Nothing

            l.Items.Clear()

            For i As Integer = 0 To aiGroupIndex.Length - 1

                'Dim item As cColorItem = m_PoolColor(items(i))
                group = Me.m_core.EcoPathGroupInputs(aiGroupIndex(i))
                l.Items.Add(New cGroupListBox.cGroupItem(group))
            Next
            l.Refresh()

        End Sub

        Private Sub UpdateGraphPaneTitle(ByVal paneType As ePaneTypes, ByVal strTitle As String)
            Dim gp As GraphPane = m_paneMaster.PaneList(CInt(paneType) - 1)
            gp.Title.Text = strTitle
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add one curve into the graph pane
        ''' </summary>
        ''' <param name="paneType">Index of the graph pane</param>
        ''' <param name="li">The curve</param>
        ''' -------------------------------------------------------------------
        Private Sub AddCurveToGraphPane(ByVal paneType As ePaneTypes, ByVal li As LineItem, _
                                        Optional ByVal bClearExistingCurves As Boolean = True)

            Dim lli As New List(Of ZedGraph.LineItem)
            lli.Add(li)
            Me.AddCurvesToGraphPane(paneType, lli, bClearExistingCurves)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add multiple curves into the graph pane
        ''' </summary>
        ''' <param name="paneType">The idnex of the graph pane</param>
        ''' <param name="lli">The lists of data points for the multiple curves</param>
        ''' <remarks>Overloaded method with different color options.</remarks>
        ''' -------------------------------------------------------------------
        Private Sub AddCurvesToGraphPane(ByVal paneType As ePaneTypes, ByVal lli As List(Of LineItem), _
                                         Optional ByVal bClearExistingCurves As Boolean = True)

            Me.m_zgh.PlotLines(lli, CInt(paneType), True, bClearExistingCurves)

        End Sub

        Private Sub UpdateColors()
            m_paneMaster.Fill = New Fill(Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            For Each p As GraphPane In Me.m_paneMaster.PaneList
                p.Chart.Fill = New Fill(Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            Next
        End Sub

        Private Sub UpdateControls()
            Me.btnShowAllFits.Enabled = Me.m_core.HasAppliedTimeSeries()
        End Sub

#End Region ' Helper methods

    End Class

End Namespace
