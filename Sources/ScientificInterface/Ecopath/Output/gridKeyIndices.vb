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

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class gridKeyIndices
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim aUnitType As cStyleGuide.eUnitType() = {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time}

            Me.Redim(1, 8)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_BIOMACCUM_UNIT, aUnitType)
            Me(0, 3) = New EwEColumnHeaderCell(SharedResources.HEADER_BIOMACCUM_RATE_ABBR_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, 4) = New EwEColumnHeaderCell(SharedResources.HEADER_NETMIGRATION_UNIT, aUnitType)
            Me(0, 5) = New EwEColumnHeaderCell(SharedResources.HEADER_FLOWTODETR_UNIT, aUnitType)
            Me(0, 6) = New EwEColumnHeaderCell(SharedResources.HEADER_NETEFFICIENCY)
            Me(0, 7) = New EwEColumnHeaderCell(SharedResources.HEADER_OMNIVORYINDEX)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim groups As cCoreGroupBase() = Me.StyleGuide.Groups(Me.Core)
            Dim group As cEcoPathGroupOutput = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim iStanzaPrev As Integer = -1

            'Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For i As Integer = 0 To groups.Count - 1

                ' Get corresponding Ecopath output group 
                group = Me.Core.EcoPathGroupOutputs(groups(i).Index)

                If Not group.isMultiStanza Then

                    iRow = Me.AddRow
                    FillInRows(iRow, group)

                Else

                    ' Group is stanza
                    sg = Core.StanzaGroups(group.iStanza)
                    If group.iStanza <> iStanzaPrev Then

                       ' Complete row with dummy cells
                        iRow = Me.AddRow()
                        For j As Integer = 0 To Me.ColumnsCount - 1 : Me(iRow, j) = New EwERowHeaderCell() : Next

                        hgcStanza = New EwEHierarchyGridCell()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)

                        iStanzaPrev = group.iStanza
                        iRow = Me.AddRow
                    Else
                        iRow = Me.AddRow(hgcStanza.Row + hgcStanza.NumChildRows + 1)
                    End If

                    'Add row index as stanza child
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, group, True)

                End If
            Next i

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, Optional ByVal isIndented As Boolean = False)
            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If

            Me(iRow, 2) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccum)
            Me(iRow, 3) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumRatePerYear)
            Me(iRow, 4) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.NetMigration)
            Me(iRow, 5) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.FlowToDet)
            Me(iRow, 6) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.NetEfficiency)
            Me(iRow, 7) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.OmnivoryIndex)
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
