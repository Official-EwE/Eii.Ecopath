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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEUtils.Core
Imports EwEPlugin
Imports System.Threading

#End Region ' Imports

Public Enum eMCParams
    NotSet = -1
    Biomass = 1
    PB = 2
    QB = 3
    EE = 4
    BA = 5
    Vulnerability = 6  '(one per consumer) same for all prey
    OtherMort = 7
End Enum

''' <summary>
''' Call each time a monte carlo trial has been completed
''' </summary>
Public Delegate Sub MonteCarloTrialProgressDelegate()

''' <summary>
''' Call each time a Ecopath model has been run
''' </summary>
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
Public Class cEcosimMonteCarlo

    Public Const EE_TOL As Single = 0.0005

    Public CVpar(,) As Single
    Public ParLimit(,,) As Single

    Public Property Ntrials As Integer
    Public Property StopTrial As Boolean
    Public Property RetainBiomass As Boolean

    ''' <summary>
    ''' Flag, states whether to include Stock Reduction Analysis (SRA) for groups with forced catches
    ''' </summary>
    Public Property IncludeFpenalty As Boolean

    ''' <summary>
    ''' F/M ratio for SRA 
    ''' </summary>
    Public Property FMratioForSRA As Single = 1

    Public Property maxEcopathTries As Integer = MAX_ECOPATH_TRIES

    ''' <summary>
    ''' Optional <see cref="EcoSimTimeStepDelegate">delegate</see> that will be called after a 
    ''' trial has been computed.
    ''' </summary>
    Friend EcosimTimeStep As EcoSimTimeStepDelegate

    ''' <summary>
    ''' Optional <see cref="MonteCarloEcopathProgressDelegate">delegate</see> that will be called 
    ''' each attempt to find a balanced Ecopath model.
    ''' </summary>
    Friend dlgEcopathIterationHandler As MonteCarloEcopathProgressDelegate

    ''' <summary>
    ''' Optional <see cref="MonteCarloTrialProgressDelegate">delegate</see> that will be called after a 
    ''' trial has been completed.
    ''' </summary>
    Friend dlgTrialStepHandler As MonteCarloTrialProgressDelegate

    ''' <summary>
    ''' Optional <see cref="MonteCarloCompletedDelegate">delegate</see> that will be called after a 
    ''' Monte Carlo run has completed.
    ''' </summary>
    Friend dlgMonteCarloCompletedHandler As MonteCarloCompletedDelegate

    ''' <summary>
    ''' Optional <see cref="MonteCarloSendMessageDelegate">delegate</see> that allows Monte Carlo
    ''' to send <see cref="cMessage">messages</see>.
    ''' </summary>
    Friend dlgMonteCarloMessageHandler As MonteCarloSendMessageDelegate

    Public nEcopathIterations As Integer
    Public nTrialIterations As Integer

    ''' <summary>
    ''' Best fitting Sum of Squares computed by Ecosim
    ''' </summary>
    Public SSBestFit As Single

    ''' <summary>
    ''' Sum of Squares computed by Ecosim of the current iteration.
    ''' </summary>
    Public SSCurrent As Single

    ''' <summary>
    ''' Sum of Squares prior to the Monte Carlo run.
    ''' </summary>
    Public SSorg As Single

    Public EcopathEETol As Single


    Public bShowPlot As Boolean
    ''' <summary>
    ''' Get/set whether output should be saved to file automatically.
    ''' </summary>
    Public Property bSaveOutput As Boolean
        Get
            Return Me.m_core.Autosave(eAutosaveTypes.MonteCarlo)
        End Get
        Set(value As Boolean)
            Me.m_core.Autosave(eAutosaveTypes.MonteCarlo) = value
        End Set
    End Property

    Private Const MAX_ECOPATH_TRIES As Integer = 10000

    Private m_core As cCore
    Private m_ecopath As cEcoPathModel
    Private m_ecosim As cEcoSimModel
    Private m_epdata As cEcopathDataStructures
    Private m_esdata As cEcosimDatastructures
    Private m_tsdata As cTimeSeriesDataStructures
    Private m_stanza As cStanzaDatastructures 'needs to come in from the core
    Private m_tracerData As cContaminantTracerDataStructures
    Private m_pluginmanager As cPluginManager

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

    Dim m_ouputWriter As cMonteCarloResultsWriter

    Private m_rand As Random

    Private m_isVariable(,) As Boolean

    Public Sub New(ByRef theCore As cCore)

        m_core = theCore

        m_ecopath = m_core.m_EcoPath
        m_ecosim = m_core.m_EcoSim
        m_epdata = m_core.m_EcoPathData
        m_esdata = m_core.m_EcoSimData
        m_tsdata = m_core.m_TSData
        'data from Ecosim
        m_stanza = m_ecosim.m_stanza
        m_tracerData = m_ecosim.TracerData

        Ntrials = 20 'default number of trials
        EcopathEETol = 0.0005 '0.05%

        m_rand = New Random(CInt(Date.Now.Ticks Mod Integer.MaxValue))

        Me.m_ouputWriter = New cMonteCarloResultsWriter(Me, Me.m_core)

    End Sub

    Public Sub initRandomSequence(seed As Integer)
        m_rand = New Random(seed)
    End Sub

    Public Function Init() As Boolean

        Try
            'Used to debug Fpenalty
            'Debug.Assert(False, "Include F Penalty has been set for debugging.")
            'IncludeFpenalty = True

            'set if a parameter can be varied
            'redimVariables() needs m_isVariable(group,parameter) to be set before it is called

            Me.maxEcopathTries = MAX_ECOPATH_TRIES
            Me.setIsVariable()

            Me.redimVariables()
            m_pluginmanager = Me.m_core.PluginManager

            'vc sep 2008: adding vulnerability to MC: changed first dimension from 5 to 6
            ReDim Pmean(Me.NumParams(), m_core.nGroups)
            ReDim startValues(Me.NumParams(), m_core.nGroups)
            ReDim BestFit(Me.NumParams(), m_core.nGroups)
            ReDim orgVul(m_core.nGroups, m_core.nGroups)

            For igrp As Integer = 1 To m_core.nGroups
                Pmean(eMCParams.Biomass, igrp) = m_epdata.B(igrp)
                Pmean(eMCParams.PB, igrp) = m_epdata.PB(igrp)
                ' JS13feb12 added
                Pmean(eMCParams.QB, igrp) = m_epdata.QB(igrp)

                Pmean(eMCParams.EE, igrp) = m_epdata.EE(igrp)
                Pmean(eMCParams.BA, igrp) = m_epdata.BA(igrp)
                'vc sep 2008: adding vulnerability to MC
                Pmean(eMCParams.Vulnerability, igrp) = m_esdata.VulnerabilityPredator(igrp)
                'js feb 2011: added other mort
                Pmean(eMCParams.OtherMort, igrp) = m_epdata.OtherMortinput(igrp)
            Next
            CalculateUpperLowerLimits(False)

            ' Fire plug-in point
            If Me.m_pluginmanager IsNot Nothing Then
                Try
                    Me.m_core.m_SearchData.SearchMode = eSearchModes.MonteCarlo
                    Me.m_pluginmanager.SearchInitialized(Me.m_core.m_SearchData)
                    Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
                    Me.m_pluginmanager.MontCarloInitialized(Me)
                Catch ex As Exception
                    cLog.Write(ex, "cEcosimMonteCarlo::Init")
                End Try
            End If

            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".Run", ex)
        End Try


    End Function

    ''' <summary>
    ''' Set the isVariable(group,parameter) boolean flag
    ''' </summary>
    ''' <remarks>Can the MonteCarlo vary an Ecopath parameter </remarks>
    Private Sub setIsVariable()
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'jb 22-Mar-2014 
        'Added the isVariable() to check if a parameter can be varied
        ReDim m_isVariable(m_core.nGroups, Me.NumParams())
        For iGrp As Integer = 1 To m_core.nGroups

            Me.m_isVariable(iGrp, eMCParams.Biomass) = (Not Me.m_ecopath.missing(iGrp, 1)) And Me.isStanzaGroupVariable(iGrp, eMCParams.Biomass)
            'Use the B index in missing(group,variable) from Ecopath for BA
            Me.m_isVariable(iGrp, eMCParams.BA) = (Not Me.m_ecopath.missing(iGrp, 1)) And Me.isStanzaGroupVariable(iGrp, eMCParams.BA)

            Me.m_isVariable(iGrp, eMCParams.PB) = (Not Me.m_ecopath.missing(iGrp, 2))
            'QB needs to check the input variable
            Me.m_isVariable(iGrp, eMCParams.QB) = ((Not Me.m_ecopath.missing(iGrp, 3)) And Me.isStanzaGroupVariable(iGrp, eMCParams.QB)) And (Me.m_epdata.QBinput(iGrp) > 0)
            Me.m_isVariable(iGrp, eMCParams.EE) = (Not Me.m_ecopath.missing(iGrp, 4))

        Next
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    End Sub

    Public Sub Clear()

        Me.Pmean = Nothing
        Me.startValues = Nothing
        Me.BestFit = Nothing
        Me.orgVul = Nothing
        Me.ParLimit = Nothing
        Me.CVpar = Nothing

    End Sub

    Private Function PedigreeVarToMCIndex(ByVal vn As eVarNameFlags) As eMCParams

        Select Case vn
            Case eVarNameFlags.BiomassAreaInput : Return eMCParams.Biomass
            Case eVarNameFlags.PBInput : Return eMCParams.PB
            Case eVarNameFlags.QBInput : Return eMCParams.QB
        End Select

        System.Console.WriteLine(Me.ToString & ".PedigreeVarToMCIndex() Invalid VarName '" & vn.ToString & "'")
        Return eMCParams.NotSet

    End Function

    ''' <summary>
    ''' Load CV values for a given variable from Pedigree.
    ''' </summary>
    ''' <param name="varname"></param>
    Friend Function LoadFromPedigree(varname As eVarNameFlags) As Boolean

        Dim opt As Integer ' Opt = pedigree level
        Dim man As cPedigreeManager = Nothing
        Dim parm As eMCParams = eMCParams.NotSet
        Dim iVar As Integer = Me.m_core.PedigreeVariableIndex(varname)

        If (iVar <= 0) Then Return False

        ' For all groups
        For i As Integer = 1 To Me.m_epdata.NumGroups
            ' Read assigned pedigree level for a group (was 'Opt = ReadPedigreeFromDatabase(Par)')
            opt = Me.m_epdata.Pedigree(i, iVar)
            If opt > 0 Then ' Non-estimated level
                Try

                    Select Case varname

                        Case eVarNameFlags.BiomassAreaInput, _
                             eVarNameFlags.PBInput, _
                             eVarNameFlags.QBInput
                            parm = Me.PedigreeVarToMCIndex(varname)
                            CVpar(parm, i) = Me.m_epdata.PedigreeLevelConfidence(opt) / 100.0! / 2.0!

                    End Select
                Catch ex As Exception
                    cLog.Write(ex, "cEcosimMonteCarlo::LoadFromPedigree(" & varname.ToString & ")")
                    Return False
                End Try
            End If
        Next
        Return True

    End Function

    Public Sub initForRun()

        Try

            StopTrial = False
            m_esdata.SS = 0

            'This gives the same sequence of random numbers 
            'Used for debugging
            'm_rand = New Random(666)

            ReDim isCrashed(m_core.nGroups)
            ReDim isExploded(m_core.nGroups)

            m_ecosim.Init(True)

            m_core.m_EcoSimData.bTimestepOutput = True
            m_ecosim.TimeStepDelegate = Nothing

            'jb remove vulnerabilities until there is a proper interface
            'if it is left in place it causes problem because it changes the vulnerabilities
            ''Set the all vulnerabilities to a predator to the max across all prey
            ''This is the same as setting all the columns in the Vulnerabiltiy matrix to the same value
            'For iPred As Integer = 1 To m_core.nGroups
            '    Dim vul As Single = 0
            '    For iPrey As Integer = 1 To m_core.nGroups
            '        'jb 18-Nov-2011 Changed from first non zero vulnerability 
            '        'To max vulnerability across all prey for this pred  
            '        vul = Math.Max(vul, m_core.m_EcoSimData.VulMult(iPrey, iPred))
            '        'If m_core.m_EcoSimData.VulMult(iPrey, iPred) > 0 Then vul = m_core.m_EcoSimData.VulMult(iPrey, iPred) : Exit For
            '    Next
            '    'Max vulnerability to this predator
            '    m_core.m_EcoSimData.VulnerabilityPredator(iPred) = vul
            'Next

            'run ecosim to get the fit (SS) of the ref data to the current ecopath parameters
            m_ecosim.Run()

            For iGrp As Integer = 1 To m_core.nGroups
                Pmean(eMCParams.Biomass, iGrp) = m_epdata.B(iGrp)
                Pmean(eMCParams.PB, iGrp) = m_epdata.PB(iGrp)
                Pmean(eMCParams.EE, iGrp) = m_epdata.EE(iGrp)
                Pmean(eMCParams.BA, iGrp) = m_epdata.BA(iGrp)
                Pmean(eMCParams.QB, iGrp) = m_epdata.QB(iGrp)
                'vc sep 2008: adding vulnerability to MC
                'Pmean(eMCParams.Vulnerability, iGrp) = m_esdata.VulnerabilityPredator(iGrp)
                'js feb 2011: added other mort
                Pmean(eMCParams.OtherMort, iGrp) = m_epdata.OtherMortinput(iGrp)

            Next

            'make a copy for the best fitting data 
            Array.Copy(Pmean, BestFit, Pmean.Length)
            'make a copy of the original values so the user can restore the values
            Array.Copy(Pmean, startValues, Pmean.Length)

            'vulnerabilities 
            'Array.Copy(m_core.m_EcoSimData.VulMult, Me.orgVul, m_core.m_EcoSimData.VulMult.Length)

            'jb Mar-24-2011 Do NOT reset Upper and Lower Parameter Limits 
            'they may have been edited by a user and this will overwrite the edits with defaults
            'CalculateUpperLowerLimits(True)

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

            If Me.m_pluginmanager IsNot Nothing Then
                Try
                    Me.m_pluginmanager.MonteCarloRunInitialized()
                Catch ex As Exception
                    cLog.Write(ex, "cEcosimMonteCarlo::InitForRun")
                End Try
            End If

            Me.m_ouputWriter.Init()

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".initForRun()", ex)
        End Try

    End Sub



    Public Sub Run(ByVal ob As Object)
        Dim iter As Integer 'number of ecopath interation to find new pararameters for each trial
        Dim Fpenalty As Single
        Dim bFirstRun As Boolean = True
        'Dim NtrialsPerThread As Integer
        'Dim nThreads As Integer

        'Dim MCthreadList As New List(Of cMonteCarloThread)
        'Dim MCthread As cMonteCarloThread
        Dim bForcedCatches(Me.m_epdata.NumGroups) As Boolean
        For its As Integer = 1 To m_tsdata.nTimeSeries
            If m_tsdata.TimeSeriesType(its) = eTimeSeriesType.CatchesForcing Then
                bForcedCatches(m_tsdata.iPool(its)) = True
            End If
        Next

        System.Console.WriteLine("----------Starting Monte Carlo----------")
        Try
            initForRun()

            ' Fire plug-in point
            If Me.m_pluginmanager IsNot Nothing Then
                Try
                    Me.m_pluginmanager.SearchIterationsStarting()
                Catch ex As Exception
                    cLog.Write(ex, "cEcosimMonteCarlo::Run(SearchIterationsStarting)")
                End Try
            End If

            'nThreads = System.Environment.ProcessorCount
            'nThreads = 1
            'NtrialsPerThread = (Ntrials + nThreads - 1) \ nThreads
            'initThreads(MCthreadList, nThreads)

            'tell ecopath to run in silent mode
            'this does not turn off the core's messages just ecopath
            m_ecopath.suppressMessages = True

            'Ecosim was run in initForRun()
            'm_esdata.SS is the fit of the currently loaded reference data
            If Me.isTimeSeriesLoaded Then
                SSBestFit = m_esdata.SS
            Else
                SSBestFit = 0
            End If

            For iTrial = 1 To Ntrials 'PerThread

                If StopTrial = True Then Exit For

                'number of ecopath interation to find new pararameters
                iter = 0
                RunsSinceLastWithLowerSS += 1

                If BalanceEcopathWithNewPars(Pmean, CVpar, iter, maxEcopathTries) Then

                    Me.BalancedEcopathModel(iTrial, iter)

                    m_ecosim.Init(True)

                    'the ecosim time step delegate was set before the loop
                    m_ecosim.Run()

                    If Me.m_pluginmanager IsNot Nothing Then
                        Try
                            Me.m_pluginmanager.MonteCarloEcosimRunCompleted()
                        Catch ex As Exception
                            cLog.Write(ex, "cEcosimMonteCarlo::Run(" & iTrial & ")")
                        End Try
                    End If

                    'xxxxxxxxxxxxxxxxxxxx Below is for global Nereus model, June 2013 xxxxxxxxxxxxxxxxxx
                    'Calculate penalty for being away from reasonable fishing mortality
                    Fpenalty = Me.getFPenalty(bFirstRun, bForcedCatches)
                    m_esdata.SS += Fpenalty
                    'Debug.Print(Me.m_esdata.SS & " = " & Me.m_esdata.SS - Fpenalty & " + " & Fpenalty)
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                    'Only keep the best fit if there is time series loaded
                    If Me.isTimeSeriesLoaded() And (m_esdata.SS < SSBestFit) Then
                        RunsSinceLastWithLowerSS = 0
                        'SSBestFit = MCthread.ESdata.SS
                        SSBestFit = m_esdata.SS
                        Console.WriteLine("Total trials: " & iTrial.ToString & ", " & SSBestFit.ToString & ", to fit last Ecopath: " & iter.ToString) '& ", total: " & Itertot.ToString)

                        CheckWhoIsCrashed()
                        'keep the best fits for applying later
                        For igrp As Integer = 1 To m_core.nGroups
                            BestFit(eMCParams.Biomass, igrp) = m_epdata.B(igrp)
                            BestFit(eMCParams.QB, igrp) = m_epdata.QB(igrp)
                            BestFit(eMCParams.PB, igrp) = m_epdata.PB(igrp)
                            BestFit(eMCParams.EE, igrp) = m_epdata.EE(igrp)
                            BestFit(eMCParams.BA, igrp) = m_epdata.BA(igrp)
                            'vc sep 2008: adding vulnerability to MC
                            '  BestFit(eMCParams.Vulnerability, igrp) = m_esdata.VulnerabilityPredator(igrp)
                        Next

                        If RetainBiomass Then
                            Array.Copy(BestFit, Pmean, BestFit.Length)
                            'VC 2008 don't want it to stop just as it found a better fit so:
                            iTrial = Math.Min(iTrial, CInt(0.9 * Ntrials))

                        End If 'bRetainBiomass
                    End If ' m_esdata.SS < SSBestFit
                End If 'iter < maxEcopathTries 

                TrialProgress(iTrial, iter)
                EcopathIterationsProgress(iter)

                Me.m_ouputWriter.Save(False)

                ' Fire plug-in point
                If Me.m_pluginmanager IsNot Nothing Then
                    Try
                        Me.m_pluginmanager.PostRunSearchResults(Me.m_core.m_SearchData)
                    Catch ex As Exception
                        cLog.Write(ex, "cEcosimMonteCarlo::Run(" & iTrial & ")")
                    End Try
                End If
                If RunsSinceLastWithLowerSS > 2000 Then Exit For
            Next iTrial

            'restore ecopath back to its original state
            Me.restoreOriginalState()

            Me.CompletedCallback()
            If Me.m_pluginmanager IsNot Nothing Then
                Try
                    Me.m_pluginmanager.SearchCompleted(Me.m_core.m_SearchData)
                Catch ex As Exception
                    cLog.Write(ex, "cEcosimMonteCarlo::Run SearchCompleted")
                End Try
            End If

            Me.m_ecopath.suppressMessages = False

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            m_ecopath.suppressMessages = False
            Throw New ApplicationException(Me.ToString & ".Run", ex)
        End Try

        If Me.m_pluginmanager IsNot Nothing Then
            Try
                Me.m_pluginmanager.MontCarloRunCompleted()
            Catch ex As Exception
                cLog.Write(ex, "cEcosimMonteCarlo::Run MontCarloRunCompleted")
            End Try
        End If

    End Sub

    Private Sub BalancedEcopathModel(ByVal iTrial As Integer, ByVal iter As Integer)
        Dim WaitLock As ManualResetEvent = New ManualResetEvent(True)
        If Me.m_pluginmanager IsNot Nothing Then
            Try
                Me.m_pluginmanager.MonteCarloBalancedEcopathModel(WaitLock, iTrial, iter)
            Catch ex As Exception
                cLog.Write(ex, "cEcosimMonteCarlo::Run BalancedEcopathModel(" & iTrial & ", " & iter & ")")
            End Try
        End If
        WaitLock.WaitOne()
    End Sub

    Public Sub setDefaults()
        Me.EcopathEETol = EE_TOL
    End Sub


    Private Function isTimeSeriesLoaded() As Boolean
        'Number of applied time series
        Return Me.m_tsdata.NdatType > 0
    End Function

    ''' <summary>
    ''' Calculate penalty for being away from reasonable fishing mortality
    ''' </summary>
    ''' <param name="bForcedCatches"></param>
    ''' <remarks></remarks>
    Private Function getFPenalty(ByRef bFirstRun As Boolean, bForcedCatches() As Boolean) As Single
        'Used for global Nereus model, June 2013
        Dim Fpenalty As Single

        If Me.IncludeFpenalty Then
            'If Fpenalty = 0 Then FirstRun = True
            Fpenalty = 0
            Dim sStr As String = ""
            For ii As Integer = 1 To Me.m_epdata.NumGroups
                If (bForcedCatches(ii)) Then
                    Dim lasttimestep As Integer = m_esdata.NTimes
                    Dim NatMort As Single = Me.m_epdata.M0(ii) + Me.m_epdata.M2(ii)
                    Dim SScont As Single = (Me.m_esdata.FishRateNo(ii, lasttimestep) - Me.FMratioForSRA * NatMort)
                    Fpenalty += CSng(100 * SScont ^ 2)
                    sStr += ii & " " & SScont & ","
                End If
            Next

            If bFirstRun Then
                SSBestFit = SSBestFit + Fpenalty
                bFirstRun = False
            End If

            System.Console.WriteLine("SS = " + m_esdata.SS.ToString + ", F Penalty = " + Fpenalty.ToString + ", SS + Fpenalty = " + (m_esdata.SS + Fpenalty).ToString)
        End If

        Return Fpenalty
    End Function

    ''' <summary>
    ''' Restore Ecopath to its original state
    ''' </summary>
    ''' <remarks>The Monte Carlo changed the basic input data of Ecopath. This will set it back to the state it was in when the Monte Carlo was run.</remarks>
    Public Sub restoreOriginalState()
        Dim bSuccess As Boolean

        Try

            'Set Ecopath inputs back to original values
            'VC Oct 02. below was setting, b, pb, ee, ba, but it needs to set input parameters,so I've changed this
            For i As Integer = 1 To Me.m_epdata.NumLiving
                If m_epdata.Binput(i) > 0 Then m_epdata.Binput(i) = startValues(eMCParams.Biomass, i)
                If m_epdata.PBinput(i) > 0 Then m_epdata.PBinput(i) = startValues(eMCParams.PB, i)
                ' JS13feb12 added
                If m_epdata.QBinput(i) > 0 Then m_epdata.QBinput(i) = startValues(eMCParams.QB, i)
                If m_epdata.EEinput(i) > 0 Then m_epdata.EEinput(i) = startValues(eMCParams.EE, i)
                If m_epdata.OtherMortinput(i) > 0 Then m_epdata.OtherMortinput(i) = startValues(eMCParams.OtherMort, i)

                m_epdata.BA(i) = startValues(eMCParams.BA, i)
                'vc sep 2008: adding vulnerability to MC
                ' m_esdata.VulnerabilityPredator(i) = startValues(eMCParams.Vulnerability, i)
            Next

            'set vulnerabilities back 
            'Array.Copy(Me.orgVul, m_core.m_EcoSimData.VulMult, m_core.m_EcoSimData.VulMult.Length)

            'copy the data from the input parameters into the modeling parameters
            Me.m_epdata.CopyInputToModelArrays()

            'run Ecopath with the original values to reset computed variables
            bSuccess = Me.m_ecopath.Run()

            'init stanza groups back to the original values
            Me.m_ecosim.InitStanza()

            'Me.m_ecosim.Init(True)

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
            Me.SSCurrent = Me.m_core.m_EcoSimData.SS
            If dlgTrialStepHandler IsNot Nothing Then
                Me.dlgTrialStepHandler()
            End If
        Catch ex As Exception
            'Bogus Dude.....the interface has thrown an error 
            'just keep ploughing on
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub EcopathIterationsProgress(ByVal iEcopathIterations As Integer)

        Try
            Me.nEcopathIterations = iEcopathIterations

            If dlgEcopathIterationHandler IsNot Nothing Then
                dlgEcopathIterationHandler.Invoke()
            End If
        Catch ex As Exception
            'Bogus Dude.....the interface has thrown an error 
            'just keep plowing on
            cLog.Write(ex)
        End Try


    End Sub


    Private Sub CompletedCallback()
        Try
            If dlgMonteCarloCompletedHandler IsNot Nothing Then
                Me.dlgMonteCarloCompletedHandler.Invoke()
            End If
        Catch ex As Exception
            Debug.Assert(False, "Monte Carlo CompletedCallback Exception: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Wrapper around <see cref="cEcosimMonteCarlo.BalanceEcopathWithNewPars">BalanceEcopathWithNewPars</see>  
    ''' so the MonteCarloManager can expose this functionality via <see cref="cMonteCarloManager.selectNewEcopathParameters">selectNewEcopathParameters()</see>
    ''' </summary>
    ''' <param name="MaxIters">Maximum number of tries to find a balanced Ecopath Model.</param>
    ''' <returns>True if successful. False otherwise.</returns>
    ''' <remarks></remarks>
    Friend Function selectNewEcopathParameters(Optional MaxIters As Integer = 10000) As Boolean
        Try
            Dim nIters As Integer
            If BalanceEcopathWithNewPars(Pmean, CVpar, nIters, MaxIters) Then
                ''Used for debugging CEFAS MSE Plugin
                'If MaxIters > 1 Then
                '    System.Console.WriteLine("Balanced model in " + nIters.ToString)
                'End If
                Return True
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".selectNewEcopathParameters() Exception: " & ex.Message)
        End Try

        'Failed to find a balanced set of parameters within MaxIters
        'or
        'An error has been thrown some place along the line
        Return False

    End Function


    Private Sub dumpEstimatedParameters()

        System.Console.WriteLine("-------------Start Parameters Estimated by Ecopath----------------")
        For igrp As Integer = 1 To m_core.nLivingGroups
            For iPar As Integer = 1 To 4
                If m_ecopath.missing(igrp, iPar) = True Then
                    'Estimated by Ecopath
                    System.Console.WriteLine(Me.m_epdata.GroupName(igrp) + ", Index =  " + igrp.ToString + ", Parameter = " + iPar.ToString)
                End If
            Next
        Next
        System.Console.WriteLine("-------------End Parameters Estimated by Ecopath------------------")

    End Sub


    Private Function BalanceEcopathWithNewPars(ByVal ParCurVal(,) As Single, _
                                               ByVal CVpar(,) As Single, _
                                               ByRef iter As Integer, _
                                               ByVal maxEcopathIterations As Integer) As Boolean
        'EwE5 StartEcosimWithNewPars(ByVal Pstartup(,) As Single, ByVal CVpar(,) As Single, ByVal iter As Long)
        Dim igrp As Integer
        Dim bEcopathNeedsBalancing As Boolean

        Try
            'for debugging which parameters are being estimated
            'dumpEstimatedParameters()

            bEcopathNeedsBalancing = True
            Do While bEcopathNeedsBalancing
                iter = iter + 1
                m_epdata.CopyInputToModelArrays() 'MakeUnknownUnknown())

                For igrp = 1 To m_core.nLivingGroups

                    'B and BA
                    If Me.m_isVariable(igrp, eMCParams.Biomass) Then


                        m_epdata.B(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.Biomass, igrp), _
                                                             CVpar(eMCParams.Biomass, igrp), _
                                                             ParLimit(0, eMCParams.Biomass, igrp), _
                                                             ParLimit(1, eMCParams.Biomass, igrp))
                    End If ' Me.m_isVariable(igrp, eMCParams.Biomass)

                    If Me.m_isVariable(igrp, eMCParams.BA) Then
                        m_epdata.BA(igrp) = ChooseFeasibleBA(m_epdata.B(igrp), _
                                                             ParCurVal(eMCParams.BA, igrp), _
                                                             CVpar(eMCParams.BA, igrp), _
                                                             ParLimit(0, eMCParams.BA, igrp), _
                                                             ParLimit(1, eMCParams.BA, igrp))
                    End If 'Me.m_isVariable(igrp, eMCParams.BA)

                    'PB
                    If Me.m_isVariable(igrp, eMCParams.PB) Then
                        m_epdata.PB(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.PB, igrp), _
                                                              CVpar(eMCParams.PB, igrp), _
                                                              ParLimit(0, eMCParams.PB, igrp), _
                                                              ParLimit(1, eMCParams.PB, igrp))
                    End If

                    If Me.m_isVariable(igrp, eMCParams.QB) Then
                        m_epdata.QB(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.QB, igrp), _
                                                              CVpar(eMCParams.QB, igrp), _
                                                              ParLimit(0, eMCParams.QB, igrp), _
                                                              ParLimit(1, eMCParams.QB, igrp))
                    End If
                    'EE
                    If Me.m_isVariable(igrp, eMCParams.EE) Then
                        m_epdata.EE(igrp) = ChooseFeasiblePar(ParCurVal(eMCParams.EE, igrp), _
                                                              CVpar(4, igrp), _
                                                              ParLimit(0, eMCParams.EE, igrp), _
                                                              ParLimit(1, eMCParams.EE, igrp))
                    End If
                Next igrp

                m_ecosim.InitStanza()

                'For debugging
                'dumpEcopathPars()

                'Estimate basic params
                If Not m_ecopath.Run() Then

                    ' ''Failed to estimate parameters
                    Dim status As eStatusFlags = m_ecopath.EstimationStatus
                    Dim msg As cMessage
                    If status = eStatusFlags.MissingParameter Then
                        msg = New cMessage(My.Resources.CoreMessages.MONTECARLO_ECOPATH_TOOMANYMISSING, eMessageType.TooManyMissingParameters, eCoreComponentType.EcoSim, eMessageImportance.Critical)
                    Else
                        msg = New cMessage(My.Resources.CoreMessages.MONTECARLO_ECOPATH_ERROR, eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Critical)
                    End If
                    ' m_manager.AddMessage(msg)
                    'Return False
                End If

                m_ecopath.DetritusCalculations()

                bEcopathNeedsBalancing = False
                For igrp = 1 To m_core.nGroups
                    If m_epdata.EE(igrp) > 1.0 + Me.EcopathEETol Or m_epdata.EE(igrp) < 0 Then
                        'this loop did not balance Ecopath
                        bEcopathNeedsBalancing = True
                        Exit For
                    End If
                Next

                'tell the interface
                'EcopathIterationsProgress(iter)

                If StopTrial = True Then Exit Do

                If iter >= maxEcopathIterations Then
                    'max number of iteration to find balanced ecopath model
                    'Exit the Do Loop
                    Exit Do
                End If

            Loop

        Catch ex As Exception
            Debug.Assert(False, ex.StackTrace)
            cLog.Write(ex)
            Throw New ApplicationException(Me.ToString & ".BalanceEcopathWithNewPars()", ex)
        End Try

        'bEcopathNeedsBalancing will be False if a balanced model was found(does not need balancing)
        'True if not balanced(the model does need balancing)
        'BalanceEcopathWithNewPars() will return True if the model was balanced, the opposite of bEcopathNeedsBalancing
        Return Not bEcopathNeedsBalancing

    End Function


    Private Function isStanzaGroupVariable(igrp As Integer, varType As eMCParams) As Boolean

        'Not a multistanza group so OK to vary
        If Not m_epdata.StanzaGroup(igrp) Then Return True

        'Optimistic this group can be varied
        Dim bReturn As Boolean = True
        Select Case varType

            Case eMCParams.BA
                'BA is never variable for Stanza groups
                If varType = eMCParams.BA Then bReturn = False

            Case eMCParams.Biomass
                'For B and QB only the leading group can be varied
                If Not Me.m_epdata.isGroupLeadingB(igrp) Then bReturn = False

            Case eMCParams.QB
                'For B and QB only the leading group can be varied
                If Not Me.m_epdata.isGroupLeadingCB(igrp) Then bReturn = False

        End Select

        Return bReturn

    End Function

    Private Sub dumpEcopathPars()
        Try
            Dim strm As New System.IO.StreamWriter("EcopathPars.csv", True)
            strm.WriteLine("iter")
            For igrp As Integer = 1 To Me.m_epdata.NumGroups
                strm.WriteLine(EwEUtils.Utilities.cStringUtils.ToCSVField(Me.m_epdata.GroupName(igrp)) + "," + Me.m_epdata.B(igrp).ToString + "," + Me.m_epdata.PB(igrp).ToString + "," + Me.m_epdata.QB(igrp).ToString + "," + Me.m_epdata.EE(igrp).ToString)
            Next
            strm.Close()
        Catch ex As Exception

        End Try
    End Sub

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

            ' JS13feb12 added
            If m_ecopath.missing(iPred, 3) = False Then
                m_epdata.QBinput(iPred) = BestFit(eMCParams.QB, iPred)
            End If

            If m_ecopath.missing(iPred, 4) = False Then
                m_epdata.EEinput(iPred) = BestFit(eMCParams.EE, iPred)
            End If

            m_epdata.BA(iPred) = BestFit(eMCParams.BA, iPred)


            'vc sep 2008: adding vulnerability to MC
            'm_esdata.VulnerabilityPredator(iPred) = BestFit(eMCParams.Vulnerability, iPred)
            'Also transfer to vulmult
            'For iPrey As Integer = 1 To m_core.nGroups
            '    m_esdata.VulMult(iPrey, iPred) = BestFit(eMCParams.Vulnerability, iPred)
            '    'jb this is done by the manager in ApplyBestFits core.onChanged() 
            '    m_core.EcoSimGroupInputs(iPrey).VulMult(iPred) = BestFit(eMCParams.Vulnerability, iPred)
            'Next


            'ToDo_jb cEcosimMonteCarlo.Run something is wrong here
            'I don't have a BAinput BA will contain the best fit parameters
            ' m_epdata.BAinput(i) = m_epdata.BA(i)
            '    optVary_Click(0)
        Next

    End Sub

    Private Function NumParams() As Integer
        ' Do not include 'not set' (thus not redim by length  + 1)
        Return [Enum].GetValues(GetType(eMCParams)).Length
    End Function

    Private Sub redimVariables()
        Try

            ReDim ParLimit(1, 6, m_core.nGroups)
            ReDim CVpar(Me.NumParams, m_core.nGroups)

            For i As Integer = 1 To m_core.nGroups
                For ivar As Integer = 1 To Me.NumParams
                    'Only set the default CV if the parameter is variable
                    If Me.m_isVariable(i, ivar) Then
                        CVpar(ivar, i) = 0.1
                        If ivar = CInt(eMCParams.BA) Then
                            'BA gets a different default CV
                            CVpar(ivar, i) = 0.05
                        End If
                    End If
                Next ivar
            Next i

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".redimVariables()", ex)
        End Try


    End Sub


    ''' <summary>
    ''' Calculte the Upper and Lower Parameter limits from CV values
    ''' </summary>
    ''' <param name="IsCrashEvaluated">Not USED!</param>
    ''' <remarks>Called once during initialization to set default values or when CV values have been edited</remarks>
    Public Sub CalculateUpperLowerLimits(ByVal IsCrashEvaluated As Boolean)

        Dim i As Integer
        Try
            'jb set the Upper and Lower Limits to 2*CV
            Dim factor As Integer = 2

            'We want a wide range for searching, cv will still limit the steps
            For i = 1 To m_core.nLivingGroups

                'Lower
                ParLimit(0, eMCParams.Biomass, i) = Me.m_epdata.B(i) * (1 - factor * CVpar(eMCParams.Biomass, i))
                If ParLimit(0, eMCParams.Biomass, i) < 0 Then ParLimit(0, eMCParams.Biomass, i) = 1.0E-10!

                ParLimit(0, eMCParams.PB, i) = Me.m_epdata.PB(i) * (1 - factor * CVpar(eMCParams.PB, i))
                If ParLimit(0, eMCParams.PB, i) < 0 Then ParLimit(0, eMCParams.PB, i) = 1.0E-10!

                ParLimit(0, eMCParams.QB, i) = Me.m_epdata.QB(i) * (1 - factor * CVpar(eMCParams.QB, i))
                If ParLimit(0, eMCParams.QB, i) < 0 Then ParLimit(0, eMCParams.QB, i) = 1.0E-10!

                ParLimit(0, eMCParams.EE, i) = Me.m_epdata.EE(i) * (1 - factor * CVpar(eMCParams.EE, i))
                If ParLimit(0, eMCParams.EE, i) < 0 Then ParLimit(0, eMCParams.EE, i) = 0

                'BA is +- relative to B not to BA (which is usually zero)
                ParLimit(0, eMCParams.BA, i) = Me.m_epdata.BA(i) + Me.m_epdata.B(i) * (-factor * CVpar(eMCParams.BA, i))
                'Vul is from 1 up
                '  ParLimit(0, eMCParams.Vulnerability, i) = m_esdata.VulnerabilityPredator(i) * (1 - factor * CVpar(eMCParams.Vulnerability, i)) : If ParLimit(0, eMCParams.Vulnerability, i) < 1.01 Then ParLimit(0, eMCParams.Vulnerability, i) = 1.01

                'upper
                ParLimit(1, eMCParams.Biomass, i) = Me.m_epdata.B(i) * (1 + factor * CVpar(eMCParams.Biomass, i))
                If ParLimit(1, eMCParams.Biomass, i) < ParLimit(0, eMCParams.Biomass, i) Then ParLimit(1, eMCParams.Biomass, i) = 10 * ParLimit(0, eMCParams.Biomass, i)

                ParLimit(1, eMCParams.PB, i) = Me.m_epdata.PB(i) * (1 + factor * CVpar(eMCParams.PB, i))
                If ParLimit(1, eMCParams.PB, i) < ParLimit(0, eMCParams.PB, i) Then ParLimit(1, eMCParams.PB, i) = 10 * ParLimit(0, eMCParams.PB, i)

                ParLimit(1, eMCParams.QB, i) = Me.m_epdata.QB(i) * (1 + factor * CVpar(eMCParams.QB, i))
                If ParLimit(1, eMCParams.QB, i) < ParLimit(0, eMCParams.QB, i) Then ParLimit(1, eMCParams.QB, i) = 10 * ParLimit(0, eMCParams.QB, i)

                ParLimit(1, eMCParams.EE, i) = Me.m_epdata.EE(i) * (1 + factor * CVpar(eMCParams.EE, i))
                If ParLimit(1, eMCParams.EE, i) > 1 Then ParLimit(1, eMCParams.EE, i) = 1

                'BA is +- relative to B not to BA (which is usually zero)
                ParLimit(1, eMCParams.BA, i) = m_epdata.BA(i) + m_epdata.B(i) * (factor * CVpar(eMCParams.BA, i))
                ' ParLimit(1, eMCParams.Vulnerability, i) = 1000 ' m_esdata.VulnerabilityPredator(i) * (1 + factor * CVpar(eMCParams.Vulnerability, i)) 'no upper limit for vulmult : If ParLimit(1, eMCParams.Vulnerability, i) > 1 Then ParLimit(1, eMCParams.Vulnerability, i) = 1

            Next

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".Run", ex)
        End Try


    End Sub

    Private Function ChooseFeasiblePar(ByVal xbar As Single, ByVal CV As Single, ByVal ParMin As Single, ByVal ParMax As Single) As Single
        Dim X As Single, ict As Integer
        Do
            'jb 7-Dec-2010 ChooseFeasiblePar() changed application of CV 
            ' X = xbar * (1 + 0.02 * CV * RandomNormal())
            X = xbar * (1 + CV * RandomNormal())

            If X >= ParMin And X <= ParMax Then
                ' Debug.Assert(X = xbar)
                Return X
            End If
            ict = ict + 1
            If ict > 10000 Then
                'If Answer <> vbCancel Then
                '    Answer = MsgBox("Can't find acceptable parameter, using mean", vbOKCancel)
                'End If
                System.Console.WriteLine("ChooseFeasiblePar() Can't find acceptable parameter, using mean")
                Return xbar
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
        For i = 1 To 12 : X = X + CSng(Me.m_rand.NextDouble()) : Next
        Return X
    End Function

    'Private Sub ChangeVulnerabilities(ByVal ParCurVal(,) As Single, ByVal CVpar(,) As Single)

    '    For iPred As Integer = 1 To m_core.nLivingGroups
    '        m_esdata.VulnerabilityPredator(iPred) = ChooseFeasiblePar(ParCurVal(eMCParams.Vulnerability, iPred), _
    '                                                                 CVpar(6, iPred), _
    '                                                                 ParLimit(0, eMCParams.Vulnerability, iPred), _
    '                                                                 ParLimit(1, eMCParams.Vulnerability, iPred), _
    '                                                                 False)
    '        For iPrey As Integer = 1 To m_core.nGroups
    '            m_esdata.VulMult(iPrey, iPred) = m_esdata.VulnerabilityPredator(iPred)
    '        Next
    '    Next

    'End Sub

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
