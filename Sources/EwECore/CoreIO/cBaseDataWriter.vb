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

Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports OfficeOpenXml

#End Region ' Imports

Namespace CoreIO

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Utility class to write bulk data to various sources (Excel, CSV and text, Clipboard) 
    ''' from Datatables or strong typed lists and collections.
    ''' </summary>
    ''' <remarks>
    ''' Original code by Jeroen Steenbeek, EII, for Safenet.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cBaseDataWriter

#Region " Generic "

        Public Shared Function Write(dt As DataTable, strFile As String, strFilter As String) As Boolean
            If (String.IsNullOrWhiteSpace(strFile)) Then
                Return WriteClipboard(dt)
            Else
                Select Case Path.GetExtension(strFile).ToLower()
                    Case ".xlsx", ".xlst" : Return SaveExcel(dt, strFile, strFilter)
                    'Case ".mdb", ".accdb" : Return LoadAccess(strFile, strFilter)
                    Case ".csv", ".txt" : Return SaveCSV(dt, strFile)
                    Case Else

                End Select
            End If
            Return Nothing
        End Function

#End Region ' Generic

#Region " Clipboard "

        Private Shared Function WriteClipboard(dt As DataTable) As Boolean
            Clipboard.Clear()
            Clipboard.SetText(WriteText(dt))
            Return True
        End Function

#End Region ' Clipboard

#Region " CSV "

        Private Shared Function SaveCSV(dt As DataTable, strCSVFile As String) As Boolean

            Dim bOK As Boolean = True
            Try
                Using sw As New StreamWriter(strCSVFile)
                    sw.Write(WriteText(dt))
                    sw.Flush()
                End Using
            Catch ex As Exception
                cLog.Write(ex)
                bOK = False
            End Try
            Return bOK

        End Function

#End Region ' CSV

#Region " Text "

        Private Shared Function WriteText(dt As DataTable) As String

            Try
                Dim sb As New StringBuilder()
                Dim bText As Boolean = False
                For iCol As Integer = 0 To dt.Columns.Count - 1
                    If (bText) Then sb.Append(",")
                    sb.Append(cStringUtils.ToCSVField(dt.Columns(iCol).ColumnName))
                    bText = True
                Next
                sb.AppendLine()

                For iRow As Integer = 0 To dt.Rows.Count - 1
                    bText = False
                    If (iRow > 0) Then sb.AppendLine()
                    Dim dr As DataRow = dt.Rows(iRow)
                    For iCol As Integer = 0 To dt.Columns.Count - 1
                        If (bText) Then sb.Append(",")
                        Dim val As Object = dr(iCol)
                        If dt.Columns(iCol).DataType Is GetType(String) Then
                            sb.Append(cStringUtils.ToCSVField(val))
                        ElseIf dt.Columns(iCol).DataType Is GetType(Boolean) Then
                            sb.Append(If(CBool(val), "1", "0"))
                        Else
                            sb.Append(cStringUtils.FormatDecimal(CDec(val)))
                        End If
                        bText = True
                    Next iCol
                Next iRow

                Return sb.ToString()

            Catch ex As Exception
                cLog.Write(ex)
            End Try
            Return ""

        End Function

#End Region ' Text

#Region " Excel "

        ''' <summary>
        ''' Save a datatable to an excel file.
        ''' </summary>
        ''' <param name="strExcelFile">The excel file to write.</param>
        ''' <param name="strWorksheet">The worksheet to write to.</param>
        ''' <returns></returns>
        Private Shared Function SaveExcel(dt As DataTable, strExcelFile As String, strWorksheet As String) As Boolean

            Dim bOK As Boolean = True
            Dim fi As New FileInfo(Path.GetFullPath(strExcelFile))

            Try
                Using pck As New ExcelPackage(fi)
                    Dim ws As ExcelWorksheet = Nothing
                    If (Not String.IsNullOrWhiteSpace(strWorksheet)) Then
                        ws = pck.Workbook.Worksheets.Add(strWorksheet)
                    Else
                        ws = pck.Workbook.Worksheets(0)
                    End If
                    ws.Cells("A1").LoadFromDataTable(dt, True)
                    pck.Save()
                End Using

            Catch ex As Exception
                cLog.Write(ex)
                bOK = False
            End Try
            Return bOK

        End Function

#End Region ' Excel

    End Class

End Namespace
