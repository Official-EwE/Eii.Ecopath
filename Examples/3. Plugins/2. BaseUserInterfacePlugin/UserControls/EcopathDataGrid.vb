' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
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
