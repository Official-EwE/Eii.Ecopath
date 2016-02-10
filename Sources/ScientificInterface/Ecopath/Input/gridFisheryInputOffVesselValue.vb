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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Off-vessel price user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridFisheryOffVesselValue
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            Me.Redim(1, core.nFleets + 1 + 1)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(sharedResources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet names
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New PropertyColumnHeaderCell(Me.PropertyManager, _
                                                                     source, eVarNameFlags.Name, Nothing, _
                                                                     SharedResources.HEADER_X_UNIT_PER_UNIT, _
                                                                     New eUnitType() {eUnitType.Monetary, eUnitType.Biomass})
            Next

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(core.nGroups) As Integer 'Hold the stanza group index
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To core.nGroups : intStanzaGroupIndex(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.nLifeStages
                    source = Core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    intStanzaGroupIndex(source.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For rowIndex As Integer = 1 To core.nGroups
                source = core.EcoPathGroupInputs(rowIndex)

                If intStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source)
                Else 'Group is stanza
                    sg = core.StanzaGroups(intStanzaGroupIndex(source.Index))
                    If (Not dtStanzaCells.ContainsKey(sg)) Then
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To Core.nFleets + 1 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        iRow = Me.AddRow
                    Else
                        hgcStanza = dtStanzaCells(sg)
                        iRow = Me.AddRow(hgcStanza.Row + hgcStanza.NumChildRows + 1)
                    End If
                    'Display group info
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, source, True)
                End If
            Next

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, Optional ByVal isIndented As Boolean = False)

            Dim sourceSec As cCoreInputOutputBase = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If
            ' For each fleet
            For fleetIndex As Integer = 1 To Core.nFleets
                ' Get the fleet info
                sourceSec = Core.FleetInputs(fleetIndex)
                ' The market price is indexed by (fleetIndex, groupIndex)
                ' Add the dynamic property to the destined cell
                Me(iRow, fleetIndex + 1) = New PropertyCell(Me.PropertyManager, sourceSec, eVarNameFlags.OffVesselPrice, source)
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

