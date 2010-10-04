Public Class cDataSheet
    Private mData As Object
    Private mName As String

    Public Property Data()
        Get
            Return mData
        End Get
        Set(ByVal value)
            mData = value
        End Set
    End Property

    Public Property Name()
        Get
            Return mName
        End Get
        Set(ByVal value)
            mName = value
        End Set
    End Property

End Class
