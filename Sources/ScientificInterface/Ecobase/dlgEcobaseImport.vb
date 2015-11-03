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
Imports EwECore
Imports EwECore.WebServices
Imports EwECore.WebServices.Ecobase
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared

#End Region ' Imports

Public Class dlgEcobaseImport

#Region " Private vars "

    Private m_ecobase As cEcoBaseWDSL = Nothing
    Private m_models As New List(Of cModelData)
    Private m_model As cModelData = Nothing

#End Region ' Private vars

#Region " Construction "

    Public Sub New(uic As cUIContext)
        Me.InitializeComponent()
        Me.UIContext = uic
    End Sub

#End Region ' Construction

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.CenterToScreen()

        Me.m_ecobase = New cEcoBaseWDSL()
        Me.m_wrkGetModels.RunWorkerAsync(Nothing)

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)
        e.Cancel = (Me.DialogResult = Windows.Forms.DialogResult.OK) And Not Me.CanDownload
        MyBase.OnFormClosing(e)
    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)

        Me.m_ecobase.CancelAsync(Nothing)
        Me.m_ecobase.Dispose()
        Me.m_ecobase = Nothing

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Dim bIsBusy As Boolean = Me.m_wrkGetModels.IsBusy
        Me.m_btnOK.Enabled = Me.CanDownload

    End Sub

#End Region ' Form overrides

#Region " Public access "

    Public ReadOnly Property SelectedModel As cModelData
        Get
            Return Me.m_model
        End Get
    End Property

    Public ReadOnly Property CanDownload As Boolean
        Get
            Dim bCanDownload As Boolean = False
            If (Me.m_model IsNot Nothing) Then
                bCanDownload = (Me.m_model.AllowDissemination And Me.m_cbAccept.Checked)
            End If
            Return bCanDownload
        End Get
    End Property

#End Region ' Public access

#Region " Control events "

    Private Sub OnModelFormat(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_lbxModels.Format

        If (e.ListItem Is Nothing) Then Return

        Dim model As cModelData = DirectCast(e.ListItem, cModelData)
#If DEBUG Then
        e.Value = model.Name & " (" & model.EcobaseCode & ")"
#Else
        e.Value = model.Name
#End If

    End Sub

    Private Sub OnModelSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_lbxModels.SelectedIndexChanged

        Dim weblinks As New cWebLinks(Me.Core)
        Dim strModel As String = ""

        Try
            If (Me.m_lbxModels.SelectedIndex > -1) Then
                Me.m_model = DirectCast(Me.m_lbxModels.SelectedItem, cModelData)
                Dim strURL As String = String.Format(weblinks.GetURL(cWebLinks.eLinkType.EcoBaseModelInfo), Me.m_model.EcobaseCode)
                Me.m_browser.Navigate(strURL)
                Me.m_browser.Refresh(WebBrowserRefreshOption.Completely)
            Else
                Me.m_model = Nothing
            End If
        Catch ex As Exception

        End Try

        Me.UpdateControls()

    End Sub

    Private Sub OnAcceptAgreement(sender As System.Object, e As System.EventArgs) _
        Handles m_cbAccept.CheckedChanged
        Try
            Me.UpdateControls()
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnReadAgreement(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) _
        Handles m_llViewEcobaseDataAgreement.LinkClicked

        Try

            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            Debug.Assert(cmd IsNot Nothing)

            cmd.Invoke("")

        Catch ex As Exception
            cLog.Write(ex, "dlgEcobaseImport.OnViewTermsConditions")
        End Try

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click, m_lbxModels.DoubleClick

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

#End Region ' Control events

#Region " Background workers "

    Private Sub OnGetModels(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) _
        Handles m_wrkGetModels.DoWork

        Dim msg As cMessage = Nothing

        cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_LOADING, -1)
        Me.m_models.Clear()

        Try
            Dim strModels As String = Me.m_ecobase.list_models("", Nothing)
            Dim data As cEcobaseModelList = cEcobaseModelList.FromXML(strModels)
            Me.m_models.AddRange(data.Models)

        Catch exWeb As Net.WebException
            msg = New cMessage(My.Resources.ECOBASE_ERROR_NOCONNECTION, eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
        Catch ex As Exception
            msg = New cMessage(String.Format(My.Resources.ECOBASE_ERROR_COMMUNICATION, ex.Message), _
                                    eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
        End Try

        If (msg IsNot Nothing) Then
            Me.Core.Messages.SendMessage(msg)
        End If

        cApplicationStatusNotifier.EndProgress(Me.Core)

    End Sub

    Private Sub OnGetModelsCompleted(sender As Object, _
                                     e As System.ComponentModel.RunWorkerCompletedEventArgs) _
        Handles m_wrkGetModels.RunWorkerCompleted

        Me.UpdateModelList()
        Me.UpdateControls()

    End Sub

#End Region ' Background workers

#Region " Internals "

    Private Sub UpdateModelList()

        Me.m_lbxModels.SuspendLayout()
        Me.m_lbxModels.Items.Clear()

        For Each model As cModelData In Me.m_models
            If (model.AllowDissemination) Then
                Me.m_lbxModels.Items.Add(model)
                If (Me.m_model Is Nothing) Then Me.m_lbxModels.SelectedItem = model
            End If
        Next
        Me.m_lbxModels.ResumeLayout()

    End Sub

#End Region ' Internals

End Class