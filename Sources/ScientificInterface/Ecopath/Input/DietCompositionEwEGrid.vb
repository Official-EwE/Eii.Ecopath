'==============================================================================
'
' $Log: DietCompositionEwEGrid.vb,v $
' Revision 1.2  2008/12/15 15:54:29  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:31  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.31  2008/08/02 03:04:12  jeroens
' Renamed resources
'
' Revision 1.30  2008/07/29 13:06:44  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.29  2008/07/21 23:48:40  jeroens
' Simplified cell construction
'
' Revision 1.28  2008/06/02 00:01:28  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.27  2008/05/29 22:22:41  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.26  2008/04/07 02:31:08  jeroens
' Cleaning up resources
'
' Revision 1.25  2007/10/10 02:59:13  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.24  2007/07/03 07:12:09  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.23  2007/06/21 23:57:21  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.22  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.21  2007/06/05 14:00:10  jeroens
' - Removed directcasts
'
' Revision 1.20  2007/06/05 02:45:49  jeroens
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.19  2007/05/10 00:07:10  jeroens
' * Last group row correctly displayed
'
' Revision 1.18  2007/04/29 03:45:11  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.17  2007/02/16 00:15:21  fgao
' Ongoing ecospace
'
' Revision 1.16  2006/11/19 04:26:45  jeroens
' + Fixed diagonal text alignment issue
'
' Revision 1.15  2006/10/25 23:35:01  fgao
' Add cell merge
'
' Revision 1.14  2006/10/25 18:47:46  fgao
' Using individual visualizer to color the cells in the diagonal line.
'
' Revision 1.13  2006/10/20 23:20:55  fgao
' Add row and column indexes and selection.
'
' Revision 1.12  2006/10/03 03:19:37  jeroens
' + Formula properties constructed with unique IDs
'
' Revision 1.11  2006/09/30 03:51:08  jeroens
' + Neatified
'
' Revision 1.10  2006/09/29 21:16:40  sherman
' Added (1-sum) row to bottom
'
' Revision 1.9  2006/09/21 01:00:24  jeroens
' * Updated to cCoreGroupBase
'
' Revision 1.8  2006/08/15 15:40:29  jeroens
' * Fixed spelling error
'
' Revision 1.7  2006/07/06 15:53:01  jeroens
' + Implemented Cell.SuppressZero for DC value cells to increase grid legibility
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    <CLSCompliant(False)> _
    Public Class DietCompositionEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing

            ' Define grid dimensions
            Me.Redim(core.nGroups + 4, 2)

            Dim rowCnt As Integer = Me.RowsCount
            ' Set header cells
            ' # (0,0)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To core.nGroups
                source = core.EcoPathGroupInputs(i)
                ' Group index header cell
                Me(i, 0) = New EwERowHeaderCell(i)
                ' # Group name row header cells
                Me(i, 1) = New EwERowHeaderCell(source.Name)
                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    ' # Group name column header cells
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If
            Next

            ' # DietImport header cell
            Me(rowCnt - 3, 0) = New EwERowHeaderCell(rowCnt - 3)
            Me(rowCnt - 3, 1) = New EwERowHeaderCell(My.Resources.HEADER_IMPORT)

            ' # Sum header cell
            Me(rowCnt - 2, 0) = New EwERowHeaderCell(rowCnt - 2)
            Me(rowCnt - 2, 1) = New EwERowHeaderCell(My.Resources.HEADER_SUM)

            ' # Sum - 1 header cell
            Me(rowCnt - 1, 0) = New EwERowHeaderCell(rowCnt - 1)
            Me(rowCnt - 1, 1) = New EwERowHeaderCell(My.Resources.HEADER_1_MINUS_SUM)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            Dim prop As cProperty = Nothing
            Dim propSum As cFormulaProperty = Nothing
            Dim prop1MinusSum As cFormulaProperty = Nothing
            Dim opSumAll As cMultiOperation = Nothing
            Dim op1MinusSumAll As cBinaryOperation = Nothing
            Dim alPropSumAll As New ArrayList()
            Dim cell As PropertyCell = Nothing

            Dim visDiagonal As New SourceGrid2.VisualModels.Common
            visDiagonal.BackColor = Color.LightGray
            visDiagonal.TextAlignment = ContentAlignment.MiddleCenter

            ' Populate grid data cells
            Dim columnIndex As Integer = 2
            Dim rowCnt As Integer = Me.RowsCount
            ' For each column
            For groupIndex As Integer = 1 To core.nLivingGroups
                ' Get the group
                source = core.EcoPathGroupInputs(groupIndex)

                If source.PP < 1 Then

                    ' Prepare for collection new range of properties to sum
                    alPropSumAll.Clear()

                    ' For each row
                    For rowIndex As Integer = 1 To core.nGroups
                        ' Get index group
                        sourceSec = core.EcoPathGroupInputs(rowIndex)

                        ' Get the indexed dietcomp property
                        prop = pm.GetProperty(source, eVarNameFlags.DietComp, sourceSec)
                        ' Add property to destined cell
                        cell = New PropertyCell(prop)

                        If rowIndex = columnIndex - 1 Then
                            cell.VisualModel = visDiagonal
                        End If

                        ' DC value cells suppress zeroes to increase legibility of the grid
                        cell.SuppressZero = True
                        ' Activate the cell
                        Me(rowIndex, columnIndex) = cell
                        ' Add this property to the list of props to sum
                        alPropSumAll.Add(prop)

                    Next rowIndex

                    ' Define DietImport cell
                    ' # Get the property
                    prop = pm.GetProperty(source, eVarNameFlags.ImpDiet)
                    ' # Add to cell
                    Me(rowCnt - 3, columnIndex) = New PropertyCell(prop)
                    ' Add this property to the list of props to sum
                    alPropSumAll.Add(prop)

                    ' Now create the formula property that will calculate the sum of all DietComp props
                    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alPropSumAll.ToArray())
                    ' Create Sum property for the SUM row (all the way at the bottom)
                    propSum = New cFormulaProperty(cValueID.Generate(source.getID, "DCSum"), opSumAll)
                    ' Define sum cell
                    Me(rowCnt - 2, columnIndex) = New PropertyCell(propSum)

                    ' Create 1-Sum property for the SUM row (all the way at the bottom)
                    op1MinusSumAll = New cBinaryOperation(cBinaryOperation.eOperatorType.Substract, 1, propSum)
                    prop1MinusSum = New cFormulaProperty(cValueID.Generate(source.getID, "DCInvSum"), op1MinusSumAll)
                    ' Define sum cell
                    Me(rowCnt - 1, columnIndex) = New PropertyCell(prop1MinusSum)

                    ' Next column
                    columnIndex = columnIndex + 1

                End If

            Next groupIndex

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

    End Class

End Namespace
