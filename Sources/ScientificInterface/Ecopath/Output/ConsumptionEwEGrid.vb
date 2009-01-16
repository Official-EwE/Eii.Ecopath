'==============================================================================
'
' $Log: ConsumptionEwEGrid.vb,v $
' Revision 1.3  2009/01/16 18:30:08  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:58:24  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/08/02 03:04:11  jeroens
' Renamed resources
'
' Revision 1.15  2008/07/29 13:06:43  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.14  2008/07/21 23:48:40  jeroens
' Simplified cell construction
'
' Revision 1.13  2008/06/02 00:01:25  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.12  2008/05/29 22:22:39  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.11  2008/04/07 02:31:05  jeroens
' Cleaning up resources
'
' Revision 1.10  2007/10/10 02:59:11  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.9  2007/07/03 07:08:45  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.8  2007/06/21 23:57:20  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.7  2007/06/05 02:45:49  jeroens
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.6  2007/04/29 03:45:09  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.5  2006/08/20 02:07:49  jeroens
' + Added header
'
'==============================================================================

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

            Dim core As cCore = cCore.GetInstance()
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
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
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

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing

            ' Variable used for sum cells
            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
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

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
