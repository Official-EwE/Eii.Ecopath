' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SourceGrid2.DataModels



Namespace UI

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Driver configuration grid; enables configuring how MEL <see cref="cPressure">pressures</see>
    ''' map to available <see cref="cDriver">Ecospace driver variables</see>.
    ''' </summary>
    ''' <seealso cref="ScientificInterfaceShared.Controls.EwEGrid.cEwEGrid" />
    ''' -----------------------------------------------------------------------
    Public Class gridPressureDriverMappings
        Inherits cEwEGrid

#Region " Internal vars "

        Private m_data As cGame = Nothing

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            Mapping
            Value
        End Enum

#End Region ' Internal vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event; fired when the user has changed a <see cref="cPressure"/>
        ''' to <see cref="cDriver"/> mapping displayed in the grid.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Event OnMappingsChanged(sender As gridPressureDriverMappings)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new <see cref="gridPressureDriverMappings">driver configuration grid</see>.
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
            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(My.Resources.HEADER_PRESSURE)
            Me(0, eColumnTypes.Mapping) = New cEwEColumnHeaderCell(My.Resources.HEADER_DRIVER)
            Me(0, eColumnTypes.Value) = New cEwEColumnHeaderCell(My.Resources.HEADER_VALUE)

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

                Me(iRow, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(i + 1))
                Me(iRow, eColumnTypes.Name) = New cEwERowHeaderCell(pressure.Name)

                Dim edt As EditorComboBox = Me.Editor(pressure)
                Dim d As cDriver = Me.Game.Driver(pressure.Name)

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

                If (TypeOf (pressure) Is cFishingEffortPressure) Then
                    Me(iRow, eColumnTypes.Value) = New cEwECell(Game.EffortMultiplier(pressure.Name))
                    Me(iRow, eColumnTypes.Value).Behaviors.Add(Me.EwEEditHandler)
                ElseIf (TypeOf (pressure) Is cFishingEcoPressure) Then
                    Me(iRow, eColumnTypes.Value) = New cEwECell(Game.EcologicalFishing(pressure.Name))
                    Me(iRow, eColumnTypes.Value).Behaviors.Add(Me.EwEEditHandler)
                Else
                    Me(iRow, eColumnTypes.Value) = New cEwECell("", eStyleFlags.Null Or eStyleFlags.NotEditable)
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
            If pressure Is Nothing Then Return False

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

                Case eColumnTypes.Value
                    If (TypeOf (pressure) Is cFishingEffortPressure) Then
                        Me.Game.EffortMultiplier(strDriver) = DirectCast(cell.GetValue(p), Double)
                        Me.Shell.OnChanged()

                    ElseIf (TypeOf (pressure) Is cFishingEcoPressure) Then
                        Game.EcologicalFishing(pressure.Name) = DirectCast(cell.GetValue(p), Boolean)
                        Me.Shell.OnChanged()
                    Else
                        Debug.Assert(False)
                    End If

            End Select
            Return True ' MyBase.OnCellValueChanged(p, cell)

        End Function

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
            lDrivers.AddRange(Game.Drivers(pressure))

            Try
                Dim e As New EditorComboBox(GetType(cDriver))
                e.StandardValues = lDrivers.ToArray()
                e.StandardValuesExclusive = True
                Return e
            Catch ex As Exception

            End Try
            Return Nothing
        End Function

        Private Property Pressure(ByVal iRow As Integer) As cPressure
            Get
                If (iRow < 1) Or (iRow > Me.RowsCount) Then Return Nothing
                Return DirectCast(Me.Rows(iRow).Tag, cPressure)
            End Get
            Set(ByVal value As cPressure)
                If (iRow < 1) Or (iRow > Me.RowsCount) Then Return
                Me.Rows(iRow).Tag = value
            End Set
        End Property

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

#End Region ' Internals

    End Class

End Namespace ' UI
