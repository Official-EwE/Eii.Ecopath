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
Imports EwEUtils.SystemUtilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to define how capacity is calculated for each group: either derived
    ''' from traditional habitats, or from environmental drivers / capacity input.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class gridCapacityCalcType
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index
            Name
            Habitat
            EnvDrivers
#If DEBUG Then
            Both
#End If
        End Enum

        Private m_lProps As New List(Of cProperty)
        Private m_bInUpdate As Boolean = False

#Region " Overrides "

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' ToDo: globalize this

            If (Me.UIContext Is Nothing) Then Return

            Dim group As cEcospaceGroup = Nothing
            Dim map As IEnviroInputMap = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            ' Define grid dimensions
            Me.Redim(Core.nGroups + 1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.Habitat) = New EwEColumnHeaderCell(My.Resources.HEADER_USE_HABITAT)
            Me(0, eColumnTypes.EnvDrivers) = New EwEColumnHeaderCell(My.Resources.HEADER_USE_ENVRESPONSES)
#If DEBUG Then
            Me(0, eColumnTypes.Both) = New EwEColumnHeaderCell("Both")
#End If
            For iGroup As Integer = 1 To Core.nGroups

                group = Core.EcospaceGroups(iGroup)

                ' # Group name row header cells
                Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iGroup))
                ' # Group name row header cells
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                Me(iGroup, eColumnTypes.Habitat) = New EwECheckboxCell(False)
                Me(iGroup, eColumnTypes.Habitat).Behaviors.Add(EwEEditHandler)

                Me(iGroup, eColumnTypes.EnvDrivers) = New EwECheckboxCell(False)
                Me(iGroup, eColumnTypes.EnvDrivers).Behaviors.Add(EwEEditHandler)

#If DEBUG Then
                Me(iGroup, eColumnTypes.Both) = New EwECheckboxCell(False)
                Me(iGroup, eColumnTypes.Both).Behaviors.Add(EwEEditHandler)
#End If

                Dim prop As cProperty = Me.PropertyManager.GetProperty(group, eVarNameFlags.EcospaceCapCalType)
                Me.m_lProps.Add(prop)
                AddHandler prop.PropertyChanged, AddressOf OnPropertyChanged

                Me.UpdateRow(group)

            Next

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

        Protected Overrides Function OnCellValueChanged(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

            If (Not Me.m_bInUpdate) Then
                Me.m_bInUpdate = True
                Try
                    Select Case DirectCast(p.Column, eColumnTypes)
                        Case eColumnTypes.Habitat
                            If (CBool(cell.GetValue(p))) Then Me.m_lProps(p.Row - 1).SetValue(eEcospaceCapacityCalType.Habitat)

                        Case eColumnTypes.EnvDrivers
                            If (CBool(cell.GetValue(p))) Then Me.m_lProps(p.Row - 1).SetValue(eEcospaceCapacityCalType.EnvResponses)

#If DEBUG Then
                        Case eColumnTypes.Both
                            If (CBool(cell.GetValue(p))) Then Me.m_lProps(p.Row - 1).SetValue(eEcospaceCapacityCalType.Both)
#End If

                    End Select

                Catch ex As Exception
                    Debug.Assert(False)
                End Try
                Me.m_bInUpdate = False
            End If

            Return MyBase.OnCellValueChanged(p, cell)
        End Function

#End Region ' Overrides

#Region " Internals "

        Private Sub OnPropertyChanged(prop As cProperty, cf As cProperty.eChangeFlags)
            Me.UpdateRow(DirectCast(prop.Source, cEcospaceGroup))
        End Sub

        Private Sub UpdateRow(grp As cEcospaceGroup)

            Dim iGroup As Integer = grp.Index

            Me(iGroup, eColumnTypes.Habitat).Value = (grp.CapacityCalculationType = eEcospaceCapacityCalType.Habitat)
            Me.InvalidateCell(Me(iGroup, eColumnTypes.Habitat))

            Me(iGroup, eColumnTypes.EnvDrivers).Value = (grp.CapacityCalculationType = eEcospaceCapacityCalType.EnvResponses)
            Me.InvalidateCell(Me(iGroup, eColumnTypes.EnvDrivers))

#If DEBUG Then
            Me(iGroup, eColumnTypes.Both).Value = (grp.CapacityCalculationType = eEcospaceCapacityCalType.Both)
            Me.InvalidateCell(Me(iGroup, eColumnTypes.Both))
#End If

        End Sub

#End Region ' Internals

    End Class

End Namespace
