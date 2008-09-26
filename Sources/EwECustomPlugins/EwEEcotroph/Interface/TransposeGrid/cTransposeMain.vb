'==============================================================================
'
' $Log: cTransposeMain.vb,v $
' Revision 1.1  2008/09/26 07:30:41  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.18  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cTransposeMain

#Region "Private field"
    Private Const NUM_COL_EXCLUDE_ROW_HEADER As Integer = 12
#End Region 'Private field

#Region "Public methods"
    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, _
      ByVal ToolStp As ToolStrip, ByVal Transpose As UserInterface.cTranspose)
        Cursor.Current = Cursors.WaitCursor
        SetUpToolStripPropertyDefault(PanelToolStrip, PanelTabCntl, ToolStp)
        SetUpToolStrip(PanelToolStrip, Transpose.m_EcotrophManager)
        Cursor.Current = Cursors.Default
    End Sub

    Public Shared Sub DisplayGridData(ByVal DataGrid As DataGridView, ByVal Transpose As UserInterface.cTranspose)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumnPropertyDefault(DataGrid, Transpose.m_EcotrophManager)
        SetUpGridRowPropertyDefault(DataGrid, Transpose.m_EcotrophManager)
        SetUpGridCellPropertyDefault(DataGrid)

        SetUpGridColumn(DataGrid)
        SetUpGridRow(DataGrid, Transpose.m_EcotrophManager)
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
        DataGrid.ColumnCount = NUM_COL_EXCLUDE_ROW_HEADER + 2
        cUtility.SetGridColumnPropertyDefault(DataGrid)
    End Sub

    Private Shared Sub SetUpGridRowPropertyDefault(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        DataGrid.RowCount = EcotrophManager.TransposeBiomassSum.GetUpperBound(0)
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
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
        Dim RowContent() As String
        Dim TLOut As Single
        ReDim RowContent(DataGrid.Columns.Count)

        DataGrid.RowHeadersVisible = False

        TLOut = 2
        For Row As Integer = 1 To EcotrophManager.TransposeBiomassSum.GetUpperBound(0)
            RowContent(0) = CStr(Row)
            If Row = 1 Then
                RowContent(1) = "1"
            Else
                RowContent(1) = TLOut.ToString("0.#")
            End If
            If Single.IsNaN(EcotrophManager.TransposeBiomassSum(Row)) Then
                RowContent(2) = ""
            Else
                RowContent(2) = EcotrophManager.TransposeBiomassSum(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.AccessBiomassSum(Row)) Then
                RowContent(3) = ""
            Else
                RowContent(3) = EcotrophManager.AccessBiomassSum(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.TransposeFlowSum(Row)) Then
                RowContent(4) = ""
            Else
                RowContent(4) = EcotrophManager.TransposeFlowSum(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.Kinetic(Row)) Then
                RowContent(5) = ""
            Else
                RowContent(5) = EcotrophManager.Kinetic(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.TransposeCatchSumGpFlt(Row)) Then
                RowContent(6) = ""
            Else
                RowContent(6) = EcotrophManager.TransposeCatchSumGpFlt(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.FishLossRate(Row)) Then
                RowContent(7) = ""
            Else
                RowContent(7) = EcotrophManager.FishLossRate(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.AccessFishLossRate(Row)) Then
                RowContent(8) = ""
            Else
                RowContent(8) = EcotrophManager.AccessFishLossRate(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.FishMortality(Row)) Then
                RowContent(10) = ""
            Else
                RowContent(10) = EcotrophManager.FishMortality(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.AccessFishMortality(Row)) Then
                RowContent(11) = ""
            Else
                RowContent(11) = EcotrophManager.AccessFishMortality(Row).ToString("F4")
            End If
            If Single.IsNaN(EcotrophManager.Selectivity(Row)) Then
                RowContent(12) = ""
            Else
                RowContent(12) = EcotrophManager.Selectivity(Row).ToString("F4")
            End If
            If Row < EcotrophManager.TransposeBiomassSum.GetUpperBound(0) Then
                If Single.IsNaN(EcotrophManager.NaturalLossRate(Row)) Then
                    RowContent(9) = ""
                Else
                    RowContent(9) = EcotrophManager.NaturalLossRate(Row).ToString("F4")
                End If
                If Single.IsNaN(EcotrophManager.Time(Row)) Then
                    RowContent(13) = ""
                Else
                    RowContent(13) = EcotrophManager.Time(Row).ToString("F4")
                End If
            Else
                RowContent(9) = ""
                RowContent(13) = ""
            End If
            DataGrid.Rows(Row - 1).SetValues(RowContent)
            DataGrid.Rows(Row - 1).Visible = True
            If Row > 1 Then TLOut = CSng(TLOut + 0.1)
        Next

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
