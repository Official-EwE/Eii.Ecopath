' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.IO
Imports EwECore
Imports EwECore.MSE
Imports EwECore.Common
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Definitions
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



Public Class frmFleetTradeoffs

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmFleetTradeoffs)()

    Public Sub New(uic As cUIContext)

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.Text = My.Resources.PLUGIN_TITLE
        Me.m_progress.Visible = False

    End Sub

    Private Property UIContext As cUIContext

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Me.CenterToParent()
        Me.UpdateControls()

    End Sub

#Region " Events "

    Private Sub OnRun(sender As Object, e As EventArgs) Handles m_btnRun.Click

        Dim core As cCore = Me.UIContext.Core
        Dim manager As cMSEManager = core.MSEManager

        If manager.IsRunning Then Return
        manager.Connect(Nothing, AddressOf OnDetailedProgress)

        Me.m_progress.Visible = True
        Try
            manager.FleetTradeoffs(Me.OutPath)
        Catch ex As Exception

        End Try
        Me.m_progress.Visible = False

        manager.Disconnect()
        Me.Close()

    End Sub

    Private Sub OnDetailedProgress(MSYProgress As cMSYProgressArgs)
        Try
            Me.m_progress.Value = CInt(100 * (MSYProgress.FleetIndex / Math.Max(MSYProgress.Iteration, 1)))
            Me.m_progress.Refresh()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnChangeOutputLocation(sender As Object, e As EventArgs) Handles m_btnChangeOutput.Click
        Try
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cShowOptionsCommand = CType(cmdh.GetCommand(cShowOptionsCommand.cCOMMAND_NAME), cShowOptionsCommand)
            cmd.Invoke(eApplicationOptionTypes.FileLocations)
            Me.UpdateControls()
        Catch ex As Exception
            m_logger.LogError(ex, "frmFleetTradeoffs::OnChangeOutputLocation Error changing output location")
        End Try
    End Sub

#End Region ' Events

#Region " Internals "

    Private ReadOnly Property OutPath As String
        Get
            Return Path.Combine(Me.UIContext.Core.DefaultOutputPath(eAutosaveTypes.Ecosim), "FleetTradeOff")
        End Get
    End Property

    Private Sub UpdateControls()

        If (Me.UIContext Is Nothing) Then Return
        Me.m_tbxOutput.Text = Me.OutPath

    End Sub

#End Region ' Internals

End Class