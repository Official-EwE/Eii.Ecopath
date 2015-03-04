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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Text
Imports EwECore
Imports EwECore.Ecosim
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ZedGraph

#End Region

Namespace Ecosim

    ''' <summary>
    ''' Form, implementing the Ecosim output plots by parameter interface.
    ''' </summary>
    Public Class frmEcosimOutputPlotsByParm


#Region " Variables "

        Private m_parms As cEcoSimModelParameters
        Private m_paneMaster As MasterPane = Nothing
        Private m_zgh As cZedGraphHelper = Nothing
        Private m_aiPlotPane([Enum].GetValues(GetType(ePlot)).Length) As Integer
        Private m_abPlotVisible([Enum].GetValues(GetType(ePlot)).Length) As Boolean


        Dim m_TSInterval As eTSDataSetInterval

        Private Enum ePlot As Integer

            plot1
            plot2
            plot3
            plot4
            plot5
            plot6
            plot7
            plot8
            plot9
            plot10
            plot11
            plot12
            plot13
            plot14
            plot15
            plot16
            plot17
            plot18
            plot19
            plot20
            plot21
            plot22
            plot23
            plot24
            plot25
            plot26
            plot27
            plot28
            plot29
            plot30

        End Enum

#End Region ' Variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()

            
            For Each plot As ePlot In [Enum].GetValues(GetType(ePlot))
                Me.m_abPlotVisible(plot) = False
            Next


        End Sub

        Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
                If (MyBase.UIContext IsNot Nothing) Then
                    Me.m_lbGroups.Detach()
                    
                End If
                MyBase.UIContext = value
                If (MyBase.UIContext IsNot Nothing) Then
                    Me.m_lbGroups.Attach(Me.UIContext)
                    
                End If
            End Set
        End Property

#End Region ' Constructors

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim group As cCoreGroupBase = Nothing

            Me.m_parms = Me.UIContext.Core.EcoSimModelParameters()
            Me.m_paneMaster = Me.m_graph.MasterPane

            Me.m_zgh = New cZedGraphHelper()
            Me.ConfigurePlots(True)
            Me.m_zgh.ShowPointValue = True
            Me.m_zgh.IsTrackVisiblity = False

            Me.PopulateParameterListBox()
            Me.UpdateColors()

            Me.m_lbParameter.SelectedIndex = 0  'By default Biomass
            Me.m_lbGroups.SelectedIndex = 0  'By default First Group

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}
            If Me.Core.ActiveTimeSeriesDatasetIndex > 0 Then
                Me.m_TSInterval = Me.Core.TimeSeriesDataset(Me.Core.ActiveTimeSeriesDatasetIndex).TimeSeriesInterval
            Else
                Me.m_TSInterval = eTSDataSetInterval.Annual
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.ConfigurePlots(False)

            Me.CoreComponents = Nothing

            Me.m_paneMaster = Nothing
            Me.m_zgh = Nothing

            MyBase.OnFormClosed(e)

        End Sub

        Protected Overrides Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
            If ((changeType And cStyleGuide.eChangeType.Colours) = cStyleGuide.eChangeType.Colours) Then
                Me.UpdateColors()
            End If
        End Sub

        ''' <summary>
        ''' Event hander to display results for another group
        ''' </summary>
        Private Sub OnGroupSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_lbGroups.SelectedIndexChanged
            Try
                Me.AddCurves()
                Me.m_zgh.RescaleAndRedraw()
            Catch
            End Try
        End Sub

        ''' <summary>
        ''' Event hander to display results for another Parameter
        ''' </summary>
        Private Sub OnParameterSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_lbParameter.SelectedIndexChanged

            Me.AddCurves()
            Me.m_zgh.RescaleAndRedraw()

        End Sub

#End Region ' Event handlers

#Region " Helper methods "

        Public Overrides Property Settings() As String
            Get
                Dim sbSettings As New StringBuilder()
                Dim iNumPlots As Integer = [Enum].GetValues(GetType(ePlot)).Length
                For iPlot As Integer = 0 To iNumPlots - 1
                    sbSettings.Append(IIF(Me.m_abPlotVisible(DirectCast(iPlot, ePlot)), "1", "0"))
                Next
                Return sbSettings.ToString()
            End Get
            Set(ByVal strSettings As String)
                If String.IsNullOrEmpty(strSettings) Then Return

                Dim iNumPlots As Integer = Math.Min([Enum].GetValues(GetType(ePlot)).Length, strSettings.Length)
                For iPlot As Integer = 0 To iNumPlots - 1
                    Me.m_abPlotVisible(DirectCast(iPlot, ePlot)) = (strSettings.Substring(iPlot, 1) = "1"c)
                Next
            End Set
        End Property



        Protected Sub ConfigurePlots(Optional ByVal bFormOpen As Boolean = True)

            Dim iPane As Integer = 1
            Dim iMaxPanes As Integer = [Enum].GetValues(GetType(ePlot)).Length - 1

            ' Determine where panes will be placed
            For Each plot As ePlot In [Enum].GetValues(GetType(ePlot))
                If Me.m_abPlotVisible(plot) Then
                    Me.m_aiPlotPane(plot) = iPane
                    iPane += 1
                Else
                    Me.m_aiPlotPane(plot) = cCore.NULL_VALUE
                End If
            Next plot

            If Me.m_zgh.IsAttached Then
                Me.m_zgh.Detach()
            End If

            If Not bFormOpen Then Return

            If Not bFormOpen Then Return
            Try
                Me.m_zgh.Attach(Me.UIContext, Me.m_graph, iPane - 1)
            Catch
            End Try
            
            Dim lbGroupsCollection As ListBox.SelectedObjectCollection = Me.m_lbGroups.SelectedItems

            'More than one group selected..?
            If lbGroupsCollection.Count > 1 Then
                Dim count As Integer = 0
                For Each data As ePlot In [Enum].GetValues(GetType(ePlot))
                    If count <= lbGroupsCollection.Count - 1 Then
                        Dim selGroupIndex1 As Integer = Me.m_lbGroups.SelectedIndices.Item(count) + 1
                        Dim group11 As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(selGroupIndex1)
                        Dim strTitle As String = group11.Name
                        Dim strYAaxisLabel As String = Me.GetPlotYAxisLabel(data)
                        Dim dAxisMax As Double = 0
                        Me.ConfigurePane(data, strTitle, strYAaxisLabel, dAxisMax)
                        count += 1
                    End If
                Next
            End If

            'only one group selected..?
            If lbGroupsCollection.Count <= 1 Then
                For Each data As ePlot In [Enum].GetValues(GetType(ePlot))
                    Dim selGroupIndex1 As Integer = Me.m_lbGroups.SelectedIndex + 1
                    Dim selGrpInd As Integer = Math.Max(1, selGroupIndex1)
                    Dim group11 As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(selGrpInd)
                    Dim strTitle As String = group11.Name
                    Dim strYAaxisLabel As String = Me.GetPlotYAxisLabel(data)
                    Dim dAxisMax As Double = 0
                    Me.ConfigurePane(data, strTitle, strYAaxisLabel, dAxisMax)
                Next
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a plot on the main graph
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub ConfigurePane(ByVal plot As ePlot, ByVal strTitle As String, ByVal strYAxisLabel As String, Optional ByVal dYAxisMax As Double = 0)

            If Not Me.m_abPlotVisible(plot) Then Return
            ' Sanity check
            Debug.Assert(Me.m_aiPlotPane(plot) > 0)
            ' Configure pane
            Me.m_zgh.ConfigurePane(strTitle, _
                       SharedResources.HEADER_TIME, _
                       CDbl(Me.UIContext.Core.EcosimFirstYear), _
                       CDbl(Me.UIContext.Core.EcosimFirstYear + (Me.UIContext.Core.nEcosimTimeSteps / cCore.N_MONTHS)), _
                       strYAxisLabel, 0, dYAxisMax, _
                       False, LegendPos.Top, Me.m_aiPlotPane(plot))

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

            Dim selIndexCount As Integer = Me.m_lbGroups.SelectedIndices.Count

            'Did User select more than one group?
            If selIndexCount > 1 Then
                ' Clear all panes
                For Each pane As GraphPane In Me.m_graph.MasterPane.PaneList
                    pane.CurveList.Clear()
                    pane.AxisChange()
                Next
                'Set the master pane title
                Me.m_zgh.Configure(Me.m_lbParameter.SelectedItem.ToString)
                ' Do not render when sim has not ran
                If Not Me.UIContext.Core.StateMonitor.HasEcosimRan Then Return

                Dim pplBlist1 As New List(Of Object)
                Dim pplConsBlist1 As New List(Of Object)
                Dim pplFeedTimelist1 As New List(Of Object)
                Dim pplAvgWorProdConslist1 As New List(Of Object)
                Dim group1list As New List(Of cEcoPathGroupInput)
                Dim iGrouplist As New List(Of Integer)

                For iii As Integer = 0 To selIndexCount - 1
                    Dim dXValue1 As Double = 0
                    Dim iGroup1 As Integer = Me.m_lbGroups.SelectedIndices.Item(iii) + 1
                    Dim groupSimOut1 As cEcosimGroupOutput = Me.UIContext.Core.EcoSimGroupOutputs(iGroup1)
                    Dim group1 As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(iGroup1)
                    Dim pplB1 As New PointPairList()
                    Dim pplConsB1 As New PointPairList()
                    Dim pplFeedTime1 As New PointPairList()
                    Dim pplAvgWorProdCons1 As New PointPairList()

                    For i As Integer = 1 To Me.UIContext.Core.nEcosimTimeSteps
                        dXValue1 = Me.UIContext.Core.EcosimFirstYear + (i / cCore.N_MONTHS)
                        pplB1.Add(dXValue1, groupSimOut1.Biomass(i))
                        pplConsB1.Add(dXValue1, groupSimOut1.ConsumpBiomass(i))
                        pplFeedTime1.Add(dXValue1, groupSimOut1.FeedingTime(i))
                        ' Special case: is mutli-stanza?
                        If groupSimOut1.isMultiStanza() Then
                            pplAvgWorProdCons1.Add(dXValue1, groupSimOut1.AvgWeight(i))
                        Else
                            pplAvgWorProdCons1.Add(dXValue1, groupSimOut1.ProdConsump(i))
                        End If
                    Next i
                    pplBlist1.Add(pplB1)
                    pplConsBlist1.Add(pplConsB1)
                    pplFeedTimelist1.Add(pplFeedTime1)
                    pplAvgWorProdConslist1.Add(pplAvgWorProdCons1)
                    group1list.Add(group1)
                    iGrouplist.Add(iGroup1)
                Next iii

                'Confuguring Plots
                For Each plot As ePlot In [Enum].GetValues(GetType(ePlot))
                    Me.m_abPlotVisible(plot) = False
                Next
                Dim plotCnt As Integer = 0
                For Each plot As ePlot In [Enum].GetValues(GetType(ePlot))
                    If plotCnt <= selIndexCount - 1 Then
                        Me.m_abPlotVisible(plot) = True
                        plotCnt += 1
                    End If
                Next
                Me.ConfigurePlots()
                Try
                    'Plots for Biomass
                    If Me.m_lbParameter.SelectedIndex = 0 Then
                        Dim icount1 As Integer = 0
                        For Each plot32 As ePlot In [Enum].GetValues(GetType(ePlot))
                            Me.AddCurveToGraphPane(plot32, Me.m_zgh.CreateLineItem(group1list.Item(icount1), CType(pplBlist1.Item(icount1), PointPairList)))
                            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassRel, iGrouplist.Item(icount1), Color.Blue)
                                Me.AddCurveToGraphPane(plot32, li)
                            Next li
                            ' Fixes issue 604:
                            For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassAbs, iGrouplist.Item(icount1), Color.Green)
                                Me.AddCurveToGraphPane(plot32, li)
                            Next li
                            icount1 += 1
                        Next plot32
                    End If

                    'Plots for Consumption/Biomass
                    If Me.m_lbParameter.SelectedIndex = 1 Then
                        Dim icount2 As Integer = 0
                        For Each plot32 As ePlot In [Enum].GetValues(GetType(ePlot))
                            Me.AddCurveToGraphPane(plot32, Me.m_zgh.CreateLineItem(group1list.Item(icount2), CType(pplConsBlist1.Item(icount2), PointPairList)))
                            icount2 += 1
                        Next plot32
                    End If

                    'Plots for FeedingTime
                    If Me.m_lbParameter.SelectedIndex = 2 Then
                        Dim icount3 As Integer = 0
                        For Each plot32 As ePlot In [Enum].GetValues(GetType(ePlot))
                            Me.AddCurveToGraphPane(plot32, Me.m_zgh.CreateLineItem(group1list.Item(icount3), CType(pplFeedTimelist1.Item(icount3), PointPairList)))
                            icount3 += 1
                        Next plot32
                    End If

                    'Plots for Production/consumption
                    If Me.m_lbParameter.SelectedIndex = 3 Then
                        Dim icount4 As Integer = 0
                        For Each plot32 As ePlot In [Enum].GetValues(GetType(ePlot))
                            Me.AddCurveToGraphPane(plot32, Me.m_zgh.CreateLineItem(group1list.Item(icount4), CType(pplAvgWorProdConslist1.Item(icount4), PointPairList)))
                            icount4 += 1
                        Next plot32
                    End If
                Catch
                End Try

            End If

            'Did user select only one group? or is default group selected..?
            If selIndexCount = 1 Then
                ' Clear all panes
                For Each pane As GraphPane In Me.m_graph.MasterPane.PaneList
                    pane.CurveList.Clear()
                    pane.AxisChange()
                Next
                ' Do not render when sim has not ran
                If Not Me.UIContext.Core.StateMonitor.HasEcosimRan Then Return
                'Set the master pane title
                Me.m_zgh.Configure(Me.m_lbParameter.SelectedItem.ToString)

                Dim dXValue As Double = 0
                Dim iGroup As Integer = Math.Max(1, Me.m_lbGroups.SelectedGroupIndex)
                Dim groupSimOut As cEcosimGroupOutput = Me.UIContext.Core.EcoSimGroupOutputs(iGroup)
                Dim group As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(iGroup)

                Dim pplB As New PointPairList()
                Dim pplConsB As New PointPairList()
                Dim pplFeedTime As New PointPairList()
                Dim pplAvgWorProdCons As New PointPairList()

                For i As Integer = 1 To Me.UIContext.Core.nEcosimTimeSteps
                    ' Time
                    dXValue = Me.UIContext.Core.EcosimFirstYear + (i / cCore.N_MONTHS)
                    ' Get sim results
                    pplB.Add(dXValue, groupSimOut.Biomass(i))
                    pplConsB.Add(dXValue, groupSimOut.ConsumpBiomass(i))
                    pplFeedTime.Add(dXValue, groupSimOut.FeedingTime(i))
                    ' Special case: is mutli-stanza?
                    If groupSimOut.isMultiStanza() Then
                        pplAvgWorProdCons.Add(dXValue, groupSimOut.AvgWeight(i))
                    Else
                        pplAvgWorProdCons.Add(dXValue, groupSimOut.ProdConsump(i))
                    End If
                Next i

                'Configuring Plots
                For Each plot As ePlot In [Enum].GetValues(GetType(ePlot))
                    Me.m_abPlotVisible(plot) = False
                Next
                Me.m_abPlotVisible(ePlot.plot1) = True
                Me.ConfigurePlots()

                'Plots for Biomass
                If Me.m_lbParameter.SelectedIndex = 0 Then
                    Me.AddCurveToGraphPane(ePlot.plot1, Me.m_zgh.CreateLineItem(group, pplB))
                    For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassRel, iGroup, Color.Blue)
                        Me.AddCurveToGraphPane(ePlot.plot1, li)
                    Next li
                    ' Fixes issue 604:
                    For Each li As LineItem In Me.GetTimeSeriesLineItems(eTimeSeriesType.BiomassAbs, iGroup, Color.Green)
                        Me.AddCurveToGraphPane(ePlot.plot1, li)
                    Next li
                End If

                'Plots for Consumption/Biomass
                If Me.m_lbParameter.SelectedIndex = 1 Then
                    Me.AddCurveToGraphPane(ePlot.plot1, Me.m_zgh.CreateLineItem(group, pplConsB))
                End If

                'Plots for Feeding Time
                If Me.m_lbParameter.SelectedIndex = 2 Then
                    Me.AddCurveToGraphPane(ePlot.plot1, Me.m_zgh.CreateLineItem(group, pplFeedTime))
                End If

                'Plots for Production/Consumption
                If Me.m_lbParameter.SelectedIndex = 3 Then
                    Me.AddCurveToGraphPane(ePlot.plot1, Me.m_zgh.CreateLineItem(group, pplAvgWorProdCons))
                End If

            End If

        End Sub

#Region " Time series "

        Private Function GetTimeSeriesLineItems(ByVal TSType As eTimeSeriesType, ByVal iGroup As Integer, ByVal clr As Color) As List(Of LineItem)

            Dim lli As New List(Of LineItem)
            Dim ppt As PointPairList = Nothing
            Dim ts As cTimeSeries = Nothing
            Dim gts As cGroupTimeSeries = Nothing
            Dim iNumLine As Integer = 0

            For i As Integer = 1 To Me.UIContext.Core.nTimeSeries
                ts = Me.UIContext.Core.EcosimTimeSeries(i)
                If ts.TimeSeriesType = TSType Then
                    If TypeOf ts Is cGroupTimeSeries Then
                        gts = DirectCast(ts, cGroupTimeSeries)
                        If (gts.GroupIndex = iGroup) And gts.Enabled() Then
                            lli.Add(Me.ToTimeSeriesLineItem(gts, cColorUtils.GetVariant(clr, iNumLine)))
                            iNumLine += 1
                        End If
                    End If
                End If
            Next

            Return lli

        End Function

        Private Function ToTimeSeriesLineItem(ByVal gts As cGroupTimeSeries, ByVal clr As Color) As LineItem

            Dim ppt As New PointPairList
            Dim dScale As Single = 1.0F
            Dim li As LineItem = Nothing
            Dim xpos As Double = 0.0
            Dim deltaT As Double = 1 / cCore.N_MONTHS
            Dim da() As Single = gts.ShapeData()
            Dim iYear As Integer = Me.UIContext.Core.EcosimFirstYear

            If (gts.TimeSeriesType = eTimeSeriesType.BiomassRel) Or (gts.TimeSeriesType = eTimeSeriesType.AverageWeight) Then
                'VC091209: totalmortality is absolute, not relative
                If gts.eDataQ > 0 Then dScale = 1.0F / gts.eDataQ
            End If

            'Just in case...
            Debug.Assert(Me.m_TSInterval = eTSDataSetInterval.Annual Or Me.m_TSInterval = eTSDataSetInterval.Monthly, "Plotting Ecosim Output unknown timeseries interval.")

            For j As Integer = 1 To da.Length - 1
                If (da(j) > 0) Then
                    Select Case Me.m_TSInterval
                        Case eTSDataSetInterval.Monthly
                            xpos = iYear + j * deltaT - deltaT * 0.5
                        Case eTSDataSetInterval.Annual
                            xpos = iYear + j - 0.5
                    End Select
                    ppt.Add(xpos, da(j) * dScale)
                End If
            Next
            Return Me.m_zgh.CreateLineItem(gts.Name, eLineType.ReferenceData, clr, ppt)

        End Function

#End Region ' Time series



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
            For i As Integer = 0 To aiGroupIndex.Count - 1
                l.SortValue(aiGroupIndex(i)) = asValues(i)
            Next
            l.SortType = cGroupListBox.eSortType.ValueDesc
            l.Refresh()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate Parameter list box with parameters.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub PopulateParameterListBox()
            Me.m_lbParameter.BeginUpdate()
            Me.m_lbParameter.Items.Add("Biomass")
            Me.m_lbParameter.Items.Add("Consumption/Biomass")
            Me.m_lbParameter.Items.Add("Feeding time")
            Me.m_lbParameter.Items.Add("Production/consumption")
            Me.m_lbParameter.EndUpdate()
        End Sub


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add one curve into the graph pane
        ''' </summary>
        ''' <param name="paneType">Index of the graph pane</param>
        ''' <param name="li">The curve</param>
        ''' -------------------------------------------------------------------
        Private Sub AddCurveToGraphPane(ByVal paneType As ePlot, _
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
        Private Sub AddCurvesToGraphPane(ByVal paneType As ePlot, _
                                         ByVal lli As List(Of LineItem), _
                                         Optional ByVal bCumulative As Boolean = False)

            If Not Me.m_abPlotVisible(paneType) Then Return
            ' Sanity check
            Debug.Assert(Me.m_aiPlotPane(paneType) > 0)
            Try
                Me.m_zgh.PlotLines(lli.ToArray, Me.m_aiPlotPane(paneType), True, False, bCumulative)
            Catch ex As Exception

            End Try

        End Sub

        Private Sub UpdateColors()
            m_paneMaster.Fill = New Fill(Me.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            For Each p As GraphPane In Me.m_paneMaster.PaneList
                p.Chart.Fill = New Fill(Me.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            Next
        End Sub


        Private Function GetPlotYAxisLabel(ByVal data As ePlot) As String
            'Biomass
            If Me.m_lbParameter.SelectedIndex = 0 Then
                Return String.Format(SharedResources.GENERIC_LABEL_UNIT, StyleGuide.GetUnitString(cStyleGuide.eUnitType.Currency))
            End If
            'Consumption/Biomass
            If Me.m_lbParameter.SelectedIndex = 1 Then
                Return String.Format(SharedResources.GENERIC_LABEL_PERUNIT, StyleGuide.GetUnitString(cStyleGuide.eUnitType.Time))
            End If

            Return ""
        End Function


#End Region ' Helper methods

    End Class

End Namespace