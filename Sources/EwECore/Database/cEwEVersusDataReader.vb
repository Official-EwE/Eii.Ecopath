' SPDX-License-Identifier: EUPL-1.2
' Decorator for IDataReader to support both a primary data source, or a secondary with comparison
Imports System.Data
Imports EwEUtils.NetUtilities

Namespace Database
    Public Class cEwEVersusDataReader
        Implements IDataReader

        Private ReadOnly _primaryReader As cCoercedDataReader
        Private ReadOnly _secondaryReader As cCoercedDataReader
        Private ReadOnly _mode As Mode

        Private _rowCount As Integer = 0
        Private _rowDiffs As New List(Of DataReaderDiff.RowDiff)()

        Public Enum Mode
            PrimaryOnly
            SecondaryOnly
            Compare
            CompareAndSwap
        End Enum

        Public Sub New(Optional primaryReader As IDataReader = Nothing, Optional secondaryReader As IDataReader = Nothing, Optional mode As Mode = Mode.Compare)
            _primaryReader = New cCoercedDataReader(If(mode = Mode.CompareAndSwap, secondaryReader, primaryReader))
            _secondaryReader = New cCoercedDataReader(If(mode = Mode.CompareAndSwap, primaryReader, secondaryReader))
            _mode = mode
        End Sub

        Public Sub SetFuncGetPropTypes(getPropTypes As Func(Of String, Dictionary(Of String, Type)))
            Dim tableName = DataReaderDiff.GetTableName(If(_mode = Mode.CompareAndSwap, _primaryReader, _secondaryReader))
            Dim propTypes = getPropTypes(tableName)
            _primaryReader.PropTypes = propTypes
            _secondaryReader.PropTypes = propTypes
        End Sub

        Public Function Read() As Boolean Implements IDataReader.Read
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader IsNot Nothing AndAlso _primaryReader.Read()
                Case Mode.SecondaryOnly
                    Return _secondaryReader IsNot Nothing AndAlso _secondaryReader.Read()
                Case Mode.Compare, Mode.CompareAndSwap
                    Dim primaryHasRow = _primaryReader IsNot Nothing AndAlso _primaryReader.Read()
                    _secondaryReader.Read()
                    If primaryHasRow Then
                        _rowCount += 1
                        _rowDiffs.AddRange(DataReaderDiff.CompareCurrentRow(_primaryReader, _secondaryReader))
                    Else
                        If _mode = Mode.CompareAndSwap Then
                            DataReaderDiff.BroadcastDiffs(_secondaryReader, _primaryReader, _rowDiffs, _rowCount)
                        Else
                            DataReaderDiff.BroadcastDiffs(_primaryReader, _secondaryReader, _rowDiffs, _rowCount)
                        End If
                        _rowDiffs.Clear()
                        _rowCount = 0
                    End If
                    Return primaryHasRow
                Case Else
                    Return False
            End Select
        End Function

        Public Function GetValue(i As Integer) As Object Implements IDataReader.GetValue
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetValue(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetValue(i)
                Case Else : Return _primaryReader.GetValue(i)
            End Select
        End Function

        Default Public ReadOnly Property Item(i As Integer) As Object Implements IDataRecord.Item
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly : Return _primaryReader(i)
                    Case Mode.SecondaryOnly : Return _secondaryReader(i)
                    Case Else : Return _primaryReader(i)
                End Select
            End Get
        End Property

        Default Public ReadOnly Property Item(name As String) As Object Implements IDataRecord.Item
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly : Return _primaryReader(name)
                    Case Mode.SecondaryOnly : Return _secondaryReader(name)
                    Case Else : Return _primaryReader(name)
                End Select
            End Get
        End Property

        Public Sub Close() Implements IDataReader.Close
            _primaryReader?.Close()
            _secondaryReader?.Close()
        End Sub

        Public ReadOnly Property Depth As Integer Implements IDataReader.Depth
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly : Return _primaryReader.Depth
                    Case Mode.SecondaryOnly : Return _secondaryReader.Depth
                    Case Else : Return _primaryReader.Depth
                End Select
            End Get
        End Property

        Public ReadOnly Property FieldCount As Integer Implements IDataReader.FieldCount
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly : Return _primaryReader.FieldCount
                    Case Mode.SecondaryOnly : Return _secondaryReader.FieldCount
                    Case Else : Return _primaryReader.FieldCount
                End Select
            End Get
        End Property

        Public Function GetName(i As Integer) As String Implements IDataReader.GetName
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetName(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetName(i)
                Case Else : Return _primaryReader.GetName(i)
            End Select
        End Function

        Public Function GetSchemaTable() As DataTable Implements IDataReader.GetSchemaTable
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetSchemaTable()
                Case Mode.SecondaryOnly : Return _secondaryReader.GetSchemaTable()
                Case Else : Return _primaryReader.GetSchemaTable()
            End Select
        End Function

        Public ReadOnly Property IsClosed As Boolean Implements IDataReader.IsClosed
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly : Return _primaryReader.IsClosed
                    Case Mode.SecondaryOnly : Return _secondaryReader.IsClosed
                    Case Else : Return _primaryReader.IsClosed AndAlso _secondaryReader.IsClosed
                End Select
            End Get
        End Property

        Public Function NextResult() As Boolean Implements IDataReader.NextResult
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.NextResult()
                Case Mode.SecondaryOnly : Return _secondaryReader.NextResult()
                Case Else
                    Dim primaryNext = _primaryReader.NextResult()
                    _secondaryReader.NextResult()
                    Return primaryNext
            End Select
        End Function

        Public ReadOnly Property RecordsAffected As Integer Implements IDataReader.RecordsAffected
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly : Return _primaryReader.RecordsAffected
                    Case Mode.SecondaryOnly : Return _secondaryReader.RecordsAffected
                    Case Else : Return _primaryReader.RecordsAffected
                End Select
            End Get
        End Property

        Public Function GetBoolean(i As Integer) As Boolean Implements IDataRecord.GetBoolean
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetBoolean(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetBoolean(i)
                Case Else : Return _primaryReader.GetBoolean(i)
            End Select
        End Function

        Public Function GetByte(i As Integer) As Byte Implements IDataRecord.GetByte
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetByte(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetByte(i)
                Case Else : Return _primaryReader.GetByte(i)
            End Select
        End Function

        Public Function GetBytes(i As Integer, fieldOffset As Long, buffer As Byte(), bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetBytes
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
                Case Else : Return _primaryReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
            End Select
        End Function

        Public Function GetChar(i As Integer) As Char Implements IDataRecord.GetChar
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetChar(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetChar(i)
                Case Else : Return _primaryReader.GetChar(i)
            End Select
        End Function

        Public Function GetChars(i As Integer, fieldoffset As Long, buffer As Char(), bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetChars
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
                Case Else : Return _primaryReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
            End Select
        End Function

        Public Function GetData(i As Integer) As IDataReader Implements IDataRecord.GetData
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetData(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetData(i)
                Case Else : Return _primaryReader.GetData(i)
            End Select
        End Function

        Public Function GetDataTypeName(i As Integer) As String Implements IDataRecord.GetDataTypeName
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetDataTypeName(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetDataTypeName(i)
                Case Else : Return _primaryReader.GetDataTypeName(i)
            End Select
        End Function

        Public Function GetDateTime(i As Integer) As Date Implements IDataRecord.GetDateTime
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetDateTime(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetDateTime(i)
                Case Else : Return _primaryReader.GetDateTime(i)
            End Select
        End Function

        Public Function GetDecimal(i As Integer) As Decimal Implements IDataRecord.GetDecimal
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetDecimal(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetDecimal(i)
                Case Else : Return _primaryReader.GetDecimal(i)
            End Select
        End Function

        Public Function GetDouble(i As Integer) As Double Implements IDataRecord.GetDouble
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetDouble(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetDouble(i)
                Case Else : Return _primaryReader.GetDouble(i)
            End Select
        End Function

        Public Function GetFieldType(i As Integer) As Type Implements IDataRecord.GetFieldType
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetFieldType(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetFieldType(i)
                Case Else : Return _primaryReader.GetFieldType(i)
            End Select
        End Function

        Public Function GetFloat(i As Integer) As Single Implements IDataRecord.GetFloat
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetFloat(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetFloat(i)
                Case Else : Return _primaryReader.GetFloat(i)
            End Select
        End Function

        Public Function GetGuid(i As Integer) As Guid Implements IDataRecord.GetGuid
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetGuid(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetGuid(i)
                Case Else : Return _primaryReader.GetGuid(i)
            End Select
        End Function

        Public Function GetInt16(i As Integer) As Short Implements IDataRecord.GetInt16
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetInt16(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetInt16(i)
                Case Else : Return _primaryReader.GetInt16(i)
            End Select
        End Function

        Public Function GetInt32(i As Integer) As Integer Implements IDataRecord.GetInt32
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetInt32(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetInt32(i)
                Case Else : Return _primaryReader.GetInt32(i)
            End Select
        End Function

        Public Function GetInt64(i As Integer) As Long Implements IDataRecord.GetInt64
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetInt64(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetInt64(i)
                Case Else : Return _primaryReader.GetInt64(i)
            End Select
        End Function

        Public Function GetOrdinal(name As String) As Integer Implements IDataRecord.GetOrdinal
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetOrdinal(name)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetOrdinal(name)
                Case Else : Return _primaryReader.GetOrdinal(name)
            End Select
        End Function

        Public Function GetString(i As Integer) As String Implements IDataRecord.GetString
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetString(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetString(i)
                Case Else : Return _primaryReader.GetString(i)
            End Select
        End Function

        Public Function GetValues(values() As Object) As Integer Implements IDataRecord.GetValues
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.GetValues(values)
                Case Mode.SecondaryOnly : Return _secondaryReader.GetValues(values)
                Case Else : Return _primaryReader.GetValues(values)
            End Select
        End Function

        Public Function IsDBNull(i As Integer) As Boolean Implements IDataRecord.IsDBNull
            Select Case _mode
                Case Mode.PrimaryOnly : Return _primaryReader.IsDBNull(i)
                Case Mode.SecondaryOnly : Return _secondaryReader.IsDBNull(i)
                Case Else : Return _primaryReader.IsDBNull(i)
            End Select
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            _primaryReader?.Dispose()
            _secondaryReader?.Dispose()
        End Sub

    End Class
End Namespace
