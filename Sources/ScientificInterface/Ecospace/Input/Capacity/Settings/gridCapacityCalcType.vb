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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to define how capacity is calculated for each group: either derived
    ''' from traditional habitats, or from environmental drivers / capacity input.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)>
    Public Class gridCapacityCalcType
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index
            Name
            Habitat
            EnvResponses
        End Enum

        Private m_lProps As New List(Of cProperty)
        Private m_bInUpdate As Boolean = False

        Public Sub SetAllCalcTypes(type As eEcospaceCapacityCalType, bSet As Boolean)

            For iGroup As Integer = 1 To Core.nGroups
                Dim group As cEcospaceGroupInput = Core.EcospaceGroups(iGroup)
                Select Case type
                    Case eEcospaceCapacityCalType.Input : group.CapacityCalculationType = eEcospaceCapacityCalType.Input
                    Case Else
                        If bSet Then
                            group.CapacityCalculationType = group.CapacityCalculationType Or type
                        Else
                            group.CapacityCalculationType = group.CapacityCalculationType And Not type
                        End If
                End Select
            Next

        End Sub

#Region " Overrides "

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return

            Dim group As cEcospaceGroupInput = Nothing
            Dim map As IEnviroInputData = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            ' Define grid dimensions
            Me.Redim(Core.nGroups + 1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.Habitat) = New EwEColumnHeaderCell(My.Resources.HEADER_USE_HABITAT)
            Me(0, eColumnTypes.EnvResponses) = New EwEColumnHeaderCell(My.Resources.HEADER_USE_ENVRESPONSES)

            Me.m_bInUpdate = True

            For iGroup As Integer = 1 To Core.nGroups

                group = Core.EcospaceGroups(iGroup)

                ' # Group name row header cells
                Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iGroup))
                ' # Group name row header cells
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                Me(iGroup, eColumnTypes.Habitat) = New EwECheckboxCell((group.CapacityCalculationType And eEcospaceCapacityCalType.Habitat) = eEcospaceCapacityCalType.Habitat)
                Me(iGroup, eColumnTypes.Habitat).Behaviors.Add(EwEEditHandler)

                Me(iGroup, eColumnTypes.EnvResponses) = New EwECheckboxCell((group.CapacityCalculationType And eEcospaceCapacityCalType.EnvResponses) = eEcospaceCapacityCalType.EnvResponses)
                Me(iGroup, eColumnTypes.EnvResponses).Behaviors.Add(EwEEditHandler)

                Dim prop As cProperty = Me.PropertyManager.GetProperty(group, eVarNameFlags.EcospaceCapCalType)
                AddHandler prop.PropertyChanged, AddressOf OnPropertyChanged
                Me.m_lProps.Add(prop)

            Next

            Me.m_bInUpdate = False

        End Sub

        Protected Overrides Sub ClearData()
            For Each prop As cProperty In Me.m_lProps
                RemoveHandler prop.PropertyChanged, AddressOf OnPropertyChanged
            Next
            MyBase.ClearData()
        End Sub

        Protected Overrides Sub FillData()
            ' NOP
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumnWidths = False
        End Sub

        Private m_bDirty As Boolean = False

        Protected Overrides Function OnCellValueChanged(p As Position, cell As ICellVirtual) As Boolean

            If (Not Me.m_bInUpdate) Then
                Dim group As cEcospaceGroupInput = Core.EcospaceGroups(p.Row)
                Dim val As eEcospaceCapacityCalType = group.CapacityCalculationType
                Dim bSet As Boolean = CBool(cell.GetValue(p))

                Select Case DirectCast(p.Column, eColumnTypes)

                    Case eColumnTypes.Habitat
                        If bSet Then
                            val = val Or eEcospaceCapacityCalType.Habitat
                        Else
                            val = val And Not eEcospaceCapacityCalType.Habitat
                        End If

                    Case eColumnTypes.EnvResponses
                        If bSet Then
                            val = val Or eEcospaceCapacityCalType.EnvResponses
                        Else
                            val = val And Not eEcospaceCapacityCalType.EnvResponses
                        End If
                End Select
                group.CapacityCalculationType = val
            End If

            Return MyBase.OnCellValueChanged(p, cell)

        End Function

#End Region ' Overrides

#Region " Internals "

        Private Sub OnPropertyChanged(prop As cProperty, cf As cProperty.eChangeFlags)
            Me.UpdateRow(DirectCast(prop.Source, cEcospaceGroupInput))
        End Sub

        Private Sub UpdateRow(grp As cEcospaceGroupInput)

            If (Me.m_bInUpdate) Then Return
            Me.m_bInUpdate = True

            Dim iGroup As Integer = grp.Index
            Dim prop As cProperty = Me.m_lProps(iGroup - 1)
            Dim val As eEcospaceCapacityCalType = DirectCast(prop.GetValue(), eEcospaceCapacityCalType)

            Me(iGroup, eColumnTypes.Habitat).Value = ((val And eEcospaceCapacityCalType.Habitat) = eEcospaceCapacityCalType.Habitat)
            Me.InvalidateCell(Me(iGroup, eColumnTypes.Habitat))

            Me(iGroup, eColumnTypes.EnvResponses).Value = ((val And eEcospaceCapacityCalType.EnvResponses) = eEcospaceCapacityCalType.EnvResponses)
            Me.InvalidateCell(Me(iGroup, eColumnTypes.EnvResponses))

            Me.m_bInUpdate = False

        End Sub

#End Region ' Internals

    End Class

End Namespace
