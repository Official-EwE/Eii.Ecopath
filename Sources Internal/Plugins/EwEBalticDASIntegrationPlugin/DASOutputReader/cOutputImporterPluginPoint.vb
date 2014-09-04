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
Imports System.Drawing
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in point for the DAS .rgn file generator
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cOutputImporterPluginPoint
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

#Region " Private variables "

    ''' <summary>Maintain a reference to the EwE core.</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Maintain a reference to the EwE UI context.</summary>
    Private m_uic As cUIContext = Nothing

#End Region ' Private variables

#Region " Generic plugin implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each plug-in must have a name for internal organization, NOT for display.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndBalticDASImportOutput"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each plug-in has a description that is displayed to users in generic
    ''' plug-in information sheets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return Me.ControlTooltipText
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each plug-in gets one shot to initialize against the EwE core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Try
            ' Hang on to this one
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each plug-in should state its author.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek, Oleg Andrejev, Maciej Tomczak"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each plug-in should state contact information.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

#End Region ' Generic plugin implementation

#Region " UI plugin implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each UI plug-in can return a small (16x16) picture.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage As System.Drawing.Image _
         Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.Baltic_img
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each UI plug-in must return a text to display on UI controls.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlText As String _
        Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.CONTROL_LOAD_TEXT
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each UI plug-in can return an optional tooltip text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return My.Resources.CONTROL_LOAD_TOOLTIP
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each UI plug-in must state when it can be accessed through the EwE UI 
    ''' framework.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each UI plug-in must respond to the event when its user interface 
    ''' element is activated.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        Try
            Dim frm As New frmOutputImporter(Me.m_uic)
            frm.ShowDialog()
        Catch ex As Exception

        End Try

    End Sub

#End Region ' UI plugin implementation

#Region " Menu plugin implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Each Menu Item plug-in must state where it will be placed on the EwE
    ''' menu.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

#End Region ' Menu plugin implementation

#Region " UI context plugin "

    Public Sub UIContext(uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext

        Try
            ' Hang on to this one
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception

        End Try

    End Sub

#End Region ' UI context plugin

End Class
