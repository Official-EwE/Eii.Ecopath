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
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Ecopath
Imports EwEPlugin
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cMergeGroupsPluginPoint
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

#Region " Private vars "

    Private m_uic As cUIContext = Nothing

#End Region ' Private vars

#Region " UI "

    Public ReadOnly Property ControlImage As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String _
        Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.MENUITEM_MERGE_TEXT
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        If (Me.m_uic Is Nothing) Then Return

        Dim core As cCore = Me.m_uic.Core
        Dim engine As New cEcopathMergeGroups(core)

        If Not engine.CanMergeGroups(True) Then Return

        Me.m_uic.Core.RunEcoPath()

        Dim dlg As New dlgMergeGroups(Me.m_uic, engine)
        dlg.ShowDialog()

    End Sub

#End Region ' UI

#Region " UIContext "

    Public Sub UIContext(uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext

        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception

        End Try

    End Sub

#End Region ' UIContext

#Region " Menu item "

    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuEcopath"
        End Get
    End Property

#End Region ' Menu item

#Region " Generic "

    Public Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize
        ' NOP
    End Sub

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "EwE development team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Lightweight plug-in to merge Ecopath groups"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndMergeGroups"
        End Get
    End Property

#End Region ' Generic

End Class
