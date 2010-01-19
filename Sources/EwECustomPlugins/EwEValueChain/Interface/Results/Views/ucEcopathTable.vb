#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports System.Collections.Generic
Imports SourceGrid2

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class ucEcopathTable
    Inherits ScrollableControl
    Implements IResultView

    Public Enum eColumnTypes As Integer
        Caption = 0
        Unit
        Producer
        Processor
        Distributor
        Market
        Total
    End Enum

    Private m_grid As New SourceGrid2.Grid()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        Me.Controls.Add(Me.m_grid)
        Me.PrepareGrid()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub ShowResults(ByVal iFleet As Integer, _
                           ByVal lUnits As List(Of cUnit), ByVal results As cResults) _
           Implements IResultView.ShowResults

        ' Split units in the different types
        Dim alUnits(4) As List(Of cUnit)
        Dim cell As SourceGrid2.Cells.Real.Cell = Nothing

        For i As Integer = 0 To 3
            alUnits(i) = New List(Of cUnit)
        Next

        ' Create subset lists
        For Each unit As cUnit In lUnits
            Select Case unit.UnitType
                Case cUnitFactory.eUnitType.Producer
                    alUnits(0).Add(unit)
                Case cUnitFactory.eUnitType.Processing
                    alUnits(1).Add(unit)
                Case cUnitFactory.eUnitType.Distribution
                    alUnits(2).Add(unit)
                Case cUnitFactory.eUnitType.Market
                    alUnits(3).Add(unit)
            End Select
        Next

        ' Populate data cells
        For i As Integer = 0 To 3

            Me.UpdateDataCell(Me.m_grid(1, 2 + i), results, cResults.eVariableType.Production, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(2, 2 + i), results, cResults.eVariableType.ProductionLive, alUnits(i), iFleet)

            Me.UpdateDataCell(Me.m_grid(3, 2 + i), results, cResults.eVariableType.RevenueProductsMain, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(4, 2 + i), results, cResults.eVariableType.RevenueProductsOther, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(5, 2 + i), results, cResults.eVariableType.RevenueTickets, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(6, 2 + i), results, cResults.eVariableType.RevenueSubsidies, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(7, 2 + i), results, cResults.eVariableType.RevenueTotal, alUnits(i), iFleet)

            Me.UpdateDataCell(Me.m_grid(8, 2 + i), results, cResults.eVariableType.CostSalariesShares, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(9, 2 + i), results, cResults.eVariableType.CostRawmaterial, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(10, 2 + i), results, cResults.eVariableType.CostInput, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(11, 2 + i), results, cResults.eVariableType.CostTaxes, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(12, 2 + i), results, cResults.eVariableType.CostManagementRoyaltyCertificationObservers, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(13, 2 + i), results, cResults.eVariableType.Cost, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(14, 2 + i), results, cResults.eVariableType.Profit, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(15, 2 + i), results, cResults.eVariableType.TotalUtility, alUnits(i), iFleet)

            Me.UpdateDataCell(Me.m_grid(16, 2 + i), results, cResults.eVariableType.NumberOfJobsFemaleTotal, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(17, 2 + i), results, cResults.eVariableType.NumberOfJobsMaleTotal, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(18, 2 + i), results, cResults.eVariableType.NumberOfJobsTotal, alUnits(i), iFleet)

            Me.UpdateDataCell(Me.m_grid(19, 2 + i), results, cResults.eVariableType.NumberOfWorkerDependents, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(20, 2 + i), results, cResults.eVariableType.NumberOfOwnerDependents, alUnits(i), iFleet)
            Me.UpdateDataCell(Me.m_grid(21, 2 + i), results, cResults.eVariableType.NumberOfDependentsTotal, alUnits(i), iFleet)

        Next i

        ' Create total cells
        For iRow As Integer = 3 To Me.m_grid.RowsCount - 1
            Dim sTotal As Single = 0.0!
            For iCol As Integer = eColumnTypes.Producer To eColumnTypes.Market
                Try
                    sTotal += CSng(Val(Me.m_grid(iRow, iCol).Value))
                Catch ex As Exception
                    Console.WriteLine("Failed to access cell for row {0}, col {1}", iRow, iCol)
                End Try
            Next
            Me.UpdateDataCell(Me.m_grid(iRow, eColumnTypes.Total), sTotal)
        Next

        Me.m_grid.InvalidateCells()

    End Sub

    Private Function CreateRowHeaderCell(ByVal strLabel As String, _
                             Optional ByVal style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Names) As EwECell
        Dim cell As New EwERowHeaderCell(strLabel)
        cell.Style = style
        Return cell
    End Function

    Private Function CreateUnitCell(ByVal strUnitValue As String, _
                             Optional ByVal style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Names) As EwECell
        Dim cell As New EwECell(strUnitValue, GetType(String))
        cell.Style = style
        cell.EditableMode = EditableMode.None
        cell.EnableEdit = False
        Return cell
    End Function

    Private Function CreateUnitCell(ByVal unitType As cStyleGuide.eUnitType, _
                                    Optional ByVal style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Names, _
                                    Optional ByVal strUnitMask As String = "{0}") As EwECell
        Dim cell As New EwEUnitCell(strUnitMask, New cStyleGuide.eUnitType() {unitType})
        cell.Style = style
        Return cell
    End Function

    Private Function CreateDataCell(Optional ByVal style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK) As EwECell
        Dim cell As New EwECell(0.0!, GetType(Single))
        cell.Style = style
        cell.EditableMode = EditableMode.None
        cell.EnableEdit = False
        ' No decimals in results
        cell.NumDigits = 0
        Return cell
    End Function

    Private Sub UpdateDataCell(ByVal cell As Cells.ICell, _
                                    ByVal results As cResults, _
                                    ByVal vn As cResults.eVariableType, _
                                    ByVal lUnits As List(Of cUnit), _
                                    ByVal iFleet As Integer)

        Me.UpdateDataCell(cell, results.GetTimeStepTotal(vn, 1, lUnits, iFleet))

    End Sub

    Private Sub UpdateDataCell(ByVal cell As Cells.ICell, ByVal sValue As Single)
        Try
            cell.Value = sValue
        Catch ex As Exception
            ' Hmm
        End Try
    End Sub

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub PrepareGrid()

        Dim cell As SourceGrid2.Cells.Real.Cell = Nothing
        Dim iR As Integer = 0

        Me.m_grid.AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.Selection.SelectionMode = GridSelectionMode.Cell
        Me.m_grid.Selection.AutoCopyPaste = True
        Me.m_grid.Selection.AutoClear = False
        Me.m_grid.Selection.ProtectReadOnly = True
        Me.m_grid.Dock = DockStyle.Fill

        Me.m_grid.Redim(22, 7)

        ' Column headers
        Me.m_grid(0, 0) = New EwEColumnHeaderCell("Categories")
        Me.m_grid(0, 1) = New EwEColumnHeaderCell("Unit")
        Me.m_grid(0, 2) = New EwEColumnHeaderCell("Producer")
        Me.m_grid(0, 3) = New EwEColumnHeaderCell("Processor")
        Me.m_grid(0, 4) = New EwEColumnHeaderCell("Distributor")
        Me.m_grid(0, 5) = New EwEColumnHeaderCell("Market")
        Me.m_grid(0, 6) = New EwEColumnHeaderCell("Total")

        ' Row headers
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Production")
        ' Units
        Me.m_grid(iR, 1) = CreateUnitCell("t")
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Production, live weight")
        Me.m_grid(iR, 1) = CreateUnitCell("t")

        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Production value")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Other production value")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Ticket revenue")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Subsidies")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("= Revenue", cStyleGuide.eStyleFlags.Sum)
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)

        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Salaries/shares")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Input (fish)")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Input other")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Taxes")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Management, royalty, certification, observers")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("= Cost", cStyleGuide.eStyleFlags.Sum)
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("= Profit", cStyleGuide.eStyleFlags.Sum)
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("= Total utility", cStyleGuide.eStyleFlags.Sum)   'throughput
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)

        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Jobs, female")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Currency)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Jobs, male")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("= Jobs, total", cStyleGuide.eStyleFlags.Sum)
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal, cStyleGuide.eStyleFlags.Sum)

        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Worker dependents")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("Owner dependents")
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal)
        iR += 1
        Me.m_grid(iR, 0) = CreateRowHeaderCell("= Dependents, total", cStyleGuide.eStyleFlags.Sum)
        Me.m_grid(iR, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal, cStyleGuide.eStyleFlags.Sum)

        ' Create data cells
        For i As Integer = 0 To 3

            Me.m_grid(1, 2 + i) = Me.CreateDataCell()
            Me.m_grid(2, 2 + i) = Me.CreateDataCell()

            Me.m_grid(3, 2 + i) = Me.CreateDataCell()
            Me.m_grid(4, 2 + i) = Me.CreateDataCell()
            Me.m_grid(5, 2 + i) = Me.CreateDataCell()
            Me.m_grid(6, 2 + i) = Me.CreateDataCell()
            Me.m_grid(7, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me.m_grid(8, 2 + i) = Me.CreateDataCell()
            Me.m_grid(9, 2 + i) = Me.CreateDataCell()
            Me.m_grid(10, 2 + i) = Me.CreateDataCell()
            Me.m_grid(11, 2 + i) = Me.CreateDataCell()
            Me.m_grid(12, 2 + i) = Me.CreateDataCell()
            Me.m_grid(13, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
            Me.m_grid(14, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
            Me.m_grid(15, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me.m_grid(16, 2 + i) = Me.CreateDataCell()
            Me.m_grid(17, 2 + i) = Me.CreateDataCell()
            Me.m_grid(18, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me.m_grid(19, 2 + i) = Me.CreateDataCell()
            Me.m_grid(20, 2 + i) = Me.CreateDataCell()
            Me.m_grid(21, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

        Next i

        ' Create total cells
        For iRow As Integer = 3 To Me.m_grid.RowsCount - 1
            Me.m_grid(iRow, eColumnTypes.Total) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
        Next

        Me.m_grid.FixedRows = 2
        Me.m_grid.FixedColumns = 2
        Me.m_grid.AutoSize = True
        Me.m_grid.AutoSizeAll()
        Me.m_grid.AutoSizeColumn(0, 140)

    End Sub

#End Region ' Internals

End Class
