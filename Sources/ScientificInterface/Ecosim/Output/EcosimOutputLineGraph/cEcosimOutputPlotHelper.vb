#Region " Imports "

Option Strict On
Imports EwECore
Imports ZedGraph
Imports ScientificInterfaceShared.Style
#End Region ' Imports

Namespace Controls

    <CLSCompliant(False)> _
    Public Class cEcosimOutputPlotHelper
        Inherits cZedGraphHelper

        Private m_graphPane As GraphPane = Nothing

        Private m_bShowMultipleRuns As Boolean = False
        Private m_bCumulative As Boolean = False
        Private m_lRuns As New List(Of cRun)
        Private m_runCurrent As cRun = Nothing
        Private m_dicTimeSeriesGroup As New Dictionary(Of Integer, CurveItem)
        Private m_pplSum As New PointPairList

#Region " Private helper classes "

        Private Class cCurveType

            Private m_strName As String = ""
            Private m_iIndex As Integer = 0
            Private m_iRun As Integer = 0
            Private m_lineType As eLineType = eLineType.CumulativeBiomass
            Private m_bVisible As Boolean = True

            Public Sub New(ByVal strName As String, ByVal iIndex As Integer, ByVal iRun As Integer, ByVal lineType As eLineType)
                Me.m_strName = strName
                Me.m_iIndex = iIndex
                Me.m_iRun = iRun
                Me.m_lineType = lineType
            End Sub

            Public ReadOnly Property Name() As String
                Get
                    Return Me.m_strName
                End Get
            End Property

            Public ReadOnly Property Index() As Integer
                Get
                    Return Me.m_iIndex
                End Get
            End Property

            Public ReadOnly Property Run() As Integer
                Get
                    Return Me.m_iRun
                End Get
            End Property

            Public ReadOnly Property LineType() As eLineType
                Get
                    Return Me.m_lineType
                End Get
            End Property

            'Public Property Visible() As Boolean
            '    Get
            '        Return Me.m_bVisible
            '    End Get
            '    Set(ByVal value As Boolean)
            '        Me.m_bVisible = value
            '    End Set
            'End Property

        End Class

        Private Class cRun

            Public m_strName As String
            Public m_curvelist As CurveList = Nothing

            Public Sub New(ByVal strName As String)
                Me.m_strName = strName
                Me.m_curvelist = New CurveList()
            End Sub
        End Class

#End Region ' Private helper classes

        Public Enum eLineType
            CumulativeBiomass = 0
            CumulativeSelectedBiomass
            RelativeBiomass
            CumulativeCatch
            CumulativeSelectedCatch
            RelativeCatch
            TimeSeries
            Value
            CummulativeValue
        End Enum

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            'Me.m_clRunCurrent = New CurveList()
        End Sub

#End Region

#Region " Public interfaces "

        Public Overrides Sub Attach(ByVal uic As cUIContext, _
                                    ByVal zgc As ZedGraph.ZedGraphControl, _
                                    Optional ByVal iNumPanes As Integer = 1)

            Debug.Assert(iNumPanes = 1)

            MyBase.Attach(uic, zgc, 1)

            Me.m_graphPane = Me.GetPane(1)
        End Sub

        Public Overrides Sub Detach()
            Me.Clear()
            MyBase.Detach()
        End Sub

#End Region

#Region " Public Properties "

        Public Sub Clear()
            Me.m_lRuns.Clear()
            Me.m_runCurrent = Nothing
            Me.m_graphPane.CurveList.Clear()
            Me.m_dicTimeSeriesGroup.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Makes sure all the object is set. Cleans up all list if required.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub CreateRun(ByVal strLabel As String)
            If (Me.m_bShowMultipleRuns = False) Then
                Me.Clear()
            End If
            Me.m_runCurrent = New cRun(strLabel)
            Me.m_lRuns.Add(Me.m_runCurrent)
        End Sub

        Public Sub ResetRun()
            If (Me.m_runCurrent Is Nothing) Then Return
            Me.m_runCurrent.m_curvelist.Clear()
            Me.m_graphPane.CurveList.Clear()
        End Sub

        ''' <summary>
        ''' Is the graph ready to plot? Has it been initialized?
        ''' </summary>
        ''' <value></value>
        ''' <returns>True if the graph is ready to plot.</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property isReady() As Boolean
            Get
                Return (Me.m_runCurrent IsNot Nothing)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a single line to the current run.
        ''' </summary>
        ''' <param name="strLabel">Label of the line to add.</param>
        ''' <param name="iGroup">Index of the underlying group.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddLine(ByVal strLabel As String, _
                           ByVal iGroup As Integer, _
                           ByVal lineType As eLineType, _
                           ByVal list As PointPairList)

            Dim crv As LineItem = Nothing
            Dim crvType As cCurveType = New cCurveType(strLabel, iGroup, Me.m_lRuns.Count - 1, lineType)

            Select Case crvType.LineType

                Case eLineType.CumulativeBiomass, eLineType.CumulativeSelectedBiomass, _
                  eLineType.CumulativeCatch, eLineType.CumulativeSelectedCatch
                    'crv = m_graphPane.AddCurve(name, list, Me.m_styGuide.GroupColor(Me.Core, index), SymbolType.None)
                    crv = m_graphPane.AddCurve(strLabel, list, Drawing.Color.LightSlateGray, SymbolType.None)
                    crv.Symbol.Type = SymbolType.None
                    crv.Line.Fill = New Fill(Me.StyleGuide.GroupColor(Me.Core, iGroup))
                    Me.m_runCurrent.m_curvelist.Add(crv)
                    crv.Tag = crvType

                Case eLineType.RelativeBiomass, eLineType.RelativeCatch
                    crv = m_graphPane.AddCurve(strLabel, list, Me.StyleGuide.GroupColor(Me.Core, iGroup), SymbolType.None)
                    crv.Symbol.Type = SymbolType.None
                    Me.m_runCurrent.m_curvelist.Add(crv)
                    crv.Tag = crvType

                Case eLineType.TimeSeries
                    If Not Me.m_dicTimeSeriesGroup.ContainsKey(crvType.Index) Then
                        crv = m_graphPane.AddCurve(strLabel, list, Me.StyleGuide.GroupColor(Me.Core, iGroup), SymbolType.None)
                        crv.Symbol.Type = SymbolType.Square
                        crv.Line.Color = Color.Transparent
                        Me.m_dicTimeSeriesGroup.Add(crvType.Index, crv)
                        crv.Tag = crvType
                    End If

            End Select

            Me.SetCurveVisibility(crv)

        End Sub

        Public Function GetValueAt(ByVal iGroup As Integer, _
                                   ByVal iRun As Integer, _
                                   ByVal iTimeStep As Integer) As Double

            Try
                ' Wow, speaking about running into a brick wall at full tilt...
                Return Me.m_lRuns(iRun).m_curvelist.Item(iGroup - 1).Item(iTimeStep).Y
            Catch ex As Exception
                Return 0.0
            End Try

        End Function

        Public Sub ClearHighlights()
            ' Clear all colors
            Me.SetAllToColors(False)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Highlight data in the graph by colouring highlighted data, and
        ''' greying out data that is not highlighted.
        ''' </summary>
        ''' <param name="iGroup">Group index to select</param>
        ''' <param name="iRun">The run to highlight.</param>
        ''' -------------------------------------------------------------------
        Public Sub Highlight(ByVal iGroup As Integer, _
                             ByVal iRun As Integer, _
                             Optional ByVal bHideGray As Boolean = False)

            Dim run As cRun = Nothing
            Dim cl As CurveList = Nothing
            Dim crv As CurveItem = Nothing
            Dim crvType As cCurveType = Nothing

            If iGroup <= 0 And iRun < 0 Then
                ' Set set all to normal color
                Me.SetAllToColors(True)
            ElseIf iGroup > 0 And iRun < 0 Then
                ' Set only group for all runs

                For iRunTest As Integer = 0 To m_lRuns.Count - 1

                    run = Me.m_lRuns.Item(iRunTest)
                    cl = run.m_curvelist
                    crv = cl.Item(iGroup - 1)
                    crvType = DirectCast(crv.Tag, cCurveType)

                    'If relative plot then plot line of selected group with highlight
                    If crvType.LineType = eLineType.RelativeBiomass Or _
                       crvType.LineType = eLineType.RelativeCatch Then
                        Me.SetCurveColour(crv, True, True)
                    End If

                    'If cumulative plot then plot line of selected group without highlight but with color fill
                    If crvType.LineType = eLineType.CumulativeBiomass Or _
                       crvType.LineType = eLineType.CumulativeSelectedBiomass Or _
                       crvType.LineType = eLineType.CumulativeCatch Then

                        Me.SetCurveColour(crv, True, False)
                        'If needed, plot line of the group of next lower index without highlight but with white fill
                        If iGroup >= 2 Then
                            run = Me.m_lRuns(iRunTest)
                            cl = run.m_curvelist
                            crv = cl.Item(iGroup - 2)
                            Me.SetCurveColour(crv, True, False, True)
                        End If
                        'If needed, plot lines of the remaining groups
                        If iGroup >= 3 Then
                            Me.SetSomeToColors(iGroup - 3)
                        End If
                    End If
                    'SetSomeToColors(index - 3, False)
                Next iRunTest

                ' Need to set all of the keys individually for all the groups.

            ElseIf iGroup <= 0 And iRun >= 0 Then
                ' Only single run to highlight
                run = Me.m_lRuns(iRun)
                cl = run.m_curvelist
                For iGroup = cl.Count - 1 To 0 Step -1
                    crv = cl.Item(iGroup)
                    Me.SetCurveColour(crv, True, False)
                Next

            ElseIf iGroup > 0 And iRun >= 0 Then
                ' Set only one line
                run = Me.m_lRuns(iRun)
                cl = run.m_curvelist
                crv = cl.Item(iGroup - 1)
                Me.SetCurveColour(crv, True, True)

            End If

            ' Colour the time series for the group
            If m_dicTimeSeriesGroup.ContainsKey(iGroup) Then
                Me.SetCurveColour(Me.m_dicTimeSeriesGroup(iGroup), True, True)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether multiple runs should be shown.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShowMultipleRuns() As Boolean
            Get
                Return Me.m_bShowMultipleRuns
            End Get
            Set(ByVal bShowMultipleRuns As Boolean)
                ' Update flag
                Me.m_bShowMultipleRuns = bShowMultipleRuns
                ' Switched to single view?
                If (bShowMultipleRuns = False) Then
                    ' #Yes: clear 
                    Me.Clear()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Gets the number of runs.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumRuns() As Integer
            Get
                Return Me.m_lRuns.Count
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Gets the label of run.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property RunLabel(ByVal iRun As Integer) As String
            Get
                Return Me.m_lRuns(iRun).m_strName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the managed graph shows a legend.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShowLegend() As Boolean
            Get
                Return Me.m_graphPane.Legend.IsVisible
            End Get
            Set(ByVal value As Boolean)
                Me.m_graphPane.Legend.IsVisible = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set the graph title.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public WriteOnly Property DataName() As String
            Set(ByVal value As String)
                Me.m_graphPane.Title.Text = value
                Me.m_graphPane.YAxis.Title.Text = value
            End Set
        End Property

#End Region ' Public Properties

#Region " Events "

        Protected Overrides Sub OnStyleguideChanged(ByVal ct As cStyleGuide.eChangeType)

            If (ct And cStyleGuide.eChangeType.GroupVisibility) = cStyleGuide.eChangeType.GroupVisibility Then
                ' Update visibility on lines
                For Each crv As CurveItem In Me.m_graphPane.CurveList
                    Me.SetCurveVisibility(crv)
                Next
                Me.Graph.Refresh()
            End If

        End Sub

#End Region ' Events

#Region " Private Helpers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set all the colors either original or gray.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SetAllToColors(Optional ByVal bUseOriginalColor As Boolean = True)

            Dim cl As CurveList = Nothing
            Dim crv As CurveItem = Nothing
            Dim crvtype As cCurveType = Nothing

            ' Set the lines
            For Each run As cRun In Me.m_lRuns
                cl = run.m_curvelist
                For iCurve As Integer = cl.Count - 1 To 0 Step -1

                    crv = cl.Item(iCurve)
                    crvtype = DirectCast(crv.Tag, cCurveType)

                    If bUseOriginalColor Then
                        Me.SetCurveColour(crv, True, False)
                    Else
                        Me.SetCurveColour(crv, False, False)
                    End If
                Next iCurve
            Next run

            ' Set the TS plots
            For iIndex As Integer = 1 To Me.Core.nGroups
                If Me.m_dicTimeSeriesGroup.ContainsKey(iIndex) Then
                    If bUseOriginalColor Then
                        Me.SetCurveColour(Me.m_dicTimeSeriesGroup(iIndex), True, False)
                    Else
                        Me.SetCurveColour(Me.m_dicTimeSeriesGroup(iIndex), False, False)
                    End If
                End If
            Next
        End Sub

        Private Sub SetSomeToColors(ByVal iStartCurve As Integer)

            Dim run As cRun = Nothing
            Dim cl As CurveList = Nothing
            Dim crv As CurveItem = Nothing

            ' Set the lines
            For iRun As Integer = 0 To Me.NumRuns - 1
                run = Me.m_lRuns(iRun)
                cl = run.m_curvelist
                For iCurve As Integer = iStartCurve To 0 Step -1
                    crv = cl.Item(iCurve)
                    Me.SetCurveColour(crv, True, False, True)
                Next iCurve
            Next iRun

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> 
        ''' Change the properties of the line to colored or thicker.
        ''' </summary>
        ''' <param name="crv">The curve to alter.</param>
        ''' <param name="bColorLine">
        ''' Flag stating whether the line should be coloured (true) or grayed-out 
        ''' (false).
        ''' </param>
        ''' <param name="bHighlightLine">
        ''' Flag stating whether the line should be higlighted (true) or
        ''' non-highlighted (false).
        ''' </param>
        ''' <param name="bWhiteFillColor">
        ''' Flag stating whether the area under the line should be filled with 
        ''' white (true) or with the group colour (false).
        ''' </param>
        ''' -------------------------------------------------------------------
        Private Sub SetCurveColour(ByVal crv As CurveItem, _
                                   Optional ByVal bColorLine As Boolean = True, _
                                   Optional ByVal bHighlightLine As Boolean = False, _
                                   Optional ByVal bWhiteFillColor As Boolean = False)

            ' Safety first
            If Not TypeOf (crv) Is LineItem Then Return
            If Not TypeOf (crv.Tag) Is cCurveType Then Return

            Dim line As LineItem = DirectCast(crv, LineItem)
            Dim curveType As cCurveType = DirectCast(crv.Tag, cCurveType)

            ' Remove the curve
            Me.m_graphPane.CurveList.Remove(crv)

            ' Change the color
            If bColorLine Then
                'TimeSeries
                If curveType.LineType = eLineType.TimeSeries Then
                    crv.Color = Me.StyleGuide.GroupColor(Me.Core, curveType.Index)
                End If

                'Relative plot
                If curveType.LineType = eLineType.RelativeBiomass Or _
                   curveType.LineType = eLineType.RelativeCatch Then
                    crv.Color = Me.StyleGuide.GroupColor(Me.Core, curveType.Index)
                End If

                'Cumulative plot
                If curveType.LineType = eLineType.CumulativeBiomass Or _
                   curveType.LineType = eLineType.CumulativeSelectedBiomass Or _
                   curveType.LineType = eLineType.CumulativeCatch Then
                    crv.Color = Drawing.Color.LightSlateGray 'Black
                    If bWhiteFillColor = True Then
                        line.Line.Fill = New Fill(Drawing.Color.White)
                    Else
                        line.Line.Fill = New Fill(Me.StyleGuide.GroupColor(Me.Core, curveType.Index))
                    End If
                End If
                Me.m_graphPane.CurveList.Insert(0, line)
            Else
                crv.Color = Drawing.Color.LightSlateGray
                'Cumulative plot
                If curveType.LineType = eLineType.CumulativeBiomass Or _
                   curveType.LineType = eLineType.CumulativeSelectedBiomass Or _
                   curveType.LineType = eLineType.CumulativeCatch Then
                    line.Line.Fill = New Fill(Drawing.Color.Transparent)
                End If
                Me.m_graphPane.CurveList.Add(crv)
            End If

            ' Set the highlights
            If bHighlightLine Then
                line.Line.Width = 3
            Else
                line.Line.Width = 1
            End If

            ' Hide the time series lines
            If curveType.LineType = eLineType.TimeSeries Then
                line.Line.Color = Color.Transparent
            End If

            Me.SetCurveVisibility(crv)

        End Sub

        Private Sub SetCurveVisibility(ByVal crv As CurveItem)

            ' Safety first
            If Not TypeOf (crv) Is LineItem Then Return
            If Not TypeOf (crv.Tag) Is cCurveType Then Return

            Dim line As LineItem = DirectCast(crv, LineItem)
            Dim curveType As cCurveType = DirectCast(crv.Tag, cCurveType)

            ' After all that, just make sure it's shown
            If curveType.LineType = eLineType.RelativeBiomass Or _
                   curveType.LineType = eLineType.CumulativeSelectedBiomass Or _
                   curveType.LineType = eLineType.CumulativeBiomass Or _
                   curveType.LineType = eLineType.TimeSeries Then
                line.IsVisible = Me.StyleGuide.GroupVisible(curveType.Index) ' And curveType.Visible
            End If

        End Sub

#End Region ' Private Helpers

    End Class

End Namespace
