#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style

#End Region

Namespace Controls

    <CLSCompliant(False)> _
    Public MustInherit Class gridResultsBase
        : Inherits EwEGrid

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()
        End Sub

        Protected Sub InitCells(ByVal iRow As Integer, ByRef astrNames() As String, ByVal asCalc() As Integer)

            Dim cell As EwECell = Nothing
            Dim cnt As Integer = Me.RowsCount - 1

            For rowIndex As Integer = (cnt + 1) To cnt + iRow - 1
                'Insert a new row
                Me.Rows.Insert(rowIndex)

                Me(rowIndex, 0) = New EwERowHeaderCell(rowIndex)
                Me(rowIndex, 1) = New EwERowHeaderCell(astrNames(rowIndex - cnt))

                For columnIndex As Integer = 2 To Me.ColumnsCount - 1

                    cell = New EwECell(0.0!, GetType(Single))
                    cell.SuppressZero = True
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable

                    For i As Integer = 0 To asCalc.Length - 1
                        If columnIndex = asCalc(i) Then
                            cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed
                            Exit For
                        End If
                    Next

                    Me(rowIndex, columnIndex) = cell

                Next
            Next

            'The row for total value
            Me.Rows.Insert(cnt + iRow)
            Me(Me.RowsCount - 1, 0) = New EwERowHeaderCell("")
            Me(Me.RowsCount - 1, 1) = New EwERowHeaderCell(My.Resources.HEADER_TOTAL)

            For columnIndex As Integer = 2 To Me.ColumnsCount - 1

                cell = New EwECell(0.0!, GetType(Single))
                cell.SuppressZero = True
                cell.Style = cStyleGuide.eStyleFlags.NotEditable

                For i As Integer = 0 To asCalc.Length - 1
                    If columnIndex = asCalc(i) Then
                        cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed
                        Exit For
                    End If
                Next
                Me(Me.RowsCount - 1, columnIndex) = cell
            Next

        End Sub

        Protected Sub SetCellValue(ByVal iRow As Integer, ByVal iCol As Integer, ByVal sValue As Single, ByVal asValueTotal() As Single)

            If sValue >= 0 Then
                Me(iRow, iCol).Value = sValue
                asValueTotal(iCol) += sValue
            End If

        End Sub

        Protected Sub SetCellValue(ByVal iRow As Integer, ByVal iCol As Integer, ByVal sValue As String)
            Try
                Me(iRow, iCol).Value = sValue
            Catch ex As Exception
                'do nothing??
            End Try
        End Sub

        Protected Sub SetCellValue(ByVal iRow As Integer, ByVal iCol As Integer, ByVal sValue As Single)
            Try
                Me(iRow, iCol).Value = sValue
            Catch ex As Exception
                'do nothing??
            End Try
        End Sub

        Protected Sub InitTotalArray(ByRef asValueTotal() As Single)
            'The array for storing total values
            For i As Integer = 2 To asValueTotal.Length - 1
                asValueTotal(i) = 0
            Next

        End Sub

        Protected Overrides Sub FinishStyle()

            MyBase.FinishStyle()

            'Set column width
            Me.Columns(0).Width = 20

            For columnIndex As Integer = 2 To Me.ColumnsCount - 1
                Me.Columns(columnIndex).Width = 60
            Next

            Me.FixedColumns = 2

        End Sub

    End Class

End Namespace


