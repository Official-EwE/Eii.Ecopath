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
Imports System.IO
Imports EwECore.SpatialData
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmSwitch

    Private m_man As EwECore.SpatialData.cSpatialDataSetManager = Nothing

    Public Sub New(uic As cUIContext)

        Me.UIContext = uic
        Me.m_man = Me.UIContext.Core.SpatialDataConnectionManager.DatasetManager
        Me.InitializeComponent()

    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_cmbDatasets.Items.Add("(default)")
        Me.m_cmbDatasets.SelectedIndex = 0

        If (My.Settings.MRU IsNot Nothing) Then
            For Each strFile As String In My.Settings.MRU
                If File.Exists(strFile) Then
                    Dim i As Integer = Me.m_cmbDatasets.Items.Add(strFile)
                    If String.Compare(strFile, Me.m_man.ConfigFile, True) = 0 Then
                        Me.m_cmbDatasets.SelectedIndex = i
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub m_btnOK_Click(sender As System.Object, e As System.EventArgs) Handles m_btnOK.Click
        Dim eng As New cEngine(Me.UIContext.Core)

        Dim strFile As String = Me.m_cmbDatasets.Text
        If strFile = "(default)" Then strFile = ""
        If eng.Switch(strFile) Then
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub m_btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles m_btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub m_lblDrop_OnFilesDropped(sender As Object, astrFiles() As String) _
        Handles m_lblDrop.OnFilesDropped

        Try
            Dim strFile As String = astrFiles(0)
            If Me.m_cmbDatasets.Items.Contains(strFile) Then Return
            Dim i As Integer = Me.m_cmbDatasets.Items.Add(strFile)
            Me.m_cmbDatasets.SelectedIndex = i
        Catch ex As Exception

        End Try

    End Sub

End Class