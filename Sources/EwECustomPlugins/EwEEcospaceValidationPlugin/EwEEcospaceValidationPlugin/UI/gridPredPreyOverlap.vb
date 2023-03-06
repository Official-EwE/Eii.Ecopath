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
Imports System.Drawing
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports
Public Class gridPredPreyOverlap
    Inherits cEwEGrid

    Private m_viz As cAvgHighlightVisualizer = Nothing

    Public Sub New()
        Me.DoubleBuffered = True
    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.m_viz = New cAvgHighlightVisualizer(Me.UIContext.StyleGuide)

        Dim source As cCoreGroupBase = Nothing

        Me.Redim(Me.Core.nGroups + 4, Me.Core.nLivingGroups + 2)
        Dim rowCnt As Integer = Me.RowsCount

        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

        For i As Integer = 1 To Me.Core.nGroups
            source = Me.Core.EcopathGroupInputs(i)

            Me(i, 0) = New cEwERowHeaderCell(CStr(i))
            Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

            If (i <= Me.Core.nLivingGroups) Then
                Me(0, i) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            End If

        Next

        Me(rowCnt - 3, 0) = New cEwERowHeaderCell()
        Me(rowCnt - 3, 1) = New cEwERowHeaderCell("Min")

        Me(rowCnt - 2, 0) = New cEwERowHeaderCell()
        Me(rowCnt - 2, 1) = New cEwERowHeaderCell("Mean")

        Me(rowCnt - 1, 0) = New cEwERowHeaderCell()
        Me(rowCnt - 1, 1) = New cEwERowHeaderCell("Max")

        Me.FixedColumns = 2

    End Sub

    Public Sub UpdateData(data As Double(,))

        Dim avgMin As Single = If(data Is Nothing, 0, Single.MaxValue)
        Dim avgMax As Single = 0

        For j As Integer = 1 To Me.Core.nLivingGroups
            Dim min As Single = If(data Is Nothing, 0, Single.MaxValue)
            Dim tot As Single = 0
            Dim max As Single = 0
            Dim n As Integer = 0
            Dim pred As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(j)

            For i As Integer = 1 To Me.Core.nGroups
                If (pred.DietComp(i) > 0) Then
                    Dim val As Single = 0
                    If (data IsNot Nothing) Then
                        ' pred x prey
                        val = CSng(data(j, i))
                    End If
                    min = Math.Min(min, val)
                    max = Math.Max(max, val)
                    tot += val
                    Me(i, j + 1).Value = val
                End If
            Next

            Dim mean As Single = tot / Math.Max(n, 1)
            avgMin = Math.Min(avgMin, mean)
            avgMax = Math.Max(avgMax, mean)

            Me(Me.Core.nGroups + 1, j + 1).Value = If(n = 0, 0, min)
            Me(Me.Core.nGroups + 2, j + 1).Value = If(n = 0, 0, mean)
            Me(Me.Core.nGroups + 3, j + 1).Value = If(n = 0, 0, max)
        Next

        Me.m_viz.Min = avgMin
        Me.m_viz.Max = avgMax

    End Sub

    Protected Overrides Sub FillData()

        Dim styleNull As eStyleFlags = eStyleFlags.OK Or eStyleFlags.NotEditable Or eStyleFlags.Null
        Dim styleDiet As eStyleFlags = eStyleFlags.OK Or eStyleFlags.NotEditable

        Dim visDiagonal As New SourceGrid2.VisualModels.Common
        visDiagonal.BackColor = Color.LightGray
        visDiagonal.TextAlignment = ContentAlignment.MiddleCenter

        For j As Integer = 1 To Me.Core.nLivingGroups
            Dim pred As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(j)
            For i As Integer = 1 To Me.Core.nGroups
                Dim style As eStyleFlags = If(pred.DietComp(i) > 0, styleDiet, styleNull)
                Dim cell As New cEwECell(0, style)
                cell.SuppressZero(0) = True
                If i = j Then cell.VisualModel = visDiagonal
                Me(i, j) = cell
            Next i
            For i As Integer = 1 To 3
                Dim cell As New cEwECell(0, styleDiet)
                If i = 2 Then cell.VisualModel = Me.m_viz
                Me(Me.Core.nGroups + i, j + 1) = cell
            Next i
        Next j

    End Sub

End Class
