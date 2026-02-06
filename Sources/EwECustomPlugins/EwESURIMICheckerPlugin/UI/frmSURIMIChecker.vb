Option Strict On

Imports System.Windows.Forms
Imports Eii.ControlledVocabularies.Common
Imports Eii.ControlledVocabularies.Core
Imports Eii.ControlledVocabularies.Vocabularies
Imports Eii.ControlledVocabularies.Vocabularies.LifeStage
Imports Eii.ControlledVocabularies.Vocabularies.Species
Imports EwECore
Imports ScientificInterfaceShared.Controls

Public Class frmSURIMIChecker

    Private m_dtSpecies As DataTable

    Public Sub New(uic As cUIContext)
        Me.InitializeComponent()
        MyBase.UIContext = uic
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Me.m_tscmbSpeciesVoc.Items.Add(New ASFISSpeciesCodeVocabulary(Nothing, Nothing, Nothing))
        Me.m_tscmbSpeciesVoc.SelectedIndex = 0

        Me.m_tscmbLifestageVoc.Items.Add(New SURIMILifestageVocabulary(Nothing, Nothing))
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

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub m_tsbnCalculateSpeciues_Click(sender As Object, e As EventArgs) Handles m_tsbnCalculateSpeciues.Click

        Dim vocSpecies As ControlledVocabularyBase = CType(Me.m_tscmbSpeciesVoc.SelectedItem, ControlledVocabularyBase)
        Dim nameVocSpecies As String = vocSpecies.VocabularyName
        Dim vocLStage As ControlledVocabularyBase = CType(Me.m_tscmbLifestageVoc.SelectedItem, ControlledVocabularyBase)
        Dim nameVocLStage As String = vocLStage.VocabularyName
        Dim nameSurimiSource As String = "SURIMI"
        Dim core As cCore = Me.UIContext.Core

        Dim reg As New KeyFieldDescriptorRegistry()
        reg.Register(New KeyFieldDescriptor(SpeciesFields.SpeciesCode, KeyDomain.Species, KeyPurpose.Species, FieldKind.Code, True, 10))
        reg.Register(New KeyFieldDescriptor(SpeciesFields.Lifestage, KeyDomain.Species, KeyPurpose.Lifestage, FieldKind.Label, False, 3))
        reg.Register(New KeyFieldDescriptor(SpeciesFields.Length, KeyDomain.Species, KeyPurpose.Length, FieldKind.Label, False, 3))
        reg.Register(New KeyFieldDescriptor(SpeciesFields.Age, KeyDomain.Species, KeyPurpose.Age, FieldKind.Label, False, 3))

        reg.Register(New KeyFieldDescriptor(FishingFields.GearCode, KeyDomain.FleetSegment, KeyPurpose.Gear, FieldKind.Code, True, 10))
        reg.Register(New KeyFieldDescriptor(FishingFields.Flag, KeyDomain.FleetSegment, KeyPurpose.Country, FieldKind.Code, False, 3))

        For Each dr As DataRow In Me.m_dtSpecies.Rows

            Dim grp As cEcoPathGroupInput = core.EcopathGroupInputs(CInt(dr("iGroup")))
            Dim tax As cTaxon = core.Taxon(CInt(dr("iTaxon")))

            Dim codeSpecies As String = ""
            Dim codeLifeStage As String = ""
            Dim mlk As MultiLevelKey = Nothing

            If (String.Compare(tax.Source, nameSurimiSource, True) = 0) Then
                Dim f As New MultiLevelKeyFactory()
                mlk = f.FromString(tax.SourceKey, KeyDomain.Species, Nothing)
                codeSpecies = mlk.GetField(SpeciesFields.SpeciesCode)
            End If

            If (String.IsNullOrWhiteSpace(codeSpecies)) Then
                codeSpecies = vocSpecies.FindCode(tax.Common, 80)
            End If

            If (String.IsNullOrWhiteSpace(codeSpecies)) Then
                Dim scname As String = tax.Genus.Trim() + " " + tax.Species.Trim()
                codeSpecies = vocSpecies.FindCode(scname, 80)
            End If

            If (tax.iStanza > 0) Then

            End If

            dr("Code") = IIf(String.IsNullOrWhiteSpace(codeSpecies), "?", codeSpecies)
            dr("Source") = nameSurimiSource
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
        dt.Columns.Add("Code", GetType(String))
        dt.Columns.Add("Source", GetType(String))

        Dim core As cCore = Me.UIContext.Core
        For i As Integer = 1 To core.nGroups
            Dim grp As cEcoPathGroupInput = core.EcopathGroupInputs(i)
            For j As Integer = 1 To grp.NTaxon
                Dim tax As cTaxon = core.Taxon(grp.iTaxon(j))
                Dim dr = dt.NewRow()
                dr("iGroup") = i
                dr("iTaxon") = grp.iTaxon(j)
                dr("Group") = grp.Name
                dr("Common") = tax.Common
                dr("Genus") = tax.Genus
                dr("Species") = tax.Species
                dr("Code") = tax.SourceKey
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
