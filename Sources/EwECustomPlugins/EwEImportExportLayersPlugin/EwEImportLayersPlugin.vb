' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class EwEImportLayersPlugin
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

    Private m_uic As cUIContext = Nothing

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuEcospace\MenuEcospaceImport"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return My.Resources.safenet
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.MENU_ITEM_IMPORT
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return My.Resources.MENU_ITEM_IMPORT
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndSafenetImportLayers"
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
        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(My.Resources.PROMPT_SELECTFILE_LOAD, "", SharedResources.FILEFILTER_STYLE)
        If ofd.ShowDialog(Me.m_uic.FormMain) = DialogResult.OK Then
            If io.Load(ofd.FileName) Then
                Dim dlg As New dlgImportLayerStyles(Me.m_uic, io)
                dlg.ShowDialog()
            End If
        End If
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

End Class
