Imports System.Data

Namespace Database

    ''' <summary>
    ''' Wraps an IDataReader and coerces values based on expected property types.
    ''' Used to normalize secondary reader values before comparison (e.g. SQLite vs Access).
    ''' </summary>
    Public Class cCoercedDataReader
        Implements IDataReader

        Private ReadOnly _inner As IDataReader
        Private _propTypes As Dictionary(Of String, Type)

        Public Sub New(inner As IDataReader)
            _inner = inner
        End Sub

        Public Property PropTypes As Dictionary(Of String, Type)
            Get
                Return _propTypes
            End Get
            Set(value As Dictionary(Of String, Type))
                _propTypes = value
            End Set
        End Property

        Private Function Coerce(value As Object, columnName As String) As Object
            Dim targetType As Type = Nothing
            If Not PropTypes.TryGetValue(columnName, targetType) Then Return value
            Dim underlying = If(Nullable.GetUnderlyingType(targetType), targetType)
            If underlying = GetType(String) AndAlso value Is DBNull.Value Then Return ""
            If value Is DBNull.Value OrElse value Is Nothing Then Return DBNull.Value
            If underlying = GetType(Boolean) AndAlso IsNumeric(value) Then Return value <> 0

            Return value
        End Function

        ' --- Coerced accessors ---

        Public Function GetValue(i As Integer) As Object Implements IDataReader.GetValue
            Return Coerce(_inner.GetValue(i), _inner.GetName(i))
        End Function

        Default Public ReadOnly Property Item(i As Integer) As Object Implements IDataRecord.Item
            Get
                Return Coerce(_inner(i), _inner.GetName(i))
            End Get
        End Property

        Default Public ReadOnly Property Item(name As String) As Object Implements IDataRecord.Item
            Get
                Return Coerce(_inner(name), name)
            End Get
        End Property

        ' --- Pass-through implementations ---

        Public Function Read() As Boolean Implements IDataReader.Read
            Return _inner.Read()
        End Function

        Public Sub Close() Implements IDataReader.Close
            _inner.Close()
        End Sub

        Public ReadOnly Property Depth As Integer Implements IDataReader.Depth
            Get
                Return _inner.Depth
            End Get
        End Property

        Public ReadOnly Property FieldCount As Integer Implements IDataReader.FieldCount
            Get
                Return _inner.FieldCount
            End Get
        End Property

        Public ReadOnly Property IsClosed As Boolean Implements IDataReader.IsClosed
            Get
                Return _inner.IsClosed
            End Get
        End Property

        Public ReadOnly Property RecordsAffected As Integer Implements IDataReader.RecordsAffected
            Get
                Return _inner.RecordsAffected
            End Get
        End Property

        Public Function GetName(i As Integer) As String Implements IDataReader.GetName
            Return _inner.GetName(i)
        End Function

        Public Function GetOrdinal(name As String) As Integer Implements IDataRecord.GetOrdinal
            Return _inner.GetOrdinal(name)
        End Function

        Public Function GetSchemaTable() As DataTable Implements IDataReader.GetSchemaTable
            Return _inner.GetSchemaTable()
        End Function

        Public Function NextResult() As Boolean Implements IDataReader.NextResult
            _propTypes = Nothing ' reset cache for next result set
            Return _inner.NextResult()
        End Function

        Public Function GetBoolean(i As Integer) As Boolean Implements IDataRecord.GetBoolean
            Return _inner.GetBoolean(i)
        End Function

        Public Function GetByte(i As Integer) As Byte Implements IDataRecord.GetByte
            Return _inner.GetByte(i)
        End Function

        Public Function GetBytes(i As Integer, fieldOffset As Long, buffer As Byte(), bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetBytes
            Return _inner.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
        End Function

        Public Function GetChar(i As Integer) As Char Implements IDataRecord.GetChar
            Return _inner.GetChar(i)
        End Function

        Public Function GetChars(i As Integer, fieldoffset As Long, buffer As Char(), bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetChars
            Return _inner.GetChars(i, fieldoffset, buffer, bufferoffset, length)
        End Function

        Public Function GetData(i As Integer) As IDataReader Implements IDataRecord.GetData
            Return _inner.GetData(i)
        End Function

        Public Function GetDataTypeName(i As Integer) As String Implements IDataRecord.GetDataTypeName
            Return _inner.GetDataTypeName(i)
        End Function

        Public Function GetDateTime(i As Integer) As Date Implements IDataRecord.GetDateTime
            Return _inner.GetDateTime(i)
        End Function

        Public Function GetDecimal(i As Integer) As Decimal Implements IDataRecord.GetDecimal
            Return _inner.GetDecimal(i)
        End Function

        Public Function GetDouble(i As Integer) As Double Implements IDataRecord.GetDouble
            Return _inner.GetDouble(i)
        End Function

        Public Function GetFieldType(i As Integer) As Type Implements IDataRecord.GetFieldType
            Return _inner.GetFieldType(i)
        End Function

        Public Function GetFloat(i As Integer) As Single Implements IDataRecord.GetFloat
            Return _inner.GetFloat(i)
        End Function

        Public Function GetGuid(i As Integer) As Guid Implements IDataRecord.GetGuid
            Return _inner.GetGuid(i)
        End Function

        Public Function GetInt16(i As Integer) As Short Implements IDataRecord.GetInt16
            Return _inner.GetInt16(i)
        End Function

        Public Function GetInt32(i As Integer) As Integer Implements IDataRecord.GetInt32
            Return _inner.GetInt32(i)
        End Function

        Public Function GetInt64(i As Integer) As Long Implements IDataRecord.GetInt64
            Return _inner.GetInt64(i)
        End Function

        Public Function GetString(i As Integer) As String Implements IDataRecord.GetString
            Return _inner.GetString(i)
        End Function

        Public Function GetValues(values() As Object) As Integer Implements IDataRecord.GetValues
            Return _inner.GetValues(values)
        End Function

        Public Function IsDBNull(i As Integer) As Boolean Implements IDataRecord.IsDBNull
            Return _inner.IsDBNull(i)
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            _inner.Dispose()
        End Sub

    End Class

End Namespace