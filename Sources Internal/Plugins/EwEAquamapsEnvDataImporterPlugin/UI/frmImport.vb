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
' Copyright 1991- Ecopath International Initiative, Barcelona, Spain and
'                 Joint Reseach Centre, Ispra, Italy.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' <summary>
''' Main UI for the Aquamaps distribution envelope import plug-in
''' </summary>
Public Class frmImport

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_data As cImportData = Nothing
    Private m_bDragOver As Boolean = False

#End Region ' Private vars 

#Region " Construction "

    Public Sub New(uic As cUIContext)
        MyBase.New()
        Me.m_uic = uic
        Me.m_data = New cImportData()
        Me.InitializeComponent()
    End Sub

#End Region ' Construction

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.UpdateControls()
        Me.CenterToParent()
    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Form overrides

#Region " Events "

    Private Sub OnDragDropFiles(sender As Object, e As System.Windows.Forms.DragEventArgs) _
        Handles m_lblDrop.DragDrop
        Try
            If Not Me.m_bDragOver Then Return
            Me.ReadFiles(CType(e.Data.GetData(DataFormats.FileDrop), String()))
        Catch ex As Exception
        End Try
        Me.m_bDragOver = False
        Me.UpdateControls()
    End Sub

    Private Sub OnDragEnterFiles(sender As Object, e As System.Windows.Forms.DragEventArgs) _
        Handles m_lblDrop.DragEnter

        Try
            If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
                e.Effect = DragDropEffects.All
                Me.m_bDragOver = True
            End If
        Catch ex As Exception
            Me.m_bDragOver = False
        End Try
        Me.UpdateControls()

    End Sub

    Private Sub OnDragLeaveFiles(sender As Object, e As System.EventArgs) _
        Handles m_lblDrop.DragLeave

        Try
            Me.m_bDragOver = False
        Catch ex As Exception

        End Try
        Me.UpdateControls()

    End Sub

    Private Sub OnImport(sender As System.Object, e As System.EventArgs) _
        Handles m_btnImport.Click

        Try
            Dim imp As New cImporter(Me.m_data, Me.m_uic)

            Dim lstrSpecies As New List(Of String)
            For Each strSpecies As String In Me.m_clbxSpecies.CheckedItems
                lstrSpecies.Add(strSpecies)
            Next

            Dim lstrEnvelopes As New List(Of String)
            For Each strEnvelope As String In Me.m_clbxEnvelopes.CheckedItems
                lstrEnvelopes.Add(strEnvelope)
            Next

            imp.Import(lstrSpecies.ToArray, lstrEnvelopes.ToArray)
            Me.Clear()

        Catch ex As Exception
            ' Whoah!
        End Try

    End Sub

    Private Sub OnAquamapsLinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) _
        Handles m_llAquamaps.LinkClicked

        Try

            Dim cmd As cBrowserCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            If (cmd IsNot Nothing) Then
                cmd.Invoke("http://aquamaps.org")
            End If

        Catch ex As Exception
            cLog.Write(ex, "AquamapsImporter::OnAquamapsLinkClicked")
        End Try

    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub Clear()
        Me.m_data.Clear()
        Me.m_clbxSpecies.Items.Clear()
        Me.m_clbxEnvelopes.Items.Clear()
    End Sub

    Private Sub ReadFiles(files As String())

        Dim reader As New cFileReader(Me.m_uic.Core)

        Me.Clear()

        For Each strFile As String In files
            reader.ReadEnvelopeData(strFile, Me.m_data)
        Next

        For Each strSpecies As String In Me.m_data.Species
            Dim i As Integer = Me.m_clbxSpecies.Items.Add(strSpecies)
            Me.m_clbxSpecies.SetItemChecked(i, True)
        Next

        For Each strEnv As String In Me.m_data.Envelopes
            Dim i As Integer = Me.m_clbxEnvelopes.Items.Add(strEnv)
            Me.m_clbxEnvelopes.SetItemChecked(i, True)
        Next

        Me.UpdateControls()

    End Sub

    Private Sub UpdateControls()

        Me.m_btnImport.Enabled = (Me.m_data.Files.Length > 0)
        If Me.m_bDragOver Then
            Me.m_lblDrop.BackColor = SystemColors.Highlight
        Else
            Me.m_lblDrop.BackColor = Drawing.Color.Transparent
        End If

    End Sub

#End Region ' Internals

End Class