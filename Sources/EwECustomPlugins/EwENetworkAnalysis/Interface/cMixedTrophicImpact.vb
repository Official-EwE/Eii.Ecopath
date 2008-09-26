Public Class cMixedTrophicImpact
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

    Public Sub SetUpPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nGroups + 7 + 1) As String

        RemoveToolStrip()

        SetUpGridColumn(m_NetworkManager.nGroups, 7)

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nGroups

        For i As Integer = 1 To m_NetworkManager.nGroups
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.GroupName(i)
            For j As Integer = 1 To m_NetworkManager.nGroups
                strary(j + 1) = (m_NetworkManager.MixedTrophicImpacts(i, j)).ToString("F4")
            Next
            DataGrid.Rows(i - 1).SetValues(strary)
        Next
        DataGrid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn(ByVal iNumGroups As Integer, ByVal iNumFleets As Integer)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = iNumGroups + iNumFleets + 2

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
        DataGrid.Columns(1).HeaderText = "Impacting / Impacted"
        For intIndex As Integer = 1 To iNumGroups
            DataGrid.Columns(intIndex + 1).HeaderText = m_NetworkManager.GroupName(intIndex)
        Next

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
