Imports EwEUtils.Core

Public Class cSpatTempConnection

    Public Sub New(DatasetName As String, LayerType As eVarNameFlags, LayerName As String, Optional Scalar As Single = -9999)
        Me.DatasetName = DatasetName
        Me.VarName = LayerType
        Me.LayerName = LayerName
        Me.Scalar = Scalar
    End Sub

    Public ReadOnly Property DatasetName As String
    Public ReadOnly Property VarName As eVarnameflags
    Public ReadOnly Property LayerName As String
    Public ReadOnly Property Scalar As Single = -9999

End Class
