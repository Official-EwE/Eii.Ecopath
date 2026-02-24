' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls

''' ---------------------------------------------------------------------------
''' <summary>
''' The plug-in point for the Multi-Sim plug-in.
''' </summary>
''' <remarks>
''' Did you know that this plug-in was briefly called 'Multi-Runs'? Tee hee hee.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cMultiSimPlugin
    Implements IUIContextPlugin
    Implements IMenuItemPlugin
    Implements INavigationTreeItemPlugin
    Implements IHelpPlugin

#Region " Private vars "

    Private m_frmUI As frmMain = Nothing
    Private m_uic As cUIContext = Nothing

#End Region ' Private vars

#Region " Plug-in points "

#Region " IPlugin "

    Public Sub Initialize(core As Object) _
        Implements IPlugin.Initialize
        ' Ignore
    End Sub

    Public ReadOnly Property Name As String _
        Implements IPlugin.Name
        Get
            Return "ndDFO_MultiSim"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.GENERIC_DISPLAYNAME
        End Get
    End Property

    Public ReadOnly Property Author As String _
        Implements IPlugin.Author
        Get
            Return "Fisheries and Oceans Canada"
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements IPlugin.Contact
        Get
            Return "Sylvie Guenette, Carie Hoover, Dave Preikshot"
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements IPlugin.Description
        Get
            Return My.Resources.GENERIC_DESCRIPTION
        End Get
    End Property

#End Region ' IPlugin

#Region " UI Context "

    Public Sub UIContext(uic As Object) _
        Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

#End Region ' UI Context

#Region " GUI integration "

    Public ReadOnly Property ControlImage As Object _
        Implements IGUIPlugin.ControlImage
        Get
            Return Nothing ' My.Resources.logo_canada
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) _
        Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.UI
    End Sub

    Public ReadOnly Property ControlTooltipText As String _
        Implements IGUIPlugin.ControlTooltipText
        Get
            Return Me.Description
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState _
        Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

#End Region ' GUI integration

#Region " Menu item "

    Public ReadOnly Property MenuItemLocation As String _
        Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

#End Region ' Menu item

#Region " Navigation tree "

    Public ReadOnly Property NavigationTreeItemLocation As String _
        Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndTimeDynamic\ndEcosimTools"
        End Get
    End Property

#End Region ' Navigation tree

#Region " Help! "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IHelpPlugin.HelpTopic"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpTopic As String _
        Implements IHelpPlugin.HelpTopic
        Get
            Return ".\UserGuide\EwEMultiSimPlugin.pdf"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IHelpPlugin.HelpURL"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpURL As String _
        Implements IHelpPlugin.HelpURL
        Get
            Return Me.HelpTopic
        End Get
    End Property

#End Region ' Help!

#End Region ' Plug-in point

#Region " Private helper methods "

    Private Function UI() As frmMain
        Dim bHasUI As Boolean = False

        If (Me.m_frmUI IsNot Nothing) Then
            bHasUI = Not Me.m_frmUI.IsDisposed
        End If

        If Not bHasUI Then
            Me.m_frmUI = New frmMain()
            Me.m_frmUI.UIContext = Me.m_uic
            Me.m_frmUI.Text = Me.DisplayName
        End If

        Return Me.m_frmUI

    End Function

#End Region ' Private helper methods

End Class
