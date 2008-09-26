'==============================================================================
'
' $Log: ZedGraphPlotter.vb,v $
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
            Biomass
            TimeSeries
        End Enum

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary> Constructor to store all the required variables. </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal p_graphPane As GraphPane, ByVal core As cCore, Optional ByVal Title As String = "", Optional ByVal XaxisTitle As String = "", Optional ByVal YaxisTitle As String = "")
            m_graphPane = p_graphPane
            m_core = core

            m_graphPane.Title.Text = Title
            m_graphPane.XAxis.Title.Text = XaxisTitle
            m_graphPane.YAxis.Title.Text = YaxisTitle
            SetCorrectAxis()

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
                Case eLineType.Biomass
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

            SetCorrectAxis()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>Highlight the dataset if required.</summary>
        ''' -------------------------------------------------------------------
        Public Sub SetHighlight(ByVal index As Integer, ByVal overlay As Integer)
            ' This is a tricky situation
            ' Overlay is one less than overlay thus
            ' the need to overlay is <= 0

            If index <= 0 And overlay < 0 Then
                ' Set set all to normal color
                SetAllToColors(True)

            ElseIf index > 0 And overlay < 0 Then
                ' Set only group for all overlays

                SetAllToColors(False)

                For iOver As Integer = 0 To m_Overlays.Count - 1
                    Dim crv As CurveItem = m_Overlays.Item(iOver).Item(index - 1)
                    SetLine(crv, True, True)
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

#End Region ' Public Properties

#Region " Private Helpers "

        ''' -------------------------------------------------------------------
        ''' <summary>Ensures the axis are set correctly</summary>
        ''' -------------------------------------------------------------------
        Private Sub SetCorrectAxis()
            m_graphPane.XAxis.Scale.Min = m_core.EcosimFirstYear
            m_graphPane.XAxis.Scale.Max = m_core.EcoSimModelParameters.NumberYears + m_core.EcosimFirstYear
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>Will set all the colors either original or gray</summary>
        ''' -------------------------------------------------------------------
        Private Sub SetAllToColors(Optional ByVal UseOriginalColor As Boolean = True)
            ' Set the lines
            For iOver As Integer = 0 To m_Overlays.Count - 1
                For iIndex As Integer = 0 To m_Overlays.Item(iOver).Count - 1
                    Dim crv As CurveItem = m_Overlays.Item(iOver).Item(iIndex)
                    If UseOriginalColor Then
                        SetLine(crv, True, False)
                    Else
                        SetLine(crv, False, False)
                    End If
                Next iIndex
            Next iOver

            ' Set the TS plots
            For iIndex As Integer = 0 To m_core.nGroups - 1
                If m_dicTimeSeriesGroup.ContainsKey(iIndex) Then
                    If UseOriginalColor Then
                        SetLine(m_dicTimeSeriesGroup(iIndex), True, False)
                    Else
                        SetLine(m_dicTimeSeriesGroup(iIndex), False, False)
                    End If
                End If
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> 
        ''' Change the properties of the line to colored or thicker
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SetLine(ByVal crv As CurveItem, Optional ByVal bColorLine As Boolean = True, Optional ByVal bHighlightLine As Boolean = False)
            Dim p_line As LineItem = DirectCast(crv, LineItem)
            If TypeOf crv.Tag Is CurveType Then

                ' Remove the curve
                Me.m_graphPane.CurveList.Remove(crv)

                ' Change the color
                If bColorLine Then
                    crv.Color = Me.m_styGuide.GroupColor(m_core, DirectCast(crv.Tag, CurveType).Index)
                    Me.m_graphPane.CurveList.Insert(0, p_line)
                Else
                    crv.Color = Drawing.Color.LightSlateGray
                    Me.m_graphPane.CurveList.Add(crv)
                End If

                ' Set the highlights
                If bHighlightLine Then
                    DirectCast(crv, LineItem).Line.Width = 3
                Else
                    DirectCast(crv, LineItem).Line.Width = 1
                End If

                ' Hide the time series lines
                If DirectCast(crv.Tag, CurveType).LineType = eLineType.TimeSeries Then
                    DirectCast(crv, LineItem).Line.Color = Color.Transparent
                End If
            End If

        End Sub

#End Region ' Private Helpers

    End Class
End Namespace
