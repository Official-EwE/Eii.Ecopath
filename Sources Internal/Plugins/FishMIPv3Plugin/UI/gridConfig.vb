' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class gridConfig
    Inherits cEwEGrid

    Public Sub New()
        MyBase.New()
    End Sub

    Public Property Configuration As cConfiguration = Nothing

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        If (Me.Configuration Is Nothing) Then Return

        Dim vars As cOutput() = Me.Configuration.Outputs
        Me.Redim(1, vars.Count + 2)

        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUP)
        For i As Integer = 0 To vars.Count - 1
            Dim cell As New cEwEColumnHeaderCell(vars(i).Name)
            cell.ToolTipText = vars(i).Description & vbNewLine & vars(i).Comments
            Me(0, 2 + i) = cell
            Me.Columns(2 + i).Tag = vars(i)
        Next

        Me.FixedColumns = 2

    End Sub

    Protected Overrides Sub FillData()

        If (Me.Configuration Is Nothing) Then Return

        Me.RowsCount = 1
        For i As Integer = 1 To Me.UIContext.Core.nGroups

            Dim iRow As Integer = Me.AddRow()
            Me(iRow, 0) = New cEwERowHeaderCell(CStr(i))
            Me(iRow, 1) = New cEwERowHeaderCell(Me.Core.EcoPathGroupInputs(i).Name)

            For j As Integer = 2 To Me.ColumnsCount - 1
                Dim var As cOutput = DirectCast(Me.Columns(j).Tag, cOutput)
                Dim cell As New cEwECell(var(i))
                cell.SuppressZero(0) = True

                Me(iRow, j) = cell
                Me(iRow, j).Behaviors.Add(Me.EwEEditHandler)
            Next
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.FixedColumnWidths = False
    End Sub

    Public Overrides Property IsOutputGrid As Boolean
        Get
            Return False
        End Get
        Set(value As Boolean)
            ' NOP
        End Set
    End Property

    Protected Overrides Function OnCellValueChanged(p As Position, cell As ICellVirtual) As Boolean

        Dim var As cOutput = DirectCast(Me.Columns(p.Column).Tag, cOutput)
        var(p.Row) = CSng(cell.GetValue(p))
        Return True

    End Function

End Class
