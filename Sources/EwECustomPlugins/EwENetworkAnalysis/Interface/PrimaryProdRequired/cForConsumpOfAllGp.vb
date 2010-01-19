#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cForConsumpOfAllGp
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
        Dim sngTotalPPRCons As Single

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = Me.NetworkManager.Core.nLivingGroups + 2
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        strRowContent(2) = My.Resources.COL_HDR_NUM_PATH
        strRowContent(3) = My.Resources.COL_HDR_TL
        strRowContent(4) = My.Resources.COL_HDR_PPR_PP
        strRowContent(5) = My.Resources.COL_HDR_PPR_DET
        strRowContent(6) = My.Resources.COL_HDR_PPR
        strRowContent(7) = My.Resources.COL_HDR_CONSUM
        strRowContent(8) = My.Resources.COL_HDR_PPR_COMSUM
        strRowContent(9) = My.Resources.COL_HDR_PPR_TOTAL_PP
        strRowContent(10) = My.Resources.COL_HDR_PPR_U_BIOMASS
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = NetworkManager.GroupName(i)
            strRowContent(2) = CStr(NetworkManager.NumerPaths(i))
            strRowContent(3) = NetworkManager.TrophicLevel(i).ToString("F2")
            strRowContent(4) = NetworkManager.PPRRequired(i).ToString("F2")
            strRowContent(5) = NetworkManager.PPRRequiredDet(i).ToString("F2")
            strRowContent(6) = NetworkManager.PPRRequiredSum(i).ToString("F2")
            strRowContent(7) = NetworkManager.PPRCons(i).ToString("F2")
            sngTotalPPRCons = sngTotalPPRCons + NetworkManager.PPRCons(i)
            If NetworkManager.PPRCons(i) > 0.0 Then
                strRowContent(8) = NetworkManager.PPROverCons(i).ToString("F2")
            Else
                strRowContent(8) = ""
            End If
            strRowContent(9) = NetworkManager.PPRTotPP(i).ToString("F2")
            If NetworkManager.TotalPrimaryProduction > 0.0 Then
                strRowContent(10) = NetworkManager.PPRU(i).ToString("F2")
            Else
                strRowContent(10) = ""
            End If
            Grid.Rows(i).SetValues(strRowContent)
            Grid.Rows(i).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next

        'Display total
        For i As Integer = 0 To Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        strRowContent(2) = CStr((NetworkManager.NumLivPath + NetworkManager.NumDetPath))
        strRowContent(7) = sngTotalPPRCons.ToString("F2")
        Grid.Rows(Grid.Rows.Count - 1).SetValues(strRowContent)
        Grid.Rows(Grid.Rows.Count - 1).Visible = True

        'Hide some rows
        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            If NetworkManager.PPRCons(i) <= 0.0 Or _
                NetworkManager.TotalPrimaryProduction <= 0.0 Then
                Grid.Rows(i).Visible = False
            End If
        Next
        Grid.ClearSelection()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub SetUpGridColumn()

        Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Grid.ColumnCount = 11

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(0).Frozen = True
        Grid.Columns(0).Width = ID_COL_WIDTH

        Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Grid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(1).Frozen = True
        Grid.Columns(1).Width = GRP_NAME_COL_WIDTH

    End Sub

End Class
