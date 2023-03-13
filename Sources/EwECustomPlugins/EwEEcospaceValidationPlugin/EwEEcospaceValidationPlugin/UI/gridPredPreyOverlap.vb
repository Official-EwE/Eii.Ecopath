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
    Private m_lGroupShown As New List(Of Integer)

    Public Sub New()
        ' Prevent flashing while updating
        Me.DoubleBuffered = True
    End Sub

#Region " Grid overrides "

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dimension the grid and create header cells.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        ' Define visualizer that does the mean cell background colouring
        If (Me.m_viz Is Nothing) Then
            Me.m_viz = New cAvgHighlightVisualizer(Me.UIContext.StyleGuide)
        End If

        Dim source As cCoreGroupBase = Nothing

        ' Only include consumers
        Me.m_lGroupShown.Clear()
        For i As Integer = 1 To Me.Core.nGroups
            source = Me.Core.EcopathGroupInputs(i)
            If (source.IsConsumer()) Then Me.m_lGroupShown.Add(i)
        Next i

        ' Resize grid
        Me.Redim(Me.Core.nGroups + 4, Me.m_lGroupShown.Count + 2)
        Dim rowCnt As Integer = Me.RowsCount

        ' Create row and column header cells
        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

        ' Column headers
        For iCol As Integer = 0 To Me.m_lGroupShown.Count - 1
            Dim iGrp As Integer = Me.m_lGroupShown(iCol)
            source = Me.Core.EcopathGroupInputs(iGrp)
            Me(0, 2 + iCol) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
        Next

        ' Row headers for groups
        For iRow As Integer = 1 To Me.Core.nGroups
            source = Me.Core.EcopathGroupInputs(iRow)
            ' Group index row header
            Me(iRow, 0) = New cEwERowHeaderCell(CStr(iRow))
            ' Group name row header
            Me(iRow, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
        Next iRow

        ' Row headers for bottom rows: min, mean and max
        Me(rowCnt - 3, 0) = New cEwERowHeaderCell()
        Me(rowCnt - 3, 1) = New cEwERowHeaderCell("Min")

        Me(rowCnt - 2, 0) = New cEwERowHeaderCell()
        Me(rowCnt - 2, 1) = New cEwERowHeaderCell("Mean")

        Me(rowCnt - 1, 0) = New cEwERowHeaderCell()
        Me(rowCnt - 1, 1) = New cEwERowHeaderCell("Max")

        Me.FixedColumns = 2

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Populate the contents of the grid
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        ' Build the content cells for the grid. This only creates the cells, but does not
        ' enter real values. These come later in response to user time step selections.

        ' Define visual styles to be used in the grid
        ' - Inactive cells
        Dim styleNull As eStyleFlags = eStyleFlags.OK Or eStyleFlags.NotEditable Or eStyleFlags.Null
        ' - Diet cells
        Dim styleDiet As eStyleFlags = eStyleFlags.OK Or eStyleFlags.NotEditable
        ' - Computed cells
        Dim styleComputed As eStyleFlags = eStyleFlags.OK Or eStyleFlags.NotEditable Or eStyleFlags.Sum

        ' - Special case: diagonal cells are done differently
        Dim visDiagonal As New SourceGrid2.VisualModels.Common
        visDiagonal.BackColor = Color.LightGray
        visDiagonal.TextAlignment = ContentAlignment.MiddleCenter

        ' For all shown consumers
        For i As Integer = 0 To Me.m_lGroupShown.Count - 1
            ' Get predator 
            Dim iPred As Integer = Me.m_lGroupShown(i)
            Dim pred As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(iPred)

            ' For all prey
            For iPrey As Integer = 1 To Me.Core.nGroups
                ' Determine cell style
                Dim style As eStyleFlags = If(pred.DietComp(iPrey) > 0, styleDiet, styleNull)
                ' Define new cell
                Dim cell As New cEwECell(0, GetType(Single), style)
                ' - that suopresses zeroes (0 =blank cell)
                cell.SuppressZero(0.0!) = True
                ' - set diagonal 
                If iPrey = iPred Then cell.VisualModel = visDiagonal
                ' Store cell in the grid
                Me(iPrey, 2 + i) = cell
            Next iPrey

            ' Define cells for summary rows too (min, mean, max)
            For k As Integer = 1 To 3
                ' Create cell
                Dim cell As New cEwECell(0, GetType(Single), styleComputed)
                ' - mean cells use the funky colour scheme that VC used in spreadsheet
                If k = 2 Then cell.VisualModel = Me.m_viz
                ' Store cell in the grid
                Me(Me.Core.nGroups + k, 2 + i) = cell
            Next k
        Next i

    End Sub

#End Region ' Grid overrides

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Callback from the parent form to update the grid with real data values.
    ''' </summary>
    ''' <param name="data">The calculated stats to show. Beware, data may be 
    ''' nothing if not computed yet.</param>
    ''' -----------------------------------------------------------------------
    Public Sub UpdateData(data As Double(,,), iRegion As Integer)

        ' The min and max bounds for the averages colour scale
        Dim minAvg As Single = If(data Is Nothing, 0, Single.MaxValue)
        Dim maxAvg As Single = 0

        For i As Integer = 0 To Me.m_lGroupShown.Count - 1
            ' Get predator 
            Dim iPred As Integer = Me.m_lGroupShown(i)
            Dim pred As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(iPred)

            ' Stuff to compute locally
            Dim min As Single = If(data Is Nothing, 0, Single.MaxValue)
            Dim tot As Single = 0
            Dim max As Single = 0
            Dim n As Integer = 0

            ' Only worry about consumers
            If (pred.IsConsumer) Then
                ' For all potential prey
                For iPrey As Integer = 1 To Me.Core.nGroups
                    ' Mjam?
                    If ((pred.DietComp(iPrey) > 0) And (iPred <> iPrey)) Then
                        Dim val As Single = 0
                        ' If there is data, update min, max and tot for this predator
                        If (data IsNot Nothing) Then
                            ' pred x prey x region
                            val = CSng(data(iPred, iPrey, iRegion))
                            min = Math.Min(min, val)
                            max = Math.Max(max, val)
                            tot += val
                            Me(iPrey, 2 + i).Value = val
                            ' Count data point
                            n += 1
                        End If
                    End If
                Next

                ' Calculate mean. Make not to div by 0
                Dim mean As Single = tot / Math.Max(n, 1)

                ' Populate summary cells; keep at 0 if there were no data points
                Me(Me.Core.nGroups + 1, 2 + i).Value = If(n = 0, 0, min)
                Me(Me.Core.nGroups + 2, 2 + i).Value = If(n = 0, 0, mean)
                Me(Me.Core.nGroups + 3, 2 + i).Value = If(n = 0, 0, max)

                ' Update min/max across all predators
                minAvg = Math.Min(minAvg, mean)
                maxAvg = Math.Max(maxAvg, mean)

            End If
        Next

        ' Update min/max colour scale in the magic visualizer
        Me.m_viz.Min = minAvg
        Me.m_viz.Max = maxAvg

    End Sub

#End Region ' Public access

End Class
