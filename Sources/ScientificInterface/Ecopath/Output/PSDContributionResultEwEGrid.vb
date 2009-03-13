' =============================================================================
'
' $Log: PSDContributionResultEwEGrid.vb,v $
' Revision 1.5  2009/03/13 22:52:37  joeh
' Add code to sum the PSD values of a row and to sum the PSD values in a column
'
' Revision 1.4  2009/03/12 23:51:06  joeh
' Add codes for tabulation of PSD contribution data
'
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
            Dim source As cCoreGroupBase = Nothing
            Dim iRow As Integer = -1

            ' Remove existing rows
            Me.RowsCount = 1

            ' Done?
            'If core.nWeightClasses = 0 Then Return

            ' Create rows for all groups and sum values in each row
            For rowIndex As Integer = 1 To core.nLivingGroups
                source = core.EcoPathGroupOutputs(rowIndex)
                iRow = Me.AddRow()
                FillRows(iRow, source)
            Next rowIndex

            'Create "Sum" row (sum values in each column)
            FillTotalValueRow()

        End Sub

        Private Sub FillRows(ByVal iRow As Integer, ByVal source As cCoreGroupBase)
            Dim core As cCore = cCore.GetInstance()
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim propManager As cPropertyManager = cPropertyManager.GetInstance()
            Dim propPSD As cProperty = Nothing
            Dim propCell As PropertyCell = Nothing

            Dim alSumRow As ArrayList = New ArrayList
            Dim opSumRow As cMultiOperation = Nothing
            Dim propSumRow As cFormulaProperty = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

            alSumRow.Clear()
            ' For each weight class (each column) 
            For wtClassIndex As Integer = 1 To core.nWeightClasses
                sourceSec = core.EcoPathGroupOutputs(wtClassIndex)
                propPSD = propManager.GetProperty(source, eVarNameFlags.PSD, sourceSec)
                propCell = New PropertyCell(CType(propPSD, cProperty))
                ' Configure the cell
                propCell.SuppressZero = True
                ' Set the cell
                Me(iRow, wtClassIndex + 1) = propCell

                'Sum values in a row
                alSumRow.Add(propPSD)
            Next

            'Display the sum of quantities in a row
            opSumRow = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumRow.ToArray())
            propSumRow = New cFormulaProperty(CType(opSumRow, cExpression))
            propCell = New PropertyCell(CType(propSumRow, cProperty))
            propCell.SuppressZero = True
            Me(iRow, Me.ColumnsCount - 1) = propCell
        End Sub

        Private Sub FillTotalValueRow()
            Dim iRow As Integer
            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing

            Dim pmPropertyManager As cPropertyManager = cPropertyManager.GetInstance()
            Dim propPSD As cProperty = Nothing
            '    Dim propMarketPrice As cProperty = Nothing
            '    Dim alProdLandingsMarketPrice As ArrayList = New ArrayList()
            '    Dim opProdLandingsMarketPrice As cMultiOperation = Nothing
            '    Dim propProdLandingsMarketPrice As cFormulaProperty = Nothing

            Dim alSumCol As New ArrayList()
            Dim opSumCol As cMultiOperation = Nothing
            Dim propSumCol As cFormulaProperty = Nothing

            Dim alSumAll As New ArrayList()
            Dim opSumAll As cMultiOperation = Nothing
            Dim propSumAll As cFormulaProperty = Nothing

            Dim propCell As PropertyCell = Nothing

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell("Sum") 'My.Resources.HEADER_TOTALVALUE, StyleGuide.eUnitType.Monetary)

            alSumAll.Clear()
            For wtClassIndex As Integer = 1 To core.nWeightClasses
                source = core.EcoPathGroupOutputs(wtClassIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To core.nLivingGroups
                    sourceSec = core.EcoPathGroupOutputs(rowIndex)
                    ' Get the index PSD property
                    propPSD = pmPropertyManager.GetProperty(sourceSec, eVarNameFlags.PSD, source)

                    'Sum values in a column
                    alSumCol.Add(propPSD)

                    'Sum all values
                    alSumAll.Add(propPSD)
                Next

                'Display the sum of values in a column
                opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
                propSumCol = New cFormulaProperty(CType(opSumCol, cExpression))
                propCell = New PropertyCell(CType(propSumCol, cProperty))
                propCell.SuppressZero = True
                Me(Me.RowsCount - 1, wtClassIndex + 1) = propCell
            Next

            'Display the sum of all values
            opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
            propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
            propCell = New PropertyCell(CType(propSumAll, cProperty))
            propCell.SuppressZero = True
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = propCell

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
