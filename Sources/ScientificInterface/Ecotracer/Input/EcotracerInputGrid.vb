#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecotracer

    <CLSCompliant(False)> _
    Public Class EcotracerInputGrid
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            GroupName
            ConcEnv
            ConcImmBiomass
            DirectAbsorptionRate
            DecayRate
            ExcretionRate
        End Enum

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

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
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.GroupName) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.ConcEnv) = New EwEColumnHeaderCell(SharedResources.HEADER_CONCENTRATION_INITIAL)
            Me(0, eColumnTypes.ConcImmBiomass) = New EwEColumnHeaderCell(SharedResources.HEADER_CONCENTRATION_IN_IMM_B)
            Me(0, eColumnTypes.DirectAbsorptionRate) = New EwEColumnHeaderCell(SharedResources.HEADER_DIRECT_ABSORPTION_RATE_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.DecayRate) = New EwEColumnHeaderCell(SharedResources.HEADER_DECAY_RATE_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.ExcretionRate) = New EwEColumnHeaderCell(SharedResources.HEADER_EXCRETION_RATE)

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
                Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Index)
                Me(iRow, eColumnTypes.GroupName) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
                Me(iRow, eColumnTypes.ConcEnv) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.CZero)
                Me(iRow, eColumnTypes.ConcImmBiomass) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.CImmig)
                Me(iRow, eColumnTypes.DirectAbsorptionRate) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.CEnvironment)
                Me(iRow, eColumnTypes.DecayRate) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.CDecay)
                Me(iRow, eColumnTypes.ExcretionRate) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.CExcretionRate)

            Next iGroup

        End Sub

    End Class

End Namespace ' Ecotracer
