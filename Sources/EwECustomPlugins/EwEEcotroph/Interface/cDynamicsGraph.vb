'==============================================================================
'
' $Log: cDynamicsGraph.vb,v $
' Revision 1.1  2008/09/26 07:30:43  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/06/05 19:43:47  joeh
' no message
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports ZedGraph

Public Class cDynamicsGraph

#Region "Public fields"
    Public m_Form As Form
    Public m_NodeText As String
    Public m_TabPageText As String
    Public m_DataGrid As DataGridView
    Public m_ZedGraph As ZedGraphControl
#End Region 'Public fields

#Region "Public methods"
    Public Sub PlotGraph()
        Select Case m_TabPageText
            Case My.Resources.TAB_SUMMARY
                PlotSummary()
            Case Else
                PlotGeneral()
        End Select
    End Sub
#End Region 'Public methods

#Region "Private methods"
    Private Sub PlotSummary()
        Dim Panes As MasterPane = m_ZedGraph.MasterPane
        Dim Pane1 As GraphPane = New ZedGraph.GraphPane
        Dim Pane2 As GraphPane = New ZedGraph.GraphPane
        Dim List As PointPairList
        Dim Row As Integer
        Dim Graphic As Graphics

        Panes.PaneList.Clear()
        Panes.Add(Pane1)
        Panes.Add(Pane2)

        'Pane1
        Pane1.Title.Text = ""
        Pane1.XAxis.Title.Text = My.Resources.GRP_XLBL_YEAR
        Pane1.XAxis.Title.FontSpec.Size = 17
        Pane1.XAxis.MajorTic.IsOutside = False
        Pane1.XAxis.MinorTic.IsOutside = False
        Pane1.XAxis.Scale.Min = CSng(m_DataGrid.Columns(3 - 1).HeaderText)
        Pane1.XAxis.Scale.Max = CSng(m_DataGrid.Columns(m_DataGrid.ColumnCount - 1).HeaderText)
        Pane1.YAxis.Title.Text = ""
        Pane1.YAxis.Title.FontSpec.Size = 17
        Pane1.YAxis.MajorTic.IsOutside = False
        Pane1.YAxis.MinorTic.IsOutside = False
        Pane1.YAxis.MinorTic.IsAllTics = False
        Pane1.Legend.Position = LegendPos.TopCenter
        Pane1.Legend.FontSpec.Size = 15

        m_ZedGraph.MasterPane(0).CurveList.Clear()
        Row = 6 'Acc biomass
        List = New PointPairList()
        For Col As Integer = 3 To m_DataGrid.ColumnCount
            List.Add(CSng(m_DataGrid.Columns(Col - 1).HeaderText), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane1.AddCurve(CStr(m_DataGrid.Item(2 - 1, Row - 1).Value), _
          List, Color.Red, SymbolType.None)
        Row = 9 'Acc flow
        List = New PointPairList()
        For Col As Integer = 3 To m_DataGrid.ColumnCount
            List.Add(CSng(m_DataGrid.Columns(Col - 1).HeaderText), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane1.AddCurve(CStr(m_DataGrid.Item(2 - 1, Row - 1).Value), _
          List, Color.Blue, SymbolType.None)

        'Pane2
        Pane2.Title.Text = ""
        Pane2.XAxis.Title.Text = My.Resources.GRP_XLBL_YEAR
        Pane2.XAxis.Title.FontSpec.Size = 17
        Pane2.XAxis.MajorTic.IsOutside = False
        Pane2.XAxis.MinorTic.IsOutside = False
        Pane2.XAxis.Scale.Min = CSng(m_DataGrid.Columns(3 - 1).HeaderText)
        Pane2.XAxis.Scale.Max = CSng(m_DataGrid.Columns(m_DataGrid.ColumnCount - 1).HeaderText)
        Pane2.YAxis.Title.Text = ""
        Pane2.YAxis.Title.FontSpec.Size = 17
        Pane2.YAxis.MajorTic.IsOutside = False
        Pane2.YAxis.MinorTic.IsOutside = False
        Pane2.YAxis.MinorTic.IsAllTics = False
        Pane2.Legend.Position = LegendPos.TopCenter
        Pane2.Legend.FontSpec.Size = 15

        m_ZedGraph.MasterPane(1).CurveList.Clear()
        Row = 7 'Pred biomaa
        List = New PointPairList()
        For Col As Integer = 3 To m_DataGrid.ColumnCount
            List.Add(CSng(m_DataGrid.Columns(Col - 1).HeaderText), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane2.AddCurve(CStr(m_DataGrid.Item(2 - 1, Row - 1).Value), _
          List, Color.Red, SymbolType.Circle)
        Row = 10 'Pred flow
        List = New PointPairList()
        For Col As Integer = 3 To m_DataGrid.ColumnCount
            List.Add(CSng(m_DataGrid.Columns(Col - 1).HeaderText), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane2.AddCurve(CStr(m_DataGrid.Item(2 - 1, Row - 1).Value), _
          List, Color.Blue, SymbolType.Circle)
        Row = 11 'Total catch
        List = New PointPairList()
        For Col As Integer = 3 To m_DataGrid.ColumnCount
            List.Add(CSng(m_DataGrid.Columns(Col - 1).HeaderText), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane2.AddCurve(CStr(m_DataGrid.Item(2 - 1, Row - 1).Value), _
          List, Color.Black, SymbolType.None)
        Row = 12 'Pred catch
        List = New PointPairList()
        For Col As Integer = 3 To m_DataGrid.ColumnCount
            List.Add(CSng(m_DataGrid.Columns(Col - 1).HeaderText), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane2.AddCurve(CStr(m_DataGrid.Item(2 - 1, Row - 1).Value), _
          List, Color.Black, SymbolType.Circle)

        m_ZedGraph.AxisChange()
        m_ZedGraph.Refresh()

        Graphic = m_Form.CreateGraphics
        Panes.AxisChange(Graphic)
        Panes.SetLayout(Graphic, PaneLayout.SingleColumn)
    End Sub

    Private Sub PlotGeneral()
        Dim Panes As MasterPane = m_ZedGraph.MasterPane
        Dim Pane1 As GraphPane = New ZedGraph.GraphPane
        Dim List As PointPairList
        Dim Colors(55) As Color
        Dim Col As Integer
        Dim IsEmptyCell As Boolean
        Dim Graphic As Graphics

        Panes.PaneList.Clear()
        Panes.Add(Pane1)

        Pane1.Title.Text = ""
        Pane1.XAxis.Title.Text = My.Resources.GRP_XLBL_TRP_LVL
        Pane1.XAxis.Title.FontSpec.Size = 17
        Pane1.XAxis.MajorTic.IsOutside = False
        Pane1.XAxis.MinorTic.IsOutside = False
        Pane1.XAxis.Scale.Min = 2.0
        Pane1.XAxis.Scale.Max = 6.0
        Pane1.YAxis.Title.Text = m_TabPageText
        Pane1.YAxis.Title.FontSpec.Size = 17
        Pane1.YAxis.MajorTic.IsOutside = False
        Pane1.YAxis.MinorTic.IsOutside = False
        Pane1.YAxis.MinorTic.IsAllTics = False
        Pane1.Legend.Position = LegendPos.TopCenter
        Pane1.Legend.FontSpec.Size = 10

        Colors(1) = Color.Red
        Colors(2) = Color.Green
        Colors(3) = Color.Blue
        Colors(4) = Color.Yellow
        Colors(5) = Color.Purple
        Colors(6) = Color.Pink
        Colors(7) = Color.Black
        Colors(8) = Color.Orange
        Colors(9) = Color.Gray
        Colors(10) = Color.Magenta
        Colors(11) = Color.DarkBlue
        Colors(12) = Color.DarkCyan
        Colors(13) = Color.DarkGoldenrod
        Colors(14) = Color.DarkGray
        Colors(15) = Color.DarkGreen
        Colors(16) = Color.DarkKhaki
        Colors(17) = Color.DarkMagenta
        Colors(18) = Color.DarkOliveGreen
        Colors(19) = Color.DarkOrange
        Colors(20) = Color.DarkOrchid
        Colors(21) = Color.DarkRed
        Colors(22) = Color.DarkSalmon
        Colors(23) = Color.DarkSeaGreen
        Colors(24) = Color.DarkSlateBlue
        Colors(25) = Color.DarkSlateGray
        Colors(26) = Color.DarkTurquoise
        Colors(27) = Color.DarkViolet
        Colors(28) = Color.LightBlue
        Colors(29) = Color.LightCoral
        Colors(30) = Color.LightCyan
        Colors(31) = Color.LightGoldenrodYellow
        Colors(32) = Color.LightGray
        Colors(33) = Color.LightGreen
        Colors(34) = Color.LightPink
        Colors(35) = Color.LightSalmon
        Colors(36) = Color.LightSeaGreen
        Colors(37) = Color.LightSkyBlue
        Colors(38) = Color.LightSlateGray
        Colors(39) = Color.LightSteelBlue
        Colors(40) = Color.LightYellow
        Colors(41) = Color.Lime
        Colors(42) = Color.LimeGreen
        Colors(43) = Color.Linen
        Colors(44) = Color.Magenta
        Colors(45) = Color.Maroon
        Colors(46) = Color.MediumAquamarine
        Colors(47) = Color.MediumBlue
        Colors(48) = Color.MediumOrchid
        Colors(49) = Color.MediumPurple
        Colors(50) = Color.MediumSeaGreen
        Colors(51) = Color.MediumSlateBlue
        Colors(52) = Color.MediumSpringGreen
        Colors(53) = Color.MediumTurquoise
        Colors(54) = Color.MediumVioletRed
        Colors(55) = Color.MidnightBlue

        If m_DataGrid.SelectedColumns.Count = 0 Then
            For Col = 3 To m_DataGrid.ColumnCount
                m_DataGrid.Columns(Col - 1).Selected = True
            Next
        End If

        m_ZedGraph.MasterPane(0).CurveList.Clear()
        For Col = 3 To m_DataGrid.ColumnCount
            If m_DataGrid.Columns(Col - 1).Selected = True Then
                IsEmptyCell = False
                For Row As Integer = 4 To m_DataGrid.RowCount
                    If m_DataGrid.Item(2 - 1, Row - 1).Value Is "" Or m_DataGrid.Item(Col - 1, Row - 1).Value Is "" Then
                        IsEmptyCell = True
                    End If
                Next

                If IsEmptyCell = False Then
                    List = New PointPairList()
                    For Row As Integer = 4 To m_DataGrid.RowCount
                        List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                          CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
                    Next
                    Pane1.AddCurve("Year " & CStr(m_DataGrid.Columns(Col - 1).HeaderText), _
                      List, Colors(Col - 1), SymbolType.None)
                    'Curve = Pane1.AddCurve("", List, Colors(Col - 1), SymbolType.None)
                Else
                    Exit For
                End If
            End If
        Next

        m_ZedGraph.AxisChange()
        m_ZedGraph.Refresh()

        Graphic = m_Form.CreateGraphics
        Panes.AxisChange(Graphic)
        Panes.SetLayout(Graphic, PaneLayout.SingleColumn)
    End Sub
#End Region 'Private methods

End Class
