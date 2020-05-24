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

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecosim

    <CLSCompliant(False)>
    Public Class gridEcosimArenaShare
        Inherits EwEGrid

        Public Sub New()
        End Sub

        Public Property Data As cEcosimAreaViewData = Nothing

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.Data Is Nothing) Then Return

            Dim n As Integer = 0

            If (Me.SelectedGroup IsNot Nothing) Then
                n = Me.Data.GroupArenaNo(Me.SelectedGroup.Index) + 1
            End If
            Me.Redim(n, n)

        End Sub

        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.Data Is Nothing) Then Return
            If (Me.SelectedGroup Is Nothing) Then Return

            Dim fmt As New cCoreInterfaceFormatter()
            Dim man As cEcosimArenaManager = Me.Data.Manager
            Dim iPrey As Integer = Me.SelectedGroup.Index

            Me(0, 0) = New EwEColumnHeaderCell("")

            For col As Integer = 1 To Me.ColumnsCount - 1
                Dim iar As Integer = Me.Data.Arena1(iPrey) + col - 1
                Dim iPred As Integer = man.JArena(iar)
                Dim pred As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iPred)
                Me(0, col) = New PropertyColumnHeaderCell(Me.PropertyManager, pred, eVarNameFlags.Index)
            Next

            For row As Integer = 1 To Me.RowsCount - 1
                Dim i As Integer = man.ArenaNo(iPrey, man.JArena(Me.Data.Arena1(iPrey) + row - 1))
                Me(row, 0) = New EwERowHeaderCell(cStringUtils.Localize(My.Resources.ECOSIM_APPLYARENA_HEADER, i))
            Next

            For row As Integer = 1 To Me.RowsCount - 1
                For col As Integer = 1 To Me.ColumnsCount - 1
                    Dim iar As Integer = Me.Data.Arena1(iPrey) + row - 1
                    Dim ipred As Integer = man.JArena(Me.Data.Arena1(iPrey) + col - 1)
                    Dim arena As cEcosimArenaShare = man.Arena(iar)
                    Dim pred As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(ipred)

                    ' Somehow property cells cause a kaboom. Need to check
                    Dim cell As New PropertyCell(Me.PropertyManager, arena, eVarNameFlags.EcosimArenaShare, pred)
                    cell.SuppressZero = True
                    Me(row, col) = cell
                Next
            Next

            Me.Columns(0).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize

        End Sub

        Public Property SelectedGroup As cEcoPathGroupInput

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoSim
            End Get
        End Property

    End Class

End Namespace
