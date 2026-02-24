' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls

Public Class cEwEFleetTradeoffsPlugin
    Implements INavigationTreeItemPlugin
    Implements IUIContextPlugin

    Private Property Core As cCore
    Private Property UIC As cUIContext

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndTimeDynamic\ndEcosimTools"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndEcosimMSEFleetTradeOffs"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.PLUGIN_TITLE
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return Me.DisplayName
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.Core = DirectCast(core, cCore)
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.UIC = DirectCast(uic, cUIContext)
    End Sub
    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick
        Dim frm As New frmFleetTradeoffs(Me.UIC)
        frm.ShowDialog(Me.UIC.FormMain)
    End Sub

End Class
