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
Imports EwECore
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports System.Drawing

#End Region ' Imports

''' <summary>
''' </summary>
Public Class frmKeyRunMain

#Region " Private vars "

    Private m_CompManager As cCompareManager

#End Region ' Private vars

#Region " Construction "

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(ByVal manager As cCompareManager)
        Me.m_CompManager = manager
        Me.m_grid.ComparisonManager = manager
    End Sub

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
            Me.m_grid.UIContext = value
        End Set
    End Property

#End Region ' Construction

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.m_CompManager Is Nothing) Then Return

        Me.UpdateControls()
        AddHandler Me.m_CompManager.OnChanged, AddressOf OnHashComputationStateChanged

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        RemoveHandler Me.m_CompManager.OnChanged, AddressOf OnHashComputationStateChanged

        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()

        Me.m_lblKeyRunFile.Text = Me.m_CompManager.KeyRunFile

        Dim strMatch As String = My.Resources.STATUS_MATCH_UNKNOWN
        Dim bmpMatch As Bitmap = Nothing

        If (Me.m_CompManager.Results IsNot Nothing) Then
            Select Case Me.m_CompManager.Results.Match
                Case cHashResults.eMatchState.Match
                    strMatch = My.Resources.STATUS_MATCH_SUCCESS
                    bmpMatch = My.Resources.Check
                Case cHashResults.eMatchState.NoMatch
                    strMatch = My.Resources.STATUS_MATCH_FAILED
                    bmpMatch = My.Resources.Uncheck
                Case cHashResults.eMatchState.NotSet
                    ' NOP
            End Select
        End If
        Me.m_lbRunStatus.Text = strMatch
        Me.m_pbStatus.BackgroundImage = bmpMatch
        Me.m_pbStatus.BackgroundImageLayout = ImageLayout.Zoom

        'this should ask the CompManager if it has a valid KeyRunFile
        If IO.File.Exists(Me.m_CompManager.KeyRunFile) Then
            Me.m_btnCompare.Enabled = True
        Else
            Me.m_btnCompare.Enabled = False
        End If

    End Sub

#End Region ' Form overrides

#Region " Control events "

    Private Sub OnStatusPictureClicked(sender As System.Object, e As System.EventArgs) _
        Handles m_pbStatus.Click

    End Sub

    Private Sub OnSaveKeyRunFile(sender As System.Object, e As System.EventArgs) Handles m_btnSave.Click

        Dim sfd As SaveFileDialog = Nothing

        sfd = cEwEFileDialogHelper.SaveFileDialog(My.Resources.PROMPT_KEYRUN_SAVE, _
                                                  Me.m_CompManager.DefaultKeyRunFileName, _
                                                  My.Resources.FILE_INDEX_KEYRUN, _
                                                  0, _
                                                  Me.m_CompManager.DefaultKeyRunFileLocation)

        If (sfd.ShowDialog = DialogResult.OK) Then
            Me.m_CompManager.SaveKeyRunFile(sfd.FileName)
        End If

    End Sub

    Private Sub OnReadKeyRunFile(sender As System.Object, e As System.EventArgs) Handles m_btnLoad.Click

        Dim ofd As OpenFileDialog = Nothing

        ofd = cEwEFileDialogHelper.OpenFileDialog(My.Resources.PROMPT_KEYRUN_LOAD, _
                                                  Me.m_CompManager.KeyRunFile, _
                                                  My.Resources.FILE_INDEX_KEYRUN, _
                                                  0)

        If (ofd.ShowDialog = DialogResult.OK) Then
            Me.m_CompManager.LoadKeyRun(ofd.FileName)
        End If

    End Sub


    Private Sub OnRunLoadedKeyRun(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCompare.Click, m_pbStatus.Click

        Try
            Me.m_CompManager.RunLoadedKeyRun()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnToggleShowErrors(sender As System.Object, e As System.EventArgs) _
        Handles m_cbShowErrorsOnly.CheckedChanged

        Try
            Me.m_grid.ShowErrorsOnly = Me.m_cbShowErrorsOnly.Checked
            Me.m_grid.RefreshContent()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnHashComputationStateChanged(ByVal man As cCompareManager)

        Me.m_grid.RefreshContent()
        Me.UpdateControls()

    End Sub

#End Region ' Control events

End Class