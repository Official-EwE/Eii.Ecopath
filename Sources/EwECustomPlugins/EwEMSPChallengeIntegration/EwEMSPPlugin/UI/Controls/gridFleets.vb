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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace UI

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fleetconfiguration grid.
    ''' </summary>
    ''' <seealso cref="ScientificInterfaceShared.Controls.EwEGrid.cEwEGrid" />
    ''' -----------------------------------------------------------------------
    Public Class gridFleets
        Inherits cEwEGrid

        Private m_data As cGame = Nothing

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            Nationality
            NoDiscards
            NoBycatch
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event; fired when the user has changed a <see cref="cPressure"/>
        ''' to <see cref="cDriver"/> mapping displayed in the grid.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Event OnMappingsChanged(sender As gridPressureDriverMappings)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new <see cref="gridFleets">test set configuration</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            ' NOP
        End Sub

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid columns and layout.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, 5)
            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_NAME)
            Me(0, eColumnTypes.Nationality) = New cEwEColumnHeaderCell("Country")
            Me(0, eColumnTypes.NoDiscards) = New cEwEColumnHeaderCell("Discards")
            Me(0, eColumnTypes.NoBycatch) = New cEwEColumnHeaderCell("Bycatch")

            Me.FixedColumnWidths = False
            Me.FixedColumns = 2
            Me.AllowBlockSelect = False

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fill the grid with pressure - driver mappings.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub FillData()

            If (Me.Shell Is Nothing) Then Return
            If (Me.Game Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            Dim iRow As Integer = 0

            For i As Integer = 0 To Game.Fleets.Count - 1

                Dim fleet As cFleet = Me.Game.Fleets(i)
                iRow = Me.AddRow()

                Me(iRow, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(i + 1))
                Me(iRow, eColumnTypes.Name) = New cEwERowHeaderCell(fleet.Name)
                Me(iRow, eColumnTypes.Nationality) = New cEwECell(1)
                Me(iRow, eColumnTypes.NoBycatch) = New cEwECell(fleet.NoBycatch)
                Me(iRow, eColumnTypes.NoDiscards) = New cEwECell(fleet.NoDiscards)


                ' Drivers are created on the fly. To avoid exceptions, make sure the shown driver is obtained from the editor
                'For Each v As Object In edt.StandardValues
                '    Dim dtmp As cDriver = DirectCast(v, cDriver)
                '    If (dtmp IsNot Nothing) And (d IsNot Nothing) Then
                '        If (dtmp.Name = d.Name) Then
                '            d = dtmp
                '        End If
                '    End If
                'Next

                'Me(iRow, eColumnTypes.Mapping) = New SourceGrid2.Cells.Real.Cell(d, edt)
                'Me(iRow, eColumnTypes.Mapping).Behaviors.Add(Me.EwEEditHandler)

                'If (pressure.DataType = cPressure.eDataTypes.Scalar) Then
                '    Me(iRow, eColumnTypes.Mulitplier) = New cEwECell(Game.Multiplier(pressure.Name))
                '    Me(iRow, eColumnTypes.Mulitplier).Behaviors.Add(Me.EwEEditHandler)
                'Else
                '    Me(iRow, eColumnTypes.Mulitplier) = New cEwECell("", eStyleFlags.Null Or eStyleFlags.NotEditable)
                'End If

                'Me.Pressure(iRow) = pressure

            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Finalizes grid formatting.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub FinishStyle()
            Me.AutoStretchColumnsToFitWidth = True
            MyBase.FinishStyle()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="P:ScientificInterfaceShared.Controls.EwEGrid.EwEGrid.EwEEditHandler">EwEEditHandler</see> callback for responding
        ''' to user cell value changes. Overridden to update driver mappings.
        ''' </summary>
        ''' <param name="p">Position that was affected.</param>
        ''' <param name="cell">Cell that has received a new value.</param>
        ''' <returns>
        ''' The return value is ignored by the EwEGrid framework.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function OnCellValueChanged(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

            'Dim pressure As cPressure = Me.Pressure(p.Row)
            'Dim strDriver As String = pressure.Name

            'Select Case DirectCast(p.Column, eColumnTypes)

            '    Case eColumnTypes.Mapping
            '        Me.Game.Driver(strDriver) = DirectCast(cell.GetValue(p), cDriver)
            '        Me.Shell.OnChanged()
            '        Try
            '            RaiseEvent OnMappingsChanged(Me)
            '        Catch ex As Exception
            '            ' WHoah!
            '            Debug.Assert(False, ex.Message)
            '        End Try

            '    Case eColumnTypes.Mulitplier
            '        Me.Game.Multiplier(strDriver) = DirectCast(cell.GetValue(p), Double)
            '        Me.Shell.OnChanged()

            'End Select
            Return MyBase.OnCellValueChanged(p, cell)

        End Function

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

#End Region ' Overrides


#Region " Public bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEwEMSPLink">MSP shell</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Shell As cEwEMSPLink

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the active <see cref="cGame">game</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Game As cGame
            Get
                Return Me.m_data
            End Get
            Set(value As cGame)
                Me.m_data = value
                Me.RefreshContent()
            End Set
        End Property

#End Region ' Public bits

    End Class

End Namespace
