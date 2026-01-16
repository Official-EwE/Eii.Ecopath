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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Output

    
    Public Class gridConsumption
        : Inherits cEwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreGroupBase = Nothing

            Me.Redim(Me.core.nGroups + 3, 2)
            'Set header cells
            Dim iRow As Integer = Me.RowsCount

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To Me.core.nGroups
                source = Me.core.EcopathGroupOutputs(i)
                'Group name row header cell
                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                Me(i, 1) = New cEwERowHeaderCell(source.Name)

                If source.PP < 1 Or source.PP = 2 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If
            Next

            ' Import cell
            Me(iRow - 2, 0) = New cEwERowHeaderCell(CStr(iRow - 2))
            Me(iRow - 2, 1) = New cEwERowHeaderCell(SharedResources.HEADER_IMPORT)

            ' Sum cell
            Me(iRow - 1, 0) = New cEwERowHeaderCell(CStr(iRow - 1))
            Me(iRow - 1, 1) = New cEwERowHeaderCell(SharedResources.HEADER_SUM)

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
            Dim cell As cPropertyCell = Nothing

            Dim visDiagonal As New SourceGrid2.VisualModels.Common
            visDiagonal.BackColor = Color.LightGray
            visDiagonal.TextAlignment = ContentAlignment.MiddleCenter

            Dim columnIndex As Integer = 2
            Dim rowCnt As Integer = Me.RowsCount

            For iPred As Integer = 1 To Me.core.nGroups

                'Get the group output
                source = Me.core.EcopathGroupOutputs(iPred)
                If source.PP < 1 Or source.PP = 2 Then

                    alPropSumAll.Clear()

                    For iPrey As Integer = 1 To Me.core.nGroups
                        ' Get the group output
                        sourceSec = Me.core.EcopathGroupOutputs(iPrey)
                        ' Get the indexed comsumption property by (rowIndex, columnIndex)
                        prop = pm.GetProperty(sourceSec, eVarNameFlags.Consumption, source)
                        cell = New cPropertyCell(prop)

                        If iPrey = iPred Then
                            cell.VisualModel = visDiagonal
                        End If

                        ' Add property to the cell
                        Me(iPrey, columnIndex) = cell
                        ' Add the property to ArrayList for the sum cell
                        alPropSumAll.Add(prop)
                    Next

                    prop = pm.GetProperty(source, eVarNameFlags.ImportedConsumption)
                    ' Get the Comsumption import property
                    Me(rowCnt - 2, columnIndex) = New cPropertyCell(prop)
                    alPropSumAll.Add(prop)

                    ' Now create the formula property that will calculate the sum of all Consumption props
                    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alPropSumAll.ToArray())
                    propSum = Me.Formula(opSumAll)

                    Me(rowCnt - 1, columnIndex) = New cPropertyCell(propSum)

                    columnIndex = columnIndex + 1

                End If
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property

    End Class

End Namespace
