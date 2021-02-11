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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Controls

    ''' <summary>
    ''' Grid base class for presenting model outputs. The grid offers built-in
    ''' support for totalling columns.
    ''' </summary>
    <CLSCompliant(False)> _
    Public MustInherit Class gridResultsBase
        : Inherits cEwEGrid

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()
        End Sub

        Protected Overrides Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
            If ((ct And cStyleGuide.eChangeType.GroupVisibility) > 0) Or _
               ((ct And cStyleGuide.eChangeType.FleetVisibility) > 0) Then
                Me.RefreshContent()
            Else
                MyBase.OnStyleGuideChanged(ct)
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <param name="astrNames"></param>
        ''' <param name="aiCalc">Array with column indices to render as computed.</param>
        ''' <remarks></remarks>
        Protected Sub InitCells(iRow As Integer, astrNames() As String, aiCalc() As Integer)

            Dim cell As cEwECell = Nothing
            Dim cnt As Integer = Me.RowsCount - 1

            For rowIndex As Integer = (cnt + 1) To cnt + iRow - 1
                'Insert a new row
                Me.Rows.Insert(rowIndex)

                Me(rowIndex, 0) = New cEwERowHeaderCell(CStr(rowIndex))
                Me(rowIndex, 1) = New cEwERowHeaderCell(astrNames(rowIndex - cnt))

                For columnIndex As Integer = 2 To Me.ColumnsCount - 1

                    cell = New cEwECell(0.0!, GetType(Single))
                    cell.SuppressZero = True
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable

                    For i As Integer = 0 To aiCalc.Length - 1
                        If columnIndex = aiCalc(i) Then
                            cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed
                            Exit For
                        End If
                    Next

                    Me(rowIndex, columnIndex) = cell

                Next
            Next

            'The row for total value
            Me.Rows.Insert(cnt + iRow)
            Me(Me.RowsCount - 1, 0) = New cEwERowHeaderCell("")
            Me(Me.RowsCount - 1, 1) = New cEwERowHeaderCell(SharedResources.HEADER_TOTAL)

            For columnIndex As Integer = 2 To Me.ColumnsCount - 1

                cell = New cEwECell(0.0!, GetType(Single))
                cell.SuppressZero = True
                cell.Style = cStyleGuide.eStyleFlags.NotEditable

                For i As Integer = 0 To aiCalc.Length - 1
                    If columnIndex = aiCalc(i) Then
                        cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed
                        Exit For
                    End If
                Next
                Me(Me.RowsCount - 1, columnIndex) = cell
            Next

        End Sub

        Protected Sub SetCellValue(iRow As Integer, iCol As Integer, _
                                   sValue As Single, asValueTotal() As Single, _
                                   Optional styleExtra As cStyleGuide.eStyleFlags = 0)

            Dim cell As cEwECell = DirectCast(Me(iRow, iCol), cEwECell)

            If sValue >= 0 Then
                cell.Value = sValue
                asValueTotal(iCol) += sValue
            End If
            cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed Or styleExtra

        End Sub

        Protected Sub SetCellValue(iRow As Integer, iCol As Integer, sValue As String)
            Try
                Me(iRow, iCol).Value = sValue
            Catch ex As Exception
                'do nothing??
            End Try
        End Sub

        Protected Sub SetCellValue(iRow As Integer, iCol As Integer, sValue As Single)
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


