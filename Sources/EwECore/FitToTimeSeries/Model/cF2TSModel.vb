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

Imports EwECore.Ecosim
Imports EwEUtils.Core
Imports EwEUtils.Utilities


Namespace FitToTimeSeries

    Public Enum eRunType As Integer
        Idle = 0
        SensitivitySS2VByPredPrey
        SensitivitySS2VByPredator
        Search
    End Enum

    ' Delegates that controlling processes must subscribe to for the model to run

    ''' <summary>
    ''' An iteration of the fitting search has been completed
    ''' </summary>
    ''' <remarks>RunState() will contain the run type. Results() will contain the results of this iteration.  </remarks>
    Public Delegate Sub RunStepDelegate()

    ''' <summary>
    ''' A call the Ecosim has been made
    ''' </summary>
    ''' <remarks>RunState() will contain the run type. Results() will contain the results of the last iteration.  </remarks>
    Public Delegate Sub RunModelDelegate(runType As eRunType, iCurrentIterationStep As Integer, nTotalInterationSteps As Integer)


    ''' <summary>
    ''' A search of sensitivity run has started
    ''' </summary>
    ''' <param name="runType">Type of run</param>
    ''' <param name="nSteps">Number of steps in this run if known at the start time otherwise zero</param>
    Public Delegate Sub RunStartedDelegate(runType As eRunType, nSteps As Integer)

    ''' <summary>
    ''' A search run has stopped
    ''' </summary>
    ''' <param name="runType">Type of run</param>
    Public Delegate Sub RunStoppedDelegate(runType As eRunType)

    ''' <summary>
    ''' A message being sent out by the search
    ''' </summary>
    ''' <param name="msg"></param>
    Public Delegate Sub RunMessageDelegate(msg As cMessage)

    Public Class cF2TSModel

#Region "Private data"

#Region "run data"

        'core data
        Private m_core As cCore = Nothing
        Private m_ecosim As cEcosimModel = Nothing
        Private m_epdata As cEcopathDataStructures
        Private m_esdata As cEcosimDatastructures
        Private m_tsData As cTimeSeriesDataStructures

        'run data
        Private m_results As cF2TSResults

        Const VUL_MULT As Single = 1.01

        Private Enum eSensType As Integer
            NotRun
            PredColumn
            PredPreyCell
        End Enum

        Private m_lastRunSens As eSensType

        Private m_lstSSResults As List(Of cSensitivityToVulResults)

#End Region

#Region "Modeling Varaibles"


        Dim rmax As Single, Jit As Integer, ic As Integer
        Dim SO As Single, dinc As Single, n As Integer, ip As Integer
        Dim i As Integer, Ipn() As Integer, Rbet As Single
        Dim Nobs As Integer, DF As Single, St() As Single
        Dim Rr2 As Single, Rmin As Single, Sbase As Single
        Dim Ss As Single, Ybase() As Single, j As Integer
        Dim Va As Single, Vmax As Single, var As Single, kkkk As Integer, Vc As Single
        Dim Vp As Single, Np As Single, Sp As Single, Dp As Single
        Dim Se(,) As Single
        Dim Sold() As Single, Xy() As Single, Su As Single, K As Integer
        Dim amat(,) As Single, Vi(,) As Single, Cl(,) As Single, Ct As Single, cy() As Single
        Dim Grad As Single, Rs As Single, Stry As Single, Rnew As Single, Rdel As Single, Snew As Single
        Dim Ss2 As Single, Den As Single, MaxObs As Integer
        Dim Penter() As Single, Po() As Single, pv() As Single, P() As Single, paramname() As String, StopIndex As Integer, MaxPars As Integer

        Public StopRun As Boolean
        Public PPyear1 As Integer, PPyear2 As Integer
        Public IsBlockEstimated() As Boolean
        Public VBlock() As Single, VblockCode() As Integer, CodeIsSet As Boolean
        Dim Xspline() As Single
        Public Numspline As Integer
        Public AnomalySearch As Boolean
        Public SearchMaxColors As Integer
        Public ForceNo As Integer = 0

        'Added by joe
        Public nBlockCodes As Integer

        Private TotalTime As Integer
        Private m_data As cF2TSDataStructures

        'count of DoEstimation interations
        Private m_estIter As Integer

        'parameter variance for vulnerability search
        'set in InitForRun()
        Private pvVul As Single

        'sensitivity for predators
        Dim PSen() As Single


#End Region

#End Region

#Region "Construction and Initialization"

        Friend Sub New(core As cCore, _
                            ByRef EcoSim As EwECore.Ecosim.cEcosimModel, _
                            ByRef EcoPathData As cEcopathDataStructures, EcosimData As cEcosimDatastructures)
            Me.m_core = core
            Me.m_ecosim = EcoSim
            Me.m_epdata = EcoPathData
            Me.m_esdata = EcosimData
            Me.m_lastRunSens = eSensType.NotRun
        End Sub

        ''' <summary>
        ''' Init model, optionally for multi-threading
        ''' </summary>
        ''' <param name="runstartedHandler"></param>
        ''' <param name="runstepHandler"></param>
        ''' <param name="runstoppedHandler"></param>
        Public Sub Init( _
                runstartedHandler As RunStartedDelegate, _
                runstepHandler As RunStepDelegate, _
                runstoppedHandler As RunStoppedDelegate, _
                AddMessageHandler As RunMessageDelegate, _
                RunModelHandler As RunModelDelegate, _
                SendMessageHandler As RunMessageDelegate)


            ' Safety check
            Debug.Assert(Me.RunState = eRunType.Idle)

            Me.m_runstartedHandler = runstartedHandler
            Me.m_runstepHandler = runstepHandler
            Me.m_runstoppedHandler = runstoppedHandler
            Me.m_AddMessageHandler = AddMessageHandler
            Me.m_SendMessageHandler = SendMessageHandler

            Me.m_runModelHandler = RunModelHandler
            Me.m_lastRunSens = eSensType.NotRun
            Me.m_lstSSResults = New List(Of cSensitivityToVulResults)

        End Sub

#End Region

#Region " Public bits "

        Public Property RunState() As eRunType = eRunType.Idle

        ''' <summary>
        ''' Results of the run or iteration depending on when it is accessed
        ''' </summary>
        Public ReadOnly Property Results() As cF2TSResults
            Get
                Return Me.m_results
            End Get
        End Property

        ''' <summary>
        ''' Get whether a sensitivity search has been ran.
        ''' </summary>
        Public ReadOnly Property HasRunSens As Boolean
            Get
                Return (Me.m_lastRunSens <> eSensType.NotRun)
            End Get
        End Property

        Public ReadOnly Property Data() As cF2TSDataStructures
            Get
                Return Me.m_data
            End Get
        End Property

#End Region ' Public bits

#Region " SensitivitySS2VByPredPrey "

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        '''pass in params that this method needs instead of obtaining them from the manager. This class may NOT KNOW ITS MANAGER!
        ''' </remarks>
        Public Sub RunSensitivitySS2VByPredPrey()

            Dim nSteps As Integer = 1 + 169

            Dim Vo As Single
            Dim Ssen() As Single
            'Dim tmpval, tempEmp, timeMan, tempEco As Double
            Dim Smax As Single, SSBase As Single, sss As Single

            Dim esData As cEcosimDatastructures = Me.m_core.m_EcoSimData
            Dim ecosim As cEcosimModel = Me.m_core.m_Ecosim

            ReDim Ssen(esData.inlinks)

            Me.m_lstSSResults.Clear()

            Try

                ' ToDo: add sanity checks; check if threading set up ok, not running, etc
                Me.RunState = eRunType.SensitivitySS2VByPredPrey

                Me.InitForRun(Me.RunState)
                Me.m_lastRunSens = eSensType.PredPreyCell
                Dim senResults As cSensitivityToVulResults = DirectCast(Me.m_results, cSensitivityToVulResults)

                Me.m_runstartedHandler(Me.RunState, esData.Narena)

                ecosim.RunModelValue(esData.NumYears, Nothing, 0)
                SSBase = esData.SS

                senResults.BaseSS = SSBase

                'logic from frmSearch.Command3_Click()
                For ii As Integer = 1 To esData.Narena

                    Me.i = esData.Iarena(ii) : Me.j = esData.Jarena(ii)
                    Vo = esData.VulMult(Me.i, Me.j)

                    esData.VulMult(Me.i, Me.j) = esData.VulMult(Me.i, Me.j) * VUL_MULT
                    ecosim.RunModelValue(esData.NumYears, Nothing, 0)

                    sss = Math.Abs(esData.SS - SSBase)
                    Ssen(ii) = sss

                    If sss > Smax Then Smax = sss

                    'set vulnerability back to its original value
                    esData.VulMult(Me.i, Me.j) = Vo

                    'set values for interface
                    senResults.iPred = Me.j
                    senResults.iPrey = Me.i
                    senResults.SSen = sss
                    senResults.SSMax = Smax

                    Me.m_lstSSResults.Add(New cSensitivityToVulResults(eRunType.SensitivitySS2VByPredPrey, Me.j, Me.i, sss, Smax))

                    Me.m_runstepHandler()

                    If Me.StopRun Then Exit For

                Next

            Catch ex As Threading.ThreadAbortException

                Me.AddMessage(New cMessage(My.Resources.CoreMessages.F2TS_ABORTED, _
                                        eMessageType.ErrorEncountered, _
                                        eCoreComponentType.EcosimFitToTimeSeries, _
                                        eMessageImportance.Critical))

            Catch ex As Exception

                Me.AddMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.F2TS_ERROR, ex.Message), _
                                        eMessageType.ErrorEncountered, _
                                        eCoreComponentType.EcosimFitToTimeSeries, _
                                        eMessageImportance.Critical))

            End Try

            ' Done searching
            Me.RunState = eRunType.Idle
            Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            Me.m_runstoppedHandler(eRunType.SensitivitySS2VByPredPrey)

        End Sub

#End Region ' SensitivitySS2VByPredPrey

#Region " SensitivitySS2VByPredator "

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' pass in params that this method needs instead of obtaining them from the manager. This class may NOT KNOW ITS MANAGER!
        ''' </remarks>
        Public Sub RunSensitivitySS2VByPredator()

            Dim results As cF2TSResults = Nothing
            Dim nSteps As Integer = 1 + 24

            Dim Smax As Single, SSBase As Single, sss As Single
            Dim ipred As Integer, iprey As Integer

            If Me.m_core.m_TSData.AppliedNdatType = 0 Then
                Exit Sub
            End If

            Dim epData As cEcopathDataStructures = Me.m_core.m_EcopathData
            Dim esData As cEcosimDatastructures = Me.m_core.m_EcoSimData
            Dim ecosim As cEcosimModel = Me.m_core.m_Ecosim

            Dim nGroups As Integer = Me.m_core.nGroups
            Dim nLiving As Integer = Me.m_core.nLivingGroups

            ReDim Me.PSen(nLiving)

            Me.m_lstSSResults.Clear()

            Try
                'init 
                Me.RunState = eRunType.SensitivitySS2VByPredator
                Me.InitForRun(Me.RunState)
                Me.m_lastRunSens = eSensType.PredColumn

                'cast the results into the correct type of object
                Dim senResults As cSensitivityToVulResults = DirectCast(Me.m_results, cSensitivityToVulResults)

                'tell the interface the run is starting
                Me.m_runstartedHandler(Me.RunState, nLiving)

                Me.initEcosimForSearchIteration()

                ecosim.RunModelValue(esData.NumYears, Nothing, 0)
                SSBase = esData.SS
                senResults.BaseSS = esData.SS
                senResults.iPrey = 0 'prey index not used it is all the prey for a given pred

                For ipred = 1 To nLiving 'predator
                    If epData.QB(ipred) > 0 Then

                        'Vary the vul for all the prey of this predator
                        For iprey = 1 To nGroups
                            If epData.DC(ipred, iprey) > 0 Then esData.VulMult(iprey, ipred) = esData.VulMult(iprey, ipred) * VUL_MULT
                        Next

                        ecosim.RunModelValue(esData.NumYears, Nothing, 0)
                        sss = Math.Abs(esData.SS - SSBase)
                        If sss > Smax Then Smax = sss 'the max sensitivity

                        For iprey = 1 To nGroups
                            If epData.DC(ipred, iprey) > 0 Then
                                esData.VulMult(iprey, ipred) = esData.VulMult(iprey, ipred) / VUL_MULT
                            End If
                        Next

                        'set values for interface
                        senResults.iPred = ipred

                        senResults.SSen = sss
                        senResults.SSMax = Smax
                        Me.PSen(ipred) = sss

                        Me.m_lstSSResults.Add(New cSensitivityToVulResults(eRunType.SensitivitySS2VByPredator, ipred, 0, sss, Smax))

                        Me.m_runstepHandler()

                        If Me.StopRun Then Exit For

                    End If 'If epData.QB(j) > 0 Then

                Next ipred


            Catch ex As Threading.ThreadAbortException
                ' Done
                'this should not happen under normal circumstances
                'm_runmessageHandler(New cMessage("Fit to Time Series aborted.", _
                '                    eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical))

            Catch ex As Exception
                ' Woops
                Me.AddMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.F2TS_ERROR, ex.Message), _
                                        eMessageType.ErrorEncountered, _
                                        eCoreComponentType.EcosimFitToTimeSeries, _
                                        eMessageImportance.Critical))

            End Try

            ' Done searching
            Me.RunState = eRunType.Idle
            Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            Me.m_runstoppedHandler(eRunType.SensitivitySS2VByPredator)

        End Sub


        Public Sub setNBlocksFromSensitivity(nBlocks As Integer)
            Dim n As Integer
            Dim icell As Integer, ipred As Integer, iprey As Integer
            Dim ssObj As cSensitivityToVulResults

            Debug.Assert(Me.m_lastRunSens <> eSensType.NotRun, "Sensitivity routine has not been run. The blocks can not be set.")

            If Me.m_lastRunSens = eSensType.NotRun Then

                Me.AddMessage(New cMessage(My.Resources.CoreMessages.F2TS_ERROR_SENSITIVITY_SETBLOCKS, _
                                        eMessageType.ErrorEncountered, _
                                        eCoreComponentType.EcosimFitToTimeSeries, _
                                        eMessageImportance.Warning))

                Exit Sub
            End If

            Try

                'sort the sensitivities biggest to smallest
                'see cSensitivityToVulResults.CompareTo()
                Me.m_lstSSResults.Sort()

                'clear out the old data
                Array.Clear(Me.VblockCode, 0, Me.VblockCode.Length)


                'now update the VblockCode() with the sorted sensitivities
                Select Case Me.m_lastRunSens

                    Case eSensType.PredColumn

                        'nBlocks is the user set number of blocks
                        'm_lstSSResults.Count is the actual number of pred/columns found by the sensitivity search
                        n = CInt(if(Me.m_lstSSResults.Count > nBlocks, nBlocks, Me.m_lstSSResults.Count))

                        icell = 0
                        For Each ssObj In Me.m_lstSSResults
                            icell = icell + 1
                            If icell > n Then Exit For

                            'convert the pred / prey indexes to an nLinks index
                            For ii As Integer = 1 To Me.m_esdata.inlinks
                                'all the prey of this predator
                                ipred = Me.m_esdata.jlink(ii)
                                If ssObj.iPred = ipred Then
                                    'Debug.Assert(VblockCode(ii) = 0)
                                    Me.VblockCode(ii) = icell
                                End If
                            Next ii

                        Next ssObj

                    Case eSensType.PredPreyCell

                        n = CInt(if(Me.m_lstSSResults.Count > nBlocks, nBlocks, Me.m_lstSSResults.Count))
                        icell = 0
                        For Each ssObj In Me.m_lstSSResults
                            icell = icell + 1
                            If icell > n Then Exit For

                            'convert the pred / prey indexes to an nLinks index
                            For ii As Integer = 1 To Me.m_esdata.inlinks
                                iprey = Me.m_esdata.ilink(ii) : ipred = Me.m_esdata.jlink(ii)
                                If ssObj.iPred = ipred And ssObj.iPrey = iprey Then
                                    Me.VblockCode(ii) = icell
                                End If
                            Next ii

                        Next ssObj

                End Select

            Catch ex As Exception
                cLog.Write(ex)

            End Try


        End Sub

#End Region ' SensitivitySS2VByPredatory

#Region " Public Search functions"

        ''' <summary>
        ''' 
        ''' </summary>
        Public Sub RunSearch()

            Dim results As cF2TSResults = Nothing
            Dim nSteps As Integer = 1 + 9

            Try

                If Me.m_core.m_TSData.AppliedNdatType = 0 Then
                    'no time series data loaded
                    Exit Sub
                End If

                '.. add init model logic here
                Dim failed As Integer
                Me.InitForRun(eRunType.Search)

                ' Start run
                Me.RunState = eRunType.Search
                Me.m_runstartedHandler(Me.RunState, nSteps)

                Me.DoEstimation(failed)

            Catch ex As Threading.ThreadAbortException
                ' Done

            Catch ex As Exception
                Me.AddMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.F2TS_ERROR, ex.Message), _
                                        eMessageType.ErrorEncountered, _
                                        eCoreComponentType.EcosimFitToTimeSeries, _
                                        eMessageImportance.Critical))

            End Try

            ' Done searching
            Me.RunState = eRunType.Idle
            Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            Me.m_runstoppedHandler(eRunType.Search)

        End Sub

#End Region ' Search

#Region " Notifications "


        ' Received delegate instances to report progress to
        Private m_runstartedHandler As RunStartedDelegate = Nothing
        Private m_runstepHandler As RunStepDelegate = Nothing
        Private m_runstoppedHandler As RunStoppedDelegate = Nothing
        Private m_AddMessageHandler As RunMessageDelegate = Nothing
        Private m_SendMessageHandler As RunMessageDelegate = Nothing
        Private m_runModelHandler As RunModelDelegate = Nothing

        ''' <summary>
        ''' Call the step handler
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub searchIterationStep()

            Try

                Me.setAIC(Me.m_data.nAICPars, Me.m_data.nAICData, Me.m_esdata.SS)

                DirectCast(Me.m_results, cSearchResults).IterSS = Me.m_esdata.SS
                DirectCast(Me.m_results, cSearchResults).AIC = Me.m_data.AIC
                DirectCast(Me.m_results, cSearchResults).nAICPars = Me.m_data.nAICPars

                Me.m_results.iStep = Me.m_estIter

                If Me.m_runstepHandler IsNot Nothing Then
                    Me.m_runstepHandler()
                End If
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".searchIterationStep() Error: " & ex.Message)
            End Try

        End Sub

        Private Sub SendMessage(msg As cMessage)
            Try
                Me.m_SendMessageHandler(msg)
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Sub

        Private Sub AddMessage(msg As cMessage)
            Try
                Me.m_AddMessageHandler(msg)
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Sub

#End Region ' Notifications

#Region " Model Logic "

        Private Sub InitForRun(runType As eRunType)

            Try

                'get the core data for this run
                Me.m_esdata = Me.m_core.m_EcoSimData
                Me.m_tsData = Me.m_core.m_TSData
                Me.m_data = Me.m_core.m_FitToTimeSeriesData

                Me.m_lastRunSens = eSensType.NotRun

                'for convenience set local variables to values set by interface
                'these variables can not be changed by the interface during a run so this should be Ok
                Me.AnomalySearch = Me.m_data.bAnomalySearch
                Me.Numspline = Me.m_data.nNumSplinePoints
                Me.PPyear1 = Me.m_data.FirstYear
                Me.PPyear2 = Me.m_data.LastYear
                Me.ForceNo = Me.m_data.iCatchAnomalySearchShapeNumber

                If Me.m_data.bVulnerabilitySearch Then
                    Me.pvVul = Me.m_data.VulnerabilityVariance
                Else
                    'this will turn off the Vulnerability search by setting pv(maxpars) to zero for all vulnerability parameter
                    'this variable can not be varied so it is not searched see DoEstimation
                    Me.pvVul = 0
                End If

                'make sure the fit to timeseries search is turned on
                Me.StopRun = False

                'Init Ecosim

                'make sure the fishing policy search is turned off
                Me.m_core.m_SearchData.SearchMode = eSearchModes.FitToTimeSeries
                'No timestep ouput
                Me.m_core.m_EcoSimData.bTimestepOutput = False

                'make sure ecosim does not call the interface 
                'setting bTimestepOutput = False should have had the same effect
                Me.m_core.m_Ecosim.TimeStepDelegate = Nothing

                ' Set V to default before initialization of Ecosim so it uses the new V's
                If Me.m_data.UseDefaultV Then
                    Me.m_core.SetVToDefault()
                End If

                'Now Init Ecosim
                Me.initEcosimForSearchIteration()

                Me.TotalTime = Me.m_esdata.NumYears

                If Me.nBlockCodes = 0 Then Me.nBlockCodes = Me.m_esdata.inlinks
                ReDim Me.VBlock(Me.nBlockCodes)
                ReDim Me.IsBlockEstimated(Me.nBlockCodes)

                'VblockCode() should have been set by an interface
                'however if this is run from a plugin then it is possible for VblockCode() to be null
                If Me.VblockCode Is Nothing Then
                    ReDim Me.VblockCode(Me.m_esdata.inlinks)
                End If

                'Clear out all selected codes > then the max number of blocks
                Dim n As Integer = Me.VBlock.Length - 1
                For i As Integer = 1 To Me.m_esdata.inlinks
                    If Me.VblockCode(i) > n Then Me.VblockCode(i) = 0
                Next i

                'create the results object for this type of run
                Me.m_results = cF2TSResultsFactory.Create(runType)

                'VBlock() and VblockCode(inLinks) should have been set by the interface
                Me.SetVblock(Me.m_esdata)

                'get Base SS from ecosim 
                Me.m_ecosim.RunModelValue(Me.TotalTime, Nothing, 0)

                Me.updateAICNPars()

                'set the baseSS in the results object that was calculated above by ecosim
                DirectCast(Me.m_results, cF2TSResults).BaseSS = Me.m_esdata.SS

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("Initialization error: " & ex.Message, ex)
            End Try

        End Sub

        ''' <summary>
        ''' Count the number of parameters being searched for. Used to compute AIC.
        ''' </summary>
        Public Sub updateAICNPars()

            'nAICData is updated by the manager
            'Me.m_data.nAICData = m_tsData.NdatType \ 3

            'WARNING SetVblock() must be called first to set IsBlockEstimated()
            'calculate the number of parameters
            Me.m_data.nAICPars = 0

            If Me.m_data.bVulnerabilitySearch Then
                For i As Integer = 1 To Me.IsBlockEstimated.Length - 1
                    If Me.IsBlockEstimated(i) Then Me.m_data.nAICPars += 1
                Next i
            End If

            If Me.m_data.bAnomalySearch Then
                Dim n As Integer
                If Me.m_data.nNumSplinePoints > 0 Then
                    'using spline points
                    n = Me.m_data.nNumSplinePoints
                Else
                    'using number of years
                    n = Me.m_data.LastYear - Me.m_data.FirstYear
                End If

                Me.m_data.nAICPars += n

            End If

        End Sub

        ''' <summary>
        ''' Populate cF2TSDataStructures.AIC value
        ''' </summary>
        Public Sub setAIC(nPars As Single, nData As Single, SS As Single)

            If (Me.m_data Is Nothing) Then Return

            'Up to 20140328 this was:
            Me.m_data.AIC = 2.0F * nPars + nData * CSng(Math.Log(SS))
            'but VC changed this to a more standard derivation of AIC (see Peru Ecosim paper)
            'based on advice from Steve Mackinson in 2011:
            '            "AIC = nlog(RSS/n) + 2k + constant*n   (ref: Venables and Riley 2002) 
            'where k is the number of parameters estimated and n is the number of observations being fitted to (i.e. n is the number of time series values, this being number of series used multiplied by the number of years for each). The constant*n can be ignored if n is the same (i.e. the observation data to be fitted to is the same) and we are comparing between alternative hypotheses.
            'So, using AIC to compare among alternative hypotheses (model parameterizations) in Ecosim, we need to calculate:
            'AIC = nlog(minSS (from ecosim)/n) + 2k
            'AICc is AIC with a second order correction for small sample sizes, to start with:
            'AICc = AIC + 2k(k-1)/n-k-1   where n is the number of observations
            'Since AICc converges to AIC as n gets large, AICc should be employed regardless of sample size (Burnham and Anderson, 2004)."

            If nData > 0 Then
                Me.m_data.AIC = 2.0F * nPars + nData * CSng(Math.Log(SS / nData))
                If nData - nPars > 1 Then
                    Me.m_data.AIC += 2 * nPars * (nPars + 1) / (nData - nPars - 1)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Initialize Ecosim before each search iteration
        ''' </summary>
        ''' <remarks>In EwE5 this is called PrepareSimSpace()</remarks>
        Private Sub initEcosimForSearchIteration()

            Me.m_ecosim.Init(True)

            'm_ecosim.Set_pbm_pbbiomass()
            'm_ecosim.RedimForSearchRun()
            ''  m_core.m_EcoSim.RedimEcoSimVars()
            'm_ecosim.CalcEatenOfBy()
            'm_ecosim.CalcStartEatenOfBy()
            'm_ecosim.InitialState()
            'm_ecosim.setpred(m_core.m_EcoSimData.StartBiomass)

        End Sub


        Private Sub DoEstimation(ByRef Failed As Integer)
            Dim t As Integer = 0
            Dim EvalCount As Integer = 0
            Dim C As Integer = 0
            Dim det As Single = 0.0!

            Dim fbmsg As cFeedbackMessage
            '****************nonlinear estimation procedures for improving par estimates

            Dim MaxObs As Integer

            Try

                'On Local Error GoTo fitfailed
                Failed = 0

                MaxObs = Me.m_tsData.Iobs

                Me.MaxPars = Me.m_esdata.NumYears + Me.VBlock.GetUpperBound(0)    '15
                If Me.VBlock.GetUpperBound(0) + Me.PPyear2 - Me.PPyear1 > Me.MaxPars Then
                    Me.MaxPars = Me.VBlock.GetUpperBound(0) + Me.PPyear2 - Me.PPyear1
                End If
                ReDim Me.Se(Me.MaxPars, MaxObs), Me.Sold(Me.MaxPars), Me.Xy(Me.MaxPars)
                ReDim Me.Ybase(MaxObs), Me.St(Me.MaxPars) ', Wt(MaxObs)
                ReDim Me.Ipn(Me.MaxPars), Me.amat(Me.MaxPars, Me.MaxPars)
                ReDim Me.Vi(Me.MaxPars, Me.MaxPars), Me.Cl(Me.MaxPars, Me.MaxPars)
                ReDim Me.cy(Me.MaxPars)
                ReDim Me.Penter(Me.MaxPars), Me.Po(Me.MaxPars), Me.pv(Me.MaxPars), Me.P(Me.MaxPars), Me.paramname(Me.MaxPars)

                Me.Nobs = Me.m_tsData.Iobs
                Me.SetPfromPars(Me.Po)

                'set the parameter variance 
                If Me.AnomalySearch Then
                    If Me.Numspline < 2 Then
                        For Me.i = Me.PPyear1 To Me.PPyear2
                            Me.pv(Me.i) = Me.m_data.PPVariance
                        Next
                    Else
                        For Me.i = 1 To Me.Numspline
                            Me.pv(Me.i) = Me.m_data.PPVariance
                        Next
                    End If
                End If

                'if vulnerability variance = 0 then these parameters will not be counted in 'n' see below
                'this means the vulnerability parameters will not be included in the search
                'InitForRun() decides if pvVul is set or not based on the bVulnerabilitySearch flag
                For Me.i = 1 To Me.IsBlockEstimated.GetUpperBound(0)   '15
                    If Me.IsBlockEstimated(Me.i) Then
                        Me.pv(Me.TotalTime + Me.i) = Me.pvVul 'pvVul was set in IntForRun
                    Else
                        Me.pv(Me.TotalTime + Me.i) = 0
                    End If
                Next

                'define which parameters are to be varied, count these up (in N), and store their indices in the vector IP
                'jb
                'n defines the number of parameters to loop over in sub290 (number of parameters to search for, number of calls to ecosim)
                'ipn() points to the index in P() to get the parameter from e.g. parameter = P(Ipn(i))
                'n is counted from pv(MaxPars) (parameter variance)
                'This decides what parameters are searched if pv(iparameter) is zero the parameter is not used
                Me.ip = 0
                Me.n = Me.MaxPars
                For Me.i = 1 To Me.MaxPars
                    If Me.pv(Me.i) > 0 Then
                        Me.ip = Me.ip + 1
                        Me.Ipn(Me.ip) = Me.i
                    Else
                        Me.n = Me.n - 1
                    End If
                Next

                If Me.n = 0 Then
                    'message

                    Me.AddMessage(New cMessage(My.Resources.CoreMessages.F2TS_ERROR_INTERACTIONS, _
                                            eMessageType.ErrorEncountered, _
                                            eCoreComponentType.EcosimFitToTimeSeries, _
                                            eMessageImportance.Warning))
                    Exit Sub
                End If

                'REM set some initial conditions for iteration counters
                Me.rmax = 1
                Me.Jit = 0
                Me.ic = 0
                Me.SO = 1.0E+30
                Me.Rmin = 0.1
                Me.dinc = 0.0001
                Me.m_estIter = 0
                EvalCount = 0
                For Me.i = 1 To MaxObs
                    Me.m_tsData.Wt(Me.i) = 1
                Next

                For Me.i = 1 To Me.MaxPars
                    Me.P(Me.i) = Me.Po(Me.i)
                Next

                Me.DF = Me.Nobs - Me.n
                If Me.DF < 1 Then Me.DF = 1

                '190:            ' Print "ITERATION BEGINS; HIT ANY KEY ONCE IF NECESSARY TO INTERRUPT"
                '200 GoSub 290: GoSub 550: Rem compute sensitivities and newton correction step for this parameter combination
200:
                Me.sub290()
                Me.sub550()

                Me.m_estIter = Me.m_estIter + 1
                Me.searchIterationStep()

                If Me.StopRun = True Then Exit Sub
                If Me.m_estIter > 500 Then GoTo 250

                If Me.StopIndex > 0 Then
                    fbmsg = New cFeedbackMessage(My.Resources.CoreMessages.F2TS_PROMPT_ITERATIONS, _
                                                 eCoreComponentType.EcosimFitToTimeSeries, _
                                                 eMessageType.Any, _
                                                 eMessageImportance.Information, _
                                                 eMessageReplyStyle.YES_NO)
                    fbmsg.Reply = eMessageReply.NO

                    If (Not Me.m_data.RunSilent) Then
                        Me.SendMessage(fbmsg)
                    End If
                    If fbmsg.Reply = eMessageReply.NO Then GoTo 250
                    '  If MsgBox("MORE ITERATIONS (y/n)?", MsgBoxStyle.YesNo) = vbNo Then GoTo 250
                End If

                For Me.i = 1 To Me.n
                    If Math.Abs(Me.St(Me.i) / (Me.P(Me.Ipn(Me.i)) + Me.dinc)) > 0.001 Then GoTo 220 REM seek correction step if newton step is still large
                Next

                GoTo 250
                '   220 GoSub 700: Rem find and apply corrected step if possible

                'VC Sep 08, had a case where the grad check in sub700 would estimate Grad to be very small, then kick out 
                'of sub700, but next check above of 
                'If Math.Abs(St(i) / (P(Ipn(i)) + dinc)) > 0.001
                'would cause it to go back to 220 and back to sub700, etc.
                'Discussed this with Carl
220:            If Me.m_estIter = 1 Or Math.Abs(Me.Grad) > 0.00000000001 Then
                    Me.sub700()
                Else 'no difference anymore so continue with 
                    GoTo 250
                End If

240:
                If Me.Rr2 >= Me.Rmin Then
                    Me.sub900()
                    GoTo 200 REM start another nonlinear iteration if key has not been hit or convergence found
                End If

250:
                Me.sub300()
                Me.sub900()
                Me.MatInv(Me.n, Me.amat, det)

                Me.searchIterationStep()

                fbmsg = New cFeedbackMessage(My.Resources.CoreMessages.F2TS_PROMPT_CONVERGED, _
                                                 eCoreComponentType.EcosimFitToTimeSeries, _
                                                 eMessageType.Any, _
                                                 eMessageImportance.Information, _
                                                 eMessageReplyStyle.YES_NO)
                fbmsg.Reply = eMessageReply.NO

                If (Not Me.m_data.RunSilent) Then
                    Me.SendMessage(fbmsg)
                End If

                If fbmsg.Reply = eMessageReply.YES Then GoTo 220

                '  searchIterationStep()

                '   If MsgBox("ESTIMATES CONVERGED; MORE ITERATIONS?", MsgBoxStyle.YesNo) = vbYes Then GoTo 220

                'MsgBox "Estimates apparently converged"
                '  frmSearch.Res.Visible = False
                Me.StopIndex = 0

                Exit Sub

            Catch ex As Threading.ThreadAbortException
                'we do not know why this happen 
                'the most likey case is the form has been closed and that aborted the thread
                'anyway clean up
                Failed = 1
                Me.SetParsFromP(Me.Po)

                Me.m_runstoppedHandler(Me.RunState)
                Me.RunState = eRunType.Idle

            Catch ex As Exception

                cLog.Write(ex)

                Failed = 1
                Me.SetParsFromP(Me.Po)
                Debug.Assert(False, ex.Message)

                Me.m_runstoppedHandler(Me.RunState)
                Me.RunState = eRunType.Idle

                Me.AddMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.F2TS_ERROR_ESTIMATION, ex.Message), _
                                        eMessageType.ErrorEncountered, _
                                        eCoreComponentType.EcosimFitToTimeSeries, _
                                        eMessageImportance.Warning))
            End Try


            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'ORIGINAL EWE5 CODE
            '
            ' PRINT "WAIT...": AP = A: GOSUB 1050: A = AP: REM compute (X'X) inverse for error statistics calculations
            ' PRINT "WANT TO SEE PARAMETER ERROR STATS (y/n)"; : INPUT Y$: IF Y$ = "Y" OR Y$ = "y" THEN GOSUB 5910
            '290:        REM routine to calculate sensitivity matrix SE(k,j) of all observations j to all parameters P(k), j=1 to ni+naux and k=1 to n, by incrementing each parameter slightly and redoing simulation
            'DoEvents: If StopEstimation = True Then Exit Sub
            '        'Dim TS As Single
            '    GoSub 300: Sbase = Ss: For j = 1 To Nobs: Ybase(j) = Yhat(j): Next
            '        Va = Ss / DF : var = Va : Vmax = Exp(-0.5 * Ss / Va) : Vc = 0.05 * Vmax : Vp = 0 'If Np > 0 Then Vp = Sp / Np
            '        For kkkk = 1 To n
            '            'TS = 0#
            '            ip = Ipn(kkkk)
            '            Dp = dinc * P(ip)
            '            If Dp = 0 Then Dp = dinc
            '            P(ip) = P(ip) + Dp
            '        GoSub 300
            '            For j = 1 To Nobs
            '                Se(kkkk, j) = (Yhat(j) - Ybase(j)) / Dp
            '                'TS = TS + Abs(Se(kkkk, j))
            '                'PRINT kkkk, j, se(kkkk, j)
            '            Next
            '            P(ip) = P(ip) - Dp
            '            DoEvents()
            '            If StopEstimation = True Then Exit Sub
            '        Next
            '        Return
            '300:    REM routine to calculate yhat(1...nobs),er(1...nobs)=yobs-yhat, and
            '        'SS=sum of (er)^2
            '        'next 4 lines are a test model for checking nonlinear search
            '        '  Yhat(1) = P(1): Yhat(2) = P(2): Yhat(3) = P(3)
            '        '  er(1) = 10 - Yhat(1): er(2) = 20 - Yhat(2): er(3) = 30 - Yhat(3)
            '        '  SS = 0: For i = 1 To 3: SS = SS + er(i) * er(i): Next
            '        '  Nobs = 3

            '        '******set model parameters from current P estimation vector
            '        '**** put model to predict yhat's, er's, and add up SS here
            '        SetParsFromP(P())
            '        RunModelFast(TotalTime, Ss)
            '        'Ss = 0
            '        'For Iobs = 1 To Nobs
            '        '    Ss = Ss + Erpred(Iobs) * Erpred(Iobs) * Wt(Iobs)
            '        'Next
            '        ' Debug.Print SS
            '        ' good form is  SS = SS + ER(i) * ER(i) * WT(i)  where ER(i)=yobs(i)-yhat(i)
            '        Return REM end of routine for calculating ss and yhat values
            '550:    REM routine to solves (X'X)(st)=X'(er) by Cholesky decompostion, using X=SE sensitivity matrix augmented by prior variances and er=fitting error vector; output is newton parameter correction step vector st(1...n)
            '        For i = 1 To n : Sold(i) = St(i) : ip = Ipn(i) : Xy(i) = (P(ip) - Po(ip)) * Va / PV(ip) : Next
            '        For i = 1 To n
            '            Su = 0 : For K = 1 To Nobs : Su = Su + Wt(K) * Se(i, K) * Erpred(K) : Next K
            '            Xy(i) = Su + Xy(i)
            '            For j = 1 To i
            '                Su = 0 : For K = 1 To Nobs : Su = Su + Wt(K) * Se(i, K) * Se(j, K) : Next K
            '                amat(i, j) = Su : amat(j, i) = Su
            '            Next j : Next i
            '        For i = 1 To n : amat(i, i) = amat(i, i) + Va / PV(Ipn(i)) : Next
            '        For i = 1 To n : For j = 1 To n : Vi(i, j) = amat(i, j) : Next : Next
            '        Cl(1, 1) = Sqr(amat(1, 1)) : For i = 2 To n : Cl(i, 1) = amat(i, 1) / Cl(1, 1) : Next
            '        For i = 2 To n : If i = 2 Then GoTo 641
            '            For j = 2 To i - 1 : Ct = 0 : For K = 1 To j - 1 : Ct = Ct + Cl(i, K) * Cl(j, K) : Next : Cl(i, j) = (amat(i, j) - Ct) / Cl(j, j) : Next
            '641:        Ct = 0 : For K = 1 To i - 1 : Ct = Ct + Cl(i, K) ^ 2 : Next : Cl(i, i) = Sqr(amat(i, i) - Ct)
            '        Next
            '        cy(1) = Xy(1) / Cl(1, 1) : For i = 2 To n : Ct = 0 : For j = 1 To i - 1 : Ct = Ct + Cl(i, j) * cy(j) : Next : cy(i) = (Xy(i) - Ct) / Cl(i, i) : Next
            '        St(n) = cy(n) / Cl(n, n) : If n = 1 Then GoTo 650
            '        For i = n - 1 To 1 Step -1 : Ct = 0 : For j = n To i + 1 Step -1 : Ct = Ct + St(j) * Cl(j, i) : Next : St(i) = (cy(i) - Ct) / Cl(i, i) : Next
            '650:    Return
            '700:    REM routine to find an acceptable step length (fraction of st) if possible, and applies it to the parameter vector P (this algorithm from p. in Bard, 1974)
            'DoEvents: If StopEstimation = True Then Exit Sub
            '        Rr2 = 1 : Grad = 0 : For i = 1 To n : Grad = Grad - St(i) * Xy(i) : Next : Rs = rmax / 2 ^ Jit
            '        If Abs(Grad) < 0.00000000001 Then Return
            '        For i = 1 To n : ip = Ipn(i) : P(ip) = P(ip) + Rs * St(i) : Next
            'DoEvents: If StopEstimation = True Then Exit Sub
            '        GoSub 300: Stry = Ss
            '        Rbet = Grad * Rs * Rs / (2 * (Grad * Rs + Sbase - Stry)) : If Stry >= Sbase Then GoTo 750
            '        Jit = Jit / 2
            '        If Rbet < 0 Then Rbet = 2 * Rs
            '        Rnew = Rbet : If Rnew > rmax Then Rnew = rmax
            'DoEvents: If StopEstimation = True Then Exit Sub
            '        Rdel = Rnew - Rs: For i = 1 To n: ip = Ipn(i): P(ip) = P(ip) + Rdel * St(i): Next: GoSub 300: Snew = Ss
            '        If Snew < Stry Then GoTo 795
            '        For i = 1 To n : ip = Ipn(i) : P(ip) = P(ip) - Rdel * St(i) : Next : GoTo 795
            '750:    Rnew = Rs : For i = 1 To n : ip = Ipn(i) : P(ip) = P(ip) - Rs * St(i) : Next
            '755:    Rr2 = 0.75 * Rnew : If Rr2 > Rbet Then Rr2 = Rbet
            '        DoEvents()
            '        If StopIndex = 1 Then Return
            '        If StopEstimation = True Then Exit Sub
            '        If Rr2 < 0.25 * Rnew Then Rr2 = 0.25 * Rnew
            '        If Rr2 < Rmin Then
            '            For i = 1 To n : ip = Ipn(i) : P(ip) = P(ip) - Rs * St(i) : Next
            '            ' Print "cannot find improved estimates": Return
            '        End If
            '        For i = 1 To n: ip = Ipn(i): P(ip) = P(ip) + Rr2 * St(i): Next: GoSub 300: Ss2 = Ss: Jit = Jit + 1
            '        If Ss2 < Sbase Then GoTo 795
            '        Den = 2 * (Grad * Rnew + Sbase - Ss2)
            '        If Den < 0.000000000000001 Then GoTo 798
            '        Rnew = Rr2 : Rs = Grad * Rnew * Rnew / Den
            '        For i = 1 To n : ip = Ipn(i) : P(ip) = P(ip) - Rr2 * St(i) : Next : GoTo 755
            '798:    For i = 1 To n : ip = Ipn(i) : P(ip) = P(ip) - Rr2 * St(i) : Next
            '        '041202VC: we were having trouble at Galveston workshop, where SS would be different on frmSearch
            '        'and on return to ecosim. Carl's solution: place a runmodelfast call here
            '        'RunModelFast TotalTime, Ss
            '        '041202VC: doesn't work though as what is shown on form is lowest ss, while what is kept in memory is
            '        'the last of the ss from the series of simrun's done for each sbase printed.
            '        'Debug.Print Ss
            '795:    ' end of trust region method to find improving parameter step Return
            '        Return
            '        'VC040131
            '        'I think that I found the problem in Doestimation that lets the code accept increasing SS parameter values.
            '        'look for the line "795  '  end of  trust region...".
            '        'Change the lines above it as shown below
            '        '(change if den<0.0000 to branch to 798, and add line 798 to remove the parameter correction).
            '        'Carl
            '        '
            '        '        If Ss2 < Sbase Then GoTo 795
            '        '        Den = 2 * (Grad * Rnew + Sbase - Ss2)
            '        '        If Den < 0.000000000000001 Then GoTo 798
            '        '        Rnew = Rr2: Rs = Grad * Rnew * Rnew / Den
            '        '        For i = 1 To n: ip = Ipn(i): P(ip) = P(ip) - Rr2 * St(i): Next: GoTo 755
            '        '798        For i = 1 To n: ip = Ipn(i): P(ip) = P(ip) - Rr2 * St(i): Next'
            '        '
            '        '795 ' end of trust region method to find improving parameter step Return
            '        '
            '900:    REM save parameter estimates
            '        SetParsFromP(P())
            '        'frmSearch.Res.Print iter; ":"; Ss


            '        'RegVar = SS / Df
            '        Return
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


        End Sub


        Sub SetVblock(ByRef esData As cEcosimDatastructures)
            Dim i As Integer, j As Integer, ii As Integer
            Dim iBlock As Integer
            For i = 1 To Me.IsBlockEstimated.GetUpperBound(0)
                Me.IsBlockEstimated(i) = False
            Next

            For ii = 1 To esData.inlinks
                i = esData.ilink(ii)
                j = esData.jlink(ii)
                iBlock = Me.VblockCode(ii)
                If iBlock > 0 And iBlock < Me.VBlock.Length Then
                    Me.VBlock(iBlock) = esData.VulMult(i, j)
                    Me.IsBlockEstimated(iBlock) = True
                End If
            Next

        End Sub

        Private Sub SetPfromPars(Par() As Single)
            'sets Par array to parameters from the model
            Dim i As Integer, i1 As Integer, i2 As Integer, im As Integer, Pvl As Single
            If Me.AnomalySearch = True Then
                If Me.Numspline < 2 Then
                    For i = 1 To Me.m_esdata.NumYears
                        Par(i) = CSng(Math.Log(Me.m_esdata.zscale(1 + 12 * (i - 1), Me.ForceNo) + 1.0E-20))   'Added 1E-20 per cjw email to vc 26sep00
                    Next
                Else
                    ReDim Me.Xspline(Me.Numspline)
                    i1 = 12 * (Me.PPyear1 - 1) + 1
                    i2 = 12 * Me.PPyear2
                    For i = 1 To Me.Numspline
                        im = CInt(i1 + (i2 - i1) * (i - 1) / (Me.Numspline - 1))
                        Me.Xspline(i) = im
                        Par(i) = CSng(Math.Log(Me.m_esdata.zscale(im, Me.ForceNo) + 1.0E-20))
                    Next
                End If
            End If
            'Par(TotalTime + 1) = VulMultAll
            For i = 1 To Me.VBlock.GetUpperBound(0)  '15
                If Me.IsBlockEstimated(i) = True And Me.VBlock(i) > 0 Then
                    Pvl = CSng(Me.VBlock(i) - 1.0)
                    If Pvl < 0.000001 Then Pvl = 0.000001
                    Par(Me.m_esdata.NumYears + i) = CSng(Math.Log(Pvl))
                End If
            Next
        End Sub

        Sub MatInv(n As Integer, amat(,) As Single, det As Single)
            Dim i As Integer, i1 As Integer, i2 As Integer
            ' inverts matrix A of order N; used to estimate parameter covariance matrix
            For i = 1 To n
                det = amat(i, i)
                amat(i, i) = 1
                For i1 = 1 To n
                    amat(i, i1) = amat(i, i1) / det
                Next
                For i2 = 1 To n
                    If i2 = i Then GoTo 1140
                    If amat(i2, i) = 0 Then GoTo 1140
                    det = amat(i2, i)
                    amat(i2, i) = 0
                    For i1 = 1 To n
                        amat(i2, i1) = amat(i2, i1) - det * amat(i, i1)
                    Next
1140:           Next
            Next i REM end of matrix inversion routine


        End Sub



        Private Sub sub290()
            '290:    REM routine to calculate sensitivity matrix SE(k,j) of all observations j to all parameters P(k), j=1 to ni+naux and k=1 to n, 
            'by incrementing each parameter slightly and redoing simulation
            If Me.StopRun = True Then Exit Sub

            Dim sumDYDX As Single, sumBase As Single ', sumY As Single
            '     GoSub 300: 

            Me.sub300()

            Me.Sbase = Me.Ss
            For Me.j = 1 To Me.Nobs
                Me.Ybase(Me.j) = Me.m_tsData.Yhat(Me.j)
            Next
            Me.Va = Me.Ss / Me.DF
            Me.var = Me.Va
            Me.Vmax = CSng(Math.Exp(-0.5 * Me.Ss / Me.Va))
            Me.Vc = CSng(0.05 * Me.Vmax)
            Me.Vp = 0 'If Np > 0 Then Vp = Sp / Np
            For Me.kkkk = 1 To Me.n
                Me.ip = Me.Ipn(Me.kkkk)
                Me.Dp = Me.dinc * Me.P(Me.ip)
                If Me.Dp = 0 Then Me.Dp = Me.dinc
                Me.P(Me.ip) = Me.P(Me.ip) + Me.Dp

                'call the model
                Me.sub300()

                'tell the interface that Ecosim has been called
                Me.modelCalled(Me.kkkk, Me.n)

                For Me.j = 1 To Me.Nobs
                    Me.Se(Me.kkkk, Me.j) = (Me.m_tsData.Yhat(Me.j) - Me.Ybase(Me.j)) / Me.Dp
                    sumDYDX = sumDYDX + Math.Abs(Me.Se(Me.kkkk, Me.j))
                    'sumY = sumY + (m_tsData.Yhat(j) - Ybase(j))
                    sumBase = sumBase + (Me.Ybase(Me.j))
                Next

                'System.Console.WriteLine("Var = " & kkkk.ToString & ", Sum SS = " & sumDYDX.ToString & ", Sum Y = " & sumY.ToString & ", sum base = " & sumBase.ToString)

                Me.P(Me.ip) = Me.P(Me.ip) - Me.Dp
                '    DoEvents()
                If Me.StopRun = True Then Exit Sub
            Next

            Return

        End Sub


        Private Sub sub300()
            '300:    REM routine to calculate yhat(1...nobs),er(1...nobs)=yobs-yhat, and
            'SS=sum of (er)^2

            'next 4 lines are a test model for checking nonlinear search
            '  Yhat(1) = P(1): Yhat(2) = P(2): Yhat(3) = P(3)
            '  er(1) = 10 - Yhat(1): er(2) = 20 - Yhat(2): er(3) = 30 - Yhat(3)
            '  SS = 0: For i = 1 To 3: SS = SS + er(i) * er(i): Next
            '  Nobs = 3

            '******set model parameters from current P estimation vector
            '**** put model to predict yhat's, er's, and add up SS here
            Me.SetParsFromP(Me.P)
            Me.m_ecosim.RunModelValue(Me.TotalTime, Nothing, 0)

            Me.Ss = Me.m_esdata.SS
            'System.Console.WriteLine("SS = " + Ss.ToString)

            'For Iobs = 1 To Nobs
            '    Ss = Ss + Erpred(Iobs) * Erpred(Iobs) * Wt(Iobs)
            'Next
            ' Debug.Print SS

            ' good form is  SS = SS + ER(i) * ER(i) * WT(i)  where ER(i)=yobs(i)-yhat(i)
            Return REM end of routine for calculating ss and yhat values
        End Sub

        Private Sub sub550()
            '550:    REM routine to solves (X'X)(st)=X'(er) by Cholesky decompostion, 
            'using X=SE sensitivity matrix augmented by prior variances and er=fitting error vector; 
            'output is newton parameter correction step vector st(1...n)
            For Me.i = 1 To Me.n
                Me.Sold(Me.i) = Me.St(Me.i)
                Me.ip = Me.Ipn(Me.i)
                Me.Xy(Me.i) = (Me.P(Me.ip) - Me.Po(Me.ip)) * Me.Va / Me.pv(Me.ip)
            Next

            For Me.i = 1 To Me.n

                Me.Su = 0
                For Me.K = 1 To Me.Nobs
                    Me.Su = Me.Su + Me.m_tsData.Wt(Me.K) * Me.Se(Me.i, Me.K) * Me.m_tsData.Erpred(Me.K)
                Next Me.K

                Me.Xy(Me.i) = Me.Su + Me.Xy(Me.i)
                For Me.j = 1 To Me.i
                    Me.Su = 0 : For Me.K = 1 To Me.Nobs
                        Me.Su = Me.Su + Me.m_tsData.Wt(Me.K) * Me.Se(Me.i, Me.K) * Me.Se(Me.j, Me.K)
                    Next Me.K
                    Me.amat(Me.i, Me.j) = Me.Su
                    Me.amat(Me.j, Me.i) = Me.Su
                Next Me.j
            Next Me.i

            For Me.i = 1 To Me.n
                Me.amat(Me.i, Me.i) = Me.amat(Me.i, Me.i) + Me.Va / Me.pv(Me.Ipn(Me.i))
            Next Me.i

            For Me.i = 1 To Me.n
                For Me.j = 1 To Me.n
                    Me.Vi(Me.i, Me.j) = Me.amat(Me.i, Me.j)
                Next Me.j
            Next Me.i

            Me.Cl(1, 1) = CSng(Math.Sqrt(Me.amat(1, 1)))
            For Me.i = 2 To Me.n
                Me.Cl(Me.i, 1) = Me.amat(Me.i, 1) / Me.Cl(1, 1)
            Next

            For Me.i = 2 To Me.n
                If Me.i = 2 Then GoTo 641

                For Me.j = 2 To Me.i - 1
                    Me.Ct = 0
                    For Me.K = 1 To Me.j - 1
                        Me.Ct = Me.Ct + Me.Cl(Me.i, Me.K) * Me.Cl(Me.j, Me.K)
                    Next
                    Me.Cl(Me.i, Me.j) = (Me.amat(Me.i, Me.j) - Me.Ct) / Me.Cl(Me.j, Me.j)
                Next
641:            Me.Ct = 0
                For Me.K = 1 To Me.i - 1
                    Me.Ct = CSng(Me.Ct + Me.Cl(Me.i, Me.K) ^ 2)
                Next
                Me.Cl(Me.i, Me.i) = CSng(Math.Sqrt(Me.amat(Me.i, Me.i) - Me.Ct))
            Next

            Me.cy(1) = Me.Xy(1) / Me.Cl(1, 1)

            For Me.i = 2 To Me.n
                Me.Ct = 0
                For Me.j = 1 To Me.i - 1
                    Me.Ct = Me.Ct + Me.Cl(Me.i, Me.j) * Me.cy(Me.j)
                Next
                Me.cy(Me.i) = (Me.Xy(Me.i) - Me.Ct) / Me.Cl(Me.i, Me.i)
            Next

            Me.St(Me.n) = Me.cy(Me.n) / Me.Cl(Me.n, Me.n)
            If Me.n = 1 Then GoTo 650
            For Me.i = Me.n - 1 To 1 Step -1
                Me.Ct = 0
                For Me.j = Me.n To Me.i + 1 Step -1
                    Me.Ct = Me.Ct + Me.St(Me.j) * Me.Cl(Me.j, Me.i)
                Next
                Me.St(Me.i) = (Me.cy(Me.i) - Me.Ct) / Me.Cl(Me.i, Me.i)
            Next
650:        Return

        End Sub


        Private Sub sub900()
            '900:    REM save parameter estimates
            Me.SetParsFromP(Me.P)
            'frmSearch.Res.Print iter; ":"; Ss

            'RegVar = SS / Df
            Return

        End Sub

        Private Sub sub700()
700:        REM routine to find an acceptable step length (fraction of st) if possible, and applies it to the parameter vector P (this algorithm from p. in Bard, 1974)
            If Me.StopRun = True Then Exit Sub
            Me.Rr2 = 1
            Me.Grad = 0

            For Me.i = 1 To Me.n
                Me.Grad = Me.Grad - Me.St(Me.i) * Me.Xy(Me.i)
            Next
            Me.Rs = CSng(Me.rmax / 2 ^ Me.Jit)

            If Math.Abs(Me.Grad) < 0.00000000001 Then Return

            For Me.i = 1 To Me.n
                Me.ip = Me.Ipn(Me.i)
                Me.P(Me.ip) = Me.P(Me.ip) + Me.Rs * Me.St(Me.i)
            Next
            If Me.StopRun = True Then Exit Sub
            ' GoSub 300: 
            Me.sub300()
            Me.Stry = Me.Ss
            Me.Rbet = Me.Grad * Me.Rs * Me.Rs / (2 * (Me.Grad * Me.Rs + Me.Sbase - Me.Stry))
            If Me.Stry >= Me.Sbase Then GoTo 750

            Me.Jit = CInt(Me.Jit / 2)
            If Me.Rbet < 0 Then Me.Rbet = 2 * Me.Rs
            Me.Rnew = Me.Rbet
            If Me.Rnew > Me.rmax Then Me.Rnew = Me.rmax

            If Me.StopRun = True Then Exit Sub

            Me.Rdel = Me.Rnew - Me.Rs
            For Me.i = 1 To Me.n
                Me.ip = Me.Ipn(Me.i) : Me.P(Me.ip) = Me.P(Me.ip) + Me.Rdel * Me.St(Me.i)
            Next

            'GoSub 300
            Me.sub300()

            Me.Snew = Me.Ss
            If Me.Snew < Me.Stry Then GoTo 795
            For Me.i = 1 To Me.n
                Me.ip = Me.Ipn(Me.i)
                Me.P(Me.ip) = Me.P(Me.ip) - Me.Rdel * Me.St(Me.i)
            Next

            GoTo 795

750:        Me.Rnew = Me.Rs
            For Me.i = 1 To Me.n
                Me.ip = Me.Ipn(Me.i)
                Me.P(Me.ip) = Me.P(Me.ip) - Me.Rs * Me.St(Me.i)
            Next

755:        Me.Rr2 = CSng(0.75 * Me.Rnew)
            If Me.Rr2 > Me.Rbet Then Me.Rr2 = Me.Rbet

            If Me.StopIndex = 1 Then Return
            If Me.StopRun = True Then Exit Sub
            If Me.Rr2 < 0.25 * Me.Rnew Then Me.Rr2 = CSng(0.25 * Me.Rnew)
            If Me.Rr2 < Me.Rmin Then
                For Me.i = 1 To Me.n
                    Me.ip = Me.Ipn(Me.i)
                    Me.P(Me.ip) = Me.P(Me.ip) - Me.Rs * Me.St(Me.i)
                Next
                ' Print "cannot find improved estimates": Return
            End If
            For Me.i = 1 To Me.n
                Me.ip = Me.Ipn(Me.i)
                Me.P(Me.ip) = Me.P(Me.ip) + Me.Rr2 * Me.St(Me.i)
            Next

            ' GoSub 300
            Me.sub300()

            Me.Ss2 = Me.Ss
            Me.Jit = Me.Jit + 1
            If Me.Ss2 < Me.Sbase Then GoTo 795
            Me.Den = 2 * (Me.Grad * Me.Rnew + Me.Sbase - Me.Ss2)
            If Me.Den < 0.000000000000001 Then GoTo 798

            Me.Rnew = Me.Rr2
            Me.Rs = Me.Grad * Me.Rnew * Me.Rnew / Me.Den

            For Me.i = 1 To Me.n
                Me.ip = Me.Ipn(Me.i)
                Me.P(Me.ip) = Me.P(Me.ip) - Me.Rr2 * Me.St(Me.i)
            Next
            GoTo 755

798:        For Me.i = 1 To Me.n
                Me.ip = Me.Ipn(Me.i)
                Me.P(Me.ip) = Me.P(Me.ip) - Me.Rr2 * Me.St(Me.i)
            Next
            '041202VC: we were having trouble at Galveston workshop, where SS would be different on frmSearch
            'and on return to ecosim. Carl's solution: place a runmodelfast call here
            'RunModelFast TotalTime, Ss
            '041202VC: doesn't work though as what is shown on form is lowest ss, while what is kept in memory is
            'the last of the ss from the series of simrun's done for each sbase printed.
            'Debug.Print Ss
795:        ' end of trust region method to find improving parameter step Return
            Return

        End Sub

        Sub SetParsFromP(Par() As Single)
            '        'puts parameter values back into model arrays after altered by estimation
            'On Local Error Resume Next

            'JB how to add penalty to VulMult
            'Do we just need to penalize vulmut 
            'or do we also need to penalize par()

            Try

                Dim i As Integer, j As Integer, epar As Single, ii As Integer, Yspline() As Single, y2() As Single, Xs As Single, Ys As Single
                Dim PBar As Single
                If Me.AnomalySearch = True Then
                    If Me.Numspline < 2 Then
                        ' No Spline points
                        PBar = 0
                        For i = 1 To Me.TotalTime : PBar = PBar + Par(i) : Next
                        PBar = PBar / Me.TotalTime
                        For i = 1 To Me.TotalTime
                            epar = CSng(Math.Exp(Par(i) - PBar))
                            For j = 1 To 12
                                Me.m_esdata.zscale(12 * (i - 1) + j, Me.ForceNo) = epar
                            Next
                        Next
                    Else
                        ' Spline the new parameters into the anomaly shape
                        ReDim Yspline(Me.Numspline), y2(Me.Numspline)
                        PBar = 0
                        For i = 1 To Me.Numspline : PBar = PBar + Par(i) : Next
                        PBar = PBar / Me.Numspline
                        'Dim Scheck As Single
                        For i = 1 To Me.Numspline : Yspline(i) = CSng(Math.Exp(Par(i) - PBar)) : Next
                        'Scheck = Scheck / Numspline: Debug.Print Scheck, Pbar
                        Me.SPLINE(Me.Xspline, Yspline, Me.Numspline, 0.0#, 0.0#, y2)
                        For i = 12 * (Me.PPyear1 - 1) + 1 To 12 * Me.PPyear2
                            Xs = i
                            Me.SPLINT(Me.Xspline, Yspline, y2, Me.Numspline, Xs, Ys)
                            Me.m_esdata.zscale(i, Me.ForceNo) = Ys
                        Next
                        Erase Yspline, y2
                    End If
                End If

                For i = 1 To Me.VBlock.GetUpperBound(0) '15
                    If Me.IsBlockEstimated(i) = True Then
                        If Par(Me.TotalTime + i) < 34.538 Then
                            Me.VBlock(i) = 1 + CSng(Math.Exp(Par(Me.TotalTime + i)))
                        Else
                            Me.VBlock(i) = 1 + CSng(Math.Exp(34.538))
                        End If
                    End If
                Next

                Me.initEcosimForSearchIteration()

                For ii = 1 To Me.m_esdata.Narena
                    'i = ilink(ii): j = jlink(ii)
                    i = Me.m_esdata.Iarena(ii) : j = Me.m_esdata.Jarena(ii)
                    If Me.VblockCode(ii) > 0 Then
                        Me.m_esdata.VulMult(i, j) = Me.VBlock(Me.VblockCode(ii))
                        ' m_esdata.VulMult(i, j) = 2
                        Me.m_ecosim.setvulratecell(i, j, Me.m_esdata.VulMult(i, j))
                        '****REMOVED BY CJW SEPT 2001; UNSAFE HERE******
                        ' MakeAMatrixCell i, j
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("Fit to time series error.", ex)
            End Try

        End Sub


        Sub SPLINE(X() As Single, Y() As Single, n As Integer, yp1 As Single, ypn As Single, y2() As Single)
            Dim U() As Single, i As Integer, Sig As Single, P As Single, Dum1 As Single, Dum2 As Single
            Dim Qn As Single, Un As Single, K As Integer
            ReDim U(n)
            'cubic spline setup function from Press et al 1995
            If yp1 > 9.9E+29 Then
                y2(1) = 0.0!
                U(1) = 0.0!
            Else
                y2(1) = -0.5
                U(1) = (3.0! / (X(2) - X(1))) * ((Y(2) - Y(1)) / (X(2) - X(1)) - yp1)
            End If
            For i = 2 To n - 1
                Sig = (X(i) - X(i - 1)) / (X(i + 1) - X(i - 1))
                P = Sig * y2(i - 1) + 2.0!
                y2(i) = (Sig - 1.0!) / P
                Dum1 = (Y(i + 1) - Y(i)) / (X(i + 1) - X(i))
                Dum2 = (Y(i) - Y(i - 1)) / (X(i) - X(i - 1))
                U(i) = (6.0! * (Dum1 - Dum2) / (X(i + 1) - X(i - 1)) - Sig * U(i - 1)) / P
            Next i
            If ypn > 9.9E+29 Then
                Qn = 0.0!
                Un = 0.0!
            Else
                Qn = 0.5
                Un = (3.0! / (X(n) - X(n - 1))) * (ypn - (Y(n) - Y(n - 1)) / (X(n) - X(n - 1)))
            End If
            y2(n) = (Un - Qn * U(n - 1)) / (Qn * y2(n - 1) + 1.0!)
            For K = n - 1 To 1 Step -1
                y2(K) = y2(K) * y2(K + 1) + U(K)
            Next K
            Erase U
        End Sub
        Sub SPLINT(Xa() As Single, Ya() As Single, Y2A() As Single, n As Integer, X As Single, ByRef Y As Single)
            Dim Klo As Integer, Khi As Integer, K As Integer, H As Single, A As Single
            Dim B As Single
            'cubic spline calculation of spline value Y at point X, using reference arrays and results
            'from Spline (Y2A vector), from Press et al 1995
            Klo = 1
            Khi = n
            While Khi - Klo > 1
                K = CInt((Khi + Klo) / 2)
                If Xa(K) > X Then
                    Khi = K
                Else
                    Klo = K
                End If
            End While
            H = Xa(Khi) - Xa(Klo)
            'If H = 0! Then Print "Bad XA input.": Exit Sub
            A = (Xa(Khi) - X) / H
            B = (X - Xa(Klo)) / H
            Y = A * Ya(Klo) + B * Ya(Khi)
            Y = CSng(Y + ((A ^ 3 - A) * Y2A(Klo) + (B ^ 3 - B) * Y2A(Khi)) * (H ^ 2) / 6.0)
        End Sub

        Private Sub modelCalled(i As Integer, n As Integer)

            Try
                Me.m_runModelHandler(Me.RunState, i, n)

            Catch ex As Exception
                'don't do anything if the interface exploded
                cLog.Write(ex)
            End Try
        End Sub


#End Region

    End Class

End Namespace
