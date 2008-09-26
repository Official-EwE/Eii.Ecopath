'==============================================================================
'
' $Log: cCTSAGraph.vb,v $
' Revision 1.1  2008/09/26 07:30:42  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2008/06/05 19:43:47  joeh
' no message
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports ZedGraph

Public Class cCTSAGraph

#Region "Public fields"
    Public m_Form As Form
    Public m_NodeText As String
    Public m_TabPageText As String
    Public m_DataGrid As DataGridView
    Public m_ZedGraph As ZedGraphControl
    Public m_EcotrophManager As cEcotrophManager
#End Region 'Public fields

#Region "Public methods"
    Public Sub PlotGraph()
        PlotCalParam() 'm_TabPageText)
    End Sub
#End Region 'Public methods

#Region "Private methods"
    Private Sub PlotCalParam() 'ByVal TabPageText As String)
        Dim Panes As MasterPane = m_ZedGraph.MasterPane
        Dim Pane1 As GraphPane = New ZedGraph.GraphPane
        Dim Pane2 As GraphPane = New ZedGraph.GraphPane
        Dim Pane3 As GraphPane = New ZedGraph.GraphPane
        Dim List As PointPairList
        Dim Curve As LineItem
        Dim Col As Integer
        Dim Graphic As Graphics

        Panes.PaneList.Clear()
        Panes.Add(Pane1)
        Panes.Add(Pane2)
        Panes.Add(Pane3)

        'Pane1
        Pane1.Title.Text = ""
        Pane1.XAxis.Title.Text = My.Resources.GRP_XLBL_TRP_LVL
        Pane1.XAxis.Title.FontSpec.Size = 17
        Pane1.XAxis.MajorTic.IsOutside = False
        Pane1.XAxis.MinorTic.IsOutside = False
        Pane1.XAxis.Scale.Min = 2.0
        Pane1.XAxis.Scale.Max = 6.0
        Pane1.YAxis.Title.Text = ""
        Pane1.YAxis.Title.FontSpec.Size = 17
        Pane1.YAxis.MajorTic.IsOutside = False
        Pane1.YAxis.MinorTic.IsOutside = False
        Pane1.YAxis.MinorTic.IsAllTics = False
        Pane1.YAxis.Type = AxisType.Log
        Pane1.Legend.Position = LegendPos.TopCenter
        Pane1.Legend.FontSpec.Size = 15

        m_ZedGraph.MasterPane(0).CurveList.Clear()
        Col = 3 'Flow
        List = New PointPairList()
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Curve = Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
          List, Color.Blue, SymbolType.Square)
        Curve.Symbol.Fill = New Fill(Color.Blue)
        Col = 4 'Biomass
        List = New PointPairList()
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Curve = Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
         List, Color.Red, SymbolType.Square)
        Curve.Symbol.Fill = New Fill(Color.Red)
        'Virgin flow
        'If TabPageText = My.Resources.TAB_FWD_CAL Then
        If m_NodeText = My.Resources.TREE_NODE_FWD_CAL Then
            Col = 6
        Else
            Col = 7
        End If
        List = New PointPairList()
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
          List, Color.Blue, SymbolType.None)
        'Virgin biomass
        'If TabPageText = My.Resources.TAB_FWD_CAL Then
        If m_NodeText = My.Resources.TREE_NODE_FWD_CAL Then
            Col = 7
        Else
            Col = 8
        End If
        List = New PointPairList()
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane1.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
          List, Color.Red, SymbolType.None)
        'Catches
        List = New PointPairList
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              m_EcotrophManager.InputData.Catches(Row))
        Next
        Pane1.AddCurve("Catches", _
          List, Color.Black, SymbolType.None)

        'Pane2
        Pane2.Title.Text = ""
        Pane2.XAxis.Title.Text = My.Resources.GRP_XLBL_TRP_LVL
        Pane2.XAxis.Title.FontSpec.Size = 17
        Pane2.XAxis.MajorTic.IsOutside = False
        Pane2.XAxis.MinorTic.IsOutside = False
        Pane2.XAxis.Scale.Min = 2.0
        Pane2.XAxis.Scale.Max = 6.0
        Pane2.YAxis.Title.Text = ""
        Pane2.YAxis.Title.FontSpec.Size = 17
        Pane2.YAxis.MajorTic.IsOutside = False
        Pane2.YAxis.MinorTic.IsOutside = False
        Pane2.YAxis.MinorTic.IsAllTics = False
        Pane2.Legend.Position = LegendPos.TopCenter
        Pane2.Legend.FontSpec.Size = 15

        m_ZedGraph.MasterPane(1).CurveList.Clear()
        Col = 5 'Fish loss rate
        List = New PointPairList()
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Pane2.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
          List, Color.Pink, SymbolType.None)
        'If TabPageText = My.Resources.TAB_FWD_CAL Then
        If m_NodeText = My.Resources.TREE_NODE_FWD_CAL Then
            '
        Else
            Col = 6 'Acc fish mortality
            List = New PointPairList()
            For Row As Integer = 2 To 40
                List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
                  CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
            Next
            Pane2.AddCurve(m_DataGrid.Columns(Col - 1).HeaderText, _
              List, Color.Yellow, SymbolType.None)
        End If
        'Catches
        List = New PointPairList
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              m_EcotrophManager.InputData.Catches(Row))
        Next
        Pane2.AddCurve("Catches", _
          List, Color.Blue, SymbolType.None)

        'Pane3
        Pane3.Title.Text = ""
        Pane3.XAxis.Title.Text = My.Resources.GRP_XLBL_TRP_LVL
        Pane3.XAxis.Title.FontSpec.Size = 17
        Pane3.XAxis.MajorTic.IsOutside = False
        Pane3.XAxis.MinorTic.IsOutside = False
        Pane3.XAxis.Scale.Min = 2.0
        Pane3.XAxis.Scale.Max = 6.0
        Pane3.YAxis.Title.Text = ""
        Pane3.YAxis.Title.FontSpec.Size = 17
        Pane3.YAxis.MajorTic.IsOutside = False
        Pane3.YAxis.MinorTic.IsOutside = False
        Pane3.YAxis.MinorTic.IsAllTics = False
        Pane3.YAxis.Type = AxisType.Log
        Pane3.Legend.Position = LegendPos.TopCenter
        Pane3.Legend.FontSpec.Size = 15

        m_ZedGraph.MasterPane(2).CurveList.Clear()
        'Kinetic (recal)
        'If TabPageText = My.Resources.TAB_FWD_CAL Then
        If m_NodeText = My.Resources.TREE_NODE_FWD_CAL Then
            Col = 8
        Else
            Col = 9
        End If
        List = New PointPairList()
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              CSng(m_DataGrid.Item(Col - 1, Row - 1).Value))
        Next
        Curve = Pane3.AddCurve("Kinetic", _
          List, Color.Yellow, SymbolType.Square)
        Curve.Symbol.Fill = New Fill(Color.Yellow)
        'Virgin kinetic
        List = New PointPairList
        For Row As Integer = 2 To 40
            List.Add(CSng(m_DataGrid.Item(2 - 1, Row - 1).Value), _
              m_EcotrophManager.CTSAKinetic(Row))
        Next
        Pane3.AddCurve("Virgin kinetic", _
          List, Color.Yellow, SymbolType.None)

        m_ZedGraph.AxisChange()
        m_ZedGraph.Refresh()

        Graphic = m_Form.CreateGraphics
        Panes.AxisChange(Graphic)
        Panes.SetLayout(Graphic, PaneLayout.SingleColumn)
    End Sub
#End Region 'Private methods

End Class
