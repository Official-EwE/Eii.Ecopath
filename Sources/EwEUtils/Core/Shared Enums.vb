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
        ''' <summary>Ecopath model data has been initialized.</summary>
        EcopathInitialized = EcopathLoaded
        ''' <summary>Ecopath scenario is ready to run.</summary>
        EcopathRunning
        ''' <summary>Ecopath model run is completed.</summary>
        EcopathCompleted
        ''' <summary>Ecopath PSD model run is completed.</summary>
        PSDCompleted
        ''' <summary>Ecotracer scenario data has been loaded.</summary>
        EcotracerLoaded
        ''' <summary>Ecosim scenario data has been loaded.</summary>
        EcosimLoaded
        ''' <summary>Ecosim scenario has been initialized.</summary>
        EcosimInitialized
        ''' <summary>Ecosim scenario is running.</summary>
        EcosimRunning
        ''' <summary>Ecosim scenario run is completed.</summary>
        EcosimCompleted
        ''' <summary>Ecospace scenario data has been loaded.</summary>
        EcospaceLoaded
        ''' <summary>Ecospace scenario has been initialized.</summary>
        EcospaceInitialized = EcospaceLoaded
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
    Public Enum eVarNameFlags As Integer

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
        ''' <summary>[Fishing Mort] / [Total mort]</summary>
        FishMortTotMort
        ''' <summary>1- FishMortTotMort</summary>
        NatMortPerTotMort

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

        'added by JS for Ecopath statistics info Jan-29-09
        ''' <summary>Sum of all consumption.</summary>
        EcopathStatsTotalConsumption
        ''' <summary>Sum of all exports.</summary>
        EcopathStatsTotalExports
        ''' <summary>Sum of all respiratory flows.</summary>
        EcopathStatsTotalRespFlow
        ''' <summary>Sum of all flows into detritus.</summary>
        EcopathStatsTotalFlowDetritus
        ''' <summary>Total system throughput.</summary>
        EcopathStatsTotalThroughput
        ''' <summary>Sum of all production.</summary>
        EcopathStatsTotalProduction
        ''' <summary>Mean trophic level of the catch.</summary>
        EcopathStatsMeanTrophicLevelCatch
        ''' <summary>Gross efficiency (catch/net p.p.).</summary>
        EcopathStatsGrossEfficiency
        ''' <summary>Calculated total net primary production.</summary>
        EcopathStatsTotalNetPP
        ''' <summary>Total primary production/total respiration.</summary>
        EcopathStatsTotalPResp
        ''' <summary>Net system production.</summary>
        EcopathStatsNetSystemProduction
        ''' <summary>Total primary production/total biomass.</summary>
        EcopathStatsTotalPB
        ''' <summary>Total biomass/total throughput.</summary>
        EcopathStatsTotalBT
        ''' <summary>Total biomass (excluding detritus).</summary>
        EcopathStatsTotalBNonDet
        ''' <summary>Total catches.</summary>
        EcopathStatsTotalCatch
        ''' <summary>Connectance Index.</summary>
        EcopathStatsConnectanceIndex
        ''' <summary>System Omnivory Index.</summary>
        EcopathStatsOmnivIndex
        ''' <summary>Total market value.</summary>
        EcopathStatsTotalMarketValue
        ''' <summary>Total shadow value.</summary>
        EcopathStatsTotalShadowValue
        ''' <summary>Total value.</summary>
        EcopathStatsTotalValue
        ''' <summary>Total fixed cost.</summary>
        EcopathStatsTotalFixedCost
        ''' <summary>Total variable cost.</summary>
        ''' <remarks>This variable may exist under a different name.</remarks>
        EcopathStatsTotalVarCost
        ''' <summary>Total cost.</summary>
        EcopathStatsTotalCost
        ''' <summary>Profit.</summary>
        EcopathStatsProfit
        ''' <summary>Pedigree.</summary>
        EcopathStatsPedigree
        ''' <summary>Measure of pedigree fit.</summary>
        EcopathStatsMeasureOfFit

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
        QuotaShare
        ''' <summary>Proportion of discards that dies</summary>
        DiscardMortality

        '' Target fishing mortality policy vars
        '''' <summary>Quota for a species.</summary>
        'QuotaSpecies
        ''' <summary>BBase for target fishing mortality policy. Upper biomass boundary.</summary>
        MSEBBase
        ''' <summary>BLimit for target fishing mortality policy. Lower biomass boundary.</summary>
        MSEBLim
        ''' <summary>Mortality/Fmsy for target fishing mortality policy.</summary>
        MSEFmax
        ''' <summary>Mortality when biomass at or below BLim(lower boundry)</summary>
        ''' <remarks>Added for MSEBatch command file.</remarks>
        MSEFmin

        MSEFmaxPM

        'jb Salinity values added Dec-07
        SalinityForceFunctionNumber
        SalinityOpt
        ' SalinitySpread
        SalinitySpreadLeft
        SalinitySpreadRight

        'jb Temerature values added Nov-10
        TemperatureForceFunctionNumber
        TemperatureOpt
        ' SalinitySpread
        TemperatureSpreadLeft
        TemperatureSpreadRight

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
        ''' <summary>Group display digits.</summary>
        GroupDigits
        ''' <summary>Unit enumerated value for text-based values.</summary>
        UnitTime
        ''' <summary>Unit text for time-based values.</summary>
        UnitTimeCustomText
        ''' <summary>Unit enumerated value for area-based values.</summary>
        UnitArea
        ''' <summary>Unit text for area-based values.</summary>
        UnitAreaCustomText
        ''' <summary>Unit enumerated value for currency-based values.</summary>
        UnitCurrency
        ''' <summary>Unit text for currency-based values.</summary>
        UnitCurrencyCustomText
        ''' <summary>Unit enumerated value for monetary values.</summary>
        UnitMonetary
        ''' <summary>Unit text for monetary values.</summary>
        UnitMonetaryCustomText
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
        LayerMPAPM
        ''' <summary>Ecospace cell relative primary production</summary>
        LayerRelPP
        ''' <summary>Ecospace cell relative level of contaminants</summary>
        LayerRelCin
        ''' <summary>Ecospace cell region assignments</summary>
        LayerRegion
        ''' <summary>Ecospace cell migration assignments</summary>
        LayerMigration
        ''' <summary>Ecospace cell advection assignments</summary>
        LayerAdvection
        ''' <summary>Ecospace transport rate layer.</summary>
        LayerTransportRate
        ''' <summary>Ecospace MLD (mixed layer depths) layer.</summary>
        LayerMLD
        ''' <summary>Ecospace wind layer.</summary>
        LayerWind
        ''' <summary>Ecospace upwelling layer.</summary>
        LayerUpwelling
        ''' <summary>Ecospace MPA importance.</summary>
        LayerImportance
        ''' <summary>Ecospace cell port assignments.</summary>
        LayerPort
        ''' <summary>Ecospace sailing cost.</summary>
        LayerSail
        ''' <summary>Ecospace distribution envelope.</summary>
        LayerDistribution
        ''' <summary>Ecospace/MPA importance weight of the weight layer.</summary>
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
        ''' <summary>Ecospace [Effort End] / [Effort Start] </summary>
        EcospaceFleetEffortES

        ''' <summary>Ecospace Catch by Fleet Time </summary>
        EcospaceFleetCatch
        ''' <summary>Ecospace Value by Fleet Time </summary>
        EcospaceFleetValue

        ''' <summary>Biomass of a group in a region for the start summary period </summary>
        EcospaceRegionBiomassStart

        ''' <summary> Biomass of a group in a region for the end summary period</summary>
        EcospaceRegionBiomassEnd

        ''' <summary>Biomass of catch in a region for the start summary period </summary>
        EcospaceRegionCatchStart

        ''' <summary> Biomass of catch in a region for the end summary period</summary>
        EcospaceRegionCatchEnd

        ''' <summary> Biomass of catch in a region by fleet, group and time </summary>
        EcospaceRegionFleetGroupCatch

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

        ''' <summary> Ecospace Catch over time </summary>
        EcospaceGroupCatchOverTime


        ''' <summary> Ecospace Value over time </summary>
        EcospaceGroupValueOverTime

        ''' <summary> Ecospace Biomass by region over time averaged over all the cells in a region for each timestep </summary>
        EcospaceRegionBiomass

        ''' <summary> Ecospace yearly average profit by fleet  </summary>
        EcospaceFleetProfit

        ''' <summary> Ecospace yearly average jobs [value of catch] * [jobs]  </summary>
        EcospaceFleetJobs

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

        'StanzaVBGF

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

        ''''''''''''''''''''''''''''
        ' Ecosim ouput data over time
        '''''''''''''''''''''
        ''' <summary>Ecosim absolute biomass over time</summary>
        EcosimBiomass
        ''' <summary>Ecosim relative biomass over time</summary>
        EcosimBiomassRel
        EcosimYield
        ''' <summary>[catch(t)]/[catch(0)]</summary>
        EcosimYieldRel

        EcosimCatchGroupGear
        ''' <summary> Fishing mortality by group fleet </summary>
        EcosimFishingMortGroupGear

        EcosimTotalMort
        EcosimConsumpBiomass
        EcosimFeedingTime
        EcosimPredMort
        EcosimFishMort
        EcosimProdConsump
        EcosimAvgWeight
        EcosimAvgPrey
        EcosimAvgPred

        ''' <summary>[predation mortality]/[total mortality]</summary>
        EcosimMortVPred
        ''' <summary>[fishing mortality]/[total mortality]</summary>
        EcosimMortVFishing
        EcosimMortVPredPM
        EcosimMortVFishingPM

        EcosimEcoSystemStruct

        'Joeh
        ''' <summary>Ecopath ouput data over time</summary>
        EcopathWeight
        EcopathNumber
        EcopathBiomass
        LorenzenMortality

        ''' <summary>Particle size distribution</summary>
        PSD
        'End Joeh

        ''' <summary>Consumption by Pred of this Prey over time </summary>
        EcosimPredConsumpTime

        ''' <summary>Consumption Rate by Pred of this Prey over time (consumpt(prey,pred)/b(prey)) over time</summary>
        EcosimPredRateTime

        EcosimElectivityTime

        ''' <summary>Percentage of a group this group consumes over time</summary>
        EcosimPreyPercentageTime


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
        EcosimFleetJobs
        EcosimFleetProfit
        EcosimFleetValueTime
        EcosimFleetCatchTime

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

        'isEconomicAvailable

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
        FPSBiomassDiversityWeight

        FPSMaxPortUtil
        SearchPrevCostEarning
        FPSIncludeComp
        'UseEcospace and BatchRun have not been implemented yet
        FPSBatchRun
        FPSUseEcospace
        FPSFishingLimit
        FPSPredictionVariance
        FPSExistenceValue
        FPSUseEconomicPlugin

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

        ''' <summary>Number of data points for AIC  </summary>
        F2TSNAICData
        ''' <summary>Use default V's instead of currect V's</summary>
        F2TSUseDefaultV

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
        MPAOptBiomassDiversityValue
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
        MPAOptAreaBoundary

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
        MSELowerRiskPercent
        ''' <summary>Number of trial that exceeded the upper biomass bounds for risk analysis</summary>
        MSEUpperRiskPercent

        ''' <summary> Sum of all economic values for the current MSE output object (results)</summary>
        MSEEconomicValue
        ''' <summary> Sum of employment values for the current MSE output object (results)</summary>
        MSEEmployValue
        MSEMandatedValue
        ''' <summary> Sum of biomass for the current MSE output object (results)</summary>
        MSEEcologicalValue

        ''' <summary> Weighted sum of all mean values</summary>
        MSEWeightedTotalValue
        MSEMeanEconomicValue
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
        MSEUseEconomicPlugin
        RHalfB0Ratio
        MSEFixedF
        MSERecruitmentCV
        ''' <summary>Total allowable catch </summary>
        MSETAC

        'data by iteration
        MSEBiomass
        MSECatchByGroup
        MSECatchByFleet
        MSEValueByFleet
        MSEEffort

        ''' <summary>True = Use predicted Effort False = user input Effort </summary>
        MSEPredictEffort
        ''' <summary>Biomass by group </summary>
        MSEFixedEscapement

        ''' <summary>Stop the current MSE run</summary>
        MSEStop

        ''' <summary>Save the output</summary>
        MSESave

        ''' <summary>Effort type the MSE is to use.</summary>
        MSEEffortSource

        ''' <summary>
        ''' Regulatory type use be MSE
        ''' </summary>
        MSERegulatoryMode

        MSERefBioLower
        MSERefBioUpper

        MSERefBioEstLower
        MSERefBioEstUpper

        MSERefGroupCatchLower
        MSERefGroupCatchUpper

        MSERefFleetCatchLower
        MSERefFleetCatchUpper

        MSERefFleetEffortLower
        MSERefFleetEffortUpper

        'MSE Stats
        MSEBiomassHistogram
        MSEBiomassMeanValues
        MSEBiomassMin
        MSEBiomassMax
        MSEBiomassCV
        MSEBiomassSdt
        MSEBiomassBins
        MSEBiomassBinWidths
        MSEBiomassValues
        MSEBiomassAboveLimit
        MSEBiomassBelowLimit

        MSEBiomassAboveLimitPM
        MSEBiomassBelowLimitPM
        MSEBiomassCVPM


        MSEGroupCatchHistogram
        MSEGroupCatchMeanValues
        MSEGroupCatchMin
        MSEGroupCatchMax
        MSEGroupCatchCV
        MSEGroupCatchStd
        MSEGroupCatchBins
        MSEGroupCatchBinWidths
        MSEGroupCatchValues
        MSEGroupCatchAboveLimit
        MSEGroupCatchBelowLimit

        MSEFleetValueHistogram
        MSEFleetValueMeanValues
        MSEFleetValueMin
        MSEFleetValueMax
        MSEFleetValueCV
        MSEFleetValueStd
        MSEFleetValueBins
        MSEFleetValueBinWidths
        MSEFleetValueValues
        MSEFleetValueAboveLimit
        MSEFleetValueBelowLimit

        MSEEffortHistogram
        MSEEffortMeanValues
        MSEEffortMin
        MSEEffortMax
        MSEEffortCV
        MSEEffortStd
        MSEEffortBins
        MSEEffortBinWidths
        MSEEffortValues
        MSEEffortAboveLimit
        MSEEffortBelowLimit

        MSEBioEstHistogram
        MSEBioEstMeanValues
        MSEBioEstMin
        MSEBioEstMax
        MSEBioEstCV
        MSEBioEstStd
        MSEBioEstBins
        MSEBioEstBinWidths
        MSEBioEstValues
        MSEBioEstAboveLimit
        MSEBioEstBelowLimit

        MSEStartYear
        MSEResultsStartYear
        MSEResultsEndYear

        MSYRunSilent
        MSYEvalValue
        MSYStartTime
        MSYEvaluateFleet

        ' Pedigree
        VariableName
        IndexValue
        ConfidenceInterval
        Pedigree

        'Varnames added for Game Server

        ''' <summary>Game server loaded model.</summary>
        GameModel
        ''' <summary>Game server run state.</summary>
        GameState
        ''' <summary>Game client moderator state.</summary>
        GameModeratorState
        ''' <summary>Items the client is allowed to show.</summary>
        GameViewVisibleItems
        ''' <summary>Items the client can request from the server.</summary>
        GameViewAvailableItems
        ''' <summary>Limits imposed on variables.</summary>
        GameDataLimits
        ''' <summary> User entered fishing rate modifiers/shapes for fleets</summary>
        GameFleetFishingRates
        ''' <summary> User entered mortality/fishing rate modifiers/shapes for groups</summary>
        GameGroupFishingMortRates
        ''' <summary>Traffic lights the client can request from the server.</summary>
        GameViewTrafficLights

        ''' <summary>Type of data available during a simulation (TimeStep or Progress)</summary>
        GameAvailableRunData

        ' EcosimResults
        GameSimulationTimeStep
        ''' <summary>Game absolute biomass.</summary>
        GameBiomass
        GameBiomassPM
        ''' <summary>Game generic relative biomass over time (no specific source)</summary>
        GameBiomassRel

        ''' <summary>Game biomass with Fishing regulation.</summary>
        GameBiomassFishRegulation
        GameBiomassFishRegulationPM

        GameBiomassByRegion
        GameCatchRegionFleetGroup
        GameGroupValue
        GameGroupFleetValue

        ''' <summary>Profit by Fleet.</summary>      
        GameFleetProfitSummary
        ''' <summary>Jobs(?) by Fleet.</summary>    
        GameFleetJobsSummary

        GameFleetValue
        GameFleetCatch
        GameFleetCatchPM
        GameFleetValuePM

        ''' <summary>For Ecosim Yield from Ecosim Plots Biomass * FishTime </summary>
        GameGroupCatch
        GameGroupCatchPM

        'Economic data for the game
        GameEconomicCost
        GameEconomicCostPM
        GameEconomicCostByFleet
        GameEconomicCostByFleetPM
        GameEconomicProfit
        GameEconomicProfitByFleet
        GameEconomicProfitByFleetPM
        GameEconomicProfitPM
        GameEconomicJobsTotal
        GameEconomicJobsTotalPM
        GameEconomicJobsByFleet
        GameEconomicJobsByFleetPM
        GameEconomicProduction
        GameEconomicProductionPM

        GameEconomicTaxes
        GameEconomicTaxesPM
        GameEconomicTaxesByFleet
        GameEconomicTaxesByFleetPM

        GameEconomicSubsidies
        GameEconomicSubsidiesPM
        GameEconomicSubsidiesByFleet
        GameEconomicSubsidiesByFleetPM

        ''' <summary>Eco system structure 1/pb * b(t)</summary>    
        GameEcoSystemStruct
        GameEcoSystemStructPM

        ''' <summary>Game names added for the Game data because EwE6 uses Name for all names</summary>
        GameFleetName
        GameMPAName
        GameHabitatName

        GameFleetFishingRatesPM

        GameForceSalinity
        GameForceNutrient
        GameForceTemperature
        GameForcePrimaryProducer

        GameForceSalinityPM
        GameForceNutrientPM
        GameForceTemperaturePM

        GameForceSalinityName
        GameForceNutrientName
        GameForceTemperatureName
        GameForcePrimaryProducerName

        GameForcePrimaryProducerNumber

        ''' <summary>Game biomass for an interation</summary>
        GameBiomassIteration
        ''' <summary>Game catch by group for an interation</summary>
        GameGroupCatchIteration
        ''' <summary>Game effort by fleet for an interation</summary>
        GameFleetEffortIteration
        ''' <summary></summary>
        GameFleetValueIteration

        PSDEnabled
        PSDComputed
        VBK
        BiomassAvgSzWt
        BiomassSzWt
        AinLWInput
        AinLWOutput
        BinLWInput
        BinLWOutput
        LooInput
        LooOutput
        WinfInput
        WinfOutput
        t0Input
        t0Output
        TCatchInput
        TCatchOutput
        TmaxInput
        TmaxOutput
        PSDIncluded
        PSDMortalityType
        PSDFirstWeightClass
        PSDNumWeightClasses
        ClimateType
        NumPtsMovAvg

        LayerIBMPackets

        ' JS 11Jan10: added for NA variables that migrated to Ecosim
        ''' <summary>Trophic level of catch.</summary>
        TLCatch
        ''' <summary>Trophic level of groups.</summary>
        TL
        ''' <summary>Fishing in-balance (FIB) index.</summary>
        FIB
        KemptonsQ
        TotalCatch

        'PM's for NA vars 
        TLCatchPM
        TLPM
        FIBPM
        KemptonsQPM

        EcopathFirstYear
        EcopathNumYears
        South
        North
        West
        East
        AreaName

        TaxonGroup
        TaxonProp
        Phylum
        [Class]
        Order
        Family
        Genus
        Species
        CodeISSCAAP
        CodeTaxon
        Code3A
        Source
        SourceKey
        LastUpdated
        OrganismType
        EcologyType
        Exploited
        IUCNConservationStatus
        OccurrenceStatus
        TaxonMeanWeight
        TaxonMeanLength
        TaxonMaxLength
        TaxonMeanLifespan
        TaxonVulnerabilityIndex

        Coriolis
        XVelocity
        YVelocity
        SorWv

        ''' <summary>States if a value is estimated by Ewe.</summary>
        Estimated
        ''' <summary>1 - EE</summary>
        ''' <remarks>JS Added 28Feb11</remarks>
        OtherMortInput

    End Enum

#End Region ' Variable names

#Region " Data Types "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type that indicates a class of data in the EwE core.
    ''' </summary>
    ''' <remarks>
    ''' These enums have fixed values since values may be used to identify 
    ''' items in the EwE6 database system.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Enum eDataTypes

        ''' <summary>
        ''' Data type is not specified.
        ''' </summary>
        NotSet = 0

        ''' <summary>
        ''' Data belongs to the EwE model.
        ''' </summary>
        EwEModel = 1

        ''' <summary>
        ''' Data belongs to the Ecopath group inputs,
        ''' which are provided to perform a parameter estimation run. 
        ''' </summary>
        EcoPathGroupInput = 2

        ''' <summary>
        ''' Data belongs to the Ecopath group outputs,
        ''' which are produced by a parameter estimation run.
        ''' </summary>
        EcoPathGroupOutput = 3

        ''' <summary>
        ''' Data belongs to the Ecopath fleet inputs,
        ''' which are provided for a parameter estimation run.
        ''' </summary>
        FleetInput = 4

        ''' <summary>
        ''' Data belongs to an Ecosim scenario.
        ''' </summary>
        EcoSimScenario = 5

        ''' <summary>
        ''' Data belongs to the Ecosim model parameters,
        ''' which instruct how to run an Ecosim scenario.
        ''' </summary>
        EcoSimModelParameter = 6

        ''' <summary>
        ''' Data belongs to an Ecosim group input.
        ''' </summary>
        EcoSimGroupInput = 7

        ''' <summary>
        ''' Data belongs to a Time Forcing Function.
        ''' </summary>
        Forcing = 8

        ''' <summary>
        ''' Data belongs to an Egg Production Forcing Function.
        ''' </summary>
        EggProd = 9

        ''' <summary>
        ''' Data belongs to an Mediation Function.
        ''' </summary>
        Mediation = 10

        ''' <summary>
        ''' Data belongs to an Fishing Rate shape.
        ''' </summary>
        FishingEffort = 11

        ''' <summary>
        ''' Data belongs to an Fishing Mortality shape.
        ''' </summary>
        FishMort = 12

        ''' <summary>
        ''' Data belongs to an EwE multi-stanza configuration.
        ''' </summary>
        Stanza = 13 'jb June-14-06 added for Stanza data types

        ''' <summary>
        ''' Data belongs to an Ecospace scenario.
        ''' </summary>
        EcoSpaceScenario = 14

        ''' <summary>
        ''' Data belongs to an Ecospace habitat.
        ''' </summary>
        EcospaceHabitat = 15

        ''' <summary>
        ''' Data belongs to an Ecospace region.
        ''' </summary>
        EcospaceRegion = 16

        ''' <summary>
        ''' Data belongs to an Ecospace group.
        ''' </summary>
        EcospaceGroup = 17

        ''' <summary>
        ''' Data belongs to an Ecospace fleet.
        ''' </summary>
        EcospaceFleet = 18

        ''' <summary>
        ''' Data belongs to an Ecospace MPA.
        ''' </summary>
        EcospaceMPA = 19

        ''' <summary>
        ''' Data belongs to the Ecospace model parameters,
        ''' which instruct how to run an Ecopace scenario.
        ''' </summary>
        EcospaceModelParameter = 20

        ''' <summary>
        ''' Data belongs to a cEcospaceModelBasemaps instance.
        ''' </summary>
        EcospaceBasemap = 21

        ''' <summary>
        ''' Data belongs to a ecospace importance layer instance.
        ''' </summary>
        ''' <remarks>The enum value </remarks>
        EcospaceLayerImportance = 22

        ''' <summary>
        ''' cPredPreyInteraction object
        ''' </summary>
        PredPreyInteraction = 23

        ''' <summary>
        ''' Time step results of the currently running Ecospace model
        ''' </summary>
        EcospaceTimestepResults = 24

        ''' <summary>
        ''' Data belongs to Ecospace calculated values for a single group.
        ''' </summary>
        EcospaceGroupOuput = 25

        ''' <summary>
        ''' Data belongs to Ecospace calculated values for a single fleet.
        ''' </summary>
        EcospaceFleetOuput = 26

        ''' <summary>
        ''' Data belongs to Ecospace calculated values for a single region.
        ''' </summary>
        EcospaceRegionResults = 27

        '''' <summary>
        '''' Data belongs to Network Analysis.
        '''' </summary>
        'NetworkFlowOutput = 28

        ''' <summary>
        ''' Data belongs to a time series that applies to groups.
        ''' </summary>
        GroupTimeSeries = 29

        ''' <summary>
        ''' Data belongs to a time series that applies to fleets.
        ''' </summary>
        FleetTimeSeries = 30

        ''' <summary>
        ''' Data belongs to a Time Series data set.
        ''' </summary>
        TimeSeriesDataset = 31

        ''' <summary>
        ''' Data belongs to a Ecosim Monte Carlo model parameters.
        ''' </summary>
        MonteCarlo = 32

        ''' <summary>
        ''' Data belongs to values calculated by Ecosim for a single group.
        ''' </summary>
        EcoSimGroupOutput = 33

        ''' <summary>
        ''' Data belongs to values calculated by Ecosim for a single fleet.
        ''' </summary>
        EcosimFleetOutput = 34

        ''' <summary>
        ''' Data belongs to Ecosim Fit To Time Series model parameters.
        ''' </summary>
        FitToTimeSeries = 35

        ''' <summary>
        ''' Data belongs to an Ecotracer scenario.
        ''' </summary>
        EcotracerScenario = 36

        ''' <summary>
        ''' Data belongs to Ecotracer model parameters.
        ''' </summary>
        EcotracerModelParameters = 37

        ''' <summary>
        ''' Data belongs to an Ecotracer input group.
        ''' </summary>
        EcotracerGroupInput = 38

        ''' <summary>
        ''' Data belongs to an Ecotracer Ecosim results for a single group.
        ''' </summary>
        EcotracerSimOutput = 39

        ''' <summary>
        ''' Data belongs to an Ecotracer Ecospace results for a single group.
        ''' </summary>
        EcotracerSpaceOutput = 40

        ''' <summary>
        ''' Data belongs to a search objectives manager.
        ''' </summary>
        ''' <remarks>
        ''' Search Objectives form the base for the shared search interface 
        ''' ISearchObjective used by Fishing Policy, Ecoseed, MSE and possibly
        ''' other searches. This system is flexible and be extended.
        ''' </remarks>
        SearchObjectiveManager = 41

        ''' <summary>
        ''' Data belongs to search objectives generic parameters.
        ''' </summary>
        ''' <remarks>Don't panic.</remarks>
        SearchObjectiveParameters = 42

        ''' <summary>
        ''' Data belongs to search objectives parameters for a single fleet.
        ''' </summary>
        SearchObjectiveFleetInput = 43

        ''' <summary>
        ''' Data belongs to search objective weights.
        ''' </summary>
        SearchObjectiveWeights = 44

        ''' <summary>
        ''' Data belongs to search objectives parameters for a single group.
        ''' </summary>
        SearchObjectiveGroupInput = 45

        ''' <summary> 
        ''' Data belongs to the Fishing Policy search manager.
        ''' </summary>
        ''' <remarks>
        ''' Note that the Fishing Policy manager may use the SearchObjectivexxxx data types as well.
        ''' </remarks>
        FishingPolicyManager = 46

        ''' <summary>
        ''' Data belongs to fishing policy search generic parameters.
        ''' </summary>
        FishingPolicyParameters = 47

        ''' <summary>
        ''' Data belongs to fishing policy search search blocks settings.
        ''' </summary>
        FishingPolicySearchBlocks = 48

        ''' <summary> 
        ''' Data belongs to the MPA optimizations/Ecoseed search manager.
        ''' </summary>
        MPAOptManager = 49

        ''' <summary>
        ''' Data belongs to the MPA optimizations/Ecoseed results.
        ''' </summary>
        MPAOptOuput = 50

        ''' <summary> 
        ''' Data belongs to the MPA optimizations/Ecoseed generic parameters.
        ''' </summary>
        MPAOptParameters = 51

        ''' <summary> 
        ''' Data belons to the Management Strategy Evaluator.
        ''' </summary>
        MSEManager = 52

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluator parameters for a single fleet.
        ''' </summary>
        MSEFleetInput = 53

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluator parameters for a single group.
        ''' </summary>
        MSEGroupInput = 54

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluator generic results.
        ''' </summary>
        MSEOutput = 55

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluator generic parameters.
        ''' </summary>
        MSEParameters = 56

        ''' <summary>
        ''' Data belongs to a single Pedigree level.
        ''' </summary>
        PedigreeLevel = 57

        ''' <summary>
        ''' Data belongs to the EwE game engine data.
        ''' </summary>    
        GameData = 58

        '''' <summary>
        '''' Data belongs to the Ecosim fisheries regulation engine.
        '''' </summary>    
        'EcosimFisheriesRegulation = 59

        ''' <summary>
        ''' Data belongs to Ecopath statistics.
        ''' </summary>
        EcoPathStatistics = 60

        ''' <summary>
        ''' Data belongs to Ecosim statistics.
        ''' </summary>
        EcoSimStatistics = 61

        ''' <summary>
        ''' Data belongs to Ecospace statistics.
        ''' </summary>
        EcospaceStatistics = 62

        ''' <summary>
        ''' Data belongs to Particle Size Distribution generic parameters.
        ''' </summary>
        ParticleSizeDistribution = 63

        ''' <summary>
        ''' Data belongs to the Ecospace Depth layer.
        ''' </summary>
        EcospaceLayerDepth = 64

        ''' <summary>
        ''' Data belongs to the Ecospace Marine Protected Areas layer.
        ''' </summary>
        EcospaceLayerMPA = 65

        ''' <summary>
        ''' Data belongs to the Ecospace MPA seed layer.
        ''' </summary>
        ''' <remarks>
        ''' MPA seeds are used in the MPA optimizations/Ecoseed searches.
        ''' </remarks>
        EcospaceLayerMPASeed = 66

        ''' <summary>
        ''' Data belongs to the Ecospace Habitat layer.
        ''' </summary>
        EcospaceLayerHabitat = 67

        ''' <summary>
        ''' Data belongs to the Ecospace Regions layer.
        ''' </summary>
        EcospaceLayerRegion = 68

        ''' <summary>
        ''' Data belongs to the Ecospace relative primary production layer.
        ''' </summary>
        EcospaceLayerRelPP = 69

        ''' <summary>
        ''' Data belongs to the Ecospace relative contaminant layer.
        ''' </summary>
        EcospaceLayerRelCin = 70

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluation results for a single group.
        ''' </summary>
        MSEGroupOutputs = 71

        ''' <summary>
        ''' Data belongs to the Ecospace layer representing the spread and quantities
        ''' of Individual Based Model packets.
        ''' </summary>
        EcospaceLayerIBMPackets = 72

        ''' <summary>
        ''' Data belongs to the Ecospace layer representing fishing ports.
        ''' </summary>
        EcospaceLayerPort = 73

        ''' <summary>
        ''' Data belongs to the Ecospace layer representing cost of sailing.
        ''' </summary>
        EcospaceLayerSail = 74

        ''' <summary>
        ''' Data belongs to Ecosim input data for single group.
        ''' </summary>
        EcosimFleetInput = 75

        ''' <summary>
        ''' Data belongs to Ecosim results for single group.
        ''' </summary>
        EcosimOutput = 76

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluation results for a single fleet.
        ''' </summary>
        MSEFleetOutputs = 76

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluation biomass statistical results.
        ''' </summary>
        MSEBiomassStats = 77

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluation statistical results on catches by group.
        ''' </summary>
        MSECatchByGroupStats = 78

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluation statistical results on catches by fleet.
        ''' </summary>
        MSECatchByFleetStats = 79

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluation statistical results on fishing effort.
        ''' </summary>
        MSEEffortStats = 80

        ''' <summary>
        ''' Data belongs to the Ecospace Migration layer.
        ''' </summary>
        EcospaceLayerMigration = 81

        ''' <summary>
        ''' Data belongs to the Ecospace Advection layer.
        ''' </summary>
        EcospaceLayerAdvection = 82

        ''' <summary>
        ''' Data belongs to Auxillary data.
        ''' </summary>
        Auxillary = 83

        MSEBioEstStats = 84

        ''' <summary>
        ''' Data belongs to the Ecospace Distribution envelope layer.
        ''' </summary>
        EcospaceLayerDistribution = 85

        ''' <summary>
        ''' Data belongs to a taxonomy definition.
        ''' </summary>
        Taxon = 85

        ''' <summary>
        ''' Data belongs to the Ecospace wind layer.
        ''' </summary>
        EcospaceLayerWind = 86

        ''' <summary>
        ''' Data belongs to the Ecospace transport rate layer.
        ''' </summary>
        EcospaceLayerTransportRate = 87

        ''' <summary>
        ''' Data belongs to the Ecospace flow layer.
        ''' </summary>
        EcospaceLayerFlow = 88

        ''' <summary>
        ''' Data belongs to the Ecospace mixed layer depth layer.
        ''' </summary>
        EcospaceLayerMLD = 89

        ''' <summary>
        ''' Data belongs to the Ecospace upwelling layer.
        ''' </summary>
        EcospaceLayerUpwelling = 90

        EcospaceAdvectionManager = 91
        EcospaceAdvectionParameters = 92

        PedigreeManager = 93

        ''' <summary>
        ''' Data belongs to Management Strategy Evaluation statistical results total value.
        ''' </summary>
        MSEValueTotalStats = 94

        ''' <summary>
        ''' Data belongs to an external, unspecified source.
        ''' </summary>
        External = 777

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

        ''' <summary>Number of rows in the Ecospace basemap.</summary>
        nRows
        ''' <summary>Number of columns in the Ecospace basemap.</summary>
        nCols

        ''' <summary>Number of timesteps in the Ecopath Weight, Number and Biomass</summary>
        nEcopathAgeSteps
        ''' <summary>Number of weight classes in the particle size distribution</summary>
        nWeightClasses

        ''' <summary>Number of forcing function that are for Salinity.</summary>
        ''' <remarks>At this time this is only used by the Decision Support Tool(game).</remarks>
        nSalinityForcingFunctions

        ''' <summary>Number of forcing function that are for Salinity.</summary>
        ''' <remarks>At this time this is only used by the Decision Support Tool(game).</remarks>
        nNutrientForcingFunctions
        ''' <summary>Number of forcing function that are for Salinity.</summary>
        ''' <remarks>At this time this is only used by the Decision Support Tool(game).</remarks>
        nTempForcingFunctions

        ''' <summary> Number of forcing function that are for Primary Producer.</summary>
        ''' <remarks>At this time this is only used by the Decision Support Tool(game).</remarks>
        nPPForcingFunctions

        ''' <summary>The number of iterations running in the game.</summary>
        nGameIterations

        ''' <summary>The number of taxonomy groups.</summary>
        nTaxon

        ''' <summary>The number of supported pedigree variables.</summary>
        nPedigreeVariables

    End Enum

#End Region ' Core counters

#Region " System units "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated types providing currency types.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eUnitCurrencyType As Integer
        ''' <summary>Unit currency type not set.</summary>
        NotSet = 0
        ''' <summary>Currency expressed in j/m².</summary>
        Joules = 1
        ''' <summary>Currency expressed in kcal/m².</summary>
        Calorie = 2
        ''' <summary>Currency expressed in g/m².</summary>
        Carbon = 3
        ''' <summary>Currency expressed in dry weight (g/m²).</summary>
        DryWeight = 4
        ''' <summary>Currency expressed in wet weight (t/km²).</summary>
        WetWeight = 5
        ''' <summary>Custom currency unit.</summary>
        CustomEnergy = 6
        ''' <summary>Currency expressed in mg n/m².</summary>
        Nitrogen = 7
        ''' <summary>Currency expressed in mg p/m².</summary>
        Phosporous = 8
        ''' <summary>Custom currency unit.</summary>
        CustomNutrient = 9
    End Enum

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type listing time units.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eUnitTimeType As Integer
        ''' <summary>User has specified a custom time unit.</summary>
        Custom = 0
        ''' <summary>Time expressed in years.</summary>
        Year
        ''' <summary>Time expressed in days.</summary>
        Day
    End Enum

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated types providing area types.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eUnitAreaType As Integer
        ''' <summary>Custom area measure.</summary>
        Custom = 0
        ''' <summary>Area expressed in square km.</summary>
        Km2
        ''' <summary>Area expressed in square miles.</summary>
        Mi2
    End Enum

#End Region ' System units

#Region " Quota types "

    'enum values are hard coded so that they can be stored in the database 
    Public Enum eQuotaTypes
        ''' <summary>No Quota controls are used.</summary>
        NoControls
        ''' <summary>Quota options apply to the weakest stock.</summary>
        Weakest
        ''' <summary>Quota options apply to the strongest stock plus discards.</summary>
        HighestValue
        ''' <summary>Quota options apply to selective fishing.</summary>
        Selective
        ''' <summary>Quota options apply to effort.</summary>
        Effort

    End Enum

#End Region ' Quota types

#Region " Datasource types "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Supported types of data sources.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Enum eDataSourceTypes
        ''' <summary>No support.</summary>
        NotSet = 0
        ''' <summary>Datasource capable of handling EII formatted data.</summary>
        EII = 1
        ''' <summary>Datasource capable of handling MDB formatted data.</summary>
        Access2003 = 2
        ''' <summary>Datasource capable of handling ACCDB and MDB formatted data.</summary>
        Access2007 = 3
        ''' <summary>Datasource capable of handling MDB formatted data.</summary>
        <System.Obsolete("Deprecated; use eDataSourceTypes.Access2003 instead")> _
        MDB = Access2003
        ''' <summary>Datasource capable of handling ACCDB and MDB formatted data.</summary>
        <System.Obsolete("Deprecated; use eDataSourceTypes.Access2007 instead")> _
        ACCDB = Access2007
    End Enum

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type describing the result of datasource access attempts.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Enum eDatasourceAccessType As Integer
        ''' <summary>Database operation succesful.</summary>
        Success = 0
        ''' <summary>Database could not be saved in the indicated location.</summary>
        Failed_CannotSave
        ''' <summary>An unknown database type was requested.</summary>
        Failed_UnknownType
        ''' <summary>System does not have the correct drivers installed to
        ''' support the requested database type.</summary>
        Failed_OSUnsupported
        ''' <summary>An unknown error has occurred.</summary>
        Failed_Unknown
        ''' <summary>No permissions to write to the database.</summary>
        Failed_ReadOnly
        ''' <summary>Cannot switch from one type of database to another.</summary>
        Failed_TransferTypes
        ''' <summary>Cannot perform requested operation on this type of file.</summary>
        Failed_DeprecatedOperation
        ''' <summary>File is not found.</summary>
        Failed_FileNotFound
        ''' <summary>Deprecated, use <see cref="eDatasourceAccessType.Success">Sccess</see> instead.</summary>
        Opened = Success
        ''' <summary>Deprecated, use <see cref="eDatasourceAccessType.Success">Sccess</see> instead.</summary>
        Created = Success
    End Enum

#End Region ' Datasource types

#Region " Search criteria results "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Search criteria result types
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eSearchCriteriaResultTypes As Integer

        TotalValue = 1
        Employment = 2
        MandateReb = 3
        Ecological = 4
        BioDiversity = 5
    End Enum

#End Region ' Search criteria results

#Region " CoreComponentType "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type, identifying sources of messages being broadcasted by the Core.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eCoreComponentType
        ''' <summary>The message source is not specified.</summary>
        NotSet
        ''' <summary>The message originated from the Ecopath module of EwE.</summary>
        EcoPath
        ''' <summary>The message originated from the Ecosim module of EwE.</summary>
        EcoSim
        ''' <summary>The message originated from the Ecospace module of EwE.</summary>
        EcoSpace
        ''' <summary>The message originated from the Forcing shapes manager(s) in EwE.</summary>
        ShapesManager
        ''' <summary>The message originated from a datasource.</summary>
        DataSource
        ''' <summary>The message originated from the core itself.</summary>
        Core
        ''' <summary>The message originated from a Plugin </summary>
        Plugin
        ''' <summary>The message originated from the Monte Carlo routines in Ecosim.</summary>
        EcoSimMonteCarlo
        ''' <summary>The message originated from the Fit to Time Series routines in Ecosim.</summary>
        EcoSimFitToTimeSeries
        ''' <summary>The message originated from a change in loaded Time Series.</summary>
        TimeSeries
        ''' <summary>The message originated from the pred/prey interaction.</summary>
        PPIManager
        ''' <summary>The message originated from Ecotracer.</summary>
        Ecotracer
        ''' <summary>The message originated from an external source (such as the user interface)</summary>
        External
        ''' <summary>The message source is one of the Search Objective classes</summary>
        SearchObjective
        ''' <summary>The message originated from Fishing Policy Search.</summary>
        FishingPolicySearch
        ''' <summary>Management Strategy Evaluation  </summary>
        MSE
        ''' <summary> EcoSeed </summary>
        MPAOptimization

    End Enum

#End Region ' CoreComponentType

#Region " Forcing application types "

    Public Enum eForcingApplicationTypes As Integer
        ''' <summary>No application specified.</summary>
        NotSet = 0
        ''' <summary>Forcing FF applied to nutrient forcing.</summary>
        Nutrient = 1
        ''' <summary>Forcing FF applied to salinity forcing.</summary>
        Salinity = 2
        ''' <summary>Forcing FF applied to temperature forcing.</summary>
        Temperature = 3
        ''' <summary>Forcing FF applied to primary production.</summary>
        PrimaryProducer = 4
    End Enum

#End Region ' Forcing application types

#Region " IUCN threat classifications "

    ''' <summary>
    ''' IUCN Red List of Threatened Species threat types.
    ''' </summary>
    ''' <remarks>
    ''' http://www.eoearth.org/article/IUCN_Red_List_Criteria_for_Endangered
    ''' </remarks>
    Public Enum eIUCNConservationStatusTypes As Integer
        NotSet = 0
        ''' <summary>Not Evaluated (NE).</summary>
        NotEvaluated
        ''' <summary>Data Deficient (DD)</summary>
        DataDeficient
        ''' <summary>Least Concern (LC)</summary>
        LeastConcern
        ''' <summary>Near Threatened (NT)</summary>
        NearThreatened
        ''' <summary>Vulnerable (VU)</summary>
        Vulnerable
        ''' <summary>Endangered (EN)</summary>
        Endangered
        ''' <summary>Critically Endangered (CR)</summary>
        CriticallyEndangered
        ''' <summary>Extinct in the Wild (EW)</summary>
        ExtinctInWild
        ''' <summary>Extinct (EX)</summary>
        Extinct
    End Enum

#End Region ' IUCN threat classifications

#Region " Ecology types "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' http://www.fishbase.org
    ''' </remarks>
    Public Enum eEcologyTypes As Integer
        NotSet = 0
        BathyDemersal
        BathyPelagic
        Bethic
        BenthoPelagic
        Demersal
        Pelagic
        PelagicNeritic
        BelagicOceanic
        ReefAssociated
        ''' <summary></summary>
        ''' <remarks>To be specified further</remarks>
        LandBased
    End Enum

#End Region ' Ecology types

#Region " Occurrence status types "

    ''' <summary>
    ''' Habitat classifications where taxa prefer to dwell. Not necessarily related to Ecospace habitats.
    ''' </summary>
    ''' <remarks>
    ''' http://www.fishbase.org
    ''' </remarks>
    Public Enum eOccurrenceStatusTypes As Integer
        NotSet = 0
        Native
        Introduced
        Endemic
        Questionable
    End Enum

#End Region ' Occurrence status types

#Region " Organism types "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' </remarks>
    Public Enum eOrganismTypes As Integer
        NotSet = 0
        Bacteria
        Fungi
        Algae
        Plants
        Invertebrates
        Fishes
        Birds
        Mammals
        Reptiles
        Other
    End Enum

#End Region ' Organism types

End Namespace ' Core
