'==============================================================================
'
' $Log: cCTSABasicParam.vb,v $
' Revision 1.1  2008/09/26 07:30:38  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cCTSABasicParam
#Region "Private field"
    Private Const NUM_COL_EXCLUDE_ROW_HEADER As Integer = 6
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
        Dim ToolStpBtnImportCatches As ToolStripButton
        Dim ToolStpBtnSetDefault As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator
        Dim ToolStpLblWaterTemp As ToolStripLabel
        Dim ToolStpTxtBoxWaterTemp As ToolStripTextBox
        Dim ToolStpLblTETL12 As ToolStripLabel
        Dim ToolStpTxtBoxTETL12 As ToolStripTextBox
        Dim ToolStpLblTETL2 As ToolStripLabel
        Dim ToolStpTxtBoxTETL2 As ToolStripTextBox
        Dim ToolStpLblAsymptote As ToolStripLabel
        Dim ToolStpTxtBoxAsymptote As ToolStripTextBox
        Dim ToolStpLblTL50 As ToolStripLabel
        Dim ToolStpTxtBoxTL50 As ToolStripTextBox
        Dim ToolStpLblSlope As ToolStripLabel
        Dim ToolStpTxtBoxSlope As ToolStripTextBox

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpBtnCal = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)
        ToolStpBtnImportCatches = CType(ToolStp.Items("tsbtnImportCatches"), ToolStripButton)
        ToolStpBtnSetDefault = CType(ToolStp.Items("tsbtnSetDefault"), ToolStripButton)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)
        ToolStpLblWaterTemp = CType(ToolStp.Items("tslblWaterTemp"), ToolStripLabel)
        ToolStpTxtBoxWaterTemp = CType(ToolStp.Items("tstbxWaterTemp"), ToolStripTextBox)
        ToolStpLblTETL12 = CType(ToolStp.Items("tslblTETL12"), ToolStripLabel)
        ToolStpTxtBoxTETL12 = CType(ToolStp.Items("tstbxTETL12"), ToolStripTextBox)
        ToolStpLblTETL2 = CType(ToolStp.Items("tslblTETL2"), ToolStripLabel)
        ToolStpTxtBoxTETL2 = CType(ToolStp.Items("tstbxTETL2"), ToolStripTextBox)
        ToolStpLblAsymptote = CType(ToolStp.Items("tslblAsymptote"), ToolStripLabel)
        ToolStpTxtBoxAsymptote = CType(ToolStp.Items("tstbxAsymptote"), ToolStripTextBox)
        ToolStpLblTL50 = CType(ToolStp.Items("tslblTL50"), ToolStripLabel)
        ToolStpTxtBoxTL50 = CType(ToolStp.Items("tstbxTL50"), ToolStripTextBox)
        ToolStpLblSlope = CType(ToolStp.Items("tslblSlope"), ToolStripLabel)
        ToolStpTxtBoxSlope = CType(ToolStp.Items("tstbxSlope"), ToolStripTextBox)

        ToolStp.Visible = False

        ToolStpBtnCal.Text = My.Resources.BTN_CALCULATE
        ToolStpBtnCal.Visible = True
        ToolStpBtnImportCatches.Text = My.Resources.BTN_IMPORT
        ToolStpBtnImportCatches.Visible = True
        ToolStpBtnSetDefault.Text = My.Resources.BTN_SET_DEFAULT
        ToolStpBtnSetDefault.Visible = True
        ToolStpSep.Visible = True

        ToolStpLblWaterTemp.Text = My.Resources.LBL_WATER_TEMP
        ToolStpLblWaterTemp.AutoToolTip = True
        ToolStpLblWaterTemp.ToolTipText = My.Resources.LBL_WATER_TEMP
        ToolStpLblWaterTemp.Visible = True
        ToolStpTxtBoxWaterTemp.Text = CStr(EcotrophManager.InputData.WaterTemp)
        ToolStpTxtBoxWaterTemp.Visible = True


        ToolStpLblTETL12.Text = My.Resources.LBL_TE_TL12
        ToolStpLblTETL12.AutoToolTip = True
        ToolStpLblTETL12.ToolTipText = My.Resources.LBL_TE_TL12
        ToolStpLblTETL12.AutoSize = False
        ToolStpLblTETL12.Visible = True
        ToolStpTxtBoxTETL12.Text = CStr(EcotrophManager.InputData.TETL12)
        ToolStpTxtBoxTETL12.Visible = True

        ToolStpLblTETL2.Text = My.Resources.LBL_TE_TL2
        ToolStpLblTETL2.AutoToolTip = True
        ToolStpLblTETL2.ToolTipText = My.Resources.LBL_TE_TL2
        ToolStpLblTETL2.Visible = True
        ToolStpTxtBoxTETL2.Text = CStr(EcotrophManager.InputData.TETL2)
        ToolStpTxtBoxTETL2.Visible = True

        ToolStpLblAsymptote.Text = My.Resources.LBL_ASYMPTOTE
        ToolStpLblAsymptote.AutoToolTip = True
        ToolStpLblAsymptote.ToolTipText = My.Resources.LBL_ASYMPTOTE
        ToolStpLblAsymptote.Visible = True
        ToolStpTxtBoxAsymptote.Text = CStr(EcotrophManager.InputData.Asymptote)
        ToolStpTxtBoxAsymptote.Visible = True

        ToolStpLblTL50.Text = My.Resources.LBL_TL50
        ToolStpLblTL50.Visible = True
        ToolStpTxtBoxTL50.Text = CStr(EcotrophManager.InputData.TL50)
        ToolStpTxtBoxTL50.Visible = True

        ToolStpLblSlope.Text = My.Resources.LBL_SLOPE
        ToolStpLblSlope.Visible = True
        ToolStpTxtBoxSlope.Text = CStr(EcotrophManager.InputData.Slope)
        ToolStpTxtBoxSlope.Visible = True

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
        DataGrid.Columns(2).HeaderText = My.Resources.COL_HDR_CATCHES
        DataGrid.Columns(3).HeaderText = My.Resources.COL_HDR_VIRGIN_KINETIC
        DataGrid.Columns(4).HeaderText = My.Resources.COL_HDR_NATURAL_LOSS_RATE
        DataGrid.Columns(5).HeaderText = My.Resources.COL_HDR_SELECTIVITY
        DataGrid.Columns(6).HeaderText = My.Resources.COL_HDR_TOP_D
        DataGrid.Columns(7).HeaderText = My.Resources.COL_HDR_FORM_D
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim RowContent() As String
        Dim TLOut As Single
        Dim CellStyle As DataGridViewCellStyle
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False
        DataGrid.ReadOnly = False
        For Col As Integer = 0 To 1
            For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next
        For Col As Integer = 3 To 5
            For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next
        'For Col As Integer = DataGrid.ColumnCount - 1 To DataGrid.ColumnCount - 1
        '    For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
        '        DataGrid.Item(Col, Row).ReadOnly = True
        '    Next
        'Next

        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.LightGreen
        For Col As Integer = 2 To 2
            For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).Style = CellStyle
            Next
        Next
        For Col As Integer = 6 To 7
            For Row As Integer = 0 To EcotrophManager.CTSAKinetic.GetUpperBound(0) - 1
                DataGrid.Item(Col, Row).Style = CellStyle
            Next
        Next

        TLOut = 2
        For Row As Integer = 1 To EcotrophManager.CTSAKinetic.GetUpperBound(0)
            RowContent(0) = CStr(Row)
            If Row = 1 Then
                RowContent(1) = "1"
            Else
                RowContent(1) = TLOut.ToString("0.#")
            End If
            If Single.IsNaN(EcotrophManager.InputData.Catches(Row)) Then
                RowContent(2) = ""
            Else
                RowContent(2) = EcotrophManager.InputData.Catches(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.CTSAKinetic(Row)) Then
                RowContent(3) = ""
            Else
                RowContent(3) = EcotrophManager.CTSAKinetic(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.CTSANaturalLossRate(Row)) Then
                RowContent(4) = ""
            Else
                RowContent(4) = EcotrophManager.CTSANaturalLossRate(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.CTSASelectivity(Row)) Then
                RowContent(5) = ""
            Else
                RowContent(5) = EcotrophManager.CTSASelectivity(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.TopD(Row)) Then
                RowContent(6) = ""
            Else
                RowContent(6) = EcotrophManager.TopD(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.FormD(Row)) Then
                RowContent(7) = ""
            Else
                RowContent(7) = EcotrophManager.FormD(Row).ToString("F4")
            End If
 
            DataGrid.Rows(Row - 1).SetValues(RowContent)
            DataGrid.Rows(Row - 1).Visible = True
            If Row > 1 Then TLOut = CSng(TLOut + 0.1)
        Next

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
