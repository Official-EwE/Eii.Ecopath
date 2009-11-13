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
        ''' <summary>Ecosim scenario data has been loaded.</summary>
        EcosimLoaded
        ''' <summary>Ecosim scenario has been initialized.</summary>
        EcosimInitialized
        ''' <summary>Ecotracer scenario data has been loaded.</summary>
        EcotracerLoaded
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
        LayerMPAPM
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
        ''' <summary>Ecospace MPA importance.</summary>
        LayerImportance
        ''' <summary>Ecospace cell port assignments.</summary>
        LayerPort
        ''' <summary>Ecospace sailing cost.</summary>
        LayerSail
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
        MSEBiomass
        MSEUseEconomicPlugin

        ' Pedigree
        VariableName
        IndexValue
        ConfidenceInterval

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

        ''' <summary>For Ecosim Yield from Ecosim Plots Biomass * FishTime </summary>
        GameGroupCatch
        GameGroupCatchPM

        'Economic data for the game
        GameEconomicCost
        GameEconomicCostPM
        GameEconomicProfit
        GameEconomicProfitPM
        GameEconomicJobsTotal
        GameEconomicJobsTotalPM
        GameEconomicProduction
        GameEconomicProductionPM

        GameEconomicTaxes
        GameEconomicTaxesPM

        GameEconomicSubsidies
        GameEconomicSubsidiesPM

        ''' <summary>Eco system structure 1/pb * b(t)</summary>    
        GameEcoSystemStruct
        GameEcoSystemStructPM

        ''' <summary>Game names added for the Game data because EwE6 uses Name for all names</summary>
        GameFleetName
        GameMPAName
        GameHabitatName

        GameForceSalinity
        GameForceNutrient
        GameForceTemperature

        GameForceSalinityName
        GameForceNutrientName
        GameForceTemperatureName

        GameForceSalinityCurrent
        GameForceNutrientCurrent
        GameForceTemperatureCurrent

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
        ''' 
        ''' </summary>
        EcospaceGroupOuput = 25

        EcospaceFleetOuput = 26

        EcospaceRegionResults = 27

        ''' <summary>
        ''' 
        ''' </summary>
        NetworkFlowOutput = 28

        ''' <summary>
        ''' Data belongs to a cGroupTimeSeries instance.
        ''' </summary>
        GroupTimeSeries = 29

        ''' <summary>
        ''' Data belongs to a cFleetTimeSeries instance.
        ''' </summary>
        FleetTimeSeries = 30

        ''' <summary>
        ''' Data belongs to a Time Series Dataset instance.
        ''' </summary>
        TimeSeriesDataset = 31

        ''' <summary>
        ''' Ecosim Monte Carlo
        ''' </summary>
        ''' <remarks></remarks>
        MonteCarlo = 32

        ''' <summary>
        ''' Data belongs to an Ecosim group output.
        ''' </summary>
        EcoSimGroupOutput = 33
        EcosimFleetOutput = 34

        FitToTimeSeries = 35

        ''' <summary>
        ''' Data belongs to an Ecotracer scenario
        ''' </summary>
        EcotracerScenario = 36

        ''' <summary>
        ''' Data belongs to Ecotracer model parameters
        ''' </summary>
        EcotracerModelParameters = 37

        ''' <summary>
        ''' Data belongs to an Ecotracer input group
        ''' </summary>
        EcotracerGroupInput = 38
        EcotracerSimOutput = 39
        EcotracerSpaceOutput = 40

        ''' <summary>Search Objectives </summary>
        '''<remarks>Search Objectives form the base for the shared search interface ISearchObjective used by Fishing Policy, Ecoseed and MSE </remarks>
        SearchObjectiveManager = 41
        SearchObjectiveParameters = 42 ' Don't panic
        SearchObjectiveFleetInput = 43
        SearchObjectiveWeights = 44
        SearchObjectiveGroupInput = 45

        ''' <summary> Fishing Policy (implements ISearchObjective) </summary>
        ''' <remarks>The Fishing Policy uses SearchObjectivexxxx data types as well </remarks>
        FishingPolicyManager = 46
        FishingPolicyParameters = 47
        FishingPolicySearchBlocks = 48

        ''' <summary> Ecoseed manager (implements ISearchObjective)  </summary>
        MPAOptManager = 49
        'EcoSeedInput
        MPAOptOuput = 50
        MPAOptParameters = 51

        ''' <summary> Management Strategy Evaluator (implements ISearchObjective)  </summary>
        MSEManager = 52
        MSEFleetInput = 53
        MSEGroupInput = 54
        MSEOutput = 55
        MSEParameters = 56

        ''' <summary>Pedigree</summary>
        PedigreeLevel = 57

        ''' <summary>Data types for the Game</summary>    
        GameData = 58

        ''' <summary>Data types for Ecosim fisheries regulation</summary>    
        EcosimFisheriesRegulation = 59

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

        ParticleSizeDistribution = 63

        EcospaceLayerDepth = 64
        EcospaceLayerMPA = 65
        EcospaceLayerMPASeed = 66
        EcospaceLayerHabitat = 67
        EcospaceLayerRegion = 68
        EcospaceLayerRelPP = 69
        EcospaceLayerRelCin = 70
        EcospaceLayerIBMPackets = 72
        EcospaceLayerPort = 73
        EcospaceLayerSail = 74

        MSEGroupOutputs = 71
        EcosimFleetInput = 75

        ''' <summary>
        ''' Data belongs to an external source.
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

        'Joe
        ''' <summary>Number of timesteps in the Ecopath Weight, Number and Biomass</summary>
        nEcopathAgeSteps
        ''' <summary>Number of weight classes in the particle size distribution</summary>
        nWeightClasses
        'End Joeh

        ''' <summary> Number of steps to complete a process </summary>
        ''' <remarks>At this time this is only used by the Decision Support Tool(game) and is dynamic depending on the current process!!!</remarks>
        nProgressSteps

        ''' <summary> Number of forcing function that are for Salinity </summary>
        '''  <remarks>At this time this is only used by the Decision Support Tool(game) </remarks>
        nSalinityForcingFunctions
        ''' <summary> Number of forcing function that are for Salinity </summary>
        '''  <remarks>At this time this is only used by the Decision Support Tool(game) </remarks>
        nNutrientForcingFunctions
        ''' <summary> Number of forcing function that are for Salinity </summary>
        '''  <remarks>At this time this is only used by the Decision Support Tool(game) </remarks>
        nTempForcingFunctions
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

#Region " Quota types "

    'enum values are hard coded so that they can be stored in the database 
    Public Enum eQuotaTypes
        NotUsed = 0
        Weakest = 1
        Strongest = 2
        Selective = 3
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
        ''' <summary>Datasource capable of handling EII-formatted data.</summary>
        EII
        ''' <summary>Datasource capable of handling MDB-formatted data.</summary>
        MDB
        ''' <summary>Datasource capable of handling ACCDB-formatted data.</summary>
        ACCDB
    End Enum

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type describing the result of datasource access attempts.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Enum eDatasourceAccessType As Integer
        ''' <summary>Database succesfully created.</summary>
        Created = 0
        ''' <summary>Database succesfully opened.</summary>
        Opened = 0
        ''' <summary>Database could not be saved in the indicated location.</summary>
        Failed_CannotSave
        ''' <summary>An unknown database type was requested.</summary>
        Failed_UnknownType
        ''' <summary>System does not have the correct drivers installed to
        ''' support the requested database type.</summary>
        Failed_OSUnsupported
        ''' <summary>An unknown error has occurred.</summary>
        Failed_Unknown
        ''' <summary>Cannot switch from one type of database to another.</summary>
        Failed_TransferTypes
        ''' <summary>Cannot perform requested operation on this type of file.</summary>
        Failed_DeprecatedOperation
        ''' <summary>File is not found.</summary>
        Failed_FileNotFound
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

End Namespace ' Core
