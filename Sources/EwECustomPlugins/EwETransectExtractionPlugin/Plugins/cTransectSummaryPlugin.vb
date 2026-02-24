' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in point to invoke the UI to view transect summaries.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTransectSummaryPlugin
    Implements INavigationTreeItemPlugin
    Implements IUIContextPlugin

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_data As cTransectDatastructures = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_frm As frmTransectSummary = Nothing

#End Region ' Private vars

#Region " Foundation "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = CType(core, cCore)
        Me.m_data = cTransectDatastructures.Instance(Me.m_core)
    End Sub

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Name"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "zCosmInsiteTransectSummary"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property DisplayName As String _
        Implements IPlugin.DisplayName
        Get
            Return My.Resources.CAPTION_OUT
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return My.Resources.TOOLTIP_OUT
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek, Chris Lynam"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

#End Region ' Foundation

#Region " UI "

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceOutput"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return SharedResources.nav_output
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return My.Resources.TOOLTIP_OUT
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

#End Region ' UI

#Region " Internals "

    Private Function HasUI() As Boolean
        If (Me.m_frm Is Nothing) Then Return False
        Return Not Me.m_frm.IsDisposed
    End Function

    Private Function GetUI() As frmTransectSummary
        If (Not Me.HasUI()) Then Me.m_frm = New frmTransectSummary(Me.m_uic)
        Return Me.m_frm
    End Function

#End Region ' Internals 

End Class
