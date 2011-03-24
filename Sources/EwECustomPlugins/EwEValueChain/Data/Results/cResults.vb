#Region " Imports "
Option Strict On
Imports System.ComponentModel
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style


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

                sRevenueProductsOther = Me.m_results(eVariableType.RevenueProductsOther, unit.Sequence) + _
                        Me.m_results(eVariableType.RevenueAgriculture, unit.Sequence)

                sRevenueTickets = Me.m_results(eVariableType.RevenueTickets, unit.Sequence)

                sRevenue = sRevenueProductsOther + sRevenueTickets + _
                        Me.m_results(eVariableType.RevenueProductsMain, unit.Sequence) + _
                        Me.m_results(eVariableType.RevenueSubsidies, unit.Sequence)

                ' Cost
                sCostSalariesShares = Me.m_results(eVariableType.CostWorker, unit.Sequence) + _
                        Me.m_results(eVariableType.CostOwner, unit.Sequence)

                sCostManagementRoyaltyCertificationObserver = Me.m_results(eVariableType.CostManagementRoyaltyCertification, unit.Sequence) + _
                        Me.m_results(eVariableType.CostObserver, unit.Sequence)

                sCostlInputOther = Me.m_results(eVariableType.CostAgriculture, unit.Sequence) + _
                        Me.m_results(eVariableType.CostInput, unit.Sequence)

                sCost = sCostSalariesShares + _
                        Me.m_results(eVariableType.CostRawmaterial, unit.Sequence) + _
                        sCostlInputOther + _
                        Me.m_results(eVariableType.CostTaxes, unit.Sequence) + _
                        sCostManagementRoyaltyCertificationObserver

                ' Profit
                sProfit = sRevenue - sCost

                ' TotalUtility a.k.a. Throughput = cost when (profit < 0), revenue otherwise
                sTotalUtility = CSng(IIf(sProfit < 0, sCost, sRevenue))

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

    ''' <summary>Contributions of a fleet to a unit per timestep.</summary>
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
        ''' <remarks></remarks>
        Production

        ''' <summary> Production of fish products in corresponding live weight </summary>
        ''' <remarks></remarks>
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset results by destroying all cached computated data in preparation
    ''' for a new search.
    ''' </summary>
    ''' <remarks>Call this method before starting a new search.</remarks>
    ''' -----------------------------------------------------------------------
    Public Sub Reset(ByVal runType As cModel.eRunTypes)

        Dim core As cCore = Me.m_data.Core
        Dim nNumUnits As Integer = Me.m_data.GetUnits(cUnitFactory.eUnitType.All).Count

        Me.m_dtResultTimeStep.Clear()
        Me.m_dtSnapshots.Clear()
        Me.m_iMaxTimeStep = 0
        Me.m_runType = runType

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
            Me.GetTimeStepResult(iTimeStep).Results(var, unit.Sequence) = sValue
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
                           ByVal iFleet As Integer) As Single

        Return Me.GetTimeStepResult(iTimeStep).Results(var, unit.Sequence) * _
               Me.GetFleetContributionRatio(iFleet, unit, iTimeStep)

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
                                    Optional ByVal lUnits As List(Of cUnit) = Nothing) As Single
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
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function GetTimeStepTotal(ByVal vartype As eVariableType, _
                             Optional ByVal iTimeStep As Integer = 1, _
                             Optional ByVal lUnits As List(Of cUnit) = Nothing, _
                             Optional ByVal iFleet As Integer = 0) As Single

        Dim sTotal As Single = 0.0!

        If lUnits Is Nothing Then
            lUnits = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        End If

        For Each unit As cUnit In lUnits
            sTotal += Me.Result(unit, vartype, iTimeStep, iFleet)
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
                             Optional ByVal lUnits As List(Of cUnit) = Nothing, _
                             Optional ByVal iFleet As Integer = 0) As Single

        Dim sTotal As Single = 0.0!

        If lUnits Is Nothing Then
            lUnits = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        End If

        For iTimestep = 0 To Me.m_iMaxTimeStep
            For Each unit As cUnit In lUnits
                sTotal += Me.Result(unit, vartype, iTimestep, iFleet)
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
    ''' <param name="sContribution">The contribution to store.</param>
    ''' <remarks>
    ''' The sum of contributions of all fleets [1..n] should equal (or very,
    ''' very closely approximate) the value for the unit for the default chain.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub StoreFleetContribution(ByVal iFleet As Integer, _
                                      ByVal unit As cUnit, _
                                      ByVal iTimeStep As Integer, _
                                      ByVal sContribution As Single)

        Dim bOkidoki As Boolean = False

        Select Case Me.RunType
            Case cModel.eRunTypes.Ecopath : bOkidoki = (iTimeStep = 1)
            Case cModel.eRunTypes.Ecosim : bOkidoki = (iTimeStep < Me.m_data.Core.nEcosimTimeSteps)
            Case cModel.eRunTypes.Equilibrium : bOkidoki = (iTimeStep < Me.m_data.Core.nEcosimTimeSteps)
        End Select

        If bOkidoki Then
            Try
                Me.m_asFleetBiomassContribution(iFleet, unit.Sequence, iTimeStep) = sContribution
            Catch ex As Exception
                ' Whoah!
            End Try
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the value * biomass ratio that a single fleet contributed for a 
    ''' single unit and time step, relative to the total contribution for all 
    ''' fleets.
    ''' </summary>
    ''' <param name="iFleet"></param>
    ''' <param name="unit"></param>
    ''' <param name="iTimestep"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function GetFleetContributionRatio(ByVal iFleet As Integer, _
                                              ByVal unit As cUnit, _
                                              ByVal iTimestep As Integer) As Single

        Dim sTotal As Single = 0 ' Total contribution for all fleets
        Dim sContr As Single = 0 ' COntribution for a single fleet

        If (iFleet = 0) Then Return 1

        Dim bOkidoki As Boolean = False

        Select Case Me.RunType
            Case cModel.eRunTypes.Ecopath : bOkidoki = (iTimestep = 1)
            Case cModel.eRunTypes.Ecosim : bOkidoki = (iTimestep < Me.m_data.Core.nEcosimTimeSteps)
            Case cModel.eRunTypes.Equilibrium : bOkidoki = (iTimestep < Me.m_data.Core.nEcosimTimeSteps)
        End Select

        If bOkidoki Then
            Try
                sTotal = Me.m_asFleetBiomassContribution(0, unit.Sequence, iTimestep)
                sContr = Me.m_asFleetBiomassContribution(iFleet, unit.Sequence, iTimestep)
            Catch ex As Exception
                Debug.Assert(False, "VC: Failure obtaining contribution for fleet")
            End Try
        End If

        ' Any contribution?
        If ((sTotal > 0) And (sContr > 0)) Then
            ' #Yes: do the math
            Return (sContr / sTotal)
        Else
            ' #No: return 0
            Return 0
        End If

    End Function

#End Region ' EXPERIMENTAL

End Class
