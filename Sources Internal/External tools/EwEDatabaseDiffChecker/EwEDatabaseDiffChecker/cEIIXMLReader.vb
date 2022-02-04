Public Class cEIIXMLReader
    Implements IReader

    Public Function Connect(src As String) As Boolean Implements IReader.Connect
        Throw New NotImplementedException()
    End Function

    Public Function TableNames() As String() Implements IReader.TableNames
        Throw New NotImplementedException()
    End Function

    Public Function Read(table As String) As DataTable Implements IReader.Read
        Throw New NotImplementedException()
    End Function
End Class
