Option Strict On
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

#Region " Imports "

Imports System.Drawing
Imports EwECore
Imports EwECore.Style
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class gridDietComposition
    Inherits EwEGrid

    Private m_data As cEcopathMergeGroupsDatastructures = Nothing
    Private m_fmt As cVarnameTypeFormatter
    Private m_iRowTarget As Integer = -1
    Private m_iColTarget As Integer = -1

    Public Sub New()
        Me.m_fmt = New cVarnameTypeFormatter()
    End Sub

    Public Sub Init(uic As cUIContext, data As cEcopathMergeGroupsDatastructures)
        ' Set data before UIC; UIC setting will trigger grid refresh etc.
        Me.m_data = data
        Me.UIContext = uic
    End Sub

    Public Sub UpdateContent()

        If (Me.m_iRowTarget < 1) Or (Me.m_iRowTarget > Me.RowsCount) Then Return
        Me(Me.m_iRowTarget, 1).Value = Me.m_data.GroupName

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_data Is Nothing) Then Return

        Dim core As cCore = Me.UIContext.Core
        Dim grp1 As cEcoPathGroupInput = Nothing
        Dim grp2 As cEcoPathGroupInput = Nothing
        Dim source As cCoreGroupBase = Nothing
        Dim style As eStyleFlags = eStyleFlags.Checked

        If Me.m_data.IndexTarget > 0 Then grp1 = core.EcoPathGroupInputs(Me.m_data.IndexTarget)
        If Me.m_data.IndexMerge > 0 Then grp2 = core.EcoPathGroupInputs(Me.m_data.IndexMerge)

        Me.m_iColTarget = -1
        Me.m_iRowTarget = -1

        Me.FixedColumnWidths = False
        Me.Redim(1, 2)

        Me(0, 0) = New EwEColumnHeaderCell()
        Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

        Dim iCol As Integer = 2
        Dim iRow As Integer = 1

        For i As Integer = 1 To core.nGroups

            If (grp1 IsNot Nothing) Then
                If (i = grp1.Index) Then
                    Me.Rows.Insert(iRow)
                    Me(iRow, 0) = New EwERowHeaderCell("New")
                    Me(iRow, 1) = New EwERowHeaderCell(Me.m_data.GroupName)
                    Me.m_iRowTarget = iRow
                    iRow += 1
                End If
            End If

            source = core.EcoPathGroupInputs(i)
            Me.Rows.Insert(iRow)
            Me(iRow, 0) = New EwERowHeaderCell(CStr(i))
            Me(iRow, 1) = New EwERowHeaderCell(source.Name)
            Me.Rows(iRow).Tag = i
            iRow += 1

            If (grp2 IsNot Nothing) Then
                If (i = grp1.Index And grp1.IsConsumer) Then
                    Me.Columns.Insert(iCol)
                    Me(0, iCol) = New EwEColumnHeaderCell("New")
                    Me.m_iColTarget = iCol
                    iCol += 1
                End If
            End If

            If (source.IsConsumer) Then
                Me.Columns.Insert(iCol)
                Me(0, iCol) = New EwEColumnHeaderCell(CStr(i))
                Me.Columns(iCol).Tag = i
                iCol += 1
            End If
        Next

        Me.Rows.Insert(iRow)
        Me(iRow, 0) = New EwEColumnHeaderCell()
        Me(iRow, 1) = New EwERowHeaderCell(SharedResources.HEADER_IMPORT)
        Me.Rows(iRow).Tag = 0 ' Imports

        Me.FixedColumns = 2
        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        Dim core As cCore = Me.UIContext.Core
        Dim iPred As Integer = -1
        Dim iPrey As Integer = -1
        Dim val As Single = 0
        Dim style As eStyleFlags
        Dim cell As EwECell = Nothing

        'Dim visOut As New SourceGrid2.VisualModels.Common()
        'visOut.BackColor = Color.LightGray
        'visOut.TextAlignment = ContentAlignment.MiddleCenter

        For iRow As Integer = 1 To Me.RowsCount - 1

            If (iRow = Me.m_iRowTarget) Then
                iPrey = -1
            Else
                iPrey = CInt(Me.Rows(iRow).Tag)
            End If

            For iCol As Integer = 2 To Me.ColumnsCount - 1

                style = eStyleFlags.NotEditable

                If (iCol = Me.m_iColTarget) Then
                    iPred = -1
                Else
                    iPred = CInt(Me.Columns(iCol).Tag)
                End If

                If (iPred = -1 Or iPrey = -1) Then
                    Dim i As Integer = iPred
                    Dim j As Integer = iPrey
                    If i = -1 Then i = Me.m_data.IndexTarget
                    If j = -1 Then j = Me.m_data.IndexTarget
                    val = Me.m_data.DCInput(i, j)
                    style = style Or eStyleFlags.Sum
                Else
                    Dim pred As cEcoPathGroupInput = core.EcoPathGroupInputs(iPred)
                    If (iPred = Me.m_data.IndexTarget Or iPred = Me.m_data.IndexMerge) Or
                       (iPrey = Me.m_data.IndexTarget Or iPrey = Me.m_data.IndexMerge) Then
                        val = pred.DietComp(iPrey)
                        style = style Or eStyleFlags.Checked
                    Else
                        val = Me.m_data.DCInput(iPred, iPrey)
                        ' Validate if changing
                        If (val - cSystemUtils.IIF(iPrey = 0, pred.ImpDiet(), pred.DietComp(iPrey))) > 0.00001 Then
                            ' Unexpected large difference
                            style = style Or eStyleFlags.ErrorEncountered
                        End If
                    End If
                End If

                cell = New EwECell(val, style)
                cell.SuppressZero = True
                Me(iRow, iCol) = cell

            Next
        Next

    End Sub

    Private Sub UpdateCell(iRow As Integer, iCol As Integer, val As Single, style As eStyleFlags)

        Dim c As EwECell = DirectCast(Me(iRow, iCol), EwECell)
        c.Value = val
        c.Style = style

    End Sub

End Class
