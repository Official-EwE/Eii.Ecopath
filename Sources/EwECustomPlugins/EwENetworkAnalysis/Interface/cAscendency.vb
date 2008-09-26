Public Class cAscendency
    Private m_NetworkManager As cNetworkManager
    Private m_Panel As Windows.Forms.Panel

    Public Sub New()
        '
    End Sub

    Public Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_Panel = Panel
    End Sub

    Public Sub SetUpByGroupPanel()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpByGroupGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nGroups

        For i As Integer = 1 To m_NetworkManager.nGroups
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.GroupName(i)
            strary(2) = m_NetworkManager.AscendancyByGroup(i).ToString("F4")
            strary(3) = m_NetworkManager.OverheadByGroup(i).ToString("F4")
            strary(4) = m_NetworkManager.CapacityByGroup(i).ToString("F4")
            strary(5) = m_NetworkManager.InformationByGroup(i).ToString("F4")
            strary(6) = m_NetworkManager.ThroughputByGroup(i).ToString("F4")
            DataGrid.Rows(i - 1).SetValues(strary)
        Next
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpTotalPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpTotalGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = 4

        strary(0) = "Import"
        strary(1) = m_NetworkManager.AscendancyImportTotal.ToString("F1")
        strary(2) = m_NetworkManager.AscendancyImportPer.ToString("F1")
        strary(3) = m_NetworkManager.OverheadImportTotal.ToString("F1")
        strary(4) = m_NetworkManager.OverheadImportPer.ToString("F1")
        strary(5) = m_NetworkManager.CapacityImportTotal.ToString("F1")
        strary(6) = m_NetworkManager.CapacityImportPer.ToString("F1")
        DataGrid.Rows(0).SetValues(strary)

        strary(0) = "Internal flow"
        strary(1) = m_NetworkManager.AscendancyInternalFlowTotal.ToString("F1")
        strary(2) = m_NetworkManager.AscendancyInternalFlowPer.ToString("F1")
        strary(3) = m_NetworkManager.OverheadFlowTotal.ToString("F1")
        strary(4) = m_NetworkManager.OverheadFlowPer.ToString("F1")
        strary(5) = m_NetworkManager.CapacityFlowTotal.ToString("F1")
        strary(6) = m_NetworkManager.CapacityFlowPer.ToString("F1")
        DataGrid.Rows(1).SetValues(strary)

        strary(0) = "Export"
        strary(1) = m_NetworkManager.AscendancyExportTotal.ToString("F1")
        strary(2) = m_NetworkManager.AscendancyExportPer.ToString("F1")
        strary(3) = m_NetworkManager.OverheadExportTotal.ToString("F1")
        strary(4) = m_NetworkManager.OverheadExportPer.ToString("F1")
        strary(5) = m_NetworkManager.CapacityExportTotal.ToString("F1")
        strary(6) = m_NetworkManager.CapacityExportPer.ToString("F1")
        DataGrid.Rows(2).SetValues(strary)

        strary(0) = "Respiration"
        strary(1) = m_NetworkManager.AscendancyRespTotal.ToString("F1")
        strary(2) = m_NetworkManager.AscendancyRespPer.ToString("F1")
        strary(3) = m_NetworkManager.OverheadRespTotal.ToString("F1")
        strary(4) = m_NetworkManager.OverheadRespPer.ToString("F1")
        strary(5) = m_NetworkManager.CapacityRespTotal.ToString("F1")
        strary(6) = m_NetworkManager.CapacityRespPer.ToString("F1")
        DataGrid.Rows(3).SetValues(strary)

        DataGrid.ClearSelection()

    End Sub

    Private Sub SetUpByGroupGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 7

        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = 110
            DataGrid.Columns(intColIndex).Frozen = False
        Next

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = 55

        DataGrid.Columns(1).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(1).Frozen = True

        For intIndex As Integer = 2 To 4
            DataGrid.Columns(intIndex).Width = 120
        Next

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = "Group name"
        DataGrid.Columns(2).HeaderText = "Ascendency (t/km2/year * bits)"
        DataGrid.Columns(3).HeaderText = "Overhead (t/km2/year * bits)"
        DataGrid.Columns(4).HeaderText = "Capacity (t/km2/year * bits)"
        DataGrid.Columns(5).HeaderText = "Information (bits)"
        DataGrid.Columns(6).HeaderText = "Throughput (t/km2/year)"

    End Sub

    Private Sub SetUpTotalGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 7

        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = 110
            DataGrid.Columns(intColIndex).Frozen = False
        Next

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True

        DataGrid.Columns(0).HeaderText = "Source"
        DataGrid.Columns(1).HeaderText = "Ascendency (flowbits)"
        DataGrid.Columns(2).HeaderText = "Ascendency (%)"
        DataGrid.Columns(3).HeaderText = "Overhead (flowbits)"
        DataGrid.Columns(4).HeaderText = "Overhead (%)"
        DataGrid.Columns(5).HeaderText = "Capacity (flowbits)"
        DataGrid.Columns(6).HeaderText = "Capacity (%)"

    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = Windows.Forms.DockStyle.Fill
        End If

    End Sub

End Class
