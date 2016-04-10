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
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmSupplyDemand

#Region " Internal vars "

    Private m_model As cResilienceModel = Nothing
    Private m_graph As cDemandSupplyGraph = Nothing
    Private m_bInUpdate As Boolean = False
    Private m_iTime As Integer = 0

#End Region ' Internal vars

    Public Sub New(uic As cUIContext, model As cResilienceModel)

        MyBase.New()
        Me.InitializeComponent()

        Me.UIContext = uic
        Me.m_model = model

    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_graph = New cDemandSupplyGraph()
        Me.m_graph.Attach(Me.UIContext, Me.m_zgc, Me.m_model.Data, "")

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Core}
        AddHandler Me.m_model.OnUpdated, AddressOf OnCalculationsUpdated

        Me.m_cbAnnual.Checked = My.Settings.ResilShowAnnual

        Me.m_tsbnAutosave.Image = SharedResources.saveOutputHS
        Me.m_tsbnAutosave.Checked = My.Settings.ResilAutosave

        Me.m_tsbnSaveNow.Image = SharedResources.saveHS

        Me.m_tsbnDynamicScales.Image = My.Resources.FixedAxesHS
        Me.m_tsbnDynamicScales.Checked = Not Me.m_graph.FixedScale

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_model.OnUpdated, AddressOf OnCalculationsUpdated
        Me.m_graph.Detach()

        My.Settings.ResilAutosave = Me.m_tsbnAutosave.Checked
        My.Settings.ResilShowAnnual = Me.m_cbAnnual.Checked
        My.Settings.Save()

        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
        MyBase.OnCoreMessage(msg)

        Select Case msg.Source
            Case eCoreComponentType.Core
                If (msg.Type = eMessageType.GlobalSettingsChanged) Then
                    Me.UpdateControls()
                End If
        End Select

    End Sub

    Protected Overrides Sub UpdateControls()

        If Me.m_model.Data.Calculated Then
            Me.m_cbAnnual.Enabled = True
            Me.m_slider.Enabled = True
            Me.m_nudTime.Enabled = True
        Else
            Me.m_cbAnnual.Enabled = False
            Me.m_slider.Enabled = False
            Me.m_nudTime.Enabled = False
        End If

        Me.m_slider.Minimum = 1
        Me.m_nudTime.Minimum = 1

        If Me.m_cbAnnual.Checked Then
            Me.m_iTime = Math.Max(1, Math.Min(Me.m_iTime, Me.m_model.Data.NumYears))
            Me.m_slider.Maximum = Me.m_model.Data.NumYears
            Me.m_nudTime.Maximum = Me.m_model.Data.NumYears
        Else
            Me.m_slider.Maximum = Me.m_model.Data.NumTimeSteps
            Me.m_nudTime.Maximum = Me.m_model.Data.NumTimeSteps
        End If
        Me.m_slider.Value = Me.m_iTime
        Me.m_nudTime.Value = Me.m_iTime

        Me.m_tsbnAutosave.Checked = My.Settings.ResilAutosave

        MyBase.UpdateControls()

    End Sub

    Private Sub UpdateGraph()

        ' Wait!
        If (Not Me.m_graph.IsAttached) Then Return

        Me.m_graph.Time = Me.m_iTime
        Me.m_graph.Annual = Me.m_cbAnnual.Checked

        Me.m_graph.Refresh()

    End Sub

    Public Overrides ReadOnly Property IsRunForm As Boolean
        Get
            Return False
        End Get
    End Property

#End Region ' Form overrides

#Region " Events "

    Private Sub OnCalculationsUpdated(sender As cResilienceData, iTime As Integer, bDone As Boolean)
        Try
            If bDone Then

                Me.m_bInUpdate = True

                Me.m_iTime = 1
                Me.m_slider.Value = iTime
                Me.m_nudTime.Value = iTime

                Me.m_bInUpdate = False
                Me.UpdateGraph()

            End If
            Me.UpdateControls()
        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex, "Reselience:frmSupplyDemand.OnCalculationsUpdated")
        End Try
    End Sub

    Private Sub OnToggleAnnual(sender As System.Object, e As System.EventArgs) _
        Handles m_cbAnnual.CheckedChanged
        Try
            Dim nStepsPerYear As Integer = CInt(Me.Core.nEcosimTimeSteps / Me.Core.nEcosimYears)

            Me.m_graph.Annual = Me.m_cbAnnual.Checked

            If Me.m_graph.Annual Then
                Me.m_iTime = CInt((Me.m_iTime - 1) / nStepsPerYear) + 1
            Else
                Me.m_iTime = CInt((Me.m_iTime - 1) * nStepsPerYear) + 1
            End If
            Me.UpdateControls()
            Me.UpdateGraph()
        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex, "Reselience:frmSupplyDemand.OnToggleAnnual")
        End Try
    End Sub

    Private Sub OnToggleAutosave(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnAutosave.Click
        Try
            My.Settings.ResilAutosave = Me.m_tsbnAutosave.Checked
            Me.Core.OnSettingsChanged()
        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex, "Reselience:frmSupplyDemand.OnToggleAutosave")
        End Try
    End Sub

    Private Sub OnToggleDynamicAxis(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnDynamicScales.Click
        Try
            Me.m_graph.FixedScale = (Me.m_tsbnDynamicScales.Checked = False)
            Me.UpdateGraph()
        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex, "Reselience:frmSupplyDemand.OnToggleDynamicAxis")
        End Try
    End Sub

    Private Sub OnSaveNow(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnSaveNow.Click
        Try
            Dim core As cCore = Me.UIContext.Core

            If (Not core.StateManager.LoadState(eCoreExecutionState.EcosimCompleted)) Then
                Dim msg As New cMessage(My.Resources.RESIL_STATUS_RUNSIM, eMessageType.StateNotMet, eCoreComponentType.EcoSim, eMessageImportance.Warning)
                core.Messages.SendMessage(msg)
                Return
            End If

            Dim writer As New cResilienceWriter(core, Me.m_model.Data)
            writer.Write()
        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex, "Reselience:frmSupplyDemand.OnSaveNow")
        End Try
    End Sub

    Private Sub OnTimeSliderChanged(sender As Object, e As System.EventArgs) _
        Handles m_slider.ValueChanged
        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        Try
            Me.m_iTime = Me.m_slider.Value
            Me.m_nudTime.Value = Me.m_iTime
            Me.UpdateGraph()
        Catch ex As Exception

        End Try
        Me.m_bInUpdate = False
    End Sub

    Private Sub OnTimeNUDChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_nudTime.ValueChanged

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        Try
            Me.m_iTime = CInt(Me.m_nudTime.Value)
            Me.m_slider.Value = Me.m_iTime
            Me.UpdateGraph()
        Catch ex As Exception

        End Try
        Me.m_bInUpdate = False
    End Sub

#End Region ' Events 

End Class