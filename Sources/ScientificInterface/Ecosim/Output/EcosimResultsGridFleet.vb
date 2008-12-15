'==============================================================================
'
' $Log: EcosimResultsGridFleet.vb,v $
' Revision 1.2  2008/12/15 15:53:26  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:47  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/08/05 17:44:37  jeroens
' Removed outdated alert
'
' Revision 1.10  2008/07/29 15:49:17  sherman
' Removed last sum column on E/S
'
' Revision 1.9  2008/06/02 00:01:32  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.8  2008/05/11 02:51:35  jeroens
' Standardized series of resource strings
'
' Revision 1.7  2008/04/07 02:31:11  jeroens
' Cleaning up resources
'
' Revision 1.6  2008/02/22 17:54:41  jeroens
' Fixed bug 413
'
' Revision 1.5  2008/02/17 16:13:31  joeb
' Added Ecosim Effort
'
' Revision 1.4  2008/02/13 19:48:44  jeroens
' Left alert that Effort E/S is not yet populated
'
' Revision 1.3  2007/10/12 15:20:50  joeb
' Changes for Results forms
'
' Revision 1.2  2007/09/19 22:15:18  joeb
' Added Summary data
'
' Revision 1.1  2007/08/07 16:43:40  jeroens
' * Renamed Gear to Fleet
'
' Revision 1.4  2007/05/03 18:56:24  fgao
' Removed registerGrid..for output grids..
'
' Revision 1.3  2007/04/29 03:45:12  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

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

            Dim core As cCore = cCore.GetInstance()

            Dim astrNames(core.nFleets) As String

            For i As Integer = 1 To core.nFleets
                astrNames(i) = core.EcosimFleetSummaries(i).Name
            Next

            Dim aCalc() As Integer = {4, 7, 10}

            Me.InitCells(core.nFleets + 1, astrNames, aCalc)

            Me.UpdateData()

        End Sub

        Friend Sub updateData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cEcosimFleetSummary = Nothing

            Dim totalValue(0 To 11) As Single
            Me.InitTotalArray(totalValue)

            For fleetIndex As Integer = 1 To core.nFleets

                source = core.EcosimFleetSummaries(fleetIndex)
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

                If source.CostStart > 0 Then SetCellValue(fleetIndex, 8, source.CostStart, totalValue)
                If source.CostEnd > 0 Then SetCellValue(fleetIndex, 9, source.CostEnd, totalValue)

                If source.CostStart > 0 And source.CostEnd > 0 Then
                    SetCellValue(fleetIndex, 10, CSng(source.CostEnd / source.CostStart), totalValue)
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

        End Sub

    End Class

End Namespace

