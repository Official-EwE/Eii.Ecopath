'==============================================================================
'
' $Log: cDynamicsGeneral.vb,v $
' Revision 1.1  2008/09/26 07:30:40  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cDynamicsGeneral

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip, ByVal Dynamics As UserInterface.cDynamics, ByVal CatchHistoryType As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip, Dynamics.m_EcotrophManager, CatchHistoryType)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal TabPgText As String, ByVal DataGrid As DataGridView, _
      ByVal Dynamics As UserInterface.cDynamics, ByVal CatchHistoryType As String)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Dynamics.m_EcotrophManager, CatchHistoryType)
        SetUpGridRowPropertyDefault(DataGrid, Dynamics.m_EcotrophManager)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid, Dynamics.m_EcotrophManager, CatchHistoryType)
        SetUpGridRow(Dynamics.m_Tree.SelectedNode.Text, Dynamics.m_PanelToolStrip, TabPgText, DataGrid, _
          Dynamics.m_EcotrophManager, CatchHistoryType)
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
      ByVal CatchHistoryType As String)
        Dim ToolStp As ToolStrip
        Dim ToolStpBtnCal As ToolStripButton
        Dim ToolStpBtnPlot As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator
        Dim ToolStpLblRefYear As ToolStripLabel
        Dim ToolStpTxtBoxRefYear As ToolStripTextBox
        Dim ToolStpLblNumYear As ToolStripLabel
        Dim ToolStpTxtBoxNumYear As ToolStripTextBox

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpBtnCal = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)
        ToolStpBtnPlot = CType(ToolStp.Items("tsbtnPlot"), ToolStripButton)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)
        ToolStpLblRefYear = CType(ToolStp.Items("tslblRefYear"), ToolStripLabel)
        ToolStpTxtBoxRefYear = CType(ToolStp.Items("tstbxRefYear"), ToolStripTextBox)
        ToolStpLblNumYear = CType(ToolStp.Items("tslblNumYear"), ToolStripLabel)
        ToolStpTxtBoxNumYear = CType(ToolStp.Items("tstbxNumYear"), ToolStripTextBox)

        ToolStp.Visible = False

        ToolStpBtnCal.Text = My.Resources.BTN_CALCULATE
        ToolStpBtnCal.Visible = True
        ToolStpBtnPlot.Text = My.Resources.BTN_PLOT
        ToolStpBtnPlot.Visible = True
        ToolStpSep.Visible = True

        If CatchHistoryType = My.Resources.TREE_NODE_CATCH_FORECAST Then
            ToolStpLblRefYear.Text = My.Resources.LBL_REF_YEAR
            ToolStpLblRefYear.Visible = True
            ToolStpTxtBoxRefYear.Text = CStr(EcotrophManager.InputData.ReferenceYear)
            ToolStpTxtBoxRefYear.Visible = True

            ToolStpLblNumYear.Text = My.Resources.LBL_NUM_FORECAST_YEAR
            ToolStpLblNumYear.Visible = True
            ToolStpTxtBoxNumYear.Text = CStr(EcotrophManager.InputData.NumForecastYear)
            ToolStpTxtBoxNumYear.Visible = True
        End If

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

    Private Shared Sub SetUpGridRowPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.RowCount = (EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1) + 2
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
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_TRP_LVL
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                For Idx As Integer = 1 To EcotrophManager.InputData.NumForecastYear
                    DataGrid.Columns(Idx + 1).HeaderText = CStr(EcotrophManager.InputData.ReferenceYear + Idx - 1)
                Next Idx
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                For Idx As Integer = 1 To EcotrophManager.InputData.NumPastAnalysisYear
                    DataGrid.Columns(Idx + 1).HeaderText = CStr(EcotrophManager.InputData.PastAnalysisYear(Idx))
                Next Idx
        End Select
    End Sub

    Private Shared Sub SetUpGridRow(ByVal EffortMultiplierType As String, ByVal PanelToolStrip As Panel, _
      ByVal TabPgText As String, ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal CatchHistoryType As String)
        Dim RowContent() As String
        Dim CellStyle As DataGridViewCellStyle
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False
        DataGrid.ReadOnly = False
        'Select Case EffortMultiplierType
        '    Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
        'Set up Catch Multiplier row
        RowContent(0) = ""
        RowContent(1) = My.Resources.CELL_CATCH_MTPLR
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                For Col As Integer = 1 To EcotrophManager.InputData.CatchMultiplier.GetUpperBound(0)
                    RowContent(Col + 1) = EcotrophManager.InputData.CatchMultiplier(Col).ToString("0.00")
                Next
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                For Col As Integer = 1 To EcotrophManager.DynamicsCatchMultiplier.GetUpperBound(0)
                    RowContent(Col + 1) = EcotrophManager.DynamicsCatchMultiplier(Col).ToString("0.00")
                Next
        End Select
        DataGrid.Rows(0).SetValues(RowContent)
        DataGrid.Rows(0).Height = cUtility.EFF_MTPLR_TRP_LVLOUT_ROW_HEIGHT
        DataGrid.Rows(0).Visible = True
        DataGrid.Rows(0).Frozen = True
        CellStyle = New DataGridViewCellStyle
        CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGrid.Item(1, 0).Style = CellStyle
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                CellStyle = New DataGridViewCellStyle
                CellStyle.BackColor = Drawing.Color.lightgreen
                For Col As Integer = 1 To EcotrophManager.InputData.CatchMultiplier.GetUpperBound(0)
                    DataGrid.Item(Col + 1, 0).Style = CellStyle
                Next
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                CellStyle = New DataGridViewCellStyle
                CellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
                For Col As Integer = 1 To EcotrophManager.DynamicsCatchMultiplier.GetUpperBound(0)
                    DataGrid.Item(Col + 1, 0).Style = CellStyle
                Next
        End Select
        DataGrid.Item(0, 0).ReadOnly = True
        DataGrid.Item(1, 0).ReadOnly = True
        If CatchHistoryType = My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS Then
            For Col As Integer = 3 To EcotrophManager.DynamicsCatchMultiplier.GetUpperBound(0) + 2
                DataGrid.Item(Col - 1, 0).ReadOnly = True
            Next
        End If

        'Set up Index PP row
        RowContent(0) = ""
        RowContent(1) = My.Resources.CELL_IDX_PP_TRP_LVL
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                For Col As Integer = 1 To EcotrophManager.InputData.IndexPPForecast.GetUpperBound(0)
                    RowContent(Col + 1) = EcotrophManager.InputData.IndexPPForecast(Col).ToString("0.00")
                Next
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                For Col As Integer = 1 To EcotrophManager.InputData.IndexPPPastAnalysis.GetUpperBound(0)
                    RowContent(Col + 1) = EcotrophManager.InputData.IndexPPPastAnalysis(Col).ToString("0.00")
                Next
        End Select
        DataGrid.Rows(1).SetValues(RowContent)
        DataGrid.Rows(1).Height = cUtility.EFF_MTPLR_TRP_LVLOUT_ROW_HEIGHT
        DataGrid.Rows(1).Visible = True
        DataGrid.Rows(1).Frozen = True
        CellStyle = New DataGridViewCellStyle
        CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGrid.Item(1, 1).Style = CellStyle
        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.LightGreen
        Select Case CatchHistoryType
            Case My.Resources.TREE_NODE_CATCH_FORECAST
                For Col As Integer = 1 To EcotrophManager.InputData.IndexPPForecast.GetUpperBound(0)
                    DataGrid.Item(Col + 1, 1).Style = CellStyle
                Next
            Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                For Col As Integer = 1 To EcotrophManager.InputData.IndexPPPastAnalysis.GetUpperBound(0)
                    DataGrid.Item(Col + 1, 1).Style = CellStyle
                Next
        End Select

        DataGrid.Item(0, 1).ReadOnly = True
        DataGrid.Item(1, 1).ReadOnly = True

        For Row As Integer = 3 To DataGrid.RowCount
            DataGrid.Rows(Row - 1).ReadOnly = True
        Next
        SetUpAryRow(TabPgText, DataGrid, EcotrophManager, CatchHistoryType)
        '    Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
        '        'Set up Effort Multiplier row
        '        RowContent(0) = ""
        '        RowContent(1) = My.Resources.CELL_EFF_MTPLR_TRP_LVLOUT
        '        For Col As Integer = 1 To EcotrophManager.InputData.EffortMultiplier.GetUpperBound(0)
        '            RowContent(Col + 1) = EcotrophManager.InputData.EffortMultiplier(Col).ToString("#0.#")
        '        Next
        '        DataGrid.Rows(0).SetValues(RowContent)
        '        DataGrid.Rows(0).Height = cUtility.EFF_MTPLR_TRP_LVLOUT_ROW_HEIGHT
        '        DataGrid.Rows(0).Visible = True
        '        DataGrid.Rows(0).Frozen = True
        '        CellStyle = New DataGridViewCellStyle
        '        CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        '        DataGrid.Item(1, 0).Style = CellStyle
        '        CellStyle = New DataGridViewCellStyle
        '        CellStyle.BackColor = Drawing.Color.lightgreen
        '        For Col As Integer = 1 To EcotrophManager.InputData.EffortMultiplier.GetUpperBound(0)
        '            DataGrid.Item(Col + 1, 0).Style = CellStyle
        '        Next
        '        DataGrid.ReadOnly = False
        '        DataGrid.Item(0, 0).ReadOnly = True
        '        DataGrid.Item(1, 0).ReadOnly = True
        '        For Row As Integer = 2 To DataGrid.RowCount
        '            DataGrid.Rows(Row - 1).ReadOnly = True
        '        Next

        '        SetUpAryRow(TabPgText, DataGrid, EcotrophManager)
        'End Select
        DataGrid.ClearSelection()
    End Sub

    Private Shared Sub SetUpAryRow(ByVal TabPgText As String, ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
      ByVal CatchHistoryType As String)
        Dim RowContent() As String
        ReDim RowContent(DataGrid.Columns.Count)

        For Row As Integer = 1 To EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1
            RowContent(0) = CStr(Row)
            If Single.IsNaN(EcotrophManager.DynamicsIntrpTL(Row)) Then
                RowContent(1) = ""
            Else
                RowContent(1) = EcotrophManager.DynamicsIntrpTL(Row).ToString("F2")
            End If
            Select Case TabPgText
                Case My.Resources.TAB_PROD
                    For Col As Integer = 1 To EcotrophManager.DynamicsFlow.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsFlow(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsFlow(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_BIOMASS
                    For Col As Integer = 1 To EcotrophManager.DynamicsBiomass.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsBiomass(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsBiomass(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_KINETIC
                    For Col As Integer = 1 To EcotrophManager.DynamicsKinetic.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsKinetic(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsKinetic(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_ACCESS_PROD
                    For Col As Integer = 1 To EcotrophManager.DynamicsAccessFlow.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsAccessFlow(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsAccessFlow(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_FISH_LOSS_RATE
                    For Col As Integer = 1 To EcotrophManager.DynamicsFishLossRate.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsFishLossRate(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsFishLossRate(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_CATCHES
                    Select Case CatchHistoryType
                        Case My.Resources.TREE_NODE_CATCH_FORECAST
                            For Col As Integer = 1 To EcotrophManager.DynamicsCatches.GetUpperBound(1)
                                If Single.IsNaN(EcotrophManager.DynamicsCatches(Row, Col)) Then
                                    RowContent(Col + 1) = ""
                                Else
                                    RowContent(Col + 1) = (EcotrophManager.DynamicsCatches(Row, Col)).ToString("F4")
                                End If
                            Next
                        Case (My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS)
                            For Col As Integer = 1 To EcotrophManager.InputData.CatchPastAnalysis.GetUpperBound(1)
                                If Single.IsNaN(EcotrophManager.InputData.CatchPastAnalysis(Row, Col)) Then
                                    RowContent(Col + 1) = ""
                                Else
                                    RowContent(Col + 1) = (EcotrophManager.InputData.CatchPastAnalysis(Row, Col)).ToString("F4")
                                End If
                            Next
                    End Select
                Case My.Resources.TAB_FISH_MORTALITY
                    For Col As Integer = 1 To EcotrophManager.DynamicsFishMortality.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsFishMortality(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsFishMortality(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_ACCESS_BIOMASS
                    For Col As Integer = 1 To EcotrophManager.DynamicsAccessBiomass.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsAccessBiomass(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsAccessBiomass(Row, Col)).ToString("F4")
                        End If
                    Next
                Case My.Resources.TAB_ACCESS_FISH_LOSS_RATE
                    For Col As Integer = 1 To EcotrophManager.DynamicsAccessFishLossRate.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsAccessFishLossRate(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsAccessFishLossRate(Row, Col)).ToString("F4")
                        End If
                    Next
                Case "Kinetic_Recal"
                    For Col As Integer = 1 To EcotrophManager.DynamicsKineticRecal.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsKineticRecal(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsKineticRecal(Row, Col)).ToString("F4")
                        End If
                    Next
                Case "Bpred"
                    For Col As Integer = 1 To EcotrophManager.DynamicsBiomassPred.GetUpperBound(1)
                        If Single.IsNaN(EcotrophManager.DynamicsBiomassPred(Row, Col)) Then
                            RowContent(Col + 1) = ""
                        Else
                            RowContent(Col + 1) = (EcotrophManager.DynamicsBiomassPred(Row, Col)).ToString("F4")
                        End If
                    Next
            End Select

                    DataGrid.Rows(Row - 1 + 2).SetValues(RowContent)
                    DataGrid.Rows(Row - 1 + 2).Visible = True
        Next
    End Sub
#End Region 'Helper methods

End Class
