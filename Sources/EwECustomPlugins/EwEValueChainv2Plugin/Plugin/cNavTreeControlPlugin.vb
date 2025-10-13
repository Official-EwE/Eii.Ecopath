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
Imports System.Linq
Imports System.Text

#End Region ' Imports

Public MustInherit Class cNavTreeControlPlugin
    Implements INavigationTreeItemPlugin
    Implements IHelpPlugin
    Implements IUIContextPlugin

    Private m_piMain As cValueChainPluginV2 = Nothing
    Private m_uic As cUIContext = Nothing

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Name"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property DisplayName() As String _
        Implements EwEPlugin.IPlugin.DisplayName

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.ControlImage"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ControlImage() As Object _
        Implements EwEPlugin.IGUIPlugin.ControlImage

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.ControlTooltipText"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property ControlTooltipText() As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.EnabledState"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathCompleted
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.OnControlClick"/>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick
        Try
            ' Resolve master plug-in on first use
            If (Me.m_uic IsNot Nothing And Me.m_piMain Is Nothing) Then
                Dim pm As cPluginManager = Me.m_uic.Core.PluginManager
                Dim pic As ICollection(Of IPlugin) = pm.GetPlugins(GetType(cValueChainPluginV2))
                If (pic IsNot Nothing AndAlso pic.Count > 0) Then
                    Me.m_piMain = DirectCast(pic.ElementAt(0), cValueChainPluginV2)
                End If
            End If

            Debug.Assert(Me.m_piMain IsNot Nothing)

            frmPlugin = Me.m_piMain.SwitchForm(Me.FormPage)
        Catch ex As Exception
            ' WHoah!
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property NavigationTreeItemLocation() As String _
        Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Institute for the Oceans and Fisheries, ECOST project, North Sea Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Contact"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Description"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Dim sb As New StringBuilder()
            sb.AppendLine("ValueChain - an economic fisheries model for EwE6")
            sb.AppendLine("")
            sb.AppendLine("This plug-in calculates a range of economic and social-economic indicators based on Ecopath and Ecosim data, where users can define economic systems as value chains of desired complexity.")
            sb.AppendLine("")
            sb.AppendLine("This plug-in was developed in conjunction with the ECOST project (http://www.ird.fr/ecostproject), and was partially funded by the North Sea Centre in Hirtshals, Denmark.")
            Return sb.ToString()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Must override to define the name of the <see cref="frmMain.ShowForm"></see>value chain page that a 
    ''' navigation item opens.
    ''' </summary>
    ''' <returns>The page to navigate to when this plug-in point is activated.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function FormPage() As eValueChainPageTypes

    Protected Function NavTreeNodeRoot() As String
        Return "ndParameterization|ndEcopathOutput|ndEcopathOutputTools"
    End Function

    Public ReadOnly Property UIContext As cUIContext
        Get
            Return Me.m_uic
        End Get
    End Property

    Public Overridable Sub SetUIContext(uicObj As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uicObj, cUIContext)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IHelpPlugin.HelpTopic"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpTopic As String Implements EwEPlugin.IHelpPlugin.HelpTopic
        Get
            Return Me.HelpURL
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IHelpPlugin.HelpURL"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpURL As String Implements EwEPlugin.IHelpPlugin.HelpURL
        Get
            Return ".\UserGuide\ChristensenValueChainMS.pdf"
        End Get
    End Property

End Class
