Public Class cRowDifference

    Public Enum eRowDifference As Integer
        Missing
        Changed
    End Enum

    Public Sub New(flt As String, col As String, v1 As String, v2 As String, diff As eRowDifference)
        Me.Filter = flt
        Me.Column = col
        Me.Values1 = v1
        Me.Values2 = v2
        Me.Diff = diff
    End Sub

    Public ReadOnly Property Filter As String
    Public ReadOnly Property Column As String
    Public ReadOnly Property Values1 As String
    Public ReadOnly Property Values2 As String
    Public ReadOnly Property Diff As eRowDifference

End Class
