Imports System.Data.OleDb

Public Class cAccessReader
    Implements IReader

    Public Function TableNames(src As String) As String() Implements IReader.TableNames

        Dim conn As OleDbConnection = Nothing
        Dim names As New List(Of String)
        Dim bSuccess As Boolean = True

        Try
            conn = New OleDbConnection(String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", src))
            conn.Open()
        Catch ex As Exception
            bSuccess = False
        End Try

        If bSuccess Then
            Dim dtTables As DataTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New String() {Nothing, Nothing, Nothing, Nothing})
            For Each drow In dtTables.Rows
                Dim strName As String = CStr(drow("TABLE_NAME"))
                'Console.WriteLine("Table: " & strName)
                If Not strName.StartsWith("MSYS") Then
                    names.Add(strName)
                End If
            Next
            names.Sort()
        End If
        Return names.ToArray()

    End Function

    Public Function Read(src As String, table As String) As DataTable Implements IReader.Read
        Return cGenericDataReader.Read(src, table)
    End Function

End Class
