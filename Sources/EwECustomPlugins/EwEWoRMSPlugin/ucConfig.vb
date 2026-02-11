' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for configuring a WoRMS web service connection.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class ucConfig
    Implements IOptionsPage

    ''' <summary>Plug-in to configure.</summary>
    Private m_plugin As cWoRMSPluginPoint = Nothing

    Public Sub New(plugin As cWoRMSPluginPoint)
        MyBase.New()
        Me.m_plugin = plugin
        Me.Text = My.Resources.ENGINE_NAME
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_nudConnTO.Value = Me.m_plugin.ConnectionTimeOut
        Me.m_nudReplyTO.Value = Me.m_plugin.ResponseTimeOut

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmd As cBrowserCommand = CType(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        cmd.AddControl(Me.m_pbBlueBridge, "http://www.i-marine.eu/Content/eLibrary.aspx?id=786ae7dd-f868-4c19-b611-3500b6697bee&li=0")
        cmd.AddControl(Me.m_pbWoRMS, "http://www.marinespecies.org")

    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso Me.components IsNot Nothing Then
                Me.components.Dispose()
            End If
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cBrowserCommand = CType(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.RemoveControl(Me.m_pbBlueBridge)
            cmd.RemoveControl(Me.m_pbWoRMS)
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Public Property UIContext As cUIContext _
        Implements IUIElement.UIContext

    Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply
        Me.m_plugin.ConnectionTimeOut = CInt(Me.m_nudConnTO.Value)
        Me.m_plugin.ResponseTimeOut = CInt(Me.m_nudReplyTO.Value)
        Return IOptionsPage.eApplyResultType.Success
    End Function

    Public Function CanApply() As Boolean Implements IOptionsPage.CanApply
        Return True
    End Function

    Public Function CanSetDefaults() As Boolean Implements IOptionsPage.CanSetDefaults
        Return True
    End Function

    Public Event OnChanged(sender As IOptionsPage, args As System.EventArgs) Implements IOptionsPage.OnChanged

    Public Sub SetDefaults() Implements IOptionsPage.SetDefaults
        Me.m_nudConnTO.Value = 60
        Me.m_nudReplyTO.Value = 300
    End Sub

End Class