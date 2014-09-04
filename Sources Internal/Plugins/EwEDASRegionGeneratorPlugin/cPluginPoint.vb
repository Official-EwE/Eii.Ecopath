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
Imports EwECore
Imports EwEUtils.Core
Imports EwEPlugin
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports WeifenLuo.WinFormsUI.Docking
Imports System.Drawing

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in point for the DAS .rgn file generator
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginPoint
    Implements IMenuItemPlugin

    Private m_core As cCore = Nothing

    Public ReadOnly Property ControlImage As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.Baltic_img
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "&Generate DAS region file..."
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "Generate a region (.rgn) file for the Baltic Data Assimiliation System (DAS) for the current Ecospace basemap"
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        Try
            Dim frm As New frmUI(Me.m_core)
            frm.ShowDialog()
        Catch ex As Exception

        End Try

    End Sub

    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek, Oleg Andrejev"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return Me.ControlTooltipText
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception

        End Try
    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndGenerateDAS"
        End Get
    End Property

End Class
