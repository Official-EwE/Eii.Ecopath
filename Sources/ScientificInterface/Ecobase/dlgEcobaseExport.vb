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

Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Web.Services
Imports EwECore
Imports EwECore.WebServices
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' Dialog to allow users to submit a model to Ecobase
''' </summary>
''' ---------------------------------------------------------------------------
Public Class dlgEcobaseExport

#Region " Private vars "

    Private m_uic As cUIContext = Nothing

#End Region ' Private vars

#Region " Construction "

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic
        Me.InitializeComponent()
    End Sub

#End Region ' Construction

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Dim core As cCore = Me.m_uic.Core
        Dim model As cEwEModel = core.EwEModel

        Me.m_tbxModel.Text = model.Name
        Me.m_tbxDescription.Text = model.Description
        Me.m_tbxAuthor.Text = cSystemUtils.IIF(String.IsNullOrWhiteSpace(model.Author), core.DefaultAuthor, model.Author)
        Me.m_tbxEmail.Text = cSystemUtils.IIF(String.IsNullOrWhiteSpace(model.Contact), core.DefaultContact, model.Contact)
        Me.m_tbxHyperlink.Text = model.PublicationURI
        Me.m_tbxDOI.Text = model.PublicationDOI

        Me.m_cbIsUpdate.Checked = (Not String.IsNullOrWhiteSpace(model.EcobaseCode))

        Me.CenterToParent()
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)

        ' NOP

    End Sub

#End Region ' Overrides

#Region " Event handlers "

    Private Sub OnContentChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbConfirmAuthor.CheckedChanged, m_tbxAuthor.TextChanged, _
                m_tbxModel.TextChanged, m_tbxEmail.TextChanged, m_tbxDescription.TextChanged, _
                m_tbxDOI.TextChanged, m_tbxHyperlink.TextChanged

        Try
            Me.UpdateControls()
        Catch ex As Exception
            cLog.Write(ex, "dlgEcobaseExport.OnContentChanged")
        End Try

    End Sub

    Private Sub OnViewPublication(sender As System.Object, e As System.EventArgs) _
        Handles m_llViewPublication.Click

        Dim strDOI As String = Me.m_tbxDOI.Text

        Try

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            Debug.Assert(cmd IsNot Nothing)

            cmd.Invoke("http://doi.org/" & HttpUtility.UrlEncode(strDOI))

        Catch ex As Exception
            cLog.Write(ex, "dlgEcobaseExport.OnViewDOIOnline(" & strDOI & ")")
        End Try

    End Sub

    Private Sub OnViewDataAgreement(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) _
        Handles m_llViewEcobaseDataAgreement.LinkClicked

        Try

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            Debug.Assert(cmd IsNot Nothing)

            cmd.Invoke("http://sirs.agrocampus-ouest.fr/EcoBase/index.php?action=base")

        Catch ex As Exception
            cLog.Write(ex, "dlgEcobaseExport.OnViewTermsConditions")
        End Try

    End Sub

    Private Sub OnSubmit(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSubmit.Click

        Try

            If Not Me.UpdateModelParameters() Then Return
            If Not Me.SubmitToEcobase() Then Return

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

#End Region ' Event handlers

#Region " Internals "

    Private Sub UpdateControls()

        Dim bHasModelName As Boolean = (Me.m_tbxModel.Text.Trim().Length > 5)
        Dim bHasDescription As Boolean = (Me.m_tbxDescription.Text.Trim().Length > 5)
        Dim bHasAuthor As Boolean = (Me.m_tbxAuthor.Text.Trim().Length > 5) And (Me.m_tbxAuthor.Text.Trim().Contains(" "c))
        Dim bHasContact As Boolean = cStringUtils.IsEmail(Me.m_tbxEmail.Text)
        Dim bHasPublication As Boolean = (Me.m_tbxDOI.Text.Trim().Length > 5) Or (Me.m_tbxHyperlink.Text.Trim().Length > 12)

        Dim bIsAuthor As Boolean = (Me.m_cbConfirmAuthor.Checked = True)

        Me.m_pbModel.BackgroundImage = CType(cSystemUtils.IIF(bHasModelName, SharedResources.OK, SharedResources.Warning), Image)
        Me.m_pbDescription.BackgroundImage = CType(cSystemUtils.IIF(bHasDescription, SharedResources.OK, SharedResources.Warning), Image)
        Me.m_pbAuthor.BackgroundImage = CType(cSystemUtils.IIF(bHasAuthor And bHasContact, SharedResources.OK, SharedResources.Warning), Image)
        Me.m_pbPublication.BackgroundImage = CType(cSystemUtils.IIF(bHasPublication, SharedResources.OK, SharedResources.Warning), Image)
        Me.m_pbConfirmAuthor.BackgroundImage = CType(cSystemUtils.IIF(bIsAuthor, SharedResources.OK, SharedResources.Warning), Image)

        Me.m_llViewPublication.Enabled = bHasPublication
        Me.m_btnSubmit.Enabled = bHasModelName And bHasDescription And bHasAuthor And bHasPublication And bIsAuthor

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store updated user input into the EwE model and save the changes.
    ''' </summary>
    ''' <returns>
    ''' True if successful.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function UpdateModelParameters() As Boolean

        Dim strName As String = Me.m_tbxModel.Text
        Dim strDescr As String = Me.m_tbxDescription.Text
        Dim strAuthor As String = Me.m_tbxAuthor.Text
        Dim strContact As String = Me.m_tbxEmail.Text
        Dim strDOI As String = Me.m_tbxDOI.Text
        Dim strURI As String = Me.m_tbxHyperlink.Text

        Dim core As cCore = Me.m_uic.Core
        Dim model As cEwEModel = core.EwEModel

        Dim bChange As Boolean = (String.Compare(strName, model.Name) <> 0) Or _
                                 (String.Compare(strAuthor, model.Author) <> 0) Or _
                                 (String.Compare(strContact, model.Contact) <> 0) Or _
                                 (String.Compare(strDescr, model.Description) <> 0) Or _
                                 (String.Compare(strDOI, model.PublicationDOI) <> 0) Or _
                                 (String.Compare(strURI, model.PublicationURI) <> 0)

        Dim bSucces As Boolean = True

        If bChange Then

            model.Name = strName
            model.Description = strDescr
            model.Author = strAuthor
            model.Contact = strContact
            model.PublicationDOI = strDOI
            model.PublicationURI = strURI

            bSucces = core.SaveChanges(True, cCore.eBatchChangeLevelFlags.Ecopath)

        End If

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store a valid Ecobase model number into the model.
    ''' </summary>
    ''' <param name="strNumber">The model number to store.</param>
    ''' <returns>
    ''' True if successful.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function UpdateModelNumber(ByVal strNumber As String) As Boolean

        Dim core As cCore = Me.m_uic.Core
        Dim model As cEwEModel = core.EwEModel

        If (String.IsNullOrWhiteSpace(strNumber)) Then Return False
        If (String.Compare(strNumber, model.EcobaseCode) <> 0) Then
            model.EcobaseCode = strNumber
            Return core.SaveChanges(True, cCore.eBatchChangeLevelFlags.Ecopath)
        End If

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sends the current model to Ecobase.
    ''' </summary>
    ''' <returns>
    ''' True if successful.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function SubmitToEcobase() As Boolean

        Dim core As cCore = Me.m_uic.Core
        Dim msg As cMessage = Nothing
        Dim wdsl As New cEcoBaseWDSL()
        Dim bSucces As Boolean = True

        ' Sanity checks
        Debug.Assert(core.StateMonitor.HasEcopathRan)

        ' Prepare data to send to Ecobase
        Dim data As New WebServices.Ecobase.cEcobaseModelParameters(core)
        Dim md As Ecobase.cModelData = data.Model

        md.AllowDissemination = Me.m_cbConfirmDessiminate.Checked
        md.IsUpdate = Me.m_cbIsUpdate.Checked

        ' Obtain XML
        Dim strXML As String = WebServices.Ecobase.cEcobaseModelParameters.ToXML(data)

#If DEBUG Then
        ' Store outgoing XML for debugging purposes
        Dim strFile As String = Path.GetFullPath(".\Ecobase_export.xml")
        Dim writer As New StreamWriter(strFile)
        writer.Write(strXML)
        writer.Close()

        msg = New cMessage("Ecobase export XML saved to " & strFile, eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        msg.Hyperlink = Path.GetDirectoryName(strFile)
        core.Messages.SendMessage(msg)
        msg = Nothing
#End If

        Try
            strXML = wdsl.Upload_Model(1, strXML)

            ' Analyse result
            Dim results As Ecobase.cEcobaseSubmission = Ecobase.cEcobaseSubmission.FromXML(strXML)

            Select Case results.ResultType
                Case Ecobase.cEcobaseSubmission.eSubmisssionResultTypes.NotInEcobase
                    msg = New cMessage(My.Resources.ECOBASE_SUBMIT_DENIED, eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)

                Case Ecobase.cEcobaseSubmission.eSubmisssionResultTypes.Pending
                    msg = New cFeedbackMessage(My.Resources.ECOBASE_SUBMIT_REVIEW, eCoreComponentType.External, eMessageType.DataExport, eMessageImportance.Information, eMessageReplyStyle.OK)

                Case Ecobase.cEcobaseSubmission.eSubmisssionResultTypes.Accepted
                    msg = New cFeedbackMessage(My.Resources.ECOBASE_SUBMIT_ACCEPTED, eCoreComponentType.External, eMessageType.DataExport, eMessageImportance.Information, eMessageReplyStyle.OK)

            End Select

            Me.UpdateModelNumber(results.ModelNumber)

        Catch ex As WebException
            bSucces = False
            msg = New cMessage(My.Resources.ECOBASE_ERROR_NOCONNECTION, _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
        Catch ex As Exception
            bSucces = False
            msg = New cMessage(cStringUtils.Localize(My.Resources.ECOBASE_ERROR_COMMUNICATION, ex.Message), _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
        End Try

        If (msg IsNot Nothing) Then
            core.Messages.SendMessage(msg)
        End If

        Return bSucces

    End Function


#End Region ' Internals

End Class