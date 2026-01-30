' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style



''' ---------------------------------------------------------------------------
''' <summary>
''' Grid to edit the varying number of parameters of a <see cref="IShapeFunction">shape function</see>.
''' </summary>
''' ---------------------------------------------------------------------------

Public Class gridShapeFunctionParameters
    Inherits cEwEGrid

    Private Enum eColumnTypes As Integer
        Name = 0
        Value
        Units
    End Enum

    Private m_shapefunction As IShapeFunction = Nothing
    Private m_bIsFreehand As Boolean = False

    Public Property ShapeFunction As IShapeFunction
        Get
            Return Me.m_shapefunction
        End Get
        Set(value As IShapeFunction)

            If (Me.m_shapefunction IsNot Nothing) Then
                ' Cleanup
                Me.RowsCount = 1
                Me.m_bIsFreehand = False
            End If

            Me.m_shapefunction = value

            If (Me.m_shapefunction IsNot Nothing) Then
                ' Set new
                Me.m_bIsFreehand = TypeOf (Me.m_shapefunction) Is cFreehandShapeFunction
                Me.RowsCount = If(Me.m_bIsFreehand, DirectCast(Me.m_shapefunction, cFreehandShapeFunction).nPoints, Me.m_shapefunction.nParameters) + 1
                Me.FillData()
            End If

        End Set
    End Property

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub InitLayout()

        MyBase.InitLayout()

        Me.Redim(1, 3)

        Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(My.Resources.HEADER_PARAMETER)
        Me(0, eColumnTypes.Value) = New cEwEColumnHeaderCell(My.Resources.HEADER_VALUE)
        Me(0, eColumnTypes.Units) = New cEwEColumnHeaderCell(My.Resources.HEADER_UNITS)

        Me.Selection.SelectionMode = SourceGrid2.GridSelectionMode.Row
        Me.Selection.EnableMultiSelection = False

        Me.AllowBlockSelect = False
        Me.FixedColumns = 1

    End Sub

    Protected Overrides Sub FillData()

        If (Me.m_bIsFreehand) Then
            Dim fh As cFreehandShapeFunction = DirectCast(Me.m_shapefunction, cFreehandShapeFunction)
            For iRow As Integer = 1 To Me.RowsCount - 1
                Me(iRow, eColumnTypes.Name) = New cEwERowHeaderCell(CStr(iRow))
                Me(iRow, eColumnTypes.Value) = New cEwECell(fh.ShapeData(iRow))
                Me(iRow, eColumnTypes.Value).Behaviors.Add(Me.EwEEditHandler)
                Me(iRow, eColumnTypes.Units) = New cEwECell("", cStyleGuide.eStyleFlags.NotEditable)
            Next iRow
        Else
            For iRow As Integer = 1 To Me.RowsCount - 1
                Me(iRow, eColumnTypes.Name) = New cEwERowHeaderCell(Me.m_shapefunction.ParamName(iRow))
                Me(iRow, eColumnTypes.Value) = New cEwECell(Me.m_shapefunction.ParamValue(iRow), CType(Me.m_shapefunction.ParamStatus(iRow), Style.cStyleGuide.eStyleFlags))
                Me(iRow, eColumnTypes.Value).Behaviors.Add(Me.EwEEditHandler)
                Me(iRow, eColumnTypes.Units) = New cEwECell(Me.m_shapefunction.ParamUnit(iRow), cStyleGuide.eStyleFlags.NotEditable)
            Next iRow
        End If

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.AutoStretchColumnsToFitWidth = True
        Me.FixedColumnWidths = False
    End Sub

    Public Event OnShapeFunctionChanged()

    Protected Overrides Function OnCellValueChanged(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.Value
                Dim iParam As Integer = p.Row
                Dim sValue As Single = CSng(cell.GetValue(p))
                If (Me.m_bIsFreehand) Then
                    Dim fh As cFreehandShapeFunction = DirectCast(Me.m_shapefunction, cFreehandShapeFunction)
                    fh.ShapeData(iParam) = sValue
                Else
                    Me.m_shapefunction.ParamValue(iParam) = sValue
                End If
                RaiseEvent OnShapeFunctionChanged()

                Me.Update()
        End Select

        Return True

    End Function

    Public Overloads Sub Update()
        MyBase.Update()
        Me.UpdateValues()
    End Sub


    Private Sub UpdateValues()
        Try
            If (Me.m_bIsFreehand) Then
                Dim fh As cFreehandShapeFunction = DirectCast(Me.m_shapefunction, cFreehandShapeFunction)
                For iRow As Integer = 1 To Me.RowsCount - 1
                    Me(iRow, eColumnTypes.Value).Value = fh.ShapeData(iRow)
                Next iRow
            Else
                For iRow As Integer = 1 To Me.RowsCount - 1
                    Me(iRow, eColumnTypes.Value).Value = Me.m_shapefunction.ParamValue(iRow)
                Next iRow
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class
