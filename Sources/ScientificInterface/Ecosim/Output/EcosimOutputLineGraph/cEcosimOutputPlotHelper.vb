'==============================================================================
'
' $Log: cEcosimOutputPlotHelper.vb,v $
' Revision 1.3  2009/06/20 01:10:36  jeroens
' Fixed multiple run init strangeness
'
' Revision 1.2  2009/05/28 12:37:08  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.1  2009/05/13 13:35:17  jeroens
' Renamed
'
' Revision 1.5  2009/05/13 12:53:02  jeroens
' Inherits from ZedGraphHelper
'
' Revision 1.4  2009/04/08 17:41:21  jeroens
' iOver -> iRunTest
'
' Revision 1.3  2009/03/30 19:02:41  jeroens
' Added Clear()
'
' Revision 1.2  2009/03/27 19:29:21  jeroens
' Overlay -> Run
'
' Revision 1.1  2009/03/23 02:31:55  jeroens
' Moved
'
' Revision 1.16  2008/12/15 15:37:28  jeroens
' no message
'
' Revision 1.15  2008/12/04 06:03:59  sherman
' Fixed Show/Hide refresh bug
' Fixed disposed bug
'
' Revision 1.14  2008/12/04 03:34:45  sherman
' Added show/hide group.
'
' Revision 1.13  2008/12/03 18:10:44  sherman
' Fixed timeseries coloring
'
' Revision 1.12  2008/11/26 16:01:24  jeroens
' Removed Ecosim-tied method SetCorrectAxis
'
' Revision 1.11  2008/11/05 22:41:06  joeh
' Use gray lines in cumulative plot
'
' Revision 1.10  2008/11/04 02:13:34  joeh
' Implement multiple selects for cumulative plot - Take two
'
' Revision 1.9  2008/11/03 18:40:00  joeh
' Implement multiple selects for cumulative plot
'
' Revision 1.8  2008/11/03 06:35:42  joeh
' Implement multiple selects for relative plot
'
' Revision 1.7  2008/10/31 19:57:03  joeh
' Implement relative catch
'
' Revision 1.6  2008/10/30 00:01:38  joeh
' Implement cumulative catch plot
'
' Revision 1.5  2008/10/29 00:15:13  joeh
' Implement cumulative biomass plot - Take three
'
' Revision 1.4  2008/10/25 00:37:05  joeh
' Implement cumulative biomass plot - Take two
'
' Revision 1.3  2008/10/24 19:36:47  joeh
' Implement cumulative biomass plot - Take one
'
' Revision 1.2  2008/10/07 21:53:32  sherman
' Set the max = 2 and min = 0 Y axis for Ecosim graph
'
' Revision 1.1  2008/09/26 07:31:20  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/08/28 21:26:41  sherman
' Moved ZedGraphHelper to SI Shared
'
'==============================================================================

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
        Private m_core As cCore = Nothing
        Private m_sg As cStyleGuide = cStyleGuide.GetInstance()

        Private m_bShowMultipleRuns As Boolean = False
        Private m_bCumulative As Boolean = False
        Private m_lclRuns As New List(Of CurveList)
        Private m_clRunCurrent As CurveList = Nothing
        Private m_dicTimeSeriesGroup As New Dictionary(Of Integer, CurveItem)
        Private m_pplSum As New PointPairList

#Region " Private helper classes "

        Private Class cCurveType

            Public m_strName As String = ""
            Public m_iIndex As Integer = 0
            Public m_iRun As Integer = 0
            Public m_lineType As eLineType = eLineType.CumulativeBiomass

            Public Sub New(ByVal strName As String, ByVal iIndex As Integer, ByVal iRun As Integer, ByVal lineType As eLineType)
                m_strName = strName
                m_iIndex = iIndex
                m_iRun = iRun
                m_lineType = lineType
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
            Me.m_clRunCurrent = New CurveList()
        End Sub

#End Region

#Region " Public interfaces "

        Public Overrides Sub Attach(ByVal core As EwECore.cCore, _
                                    ByVal zgc As ZedGraph.ZedGraphControl, _
                                    Optional ByVal iNumPanes As Integer = 1)

            Debug.Assert(iNumPanes = 1)
            Debug.Assert(core IsNot Nothing)

            MyBase.Attach(core, zgc, 1)

            Me.m_graphPane = Me.GetPane(1)
            Me.m_core = core

        End Sub

        Public Overrides Sub Detach()
            Me.Clear()
            MyBase.Detach()
        End Sub

#End Region

#Region " Public Properties "

        Public Sub Clear()
            Me.m_lclRuns.Clear()
            Me.m_graphPane.CurveList.Clear()
            Me.m_dicTimeSeriesGroup.Clear()
            Me.m_clRunCurrent = Nothing
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Makes sure all the object is set. Cleans up all list if required.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub CreateRun()
            If (Me.m_bShowMultipleRuns = False) Then
                Me.Clear()
            End If
            Me.m_clRunCurrent = New CurveList()
            Me.m_lclRuns.Add(Me.m_clRunCurrent)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a single line to the current run.
        ''' </summary>
        ''' <param name="strLabel">Label of the line to add.</param>
        ''' <param name="iGroup">Index of the underlying group.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddLine(ByVal strLabel As String, ByVal iGroup As Integer, ByVal lineType As eLineType, ByVal list As PointPairList)
            Dim crv As LineItem = Nothing
            Dim crvType As cCurveType = New cCurveType(strLabel, iGroup, m_lclRuns.Count - 1, lineType)

            Select Case crvType.m_lineType

                Case eLineType.CumulativeBiomass, eLineType.CumulativeSelectedBiomass, _
                  eLineType.CumulativeCatch, eLineType.CumulativeSelectedCatch
                    'crv = m_graphPane.AddCurve(name, list, Me.m_styGuide.GroupColor(m_core, index), SymbolType.None)
                    crv = m_graphPane.AddCurve(strLabel, list, Drawing.Color.LightSlateGray, SymbolType.None)
                    crv.Symbol.Type = SymbolType.None
                    crv.Line.Fill = New Fill(Me.m_sg.GroupColor(m_core, iGroup))
                    Me.m_clRunCurrent.Add(crv)
                    crv.Tag = crvType

                Case eLineType.RelativeBiomass, eLineType.RelativeCatch
                    crv = m_graphPane.AddCurve(strLabel, list, Me.m_sg.GroupColor(m_core, iGroup), SymbolType.None)
                    crv.Symbol.Type = SymbolType.None
                    Me.m_clRunCurrent.Add(crv)
                    crv.Tag = crvType

                Case eLineType.TimeSeries
                    If Not Me.m_dicTimeSeriesGroup.ContainsKey(crvType.m_iIndex) Then
                        crv = m_graphPane.AddCurve(strLabel, list, Me.m_sg.GroupColor(m_core, iGroup), SymbolType.None)
                        crv.Symbol.Type = SymbolType.Square
                        crv.Line.Color = Color.Transparent
                        Me.m_dicTimeSeriesGroup.Add(crvType.m_iIndex, crv)
                        crv.Tag = crvType
                    End If

            End Select

            ' After all that, just make sure it's shown
            If (lineType = eLineType.RelativeBiomass Or _
                   lineType = eLineType.CumulativeSelectedBiomass Or _
                   lineType = eLineType.CumulativeBiomass Or _
                   lineType = eLineType.TimeSeries) And _
                   Not crv Is Nothing Then
                crv.IsVisible = Me.m_sg.GroupVisible(iGroup)
            End If

        End Sub

        Public Function GetValueAt(ByVal iGroup As Integer, ByVal iRun As Integer, ByVal iTimeStep As Integer) As Double
            ' Wow, speaking about running into a brick wall at full tilt...
            Try
                Return Me.m_lclRuns.Item(iRun).Item(iGroup - 1).Item(iTimeStep).Y
            Catch ex As Exception
                Return 0.0
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Highlight data in the graph by colouring highlighted data, and
        ''' greying out data that is not highlighted.
        ''' </summary>
        ''' <param name="i"></param>
        ''' <param name="iCount"></param>
        ''' <param name="iGroup">Group index to select</param>
        ''' <param name="iRun">The run to highlight.</param>
        ''' -------------------------------------------------------------------
        Public Sub Highlight(ByVal i As Integer, ByVal iCount As Integer, ByVal iGroup As Integer, ByVal iRun As Integer)

            ' This is a tricky situation
            Dim crv As CurveItem = Nothing
            Dim crvType As cCurveType = Nothing

            If iGroup <= 0 And iRun < 0 Then
                ' Set set all to normal color
                Me.SetAllToColors(True)

            ElseIf iGroup > 0 And iRun < 0 Then
                ' Set only group for all runs

                'If highlighting the last curve (curve with the highest index in the collection)
                If i = iCount - 1 Then SetAllToColors(False)

                For iRunTest As Integer = 0 To m_lclRuns.Count - 1
                    crv = Me.m_lclRuns.Item(iRunTest).Item(iGroup - 1)
                    crvType = DirectCast(crv.Tag, cCurveType)

                    'If relative plot then plot line of selected group with highlight
                    If crvType.m_lineType = eLineType.RelativeBiomass Or _
                       crvType.m_lineType = eLineType.RelativeCatch Then
                        Me.SetLine(crv, True, True)
                    End If

                    'If cumulative plot then plot line of selected group without highlight but with color fill
                    If crvType.m_lineType = eLineType.CumulativeBiomass Or _
                       crvType.m_lineType = eLineType.CumulativeSelectedBiomass Or _
                       crvType.m_lineType = eLineType.CumulativeCatch Then

                        Me.SetLine(crv, True, False)
                        'If needed, plot line of the group of next lower index without highlight but with white fill
                        If iGroup >= 2 Then
                            crv = Me.m_lclRuns.Item(iRunTest).Item(iGroup - 2)
                            Me.SetLine(crv, True, False, True)
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

                Me.SetAllToColors(False)
                For j As Integer = 1 To m_lclRuns.Item(iRun).Count
                    crv = Me.m_lclRuns.Item(iRun).Item(j - 1)
                    Me.SetLine(crv, True, True)
                Next


            ElseIf iGroup > 0 And iRun >= 0 Then
                ' Set only one line

                SetAllToColors(False)
                crv = m_lclRuns.Item(iRun).Item(iGroup - 1)
                Me.SetLine(crv, True, True)

            End If

            ' Draw the time series for the group
            If m_dicTimeSeriesGroup.ContainsKey(iGroup) Then
                Me.SetLine(Me.m_dicTimeSeriesGroup(iGroup), True, True)
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
                Return Me.m_lclRuns.Count
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

#Region " Private Helpers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set all the colors either original or gray.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SetAllToColors(Optional ByVal bUseOriginalColor As Boolean = True)

            Dim crv As CurveItem = Nothing

            ' Set the lines
            For iOver As Integer = 0 To Me.m_lclRuns.Count - 1
                For iIndex As Integer = Me.m_lclRuns.Item(iOver).Count - 1 To 0 Step -1
                    crv = Me.m_lclRuns.Item(iOver).Item(iIndex)
                    If bUseOriginalColor Then
                        Me.SetLine(crv, True, False)
                    Else
                        Me.SetLine(crv, False, False)
                    End If
                Next iIndex
            Next iOver

            ' Set the TS plots
            For iIndex As Integer = 1 To m_core.nGroups
                If Me.m_dicTimeSeriesGroup.ContainsKey(iIndex) Then
                    If bUseOriginalColor Then
                        Me.SetLine(Me.m_dicTimeSeriesGroup(iIndex), True, False)
                    Else
                        Me.SetLine(Me.m_dicTimeSeriesGroup(iIndex), False, False)
                    End If
                End If
            Next
        End Sub

        Private Sub SetSomeToColors(ByVal startIndex As Integer)

            Dim crv As CurveItem = Nothing

            ' Set the lines
            For iOver As Integer = 0 To Me.m_lclRuns.Count - 1
                For iIndex As Integer = startIndex To 0 Step -1
                    crv = Me.m_lclRuns.Item(iOver).Item(iIndex)
                    Me.SetLine(crv, True, False, True)
                Next iIndex
            Next iOver

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
        Private Sub SetLine(ByVal crv As CurveItem, _
                            Optional ByVal bColorLine As Boolean = True, Optional ByVal bHighlightLine As Boolean = False, _
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
                If curveType.m_lineType = eLineType.TimeSeries Then
                    crv.Color = Me.m_sg.GroupColor(m_core, curveType.m_iIndex)
                End If

                'Relative plot
                If curveType.m_lineType = eLineType.RelativeBiomass Or _
                   curveType.m_lineType = eLineType.RelativeCatch Then
                    crv.Color = Me.m_sg.GroupColor(m_core, curveType.m_iIndex)
                End If

                'Cumulative plot
                If curveType.m_lineType = eLineType.CumulativeBiomass Or _
                   curveType.m_lineType = eLineType.CumulativeSelectedBiomass Or _
                   curveType.m_lineType = eLineType.CumulativeCatch Then
                    crv.Color = Drawing.Color.LightSlateGray 'Black
                    If bWhiteFillColor = True Then
                        line.Line.Fill = New Fill(Drawing.Color.White)
                    Else
                        line.Line.Fill = New Fill(Me.m_sg.GroupColor(m_core, DirectCast(crv.Tag, cCurveType).m_iIndex))
                    End If
                End If
                Me.m_graphPane.CurveList.Insert(0, line)
            Else
                crv.Color = Drawing.Color.LightSlateGray
                'Cumulative plot
                If curveType.m_lineType = eLineType.CumulativeBiomass Or _
                   curveType.m_lineType = eLineType.CumulativeSelectedBiomass Or _
                   curveType.m_lineType = eLineType.CumulativeCatch Then
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
            If curveType.m_lineType = eLineType.TimeSeries Then
                line.Line.Color = Color.Transparent
            End If

            ' After all that, just make sure it's shown
            If curveType.m_lineType = eLineType.RelativeBiomass Or _
                   curveType.m_lineType = eLineType.CumulativeSelectedBiomass Or _
                   curveType.m_lineType = eLineType.CumulativeBiomass Or _
                   curveType.m_lineType = eLineType.TimeSeries Then
                line.IsVisible = Me.m_sg.GroupVisible(curveType.m_iIndex)
            End If

        End Sub

#End Region ' Private Helpers

    End Class

End Namespace
