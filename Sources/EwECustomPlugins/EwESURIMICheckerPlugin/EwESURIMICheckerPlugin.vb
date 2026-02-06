' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.Ecospace
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls

Public Class EwESURIMICheckerPlugin
    Implements IMenuItemPlugin, IEcospaceInitializedPlugin, IUIContextPlugin

    Private m_uic As cUIContext = Nothing
    Private m_frm As frmSURIMIChecker = Nothing

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return Me.DisplayName
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndSURIMIcheck"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "SURIMI checker"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Check if a model meets the SURIMI integration requirements"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE devteam"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick
        Try
            frmPlugin = Me.GetUI()
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Private Function GetUI() As frmSURIMIChecker

        If (Not Me.HasUI) Then
            Me.m_frm = New frmSURIMIChecker(Me.m_uic)
        End If
        Return Me.m_frm

    End Function

    Private Function HasUI() As Boolean
        If (Me.m_frm IsNot Nothing) Then
            Return (Me.m_frm.IsDisposed = False)
        End If
        Return False
    End Function
End Class
