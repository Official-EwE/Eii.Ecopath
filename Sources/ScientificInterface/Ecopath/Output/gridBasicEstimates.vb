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
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)>
    Public Class gridBasicEstimates
        Inherits cEwEGrid

        Enum eColumnTypes As Integer
            Index = 0
            Name
            TL
            Area
            BArea
            B
            Z
            PB
            QB
            EE
            GE
            BA
            BArate
        End Enum

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.TL) = New cEwEColumnHeaderCell(eVarNameFlags.TTLX)
            Me(0, eColumnTypes.Area) = New cEwEColumnHeaderCell(eVarNameFlags.HabitatArea)
            Me(0, eColumnTypes.BArea) = New cEwEColumnHeaderCell(eVarNameFlags.BiomassAreaOutput)
            Me(0, eColumnTypes.B) = New cEwEColumnHeaderCell(eVarNameFlags.Biomass)
            Me(0, eColumnTypes.Z) = New cEwEColumnHeaderCell(eVarNameFlags.Z)
            Me(0, eColumnTypes.PB) = New cEwEColumnHeaderCell(eVarNameFlags.PBOutput)
            Me(0, eColumnTypes.QB) = New cEwEColumnHeaderCell(eVarNameFlags.QBOutput)
            Me(0, eColumnTypes.EE) = New cEwEColumnHeaderCell(eVarNameFlags.EEOutput)
            Me(0, eColumnTypes.GE) = New cEwEColumnHeaderCell(eVarNameFlags.GEOutput)
            Me(0, eColumnTypes.BA) = New cEwEColumnHeaderCell(eVarNameFlags.BioAccumOutput)
            Me(0, eColumnTypes.BArate) = New cEwEColumnHeaderCell(eVarNameFlags.BioAccumRate, eDescriptorTypes.Abbreviation)

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True

        End Sub

        Protected Overrides Sub FillData()

            Dim groups As cCoreGroupBase() = Me.StyleGuide.Groups(Me.Core)
            Dim group As cEcopathGroupOutput = Nothing
            Dim cell As cEwECellBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim iStanzaPrev As Integer = -1
            Dim hgcStanza As cEwEHierarchyGridCell = Nothing

            'Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For i As Integer = 0 To groups.Count - 1

                ' Get corresponding Ecopath output group 
                group = Me.Core.EcopathGroupOutputs(groups(i).Index)

                If Not group.IsMultiStanza Then

                    iRow = Me.AddRow
                    Me.UpdateRow(iRow, group)

                Else
                    ' Group is stanza
                    sg = Me.Core.StanzaGroups(group.iStanza)
                    If group.iStanza <> iStanzaPrev Then

                        ' Complete row with dummy cells
                        iRow = Me.AddRow()
                        For j As Integer = 0 To Me.ColumnsCount - 1 : Me(iRow, j) = New cEwERowHeaderCell() : Next

                        hgcStanza = New cEwEHierarchyGridCell()
                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)

                        iStanzaPrev = group.iStanza
                        iRow = Me.AddRow
                    Else
                        iRow = Me.AddRow(hgcStanza.Row + hgcStanza.NumChildRows + 1)
                    End If

                    'Display group info
                    hgcStanza.AddChildRow(iRow)
                    Me.UpdateRow(iRow, group, True)
                End If
            Next i

        End Sub

        Private Sub UpdateRow(iRow As Integer, source As cCoreInputOutputBase, Optional bIsStanza As Boolean = False)

            Dim cell As cEwECellBase = Nothing

            Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If bIsStanza Then
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If

            Me(iRow, eColumnTypes.TL) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.TTLX)
            Me(iRow, eColumnTypes.Area) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.HabitatArea)
            Me(iRow, eColumnTypes.BArea) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.BiomassAreaOutput)
            Me(iRow, eColumnTypes.B) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.Biomass)
            Me(iRow, eColumnTypes.BA) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumOutput)
            Me(iRow, eColumnTypes.BArate) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumRatePerYear)

            If bIsStanza Then
                Me(iRow, eColumnTypes.Z) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.PBOutput)
            Else
                cell = New cEwECell("", GetType(String))
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.Z) = cell
            End If

            If Not bIsStanza Then
                Me(iRow, eColumnTypes.PB) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.PBOutput)
            Else
                cell = New cEwECell("", GetType(String))
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.PB) = cell
            End If

            Me(iRow, eColumnTypes.QB) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.QBOutput)
            Me(iRow, eColumnTypes.EE) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.EEOutput)
            Me(iRow, eColumnTypes.GE) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.GEOutput)

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Dim ci As ColumnInfo = Me.Columns(eColumnTypes.Z)

            Me.Rows(0).Height = 60
            Me.Columns(eColumnTypes.Index).Width = 24
            Me.Columns(eColumnTypes.Name).Width = 120
            Me.Columns(eColumnTypes.Name).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

            If (Me.Core Is Nothing) Then Return

            ci.Visible = (Me.Core.nStanzas > 0)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property

    End Class

End Namespace
