'==============================================================================
'
' $Log: ZedGraphHelper.vb,v $
' Revision 1.16  2009/05/18 02:11:28  jeroens
' Left ToDo
'
' Revision 1.15  2009/05/13 12:51:45  jeroens
' Made inheritable
'
' Revision 1.14  2009/05/11 01:51:04  jeroens
' Renamed command classes
'
' Revision 1.13  2009/04/08 17:40:29  jeroens
' Cursor properly set and removed in ShowCursor
'
' Revision 1.12  2009/04/07 20:55:53  jeroens
' FIxed time series line style
' Added line items directly
'
' Revision 1.11  2009/04/07 20:01:14  jeroens
' Added preformatted line support
' Changed constructor; need to use Attach and Detach explicitly
'
' Revision 1.10  2009/04/03 18:21:24  jeroens
' Added Attach, Detach
'
' Revision 1.9  2009/03/24 13:44:30  jeroens
' Tick tock
'
' Revision 1.8  2009/03/23 02:43:43  jeroens
' Added option to show data under cursor in tooltip
'
' Revision 1.7  2009/02/23 03:21:39  jeroens
' Cleaned
' Left ToDo
'
' Revision 1.6  2008/12/02 20:45:35  sherman
' Fixed autoscale bugs
'
' Revision 1.5  2008/11/29 19:00:11  sherman
' Updated bugs and rescaling in RunEcosim plot
'
' Revision 1.4  2008/11/11 00:52:24  joeh
' Set plot type default to relative and scale default to auto
'
' Revision 1.3  2008/11/10 05:34:37  jeroens
' Renamed file command
'
' Revision 1.2  2008/10/08 17:44:58  jeroens
' Bells and whistles
'
' Revision 1.1  2008/09/26 07:31:20  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

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

#End Region ' Imports

Namespace Controls

    ' ToDo_JS: 22Feb09 - Add support for displaying units in axis labels (similar to EwECells)

    ''' =======================================================================
    ''' <summary>
    ''' Helper class, wraps a <see cref="ZedGraph">ZedGraph</see> graph control
    ''' to standardize look and feel. Additionally, this class implements 
    ''' generic cursor behaviour on the graph, and provides standardized data 
    ''' export.
    ''' </summary>
    ''' =======================================================================
    Public Class ZedGraphHelper

#Region " Private vars "

        ''' <summary>Wrapped ZedGraph control.</summary>
        Private m_zgc As ZedGraphControl = Nothing
        ''' <summary>Core to accompany this monster.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>Number of panels wanted in the zed graph</summary>
        Private m_nPanels As Integer = 1
        ''' <summary>Style! Styyyyyle, baby!</summary>
        Private m_sg As StyleGuide = Nothing
        ''' <summary>Registered lines representing EwE groups.</summary>
        Private m_dtGroupLines As New Dictionary(Of LineItem, Integer)

        Private m_bShowCursor() As Boolean
        Private m_sCursorPos() As Single
        Private m_liCursor() As LineItem

        ''' <summary>To set the max and min auto options.</summary>
        Public Enum ScaleOptions
            MaxOnly
            MinOnly
            Both
            None
        End Enum

#End Region ' Private vars

#Region " Public enums "

        Public Enum eCurveTypes As Integer
            EcosimOutput
            TimeSeries
        End Enum

#End Region ' Public enums

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

        Public Overridable Sub Attach(ByVal core As cCore, ByVal zgc As ZedGraphControl, Optional ByVal iNumPanels As Integer = 1)

            If Me.m_zgc IsNot Nothing Then Me.Detach()

            Me.m_core = core
            Me.m_zgc = zgc
            Me.m_sg = StyleGuide.GetInstance()
            Me.m_nPanels = iNumPanels

            While Me.m_zgc.MasterPane.PaneList.Count < iNumPanels
                Me.m_zgc.MasterPane.PaneList.Add(New GraphPane())
            End While

            While Me.m_zgc.MasterPane.PaneList.Count > iNumPanels
                Me.m_zgc.MasterPane.PaneList.RemoveAt(iNumPanels)
            End While

            ReDim Me.m_bShowCursor(iNumPanels)
            ReDim Me.m_liCursor(iNumPanels)
            ReDim Me.m_sCursorPos(iNumPanels)

            AddHandler Me.m_zgc.MouseDownEvent, AddressOf OnMouseDownEvent
            AddHandler Me.m_zgc.MouseMoveEvent, AddressOf OnMouseMoveEvent
            AddHandler Me.m_zgc.MouseUpEvent, AddressOf OnMouseUpEvent
            AddHandler Me.m_zgc.ContextMenuBuilder, AddressOf OnBuildContextMenu
            AddHandler Me.m_zgc.PointValueEvent, AddressOf OnPointValueEvent

            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            ' Configure graph control
            Me.InitStyle()

        End Sub

        Public Overridable Sub Detach()

            If Me.m_zgc Is Nothing Then Return

            RemoveHandler Me.m_zgc.MouseDownEvent, AddressOf OnMouseDownEvent
            RemoveHandler Me.m_zgc.MouseMoveEvent, AddressOf OnMouseMoveEvent
            RemoveHandler Me.m_zgc.MouseUpEvent, AddressOf OnMouseUpEvent
            RemoveHandler Me.m_zgc.ContextMenuBuilder, AddressOf OnBuildContextMenu
            RemoveHandler Me.m_zgc.PointValueEvent, AddressOf OnPointValueEvent

            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            Me.m_dtGroupLines.Clear()
            Me.m_sg = Nothing
            Me.m_zgc = Nothing
            Me.m_core = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of panels in the <see cref="ZedGraph">graph</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumPanes() As Integer
            Get
                Return Me.m_nPanels
            End Get
        End Property

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

            Me.ConfigurePane(strTitle, strXAxisLabel, strYAxisLabel, bShowLegend, legendPos, iPane)

            Dim gp As GraphPane = Me.GetPane(iPane)
            With gp

                .XAxis.Scale.Min = dXAxisMin
                If dXAxisMin <> dXAxisMax Then .XAxis.Scale.Max = dXAxisMax

                .YAxis.Scale.Min = dYAxisMin
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

            Dim gp As GraphPane = Me.GetPane(iPane)
            With gp

                ' Set title
                .Title.Text = strTitle
                .Title.IsVisible = Not String.IsNullOrEmpty(strTitle)

                ' Configure axis
                .XAxis.Title.Text = strXAxisLabel
                .XAxis.Title.IsVisible = Not String.IsNullOrEmpty(strXAxisLabel)
                .XAxis.MinorTic.IsAllTics = False
                .XAxis.MinorTic.IsOpposite = False
                .XAxis.MajorTic.IsOpposite = False

                .YAxis.Title.Text = strYAxisLabel
                .YAxis.Title.IsVisible = Not String.IsNullOrEmpty(strYAxisLabel)
                .YAxis.MinorTic.IsAllTics = False
                .YAxis.MinorTic.IsOpposite = False
                .YAxis.MajorTic.IsOpposite = False

                .Legend.IsVisible = bShowLegend
                .Legend.Position = legendPos

                .Border.IsVisible = False
                .Chart.Border.IsVisible = True
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
        Public Overridable Sub PlotLines(ByVal lines As List(Of LineItem), _
                             Optional ByVal iPane As Integer = 1, _
                             Optional ByVal bRescale As Boolean = True, _
                             Optional ByVal bClear As Boolean = True)
            Try

                With Me.GetPane(iPane)

                    ' ToDo_JS: auto-unregister group lines
                    If bClear Then .CurveList.Clear()

                    If lines IsNot Nothing Then
                        For Each li As LineItem In lines
                            ' If not provided, use pane title
                            If String.IsNullOrEmpty(li.Label.Text) Then li.Label.Text = .Title.Text
                            ' Add the curve
                            .CurveList.Add(li)
                        Next li

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
            Next

            ' Recalc axis
            Me.m_zgc.AxisChange()

            ' Restore cursors
            For iPane = iMin To iMax
                Me.ShowCursor(iPane) = abCursor(iPane)
            Next

            Me.Redraw()

        End Sub

        Public Property AutoscalePane(Optional ByVal iPane As Integer = 1) As Boolean
            Get
                With Me.GetPane(iPane).YAxis.Scale
                    Return .MaxAuto And .MinAuto
                End With
            End Get
            Set(ByVal bAutoscale As Boolean)
                With Me.GetPane(iPane).YAxis.Scale
                    'If bAutoscale <> .MinAuto And bAutoscale <> .MaxAuto Then
                    .MinAuto = bAutoscale
                    .MaxAuto = bAutoscale
                    RescaleAndRedraw(iPane)
                    'End If
                End With
            End Set
        End Property

        Public Property AutoScaleOption(Optional ByVal iPane As Integer = 1) As ScaleOptions
            Get
                With Me.GetPane(iPane).YAxis.Scale
                    If .MinAuto And .MaxAuto Then
                        Return ScaleOptions.Both
                    ElseIf .MaxAuto And Not .MinAuto Then
                        Return ScaleOptions.MaxOnly
                    ElseIf Not .MaxAuto And .MinAuto Then
                        Return ScaleOptions.MinOnly
                    ElseIf Not .MaxAuto And Not .MinAuto Then
                        Return ScaleOptions.None
                    End If
                    Return ScaleOptions.None
                End With
            End Get
            Set(ByVal value As ScaleOptions)
                With Me.GetPane(iPane).YAxis.Scale
                    Select Case value
                        Case ScaleOptions.Both
                            .MinAuto = True
                            .MaxAuto = True
                        Case ScaleOptions.MaxOnly
                            .MaxAuto = True
                            .MinAuto = False
                        Case ScaleOptions.MinOnly
                            .MaxAuto = False
                            .MinAuto = True
                        Case ScaleOptions.None
                            .MinAuto = False
                            .MaxAuto = False
                    End Select
                    RescaleAndRedraw(iPane)
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

        Public Property YScaleMax(Optional ByVal iPane As Integer = 1) As Double
            Get
                Return Me.GetPane(iPane).YAxis.Scale.Max
            End Get
            Set(ByVal value As Double)
                Me.GetPane(iPane).YAxis.Scale.Max = value
                RescaleAndRedraw(iPane)
            End Set
        End Property

        Public WriteOnly Property AllowZoom() As Boolean
            Set(ByVal value As Boolean)
                Me.m_zgc.IsZoomOnMouseCenter = value
                Me.m_zgc.IsEnableVZoom = value
                Me.m_zgc.IsEnableHZoom = value
                Me.m_zgc.IsEnableWheelZoom = value
                Me.m_zgc.IsEnableZoom = value
            End Set
        End Property

        Public WriteOnly Property AllowPan() As Boolean
            Set(ByVal value As Boolean)
                Me.m_zgc.IsEnableVPan = value
                Me.m_zgc.IsEnableHPan = value
            End Set
        End Property

        Public WriteOnly Property AllowEdit() As Boolean
            Set(ByVal value As Boolean)
                Me.m_zgc.IsEnableHEdit = value
                Me.m_zgc.IsEnableVEdit = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an EwE-styled line
        ''' </summary>
        ''' <param name="curveType"></param>
        ''' <param name="iGroup"></param>
        ''' <param name="ppl"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function CreateLineItem(ByVal curveType As eCurveTypes, _
                                        ByVal iGroup As Integer, _
                                        ByVal ppl As PointPairList) As LineItem

            Dim group As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iGroup)
            Return Me.CreateLineItem(group.Name, curveType, Me.m_sg.GroupColor(Me.m_core, group.Index), ppl)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strName"></param>
        ''' <param name="curveType"></param>
        ''' <param name="clr"></param>
        ''' <param name="ppl"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function CreateLineItem(ByVal strName As String, _
                                        ByVal curveType As eCurveTypes, _
                                        ByVal clr As Color, _
                                        ByVal ppl As PointPairList) As LineItem

            Dim li As LineItem = Nothing

            Select Case curveType

                Case eCurveTypes.TimeSeries
                    li = New ZedGraph.LineItem(strName, ppl, clr, SymbolType.Circle, 1)

                    li.Line.Color = Color.Transparent
                    li.Line.IsVisible = False

                    ' ToDo_JS: obtain symbol size from style guide
                    li.Symbol.Border.Color = clr

                    li.Symbol.Size = 4
                    li.Symbol.Fill.Color = Color.Transparent
                    li.Symbol.Border.IsVisible = True
                    li.Symbol.Fill.IsVisible = False
                    li.Symbol.IsVisible = True


                Case eCurveTypes.EcosimOutput
                    li = New ZedGraph.LineItem(strName, ppl, clr, SymbolType.None, 1)

            End Select

            Return li

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
            Return String.Format("{0}: ({1}, {2})", curve.Label.Text, Me.m_sg.FormatNumber(pp.X), Me.m_sg.FormatNumber(pp.Y))
        End Function

#End Region ' Tooltip

#Region " Line colour management "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Register a <see cref="LineItem">line</see> currently existing in the 
        ''' wrapped graph, and connect it to an <see cref="cEcoPathGroupInput">Ecopath group</see>. 
        ''' The line colour will be coloured according to the <see cref="StyleGuide.GroupColor">colour</see> 
        ''' of the group, and colour changes will be automagically applied.
        ''' </summary>
        ''' <param name="line">The <see cref="LineItem">line</see> to register.</param>
        ''' <param name="iGroup">The <see cref="cEcoPathGroupInput.Index">Index</see> of
        ''' the group to connect this line to.</param>
        ''' <remarks>
        ''' <para>A line that is registered should be properly unregister with 
        ''' <see cref="UnregisterGroupLine">UnregisterGroupLine</see>.</para>
        ''' <para>Note that the line should already exist in the graph. This is 
        ''' not enforced, but it makes sense, doesn't it?</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub RegisterGroupLine(ByVal line As LineItem, ByVal iGroup As Integer)
            Me.m_dtGroupLines(line) = iGroup
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Unregister a <see cref="LineItem">line</see> currently existing in the 
        ''' wrapped graph from the automatic group colour management.
        ''' </summary>
        ''' <param name="line">The <see cref="LineItem">line</see> to unregister.</param>
        ''' <remarks>
        ''' Lines should be registered with <see cref="RegisterGroupLine">RegisterGroupLine</see>
        ''' first.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub UnregisterGroupLine(ByVal line As LineItem)
            Me.m_dtGroupLines.Remove(line)
        End Sub

#End Region ' Line colour management

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
        Public Event OnCursorPos(ByVal zgh As ZedGraphHelper, ByVal iPane As Integer, ByVal sPos As Single)

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

            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim sw As StreamWriter = Nothing

            'ToDo_JS: globalize this
            cmdFS.Invoke("csv files (*.csv)|*.csv|text files (*.txt)|*.txt", 0)

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
            Dim sb As New StringBuilder
            Dim sbX As StringBuilder = Nothing
            Dim sbY As StringBuilder = Nothing
            Dim gp As GraphPane = Nothing

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
                            sbX = New StringBuilder("X")
                            sbY = New StringBuilder("Y")
                            For i As Integer = 0 To ci.NPts - 1
                                sbX.Append(", ")
                                sbX.Append(ci.Points(i).X.ToString)

                                sbY.Append(", ")
                                sbY.Append(ci.Points(i).Y.ToString)
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

#End Region ' Public interfaces

#Region " Events "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="changeType"></param>
        ''' -----------------------------------------------------------------------
        Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType)
            If ((changeType And StyleGuide.eChangeType.Fonts) = StyleGuide.eChangeType.Fonts) Then
                Me.InitStyle()
            ElseIf changeType = StyleGuide.eChangeType.Colours Then
                Me.InitColors()
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

        Private Sub InitColors()
            For iPane As Integer = 1 To Me.m_nPanels
                With Me.GetPane(iPane)
                    .Chart.Fill = New Fill(Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PLOT_BACKGROUND))
                    .Fill = New Fill(Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PLOT_BACKGROUND))

                    Me.RemoveCursor(iPane)
                    Me.SetCursor(iPane)

                End With
            Next iPane
        End Sub

        Protected Overridable Sub InitStyle()

            For iPane As Integer = 1 To Me.m_nPanels
                With Me.GetPane(iPane)
                    .IsFontsScaled = False

                    .Title.FontSpec.Family = Me.m_sg.GraphFontFamilyName
                    .Title.FontSpec.Size = Me.m_sg.GraphCaptionFontSize
                    .Title.FontSpec.IsBold = ((Me.m_sg.GraphCaptionFontStyle And FontStyle.Bold) = FontStyle.Bold)
                    .Title.FontSpec.IsItalic = ((Me.m_sg.GraphCaptionFontStyle And FontStyle.Italic) = FontStyle.Italic)
                    .Title.FontSpec.IsUnderline = ((Me.m_sg.GraphCaptionFontStyle And FontStyle.Underline) = FontStyle.Underline)

                    .XAxis.Title.FontSpec.Family = Me.m_sg.GraphFontFamilyName
                    .YAxis.Title.FontSpec.Family = Me.m_sg.GraphFontFamilyName
                    .XAxis.Title.FontSpec.Size = Me.m_sg.GraphAxisLabelFontSize
                    .YAxis.Title.FontSpec.Size = Me.m_sg.GraphAxisLabelFontSize
                    .XAxis.Title.FontSpec.IsBold = ((Me.m_sg.GraphAxisLabelFontStyle And FontStyle.Bold) = FontStyle.Bold)
                    .YAxis.Title.FontSpec.IsBold = ((Me.m_sg.GraphAxisLabelFontStyle And FontStyle.Bold) = FontStyle.Bold)
                    .XAxis.Title.FontSpec.IsItalic = ((Me.m_sg.GraphAxisLabelFontStyle And FontStyle.Italic) = FontStyle.Italic)
                    .YAxis.Title.FontSpec.IsItalic = ((Me.m_sg.GraphAxisLabelFontStyle And FontStyle.Italic) = FontStyle.Italic)
                    .XAxis.Title.FontSpec.IsUnderline = ((Me.m_sg.GraphAxisLabelFontStyle And FontStyle.Underline) = FontStyle.Underline)
                    .YAxis.Title.FontSpec.IsUnderline = ((Me.m_sg.GraphAxisLabelFontStyle And FontStyle.Underline) = FontStyle.Underline)

                    .XAxis.Scale.FontSpec.Family = Me.m_sg.GraphFontFamilyName
                    .YAxis.Scale.FontSpec.Family = Me.m_sg.GraphFontFamilyName
                    .XAxis.Scale.FontSpec.Size = Me.m_sg.GraphAxisScaleFontSize
                    .YAxis.Scale.FontSpec.Size = Me.m_sg.GraphAxisScaleFontSize
                    .XAxis.Scale.FontSpec.IsBold = ((Me.m_sg.GraphAxisScaleFontStyle And FontStyle.Bold) = FontStyle.Bold)
                    .YAxis.Scale.FontSpec.IsBold = ((Me.m_sg.GraphAxisScaleFontStyle And FontStyle.Bold) = FontStyle.Bold)
                    .XAxis.Scale.FontSpec.IsItalic = ((Me.m_sg.GraphAxisScaleFontStyle And FontStyle.Italic) = FontStyle.Italic)
                    .YAxis.Scale.FontSpec.IsItalic = ((Me.m_sg.GraphAxisScaleFontStyle And FontStyle.Italic) = FontStyle.Italic)
                    .XAxis.Scale.FontSpec.IsUnderline = ((Me.m_sg.GraphAxisScaleFontStyle And FontStyle.Underline) = FontStyle.Underline)
                    .YAxis.Scale.FontSpec.IsUnderline = ((Me.m_sg.GraphAxisScaleFontStyle And FontStyle.Underline) = FontStyle.Underline)

                    .Legend.FontSpec.Family = Me.m_sg.GraphFontFamilyName
                    .Legend.FontSpec.Size = Me.m_sg.GraphLegendFontSize
                    .Legend.FontSpec.IsBold = ((Me.m_sg.GraphLegendFontStyle And FontStyle.Bold) = FontStyle.Bold)
                    .Legend.FontSpec.IsItalic = ((Me.m_sg.GraphLegendFontStyle And FontStyle.Italic) = FontStyle.Italic)
                    .Legend.FontSpec.IsUnderline = ((Me.m_sg.GraphLegendFontStyle And FontStyle.Underline) = FontStyle.Underline)

                End With
            Next

            ' Refresh all registered lines
            For Each l As LineItem In Me.m_dtGroupLines.Keys
                l.Color = Me.m_sg.GroupColor(Me.m_core, Me.m_dtGroupLines(l))
            Next


            With Me.m_zgc.MasterPane
                .Border.IsVisible = False
                .Title.FontSpec.Family = Me.m_sg.GraphFontFamilyName
                .Title.FontSpec.Size = Me.m_sg.GraphCaptionFontSize
                .Title.FontSpec.IsBold = ((Me.m_sg.GraphCaptionFontStyle And FontStyle.Bold) = FontStyle.Bold)
                .Title.FontSpec.IsItalic = ((Me.m_sg.GraphCaptionFontStyle And FontStyle.Italic) = FontStyle.Italic)
                .Title.FontSpec.IsUnderline = ((Me.m_sg.GraphCaptionFontStyle And FontStyle.Underline) = FontStyle.Underline)

                Using g As Graphics = Me.m_zgc.CreateGraphics()
                    .SetLayout(g, PaneLayout.SquareColPreferred)
                End Using
            End With

            ' Redraw at your convenience
            Me.m_zgc.Invalidate()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a graph pane.
        ''' </summary>
        ''' <param name="iPane">The one-based index of the pane to return. This 
        ''' index should be between 1 and <see cref="NumPanes">NumPanes</see>.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Function GetPane(ByVal iPane As Integer) As ZedGraph.GraphPane

            Dim pane As GraphPane = Nothing

            If Me.m_nPanels = 1 Then pane = Me.m_zgc.GraphPane
            pane = Me.m_zgc.MasterPane.PaneList(iPane - 1)

            Debug.Assert(pane IsNot Nothing, "ZedGraphHelper already disconnected")

            Return pane

        End Function

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
                        Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.HIGHLIGHT), _
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

            'ToDo_JS: globalize this

            ' create a new menu item
            Dim item As ToolStripMenuItem = New ToolStripMenuItem()
            ' This is the user-defined Tag so you can find this menu item later if necessary
            item.Name = "Extract_CSV_Data"
            ' This is the text that will show up in the menu
            item.Text = "E&xtract to CSV..."
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

#End Region ' Internal bits

    End Class

End Namespace ' Controls
