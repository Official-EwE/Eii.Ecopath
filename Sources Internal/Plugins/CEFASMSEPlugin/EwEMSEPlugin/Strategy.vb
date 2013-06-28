
Imports System.IO


Public Class Strategy

    Public Name As String
    Public FileName As String
    Public HCRules As List(Of HCR_Group)

    Public Sub New()
        HCRules = New List(Of HCR_Group)
    End Sub

    Public Sub New(StrategyName As String)
        Me.New()
        Me.Name = StrategyName
    End Sub

    Public Sub New(StrategyName As String, theFilename As String)
        Me.New(StrategyName)
        Me.FileName = theFilename
    End Sub

    Public Sub New(StrategyName As String, ListOfHCRules As List(Of HCR_Group))
        Me.New(StrategyName)
        Me.HCRules = ListOfHCRules
    End Sub

    Public Sub New(StrategyName As String, FullPathFileName As String, ListOfHCRules As List(Of HCR_Group))
        Me.New(StrategyName, FullPathFileName)
        Me.HCRules = ListOfHCRules
    End Sub

End Class
