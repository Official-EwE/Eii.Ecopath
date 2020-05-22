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

#End Region ' Imports

Namespace Ecosim

    <CLSCompliant(False)>
    Public Class gridEcosimArenaShare
        Inherits EwEGrid

        Private m_manager As cEcosimArenaManager = Nothing
        Private Const ShowPredPreyHeader As Boolean = False

        Public Sub New()
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim fmt As New cCoreInterfaceFormatter()
            Me.m_manager = Me.Core.EcosimArenaManager
            Dim n As Integer = If(ShowPredPreyHeader, 2, 0)

            ' ToDo: globalize this

            Me.Redim(1 + Me.m_manager.NumArenas, 1 + n + Me.Core.nGroups)
            If (ShowPredPreyHeader) Then
                Me(0, 0) = New EwEColumnHeaderCell("")
                Me(0, 1) = New EwEColumnHeaderCell("Arena prey")
                Me(0, 2) = New EwEColumnHeaderCell("Arena pred")
            Else
                Me(0, 0) = New EwEColumnHeaderCell("Arena")
            End If
            For j As Integer = 1 To Me.Core.nLivingGroups
                Me(0, n + j) = New EwEColumnHeaderCell(CStr(j))
            Next
            For k As Integer = 1 To Me.m_manager.NumArenas
                Dim arena As cEcosimArenaShare = Me.m_manager.Arena(k)
                Me(k, 0) = New EwERowHeaderCell(CStr(arena.iArena))
                If (ShowPredPreyHeader) Then
                    Me(k, 1) = New EwERowHeaderCell(fmt.ToString(Core.EcoPathGroupInputs(arena.Prey)))
                    Me(k, 2) = New EwERowHeaderCell(fmt.ToString(Core.EcoPathGroupInputs(arena.Pred)))
                End If
                For j As Integer = 1 To Me.Core.nLivingGroups
                    Dim cell As New PropertyCell(Me.PropertyManager, arena, eVarNameFlags.EcosimArenaShare, Core.EcoPathGroupInputs(j))
                    cell.SuppressZero = True
                    Me(k, n + j) = cell
                Next
            Next

        End Sub

        Protected Overrides Sub FillData()
            ' NOP
        End Sub
    End Class

End Namespace
