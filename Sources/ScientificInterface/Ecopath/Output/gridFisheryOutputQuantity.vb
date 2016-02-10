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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class gridFisheryOutputQuantity
        : Inherits EwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub InitStyle()

            Dim source As cCoreInputOutputBase = Nothing
            Dim aunits As eUnitType() = New eUnitType() {eUnitType.Currency, eUnitType.Time}

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            'Define grid dimensions
            Me.Redim(1, Core.nFleets + 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet name
            For fleetIndex As Integer = 1 To Core.nFleets
                source = Core.FleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New PropertyColumnHeaderCell(Me.PropertyManager, _
                                                                     source, eVarNameFlags.Name, Nothing, _
                                                                     SharedResources.HEADER_X_UNIT_PER_UNIT, aunits)
            Next

            ' Total catch column
            Me(0, Core.nFleets + 2) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTALCATCH_UNIT_PY, aunits)

            Me.FixedColumns = 2
        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim iRow As Integer = -1

            'Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If Core.nFleets = 0 Then Return

            ''Create rows for all groups and sum quantities in each row
            For rowIndex As Integer = 1 To Core.nGroups
                source = Core.EcoPathGroupInputs(rowIndex)
                iRow = Me.AddRow()
                FillRows(iRow, source)
            Next rowIndex

            'Create "Total catch" row (sum values in each column)
            FillTotalCatchRow()

            'Create "Trophic level" row
            FillTrophicLevelRow()

        End Sub

        Private Sub FillRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase)

            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim propLandings As cProperty = Nothing
            Dim propDiscards As cProperty = Nothing
            Dim alSumLandingsDiscards As ArrayList = New ArrayList()
            Dim opSumLandingsDiscards As cMultiOperation = Nothing
            Dim propSumLandingsDiscards As cFormulaProperty = Nothing
            Dim propCell As PropertyCell = Nothing

            Dim alSumRow As ArrayList = New ArrayList()
            Dim opSumRow As cMultiOperation = Nothing
            Dim propSumRow As cFormulaProperty = Nothing

            Dim blnAllZeroCells As Boolean = True

            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

            ' For each fleet (each column) 
            For fleetIndex As Integer = 1 To core.nFleets
                alSumLandingsDiscards.Clear()
                ' Get the fleet object 
                sourceSec = core.FleetInputs(fleetIndex)
                ' Get the index landing property
                propLandings = PropertyManager.GetProperty(sourceSec, eVarNameFlags.Landings, source)
                alSumLandingsDiscards.Add(propLandings)
                ' Get the index discard property
                propDiscards = PropertyManager.GetProperty(sourceSec, eVarNameFlags.Discards, source)
                alSumLandingsDiscards.Add(propDiscards)
                ' Set the property to the cell
                opSumLandingsDiscards = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumLandingsDiscards.ToArray())
                propSumLandingsDiscards = Me.Formula(opSumLandingsDiscards)
                propCell = New PropertyCell(propSumLandingsDiscards)
                ' Configure the cell
                propCell.SuppressZero = True
                ' Set the cell
                Me(iRow, fleetIndex + 1) = propCell
                If CSng(propCell.Value) > 0.0 Then blnAllZeroCells = False

                'Sum quantities in a row
                alSumRow.Add(propSumLandingsDiscards)
            Next

            'Display the sum of quantities in a row
            opSumRow = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumRow.ToArray())
            propSumRow = Me.Formula(opSumRow)
            propCell = New PropertyCell(propSumRow)
            Me(iRow, Me.ColumnsCount - 1) = propCell

            If blnAllZeroCells = True Then Me.RowsCount = Me.Rows.Count - 1
        End Sub

        Private Sub FillTotalCatchRow()

            Dim iRow As Integer
            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim propLandings As cProperty = Nothing
            Dim propDiscards As cProperty = Nothing
            Dim alSumLandingsDiscards As ArrayList = New ArrayList()
            Dim opSumLandingsDiscards As cMultiOperation = Nothing
            Dim propSumLandingsDiscards As cFormulaProperty = Nothing

            Dim alSumCol As New ArrayList()
            Dim opSumCol As cMultiOperation = Nothing
            Dim propSumCol As cFormulaProperty = Nothing

            Dim alSumAll As New ArrayList()
            Dim opSumAll As cMultiOperation = Nothing
            Dim propSumAll As cFormulaProperty = Nothing

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell(SharedResources.HEADER_TOTALCATCH)

            alSumAll.Clear()
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To core.nGroups
                    sourceSec = core.EcoPathGroupInputs(rowIndex)
                    alSumLandingsDiscards.Clear()
                    ' Get the index landing property
                    propLandings = Me.PropertyManager.GetProperty(source, eVarNameFlags.Landings, sourceSec)
                    alSumLandingsDiscards.Add(propLandings)
                    ' Get the index discard property
                    propDiscards = Me.PropertyManager.GetProperty(source, eVarNameFlags.Discards, sourceSec)
                    alSumLandingsDiscards.Add(propDiscards)
                    ' Set the property 
                    opSumLandingsDiscards = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumLandingsDiscards.ToArray())
                    propSumLandingsDiscards = Me.Formula(opSumLandingsDiscards)

                    'Sum values in a column
                    alSumCol.Add(propSumLandingsDiscards)

                    'Sum all values
                    alSumAll.Add(propSumLandingsDiscards)
                Next

                'Display the sum of values in a column
                opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
                propSumCol = Me.Formula(opSumCol)
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(propSumCol)
            Next

            'Display the sum of all values
            opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
            propSumAll = Me.Formula(opSumAll)
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = New PropertyCell(propSumAll)

        End Sub

        Private Sub FillTrophicLevelRow()

            Dim iRow As Integer
            Dim sourceGrpIntput As cCoreInputOutputBase = Nothing
            Dim sourceGrpIntputSec As cCoreInputOutputBase = Nothing
            Dim sourceGrpOutput As cCoreInputOutputBase = Nothing
            Dim propLandings As cProperty = Nothing
            Dim propDiscards As cProperty = Nothing
            Dim propTTLX As cProperty = Nothing

            Dim alSumLandingsDiscards As ArrayList = New ArrayList()
            Dim opSumLandingsDiscards As cMultiOperation = Nothing
            Dim propSumLandingsDiscards As cFormulaProperty = Nothing

            Dim alProdQuantityTTLX As ArrayList = New ArrayList()
            Dim opProdQuantityTTLX As cMultiOperation = Nothing
            Dim propProdQuantityTTLX As cFormulaProperty = Nothing

            Dim alSumQuantityCol As New ArrayList()
            Dim opSumQuantityCol As cMultiOperation = Nothing
            Dim propSumQuantityCol As cFormulaProperty = Nothing

            Dim alSumQuantityTTLXCol As New ArrayList()
            Dim opSumQuantityTTLXCol As cMultiOperation = Nothing
            Dim propSumQuantityTTLXCol As cFormulaProperty = Nothing

            Dim alSumQuantityAll As New ArrayList()
            Dim opSumQuantityAll As cMultiOperation = Nothing
            Dim propSumQuantityAll As cFormulaProperty = Nothing

            Dim alSumQuantityTTLXAll As New ArrayList()
            Dim opSumQuantityTTLXAll As cMultiOperation = Nothing
            Dim propSumQuantityTTLXAll As cFormulaProperty = Nothing

            Dim opDivTTLXQuantity As cBinaryOperation = Nothing
            Dim propDivTTLXQuantity As cFormulaProperty = Nothing

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell(eVarNameFlags.TTLX)

            alSumQuantityAll.Clear()
            alSumQuantityTTLXAll.Clear()
            For fleetIndex As Integer = 1 To Core.nFleets
                sourceGrpIntput = Core.FleetInputs(fleetIndex)
                alSumQuantityCol.Clear()
                alSumQuantityTTLXCol.Clear()

                For rowIndex As Integer = 1 To Core.nGroups
                    sourceGrpIntputSec = Core.EcoPathGroupInputs(rowIndex)
                    sourceGrpOutput = Core.EcoPathGroupOutputs(rowIndex)
                    alSumLandingsDiscards.Clear()
                    alProdQuantityTTLX.Clear()
                    ' Get the index landing property
                    propLandings = Me.PropertyManager.GetProperty(sourceGrpIntput, eVarNameFlags.Landings, sourceGrpIntputSec)
                    alSumLandingsDiscards.Add(propLandings)
                    ' Get the index discard property
                    propDiscards = Me.PropertyManager.GetProperty(sourceGrpIntput, eVarNameFlags.Discards, sourceGrpIntputSec)
                    alSumLandingsDiscards.Add(propDiscards)
                    ' Get the index TTLX property
                    propTTLX = Me.PropertyManager.GetProperty(sourceGrpOutput, eVarNameFlags.TTLX)
                    'propCell = New PropertyCell(CType(propTTLX, cProperty))
                    'MsgBox("TTLX" & CStr(propCell.Value), MsgBoxStyle.Information)
                    alProdQuantityTTLX.Add(propTTLX)
                    ' Set the property 
                    opSumLandingsDiscards = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumLandingsDiscards.ToArray())
                    propSumLandingsDiscards = Me.Formula(opSumLandingsDiscards)

                    alProdQuantityTTLX.Add(propSumLandingsDiscards)
                    opProdQuantityTTLX = New cMultiOperation(cMultiOperation.eOperatorType.Multiply, alProdQuantityTTLX.ToArray())
                    propProdQuantityTTLX = Me.Formula(opProdQuantityTTLX)

                    'Sum quantity in a column
                    alSumQuantityCol.Add(propSumLandingsDiscards)
                    'Sum quantity*TTLX in a column
                    alSumQuantityTTLXCol.Add(propProdQuantityTTLX)

                    'Sum all quantity
                    alSumQuantityAll.Add(propSumLandingsDiscards)
                    'Sum all quantity*TTLX
                    alSumQuantityTTLXAll.Add(propProdQuantityTTLX)
                Next

                'Display (sum of quantity*TTLX in a column) / (sum of quantity in a column)
                opSumQuantityCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumQuantityCol.ToArray())
                propSumQuantityCol = Me.Formula(opSumQuantityCol)
                opSumQuantityTTLXCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumQuantityTTLXCol.ToArray())
                propSumQuantityTTLXCol = Me.Formula(opSumQuantityTTLXCol)
                opDivTTLXQuantity = New cBinaryOperation(cBinaryOperation.eOperatorType.Divide, propSumQuantityTTLXCol, propSumQuantityCol)
                propDivTTLXQuantity = Me.Formula(opDivTTLXQuantity)
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(propDivTTLXQuantity)
            Next

            'Display (sum of all quantity*TTLX) / (sum of all quantity)
            opSumQuantityAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumQuantityAll.ToArray())
            propSumQuantityAll = Me.Formula(opSumQuantityAll)
            opSumQuantityTTLXAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumQuantityTTLXAll.ToArray())
            propSumQuantityTTLXAll = Me.Formula(opSumQuantityTTLXAll)
            opDivTTLXQuantity = New cBinaryOperation(cBinaryOperation.eOperatorType.Divide, propSumQuantityTTLXAll, propSumQuantityAll)
            propDivTTLXQuantity = Me.Formula(opDivTTLXQuantity)
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = New PropertyCell(propDivTTLXQuantity)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
