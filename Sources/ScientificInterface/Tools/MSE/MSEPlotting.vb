
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim

Imports ZedGraph

#End Region

#Region "Enumerators"

Friend Enum ePlotTypes
    Histogram
    Values
    Line
End Enum

#End Region

#Region "Plotting Class"

#Region "Reference point class"

Friend Class cMSERefPoint
    Private m_low As Single
    Private m_upper As Single
    Public Sub New(ByVal LowerRef As Single, ByVal UpperRef As Single)
        Me.m_low = LowerRef
        Me.m_upper = UpperRef
    End Sub

    Public Property LowerReference() As Single
        Get
            Return Me.m_low
        End Get
        Set(ByVal value As Single)
            Me.m_low = value
        End Set
    End Property

    Public Property UpperReference() As Single
        Get
            Return Me.m_upper
        End Get
        Set(ByVal value As Single)
            Me.m_upper = value
        End Set
    End Property

End Class

#End Region

#Region "Plotter class"

Friend Class cMSEPlotter

#Region "Private data"

    Private Const LB_TAG As String = "LB"
    Private Const UB_TAG As String = "UB"

    Private m_uic As cUIContext = Nothing
    Private m_zgh As cZedGraphHelper = Nothing
    Private m_zdGraph As ZedGraphControl
    Private m_manager As cMSEManager
    Private m_nvis As Integer
    Private m_type As ePlotTypes
    Private m_dataType As ePlotData
    Private m_Data As List(Of cCoreGroupBase)
    Private m_RefPoints As List(Of cMSERefPoint)
    Private m_nLines As Integer

#End Region

#Region "Public interface"

    ''' <summary>
    ''' Initialize to ZedGraphHelper and a Zedgraph control
    ''' </summary>
    ''' <param name="uic"></param>
    ''' <param name="ZedGraphHelper"></param>
    ''' <param name="ZedGraph"></param>
    ''' <remarks></remarks>
    Public Sub Init(ByVal uic As cUIContext, _
                    ByVal MSEManager As cMSEManager, _
                    ByVal ZedGraphHelper As cZedGraphHelper, _
                    ByVal ZedGraph As ZedGraphControl)
        Me.m_zgh = ZedGraphHelper
        Me.m_uic = uic
        Me.m_zdGraph = ZedGraph
        Me.m_manager = MSEManager
    End Sub


    ''' <summary>
    ''' How the current data is to be plotted
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PlotType() As ePlotTypes
        Get
            Return PlotType
        End Get
        Set(ByVal value As ePlotTypes)
            m_type = value
        End Set
    End Property

    ''' <summary>
    ''' What type of data is being plotted. Used mostly for labels
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataType() As ePlotData
        Get
            Return Me.m_dataType
        End Get
        Set(ByVal value As ePlotData)
            m_dataType = value
        End Set
    End Property


    Public Sub Clear()
        Try
            Dim npanes As Integer = Me.nVisGroups
            If Me.m_dataType = ePlotData.Effort Or Me.m_dataType = ePlotData.FleetValue Then
                npanes = Me.nVisFleets
            End If
            Me.m_zgh.Attach(Me.m_uic, Me.m_zdGraph, npanes)

            Me.ClearData()
            Me.ClearGraphs()
            Me.configPanes()
            Me.m_nLines = 0
            '  Me.configPanes()
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Draw() Exception: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Plot the current data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Draw()

        Try

            If Me.m_Data IsNot Nothing Then

                If Me.m_type <> ePlotTypes.Line Then
                    Me.m_zgh.Attach(Me.m_uic, Me.m_zdGraph, Me.m_Data.Count)
                    Me.ClearGraphs()
                    Me.configPanes()
                End If

                Me.plotRefLines()

                If m_type = ePlotTypes.Histogram Then
                    Me.plotHistoGram()

                ElseIf Me.m_type = ePlotTypes.Values Then
                    'mass lines
                    Me.plotValues()

                ElseIf Me.m_type = ePlotTypes.Line Then
                    'single line
                    Me.plotline()

                End If

            End If

            ' Me.plotRefLines()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Draw() Exception: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Added data to be plotted
    ''' </summary>
    ''' <param name="ListOfData"></param>
    ''' <remarks></remarks>
    Public Sub AddData(ByVal ListOfData As List(Of cCoreGroupBase))

        If Me.m_Data IsNot Nothing Then
            Me.m_Data.Clear()
        End If
        Me.m_Data = ListOfData

        'if we are adding one line at a time
        'and this is the first line then configure the panes
        If Me.m_type = ePlotTypes.Line And Me.m_nLines = 0 Then
            Me.configValuePanes()
        End If

        Me.m_nLines += 1

    End Sub

    ''' <summary>
    ''' Added Mean data to be plotted
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AddMean()

        Dim stats As cMSEStats
        Dim ipane As Integer

        'Only data added one line at a time should be plotted this way
        If Me.m_type <> ePlotTypes.Line Then Exit Sub

        For Each data As cCoreGroupBase In Me.m_Data
            ipane += 1
            stats = Me.m_manager.BiomassStats(data.Index)
            Me.plotMean(stats, Me.m_zgh.GetPane(ipane))
        Next

        '  Me.m_zgh.RescaleAndRedraw()

    End Sub

    ''' <summary>
    ''' Added the Reference lines to the plots
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AddReference()

        plotRefLines()

    End Sub

    Private Sub plotRefLines()
        Dim ipane As Integer

        'Reference lines are retrieved from MSEManager based on the type of data that is being plotted
        'this should mean the reference lines are always in sync with the current data
        For Each statobj As cCoreGroupBase In Me.m_Data
            ipane += 1
            'get the reference data from the core for this datatype
            Dim RefPoint As cMSERefPoint = Me.getRefPoint(statobj.Index)
            'Data the data to the graph pane, this will remove existing reference lines
            Me.plotRefLine(RefPoint.LowerReference, RefPoint.UpperReference, Me.m_zgh.GetPane(ipane))

            'Do NOT rescale if this is a Histogram
            If Me.m_type <> ePlotTypes.Histogram Then
                Me.m_zgh.AutoscalePane(ipane) = True
            End If

        Next

        Me.m_zgh.Redraw()

    End Sub


    Private Function getRefPoint(ByVal ItemIndex As Integer) As cMSERefPoint

        Dim refPoint As cMSERefPoint = Nothing

        Select Case Me.m_dataType

            Case ePlotData.Biomass
                Dim grp As cMSEGroupInput = Me.m_manager.GroupInputs(ItemIndex)
                refPoint = New cMSERefPoint(grp.BiomassRefLower, grp.BiomassRefUpper)
            Case ePlotData.GroupCatch
                Dim grp As cMSEGroupInput = Me.m_manager.GroupInputs(ItemIndex)
                refPoint = New cMSERefPoint(grp.CatchRefLower, grp.CatchRefUpper)
            Case ePlotData.FleetValue
                Dim flt As cMSEFleetInput = Me.m_manager.FleetInputs(ItemIndex)
                refPoint = New cMSERefPoint(flt.CatchRefLower, flt.CatchRefUpper)
            Case ePlotData.Effort
                Dim flt As cMSEFleetInput = Me.m_manager.FleetInputs(ItemIndex)
                refPoint = New cMSERefPoint(flt.EffortRefLower, flt.EffortRefUpper)

        End Select

        Return refPoint

    End Function

    Private Sub plotRefLines_old()
        Dim ipane As Integer

        'this should not be called if m_RefPoints is null
        ' Debug.Assert(Me.m_RefPoints IsNot Nothing, Me.ToString & ".plotRefLines() no reference lines have been added!")
        If Me.m_RefPoints Is Nothing Then
            Exit Sub 'just in case
        End If

        For Each Ref As cMSERefPoint In m_RefPoints
            ipane += 1
            Me.plotRefLine(Ref.LowerReference, Ref.UpperReference, Me.m_zgh.GetPane(ipane))
            If Me.m_type = ePlotTypes.Histogram Then
                'do not rescale a histogram
                Me.m_zgh.Redraw()
            Else
                Me.m_zgh.AutoscalePane(ipane) = True
            End If

        Next

    End Sub

#End Region

#Region "Private methods"

    Private Sub configPanes()
        If Me.m_type = ePlotTypes.Histogram Then
            configHistoPanes()
        ElseIf Me.m_type = ePlotTypes.Values Or Me.m_type = ePlotTypes.Line Then
            configValuePanes()
        End If
    End Sub

    Private Sub configHistoPanes()

        Dim ipane As Integer
        For Each data As cMSEStats In m_Data
            ipane += 1
            Me.m_zgh.ConfigurePane(data.Name, Me.XLabel, data.Min, data.Max, Me.YLabel, 0, 0, False, LegendPos.Top, ipane)
        Next

    End Sub

    Private Sub configValuePanes()

        Dim ipane As Integer

        Select Case Me.m_dataType

            Case ePlotData.Biomass, ePlotData.GroupCatch
                'By group
                For Each grp As cCoreGroupBase In Me.m_manager.GroupInputs
                    If Me.m_uic.StyleGuide.GroupVisible(grp.Index) Then
                        ipane += 1
                        Me.m_zgh.ConfigurePane(grp.Name, _
                                               Me.XLabel, _
                                               CDbl(Me.m_uic.Core.EcosimFirstYear), _
                                               CDbl(Me.m_uic.Core.EcosimFirstYear + (Me.m_uic.Core.nEcosimTimeSteps / cCore.N_MONTHS)), _
                                               Me.YLabel, 0, 0, False, LegendPos.Top, ipane)
                        Me.m_zgh.AutoscalePane(ipane) = True
                    End If
                Next

            Case ePlotData.Effort, ePlotData.FleetValue
                'By Fleet
                Dim flt As cFleetInput
                For iflt As Integer = 1 To Me.m_uic.Core.nFleets
                    flt = Me.m_uic.Core.FleetInputs(iflt)
                    If Me.m_uic.StyleGuide.FleetVisible(flt.Index) Then
                        ipane += 1
                        Me.m_zgh.ConfigurePane(flt.Name, _
                                               Me.XLabel, _
                                               CDbl(Me.m_uic.Core.EcosimFirstYear), _
                                               CDbl(Me.m_uic.Core.EcosimFirstYear + (Me.m_uic.Core.nEcosimTimeSteps / cCore.N_MONTHS)), _
                                               Me.YLabel, 0, 0, _
                                               False, LegendPos.Top, ipane)
                        Me.m_zgh.AutoscalePane(ipane) = True
                    End If
                Next

        End Select

    End Sub

    Private ReadOnly Property YLabel() As String
        Get

            Select Case Me.m_type

                Case ePlotTypes.Histogram

                    Return "Probability"

                Case ePlotTypes.Line, ePlotTypes.Values

                    Select Case Me.m_dataType
                        Case ePlotData.Biomass
                            Return "Biomass k/km2"
                        Case ePlotData.Effort
                            Return "Effort"
                        Case ePlotData.FleetValue
                            Return "Value of catch"
                        Case ePlotData.GroupCatch
                            Return "Biomass of catch"
                    End Select

            End Select


            Return ""

        End Get
    End Property



    Private ReadOnly Property XLabel() As String
        Get

            Select Case Me.m_type

                Case ePlotTypes.Histogram

                    Select Case Me.m_dataType
                        Case ePlotData.Biomass
                            Return "Biomass k/km2"
                        Case ePlotData.Effort
                            Return "Effort"
                        Case ePlotData.FleetValue
                            Return "Value of catch"
                        Case ePlotData.GroupCatch
                            Return "Biomass of catch"
                    End Select

                Case ePlotTypes.Line, ePlotTypes.Values

                    Return "Year"

            End Select

            Return ""
        End Get
    End Property

    Private Sub ClearData()

        If Me.m_Data IsNot Nothing Then
            Me.m_Data.Clear()
        End If

        If Me.m_RefPoints IsNot Nothing Then
            Me.m_RefPoints.Clear()
        End If

    End Sub
    Private Sub ClearGraphs()

        For Each Pane As GraphPane In Me.m_zgh.Graph.MasterPane.PaneList
            Pane.CurveList.Clear()
        Next

    End Sub

    Private Sub plotHistoGram()

        Try

            Dim ipane As Integer
            Dim dx As Double
            Dim binWidth As Single
            Dim min As Single

            For Each data As cMSEStats In Me.m_Data '
                ipane += 1
                binWidth = data.BinWidths
                min = data.Min
                Dim max As Single = Single.MinValue
                Dim ppl As New PointPairList
                For ibin As Integer = 1 To data.nBins
                    dx = min + binWidth * (ibin - 1)
                    ppl.Add(dx, data.Histogram(ibin))

                    dx = min + binWidth * ibin
                    ppl.Add(dx, data.Histogram(ibin))

                    If ibin <> data.nBins Then
                        'draw the start of the next bin/column if this is not the last bin
                        ppl.Add(dx, data.Histogram(ibin + 1))
                    End If

                    max = Math.Max(max, data.Histogram(ibin))
                Next

                Dim pane As ZedGraph.GraphPane = Me.m_zgh.GetPane(ipane)
                Dim li As LineItem = pane.AddCurve(data.Name, ppl, System.Drawing.Color.Black, SymbolType.None)
                li.Line.Fill = New Fill(System.Drawing.Color.Gray)
                li.IsOverrideOrdinal = True

                pane.XAxis.Type = AxisType.Linear
                pane.YAxis.Scale.Max = max * 1.2

            Next

            Me.m_zgh.RescaleAndRedraw()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".AddLineToGraph() Error: " & ex.Message)
        End Try

    End Sub


    Private Sub plotValues()

        Try

            Dim ipane As Integer
            Dim dx As Double
            Dim values() As Single
            Dim lstLines As New List(Of ZedGraph.LineItem)

            For Each data As cMSEStats In Me.m_Data '
                ipane += 1
                lstLines.Clear()

                For iter As Integer = 1 To data.nIterations
                    Dim ppl As New PointPairList
                    'cMSEStats.Values(iteration) is zero based!!!
                    values = data.Values(iter)

                    For iTime As Integer = 0 To Me.m_uic.Core.nEcosimTimeSteps - 1
                        dx = Me.m_uic.Core.EcosimFirstYear + ((iTime + 1) / cCore.N_MONTHS)
                        ppl.Add(dx, values(iTime))
                    Next

                    Dim Line As LineItem = Me.m_zgh.CreateLineItem(data.Name, ScientificInterfaceShared.Controls.cZedGraphHelper.eCurveTypes.EcosimOutput, _
                                                                System.Drawing.Color.Gray, ppl)
                    lstLines.Add(Line)

                Next

                Me.m_zgh.PlotLines(lstLines.ToArray, ipane, False, False)
                Me.plotMean(data, Me.m_zgh.GetPane(ipane))
            Next

            Me.m_zgh.RescaleAndRedraw()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".AddLineToGraph() Error: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Plot one line across all the groups/fleets
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub plotline()

        Try

            Dim ipane As Integer
            Dim dx As Double
            Dim lstLines As New List(Of ZedGraph.LineItem)

            For Each data As cMSEGroupOutput In Me.m_Data '
                ipane += 1
                lstLines.Clear()

                Dim ppl As New PointPairList

                For iTime As Integer = 1 To Me.m_uic.Core.nEcosimTimeSteps
                    dx = Me.m_uic.Core.EcosimFirstYear + (iTime / cCore.N_MONTHS)
                    ppl.Add(dx, data.Biomass(iTime))
                Next

                Dim Line As LineItem = Me.m_zgh.CreateLineItem(data.Name, ScientificInterfaceShared.Controls.cZedGraphHelper.eCurveTypes.EcosimOutput, _
                                                            System.Drawing.Color.Gray, ppl)
                lstLines.Add(Line)

                Me.m_zgh.PlotLines(lstLines.ToArray, ipane, True, False)
            Next

            Me.m_zgh.RescaleAndRedraw()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".AddLineToGraph() Error: " & ex.Message)
        End Try

    End Sub

    Private Sub plotMean(ByVal StatsData As cMSEStats, ByVal pane As ZedGraph.GraphPane)
        Dim dx As Double

        Dim ppl As New PointPairList
        For iTime As Integer = 1 To Me.m_uic.Core.nEcosimTimeSteps
            dx = Me.m_uic.Core.EcosimFirstYear + (iTime / cCore.N_MONTHS)
            ppl.Add(dx, StatsData.Mean(iTime))
        Next

        Dim LineItem As LineItem = New ZedGraph.LineItem("", ppl, Me.getLineColour(StatsData), SymbolType.None, 2)
        Try
            'Dim pane As ZedGraph.GraphPane = Me.m_zgh.GetPane(ipane)
            'draw the mean line on top of the other lines (insert at the zero position)
            pane.CurveList.Insert(0, LineItem)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".AddMeanLineToGraph() Error: " & ex.Message)
        End Try

    End Sub

    Private Sub plotRefLine(ByVal LowerBound As Single, ByVal UpperBound As Single, ByVal pane As ZedGraph.GraphPane)
        'Dim dx As Double

        Me.removeRefLines(pane)

        If Me.m_type = ePlotTypes.Histogram Then
            'Histogram plot

            Dim pplLB As New PointPairList
            Dim pplUB As New PointPairList

            pplLB.Add(LowerBound, 0)
            pplLB.Add(LowerBound, 1)
            pplUB.Add(UpperBound, 0)
            pplUB.Add(UpperBound, 1)
            Dim crv As ZedGraph.CurveItem
            crv = pane.AddCurve("", pplLB, System.Drawing.Color.Pink, SymbolType.None)
            crv.IsOverrideOrdinal = True
            crv.Tag = LB_TAG

            crv = pane.AddCurve("", pplUB, System.Drawing.Color.Pink, SymbolType.None)
            crv.IsOverrideOrdinal = True
            crv.Tag = UB_TAG

        Else
            'Line plot
            Dim pplLB As New PointPairList
            Dim pplUB As New PointPairList

            pplLB.Add(0, LowerBound)
            pplLB.Add(Me.m_uic.Core.nEcosimTimeSteps, LowerBound)

            pplUB.Add(0, UpperBound)
            pplUB.Add(Me.m_uic.Core.nEcosimTimeSteps, UpperBound)

            Dim LBItem As LineItem = New ZedGraph.LineItem("", pplLB, Color.Pink, SymbolType.None, 1)
            Dim UBItem As LineItem = New ZedGraph.LineItem("", pplUB, Color.Pink, SymbolType.None, 1)
            LBItem.Tag = LB_TAG
            UBItem.Tag = UB_TAG
            Try
                pane.CurveList.Insert(0, LBItem)
                pane.CurveList.Insert(0, UBItem)
            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".AddMeanLineToGraph() Error: " & ex.Message)
            End Try

        End If

    End Sub

    Private Sub removeRefLines(ByVal pane As ZedGraph.GraphPane)

        Dim lbIndex As Integer = pane.CurveList.IndexOfTag(LB_TAG)
        If lbIndex > -1 Then
            pane.CurveList.RemoveAt(lbIndex)
        End If

        Dim ubIndex As Integer = pane.CurveList.IndexOfTag(UB_TAG)
        If ubIndex > -1 Then
            pane.CurveList.RemoveAt(ubIndex)
        End If

    End Sub

    Private Function getLineColour(ByVal StatsData As cMSEStats) As Color

        Try
            'if this is group data then get the colour from the style guide
            If StatsData.DataType = eDataTypes.MSECatchByGroupStats Or StatsData.DataType = eDataTypes.MSEBiomassStats Then
                Return Me.m_uic.StyleGuide.GroupColor(Me.m_uic.Core, StatsData.Index)
            End If
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".getLineColour() Exception thrown from the Style Guide. Default colour will be used.")
        End Try

        'Not group data or the style guide through an error so just return a colour
        Return Color.Red

    End Function

    Private Function nVisGroups() As Integer
        Dim n As Integer
        For igrp As Integer = 1 To Me.m_uic.Core.nGroups
            If Me.m_uic.StyleGuide.GroupVisible(igrp) Then
                n += 1
            End If
        Next
        Return n
    End Function

    Private Function nVisFleets() As Integer
        Dim n As Integer
        For igrp As Integer = 1 To Me.m_uic.Core.nFleets
            If Me.m_uic.StyleGuide.FleetVisible(igrp) Then
                n += 1
            End If
        Next
        Return n
    End Function

#End Region

End Class

#End Region


#End Region

