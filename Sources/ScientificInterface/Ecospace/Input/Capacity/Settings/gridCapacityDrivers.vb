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
    ''' Grid to define which drivers are used to calculate capacity.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class gridCapacityDrivers
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index
            Name
            Driver
        End Enum

        Private m_bInUpdate As Boolean = False

#Region " Overrides "

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return

            Dim group As cEcospaceGroup = Nothing
            Dim map As IEnviroInputMap = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            ' Define grid dimensions
            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell("Map layer")
            Me(0, eColumnTypes.Driver) = New EwEColumnHeaderCell("Use for capacity calculations")

        End Sub

        Protected Overrides Sub FillData()

            Me.RowsCount = 1

            Dim mapManager As cMapResponseInteractionManager = Core.CapacityMapInteractionManager
            Dim map As IEnviroInputMap = Nothing
            Dim layer As cEcospaceLayer = Nothing
            Dim iRow As Integer = 0

            For iMap As Integer = 1 To mapManager.nMaps

                map = mapManager.Map(iMap)
                layer = map.Layer

                iRow = Me.AddRow()

                Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(map.Layer.Index))
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, DirectCast(map, cEnviroInputMap).Layer, eVarNameFlags.Name)
                Me(iRow, eColumnTypes.Driver) = New EwECheckboxCell(layer.IsActive)
                Me(iRow, eColumnTypes.Driver).Behaviors.Add(Me.EwEEditHandler)
                Me.Rows(iRow).Tag = layer
            Next iMap

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
                        Case eColumnTypes.Driver
                            Dim layer As cEcospaceLayer = DirectCast(Me.Rows(p.Row).Tag, cEcospaceLayer)
                            layer.IsActive = CBool(Me(p.Row, p.Column).Value)
                            Me.Core.onChanged(layer)
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

        'Private Sub OnPropertyChanged(prop As cProperty, cf As cProperty.eChangeFlags)
        '    Me.UpdateRow(DirectCast(prop.Source, cEcospaceGroup))
        'End Sub

        'Private Sub UpdateRow(grp As cEcospaceGroup)

        '    Dim iGroup As Integer = grp.Index

        '    Me(iGroup, eColumnTypes.FromHabitat).Value = (grp.CapacityCalculationType = eEcospaceCapacityCalType.Habitat)
        '    Me.InvalidateCell(Me(iGroup, eColumnTypes.FromHabitat))

        '    Me(iGroup, eColumnTypes.FromEnvDrivers).Value = (grp.CapacityCalculationType = eEcospaceCapacityCalType.Capacity)
        '    Me.InvalidateCell(Me(iGroup, eColumnTypes.FromEnvDrivers))

        'End Sub

#End Region ' Internals

    End Class

End Namespace
