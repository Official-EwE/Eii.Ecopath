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

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ValueChain
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug


#End Region ' Imports
''' ---------------------------------------------------------------------------
''' <summary>
''' Interface between Ecopath and the Ecost flow.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cValueChainController

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

    Private m_data As cValueChainData = Nothing
    Private m_chain As cValueChain = Nothing
    Private m_results As cValueChainResults = Nothing
    Private m_core As cCore = Nothing
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cValueChainController)()

#End Region ' Private vars

    Public Sub New(core As cCore, data As cValueChainData, results As cValueChainResults)
        m_data = data
        m_chain = New cValueChain(m_data)
        m_core = core
    End Sub

#Region " Running "

    ''' <summary>States whether running manual (from Value Chain UI) or automatically with core.</summary>
    Public Property IsManualRunMode() As Boolean

    Public Property RunType As eRunTypes = eRunTypes.Ecopath

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' VC 090910: beginning of equilibrium analysis in value chain
    ''' </summary>
    ''' <param name="data">Data to operate on.</param>
    ''' <param name="results">Results to plunder.</param>
    ''' <returns>True if successful</returns>
    ''' -----------------------------------------------------------------------
    Public Function RunEquilibrium(data As cValueChainData, results As cValueChainResults) As Boolean

        Dim sMin As Single = Math.Min(data.Parameters.EquilibriumEffortMin, data.Parameters.EquilibriumEffortMax)
        Dim sMax As Single = Math.Max(data.Parameters.EquilibriumEffortMin, data.Parameters.EquilibriumEffortMax)
        Dim sStep As Single = Math.Max(0.01!, data.Parameters.EquilibriumEffortIncrement)
        Dim iNumSteps As Integer = CInt(((sMax - sMin) / sStep) * data.Parameters.EquilibriumFleetsToVary.Count)
        Dim iStep As Integer = 0
        Dim fleet As cEcopathFleetInput = Nothing

        Me.RunType = eRunTypes.Equilibrium
        Me.PreserveFishingEffort(data)

        Try

            'First reset all fishing effort
            Me.SetFishingEffort(data, 0, 1)

            For Each iFleet As Integer In data.Parameters.EquilibriumFleetsToVary

                fleet = m_core.EcopathFleetInputs(iFleet)

                For sEffort As Single = sMin To sMax Step data.Parameters.EquilibriumEffortIncrement

                    ' Update status text
                    cApplicationStatusNotifier.UpdateProgress(m_core,
                                                              String.Format(My.Resources.STATUS_PROGRESS_EQUILIBIRUM, fleet.Name, Math.Round(sEffort, 2)),
                                                              CSng(iStep / iNumSteps))
                    ' Set effort
                    Me.SetFishingEffort(data, iFleet, sEffort)
                    ' Run Ecosim for X years
                    m_core.RunEcosim()
                    ' Store values for the last time step
                    results.StoreSnapshot(sEffort, m_core.nEcosimTimeSteps)

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

        Dim Manager As cFishingEffortShapeManger = m_core.FishingEffortShapeManager
        Dim Shape As cShapeData = Nothing

        ' Clear cache of previously preserved effort shapes.
        Me.m_lPreservedEffort.Clear()
        ' For every fleet (including 'all' fleet at index 0)
        For iFleet As Integer = 0 To m_core.nFleets
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

        Dim Manager As cFishingEffortShapeManger = m_core.FishingEffortShapeManager
        Dim Shape As cShapeData = Nothing

        Try
            ' For all fleets (yes, including 'all' fleet at index 0)
            For iFleet As Integer = 0 To m_core.nFleets
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
            Dim Manager As cFishingEffortShapeManger = m_core.FishingEffortShapeManager
            Dim Shape As cShapeData = Nothing

            Dim StartStep As Integer
            Dim EndStep As Integer
            If Fleet = 0 Then
                StartStep = 0
                EndStep = m_core.nFleets - 1
            Else
                StartStep = Fleet - 1
                EndStep = Fleet - 1
            End If

            For iFl As Integer = StartStep To EndStep
                Shape = Manager.Item(iFl)
                Shape.LockUpdates()
                Shape.ShapeData(1) = 1
                For iTimeStep As Integer = 2 To m_core.nEcosimTimeSteps 'Step cCore.N_MONTHS
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
    Public Function RunTimeStep(data As cValueChainData, result As cValueChainResults, iTimeStep As Integer, Optional ecosimResults As cEcoSimResults = Nothing, Optional ecosimDS As cEcosimDatastructures = Nothing) As Boolean

        Dim bAllowedToRun As Boolean = True
        Dim iBaseYear As Integer = 0
        Dim writer As New cValueChainResultsWriter(data, result)

        ' Sanity check
        Select Case Me.RunType

            Case eRunTypes.Ecopath
                ' sanity check
                Debug.Assert(ecosimResults Is Nothing)
                ' always run
                bAllowedToRun = True

            Case eRunTypes.Ecosim, eRunTypes.Equilibrium

                ' sanity check
                Debug.Assert(ecosimResults IsNot Nothing)
                ' grab base year
                iBaseYear = m_core.SearchObjective.ObjectiveParameters.BaseYear
                ' run when time step falls in the given base year
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
    Private Function RunFullModel(data As cValueChainData, result As cValueChainResults, iTimeStep As Integer, ecosimResults As cEcoSimResults, ecosimDS As cEcosimDatastructures) As Boolean

        Dim prodUnit As cProducerUnit = Nothing
        Dim iFleet As Integer = 0

        ' Prepare data for a time step
        data.InitTimeStep()

        ' For each producer
        For Each unit As cUnit In data.GetUnits(cUnitFactory.eUnitType.Producer)

            ' Get actual producer
            prodUnit = DirectCast(unit, cProducerUnit)
            Dim flt As cEcopathFleetInput = Me.FindFleet(prodUnit.GearCode)
            If (flt IsNot Nothing) Then
                iFleet = flt.Index
                For iGroupSrc = 1 To m_core.nGroups
                    Dim strGroup As String = Me.m_core.EcopathGroupInputs(iGroupSrc).Name
                    prodUnit.SetLandings(strGroup,
                                         Me.GetLandings(m_core, iFleet, iGroupSrc, iTimeStep, ecosimResults, ecosimDS),
                                         Me.GetLandingValue(m_core, iFleet, iGroupSrc, iTimeStep, ecosimResults, ecosimDS))
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
    Public Function RunTimeStepByFleet(data As cValueChainData, result As cValueChainResults, iTimeStep As Integer, ecosimResults As cEcoSimResults, ecosimDS As cEcosimDatastructures) As Boolean

        Dim prodUnit As cProducerUnit = Nothing
        Dim fltSrc As cEcopathFleetInput = Nothing

        ' First run chain for full model
        Me.RunFullModel(data, result, iTimeStep, ecosimResults, ecosimDS)

        ' Next run chain for each fleet
        For iFleet As Integer = 1 To m_core.nFleets

            ' Prepare data for a time step
            data.InitTimeStep()

            ' For each producer
            For Each unit As cUnit In data.GetUnits(cUnitFactory.eUnitType.Producer)

                ' Get actual producer
                prodUnit = DirectCast(unit, cProducerUnit)
                fltSrc = FindFleet(prodUnit.GearCode)
                If (fltSrc IsNot Nothing) Then
                    ' Get fleet
                    Dim iFleetSrc As Integer = fltSrc.Index
                    For iGroupSrc = 1 To m_core.nGroups
                        ' Get name
                        Dim grpName As String = Me.m_core.EcopathGroupInputs(iGroupSrc).Name
                        ' Gathering results for a fleet that does not serve the current producer?
                        If (iFleet <> fltSrc.Index) Then
                            ' #Yes: run this fleet without landings and value
                            prodUnit.SetLandings(grpName, 0, 0)
                        Else
                            ' #No: Run this fleet using standard landings and value
                            prodUnit.SetLandings(grpName,
                                                 Me.GetLandings(m_core, iFleetSrc, iGroupSrc, iTimeStep, ecosimResults, ecosimDS),
                                                 Me.GetLandingValue(m_core, iFleetSrc, iGroupSrc, iTimeStep, ecosimResults, ecosimDS))
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
    Public Function RunTimeStepByLanding(data As cValueChainData, result As cValueChainResults, iTimeStep As Integer, ecosimResults As cEcoSimResults, ecosimDS As cEcosimDatastructures) As Boolean

        Dim grp As cEcoPathGroupInput = Nothing
        Dim flt As cEcopathFleetInput = Nothing
        Dim prodUnit As cProducerUnit = Nothing

        ' First run chain for full model
        Me.RunFullModel(data, result, iTimeStep, ecosimResults, ecosimDS)

        ' Next run chain for each group
        For iGroup As Integer = 1 To m_core.nGroups

            ' Get group
            grp = m_core.EcopathGroupInputs(iGroup)
            Dim grpName As String = Me.m_core.EcopathGroupInputs(iGroup).Name

            ' Prepare data for a time step
            data.InitTimeStep()

            ' Set this groups' landing for each producer
            For Each unit As cUnit In data.GetUnits(cUnitFactory.eUnitType.Producer)

                ' Get actual producer and its connected fleet
                prodUnit = DirectCast(unit, cProducerUnit)
                flt = FindFleet(prodUnit.GearCode)

                ' Has a fleet?
                If (flt IsNot Nothing) Then

                    Dim sCatch As Single = flt.Landings(iGroup) + flt.Discards(iGroup)
                    Dim iFleet As Integer = flt.Index

                    ' Is group being caught?
                    If (sCatch = 0) Then
                        ' #No: Assign no landings and value
                        prodUnit.SetLandings(grpName, 0, 0)
                    Else
                        ' #Yes: Assign standard landings and value
                        Dim sB As Single = Me.GetLandings(m_core, iFleet, iGroup, iTimeStep, ecosimResults, ecosimDS)
                        Dim sV As Single = Me.GetLandingValue(m_core, iFleet, iGroup, iTimeStep, ecosimResults, ecosimDS)
                        prodUnit.SetLandings(grpName, sB, sV)
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

    Public Shared Function ToSafeName(strName As String) As String
        Return strName.Trim().ToLowerInvariant()

    End Function


    Public Function FindFleet(strName As String) As cEcopathFleetInput
        If (Not String.IsNullOrWhiteSpace(strName)) Then
            strName = strName.Trim().ToLowerInvariant()
            For iFleet As Integer = 1 To Me.m_core.nFleets
                Dim flt As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(iFleet)
                If String.CompareOrdinal(strName, flt.Name) = 0 Then
                    Return flt
                End If
            Next
        End If
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="agg"></param>
    ''' <param name="strItem"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetFileName(agg As cParameters.eAggregationModeType, Optional strItem As String = "", Optional iTimeStep As Integer = 0) As String

        Dim strFile As String = ""
        strFile = cStringUtils.Localize("valuechain_{0}.csv", agg.ToString())

        If Not String.IsNullOrWhiteSpace(strItem) Then
            strFile = cStringUtils.Localize("{0}_{1}", strFile, strItem)
        End If

        If (iTimeStep > 0) Then
            strFile = cStringUtils.Localize("{0}_{1:0000}", strFile, iTimeStep)
        End If

        Return cFileUtils.ToValidFileName(strFile, False) & ".csv"

    End Function

    Friend Sub SaveResults(data As cValueChainData, results As cValueChainResults)

        Try

            Dim agg As cParameters.eAggregationModeType = data.Parameters.AggregationMode

            Dim iTimeStart As Integer = If(Me.RunType = cValueChainController.eRunTypes.Ecopath, 0, 1)
            Dim iTimeEnd As Integer = If(Me.RunType = cValueChainController.eRunTypes.Ecopath, 0, results.NumTimeSteps)

            Dim pout As String = ""
            Select Case Me.RunType
                Case cValueChainController.eRunTypes.Ecopath
                    pout = Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecopath), "ValueChain")
                Case cValueChainController.eRunTypes.Ecosim
                    pout = Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecosim), "ValueChain")
                Case cValueChainController.eRunTypes.Equilibrium
                    Return
            End Select

            If Not cFileUtils.IsDirectoryAvailable(pout, True) Then Return

            Dim iItem As Integer = 0
            Dim strItem As String = ""
            Dim writer As New cValueChainResultsWriter(data, results)

            For iStep As Integer = iTimeStart To iTimeEnd

                Select Case agg

                    Case cParameters.eAggregationModeType.FullModel
                        Dim strFile As String = Me.GetFileName(agg, strItem, iStep)
                        writer.WriteResults(Path.Combine(pout, strFile), agg, iStep, iItem, strItem)

                    Case cParameters.eAggregationModeType.ByFleet
                        For iFleet As Integer = 1 To Me.m_core.nFleets
                            Dim flt As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(iFleet)
                            iItem = iFleet
                            strItem = flt.Name
                            Dim strFile As String = Me.GetFileName(agg, strItem, iStep)
                            writer.WriteResults(Path.Combine(pout, strFile), agg, iStep, iItem, strItem)
                        Next

                    Case cParameters.eAggregationModeType.ByGroup
                        For iGroup As Integer = 1 To Me.m_core.nGroups
                            Dim grp As cEcoPathGroupInput = Me.m_core.EcopathGroupInputs(iGroup)
                            If grp.IsFished Then
                                iItem = iGroup
                                strItem = grp.Name
                                Dim strFile As String = Me.GetFileName(agg, strItem, iStep)
                                writer.WriteResults(Path.Combine(pout, strFile), agg, iStep, iItem, strItem)
                            End If
                        Next

                End Select

            Next

        Catch ex As Exception

        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get default model details to report in output file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetModelDetails() As String

        Dim sb As New StringBuilder()
        Dim core As cCore = Me.m_core
        Dim dtFields As New Dictionary(Of String, String)
        dtFields("RunType") = (Me.RunType.ToString())

        ' Append header
        If (Me.RunType = cValueChainController.eRunTypes.Ecopath) Then
            sb.AppendLine(core.DefaultFileHeader(eAutosaveTypes.Ecopath, extraFields:=dtFields))
        Else
            sb.AppendLine(core.DefaultFileHeader(eAutosaveTypes.Ecosim, extraFields:=dtFields))
        End If

        Return sb.ToString()

    End Function

#End Region

#End Region ' Running

#Region " Equations "


    Public Sub Equations(TH As Integer)
        '*----------------------------------
        '*Equations
        '*----------------------------------
        'EtProfit(TH)
        'EtSales(TH)
        'EtCosts(TH)
        'EProfit(TH)
        'EProfitT(Me.m_data.Port, Me.m_data.Tech, Me.m_data.SCal, TH)
        'EProfitTD(Me.m_data.Port, Me.m_data.Tech, Me.m_data.SCal, TH)
        'ESales(TH)
        'ECosts(TH)
        'EOutput1(Me.m_data.Spec, TH)
        'EOutput2(Me.m_data.Port, Me.m_data.Tech, Me.m_data.SCal, TH)
        'EOutput3(Me.m_data.Spec, TH)
        'EExploit(Me.m_data.Spec, TH)
        'EMinStock(Me.m_data.Spec, TH)
        'EInput1(Me.m_data.Uses, TH)
        'EInput2(Me.m_data.Port, Me.m_data.Tech, Me.m_data.SCal, TH)
        'EInput3(Me.m_data.Uses, Me.m_data.Tech, Me.m_data.SCal, TH)
        'EStock(Me.m_data.Spec, TH)
        'EGrowth(Me.m_data.Spec, TH)
        'EMT(Me.m_data.Port, Me.m_data.Tech, Me.m_data.SCal, Me.m_data.Spec, TH)
        'EUT(Me.m_data.Uses, Me.m_data.Port, Me.m_data.Tech, Me.m_data.SCal, TH)

    End Sub

    Private Sub EtProfit(TH As Integer)
        'TPROFIT = E = TSALES - TCOSTS

    End Sub
    Private Sub EtSales(TH As Integer)
        '        TSALES =E= SUM((SPEC,STIME),PRICE1(SPEC,STIME)*OUTPUT1(SPEC,STIME))

    End Sub

    Private Sub EtCosts(TH As Integer)
        '        TCOSTS =E= SUM((USES,STIME),INPUT1(USES,STIME))

    End Sub

    Private Sub EProfit(TH As Integer)
        '        PROFIT(STIME) =E= SALES(STIME) - COSTS(STIME)

    End Sub

    Private Sub EProfitT(PORT As Integer, TECH As Integer, SCAL As Integer, TH As Integer)
        '        PROFITT(PORT,TECH,SCAL,STIME) =E= OUTPUT2(PORT,TECH,SCAL,STIME) - INPUT2(PORT,TECH,SCAL,STIME)

    End Sub

    Private Sub EProfitTD(PORT As Integer, TECH As Integer, SCAL As Integer, TH As Integer)
        '        PROFITTD(PORT,TECH,SCAL,STIME) =E= PROFITT(PORT,TECH,SCAL,STIME)/(EFFORT(PORT,TECH,SCAL,STIME)+1)

    End Sub

    Private Sub ESales(TH As Integer)
        '        SALES(STIME) =E= SUM(SPEC,PRICE1(SPEC,STIME)*OUTPUT1(SPEC,STIME))

    End Sub

    Private Sub ECosts(TH As Integer)
        'ECOSTS(STIME) ..
        '        COSTS(STIME) = E = SUM(Uses, INPUT1(Uses, STIME))

    End Sub

    Private Sub EOutput1(SPEC As Integer, TH As Integer)
        'EOUTPUT1(SPEC,STIME)$(ord(STIME) LE 15) .. 
        '        OUTPUT1(SPEC,STIME) =E= SUM((PORT,TECH,SCAL),MT(PORT,TECH,SCAL,SPEC,STIME)*EFFORT(PORT,TECH,SCAL,STIME))

    End Sub

    Private Sub EOutput2(PORT As Integer, TECH As Integer, SCAL As Integer, TH As Integer)
        'EOUTPUT2(PORT,TECH,SCAL,STIME)$(ord(STIME) LE 15) .. 
        '        OUTPUT2(PORT, TECH, SCAL, STIME) = E = SUM(Spec, PRICE1(Spec, STIME) * MT(PORT, TECH, SCAL, Spec, STIME) * EFFORT(PORT, TECH, SCAL, STIME))

    End Sub

    Private Sub EOutput3(SPEC As Integer, TH As Integer)
        'EOUTPUT3(SPEC,STIME)$(ord(STIME) LE 15) .. 
        '        OUTPUT3(SPEC,STIME) =E= SUM((PORT,TECH,SCAL),MT(PORT,TECH,SCAL,SPEC,STIME)*FFLEET(TECH,SCAL,STIME))

    End Sub

    Private Sub EExploit(SPEC As Integer, TH As Integer)
        'EEXPLOIT(SPEC,STIME)$(ord(STIME) LE 15) ..
        '*        SUM((PORT,TECH,SCAL),MT(PORT,TECH,SCAL,SPEC,STIME)*STOCK(SPEC,STIME)) =G= OUTPUT1(SPEC,STIME)
        '        0.5*STOCK(SPEC,STIME) =G= OUTPUT1(SPEC,STIME)+OUTPUT3(SPEC,STIME)

    End Sub

    Private Sub EMinStock(SPEC As Integer, TH As Integer)
        'EMINSTOCK(SPEC,STIME)$(ord(STIME) LE 15) ..
        '        STOCK(SPEC,STIME) =G= 0.05*BIOMASS(SPEC,"2000")

    End Sub

    Private Sub EInput1(USES As Integer, TH As Integer)
        'EINPUT1(USES,STIME)$(ord(STIME) LE 15) .. 
        '        INPUT1(USES,STIME) =E= SUM((PORT,TECH,SCAL),PRICE2(USES,TECH,SCAL,STIME)*UT(USES,PORT,TECH,SCAL,STIME)*EFFORT(PORT,TECH,SCAL,STIME))

    End Sub

    Private Sub EInput2(PORT As Integer, TECH As Integer, SCAL As Integer, TH As Integer)
        'EINPUT2(PORT,TECH,SCAL,STIME)$(ord(STIME) LE 15) .. 
        '        INPUT2(PORT, TECH, SCAL, STIME) = E = SUM(Uses, PRICE2(Uses, TECH, SCAL, STIME) * UT(Uses, PORT, TECH, SCAL, STIME) * EFFORT(PORT, TECH, SCAL, STIME))

    End Sub

    Private Sub EInput3(USES As Integer, TECH As Integer, SCAL As Integer, TH As Integer)
        'EINPUT3(USES,TECH,SCAL,STIME)$(ord(STIME) LE 15) .. 
        '        INPUT3(USES, TECH, SCAL, STIME) = E = SUM(Port, PRICE2(USES, TECH, SCAL, STIME) * UT(USES, Port, TECH, SCAL, STIME) * EFFORT(Port, TECH, SCAL, STIME))

    End Sub

    Private Sub EStock(SPEC As Integer, TH As Integer)
        'ESTOCK(SPEC,STIME)$(ord(STIME) LE 15) ..
        '        STOCK(SPEC,STIME) =E= (BIOMASS(SPEC,"2000")$(ord(STIME) EQ 1)
        '                              +GROWTH(SPEC,STIME)*(STOCK(SPEC,STIME-1)-OUTPUT1(SPEC,STIME-1)-OUTPUT3(SPEC,STIME-1))$(ord(STIME) GE 2)
        '                             )$(ord(SPEC) NE 2)
        '                           +STOCK("JTP","2001")$(ord(SPEC) EQ 2)   

    End Sub

    Private Sub EGrowth(SPEC As Integer, TH As Integer)
        'EGROWTH(SPEC,STIME)$(ord(STIME) LE 15) ..
        '       GROWTH(SPEC, STIME) = E = 1.25

    End Sub

    Private Sub EMT(PORT As Integer, TECH As Integer, SCAL As Integer, SPEC As Integer, TH As Integer)
        'EMT(PORT,TECH,SCAL,SPEC,STIME)$(ord(STIME) LE 15) .. 
        '        MT(PORT,TECH,SCAL,SPEC,STIME) =E= MT2000(PORT,TECH,SCAL,SPEC)$(ord(STIME) EQ 1)+(MT2000(PORT,TECH,SCAL,SPEC)*(STOCK(SPEC,STIME)/STOCK(SPEC,"2001")))$(ord(STIME) GE 2)
        '*        MT(PORT,TECH,SCAL,SPEC,STIME) =E= MT2000(PORT,TECH,SCAL,SPEC)$(ord(STIME) EQ 1)+(MT(PORT,TECH,SCAL,SPEC,STIME-1)*(STOCK(SPEC,STIME)/STOCK(SPEC,STIME-1)))$(ord(STIME) GE 2)
        '*        MT(PORT,TECH,SCAL,SPEC,STIME) =E= MT2000(PORT,TECH,SCAL,SPEC)

    End Sub

    Private Sub EUT(USES As Integer, PORT As Integer, TECH As Integer, SCAL As Integer, TH As Integer)
        'EUT(USES,PORT,TECH,SCAL,STIME)$(ord(STIME) LE 15) .. 
        '        UT(USES, PORT, TECH, SCAL, STIME) = E = UT2000(USES, PORT, TECH, SCAL)

    End Sub

    'not used:
    '    EPRICOST1(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        PRICOST1(METR1,STIME) =E= SUM(USES,PUSES1(USES,STIME)*UT(USES,METR1,STIME)*EFFORT1(METR1,STIME))
    ';
    'EPRICOST2(METR2,STIME)$(ord(STIME) LE 15) .. 
    '        PRICOST2(METR2,STIME) =E= SUM(USES,PUSES2(USES,STIME)*UT(USES,METR2,STIME)*EFFORT2(METR2,STIME))
    ';
    'EPRICOST3(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        PRICOST3(METR3,STIME) =E= SUM(USES,PUSES3(USES,STIME)*UT(USES,METR3,STIME)*EFFORT3(METR3,STIME))
    ';
    'EVOUTPUT1(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        VOUTPUT1(METR1,STIME) =E= SUM(SPEC,PRFISH(SPEC,STIME)*OUTPUT1(SPEC,METR1,STIME))
    ';
    'EVOUTPUT2(METR2,STIME)$(ord(STIME) LE 15) .. 
    '        VOUTPUT2(METR2,STIME) =E= SUM(SPEC,PPFISH(SPEC,STIME)*OUTPUT2(SPEC,METR2,STIME))
    ';
    'EVOUTPUT3(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        VOUTPUT3(METR3,STIME) =E= SUM(SPEC,PMFISH(SPEC,STIME)*OUTPUT3(SPEC,METR3,STIME))
    ';
    'EPRIBNFT1(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        PRIBNFT1(METR1,STIME) =E= VOUTPUT1(METR1,STIME)-PRICOST1(METR1,STIME)
    ';
    'EPRIBNFT2(METR2,STIME)$(ord(STIME) LE 15) .. 
    '        PRIBNFT2(METR2,STIME) =E= VOUTPUT2(METR2,STIME)-PRICOST2(METR2,STIME)
    ';
    'EPRIBNFT3(METR3,STIME)$(ord(STIME) LE 15) .. 
    '        PRIBNFT3(METR3,STIME) =E= VOUTPUT3(METR3,STIME)-PRICOST3(METR3,STIME)
    ';
    'EPUBCOST1(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        PUBCOST1(METR,STIME) =E=  ((PRDTAX(STIME)+EVMTAX(STIME))*PRICOST1(METR1,STIME)+MCOST1(METR1,STIME)/EFFORT1(METR1,STIME))
    ';
    'EPUBCOST2(METR2,STIME)$(ord(STIME) LE 15) .. 
    '        PUBCOST2(METR,STIME) =E=  ((PRDTAX(STIME)+EVMTAX(STIME))*PRICOST2(METR2,STIME)+MCOST2(METR2,STIME)/EFFORT2(METR2,STIME))
    ';
    'EPUBBNFT1(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        PUBBNFT1(METR1,STIME) =E=  SUBSIDY1(METR1,STIME)
    ';
    'EPUBBNFT2(METR2,STIME)$(ord(STIME) LE 15) .. 
    '        PUBBNFT2(METR2,STIME) =E=  SUBSIDY2(METR2,STIME)
    ';
    'EPUBBNFT3(METR3,STIME)$(ord(STIME) LE 15) .. 
    '        PUBBNFT3(METR3,STIME) =E=  SUBSIDY3(METR3,STIME)
    ';
    'ETOTCOST1(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        TOTCOST1(METR1,STIME) =E=  PRICOST1(METR1,STIME)+PUBCOST1(METR1,STIME)
    ';
    'ETOTCOST2(METR2,STIME)$(ord(STIME) LE 15) .. 
    '        TOTCOST2(METR2,STIME) =E=  PRICOST2(METR2,STIME)+PUBCOST2(METR2,STIME)
    ';
    'ETOTCOST3(METR3,STIME)$(ord(STIME) LE 15) .. 
    '        TOTCOST3(METR3,STIME) =E=  PRICOST3(METR3,STIME)+PUBCOST3(METR3,STIME)
    ';
    'ETOTCOST1(METR1,STIME)$(ord(STIME) LE 15) .. 
    '        TOTCOST1(METR1,STIME) =E=  PRICOST1(METR1,STIME)+PUBCOST1(METR1,STIME)
    ';
    'ETOTCOST2(METR2,STIME)$(ord(STIME) LE 15) .. 
    '        TOTCOST2(METR2,STIME) =E=  PRICOST2(METR2,STIME)+PUBCOST2(METR2,STIME)
    ';
    'ETOTCOST3(METR3,STIME)$(ord(STIME) LE 15) .. 
    '        TOTCOST3(METR3,STIME) =E=  PRICOST3(METR3,STIME)+PUBCOST3(METR3,STIME)
    ';

#End Region ' Equations

End Class
