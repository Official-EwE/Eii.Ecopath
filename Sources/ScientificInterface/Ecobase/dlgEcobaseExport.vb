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
Imports System.Collections.Specialized

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' Dialog to allow users to submit a model to Ecobase
''' </summary>
''' ---------------------------------------------------------------------------
Public Class dlgEcobaseExport

#Region " Private vars "

    Private m_uic As cUIContext = Nothing

    Private m_fpNorth As cEwEFormatProvider = Nothing
    Private m_fpEast As cEwEFormatProvider = Nothing
    Private m_fpWest As cEwEFormatProvider = Nothing
    Private m_fpSouth As cEwEFormatProvider = Nothing

    Private m_fpDmin As cEwEFormatProvider = Nothing
    Private m_fpDmean As cEwEFormatProvider = Nothing
    Private m_fpDmax As cEwEFormatProvider = Nothing

    Private m_fpTmin As cEwEFormatProvider = Nothing
    Private m_fpTmean As cEwEFormatProvider = Nothing
    Private m_fpTmax As cEwEFormatProvider = Nothing

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

        Me.m_bInUpdate = True

        Dim il As New ImageList()
        il.Images.Add(SharedResources.OK)
        il.Images.Add(SharedResources.Warning)
        il.Images.Add(SharedResources.Critical)
        Me.m_tcExport.ImageList = il

        ' -- Model page --
        Me.m_tbxModel.Text = model.Name
        Me.m_tbxDescription.Text = model.Description
        Me.m_tbxObjectives.Text = model.Objectives
        Me.m_tbxAuthor.Text = cSystemUtils.IIF(String.IsNullOrWhiteSpace(model.Author), core.DefaultAuthor, model.Author)
        Me.m_tbxEmail.Text = cSystemUtils.IIF(String.IsNullOrWhiteSpace(model.Contact), core.DefaultContact, model.Contact)
        Me.m_tbxObjectives.Text = ""

        ' -- Publication page --
        Me.m_tbxHyperlink.Text = model.PublicationURI
        Me.m_tbxDOI.Text = model.PublicationDOI
        Me.m_tbxReference.Text = model.PublicationReference

        ' -- Classification page --
        Me.m_fpNorth = New cEwEFormatProvider(Me.m_uic, Me.m_nudNorth, GetType(Single), model.GetVariableMetadata(eVarNameFlags.North))
        Me.m_fpNorth.Value = model.North
        Me.m_fpEast = New cEwEFormatProvider(Me.m_uic, Me.m_nudEast, GetType(Single), model.GetVariableMetadata(eVarNameFlags.East))
        Me.m_fpEast.Value = model.East
        Me.m_fpWest = New cEwEFormatProvider(Me.m_uic, Me.m_nudWest, GetType(Single), model.GetVariableMetadata(eVarNameFlags.West))
        Me.m_fpWest.Value = model.West
        Me.m_fpSouth = New cEwEFormatProvider(Me.m_uic, Me.m_nudSouth, GetType(Single), model.GetVariableMetadata(eVarNameFlags.South))
        Me.m_fpSouth.Value = model.South

        Me.FillCombo(Me.m_cmbCountry, Me.m_uic.StyleGuide.EcoBaseFields(cStyleGuide.eEcobaseFieldType.CountryName))
        Me.FillCombo(Me.m_cmbRegion, Me.m_uic.StyleGuide.EcoBaseFields(cStyleGuide.eEcobaseFieldType.RegionName))

        Me.FillCombo(Me.m_cmbEcoCat, Me.m_uic.StyleGuide.EcoBaseFields(cStyleGuide.eEcobaseFieldType.EcosystemCategory))
        Me.FillCombo(Me.m_cmbEcoType, Me.m_uic.StyleGuide.EcoBaseFields(cStyleGuide.eEcobaseFieldType.EcosystemType))

        Dim mdDepth As New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        Me.m_fpDmin = New cEwEFormatProvider(Me.m_uic, Me.m_tbxDepthMin, GetType(Single), mdDepth)
        Me.m_fpDmin.Value = 0
        Me.m_fpDmean = New cEwEFormatProvider(Me.m_uic, Me.m_tbxDepthMean, GetType(Single), mdDepth)
        Me.m_fpDmean.Value = 0
        Me.m_fpDmax = New cEwEFormatProvider(Me.m_uic, Me.m_tbxDepthMax, GetType(Single), mdDepth)
        Me.m_fpDmax.Value = 0

        Dim mdTemp As New cVariableMetaData(-8, 100, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        Me.m_fpTmin = New cEwEFormatProvider(Me.m_uic, Me.m_tbxTempMin, GetType(Single), mdTemp)
        Me.m_fpTmin.Value = 0
        Me.m_fpTmean = New cEwEFormatProvider(Me.m_uic, Me.m_tbxTempMean, GetType(Single), mdTemp)
        Me.m_fpTmean.Value = 0
        Me.m_fpTmax = New cEwEFormatProvider(Me.m_uic, Me.m_tbxTempMax, GetType(Single), mdTemp)
        Me.m_fpTmax.Value = 0

        ' -- Agreement page --

        Me.m_cbIsUpdate.Checked = (Not String.IsNullOrWhiteSpace(model.EcobaseCode))

        Me.m_bInUpdate = False

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
        Handles m_tbxAuthor.TextChanged, _
                m_tbxModel.TextChanged, m_tbxEmail.TextChanged, m_tbxDescription.TextChanged, m_tbxObjectives.TextChanged, _
                m_tbxDOI.TextChanged, m_tbxHyperlink.TextChanged, m_tbxReference.TextChanged, _
                m_cbConfirmAuthor.CheckedChanged, m_cbConfirmDessiminate.CheckedChanged, _
                m_cbEcosimUsed.CheckedChanged, m_cbFittedToTimeSeries.CheckedChanged, m_cbEcospaceUsed.CheckedChanged, _
                m_tbxDepthMin.TextChanged, m_tbxDepthMean.TextChanged, m_tbxDepthMax.TextChanged, _
                m_tbxTempMin.TextChanged, m_tbxTempMean.TextChanged, m_tbxTempMax.TextChanged, _
                m_cmbCountry.TextChanged, m_cmbRegion.TextChanged, m_tbxLME.TextChanged, _
                m_nudNorth.ValueChanged, m_nudEast.ValueChanged, m_nudWest.ValueChanged, m_nudSouth.ValueChanged
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

    Private m_bInUpdate As Boolean = False

    Private Sub UpdateControls()

        If (Me.m_uic Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.m_bInUpdate = True

        Dim core As cCore = Me.m_uic.Core

        ' -- Model page --
        Dim bHasModelName As Boolean = (Me.m_tbxModel.Text.Trim().Length > 5)
        Dim bHasDescription As Boolean = (Me.m_tbxDescription.Text.Trim().Length > 5)
        Dim bHasAuthor As Boolean = (Me.m_tbxAuthor.Text.Trim().Length > 5) And (Me.m_tbxAuthor.Text.Trim().Contains(" "c))
        Dim bHasContact As Boolean = cStringUtils.IsEmail(Me.m_tbxEmail.Text)
        Dim bIsAuthor As Boolean = (Me.m_cbConfirmAuthor.Checked = True)
        Dim bHasObjectives As Boolean = (Me.m_tbxObjectives.Text.Trim().Length > 15)
        Dim bModelOK As Boolean = bHasModelName And bHasDescription And bHasAuthor And bIsAuthor And bHasObjectives

        Me.m_pbModel.BackgroundImage = cSystemUtils.IIF(bHasModelName, SharedResources.OK, SharedResources.Critical)
        Me.m_pbDescription.BackgroundImage = cSystemUtils.IIF(bHasDescription, SharedResources.OK, SharedResources.Critical)
        Me.m_pbObjectives.BackgroundImage = cSystemUtils.IIF(bHasObjectives, SharedResources.OK, SharedResources.Critical)
        Me.m_pbAuthor.BackgroundImage = cSystemUtils.IIF(bHasAuthor And bHasContact, SharedResources.OK, SharedResources.Critical)
        Me.m_pbIsAuthor.BackgroundImage = cSystemUtils.IIF(bIsAuthor, SharedResources.OK, SharedResources.Critical)

        Me.m_cbEcosimUsed.Enabled = (core.nEcosimScenarios > 0)
        If (Not Me.m_cbEcosimUsed.Enabled) Then Me.m_cbFittedToTimeSeries.Checked = False
        Me.m_cbFittedToTimeSeries.Enabled = (core.nTimeSeriesDatasets > 0) And Me.m_cbEcosimUsed.Checked
        Me.m_cbEcospaceUsed.Enabled = (core.nEcospaceScenarios > 0)

        Me.m_tpModel.ImageIndex = cSystemUtils.IIF(bModelOK, 0, 2)

        ' -- Publication page --
        Dim bHasPublication As Boolean = (Me.m_tbxDOI.Text.Trim().Length > 5) Or (Me.m_tbxHyperlink.Text.Trim().Length > 12)
        Dim bHasReference As Boolean = (Me.m_tbxReference.Text.Trim().Length > 20)
        Dim bPubsOK As Boolean = bHasPublication Or bHasReference

        Me.m_pbPublication.BackgroundImage = cSystemUtils.IIF(bHasPublication, SharedResources.OK, SharedResources.Critical)
        Me.m_pbRef.BackgroundImage = cSystemUtils.IIF(bHasReference, SharedResources.OK, SharedResources.Critical)
        Me.m_llViewPublication.Enabled = bHasPublication

        Me.m_tpPublication.ImageIndex = cSystemUtils.IIF(bPubsOK, 0, 2)

        ' -- Classification page --
        Dim bHasArea As Boolean = (Not String.IsNullOrWhiteSpace(Me.SelectedText(Me.m_cmbCountry))) And (Not String.IsNullOrWhiteSpace(Me.SelectedText(Me.m_cmbRegion)))
        Dim bHasBoundingBox As Boolean = (CSng(Me.m_fpNorth.Value) <> CSng(Me.m_fpSouth.Value)) And (CSng(Me.m_fpWest.Value) <> CSng(Me.m_fpEast.Value))
        Dim bHasEcosystem As Boolean = (Not String.IsNullOrWhiteSpace(Me.SelectedText(Me.m_cmbEcoCat))) And (Not String.IsNullOrWhiteSpace(Me.SelectedText(Me.m_cmbEcoType)))
        Dim bHasEnv As Boolean = (CSng(Me.m_fpDmean.Value) > 0) And (CSng(Me.m_fpDmax.Value) > 0)

        Me.m_pbAreaName.BackgroundImage = cSystemUtils.IIF(bHasArea, SharedResources.OK, SharedResources.Critical)
        Me.m_pbBoundingBox.BackgroundImage = cSystemUtils.IIF(bHasBoundingBox, SharedResources.OK, SharedResources.Critical)
        Me.m_pbEcosystem.BackgroundImage = cSystemUtils.IIF(bHasEcosystem, SharedResources.OK, SharedResources.Critical)
        Me.m_pbEnvVars.BackgroundImage = cSystemUtils.IIF(bHasEnv, SharedResources.OK, SharedResources.Warning)

        If (bHasArea And bHasBoundingBox And bHasBoundingBox) Then
            Me.m_tpClassification.ImageIndex = cSystemUtils.IIF(bHasEnv, 0, 1)
        Else
            Me.m_tpClassification.ImageIndex = 2
        End If

        ' -- Agreement page --

        Me.m_btnSubmit.Enabled = bModelOK And bPubsOK

        Me.m_bInUpdate = False

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
        Dim strObjs As String = Me.m_tbxObjectives.Text
        Dim strAuthor As String = Me.m_tbxAuthor.Text
        Dim strContact As String = Me.m_tbxEmail.Text
        Dim strCountry As String = Me.SelectedText(Me.m_cmbCountry)
        Dim strRegion As String = Me.SelectedText(Me.m_cmbRegion)
        Dim strEcoType As String = Me.SelectedText(Me.m_cmbEcoType)
        Dim strEcoCat As String = Me.SelectedText(Me.m_cmbEcoCat)
        Dim strLME As String = Me.m_tbxLME.Text

        Dim strDOI As String = Me.m_tbxDOI.Text
        Dim strURI As String = Me.m_tbxHyperlink.Text
        Dim strRef As String = Me.m_tbxReference.Text
        Dim sNorth As Single = CSng(Me.m_fpNorth.Value)
        Dim sEast As Single = CSng(Me.m_fpEast.Value)
        Dim sWest As Single = CSng(Me.m_fpWest.Value)
        Dim sSouth As Single = CSng(Me.m_fpSouth.Value)

        Dim core As cCore = Me.m_uic.Core
        Dim model As cEwEModel = core.EwEModel
        Dim bSucces As Boolean = True

        Dim bChange As Boolean = (String.Compare(strName, model.Name) <> 0) Or _
                                 (String.Compare(strAuthor, model.Author) <> 0) Or _
                                 (String.Compare(strContact, model.Contact) <> 0) Or _
                                 (String.Compare(strDescr, model.Description) <> 0) Or _
                                 (String.Compare(strObjs, model.Objectives) <> 0) Or _
                                 (String.Compare(strDOI, model.PublicationDOI) <> 0) Or _
                                 (String.Compare(strURI, model.PublicationURI) <> 0) Or _
                                 (String.Compare(strRef, model.PublicationReference) <> 0) Or _
                                 (String.Compare(strCountry, model.Country) <> 0) Or _
                                 (String.Compare(strRegion, model.Region) <> 0) Or _
                                 (String.Compare(strEcoCat, model.EcosystemCategory) <> 0) Or _
                                 (String.Compare(strEcoType, model.EcosystemType) <> 0) Or _
                                 (String.Compare(strLME, model.LME) <> 0)

        bChange = bChange Or (model.North <> sNorth) Or _
                             (model.East <> sEast) Or _
                             (model.West <> sWest) Or _
                             (model.South <> sSouth)

        If bChange Then

            model.Name = strName
            model.Description = strDescr
            model.Objectives = strObjs
            model.Author = strAuthor
            model.Contact = strContact

            model.PublicationDOI = strDOI
            model.PublicationURI = strURI
            model.PublicationReference = strRef

            model.Country = strCountry
            model.Region = strRegion
            model.LME = strLME
            model.EcosystemCategory = strEcoCat
            model.EcosystemType = strEcoType

            model.North = sNorth
            model.East = sEast
            model.West = sWest
            model.South = sSouth

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

        ' Update values not stored in the model
        md.EcosimUsed = Me.m_cbEcosimUsed.Checked
        md.EcospaceUsed = Me.m_cbEcospaceUsed.Checked
        md.IsFittedToTimeSeries = Me.m_cbFittedToTimeSeries.Checked

        md.DepthMin = CSng(Me.m_fpDmin.Value)
        md.DepthMean = CSng(Me.m_fpDmean.Value)
        md.DepthMax = CSng(Me.m_fpDmax.Value)

        md.TempMin = CSng(Me.m_fpTmin.Value)
        md.TempMean = CSng(Me.m_fpTmean.Value)
        md.TempMax = CSng(Me.m_fpTmax.Value)

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

    Private Sub FillCombo(cmb As ComboBox, values As StringCollection)

        cmb.Items.Clear()

        If (values IsNot Nothing) Then
            For Each str As String In values
                cmb.Items.Add(str)
            Next
        End If

    End Sub

    Private Function SelectedText(cmb As ComboBox) As String
        If (cmb Is Nothing) Then Return ""
        If (cmb.SelectedItem Is Nothing) Then Return ""
        Return cmb.SelectedItem.ToString()
    End Function

#End Region ' Internals

End Class