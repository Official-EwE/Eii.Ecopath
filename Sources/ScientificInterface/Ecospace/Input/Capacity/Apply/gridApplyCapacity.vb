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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Text
Imports System.Globalization
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceLibrary

#End Region

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to apply environmental response functions to capacity maps.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class ucApplyMapResponseGrid
        Inherits Ecosim.gridApplyShapeBase

#Region " Overrides "

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return

            Dim group As cCoreGroupBase = Nothing
            Dim mapManager As cMapResponseInteractionManager = Core.CapacityMapInteractionManager
            Dim map As IEnviroInputMap = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            ' Define grid dimensions
            Me.Redim(Core.nGroups + 1, mapManager.nMaps + 2)

            For iMap As Integer = 1 To mapManager.nMaps

                map = mapManager.Map(iMap)
                Me(0, 1 + iMap) = New PropertyColumnHeaderCell(Me.PropertyManager, DirectCast(map, cEnviroInputMap).Layer, eVarNameFlags.Name)
                Me(0, 1 + iMap).Behaviors.Add(Me.m_bmRowCol)

            Next iMap

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            For iGroup As Integer = 1 To Core.nGroups
                group = Core.EcoPathGroupInputs(iGroup)
                ' # Group name row header cells
                Me(iGroup, 0) = New EwERowHeaderCell(CStr(iGroup))
                Me(iGroup, 0).Behaviors.Add(Me.m_bmRowCol)

                ' # Group name row header cells
                Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
                Me(iGroup, 1).Behaviors.Add(Me.m_bmRowCol)
            Next

        End Sub

        Protected Overrides Sub FillData()

            Try
                Dim Manager As cMapResponseInteractionManager = Core.CapacityMapInteractionManager
                Dim ShapeManager As cCapMapResponseManager = Me.Core.CapacityShapeManager
                Dim ff As cForcingFunction
                Dim label As String

                For imap As Integer = 1 To Manager.nMaps
                    Dim map As IEnviroInputMap = Manager.Map(imap)
                    For igrp As Integer = 1 To Core.nGroups
                        label = ""
                        Dim ishp As Integer = map.ResponseIndexForGroup(igrp)
                        If ishp > 0 Then
                            ff = ShapeManager.Item(ishp - 1)
                            label = String.Format(SharedResources.GENERIC_LABEL_INDEXED, ff.Index, ff.Name)
                        End If

                        Me(igrp, imap + 1) = New Cells.Real.Cell(label)
                        Me(igrp, imap + 1).DataModel = Me.m_editor
                        Me(igrp, imap + 1).Behaviors.Add(Me.m_bmCell)

                    Next

                Next
            Catch ex As Exception

            End Try


        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            'Me.FixedColumnWidths = False
        End Sub

        Public Overrides Sub ClearAllPairs()
            ' NOP
        End Sub

        Public Overrides Sub SetAllPairs()
            ' NOP
        End Sub

#End Region ' Overrides

#Region " Internals "

        Protected Overrides Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)

            Try

                Dim iGrp As Integer = e.Position.Row
                Dim iMap As Integer = e.Position.Column - 1

                Me.ShowSelectionDialog(dlgSelectResponse.eSelectionType.MapGroup, iGrp, iMap)

            Catch ex As Exception
                ' Whoah
            End Try

        End Sub

        Private Sub ShowSelectionDialog(ByVal SelectionType As dlgSelectResponse.eSelectionType, ByVal iGrp As Integer, ByVal iMap As Integer)
            Try
                Dim MapManager As cMapResponseInteractionManager = Core.CapacityMapInteractionManager
                Dim ShapeManager As cBaseShapeManager = Core.CapacityShapeManager

                Dim dlg As New dlgSelectResponse(Me.UIContext, ShapeManager, MapManager, iMap, iGrp, SelectionType)
                dlg.ShowDialog()
                If dlg.DialogResult = DialogResult.OK Then
                    'the dialogue will update the CapacitMapInteractionManager with the selected Shapes
                    'update the interface from the CapacitMapInteractionManager data
                    Me.FillData()
                End If

            Catch ex As Exception

            End Try
        End Sub

        Protected Overrides Sub OnRowColClicked(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
            Try

                Dim igrp As Integer = e.Position.Row
                Dim iMap As Integer = e.Position.Column - 1
                'just assume it is the column that the user has selected!!!
                Dim selectionType As dlgSelectResponse.eSelectionType = dlgSelectResponse.eSelectionType.Map
                If iMap < 0 Then
                    'the user has selected a Row not the Col(as set above)
                    selectionType = dlgSelectResponse.eSelectionType.Group
                End If

                Me.ShowSelectionDialog(selectionType, igrp, iMap)

            Catch ex As Exception

            End Try

        End Sub

#End Region ' Internals

    End Class

End Namespace
