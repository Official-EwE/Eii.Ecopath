' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Commands



Namespace Integration

    ''' ===========================================================================
    ''' <summary>
    ''' Helper class, launches all Auto-launchable plug-ins.
    ''' </summary>
    ''' ===========================================================================
    Public Class cPluginAutolaunchHandler
        Inherits cPluginGUIHandler

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of a cPluginAutolaunchHandler.
        ''' </summary>
        ''' <param name="pm"><see cref="cPluginManager">Plugin manager</see>
        ''' that holds the plugins to launch.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(pm As cPluginManager, cmdh As cCommandHandler)
            MyBase.New(pm, cmdh)
            Me.LaunchPlugins()
        End Sub

#End Region ' Construction 

#Region " Overrides "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden with emtpy method to comply to base class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub EnablePlugin(ip As IGUIPlugin, bEnable As Boolean)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden with emtpy method to comply to base class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub PlacePlugin(ip As IGUIPlugin, bPlace As Boolean)
        End Sub

#End Region ' Overrides 

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Launch all <see cref="IAutolaunchPlugin">Auto-launchable plug-ins.</see>
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub LaunchPlugins()
            Dim collPlugins As ICollection(Of cPluginManager.cPluginContext) = Me.PluginManager.GetPluginDefs(GetType(IAutolaunchPlugin))
            For Each ipc As cPluginManager.cPluginContext In collPlugins
                Dim ip As IAutolaunchPlugin = DirectCast(ipc.Plugin, IAutolaunchPlugin)
                If ip.Autolaunch Then
                    Me.RunPlugin(ip, Nothing, Nothing)
                End If
            Next
        End Sub

#End Region ' Internals

    End Class

End Namespace
