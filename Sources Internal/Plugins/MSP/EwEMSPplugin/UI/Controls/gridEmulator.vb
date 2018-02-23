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
Imports System.Windows.Forms
Imports EwECore
Imports EwEMSPPlugin.Emulator
Imports EwEShell
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SourceGrid2
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace UI

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Test data configuration grid; enables managing <see cref="cTestset">simulated MEL test data</see>.
    ''' </summary>
    ''' <seealso cref="ScientificInterfaceShared.Controls.EwEGrid.EwEGrid" />
    ''' -----------------------------------------------------------------------
    Public Class gridEmulator
        Inherits EwEGrid

#Region " Private vars "

        Private m_testset As cTestset = Nothing
        Private m_game As cGame = Nothing

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            Testdata
        End Enum

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new <see cref="gridEmulator">test set configuration</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event; fired when the user has changed the content of a <see cref="cTestset">test set</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Event OnTestsetChanged(sender As Object, t As cTestset)

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid columns and layout.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, 3)
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_PRESSURE)
            Me(0, eColumnTypes.Testdata) = New EwEColumnHeaderCell(My.Resources.HEADER_TESTDATA)

            Me.FixedColumnWidths = False
            Me.FixedColumns = 2
            Me.AllowBlockSelect = False

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fill the grid with testset data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_testset Is Nothing) Then Return
            If (Me.m_game Is Nothing) Then Return

            Dim iRow As Integer = 0
            Dim cell As EwECell = Nothing

            For i As Integer = 0 To Me.m_testset.Pressures.Count - 1

                Dim pressure As cPressure = Me.m_testset.Pressures(i)
                Dim style As eStyleFlags = eStyleFlags.OK
                If (Me.m_game.Driver(pressure.Name) Is Nothing) Then style = eStyleFlags.NotEditable

                iRow = Me.AddRow()

                Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(i + 1))
                Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(pressure.Name)

                Select Case pressure.DataType
                    Case cPressure.eDataTypes.Scalar
                        cell = New EwECell(cStringUtils.ConvertToSingle(Me.m_testset.Testdata(pressure)), style)
                        cell.SuppressZero(cCore.NULL_VALUE) = True
                    Case cPressure.eDataTypes.Grid
                        cell = New EwECell(Me.m_testset.Testdata(pressure), style)
                    Case Else
                        Debug.Assert(False)
                        cell = New EwECell("", eStyleFlags.NotEditable)
                End Select

                Me(iRow, eColumnTypes.Testdata) = cell
                Me(iRow, eColumnTypes.Testdata).Behaviors.Add(Me.EwEEditHandler)

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
            Me.Columns(eColumnTypes.Index).Width = 20
            MyBase.FinishStyle()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Edit handler to catch driver cell clicks. Overridden to invoke a 
        ''' folder browse dialog when the user clicks a Grid pressure cell.
        ''' </summary>
        ''' <param name="p">Position that was affected.</param>
        ''' <param name="cell">Cell that was clicked.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnCellClicked(p As Position, cell As ICellVirtual)

            Dim pressure As cPressure = Me.Pressure(p.Row)
            Select Case pressure.DataType
                Case cPressure.eDataTypes.Scalar
                    ' NOP
                Case cPressure.eDataTypes.Grid
                    Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(cStringUtils.Localize(My.Resources.PROMPT_SELECT_MAP, pressure.Name), "", My.Resources.FILEFILTER_TESTSET_MAP)
                    If (ofd.ShowDialog() = DialogResult.OK) Then
                        Me.m_testset.Testdata(pressure) = ofd.FileName
                        Me(p.Row, p.Column).SetValue(p, ofd.FileName)
                    End If
                    Return
                Case Else
                    ' NOP
            End Select

            MyBase.OnCellClicked(p, cell)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="P:ScientificInterfaceShared.Controls.EwEGrid.EwEGrid.EwEEditHandler">EwEEditHandler</see> callback for responding
        ''' to user cell value changes. Overridden to update testset content.
        ''' </summary>
        ''' <param name="p">Position that was affected.</param>
        ''' <param name="cell">Cell that has received a new value.</param>
        ''' <returns>
        ''' The return value is ignored by the EwEGrid framework.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function OnCellValueChanged(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

            Dim pressure As cPressure = Me.Pressure(p.Row)

            Select Case pressure.DataType
                Case cPressure.eDataTypes.Scalar
                    Dim val As Single = cCore.NULL_VALUE
                    If (cell.GetValue(p) IsNot Nothing) Then val = CSng(cell.GetValue(p))
                    Me.m_testset.Testdata(pressure) = cStringUtils.FormatSingle(val)
                    Me.OnChanged()
                Case cPressure.eDataTypes.Grid
                    Me.m_testset.Testdata(pressure) = CStr(cell.GetValue(p))
                    Me.OnChanged()
                Case Else
                    ' NOP
            End Select
            Return MyBase.OnCellValueChanged(p, cell)

        End Function

#End Region ' Overrides

#Region " Public bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the active <see cref="cTestset">test set</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Testset() As cTestset
            Get
                Return Me.m_testset
            End Get
            Set(value As cTestset)
                Me.m_testset = value
                Me.RefreshContent()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the active <see cref="cGame">game</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Game As cGame
            Get
                Return Me.m_game
            End Get
            Set(value As cGame)
                Me.m_game = value
                Me.RefreshContent()
            End Set
        End Property

#End Region ' Public bits

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the pressure associated with a given grid row.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Property Pressure(ByVal iRow As Integer) As cPressure
            Get
                Return DirectCast(Me.Rows(iRow).Tag, cPressure)
            End Get
            Set(ByVal value As cPressure)
                Me.Rows(iRow).Tag = value
            End Set
        End Property

        Private Sub OnChanged()
            Try
                RaiseEvent OnTestsetChanged(Me, Me.m_testset)
            Catch ex As Exception
                ' Aargh
            End Try
        End Sub

#End Region ' Internals

    End Class

End Namespace
