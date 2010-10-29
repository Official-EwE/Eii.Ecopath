#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class cGridEcospaceResultsGear
        : Inherits gridResultsBase

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            'Define grid dimensions
            Me.Redim(1, 12)

            'Define column header
            Me(0, 0) = New EwEColumnHeaderCell("")
            'Gear name
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
            ' Catch (Start)
            Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_CATCH_START)
            ' Catch (End)
            Me(0, 3) = New EwEColumnHeaderCell(SharedResources.HEADER_CATCH_END)
            ' Catch (E/S)
            Me(0, 4) = New EwEColumnHeaderCell(SharedResources.HEADER_CATCH_E_OVER_S)
            ' Value (Start)
            Me(0, 5) = New EwEColumnHeaderCell(SharedResources.HEADER_VALUESTART)
            ' Value (End)
            Me(0, 6) = New EwEColumnHeaderCell(SharedResources.HEADER_VALUEEND)
            ' Value (E/S)
            Me(0, 7) = New EwEColumnHeaderCell(SharedResources.HEADER_VALUEES)
            ' Cost (Start)
            Me(0, 8) = New EwEColumnHeaderCell(SharedResources.HEADER_COST_START)
            ' Cost (End)
            Me(0, 9) = New EwEColumnHeaderCell(SharedResources.HEADER_COST_END)
            ' Cost (E/S)
            Me(0, 10) = New EwEColumnHeaderCell(SharedResources.HEADER_COST_E_OVER_S)
            ' Effort (E/S)
            Me(0, 11) = New EwEColumnHeaderCell(SharedResources.HEADER_EFFORTES)

        End Sub

        'This method init the cells, its visual and data models. 
        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return

            'This method init the cells, its visual and data models. 
            Dim core As cCore = Me.Core

            Dim aName(core.nFleets) As String
            For i As Integer = 1 To core.nFleets
                aName(i) = core.EcospaceFleets(i).Name
            Next

            Dim aCalc() As Integer = {4, 7, 10}

            Me.InitCells(core.nFleets + 1, aName, aCalc)

            Me.UpdateData()

        End Sub

        Private Sub UpdateData()

            If (Me.UIContext Is Nothing) Then Return

            Dim core As cCore = Me.Core
            Dim source As cEcospaceFleetOutput = Nothing

            Dim totalValue(0 To 11) As Single
            Me.InitTotalArray(totalValue)

            For fleetIndex As Integer = 1 To core.nFleets

                source = core.EcospaceFleetOutput(fleetIndex)

                SetCellValue(fleetIndex, 2, source.CatchStart, totalValue)
                SetCellValue(fleetIndex, 3, source.CatchEnd, totalValue)

                If source.CatchStart > 0 And source.CatchEnd > 0 Then
                    SetCellValue(fleetIndex, 4, CSng(source.CatchEnd / source.CatchStart), totalValue)
                End If

                SetCellValue(fleetIndex, 5, source.ValueStart, totalValue)
                SetCellValue(fleetIndex, 6, source.ValueEnd, totalValue)

                If source.ValueStart > 0 And source.ValueEnd > 0 Then
                    SetCellValue(fleetIndex, 7, CSng(source.ValueEnd / source.ValueStart), totalValue)
                End If

                SetCellValue(fleetIndex, 8, source.CostStart, totalValue)
                SetCellValue(fleetIndex, 9, source.CostEnd, totalValue)

                If source.CostStart > 0 And source.CostEnd > 0 Then
                    SetCellValue(fleetIndex, 10, CSng(source.CostEnd / source.CostStart), totalValue)
                End If

                SetCellValue(fleetIndex, 11, source.EffortES)

            Next

            'Display total values
            For columnIndex As Integer = 2 To Me.ColumnsCount - 1
                If columnIndex = 4 Or columnIndex = 7 Or columnIndex = 10 Then
                    If totalValue(columnIndex - 2) > 0 And totalValue(columnIndex - 1) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = totalValue(columnIndex - 1) / totalValue(columnIndex - 2)
                    End If
                Else
                    If totalValue(columnIndex) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = totalValue(columnIndex)
                    End If
                End If
            Next

        End Sub

    End Class

End Namespace
