#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls

#End Region ' Imports 

Namespace TL1ToConsumer

    <CLSCompliant(False)> _
    Public Class cCyclesPathways
        Inherits cContentManager

        Public Sub New()
        End Sub

        Public Overrides Function PageTitle() As String
            Return "Cycles and pathways TL1 to consumer"
        End Function

        Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                        ByVal datagrid As DataGridView, _
                                        ByVal graph As ZedGraphControl, _
                                        ByVal plot As ucPlot, _
                                        ByVal toolstrip As ToolStrip, _
                                        ByVal uic As cUIContext) As Boolean
            Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, uic)
            Me.Grid.Visible = bSucces
            Me.Toolstrip.Visible = bSucces
            Me.ToolstripShowGroupSelections(My.Resources.LBL_PATH_TO, eGroupFilterTypes.Living)
            Return bSucces
        End Function

        Public Overrides Sub DisplayData()

            Grid.ReadOnly = True
            Grid.ColumnCount = 2

            SetGridColumnPropertyDefault(Grid)

            Grid.Columns(0).Frozen = True
            Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control

            Grid.Columns(1).Width = 660
            Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        End Sub

        Public Overrides Sub UpdateData(ByVal iSel1 As Integer, ByVal iSel2 As Integer)

            Dim strRowContent() As String

            Grid.RowHeadersVisible = False

            ReDim strRowContent(Grid.Columns.Count)
            NetworkManager.FindPathwaysToConsumer(iSel1)
            If NetworkManager.PathWays.Count > 0 Then
                Grid.RowCount = NetworkManager.PathWays.Count + 1
                Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
                Grid.Rows(0).Frozen = True
                Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_PATH_CONSUM
                Grid.Rows(0).SetValues(strRowContent)
                Grid.Rows(0).Visible = True

                For intPathwayIndex As Integer = 0 To NetworkManager.PathWays.Count - 1
                    strRowContent(0) = CStr(intPathwayIndex + 1)
                    strRowContent(1) = CStr(NetworkManager.PathWays.Item(intPathwayIndex))
                    Grid.Rows(intPathwayIndex + 1).SetValues(strRowContent)
                    Grid.Rows(intPathwayIndex + 1).Visible = True
                Next
            Else
                Grid.RowCount = 2
                Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
                Grid.Rows(0).Frozen = True
                Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_PATH_CONSUM
                Grid.Rows(0).SetValues(strRowContent)
                Grid.Rows(0).Visible = True

                strRowContent(0) = My.Resources.ROW_HDR_NO_PATH_FOUND
                strRowContent(1) = ""
                Grid.Rows(1).SetValues(strRowContent)
                Grid.Rows(1).Visible = True
            End If
            Grid.ClearSelection()
        End Sub

    End Class

End Namespace

