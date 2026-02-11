' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecopath.Output

    Public Class gridKeyIndices
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            NetMig
            FlowToDet
            NetEff
            OI
        End Enum

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(eVarNameFlags.Name)
            Me(0, eColumnTypes.NetMig) = New cEwEColumnHeaderCell(eVarNameFlags.NetMigration)
            'Me(0, eColumnTypes.NetMig) = New EwEColumnHeaderCell(SharedResources.HEADER_NETMIGRATION_UNIT, cUnitFormatter.CurrencyOverTime)
            Me(0, eColumnTypes.FlowToDet) = New cEwEColumnHeaderCell(eVarNameFlags.FlowToDet)
            'Me(0, eColumnTypes.FlowToDet) = New EwEColumnHeaderCell(SharedResources.HEADER_FLOWTODETR_UNIT, cUnitFormatter.CurrencyOverTime)
            Me(0, eColumnTypes.NetEff) = New cEwEColumnHeaderCell(eVarNameFlags.NetEfficiency)
            'Me(0, eColumnTypes.NetEff) = New EwEColumnHeaderCell(SharedResources.HEADER_NETEFFICIENCY)
            'Me(0, eColumnTypes.OI) = New EwEColumnHeaderCell(SharedResources.HEADER_OMNIVORYINDEX)
            Me(0, eColumnTypes.OI) = New cEwEColumnHeaderCell(eVarNameFlags.OmnivoryIndex)

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True

        End Sub

        Protected Overrides Sub FillData()

            Dim groups As cCoreGroupBase() = Me.StyleGuide.Groups(Me.Core)
            Dim group As cEcopathGroupOutput = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim hgcStanza As cEwEHierarchyGridCell = Nothing
            Dim iStanzaPrev As Integer = -1

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
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New cPropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)

                        iStanzaPrev = group.iStanza
                        iRow = Me.AddRow
                    Else
                        iRow = Me.AddRow(hgcStanza.Row + hgcStanza.NumChildRows + 1)
                    End If

                    'Add row index as stanza child
                    hgcStanza.AddChildRow(iRow)
                    Me.UpdateRow(iRow, group, True)

                End If
            Next i

        End Sub

        Private Sub UpdateRow(iRow As Integer, source As cCoreInputOutputBase, Optional isIndented As Boolean = False)
            Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If

            Me(iRow, eColumnTypes.NetMig) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.NetMigration)
            Me(iRow, eColumnTypes.FlowToDet) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FlowToDet)
            Me(iRow, eColumnTypes.NetEff) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.NetEfficiency)
            Me(iRow, eColumnTypes.OI) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.OmnivoryIndex)
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property

    End Class

End Namespace
