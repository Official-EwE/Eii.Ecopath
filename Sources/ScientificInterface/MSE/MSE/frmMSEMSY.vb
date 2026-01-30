' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' ===========================================================================
''' <summary>
''' Form, implements the main MSY search interface.
''' </summary>
''' ===========================================================================
Public Class frmMSEMSY

    Private m_mse As MSE.cMSEManager
    Private m_nFleets As Integer
    Private MSY() As Single

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_mse = Me.UIContext.Core.MSEManager
        Me.m_rbValue.Checked = True

    End Sub

    Private Sub OnRun(sender As System.Object, e As System.EventArgs) _
        Handles m_btnRunMSY.Click

        cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_MSE_INITIALIZING, -1)
        Try

            ' Hard-wire run state parameters...for now
            Me.m_mse.ModelParameters.MSYEvaluateValue = Me.m_rbValue.Checked
            Me.m_mse.ModelParameters.MSYRunSilent = False
            Me.m_mse.ModelParameters.MSYStartTimeIndex = 2

            'get the number of fleets for the progress updates
            Me.m_nFleets = Me.UIContext.Core.nFleets
            ReDim Me.MSY(Me.UIContext.Core.nFleets)
            Me.UpdateControls(True)

            'connect and disconnect every time we run the MSY
            Me.m_mse.Connect(Nothing, AddressOf Me.OnMSYProgress)
            Me.m_mse.RunMSYSearch(True)
            Me.m_mse.Disconnect()

        Catch ex As Exception

        End Try
        cApplicationStatusNotifier.EndProgress(Me.Core)
        Me.UpdateControls(False)

    End Sub

    Private Shadows Sub UpdateControls(bRunning As Boolean)

        If bRunning Then
            Me.m_btnRunMSY.Enabled = False
            Me.m_btnStop.Enabled = True
        Else
            Me.m_btnRunMSY.Enabled = True
            Me.m_btnStop.Enabled = False

            Dim sb As New StringBuilder()
            sb.AppendLine(cStringUtils.Localize(My.Resources.MSE_ITERATION_HEADER, cStringUtils.vbTab))
            For i As Integer = 1 To Me.UIContext.Core.nFleets
                sb.AppendLine(cStringUtils.Localize(My.Resources.MSE_ITERATION_LINE, cStringUtils.vbTab, i, Me.StyleGuide.FormatNumber(Me.MSY(i))))
            Next
            Me.m_txtMSYresults.Text = sb.ToString
        End If

    End Sub

    Private Sub OnMSYProgress(MSYProgress As MSE.cMSYProgressArgs)

        Try
            Me.m_lbFleet.Text = cStringUtils.Localize(SharedResources.GENERIC_VALUE_FLEET_OF_N, MSYProgress.FleetIndex, Me.m_nFleets)
            Me.m_lblIter.Text = cStringUtils.Localize(SharedResources.GENERIC_VALUE_ITERATION, MSYProgress.Iteration)
            Me.m_lblEffort.Text = cStringUtils.Localize(My.Resources.MSE_EFFORT_VALUE, Me.StyleGuide.FormatNumber(MSYProgress.CurrentEffort))
            If MSYProgress.CurrentEffort > 0 Then Me.MSY(MSYProgress.FleetIndex) = MSYProgress.CurrentEffort

            cApplicationStatusNotifier.UpdateProgress(Me.Core, cStringUtils.Localize(SharedResources.GENERIC_VALUE_FLEET_OF_N, MSYProgress.FleetIndex, Me.m_nFleets), CSng(MSYProgress.FleetIndex / Me.m_nFleets))

            'the DoEvents can be removed once the MSY is running on a thread 
            Application.DoEvents()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnStop(sender As System.Object, e As System.EventArgs) _
        Handles m_btnStop.Click
        Me.m_mse.StopRun(0)
    End Sub

End Class