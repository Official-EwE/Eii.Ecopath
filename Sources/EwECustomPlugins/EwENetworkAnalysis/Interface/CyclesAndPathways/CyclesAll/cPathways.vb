'==============================================================================
'
' $Log: cPathways.vb,v $
' Revision 1.6  2009/05/01 17:43:01  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:06  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/16 00:11:56  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:55  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 23:44:07  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:48  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Namespace CyclesAll

    Public Class cPathways
        Inherits cContentManager

        Public Sub New()
        End Sub

        Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                      ByVal datagrid As DataGridView, _
                                      ByVal graph As ZedGraphControl, _
                                      ByVal plot As ucPlot)
            MyBase.Attach(manager, datagrid, graph, plot)
            Me.DataGrid.Visible = True
        End Sub

        Public Overrides Sub DisplayData()
            Dim strRowContent() As String

            SetUpGridColumn()

            'Set up grid rows
            DataGrid.RowHeadersVisible = False

            ReDim strRowContent(DataGrid.Columns.Count)
            'm_NetworkManager.FindPathwaysCyclesAll()
            If NetworkManager.PathWays.Count > 0 Then
                DataGrid.RowCount = NetworkManager.PathWays.Count + 1
                DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
                DataGrid.Rows(0).Frozen = True
                DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_CYC
                DataGrid.Rows(0).SetValues(strRowContent)
                DataGrid.Rows(0).Visible = True

                For intPathwayIndex As Integer = 0 To NetworkManager.PathWays.Count - 1
                    strRowContent(0) = CStr(intPathwayIndex + 1)
                    strRowContent(1) = CStr(NetworkManager.PathWays.Item(intPathwayIndex))
                    DataGrid.Rows(intPathwayIndex + 1).SetValues(strRowContent)
                    DataGrid.Rows(intPathwayIndex + 1).Visible = True
                Next
            Else
                DataGrid.RowCount = 2
                DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
                DataGrid.Rows(0).Frozen = True
                DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_CYC
                DataGrid.Rows(0).SetValues(strRowContent)
                DataGrid.Rows(0).Visible = True

                strRowContent(0) = My.Resources.ROW_HDR_NO_PATH_FOUND
                strRowContent(1) = ""
                DataGrid.Rows(1).SetValues(strRowContent)
                DataGrid.Rows(1).Visible = True
            End If
            DataGrid.ClearSelection()
        End Sub

        Private Sub SetUpGridColumn()

            DataGrid.ReadOnly = True
            DataGrid.ColumnCount = 2

            SetGridColumnPropertyDefault(DataGrid)

            DataGrid.Columns(0).Frozen = True
            DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

            DataGrid.Columns(1).Width = 660
            DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        End Sub

    End Class

End Namespace

