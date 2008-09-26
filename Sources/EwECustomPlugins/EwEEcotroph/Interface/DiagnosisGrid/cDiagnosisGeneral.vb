'==============================================================================
'
' $Log: cDiagnosisGeneral.vb,v $
' Revision 1.1  2008/09/26 07:30:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cDiagnosisGeneral

#Region "Private field"
    Private Const NUM_COL_EXCLUDE_ROW_HEADER As Integer = 11
#End Region 'Private field

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip, ByVal EffortMultiplierType As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip, EffortMultiplierType)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal TabPgText As String, ByVal DataGrid As DataGridView, ByVal Diagnosis As UserInterface.cDiagnosis)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Diagnosis.m_EcotrophManager)
        SetUpGridRowPropertyDefault(DataGrid, Diagnosis.m_EcotrophManager)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid)
        SetUpGridRow(Diagnosis.m_Tree.SelectedNode.Text, Diagnosis.m_PanelToolStrip, TabPgText, DataGrid, Diagnosis.m_EcotrophManager)
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

    Private Shared Sub SetUpToolStrip(ByVal PanelToolStrip As Panel, ByVal EffortMultiplierType As String)
        Dim ToolStp As ToolStrip
        Dim ToolStpBtnCal As ToolStripButton
        Dim ToolStpBtnPlot As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpBtnCal = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)
        ToolStpBtnPlot = CType(ToolStp.Items("tsbtnPlot"), ToolStripButton)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)

        ToolStp.Visible = False

        Select Case EffortMultiplierType
            Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                ToolStpBtnCal.Text = My.Resources.BTN_CALCULATE
                ToolStpBtnCal.Visible = True
        End Select
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

    Private Shared Sub SetUpGridRowPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.RowCount = EcotrophManager.DiagnosisFlow.GetUpperBound(0) + 1
        cUtility.SetGridRowPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridCellPropertyDefault(ByVal DataGrid As DataGridView)
        cUtility.SetGridCellPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridColumn(ByVal DataGrid As DataGridView)
        DataGrid.ColumnHeadersVisible = False

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH
    End Sub

    Private Shared Sub SetUpGridRow(ByVal EffortMultiplierType As String, ByVal PanelToolStrip As Panel, _
      ByVal TabPgText As String, ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim RowContent() As String
        Dim CellStyle As DataGridViewCellStyle
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False
        Select Case EffortMultiplierType
            Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                'Set up Effort Multiplier row
                RowContent(0) = ""
                RowContent(1) = My.Resources.CELL_EFF_MTPLR_TRP_LVL
                For Col As Integer = 1 To EcotrophManager.DiagnosisEffortMultiplier.GetUpperBound(0)
                    RowContent(Col + 1) = EcotrophManager.DiagnosisEffortMultiplier(Col).ToString("#0.#")
                Next
                DataGrid.Rows(0).SetValues(RowContent)
                DataGrid.Rows(0).Height = cUtility.EFF_MTPLR_TRP_LVLOUT_ROW_HEIGHT
                DataGrid.Rows(0).Visible = True
                DataGrid.Rows(0).Frozen = True
                CellStyle = New DataGridViewCellStyle
                CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DataGrid.Item(1, 0).Style = CellStyle
                CellStyle = New DataGridViewCellStyle
                CellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
                For Col As Integer = 1 To EcotrophManager.DiagnosisEffortMultiplier.GetUpperBound(0)
                    DataGrid.Item(Col + 1, 0).Style = CellStyle
                Next

                SetUpAryRow(TabPgText, DataGrid, EcotrophManager)
            Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                'Set up Effort Multiplier row
                RowContent(0) = ""
                RowContent(1) = My.Resources.CELL_EFF_MTPLR_TRP_LVL
                For Col As Integer = 1 To EcotrophManager.InputData.EffortMultiplier.GetUpperBound(0)
                    RowContent(Col + 1) = EcotrophManager.InputData.EffortMultiplier(Col).ToString("#0.#")
                Next
                DataGrid.Rows(0).SetValues(RowContent)
                DataGrid.Rows(0).Height = cUtility.EFF_MTPLR_TRP_LVLOUT_ROW_HEIGHT
                DataGrid.Rows(0).Visible = True
                DataGrid.Rows(0).Frozen = True
                CellStyle = New DataGridViewCellStyle
                CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DataGrid.Item(1, 0).Style = CellStyle
                CellStyle = New DataGridViewCellStyle
                CellStyle.BackColor = Drawing.Color.LightGreen
                For Col As Integer = 1 To EcotrophManager.InputData.EffortMultiplier.GetUpperBound(0)
                    DataGrid.Item(Col + 1, 0).Style = CellStyle
                Next
                DataGrid.ReadOnly = False
                DataGrid.Item(0, 0).ReadOnly = True
                DataGrid.Item(1, 0).ReadOnly = True
                For Row As Integer = 2 To DataGrid.RowCount
                    DataGrid.Rows(Row - 1).ReadOnly = True
                Next

                SetUpAryRow(TabPgText, DataGrid, EcotrophManager)
        End Select
        DataGrid.ClearSelection()
    End Sub

    Private Shared Sub SetUpAryRow(ByVal TabPgText As String, ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim RowContent() As String
        Dim TLOut As Single
        ReDim RowContent(DataGrid.Columns.Count)

        TLOut = 2
        For Row As Integer = 1 To EcotrophManager.DiagnosisFlow.GetUpperBound(0)
            RowContent(0) = CStr(Row)
            If Row = 1 Then
                RowContent(1) = "1"
            Else
                RowContent(1) = TLOut.ToString("0.#")
            End If
            Select Case TabPgText
                Case My.Resources.TAB_PROD
                    For Col As Integer = 1 To EcotrophManager.DiagnosisFlow.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DiagnosisFlow(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DiagnosisFlow(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_BIOMASS
                    For Col As Integer = 1 To EcotrophManager.DiagnosisBiomass.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DiagnosisBiomass(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DiagnosisBiomass(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_KINETIC
                    For Col As Integer = 1 To EcotrophManager.DiagnosisKinetic.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DiagnosisKinetic(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DiagnosisKinetic(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_ACCESS_PROD
                    For Col As Integer = 1 To EcotrophManager.DiagnosisAccessFlow.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DiagnosisAccessFlow(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DiagnosisAccessFlow(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_ACCESS_BIOMASS
                    For Col As Integer = 1 To EcotrophManager.DiagnosisAccessBiomass.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DiagnosisAccessBiomass(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DiagnosisAccessBiomass(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_CATCHES
                    For Col As Integer = 1 To EcotrophManager.DiagnosisCatches.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DiagnosisCatches(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DiagnosisCatches(Row, Col)).ToString("F4")
                        End If
                    Next
            End Select

            DataGrid.Rows(Row - 1 + 1).SetValues(RowContent)
            DataGrid.Rows(Row - 1 + 1).Visible = True
            If Row > 1 Then TLOut = CSng(TLOut + 0.1)
        Next
    End Sub
#End Region 'Helper methods

End Class
