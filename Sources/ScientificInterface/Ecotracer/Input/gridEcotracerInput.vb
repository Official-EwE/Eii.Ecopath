' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecotracer

    Public Class gridEcotracerInput
        Inherits cEwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            GroupName
            ConcEnv
            ConcImmBiomass
            DirectAbsorptionRate
            PhysicalDecayRate
            AssimProp
            MetablismRate
        End Enum

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim core As cCore = Me.UIContext.Core
            Dim source As cCoreGroupBase = Nothing

            ' Define grid dimensions
            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Dim rowCnt As Integer = Me.RowsCount
            ' Set header cells
            ' # (0,0)
            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.GroupName) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.ConcEnv) = New cEwEColumnHeaderCell(eVarNameFlags.CZero)
            Me(0, eColumnTypes.ConcImmBiomass) = New cEwEColumnHeaderCell(eVarNameFlags.CImmig)
            Me(0, eColumnTypes.DirectAbsorptionRate) = New cEwEColumnHeaderCell(SharedResources.HEADER_DIRECT_ABSORPTION_RATE)
            Me(0, eColumnTypes.PhysicalDecayRate) = New cEwEColumnHeaderCell(SharedResources.HEADER_PHYSICAL_DECAY_RATE)
            Me(0, eColumnTypes.AssimProp) = New cEwEColumnHeaderCell(SharedResources.HEADER_EXCRETION_RATE)
            Me(0, eColumnTypes.MetablismRate) = New cEwEColumnHeaderCell(SharedResources.HEADER_METABOLISMRATE)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = Me.UIContext.Core
            Dim group As cEcotracerGroupInput = Nothing
            Dim iRow As Integer = -1

            ' Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For iGroup As Integer = 1 To core.nGroups

                group = core.EcotracerGroupInputs(iGroup)

                iRow = Me.AddRow()
                Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Index)
                Me(iRow, eColumnTypes.GroupName) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
                Me(iRow, eColumnTypes.ConcEnv) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.CZero)
                Me(iRow, eColumnTypes.ConcImmBiomass) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.CImmig)
                Me(iRow, eColumnTypes.DirectAbsorptionRate) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.CEnvironment)
                Me(iRow, eColumnTypes.PhysicalDecayRate) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.CPhysicalDecayRate)
                Me(iRow, eColumnTypes.AssimProp) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.CAssimilationProp)
                Me(iRow, eColumnTypes.MetablismRate) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.CMetablismRate)

            Next iGroup

        End Sub

    End Class

End Namespace ' Ecotracer
