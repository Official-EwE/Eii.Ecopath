' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls


Public Class cSplitGroupPluginPoint
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

#Region " Private vars "

    Private m_uic As cUIContext = Nothing

#End Region ' Private vars

#Region " UI "

    Public ReadOnly Property ControlImage As Object _
        Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property DisplayName As String _
        Implements IPlugin.DisplayName
        Get
            Return My.Resources.MENUITEM_SPLIT_TEXT
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String _
        Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState _
        Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) _
        Implements IGUIPlugin.OnControlClick

        If (Me.m_uic Is Nothing) Then Return

        Dim core As cCore = Me.m_uic.Core

        If (Not core.SaveChanges) Then Return

        Dim engine As New cEcopathSplitGroup(core)

        Me.m_uic.Core.RunEcopath()

        Dim dlg As New dlgSplitGroup(Me.m_uic, engine)
        dlg.ShowDialog()

    End Sub

#End Region ' UI

#Region " UIContext "

    Public Sub UIContext(uic As Object) _
        Implements IUIContextPlugin.UIContext

        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception

        End Try

    End Sub

#End Region ' UIContext

#Region " Menu item "

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuEcopath"
        End Get
    End Property

#End Region ' Menu item

#Region " Generic "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE development team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Lightweight plug-in to split an Ecopath group"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "Z08SplitGroup"
        End Get
    End Property

#End Region ' Generic

End Class
