Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class cProportionSTD

#Region "Public methods"
    Public Shared Sub DisplayData(ByVal DataGrid As DataGridView, ByVal Transpose As UserInterface.cTranspose)
        Cursor.Current = Cursors.WaitCursor
        SetUpGridColumn(DataGrid, Transpose.m_EcotrophManager.ProportionSTD)
        SetUpGridRow(DataGrid, Transpose.m_EcotrophManager.ProportionSTD)
        Cursor.Current = Cursors.Default
    End Sub
#End Region 'Public methods

#Region "Helper methods"
    Private Shared Sub SetUpGridColumn(ByVal DataGrid As DataGridView, ByVal ProportionSTD(,) As Single)
        Dim Col As Integer

        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = ProportionSTD.GetUpperBound(1) + 2
        cUtility.SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = cUtility.TRP_LVL_COL_WIDTH

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_TRP_LVLIN_TRP_LVLOUT
        DataGrid.Columns(2).HeaderText = "1"
        Col = 1
        For TLIn As Single = 2 To 5 Step 0.1
            DataGrid.Columns(Col + 2).HeaderText = TLIn.ToString("0.#")
            Col = Col + 1
        Next
    End Sub

    Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal ProportionSTD(,) As Single)
        Dim RowContent() As String
        Dim TLOut As Single

        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = ProportionSTD.GetUpperBound(0)

        ReDim RowContent(DataGrid.Columns.Count)
        TLOut = 2
        For Row As Integer = 1 To ProportionSTD.GetUpperBound(0)
            RowContent(0) = CStr(Row)
            If Row = 1 Then
                RowContent(1) = "1"
            Else
                RowContent(1) = TLOut.ToString("0.#")
            End If
            For Col As Integer = 1 To ProportionSTD.GetUpperBound(1)
                RowContent(Col + 1) = (ProportionSTD(Row, Col)).ToString("F4")
            Next
            DataGrid.Rows(Row - 1).SetValues(RowContent)
            DataGrid.Rows(Row - 1).Visible = True
            If Row > 1 Then TLOut = CSng(TLOut + 0.1)
        Next

        DataGrid.ClearSelection()
    End Sub
#End Region 'Helper methods

End Class
