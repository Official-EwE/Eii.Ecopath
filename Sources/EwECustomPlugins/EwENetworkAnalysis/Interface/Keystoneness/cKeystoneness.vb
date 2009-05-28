'==============================================================================
'
' $Log: cKeystoneness.vb,v $
' Revision 1.2  2009/05/28 02:28:32  jeroens
' Added scaled impact column
'
' Revision 1.1  2009/05/28 02:12:02  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cKeystoneness
    Inherits cContentManager

    Public Sub New()
        ' Just needs main network to run
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                    ByVal datagrid As DataGridView, _
                                    ByVal graph As ZedGraphControl, _
                                    ByVal plot As ucPlot) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()
        Dim strRowContent() As String

        SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nLivingGroups + 1
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        strRowContent(2) = "Keystone index"
        strRowContent(3) = "Scaled impact"
        Me.Grid.Rows(0).SetValues(strRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.nLivingGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = Me.NetworkManager.GroupName(i)
            strRowContent(2) = Me.NetworkManager.KeystoneIndex(i).ToString("F4")
            strRowContent(3) = Me.NetworkManager.ScaledImpact(i).ToString("F4")
            Me.Grid.Rows(i).SetValues(strRowContent)
            Me.Grid.Rows(i).Visible = True
        Next

        Me.Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        Me.Graph.Visible = False
        Me.Grid.ReadOnly = True
        Me.Grid.Visible = True
        Me.Grid.ColumnCount = 4

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).Width = ID_COL_WIDTH

        Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Grid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(1).Frozen = True
        Grid.Columns(1).Width = GRP_NAME_COL_WIDTH

    End Sub

End Class
