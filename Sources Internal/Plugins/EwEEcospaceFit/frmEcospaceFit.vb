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

Imports System.Windows.Forms

Public Class frmEcospaceFit

    Private m_plugin As cEcospaceFitPlugin


    Public Sub New()
        MyBase.New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Friend Sub Init(BasePlugin As cEcospaceFitPlugin)
        Me.m_plugin = BasePlugin
    End Sub


    Private Sub OnRunStarted()

    End Sub


    Private Sub OnRunCompleted()

        m_grdFit.RefreshContent()

    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_grdFit.UIContext = Me.UIContext
        Me.m_grdFit.EcospaceFit = Me.m_plugin.Fit


        AddHandler Me.m_plugin.Fit.onRunStarted, AddressOf Me.OnRunStarted
        AddHandler Me.m_plugin.Fit.onRunCompleted, AddressOf Me.OnRunCompleted

        Me.m_grdFit.RefreshContent()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        RemoveHandler Me.m_plugin.Fit.onRunStarted, AddressOf Me.OnRunStarted
        RemoveHandler Me.m_plugin.Fit.onRunCompleted, AddressOf Me.OnRunCompleted
    End Sub


    Private Sub onbtClearClick(sender As System.Object, e As System.EventArgs) Handles m_btClear.Click
        Try
            Me.m_plugin.Fit.Clear()
            Me.m_grdFit.RefreshContent()
        Catch ex As Exception

        End Try
    End Sub
End Class