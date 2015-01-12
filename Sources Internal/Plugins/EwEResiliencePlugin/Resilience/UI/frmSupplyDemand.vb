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

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmSupplyDemand

#Region " Internal vars "

    Private m_model As cResilienceModel = Nothing
    Private m_graph As cSupplyDemandGraph = Nothing

#End Region ' Internal vars

    Public Sub New(uic As cUIContext, model As cResilienceModel)
        MyBase.New()
        Me.UIContext = uic
        Me.m_model = model
        Me.InitializeComponent()

    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_graph = New cSupplyDemandGraph()
        Me.m_graph.Attach(Me.UIContext, Me.m_zgc, Me.m_model.Data, "")

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Core}
        AddHandler Me.m_model.OnUpdated, AddressOf OnCalculationsUpdated

        Me.m_tsbnAutosave.Image = SharedResources.saveOutputHS
        Me.m_tsbnSaveNow.Image = SharedResources.saveHS
        Me.m_tsbnDynamicScales.Image = My.Resources.FixedAxesHS

        Me.m_cbAnnual.Checked = My.Settings.ResilShowAnnual

        Me.UpdateControls()
        Me.UpdateGraph()

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
            Me.m_slider.Enabled = True
            Me.m_cbAnnual.Enabled = True
            Me.m_slider.Minimum = 1

            If Me.m_cbAnnual.Checked Then
                Me.m_slider.Maximum = Me.m_model.Data.NumYears
            Else
                Me.m_slider.Maximum = Me.m_model.Data.NumTimeSteps
            End If
        Else
            Me.m_slider.Enabled = False
            Me.m_cbAnnual.Enabled = False
        End If

        Me.m_tsbnAutosave.Checked = My.Settings.ResilAutosave

        MyBase.UpdateControls()
    End Sub

    Private Sub UpdateGraph()

        Me.m_graph.Time = Me.m_slider.Value
        Me.m_graph.Annual = Me.m_cbAnnual.Checked
        Me.m_lblTime.Text = CStr(Me.m_slider.Value)

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
            If bDone Then Me.UpdateGraph()
            Me.UpdateControls()
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

    Private Sub OnToggleAnnual(sender As System.Object, e As System.EventArgs) _
        Handles m_cbAnnual.CheckedChanged
        Try
            Me.m_graph.Annual = Me.m_cbAnnual.Checked
            Me.UpdateControls()
            Me.UpdateGraph()
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

    Private Sub OnToggleAutosave(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnAutosave.Click
        Try
            My.Settings.ResilAutosave = Me.m_tsbnAutosave.Checked
            Me.Core.OnSettingsChanged()
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

    Private Sub OnToggleDynamicAxis(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnDynamicScales.Click
        Try
            Me.m_graph.FixedScale = (Me.m_tsbnDynamicScales.Checked = False)
            Me.UpdateGraph()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnSaveNow(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnSaveNow.Click
        Try
            Dim writer As New cResilienceWriter(Me.UIContext.Core, Me.m_model.Data)
            writer.Write()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnTimeSliderChanged(sender As Object, e As System.EventArgs) _
        Handles m_slider.ValueChanged
        Try
            Me.UpdateGraph()
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

#End Region ' Events 

End Class