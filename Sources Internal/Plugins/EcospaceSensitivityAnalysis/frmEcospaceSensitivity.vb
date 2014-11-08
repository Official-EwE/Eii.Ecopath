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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Imports EwECore
Imports System.Windows.Forms
Imports System.IO

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmEcospaceSensitivity

    Private m_plugin As cEcospaceSensitivityPluginPoint

    Private m_inInit As Boolean

    Public Sub New()

        ' This call is required by the designer.
        Me.InitializeComponent()


    End Sub



    Public Overrides ReadOnly Property IsRunForm As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' OnLoad is called when a form is about to go 'live'. It is the perfect place to
    ''' perform last moment configurations before the form is made visible to the user.
    ''' </summary>
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.Core IsNot Nothing) Then

            Dim model As cEwEModel = Me.Core.EwEModel

        End If

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If Me.RunManager IsNot Nothing Then

            Me.m_inInit = True

            'Me.m_txBeforeStart.Text = Me.RunManager.RunParameters.BeforeRun.StartYear.ToString
            'Me.m_txBeforeNYears.Text = Me.RunManager.RunParameters.BeforeRun.nYears.ToString
            'Me.m_txAfterStart.Text = Me.RunManager.RunParameters.AfterRun.StartYear.ToString
            'Me.m_txAfterNYears.Text = Me.RunManager.RunParameters.AfterRun.nYears.ToString

            'If Not String.IsNullOrWhiteSpace(Me.RunManager.RunParameters.OutputFileName) Then
            '    Me.m_lbOutputFile.Text = Path.GetFileName("'" + Me.RunManager.RunParameters.OutputFileName) + "' in directory '" + Path.GetDirectoryName(Me.RunManager.RunParameters.OutputFileName) + "'"
            'End If

            Me.m_inInit = False

        End If

    End Sub




    Private Sub UpdateParameters()

        If Me.m_inInit Then Return

        If Me.RunManager IsNot Nothing Then
            'Me.RunManager.RunParameters.BeforeRun.StartYear = Me.toInt(Me.m_txBeforeStart)
            'Me.RunManager.RunParameters.BeforeRun.nYears = Me.toInt(Me.m_txBeforeNYears)
            'Me.RunManager.RunParameters.AfterRun.StartYear = Me.toInt(Me.m_txAfterStart)
            'Me.RunManager.RunParameters.AfterRun.nYears = Me.toInt(Me.m_txAfterNYears)
        End If

    End Sub

    Private Function toInt(TextBox As TextBox) As Integer
        Dim value As Integer
        If Integer.TryParse(TextBox.Text, value) Then
            Return value
        End If
        Return 0
    End Function


    Private ReadOnly Property RunManager As cRunManager
        Get
            Return Me.m_plugin.RunManager
        End Get
    End Property

    Public Sub Init(ByVal PluginPoint As cEcospaceSensitivityPluginPoint)
        m_plugin = PluginPoint

        UpdateControls()

    End Sub


    Private Sub m_btOutput_Click(sender As System.Object, e As System.EventArgs)
        'Dim SFD As New SaveFileDialog

        'SFD.FileName = "RBT_Ecospace_MonteCarlo.csv"
        'SFD.Filter = "*.csv|*.csv|*.*|*.*"
        'SFD.FilterIndex = 0

        'SFD.OverwritePrompt = False

        'If SFD.ShowDialog = Windows.Forms.DialogResult.OK Then
        '    Dim filename As String = SFD.FileName

        '    If File.Exists(filename) Then
        '        If MsgBox("Selected output file already exists. Do you want to overwrite it?" + vbCrLf + "Yes to overwrite." + vbCrLf + "No to append new results.", _
        '            MsgBoxStyle.YesNo, "Ecospace MonteCarlo.") = MsgBoxResult.Yes Then
        '            Try
        '                File.Delete(filename)
        '                File.Delete(Me.RunManager.getEcopathParFile(filename))
        '            Catch ex As Exception

        '            End Try
        '        End If
        '    End If

        '    Me.m_plugin.RunManager.RunParameters.OutputFileName = filename

        '    Me.UpdateControls()

        'End If

    End Sub

End Class