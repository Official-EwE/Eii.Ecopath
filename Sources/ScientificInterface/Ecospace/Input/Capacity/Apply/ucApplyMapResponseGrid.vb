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

''' <summary>
''' Grid to configure the environmental response 
''' </summary>
''' <remarks></remarks>
<CLSCompliant(False)> _
Public Class ucApplyMapResponseGrid
    Inherits Ecosim.ApplyShapeGrid

#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        If (Me.UIContext Is Nothing) Then Return

        Dim group As cCoreGroupBase = Nothing
        Dim mapManager As cMapResponseInteractionManager = Core.CapacitMapInteractionManager
        Dim src As cCoreInputOutputBase = Nothing
        Dim fmt As New cCoreInterfaceFormatter()

        ' Define grid dimensions
        Me.Redim(Core.nGroups + 1, mapManager.nMaps + 2)

        For imap As Integer = 1 To mapManager.nMaps

            src = DirectCast(mapManager.Map(imap), cCoreInputOutputBase)
            Me(0, 1 + imap) = New PropertyColumnHeaderCell(Me.PropertyManager, src, eVarNameFlags.Name)
            Me(0, 1 + imap).Behaviors.Add(Me.m_RowColClick)

        Next imap


        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

        For iGroup As Integer = 1 To Core.nGroups
            group = Core.EcoPathGroupInputs(iGroup)
            ' # Group name row header cells
            Me(iGroup, 0) = New EwERowHeaderCell(CStr(iGroup))
            Me(iGroup, 0).Behaviors.Add(Me.m_RowColClick)

            ' # Group name row header cells
            Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
            Me(iGroup, 1).Behaviors.Add(Me.m_RowColClick)
        Next

    End Sub

    Protected Overrides Sub FillData()

        Try
            Dim Manager As cMapResponseInteractionManager = Core.CapacitMapInteractionManager
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
                    Me(igrp, imap + 1).Behaviors.Add(Me.m_BehaviorClick)

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

    End Sub

    Public Overrides Sub SetAllPairs()

    End Sub

#End Region ' Overrides

#Region " Internals "

    Protected Overrides Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)

        Try

            Dim igrp As Integer = e.Position.Row
            Dim iMap As Integer = e.Position.Column - 1

            Me.showSelectionDialog(dlgSelectResponse.eSelectionType.MapGroup, igrp, iMap)

        Catch ex As Exception
            ' Whoah
        End Try

    End Sub


    Private Sub showSelectionDialog(ByVal SelectionType As dlgSelectResponse.eSelectionType, ByVal iGrp As Integer, ByVal iMap As Integer)
        Try
            Dim MapManager As cMapResponseInteractionManager = Core.CapacitMapInteractionManager
            Dim ShapeManager As cBaseShapeManager = Core.CapacityShapeManager

            Dim dlg As New dlgSelectResponse(Me.UIContext, ShapeManager, MapManager, iMap, igrp, SelectionType)
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

            Me.showSelectionDialog(selectionType, igrp, iMap)

        Catch ex As Exception

        End Try
      
    End Sub

#End Region ' Internals

End Class
