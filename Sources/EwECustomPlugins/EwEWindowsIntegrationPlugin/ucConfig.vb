' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls

''' ---------------------------------------------------------------------------
''' <summary>
''' Minimalistic configuration UI for this plug-in. The UI is flagged as an 
''' <see cref="IOptionsPage"/>, making it discoverable by, and usable in, the 
''' EwE options interface.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class ucConfig
    Implements IOptionsPage

#Region " Construction / destruction "

    Private m_plugin As cEwEKeepSystemAwakePlugin = Nothing

    Public Sub New(pi As cEwEKeepSystemAwakePlugin)
        Me.InitializeComponent()
        Me.m_hdr.Text = My.Resources.Caption
        Me.m_plugin = pi
        Me.UpdateControls()
    End Sub

#End Region ' Construction / destruction

#Region " IOptionsPage implementation "

    Public Property UIContext As cUIContext Implements IUIElement.UIContext

    Public Event OnChanged As IOptionsPage.OnChangedEventHandler Implements IOptionsPage.OnChanged

    Public Sub SetDefaults() Implements IOptionsPage.SetDefaults
        My.Settings.Enabled = False
        My.Settings.KeepOSAwake = False
        My.Settings.KeepMonitorOn = False
        My.Settings.NoRestart = False
        Me.Persist()
        Me.UpdateControls()
    End Sub

    Public Function CanApply() As Boolean Implements IOptionsPage.CanApply
        Return True
    End Function

    Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply
        My.Settings.Enabled = Me.m_cbEnabled.Checked
        My.Settings.KeepOSAwake = Me.m_cbKeepOSAwake.Checked
        My.Settings.KeepMonitorOn = Me.m_cbKeepMonitorOn.Checked
        My.Settings.NoRestart = Me.m_cbNoRestart.Checked
        Me.Persist()
        Return IOptionsPage.eApplyResultType.Success
    End Function

    Public Function CanSetDefaults() As Boolean Implements IOptionsPage.CanSetDefaults
        Return True
    End Function

#End Region ' IOptionsPage implementation

#Region " Internals "

    Private Sub UpdateControls()
        Me.m_cbEnabled.Checked = My.Settings.Enabled
        Me.m_cbKeepOSAwake.Checked = My.Settings.KeepOSAwake
        Me.m_cbKeepMonitorOn.Checked = My.Settings.KeepMonitorOn
        Me.m_cbNoRestart.Checked = My.Settings.NoRestart

        Dim isSupportedPlatform As Boolean = Me.m_plugin.IsSupportedPlatform
        Dim hasActiveHoursRights As Boolean = Me.m_plugin.HasRightsToMessWithActiveHours
        Me.m_cbEnabled.Enabled = isSupportedPlatform
        Me.m_cbKeepOSAwake.Enabled = isSupportedPlatform
        Me.m_cbKeepMonitorOn.Enabled = isSupportedPlatform
        Me.m_cbNoRestart.Enabled = isSupportedPlatform AndAlso hasActiveHoursRights
        Me.m_lblError.Visible = Not isSupportedPlatform OrElse Not hasActiveHoursRights
        If Not isSupportedPlatform Then
            Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(ucConfig))
            Me.m_lblError.Text = resources.GetString("UnsupportedPlatform")
        End If
    End Sub

    Private Sub Persist()
        My.Settings.Save()
        Me.m_plugin.RefreshState()
    End Sub

#End Region ' Internals

End Class
