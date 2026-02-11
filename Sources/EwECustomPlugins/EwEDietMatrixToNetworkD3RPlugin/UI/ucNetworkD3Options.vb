' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

Public Class ucNetworkD3Options
    Implements IOptionsPage

    Private m_plugin As cNetworkD3RWriterPlugin = Nothing

    Public Property UIContext As cUIContext Implements IUIElement.UIContext

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Friend Sub Init(plugin As cNetworkD3RWriterPlugin)
        Me.m_plugin = plugin
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Debug.Assert(Me.m_plugin IsNot Nothing)

        Me.m_cmbNetworkType.Items.Clear()
        For Each t As eNetworkD3DiagramType In [Enum].GetValues(GetType(eNetworkD3DiagramType))
            Me.m_cmbNetworkType.Items.Add(t)
        Next

        Me.UpdateControls()
    End Sub

    Public Event OnChanged As IOptionsPage.OnChangedEventHandler _
        Implements IOptionsPage.OnChanged

    Public Sub SetDefaults() Implements IOptionsPage.SetDefaults

        Me.m_plugin.UseSymbolicNames = True
        Me.m_plugin.UseClipboard = True
        Me.m_plugin.NetworkType = eNetworkD3DiagramType.simpleNetwork
        Me.UpdateControls()

    End Sub

    Public Function CanApply() As Boolean Implements IOptionsPage.CanApply
        Return True
    End Function

    Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply

        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf Me.ApplySafe))
        Else
            Me.ApplySafe()
        End If
        Return IOptionsPage.eApplyResultType.Success

    End Function

    Public Function CanSetDefaults() As Boolean Implements IOptionsPage.CanSetDefaults
        Return True
    End Function

    Private Sub UpdateControls()

        Me.m_cbUseSymbolicaNames.Checked = Me.m_plugin.UseSymbolicNames
        If Me.m_plugin.UseClipboard Then
            Me.m_rbClipboard.Checked = True
        Else
            Me.m_rbFile.Checked = True
        End If
        Me.m_cmbNetworkType.SelectedItem = Me.m_plugin.NetworkType

    End Sub

    Private Sub ApplySafe()
        Me.m_plugin.UseSymbolicNames = Me.m_cbUseSymbolicaNames.Checked
        Me.m_plugin.UseClipboard = Me.m_rbClipboard.Checked
        Me.m_plugin.NetworkType = CType(Me.m_cmbNetworkType.SelectedItem, eNetworkD3DiagramType)
    End Sub

End Class
