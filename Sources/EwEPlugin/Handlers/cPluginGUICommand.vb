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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System
Imports System.Windows.Forms
Imports System.Diagnostics
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports System.Collections.Generic

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Command enabling centralized launching of GUI plug-ins.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginGUICommand
    Inherits cCommand

#Region " Private vars "

    Private m_ip As IGUIPlugin = Nothing
    Private m_sender As Object = Nothing
    Private m_e As EventArgs = Nothing
    Private m_form As Windows.Forms.Form = Nothing
    Private m_iDockState As Integer = 0 ' Unknown
    Private m_bHasRun As Boolean = False

    ' - Help -
    Private m_strHelpURL As String = ""
    Private m_strHelpTopic As String = ""

#End Region ' Private vars

    Public Sub New(ByVal cmdh As cCommandHandler)
        MyBase.New(cmdh, cPluginGUICommand.COMMAND_NAME)
    End Sub

    Public Shared COMMAND_NAME As String = "~launchguiplugin"

    Public ReadOnly Property CoreExecutionState() As eCoreExecutionState
        Get
            If (Me.m_ip Is Nothing) Then Return eCoreExecutionState.Idle
            Return Me.m_ip.EnabledState
        End Get
    End Property

    Public Property Form() As Windows.Forms.Form
        Get
            Return Me.m_form
        End Get
        Friend Set(ByVal value As Windows.Forms.Form)
            Me.m_form = value
        End Set
    End Property

    Public Property DockState() As Integer
        Get
            Return Me.m_iDockState
        End Get
        Set(ByVal iDockState As Integer)
            Me.m_iDockState = iDockState
        End Set
    End Property

    Public Sub RunPlugin()

        If (Me.m_ip Is Nothing) Then Return
        If Me.m_bHasRun Then Return

        ' Get dockstate, if possible
        If TypeOf Me.m_ip Is IDockStatePlugin Then
            Me.DockState = DirectCast(Me.m_ip, IDockStatePlugin).DockState
        End If

        Try
            Me.m_ip.OnControlClick(Me.m_sender, Me.m_e, Me.m_form)
        Catch ex As Exception
            Debug.Assert(False, String.Format("Error {0} occurred while running plugin {1}", ex.Message, Me.m_ip.Name))
        Finally
            Me.m_bHasRun = True
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the URL to the help file for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpURL As String
        Get
            Return Me.m_strHelpURL
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the URL to the help topic for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpTopic As String
        Get
            Return Me.m_strHelpTopic
        End Get
    End Property

    Friend Overloads Sub Invoke(ByVal ip As IGUIPlugin, ByVal sender As Object, ByVal e As EventArgs)
        Me.m_ip = ip
        Me.m_sender = sender
        Me.m_e = e
        Me.m_bHasRun = False
        Me.m_form = Nothing

        If (TypeOf ip Is IHelpPlugin) Then
            Me.m_strHelpURL = DirectCast(ip, IHelpPlugin).HelpURL
            Me.m_strHelpTopic = DirectCast(ip, IHelpPlugin).HelpTopic
        End If

        ' Try to launch plugin via command structure first
        MyBase.Invoke()
        ' Try to run the plug-in manually
        Me.RunPlugin()
    End Sub

End Class

