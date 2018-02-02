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
#Region " imports "

Option Strict On
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

#End Region ' imports

Public Class ucNetworkD3Options
    Implements IOptionsPage

    Private m_plugin As cNetworkD3RWriterPlugin = Nothing

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
        Return IOptionsPage.eApplyResultType.Success

        ' For some bizarre reason the body of this method is skipped

        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf ApplySafe))
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
        Me.m_cbUseClipboard.Checked = Me.m_plugin.UseClipboard
        Me.m_cmbNetworkType.SelectedItem = Me.m_plugin.NetworkType

    End Sub

    Private Sub ApplySafe()
        Me.m_plugin.UseSymbolicNames = Me.m_cbUseSymbolicaNames.Checked
        Me.m_plugin.UseClipboard = Me.m_cbUseClipboard.Checked
        Me.m_plugin.NetworkType = CType(Me.m_cmbNetworkType.SelectedItem, eNetworkD3DiagramType)
    End Sub

End Class
