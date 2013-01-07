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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region ' Imports

<CLSCompliant(False)> _
Public Class gridEcopathResult
    Inherits EwEGrid
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

    Public Sub New(ByVal uic As cUIContext)
        MyBase.New()
        Me.UIContext = uic
    End Sub

    Protected Overrides Sub FillData()
        ' NOP
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' ToDo: globalize this method!

        Dim cell As SourceGrid2.Cells.Real.Cell = Nothing

        Me.AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GridToolTipActive = True
        Me.Selection.SelectionMode = GridSelectionMode.Cell
        Me.Selection.AutoCopyPaste = True
        Me.Selection.AutoClear = False
        Me.Selection.ProtectReadOnly = True
        Me.Dock = DockStyle.Fill
        Me.FixedColumnWidths = False

        Me.Redim(22, 8)

        ' Column headers
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_UNITS)
        Me(0, 2) = New EwEColumnHeaderCell(My.Resources.UNIT_TYPE_PRODUCER)
        Me(0, 3) = New EwEColumnHeaderCell(My.Resources.UNIT_TYPE_PROCESSING)
        Me(0, 4) = New EwEColumnHeaderCell(My.Resources.UNIT_TYPE_DISTRIBUTION)
        Me(0, 5) = New EwEColumnHeaderCell(My.Resources.UNIT_TYPE_WHOLESALER)
        Me(0, 6) = New EwEColumnHeaderCell(My.Resources.UNIT_TYPE_RETAILER)
        Me(0, 7) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTAL)

        ' Row headers
        Me(1, 0) = CreateRowHeaderCell("Production")
        Me(1, 1) = CreateUnitCell("t")

        Me(2, 0) = CreateRowHeaderCell("Production, live weight")
        Me(2, 1) = CreateUnitCell("t")

        Me(3, 0) = CreateRowHeaderCell("Production value")
        Me(3, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(4, 0) = CreateRowHeaderCell("Other production value")
        Me(4, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(5, 0) = CreateRowHeaderCell("Ticket revenue")
        Me(5, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(6, 0) = CreateRowHeaderCell("Subsidies")
        Me(6, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(7, 0) = CreateRowHeaderCell("= Revenue", cStyleGuide.eStyleFlags.Sum)
        Me(7, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(8, 0) = CreateRowHeaderCell("Salaries/shares")
        Me(8, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(9, 0) = CreateRowHeaderCell("Input (fish)")
        Me(9, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(10, 0) = CreateRowHeaderCell("Input other")
        Me(10, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(11, 0) = CreateRowHeaderCell("Taxes")
        Me(11, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(12, 0) = CreateRowHeaderCell("Management, royalty, certification, observers")
        Me(12, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary)

        Me(13, 0) = CreateRowHeaderCell("= Cost", cStyleGuide.eStyleFlags.Sum)
        Me(13, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(14, 0) = CreateRowHeaderCell("= Profit", cStyleGuide.eStyleFlags.Sum)
        Me(14, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(15, 0) = CreateRowHeaderCell("= Total utility", cStyleGuide.eStyleFlags.Sum)   'throughput
        Me(15, 1) = CreateUnitCell(cStyleGuide.eUnitType.Monetary, cStyleGuide.eStyleFlags.Sum)

        Me(16, 0) = CreateRowHeaderCell("Jobs, female")
        Me(16, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal)

        Me(17, 0) = CreateRowHeaderCell("Jobs, male")
        Me(17, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal)

        Me(18, 0) = CreateRowHeaderCell("= Jobs, total", cStyleGuide.eStyleFlags.Sum)
        Me(18, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal, cStyleGuide.eStyleFlags.Sum)

        Me(19, 0) = CreateRowHeaderCell("Worker dependents")
        Me(19, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal)

        Me(20, 0) = CreateRowHeaderCell("Owner dependents")
        Me(20, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal)

        Me(21, 0) = CreateRowHeaderCell("= Dependents, total", cStyleGuide.eStyleFlags.Sum)
        Me(21, 1) = CreateUnitCell(cStyleGuide.eUnitType.Nominal, cStyleGuide.eStyleFlags.Sum)

        ' Create data cells for unit cells
        For i As Integer = 0 To 4

            Me(1, 2 + i) = Me.CreateDataCell()
            Me(2, 2 + i) = Me.CreateDataCell()

            Me(3, 2 + i) = Me.CreateDataCell()
            Me(4, 2 + i) = Me.CreateDataCell()
            Me(5, 2 + i) = Me.CreateDataCell()
            Me(6, 2 + i) = Me.CreateDataCell()
            Me(7, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me(8, 2 + i) = Me.CreateDataCell()
            Me(9, 2 + i) = Me.CreateDataCell()
            Me(10, 2 + i) = Me.CreateDataCell()
            Me(11, 2 + i) = Me.CreateDataCell()
            Me(12, 2 + i) = Me.CreateDataCell()
            Me(13, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
            Me(14, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)
            Me(15, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me(16, 2 + i) = Me.CreateDataCell()
            Me(17, 2 + i) = Me.CreateDataCell()
            Me(18, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

            Me(19, 2 + i) = Me.CreateDataCell()
            Me(20, 2 + i) = Me.CreateDataCell()
            Me(21, 2 + i) = Me.CreateDataCell(cStyleGuide.eStyleFlags.Sum)

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
    Public Sub ShowResults(ByVal iFleet As Integer, _
                           ByVal lUnits As cUnit(), _
                           ByVal results As cResults, _
                           ByVal iYear As Integer) _
        Implements IResultView.ShowResults

        ' Split units in the different types
        Dim alUnits(5) As List(Of cUnit)
        Dim cell As SourceGrid2.Cells.Real.Cell = Nothing

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

            Me.UpdateDataCell(Me(1, 2 + i), results, cResults.eVariableType.Production, alUnits(i).ToArray, iFleet, iYear)
            Me.UpdateDataCell(Me(2, 2 + i), results, cResults.eVariableType.ProductionLive, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(3, 2 + i), results, cResults.eVariableType.RevenueProductsMain, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(4, 2 + i), results, cResults.eVariableType.RevenueProductsOther, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(5, 2 + i), results, cResults.eVariableType.RevenueTickets, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(6, 2 + i), results, cResults.eVariableType.RevenueSubsidies, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(7, 2 + i), results, cResults.eVariableType.RevenueTotal, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(8, 2 + i), results, cResults.eVariableType.CostSalariesShares, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(9, 2 + i), results, cResults.eVariableType.CostRawmaterial, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(10, 2 + i), results, cResults.eVariableType.CostInput, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(11, 2 + i), results, cResults.eVariableType.CostTaxes, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(12, 2 + i), results, cResults.eVariableType.CostManagementRoyaltyCertificationObservers, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(13, 2 + i), results, cResults.eVariableType.Cost, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(14, 2 + i), results, cResults.eVariableType.Profit, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(15, 2 + i), results, cResults.eVariableType.TotalUtility, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(16, 2 + i), results, cResults.eVariableType.NumberOfJobsFemaleTotal, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(17, 2 + i), results, cResults.eVariableType.NumberOfJobsMaleTotal, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(18, 2 + i), results, cResults.eVariableType.NumberOfJobsTotal, alUnits(i).ToArray(), iFleet, iYear)

            Me.UpdateDataCell(Me(19, 2 + i), results, cResults.eVariableType.NumberOfWorkerDependents, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(20, 2 + i), results, cResults.eVariableType.NumberOfOwnerDependents, alUnits(i).ToArray(), iFleet, iYear)
            Me.UpdateDataCell(Me(21, 2 + i), results, cResults.eVariableType.NumberOfDependentsTotal, alUnits(i).ToArray(), iFleet, iYear)

        Next i

        ' Create total cells
        For iRow As Integer = 3 To Me.RowsCount - 1
            Dim sTotal As Single = 0.0!
            For iCol As Integer = eColumnTypes.Producer To eColumnTypes.Retailer
                Try
                    sTotal += CSng(Val(Me(iRow, iCol).Value))
                Catch ex As Exception
                    Debug.Assert(False, ex.Message)
                End Try
            Next
            Me.UpdateDataCell(Me(iRow, eColumnTypes.Total), sTotal)
        Next

        Me.FixedColumnWidths = False
        Me.InvalidateCells()

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

        Dim cell As New EwECell(0, GetType(Single))

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

    Private Sub UpdateDataCell(ByVal cell As Cells.ICell, _
                               ByVal results As cResults, _
                               ByVal vn As cResults.eVariableType, _
                               ByVal lUnits As cUnit(), _
                               ByVal iFleet As Integer, _
                               ByVal iYear As Integer)

        Dim iTimeMin As Integer = 1
        Dim iTimeMax As Integer = 1
        Dim sValue As Single = 0

        If iYear > 0 Then
            Dim sSteps As Single = CSng(Me.Core.nEcosimTimeSteps / Me.Core.nEcosimYears)
            iTimeMin = CInt((iYear - 1) * sSteps) + 1
            iTimeMax = CInt(iYear * sSteps)
        End If

        For iTimeStep As Integer = iTimeMin To iTimeMax
            sValue += results.GetTimeStepTotal(vn, iTimeStep, lUnits, iFleet, cResults.GetVariableContributionType(vn))
        Next
        Me.UpdateDataCell(cell, sValue)

    End Sub

    Private Sub UpdateDataCell(ByVal cell As Cells.ICell, ByVal sValue As Single)
        Try
            cell.Value = sValue
        Catch ex As Exception
            ' Hmm
        End Try
    End Sub

End Class
