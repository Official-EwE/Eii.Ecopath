
Imports System.IO


Public Class Strategy

    Public Name As String
    Public FileName As String
    Public HCRs As List(Of cMSE.HCR_Group)

    Public Sub New()
        HCRs = New List(Of cMSE.HCR_Group)
    End Sub

    Public Sub New(StrategyName As String)
        Me.New()
        Me.Name = StrategyName
    End Sub

    Public Sub New(StrategyName As String, theFilename As String)
        Me.New(StrategyName)
        Me.FileName = theFilename
    End Sub

    Public Sub New(StrategyName As String, ListOfHCRs As List(Of cMSE.HCR_Group))
        Me.New(StrategyName)
        Me.HCRs = ListOfHCRs
    End Sub

    Public Sub New(StrategyName As String, FullPathFileName As String, ListOfHCRs As List(Of cMSE.HCR_Group))
        Me.New(StrategyName, FullPathFileName)
        Me.HCRs = ListOfHCRs
    End Sub

End Class
