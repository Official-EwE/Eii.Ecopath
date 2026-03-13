Imports System.Data
Imports System.Reflection
Imports EwEUtils.NetUtilities

Namespace Database

    Public Module DataReaderDiff

        Public Class RowDiff
            Public Property ColumnName As String
            Public Property ValueA As Object
            Public Property ValueB As Object
            Public Overrides Function ToString() As String
                Return $"{ColumnName}: [{ValueA}] -> [{ValueB}]"
            End Function
        End Class

        Private _tableNameCache As New Dictionary(Of IDataReader, String)()
        Private _columnMapCache As New Dictionary(Of IDataReader, Dictionary(Of String, Integer))()

        Public Sub BroadcastDiffs(readerA As IDataReader, readerB As IDataReader, diffs As List(Of RowDiff), rowCount As Integer)
            Dim tableName = GetTableName(readerB)
            If diffs.Any() Then
                cWebSocketHelper.BroadcastMessage("table", tableName, "rowCount", rowCount, "rowDiffs", diffs.ToArray())
            Else
                cWebSocketHelper.BroadcastMessage("table", tableName, "rowCount", rowCount)
            End If
        End Sub

        Public Function CompareCurrentRow(readerA As IDataReader, readerB As IDataReader,
                                          getPropTypes As Func(Of String, Dictionary(Of String, Type))) As List(Of RowDiff)
            Dim diffs As New List(Of RowDiff)()
            Dim colsA = GetColumnMap(readerA)
            Dim colsB = GetColumnMap(readerB)
            Dim tableName = GetTableName(readerB)
            Dim propTypes = If(getPropTypes IsNot Nothing, getPropTypes(tableName), New Dictionary(Of String, Type)())

            For Each col In colsA.Keys
                If Not colsB.ContainsKey(col) Then Continue For
                Dim valA = CoerceValue(readerA(colsA(col)), col, propTypes)
                Dim valB = CoerceValue(readerB(colsB(col)), col, propTypes)
                If Not ObjectsEqual(valA, valB) Then
                    diffs.Add(New RowDiff With {
                        .ColumnName = col,
                        .ValueA = valA,
                        .ValueB = valB
                    })
                End If
            Next
            Return diffs
        End Function

        Private Function CoerceValue(value As Object, columnName As String, propTypes As Dictionary(Of String, Type)) As Object
            Dim targetType = propTypes(columnName)
            Dim underlying = If(Nullable.GetUnderlyingType(targetType), targetType)
            If underlying = GetType(String) AndAlso value Is DBNull.Value Then
                Return ""
            End If
            If value Is DBNull.Value OrElse value Is Nothing Then Return DBNull.Value
            If Not propTypes.ContainsKey(columnName) Then Return value
            If underlying = GetType(Boolean) AndAlso IsNumeric(value) Then
                Return value <> 0
            End If
            Return value
        End Function

        Private Function GetColumnMap(reader As IDataReader) As Dictionary(Of String, Integer)
            If _columnMapCache.ContainsKey(reader) Then Return _columnMapCache(reader)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To reader.FieldCount - 1
                map(reader.GetName(i).Trim()) = i
            Next
            _columnMapCache(reader) = map
            Return map
        End Function

        Private Function GetTableName(reader As IDataReader) As String
            If _tableNameCache.ContainsKey(reader) Then Return _tableNameCache(reader)
            Dim name = "Unknown"
            Try
                Dim schema = reader.GetSchemaTable()
                If schema IsNot Nothing AndAlso schema.Rows.Count > 0 Then
                    Dim val = schema.Rows(0)("BaseTableName")
                    If val IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(val.ToString()) Then
                        name = val.ToString()
                    End If
                End If
            Catch
            End Try
            _tableNameCache(reader) = name
            Return name
        End Function

        Private Const Tolerance As Double = 0.001 ' eg. 18765.7676 vs 18765.768

        Private Function ObjectsEqual(a As Object, b As Object) As Boolean
            If a Is DBNull.Value AndAlso b Is DBNull.Value Then Return True
            If a Is DBNull.Value OrElse b Is DBNull.Value Then Return False

            If TypeOf a Is Double OrElse TypeOf a Is Single OrElse
               TypeOf a Is Decimal OrElse TypeOf b Is Double OrElse
               TypeOf b Is Single OrElse TypeOf b Is Decimal Then
                Return Math.Abs(Convert.ToDouble(a) - Convert.ToDouble(b)) <= Tolerance
            End If
            Return a.Equals(b) OrElse a.ToString() = b.ToString()
        End Function

    End Module

End Namespace