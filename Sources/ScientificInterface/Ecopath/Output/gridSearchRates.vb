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

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class gridSearchRates
        Inherits cEwEGrid

        Public Sub New()
            MyBase.new()
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

            For groupIndex As Integer = 1 To Me.core.nGroups

                'Get the group output
                source = Me.core.EcopathGroupOutputs(groupIndex)
                If source.PP < 1 Then
                    For rowIndex As Integer = 1 To Me.core.nGroups
                        ' Get the group output
                        sourceSec = Me.core.EcopathGroupOutputs(rowIndex)
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
