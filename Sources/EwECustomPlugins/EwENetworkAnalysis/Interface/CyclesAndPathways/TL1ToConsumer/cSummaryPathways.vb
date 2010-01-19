#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

#End Region ' Imports 

Namespace TL1ToConsumer

    <CLSCompliant(False)> _
    Public Class cSummaryPathways
        Inherits cContentManager

        Public Sub New()
            '
        End Sub

        Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                        ByVal datagrid As DataGridView, _
                                        ByVal graph As ZedGraphControl, _
                                        ByVal plot As ucPlot, _
                                        ByVal toolstrip As ToolStrip) As Boolean
            Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip)
            Me.Grid.Visible = bSucces
            Return bSucces
        End Function

        Public Overrides Sub DisplayData()
            Dim strRowContent() As String

            SetUpGridColumn()

            'Set up grid rows
            Grid.RowHeadersVisible = False
            Grid.RowCount = 3
            Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
            Grid.Rows(0).Frozen = True
            Grid.Rows(0).Height = FIRST_ROW_HEIGHT

            ReDim strRowContent(Grid.Columns.Count)
            strRowContent(0) = My.Resources.COL_HDR_PARAM
            strRowContent(1) = My.Resources.COL_HDR_VALUE
            Grid.Rows(0).SetValues(strRowContent)
            Grid.Rows(0).Visible = True

            strRowContent(0) = My.Resources.ROW_HDR_TOTAL_NUM_PATH
            strRowContent(1) = CStr(NetworkManager.PathWays.Count)
            Grid.Rows(1).SetValues(strRowContent)
            Grid.Rows(1).Visible = True

            strRowContent(0) = My.Resources.ROW_HDR_MEAN_PATH_LEN
            If NetworkManager.PathWays.Count = 0 Then
                strRowContent(1) = My.Resources.ROW_HDR_NOT_APP
            Else
                strRowContent(1) = (NetworkManager.NumArrows / NetworkManager.PathWays.Count).ToString("F2")
            End If
            Grid.Rows(2).SetValues(strRowContent)
            Grid.Rows(2).Visible = True

            Grid.ClearSelection()

        End Sub

        Private Sub SetUpGridColumn()

            Grid.ColumnCount = 2

            SetGridColumnPropertyDefault(Grid)

            Grid.Columns(0).Frozen = True
            Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
            Grid.Columns(0).Width = 400

        End Sub

    End Class

End Namespace

