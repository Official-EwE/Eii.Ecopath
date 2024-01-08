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
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Linq
Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class gridRun
    Inherits cEwEGrid

    Private m_manager As cSFPManager = Nothing
    Private m_bInUpdate As Boolean = False

    ''' <summary>
    ''' Enumerated type, defining the columns to display in this grid
    ''' </summary>
    ''' <remarks>
    ''' To reorder of columns just change the order of the enumerated values
    ''' </remarks>
    Private Enum eColumnTypes As Integer
        Index = 0
        Name
        Enabled
        K
        EstimatedV
        SplinePoints
        SS
        AIC
        AICc
        State
        Elapsed
        Completed
    End Enum

    Public Sub Initialize(manager As cSFPManager)
        Me.m_manager = manager
    End Sub

    ''' <summary>
    ''' Refresh the iterations currently displayed in the grid 
    ''' </summary>
    Public Sub UpdateContent()

        For iRow As Integer = 1 To Me.RowsCount - 1
            Me.UpdateRowState(iRow)
        Next

        ' Re-fire selection event
        Me.RaiseSelectionChangeEvent()

    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Friend ReadOnly Property SelectedIteration As ISFPIteration
        Get
            Dim iRow As Integer = Me.SelectedRow
            If (iRow < 1) Then Return Nothing
            Return CType(Me.Rows(iRow).Tag, ISFPIteration)
        End Get
    End Property

    Public Sub UpdateRunState()
        For i As Integer = 1 To Me.RowsCount - 1
            Me.UpdateRowState(i)
        Next
    End Sub

#Region " Overrides "

    Public Overrides Sub RefreshContent()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_manager Is Nothing) Then Return

        MyBase.RefreshContent()

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_NAME)
        Me(0, eColumnTypes.Enabled) = New cEwEColumnHeaderCell(SharedResources.HEADER_ENABLED)
        Me(0, eColumnTypes.K) = New cEwEColumnHeaderCell(My.Resources.HEADER_K)
        Me(0, eColumnTypes.EstimatedV) = New cEwEColumnHeaderCell(My.Resources.HEADER_NUMVULS)
        Me(0, eColumnTypes.SplinePoints) = New cEwEColumnHeaderCell(My.Resources.HEADER_NUMSPLINE)
        Me(0, eColumnTypes.SS) = New cEwEColumnHeaderCell(My.Resources.HEADER_SS)
        Me(0, eColumnTypes.AIC) = New cEwEColumnHeaderCell(My.Resources.HEADER_AIC)
        Me(0, eColumnTypes.AICc) = New cEwEColumnHeaderCell(My.Resources.HEADER_AICc)
        Me(0, eColumnTypes.State) = New cEwEColumnHeaderCell(My.Resources.HEADER_STATE)
        Me(0, eColumnTypes.Elapsed) = New cEwEColumnHeaderCell(My.Resources.HEADER_ELAPSED)
        Me(0, eColumnTypes.Completed) = New cEwEColumnHeaderCell(My.Resources.HEADER_COMPLETED)

        Me.AllowBlockSelect = False
        Me.FixedColumnWidths = False
        Me.Selection.SelectionMode = SourceGrid2.GridSelectionMode.Row

    End Sub

    Protected Overrides Sub FillData()

        Me.RowsCount = 1

        Dim iterations As ISFPIteration() = Me.m_manager.Iterations
        Dim iteration As ISFPIteration = Nothing
        Dim cell As cEwECellBase = Nothing

        If iterations.Count = 0 Then Return

        Me.Rows.InsertRange(1, iterations.Length)

        For i As Integer = 0 To iterations.Length - 1

            iteration = iterations(i)
            Dim iRow = i + 1

            Me(iRow, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(i + 1))
            Me(iRow, eColumnTypes.Name) = New cEwERowHeaderCell(iteration.Name)

            Me(iRow, eColumnTypes.Enabled) = New cEwECheckboxCell(False)
            Me(iRow, eColumnTypes.Enabled).Behaviors.Add(Me.EwEEditHandler)

            Me(iRow, eColumnTypes.EstimatedV) = New cEwECell(cCore.NULL_VALUE, GetType(Integer), eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.SplinePoints) = New cEwECell(cCore.NULL_VALUE, GetType(Integer), eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.K) = New cEwECell(cCore.NULL_VALUE, GetType(Integer), eStyleFlags.NotEditable)

            cell = New cEwECell(cCore.NULL_VALUE, GetType(Single), eStyleFlags.NotEditable)
            cell.SuppressZero(0) = True
            Me(iRow, eColumnTypes.SS) = cell

            cell = New cEwECell(cCore.NULL_VALUE, GetType(Single), eStyleFlags.NotEditable)
            cell.SuppressZero(0) = True
            Me(iRow, eColumnTypes.AIC) = cell

            cell = New cEwECell(cCore.NULL_VALUE, GetType(Single), eStyleFlags.NotEditable)
            cell.SuppressZero(0) = True
            Me(iRow, eColumnTypes.AICc) = cell

            cell = New cEwECell("", GetType(String), eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.Elapsed) = cell

            cell = New cEwECell("", GetType(String), eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.Completed) = cell

            cell = New cEwECell("", GetType(String), eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.State) = cell

            Me.Rows(iRow).Tag = iteration

            Me.UpdateRowState(iRow)
        Next

    End Sub

    Private Sub UpdateRowState(iRow As Integer)

        Dim iteration As ISFPIteration = CType(Me.Rows(iRow).Tag, ISFPIteration)
        Dim style As eStyleFlags = 0
        Dim bIsRunning As Boolean = Me.m_manager.IsRunning

        Me.m_bInUpdate = True

        If (iteration.IsBestFit) Then style = eStyleFlags.Checked
        If bIsRunning Then style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.Enabled).Value = iteration.Enabled And Not bIsRunning
        DirectCast(Me(iRow, eColumnTypes.Enabled), IEwECell).Style = style

        Me(iRow, eColumnTypes.K).Value = iteration.K
        DirectCast(Me(iRow, eColumnTypes.K), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.EstimatedV).Value = iteration.EstimatedV
        DirectCast(Me(iRow, eColumnTypes.EstimatedV), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.SplinePoints).Value = iteration.SplinePoints
        DirectCast(Me(iRow, eColumnTypes.SplinePoints), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.SS).Value = iteration.SS
        DirectCast(Me(iRow, eColumnTypes.SS), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.AIC).Value = iteration.AIC
        DirectCast(Me(iRow, eColumnTypes.AIC), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.AICc).Value = iteration.AICc
        DirectCast(Me(iRow, eColumnTypes.AICc), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.Elapsed).Value = If(iteration.Elapsed.Milliseconds = 0, "", iteration.Elapsed.ToString())
        DirectCast(Me(iRow, eColumnTypes.Elapsed), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.Completed).Value = If(iteration.RunState < ISFPIteration.eRunState.Completed, "", iteration.Completed.ToString())
        DirectCast(Me(iRow, eColumnTypes.Elapsed), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me(iRow, eColumnTypes.State).Value = Me.State(iteration)
        DirectCast(Me(iRow, eColumnTypes.State), IEwECell).Style = style Or eStyleFlags.NotEditable

        Me.m_bInUpdate = False

    End Sub

    Protected Overrides Function OnCellValueChanged(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        If (Me.m_bInUpdate) Then Return True

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.Enabled

                Dim iteration As ISFPIteration = CType(Me.Rows(p.Row).Tag, ISFPIteration)
                iteration.Enabled = CBool(cell.GetValue(p))

                ' Cheat!
                Me.RaiseSelectionChangeEvent()

        End Select
        Return MyBase.OnCellValueChanged(p, cell)

    End Function

    Friend Function State(iteration As ISFPIteration) As String

        Select Case iteration.RunState
            Case ISFPIteration.eRunState.Idle
                Return ""
            Case ISFPIteration.eRunState.Pending
                Return My.Resources.STATE_ITERATION_PENDING
            Case ISFPIteration.eRunState.Initializing
                Return My.Resources.STATE_ITERATION_INITIALIZING
            Case ISFPIteration.eRunState.Completed
                Return My.Resources.STATE_ITERATION_OK
            Case ISFPIteration.eRunState.Error
                Return My.Resources.STATE_ITERATION_ERROR
            Case ISFPIteration.eRunState.Running
                Return My.Resources.STATE_ITERATION_RUNNING
            Case ISFPIteration.eRunState.Stopping
                Return My.Resources.STATE_ITERATION_STOPPING
            Case Else
                Debug.Assert(False, "Unknown state")
        End Select

        Return "?"

    End Function

#End Region ' Overrides

End Class
