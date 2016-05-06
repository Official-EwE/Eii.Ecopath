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
' Copyright 1991-2012 UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

Imports EwECore
Imports System.Windows.Forms

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmHabCap

    Private m_plugin As cHabitatCapacityPluginPoint

    Friend Delegate Sub MessageUpdaterDelegate()
    Friend updater As MessageUpdaterDelegate

    Public Sub New()

        ' This call is required by the designer.
        Me.InitializeComponent()
        updater = New MessageUpdaterDelegate(AddressOf Me.postMessage)

    End Sub

    ''' <summary>
    ''' OnLoad is called when a form is about to go 'live'. It is the perfect place to
    ''' perform last moment configurations before the form is made visible to the user.
    ''' </summary>
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.WorkerThread.WorkerReportsProgress = True

    End Sub


    Public Sub Init(ByVal PluginPoint As cHabitatCapacityPluginPoint)
        m_plugin = PluginPoint
    End Sub


    Private Sub onRunClicked(sender As Object, e As System.EventArgs) Handles m_btRun.Click

        If Me.WorkerThread.IsBusy Then
            MsgBox("Sorry already running.", MsgBoxStyle.Information)
            Return
        End If

        Me.m_lstMessages.Items.Clear()
        Me.WorkerThread.RunWorkerAsync()

    End Sub


    Friend Sub postMessage()

        ' Me.EndInvoke()
        Me.m_lstMessages.Items.Insert(0, Me.m_plugin.Message)
        Me.m_lstMessages.Refresh()
    End Sub


    Private Sub onStopClick(sender As Object, e As System.EventArgs) Handles m_btStop.Click
        Me.m_plugin.bStopRun = True
    End Sub

    Private Sub RunHabCapModel(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles WorkerThread.DoWork
        Me.m_plugin.HabitatCapacityModel()
    End Sub

    Private Sub onBGW_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles WorkerThread.ProgressChanged
        Me.m_lstMessages.Items.Insert(0, Me.m_plugin.Message)
        Me.m_lstMessages.Refresh()
    End Sub

    Private Sub onBGW_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles WorkerThread.RunWorkerCompleted

    End Sub


    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return True
        End Get
    End Property

End Class