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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
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
    ''' Grid accepting Ecopath Other Production user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridOtherProduction
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            Immig
            Emig
            EmigRate
            BioAccum
            BioAccumRate
        End Enum

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim aUnitType As cStyleGuide.eUnitType() = {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time}

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.Immig) = New EwEColumnHeaderCell(SharedResources.HEADER_IMMIGRATION_UNIT, aUnitType)
            Me(0, eColumnTypes.Emig) = New EwEColumnHeaderCell(SharedResources.HEADER_EMIGRATION_UNIT, aUnitType)
            Me(0, eColumnTypes.EmigRate) = New EwEColumnHeaderCell(SharedResources.HEADER_EMIGRATIONRATE_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.BioAccum) = New EwEColumnHeaderCell(SharedResources.HEADER_BIOMACCUM_UNIT, aUnitType)
            Me(0, eColumnTypes.BioAccumRate) = New EwEColumnHeaderCell(SharedResources.HEADER_BIOMACCUM_RATE_ABBR_UNIT, cStyleGuide.eUnitType.Time)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreGroupBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim blnStanza(Core.nLivingGroups) As Boolean
            Dim intStanza(Core.nLivingGroups) As Integer 'Hold the stanza group number
            Dim intStanzaPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To Core.nLivingGroups : intStanza(i) = -1 : Next

            'Remove existing rows
            Me.RowsCount = 1

            'Tag stanza group first
            For stanzaGroupIndex As Integer = 0 To Core.nStanzas - 1
                sg = Core.StanzaGroups(stanzaGroupIndex)
                For stanzaIndex As Integer = 1 To sg.NStanzas
                    source = Core.EcoPathGroupInputs(sg.iGroups(stanzaIndex))
                    blnStanza(source.Index) = True
                    intStanza(source.Index) = stanzaGroupIndex
                Next
            Next

            'Create rows for all groups
            For groupIndex As Integer = 1 To Core.nLivingGroups
                source = Core.EcoPathGroupInputs(groupIndex)

                If intStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info

                    iRow = Me.AddRow
                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(iRow, eColumnTypes.Immig) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Immig)
                    Me(iRow, eColumnTypes.Emig) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Emig)
                    Me(iRow, eColumnTypes.EmigRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EmigRate)
                    Me(iRow, eColumnTypes.BioAccum) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccum)
                    Me(iRow, eColumnTypes.BioAccumRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumRate)

                Else 'Group is stanza

                    sg = Core.StanzaGroups(intStanza(source.Index))
                    If intStanza(source.Index) <> intStanzaPrev Then 'If stanza group appears the first time Then display + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()

                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)

                        'Complete row with dummy cells
                        For i As Integer = 2 To Me.ColumnsCount - 1 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaPrev = intStanza(source.Index)
                        iRow = Me.AddRow
                    Else
                        hgcStanza = dtStanzaCells(sg)
                        iRow = Me.AddRow(hgcStanza.Row + hgcStanza.NumChildRows + 1)
                    End If

                    'Display group info
                    hgcStanza.AddChildRow(iRow)

                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(iRow, eColumnTypes.Immig) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Immig)
                    Me(iRow, eColumnTypes.Emig) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Emig)
                    Me(iRow, eColumnTypes.EmigRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EmigRate)
                    Me(iRow, eColumnTypes.BioAccum) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccum)
                    Me(iRow, eColumnTypes.BioAccumRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumRate)

                End If
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
