'==============================================================================
'
' $Log: cDynamicsIntrpParam.vb,v $
' Revision 1.1  2008/09/26 07:30:40  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cDynamicsIntrpParam

#Region "Private field"
    Private Const NUM_COL_EXCLUDE_ROW_HEADER As Integer = 14
#End Region 'Private field

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip, ByVal Dynamics As UserInterface.cDynamics)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip, Dynamics.m_EcotrophManager)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal DataGrid As DataGridView, ByVal Dynamics As UserInterface.cDynamics)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Dynamics.m_EcotrophManager)
        SetUpGridRowPropertyDefault(DataGrid, Dynamics.m_EcotrophManager)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid)
        SetUpGridRow(Dynamics.m_PanelToolStrip, DataGrid, Dynamics.m_EcotrophManager)
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
        Dim ToolStpBtnSetDefault As ToolStripButton
        Dim ToolStpSep As ToolStripSeparator
        Dim ToolStpLblMain As ToolStripLabel
        Dim ToolStpCbx As ToolStripComboBox
        Dim ToolStpLblBeta As ToolStripLabel
        Dim ToolStpTxtBoxBeta As ToolStripTextBox

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStpBtnCal = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)
        ToolStpBtnSetDefault = CType(ToolStp.Items("tsbtnSetDefault"), ToolStripButton)
        ToolStpSep = CType(ToolStp.Items("tssepSeparator"), ToolStripSeparator)
        ToolStpLblMain = CType(ToolStp.Items("tslblMain"), ToolStripLabel)
        ToolStpCbx = CType(ToolStp.Items("tscbxMainDynamics"), ToolStripComboBox)
        ToolStpLblBeta = CType(ToolStp.Items("tslblBeta"), ToolStripLabel)
        ToolStpTxtBoxBeta = CType(ToolStp.Items("tstbxBeta"), ToolStripTextBox)

        ToolStp.Visible = False

        ToolStpBtnCal.Text = My.Resources.BTN_CALCULATE
        ToolStpBtnCal.Visible = True

        ToolStpBtnSetDefault.Text = My.Resources.BTN_SET_DEFAULT
        ToolStpBtnSetDefault.Visible = True

        ToolStpSep.Visible = True

        ToolStpLblMain.Text = My.Resources.LBL_MAIN_FROM
        ToolStpLblMain.Visible = True
        ToolStpCbx.AutoToolTip = True
        ToolStpCbx.Visible = True

        ToolStpLblBeta.Text = My.Resources.LBL_BETA
        ToolStpLblBeta.Visible = True
        ToolStpTxtBoxBeta.Text = CStr(EcotrophManager.InputData.DynamicsBeta)
        ToolStpTxtBoxBeta.Visible = True

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
        DataGrid.RowCount = EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1
        cUtility.SetGridRowPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridCellPropertyDefault(ByVal DataGrid As DataGridView)
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
        DataGrid.Columns(2).HeaderText = My.Resources.COL_HDR_BIOMASS
        DataGrid.Columns(3).HeaderText = My.Resources.COL_HDR_ACCESS_BIOMASS
        DataGrid.Columns(4).HeaderText = My.Resources.COL_HDR_PROD
        DataGrid.Columns(5).HeaderText = My.Resources.COL_HDR_KINETIC
        DataGrid.Columns(6).HeaderText = My.Resources.COL_HDR_CATCHES
        DataGrid.Columns(7).HeaderText = My.Resources.COL_HDR_FISH_LOSS_RATE
        DataGrid.Columns(8).HeaderText = My.Resources.COL_HDR_ACCESS_FISH_LOSS_RATE
        DataGrid.Columns(9).HeaderText = My.Resources.COL_HDR_NATURAL_LOSS_RATE
        DataGrid.Columns(10).HeaderText = My.Resources.COL_HDR_FISH_MORTALITY
        DataGrid.Columns(11).HeaderText = My.Resources.COL_HDR_ACCESS_FISH_MORTALITY
        DataGrid.Columns(12).HeaderText = My.Resources.COL_HDR_SELECTIVITY
        DataGrid.Columns(13).HeaderText = My.Resources.COL_HDR_TIME
        DataGrid.Columns(14).HeaderText = My.Resources.COL_HDR_TOP_D
        DataGrid.Columns(15).HeaderText = My.Resources.COL_HDR_FORM_D
    End Sub

    Private Shared Sub SetUpGridRow(ByVal PanelToolStrip As Panel, ByVal DataGrid As DataGridView, _
      ByVal EcotrophManager As cEcotrophManager)
        Dim RowContent() As String
        Dim CellStyle As DataGridViewCellStyle
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False
        DataGrid.ReadOnly = False
        For Col As Integer = 0 To DataGrid.ColumnCount - 2 - 1
            For Row As Integer = 0 To EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1 - 1
                DataGrid.Item(Col, Row).ReadOnly = True
            Next
        Next

        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.LightGreen
        For Col As Integer = DataGrid.ColumnCount - 1 - 1 To DataGrid.ColumnCount - 1
            For Row As Integer = 0 To EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1 - 1
                DataGrid.Item(Col, Row).Style = CellStyle
            Next
        Next

        For Row As Integer = 1 To EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1
            RowContent(0) = CStr(Row)
            If Single.IsNaN(EcotrophManager.DynamicsIntrpTL(Row)) Then
                RowContent(1) = ""
            Else
                RowContent(1) = EcotrophManager.DynamicsIntrpTL(Row).ToString("F2")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpBiomass(Row)) Then
                RowContent(2) = ""
            Else
                RowContent(2) = EcotrophManager.DynamicsIntrpBiomass(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpAccessBiomass(Row)) Then
                RowContent(3) = ""
            Else
                RowContent(3) = EcotrophManager.DynamicsIntrpAccessBiomass(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpFlow(Row)) Then
                RowContent(4) = ""
            Else
                RowContent(4) = EcotrophManager.DynamicsIntrpFlow(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpKinetic(Row)) Then
                RowContent(5) = ""
            Else
                RowContent(5) = EcotrophManager.DynamicsIntrpKinetic(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpCatches(Row)) Then
                RowContent(6) = ""
            Else
                RowContent(6) = EcotrophManager.DynamicsIntrpCatches(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpFishLossRate(Row)) Then
                RowContent(7) = ""
            Else
                RowContent(7) = EcotrophManager.DynamicsIntrpFishLossRate(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpAccessFishLossRate(Row)) Then
                RowContent(8) = ""
            Else
                RowContent(8) = EcotrophManager.DynamicsIntrpAccessFishLossRate(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpFishMortality(Row)) Then
                RowContent(10) = ""
            Else
                RowContent(10) = EcotrophManager.DynamicsIntrpFishMortality(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpAccessFishMortality(Row)) Then
                RowContent(11) = ""
            Else
                RowContent(11) = EcotrophManager.DynamicsIntrpAccessFishMortality(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpSelectivity(Row)) Then
                RowContent(12) = ""
            Else
                RowContent(12) = EcotrophManager.DynamicsIntrpSelectivity(Row).ToString("F4")
            End If
            If Row < EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1 Then
                If Single.IsNaN(EcotrophManager.DynamicsIntrpNaturalLossRate(Row)) Then
                    RowContent(9) = ""
                Else
                    RowContent(9) = EcotrophManager.DynamicsIntrpNaturalLossRate(Row).ToString("F4")
                End If
                If Single.IsNaN(EcotrophManager.DynamicsIntrpITime2(Row)) Then
                    RowContent(13) = ""
                Else
                    RowContent(13) = EcotrophManager.DynamicsIntrpITime2(Row).ToString("F4")
                End If
            Else
                RowContent(9) = ""
                RowContent(13) = ""
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpTopD(Row)) Then
                RowContent(14) = ""
            Else
                RowContent(14) = EcotrophManager.DynamicsIntrpTopD(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.DynamicsIntrpFormD(Row)) Then
                RowContent(15) = ""
            Else
                RowContent(15) = EcotrophManager.DynamicsIntrpFormD(Row).ToString("F4")
            End If
            DataGrid.Rows(Row - 1).SetValues(RowContent)
            DataGrid.Rows(Row - 1).Visible = True
        Next

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
