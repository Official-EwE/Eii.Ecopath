'==============================================================================
'
' $Log: gridSearchObjectivesGroup.vb,v $
' Revision 1.2  2008/11/19 14:46:10  jeroens
' Renamed a few resources
'
' Revision 1.1  2008/11/12 21:37:32  jeroens
' Renamed, moved
'
'==============================================================================

#Region "Imports directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.SearchObjectives
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class gridSearchObjectivesGroup
        : Inherits EwEGrid

        Private m_core As cCore
        Private m_manager As ISearchObjective

        Private Enum eColumnTypes As Integer
            Group = 0
            ManRB
            StructureW
        End Enum

        Public Sub New(ByVal Manager As ISearchObjective)
            MyBase.New()
            Me.m_core = cCore.GetInstance()
            Me.m_manager = Manager
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Group) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUP)
            Me(0, eColumnTypes.ManRB) = New EwEColumnHeaderCell(My.Resources.HEADER_MANDATED_BIOMASS_RELATIVE)
            Me(0, eColumnTypes.StructureW) = New EwEColumnHeaderCell(My.Resources.HEADER_STRUCTURERELATIVEWEIGHT)

            'Me(0, iColumn) = New EwEColumnHeaderCell(My.Resources.FPS_GROUP_MAX_FM)

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreGroupBase = Nothing

            For i As Integer = 1 To m_core.nGroups
                source = m_manager.GroupObjectives(i)

                Me.Rows.Insert(i)
                Me(i, eColumnTypes.Group) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                Me(i, eColumnTypes.ManRB) = New PropertyCell(source, eVarNameFlags.FPSGroupMandRelBiom)
                Me(i, eColumnTypes.StructureW) = New PropertyCell(source, eVarNameFlags.FPSGroupStrucRelWeight)

                'Me(i, iColumn) = New PropertyCell(source, eVarNameFlags.FPSGroupMaxMort)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
        End Sub

        Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
            Return DockStyle.Fill
        End Function

    End Class

End Namespace


