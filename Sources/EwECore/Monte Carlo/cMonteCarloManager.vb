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
Imports System
Imports System.Threading
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Manager to run the ecosim monte carlo object
''' </summary>
Public Class cMonteCarloManager
    Inherits cThreadWaitBase
    Implements ICoreInterface

    Private Delegate Sub dlgSendMessages()

    'ToDo_jb: cMonteCarloManager FisForce flag in EwE5 the "Retain current Ecosim fishing rate pattern" check box sets fisforce to true for all groups
    'this never gets set back to the value computed in DoDatValCalculations. It should be able to reset fisforce() by calling the EwE6 equivalent of DoDatValCalculations when False

#Region "Private variables"

    Private m_lstGrps As List(Of cMonteCarloGroup)
    Private m_core As cCore
    Private m_mc As cEcosimMonteCarlo

    'Synchronization object from the user interface that handles the passing of data from the model thread to the user interface thread
    Private m_SyncObject As System.ComponentModel.ISynchronizeInvoke


    'Time step handler for ecosim 
    Private m_EcosimTimeStepHandler As EcoSimTimeStepDelegate

    'Delegates supplied by the interface to call in responce to an Monte Carlo delegate
    Private m_dlgMCCompletedHandler As MonteCarloCompletedDelegate
    Private m_dlgMCEcopathStepHandler As MonteCarloEcopathProgressDelegate
    Private m_dlgMCTrialStepHandler As MonteCarloTrialProgressDelegate

    Private m_lstMessages As New List(Of cMessage)

    'Private m_MCCallback As MonteCarloTrialDelegate
    ' Private m_EcoPathCallback As MonteCarloEcopathDelegate

    Private m_bPlot As Boolean
    '  Private m_isRunning As Boolean
    Private m_UseFishingPattern As Boolean


    'for ICoreInterface
    Private m_dbid As Integer
    Private m_index As Integer
    Private m_name As String

#End Region

#Region "Construction and initialization"


    Friend Sub New()


    End Sub


    Friend Sub init(ByRef theCore As cCore)

        Try
            m_core = theCore

            m_mc = New cEcosimMonteCarlo(m_core)
            'set all the delegates to handle events/messages from the monte carlo
            m_mc.dlgMonteCarloCompletedHandler = AddressOf Me.MCCompletedHandler
            m_mc.dlgEcopathIterationHandler = AddressOf Me.MCEcopathInterationHandler
            m_mc.dlgTrialStepHandler = AddressOf Me.MCTrialProgressHandler
            m_mc.dlgMonteCarloMessageHandler = AddressOf Me.MCSendMessageHandler

            m_mc.Init()

            InitGroups()
            LoadGroups()

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".init()", ex)
        End Try


    End Sub


    Public Sub Clear()
        Try
            If Me.m_lstGrps Is Nothing Then Exit Sub
            For Each MCgrp As cMonteCarloGroup In Me.m_lstGrps
                MCgrp.Clear()
            Next
            Me.m_lstGrps.Clear()
            Me.m_lstGrps = Nothing

            Me.m_mc.dlgMonteCarloCompletedHandler = Nothing
            Me.m_mc.dlgEcopathIterationHandler = Nothing
            Me.m_mc.dlgTrialStepHandler = Nothing
            Me.m_mc.dlgMonteCarloMessageHandler = Nothing

            Me.m_mc.Clear()
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".Clear() Exception: " & ex.Message)
        End Try

    End Sub

    Public Sub setDefaultTol()
        Try
            Me.m_mc.setDefaults()
        Catch ex As Exception
            Debug.Assert(False, "setDefaultTol() Exception: " & ex.Message)
        End Try
    End Sub


#End Region

#Region "Running"

    Public Overrides Sub SetWait()
        Me.m_core.m_SearchData.SearchMode = eSearchModes.MonteCarlo
        MyBase.SetWait()
    End Sub

    Public Overrides Sub ReleaseWait()
        Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
        MyBase.ReleaseWait()
    End Sub

    ''' <summary>
    ''' Run the Monte Carlo trials with the current parameters
    ''' </summary>
    Public Sub Run()
        Dim thrdMC As Thread

        Try
            If m_core.StateMonitor.HasEcosimLoaded Then

                Me.SetWait()
                Me.update()

                thrdMC = New Thread(AddressOf m_mc.Run)
                thrdMC.Start()

            Else 'If m_core.StateMonitor.HasEcosimLoaded Then

                'no ecosim scenario loaded
                m_core.Messages.SendMessage(New cMessage(My.Resources.CoreMessages.MONTECARLO_ECOSIM_MISSING, eMessageType.StateNotMet, eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Warning, eDataTypes.MonteCarlo))

            End If

        Catch ex As Exception
            cLog.Write(ex)
            Me.ReleaseWait()
            m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.MONTECARLO_RUN_ERROR, ex.Message), _
                                                     eMessageType.ErrorEncountered, eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Critical, eDataTypes.MonteCarlo))
        End Try

        m_core.Messages.sendAllMessages()

        Return

    End Sub

    ''' <summary>
    ''' Load the current data into the MonteCarlo parameters
    ''' </summary>
    Public Sub Load()
        m_mc.Init()
        m_mc.initForRun()
    End Sub

#End Region

#Region "Delegates called by the Monte Carlo class"

    ''' <summary>
    ''' The Monte Carlo routine has complete its trials. Load the best fitting data into the interace objects and tell the interface that the trials have completed
    ''' </summary>
    Private Sub MCCompletedHandler()

        Try

            'reload the groups
            Me.LoadGroups()

            'set the signaled state 
            'Release any waiting threads
            Me.ReleaseWait()

            'send all the messages that the MonteCarlo model added to the manager via the Syncronization object (m_SyncObject)
            'this way the messages are sent on the interfaces thread not the models
            Dim dlgsendmsgs As dlgSendMessages = AddressOf Me.sendmessages
            m_SyncObject.BeginInvoke(dlgsendmsgs, Nothing)

            'tell the interface
            If m_SyncObject IsNot Nothing And m_dlgMCCompletedHandler IsNot Nothing Then
                'use the SyncObject provided by the interface to call the completed handler in the interface
                m_SyncObject.BeginInvoke(m_dlgMCCompletedHandler, Nothing)
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Me.ReleaseWait()
        End Try

    End Sub

    ''' <summary>
    ''' Send all the messages in the managers list of messages by adding them to the cores message publisher
    ''' </summary>
    ''' <remarks>This has to be marshalled to the interface/core thread via m_syncObject.BeginInvoke()</remarks>
    Private Sub sendmessages()

        Try
            'send any messages created by the monte carlo
            For Each msg As cMessage In m_lstMessages
                m_core.Messages.AddMessage(msg)
            Next
            m_core.Messages.sendAllMessages()
            m_lstMessages.Clear()
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException(Me.ToString & ".sendmessage()", ex)
        End Try

    End Sub


    Private Sub MCEcopathInterationHandler()

        Try

            'make the interface has setup the manager properly
            'Debug.Assert(m_SyncObject IsNot Nothing)
            'Debug.Assert(Me.m_dlgMCEcopathStepHandler IsNot Nothing)

            'tell the interface
            If m_SyncObject IsNot Nothing And m_dlgMCEcopathStepHandler IsNot Nothing Then
                'use the SyncObject provided by the interface to call the completed handler in the interface
                m_SyncObject.BeginInvoke(m_dlgMCEcopathStepHandler, Nothing)
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub MCTrialProgressHandler()

        Try


            'tell the interface
            If m_SyncObject IsNot Nothing And m_dlgMCTrialStepHandler IsNot Nothing Then
                'use the SyncObject provided by the interface to call the completed handler in the interface
                m_SyncObject.Invoke(m_dlgMCTrialStepHandler, Nothing)
            End If
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".MCTrialProgressHandler() " & ex.Message)
        End Try


    End Sub

    ''' <summary>
    ''' Add a message to the managers list of messages. These messages will be sent at the end of the Monte Carlo run.
    ''' </summary>
    ''' <param name="theMessage"></param>
    ''' <remarks>This sub has the same signature as cEcosimMonteCarlo.MonteCarloSendMessageDelegate(). 
    ''' The Monte Carlo model uses it to send messages</remarks>
    Private Sub MCSendMessageHandler(ByRef theMessage As cMessage)

        Try
            Me.m_lstMessages.Add(theMessage)
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub


#End Region

#Region "Saving"


    ''' <summary>
    ''' Apply the Monte Carlo results (best fitting parameters) to the Ecopath inputs (B,PB....)
    ''' </summary>
    Public Sub ApplyBestFits()

        Try

            m_mc.ApplyBestFits()

            '#Hack: Tell the core that Ecopath inputs have changed
            '       cCore.OnChanged(me) does not support the granularity to invalidate Ecopath data in response to only this event
            m_core.DataSource.SetChanged(eCoreComponentType.EcoPath)

            'tell the core to reload groups from modified Ecopath inputs
            m_core.onChanged(Me, eMessageType.DataModified)

            Me.LoadGroups()

            'run ecopath with the new parameters
            m_core.RunEcoPath()
            'initialize ecosim with the new data
            m_core.m_EcoSim.Init(True)

            m_core.RunEcoSim()
            Dim ss As Single = m_core.EcosimStats.SS

        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex)
            m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.MONTECARLO_APPLY_ERROR, ex.Message), _
                                                     eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Critical))
        End Try

    End Sub

#End Region

#Region "Public Properties and Methods"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a message the the managers list of messages generated by the monte carlo routine
    ''' </summary>
    ''' <param name="theMessage">The <see cref="cMessage"/> to add.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub AddMessage(ByRef theMessage As cMessage)
        Try
            m_lstMessages.Add(theMessage)
        Catch ex As Exception
            Debug.Assert(False, "AddMessage error " & ex.Message)
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stop the current Monte Carlo trials
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function StopRun(Optional ByVal WaitTimeInMillSec As Integer = -1) As Boolean
        Dim result As Boolean = True

        If (Me.m_core Is Nothing) Then Return result
        If (Me.m_mc Is Nothing) Then Return result

        Try
            m_mc.StopTrial = True
            Me.m_core.StopEcoSim()

            result = Me.Wait(WaitTimeInMillSec)
        Catch ex As Exception

        End Try

        Return result

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of trials.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property nTrials() As Integer
        Get
            If (Me.m_mc Is Nothing) Then Return 0
            Return Me.m_mc.Ntrials
        End Get
        Set(ByVal value As Integer)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.Ntrials = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether to better fitting estimates (use trials to search)
    ''' </summary>
    ''' <remarks>
    ''' Flag copied from EwE5
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property RetainFits() As Boolean
        Get
            If (Me.m_mc Is Nothing) Then Return False
            Return m_mc.RetainBiomass
        End Get
        Set(ByVal value As Boolean)
            If (Me.m_mc IsNot Nothing) Then
                If value Then
                    'EwE5 code
                    'this is saying if NO time series data is loaded then set FisForced to true 
                    'this is impossible
                    'If Check1.value = Checked And NdatType = 0 Then
                    '    For i = 1 To NumGroups
                    '        FisForced(i) = True
                    '    Next
                    'End If
                Else
                    'set FisForced back to its original value
                End If
                Me.m_mc.RetainBiomass = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether to include SRA for groups with forced catches.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property IncludeFpenalty() As Boolean
        Get
            If (Me.m_mc Is Nothing) Then Return False
            Return Me.m_mc.IncludeFpenalty
        End Get
        Set(ByVal value As Boolean)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.IncludeFpenalty = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the F/M ratio for SRA.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FMRatioForSRA As Single
        Get
            If (Me.m_mc Is Nothing) Then Return cCore.NULL_VALUE
            Return Me.m_mc.FMratioForSRA
        End Get
        Set(value As Single)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.FMratioForSRA = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether to retain EwE5 current Ecosim fishing rate patterns.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseFishingPattern() As Boolean
        Get
            Throw New NotImplementedException("MonteCarlo UseFishingPattern Not implemented yet")
            Return False
        End Get
        Set(ByVal value As Boolean)
            Throw New NotImplementedException("MonteCarlo UseFishingPattern Not implemented yet")
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Sum of Squares fit to the currently loaded reference data for 
    ''' the current trial.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SS() As Single
        Get
            'Sum of Squares fit to the currently loaded reference data 
            'compute by Ecosim into its cEcosimDatastructures object for each trial
            If (Me.m_mc Is Nothing) Then Return cCore.NULL_VALUE
            Return m_core.m_EcoSimData.SS
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Sum of Squares, fit to the currently loaded reference data for 
    ''' the original ecopath parameters.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SSorg() As Single
        Get
            'Sum of Squares fit to the currently loaded reference data 
            If (Me.m_mc Is Nothing) Then Return cCore.NULL_VALUE
            Return m_mc.SSorg
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the best fitting Sum of Squares to the currently loaded reference 
    ''' data for all the trials run to date.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SSBestFit() As Single
        Get
            'Sum of Squares fit to the currently loaded reference data 
            If (Me.m_mc Is Nothing) Then Return cCore.NULL_VALUE
            Return m_mc.SSBestFit
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of attempts at finding a balanced Ecopath model for
    ''' the current trial.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nEcopathIterations() As Single
        Get
            If (Me.m_mc Is Nothing) Then Return 0
            Return m_mc.nEcopathIterations
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of trials performed in the currently running simulation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nTrialIterations() As Single
        Get
            If (Me.m_mc Is Nothing) Then Return 0
            Return Me.m_mc.nTrialIterations
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the <see cref="MonteCarloEcopathProgressDelegate">delegate</see> to 
    ''' call at each attempt to find a balanced Ecopath model.
    ''' </summary>
    ''' <remarks>Call to update an interface when Ecopath has been run</remarks>
    ''' -----------------------------------------------------------------------
    Public WriteOnly Property MonteCarloEcopathStepHandler() As MonteCarloEcopathProgressDelegate
        Set(ByVal value As MonteCarloEcopathProgressDelegate)
            Me.m_dlgMCEcopathStepHandler = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the <see cref="MonteCarloTrialProgressDelegate"/> to call at the completion of each Monte Carlo trial.
    ''' </summary>
    ''' <remarks>This delegate is supplied by a user interface and will be called 
    ''' by the Monte Carlo routines at the end of each Monte Carlo trial.
    ''' It will tell an interface that a single trial has completed. </remarks>
    ''' -----------------------------------------------------------------------
    Public WriteOnly Property MonteCarloStepHandler() As MonteCarloTrialProgressDelegate
        Set(ByVal value As MonteCarloTrialProgressDelegate)
            Me.m_dlgMCTrialStepHandler = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the method to call in the interface when the Monte Carlo trials have completed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public WriteOnly Property MonteCarloCompletedHandler() As MonteCarloCompletedDelegate
        Set(ByVal value As MonteCarloCompletedDelegate)
            'the Monte Carlo object will call the manger and the manager will call the interface with this delegate
            'see MCCompletedHandler()
            Me.m_dlgMCCompletedHandler = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the <see cref="System.ComponentModel.ISynchronizeInvoke">Synchronization object</see>, which can be
    ''' a Windows.Forms.Control, used for calling all the delegates across threads
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public WriteOnly Property SyncObject() As System.ComponentModel.ISynchronizeInvoke
        Set(ByVal value As System.ComponentModel.ISynchronizeInvoke)
            m_SyncObject = value
        End Set
    End Property

    Public WriteOnly Property EcosimTimeStepHandler() As EcoSimTimeStepDelegate
        Set(ByVal value As EcoSimTimeStepDelegate)
            Debug.Assert(Me.m_mc IsNot Nothing)
            'save the delegate for use with the bShowPlot flag
            '  m_EcosimTimeStepHandler = value
            'Changed this to pass the delegate directly to the monte carlo model
            'it will decide when to turn the plotting on or off
            m_mc.EcosimTimeStep = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the max. number of iterations that Monte Carlo will perform.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MaxEcoPathInterations() As Integer
        Get
            'ToDo_jb montecarlo manager this should come from the monte carlo model
            Return 2000
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cMonteCarloGroup"/> for a given index.
    ''' </summary>
    ''' <param name="iGroup">The one-based group index to obtain the group for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Groups(ByVal iGroup As Integer) As cMonteCarloGroup
        Get
            Return m_lstGrps.Item(iGroup - 1)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Monte Carlo should automatically save trial outputs.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property IsSaveOutput() As Boolean
        Get
            If (Me.m_mc Is Nothing) Then Return False
            Return Me.m_mc.bSaveOutput
        End Get
        Set(ByVal value As Boolean)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.bSaveOutput = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load CV values from pedigree for a given variable.
    ''' </summary>
    ''' <param name="var">The <see cref="eVarNameFlags">variable</see> to load 
    ''' CV values for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub LoadFromPedigree(var As eVarNameFlags)
        Try
            If (Me.m_mc Is Nothing) Then Return
            If Me.m_mc.LoadFromPedigree(var) Then
                Me.m_mc.CalculateUpperLowerLimits(False)
                Me.LoadGroups()
                Me.m_core.onChanged(Me, eMessageType.DataModified)
            End If
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set a tolerance for EE estimates if the default mass-balance constraint 
    ''' of [0, 1] proves too strict.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EcopathEETolerance() As Single
        Get
            If (Me.m_mc Is Nothing) Then Return cCore.NULL_VALUE
            Return Me.m_mc.EcopathEETol
        End Get
        Set(ByVal value As Single)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.EcopathEETol = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Initialize the random sequence generator to a new seed.
    ''' </summary>
    ''' <param name="seed"></param>
    ''' <remarks>This can be used to generate the same sequence of random numbers for each run. This can be useful for debugging. </remarks>
    Public Sub InitRandomSequence(seed As Integer)
        Debug.Assert(Me.m_mc IsNot Nothing)
        Me.m_mc.initRandomSequence(seed)
    End Sub
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Yippee.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ShowBiomassTrajectories() As Boolean
        Get
            If (Me.m_mc Is Nothing) Then Return False
            Return Me.m_mc.bShowPlot
        End Get
        Set(ByVal value As Boolean)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.bShowPlot = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Select a new set of Ecopath parameters using  CV, Mean, Max and Min set in <see cref="cMonteCarloGroup">cMonteCarloGroup</see>
    ''' </summary>
    ''' <param name="MaxEcopathIteration">Maximum number of tries to find a balanced Ecopath Model.</param>
    ''' <returns>True if a balanced Ecopath model was found within MaxEcopathIteration. False otherwise. </returns>
    ''' <remarks>This functionality was added to simplify external process that want to run there own Monte Carlo style models. </remarks>
    ''' -----------------------------------------------------------------------
    Public Function selectNewEcopathParameters(Optional MaxEcopathIteration As Integer = 10000) As Boolean
        Try
            Debug.Assert(Me.m_mc IsNot Nothing)
            'force the interface objects to update the underlying data
            Me.update()
            If Me.m_mc.selectNewEcopathParameters(MaxEcopathIteration) Then
                'BalanceEcopathWithNewPars() updated the core arrays 
                'Now load the new values into the MonteCarloManagers Input/Output objects
                'Core.EcoPathGroupInputs and Core.EcoPathGroupOutputs have NOT been update and will not contain the latest values
                'For now this is messing up the way the model re-initializes so remove it...
                'Me.LoadGroups()

                Return True
            End If

        Catch ex As Exception
            cLog.Write(ex.Message)
            Debug.Assert(False, Me.ToString & ".selectNewEcopathParameters(MaxIteration): Exception: " & ex.Message)
        End Try

        'selectNewEcopathParameters has either thrown an error
        'or failed to find a balanced Ecopath model
        'in either case return false
        Return False

    End Function

    Public Function RestoreOriginalValues() As Boolean
        Debug.Assert(Me.m_mc IsNot Nothing)
        Me.m_mc.restoreOriginalState()
        Return True
    End Function

    Public Sub SaveOriginalValues()
        Debug.Assert(Me.m_mc IsNot Nothing)
        Me.m_mc.initForRun()
    End Sub
#End Region

#Region "Private methods"

    Friend Sub CalculateUpperLowerLimits()

        Try
            Me.update()
            Me.m_mc.CalculateUpperLowerLimits(False)
            Me.LoadGroups()
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Friend Sub LoadGroups()

        Try
            Dim m_epdata As cEcopathDataStructures = m_core.m_EcoPathData
            Dim m_esdata As cEcosimDatastructures = m_core.m_EcoSimData
            Dim iIndex As Integer = 0

            For Each grp As cMonteCarloGroup In m_lstGrps

                grp.AllowValidation = False

                'convert the Database ID into an iGroup
                iIndex = Array.IndexOf(m_epdata.GroupDBID, grp.DBID)
                grp.Index = iIndex
                grp.Resize()

                grp.Name = m_epdata.GroupName(grp.Index)

                'data from Ecopath
                grp.B = m_epdata.B(iIndex)
                grp.PB = m_epdata.PB(iIndex)
                grp.QB = m_epdata.QB(iIndex)
                grp.BA = m_epdata.BA(iIndex)
                grp.EE = m_epdata.EE(iIndex)
                grp.VU = m_esdata.VulnerabilityPredator(iIndex)

                grp.Bcv = m_mc.CVpar(eMCParams.Biomass, iIndex)
                grp.PBcv = m_mc.CVpar(eMCParams.PB, iIndex)
                grp.QBcv = m_mc.CVpar(eMCParams.QB, iIndex)
                grp.BAcv = m_mc.CVpar(eMCParams.BA, iIndex)
                grp.EEcv = m_mc.CVpar(eMCParams.EE, iIndex)
                grp.VUcv = m_mc.CVpar(eMCParams.Vulnerability, iIndex)

                grp.BLower = m_mc.ParLimit(0, eMCParams.Biomass, iIndex)
                grp.PBLower = m_mc.ParLimit(0, eMCParams.PB, iIndex)
                grp.QBLower = m_mc.ParLimit(0, eMCParams.QB, iIndex)
                grp.BALower = m_mc.ParLimit(0, eMCParams.BA, iIndex)
                grp.EELower = m_mc.ParLimit(0, eMCParams.EE, iIndex)
                grp.VULower = m_mc.ParLimit(0, eMCParams.Vulnerability, iIndex)

                grp.BUpper = m_mc.ParLimit(1, eMCParams.Biomass, iIndex)
                grp.PBUpper = m_mc.ParLimit(1, eMCParams.PB, iIndex)
                grp.QBUpper = m_mc.ParLimit(1, eMCParams.QB, iIndex)
                grp.BAUpper = m_mc.ParLimit(1, eMCParams.BA, iIndex)
                grp.EEUpper = m_mc.ParLimit(1, eMCParams.EE, iIndex)
                grp.VUUpper = m_mc.ParLimit(1, eMCParams.Vulnerability, iIndex)

                'best fit data from the monte carlo trials, if any
                grp.Bbf = m_mc.BestFit(eMCParams.Biomass, iIndex)
                grp.PBbf = m_mc.BestFit(eMCParams.PB, iIndex)
                grp.QBbf = m_mc.BestFit(eMCParams.QB, iIndex)
                grp.BAbf = m_mc.BestFit(eMCParams.BA, iIndex)
                grp.EEbf = m_mc.BestFit(eMCParams.EE, iIndex)
                grp.VUbf = m_mc.BestFit(eMCParams.Vulnerability, iIndex)

                grp.ResetStatusFlags()

                Dim grpPath As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iIndex)

                ' B
                grp.SetStatusFlags(eVarNameFlags.mcB, Me.ToMCStatus(grpPath, eVarNameFlags.BiomassAreaInput))
                grp.SetStatusFlags(eVarNameFlags.mcBcv, Me.ToMCStatus(grpPath, eVarNameFlags.BiomassAreaInput))
                grp.SetStatusFlags(eVarNameFlags.mcBLower, Me.ToMCStatus(grpPath, eVarNameFlags.BiomassAreaInput))
                grp.SetStatusFlags(eVarNameFlags.mcBUpper, Me.ToMCStatus(grpPath, eVarNameFlags.BiomassAreaInput))
                grp.SetStatusFlags(eVarNameFlags.mcBbf, Me.ToMCStatus(grpPath, eVarNameFlags.BiomassAreaInput, True))

                ' PB
                grp.SetStatusFlags(eVarNameFlags.mcPB, Me.ToMCStatus(grpPath, eVarNameFlags.PBInput))
                grp.SetStatusFlags(eVarNameFlags.mcPBcv, Me.ToMCStatus(grpPath, eVarNameFlags.PBInput))
                grp.SetStatusFlags(eVarNameFlags.mcPBLower, Me.ToMCStatus(grpPath, eVarNameFlags.PBInput))
                grp.SetStatusFlags(eVarNameFlags.mcPBUpper, Me.ToMCStatus(grpPath, eVarNameFlags.PBInput))
                grp.SetStatusFlags(eVarNameFlags.mcPBbf, Me.ToMCStatus(grpPath, eVarNameFlags.PBInput, True))

                ' QB
                grp.SetStatusFlags(eVarNameFlags.mcQB, Me.ToMCStatus(grpPath, eVarNameFlags.QBInput))
                grp.SetStatusFlags(eVarNameFlags.mcQBcv, Me.ToMCStatus(grpPath, eVarNameFlags.QBInput))
                grp.SetStatusFlags(eVarNameFlags.mcQBLower, Me.ToMCStatus(grpPath, eVarNameFlags.QBInput))
                grp.SetStatusFlags(eVarNameFlags.mcQBUpper, Me.ToMCStatus(grpPath, eVarNameFlags.QBInput))
                grp.SetStatusFlags(eVarNameFlags.mcQBbf, Me.ToMCStatus(grpPath, eVarNameFlags.QBInput, True))

                ' BA
                grp.SetStatusFlags(eVarNameFlags.mcBA, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccum))
                grp.SetStatusFlags(eVarNameFlags.mcBAcv, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccum))
                grp.SetStatusFlags(eVarNameFlags.mcBALower, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccum))
                grp.SetStatusFlags(eVarNameFlags.mcBAUpper, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccum))
                grp.SetStatusFlags(eVarNameFlags.mcBAbf, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccum, True))

                ' EE
                grp.SetStatusFlags(eVarNameFlags.mcEE, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEEcv, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEELower, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEEUpper, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEEbf, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput, True))

                grp.AllowValidation = True

            Next 'For Each grp As cMonteCarloGroup

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
        End Try

        Me.AddMessage(New cMessage("MC groups updated", eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Maintenance))

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Derive status flags for Monte Carlo groups from Ecopath input statuses.
    ''' </summary>
    ''' <param name="grp">The Ecopath group to read status information from.</param>
    ''' <param name="var">The varname of the status to read.</param>
    ''' <returns>A montecarlified status flag.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ToMCStatus(ByVal grp As cEcoPathGroupInput, ByVal var As eVarNameFlags, _
                                Optional ByVal bIsBestFit As Boolean = False) As eStatusFlags

        Dim status As eStatusFlags = grp.GetStatus(var)

        ' Stanza groups should only allow B and QB edits in MCMC when configured as leading
        If grp.isMultiStanza Then

            Dim sg As cStanzaGroup = Me.m_core.StanzaGroups(grp.iStanza)
            Select Case var
                Case eVarNameFlags.BiomassAreaInput
                    If (sg.iGroups(sg.LeadingB) = grp.Index) Then status = eStatusFlags.OK
                Case eVarNameFlags.QBInput
                    If (sg.iGroups(sg.LeadingCB) = grp.Index) Then status = eStatusFlags.OK

                Case eVarNameFlags.PBInput
                    'PB needs to be supplied for all stages in a Multistanza group
                    'so it can be varied
                    status = eStatusFlags.OK
            End Select
        End If

        ' Any null or not editable status flag should be blocked out in the MCMC interface
        If ((status And (eStatusFlags.Null Or eStatusFlags.NotEditable)) > 0) Then
            If bIsBestFit Then
                status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
            Else
                status = eStatusFlags.NotEditable Or eStatusFlags.Null
            End If
        End If

        Return status

    End Function

    ' ''' <summary>
    ' ''' Update the Monte Carlo groups with the best fit of the trials
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Sub LoadBestFitsToGroups()

    '    Try
    '        Dim m_epdata As cEcopathDataStructures = m_core.m_EcoPathData
    '        Dim m_ecopath As cEcoPathModel = m_core.m_EcoPath

    '        For Each grp As cMonteCarloGroup In m_lstGrps

    '            grp.AllowValidation = False

    '            'convert the Database ID into an iGroup
    '            grp.Index = Array.IndexOf(m_epdata.GroupDBID, grp.DBID)
    '            grp.Resize()

    '            'best fit data from the monte carlo trials
    '            grp.Bbf = m_mc.BestFit(eMCParams.Biomass, grp.Index)
    '            grp.PBbf = m_mc.BestFit(eMCParams.PB, grp.Index)
    '            grp.QBbf = m_mc.BestFit(eMCParams.QB, grp.Index)
    '            grp.BAbf = m_mc.BestFit(eMCParams.BA, grp.Index)
    '            grp.EEbf = m_mc.BestFit(eMCParams.EE, grp.Index)
    '            grp.VUbf = m_mc.BestFit(eMCParams.Vulnerability, grp.Index)

    '            'ReDim CVpar(5, m_core.nGroups)
    '            grp.Bcv = m_mc.CVpar(eMCParams.Biomass, grp.Index)
    '            grp.PBcv = m_mc.CVpar(eMCParams.PB, grp.Index)
    '            grp.QBcv = m_mc.CVpar(eMCParams.QB, grp.Index)
    '            grp.BAcv = m_mc.CVpar(eMCParams.BA, grp.Index)
    '            grp.EEcv = m_mc.CVpar(eMCParams.EE, grp.Index)
    '            grp.VUcv = m_mc.CVpar(eMCParams.Vulnerability, grp.Index)

    '            'ReDim ParLimit(1, 5, m_core.nGroups)
    '            grp.BLower = m_mc.ParLimit(0, eMCParams.Biomass, grp.Index)
    '            grp.PBLower = m_mc.ParLimit(0, eMCParams.PB, grp.Index)
    '            grp.QBLower = m_mc.ParLimit(0, eMCParams.QB, grp.Index)
    '            grp.BALower = m_mc.ParLimit(0, eMCParams.BA, grp.Index)
    '            grp.EELower = m_mc.ParLimit(0, eMCParams.EE, grp.Index)
    '            grp.VULower = m_mc.ParLimit(0, eMCParams.Vulnerability, grp.Index)

    '            grp.BUpper = m_mc.ParLimit(1, eMCParams.Biomass, grp.Index)
    '            grp.PBUpper = m_mc.ParLimit(1, eMCParams.PB, grp.Index)
    '            grp.QBUpper = m_mc.ParLimit(1, eMCParams.QB, grp.Index)
    '            grp.BAUpper = m_mc.ParLimit(1, eMCParams.BA, grp.Index)
    '            grp.EEUpper = m_mc.ParLimit(1, eMCParams.EE, grp.Index)
    '            grp.VUUpper = m_mc.ParLimit(1, eMCParams.Vulnerability, grp.Index)

    '            grp.ResetStatusFlags()

    '            'validation for monte carlo groups should be handled by the manager
    '            'the manager and the monte carlo model know what to do in response to an edit
    '            'this is not setup yet
    '            grp.AllowValidation = True

    '        Next 'For Each grp As cMonteCarloGroup

    '        Me.AddMessage(New cMessage("MC groups updated", eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Maintenance))

    '    Catch ex As Exception
    '        cLog.Write(ex)
    '        Debug.Assert(False, ex.StackTrace)
    '        Throw New ApplicationException("UpdateGroupsBestFit", ex)
    '    End Try


    'End Sub

    Private Sub InitGroups()

        Try
            m_lstGrps = Nothing
            m_lstGrps = New List(Of cMonteCarloGroup)

            For igrp As Integer = 1 To m_core.nGroups
                m_lstGrps.Add(New cMonteCarloGroup(m_core, m_core.m_EcoPathData.GroupDBID(igrp)))
            Next

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException("LoadGroupParameters", ex)
        End Try


    End Sub

    ''' <summary>
    ''' Update the underlying data with edited values from the MonteCarloGroups
    ''' </summary>
    ''' <remarks>Brute force called at the start of each run</remarks>
    Friend Sub update()

        Try

            For Each MCGroup As cMonteCarloGroup In m_lstGrps
                'convert the Database ID into an iGroup
                MCGroup.Index = Array.IndexOf(m_core.m_EcoPathData.GroupDBID, MCGroup.DBID)
                MCGroup.Resize()

                m_mc.Pmean(eMCParams.Biomass, MCGroup.Index) = MCGroup.B
                m_mc.Pmean(eMCParams.PB, MCGroup.Index) = MCGroup.PB
                m_mc.Pmean(eMCParams.QB, MCGroup.Index) = MCGroup.QB
                m_mc.Pmean(eMCParams.BA, MCGroup.Index) = MCGroup.BA
                m_mc.Pmean(eMCParams.EE, MCGroup.Index) = MCGroup.EE
                m_mc.Pmean(eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VU

                'ReDim CVpar(5, m_core.nGroups)
                m_mc.CVpar(eMCParams.Biomass, MCGroup.Index) = MCGroup.Bcv
                m_mc.CVpar(eMCParams.PB, MCGroup.Index) = MCGroup.PBcv
                m_mc.CVpar(eMCParams.QB, MCGroup.Index) = MCGroup.QBcv
                m_mc.CVpar(eMCParams.BA, MCGroup.Index) = MCGroup.BAcv
                m_mc.CVpar(eMCParams.EE, MCGroup.Index) = MCGroup.EEcv
                m_mc.CVpar(eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VUcv

                'ReDim ParLimit(1, 5, m_core.nGroups)
                m_mc.ParLimit(0, eMCParams.Biomass, MCGroup.Index) = MCGroup.BLower
                m_mc.ParLimit(0, eMCParams.PB, MCGroup.Index) = MCGroup.PBLower
                m_mc.ParLimit(0, eMCParams.QB, MCGroup.Index) = MCGroup.QBLower
                m_mc.ParLimit(0, eMCParams.BA, MCGroup.Index) = MCGroup.BALower
                m_mc.ParLimit(0, eMCParams.EE, MCGroup.Index) = MCGroup.EELower
                m_mc.ParLimit(0, eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VULower

                m_mc.ParLimit(1, eMCParams.Biomass, MCGroup.Index) = MCGroup.BUpper
                m_mc.ParLimit(1, eMCParams.PB, MCGroup.Index) = MCGroup.PBUpper
                m_mc.ParLimit(1, eMCParams.QB, MCGroup.Index) = MCGroup.QBUpper
                m_mc.ParLimit(1, eMCParams.BA, MCGroup.Index) = MCGroup.BAUpper
                m_mc.ParLimit(1, eMCParams.EE, MCGroup.Index) = MCGroup.EEUpper
                m_mc.ParLimit(1, eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VUUpper
            Next MCGroup

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException("UpdateGroupsBestFit", ex)
        End Try


    End Sub

#End Region

#Region "ICoreInterface"

    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return (eDataTypes.MonteCarlo)
        End Get
    End Property

    Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return eCoreComponentType.EcoSimMonteCarlo
        End Get
    End Property

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return m_dbid
        End Get
        Set(ByVal value As Integer)
            m_dbid = value
        End Set
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Return m_name & "_" & m_dbid.ToString
    End Function

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return m_index
        End Get
        Set(ByVal value As Integer)
            m_index = value
        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return m_name
        End Get
        Set(ByVal value As String)
            m_name = value
        End Set
    End Property
#End Region

End Class
