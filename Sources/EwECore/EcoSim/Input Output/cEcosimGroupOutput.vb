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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Results from EcoSim for a single group.
''' </summary>
''' <remarks>
''' This class wraps results from EcoSim for one group into a single object.
''' </remarks>
Public Class cEcosimGroupOutput
    Inherits cCoreGroupBase

#Region "Private Data"

    Private m_simData As cEcosimDatastructures
    'dictionary of vars and wrappers that directly access the core data
    Private m_coreData As New Dictionary(Of eVarNameFlags, IResultsWrapper)

#End Region

#Region "Constructor"

    Public Sub New(core As cCore, EcosimData As cEcosimDatastructures, iGroup As Integer)
        MyBase.New(core)

        Debug.Assert(core IsNot Nothing)
        Debug.Assert(EcosimData IsNot Nothing)

        Me.m_simData = EcosimData

        Dim val As cValue = Nothing

        Me.DBID = iGroup '????
        Me.Index = iGroup
        Me.m_dataType = eDataTypes.EcoSimGroupOutput

        'See Me.Init() for list of variables
        val = New cValue(core, 0, eVarNameFlags.EcosimGroupBiomassStart, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcosimGroupBiomassEnd, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcosimGroupCatchEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcosimGroupCatchStart, eStatusFlags.OK, eCoreCounterTypes.nFleets)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcosimGroupValueStart, eStatusFlags.OK, eCoreCounterTypes.nFleets)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcosimGroupValueEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, False, eVarNameFlags.EcosimIsCatchAggregated, eStatusFlags.OK, eValueTypes.Bool)
        Me.m_values.Add(val.varName, val)

    End Sub


    Public Sub Init()

        'the results arrays of ecosim are redim for each run
        'this means the reference to the results data is lost on each run 
        'so reset the reference
        Me.m_coreData.Clear()

        'jb 15-Nov-2010 Force the garbage collection on the memory that was released above
        GC.Collect()


        'cEcosimDataStrucures.ResultsOverTime(var,group,time) Var and Group are fixed
        Me.m_coreData.Add(eVarNameFlags.EcosimBiomass, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.Biomass, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimBiomassRel, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.BiomassRel, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimYield, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.Yield, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimYieldRel, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.YieldRel, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimFeedingTime, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.FeedingTime, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimConsumpBiomass, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.ConsumpBiomass, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimPredMort, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.PredMort, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimFishMort, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.FishMort, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimTotalMort, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.TotalMort, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimAvgWeight, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.AvgWeight, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimProdConsump, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.ProdConsump, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.TL, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.TL, Me.Index))

        Me.m_coreData.Add(eVarNameFlags.EcosimMortVPred, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.MortVPred, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimMortVFishing, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.MortVFishing, Me.Index))

        'cEcosimDataStrucures.ResultsAvgByPreyPred(var,group,time) Var and Group are fixed
        Me.m_coreData.Add(eVarNameFlags.EcosimAvgPred, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsAvgByPreyPred, cEcosimDatastructures.eEcosimPreyPredResults.Pred, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimAvgPrey, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsAvgByPreyPred, cEcosimDatastructures.eEcosimPreyPredResults.Prey, Me.Index))

        'cEcosimDataStrucures.PredPreyResultsOverTime(var,prey,pred,time) Var and Prey are fixed
        Me.m_coreData.Add(eVarNameFlags.EcosimPredConsumpTime, New c4DResultsWrapper(Me.m_simData.PredPreyResultsOverTime, cEcosimDatastructures.eEcosimPreyPredResults.Consumption, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimPreyPercentageTime, New c4DResultsWrapper(Me.m_simData.PredPreyResultsOverTime, cEcosimDatastructures.eEcosimPreyPredResults.Prey, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimPredRateTime, New c4DResultsWrapper(Me.m_simData.PredPreyResultsOverTime, cEcosimDatastructures.eEcosimPreyPredResults.Pred, Me.Index))

        Me.m_coreData.Add(eVarNameFlags.EcosimEcoSystemStruct, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.EcoSysStructure, Me.Index))

        ' EcosimEcoSystemStruct()
        'cEcosimDataStrucures.Elect(group,group,time) First Group is fixed
        Me.m_coreData.Add(eVarNameFlags.EcosimElectivityTime, New c3DResultsWrapper(Me.m_simData.Elect, Me.Index))

        Me.m_coreData.Add(eVarNameFlags.EcosimCatchGroupGear, New c3DResultsWrapper(Me.m_simData.ResultsSumCatchByGroupGear, Me.Index))

        'ResultsSumValueByGroupGear(Groups,Fleets,Time) the Zero fleet index is the sum across all fleets
        Me.m_coreData.Add(eVarNameFlags.EcosimValueGroup, New c3DResultsWrapper2Fixed(Me.m_simData.ResultsSumValueByGroupGear, Me.Index, 0))

        'ResultsSumValueByGroupGear(Groups,Fleets,Time) the Zero fleet index is the sum across all fleets
        Me.m_coreData.Add(eVarNameFlags.EcosimValueGroupRel, New c2DResultsWrapper(Me.m_simData.ResultsSumRelValueByGroup, Me.Index))

        'ResultsSumValueByGroupGear(Groups,Fleets,Time) the Zero fleet index is the sum across all fleets
        Me.m_coreData.Add(eVarNameFlags.EcosimValueGroupFleet, New c3DResultsWrapper(Me.m_simData.ResultsSumValueByGroupGear, Me.Index))

        'Fishing Mortality by group/fleet
        Me.m_coreData.Add(eVarNameFlags.EcosimFishingMortGroupGear, New c3DResultsWrapper(Me.m_simData.ResultsSumFMortByGroupGear, Me.Index))

        'Discards added 24-Oct-2016 as part of the Discardless project
        Me.m_coreData.Add(eVarNameFlags.EcosimLandingsGroupGear, New c3DResultsWrapper(Me.m_simData.ResultsTimeLandingsGroupGear, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimDiscardsGroupGear, New c3DResultsWrapper(Me.m_simData.ResultsTimeDiscardsGroupGear, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimDiscardsMortGroupGear, New c3DResultsWrapper(Me.m_simData.ResultsTimeDiscardsMortGroupGear, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimDiscardsSurvivedGroupGear, New c3DResultsWrapper(Me.m_simData.ResultsTimeDiscardsSurvivedGroupGear, Me.Index))

    End Sub

#End Region

#Region "Overridden base class methods"


    Public Overrides Function GetVariable(VarName As EwEUtils.Core.eVarNameFlags, Optional iIndex1 As Integer = -9999, Optional iIndex2 As Integer = -9999, Optional iIndex3 As Integer = cCore.NULL_VALUE) As Object

        If Not Me.m_coreData.ContainsKey(VarName) Then
            'NOT in list of sim vars so get the value from the base class GetVariable(...)
            Return MyBase.GetVariable(VarName, iIndex1, iIndex2)
        Else
            'Varname is access directly via the core data
            Return Me.m_coreData.Item(VarName).Value(iIndex1, iIndex2)
        End If

    End Function

#End Region

#Region "Status flag setting"

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In Me.m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray
                        For i = 1 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Str

                        If CStr(value.Value) = "" Then
                            value.Status = eStatusFlags.NotEditable Or eStatusFlags.Null
                        Else
                            value.Status = eStatusFlags.NotEditable Or eStatusFlags.OK
                        End If

                    Case Else
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                End Select

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

#End Region

#Region "Properties via dot operator"


    ''' <summary>
    ''' Is the catch on this group aggregated across all the fleets.
    ''' </summary>
    Public Property isCatchAggregated() As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcosimIsCatchAggregated))
        End Get

        Friend Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcosimIsCatchAggregated, value)
        End Set

    End Property

    ''' <summary>
    ''' Get the Biomass at a given time step.
    ''' </summary>
    ''' <param name="iTime">Time index</param>
    ''' <value>Single</value>
    Public ReadOnly Property Biomass(iTime As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimBiomass, iTime))
        End Get

    End Property

    ''' <summary>
    ''' Get the Trophic Level of a group at a given time step.
    ''' </summary>
    Public ReadOnly Property TL(iTime As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TL, iTime))
        End Get

    End Property

    ''' <summary>
    ''' Get the biomass relative to the base biomass at a given time step.
    ''' </summary>
    ''' <param name="iTime">Time index</param>
    ''' <value>Single</value>
    ''' <remarks> B(t)/B(0)</remarks>
    Public ReadOnly Property BiomassRel(iTime As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimBiomassRel, iTime))
        End Get

    End Property

    <Obsolete("Use Catch(i) instead")>
    Public ReadOnly Property Yield(iTime As Integer) As Single
        Get
            Return Me.Catch(iTime)
        End Get
    End Property

    <Obsolete("Use CatchRel(i) instead")>
    Public ReadOnly Property YieldRel(iTime As Integer) As Single
        Get
            Return Me.CatchRel(iTime)
        End Get
    End Property

    ''' <summary>
    ''' Get the total catch on this group at a given time step.
    ''' </summary>
    ''' <param name="iTime">Time index</param>
    ''' <remarks>Sum of catch across all fleets for this group</remarks>
    Public ReadOnly Property [Catch](iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimYield, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Get the total catch relative to the Ecopath inputs catch on this group at a given time step.
    ''' </summary>
    ''' <param name="iTime">Time index</param>
    Public ReadOnly Property CatchRel(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimYieldRel, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Total Catch by fleet for this group at a given time step. Includes discards the died.
    ''' </summary>
    ''' <param name="iFleetIndex">Fleet index</param>
    ''' <param name="iTime">Time index</param>
    Public ReadOnly Property CatchByFleet(iFleetIndex As Integer, iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimCatchGroupGear, iFleetIndex, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Fishing Mortality by Fleet at a given time step.
    ''' </summary>
    ''' <param name="iFleetIndex">Fleet inded</param>
    ''' <param name="iTime">Time index</param>
    ''' <value>Single</value>
    ''' <returns>Fishing Mortality on this group caused by a fleet</returns>
    Public ReadOnly Property FishingMortByFleet(iFleetIndex As Integer, iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFishingMortGroupGear, iFleetIndex, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Get consumption over biomass at a given time step.
    ''' </summary>
    ''' <param name="iTime"></param>
    Public ReadOnly Property ConsumpBiomass(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimConsumpBiomass, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Get the feeding time at a given time step.
    ''' </summary>
    ''' <param name="iTime"></param>
    Public ReadOnly Property FeedingTime(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFeedingTime, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Get the predation mortality at a given time step.
    ''' </summary>
    ''' <param name="iTime"></param>
    Public ReadOnly Property PredMort(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimPredMort, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Get the Predation mortality + fishing mortality at a given time step.
    ''' </summary>
    Public ReadOnly Property FishMort(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFishMort, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Sum of all mortality for this group at a given time step.
    ''' </summary>
    ''' <param name="iTime">Time index</param>
    ''' <remarks>Fishing mort + Predation mort + Natural mort</remarks>
    Public ReadOnly Property TotalMort(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimTotalMort, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Production / Consumption (Ecopath GE) at a given time step.
    ''' </summary>
    Public ReadOnly Property ProdConsump(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimProdConsump, iTime))
        End Get
    End Property

    Public ReadOnly Property AvgWeight(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimAvgWeight, iTime))
        End Get
    End Property

    ''' <summary>
    '''  Predation / total loss rate  [Eatenof(i) / (loss(i) / B(i))]
    ''' </summary>
    Public ReadOnly Property MortVPred(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimMortVPred, iTime))
        End Get
    End Property

    ''' <summary>
    ''' Catch / total loss rate [B(i) * F(i) / (loss(i) / b(i))
    ''' </summary>
    Public ReadOnly Property MortVFishing(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimMortVFishing, iTime))
        End Get
    End Property

    Public ReadOnly Property EcoSystemStruct(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimEcoSystemStruct, iTime))
        End Get
    End Property

    Public ReadOnly Property AvgPredConsumption(iGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimAvgPred, iGroup))
        End Get
    End Property

    Public ReadOnly Property AvgPreyConsumption(igroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimAvgPrey, igroup))
        End Get
    End Property

    Public ReadOnly Property Value(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimValueGroup, iTime))
        End Get
    End Property

    Public ReadOnly Property ValueRel(iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimValueGroupRel, iTime))
        End Get
    End Property

    Public ReadOnly Property ValueByFleet(iFleetIndex As Integer, iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimValueGroupFleet, iFleetIndex, iTime))
        End Get
    End Property

    Public ReadOnly Property DiscardMortByFleet(iFleetIndex As Integer, iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimDiscardsMortGroupGear, iFleetIndex, iTime))
        End Get
    End Property

    Public ReadOnly Property DiscardByFleet(iFleetIndex As Integer, iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimDiscardsGroupGear, iFleetIndex, iTime))
        End Get
    End Property

    Public ReadOnly Property DiscardSurvivedByFleet(iFleetIndex As Integer, iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimDiscardsSurvivedGroupGear, iFleetIndex, iTime))
        End Get
    End Property


    Public ReadOnly Property LandingsByFleet(iFleetIndex As Integer, iTime As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimLandingsGroupGear, iFleetIndex, iTime))
        End Get
    End Property


#End Region

#Region "Variables arrayed by group and time"

    ''' <summary>
    ''' Percentage of a group this group consumes
    ''' </summary>
    ''' <param name="iPreyGroup">Index of group that this group preys on</param>
    ''' <param name="iTime">Ecosim time step</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property PreyPercentage(iPreyGroup As Integer, iTime As Integer) As Single
        Get
            Try
                Return CSng(Me.GetVariable(eVarNameFlags.EcosimPreyPercentageTime, iPreyGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".PreyPercentage() " & ex.Message)
            End Try
        End Get

    End Property

    ''' <summary>
    ''' Predation rate on this prey by a pred 
    ''' </summary>
    ''' <param name="iPredGroup">Index of group that predates on this group</param>
    ''' <param name="iTime">Ecosim time step</param>
    ''' <value></value>
    ''' <returns>Predation on this group</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Predation(iPredGroup As Integer, iTime As Integer) As Single
        Get
            Try
                Return CSng(Me.GetVariable(eVarNameFlags.EcosimPredRateTime, iPredGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".Predation() " & ex.Message)
            End Try
        End Get

    End Property


    Public ReadOnly Property Consumption(iPredGroup As Integer, iTime As Integer) As Single
        Get
            Try
                Return CSng(Me.GetVariable(eVarNameFlags.EcosimPredConsumpTime, iPredGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".Consumption() " & ex.Message)
            End Try
        End Get

    End Property

    Public ReadOnly Property Electivity(iPredGroup As Integer, iTime As Integer) As Single
        Get
            Try
                Return CSng(Me.GetVariable(eVarNameFlags.EcosimElectivityTime, iPredGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".Electivity() " & ex.Message)
            End Try
        End Get

    End Property

#End Region

#Region "Summary values"

    Public Property BiomassStart() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimGroupBiomassStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimGroupBiomassStart, value)
        End Set
    End Property

    Public Property BiomassEnd() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimGroupBiomassEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimGroupBiomassEnd, value)
        End Set
    End Property


    Public Property CatchStart(iFleet As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimGroupCatchStart, iFleet))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimGroupCatchStart, value, iFleet)
        End Set
    End Property


    Public Property CatchEnd(iFleet As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimGroupCatchEnd, iFleet))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimGroupCatchEnd, value, iFleet)
        End Set
    End Property


    Public Property ValueStart(iFleet As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimGroupValueStart, iFleet))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimGroupValueStart, value, iFleet)
        End Set
    End Property

    Public Property ValueEnd(iFleet As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimGroupValueEnd, iFleet))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimGroupValueEnd, value, iFleet)
        End Set
    End Property

#End Region

#Region " Deprecated "

    <Obsolete("Use cEcopathGroupInput.IsPred() instead")>
    Public ReadOnly Property isPred(iGroup As Integer) As Boolean
        Get
            Return Me.m_core.EcopathGroupInputs(Me.Index).IsPred(iGroup)
        End Get
    End Property

    <Obsolete("Use cEcopathGroupInput.IsPrey() instead")>
    Public ReadOnly Property isPrey(iGroup As Integer) As Boolean
        Get
            Return Me.m_core.EcopathGroupInputs(Me.Index).IsPrey(iGroup)
        End Get
    End Property

#End Region ' Deprecated

End Class
