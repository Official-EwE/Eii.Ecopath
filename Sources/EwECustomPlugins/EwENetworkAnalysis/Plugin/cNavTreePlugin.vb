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

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI

Public MustInherit Class cNavTreeControlPlugin
    Implements INavigationTreeItemPlugin

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IPlugin.Name"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property Name() As String Implements IPlugin.Name

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IGUIPlugin.ControlImage"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ControlImage() As Object _
        Implements IGUIPlugin.ControlImage

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IPlugin.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property DisplayName() As String _
        Implements IPlugin.DisplayName

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IGUIPlugin.ControlTooltipText"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property ControlTooltipText() As String _
        Implements IGUIPlugin.ControlTooltipText
        Get
            Return My.Resources.GENERIC_TOOLTIP
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IGUIPlugin.EnabledState"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property EnabledState() As eCoreExecutionState _
        Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathCompleted
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IGUIPlugin.OnControlClick"/>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) _
        Implements IGUIPlugin.OnControlClick
        frmPlugin = cEwENetworkAnalysisPlugin.SwitchForm(Me.FormPage)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="INavigationTreeItemPlugin.NavigationTreeItemLocation"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property NavigationTreeItemLocation() As String _
        Implements INavigationTreeItemPlugin.NavigationTreeItemLocation

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements IPlugin.Author
        Get
            Return "UBC Institute for the Oceans and Fisheries"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IPlugin.Contact"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IPlugin.Description"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property Description() As String Implements IPlugin.Description
        Get
            ' ToDo: globalize this
            Return "Network Analysis plug-in for Ecopath with Ecosim"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Must override to define the name of the <see cref="frmNetworkAnalysis.ShowPage"></see>
    ''' network analysis page that a navigation item opens.
    ''' </summary>
    ''' <returns>The page to navigate to when this plug-in point is activated.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes

    Protected Function NavTreeNodeRoot() As String
        Return "ndParameterization|ndEcopathOutput|ndEcopathOutputTools"
    End Function

End Class
