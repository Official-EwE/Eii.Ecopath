
Imports System.IO

''' <summary>
''' Class to group a list of Harvest Control Rules into an object
''' </summary>
''' <remarks></remarks>
Public Class Strategy
    Inherits List(Of HCR_Group)

    Public Name As String
    Public FileName As String

    Public Sub New()

    End Sub

    Public Sub New(StrategyName As String)
        Me.New()
        Me.Name = StrategyName
    End Sub

    Public Sub New(StrategyName As String, theFilename As String)
        Me.New(StrategyName)
        Me.FileName = theFilename
    End Sub


    Public Shadows Sub Add(Item As HCR_Group)
        If Not Me.Contains(Item) Then
            MyBase.Add(Item)
        End If
    End Sub


    Public Shadows Function Contains(Item As HCR_Group) As Boolean

        For Each Rule As HCR_Group In Me
            If Item.GroupNumber4Biomass = Rule.GroupNumber4Biomass And Item.GroupNumber4F = Rule.GroupNumber4F Then
                Return True
            End If
        Next
        Return False

    End Function


End Class
