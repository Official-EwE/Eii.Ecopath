' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports System.Data.OleDb
Imports System.IO
Imports System.Text

Public Class cGenericDataReader

#Region " Config "

    Public Enum eColumnNameCasingEnforcement As Integer
        None = 0
        UpperCase
        LowerCase
    End Enum

    Public Shared Property TextFieldSeparator As Char = ","c
    Public Shared Property TextDecimalSeparator As Char = "."c
    Public Shared Property ColumnNameEnforcement As eColumnNameCasingEnforcement = eColumnNameCasingEnforcement.None

#End Region ' Config

#Region " Loading "

    Public Shared ColumnMappings As New Dictionary(Of String, String)

    Public Shared CleanupColunmNames As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Read data into a datatable.
    ''' </summary>
    ''' <param name="strFile">The filename to load. If left empty an attempt 
    ''' is made to read data from the clipboard.</param>
    ''' <param name="strFilter">An optional filter, such as Excel worksheet name, 
    ''' Access database table, etc. This second value depends on the format of 
    ''' the file that is loaded.</param>
    ''' <returns>A datatable instance, or Nothing if something went wrong.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function Read(strFile As String, strFilter As String,
                                Optional ColumnTypes As Dictionary(Of String, Type) = Nothing) As DataTable
        If (Not String.IsNullOrWhiteSpace(strFile)) Then
            Select Case Path.GetExtension(strFile).ToLower()
                Case ".mdb", ".accdb", ".ewemdb", ".eweaccdb" : Return LoadAccess(strFile, strFilter)
                Case Else
                    Debug.Assert(False, "File format " & strFile & " not supported")
            End Select
        End If
        Return Nothing
    End Function

#End Region ' Loading

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

#Region " Internals "

    Private Shared Sub MapColumnNames(dt As DataTable)

        ' Map column names, if needed
        If (ColumnMappings Is Nothing) Then Return
        If (dt Is Nothing) Then Return

        For Each col As DataColumn In dt.Columns
            If ColumnMappings.ContainsKey(col.ColumnName) Then
                col.ColumnName = ColumnMappings(col.ColumnName)
            ElseIf CleanupColunmNames Then
                col.ColumnName = CleanColumnName(col.ColumnName)
            End If

            Select Case ColumnNameEnforcement
                Case eColumnNameCasingEnforcement.None
                Case eColumnNameCasingEnforcement.UpperCase
                    col.ColumnName = col.ColumnName.ToUpper
                Case eColumnNameCasingEnforcement.LowerCase
                    col.ColumnName = col.ColumnName.ToLower
            End Select
        Next

    End Sub

    Private Const AllowedColChars = "_-"

    Private Shared Function CleanColumnName(name As String) As String

        If (String.IsNullOrWhiteSpace(name)) Then Return ""

        name = name.Trim()

        Dim sbClean As New StringBuilder()
        Dim bInText As Boolean = Char.IsLetterOrDigit(name(0))
        Dim separator As String = ""

        For i As Integer = 0 To name.Length - 1
            Dim c As Char = name(i)
            If Char.IsLetterOrDigit(name(i)) Then
                If Not String.IsNullOrWhiteSpace(separator) Then
                    sbClean.Append(separator)
                    separator = ""
                End If
                sbClean.Append(name(i))
                bInText = True
            ElseIf (bInText = True) Then
                If AllowedColChars.Contains(c) Then
                    separator = CStr(c)
                Else
                    separator = "-"
                End If
                bInText = False
            End If
        Next
        Return sbClean.ToString()

    End Function

#End Region ' Internals

#Region " Utilities "

    Public Shared Function Value(Of T As IConvertible)(val As Object, valDefault As T) As T
        If Convert.IsDBNull(val) Then Return valDefault
        Return CType(Convert.ChangeType(val, GetType(T)), T)
    End Function

#End Region ' Utilities

End Class
