'==============================================================================
'
' $Log: cTransposeBiomass.vb,v $
' Revision 1.1  2008/09/26 07:30:41  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cTransposeBiomass

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip, ByVal Transpose As UserInterface.cTranspose, ByVal Algor As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip, Transpose.m_EcotrophManager, Algor)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal DataGrid As DataGridView, ByVal Transpose As UserInterface.cTranspose, _
      ByVal Algor As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Transpose.m_EcotrophManager)
        SetUpGridRowPropertyDefault(DataGrid, Transpose.m_EcotrophManager, Algor)
        SetUpGridCellPropertyDefault(DataGrid, Transpose.m_EcotrophManager)

        SetUpGridColumn(DataGrid, Transpose.m_EcotrophManager, Algor)
        SetUpGridRow(DataGrid, Transpose.m_EcotrophManager, Algor)
        Cursor.Current = Cursors.Default
    End Sub
#End Region 'Public methods

#Region "Helper methods"
    Private Shared Sub SetUpToolStripPropertyDefault(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
        ByVal ToolStp As ToolStrip)
        cUtility.RemoveToolStrip(PanelToolStrip, PanelTabCntl)
        cUtility.AddToolStrip(PanelToolStrip, ToolStp)
        cUtility.SetToolStripPropertyDefault(PanelToolStrip)
    End Sub

    Private Shared Sub SetUpToolStrip(ByVal PanelToolStrip As Panel, ByVal EcotrophManager As cEcotrophManager, _
     ByVal Algor As String)
        Dim ToolStp As ToolStrip
        Dim ToolStpBtnCal As ToolStripButton
        Dim ToolStpBtnPlot As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator
        Dim ToolStpLblSmoothFactor As ToolStripLabel
        Dim ToolStpTxtBoxSmoothFactor As ToolStripTextBox

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)
        ToolStpBtnPlot = CType(ToolStp.Items("tsbtnPlot"), ToolStripButton)
        ToolStpBtnPlot.Text = My.Resources.BTN_PLOT
        ToolStpBtnPlot.Visible = True
        ToolStp.Visible = False
        ToolStpSep.Visible = True
        Select Case Algor
            Case My.Resources.TREE_NODE_AUTO_SMOOTH
                ToolStpBtnCal = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)
                ToolStpLblSmoothFactor = CType(ToolStp.Items("tslblSmoothFactor"), ToolStripLabel)
                ToolStpTxtBoxSmoothFactor = CType(ToolStp.Items("tstbxSmoothFactor"), ToolStripTextBox)

                ToolStpBtnCal.Text = My.Resources.BTN_CALCULATE
                ToolStpBtnCal.Visible = True
                ToolStpLblSmoothFactor.Text = My.Resources.LBL_SMOOTH_FACTOR
                ToolStpLblSmoothFactor.Visible = True
                ToolStpTxtBoxSmoothFactor.Text = CStr(EcotrophManager.InputData.SmoothFactor)
                ToolStpTxtBoxSmoothFactor.Visible = True
            Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                ToolStpBtnCal = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)

                ToolStpBtnCal.Text = My.Resources.BTN_CALCULATE
                ToolStpBtnCal.Visible = True
            Case My.Resources.TREE_NODE_OMNI_IDX
                '
        End Select
        ToolStp.Refresh()
        ToolStp.Visible = True
        ToolStp.Update()
    End Sub

    Private Shared Sub SetUpGridColumnPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect 'relax this condition to add columns
        DataGrid.ColumnCount = EcotrophManager.TransposeBiomass.GetUpperBound(1) + 2
        cUtility.SetGridColumnPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridRowPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal Algor As String)
        Select Case Algor
            Case My.Resources.TREE_NODE_AUTO_SMOOTH
                DataGrid.RowCount = EcotrophManager.TransposeBiomass.GetUpperBound(0)
            Case My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                DataGrid.RowCount = EcotrophManager.TransposeBiomass.GetUpperBound(0) + 1
        End Select
        cUtility.SetGridRowPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridCellPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim CellStyle As DataGridViewCellStyle

        CellStyle = New DataGridViewCellStyle
        CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Item(1, 0).Style = CellStyle
        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.White
        For Col As Integer = 1 To EcotrophManager.TransposeBiomass.GetUpperBound(1)
            DataGrid.Item(Col + 1, 0).Style = CellStyle
        Next
    End Sub

    Private Shared Sub SetUpGridColumn(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal Algor As String)
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

        DataGrid.Columns(0).HeaderText = ""
        Select Case Algor
            Case My.Resources.TREE_NODE_AUTO_SMOOTH
                DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_GRP_NAMETL_TRP_LVL
                For Col As Integer = 1 To EcotrophManager.EcopathData.NumGroups
                    DataGrid.Columns(Col + 1).HeaderText = EcotrophManager.EcopathData.GroupName(Col) & _
                      Chr(10) & " (" & EcotrophManager.TLTuncated(Col) & ")"
                Next
            Case My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_GRP_NAMETL
                For Col As Integer = 1 To EcotrophManager.EcopathData.NumLiving
                    DataGrid.Columns(Col + 1).HeaderText = EcotrophManager.EcopathData.GroupName(Col) & _
                      Chr(10) & " (" & EcotrophManager.TLTuncated(Col) & ")"
                Next
        End Select
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal Algor As String)
        Dim RowContent() As String
        Dim CellStyle As DataGridViewCellStyle
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False
        Select Case Algor
            Case My.Resources.TREE_NODE_AUTO_SMOOTH ', My.Resources.TREE_NODE_OMIN_IDX
                SetUpAryRow(DataGrid, EcotrophManager, Algor)
            Case My.Resources.TREE_NODE_OMNI_IDX
                'Set up Sigma row
                RowContent(0) = ""
                RowContent(1) = My.Resources.CELL_SIGMA_TRP_LVL
                For Col As Integer = 1 To EcotrophManager.TransposeBiomass.GetUpperBound(1)
                    RowContent(Col + 1) = EcotrophManager.EcopathData.BQB(Col).ToString("0.####")
                Next
                DataGrid.Rows(0).SetValues(RowContent)
                DataGrid.Rows(0).Height = cUtility.SIGMA_TRP_LVLOUT_ROW_HEIGHT
                DataGrid.Rows(0).Visible = True
                DataGrid.Rows(0).Frozen = True
                CellStyle = New DataGridViewCellStyle
                CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DataGrid.Item(1, 0).Style = CellStyle
                CellStyle = New DataGridViewCellStyle
                CellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
                For Col As Integer = 1 To EcotrophManager.TransposeBiomass.GetUpperBound(1)
                    DataGrid.Item(Col + 1, 0).Style = CellStyle
                Next

                SetUpAryRow(DataGrid, EcotrophManager, Algor)
            Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                'Set up Sigma row
                RowContent(0) = ""
                RowContent(1) = My.Resources.CELL_SIGMA_TRP_LVL
                For Col As Integer = 1 To EcotrophManager.TransposeBiomass.GetUpperBound(1)
                    RowContent(Col + 1) = EcotrophManager.InputData.Sigma(Col).ToString("0.####")
                Next
                DataGrid.Rows(0).SetValues(RowContent)
                DataGrid.Rows(0).Height = cUtility.SIGMA_TRP_LVLOUT_ROW_HEIGHT
                DataGrid.Rows(0).Visible = True
                DataGrid.Rows(0).Frozen = True
                CellStyle = New DataGridViewCellStyle
                CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DataGrid.Item(1, 0).Style = CellStyle
                CellStyle = New DataGridViewCellStyle
                CellStyle.BackColor = Drawing.Color.lightgreen
                For Col As Integer = 1 To EcotrophManager.TransposeBiomass.GetUpperBound(1)
                    DataGrid.Item(Col + 1, 0).Style = CellStyle
                Next
                DataGrid.ReadOnly = False
                DataGrid.Item(0, 0).ReadOnly = True
                DataGrid.Item(1, 0).ReadOnly = True
                For Row As Integer = 2 To DataGrid.RowCount
                    DataGrid.Rows(Row - 1).ReadOnly = True
                Next

                SetUpAryRow(DataGrid, EcotrophManager, Algor)
        End Select
        DataGrid.ClearSelection()
    End Sub

    Private Shared Sub SetUpAryRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal Algor As String)
        Dim RowContent() As String
        Dim TLOut As Single
        ReDim RowContent(DataGrid.Columns.Count)

        TLOut = 2
        For Row As Integer = 1 To EcotrophManager.TransposeBiomass.GetUpperBound(0)
            RowContent(0) = CStr(Row)
            If Row = 1 Then
                RowContent(1) = "1"
            Else
                RowContent(1) = TLOut.ToString("0.#")
            End If
            For Col As Integer = 1 To EcotrophManager.TransposeBiomass.GetUpperBound(1)
                If Single.IsNaN(EcotrophManager.TransposeBiomass(Row, Col)) Then
                    RowContent(Col + 1) = ""
                Else
                    RowContent(Col + 1) = (EcotrophManager.TransposeBiomass(Row, Col)).ToString("F4")
                End If
            Next
            Select Case Algor
                Case My.Resources.TREE_NODE_AUTO_SMOOTH
                    DataGrid.Rows(Row - 1).SetValues(RowContent)
                    DataGrid.Rows(Row - 1).Visible = True
                Case My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                    DataGrid.Rows(Row - 1 + 1).SetValues(RowContent)
                    DataGrid.Rows(Row - 1 + 1).Visible = True
            End Select
            If Row > 1 Then TLOut = CSng(TLOut + 0.1)
        Next
    End Sub

#End Region 'Helper methods

End Class