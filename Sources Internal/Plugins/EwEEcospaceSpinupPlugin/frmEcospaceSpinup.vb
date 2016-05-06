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

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Windows.Forms

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmEcospaceSpinup
    'ToDo 8-May-2014
    'Grid columns B(0), B(t), B(t)/B(0), B(t)/B(t-1)

    Private m_plugin As cEcospaceSpinupPlugin
    Private m_bInitializing As Boolean

    Public Sub New()

        ' This call is required by the designer.
        Me.InitializeComponent()


    End Sub

    ''' <summary>
    ''' OnLoad is called when a form is about to go 'live'. It is the perfect place to
    ''' perform last moment configurations before the form is made visible to the user.
    ''' </summary>
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
        m_bInitializing = True

        Try

            Me.m_gridSpinUpDif.UIContext = Me.UIContext
            Me.m_gridSpinUpDif.Init(Me.m_plugin)

            Me.UpdateControls()

            AddHandler Me.m_plugin.OnEcospaceTimeStep, AddressOf Me.OnTimeStep
            AddHandler Me.m_plugin.OnEcospaceRunStarting, AddressOf Me.OnRunStarted
            AddHandler Me.m_plugin.OnEcospaceRunCompleted, AddressOf Me.OnRunCompleted
        Catch ex As Exception

        End Try

        m_bInitializing = False
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        RemoveHandler Me.m_plugin.OnEcospaceTimeStep, AddressOf Me.OnTimeStep
        RemoveHandler Me.m_plugin.OnEcospaceRunStarting, AddressOf Me.OnRunStarted
    End Sub



    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return True
        End Get
    End Property


    Private Sub OnTimeStep()
        Try
            Me.m_gridSpinUpDif.OnTimeStep()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnRunStarted()
        Try
            Me.UpdateCore()
            Me.setEnabledState(False)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnRunCompleted()
        Try
            Me.setEnabledState(True)
        Catch ex As Exception

        End Try
    End Sub


    Public Sub Init(ByVal PluginPoint As cEcospaceSpinupPlugin)
        m_plugin = PluginPoint
    End Sub


    Private Sub m_chkUseSpinup_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles m_chkUseSpinup.CheckedChanged
        Try
            If Not Me.m_bInitializing Then
                Me.m_plugin.UseSpinUp = Me.m_chkUseSpinup.Checked
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub m_chkUseBaseBio_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles m_chkUseBaseBio.CheckedChanged
        Try
            If Not Me.m_bInitializing Then
                Me.m_plugin.UseSpinUpBaseBio = Me.m_chkUseBaseBio.Checked
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub m_txSpinUpYears_TextChanged(sender As System.Object, e As System.EventArgs) Handles m_txSpinUpYears.TextChanged
        Try
            If Not Me.m_bInitializing Then
                Dim value As Single
                If Single.TryParse(Me.m_txSpinUpYears.Text, value) Then
                    Me.m_plugin.SpinUpYears = value
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub UpdateCore()
        Try

            If Me.m_plugin IsNot Nothing Then
                'Update the Plugin State
                Me.m_plugin.UseSpinUp = Me.m_chkUseSpinup.Checked
                Single.TryParse(Me.m_txSpinUpYears.Text, Me.m_plugin.SpinUpYears)
                Me.m_plugin.UseSpinUpBaseBio = Me.m_chkUseBaseBio.Checked

            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub setEnabledState(ControlsEnabled As Boolean)
        Try
            Me.m_plControls.Enabled = ControlsEnabled
            Me.m_plControls.Refresh()
        Catch ex As Exception

        End Try
    End Sub


    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Try

            If Me.m_plugin IsNot Nothing Then
                'Update the controls to the Plugin State
                Me.m_chkUseSpinup.Checked = Me.m_plugin.UseSpinUp
                Me.m_txSpinUpYears.Text = Me.m_plugin.SpinUpYears.ToString
                Me.m_chkUseBaseBio.Checked = Me.m_plugin.UseSpinUpBaseBio
            End If

        Catch ex As Exception

        End Try

    End Sub


End Class