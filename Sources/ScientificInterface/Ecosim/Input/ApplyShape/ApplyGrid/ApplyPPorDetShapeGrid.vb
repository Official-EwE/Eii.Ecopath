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
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceLibrary

#End Region

Namespace Ecosim

    ''' <summary>
    ''' Grid to apply shapes to PP or Detritus groups.
    ''' </summary>
    <CLSCompliant(False)> _
    Public Class ApplyPredPPorDetShapeGrid
        Inherits ApplyShapeGrid

#Region " Private vars "

        Private m_applyShapeMode As eApplyShapeTypes = eApplyShapeTypes.NotSet

#End Region ' Private vars

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)
        End Sub

#Region " Public access "

        Public Property ApplyShapeMode() As eApplyShapeTypes
            Get
                Return Me.m_applyShapeMode
            End Get
            Set(ByVal value As eApplyShapeTypes)
                If (Me.m_applyShapeMode <> value) Then
                    Me.m_applyShapeMode = value
                    Me.RefreshContent()
                End If
            End Set
        End Property

        Public Overrides Sub ClearAllPairs()

            Dim interaction As cMediatedInteraction = Nothing
            Dim application As eForcingFunctionApplication
            Dim ff As cForcingFunction = Nothing

            cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_APPLYVALUES)
            Me.Core.SetBatchLock(cCore.eBatchLockType.Update)

            ' For each row
            For iRow As Integer = 1 To Me.RowsCount - 1

                Dim iGroup As Integer = Me.GroupAtRow(iRow)

                ' Can assign FF at this spot in the matrix?
                If Me.m_InteractionManager.isPredPrey(iGroup, iGroup) Then

                    interaction = Me.m_InteractionManager.PredPreyInteraction(iGroup, iGroup)
                    interaction.LockUpdates = True

                    For i As Integer = 1 To Me.m_InteractionManager.MaxNShapes
                        interaction.getShape(i, ff, application)

                        ' Only delete pairs of current type
                        If (TypeOf ff Is cMediationBaseFunction) And _
                           (Me.m_applyShapeMode = eApplyShapeTypes.Mediation) Then
                            interaction.setShape(i, Nothing)
                        End If

                        If (TypeOf ff Is cForcingFunction) And _
                           (Me.m_applyShapeMode = eApplyShapeTypes.Forcing) Then
                            interaction.setShape(i, Nothing)
                        End If
                    Next

                    interaction.LockUpdates = False

                End If
            Next

            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, True)
            cApplicationStatusNotifier.EndProgress(Me.Core)

        End Sub

        Public Overrides Sub SetAllPairs()
            Throw New NotImplementedException("Aaargh")
        End Sub

#End Region ' Public properties

#Region " Overrides "

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_applyShapeMode = eApplyShapeTypes.NotSet) Then Return

            Me.Redim(1, 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell("Functions")

            ' For each row (rowIndex - Prey)
            For iGroup As Integer = 1 To Core.nGroups

                Dim group As cCoreGroupBase = Me.Core.EcoPathGroupInputs(iGroup)
                If group.IsProducer Or group.IsDetritus Then

                    Dim iRow As Integer = Me.AddRow()

                    Me(iRow, 0) = New EwERowHeaderCell(CStr(iGroup))
                    Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                    Me.GroupAtRow(iRow) = iGroup

                End If

            Next iGroup
        End Sub

        Protected Overrides Sub FillData()

            Dim cellDefault As EwECell = Nothing
            Dim ff As cForcingFunction = Nothing
            Dim PPI As cMediatedInteraction = Nothing

            If (Me.m_InteractionManager Is Nothing) Then Return
            If (Me.m_applyShapeMode = eApplyShapeTypes.NotSet) Then Return

            For iRow As Integer = 1 To Me.RowsCount - 1

                Dim iGroup As Integer = Me.GroupAtRow(iRow)
                Dim group As cCoreGroupBase = Me.Core.EcoPathGroupInputs(iGroup)

                If group.IsProducer Or group.IsDetritus Then

                    PPI = m_InteractionManager.PredPreyInteraction(iGroup, iGroup)

                    Dim shape As cForcingFunction = Nothing
                    Dim aplType As eForcingFunctionApplication
                    Dim sb As New StringBuilder()

                    If PPI IsNot Nothing Then

                        For i As Integer = 1 To PPI.NAppliedShapes
                            PPI.getShape(i, shape, aplType)

                            ' Is med?
                            If (TypeOf shape Is cMediationFunction) Then
                                If ((Me.m_applyShapeMode And eApplyShapeTypes.Mediation) = eApplyShapeTypes.Mediation) Then
                                    If sb.Length > 0 Then sb.Append(" ")
                                    sb.Append(String.Format(My.Resources.ECOSIM_APPLYFF_FFTYPE_MEDIATION, shape.Index))
                                End If
                            Else
                                If ((Me.m_applyShapeMode And eApplyShapeTypes.Forcing) = eApplyShapeTypes.Forcing) Then
                                    If sb.Length > 0 Then sb.Append(" ")
                                    sb.Append(String.Format(My.Resources.ECOSIM_APPLYFF_FFTYPE_FORCING, shape.Index))
                                End If
                            End If
                        Next
                    Else
                        ' This should NOT occur; this indicates that the PPI manager is not up to date!
                        sb.Append("X")
                    End If

                    Me(iRow, 2) = New Cells.Real.Cell(sb.ToString)
                    Me(iRow, 2).DataModel = Me.m_editor
                    Me(iRow, 2).Behaviors.Add(Me.m_bmCell)

                End If

            Next iRow

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumnWidths = False
        End Sub

#End Region ' Overrides 

#Region " Internals "

        Protected Overrides Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)

            Dim iGroup As Integer = GroupAtRow(e.Position.Row)
            Dim group As cCoreGroupBase = Nothing

            If (iGroup = 0) Then Return

            Dim dlg As New dlgApplyPPorDetShape(Me.UIContext, iGroup, Me.m_applyShapeMode)
            dlg.ShowDialog()

        End Sub

        Protected Overrides Sub OnRowColClicked(sender As Object, e As SourceGrid2.PositionEventArgs)
            ' NOP
        End Sub

#End Region ' Internals

    End Class

End Namespace
