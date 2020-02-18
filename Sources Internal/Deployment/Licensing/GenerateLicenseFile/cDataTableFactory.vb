Imports System.Reflection

Public Class cDataTableFactory

    Public Shared Function ToDataTable(Of T)(collection As IEnumerable(Of T), tableName As String) As DataTable
        Dim tbl As DataTable = ToDataTable(collection)
        tbl.TableName = tableName
        Return tbl
    End Function

    Public Shared Function ToDataTable(Of T)(collection As IEnumerable(Of T)) As DataTable
        Dim dt As New DataTable()
        Dim ti As Type = GetType(T)

        Dim pia() As PropertyInfo = ti.GetProperties()
        Dim temp As Object
        Dim dr As DataRow

        Dim props As New List(Of PropertyInfo)
        For i As Integer = 0 To pia.Count - 1
            Dim pi As PropertyInfo = pia(i)
            If (pi.CanRead And pi.CanWrite) Then
                dt.Columns.Add(pi.Name, If(Nullable.GetUnderlyingType(pi.PropertyType), pi.PropertyType))
                dt.Columns(i).AllowDBNull = True
                props.Add(pi)
            End If
        Next

        ' Populate the table
        For Each item As T In collection
            dr = dt.NewRow()
            For i As Integer = 0 To props.Count - 1
                Dim pi As PropertyInfo = props(i)
                temp = pi.GetValue(item, Nothing)

                If (temp Is Nothing) Then
                    dr(pi.Name) = DBNull.Value
                Else
                    dr(pi.Name) = temp
                End If
            Next
            dt.Rows.Add(dr)
        Next
        Return dt

    End Function

End Class
