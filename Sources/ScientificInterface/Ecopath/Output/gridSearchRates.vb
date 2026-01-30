' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Output


    Public Class gridSearchRates
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

            MyBase.InitStyle()
            Dim source As cCoreGroupBase = Nothing

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(Me.Core.nGroups + 1, 2)
            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To Me.Core.nGroups
                ' Column displays mixed consumer/producer groups ( PP < 1)
                source = Me.Core.EcopathGroupOutputs(i)
                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                ' # Group name row header cells
                Me(i, 1) = New cEwERowHeaderCell(source.Name)

                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If

            Next
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = Me.PropertyManager
            Dim cell As cPropertyCell = Nothing
            Dim columnIndex As Integer = 2

            Dim visDiagonal As New SourceGrid2.VisualModels.Common
            visDiagonal.BackColor = Color.LightGray
            visDiagonal.TextAlignment = ContentAlignment.MiddleCenter

            For groupIndex As Integer = 1 To Me.Core.nGroups

                'Get the group output
                source = Me.Core.EcopathGroupOutputs(groupIndex)
                If source.PP < 1 Then
                    For rowIndex As Integer = 1 To Me.Core.nGroups
                        ' Get the group output
                        sourceSec = Me.Core.EcopathGroupOutputs(rowIndex)
                        ' Get the indexed comsumption property by (rowIndex, columnIndex)
                        prop = pm.GetProperty(sourceSec, eVarNameFlags.SearchRate, source)
                        ' Add property to the cell
                        cell = New cPropertyCell(prop)

                        If rowIndex = columnIndex - 1 Then
                            cell.VisualModel = visDiagonal
                        End If

                        ' Config cell
                        cell.SuppressZero = True
                        ' Plug cell into grid
                        Me(rowIndex, columnIndex) = cell
                    Next
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
