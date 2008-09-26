'==============================================================================
'
' $Log: EcotracerInputGrid.vb,v $
' Revision 1.1  2008/09/26 07:32:03  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/08/02 03:04:16  jeroens
' Renamed resources
'
' Revision 1.11  2008/07/29 13:06:45  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.10  2008/06/02 00:01:35  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.9  2008/05/29 22:22:55  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.8  2008/04/07 02:31:15  jeroens
' Cleaning up resources
'
' Revision 1.7  2008/01/08 11:24:34  jeroens
' Merged input parms and group grid in one screen
'
' Revision 1.6  2008/01/06 11:02:24  jeroens
' * Fixed env input cells r/o state
'
' Revision 1.5  2008/01/03 17:40:41  joeb
' Renamed excretion rate column
'
' Revision 1.4  2007/12/29 20:29:06  joeb
' Changed Lipophilic to Excretion
'
' Revision 1.3  2007/12/21 18:08:48  jeroens
' * Hacked environment into grid
'
' Revision 1.2  2007/12/21 15:32:03  jeroens
' * Implemented
'
' Revision 1.1  2007/12/05 03:54:04  jeroens
' * Initial version
'
' Revision 1.1  2007/11/06 19:27:55  jeroens
' * Moved
'
' Revision 1.1  2007/11/06 03:14:15  jeroens
' + Added as prototype
'
'==============================================================================

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
                Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(group, eVarNameFlags.Index)
                Me(iRow, eColumnTypes.GroupName) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)
                Me(iRow, eColumnTypes.ConcEnv) = New PropertyCell(group, eVarNameFlags.CZero)
                Me(iRow, eColumnTypes.ConcImmBiomass) = New PropertyCell(group, eVarNameFlags.CImmig)
                Me(iRow, eColumnTypes.DirectAbsorptionRate) = New PropertyCell(group, eVarNameFlags.CEnvironment)
                Me(iRow, eColumnTypes.DecayRate) = New PropertyCell(group, eVarNameFlags.CDecay)
                Me(iRow, eColumnTypes.ExcretionRate) = New PropertyCell(group, eVarNameFlags.CExcretionRate)

            Next iGroup

        End Sub

    End Class

End Namespace ' Ecotracer
