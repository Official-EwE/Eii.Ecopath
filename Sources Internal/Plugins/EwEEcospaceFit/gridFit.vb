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
Public Class gridFit

    Private m_FitData As cEcospaceFit

    Private m_nRowHeaders As Integer

    Public WriteOnly Property EcospaceFit As cEcospaceFit
        Set(value As cEcospaceFit)
            m_FitData = value
        End Set
    End Property


    Public Sub New()
        MyBase.new()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Try
            m_nRowHeaders = 4
            'Define grid dimensions
            ' Me.Redim(1, Me.Core.nGroups + m_nFixed)
            Me.Redim(Me.Core.nGroups + m_nRowHeaders, 1)

            Me.FixedColumns = 1
            Me.FixedRows = m_nRowHeaders
            Dim headercell As EwERowHeaderCell
            'Define row headers
            Me(0, 0) = New EwERowHeaderCell("Ecospace run number")
            Me(1, 0) = New EwERowHeaderCell("Timesteps")
            headercell = New EwERowHeaderCell("SS")
            headercell.ToolTipText = "=sumof(log(B(t)/B(0))^2)"
            Me(2, 0) = headercell
            headercell = New EwERowHeaderCell("MSE")
            headercell.ToolTipText = "=SS/N"
            Me(3, 0) = headercell
            For i As Integer = 0 To Me.Core.nGroups - 1
                Me(i + m_nRowHeaders, 0) = New EwERowHeaderCell(Me.Core.EcoPathGroupInputs(i + 1).Name)
            Next

        Catch ex As Exception

        End Try


    End Sub


    Protected Overrides Sub FillData()

        If Me.m_FitData Is Nothing Then Exit Sub
        Dim RunNumber As Integer
        For Each stat As cFitStats In Me.m_FitData.FitStats
            RunNumber += 1
            Dim n As Integer = Me.Columns.Count
            Me.Columns.Insert(n)

            Me(0, n) = New EwEColumnHeaderCell(CStr(RunNumber))
            Me(1, n) = New EwEColumnHeaderCell(CStr(stat.nTimeSteps))
            Me(2, n) = New EwEColumnHeaderCell(EwEUtils.Utilities.cStringUtils.FormatSingle(stat.SS))
            Me(3, n) = New EwEColumnHeaderCell(EwEUtils.Utilities.cStringUtils.FormatSingle(stat.MSE))
            For i As Integer = 0 To Me.Core.nGroups - 1
                Me(i + Me.m_nRowHeaders, n) = New Cell(EwEUtils.Utilities.cStringUtils.FormatSingle(stat.bSSGroup(i + 1)))
            Next

        Next

    End Sub


End Class
