'==============================================================================
'
' $Log: ZedGraphPlotter.vb,v $
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

#Region " Imports directive "

Option Strict On
Imports EwECore
Imports ZedGraph
Imports ScientificInterfaceShared.Style
#End Region


Namespace Controls
    Public Class ZedGraphPlotter

        Private m_graphPane As GraphPane = Nothing
        Private m_core As cCore = Nothing
        Private m_styGuide As StyleGuide = StyleGuide.GetInstance()

        Private m_IsOverlayOn As Boolean = False
        Private m_Overlays As New List(Of CurveList)
        Private m_CurrentOverlay As New CurveList
        Private m_dicTimeSeriesGroup As New Dictionary(Of Integer, CurveItem)
        Private m_listSum As New PointPairList


        Public Class CurveType
            Public Name As String
            Public Index As Integer
            Public Overlay As Integer
            Public LineType As eLineType

            Public Sub New(ByVal p_Name As String, ByVal p_Index As Integer, ByVal p_Overlay As Integer, ByVal p_LineType As eLineType)
                Name = p_Name
                Index = p_Index
                Overlay = p_Overlay
                LineType = p_LineType
            End Sub
        End Class

        Public Enum eLineType
            CumulativeBiomass
            CumulativeSelectedBiomass
            RelativeBiomass
            CumulativeCatch
            CumulativeSelectedCatch
            RelativeCatch
            TimeSeries
        End Enum

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary> Constructor to store all the required variables. </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal p_graphPane As GraphPane, ByVal core As cCore, Optional ByVal Title As String = "", Optional ByVal XaxisTitle As String = "", Optional ByVal YaxisTitle As String = "")
            m_graphPane = p_graphPane
            m_core = core

            ' Designer-time bailout
            If core Is Nothing Then Return

            m_graphPane.Title.Text = Title
            m_graphPane.XAxis.Title.Text = XaxisTitle
            m_graphPane.YAxis.Title.Text = YaxisTitle

            ' Oof, this code relies on Ecosim being loaded - this cannot be allowed in ScIntShared!
            'SetCorrectAxis()

            m_graphPane.Legend.IsVisible = False

            m_graphPane.AxisChange()

        End Sub

#End Region

#Region " Public Properties "

        ''' -------------------------------------------------------------------
        ''' <summary>(1) Makes sure all the object is set.  Cleans up all list if required.</summary>
        ''' -------------------------------------------------------------------
        Public Sub PrepareDataset(Optional ByVal ForceClear As Boolean = False)
            If Not m_IsOverlayOn Or ForceClear Then
                m_Overlays.Clear()
                m_graphPane.CurveList.Clear()
                m_dicTimeSeriesGroup.Clear()
            End If
            m_CurrentOverlay = New CurveList
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>(2) Add a single dataset</summary>
        ''' -------------------------------------------------------------------
        Public Sub AddSingleData(ByVal name As String, ByVal index As Integer, ByVal lineType As eLineType, ByVal list As PointPairList)
            Dim crv As LineItem
            Dim crvType As CurveType = New CurveType(name, index, m_Overlays.Count - 1, lineType)

            Select Case crvType.LineType
                Case eLineType.CumulativeBiomass, eLineType.CumulativeSelectedBiomass, _
                  eLineType.CumulativeCatch, eLineType.CumulativeSelectedCatch
                    'crv = m_graphPane.AddCurve(name, list, Me.m_styGuide.GroupColor(m_core, index), SymbolType.None)
                    crv = m_graphPane.AddCurve(name, list, Drawing.Color.LightSlateGray, SymbolType.None)
                    crv.Symbol.Type = SymbolType.None
                    crv.Line.Fill = New Fill(Me.m_styGuide.GroupColor(m_core, index))
                    m_CurrentOverlay.Add(crv)
                    crv.Tag = crvType
                Case eLineType.RelativeBiomass, eLineType.RelativeCatch
                    crv = m_graphPane.AddCurve(name, list, Me.m_styGuide.GroupColor(m_core, index), SymbolType.None)
                    crv.Symbol.Type = SymbolType.None
                    m_CurrentOverlay.Add(crv)
                    crv.Tag = crvType
                Case eLineType.TimeSeries
                    If Not m_dicTimeSeriesGroup.ContainsKey(crvType.Index) Then
                        crv = m_graphPane.AddCurve(name, list, Me.m_styGuide.GroupColor(m_core, index), SymbolType.None)
                        crv.Symbol.Type = SymbolType.Square
                        crv.Line.Color = Color.Transparent
                        m_dicTimeSeriesGroup.Add(crvType.Index, crv)
                        crv.Tag = crvType
                    End If
            End Select

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>(3) Finally store the dataset.</summary>
        ''' -------------------------------------------------------------------
        Public Sub StoreDataset()
            m_Overlays.Add(m_CurrentOverlay)

            'SetCorrectAxis()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>Highlight the dataset if required.</summary>
        ''' -------------------------------------------------------------------
        Public Sub SetHighlight(ByVal i As Integer, ByVal count As Integer, ByVal index As Integer, ByVal overlay As Integer)
            ' This is a tricky situation
            ' Overlay is one less than overlay thus
            ' the need to overlay is <= 0

            If index <= 0 And overlay < 0 Then
                ' Set set all to normal color
                SetAllToColors(True)

            ElseIf index > 0 And overlay < 0 Then
                ' Set only group for all overlays

                'If highlighting the last curve (curve with the highest index in the collection)
                If i = count - 1 Then SetAllToColors(False)

                For iOver As Integer = 0 To m_Overlays.Count - 1
                    Dim crv As CurveItem = m_Overlays.Item(iOver).Item(index - 1)
                    Dim crvType As CurveType = DirectCast(crv.Tag, CurveType)

                    'If relative plot then plot line of selected group with highlight
                    If crvType.LineType = eLineType.RelativeBiomass Or _
                       crvType.LineType = eLineType.RelativeCatch Then
                        SetLine(crv, True, True)
                    End If

                    'If cumulative plot then plot line of selected group without highlight but with color fill
                    If crvType.LineType = eLineType.CumulativeBiomass Or _
                       crvType.LineType = eLineType.CumulativeSelectedBiomass Or _
                       crvType.LineType = eLineType.CumulativeCatch Then

                        SetLine(crv, True, False)
                        'If needed, plot line of the group of next lower index without highlight but with white fill
                        If index >= 2 Then
                            crv = m_Overlays.Item(iOver).Item(index - 2)
                            SetLine(crv, True, False, True)
                        End If
                        'If needed, plot lines of the remaining groups
                        If index >= 3 Then
                            SetSomeToColors(index - 3, False)
                        End If
                    End If
                    'SetSomeToColors(index - 3, False)
                Next iOver

                ' Need to set all of the keys individually for all the groups.


            ElseIf index <= 0 And overlay >= 0 Then
                ' Only single Overlay to highlight

                SetAllToColors(False)
                For iIndex As Integer = 1 To m_Overlays.Item(overlay).Count
                    Dim crv As CurveItem = m_Overlays.Item(overlay).Item(iIndex - 1)
                    SetLine(crv, True, True)
                Next


            ElseIf index > 0 And overlay >= 0 Then
                ' Set only one line

                SetAllToColors(False)
                Dim crv As CurveItem = m_Overlays.Item(overlay).Item(index - 1)
                SetLine(crv, True, True)

            End If

            ' Draw the time series for the group
            If m_dicTimeSeriesGroup.ContainsKey(index) Then SetLine(m_dicTimeSeriesGroup(index), True, True)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>Get/Sets the overlay boolean property</summary>
        ''' -------------------------------------------------------------------
        Public Property Overlay() As Boolean
            Get
                Return m_IsOverlayOn
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    PrepareDataset()
                End If
                m_IsOverlayOn = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>Gets the number of overlays</summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumOverlays() As Integer
            Get
                Return m_Overlays.Count
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>Allows users to toggle the Legend</summary>
        ''' -------------------------------------------------------------------
        Public Property ShowLegend() As Boolean
            Get
                Return m_graphPane.Legend.IsVisible
            End Get
            Set(ByVal value As Boolean)
                m_graphPane.Legend.IsVisible = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>Sets the Y axis title property</summary>
        ''' -------------------------------------------------------------------
        Public WriteOnly Property YaxisTitle() As String
            Set(ByVal value As String)
                m_graphPane.YAxis.Title.Text = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>Sets the title property</summary>
        ''' -------------------------------------------------------------------
        Public WriteOnly Property Title() As String
            Set(ByVal value As String)
                m_graphPane.Title.Text = value
            End Set
        End Property

#End Region ' Public Properties

#Region " Private Helpers "

        ' JS26nov08: No references to Ecosim from within this class!
        '''' -------------------------------------------------------------------
        '''' <summary>Ensures the axis are set correctly</summary>
        '''' -------------------------------------------------------------------
        'Private Sub SetCorrectAxis()
        '    m_graphPane.XAxis.Scale.Min = m_core.EcosimFirstYear
        '    m_graphPane.XAxis.Scale.Max = m_core.EcoSimModelParameters.NumberYears + m_core.EcosimFirstYear
        '    m_graphPane.YAxis.Scale.Min = 0
        '    If m_graphPane.YAxis.Scale.Max < 2 Then m_graphPane.YAxis.Scale.Max = 2
        'End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>Will set all the colors either original or gray</summary>
        ''' -------------------------------------------------------------------
        Private Sub SetAllToColors(Optional ByVal bUseOriginalColor As Boolean = True)
            ' Set the lines
            For iOver As Integer = 0 To m_Overlays.Count - 1
                'For iIndex As Integer = 0 To m_Overlays.Item(iOver).Count - 1
                For iIndex As Integer = m_Overlays.Item(iOver).Count - 1 To 0 Step -1
                    Dim crv As CurveItem = m_Overlays.Item(iOver).Item(iIndex)
                    If bUseOriginalColor Then
                        SetLine(crv, True, False)
                    Else
                        SetLine(crv, False, False)
                    End If
                Next iIndex
            Next iOver

            ' Set the TS plots
            For iIndex As Integer = 1 To m_core.nGroups
                If m_dicTimeSeriesGroup.ContainsKey(iIndex) Then
                    If bUseOriginalColor Then
                        SetLine(m_dicTimeSeriesGroup(iIndex), True, False)
                    Else
                        SetLine(m_dicTimeSeriesGroup(iIndex), False, False)
                    End If
                End If
            Next
        End Sub

        Private Sub SetSomeToColors(ByVal startIndex As Integer, Optional ByVal UseOriginalColor As Boolean = True)
            ' Set the lines
            For iOver As Integer = 0 To m_Overlays.Count - 1
                For iIndex As Integer = startIndex To 0 Step -1
                    Dim crv As CurveItem = m_Overlays.Item(iOver).Item(iIndex)
                    'If UseOriginalColor Then
                    SetLine(crv, True, False, True)
                    'Else
                    'SetLine(crv, False, False)
                    'End If
                Next iIndex
            Next iOver
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> 
        ''' Change the properties of the line to colored or thicker
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SetLine(ByVal crv As CurveItem, Optional ByVal bColorLine As Boolean = True, Optional ByVal bHighlightLine As Boolean = False, _
          Optional ByVal bWhiteFillColor As Boolean = False)

            ' Safety first
            If Not TypeOf (crv) Is LineItem Then Return
            If Not TypeOf (crv.Tag) Is CurveType Then Return

            Dim p_line As LineItem = DirectCast(crv, LineItem)
            Dim p_CurveTag As CurveType = DirectCast(crv.Tag, CurveType)

            ' Remove the curve
            Me.m_graphPane.CurveList.Remove(crv)

            ' Change the color
            If bColorLine Then
                'TimeSeries
                If p_CurveTag.LineType = eLineType.TimeSeries Then
                    crv.Color = Me.m_styGuide.GroupColor(m_core, p_CurveTag.Index)
                End If

                'Relative plot
                If p_CurveTag.LineType = eLineType.RelativeBiomass Or _
                   p_CurveTag.LineType = eLineType.RelativeCatch Then
                    crv.Color = Me.m_styGuide.GroupColor(m_core, p_CurveTag.Index)
                End If

                'Cumulative plot
                If p_CurveTag.LineType = eLineType.CumulativeBiomass Or _
                   p_CurveTag.LineType = eLineType.CumulativeSelectedBiomass Or _
                   p_CurveTag.LineType = eLineType.CumulativeCatch Then
                    crv.Color = Drawing.Color.LightSlateGray 'Black
                    If bWhiteFillColor = True Then
                        p_line.Line.Fill = New Fill(Drawing.Color.White)
                    Else
                        p_line.Line.Fill = New Fill(Me.m_styGuide.GroupColor(m_core, DirectCast(crv.Tag, CurveType).Index))
                    End If
                End If
                Me.m_graphPane.CurveList.Insert(0, p_line)
            Else
                crv.Color = Drawing.Color.LightSlateGray
                'Cumulative plot
                If p_CurveTag.LineType = eLineType.CumulativeBiomass Or _
                   p_CurveTag.LineType = eLineType.CumulativeSelectedBiomass Or _
                   p_CurveTag.LineType = eLineType.CumulativeCatch Then
                    p_line.Line.Fill = New Fill(Drawing.Color.Transparent)
                End If
                Me.m_graphPane.CurveList.Add(crv)
            End If

            ' Set the highlights
            If bHighlightLine Then
                p_line.Line.Width = 3
            Else
                p_line.Line.Width = 1
            End If

            ' Hide the time series lines
            If p_CurveTag.LineType = eLineType.TimeSeries Then
                p_line.Line.Color = Color.Transparent
            End If

            ' After all that, just make sure it's shown
            p_line.IsVisible = Me.m_styGuide.GroupVisible(p_CurveTag.Index)
        End Sub

#End Region ' Private Helpers

    End Class
End Namespace
