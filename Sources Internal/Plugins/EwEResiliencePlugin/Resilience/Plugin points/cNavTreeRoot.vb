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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Core
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Forms

#End Region ' Imports

Public Class cNavTreeRoot
    Inherits cResiliencePluginBase
    Implements INavigationTreeItemPlugin

    Protected m_frm As System.Windows.Forms.Form

#Region " UI integration "

    Public ReadOnly Property ControlImage As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property ControlText As String _
        Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.NAVTREE_ROOT
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public Overridable ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.Idle
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick
        Try
            If (Not Me.HasUI()) Then
                Me.m_frm = Me.GetUI()
            End If

            frmPlugin = Me.m_frm

            ' Configure UI
            If (frmPlugin IsNot Nothing) Then
                frmPlugin.Text = Me.ControlText
                If (TypeOf frmPlugin Is frmEwE) Then
                    DirectCast(frmPlugin, frmEwE).TabText = Me.ControlText
                End If
            End If

        Catch ex As Exception

        End Try
    End Sub

    Public Overridable ReadOnly Property NavigationTreeItemLocation As String _
        Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndTimeDynamic\ndEcosimOutput"
        End Get
    End Property

#End Region ' UI integration

#Region " Internals "

    Private Function HasUI() As Boolean
        If (Me.m_frm Is Nothing) Then Return False
        Return (Not Me.m_frm.IsDisposed)
    End Function

    Protected Overridable Function GetUI() As Form
        Return Nothing
    End Function

    Public Overrides ReadOnly Property Name As String
        Get
            Return "ndXResilience-01-Root"
        End Get
    End Property

#End Region ' Internals

#Region " Disposal "

    Protected Overrides Sub Dispose()

        If (Me.m_frm IsNot Nothing) Then
            Me.m_frm.Dispose()
            Me.m_frm = Nothing
        End If

    End Sub

#End Region ' Disposal

End Class
