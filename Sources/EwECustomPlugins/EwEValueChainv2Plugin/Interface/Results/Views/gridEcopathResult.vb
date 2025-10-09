' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore.Style
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports ValueChain
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

<CLSCompliant(False)>
Public Class gridEcopathResult
    Inherits cEwEGrid
    Implements IResultView

    Private Enum eColumnTypes As Integer
        Caption = 0
        Unit
        Producer
        Processor
        Distributor
        Wholesaler
        Retailer
        Total
    End Enum

    Private Enum eRowTypes As Integer
        Header = 0
        Production
        ProductionLive
        RevenueProductsMain
        RevenueProductsOther
        RevenueTickets
        RevenueSubsidies
        RevenueTotal
        CostSalariesShares
        CostRawmaterial
        CostInput
        CostTaxes
        CostManagementRoyaltyCertificationObservers
        Cost
        Profit
        TotalUtility
        AverageSaleries
        GDPContribution
        NumberOfJobsFemaleTotal
        NumberOfJobsMaleTotal
        NumberOfJobsTotal
        NumberOfWorkerDependents
        NumberOfOwnerDependents
        NumberOfDependentsTotal
    End Enum

    Public Sub New(uic As cUIContext)
        MyBase.New()
        Me.UIContext = uic
    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property
    Protected Overrides Sub FillData()
        ' NOP
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' ToDo: globalize this method!

        Dim cell As SourceGrid2.Cells.Real.Cell = Nothing

        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GridToolTipActive = True
        Me.Selection.SelectionMode = GridSelectionMode.Cell
        Me.Selection.AutoCopyPaste = True
        Me.Selection.AutoClear = False
        Me.Selection.ProtectReadOnly = True
        Me.Dock = DockStyle.Fill
        Me.FixedColumnWidths = False

        Me.Redim([Enum].GetValues(GetType(eRowTypes)).Length, 8)

        ' Column headers
        Me(eRowTypes.Header, 0) = New cEwEColumnHeaderCell("")
        Me(eRowTypes.Header, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_UNITS)
        Me(eRowTypes.Header, 2) = New cEwEColumnHeaderCell(My.Resources.UNIT_TYPE_PRODUCER)
        Me(eRowTypes.Header, 3) = New cEwEColumnHeaderCell(My.Resources.UNIT_TYPE_PROCESSING)
        Me(eRowTypes.Header, 4) = New cEwEColumnHeaderCell(My.Resources.UNIT_TYPE_DISTRIBUTION)
        Me(eRowTypes.Header, 5) = New cEwEColumnHeaderCell(My.Resources.UNIT_TYPE_WHOLESALER)
        Me(eRowTypes.Header, 6) = New cEwEColumnHeaderCell(My.Resources.UNIT_TYPE_RETAILER)
        Me(eRowTypes.Header, 7) = New cEwEColumnHeaderCell(SharedResources.HEADER_TOTAL)

        ' Row headers
        Me(eRowTypes.Production, 0) = Me.CreateRowHeaderCell("Production")
        Me(eRowTypes.Production, 1) = Me.CreateUnitCell("t")

        Me(eRowTypes.ProductionLive, 0) = Me.CreateRowHeaderCell("Production, live weight")
        Me(eRowTypes.ProductionLive, 1) = Me.CreateUnitCell("t")

        Me(eRowTypes.RevenueProductsMain, 0) = Me.CreateRowHeaderCell("Production value")
        Me(eRowTypes.RevenueProductsMain, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.RevenueProductsOther, 0) = Me.CreateRowHeaderCell("Other production value")
        Me(eRowTypes.RevenueProductsOther, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.RevenueTickets, 0) = Me.CreateRowHeaderCell("Ticket revenue")
        Me(eRowTypes.RevenueTickets, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.RevenueSubsidies, 0) = Me.CreateRowHeaderCell("Subsidies")
        Me(eRowTypes.RevenueSubsidies, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.RevenueTotal, 0) = Me.CreateRowHeaderCell("= Revenue", cStyleGuide.eStyleFlags.Sum)
        Me(eRowTypes.RevenueTotal, 1) = Me.CreateUnitCell(cUnits.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(eRowTypes.CostSalariesShares, 0) = Me.CreateRowHeaderCell("Salaries/shares")
        Me(eRowTypes.CostSalariesShares, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.CostRawmaterial, 0) = Me.CreateRowHeaderCell("Input (fish)")
        Me(eRowTypes.CostRawmaterial, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.CostInput, 0) = Me.CreateRowHeaderCell("Input other")
        Me(eRowTypes.CostInput, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.CostTaxes, 0) = Me.CreateRowHeaderCell("Taxes")
        Me(eRowTypes.CostTaxes, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.CostManagementRoyaltyCertificationObservers, 0) = Me.CreateRowHeaderCell("Management, royalty, certification, observers")
        Me(eRowTypes.CostManagementRoyaltyCertificationObservers, 1) = Me.CreateUnitCell(cUnits.Monetary)

        Me(eRowTypes.Cost, 0) = Me.CreateRowHeaderCell("= Cost", cStyleGuide.eStyleFlags.Sum)
        Me(eRowTypes.Cost, 1) = Me.CreateUnitCell(cUnits.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(eRowTypes.Profit, 0) = Me.CreateRowHeaderCell("= Profit", cStyleGuide.eStyleFlags.Sum)
        Me(eRowTypes.Profit, 1) = Me.CreateUnitCell(cUnits.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(eRowTypes.TotalUtility, 0) = Me.CreateRowHeaderCell("= Total utility", cStyleGuide.eStyleFlags.Sum)   'throughput
        Me(eRowTypes.TotalUtility, 1) = Me.CreateUnitCell(cUnits.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(eRowTypes.AverageSaleries, 0) = Me.CreateRowHeaderCell("Average saleries", cStyleGuide.eStyleFlags.Sum)
        Me(eRowTypes.AverageSaleries, 1) = Me.CreateUnitCell(cUnits.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(eRowTypes.GDPContribution, 0) = Me.CreateRowHeaderCell("GDP contribution", cStyleGuide.eStyleFlags.Sum)
        Me(eRowTypes.GDPContribution, 1) = Me.CreateUnitCell(cUnits.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(eRowTypes.NumberOfJobsFemaleTotal, 0) = Me.CreateRowHeaderCell("Jobs, female")
        Me(eRowTypes.NumberOfJobsFemaleTotal, 1) = Me.CreateUnitCell()

        Me(eRowTypes.NumberOfJobsMaleTotal, 0) = Me.CreateRowHeaderCell("Jobs, male")
        Me(eRowTypes.NumberOfJobsMaleTotal, 1) = Me.CreateUnitCell()

        Me(eRowTypes.NumberOfJobsTotal, 0) = Me.CreateRowHeaderCell("= Jobs, total", cStyleGuide.eStyleFlags.Sum)
        Me(eRowTypes.NumberOfJobsTotal, 1) = Me.CreateUnitCell("", cStyleGuide.eStyleFlags.Sum)

        Me(eRowTypes.NumberOfWorkerDependents, 0) = Me.CreateRowHeaderCell("Worker dependents")
        Me(eRowTypes.NumberOfWorkerDependents, 1) = Me.CreateUnitCell()

        Me(eRowTypes.NumberOfOwnerDependents, 0) = Me.CreateRowHeaderCell("Owner dependents")
        Me(eRowTypes.NumberOfOwnerDependents, 1) = Me.CreateUnitCell()

        Me(eRowTypes.NumberOfDependentsTotal, 0) = Me.CreateRowHeaderCell("= Dependents, total", cStyleGuide.eStyleFlags.Sum)
        Me(eRowTypes.NumberOfDependentsTotal, 1) = Me.CreateUnitCell("", cStyleGuide.eStyleFlags.Sum)

        ' Create data cells for unit cells
        For i As Integer = 0 To 4

            Me(eRowTypes.Production, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.ProductionLive, 2 + i) = Me.CreateDataCell()

            Me(eRowTypes.RevenueProductsMain, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.RevenueProductsOther, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.RevenueTickets, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.RevenueSubsidies, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.RevenueTotal, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me(eRowTypes.CostSalariesShares, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.CostRawmaterial, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.CostInput, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.CostTaxes, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.CostManagementRoyaltyCertificationObservers, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.Cost, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
            Me(eRowTypes.Profit, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
            Me(eRowTypes.TotalUtility, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me(eRowTypes.AverageSaleries, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
            Me(eRowTypes.GDPContribution, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me(eRowTypes.NumberOfJobsFemaleTotal, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.NumberOfJobsMaleTotal, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.NumberOfJobsTotal, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me(eRowTypes.NumberOfWorkerDependents, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.NumberOfOwnerDependents, 2 + i) = Me.CreateDataCell()
            Me(eRowTypes.NumberOfDependentsTotal, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

        Next i

        ' Create total cells
        For iRow As Integer = 3 To Me.RowsCount - 1
            Me(iRow, eColumnTypes.Total) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
        Next

        Me.FixedRows = 2
        Me.FixedColumns = 2
        Me.AutoSize = True
        Me.AutoSizeAll()
        Me.AutoSizeColumn(0, 140)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub ShowResults(iFleet As Integer, lUnits As cUnit(), results As cValueChainResults, iYear As Integer) _
        Implements IResultView.ShowResults

        ' Split units in the different types
        Dim alUnits(5) As List(Of cUnit)
        Dim cell As SourceGrid2.Cells.Real.Cell = Nothing

        Dim bTotal As Boolean = False
        Dim sTotal As Single = 0.0!

        For i As Integer = 0 To 4
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
                Case cUnitFactory.eUnitType.Wholesaler
                    alUnits(3).Add(unit)
                Case cUnitFactory.eUnitType.Retailer
                    alUnits(4).Add(unit)
            End Select
        Next

        ' Populate data cells
        For i As Integer = 0 To 4

            Me.UpdateDataCell(Me(eRowTypes.Production, 2 + i), results, cValueChainResults.eVariableType.Production, alUnits(i).ToArray, iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.ProductionLive, 2 + i), results, cValueChainResults.eVariableType.ProductionLive, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(eRowTypes.RevenueProductsMain, 2 + i), results, cValueChainResults.eVariableType.RevenueProductsMain, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.RevenueProductsOther, 2 + i), results, cValueChainResults.eVariableType.RevenueProductsOther, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.RevenueTickets, 2 + i), results, cValueChainResults.eVariableType.RevenueTickets, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.RevenueSubsidies, 2 + i), results, cValueChainResults.eVariableType.RevenueSubsidies, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.RevenueTotal, 2 + i), results, cValueChainResults.eVariableType.RevenueTotal, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(eRowTypes.CostSalariesShares, 2 + i), results, cValueChainResults.eVariableType.CostSalariesShares, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.CostRawmaterial, 2 + i), results, cValueChainResults.eVariableType.CostRawmaterial, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.CostInput, 2 + i), results, cValueChainResults.eVariableType.CostInput, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.CostTaxes, 2 + i), results, cValueChainResults.eVariableType.CostTaxes, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.CostManagementRoyaltyCertificationObservers, 2 + i), results, cValueChainResults.eVariableType.CostManagementRoyaltyCertificationObservers, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.Cost, 2 + i), results, cValueChainResults.eVariableType.Cost, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.Profit, 2 + i), results, cValueChainResults.eVariableType.Profit, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.TotalUtility, 2 + i), results, cValueChainResults.eVariableType.TotalUtility, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(eRowTypes.GDPContribution, 2 + i), results, cValueChainResults.eVariableType.GDPContribution, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(eRowTypes.NumberOfJobsFemaleTotal, 2 + i), results, cValueChainResults.eVariableType.NumberOfJobsFemaleTotal, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.NumberOfJobsMaleTotal, 2 + i), results, cValueChainResults.eVariableType.NumberOfJobsMaleTotal, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.NumberOfJobsTotal, 2 + i), results, cValueChainResults.eVariableType.NumberOfJobsTotal, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(eRowTypes.NumberOfWorkerDependents, 2 + i), results, cValueChainResults.eVariableType.NumberOfWorkerDependents, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.NumberOfOwnerDependents, 2 + i), results, cValueChainResults.eVariableType.NumberOfOwnerDependents, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(eRowTypes.NumberOfDependentsTotal, 2 + i), results, cValueChainResults.eVariableType.NumberOfDependentsTotal, alUnits(i).ToArray(), iFleet, iYear)

            ' Calc avg. salaries on the fly
            Dim sNumJobs As Single = CSng(Me(eRowTypes.NumberOfJobsTotal, 2 + i).Value)
            Dim sSalaries As Single = CSng(Me(eRowTypes.CostSalariesShares, 2 + i).Value)
            Dim sAvgSal As Single = 0
            If sNumJobs > 0 Then sAvgSal = sSalaries / sNumJobs
            Me.UpdateDataCell(Me(eRowTypes.AverageSaleries, 2 + i), sAvgSal)

        Next i

        ' Create total cells
        For Each row As eRowTypes In [Enum].GetValues(GetType(eRowTypes))
            Select Case row
                Case eRowTypes.Header, eRowTypes.Production, eRowTypes.AverageSaleries
                    bTotal = False
                Case Else
                    bTotal = True
            End Select

            If (bTotal) Then
                sTotal = 0.0!
                ' What if units are missing?
                For iCol As Integer = eColumnTypes.Producer To eColumnTypes.Retailer
                    Try
                        sTotal += CSng(Val(Me(row, iCol).Value))
                    Catch ex As Exception
                        Debug.Assert(False, ex.Message)
                    End Try
                Next
                Me.UpdateDataCell(Me(row, eColumnTypes.Total), sTotal)
            End If
        Next

        Me.FixedColumnWidths = False
        Me.InvalidateCells()

    End Sub

    Private Function CreateRowHeaderCell(strLabel As String,
                                         Optional style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Names) As cEwECell
        Dim cell As New cEwERowHeaderCell(strLabel)
        cell.Style = style
        Return cell
    End Function

    Private Function CreateUnitCell(Optional strUnit As String = "",
                                    Optional style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Names,
                                    Optional strUnitMask As String = "{0}") As cEwECell
        Dim cell As New cEwEUnitCell(strUnitMask, strUnit)
        cell.Style = style
        cell.EditableMode = EditableMode.None
        cell.EnableEdit = False
        Return cell
    End Function

    Private Function CreateDataCell(Optional style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK) As cEwECell

        Dim cell As New cEwECell(0, GetType(Single))

        ' Prettinize
        cell.Style = style
        cell.EditableMode = EditableMode.None
        cell.EnableEdit = False
        cell.SuppressZero(0.0!) = True
        ' No decimals in results
        cell.NumDigits = 0
        ' Group digits
        cell.GroupDigits = TriState.True
        cell.VisualModel.TextAlignment = Drawing.ContentAlignment.MiddleRight

        Return cell

    End Function

    Private Sub UpdateDataCell(cell As Cells.ICell, _
                               results As cValueChainResults, _
                               vn As cValueChainResults.eVariableType, _
                               lUnits As cUnit(), _
                               iFleet As Integer, _
                               iYear As Integer)

        Dim iTimeMin As Integer = 1
        Dim iTimeMax As Integer = 1
        Dim sValue As Single = 0

        If iYear > 0 Then
            Dim sSteps As Single = CSng(Me.Core.nEcosimTimeSteps / Me.Core.nEcosimYears)
            iTimeMin = CInt((iYear - 1) * sSteps) + 1
            iTimeMax = CInt(iYear * sSteps)
        End If

        For iTimeStep As Integer = iTimeMin To iTimeMax
            sValue += results.GetTimeStepTotal(vn, iTimeStep, lUnits, iFleet, cValueChainResults.GetVariableContributionType(vn))
        Next
        Me.UpdateDataCell(cell, sValue)

    End Sub

    Private Sub UpdateDataCell(cell As Cells.ICell, sValue As Single)
        Try
            If (cell IsNot Nothing) Then
                cell.Value = Math.Round(sValue)
            Else
                ' WHoops
            End If
        Catch ex As Exception
            ' Hmm
        End Try
    End Sub

End Class
