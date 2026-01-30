' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports SourceGrid2

Public MustInherit Class gridApplyShapeBase
    Inherits cEwEGrid

#Region " Private vars "

    Protected m_editor As DataModels.EditorTextBox = Nothing
    Protected m_interactionManager As cMediatedInteractionManager = Nothing
    Protected m_bmRowCol As BehaviorModels.CustomEvents = Nothing
    Protected m_bmCell As BehaviorModels.CustomEvents = Nothing

#End Region ' Private vars

#Region " Construction and destruction "

    Public Sub New()
        MyBase.New()

        Me.m_editor = New DataModels.EditorTextBox(GetType(String))
        Me.m_bmRowCol = New BehaviorModels.CustomEvents()
        Me.m_bmCell = New BehaviorModels.CustomEvents()

        AddHandler Me.m_bmRowCol.Click, New SourceGrid2.PositionEventHandler(AddressOf Me.OnRowColClicked)
        AddHandler Me.m_bmCell.Click, AddressOf Me.CellClick

    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)

        If (Me.m_editor IsNot Nothing) Then
            RemoveHandler Me.m_bmRowCol.Click, New SourceGrid2.PositionEventHandler(AddressOf Me.OnRowColClicked)
            RemoveHandler Me.m_bmCell.Click, AddressOf Me.CellClick
            Me.m_editor = Nothing
            Me.m_bmCell = Nothing
            Me.m_bmRowCol = Nothing
        End If

    End Sub

#End Region ' Construction and destruction

#Region " Public access "

    <Browsable(False)>
    Public Overrides Property UIContext() As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As cUIContext)
            If (value IsNot Nothing) Then
                ' First set crucial properties
                Me.m_interactionManager = value.Core.MediatedInteractionManager
            Else
                Me.m_interactionManager = Nothing
            End If
            ' Always pass this to the grid
            MyBase.UIContext = value
        End Set
    End Property

    Public MustOverride Sub ClearAllPairs()

    Public MustOverride Sub SetAllPairs()

    ''' <summary>
    ''' Repopulate content without redimensioning
    ''' </summary>
    Public Sub UpdateContent()
        Me.FillData()
    End Sub

#End Region ' Public access

#Region " Overrides "

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.FixedColumnWidths = False
    End Sub

#End Region ' Overrides 

#Region " Internals "

    Protected MustOverride Sub CellClick(sender As Object, e As PositionEventArgs)

    Protected MustOverride Sub OnRowColClicked(sender As Object, e As SourceGrid2.PositionEventArgs)

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the group at a given row.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' -------------------------------------------------------------------
    Protected Property GroupAtRow(iRow As Integer) As Integer
        Get
            If (1 <= iRow And iRow < Me.RowsCount) Then
                Return CInt(Me.Rows(iRow).Tag)
            End If
            Return 0
        End Get
        Set(value As Integer)
            If (1 <= iRow And iRow < Me.RowsCount) Then
                Me.Rows(iRow).Tag = value
            End If
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the group at a given column.
    ''' </summary>
    ''' <param name="iCol"></param>
    ''' -------------------------------------------------------------------
    Protected Property GroupAtColumn(iCol As Integer) As Integer
        Get
            If (2 <= iCol And iCol < Me.ColumnsCount) Then
                Return CInt(Me.Columns(iCol).Tag)
            End If
            Return 0
        End Get
        Set(value As Integer)
            If (2 <= iCol And iCol < Me.ColumnsCount) Then
                Me.Columns(iCol).Tag = value
            End If
        End Set
    End Property

#End Region ' Internals

End Class
