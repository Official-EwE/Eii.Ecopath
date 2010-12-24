#Region " Imports "

Option Strict On

Imports EwEPlugin.Data
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

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
        Phylum
        Code
    End Enum

    Public Sub New()
    End Sub

    Public Sub Init(ByVal uic As cUIContext)

        Me.UIContext = uic
        Try
            Me.m_results = Nothing
            Me.InitLayout()
        Catch ex As Exception
            ' Aargh
        End Try

    End Sub

    Public Sub AddResults(ByVal results As IDataSearchResults)

        Try
            Me.m_results = results
            Me.RefreshContent()
        Catch ex As Exception
            ' Aargh
        End Try

    End Sub

    Public Event OnResultSelected(ByVal result As Object)

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
        Me(0, eColumnTypes.Code) = New EwEColumnHeaderCell(SharedResources.HEADER_CODE)
        Me(0, eColumnTypes.Common) = New EwEColumnHeaderCell(SharedResources.HEADER_COMMON_NAME)
        Me(0, eColumnTypes.Species) = New EwEColumnHeaderCell(SharedResources.HEADER_SPECIES)
        Me(0, eColumnTypes.Family) = New EwEColumnHeaderCell(SharedResources.HEADER_FAMILY)
        Me(0, eColumnTypes.Order) = New EwEColumnHeaderCell(SharedResources.HEADER_ORDER)
        Me(0, eColumnTypes.Class) = New EwEColumnHeaderCell(SharedResources.HEADER_CLASS)
        Me(0, eColumnTypes.Genus) = New EwEColumnHeaderCell(SharedResources.HEADER_GENUS)
        Me(0, eColumnTypes.Phylum) = New EwEColumnHeaderCell(SharedResources.HEADER_PHYLUM)

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
            Me.AddResult(DirectCast(Me.m_results.SearchResults(iRow), ITaxonSearchData))
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()

        For i As Integer = 0 To Me.ColumnsCount - 1
            Select Case i
                Case eColumnTypes.Index ' Nop
                Case Else
                    Me.Columns(i).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                    Me.AutoSizeColumn(i, 100)
            End Select
        Next

    End Sub

    Protected Overrides Sub OnCellDoubleClicked(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual)
        Try
            RaiseEvent OnResultSelected(Me.SelectedResult)
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="result"></param>
    ''' -----------------------------------------------------------------------
    Private Sub AddResult(ByVal result As ITaxonSearchData)
        Dim iRow As Integer = Me.AddRow()
        For iCol As Integer = 0 To Me.ColumnsCount - 1
            Me.AddCell(result, iRow, DirectCast(iCol, eColumnTypes))
        Next
    End Sub

    Protected Sub AddCell(ByVal result As ITaxonSearchData, ByVal iRow As Integer, ByVal col As eColumnTypes)

        Dim strValue As String = ""
        Dim cell As EwECell = Nothing
        Dim style As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.TaxonReg)

        Select Case col
            Case eColumnTypes.Index
                strValue = CStr(iRow)

            Case eColumnTypes.Common
                strValue = result.Common

            Case eColumnTypes.Species
                strValue = result.Species
                style = style Or cStyleGuide.eStyleFlags.TaxonItalics

            Case eColumnTypes.Genus
                strValue = result.Genus
                style = style Or cStyleGuide.eStyleFlags.TaxonItalics

            Case eColumnTypes.Family
                strValue = result.Family

            Case eColumnTypes.Order
                strValue = result.Order

            Case eColumnTypes.Class
                strValue = result.Class

            Case eColumnTypes.Code
                strValue = result.SourceKey

            Case eColumnTypes.Phylum
                strValue = ""
                style = style Or cStyleGuide.eStyleFlags.Null

        End Select

        cell = New EwECell(strValue, GetType(String), style)
        cell.Behaviors.Add(EwEEditHandler)

        If (col = eColumnTypes.Index) Then cell.Tag = result
        Me(iRow, col) = cell

    End Sub

#End Region ' Internals

End Class
