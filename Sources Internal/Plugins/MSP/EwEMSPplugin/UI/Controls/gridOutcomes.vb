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
Imports EwECore
Imports EwEShell
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace UI

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Outcome configuration grid; enables configuring how Ecospace predictions
    ''' are fed back to MEL.
    ''' </summary>
    ''' <seealso cref="ScientificInterfaceShared.Controls.EwEGrid.EwEGrid" />
    ''' -----------------------------------------------------------------------
    Public Class gridOutcomes
        Inherits EwEGrid

#Region " Internal vars "

        Private m_data As cGame = Nothing

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            Numerator
            Denominator
        End Enum

#End Region ' Internal vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event; fired when the user has changed a <see cref="cOutcome"/>
        ''' configuration.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Event OnMappingsChanged(sender As gridOutcomes)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new <see cref="gridOutcomes">outcome configuration grid</see>.
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

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(ScientificInterfaceShared.My.Resources.HEADER_NAME)
            Me(0, eColumnTypes.Numerator) = New EwEColumnHeaderCell(My.Resources.HEADER_NUMERATOR)
            Me(0, eColumnTypes.Denominator) = New EwEColumnHeaderCell(My.Resources.HEADER_DENOMINATOR)

            Me.FixedColumnWidths = False
            Me.FixedColumns = 2
            Me.AllowBlockSelect = False

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fill the grid with outcome configurations.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub FillData()

            If (Me.Shell Is Nothing) Then Return
            If (Me.Output Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            Dim bAllowDemonimators As Boolean = (Me.Output.LayerType = cOutcome.eLayerType.Indicator)
            Dim iRow As Integer = 0

            For i As Integer = 1 To Me.Output.NumItems

                Dim strName As String = ""
                Dim c As EwECell = Nothing

                Select Case Me.Output.LayerType
                    Case cOutcome.eLayerType.Biomass
                        strName = Me.UIContext.Core.EcoPathGroupInputs(i).Name
                    Case cOutcome.eLayerType.Effort, cOutcome.eLayerType.Catch
                        strName = Me.UIContext.Core.EcopathFleetInputs(i).Name
                    Case cOutcome.eLayerType.Indicator
                        strName = DirectCast(i - 1, cOutcome.eMSPDIversityIndex).ToString()
                End Select

                If (Not String.IsNullOrWhiteSpace(strName)) Then
                    iRow = Me.AddRow()

                    Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(i))
                    Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(strName)

                    c = New EwECell(CSng(Output.Numerator(i)))
                    c.SuppressZero(0) = True
                    Me(iRow, eColumnTypes.Numerator) = c
                    Me(iRow, eColumnTypes.Numerator).Behaviors.Add(Me.EwEEditHandler)

                    c = New EwECell(CSng(Output.Denominator(i)))
                    c.SuppressZero(0) = True
                    Me(iRow, eColumnTypes.Denominator) = c
                    Me(iRow, eColumnTypes.Denominator).Behaviors.Add(Me.EwEEditHandler)

                    Me.ItemIndex(iRow) = i
                End If

            Next

            'Me.Columns(eColumnTypes.Denominator).Visible = bAllowDemonimators

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
        ''' to user cell value changes. Overridden to update the configuration of 
        ''' the selected outcome.
        ''' </summary>
        ''' <param name="p">Position that was affected.</param>
        ''' <param name="cell">Cell that has received a new value.</param>
        ''' <returns>
        ''' The return value is ignored by the EwEGrid framework.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function OnCellValueChanged(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

            If MyBase.OnCellValueChanged(p, cell) Then

                Dim iItem As Integer = Me.ItemIndex(p.Row)

                Select Case DirectCast(p.Column, eColumnTypes)

                    Case eColumnTypes.Numerator
                        Me.Output.Numerator(iItem) = CDec(cell.GetValue(p))
                        Me.Shell.OnChanged()

                    Case eColumnTypes.Denominator
                        Me.Output.Denominator(iItem) = CDec(cell.GetValue(p))
                        Me.Shell.OnChanged()

                End Select

                Try
                    RaiseEvent OnMappingsChanged(Me)
                Catch ex As Exception

                End Try

                Return True
            End If

            Return False

        End Function

#End Region ' Overrides

#Region " Public bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEwEShell">MSP shell</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Shell As cEwEShell = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cOutcome">output</see> to configure.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Output As cOutcome = Nothing

#End Region ' Public bits

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the ordinal number of the <see cref="cOutcome"/> represented
        ''' in the grid row indicated by <paramref name="iRow"/>
        ''' </summary>
        ''' <param name="iRow">The row to get/set the outcome index for.</param>
        ''' <returns>The index, or 0 if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Protected Property ItemIndex(ByVal iRow As Integer) As Integer
            Get
                If (iRow < 1) Then Return 0
                Return CInt(Me.Rows(iRow).Tag)
            End Get
            Set(ByVal value As Integer)
                If (iRow < 1) Then Return
                Me.Rows(iRow).Tag = value
            End Set
        End Property

#End Region ' Internals

    End Class

End Namespace
