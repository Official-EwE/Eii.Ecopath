'==============================================================================
'
' $Log: cTransposeCatches.vb,v $
' Revision 1.1  2008/09/26 07:30:41  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cTransposeCatches

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip, ByVal Transpose As UserInterface.cTranspose, ByVal Algor As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp, Algor)
        SetUpToolStrip(PanelToolStrip, Transpose.m_EcotrophManager, Algor)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal DataGrid As DataGridView, ByVal Transpose As UserInterface.cTranspose)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Transpose.m_EcotrophManager)
        SetUpGridRowPropertyDefault(DataGrid, Transpose.m_EcotrophManager)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid, Transpose.m_EcotrophManager)
        SetUpGridRow(DataGrid, Transpose.m_EcotrophManager)
        Cursor.Current = Cursors.Default
    End Sub
#End Region 'Public methods

#Region "Helper methods"
    Private Shared Sub SetUpToolStripPropertyDefault(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
        ByVal ToolStp As ToolStrip, ByVal Algor As String)
        cUtility.RemoveToolStrip(PanelToolStrip, PanelTabCntl)
        cUtility.AddToolStrip(PanelToolStrip, ToolStp)
        cUtility.SetToolStripPropertyDefault(PanelToolStrip)
    End Sub

    Private Shared Sub SetUpToolStrip(ByVal PanelToolStrip As Panel, ByVal EcotrophManager As cEcotrophManager, _
      ByVal Algor As String)
        Dim ToolStp As ToolStrip
        Dim ToolStpBtn As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpBtn = CType(ToolStp.Items("tsbtnPlot"), ToolStripButton)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)

        ToolStp.Visible = False
        ToolStpBtn.Text = My.Resources.BTN_PLOT
        ToolStpBtn.Visible = True
        ToolStpSep.Visible = True
        ToolStp.Refresh()
        ToolStp.Visible = True
        ToolStp.Update()
    End Sub

    Private Shared Sub SetUpGridColumnPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect 'relax this condition to add columns
        DataGrid.ColumnCount = EcotrophManager.TransposeCatchSumGp.GetUpperBound(1) + 2
        cUtility.SetGridColumnPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridRowPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.RowCount = EcotrophManager.TransposeCatchSumGp.GetUpperBound(0)
        cUtility.SetGridRowPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridCellPropertyDefault(ByVal DataGrid As DataGridView) ', ByVal EcotrophManager As cEcotrophManager)
        'Dim CellStyle As DataGridViewCellStyle

        'CellStyle = New DataGridViewCellStyle
        'CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight '???
        'DataGrid.Item(1, 0).Style = CellStyle
        'DataGrid.Item(1, 1).Style = CellStyle
        'CellStyle = New DataGridViewCellStyle
        'CellStyle.BackColor = Drawing.Color.White
        'For Col As Integer = 1 To EcotrophManager.AccessBiomass.GetUpperBound(1)
        '    DataGrid.Item(Col + 1, 0).Style = CellStyle
        '    DataGrid.Item(Col + 1, 1).Style = CellStyle
        'Next
        cUtility.SetGridCellPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridColumn(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_TRP_LVL
        For Col As Integer = 1 To EcotrophManager.EcopathData.NumFleet
            DataGrid.Columns(Col + 1).HeaderText = EcotrophManager.EcopathData.FleetName(Col)
        Next
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim RowContent() As String
        Dim TLOut As Single
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False

        TLOut = 2
        For Row As Integer = 1 To EcotrophManager.TransposeCatchSumGp.GetUpperBound(0)
            RowContent(0) = CStr(Row)
            If Row = 1 Then
                RowContent(1) = "1"
            Else
                RowContent(1) = TLOut.ToString("0.#")
            End If
            For Col As Integer = 1 To EcotrophManager.TransposeCatchSumGp.GetUpperBound(1)
                If Single.IsNaN(EcotrophManager.TransposeCatchSumGp(Row, Col)) Then
                    RowContent(Col + 1) = ""
                Else
                    RowContent(Col + 1) = (EcotrophManager.TransposeCatchSumGp(Row, Col)).ToString("F4")
                End If
            Next
            DataGrid.Rows(Row - 1).SetValues(RowContent)
            DataGrid.Rows(Row - 1).Visible = True
            If Row > 1 Then TLOut = CSng(TLOut + 0.1)
        Next

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
