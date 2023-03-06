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
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid to configure 'Ecopath model from Ecosim' generation.
''' </summary>
''' ---------------------------------------------------------------------------
Friend Class gridUI
    Inherits cEwEGrid

    ''' <summary>Grid columns.</summary>
    Private Enum eColumnTypes As Integer
        Year = 0
        Check
        Name
    End Enum

#Region " Public bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the data to use for configuring model generation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Data() As cData

#End Region ' Public bits

#Region " Grid overrides "

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub InitStyle()

        Me.Redim(1, [Enum].GetNames(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Year) = New cEwEColumnHeaderCell(My.Resources.HEADER_ECOSIM_YEAR)
        Me(0, eColumnTypes.Check) = New cEwEColumnHeaderCell(My.Resources.HEADER_CREATE_MODEL)
        Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(My.Resources.HEADER_MODEL_NAME)

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False
        Me.AllowBlockSelect = False

        Me.Columns(eColumnTypes.Year).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.Columns(eColumnTypes.Check).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(eColumnTypes.Name).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch

    End Sub

    Protected Overrides Sub FillData()

        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

        If (Me.UIContext Is Nothing) Then Return
        If (Me.Data Is Nothing) Then Return

        Me.RowsCount = 1
        For i As Integer = 1 To Me.Data.NumYears

            Me.AddRow()

            If Me.Data.CreateModel(i) Then
                style = cStyleGuide.eStyleFlags.OK
            Else
                style = cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable
            End If

            Me(i, eColumnTypes.Year) = New cEwERowHeaderCell(CStr(Me.Data.FirstLabelYear + i - 1))

            Me(i, eColumnTypes.Check) = New Cells.Real.CheckBox(Me.Data.CreateModel(i))
            Me(i, eColumnTypes.Check).Behaviors.Add(Me.EwEEditHandler)

            Me(i, eColumnTypes.Name) = New cEwECell(Me.Data.ModelName(i), GetType(String), style)
            Me(i, eColumnTypes.Name).Behaviors.Add(Me.EwEEditHandler)

        Next

    End Sub

    Protected Overrides Function OnCellValueChanged(p As Position,
                                                    cell As Cells.ICellVirtual) As Boolean

        Select Case DirectCast(p.Column, eColumnTypes)

            Case eColumnTypes.Check
                Me.Data.CreateModel(p.Row) = CBool(Me(p.Row, p.Column).Value)

                Dim ewec As cEwECell = DirectCast(Me(p.Row, eColumnTypes.Name), cEwECell)
                If Me.Data.CreateModel(p.Row) Then
                    ewec.Style = cStyleGuide.eStyleFlags.OK
                Else
                    ewec.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
                End If
                Me(p.Row, eColumnTypes.Name).Value = Me.Data.ModelName(p.Row)

        End Select

        Me.InvalidateRange(New Range(p.Row, 0, p.Row, Me.RowsCount - 1))
        Return MyBase.OnCellValueChanged(p, cell)

    End Function

    Protected Overrides Function OnCellEdited(p As Position,
                                              cell As Cells.ICellVirtual) As Boolean
        Try
            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.Name
                    Me.Data.ModelName(p.Row) = CStr(Me(p.Row, p.Column).Value)

            End Select
        Catch ex As Exception

        End Try

        Me.InvalidateRange(New Range(p.Row, 0, p.Row, Me.RowsCount - 1))
        Return MyBase.OnCellEdited(p, cell)

    End Function

#End Region ' Grid overrides

End Class
