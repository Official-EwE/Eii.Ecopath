Public Class cFlowAndBiomasses
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

    Public Sub SetUpFromPrimaryProducersPanel()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nTrophicLevels

        For i As Integer = m_NetworkManager.nTrophicLevels To 1 Step -1
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.PPImport(i).ToString("F4")
            strary(2) = m_NetworkManager.PPConsByPred(i).ToString("F4")
            strary(3) = m_NetworkManager.PPExport(i).ToString("F4")
            strary(4) = m_NetworkManager.PPToDetritus(i).ToString("F4")
            strary(5) = m_NetworkManager.PPRespiration(i).ToString("F4")
            strary(6) = m_NetworkManager.PPThroughtput(i).ToString("F4")
            DataGrid.Rows(m_NetworkManager.nTrophicLevels - i).SetValues(strary)
        Next
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpFromDetritusPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nTrophicLevels

        For i As Integer = m_NetworkManager.nTrophicLevels To 1 Step -1
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.DetImport(i).ToString("F4")
            strary(2) = m_NetworkManager.DetConsByPred(i).ToString("F4")
            strary(3) = m_NetworkManager.DetExport(i).ToString("F4")
            strary(4) = m_NetworkManager.DetToDetritus(i).ToString("F4")
            strary(5) = m_NetworkManager.DetRespiration(i).ToString("F4")
            strary(6) = m_NetworkManager.DetThroughtput(i).ToString("F4")
            DataGrid.Rows(m_NetworkManager.nTrophicLevels - i).SetValues(strary)
        Next
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpFromAllCombinedPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nTrophicLevels

        For i As Integer = m_NetworkManager.nTrophicLevels To 1 Step -1
            strary(0) = CStr(i)
            strary(1) = (m_NetworkManager.DetImport(i) + m_NetworkManager.PPImport(i)).ToString("F4")
            strary(2) = (m_NetworkManager.DetConsByPred(i) + m_NetworkManager.PPConsByPred(i)).ToString("F4")
            strary(3) = (m_NetworkManager.DetExport(i) + m_NetworkManager.PPExport(i)).ToString("F4")
            strary(4) = (m_NetworkManager.DetToDetritus(i) + m_NetworkManager.PPToDetritus(i)).ToString("F4")
            strary(5) = (m_NetworkManager.DetRespiration(i) + m_NetworkManager.PPRespiration(i)).ToString("F4")
            strary(6) = (m_NetworkManager.DetThroughtput(i) + m_NetworkManager.PPThroughtput(i)).ToString("F4")
            DataGrid.Rows(m_NetworkManager.nTrophicLevels - i).SetValues(strary)
        Next
        DataGrid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn()
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

        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

        DataGrid.Columns(0).HeaderText = "Trophic level / Flow"
        DataGrid.Columns(1).HeaderText = "Import"
        DataGrid.Columns(2).HeaderText = "Consumption by predators"
        DataGrid.Columns(3).HeaderText = "Export"
        DataGrid.Columns(4).HeaderText = "Flow to detritus"
        DataGrid.Columns(5).HeaderText = "Respiration"
        DataGrid.Columns(6).HeaderText = "Throughput"

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
