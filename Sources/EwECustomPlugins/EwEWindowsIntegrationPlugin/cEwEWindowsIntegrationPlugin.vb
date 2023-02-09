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

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cEwEKeepSystemAwakePlugin
    Implements IUIContextPlugin
    Implements IEwEOptionsPlugin

#Region " Private parts "

    Private Const PLUGIN_NAME As String = "EwEKeepOSAwakeWhileRunningPlugin"

    Private m_core As cCore = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_mon As cCoreStateMonitorMonitor = Nothing

#End Region ' Private parts

#Region " Generic plup-in bits "

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return PLUGIN_NAME
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.Caption
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Windows-only plug-in to prevent the OS from sleeping while EwE is busy"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

#End Region ' Generic plup-in bits

#Region " Core and UI integration "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
        Me.m_mon = New cCoreStateMonitorMonitor(Me.m_core.StateMonitor)
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

#End Region ' Core and UI integration

#Region " Config plug-in bits "

    Public ReadOnly Property Label As String Implements IEwEOptionsPlugin.Label
        Get
            Return My.Resources.Caption
        End Get
    End Property

    Public Function IsConfigured() As Boolean Implements IConfigurable.IsConfigured
        Return True
    End Function

    Public Function GetConfigUI() As Windows.Forms.Control Implements IConfigurable.GetConfigUI
        Return New ucConfig(Me)
    End Function

#End Region ' Config plug-in bits

#Region " UI callback "

    Friend Sub UpdateKeepAliveState()
        Me.m_mon.UpdateKeepAliveState()
    End Sub

#End Region ' UI callback

End Class
