Public Class cPrimaryProdRequired
    Private m_NetworkManager As cNetworkManager
    Private m_Panel As Windows.Forms.Panel

    Public Sub New()
        '
    End Sub

    Public Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_Panel = Panel

        m_NetworkManager.RunRequiredPrimaryProd()
    End Sub

    Public Sub SetUpHarvestOfAllGroupsPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpHarvestGridColumn()

        'Set up grid rows
        'DataGrid.RowHeadersVisible = True
        'DataGrid.RowHeadersWidth = 70
        'DataGrid.RowHeadersDefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nGroups

        'DataGrid.RowHeadersDefaultCellStyle.BackColor = Drawing.Color.Beige

        For i As Integer = 1 To m_NetworkManager.nGroups
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.GroupName(i)
            strary(4) = m_NetworkManager.PPRRequired(i).ToString("F2")
            strary(5) = m_NetworkManager.PPRRequiredDetHarvest(i).ToString("F2")
            strary(6) = m_NetworkManager.PPRRequiredSumHarvest(i).ToString("F2")
            strary(7) = m_NetworkManager.PPRCatchHarvest(i).ToString("F2")
            strary(8) = m_NetworkManager.PPROverCatchHarvest(i).ToString("F2")
            strary(9) = m_NetworkManager.PPRTotPPHarvest(i).ToString("F2")
            strary(10) = m_NetworkManager.PPRUHarvest(i).ToString("F2")
            DataGrid.Rows(i - 1).SetValues(strary)

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next
        DataGrid.ClearSelection()

    End Sub

    Private Sub SetUpHarvestGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 11

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

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = "Group name"
        DataGrid.Columns(2).HeaderText = "No. of paths"
        DataGrid.Columns(3).HeaderText = "TL"
        DataGrid.Columns(4).HeaderText = "PPR (PP)"
        DataGrid.Columns(5).HeaderText = "PPR (Det)"
        DataGrid.Columns(6).HeaderText = "PPR"
        DataGrid.Columns(7).HeaderText = "Catch"
        DataGrid.Columns(8).HeaderText = "PPR/catch"
        DataGrid.Columns(9).HeaderText = "PPR/TotPP (%)"
        DataGrid.Columns(10).HeaderText = "PPR/u. catch"

    End Sub

    Public Sub SetUpConsumptionOfAllGroupsPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpConsumptionGridColumn()

        'Set up grid rows
        'DataGrid.RowHeadersVisible = True
        'DataGrid.RowHeadersWidth = 70
        'DataGrid.RowHeadersDefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nGroups

        'DataGrid.RowHeadersDefaultCellStyle.BackColor = Drawing.Color.Beige

        For i As Integer = 1 To m_NetworkManager.nGroups
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.GroupName(i)
            strary(4) = m_NetworkManager.PPRRequired(i).ToString("F2")
            strary(5) = m_NetworkManager.PPRRequiredDet(i).ToString("F2")
            strary(6) = m_NetworkManager.PPRRequiredSum(i).ToString("F2")
            strary(7) = m_NetworkManager.PPRCons(i).ToString("F2")
            strary(8) = m_NetworkManager.PPROverCons(i).ToString("F2")
            strary(9) = m_NetworkManager.PPRTotPP(i).ToString("F2")
            strary(10) = m_NetworkManager.PPRU(i).ToString("F2")
            DataGrid.Rows(i - 1).SetValues(strary)

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next
        DataGrid.ClearSelection()

    End Sub

    Private Sub SetUpConsumptionGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 11

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

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = "Group name"
        DataGrid.Columns(2).HeaderText = "No. of paths"
        DataGrid.Columns(3).HeaderText = "TL"
        DataGrid.Columns(4).HeaderText = "PPR (PP)"
        DataGrid.Columns(5).HeaderText = "PPR (Det)"
        DataGrid.Columns(6).HeaderText = "PPR"
        DataGrid.Columns(7).HeaderText = "Consumption"
        DataGrid.Columns(8).HeaderText = "PPR/consumption"
        DataGrid.Columns(9).HeaderText = "PPR/TotPP (%)"
        DataGrid.Columns(10).HeaderText = "PPR/u. biom"

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


