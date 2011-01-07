Option Strict On
Imports EwECore.Ecopath
Imports EwECore.EcoSim
Imports System.Threading
Imports EwEUtils.Core

'vc sep 2008, using streamwriter from system.io:
Imports System.IO


Public Enum eMCParams
    Biomass = 1
    PB = 2
    QB = 3
    EE = 4
    BA = 5
    Vulnerability = 6  '(one per consumer) same for all prey
End Enum

''' <summary>
''' Call each time a monte carlo trial has been completed
''' </summary>
''' <remarks></remarks>
Public Delegate Sub MonteCarloTrialProgressDelegate()

''' <summary>
''' Call each time a Ecopath model has been run
''' </summary>
''' <remarks></remarks>
Public Delegate Sub MonteCarloEcopathProgressDelegate()

''' <summary>
''' Call at the completion of the monte carlo trials
''' </summary>
''' <remarks>There can be multiple Ecopath model runs for each monte carlo trial</remarks>
Public Delegate Sub MonteCarloCompletedDelegate()

Public Delegate Sub MonteCarloSendMessageDelegate(ByRef Message As cMessage)



''' <summary>
''' This class wraps the Ecosim monte carlo routines
''' </summary>
''' <remarks></remarks>
Public Class cEcosimMonteCarlo

    Public CVpar(,) As Single
    Public ParLimit(,,) As Single

    Public Ntrials As Integer
    Public StopTrial As Boolean
    Public bShowPlot As Boolean
    Public bRetainBiomass As Boolean
    Public EcosimTimeStep As EcoSimTimeStepDelegate

    Public dlgEcopathIterationHandler As MonteCarloEcopathProgressDelegate
    Public dlgTrialStepHandler As MonteCarloTrialProgressDelegate
    Public dlgMonteCarloCompletedHandler As MonteCarloCompletedDelegate
    Public dlgMonteCarloMessageHandler As MonteCarloSendMessageDelegate
    '  publc 

    Public nEcopathIterations As Integer
    Public nTrialIterations As Integer

    ''' <summary>
    ''' Best fitting Sum of Squares computed by Ecosim
    ''' </summary>
    ''' <remarks></remarks>
    Public SSBestFit As Single

    Public SSCurrent As Single

    Public SSorg As Single

    Private m_core As cCore
    Private m_ecopath As cEcoPathModel
    Private m_ecosim As cEcoSimModel
    Private m_epdata As cEcopathDataStructures
    Private m_esdata As cEcosimDatastructures
    Private m_stanza As cStanzaDatastructures 'needs to come in from the core
    Private m_tracerData As cContaminantTracerDataStructures


    Private AbortRun As Boolean

    ''' <summary>
    ''' Ecopath parameters by Parameter, nGroups 
    ''' </summary>
    Public Pmean(,) As Single

    Private isCrashed() As Boolean
    Public isExploded() As Boolean
    Private iTrial As Integer

    ''' <summary>
    ''' Best fitting parameter to the last run Monte Carlo trials
    ''' </summary>
    Public BestFit(,) As Single
    Dim RunsSinceLastWithLowerSS As Integer = 0

    ''' <summary>
    ''' Original Ecopath parameters before trials were run
    ''' </summary>
    Dim startValues(,) As Single 'copy of the original values used to restore to original state

    Dim orgVul(,) As Single


    Public Sub New(ByRef theCore As cCore)

        m_core = theCore

        m_ecopath = m_core.m_EcoPath
        m_ecosim = m_core.m_EcoSim
        m_epdata = m_core.m_EcoPathData
        m_esdata = m_core.m_EcoSimData
        'data from Ecosim
        m_stanza = m_ecosim.m_stanza
        m_tracerData = m_ecosim.TracerData

        Ntrials = 20 'default number of trials

    End Sub


    Public Function Init() As Boolean

        Try
            redimVariables()

            'vc sep 2008: adding vulnerability to MC: changed first dimension from 5 to 6
            ReDim Pmean(6, m_core.nGroups)
            ReDim startValues(6, m_core.nGroups)
            ReDim BestFit(6, m_core.nGroups)
            ReDim orgVul(m_core.nGroups, m_core.nGroups)

            For igrp As Integer = 1 To m_core.nGroups
                Pmean(eMCParams.Biomass, igrp) = m_epdata.B(igrp)
                Pmean(eMCParams.PB, igrp) = m_epdata.PB(igrp)
                Pmean(eMCParams.EE, igrp) = m_epdata.EE(igrp)
                Pmean(eMCParams.BA, igrp) = m_epdata.BA(igrp)
                'vc sep 2008: adding vulnerability to MC
                Pmean(eMCParams.Vulnerability, igrp) = m_esdata.VulnerabilityPredator(igrp)
            Next
            CalculateUpperLowerLimits(False)

            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".Run", ex)
        End Try


    End Function


    Public Sub Clear()

        Me.Pmean = Nothing '(6, m_core.nGroups)
        Me.startValues = Nothing '(6, m_core.nGroups)
        Me.BestFit = Nothing '(6, m_core.nGroups)
        Me.orgVul = Nothing '(m_core.nGroups, m_core.nGroups)
        Me.ParLimit = Nothing '(1, 6, m_core.nGroups)
        Me.CVpar = Nothing '(6, m_core.nGroups)

    End Sub

    Private Function PedigreeVarToMCIndex(ByVal vn As eVarNameFlags) As Integer

        Select Case vn
            Case eVarNameFlags.Biomass : Return 1
            Case eVarNameFlags.PBInput : Return 2
            Case eVarNameFlags.QBInput : Return 3
            Case eVarNameFlags.DietComp : Return 4
            Case eVarNameFlags.TCatchInput : Return 5
        End Select
        Return -1

    End Function

    Friend Sub LoadFromPedigree()

        Dim opt As Integer ' Opt = pedigree level
        Dim varname As eVarNameFlags = eVarNameFlags.NotSet
        Dim man As cPedigreeManager = Nothing
        Dim Par As Integer = 0

        ' For all pedigree supported vars
        For iVar As Integer = 1 To Me.m_epdata.NumPedigreeVariables
            ' Get variable
            varname = Me.m_core.PedigreeVariable(iVar)
            ' For all groups
            For i As Integer = 1 To Me.m_epdata.NumGroups
                ' Read assigned pedigree level for a group (was 'Opt = ReadPedigreeFromDatabase(Par)')
                opt = Me.m_epdata.Pedigree(i, iVar)
                If opt > 0 Then ' Non-estimated level
                    Try

                        Par = Me.PedigreeVarToMCIndex(varname)
                        'ToDo 7-Dec-2010 MonteCarlo.LoadFromPedigree see ticket #855
                        'TCatchInput pedigree level is being applied to BA
                        'DietComp and TCatchInput should be removed
                        'BA should be added???
                        Select Case varname
                            Case eVarNameFlags.Biomass, _
                                 eVarNameFlags.PBInput, _
                                 eVarNameFlags.QBInput

                                CVpar(Par, i) = Me.m_epdata.PedigreeLevelConfidence(opt) / 100.0! / 2.0!

                                'ToDo 7-Dec-2010 MonteCarlo.LoadFromPedigree the intention here is to reset the ParLimit() base on the new CV value
                                ParLimit(0, Par, i) = 0
                                ParLimit(1, Par, i) = 0

                            Case eVarNameFlags.DietComp  'DC
                                'If PP(GrpNo) < 1 Then
                                '    For Grp2 = 0 To NumGroups
                                '        DCPct(GrpNo, Grp2) = value
                                '    Next
                                'End If

                            Case eVarNameFlags.TCatchInput
                                Debug.Assert(False, Me.ToString & ".LoadFromPedigree() Pedigree for Catch is being applied to BA()")
                                '040114VC: Now want to use BA in Ecosim Monte Carlo, so trying this
                                CVpar(Par, i) = Me.m_epdata.PedigreeLevelConfidence(opt) / 100.0! / 2.0!

                        End Select
                    Catch ex As Exception

                    End Try
                End If
            Next
        Next

    End Sub

    Friend Sub initForRun()

        Try

            StopTrial = False

            ReDim isCrashed(m_core.nGroups)
            ReDim isExploded(m_core.nGroups)

            m_ecosim.Init(True)

            m_core.m_EcoSimData.bTimestepOutput = True
            m_ecosim.TimeStepDelegate = Nothing
            'run ecosim to get the fit (SS) of the ref data to the current ecopath parameters

            For iPred As Integer = 1 To m_core.nGroups
                Dim vul As Single = 0
                For iPrey As Integer = 1 To m_core.nGroups
                    If m_core.m_EcoSimData.VulMult(iPrey, iPred) > 0 Then vul = m_core.m_EcoSimData.VulMult(iPrey, iPred) : Exit For
                Next
                m_core.m_EcoSimData.VulnerabilityPredator(iPred) = vul
            Next

            m_ecosim.Run()

            For iGrp As Integer = 1 To m_core.nGroups
                Pmean(eMCParams.Biomass, iGrp) = m_epdata.B(iGrp)
                Pmean(eMCParams.PB, iGrp) = m_epdata.PB(iGrp)
                Pmean(eMCParams.EE, iGrp) = m_epdata.EE(iGrp)
                Pmean(eMCParams.BA, iGrp) = m_epdata.BA(iGrp)
                'vc sep 2008: adding vulnerability to MC
                Pmean(eMCParams.Vulnerability, iGrp) = m_esdata.VulnerabilityPredator(iGrp)
            Next

            'make a copy for the best fitting data 
            Array.Copy(Pmean, BestFit, Pmean.Length)
            'make a copy of the original values so the user can restore the values
            Array.Copy(Pmean, startValues, Pmean.Length)

            'vulnerabilities 
            Array.Copy(m_core.m_EcoSimData.VulMult, Me.orgVul, m_core.m_EcoSimData.VulMult.Length)

            CalculateUpperLowerLimits(True)

#If 0 Then

            Dim FromEcobio As Boolean = True
            If FromEcobio Then
                'Using sw As StreamWriter = New StreamWriter("c:\LME\UpperLowerLimits.csv", True)  'true makes it append
                '    sw.WriteLine(m_core.m_EwEModelName & ", " & Date.Now.ToString)
                For i As Integer = 1 To m_core.nLivingGroups
                    'sw.WriteLine(i.ToString & "," & _
                    '             ParLimit(0, 1, i).ToString & "," & _
                    '             ParLimit(1, 1, i).ToString & _
                    '             "," & ParLimit(0, 4, i).ToString & _
                    '             "," & ParLimit(1, 4, i).ToString & _
                    '             "," & ParLimit(0, 6, i).ToString & "," _
                    '             & ParLimit(1, 6, i).ToString)
                Next
                'sw.Close()
                'End Using
            End If
#End If
            SSorg = m_esdata.SS

            'make sure the ecopath type of run is correct for the monte carlo runs
            m_ecopath.ParameterEstimationType = eEstimateParameterFor.Sensitivity

            'set the ecosim time step delegate for plotting
            If bShowPlot Then
                m_ecosim.TimeStepDelegate = EcosimTimeStep
            Else
                m_ecosim.TimeStepDelegate = Nothing
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".initForRun()", ex)
        End Try

    End Sub



    Public Sub Run(ByVal ob As Object)
        Dim iter As Integer 'number of ecopath interation to find new pararameters for each trial
        Dim Itertot As Integer 'total number of ecopath interation across all the trials
        'Dim NtrialsPerThread As Integer
        'Dim nThreads As Integer

        'Dim MCthreadList As New List(Of cMonteCarloThread)
        'Dim MCthread As cMonteCarloThread

        Dim st As Double = Microsoft.VisualBasic.Timer
        System.Console.WriteLine("----------Starting Monte Carlo----------")
        Try
            initForRun()

            'nThreads = System.Environment.ProcessorCount
            'nThreads = 1
            'NtrialsPerThread = (Ntrials + nThreads - 1) \ nThreads
            'initThreads(MCthreadList, nThreads)

            'tell ecopath to run in silent mode
            'this does not turn off the core's messages just ecopath
            m_ecopath.suppressMessages = True

            'm_ecosim.Run()
            'Dim OrigSS As Single = m_core.EcosimStats.SS

            'Ecosim was run in initForRun()
            'm_esdata.SS is the fit of the currently loaded reference data
            If m_esdata.SS > 0 Then
                SSBestFit = m_esdata.SS
            Else
                SSBestFit = 10000000000000000
            End If

            Dim maxEcopathTries As Integer = 10000
            For iTrial = 1 To Ntrials 'PerThread

                If StopTrial = True Then Exit For

                'number of ecopath interation to find new pararameters
                iter = 0
                RunsSinceLastWithLowerSS += 1

                If Not BalanceEcopathWithNewPars(Pmean, CVpar, iter, maxEcopathTries) Then
                    'Ecopath failed to run stop the trials loop
                    Exit For
                End If

                Itertot = Itertot + iter

                If iter < maxEcopathTries Then

                    'VC Sep 2008 adding vulnerability to MC routine
                    ' The Ecopath balancing above does not need to consider the vulnerabilities, so just set them now before returning:
                    'VC Sep 2008 found that it would increase vulnerabilities to get certain groups to increase initially,
                    'while instead it should have increased the initial biomass, so letting it get started before 
                    'changing vulnerabilities
                    ChangeVulnerabilities(Pmean, CVpar)

                    'For Each MCthread In MCthreadList
                    '    Itertot = Itertot + iter
                    '    Array.Copy(Pmean, MCthread.pmean, Pmean.Length)
                    '    Array.Copy(CVpar, MCthread.CVpar, CVpar.Length)
                    '    MCthread.iter = iter
                    '    MCthread.signalState.Reset()
                    '    ThreadPool.QueueUserWorkItem(AddressOf MCthread.run)
                    'Next

                    'VC Sep 2008: Change the vulmult at this point

                    m_ecosim.Init(True) 'StartEcoSimAgain())

                    'the ecosim time step delegate was set before the loop
                    m_ecosim.Run()

                    'For Each MCthread In MCthreadList
                    '    MCthread.signalState.WaitOne()
                    'Next
                    'm_esdata = MCthread.ESdata
                    'm_epdata = MCthread.EPdata

                    'For Each MCthread In MCthreadList
                    'If MCthread.ESdata.SS < SSBestFit Then
                    'Console.Write(m_esdata.SS.ToString & ", ")

                    If m_esdata.SS < SSBestFit Then
                        RunsSinceLastWithLowerSS = 0
                        'SSBestFit = MCthread.ESdata.SS
                        SSBestFit = m_esdata.SS
                        Console.WriteLine("Total trials: " & iTrial.ToString & ", " & SSBestFit.ToString & ", to fit last Ecopath: " & iter.ToString) '& ", total: " & Itertot.ToString)
                        'sw.WriteLine(iTrial.ToString & ", " & SSBestFit.ToString)

                        'keep the best fits for applying later

                        CheckWhoIsCrashed()
                        For igrp As Integer = 1 To m_core.nGroups
                            'If isCrashed(igrp) Then
                            '    BestFit(eMCParams.Biomass, igrp) = m_epdata.B(igrp) * 1.2
                            'Else
                            BestFit(eMCParams.Biomass, igrp) = m_epdata.B(igrp)
                            'End If
                            BestFit(eMCParams.PB, igrp) = m_epdata.PB(igrp)
                            BestFit(eMCParams.EE, igrp) = m_epdata.EE(igrp)
                            BestFit(eMCParams.BA, igrp) = m_epdata.BA(igrp)
                            'vc sep 2008: adding vulnerability to MC
                            BestFit(eMCParams.Vulnerability, igrp) = m_esdata.VulnerabilityPredator(igrp)
                        Next

                        If bRetainBiomass Then
                            Array.Copy(BestFit, Pmean, BestFit.Length)
                            'VC 2008 don't want it to stop just as it found a better fit so:
                            iTrial = Math.Min(iTrial, CInt(0.9 * Ntrials))

                            'we also need to change the upper and lower limits, and will do this based on new parameters 
                            'CheckWhoIsCrashed()
                            'CalculateUpperLowerLimits()
                        End If
                        'm_core.Save()

                    End If
                    'Next
                End If
                'TrialProgress(itrial * nThreads, iter)
                TrialProgress(iTrial, iter)
                EcopathIterationsProgress(iter)

                If RunsSinceLastWithLowerSS > 2000 Then Exit For
            Next iTrial

            'restore ecopath back to its original state
            Me.restoreOriginalState()

            System.Console.WriteLine("Finished Monte Carlo. Run time = " & CStr(Microsoft.VisualBasic.Timer - st))

            Me.CompletedCallback()

            m_ecopath.suppressMessages = False

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            m_ecopath.suppressMessages = False
            Throw New ApplicationException(Me.ToString & ".Run", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Restore Ecopath to its original state
    ''' </summary>
    ''' <remarks>The Monte Carlo changed to basic input data of Ecopath this will set it back to the state it was in when the Monte Carlo was run.</remarks>
    Private Sub restoreOriginalState()
        Dim bSuccess As Boolean

        Try

            'Set Ecopath inputs back to original values
            'VC Oct 02. below was setting, b, pb, ee, ba, but it needs to set input parameters,so I've changed this
            For i As Integer = 1 To Me.m_epdata.NumLiving
                If m_epdata.Binput(i) > 0 Then m_epdata.Binput(i) = startValues(eMCParams.Biomass, i)
                If m_epdata.PBinput(i) > 0 Then m_epdata.PBinput(i) = startValues(eMCParams.PB, i)
                If m_epdata.EEinput(i) > 0 Then m_epdata.EEinput(i) = startValues(eMCParams.EE, i)
                m_epdata.BA(i) = startValues(eMCParams.BA, i)
                'vc sep 2008: adding vulnerability to MC
                m_esdata.VulnerabilityPredator(i) = startValues(eMCParams.Vulnerability, i)
            Next

            'set vulnerabilities back 
            Array.Copy(Me.orgVul, m_core.m_EcoSimData.VulMult, m_core.m_EcoSimData.VulMult.Length)

            'copy the data from the input parameters into the modeling parameters
            Me.m_epdata.CopyInputToModelArrays()

            'run Ecopath with the original values to reset computed variables
            bSuccess = Me.m_ecopath.Run()

            'init stanza groups back to the original values
            Me.m_ecosim.InitStanza()

            Me.m_ecosim.Init(True)

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            bSuccess = False
        End Try

        If Not bSuccess Then
            Me.m_core.Messages.AddMessage(New cMessage(My.Resources.CoreMessages.MONTECARLO_RESTORE_FAILED, eMessageType.ErrorEncountered, eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Warning))
        End If

    End Sub


    Private Sub TrialProgress(ByVal iTrial As Integer, ByVal iEcopathIterations As Integer)

        Try
            Me.nTrialIterations = iTrial
            Me.nEcopathIterations = iEcopathIterations
            If dlgTrialStepHandler IsNot Nothing Then
                Me.dlgTrialStepHandler()
            End If
        Catch ex As Exception
            'Bogus Dude.....the interface through an error 
            'just keep ploughing on
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub EcopathIterationsProgress(ByVal iEcopathIterations As Integer)

        Try
            Me.nEcopathIterations = iEcopathIterations

            If dlgEcopathIterationHandler IsNot Nothing Then
                dlgEcopathIterationHandler()
            End If
        Catch ex As Exception
            'Bogus Dude.....the interface through an error 
            'just keep plowing on
            cLog.Write(ex)
        End Try


    End Sub


    Private Sub CompletedCallback()
        Try
            If dlgMonteCarloCompletedHandler IsNot Nothing Then
                Me.dlgMonteCarloCompletedHandler()
            End If
        Catch ex As Exception
            Debug.Assert(False, "Monte Carlo CompletedCallback Exception: " & ex.Message)
        End Try

    End Sub


    Private Function BalanceEcopathWithNewPars(ByVal ParCurVal(,) As Single, _
                                               ByVal CVpar(,) As Single, _
                                               ByRef iter As Integer, _
                                               ByVal maxEcopathIterations As Integer) As Boolean
        'EwE5 StartEcosimWithNewPars(ByVal Pstartup(,) As Single, ByVal CVpar(,) As Single, ByVal iter As Long)
        Dim igrp As Integer
        Dim bEcopathNeedsBalancing As Boolean

        Try
            'Dim BBar As Single
            AbortRun = True
            bEcopathNeedsBalancing = True
            Do While bEcopathNeedsBalancing
                iter = iter + 1
                m_epdata.CopyInputToModelArrays() 'MakeUnknownUnknown())

                For igrp = 1 To m_core.nLivingGroups                               ' Using default if not
                    If m_ecopath.missing(igrp, 1) = False Then                   ' Then B is an input par
                        'If isCrashed(igrp) Then
                        '    BBar = 1.2 * ParCurVal(eMCParams.Biomass, igrp)
                        'Else
                        '    BBar = ParCurVal(eMCParams.Biomass, igrp)
                        'End If
                        m_epdata.B(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.Biomass, igrp), _
                                                             CVpar(eMCParams.Biomass, igrp), _
                                                             ParLimit(0, eMCParams.Biomass, igrp), _
                                                             ParLimit(1, eMCParams.Biomass, igrp), _
                                                             isCrashed(igrp))
                        m_epdata.BA(igrp) = ChooseFeasibleBA(m_epdata.B(igrp), _
                                                             ParCurVal(eMCParams.BA, igrp), _
                                                             CVpar(eMCParams.BA, igrp), _
                                                             ParLimit(0, eMCParams.BA, igrp), _
                                                             ParLimit(1, eMCParams.BA, igrp))
                    End If
                        If m_ecopath.missing(igrp, 2) = False Then                   ' Then PB is an input par
                            m_epdata.PB(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.PB, igrp), _
                                                                  CVpar(eMCParams.PB, igrp), _
                                                                  ParLimit(0, eMCParams.PB, igrp), _
                                                                  ParLimit(1, eMCParams.PB, igrp), _
                                                                  False)
                        End If
                        If m_ecopath.missing(igrp, 4) = False Then                   ' Then EE is an input par
                            m_epdata.EE(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.EE, igrp), _
                                                                  CVpar(4, igrp), _
                                                                  ParLimit(0, eMCParams.EE, igrp), _
                                                                  ParLimit(1, eMCParams.EE, igrp), _
                                                                  False)
                        End If
                Next igrp

                m_ecosim.InitStanza()

                'Estimate basic params
                If Not m_ecopath.Run() Then

                    ' ''Failed to estimate parameters
                    Dim status As eStatusFlags = m_ecopath.EstimationStatus
                    Dim msg As cMessage
                    If status = eStatusFlags.MissingParameter Then
                        msg = New cMessage("Monte Carlo: To many missing parameters to run Ecopath. Check your input parameters.", eMessageType.TooManyMissingParameters, eCoreComponentType.EcoSim, eMessageImportance.Critical)
                    Else
                        msg = New cMessage("Error in Ecopath Monte Carlo trials could not be run.", eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Critical)
                    End If
                    ' m_manager.AddMessage(msg)
                    'Return False
                End If

                m_ecopath.DetritusCalculations()

                bEcopathNeedsBalancing = False
                For igrp = 1 To m_core.nGroups
                    If m_epdata.EE(igrp) > 1.0005 Or m_epdata.EE(igrp) < 0 Then
                        'this loop did not balance Ecopath
                        bEcopathNeedsBalancing = True
                        Exit For
                    End If
                Next

                'tell the interface
                'EcopathIterationsProgress(iter)

                If StopTrial = True Then Exit Do

                If iter > maxEcopathIterations Then
                    'max number of iteration to find balanced ecopath model
                    'it is OK to try again so return True
                    Return True 'frmBvary.lblNoGood.Caption = "Cannot find feasible Ecopath model; Quitting": Exit Sub
                End If

            Loop


            'change 


        Catch ex As Exception
            Debug.Assert(False, ex.StackTrace)
            cLog.Write(ex)
            Throw New ApplicationException(Me.ToString & ".BalanceEcopathWithNewPars()", ex)
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Apply the results of the Monte Carlo trials (best fitting parameters) to the ecopath data
    ''' </summary>
    ''' <remarks>This does not update the Core's interface objects</remarks>
    Friend Sub ApplyBestFits()

        'user wants to keep the best fit parameters
        For iPred As Integer = 1 To m_core.nGroups
            If m_ecopath.missing(iPred, 1) = False Then
                m_epdata.Binput(iPred) = BestFit(eMCParams.Biomass, iPred)
                m_epdata.BHinput(iPred) = BestFit(eMCParams.Biomass, iPred) / m_epdata.Area(iPred)
            End If
            If m_ecopath.missing(iPred, 2) = False Then
                m_epdata.PBinput(iPred) = BestFit(eMCParams.PB, iPred)
            End If
            If m_ecopath.missing(iPred, 4) = False Then
                m_epdata.EEinput(iPred) = BestFit(eMCParams.EE, iPred)
            End If


            m_epdata.BA(iPred) = BestFit(eMCParams.BA, iPred)
            'vc sep 2008: adding vulnerability to MC
            m_esdata.VulnerabilityPredator(iPred) = BestFit(eMCParams.Vulnerability, iPred)
            'Also transfer to vulmult
            For iPrey As Integer = 1 To m_core.nGroups
                m_esdata.VulMult(iPrey, iPred) = BestFit(eMCParams.Vulnerability, iPred)
                'jb this is done by the manager in ApplyBestFits core.onChanged() 
                m_core.EcoSimGroupInputs(iPrey).VulMult(iPred) = BestFit(eMCParams.Vulnerability, iPred)
            Next


            'ToDo_jb cEcosimMonteCarlo.Run something is wrong here
            'I don't have a BAinput BA will contain the best fit parameters
            ' m_epdata.BAinput(i) = m_epdata.BA(i)
            '    optVary_Click(0)
        Next

    End Sub

    Private Sub redimVariables()
        Try

            ReDim ParLimit(1, 6, m_core.nGroups)
            ReDim CVpar(6, m_core.nGroups)

            For i As Integer = 1 To m_core.nGroups
                For j As Integer = 0 To 4
                    CVpar(j, i) = 0.1
                Next
                CVpar(5, i) = 0.05
                CVpar(6, i) = 0.1
            Next

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".redimVariables()", ex)
        End Try


    End Sub

    Public Sub CalculateUpperLowerLimits(ByVal IsCrashEvaluated As Boolean)

        Dim i As Integer
        Try
            Dim factor As Integer = 100 'IIf(IsCrashEvaluated, 1000, 2)
            'We want a wide range for searching, cv will still limit the steps
            For i = 1 To m_core.nLivingGroups
                'If IsCrashEvaluated Then factor = IIf(isCrashed(i), 4, 2)
                'VC Sep 2008 changed it to use best fit for calculating limits:
                ''Lower
                'ParLimit(0, 1, i) = BestFit(eMCParams.Biomass, i) * (1 - factor * CVpar(1, i)) : If ParLimit(0, 1, i) < 0 Then ParLimit(0, 1, i) = 0
                'ParLimit(0, 2, i) = BestFit(eMCParams.PB, i) * (1 - factor * CVpar(2, i)) : If ParLimit(0, 2, i) < 0 Then ParLimit(0, 2, i) = 0
                'ParLimit(0, 4, i) = BestFit(eMCParams.EE, i) * (1 - factor * CVpar(4, i)) : If ParLimit(0, 4, i) < 0 Then ParLimit(0, 4, i) = 0
                ''BA is +- relative to B not to BA (which is usually zero)
                'ParLimit(0, 5, i) = BestFit(eMCParams.BA, i) + m_epdata.B(i) * (-factor * CVpar(5, i))
                ''Vul is from 1 up
                'ParLimit(0, 6, i) = BestFit(eMCParams.Vulnerability, i) * (1 - factor * CVpar(6, i)) : If ParLimit(0, 6, i) < 1.01 Then ParLimit(0, 6, i) = 1.01

                ''upper
                ''factor = IIf(isExploded(i), 0.1, 2)
                'ParLimit(1, 1, i) = m_epdata.B(i) * (1 + factor * CVpar(1, i))
                'ParLimit(1, 2, i) = m_epdata.PB(i) * (1 + factor * CVpar(2, i))
                'ParLimit(1, 4, i) = m_epdata.EE(i) * (1 + factor * CVpar(4, i)) : If ParLimit(1, 4, i) > 1 Then ParLimit(1, 4, i) = 1
                ''BA is +- relative to B not to BA (which is usually zero)
                'ParLimit(1, 5, i) = 1.001 'm_epdata.BA(i) + m_epdata.B(i) * (factor * CVpar(5, i))
                'ParLimit(1, 6, i) = 1000 ' m_esdata.VulnerabilityPredator(i) * (1 + factor * CVpar(6, i)) 'no upper limit for vulmult : If ParLimit(1, 6, i) > 1 Then ParLimit(1, 6, i) = 1


                'Lower
                ParLimit(0, 1, i) = m_epdata.B(i) * (1 - factor * CVpar(1, i)) : If ParLimit(0, 1, i) < 0 Then ParLimit(0, 1, i) = 0.0000000001
                ParLimit(0, 2, i) = m_epdata.PB(i) * (1 - factor * CVpar(2, i)) : If ParLimit(0, 2, i) < 0 Then ParLimit(0, 2, i) = 0.0000000001
                ParLimit(0, 4, i) = m_epdata.EE(i) * (1 - factor * CVpar(4, i)) : If ParLimit(0, 4, i) < 0 Then ParLimit(0, 4, i) = 0
                'BA is +- relative to B not to BA (which is usually zero)
                ParLimit(0, 5, i) = m_epdata.BA(i) + m_epdata.B(i) * (-factor * CVpar(5, i))
                'Vul is from 1 up
                ParLimit(0, 6, i) = m_esdata.VulnerabilityPredator(i) * (1 - factor * CVpar(6, i)) : If ParLimit(0, 6, i) < 1.01 Then ParLimit(0, 6, i) = 1.01

                'upper
                ParLimit(1, 1, i) = m_epdata.B(i) * (1 + factor * CVpar(1, i)) : If ParLimit(1, 1, i) <= ParLimit(0, 1, i) Then ParLimit(1, 1, i) = 10 * ParLimit(0, 1, i)
                ParLimit(1, 2, i) = m_epdata.PB(i) * (1 + factor * CVpar(2, i)) : If ParLimit(1, 2, i) <= ParLimit(0, 2, i) Then ParLimit(1, 2, i) = 10 * ParLimit(0, 2, i)
                ParLimit(1, 4, i) = m_epdata.EE(i) * (1 + factor * CVpar(4, i)) : If ParLimit(1, 4, i) > 1 Then ParLimit(1, 4, i) = 1
                'BA is +- relative to B not to BA (which is usually zero)
                ParLimit(1, 5, i) = m_epdata.BA(i) + m_epdata.B(i) * (factor * CVpar(5, i))
                ParLimit(1, 6, i) = 1000 ' m_esdata.VulnerabilityPredator(i) * (1 + factor * CVpar(6, i)) 'no upper limit for vulmult : If ParLimit(1, 6, i) > 1 Then ParLimit(1, 6, i) = 1

            Next


        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".Run", ex)
        End Try


    End Sub

    Private Function ChooseFeasiblePar(ByVal xbar As Single, ByVal CV As Single, ByVal ParMin As Single, ByVal ParMax As Single, ByVal isCrashed As Boolean) As Single
        Dim X As Single, ict As Integer
        '  Static Answer As Object

        'if the populatoin is crashed then double the cv:
        'Dim cvFactor As Double = 0.02 ' 0.01 + 0.5 * Math.Log10(RunsSinceLastWithLowerSS) ' IIf(isCrashed, 2, 1)


        'Debug.Assert(ParMin <> ParMax, Me.ToString & ".ChooseFeasiblePar() ParMax = ParMin!!!!!")

        Do
            'jb 7-Dec-2010 ChooseFeasiblePar() changed application of CV 
            ' X = xbar * (1 + 0.02 * CV * RandomNormal())
            X = xbar * (1 + CV * RandomNormal())

            If X >= ParMin And X <= ParMax Then
                ChooseFeasiblePar = X
                Exit Function
            End If
            ict = ict + 1
            If ict > 10000 Then
                'If Answer <> vbCancel Then
                '    Answer = MsgBox("Can't find acceptable parameter, using mean", vbOKCancel)
                'End If
                System.Console.WriteLine("ChooseFeasiblePar() Can't find acceptable parameter, using mean")
                ChooseFeasiblePar = xbar
                Exit Function
            End If
        Loop
    End Function

    Private Function ChooseFeasibleBA(ByVal Biomass As Single, ByVal xbar As Single, ByVal CV As Single, ByVal ParMin As Single, ByVal ParMax As Single) As Single
        Dim X As Single, ict As Integer
        Do
            X = xbar + Biomass * (CV * RandomNormal())
            If X >= ParMin And X <= ParMax Then
                ChooseFeasibleBA = X
                Exit Function
            End If
            ict = ict + 1
            If ict > 10000 Then
                'System.Console.WriteLine("Monte Carlo Can't find acceptable parameter for BA, using mean.")
                'If done = False Then RetVal = MsgBox("Can't find acceptable parameter, using mean. Press 'Cancel' to avoid this message", vbOKCancel)
                'If RetVal = vbCancel Then done = True
                ChooseFeasibleBA = 0    'xbar
                Exit Function
            End If
        Loop
    End Function

    Private Function RandomNormal() As Single
        Dim i As Integer, X As Single
        X = -6
        For i = 1 To 12 : X = X + Rnd() : Next
        RandomNormal = X
    End Function

    Private Sub ChangeVulnerabilities(ByVal ParCurVal(,) As Single, ByVal CVpar(,) As Single)

        'm_epdata.EE(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.Vulnerability, igrp), _
        '                              CVpar(6, igrp), ParLimit(0, eMCParams.Vulnerability, igrp), _
        '                             ParLimit(1, eMCParams.Vulnerability, igrp))
        '        Using sw As StreamWriter = New StreamWriter("c:\LME\Vulnerabilities.csv", True)  'true makes it append
        For iPred As Integer = 1 To m_core.nLivingGroups
            m_esdata.VulnerabilityPredator(iPred) = ChooseFeasiblePar(ParCurVal(eMCParams.Vulnerability, iPred), _
                                                                     CVpar(6, iPred), _
                                                                     ParLimit(0, eMCParams.Vulnerability, iPred), _
                                                                     ParLimit(1, eMCParams.Vulnerability, iPred), _
                                                                     False)
            For iPrey As Integer = 1 To m_core.nGroups
                m_esdata.VulMult(iPrey, iPred) = m_esdata.VulnerabilityPredator(iPred)
            Next
            '    sw.WriteLine(iPred.ToString & ", " & m_esdata.VulnerabilityPredator(iPred).ToString)
        Next
        'sw.Close()
        'End Using

    End Sub

    Private Sub CheckWhoIsCrashed()
        Dim EndTime As Integer = (m_core.EcoSimModelParameters.NumberYears - 1) * 12
        'Dim sStr As String = "Crashed: "
        For iGrp As Integer = 1 To m_core.nLivingGroups

            If Me.m_esdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, EndTime) / m_core.EcoPathGroupOutputs(iGrp).Biomass < 0.01 Then
                'jb use the core arrays instead of the Ecosim Output objects because the output objects have not been initialized
                'If m_core.EcoSimGroupOutputs(iGrp).Biomass(EndTime) / m_core.EcoPathGroupOutputs(iGrp).Biomass < 0.01 Then
                isCrashed(iGrp) = True
                'sStr += iGrp.ToString & ", "
            Else
                isCrashed(iGrp) = False
            End If
            'If m_core.EcoSimGroupOutputs(iGrp).Biomass(EndTime) / m_core.EcoPathGroupOutputs(iGrp).Biomass > 10 Then
            '    isexploded(iGrp) = True
            'Else
            '    isExploded(iGrp) = False
            'End If
        Next
        'If sStr <> "Crashed: " Then
        '    Using sw As StreamWriter = New StreamWriter("c:\LME\Vulnerabilities.csv", True)  'true makes it append
        '        sw.WriteLine(iTrial.ToString & ", " & sStr)
        '        sw.Close()
        '    End Using
        '    Console.WriteLine(sStr)
        'End If
    End Sub


#Region "xxx DEAD CODE (Multi threaded Monte Carlo) xxx"

#If 0 Then

    ''' <summary>
    ''' Multi threaded Monte Carlo code has been disabled but left in place for future reference
    ''' </summary>
    Private Sub initThreads(ByVal trList As List(Of cMonteCarloThread), ByVal nThreads As Integer)
        'gives back a list (nThreads long) of fully initialized cMonteCarloThread objects

        Dim MCthread As cMonteCarloThread

        Try
            For i As Integer = 1 To nThreads
                MCthread = New cMonteCarloThread(i)
                MCthread.init(m_core.nGroups, m_core.nLivingGroups)

                'get ep data
                m_epdata.copyTo(MCthread.EPdata)
                MCthread.EP.ModelingData = MCthread.EPdata
                MCthread.EP.ParameterEstimationType = m_ecopath.ParameterEstimationType
                MCthread.EP.EstimateParameters()
                MCthread.ES.EcopathParameters = MCthread.EPdata
                MCthread.EP.missing = m_ecopath.missing.Clone

                'init ES and copy data
                m_esdata.CopyTo(MCthread.ESdata)
                m_ecosim.copyTo(MCthread.ES)
                MCthread.ES.m_Data = MCthread.ESdata
                MCthread.ES.SetCounters()
                'MCthread.ES.SetDefaultParameters()

                'get other data structures
                m_stanza.copyTo(MCthread.StanzaData)
                MCthread.ES.TracerData = New cContaminantTracerDataStructures
                m_tracerData.CopyTo(MCthread.ES.TracerData)

                'link models to data structures
                MCthread.ES.m_stanza = MCthread.StanzaData
                MCthread.ES.TimeSeriesData = m_ecosim.TimeSeriesData
                MCthread.ES.SearchData = m_ecosim.SearchData
                'MCthread.ES.TimeStepDelegate = m_ecosim.TimeStepDelegate

                'init some ecosim stuff

                'MCthread.ES.m_Data.RedimVars()
                MCthread.ES.InitStanza()

                'assign thread properties
                MCthread.pmean = Pmean.Clone
                MCthread.CVpar = CVpar.Clone
                MCthread.parLimit = ParLimit.Clone

                trList.Add(MCthread)
            Next

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        'mcthread.iter=iter
    End Sub

#End If

#End Region
End Class
