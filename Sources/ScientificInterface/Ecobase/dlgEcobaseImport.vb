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
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.SystemUtilities
Imports System.Text

#End Region ' Imports

Public Class dlgEcobaseImport

#Region " Private vars "

    Private m_ecobase As cEcoBaseWDSL = Nothing
    Private m_models As New List(Of cModelData)
    Private m_model As cModelData = Nothing

    Private m_strAgreement As String = ""
    Private m_img As Image = Nothing

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Tags in <see cref="m_tsddValue">filter dropdown items</see> must correspond to the values in this enum.
    ''' </remarks>
    Private Enum eFilterTypes As Integer
        None = 0
        Author = 1
        Country = 2
        EcosystemType = 3
        Depth = 4
        Temperature = 5
    End Enum

    Private m_filter As eFilterTypes = eFilterTypes.None

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

        Dim il As New ImageList()
        il.Images.Add(SharedResources.OK)
        il.Images.Add(SharedResources.Warning)
        il.Images.Add(SharedResources.Critical)
        Me.m_tcContent.ImageList = il

        Me.m_lblRefValue.AutoSize = False
        Me.m_lblRefValue.MaximumSize = New Size(100, 0)
        Me.m_lblRefValue.AutoSize = True

        Me.m_tsddValue.Image = SharedResources.FilterHS

        Me.m_ecobase = New cEcoBaseWDSL()
        Me.m_wrkGetAgreement.RunWorkerAsync(Nothing)
        Me.m_wrkGetModels.RunWorkerAsync(Nothing)

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)
        e.Cancel = (Me.DialogResult = Windows.Forms.DialogResult.OK) And Not Me.CanDownload
        MyBase.OnFormClosing(e)
    End Sub

    Protected Overrides Sub OnSizeChanged(e As System.EventArgs)
        MyBase.OnSizeChanged(e)
    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)

        Me.m_ecobase.CancelAsync(Nothing)
        Me.m_ecobase.Dispose()
        Me.m_ecobase = Nothing

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        ' ToDo: Globalize this method

        Me.m_tsddValue.Text = Me.FilterItemText(Me.m_filter)
        Me.m_tstbSearch.Enabled = (Me.m_filter <> eFilterTypes.None)

        Me.m_rtfAgreement.Text = Me.m_strAgreement
        Me.m_btnOK.Enabled = Me.CanDownload

        ' Populate model controls
        If (Me.m_model IsNot Nothing) Then
            Dim sb As New StringBuilder()

            Me.m_lblModelNameValue.Text = cStringUtils.ToSentenceCase(Me.m_model.Name)
            Me.m_lblAreaValue.Text = cStringUtils.FormatNumber(Me.m_model.Area)
            Me.m_lblAuthorValue.Text = cStringUtils.ToTitleCase(Me.m_model.Author)
            Me.m_lblCountryValue.Text = cStringUtils.ToTitleCase(Me.m_model.Country)
            Me.m_lblEcosystemTypeValue.Text = cStringUtils.ToSentenceCase(Me.m_model.EcosystemType)
            Me.m_lblNoGroupsValue.Text = "?"
            Me.m_lblNoFleetsValue.Text = "?"
            Me.m_lblPeriodValue.Text = Me.PeriodLabel()
            Me.m_lblEcosimUsedValue.Text = cSystemUtils.IIF(Me.m_model.EcosimUsed, SharedResources.BUTTON_YES, SharedResources.BUTTON_NO)
            Me.m_lblFittedValue.Text = cSystemUtils.IIF(Me.m_model.IsFittedToTimeSeries, SharedResources.BUTTON_YES, SharedResources.BUTTON_NO)
            Me.m_lblEcospaceUsedValue.Text = cSystemUtils.IIF(Me.m_model.EcospaceUsed, SharedResources.BUTTON_YES, SharedResources.BUTTON_NO)
            Me.m_lblRefValue.Text = Me.ReferenceLabel()

            If (Me.m_img Is Nothing) Then
                Me.m_pbImage.BackgroundImageLayout = ImageLayout.Center
                Me.m_pbImage.BackgroundImage = SharedResources.ani_loader
            Else
                Me.m_pbImage.BackgroundImage = Me.m_img
                Me.m_pbImage.BackgroundImageLayout = ImageLayout.Zoom
            End If
            Me.m_lblLonVal.Text = Me.LonLabel()
            Me.m_lblLatVal.Text = Me.LatLabel()
            Me.m_lblDepthRangeVal.Text = cStringUtils.FormatNumber(Me.m_model.DepthMin) & " - " & cStringUtils.FormatNumber(Me.m_model.DepthMax)
            Me.m_lblDepthMeanVal.Text = cStringUtils.FormatNumber(Me.m_model.DepthMean)
            Me.m_lblTempRangeVal.Text = cStringUtils.FormatNumber(Me.m_model.TempMin) & " - " & cStringUtils.FormatNumber(Me.m_model.TempMax)
            Me.m_lblTempMeanVal.Text = cStringUtils.FormatNumber(Me.m_model.TempMean)

        Else
            Me.m_lblModelNameValue.Text = ""
            Me.m_lblAreaValue.Text = ""
            Me.m_lblAuthorValue.Text = ""
            Me.m_lblCountryValue.Text = ""
            Me.m_lblEcosystemTypeValue.Text = ""
            Me.m_lblNoGroupsValue.Text = ""
            Me.m_lblNoFleetsValue.Text = ""
            Me.m_lblPeriodValue.Text = ""
            Me.m_lblEcosimUsedValue.Text = ""
            Me.m_lblFittedValue.Text = ""
            Me.m_lblEcospaceUsedValue.Text = ""
            Me.m_lblRefValue.Text = ""
            Me.m_pbImage.Image = Nothing
            Me.m_lblLonVal.Text = ""
            Me.m_lblLatVal.Text = ""
            Me.m_lblDepthRangeVal.Text = ""
            Me.m_lblDepthMeanVal.Text = ""
            Me.m_lblTempRangeVal.Text = ""
            Me.m_lblTempMeanVal.Text = ""
        End If

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
                bCanDownload = (Me.m_model.AllowDissemination And Me.m_cbEcoBaseAgreement.Checked)
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
        e.Value = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, model.Name, model.FirstYear)

    End Sub

    Private Sub OnModelSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_lbxModels.SelectedIndexChanged

        Try
            Me.m_model = Nothing
            If (Me.m_lbxModels.SelectedIndex > -1) Then
                Me.m_model = DirectCast(Me.m_lbxModels.SelectedItem, cModelData)
                If (Me.m_wrkGetImage.IsBusy) Then
                    Me.m_wrkGetImage.CancelAsync()
                End If
                Me.m_wrkGetImage.RunWorkerAsync(Nothing)
            End If

        Catch ex As Exception

        End Try

        Me.UpdateControls()

    End Sub

    Private Sub OnAcceptAgreement(sender As System.Object, e As System.EventArgs) _
        Handles m_cbEcoBaseAgreement.CheckedChanged

        Try
            Me.UpdateControls()
        Catch ex As Exception
            ' NOP
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

#Region " Filter events "

    Private Sub OnSearchTextChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tstbSearch.TextChanged

        Me.UpdateModelList()

    End Sub

    Private Sub OnFilterSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_tsmiNone.Click, m_tsmiAuthor.Click, m_tsmiCountry.Click, m_tsmiEcoType.Click, m_tsmiDepth.Click, m_tsmiTemperature.Click

        Dim tsmi As ToolStripItem = DirectCast(sender, ToolStripItem)
        If (tsmi.Tag IsNot Nothing) Then
            Dim iVal As Integer
            If (Integer.TryParse(CStr(tsmi.Tag), iVal)) Then
                Try
                    Me.m_filter = DirectCast(iVal, eFilterTypes)
                Catch ex As Exception
                    Me.m_filter = eFilterTypes.None
                End Try
            End If
        End If
        Me.UpdateControls()

    End Sub

    Private Function FilterItemText(filter As Integer) As String

        Dim strFilterText As String = SharedResources.GENERIC_VALUE_NONE

        For Each tsmi As ToolStripItem In Me.m_tsddValue.DropDownItems
            Dim iVal As Integer = cCore.NULL_VALUE
            If tsmi.Tag IsNot Nothing Then
                If Integer.TryParse(CStr(tsmi.Tag), iVal) Then
                    If CInt(filter) = iVal Then
                        strFilterText = tsmi.Text
                    End If
                End If
            End If
        Next
        Return strFilterText

    End Function

#End Region ' Filter events

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

        Me.UpdateEcoBaseLists()
        Me.UpdateModelList()
        Me.UpdateControls()

    End Sub


    Private Sub OnGetAgreement(sender As Object, e As System.ComponentModel.DoWorkEventArgs) _
        Handles m_wrkGetAgreement.DoWork

        Try
            Dim wdsl As New cEcoBaseWDSL()
            Dim strAgreement As String = wdsl.getModel("agreement", -1)
            Dim data As cEcobaseDataAccessAgreement = cEcobaseDataAccessAgreement.FromXML(strAgreement)

            Me.m_strAgreement = data.Agreement

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnGetAgreementComplete(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) _
        Handles m_wrkGetAgreement.RunWorkerCompleted

        Try
            Me.UpdateControls()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnGetImage(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) _
    Handles m_wrkGetImage.DoWork

        Try
            If (Me.m_model Is Nothing) Then
                Me.m_img = Nothing
                Return
            End If

            Dim MyWebClient As New System.Net.WebClient()
            Dim data() As Byte = MyWebClient.DownloadData("http://sirs.agrocampus-ouest.fr/EcoBase/php/mapserver.php?model=" & m_model.EcobaseCode)
            Dim strm As New IO.MemoryStream(data)
            Me.m_img = New System.Drawing.Bitmap(strm)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnGetImageCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles m_wrkGetImage.RunWorkerCompleted

        Try
            Me.UpdateControls()
        Catch ex As Exception

        End Try

    End Sub

#End Region ' Background workers

#Region " Internals "

    Private Sub UpdateModelList()

        Dim strFilter As String = Me.m_tstbSearch.Text
        Dim bUseModel As Boolean = True
        Dim bKeepSelection As Boolean = False

        Me.m_lbxModels.SuspendLayout()
        Me.m_lbxModels.Items.Clear()

        For Each model As cModelData In Me.m_models
            If (model.AllowDissemination) Then
                bUseModel = True

                ' Filters
                If (Not String.IsNullOrWhiteSpace(strFilter)) Then
                    Select Case Me.m_filter
                        Case eFilterTypes.None
                        Case eFilterTypes.Author : bUseModel = Me.StartsWith(strFilter, model.Author)
                        Case eFilterTypes.Country : bUseModel = Me.StartsWith(strFilter, model.Country)
                        Case eFilterTypes.EcosystemType : bUseModel = Me.StartsWith(strFilter, model.EcosystemType)
                        Case eFilterTypes.Depth : bUseModel = Me.IsInRange(strFilter, model.DepthMin, model.DepthMax)
                        Case eFilterTypes.Temperature : bUseModel = Me.IsInRange(strFilter, model.TempMean, model.TempMax)
                    End Select
                End If

                If (bUseModel) Then
                    Me.m_lbxModels.Items.Add(model)
                    bKeepSelection = bKeepSelection Or (Object.ReferenceEquals(model, Me.m_model))
                End If
            End If
        Next

        Me.m_lbxModels.ResumeLayout()

        If (bKeepSelection) Then
            Me.m_lbxModels.SelectedItem = Me.m_model
        Else
            If (Me.m_lbxModels.Items.Count > 0) Then
                Me.m_lbxModels.SelectedIndex = 0
            Else
                Me.m_lbxModels.SelectedIndex = -1
                Me.m_model = Nothing
            End If
        End If
        Me.UpdateControls()

    End Sub

    Private Function StartsWith(strFilter As String, strValue As String) As Boolean
        If (String.IsNullOrWhiteSpace(strValue)) Then Return False
        Return strValue.StartsWith(strFilter, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function ContainsSubItem(strFilter As String, strLMEs As String, Optional cSplit As Char = ","c) As Boolean

        ' Can be greatly refined later on

        If (String.IsNullOrWhiteSpace(strLMEs)) Then Return False
        Dim bits As String() = strLMEs.Split(","c)
        Return bits.Contains(strFilter.Trim())

    End Function

    Private Function IsInRange(strFilter As String, sMin As Single, sMax As Single) As Boolean

        Dim sVal As Single = 0
        If Not Single.TryParse(strFilter.Trim, sVal) Then Return False
        Return (sMin <= sVal) And (sVal <= sMax)

    End Function

    Private Sub UpdateEcoBaseLists()

        Dim lCountry As New List(Of String)
        Dim lEcoTyp As New List(Of String)

        For Each model As cModelData In Me.m_models
            If Not String.IsNullOrWhiteSpace(model.Country) Then lCountry.Add(model.Country)
            If Not String.IsNullOrWhiteSpace(model.EcosystemType) Then lEcoTyp.Add(model.EcosystemType)
        Next

        If (My.Settings.CountryNames IsNot Nothing) Then
            For Each str As String In My.Settings.CountryNames : lCountry.Add(str) : Next
        End If

        If (My.Settings.EcosystemTypes IsNot Nothing) Then
            For Each str As String In My.Settings.EcosystemTypes : lEcoTyp.Add(str) : Next
        End If

        Dim sgu As New cStyleGuideUpdater(Me.UIContext)
        sgu.Save()

        My.Settings.CountryNames.Clear()
        My.Settings.CountryNames.AddRange(lCountry.Distinct(StringComparer.CurrentCultureIgnoreCase).ToArray())

        My.Settings.EcosystemTypes.Clear()
        My.Settings.EcosystemTypes.AddRange(lEcoTyp.Distinct(StringComparer.CurrentCultureIgnoreCase).ToArray())

        sgu.Load()

        Me.StyleGuide.EcoBaseFieldsChanged()

    End Sub

    Private Function PeriodLabel() As String

        If (Me.m_model Is Nothing) Then Return ""
        If (Me.m_model.FirstYear = 0) Then Return "?"
        If (Me.m_model.NumYears <= 1) Then Return CStr(Me.m_model.FirstYear)
        Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_SPLIT, _
                                     Me.m_model.FirstYear, _
                                     Me.m_model.FirstYear + Math.Max(1, Me.m_model.NumYears) - 1)

    End Function

    Private Function ReferenceLabel() As String

        If (Me.m_model Is Nothing) Then Return ""

        Dim sb As New StringBuilder()
        If (Not String.IsNullOrWhiteSpace(Me.m_model.DOI)) Then
            sb.AppendLine(cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, "DOI", Me.m_model.DOI))
        End If
        If (Not String.IsNullOrWhiteSpace(Me.m_model.URI)) Then
            sb.AppendLine(Me.m_model.URI)
        End If
        If (Not String.IsNullOrWhiteSpace(Me.m_model.Reference)) Then
            sb.AppendLine(Me.m_model.Reference)
        End If
        If (sb.Length = 0) Then sb.Append("?")
        Return sb.ToString()

    End Function

    Private Function LatLabel() As String

        ' ToDo: globalize this method
        If (Me.m_model Is Nothing) Then Return ""
        If (Me.m_model.North = Me.m_model.South) Then Return "?"
        Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, _
                                     cStringUtils.Localize("{0} dd N", cStringUtils.FormatNumber(Me.m_model.North)), _
                                     cStringUtils.Localize("{0} dd S", cStringUtils.FormatNumber(Me.m_model.South)))

    End Function

    Private Function LonLabel() As String

        ' ToDo: globalize this method
        If (Me.m_model Is Nothing) Then Return ""
        If (Me.m_model.North = Me.m_model.South) Then Return "?"
        Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, _
                                     cStringUtils.Localize("{0} dd W", cStringUtils.FormatNumber(Me.m_model.West)), _
                                     cStringUtils.Localize("{0} dd E", cStringUtils.FormatNumber(Me.m_model.East)))

    End Function

#End Region ' Internals

End Class