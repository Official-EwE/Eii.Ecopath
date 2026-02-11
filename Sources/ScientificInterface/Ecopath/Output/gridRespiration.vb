' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Output

    Public Class gridRespiration
        Inherits cEwEGrid

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

            Me.Redim(1, 7)

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, 2) = New cEwEColumnHeaderCell(eVarNameFlags.Respiration)
            Me(0, 3) = New cEwEColumnHeaderCell(eVarNameFlags.Assimilation)
            Me(0, 4) = New cEwEColumnHeaderCell(eVarNameFlags.RespAssim)
            Me(0, 5) = New cEwEColumnHeaderCell(eVarNameFlags.ProdResp)
            Me(0, 6) = New cEwEColumnHeaderCell(eVarNameFlags.RespBiom)

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
                    Me.FillInRows(iRow, group)

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

                    'Display group info
                    hgcStanza.AddChildRow(iRow)
                    Me.FillInRows(iRow, group, True)
                End If
            Next i

        End Sub

        Private Sub FillInRows(iRow As Integer, source As cCoreInputOutputBase, Optional isIndented As Boolean = False)
            Me(iRow, 0) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New cPropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If
            Me(iRow, 2) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.Respiration)
            Me(iRow, 3) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.Assimilation)
            Me(iRow, 4) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.RespAssim)
            Me(iRow, 5) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.ProdResp)
            Me(iRow, 6) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.RespBiom)
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property

    End Class

End Namespace

