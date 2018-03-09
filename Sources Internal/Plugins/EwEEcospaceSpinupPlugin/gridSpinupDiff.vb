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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

<CLSCompliant(False)>
Public Class gridSpinupDiff
    Inherits EwEGrid

    Private Const cRowHeaders As Integer = 2
    Private m_Plugin As cEcospaceSpinupPlugin

    Private Enum eColumnTypes As Integer
        Index = 0
        Name
        B0
        Bt
        BtRel
        BtDiff
    End Enum

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub Init(SpinUpPlugin As cEcospaceSpinupPlugin)
        Me.m_Plugin = SpinUpPlugin
    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        ' ToDo: globalize this
        If (Me.UIContext Is Nothing) Then Return

        Me.Redim(Me.Core.nGroups + cRowHeaders, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

        'Column headers
        Dim headercell As EwEColumnHeaderCell

        headercell = New EwEColumnHeaderCell("B(0)")
        Me(0, eColumnTypes.B0) = headercell
        ' headercell.ToolTipText = "=sumof(log(B(t)/B(0))^2)"

        headercell = New EwEColumnHeaderCell("B(t)")
        Me(0, eColumnTypes.Bt) = headercell
        ' headercell.ToolTipText = "=(B(t)-B(0))/B(0)"

        Me(0, eColumnTypes.BtRel) = New EwEColumnHeaderCell("B(t)/B(0)")
        Me(0, eColumnTypes.BtDiff) = New EwEColumnHeaderCell("B(t)/B(t-1)")

        For iGroup As Integer = 0 To Me.Core.nGroups
            Dim iRow As Integer = iGroup + cRowHeaders - 1
            If (iGroup = 0) Then
                Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell("")
                Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(SharedResources.GENERIC_VALUE_ALLGROUPS)
            Else
                Dim grp As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iGroup)
                Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iGroup))
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, grp, eVarNameFlags.Name)
            End If

            Me(iRow, eColumnTypes.B0) = New EwECell(0.0, GetType(Single))
            Me(iRow, eColumnTypes.Bt) = New EwECell(0.0, GetType(Single))
            Me(iRow, eColumnTypes.BtRel) = New EwECell(0.0, GetType(Single))
            Me(iRow, eColumnTypes.BtDiff) = New EwECell(0.0, GetType(Single))
        Next

        Me.FixedColumns = 1
        Me.FixedRows = cRowHeaders

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        'Me.FixedColumnWidths = False
    End Sub

    Public Sub OnTimeStep()
        Try
            Me.FillData()
        Catch ex As Exception

        End Try
    End Sub

    Protected Overrides Sub FillData()

        If (Me.m_Plugin Is Nothing) Then Return
        If (Me.UIContext Is Nothing) Then Return
        Return

        For iGroup As Integer = 0 To Me.Core.nGroups
            Dim iRow As Integer = iGroup + cRowHeaders - 1
            DirectCast(Me(iRow, eColumnTypes.B0), EwECell).Value = Me.m_Plugin.BioAtBase(iGroup)

            DirectCast(Me(iRow, eColumnTypes.Bt), EwECell).Value = Me.m_Plugin.BioAtTime(iGroup)
            DirectCast(Me(iRow, eColumnTypes.BtRel), EwECell).Value = Me.m_Plugin.BtB0(iGroup)
            DirectCast(Me(iRow, eColumnTypes.BtDiff), EwECell).Value = Me.m_Plugin.BtBtMinus1(iGroup)
        Next

    End Sub


End Class
