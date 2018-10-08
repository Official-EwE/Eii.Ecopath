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
Imports System.Data.OleDb
Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports OfficeOpenXml

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Utility class to read bulk data from various sources (Excel, Access, CSV, Clipboard) 
''' into Datatables or strong typed lists and collections.
''' </summary>
''' <remarks>
''' Original code by Jeroen Steenbeek, EII, for Safenet.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cBaseDataReader

#Region " Config "

    Public Shared Property TextFieldSeparator As Char = ","c
    Public Shared Property TextDecimalSeparator As Char = "."c

#End Region ' Config

#Region " Loading "

    Public Shared ColumnMappings As New Dictionary(Of String, String)

    ''' <summary>
    ''' Read data into a datatable.
    ''' </summary>
    ''' <param name="strFile">The filename to load. If left empty an attempt is made to read data from the clipboard.</param>
    ''' <param name="strFilter">An optional filter, such as Excel worksheet name, Access database table, etc. This second value depends on the format of the file that is loaded.</param>
    ''' <returns>A datatable instance, or Nothing if something went wrong.</returns>
    Public Shared Function Read(strFile As String, strFilter As String,
                                Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As DataTable
        If (Not String.IsNullOrWhiteSpace(strFile)) Then
            Select Case Path.GetExtension(strFile).ToLower()
                Case ".xlsx", ".xlst" : Return LoadExcel(strFile, strFilter, ColumnTypes)
                Case ".mdb", ".accdb" : Return LoadAccess(strFile, strFilter)
                Case ".csv", ".txt" : Return LoadCSV(strFile, ColumnTypes)
                Case Else
                    Debug.Assert(False, "File format " & strFile & " not supported")
            End Select
        Else
            Return LoadClipboard()
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Read data into a templated list.
    ''' </summary>
    ''' <param name="strFile">The filename to load. If left empty an attempt is made to read data from the clipboard.</param>
    ''' <param name="strFilter">An optional filter, such as Excel worksheet name, Access database table, etc. This second value depends on the format of the file that is loaded.</param>
    ''' <returns>True if successful.</returns>
    Public Shared Function Read(Of T)(strFile As String, strFilter As String, data As List(Of T),
                                      Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As Boolean
        Return cDataTableConverter.ToList(Read(strFile, strFilter, ColumnTypes), data)
    End Function

    ''' <summary>
    ''' Read data into a templated dictionary.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="strFile">The filename to load. If left empty an attempt is made to read data from the clipboard.</param>
    ''' <param name="strFilter">An optional filter, such as Excel worksheet name, Access database table, etc. This second value depends on the format of the file that is loaded.</param>
    ''' <param name="data">The dictionary to load the file into.</param>
    ''' <param name="strField">The field name to use as dictionary key.</param>
    ''' <returns></returns>
    Public Shared Function Read(Of T)(strFile As String, strFilter As String, data As Dictionary(Of String, T), strField As String,
                                      Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As Boolean
        Return cDataTableConverter.ToDictionary(Read(strFile, strFilter, ColumnTypes), data, strField)
    End Function

#End Region ' Loading

#Region " Excel "

    ''' <summary>
    ''' Loads an excel file into a datatable.
    ''' </summary>
    ''' <param name="strExcelFile">The string excel file.</param>
    ''' <param name="strWorksheet">The string worksheet.</param>
    ''' <returns></returns>
    Private Shared Function LoadExcel(strExcelFile As String, strWorksheet As String,
                                      Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As DataTable

        Dim bOK As Boolean = True
        Dim dt As New DataTable()

        strExcelFile = Path.GetFullPath(strExcelFile)

        Using pck As New ExcelPackage()
            Try
                Using strm As Stream = File.OpenRead(strExcelFile)
                    pck.Load(strm)
                End Using
            Catch ex As Exception
                Return Nothing
            End Try

            Dim ws As ExcelWorksheet = Nothing
            If (Not String.IsNullOrWhiteSpace(strWorksheet)) Then
                For Each wsTemp As ExcelWorksheet In pck.Workbook.Worksheets
                    If (String.Compare(wsTemp.Name, strWorksheet, True) = 0) Then ws = wsTemp
                Next
                If (ws Is Nothing) Then
                    Return Nothing
                End If
            Else
                ws = pck.Workbook.Worksheets.First
            End If

            Dim nCols As Integer = ws.Dimension.End.Column

            For iCol As Integer = 1 To nCols
                Dim cell As ExcelRange = ws.Cells(1, iCol, 1, iCol)
                Dim col As String = cell.Text
                Dim type As Type = GetType(String)
                If (ColumnTypes IsNot Nothing) Then
                    If ColumnTypes.ContainsKey(col) Then type = ColumnTypes(col)
                End If
                dt.Columns.Add(col, type)
            Next

            For iRow As Integer = 2 To ws.Dimension.End.Row
                Dim drow As DataRow = dt.NewRow()
                For iCol As Integer = 1 To nCols
                    Dim cell As ExcelRange = ws.Cells(iRow, iCol, iRow, iCol)
                    drow(iCol - 1) = cell.Value
                Next
                dt.Rows.Add(drow)
            Next

        End Using

        MapColumnNames(dt)

        Return dt

    End Function

#End Region ' Excel

#Region " Access "

    Private Shared Function LoadAccess(strAccess As String, strTable As String) As DataTable

        Dim conn As OleDbConnection = Nothing
        Dim bOK As Boolean = True
        Dim dt As New DataTable()

        strAccess = Path.GetFullPath(strAccess)

        Try
            conn = New OleDbConnection(String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", strAccess))
            conn.Open()
        Catch ex As Exception
            Return Nothing
        End Try

        Try
            Dim cmd As New OleDbCommand("SELECT * FROM " & strTable, conn)
            Dim da As New OleDbDataAdapter()
            da.SelectCommand = cmd
            dt.Reset()
            da.Fill(dt)
            da.Dispose()
        Catch ex As Exception
            bOK = False
        End Try

        MapColumnNames(dt)

        conn.Close()
        conn.Dispose()

        If (bOK) Then
            'StatusHandler.Log("Access file '" & strAccess & "', table  '" & strTable & "' loaded", eAlert.OK)
        End If

        Return dt

    End Function

#End Region ' Access

#Region " CSV "

    Private Shared Function LoadCSV(strCSV As String, Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As DataTable

        Dim dt As DataTable = Nothing

        strCSV = Path.GetFullPath(strCSV)

        Try
            Using sr As New StreamReader(strCSV)
                dt = LoadText(sr, ColumnTypes)
            End Using
            cLog.Write("CSV file '" & strCSV & "' loaded")

        Catch ex As Exception
            cLog.Write(ex)
        End Try

        Return dt

    End Function

#End Region ' CSV

#Region " Clipboard "

    Private Shared Function LoadClipboard(Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As DataTable

        Dim dt As DataTable = Nothing
        If Clipboard.ContainsText Then
            Dim text As String = Clipboard.GetText()
            Using sr As New StringReader(text)
                dt = LoadText(sr, ColumnTypes)
            End Using
        End If
        Return dt

    End Function

#End Region ' Clipboard

#Region " Text "

    Private Shared Function NumberInfo() As NumberFormatInfo
        Dim ci As CultureInfo = CultureInfo.GetCultureInfo("en-US")
        Dim nf As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)
        nf.NumberDecimalSeparator = TextDecimalSeparator
        nf.PercentDecimalSeparator = TextDecimalSeparator
        nf.CurrencyDecimalSeparator = TextDecimalSeparator
        Return nf
    End Function

    Private Shared Function LoadText(sr As TextReader,
                                     Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As DataTable

        Try
            Dim strLine As String = sr.ReadLine()
            Dim strArray() As String = cStringUtils.SplitQualified(strLine, TextFieldSeparator)
            Dim dt As New DataTable()
            Dim row As DataRow = Nothing
            Dim nf As NumberFormatInfo = NumberInfo()

            For Each col As String In strArray
                Dim type As Type = GetType(String)
                If (ColumnTypes IsNot Nothing) Then
                    ' Default
                    If ColumnTypes.ContainsKey("*") Then type = ColumnTypes("*")
                    ' Specific
                    If ColumnTypes.ContainsKey(col) Then type = ColumnTypes(col)
                End If
                dt.Columns.Add(New DataColumn(col, type))
            Next

            strLine = sr.ReadLine
            While Not String.IsNullOrEmpty(strLine)
                row = dt.NewRow()
                ' Parse
                strArray = cStringUtils.SplitQualified(strLine, TextFieldSeparator)
                Dim data(dt.Columns.Count - 1) As Object
                For i As Integer = 0 To dt.Columns.Count - 1
                    Select Case dt.Columns(i).DataType
                        Case GetType(Single) : data(i) = Single.Parse(strArray(i), nf)
                        Case GetType(Double) : data(i) = Double.Parse(strArray(i), nf)
                        Case GetType(Decimal) : data(i) = Decimal.Parse(strArray(i), nf)
                        Case Else : data(i) = strArray(i)
                    End Select
                Next
                row.ItemArray = data
                dt.Rows.Add(row)
                strLine = sr.ReadLine
            End While

            MapColumnNames(dt)

            'StatusHandler.Log("Text loaded", eAlert.OK)
            Return dt

        Catch ex As Exception
            'StatusHandler.Log("Unable to load text: " & ex.Message, eAlert.Error)
        End Try
        Return Nothing

    End Function

#End Region ' Text

#Region " Internals "

    Private Shared Sub MapColumnNames(dt As DataTable)

        ' Map column names, if needed
        If (ColumnMappings Is Nothing) Then Return
        If (dt Is Nothing) Then Return

        For Each col As DataColumn In dt.Columns
            If ColumnMappings.ContainsKey(col.ColumnName) Then
                col.ColumnName = ColumnMappings(col.ColumnName)
            End If
        Next

    End Sub

#End Region ' Internals

End Class
