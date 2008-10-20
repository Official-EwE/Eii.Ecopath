'==============================================================================
'
' $Log: Shared Enums.vb,v $
' Revision 1.7  2008/10/20 20:22:06  joeb
' Moved eQuotaTypes here
'
' Revision 1.6  2008/10/15 23:53:28  jeroens
' more basemap definitions
'
' Revision 1.5  2008/10/08 20:32:11  joeb
' Added CVBest and KalWt
'
' Revision 1.4  2008/10/08 17:41:07  jeroens
' Added target fishing mortality policy vars
'
' Revision 1.3  2008/10/03 21:52:44  jeroens
' Added Fisheries regulations varnames
'
' Revision 1.2  2008/09/26 21:21:35  joeb
' Added GameFleetFishingRates and GameGroupFishingRates
'
' Revision 1.1  2008/09/26 07:31:10  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.46  2008/09/26 00:22:50  villyc
' updating ecosimMonteCarlo to pick vulnerabilities
'
' Revision 1.45  2008/09/25 02:31:49  jeroens
' Moved max fishing mortaility from search datastructures to Ecosim
'
' Revision 1.44  2008/09/23 19:16:47  jeroens
' Added SearchFishingMortalityPenalty
'
' Revision 1.43  2008/09/19 19:33:44  joeb
' Added GameBiomassByRegion
'
' Revision 1.42  2008/09/17 22:27:59  joeb
' Added FleetName to GameServer data
'
' Revision 1.41  2008/09/17 01:24:58  jeroens
' Fixed currency unit enum values in accordance to EwE5
'
' Revision 1.40  2008/09/16 14:57:04  joeb
' Added nGameSimTimeStepsPerYear to Counters number of timesteps per year
'
' Revision 1.39  2008/09/15 16:58:21  joeb
' Added more Ecospace output for Game Server
'
' Revision 1.38  2008/09/06 15:43:22  joeb
' Renamed GameTimestep to GameData
'
' Revision 1.37  2008/09/05 19:12:42  joeb
' Added GameState varname
'
' Revision 1.36  2008/09/04 15:49:52  joeb
' Added GameYield varname
'
' Revision 1.35  2008/09/04 14:56:56  joeb
' Rename SimulationTimeStep to GameSimulationTimeStep
'
' Revision 1.34  2008/09/04 00:32:36  joeb
' Added GameBiomass
'
' Revision 1.33  2008/09/02 19:33:50  joeb
' Added GameModell as Varname and GameData as datatype
'
' Revision 1.32  2008/09/02 17:55:28  joeb
' Added GameCounters
'
' Revision 1.31  2008/08/25 18:11:48  joeb
' Added TimeStep as an eInputOutput type
'
' Revision 1.30  2008/08/15 18:35:22  joeb
' Added TotalValue and PercentageClosed to cMPAOptOutPut
'
' Revision 1.29  2008/08/14 18:07:06  joeb
' Added StartYear and EndYear to MPA Optimizations
'
' Revision 1.28  2008/08/13 17:33:34  jeroens
' Renamed LayerImportanceWeight to ImportanceWeight
'
' Revision 1.27  2008/08/12 16:10:29  joeb
' Added varnames for Game Server
'
' Revision 1.26  2008/08/08 02:54:01  jeroens
' Added varname for new layer type
'
' Revision 1.25  2008/08/07 19:41:24  sherman
' Exposed LayerImportance from the Core
'
' Revision 1.24  2008/08/07 18:19:57  sherman
' Added Importance Layers to EcospaceDatastructures
'
' Revision 1.23  2008/08/04 02:27:45  jeroens
' Renamed varname MarketPrice to OffVesselPrice
'
' Revision 1.22  2008/07/21 14:05:11  jeroens
' Added pedigree vars
'
' Revision 1.21  2008/07/17 18:14:59  jeroens
' Added eUnitMonetaryType
' Added vars for monetary units
'
' Revision 1.20  2008/07/16 13:27:15  jeroens
' Fixed comment
'
' Revision 1.19  2008/07/10 18:19:10  jeroens
' Added unit enums
'
' Revision 1.18  2008/06/27 02:32:59  jeroens
' Added LayerMPARandom
'
' Revision 1.17  2008/06/24 21:56:09  joeb
' Added bUseCellWeight
'
' Revision 1.16  2008/06/20 19:43:39  joeb
' Added SSGroup to EcosimStats
'
' Revision 1.15  2008/06/18 18:21:01  joeb
' Added iMPAOptToUse
'
' Revision 1.14  2008/06/17 19:58:57  joeb
' Added EcosimResults varname
'
' Revision 1.13  2008/06/16 20:28:36  joeb
' Added FishingRate varname
'
' Revision 1.12  2008/06/11 17:26:12  joeb
' Added Varnames for MPAOpt
'
' Revision 1.11  2008/06/11 15:55:23  joeb
' Changed EcoSeed to MPAOpt
'
' Revision 1.10  2008/06/10 22:05:36  joeb
' Changes for new MPA optimization
'
' Revision 1.9  2008/06/09 22:00:28  jeroens
' Added core counter NotSet
'
' Revision 1.8  2008/06/09 21:57:11  jeroens
' Added core counters
'
'==============================================================================

Option Strict On

Namespace Core

#Region " Core execution state "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type identifying known core states.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eCoreExecutionState As Integer
        ''' <summary>The core is initialized and ready for use.</summary>
        Idle
        ''' <summary>Ecopath model data has been loaded.</summary>
        EcopathLoaded
        ''' <summary>Ecopath model is running.</summary>
        EcopathRunning
        ''' <summary>Ecopath model run is completed.</summary>
        EcopathCompleted
        ''' <summary>Ecosim scenario data has been loaded.</summary>
        EcosimLoaded
        ''' <summary>Ecotracer scenario data has been loaded.</summary>
        EcotracerLoaded
        ''' <summary>Ecosim scenario is running.</summary>
        EcosimRunning
        ''' <summary>Ecosim scenario run is completed.</summary>
        EcosimCompleted
        ''' <summary>Ecospace scenario data has been loaded.</summary>
        EcospaceLoaded
        ''' <summary>Ecospace scenario is running.</summary>
        EcospaceRunning
        ''' <summary>Ecospace scenario run is completed.</summary>
        EcospaceCompleted

    End Enum

#End Region ' Core execution state

#Region " Variable names "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type used for exposing variables a.k.a. parameters provided by
    ''' the Core models.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eVarNameFlags

        ''' <summary>Variable name is not specified.</summary>
        NotSet
        ''' <summary>Production over Biomass (ratio)</summary>
        ''' <remarks>Also referred to as Mortality or Z.</remarks>
        PBInput
        ''' <summary></summary>
        PBOutput
        ''' <summary></summary>
        EEInput
        ''' <summary></summary>
        EEOutput
        ''' <summary></summary>
        QBInput
        ''' <summary></summary>
        QBOutput
        ''' <summary></summary>
        GEInput
        ''' <summary></summary>
        GEOutput

        ''' <summary>Generic item names.</summary>
        Name
        ''' <summary>Numerical position of an item in a list.</summary>
        ''' <remarks>This value has replaced former EwE5 indices such as iGroup.</remarks>
        Index
        ''' <summary>Area surface, as a fraction.</summary>
        Area
        ''' <summary>Biomass, in ..</summary>
        Biomass
        ''' <summary><see cref="eVarNameFlags.Biomass">Biomass</see> per <see cref="eVarNameFlags.Area">Area</see>.</summary>
        BiomassAreaInput
        ''' <summary><see cref="eVarNameFlags.Biomass">Biomass</see> per <see cref="eVarNameFlags.Area">Area</see>.</summary>
        BiomassAreaOutput
        ''' <summary></summary>
        BioAccum
        ''' <summary></summary>
        BioAccumRatePerYear
        ''' <summary></summary>
        GS
        ''' <summary></summary>
        DetImp
        ''' <summary></summary>
        TTLX
        ''' <summary></summary>
        Immig
        ''' <summary></summary>
        Emig
        ''' <summary></summary>
        EmigRate
        ''' <summary></summary>
        BioAccumRate
        ''' <summary></summary>
        DietComp
        ''' <summary></summary>
        ImpDiet
        ''' <summary></summary>
        DetritusFate
        'Fleet definition table parameter names; Added by FG on Jan 26 2006
        ''' <summary></summary>
        FixedCost
        ''' <summary></summary>
        CPUECost
        ''' <summary></summary>
        SailCost
        ''' <summary></summary>
        EPower
        ''' <summary></summary>
        PcapBase
        ''' <summary></summary>
        CapDepreciate
        ''' <summary></summary>
        CapBaseGrowth
        'Mortality - Coefficients parameter names; Added by FG on Jan 26 2006
        ''' <summary></summary>
        MortCoPB
        ''' <summary></summary>
        MortCoFishRate
        ''' <summary></summary>
        MortCoPredMort
        ''' <summary></summary>
        MortCoBioAcumRate
        ''' <summary></summary>
        MortCoNetMig
        ''' <summary></summary>
        MortCoOtherMort

        'added by JB Jan-30-06 for EcoPath Outputs
        ''' <summary></summary>
        Consumption
        ''' <summary></summary>
        ImportedConsumption

        'added by JB Feb-01-06 for EcoPath Outputs
        ''' <summary></summary>
        PredMort

        'added by JB Feb-07-2006
        ''' <summary></summary>
        Landings
        ''' <summary></summary>
        Discards
        ''' <summary></summary>
        OffVesselPrice
        ''' <summary></summary>
        NonMarketValue
        ''' <summary></summary>
        DiscardFate

        'added by JB for EcoSim Group info Feb-14-06
        ''' <summary></summary>
        MaxRelPB
        ''' <summary></summary>
        MaxRelFeedingTime
        ''' <summary></summary>
        FeedingTimeAdjRate
        ''' <summary></summary>
        OtherMortFeedingTime
        ''' <summary></summary>
        PredEffectFeedingTime
        ''' <summary></summary>
        DenDepCatchability
        ''' <summary></summary>
        QBMaxQBio
        ''' <summary></summary>
        SwitchingPower
        ''' <summary></summary>
        VBGF
        ''' <summary></summary>
        VulRate
        ''' <summary></summary>
        VulMult
        ''' <summary></summary>
        ForcingFunctNumber
        ''' <summary></summary>
        MedFunctNumber
        'IsPredPrey ' States if has an existing predator/prey relationship

        'added by JB for EcoSim model parameters Feb-26-06
        ''' <summary></summary>
        StepSize
        ''' <summary></summary>
        Relaxation
        ''' <summary></summary>
        Discount
        ''' <summary></summary>
        EquilibriumStepSize
        ''' <summary></summary>
        EquilMaxFishingRate
        ''' <summary></summary>
        NumStepAvg
        ''' <summary></summary>
        NutBaseFreeProp
        ''' <summary></summary>
        NutForceFunctionNumber
        ''' <summary></summary>
        NutPBMax
        ''' <summary></summary>
        SystemRecovery
        ''' <summary></summary>
        NudgeChecked
        ''' <summary></summary>
        UseVarPQ
        ''' <summary></summary>
        BiomassOn
        ''' <summary></summary>
        EcoSimNYears

        'js Fisheries regulations added Oct 3 '08
        ''' <summary>Maximum effort of a fleet</summary>
        MaxEffort
        ''' <summary>Quota type imposed on a fleet</summary>
        QuotaType
        ''' <summary>Quota set for a gear/group combination</summary>
        Quota
        ''' <summary>Proportion of discards that dies</summary>
        DiscardMortality

        ''' <summary>Flag stating whether to use regulatory feedback.</summary>
        RegFeedback

        ' Target fishing mortality policy vars
        ''' <summary>Quota for a species.</summary>
        QuotaSpecies
        ''' <summary>BBase for target fishing mortality policy.</summary>
        BBase
        ''' <summary>BLimit for target fishing mortality policy.</summary>
        BLim
        ''' <summary>Mortality/Fmsy for target fishing mortality policy.</summary>
        Fopt

        ''' <summary>Coefficient of variation in estimated biomass for regulated fisheries .</summary>
        RegCVBest

        ''' <summary>Kalman weight for regulated fisheries ????</summary>
        RegKalWt

        'jb Salinity values added Dec-07
        SalinityForceFunctionNumber
        SalinityOpt
        ' SalinitySpread
        SalinitySpreadLeft
        SalinitySpreadRight


        ''' <summary>Contaminant tracing on/off</summary>
        ConSimOnEcoSim
        ConSimOnEcoSpace

        ''' <summary>Predict Ecosim Fishing Effort</summary>
        PredictEffort

        ''' <summary>Start of summary time period in years</summary>
        EcosimSumStart

        ''' <summary>end of summary time period in years</summary>
        EcosimSumEnd

        ''' <summary>number of time steps to summarize ecosim data over</summary>
        EcosimSumNTimeSteps

        ''' <summary>Database ID.</summary>
        DBID
        ''' <summary></summary>
        CyclePath

        'jb June-13-06 added for precentage of primary production
        ''' <summary>Percentage of primary production.</summary>
        PP

        'js 060630 added for storing generic EwE5 remarks
        ''' <summary>Generic description.</summary>
        Description
        ''' <summary>Number of digits to display.</summary>
        NumDigits
        ''' <summary>Unit enumerated value for text-based values.</summary>
        UnitTime
        ''' <summary>Unit text for time-based values.</summary>
        UnitTimeCustomText
        ''' <summary>Unit enumerated value for currency-based values.</summary>
        UnitCurrency
        ''' <summary>Unit text for currency-based values.</summary>
        UnitCurrencyCustomText
        ''' <summary>Unit enumerated value for monetary values.</summary>
        UnitMonetary
        ''' <summary>Unit text for monetary values.</summary>
        UnitMonetaryCustomText
        'js 071030 added for storing more generic EwE5 remarks
        ''' <summary>Author of an EwE component.</summary>
        Author
        ''' <summary>Contact info of an EwE component.</summary>
        Contact
        ''' <summary>Julian day an EwE component was last saved.</summary>
        LastSaved

        'js 060818 added for ecopath outputs
        ''' <summary></summary>
        NetMigration
        ''' <summary></summary>
        FlowToDet
        ''' <summary></summary>
        NetEfficiency
        ''' <summary></summary>
        OmnivoryIndex
        ''' <summary></summary>
        Respiration
        ''' <summary></summary>
        Assimilation
        ''' <summary>Resp / Assim</summary>
        RespAssim
        ''' <summary>Prod / Resp</summary>
        ProdResp
        ''' <summary>Resp / Biomass</summary>
        RespBiom
        ''' <summary>To document</summary>
        SearchRate
        ''' <summary>To document</summary>
        Hlap
        ''' <summary>To document</summary>
        Plap
        ''' <summary>Colour value to represent an exposed core I/O object.</summary>
        PoolColor
        ''' <summary>To document</summary>
        Alpha ' Borrowed from EwE5 EcoRanger

        ''' <summary>Recruitment power</summary>
        RecPowerSplit
        ''' <summary>Relative biomass accumulation rate (ratio)</summary>
        BABsplit
        ''' <summary>Weight at maturity over weight at infancy (ratio)</summary>
        WmatWinf
        ''' <summary>Forcing function number for hathery stocking (scalar)</summary>
        HatchCode
        ''' <summary>To document</summary>
        FixedFecundity

        ''' <summary>Stanza parameter; used to indicate the group that leads 
        ''' <see cref="eVarNameFlags.Biomass">biomass</see> in a multi-stanza
        ''' configuration.</summary>
        LeadingBiomass
        ''' <summary>Stanza parameter; used to indicate the group that leads 
        ''' <see cref="eVarNameFlags.QBInput">QB</see> in a multi-stanza
        ''' configuration.</summary>
        LeadingCB
        ''' <summary>BaB * Bio</summary>
        Bat
        ''' <summary>Start age of a group in a stanza configuration (in months)</summary>
        StartAge
        ''' <summary>End age of a group in a stanza configuration (in months)</summary>
        EndAge
        ''' <summary>Stanza Consumption over Biomass coefficient.</summary>
        CB
        ''' <summary>A multiplier to change the number of packets for the IBM model.</summary>
        ''' <remarks>..but what about Dell? Acer? Toshiba? This is simply not fair!</remarks>
        PacketsMultiplier

        ''' <summary>Full path of the current datasource/database </summary>
        ModelFileName

        ' ---------------------------------
        ' Ecospace
        ' ---------------------------------
        InRow
        InCol
        CellLength
        ''' <summary>Latitude of spatial data.</summary>
        Latitude
        ''' <summary>Longitude of spatial data.</summary>
        Longitude
        ''' <summary>Basemap stepsize in number of steps per degree.</summary>
        BasemapStepSize
        ''' <summary>Relative catchability per fleet/gear type (multiplier)</summary>
        EffectivePower
        ''' <summary>Base dispersal</summary>
        MVel
        ''' <summary>Relative dispersal in bad habitat</summary>
        RelMoveBad
        ''' <summary>Relative vulnerability in bad habitat</summary>
        RelVulBad
        ''' <summary>Relative feeding in bad habitat</summary>
        EatEffBad
        ''' <summary>To document</summary>
        IsAdvected
        ''' <summary>To document</summary>
        IsMigratory
        ''' <summary>To document</summary>
        MigrationConcRow
        ''' <summary>To document</summary>
        MigrationConcCol

        ''' <summary>To document</summary>
        PreferredCell
        ''' <summary>To document</summary>
        PreferredCol
        ''' <summary>To document</summary>
        PreferredHabitat
        ''' <summary>To document</summary>
        HabitatFishery
        ''' <summary>To document</summary>
        MPAFishery
        ''' <summary>Which months of the year a MPA is open for fishing</summary>
        MPAMonth
        ''' <summary>Ecospace cell depth assignments</summary>
        LayerDepth
        ''' <summary>Ecospace cell habitat assignments</summary>
        LayerHabitat
        ''' <summary>Ecospace cell MPA assignments</summary>
        LayerMPA
        ''' <summary>Ecospace cell RelPP assignments</summary>
        LayerRelPP
        ''' <summary>Ecospace cell RelCin assignments</summary>
        LayerRelCin
        ''' <summary>Ecospace cell region assignments</summary>
        LayerRegion
        ''' <summary>Ecospace cell migration assignments</summary>
        LayerMigration
        ''' <summary>Ecospace cell advection assignments</summary>
        LayerAdvection
        ''' <summary>Ecospace/MPA importance.</summary>
        LayerImportance
        ''' <summary>Ecospace/MPA importance weight of the LayerImportance Variable.</summary>
        ImportanceWeight
        ''' <summary>Proportion of total habitat area by Habitat type.</summary>
        HabAreaProportion
        ''' <summary>Total Eff, Muiltiplier.</summary>
        ''' <remarks>Summary taken from EwE5 code, not overly helpful I'm afraid...</remarks>
        SEmult
        ''' <summary>
        ''' Ecospace: Habitat-adjusted biomass = True. Ecopath base biomass = False
        ''' </summary>
        AdjustSpace

        'jb outputs for ecospace
        'ToDo_jb Ecospace output in eVarNameFlags documentation
        EcospaceMapBiomass

        'Ecospace Group output
        EcospaceGroupBiomassStart
        EcospaceGroupBiomassEnd

        EcospaceGroupCatchStart
        EcospaceGroupCatchEnd

        EcospaceGroupValueStart
        EcospaceGroupValueEnd

        'Ecospace Fleet output
        EcospaceFleetCatchStart
        EcospaceFleetCatchEnd

        EcospaceFleetValueStart
        EcospaceFleetValueEnd

        EcospaceFleetCostStart
        EcospaceFleetCostEnd

        ''' <summary>Biomass of a group in a region for the start summary period </summary>
        EcospaceRegionBiomassStart

        ''' <summary> Biomass of a group in a region for the end summary period</summary>
        EcospaceRegionBiomassEnd

        ''' <summary>Biomass of catch in a region for the start summary period </summary>
        EcospaceRegionCatchStart

        ''' <summary> Biomass of catch in a region for the end summary period</summary>
        EcospaceRegionCatchEnd

        ''' <summary>Time in Years of the Start summary time period </summary>
        EcospaceSummaryTimeStart

        ''' <summary> Time in Years of the End summary time period </summary>
        EcospaceSummaryTimeEnd

        ''' <summary>Number of time steps in the summary periods</summary>
        EcospaceNumberSummaryTimeSteps

        ''' <summary> Ecospace output biomass averaged over all the cells for each timestep</summary>
        EcospaceBiomassOverTime

        ''' <summary> Ecospace [computed biomass] / [base biomass] averaged over all the cells for each timestep </summary>
        EcospaceRelativeBiomassOverTime

        ''' <summary> Ecospace Biomass by region over time averaged over all the cells in a region for each timestep </summary>
        EcospaceRegionBiomass

        ''' <summary>Number of fish in a monthly stanza age group </summary>
        StanzaNumberAtAge

        ''' <summary>Weight of individual fish in a monthly stanza age group </summary>
        StanzaWeightAtAge

        ''' <summary>Biomass in a monthly stanza age group [StanzaNumberAtAge]*[StanzaWeightAtAge]</summary>
        StanzaBiomassAtAge

        ''' <summary>Index to the Ecopath Groups in the Stanza Group </summary>
        StanzaGroup

        ''' <summary>Biomass for this a stanza iStanzaGroup</summary>
        StanzaBiomass

        ''' <summary>Consumption/Biomass for this a stanza iStanzaGroup Ecopath QB</summary>
        StanzaCB


        ''' <summary>Mortality for this a stanza iStanzaGroup Ecopath PB</summary>
        StanzaMortaility

        StanzaVBGF

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Ecospace multi thread vars
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''

        nSolverThreads
        nGroupsPerThread
        nSpaceThreads
        nMapCellsPerThread
        IFDPower
        UseIBM
        UseNewMultiStanza
        ''' <summary>Flag stating whether to use exact calculations or iterations for Ecospace migratory species.</summary>
        UseExact
        ''' <summary>Ecospace run time.</summary>
        TotalTime
        ''' <summary>Number of time steps per year.</summary>
        NumTimeStepsPerYear
        ''' <summary>Ecospace Tolerance.</summary>
        Tolerance
        ''' <summary>Ecospace successive over-relaxation.</summary>
        SOR
        ''' <summary>Ecospace maximum number of iterations.</summary>
        MaxIterations

        ''' <summary>Ecosim ouput data over time</summary>
        EcosimBiomass
        EcosimYield
        EcosimTotalMort
        EcosimConsumpBiomass
        EcosimFeedingTime
        EcosimPredMort
        EcosimFishMort
        EcosimProdConsump
        EcosimAvgWeight
        EcosimAvgPrey
        EcosimAvgPred

        isPred
        isPrey

        ''' <summary> Network analysis variables</summary>
        nTrophicLevels
        NetworkAbsFlow
        NetworkRelFlow

        MixedTrophicImpact

        ''' <summary> Network Flow and Biomss </summary>
        PPImportFlow
        PPConsFlow
        PPExportFlow
        PPToDetFlow
        PPRespFlow
        PPThroughFlow
        DetImportFlow
        DetConsFlow
        DetExportFlow
        DetToDetFlow
        DetRespFlow
        DetThroughFlow

        ''' <summary>Network Acendency </summary>
        AscendGroup
        AscendOverheadGroup
        AscendCapacityGroup
        AscendInfoGroup
        AscendThroughputGroup

        AscendImportTot
        AscendImportPer
        OverheadImportTot
        OverheadImportPer
        CapacityImportTot
        CapacityImportPer

        AscendFlowTot
        AscendFlowPer
        OverheadFlowTot
        OverheadFlowPer
        CapacityFlowTot
        CapacityFlowPer

        AscendExportTot
        AscendExportPer
        OverheadExportTot
        OverheadExportPer
        CapacityExportTot
        CapacityExportPer

        AscendRespTot
        AscendRespPer
        OverheadRespTot
        OverheadRespPer
        CapacityRespTot
        CapacityRespPer

        'Ecosim Group summary output
        EcosimGroupBiomassStart
        EcosimGroupBiomassEnd

        EcosimGroupCatchStart
        EcosimGroupCatchEnd
        EcosimGroupMaxMort

        EcosimGroupValueStart
        EcosimGroupValueEnd

        'Ecosim Fleet output
        EcosimFleetCatchStart
        EcosimFleetCatchEnd

        EcosimFleetValueStart
        EcosimFleetValueEnd

        EcosimFleetCostStart
        EcosimFleetCostEnd
        EcosimFleetEffort

        ' Time series

        ''' <summary>Type of a time series.</summary>
        TimeSeriesType
        ''' <summary>Name of data set that a time series was imported from.</summary>
        DataSet
        ''' <summary>Weight of time for a time series.</summary>
        WtType
        ''' <summary>Index of a group or fleet that a time series applies to.</summary>
        DatPool
        GroupIndex = DatPool
        FleetIndex = DatPool
        ''' <summary>The first year in a time series.</summary>
        DatYear
        ''' <summary>The number of years of a time series.</summary>
        nDatYears
        ''' <summary>Value for a given year in a time series.</summary>
        DatVal
        ''' <summary>Flag stating whether a time series is applied.</summary>
        Applied
        ''' <summary>Average zstat sumof(Log(observed/predicted))/nobs.</summary>
        ''' <remarks>There, you've GOT to love that description.</remarks>
        DataQ
        ''' <summary>Sum of squares fit of this data set to the predicted value.</summary>
        DataSS
        ''' <summary>Future extension: time series can be associated with any variable.</summary>
        CustomVariable

        ''' <summary>Sum of squares fit of Ecospace predicted values to all reference data across all the groups and data.</summary>
        EcospaceSS

        ''' <summary>Sum of squares fit of Ecospace predicted values to reference data for a region.</summary>
        EcospaceRegionSS

        ''' <summary>Sum of squares fit of Ecosim predicted values to all reference data across all the groups and data.</summary>
        EcosimSS

        ''' <summary>Sum of squares fit of Ecosim predicted values to reference data by group.</summary>
        EcosimSSGroup


        ''' <summary>Monte Carlo variables</summary>
        ''' <remarks>variables used by ecosim monte carlo</remarks>
        mcB
        mcPB
        mcQB
        mcBA
        mcEE
        mcVU


        mcBbf
        mcPBbf
        mcQBbf
        mcBAbf
        mcEEbf
        mcVUbf


        mcBLower
        mcPBLower
        mcQBLower
        mcBALower
        mcEELower
        mcVULower

        mcBUpper
        mcPBUpper
        mcQBUpper
        mcBAUpper
        mcEEUpper
        mcVUUpper

        mcBcv
        mcPBcv
        mcQBcv
        mcBAcv
        mcEEcv
        mcVUcv


        'end monte carlo variables
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        BarrierAvoidanceWeight

        ''' <summary>
        ''' Fishing Policy Search varaibles
        ''' </summary>
        ''' 

        SearchBlock 'codeblock in EwE5

        ' Generic search parameters
        SearchDiscountRate
        SearchGenDiscRate
        SearchBaseYear
        SearchFishingMortalityPenalty

        FPSValueComponentType

        FPSGroupStrucRelWeight

        FPSFleetJobCatchValue
        FPSFleetTargetProfit

        'Model Parameters
        FPSNRuns 'number of runs
        FPSGroupMandRelBiom
        FPSMaxNumEval
        FPSMaxEffChange
        FPSInitOption
        FPSSearchOption
        FPSOptimizeApproach
        FPSOptimizeOptions

        FPSEconomicWeight
        FPSSocialWeight
        FPSMandatedRebuildingWeight
        FPSEcoSystemWeight

        FPSMaxPortUtil
        SearchPrevCostEarning
        FPSIncludeComp
        'UseEcospace and BatchRun have not been implemented yet
        FPSBatchRun
        FPSUseEcospace
        FPSFishingLimit
        FPSPredictionVariance
        FPSExistenceValue

        ' Fit to time series
        F2TSVulnerabilitySearch
        F2TSAnomalySearch
        F2TSCatchAnomaly
        F2TSCatchAnomalySearchShapeNumber
        F2TSFirstYear
        F2TSLastYear
        F2TSNumSplinePoints
        ''' <summary>Weights of applied TS in search algorithm.</summary>
        F2TSAppliedWeights
        ''' <summary>Variance of Vulnerability.</summary>
        F2TSVulnerabilityVariance
        ''' <summary>Variance of Primary Production.</summary>
        F2TSPPVariance

        ' Ecotracer
        CZero
        CInflow
        COutflow
        CDecay
        ConForceNumber
        CImmig
        CEnvironment
        CBEnvironment
        CExcretionRate
        Concentration
        ConcBio
        CSum

        'MPA Optimization EcoSeed RandomSearch
        MPAOptEconomicValue
        MPAOptSocialValue
        MPAOptMandatedValue
        MPAOptEcologicalValue
        MPAOptBestRow
        MPAOptBestCol
        MPAOptCurRow
        MPAOptCurCol

        MPAOptBoundaryWeight
        MPAOptSearchType

        MPAOptStepSize
        MPAOptIterations
        MPAOptMaxArea
        MPAOptMinArea
        iMPAOptToUse
        MPAbUseCellWeight

        MPAOptStartYear
        MPAOptEndYear

        MPAOptPercentageClosed
        MPAOptTotalValue

        ''' <summary>Ecospace cell MPA seed assignments</summary>
        LayerMPASeed
        LayerMPASeedCurrent
        LayerMPASeedBest
        ''' <summary>Ecospace cell MPA Random assignments</summary>
        LayerMPARandom
        ''' <summary>MSE coefficient of variation for biomass</summary>
        MSEBioCV
        ''' <summary>MSE coefficient of variation for fishing fleets</summary>
        MSEFleetCV
        ''' <summary>MSE increase in catchability by group per year (multiplier)</summary>
        MSEQIncrease
        ''' <summary>MSE importance weight in assuming impact of fleet on a group (multiplier)</summary>
        MSEFleetWeight
        ''' <summary>Lower biomass bounds for risk analysis</summary>
        MSELowerRisk
        ''' <summary>Upper biomass bounds for risk analysis</summary>
        MSEUpperRisk
        ''' <summary>Number of trial that exceeded the lower biomass bounds for risk analysis</summary>
        MSELowerRiskCount
        ''' <summary>Number of trial that exceeded the upper biomass bounds for risk analysis</summary>
        MSEUpperRiskCount

        ''' <summary> Sum of all economic values for the current MSE output object (results)</summary>
        MSETotalValue
        ''' <summary> Sum of employment values for the current MSE output object (results)</summary>
        MSEEmployValue
        MSEMandatedValue
        ''' <summary> Sum of biomass for the current MSE output object (results)</summary>
        MSEEcologicalValue

        MSEMeanTotalValue
        MSEMeanEmployValue
        MSEMeanMandatedValue
        MSEMeanEcologicalValue

        MSEBestTotalValue

        ''' <summary> Trial number for the current MSE output object (results)</summary>
        MSETrialNumber

        MSEAssessMethod
        MSEKalmanGain
        MSEForcastGain
        MSEAssessPower
        MSENTrials

        ' Pedigree
        VariableName
        IndexValue
        ConfidenceInterval

        'Varnames added for Game Server
        ''' <summary> User entered fishing rate modifiers/shapes for fleets</summary>
        GameFleetFishingRates
        ''' <summary> User entered fishing rate modifiers/shapes for groups</summary>
        GameGroupFishingRates

        ' EcosimResults
        GameSimulationTimeStep
        GameModel
        GameBiomass
        GameBiomassByRegion

        ''' <summary>For Ecosim Yield from Ecosim Plots Biomass * FishTime </summary>
        GameYield

        GameState
        ''' <summary>Fleet name added for the Game data because EwE6 uses Name for all names</summary>
        FleetName


    End Enum



#End Region ' Variable names

#Region " Data Types "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type that indicates a class of data in the EwE core.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eDataTypes
        ''' <summary>
        ''' Data type is not specified.
        ''' </summary>
        NotSet

        ''' <summary>
        ''' Data belongs to the EwE model.
        ''' </summary>
        EwEModel

        ''' <summary>
        ''' Data belongs to the Ecopath group inputs,
        ''' which are provided to perform a parameter estimation run. 
        ''' </summary>
        EcoPathGroupInput

        ''' <summary>
        ''' Data belongs to the Ecopath group outputs,
        ''' which are produced by a parameter estimation run.
        ''' </summary>
        EcoPathGroupOutput

        ''' <summary>
        ''' Data belongs to the Ecopath fleet inputs,
        ''' which are provided for a parameter estimation run.
        ''' </summary>
        FleetInput

        '''' <summary>
        '''' Data belongs to the Ecopath fleet outputs,
        '''' which are produced by a parameter estimation run.
        '''' </summary>
        'FleetOutput

        ''' <summary>
        ''' Data belongs to an Ecosim scenario.
        ''' </summary>
        EcoSimScenario

        ''' <summary>
        ''' Data belongs to the Ecosim model parameters,
        ''' which instruct how to run an Ecosim scenario.
        ''' </summary>
        EcoSimModelParameter

        ''' <summary>
        ''' Data belongs to an Ecosim group input.
        ''' </summary>
        EcoSimGroupInput

        ''' <summary>
        ''' Data belongs to a Time Forcing Function.
        ''' </summary>
        Forcing

        ''' <summary>
        ''' Data belongs to an Egg Production Forcing Function.
        ''' </summary>
        EggProd

        ''' <summary>
        ''' Data belongs to an Mediation Function.
        ''' </summary>
        Mediation

        ''' <summary>
        ''' Data belongs to an Fishing Rate shape.
        ''' </summary>
        FishingRate

        ''' <summary>
        ''' Data belongs to an Fishing Mortality shape.
        ''' </summary>
        FishMort

        ''' <summary>
        ''' Data belongs to an EwE multi-stanza configuration.
        ''' </summary>
        Stanza 'jb June-14-06 added for Stanza data types

        ''' <summary>
        ''' Data belongs to an Ecospace scenario.
        ''' </summary>
        EcoSpaceScenario

        ''' <summary>
        ''' Data belongs to an Ecospace habitat.
        ''' </summary>
        EcospaceHabitat

        ''' <summary>
        ''' Data belongs to an Ecospace region.
        ''' </summary>
        EcospaceRegion

        ''' <summary>
        ''' Data belongs to an Ecospace group.
        ''' </summary>
        EcospaceGroup

        ''' <summary>
        ''' Data belongs to an Ecospace fleet.
        ''' </summary>
        EcospaceFleet

        ''' <summary>
        ''' Data belongs to an Ecospace MPA.
        ''' </summary>
        EcospaceMPA

        ''' <summary>
        ''' Data belongs to the Ecospace model parameters,
        ''' which instruct how to run an Ecopace scenario.
        ''' </summary>
        EcospaceModelParameter

        ''' <summary>
        ''' Data belongs to a cEcospaceModelBasemaps instance.
        ''' </summary>
        EcospaceBasemap

        ''' <summary>
        ''' Data belongs to a cEcospaceModelBasemapLayer instance.
        ''' </summary>
        EcospaceBasemapLayer

        ''' <summary>
        ''' Data belongs to a ecospace importance layer instance.
        ''' </summary>
        EcospaceImportanceLayer

        ''' <summary>
        ''' cPredPreyInteraction object
        ''' </summary>
        PredPreyInteraction

        ''' <summary>
        ''' Time step results the currently running for Ecospace model
        ''' </summary>
        EcospaceTimestepResults

        ''' <summary>
        ''' 
        ''' </summary>
        EcospaceBiomassResults

        EcospaceRegionResults

        ''' <summary>
        ''' 
        ''' </summary>
        NetworkFlowOutput

        ''' <summary>
        ''' Data belongs to a cGroupTimeSeries instance.
        ''' </summary>
        GroupTimeSeries

        ''' <summary>
        ''' Data belongs to a cFleetTimeSeries instance.
        ''' </summary>
        FleetTimeSeries

        ''' <summary>
        ''' Data belongs to a Time Series Dataset instance.
        ''' </summary>
        TimeSeriesDataset

        ''' <summary>
        ''' Ecosim Monte Carlo
        ''' </summary>
        ''' <remarks></remarks>
        MonteCarlo

        ''' <summary>
        ''' Data belongs to an Ecosim group output.
        ''' </summary>
        EcoSimGroupOutput
        EcosimFleetOutput
        EcosimFleetSummary
        EcosimGroupSummary


        FitToTimeSeries

        ''' <summary>
        ''' Data belongs to an Ecotracer scenario
        ''' </summary>
        EcotracerScenario

        ''' <summary>
        ''' Data belongs to Ecotracer model parameters
        ''' </summary>
        EcotracerModelParameters

        ''' <summary>
        ''' Data belongs to an Ecotracer input group
        ''' </summary>
        EcotracerGroupInput
        EcotracerSimOutput
        EcotracerSpaceOutput

        ''' <summary>Search Objectives </summary>
        '''<remarks>Search Objectives form the base for the shared search interface ISearchObjective used by Fishing Policy, Ecoseed and MSE </remarks>
        SearchObjectiveManager
        SearchObjectiveParameters
        SearchObjectiveFleetInput
        SearchObjectiveWeights
        SearchObjectiveGroupInput

        ''' <summary> Fishing Policy (implements ISearchObjective) </summary>
        ''' <remarks>The Fishing Policy uses SearchObjectivexxxx data types as well </remarks>
        FishingPolicyManager
        FishingPolicyParameters
        FishingPolicySearchBlocks

        ''' <summary> Ecoseed manager (implements ISearchObjective)  </summary>
        MPAOptManager
        'EcoSeedInput
        MPAOptOuput
        MPAOptParameters

        ''' <summary> Management Strategy Evaluator (implements ISearchObjective)  </summary>
        MSEManager
        MSEFleetInput
        MSEGroupInput
        MSEOutput
        MSEParameters

        ''' <summary>Pedigree</summary>
        PedigreeLevel

        ''' <summary>Data types for the Game</summary>    
        GameData

        ''' <summary>Data types for Ecosim fisheries regulation</summary>    
        EcosimFisheriesRegulation

    End Enum

#End Region ' Data Types

#Region " Core Counters "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated types indicating the EwE counters that define data structure
    ''' sizes in the various models.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eCoreCounterTypes

        ''' <summary>Unspecified counter.</summary>
        NotSet = 0
        ''' <summary>Number of groups across all models.</summary>
        nGroups
        ''' <summary>Number of detritus groups across all models.</summary>
        nDetritus
        ''' <summary>Number of living groups across all models.</summary>
        nLivingGroups
        ''' <summary>Number of fishing fleets across all models.</summary>
        nFleets
        ''' <summary>Max number of groups in a single stanza configuration over all stanza groups.</summary>
        nMaxStanza
        ''' <summary>Max age for a stanza group.</summary>
        ''' <remarks>Age2(iStanza, m_Stanza.Nstanza(iStanza))</remarks>
        nMaxStanzaAge
        ''' <summary>Number of stanza configuratons.</summary>
        nStanzas
        ''' <summary>Number of stanzas for a stanza group.</summary>
        ''' <remarks>Nstanza(iStanza)</remarks>
        nStanzasForStanzaGroup
        ''' <summary>Number of years to run an Ecosim model.</summary>
        nEcosimYears
        ''' <summary>Number of time steps in an Ecosim run.</summary>
        nEcosimTimeSteps
        ''' <summary>Number of years to run an Ecospace model.</summary>
        nEcospaceYears
        ''' <summary>Number time steps in an Ecospace model.</summary>
        nEcospaceTimeSteps
        ''' <summary>Number of Ecospace habitats.</summary>
        nHabitats
        ''' <summary>Number of Ecospace regions.</summary>
        nRegions
        ''' <summary>Number of months per year.</summary>
        ''' <remarks>Albeit quite obvious and constant, this value is added to facilitate automatic array resizing.</remarks>
        nMonths
        ''' <summary>Number of Ecospace MPAs.</summary>
        nMPAs
        ''' <summary>Number of trophic levels from the Network analysis</summary>
        nTrophicLevels
        ''' <summary>Number of available time series.</summary>
        nTimeSeries
        ''' <summary>Number of applied time series.</summary>
        nTimeSeriesApplied
        ''' <summary>Max number of years over all time series.</summary>
        nTimeSeriesYears
        ''' <summary>Number of time series datasets.</summary>
        nTimeSeriesDatasets
        ''' <summary>Number of importance layers.</summary>
        nImportanceLayers


        ''' <summary>Number of years the game simulation can run for.</summary>
        nGameSimYears
        ''' <summary>Number of timesteps the game simulation can run for.</summary>
        nGameSimTimeSteps
        ''' <summary>Number of timesteps per year.</summary>
        nGameSimTimeStepsPerYear


    End Enum

#End Region ' Core counters

#Region " System units "

    ''' <summary>
    ''' Order is important!
    ''' </summary>
    Public Enum eUnitCurrencyType As Integer
        NotSet = 0
        Joules = 1
        Calorie = 2
        Carbon = 3
        DryWeight = 4
        WetWeight = 5
        CustomEnergy = 6
        Nitrogen = 7
        Phosporous = 8
        CustomNutrient = 9
    End Enum

    Public Enum eUnitTimeType As Integer
        Custom = 0
        Year
        Day
    End Enum

    Public Enum eUnitMonetaryType As Integer
        Custom = 0
        AED '   UAE Dirham
        AFN ' 	Afghanistan Afghani
        ALL ' 	Albanian Lek
        AMD ' 	Armenian Dram
        ANG ' 	Antillean Guilder
        AOR ' 	Angolan New Kwanza
        ARS ' 	Argentinian Peso
        AUD ' 	Australian Dollar
        AWG ' 	Aruban Florin
        AZM ' 	Azerbaijan Manat
        BAM ' 	Bosnian Konvertibilna Marka
        BBD ' 	Barbadian Dollar
        BDT ' 	Bangladesh Taka
        BGN ' 	Bulgarian New Lev
        BHD ' 	Bahraini Dinar
        BIF ' 	Burundian Franc
        BMD ' 	Bermudan Dollar
        BND ' 	Brunei Dollar
        BOB ' 	Bolivian Boliviano
        BRL ' 	Brazilian Real
        BSD ' 	Bahamas Dollar
        BTN ' 	Bhutan Ngultrum
        BWP ' 	Botswana Pula
        BYB ' 	Belarussian Ruble
        BZD ' 	Belizean Dollar
        CAD ' 	Canadian Dollar
        CDF ' 	Congolese Franc
        CHF ' 	Swiss Franc
        CLP ' 	Chilean Peso
        CNY ' 	Chinese Yuan Renminbi
        COP ' 	Colombian Peso
        CRC ' 	Costa Rican Colon
        CUP ' 	Cuban Peso
        CVE ' 	Cape Verdean Escudo
        CZK ' 	Czech Koruna
        DJF ' 	Djiboutian Franc
        DKK ' 	Danish Krone
        DOP ' 	Dominican Republic Peso
        DZD ' 	Algerian Dinar
        ECS ' 	Ecuador Sucre
        EEK ' 	Estonian Kroon
        EGP ' 	Eqyptian Pound
        ETB ' 	Ethiopian Birr
        EUR ' 	Euro
        FJD ' 	Fijian Dollar
        FKP ' 	Falkland Islands Pound
        FRF ' 	French Franc
        GBP ' 	UK Pound Sterling
        GEL ' 	Georgian Lari
        GHC ' 	Ghana Cedi
        GIP ' 	Gibraltarian Pound
        GMD ' 	Gambian Dalasi
        GNF ' 	Guinean Franc
        GTQ ' 	Guatemalan Quetzal
        GYD ' 	Guyanese Dollar
        HKD ' 	Hong Kong Dollar
        HNL ' 	Honduran Lempira
        HRK ' 	Croatian Kuna
        HTG ' 	Haitian Gourde
        HUF ' 	Hungarian Forint
        IDR ' 	Indonesian Rupiah
        ILS ' 	Israeli New Sheqel
        INR ' 	Indian Rupee
        IQD ' 	Iraqi Dinar
        IRR ' 	Iranian Rial
        ISK ' 	Iceland Krona
        JMD ' 	Jamaican Dollar
        JOD ' 	Jordanian Dinar
        JPY ' 	Japanese Yen
        KES ' 	Kenyan Shilling
        KGS ' 	Kyrgyzstan Som
        KHR ' 	Cambodian Riel
        KMF ' 	Comoran Franc
        KPW ' 	Korean PR Won (N.Korea)
        KRW ' 	Korean Republic Won (S.Korea)
        KWD ' 	Kuwaiti Dinar
        KYD ' 	Caymanian Dollar
        KZT ' 	Kazakhstan Tenge
        LAK ' 	Laos Kip
        LBP ' 	Lebanese Pound
        LKR ' 	Sri Lanka Rupee
        LRD ' 	Liberian Dollar
        LSL ' 	Lesothian Loti
        LTL ' 	Lithuanian Litas
        LVL ' 	Latvian Lat
        LYD ' 	Libyan Dinar
        MAD ' 	Moroccan Dirham
        MDL ' 	Moldovan Leu
        MGF ' 	Malagasy Franc
        MKD ' 	Macedonian Denar
        MMK ' 	Myanmar Kyat
        MNT ' 	Mongolian Tugrik
        MOP ' 	Macau Pataca
        MRO ' 	Mauritanian Ougiya
        MUR ' 	Mauritian Rupee
        MVR ' 	Maldivian Rufiyaa
        MWK ' 	Malawi Kwacha
        MXN ' 	Mexican New Peso
        MYR ' 	Malaysian Ringgit
        MZM ' 	Mozambique Metical
        NAD ' 	Namibia Dollar
        NGN ' 	Nigerian Naira
        NIO ' 	Nicaraguan Crdoba
        NOK ' 	Norwegian Krone
        NPR ' 	Nepalese Rupee
        NZD ' 	New Zealand Dollar
        OMR ' 	Omani Rial
        PAB ' 	Panamanian Balboa
        PEN ' 	Peruvian New Sol
        PGK ' 	Papua New Guinea Kina
        PHP ' 	Philippine Piso
        PKR ' 	Pakistani Rupee
        PLN ' 	Polish New Zloty
        PYG ' 	Paraguayan Guaran
        QAR ' 	Qatari Riyal
        ROL ' 	Romanian Leu
        RSD ' 	Serbian Dinar
        RUB ' 	Russian Rouble
        RWF ' 	Rwandan Franc
        SAR ' 	Saudi Riyal
        SBD ' 	Solomon Islands Dollar
        SCR ' 	Seychelles Rupee
        SDD ' 	Sudanese Dinar
        SEK ' 	Swedish Krona
        SGD ' 	Singaporean Dollar
        SHP ' 	Saint Helena Pound
        SKK ' 	Slovak Koruna
        SLL ' 	Sierra Leone Leone
        SOS ' 	Somali Shilling
        SRG ' 	Surinamese Guilder
        STD ' 	Sáo Tome & Principe Dobra
        SVC ' 	Salvadoran Colon
        SYP ' 	Syrian Pound
        SZL ' 	Swaziland Lilangeni
        THB ' 	Thai Baht
        TJR ' 	Tajikistan Ruble
        TMM ' 	Turkmenistan Manat
        TND ' 	Tunisian Dinar
        TOP ' 	Tonga Pa'anga
        [TRY] ' New Turkish Lira
        TTD ' 	Trinidadian Dollar
        TWD ' 	Taiwanese Yuan
        TZS ' 	Tanzanian Shilling
        UAH ' 	Ukraine Hryvnias
        UGX ' 	Uganda New Shilling
        USD ' 	U.S. Dollar
        UYU ' 	Peso Uruguayo
        UZS ' 	Uzbekistan Sum
        VEB ' 	Venezuelan Bolvar
        VND ' 	Vietnamese New Dng
        VUV ' 	Vanuatu Vatu
        WST ' 	Western Samoan Tala
        XAF ' 	CFA Central Franc
        XCD ' 	East Caribbean Dollar
        XOF ' 	CFA West Franc
        XPF ' 	French Polynesian CFA France (CFP)
        YER ' 	Yemen Rial
        ZAR ' 	South African Rand
        ZMK ' 	Zambian Kwacha
        ZWD ' 	Zimbabwean Dollar

    End Enum

#End Region ' System units

#Region "Quota types"

    'enum values are hard coded so that they can be stored in the database 
    Public Enum eQuotaTypes
        NotUsed = 0
        Weakest = 1
        Strongest = 2
        Selective = 3
    End Enum

#End Region

End Namespace ' Core
