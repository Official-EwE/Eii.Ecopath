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
    ''' Form, implementing the Ecosim output plots interface.
    ''' </summary>
    Public Class EcosimOutputPlots

#Region " Variables "

        Private m_parms As cEcoSimModelParameters
        Private m_paneMaster As MasterPane = Nothing
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
            FleetFishingMortality
        End Enum

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim group As cCoreGroupBase = Nothing

            Me.m_parms = Me.UIContext.Core.EcoSimModelParameters()
            Me.m_paneMaster = Me.m_graph.MasterPane

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_graph, [Enum].GetValues(GetType(ePaneTypes)).Length)
            Me.m_zgh.ShowPointValue = True
            Me.m_zgh.IsTrackVisiblity = False

            Me.ConfigurePane(ePaneTypes.Biomass, My.Resources.HEADER_BIOMASS)
            Me.ConfigurePane(ePaneTypes.ConsumptionBiomass, My.Resources.HEADER_CONSUMPTIONBIOMASS)
            Me.ConfigurePane(ePaneTypes.PredationMortality, My.Resources.HEADER_PREDMORT)
            ' JS: EwE5 distinguishes between P/B (PP=1) and TotalMort (PP=0) for 
            Me.ConfigurePane(ePaneTypes.Mortality, My.Resources.ECOSIM_PLOT_CAPTION_MORT_CONS)
            Me.ConfigurePane(ePaneTypes.FeedingTime, My.Resources.HEADER_FEEDINGTIME)
            Me.ConfigurePane(ePaneTypes.Prey, My.Resources.HEADER_PREY_PERCENTAGE)
            Me.ConfigurePane(ePaneTypes.Yield, My.Resources.HEADER_YIELD)
            Me.ConfigurePane(ePaneTypes.FleetFishingMortality, My.Resources.HEADER_FISHINGMORTALITY)
            ' Need to test StanZaGroup..Sometimes displayed as Average weight
            ' update it in the actual rendering.
            Me.ConfigurePane(ePaneTypes.AvgWeightOrProdCons, My.Resources.HEADER_PRODCONS)

            Me.m_lbGroups.Attach(Me.UIContext)
            Me.m_lbPredators.Attach(Me.UIContext)
            Me.m_lbPrey.Attach(Me.UIContext)
            Me.m_lbFleets.Attach(Me.UIContext)

            Me.m_lbGroups.SelectedIndex = 0

            Me.UpdateColors()

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}
            AddHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.CoreComponents = Nothing
            Me.m_lbGroups.Detach()
            Me.m_lbPredators.Detach()
            Me.m_lbPrey.Detach()
            Me.m_lbFleets.Detach()

            RemoveHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

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
            Handles m_lbGroups.SelectedIndexChanged

            Me.AddCurves()
            Me.m_zgh.RescaleAndRedraw()

            'Display pred and prey ranks
            Me.ShowGroup()

        End Sub

        Private Sub OnSaveData(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSaveData.Click

            Dim cmd As cCommand = Me.CommandHandler.GetCommand("ExportEcosimResultsToCSV")
            cmd.Tag = Me.m_lbGroups.SelectedGroupIndex
            cmd.Invoke()
            cmd.Tag = cCore.NULL_VALUE

        End Sub

#End Region ' Event handlers

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a plot on the main graph
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub ConfigurePane(ByVal iPane As ePaneTypes, ByVal strTitle As String)

            Me.m_zgh.ConfigurePane(strTitle, _
                       "", _
                       CDbl(Me.UIContext.Core.EcosimFirstYear), _
                       CDbl(Me.UIContext.Core.EcosimFirstYear + (Me.UIContext.Core.nEcosimTimeSteps / cCore.N_MONTHS)), _
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
            Dim iGroup As Integer = Math.Max(1, Me.m_lbGroups.SelectedGroupIndex)
            Dim groupSimOut As cEcosimGroupOutput = Me.UIContext.Core.EcoSimGroupOutputs(iGroup)
            Dim group As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(iGroup)

            Dim pplB As New PointPairList()
            Dim pplConsB As New PointPairList()
            Dim pplFeedTime As New PointPairList()
            Dim pplYield As New PointPairList()
            Dim pplAvgWorProdCons As New PointPairList()

            Dim pplMortTotal As New PointPairList()
            Dim pplMortPredation As New PointPairList()
            Dim pplMortFishing As New PointPairList()

            'Set the master pane title
            Me.m_zgh.Configure(groupSimOut.Name)

            ' Configure mort pane caption
            If group.IsConsumer Then
                Me.ConfigurePane(ePaneTypes.Mortality, My.Resources.ECOSIM_PLOT_CAPTION_MORT_CONS)
            Else
                Me.ConfigurePane(ePaneTypes.Mortality, My.Resources.ECOSIM_PLOT_CAPTION_MORT_PROD)
            End If

            ' Clear all panes
            For Each pt As ePaneTypes In [Enum].GetValues(GetType(ePaneTypes))
                With Me.m_zgh.GetPane(CInt(pt))
                    .CurveList.Clear()
                    .AxisChange()
                End With
            Next

            ' Do not render when sim has not ran
            If Not Me.UIContext.Core.StateMonitor.HasEcosimRan Then Return

            Dim applYieldFleet(Me.UIContext.Core.nFleets) As PointPairList
            Dim applFishMortFleet(Me.UIContext.Core.nFleets) As PointPairList

            For i As Integer = 1 To Me.UIContext.Core.nFleets
                applYieldFleet(i) = New PointPairList()
                applFishMortFleet(i) = New PointPairList()
            Next

            For i As Integer = 1 To Me.UIContext.Core.nEcosimTimeSteps
                ' Time
                dXValue = Me.UIContext.Core.EcosimFirstYear + (i / cCore.N_MONTHS)
                ' Get sim results
                pplB.Add(dXValue, groupSimOut.Biomass(i))
                pplConsB.Add(dXValue, groupSimOut.ConsumpBiomass(i))
                pplFeedTime.Add(dXValue, groupSimOut.FeedingTime(i))
                pplYield.Add(dXValue, groupSimOut.Yield(i))
                For iFleet As Integer = 1 To Me.UIContext.Core.nFleets
                    applYieldFleet(iFleet).Add(dXValue, CSng(groupSimOut.CatchByFleet(iFleet, i)))
                    applFishMortFleet(iFleet).Add(dXValue, CSng(groupSimOut.FishingMortByFleet(iFleet, i)))
                Next
                If groupSimOut.isMultiStanza() Then
                    pplAvgWorProdCons.Add(dXValue, groupSimOut.AvgWeight(i))
                Else
                    pplAvgWorProdCons.Add(dXValue, groupSimOut.ProdConsump(i))
                End If
                pplMortTotal.Add(dXValue, groupSimOut.TotalMort(i))
                pplMortPredation.Add(dXValue, groupSimOut.PredMort(i))
                pplMortFishing.Add(dXValue, groupSimOut.FishMort(i))
            Next

            Me.AddCurveToGraphPane(ePaneTypes.Biomass, Me.m_zgh.CreateLineItem(group, pplB))
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassRel, iGroup, Color.Blue)
                Me.AddCurveToGraphPane(ePaneTypes.Biomass, li)
            Next li
            ' Fixes issue 604:
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassAbs, iGroup, Color.Green)
                Me.AddCurveToGraphPane(ePaneTypes.Biomass, li)
            Next li

            Me.AddCurveToGraphPane(ePaneTypes.ConsumptionBiomass, Me.m_zgh.CreateLineItem(group, pplConsB))
            Me.AddCurveToGraphPane(ePaneTypes.FeedingTime, Me.m_zgh.CreateLineItem(group, pplFeedTime))

            For i As Integer = 1 To Me.UIContext.Core.nFleets

                Dim fleet As cFleetInput = Me.UIContext.Core.FleetInputs(i)
                Dim clr As Color = Me.UIContext.StyleGuide.FleetColor(Me.UIContext.Core, i)

                If fleet.Landings(iGroup) > 0 Then
                    Me.AddCurveToGraphPane(ePaneTypes.Yield, _
                                           Me.m_zgh.CreateLineItem(fleet, applYieldFleet(i)), _
                                           True)
                End If
                Me.AddCurveToGraphPane(ePaneTypes.FleetFishingMortality, _
                                       Me.m_zgh.CreateLineItem(fleet, applFishMortFleet(i)), _
                                       True)
            Next
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.Catches, iGroup, Color.Red)
                Me.AddCurveToGraphPane(ePaneTypes.Yield, li, True)
            Next li
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.CatchesForcing, iGroup, Color.Blue)
                Me.AddCurveToGraphPane(ePaneTypes.Yield, li)
            Next li

            If groupSimOut.isMultiStanza() Then

                Me.UpdateGraphPaneTitle(ePaneTypes.AvgWeightOrProdCons, My.Resources.HEADER_AVGERAGEWEIGHT)

                Me.AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, Me.m_zgh.CreateLineItem(group, pplAvgWorProdCons))
                For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.AverageWeight, iGroup, Color.Blue)
                    Me.AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, li)
                Next li

            Else

                Me.UpdateGraphPaneTitle(ePaneTypes.AvgWeightOrProdCons, My.Resources.HEADER_PRODCONS)
                Me.AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, Me.m_zgh.CreateLineItem(group, pplAvgWorProdCons))

            End If

            Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem(My.Resources.HEADER_TOTAL, eLineType.ModelData, Color.Black, pplMortTotal))
            Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem(My.Resources.HEADER_PREDATION, eLineType.ModelData, Color.Red, pplMortPredation))
            Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem(My.Resources.HEADER_FISHING, eLineType.ModelData, Color.Blue, pplMortFishing))
            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.TotalMortality, iGroup, Color.Green)
                Me.AddCurveToGraphPane(ePaneTypes.Mortality, li)
            Next li

            'VC 07apr09: F values (type = 4) should not be plotted as they are used to drive the model
            'For Each ppl As PointPairList In Me.GetTSData(eTimeSeriesType.FishingMortality)
            '    Me.AddCurveToGraphPane(ePaneTypes.Mortality, Me.m_zgh.CreateLineItem("Fishing", ZedGraphHelper.eCurveTypes.TimeSeries, Color.Red, ppl), False)
            'Next ppl

            'Predation mortality 
            iCount = 0
            For i As Integer = 1 To Me.UIContext.Core.nLivingGroups
                If groupSimOut.isPred(i) Then
                    Dim ppl As New PointPairList
                    Dim pred As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(i)
                    For j As Integer = 1 To Me.UIContext.Core.nEcosimTimeSteps
                        dXValue = Me.UIContext.Core.EcosimFirstYear + (j / cCore.N_MONTHS)
                        ppl.Add(dXValue, groupSimOut.Predation(i, j))
                    Next
                    Me.AddCurveToGraphPane(ePaneTypes.PredationMortality, Me.m_zgh.CreateLineItem(pred, ppl))
                    iCount += 1
                End If
            Next

            'Prey %
            iCount = 0
            For i As Integer = 1 To Me.UIContext.Core.nLivingGroups
                If groupSimOut.isPrey(i) Then
                    Dim ppl As New PointPairList
                    Dim prey As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(i)
                    For j As Integer = 1 To Me.UIContext.Core.nEcosimTimeSteps
                        dXValue = Me.UIContext.Core.EcosimFirstYear + (j / cCore.N_MONTHS)
                        ppl.Add(dXValue, groupSimOut.PreyPercentage(i, j) * 100)
                    Next
                    Me.AddCurveToGraphPane(ePaneTypes.Prey, Me.m_zgh.CreateLineItem(prey, ppl))
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

            For i As Integer = 1 To Me.UIContext.Core.nTimeSeries
                ts = Me.UIContext.Core.EcosimTimeSeries(i)
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
                    gts.TimeSeriesType = eTimeSeriesType.AverageWeight Then
                'VC091209; the totalmortality is absolute, not relative, so removed it from here
                ' gts.TimeSeriesType = eTimeSeriesType.TotalMortality Or _
                If gts.eDataQ > 0 Then
                    dScale = 1.0F / gts.eDataQ
                End If
            End If

            Dim da() As Single = gts.ShapeData()
            For j As Integer = 1 To da.Length - 1
                If (da(j) > 0) Then
                    ppt.Add(Me.UIContext.Core.EcosimFirstYear + j - 0.5, da(j) * dScale)
                End If
            Next
            Return Me.m_zgh.CreateLineItem(gts, ppt)

        End Function

#End Region ' Time series

        Private Sub ShowGroup()

            Dim iGroup As Integer = m_lbGroups.SelectedIndex + 1
            Dim grpOutput As cEcosimGroupOutput = Me.UIContext.Core.EcoSimGroupOutputs(iGroup)

            Dim lAvgPredConsumption As New List(Of Single)
            Dim lAvgPredIndex As New List(Of Integer)

            Dim lAvgPreyConsumption As New List(Of Single)
            Dim lAvgPreyIndex As New List(Of Integer)

            Dim lCatch As New List(Of Single)
            Dim lFleetIndex As New List(Of Integer)

            For i As Integer = 1 To Me.UIContext.Core.nLivingGroups

                If grpOutput.isPred(i) Then
                    lAvgPredConsumption.Add(grpOutput.AvgPredConsumption(i))
                    lAvgPredIndex.Add(i)
                End If

                If grpOutput.isPrey(i) Then
                    lAvgPreyConsumption.Add(grpOutput.AvgPreyConsumption(i))
                    lAvgPreyIndex.Add(i)
                End If

            Next

            For i As Integer = 1 To Me.UIContext.Core.nFleets
                If Me.UIContext.Core.FleetInputs(i).Landings(iGroup) > 0 Then
                    Dim sCatch As Single = 0
                    For j As Integer = 1 To Me.UIContext.Core.nEcosimTimeSteps
                        sCatch += grpOutput.CatchByFleet(i, j)
                    Next
                    lCatch.Add(sCatch)
                    lFleetIndex.Add(i)
                End If
            Next

            Me.PopulateGroupListBox(Me.m_lbPredators, lAvgPredIndex.ToArray(), lAvgPredConsumption.ToArray())
            Me.PopulateGroupListBox(Me.m_lbPrey, lAvgPreyIndex.ToArray(), lAvgPreyConsumption.ToArray())
            Me.PopulateFleetListBox(Me.m_lbFleets, lFleetIndex.ToArray(), lCatch.ToArray())

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate a group list box with Ecopath groups.
        ''' </summary>
        ''' <param name="l">Listbox to add items to.</param>
        ''' <param name="aiGroupIndex">Array of group index values.</param>
        ''' -------------------------------------------------------------------
        Private Sub PopulateGroupListBox(ByVal l As cGroupListBox, _
                                         ByVal aiGroupIndex() As Integer, _
                                         ByVal asValues() As Single)

            l.Populate(aiGroupIndex)
            l.Sorted = False
            For i As Integer = 0 To aiGroupIndex.Count - 1
                l.SortValue(aiGroupIndex(i)) = asValues(i)
            Next
            l.SortType = cGroupListBox.eSortType.ValueAsc
            l.Sorted = True

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate a fleet list box with Ecopath fleets.
        ''' </summary>
        ''' <param name="l">Listbox to add items to.</param>
        ''' <param name="aiFleetIndex">Array of fleet index values.</param>
        ''' -------------------------------------------------------------------
        Private Sub PopulateFleetListBox(ByVal l As cFleetListBox, _
                                         ByVal aiFleetIndex() As Integer, _
                                         ByVal asValues() As Single)

            l.Populate(aiFleetIndex)
            l.Sorted = False
            For i As Integer = 0 To aiFleetIndex.Count - 1
                l.SortValue(i) = asValues(i)
            Next
            l.SortType = cFleetListBox.eSortType.ValueAsc
            l.Sorted = True

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
        Private Sub AddCurveToGraphPane(ByVal paneType As ePaneTypes, _
                                        ByVal li As LineItem, _
                                        Optional ByVal bCumulative As Boolean = False)

            Dim lli As New List(Of ZedGraph.LineItem)
            lli.Add(li)
            Me.AddCurvesToGraphPane(paneType, lli, bCumulative)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add multiple curves into the graph pane
        ''' </summary>
        ''' <param name="paneType">The idnex of the graph pane</param>
        ''' <param name="lli">The lists of data points for the multiple curves</param>
        ''' <remarks>Overloaded method with different color options.</remarks>
        ''' -------------------------------------------------------------------
        Private Sub AddCurvesToGraphPane(ByVal paneType As ePaneTypes, _
                                         ByVal lli As List(Of LineItem), _
                                         Optional ByVal bCumulative As Boolean = False)

            Me.m_zgh.PlotLines(lli.ToArray, CInt(paneType), True, False, bCumulative)

        End Sub

        Private Sub UpdateColors()
            m_paneMaster.Fill = New Fill(Me.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            For Each p As GraphPane In Me.m_paneMaster.PaneList
                p.Chart.Fill = New Fill(Me.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            Next
        End Sub

#End Region ' Helper methods

    End Class

End Namespace
