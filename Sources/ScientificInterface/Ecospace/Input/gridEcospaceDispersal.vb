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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.BehaviorModels

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Grid control, implements the Ecospace interface to set dispersal rates.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridEcospaceDispersal
        : Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            DispersalRate
            RelDisp
            RelVul
            RelFeedRate
            Advected
            Migrating
            NSCont
            EWCont
            BarrierAvoidance
        End Enum

        Private m_lProps As New List(Of cProperty)

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Construction / destruction

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            'Add column headers
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.DispersalRate) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_BASEDISPRATE)
            Me(0, eColumnTypes.RelDisp) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_RELDISP)
            Me(0, eColumnTypes.RelVul) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_RELVUL)
            Me(0, eColumnTypes.RelFeedRate) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_RELFEEDRATE)
            Me(0, eColumnTypes.Advected) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_ADVECTED)
            Me(0, eColumnTypes.Migrating) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_MIGRATING)
            Me(0, eColumnTypes.NSCont) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_NSCONT)
            Me(0, eColumnTypes.EWCont) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_EWCONT)
            Me(0, eColumnTypes.BarrierAvoidance) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_BARRIERAVOIDANCEWT)

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cEcospaceGroup = Nothing
            Dim cell As EwECellBase = Nothing

            For iGroup As Integer = 1 To Me.Core.nGroups
                Me.Rows.Insert(iGroup)

                source = Me.Core.EcospaceGroups(iGroup)
                Me(iGroup, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

                'MVel - Base dispersal rate
                cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MVel)
                cell.SuppressZero = False
                Me(iGroup, eColumnTypes.DispersalRate) = cell
                'Rel dispersal in bad habitat
                Me(iGroup, eColumnTypes.RelDisp) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.RelMoveBad)
                ' Rel. vul.to pred. in bad habitat
                Me(iGroup, eColumnTypes.RelVul) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.RelVulBad)
                'Rel. feed.rate in bad habitat
                Me(iGroup, eColumnTypes.RelFeedRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EatEffBad)
                'Advected?
                Me(iGroup, eColumnTypes.Advected) = New PropertyCheckboxCell(Me.PropertyManager, source, eVarNameFlags.IsAdvected)
                'Migrating?
                Me(iGroup, eColumnTypes.Migrating) = New PropertyCheckboxCell(Me.PropertyManager, source, eVarNameFlags.IsMigratory)
                'North/south concentration
                Me(iGroup, eColumnTypes.NSCont) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MigrationConcRow)
                'East/west concentration
                Me(iGroup, eColumnTypes.EWCont) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MigrationConcCol)
                'Barrier avoidance weight
                Me(iGroup, eColumnTypes.BarrierAvoidance) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BarrierAvoidanceWeight)

                Me.UpdateRow(source)

                ' ToDo: solve this with core status flags: Set_BadHab_Flags
                Dim prop As cProperty = Me.PropertyManager.GetProperty(source, eVarNameFlags.EcospaceCapCalType)
                Me.m_lProps.Add(prop)
                AddHandler prop.PropertyChanged, AddressOf OnPropertyChanged

            Next

        End Sub

        Protected Overrides Sub ClearData()
            For Each prop As cProperty In Me.m_lProps
                RemoveHandler prop.PropertyChanged, AddressOf OnPropertyChanged
            Next
            Me.m_lProps.Clear()
            MyBase.ClearData()
        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                ' Refresh on Ecopath notifications
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSpace}
            End Get
        End Property

        Private Sub OnPropertyChanged(prop As cProperty, cf As cProperty.eChangeFlags)
            Me.UpdateRow(DirectCast(prop.Source, cEcospaceGroup))
        End Sub

        Private Sub UpdateRow(grp As cEcospaceGroup)

            Dim iGroup As Integer = grp.Index
            Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
            Dim mapManager As cMapResponseInteractionManager = Core.CapacityMapInteractionManager

            Dim cols As eColumnTypes() = New eColumnTypes() {eColumnTypes.RelDisp, eColumnTypes.RelVul, eColumnTypes.RelFeedRate}

            If (grp.CapacityCalculationType = eEcospaceCapacityCalType.EnvResponses) Then
                style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
            End If

            For Each col As eColumnTypes In cols
                Dim cell As EwECellBase = CType(Me(iGroup, col), EwECellBase)
                cell.Style = style
                Me.InvalidateCell(cell)
            Next

        End Sub

    End Class

End Namespace
