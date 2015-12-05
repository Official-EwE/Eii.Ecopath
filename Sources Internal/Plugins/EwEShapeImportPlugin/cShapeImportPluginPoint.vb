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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports EwEShapeImportPlugin.Controls.Shapes

#End Region ' Imports

Public Class cShapeImportPluginPoint
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

    Private m_core As cCore = Nothing
    Private m_uic As cUIContext = Nothing

    Public ReadOnly Property ControlImage As System.Drawing.Image _
        Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String _
        Implements IGUIPlugin.ControlText
        Get
            Return "Import shapes..."
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
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements IGUIPlugin.OnControlClick

        Dim frm As New frmImportShapes(Me.m_uic)
        frm.ShowDialog()

    End Sub

    Public ReadOnly Property MenuItemLocation As String _
        Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "ewedevteam"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

    End Sub

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndImportShapes"
        End Get
    End Property

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub


End Class
