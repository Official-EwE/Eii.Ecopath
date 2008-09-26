'==============================================================================
'
' $Log: cDynamicsSummary.vb,v $
' Revision 1.1  2008/09/26 07:30:40  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cDynamicsSummary

#Region "Private field"
    Private Const NUM_ROW_EXCLUDE_COL_HEADER As Integer = 12
#End Region 'Private field

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal DataGrid As DataGridView, ByVal Dynamics As UserInterface.cDynamics, _
       ByVal CatchHistoryType As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Dynamics.m_EcotrophManager, CatchHistoryType)
        SetUpGridRowPropertyDefault(DataGrid)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid, Dynamics.m_EcotrophManager, CatchHistoryType)
        SetUpGridRow(DataGrid, Dynamics.m_EcotrophManager, CatchHistoryType)
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

    Private Shared Sub SetUpToolStrip(ByVal PanelToolStrip As Panel)
        Dim ToolStp As ToolStrip
        Dim ToolStpBtnPlot As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpBtnPlot = CType(ToolStp.Items("tsbtnPlot"), ToolStripButton)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)

        ToolStp.Visible = False

        ToolStpBtnPlot.Text = My.Resources.BTN_PLOT
        ToolStpBtnPlot.Visible = True
        ToolStpSep.Visible = True

        ToolStp.Refresh()
        ToolStp.Visible = True
        ToolStp.Update()
    End Sub

    Private Shared Sub SetUpGridColumnPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
       ByVal CatchHistoryType As String)
        DataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect 'relax this condition to add columns
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                DataGrid.ColumnCount = EcotrophManager.InputData.NumForecastYear + 2
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                DataGrid.ColumnCount = EcotrophManager.InputData.NumPastAnalysisYear + 2
        End Select
        cUtility.SetGridColumnPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridRowPropertyDefault(ByVal DataGrid As DataGridView)
        DataGrid.RowCount = NUM_ROW_EXCLUDE_COL_HEADER
        cUtility.SetGridRowPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridCellPropertyDefault(ByVal DataGrid As DataGridView)
        cUtility.SetGridCellPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridColumn(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal CatchHistoryType As String)
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_PARM
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                For Idx As Integer = 1 To DataGrid.ColumnCount - 2
                    DataGrid.Columns(Idx + 1).HeaderText = CStr(EcotrophManager.InputData.ReferenceYear + Idx - 1)
                Next Idx
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                For Idx As Integer = 1 To DataGrid.ColumnCount - 2
                    DataGrid.Columns(Idx + 1).HeaderText = CStr(EcotrophManager.InputData.PastAnalysisYear(Idx))
                Next Idx
        End Select
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal CatchHistoryType As String)
        Dim Row As Integer
        Dim RowContent() As String
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False

        Row = 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_EFF_MTPLR
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsEffortMultiplier(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsEffortMultiplier(Idx).ToString("F2")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_CATCH_MTPLR
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                For Idx As Integer = 1 To DataGrid.ColumnCount - 2
                    If Single.IsNaN(EcotrophManager.InputData.CatchMultiplier(Idx)) Then
                        RowContent(Idx + 1) = ""
                    Else
                        RowContent(Idx + 1) = EcotrophManager.InputData.CatchMultiplier(Idx).ToString("F2")
                    End If
                Next
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                For Idx As Integer = 1 To DataGrid.ColumnCount - 2
                    If Single.IsNaN(EcotrophManager.DynamicsCatchMultiplier(Idx)) Then
                        RowContent(Idx + 1) = ""
                    Else
                        RowContent(Idx + 1) = EcotrophManager.DynamicsCatchMultiplier(Idx).ToString("F2")
                    End If
                Next
        End Select
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_IDX_PP
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                For Idx As Integer = 1 To DataGrid.ColumnCount - 2
                    If Single.IsNaN(EcotrophManager.InputData.IndexPPForecast(Idx)) Then
                        RowContent(Idx + 1) = ""
                    Else
                        RowContent(Idx + 1) = EcotrophManager.InputData.IndexPPForecast(Idx).ToString("F2")
                    End If
                Next
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                For Idx As Integer = 1 To DataGrid.ColumnCount - 2
                    If Single.IsNaN(EcotrophManager.InputData.IndexPPPastAnalysis(Idx)) Then
                        RowContent(Idx + 1) = ""
                    Else
                        RowContent(Idx + 1) = EcotrophManager.InputData.IndexPPPastAnalysis(Idx).ToString("F2")
                    End If
                Next
        End Select
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = ""
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            RowContent(Idx + 1) = ""
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_BIOMASS
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryTotalBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryTotalBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ACCESS_BIOMASS
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryAccessBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryAccessBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_BIOMASS
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryPredBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryPredBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_PROD
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryTotalFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryTotalFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ACCESS_PROD
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryAccessFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryAccessFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_PROD
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryPredFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryPredFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_CATCH
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryTotalCatch(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryTotalCatch(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_CATCH
        For Idx As Integer = 1 To DataGrid.ColumnCount - 2
            If Single.IsNaN(EcotrophManager.DynamicsSryPredCatch(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DynamicsSryPredCatch(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
