' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public MustInherit Class cShapeGridPlugin
    Implements IUIContextPlugin
    Implements IDisposedPlugin
    Implements INavigationTreeItemPlugin

    Private m_uic As cUIContext = Nothing
    Private m_ui As frmShapes = Nothing
    Private m_core As cCore = Nothing

#Region " Plug-in methods "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(core As Object) _
        Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IUIContextPlugin.UIContext"/>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(uic As Object) _
        Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IDisposedPlugin.Dispose"/>
    ''' -----------------------------------------------------------------------
    Public Sub Dispose() _
        Implements IDisposedPlugin.Dispose
        If (Me.HasForm) Then
            Me.m_ui.Close()
            Me.m_ui.Dispose()
        End If
        Me.m_ui = Nothing
        Me.m_uic = Nothing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.ControlImage"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage As Object _
        Implements IGUIPlugin.ControlImage
        Get
            Return SharedResources.nav_input
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.EnabledState"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property EnabledState As eCoreExecutionState _
        Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.OnControlClick"/>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) _
        Implements IGUIPlugin.OnControlClick
        If Not Me.HasForm Then
            Me.m_ui = New frmShapes(Me.GridType)
            Me.m_ui.UIContext = Me.m_uic
            Me.m_ui.Text = Me.DisplayName
            AddHandler Me.m_ui.FormClosed, AddressOf Me.OnFormClosed
        End If
        frmPlugin = Me.m_ui
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String _
        Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek, Ecopath International Initiative"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Contact"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String _
        Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Description"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property Description As String _
        Implements IPlugin.Description

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Name"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property Name As String _
        Implements IPlugin.Name

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property DisplayName As String _
        Implements IPlugin.DisplayName

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.ControlTooltipText"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ControlTooltipText As String _
            Implements IGUIPlugin.ControlTooltipText

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="INavigationTreeItemPlugin.NavigationTreeItemLocation"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property NavigationTreeItemLocation As String _
            Implements INavigationTreeItemPlugin.NavigationTreeItemLocation

    Friend MustOverride Function GridType() As Type

#End Region ' Plug-in methods

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Protected Function HasForm() As Boolean
        If Me.m_ui Is Nothing Then Return False
        Return Not Me.m_ui.IsDisposed
    End Function

    Private Sub OnFormClosed(sender As Object, arg As EventArgs)
        RemoveHandler Me.m_ui.FormClosed, AddressOf Me.OnFormClosed
        Me.m_ui.Dispose()
        Me.m_ui = Nothing
    End Sub
#End Region ' Internals

End Class
