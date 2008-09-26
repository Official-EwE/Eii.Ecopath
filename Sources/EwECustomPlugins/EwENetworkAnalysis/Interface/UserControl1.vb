Public Class UserControl1
    Private m_NetworkManager As cNetworkManager

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal NetworkManager As cNetworkManager)
        Me.New()
        m_NetworkManager = NetworkManager
    End Sub

    Public Sub DisplayRelativeFlows()
        Dim strary() As String
        ReDim strary(10)

        DataGridView1.ReadOnly = True
        DataGridView1.RowCount = 1
        DataGridView1.ColumnCount = 11
        DataGridView1.Columns(0).Name = ""
        DataGridView1.Columns(1).Name = "Group name / Trophic level"
        DataGridView1.Columns(2).Name = "I"
        DataGridView1.Columns(3).Name = "II"
        DataGridView1.Columns(4).Name = "III"
        DataGridView1.Columns(5).Name = "IV"
        DataGridView1.Columns(6).Name = "V"
        DataGridView1.Columns(7).Name = "VI"
        DataGridView1.Columns(8).Name = "VII"
        DataGridView1.Columns(9).Name = "VIII"
        DataGridView1.Columns(10).Name = "IX"

        For i As Integer = 1 To m_NetworkManager.nGroups
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.GroupName(i)
            For j As Integer = 1 To m_NetworkManager.nTrophicLevels
                strary(j + 1) = CStr((m_NetworkManager.RelativeFlow(i, j)))
            Next
            DataGridView1.Rows.Add(strary)
        Next
    End Sub

    Public Sub DisplayAbsoluteFlowsGrid()
        Dim strary() As String
        ReDim strary(10)

        DataGridView1.ReadOnly = True
        DataGridView1.RowCount = 1
        DataGridView1.ColumnCount = 11
        DataGridView1.Columns(0).Name = ""
        DataGridView1.Columns(1).Name = "Group name / Trophic level"
        DataGridView1.Columns(2).Name = "I"
        DataGridView1.Columns(3).Name = "II"
        DataGridView1.Columns(4).Name = "III"
        DataGridView1.Columns(5).Name = "IV"
        DataGridView1.Columns(6).Name = "V"
        DataGridView1.Columns(7).Name = "VI"
        DataGridView1.Columns(8).Name = "VII"
        DataGridView1.Columns(9).Name = "VIII"
        DataGridView1.Columns(10).Name = "IX"

        For i As Integer = 1 To m_NetworkManager.nGroups
            strary(0) = CStr(i)
            strary(1) = m_NetworkManager.GroupName(i)
            For j As Integer = 1 To m_NetworkManager.nTrophicLevels
                strary(j + 1) = CStr((m_NetworkManager.AbsoluteFlow(i, j)))
            Next
            DataGridView1.Rows.Add(strary)
        Next
    End Sub

End Class
