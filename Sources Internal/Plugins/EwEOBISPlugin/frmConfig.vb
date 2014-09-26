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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Option Strict On
Imports System.Windows.Forms

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for configuring a WoRMS web service connection.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmConfig

    ''' <summary>Plug-in to configure.</summary>
    Private m_plugin As cOBISPluginPoint = Nothing

    Public Sub New(ByVal plugin As cOBISPluginPoint)
        MyBase.New()
        Me.m_plugin = plugin
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.m_nudConnTO.Value = Me.m_plugin.ConnectionTimeOut
        Me.m_nudReplyTO.Value = Me.m_plugin.ResponseTimeOut
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub m_btnOK_Click(sender As System.Object, e As System.EventArgs) Handles m_btnOK.Click

        Me.m_plugin.ConnectionTimeOut = CInt(Me.m_nudConnTO.Value)
        Me.m_plugin.ResponseTimeOut = CInt(Me.m_nudReplyTO.Value)

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

End Class