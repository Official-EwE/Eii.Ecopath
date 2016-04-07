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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Ecospace.Advection
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Commands

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class frmAdvection
        Implements ILayerEditorGUI

#Region " Private vars "

        Private m_manager As cAdvectionManager = Nothing

        Private m_fpVXelocity As cEwEFormatProvider = Nothing
        Private m_fpVYelocity As cEwEFormatProvider = Nothing
        Private m_fpCoriolis As cEwEFormatProvider = Nothing
        Private m_fpSorWv As cEwEFormatProvider = Nothing
        Private m_fpWind As cEwEFormatProvider = Nothing
        Private m_fpMLD As cEwEFormatProvider = Nothing
        Private m_fpUpwell As cEwEFormatProvider = Nothing

        Private m_dlgtStarted As cAdvectionManager.ComputationStartedDelegate = Nothing
        Private m_dlgtProgress As cAdvectionManager.ComputationProgressDelegate = Nothing
        Private m_dlgtStopped As cAdvectionManager.ComputationCompletedDelegate = Nothing

        Private m_edtWind As cLayerEditorVector = Nothing
        Private m_edtMLD As cLayerEditor = Nothing
        Private m_edtUpwell As cLayerEditor = Nothing

        ''' <summary>Flag stating whether this form started a search.</summary>
        Private m_bSearching As Boolean = False
        ''' <summary>Flag stating whether a search was completed from this form.</summary>
        Private m_bHasRun As Boolean = False

#End Region ' Private vars

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Design time bypasses
            If Me.UIContext Is Nothing Then Return
            If Me.DesignMode Then Return

            Me.m_manager = Me.Core.AdvectionManager

            ' Set up format providers
            Me.m_fpVXelocity = New cPropertyFormatProvider(Me.UIContext, Me.m_nudXVelocity, Me.Core.AdvectionParameters, eVarNameFlags.XVelocity)
            Me.m_fpVYelocity = New cPropertyFormatProvider(Me.UIContext, Me.m_nudYVelocity, Me.Core.AdvectionParameters, eVarNameFlags.YVelocity)
            Me.m_fpCoriolis = New cPropertyFormatProvider(Me.UIContext, Me.m_nudCoriolis, Me.Core.AdvectionParameters, eVarNameFlags.Coriolis)
            Me.m_fpSorWv = New cPropertyFormatProvider(Me.UIContext, Me.m_nudSorWv, Me.Core.AdvectionParameters, eVarNameFlags.SorWv)
            Me.m_fpWind = New cEwEFormatProvider(Me.UIContext, Me.m_nudWind, GetType(Single))
            Me.m_fpMLD = New cEwEFormatProvider(Me.UIContext, Me.m_nudMLD, GetType(Integer))
            Me.m_fpUpwell = New cEwEFormatProvider(Me.UIContext, Me.m_nudUpwell, GetType(Single))

            ' Connect all layers to the zoom toolbar
            For Each uc As ucAdvectionMap In Me.Maps
                Me.m_ucZoomToolbar.AddZoomContainer(uc.ZoomCtrl)
            Next
            Me.m_ucZoomToolbar.PositionMode = ucMapZoom.ePositionModeTypes.Center

            ' Populate month dropdown
            Me.m_tscmMonth.Items.Clear()
            For i As Integer = 1 To cCore.N_MONTHS
                Me.m_tscmMonth.Items.Add(cDateUtils.GetMonthName(i))
            Next
            Me.m_tscmMonth.SelectedIndex = 0

            ' Initialize editors
            Me.m_edtWind = DirectCast(Me.m_ucWind.DataLayer.Editor, cLayerEditorVector)
            Me.m_edtWind.GUI = Me
            Me.m_edtMLD = Me.m_ucMLD.DataLayer.Editor
            Me.m_edtMLD.GUI = Me
            Me.m_edtUpwell = Me.m_ucUpwelling.DataLayer.Editor
            Me.m_edtUpwell.GUI = Me

            Me.m_dlgtStarted = New cAdvectionManager.ComputationStartedDelegate(AddressOf OnCalcStarted)
            Me.m_dlgtProgress = New cAdvectionManager.ComputationProgressDelegate(AddressOf OnCalcProgress)
            Me.m_dlgtStopped = New cAdvectionManager.ComputationCompletedDelegate(AddressOf OnCalcStopped)
            Me.m_manager.Connect(Me.m_dlgtStarted, Me.m_dlgtStopped, Me.m_dlgtProgress)

            ' Listen to format providers
            AddHandler Me.m_fpVXelocity.OnValueChanged, AddressOf OnVelocityChanged
            AddHandler Me.m_fpVYelocity.OnValueChanged, AddressOf OnVelocityChanged

            ' Config EwEForm
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}

            ' Kick off
            Me.UpdateTransportVelocity()
            'Me.UpdateLayerEditorContent()
            Me.UpdateControls()

            If Me.m_manager.IsRunning Then Me.StartRun()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            ' Stop any pending run, just in case
            Me.StopRun()

            ' Unplug
            Me.m_edtWind.GUI = Nothing
            Me.m_edtWind = Nothing
            Me.m_edtMLD.GUI = Nothing
            Me.m_edtMLD = Nothing
            Me.m_edtUpwell.GUI = Nothing
            Me.m_edtUpwell = Nothing

            For Each uc As ucAdvectionMap In Me.Maps
                Me.m_ucZoomToolbar.RemoveZoomContainer(uc.ZoomCtrl)
            Next

            RemoveHandler Me.m_fpVXelocity.OnValueChanged, AddressOf OnVelocityChanged
            RemoveHandler Me.m_fpVYelocity.OnValueChanged, AddressOf OnVelocityChanged

            Me.m_fpVXelocity.Release()
            Me.m_fpVYelocity.Release()
            Me.m_fpCoriolis.Release()
            Me.m_fpSorWv.Release()
            Me.m_fpWind.Release()
            Me.m_fpMLD.Release()
            Me.m_fpUpwell.Release()

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form overrides

#Region " Public bits "

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_ucZoomToolbar.UIContext = Me.UIContext
                For Each uc As ucAdvectionMap In Me.Maps
                    uc.UIContext = value
                Next
            End Set
        End Property

#End Region ' Public bits

#Region " Control events "

        Private Sub OnToggleOptions(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiToggleOptions.Click

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Me.m_scMain.Panel1Collapsed = Not Me.m_scMain.Panel1Collapsed
            Me.UpdateControls()

        End Sub

        Private Sub OnShowMonth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tscmMonth.SelectedIndexChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Dim layer As cDisplayRasterLayer = Me.m_ucWind.DataLayer
            DirectCast(layer.Data, cEcospaceLayerWind).Month = (1 + Me.m_tscmMonth.SelectedIndex)
            layer.Update(cDisplayLayer.eChangeFlags.Map, False)

        End Sub

        Private Sub OnCursorSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_sliderCursor.ValueChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            ' Get cursor size
            Dim iCursorSize As Integer = CInt(Me.m_sliderCursor.Value)

            ' Distribute cursor size
            For Each uc As ucAdvectionMap In Me.Maps
                If uc.DataLayer IsNot Nothing Then
                    uc.DataLayer.Editor.CursorSize = iCursorSize
                    uc.Map.UpdateCursorFeedback()
                End If
            Next

        End Sub

        Private Sub OnComputeVels(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnStart.Click
            Me.StartRun()
        End Sub

        Private Sub OnStopComputing(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnStop.Click
            Me.m_manager.StopRun()
        End Sub

        Private Sub OnRevertVels(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnRevert.Click
            Me.Revert()
        End Sub

        Private Sub OnEditWindLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_btnEditWind.Click

            Dim rl As cDisplayRasterLayer = DirectCast(Me.m_ucWind.DataLayer, cDisplayRasterLayer)
            Dim cmd As cEditLayerCommand = DirectCast(Me.CommandHandler.GetCommand(cEditLayerCommand.cCOMMAND_NAME), cEditLayerCommand)
            cmd.Invoke(rl, Nothing, eLayerEditTypes.EditData)

        End Sub

        Private Sub OnEditMLDLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_btnEditMLD.Click

            Dim rl As cDisplayRasterLayer = DirectCast(Me.m_ucMLD.DataLayer, cDisplayRasterLayer)
            Dim cmd As cEditLayerCommand = DirectCast(Me.CommandHandler.GetCommand(cEditLayerCommand.cCOMMAND_NAME), cEditLayerCommand)
            cmd.Invoke(rl, Nothing, eLayerEditTypes.EditData)

        End Sub

        Private Sub OnEditUpwellingLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_btnEditUpwelling.Click

            Dim rl As cDisplayRasterLayer = DirectCast(Me.m_ucUpwelling.DataLayer, cDisplayRasterLayer)
            Dim cmd As cEditLayerCommand = DirectCast(Me.CommandHandler.GetCommand(cEditLayerCommand.cCOMMAND_NAME), cEditLayerCommand)
            cmd.Invoke(rl, Nothing, eLayerEditTypes.EditData)

        End Sub

#End Region ' Control events

#Region " Event handlers "

        Private Sub OnVelocityChanged(ByVal sender As Object, args As EventArgs)
            Me.UpdateTransportVelocity()
        End Sub

        Private Sub OnCalcStarted()
            Me.m_bHasRun = False
            Me.UpdateControls()
        End Sub

        Private Sub OnCalcProgress(ByVal iIter As Integer)

            If (iIter Mod 100 = 0) Then
                ' Update app status
                cApplicationStatusNotifier.UpdateProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ADVECTION_ITERATION, iIter), -1)
            End If

            ' Update data layer
            Dim layer As cDisplayRasterLayer = Me.m_ucMap.DataLayer
            layer.IsModified = True
            layer.Update(cDisplayLayer.eChangeFlags.Map, False)

        End Sub

        Private Sub OnCalcStopped(ByVal iIter As Integer, ByVal bInterrupted As Boolean, ByVal bBadFlow As Boolean)
            Me.StopRun()
            Me.m_ucMap.Invalidate()

            If bBadFlow Then
                Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_ADVECTION_INBALANCED, _
                                                 eCoreComponentType.EcoSpace, eMessageType.Any, _
                                                 eMessageImportance.Warning, eMessageReplyStyle.YES_NO, eDataTypes.NotSet, eMessageReply.YES)
                fmsg.Suppressable = True
                Me.Core.Messages.SendMessage(fmsg)
                If fmsg.Reply = eMessageReply.YES Then
                    Me.Revert()
                End If
            End If

            Me.m_bHasRun = True
            Me.UpdateControls()

        End Sub

#End Region ' Event handlers

#Region " ILayerEditor implementation "

        Public Sub Initialize(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.Initialize
            ' NOP
        End Sub

        Public Sub StartEdit(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.StartEdit

            If (Object.ReferenceEquals(editor, Me.m_edtWind)) Then
                Me.m_edtWind.CellValue = CSng(Me.m_nudWind.Value)
            ElseIf (Object.ReferenceEquals(editor, Me.m_edtUpwell)) Then
                Me.m_edtUpwell.CellValue = CSng(Me.m_nudUpwell.Value)
            ElseIf (Object.ReferenceEquals(editor, Me.m_edtMLD)) Then
                Me.m_edtMLD.CellValue = CSng(Me.m_nudMLD.Value)
            End If

        End Sub

        Public Sub EndEdit(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.EndEdit
            ' NOP
        End Sub

        Public Sub UpdateLayerEditorContent(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.UpdateContent

            If (Object.ReferenceEquals(editor, Me.m_edtWind)) Then
                Me.m_nudWind.Value = CDec(Me.m_edtWind.CellValue)
            ElseIf (Object.ReferenceEquals(editor, Me.m_edtUpwell)) Then
                Me.m_nudUpwell.Value = CDec(Me.m_edtUpwell.CellValue)
            ElseIf (Object.ReferenceEquals(editor, Me.m_edtMLD)) Then
                Me.m_nudMLD.Value = CDec(Me.m_edtMLD.CellValue)
            End If

        End Sub

#End Region ' ILayerEditor implementation

#Region " Internals "

        Protected Overrides Sub UpdateControls()

            ' Gather stats
            Dim bBusy As Boolean = Me.m_manager.IsRunning

            Me.m_btnStart.Enabled = Not bBusy And Not Me.m_bSearching
            Me.m_btnStop.Enabled = Me.m_bSearching
            Me.m_btnRevert.Enabled = Me.m_bHasRun

            Me.m_tsmiToggleOptions.Checked = Not Me.m_scMain.Panel1Collapsed

        End Sub

        Private Sub UpdateTransportVelocity()
            Dim sVX As Single = CSng(Me.m_fpVXelocity.Value)
            Dim sVY As Single = CSng(Me.m_fpVYelocity.Value)
            Dim sVel As Single = CSng(Math.Sqrt(sVX * sVX + sVY * sVY))
            Me.m_fpWind.Value = sVel
        End Sub

        Private Function Maps() As ucAdvectionMap()
            Return New ucAdvectionMap() {Me.m_ucMap, Me.m_ucMLD, Me.m_ucUpwelling, Me.m_ucWind}
        End Function

        Private Sub StartRun()

            ' Already running? Abort
            If Me.m_bSearching Then Return

            If Not Me.m_manager.IsRunning Then Me.m_manager.Run(Me)
            Me.m_bSearching = Me.m_manager.IsRunning

            If m_bSearching Then
                cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_ADVECTION_STARTED, -1)
                Me.UpdateControls()
            End If

        End Sub

        Private Sub StopRun()

            If Not Me.m_bSearching Then Return

            Me.m_bSearching = False
            cApplicationStatusNotifier.EndProgress(Me.Core)
            Me.m_manager.StopRun()
            Me.UpdateControls()

        End Sub

        Private Sub Revert()
            If Me.m_manager.Revert Then
                Dim layer As cDisplayRasterLayer = Me.m_ucMap.DataLayer
                layer.IsModified = True
                layer.Update(cDisplayLayer.eChangeFlags.Map, False)
                Me.m_bHasRun = False
                Me.UpdateControls()
            End If
        End Sub

#End Region ' Internals

    End Class

End Namespace
