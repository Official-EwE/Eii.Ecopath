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

Imports EwECore
Imports EwECore.Style
Imports EwEUtils.Core
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports 

<CLSCompliant(False)>
Public Class gridCatchability
    Inherits cEwEGrid

    Private m_iSelFleet As Integer = 1

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides ReadOnly Property CoreComponents As eCoreComponentType()
        Get
            Return New eCoreComponentType() {eCoreComponentType.EcoSim}
        End Get
    End Property

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' Test for UI context to prevent core from being accessed
        If (Me.UIContext Is Nothing) Then Return

        Me.FixedColumns = 1
        Me.FixedRows = 1

        Dim source As cCoreInputOutputBase = Nothing

        ' ToDo: consider adding only columns for caught groups; this is way too expensive on the UI
        Me.Redim(1, Me.Core.nGroups + 1)

        Me(0, 0) = New cEwEColumnHeaderCell(SharedResources.TSDATASETINTERVAL_TIMESTEP)

        For columnIndex As Integer = 1 To Me.Core.nGroups
            source = Me.Core.EcoSimGroupInputs(columnIndex)
            Me(0, columnIndex) = New cEwEColumnHeaderCell(source.Name)
        Next

    End Sub

    Protected Overrides Sub FillData()

        Dim cell As cEwECell = Nothing
        Dim source As cEcosimFleetInput
        source = Me.Core.EcosimFleetInputs(Me.m_iSelFleet)

        For it As Integer = 1 To Me.Core.nEcosimTimeSteps
            Me.AddRow(it)

            cell = New cEwECell(CStr(it), GetType(String))
            cell.Style = cStyleGuide.eStyleFlags.NotEditable
            Me(it, 0) = cell

            '    Me.SetCell(it, 0, New EwECell(it, GetType(Integer)))
            For igrp As Integer = 1 To Me.Core.nGroups
                Dim style As cStyleGuide.eStyleFlags
                If source.RelQtStatus(igrp, it) = (eStatusFlags.OK Or eStatusFlags.Stored) Then
                    style = cStyleGuide.eStyleFlags.OK
                Else
                    ' JS 01Mar18: unused data should be hidden as NULL
                    style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
                End If

                cell = New cEwECell(0, GetType(Single), style)
                cell.SuppressZero = False
                cell.Behaviors.Add(Me.EwEEditHandler)
                ' JS 01Mar18: Not core null?
                cell.DataModel.DefaultValue = 0.0!

                Me(it, igrp) = cell

            Next
        Next
        Me.UpdateContent()

    End Sub

    Private Sub UpdateContent()

        If Me.IsDisposed Then Return

        Dim cell As cEwECell = Nothing
        Dim source As cEcosimFleetInput
        source = Me.Core.EcosimFleetInputs(Me.m_iSelFleet)

        For it As Integer = 1 To Me.Core.nEcosimTimeSteps

            For igrp As Integer = 1 To Me.Core.nGroups

                cell = CType(Me(it, igrp), cEwECell)
                cell.Value = source.RelQt(igrp, it)
            Next
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()

        If Me.UIContext Is Nothing Then Return
        If (Me.SelectedFleetIndex <= 0) Then Return

        ' For CSV import/export
        Dim fleet As cEcopathFleetInput = Me.Core.EcopathFleetInputs(Me.SelectedFleetIndex)
        Me.DataName = "Ecosim_catchbility_" & fleet.Name

    End Sub

    Public Property SelectedFleetIndex As Integer
        Get
            Return Me.m_iSelFleet
        End Get
        Set(value As Integer)
            If Me.UIContext Is Nothing Then Return
            If (value <> Me.m_iSelFleet) Then
                Me.m_iSelFleet = value
                Me.RefreshContent()
            End If
        End Set
    End Property

    Protected Overrides Function OnCellValueChanged(p As Position, cell As Cells.ICellVirtual) As Boolean
        MyBase.OnCellValueChanged(p, cell)
        Try

            Dim source As cEcosimFleetInput
            source = Me.Core.EcosimFleetInputs(Me.m_iSelFleet)
            Dim igrp As Integer = p.Column
            Dim it As Integer = p.Row
            source.RelQt(igrp, it) = CSng(cell.GetValue(p))

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Overrides Sub OnCoreMessage(ByRef msg As cMessage)
        MyBase.OnCoreMessage(msg)
        If (msg.Source = eCoreComponentType.EcoSim And msg.DataType = eDataTypes.EcosimFleetInput) Then
            Me.BeginInvoke(New MethodInvoker(AddressOf UpdateContent))
        End If
    End Sub

End Class
