' ===============================================================================
' This file is part of the Safenet toolkit, contributed to Ecopath with Ecosim
' as part of Safenet project.
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
' Safenet Copyright 2017-, EwE copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class EwEExportLayersPlugin
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

    Private m_uic As cUIContext = Nothing

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuEcospace\MenuEcospaceExport"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return My.Resources.safenet
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IGUIPlugin.DisplayName
        Get
            Return My.Resources.MENU_ITEM_EXPORT
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return My.Resources.TOOLTIP_EXPORT
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndSafenetExportLayers"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return Me.ControlTooltipText
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick
        Dim io As New cImportExportStyle(Me.m_uic)
        Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog(My.Resources.PROMPT_SELECTFILE_SAVE, "", SharedResources.FILEFILTER_STYLE)
        If sfd.ShowDialog(Me.m_uic.FormMain) = DialogResult.OK Then
            io.Add(Me.m_uic.Core.EcospaceBasemap)
            io.Save(sfd.FileName)
        End If
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

End Class
