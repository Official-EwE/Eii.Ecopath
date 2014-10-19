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

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.BehaviorModels

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Grid control, implements the Ecospace interface to assign species to habitats.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridHabitatPreference
        : Inherits EwEGrid

#Region " Private vars "

        Private m_lProps As New List(Of cProperty)

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Construction / destruction

#Region " Overrides "

        Protected Overrides Sub InitStyle()

            'Call base class InitStyle method. 
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(Me.Core.nGroups + 2, Me.Core.nHabitats + 2)

            'Set header cells # (0,0)
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_HEADER_GROUP_HABITAT)
            Me(0, 0).ColumnSpan = 2

            'Dynamic row header - group name 
            For i As Integer = 1 To Me.Core.nGroups
                source = Me.Core.EcospaceGroups(i)
                Me(i, 0) = New EwERowHeaderCell(CStr(i))
                ' # Group name row header cells
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            'Row header cell - Habitat area
            Me(Me.Core.nGroups + 1, 0) = New EwERowHeaderCell(CStr(Me.Core.nGroups + 1))
            Me(Me.Core.nGroups + 1, 1) = New EwERowHeaderCell(My.Resources.ECOSPACE_HEADER_HABITAT_AREA)

            'Dynamic column header - Habitat name
            For j As Integer = 0 To Me.Core.nHabitats - 1
                source = Me.Core.EcospaceHabitats(j)
                ' +1 to compensate for header column, +1 to compensate for zero-based habitat index.
                Me(0, j + 2) = New EwEColumnHeaderCell(source.Name)
            Next

            Me.FixedColumns = 2
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim groupEcospace As cEcospaceGroup = Nothing
            Dim groupEcopath As cEcoPathGroupInput = Nothing
            Dim hab As cEcospaceHabitat = Nothing
            Dim cell As EwECellBase = Nothing

            For iGroup As Integer = 1 To Me.Core.nGroups

                ' Get sources
                groupEcospace = Me.Core.EcospaceGroups(iGroup)
                groupEcopath = Me.Core.EcoPathGroupInputs(iGroup)

                For iHabitat As Integer = 0 To Me.Core.nHabitats - 1

                    hab = Me.Core.EcospaceHabitats(iHabitat)

                    ' Create proportion cell (was checkbox)
                    cell = New PropertyCell(Me.PropertyManager, groupEcospace, eVarNameFlags.PreferredHabitat, hab)
                    cell.Behaviors.Add(Me.EwEEditHandler)
                    cell.SuppressZero = True
                    Me(iGroup, iHabitat + 2) = cell

                Next

                Dim prop As cProperty = Me.PropertyManager.GetProperty(groupEcospace, eVarNameFlags.EcospaceCapCalType)
                Me.m_lProps.Add(prop)
                AddHandler prop.PropertyChanged, AddressOf OnPropertyChanged

                Me.UpdateRow(groupEcospace)

            Next

        End Sub

        Protected Overrides Sub ClearData()
            For Each prop As cProperty In Me.m_lProps
                RemoveHandler prop.PropertyChanged, AddressOf OnPropertyChanged
            Next
            MyBase.ClearData()
        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSpace}
            End Get
        End Property

#End Region ' Overrides

#Region " Internals "

        Private Sub OnPropertyChanged(prop As cProperty, cf As cProperty.eChangeFlags)
            Me.UpdateRow(DirectCast(prop.Source, cEcospaceGroup))
        End Sub

        Private Sub UpdateRow(grp As cEcospaceGroup)

            Dim iGroup As Integer = grp.Index
            Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
            Dim mapManager As cMapResponseInteractionManager = Core.CapacityMapInteractionManager

            If (grp.CapacityCalculationType = eEcospaceCapacityCalType.Capacity) Then
                style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
            End If

            For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                Dim cell As EwECellBase = CType(Me(iGroup, 2 + iHabitat), EwECellBase)
                cell.Style = style
                Me.InvalidateCell(cell)
            Next

        End Sub

#End Region ' Internals

    End Class

End Namespace

