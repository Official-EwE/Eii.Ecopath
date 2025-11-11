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

Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace SearchObjectives

#Region "ISearchObjective definition "

    ''' <summary>
    ''' Interface for shared search variables
    ''' </summary>
    ''' <remarks>The Fishing Policy Search, Ecoseed and MSE all share some base variables. This interface provides a consistance interface for accessing these variables.</remarks>
    Public Interface ISearchObjective

        Function Init(ByRef theCore As cCore) As Boolean
        Function Update(DataType As eDataTypes) As Boolean
        Function Load() As Boolean
        Sub Clear()

        '''' <summary>
        '''' Stop a running process
        '''' </summary>
        '''' <param name="WaitTimeinMillSec">Length of time in milliseconds to wait for the process to complete, -1 wait indefinitely.  </param>
        '''' <returns>True if the process was stop within the wait time, False if it timed out.</returns>
        '''' <remarks></remarks>
        'Function StopRun(Optional WaitTimeinMillSec As Integer = -1) As Boolean

        ReadOnly Property ValueWeights() As cSearchObjectiveWeights
        ReadOnly Property GroupObjectives(iGroup As Integer) As cSearchObjectiveGroupInput
        ReadOnly Property FleetObjectives(iGroup As Integer) As cSearchObjectiveFleetInput
        ReadOnly Property ObjectiveParameters() As cSearchObjectiveParameters

    End Interface

#End Region

#Region "cSearchObjective implementation"

    ''' <summary>
    ''' Implementation of ISearchObjective that is used by all classes that implement the ISearchObjective interface.
    ''' </summary>
    ''' <remarks>
    ''' An single instance of this class is created by the core and made available via cCore.SearchObjective(). 
    ''' All objects that implement the ISearchObjective interface do so by sharing the same instance of cCore.SearchObjective(). 
    ''' This allows the data to be synced between different objects at all times.
    ''' </remarks>
    Public Class cSearchObjective
        Implements ISearchObjective

        Protected m_core As cCore
        Protected m_valWeights As cSearchObjectiveWeights
        Protected m_parameters As cSearchObjectiveParameters
        Private m_lstGroups As New cCoreInputOutputList(Of cSearchObjectiveGroupInput)(eDataTypes.SearchObjectiveGroupInput, 1)
        Private m_lstFleets As New cCoreInputOutputList(Of cSearchObjectiveFleetInput)(eDataTypes.SearchObjectiveFleetInput, 1)
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cSearchObjective)()

        ''' <summary>
        ''' Build interface objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Overridable Function Init(ByRef theCore As cCore) As Boolean Implements ISearchObjective.Init
            Try

                Me.m_core = theCore

                Me.m_valWeights = New cSearchObjectiveWeights(Me.m_core)
                Me.m_parameters = New cSearchObjectiveParameters(Me.m_core)

                'Init the search data
                Dim search As cSearchDatastructures = Me.m_core.m_SearchData

                'sets BGoalValue() as a function of PB from last ecopath run
                search.SetDefaultBGoal(Me.m_core.m_EcopathData.PB)
                'discount factor, FLimit, Default F rates
                search.SetDefaultOptimizationValues()

                'default weights
                search.ValWeight(eValueWeightTypes.NetEcomValue) = 1

                Me.m_lstGroups.Clear()
                Dim grp As cSearchObjectiveGroupInput
                For igrp As Integer = 1 To Me.m_core.nGroups
                    'use the database ID for the Ecopath Groups
                    grp = New cSearchObjectiveGroupInput(Me.m_core, Me.m_core.m_EcopathData.GroupDBID(igrp))
                    Me.m_lstGroups.Add(grp)
                Next


                Me.m_lstFleets.Clear()
                Dim flt As cSearchObjectiveFleetInput
                For iflt As Integer = 1 To Me.m_core.nFleets
                    'use the database ID for the Fleets
                    flt = New cSearchObjectiveFleetInput(Me.m_core, Me.m_core.m_EcopathData.FleetDBID(iflt))
                    Me.m_lstFleets.Add(flt)
                Next

                'set the search back to false 
                search.SearchMode = eSearchModes.NotInSearch

                Return True

            Catch ex As Exception
                m_logger.LogError(ex, "cSearchObjective.Init() Exception")
                Return False
            End Try

        End Function

        Friend Overridable Function Load() As Boolean Implements ISearchObjective.Load

            Try
                Dim igrp As Integer
                Dim iflt As Integer

                Dim coreData As cSearchDatastructures = Me.m_core.m_SearchData

                'values weights
                Me.m_valWeights.AllowValidation = False
                Me.m_valWeights.EconomicWeight = coreData.ValWeight(eValueWeightTypes.NetEcomValue)
                Me.m_valWeights.EcoSystemWeight = coreData.ValWeight(eValueWeightTypes.EcoStructure)
                Me.m_valWeights.MandatedRebuildingWeight = coreData.ValWeight(eValueWeightTypes.MandatedRebuilding)
                Me.m_valWeights.SocialWeight = coreData.ValWeight(eValueWeightTypes.SocialValue)

                Me.m_valWeights.BiomassDiversityWeight = coreData.ValWeight(eValueWeightTypes.BiomassDiversity)

                Me.m_valWeights.PredictionVariance = coreData.ValWeight(eValueWeightTypes.PredictionVariance)
                Me.m_valWeights.ExistenceValue = coreData.ValWeight(eValueWeightTypes.ExistenceValue)

                Me.m_valWeights.AllowValidation = True

                Me.m_valWeights.ResetStatusFlags()

                Me.m_parameters.AllowValidation = False
                Me.m_parameters.BaseYear = coreData.BaseYear
                'Me.m_parameters.GenDiscRate = coreData.GenDiscountFactor
                Me.m_parameters.DiscountRate = coreData.DiscountFactor
                Me.m_parameters.FishingMortalityPenalty = coreData.UseFishingMortalityPenalty

                Me.m_parameters.PrevCostEarning = coreData.UseCostPenalty

                Me.m_parameters.ResetStatusFlags()

                Me.m_parameters.AllowValidation = True

                For Each grp As cSearchObjectiveGroupInput In Me.m_lstGroups
                    grp.AllowValidation = False
                    igrp = Array.IndexOf(Me.m_core.m_EcopathData.GroupDBID, grp.DBID)
                    grp.Index = igrp
                    grp.Name = Me.m_core.m_EcopathData.GroupName(igrp)

                    grp.MandRelBiom = coreData.MGoalValue(grp.Index)
                    grp.StrucRelWeight = coreData.BGoalValue(grp.Index)
                    grp.FishingLimit = coreData.FLimit(grp.Index)

                    grp.AllowValidation = True

                Next

                For Each flt As cSearchObjectiveFleetInput In Me.m_lstFleets
                    flt.AllowValidation = False

                    iflt = Array.IndexOf(Me.m_core.m_EcopathData.FleetDBID, flt.DBID)
                    flt.Index = iflt

                    flt.Resize()
                    flt.Name = Me.m_core.m_EcopathData.FleetName(iflt)
                    'pop variables.....

                    flt.JobCatchValue = coreData.Jobs(flt.Index)
                    flt.TargetProfitability = coreData.TargetProfitability(flt.Index)

                    'For it As Integer = 1 To m_core.nEcosimYears
                    '    flt.SearchBlocks(it) = coreData.FblockCode(iflt, it)
                    'Next it

                    flt.AllowValidation = True

                Next

                Return True

            Catch ex As Exception
                Return False
            End Try

        End Function

        Public Sub Clear() Implements ISearchObjective.Clear
            Me.m_lstFleets.Clear()
            Me.m_lstGroups.Clear()
        End Sub

        Public Overridable Function Update(DataType As eDataTypes) As Boolean Implements ISearchObjective.Update
            Dim coreData As cSearchDatastructures = Me.m_core.m_SearchData

            Select Case DataType

                Case eDataTypes.SearchObjectiveParameters

                    coreData.UseCostPenalty = Me.m_parameters.PrevCostEarning
                    coreData.BaseYear = Me.m_parameters.BaseYear
                    'coreData.GenDiscountFactor = Me.m_parameters.GenDiscRate
                    coreData.DiscountFactor = Me.m_parameters.DiscountRate
                    coreData.UseFishingMortalityPenalty = Me.m_parameters.FishingMortalityPenalty

                Case eDataTypes.SearchObjectiveFleetInput

                    'load the code blocks
                    For Each flt As cSearchObjectiveFleetInput In Me.m_lstFleets

                        coreData.Jobs(flt.Index) = flt.JobCatchValue
                        coreData.TargetProfitability(flt.Index) = flt.TargetProfitability

                        'For it As Integer = 1 To m_core.nEcosimYears
                        '    coreData.FblockCode(flt.Index, it) = flt.SearchBlocks(it)
                        'Next it
                    Next

                Case eDataTypes.SearchObjectiveGroupInput

                    For Each grp As cSearchObjectiveGroupInput In Me.m_lstGroups

                        coreData.MGoalValue(grp.Index) = grp.MandRelBiom
                        coreData.BGoalValue(grp.Index) = grp.StrucRelWeight
                        coreData.FLimit(grp.Index) = grp.FishingLimit

                    Next grp

                Case eDataTypes.SearchObjectiveWeights
                    'Value Weights
                    coreData.ValWeight(eValueWeightTypes.NetEcomValue) = Me.m_valWeights.EconomicWeight
                    coreData.ValWeight(eValueWeightTypes.EcoStructure) = Me.m_valWeights.EcoSystemWeight

                    coreData.ValWeight(eValueWeightTypes.BiomassDiversity) = Me.m_valWeights.BiomassDiversityWeight

                    'ValWeight() shares indexes for different values based on the search.PortFolio flag
                    'SocialValue = 2
                    'PredictionVariance = 2
                    'MandatedRebuilding = 3
                    'ExistenceValue = 3
                    If coreData.PortFolio Then
                        coreData.ValWeight(eValueWeightTypes.PredictionVariance) = Me.m_valWeights.PredictionVariance
                        coreData.ValWeight(eValueWeightTypes.ExistenceValue) = Me.m_valWeights.ExistenceValue
                    Else
                        coreData.ValWeight(eValueWeightTypes.MandatedRebuilding) = Me.m_valWeights.MandatedRebuildingWeight
                        coreData.ValWeight(eValueWeightTypes.SocialValue) = Me.m_valWeights.SocialWeight
                    End If

            End Select

            Return True

        End Function


        Public ReadOnly Property ValueWeights() As cSearchObjectiveWeights Implements ISearchObjective.ValueWeights
            Get
                Return Me.m_valWeights
            End Get
        End Property

        Public ReadOnly Property GroupObjectives(iGroup As Integer) As cSearchObjectiveGroupInput Implements ISearchObjective.GroupObjectives
            Get
                Return Me.m_lstGroups(iGroup)
            End Get
        End Property

        Public ReadOnly Property FleetObjectives(iGroup As Integer) As cSearchObjectiveFleetInput Implements ISearchObjective.FleetObjectives
            Get
                Return Me.m_lstFleets(iGroup)
            End Get
        End Property

        Public ReadOnly Property ObjectiveParameters() As cSearchObjectiveParameters Implements ISearchObjective.ObjectiveParameters
            Get
                Return Me.m_parameters
            End Get
        End Property


    End Class

#End Region

End Namespace





