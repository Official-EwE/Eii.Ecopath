#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class EcosimResultsGridFleet
        : Inherits gridResultsBase

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            'Define grid dimensions
            Me.Redim(1, 12)

            'Define column header
            Me(0, 0) = New EwEColumnHeaderCell("")
            'Gear name
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
            ' Catch (Start)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHSTART)
            ' Catch (End)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHEND)
            ' Catch (E/S)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHES)
            ' Value (Start)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUESTART)
            ' Value (End)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUEEND)
            ' Value (E/S)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUEES)
            ' Cost (Start)
            Me(0, 8) = New EwEColumnHeaderCell(My.Resources.HEADER_COSTSTART)
            ' Cost (End)
            Me(0, 9) = New EwEColumnHeaderCell(My.Resources.HEADER_COSTEND)
            ' Cost (E/S)
            Me(0, 10) = New EwEColumnHeaderCell(My.Resources.HEADER_COSTES)
            ' Effort (E/S)
            Me(0, 11) = New EwEColumnHeaderCell(My.Resources.HEADER_EFFORTES)

        End Sub

        'This method init the cells, its visual and data models. 
        Protected Overrides Sub FillData()

            Dim astrNames(Core.nFleets) As String

            For i As Integer = 1 To core.nFleets
                astrNames(i) = core.EcosimFleetOutput(i).Name
            Next

            Dim aCalc() As Integer = {4, 7, 10}

            Me.InitCells(core.nFleets + 1, astrNames, aCalc)

            Me.UpdateData()

        End Sub

        Friend Sub updateData()

            Dim source As cEcosimFleetOutput = Nothing
            Dim ts As cTimeSeries = Nothing
            'Dim bForcedCatch As Boolean = False
            Dim styleCost As cStyleGuide.eStyleFlags = 0

            Dim totalValue(0 To 11) As Single
            Me.InitTotalArray(totalValue)

            For fleetIndex As Integer = 1 To core.nFleets

                source = Core.EcosimFleetOutput(fleetIndex)

                If source.CatchStart > 0 Then SetCellValue(fleetIndex, 2, source.CatchStart, totalValue)
                If source.CatchEnd > 0 Then SetCellValue(fleetIndex, 3, source.CatchEnd, totalValue)

                If source.CatchStart > 0 And source.CatchEnd > 0 Then
                    SetCellValue(fleetIndex, 4, CSng(source.CatchEnd / source.CatchStart), totalValue)
                End If

                If source.ValueStart > 0 Then SetCellValue(fleetIndex, 5, source.ValueStart, totalValue)
                If source.ValueEnd > 0 Then SetCellValue(fleetIndex, 6, source.ValueEnd, totalValue)

                If source.ValueStart > 0 And source.ValueEnd > 0 Then
                    SetCellValue(fleetIndex, 7, CSng(source.ValueEnd / source.ValueStart), totalValue)
                End If

                If source.CostStart > 0 Then SetCellValue(fleetIndex, 8, source.CostStart, totalValue, styleCost)
                If source.CostEnd > 0 Then SetCellValue(fleetIndex, 9, source.CostEnd, totalValue, styleCost)
                If source.CostStart > 0 And source.CostEnd > 0 Then
                    SetCellValue(fleetIndex, 10, CSng(source.CostEnd / source.CostStart), totalValue, styleCost)
                End If

                'jb feb??08 cEcosimFleetSummary.Effort is endEffort/StartEffort
                SetCellValue(fleetIndex, 11, CSng(source.Effort), totalValue)

            Next

            'Display total values
            ' Bug fix 413: will not sum last column
            For columnIndex As Integer = 2 To Me.ColumnsCount - 2
                ' Hmm, how about using constants here?
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

            Me.Refresh()

        End Sub

    End Class

End Namespace

