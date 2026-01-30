' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.MSE
Imports EwECore.Common
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style

''' ===========================================================================
''' <summary>
''' Grid to allow biomass limits to be specified for credible results.
''' </summary>
''' ===========================================================================

Public Class gridBiomassLimits
    Inherits cEwEGrid

#Region " Internal defs "

    Private Enum eColumnTypes As Integer
        GroupIndex = 0
        GroupName
        LowerLimit
        UpperLimit
    End Enum

#End Region ' Internal defs

    Private m_data As cBiomassLimits = Nothing

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Constructor

#Region " Public access "

    Public Sub Init(data As cBiomassLimits)
        Me.m_data = data
        Me.FillData()
    End Sub

    Public Event onEdited()

#End Region ' Public access

#Region " Overrides "

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length
        Me.Redim(1, iNumCols)

        Me(0, eColumnTypes.GroupIndex) = New cEwEColumnHeaderCell(My.Resources.HEADER_GROUP_INDEX)
        Me(0, eColumnTypes.GroupName) = New cEwEColumnHeaderCell(My.Resources.HEADER_GROUP_NAME)
        Me(0, eColumnTypes.LowerLimit) = New cEwEColumnHeaderCell(My.Resources.HEADER_LOWER_LIMIT_VALID)
        Me(0, eColumnTypes.UpperLimit) = New cEwEColumnHeaderCell(My.Resources.HEADER_UPPER_LIMIT_VALID)

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False
        Me.AllowBlockSelect = False
        'Me.Columns(0).Width = 200

    End Sub

    Protected Overrides Sub FillData()

        If (Me.m_data Is Nothing) Then Return

        Dim iRow As Integer = -1
        Dim cell As cEwECell = Nothing

        Me.RowsCount = 1

        For i As Integer = 1 To Me.m_data.Count
            iRow = Me.AddRow()

            Dim igroup As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(i)
            'Me(iRow, eColumnTypes.GroupIndex) = New EwERowHeaderCell(CStr(igroup.Index))
            'Me(iRow, eColumnTypes.GroupName) = New EwERowHeaderCell(CStr(igroup.Name))

            Me(iRow, eColumnTypes.GroupIndex) = New cEwERowHeaderCell(CStr(Me.m_data(i - 1).mGroup.Index))
            Me(iRow, eColumnTypes.GroupName) = New cEwERowHeaderCell(CStr(Me.m_data(i - 1).mGroup.Name))
            Me(iRow, eColumnTypes.LowerLimit) = Me.DataCell(CSng(Me.m_data(i - 1).mLowerLimit))
            Me(iRow, eColumnTypes.UpperLimit) = Me.DataCell(CSng(Me.m_data(i - 1).mUpperLimit))

            ' No need to use tags here: row number = fleet number
            ' Me.Rows(iRow).Tag = i

        Next

        Me.Columns(eColumnTypes.GroupIndex).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        'Me.Columns(eColumnTypes.GroupIndex)
        Me.Columns(eColumnTypes.GroupName).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.Columns(eColumnTypes.LowerLimit).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch Or SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.AutoSizeColumn(eColumnTypes.LowerLimit, 150)
        Me.Columns(eColumnTypes.UpperLimit).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch Or SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.AutoSizeColumn(eColumnTypes.UpperLimit, 150)

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.NotSet
        End Get
    End Property

    Private Function DataCell(dValue As Single) As cEwECell

        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
        Dim cell As cEwECell = Nothing

        If (dValue = cCore.NULL_VALUE) Then
            style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
        End If

        cell = New cEwECell(CSng(dValue), GetType(Single), style)
        cell.Behaviors.Add(Me.EwEEditHandler)
        Return cell

    End Function

    Public Property Data As cBiomassLimits
        Get
            Return Me.m_data
        End Get
        Set(value As cBiomassLimits)
            Me.m_data = value
            Me.FillData()
        End Set

    End Property

    Protected Overrides Function OnCellEdited(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        If (Me.m_data Is Nothing) Then Return False
        If (Not MyBase.OnCellEdited(p, cell)) Then Return False

        ' Check column
        If (p.Column = eColumnTypes.LowerLimit) Then
            ' Store value
            Me.m_data(p.Row - 1).mLowerLimit = Convert.ToSingle(cell.GetValue(p))
        End If
        If (p.Column = eColumnTypes.UpperLimit) Then
            ' Store value
            Me.m_data(p.Row - 1).mUpperLimit = Convert.ToSingle(cell.GetValue(p))
        End If

        ' Yippee
        Me.RaiseDataChangeEvent()

        ' Done
        Return True

    End Function

    Private Sub RaiseDataChangeEvent()
        Try
            RaiseEvent onEdited()
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Overrides

End Class

