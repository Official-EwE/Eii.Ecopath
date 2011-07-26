
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

<CLSCompliant(False)> _
Public Class ucApplyMapResponseGrid
    Inherits Ecosim.ApplyShapeGrid


#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        If (Me.UIContext Is Nothing) Then Return

        Dim group As cCoreGroupBase = Nothing
        Dim map As cMapResponseInteractionManager = Core.MapInteractionManager

        ' Define grid dimensions
        Me.Redim(Core.nLivingGroups + 1, map.nMaps + 2)

        For imap As Integer = 1 To map.nMaps

            Me(0, 1 + imap) = New EwEColumnHeaderCell(map.Maps(imap).Name)
            Me(0, 1 + imap).Behaviors.Add(Me.m_RowColClick)

        Next imap


        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        For iGroup As Integer = 1 To Core.nLivingGroups
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
            Dim Manager As cMapResponseInteractionManager = Core.MapInteractionManager

            For imap As Integer = 1 To Manager.nMaps
                Dim map As IEnviroInputMap = Manager.Maps(imap)
                For igrp As Integer = 1 To Me.Core.nLivingGroups

                    Me(igrp, imap + 1) = New Cells.Real.Cell(map.ResponseIndexForGroup(igrp))
                    Me(igrp, imap + 1).DataModel = Me.m_editor
                    Me(igrp, imap + 1).Behaviors.Add(Me.m_BehaviorClick)

                Next

            Next
        Catch ex As Exception

        End Try

        'Dim ff As cForcingFunction = Nothing
        'Dim interaction As cMediatedInteraction = Nothing
        'Dim cellBlocked As EwECell = Nothing
        'Dim fmt As New cMonetaryTypeFormatter()
        'Dim ri As New RegionInfo(CultureInfo.CurrentUICulture.LCID)
        'Dim strSymbol As String = fmt.GetDescriptor(ri, eDescriptorTypes.Symbol)

        'If (Me.m_InteractionManager Is Nothing) Then Return

        'For iFleet As Integer = 1 To Me.Core.nFleets
        '    For iGroup As Integer = 1 To Me.Core.nLivingGroups

        '        If Me.m_InteractionManager.isLandings(iFleet, iGroup) Then

        '            interaction = Me.m_InteractionManager.LandingInteraction(iFleet, iGroup)
        '            Dim shape As cForcingFunction = Nothing
        '            Dim aplType As eForcingFunctionApplication
        '            Dim sb As New StringBuilder()

        '            If interaction IsNot Nothing Then
        '                For i As Integer = 1 To interaction.NAppliedShapes
        '                    interaction.getShape(i, shape, aplType)
        '                    If shape IsNot Nothing Then
        '                        If sb.Length > 0 Then sb.Append(" ")
        '                        sb.Append(String.Format(My.Resources.ECOSIM_APPLYFF_FFTYPE_PRICEELASTICITY, shape.Index, strSymbol))
        '                    End If
        '                Next
        '            Else
        '                ' This should NOT occur; this indicates that the interaction manager is not up to date!
        '                sb.Append("X")
        '            End If

        '            Me(iGroup, iFleet + 1) = New Cells.Real.Cell(sb.ToString)
        '            Me(iGroup, iFleet + 1).DataModel = Me.m_editor
        '            Me(iGroup, iFleet + 1).Behaviors.Add(Me.m_BehaviorClick)

        '        Else
        '            ' #No: cannot assign FF to this pred/prey combo
        '            cellBlocked = New EwECell(Nothing, GetType(Single))
        '            '  Setup default cell
        '            cellBlocked.Style = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null)
        '            ' Apply cell to the grid
        '            Me(iGroup, iFleet + 1) = cellBlocked
        '        End If

        '    Next iGroup

        'Next iFleet

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        'Me.FixedColumnWidths = False
    End Sub


    Public Overrides Sub ClearAllPairs()

    End Sub



    Public Overrides Sub SetAllPairs()

    End Sub

#End Region


#Region " Internals "

    Protected Overrides Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)

        Try

            Dim MapInteraction As cMapResponseInteractionManager = Core.MapInteractionManager
            Dim igrp As Integer = e.Position.Row
            Dim iMap As Integer = e.Position.Column - 1
            Dim map As EwECore.IEnviroInputMap = MapInteraction.Maps(iMap)
            Dim CapShapes As cBaseShapeManager = Core.CapacityShapeManager

            Dim dlg As New dlgSelectResponse(Me.UIContext, CapShapes, map, igrp)
            dlg.ShowDialog()
            If dlg.DialogResult = DialogResult.OK Then
                'update the interface to the newly selected response
                Me.FillData()
            End If

        Catch ex As Exception
            ' Whoah
        End Try

    End Sub

    Protected Overrides Sub OnRowColClicked(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
        Try
            Dim iRow As Integer = e.Position.Row
            Dim iCol As Integer = e.Position.Column

            MsgBox("Sorry selection of map response function not implemented yet!")

        Catch ex As Exception

        End Try
        'Dim dlg As dlgApplyLandingShape = Nothing

        '' --------------
        '' Prepare dialog
        '' --------------

        '' Column header clicked?
        'If iRow = 0 Then
        '    ' #Yes: Predator column clicked?
        '    If iCol > 1 Then
        '        ' #Yes: launch dialog for all diets of this predator
        '        Dim iFleet As Integer = iCol - 1
        '        dlg = New dlgApplyLandingShape(Me.UIContext, iFleet, dlgApplyLandingShape.eEditMode.Fleet)
        '    Else
        '        dlg = New dlgApplyLandingShape(Me.UIContext)
        '    End If
        'Else
        '    ' #No: Prey row header clicked?
        '    If iCol < Me.FixedColumns Then
        '        ' #Yes: Prey row clicked?
        '        If iRow > 0 Then
        '            ' #Yes: launch dialog for all predation of this prey
        '            dlg = New dlgApplyLandingShape(Me.UIContext, iRow, dlgApplyLandingShape.eEditMode.Group)
        '        End If
        '    End If
        'End If

        ' --------------
        ' Invoke dialog
        ' --------------

        'If dlg IsNot Nothing Then
        '    dlg.ShowDialog()
        'End If

    End Sub

#End Region ' Internals

End Class
