'==============================================================================
'
' $Log: cSummaryPathways.vb,v $
' Revision 1.7  2009/05/02 01:51:25  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:43:05  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:08  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/16 00:11:58  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:58  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 23:14:04  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:49  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.17  2008/06/25 01:53:42  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.16  2008/06/24 18:08:40  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.15  2007/06/28 19:20:18  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.14  2007/06/26 21:16:58  joeh
' Add wait cursor when set up grid
'
' Revision 1.13  2007/06/22 19:12:48  joeh
' Modify GetInstance()
'
' Revision 1.12  2007/06/22 00:35:31  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.11  2007/06/21 23:49:36  joeh
' Move hard coded strings into the resource file
'
' Revision 1.10  2007/06/21 00:14:40  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.9  2007/06/20 18:13:59  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Namespace TL1ToConsumer

    Public Class cSummaryPathways
        Inherits cContentManager

        Public Sub New()
            '
        End Sub

        Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                      ByVal datagrid As DataGridView, _
                                      ByVal graph As ZedGraphControl, _
                                      ByVal plot As ucPlot)
            MyBase.Attach(manager, datagrid, graph, plot)
            Me.Grid.Visible = True
        End Sub

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

