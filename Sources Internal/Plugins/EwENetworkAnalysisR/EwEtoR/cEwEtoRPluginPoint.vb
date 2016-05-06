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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Database
Imports ScientificInterfaceShared.Controls
Imports EwECore

#End Region ' Imports

Public Class cEwEtoRPluginPoint
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.IDisposedPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin

    Friend m_uic As cUIContext = Nothing
    Friend m_epData As cEcopathDataStructures = Nothing

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Stuart Borrett, Sheila Heymans, Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for EwE6 that invokes an R NETWRK script using a model-generated SCOR file"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize

    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "InvokeR"
        End Get
    End Property

    Public Sub Dispose() _
        Implements EwEPlugin.IDisposedPlugin.Dispose
    End Sub

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.Rlogo_5
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Execute NETWRK (R)"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "Execute an R script using a model-generated SCOR file"
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        Dim frm As New frmInvokeR(Me.m_uic, Me)
        frm.ShowDialog()

    End Sub

    Public ReadOnly Property MenuItemLocation As String _
        Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public Sub UIContext(uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) _
        Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted
        Me.m_epData = DirectCast(EcopathDataStructures, cEcopathDataStructures)
    End Sub

End Class
