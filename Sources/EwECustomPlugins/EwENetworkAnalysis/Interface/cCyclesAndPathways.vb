#Region "Imports directive"
Option Strict On
Option Explicit On

Imports EwECore

#End Region

Public Class cCyclesAndPathways
    Public Event AddToolStrip()

    Private m_NumGroups As Integer
    Private m_GroupNames() As String
    Private m_NetworkManager As cNetworkManager
    Private m_Panel As Windows.Forms.Panel

    Public Sub New()
        Dim core As cCore = cCore.GetInstance

        m_NumGroups = core.nGroups
    End Sub

    Public Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()

        m_NetworkManager = NetworkManager
        m_Panel = Panel

        ReDim m_GroupNames(m_NumGroups - 1)
        For intIndex As Integer = 0 To m_NumGroups - 1
            m_GroupNames(intIndex) = m_NetworkManager.GroupName(intIndex + 1)
        Next
    End Sub

    'Public ReadOnly Property NGroups() As Integer
    '    Get
    '        NGroups = m_NumGroups
    '    End Get
    'End Property

    'Public ReadOnly Property GroupNames(ByVal intGroupNum As Integer) As String
    '    Get
    '        Return m_GroupNames(intGroupNum)
    '    End Get
    'End Property


    Public Sub SetUpTL1ToConsumerPanel()
        SetUpGridColumn("Pathways from primary producer or detritus groups to a selected consumer")

        SetUpTL1ToConsumerToolStripAndRow()

    End Sub

    Private Sub SetUpGridColumn(ByVal strSecondColumnHeader As String)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        DataGrid.ColumnCount = 2

        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = 110
            DataGrid.Columns(intColIndex).Frozen = False
        Next

        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).HeaderText = "Pathway number"

        DataGrid.Columns(1).Width = 660
        DataGrid.Columns(1).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).HeaderText = strSecondColumnHeader

    End Sub

    Private Sub SetUpTL1ToConsumerToolStripAndRow()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim ToolStripLabel1 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripLabel2 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripCombo1 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox
        Dim ToolStripCombo2 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox


        RemoveToolStrip()
        RaiseEvent AddToolStrip()

        ToolStrip = CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        ToolStripLabel1 = CType(ToolStrip.Items("tslblSelection1"), Windows.Forms.ToolStripLabel)
        ToolStripLabel2 = CType(ToolStrip.Items("tslblSelection2"), Windows.Forms.ToolStripLabel)
        ToolStripCombo1 = CType(ToolStrip.Items("tscmbSelection1"), Windows.Forms.ToolStripComboBox)
        ToolStripCombo2 = CType(ToolStrip.Items("tscmbSelection2"), Windows.Forms.ToolStripComboBox)

        ToolStripLabel1.Text = "Pathways leading to:"
        ToolStripCombo1.Items.Clear()
        For intIndex As Integer = 0 To m_NumGroups - 1
            ToolStripCombo1.Items.Add(CStr(intIndex + 1) + ", " + m_GroupNames(intIndex))
        Next
        'This will trigger SetUpTL1ToConsumerRow()
        ToolStripCombo1.Text = CStr(1) + ", " + m_GroupNames(0)

        ToolStripLabel2.Visible = False
        ToolStripCombo2.Visible = False

    End Sub

    Public Sub SetUpTL1ToConsumerRow(ByVal intSelection1 As Integer)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(2) As String

        DataGrid.RowHeadersVisible = False

        m_NetworkManager.FindPathwaysToConsumer(intSelection1)
        If m_NetworkManager.PathWays.Count > 0 Then
            DataGrid.RowCount = m_NetworkManager.PathWays.Count
            For intPathwayIndex As Integer = 0 To m_NetworkManager.PathWays.Count - 1
                strary(0) = CStr(intPathwayIndex + 1)
                strary(1) = CStr(m_NetworkManager.PathWays.Item(intPathwayIndex))
                DataGrid.Rows(intPathwayIndex).SetValues(strary)
            Next
        Else
            DataGrid.RowCount = 1
            strary(0) = "No pathways found"
            strary(1) = ""
            DataGrid.Rows(0).SetValues(strary)
        End If
        DataGrid.ClearSelection()

    End Sub
    Public Sub SetUpTL1ToPreyToConsumerPanel()
        SetUpGridColumn("Pathways from primary producer or detritus groups to a selected consumer via a specified prey")

        SetUpTL1ToPreyToConsumerToolStripAndRow()

    End Sub

    Private Sub SetUpTL1ToPreyToConsumerToolStripAndRow()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim ToolStripLabel1 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripLabel2 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripCombo1 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox
        Dim ToolStripCombo2 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox

        RemoveToolStrip()
        RaiseEvent AddToolStrip()

        ToolStrip = CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        ToolStripLabel1 = CType(ToolStrip.Items("tslblSelection1"), Windows.Forms.ToolStripLabel)
        ToolStripLabel2 = CType(ToolStrip.Items("tslblSelection2"), Windows.Forms.ToolStripLabel)
        ToolStripCombo1 = CType(ToolStrip.Items("tscmbSelection1"), Windows.Forms.ToolStripComboBox)
        ToolStripCombo2 = CType(ToolStrip.Items("tscmbSelection2"), Windows.Forms.ToolStripComboBox)

        ToolStripLabel1.Text = "Pathways leading to:"
        ToolStripCombo1.Items.Clear()
        For intIndex As Integer = 0 To m_NumGroups - 1
            ToolStripCombo1.Items.Add(CStr(intIndex + 1) + ", " + m_GroupNames(intIndex))
        Next
        'This will or will NOT trigger SetUpTL1ToPreyToConsumerRow()
        ToolStripCombo1.Text = CStr(1) + ", " + m_GroupNames(0)

        ToolStripLabel2.Visible = True
        ToolStripLabel2.Text = "via:"
        ToolStripCombo2.Visible = True
        ToolStripCombo2.Items.Clear()
        For intIndex As Integer = 0 To m_NumGroups - 1
            ToolStripCombo2.Items.Add(CStr(intIndex + 1) + ", " + m_GroupNames(intIndex))
        Next
        'This will trigger SetUpTL1ToPreyToConsumerRow()
        ToolStripCombo2.Text = CStr(1) + ", " + m_GroupNames(0)

    End Sub

    Public Sub SetUpTL1ToPreyToConsumerRow(ByVal intSelection1 As Integer, ByVal intSelection2 As Integer)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(2) As String

        DataGrid.RowHeadersVisible = False

        m_NetworkManager.FindPathwaysToConsumerViaPrey(intSelection1, intSelection2)
        If m_NetworkManager.PathWays.Count > 0 Then
            DataGrid.RowCount = m_NetworkManager.PathWays.Count
            For intPathwayIndex As Integer = 0 To m_NetworkManager.PathWays.Count - 1
                strary(0) = CStr(intPathwayIndex + 1)
                strary(1) = CStr(m_NetworkManager.PathWays.Item(intPathwayIndex))
                DataGrid.Rows(intPathwayIndex).SetValues(strary)
            Next
        Else
            DataGrid.RowCount = 1
            strary(0) = "No pathways found"
            strary(1) = ""
            DataGrid.Rows(0).SetValues(strary)
        End If
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpPreyToTopPredatorPanel()
        SetUpGridColumn("Pathways from the specified group to all top predators")

        SetUpPreyToTopPredatorToolStripAndRow()

    End Sub

    Private Sub SetUpPreyToTopPredatorToolStripAndRow()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim ToolStripLabel1 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripLabel2 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripCombo1 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox
        Dim ToolStripCombo2 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox

        RemoveToolStrip()
        RaiseEvent AddToolStrip()

        ToolStrip = CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        ToolStripLabel1 = CType(ToolStrip.Items("tslblSelection1"), Windows.Forms.ToolStripLabel)
        ToolStripLabel2 = CType(ToolStrip.Items("tslblSelection2"), Windows.Forms.ToolStripLabel)
        ToolStripCombo1 = CType(ToolStrip.Items("tscmbSelection1"), Windows.Forms.ToolStripComboBox)
        ToolStripCombo2 = CType(ToolStrip.Items("tscmbSelection2"), Windows.Forms.ToolStripComboBox)

        ToolStripLabel1.Text = "Pathways from:"
        ToolStripCombo1.Items.Clear()
        For intIndex As Integer = 0 To m_NumGroups - 1
            ToolStripCombo1.Items.Add(CStr(intIndex + 1) + ", " + m_GroupNames(intIndex))
        Next
        'This will trigger SetUpPreyToTopPredatorRow()
        ToolStripCombo1.Text = CStr(1) + ", " + m_GroupNames(0)

        ToolStripLabel2.Visible = False
        ToolStripCombo2.Visible = False

    End Sub

    Public Sub SetUpPreyToTopPredatorRow(ByVal intSelection1 As Integer)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(2) As String

        DataGrid.RowHeadersVisible = False

        m_NetworkManager.FindPathwaysFromPrey(intSelection1)
        If m_NetworkManager.PathWays.Count > 0 Then
            DataGrid.RowCount = m_NetworkManager.PathWays.Count
            For intPathwayIndex As Integer = 0 To m_NetworkManager.PathWays.Count - 1
                strary(0) = CStr(intPathwayIndex + 1)
                strary(1) = CStr(m_NetworkManager.PathWays.Item(intPathwayIndex))
                DataGrid.Rows(intPathwayIndex).SetValues(strary)
            Next
        Else
            DataGrid.RowCount = 1
            strary(0) = "No pathways found"
            strary(1) = ""
            DataGrid.Rows(0).SetValues(strary)
        End If
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpCyclesLivingPanel()
        SetUpGridColumn("Cycles are linked pathways, starting from a group and returning to it")

        SetUpCyclesLivingToolStripAndRow()

    End Sub

    Private Sub SetUpCyclesLivingToolStripAndRow()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)

        RemoveToolStrip()

        SetUpCyclesLivingRow()
    End Sub

    Private Sub SetUpCyclesLivingRow()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(2) As String

        DataGrid.RowHeadersVisible = False

        m_NetworkManager.FindPathwaysCycles()
        If m_NetworkManager.PathWays.Count > 0 Then
            DataGrid.RowCount = m_NetworkManager.PathWays.Count
            For intPathwayIndex As Integer = 0 To m_NetworkManager.PathWays.Count - 1
                strary(0) = CStr(intPathwayIndex + 1)
                strary(1) = CStr(m_NetworkManager.PathWays.Item(intPathwayIndex))
                DataGrid.Rows(intPathwayIndex).SetValues(strary)
            Next
        Else
            DataGrid.RowCount = 1
            strary(0) = "No pathways found"
            strary(1) = ""
            DataGrid.Rows(0).SetValues(strary)
        End If
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpCyclesAllPanel()
        SetUpGridColumn("Cycles are linked pathways, starting from a group and returning to it")

        SetUpCyclesAllToolStripAndRow()

    End Sub

    Private Sub SetUpCyclesAllToolStripAndRow()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)

        RemoveToolStrip()

        SetUpCyclesAllRow()
    End Sub

    Private Sub SetUpCyclesAllRow()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(2) As String

        DataGrid.RowHeadersVisible = False

        m_NetworkManager.FindPathwaysCyclesAll()
        If m_NetworkManager.PathWays.Count > 0 Then
            DataGrid.RowCount = m_NetworkManager.PathWays.Count
            For intPathwayIndex As Integer = 0 To m_NetworkManager.PathWays.Count - 1
                strary(0) = CStr(intPathwayIndex + 1)
                strary(1) = CStr(m_NetworkManager.PathWays.Item(intPathwayIndex))
                DataGrid.Rows(intPathwayIndex).SetValues(strary)
            Next
        Else
            DataGrid.RowCount = 1
            strary(0) = "No pathways found"
            strary(1) = ""
            DataGrid.Rows(0).SetValues(strary)
        End If
        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpTL1ToConsumerSummaryPanel()
        SetUpSummaryGridColumn()

        SetUpTL1ToConsumerSummaryToolStripAndRow()

    End Sub
    Private Sub SetUpSummaryGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        DataGrid.ColumnCount = 2

        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = 110
            DataGrid.Columns(intColIndex).Frozen = False
        Next

        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).HeaderText = "Parameter"

        DataGrid.Columns(1).HeaderText = "Value"

    End Sub
    Private Sub SetUpTL1ToConsumerSummaryToolStripAndRow()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)

        RemoveToolStrip()

        SetUpTL1ToConsumerSummaryRow()

    End Sub

    Private Sub SetUpTL1ToConsumerSummaryRow()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(2) As String

        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = 1

        strary(0) = ""
        strary(1) = ""
        DataGrid.Rows(0).SetValues(strary)

        DataGrid.ClearSelection()

    End Sub

    Public Sub SetUpCyclingAndPathLengthPanel()
        SetUpCyclingAndPathLengthGridColumn()

        SetUpCyclingAndPathLengthToolStripAndRow()

    End Sub

    Private Sub SetUpCyclingAndPathLengthGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        DataGrid.ReadOnly = True
        DataGrid.ColumnCount = 3

        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = 110
            DataGrid.Columns(intColIndex).Frozen = False
        Next

        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).HeaderText = "Parameter"

        DataGrid.Columns(1).HeaderText = "Value"

        DataGrid.Columns(2).HeaderText = "Unit"

    End Sub

    Private Sub SetUpCyclingAndPathLengthToolStripAndRow()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)

        RemoveToolStrip()

        SetUpCyclingAndPathLengthRow()
    End Sub

    Private Sub SetUpCyclingAndPathLengthRow()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strary(3) As String

        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = 1

        strary(0) = ""
        strary(1) = ""
        strary(2) = ""
        DataGrid.Rows(0).SetValues(strary)

        DataGrid.ClearSelection()

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
