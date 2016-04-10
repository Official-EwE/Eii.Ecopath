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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2.Cells.Real

<CLSCompliant(False)> _
Public Class gridSpinupDiff

    Private m_nRowHeaders As Integer

    Private m_Plugin As cEcospaceSpinupPlugin

    Public Sub Init(SpinUpPlugin As cEcospaceSpinupPlugin)
        m_Plugin = SpinUpPlugin
    End Sub

    Public Sub New()
        MyBase.new()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Try

            If (Me.UIContext Is Nothing) Then Return

            m_nRowHeaders = 2
            'Define grid dimensions
            ' Me.Redim(1, Me.Core.nGroups + m_nFixed)
            Me.Redim(Me.Core.nGroups + m_nRowHeaders, 5)

            Me.FixedColumns = 1
            Me.FixedRows = m_nRowHeaders

            'Define row headers
            Me(0, 0) = New EwERowHeaderCell("Groups")

            'Column headers
            Dim headercell As EwEColumnHeaderCell

            headercell = New EwEColumnHeaderCell("B(0)")
            ' headercell.ToolTipText = "=sumof(log(B(t)/B(0))^2)"
            Me(0, 1) = headercell

            headercell = New EwEColumnHeaderCell("B(t)")
            ' headercell.ToolTipText = "=(B(t)-B(0))/B(0)"
            Me(0, 2) = headercell

            Me(0, 3) = New EwEColumnHeaderCell("B(t)/B(0)")
            Me(0, 4) = New EwEColumnHeaderCell("B(t)/B(t-1)")

            For igrp As Integer = 0 To Me.Core.nGroups
                Dim irow As Integer = igrp + m_nRowHeaders - 1
                If igrp = 0 Then
                    Me(irow, 0) = New EwERowHeaderCell("All Groups")
                Else
                    Me(irow, 0) = New EwERowHeaderCell(Me.Core.EcoPathGroupInputs(igrp).Name)
                End If

                Me(irow, 1) = New EwECell(0.0, GetType(Double))
                Me(irow, 2) = New EwECell(0.0, GetType(Double))
                Me(irow, 3) = New EwECell(0.0, GetType(Double))
                Me(irow, 4) = New EwECell(0.0, GetType(Double))
            Next

        Catch ex As Exception

        End Try

    End Sub

    Public Sub OnTimeStep()
        Try
            Me.FillData()
        Catch ex As Exception

        End Try
    End Sub


    Protected Overrides Sub FillData()

        If Me.m_Plugin Is Nothing Then Return
        If (Me.UIContext Is Nothing) Then Return

        'DirectCast(Me(Me.m_nRowHeaders - 1, 1), EwECell).Value = Me.m_Plugin.SS

        For igrp As Integer = 0 To Me.Core.nGroups
            Dim irow As Integer = igrp + Me.m_nRowHeaders - 1
            DirectCast(Me(irow, 1), EwECell).Value = Me.m_Plugin.BioAtBase(igrp)

            DirectCast(Me(irow, 2), EwECell).Value = Me.m_Plugin.BioAtTime(igrp)
            DirectCast(Me(irow, 3), EwECell).Value = Me.m_Plugin.BtB0(igrp)
            DirectCast(Me(irow, 4), EwECell).Value = Me.m_Plugin.BtBtMinus1(igrp)
        Next

    End Sub


End Class
