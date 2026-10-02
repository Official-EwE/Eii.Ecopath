Imports ValueChain

Public Class cEwEValueChainResults
    Inherits cValueChainResults

    Public Sub New(data As cEwEValueChainData)
        MyBase.New(data)
    End Sub

    Public Property RunType As cEwEValueChainModel.eRunTypes

End Class
