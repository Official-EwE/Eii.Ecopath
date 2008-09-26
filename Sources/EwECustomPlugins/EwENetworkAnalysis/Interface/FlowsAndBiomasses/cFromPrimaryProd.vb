'==============================================================================
'
' $Log: cFromPrimaryProd.vb,v $
' Revision 1.1  2008/09/26 07:30:53  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.14  2008/06/25 01:53:41  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.13  2008/06/24 18:08:38  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.12  2007/07/06 00:44:59  joeh
' Move hard coded strings to resource file
'
' Revision 1.11  2007/06/28 19:22:10  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.10  2007/06/22 19:12:46  joeh
' Modify GetInstance()
'
' Revision 1.9  2007/06/22 00:35:29  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.8  2007/06/21 00:14:39  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.7  2007/06/20 18:13:58  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cFromPrimaryProd
    Private Shared m_FromPrimaryProdInstance As cFromPrimaryProd

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cFromPrimaryProd
        m_Panel = Panel

        If m_FromPrimaryProdInstance Is Nothing Then m_FromPrimaryProdInstance = New cFromPrimaryProd(NetworkManager, Panel)
        Return m_FromPrimaryProdInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_Panel = Panel
    End Sub

    Public Sub DisplayData()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strRowContent() As String
        Dim sngSumVariable() As Single

        Cursor.Current = Cursors.WaitCursor
        RemoveToolStrip()

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nTrophicLevels + 1

        ReDim strRowContent(DataGrid.Columns.Count)
        ReDim sngSumVariable(DataGrid.Columns.Count)
        For i As Integer = m_NetworkManager.nTrophicLevels To 1 Step -1
            'strRowContent(0) = CStr(i)
            strRowContent(0) = CRoman(i)
            If i = 1 Then
                strRowContent(1) = m_NetworkManager.PPImport(i).ToString("F4")
                sngSumVariable(1) = sngSumVariable(1) + m_NetworkManager.PPImport(i)
            Else
                strRowContent(1) = ""
            End If
            strRowContent(2) = m_NetworkManager.PPConsByPred(i).ToString("F4")
            sngSumVariable(2) = sngSumVariable(2) + m_NetworkManager.PPConsByPred(i)
            strRowContent(3) = m_NetworkManager.PPExport(i).ToString("F4")
            sngSumVariable(3) = sngSumVariable(3) + m_NetworkManager.PPExport(i)
            strRowContent(4) = m_NetworkManager.PPToDetritus(i).ToString("F4")
            sngSumVariable(4) = sngSumVariable(4) + m_NetworkManager.PPToDetritus(i)
            strRowContent(5) = m_NetworkManager.PPRespiration(i).ToString("F4")
            sngSumVariable(5) = sngSumVariable(5) + m_NetworkManager.PPRespiration(i)
            strRowContent(6) = m_NetworkManager.PPThroughtput(i).ToString("F4")
            sngSumVariable(6) = sngSumVariable(6) + m_NetworkManager.PPThroughtput(i)
            DataGrid.Rows(m_NetworkManager.nTrophicLevels - i).SetValues(strRowContent)
            DataGrid.Rows(m_NetworkManager.nTrophicLevels - i).Visible = True
        Next

        strRowContent(0) = My.Resources.ROW_HDR_SUM
        For i As Integer = 1 To DataGrid.Columns.Count - 1
            strRowContent(i) = sngSumVariable(i).ToString("F4")
        Next
        DataGrid.Rows(DataGrid.RowCount - 1).SetValues(strRowContent)
        DataGrid.Rows(DataGrid.RowCount - 1).Visible = True
        DataGrid.ClearSelection()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub SetUpGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As Windows.Forms.TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), Windows.Forms.TableLayoutPanel)

        LogoPanel.Visible = False
        GraphPane.Visible = False
        DataGrid.ReadOnly = True
        DataGrid.Visible = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 7

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

        DataGrid.Columns(0).HeaderText = My.Resources.COL_HDR_TRP_LVL_FLOW
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_IMPORT
        DataGrid.Columns(2).HeaderText = My.Resources.COL_HDR_CONSUM_PREDAT
        DataGrid.Columns(3).HeaderText = My.Resources.COL_HDR_EXPORT
        DataGrid.Columns(4).HeaderText = My.Resources.COL_HDR_FLOW_DET
        DataGrid.Columns(5).HeaderText = My.Resources.COL_HDR_RESP
        DataGrid.Columns(6).HeaderText = My.Resources.COL_HDR_THROUGHPUT
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = Windows.Forms.DockStyle.Fill
        End If
    End Sub

End Class
