#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class ConsumptionEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim source As cCoreGroupBase = Nothing

            Me.Redim(core.nGroups + 3, 2)
            'Set header cells
            Dim rowCnt As Integer = Me.RowsCount

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To core.nGroups
                source = core.EcoPathGroupOutputs(i)
                'Group name row header cell
                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 1) = New EwERowHeaderCell(source.Name)

                If source.PP < 1 Or source.PP = 2 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If
            Next

            ' Import cell
            Me(rowCnt - 2, 0) = New EwERowHeaderCell(rowCnt - 2)
            Me(rowCnt - 2, 1) = New EwERowHeaderCell(My.Resources.HEADER_IMPORT)

            ' Sum cell
            Me(rowCnt - 1, 0) = New EwERowHeaderCell(rowCnt - 1)
            Me(rowCnt - 1, 1) = New EwERowHeaderCell(My.Resources.HEADER_SUM)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing

            ' Variable used for sum cells
            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = Me.PropertyManager
            Dim alPropSumAll As New ArrayList()
            Dim propSum As cFormulaProperty = Nothing
            Dim opSumAll As cMultiOperation = Nothing
            Dim cell As PropertyCell = Nothing

            Dim visDiagonal As New SourceGrid2.VisualModels.Common
            visDiagonal.BackColor = Color.LightGray
            visDiagonal.TextAlignment = ContentAlignment.MiddleCenter

            Dim columnIndex As Integer = 2
            Dim rowCnt As Integer = Me.RowsCount

            For groupIndex As Integer = 1 To core.nGroups

                'Get the group output
                source = core.EcoPathGroupOutputs(groupIndex)
                If source.PP < 1 Or source.PP = 2 Then

                    alPropSumAll.Clear()

                    For rowIndex As Integer = 1 To core.nGroups
                        ' Get the group output
                        sourceSec = core.EcoPathGroupOutputs(rowIndex)
                        ' Get the indexed comsumption property by (rowIndex, columnIndex)
                        prop = pm.GetProperty(sourceSec, eVarNameFlags.Consumption, source)
                        cell = New PropertyCell(prop)

                        If rowIndex = columnIndex - 1 Then
                            cell.VisualModel = visDiagonal
                        End If

                        ' Add property to the cell
                        Me(rowIndex, columnIndex) = cell
                        ' Add the property to ArrayList for the sum cell
                        alPropSumAll.Add(prop)
                    Next

                    prop = pm.GetProperty(source, eVarNameFlags.ImportedConsumption)
                    ' Get the Comsumption import property
                    Me(rowCnt - 2, columnIndex) = New PropertyCell(prop)
                    alPropSumAll.Add(prop)

                    ' Now create the formula property that will calculate the sum of all Consumption props
                    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alPropSumAll.ToArray())
                    propSum = New cFormulaProperty(CType(opSumAll, cExpression))

                    Me(rowCnt - 1, columnIndex) = New PropertyCell(CType(propSum, cProperty))

                    columnIndex = columnIndex + 1

                End If
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
