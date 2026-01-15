' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Output

    
    Public Class gridFisheryOutputDiscardSurvival
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            Dim source As cCoreInputOutputBase = Nothing

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            'Define grid dimensions
            Me.Redim(1, Me.Core.nFleets + 3)

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet name
            For fleetIndex As Integer = 1 To Me.Core.nFleets
                source = Me.Core.EcopathFleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New cPropertyColumnHeaderCell(Me.PropertyManager,
                                                                     source, eVarNameFlags.Name, Nothing,
                                                                     cUnits.CurrencyOverTime)
            Next

            ' Total column
            Me(0, Me.Core.nFleets + 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_DISCARD_SURV)

            Me.FixedColumns = 2
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim iRow As Integer = -1

            ' Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If Me.Core.nFleets = 0 Then Return

            ' Create rows for all groups and sum quantities in each row
            For iGroup As Integer = 1 To Me.Core.nGroups
                iRow = Me.AddRow()
                Me.FillRows(iRow, iGroup)
            Next iGroup

            ' Create column totals
            Me.FillTotalsRow()

        End Sub

        Private Sub FillRows(iRow As Integer, iGroup As Integer)

            Dim group As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(iGroup)
            Dim fleetOut As cEcopathFleetOutput = Nothing
            Dim sVal As Single = 0
            Dim sTot As Single = 0

            Me(iRow, 0) = New cEwERowHeaderCell(CStr(iGroup))
            Me(iRow, 1) = New cEwERowHeaderCell(group.Name)

            ' For each fleet (each column) 
            For iFleet As Integer = 1 To Me.Core.nFleets
                ' Get the fleet object 
                fleetOut = Me.Core.EcopathFleetOutputs(iFleet)
                sVal = fleetOut.DiscardSurvivalByGroup(iGroup)
                Dim cell As New cEwECell(sVal, cStyleGuide.eStyleFlags.ValueComputed Or cStyleGuide.eStyleFlags.NotEditable)
                cell.SuppressZero = True
                ' Set the cell
                Me(iRow, iFleet + 1) = cell

                sTot += sVal
            Next

            ' Total column
            Me(iRow, Me.ColumnsCount - 1) = New cEwECell(sTot, cStyleGuide.eStyleFlags.Sum Or cStyleGuide.eStyleFlags.NotEditable)

        End Sub

        Private Sub FillTotalsRow()

            Dim fleetOut As cEcopathFleetOutput = Nothing
            Dim iRow As Integer = Me.AddRow()
            Dim sTot As Single = 0

            Me(Me.RowsCount - 1, 0) = New cEwERowHeaderCell()
            Me(Me.RowsCount - 1, 1) = New cEwERowHeaderCell(SharedResources.HEADER_DISCARD_SURV)

            For iFleet As Integer = 1 To Me.Core.nFleets
                Dim sFleetTot As Single = 0
                fleetOut = Me.Core.EcopathFleetOutputs(iFleet)

                For iGroup As Integer = 1 To Me.Core.nGroups
                    sFleetTot += fleetOut.DiscardSurvivalByGroup(iGroup)
                Next
                Me(Me.RowsCount - 1, iFleet + 1) = New cEwECell(sFleetTot, cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Sum)
                sTot += sFleetTot
            Next
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = New cEwECell(sTot, cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Sum)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property

    End Class

End Namespace
