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

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style.cStyleGuide

#End Region ' Imports

Public Class frmTransectWriter

    Private m_data As cTransectDatastructures = Nothing
    Private m_layer As cDisplayLayerTransect = Nothing
    Private m_fpX1 As cEwEFormatProvider = Nothing
    Private m_fpY1 As cEwEFormatProvider = Nothing
    Private m_fpX2 As cEwEFormatProvider = Nothing
    Private m_fpY2 As cEwEFormatProvider = Nothing

    Public Sub New(uic As cUIContext, data As cTransectDatastructures)
        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_data = data

        Me.Text = My.Resources.CAPTION
        Me.TabText = Me.Text
    End Sub

#Region " Form overrides "

    Public Overrides Property UIContext As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As cUIContext)
            MyBase.UIContext = value
            Me.m_mapzoom.UIContext = Me.UIContext
        End Set
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim factory As New cLayerFactoryBase()

        Me.m_layer = New cDisplayLayerTransect(Me.UIContext)
        Me.m_layer.IsSelected = True
        Me.m_layer.Data = Me.m_data
        Me.m_mapzoom.Map.Editable = True
        Me.m_mapzoom.Map.AddLayer(Me.m_layer)

        For Each l As cDisplayLayer In factory.GetLayers(Me.UIContext, eVarNameFlags.LayerDepth)
            l.RenderMode = ScientificInterfaceShared.Definitions.eLayerRenderType.Always
            Me.m_mapzoom.Map.AddLayer(l)
        Next

        Me.m_fpX1 = New cEwEFormatProvider(Me.UIContext, m_tbxX1, GetType(Single))
        Me.m_fpY1 = New cEwEFormatProvider(Me.UIContext, m_tbxY1, GetType(Single))
        Me.m_fpX2 = New cEwEFormatProvider(Me.UIContext, m_tbxX2, GetType(Single))
        Me.m_fpY2 = New cEwEFormatProvider(Me.UIContext, m_tbxY2, GetType(Single))

        For Each t As cTransect In Me.m_data.Transects
            Me.m_lbxTransects.Items.Add(t)
        Next

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Dim bHasName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxTName.Text)

    End Sub

#End Region ' Form overrides

#Region " Events "

    Private Sub m_tbxTName_TextChanged(sender As Object, e As EventArgs) Handles m_tbxTName.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub m_btnTAdd_Click(sender As Object, e As EventArgs) Handles m_btnTAdd.Click
        Dim t As New cTransect()
        Me.UpdateTransect(t)
        Me.m_data.Transects.Add(t)

        Me.m_lbxTransects.Items.Add(t)
        Me.m_lbxTransects.SelectedItem = t
        Me.UpdateMap()
    End Sub

    Private Sub m_btnTRename_Click(sender As Object, e As EventArgs) Handles m_btnTRename.Click
        Me.UpdateTransect(Me.SelectedTransect)
        Me.UpdateMap()
    End Sub

    Private Sub m_btnTDelete_Click(sender As Object, e As EventArgs) Handles m_btnTDelete.Click
        Dim t As cTransect = Me.SelectedTransect()
        Me.m_data.Transects.Remove(t)
        Me.m_lbxTransects.Items.Remove(t)
        Me.UpdateMap()
    End Sub

    Private Sub m_lbxTransects_SelectedIndexChanged(sender As Object, e As EventArgs) Handles m_lbxTransects.SelectedIndexChanged
        Dim t As cTransect = Me.SelectedTransect()
        If (t IsNot Nothing) Then
            Me.m_tbxTName.Text = t.Name
            Me.m_fpX1.Value = t.Start.X
            Me.m_fpY1.Value = t.Start.Y
            Me.m_fpX2.Value = t.End.X
            Me.m_fpY2.Value = t.End.Y
        Else
            Me.m_tbxTName.Text = ""
            Me.m_fpX1.Style = eStyleFlags.Null
            Me.m_fpY1.Style = eStyleFlags.Null
            Me.m_fpX2.Style = eStyleFlags.Null
            Me.m_fpY2.Style = eStyleFlags.Null
        End If
        Me.m_data.Selection = t
        Me.UpdateMap()
    End Sub

    Private Sub m_cbAutosave_CheckedChanged(sender As Object, e As EventArgs) Handles m_cbAutosave.CheckedChanged

    End Sub

#End Region ' Events

#Region " Internals "

    Private Function SelectedTransect() As cTransect
        Return DirectCast(Me.m_lbxTransects.SelectedItem, cTransect)
    End Function

    Private Sub UpdateTransect(t As cTransect)

        If (t Is Nothing) Then Return
        t.Name = Me.m_tbxTName.Text
        t.Start = New PointF(CSng(Me.m_fpX1.Value), CSng(Me.m_fpY1.Value))
        t.End = New PointF(CSng(Me.m_fpX2.Value), CSng(Me.m_fpY2.Value))

        Me.UpdateMap()

    End Sub

    Private Sub UpdateMap()
        Me.m_mapzoom.Map.Refresh()
    End Sub

#End Region ' Internals

End Class