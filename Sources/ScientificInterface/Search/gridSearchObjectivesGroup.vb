#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.SearchObjectives
Imports EwEUtils.Core
Imports System.ComponentModel

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Grid allowing setting of Group search objectives.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridSearchObjectivesGroup
        : Inherits EwEGrid

        Private m_manager As ISearchObjective

        Private Enum eColumnTypes As Integer
            Index = 0
            Group
            ManRB
            StructureW
            FLimit
        End Enum

        Public Sub New()
            MyBase.New()
        End Sub

        <Browsable(False)> _
        Public Property Manager() As ISearchObjective
            Get
                Return Me.m_manager
            End Get
            Set(ByVal value As ISearchObjective)
                Me.m_manager = value
                Me.RefreshContent()
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Group) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUP)
            Me(0, eColumnTypes.ManRB) = New EwEColumnHeaderCell(My.Resources.HEADER_MANDATED_BIOMASS_RELATIVE)
            Me(0, eColumnTypes.StructureW) = New EwEColumnHeaderCell(My.Resources.HEADER_STRUCTURERELATIVEWEIGHT)
            Me(0, eColumnTypes.FLimit) = New EwEColumnHeaderCell(My.Resources.HEADER_MAXFISHINGMORTAILITY)

        End Sub

        Protected Overrides Sub FillData()

            If (Me.Manager Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreGroupBase = Nothing

            For i As Integer = 1 To Me.UIContext.Core.nGroups
                source = m_manager.GroupObjectives(i)

                Me.Rows.Insert(i)
                Me(i, eColumnTypes.Index) = New EwERowHeaderCell(i)
                Me(i, eColumnTypes.Group) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                Me(i, eColumnTypes.ManRB) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSGroupMandRelBiom)
                Me(i, eColumnTypes.StructureW) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSGroupStrucRelWeight)
                Me(i, eColumnTypes.FLimit) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSFishingLimit)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
        End Sub

    End Class

End Namespace


