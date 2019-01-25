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
Imports EwEShell
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SourceGrid2.DataModels

#End Region ' Imports

Namespace UI

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Driver configuration grid; enables configuring how MEL <see cref="cPressure">pressures</see>
    ''' map to available <see cref="cDriver">Ecospace driver variables</see>.
    ''' </summary>
    ''' <seealso cref="ScientificInterfaceShared.Controls.EwEGrid.EwEGrid" />
    ''' -----------------------------------------------------------------------
    Public Class gridDrivers
        Inherits EwEGrid

#Region " Internal vars "

        Private m_data As cGame = Nothing

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            Mapping
            Mulitplier
        End Enum

#End Region ' Internal vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event; fired when the user has changed a <see cref="cPressure"/>
        ''' to <see cref="cDriver"/> mapping displayed in the grid.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Event OnMappingsChanged(sender As gridDrivers)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new <see cref="gridDrivers">driver configuration grid</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
        End Sub

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid columns and layout.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, 4)
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_PRESSURE)
            Me(0, eColumnTypes.Mapping) = New EwEColumnHeaderCell(My.Resources.HEADER_DRIVER)
            Me(0, eColumnTypes.Mulitplier) = New EwEColumnHeaderCell(My.Resources.HEADER_MULTIPLIER)

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

            For i As Integer = 0 To Game.Pressures.Count - 1

                Dim pressure As cPressure = Me.Game.Pressures(i)
                iRow = Me.AddRow()

                Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(i + 1))
                Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(pressure.Name)

                Dim edt As EditorComboBox = Me.Editor(pressure)
                Dim d As cDriver = Game.Driver(pressure.Name)

                ' Drivers are created on the fly. To avoid exceptions, make sure the shown driver is obtained from the editor
                For Each v As Object In edt.StandardValues
                    Dim dtmp As cDriver = DirectCast(v, cDriver)
                    If (dtmp IsNot Nothing) And (d IsNot Nothing) Then
                        If (dtmp.Name = d.Name) Then
                            d = dtmp
                        End If
                    End If
                Next

                Me(iRow, eColumnTypes.Mapping) = New SourceGrid2.Cells.Real.Cell(d, edt)
                Me(iRow, eColumnTypes.Mapping).Behaviors.Add(Me.EwEEditHandler)

                If (pressure.DataType = cPressure.eDataTypes.Scalar) Then
                    Me(iRow, eColumnTypes.Mulitplier) = New EwECell(Game.Multiplier(pressure.Name))
                    Me(iRow, eColumnTypes.Mulitplier).Behaviors.Add(Me.EwEEditHandler)
                Else
                    Me(iRow, eColumnTypes.Mulitplier) = New EwECell("", eStyleFlags.Null Or eStyleFlags.NotEditable)
                End If

                Me.Pressure(iRow) = pressure

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

            Dim pressure As cPressure = Me.Pressure(p.Row)
            Dim strDriver As String = pressure.Name

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.Mapping
                    Me.Game.Driver(strDriver) = DirectCast(cell.GetValue(p), cDriver)
                    Me.Shell.OnChanged()
                    Try
                        RaiseEvent OnMappingsChanged(Me)
                    Catch ex As Exception
                        ' WHoah!
                        Debug.Assert(False, ex.Message)
                    End Try

                Case eColumnTypes.Mulitplier
                    Me.Game.Multiplier(strDriver) = DirectCast(cell.GetValue(p), Double)
                    Me.Shell.OnChanged()

            End Select
            Return MyBase.OnCellValueChanged(p, cell)

        End Function

#End Region ' Overrides

#Region " Public bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEwEShell">MSP shell</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Shell As cEwEShell

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cPressure">pressure</see> currently selected in
        ''' the grid.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property SelectedPressure As cPressure
            Get
                Return Me.Pressure(Me.SelectedRow)
            End Get
        End Property

#End Region ' Public bits

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Build a <see cref="EditorComboBox">editor</see> for a given <see cref="cPressure">pressure</see>
        ''' that ontains compatible <see cref="cDriver">drivers</see>.
        ''' </summary>
        ''' <param name="pressure">The pressure.</param>
        ''' <returns>The editor.</returns>
        ''' -------------------------------------------------------------------
        Private Function Editor(pressure As cPressure) As EditorComboBox

            Dim lDrivers As New List(Of cDriver)
            lDrivers.Add(Nothing)
            lDrivers.AddRange(Game.Drivers(pressure.DataType))

            Dim e As New EditorComboBox(GetType(cDriver))
            e.StandardValues = lDrivers.ToArray()
            e.StandardValuesExclusive = True

            Return e

        End Function

        Private Property Pressure(ByVal iRow As Integer) As cPressure
            Get
                If (iRow < 1) Then Return Nothing
                Return DirectCast(Me.Rows(iRow).Tag, cPressure)
            End Get
            Set(ByVal value As cPressure)
                If (iRow < 1) Then Return
                Me.Rows(iRow).Tag = value
            End Set
        End Property

#End Region ' Internals

    End Class

End Namespace ' UI
