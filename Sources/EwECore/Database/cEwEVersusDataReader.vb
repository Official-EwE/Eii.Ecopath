' SPDX-License-Identifier: EUPL-1.2
' Decorator for IDataReader to support both a primary data source, or a secondary with comparison
Imports System.Data

Namespace Database
    Public Class cEwEVersusDataReader
        Implements IDataReader

        Private ReadOnly _primaryReader As IDataReader
        Private ReadOnly _secondaryReader As IDataReader
        Private ReadOnly _mode As Mode

        Public Enum Mode
            PrimaryOnly
            SecondaryOnly
            Compare
        End Enum

        Public Sub New(Optional primaryReader As IDataReader = Nothing, Optional secondaryReader As IDataReader = Nothing, Optional mode As Mode = Mode.PrimaryOnly)
            _primaryReader = primaryReader
            _secondaryReader = secondaryReader
            _mode = mode
        End Sub

        ' Example: Read() implementation
        Public Function Read() As Boolean Implements IDataReader.Read
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader IsNot Nothing AndAlso _primaryReader.Read()
                Case Mode.SecondaryOnly
                    Return _secondaryReader IsNot Nothing AndAlso _secondaryReader.Read()
                Case Mode.Compare
                    Dim primaryHasRow = _primaryReader IsNot Nothing AndAlso _primaryReader.Read()
                    Dim secondaryHasRow = _secondaryReader IsNot Nothing AndAlso _secondaryReader.Read()
                    ' Optionally compare row data here and log/report differences
                    Return primaryHasRow Or secondaryHasRow
                Case Else
                    Return False
            End Select
        End Function

        ' Example: GetValue() implementation
        Public Function GetValue(i As Integer) As Object Implements IDataReader.GetValue
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetValue(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetValue(i)
                Case Mode.Compare
                    Dim primaryVal = _primaryReader.GetValue(i)
                    Dim secondaryVal = _secondaryReader.GetValue(i)
                    ' Optionally compare values and log/report differences
                    Return primaryVal ' or secondaryVal, or both, or a tuple
                Case Else
                    Return Nothing
            End Select
        End Function

        ' Implement other IDataReader members by delegating to the appropriate reader(s)
        ' ...existing code...

        Public Sub Close() Implements IDataReader.Close
            _primaryReader?.Close()
            _secondaryReader?.Close()
        End Sub

        Public ReadOnly Property Depth As Integer Implements IDataReader.Depth
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly
                        Return _primaryReader.Depth
                    Case Mode.SecondaryOnly
                        Return _secondaryReader.Depth
                    Case Mode.Compare
                        Return Math.Max(_primaryReader.Depth, _secondaryReader.Depth)
                    Case Else
                        Return 0
                End Select
            End Get
        End Property

        Public ReadOnly Property FieldCount As Integer Implements IDataReader.FieldCount
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly
                        Return _primaryReader.FieldCount
                    Case Mode.SecondaryOnly
                        Return _secondaryReader.FieldCount
                    Case Mode.Compare
                        Return Math.Max(_primaryReader.FieldCount, _secondaryReader.FieldCount)
                    Case Else
                        Return 0
                End Select
            End Get
        End Property

        Public Function GetName(i As Integer) As String Implements IDataReader.GetName
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetName(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetName(i)
                Case Mode.Compare
                    ' Optionally compare names
                    Return _primaryReader.GetName(i)
                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function GetSchemaTable() As DataTable Implements IDataReader.GetSchemaTable
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetSchemaTable()
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetSchemaTable()
                Case Mode.Compare
                    ' Optionally compare schema tables
                    Return _primaryReader.GetSchemaTable()
                Case Else
                    Return Nothing
            End Select
        End Function

        Public ReadOnly Property IsClosed As Boolean Implements IDataReader.IsClosed
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly
                        Return _primaryReader.IsClosed
                    Case Mode.SecondaryOnly
                        Return _secondaryReader.IsClosed
                    Case Mode.Compare
                        Return _primaryReader.IsClosed AndAlso _secondaryReader.IsClosed
                    Case Else
                        Return True
                End Select
            End Get
        End Property

        Public Function NextResult() As Boolean Implements IDataReader.NextResult
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.NextResult()
                Case Mode.SecondaryOnly
                    Return _secondaryReader.NextResult()
                Case Mode.Compare
                    Dim primaryNext = _primaryReader.NextResult()
                    Dim secondaryNext = _secondaryReader.NextResult()
                    Return primaryNext Or secondaryNext
                Case Else
                    Return False
            End Select
        End Function

        Public ReadOnly Property RecordsAffected As Integer Implements IDataReader.RecordsAffected
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly
                        Return _primaryReader.RecordsAffected
                    Case Mode.SecondaryOnly
                        Return _secondaryReader.RecordsAffected
                    Case Mode.Compare
                        Return Math.Max(_primaryReader.RecordsAffected, _secondaryReader.RecordsAffected)
                    Case Else
                        Return 0
                End Select
            End Get
        End Property

        Public Function GetBoolean(i As Integer) As Boolean Implements IDataRecord.GetBoolean
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetBoolean(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetBoolean(i)
                Case Mode.Compare
                    ' Optionally compare values
                    Return _primaryReader.GetBoolean(i)
                Case Else
                    Return False
            End Select
        End Function

        Public Function GetByte(i As Integer) As Byte Implements IDataRecord.GetByte
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetByte(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetByte(i)
                Case Mode.Compare
                    Return _primaryReader.GetByte(i)
                Case Else
                    Return 0
            End Select
        End Function

        Public Function GetBytes(i As Integer, fieldOffset As Long, buffer As Byte(), bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetBytes
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
                Case Mode.Compare
                    Return _primaryReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
                Case Else
                    Return 0
            End Select
        End Function

        Public Function GetChar(i As Integer) As Char Implements IDataRecord.GetChar
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetChar(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetChar(i)
                Case Mode.Compare
                    Return _primaryReader.GetChar(i)
                Case Else
                    Return ChrW(0)
            End Select
        End Function

        Public Function GetChars(i As Integer, fieldoffset As Long, buffer As Char(), bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetChars
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
                Case Mode.Compare
                    Return _primaryReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
                Case Else
                    Return 0
            End Select
        End Function

        Public Function GetData(i As Integer) As IDataReader Implements IDataRecord.GetData
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetData(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetData(i)
                Case Mode.Compare
                    Return _primaryReader.GetData(i)
                Case Else
                    Return Nothing
            End Select
        End Function

        Public Function GetDataTypeName(i As Integer) As String Implements IDataRecord.GetDataTypeName
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetDataTypeName(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetDataTypeName(i)
                Case Mode.Compare
                    Return _primaryReader.GetDataTypeName(i)
                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function GetDateTime(i As Integer) As Date Implements IDataRecord.GetDateTime
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetDateTime(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetDateTime(i)
                Case Mode.Compare
                    Return _primaryReader.GetDateTime(i)
                Case Else
                    Return Date.MinValue
            End Select
        End Function

        Public Function GetDecimal(i As Integer) As Decimal Implements IDataRecord.GetDecimal
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetDecimal(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetDecimal(i)
                Case Mode.Compare
                    Return _primaryReader.GetDecimal(i)
                Case Else
                    Return 0D
            End Select
        End Function

        Public Function GetDouble(i As Integer) As Double Implements IDataRecord.GetDouble
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetDouble(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetDouble(i)
                Case Mode.Compare
                    Return _primaryReader.GetDouble(i)
                Case Else
                    Return 0.0
            End Select
        End Function

        Public Function GetFieldType(i As Integer) As Type Implements IDataRecord.GetFieldType
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetFieldType(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetFieldType(i)
                Case Mode.Compare
                    Return _primaryReader.GetFieldType(i)
                Case Else
                    Return GetType(Object)
            End Select
        End Function

        Public Function GetFloat(i As Integer) As Single Implements IDataRecord.GetFloat
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetFloat(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetFloat(i)
                Case Mode.Compare
                    Return _primaryReader.GetFloat(i)
                Case Else
                    Return 0.0F
            End Select
        End Function

        Public Function GetGuid(i As Integer) As Guid Implements IDataRecord.GetGuid
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetGuid(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetGuid(i)
                Case Mode.Compare
                    Return _primaryReader.GetGuid(i)
                Case Else
                    Return Guid.Empty
            End Select
        End Function

        Public Function GetInt16(i As Integer) As Short Implements IDataRecord.GetInt16
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetInt16(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetInt16(i)
                Case Mode.Compare
                    Return _primaryReader.GetInt16(i)
                Case Else
                    Return 0S
            End Select
        End Function

        Public Function GetInt32(i As Integer) As Integer Implements IDataRecord.GetInt32
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetInt32(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetInt32(i)
                Case Mode.Compare
                    Return _primaryReader.GetInt32(i)
                Case Else
                    Return 0
            End Select
        End Function

        Public Function GetInt64(i As Integer) As Long Implements IDataRecord.GetInt64
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetInt64(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetInt64(i)
                Case Mode.Compare
                    Return _primaryReader.GetInt64(i)
                Case Else
                    Return 0L
            End Select
        End Function

        Public Function GetOrdinal(name As String) As Integer Implements IDataRecord.GetOrdinal
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetOrdinal(name)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetOrdinal(name)
                Case Mode.Compare
                    Return _primaryReader.GetOrdinal(name)
                Case Else
                    Return -1
            End Select
        End Function

        Public Function GetString(i As Integer) As String Implements IDataRecord.GetString
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetString(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetString(i)
                Case Mode.Compare
                    Return _primaryReader.GetString(i)
                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function GetValues(values() As Object) As Integer Implements IDataRecord.GetValues
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.GetValues(values)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.GetValues(values)
                Case Mode.Compare
                    Return _primaryReader.GetValues(values)
                Case Else
                    Return 0
            End Select
        End Function

        Public Function IsDBNull(i As Integer) As Boolean Implements IDataRecord.IsDBNull
            Select Case _mode
                Case Mode.PrimaryOnly
                    Return _primaryReader.IsDBNull(i)
                Case Mode.SecondaryOnly
                    Return _secondaryReader.IsDBNull(i)
                Case Mode.Compare
                    Return _primaryReader.IsDBNull(i)
                Case Else
                    Return True
            End Select
        End Function

        Default Public ReadOnly Property Item(i As Integer) As Object Implements IDataRecord.Item
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly
                        Return _primaryReader(i)
                    Case Mode.SecondaryOnly
                        Return _secondaryReader(i)
                    Case Mode.Compare
                        Return _primaryReader(i)
                    Case Else
                        Return Nothing
                End Select
            End Get
        End Property

        Default Public ReadOnly Property Item(name As String) As Object Implements IDataRecord.Item
            Get
                Select Case _mode
                    Case Mode.PrimaryOnly
                        Return _primaryReader(name)
                    Case Mode.SecondaryOnly
                        Return _secondaryReader(name)
                    Case Mode.Compare
                        Return _primaryReader(name)
                    Case Else
                        Return Nothing
                End Select
            End Get
        End Property

        ' Dispose pattern
        Public Sub Dispose() Implements IDisposable.Dispose
            _primaryReader?.Dispose()
            _secondaryReader?.Dispose()
        End Sub

        ' ...existing code...
    End Class
End Namespace

