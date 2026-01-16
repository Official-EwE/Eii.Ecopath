' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwECore
Imports EwEUtils.SystemUtilities.cSystemUtils



''' ===========================================================================
''' <summary>
''' Value Chain results holder.
''' </summary>
''' ===========================================================================
Public Class cResults

#Region " Private helper class "

    ''' =======================================================================
    ''' <summary>
    ''' Results for a single time step.
    ''' </summary>
    ''' =======================================================================
    Private Class cTimeStepResults

        ''' <summary>Ecost data that these results relate to.</summary>
        Private m_data As cData = Nothing
        ''' <summary>Redundant: time step index</summary>
        Private m_iTimeStep As Integer = 0
        ''' <summary>Results(# variable types, # units)</summary>
        Private m_results(,) As Single

        Public Sub New(data As cData, iTimeStep As Integer)
            Me.m_data = data
            Me.m_iTimeStep = iTimeStep
            ReDim Me.m_results([Enum].GetNames(GetType(eVariableType)).Length, Me.m_data.UnitCount)
        End Sub

        Public Property Results(iVar As Integer, iUnit As Integer) As Single
            Get
                Return Me.m_results(iVar, iUnit)
            End Get
            Set(value As Single)
                Me.m_results(iVar, iUnit) = value
            End Set
        End Property

        Public Function Clone() As cTimeStepResults
            Dim tsr As New cTimeStepResults(Me.m_data, Me.m_iTimeStep)
            For i As Integer = 0 To Me.m_results.GetUpperBound(0)
                For j As Integer = 0 To Me.m_results.GetUpperBound(1)
                    tsr.Results(i, j) = Me.m_results(i, j)
                Next j
            Next i
            Return tsr
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, calculates derived values for a timestep result.
        ''' Derived variables are totals and sub-totals of result categories.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Friend Sub CalculateDerivedValues()

            Dim unit As cUnit = Nothing

            ' Note that although units provide different types of variables, all 
            ' variable categories can still be bluntly Totaled. Variable values 
            ' that are not used are 0 by default.

            ' Calc derived vars for each unit
            For iUnit As Integer = 0 To Me.m_data.UnitCount - 1

                unit = Me.m_data.Unit(iUnit)

                ' Revenue total
                Dim sRevenue As Single = 0.0!
                ' Revenue breakdown
                Dim sRevenueProductsOther As Single = 0.0!
                Dim sRevenueTickets As Single = 0

                ' Cost total
                Dim sCost As Single = 0.0!
                Dim sProfit As Single = 0.0!
                Dim sTotalUtility As Single = 0.0!
                ' Cost breakdown
                Dim sCostSalariesShares As Single = 0.0!
                Dim sCostManagementRoyaltyCertificationObserver As Single = 0.0!
                Dim sCostlInputOther As Single = 0.0!

                ' Jobs
                Dim sTotalJobs As Single = 0.0!
                ' Jobs breakdown
                Dim sTotalJobsMale As Single = 0.0!
                Dim sTotalJobsFemale As Single = 0.0!

                ' Dependents total
                Dim sDependentsTotal As Single = 0.0!
                Dim sGDP As Single = 0.0!

                sRevenueProductsOther = Me.m_results(eVariableType.RevenueProductsOther, unit.Sequence) +
                        Me.m_results(eVariableType.RevenueAgriculture, unit.Sequence)

                sRevenueTickets = Me.m_results(eVariableType.RevenueTickets, unit.Sequence)

                sRevenue = sRevenueProductsOther + sRevenueTickets +
                        Me.m_results(eVariableType.RevenueSubsidies, unit.Sequence)

                'If isBroker = False Then  'this is not a broker, so the revenus from selling the product is theirs, and it counts in the utility
                sRevenue += Me.m_results(eVariableType.RevenueProductsMain, unit.Sequence)

                ' Cost
                sCostSalariesShares = Me.m_results(eVariableType.CostWorker, unit.Sequence) +
                        Me.m_results(eVariableType.CostOwner, unit.Sequence)

                sCostManagementRoyaltyCertificationObserver = Me.m_results(eVariableType.CostManagementRoyaltyCertification, unit.Sequence) +
                        Me.m_results(eVariableType.CostObserver, unit.Sequence)

                sCostlInputOther = Me.m_results(eVariableType.CostAgriculture, unit.Sequence) +
                        Me.m_results(eVariableType.CostInput, unit.Sequence)

                sCost = sCostSalariesShares +
                        sCostlInputOther +
                        Me.m_results(eVariableType.CostTaxes, unit.Sequence) +
                        sCostManagementRoyaltyCertificationObserver

                sCost += Me.m_results(eVariableType.CostRawmaterial, unit.Sequence)

                ' Profit
                Dim grossProfit As Single = sRevenue - sCost
                'tax on profit:
                If grossProfit > 0 And TypeOf unit Is cEconomicUnit Then
                    Dim TaxOnProfit As Single = DirectCast(unit, cEconomicUnit).ProfitTax * grossProfit
                    Me.m_results(eVariableType.CostTaxes, unit.Sequence) += TaxOnProfit
                    sCost += TaxOnProfit
                End If

                sProfit = sRevenue - sCost


                ' TotalUtility a.k.a. Throughput = cost when (profit < 0), revenue otherwise
                sTotalUtility = If(sProfit < 0, sCost, sRevenue)

                ' Jobs
                sTotalJobsMale = Me.m_results(eVariableType.NumberOfWorkerMales, unit.Sequence) +
                        Me.m_results(eVariableType.NumberOfOwnerMales, unit.Sequence)
                sTotalJobsFemale = Me.m_results(eVariableType.NumberOfWorkerFemales, unit.Sequence) +
                        Me.m_results(eVariableType.NumberOfOwnerFemales, unit.Sequence)
                sTotalJobs = sTotalJobsFemale + sTotalJobsMale

                ' Dependents, total
                sDependentsTotal = Me.m_results(eVariableType.NumberOfOwnerDependents, unit.Sequence) +
                        Me.m_results(eVariableType.NumberOfWorkerDependents, unit.Sequence)

                ' Store
                Me.m_results(eVariableType.RevenueProductsOther, unit.Sequence) = sRevenueProductsOther
                Me.m_results(eVariableType.RevenueTotal, unit.Sequence) = sRevenue

                Me.m_results(eVariableType.CostTotalInputOther, unit.Sequence) = sCostlInputOther
                Me.m_results(eVariableType.CostSalariesShares, unit.Sequence) = sCostSalariesShares
                Me.m_results(eVariableType.CostManagementRoyaltyCertificationObservers, unit.Sequence) = sCostManagementRoyaltyCertificationObserver
                Me.m_results(eVariableType.Cost, unit.Sequence) = sCost
                Me.m_results(eVariableType.Profit, unit.Sequence) = sProfit
                Me.m_results(eVariableType.TotalUtility, unit.Sequence) = sTotalUtility

                Me.m_results(eVariableType.NumberOfJobsFemaleTotal, unit.Sequence) = sTotalJobsFemale
                Me.m_results(eVariableType.NumberOfJobsMaleTotal, unit.Sequence) = sTotalJobsMale
                Me.m_results(eVariableType.NumberOfJobsTotal, unit.Sequence) = sTotalJobs

                Me.m_results(eVariableType.NumberOfDependentsTotal, unit.Sequence) = sDependentsTotal

                sGDP = Me.m_results(eVariableType.CostSalariesShares, unit.Sequence) +
                       Me.m_results(eVariableType.CostTaxes, unit.Sequence) +
                       Me.m_results(eVariableType.CostManagementRoyaltyCertificationObservers, unit.Sequence) +
                       Me.m_results(eVariableType.Profit, unit.Sequence) -
                       Me.m_results(eVariableType.RevenueSubsidies, unit.Sequence)

                Me.m_results(eVariableType.GDPContribution, unit.Sequence) = sGDP

            Next iUnit

        End Sub

    End Class

#End Region ' Private helper class

#Region " Private vars "

    ''' <summary>The data to aggregate results for.</summary>
    Private m_data As cData = Nothing
    ''' <summary>Dictionary[timestep, result] of results per time step.</summary>
    Private m_dtResultTimeStep As New Dictionary(Of Integer, cTimeStepResults)
    ''' <summary>Dictionary[key, result] of results for an equilbrium run.</summary>
    Private m_dtSnapshots As New Dictionary(Of Object, cTimeStepResults)

    ''' <summary>Contributions of an item (fleet, group, ..) to a unit per timestep to the total value.</summary>
    ''' <remarks>Indexed as (item, time step, unit sequence).</remarks>
    Private m_ValueContribution As Single(,,)
    ''' <summary>Contributions of an item (fleet, group, ..) to a unit per timestep to the total biomass.</summary>
    ''' <remarks>Indexed as (item, time step, unit sequence).</remarks>
    Private m_BiomassContribution As Single(,,)

    ''' <summary>Max no of time steps.</summary>
    Private m_iMaxTimeStep As Integer = 0
    ''' <summary>Max no of items values are aggregated over.</summary>
    Private m_iMaxItem As Integer = 0

    ''' <summary>Run type that results were computed for.</summary>
    Private m_runType As cModel.eRunTypes = cModel.eRunTypes.Ecopath

    ''' <summary>The biomass flows from one unit to another (source x target)</summary>
    Public m_BiomassFlows(,) As Double

#End Region ' Private vars

#Region " Public enums "

    ''' <summary>
    ''' Types of calculated results.
    ''' </summary>
    Public Enum eVariableType As Integer

        ''' <summary> Production of fish products in tonnes </summary>
        Production
        ''' <summary> Production of fish products in corresponding live weight </summary>
        ProductionLive

        CostRawmaterial
        CostInput
        CostAgriculture
        CostManagementRoyaltyCertification
        CostTaxes
        CostOwner
        CostWorker

        ''' <summary>Cost of observers</summary>
        ''' <remarks>over tonnes</remarks>
        CostObserver
        Cost
        CostManagementRoyaltyCertificationObservers
        CostSalariesShares
        CostTotalInputOther

        Profit

        ''' <summary> The value of the fish products  </summary>
        RevenueProductsMain
        ''' <summary> Revenue from Agricultural products, should they be making any such as a byproduct </summary>
        RevenueAgriculture
        ''' <summary> Revenue from ticket sale, which will be a function of effort </summary>
        RevenueTickets
        ''' <summary> The value of other products than the actual fish </summary>
        ''' <remarks>over tonnes</remarks>
        RevenueProductsOther
        ''' <remarks>over tonnes</remarks>
        RevenueSubsidies
        RevenueTotal

        TotalUtility

        NumberOfWorkerFemales
        NumberOfWorkerMales
        NumberOfWorkerPartTime
        NumberOfWorkerOther

        NumberOfOwnerFemales
        NumberOfOwnerMales
        NumberOfJobsTotal

        NumberOfWorkerDependents
        NumberOfOwnerDependents
        NumberOfDependentsTotal

        OutputBiomass
        OutputBiomassLW

        NumberOfJobsMaleTotal
        NumberOfJobsFemaleTotal

        'VC090401: added the factors below to calc by type of units:
        CostProducers
        CostProcessors
        CostDistributors
        CostMarket
        CostConsumer
        RevenueProducers
        RevenueProcessors
        RevenueDistributors
        RevenueMarket
        'No revenue for consumers
        ProfitProducers
        ProfitProcessors
        ProfitDistributors
        ProfitMarket
        'No profit for consumers

        Landings
        LandingsPrice

        GDPContribution

    End Enum

    Public Enum eGraphDataType As Integer
        CostRevenue = 0
        Cost
        Revenue
        Jobs
        Dependents
    End Enum

#End Region ' Public enums

#Region " Construction "

    Public Sub New(data As cData)
        Me.m_data = data
    End Sub

#End Region ' Construction

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the collection of <see cref="eVariableType">variables</see> to
    ''' populate a given <see cref="eGraphDataType">graph</see>.
    ''' </summary>
    ''' <param name="graph">The graph type to obtain variables for.</param>
    ''' <returns>The collection of <see cref="eVariableType">variables</see> to
    ''' populate a given <see cref="eGraphDataType">graph</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetVariables(graph As eGraphDataType) As eVariableType()

        Dim vars() As eVariableType = Nothing

        Select Case graph

            Case eGraphDataType.CostRevenue
                vars = New eVariableType() {eVariableType.RevenueTotal,
                                            eVariableType.Cost,
                                            eVariableType.Profit}

            Case eGraphDataType.Cost
                vars = New eVariableType() {eVariableType.CostAgriculture,
                                            eVariableType.CostInput,
                                            eVariableType.CostManagementRoyaltyCertification,
                                            eVariableType.CostManagementRoyaltyCertificationObservers,
                                            eVariableType.CostRawmaterial}

            Case eGraphDataType.Revenue
                vars = New eVariableType() {eVariableType.RevenueTickets,
                                            eVariableType.RevenueSubsidies,
                                            eVariableType.RevenueProductsMain,
                                            eVariableType.RevenueProductsOther,
                                            eVariableType.RevenueAgriculture}

            Case eGraphDataType.Jobs
                vars = New eVariableType() {eVariableType.NumberOfJobsTotal,
                                            eVariableType.NumberOfJobsMaleTotal,
                                            eVariableType.NumberOfJobsFemaleTotal}
            Case eGraphDataType.Dependents
                vars = New eVariableType() {eVariableType.NumberOfDependentsTotal,
                                            eVariableType.NumberOfWorkerDependents,
                                            eVariableType.NumberOfWorkerFemales,
                                            eVariableType.NumberOfWorkerMales,
                                            eVariableType.NumberOfOwnerMales,
                                            eVariableType.NumberOfOwnerFemales,
                                            eVariableType.NumberOfOwnerDependents}

            Case Else
                Debug.Assert(False)

        End Select
        Return vars

    End Function

    Public Shared Function GetVariableContributionType(var As eVariableType) As eContributionType

        Select Case var
            Case eVariableType.NumberOfDependentsTotal,
                eVariableType.NumberOfJobsFemaleTotal,
                eVariableType.NumberOfJobsMaleTotal,
                eVariableType.NumberOfJobsTotal,
                eVariableType.NumberOfOwnerDependents,
                eVariableType.NumberOfOwnerFemales,
                eVariableType.NumberOfOwnerMales,
                eVariableType.NumberOfWorkerDependents,
                eVariableType.NumberOfWorkerFemales,
                eVariableType.NumberOfWorkerMales,
                eVariableType.NumberOfWorkerOther,
                eVariableType.NumberOfWorkerPartTime
                Return eContributionType.Biomass
            Case eVariableType.Production,
                eVariableType.ProductionLive
                Return eContributionType.Biomass
        End Select

        Return eContributionType.Value

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset results by destroying all cached computated data in preparation
    ''' for a new search.
    ''' </summary>
    ''' <remarks>Call this method before starting a new search.</remarks>
    ''' -----------------------------------------------------------------------
    Public Sub Reset(runType As cModel.eRunTypes)

        Dim core As cCore = Me.m_data.Core
        Dim nNumUnits As Integer = Me.m_data.GetUnits(cUnitFactory.eUnitType.All).Length
        Dim nItems As Integer = Math.Max(Me.m_data.Core.nFleets, Me.m_data.Core.nGroups)

        Me.m_dtResultTimeStep.Clear()
        Me.m_dtSnapshots.Clear()
        Me.m_iMaxTimeStep = 0
        Me.m_iMaxItem = 0
        Me.m_runType = runType

        ReDim Me.m_ValueContribution(nItems, nNumUnits, Math.Max(1, core.nEcosimTimeSteps))
        ReDim Me.m_BiomassContribution(nItems, nNumUnits, Math.Max(1, core.nEcosimTimeSteps))
        ReDim Me.m_BiomassFlows(nNumUnits, nNumUnits)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store a value of a particular variable type for a particular unit
    ''' </summary>
    ''' <param name="unit">Unit to save variable for</param>
    ''' <param name="var">Type of the variable to save</param>
    ''' <param name="sValue">Value to save</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Store(unit As cUnit, var As eVariableType, sValue As Single, iTimeStep As Integer) As Boolean

        Try
            Me.m_iMaxTimeStep = Math.Max(Me.m_iMaxTimeStep, iTimeStep)
            Dim rs As cTimeStepResults = Me.GetTimeStepResult(iTimeStep, True)
            rs.Results(var, unit.Sequence) = sValue

        Catch ex As Exception
            Return False
        End Try
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Make a snapshot of a given time step, and store it under a given key.
    ''' </summary>
    ''' <param name="objKey">The key to store the snapshot for.</param>
    ''' <param name="iTimeStep">The time step to store a snapshot for.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function StoreSnapshot(objKey As Object, iTimeStep As Integer) As Boolean

        ' Why on earth are we cloning here?!?! 
        Dim tsr As cTimeStepResults = Me.GetTimeStepResult(iTimeStep, True).Clone
        Me.m_dtSnapshots(objKey) = tsr
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns list of all snapshot keys.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Snapshots() As Object()
        Get
            Dim lsnapshotKeys As New List(Of Object)
            For Each key As Object In Me.m_dtSnapshots.Keys
                lsnapshotKeys.Add(key)
            Next
            lsnapshotKeys.Sort()
            Return lsnapshotKeys.ToArray()
        End Get
    End Property

    Public Enum eContributionType
        Value
        Biomass
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get result for a given unit and variable at a given time step, optionally
    ''' filtered by item (fleet, group, ..).
    ''' </summary>
    ''' <param name="var"></param>
    ''' <param name="iTimeStep"></param>
    ''' <param name="unit"></param>
    ''' <param name="iItem"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Result(unit As cUnit,
                           var As eVariableType,
                           iTimeStep As Integer,
                           iItem As Integer,
                           contr As eContributionType) As Single

        Dim rs As cTimeStepResults = Me.GetTimeStepResult(iTimeStep, False)
        If (rs IsNot Nothing) Then

            Dim sValue As Single = rs.Results(var, unit.Sequence)
            Dim sContrVal As Single = 0
            Dim sContrBio As Single = 0

            Me.GetContributionRatios(iItem, unit, iTimeStep, sContrVal, sContrBio)

            Select Case contr
                Case eContributionType.Value
                    Return sValue * sContrVal
                Case eContributionType.Biomass
                    Return sValue * sContrBio
            End Select
        End If

        Return 0

    End Function

    Public Sub CalculateDerivedValues(iTimeStep As Integer)
        Dim rs As cTimeStepResults = Me.GetTimeStepResult(iTimeStep, False)
        If (rs IsNot Nothing) Then
            rs.CalculateDerivedValues()
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get result for a given unit and variable at a given snapshot.
    ''' </summary>
    ''' <param name="var"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function SnapshotValue(unit As cUnit, var As eVariableType, objKey As Object) As Single
        Dim tsr As cTimeStepResults = Me.GetSnapshot(objKey)
        If tsr IsNot Nothing Then Return tsr.Results(var, unit.Sequence)
        Return 0.0!
    End Function

    Public Shared Function GetVariables() As eVariableType()
        Return DirectCast([Enum].GetValues(GetType(eVariableType)), eVariableType())
    End Function

    Public Function NumTimeSteps() As Integer
        Return Me.m_iMaxTimeStep
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the run type that the results were populated for.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function RunType() As cModel.eRunTypes
        Return Me.m_runType
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the amount of a flow between a source and target unit. Dimensioned
    ''' as (source x target).
    ''' </summary>
    ''' <param name="iSource"><see cref="cUnit.Sequence"/> of source unit (donor).</param>
    ''' <param name="iTarget"><see cref="cUnit.Sequence"/> of target unit (recipient).</param>
    ''' -----------------------------------------------------------------------
    Public Property FlowsBiomass(iSource As Integer, iTarget As Integer) As Double
        Get
            Return Me.m_BiomassFlows(iSource, iTarget)
        End Get
        Set(value As Double)
            Me.m_BiomassFlows(iSource, iTarget) = value
        End Set
    End Property

#End Region ' Public access

#Region " Totals "

    Public Function GetSnapshotTotal(vartype As eVariableType,
                                    objKey As Object,
                                    Optional lUnits As cUnit() = Nothing) As Single
        Dim sTotal As Single = 0.0!

        If lUnits Is Nothing Then
            For Each unit As cUnit In Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
                sTotal += Me.SnapshotValue(unit, vartype, objKey)
            Next
        Else
            For Each unit As cUnit In lUnits
                sTotal += Me.SnapshotValue(unit, vartype, objKey)
            Next
        End If
        Return sTotal

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the total sum of a given variabe for a single time step.
    ''' </summary>
    ''' <param name="vartype"></param>
    ''' <param name="iTimeStep"></param>
    ''' <param name="lUnits"></param>
    ''' <param name="iItem">Aggreagation item index, if any.</param>
    ''' <param name="contr"><see cref="eContributionType"/> to extract contribution for.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function GetTimeStepTotal(vartype As eVariableType,
                                     iTimeStep As Integer,
                                     lUnits As cUnit(),
                                     iItem As Integer,
                                     contr As eContributionType) As Single

        Dim sTotal As Single = 0.0!

        If lUnits Is Nothing Then
            lUnits = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        End If

        For Each unit As cUnit In lUnits
            sTotal += Me.Result(unit, vartype, iTimeStep, iItem, contr)
        Next

        Return sTotal

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the total sum of a given variabe across all time steps.
    ''' </summary>
    ''' <param name="vartype">Variable to extract.</param>
    ''' <param name="lUnits">Units to extract total for.</param>
    ''' <param name="contr"><see cref="eContributionType">Contribution type</see>.</param>
    ''' <param name="iItem">Item to filter by.</param>
    ''' <returns>A total value.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetTotal(vartype As eVariableType,
                             Optional lUnits As cUnit() = Nothing,
                             Optional iItem As Integer = 0,
                             Optional contr As eContributionType = eContributionType.Value) As Single

        Dim sTotal As Single = 0.0!

        If lUnits Is Nothing Then
            lUnits = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        End If

        For iTimestep = 1 To Me.m_iMaxTimeStep
            For Each unit As cUnit In lUnits
                sTotal += Me.Result(unit, vartype, iTimestep, iItem, contr)
            Next
        Next iTimestep

        Return sTotal

    End Function

#End Region ' Totals

#Region " Internals "

    Private Function GetTimeStepResult(iTimeStep As Integer, bCreateIfMissing As Boolean) As cTimeStepResults

        Dim tsr As cTimeStepResults = Nothing
        If Not Me.m_dtResultTimeStep.ContainsKey(iTimeStep) Then
            If (bCreateIfMissing) Then
                tsr = New cTimeStepResults(Me.m_data, iTimeStep)
                Me.m_dtResultTimeStep.Add(iTimeStep, tsr)
            End If
        Else
            tsr = Me.m_dtResultTimeStep(iTimeStep)
        End If

        Return tsr

    End Function

    Private Function GetSnapshot(objKey As Object) As cTimeStepResults

        Dim tsr As cTimeStepResults = Nothing
        If Me.m_dtSnapshots.ContainsKey(objKey) Then Return Me.m_dtSnapshots(objKey)
        Return Nothing

    End Function

#End Region ' Internals

#Region " Contribution by item "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store the contribution of a single item to a unit at a given time step.
    ''' </summary>
    ''' <param name="iItem">The item to store the contribution for.</param>
    ''' <param name="unit">The unit to store the contribution for.</param>
    ''' <param name="iTimeStep">The time step to store the contribution for.</param>
    ''' <param name="sValueContribution">The value contribution to store.</param>
    ''' <param name="sBiomassContribution">The biomass contribution to store.</param>
    ''' <remarks>
    ''' The sum of contributions of all items should equal (or very,
    ''' very closely approximate) the value for the unit for the default chain.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub StoreContribution(iItem As Integer,
                                 unit As cUnit,
                                 iTimeStep As Integer,
                                 sValueContribution As Single,
                                 sBiomassContribution As Single)

        Dim bOkidoki As Boolean = False

        Select Case Me.RunType
            Case cModel.eRunTypes.Ecopath : bOkidoki = (iTimeStep = 1)
            Case cModel.eRunTypes.Ecosim : bOkidoki = (iTimeStep < Me.m_data.Core.nEcosimTimeSteps)
            Case cModel.eRunTypes.Equilibrium : bOkidoki = (iTimeStep < Me.m_data.Core.nEcosimTimeSteps)
        End Select

        If bOkidoki Then
            Try
                ' Append contribution in case this is called multiple times for a single ([fleet|group], unit combo)
                Me.m_ValueContribution(iItem, unit.Sequence, iTimeStep) += sValueContribution
                Me.m_BiomassContribution(iItem, unit.Sequence, iTimeStep) += sBiomassContribution
            Catch ex As Exception
                ' Whoah!
            End Try
        End If

        Me.m_iMaxItem = Math.Max(Me.m_iMaxItem, iItem)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the value ratio that a single item contributed for a given unit and 
    ''' time step, relative to the total value contribution for all items.
    ''' </summary>
    ''' <param name="iItem">Item to explore, 0 for all items.</param>
    ''' <param name="unit"></param>
    ''' <param name="iTimestep"></param>
    ''' -----------------------------------------------------------------------
    Public Sub GetContributionRatios(iItem As Integer,
                                     unit As cUnit,
                                     iTimestep As Integer,
                                     ByRef sValueContribution As Single,
                                     ByRef sBiomassContribution As Single)

        Dim sAllItemsValue As Single = 0 ' Value contribution for 'all fleets' calculation
        Dim sAllItemsBiomass As Single = 0 ' Biomass contribution for 'all fleets' calculation
        Dim sTotalValue As Single = 0 ' Total value contribution for fleets - should equal sAllFleet!
        Dim sTotalBiomass As Single = 0 ' Total biomass contribution for fleets - should equal sAllFleet!
        Dim sContrValue As Single = 0 ' Value contribution for a single fleet
        Dim sContrBiomass As Single = 0 ' Biomass contribution for a single fleet
        Dim bOkidoki As Boolean = False

        If (iItem = 0) Then
            sValueContribution = 1
            sBiomassContribution = 1
            Return
        End If

        sValueContribution = 0
        sBiomassContribution = 0

        Select Case Me.RunType
            Case cModel.eRunTypes.Ecopath : bOkidoki = (iTimestep = 1)
            Case cModel.eRunTypes.Ecosim : bOkidoki = (iTimestep < Me.m_data.Core.nEcosimTimeSteps)
            Case cModel.eRunTypes.Equilibrium : bOkidoki = (iTimestep < Me.m_data.Core.nEcosimTimeSteps)
        End Select

        If bOkidoki Then
            Try
                sAllItemsValue = Me.m_ValueContribution(0, unit.Sequence, iTimestep)
                sAllItemsBiomass = Me.m_BiomassContribution(0, unit.Sequence, iTimestep)

                For i As Integer = 1 To Me.m_iMaxItem
                    sTotalValue += Me.m_ValueContribution(i, unit.Sequence, iTimestep)
                    sTotalBiomass += Me.m_BiomassContribution(i, unit.Sequence, iTimestep)
                Next

                ' ************** VALIDATION ***************
                ' Contributions of all fleets [1..n] should equal the contribution of fleet 0
                'Debug.Assert(sAllFleetValue = sTotalValue, "Error: contribution of individual fleets does not match the contributions of all fleets.")
                'Debug.Assert(sAllFleetBiomass = sTotalBiomass, "Error: contribution of individual fleets does not match the contributions of all fleets.")
                ' ************** VALIDATION ***************

                sContrValue = Me.m_ValueContribution(iItem, unit.Sequence, iTimestep)
                sContrBiomass = Me.m_BiomassContribution(iItem, unit.Sequence, iTimestep)
            Catch ex As Exception
                Debug.Assert(False, "VC: Failure obtaining contribution for item")
            End Try
        End If

        ' Calc contributions
        If (sTotalValue > 0) Then
            sValueContribution = (sContrValue / sTotalValue)
        End If
        If (sTotalBiomass > 0) Then
            sBiomassContribution = (sContrBiomass / sTotalBiomass)
        End If

    End Sub

#End Region ' Contribution by item

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
