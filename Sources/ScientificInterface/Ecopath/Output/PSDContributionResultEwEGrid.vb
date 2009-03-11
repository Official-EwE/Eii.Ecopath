' =============================================================================
'
' $Log: PSDContributionResultEwEGrid.vb,v $
' Revision 1.3  2009/03/11 00:14:28  joeh
' Add PSD calculation
'
' Revision 1.2  2009/02/21 00:23:07  jeroens
' Added headers
'
' =============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class PSDContributionResult
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()
            Dim core As cCore = cCore.GetInstance()

            MyBase.InitStyle()

            'Define grid dimensions
            Me.Redim(1, core.nWeightClasses + 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAMEWEIGHT_UNIT)

            ' Dynamic column header - weight class
            For wtClassIndex As Integer = 1 To core.nWeightClasses
                Me(0, wtClassIndex + 1) = New EwEColumnHeaderCell((core.FirstWeightClass * 2 ^ (wtClassIndex - 1)).ToString)
            Next

            ' Sum value column
            Me(0, core.nWeightClasses + 2) = New EwEColumnHeaderCell(My.Resources.HEADER_SUM)

            Me.FixedColumns = 2
        End Sub

        Protected Overrides Sub FillData()
            Dim core As cCore = cCore.GetInstance()
            Dim source As cEcoPathGroupInput = Nothing
            Dim iRow As Integer = -1

            ' Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If core.nWeightClasses = 0 Then Return

            ' Create rows for all groups and sum values in each row
            For rowIndex As Integer = 1 To core.nLivingGroups
                source = core.EcoPathGroupInputs(rowIndex)
                iRow = Me.AddRow()
                FillRows(iRow, source)
            Next rowIndex

            'Create "Sum" row (sum values in each column)
            'FillTotalValueRow()

        End Sub

        Private Sub FillRows(ByVal iRow As Integer, ByVal source As cEcoPathGroupInput)
            Dim core As cCore = cCore.GetInstance()
            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim propManager As cPropertyManager = cPropertyManager.GetInstance()
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
            Dim opSumRow As cMultiOperation = Nothing
            Dim propSumRow As cFormulaProperty = Nothing
            Dim opSumMarketValues As cMultiOperation = Nothing
            Dim propSumMarketValues As cFormulaProperty = Nothing

            ' Total value
            Dim opTotalValue As cBinaryOperation = Nothing
            Dim propTotalValue As cFormulaProperty = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

            alSumRow.Clear()
            ' For each fleet (each column) 
            For wtClassIndex As Integer = 1 To core.nWeightClasses
                alProdLandingsMarketPrice.Clear()
                ' Get the fleet object 
                sourceSec = core.FleetInputs(1) 'fleetIndex)
                ' Get the index landing property
                propLandings = propManager.GetProperty(sourceSec, eVarNameFlags.Landings, source)
                alProdLandingsMarketPrice.Add(propLandings)
                ' Get the index market price property
                propMarketPrice = propManager.GetProperty(sourceSec, eVarNameFlags.OffVesselPrice, source)
                alProdLandingsMarketPrice.Add(propMarketPrice)
                ' Set the property to the cell
                opProdLandingsMarketPrice = New cMultiOperation(cMultiOperation.eOperatorType.Multiply, alProdLandingsMarketPrice.ToArray())
                propProdLandingsMarketPrice = New cFormulaProperty(CType(opProdLandingsMarketPrice, cExpression))
                propCell = New PropertyCell(CType(propProdLandingsMarketPrice, cProperty))
                ' Configure the cell
                propCell.SuppressZero = True
                propCell.Value = 0
                ' Set the cell
                Me(iRow, wtClassIndex + 1) = propCell

                'Sum values in a row
                alSumRow.Add(propProdLandingsMarketPrice)
            Next

            'Display the sum of quantities in a row
            opSumRow = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumRow.ToArray())
            propSumRow = New cFormulaProperty(CType(opSumRow, cExpression))
            propCell = New PropertyCell(CType(propSumRow, cProperty))
            Me(iRow, Me.ColumnsCount - 1) = propCell
        End Sub

        'Private Sub FillTotalValueRow()
        '    Dim iRow As Integer
        '    Dim core As cCore = cCore.GetInstance()
        '    Dim source As cCoreInputOutputBase = Nothing
        '    Dim sourceSec As cCoreInputOutputBase = Nothing

        '    Dim pm As cPropertyManager = cPropertyManager.GetInstance()
        '    Dim propLandings As cProperty = Nothing
        '    Dim propMarketPrice As cProperty = Nothing
        '    Dim alProdLandingsMarketPrice As ArrayList = New ArrayList()
        '    Dim opProdLandingsMarketPrice As cMultiOperation = Nothing
        '    Dim propProdLandingsMarketPrice As cFormulaProperty = Nothing

        '    Dim alSumCol As New ArrayList()
        '    Dim opSumCol As cMultiOperation = Nothing
        '    Dim propSumCol As cFormulaProperty = Nothing

        '    Dim alSumAll As New ArrayList()
        '    Dim opSumAll As cMultiOperation = Nothing
        '    Dim propSumAll As cFormulaProperty = Nothing

        '    Dim propCell As PropertyCell = Nothing

        '    iRow = Me.AddRow()
        '    Me(iRow, 0) = New EwERowHeaderCell("")
        '    Me(iRow, 1) = New EwERowHeaderCell("Sum") 'My.Resources.HEADER_TOTALVALUE, StyleGuide.eUnitType.Monetary)

        '    alSumAll.Clear()
        '    For fleetIndex As Integer = 1 To core.nWeightClasses
        '        source = core.FleetInputs(1) 'fleetIndex)
        '        alSumCol.Clear()

        '        For rowIndex As Integer = 1 To core.nLivingGroups
        '            sourceSec = core.EcoPathGroupInputs(rowIndex)
        '            alProdLandingsMarketPrice.Clear()
        '            ' Get the index landing property
        '            propLandings = pm.GetProperty(source, eVarNameFlags.Landings, sourceSec)
        '            alProdLandingsMarketPrice.Add(propLandings)
        '            ' Get the index market price property
        '            propMarketPrice = pm.GetProperty(source, eVarNameFlags.OffVesselPrice, sourceSec)
        '            alProdLandingsMarketPrice.Add(propMarketPrice)
        '            ' Set the property 
        '            opProdLandingsMarketPrice = New cMultiOperation(cMultiOperation.eOperatorType.Multiply, alProdLandingsMarketPrice.ToArray())
        '            propProdLandingsMarketPrice = New cFormulaProperty(CType(opProdLandingsMarketPrice, cExpression))

        '            'Sum values in a column
        '            alSumCol.Add(propProdLandingsMarketPrice)

        '            'Sum all values
        '            alSumAll.Add(propProdLandingsMarketPrice)
        '        Next

        '        'Display the sum of values in a column
        '        opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
        '        propSumCol = New cFormulaProperty(CType(opSumCol, cExpression))
        '        propCell = New PropertyCell(CType(propSumCol, cProperty))
        '        propCell.Value = 0
        '        Me(Me.RowsCount - 1, fleetIndex + 1) = propCell
        '    Next

        '    'Display the sum of all values
        '    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
        '    propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
        '    propCell = New PropertyCell(CType(propSumAll, cProperty))
        '    propCell.Value = 0
        '    Me(Me.RowsCount - 1, Me.ColumnsCount - 3) = propCell

        'End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
