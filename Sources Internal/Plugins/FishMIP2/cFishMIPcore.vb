Option Strict On
Imports System.Drawing
Imports System.Windows.Forms
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

Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cFishMIPcore
    Implements IPlugin
    Implements IEcopathPlugin
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

#Region " Private variables "

    Private m_uic As cUIContext = Nothing
    Private m_ui As frmConfig = Nothing
    Private Shared s_inst As cFishMIPcore
    Private m_config As cConfiguration = Nothing

#End Region ' Private variables

    Public Shared Function GetInstance() As cFishMIPcore
        Return s_inst
    End Function

    Public ReadOnly Property Core As cCore
        Get
            If (Me.m_uic Is Nothing) Then Return Nothing
            Return Me.m_uic.Core
        End Get
    End Property

    Public ReadOnly Property Configuration As cConfiguration
        Get
            Return Me.m_config
        End Get
    End Property

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
        s_inst = Me
    End Sub

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ecopathinternational@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "fishmipCore"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
    End Sub

#Region " Model integration "

    Public Function LoadModel(dataSource As Object) As Boolean Implements IEcopathPlugin.LoadModel

        Me.m_config = New cConfiguration(Me.Core)
        Me.m_config.Load()
        Return True

    End Function

    Public Function SaveModel(dataSource As Object) As Boolean Implements IEcopathPlugin.SaveModel
        Return True
    End Function

    Public Function CloseModel() As Boolean Implements IEcopathPlugin.CloseModel
        Return True
    End Function

#End Region ' Model integration

#Region " UI integration "

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

    Public ReadOnly Property ControlImage As Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Private Function HasUI() As Boolean
        If (Me.m_ui Is Nothing) Then Return False
        Return Not Me.m_ui.IsDisposed()
    End Function

    Private Function GetUI() As frmConfig
        If Not HasUI() Then Return New frmConfig(Me.m_uic)
        Return m_ui
    End Function

#End Region ' UI integration

End Class
