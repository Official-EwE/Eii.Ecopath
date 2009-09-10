Imports EwECore

Public Class EcopathDataGrid
    Private m_core As cCore

    Public Sub New(ByVal core As cCore)
        m_core = core

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initializeGrid()
        populateDataGrid()

    End Sub

    ''' <summary>
    ''' Sets up the Grid
    ''' </summary>
    Private Sub initializeGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            ' Change the colors here
            .BackColor = Color.Gray
            .ForeColor = Color.White
            .Font = New Font(DataGridView1.Font, FontStyle.Bold)
        End With

        With DataGridView1
            .AutoSizeRowsMode = _
                DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .CellBorderStyle = DataGridViewCellBorderStyle.Single
            .GridColor = Color.Black
            .RowHeadersVisible = False
            .MultiSelect = False
            .Dock = DockStyle.Fill
        End With
    End Sub

    ''' <summary>
    ''' Polulates the grid
    ''' </summary>
    Private Sub populateDataGrid()
        Dim rowStr As String()

        ' Change the header names here
        Dim HeaderNames As String() = {"ID", "Names", "Biomass", "P/B"}

        With DataGridView1
            ' Set the number of columns
            .ColumnCount = HeaderNames.Length

            ' Write the headers
            For i As Integer = 0 To HeaderNames.Length - 1
                .Columns(i).Name = HeaderNames(i)
                .Columns(i).Width = 50
            Next i

            ' Write the body
            For i As Integer = 1 To m_core.nGroups
                rowStr = New String() {i, m_core.EcoPathGroupInputs(i).Name.ToString, m_core.EcoPathGroupInputs(i).BiomassAreaInput.ToString, m_core.EcoPathGroupInputs(i).PBInput.ToString}
                .Rows.Insert(i - 1, rowStr)
            Next i

        End With
    End Sub
End Class
