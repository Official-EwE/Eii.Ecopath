' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict On
Imports System.Windows.Forms
Imports Eii.ControlledVocabularies.Common
Imports Eii.ControlledVocabularies.Core
Imports Eii.ControlledVocabularies.Descriptors
Imports Eii.ControlledVocabularies.Vocabularies
Imports Eii.ControlledVocabularies.Vocabularies.LifeStage
Imports Eii.ControlledVocabularies.Vocabularies.Species
Imports EwECore
Imports ScientificInterfaceShared.Controls

Public Class frmSURIMIChecker

    Const Source As String = "SURIMI"

    Private m_dtSpecies As DataTable
    Private m_regFields As New KeyFieldDescriptorRegistry()
    Private m_asfis As ASFISSpeciesCodeVocabulary = Nothing
    Private m_surimi As SURIMILifestageVocabulary = Nothing

    Public Sub New(uic As cUIContext, provider As IServiceProvider)

        Me.InitializeComponent()
        MyBase.UIContext = uic

        Me.DoubleBuffered = True

        Me.m_asfis = TryCast(provider.GetService(GetType(ASFISSpeciesCodeVocabulary)), ASFISSpeciesCodeVocabulary)
        Me.m_surimi = TryCast(provider.GetService(GetType(SURIMILifestageVocabulary)), SURIMILifestageVocabulary)

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        m_regFields.Register(New KeyFieldDescriptor(SpeciesFields.SpeciesCode, KeyDomain.Species, KeyPurpose.Species, FieldKind.Code, True, 10))
        m_regFields.Register(New KeyFieldDescriptor(SpeciesFields.Lifestage, KeyDomain.Species, KeyPurpose.Lifestage, FieldKind.Label, False, 3))
        m_regFields.Register(New KeyFieldDescriptor(SpeciesFields.Length, KeyDomain.Species, KeyPurpose.Length, FieldKind.Label, False, 3))
        m_regFields.Register(New KeyFieldDescriptor(SpeciesFields.Age, KeyDomain.Species, KeyPurpose.Age, FieldKind.Label, False, 3))

        m_regFields.Register(New KeyFieldDescriptor(FishingFields.GearCode, KeyDomain.FleetSegment, KeyPurpose.Gear, FieldKind.Code, True, 10))
        m_regFields.Register(New KeyFieldDescriptor(FishingFields.Flag, KeyDomain.FleetSegment, KeyPurpose.Country, FieldKind.Code, False, 3))

        Console.WriteLine("Loading ASFIS voc: " & m_asfis.Load())
        Console.WriteLine("Loading SURIMI voc: " & m_surimi.Load())

        Me.m_tscmbSpeciesVoc.Items.Add(m_asfis)
        Me.m_tscmbSpeciesVoc.SelectedIndex = 0

        Me.m_tscmbLifestageVoc.Items.Add(m_regFields)
        Me.m_tscmbLifestageVoc.SelectedIndex = 0

        Me.m_tscmbSpeciesVoc.ComboBox.FormattingEnabled = True
        AddHandler Me.m_tscmbSpeciesVoc.ComboBox.Format, AddressOf OnFormatComboItem
        Me.m_tscmbLifestageVoc.ComboBox.FormattingEnabled = True
        AddHandler Me.m_tscmbLifestageVoc.ComboBox.Format, AddressOf OnFormatComboItem

        Me.FillSpeciesGrid()

    End Sub


    Protected Overrides Sub OnClosed(e As EventArgs)

        RemoveHandler Me.m_tscmbSpeciesVoc.ComboBox.Format, AddressOf OnFormatComboItem
        RemoveHandler Me.m_tscmbLifestageVoc.ComboBox.Format, AddressOf OnFormatComboItem

        MyBase.OnClosed(e)

    End Sub

    Private Sub OnFormatComboItem(sender As Object, e As ListControlConvertEventArgs)
        If (TypeOf e.ListItem Is ControlledVocabularyBase) Then
            e.Value = DirectCast(e.ListItem, ControlledVocabularyBase).VocabularyName
        End If
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnOK.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub m_tsbnCalculateSpeciues_Click(sender As Object, e As EventArgs) Handles m_tsbnCalculateSpeciues.Click

        Dim nameVocSpecies As String = m_asfis.VocabularyName
        Dim nameVocLStage As String = m_surimi.VocabularyName
        Dim core As cCore = Me.UIContext.Core

        For Each dr As DataRow In Me.m_dtSpecies.Rows

            Try

                Dim grp As cEcoPathGroupInput = core.EcopathGroupInputs(CInt(dr("iGroup")))
                Dim tax As cTaxon = core.Taxon(CInt(dr("iTaxon")))

                Dim codeSpecies As String = ""
                Dim codeLifeStage As String = ""
                Dim mlk As MultiLevelKey = Nothing
                Dim bSkip As Boolean = False

                If (String.Compare(tax.Source, Source, True) = 0) Then
                    Dim f As New MultiLevelKeyFactory()
                    mlk = f.FromString(tax.SourceKey, KeyDomain.Species, m_regFields)
                    codeSpecies = mlk.GetField(SpeciesFields.SpeciesCode).Value
                    codeLifeStage = mlk.GetField(SpeciesFields.Lifestage).Value
                End If

                If (String.IsNullOrWhiteSpace(codeSpecies)) Then
                    Dim scname As String = (tax.Genus.Trim() + " " + tax.Species.Trim()).ToLowerInvariant()
                    bSkip = String.IsNullOrWhiteSpace(tax.Species) Or String.IsNullOrWhiteSpace(scname)
                    If (Not bSkip) Then bSkip = scname.EndsWith("p.")
                    codeSpecies = m_asfis.FindCode(scname, 80)
                End If

                If (String.IsNullOrWhiteSpace(codeSpecies)) Then
                    codeSpecies = m_asfis.FindCode(tax.Common, 80)
                End If

                If (tax.iStanza > 0 And String.IsNullOrWhiteSpace(codeLifeStage)) Then
                    codeLifeStage = m_surimi.FindCode(grp.Name, 80)
                End If

                mlk = New MultiLevelKey(KeyDomain.Species, False)
                If (Not String.IsNullOrWhiteSpace(SpeciesCode)) Then
                    mlk.SetField(SpeciesFields.SpeciesCode, nameVocSpecies & ":" & codeSpecies, m_regFields)
                End If
                If (Not String.IsNullOrWhiteSpace(codeLifeStage)) Then
                    mlk.SetField(SpeciesFields.Lifestage, nameVocLStage & ":" & codeLifeStage, m_regFields)
                End If

                dr("SpeciesCode") = codeSpecies
                dr("LifeStageCode") = codeLifeStage

                dr("MultiLevelKey") = mlk.ToString()
                dr("Source") = Source
            Catch ex As Exception

            End Try
        Next

    End Sub

    Private Sub FillSpeciesGrid()

        Dim dt As New DataTable()
        dt.Columns.Add("iGroup", GetType(Integer)).ColumnMapping = MappingType.Hidden
        dt.Columns.Add("iTaxon", GetType(Integer)).ColumnMapping = MappingType.Hidden
        dt.Columns.Add("Group", GetType(String)).ReadOnly = True
        dt.Columns.Add("Common", GetType(String)).ReadOnly = True
        dt.Columns.Add("Genus", GetType(String)).ReadOnly = True
        dt.Columns.Add("Species", GetType(String)).ReadOnly = True
        dt.Columns.Add("SpeciesCode", GetType(String))
        dt.Columns.Add("LifeStageCode", GetType(String))
        dt.Columns.Add("MultiLevelKey", GetType(String))
        dt.Columns.Add("Source", GetType(String))

        Dim core As cCore = Me.UIContext.Core
        For i As Integer = 1 To core.nGroups
            Dim grp As cEcoPathGroupInput = core.EcopathGroupInputs(i)
            For j As Integer = 1 To grp.NTaxon
                Dim tax As cTaxon = core.Taxon(grp.iTaxon(j))
                Dim SpeciesCode As String = ""
                Dim LifeStageCode As String = ""
                If (Not String.IsNullOrWhiteSpace(tax.SourceKey) And tax.Source = Source) Then
                    Dim f As New MultiLevelKeyFactory()
                    Dim mlk = f.FromString(tax.SourceKey, KeyDomain.Species, m_regFields)
                    SpeciesCode = mlk.GetField(SpeciesFields.SpeciesCode).Value
                    SpeciesCode = mlk.GetField(SpeciesFields.SpeciesCode).Value
                End If
                Dim dr = dt.NewRow()
                dr("iGroup") = i
                dr("iTaxon") = grp.iTaxon(j)
                dr("Group") = grp.Name
                dr("Common") = tax.Common
                dr("Genus") = tax.Genus
                dr("Species") = tax.Species
                dr("SpeciesCode") = SpeciesCode
                dr("LifeStageCode") = LifeStageCode
                dr("MultiLevelKey") = tax.SourceKey
                dr("Source") = tax.Source
                dt.Rows.Add(dr)
            Next
        Next

        Me.m_dgvSpecies.Columns.Clear()
        Dim src As New BindingSource()

        m_dtSpecies = dt
        src.DataSource = m_dtSpecies
        Me.m_dgvSpecies.DataSource = src

    End Sub

End Class
