#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class FisheryOutputValueEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            Dim source As cCoreInputOutputBase = Nothing

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            MyBase.InitStyle()

            'Define grid dimensions
            Me.Redim(1, Core.nFleets + 5)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet name
            For fleetIndex As Integer = 1 To Core.nFleets
                source = Core.FleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            ' Catch value column
            Me(0, Core.nFleets + 2) = New EwEColumnHeaderCell(SharedResources.HEADER_CATCH_VALUE)
            Me(0, Core.nFleets + 3) = New EwEColumnHeaderCell(SharedResources.HEADER_NONMARKET_VALUE, cStyleGuide.eUnitType.Monetary)
            Me(0, Core.nFleets + 4) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTALVALUE, cStyleGuide.eUnitType.Monetary)

            Me.FixedColumns = 2
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub FillData()

            Dim source As cEcoPathGroupInput = Nothing
            Dim iRow As Integer = -1

            ' Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If core.nFleets = 0 Then Return

            ' Create rows for all groups and sum values in each row
            For rowIndex As Integer = 1 To core.nGroups
                source = core.EcoPathGroupInputs(rowIndex)
                iRow = Me.AddRow()
                FillRows(iRow, source)
            Next rowIndex

            'Create "Total value" row (sum values in each column)
            FillTotalValueRow()

            'Create "Total cost" row
            FillTotalCostRow()

            'Create "Total profit" row
            FillTotalProfitRow()

        End Sub

        Private Sub FillRows(ByVal iRow As Integer, ByVal source As cEcoPathGroupInput)

            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim propLandings As cProperty = Nothing

            ' Single marketprice property
            Dim propMarketPrice As cProperty = Nothing
            Dim alProdLandingsMarketPrice As New ArrayList()
            Dim opProdLandingsMarketPrice As cMultiOperation = Nothing
            Dim propProdLandingsMarketPrice As cFormulaProperty = Nothing

            ' Operation to sum landings non-market price
            Dim alNonMarketValue As New ArrayList()
            Dim opNonMarketValue As cBinaryOperation = Nothing
            Dim propProdNonMarketValue As cProperty = Nothing

            Dim propCell As PropertyCell = Nothing
            Dim alSumRow As ArrayList = New ArrayList()
            Dim opSumMarketValues As cMultiOperation = Nothing
            Dim propSumMarketValues As cFormulaProperty = Nothing

            ' Total value
            Dim opTotalValue As cBinaryOperation = Nothing
            Dim propTotalValue As cFormulaProperty = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

            alSumRow.Clear()
            ' For each fleet (each column) 
            For fleetIndex As Integer = 1 To core.nFleets
                alProdLandingsMarketPrice.Clear()
                ' Get the fleet object 
                sourceSec = core.FleetInputs(fleetIndex)
                ' Get the index landing property
                propLandings = Me.PropertyManager.GetProperty(sourceSec, eVarNameFlags.Landings, source)
                alProdLandingsMarketPrice.Add(propLandings)
                ' Get the index market price property
                propMarketPrice = Me.PropertyManager.GetProperty(sourceSec, eVarNameFlags.OffVesselPrice, source)
                alProdLandingsMarketPrice.Add(propMarketPrice)
                ' Set the property to the cell
                opProdLandingsMarketPrice = New cMultiOperation(cMultiOperation.eOperatorType.Multiply, alProdLandingsMarketPrice.ToArray())
                propProdLandingsMarketPrice = New cFormulaProperty(CType(opProdLandingsMarketPrice, cExpression))
                propCell = New PropertyCell(CType(propProdLandingsMarketPrice, cProperty))
                ' Configure the cell
                propCell.SuppressZero = True
                ' Set the cell
                Me(iRow, fleetIndex + 1) = propCell

                'Sum values in a row
                alSumRow.Add(propProdLandingsMarketPrice)

            Next

            'Display the sum of market values
            opSumMarketValues = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumRow.ToArray())
            propSumMarketValues = New cFormulaProperty(CType(opSumMarketValues, cExpression))
            propCell = New PropertyCell(CType(propSumMarketValues, cProperty))
            Me(iRow, Me.ColumnsCount - 3) = propCell

            ' Non-market value
            ' .. multiply group non-market value by calculated broup biomass
            opNonMarketValue = New cBinaryOperation(cBinaryOperation.eOperatorType.Multiply, _
                Me.PropertyManager.GetProperty(source, eVarNameFlags.NonMarketValue), _
                Me.PropertyManager.GetProperty(Core.EcoPathGroupOutputs(source.Index), eVarNameFlags.Biomass))
            propProdNonMarketValue = New cFormulaProperty(opNonMarketValue)
            propCell = New PropertyCell(CType(propProdNonMarketValue, cProperty))
            propCell.SuppressZero = True
            Me(iRow, Me.ColumnsCount - 2) = propCell

            ' Total value
            opTotalValue = New cBinaryOperation(cBinaryOperation.eOperatorType.Add, propSumMarketValues, propProdNonMarketValue)
            propTotalValue = New cFormulaProperty(opTotalValue)
            propCell = New PropertyCell(propTotalValue)
            Me(iRow, Me.ColumnsCount - 1) = propCell

        End Sub

        Private Sub FillTotalValueRow()

            Dim iRow As Integer
            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing

            Dim propLandings As cProperty = Nothing
            Dim propMarketPrice As cProperty = Nothing
            Dim alProdLandingsMarketPrice As ArrayList = New ArrayList()
            Dim opProdLandingsMarketPrice As cMultiOperation = Nothing
            Dim propProdLandingsMarketPrice As cFormulaProperty = Nothing

            Dim alSumCol As New ArrayList()
            Dim opSumCol As cMultiOperation = Nothing
            Dim propSumCol As cFormulaProperty = Nothing

            Dim alSumAll As New ArrayList()
            Dim opSumAll As cMultiOperation = Nothing
            Dim propSumAll As cFormulaProperty = Nothing

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell(SharedResources.HEADER_TOTALVALUE, cStyleGuide.eUnitType.Monetary)

            alSumAll.Clear()
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To core.nGroups
                    sourceSec = core.EcoPathGroupInputs(rowIndex)
                    alProdLandingsMarketPrice.Clear()
                    ' Get the index landing property
                    propLandings = Me.PropertyManager.GetProperty(source, eVarNameFlags.Landings, sourceSec)
                    alProdLandingsMarketPrice.Add(propLandings)
                    ' Get the index market price property
                    propMarketPrice = Me.PropertyManager.GetProperty(source, eVarNameFlags.OffVesselPrice, sourceSec)
                    alProdLandingsMarketPrice.Add(propMarketPrice)
                    ' Set the property 
                    opProdLandingsMarketPrice = New cMultiOperation(cMultiOperation.eOperatorType.Multiply, alProdLandingsMarketPrice.ToArray())
                    propProdLandingsMarketPrice = New cFormulaProperty(CType(opProdLandingsMarketPrice, cExpression))

                    'Sum values in a column
                    alSumCol.Add(propProdLandingsMarketPrice)

                    'Sum all values
                    alSumAll.Add(propProdLandingsMarketPrice)
                Next

                'Display the sum of values in a column
                opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
                propSumCol = New cFormulaProperty(CType(opSumCol, cExpression))
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(CType(propSumCol, cProperty))
            Next

            'Display the sum of all values
            opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
            propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
            Me(Me.RowsCount - 1, Me.ColumnsCount - 3) = New PropertyCell(CType(propSumAll, cProperty))

        End Sub

        Private Sub FillTotalCostRow()

            Dim iRow As Integer
            Dim source As cCoreInputOutputBase = Nothing
            Dim propFixedCost As cProperty = Nothing
            Dim propCPUECost As cProperty = Nothing
            Dim propSailCost As cProperty = Nothing

            Dim alSumFixedCPUESailCost As New ArrayList()
            Dim opSumFixedCPUESailCost As cMultiOperation = Nothing
            Dim propSumFixedCPUESailCost As cFormulaProperty = Nothing

            Dim alProdCostValue As New ArrayList()
            Dim opProdCostValue As cMultiOperation = Nothing
            Dim propProdCostValue As cFormulaProperty = Nothing

            Dim alSumCost As New ArrayList
            Dim opSumCost As cMultiOperation = Nothing
            Dim propSumCost As cFormulaProperty = Nothing

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell(SharedResources.HEADER_TOTALCOST, cStyleGuide.eUnitType.Monetary)

            alSumCost.Clear()
            For fleetIndex As Integer = 1 To core.nFleets

                ' Clear the arrayList for the new row
                alSumFixedCPUESailCost.Clear()

                source = core.FleetInputs(fleetIndex)

                'Fixed cost 
                propFixedCost = Me.PropertyManager.GetProperty(source, eVarNameFlags.FixedCost)
                alSumFixedCPUESailCost.Add(propFixedCost)

                'Effort related cost
                propCPUECost = Me.PropertyManager.GetProperty(source, eVarNameFlags.CPUECost)
                alSumFixedCPUESailCost.Add(propCPUECost)

                'Sailing related cost
                propSailCost = Me.PropertyManager.GetProperty(source, eVarNameFlags.SailCost)
                alSumFixedCPUESailCost.Add(propSailCost)

                'Total cost
                opSumFixedCPUESailCost = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumFixedCPUESailCost.ToArray())
                propSumFixedCPUESailCost = New cFormulaProperty(CType(opSumFixedCPUESailCost, cExpression))

                alProdCostValue.Clear()
                alProdCostValue.Add(propSumFixedCPUESailCost)
                alProdCostValue.Add(0.01)
                alProdCostValue.Add(Me(Me.RowsCount - 2, fleetIndex + 1)) 'total value
                opProdCostValue = New cMultiOperation(cMultiOperation.eOperatorType.Multiply, alProdCostValue.ToArray()) 'total cost as a percent of total value
                propProdCostValue = New cFormulaProperty(CType(opProdCostValue, cExpression))
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(propProdCostValue)

                alSumCost.Add(propProdCostValue)
            Next

            opSumCost = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCost.ToArray())
            propSumCost = New cFormulaProperty(CType(opSumCost, cExpression))
            Me(Me.RowsCount - 1, Me.ColumnsCount - 3) = New PropertyCell(propSumCost)
        End Sub

        Private Sub FillTotalProfitRow()

            Dim iRow As Integer
            Dim opMinusValueCost As cBinaryOperation = Nothing
            Dim propMinusValueCost As cFormulaProperty = Nothing
            Dim alSumProfit As New ArrayList()
            Dim opSumProfit As cMultiOperation = Nothing
            Dim propSumProfit As cFormulaProperty = Nothing

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell(SharedResources.HEADER_TOTALPROFIT, cStyleGuide.eUnitType.Monetary)

            alSumProfit.Clear()
            For fleetIndex As Integer = 1 To core.nFleets

                opMinusValueCost = New cBinaryOperation(cBinaryOperation.eOperatorType.Substract, _
                                                CType(Me(Me.RowsCount - 3, fleetIndex + 1), Object), _
                                                CType(Me(Me.RowsCount - 2, fleetIndex + 1), Object))  'total value - total cost
                propMinusValueCost = New cFormulaProperty(CType(opMinusValueCost, cExpression))
                alSumProfit.Add(propMinusValueCost)
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(CType(propMinusValueCost, cProperty))
            Next

            opSumProfit = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumProfit.ToArray())
            propSumProfit = New cFormulaProperty(CType(opSumProfit, cExpression))
            Me(Me.RowsCount - 1, Me.ColumnsCount - 3) = New PropertyCell(propSumProfit)
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
