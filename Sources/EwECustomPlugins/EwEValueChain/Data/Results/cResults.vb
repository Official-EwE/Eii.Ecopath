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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.SystemUtilities.cSystemUtils

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Computed ECost results.
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

        Public Sub New(ByVal data As cData, ByVal iTimeStep As Integer)
            Me.m_data = data
            Me.m_iTimeStep = iTimeStep
            ReDim Me.m_results([Enum].GetNames(GetType(eVariableType)).Length, Me.m_data.UnitCount)
        End Sub

        Public Property Results(ByVal iVar As Integer, ByVal iUnit As Integer) As Single
            Get
                Return Me.m_results(iVar, iUnit)
            End Get
            Set(ByVal value As Single)
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

                'Dim isBroker As Boolean = False
                'If DirectCast(unit, cEconomicUnit).Broker = True Then isBroker = True

                sRevenueProductsOther = Me.m_results(eVariableType.RevenueProductsOther, unit.Sequence) + _
                        Me.m_results(eVariableType.RevenueAgriculture, unit.Sequence)

                sRevenueTickets = Me.m_results(eVariableType.RevenueTickets, unit.Sequence)

                sRevenue = sRevenueProductsOther + sRevenueTickets + _
                        Me.m_results(eVariableType.RevenueSubsidies, unit.Sequence)

                'If isBroker = False Then  'this is not a broker, so the revenus from selling the product is theirs, and it counts in the utility
                sRevenue += Me.m_results(eVariableType.RevenueProductsMain, unit.Sequence)
                'End If
                ' Cost
                sCostSalariesShares = Me.m_results(eVariableType.CostWorker, unit.Sequence) + _
                        Me.m_results(eVariableType.CostOwner, unit.Sequence)

                sCostManagementRoyaltyCertificationObserver = Me.m_results(eVariableType.CostManagementRoyaltyCertification, unit.Sequence) + _
                        Me.m_results(eVariableType.CostObserver, unit.Sequence)

                sCostlInputOther = Me.m_results(eVariableType.CostAgriculture, unit.Sequence) + _
                        Me.m_results(eVariableType.CostInput, unit.Sequence)

                sCost = sCostSalariesShares + _
                        sCostlInputOther + _
                        Me.m_results(eVariableType.CostTaxes, unit.Sequence) + _
                        sCostManagementRoyaltyCertificationObserver

                'If isBroker = False Then 'it is not a broker, so they pay for the fish: (and it is added to the utility)
                sCost += Me.m_results(eVariableType.CostRawmaterial, unit.Sequence)
                'End If

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
                sTotalUtility = IIf(sProfit < 0, sCost, sRevenue)

                ' Jobs
                sTotalJobsMale = Me.m_results(eVariableType.NumberOfWorkerMales, unit.Sequence) + _
                        Me.m_results(eVariableType.NumberOfOwnerMales, unit.Sequence)
                sTotalJobsFemale = Me.m_results(eVariableType.NumberOfWorkerFemales, unit.Sequence) + _
                        Me.m_results(eVariableType.NumberOfOwnerFemales, unit.Sequence)
                sTotalJobs = sTotalJobsFemale + sTotalJobsMale

                ' Dependents, total
                sDependentsTotal = Me.m_results(eVariableType.NumberOfOwnerDependents, unit.Sequence) + _
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

    ''' <summary>Contributions of a fleet to a unit per timestep to the total value.</summary>
    ''' <remarks>Indexed as (fleet, time step, unit sequence).</remarks>
    Private m_asFleetValueContribution As Single(,,)
    ''' <summary>Contributions of a fleet to a unit per timestep to the total biomass.</summary>
    ''' <remarks>Indexed as (fleet, time step, unit sequence).</remarks>
    Private m_asFleetBiomassContribution As Single(,,)

    ''' <summary>Max no of time steps</summary>
    Private m_iMaxTimeStep As Integer = 0
    ''' <summary>Run type that results were computed for.</summary>
    Private m_runType As cModel.eRunTypes = cModel.eRunTypes.Ecopath

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
        Profit
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

    End Enum

#End Region ' Public enums

#Region " Construction "

    Public Sub New(ByVal data As cData)
        Me.m_data = data
    End Sub

#End Region ' Construction

#Region " Public access "

    Public Shared Function GetVariableContributionType(var As eVariableType) As eContributionType

        Select Case var
            Case eVariableType.NumberOfDependentsTotal, _
                eVariableType.NumberOfJobsFemaleTotal, _
                eVariableType.NumberOfJobsMaleTotal, _
                eVariableType.NumberOfJobsTotal, _
                eVariableType.NumberOfOwnerDependents, _
                eVariableType.NumberOfOwnerFemales, _
                eVariableType.NumberOfOwnerMales, _
                eVariableType.NumberOfWorkerDependents, _
                eVariableType.NumberOfWorkerFemales, _
                eVariableType.NumberOfWorkerMales, _
                eVariableType.NumberOfWorkerOther, _
                eVariableType.NumberOfWorkerPartTime
                Return eContributionType.Biomass
            Case eVariableType.Production, _
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
    Public Sub Reset(ByVal runType As cModel.eRunTypes)

        Dim core As cCore = Me.m_data.Core
        Dim nNumUnits As Integer = Me.m_data.GetUnits(cUnitFactory.eUnitType.All).Length

        Me.m_dtResultTimeStep.Clear()
        Me.m_dtSnapshots.Clear()
        Me.m_iMaxTimeStep = 0
        Me.m_runType = runType

        ReDim Me.m_asFleetValueContribution(core.nFleets, nNumUnits, Math.Max(1, core.nEcosimTimeSteps))
        ReDim Me.m_asFleetBiomassContribution(core.nFleets, nNumUnits, Math.Max(1, core.nEcosimTimeSteps))

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store a value of a particular variable type for a particular unit
    ''' </summary>
    ''' <param name="unit">Unit to save variable for</param>
    ''' <param name="var">Type of the variable to save</param>
    ''' <param name="sValue">Value to save</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Store(ByVal unit As cUnit, _
                          ByVal var As eVariableType, _
                          ByVal sValue As Single, _
                          ByVal iTimeStep As Integer) As Boolean

        Try

            Me.m_iMaxTimeStep = Math.Max(Me.m_iMaxTimeStep, iTimeStep)
            Dim rs As cTimeStepResults = Me.GetTimeStepResult(iTimeStep)
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
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function StoreSnapshot(ByVal objKey As Object, ByVal iTimeStep As Integer) As Boolean

        Dim tsr As cTimeStepResults = Me.GetTimeStepResult(iTimeStep).Clone
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all results for a given unit type and variable type
    ''' </summary>
    ''' <param name="unitType"></param>
    ''' <param name="var"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Results(ByVal unitType As cUnitFactory.eUnitType, _
                            ByVal var As eVariableType, _
                            ByVal iTimeStep As Integer, _
                            ByVal iFleet As Integer) As Single()

        Dim ls As New List(Of Single)
        For Each unit As cUnit In Me.m_data.GetUnits(unitType)
            ls.Add(Me.GetTimeStepResult(iTimeStep).Results(var, unit.Sequence))
        Next
        Return ls.ToArray()

    End Function

    Public Enum eContributionType
        Value
        Biomass
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get result for a given unit and variable at a given time step, optionally
    ''' filtered by fleet.
    ''' </summary>
    ''' <param name="var"></param>
    ''' <param name="iTimeStep"></param>
    ''' <param name="unit"></param>
    ''' <param name="iFleet"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Result(ByVal unit As cUnit, _
                           ByVal var As eVariableType, _
                           ByVal iTimeStep As Integer, _
                           ByVal iFleet As Integer, _
                           ByVal contr As eContributionType) As Single

        Dim rs As cTimeStepResults = Me.GetTimeStepResult(iTimeStep)
        Dim sValue As Single = rs.Results(var, unit.Sequence)
        Dim sContrVal As Single = 0
        Dim sContrBio As Single = 0

        Me.GetFleetContributionRatios(iFleet, unit, iTimeStep, sContrVal, sContrBio)

        Select Case contr
            Case eContributionType.Value
                Return sValue * sContrVal
            Case eContributionType.Biomass
                Return sValue * sContrBio
        End Select

    End Function

    Public Sub CalculateDerivedValues(ByVal iTimeStep As Integer)
        Me.GetTimeStepResult(iTimeStep).CalculateDerivedValues()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get result for a given unit and variable at a given snapshot.
    ''' </summary>
    ''' <param name="var"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function SnapshotValue(ByVal unit As cUnit, ByVal var As eVariableType, ByVal objKey As Object) As Single
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

#End Region ' Public access

#Region " Totals "

    Public Function GetSnapshotTotal(ByVal vartype As eVariableType, _
                                    ByVal objKey As Object, _
                                    Optional ByVal lUnits As cUnit() = Nothing) As Single
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
    ''' <param name="iFleet">Fleet to extract contribution for.</param>
    ''' <param name="contr"><see cref="eContributionType"/> to extract contribution for.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function GetTimeStepTotal(ByVal vartype As eVariableType, _
                                     ByVal iTimeStep As Integer, _
                                     ByVal lUnits As cUnit(), _
                                     ByVal iFleet As Integer, _
                                     ByVal contr As eContributionType) As Single

        Dim sTotal As Single = 0.0!

        If lUnits Is Nothing Then
            lUnits = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        End If

        For Each unit As cUnit In lUnits
            sTotal += Me.Result(unit, vartype, iTimeStep, iFleet, contr)
        Next

        Return sTotal

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the total sum of a given variabe across all time steps.
    ''' </summary>
    ''' <param name="vartype"></param>
    ''' <param name="lUnits"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function GetTotal(ByVal vartype As eVariableType, _
                             Optional ByVal lUnits As cUnit() = Nothing, _
                             Optional ByVal iFleet As Integer = 0, _
                             Optional contr As eContributionType = eContributionType.Value) As Single

        Dim sTotal As Single = 0.0!

        If lUnits Is Nothing Then
            lUnits = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        End If

        For iTimestep = 0 To Me.m_iMaxTimeStep
            For Each unit As cUnit In lUnits
                sTotal += Me.Result(unit, vartype, iTimestep, iFleet, contr)
            Next
        Next iTimestep

        Return sTotal

    End Function

#End Region ' Totals

#Region " Internals "

    Private Function GetTimeStepResult(ByVal iTimeStep As Integer) As cTimeStepResults

        Dim tsr As cTimeStepResults = Nothing
        If Not Me.m_dtResultTimeStep.ContainsKey(iTimeStep) Then
            tsr = New cTimeStepResults(Me.m_data, iTimeStep)
            m_dtResultTimeStep.Add(iTimeStep, tsr)
        Else
            tsr = Me.m_dtResultTimeStep(iTimeStep)
        End If

        Return tsr

    End Function

    Private Function GetSnapshot(ByVal objKey As Object) As cTimeStepResults

        Dim tsr As cTimeStepResults = Nothing
        If Me.m_dtSnapshots.ContainsKey(objKey) Then Return Me.m_dtSnapshots(objKey)
        Return Nothing

    End Function

#End Region ' Internals

#Region " EXPERIMENTAL "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store the contribution of a single fleet to a unit at a given time step.
    ''' </summary>
    ''' <param name="iFleet">The fleet to store the contribution for.</param>
    ''' <param name="unit">The unit to store the contribution for.</param>
    ''' <param name="iTimeStep">The time step to store the contribution for.</param>
    ''' <param name="sValueContribution">The value contribution to store.</param>
    ''' <param name="sBiomassContribution">The biomass contribution to store.</param>
    ''' <remarks>
    ''' The sum of contributions of all fleets [1..n] should equal (or very,
    ''' very closely approximate) the value for the unit for the default chain.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub StoreFleetContribution(ByVal iFleet As Integer, _
                                      ByVal unit As cUnit, _
                                      ByVal iTimeStep As Integer, _
                                      ByVal sValueContribution As Single, _
                                      ByVal sBiomassContribution As Single)

        Dim bOkidoki As Boolean = False

        Select Case Me.RunType
            Case cModel.eRunTypes.Ecopath : bOkidoki = (iTimeStep = 1)
            Case cModel.eRunTypes.Ecosim : bOkidoki = (iTimeStep < Me.m_data.Core.nEcosimTimeSteps)
            Case cModel.eRunTypes.Equilibrium : bOkidoki = (iTimeStep < Me.m_data.Core.nEcosimTimeSteps)
        End Select

        If bOkidoki Then
            Try
                ' Append contribution in case this is called multiple times for a single fleet + unit combo
                Me.m_asFleetValueContribution(iFleet, unit.Sequence, iTimeStep) += sValueContribution
                Me.m_asFleetBiomassContribution(iFleet, unit.Sequence, iTimeStep) += sBiomassContribution
            Catch ex As Exception
                ' Whoah!
            End Try
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the value ratio that a single fleet contributed for a given unit and 
    ''' time step, relative to the total value contribution for all fleets.
    ''' </summary>
    ''' <param name="iFleet"></param>
    ''' <param name="unit"></param>
    ''' <param name="iTimestep"></param>
    ''' -----------------------------------------------------------------------
    Public Sub GetFleetContributionRatios(ByVal iFleet As Integer, _
                                          ByVal unit As cUnit, _
                                          ByVal iTimestep As Integer, _
                                          ByRef sValueContribution As Single, _
                                          ByRef sBiomassContribution As Single)

        Dim sAllFleetValue As Single = 0 ' Value contribution for 'all fleets' calculation
        Dim sAllFleetBiomass As Single = 0 ' Biomass contribution for 'all fleets' calculation
        Dim sTotalValue As Single = 0 ' Total value contribution for fleets - should equal sAllFleet!
        Dim sTotalBiomass As Single = 0 ' Total biomass contribution for fleets - should equal sAllFleet!
        Dim sContrValue As Single = 0 ' Value contribution for a single fleet
        Dim sContrBiomass As Single = 0 ' Biomass contribution for a single fleet
        Dim bOkidoki As Boolean = False

        If (iFleet = 0) Then
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
                sAllFleetValue = Me.m_asFleetValueContribution(0, unit.Sequence, iTimestep)
                sAllFleetBiomass = Me.m_asFleetBiomassContribution(0, unit.Sequence, iTimestep)

                ' ************** VALIDATION ***************
                ' Constributions of all fleets [1..n] should equal the contribution of fleet 0
                For i As Integer = 1 To Me.m_data.Core.nFleets
                    sTotalValue += Me.m_asFleetValueContribution(i, unit.Sequence, iTimestep)
                    sTotalBiomass += Me.m_asFleetBiomassContribution(i, unit.Sequence, iTimestep)
                Next
                'Debug.Assert(sAllFleetValue = sTotalValue, "Error: contribution of individual fleets does not match the contributions of all fleets.")
                'Debug.Assert(sAllFleetBiomass = sTotalBiomass, "Error: contribution of individual fleets does not match the contributions of all fleets.")
                ' ************** VALIDATION ***************

                sContrValue = Me.m_asFleetValueContribution(iFleet, unit.Sequence, iTimestep)
                sContrBiomass = Me.m_asFleetBiomassContribution(iFleet, unit.Sequence, iTimestep)
            Catch ex As Exception
                Debug.Assert(False, "VC: Failure obtaining contribution for fleet")
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

#End Region ' EXPERIMENTAL

End Class
