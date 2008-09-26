Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports ZedGraph

Namespace Graph

    Public Class cTransposeCatch

#Region "Public fields"
        Public m_Form As Form
        Public m_NodeText As String
        Public m_TabPageText As String
        Public m_DataGrid As DataGridView
        Public m_ZedGraph As ZedGraphControl
#End Region 'Public fields

#Region "Public methods"
        Public Sub PlotGraph()
            Dim RowInitial As Integer
            Dim RowFinal As Integer

            Select Case m_TabPageText
                Case My.Resources.TAB_MAIN
                    PlotMain()
                Case My.Resources.TAB_ACCESS_BIOMASS
                    Select Case m_NodeText
                        Case My.Resources.TREE_NODE_AUTO_EMPIR_FUNCT
                            RowInitial = 3
                            RowFinal = 43
                        Case My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                            RowInitial = 4
                            RowFinal = 44
                    End Select
                    PlotGeneral(RowInitial, RowFinal)
                Case My.Resources.TAB_CATCHES
                    RowInitial = 2
                    RowFinal = 42
                    PlotGeneral(RowInitial, RowFinal)
                Case My.Resources.TAB_FLOW, My.Resources.TAB_BIOMASS
                    Select Case m_NodeText
                        Case My.Resources.TREE_NODE_AUTO_EMPIR_FUNCT
                            RowInitial = 2
                            RowFinal = 42
                            PlotGeneral(RowInitial, RowFinal)
                        Case My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                            RowInitial = 3
                            RowFinal = 43
                            PlotGeneral(RowInitial, RowFinal)
                    End Select
            End Select

            If m_TabPageText.Contains(My.Resources.TAB_CATCH) Then
                Select Case m_NodeText
                    Case My.Resources.TREE_NODE_AUTO_EMPIR_FUNCT
                        RowInitial = 2
                        RowFinal = 42
                    Case My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                        RowInitial = 3
                        RowFinal = 43
                End Select
                PlotGeneral(RowInitial, RowFinal)
            End If
        End Sub
#End Region 'Public methods

#Region "Private methods"
        Private Sub PlotMain()
            Dim Panes As MasterPane = m_ZedGraph.MasterPane
            Dim Pane1 As GraphPane = New ZedGraph.GraphPane
            Dim Pane2 As GraphPane = New ZedGraph.GraphPane
            Dim List As PointPairList
            Dim Curve As LineItem
            Dim Col As Integer
            Dim Graphic As Graphics

            Panes.PaneList.Clear()
            Panes.Add(Pane1)
            Panes.Add(Pane2)

            'Pane1
            Pane1.Title.Text = ""
            Pane1.XAxis.Title.Text = My.Resources.GRP_XLBL_TRP_LVL
            Pane1.XAxis.Title.FontSpec.Size = 17
            Pane1.XAxis.Scale.Min = 2.0
            Pane1.XAxis.Scale.Max = 6.0
            Pane1.YAxis.Title.Text = ""
            Pane1.YAxis.Title.FontSpec.Size = 17
            Pane1.YAxis.MinorTic.IsAllTics = False
            Pane1.YAxis.Type = AxisType.Log
            Pane1.Legend.Position = LegendPos.InsideTopRight
            Pane1.Legend.FontSpec.Size = 15

            m_ZedGraph.MasterPane(0).CurveList.Clear()
            Col = 3 'Biomass
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Red, SymbolType.None)
            Col = 4 'Acc biomass
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
             List, Color.Orange, SymbolType.None)
            Col = 5 'Flow
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Violet, SymbolType.None)
            Col = 6 'Kinetic
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Yellow, SymbolType.None)
            Col = 7 'Catches
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Curve = Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Black, SymbolType.Circle)
            Curve.Symbol.Fill = New Fill(Color.Black)

            'Pane2
            Pane2.Title.Text = ""
            Pane2.XAxis.Title.Text = My.Resources.GRP_XLBL_TRP_LVL
            Pane2.XAxis.Title.FontSpec.Size = 17
            Pane2.XAxis.Scale.Min = 2.0
            Pane2.XAxis.Scale.Max = 6.0
            Pane2.YAxis.Title.Text = ""
            Pane2.YAxis.Title.FontSpec.Size = 17
            Pane2.YAxis.MinorTic.IsAllTics = False
            Pane2.Legend.Position = LegendPos.InsideTopLeft
            Pane2.Legend.FontSpec.Size = 15

            m_ZedGraph.MasterPane(1).CurveList.Clear()
            Col = 8 'Fish loss rate
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Curve = Pane2.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Black, SymbolType.Square)
            Curve.Symbol.Fill = New Fill(Color.Black)
            Col = 9 'Acc fish loss rate
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))

            Next
            Pane2.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Black, SymbolType.None)
            Col = 11 'Fish mort
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Curve = Pane2.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Violet, SymbolType.Square)
            Curve.Symbol.Fill = New Fill(Color.Violet)
            Col = 12 'Acc fish mort
            List = New PointPairList()
            For Row As Integer = 2 To 42
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Pane2.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Violet, SymbolType.None)

            m_ZedGraph.AxisChange()
            m_ZedGraph.Refresh()

            Graphic = m_Form.CreateGraphics
            Panes.AxisChange(Graphic)
            Panes.SetLayout(Graphic, PaneLayout.SingleColumn)
        End Sub

        Private Sub PlotGeneral(ByVal RowInitial As Integer, ByVal RowFinal As Integer)
            Dim Panes As MasterPane = m_ZedGraph.MasterPane
            Dim Pane1 As GraphPane = New ZedGraph.GraphPane
            Dim List As PointPairList
            Dim Curve As LineItem
            Dim Colors(40) As Color
            Dim Col As Integer
            Dim IsEmptyColumn As Boolean
            Dim Graphic As Graphics

            Panes.PaneList.Clear()
            Panes.Add(Pane1)

            Pane1.Title.Text = ""
            Pane1.XAxis.Title.Text = My.Resources.GRP_XLBL_TRP_LVL
            Pane1.XAxis.Title.FontSpec.Size = 17
            Pane1.XAxis.Scale.Min = 2.0
            Pane1.XAxis.Scale.Max = 6.0
            Pane1.YAxis.Title.Text = ""
            Pane1.YAxis.Title.FontSpec.Size = 17
            Pane1.YAxis.MinorTic.IsAllTics = False
            Pane1.Legend.Position = LegendPos.Right
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

            If m_DataGrid.SelectedColumns.Count = 0 Then
                For Col = 3 To m_DataGrid.ColumnCount
                    m_DataGrid.Columns(Col - 1).Selected = True
                Next
            End If

            m_ZedGraph.MasterPane(0).CurveList.Clear()
            For Col = 3 To m_DataGrid.ColumnCount
                'For Col = m_DataGrid.ColumnCount To 3 Step -1
                If m_DataGrid.Columns(Col - 1).Selected = True Then
                    IsEmptyColumn = False
                    For Row As Integer = RowInitial To RowFinal
                        If m_DataGrid.Item(2 - 1, Row - 1).Value Is "" Or m_DataGrid.Item(Col - 1, Row - 1).Value Is "" Then
                            IsEmptyColumn = True
                        End If
                    Next

                    If IsEmptyColumn = False Then
                        List = New PointPairList()
                        For Row As Integer = RowInitial To RowFinal
                            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
                        Next
                        Curve = Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
                          List, Colors(Col - 1), SymbolType.None)
                        'Curve = Pane1.AddCurve("", List, Colors(Col - 1), SymbolType.None)
                        Curve.Line.Fill = New Fill(Colors(Col - 1))
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

End Namespace

