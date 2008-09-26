'==============================================================================
'
' $Log: cDiagnosisSummary.vb,v $
' Revision 1.1  2008/09/26 07:30:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cDiagnosisSummary

#Region "Private field"
    Private Const NUM_COL_EXCLUDE_ROW_HEADER As Integer = 11
    Private Const NUM_ROW_EXCLUDE_COL_HEADER As Integer = 22
#End Region 'Private field

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal DataGrid As DataGridView, ByVal Diagnosis As UserInterface.cDiagnosis, _
      ByVal EffortMultiplierType As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Diagnosis.m_EcotrophManager)
        SetUpGridRowPropertyDefault(DataGrid)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid, Diagnosis.m_EcotrophManager, EffortMultiplierType)
        SetUpGridRow(DataGrid, Diagnosis.m_EcotrophManager)
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

    Private Shared Sub SetUpGridColumnPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect 'relax this condition to add columns
        DataGrid.ColumnCount = NUM_COL_EXCLUDE_ROW_HEADER + 2
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
      ByVal EffortMultiplierType As String)
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_EFF_MTPLR_PARM
        Select Case EffortMultiplierType
            Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
                    DataGrid.Columns(Idx + 1).HeaderText = CStr(EcotrophManager.DiagnosisEffortMultiplier(Idx))
                Next Idx
            Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
                    DataGrid.Columns(Idx + 1).HeaderText = CStr(EcotrophManager.InputData.EffortMultiplier(Idx))
                Next Idx
        End Select
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim Row As Integer
        Dim RowContent() As String
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False

        Row = 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ABS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            RowContent(Idx + 1) = ""
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsTotalBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsTotalBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ACCESS_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsVulnerBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsVulnerBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsPredBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsPredBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_PROD
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsTotalFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsTotalFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ACCESS_PROD
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsVulnerFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsVulnerFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_PROD
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsPredFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsPredFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_CATCH
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsTotalCatch(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsTotalCatch(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_CATCH
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisAbsPredCatch(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisAbsPredCatch(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_REL
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            RowContent(Idx + 1) = ""
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelTotalBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelTotalBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ACCESS_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelVulnerBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelVulnerBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelPredBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelPredBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_PROD
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelTotalFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelTotalFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ACCESS_PROD
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelVulnerFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelVulnerFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_PROD
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelPredFlow(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelPredFlow(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_CATCH
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelTotalCatch(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelTotalCatch(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_PDR_CATCH
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisRelPredCatch(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisRelPredCatch(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TRP_LVL
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            RowContent(Idx + 1) = ""
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisTLTotalBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisTLTotalBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_ACCESS_BIOMASS
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisTLVulnerBiomass(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisTLVulnerBiomass(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        Row = Row + 1
        RowContent(0) = CStr(Row)
        RowContent(1) = My.Resources.ROW_HDR_TOT_CATCH
        For Idx As Integer = 1 To NUM_COL_EXCLUDE_ROW_HEADER
            If Single.IsNaN(EcotrophManager.DiagnosisTLTotalCatch(Idx)) Then
                RowContent(Idx + 1) = ""
            Else
                RowContent(Idx + 1) = EcotrophManager.DiagnosisTLTotalCatch(Idx).ToString("F4")
            End If
        Next
        DataGrid.Rows(Row - 1).SetValues(RowContent)
        DataGrid.Rows(Row - 1).Visible = True

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
