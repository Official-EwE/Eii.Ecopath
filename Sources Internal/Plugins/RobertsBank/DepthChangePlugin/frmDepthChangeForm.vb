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

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmEwEPlugin

    Private m_plugin As cDepthChangePluginPoint
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

        If (Me.Core IsNot Nothing) Then

            Dim model As cEwEModel = Me.Core.EwEModel

            Me.UpdateControls()

        End If

    End Sub


    Public Sub Init(ByVal PluginPoint As cDepthChangePluginPoint)
        m_plugin = PluginPoint
    End Sub


    Private Sub m_btConfigDepth_Click(sender As System.Object, e As System.EventArgs) Handles m_btConfigDepth.Click
        Try
            Dim configFile As String
            Dim OpenFileDialog As New OpenFileDialog
            OpenFileDialog.DefaultExt = "xml"
            OpenFileDialog.Filter = ".xml files|*.xml|All files (*.*)|*.*"
            OpenFileDialog.FilterIndex = 0

            If OpenFileDialog.ShowDialog() = DialogResult.OK Then
                configFile = OpenFileDialog.FileName
                If Me.m_plugin.SpatialDataLoader.LoadSpatialConfigFile(configFile) Then
                    Me.UpdateControls()
                Else
                    MsgBox("Failed to load the Spatial Configuration file.", MsgBoxStyle.Critical, "Warning")
                End If ' Me.m_plugin.SpatialDataLoader.LoadSpatialConfigFile(configFile)
            End If 'DialogResult.OK

        Catch ex As Exception

        End Try

    End Sub

    Private Overloads Sub UpdateControls()
        MyBase.UpdateControls()

        Try
            Me.m_lbConfigFile.Text = Me.m_plugin.SpatialDataLoader.SpatialConfigFile

            Me.m_lstDatasets.Items.Clear()
            For Each DataSet In Me.m_plugin.SpatialDataLoader.DataSets
                Me.m_lstDatasets.Items.Add(DataSet.DisplayName)
            Next DataSet

        Catch ex As Exception

        End Try

    End Sub


    Private Sub m_btLoadDepthDataset_Click(sender As System.Object, e As System.EventArgs) Handles m_btLoadDepthDataset.Click
        Try

            If Me.m_lstDatasets.SelectedItem IsNot Nothing Then
                Me.m_plugin.SpatialDataLoader.DepthDataSetName = Me.m_lstDatasets.SelectedItem.ToString
                Me.m_plugin.SpatialDataLoader.InitDepthDataSet()
            End If

        Catch ex As Exception

        End Try
    End Sub
End Class