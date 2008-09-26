'==============================================================================
'
' $Log: cCTSABwdCalParam.vb,v $
' Revision 1.1  2008/09/26 07:30:38  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cCTSABwdCalParam

#Region "Private field"
    Private Const NUM_COL_EXCLUDE_ROW_HEADER As Integer = 7
#End Region 'Private field

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip, ByVal CTSA As UserInterface.cCTSA)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip, CTSA.m_EcotrophManager)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal DataGrid As DataGridView, ByVal CTSA As UserInterface.cCTSA)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, CTSA.m_EcotrophManager)
        SetUpGridRowPropertyDefault(DataGrid, CTSA.m_EcotrophManager)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid)
        SetUpGridRow(DataGrid, CTSA.m_EcotrophManager)
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

    Private Shared Sub SetUpToolStrip(ByVal PanelToolStrip As Panel, ByVal EcotrophManager As cEcotrophManager)
        Dim ToolStp As ToolStrip
        Dim ToolStpBtnCal As ToolStripButton
        Dim ToolStpBtnPlot As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator
        Dim ToolStpLblTTL As ToolStripLabel
        Dim ToolStpCbxTTL As ToolStripComboBox
        'Dim ToolStpLblSlopeSelectTTL As ToolStripLabel
        'Dim ToolStpTbxSlopeSelectTTL As ToolStripTextBox
        Dim ToolStpLblInit As ToolStripLabel
        Dim ToolStpCbxInit As ToolStripComboBox

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpBtnCal = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)
        ToolStpBtnPlot = CType(ToolStp.Items("tsbtnPlot"), ToolStripButton)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)
        ToolStpLblTTL = CType(ToolStp.Items("tslblTerminalTL"), ToolStripLabel)
        ToolStpCbxTTL = CType(ToolStp.Items("tscbxTerminalTL"), ToolStripComboBox)
        'ToolStpLblSlopeSelectTTL = CType(ToolStp.Items("tslblSlopeSelectivityTTL"), ToolStripLabel)
        'ToolStpTbxSlopeSelectTTL = CType(ToolStp.Items("tstbxSlopeSelectivityTTL"), ToolStripTextBox)
        ToolStpLblInit = CType(ToolStp.Items("tslblInitializationBwdCal"), ToolStripLabel)
        ToolStpCbxInit = CType(ToolStp.Items("tscbxInitializationBwdCal"), ToolStripComboBox)

        ToolStp.Visible = False

        ToolStpBtnCal.Text = My.Resources.BTN_CALCULATE
        ToolStpBtnCal.Visible = True
        ToolStpBtnPlot.Text = My.Resources.BTN_PLOT
        ToolStpBtnPlot.Visible = True
        ToolStpSep.Visible = True
        ToolStpLblTTL.Text = My.Resources.LBL_TTL
        ToolStpLblTTL.Visible = True
        ToolStpCbxTTL.Text = CStr(EcotrophManager.InputData.TTL)
        ToolStpCbxTTL.Visible = True
        'ToolStpLblSlopeSelectTTL.Text = My.Resources.LBL_SLOPE_SELECTIVITY_TTL
        'ToolStpLblSlopeSelectTTL.Visible = True
        'ToolStpTbxSlopeSelectTTL.Text = CStr(EcotrophManager.InputData.SlopeSelectivityTTL)
        'ToolStpTbxSlopeSelectTTL.Visible = True
        ToolStpLblInit.Text = My.Resources.LBL_INIT
        ToolStpLblInit.Visible = True
        ToolStpCbxInit.Text = EcotrophManager.InputData.SeedNameBwdCal
        ToolStpCbxInit.Visible = True

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
        DataGrid.RowCount = EcotrophManager.CTSAKinetic.GetUpperBound(0)
        cUtility.SetGridRowPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridCellPropertyDefault(ByVal DataGrid As DataGridView) ', ByVal EcotrophManager As cEcotrophManager)
        cUtility.SetGridCellPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridColumn(ByVal DataGrid As DataGridView)
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_TRP_LVL
        DataGrid.Columns(2).HeaderText = My.Resources.COL_HDR_PROD
        DataGrid.Columns(3).HeaderText = My.Resources.COL_HDR_BIOMASS
        DataGrid.Columns(4).HeaderText = My.Resources.COL_HDR_FISH_LOSS_RATE
        DataGrid.Columns(5).HeaderText = My.Resources.COL_HDR_ACCESS_FISH_MORTALITY
        DataGrid.Columns(6).HeaderText = My.Resources.COL_HDR_VIRGIN_PROD
        DataGrid.Columns(7).HeaderText = My.Resources.COL_HDR_VIRGIN_BIOMASS
        DataGrid.Columns(8).HeaderText = My.Resources.COL_HDR_KINETIC_RECAL
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim RowContent() As String
        Dim RowTTL As Integer
        Dim TLOut As Single
        Dim CellStyle As DataGridViewCellStyle
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False
        DataGrid.ReadOnly = False
        For Col As Integer = 0 To 3
            For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next
        RowTTL = CInt((Int(EcotrophManager.InputData.TTL) - 2) * 10 + CInt((EcotrophManager.InputData.TTL - Int(EcotrophManager.InputData.TTL)) * 10) + 2) _
          - 1
        For col As Integer = 4 To 5
            For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                If Row <> RowTTL Then DataGrid.Item(col, Row).ReadOnly = True
            Next
        Next
        Select Case EcotrophManager.InputData.SeedNameBwdCal
            Case My.Resources.DROP_DWN_LST_ITM_FISH_LOSS_RATE_TLL
                DataGrid.Item(5, RowTTL).ReadOnly = True
            Case My.Resources.DROP_DWN_LST_ITM_ACCESS_FISH_MORTALITY_TTL
                DataGrid.Item(4, RowTTL).ReadOnly = True
        End Select
        For Col As Integer = 6 To DataGrid.ColumnCount - 1
            For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next

        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.LightGreen
        Select Case EcotrophManager.InputData.SeedNameBwdCal
            Case My.Resources.DROP_DWN_LST_ITM_FISH_LOSS_RATE_TLL
                DataGrid.Item(4, RowTTL).Style = CellStyle
            Case My.Resources.DROP_DWN_LST_ITM_ACCESS_FISH_MORTALITY_TTL
                DataGrid.Item(5, RowTTL).Style = CellStyle
        End Select

        TLOut = 2
        For Row As Integer = 1 To EcotrophManager.BwdCalFlow.GetUpperBound(0)
            RowContent(0) = CStr(Row)
            If Row = 1 Then
                RowContent(1) = "1"
            Else
                RowContent(1) = TLOut.ToString("0.#")
            End If
            If Single.IsNaN(EcotrophManager.BwdCalFlow(Row)) Then
                RowContent(2) = ""
            Else
                RowContent(2) = EcotrophManager.BwdCalFlow(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.BwdCalBiomass(Row)) Then
                RowContent(3) = ""
            Else
                RowContent(3) = EcotrophManager.BwdCalBiomass(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.BwdCalFishLossRate(Row)) Then
                RowContent(4) = ""
            Else
                RowContent(4) = EcotrophManager.BwdCalFishLossRate(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.BwdCalAccessFishMortality(Row)) Then
                RowContent(5) = ""
            Else
                RowContent(5) = EcotrophManager.BwdCalAccessFishMortality(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.BwdCalVirginFlow(Row)) Then
                RowContent(6) = ""
            Else
                RowContent(6) = EcotrophManager.BwdCalVirginFlow(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.BwdCalVirginBiomass(Row)) Then
                RowContent(7) = ""
            Else
                RowContent(7) = EcotrophManager.BwdCalVirginBiomass(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.BwdCalKineticRecal(Row)) Then
                RowContent(8) = ""
            Else
                RowContent(8) = EcotrophManager.BwdCalKineticRecal(Row).ToString("F4")
            End If
            DataGrid.Rows(Row - 1).SetValues(RowContent)
            DataGrid.Rows(Row - 1).Visible = True
            If Row > 1 Then TLOut = CSng(TLOut + 0.1)
        Next

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
