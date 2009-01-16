'==============================================================================
'
' $Log: FisheryOutputQuantityEwEGrid.vb,v $
' Revision 1.3  2009/01/16 18:30:08  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:53:40  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/06/02 00:01:26  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.11  2008/05/29 22:22:40  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.10  2008/04/07 02:31:06  jeroens
' Cleaning up resources
'
' Revision 1.9  2008/02/13 16:44:29  jeroens
' Renamed resources
'
' Revision 1.8  2008/01/31 17:08:15  jeroens
' Made fleet column headers live updating
'
' Revision 1.7  2007/10/10 02:59:11  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.6  2007/07/05 19:34:32  joeh
' Create columns dynamically
'
' Revision 1.5  2007/07/04 19:14:02  joeh
' Implement Total Catch and Trophic Level
'
' Revision 1.4  2007/06/05 02:45:49  jeroens
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.3  2007/05/05 00:47:25  joeh
' no message
'
' Revision 1.2  2007/05/04 18:15:30  joeh
' Change FleetOuput to FleetInput
'
' Revision 1.1  2007/05/04 17:38:54  joeh
' Change FisheryOutputQualityEwEGrid.vb to FisheryOutputQuantityEwEGrid.vb
'
' Revision 1.4  2007/04/29 03:45:10  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class FisheryOutputQuantityEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub InitStyle()
            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing

            MyBase.InitStyle()

            'Define grid dimensions
            Me.Redim(1, core.nFleets + 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet name
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New PropertyColumnHeaderCell(source, eVarNameFlags.Name)
            Next

            ' Total catch column
            Me(0, core.nFleets + 2) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALCATCH)

            Me.FixedColumns = 2
        End Sub

        Protected Overrides Sub FillData()
            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim iRow As Integer = -1


            'Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If core.nFleets = 0 Then Return

            ''Create rows for all groups and sum quantities in each row
            For rowIndex As Integer = 1 To core.nGroups
                source = core.EcoPathGroupInputs(rowIndex)
                iRow = Me.AddRow()
                FillRows(iRow, source)
            Next rowIndex

            'Create "Total catch" row (sum values in each column)
            FillTotalCatchRow()

            'Create "Trophic level" row
            FillTrophicLevelRow()

        End Sub

        Private Sub FillRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase)
            Dim core As cCore = cCore.GetInstance()
            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
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

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

            ' For each fleet (each column) 
            For fleetIndex As Integer = 1 To core.nFleets
                alSumLandingsDiscards.Clear()
                ' Get the fleet object 
                sourceSec = core.FleetInputs(fleetIndex)
                ' Get the index landing property
                propLandings = pm.GetProperty(sourceSec, eVarNameFlags.Landings, source)
                alSumLandingsDiscards.Add(propLandings)
                ' Get the index discard property
                propDiscards = pm.GetProperty(sourceSec, eVarNameFlags.Discards, source)
                alSumLandingsDiscards.Add(propDiscards)
                ' Set the property to the cell
                opSumLandingsDiscards = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumLandingsDiscards.ToArray())
                propSumLandingsDiscards = New cFormulaProperty(CType(opSumLandingsDiscards, cExpression))
                propCell = New PropertyCell(CType(propSumLandingsDiscards, cProperty))
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
            propSumRow = New cFormulaProperty(CType(opSumRow, cExpression))
            propCell = New PropertyCell(CType(propSumRow, cProperty))
            Me(iRow, Me.ColumnsCount - 1) = propCell

            If blnAllZeroCells = True Then Me.RowsCount = Me.Rows.Count - 1
        End Sub

        Private Sub FillTotalCatchRow()
            Dim iRow As Integer
            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
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
            Me(iRow, 1) = New EwERowHeaderCell(My.Resources.HEADER_TOTALCATCH)

            alSumAll.Clear()
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To core.nGroups
                    sourceSec = core.EcoPathGroupInputs(rowIndex)
                    alSumLandingsDiscards.Clear()
                    ' Get the index landing property
                    propLandings = pm.GetProperty(source, eVarNameFlags.Landings, sourceSec)
                    alSumLandingsDiscards.Add(propLandings)
                    ' Get the index discard property
                    propDiscards = pm.GetProperty(source, eVarNameFlags.Discards, sourceSec)
                    alSumLandingsDiscards.Add(propDiscards)
                    ' Set the property 
                    opSumLandingsDiscards = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumLandingsDiscards.ToArray())
                    propSumLandingsDiscards = New cFormulaProperty(CType(opSumLandingsDiscards, cExpression))

                    'Sum values in a column
                    alSumCol.Add(propSumLandingsDiscards)

                    'Sum all values
                    alSumAll.Add(propSumLandingsDiscards)
                Next

                'Display the sum of values in a column
                opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
                propSumCol = New cFormulaProperty(CType(opSumCol, cExpression))
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(CType(propSumCol, cProperty))
            Next

            'Display the sum of all values
            opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
            propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = New PropertyCell(CType(propSumAll, cProperty))

        End Sub

        Private Sub FillTrophicLevelRow()
            Dim iRow As Integer
            Dim core As cCore = cCore.GetInstance()
            Dim sourceGrpIntput As cCoreInputOutputBase = Nothing
            Dim sourceGrpIntputSec As cCoreInputOutputBase = Nothing
            Dim sourceGrpOutput As cCoreInputOutputBase = Nothing

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
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
            Me(iRow, 1) = New EwERowHeaderCell(My.Resources.HEADER_TROPHICLEVEL)

            alSumQuantityAll.Clear()
            alSumQuantityTTLXAll.Clear()
            For fleetIndex As Integer = 1 To core.nFleets
                sourceGrpIntput = core.FleetInputs(fleetIndex)
                alSumQuantityCol.Clear()
                alSumQuantityTTLXCol.Clear()

                For rowIndex As Integer = 1 To core.nGroups
                    sourceGrpIntputSec = core.EcoPathGroupInputs(rowIndex)
                    sourceGrpOutput = core.EcoPathGroupOutputs(rowIndex)
                    alSumLandingsDiscards.Clear()
                    alProdQuantityTTLX.Clear()
                    ' Get the index landing property
                    propLandings = pm.GetProperty(sourceGrpIntput, eVarNameFlags.Landings, sourceGrpIntputSec)
                    alSumLandingsDiscards.Add(propLandings)
                    ' Get the index discard property
                    propDiscards = pm.GetProperty(sourceGrpIntput, eVarNameFlags.Discards, sourceGrpIntputSec)
                    alSumLandingsDiscards.Add(propDiscards)
                    ' Get the index TTLX property
                    propTTLX = pm.GetProperty(sourceGrpOutput, eVarNameFlags.TTLX)
                    'propCell = New PropertyCell(CType(propTTLX, cProperty))
                    'MsgBox("TTLX" & CStr(propCell.Value), MsgBoxStyle.Information)
                    alProdQuantityTTLX.Add(propTTLX)
                    ' Set the property 
                    opSumLandingsDiscards = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumLandingsDiscards.ToArray())
                    propSumLandingsDiscards = New cFormulaProperty(CType(opSumLandingsDiscards, cExpression))

                    alProdQuantityTTLX.Add(propSumLandingsDiscards)
                    opProdQuantityTTLX = New cMultiOperation(cMultiOperation.eOperatorType.Multiply, alProdQuantityTTLX.ToArray())
                    propProdQuantityTTLX = New cFormulaProperty(CType(opProdQuantityTTLX, cExpression))

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
                propSumQuantityCol = New cFormulaProperty(CType(opSumQuantityCol, cExpression))
                opSumQuantityTTLXCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumQuantityTTLXCol.ToArray())
                propSumQuantityTTLXCol = New cFormulaProperty(CType(opSumQuantityTTLXCol, cExpression))
                opDivTTLXQuantity = New cBinaryOperation(cBinaryOperation.eOperatorType.Divide, _
                                    CType(propSumQuantityTTLXCol, Object), CType(propSumQuantityCol, Object))
                propDivTTLXQuantity = New cFormulaProperty(CType(opDivTTLXQuantity, cExpression))
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(CType(propDivTTLXQuantity, cProperty))
            Next

            'Display (sum of all quantity*TTLX) / (sum of all quantity)
            opSumQuantityAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumQuantityAll.ToArray())
            propSumQuantityAll = New cFormulaProperty(CType(opSumQuantityAll, cExpression))
            opSumQuantityTTLXAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumQuantityTTLXAll.ToArray())
            propSumQuantityTTLXAll = New cFormulaProperty(CType(opSumQuantityTTLXAll, cExpression))
            opDivTTLXQuantity = New cBinaryOperation(cBinaryOperation.eOperatorType.Divide, _
                                CType(propSumQuantityTTLXAll, Object), CType(propSumQuantityAll, Object))
            propDivTTLXQuantity = New cFormulaProperty(CType(opDivTTLXQuantity, cExpression))
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = New PropertyCell(CType(propDivTTLXQuantity, cProperty))

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
