' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Option Explicit On

Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources



Namespace Ecosim

    
    Public Class gridEcosimArenaShare
        Inherits cEwEGrid

        Private m_man As cEcosimArenaManager = Nothing

        Public Sub New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Property UIContext As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
                If (Me.UIContext IsNot Nothing) Then
                    Me.m_man = Nothing
                End If
                MyBase.UIContext = value
                If (Me.UIContext IsNot Nothing) Then
                    Me.m_man = value.Core.EcosimArenaManager
                End If
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return

            Dim n As Integer = 0

            If (Me.SelectedGroup IsNot Nothing) Then
                Dim arenas As cEcosimArena() = Me.m_man.Arenas(Me.SelectedGroup.Index)
                n = arenas.Count() + 1
            End If

            ' Also account for 'sum to one' row
            Me.Redim(n + 1, n)

            Me.FixedColumns = 1

        End Sub

        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.SelectedGroup Is Nothing) Then Return

            Dim fmt As New cCoreInterfaceFormatter()
            Dim iPrey As Integer = Me.SelectedGroup.Index
            Dim arenas As cEcosimArena() = Me.m_man.Arenas(Me.SelectedGroup.Index)

            Me(0, 0) = New cEwEColumnHeaderCell("")

            For col As Integer = 1 To Me.ColumnsCount - 1
                Dim ar As cEcosimArena = arenas(col - 1)
                Dim iPred As Integer = ar.Pred
                Dim pred As cEcosimGroupInput = Me.Core.EcosimGroupInputs(iPred)
                Me(0, col) = New cPropertyColumnHeaderCell(Me.PropertyManager, pred, eVarNameFlags.Index)
            Next

            For row As Integer = 1 To Me.RowsCount - 2
                Dim ar As cEcosimArena = arenas(row - 1)
                Dim i As Integer = ar.Index
                Me(row, 0) = New cEwERowHeaderCell(cStringUtils.Localize(My.Resources.ECOSIM_APPLYARENA_HEADER, i))
            Next
            Me(Me.RowsCount - 1, 0) = New cEwERowHeaderCell(SharedResources.HEADER_SUM)

            For col As Integer = 1 To Me.ColumnsCount - 1
                Dim props As New List(Of cProperty)
                For row As Integer = 1 To Me.RowsCount - 2
                    Dim ar As cEcosimArena = arenas(row - 1)
                    Me(row, 0).Tag = ar
                    Dim ar2 As cEcosimArena = arenas(col - 1)
                    Dim pred As cEcosimGroupInput = Me.Core.EcosimGroupInputs(ar2.Pred)
                    Dim prop As cProperty = Me.PropertyManager.GetProperty(ar, eVarNameFlags.EcosimArenaShare, pred)
                    Dim cell As New cPropertyCell(prop)
                    cell.SuppressZero = True
                    Me(row, col) = cell

                    props.Add(prop)
                Next

                Dim op As New cMultiOperation(cMultiOperation.eOperatorType.Sum, props.ToArray())
                Dim sum As cFormulaProperty = Me.Formula(op)
                Me(Me.RowsCount - 1, col) = New cPropertyCell(sum)
            Next

            Me.Columns(0).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize

        End Sub

        Public Property SelectedGroup As cEcoPathGroupInput

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecosim
            End Get
        End Property

    End Class

End Namespace
