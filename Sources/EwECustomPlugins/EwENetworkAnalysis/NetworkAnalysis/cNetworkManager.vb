'==============================================================================
'
' $Log: cNetworkManager.vb,v $
' Revision 1.24  2009/06/06 21:55:39  jeroens
' Added DetritusByTrophicLevel
'
' Revision 1.23  2009/06/05 02:50:25  jeroens
' Moving calcs from UI to manager for reuse
'
' Revision 1.22  2009/06/04 18:04:07  jeroens
' Moved transfer efficiency computations to manager
'
' Revision 1.21  2009/06/03 02:22:40  jeroens
' Implemented VC changes 2jun09
'
' Revision 1.20  2009/06/02 15:56:20  jeroens
' Added TotalImpact (needs validation)
'
' Revision 1.19  2009/06/02 02:38:51  jeroens
' Renamed exposed keystoneness indicators
'
' Revision 1.18  2009/05/30 00:00:54  jeroens
' Toolstrip usage centralized
'
' Revision 1.17  2009/05/28 12:37:17  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.16  2009/05/28 02:14:33  jeroens
' Keystoneness vars exposed
'
' Revision 1.15  2009/05/21 18:53:38  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.14  2009/05/04 02:12:49  jeroens
' NA Sim off unless initiated from NA nav tree
'
' Revision 1.13  2009/05/02 03:02:17  jeroens
' Cleaned up
'
' Revision 1.12  2009/05/01 17:46:40  jeroens
' Simplified run state management
' Uses central status feedback
'
' Revision 1.11  2009/04/28 19:00:30  jeroens
' Revamped to be able to use styleguide hide groups, rather than an isolated hidegroups interface
'
' Revision 1.10  2009/04/28 16:37:29  jeroens
' Fixed issue 617
'
' Revision 1.9  2009/03/22 14:01:37  jeroens
' Core state monitor exec event parameters simplified
'
' Revision 1.8  2009/01/23 03:10:59  jeroens
' Removed unused references
'
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
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls
Imports EwECore
Imports EwEUtils.Core

' ToDo_JS: localize this class

''' <summary>
''' Manager for the Network Analysis
''' </summary>
''' <remarks>This object is used to coordinate running of the Network Analysis and population of the ouput. </remarks>
Public Class cNetworkManager

#Region " Private data "

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
    Private Enum eRunState As Byte
        CoreNotReady
        NetworkNeedsToRun
        NetworkHasRun
        RequirePPHasRun

        ''' <summary>Ecoism has loaded a scenario. Ecosim network can initialize. </summary>
        EcosimIsLoaded
        ''' <summary>Ecosim network has been initialized. </summary>
        EcosimNetworkInitialized

    End Enum

    Private m_econetwork As cEcoNetwork = Nothing
    Private m_core As cCore = Nothing
    Private m_corestatemonitor As cCoreStateMonitor = Nothing
    Private m_epdata As cEcopathDataStructures = Nothing
    Private m_esdata As cEcosimDatastructures = Nothing
    Private m_messagesource As eCoreComponentType = eCoreComponentType.Plugin
    Private m_runstate As eRunState = eRunState.CoreNotReady

    ''' <summary>Flag stating whether Ecosim NA should run with Ecosim.</summary>
    Private m_bUseEcosimNetwork As Boolean = False

    ''' <summary>Flag stating whether the main N/A network has ran.</summary>
    Private m_bIsMainNetworkRun As Boolean = False
    ''' <summary>Flag stating whether the Ecosin N/A network has ran.</summary>
    Private m_bIsEcosimNetworkRun As Boolean = False
    ''' <summary>To comment</summary>
    Private m_bIsRequiredPrimaryProdRun As Boolean = False

#If 0 Then ' Unused code
    ''' <summary>List of iGroups that have fish catch </summary>
    Private lstCatch As New List(Of Integer)
#End If

    ''' <summary><see cref="cMessagePublisher">Core message publisher</see> for
    ''' sending messages through the EwE core system.</summary>
    Private m_publisher As cMessagePublisher = Nothing

#End Region ' Private data

#Region " Construction and initialization "

    Public Sub New()
        m_runstate = eRunState.CoreNotReady
    End Sub

    Friend Function Init(ByRef theCore As cCore) As Boolean

        m_core = theCore
        m_corestatemonitor = m_core.StateMonitor
        m_publisher = theCore.Messages
        m_econetwork = New cEcoNetwork(Me)

        AddHandler Me.m_corestatemonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
        Return True

    End Function

    Friend Sub Clear()
        RemoveHandler Me.m_corestatemonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
    End Sub

#End Region ' Construction and initialization

#Region " Public Methods for running models "

#Region " Main Network Analysis "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run the Main Network Analysis routines - if necessary
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' This populates the data for EwE5 'Trophic level decomposition', 
    ''' 'Flow and biomass', 'Mixed Trophic impact', 'Acendency' and 
    ''' 'Flow form detritus' tabs.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function RunMainNetwork() As Boolean

        Dim bSucces As Boolean = True
        Dim sg As cStyleGuide = cStyleGuide.GetInstance()
        Dim abGroupsToShow(Me.m_core.nGroups) As Boolean

        Debug.Assert(m_econetwork IsNot Nothing)

        ' Optimization
        If Me.m_bIsMainNetworkRun = True Then Return True

        m_runstate = eRunState.NetworkNeedsToRun

        If m_econetwork Is Nothing Then
            'message of some sort
            m_publisher.SendMessage(New cMessage("Network Analysis not initialized properly.", eMessageType.ErrorEncountered, m_messagesource, eMessageImportance.Warning))
            bSucces = False
        End If

        If bSucces And (m_runstate <> eRunState.CoreNotReady) Then
            Try
                For iGroup As Integer = 1 To Me.m_core.nGroups
                    abGroupsToShow(iGroup) = True
                    ' JS: group hiding has not yet been enabled
                    'abGroupsToShow(iGroup) = sg.GroupVisible(iGroup)
                Next

                m_runstate = eRunState.NetworkNeedsToRun
                m_econetwork.GroupsToShow = abGroupsToShow

                'Make sure the network analysis object has the latest data computed by the core
                'This may not be necessary because m_EcoNetwork keeps a reference to the data. 
                'However, this is more robust, incase the core has created a new m_EcoPathData object.
                m_econetwork.EcopathData = m_epdata
                m_econetwork.RunNetworkAnalysis()

                m_runstate = eRunState.NetworkHasRun

                bSucces = True
                m_bIsMainNetworkRun = True

            Catch ex As Exception
                cLog.Write(ex)
                Dim msg As String = Me.unravelExceptionMessage(ex)
                m_publisher.SendMessage(New cMessage(Me.ToString & ".RunMainNetwork() Error " & msg, eMessageType.ErrorEncountered, m_messagesource, eMessageImportance.Critical))
                'Debug.Assert(False, msg)
                bSucces = False
            End Try
        Else
            ''message of some sort
            m_publisher.SendMessage(New cMessage("Network Analysis can not be run before Ecopath.", eMessageType.StateNotMet, m_messagesource, eMessageImportance.Warning))
            bSucces = False
        End If

        Return bSucces

    End Function

    'Bug 252 fix by joeh
    'Cahnge
    'Public Function IsMainNetworkRun() As Boolean
    '    Return m_IsMainNetworkRun
    'End Function
    Public Property IsMainNetworkRun() As Boolean
        Get
            Return m_bIsMainNetworkRun
        End Get
        Set(ByVal value As Boolean)
            m_bIsMainNetworkRun = value
        End Set
    End Property
    'End Change

#End Region ' Main Network Analysis

#Region " Required PP "

    ''' <summary>
    ''' Run the Require Primary Procuction models - if not already ran.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This popluates data for the EwE5 tabs 'Primary prod. required'-'For harvest of all groups' and 'For consumption of all groups'</remarks>
    Public Function RunRequiredPrimaryProd() As Boolean

        Dim bSuccess As Boolean = True

        If (m_bIsRequiredPrimaryProdRun = True) Then
            Return bSuccess
        End If

        Debug.Assert(m_econetwork IsNot Nothing)

        If m_econetwork Is Nothing Then
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
                        m_core.Messages.SendMessage(New cMessage("Required Primary Production could not be run because of a problem in Network Analysis.", _
                                                                 eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Critical))
                        Return False
                    End If
                End If

                'Debug.Assert(m_runstate = eRunState.NetworkHasRun)

                m_econetwork.CalculateRequiredPP()

                m_runstate = eRunState.RequirePPHasRun

                bSuccess = True
                m_bIsRequiredPrimaryProdRun = True

            Catch ex As Exception
                cLog.Write(ex)
                Dim msg As String = Me.unravelExceptionMessage(ex)
                m_core.Messages.SendMessage(New cMessage(Me.ToString & ".RunReguiredPrimaryProd() Error " & msg, eMessageType.ErrorEncountered, eCoreComponentType.EcoPath, eMessageImportance.Critical))

                bSuccess = False
            End Try
        Else
            'message of some sort
            m_core.Messages.SendMessage(New cMessage("Required Primary Production can not be run.", eMessageType.StateNotMet, m_messagesource, eMessageImportance.Warning))
            bSuccess = False
        End If

        Return bSuccess

    End Function

    'Bug 252 fix by joeh
    'Change
    'Public Function IsRequiredPrimaryProdRun() As Boolean
    '    Return m_IsRequiredPrimaryProdRun
    'End Function
    Public Property IsRequiredPrimaryProdRun() As Boolean
        Get
            Return m_bIsRequiredPrimaryProdRun
        End Get
        Set(ByVal value As Boolean)
            m_bIsRequiredPrimaryProdRun = value
        End Set
    End Property
    'End change

#End Region ' Required PP

#Region " Pathways "

    ''' <summary>
    ''' TL1-->Consumer
    ''' </summary>
    ''' <param name="iToGroup"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysToConsumer(ByVal iToGroup As Integer) As Boolean

        Dim nPaths As Integer, nArrows As Integer

        cApplicationStatusNotifier.SetStatusText(String.Format(My.Resources.STATUS_FINDING_PATHWAYS_CONSUMER, _
                                        Me.GroupName(iToGroup)), TriState.True)
        Try
            m_econetwork.FindCycles(m_epdata.DC, ePathways.ToConsumer, iToGroup, 0, nPaths, nArrows)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

        cApplicationStatusNotifier.SetStatusText("", TriState.False)
        Return True

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

        cApplicationStatusNotifier.SetStatusText(String.Format(My.Resources.STATUS_FINDING_PATHWAYS_CONSPREY, _
                                        Me.GroupName(iToGroup), _
                                        Me.GroupName(iViaGroup)), TriState.True)

        Try
            m_econetwork.FindCycles(m_epdata.DC, ePathways.ToConsumerViaPrey, iToGroup, iViaGroup, nPaths, nArrows)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

        cApplicationStatusNotifier.SetStatusText("", TriState.False)
        Return True

    End Function

    ''' <summary>
    ''' Prey-->Top Predator
    ''' </summary>
    ''' <param name="iFromGroup"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysFromPrey(ByVal iFromGroup As Integer) As Boolean

        Dim nPaths As Integer, nArrows As Integer

        cApplicationStatusNotifier.SetStatusText(String.Format(My.Resources.STATUS_FINDING_PATHWAYS_PREY, _
                                        Me.GroupName(iFromGroup)), TriState.True)

        Try
            m_econetwork.FindCycles(m_epdata.DC, ePathways.FromPrey, 1, iFromGroup, nPaths, nArrows)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

        cApplicationStatusNotifier.SetStatusText("", TriState.False)
        Return True

    End Function

    ''' <summary>
    ''' Cycles(excl. detitus)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysCycles() As Boolean

        Dim nPaths As Integer, nArrows As Integer

        cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_FINDING_PATHWAYS, TriState.True)

        Try
            'ToDo_jb FindPathwaysCycles EwE5 calls InitCyclesList ????? I can not find this again
            m_econetwork.FindCycles(m_epdata.DC, ePathways.LinkedPathways, 1, 1, nPaths, nArrows)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

        cApplicationStatusNotifier.SetStatusText("", TriState.False)
        Return True

    End Function

    ''' <summary>
    ''' All cycles
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPathwaysCyclesAll() As Boolean

        Dim nPaths As Integer, nArrows As Integer

        cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_FINDING_PATHWAYS, TriState.True)

        Try
            m_econetwork.FindCycles(m_epdata.DC, ePathways.All, 1, 1, nPaths, nArrows)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

        cApplicationStatusNotifier.SetStatusText("", TriState.False)
        Return True

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

#End Region ' Pathways

#Region " Network from Ecosim "

    ''' <summary>
    ''' Run Ecosim and compute the ecosim network analysis data - if not already ran.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function RunEcosimNetwork() As Boolean

        Try

            If Not Me.m_bUseEcosimNetwork Then Return False

            If Not m_core.StateMonitor.HasEcosimLoaded Then
                'No Ecosim Scenario is loaded the Ecosim network analysis can not be run
                m_core.Messages.SendMessage(New cMessage("Please load an Ecosim scenario before running Network Analysis for Ecosim.", _
                         eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Warning))

                Return False
            End If

            If Me.m_bIsEcosimNetworkRun Then Return True

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_RUNNING_NETWORKANALYSIS, TriState.True)
            Me.m_bIsEcosimNetworkRun = Me.m_core.RunEcoSim()
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

        Catch ex As Exception
            cLog.Write(ex)
            m_core.Messages.SendMessage(New cMessage("Error while running Network Analysis for Ecosim. " & ex.Message, _
                                            eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Critical))
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Initialize Ecosim Network Analysis
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InitNetworkForEcosim() As Boolean

        Try

            'don't do anything if the Ecosim Network Analysis is turned off
            If Not Me.m_bUseEcosimNetwork Then
                Return False
            End If

            'If m_runstate < eRunState.EcosimIsLoaded Then
            If Not m_core.StateMonitor.HasEcosimLoaded Then
                'No Ecosim Scenario is loaded this can not be initialized
                m_core.Messages.SendMessage(New cMessage("Network Analysis for Ecosim could not be initialized because an Ecosim scenario has not been loaded.", _
                         eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Information))
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

            m_econetwork.EcopathData = m_epdata
            m_econetwork.EcosimData = m_esdata

            m_econetwork.InitForEcosim()
            m_runstate = eRunState.EcosimNetworkInitialized


        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".InitNetworkForEcosim " & ex.Message)
            Return False
        End Try
        Return True

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

        Dim bSucces As Boolean = True
        Try

            'don't do anything if the Ecosim Network Analysis is turned off
            If Not Me.UseEcosimNetwork Then
                Return False
            End If

            If m_runstate < eRunState.EcosimNetworkInitialized Then
                'do not try to run this if it has not been initialized
                'no messages here so that this does not slow down Ecosim
                Return False
            End If

            'do ecosim network calculation for this time step
            m_econetwork.EcosimTimestep(BiomassAtTimestep, EcosimDatastructures, iTime)
            'tell the world that a time step has been computed
            Me.UpdateProgress(My.Resources.STATUS_RUNNING_NETWORKANALYSIS, CSng(iTime / Me.m_esdata.NTimes))

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.ToString)
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Network from Ecosim

#End Region ' Public Methods for running models

#Region " Public Properties "

#Region " Inputs "

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
    Public Property UseEcosimNetwork() As Boolean
        Get
            Return m_bUseEcosimNetwork
        End Get
        Set(ByVal value As Boolean)
            m_bUseEcosimNetwork = value
        End Set
    End Property

    ''' <summary>
    ''' Run whether network analysis for Ecosim has ran
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property IsEcosimNetworkRan() As Boolean
        Get
            Return m_bIsEcosimNetworkRun
        End Get
        Set(ByVal value As Boolean)
            m_bIsEcosimNetworkRun = value
        End Set
    End Property

    ''' <summary>
    ''' Run the Required Primary Production routines for ecosim
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is very time consuming</remarks>
    Public Property EcosimPPROn() As Boolean
        Get
            Return Me.m_econetwork.PPRon
        End Get
        Set(ByVal value As Boolean)
            If (value <> Me.m_econetwork.PPRon) Then
                ' Update flag
                Me.m_econetwork.PPRon = value
                ' Void ecosim run
                Me.m_bIsEcosimNetworkRun = False
            End If
        End Set
    End Property

    Public ReadOnly Property Core() As cCore
        Get
            Return Me.m_core
        End Get
    End Property

#End Region ' Inputs

#Region " Model outputs "

#Region " Counters "

    Public ReadOnly Property nTrophicLevels() As Integer
        Get
            Return Me.m_econetwork.NoTL
        End Get
    End Property

    Public ReadOnly Property nGroups() As Integer
        Get
            Return Me.m_core.nGroups
        End Get
    End Property

    Public ReadOnly Property nLivingGroups() As Integer
        Get
            Return Me.m_core.nLivingGroups
        End Get
    End Property

    Public ReadOnly Property nDetritusGroups() As Integer
        Get
            Return Me.m_core.nDetritusGroups
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

#End Region ' Counters

#Region " Pathways "

    ''' <summary>
    ''' EwE5 Cycles and Pathways
    ''' </summary>
    ''' <remarks>PathWays will contain new data on each call to FindPathwaysxxxxxx</remarks>
    Public ReadOnly Property PathWays() As List(Of String)
        Get
            Return Me.m_econetwork.lstPathways
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Cycles and Pathways
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property NumArrows() As Integer
        Get
            Return Me.m_econetwork.NumberArrows
        End Get
    End Property

#End Region ' Pathways

#Region " Flows "

    ''' <summary>
    ''' EwE5 Trophic level decomposition Relative Flows
    ''' </summary>
    Public ReadOnly Property RelativeFlow(ByVal iGroup As Integer, ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.AM(iTrophicLevel, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Absolute Flows
    ''' </summary>
    Public ReadOnly Property AbsoluteFlow(ByVal iGroup As Integer, ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.AM_Abs(iTrophicLevel, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Sum of Absolute Flows across all the groups for a trophic level
    ''' </summary>
    Public ReadOnly Property AbsoluteFlowTotal(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.QTL(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Used in computing Transfer Effeiciency
    ''' </summary>
    Public ReadOnly Property CA(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.CA(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Used in computing Transfer Effeiciency
    ''' </summary>
    Public ReadOnly Property CatchDetritus(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.CAD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Used in computing Transfer Effeiciency
    ''' </summary>
    Public ReadOnly Property FlowFromDetritus() As Single
        Get
            Return m_econetwork.DetIndex
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Biomass by Trophic Level
    ''' </summary>
    Public ReadOnly Property BiomassByTrophicLevel(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.BbyTL(iTrophicLevel)
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
    ''' EwE5 Trophic level decomposition detritus by Trophic Level
    ''' </summary>
    Public ReadOnly Property DetritusByTrophicLevel(ByVal iTrophicLevel As Integer) As Single
        Get
            Dim sMass As Single = 0
            If iTrophicLevel = 1 Then
                For i As Integer = Me.nLivingGroups + 1 To Me.nGroups
                    sMass += Me.BiomassByGroup(i)
                Next
            Else
                If Me.PPToDetritus(iTrophicLevel) > 0 Then
                    sMass = (Me.BiomassByTrophicLevel(iTrophicLevel) * Me.DetToDetritus(iTrophicLevel)) / _
                            (Me.PPToDetritus(iTrophicLevel) + Me.DetToDetritus(iTrophicLevel))
                End If
            End If
            Return sMass
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Trophic level decomposition Catch by Trophic Level
    ''' </summary>
    Public ReadOnly Property CatchByTrophicLevel(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.CbyTL(iTrophicLevel)
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
            Return m_econetwork.MTI(iPred, iPrey)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 Flow from detritus
    ''' </summary>
    Public ReadOnly Property FlowFromDetritus(ByVal iGroup As Integer) As Single
        Get
            Dim sumad As Single
            For itl As Integer = 1 To m_econetwork.NoTL
                sumad += m_econetwork.Ad(itl, iGroup)
            Next
            Return sumad
        End Get
    End Property

    Public ReadOnly Property DetTransferEfficiency(ByVal iTrophicLevel As Integer) As Single
        Get
            If Me.DetThroughtput(iTrophicLevel) > 0.001 Then
                Return (Me.CatchDetritus(iTrophicLevel) + Me.DetConsByPred(iTrophicLevel)) / Me.DetThroughtput(iTrophicLevel)
            End If
            Return 0
        End Get
    End Property

    Public ReadOnly Property PPTransferEfficiency(ByVal iTrophicLevel As Integer) As Single
        Get
            If Me.PPThroughtput(iTrophicLevel) > 0.001 Then
                Return (Me.CA(iTrophicLevel) + Me.PPConsByPred(iTrophicLevel)) / Me.PPThroughtput(iTrophicLevel)
            End If
            Return 0
        End Get
    End Property

    Public ReadOnly Property TotTransferEfficiency(ByVal iTrophicLevel As Integer) As Single
        Get
            Dim sTotThroughput As Single = (Me.DetThroughtput(iTrophicLevel) + Me.PPThroughtput(iTrophicLevel))
            If sTotThroughput > 0 Then
                Return (Me.CatchDetritus(iTrophicLevel) + _
                        Me.CA(iTrophicLevel) + _
                        Me.DetConsByPred(iTrophicLevel) + _
                        Me.PPConsByPred(iTrophicLevel)) / sTotThroughput
            End If
            Return 0
        End Get
    End Property

#End Region ' Flows

#Region " Ascendancy "

#Region "By Group"

    Public ReadOnly Property AscendancyByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_econetwork.Ac(iGroup)
        End Get
    End Property

    Public ReadOnly Property OverheadByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_econetwork.Ec(iGroup)
        End Get
    End Property

    Public ReadOnly Property CapacityByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_econetwork.CC(iGroup)
        End Get
    End Property

    Public ReadOnly Property InformationByGroup(ByVal iGroup As Integer) As Single
        Get
            If m_econetwork.TruPut > 0 Then
                Return m_econetwork.Ac(iGroup) / m_econetwork.TruPut
            Else
                Return cCore.NULL_VALUE
            End If
        End Get
    End Property

    Public ReadOnly Property ThroughputByGroup(ByVal iGroup As Integer) As Single
        Get
            Return m_econetwork.Q(iGroup)
        End Get
    End Property

    Public ReadOnly Property AscendencyTotal() As Single
        Get
            Return m_econetwork.SumAc
        End Get
    End Property

    Public ReadOnly Property OverheadTotal() As Single
        Get
            Return m_econetwork.SumEc
        End Get
    End Property

    Public ReadOnly Property CapacityTotal() As Single
        Get
            Return m_econetwork.SumCc
        End Get
    End Property

    Public ReadOnly Property ThroughputTotal() As Single
        Get
            Return m_econetwork.TruPut
        End Get
    End Property

    Public ReadOnly Property ThroughputCycledLiving() As Single
        Get
            Return m_econetwork.Tc
        End Get
    End Property

    Public ReadOnly Property ThroughputCycledPredatory() As Single
        Get
            Return m_econetwork.TCyc
        End Get
    End Property

    Public ReadOnly Property ThroughputCycledAll() As Single
        Get
            Return m_econetwork.TcD
        End Get
    End Property

    Public ReadOnly Property ThroughputExport() As Single
        Get
            Return m_econetwork.SumEx
        End Get
    End Property

    Public ReadOnly Property ThroughputResp() As Single
        Get
            Return m_econetwork.SumResp
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
            Return m_econetwork.Ai
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage flow
    ''' </summary>
    Public ReadOnly Property AscendancyInternalFlowPer() As Single
        Get
            Return m_econetwork.Aip
        End Get
    End Property


    ''' <summary>
    ''' Ascendency total import
    ''' </summary>
    Public ReadOnly Property AscendancyImportTotal() As Single
        Get
            Return m_econetwork.Ao
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage import
    ''' </summary>
    Public ReadOnly Property AscendancyImportPer() As Single
        Get
            Return m_econetwork.Aop
        End Get
    End Property


    ''' <summary>
    ''' Ascendency total export
    ''' </summary>
    Public ReadOnly Property AscendancyExportTotal() As Single
        Get
            Return m_econetwork.Ae
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage export
    ''' </summary>
    Public ReadOnly Property AscendancyExportPer() As Single
        Get
            Return m_econetwork.Aep
        End Get
    End Property

    ''' <summary>
    ''' Ascendency total respiration
    ''' </summary>
    Public ReadOnly Property AscendancyRespTotal() As Single
        Get
            Return m_econetwork.Ar
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage respiration
    ''' </summary>
    Public ReadOnly Property AscendancyRespPer() As Single
        Get
            Return m_econetwork.Arp
        End Get
    End Property

    ''' <summary>
    ''' Ascendency total totals
    ''' </summary>
    Public ReadOnly Property AscendancyTotalsTotal() As Single
        Get
            Return m_econetwork.Ascen
        End Get
    End Property

    ''' <summary>
    ''' Ascendency percentage totals
    ''' </summary>
    Public ReadOnly Property AscendancyTotalsPer() As Single
        Get
            Return m_econetwork.Ascp
        End Get
    End Property

#End Region

#Region "Overhead"

    ''' <summary>
    ''' Overhead flow total 
    ''' </summary>
    Public ReadOnly Property OverheadFlowTotal() As Single
        Get
            Return m_econetwork.Ei
        End Get
    End Property

    ''' <summary>
    ''' Overhead Flow percentage 
    ''' </summary>
    Public ReadOnly Property OverheadFlowPer() As Single
        Get
            Return m_econetwork.Eip
        End Get
    End Property


    ''' <summary>
    ''' Overhead total import
    ''' </summary>
    Public ReadOnly Property OverheadImportTotal() As Single
        Get
            Return m_econetwork.Eo
        End Get
    End Property

    ''' <summary>
    ''' Overhead percentage import
    ''' </summary>
    Public ReadOnly Property OverheadImportPer() As Single
        Get
            Return m_econetwork.Eop
        End Get
    End Property

    ''' <summary>
    ''' Overhead  Export total 
    ''' </summary>
    Public ReadOnly Property OverheadExportTotal() As Single
        Get
            Return m_econetwork.Eee
        End Get
    End Property

    ''' <summary>
    ''' Overhead Export percentage 
    ''' </summary>
    Public ReadOnly Property OverheadExportPer() As Single
        Get
            Return m_econetwork.Eep
        End Get
    End Property

    ''' <summary>
    ''' Overhead respiration  total 
    ''' </summary>
    Public ReadOnly Property OverheadRespTotal() As Single
        Get
            Return m_econetwork.er
        End Get
    End Property

    ''' <summary>
    ''' Overhead respiration percentage 
    ''' </summary>
    Public ReadOnly Property OverheadRespPer() As Single
        Get
            Return m_econetwork.Erp
        End Get
    End Property

    ''' <summary>
    ''' Overhead totals total 
    ''' </summary>
    Public ReadOnly Property OverheadTotalsTotal() As Single
        Get
            Return m_econetwork.Overhead
        End Get
    End Property

    ''' <summary>
    ''' Overhead totals percentage 
    ''' </summary>
    Public ReadOnly Property OverheadTotalsPer() As Single
        Get
            Return m_econetwork.Overp
        End Get
    End Property

#End Region

#Region "Capacity"

    ''' <summary>
    ''' Capacity  flow percentage 
    ''' </summary>
    Public ReadOnly Property CapacityFlowTotal() As Single
        Get
            Return m_econetwork.Ci
        End Get
    End Property

    ''' <summary>
    ''' Capacity flow percentage 
    ''' </summary>
    Public ReadOnly Property CapacityFlowPer() As Single
        Get
            Return m_econetwork.Cip
        End Get
    End Property


    ''' <summary>
    ''' Capacity total import
    ''' </summary>
    Public ReadOnly Property CapacityImportTotal() As Single
        Get
            Return m_econetwork.Co
        End Get
    End Property

    ''' <summary>
    ''' Capacity percentage import
    ''' </summary>
    Public ReadOnly Property CapacityImportPer() As Single
        Get
            Return m_econetwork.Cop
        End Get
    End Property

    ''' <summary>
    ''' Capacity export total
    ''' </summary>
    Public ReadOnly Property CapacityExportTotal() As Single
        Get
            Return m_econetwork.Ce
        End Get
    End Property

    ''' <summary>
    ''' Capacity export precentage
    ''' </summary>
    Public ReadOnly Property CapacityExportPer() As Single
        Get
            Return m_econetwork.Cep
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration total
    ''' </summary>
    Public ReadOnly Property CapacityRespTotal() As Single
        Get
            Return m_econetwork.Cr
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration precentage
    ''' </summary>
    Public ReadOnly Property CapacityRespPer() As Single
        Get
            Return m_econetwork.Crp
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration precentage
    ''' </summary>
    Public ReadOnly Property CapacityTotalsTotal() As Single
        Get
            Return m_econetwork.Capacity
        End Get
    End Property

    ''' <summary>
    ''' Capacity respiration precentage
    ''' </summary>
    Public ReadOnly Property CapacityTotalsPer() As Single
        Get
            Return m_econetwork.Capp
        End Get
    End Property

#End Region

#End Region

#End Region ' Ascendancy

#Region " Trophic Level "

    ''' <summary>
    ''' Flow and Biomass From primary prod. Import 
    ''' </summary>
    Public ReadOnly Property PPImport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.Impo(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Cons by Pred 
    ''' </summary>
    Public ReadOnly Property PPConsByPred(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.Predat(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Export
    ''' </summary>
    Public ReadOnly Property PPExport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.EXA(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Flow To Detritus
    ''' </summary>
    Public ReadOnly Property PPToDetritus(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.DTA(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Respiration
    ''' </summary>
    Public ReadOnly Property PPRespiration(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.RSP(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Throughtput
    ''' </summary>
    Public ReadOnly Property PPThroughtput(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.TRP(iTrophicLevel)
        End Get
    End Property

    Public ReadOnly Property PPThroughtputSum() As Single
        Get
            Dim sSum As Single = 0
            For i As Integer = 1 To Me.nTrophicLevels
                sSum += Me.PPThroughtput(i)
            Next
            Return sSum
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Import 
    ''' </summary>
    Public ReadOnly Property DetImport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.ImpD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Cons by Pred 
    ''' </summary>
    Public ReadOnly Property DetConsByPred(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.PredatD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Export
    ''' </summary>
    Public ReadOnly Property DetExport(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.EXAD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Flow To Detritus
    ''' </summary>
    Public ReadOnly Property DetToDetritus(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.DTAD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Respiration
    ''' </summary>
    Public ReadOnly Property DetRespiration(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.RSPD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Throughtput
    ''' </summary>
    Public ReadOnly Property DetThroughtput(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.TRPD(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From detritus. Throughtput sum
    ''' </summary>
    Public ReadOnly Property DetThroughtputSum() As Single
        Get
            Dim sSum As Single = 0
            For iTrophicLevel As Integer = 1 To Me.nTrophicLevels
                sSum += Me.DetThroughtput(iTrophicLevel)
            Next
            Return sSum
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Throughtput shown
    ''' </summary>
    Public ReadOnly Property ThroughtputShow(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.TrpShow(iTrophicLevel)
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From primary prod. Throughtput shown
    ''' </summary>
    Public Property TrEm1(ByVal iTrophicLevel As Integer) As Single
        Get
            Return m_econetwork.TrEm1(iTrophicLevel)
        End Get
        Set(ByVal value As Single)
            m_econetwork.TrEm1(iTrophicLevel) = value
        End Set
    End Property

    ''' <summary>
    ''' Flow and Biomass From all combined. Extracted to break cycles
    ''' </summary>
    Public ReadOnly Property ExtractedToBreakCycles() As Single
        Get
            Return m_econetwork.AmCyc
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From all combined. Input TLII+
    ''' </summary>
    Public ReadOnly Property InputTLIIPlus() As Single
        Get
            Return m_econetwork.SumIm
        End Get
    End Property

    ''' <summary>
    ''' Flow and Biomass From all combined. Total throughput
    ''' </summary>
    Public ReadOnly Property TotalThroughput() As Single
        Get
            Return m_econetwork.TotalTrp
        End Get
    End Property

#End Region ' Trophic Level

#Region " Indicators "

    Public ReadOnly Property Electivity(ByVal iSel As Integer, ByVal iPrey As Integer, ByVal iTime As Integer) As Single
        Get
            Return Me.EcosimData.Elect(iSel, iPrey, iTime)
        End Get
    End Property

#End Region ' Indicators

#Region " Primary Production Required "

#If 0 Then ' Unused code

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

#End If ' Unused code

    ''' <summary>
    ''' EwE5 No.of paths
    ''' </summary>
    Public ReadOnly Property NumerPaths(ByVal iGroup As Integer) As Integer
        Get
            Return m_econetwork.NumPath(iGroup)
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
            Return m_econetwork.totalPP
        End Get
    End Property

#Region "For consumption of all groups"

    ''' <summary>
    ''' EwE5 PPR(PP)
    ''' </summary>
    Public ReadOnly Property PPRRequired(ByVal iGroup As Integer) As Single
        Get
            'Return m_epdata.TTLX(iGroup)
            Return m_econetwork.SumPPRequired(1, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR(det)
    ''' </summary>
    Public ReadOnly Property PPRRequiredDet(ByVal iGroup As Integer) As Single
        Get
            Return m_econetwork.SumDetRequired(1, iGroup)
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
            Return CSng(100.0 * PPRRequiredSum(iGroup) / (m_econetwork.totalPP + m_econetwork.TRPD(1)))
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/u.biom.
    ''' </summary>
    Public ReadOnly Property PPRU(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequiredSum(iGroup) / (m_econetwork.totalPP + m_econetwork.TRPD(1)) / m_epdata.B(iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 NumLivPath.
    ''' </summary>
    Public ReadOnly Property NumLivPath() As Single
        Get
            Return m_econetwork.NumLivPath
        End Get
    End Property

    ''' <summary>
    ''' EwE5 NumDetPath.
    ''' </summary>
    Public ReadOnly Property NumDetPath() As Single
        Get
            Return m_econetwork.NumDetPath
        End Get
    End Property

#End Region

#Region "For harvest of all groups"
    ''' <summary>
    ''' EwE5 PPR(PP)
    ''' </summary>
    Public ReadOnly Property PPRRequiredHarvest(ByVal iGroup As Integer) As Single
        Get
            Return m_econetwork.SumPPRequired(0, iGroup)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR(det)
    ''' </summary>
    Public ReadOnly Property PPRRequiredDetHarvest(ByVal iGroup As Integer) As Single
        Get
            Return m_econetwork.SumDetRequired(0, iGroup)
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
            Return CSng(100.0 * PPRRequiredSumHarvest(iGroup) / (m_econetwork.totalPP + m_econetwork.TRPD(1)))
        End Get
    End Property

    ''' <summary>
    ''' EwE5 PPR/u.catch
    ''' </summary>
    Public ReadOnly Property PPRUHarvest(ByVal iGroup As Integer) As Single
        Get
            Return PPRRequiredSumHarvest(iGroup) / (m_econetwork.totalPP + m_econetwork.TRPD(1)) / m_epdata.fCatch(iGroup)
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
            Return m_econetwork.RaiseToPP(0)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 RaiseToDet
    ''' </summary>
    Public ReadOnly Property TotalPPRDet() As Single
        Get
            Return m_econetwork.RaiseToDet(0)
        End Get
    End Property

    ''' <summary>
    ''' EwE5 totalCatch
    ''' </summary>
    Public ReadOnly Property TotalCatch() As Single
        Get
            Return m_econetwork.totalCatch
        End Get
    End Property

#End Region

#End Region ' Primary Production Required

#Region " Ecosim Public Properties "

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
            Return Me.EcosimData.FIB
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
            Return Me.m_econetwork.RelativeSumOfCatchPlot
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
            Return Me.m_econetwork.RelativeKemptonsPlot
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
            Return Me.m_econetwork.TLCatchPlot
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
            Return Me.m_econetwork.TLSimPlot(iGroup, iTime)
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
            Return Me.m_econetwork.RelativeCatchPPR
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
            Return Me.m_econetwork.RelativeCatchDetReq
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
            Return Me.m_econetwork.Throughput
        End Get
    End Property

    Public ReadOnly Property CapacityEcosim() As Single()
        Get
            Return Me.m_econetwork.CapacityEcosim
        End Get
    End Property

    Public ReadOnly Property AscendImportEcosim() As Single()
        Get
            Return Me.m_econetwork.AscendImport
        End Get
    End Property

    Public ReadOnly Property AscendFlowEcosim() As Single()
        Get
            Return Me.m_econetwork.AscendFlow
        End Get
    End Property

    Public ReadOnly Property AscendExportEcosim() As Single()
        Get
            Return Me.m_econetwork.AscendExport
        End Get
    End Property

    Public ReadOnly Property AscendRespEcosim() As Single()
        Get
            Return Me.m_econetwork.AscendResp
        End Get
    End Property

    Public ReadOnly Property OverheadImportEcosim() As Single()
        Get
            Return Me.m_econetwork.OverheadImport
        End Get
    End Property

    Public ReadOnly Property OverheadFlowEcosim() As Single()
        Get
            Return Me.m_econetwork.OverheadFlow
        End Get
    End Property

    Public ReadOnly Property OverheadExportEcosim() As Single()
        Get
            Return Me.m_econetwork.OverheadExport
        End Get
    End Property

    Public ReadOnly Property OverheadRespEcosim() As Single()
        Get
            Return Me.m_econetwork.OverheadResp
        End Get
    End Property

    Public ReadOnly Property PCIEcosim() As Single()
        Get
            Return Me.m_econetwork.PCI
        End Get
    End Property

    Public ReadOnly Property FCIEcosim() As Single()
        Get
            Return Me.m_econetwork.FCI
        End Get
    End Property

    Public ReadOnly Property PathLengthEcosim() As Single()
        Get
            Return Me.m_econetwork.PathLength
        End Get
    End Property

    Public ReadOnly Property ExportEcosim() As Single()
        Get
            Return Me.m_econetwork.Export
        End Get
    End Property

    Public ReadOnly Property RespEcosim() As Single()
        Get
            Return Me.m_econetwork.Resp
        End Get
    End Property

    Public ReadOnly Property PrimaryProdEcosim() As Single()
        Get
            Return Me.m_econetwork.PrimaryProd
        End Get
    End Property

    Public ReadOnly Property ProdEcosim() As Single()
        Get
            Return Me.m_econetwork.Prod
        End Get
    End Property

    Public ReadOnly Property BiomassEcosim() As Single()
        Get
            Return Me.m_econetwork.Biomass
        End Get
    End Property

    Public ReadOnly Property CatchEcosim() As Single()
        Get
            Return Me.m_econetwork.CatchEcosim
        End Get
    End Property

    Public ReadOnly Property PropFlowDetEcosim() As Single()
        Get
            Return Me.m_econetwork.PropFlowDet
        End Get
    End Property

    Public ReadOnly Property RaiseToPPEcosim() As Single()
        Get
            Return Me.m_econetwork.RaiseToPPEcosim
        End Get
    End Property

    Public ReadOnly Property RaiseToDetEcosim() As Single()
        Get
            Return Me.m_econetwork.RaiseToDetEcosim
        End Get
    End Property

    Public ReadOnly Property AscendTotalEcosim() As Single()
        Get
            Return Me.m_econetwork.Ascendency
        End Get
    End Property

    Public ReadOnly Property AMIEcosim() As Single()
        Get
            Return Me.m_econetwork.AMI
        End Get
    End Property

    Public ReadOnly Property EntropyEcosim() As Single()
        Get
            Return Me.m_econetwork.Entropy
        End Get
    End Property

#End Region ' Ecosim Public Properties

#Region " Keystoneness "

    Public ReadOnly Property KeystoneIndex(ByVal iGroup As Integer) As Double
        Get
            Return Me.m_econetwork.KeystoneIndex(iGroup)
        End Get
    End Property

    Public ReadOnly Property TotalImpactOverBiomass(ByVal iGroup As Integer) As Double
        Get
            Return Me.m_econetwork.TotalImpactOverBiomass(iGroup)
        End Get
    End Property

    Public ReadOnly Property RelativeTotalImpact(ByVal iGroup As Integer) As Double
        Get
            Return Me.m_econetwork.RelTotalImpact(iGroup)
        End Get
    End Property

#End Region ' Keystoneness

#End Region ' Model outputs

#End Region ' Public Properties

#Region " Misc private methods "

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

#End Region ' Misc private methods

#Region " Message handlers "

#Region "Methods used by Network Analysis to update the Manager about progress."

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Notify the world of our progress.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    Friend Sub UpdateProgress(ByVal strText As String, ByVal sProgress As Single)
        Try
            cApplicationStatusNotifier.SetStatusText(strText, TriState.UseDefault, sProgress)
        Catch ex As Exception
        End Try
    End Sub

#End Region

    ''' <summary>
    ''' Listen to the core's state monitor to see if Ecopath has been changed
    ''' </summary>
    Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)

        'ToDo_jb CoreStateMonitor_CoreExecutionStateEvent() Ecoism loaded does not need to be false if Ecopath is rerun 

        'If ecopath has loaded or it has just run 
        'then the network analysis needs to be run or re-run
        If csm.IsExecutionStateSuperceded(EwEUtils.Core.eCoreExecutionState.EcopathCompleted) Then
            m_runstate = eRunState.NetworkNeedsToRun
            'System.Console.WriteLine("Network Analysis Plugin state changed. Core state = " & iState.ToString & " Network Analysis plugin state = " & m_runstate.ToString)
        End If

        'An ecosim scenario has loaded 
        If csm.IsExecutionStateSuperceded(EwEUtils.Core.eCoreExecutionState.EcosimLoaded) Then
            m_runstate = eRunState.EcosimIsLoaded
            'System.Console.WriteLine("Network Analysis Plugin state changed. Core state = " & iState.ToString & " Network Analysis plugin state = " & m_runstate.ToString)
        End If

        ' Invalidate results when core states dictate 
        ' Fixes bug 617
        If Not csm.HasEcopathRan Then
            Me.m_bIsMainNetworkRun = False
        End If

        If Not csm.HasEcosimRan Then
            Me.m_bIsEcosimNetworkRun = False
        End If

    End Sub


#End Region ' Message handlers

End Class
