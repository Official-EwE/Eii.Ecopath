'==============================================================================
'
' $Log: cSummaryPathways.vb,v $
' Revision 1.9  2009/05/30 00:00:57  jeroens
' Toolstrip usage centralized
'
' Revision 1.8  2009/05/19 13:41:10  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.7  2009/05/02 01:51:26  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:43:07  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:08  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/16 00:11:59  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:58  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 23:14:04  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:50  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Namespace TL1ToPreyToConsumer

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
            Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, ToolStrip)
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

            Grid.ReadOnly = True
            Grid.ColumnCount = 2

            SetGridColumnPropertyDefault(Grid)

            Grid.Columns(0).Frozen = True
            Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
            Grid.Columns(0).Width = 400

        End Sub

    End Class

End Namespace

