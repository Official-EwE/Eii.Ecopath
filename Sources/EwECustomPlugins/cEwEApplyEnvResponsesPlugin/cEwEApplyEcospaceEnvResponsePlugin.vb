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
Imports System.Drawing
Imports System.Windows.Forms
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cEwEApplyEcospaceEnvResponsePlugin
    Implements IUIContextPlugin
    Implements INavigationTreeItemPlugin

    Private m_uic As cUIContext = Nothing
    Private m_form As frmApplyResponses = Nothing

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return Me.GetType.ToString
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "Apply env responses - experimental"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceInput"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return Me.DisplayName()
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception
            Me.m_uic = Nothing
        End Try
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

    End Sub

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetMainForm
    End Sub

    Private Function HasMainForm() As Boolean
        If (Me.m_form IsNot Nothing) Then
            Return Not Me.m_form.IsDisposed
        End If
        Return False
    End Function

    Private Function GetMainForm() As frmApplyResponses

        If Not HasMainForm() Then
            Me.m_form = New frmApplyResponses()
            Me.m_form.UIContext = Me.m_uic
            'Me.m_form.Init(Me)
            ' Me.m_form.Text = "Ecospace fit"
        End If

        Return Me.m_form

    End Function

End Class
