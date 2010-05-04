#Region " Imports "

Option Strict On

Imports System.IO
Imports System.Text
Imports ZedGraph
Imports EwECore
Imports System.Windows.Forms
Imports System.Drawing
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports System.Globalization
Imports System.Threading
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Definitions
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Helper class, wraps a <see cref="ZedGraph">ZedGraph</see> graph control
    ''' to standardize look and feel. Additionally, this class implements 
    ''' generic cursor behaviour on the graph, and provides standardized data 
    ''' export.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class cZedGraphHelper

#Region " Helper classes "

        ''' <summary>
        ''' cZedGraphHelper internal class, manages curve contextual information.
        ''' </summary>
        Protected Class cCurveInfo

            ' == Auto-properties ==

            Private m_source As ICoreInterface = Nothing
            Private m_iGroup As Integer = cCore.NULL_VALUE
            Private m_iFleet As Integer = cCore.NULL_VALUE
            Private m_uic As cUIContext = Nothing

            ' == Fixed properties ==

            Private m_strLabel As String = ""
            Private m_colour As Color = Color.Aqua
            Private m_lineType As eLineType = eLineType.ModelData

            ' == Status flags ==

            Private m_bGrayedOut As Boolean = False
            Private m_bHighlighted As Boolean = False

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Constructor type 1: manage a curve connected to a core data object.
            ''' </summary>
            ''' <param name="src">Core data source to connect to.</param>
            ''' <param name="uic">UI context to use for colours, visibility, etc.</param>
            ''' <param name="strLabel">Label of the curve. If not provided, the
            ''' curve label is obtained from the core data object.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal src As ICoreInterface, _
                           ByVal uic As cUIContext, _
                           Optional ByVal strLabel As String = "")

                ' Sanity checks
                Debug.Assert(src IsNot Nothing)
                Debug.Assert(uic IsNot Nothing)

                Me.m_source = src
                Me.m_uic = uic
                Me.m_strLabel = strLabel

                If (TypeOf src Is cCoreInputOutputBase) Then
                    If (TypeOf src Is cEcoPathGroupInput) Then
                        Me.m_iGroup = src.Index
                        Me.m_lineType = eLineType.ModelData
                    ElseIf (TypeOf src Is cFleetInput) Then
                        Me.m_iFleet = src.Index
                        Me.m_lineType = eLineType.ModelData
                    End If
                Else
                    If (TypeOf src Is cGroupTimeSeries) Then
                        Me.m_iGroup = DirectCast(src, cGroupTimeSeries).GroupIndex
                        Me.m_lineType = eLineType.ReferenceData
                    ElseIf (TypeOf src Is cFleetTimeSeries) Then
                        Me.m_iFleet = DirectCast(src, cFleetTimeSeries).FleetIndex
                        Me.m_lineType = eLineType.ReferenceData
                    End If
                End If

                ' Post-anaysis sanity checks
                Debug.Assert(Me.m_lineType <> eLineType.NotSet)
                Debug.Assert(Me.m_iGroup <> cCore.NULL_VALUE Or Me.m_iFleet <> cCore.NULL_VALUE)

            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Constructor type 2: manage a curve with custom attributes.
            ''' </summary>
            ''' <param name="strLabel">Label of the curve.</param>
            ''' <param name="colour">Colour of the curve.</param>
            ''' <param name="lineType">Data type of the curve that will determine
            ''' the curve display style.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal strLabel As String, _
                           ByVal colour As Color, _
                           ByVal lineType As eLineType)

                Me.m_strLabel = strLabel
                Me.m_colour = colour
                Me.m_lineType = lineType

            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="eLineType">data type</see> of the curve.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property LineType() As eLineType
                Get
                    Return Me.m_lineType
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the label for the curve.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Label() As String
                Get
                    ' Return overriding name, if any
                    If Not String.IsNullOrEmpty(Me.m_strLabel) Then Return Me.m_strLabel
                    ' Deduct from source
                    If Me.m_source IsNot Nothing Then
                        Return String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, _
                                             Me.m_source.Index, Me.m_source.Name)
                    End If
                    ' Hmm...
                    Return ""
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the index for the curve, if any.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Index() As Integer
                Get
                    If Me.m_iGroup <> cCore.NULL_VALUE Then Return Me.m_iGroup
                    If Me.m_iFleet <> cCore.NULL_VALUE Then Return Me.m_iFleet
                    Return cCore.NULL_VALUE
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the colour for the curve.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Colour() As Color
                Get
                    If Me.m_bGrayedOut Then Return Color.LightGray

                    If Me.m_iGroup <> cCore.NULL_VALUE Then Return Me.m_uic.StyleGuide.GroupColor(Me.m_uic.Core, Me.m_iGroup)
                    If Me.m_iFleet <> cCore.NULL_VALUE Then Return Me.m_uic.StyleGuide.FleetColor(Me.m_uic.Core, Me.m_iFleet)
                    Return Me.m_colour
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the visibility state for the curve.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property IsVisible() As Boolean
                Get
                    If Me.m_iGroup <> cCore.NULL_VALUE Then Return Me.m_uic.StyleGuide.GroupVisible(Me.m_iGroup)
                    If Me.m_iFleet <> cCore.NULL_VALUE Then Return Me.m_uic.StyleGuide.FleetVisible(Me.m_iFleet)
                    Return True
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set grayed-out state of a curve.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property IsGrayedOut() As Boolean
                Get
                    Return Me.m_bGrayedOut
                End Get
                Set(ByVal value As Boolean)
                    Me.m_bGrayedOut = value
                End Set
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set highlight state of a curve.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property IsHighlighted() As Boolean
                Get
                    Return Me.m_bHighlighted
                End Get
                Set(ByVal value As Boolean)
                    Me.m_bHighlighted = value
                End Set
            End Property

        End Class

#End Region ' Helper classes

#Region " Private vars "

        Private m_uic As cUIContext = Nothing

        ''' <summary>Wrapped ZedGraph control.</summary>
        Private m_zgc As ZedGraphControl = Nothing
        ''' <summary>Number of panels wanted in the zed graph</summary>
        Private m_nPanels As Integer = 1
        ''' <summary>Registered axis that need to display units.</summary>
        Private m_dtAxisLabels As New Dictionary(Of Axis, cAxisLabelFormatter)
        ''' <summary>Grace value for manual scaled Y-axis.</summary>
        Private m_sYScaleGrace As Single = 1.2!

        ' == Legend ==
        ''' <summary>States whether this instance should show a legend if left to 'default'</summary>
        Private m_bShowLegend As Boolean = True

        ' == Cursor ==
        Private m_bShowCursor() As Boolean
        Private m_sCursorPos() As Single
        Private m_liCursor() As LineItem

        ' == Cumulative ==
        Private m_bCumulative() As Boolean

        ' == Visibility tracking ==

        ''' <summary>Flag stating whether styleguide item visibility should be tracked.</summary>
        Private m_bTrackVisibility As Boolean = True

        ''' <summary>To set the max and min auto options.</summary>
        Public Enum eScaleOptionTypes
            MaxOnly
            MinOnly
            Both
            None
        End Enum

        ' == Hover menu ==

        ''' <summary>States whether a floating hover menu should be displayed on the graph.</summary>
        Private m_bShowHoverMenu As Boolean = True
        ''' <summary>The hover menu to display on top of graph areas.</summary>
        Private m_hovermenu As ucZedGraphHoverMenu = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
        End Sub

        Protected Overrides Sub Finalize()
            Me.Detach()
            MyBase.Finalize()
        End Sub

#End Region ' Construction / destruction

#Region " Selection "

        Public Event OnCurveClicked(ByVal curve As CurveItem, ByVal iPoint As Integer)

#End Region ' Selection

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attach a zedgraph helper to a zedgraph control.
        ''' </summary>
        ''' <param name="uic">cUIContext providing UI contextual information.</param>
        ''' <param name="zgc">ZedGraph control to control.</param>
        ''' <param name="iNumPanels">Number of panels to create.</param>
        ''' <remarks>
        ''' Make sure to cleanup using <see cref="Detach">Detach</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Attach(ByVal uic As cUIContext, _
                                      ByVal zgc As ZedGraphControl, _
                                      Optional ByVal iNumPanels As Integer = 1)

            ' Sanity checks
            Debug.Assert(uic IsNot Nothing)
            Debug.Assert(zgc IsNot Nothing)

            If Me.m_zgc IsNot Nothing Then Me.Detach()

            Me.m_uic = uic
            Me.m_zgc = zgc
            Me.m_nPanels = iNumPanels

            Me.m_hovermenu = New ucZedGraphHoverMenu(New ucZedGraphHoverMenu.OnCommandDelegate(AddressOf OnHoverMenuCommandCallback))

            While Me.m_zgc.MasterPane.PaneList.Count < iNumPanels
                Me.m_zgc.MasterPane.PaneList.Add(New GraphPane())
            End While

            While Me.m_zgc.MasterPane.PaneList.Count > iNumPanels
                Me.m_zgc.MasterPane.PaneList.RemoveAt(iNumPanels)
            End While

            ReDim Me.m_bShowCursor(iNumPanels)
            ReDim Me.m_liCursor(iNumPanels)
            ReDim Me.m_sCursorPos(iNumPanels)
            ReDim Me.m_bCumulative(iNumPanels)

            AddHandler Me.m_zgc.MouseDownEvent, AddressOf OnMouseDownEvent
            AddHandler Me.m_zgc.MouseMoveEvent, AddressOf OnMouseMoveEvent
            AddHandler Me.m_zgc.MouseUpEvent, AddressOf OnMouseUpEvent
            AddHandler Me.m_zgc.MouseEnter, AddressOf OnMouseEnter
            AddHandler Me.m_zgc.MouseLeave, AddressOf OnMouseLeave
            AddHandler Me.m_zgc.ContextMenuBuilder, AddressOf OnBuildContextMenu
            AddHandler Me.m_zgc.PointValueEvent, AddressOf OnPointValueEvent

            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            Me.AllowZoom = True
            Me.AllowPan = False

            ' Configure graph control
            Me.UpdateStyle()
            Me.UpdateColours()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Detach a zedgraph control that was previously 
        ''' <see cref="Attach">attached</see>.
        ''' </summary>
        ''' <remarks>
        ''' Failing to detach an attached control causes memory leaks.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Detach()

            If Me.m_zgc Is Nothing Then Return

            RemoveHandler Me.m_zgc.MouseDownEvent, AddressOf OnMouseDownEvent
            RemoveHandler Me.m_zgc.MouseMoveEvent, AddressOf OnMouseMoveEvent
            RemoveHandler Me.m_zgc.MouseUpEvent, AddressOf OnMouseUpEvent
            RemoveHandler Me.m_zgc.MouseEnter, AddressOf OnMouseEnter
            RemoveHandler Me.m_zgc.MouseLeave, AddressOf OnMouseLeave
            RemoveHandler Me.m_zgc.ContextMenuBuilder, AddressOf OnBuildContextMenu
            RemoveHandler Me.m_zgc.PointValueEvent, AddressOf OnPointValueEvent

            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            Me.m_dtAxisLabels.Clear()

            Me.m_hovermenu.Close()
            Me.m_hovermenu.Dispose()
            Me.m_hovermenu = Nothing

            Me.m_zgc = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of panels in the <see cref="ZedGraph">graph</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumPanes() As Integer
            Get
                Return Me.m_nPanels
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cStyleGuide">style guide</see> attached to this
        ''' instance.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property StyleGuide() As cStyleGuide
            Get
                Return Me.m_uic.StyleGuide
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cCore">core</see> attached to this instance.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Core() As cCore
            Get
                Return Me.m_uic.Core
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="ZedGraphControl">graph</see> attached to this instance.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Graph() As ZedGraphControl
            Get
                Return Me.m_zgc
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a graph pane.
        ''' </summary>
        ''' <param name="iPane">The one-based index of the pane to return. This 
        ''' index should be between 1 and <see cref="NumPanes">NumPanes</see>.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function GetPane(ByVal iPane As Integer) As ZedGraph.GraphPane

            Dim pane As GraphPane = Nothing

            If Me.m_nPanels = 1 Then pane = Me.m_zgc.GraphPane
            pane = Me.m_zgc.MasterPane.PaneList(iPane - 1)

            Debug.Assert(pane IsNot Nothing, "ZedGraphHelper already disconnected")

            Return pane

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure main panel
        ''' </summary>
        ''' <param name="strTitle">The title to set to the master pane.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Configure(ByVal strTitle As String)

            With Me.m_zgc.MasterPane
                .Title.Text = strTitle
                .Title.IsVisible = Not String.IsNullOrEmpty(strTitle)
            End With

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a single <see cref="GraphPane">pane</see> in the graph.
        ''' </summary>
        ''' <param name="strTitle">Title for the pane.</param>
        ''' <param name="strXAxisLabel">Label for the X-axis.</param>
        ''' <param name="dXAxisMin">X-axis min scale.</param>
        ''' <param name="dXAxisMax">X-axis max scale.</param>
        ''' <param name="strYAxisLabel">Label for the Y-axis.</param>
        ''' <param name="dYAxisMin">Y-axis min scale.</param>
        ''' <param name="dYAxisMax">Y-axis max scale.</param>
        ''' <param name="bShowLegend">Flag stating whether the legend should be shown.</param>
        ''' <param name="legendPos">Legend <see cref="LegendPos">position</see>.</param>
        ''' <param name="iPane">The pane to configure. If not specified, the main pane
        ''' is configured.</param>
        ''' <returns>The configured <see cref="GraphPane">GraphPane</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ConfigurePane(ByVal strTitle As String, _
            ByVal strXAxisLabel As String, ByVal dXAxisMin As Double, ByVal dXAxisMax As Double, _
            ByVal strYAxisLabel As String, ByVal dYAxisMin As Double, ByVal dYAxisMax As Double, _
            ByVal bShowLegend As Boolean, Optional ByVal legendPos As LegendPos = LegendPos.TopCenter, _
            Optional ByVal iPane As Integer = 1) As GraphPane

            Return Me.ConfigurePane(strTitle, _
                                    strXAxisLabel, Nothing, dXAxisMin, dXAxisMax, _
                                    strYAxisLabel, Nothing, dYAxisMin, dYAxisMax, _
                                    bShowLegend, legendPos, iPane)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a single <see cref="GraphPane">pane</see> in the graph.
        ''' </summary>
        ''' <param name="strTitle">Title for the pane.</param>
        ''' <param name="strXAxisLabel">Label for the X-axis.</param>
        ''' <param name="aUnitsXAxis">Units to display in the x-axis label.</param>
        ''' <param name="dXAxisMin">X-axis min scale.</param>
        ''' <param name="dXAxisMax">X-axis max scale.</param>
        ''' <param name="strYAxisLabel">Label for the Y-axis.</param>
        ''' <param name="aUnitsYAxis">Units to display in the Y-axis label.</param>
        ''' <param name="dYAxisMin">Y-axis min scale.</param>
        ''' <param name="dYAxisMax">Y-axis max scale.</param>
        ''' <param name="bShowLegend">Flag stating whether the legend should be shown.</param>
        ''' <param name="legendPos">Legend <see cref="LegendPos">position</see>.</param>
        ''' <param name="iPane">The pane to configure. If not specified, the main pane
        ''' is configured.</param>
        ''' <returns>The configured <see cref="GraphPane">GraphPane</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ConfigurePane(ByVal strTitle As String, _
            ByVal strXAxisLabel As String, ByVal aUnitsXAxis() As cStyleGuide.eUnitType, ByVal dXAxisMin As Double, ByVal dXAxisMax As Double, _
            ByVal strYAxisLabel As String, ByVal aUnitsYAxis() As cStyleGuide.eUnitType, ByVal dYAxisMin As Double, ByVal dYAxisMax As Double, _
            ByVal bShowLegend As Boolean, Optional ByVal legendPos As LegendPos = LegendPos.TopCenter, _
            Optional ByVal iPane As Integer = 1) As GraphPane

            Dim gp As GraphPane = Me.ConfigurePane(strTitle, _
                                                   strXAxisLabel, aUnitsXAxis, _
                                                   strYAxisLabel, aUnitsYAxis, _
                                                   bShowLegend, legendPos, iPane)
            With gp

                .XAxis.Scale.Min = dXAxisMin
                .XAxis.Scale.MinGrace = 0.0#
                .XAxis.Scale.MaxGrace = 0.0#
                If dXAxisMin <> dXAxisMax Then .XAxis.Scale.Max = dXAxisMax

                .YAxis.Scale.Min = dYAxisMin
                .YAxis.Scale.MinGrace = Me.m_sYScaleGrace
                .YAxis.Scale.MaxGrace = Me.m_sYScaleGrace
                If dYAxisMin <> dYAxisMax Then .YAxis.Scale.Max = dYAxisMax

            End With
            Me.RescaleAndRedraw()

            Return gp

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a single <see cref="GraphPane">pane</see> in the graph.
        ''' </summary>
        ''' <param name="strTitle">Title for the pane.</param>
        ''' <param name="strXAxisLabel">Label for the X-axis.</param>
        ''' <param name="strYAxisLabel">Label for the Y-axis.</param>
        ''' <param name="bShowLegend">Flag stating whether the legend should be shown.</param>
        ''' <param name="legendPos">Legend <see cref="LegendPos">position</see>.</param>
        ''' <param name="iPane">The pane to configure. If not specified, the main pane
        ''' is configured.</param>
        ''' <returns>The configured <see cref="GraphPane">GraphPane</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ConfigurePane(ByVal strTitle As String, _
             ByVal strXAxisLabel As String, ByVal strYAxisLabel As String, _
             ByVal bShowLegend As Boolean, Optional ByVal legendPos As LegendPos = LegendPos.TopCenter, _
             Optional ByVal iPane As Integer = 1) As GraphPane

            Return Me.ConfigurePane(strTitle, strXAxisLabel, Nothing, strYAxisLabel, Nothing, bShowLegend, legendPos, iPane)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a single <see cref="GraphPane">pane</see> in the graph.
        ''' </summary>
        ''' <param name="strTitle">Title for the pane.</param>
        ''' <param name="strXAxisLabel">Label for the X-axis.</param>
        ''' <param name="aUnitsXAxis">Units to display in the x-axis label.</param>
        ''' <param name="strYAxisLabel">Label for the Y-axis.</param>
        ''' <param name="aUnitsYAxis">Units to display in the Y-axis label.</param>
        ''' <param name="bShowLegend">Flag stating whether the legend should be shown.</param>
        ''' <param name="legendPos">Legend <see cref="LegendPos">position</see>.</param>
        ''' <param name="iPane">The pane to configure. If not specified, the main pane
        ''' is configured.</param>
        ''' <returns>The configured <see cref="GraphPane">GraphPane</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ConfigurePane(ByVal strTitle As String, _
             ByVal strXAxisLabel As String, ByVal aUnitsXAxis() As cStyleGuide.eUnitType, _
             ByVal strYAxisLabel As String, ByVal aUnitsYAxis() As cStyleGuide.eUnitType, _
             ByVal bShowLegend As Boolean, Optional ByVal legendPos As LegendPos = LegendPos.TopCenter, _
             Optional ByVal iPane As Integer = 1) As GraphPane

            Me.m_bShowLegend = bShowLegend

            Dim gp As GraphPane = Me.GetPane(iPane)
            With gp

                ' Set title
                .Title.Text = strTitle
                .Title.IsVisible = Not String.IsNullOrEmpty(strTitle)

                ' Configure axis
                Me.AxisLabel(.XAxis, strXAxisLabel, aUnitsXAxis)
                .XAxis.Title.IsVisible = Not String.IsNullOrEmpty(strXAxisLabel)
                .XAxis.MinorTic.IsAllTics = False
                .XAxis.MinorTic.IsOpposite = False
                .XAxis.MajorTic.IsOpposite = False
                .XAxis.Scale.MinGrace = 0.0#
                .XAxis.Scale.MaxGrace = 0.0#

                Me.AxisLabel(.YAxis, strYAxisLabel, aUnitsYAxis)
                .YAxis.Title.IsVisible = Not String.IsNullOrEmpty(strYAxisLabel)
                .YAxis.MinorTic.IsAllTics = False
                .YAxis.MinorTic.IsOpposite = False
                .YAxis.MajorTic.IsOpposite = False
                .YAxis.Scale.MinGrace = Me.m_sYScaleGrace
                .YAxis.Scale.MaxGrace = Me.m_sYScaleGrace

                .Legend.Position = legendPos

                .Border.IsVisible = False
                .Chart.Border.IsVisible = True

                Me.UpdateLegends(gp)

            End With

            Me.RescaleAndRedraw()

            Return gp

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a series of lines to the ZedGraph.
        ''' </summary>
        ''' <param name="lines">The <see cref="LineItem">lines</see> to add.</param>
        ''' <param name="iPane">The panel to assign these lines to (optional).</param>
        ''' <param name="bRescale">Flag stating whether the graph needs to be
        ''' rescaled (optional).</param>
        ''' <remarks>Note that this method clears out all lines existing in the
        ''' indicated panel.</remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Sub PlotLines(ByVal lines() As LineItem, _
                             Optional ByVal iPane As Integer = 1, _
                             Optional ByVal bRescale As Boolean = True, _
                             Optional ByVal bClear As Boolean = True, _
                             Optional ByVal bCumulative As Boolean = False)
            Try

                If (Me.IsPaneCumulative(iPane) <> bCumulative) Then
                    bClear = True
                    Me.IsPaneCumulative(iPane) = bCumulative
                End If

                Dim li As LineItem = Nothing
                Dim linetype As eLineType = eLineType.NotSet

                With Me.GetPane(iPane)

                    If bClear Then .CurveList.Clear()

                    If lines IsNot Nothing Then
                        For i As Integer = 0 To lines.Length - 1
                            ' Get line
                            li = lines(i)
                            ' Just to make sure
                            If (li IsNot Nothing) Then

                                ' Has no line title?
                                If String.IsNullOrEmpty(li.Label.Text) Then
                                    ' #Yes: use pane title to identify line
                                    li.Label.Text = .Title.Text
                                End If

                                Select Case Me.CurveType(li)

                                    Case eLineType.ModelData

                                        If Me.IsPaneCumulative(iPane) Then

                                            ' Note that this code assumes that every line added here has the 
                                            ' exact number of points in the exact same X-axis order. No validations 
                                            ' are performed whether this is indeed the case

                                            ' ZedGraph renders curvelists last to first. Higher cumulative curves are
                                            ' thus stored with increasing indices in the list
                                            Dim iLastLine As Integer = Me.FindLastCurvePos(eLineType.ModelData, iPane)

                                            If iLastLine > -1 Then
                                                Me.SumLines(DirectCast(.CurveList(iLastLine), LineItem), li)
                                            End If

                                            ' Set cumulative colour style
                                            li.Line.Fill = New Fill(li.Color)
                                            li.Line.Fill.IsVisible = True
                                            li.Line.Color = Color.SlateGray

                                            ' Add the curve to the end 
                                            .CurveList.Insert(iLastLine + 1, li)
                                        Else
                                            .CurveList.Add(li)
                                        End If

                                    Case eLineType.ReferenceData

                                        ' Reference curves should be rendered on top of everything else.
                                        ' Hence, reference curves are 
                                        .CurveList.Insert(0, li)

                                    Case eLineType.NotSet

                                        ' Unknow data type: just append curve to end of the list
                                        .CurveList.Add(li)

                                End Select
                            Else
                                Debug.Assert(False)
                            End If
                        Next i

                    End If
                End With

                If bRescale Then Me.RescaleAndRedraw(iPane) Else Me.Redraw()

            Catch ex As Exception
                EwECore.cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".PlotLines() " & ex.Message, ex)
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Redraw the wrapped ZedGraph.
        ''' </summary>
        ''' <param name="iPane">The pane to redraw, or -1 to redraw all panes 
        ''' in the graph.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Redraw(Optional ByVal iPane As Integer = -1)
            Me.m_zgc.Invalidate()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Totally redraw the wrapped ZedGraph by recalculating the axis and 
        ''' invalidating all panels.
        ''' </summary>
        ''' <param name="iPane">The pane to rescale and redraw, or -1 to
        ''' update all panes in the graph.</param>
        ''' <remarks>When using cursors please use this method to rescale the
        ''' graph axis.</remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Sub RescaleAndRedraw(Optional ByVal iPane As Integer = -1)

            Dim iMin As Integer = 1
            Dim iMax As Integer = Me.m_nPanels
            Dim abCursor(Me.m_nPanels) As Boolean

            If iPane > -1 Then
                iMin = Math.Max(iMin, iPane)
                iMax = Math.Min(iMax, iPane)
            End If

            ' Hide cursors, but remember settings
            For iPane = iMin To iMax
                abCursor(iPane) = Me.ShowCursor(iPane)
                Me.ShowCursor(iPane) = False
                With Me.GetPane(iPane).YAxis.Scale
                    .MaxGrace = Me.YScaleGrace
                    .MinGrace = Me.YScaleGrace
                End With
                With Me.GetPane(iPane).XAxis.Scale
                    .MaxGrace = 0
                    .MinGrace = 0
                End With
            Next

            ' Recalc axis
            Me.m_zgc.AxisChange()

            ' Restore cursors
            For iPane = iMin To iMax
                Me.ShowCursor(iPane) = abCursor(iPane)
            Next

            Me.Redraw()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether Y values in a given pane should be auto-scaled.
        ''' </summary>
        ''' <param name="iPane"></param>
        ''' -------------------------------------------------------------------
        Public Property AutoscalePane(Optional ByVal iPane As Integer = 1) As Boolean
            Get
                With Me.GetPane(iPane).YAxis.Scale
                    Return .MaxAuto And .MinAuto
                End With
            End Get
            Set(ByVal bAutoscale As Boolean)
                With Me.GetPane(iPane).YAxis.Scale
                    .MinAuto = bAutoscale
                    .MaxAuto = bAutoscale
                    Me.RescaleAndRedraw(iPane)
                End With
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the auto-scale behaviour of a pane Y axis.
        ''' </summary>
        ''' <param name="iPane"></param>
        ''' -------------------------------------------------------------------
        Public Property AutoScaleYOption(Optional ByVal iPane As Integer = 1) As eScaleOptionTypes
            Get
                With Me.GetPane(iPane).YAxis.Scale
                    If .MinAuto And .MaxAuto Then
                        Return eScaleOptionTypes.Both
                    ElseIf .MaxAuto And Not .MinAuto Then
                        Return eScaleOptionTypes.MaxOnly
                    ElseIf Not .MaxAuto And .MinAuto Then
                        Return eScaleOptionTypes.MinOnly
                    ElseIf Not .MaxAuto And Not .MinAuto Then
                        Return eScaleOptionTypes.None
                    End If
                    Return eScaleOptionTypes.None
                End With
            End Get
            Set(ByVal value As eScaleOptionTypes)
                With Me.GetPane(iPane).YAxis.Scale
                    Select Case value
                        Case eScaleOptionTypes.Both
                            .MinAuto = True
                            .MaxAuto = True
                        Case eScaleOptionTypes.MaxOnly
                            .MaxAuto = True
                            .MinAuto = False
                        Case eScaleOptionTypes.MinOnly
                            .MaxAuto = False
                            .MinAuto = True
                        Case eScaleOptionTypes.None
                            .MinAuto = False
                            .MaxAuto = False
                    End Select
                    Me.RescaleAndRedraw(iPane)
                End With
            End Set
        End Property

        Public Property YScaleMin(Optional ByVal iPane As Integer = 1) As Double
            Get
                Return Me.GetPane(iPane).YAxis.Scale.Min
            End Get
            Set(ByVal value As Double)
                Me.GetPane(iPane).YAxis.Scale.Min = value
                RescaleAndRedraw(iPane)
            End Set
        End Property

        Public Property XScaleMax(Optional ByVal iPane As Integer = 1) As Double
            Get
                Return Me.GetPane(iPane).XAxis.Scale.Max
            End Get
            Set(ByVal value As Double)
                Me.GetPane(iPane).XAxis.Scale.Max = value
                RescaleAndRedraw(iPane)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the grace margin for Y axis.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property YScaleGrace() As Single
            Get
                Return Me.m_sYScaleGrace
            End Get
            Set(ByVal value As Single)
                Me.m_sYScaleGrace = value
                Me.RescaleAndRedraw()
            End Set
        End Property

        Public Property XScaleMin(Optional ByVal iPane As Integer = 1) As Double
            Get
                Return Me.GetPane(iPane).XAxis.Scale.Min
            End Get
            Set(ByVal value As Double)
                Me.AutoScaleYOption(iPane) = eScaleOptionTypes.None
                Me.GetPane(iPane).XAxis.Scale.Min = value
                Me.RescaleAndRedraw(iPane)
            End Set
        End Property

        Public Property YScaleMax(Optional ByVal iPane As Integer = 1) As Double
            Get
                Return Me.GetPane(iPane).YAxis.Scale.Max
            End Get
            Set(ByVal value As Double)
                Me.AutoScaleYOption(iPane) = eScaleOptionTypes.None
                Me.GetPane(iPane).YAxis.Scale.Max = value
                Me.RescaleAndRedraw(iPane)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set whether vetical zoom is allowed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public WriteOnly Property AllowZoom() As Boolean
            Set(ByVal value As Boolean)
                Me.m_zgc.IsZoomOnMouseCenter = value
                Me.m_zgc.IsEnableVZoom = value
                Me.m_zgc.IsEnableHZoom = False
                Me.m_zgc.IsEnableWheelZoom = value
                Me.m_zgc.IsEnableZoom = value
                If value Then
                    Me.m_zgc.ZoomButtons = MouseButtons.Left
                Else
                    Me.m_zgc.ZoomButtons = MouseButtons.None
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set whether vetical pan is allowed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public WriteOnly Property AllowPan() As Boolean
            Set(ByVal value As Boolean)
                Me.m_zgc.IsEnableVPan = value
                Me.m_zgc.IsEnableHPan = False
                If value Then
                    Me.m_zgc.PanButtons = MouseButtons.Left
                Else
                    Me.m_zgc.EditButtons = MouseButtons.None
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set whether value edits are allowed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public WriteOnly Property AllowEdit() As Boolean
            Set(ByVal value As Boolean)
                Me.m_zgc.IsEnableHEdit = value
                Me.m_zgc.IsEnableVEdit = value
                If value Then
                    Me.m_zgc.EditButtons = MouseButtons.Right
                Else
                    Me.m_zgc.EditButtons = MouseButtons.None
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the values in a pane should be added cumulatively.
        ''' </summary>
        ''' <param name="iPane"></param>
        ''' -------------------------------------------------------------------
        Public Property IsPaneCumulative(Optional ByVal iPane As Integer = 1) As Boolean
            Get
                Return Me.m_bCumulative(iPane)
            End Get
            Protected Set(ByVal value As Boolean)
                Me.m_bCumulative(iPane) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the graph tracks styleguide item visibility settings.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsTrackVisiblity() As Boolean
            Get
                Return Me.m_bTrackVisibility
            End Get
            Set(ByVal value As Boolean)
                If (value <> Me.m_bTrackVisibility) Then
                    value = Me.m_bTrackVisibility
                    Me.UpdateCurveVisibility()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether to display legends.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsLegendVisible() As Boolean
            Get
                Return Me.m_bShowLegend
            End Get
            Set(ByVal value As Boolean)
                If (value <> Me.m_bShowLegend) Then
                    Me.m_bShowLegend = value
                    For i As Integer = 1 To Me.m_nPanels
                        Me.UpdateLegends(Me.GetPane(i))
                    Next
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an EwE-styled line for either an Ecopath group or fleet.
        ''' </summary>
        ''' <param name="ppl"></param>
        ''' <remarks>All other source types will be rejected.</remarks>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function CreateLineItem(ByVal src As ICoreInterface, _
                                                   ByVal ppl As PointPairList, _
                                                   Optional ByVal strLabel As String = "") As LineItem
            ' SAnity check
            Debug.Assert(TypeOf (src) Is cEcoPathGroupInput Or TypeOf (src) Is cFleetInput)
            Return Me.CreateLineItem(New cCurveInfo(src, Me.m_uic, strLabel), ppl)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an EwE-styled line.
        ''' </summary>
        ''' <param name="strName"></param>
        ''' <param name="curveType"></param>
        ''' <param name="clr"></param>
        ''' <param name="ppl"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function CreateLineItem(ByVal strName As String, _
                                                   ByVal curveType As eLineType, _
                                                   ByVal clr As Color, _
                                                   ByVal ppl As PointPairList) As LineItem
            Return Me.CreateLineItem(New cCurveInfo(strName, clr, curveType), ppl)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Find the next curve of a given <see cref="eLineType">type</see>.
        ''' </summary>
        ''' <param name="curvetype">The <see cref="eLineType">type</see> of the
        ''' curve to locate.</param>
        ''' <param name="iPane">Index of the graph pane to look into.</param>
        ''' <param name="iStart">Search start index, 0 by default.</param>
        ''' <returns>Index of the curve that matches the line type, or -1 if
        ''' no such curve could be found.</returns>
        ''' -------------------------------------------------------------------
        Public Function FindNextCurvePos(ByVal curvetype As eLineType, _
                                         Optional ByVal iPane As Integer = 1, _
                                         Optional ByVal iStart As Integer = 0) As Integer

            Dim pane As GraphPane = Me.GetPane(iPane)
            Dim ci As CurveItem = Nothing

            If (pane Is Nothing) Then Return -1

            For iCurve As Integer = iStart To pane.CurveList.Count - 1
                ci = pane.CurveList(iCurve)
                If Me.CurveType(ci) = curvetype Then Return iCurve
            Next
            Return -1

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Find the previous curve of a given <see cref="eLineType">type</see>.
        ''' </summary>
        ''' <param name="curvetype">The <see cref="eLineType">type</see> of the
        ''' curve to locate.</param>
        ''' <param name="iPane">Index of the graph pane to look into.</param>
        ''' <param name="iStart">Search start index, provide -1 to start searching
        ''' at the end of the curve list.</param>
        ''' <returns>Index of the curve that matches the line type, or -1 if
        ''' no such curve could be found.</returns>
        ''' -------------------------------------------------------------------
        Public Function FindLastCurvePos(ByVal curvetype As eLineType, _
                                          Optional ByVal iPane As Integer = 1, _
                                          Optional ByVal iStart As Integer = -1) As Integer

            Dim pane As GraphPane = Me.GetPane(iPane)
            Dim ci As CurveItem = Nothing

            If (pane Is Nothing) Then Return -1

            ' Fix default
            If (iStart = -1) Then iStart = pane.CurveList.Count

            For iCurve As Integer = Math.Min(iStart, pane.CurveList.Count - 1) To 0 Step -1
                ci = pane.CurveList(iCurve)
                If Me.CurveType(ci) = curvetype Then Return iCurve
            Next
            Return -1

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the <see cref="eLineType">type</see> of a curve.
        ''' </summary>
        ''' <param name="ci">The curve to extract information for.</param>
        ''' <returns>A <see cref="eLineType">type</see>, or NotSet if this 
        ''' information could not be found.</returns>
        ''' <remarks>
        ''' Note that this information only works on curves created via 
        ''' <see cref="CreateLineItem">CreateLineItem</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function CurveType(ByVal ci As CurveItem) As eLineType
            Dim info As cCurveInfo = Me.CurveInfo(ci)
            If (info Is Nothing) Then Return eLineType.NotSet
            Return info.LineType
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the <see cref="cCurveInfo">curve info</see> for a curve.
        ''' </summary>
        ''' <param name="ci">The curve to extract information for.</param>
        ''' <returns>
        ''' A <see cref="cCurveInfo">curve info</see> instance, or Nothing if
        ''' this information could not be found.
        ''' </returns>
        ''' <remarks>
        ''' Note that this information only works on curves created via 
        ''' <see cref="CreateLineItem">CreateLineItem</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Function CurveInfo(ByVal ci As CurveItem) As cCurveInfo
            If (ci Is Nothing) Then Return Nothing
            If (ci.Tag Is Nothing) Then Return Nothing
            If Not (TypeOf ci.Tag Is cCurveInfo) Then Return Nothing
            Return DirectCast(ci.Tag, cCurveInfo)
        End Function

#Region " Tooltip "

        Public Property ShowPointValue() As Boolean
            Get
                Return Me.m_zgc.IsShowPointValues
            End Get
            Set(ByVal value As Boolean)
                Me.m_zgc.IsShowPointValues = value
            End Set
        End Property

        Private Function OnPointValueEvent(ByVal sender As Object, ByVal pane As GraphPane, ByVal curve As CurveItem, ByVal iPoint As Integer) As String
            Dim pp As PointPair = curve(iPoint)
            Return String.Format(My.Resources.GENERIC_LABEL_GRAPHVALUE, _
                                 curve.Label.Text, _
                                 Me.StyleGuide.FormatNumber(pp.X), _
                                 Me.StyleGuide.FormatNumber(pp.Y))
        End Function

#End Region ' Tooltip

#Region " Axis label management "

        Private Class cAxisLabelFormatter

            Private m_uic As cUIContext = Nothing
            Private m_axis As Axis = Nothing
            Protected m_aUnitTypes() As cStyleGuide.eUnitType
            Protected m_strUnitMask As String = ""

            Public Sub New(ByVal uic As cUIContext, _
                           ByVal axis As Axis, _
                           ByVal strUnitMask As String, _
                           ByVal aUnitTypes() As cStyleGuide.eUnitType)

                Me.m_uic = uic
                Me.m_axis = axis
                Me.m_strUnitMask = strUnitMask
                Me.m_aUnitTypes = aUnitTypes

            End Sub

            Public Sub Update()
                Me.m_axis.Title.Text = Me.LabelText()
            End Sub

            Private ReadOnly Property LabelText() As String
                Get
                    Dim strDisplayText As String = ""

                    If Me.m_aUnitTypes IsNot Nothing Then

                        Select Case m_aUnitTypes.Length
                            Case 0
                                ' NOP
                            Case 1
                                strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)))
                            Case 2
                                strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)), GetUnitString(m_aUnitTypes(1)))
                            Case Else
                                Debug.Assert(False)
                        End Select

                    End If

                    Return strDisplayText
                End Get
            End Property

            Private Function GetUnitString(ByVal unitType As cStyleGuide.eUnitType) As String
                Dim sg As cStyleGuide = Me.m_uic.StyleGuide
                Dim strUnitString As String = ""
                Select Case unitType
                    Case cStyleGuide.eUnitType.Currency
                        strUnitString = sg.CurrencyUnitText(sg.CurrencyUnit)
                    Case cStyleGuide.eUnitType.Time
                        strUnitString = sg.TimeUnitText(sg.TimeUnit)
                    Case cStyleGuide.eUnitType.Monetary
                        strUnitString = sg.MonetaryUnitText(sg.MonetaryUnit)
                    Case cStyleGuide.eUnitType.Nominal
                        strUnitString = sg.NominalUnitText()
                    Case Else
                        Debug.Assert(False)
                End Select
                Return strUnitString
            End Function
        End Class

        Private Sub RefreshAxisLabels()
            For Each Axis As Axis In Me.m_dtAxisLabels.Keys
                Me.m_dtAxisLabels(Axis).Update()
            Next
            Me.m_zgc.Refresh()
        End Sub

        Public Sub AxisLabel(ByVal axis As Axis, _
                             ByVal strLabel As String, _
                             Optional ByVal aUnitTypes() As cStyleGuide.eUnitType = Nothing)
            If String.IsNullOrEmpty(strLabel) Then
                Try
                    Me.m_dtAxisLabels.Remove(axis)
                Catch ex As Exception
                End Try
            End If

            If aUnitTypes IsNot Nothing Then
                Try
                    Dim alf As New cAxisLabelFormatter(Me.m_uic, axis, strLabel, aUnitTypes)
                    Me.m_dtAxisLabels(axis) = alf
                    alf.Update()
                Catch ex As Exception
                End Try
            Else
                axis.Title.Text = strLabel
            End If

            axis.Scale.IsUseTenPower = True
            axis.Scale.MagAuto = Not String.IsNullOrEmpty(strLabel)

        End Sub

#End Region ' Axis label management

#Region " Cursor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event for public consumption called when the cursor position changes
        ''' in a graph pane.
        ''' </summary>
        ''' <param name="zgh">The zed graph helper that sent out the event.</param>
        ''' <param name="iPane">The index of the pane that the cursor change event
        ''' pertains to.</param>
        ''' <param name="sPos">The new cursor position.</param>
        ''' -------------------------------------------------------------------
        Public Event OnCursorPos(ByVal zgh As cZedGraphHelper, ByVal iPane As Integer, ByVal sPos As Single)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Show or hide a vertical cursor to a graph pane.
        ''' </summary>
        ''' <param name="iPane">The index of the pane to show or hide the cursor for.</param>
        ''' <remarks>
        ''' <para>Note that ZedGraph does not support a real cursor. Instead, 
        ''' cursors are simulated by manually setting a vertical line in the 
        ''' pane, which will conflict with the Y-axis autoscale ability.</para>
        ''' <para>You will need to manually remove and restore the cursor
        ''' around <see cref="ZedGraphControl.AxisChange">AxisChange</see> 
        ''' events. The ZedGraphHelper method 
        ''' <see cref="RescaleAndRedraw">RescaleAndRedraw</see> performs this
        ''' for you.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property ShowCursor(Optional ByVal iPane As Integer = 1) As Boolean
            Get
                Return Me.m_bShowCursor(iPane)
            End Get
            Set(ByVal value As Boolean)
                Dim gp As GraphPane = Me.GetPane(iPane)
                If (value <> Me.m_bShowCursor(iPane)) Then
                    Me.RemoveCursor(iPane)
                    Me.m_bShowCursor(iPane) = value
                    Me.SetCursor(iPane)
                End If
                Me.m_zgc.IsEnableZoom = (Me.m_bShowCursor(iPane) = False)
                Me.m_zgc.Cursor = DirectCast(IIf(value, Cursors.Hand, Cursors.Default), Cursor)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the cursor position for a graph pane.
        ''' </summary>
        ''' <param name="iPane">The pane to access the cursor position for.</param>
        ''' <remarks>Note that this will not make a cursor appear. The cursor
        ''' visibility state should be set with 
        ''' <see cref="ShowCursor">ShowCursor</see> first.</remarks>
        ''' -------------------------------------------------------------------
        Public Property CursorPos(Optional ByVal iPane As Integer = 1) As Single
            Get
                Return Me.m_sCursorPos(iPane)
            End Get
            Set(ByVal value As Single)
                If (value <> Me.m_sCursorPos(iPane)) Then
                    Me.RemoveCursor(iPane)
                    If value <> Me.m_sCursorPos(iPane) Then
                        Me.m_sCursorPos(iPane) = value
                        RaiseEvent OnCursorPos(Me, iPane, value)
                    End If
                    Me.SetCursor(iPane)
                End If
            End Set
        End Property

#End Region ' Cursor

#Region " Context Menu "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Extract the data in the ZedGraph to a comma-separated (.CSV) file.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Function ExtractDataToCSV() As Boolean

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim sw As StreamWriter = Nothing
            Dim strFN As String = ""

            If Me.m_zgc.MasterPane.PaneList.Count = 1 Then
                strFN = FileUtilities.ToValidFileName(Me.m_zgc.MasterPane.Title.Text, False)
            End If

            cmdFS.Invoke(strFN, My.Resources.FILEFILTER_CSV, 0)

            If cmdFS.Result = DialogResult.OK Then
                sw = New StreamWriter(cmdFS.FileName)
                If (sw IsNot Nothing) Then
                    ' Code to write the stream goes here.
                    sw.Write(ExtractData(Me.m_zgc))
                    sw.Close()
                End If
            End If

            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Extract the data in the graph to a comma-separated string.
        ''' </summary>
        ''' <param name="z">The graph to extract the data from.</param>
        ''' <returns>A massive string. Format to be described</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function ExtractData(ByVal z As ZedGraphControl) As String

            Dim cult As CultureInfo = Thread.CurrentThread.CurrentUICulture
            Dim nfi As NumberFormatInfo = DirectCast(cult.NumberFormat.Clone(), NumberFormatInfo)
            Dim sb As New StringBuilder
            Dim sbX As StringBuilder = Nothing
            Dim sbY As StringBuilder = Nothing
            Dim gp As GraphPane = Nothing

            nfi.NumberDecimalSeparator = "."

            ' Safety first
            If z IsNot Nothing Then
                ' Each Zedgraph Plane
                For Each p As ZedGraph.PaneBase In z.MasterPane.PaneList
                    ' Check if it's a graphpane
                    If TypeOf (p) Is GraphPane Then
                        gp = DirectCast(p, GraphPane)

                        ' Print the title
                        sb.AppendLine(String.Format("{0}{1}{0}", Chr(34), p.Title.Text))
                        For Each ci As CurveItem In gp.CurveList
                            ' Print Item
                            sb.AppendLine(String.Format("{0}{1}{0}", Chr(34), ci.Label.Text))
                            sbX = New StringBuilder("x")
                            sbY = New StringBuilder("y")
                            For i As Integer = 0 To ci.NPts - 1
                                sbX.Append(", ")
                                sbX.Append(Convert.ToString(ci.Points(i).X, nfi))
                                sbY.Append(", ")
                                sbY.Append(Convert.ToString(ci.Points(i).Y, nfi))
                            Next

                            sb.AppendLine(sbX.ToString())
                            sb.AppendLine(sbY.ToString())
                        Next
                    End If
                Next
            End If

            Return sb.ToString()
        End Function

#End Region ' Context Menu

#Region " Hover menu "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether a hovering menu should be displayed on the graph.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShowHoverMenu() As Boolean
            Get
                Return Me.m_bShowHoverMenu
            End Get
            Set(ByVal value As Boolean)
                If value <> Me.m_bShowHoverMenu Then
                    Me.m_bShowHoverMenu = value
                    Me.UpdateHoverMenuState()
                End If
            End Set
        End Property

#End Region ' Hover menu

#Region " Pane value querying "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get mean value of all <see cref="eLineType.ModelData">model data</see>
        ''' in a pane.
        ''' </summary>
        ''' <param name="iPane"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function GetValueAvg(Optional ByVal iPane As Integer = 1) As Single

            Dim gp As GraphPane = Me.GetPane(iPane)
            Dim ci As CurveItem = Nothing
            Dim dTotal As Double = 0.0#
            Dim iNumValues As Integer = 0

            For iCurve As Integer = 0 To gp.CurveList.Count - 1
                ci = gp.CurveList(iCurve)
                If Me.CurveType(ci) = eLineType.ModelData Then
                    For iPT As Integer = 0 To ci.Points.Count - 1
                        dTotal += ci.Points(iPT).Y
                        iNumValues += 1
                    Next
                End If
            Next

            If (iNumValues = 0) Then
                Return 0
            Else
                Return CSng(dTotal / iNumValues)
            End If

        End Function

#End Region ' Pane value querying

#End Region ' Public interfaces

#Region " Events "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="changeType"></param>
        ''' -----------------------------------------------------------------------
        Protected Overridable Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)

            If ((changeType And cStyleGuide.eChangeType.Fonts) > 0) Then
                Me.UpdateStyle()
            End If

            If ((changeType And cStyleGuide.eChangeType.Colours) > 0) Then
                Me.UpdateColours()
            End If

            If ((changeType And cStyleGuide.eChangeType.GroupVisibility) > 0) Or _
               ((changeType And cStyleGuide.eChangeType.FleetVisibility) > 0) Then
                Me.UpdateCurveVisibility()
            End If

            If ((changeType And cStyleGuide.eChangeType.Units) > 0) Then
                Me.RefreshAxisLabels()
            End If

            If ((changeType And cStyleGuide.eChangeType.Legends) > 0) Then
                Me.UpdateLegends()
            End If
        End Sub

        Private Function OnMouseDownEvent(ByVal zg As ZedGraphControl, ByVal args As MouseEventArgs) As Boolean

            Dim iPane As Integer = GetPaneAtPoint(args.Location)
            Dim pane As GraphPane = Nothing
            Dim ciNearest As CurveItem = Nothing
            Dim iNearest As Integer = -1

            If iPane > -1 Then

                ' Get the clicked pane
                pane = Me.GetPane(iPane)

                'If (pane.FindNearestPoint(args.Location, ciNearest, iNearest)) Then
                '    RaiseEvent OnCurveClicked(ciNearest, iNearest)
                'End If

                ' Cursor?
                If Me.m_bShowCursor(iPane) Then
                    Me.CursorPos = GraphToScale(New PointF(args.Location.X, args.Location.Y)).X
                    Return True
                End If

            End If
            Return False
        End Function

        Private Function OnMouseMoveEvent(ByVal zg As ZedGraphControl, ByVal args As MouseEventArgs) As Boolean

            Dim iPane As Integer = GetPaneAtPoint(args.Location)
            Dim ciNearest As CurveItem = Nothing
            Dim iNearest As Integer = -1

            If iPane > -1 Then

                ' Get the clicked pane
                Dim pane As GraphPane = Nothing

                'If (pane.FindNearestPoint(args.Location, ciNearest, iNearest)) Then
                '    Me.m_zgc.
                'End If

                ' Cursor?
                If Me.m_bShowCursor(iPane) Then
                    Me.CursorPos = GraphToScale(New PointF(args.Location.X, args.Location.Y)).X
                    Return True
                End If

            End If
            Return False
        End Function

        Private Function OnMouseUpEvent(ByVal zg As ZedGraphControl, ByVal args As MouseEventArgs) As Boolean
            Dim iPanel As Integer = GetPaneAtPoint(args.Location)
            If iPanel > -1 Then
                If Me.m_bShowCursor(iPanel) Then
                    Me.CursorPos = CSng(Math.Round(Me.CursorPos))
                    Return True
                End If
            End If
            Return False
        End Function

#End Region ' Events

#Region " Internal bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an EwE-styled line.
        ''' </summary>
        ''' <param name="ppl"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function CreateLineItem(ByVal info As cCurveInfo, _
                                                      ByVal ppl As PointPairList) As LineItem

            Dim li As LineItem = Nothing

            Select Case info.LineType()

                Case eLineType.ReferenceData
                    li = New ZedGraph.LineItem(info.Label, ppl, info.Colour, SymbolType.Circle, 1)

                    li.Line.Color = Color.SlateGray
                    li.Line.IsVisible = False

                    li.Line.Fill.Color = info.Colour
                    li.Line.Fill.IsVisible = False

                    ' ToDo_JS: obtain symbol size from style guide
                    li.Symbol.Size = 4
                    li.Symbol.Border.Color = info.Colour
                    li.Symbol.Border.IsVisible = True
                    li.Symbol.Fill.Color = info.Colour
                    li.Symbol.Fill.IsVisible = False
                    li.Symbol.IsVisible = True

                Case eLineType.ModelData, _
                     eLineType.NotSet
                    li = New ZedGraph.LineItem(info.Label, ppl, info.Colour, SymbolType.None, 1)

            End Select

            li.IsVisible = info.IsVisible
            li.Tag = info

            Return li

        End Function

#Region " Styling "

        Protected Overridable Sub UpdateColours()

            Dim info As cCurveInfo = Nothing
            Dim gp As GraphPane = Nothing
            Dim ci As CurveItem = Nothing
            Dim acurves() As CurveItem = Nothing
            Dim iFirstDataLinePos As Integer = -1
            Dim bPaneCumulative As Boolean = False
            Dim iNumHighlights As Integer = 0

            For iPane As Integer = 1 To Me.m_nPanels
                gp = Me.GetPane(iPane)
                gp.Chart.Fill = New Fill(Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))
                gp.Fill = New Fill(Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND))

                bPaneCumulative = Me.IsPaneCumulative(iPane)
                Me.RemoveCursor(iPane)
                Me.SetCursor(iPane)

                acurves = gp.CurveList.ToArray
                iFirstDataLinePos = Me.FindNextCurvePos(eLineType.ModelData, iPane)
                iNumHighlights = 0

                For iCurve As Integer = 0 To acurves.Length - 1
                    ci = acurves(iCurve)
                    info = Me.CurveInfo(ci)
                    If (info IsNot Nothing) Then
                        If (info.IsHighlighted) Then iNumHighlights += 1
                    End If
                Next

                For iCurve As Integer = 0 To acurves.Length - 1

                    ci = acurves(iCurve)
                    info = Me.CurveInfo(ci)
                    If info IsNot Nothing Then

                        ' Not cumulative pane?
                        If (Not bPaneCumulative) Then
                            ' #Yes: Reorder coloured data lines
                            If (info.LineType = eLineType.ModelData) And _
                               (info.IsGrayedOut = False) Then
                                gp.CurveList.Remove(ci)
                                gp.CurveList.Insert(iFirstDataLinePos, ci)
                            End If
                        End If

                        ci.Color = info.Colour

                        If TypeOf ci Is LineItem Then
                            With DirectCast(ci, LineItem)
                                If bPaneCumulative Then
                                    .Line.Color = Color.Gray
                                    .Line.Fill.Color = info.Colour
                                Else
                                    .Line.Color = info.Colour
                                    .Line.Fill.Color = Color.White
                                End If
                                .Line.Width = CSng(IIf(info.IsHighlighted And iNumHighlights = 1, 3.0!, 1.0!))
                                .Symbol.Border.Color = .Line.Color
                                .Symbol.Fill.Color = .Line.Color
                            End With
                        End If

                    End If
                Next

            Next iPane

        End Sub

        Public Sub UpdateCurveVisibility()

            For iPane As Integer = 1 To Me.m_nPanels
                With Me.GetPane(iPane)
                    For Each ci As CurveItem In .CurveList
                        ci.IsVisible = Me.IsCurveVisible(ci)
                    Next
                End With
            Next iPane

        End Sub

        Protected Overridable Function IsCurveVisible(ByVal ci As CurveItem) As Boolean
            Dim info As cCurveInfo = Me.CurveInfo(ci)
            If info IsNot Nothing Then
                Return info.IsVisible Or (Me.IsTrackVisiblity = False)
            End If
            Return ci.IsVisible
        End Function

        Protected Overridable Sub UpdateStyle()

            For iPane As Integer = 1 To Me.m_nPanels
                With Me.GetPane(iPane)
                    .IsFontsScaled = False

                    .Title.FontSpec.Family = Me.StyleGuide.FontFamilyName(cStyleGuide.eApplicationFontType.Title)
                    .Title.FontSpec.Size = Me.StyleGuide.FontSize(cStyleGuide.eApplicationFontType.Title)
                    .Title.FontSpec.IsBold = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Title) And FontStyle.Bold) = FontStyle.Bold)
                    .Title.FontSpec.IsItalic = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Title) And FontStyle.Italic) = FontStyle.Italic)
                    .Title.FontSpec.IsUnderline = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Title) And FontStyle.Underline) = FontStyle.Underline)

                    .XAxis.Title.FontSpec.Family = Me.StyleGuide.FontFamilyName(cStyleGuide.eApplicationFontType.SubTitle)
                    .YAxis.Title.FontSpec.Family = .XAxis.Title.FontSpec.Family
                    .XAxis.Title.FontSpec.Size = Me.StyleGuide.FontSize(cStyleGuide.eApplicationFontType.SubTitle)
                    .YAxis.Title.FontSpec.Size = .XAxis.Title.FontSpec.Size
                    .XAxis.Title.FontSpec.IsBold = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.SubTitle) And FontStyle.Bold) = FontStyle.Bold)
                    .YAxis.Title.FontSpec.IsBold = .XAxis.Title.FontSpec.IsBold
                    .XAxis.Title.FontSpec.IsItalic = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.SubTitle) And FontStyle.Italic) = FontStyle.Italic)
                    .YAxis.Title.FontSpec.IsItalic = .XAxis.Title.FontSpec.IsItalic
                    .XAxis.Title.FontSpec.IsUnderline = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.SubTitle) And FontStyle.Underline) = FontStyle.Underline)
                    .YAxis.Title.FontSpec.IsUnderline = .XAxis.Title.FontSpec.IsUnderline

                    .XAxis.Scale.FontSpec.Family = Me.StyleGuide.FontFamilyName(cStyleGuide.eApplicationFontType.Scale)
                    .YAxis.Scale.FontSpec.Family = .XAxis.Scale.FontSpec.Family
                    .XAxis.Scale.FontSpec.Size = Me.StyleGuide.FontSize(cStyleGuide.eApplicationFontType.Scale)
                    .YAxis.Scale.FontSpec.Size = .XAxis.Scale.FontSpec.Size
                    .XAxis.Scale.FontSpec.IsBold = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Scale) And FontStyle.Bold) = FontStyle.Bold)
                    .YAxis.Scale.FontSpec.IsBold = .XAxis.Scale.FontSpec.IsBold
                    .XAxis.Scale.FontSpec.IsItalic = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Scale) And FontStyle.Italic) = FontStyle.Italic)
                    .YAxis.Scale.FontSpec.IsItalic = .XAxis.Scale.FontSpec.IsItalic
                    .XAxis.Scale.FontSpec.IsUnderline = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Scale) And FontStyle.Underline) = FontStyle.Underline)
                    .YAxis.Scale.FontSpec.IsUnderline = .XAxis.Scale.FontSpec.IsUnderline

                    .Legend.FontSpec.Family = Me.StyleGuide.FontFamilyName(cStyleGuide.eApplicationFontType.Legend)
                    .Legend.FontSpec.Size = Me.StyleGuide.FontSize(cStyleGuide.eApplicationFontType.Legend)
                    .Legend.FontSpec.IsBold = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Legend) And FontStyle.Bold) = FontStyle.Bold)
                    .Legend.FontSpec.IsItalic = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Legend) And FontStyle.Italic) = FontStyle.Italic)
                    .Legend.FontSpec.IsUnderline = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Legend) And FontStyle.Underline) = FontStyle.Underline)

                End With
            Next

            With Me.m_zgc.MasterPane
                .Border.IsVisible = False
                .Title.FontSpec.Family = Me.StyleGuide.FontFamilyName(cStyleGuide.eApplicationFontType.Title)
                .Title.FontSpec.Size = Me.StyleGuide.FontSize(cStyleGuide.eApplicationFontType.Title)
                .Title.FontSpec.IsBold = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Title) And FontStyle.Bold) = FontStyle.Bold)
                .Title.FontSpec.IsItalic = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Title) And FontStyle.Italic) = FontStyle.Italic)
                .Title.FontSpec.IsUnderline = ((Me.StyleGuide.FontStyle(cStyleGuide.eApplicationFontType.Title) And FontStyle.Underline) = FontStyle.Underline)

                Using g As Graphics = Me.m_zgc.CreateGraphics()
                    .SetLayout(g, PaneLayout.SquareColPreferred)
                End Using
            End With

            ' Redraw at your convenience
            Me.m_zgc.Invalidate()

        End Sub

        Private Sub UpdateLegends(Optional ByVal gp As GraphPane = Nothing)

            Dim bShow As Boolean = (Me.StyleGuide.ShowLegends = TriState.True) Or _
                                   (Me.StyleGuide.ShowLegends = TriState.UseDefault And Me.m_bShowLegend = True)
            If gp Is Nothing Then
                For Each gp In Me.m_zgc.MasterPane.PaneList
                    gp.Legend.IsVisible = bShow
                Next
            Else
                gp.Legend.IsVisible = bShow
            End If
            Me.m_zgc.Invalidate()

        End Sub

#End Region ' Styling

#Region " Mouse support "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the graph pane located at a given point.
        ''' </summary>
        ''' <param name="pt">The point to test.</param>
        ''' <returns>Index of a pane, or -1 if no pane was found at the given
        ''' location.</returns>
        ''' -------------------------------------------------------------------
        Protected Function GetPaneAtPoint(ByVal pt As Point) As Integer
            For i As Integer = 1 To Me.m_nPanels
                Dim gp As GraphPane = Me.GetPane(i)
                If gp.Rect.Contains(pt) Then Return i
            Next
            Return -1
        End Function

#End Region ' Mouse support

#Region " Cumulative plot support "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add an offset line to a target line
        ''' </summary>
        ''' <param name="liOffset"></param>
        ''' <param name="lTarget"></param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub SumLines(ByVal liOffset As LineItem, ByVal lTarget As LineItem)

            If (liOffset Is Nothing) Or (lTarget Is Nothing) Then Return

            For iPt As Integer = 0 To lTarget.Points.Count - 1
                lTarget(iPt).Y += liOffset.Points(iPt).Y
            Next

        End Sub

#End Region ' Cumulative plot support

#Region " Cursor "

        Protected Function GraphToScale(ByVal ptf As PointF) As PointF
            Dim myPane As GraphPane = Me.m_zgc.GraphPane
            Dim dX As Double = 0.0
            Dim dY As Double = 0.0
            myPane.ReverseTransform(ptf, dX, dY)
            Return New PointF(CSng(dX), CSng(dY))
        End Function

        Protected Sub RemoveCursor(ByVal iPane As Integer)
            If Me.m_bShowCursor(iPane) Then
                Me.GetPane(iPane).CurveList.Remove(Me.m_liCursor(iPane))
                Me.m_liCursor(iPane) = Nothing
                Me.m_zgc.Invalidate()
            End If
        End Sub

        Protected Sub SetCursor(ByVal iPane As Integer)
            If Me.m_bShowCursor(iPane) Then

                Dim gp As GraphPane = Me.GetPane(iPane)
                Dim dYMin As Double = gp.YAxis.Scale.Min
                Dim dYMax As Double = gp.YAxis.Scale.Max

                ' Clean up if necessary
                If Me.m_liCursor(iPane) IsNot Nothing Then Me.RemoveCursor(iPane)
                ' Set cursor
                Me.m_liCursor(iPane) = New LineItem(My.Resources.GENERIC_TEXT_CURSOR, _
                        New Double() {Me.m_sCursorPos(iPane), Me.m_sCursorPos(iPane)}, _
                        New Double() {dYMin, dYMax}, _
                        Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT), _
                        SymbolType.None, _
                        3)

                gp.CurveList.Add(Me.m_liCursor(iPane))
                Me.m_zgc.Invalidate()
            End If
        End Sub

#End Region ' Cursor

#Region " Context Menu "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Handler to extend the context menu for the wrapped ZedGraph.
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="menuStrip"></param>
        ''' <param name="mousePt"></param>
        ''' <param name="objState"></param>
        ''' -----------------------------------------------------------------------
        Protected Sub OnBuildContextMenu(ByVal control As ZedGraphControl, _
                                         ByVal menuStrip As ContextMenuStrip, _
                                         ByVal mousePt As Point, _
                                         ByVal objState As ZedGraphControl.ContextMenuObjectState)

            Dim bLegendsVisible As Boolean = False

            ' create a new menu item
            Dim item As ToolStripMenuItem = New ToolStripMenuItem()
            ' This is the user-defined Tag so you can find this menu item later if necessary
            item.Name = "Extract_CSV_Data"
            ' This is the text that will show up in the menu
            item.Text = My.Resources.MENU_EXTRACT_TO_CSV
            ' Add a handler that will respond when that menu item is selected
            AddHandler item.Click, AddressOf OnExtractToCSV
            ' Add the menu item to the menu
            menuStrip.Items.Add(item)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Sub OnExtractToCSV(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.ExtractDataToCSV()
        End Sub

#End Region ' Context menu

#Region " Hover menu handling "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, trapped to show the hover menu when the mouse enters
        ''' the zed graph area.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnMouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.ShowHover(True)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, trapped to hide the hover menu when the mouse enters
        ''' the zed graph area.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnMouseLeave(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.ShowHover(False)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Evaluate the hover menu state anew.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateHoverMenuState()
            Me.ShowHover(Me.IsMouseOverGraph() Or Me.IsMouseOverPanel())
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Show or hide the hover menu.
        ''' </summary>
        ''' <param name="bShow">Flag stating whether the hover menu should be 
        ''' shown (True) or hidden (False).</param>
        ''' -------------------------------------------------------------------
        Private Sub ShowHover(ByVal bShow As Boolean)
            Me.m_hovermenu.Visible = (bShow Or IsMouseOverPanel()) And Me.m_bShowHoverMenu
            Dim ptHover As New Point(4, Me.m_zgc.Height - Me.m_hovermenu.Height - 8)
            Me.m_hovermenu.Location = Me.m_zgc.PointToScreen(ptHover)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns whether the mouse is over the hover menu.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function IsMouseOverPanel() As Boolean
            Dim pt As Point = Me.m_hovermenu.PointToClient(Form.MousePosition)
            Return Me.m_hovermenu.ClientRectangle.Contains(pt)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns whether the mouse is over the zed graph.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function IsMouseOverGraph() As Boolean
            Dim pt As Point = Me.m_zgc.PointToClient(Form.MousePosition)
            Return Me.m_zgc.ClientRectangle.Contains(pt)
        End Function

        ''' <summary>Cross-threading delegate.</summary>
        ''' <param name="cmd"></param>
        Private Delegate Sub OnHoverMenuCommandCallbackDelegate(ByVal cmd As ucZedGraphHoverMenu.eCommandTypes)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for hover menu events.
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' -------------------------------------------------------------------
        Private Sub OnHoverMenuCommandCallback(ByVal cmd As ucZedGraphHoverMenu.eCommandTypes)

            Dim gp As GraphPane = Nothing
            Dim zs As ZoomState = Nothing
            Dim sValueAvg As Single = 0.0

            If Me.m_zgc.InvokeRequired Then
                Me.m_zgc.Invoke(New OnHoverMenuCommandCallbackDelegate(AddressOf OnHoverMenuCommandCallback), New Object() {cmd})
                Return
            End If

            For iPane As Integer = 1 To Me.NumPanes
                ' Get pane
                gp = Me.GetPane(iPane)
                ' Get pane avg
                sValueAvg = Me.GetValueAvg(iPane)

                ' Manually zoom in, place zoom in zoom stack. Zoom out means recalling zoom positions
                Select Case cmd
                    Case ucZedGraphHoverMenu.eCommandTypes.ZoomIn
                        zs = New ZoomState(gp, ZoomState.StateType.Zoom)
                        gp.ZoomStack.Add(zs)
                        gp.YAxis.Scale.Max -= (gp.YAxis.Scale.Max - sValueAvg) / 4
                        gp.YAxis.Scale.Min += (sValueAvg - gp.YAxis.Scale.Min) / 4
                    Case ucZedGraphHoverMenu.eCommandTypes.ZoomOut
                        zs = gp.ZoomStack.Pop(gp)
                        If zs IsNot Nothing Then zs.ApplyState(gp)
                End Select
            Next

            Me.m_zgc.Refresh()
            Me.UpdateHoverMenuState()

        End Sub

#End Region ' Hover menu handling

#End Region ' Internal bits

    End Class

End Namespace ' Controls
