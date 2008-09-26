Public Class cTrophicLevelDecomp
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

    Public Sub SetUpRelativeFlowsPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpGridColumn(m_NetworkManager.nTrophicLevels)

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
            For j As Integer = 1 To m_NetworkManager.nTrophicLevels
                strary(j + 1) = (m_NetworkManager.RelativeFlow(i, j)).ToString("F4")
            Next
            'DataGrid.Rows.Add(strary)
            DataGrid.Rows(i - 1).SetValues(strary)


            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpAbsoluteFlowsPanel()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(m_NetworkManager.nTrophicLevels + 1) As String

        RemoveToolStrip()

        SetUpGridColumn(m_NetworkManager.nTrophicLevels)

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
            For j As Integer = 1 To m_NetworkManager.nTrophicLevels
                strary(j + 1) = (m_NetworkManager.AbsoluteFlow(i, j)).ToString("F4")
            Next
            'DataGrid.Rows.Add(strary)
            DataGrid.Rows(i - 1).SetValues(strary)

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next
        DataGrid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn(ByVal iNumTrophicLevels As Integer)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = iNumTrophicLevels + 2

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
        DataGrid.Columns(1).HeaderText = "Group name / Trophic level"
        DataGrid.Columns(2).HeaderText = "I"
        DataGrid.Columns(3).HeaderText = "II"
        DataGrid.Columns(4).HeaderText = "III"
        DataGrid.Columns(5).HeaderText = "IV"
        DataGrid.Columns(6).HeaderText = "V"
        DataGrid.Columns(7).HeaderText = "VI"
        DataGrid.Columns(8).HeaderText = "VII"
        DataGrid.Columns(9).HeaderText = "VIII"
        DataGrid.Columns(10).HeaderText = "IX"

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
