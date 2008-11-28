'==============================================================================
'
' $Log: cMonteCarloManager.vb,v $
' Revision 1.9  2008/11/28 16:54:14  joeb
' Cleaned up ToDo's
'
' Revision 1.8  2008/10/15 21:15:26  villyc
' mc fixes
'
' Revision 1.7  2008/10/15 20:25:37  joeb
' Added MonteCarlo handler to onChanged()
'
' Revision 1.6  2008/10/10 23:22:20  villyc
' *** empty log message ***
'
' Revision 1.5  2008/10/04 01:10:30  villyc
' mc stuff, SS after MC are not correct, so not loading all parameters
'
' Revision 1.4  2008/10/01 16:50:29  villyc
' Ecosim monte carlo updates, plus ecosim plot bug fix
'
' Revision 1.3  2008/09/27 01:39:07  villyc
' ecosim monte carlo running with vulnerability fitting
'
' Revision 1.2  2008/09/26 23:00:41  villyc
' more ecosimmontecarlo fixing
'
' Revision 1.1  2008/09/26 07:30:28  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.30  2008/09/26 00:22:50  villyc
' updating ecosimMonteCarlo to pick vulnerabilities
'
' Revision 1.29  2008/06/25 17:38:28  joeb
' Fix bug 491 Initialization of Ecosim overwriting fishing mort with default
'
' Revision 1.28  2008/06/06 15:56:06  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.27  2008/05/12 18:59:02  joeb
' Restructure of search objects to use ISearchObjective interface
'
' Revision 1.26  2008/05/06 20:04:25  joeb
' Minor changes to cThreadedManagerBase
'
' Revision 1.25  2008/04/24 20:04:22  joeb
' Now inherits from cThreadedManagerBase
'
' Revision 1.24  2008/04/02 21:26:42  joeb
' Added Wait to the Monte Carlo manager
'
' Revision 1.23  2008/01/14 16:15:24  joeb
' Minor edits to comments
'
' Revision 1.22  2007/10/05 19:09:00  joeb
' Mean and BestFit values notEditable
'
' Revision 1.21  2007/09/29 01:15:50  joeb
' Bug fixes
'
' Revision 1.20  2007/08/29 14:49:50  joeb
' References are no longer stored when data is needed it is retrieved from the core.
'
' Revision 1.19  2007/08/24 20:06:53  joeb
' Minor change
'
' Revision 1.18  2007/08/24 19:53:04  joeb
' Changed communication between Model-Manager and Interface all interaction is now handled by the Manager
'
' Revision 1.17  2007/08/09 02:31:04  jeroens
' + Added maintenance message to the end of LoadBestFitsToGroups() to
'   inform GUI of changes
'
' Revision 1.16  2007/08/08 19:01:35  joeb
' Added messages to manager
'
' Revision 1.15  2007/08/07 19:20:54  joeb
' Group Name update
'
' Revision 1.14  2007/08/01 21:37:48  joeb
' Added bShowPlot flag
'
' Revision 1.13  2007/07/31 17:19:04  joeb
' Comments and ToDo
'
' Revision 1.12  2007/07/31 17:15:00  joeb
' Added isRunning Flag
'
' Revision 1.11  2007/07/31 16:22:18  jeroens
' + Added ResetStatusFlags after group update
'
' Revision 1.10  2007/07/31 16:02:52  joeb
' Changed eMessageSource from Ecosim to EcosimMonteCarlo
'
' Revision 1.9  2007/07/24 18:32:20  joeb
' Get ss to current data on load
'
' Revision 1.8  2007/07/24 16:55:49  joeb
' Fixed Ecosim initialization bug
'
' Revision 1.7  2007/07/19 19:54:28  joeb
' Updating of data on edit
'
' Revision 1.6  2007/07/18 17:27:43  joeb
' Made the MonteCarloManager a ICoreInterface object
' ApplyBestFits call cCore.OnChanged to tell the core that the manager has changed data
'
' Revision 1.5  2007/07/13 23:09:03  joeb
' Bunch of crap
'
' Revision 1.4  2007/07/13 00:07:45  joeb
' Bug fixes
'
' Revision 1.3  2007/06/26 22:26:00  joeb
' more more more cooooode
'
' Revision 1.2  2007/06/25 21:30:36  joeb
' A bunch of stuff
'
' Revision 1.1  2007/06/25 16:07:58  joeb
' Added Monte Carlo
'
'
'=====================================

Imports EwECore.Ecopath
Imports EwECore.EcoSim
Imports System
Imports System.Threading
Imports EwEUtils.Core


''' <summary>
''' Manager to run the ecosim monte carlo object
''' </summary>
''' <remarks>This object is a public interface to the monte carlo model.
'''  It will be a public property of the core to run the monte carlo model. 
'''  It will manage the interaction between the core, interface, monte carlo model and Ecopath/Ecoism. 
'''  Creating any objects that are needed to run the monte carlo routine.
'''  </remarks>
Public Class cMonteCarloManager
    Inherits cThreadWaitBase
    Implements ICoreInterface

    Private Delegate Sub dlgSendMessages()


    'ToDo_jb cMonteCarloManager FisForce flag in EwE5 the "Retain current Ecosim fishing rate pattern" check box sets fisforce to true for all groups
    'this never gets set back to the value computed in DoDatValCalculations. It should be able to reset fisforce() by calling the EwE6 equivalent of DoDatValCalculations when False


#Region "Private variables"

    Private m_lstGrps As List(Of cMonteCarloGroup)
    Private m_core As cCore
    Private m_mc As cEcosimMonteCarlo

    'Synchronization object from the user interface that handles the passing of data from the model thread to the user interface thread
    Private m_SyncObject As System.ComponentModel.ISynchronizeInvoke


    'Time step handler for ecosim 
    Private m_EcosimTimeStepHandler As EcoSimTimeStepDelegate

    'Delegates supplied by the interface to call in responce to an Monte Carlo delagate
    Private m_dlgMCCompletedHandler As MonteCarloCompletedDelegate
    Private m_dlgMCEcopathStepHandler As MonteCarloEcopathProgressDelegate
    Public m_dlgMCTrialStepHandler As MonteCarloTrialProgressDelegate

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

#End Region

#Region "Running"

    ''' <summary>
    ''' Run the Monte Carlo trials with the current parameters
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Run()
        Dim isThreading As Boolean = True

        If isThreading Then

            Dim thrdMC As Thread

            Try
                If m_core.StateMonitor.HasEcosimLoaded Then
                    If m_core.m_TSData.NdatType > 0 Then

                        Me.setWait()

                        Me.update()

                        thrdMC = New Thread(AddressOf m_mc.Run)
                        thrdMC.Start()

                    Else 'If m_core.m_TSData.NdatType > 0 Then
                        'm_core.m_TSData.NdatType = 0
                        'there must be at least one reference data set loaded
                        m_core.Messages.SendMessage(New cMessage("Monte Carlo: No time series reference data has been loaded. Please load time series reference data and try again.", eMessageType.StateNotMet, eMessageSource.EcoSimMonteCarlo, eMessageImportance.Warning, eDataTypes.MonteCarlo))
                    End If

                Else 'If m_core.StateMonitor.HasEcosimLoaded Then

                    'no ecosim scenario loaded
                    m_core.Messages.SendMessage(New cMessage("Monte Carlo: Please load an Ecosim scenario before running Monte Carlo.", eMessageType.StateNotMet, eMessageSource.EcoSimMonteCarlo, eMessageImportance.Warning, eDataTypes.MonteCarlo))

                End If

            Catch ex As Exception
                cLog.Write(ex)
                Me.ReleaseWait()
                m_core.Messages.SendMessage(New cMessage("Error running the Monte Carlo trials.", eMessageType.ErrorEncountered, eMessageSource.EcoSimMonteCarlo, eMessageImportance.Critical, eDataTypes.MonteCarlo))
            End Try

            m_core.Messages.sendAllMessages()
        End If

        Return

    End Sub



    ''' <summary>
    ''' Load the current data into the MonteCarlo parameters
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Load()

        m_mc.Init()
        m_mc.initForRun()

    End Sub


#End Region

#Region "Delegates called by the Monte Carlo class"

    ''' <summary>
    ''' The Monte Carlo routine has complete its trials. Load the best fitting data into the interace objects and tell the interface that the trials have completed
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub MCCompletedHandler()

        Try

            'load the best fits into the interface groups
            Me.LoadBestFitsToGroups()

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

            'make the interface has setup the manager properly
            'Debug.Assert(m_SyncObject IsNot Nothing)
            'Debug.Assert(Me.m_dlgMCTrialStepHandler IsNot Nothing)

            'tell the interface
            If m_SyncObject IsNot Nothing And m_dlgMCTrialStepHandler IsNot Nothing Then
                'use the SyncObject provided by the interface to call the completed handler in the interface
                m_SyncObject.BeginInvoke(m_dlgMCTrialStepHandler, Nothing)
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
    ''' <remarks></remarks>
    Public Sub ApplyBestFits()

        Try

            m_mc.ApplyBestFits()
            Me.LoadBestFitsToGroups()

            'tell the core that the monte carlo manager has changed the ecopath and ecosim data
            'this loads modeling data into core input/output objects
            m_core.onChanged(Me, eMessageType.DataModified)

            'run ecopath with the new parameters
            m_core.RunEcoPath()
            'initialize ecosim with the new data
            m_core.m_EcoSim.Init(True)

            m_core.RunEcoSim()
            Dim ss As Single = m_core.EcosimStats.SS

        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex)
            m_core.Messages.SendMessage(New cMessage("Monte Carlo Error: Failed to apply best fits.", eMessageType.ErrorEncountered, eMessageSource.EcoSim, eMessageImportance.Critical))
        End Try

    End Sub

#End Region

#Region "Public Properties"

    ''' <summary>
    ''' Add a message the the managers list of messages generated by the monte carlo
    ''' </summary>
    ''' <param name="theMessage"></param>
    ''' <remarks></remarks>
    Friend Sub AddMessage(ByRef theMessage As cMessage)
        Try
            m_lstMessages.Add(theMessage)
        Catch ex As Exception
            Debug.Assert(False, "AddMessage error " & ex.Message)
        End Try
    End Sub


    Public Property bShowPlot() As Boolean
        Get
            Return m_mc.bShowPlot
        End Get

        Set(ByVal value As Boolean)

            m_mc.bShowPlot = value

        End Set

    End Property

    ''' <summary>
    ''' Stop the current Monte Carlo trials
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StopRun() As Boolean
        m_mc.StopTrial = True
        Return True
    End Function


    ''' <summary>
    ''' Number of trials
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property nTrials() As Integer
        Get
            Return m_mc.Ntrials
        End Get
        Set(ByVal value As Integer)
            m_mc.Ntrials = value
        End Set
    End Property


    ''' <summary>
    ''' EwE 5 Retain better fitting estimates (use trials to search)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property bRetainFits() As Boolean
        Get
            Return m_mc.bRetainBiomass
        End Get
        Set(ByVal value As Boolean)

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

            m_mc.bRetainBiomass = value

        End Set
    End Property
    ''' <summary>
    ''' EwE5 Retain current Ecosim fishing rate pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseFishingPattern() As Boolean
        Get
            'see ToDo
            Return False
        End Get
        Set(ByVal value As Boolean)
            Debug.Assert(False, "UseFishingPattern Not implemented yet!")
        End Set
    End Property

    ''' <summary>
    ''' Sum of Squares fit to the currently loaded reference data for the current trial
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SS() As Single
        Get
            'Sum of Squares fit to the currently loaded reference data 
            'compute by Ecosim into its cEcosimDatastructures object for each trial
            Return m_core.m_EcoSimData.SS
        End Get
    End Property

    ''' <summary>
    ''' Sum of Squares fit to the currently loaded reference data for the original ecopath parameters
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SSorg() As Single
        Get
            'Sum of Squares fit to the currently loaded reference data 
            Return m_mc.SSorg
        End Get
    End Property



    ''' <summary>
    ''' Best fitting Sum of Squares to the currently loaded reference data for all the trials run to date
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property SSBestFit() As Single
        Get
            'Sum of Squares fit to the currently loaded reference data 
            Return m_mc.SSBestFit
        End Get
    End Property

    ''' <summary>
    ''' Number of attempts at finding a balanced Ecopath model for the current trial
    ''' </summary>
    Public ReadOnly Property nEcopathIterations() As Single
        Get
            Return m_mc.nEcopathIterations
        End Get
    End Property

    ''' <summary>
    ''' Number of trials performed in the currently running simulation
    ''' </summary>
    Public ReadOnly Property nTrialIterations() As Single
        Get
            Return m_mc.nTrialIterations
        End Get
    End Property


    ''' <summary>
    ''' MonteCarloEcopathDelegate to call at each attempt to find a balanced Ecopath model
    ''' </summary>
    ''' <remarks>Call to update an interface when Ecopath has been run</remarks>
    Public WriteOnly Property MonteCarloEcopathStepHandler() As MonteCarloEcopathProgressDelegate
        Set(ByVal value As MonteCarloEcopathProgressDelegate)
            Me.m_dlgMCEcopathStepHandler = value
        End Set
    End Property

    ''' <summary>
    ''' MonteCarloTrialDelegate to call at each Monte Carlo trial
    ''' </summary>
    ''' <remarks>This delegate is supplied by a user interface and will be called by the Monte Carlo object at the end of each Monte Carlo trial. 
    ''' It will tell an interface that a single trial has completed. </remarks>
    Public WriteOnly Property MonteCarloStepHandler() As MonteCarloTrialProgressDelegate
        Set(ByVal value As MonteCarloTrialProgressDelegate)
            Me.m_dlgMCTrialStepHandler = value
        End Set
    End Property

    ''' <summary>
    ''' Method to call in the interface when the Monte Carlo trials have completed.
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property MonteCarloCompletedHandler() As MonteCarloCompletedDelegate
        Set(ByVal value As MonteCarloCompletedDelegate)
            'the Monte Carlo object will call the manger and the manager will call the interface with this delegate
            'see MCCompletedHandler()
            Me.m_dlgMCCompletedHandler = value
        End Set
    End Property


    ''' <summary>
    ''' Synchronization object (Windows form) used for calling all the delegates across threads
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property SyncObject() As System.ComponentModel.ISynchronizeInvoke
        Set(ByVal value As System.ComponentModel.ISynchronizeInvoke)
            m_SyncObject = value
        End Set
    End Property


    Public WriteOnly Property EcosimTimeStepHandler() As EcoSimTimeStepDelegate
        Set(ByVal value As EcoSimTimeStepDelegate)
            'save the delegate for use with the bShowPlot flag
            '  m_EcosimTimeStepHandler = value
            'Changed this to pass the delegate directly to the monte carlo model
            'it will decide when to turn the plotting on or off
            m_mc.EcosimTimeStep = value
        End Set
    End Property


    Public ReadOnly Property MaxEcoPathInterations() As Integer
        Get
            'ToDo_jb montecarlo manager this should come from the monte carlo model
            Return 2000
        End Get
    End Property


    ''' <summary>
    ''' Monte Carlo group information
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Groups(ByVal iGroup As Integer) As cMonteCarloGroup
        Get
            Return m_lstGrps.Item(iGroup - 1)
        End Get
    End Property


    Public Sub CalculateUpperLowerLimits()

        Try
            Me.update()
            Me.m_mc.CalculateUpperLowerLimits(False)
            Me.LoadGroups()
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

#End Region

#Region "Private methods"

    Friend Sub LoadGroups()

        Try
            Dim m_epdata As cEcopathDataStructures = m_core.m_EcoPathData
            Dim m_esdata As cEcosimDatastructures = m_core.m_EcoSimData

            For Each grp As cMonteCarloGroup In m_lstGrps

                grp.AllowValidation = False

                'convert the Database ID into an iGroup
                grp.Index = Array.IndexOf(m_epdata.GroupDBID, grp.DBID)
                grp.Resize()

                grp.Name = m_epdata.GroupName(grp.Index)

                'data from Ecopath
                grp.B = m_epdata.B(grp.Index)
                grp.PB = m_epdata.PB(grp.Index)
                grp.QB = m_epdata.QB(grp.Index)
                grp.BA = m_epdata.BA(grp.Index)
                grp.EE = m_epdata.EE(grp.Index)
                grp.VU = m_esdata.VulnerabilityPredator(grp.Index)

                'ReDim CVpar(5, m_core.nGroups)
                grp.Bcv = m_mc.CVpar(eMCParams.Biomass, grp.Index)
                grp.PBcv = m_mc.CVpar(eMCParams.PB, grp.Index)
                grp.QBcv = m_mc.CVpar(eMCParams.QB, grp.Index)
                grp.BAcv = m_mc.CVpar(eMCParams.BA, grp.Index)
                grp.EEcv = m_mc.CVpar(eMCParams.EE, grp.Index)
                grp.VUcv = m_mc.CVpar(eMCParams.Vulnerability, grp.Index)

                'ReDim ParLimit(1, 5, m_core.nGroups)
                grp.BLower = m_mc.ParLimit(0, eMCParams.Biomass, grp.Index)
                grp.PBLower = m_mc.ParLimit(0, eMCParams.PB, grp.Index)
                grp.QBLower = m_mc.ParLimit(0, eMCParams.QB, grp.Index)
                grp.BALower = m_mc.ParLimit(0, eMCParams.BA, grp.Index)
                grp.EELower = m_mc.ParLimit(0, eMCParams.EE, grp.Index)
                grp.VULower = m_mc.ParLimit(0, eMCParams.Vulnerability, grp.Index)

                grp.BUpper = m_mc.ParLimit(1, eMCParams.Biomass, grp.Index)
                grp.PBUpper = m_mc.ParLimit(1, eMCParams.PB, grp.Index)
                grp.QBUpper = m_mc.ParLimit(1, eMCParams.QB, grp.Index)
                grp.BAUpper = m_mc.ParLimit(1, eMCParams.BA, grp.Index)
                grp.EEUpper = m_mc.ParLimit(1, eMCParams.EE, grp.Index)
                grp.VUUpper = m_mc.ParLimit(1, eMCParams.Vulnerability, grp.Index)

                grp.Bbf = 0
                grp.PBbf = 0
                grp.QBbf = 0
                grp.BAbf = 0
                grp.EEbf = 0
                grp.VUbf = 0

                grp.ResetStatusFlags()

                grp.AllowValidation = True

            Next 'For Each grp As cMonteCarloGroup

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException("LoadGroupParameters", ex)
        End Try


    End Sub

    ''' <summary>
    ''' Update the Monte Carlo groups with the best fit of the trials
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadBestFitsToGroups()

        Try
            Dim m_epdata As cEcopathDataStructures = m_core.m_EcoPathData
            Dim m_ecopath As cEcoPathModel = m_core.m_EcoPath

            For Each grp As cMonteCarloGroup In m_lstGrps

                grp.AllowValidation = False

                'convert the Database ID into an iGroup
                grp.Index = Array.IndexOf(m_epdata.GroupDBID, grp.DBID)
                grp.Resize()

                'best fit data from the monte carlo trials
                grp.Bbf = m_mc.BestFit(eMCParams.Biomass, grp.Index)
                grp.PBbf = m_mc.BestFit(eMCParams.PB, grp.Index)
                grp.QBbf = m_mc.BestFit(eMCParams.QB, grp.Index)
                grp.BAbf = m_mc.BestFit(eMCParams.BA, grp.Index)
                grp.EEbf = m_mc.BestFit(eMCParams.EE, grp.Index)
                grp.VUbf = m_mc.BestFit(eMCParams.Vulnerability, grp.Index)

                'ReDim CVpar(5, m_core.nGroups)
                grp.Bcv = m_mc.CVpar(eMCParams.Biomass, grp.Index)
                grp.PBcv = m_mc.CVpar(eMCParams.PB, grp.Index)
                grp.QBcv = m_mc.CVpar(eMCParams.QB, grp.Index)
                grp.BAcv = m_mc.CVpar(eMCParams.BA, grp.Index)
                grp.EEcv = m_mc.CVpar(eMCParams.EE, grp.Index)
                grp.VUcv = m_mc.CVpar(eMCParams.Vulnerability, grp.Index)

                'ReDim ParLimit(1, 5, m_core.nGroups)
                grp.BLower = m_mc.ParLimit(0, eMCParams.Biomass, grp.Index)
                grp.PBLower = m_mc.ParLimit(0, eMCParams.PB, grp.Index)
                grp.QBLower = m_mc.ParLimit(0, eMCParams.QB, grp.Index)
                grp.BALower = m_mc.ParLimit(0, eMCParams.BA, grp.Index)
                grp.EELower = m_mc.ParLimit(0, eMCParams.EE, grp.Index)
                grp.VULower = m_mc.ParLimit(0, eMCParams.Vulnerability, grp.Index)

                grp.BUpper = m_mc.ParLimit(1, eMCParams.Biomass, grp.Index)
                grp.PBUpper = m_mc.ParLimit(1, eMCParams.PB, grp.Index)
                grp.QBUpper = m_mc.ParLimit(1, eMCParams.QB, grp.Index)
                grp.BAUpper = m_mc.ParLimit(1, eMCParams.BA, grp.Index)
                grp.EEUpper = m_mc.ParLimit(1, eMCParams.EE, grp.Index)
                grp.VUUpper = m_mc.ParLimit(1, eMCParams.Vulnerability, grp.Index)


                'validation for monte carlo groups should be handled by the manager
                'the manager and the monte carlo model know what to do in response to an edit
                'this is not setup yet
                grp.AllowValidation = False

                grp.ResetStatusFlags()

            Next 'For Each grp As cMonteCarloGroup

            Me.AddMessage(New cMessage("MC groups updated", eMessageType.DataModified, eMessageSource.EcoSim, eMessageImportance.Maintenance))

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException("UpdateGroupsBestFit", ex)
        End Try


    End Sub

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
