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

#Region " Private vars "

    Private m_results As IDataSearchResults = Nothing
    Private m_dgtIsTaxonUseCallback As IsTaxonUsedDelegate = Nothing

    Private Enum eColumnTypes As Integer
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

#End Region ' Private vars

    Public Sub New()
    End Sub

    Public Delegate Function IsTaxonUsedDelegate(ti As ITaxonSearchData) As Boolean

    Public Sub Init(ByVal uic As cUIContext, Optional dgt As IsTaxonUsedDelegate = Nothing)

        Me.UIContext = uic
        Me.m_dgtIsTaxonUseCallback = dgt

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
            Me.RowsCount = 1
            Me.FillData()
        Catch ex As Exception
            ' Aargh
        End Try

    End Sub

    Public Event OnResultSelected(ByVal result As Object)

    Public Property TaxonAtRow(Optional iRow As Integer = -1) As ITaxonSearchData
        Get
            If (iRow <= 0) Then
                iRow = Me.SelectedRow()
            End If
            If iRow < 1 Then
                Return Nothing
            End If
            Return DirectCast(Me(iRow, eColumnTypes.Index).Tag, ITaxonSearchData)
        End Get
        Set(value As ITaxonSearchData)
            If (iRow < 1) Then Return
            Me(iRow, eColumnTypes.Index).Tag = value
        End Set
    End Property

    Public Sub OnUsedTaxaChanged()
        Me.UpdateTaxaUsedStatus()
    End Sub

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

        For iCol As Integer = 1 To Me.ColumnsCount - 1
            Select Case DirectCast(iCol, eColumnTypes)
                Case eColumnTypes.Index
                    Me.Columns(iCol).Width = 20
                Case Else
                    Me.Columns(iCol).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                    Me.AutoSizeColumn(iCol, 80)
            End Select
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
    End Sub

    Protected Overrides Sub OnCellDoubleClicked(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual)
        Try
            If (Me.m_dgtIsTaxonUseCallback IsNot Nothing) Then
                If Me.m_dgtIsTaxonUseCallback.Invoke(Me.TaxonAtRow(p.Row)) Then
                    Return
                End If
            End If

            RaiseEvent OnResultSelected(Me.TaxonAtRow)
            Me.UpdateTaxaUsedStatus()

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
        Me.TaxonAtRow(iRow) = result
        Me.UpdateTaxaUsedStatus(iRow)

    End Sub

    Private Sub AddCell(ByVal result As ITaxonSearchData, ByVal iRow As Integer, ByVal col As eColumnTypes)

        Dim strValue As String = ""
        Dim cell As EwECell = Nothing
        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

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
                strValue = result.Phylum

        End Select

        cell = New EwECell(strValue, GetType(String), style)
        cell.Behaviors.Add(EwEEditHandler)
        cell.EnableEdit = False

        Me(iRow, col) = cell

    End Sub

    Private Sub UpdateTaxaUsedStatus(Optional iRow As Integer = 0)

        Dim ti As ITaxonSearchData = Nothing
        Dim cell As EwECell = Nothing
        Dim iRowMin As Integer = 1
        Dim iRowMax As Integer = Me.RowsCount - 1

        If (Me.m_dgtIsTaxonUseCallback Is Nothing) Then Return

        If (iRow > 0) Then iRowMin = iRow : iRowMax = iRow

        For iRow = iRowMin To iRowMax

            cell = DirectCast(Me(iRow, eColumnTypes.Index), EwECell)
            ti = Me.TaxonAtRow(iRow)

            If Me.m_dgtIsTaxonUseCallback.Invoke(ti) Then
                cell.Style = cell.Style Or cStyleGuide.eStyleFlags.Checked
            Else
                cell.Style = cell.Style And Not cStyleGuide.eStyleFlags.Checked
            End If
            cell.Invalidate()
        Next

    End Sub

#End Region ' Internals

End Class
