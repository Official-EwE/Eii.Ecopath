#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cCatchByTrophicLevel
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip, _
                                     ByVal uic As cUIContext) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, uic)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String
        Dim core As cCore = Me.UIContext.Core
        Dim bShowItem As Boolean = True
        Dim CatchGroupsShown() As Single

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nTrophicLevels + 1
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        'Calculate non-hidden data
        ReDim CatchGroupsShown(NetworkManager.nGroups)
        For i As Integer = 1 To NetworkManager.nGroups
            ' bShowItem = sg.GroupVisible(i)
            If bShowItem Then
                For j As Integer = 1 To NetworkManager.nTrophicLevels
                    If NetworkManager.RelativeFlow(i, j) = 0 Then
                    Else
                        If i <= core.nLivingGroups Then
                            CatchGroupsShown(j) = CatchGroupsShown(j) + NetworkManager.RelativeFlow(i, j) * NetworkManager.CatchByGroup(i)
                        End If
                    End If
                Next
            End If
        Next

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_TRP_LVL
        strRowContent(1) = My.Resources.COL_HDR_TOTAL_TKM2YR
        strRowContent(2) = My.Resources.COL_HDR_NONHIDDEN
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = NetworkManager.nTrophicLevels To 1 Step -1
            strRowContent(0) = cStringUtils.ToRoman(i)
            strRowContent(1) = Me.StyleGuide.FormatNumber(NetworkManager.CatchByTrophicLevel(i))
            strRowContent(2) = Me.StyleGuide.FormatNumber(CatchGroupsShown(i))
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).SetValues(strRowContent)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        'DataGrid.RowCount = 1
        Grid.ColumnCount = 3

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).Frozen = True
        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

    End Sub

End Class
