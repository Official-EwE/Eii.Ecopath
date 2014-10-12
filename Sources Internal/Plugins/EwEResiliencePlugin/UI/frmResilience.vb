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
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Core

#End Region ' Imports

Public Class frmResilience

    Private m_model As cResilienceModel = Nothing
    Private m_zgh As cZedGraphHelper = Nothing

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

        Me.Text = My.Resources.CAPTION

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_graph)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Core}
        AddHandler Me.m_model.OnUpdated, AddressOf OnCalculationsUpdated

        Me.UpdateControls()
        Me.UpdatePlot()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_model.OnUpdated, AddressOf OnCalculationsUpdated
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
        Me.m_cbAutosave.Checked = My.Settings.Autosave
        MyBase.UpdateControls()
    End Sub

    Private Sub UpdatePlot()

        Me.m_zgh.ConfigurePane(My.Resources.LABEL_CAPTION, My.Resources.LABEL_XAXIS, My.Resources.LABEL_YAXIS, True)
        Me.m_zgh.RescaleAndRedraw()

    End Sub

#End Region ' Form overrides

#Region " Events "

    Private Sub OnCalculationsUpdated(sender As cResilienceData, iTime As Integer)
        Dim data As cResilienceData = Me.m_model.Data
        Console.WriteLine("Resilience {0}: supply {1}, demand {2}", iTime, data.SupplyAtT(iTime), data.DemandAtT(iTime))
    End Sub

    Private Sub m_btnRunEcosim_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnRunEcosim.Click
        Try
            Me.Core.RunEcoSim()
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

    Private Sub OnToggleAutosave(sender As System.Object, e As System.EventArgs) _
        Handles m_cbAutosave.CheckedChanged
        Try
            My.Settings.Autosave = Me.m_cbAutosave.Checked
            Me.Core.OnSettingsChanged()
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

#End Region ' Events 

End Class