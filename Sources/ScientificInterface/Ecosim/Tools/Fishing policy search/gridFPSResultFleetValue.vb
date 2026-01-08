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
Imports EwECore.FishingPolicy
Imports EwECore.Common
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper grid for Fishing Policy Search interface, displaying ...
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
     Public Class gridFPSResultFleetValue
        : Inherits cEwEGrid

        Private m_FPManager As cFishingPolicyManager

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
                MyBase.UIContext = value
                If value IsNot Nothing Then
                    Me.m_FPManager = Me.Core.FishingPolicyManager
                End If
            End Set
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(Me.Core.nFleets + 1, Me.Core.nFleets + 3)
            Me(0, 0) = New cEwEColumnHeaderCell(My.Resources.FPS_FV_RESULT_COL0)
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_INCOME)
            Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_PROFIT)

            For i As Integer = 1 To Me.Core.nFleets
                Me(i, 0) = New cPropertyRowHeaderCell(Me.PropertyManager, Me.Core.EcopathFleetInputs(i), eVarNameFlags.Name)
                Me(0, i + 2) = New cPropertyColumnHeaderCell(Me.PropertyManager, Me.Core.EcopathFleetInputs(i), eVarNameFlags.Name)
            Next

        End Sub

        Protected Overrides Sub FillData()

        End Sub

        Public Sub InsertOneIterResult(ByRef results As cFPSSearchResults)

            Debug.Assert(Me.m_FPManager IsNot Nothing)
            Debug.Assert(Me.UIContext IsNot Nothing)

            For i As Integer = 1 To Me.Core.nFleets
                Me(i, 1) = New Cell(results.Income(i).ToString)
                Me(i, 2) = New Cell(results.Profitability(i).ToString)

                For j As Integer = 1 To Me.Core.nFleets
                    Me(i, j + 2) = New Cell(results.CompensationMatrix(i, j).ToString)
                Next
            Next
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
        End Sub

    End Class

End Namespace
