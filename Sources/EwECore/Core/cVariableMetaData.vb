' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

#Const DUMP_TO_FILE = 0


Imports EwECore.Style
Imports EwECore.ValueWrapper
#If DUMP_TO_FILE Then
Imports System.IO
#End If

''' ---------------------------------------------------------------------------
''' <summary>
''' Meta data for a variable, describing its value range and default value.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cVariableMetaData

#Region "Private vars"

    Private Shared s_inst As cVariableMetaData = Nothing

    ''' <summary>Shared registry of metadata definitions</summary>
    Private Shared s_metadata As Dictionary(Of eVarNameFlags, cVariableMetaData)

    ' -- Variables for numeric values --
    Private m_vartype As eValueTypes
    Private m_units As String = ""

#End Region 'Private vars    

#Region "Singleton"

    Private Shared Function GetInstance() As cVariableMetaData
        If (s_inst Is Nothing) Then
            s_inst = New cVariableMetaData()
            s_inst.Init()
        End If
        Return s_inst
    End Function

    Private Sub Init()

        ' Ok. Stop. Halt. Wait. First a bit about Units

        ' cUnit accepts whatever field you provide with the metadata, and placeholders are used to find localized and/or
        ' customized units. Placeholders are [encapsulated]. For units that are hard-coded and unlocalizable
        ' (such as the vulnerability index below), encapsulation brackets are not needed.

        ' If cUnit cannot find a replacement for a unit placeholder, the placeholder text is shown in the UI.
        ' This could be the case for units such as [cm] or [kg]. Maybe they get localized one day, but hey,
        ' at least the UI will display some sort of a unit.

        ' Now carry on.

        s_metadata = New Dictionary(Of eVarNameFlags, cVariableMetaData)

        Dim gt As cOperatorBase = cOperatorManager.getOperator(eOperators.GreaterThan)
        Dim ge As cOperatorBase = cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo)
        Dim lt As cOperatorBase = cOperatorManager.getOperator(eOperators.LessThan)
        Dim le As cOperatorBase = cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)

        ' -- Generics --
        Me.Metadata(eVarNameFlags.Name) = New cVariableMetaData(50)
        Me.Metadata(eVarNameFlags.Index) = New cVariableMetaData(1, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.DBID) = New cVariableMetaData(1, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.Description) = New cVariableMetaData(253)
        Me.Metadata(eVarNameFlags.Author) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.Contact) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.Color) = New cVariableMetaData(-4294967295, 4294967295, ge, le)
        Me.Metadata(eVarNameFlags.LastSaved) = New cVariableMetaData(0, Double.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.LastUpdated) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.North) = New cVariableMetaData(Single.MinValue, Single.MaxValue, ge, le, 0, cUnits.Mapping)
        Me.Metadata(eVarNameFlags.South) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0, cUnits.Mapping)
        Me.Metadata(eVarNameFlags.East) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0, cUnits.Mapping)
        Me.Metadata(eVarNameFlags.West) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0, cUnits.Mapping)

        ' -- Model --
        Me.Metadata(eVarNameFlags.Area) = New cVariableMetaData(0, Single.MaxValue, ge, lt, , cUnits.Area)
        Me.Metadata(eVarNameFlags.NumDigits) = New cVariableMetaData(0, 10, ge, le)
        Me.Metadata(eVarNameFlags.GroupDigits) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EcopathFirstYear) = New cVariableMetaData(0, 10000, ge, lt, 1950)
        Me.Metadata(eVarNameFlags.EcopathNumYears) = New cVariableMetaData(1, 10000, ge, lt, 1)
        Me.Metadata(eVarNameFlags.UnitTime) = New cVariableMetaData(0, [Enum].GetValues(GetType(eUnitTimeType)).Length - 1, ge, le)
        Me.Metadata(eVarNameFlags.UnitTimeCustomText) = New cVariableMetaData(20)
        Me.Metadata(eVarNameFlags.UnitCurrency) = New cVariableMetaData(0, [Enum].GetValues(GetType(eUnitCurrencyType)).Length - 1, ge, le)
        Me.Metadata(eVarNameFlags.UnitCurrencyCustomText) = New cVariableMetaData(20)
        Me.Metadata(eVarNameFlags.UnitMonetary) = New cVariableMetaData(10)
        Me.Metadata(eVarNameFlags.UnitArea) = New cVariableMetaData(0, [Enum].GetValues(GetType(eUnitAreaType)).Length - 1, ge, le)
        Me.Metadata(eVarNameFlags.UnitAreaCustomText) = New cVariableMetaData(20)
        Me.Metadata(eVarNameFlags.UnitMapRef) = New cVariableMetaData(0, [Enum].GetValues(GetType(eUnitMapRefType)).Length - 1, ge, le)
        Me.Metadata(eVarNameFlags.Country) = New cVariableMetaData(63)
        Me.Metadata(eVarNameFlags.EcosystemType) = New cVariableMetaData(254)
        Me.Metadata(eVarNameFlags.CodeEcobase) = New cVariableMetaData(49)
        Me.Metadata(eVarNameFlags.PublicationDOI) = New cVariableMetaData(2000)
        Me.Metadata(eVarNameFlags.PublicationURI) = New cVariableMetaData(2000)
        Me.Metadata(eVarNameFlags.PublicationReference) = New cVariableMetaData(2000)
        Me.Metadata(eVarNameFlags.IsEcospaceModelCoupled) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.DiversityIndex) = New cVariableMetaData(0, [Enum].GetValues(GetType(eDiversityIndexType)).Length - 1, ge, le)

        ' -- Ecopath groups --
        ' in 
        Me.Metadata(eVarNameFlags.Biomass) = New cVariableMetaData(0, Single.MaxValue, gt, lt, , cUnits.Currency)
        Me.Metadata(eVarNameFlags.HabitatArea) = New cVariableMetaData(0, 1, gt, le, cCore.NULL_VALUE, cUnits.Proportion)
        'jb allow BA to have a greater range then 0-1
        'Me.Metadata(eVarNameFlags.BioAccumInput) = New cVariableMetaData(0, 1, gt, le, cCore.NULL_VALUE, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.BioAccumInput) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, le, cCore.NULL_VALUE, cUnits.Currency)
        Me.Metadata(eVarNameFlags.BioAccumRate) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt,, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.BiomassAreaInput) = New cVariableMetaData(0, Single.MaxValue, gt, lt, cCore.NULL_VALUE, cUnits.Currency) ' Set to null when cleared
        Me.Metadata(eVarNameFlags.PBInput) = New cVariableMetaData(0, Single.MaxValue, gt, lt, cCore.NULL_VALUE, cUnits.OverTime) ' Set to null when cleared
        Me.Metadata(eVarNameFlags.QBInput) = New cVariableMetaData(0, Single.MaxValue, gt, lt, cCore.NULL_VALUE, cUnits.OverTime) ' Set to null when cleared
        Me.Metadata(eVarNameFlags.EEInput) = New cVariableMetaData(0, 1, gt, le, cCore.NULL_VALUE) ' Set to null when cleared
        Me.Metadata(eVarNameFlags.GEInput) = New cVariableMetaData(0, 1, gt, lt, cCore.NULL_VALUE) ' Set to null when cleared
        Me.Metadata(eVarNameFlags.Emig) = New cVariableMetaData(0, Single.MaxValue, ge, le,, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EmigRate) = New cVariableMetaData(0, Single.MaxValue, ge, le,, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.Immig) = New cVariableMetaData(0, Single.MaxValue, ge, lt,, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.GS) = New cVariableMetaData(0, 1, ge, le)
        Me.Metadata(eVarNameFlags.Z) = New cVariableMetaData(0, Single.MaxValue, gt, lt, cCore.NULL_VALUE, cUnits.OverTime) ' Set to null when cleared
        Me.Metadata(eVarNameFlags.OtherMortInput) = New cVariableMetaData(0, 1, gt, le, cCore.NULL_VALUE) ' Set to null when cleared 
        Me.Metadata(eVarNameFlags.NonMarketValue) = New cVariableMetaData(0, Single.MaxValue, ge, lt,, cUnits.MonetaryOverKg)
        Me.Metadata(eVarNameFlags.DetImp) = New cVariableMetaData(0, Single.MaxValue, ge, le, cCore.NULL_VALUE, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.DetritusFate) = New cVariableMetaData(0, 1, ge, le, 0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.DietComp) = New cVariableMetaData(0, 1, ge, le, 0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.ImpDiet) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.VBK) = New cVariableMetaData(0, Single.MaxValue, ge, le)
        Me.Metadata(eVarNameFlags.TCatchInput) = New cVariableMetaData(0, Single.MaxValue, gt, le, cCore.NULL_VALUE, cUnits.Time)
        Me.Metadata(eVarNameFlags.AinLWInput) = New cVariableMetaData(0, Single.MaxValue, gt, le, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.BinLWInput) = New cVariableMetaData(0, Single.MaxValue, gt, le, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.LooInput) = New cVariableMetaData(0, Single.MaxValue, gt, le, cCore.NULL_VALUE, "[cm]")
        Me.Metadata(eVarNameFlags.WinfInput) = New cVariableMetaData(0, Single.MaxValue, gt, le, cCore.NULL_VALUE, "[g]")
        Me.Metadata(eVarNameFlags.t0Input) = New cVariableMetaData(-1, 0, gt, le, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.TmaxInput) = New cVariableMetaData(0, Single.MaxValue, gt, le, cCore.NULL_VALUE, cUnits.Time)
        Me.Metadata(eVarNameFlags.IsFished) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.PP) = New cVariableMetaData(0, 2, ge, le)
        Me.Metadata(eVarNameFlags.Energy) = New cVariableMetaData(0.0001, Single.MaxValue, ge, lt, 1)
        ' out
        Me.Metadata(eVarNameFlags.BiomassAreaOutput) = cVariableMetaData.Get(eVarNameFlags.BiomassAreaInput)
        Me.Metadata(eVarNameFlags.PBOutput) = cVariableMetaData.Get(eVarNameFlags.PBInput)
        Me.Metadata(eVarNameFlags.QBOutput) = cVariableMetaData.Get(eVarNameFlags.QBInput)
        Me.Metadata(eVarNameFlags.EEOutput) = cVariableMetaData.Get(eVarNameFlags.EEInput)
        Me.Metadata(eVarNameFlags.GEOutput) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.BioAccumOutput) = cVariableMetaData.Get(eVarNameFlags.BioAccumInput)
        Me.Metadata(eVarNameFlags.Respiration) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.Assimilation) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.ProdResp) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.RespAssim) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.RespBiom) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.BioAccumRatePerYear) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.TTLX) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.ImportedConsumption) = [Default](eValueTypes.Sng, cUnits.Currency)
        Me.Metadata(eVarNameFlags.MortCoPB) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.MortCoFishRate) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.MortCoPredMort) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.MortCoBioAcumRate) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.MortCoNetMig) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.MortCoOtherMort) = [Default](eValueTypes.Sng, cUnits.OverTime)
        Me.Metadata(eVarNameFlags.NetMigration) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.FlowToDet) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.NetEfficiency) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.OmnivoryIndex) = [Default](eValueTypes.Sng)

        Me.Metadata(eVarNameFlags.BiomassAvgSzWt) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.BiomassSzWt) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.TCatchOutput) = cVariableMetaData.Get(eVarNameFlags.TCatchInput)
        Me.Metadata(eVarNameFlags.AinLWOutput) = cVariableMetaData.Get(eVarNameFlags.AinLWInput)
        Me.Metadata(eVarNameFlags.BinLWOutput) = cVariableMetaData.Get(eVarNameFlags.BinLWInput)
        Me.Metadata(eVarNameFlags.LooOutput) = cVariableMetaData.Get(eVarNameFlags.LooInput)
        Me.Metadata(eVarNameFlags.WinfOutput) = cVariableMetaData.Get(eVarNameFlags.WinfInput)
        Me.Metadata(eVarNameFlags.t0Output) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.TmaxOutput) = cVariableMetaData.Get(eVarNameFlags.TmaxInput)
        Me.Metadata(eVarNameFlags.FishMortTotMort) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.NatMortPerTotMort) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.Consumption) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.PredMort) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.SearchRate) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.Hlap) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.Plap) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.Alpha) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathWeight) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathNumber) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathBiomass) = [Default](eValueTypes.Sng, cUnits.Currency)
        Me.Metadata(eVarNameFlags.LorenzenMortality) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.PSD) = [Default](eValueTypes.Sng)

        ' -- Ecopath fleets  --
        ' in 
        Me.Metadata(eVarNameFlags.FixedCost) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Percentage)
        Me.Metadata(eVarNameFlags.EffortCost) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 40, cUnits.Percentage)
        Me.Metadata(eVarNameFlags.SailCost) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 40, cUnits.Percentage)
        Me.Metadata(eVarNameFlags.OffVesselPrice) = New cVariableMetaData(Single.MinValue, Single.MaxValue, ge, lt, 1, cUnits.MonetaryOverKg)
        Me.Metadata(eVarNameFlags.NominalEffort) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 1, cUnits.FishingEffort)
        Me.Metadata(eVarNameFlags.Landings) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.Discards) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.DiscardFate) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0) ' Unit?
        Me.Metadata(eVarNameFlags.DiscardMortality) = New cVariableMetaData(0, 1, ge, le, 1) ' Unit?

        ' out

        ' -- Stanza --
        Me.Metadata(eVarNameFlags.LeadingBiomass) = New cVariableMetaData(0, Integer.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.LeadingCB) = New cVariableMetaData(0, Integer.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.RecPowerSplit) = New cVariableMetaData(0, Single.MaxValue, gt, lt) ' default?
        Me.Metadata(eVarNameFlags.BABsplit) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt) ' default?
        Me.Metadata(eVarNameFlags.WmatWinf) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.HatchCode) = New cVariableMetaData(0, Integer.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.FixedFecundity) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EggAtSpawn) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.SpawnProp) = New cVariableMetaData(0, 1, ge, le, 1, cUnits.Proportion) ' default?
        Me.Metadata(eVarNameFlags.Bat) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' default?
        Me.Metadata(eVarNameFlags.StartAge) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, 0, "[Timestep]")
        Me.Metadata(eVarNameFlags.StanzaNumberAtAge) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' Default, units, range?
        Me.Metadata(eVarNameFlags.StanzaWeightAtAge) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' Default, units, range?
        Me.Metadata(eVarNameFlags.StanzaBiomassAtAge) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' Default, units, range?
        Me.Metadata(eVarNameFlags.StanzaGroup) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' Default, units, range?
        Me.Metadata(eVarNameFlags.StanzaBiomass) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' Default, units, range?
        Me.Metadata(eVarNameFlags.StanzaCB) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' Default, units, range?
        Me.Metadata(eVarNameFlags.StanzaMortaility) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' Default, units, range?

        ' -- Indicators --
        Me.Metadata(eVarNameFlags.GroupUsedInIndicators) = New cVariableMetaData()

        ' -- Ecopath stats --
        Me.Metadata(eVarNameFlags.EcopathStatsTotalConsumption) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalExports) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalRespFlow) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalFlowDetritus) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalThroughput) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalProduction) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsGrossEfficiency) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalNetPP) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalPResp) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsNetSystemProduction) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalPB) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalBT) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalBNonDet) = [Default](eValueTypes.Sng, cUnits.Currency)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalCatch) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcopathStatsConnectanceIndex) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsOmnivIndex) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalMarketValue) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalShadowValue) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalValue) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalFixedCost) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalVarCost) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcopathStatsTotalCost) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcopathStatsProfit) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcopathStatsPedigree) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsMeasureOfFit) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcopathStatsDiversity) = [Default](eValueTypes.Sng)

        ' -- Taxonomy and traits --
        Me.Metadata(eVarNameFlags.Phylum) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.Order) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.Class) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.Genus) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.Family) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.Species) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.TaxonGroup) = New cVariableMetaData(0, Integer.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.TaxonStanza) = New cVariableMetaData(0, Integer.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.CodeSAUP) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.CodeFB) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.CodeSLB) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.CodeLSID) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.CodeFAO) = New cVariableMetaData(13)
        Me.Metadata(eVarNameFlags.Source) = New cVariableMetaData(250)
        Me.Metadata(eVarNameFlags.SourceKey) = New cVariableMetaData(1024)
        Me.Metadata(eVarNameFlags.TaxonSearchFields) = New cVariableMetaData(0, Long.MaxValue, ge, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.TaxonPropBiomass) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.TaxonPropCatch) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.TaxonMeanLifespan) = [Default](eValueTypes.Sng, cUnits.Time)
        Me.Metadata(eVarNameFlags.TaxonMeanLength) = [Default](eValueTypes.Sng, "[cm]")
        Me.Metadata(eVarNameFlags.TaxonMaxLength) = [Default](eValueTypes.Sng, "[cm]")
        Me.Metadata(eVarNameFlags.TaxonMeanWeight) = [Default](eValueTypes.Sng, "[kg]")
        Me.Metadata(eVarNameFlags.OrganismType) = [Default](eValueTypes.Int)
        Me.Metadata(eVarNameFlags.EcologyType) = [Default](eValueTypes.Int)
        Me.Metadata(eVarNameFlags.IUCNConservationStatus) = [Default](eValueTypes.Int)
        Me.Metadata(eVarNameFlags.ExploitationStatus) = [Default](eValueTypes.Int)
        Me.Metadata(eVarNameFlags.OccurrenceStatus) = [Default](eValueTypes.Int)
        Me.Metadata(eVarNameFlags.TaxonVulnerabilityIndex) = [Default](eValueTypes.Int, "0-100")
        Me.Metadata(eVarNameFlags.TaxonWinf) = New cVariableMetaData(0, Single.MaxValue, gt, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.TaxonvbgfK) = New cVariableMetaData(0, Single.MaxValue, gt, lt, cCore.NULL_VALUE)

        ' -- Pedigree
        Me.Metadata(eVarNameFlags.VariableName) = New cVariableMetaData(0, [Enum].GetValues(GetType(eVarNameFlags)).Length, gt, le)
        Me.Metadata(eVarNameFlags.IndexValue) = New cVariableMetaData(0, 1, gt, le)
        Me.Metadata(eVarNameFlags.Pedigree) = New cVariableMetaData(0, Integer.MaxValue, ge, le, 0)

        ' -- PSD --
        Me.Metadata(eVarNameFlags.PSDEnabled) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.PSDComputed) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.PSDIncluded) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.PSDNumWeightClasses) = New cVariableMetaData(2, Integer.MaxValue, ge, lt, 2)
        Me.Metadata(eVarNameFlags.PSDMortalityType) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, 0)
        Me.Metadata(eVarNameFlags.PSDFirstWeightClass) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0)
        Me.Metadata(eVarNameFlags.ClimateType) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, 0)
        Me.Metadata(eVarNameFlags.NumPtsMovAvg) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, 0)

        ' -- Samples --
        Me.Metadata(eVarNameFlags.SampleRating) = New cVariableMetaData(0, 5, ge, le)
        Me.Metadata(eVarNameFlags.SampleSS) = New cVariableMetaData(0, Single.MaxValue, ge, lt, cCore.NULL_VALUE)

        ' -- Ecosim --
        ' params
        Me.Metadata(eVarNameFlags.StepSize) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.Discount) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' UNITS?
        Me.Metadata(eVarNameFlags.EquilibriumStepSize) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.EquilMaxFishingRate) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' UNITS?
        Me.Metadata(eVarNameFlags.NumStepAvg) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.NutBaseFreeProp) = New cVariableMetaData(0.1, 0.99999, ge, le, 2.0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.NutPBMax) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.SystemRecovery) = New cVariableMetaData(0, Single.MaxValue, ge, lt) ' UNITS?
        Me.Metadata(eVarNameFlags.ForagingTimeLowerLimit) = New cVariableMetaData(0.00001, 0.1, ge, le) ' UNITS?
        Me.Metadata(eVarNameFlags.NutForceFunctionNumber) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.EcoSimNYears) = New cVariableMetaData(0, cCore.MAX_RUN_LENGTH, ge, le)
        Me.Metadata(eVarNameFlags.EcosimSumStart) = New cVariableMetaData(0, 999, ge, le)
        Me.Metadata(eVarNameFlags.EcosimSumEnd) = New cVariableMetaData(0, 999, ge, le)
        Me.Metadata(eVarNameFlags.EcosimSumNTimeSteps) = New cVariableMetaData(1, 999, ge, le)
        Me.Metadata(eVarNameFlags.NudgeChecked) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.UseVarPQ) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.BiomassOn) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.ConSimOnEcoSim) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.PredictEffort) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EcosimSORWt) = New cVariableMetaData(0, 1, ge, le)
        Me.Metadata(eVarNameFlags.AdditivePredMortProp) = New cVariableMetaData(0, 1, ge, le, 1, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.VulnerabilityCap) = New cVariableMetaData(10, Single.MaxValue, ge, le, 100000000.0#)

        ' groups in
        Me.Metadata(eVarNameFlags.MaxRelPB) = New cVariableMetaData(0, Single.MaxValue, ge, le, 2)
        Me.Metadata(eVarNameFlags.MaxRelFeedingTime) = New cVariableMetaData(0, 100, gt, le, 2)
        Me.Metadata(eVarNameFlags.FeedingTimeAdjRate) = New cVariableMetaData(0, 1, ge, le, 0.5)
        Me.Metadata(eVarNameFlags.OtherMortFeedingTime) = New cVariableMetaData(0, 1, ge, le, 1)
        Me.Metadata(eVarNameFlags.PredEffectFeedingTime) = New cVariableMetaData(0, 1, ge, le, 0)
        Me.Metadata(eVarNameFlags.DenDepCatchability) = New cVariableMetaData(1, Single.MaxValue, ge, le, 1) ' QmQo
        Me.Metadata(eVarNameFlags.QBMaxQBio) = New cVariableMetaData(1, Single.MaxValue, ge, lt, 1000)
        Me.Metadata(eVarNameFlags.EcosimArenaShare) = New cVariableMetaData(0, 1, ge, le, 1)

        ' groups out
        Me.Metadata(eVarNameFlags.IsPred) = [Default](eValueTypes.Bool)
        Me.Metadata(eVarNameFlags.IsPrey) = [Default](eValueTypes.Bool)
        Me.Metadata(eVarNameFlags.EcosimIsCatchAggregated) = [Default](eValueTypes.Bool)
        Me.Metadata(eVarNameFlags.EcosimGroupBiomassStart) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcosimGroupBiomassEnd) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcosimGroupCatchStart) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcosimGroupCatchEnd) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcosimGroupValueStart) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.EcosimGroupValueEnd) = [Default](eValueTypes.Sng)

        ' fleets in
        Me.Metadata(eVarNameFlags.EPower) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 3) ' UNITS?
        Me.Metadata(eVarNameFlags.PcapBase) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 0.5!) ' UNITS?
        Me.Metadata(eVarNameFlags.CapDepreciate) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 0.06!) ' UNITS?
        Me.Metadata(eVarNameFlags.CapBaseGrowth) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 0.2!, cUnits.ProportionOverTime)
        Me.Metadata(eVarNameFlags.FleetEffortConversion) = New cVariableMetaData(0, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.SwitchingPower) = New cVariableMetaData(0, 2, ge, le, 0)
        Me.Metadata(eVarNameFlags.VulMult) = New cVariableMetaData(1, Single.MaxValue, ge, lt, 2)
        Me.Metadata(eVarNameFlags.EcosimRelQ) = New cVariableMetaData(1, Single.MaxValue, ge, lt, 1)

        ' fleets out. Must have metadata for units
        Me.Metadata(eVarNameFlags.EcosimFleetCatchStart) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcosimFleetCatchEnd) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcosimFleetValueStart) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcosimFleetValueEnd) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcosimFleetCostStart) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcosimFleetCostEnd) = [Default](eValueTypes.Sng, cUnits.Monetary)
        Me.Metadata(eVarNameFlags.EcosimFleetEffort) = [Default](eValueTypes.Sng) ' UNITS?
        Me.Metadata(eVarNameFlags.EcosimFleetProfit) = [Default](eValueTypes.Sng) ' UNITS?
        Me.Metadata(eVarNameFlags.EcosimFleetJobs) = [Default](eValueTypes.Sng) ' UNITS?

        ' Output
        Me.Metadata(eVarNameFlags.FIB) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.TLCatch) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.KemptonsQ) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.ShannonDiversity) = [Default](eValueTypes.Sng)
        Me.Metadata(eVarNameFlags.TotalCatch) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)

        ' -- Ecospace --
        ' parameters
        Me.Metadata(eVarNameFlags.NumTimeStepsPerYear) = New cVariableMetaData(0, Integer.MaxValue, gt, lt, cCore.N_MONTHS)
        Me.Metadata(eVarNameFlags.EcospaceRegionNumber) = New cVariableMetaData(0, 20000, ge, lt, 0)
        Me.Metadata(eVarNameFlags.AdjustSpace) = New cVariableMetaData(True)
        Me.Metadata(eVarNameFlags.TotalTime) = New cVariableMetaData(1, cCore.MAX_RUN_LENGTH, ge, le, 50)
        Me.Metadata(eVarNameFlags.Tolerance) = New cVariableMetaData(0.000001, 0.1, ge, le, 0.01)
        Me.Metadata(eVarNameFlags.SOR) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 0.9)
        Me.Metadata(eVarNameFlags.MaxIterations) = New cVariableMetaData(1, Integer.MaxValue, ge, lt, 1)
        Me.Metadata(eVarNameFlags.UseExact) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.ConSimOnEcoSpace) = New cVariableMetaData(False)

        Me.Metadata(eVarNameFlags.nGridSolverThreads) = New cVariableMetaData(0, Environment.ProcessorCount, gt, le, 1) ' Was N_CORES_HUNGABEE
        Me.Metadata(eVarNameFlags.nSpaceThreads) = New cVariableMetaData(0, Environment.ProcessorCount, gt, le, 1) ' Was N_CORES_HUNGABEE
        Me.Metadata(eVarNameFlags.nEffortDistThreads) = New cVariableMetaData(0, Environment.ProcessorCount, gt, le, 1) ' Was N_CORES_HUNGABEE

        Me.Metadata(eVarNameFlags.nIBMMovementThreads) = New cVariableMetaData(0, Environment.ProcessorCount, gt, le, 1) ' Was N_CORES_HUNGABEE

        Me.Metadata(eVarNameFlags.PacketsMultiplier) = New cVariableMetaData(0, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.EcospaceSummaryTimeStart) = New cVariableMetaData(1, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.EcospaceSummaryTimeEnd) = New cVariableMetaData(1, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.EcospaceNumberSummaryTimeSteps) = New cVariableMetaData(1, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.UseNewMultiStanza) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.UseIBM) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.EcospaceIBMMovePacketOnStanza) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.IFDPower) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 0.5)
        Me.Metadata(eVarNameFlags.EcospaceUseCoreOutputDir) = New cVariableMetaData(True)
        Me.Metadata(eVarNameFlags.EcospaceAutosaveAnnualOutput) = New cVariableMetaData(True)
        Me.Metadata(eVarNameFlags.EcospaceAutosaveSelectedGroupsFleetsOnly) = New cVariableMetaData(True)
        Me.Metadata(eVarNameFlags.EcospaceAutosaveFirstTimeStep) = New cVariableMetaData(1, Integer.MaxValue, gt, lt, 1)
        Me.Metadata(eVarNameFlags.UseEffortDistThreshold) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EffortDistThreshold) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 10000)
        Me.Metadata(eVarNameFlags.EcospaceAreaOutputDir) = New cVariableMetaData(1024)
        Me.Metadata(eVarNameFlags.EcospaceMapOutputDir) = New cVariableMetaData(1024)
        Me.Metadata(eVarNameFlags.EcospaceIsEcosimBiomassForcingLoaded) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EcospaceUseEcosimBiomassForcing) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EcospaceIsEcosimDiscardForcingLoaded) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EcospaceUseEcosimDiscardForcing) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.EcospaceMinForagingCapacity) = New cVariableMetaData(cEcoSpace.MIN_HABCAP, 0.1, ge, le, 0.01)
        Me.Metadata(eVarNameFlags.EcospaceUseHabCapGradCorrections) = New cVariableMetaData(True)
        Me.Metadata(eVarNameFlags.EcospaceEffortZoneNumber) = New cVariableMetaData(1, 999, gt, lt, 1)
        Me.Metadata(eVarNameFlags.EcospaceSpinupEnabled) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.EcospaceSpinupYears) = New cVariableMetaData(1, 999, ge, le, 10)
        Me.Metadata(eVarNameFlags.EcospaceIsExternalDataEnabled) = New cVariableMetaData(True)
        Me.Metadata(eVarNameFlags.EcospaceDoPenaltySearch) = New cVariableMetaData(True)
        Me.Metadata(eVarNameFlags.EcospacePenpow) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 10)
        Me.Metadata(eVarNameFlags.EcospaceNoFishWeight) = New cVariableMetaData(0, 1000, gt, le, 0.3)
        Me.Metadata(eVarNameFlags.EcospaceFirstPenaltyMonth) = New cVariableMetaData(1, 1200, ge, lt, 60)

        ' map
        Me.Metadata(eVarNameFlags.InRow) = New cVariableMetaData(1, 10000, ge, le)
        Me.Metadata(eVarNameFlags.InCol) = New cVariableMetaData(1, 10000, ge, le)
        Me.Metadata(eVarNameFlags.CellLength) = New cVariableMetaData(0, Single.MaxValue, gt, lt, , "[km]")
        Me.Metadata(eVarNameFlags.CellSize) = New cVariableMetaData(0, Single.MaxValue, gt, lt,, cUnits.Mapping)
        Me.Metadata(eVarNameFlags.Latitude) = New cVariableMetaData(Single.MinValue, Single.MaxValue, ge, le,, cUnits.Mapping)
        Me.Metadata(eVarNameFlags.Longitude) = New cVariableMetaData(Single.MinValue, Single.MaxValue, ge, le,, cUnits.Mapping)
        Me.Metadata(eVarNameFlags.AssumeSquareCells) = New cVariableMetaData(cUnits.TrueFalse)

        ' layers
        Me.Metadata(eVarNameFlags.LayerRelPP) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.LayerContaminantRelativeDistribution) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.LayerContaminantForcingAbsolute) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Multiplier)
        Me.Metadata(eVarNameFlags.LayerDepth) = New cVariableMetaData(Integer.MinValue, Integer.MaxValue, gt, lt, 0, cUnits.Depth)
        Me.Metadata(eVarNameFlags.LayerHabitat) = New cVariableMetaData(0, 1, ge, le, 1, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.LayerHabitatCapacityInput) = New cVariableMetaData(0, 1, ge, le, 1, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.LayerHabitatCapacity) = cVariableMetaData.Get(eVarNameFlags.LayerHabitatCapacityInput)
        Me.Metadata(eVarNameFlags.LayerRegion) = New cVariableMetaData(0, 1000, ge, le, 0, cUnits.Number)
        Me.Metadata(eVarNameFlags.LayerEffortZone) = New cVariableMetaData(1, 1000, ge, le, 1, cUnits.Number)
        Me.Metadata(eVarNameFlags.LayerMigration) = New cVariableMetaData(0, 1000, ge, le, 0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.LayerMPASeed) = New cVariableMetaData(0, Integer.MaxValue, ge, lt, 0)
        Me.Metadata(eVarNameFlags.LayerPort) = New cVariableMetaData(cUnits.PresenceAbsence)
        Me.Metadata(eVarNameFlags.LayerSail) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.LayerAdvection) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0, cUnits.Velocity)
        Me.Metadata(eVarNameFlags.LayerWind) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0, cUnits.Velocity)
        Me.Metadata(eVarNameFlags.LayerUpwelling) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0)
        Me.Metadata(eVarNameFlags.LayerDriver) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0)
        Me.Metadata(eVarNameFlags.LayerBiomassForcing) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0, cUnits.Biomass)
        Me.Metadata(eVarNameFlags.LayerBiomassRelativeForcing) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt, 0, cUnits.Proportion)
        Me.Metadata(eVarNameFlags.LayerExclusion) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.LayerCellArea) = New cVariableMetaData(1, Single.MaxValue, ge, lt, 0, cUnits.Area)
        Me.Metadata(eVarNameFlags.LayerMPA) = New cVariableMetaData(cUnits.PresenceAbsence)
        Me.Metadata(eVarNameFlags.ImportanceWeight) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 0)

        ' group in
        Me.Metadata(eVarNameFlags.MVel) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 300)
        Me.Metadata(eVarNameFlags.RelMoveBad) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 5, cUnits.Multiplier)
        Me.Metadata(eVarNameFlags.RelVulBad) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 2)
        Me.Metadata(eVarNameFlags.KMoveFitness) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Multiplier)
        Me.Metadata(eVarNameFlags.IsAdvected) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.IsMigratory) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.BarrierAvoidanceWeight) = New cVariableMetaData(0, 1, ge, le, 0)
        Me.Metadata(eVarNameFlags.EcospaceCapCalType) = New cVariableMetaData(0, Integer.MaxValue, ge, le)
        Me.Metadata(eVarNameFlags.InMigAreaMoveWeight) = New cVariableMetaData(0, 1, ge, le, 1)
        Me.Metadata(eVarNameFlags.PreferredHabitat) = New cVariableMetaData(0, 1, ge, le, 1)
        Me.Metadata(eVarNameFlags.EcospaceFTarget) = New cVariableMetaData(0, 1000, gt, le, 1000)

        ' group out
        Me.Metadata(eVarNameFlags.EcospaceGroupBiomassStart) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcospaceGroupBiomassEnd) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcospaceGroupCatchStart) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcospaceGroupCatchEnd) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcospaceGroupValueStart) = [Default](eValueTypes.Sng, cUnits.MonetaryOverKg)
        Me.Metadata(eVarNameFlags.EcospaceGroupValueEnd) = [Default](eValueTypes.Sng, cUnits.MonetaryOverKg)

        ' fleet int
        Me.Metadata(eVarNameFlags.EffectivePower) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 1)
        Me.Metadata(eVarNameFlags.SEmult) = New cVariableMetaData(0, Single.MaxValue, gt, lt, 1)
        Me.Metadata(eVarNameFlags.HabitatFishery) = New cVariableMetaData(False)
        Me.Metadata(eVarNameFlags.MPAFishery) = New cVariableMetaData(False)

        ' fleet out
        Me.Metadata(eVarNameFlags.EcospaceFleetCatchStart) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcospaceFleetCatchEnd) = [Default](eValueTypes.Sng, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.EcospaceFleetValueStart) = [Default](eValueTypes.Sng, cUnits.MonetaryOverKg)
        Me.Metadata(eVarNameFlags.EcospaceFleetValueEnd) = [Default](eValueTypes.Sng, cUnits.MonetaryOverKg)
        Me.Metadata(eVarNameFlags.EcospaceFleetCostStart) = [Default](eValueTypes.Sng, cUnits.MonetaryOverKg)
        Me.Metadata(eVarNameFlags.EcospaceFleetCostEnd) = [Default](eValueTypes.Sng, cUnits.MonetaryOverKg)
        Me.Metadata(eVarNameFlags.EcospaceFleetEffortES) = [Default](eValueTypes.Sng, cUnits.Proportion)

        ' Habitats
        Me.Metadata(eVarNameFlags.HabAreaProportion) = New cVariableMetaData(0, 1, ge, le, 1)

        ' MPA
        Me.Metadata(eVarNameFlags.MPAMonth) = New cVariableMetaData()

        ' Advection
        Me.Metadata(eVarNameFlags.XVelocity) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.YVelocity) = New cVariableMetaData(Single.MinValue, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.Coriolis) = New cVariableMetaData(-1, 1, ge, le)
        Me.Metadata(eVarNameFlags.SorWv) = New cVariableMetaData(-1, 1, ge, le)
        Me.Metadata(eVarNameFlags.AdvectionUpwellingThreshold) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.AdvectionUpwellingPPMultiplier) = New cVariableMetaData(0, 1000, ge, le)

        ' -- Ecotracer --
        ' parameters
        Me.Metadata(eVarNameFlags.CInflow) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.COutflow) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.CPhysicalDecayRate) = New cVariableMetaData(0, Single.MaxValue, ge, lt,, cUnits.CurrencyOverTime)
        Me.Metadata(eVarNameFlags.ConForceNumber) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.ConMaxTimeSteps) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)

        ' group in
        Me.Metadata(eVarNameFlags.CZero) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Contaminants & "/" & cUnits.Area)
        Me.Metadata(eVarNameFlags.CImmig) = New cVariableMetaData(0, Single.MaxValue, ge, lt, 0, cUnits.Contaminants & "/" & cUnits.Area)
        Me.Metadata(eVarNameFlags.CEnvironment) = New cVariableMetaData(0, Single.MaxValue, ge, lt,, "t/t/t/[Time]")
        Me.Metadata(eVarNameFlags.CAssimilationProp) = New cVariableMetaData(0, 1, ge, lt)
        Me.Metadata(eVarNameFlags.CMetablismRate) = New cVariableMetaData(0 + Single.Epsilon, Single.MaxValue, gt, lt,, "t/[Time]")

        ' -- Searches --
        ' groups
        Me.Metadata(eVarNameFlags.FPSGroupMandRelBiom) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSGroupStrucRelWeight) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSFishingLimit) = New cVariableMetaData(0, 1000, ge, le)

        ' fleet 
        Me.Metadata(eVarNameFlags.FPSFleetJobCatchValue) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSFleetTargetProfit) = New cVariableMetaData(0, Single.MaxValue, ge, lt)

        ' objective weights
        Me.Metadata(eVarNameFlags.FPSEconomicWeight) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSSocialWeight) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSBiomassDiversityWeight) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSMandatedRebuildingWeight) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSEcoSystemWeight) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSPredictionVariance) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSExistenceValue) = New cVariableMetaData(0, Single.MaxValue, ge, lt)

        '  objective parameters
        Me.Metadata(eVarNameFlags.SearchDiscountRate) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.SearchGenDiscRate) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.SearchBaseYear) = New cVariableMetaData(0, 1000, ge, lt)
        Me.Metadata(eVarNameFlags.SearchPrevCostEarning) = [Default](eValueTypes.Bool)
        Me.Metadata(eVarNameFlags.SearchFishingMortalityPenalty) = [Default](eValueTypes.Bool)

        ' Fishing policy
        Me.Metadata(eVarNameFlags.FPSMaxNumEval) = New cVariableMetaData(1, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSMaxEffChange) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.FPSInitOption) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Enum validation handled by dedicated validator
        Me.Metadata(eVarNameFlags.FPSSearchOption) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Enum validation handled by dedicated validator
        Me.Metadata(eVarNameFlags.FPSOptimizeApproach) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Enum validation handled by dedicated validator
        Me.Metadata(eVarNameFlags.FPSOptimizeOptions) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Enum validation handled by dedicated validator
        Me.Metadata(eVarNameFlags.FPSNRuns) = New cVariableMetaData(1, 1000, ge, le)
        Me.Metadata(eVarNameFlags.FPSMaxPortUtil) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.FPSIncludeComp) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.FPSBatchRun) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.FPSUseEcospace) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.FPSUseEconomicPlugin) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.SearchBlock) = New cVariableMetaData(1, Integer.MaxValue, ge, lt)

        ' -- Fit to time series --
        Me.Metadata(eVarNameFlags.F2TSVulnerabilitySearch) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.F2TSAnomalySearch) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.F2TSUseDefaultV) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.F2TSCatchAnomaly) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.F2TSFirstYear) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.F2TSLastYear) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.F2TSVulnerabilityVariance) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.F2TSPPVariance) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.F2TSCatchAnomalySearchShapeNumber) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.F2TSNumSplinePoints) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.F2TSAppliedWeights) = New cVariableMetaData(0, 1, ge, le)
        Me.Metadata(eVarNameFlags.F2TSNAICData) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)

        ' -- Monte Carlo --
        ' manager
        ' group
        Me.Metadata(eVarNameFlags.mcB) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBcv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBA) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBALower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBAUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBAbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBAcv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBaBi) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBaBiUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBaBiLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBaBibf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcBaBicv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcPB) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcPBLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcPBUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcPBbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcPBcv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcQB) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcQBLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcQBUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcQBbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcQBcv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcEE) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcEELower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcEEUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcEEbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcEEcv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcVU) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcVULower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcVUUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcVUbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcVUcv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDC) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDCbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDietMult) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcLandings) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcLandingsUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcLandingsLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcLandingsbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcLandingscv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDiscards) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDiscardsUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDiscardsLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDiscardsbf) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.mcDiscardscv) = New cVariableMetaData(0, Single.MaxValue, ge, lt)

        ' -- MPA opt --
        ' parameters
        Me.Metadata(eVarNameFlags.MPAOptSearchType) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Special validator takes care of enum checking
        Me.Metadata(eVarNameFlags.MPAOptBoundaryWeight) = New cVariableMetaData(0.0!, 10.0!, ge, le) ' Default?
        Me.Metadata(eVarNameFlags.MPAOptStepSize) = New cVariableMetaData(0, 1000, ge, lt)
        Me.Metadata(eVarNameFlags.MPAOptIterations) = New cVariableMetaData(0, Integer.MaxValue, ge, le)
        Me.Metadata(eVarNameFlags.MPAOptMaxArea) = New cVariableMetaData(0, 100, ge, le)
        Me.Metadata(eVarNameFlags.MPAOptMinArea) = New cVariableMetaData(0, 100, ge, le)
        Me.Metadata(eVarNameFlags.iMPAOptToUse) = New cVariableMetaData(0, Integer.MaxValue, ge, le)
        Me.Metadata(eVarNameFlags.MPAOptStartYear) = New cVariableMetaData(0, 2000, ge, le)
        Me.Metadata(eVarNameFlags.MPAOptEndYear) = New cVariableMetaData(0, 2000, ge, le)
        Me.Metadata(eVarNameFlags.MPAOptUseRegions) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MPAUseCellWeight) = New cVariableMetaData()

        ' -- MSE --
        ' group in
        Me.Metadata(eVarNameFlags.MSEBioCV) = New cVariableMetaData(0, 1, ge, le)
        Me.Metadata(eVarNameFlags.MSELowerRisk) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEUpperRisk) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefBioLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefBioUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefBioEstLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefBioEstUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefGroupCatchLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefGroupCatchUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEForcastGain) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.RHalfB0Ratio) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEFixedF) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERecruitmentCV) = New cVariableMetaData(0, 1, ge, le)
        Me.Metadata(eVarNameFlags.MSETAC) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBBase) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBLim) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEFmax) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEFmin) = New cVariableMetaData(0, Single.MaxValue, ge, lt)

        ' fleet in
        Me.Metadata(eVarNameFlags.MSEQIncrease) = New cVariableMetaData(0, Single.MaxValue, gt, lt)
        Me.Metadata(eVarNameFlags.MSEFleetCV) = New cVariableMetaData(0, 1, ge, le)
        Me.Metadata(eVarNameFlags.MSEFleetWeight) = New cVariableMetaData(1, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefFleetCatchLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefFleetCatchUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefFleetEffortLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSERefFleetEffortUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSYEvaluateFleet) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MaxEffort) = New cVariableMetaData(0, Single.MaxValue, ge, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.MSELowerLPEffort) = New cVariableMetaData(0, Single.MaxValue, ge, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.MSEUpperLPEffort) = New cVariableMetaData(0, Single.MaxValue, ge, lt, cCore.NULL_VALUE)
        Me.Metadata(eVarNameFlags.QuotaType) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Special validator takes care of enum checking 
        Me.Metadata(eVarNameFlags.QuotaShare) = New cVariableMetaData(0, Single.MaxValue, ge, lt)

        ' batch params
        Me.Metadata(eVarNameFlags.MSETFMNIteration) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchFNIteration) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchTACNIteration) = New cVariableMetaData(0, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchIterCalcType) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Special validator takes care of enum checking 
        Me.Metadata(eVarNameFlags.MSEBatchOutputBiomass) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSEBatchOutputConBio) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSEBatchOutputFeedingTime) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSEBatchOutputPredRate) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSEBatchOutputCatch) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSEBatchOutputFishingMortRate) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSEBatchOuputDir) = New cVariableMetaData(1024)

        ' batch group
        Me.Metadata(eVarNameFlags.MSEBatchFLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchFUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchFManaged) = New cVariableMetaData()

        ' batch TAC group
        Me.Metadata(eVarNameFlags.MSEBatchTACLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchTACUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchTACManaged) = New cVariableMetaData()

        ' batch TFM group
        Me.Metadata(eVarNameFlags.MSETFMBLimLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSETFMBLimUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSETFMBBaseLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSETFMBBaseUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSETFMFOptLower) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSETFMFOptUpper) = New cVariableMetaData(0, Single.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSEBatchTFMManaged) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSETFMFOptValues) = New cVariableMetaData(1, Single.MaxValue, ge, lt, 1)
        Me.Metadata(eVarNameFlags.MSETFMBBaseValues) = New cVariableMetaData(1, Single.MaxValue, ge, lt, 1)
        Me.Metadata(eVarNameFlags.MSETFMBLimValues) = New cVariableMetaData(1, Single.MaxValue, ge, lt, 1)

        ' -- MSY --
        ' parameters
        Me.Metadata(eVarNameFlags.MSYFSelection) = New cVariableMetaData(1, Integer.MaxValue, ge, lt)
        Me.Metadata(eVarNameFlags.MSYFSelectionMode) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Special validator takes care of enum checking 
        Me.Metadata(eVarNameFlags.MSYAssessment) = New cVariableMetaData()
        Me.Metadata(eVarNameFlags.MSYRunLengthMode) = New cVariableMetaData(0, Integer.MaxValue, ge, lt) ' Special validator takes care of enum checking 
        Me.Metadata(eVarNameFlags.MSYMaxFishingRate) = New cVariableMetaData(1, 1000000, ge, le)
        Me.Metadata(eVarNameFlags.MSYNumTrialYears) = New cVariableMetaData(1, 1000, ge, le)
        Me.Metadata(eVarNameFlags.MSYEquilibriumStepSize) = New cVariableMetaData(0, 1, ge, le)

#If DEBUG Then
        Try
            Dim sw As New System.IO.StreamWriter(".\cVariableMetadata_log.txt")
            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
            sw.WriteLine("VarNames without registered metadata")
            For Each vn As eVarNameFlags In [Enum].GetValues(GetType(eVarNameFlags))
                If Not s_metadata.ContainsKey(vn) Then
                    Dim str As String = cin.GetVarName(vn)
                    If Not str.StartsWith("Game") Then
                        sw.WriteLine(str)
                    End If
                End If
            Next
            sw.Flush()
            sw.Close()
        Catch ex As Exception

        End Try
#End If

    End Sub

#End Region 'Singleton

#Region "Constructors"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for boolean values.
    ''' </summary>
    ''' <param name="units">Units format mask</param>
    ''' <param name="bValueDefault">Default value to assign to variable when in error.</param>
    ''' <remarks>Booleans do not have min or max values.</remarks>
    ''' -----------------------------------------------------------------------
    Public Sub New(units As String, Optional bValueDefault As Boolean = False)
        Me.NullValue = bValueDefault
        Me.m_vartype = eValueTypes.Bool
        Me.Min = Math.Min(CSng(True), CSng(False))
        Me.Max = Math.Max(CSng(True), CSng(False))
        Me.Units = units
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor use boolean values.
    ''' </summary>
    ''' <param name="bValueDefault">Default value to assign to variable when in error.</param>
    ''' <remarks>Booleans do not have min or max values.</remarks>
    ''' -----------------------------------------------------------------------
    Public Sub New(Optional bValueDefault As Boolean = False)
        Me.New(cUnits.TrueFalse, bValueDefault)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constuctor for string values.
    ''' </summary>
    ''' <param name="iLength">The max allowed string length.</param>
    ''' <param name="strValueDefault">
    ''' Default value to assign to variable when in error.</param>
    ''' <remarks>Strings do not have min or max values.</remarks>
    ''' -----------------------------------------------------------------------
    Public Sub New(iLength As Integer, Optional strValueDefault As String = "")
        Me.Length = iLength
        Me.NullValue = strValueDefault
        Me.m_vartype = eValueTypes.Str
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for numeric values.
    ''' </summary>
    ''' <param name="sMin">Lowest value a variable can contain.</param>
    ''' <param name="sMax">Highest value a variable can contain.</param>
    ''' <param name="operatorMin"><see cref="cOperatorBase">Operator</see>
    ''' stating how the <paramref name="sMin">minimum value</paramref> is included
    ''' in the variable value range.</param>
    ''' <param name="operatorMax"><see cref="cOperatorBase">Operator</see>
    ''' stating how the <paramref name="sMax">maximum value</paramref> is included
    ''' in the variable value range.</param>
    ''' <param name="sValueDefault">Default value to assign to variable when in error.</param>
    ''' <param name="units">Units of the value.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(sMin As Single, sMax As Single,
                   operatorMin As cOperatorBase, operatorMax As cOperatorBase,
                   Optional sValueDefault As Single = 0.0!,
                   Optional units As String = "")
        Me.Min = sMin
        Me.Max = sMax
        Me.MinOperator = operatorMin
        Me.MaxOperator = operatorMax
        Me.NullValue = sValueDefault
        Me.Units = units
    End Sub

#End Region 'Constructors

#Region "Central registry"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get any pre-registered metadata, or nothing if the metadata does not
    ''' exist for the indicated variable.
    ''' </summary>
    ''' <param name="varname"></param>
    ''' -----------------------------------------------------------------------
    Public Shared Function [Get](varname As eVarNameFlags) As cVariableMetaData
        Return GetInstance().Metadata(varname)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Register and retrieve metadata for numerical model output values.
    ''' </summary>
    ''' <param name="vartype"></param>
    ''' <param name="units"></param>
    ''' <remarks>
    ''' Metadata for computed values do not need specification of range operators
    ''' to support variable validation.
    ''' </remarks>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function [Default](vartype As eValueTypes,
                                     Optional units As String = "") As cVariableMetaData

        Dim md As cVariableMetaData = Nothing

        Select Case vartype
            Case eValueTypes.Bool, eValueTypes.BoolArray
                md = New cVariableMetaData()
            Case eValueTypes.Int, eValueTypes.IntArray
                md = New cVariableMetaData(Integer.MinValue, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), units:=units)
            Case eValueTypes.Sng, eValueTypes.SingleArray
                md = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), units:=units)
            Case eValueTypes.Str
                md = New cVariableMetaData(32)
        End Select
        Return md

    End Function

    Private Property Metadata(varname As eVarNameFlags) As cVariableMetaData
        Get
            If (s_metadata.ContainsKey(varname)) Then Return s_metadata(varname)
            Return Nothing
        End Get
        Set(value As cVariableMetaData)
            Debug.Assert(Not s_metadata.ContainsKey(varname), "Variable " & varname.ToString() & " already registered")
            s_metadata(varname) = value
        End Set
    End Property

#End Region 'Central registry

#Region "Operators"

    Friend Sub Attach(value As cValue)

        Me.m_vartype = value.varType

        Select Case Me.m_vartype
            Case eValueTypes.Bool, eValueTypes.Int, eValueTypes.Sng
                Debug.Assert(value.Length = 0, "Metadata malformed")
            Case eValueTypes.BoolArray, eValueTypes.IntArray, eValueTypes.SingleArray
                ' NOP
            Case eValueTypes.Str
                ' NOP
            Case Else
                Throw New NotImplementedException()
        End Select

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the minimum value <see cref="cOperatorBase">operator</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MinOperator() As cOperatorBase

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum value <see cref="cOperatorBase">operator</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MaxOperator() As cOperatorBase

#End Region 'Operators

#Region "Properties"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the minimum value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Min() As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Max() As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the default value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NullValue() As Object

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum allowed string length for variables.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Length() As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="eValueTypes">value type</see> of the variable 
    ''' that this metadata represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property VarType As eValueTypes
        Get
            Return Me.m_vartype
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the units for this variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Units As String
        Get
            Return Me.m_units
        End Get
        Friend Set(value As String)
            Me.m_units = value
        End Set
    End Property

#End Region 'Properties

#Region "Overrides"

    Public Overrides Function Equals(obj As Object) As Boolean

        If (TypeOf obj Is cVariableMetaData) Then
            Dim md As cVariableMetaData = DirectCast(obj, cVariableMetaData)
            If (md.m_vartype <> Me.m_vartype) Then
                Return False
            End If
            If (md.Length <> Me.Length) Then
                Return False
            End If
            If (md.Units <> Me.Units) Then
                Return False
            End If
            Return True
        End If
        Return False

    End Function

#End Region 'Overrides

End Class

