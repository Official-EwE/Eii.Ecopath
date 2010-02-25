Option Strict On
Imports EwECore
Imports EwEUtils.Core

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

            ' ToDo_JS: globalize this method

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing

            ' Define grid dimensions
            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Dim rowCnt As Integer = Me.RowsCount
            ' Set header cells
            ' # (0,0)
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.GroupName) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.ConcEnv) = New EwEColumnHeaderCell("Initial conc. (t/t)")
            Me(0, eColumnTypes.ConcImmBiomass) = New EwEColumnHeaderCell("Conc. in immigrating biomass (t/t)")
            Me(0, eColumnTypes.DirectAbsorptionRate) = New EwEColumnHeaderCell("Direct absorption rate (t/t/t/year)")
            Me(0, eColumnTypes.DecayRate) = New EwEColumnHeaderCell("Decay rate (t/year)")
            Me(0, eColumnTypes.ExcretionRate) = New EwEColumnHeaderCell("Prop. 1-GS excreted (0-1)")

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim group As cEcotracerGroupInput = Nothing
            Dim iRow As Integer = -1
            'Dim c As EwECell = Nothing

            ' Remove existing rows
            Me.RowsCount = 1

            'iRow = Me.AddRow()
            'Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell("")
            'Me(iRow, eColumnTypes.GroupName) = New EwERowHeaderCell(My.Resources.HEADER_ENVIRONMENT)
            'Me(iRow, eColumnTypes.ConcEnv) = New PropertyCell(parms, eVarNameFlags.CZero)
            'Me(iRow, eColumnTypes.ConcImmBiomass) = New PropertyCell(parms, eVarNameFlags.CInflow)
            'Me(iRow, eColumnTypes.DirectAbsorptionRate) = New PropertyCell(parms, eVarNameFlags.COutflow)
            'Me(iRow, eColumnTypes.DecayRate) = New PropertyCell(parms, eVarNameFlags.CDecay)

            'c = New EwECell(0, GetType(Single))
            'c.Style = StyleGuide.eStyleFlags.NotEditable
            'Me(iRow, eColumnTypes.ExcretionRate) = c

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
