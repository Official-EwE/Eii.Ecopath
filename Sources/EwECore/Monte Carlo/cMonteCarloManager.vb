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
Imports System.Threading
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Manager to run the Ecosim Monte Carlo.
''' </summary>
Public Class cMonteCarloManager
    Inherits cThreadWaitBase
    Implements ICoreInterface

    Private Delegate Sub dlgSendMessages()

    'ToDo_jb: cMonteCarloManager FisForce flag in EwE5 the "Retain current Ecosim fishing rate pattern" check box sets fisforce to true for all groups
    'this never gets set back to the value computed in DoDatValCalculations. It should be able to reset fisforce() by calling the EwE6 equivalent of DoDatValCalculations when False

#Region "Private variables"

    Private m_groups As List(Of cMonteCarloGroup)
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
    ''' <summary>Available monte carlo result writers.</summary>
    Private m_ResultsWriters As New List(Of IMonteCarloResultsWriter)

#End Region

#Region " Construction and initialization "

    Friend Sub New()
        ' NOP
    End Sub

    Friend Sub init(ByRef theCore As cCore)

        Try
            Me.m_core = theCore

            Me.m_mc = New cEcosimMonteCarlo(Me.m_core)
            'set all the delegates to handle events/messages from the monte carlo
            Me.m_mc.dlgMonteCarloCompletedHandler = AddressOf Me.MCCompletedHandler
            Me.m_mc.dlgEcopathIterationHandler = AddressOf Me.MCEcopathInterationHandler
            Me.m_mc.dlgTrialStepHandler = AddressOf Me.MCTrialProgressHandler
            Me.m_mc.dlgMonteCarloMessageHandler = AddressOf Me.MCSendMessageHandler

            Me.m_mc.Init()

            Me.m_ResultsWriters.Clear()
            Me.m_ResultsWriters.Add(New cMonteCarloResultsWriterOneFile(Me.m_mc, Me.m_core))
            Me.m_ResultsWriters.Add(New cMonteCarloResultsWriterMultipleFiles(Me.m_mc, Me.m_core))

            ' Plug-in manager provided?
            If (Me.m_core.PluginManager IsNot Nothing) Then
                ' #Yes: see if a plug-in based writer supports the requested format
                For Each ip As IMonteCarloResultWriterPlugin In Me.m_core.PluginManager.GetPlugins(GetType(IMonteCarloResultWriterPlugin))
                    Me.m_ResultsWriters.Add(ip)
                Next
            End If

            Me.InitGroups()
            Me.LoadGroups()

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".init()", ex)
        End Try


    End Sub

    Public Sub Clear()
        Try
            If Me.m_groups Is Nothing Then Exit Sub
            For Each MCgrp As cMonteCarloGroup In Me.m_groups
                MCgrp.Clear()
            Next
            Me.m_groups.Clear()
            Me.m_groups = Nothing

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of available <see cref="IMonteCarloResultsWriter">Monte
    ''' Carlo result writers</see>.
    ''' <seealso cref="ResultWriter"/>
    ''' <seealso cref="IMonteCarloResultsWriter"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nResultWriters As Integer
        Get
            Return Me.m_ResultsWriters.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a <see cref="IMonteCarloResultsWriter"/>.
    ''' <seealso cref="nResultWriters"/>
    ''' <seealso cref="IMonteCarloResultsWriter"/>
    ''' </summary>
    ''' <param name="iIndex">One-based index of the result writer to obtain.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function ResultWriter(iIndex As Integer) As IMonteCarloResultsWriter
        Try
            If (iIndex < 0) Or (iIndex > Me.nResultWriters) Then Return Nothing
            Return Me.m_ResultsWriters(iIndex - 1)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Return Nothing
    End Function

    Public Property ActiveResultWriter As IMonteCarloResultsWriter
        Get
            If (Me.m_mc Is Nothing) Then Return Nothing
            Return Me.m_mc.ResultWriter
        End Get
        Set(value As IMonteCarloResultsWriter)
            If (Me.m_mc Is Nothing) Then Return
            Me.m_mc.ResultWriter = value
        End Set
    End Property

#End Region ' Construction and initialization

#Region " Running "

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
            If Me.m_core.StateMonitor.HasEcosimLoaded Then

                Me.SetWait()
                Me.Update()

                thrdMC = New Thread(AddressOf Me.m_mc.Run)
                thrdMC.Start()

            Else 'If m_core.StateMonitor.HasEcosimLoaded Then

                'no ecosim scenario loaded
                Me.m_core.Messages.SendMessage(New cMessage(My.Resources.CoreMessages.MONTECARLO_ECOSIM_MISSING, eMessageType.StateNotMet, eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Warning, eDataTypes.MonteCarlo))

            End If

        Catch ex As Exception
            cLog.Write(ex)
            Me.ReleaseWait()
            Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.MONTECARLO_RUN_ERROR, ex.Message),
                                                     eMessageType.ErrorEncountered, eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Critical, eDataTypes.MonteCarlo))
        End Try

        Me.m_core.Messages.sendAllMessages()

        Return

    End Sub

    ''' <summary>
    ''' Load the current data into the MonteCarlo parameters
    ''' </summary>
    Public Sub Load()

        Me.m_mc.Init()
        Me.m_mc.initForRun()

        ' Let MC know which variables can be varied
        ' MC used to figure this out, but the MC manager does a much more thorough job
        ' Ideally both tools would draw from the same base logic. Using reviously set status 
        ' flags is not the neatest way to inform core models how to behave, but for now this
        ' fixes bugs where variables were being varied while they should not have.

        For igroup As Integer = 1 To Me.m_core.nGroups
            Dim grp As cMonteCarloGroup = Me.Groups(igroup)
            Me.m_mc.SetIsVariable(igroup, eMCParams.Biomass, Me.IsMCVariable(grp, eVarNameFlags.mcB))
            Me.m_mc.SetIsVariable(igroup, eMCParams.PB, Me.IsMCVariable(grp, eVarNameFlags.mcPB))
            Me.m_mc.SetIsVariable(igroup, eMCParams.QB, Me.IsMCVariable(grp, eVarNameFlags.mcQB))
            Me.m_mc.SetIsVariable(igroup, eMCParams.BA, Me.IsMCVariable(grp, eVarNameFlags.mcBA))
            Me.m_mc.SetIsVariable(igroup, eMCParams.BaBi, Me.IsMCVariable(grp, eVarNameFlags.mcBaBi))
            Me.m_mc.SetIsVariable(igroup, eMCParams.EE, Me.IsMCVariable(grp, eVarNameFlags.mcEE))
            ' Cannot disable diet perturbation per group
            'Me.m_mc.SetIsVariable(iGroup, eMCParams.Diets, IsMCVariable(grp, eVarNameFlags.mcDC))

        Next
    End Sub

#End Region ' Running

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
            Me.m_SyncObject.BeginInvoke(dlgsendmsgs, Nothing)

            'tell the interface
            If Me.m_SyncObject IsNot Nothing And Me.m_dlgMCCompletedHandler IsNot Nothing Then
                'use the SyncObject provided by the interface to call the completed handler in the interface
                Me.m_SyncObject.BeginInvoke(Me.m_dlgMCCompletedHandler, Nothing)
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
            For Each msg As cMessage In Me.m_lstMessages
                Me.m_core.Messages.AddMessage(msg)
            Next
            Me.m_core.Messages.sendAllMessages()
            Me.m_lstMessages.Clear()
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
            If Me.m_SyncObject IsNot Nothing And Me.m_dlgMCEcopathStepHandler IsNot Nothing Then
                'use the SyncObject provided by the interface to call the completed handler in the interface
                Me.m_SyncObject.BeginInvoke(Me.m_dlgMCEcopathStepHandler, Nothing)
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub MCTrialProgressHandler()

        Try


            'tell the interface
            If Me.m_SyncObject IsNot Nothing And Me.m_dlgMCTrialStepHandler IsNot Nothing Then
                'use the SyncObject provided by the interface to call the completed handler in the interface
                Me.m_SyncObject.Invoke(Me.m_dlgMCTrialStepHandler, Nothing)
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

#Region " Saving "


    ''' <summary>
    ''' Apply the Monte Carlo results (best fitting parameters) to the Ecopath inputs (B,PB....)
    ''' </summary>
    Public Sub ApplyBestFits()

        Try

            Me.m_mc.ApplyBestFits()

            '#Hack: Tell the core that Ecopath inputs have changed
            '       cCore.OnChanged(me) does not support the granularity to invalidate Ecopath data in response to only this event
            Me.m_core.DataSource.SetChanged(eCoreComponentType.EcoPath)

            'tell the core to reload groups from modified Ecopath inputs
            Me.m_core.onChanged(Me, eMessageType.DataModified)

            Me.LoadGroups()

            'run ecopath with the new parameters
            Me.m_core.RunEcopath()
            'initialize ecosim with the new data
            Me.m_core.m_EcoSim.Init(True)

            Me.m_core.RunEcosim()
            Dim ss As Single = Me.m_core.EcosimStats.SS

        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex)
            Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.MONTECARLO_APPLY_ERROR, ex.Message),
                                                     eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Critical))
        End Try

    End Sub

#End Region ' Saving

#Region " Public Properties and Methods "

    ''' <summary>
    ''' Returns which <see cref="eMCParams">parameters</see> can be pertubed.
    ''' </summary>
    ''' <returns></returns>
    Public Function SupportedVariables() As eMCParams()
        Return New eMCParams() {eMCParams.BA, eMCParams.Biomass, eMCParams.Diets, eMCParams.Discards, eMCParams.EE, eMCParams.Landings, eMCParams.PB, eMCParams.QB, eMCParams.BaBi}
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a message the the managers list of messages generated by the monte carlo routine
    ''' </summary>
    ''' <param name="theMessage">The <see cref="cMessage"/> to add.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub AddMessage(ByRef theMessage As cMessage)
        Try
            Me.m_lstMessages.Add(theMessage)
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
    Public Overrides Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean
        Dim result As Boolean = True

        If (Me.m_core Is Nothing) Then Return result
        If (Me.m_mc Is Nothing) Then Return result

        Try
            Me.m_mc.StopTrial = True
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
        Set(value As Integer)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.Ntrials = value
            End If
        End Set
    End Property

    Public ReadOnly Property IsBestFit As Boolean
        Get
            Return Me.m_mc.IsBestFit
        End Get
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
            Return Me.m_mc.RetainBiomass
        End Get
        Set(value As Boolean)
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
        Set(value As Boolean)
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
        Set(value As Boolean)
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
            Return Me.m_core.m_EcoSimData.SS
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
            Return Me.m_mc.SSorg
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
            Return Me.m_mc.SSBestFit
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of attempts at finding a balanced Ecopath model for
    ''' the current trial.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nEcopathIterations() As Integer
        Get
            If (Me.m_mc Is Nothing) Then Return 0
            Return Me.m_mc.nEcopathIterations
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
        Set(value As MonteCarloEcopathProgressDelegate)
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
        Set(value As MonteCarloTrialProgressDelegate)
            Me.m_dlgMCTrialStepHandler = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the method to call in the interface when the Monte Carlo trials have completed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public WriteOnly Property MonteCarloCompletedHandler() As MonteCarloCompletedDelegate
        Set(value As MonteCarloCompletedDelegate)
            'the Monte Carlo object will call the manger and the manager will call the interface with this delegate
            'see MCCompletedHandler()
            Me.m_dlgMCCompletedHandler = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the <see cref="System.ComponentModel.ISynchronizeInvoke">Synchronization object</see>, which can be
    ''' a System.Windows.Forms.Control, used for calling all the delegates across threads
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public WriteOnly Property SyncObject() As System.ComponentModel.ISynchronizeInvoke
        Set(value As System.ComponentModel.ISynchronizeInvoke)
            Me.m_SyncObject = value
        End Set
    End Property

    Public WriteOnly Property EcosimTimeStepHandler() As EcoSimTimeStepDelegate
        Set(value As EcoSimTimeStepDelegate)
            Debug.Assert(Me.m_mc IsNot Nothing)
            'save the delegate for use with the bShowPlot flag
            '  m_EcosimTimeStepHandler = value
            'Changed this to pass the delegate directly to the monte carlo model
            'it will decide when to turn the plotting on or off
            Me.m_mc.EcosimTimeStep = value
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
    Public ReadOnly Property Groups(iGroup As Integer) As cMonteCarloGroup
        Get
            Return Me.m_groups.Item(iGroup - 1)
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
            Return Me.m_mc.SaveOutput
        End Get
        Set(value As Boolean)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.SaveOutput = value
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
        Set(value As Single)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.EcopathEETol = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Monte Carlo should validate and reject negative respiration values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ValidateRespiration As Boolean
        Get
            If (Me.m_mc Is Nothing) Then Return False
            Return Me.m_mc.ValidateRespiration
        End Get
        Set(value As Boolean)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.ValidateRespiration = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set which <see cref="eMCDietSamplingMethod">diet sampling method</see>
    ''' Monte Carlo should use.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DietSamplingMethod As eMCDietSamplingMethod
        Get
            If (Me.m_mc Is Nothing) Then Return eMCDietSamplingMethod.Dirichlets
            Return Me.m_mc.DietSamplingMethod
        End Get
        Set(value As eMCDietSamplingMethod)
            If (Me.m_mc IsNot Nothing) Then
                Me.m_mc.DietSamplingMethod = value
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
    ''' Select a new set of Ecopath parameters using  CV, Mean, Max and Min set in <see cref="cMonteCarloGroup">cMonteCarloGroup</see>
    ''' </summary>
    ''' <param name="MaxEcopathIteration">Maximum number of tries to find a balanced Ecopath Model.</param>
    ''' <returns>True if a balanced Ecopath model was found within MaxEcopathIteration. False otherwise. </returns>
    ''' <remarks>This functionality was added to simplify external process that want to run there own Monte Carlo style models. </remarks>
    ''' -----------------------------------------------------------------------
    Public Function SelectNewEcopathParameters(Optional MaxEcopathIteration As Integer = 10000) As Boolean
        Try
            Debug.Assert(Me.m_mc IsNot Nothing)
            'force the interface objects to update the underlying data
            Me.Update()
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

    ''' <summary>
    ''' Get/set whether Monte Carlo is allowed to vary a given variable.
    ''' </summary>
    ''' <param name="vn">The <see cref="eMCParams">variable</see> to change.</param>
    Public Property Enable(vn As eMCParams) As Boolean
        Get
            If (Me.m_mc Is Nothing) Then Return False
            Return Me.m_mc.IsEnabled(vn)
        End Get
        Set(value As Boolean)
            If (Me.m_mc Is Nothing) Then Return
            Me.m_mc.IsEnabled(vn) = value
        End Set
    End Property

#End Region ' Public Properties and Methods

#Region "Private methods"

    Friend Sub CalculateUpperLowerLimits()

        Try
            Me.Update()
            Me.m_mc.CalculateUpperLowerLimits(False)
            Me.LoadGroups()
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Friend Sub LoadGroups()

        Try
            Dim m_epdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim m_esdata As cEcosimDatastructures = Me.m_core.m_EcoSimData
            Dim iGroup As Integer = 0

            For Each grp As cMonteCarloGroup In Me.m_groups

                grp.AllowValidation = False

                'convert the Database ID into an iGroup
                iGroup = Array.IndexOf(m_epdata.GroupDBID, grp.DBID)
                grp.Index = iGroup
                grp.Resize()

                grp.Name = m_epdata.GroupName(grp.Index)

                'data from Ecopath
                grp.B = m_epdata.B(iGroup)
                grp.PB = m_epdata.PB(iGroup)
                grp.QB = m_epdata.QB(iGroup)
                grp.BA = m_epdata.BA(iGroup)
                grp.BaBi = m_epdata.BaBi(iGroup)
                grp.EE = m_epdata.EE(iGroup)
                grp.VU = m_esdata.VulnerabilityPredator(iGroup)

                For iflt As Integer = 1 To m_epdata.NumFleet
                    grp.Landings(iflt) = m_epdata.Landing(iflt, iGroup)
                    grp.Discards(iflt) = m_epdata.Discard(iflt, iGroup)
                Next

                For iPrey As Integer = 0 To Me.m_core.nGroups
                    grp.Diets(iPrey) = m_epdata.DC(iGroup, iPrey)
                Next

                grp.Bcv = Me.m_mc.CVpar(eMCParams.Biomass, iGroup)
                grp.PBcv = Me.m_mc.CVpar(eMCParams.PB, iGroup)
                grp.QBcv = Me.m_mc.CVpar(eMCParams.QB, iGroup)
                grp.BAcv = Me.m_mc.CVpar(eMCParams.BA, iGroup)
                grp.BaBicv = Me.m_mc.CVpar(eMCParams.BaBi, iGroup)
                grp.EEcv = Me.m_mc.CVpar(eMCParams.EE, iGroup)
                grp.VUcv = Me.m_mc.CVpar(eMCParams.Vulnerability, iGroup)

                For iFleet As Integer = 1 To Me.m_core.nFleets
                    grp.Landingscv(iFleet) = Me.m_mc.CVparLanding(iFleet, iGroup)
                    grp.Discardscv(iFleet) = Me.m_mc.CVparDiscard(iFleet, iGroup)
                Next

                grp.DietMultiplier = Me.m_mc.CVParDC(eMCDietSamplingMethod.Dirichlets, iGroup)
                grp.Dietcv = Me.m_mc.CVParDC(eMCDietSamplingMethod.NormalDistribution, iGroup)

                grp.BLower = Me.m_mc.ParLimit(0, eMCParams.Biomass, iGroup)
                grp.PBLower = Me.m_mc.ParLimit(0, eMCParams.PB, iGroup)
                grp.QBLower = Me.m_mc.ParLimit(0, eMCParams.QB, iGroup)
                grp.BALower = Me.m_mc.ParLimit(0, eMCParams.BA, iGroup)
                grp.BaBiLower = Me.m_mc.ParLimit(0, eMCParams.BaBi, iGroup)
                grp.EELower = Me.m_mc.ParLimit(0, eMCParams.EE, iGroup)
                grp.VULower = Me.m_mc.ParLimit(0, eMCParams.Vulnerability, iGroup)

                For iFleet As Integer = 1 To Me.m_core.nFleets
                    grp.LandingsLower(iFleet) = Me.m_mc.ParLimitLanding(0, iFleet, iGroup)
                    grp.DiscardsLower(iFleet) = Me.m_mc.ParLimitDiscard(0, iFleet, iGroup)
                Next

                grp.BUpper = Me.m_mc.ParLimit(1, eMCParams.Biomass, iGroup)
                grp.PBUpper = Me.m_mc.ParLimit(1, eMCParams.PB, iGroup)
                grp.QBUpper = Me.m_mc.ParLimit(1, eMCParams.QB, iGroup)
                grp.BAUpper = Me.m_mc.ParLimit(1, eMCParams.BA, iGroup)
                grp.BaBiUpper = Me.m_mc.ParLimit(1, eMCParams.BaBi, iGroup)
                grp.EEUpper = Me.m_mc.ParLimit(1, eMCParams.EE, iGroup)
                grp.VUUpper = Me.m_mc.ParLimit(1, eMCParams.Vulnerability, iGroup)

                For iFleet As Integer = 1 To Me.m_core.nFleets
                    grp.LandingsUpper(iFleet) = Me.m_mc.ParLimitLanding(1, iFleet, iGroup)
                    grp.DiscardsUpper(iFleet) = Me.m_mc.ParLimitDiscard(1, iFleet, iGroup)
                Next

                'best fit data from the monte carlo trials, if any
                grp.Bbf = Me.m_mc.BestFit(eMCParams.Biomass, iGroup)
                grp.PBbf = Me.m_mc.BestFit(eMCParams.PB, iGroup)
                grp.QBbf = Me.m_mc.BestFit(eMCParams.QB, iGroup)
                grp.BAbf = Me.m_mc.BestFit(eMCParams.BA, iGroup)
                grp.BaBibf = Me.m_mc.BestFit(eMCParams.BaBi, iGroup)
                grp.EEbf = Me.m_mc.BestFit(eMCParams.EE, iGroup)
                grp.VUbf = Me.m_mc.BestFit(eMCParams.Vulnerability, iGroup)

                For iFleet As Integer = 1 To Me.m_core.nFleets
                    grp.Landingsbf(iFleet) = Me.m_mc.BestFit(eMCParams.Landings, iGroup)
                    grp.Discardsbf(iFleet) = Me.m_mc.BestFit(eMCParams.Discards, iGroup)
                Next

                For iPrey As Integer = 1 To Me.m_core.nGroups
                    grp.Dietsbf = Me.m_mc.BestFitDiets(iGroup, iPrey)
                Next

                grp.ResetStatusFlags()

                Dim grpPath As cEcoPathGroupInput = Me.m_core.EcopathGroupInputs(iGroup)

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
                grp.SetStatusFlags(eVarNameFlags.mcBA, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumInput))
                grp.SetStatusFlags(eVarNameFlags.mcBAcv, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumInput))
                grp.SetStatusFlags(eVarNameFlags.mcBALower, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumInput))
                grp.SetStatusFlags(eVarNameFlags.mcBAUpper, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumInput))
                grp.SetStatusFlags(eVarNameFlags.mcBAbf, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumInput, True))

                ' BaBi
                grp.SetStatusFlags(eVarNameFlags.mcBaBi, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumRate))
                grp.SetStatusFlags(eVarNameFlags.mcBaBicv, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumRate))
                grp.SetStatusFlags(eVarNameFlags.mcBaBiLower, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumRate))
                grp.SetStatusFlags(eVarNameFlags.mcBaBiUpper, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumRate))
                grp.SetStatusFlags(eVarNameFlags.mcBaBibf, Me.ToMCStatus(grpPath, eVarNameFlags.BioAccumRate, True))

                ' EE
                grp.SetStatusFlags(eVarNameFlags.mcEE, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEEcv, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEELower, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEEUpper, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput))
                grp.SetStatusFlags(eVarNameFlags.mcEEbf, Me.ToMCStatus(grpPath, eVarNameFlags.EEInput, True))

                ' Diet
                grp.SetStatusFlags(eVarNameFlags.mcDietMult, Me.ToMCStatus(grpPath, eVarNameFlags.DietComp))
                grp.SetStatusFlags(eVarNameFlags.mcDC, Me.ToMCStatus(grpPath, eVarNameFlags.DietComp))
                grp.SetStatusFlags(eVarNameFlags.mcDCcv, Me.ToMCStatus(grpPath, eVarNameFlags.DietComp))
                grp.SetStatusFlags(eVarNameFlags.mcDCbf, Me.ToMCStatus(grpPath, eVarNameFlags.DietComp, True))

                ' Catches
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    grp.SetStatusFlags(eVarNameFlags.mcLandingsbf, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, iFleet)
                    grp.SetStatusFlags(eVarNameFlags.mcDiscardsbf, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, iFleet)
                Next

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
    Private Function ToMCStatus(grp As cEcoPathGroupInput, var As eVarNameFlags,
                                Optional bIsBestFit As Boolean = False) As eStatusFlags

        Dim status As eStatusFlags = grp.GetStatus(var)

        ' Stanza groups should only allow B and QB edits in MCMC when configured as leading
        If grp.IsMultiStanza Then

            Dim sg As cStanzaGroup = Me.m_core.StanzaGroups(grp.iStanza)
            Select Case var
                Case eVarNameFlags.BiomassAreaInput
                    status = If(sg.iGroups(sg.LeadingB) = grp.Index, eStatusFlags.OK, eStatusFlags.NotEditable Or eStatusFlags.Null)
                Case eVarNameFlags.QBInput
                    status = If(sg.iGroups(sg.LeadingCB) = grp.Index, eStatusFlags.OK, eStatusFlags.NotEditable Or eStatusFlags.Null)
                Case eVarNameFlags.PBInput
                    'PB needs to be supplied for all stages in a Multistanza group; it can be varied
                    status = eStatusFlags.OK
            End Select
        Else
            Dim grpIn As cEcoPathGroupInput = Me.m_core.EcopathGroupInputs(grp.Index)
            Select Case var
                Case eVarNameFlags.BioAccumInput
                    If (grpIn.BioAccumInput = 0) Then
                        status = eStatusFlags.NotEditable Or eStatusFlags.Null
                    End If
                Case eVarNameFlags.BioAccumRate
                    If (grpIn.BioAccumRate = 0) Then
                        status = eStatusFlags.NotEditable Or eStatusFlags.Null
                    End If
            End Select
        End If

        If (var = eVarNameFlags.DietComp) Then
            status = If(grp.IsConsumer, eStatusFlags.OK, eStatusFlags.NotEditable Or eStatusFlags.Null)
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

    Private Function IsMCVariable(grp As cMonteCarloGroup, var As eVarNameFlags) As Boolean
        Dim status As eStatusFlags = grp.GetStatus(var)
        Return (status And eStatusFlags.Null) = 0
    End Function

    Private Sub InitGroups()

        Try
            Me.m_groups = Nothing
            Me.m_groups = New List(Of cMonteCarloGroup)

            For igrp As Integer = 1 To Me.m_core.nGroups
                Me.m_groups.Add(New cMonteCarloGroup(Me.m_core, Me.m_core.m_EcoPathData.GroupDBID(igrp)))
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
    Friend Sub Update()

        Try

            For Each MCGroup As cMonteCarloGroup In Me.m_groups
                'convert the Database ID into an iGroup
                MCGroup.Index = Array.IndexOf(Me.m_core.m_EcoPathData.GroupDBID, MCGroup.DBID)
                MCGroup.Resize()

                Me.m_mc.Pmean(eMCParams.Biomass, MCGroup.Index) = MCGroup.B
                Me.m_mc.Pmean(eMCParams.PB, MCGroup.Index) = MCGroup.PB
                Me.m_mc.Pmean(eMCParams.QB, MCGroup.Index) = MCGroup.QB
                Me.m_mc.Pmean(eMCParams.BA, MCGroup.Index) = MCGroup.BA
                Me.m_mc.Pmean(eMCParams.BaBi, MCGroup.Index) = MCGroup.BaBi
                Me.m_mc.Pmean(eMCParams.EE, MCGroup.Index) = MCGroup.EE
                Me.m_mc.Pmean(eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VU

                For ifleet As Integer = 1 To Me.m_core.nFleets
                    Me.m_mc.PMeanLanding(ifleet, MCGroup.Index) = MCGroup.Landings(ifleet)
                    Me.m_mc.PMeanDiscard(ifleet, MCGroup.Index) = MCGroup.Discards(ifleet)
                Next

                For iPrey As Integer = 0 To Me.m_core.nGroups
                    Me.m_mc.PMeanDC(MCGroup.Index, iPrey) = MCGroup.Diets(iPrey)
                Next

                Me.m_mc.CVpar(eMCParams.Biomass, MCGroup.Index) = MCGroup.Bcv
                Me.m_mc.CVpar(eMCParams.PB, MCGroup.Index) = MCGroup.PBcv
                Me.m_mc.CVpar(eMCParams.QB, MCGroup.Index) = MCGroup.QBcv
                Me.m_mc.CVpar(eMCParams.BA, MCGroup.Index) = MCGroup.BAcv
                Me.m_mc.CVpar(eMCParams.BaBi, MCGroup.Index) = MCGroup.BaBicv
                Me.m_mc.CVpar(eMCParams.EE, MCGroup.Index) = MCGroup.EEcv
                Me.m_mc.CVpar(eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VUcv
                Me.m_mc.CVParDC(eMCDietSamplingMethod.Dirichlets, MCGroup.Index) = MCGroup.DietMultiplier
                Me.m_mc.CVParDC(eMCDietSamplingMethod.NormalDistribution, MCGroup.Index) = MCGroup.Dietcv

                For ifleet As Integer = 1 To Me.m_core.nFleets
                    Me.m_mc.CVparLanding(ifleet, MCGroup.Index) = MCGroup.Landingscv(ifleet)
                    Me.m_mc.CVparDiscard(ifleet, MCGroup.Index) = MCGroup.Discardscv(ifleet)
                Next

                Me.m_mc.ParLimit(0, eMCParams.Biomass, MCGroup.Index) = MCGroup.BLower
                Me.m_mc.ParLimit(0, eMCParams.PB, MCGroup.Index) = MCGroup.PBLower
                Me.m_mc.ParLimit(0, eMCParams.QB, MCGroup.Index) = MCGroup.QBLower
                Me.m_mc.ParLimit(0, eMCParams.BA, MCGroup.Index) = MCGroup.BALower
                Me.m_mc.ParLimit(0, eMCParams.BaBi, MCGroup.Index) = MCGroup.BaBiLower
                Me.m_mc.ParLimit(0, eMCParams.EE, MCGroup.Index) = MCGroup.EELower
                Me.m_mc.ParLimit(0, eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VULower

                For iFleet As Integer = 1 To Me.m_core.nFleets
                    Me.m_mc.ParLimitLanding(0, iFleet, MCGroup.Index) = MCGroup.LandingsLower(iFleet)
                    Me.m_mc.ParLimitDiscard(0, iFleet, MCGroup.Index) = MCGroup.DiscardsLower(iFleet)
                Next

                Me.m_mc.ParLimit(1, eMCParams.Biomass, MCGroup.Index) = MCGroup.BUpper
                Me.m_mc.ParLimit(1, eMCParams.PB, MCGroup.Index) = MCGroup.PBUpper
                Me.m_mc.ParLimit(1, eMCParams.QB, MCGroup.Index) = MCGroup.QBUpper
                Me.m_mc.ParLimit(1, eMCParams.BA, MCGroup.Index) = MCGroup.BAUpper
                Me.m_mc.ParLimit(1, eMCParams.BaBi, MCGroup.Index) = MCGroup.BaBiUpper
                Me.m_mc.ParLimit(1, eMCParams.EE, MCGroup.Index) = MCGroup.EEUpper
                Me.m_mc.ParLimit(1, eMCParams.Vulnerability, MCGroup.Index) = MCGroup.VUUpper

                For iFleet As Integer = 1 To Me.m_core.nFleets
                    Me.m_mc.ParLimitLanding(1, iFleet, MCGroup.Index) = MCGroup.LandingsUpper(iFleet)
                    Me.m_mc.ParLimitDiscard(1, iFleet, MCGroup.Index) = MCGroup.DiscardsUpper(iFleet)
                Next
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

    Public Property Index() As Integer Implements ICoreInterface.Index

    Public Property Name() As String Implements ICoreInterface.Name

    Public Function GetID() As String Implements ICoreInterface.GetID
        Return Me.Name & "_" & Me.DBID.ToString
    End Function

#End Region

End Class
