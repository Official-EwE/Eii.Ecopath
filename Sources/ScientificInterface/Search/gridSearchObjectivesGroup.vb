' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwECore.SearchObjectives
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Grid allowing setting of Group search objectives.
    ''' </summary>
    ''' =======================================================================

    Public Class gridSearchObjectivesGroup
        Inherits cEwEGrid

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

        <Browsable(False)>
        Public Property Manager() As ISearchObjective
            Get
                Return Me.m_manager
            End Get
            Set(value As ISearchObjective)
                Me.m_manager = value
                Me.RefreshContent()
            End Set
        End Property

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Group) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUP)
            Me(0, eColumnTypes.ManRB) = New cEwEColumnHeaderCell(SharedResources.HEADER_MANDATED_BIOMASS_RELATIVE)
            Me(0, eColumnTypes.StructureW) = New cEwEColumnHeaderCell(SharedResources.HEADER_STRUCTURERELATIVEWEIGHT)
            Me(0, eColumnTypes.FLimit) = New cEwEColumnHeaderCell(SharedResources.HEADER_MAXFISHINGMORTAILITY)

        End Sub

        Protected Overrides Sub FillData()

            If (Me.Manager Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreGroupBase = Nothing

            For i As Integer = 1 To Me.UIContext.Core.nGroups
                source = Me.m_manager.GroupObjectives(i)

                Me.Rows.Insert(i)
                Me(i, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(i))
                Me(i, eColumnTypes.Group) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                Me(i, eColumnTypes.ManRB) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSGroupMandRelBiom)
                Me(i, eColumnTypes.StructureW) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSGroupStrucRelWeight)
                Me(i, eColumnTypes.FLimit) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSFishingLimit)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
            Me.Columns(0).Width = 20
        End Sub

    End Class

End Namespace


