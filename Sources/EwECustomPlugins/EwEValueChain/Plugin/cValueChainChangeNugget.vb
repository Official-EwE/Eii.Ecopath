Imports EwECore

Friend Class cValueChainChangeNugget
    Implements ICoreInterface

    Public Property Name As String Implements ICoreInterface.Name
        Get
            Return ""
        End Get
        Set(value As String)
        End Set
    End Property

    Public Property Index As Integer Implements ICoreInterface.Index
        Get
            Return -1
        End Get
        Set(value As Integer)
        End Set
    End Property

    Public Property DBID As Integer Implements ICoreInterface.DBID
        Get
            Return 42
        End Get
        Set(value As Integer)
        End Set
    End Property

    Public ReadOnly Property DataType As eDataTypes Implements ICoreInterface.DataType
        Get
            Return eDataTypes.External
        End Get
    End Property

    Public ReadOnly Property CoreComponent As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return eCoreComponentType.External
        End Get
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Return "ValueChainChangeNugget"
    End Function

End Class
