'==============================================================================
'
' $Log: cF2TSModel.vb,v $
' Revision 1.8  2009/05/21 18:53:35  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.7  2009/04/24 16:06:20  joeb
' Added text for proper initialization
'
' Revision 1.6  2009/01/16 18:30:28  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.5  2008/12/02 19:07:43  joeb
' Added flag for computation of EcoSim timestep ouput
'
' Revision 1.4  2008/11/28 16:54:13  joeb
' Cleaned up ToDo's
'
' Revision 1.3  2008/11/25 17:58:11  joeb
' Fixed bug 459 initEcosimForSearchIteration() now does a full init of ecosim
'
' Revision 1.2  2008/11/19 18:03:08  joeb
' Update calls to RunModelValue to use new signature
'
' Revision 1.1  2008/09/26 07:30:25  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On

Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports System.ComponentModel
Imports EwEUtils.Core

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
    Public Delegate Sub RunModelDelegate(ByVal runType As eRunType, ByVal iCurrentIterationStep As Integer, ByVal nTotalInterationSteps As Integer)


    ''' <summary>
    ''' A search of sensitivity run has started
    ''' </summary>
    ''' <param name="runType">Type of run</param>
    ''' <param name="nSteps">Number of steps in this run if known at the start time otherwise zero</param>
    ''' <remarks></remarks>
    Public Delegate Sub RunStartedDelegate(ByVal runType As eRunType, ByVal nSteps As Integer)

    ''' <summary>
    ''' A search run has stopped
    ''' </summary>
    ''' <param name="runType">Type of run</param>
    ''' <remarks></remarks>
    Public Delegate Sub RunStoppedDelegate(ByVal runType As eRunType)

    ''' <summary>
    ''' A message being sent out by the search
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <remarks></remarks>
    Public Delegate Sub RunMessageDelegate(ByVal msg As cMessage)


    Public Class cF2TSModel

#Region "Private data"

#Region "run data"

        'core data
        Private m_core As cCore = Nothing
        Private m_ecosim As cEcoSimModel = Nothing
        Private m_epdata As cEcopathDataStructures
        Private m_esdata As cEcosimDatastructures
        Private m_tsData As cTimeSeriesDataStructures

        'run data
        Private m_results As cF2TSResults
        Private m_runType As eRunType = eRunType.Idle

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

        '  Public Iobs As Long, Erpred() As Single
        Public StopRun As Boolean
        '  Public TraceObs As Long
        '  Public ErTrace() As Single
        'Public YTraceHat() As Single
        Public PPyear1 As Integer, PPyear2 As Integer ', PPVar As Single
        ' Public VulVar As Single, PPVar As Single
        'Yhat() As Single,
        Public IsBlockEstimated() As Boolean
        Public VBlock() As Single, VblockCode() As Integer, CodeIsSet As Boolean
        ' Public WtType() As Single, Wt() As Single
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

        Friend Sub New(ByVal core As cCore, _
                            ByRef EcoSim As EwECore.Ecosim.cEcoSimModel, _
                            ByRef EcoPathData As cEcopathDataStructures, ByVal EcosimData As cEcosimDatastructures)
            Me.m_core = core
            Me.m_ecosim = EcoSim
            Me.m_epdata = EcoPathData
            Me.m_esdata = EcosimData
            m_lastRunSens = eSensType.NotRun
        End Sub

        ''' <summary>
        ''' Init model, optionally for multi-threading
        ''' </summary>
        ''' <param name="runstartedHandler"></param>
        ''' <param name="runstepHandler"></param>
        ''' <param name="runstoppedHandler"></param>
        Public Sub Init( _
                ByVal runstartedHandler As RunStartedDelegate, _
                ByVal runstepHandler As RunStepDelegate, _
                ByVal runstoppedHandler As RunStoppedDelegate, _
                ByVal AddMessageHandler As RunMessageDelegate, _
                ByVal RunModelHandler As RunModelDelegate, _
                ByVal SendMessageHandler As RunMessageDelegate)


            ' Safety check
            Debug.Assert(m_runType = eRunType.Idle)

            Me.m_runstartedHandler = runstartedHandler
            Me.m_runstepHandler = runstepHandler
            Me.m_runstoppedHandler = runstoppedHandler
            Me.m_AddMessageHandler = AddMessageHandler
            Me.m_SendMessageHandler = SendMessageHandler

            m_runModelHandler = RunModelHandler
            m_lastRunSens = eSensType.NotRun
            m_lstSSResults = New List(Of cSensitivityToVulResults)

        End Sub

#End Region

#Region "Public Properties"

        Public Property RunState() As eRunType
            Get
                Return Me.m_runType
            End Get
            Private Set(ByVal runState As eRunType)
                Me.m_runType = runState
            End Set
        End Property

        ''' <summary>
        ''' Results of the run or iteration depending on when it is accessed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Results() As cF2TSResults
            Get
                Return Me.m_results
            End Get
        End Property



#End Region

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

            Dim esData As cEcosimDatastructures = m_core.m_EcoSimData
            Dim ecosim As cEcoSimModel = m_core.m_EcoSim

            ReDim Ssen(esData.inlinks)

            m_lstSSResults.Clear()

            Try

                ' ToDo: add sanity checks; check if threading set up ok, not running, etc
                m_runType = eRunType.SensitivitySS2VByPredPrey

                InitForRun(m_runType)
                m_lastRunSens = eSensType.PredPreyCell
                Dim senResults As cSensitivityToVulResults = DirectCast(m_results, cSensitivityToVulResults)

                Me.m_runstartedHandler(Me.m_runType, esData.Narena)

                ecosim.RunModelValue(esData.NumYears, Nothing, 0)
                SSBase = esData.SS

                senResults.BaseSS = SSBase

                'logic from frmSearch.Command3_Click()
                For ii As Integer = 1 To esData.Narena

                    i = esData.Iarena(ii) : j = esData.Jarena(ii)
                    Vo = esData.VulMult(i, j)

                    esData.VulMult(i, j) = esData.VulMult(i, j) * VUL_MULT
                    ecosim.RunModelValue(esData.NumYears, Nothing, 0)

                    sss = Math.Abs(esData.SS - SSBase)
                    Ssen(ii) = sss

                    If sss > Smax Then Smax = sss

                    'set vulnerability back to its original value
                    esData.VulMult(i, j) = Vo

                    'set values for interface
                    senResults.iPred = j
                    senResults.iPrey = i
                    senResults.SSen = sss
                    senResults.SSMax = Smax

                    m_lstSSResults.Add(New cSensitivityToVulResults(eRunType.SensitivitySS2VByPredPrey, j, i, sss, Smax))

                    Me.m_runstepHandler()

                    If Me.StopRun Then Exit For

                Next

            Catch ex As Threading.ThreadAbortException

                AddMessage(New cMessage("Fit to Time Series aborted.", _
                                     eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical))

            Catch ex As Exception

                AddMessage(New cMessage("Fit to Time Series Error: " & ex.Message, _
                    eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical))

            End Try

            Me.m_runstoppedHandler(Me.m_runType)
            m_runType = eRunType.Idle


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

            If m_core.m_TSData.NdatType = 0 Then
                Exit Sub
            End If

            Dim epData As cEcopathDataStructures = m_core.m_EcoPathData
            Dim esData As cEcosimDatastructures = m_core.m_EcoSimData
            Dim ecosim As cEcoSimModel = m_core.m_EcoSim

            Dim nGroups As Integer = m_core.nGroups
            Dim nLiving As Integer = m_core.nLivingGroups

            ReDim PSen(nLiving)


            Try

                'init 
                m_runType = eRunType.SensitivitySS2VByPredator
                InitForRun(m_runType)
                m_lastRunSens = eSensType.PredColumn

                'cast the results into the correct type of object
                Dim senResults As cSensitivityToVulResults = DirectCast(m_results, cSensitivityToVulResults)

                'tell the interface the run is starting
                Me.m_runstartedHandler(Me.m_runType, nLiving)

                initEcosimForSearchIteration()

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
                        PSen(ipred) = sss

                        m_lstSSResults.Add(New cSensitivityToVulResults(eRunType.SensitivitySS2VByPredator, ipred, 0, sss, Smax))

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
                AddMessage(New cMessage("Fit to Time Series Error: " & ex.Message, _
                                    eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical))

            End Try

            Me.m_runstoppedHandler(Me.m_runType)
            m_runType = eRunType.Idle

        End Sub


        Public Sub setNBlocksFromSensitivity(ByVal nBlocks As Integer)
            Dim n As Integer
            Dim icell As Integer, ipred As Integer, iprey As Integer
            Dim ssObj As cSensitivityToVulResults

            Debug.Assert(Me.m_lastRunSens <> eSensType.NotRun, "Sensitivity routine has not been run. The blocks can not be set.")

            If Me.m_lastRunSens = eSensType.NotRun Then

                AddMessage(New cMessage("Sensitivity routine has not been run. The blocks can not be set.", _
                            eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Warning))

                Exit Sub
            End If

            Try

                'sort the sensitivities biggest to smallest
                'see cSensitivityToVulResults.CompareTo()
                m_lstSSResults.Sort()

                'now update the VblockCode() with the sorted sensitivities
                Select Case Me.m_lastRunSens

                    Case eSensType.PredColumn

                        'nBlocks is the user set number of blocks
                        'm_lstSSResults.Count is the actual number of pred/columns found by the sensitivity search
                        n = CInt(IIf(m_lstSSResults.Count > nBlocks, nBlocks, m_lstSSResults.Count))

                        icell = 0
                        For Each ssObj In m_lstSSResults
                            icell = icell + 1
                            If icell > n Then Exit For

                            'convert the pred / prey indexes to an nLinks index
                            For ii As Integer = 1 To m_esdata.inlinks
                                'all the prey of this predator
                                ipred = m_esdata.jlink(ii)
                                If ssObj.iPred = ipred Then
                                    VblockCode(ii) = icell
                                End If
                            Next ii

                        Next ssObj

                    Case eSensType.PredPreyCell

                        n = CInt(IIf(m_lstSSResults.Count > nBlocks, nBlocks, m_lstSSResults.Count))
                        icell = 0
                        For Each ssObj In m_lstSSResults
                            icell = icell + 1
                            If icell > n Then Exit For

                            'convert the pred / prey indexes to an nLinks index
                            For ii As Integer = 1 To m_esdata.inlinks
                                iprey = m_esdata.ilink(ii) : ipred = m_esdata.jlink(ii)
                                If ssObj.iPred = ipred And ssObj.iPrey = iprey Then
                                    VblockCode(ii) = icell
                                End If
                            Next ii

                        Next ssObj

                End Select

            Catch ex As Exception
                cLog.Write(ex)

            End Try


        End Sub


        '  private 

#End Region ' SensitivitySS2VByPredatory

#Region " Public Search functions"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Sub RunSearch()

            Dim results As cF2TSResults = Nothing
            Dim nSteps As Integer = 1 + 9

            Try

                If m_core.m_TSData.NdatType = 0 Then
                    'no time series data loaded
                    Exit Sub
                End If

                '.. add init model logic here
                Dim failed As Integer
                InitForRun(eRunType.Search)

                ' Start run
                m_runType = eRunType.Search
                Me.m_runstartedHandler(Me.m_runType, nSteps)

                DoEstimation(failed)

            Catch ex As Threading.ThreadAbortException
                ' Done

            Catch ex As Exception
                AddMessage(New cMessage("Fit to Time Series Error: " & ex.Message, _
                      eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical))

            End Try

            Me.m_runstoppedHandler(Me.m_runType)
            m_runType = eRunType.Idle


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

                DirectCast(m_results, cSearchResults).IterSS = m_esdata.SS
                m_results.iStep = m_estIter

                If m_runstepHandler IsNot Nothing Then
                    m_runstepHandler()
                End If
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".searchIterationStep() Error: " & ex.Message)
            End Try

        End Sub

        Private Sub sendMessage(ByVal msg As cMessage)
            Try
                m_SendMessageHandler(msg)
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Sub

        Private Sub AddMessage(ByVal msg As cMessage)
            Try
                m_AddMessageHandler(msg)
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Sub

#End Region ' Notifications

#Region "Model Logic"

        Private Sub InitForRun(ByVal runType As eRunType)

            Try

                'get the core data for this run
                m_esdata = m_core.m_EcoSimData
                m_tsData = m_core.m_TSData
                m_data = m_core.m_FitToTimeSeriesData

                m_lastRunSens = eSensType.NotRun

                Debug.Assert(m_tsData.Iobs <> 0, "No time series data has been loaded, cannot do nonlinear estimation.")

                'for convenience set local variables to values set by interface
                'these variables can not be changed by the interface during a run so this should be Ok
                AnomalySearch = m_data.bAnomalySearch
                Numspline = m_data.nNumSplinePoints
                PPyear1 = m_data.FirstYear
                PPyear2 = m_data.LastYear
                ForceNo = m_data.iCatchAnomalySearchShapeNumber

                If m_data.bVulnerabilitySearch Then
                    pvVul = m_data.VulnerabilityVariance
                Else
                    'this will turn off the Vulnerability search by setting pv(maxpars) to zero for all vulnerability parameter
                    'this variable can not be varied so it is not searched see DoEstimation
                    pvVul = 0
                End If

                'make sure the fit to timeseries search is turned on
                StopRun = False

                'Init Ecosim

                'make sure the fishing policy search is turned off
                m_core.m_SearchData.SearchMode = eSearchModes.FitToTimeSeries
                'No timestep ouput
                m_core.m_EcoSimData.bTimestepOutput = False

                'make sure ecosim does not call the interface 
                'setting bTimestepOutput = False should have had the same effect
                m_core.m_EcoSim.TimeStepDelegate = Nothing

                ''init ecosim
                'm_esdata.dimResults()

                'Now Init Ecosim
                initEcosimForSearchIteration()

                TotalTime = m_esdata.NumYears

                If nBlockCodes = 0 Then nBlockCodes = m_esdata.inlinks
                ReDim VBlock(nBlockCodes)
                ReDim IsBlockEstimated(nBlockCodes)

                'VblockCode() should have been set by an interface
                'however if this is run from a plugin then it is possible for VblockCode() to be null
                If VblockCode Is Nothing Then
                    ReDim VblockCode(m_esdata.inlinks)
                End If

                'create the results object for this type of run
                m_results = cF2TSResultsFactory.Create(runType)

                'VBlock() and VblockCode(inLinks) should have been set by the interface
                SetVblock(m_esdata)

                'get Base SS from ecosim 
                m_ecosim.RunModelValue(TotalTime, Nothing, 0)

                'set the baseSS in the results object that was calculated above by ecosim
                DirectCast(m_results, cF2TSResults).BaseSS = m_esdata.SS

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("Initialization error: " & ex.Message, ex)
            End Try

        End Sub


        ''' <summary>
        ''' Initialize Ecosim before each search iteration
        ''' </summary>
        ''' <remarks>In EwE5 this is called PrepareSimSpace()</remarks>
        Private Sub initEcosimForSearchIteration()

            m_ecosim.Init(True)

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

                MaxObs = m_tsData.Iobs

                MaxPars = m_esdata.NumYears + UBound(VBlock)    '15
                If UBound(VBlock) + PPyear2 - PPyear1 > MaxPars Then
                    MaxPars = UBound(VBlock) + PPyear2 - PPyear1
                End If
                ReDim Se(MaxPars, MaxObs), Sold(MaxPars), Xy(MaxPars)
                ReDim Ybase(MaxObs), St(MaxPars) ', Wt(MaxObs)
                ReDim Ipn(MaxPars), amat(MaxPars, MaxPars)
                ReDim Vi(MaxPars, MaxPars), Cl(MaxPars, MaxPars)
                ReDim cy(MaxPars)
                ReDim Penter(MaxPars), Po(MaxPars), pv(MaxPars), P(MaxPars), paramname(MaxPars)

                Nobs = m_tsData.Iobs
                SetPfromPars(Po)

                'set the parameter variance 
                If AnomalySearch Then
                    If Numspline < 2 Then
                        For i = PPyear1 To PPyear2
                            pv(i) = m_data.PPVariance
                        Next
                    Else
                        For i = 1 To Numspline
                            pv(i) = m_data.PPVariance
                        Next
                    End If
                End If

                'if vulnerability variance = 0 then these parameters will not be counted in 'n' see below
                'this means the vulnerability parameters will not be included in the search
                'InitForRun() decides if pvVul is set or not based on the bVulnerabilitySearch flag
                For i = 1 To UBound(IsBlockEstimated)   '15
                    If IsBlockEstimated(i) Then
                        pv(TotalTime + i) = pvVul 'pvVul was set in IntForRun
                    Else
                        pv(TotalTime + i) = 0
                    End If
                Next

                'define which parameters are to be varied, count these up (in N), and store their indices in the vector IP
                'jb
                'n defines the number of parameters to loop over in sub290 (number of parameters to search for, number of calls to ecosim)
                'ipn() points to the index in P() to get the parameter from e.g. parameter = P(Ipn(i))
                'n is counted from pv(MaxPars) (parameter variance)
                'This decides what parameters are searched if pv(iparameter) is zero the parameter is not used
                ip = 0
                n = MaxPars
                For i = 1 To MaxPars
                    If pv(i) > 0 Then
                        ip = ip + 1
                        Ipn(ip) = i
                    Else
                        n = n - 1
                    End If
                Next

                If n = 0 Then
                    'message

                    AddMessage(New cMessage("Nothing to estimate, sketch interactions, exiting search", eMessageType.ErrorEncountered, _
                                                eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Warning))
                    '   MsgBox("Nothing to estimate, sketch interactions, exiting search")
                    Exit Sub
                End If

                'REM set some initial conditions for iteration counters
                rmax = 1
                Jit = 0
                ic = 0
                SO = 1.0E+30
                Rmin = 0.1
                dinc = 0.0001
                m_estIter = 0
                EvalCount = 0
                For i = 1 To MaxObs
                    m_tsData.Wt(i) = 1
                Next

                For i = 1 To MaxPars
                    P(i) = Po(i)
                Next

                DF = Nobs - n
                If DF < 1 Then DF = 1

                '190:            ' Print "ITERATION BEGINS; HIT ANY KEY ONCE IF NECESSARY TO INTERRUPT"
                '200 GoSub 290: GoSub 550: Rem compute sensitivities and newton correction step for this parameter combination
200:
                sub290()
                sub550()

                m_estIter = m_estIter + 1
                searchIterationStep()

                If StopRun = True Then Exit Sub
                If m_estIter > 500 Then GoTo 250

                If StopIndex > 0 Then
                    fbmsg = New cFeedbackMessage("More Iterations (y/n)?", eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Information, cFeedbackMessage.eReplyStyle.YES_NO)
                    sendMessage(fbmsg)
                    If fbmsg.Reply = cFeedbackMessage.eReply.NO Then GoTo 250
                    '  If MsgBox("MORE ITERATIONS (y/n)?", MsgBoxStyle.YesNo) = vbNo Then GoTo 250
                End If

                For i = 1 To n
                    If Math.Abs(St(i) / (P(Ipn(i)) + dinc)) > 0.001 Then GoTo 220 REM seek correction step if newton step is still large
                Next

                GoTo 250
                '   220 GoSub 700: Rem find and apply corrected step if possible

                'VC Sep 08, had a case where the grad check in sub700 would estimate Grad to be very small, then kick out 
                'of sub700, but next check above of 
                'If Math.Abs(St(i) / (P(Ipn(i)) + dinc)) > 0.001
                'would cause it to go back to 220 and back to sub700, etc.
                'Discussed this with Carl
220:            If m_estIter = 1 Or Math.Abs(Grad) > 0.00000000001 Then
                    sub700()
                Else 'no difference anymore so continue with 
                    GoTo 250
                End If

240:
                If Rr2 >= Rmin Then
                    sub900()
                    GoTo 200 REM start another nonlinear iteration if key has not been hit or convergence found
                End If

250:
                sub300()
                sub900()
                MatInv(n, amat, det)

                fbmsg = New cFeedbackMessage("Estimates converged. Do you want to do more iterations (y/n)?", eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Information, cFeedbackMessage.eReplyStyle.YES_NO)
                sendMessage(fbmsg)
                If fbmsg.Reply = cFeedbackMessage.eReply.YES Then GoTo 220

                '   If MsgBox("ESTIMATES CONVERGED; MORE ITERATIONS (y/n)?", MsgBoxStyle.YesNo) = vbYes Then GoTo 220

                'MsgBox "Estimates apparently converged"
                '  frmSearch.Res.Visible = False
                StopIndex = 0

                Exit Sub

            Catch ex As Threading.ThreadAbortException
                'we do not know why this happen 
                'the most likey case is the form has been closed and that aborted the thread
                'anyway clean up
                Failed = 1
                SetParsFromP(Po)

                Me.m_runstoppedHandler(Me.m_runType)
                m_runType = eRunType.Idle

            Catch ex As Exception

                cLog.Write(ex)

                Failed = 1
                SetParsFromP(Po)
                Debug.Assert(False, ex.Message)

                Me.m_runstoppedHandler(Me.m_runType)
                m_runType = eRunType.Idle

                AddMessage(New cMessage("Estimation Error; original parameter values restored " & ex.Message, _
                    eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Warning))
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

            For i = 1 To UBound(IsBlockEstimated)
                IsBlockEstimated(i) = False
            Next

            For ii = 1 To esData.inlinks
                i = esData.ilink(ii)
                j = esData.jlink(ii)
                If VblockCode(ii) > 0 Then
                    VBlock(VblockCode(ii)) = esData.VulMult(i, j)
                    IsBlockEstimated(VblockCode(ii)) = True
                End If
            Next

        End Sub

        Private Sub SetPfromPars(ByVal Par() As Single)
            'sets Par array to parameters from the model
            Dim i As Integer, i1 As Integer, i2 As Integer, im As Integer, Pvl As Single
            If AnomalySearch = True Then
                If Numspline < 2 Then
                    For i = 1 To m_esdata.NumYears
                        Par(i) = CSng(Math.Log(m_esdata.zscale(1 + 12 * (i - 1), ForceNo) + 1.0E-20))   'Added 1E-20 per cjw email to vc 26sep00
                    Next
                Else
                    ReDim Xspline(Numspline)
                    i1 = 12 * (PPyear1 - 1) + 1
                    i2 = 12 * PPyear2
                    For i = 1 To Numspline
                        im = CInt(i1 + (i2 - i1) * (i - 1) / (Numspline - 1))
                        Xspline(i) = im
                        Par(i) = CSng(Math.Log(m_esdata.zscale(im, ForceNo) + 1.0E-20))
                    Next
                End If
            End If
            'Par(TotalTime + 1) = VulMultAll
            For i = 1 To UBound(VBlock) '15
                If IsBlockEstimated(i) = True And VBlock(i) > 0 Then
                    Pvl = CSng(VBlock(i) - 1.0)
                    If Pvl < 0.000001 Then Pvl = 0.000001
                    Par(m_esdata.NumYears + i) = CSng(Math.Log(Pvl))
                End If
            Next
        End Sub

        Sub MatInv(ByVal n As Integer, ByVal amat(,) As Single, ByVal det As Single)
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
            If StopRun = True Then Exit Sub

            Dim sumDYDX As Single, sumBase As Single ', sumY As Single
            '     GoSub 300: 

            sub300()

            Sbase = Ss
            For j = 1 To Nobs
                Ybase(j) = m_tsData.Yhat(j)
            Next
            Va = Ss / DF
            var = Va
            Vmax = CSng(Math.Exp(-0.5 * Ss / Va))
            Vc = CSng(0.05 * Vmax)
            Vp = 0 'If Np > 0 Then Vp = Sp / Np
            For kkkk = 1 To n
                ip = Ipn(kkkk)
                Dp = dinc * P(ip)
                If Dp = 0 Then Dp = dinc
                P(ip) = P(ip) + Dp

                'call the model
                sub300()

                'tell the interface that Ecosim has been called
                Me.modelCalled(kkkk, n)

                For j = 1 To Nobs
                    Se(kkkk, j) = (m_tsData.Yhat(j) - Ybase(j)) / Dp
                    sumDYDX = sumDYDX + Math.Abs(Se(kkkk, j))
                    'sumY = sumY + (m_tsData.Yhat(j) - Ybase(j))
                    sumBase = sumBase + (Ybase(j))
                Next

                'System.Console.WriteLine("Var = " & kkkk.ToString & ", Sum SS = " & sumDYDX.ToString & ", Sum Y = " & sumY.ToString & ", sum base = " & sumBase.ToString)

                P(ip) = P(ip) - Dp
                '    DoEvents()
                If StopRun = True Then Exit Sub
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
            SetParsFromP(P)
            m_ecosim.RunModelValue(TotalTime, Nothing, 0)

            Ss = m_esdata.SS

            'For Iobs = 1 To Nobs
            '    Ss = Ss + Erpred(Iobs) * Erpred(Iobs) * Wt(Iobs)
            'Next
            ' Debug.Print SS

            ' good form is  SS = SS + ER(i) * ER(i) * WT(i)  where ER(i)=yobs(i)-yhat(i)
            Return REM end of routine for calculating ss and yhat values
        End Sub

        Private Sub sub550()
            '550:    REM routine to solves (X'X)(st)=X'(er) by Cholesky decompostion, using X=SE sensitivity matrix augmented by prior variances and er=fitting error vector; output is newton parameter correction step vector st(1...n)
            For i = 1 To n
                Sold(i) = St(i)
                ip = Ipn(i)
                Xy(i) = (P(ip) - Po(ip)) * Va / pv(ip)
            Next

            For i = 1 To n

                Su = 0
                For K = 1 To Nobs
                    Su = Su + m_tsData.Wt(K) * Se(i, K) * m_tsData.Erpred(K)
                Next K

                Xy(i) = Su + Xy(i)
                For j = 1 To i
                    Su = 0 : For K = 1 To Nobs
                        Su = Su + m_tsData.Wt(K) * Se(i, K) * Se(j, K)
                    Next K
                    amat(i, j) = Su
                    amat(j, i) = Su
                Next j
            Next i

            For i = 1 To n
                amat(i, i) = amat(i, i) + Va / pv(Ipn(i))
            Next i

            For i = 1 To n
                For j = 1 To n
                    Vi(i, j) = amat(i, j)
                Next j
            Next i

            Cl(1, 1) = CSng(Math.Sqrt(amat(1, 1)))
            For i = 2 To n
                Cl(i, 1) = amat(i, 1) / Cl(1, 1)
            Next

            For i = 2 To n
                If i = 2 Then GoTo 641

                For j = 2 To i - 1
                    Ct = 0
                    For K = 1 To j - 1
                        Ct = Ct + Cl(i, K) * Cl(j, K)
                    Next
                    Cl(i, j) = (amat(i, j) - Ct) / Cl(j, j)
                Next
641:            Ct = 0
                For K = 1 To i - 1
                    Ct = CSng(Ct + Cl(i, K) ^ 2)
                Next
                Cl(i, i) = CSng(Math.Sqrt(amat(i, i) - Ct))
            Next

            cy(1) = Xy(1) / Cl(1, 1)

            For i = 2 To n
                Ct = 0
                For j = 1 To i - 1
                    Ct = Ct + Cl(i, j) * cy(j)
                Next
                cy(i) = (Xy(i) - Ct) / Cl(i, i)
            Next

            St(n) = cy(n) / Cl(n, n)
            If n = 1 Then GoTo 650
            For i = n - 1 To 1 Step -1
                Ct = 0
                For j = n To i + 1 Step -1
                    Ct = Ct + St(j) * Cl(j, i)
                Next
                St(i) = (cy(i) - Ct) / Cl(i, i)
            Next
650:        Return

        End Sub


        Private Sub sub900()
            '900:    REM save parameter estimates
            SetParsFromP(P)
            'frmSearch.Res.Print iter; ":"; Ss

            'RegVar = SS / Df
            Return

        End Sub

        Private Sub sub700()
700:        REM routine to find an acceptable step length (fraction of st) if possible, and applies it to the parameter vector P (this algorithm from p. in Bard, 1974)
            If StopRun = True Then Exit Sub
            Rr2 = 1
            Grad = 0

            For i = 1 To n
                Grad = Grad - St(i) * Xy(i)
            Next
            Rs = CSng(rmax / 2 ^ Jit)

            If Math.Abs(Grad) < 0.00000000001 Then Return

            For i = 1 To n
                ip = Ipn(i)
                P(ip) = P(ip) + Rs * St(i)
            Next
            If StopRun = True Then Exit Sub
            ' GoSub 300: 
            sub300()
            Stry = Ss
            Rbet = Grad * Rs * Rs / (2 * (Grad * Rs + Sbase - Stry))
            If Stry >= Sbase Then GoTo 750

            Jit = CInt(Jit / 2)
            If Rbet < 0 Then Rbet = 2 * Rs
            Rnew = Rbet
            If Rnew > rmax Then Rnew = rmax

            If StopRun = True Then Exit Sub

            Rdel = Rnew - Rs
            For i = 1 To n
                ip = Ipn(i) : P(ip) = P(ip) + Rdel * St(i)
            Next

            'GoSub 300
            sub300()

            Snew = Ss
            If Snew < Stry Then GoTo 795
            For i = 1 To n
                ip = Ipn(i)
                P(ip) = P(ip) - Rdel * St(i)
            Next

            GoTo 795

750:        Rnew = Rs
            For i = 1 To n
                ip = Ipn(i)
                P(ip) = P(ip) - Rs * St(i)
            Next

755:        Rr2 = CSng(0.75 * Rnew)
            If Rr2 > Rbet Then Rr2 = Rbet

            If StopIndex = 1 Then Return
            If StopRun = True Then Exit Sub
            If Rr2 < 0.25 * Rnew Then Rr2 = CSng(0.25 * Rnew)
            If Rr2 < Rmin Then
                For i = 1 To n
                    ip = Ipn(i)
                    P(ip) = P(ip) - Rs * St(i)
                Next
                ' Print "cannot find improved estimates": Return
            End If
            For i = 1 To n
                ip = Ipn(i)
                P(ip) = P(ip) + Rr2 * St(i)
            Next

            ' GoSub 300
            sub300()

            Ss2 = Ss
            Jit = Jit + 1
            If Ss2 < Sbase Then GoTo 795
            Den = 2 * (Grad * Rnew + Sbase - Ss2)
            If Den < 0.000000000000001 Then GoTo 798

            Rnew = Rr2
            Rs = Grad * Rnew * Rnew / Den

            For i = 1 To n
                ip = Ipn(i)
                P(ip) = P(ip) - Rr2 * St(i)
            Next
            GoTo 755

798:        For i = 1 To n
                ip = Ipn(i)
                P(ip) = P(ip) - Rr2 * St(i)
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

        Sub SetParsFromP(ByVal Par() As Single)
            '        'puts parameter values back into model arrays after altered by estimation
            'On Local Error Resume Next

            Try

                Dim i As Integer, j As Integer, epar As Single, ii As Integer, Yspline() As Single, y2() As Single, Xs As Single, Ys As Single
                Dim PBar As Single
                If AnomalySearch = True Then
                    If Numspline < 2 Then
                        PBar = 0
                        For i = 1 To TotalTime : PBar = PBar + Par(i) : Next
                        PBar = PBar / TotalTime
                        For i = 1 To TotalTime
                            epar = CSng(Math.Exp(Par(i) - PBar))
                            For j = 1 To 12
                                m_esdata.zscale(12 * (i - 1) + j, ForceNo) = epar
                            Next
                        Next
                    Else
                        ReDim Yspline(Numspline), y2(Numspline)
                        PBar = 0
                        For i = 1 To Numspline : PBar = PBar + Par(i) : Next
                        PBar = PBar / Numspline
                        'Dim Scheck As Single
                        For i = 1 To Numspline : Yspline(i) = CSng(Math.Exp(Par(i) - PBar)) : Next
                        'Scheck = Scheck / Numspline: Debug.Print Scheck, Pbar
                        SPLINE(Xspline, Yspline, Numspline, 0.0#, 0.0#, y2)
                        For i = 12 * (PPyear1 - 1) + 1 To 12 * PPyear2
                            Xs = i
                            SPLINT(Xspline, Yspline, y2, Numspline, Xs, Ys)
                            m_esdata.zscale(i, ForceNo) = Ys
                        Next
                        Erase Yspline, y2
                    End If
                End If

                For i = 1 To UBound(VBlock) '15
                    If IsBlockEstimated(i) = True Then
                        If Par(TotalTime + i) < 34.538 Then
                            VBlock(i) = 1 + CSng(Math.Exp(Par(TotalTime + i)))
                        Else
                            VBlock(i) = 1 + CSng(Math.Exp(34.538))
                        End If
                    End If
                Next

                initEcosimForSearchIteration()

                For ii = 1 To m_esdata.Narena
                    'i = ilink(ii): j = jlink(ii)
                    i = m_esdata.Iarena(ii) : j = m_esdata.Jarena(ii)
                    If VblockCode(ii) > 0 Then
                        m_esdata.VulMult(i, j) = VBlock(VblockCode(ii))
                        ' m_esdata.VulMult(i, j) = 2
                        m_ecosim.setvulratecell(i, j, m_esdata.VulMult(i, j))
                        '****REMOVED BY CJW SEPT 2001; UNSAFE HERE******
                        ' MakeAMatrixCell i, j
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("Fit to time series error.", ex)
            End Try

        End Sub


        Sub SPLINE(ByVal X() As Single, ByVal Y() As Single, ByVal n As Integer, ByVal yp1 As Single, ByVal ypn As Single, ByVal y2() As Single)
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
        Sub SPLINT(ByVal Xa() As Single, ByVal Ya() As Single, ByVal Y2A() As Single, ByVal n As Integer, ByVal X As Single, ByRef Y As Single)
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

        Private Sub modelCalled(ByVal i As Integer, ByVal n As Integer)

            Try
                Me.m_runModelHandler(m_runType, i, n)

            Catch ex As Exception
                'don't do anything if the interface exploded
                cLog.Write(ex)
            End Try
        End Sub


#End Region

    End Class

End Namespace
