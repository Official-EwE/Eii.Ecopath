#Region " Imports "

Option Strict On

Imports EwEPlugin.Data
Imports System.Reflection
Imports ScientificInterfaceShared
Imports SourceGrid2
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

<CLSCompliant(False)> _
Public Class gridTaxonSearchResults
    Inherits EwEGrid

    Private m_results As IDataSearchResults = Nothing

    Public Enum eColumnTypes As Integer
        Index = 0
        Common
        Species
        Genus
        Family
        Order
        [Class]
    End Enum

    Public Sub New()
    End Sub

    Public Sub Init(ByVal uic As cUIContext, ByVal results As IDataSearchResults)

        Me.UIContext = uic

        Try
            Me.m_results = results
            Me.InitLayout()
        Catch ex As Exception
            ' Aargh
        End Try

    End Sub

    Public Function SelectedResult() As Object

        Dim iRow As Integer = Me.SelectedRow()
        If iRow < 1 Then Return Nothing
        Return Me(iRow, 0).Tag

    End Function

#Region " Internals "

    Protected Overrides Sub InitLayout()

        MyBase.InitLayout()

        Me.Selection.SelectionMode = GridSelectionMode.Row
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Common) = New EwEColumnHeaderCell("Common name")
        Me(0, eColumnTypes.Species) = New EwEColumnHeaderCell("Species")
        Me(0, eColumnTypes.Family) = New EwEColumnHeaderCell("Family")
        Me(0, eColumnTypes.Order) = New EwEColumnHeaderCell("Order")
        Me(0, eColumnTypes.Class) = New EwEColumnHeaderCell("Class")
        Me(0, eColumnTypes.Genus) = New EwEColumnHeaderCell("Genus")

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        If Me.UIContext Is Nothing Then Return
        If Me.m_results Is Nothing Then Return

        For iRow As Integer = 0 To Me.m_results.SearchResults.Count - 1
            Me.AddResult(DirectCast(Me.m_results.SearchResults(iRow), ITaxonData))
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="result"></param>
    ''' -----------------------------------------------------------------------
    Private Sub AddResult(ByVal result As ITaxonData)
        Dim iRow As Integer = Me.AddRow()
        For iCol As Integer = 0 To Me.ColumnsCount - 1
            Me.AddCell(result, iRow, DirectCast(iCol, eColumnTypes))
        Next
    End Sub

    Protected Sub AddCell(ByVal result As ITaxonData, ByVal iRow As Integer, ByVal col As eColumnTypes)

        Dim strValue As String = ""
        Dim cell As EwECell = Nothing

        Select Case col
            Case eColumnTypes.Index : strValue = CStr(iRow)
            Case eColumnTypes.Common : strValue = result.Common
            Case eColumnTypes.Species : strValue = result.Species
            Case eColumnTypes.Family : strValue = result.Family
            Case eColumnTypes.Genus : strValue = result.Genus
            Case eColumnTypes.Order : strValue = result.Order
            Case eColumnTypes.Class : strValue = result.Class
        End Select

        cell = New EwECell(strValue, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
        If (col = eColumnTypes.Index) Then cell.Tag = result
        Me(iRow, col) = cell

    End Sub

#End Region ' Internals

End Class
