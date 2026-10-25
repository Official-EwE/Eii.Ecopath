' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports ScientificInterfaceShared.Controls
Imports ValueChain
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface between Ecopath and the Ecost flow.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEwEValueChainModel

    ''' <summary>
    ''' Enumerated type used to describe for what model Value Chain produced results.
    ''' </summary>
    Public Enum eRunTypes As Integer
        ''' <summary>Value chain results computed for Ecopath.</summary>
        Ecopath
        ''' <summary>Value chain results computed for Ecosim.</summary>
        Ecosim
        ''' <summary>Value chain results computed for the Equilibrium search.</summary>
        Equilibrium
    End Enum

#Region " Private vars "

    ''' <summary>Preserved effort during equilibrium search.</summary>
    Private m_lPreservedEffort As New Dictionary(Of Integer, Single())
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEValueChainModel)()

    Private ReadOnly Property Core As cCore

#End Region ' Private vars

    Public Sub New(core As cCore)
        Me.Core = core
    End Sub

#Region " Running "

    Public Property IsManualRunMode() As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' VC 090910: beginning of equilibrium analysis in value chain
    ''' </summary>
    ''' <param name="data">Data to operate on.</param>
    ''' <param name="results">Results to plunder.</param>
    ''' <returns>True if successful</returns>
    ''' -----------------------------------------------------------------------
    Public Function RunEquilibrium(data As cValueChainData, results As cEwEValueChainResults) As Boolean

        Dim sMin As Single = Math.Min(data.Parameters.EquilibriumEffortMin, data.Parameters.EquilibriumEffortMax)
        Dim sMax As Single = Math.Max(data.Parameters.EquilibriumEffortMin, data.Parameters.EquilibriumEffortMax)
        Dim sStep As Single = Math.Max(0.01!, data.Parameters.EquilibriumEffortIncrement)
        Dim iNumSteps As Integer = CInt(((sMax - sMin) / sStep) * data.Parameters.EquilibriumFleetsToVary.Count)
        Dim iStep As Integer = 0
        Dim fleet As cEcopathFleetInput = Nothing

        Me.PreserveFishingEffort(data)

        Try

            'First reset all fishing effort
            Me.SetFishingEffort(data, 0, 1)

            For Each iFleet As Integer In data.Parameters.EquilibriumFleetsToVary

                fleet = Me.Core.EcopathFleetInputs(iFleet)

                For sEffort As Single = sMin To sMax Step data.Parameters.EquilibriumEffortIncrement

                    ' Update status text
                    cApplicationStatusNotifier.UpdateProgress(Me.Core,
                                                              String.Format(My.Resources.STATUS_PROGRESS_EQUILIBIRUM, fleet.Name, Math.Round(sEffort, 2)),
                                                              CSng(iStep / iNumSteps))
                    ' Set effort
                    Me.SetFishingEffort(data, iFleet, sEffort)
                    ' Run Ecosim for X years
                    Me.Core.RunEcosim()
                    ' Store values for the last time step
                    results.StoreSnapshot(sEffort, Me.Core.nEcosimTimeSteps)

                    ' Next
                    iStep += 1

                Next sEffort

                ' Reset effort to 1 before proceeding to next fleet
                Me.SetFishingEffort(data, iFleet, 1.0!)

            Next iFleet

        Catch ex As Exception
            Return False
        End Try

        Me.RestoreFishingEffort(data)

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Preserve the fishing effort shapes prior to running an Equilibrium search.
    ''' </summary>
    ''' <param name="data">The data to preserve effort shapes from.</param>
    ''' -----------------------------------------------------------------------
    Private Sub PreserveFishingEffort(data As cValueChainData)

        Dim Manager As cFishingEffortShapeManger = Me.Core.FishingEffortShapeManager
        Dim Shape As cShapeData = Nothing

        ' Clear cache of previously preserved effort shapes.
        Me.m_lPreservedEffort.Clear()
        ' For every fleet (including 'all' fleet at index 0)
        For iFleet As Integer = 0 To Me.Core.nFleets
            ' Get effort shape
            Shape = Manager.Item(iFleet)
            ' Store COPY of shape values
            Me.m_lPreservedEffort(iFleet) = DirectCast(Shape.ShapeData, Single())
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Restore previously preserved fishing effort shapes in the EwE core.
    ''' </summary>
    ''' <param name="data">The data to restore fishing effort to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RestoreFishingEffort(data As cValueChainData)

        Dim Manager As cFishingEffortShapeManger = Me.Core.FishingEffortShapeManager
        Dim Shape As cShapeData = Nothing

        Try
            ' For all fleets (yes, including 'all' fleet at index 0)
            For iFleet As Integer = 0 To Me.Core.nFleets
                ' Get effort shape
                Shape = Manager.Item(iFleet)
                ' Restore shape values
                Shape.ShapeData = Me.m_lPreservedEffort(iFleet)
            Next
            ' Update manager when all went well
            Manager.Update()

        Catch ex As Exception
            ' Ouch
        End Try

        ' Clear effort preservation cache
        Me.m_lPreservedEffort.Clear()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the fishing effort for a given fleet to a given value.
    ''' </summary>
    ''' <param name="data">The data to set effort into.</param>
    ''' <param name="Fleet">The fleet index to set effort for.</param>
    ''' <param name="Val">The value to set effort to.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SetFishingEffort(data As cValueChainData, Fleet As Integer, Val As Single) As Boolean

        Try
            Dim Manager As cFishingEffortShapeManger = Me.Core.FishingEffortShapeManager
            Dim Shape As cShapeData = Nothing

            Dim StartStep As Integer
            Dim EndStep As Integer
            If Fleet = 0 Then
                StartStep = 0
                EndStep = Me.Core.nFleets - 1
            Else
                StartStep = Fleet - 1
                EndStep = Fleet - 1
            End If

            For iFl As Integer = StartStep To EndStep
                Shape = Manager.Item(iFl)
                Shape.LockUpdates()
                Shape.ShapeData(1) = 1
                For iTimeStep As Integer = 2 To Me.Core.nEcosimTimeSteps 'Step cCore.N_MONTHS
                    Shape.ShapeData(iTimeStep) = Val
                    'set effort to unity 
                Next
                Shape.UnlockUpdates()
            Next
            Manager.Update()
        Catch ex As Exception
            Return False
        End Try
        Return True

    End Function

    ''' <summary>
    ''' Run a time step.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="result"></param>
    ''' <param name="iTimeStep">1 when running Ecopath.</param>
    ''' <param name="ecosimResults"></param>
    ''' <param name="ecosimDS"></param>
    Public Function RunTimeStep(data As cEwEValueChainData, result As cEwEValueChainResults, iTimeStep As Integer, Optional ecosimResults As cEcoSimResults = Nothing, Optional ecosimDS As cEcosimDatastructures = Nothing) As Boolean

        Dim bAllowedToRun As Boolean = False
        Dim iBaseYear As Integer = 0
        Dim writer As New cResultWriter(data, result)

        ' Sanity check
        Select Case result.RunType

            Case eRunTypes.Ecopath
                ' Sanity check
                Debug.Assert(ecosimResults Is Nothing)
                ' Always run
                bAllowedToRun = True

            Case eRunTypes.Ecosim, eRunTypes.Equilibrium

                ' Sanity check
                Debug.Assert(ecosimResults IsNot Nothing)
                ' Grab base year
                iBaseYear = Me.Core.SearchObjective.ObjectiveParameters.BaseYear
                ' Run when time step falls in the given base year
                bAllowedToRun = (iTimeStep >= CInt((iBaseYear - 1) * cCore.N_MONTHS))

        End Select

        If bAllowedToRun Then

            Try

                Select Case data.Parameters.AggregationMode

                    Case cParameters.eAggregationModeType.FullModel
                        Me.RunFullModel(data, result, iTimeStep, ecosimResults, ecosimDS)

                    Case cParameters.eAggregationModeType.ByFleet
                        Me.RunTimeStepByFleet(data, result, iTimeStep, ecosimResults, ecosimDS)

                    Case cParameters.eAggregationModeType.ByGroup
                        Me.RunTimeStepByLanding(data, result, iTimeStep, ecosimResults, ecosimDS)

                End Select

            Catch ex As Exception
                ' Aargh
                Debug.Assert(False, ex.Message)
                m_logger.LogError(ex, "VC::cModel.RunTimeStep(" & iTimeStep & ")")
            End Try

        End If

        ' Finish results
        result.CalculateDerivedValues(iTimeStep)

        Return True

    End Function

    ''' <summary>
    ''' Run a time step for the entire chain, unfiltered.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="result"></param>
    ''' <param name="iTimeStep">1 when running Ecopath.</param>
    ''' <param name="ecosimResults"></param>
    ''' <param name="ecosimDS"></param>
    Private Function RunFullModel(data As cEwEValueChainData, result As cEwEValueChainResults, iTimeStep As Integer, ecosimResults As cEcoSimResults, ecosimDS As cEcosimDatastructures) As Boolean

        Dim prodUnit As cProducerUnit = Nothing
        Dim iFleet As Integer = 0

        ' Prepare data for a time step
        data.InitTimeStep()

        ' For each producer
        For Each unit As cUnit In data.GetUnits(cUnitFactory.eUnitType.Producer)

            ' Get actual producer
            prodUnit = DirectCast(unit, cProducerUnit)

            iFleet = data.IDtoEwEiFleet(prodUnit.GearCode)
            If (iFleet > 0) Then
                For iGroupSrc As Integer = 1 To Me.Core.nGroups
                    prodUnit.AddLandings(data.EwEiGroupToID(iGroupSrc),
                                         Me.GetLandings(Me.Core, iFleet, iGroupSrc, iTimeStep, ecosimResults, ecosimDS),
                                         Me.GetLandingValue(Me.Core, iFleet, iGroupSrc, iTimeStep, ecosimResults, ecosimDS),
                                         Nothing)
                Next iGroupSrc
            End If

            Try
                prodUnit.Process(result, iTimeStep, 0)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                m_logger.LogError(ex, "VC::cModel.RunFullModel(" & prodUnit.Name & ")")
            End Try
        Next unit

        Return True

    End Function

    ''' <summary>
    ''' Run a time step, aggregated values by fleet.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="result"></param>
    ''' <param name="iTimeStep">1 when running Ecopath.</param>
    ''' <param name="ecosimResults"></param>
    ''' <param name="ecosimDS"></param>
    ''' <returns></returns>
    Public Function RunTimeStepByFleet(data As cEwEValueChainData, result As cEwEValueChainResults, iTimeStep As Integer, ecosimResults As cEcoSimResults, ecosimDS As cEcosimDatastructures) As Boolean

        Dim prodUnit As cProducerUnit = Nothing
        Dim iFleetSrc As Integer = 0 ' Fleet that is the landings source for a given producer unit

        ' First run chain for full model
        Me.RunFullModel(data, result, iTimeStep, ecosimResults, ecosimDS)

        ' Next run chain for each fleet
        For iFleet As Integer = 1 To Me.Core.nFleets

            ' Prepare data for a time step
            data.InitTimeStep()

            ' For each producer
            For Each unit As cUnit In data.GetUnits(cUnitFactory.eUnitType.Producer)

                ' Get actual producer
                prodUnit = DirectCast(unit, cProducerUnit)
                ' Get index
                iFleetSrc = data.IDtoEwEiFleet(prodUnit.GearCode)

                If (iFleetSrc > 0) Then
                    For iGroupSrc = 1 To Me.Core.nGroups
                        ' Gathering results for a fleet that does not serve the current producer?
                        If (iFleet <> iFleetSrc) Then
                            ' #Yes: run this fleet without landings and value
                            ' --- prodUnit.AddLandings(Me.EwEiGroupToID(iGroupSrc), 0, 0, Nothing)
                        Else
                            ' #No: Run this fleet using standard landings and value
                            prodUnit.AddLandings(data.EwEiGroupToID(iGroupSrc),
                                                 Me.GetLandings(Me.Core, iFleetSrc, iGroupSrc, iTimeStep, ecosimResults, ecosimDS),
                                                 Me.GetLandingValue(Me.Core, iFleetSrc, iGroupSrc, iTimeStep, ecosimResults, ecosimDS),
                                                 Nothing)
                        End If

                    Next iGroupSrc
                End If

                ' Start calculating!
                prodUnit.Process(result, iTimeStep, iFleet)

            Next unit

        Next iFleet

        Return True

    End Function

    ''' <summary>
    ''' Run a time step, aggregated values by fleet.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="result"></param>
    ''' <param name="iTimeStep">1 when running Ecopath.</param>
    ''' <param name="ecosimResults"></param>
    ''' <param name="ecosimDS"></param>
    ''' <returns></returns>
    Public Function RunTimeStepByLanding(data As cEwEValueChainData, result As cEwEValueChainResults, iTimeStep As Integer, ecosimResults As cEcoSimResults, ecosimDS As cEcosimDatastructures) As Boolean

        Dim grpRun As cEcoPathGroupInput = Nothing
        Dim flt As cEcopathFleetInput = Nothing
        Dim prodUnit As cProducerUnit = Nothing

        ' First run chain for full model
        Me.RunFullModel(data, result, iTimeStep, ecosimResults, ecosimDS)

        ' Next run chain for each group
        For iGroup As Integer = 1 To Me.Core.nGroups

            ' Get group
            grpRun = Me.Core.EcopathGroupInputs(iGroup)

            ' Prepare data for a time step
            data.InitTimeStep()

            ' Set this groups' landing for each producer
            For Each unit As cUnit In data.GetUnits(cUnitFactory.eUnitType.Producer)

                ' Get actual producer and its connected fleet
                prodUnit = DirectCast(unit, cProducerUnit)
                Dim iFleet As Integer = data.IDtoEwEiFleet(prodUnit.GearCode)

                ' Has a fleet?
                If (iFleet > 0) Then

                    Dim sCatch As Single = flt.Landings(iGroup) + flt.Discards(iGroup)

                    ' Is group being caught?
                    If (sCatch = 0) Then
                        ' #No: Assign no landings and value
                        ' --- prodUnit.AddLandings(Me.EwEiGroupToID(iGroup), 0, 0, Nothing)
                    Else
                        ' #Yes: Assign standard landings and value
                        Dim sB As Single = Me.GetLandings(Me.Core, iFleet, iGroup, iTimeStep, ecosimResults, ecosimDS)
                        Dim sV As Single = Me.GetLandingValue(Me.Core, iFleet, iGroup, iTimeStep, ecosimResults, ecosimDS)
                        prodUnit.AddLandings(data.EwEiGroupToID(iGroup), sB, sV, Nothing)
                    End If
                End If

                ' Start calculating for this group only
                prodUnit.Process(result, iTimeStep, iGroup)

            Next unit

        Next iGroup

        Return True

    End Function

#Region " Helpers "

    Private Function GetLandings(core As cCore,
                                 iFleet As Integer, iGroup As Integer, iTimeStep As Integer,
                                 ecosimresults As cEcoSimResults,
                                 ecosimDS As cEcosimDatastructures) As Single

        Dim model As cEwEModel = core.EwEModel
        Dim sArea As Single = model.Area
        Dim sLandings As Single = 0.0

        ' Has Ecosim results?
        If (ecosimresults Is Nothing) Then
            ' #No: run for Ecopath
            Dim fleet As cEcopathFleetInput = core.EcopathFleetInputs(iFleet)
            sLandings = fleet.Landings(iGroup) * sArea
        Else
            ' Yes: run for Ecosim
            Debug.Assert(iTimeStep = ecosimresults.CurrentT)
            ' JS 07Nov12: Ecosim produces 'Ecopath values': values across a year
            sLandings = ecosimresults.BCatch(iGroup, iFleet) * sArea / cCore.N_MONTHS
        End If

        Return sLandings

    End Function

    Private Function GetLandingValue(core As cCore,
                                     iFleet As Integer, iGroup As Integer, iTimeStep As Integer,
                                     ecosimresults As cEcoSimResults,
                                     ecosimDS As cEcosimDatastructures) As Single

        Dim model As cEwEModel = core.EwEModel
        Dim sArea As Single = model.Area

        ' Has Ecosim results?
        If (ecosimresults Is Nothing) Then
            ' #No: run for Ecopath

            'VC changed the calc below, it just summed up the marketprices, but it should sum
            'up the values and later divide by landings to get average price
            Dim fleet As cEcopathFleetInput = core.EcopathFleetInputs(iFleet)
            Return fleet.OffVesselValue(iGroup) * fleet.Landings(iGroup) * sArea
        Else
            ' #Yes: run for Ecosim
            ' JS 19Nov11: use Ecosim value for time step
            ' JS 07Nov12: Ecosim produces 'Ecopath values': values across a year
            Debug.Assert(iTimeStep = ecosimresults.CurrentT)
            Return ecosimDS.ResultsSumValueByGroupGear(iGroup, iFleet, iTimeStep) * sArea / cCore.N_MONTHS
        End If

    End Function

    Friend Sub SaveResults(data As cEwEValueChainData, result As cEwEValueChainResults, Optional iYear As Integer = 0)

        Try

            Dim w As New cResultWriter(data, result)
            Dim agg As cParameters.eAggregationModeType = data.Parameters.AggregationMode

            Select Case agg

                Case cParameters.eAggregationModeType.FullModel
                    w.WriteResults(agg)

                Case cParameters.eAggregationModeType.ByFleet
                    For iFleet As Integer = 1 To Me.Core.nFleets
                        Dim flt As cEcopathFleetInput = Me.Core.EcopathFleetInputs(iFleet)
                        w.WriteResults(agg, iFleet, flt.Name)
                    Next

                Case cParameters.eAggregationModeType.ByGroup
                    For iGroup As Integer = 1 To Me.Core.nGroups
                        Dim grp As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(iGroup)
                        If grp.IsFished Then
                            w.WriteResults(agg, iGroup, grp.Name)
                        End If
                    Next

            End Select

            If (w.Message IsNot Nothing) Then
                Me.Core.Messages.SendMessage(w.Message)
            End If

        Catch ex As Exception

        End Try

    End Sub

#End Region

#End Region ' Running

End Class
