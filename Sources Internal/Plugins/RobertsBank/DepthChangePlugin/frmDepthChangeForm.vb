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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' <summary>
''' Form to config the depth change plug-in.
''' </summary>
Public Class frmEwEPlugin

    Private m_plugin As cDepthChangePluginPoint

    Public Sub New(ByVal plugin As cDepthChangePluginPoint)
        Me.InitializeComponent()
        Me.m_plugin = plugin
        Me.Text = Me.m_plugin.ControlText
        Me.TabText = Me.m_plugin.ControlText
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
        If (Me.Core Is Nothing) Then Return
        Me.m_lbConfigFile.Text = My.Settings.MRUConfig
        Me.UpdateControls()
    End Sub

    Private Sub OnOpenConfigFile(sender As System.Object, e As System.EventArgs) _
        Handles m_btConfigDepth.Click
        Try
            Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog("Select spatial configuration file", _
                                                                            Me.m_lbConfigFile.Text, _
                                                                            "Spatial configuration files|*.xml|All files (*.*)|*.*")
            If (ofd.ShowDialog() = DialogResult.OK) Then
                If Me.m_plugin.SpatialDataLoader.LoadSpatialConfigFile(ofd.FileName) Then
                    Me.UpdateControls()
                End If ' Me.m_plugin.SpatialDataLoader.LoadSpatialConfigFile(configFile)
            End If 'DialogResult.OK

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnLoadDataset(sender As System.Object, e As System.EventArgs) _
        Handles m_btLoadDepthDataset.Click
        Try
            If (Me.m_lstDatasets.SelectedItem IsNot Nothing) Then
                Me.m_plugin.SpatialDataLoader.DepthDataSetName = Me.m_lstDatasets.SelectedItem.ToString
                Me.m_plugin.SpatialDataLoader.InitDepthDataSet()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Try
            Me.m_lbConfigFile.Text = Me.m_plugin.SpatialDataLoader.SpatialConfigFile

            Me.m_lstDatasets.Items.Clear()
            For Each ds In Me.m_plugin.SpatialDataLoader.DataSets
                Me.m_lstDatasets.Items.Add(ds.DisplayName)
            Next ds
        Catch ex As Exception

        End Try

    End Sub

End Class