'==============================================================================
'
' $Log: ZedGraphHelper.vb,v $
' Revision 1.1  2008/09/26 07:31:20  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/09/19 16:05:01  jeroens
' Fixed issue 542
'
' Revision 1.9  2008/09/09 14:44:57  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.8  2008/09/02 14:46:45  jeroens
' Commented public interfaces
' ExtractData uses stringbuilder
'
' Revision 1.7  2008/09/02 14:05:42  jeroens
' Cursor handled properly per pane
' Zoom fixed when cursor disabled
'
' Revision 1.6  2008/08/26 18:21:27  jeroens
' Fixed cursor logic
'
' Revision 1.5  2008/08/18 15:48:32  jeroens
' Preparing for cursor
' ToDo: rescale cursor to Y axis (e.g. prevent cursor from affecting Y scale)
'
' Revision 1.4  2008/08/16 00:00:30  sherman
' Hooked background colors to style guide
'
' Revision 1.3  2008/08/15 21:24:46  jeroens
' Renamed 'GraphsAndFiguresd' style to 'Fonts'
'
' Revision 1.2  2008/07/18 17:50:36  jeroens
' Added line colour management
'
' Revision 1.1  2008/06/01 23:45:11  jeroens
' Separated from Scientific Interface
'
' Revision 1.7  2008/04/07 18:09:16  jeroens
' Labels contained in quotes in CSV output
'
' Revision 1.6  2008/04/07 17:57:20  jeroens
' Every ZGH extends the context menu
'
' Revision 1.5  2008/03/03 16:15:54  joeb
' Error handling in PlotLines
'
' Revision 1.4  2007/12/22 22:54:24  jeroens
' * Added legend configuration
'
' Revision 1.3  2007/12/22 16:49:07  jeroens
' * Connected to StyleGuide
' + Attempting to standardize
'
' Revision 1.2  2007/11/24 16:46:51  jeroens
' * Fixed compiler warnings
'
' Revision 1.1  2007/11/21 20:17:07  sherman
' Added Export to CSV to zedgraph.
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

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Wrap and initialize a <see cref="ZedGraph">ZedGraph</see> control.
        ''' </summary>
        ''' <param name="zgc">The <see cref="ZedGraph">ZedGraph</see> to wrap.</param>
        ''' <param name="iNumPanels">The number of panels to initialize on the graph.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal zgc As ZedGraphControl, Optional ByVal iNumPanels As Integer = 1)

            Me.m_zgc = zgc
            Me.m_sg = StyleGuide.GetInstance()
            Me.m_nPanels = iNumPanels

            ReDim Me.m_bShowCursor(iNumPanels)
            ReDim Me.m_liCursor(iNumPanels)
            ReDim Me.m_sCursorPos(iNumPanels)

            AddHandler Me.m_zgc.MouseDownEvent, AddressOf OnMouseDownEvent
            AddHandler Me.m_zgc.MouseMoveEvent, AddressOf OnMouseMoveEvent
            AddHandler Me.m_zgc.MouseUpEvent, AddressOf OnMouseUpEvent

            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            AddHandler Me.m_zgc.ContextMenuBuilder, AddressOf MyContextMenuBuilder

            ' Configure graph control
            Me.InitStyle()

        End Sub

        Protected Overrides Sub Finalize()

            RemoveHandler Me.m_zgc.MouseDownEvent, AddressOf OnMouseDownEvent
            RemoveHandler Me.m_zgc.MouseMoveEvent, AddressOf OnMouseMoveEvent
            RemoveHandler Me.m_zgc.MouseUpEvent, AddressOf OnMouseUpEvent

            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            RemoveHandler Me.m_zgc.ContextMenuBuilder, AddressOf MyContextMenuBuilder

            Me.m_dtGroupLines.Clear()
            Me.m_sg = Nothing
            Me.m_zgc = Nothing
            MyBase.Finalize()
        End Sub

#End Region ' Construction / destruction

#Region " Public interfaces "

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
        Public Function ConfigurePane(ByVal strTitle As String, _
            ByVal strXAxisLabel As String, ByVal dXAxisMin As Double, ByVal dXAxisMax As Double, _
            ByVal strYAxisLabel As String, ByVal dYAxisMin As Double, ByVal dYAxisMax As Double, _
            ByVal bShowLegend As Boolean, Optional ByVal legendPos As LegendPos = LegendPos.TopCenter, _
            Optional ByVal iPane As Integer = 0) As GraphPane

            Dim gp As GraphPane = Me.GetPane(iPane)
            With gp
                ' Set title
                .Title.Text = strTitle
                .Title.IsVisible = Not String.IsNullOrEmpty(strTitle)

                ' Configure axis
                .XAxis.Title.Text = strXAxisLabel
                .XAxis.Title.IsVisible = Not String.IsNullOrEmpty(strXAxisLabel)
                .XAxis.Scale.Min = dXAxisMin
                .XAxis.Scale.Max = dXAxisMax

                .YAxis.Title.Text = strYAxisLabel
                .YAxis.Title.IsVisible = Not String.IsNullOrEmpty(strYAxisLabel)
                .YAxis.Scale.Min = dYAxisMin
                .YAxis.Scale.Max = dYAxisMax

                .Legend.IsVisible = bShowLegend
                .Legend.Position = legendPos

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
        Public Function ConfigurePane(ByVal strTitle As String, _
             ByVal strXAxisLabel As String, ByVal strYAxisLabel As String, _
             ByVal bShowLegend As Boolean, Optional ByVal legendPos As LegendPos = LegendPos.TopCenter, _
             Optional ByVal iPane As Integer = 0) As GraphPane

            Dim gp As GraphPane = Me.GetPane(iPane)
            With gp
                ' Set title
                .Title.Text = strTitle
                .Title.IsVisible = Not String.IsNullOrEmpty(strTitle)

                ' Configure axis
                .XAxis.Title.Text = strXAxisLabel
                .XAxis.Title.IsVisible = Not String.IsNullOrEmpty(strXAxisLabel)

                .YAxis.Title.Text = strYAxisLabel
                .YAxis.Title.IsVisible = Not String.IsNullOrEmpty(strYAxisLabel)

                .Legend.IsVisible = bShowLegend
                .Legend.Position = legendPos

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
        ''' <param name="bForceRedraw">Flag stating whether the graph needs to be
        ''' rescaled and redrawn (optional).</param>
        ''' <remarks>Note that this method clears out all lines existing in the
        ''' indicated panel.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub PlotLines(ByVal lines As List(Of LineItem), Optional ByVal iPane As Integer = 1, Optional ByVal bForceRedraw As Boolean = True)
            Try

                With Me.GetPane(iPane)

                    .CurveList.Clear()
                    For Each line As LineItem In lines
                        .AddCurve(line.Label.Text, line.Points, line.Color, line.Symbol.Type)
                    Next
                End With

                If bForceRedraw Then Me.RescaleAndRedraw()

            Catch ex As Exception
                EwECore.cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".PlotLines() " & ex.Message, ex)
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Totally redraw the wrapped ZedGraph by recalculating the axis and 
        ''' invalidating all panels.
        ''' </summary>
        ''' <remarks>When using cursors please use this method to rescale the
        ''' graph axis.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub RescaleAndRedraw(Optional ByVal iPane As Integer = -1)

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
            Me.m_zgc.Invalidate()

            ' Restore cursors
            For iPane = iMin To iMax
                Me.ShowCursor(iPane) = abCursor(iPane)
            Next

        End Sub

        Public Property AutoscalePane(Optional ByVal iPane As Integer = 1) As Boolean
            Get
                With Me.GetPane(iPane).YAxis.Scale
                    Return .MaxAuto And .MinAuto
                End With
            End Get
            Set(ByVal bAutoscale As Boolean)
                With Me.GetPane(iPane).YAxis.Scale
                    If bAutoscale <> .MinAuto And bAutoscale <> .MaxAuto Then
                        .MinAuto = bAutoscale
                        .MaxAuto = bAutoscale
                        RescaleAndRedraw(iPane)
                    End If
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
                    Me.m_bShowCursor(iPane) = value
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

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFS As FileSaveCommand = DirectCast(cmdh.GetCommand(FileSaveCommand.COMMAND_NAME), FileSaveCommand)
            Dim sw As StreamWriter = Nothing

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
            'Dim str As String = ""
            'Dim tempX, tempY As String

            ' Safety first
            If z IsNot Nothing Then
                ' Each Zedgraph Plane
                For Each p As ZedGraph.PaneBase In z.MasterPane.PaneList
                    ' Check if it's a graphpane
                    If TypeOf (p) Is GraphPane Then
                        gp = DirectCast(p, GraphPane)

                        ' Print the title
                        sb.AppendLine(String.Format("{0}{1}{0}", Chr(34), p.Title.Text))
                        'str = str + """" + p.Title.Text + """" + vbCrLf
                        For Each ci As CurveItem In gp.CurveList
                            ' Print Item
                            sb.AppendLine(String.Format("{0}{1}{0}", Chr(34), ci.Label.Text))
                            'str = str + """" + ci.Label.Text + """" + vbCrLf
                            sbX = New StringBuilder("X")
                            'tempX = "X, "
                            sbY = New StringBuilder("Y")
                            'tempY = "Y, "
                            For i As Integer = 0 To ci.NPts - 1
                                sbX.Append(", ")
                                sbX.Append(ci.Points(i).X.ToString)
                                'tempX = tempX + ci.Points(i).X.ToString + ", "

                                sbY.Append(", ")
                                sbY.Append(ci.Points(i).Y.ToString)
                                'tempY = tempY + ci.Points(i).Y.ToString + ", "
                            Next

                            sb.AppendLine(sbX.ToString())
                            sb.AppendLine(sbY.ToString())
                            'str = str + tempX + vbCrLf + tempY + vbCrLf
                        Next
                    End If
                Next
            End If

            Return sb.ToString()
            'Return str
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
            Dim iPanel As Integer = GetPaneAtPoint(args.Location)
            If iPanel > -1 Then
                If Me.m_bShowCursor(iPanel) Then
                    Me.CursorPos = GraphToScale(New PointF(args.Location.X, args.Location.Y)).X
                    Return True
                End If
            End If
            Return False
        End Function

        Private Function OnMouseMoveEvent(ByVal zg As ZedGraphControl, ByVal args As MouseEventArgs) As Boolean
            Dim iPanel As Integer = GetPaneAtPoint(args.Location)
            If iPanel > -1 Then
                If Me.m_bShowCursor(iPanel) Then
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

        Private Sub InitStyle()

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

            ' Redraw at your convenience
            Me.m_zgc.Invalidate()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a graph pane.
        ''' </summary>
        ''' <param name="iPane">The index of the pane to return. This index
        ''' should be between 1 and <see cref="NumPanes">NumPanes</see>.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function GetPane(ByVal iPane As Integer) As ZedGraph.GraphPane
            If Me.m_nPanels = 1 Then Return Me.m_zgc.GraphPane
            Return Me.m_zgc.MasterPane.PaneList(iPane - 1)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the graph pane located at a given point.
        ''' </summary>
        ''' <param name="pt">The point to test.</param>
        ''' <returns>Index of a pane, or -1 if no pane was found at the given
        ''' location.</returns>
        ''' -------------------------------------------------------------------
        Private Function GetPaneAtPoint(ByVal pt As Point) As Integer
            For i As Integer = 1 To Me.m_nPanels
                Dim gp As GraphPane = Me.GetPane(i)
                If gp.Rect.Contains(pt) Then Return i
            Next
            Return -1
        End Function

#Region " Cursor "

        Private Function GraphToScale(ByVal ptf As PointF) As PointF
            Dim myPane As GraphPane = Me.m_zgc.GraphPane
            Dim dX As Double = 0.0
            Dim dY As Double = 0.0
            myPane.ReverseTransform(ptf, dX, dY)
            Return New PointF(CSng(dX), CSng(dY))
        End Function

        Private Sub RemoveCursor(ByVal iPane As Integer)
            If Me.m_bShowCursor(iPane) Then
                Me.GetPane(iPane).CurveList.Remove(Me.m_liCursor(iPane))
                Me.m_liCursor(iPane) = Nothing
                Me.m_zgc.Invalidate()
            End If
        End Sub

        Private Sub SetCursor(ByVal iPane As Integer)
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
        Private Sub MyContextMenuBuilder(ByVal control As ZedGraphControl, ByVal menuStrip As ContextMenuStrip, ByVal mousePt As Point, ByVal objState As ZedGraphControl.ContextMenuObjectState)

            'ToDo_JS: globalize this

            ' create a new menu item
            Dim item As ToolStripMenuItem = New ToolStripMenuItem()
            ' This is the user-defined Tag so you can find this menu item later if necessary
            item.Name = "Extract_CSV_Data"
            item.Tag = "Extract_CSV_Data_tag"
            ' This is the text that will show up in the menu
            item.Text = "Extract to CSV..."
            ' Add a handler that will respond when that menu item is selected
            AddHandler item.Click, AddressOf ExtractToCSV
            ' Add the menu item to the menu
            menuStrip.Items.Add(item)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub ExtractToCSV(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.ExtractDataToCSV()
        End Sub

#End Region ' Context menu

#End Region ' Internal bits

    End Class

End Namespace ' Controls
