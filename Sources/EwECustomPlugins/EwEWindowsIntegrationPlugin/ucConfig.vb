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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



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
        Me.m_cbNoRestart.Enabled = Me.m_plugin.HasRightsToMessWithActiveHours
        Me.m_lblError.Visible = Not Me.m_plugin.HasRightsToMessWithActiveHours
    End Sub

    Private Sub Persist()
        My.Settings.Save()
        Me.m_plugin.RefreshState()
    End Sub

#End Region ' Internals

End Class
