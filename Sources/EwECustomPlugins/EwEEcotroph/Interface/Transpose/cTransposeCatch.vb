Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace Grid

    Public Class cTransposeCatch

#Region "Public methods"
        Public Shared Sub DisplayData(ByVal DataGrid As DataGridView, ByVal Transpose As UserInterface.cTranspose, _
          ByVal FleetNumber As Integer)
            Cursor.Current = Cursors.WaitCursor
            SetUpGridColumn(DataGrid, Transpose.m_EcotrophManager)
            SetUpGridRow(DataGrid, Transpose.m_EcotrophManager, FleetNumber)
            Cursor.Current = Cursors.Default
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Shared Sub SetUpGridColumn(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager)
            DataGrid.ColumnCount = EcotrophManager.TransposeCatch.GetUpperBound(1) + 2
            cUtility.SetGridColumnPropertyDefault(DataGrid)

            DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
            DataGrid.Columns(0).Frozen = True
            DataGrid.Columns(0).Width = cUtility.ID_COL_WIDTH

            DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.LightGoldenrodYellow
            DataGrid.Columns(1).Frozen = True
            DataGrid.Columns(1).Width = cUtility.GRP_NAME_TRP_LVL_COL_WIDTH

            DataGrid.Columns(0).HeaderText = ""
            DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_GRP_NAMETL_TRP_LVLOUT
            For Col As Integer = 1 To EcotrophManager.EcopathData.NumLiving
                DataGrid.Columns(Col + 1).HeaderText = EcotrophManager.EcopathData.GroupName(Col) & _
                  Chr(10) & " (" & EcotrophManager.TLTuncated(Col) & ")"
            Next
        End Sub

        Private Shared Sub SetUpGridRow(ByVal DataGrid As DataGridView, ByVal EcotrophManager As cEcotrophManager, _
          ByVal FleetNumber As Integer)
            Dim RowContent() As String
            Dim TLOut As Single

            DataGrid.RowHeadersVisible = False
            DataGrid.RowCount = EcotrophManager.TransposeCatch.GetUpperBound(0)

            ReDim RowContent(DataGrid.Columns.Count)
            TLOut = 2
            For Row As Integer = 1 To EcotrophManager.TransposeCatch.GetUpperBound(0)
                RowContent(0) = CStr(Row)
                If Row = 1 Then
                    RowContent(1) = "1"
                Else
                    RowContent(1) = TLOut.ToString("0.#")
                End If
                For Col As Integer = 1 To EcotrophManager.TransposeCatch.GetUpperBound(1)
                    RowContent(Col + 1) = (EcotrophManager.TransposeCatch(Row, Col, FleetNumber)).ToString("F4")
                Next
                DataGrid.Rows(Row - 1).SetValues(RowContent)
                DataGrid.Rows(Row - 1).Visible = True
                If Row > 1 Then TLOut = CSng(TLOut + 0.1)
            Next

            DataGrid.ClearSelection()
        End Sub
#End Region 'Helper methods

    End Class

End Namespace
