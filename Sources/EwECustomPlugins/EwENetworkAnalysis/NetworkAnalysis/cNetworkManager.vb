'==============================================================================
'
' $Log: cNetworkManager.vb,v $
' Revision 1.7  2009/01/16 18:30:33  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.6  2008/11/28 16:54:56  joeb
' Cleaned up ToDo's
'
' Revision 1.5  2008/11/24 18:11:07  jeroens
' Electivity exposed
'
' Revision 1.4  2008/11/13 19:34:14  joeh
' Fix the error in the calculation of total ascendency
'
' Revision 1.3  2008/11/11 21:35:20  joeb
' Moved FunctionKemptonsQ to cCore.EcoFunctions.KemptonsQ so it would be accessible for the Seach functions
'
' Revision 1.2  2008/11/10 06:30:30  jeroens
' No need to assert
'
' Revision 1.1  2008/09/26 07:31:00  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Option Explicit On

Imports EwECore
Imports System.Xml

''' <summary>
''' Manager for the Network Analysis
''' </summary>
''' <remarks>This object is used to coordinate running of the Network Analysis and population of the ouput. </remarks>
Public Class cNetworkManager

#Region "Events"

    ''' <summary>
    ''' Progress of the RunMainNetwork() method. The total number of time this will be fired is not known in advance. 
    ''' So it simmply indicates progress.
    ''' </summary>
    ''' <param name="iProgress"></param>
    ''' <remarks></remarks>
    Public Event RunMainNetworkProgress(ByVal iProgress As Integer)

    ''' <summary>
    ''' Progress of the RunMainNetwork() method. A cycle has been found.
    ''' </summary>
    ''' <param name="iCycle"></param>
    ''' <remarks>The number of cycles is not known in advance</remarks>
    Public Event CycleFound(ByVal iCycle As Integer)

    ''' <summary>
    ''' Progress from one of the FindPathwaysxxx() methods
    ''' </summary>
    ''' <param name="iPath"></param>
    ''' <remarks>The number of pathways is not known in advance.</remarks>
    Public Event FindPathwaysProgress(ByVal iPath As Integer)

    ''' <summary>
    ''' Progress from one of the FindPathwaysxxx() methods
    ''' </summary>
    ''' <param name="iCycle"></param>
    ''' <remarks>The number of pathways is not known in advance.</remarks>
    Public Event FindCyclesProgress(ByVal iCycle As Integer)

    Public Event CalculateRequiredPPProgress(ByVal nPaths As Integer)

    ''' <summary>
    ''' Progress of Network Analysis for Ecosim
    ''' </summary>
    ''' <param name="iTime">Time step</param>
    ''' <remarks>The total number of time steps </remarks>
    Public Event EcosimNetworkProgress(ByVal iTime As Integer)


#End Region

#Region "Private data"

    Private m_HideGroupsForm As frmHideGroups  'joeh

    Private Enum ePathways
        ''' <summary>TL1->Consumer </summary>
        ToConsumer = 1
        ''' <summary>TL1->Prey->Consumer </summary>
        ToConsumerViaPrey = 2
        ''' <summary>Prey->Top Predator </summary>
        FromPrey = 3
        ''' <summary>Cycles</summary>
        LinkedPathways = 4
        ''' <summary>All Cycles </summary>
        All = 14
    End Enum


    ''' <summary>
    ''' State infomation for the core set in CoreStateMonitor_CoreExecutionStateEvent(...)
    ''' </summary>
    ''' <remarks>
    ''' This is the state of the core as it relates to the Network Analysis. 
    ''' It is hierarchical
    ''' </remarks>
    Private Enum eRunState
        CoreNotReady
        NetworkNeedsToRun
        NetworkHasRun
        RequirePPHasRun

        ''' <summary>Ecoism has loaded a scenario. Ecosim network can initialize. </summary>
        EcosimIsLoaded
        ''' <summary>Ecosim network has been initialized. </summary>
        EcosimNetworkInitialized

    End Enum

    Private m_EcoNetwork As cEcoNetwork
    Private m_core As cCore
    Private m_messagesource As eCoreComponentType = eCoreComponentType.Plugin
    Private m_runstate As eRunState
    Private m_epdata As cEcopathDataStructures
    Private m_esdata As cEcosimDatastructures

    Private m_IsMainNetworkRun As Boolean
    Private m_IsRequiredPrimaryProdRun As Boolean
    Private m_IsEcosimNetworkWithoutPPREstRun As Boolean
    Private m_IsEcosimNetworkWithPPREstRun As Boolean

    Private m_CancelRequiredPrimaryProdRun As Boolean

    ''' <summary>
    ''' Run network analysis for ecosim
    ''' </summary>
    ''' <remarks>in EwE5 this is called IndicesOn</remarks>
    Private m_bEcosimNetwork As Boolean

    ''' <summary>List of iGroups that have fish catch </summary>
    Dim lstCatch As New List(Of Integer)

    Private m_publisher As cMessagePublisher

    Private WithEvents CoreStateMonitor As cCoreStateMonitor

#End Region

#Region "Construction and initialization"

    Friend Function Init(ByRef theCore As cCore) As Boolean

        m_core = theCore
        CoreStateMonitor = m_core.StateMonitor
        m_publisher = theCore.Messages
        m_EcoNetwork = New cEcoNetwork(Me)
        Return True

    End Function


    Public Sub New()
        m_runstate = eRunState.CoreNotReady
    End Sub

#End Region

#Region "Public Methods for running models"

    '''' <summary>
    '''' Interface for testing Network Analysis routines from the Plugin
    '''' </summary>
    '''' <remarks>Temp just for debugging</remarks>
    'Public Sub Test()

    '    Try

    '        Me.RunMainNetwork()
    '        Me.FindPathwaysToConsumer(1)

    '    Catch ex As Exception
    '        cLog.Write(ex)
    '        Debug.Assert(False, ex.ToString)
    '    End Try

    'End Sub
#Region "Main Network Analysis"


    ''' <summary>
    ''' Run the Main Network Analysis routines
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This populates the data for EwE5 'Trophic level decomposition', 'Flow and biomass', 'Mixed Trophic impact', 'Acendency' and 'Flow form detritus' tabs</remarks>
    Public Function RunMainNetwork() As Boolean

        Dim breturn As Boolean
        Debug.Assert(m_EcoNetwork IsNot Nothing)

        m_runstate = eRunState.NetworkNeedsToRun

        If m_EcoNetwork Is Nothing Then
            'message of some sort
            m_publisher.SendMessage(New cMessage("Network Analysis not initialized properly.", eMessageType.ErrorEncountered, m_messagesource, eMessageImportance.Warning))
            Return False
        End If

        If m_runstate <> eRunState.CoreNotReady Then
            Try

                m_runstate = eRunState.NetworkNeedsToRun
                'jb probable don't need to reset the instance each time but this is robust
                Dim frmHide As frmHideGroups = frmHideGroups.GetInstance(Me)
                frmHide.Init() 'load the groupname and fleetname
                m_EcoNetwork.HideGroupsForm() = frmHide


                'Make sure the network analysis object has the latest data computed by the core
                'This may not be necessary because m_EcoNetwork keeps a reference to the data. 
                'However, this is more robust, incase the core has created a new m_EcoPathData object.
                m_EcoNetwork.EcopathData = m_epdata
                m_EcoNetwork.RunNetworkAnalysis()

                m_runstate = eRunState.NetworkHasRun

                breturn = True
                m_IsMainNetworkRun = True

            Catch ex As Exception
                cLog.Write(ex)
                Dim msg As String = Me.unravelExceptionMessage(ex)
                m_publisher.SendMessage(New cMessage(Me.ToString & ".RunMainNetwork() Error " & msg, eMessageType.ErrorEncountered, m_messagesource, eMessageImportance.Critical))
                'Debug.Assert(False, msg)
                breturn = False
            End Try
        Else
            ''message of some sort
            m_publisher.SendMessage(New cMessage("Network Analysis can not be run before Ecopath.", eMessageType.StateNotMet, m_messagesource, eMessageImportance.Warning))
            breturn = False
        End If

        Return breturn

    End Function

    'Bug 252 fix by joeh
    'Cahnge
    'Public Function IsMainNetworkRun() As Boolean
    '    Return m_IsMainNetworkRun
    'End Function
    Public Property IsMainNetworkRun() As Boolean
        Get
            Return m_IsMainNetworkRun
        End Get
        Set(ByVal value As Boolean)
            m_IsMainNetworkRun = value
        End Set
    End Property
    'End Change
#End Region

#Region "Required PP"
    ''' <summary>
    ''' Run the Require Primary Procuction models
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This popluates data for the EwE5 tabs 'Primary prod. required'-'For harvest of all groups' and 'For consumption of all groups'</remarks>
    Public Function RunRequiredPrimaryProd() As Boolean
        Dim breturn As Boolean
        Debug.Assert(m_EcoNetwork IsNot Nothing)

        If m_EcoNetwork Is Nothing Then
            'message of some sort
            m_core.Messages.SendMessage(New cMessage("Network Analysis not initialized properly.", eMessageType.ErrorEncountered, m_messagesource, eMessageImportance.Critical))
            Return False
        End If

        If m_runstate <> eRunState.CoreNotReady Then
            Try
                'For Primary Prod to run the main network routines need to have run first
                'm_runstate is set by the core's statemonitor events see CoreStateMonitor_CoreExecutionStateEvent()
                If m_runstate < eRunState.NetworkHasRun Then
                    'implicitly run the network analysis if it has not been run
                    If Not Me.RunMainNetwork() Then
                        'ooopssss........
                        m_core.Messages.SendMessage(New cMessage("Required Primary Production could not be run because of a problem in Network Analysis.", eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Critical))
                        Return False
                    End If
                End If

                'Debug.Assert(m_runstate = eRunState.NetworkHasRun)

                m_EcoNetwork.CalculateRequiredPP()

                m_runstate = eRunState.RequirePPHasRun

                breturn = True
                m_IsRequiredPrimaryProdRun = True

            Catch ex As Exception
                cLog.Write(ex)
                Dim msg As String = Me.unravelExceptionMessage(ex)
                m_core.Messages.SendMessage(New cMessage(Me.ToString & ".RunReguiredPrimaryProd() Error " & msg, eMessageType.ErrorEncountered, eCoreComponentType.EcoPath, eMessageImportance.Critical))

                breturn = False
            End Try
        Else
            'message of some sort
            m_core.Messages.SendMessage(New cMessage("Required Primary Production can not be run.", eMessageType.StateNotMet, m_messagesource, eMessageImportance.Warning))
            breturn = False
        End If

        Return breturn

    End Function

    'Bug 252 fix by joeh
    'Change
    'Public Function IsRequiredPrimaryProdRun() As Boolean
    '    Return m_IsRequiredPrimaryProdRun
    'End Function
    Public Property IsRequiredPrimaryProdRun() As Boolean
        Get
            Return m_IsRequiredPrimaryProdRun
        End Get
        Set(ByVal value As Boolean)
            m_IsRequiredPrimaryProdRun = value
        End Set
    End Property
    'End change

#End Region

#Region "Pathways"

    ''' <summary>
    ''' TL1-->Consumer
    ''' </summary>
    ''' <param name="iToGroup"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysToConsumer(ByVal iToGroup As Integer) As Boolean
        Dim nPaths As Integer, nArrows As Integer

        Try
            m_EcoNetwork.FindCycles(m_epdata.DC, ePathways.ToConsumer, iToGroup, 0, nPaths, nArrows)
            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try


    End Function

    ''' <summary>
    ''' TL1-->Prey-->Consumer
    ''' </summary>
    ''' <param name="iToGroup"></param>
    ''' <param name="iViaGroup"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysToConsumerViaPrey(ByVal iToGroup As Integer, ByVal iViaGroup As Integer) As Boolean
        Dim nPaths As Integer, nArrows As Integer

        Try
            m_EcoNetwork.FindCycles(m_epdata.DC, ePathways.ToConsumerViaPrey, iToGroup, iViaGroup, nPaths, nArrows)
            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

    End Function

    ''' <summary>
    ''' Prey-->Top Predator
    ''' </summary>
    ''' <param name="iFromGroup"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysFromPrey(ByVal iFromGroup As Integer) As Boolean
        Dim nPaths As Integer, nArrows As Integer

        Try
            m_EcoNetwork.FindCycles(m_epdata.DC, ePathways.FromPrey, 1, iFromGroup, nPaths, nArrows)
            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

    End Function

    ''' <summary>
    ''' Cycles(excl. detitus)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysCycles() As Boolean
        Dim nPaths As Integer, nArrows As Integer

        Try
            'ToDo_jb FindPathwaysCycles EwE5 calls InitCyclesList ????? I can not find this again
            m_EcoNetwork.FindCycles(m_epdata.DC, ePathways.LinkedPathways, 1, 1, nPaths, nArrows)
            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

    End Function


    ''' <summary>
    ''' All cycles
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysCyclesAll() As Boolean
        Dim nPaths As Integer, nArrows As Integer

        Try
            m_EcoNetwork.FindCycles(m_epdata.DC, ePathways.All, 1, 1, nPaths, nArrows)
            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

    End Function

    ''' <summary>
    ''' Primary producer required
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysPPR() As Boolean

        Try
            'm_EcoNetwork.FindPaths(
            Return True
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

    End Function
#End Region

#Region "Network from Ecosim"

    ''' <summary>
    ''' Run Ecosim and compute the ecosim network analysis data 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function RunEcosimNetwork() As Boolean

        Try

            m_bEcosimNetwork = True

            If Not m_core.StateMonitor.HasEcosimLoaded Then
                'No Ecosim Scenario is loaded the Ecosim network analysis can not be run
                m_core.Messages.SendMessage(New cMessage("Please load an Ecosim scenario before running Network Analysis for Ecosim.", _
                         eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Critical))

                m_bEcosimNetwork = False

                Return False
            End If

            m_core.RunEcoSim()

            If bEcosimPPR Then
                m_IsEcosimNetworkWithPPREstRun = True
            Else
                m_IsEcosimNetworkWithoutPPREstRun = True
            End If

            m_bEcosimNetwork = False

        Catch ex As Exception
            cLog.Write(ex)
            m_bEcosimNetwork = False
            m_core.Messages.SendMessage(New cMessage("Error while running Network Analysis for Ecosim. " & ex.Message, _
                                            eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Critical))
            Return False
        End Try

        Return True

    End Function

    'Bug 252 fix by joeh
    'Change
    'Public Function IsEcosimNetworkWithPPREstRun() As Boolean
    '    Return m_IsEcosimNetworkWithPPREstRun
    'End Function
    Public Property IsEcosimNetworkWithPPREstRun() As Boolean
        Get
            Return m_IsEcosimNetworkWithPPREstRun
        End Get
        Set(ByVal value As Boolean)
            m_IsEcosimNetworkWithPPREstRun = value
        End Set
    End Property
    'End change

    'Bug 252 fix by joeh
    'Change
    'Public Function IsEcosimNetworkWithoutPPREstRun() As Boolean
    '    Return m_IsEcosimNetworkWithoutPPREstRun
    'End Function
    Public Property IsEcosimNetworkWithoutPPREstRun() As Boolean
        Get
            Return m_IsEcosimNetworkWithoutPPREstRun
        End Get
        Set(ByVal value As Boolean)
            m_IsEcosimNetworkWithoutPPREstRun = value
        End Set
    End Property
    'End change

    ''' <summary>
    ''' Initialize Ecosim Network Analysis
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InitNetworkForEcosim() As Boolean

        Try

            'don't do anything if the Ecosim Network Analysis is turned off
            If Not m_bEcosimNetwork Then
                Return False
            End If

            'If m_runstate < eRunState.EcosimIsLoaded Then
            If Not m_core.StateMonitor.HasEcosimLoaded Then
                'No Ecosim Scenario is loaded this can not be initialized
                m_core.Messages.SendMessage(New cMessage("Network Analysis for Ecosim could not be initialized because an Ecosim scenario has not been loaded.", _
                         eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Critical))
                Return False
            End If

            'm_runstate is set by the core's statemonitor events see CoreStateMonitor_CoreExecutionStateEvent()
            If m_runstate < eRunState.NetworkHasRun Then
                'implicitly run the network analysis if it has not been run
                If Not Me.RunMainNetwork() Then
                    'ooopssss........
                    m_core.Messages.SendMessage(New cMessage("Network Analysis for Ecosim could not be initialized because of a problem in Network Analysis.", eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Critical))
                    Return False
                End If
            End If

            m_EcoNetwork.EcopathData = m_epdata
            m_EcoNetwork.EcosimData = m_esdata

            m_EcoNetwork.InitForEcosim()
            m_runstate = eRunState.EcosimNetworkInitialized


        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".InitNetworkForEcosim " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".InitNetworkForEcosim() Error: " & ex.Message, ex)
        End Try


    End Function

    ''' <summary>
    ''' Compute network analysis for ecosim at this time step
    ''' </summary>
    ''' <param name="BiomassAtTimestep"></param>
    ''' <param name="EcosimDatastructures"></param>
    ''' <param name="iTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EcosimTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As cEcosimDatastructures, ByVal iTime As Integer) As Boolean

        Try

            'don't do anything if the Ecosim Network Analysis is turned off
            If Not m_bEcosimNetwork Then
                Return False
            End If

            If m_runstate < eRunState.EcosimNetworkInitialized Then
                'do not try to run this if it has not been initialized
                'no messages here so that this does not slow down Ecosim
                Return False
            End If

            'do ecosim network calculation for this time step
            m_EcoNetwork.EcosimTimestep(BiomassAtTimestep, EcosimDatastructures, iTime)
            'tell the world that a time step has been computed
            'If (iTime Mod 10) = 0 Then RaiseEvent EcosimNetworkProgress(iTime)
            RaiseEvent EcosimNetworkProgress(iTime)


        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.ToString)
            Throw New ApplicationException(Me.ToString & ".EcosimTimeStep() Error: " & ex.Message, ex)
        End Try

    End Function



#End Region

#End Region

#Region "Public Properties"

#Region "Inputs"

    ''' <summary>
    ''' Ecopath data to run the analysis on
    ''' </summary>
    ''' <remarks>This is set by plugin (cEwENetworkAnalysisPlugin) each time the core fire the EcopathRunCompleted() Plugin point.</remarks>
    Public Property EcopathData() As cEcopathDataStructures
        Get
            Return m_epdata
        End Get
        Set(ByVal value As cEcopathDataStructures)
            m_epdata = value
        End Set
    End Property

    ''' <summary>
    ''' Ecopath data to run the analysis on
    ''' </summary>
    ''' <remarks></remarks>
    Public Property EcosimData() As cEcosimDatastructures
        Get
            Return m_esdata
        End Get
        Set(ByVal value As cEcosimDatastructures)
            m_esdata = value
        End Set
    End Property

    ''' <summary>
    ''' Run the network analysis for Ecosim (Ecosim Indicies)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>IndicesOn in EwE5. This is set implicitly by RunEcosimNetwork.
    '''  If this flag is false the plugin will not responed to the EcosimEndTimeStep() plugin point.</remarks>
    Public Property bEcosimNetwork() As Boolean
        Get
            Return m_bEcosimNetwork
        End Get
        Set(ByVal value As Boolean)
            m_bEcosimNetwork = value
        End Set
    End Property

    ''' <summary>
    ''' Run the Required Primary Production routines for ecosim
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is very time consuming</remarks>
    Public Property bEcosimPPR() As Boolean
        Get
            Return Me.m_EcoNetwork.PPRon
        End Get
        Set(ByVal value As Boolean)
            Me.m_EcoNetwork.PPRon = value
        End Set
    End Property


    Public ReadOnly Property Core() As cCore
        Get
            Return Me.m_core
        End Get
    End Property

#End Region

#Region "Model outputs"

#Region "Counters"

    Public ReadOnly Property nTrophicLevels() As Integer
        Get
            Return Me.m_EcoNetwork.NoTL
        End Get
    End Property

    Public ReadOnly Property nGroups() As Integer
        Get
            Return Me.m_core.nGroups
        End Get
    End Property

    Public ReadOnly Property GroupName(ByVal iGroup As Integer) As String
        Get
            Try
                Return Me.m_epdata.GroupName(iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return ""
            End Try
        End Get
    End Property

    Public ReadOnly Property nFleets() As Integer
        Get
            Return Me.m_core.nFleets
        End Get
    End Property

    Public ReadOnly Property FleetName(ByVal iFleet As Integer) As String
        Get
            Try
                Return Me.m_epdata.FleetName(iFleet)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return ""
            End Try
        End Get
    End Property
#End Region

#Region "Pathways"

    ''' <summary>
    ''' EwE5 Cycles and Pathways
    ''' </summary>
    ''' <remarks>PathWays will contain new data on each call to FindPathwaysxxxxxx</remarks>
    Public ReadOnly Property PathWays() As List(Of String)
        Get
            Return Me.m_EcoNetwork.lstPathways
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Cycles and Pathways
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property NumArrows() As Integer
        Get
            Return Me.m_EcoNetwork.NumberArrows
        End Get
    End Property

#End Region

#Region "Flows"

    ''' <summary>
    ''' EwE5 Trophic level decomposition Relative Flows
    ''' </summary>
    Public ReadOnly Property RelativeFlow(ByVal iGroup As Integer, ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.AM(iTrophicLevel, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Absolute Flows
    ''' </summary>
    Public ReadOnly Property AbsoluteFlow(ByVal iGroup As Integer, ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.AM_Abs(iTrophicLevel, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Sum of Absolute Flows across all the groups for a trophic level
    ''' </summary>
    Public ReadOnly Property AbsoluteFlowTotal(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.QTL(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Used in computing Transfer Effeiciency
    ''' </summary>
    Public ReadOnly Property CA(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.CA(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Used in computing Transfer Effeiciency
    ''' </summary>
    Public ReadOnly Property CatchDetritus(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.CAD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Used in computing Transfer Effeiciency
    ''' </summary>
    Public ReadOnly Property FlowFromDetritus() As Single
        Get
            Return m_EcoNetwork.DetIndex
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Biomass by Trophic Level
    ''' </summary>
    Public ReadOnly Property BiomassByTrophicLevel(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.BbyTL(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Biomass by Group
    ''' </summary>
    Public ReadOnly Property BiomassByGroup(ByVal iGroupNum As Integer) As Single
        Get
            Return m_epdata.B(iGroupNum)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Catch by Trophic Level
    ''' </summary>
    Public ReadOnly Property CatchByTrophicLevel(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.CbyTL(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Catch by Group
    ''' </summary>
    Public ReadOnly Property CatchByGroup(ByVal iGroupNum As Integer) As Single
        Get
            Return m_epdata.fCatch(iGroupNum)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Mixed Trophic impact
    ''' </summary>
    Public ReadOnly Property MixedTrophicImpacts(ByVal iPred As Integer, ByVal iPrey As Integer) As Single
        Get
            Return m_EcoNetwork.MTI(iPred, iPrey)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Flow from detritus
    ''' </summary>
    Public ReadOnly Property FlowFromDetritus(ByVal iGroup As Integer) As Single
        Get
            Dim sumad As Single
            For itl As Integer = 1 To m_EcoNetwork.NoTL
                sumad += m_EcoNetwork.Ad(itl, iGroup)
            Next
            Return sumad
        End Get
    End Property
#End Region

#Region "Ascendancy"

#Region "By Group"

    Public ReadOnly Property AscendancyByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoNetwork.Ac(iGroup)
        End Get
    End Property

    Public ReadOnly Property OverheadByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoNetwork.Ec(iGroup)
        End Get
    End Property

    Public ReadOnly Property CapacityByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoNetwork.CC(iGroup)
        End Get
    End Property

    Public ReadOnly Property InformationByGroup(ByVal iGroup As Integer) As Single
        Get
            If m_EcoNetwork.TruPut > 0 Then
                Return m_EcoNetwork.Ac(iGroup) / m_EcoNetwork.TruPut
            Else
                Return cCore.NULL_VALUE
            End If
        End Get
    End Property

    Public ReadOnly Property ThroughputByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoNetwork.Q(iGroup)
        End Get
    End Property

    Public ReadOnly Property AscendencyTotal() As Single
        Get
            Return m_EcoNetwork.SumAc
        End Get
    End Property

    Public ReadOnly Property OverheadTotal() As Single
        Get
            Return m_EcoNetwork.SumEc
        End Get
    End Property

    Public ReadOnly Property CapacityTotal() As Single
        Get
            Return m_EcoNetwork.SumCc
        End Get
    End Property

    Public ReadOnly Property ThroughputTotal() As Single
        Get
            Return m_EcoNetwork.TruPut
        End Get
    End Property

    Public ReadOnly Property ThroughputCycledLiving() As Single
        Get
            Return m_EcoNetwork.Tc
        End Get
    End Property

    Public ReadOnly Property ThroughputCycledPredatory() As Single
        Get
            Return m_EcoNetwork.TCyc
        End Get
    End Property

    Public ReadOnly Property ThroughputCycledAll() As Single
        Get
            Return m_EcoNetwork.TcD
        End Get
    End Property

    Public ReadOnly Property ThroughputExport() As Single
        Get
            Return m_EcoNetwork.SumEx
        End Get
    End Property

    Public ReadOnly Property ThroughputResp() As Single
        Get
            Return m_EcoNetwork.SumResp
        End Get
    End Property

    Public ReadOnly Property ThroughputExportByGroup(ByVal iGroup As Integer) As Single
        Get
            Return Me.m_epdata.Ex(iGroup)
        End Get
    End Property
#End Region

#Region "Totals"

#Region "Ascendancy Flow"

    ''' <summary>
    ''' Ascendency total flow
    ''' </summary>
    Public ReadOnly Property AscendancyInternalFlowTotal() As Single
        Get
            Return m_EcoNetwork.Ai
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage flow
    ''' </summary>
    Public ReadOnly Property AscendancyInternalFlowPer() As Single
        Get
            Return m_EcoNetwork.Aip
        End Get
    End Property


    ''' <summary>
    ''' Ascendency total import
    ''' </summary>
    Public ReadOnly Property AscendancyImportTotal() As Single
        Get
            Return m_EcoNetwork.Ao
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage import
    ''' </summary>
    Public ReadOnly Property AscendancyImportPer() As Single
        Get
            Return m_EcoNetwork.Aop
        End Get
    End Property


    ''' <summary>
    ''' Ascendency total export
    ''' </summary>
    Public ReadOnly Property AscendancyExportTotal() As Single
        Get
            Return m_EcoNetwork.Ae
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage export
    ''' </summary>
    Public ReadOnly Property AscendancyExportPer() As Single
        Get
            Return m_EcoNetwork.Aep
        End Get
    End Property

    ''' <summary>
    ''' Ascendency total respiration
    ''' </summary>
    Public ReadOnly Property AscendancyRespTotal() As Single
        Get
            Return m_EcoNetwork.Ar
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage respiration
    ''' </summary>
    Public ReadOnly Property AscendancyRespPer() As Single
        Get
            Return m_EcoNetwork.Arp
        End Get
    End Property

    ''' <summary>
    ''' Ascendency total totals
    ''' </summary>
    Public ReadOnly Property AscendancyTotalsTotal() As Single
        Get
            Return m_EcoNetwork.Ascen
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage totals
    ''' </summary>
    Public ReadOnly Property AscendancyTotalsPer() As Single
        Get
            Return m_EcoNetwork.Ascp
        End Get
    End Property

#End Region

#Region "Overhead"

    ''' <summary>
    ''' Overhead flow total 
    ''' </summary>
    Public ReadOnly Property OverheadFlowTotal() As Single
        Get
            Return m_EcoNetwork.Ei
        End Get
    End Property

    ''' <summary>
    ''' Overhead Flow percentage 
    ''' </summary>
    Public ReadOnly Property OverheadFlowPer() As Single
        Get
            Return m_EcoNetwork.Eip
        End Get
    End Property


    ''' <summary>
    ''' Overhead total import
    ''' </summary>
    Public ReadOnly Property OverheadImportTotal() As Single
        Get
            Return m_EcoNetwork.Eo
        End Get
    End Property

    ''' <summary>
    ''' Overhead percentage import
    ''' </summary>
    Public ReadOnly Property OverheadImportPer() As Single
        Get
            Return m_EcoNetwork.Eop
        End Get
    End Property

    ''' <summary>
    ''' Overhead  Export total 
    ''' </summary>
    Public ReadOnly Property OverheadExportTotal() As Single
        Get
            Return m_EcoNetwork.Eee
        End Get
    End Property

    ''' <summary>
    ''' Overhead Export percentage 
    ''' </summary>
    Public ReadOnly Property OverheadExportPer() As Single
        Get
            Return m_EcoNetwork.Eep
        End Get
    End Property

    ''' <summary>
    ''' Overhead respiration  total 
    ''' </summary>
    Public ReadOnly Property OverheadRespTotal() As Single
        Get
            Return m_EcoNetwork.er
        End Get
    End Property

    ''' <summary>
    ''' Overhead respiration percentage 
    ''' </summary>
    Public ReadOnly Property OverheadRespPer() As Single
        Get
            Return m_EcoNetwork.Erp
        End Get
    End Property

    ''' <summary>
    ''' Overhead totals total 
    ''' </summary>
    Public ReadOnly Property OverheadTotalsTotal() As Single
        Get
            Return m_EcoNetwork.Overhead
        End Get
    End Property

    ''' <summary>
    ''' Overhead totals percentage 
    ''' </summary>
    Public ReadOnly Property OverheadTotalsPer() As Single
        Get
            Return m_EcoNetwork.Overp
        End Get
    End Property

#End Region

#Region "Capacity"

    ''' <summary>
    ''' Capacity  flow percentage 
    ''' </summary>
    Public ReadOnly Property CapacityFlowTotal() As Single
        Get
            Return m_EcoNetwork.Ci
        End Get
    End Property

    ''' <summary>
    ''' Capacity flow percentage 
    ''' </summary>
    Public ReadOnly Property CapacityFlowPer() As Single
        Get
            Return m_EcoNetwork.Cip
        End Get
    End Property


    ''' <summary>
    ''' Capacity total import
    ''' </summary>
    Public ReadOnly Property CapacityImportTotal() As Single
        Get
            Return m_EcoNetwork.Co
        End Get
    End Property

    ''' <summary>
    ''' Capacity percentage import
    ''' </summary>
    Public ReadOnly Property CapacityImportPer() As Single
        Get
            Return m_EcoNetwork.Cop
        End Get
    End Property

    ''' <summary>
    ''' Capacity export total
    ''' </summary>
    Public ReadOnly Property CapacityExportTotal() As Single
        Get
            Return m_EcoNetwork.Ce
        End Get
    End Property

    ''' <summary>
    ''' Capacity export precentage
    ''' </summary>
    Public ReadOnly Property CapacityExportPer() As Single
        Get
            Return m_EcoNetwork.Cep
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration total
    ''' </summary>
    Public ReadOnly Property CapacityRespTotal() As Single
        Get
            Return m_EcoNetwork.Cr
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration precentage
    ''' </summary>
    Public ReadOnly Property CapacityRespPer() As Single
        Get
            Return m_EcoNetwork.Crp
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration precentage
    ''' </summary>
    Public ReadOnly Property CapacityTotalsTotal() As Single
        Get
            Return m_EcoNetwork.Capacity
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration precentage
    ''' </summary>
    Public ReadOnly Property CapacityTotalsPer() As Single
        Get
            Return m_EcoNetwork.Capp
        End Get
    End Property

#End Region

#End Region

#End Region

#Region "Trophic Level"
    ''' <summary>
    ''' Flow and Biomass From primary prod. Import 
    ''' </summary>
    Public ReadOnly Property PPImport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.Impo(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Cons by Pred 
    ''' </summary>
    Public ReadOnly Property PPConsByPred(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.Predat(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Export
    ''' </summary>
    Public ReadOnly Property PPExport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.EXA(iTrophicLevel)
        End Get
    End Property


    ''' <summary>
    ''' Flow and Biomass From primary prod. Flow To Detritus
    ''' </summary>
    Public ReadOnly Property PPToDetritus(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.DTA(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Respiration
    ''' </summary>
    Public ReadOnly Property PPRespiration(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.RSP(iTrophicLevel)
        End Get
    End Property


    ''' <summary>
    ''' Flow and Biomass From primary prod. Throughtput
    ''' </summary>
    Public ReadOnly Property PPThroughtput(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.TRP(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Import 
    ''' </summary>
    Public ReadOnly Property DetImport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.ImpD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Cons by Pred 
    ''' </summary>
    Public ReadOnly Property DetConsByPred(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.PredatD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Export
    ''' </summary>
    Public ReadOnly Property DetExport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.EXAD(iTrophicLevel)
        End Get
    End Property


    ''' <summary>
    ''' Flow and Biomass From detritus. Flow To Detritus
    ''' </summary>
    Public ReadOnly Property DetToDetritus(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.DTAD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Respiration
    ''' </summary>
    Public ReadOnly Property DetRespiration(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.RSPD(iTrophicLevel)
        End Get
    End Property


    ''' <summary>
    ''' Flow and Biomass From detritus. Throughtput
    ''' </summary>
    Public ReadOnly Property DetThroughtput(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.TRPD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Throughtput shown
    ''' </summary>
    Public ReadOnly Property ThroughtputShow(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.TrpShow(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Throughtput shown
    ''' </summary>
    Public Property TrEm1(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_EcoNetwork.TrEm1(iTrophicLevel)
        End Get
        Set(ByVal value As Single)
            m_EcoNetwork.TrEm1(iTrophicLevel) = value
        End Set
    End Property

    ''' <summary>
    ''' Flow and Biomass From all combined. Extracted to break cycles
    ''' </summary>
    Public ReadOnly Property ExtractedToBreakCycles() As Single
        Get
            Return m_EcoNetwork.AmCyc
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From all combined. Input TLII+
    ''' </summary>
    Public ReadOnly Property InputTLIIPlus() As Single
        Get
            Return m_EcoNetwork.SumIm
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From all combined. Total throughput
    ''' </summary>
    Public ReadOnly Property TotalThroughput() As Single
        Get
            Return m_EcoNetwork.TotalTrp
        End Get
    End Property
#End Region

#Region " Indicators "

    Public ReadOnly Property Electivity(ByVal iSel As Integer, ByVal iPrey As Integer, ByVal iTime As Integer) As Single
        Get
            Return Me.m_EcoNetwork.Elect(iSel, iPrey, iTime)
        End Get
    End Property

#End Region
#Region "Primary Production Required"

    Public ReadOnly Property nCatch() As Integer
        Get
            'count the number of groups with that have fish catch
            lstCatch.Clear()
            Dim n As Integer
            For igrp As Integer = 1 To m_epdata.NumLiving
                If m_epdata.fCatch(igrp) <> 0 Then
                    n += 1
                End If
            Next igrp
            Return n

        End Get
    End Property

    ''' <summary>
    ''' List of Groups that have fish catch
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is the iGroup of groups that make up the EwE5 Primary Prod.required "For harvest of all groups" grid  </remarks>
    Public ReadOnly Property CatchGroups() As List(Of Integer)

        Get

            For igrp As Integer = 1 To m_epdata.NumLiving
                If m_epdata.fCatch(igrp) <> 0 Then
                    lstCatch.Add(igrp)
                End If
            Next igrp
            Return lstCatch
        End Get

    End Property

    ''' <summary>
    ''' EwE5 No.of paths
    ''' </summary>
    Public ReadOnly Property NumerPaths(ByVal iGroup As Integer) As Integer
        Get
            Return m_EcoNetwork.NumPath(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 TL
    ''' </summary>
    Public ReadOnly Property TrophicLevel(ByVal iGroup As Integer) As Single
        Get
            Return m_epdata.TTLX(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 totalPP
    ''' </summary>
    Public ReadOnly Property TotalPrimaryProduction() As Single
        Get
            Return m_EcoNetwork.totalPP
        End Get
    End Property

#Region "For consumption of all groups"

    ''' <summary>
    ''' EwE5 PPR(PP)
    ''' </summary>
    Public ReadOnly Property PPRRequired(ByVal iGroup As Integer) As Single
        Get
            'Return m_epdata.TTLX(iGroup)
            Return m_EcoNetwork.SumPPRequired(1, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR(det)
    ''' </summary>
    Public ReadOnly Property PPRRequiredDet(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoNetwork.SumDetRequired(1, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR (= PPRRequired + PPRRequiredDet)
    ''' </summary>
    Public ReadOnly Property PPRRequiredSum(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequired(iGroup) + PPRRequiredDet(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Cons
    ''' </summary>
    Public ReadOnly Property PPRCons(ByVal iGroup As Integer) As Single
        Get
            Return m_epdata.B(iGroup) * m_epdata.QB(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/cons
    ''' </summary>
    Public ReadOnly Property PPROverCons(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequiredSum(iGroup) / PPRCons(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/TotPP(%)
    ''' </summary>
    Public ReadOnly Property PPRTotPP(ByVal iGroup As Integer) As Single
        Get
            Return CSng(100.0 * PPRRequiredSum(iGroup) / (m_EcoNetwork.totalPP + m_EcoNetwork.TRPD(1)))
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/u.biom.
    ''' </summary>
    Public ReadOnly Property PPRU(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequiredSum(iGroup) / (m_EcoNetwork.totalPP + m_EcoNetwork.TRPD(1)) / m_epdata.B(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 NumLivPath.
    ''' </summary>
    Public ReadOnly Property NumLivPath() As Single
        Get
            Return m_EcoNetwork.NumLivPath
        End Get
    End Property

    ''' <summary>
    ''' EwE5 NumDetPath.
    ''' </summary>
    Public ReadOnly Property NumDetPath() As Single
        Get
            Return m_EcoNetwork.NumDetPath
        End Get
    End Property

#End Region

#Region "For harvest of all groups"
    ''' <summary>
    ''' EwE5 PPR(PP)
    ''' </summary>
    Public ReadOnly Property PPRRequiredHarvest(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoNetwork.SumPPRequired(0, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR(det)
    ''' </summary>
    Public ReadOnly Property PPRRequiredDetHarvest(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoNetwork.SumDetRequired(0, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR (= PPRRequired + PPRRequiredDet)
    ''' </summary>
    Public ReadOnly Property PPRRequiredSumHarvest(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequiredHarvest(iGroup) + PPRRequiredDetHarvest(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Catch
    ''' </summary>
    Public ReadOnly Property PPRCatchHarvest(ByVal iGroup As Integer) As Single
        Get
            Return m_epdata.fCatch(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/catch
    ''' </summary>
    Public ReadOnly Property PPROverCatchHarvest(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequiredSumHarvest(iGroup) / PPRCatchHarvest(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/TotPP(%)
    ''' </summary>
    Public ReadOnly Property PPRTotPPHarvest(ByVal iGroup As Integer) As Single
        Get
            Return CSng(100.0 * PPRRequiredSumHarvest(iGroup) / (m_EcoNetwork.totalPP + m_EcoNetwork.TRPD(1)))
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/u.catch
    ''' </summary>
    Public ReadOnly Property PPRUHarvest(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequiredSumHarvest(iGroup) / (m_EcoNetwork.totalPP + m_EcoNetwork.TRPD(1)) / m_epdata.fCatch(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 RaiseToPP
    ''' </summary>
    Public ReadOnly Property TotalTL() As Single
        Get
            Return m_epdata.TLcatch
        End Get
    End Property

    ''' <summary>
    ''' EwE5 RaiseToPP
    ''' </summary>
    Public ReadOnly Property TotalPPRPP() As Single
        Get
            Return m_EcoNetwork.RaiseToPP(0)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 RaiseToDet
    ''' </summary>
    Public ReadOnly Property TotalPPRDet() As Single
        Get
            Return m_EcoNetwork.RaiseToDet(0)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 totalCatch
    ''' </summary>
    Public ReadOnly Property TotalCatch() As Single
        Get
            Return m_EcoNetwork.totalCatch
        End Get
    End Property

#End Region

#End Region

#Region "Ecosim Public Properties"

    Public ReadOnly Property nEcosimTimesteps() As Integer
        Get
            Return m_core.nEcosimTimeSteps
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Ecosim plot "FIB index"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FIB() As Single()
        Get
            Return Me.m_EcoNetwork.FIB
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Ecosim plot "Total catch "
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RelativeSumOfCatchPlot() As Single()
        Get
            Return Me.m_EcoNetwork.RelativeSumOfCatchPlot
        End Get
    End Property


    ''' <summary>
    ''' EwE5 Ecosim plot "Kemptons Q"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RelativeKemptonsPlot() As Single()
        Get
            Return Me.m_EcoNetwork.RelativeKemptonsPlot
        End Get
    End Property

    ''' <summary>
    '''  EwE5 Ecosim plot "TL of catch "
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TLCatchPlot() As Single()
        Get
            Return Me.m_EcoNetwork.TLCatchPlot
        End Get
    End Property

    ''' <summary>
    '''  EwE5 Ecosim plot TL (trophic level of all groups)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TLSimPlot(ByVal iGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Return Me.m_EcoNetwork.TLSimPlot(iGroup, iTime)
        End Get
    End Property


    ''' <summary>
    ''' EwE5 Ecosim plot "Catch PPR "
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RelativeCatchPPRPlot() As Single()
        Get
            Return Me.m_EcoNetwork.RelativeCatchPPR
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Ecosim plot "Catch detritus req."
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RelativeDetritusReqPlot() As Single()
        Get
            Return Me.m_EcoNetwork.RelativeCatchDetReq
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Ecosim csv parameter "TruPut"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ThroughputEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.Throughput
        End Get
    End Property

    Public ReadOnly Property CapacityEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.CapacityEcosim
        End Get
    End Property

    Public ReadOnly Property AscendImportEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.AscendImport
        End Get
    End Property

    Public ReadOnly Property AscendFlowEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.AscendFlow
        End Get
    End Property

    Public ReadOnly Property AscendExportEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.AscendExport
        End Get
    End Property

    Public ReadOnly Property AscendRespEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.AscendResp
        End Get
    End Property

    Public ReadOnly Property OverheadImportEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.OverheadImport
        End Get
    End Property

    Public ReadOnly Property OverheadFlowEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.OverheadFlow
        End Get
    End Property

    Public ReadOnly Property OverheadExportEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.OverheadExport
        End Get
    End Property

    Public ReadOnly Property OverheadRespEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.OverheadResp
        End Get
    End Property

    Public ReadOnly Property PCIEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.PCI
        End Get
    End Property

    Public ReadOnly Property FCIEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.FCI
        End Get
    End Property

    Public ReadOnly Property PathLengthEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.PathLength
        End Get
    End Property

    Public ReadOnly Property ExportEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.Export
        End Get
    End Property

    Public ReadOnly Property RespEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.Resp
        End Get
    End Property

    Public ReadOnly Property PrimaryProdEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.PrimaryProd
        End Get
    End Property

    Public ReadOnly Property ProdEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.Prod
        End Get
    End Property

    Public ReadOnly Property BiomassEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.Biomass
        End Get
    End Property

    Public ReadOnly Property CatchEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.CatchEcosim
        End Get
    End Property

    Public ReadOnly Property PropFlowDetEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.PropFlowDet
        End Get
    End Property

    Public ReadOnly Property RaiseToPPEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.RaiseToPPEcosim
        End Get
    End Property

    Public ReadOnly Property RaiseToDetEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.RaiseToDetEcosim
        End Get
    End Property

    Public ReadOnly Property AscendTotalEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.Ascendency
        End Get
    End Property

    Public ReadOnly Property AMIEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.AMI
        End Get
    End Property

    Public ReadOnly Property EntropyEcosim() As Single()
        Get
            Return Me.m_EcoNetwork.Entropy
        End Get
    End Property

#End Region

#End Region

#End Region

#Region "Misc private methods"

    ''' <summary>
    ''' Build a string that contains the messages from this exception and all it's InnerException's
    ''' </summary>
    ''' <param name="theEX">Exception </param>
    ''' <returns>String formatted with all the messages from the Exception</returns>
    Private Function unravelExceptionMessage(ByRef theEX As Exception) As String
        'unravel the error messages
        'ToDo_jb unravelErrorMessage() sort out where this should be. Not here!!!!!
        'this should be a static function some place in the core 
        'or it could be part of an EwEException Class
        Dim thisEx As Exception = theEX
        Try
            Dim errormsg As String = ""
            Do While thisEx IsNot Nothing
                errormsg = errormsg & thisEx.Message & vbNewLine
                thisEx = thisEx.InnerException
            Loop
            Return errormsg
        Catch ex As Exception
            'oooppps
            Debug.Assert(False, ex.Message)
            Return ""
        End Try

    End Function

#End Region

#Region "Message handlers"

#Region "Methods used by Network Analysis to update the Manager about progress."


    ''' <summary>
    ''' Called by the network analysis to update on progress from FindCycles
    ''' </summary>
    ''' <param name="iCounter"></param>
    ''' <remarks></remarks>
    Friend Sub updateProgressFindCycle(ByVal iCounter As Integer)
        'Tell the world
        Try
            RaiseEvent RunMainNetworkProgress(iCounter)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Called from RunNetworkAnalysis
    ''' </summary>
    ''' <param name="iCounter"></param>
    ''' <remarks></remarks>
    Friend Sub UpdateNetworkAnalysis(ByVal iCounter As Integer)
        'Tell the world
        Try
            RaiseEvent RunMainNetworkProgress(iCounter)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try

    End Sub

    Friend Sub updateFoundCycle(ByVal iCycle As Integer)
        'Tell the world
        Try
            RaiseEvent CycleFound(iCycle)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Called from PrintPath()
    ''' </summary>
    ''' <param name="iPath"></param>
    ''' <remarks></remarks>
    Friend Sub UpdatePrintPath(ByVal iPath As Integer)
        'Tell the world
        Try
            RaiseEvent FindPathwaysProgress(iPath)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Called from PrintCycle()
    ''' </summary>
    ''' <param name="iCycle"></param>
    ''' <remarks></remarks>
    Friend Sub UpdatePrintCycle(ByVal iCycle As Integer)
        'Tell the world
        Try
            If (iCycle Mod 100) = 0 Then RaiseEvent FindCyclesProgress(iCycle)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Called from FindPaths()
    ''' </summary>
    Friend Sub UpdateCalculateRequiredPP(ByVal nPathsFound As Integer)
        'Tell the world
        Try
            If (nPathsFound Mod 1000) = 0 Then RaiseEvent CalculateRequiredPPProgress(nPathsFound)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try

    End Sub

    Friend Sub UpdateEcosimNetwork(ByVal iTime As Integer)
        'Tell the world
        Try
            RaiseEvent EcosimNetworkProgress(iTime)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try

    End Sub

    Public Property CancelRequiredPrimaryProdRun() As Boolean
        Get
            Return m_CancelRequiredPrimaryProdRun
        End Get
        Set(ByVal value As Boolean)
            m_CancelRequiredPrimaryProdRun = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Listen to the core's state monitor to see if Ecopath has been changed
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="iState"></param>
    ''' <remarks></remarks>
    Private Sub CoreStateMonitor_CoreExecutionStateEvent(ByVal core As cCore, ByVal iState As EwEUtils.Core.eCoreExecutionState) Handles CoreStateMonitor.CoreExecutionStateEvent

        'ToDo_jb CoreStateMonitor_CoreExecutionStateEvent() Ecoism loaded does not need to be false if Ecopath is rerun 

        'If ecopath has loaded or it has just run 
        'then the network analysis needs to be run or re-run
        If iState <= EwEUtils.Core.eCoreExecutionState.EcopathCompleted Then
            m_runstate = eRunState.NetworkNeedsToRun
            'System.Console.WriteLine("Network Analysis Plugin state changed. Core state = " & iState.ToString & " Network Analysis plugin state = " & m_runstate.ToString)
        End If

        'An ecosim scenario has loaded 
        If iState = EwEUtils.Core.eCoreExecutionState.EcosimLoaded Then
            m_runstate = eRunState.EcosimIsLoaded
            'System.Console.WriteLine("Network Analysis Plugin state changed. Core state = " & iState.ToString & " Network Analysis plugin state = " & m_runstate.ToString)
        End If


    End Sub


#End Region

End Class
