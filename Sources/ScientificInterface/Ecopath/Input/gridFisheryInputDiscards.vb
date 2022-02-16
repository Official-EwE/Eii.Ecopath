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
Option Explicit On

Imports EwECore
Imports EwECore.Style
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Discards user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)>
    Public Class gridFisheryInputDiscards
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub InitStyle()

            Dim source As cCoreInputOutputBase = Nothing
            Dim md As cVariableMetaData = Nothing

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            'Define grid dimensions
            Me.Redim(1, Me.Core.nFleets + 3)

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet name
            For fleetIndex As Integer = 1 To Me.Core.nFleets
                source = Me.Core.EcopathFleetInputs(fleetIndex)
                md = source.GetVariableMetadata(eVarNameFlags.Discards)
                Me(0, fleetIndex + 1) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name, Nothing, md.Units)

            Next

            ' Total column
            Me(0, Me.Core.nFleets + 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_TOTAL)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim groups As cCoreGroupBase() = Me.StyleGuide.Groups(Me.Core)
            Dim group As cCoreGroupBase = Nothing
            Dim fleet As cEcopathFleetInput = Nothing
            Dim cell As cEwECellBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim iStanzaPrev As Integer = -1
            Dim hgcStanza As cEwEHierarchyGridCell = Nothing

            Dim prop As cProperty = Nothing

            Dim alSumRow As New ArrayList()
            Dim alSumAll As New ArrayList()
            Dim alSumCol As New ArrayList()

            Dim opSumAll As cMultiOperation = Nothing
            Dim opSumRow As cMultiOperation = Nothing
            Dim opSumCol As cMultiOperation = Nothing

            Dim propSumRow As cFormulaProperty = Nothing
            Dim propSumAll As cFormulaProperty = Nothing
            Dim propSumCol As cFormulaProperty = Nothing

            'Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If Me.Core.nFleets = 0 Then Return

            ' Create rows for all groups
            For i As Integer = 0 To groups.Count - 1

                ' Clear the arrayList for the new row
                alSumRow.Clear()
                ' Get the Ecopath input for this specific group
                group = groups(i)

                If Not group.IsMultiStanza Then

                    iRow = Me.AddRow()
                    Me.FillInRows(iRow, group, alSumRow, alSumAll)

                Else

                    sg = Me.Core.StanzaGroups(group.iStanza)

                    ' Create hierarchy cell if entering a new stanza config
                    If group.iStanza <> iStanzaPrev Then
                        ' Fill row with dummy cells. We'll do something fancy here one day
                        iRow = Me.AddRow()
                        For j As Integer = 0 To Me.ColumnsCount - 1 : Me(iRow, j) = New cEwERowHeaderCell() : Next

                        hgcStanza = New cEwEHierarchyGridCell()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New cPropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)

                        iStanzaPrev = group.iStanza
                        iRow = Me.AddRow()
                    Else
                        iRow = Me.AddRow(hgcStanza.Row + hgcStanza.NumChildRows + 1)
                    End If

                    ' Display group info
                    hgcStanza.AddChildRow(iRow)
                    Me.FillInRows(iRow, group, alSumRow, alSumAll, True)
                End If

                ' Set the property to the last cell of the row, which is the sum of the row
                opSumRow = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumRow.ToArray())
                propSumRow = Me.Formula(opSumRow)
                Me(iRow, Me.ColumnsCount - 1) = New cPropertyCell(propSumRow)
            Next

            ' Sum row
            iRow = Me.AddRow()
            Me(iRow, 0) = New cEwERowHeaderCell(CStr(iRow))
            Me(iRow, 1) = New cEwERowHeaderCell(SharedResources.HEADER_SUM)
            For fleetIndex As Integer = 1 To Me.Core.nFleets
                fleet = Me.Core.EcopathFleetInputs(fleetIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To Me.Core.nGroups
                    sourceSec = Me.Core.EcopathGroupInputs(rowIndex)
                    prop = Me.PropertyManager.GetProperty(fleet, eVarNameFlags.Discards, sourceSec)
                    alSumCol.Add(prop)
                Next
                opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
                propSumCol = Me.Formula(opSumCol)
                ' Set the property to the last cell of the column, which is the sum of the column
                Me(Me.RowsCount - 1, fleetIndex + 1) = New cPropertyCell(propSumCol)
            Next


            opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
            propSumAll = Me.Formula(opSumAll)

            ' Set the property to the bottom-right cell, which is the sum of all cells
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = New cPropertyCell(propSumAll)
        End Sub

        Private Sub FillInRows(iRow As Integer, source As cCoreInputOutputBase, _
            ByRef alSumRow As ArrayList, ByRef alSumAll As ArrayList, Optional isIndented As Boolean = False)

            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim prop As cProperty = Nothing
            Dim propCell As cPropertyCell = Nothing

            Me(iRow, 0) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New cPropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If
            ' For each fleet (each column) 
            For fleetIndex As Integer = 1 To Me.Core.nFleets
                ' Get the fleet object 
                sourceSec = Me.Core.EcopathFleetInputs(fleetIndex)
                ' Get the index landing property
                prop = Me.PropertyManager.GetProperty(sourceSec, eVarNameFlags.Discards, source)

                propCell = New cPropertyCell(prop)
                propCell.SuppressZero = True
                ' Set the property to the cell
                Me(iRow, fleetIndex + 1) = propCell

                ' Add the property to ArrayList; it is used for the sum
                alSumRow.Add(prop)
                alSumAll.Add(prop)
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property

    End Class

End Namespace

