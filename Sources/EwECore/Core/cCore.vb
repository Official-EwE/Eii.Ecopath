Option Strict On

#Region " Imports "

Imports System.Drawing
Imports EwECore.DataSources
Imports EwECore.ValueWrapper
Imports EwECore.Auxiliary
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwECore.FishingPolicy
Imports EwECore.EcoSeed
Imports EwECore.MSE
Imports EwECore.SearchObjectives
Imports EwECore.Database
Imports System.IO
Imports EwEUtils.Utilities
Imports EwECore.ExternalData
Imports EwEPlugin.Data

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class to handle all interactions between a user interface layer, a 
''' <see cref="IEwEDataSource">datasource</see> and the 
''' <see cref="Ecopath.cEcoPathModel">EcoPath</see>, 
''' <see cref="EcoSim.cEcoSimModel">EcoSim</see> and EcoSpace models.
''' </summary>
''' <remarks>
''' <para>This class provides a wrapper for the underlying EcoPath, EcoSim and
''' EcoSpace models.</para>
''' <para>The underlying model data structures have been converted into classes
''' that an interface can program against. For instance, cFleetInput is the
''' representation of a fishing fleet.</para>
''' <para>The Fleets(iFleet) property provides a way for a user interface to
''' interact with the underlying data structures that represent a fishing fleet
''' without having to understand the modeling array structures.</para>
''' <para>Most conversions from interface objects (cFleetInput or cEcoSimResults) into
''' model data structures are handled by the core.</para>
''' <para>Data structures for each model that need to be made public for setting
''' of parameters or storing to file are held in a wrapper class for each model
''' (<see cref="cEcopathDataStructures">cEcopathDataStructures</see> or 
''' <see cref="cEcosimDatastructures">cEcoSimDatastructures</see>). These classes
''' provide a thin wrapper as well as a way to pass data back and forth between 
''' each other and a <see cref="IEwEDataSource">datasource</see>.</para>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cCore

#Region " Shared consts "

    ''' <summary>The NULL or 'no data' value for values maintained in the EwE Core.</summary>
    Public Const NULL_VALUE As Integer = -9999
    ''' <summary>The maximum age of a stanza life stage.</summary>
    Public Const MAX_AGE As Integer = 400 '1200 '
    ''' <summary>The number of months in a year.</summary><remarks type="bs">A petition to change the number of months per year to 10 has been submitted to the international organization for standardization (ISO) dd Jun02, 2007. We sincerely hope that the next addendum to ISO 9000 will include this change to facilitate our computational models. Unfortunately, until this change has globally been implemented issued, Ecopath will be using the more conventional assumption of 12 months per year.</remarks>
    Public Const N_MONTHS As Integer = 12
    ''' <summary>Max number of year ecosim or ecospace can run for</summary>
    Public Const MAX_RUN_LENGTH As Integer = 500

#End Region ' Shared consts

#Region " Public delegates "

    ''' <summary>
    ''' Delegate defintion used to pass a message from the core to the interface.
    ''' </summary>
    ''' <remarks>
    ''' Used by cMessageHandler to pass messages to an interface.
    ''' </remarks>
    Public Delegate Sub CoreMessageDelegate(ByRef Message As cMessage)

    ''' <summary>
    ''' Delegate defintion used to inform external processes on the progress of
    ''' Ecospace.
    ''' </summary>
    ''' <param name="EcospaceResults">Ecospace results for a single time step.</param>
    Public Delegate Sub EcoSpaceInterfaceDelegate(ByRef EcospaceResults As cEcospaceTimestep)

#End Region ' Public delegates

    ''' <summary>Datasource used by the core for reading and writing model data.</summary>
    Private m_DataSource As IEwEDataSource = Nothing
    ''' <summary>Plug-in manager.</summary>
    Private WithEvents m_pluginManager As cPluginManager = Nothing
    ''' <summary>Path for EwE core processes to write output information to.</summary>
    Private m_strOutputPath As String = ""
    ''' <summary>Core state monitor</summary>
    Private WithEvents m_StateMonitor As cCoreStateMonitor = Nothing
    ''' <summary>Core thread synchronization object for thread marshalling.</summary>
    Private m_SyncObj As System.Threading.SynchronizationContext = Nothing

    ''' <summary>Core state manager</summary>
    ''' <remarks>performs actions to bring core state up-to-date</remarks>
    Private m_StateManager As cCoreStateManager = Nothing

    ''' <summary>Manager to access interface specific to the "Game" interface </summary>
    Private m_gameManager As cGameServerInterface = Nothing

    Private m_ShapeManagers As Dictionary(Of eDataTypes, cBaseShapeManager) = Nothing
    Private m_PedigreeManagers As Dictionary(Of eVarNameFlags, cPedigreeManager) = Nothing
    Private m_MonteCarlo As cMonteCarloManager
    Private m_ConTracer As cContaminantTracer

    ''' <summary>Class to wrap stand alone functions for internal and external access.</summary>
    Private m_Functions As cEcoFunctions = Nothing

    Private m_EwEModel As cEwEModel = Nothing
    Private m_stanzaGroups As New cCoreInputOutputList(Of cStanzaGroup)(eDataTypes.Stanza, 0)
    Private m_timeSeriesDatasets As New cCoreInputOutputList(Of cTimeSeriesDataset)(eDataTypes.TimeSeriesDataset, 1)
    Private m_timeSeriesGroup As New cCoreInputOutputList(Of cTimeSeries)(eDataTypes.GroupTimeSeries, 1)
    Private m_timeSeriesFleet As New cCoreInputOutputList(Of cTimeSeries)(eDataTypes.FleetTimeSeries, 1)

    ''' <summary>Auxillary data not stored in a list, needs to be quickly accessible via hash keys</summary>
    ''' <remarks>Dictionary is 'friend' to be accessible to datasource.</remarks>
    Friend m_dtAuxiliaryData As New Dictionary(Of String, cAuxiliaryData)

    ''' <summary>The central core message publisher.</summary>
    Friend m_publisher As New cMessagePublisher
    Friend m_validators As cValidatorManager = Nothing

    Friend m_TSData As cTimeSeriesDataStructures
    Friend m_SpaceTSData As cEcospaceTimeSeriesDataStructures
    Friend m_Stanza As cStanzaDatastructures
    Friend m_FitToTimeSeriesData As cF2TSDataStructures
    Friend m_tracerData As cContaminantTracerDataStructures

#Region "Private Initialization Flags"

    ''' <summary>Has the Core been initialized.</summary>
    ''' <remarks>True if a Core has been initialized.</remarks>
    Private m_bCoreIsInit As Boolean = False
    Private m_bEcoSimIsInit As Boolean

#End Region

#Region "Public Core Counters"

    ''' <summary>
    ''' Returns the value the Core holds for a given eCoreCounterTypes enumerator. These
    ''' values are referred to as Core Counters.
    ''' </summary>
    ''' <param name="counterType">The core counter to find a value for.</param>
    ''' <returns>Value of a core counter.</returns>
    ''' <remarks>
    ''' <para>This is used by any object that needs to know the size of one of the core counters.</para>
    ''' <para>For example:</para>
    ''' <code>
    ''' Dim core As cCore = cCore.GetInstance()
    ''' Dim iNumGroups As Integer = core.GetCoreCounter(eCoreCounterTypes.nGroups)
    ''' </code>
    ''' </remarks>
    Public Function GetCoreCounter(ByVal counterType As eCoreCounterTypes) As Integer
        Try
            Select Case counterType
                Case eCoreCounterTypes.NotSet
                    Return 0
                Case eCoreCounterTypes.nGroups
                    Return Me.nGroups
                Case eCoreCounterTypes.nFleets
                    Return Me.nFleets
                Case eCoreCounterTypes.nDetritus
                    Return Me.nDetritusGroups
                Case eCoreCounterTypes.nLivingGroups
                    Return Me.nLivingGroups
                Case eCoreCounterTypes.nHabitats
                    Return Me.nHabitats
                Case eCoreCounterTypes.nRegions
                    Return Me.nRegions
                Case eCoreCounterTypes.nMonths
                    Return cCore.N_MONTHS
                Case eCoreCounterTypes.nMPAs
                    Return Me.nMPAs
                Case eCoreCounterTypes.nEcospaceYears
                    Return Me.nEcospaceYears
                Case eCoreCounterTypes.nEcosimYears
                    Return Me.nEcosimYears
                Case eCoreCounterTypes.nEcospaceTimeSteps
                    Return Me.nEcospaceTimeSteps
                Case eCoreCounterTypes.nStanzas
                    Return Me.nStanzas
                Case eCoreCounterTypes.nMaxStanza
                    Return Me.nMaxStanza
                Case eCoreCounterTypes.nEcosimTimeSteps
                    Return Me.nEcosimTimeSteps
                Case eCoreCounterTypes.nTimeSeries
                    Return Me.nTimeSeries
                Case eCoreCounterTypes.nTimeSeriesApplied
                    Return Me.nTimeSeriesEnabled
                Case eCoreCounterTypes.nTimeSeriesYears
                    Return Me.nTimeSeriesYears
                Case eCoreCounterTypes.nTimeSeriesDatasets
                    Return Me.nTimeSeriesDatasets
                Case eCoreCounterTypes.nImportanceLayers
                    Return Me.nImportanceLayers
                    ' Case eCoreCounterTypes.nTrophicLevels
                    '     Return m_NetworkManager.nTrophicLevels
                Case eCoreCounterTypes.nRows
                    If m_EcospaceBasemap IsNot Nothing Then
                        Return Me.m_EcospaceBasemap.InRow
                    Else
                        Return 0
                    End If
                Case eCoreCounterTypes.nCols
                    If m_EcospaceBasemap IsNot Nothing Then
                        Return Me.m_EcospaceBasemap.InCol
                    Else
                        Return 0
                    End If
                Case eCoreCounterTypes.nEcopathAgeSteps
                    Return Me.nAgeSteps
                Case eCoreCounterTypes.nWeightClasses
                    Return Me.nWeightClasses
                Case eCoreCounterTypes.nTaxon
                    Return Me.nTaxon

                Case Else
                    'Debug.Assert(False, String.Format("{0}.GetCoreCounter() Invalid eCoreCounterTypes enumerator '{1}'.", Me.ToString(), counterType))
                    Return NULL_VALUE
            End Select

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".getCoreCounter() Error: " & ex.Message)
        End Try


    End Function

    ''' <summary>
    ''' Overloaded getCoreCounter() for counters that are specific to a group. E.g. nStanzasForStanzaGroup number of stanzas (life stages) in a StanzaGroup
    ''' </summary>
    ''' <param name="SizeType"></param>
    ''' <param name="iIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function GetCoreCounter(ByVal SizeType As eCoreCounterTypes, ByVal iIndex As Integer) As Integer

        Select Case SizeType

            Case eCoreCounterTypes.nMaxStanzaAge
                Try
                    Return m_Stanza.Age2(iIndex, m_Stanza.Nstanza(iIndex))
                Catch ex As Exception
                    cLog.Write(ex)
                    Return 0 '?
                End Try

            Case eCoreCounterTypes.nStanzasForStanzaGroup
                Try
                    Return m_Stanza.Nstanza(iIndex)
                Catch ex As Exception
                    cLog.Write(ex)
                    Return 0 '?
                End Try

            Case Else
                Debug.Assert(False, "Invalid Counter passed to getCoreCounter(SizeType,iIndex)")
        End Select

    End Function

    ''' <summary>
    ''' Total number of groups across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nGroups">eCoreCounterTypes.nGroups</see>.
    ''' </remarks>
    Public ReadOnly Property nGroups() As Integer
        Get
            Return m_EcoPathData.NumGroups
        End Get
    End Property

    ''' <summary>
    ''' Number of detritus groups across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nDetritus">eCoreCounterTypes.nDetritus</see>.
    ''' </remarks>
    Public ReadOnly Property nDetritusGroups() As Integer
        Get
            Return m_EcoPathData.NumDetrit
        End Get
    End Property

    ''' <summary>
    ''' Number of living groups across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nLivingGroups">eCoreCounterTypes.nLivingGroups</see>.
    ''' </remarks>
    Public ReadOnly Property nLivingGroups() As Integer
        Get
            Return m_EcoPathData.NumLiving
        End Get
    End Property

    ''' <summary>
    ''' Number of fishing fleets across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nFleets">eCoreCounterTypes.nFleets</see>.
    ''' </remarks>
    Public ReadOnly Property nFleets() As Integer
        Get
            Return m_EcoPathData.NumFleet
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace habitats.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nHabitats">eCoreCounterTypes.nHabitats</see>.
    ''' </remarks>
    Public ReadOnly Property nHabitats() As Integer
        Get
            Return m_EcoSpaceData.NoHabitats
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace regions.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nRegions">eCoreCounterTypes.nRegions</see>.
    ''' </remarks>
    Public ReadOnly Property nRegions() As Integer
        Get
            Return m_EcoSpaceData.NoRegions
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace MPAs.
    ''' </summary>
    Public ReadOnly Property nMPAs() As Integer
        Get
            Return m_EcoSpaceData.MPAno
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace Importance layers.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nImportanceLayers">eCoreCounterTypes.nImportanceLayers</see>.
    ''' </remarks>
    Public ReadOnly Property nImportanceLayers() As Integer
        Get
            Return Me.m_EcoSpaceData.nImportanceLayers
        End Get
    End Property

    ''' <summary>
    ''' Number of years to run an Ecospace model.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEcospaceYears">eCoreCounterTypes.nEcospaceYears</see>.
    ''' </remarks>
    Public ReadOnly Property nEcospaceYears() As Integer
        Get
            Return CInt(m_EcoSpaceData.TotalTime)
        End Get
    End Property

    ''' <summary>
    ''' Number time steps in an Ecospace model.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEcospaceYears">eCoreCounterTypes.nEcospaceYears</see>.
    ''' </remarks>
    Public ReadOnly Property nEcospaceTimeSteps() As Integer
        Get
            Return m_EcoSpaceData.nTimeSteps
        End Get
    End Property

    ''' <summary>
    ''' Number of years to run an Ecosim model.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEcosimYears">eCoreCounterTypes.nEcosimYears</see>.
    ''' </remarks>
    Public ReadOnly Property nEcosimYears() As Integer
        Get
            Return m_EcoSimData.NumYears
        End Get
    End Property

    ''' <summary>
    ''' Number of time steps in an Ecosim run.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEcosimTimeSteps">eCoreCounterTypes.nEcosimTimeSteps</see>.
    ''' </remarks>
    Public ReadOnly Property nEcosimTimeSteps() As Integer
        Get
            'Ecosim is always 12 timesteps per year
            'this should change to a constant that is the number of ecosim timesteps per year
            'it should not be n_months that is potential different
            Return m_EcoSimData.NTimes
        End Get
    End Property

    ''' <summary>
    ''' Max number of groups in a single stanza configuration over all stanza groups.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nMaxStanza">eCoreCounterTypes.MaxStanza</see>.
    ''' </remarks>
    Public ReadOnly Property nMaxStanza() As Integer
        Get
            Return m_Stanza.MaxStanza
        End Get
    End Property

    ''' <summary>
    ''' Number of stanza configurations.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nStanzas">eCoreCounterTypes.nStanzas</see>.
    ''' </remarks>
    Public ReadOnly Property nStanzas() As Integer
        Get
            Return m_Stanza.Nsplit
        End Get
    End Property

    ''' <summary>
    ''' Number of available time series.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nTimeSeries">eCoreCounterTypes.nTimeSeries</see>.
    ''' </remarks>
    Public ReadOnly Property nTimeSeries() As Integer
        Get
            Return m_TSData.nNumTimeSeries
        End Get
    End Property

    ''' <summary>
    ''' Number of applied time series.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nTimeSeriesApplied">eCoreCounterTypes.nTimeSeriesApplied</see>.
    ''' </remarks>
    Public ReadOnly Property nTimeSeriesEnabled() As Integer
        Get
            Return m_TSData.NdatType
        End Get
    End Property

    ''' <summary>
    ''' Number of applied time series.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nTimeSeriesApplied">eCoreCounterTypes.nTimeSeriesApplied</see>.
    ''' </remarks>
    Public ReadOnly Property nTimeSeriesYears() As Integer
        Get
            Return m_TSData.nMaxYears
        End Get
    End Property

    Public ReadOnly Property nTimeSeriesDatasets() As Integer
        Get
            Return m_TSData.nDatasets
        End Get
    End Property

    Public ReadOnly Property nAgeSteps() As Integer
        Get
            Return m_PSDData.NAgeSteps
        End Get
    End Property

    Public ReadOnly Property nWeightClasses() As Integer
        Get
            Return m_PSDData.NWeightClasses
        End Get
    End Property

    ''' <summary>
    ''' Get the number of taxonomy groups.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nTaxon">eCoreCounterTypes.nTaxon</see>.
    ''' </remarks>
    Public ReadOnly Property nTaxon() As Integer
        Get
            Return Me.m_EcoPathData.NumTaxon
        End Get
    End Property

#End Region 'Public core variables

#Region " Singleton "

    ''' <summary>The single instance of the core.</summary>
    ''' <remarks>This is the instance of the core that is supplied to the user via the GetInstance() method.</remarks>
    Private Shared __inst__ As cCore = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This provides a Singleton style interface for getting a reference to the core
    ''' </summary>
    ''' <returns>A Core instance</returns>
    ''' <remarks>
    ''' This will return the same instance of the core on each call.
    ''' For a different instance of the core use the New operator.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetInstance() As cCore

        'if the core has not been created then create a new cCore instance and return it.
        If __inst__ Is Nothing Then
            __inst__ = New cCore
            __inst__.InitCore()
        End If

        Return __inst__

    End Function

#End Region ' Singleton

#Region "Public Core Interfaces"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        Me.m_bCoreIsInit = False

        ' Create core data structures
        Me.m_EcoPathData = New cEcopathDataStructures
        Me.m_EcoSimData = New cEcosimDatastructures
        Me.m_EcoSpaceData = New cEcospaceDataStructures(Me.Messages)
        Me.m_Stanza = New cStanzaDatastructures
        Me.m_tracerData = New cContaminantTracerDataStructures
        Me.m_TSData = New cTimeSeriesDataStructures
        Me.m_MPAOptData = New cMPAOptDataStructures
        Me.m_PSDData = New cPSDDatastructures(Me.m_EcoPathData)
        Me.m_MSEData = New cMSEDataStructures(Me.m_EcoPathData, Me.m_EcoSimData)

        ' Create core state monitor and manager
        Me.m_StateMonitor = New cCoreStateMonitor(Me)
        Me.m_StateManager = New cCoreStateManager(Me)

        Me.m_SyncObj = System.Threading.SynchronizationContext.Current
        'if there is no current context then create a new one on this thread. 
        If (Me.m_SyncObj Is Nothing) Then Me.m_SyncObj = New System.Threading.SynchronizationContext()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the <see cref="cCoreStateMonitor">state monitor</see> that
    ''' reflects the running state and data state of this core instance.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property StateMonitor() As cCoreStateMonitor
        Get
            Return Me.m_StateMonitor
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the <see cref="cCoreStateManager">state manager</see> that
    ''' provides methods to bring the core execution state up-to-date.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property StateManager() As cCoreStateManager
        Get
            Return Me.m_StateManager
        End Get
    End Property


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Exposes the MessagePublisher instance so that an interface can add message handlers to the message publisher
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Messages() As cMessagePublisher
        Get
            Return m_publisher
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Export the Ecopath model to a new Datasource
    ''' </summary>
    ''' <param name="ds"><see cref="IEwEDataSource">DataSource</see> to save to</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>This will perform a full model save to the temporary datasource
    ''' passed to this method.</remarks>
    ''' -----------------------------------------------------------------------
    Friend Function Export(ByVal ds As IEwEDataSource) As Boolean
        ' Sanity check
        If ds Is Nothing Then Return False
        If Not TypeOf (ds) Is IEcopathDataSource Then Return False
        ' Perform full save
        Return DirectCast(ds, IEcopathDataSource).SaveModel()
    End Function

#Region " Batch operations "

    ''' <summary>
    ''' Enum describing the impact level of batch operations on EwE
    ''' </summary>
    ''' <remarks>
    ''' The value of bacth change level flags is crucial to the implementation of
    ''' determining the most serious level of impact. Please leave the values intact.
    ''' </remarks>
    Public Enum eBatchChangeLevelFlags As Integer
        Ecopath = 0
        Ecosim = 1
        Ecospace = 2
        Ecotracer = 3
        TimeSeries = 4
        NotSet = 42 ' Just the highest number, and a random value at that :p
    End Enum

    ''' <summary>
    ''' Enum describing the type of batch lock that is currently active.
    ''' </summary>
    Public Enum eBatchLockType As Integer
        ''' <summary>Lock is not active.</summary>
        NotSet = 0
        ''' <summary>Lock is set for updating values.</summary>
        Update
        ''' <summary>Lock is set for restructuring data, e.g. adding, removing or reordering items.</summary>
        Restructure
    End Enum

    ''' <summary>Batch operation lock type.</summary>
    Private m_batchLockType As eBatchLockType = eBatchLockType.NotSet
    ''' <summary>Batch level impact.</summary>
    Private m_batchChangeLevel As eBatchChangeLevelFlags = eBatchChangeLevelFlags.NotSet
    ''' <summary>Batch operation lock count.</summary>
    Private m_iBatchLock As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Begin a batch operation of additions and removals of Core objects.
    ''' All messages will be locked while a batch operation is active.
    ''' </summary>
    ''' <param name="batchLockType">The type of lock to set. Values are interpreted as follows:
    ''' <list type="table">
    ''' <item>
    ''' <term>NotSet</term>
    ''' <description>There is no lock active.</description>
    ''' </item>
    ''' <item>
    ''' <term>Update</term>
    ''' <description>Values will be modified during the lock. Upon releasing such a lock,
    ''' held messages will be sent and no data will be reloaded.</description>
    ''' </item>
    ''' <item>
    ''' <term>Restructure</term>
    ''' <description>Core data will be restructured during the lock. Upon releasing such a lock,
    ''' the core will reload affected components of the core.</description>
    ''' </item>
    ''' </list>
    ''' </param>
    ''' <returns>True if batch lock succesfully set.</returns>
    ''' <remarks>
    ''' <para>End the batch operation by calling <see cref="ReleaseBatchLock">ReleaseBatchLock</see>.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function SetBatchLock(ByVal batchLockType As eBatchLockType) As Boolean

        ' Need to save prior to restructuring
        If (batchLockType = eBatchLockType.Restructure) Then
            If Not Me.SaveChanges() Then Return False
        End If

        ' Set batch lock type
        Me.m_batchLockType = DirectCast(Math.Max(Me.m_batchLockType, batchLockType), eBatchLockType)

        ' Increase batch lock count
        Me.m_iBatchLock += 1
        ' Increase messages lock count to stop any messages from being sent while in a batch
        Me.Messages.SetMessageLock()

        If Me.m_iBatchLock = 1 Then Me.DataSource.BeginTransaction()

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' End a batch operation of additions and removals of Core objects.
    ''' Relevant data is reloaded and all locked messages will be sent to allow
    ''' listening user interfaces to catch up.
    ''' </summary>
    ''' <param name="batchChangeLevel">The level of impact on EwE of releasing the batch lock.
    ''' This level will be used to reload the most severe impact level when the last
    ''' batch lock is released.</param>
    ''' <param name="bCommit">States whether any database changes must be committed (true)
    ''' or rolled back (false).</param>
    ''' <returns>Always true.</returns>
    ''' <remarks>
    ''' This method completes a batch operation initiated via <see cref="SetBatchLock">SetBatchLock</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function ReleaseBatchLock(ByVal batchChangeLevel As eBatchChangeLevelFlags, _
            Optional ByVal bCommit As Boolean = True) As Boolean

        ' Sanity checks: validate batch lock type
        Debug.Assert(Me.m_batchLockType <> eBatchLockType.NotSet, "Cannot release a batch lock; no current lock active")

        ' Decrease batch lock count
        Me.m_iBatchLock -= 1

        ' Keep track of most serious impact level
        Me.m_batchChangeLevel = DirectCast(Math.Min(CInt(batchChangeLevel), CInt(Me.m_batchChangeLevel)), eBatchChangeLevelFlags)

        ' Last batch lock released?
        If (Me.m_iBatchLock = 0) Then

            Me.m_DataSource.EndTransaction(bCommit)

            ' Need to reload?
            If (Me.m_batchLockType = eBatchLockType.Restructure) Then

                ' Determine level of reload
                ' JS 24 March 2010: Only reload a scenario when the batch release affected that particular level.
                '                   In other words, adding or removing groups (batch level Ecopath) will NOT
                '                   cause batch level Ecosim and higher to automatically reload because Ecopath 
                '                   will most likely not run. This addresses issue #512
                Dim iEcosimScenarioToLoad As Integer = CInt(IIf(Me.m_batchChangeLevel = eBatchChangeLevelFlags.Ecosim, Me.m_EcoPathData.ActiveEcosimScenario, cCore.NULL_VALUE))
                Dim iEcospaceScenarioToLoad As Integer = CInt(IIf(Me.m_batchChangeLevel = eBatchChangeLevelFlags.Ecospace, Me.ActiveEcospaceScenarioIndex, cCore.NULL_VALUE))
                Dim iEcotracerScenarioToLoad As Integer = CInt(IIf(Me.m_batchChangeLevel = eBatchChangeLevelFlags.Ecotracer, Me.m_EcoPathData.ActiveEcotracerScenario, cCore.NULL_VALUE))
                Dim iDatasetToReload As Integer = 0

                If (Me.m_batchChangeLevel <= eBatchChangeLevelFlags.TimeSeries) Then
                    For ids As Integer = 1 To Me.nTimeSeriesDatasets
                        Dim ds As cTimeSeriesDataset = Me.TimeSeriesDataset(ids)
                        If ds.IsLoaded() Then iDatasetToReload = ids : Exit For
                    Next
                End If

                ' Reload restructured data
                ' JS 17 May 2010: If a scenario does not need reloading then the scenario is explicitly discarded.
                If (Me.m_batchChangeLevel = eBatchChangeLevelFlags.Ecopath) Then Me.LoadModel(DataSource)
                If (iEcosimScenarioToLoad >= 0) Then Me.LoadEcosimScenario(iEcosimScenarioToLoad) Else Me.m_StateMonitor.SetEcoSimLoaded(Me.m_batchChangeLevel >= eBatchChangeLevelFlags.Ecosim)
                If (iEcospaceScenarioToLoad >= 0) Then Me.LoadEcospaceScenario(iEcospaceScenarioToLoad) Else Me.m_StateMonitor.SetEcospaceLoaded(Me.m_batchChangeLevel >= eBatchChangeLevelFlags.Ecospace)
                If (iEcotracerScenarioToLoad >= 0) Then Me.LoadEcotracerScenario(iEcotracerScenarioToLoad) Else Me.m_StateMonitor.SetEcotracerLoaded(Me.m_batchChangeLevel >= eBatchChangeLevelFlags.Ecotracer)

                If (Me.m_batchChangeLevel <= eBatchChangeLevelFlags.TimeSeries) Then
                    Me.InitAndLoadEcosimTimeSeriesDatasets()
                    If (iDatasetToReload > 0) Then
                        Me.LoadTimeSeries(iDatasetToReload, True)
                    End If
                End If

            End If

            ' Clear batch change level
            Me.m_batchChangeLevel = eBatchChangeLevelFlags.NotSet
            ' Clear batch lock type
            Me.m_batchLockType = eBatchLockType.NotSet

            ' Broadcast data state change
            Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.True)

        End If

        ' Decrease messages lock count
        Me.Messages.RemoveMessageLock()

        Return True

    End Function

#End Region ' Batch operations

#Region " Groups"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a Group
    ''' </summary>
    ''' <param name="strName">Name of the group.</param>
    ''' <param name="sPP"><see cref="ePrimaryProductionTypes">Primary Production type</see> of the group (producer, consumer or detritus).</param>
    ''' <param name="iGroup">Position to insert group into the current group list. This position may be modified by this call.</param>
    ''' <param name="iGroupID">Database ID assigned to the new group.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddGroup(ByVal strName As String, ByVal sPP As Single, ByVal sVBK As Single, _
            ByRef iGroup As Integer, ByRef iGroupID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcopathDataSource Then Return False

        If iGroup < 1 And iGroup <> NULL_VALUE Then iGroup = 1 'less than 1 insert the new group as one

        ' iGroup value does not really matter. This addition may be part of a batch run; the datasource 
        ' will take care of proper iGroup value assignments
        'If iGroup > nGroups Then iGroup = nGroups + 1 'greater then ngroups append the new group to the end this means the new group is a detritus group?????

        ' Must specify an iGroup value
        Debug.Assert(iGroup <> NULL_VALUE)

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ' Start the actual work
        If (DirectCast(Me.DataSource, IEcopathDataSource).AddGroup(strName, sPP, sVBK, iGroup, iGroupID)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.EcoPath, eDataTypes.EcoPathGroupInput)
            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.EcoPath, eDataTypes.EcoPathGroupOutput)
            Me.DataAddedOrRemovedMessage("Fleet number of groups has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetInput)
            Me.DataAddedOrRemovedMessage("Stanza number of groups has changed.", eCoreComponentType.EcoPath, eDataTypes.Stanza)

            If m_bEcoSimIsInit And (m_EcoSimData.GroupDBID IsNot Nothing) Then
                Me.DataAddedOrRemovedMessage("EcoSim number of groups has changed.", eCoreComponentType.EcoSim, eDataTypes.EcoSimGroupInput)
            End If

            bSucces = True

        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    Public Function RemoveGroup(ByVal iGroup As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(Me.DataSource, IEcopathDataSource)
        If ds.RemoveGroup(Me.m_EcoPathData.GroupDBID(iGroup)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.EcoPath, eDataTypes.EcoPathGroupInput)
            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.EcoPath, eDataTypes.EcoPathGroupOutput)
            Me.DataAddedOrRemovedMessage("Fleet number of groups has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetInput)
            Me.DataAddedOrRemovedMessage("Stanza number of groups has changed.", eCoreComponentType.EcoSim, eDataTypes.Stanza)

            If m_bEcoSimIsInit And (m_EcoSimData.GroupDBID IsNot Nothing) Then
                'load the Ecosim Groups with the Ecosim data reloaded from the database above
                Me.DataAddedOrRemovedMessage("EcoSim number of groups has changed.", eCoreComponentType.EcoSim, eDataTypes.EcoSimGroupInput)
            End If

            bSucces = True
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    Public Function MoveGroup(ByVal iGroup As Integer, ByVal iIndex As Integer) As Boolean
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.MoveGroup(Me.m_EcoPathData.GroupDBID(iGroup), iIndex) Then

            Me.DataAddedOrRemovedMessage("Ecopath group order has changed.", eCoreComponentType.EcoPath, eDataTypes.EcoPathGroupInput)
            Me.DataAddedOrRemovedMessage("Ecopath group order has changed.", eCoreComponentType.EcoPath, eDataTypes.EcoPathGroupOutput)

            If m_bEcoSimIsInit And (m_EcoSimData.GroupDBID IsNot Nothing) Then
                'load the Ecosim Groups with the Ecosim data reloaded from the database above
                Me.DataAddedOrRemovedMessage("EcoSim group order has changed.", eCoreComponentType.EcoSim, eDataTypes.EcoSimGroupInput)
            End If

            bSucces = True
        End If

        ' Decrease batch count
        ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

#End Region ' Groups

#Region " Shapes: Forcing Mediation or Otherwise"

    'All public interaction with the Shapes is through the ShapeManagers 
    'so all Shape related functions of the Core are declared as Friend so they are not accessable to the Public

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Adds a shape to the core.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="DataType"></param>
    ''' <param name="newDBID"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Friend Function AddShape(ByVal strName As String, ByVal DataType As eDataTypes, ByRef newDBID As Integer, _
            Optional ByVal asData As Single() = Nothing, _
            Optional ByVal sYZero As Single = 0, Optional ByVal sYBase As Single = 0, _
            Optional ByVal sYEnd As Single = 0, Optional ByVal sSteep As Single = 0, _
            Optional ByVal shapeType As eShapeFunctionType = eShapeFunctionType.NotSet) As Boolean

        'the datasource will allocate space in the EcoSim data arrays
        Dim ds As IEcosimDatasource = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcosimDatasource Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        ds = DirectCast(Me.DataSource, IEcosimDatasource)

        bSucces = ds.AppendShape(strName, DataType, newDBID, asData, sYZero, sYBase, sYEnd, sSteep, shapeType)

        If bSucces = False Then
            'oops.....
            'do something
            'ToDo_jb this could throw an error back to the shape manager
        End If

        'At this time the shape manager has not had time to add the shape to it's list 
        'so sending a message or telling the other manager what has happend is premature.
        'The shape manager will handle telling the other shape managers that it has changed the underlying data
        Return bSucces

    End Function

    Friend Function RemoveShape(ByVal iDBID As Integer) As Boolean

        Dim ds As IEcosimDatasource = Nothing

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcosimDatasource Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        ds = DirectCast(Me.DataSource, IEcosimDatasource)
        'the datasource is responsible for 
        '1 removing the record from the database
        '2 resizing the Ecosim data arrays
        '3 reloading the Ecosim data arrays with the values from the database
        'The shape manager that asked for the remove will handle loading the Ecosim data back into the shape managers
        Return ds.RemoveShape(iDBID)
    End Function

#End Region

#End Region 'Public Core Interfaces

#Region "Private and Friend Core Functions" 'private functionality used by the core

    ''' <summary>
    ''' Initialize all core objects
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This initializes all the object that the core need to run a basic model (EcoPath). This does not load the model that happens in LoadModel(DataSource)</remarks>
    Public Function InitCore() As Boolean

        m_bCoreIsInit = False
        m_bEcoSimIsInit = False

        'Ecofunctions is needed by Ecopath so make sure it is created before Ecopath
        m_Functions = New cEcoFunctions

        'initialize the models
        'each models initialization will handle its own messages and flags
        Dim bsuccess As Boolean
        bsuccess = InitEcopath()
        bsuccess = bsuccess And InitPSDModel()
        bsuccess = bsuccess And InitEcoSim()
        bsuccess = bsuccess And InitEcoSpace()

        m_MonteCarlo = New cMonteCarloManager
        m_ConTracer = New cContaminantTracer
        m_gameManager = New cGameServerInterface(Me)

        If bsuccess Then
            m_bCoreIsInit = True
            Return True
        Else
            m_bCoreIsInit = False
            Return False
        End If

    End Function

    Private Function InitEcopath() As Boolean

        Try
            'build a new EcoPath Model object
            m_EcoPath = New Ecopath.cEcoPathModel(Me.m_Functions)
            m_EcoPath.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.EcoPathMessage_Handler, eCoreComponentType.EcoPath, eMessageType.Any, Me.m_SyncObj))
            m_EcoPath.m_stanza = m_Stanza
            m_EcoPath.m_psd = m_PSDData

            'the Ecopath Data belongs to the core instead of Ecopath so that it can be shared by all the models
            m_EcoPath.EcopathData = m_EcoPathData

            'protect against error loading the validators
            Try
                m_validators = New cValidatorManager(Me)
            Catch ex As Exception
                'the validation manager creates all the validators. Make sure we know if something went wrong
                Dim msg As cMessage = New cMessage(String.Format(My.Resources.CoreMessages.CORE_INIT_CRITICAL_VALIDATORS, ex.Message), _
                        eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical)
                'the message publisher is declared with the new operator so it already exists 
                m_publisher.AddMessage(msg)
                m_publisher.sendAllMessages()
                Return False
            End Try

        Catch ex As Exception
            'Major Error ???????
            Dim msg As cMessage = New cMessage(String.Format(My.Resources.CoreMessages.CORE_INIT_CRITICAL_GENERIC, ex.Message), _
                    eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical)
            'the message publisher is declared with the new operator so it already exists 
            m_publisher.AddMessage(msg)
            m_publisher.sendAllMessages()
            Return False
        End Try

        Return True

    End Function

    Private Function InitPSDModel() As Boolean
        Try
            m_psdModel = New cPSDModel
            m_psdModel.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.PSDMessage_Handler, eCoreComponentType.EcoPath, eMessageType.Any, Me.m_SyncObj))

            m_psdModel.m_Data = m_EcoPathData
            m_psdModel.m_stanza = m_Stanza
            m_psdModel.m_psd = m_PSDData
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Send a Data Changed message
    ''' </summary>
    ''' <param name="message">Test of the message</param>
    ''' <param name="dataType">eDataTypes enumerator for the type of data</param>
    ''' <remarks>This is just to wrap the creation and sending of a datachanged message to clean up the code a bit</remarks>
    Private Sub DataAddedOrRemovedMessage(ByRef message As String, ByVal messageSource As eCoreComponentType, ByVal dataType As eDataTypes, Optional ByVal vars() As cVariableStatus = Nothing)

        ' Create msg
        Dim msg As New cMessage(message, eMessageType.DataAddedOrRemoved, messageSource, eMessageImportance.Maintenance, dataType)
        ' Any variables to attach?
        If vars IsNot Nothing Then
            ' #Yes: attach variables
            For Each v As cVariableStatus In vars
                msg.AddVariable(v)
            Next
        End If
        ' Send
        m_publisher.SendMessage(msg)

    End Sub

    ''' <summary>
    ''' Is the a Biomass/Area for all detritus groups
    ''' </summary>
    ''' <returns>True if all detritus groups have a Biomass/Area (density)</returns>
    ''' <remarks>This was part of FindMissing in EwE5</remarks>
    Private Function checkBiomassForDetritus() As Boolean

        'check make sure there is a biomass for all Detritus groups
        For i As Integer = m_EcoPathData.NumLiving + 1 To m_EcoPathData.NumGroups
            If m_EcoPathData.BH(i) < 0 Then

                'toDo:  message in EcoSim.checkBiomassForDetritus() missing biomass
                Return False '?????

                'jb from EwE5
                'this was only done once here it will be done every time
                'If DoneAlready = False Then
                '    RetVal = MsgBox("Enter a 'biomass' for all detritus groups before proceeding to Ecosim", vbOKCancel)
                '    If RetVal = vbCancel Then DoneAlready = True
                'End If
            End If
        Next

        Return True

    End Function

    Private Function FindObjectByDBID(ByVal lItems As IList, ByVal iDBID As Integer) As cCoreInputOutputBase
        Dim obj As cCoreInputOutputBase = Nothing

        For Each objTest As Object In lItems
            If TypeOf (objTest) Is cCoreInputOutputBase Then
                If Object.Equals(iDBID, DirectCast(objTest, cCoreInputOutputBase).DBID) Then
                    obj = DirectCast(objTest, cCoreInputOutputBase)
                    Exit For
                End If
            End If
        Next
        Return obj
    End Function

    Private Function FindObjectByIndex(ByVal lItems As IList, ByVal iIndex As Integer) As cCoreInputOutputBase
        Dim obj As cCoreInputOutputBase = Nothing

        For Each objTest As Object In lItems
            If TypeOf (objTest) Is cCoreInputOutputBase Then
                If Object.Equals(iIndex, DirectCast(objTest, cCoreInputOutputBase).Index) Then
                    obj = DirectCast(objTest, cCoreInputOutputBase)
                    Exit For
                End If
            End If
        Next
        Return obj
    End Function

#End Region 'Private and Friend Core Functions

#Region "Time series"

#Region " Import "

    ''' <summary>
    ''' Import one time series into the database.
    ''' </summary>
    ''' <param name="ts">The <see cref="cTimeSeries">cTimeSeries-derived</see> object to import.</param>
    ''' <returns>True if succesful.</returns>
    Public Function ImportEcosimTimeSeries(ByVal ts As cTimeSeriesImport, ByVal iDataset As Integer) As Boolean

        Dim bSucces As Boolean = True

        If Not (TypeOf DataSource Is IEcosimDatasource) Then Return False
        bSucces = DirectCast(DataSource, IEcosimDatasource).ImportTimeSeries(ts, iDataset)
        Return bSucces

    End Function

#End Region ' Import

#Region " Init and loading "

    Private Function InitAndLoadEcosimTimeSeriesDatasets() As Boolean

        Dim tsd As cTimeSeriesDataset = Nothing

        Try

            Me.m_timeSeriesDatasets.Clear()

            For iDS As Integer = 1 To Me.m_TSData.nDatasets
                tsd = New cTimeSeriesDataset(Me, Me.m_TSData.nDatasetNumTimeSeries(iDS))
                tsd.AllowValidation = False
                tsd.DBID = Me.m_TSData.iDatasetDBID(iDS)
                tsd.Index = iDS
                tsd.Name = Me.m_TSData.strDatasetNames(iDS)
                tsd.FirstYear = Me.m_TSData.nDatasetFirstYear(iDS)
                tsd.NumYears = Me.m_TSData.nDatasetNumYears(iDS)
                tsd.AllowValidation = True

                Me.m_timeSeriesDatasets.Add(tsd)

            Next

            ' Reset number of loaded and applied time series
            Me.m_TSData.ClearTimeSeries()
            ' Set number of groups
            Me.m_TSData.nGroups = Me.nGroups

        Catch ex As Exception
            Debug.Assert(False)
            Return False
        End Try
        Return True

    End Function

    ''' <summary>
    ''' Initialize Time Series interface objects.
    ''' </summary>
    Private Function InitEcosimTimeSeries() As Boolean

        Dim ts As cTimeSeries = Nothing

        m_timeSeriesGroup.Clear()
        m_timeSeriesFleet.Clear()

        ' Create time series
        For iSeries As Integer = 1 To Me.nTimeSeries
            ts = cTimeSeriesFactory.CreateTimeSeries(DirectCast(Me.m_TSData.TimeSeriesType(iSeries), eTimeSeriesType), Me, Me.m_TSData.iTimeSeriesDBID(iSeries))
            ts.Index = iSeries
            Select Case ts.DataType
                Case eDataTypes.GroupTimeSeries
                    Me.m_timeSeriesGroup.Add(ts)
                Case eDataTypes.FleetTimeSeries
                    Me.m_timeSeriesFleet.Add(ts)
                Case Else
                    ' Other types of TS are not supported in the core
                    Debug.Assert(False)
            End Select
        Next iSeries

        Return True
    End Function

    ''' <summary>
    ''' Populate Time Series interface objects
    ''' </summary>
    Private Function LoadEcosimTimeSeries() As Boolean

        Dim tsd As cTimeSeriesDataset = Nothing

        Dim iNumYears As Integer = 0
        Dim bSucces As Boolean = True

        ' Clear all time series from existing datasets
        For Each tsd In Me.m_timeSeriesDatasets
            tsd.Clear()
        Next


        Try
            If (Me.ActiveTimeSeriesDatasetIndex > 0) Then
                tsd = Me.TimeSeriesDataset(Me.ActiveTimeSeriesDatasetIndex)
                iNumYears = Me.m_TSData.nDatasetNumYears(Me.ActiveTimeSeriesDatasetIndex)
            End If

            For Each ts As cGroupTimeSeries In Me.m_timeSeriesGroup

                ts.LockUpdates()

                ts.Name = Me.m_TSData.strName(ts.Index)
                ts.Index = ts.Index
                ts.DBID = Me.m_TSData.iTimeSeriesDBID(ts.Index)
                ts.TimeSeriesType = DirectCast(Me.m_TSData.TimeSeriesType(ts.Index), eTimeSeriesType)
                ts.DatPool = Me.m_TSData.iPool(ts.Index)
                ts.WtType = Me.m_TSData.sWeight(ts.Index)

                'DatSS and DatQ are not part of m_TSData yet
                ts.DataSS = Me.m_TSData.sDatSS(ts.Index)
                ts.DataQ = Me.m_TSData.sDatQ(ts.Index)
                ts.eDataQ = Me.m_TSData.sEDatQ(ts.Index)

                ts.ResizeData(iNumYears)
                For iYear As Integer = 1 To iNumYears
                    ts.DatVal(iYear) = Me.m_TSData.sValues(iYear, ts.Index)
                Next iYear

                ts.Enabled = Me.m_TSData.bEnable(ts.Index)
                ts.UnlockUpdates(False)

                tsd.Add(ts)
            Next

            For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet

                ts.LockUpdates()

                ts.Name = Me.m_TSData.strName(ts.Index)
                ts.Index = ts.Index
                ts.DBID = Me.m_TSData.iTimeSeriesDBID(ts.Index)
                ts.TimeSeriesType = DirectCast(Me.m_TSData.TimeSeriesType(ts.Index), eTimeSeriesType)
                ts.DatPool = Me.m_TSData.iPool(ts.Index)
                ts.WtType = Me.m_TSData.sWeight(ts.Index)

                'DatSS and DatQ are not part of m_TSData yet
                ts.DataSS = Me.m_TSData.sDatSS(ts.Index)
                ts.DataQ = Me.m_TSData.sDatQ(ts.Index)
                ts.eDataQ = Me.m_TSData.sEDatQ(ts.Index)

                ts.ResizeData(iNumYears)
                For iYear As Integer = 1 To iNumYears
                    ts.DatVal(iYear) = Me.m_TSData.sValues(iYear, ts.Index)
                Next iYear

                ts.Enabled = Me.m_TSData.bEnable(ts.Index)
                ts.UnlockUpdates(False)

                tsd.Add(ts)
            Next

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Init and loading

#Region " Update "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store TS Input/output data in the core.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function UpdateEcosimTimeSeries() As Boolean
        Dim bSucces As Boolean = (Me.UpdateEcosimGroupTimeSeries() And Me.UpdateEcosimFleetTimeSeries())
        Return bSucces
    End Function

    Private Function UpdateEcosimGroupTimeSeries() As Boolean

        Dim bSucces As Boolean = True
        Try
            For Each ts As cGroupTimeSeries In Me.m_timeSeriesGroup

                ' Validate whether TS will remain in its category (group)
                Debug.Assert(cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType) = eTimeSeriesCategoryType.Group, "Cannot change TS to a different category")
                Me.m_TSData.TimeSeriesType(ts.Index) = ts.TimeSeriesType
                Me.m_TSData.strName(ts.Index) = ts.Name
                Me.m_TSData.iPool(ts.Index) = ts.DatPool
                Me.m_TSData.sWeight(ts.Index) = ts.WtType

                'DatSS and DatQ are computed so they are not updated from the interface
                'Me.m_TSData.datass(ts.Index) = ts.DataQ
                'Me.m_TSData.Datq(ts.Index) = ts.DataSS 

                ' Update core DatVal
                For iYear As Integer = 1 To ts.XMax
                    Me.m_TSData.sValues(iYear, ts.Index) = ts.DatVal(iYear)
                Next iYear

                Me.m_TSData.bEnable(ts.Index) = ts.Enabled

            Next

            DataSource.SetChanged(eCoreComponentType.EcoSim)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function UpdateEcosimFleetTimeSeries() As Boolean

        Dim bSucces As Boolean = True
        Try
            For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet

                ' Validate whether TS will remain in its category (fleet)
                Debug.Assert(cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType) = eTimeSeriesCategoryType.Fleet, "Cannot change TS to a different category")
                Me.m_TSData.TimeSeriesType(ts.Index) = ts.TimeSeriesType

                Me.m_TSData.strName(ts.Index) = ts.Name
                Me.m_TSData.iPool(ts.Index) = ts.DatPool
                Me.m_TSData.sWeight(ts.Index) = ts.WtType

                'DatSS and DatQ are computed so they are not updated from the interface
                'Me.m_TSData.datass(ts.Index) = ts.DataQ
                'Me.m_TSData.Datq(ts.Index) = ts.DataSS 

                ' Update core DatVal
                For iYear As Integer = 1 To ts.XMax
                    Me.m_TSData.sValues(iYear, ts.Index) = ts.DatVal(iYear)
                Next iYear

                Me.m_TSData.bEnable(ts.Index) = ts.Enabled

            Next

            DataSource.SetChanged(eCoreComponentType.EcoSim)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' Update

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the index of the active <see cref="cTimeSeriesDataset">TimeSeries Dataset</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveTimeSeriesDatasetIndex() As Integer
        Get
            Return Me.m_TSData.ActiveDatasetIndex
        End Get
    End Property

    Public Function TimeSeriesDataset(ByVal iDatasetIndex As Integer) As cTimeSeriesDataset

        ' Sanity check
        Debug.Assert(iDatasetIndex > 0 And iDatasetIndex <= Me.m_TSData.nDatasets)
        Return Me.m_timeSeriesDatasets(iDatasetIndex)

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strDataset">Name of dataset that was loaded</param>
    ''' <param name="strError"></param>
    ''' <remarks></remarks>
    Private Sub SendTimeSeriesLoadMessage(ByVal strDataset As String, Optional ByVal strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            If String.IsNullOrEmpty(strDataset) Then
                strText = My.Resources.CoreMessages.TIMESERIES_UNLOAD_SUCCESS
            Else
                strText = String.Format(My.Resources.CoreMessages.TIMESERIES_LOAD_SUCCESS, strDataset)
            End If
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.TimeSeries, eMessageImportance.Information, eDataTypes.TimeSeriesDataset)
        Else
            If String.IsNullOrEmpty(strDataset) Then
                strText = String.Format(My.Resources.CoreMessages.TIMESERIES_UNLOAD_FAILED, strError)
            Else
                strText = String.Format(My.Resources.CoreMessages.TIMESERIES_LOAD_FAILED, strDataset, strError)
            End If
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.TimeSeries, eMessageImportance.Warning, eDataTypes.TimeSeriesDataset)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load (and optionally apply) a single time series dataset
    ''' </summary>
    ''' <param name="tsd">The dataset to load. Provide 'nothing' to unload any dataset.</param>
    ''' <param name="bApply">Flag stating whether loaded time series should be applied immediately.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadTimeSeries(ByVal tsd As cTimeSeriesDataset, Optional ByVal bApply As Boolean = False) As Boolean
        Dim iIndex As Integer = 0
        If tsd IsNot Nothing Then iIndex = tsd.Index
        Return Me.LoadTimeSeries(iIndex, bApply)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load (and optionally apply) a single time series dataset
    ''' </summary>
    ''' <param name="iDataset">Index of the dataset to load. Provide 0 to unload any dataset.</param>
    ''' <param name="bApply">Flag stating whether loaded time series should be applied immediately.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadTimeSeries(ByVal iDataset As Integer, Optional ByVal bApply As Boolean = False) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity check
        If ((iDataset < 0) Or (iDataset > Me.nTimeSeriesDatasets)) Then Return bSucces

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return bSucces

        ' Ask for saving
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return bSucces

        If (TypeOf Me.m_DataSource Is IEcosimDatasource) Then
            Dim sds As IEcosimDatasource = DirectCast(Me.m_DataSource, IEcosimDatasource)

            ' Can laod dataset succesfully?
            If sds.LoadTimeSeriesDataset(iDataset) Then
                ' #Yes: Can init core interface objects succesfully?
                If Me.InitEcosimTimeSeries() Then
                    ' #Yes: Can populate core interface objects succesfully?
                    If (Me.LoadEcosimTimeSeries()) Then
                        ' Need to apply too?
                        If (bApply = True) Then
                            ' #Yes: Apply
                            For Each ts As cTimeSeries In Me.m_timeSeriesGroup : ts.Enabled = True : Next
                            For Each ts As cTimeSeries In Me.m_timeSeriesFleet : ts.Enabled = True : Next
                            Me.UpdateTimeSeries()
                        End If
                        ' Send messages
                        If iDataset > 0 Then
                            Me.SendTimeSeriesLoadMessage(Me.TimeSeriesDataset(iDataset).Name)
                        Else
                            Me.SendTimeSeriesLoadMessage("")
                        End If
                        ' Flag as succesful
                        bSucces = True
                    End If
                End If
                ' Invalidate Ecosim outputs
                Me.m_StateMonitor.SetEcoSimLoaded(True, TriState.True)
            End If
        End If
        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain a cTimeSeries-derived instance from the core.
    ''' </summary>
    ''' <param name="iIndex">One-based index indicating the time series to obtain.</param>
    ''' <returns>A cTimeSeries-derived object, or Nothing if an error occurs.</returns>
    ''' <remarks>
    ''' How to use this:
    ''' <code>
    ''' Dim core As cCore = cCore.GetInstance()
    ''' Dim ts As cTimeSeries = Nothing
    ''' Dim iYearStart As Integer = Integer.MaxValue
    ''' Dim iYearEnd As Integer = Integer.MinValue
    ''' Dim asValues() As Single = Nothing
    ''' Dim sX As Single = 0.0
    ''' Dim sY As Single = 0.0
    ''' 
    ''' For i As Integer = 1 To core.nTimeSeries
    '''    ts = core.EcosimTimeSeries(i)
    '''    ' Determine year range
    '''    If ts.TimeSeriesType = eTimeSeriesType.BiomassForcing Then
    '''       iYearStart = Math.Min(iYearStart, ts.FirstYear)
    '''       iYearEnd = Math.Max(iYearEnd, ts.FirstYear + ts.NumYears)
    '''    End If
    ''' Next i
    ''' 
    ''' ' Now plot
    ''' For i As Integer = 1 To core.nTimeSeries
    '''    ts = core.EcosimTimeSeries(i)
    '''    ' Determine year range
    '''    If ts.TimeSeriesType = eTimeSeriesType.BiomassForcing Then
    '''       asValues = ts.Values()
    '''       For iValue As Integer = 0 To asValues.Length - 1
    '''          sX = CSng(ts.FirstYear - iYearStart)
    '''          sY = asValues(iValue)
    ''' 
    '''          ' Plot here...
    ''' 
    '''       Next iValue
    '''    End If
    ''' Next i
    ''' </code>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function EcosimTimeSeries(ByVal iIndex As Integer) As cTimeSeries
        ' Ouch, this is suddenly not so straight-forward anymore now TS are stored in two different strong-typed lists...
        Dim bFound As Boolean = False

        For Each ts As cTimeSeries In Me.m_timeSeriesGroup
            If ts.Index = iIndex Then Return ts
        Next

        For Each ts As cTimeSeries In Me.m_timeSeriesFleet
            If ts.Index = iIndex Then Return ts
        Next
        Debug.Assert(False, "Index out of range")
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply all <see cref="cTimeSeries">Time Series</see> that are flagged as
    ''' <see cref="cTimeSeries.Enabled">Enabled</see> to the Ecosim model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function UpdateTimeSeries() As Boolean

        Dim lstEffortToReset As New List(Of Integer)
        Dim bChanged As Boolean = False

        ' Update enable flags and weights
        For Each ts As cGroupTimeSeries In Me.m_timeSeriesGroup
            bChanged = bChanged Or (Me.m_TSData.bEnable(ts.Index) <> ts.Enabled)
            Me.m_TSData.bEnable(ts.Index) = ts.Enabled
            bChanged = bChanged Or (Me.m_TSData.sWeight(ts.Index) <> ts.WtType)
            Me.m_TSData.sWeight(ts.Index) = ts.WtType
        Next

        For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet
            'build a list of all disabled Effort timeseries that need to be reset to default (one)
            If ts.TimeSeriesType = eTimeSeriesType.FishingEffort Then
                If Me.m_TSData.bEnable(ts.Index) = True And ts.Enabled = False Then
                    'Timeseries is being disabled so keep the index to reset effort from
                    lstEffortToReset.Add(ts.DatPool)
                End If
            End If
            Me.m_TSData.bEnable(ts.Index) = ts.Enabled
        Next

        ' Load enabled TS
        Me.m_TSData.loadEnabled()
        ' Ecosim needs to run again
        Me.StateMonitor.SetEcoSimLoaded(True)

        'setEcosimRunLength() will call DoDatValCalculations to re-load forcing data
        Me.setEcosimRunLength(Me.m_TSData.NdatYear, False)

        'reset all efforts that were unloaded/disabled
        Me.m_EcoSimData.setEffortToDefault(lstEffortToReset)

        'set fishing mortality from Fleet Effort/Gear
        Dim QYear() As Single
        ReDim QYear(Me.nGroups)
        For i As Integer = 1 To Me.nGroups
            QYear(i) = 1
        Next
        Me.m_EcoSim.SetFFromGear(QYear)

        Me.m_SearchManagers(eDataTypes.FitToTimeSeries).Load()

        Me.m_ShapeManagers.Item(eDataTypes.FishingEffort).Load()
        Me.m_ShapeManagers.Item(eDataTypes.FishMort).Load()

        'tell the interface that the shapes have changed
        Me.m_publisher.AddMessage(New cMessage("Fish rate shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishingEffort))
        Me.m_publisher.AddMessage(New cMessage("Fish mort shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishMort))

        If (bChanged) Then
            Me.DataAddedOrRemovedMessage("Time Series have been updated", eCoreComponentType.TimeSeries, eDataTypes.NotSet)
            DataSource.SetChanged(eCoreComponentType.TimeSeries)
            Me.m_StateMonitor.UpdateDataState(DataSource)
        End If

        Me.Messages.sendAllMessages()

        Return True

    End Function

    Public Function HasTimeSeries() As Boolean
        Return (Me.m_TSData.nNumTimeSeries > 0)
    End Function

    Public Function HasAppliedTimeSeries() As Boolean
        Return (Me.m_TSData.NdatType > 0)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an Ecosim Time Series to the datasource.
    ''' </summary>
    ''' <param name="strName">Name of the new Time Series to add.</param>
    ''' <param name="iPool">Index of item to assign this TS to.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of the time series.</param>
    ''' <param name="asValues">Initial values to set in the TS.</param>
    ''' <param name="iDBID">Database ID assigned to the new TS.</param>
    ''' -----------------------------------------------------------------------
    Public Function AddTimeSeries(ByVal strName As String, _
            ByVal iPool As Integer, ByVal timeSeriesType As eTimeSeriesType, _
            ByVal sWeight As Single, ByVal asValues() As Single, _
            ByRef iDBID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Safety check
        If Not TypeOf DataSource Is IEcosimDatasource Then Return False

        ' Set bach lock for adding and removing items
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        Try
            ' Try to add TS to the datasource
            If DirectCast(DataSource, IEcosimDatasource).AppendTimeSeries(strName, iPool, timeSeriesType, sWeight, asValues, iDBID) Then
                Me.DataAddedOrRemovedMessage("Ecosim number of time series has changed.", eCoreComponentType.TimeSeries, eDataTypes.NotSet)
                bSucces = True
            End If
        Catch ex As Exception
            ' Woops
        End Try

        ' Release batch lock
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.TimeSeries)
        ' Report suces
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an Ecosim Time Series from the datasource.
    ''' </summary>
    ''' <param name="TS"><see cref="cTimeSeries">Time Series instance</see> to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Function RemoveTimeSeries(ByVal TS As cTimeSeries) As Boolean
        Return Me.RemoveTimeSeries(TS.Index)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an Ecosim Time Series from the datasource.
    ''' </summary>
    ''' <param name="iTS">Index of the time series to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Function RemoveTimeSeries(ByVal iTS As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Safety check
        If Not TypeOf DataSource Is IEcosimDatasource Then Return False

        ' Set bach lock for adding and removing items
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        Try
            ' Try to add TS to the datasource
            If DirectCast(DataSource, IEcosimDatasource).RemoveTimeSeries(iTS) Then
                Me.DataAddedOrRemovedMessage("Ecosim number of time series has changed.", eCoreComponentType.TimeSeries, eDataTypes.NotSet)
                bSucces = True
            End If
        Catch ex As Exception
            ' Woops
            bSucces = False
        End Try

        Me.ReleaseBatchLock(eBatchChangeLevelFlags.TimeSeries)

        ' Report suces
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Append a time series dataset to the core.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <param name="strAuthor"></param>
    ''' <param name="strContact"></param>
    ''' <param name="iFirstYear"></param>
    ''' <param name="iNumYears"></param>
    ''' <param name="iDataset">Index of the new time series dataset if the 
    ''' operation completed succesfully.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AppendTimeSeriesDataset(ByVal strName As String, _
                                            ByVal strDescription As String, _
                                            ByVal strAuthor As String, _
                                            ByVal strContact As String, _
                                            ByVal iFirstYear As Integer, _
                                            ByVal iNumYears As Integer, _
                                            ByRef iDataset As Integer) As Boolean

        Dim ds As IEcosimDatasource = Nothing
        Dim iDatasetID As Integer = 0

        ' Safety check
        If DataSource Is Nothing Then Return False
        If Not TypeOf DataSource Is IEcosimDatasource Then Return False

        If Me.m_StateMonitor.HasEcosimLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        Try

            ds = DirectCast(DataSource, IEcosimDatasource)
            If ds.AppendTimeSeriesDataset(strName, strDescription, strAuthor, strContact, iFirstYear, iNumYears, iDatasetID) Then

                Me.InitAndLoadEcosimTimeSeriesDatasets()
                Me.DataAddedOrRemovedMessage("Number of time series datasets has changed.", eCoreComponentType.TimeSeries, eDataTypes.TimeSeriesDataset)
                iDataset = Array.IndexOf(Me.m_TSData.iDatasetDBID, iDatasetID)
                'If Me.LoadTimeSeries(iDataset, False) Then
                '    ' Update enabled TS
                '    Me.m_TSData.loadEnabled()
                '    Return True
                'End If
                Return True
            End If

        Catch ex As Exception

        End Try
        Return False

    End Function

    Public Function RemoveTimeSeriesDataset(ByVal iDatasetIndex As Integer) As Boolean
        Return Me.RemoveTimeSeriesDataset(Me.TimeSeriesDataset(iDatasetIndex))
    End Function

    Public Function RemoveTimeSeriesDataset(ByVal dataset As cTimeSeriesDataset) As Boolean
        Dim bSucces As Boolean = False

        ' Safety check
        If Not TypeOf DataSource Is IEcosimDatasource Then Return False
        Try
            ' Try to add TS to the datasource
            If DirectCast(DataSource, IEcosimDatasource).RemoveTimeSeriesDataset(dataset.Index) Then
                Me.DataAddedOrRemovedMessage("Ecosim number of time series has changed.", eCoreComponentType.TimeSeries, eDataTypes.NotSet)
                bSucces = True
            End If
        Catch ex As Exception
            ' Woops
            bSucces = False
        End Try

        ' Report suces
        Return bSucces

    End Function

#End Region ' Public interfaces

#End Region ' Time series

#Region "Generic helper methods"

    ''' <summary>
    ''' Creates a new cMessage Object
    ''' </summary>
    ''' <param name="message">Message to send</param>
    ''' <param name="source">Source of the message</param>
    ''' <param name="MessageType">Type of message</param>
    ''' <returns>A new cMessage Object</returns>
    ''' <remarks>Used as a simple way to build a new object</remarks>
    Private Function CreateMessage(ByVal message As String, ByVal source As eCoreComponentType, ByVal MessageType As eMessageType) As cMessage
        Dim msg As New cMessage
        msg.Message = message
        msg.Source = source
        msg.Type = MessageType
        Return msg
    End Function

    Public Function SaveChanges(Optional ByVal bQuiet As Boolean = False, _
                                Optional ByVal savelevel As eBatchChangeLevelFlags = eBatchChangeLevelFlags.Ecopath) As Boolean

        Dim fm As cFeedbackMessage = Nothing
        Dim strPrompt As String = ""

        ' For later use
        ' Save level is the MINIMUM level of data to save. For instance, when
        '    loading a new Ecospace scenario, any pending Ecospace changes have to
        '    be stored but there is no need to save Sim or Path. A savelevel value 
        '    of Ecospace would achieve this.

        ' Hang on, can we do this at all?
        If (Me.m_DataSource Is Nothing) Then Return True

        ' In a batch?
        If (Me.m_iBatchLock > 0) Then Return True

        ' Assess tracer
        Dim bIsModified As Boolean = Me.m_StateMonitor.IsEcotracerModified
        If savelevel = eBatchChangeLevelFlags.Ecotracer Then
            If Not bIsModified Then Return True
        End If
        ' Assess ecospace
        bIsModified = bIsModified Or Me.m_StateMonitor.IsEcospaceModified
        If savelevel = eBatchChangeLevelFlags.Ecospace Then
            If Not bIsModified Then Return True
        End If
        ' Assess Ecosim
        bIsModified = bIsModified Or Me.m_StateMonitor.IsEcosimModified
        If savelevel = eBatchChangeLevelFlags.Ecosim Then
            If Not bIsModified Then Return True
        End If
        ' Assess Ecopath
        If Not Me.m_StateMonitor.IsModified Then Return True

        ' Prepare feedback message
        strPrompt = My.Resources.CoreMessages.PROMPT_SAVE_CHANGES
        fm = New cFeedbackMessage(strPrompt, _
                                  eCoreComponentType.Core, eMessageType.Any, _
                                  eMessageImportance.Maintenance, cFeedbackMessage.eReplyStyle.YES_NO_CANCEL)

        If (bQuiet) Then
            ' Auto-affirm
            fm.Reply = cFeedbackMessage.eReply.YES
        Else
            ' Send and see what happens
            Me.m_publisher.SendMessage(fm)
        End If

        ' Hmm...
        Select Case fm.Reply

            Case cFeedbackMessage.eReply.CANCEL
                ' Do not save
                Return False

            Case cFeedbackMessage.eReply.YES

                ' Plug-ins
                If Me.m_StateMonitor.IsPluginModified Then
                    If Not Me.PluginManager.SaveModel(Me.m_DataSource) Then
                        Return False
                    Else
                        Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                    End If
                End If

                ' Ecotracer
                If (savelevel <= eBatchChangeLevelFlags.Ecotracer) Then
                    If Me.m_StateMonitor.IsEcotracerModified Then
                        If Not Me.SaveEcotracerScenario() Then
                            Return False
                        Else
                            Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                        End If
                    End If
                End If

                ' Ecospace
                If (savelevel <= eBatchChangeLevelFlags.Ecospace) Then
                    If Me.m_StateMonitor.IsEcospaceModified Then
                        If Not Me.SaveEcospaceScenario() Then
                            Return False
                        Else
                            Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                        End If
                    End If
                End If

                ' Ecosim
                If (savelevel <= eBatchChangeLevelFlags.Ecosim) Then
                    If Me.m_StateMonitor.IsEcosimModified Then
                        If Not Me.SaveEcosimScenario() Then
                            Return False
                        Else
                            Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                        End If
                    End If
                End If

                ' The bottom of it all
                If (savelevel <= eBatchChangeLevelFlags.Ecopath) Then
                    If Me.m_StateMonitor.IsEcopathModified Or Me.m_StateMonitor.IsDatasourceModified Then
                        If Not Me.SaveModel() Then
                            ' VERIFY_JS: Discuss what to do here. Prompt user how to proceed?
                            Return False
                        Else
                            Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                        End If
                    End If
                End If

            Case cFeedbackMessage.eReply.NO
                ' Forget changes
                Me.DiscardChanges()

        End Select

        ' All well, proceed.
        Return True

    End Function

    Public Function DiscardChanges() As Boolean

        ' Hang on, can we do this at all?
        If (Me.m_DataSource Is Nothing) Then Return False
        Me.m_DataSource.ClearChanged()
        Me.m_StateMonitor.UpdateDataState(Me.m_DataSource)

    End Function

    ''' <summary>
    ''' Get/set the path for core processes to write output information to.
    ''' </summary>
    Public Property OutputPath() As String
        Get
            If String.IsNullOrEmpty(m_strOutputPath) Then Return System.Windows.Forms.Application.StartupPath
            Return Me.m_strOutputPath
        End Get
        Set(ByVal value As String)
            Me.m_strOutputPath = value
        End Set
    End Property

#End Region 'Generic helper methods

#Region " Datasource "

    ''' <summary>
    ''' The <see cref="IEwEDataSource">data source</see> that the core will use
    ''' for reading and writing model data.
    ''' </summary>
    Public Property DataSource() As IEwEDataSource
        Get
            Return Me.m_DataSource
        End Get
        Private Set(ByVal value As IEwEDataSource)
            ' Assign new DS
            Me.m_DataSource = value
        End Set
    End Property

    Private Function UpdateDatasource(ByVal ds As IEwEDataSource) As Boolean

        ' Run database updates
        If (TypeOf ds.Connection Is cEwEDatabase) Then
            Dim db As cEwEDatabase = DirectCast(ds.Connection, cEwEDatabase)
            Dim dbUpd As New cDatabaseUpdater(Me, 6.0!)

            If dbUpd.HasUpdates(db) Then
                ' Create a copy of the database for select types
                Dim src As String = db.Name
                Try
                    If File.Exists(src) Then

                        ' User wants to make a backup?
                        Dim fmsg As New cFeedbackMessage(My.Resources.CoreMessages.DATABASE_BACKUP_PROMPT, _
                                                         eCoreComponentType.DataSource, eMessageType.Any, _
                                                         eMessageImportance.Information, _
                                                         cFeedbackMessage.eReplyStyle.YES_NO_CANCEL)
                        ' By default (if message is not handled) assume positive confirmation
                        fmsg.Reply = cFeedbackMessage.eReply.OK
                        ' Send message
                        Me.m_publisher.SendMessage(fmsg)
                        ' Cancel if requested
                        If (fmsg.Reply = cFeedbackMessage.eReply.CANCEL) Then Return False

                        If (fmsg.Reply = cFeedbackMessage.eReply.OK) Then

                            Dim strSrc As String = CStr(src)
                            Dim strDest As String = ""
                            Dim msg As cMessage = Nothing

                            ' Create backup
                            If cFileUtils.CreateBackup(strSrc, strDest) Then
                                msg = New cMessage(String.Format(My.Resources.CoreMessages.DATABASE_BACKUP_SUCCESS, strDest), _
                                                        eMessageType.DataImport, _
                                                        eCoreComponentType.DataSource, _
                                                        eMessageImportance.Information)
                            Else
                                msg = New cMessage(String.Format(My.Resources.CoreMessages.DATABASE_BACKUP_FAILED, strDest), _
                                                        eMessageType.DataImport, _
                                                        eCoreComponentType.DataSource, _
                                                        eMessageImportance.Information)
                            End If
                            ' Send backup result message
                            Me.m_publisher.SendMessage(msg)
                        End If
                    End If

                Catch ex As Exception
                    ' Whoah
                End Try

                ' Run updates
                If Not dbUpd.UpdateDatabase(db) Then
                    ' Database update failed
                    Dim msg As cMessage = Nothing
                    msg = New cMessage(My.Resources.CoreMessages.ECOPATH_MODEL_UPDATE_FAILED, _
                                            eMessageType.DataImport, _
                                            eCoreComponentType.DataSource, _
                                            eMessageImportance.Critical)

                    Me.m_publisher.SendMessage(msg)
                    Return False
                Else
                    ' Database update failed
                    Dim msg As cMessage = Nothing
                    msg = New cMessage(My.Resources.CoreMessages.ECOPATH_MODEL_UPDATE_SUCCESS, _
                                            eMessageType.DataImport, _
                                            eCoreComponentType.DataSource, _
                                            eMessageImportance.Information)
                    Me.m_publisher.SendMessage(msg)
                End If
            End If

        End If
        Return True

    End Function

#If 0 Then

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns if the compact database engine is available.
    ''' </summary>
    ''' <returns>True if can compact the current data source.</returns>
    ''' -------------------------------------------------------------------
    Public Function CanCompact() As Boolean

        If Me.DataSource Is Nothing Then Return False
        If Me.PluginManager Is Nothing Then Return False

        Return Me.PluginManager.CanCompactDatabase(cDataSourceFactory.GetSupportedType(Me.DataSource.ToString()))

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Compact the current M$ Access database.
    ''' </summary>
    ''' <param name="strFileTo">Target database to compact to. Can be left blank.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function Compact(ByVal strFileTo As String) As eDatasourceAccessType

        Dim strFileFrom As String = Me.DataSource.ToString()
        Dim dst As eDataSourceTypes = cDataSourceFactory.GetSupportedType(strFileFrom)

        ' Fix params
        If String.IsNullOrEmpty(strFileTo) Then strFileTo = strFileFrom

        Dim bCompactToOriginal As Boolean = (String.Compare(strFileFrom, strFileTo, True) = 0)
        Dim strConnectionFrom As String = ""
        Dim strConnectionTo As String = ""
        Dim strConnection As String = ""
        Dim result As eDatasourceAccessType = eDatasourceAccessType.Success
        'Dim comp As IDatabaseCompact = cDatabaseCompactFactory.GetDatabaseCompact(strFileFrom)

        Select Case cDataSourceFactory.GetSupportedType(strFileFrom)
            Case eDataSourceTypes.MDB
                strConnection = Me.m_strConnectionMDB
            Case eDataSourceTypes.ACCDB
                ' Accdb needs different compaction engine, no idea how to do that for now
                strConnection = Me.m_strConnectionACCDB
            Case Else
                ' Not supported
                strConnection = ""
        End Select

        ' No connection string for compacting this type of database?
        If String.IsNullOrEmpty(strConnection) Then Return eDatasourceAccessType.Failed_OSUnsupported
        ' Cannot compact when connected
        Debug.Assert(Me.DataSource.IsConnected = False)

        Try

            ' #Yes: try to compact
            Dim strDBToOrg As String = strFileTo
            ' Identical database specified for in and out?
            If (bCompactToOriginal) Then
                ' #Yes: compact DB to temp location
                strFileTo = System.IO.Path.GetTempFileName()
            End If

            If File.Exists(strFileTo) Then
                Try
                    File.Delete(strFileTo)
                Catch ex As Exception
                    Return eDatasourceAccessType.Failed_CannotSave
                End Try
            End If

            ' Set up connections
            strConnectionFrom = String.Format(strConnection, strFileFrom)
            strConnectionTo = String.Format(strConnection, strFileTo)

            If Me.PluginManager.CompactDatabase(dst, strFileFrom, "", strFileTo, "", result) Then

                ' Is succesfully compacted?
                If File.Exists(strFileTo) Then
                    ' #Yes: Need to copy from temp location?
                    If (bCompactToOriginal) Then
                        ' #Yes: Overwrite original db with compacted db
                        File.Copy(strFileTo, strDBToOrg, True)
                        ' Delete temp location compacted database
                        File.Delete(strFileTo)
                    End If
                End If

            End If

        Catch ex As Exception
            ' Wow
        End Try

        Return result

    End Function
#End If

#End Region ' Datasource

#Region " EwEModel "

    ''' <summary>
    ''' Returns the <see cref="cEwEModel">EwE model</see> for the current loaded datasource.
    ''' </summary>
    Public ReadOnly Property EwEModel() As cEwEModel
        Get
            Return Me.m_EwEModel
        End Get
    End Property

    Friend m_EwEModelDBID As Integer = 0
    Friend m_EwEModelName As String = ""
    Friend m_EwEModelDescription As String = ""
    Friend m_EwEModelArea As Single = 0
    Friend m_EwEModelNumDigits As Integer = 0
    Friend m_EwEModelGroupDigits As Boolean = False
    Friend m_EwEModelUnitTime As eUnitTimeType = 0
    Friend m_EwEModelUnitTimeCustom As String = ""
    Friend m_EwEModelUnitCurrency As eUnitCurrencyType = eUnitCurrencyType.NotSet
    Friend m_EwEModelUnitCurrencyCustom As String = ""
    Friend m_EwEModelUnitMonetary As eUnitMonetaryType = 0
    Friend m_EwEModelUnitMonetaryCustom As String = ""
    Friend m_EwEModelAuthor As String = ""
    Friend m_EwEModelContact As String = ""
    Friend m_EwEModelLastSaved As Double = 0
    Friend m_EwEModelAreaName As String = ""
    Friend m_EwEModelSouth As Single = 0
    Friend m_EwEModelNorth As Single = 0
    Friend m_EwEModelWest As Single = 0
    Friend m_EwEModelEast As Single = 0
    Friend m_EwEModelFirstYear As Integer = Date.Now.Year
    Friend m_EwEModelNumYears As Integer = 1

    Private Function InitEwEModel() As Boolean
        Me.m_EwEModel = New cEwEModel(Me)
        Return LoadEwEModel()
    End Function

    Friend Function LoadEwEModel() As Boolean
        'Pre
        Debug.Assert(Me.m_EwEModel IsNot Nothing)
        Me.m_EwEModel.AllowValidation = False
        Me.m_EwEModel.DBID = Me.m_EwEModelDBID
        Me.m_EwEModel.Name = Me.m_EwEModelName
        Me.m_EwEModel.Description = Me.m_EwEModelDescription
        Me.m_EwEModel.Area = Me.m_EwEModelArea
        Me.m_EwEModel.Author = Me.m_EwEModelAuthor
        Me.m_EwEModel.Contact = Me.m_EwEModelContact
        Me.m_EwEModel.NumDigits = Me.m_EwEModelNumDigits
        Me.m_EwEModel.GroupDigits = Me.m_EwEModelGroupDigits
        Me.m_EwEModel.UnitCurrency = Me.m_EwEModelUnitCurrency
        Me.m_EwEModel.UnitCurrencyCustomText = Me.m_EwEModelUnitCurrencyCustom
        Me.m_EwEModel.UnitTime = Me.m_EwEModelUnitTime
        Me.m_EwEModel.UnitTimeCustomText = Me.m_EwEModelUnitTimeCustom
        Me.m_EwEModel.UnitMonetary = Me.m_EwEModelUnitMonetary
        Me.m_EwEModel.UnitMonetaryCustomText = Me.m_EwEModelUnitMonetaryCustom
        Me.m_EwEModel.FirstYear = Me.m_EwEModelFirstYear
        Me.m_EwEModel.NumYears = Me.m_EwEModelNumYears
        Me.m_EwEModel.AreaName = Me.m_EwEModelAreaName
        Me.m_EwEModel.South = Me.m_EwEModelSouth
        Me.m_EwEModel.North = Me.m_EwEModelNorth
        Me.m_EwEModel.West = Me.m_EwEModelWest
        Me.m_EwEModel.East = Me.m_EwEModelEast
        Me.m_EwEModel.LastSaved = Me.m_EwEModelLastSaved
        Me.m_EwEModel.AllowValidation = True

        ' Update relevant unit(s) in Ecopath
        Me.m_EcoPathData.currUnitIndex = Me.m_EwEModelUnitCurrency

        Me.m_EwEModel.ResetStatusFlags()
        Return True
    End Function

    Friend Function UpdateEwEModel() As Boolean
        Me.m_EwEModelName = Me.m_EwEModel.Name
        Me.m_EwEModelDescription = Me.m_EwEModel.Description
        Me.m_EwEModelAuthor = Me.m_EwEModel.Author
        Me.m_EwEModelContact = Me.m_EwEModel.Contact
        Me.m_EwEModelArea = Me.m_EwEModel.Area
        Me.m_EwEModelNumDigits = Me.m_EwEModel.NumDigits
        Me.m_EwEModelGroupDigits = Me.m_EwEModel.GroupDigits
        Me.m_EwEModelUnitCurrency = Me.m_EwEModel.UnitCurrency
        Me.m_EwEModelUnitCurrencyCustom = Me.m_EwEModel.UnitCurrencyCustomText
        Me.m_EwEModelUnitTime = Me.m_EwEModel.UnitTime
        Me.m_EwEModelUnitTimeCustom = Me.m_EwEModel.UnitTimeCustomText
        Me.m_EwEModelUnitMonetary = Me.m_EwEModel.UnitMonetary
        Me.m_EwEModelUnitMonetaryCustom = Me.m_EwEModel.UnitMonetaryCustomText
        Me.m_EwEModelFirstYear = Me.m_EwEModel.FirstYear
        Me.m_EwEModelNumYears = Me.m_EwEModel.NumYears
        Me.m_EwEModelAreaName = Me.m_EwEModel.AreaName
        Me.m_EwEModelSouth = Me.m_EwEModel.South
        Me.m_EwEModelNorth = Me.m_EwEModel.North
        Me.m_EwEModelWest = Me.m_EwEModel.West
        Me.m_EwEModelEast = Me.m_EwEModel.East
        ' Do not update LastSaved; exclusively set by core

        ' Update relevant unit(s) in Ecopath
        Me.m_EcoPathData.currUnitIndex = Me.m_EwEModelUnitCurrency

        Return True
    End Function

#End Region 'EwEModel

#Region " EcoPath "

#Region " Variables "

    'Private EcoPath Model Variables
    Friend m_EcoPath As Ecopath.cEcoPathModel ' the EcoPath model
    Friend m_EcoPathData As cEcopathDataStructures = Nothing 'Parameters read for datasource for EcoPath

    Friend m_EcoPathInputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoPathGroupInput, 1)
    Friend m_EcoPathOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoPathGroupOutput, 1)
    Friend m_EcopathFleetsInput As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.FleetInput, 1)
    Friend m_EcopathTaxon As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.Taxon, 1)

    Private m_postEcoPathMessage As CoreMessageDelegate
    Friend m_PSDData As cPSDDatastructures
    Private m_PSDParameters As cPSDParameters
    Private m_psdModel As cPSDModel

#End Region ' Variables

#Region " Model "

    Private Sub SendEcopathLoadMessage(ByVal ds As IEwEDataSource, Optional ByVal strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = String.Format(My.Resources.CoreMessages.ECOPATH_LOAD_SUCCESS, ds.ToString())
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoPath, eMessageImportance.Information)
        Else
            strText = String.Format(My.Resources.CoreMessages.ECOPATH_LOAD_FAILED, ds.ToString(), strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.EcoPath, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the Ecopath model from a given Datasource.
    ''' </summary>
    ''' <param name="ds">A <see cref="IEwEDataSource">IEwEDataSource</see>-derived
    ''' object that provides access to the </param>
    ''' <returns>True if the model was loaded successfully. False otherwise</returns>
    ''' <remarks>The given datasource will be remembered here for subsequent 
    ''' <see cref="SaveModel">SaveModel</see> and SaveEcosimScenario calls.</remarks>
    ''' -----------------------------------------------------------------------
    Public Function LoadModel(ByVal ds As IEwEDataSource) As Boolean

        Dim dsEcopath As IEcopathDataSource = Nothing
        Dim bsuccess As Boolean

        ' Sanity checks
        Debug.Assert(ds IsNot Nothing, Me.ToString & "LoadModel() Datasource can not be NULL.")
        Debug.Assert(TypeOf ds Is IEcopathDataSource, "Invalid datasource type specified")

        ' Update core state
        If Not Me.SaveChanges() Then Return False

        Me.m_EcoPathData.ActiveEcosimScenario = -1
        Me.m_EcoPathData.ActiveEcospaceScenario = -1
        Me.m_EcoPathData.ActiveEcotracerScenario = -1

        'm_bCoreIsInit was set in InitCore()
        If Not m_bCoreIsInit Then
            'core has not been initialized this can not be run
            Debug.Assert(False, "The Core has not been initialized. Call InitCore() first.")
            ' Flag data as gone
            Me.m_StateMonitor.SetEcopathLoaded(False)
            SendEcopathLoadMessage(ds, "Core not initialized")
            Return False
        End If

        ' Run any available updates on the datasource
        If Not Me.UpdateDatasource(ds) Then
            Return False
        End If

        ' Remember the new datasource
        DataSource = ds

        Try

            ' Clear auxillary data
            Me.m_dtAuxiliaryData.Clear()
            ' Clear time series datasets
            Me.m_TSData.ClearTimeSeriesDatasets()

            'Init the parameters from the datasource
            dsEcopath = DirectCast(DataSource, IEcopathDataSource)
            If dsEcopath.LoadModel() Then

                'build model
                bsuccess = InitEwEModel()

                cLog.InitLog(Me.m_EwEModel.Name)

                'I'm not sure about this there 
                'there needs to be a Maintenance message sent SendEcopathLoadMessage() does not really seem like it would work for this
                ' VERIFY_JS: Discuss what to do here
                m_publisher.AddMessage(New cMessage("Loaded model '" & m_EwEModel.Name & "'", eMessageType.DataModified, _
                                        eCoreComponentType.Core, eMessageImportance.Maintenance))

                'copy the input data into the output data this could wait for a model run but it may be safer to do it here
                m_EcoPathData.CopyInputToModelArrays()
                m_PSDData.Enabled = False ' Fixes bug 683
                m_PSDData.CopyInputToModelArrays()

                'compute the stanza data from the parameters loaded from the model 
                'this has to come before initializing and loading the ecopath groups because 
                'InitStanza can modify the ecopath value: b, pb and qb
                m_EcoSim.InitStanza()

                ' ToDo_JS: wrap this more neatly one day
                Me.m_tracerData.RedimByNGroups(Me.nGroups)

                'build input and output objects
                bsuccess = bsuccess And InitEcoPathGroups()
                bsuccess = bsuccess And InitTaxa()

                'build the Stanza Groups for the interface
                bsuccess = bsuccess And InitStanzas()

                'populate input objects
                bsuccess = bsuccess And LoadEcopathInputs()
                'populate output objects
                bsuccess = bsuccess And LoadEcopathOutputs()

                'build the fleets
                bsuccess = bsuccess And InitFleets()

                ' Initialize scenarios
                bsuccess = bsuccess And InitEcosimScenarios()
                bsuccess = bsuccess And InitEcospaceScenarios()
                bsuccess = bsuccess And InitEcotracerScenarios()
                bsuccess = bsuccess And InitAndLoadEcosimTimeSeriesDatasets()

                bsuccess = bsuccess And InitPedigreeManagers()
                bsuccess = bsuccess And InitPSDParameters()

                Me.m_EcopathStats = New cEcoPathStats(Me, cCore.NULL_VALUE)
                Me.InitSearchManagers()

                Me.m_gameManager.Init()

                Me.initEcoFunctions()

                If Not bsuccess Then
                    ' Flag data as gone
                    Me.m_StateMonitor.SetEcopathLoaded(False)
                    'this assumes that if there was a problem above then a message will have been posted already?????
                    ' Let go
                    DataSource = Nothing
                    m_publisher.sendAllMessages()
                    Return False
                End If

            Else
                ' Flag data as gone
                Me.m_StateMonitor.SetEcopathLoaded(False)
                ' Let go
                DataSource = Nothing
                Return False
            End If

        Catch ex As Exception
            ' Flag data as gone
            Me.m_StateMonitor.SetEcopathLoaded(False)
            ' Major Error
            Me.SendEcopathLoadMessage(ds, ex.Message)
            ' Release datasource
            DataSource = Nothing
            ' Report error
            Return False
        End Try

        Me.SendEcopathLoadMessage(ds)

        ' Invoke plugin point
        If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.LoadModel(DataSource)

        ' Update core state
        Me.m_StateMonitor.SetEcopathLoaded(True)

        'Core initialized plugin point
        If (Me.PluginManager IsNot Nothing) Then Me.m_pluginManager.CoreInitialized(m_EcoPath, m_EcoSim, m_Ecospace)

        m_publisher.sendAllMessages()

        Return True

    End Function

    Public Function Save(Optional ByVal strFileName As String = "") As Boolean

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcopathDataSource Then Return False

        Dim bSucces As Boolean = True

        ' Saving to a new file name?
        If (Not String.IsNullOrEmpty(strFileName)) Then
            ' #Yes: First save current database to a new location
            If (DirectCast(DataSource, cDBDataSource).SaveAs(strFileName, Me.m_EwEModel.Name)) = eDatasourceAccessType.Success Then
                ' #Succes! The datasource has been changed this new location, now save data in memory to the new datasource.
                bSucces = Me.SaveChanges(True)
            End If
        Else
            bSucces = Me.SaveChanges(True)
        End If

        ' Force an update since datasources have been switched
        Me.m_StateMonitor.UpdateDataState(DataSource, TriState.True)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the Ecopath model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>Note that this logic will NOT sync the two datasources; this
    ''' responsibility is left to the calling process. Yes, this is a hack 
    ''' around a process that needs to be very well thought out!!!</remarks>
    ''' -----------------------------------------------------------------------
    Private Function SaveModel() As Boolean

        Me.m_EwEModelLastSaved = CInt(Date.Now().ToOADate())

        If (DirectCast(Me.DataSource, IEcopathDataSource).SaveModel()) Then
            ' #Yes: invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveModel(Me)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Oh we're happy now!
            Return True
        Else
            Me.m_publisher.SendMessage(New cMessage(String.Format(My.Resources.CoreMessages.ECOPATH_SAVE_FAILED, DataSource.ToString), eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Warning))
            cLog.Write("cCore.SaveModel() Failed to save the current model") 'the current model name will be in the log file
            Return False
        End If

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Try to terminate the core
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function CloseModel() As Boolean

        If Not Me.SaveChanges() Then Return False

        ' Has datasource?
        If (DataSource IsNot Nothing) Then
            ' #Yes: has open connection?
            If DataSource.Connection IsNot Nothing Then
                ' #Yes: Close connection
                DataSource.Close()
                ' Close plug-in data sources, close plug-in 
                If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.CloseDatabase() : Me.m_pluginManager.CloseModel()
            End If
            ' Release datasource
            DataSource = Nothing
        End If

        ' Forget model
        Me.m_StateMonitor.SetEcopathLoaded(False)
        Me.m_StateMonitor.UpdateDataState(Nothing)

        ' Clear core data
        Me.m_EcoPathData.Clear()
        Me.m_Stanza.Clear()
        Me.m_EcoSimData.Clear()
        Me.m_EcoSpaceData.Clear()
        Me.m_tracerData.Clear()

        ' JS 5may10: Do not discards IO objects; this may have effects throughout the UI
        '' Empty IO objects
        'Me.m_EcoPathInputs.Clear()
        'Me.m_EcoPathOutputs.Clear()
        'Me.m_EcopathFleetsInput.Clear()
        'Me.m_EcosimFleetInputs.Clear()
        'Me.m_EcosimFleetOutputs.Clear()
        'Me.m_EcoSimGroupOutputs.Clear()
        'Me.m_EcoSimScenarios.Clear()
        'Me.m_EcospaceFleetOutputs.Clear()
        'Me.m_EcoSpaceFleets.Clear()
        'Me.m_EcospaceGroupOuputs.Clear()
        'Me.m_EcoSpaceGroups.Clear()
        'Me.m_EcospaceHabitats.Clear()
        'Me.m_EcospaceMPAs.Clear()
        'Me.m_EcospaceRegions.Clear()
        'Me.m_EcoSpaceScenarios.Clear()
        'Me.m_EcotracerGroupInputs.Clear()
        'Me.m_EcotracerScenarios.Clear()
        'Me.m_stanzaGroups.Clear()

        Return True

    End Function

#End Region ' Model

#Region " Groups "

    ''' <summary>
    ''' Basic Inputs for EcoPath for a single group
    ''' </summary>
    ''' <param name="iGroup">
    ''' Number of the group the data is for
    ''' This collection of GroupInputs is one base </param>
    ''' <value>Returns a Valid group if a Group exists for this iGroup. Returns nothing if iGroup is out of bounds</value>
    ''' <remarks>
    ''' The cEcoPathGroup object returned is a reference to a cEcoPathGroup held by the Core.
    ''' Any changes made to the returned object will also be made to the object held by the Core/EcoPath model. 
    ''' This property is ReadOnly because it returns a reference that allows direct manipulation of the underlying data
    ''' and updating is not needed.
    ''' How to use:
    ''' 'this will update the Biomass for all groups to two
    ''' dim prvtGetSetEcoPathInputs as cEcoPathGroup
    ''' For i = 1 to Core.NumberGroups
    '''      prvtGetSetEcoPathInputs = Core.EcoPathGroupInputs(i)
    '''      prvtGetSetEcoPathInputs.Biomass = 2
    ''' next i 
    ''' </remarks>
    Public ReadOnly Property EcoPathGroupInputs(ByVal iGroup As Integer) As cEcoPathGroupInput
        Get
            ' JS 06Jul07: list takes care of group index / item index offset
            Return DirectCast(m_EcoPathInputs(iGroup), cEcoPathGroupInput)
        End Get

    End Property

    Private Function InitEcoPathGroups() As Boolean
        Dim bsuccess As Boolean = True

        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.m_EcoPathInputs.AllowEvents = False
        'Me.m_EcoPathOutputs.AllowEvents = False

        Try

            m_EcoPathInputs.Clear()
            m_EcoPathOutputs.Clear()

            'populate the list of Inputs and Outputs (cEcoPathGroupInputs and cEcoPathGroupOutput)
            For i As Integer = 1 To nGroups
                'creates an instance of both the input and output objects and adds it to the list
                'the Input and Output objects have only been created they are not Loaded with the Ecopath data at this time
                m_EcoPathInputs.Add(New cEcoPathGroupInput(Me, m_EcoPathData.GroupDBID(i)))
                m_EcoPathOutputs.Add(New cEcoPathGroupOutput(Me, m_EcoPathData.GroupDBID(i)))
            Next

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.m_EcoPathInputs.AllowEvents = True
        'Me.m_EcoPathOutputs.AllowEvents = True

        Return bsuccess

    End Function

    ''' <summary>
    ''' Load the Ecopath data into all the existing cEcoPathGroupInputs objects in the core
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function LoadEcopathInputs() As Boolean
        Try

            For Each Input As cEcoPathGroupInput In m_EcoPathInputs
                Me.LoadEcopathInput(Input)
            Next

            Return True
        Catch ex As Exception
            'ToDo_jb LoadEcopathInputs some kind of error handling
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadEcopathInput(ByVal Input As cEcoPathGroupInput) As Boolean
        Dim iGroup As Integer
        Try

            'do not run the data validation when the object is populated
            Input.AllowValidation = False

            'convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcoPathData.GroupDBID, Input.DBID)

            If iGroup >= 0 And iGroup <= m_EcoPathData.NumGroups Then

                Input.Resize()

                Input.Index = iGroup
                'get the public variables
                'jb June-7-2006 DatabaseID is now set in the constructor so that an object always knows it DatabaseID
                'Input.DBID = m_EcoPathData.GroupDBID(iGroup)
                Input.Name = m_EcoPathData.GroupName(iGroup)

                'input variables
                Input.EEInput = CSng(m_EcoPathData.EEinput(iGroup))
                Input.QBInput = CSng(m_EcoPathData.QBinput(iGroup))
                Input.PBInput = CSng(m_EcoPathData.PBinput(iGroup))
                Input.GEInput = CSng(m_EcoPathData.GEinput(iGroup))
                Input.BiomassAreaInput = CSng(m_EcoPathData.BHinput(iGroup))

                Input.Area = m_EcoPathData.Area(iGroup)
                Input.GS = m_EcoPathData.GS(iGroup)
                Input.DetImport = m_EcoPathData.DtImp(iGroup)
                Input.EmigRate = m_EcoPathData.Emig(iGroup)
                Input.BioAccumRate = CSng(m_EcoPathData.BaBi(iGroup))
                Input.Immigration = m_EcoPathData.Immig(iGroup)
                Input.PP = m_EcoPathData.PP(iGroup)
                Input.VBK = m_EcoPathData.vbK(iGroup)
                Input.PoolColor = m_EcoPathData.GroupColor(iGroup)
                Input.NonMarketValue = m_EcoPathData.Shadow(iGroup)

                Input.BioAccum = CSng((IIf(m_EcoPathData.BaBi(iGroup) <> 0 And m_EcoPathData.B(iGroup) > 0, m_EcoPathData.BaBi(iGroup) * m_EcoPathData.B(iGroup), m_EcoPathData.BA(iGroup))))

                'if  Emigration = 0 then compute Emigration as EmigRate * biomass for this group
                'from original code
                Input.Emigration = CSng(IIf(m_EcoPathData.Emig(iGroup) > 0 And m_EcoPathData.B(iGroup) > 0 And m_EcoPathData.Emigration(iGroup) = 0, _
                                                m_EcoPathData.Emig(iGroup) * m_EcoPathData.B(iGroup), m_EcoPathData.Emigration(iGroup)))
                Dim j As Integer
                'Diet Comp (DO NOT INCLUDE IMPORT IN THE DC ARRAY - THIS IS SEPARATED IN ECOPATHGROUP!)
                For j = 1 To m_EcoPathData.NumGroups
                    Input.DietComp(j) = m_EcoPathData.DCInput(iGroup, j)
                Next
                Input.ImpDiet = m_EcoPathData.DCInput(iGroup, 0)

                'detritus fate
                For j = 1 To nDetritusGroups
                    Input.DetritusFate(j) = m_EcoPathData.DF(iGroup, j)
                Next

                'stanza variables setting the stanza id will also set the isMultiStanza Flag
                Input.iStanza = getStanzaIndexForGroup(iGroup)

                ' === PSD ===
                Input.AinLWInput = m_PSDData.AinLWInput(iGroup)
                Input.BinLWInput = m_PSDData.BinLWInput(iGroup)
                Input.LooInput = m_PSDData.LooInput(iGroup)
                Input.WinfInput = m_PSDData.WinfInput(iGroup)
                Input.t0Input = m_PSDData.t0Input(iGroup)
                Input.TcatchInput = m_PSDData.TcatchInput(iGroup)
                Input.TmaxInput = m_PSDData.TmaxInput(iGroup)
                ' === END PSD ===

                'set all the status flags to default value
                Input.ResetStatusFlags()

                Input.AllowValidation = True
            Else
                Debug.Assert(False)
            End If

            Return True
        Catch ex As Exception
            'ToDo_jb LoadEcopathInputs some kind of error handling
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Update the underlying Ecopath Data with the values in EcoPath inputs list
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateEcopathInput(ByVal iDBID As Integer) As Boolean

        Dim iGroup As Integer = Array.IndexOf(m_EcoPathData.GroupDBID, iDBID)
        'jb List of inputs is indexed from zero iGroup is the array index which is indexed from one 
        'so subtract one from the array index to get the correct index in the list
        Dim Input As cEcoPathGroupInput = Me.EcoPathGroupInputs(iGroup)
        Dim bSucces As Boolean = True

        Try

            If iGroup >= 1 And iGroup <= m_EcoPathData.NumGroups Then

                m_EcoPathData.GroupName(iGroup) = Input.Name
                m_EcoPathData.Area(iGroup) = Input.Area
                m_EcoPathData.GS(iGroup) = Input.GS
                m_EcoPathData.DtImp(iGroup) = Input.DetImport
                'jb 17/mar/06 removed biomass from input
                'mEcoPathData.B(iGroup) = input.Biomass
                m_EcoPathData.BaBi(iGroup) = Input.BioAccumRate
                m_EcoPathData.Immig(iGroup) = Input.Immigration
                m_EcoPathData.BA(iGroup) = Input.BioAccum
                m_EcoPathData.Emig(iGroup) = Input.EmigRate
                m_EcoPathData.PP(iGroup) = Input.PP

                m_EcoPathData.vbK(iGroup) = Input.VBK
                m_PSDData.AinLWInput(iGroup) = Input.AinLWInput
                m_PSDData.BinLWInput(iGroup) = Input.BinLWInput
                m_PSDData.LooInput(iGroup) = Input.LooInput
                m_PSDData.WinfInput(iGroup) = Input.WinfInput
                m_PSDData.t0Input(iGroup) = Input.t0Input
                m_PSDData.TcatchInput(iGroup) = Input.TcatchInput
                m_PSDData.TmaxInput(iGroup) = Input.TmaxInput

                m_EcoPathData.QBinput(iGroup) = Input.QBInput
                m_EcoPathData.PBinput(iGroup) = Input.PBInput
                m_EcoPathData.EEinput(iGroup) = Input.EEInput
                m_EcoPathData.GEinput(iGroup) = Input.GEInput
                m_EcoPathData.BHinput(iGroup) = Input.BiomassAreaInput

                m_EcoPathData.GroupColor(iGroup) = Input.PoolColor
                m_EcoPathData.Shadow(iGroup) = Input.NonMarketValue()

                'from the original code MakeUnknownUnknown
                m_EcoPathData.BA(iGroup) = CSng(IIf(m_EcoPathData.BaBi(iGroup) <> 0 And m_EcoPathData.B(iGroup) > 0, _
                                                m_EcoPathData.BaBi(iGroup) * m_EcoPathData.B(iGroup), m_EcoPathData.BA(iGroup)))

                'Emigi(igroup) = inputVars.EmigRate
                'if  Emigration = 0 then compute Emigration as EmigRate * biomass for this group
                'from original code
                m_EcoPathData.Emigration(iGroup) = CSng(IIf(m_EcoPathData.Emig(iGroup) > 0 And m_EcoPathData.B(iGroup) > 0 And m_EcoPathData.Emigration(iGroup) = 0, _
                                                         m_EcoPathData.Emig(iGroup) * m_EcoPathData.B(iGroup), Input.Emigration))
                For i As Integer = 1 To m_EcoPathData.NumGroups
                    'Diet Comp is stored by Pred/Prey
                    'so this is the Prey for Predator iGroup
                    m_EcoPathData.DCInput(iGroup, i) = Input.DietComp(i)
                Next i
                m_EcoPathData.DCInput(iGroup, 0) = Input.ImpDiet()

                For i As Integer = 1 To nDetritusGroups
                    m_EcoPathData.DF(iGroup, i) = Input.DetritusFate(i)
                Next i

            Else
                Debug.Assert(False)
                bSucces = False
            End If

        Catch ex As Exception
            'ToDo_jb UpdateEcopathInputs() Error do something
            Debug.Assert(False)
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' <summary>
    ''' Retrieves the EcoPath estimated parameters for the last run parameter estimation for this iGroup
    ''' by creating a new EcoPathGroupOutputs object that is populated with the estimated parameters.
    ''' </summary>
    ''' <param name="iGroup">Group that the model results are for</param>
    ''' <returns>A valid cEcoPathGroupOutput object if successfull. Nothing(NULL) otherwise</returns>
    ''' <remarks>
    ''' This data is the estimated parameters. 
    ''' i.e.
    ''' Model.InitEcoPath("SomeDatasource")
    ''' Model.RunEcopath()
    ''' Model.EcoPathGroupOutputs(1)'will get the output (estimated parameters)of the EcoPath model for group 1
    ''' </remarks>
    Public ReadOnly Property EcoPathGroupOutputs(ByVal iGroup As Integer) As cEcoPathGroupOutput

        Get
            ' The list takes care of group index / item index differences
            Return DirectCast(m_EcoPathOutputs(iGroup), cEcoPathGroupOutput)
        End Get

    End Property

    ''' <summary>
    ''' Clear all status flags on Ecopath group outputs
    ''' </summary>
    Private Sub ResetEcopathGroupOutputs()
        For Each group As cEcoPathGroupOutput In Me.m_EcoPathOutputs
            group.ResetStatusFlags(True)
        Next
    End Sub

    Private Function LoadEcopathOutputs() As Boolean

        Dim predmort() As Single
        Dim searchrate() As Single
        Dim consump() As Single
        Dim impConsump As Single
        Dim Hlap() As Single
        Dim Plap() As Single
        Dim Alpha() As Single
        Dim EcopathWeight() As Single
        Dim EcopathNumber() As Single
        Dim EcopathBiomass() As Single
        Dim LorenzenMortality() As Single
        Dim PSD() As Single

        Dim convalue As Single
        Dim iGroup As Integer

        Try

            For Each output As cEcoPathGroupOutput In m_EcoPathOutputs
                'convert the DBID into an iGroup
                iGroup = Array.IndexOf(m_EcoPathData.GroupDBID, output.DBID)

                'set the size of any array data
                output.Resize()

                'iGroup out of bounds
                If (iGroup > nGroups Or iGroup < 0) And iGroup <> NULL_VALUE Then
                    cLog.Write(Me.ToString & ".PopulateEcoPathOutput() iGroup out of bounds.")
                    'ToDo LoadEcopathOutputs() failed to find iGroup do something better than exiting
                    Return False
                End If

                'set output readonly to false so the values can be set
                output.m_bReadOnly = False

                output.Index = iGroup
                ReDim predmort(nGroups)
                ReDim consump(nGroups)
                ReDim searchrate(nGroups)
                ReDim Hlap(nGroups)
                ReDim Plap(nGroups)
                ReDim Alpha(nGroups)

                For iPred As Integer = 1 To m_EcoPathData.NumLiving
                    If m_EcoPathData.B(iGroup) > 0 Then
                        'predation mortality is not held by EcoPath; it is computed every time it's needed
                        predmort(iPred) = CSng(m_EcoPathData.B(iPred) * m_EcoPathData.QB(iPred) * m_EcoPathData.DC(iPred, iGroup) / m_EcoPathData.B(iGroup))
                        'search rate is not held by EcoPath; it is computed every time it's needed
                        searchrate(iPred) = CSng(m_EcoPathData.B(iPred) * m_EcoPathData.QB(iPred) * m_EcoPathData.DC(iPred, iGroup) / (m_EcoPathData.B(iGroup) * m_EcoPathData.B(iPred)))
                    End If
                Next
                output.PredMort = predmort
                output.SearchRate = searchrate

                output.Index = iGroup
                output.DBID = m_EcoPathData.GroupDBID(iGroup)
                output.Name = m_EcoPathData.GroupName(iGroup)
                output.Area = m_EcoPathData.Area(iGroup)
                output.BioAccum = CSng(m_EcoPathData.BA(iGroup))
                output.Biomass = CSng(m_EcoPathData.B(iGroup))
                output.BiomassArea = CSng(m_EcoPathData.BH(iGroup))
                Try
                    ' ToDo_JS: Test for Null? Core_null?
                    output.BioAccumRatePerYear = CSng(m_EcoPathData.BA(iGroup) / m_EcoPathData.B(iGroup))
                Catch ex As Exception
                    output.BioAccumRatePerYear = 0.0!
                End Try
                output.GS = m_EcoPathData.GS(iGroup)
                output.TTLX = m_EcoPathData.TTLX(iGroup)

                output.PP = m_EcoPathData.PP(iGroup)

                'output variables
                output.PBOutput = CSng(m_EcoPathData.PB(iGroup))
                output.QBOutput = CSng(m_EcoPathData.QB(iGroup))
                output.EEOutput = CSng(m_EcoPathData.EE(iGroup))
                output.GEOutput = CSng(m_EcoPathData.GE(iGroup))


                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'mortality coefficients are computed when they are needed
                'see Ewe-5 code frmPasicParams.DisplayMortalityCoefficients() for original code
                output.MortCoBioAcumRate = CSng(m_EcoPathData.BA(iGroup) / m_EcoPathData.B(iGroup))
                output.MortCoFishRate = CSng(m_EcoPathData.fCatch(iGroup) / m_EcoPathData.B(iGroup))
                output.MortCoNetMig = CSng((m_EcoPathData.Emigration(iGroup) - m_EcoPathData.Immig(iGroup)) / m_EcoPathData.B(iGroup))
                output.MortCoOtherMort = CSng((1 - m_EcoPathData.EE(iGroup)) * m_EcoPathData.PB(iGroup))
                output.MortCoPB = CSng(m_EcoPathData.PB(iGroup))
                output.MortCoPredMort = m_EcoPathData.M2(iGroup)
                Dim m0 As Single = CSng((1 - m_EcoPathData.EE(iGroup)))
                output.FishMortPerTotMort = output.MortCoFishRate / (m0 + m_EcoPathData.M2(iGroup) + output.MortCoFishRate) 'F/Z
                output.NatMortPerTotMort = CSng(1.0 - output.FishMortPerTotMort) 'M/Z

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'consumption
                'see frmPasicParams.DisplayFoodIntake
                For i As Integer = 1 To m_EcoPathData.NumGroups
                    If i <= m_EcoPathData.NumLiving Then
                        convalue = CSng(m_EcoPathData.B(i) * m_EcoPathData.QB(i) * m_EcoPathData.DC(i, iGroup))
                    Else
                        convalue = CSng(m_EcoPathData.det(iGroup, i))
                    End If

                    If convalue > 0 Then
                        consump(i) = convalue
                    End If
                Next i

                'set the Consumption array in the output
                output.Consumption = consump

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'imported comsumption for this group
                'imported diet compostion is in the zero array element of the DC() array
                impConsump = CSng(m_EcoPathData.B(iGroup) * m_EcoPathData.QB(iGroup) * m_EcoPathData.DC(iGroup, 0))
                If impConsump > 0 Then
                    output.ImportedConsumption = impConsump
                Else
                    output.ImportedConsumption = 0
                End If

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'key indices
                output.NetMigration = CSng(m_EcoPathData.Emigration(iGroup) - m_EcoPathData.Immig(iGroup))
                output.FlowToDet = m_EcoPathData.FlowToDet(iGroup)
                If (iGroup <= Me.m_EcoPathData.NumLiving) Then
                    If (m_EcoPathData.QB(iGroup) * (1 - m_EcoPathData.GS(iGroup)) > 0) Then
                        output.NetEfficiency = m_EcoPathData.PB(iGroup) / (m_EcoPathData.QB(iGroup) * (1 - m_EcoPathData.GS(iGroup)))
                    Else
                        output.NetEfficiency = cCore.NULL_VALUE
                    End If
                End If
                output.OmnivoryIndex = m_EcoPathData.BQB(iGroup)

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'respiration
                output.Respiration = m_EcoPathData.Resp(iGroup)
                output.Assimilation = cCore.NULL_VALUE
                output.RespAssim = cCore.NULL_VALUE
                output.ProdResp = cCore.NULL_VALUE
                output.RespBiom = cCore.NULL_VALUE
                If (iGroup <= Me.m_EcoPathData.NumLiving) Then
                    If m_EcoPathData.QB(iGroup) > 0 Then
                        Dim sAssim As Single = m_EcoPathData.QB(iGroup) * m_EcoPathData.B(iGroup) * (1 - m_EcoPathData.GS(iGroup))
                        output.Assimilation = sAssim
                        output.RespAssim = CSng(m_EcoPathData.Resp(iGroup) / sAssim)
                    End If

                    If (m_EcoPathData.Resp(iGroup) > 0 And m_EcoPathData.B(iGroup) > 0) Then
                        output.ProdResp = m_EcoPathData.PB(iGroup) * m_EcoPathData.B(iGroup) / m_EcoPathData.Resp(iGroup)
                        output.RespBiom = m_EcoPathData.Resp(iGroup) / m_EcoPathData.B(iGroup)
                    End If
                End If

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Niche
                For i As Integer = 1 To m_EcoPathData.NumLiving
                    Hlap(i) = m_EcoPathData.Hlap(i, iGroup)
                    Plap(i) = m_EcoPathData.Plap(i, iGroup)
                Next
                output.Hlap = Hlap
                output.Plap = Plap

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Electivity
                For i As Integer = 1 To m_EcoPathData.NumGroups
                    Alpha(i) = m_EcoPathData.Alpha(iGroup, i)
                Next
                output.Alpha = Alpha

                output.iStanza = getStanzaIndexForGroup(iGroup)

                ' === PSD ===

                ReDim EcopathWeight(nAgeSteps)
                ReDim EcopathNumber(nAgeSteps)
                ReDim EcopathBiomass(nAgeSteps)
                ReDim LorenzenMortality(nAgeSteps)
                ReDim PSD(nWeightClasses)

                output.VBK = CSng(m_EcoPathData.vbK(iGroup))
                output.BiomassAvgSzWt = CSng(m_PSDData.BiomassAvgSzWt(iGroup))
                output.BiomassSzWt = CSng(m_PSDData.BiomassSzWt(iGroup))
                output.AinLWOutput = CSng(m_PSDData.AinLW(iGroup))
                output.BinLWOutput = CSng(m_PSDData.BinLW(iGroup))
                output.LooOutput = CSng(m_PSDData.Loo(iGroup))
                output.WinfOutput = CSng(m_PSDData.Winf(iGroup))
                output.t0Output = CSng(m_PSDData.t0(iGroup))
                output.TcatchOutput = CSng(m_PSDData.Tcatch(iGroup))
                output.TmaxOutput = CSng(m_PSDData.Tmax(iGroup))

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Weight
                For t As Integer = 1 To nAgeSteps
                    EcopathWeight(t) = m_PSDData.EcopathWeight(iGroup, t)
                Next
                output.EcopathWeight = EcopathWeight

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Number
                For t As Integer = 1 To nAgeSteps
                    EcopathNumber(t) = m_PSDData.EcopathNumber(iGroup, t)
                Next
                output.EcopathNumber = EcopathNumber

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Biomass
                For t As Integer = 1 To nAgeSteps
                    EcopathBiomass(t) = m_PSDData.EcopathBiomass(iGroup, t)
                Next
                output.EcopathBiomass = EcopathBiomass

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Lorenzen mortality
                For t As Integer = 1 To nAgeSteps
                    LorenzenMortality(t) = m_PSDData.LorenzenMortality(iGroup, t)
                Next
                output.LorenzenMortality = LorenzenMortality

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' PSD
                For wc As Integer = 1 To nWeightClasses
                    PSD(wc) = m_PSDData.PSD(iGroup, wc)
                Next
                output.PSD = PSD

                ' === END PSD ===

                output.m_bReadOnly = True
                output.ResetStatusFlags()
            Next

            Return True

        Catch ex As Exception

            cLog.Write(ex)
            Return False

        End Try

    End Function

#End Region ' Groups

#Region " Taxon "

    Private Function InitTaxa() As Boolean

        Dim bSucces As Boolean = True
        Try

            Me.m_EcopathTaxon.Clear()
            For iTaxon As Integer = 1 To m_EcoPathData.NumTaxon
                Me.m_EcopathTaxon.Add(New cTaxon(Me, m_EcoPathData.TaxonDBID(iTaxon)))
            Next iTaxon
            Me.LoadTaxa()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitTaxa() Error: " & ex.Message)
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadTaxa() As Boolean

        Dim iTaxon As Integer = 0

        Try

            For Each taxon As cTaxon In Me.m_EcopathTaxon

                taxon.AllowValidation = False

                iTaxon = Array.IndexOf(Me.m_EcoPathData.TaxonDBID, taxon.DBID)

                Debug.Assert(iTaxon > 0 And iTaxon <= m_EcoPathData.NumTaxon, "Failed to find Taxon index for database ID " & taxon.DBID)

                taxon.Resize()

                taxon.Index = iTaxon
                taxon.DBID = Me.m_EcoPathData.TaxonDBID(iTaxon)
                taxon.Group = Me.m_EcoPathData.TaxonGroup(iTaxon)
                taxon.Proportion = Me.m_EcoPathData.TaxonGroupProp(iTaxon)
                taxon.Name = Me.m_EcoPathData.TaxonCommonName(iTaxon)
                taxon.Class = Me.m_EcoPathData.TaxonClass(iTaxon)
                taxon.Order = Me.m_EcoPathData.TaxonOrder(iTaxon)
                taxon.Family = Me.m_EcoPathData.TaxonFamily(iTaxon)
                taxon.Genus = Me.m_EcoPathData.TaxonGenus(iTaxon)
                taxon.Species = Me.m_EcoPathData.TaxonSpecies(iTaxon)
                taxon.Code3A = Me.m_EcoPathData.TaxonCode3A(iTaxon)
                taxon.CodeISSCAAP = Me.m_EcoPathData.TaxonCodeISCAAP(iTaxon)
                taxon.CodeTaxon = Me.m_EcoPathData.TaxonCodeTaxon(iTaxon)
                taxon.Source = Me.m_EcoPathData.TaxonSource(iTaxon)
                taxon.SourceKey = Me.m_EcoPathData.TaxonSourceKey(iTaxon)
                taxon.North = Me.m_EcoPathData.TaxonNorth(iTaxon)
                taxon.South = Me.m_EcoPathData.TaxonSouth(iTaxon)
                taxon.East = Me.m_EcoPathData.TaxonEast(iTaxon)
                taxon.West = Me.m_EcoPathData.TaxonWest(iTaxon)
                taxon.LastUpdated = Me.m_EcoPathData.TaxonLastUpdated(iTaxon)

                taxon.ResetStatusFlags()
                taxon.AllowValidation = True
            Next

            Return True

        Catch ex As Exception

            cLog.Write(Me.ToString() & ".LoadTaxon() Error: " & ex.Message)
            Debug.Assert(False, Me.ToString & ".LoadTaxon() Error: " & ex.Message)
            Return False

        End Try

    End Function

    Private Function UpdateTaxon(ByVal iDBID As Integer) As Boolean

        Dim iTaxon As Integer = Array.IndexOf(Me.m_EcoPathData.TaxonDBID, iDBID)
        Debug.Assert(iTaxon > 0 And iTaxon <= m_EcoPathData.NumTaxon, "Failed to find Taxon index for database ID " & iDBID)

        Dim taxon As cTaxon = Me.Taxon(iTaxon)

        Me.m_EcoPathData.TaxonGroup(iTaxon) = taxon.Group
        Me.m_EcoPathData.TaxonGroupProp(iTaxon) = taxon.Proportion
        Me.m_EcoPathData.TaxonCommonName(iTaxon) = taxon.Name
        Me.m_EcoPathData.TaxonClass(iTaxon) = taxon.Class
        Me.m_EcoPathData.TaxonOrder(iTaxon) = taxon.Order
        Me.m_EcoPathData.TaxonFamily(iTaxon) = taxon.Family
        Me.m_EcoPathData.TaxonGenus(iTaxon) = taxon.Genus
        Me.m_EcoPathData.TaxonSpecies(iTaxon) = taxon.Species
        Me.m_EcoPathData.TaxonCode3A(iTaxon) = taxon.Code3A
        Me.m_EcoPathData.TaxonCodeISCAAP(iTaxon) = taxon.CodeISSCAAP
        Me.m_EcoPathData.TaxonCodeTaxon(iTaxon) = taxon.CodeTaxon
        Me.m_EcoPathData.TaxonSource(iTaxon) = taxon.Source
        Me.m_EcoPathData.TaxonSourceKey(iTaxon) = taxon.SourceKey
        Me.m_EcoPathData.TaxonLastUpdated(iTaxon) = taxon.LastUpdated

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cTaxon">taxon</see> for a given index.
    ''' </summary>
    ''' <param name="iTaxon">The index to obtain the Taxon definition for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Taxon(ByVal iTaxon As Integer) As cTaxon
        Get
            ' List will handle index / item index offsets
            Return DirectCast(Me.m_EcopathTaxon(iTaxon), cTaxon)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a taxonomy definition to an Ecopath group.
    ''' </summary>
    ''' <param name="iGroup">Group index to add the taxonomy definition to.</param>
    ''' <param name="data">Taxonomy data to add.</param>
    ''' <param name="sProportion">Proportion that this taxonomy definition contributes to the entire group.</param>
    ''' <param name="iDBID">Database ID for the new taxonomy definition.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddTaxon(ByVal iGroup As Integer, _
                             ByVal data As ITaxonData, _
                             ByVal sProportion As Single, _
                             ByRef iDBID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ' Start the actual work. The datasource will ensure the new fleet will be added througout models and scenarios
        If (DirectCast(DataSource, IEcopathDataSource).AddTaxon(Me.m_EcoPathData.GroupDBID(iGroup), data, sProportion, iDBID)) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of taxa has changed.", eCoreComponentType.EcoPath, eDataTypes.Taxon)
            bSucces = True
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a taxonomy definition from an Ecopath group.
    ''' </summary>
    ''' <param name="iTaxon">Index of the taxonomy definition to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Function RemoveTaxon(ByVal iTaxon As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.RemoveTaxon(Me.m_EcoPathData.TaxonDBID(iTaxon)) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of taxa has changed.", eCoreComponentType.EcoPath, eDataTypes.Taxon)
            bSucces = True
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

#End Region ' Taxon

#Region " Fleets "

    Private Function InitFleets() As Boolean
        Try
            Dim iFleet As Integer

            'clear out the old data
            m_EcopathFleetsInput.Clear()
            'm_FleetsOutput.Clear()

            'loop over the number of fleets 
            'adding a new fleet to the Fleets collection for each iFleet
            For iFleet = 1 To m_EcoPathData.NumFleet
                m_EcopathFleetsInput.Add(New cFleetInput(Me, m_EcoPathData.FleetDBID(iFleet)))
                'm_FleetsOutput.Add(New cFleetOutput(Me, m_EcoPathData.FleetDBID(iFleet)))
            Next iFleet

            LoadFleetInput()

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitFleets() Error: " & ex.Message)
            Return False
        End Try
    End Function

    Private Function UpdateFleetInput(ByVal iDBID As Integer) As Boolean

        Dim iFleet As Integer = Array.IndexOf(m_EcoPathData.FleetDBID, iDBID)
        Dim fleet As cFleetInput = Me.FleetInputs(iFleet)

        Try

            Debug.Assert(iFleet > 0 And iFleet <= m_EcoPathData.NumFleet, "Failed to find Fleet index for database ID " & fleet.DBID)

            Me.m_EcoPathData.FleetName(iFleet) = fleet.Name
            Me.m_EcoPathData.CostPct(iFleet, eCostIndex.Fixed) = fleet.FixedCost
            Me.m_EcoPathData.CostPct(iFleet, eCostIndex.CUPE) = fleet.CPUECost
            Me.m_EcoPathData.CostPct(iFleet, eCostIndex.Sail) = fleet.SailCost
            Me.m_EcoPathData.FleetColor(iFleet) = fleet.PoolColor

            For iGroup As Integer = 1 To m_EcoPathData.NumLiving
                Me.m_EcoPathData.Landing(iFleet, iGroup) = fleet.Landings(iGroup)
                Me.m_EcoPathData.Market(iFleet, iGroup) = fleet.OffVesselPrice(iGroup)
                Me.m_EcoPathData.Discard(iFleet, iGroup) = fleet.Discards(iGroup)
                Me.m_EcoPathData.PropDiscardMort(iFleet, iGroup) = fleet.DiscardMortality(iGroup)
            Next

            For iGroup As Integer = 1 To nDetritusGroups
                Me.m_EcoPathData.DiscardFate(iFleet, iGroup) = fleet.DiscardFate(iGroup)
            Next iGroup

        Catch ex As Exception
            cLog.Write(Me.ToString & ".updateFleets() Error: " & ex.Message)
            'ok figure out what happened!!!!!!!!!!!!!
            Debug.Assert(False, Me.ToString & ".updateFleets() Error: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadFleetInput() As Boolean
        Dim iFleet As Integer
        Dim iGroup As Integer

        Try

            For Each fleet As cFleetInput In m_EcopathFleetsInput

                fleet.AllowValidation = False

                iFleet = Array.IndexOf(m_EcoPathData.FleetDBID, fleet.DBID)

                Debug.Assert(iFleet > 0 And iFleet <= m_EcoPathData.NumFleet, "Failed to find Fleet index for database ID " & fleet.DBID.ToString)

                fleet.Resize()

                fleet.Index = iFleet

                fleet.DBID = m_EcoPathData.FleetDBID(iFleet)
                fleet.Name = m_EcoPathData.FleetName(iFleet)
                fleet.FixedCost = m_EcoPathData.CostPct(iFleet, eCostIndex.Fixed)
                fleet.CPUECost = m_EcoPathData.CostPct(iFleet, eCostIndex.CUPE)
                fleet.SailCost = m_EcoPathData.CostPct(iFleet, eCostIndex.Sail)
                fleet.PoolColor = m_EcoPathData.FleetColor(iFleet)

                For iGroup = 1 To m_EcoPathData.NumGroups
                    fleet.Landings(iGroup) = CSng(m_EcoPathData.Landing(iFleet, iGroup))
                    fleet.OffVesselPrice(iGroup) = m_EcoPathData.Market(iFleet, iGroup)
                    fleet.Discards(iGroup) = CSng(m_EcoPathData.Discard(iFleet, iGroup))
                    fleet.DiscardMortality(iGroup) = m_EcoPathData.PropDiscardMort(iFleet, iGroup)
                Next

                For iGroup = 1 To nDetritusGroups
                    fleet.DiscardFate(iGroup) = m_EcoPathData.DiscardFate(iFleet, iGroup)
                Next iGroup

                fleet.ResetStatusFlags()
                fleet.AllowValidation = True
            Next

            Return True

        Catch ex As Exception

            cLog.Write(Me.ToString() & ".LoadFleets() Error: " & ex.Message)
            Debug.Assert(False, Me.ToString & ".LoadFleets() Error: " & ex.Message)
            Return False

        End Try

    End Function

    Public ReadOnly Property FleetInputs(ByVal iFleet As Integer) As cFleetInput

        Get
            Try
                ' List handles item index offset
                Return DirectCast(m_EcopathFleetsInput(iFleet), cFleetInput)
            Catch ex As Exception
                Return Nothing
            End Try
        End Get

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a Fleet to the system.
    ''' </summary>
    ''' <param name="strName">Name of the fleet.</param>
    ''' <param name="iFleet">Position to insert fleet into the current fleet list. This position may be modified by this call.</param>
    ''' <param name="iFleetID">Database ID assigned to the new fleet.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddFleet(ByVal strName As String, ByRef iFleet As Integer, ByRef iFleetID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ' Start the actual work. The datasource will ensure the new fleet will be added througout models and scenarios
        If (DirectCast(DataSource, IEcopathDataSource).AddFleet(strName, iFleet, iFleetID)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetInput)
            'DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetOutput)

            If Me.ActiveEcospaceScenarioIndex > 0 Then
                Me.DataAddedOrRemovedMessage("EcoSpace number of fleets has changed.", eCoreComponentType.EcoSpace, eDataTypes.EcospaceFleet)
            End If

            bSucces = True

        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    Public Function RemoveFleet(ByVal iFleet As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.RemoveFleet(Me.m_EcoPathData.FleetDBID(iFleet)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetInput)
            'Me.DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetOutput)

            If Me.ActiveEcospaceScenarioIndex > 0 Then
                Me.DataAddedOrRemovedMessage("EcoSpace number of fleets has changed.", eCoreComponentType.EcoSpace, eDataTypes.EcospaceFleet)
            End If

            bSucces = True
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    Public Function MoveFleet(ByVal iFleet As Integer, ByVal iIndex As Integer) As Boolean
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.MoveFleet(Me.m_EcoPathData.FleetDBID(iFleet), iIndex) Then

            Me.DataAddedOrRemovedMessage("Ecopath fleet order has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetInput)
            'Me.DataAddedOrRemovedMessage("Ecopath fleet order has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetOutput)

            If Me.ActiveEcospaceScenarioIndex > 0 Then
                Me.DataAddedOrRemovedMessage("EcoSpace group order has changed.", eCoreComponentType.EcoSpace, eDataTypes.EcospaceFleet)
            End If

            bSucces = True
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

#End Region ' Fleets

#Region " Particle size distribution "

    ''' <summary>
    ''' Returns the <see cref="cEwEModel">EwE model</see> for the current loaded datasource.
    ''' </summary>
    Public ReadOnly Property ParticleSizeDistributionParameters() As cPSDParameters
        Get
            Return Me.m_PSDParameters
        End Get
    End Property

    Private Function InitPSDParameters() As Boolean
        Me.m_PSDParameters = New cPSDParameters(Me)
        Return Me.LoadPSDParameters()
    End Function

    Private Function LoadPSDParameters() As Boolean

        Me.m_PSDParameters.AllowValidation = False

        Me.m_PSDParameters.PSDEnabled = Me.m_PSDData.Enabled
        Me.m_PSDParameters.MortalityType = Me.m_PSDData.MortalityType
        Me.m_PSDParameters.NumWeightClasses = Me.m_PSDData.NWeightClasses
        Me.m_PSDParameters.FirstWeightClass = Me.m_PSDData.FirstWeightClass
        Me.m_PSDParameters.ClimateType = Me.m_PSDData.ClimateType
        Me.m_PSDParameters.NumPtsMovAvg = Me.m_PSDData.NPtsMovAvg

        For iGroup As Integer = 1 To m_EcoPathData.NumGroups
            Me.m_PSDParameters.GroupIncluded(iGroup) = Me.m_PSDData.Include(iGroup)
        Next

        Me.m_PSDParameters.ResetStatusFlags()

        Me.m_PSDParameters.AllowValidation = True

        Return True
    End Function

    Private Function UpdatePSDParameters() As Boolean

        Me.m_PSDData.Enabled = Me.m_PSDParameters.PSDEnabled
        Me.m_PSDData.MortalityType = Me.m_PSDParameters.MortalityType
        Me.m_PSDData.NWeightClasses = Me.m_PSDParameters.NumWeightClasses
        Me.m_PSDData.FirstWeightClass = Me.m_PSDParameters.FirstWeightClass
        Me.m_PSDData.ClimateType = Me.m_PSDParameters.ClimateType
        Me.m_PSDData.NPtsMovAvg = Me.m_PSDParameters.NumPtsMovAvg

        For iGroup As Integer = 1 To m_EcoPathData.NumGroups
            Me.m_PSDData.Include(iGroup) = Me.m_PSDParameters.GroupIncluded(iGroup)
        Next

    End Function

#End Region ' Particle size distribution

#Region " Stats "

    Friend Sub LoadEcopathStats()
        Try

            Dim sTroughput As Single = Me.m_EcoPathData.Consum + Me.m_EcoPathData.SumEx + Me.m_EcoPathData.Dt + Me.m_EcoPathData.RTZ

            Me.m_EcopathStats.TotalConsumption = Me.m_EcoPathData.Consum
            Me.m_EcopathStats.TotalExports = Me.m_EcoPathData.SumEx
            Me.m_EcopathStats.TotalRespFlow = Me.m_EcoPathData.RTZ
            Me.m_EcopathStats.TotalFlowDetritus = Me.m_EcoPathData.Dt
            Me.m_EcopathStats.TotalThroughput = sTroughput
            Me.m_EcopathStats.TotalProduction = Me.m_EcoPathData.SumP

            If (Me.m_EcoPathData.GEff > 0) Then
                Me.m_EcopathStats.MeanTrophicLevelCatch = Me.m_EcoPathData.TLcatch
                Me.m_EcopathStats.GrossEfficiency = Me.m_EcoPathData.GEff
            Else
                Me.m_EcopathStats.MeanTrophicLevelCatch = cCore.NULL_VALUE
                Me.m_EcopathStats.GrossEfficiency = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.TotalNetPP = Me.m_EcoPathData.PProd

            If (Me.m_EcoPathData.Totpp > 0) Then
                If (Me.m_EcoPathData.RTZ > 0) Then
                    Me.m_EcopathStats.TotalPResp = Me.m_EcoPathData.Totpp / Me.m_EcoPathData.RTZ
                Else
                    Me.m_EcopathStats.TotalPResp = cCore.NULL_VALUE
                End If

                Me.m_EcopathStats.NetSystemProduction = Me.m_EcoPathData.Totpp - Me.m_EcoPathData.RTZ
                Me.m_EcopathStats.TotalPB = Me.m_EcoPathData.Totpp / Me.m_EcoPathData.SumBio
            Else
                If (Me.m_EcoPathData.RTZ > 0) Then
                    Me.m_EcopathStats.TotalPResp = Me.m_EcoPathData.PProd / Me.m_EcoPathData.RTZ
                Else
                    Me.m_EcopathStats.TotalPResp = cCore.NULL_VALUE
                End If

                Me.m_EcopathStats.NetSystemProduction = Me.m_EcoPathData.PProd - Me.m_EcoPathData.RTZ
                Me.m_EcopathStats.TotalPB = Me.m_EcoPathData.PProd / Me.m_EcoPathData.SumBio
            End If

            If (sTroughput > 0) Then
                Me.m_EcopathStats.TotalBT = Me.m_EcoPathData.SumBio / sTroughput
            Else
                Me.m_EcopathStats.TotalBT = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.TotalBNonDet = Me.m_EcoPathData.SumBio

            If Me.m_EcoPathData.CatchSum > 0 Then
                Me.m_EcopathStats.TotalCatch = Me.m_EcoPathData.CatchSum
            Else
                Me.m_EcopathStats.TotalCatch = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.ConnectanceIndex = Me.m_EcoPathData.Conn

            If (Me.m_EcoPathData.SysOm > 0) Then
                Me.m_EcopathStats.OmnivIndex = Me.m_EcoPathData.SysOm
            Else
                Me.m_EcopathStats.OmnivIndex = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.TotalMarketValue = Me.m_EcoPathData.LandingValue
            Me.m_EcopathStats.TotalShadowValue = Me.m_EcoPathData.ShadowValue
            Me.m_EcopathStats.TotalValue = Me.m_EcoPathData.LandingValue + Me.m_EcoPathData.ShadowValue
            Me.m_EcopathStats.TotalFixedCost = Me.m_EcoPathData.Fixed
            Me.m_EcopathStats.TotalVarCost = Me.m_EcoPathData.Variab
            Me.m_EcopathStats.TotalCost = Me.m_EcoPathData.Fixed + Me.m_EcoPathData.Variab
            Me.m_EcopathStats.Profit = Me.m_EcoPathData.LandingValue + Me.m_EcoPathData.ShadowValue - (Me.m_EcoPathData.Fixed + Me.m_EcoPathData.Variab)

            Me.m_EcopathStats.ResetStatusFlags()

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ArgumentException(Me.ToString & ".LoadEcopathStats() Error: " & ex.Message, ex)
        End Try
    End Sub

#End Region ' Stats


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return a <see cref="cStanzaGroup">stanza group</see> from the core.
    ''' </summary>
    ''' <param name="iIndex">Zero-based index of the group.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property StanzaGroups(ByVal iIndex As Integer) As cStanzaGroup
        Get
            Return m_stanzaGroups(iIndex)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Stanza index for this iGroup. This index is one-based.
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <returns>Gets Stanza index if this group is a stanza group, or
    ''' cCore.NULL_VALUE if this group does not belong to a stanza configuration.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function getStanzaIndexForGroup(ByVal iGroup As Integer) As Integer

        For i As Integer = 1 To m_Stanza.Nsplit

            For ii As Integer = 1 To m_Stanza.Nstanza(i)
                If iGroup = m_Stanza.EcopathCode(i, ii) Then
                    Return i - 1 'stanzas are indexed from zero
                End If
            Next ii

        Next i

        Return NULL_VALUE

    End Function

    ''' <summary>
    ''' Public function to run the EcoPath model
    ''' </summary>
    ''' <returns>True if EcoPath model ran successfully. False if a problem was encountered</returns>
    ''' <remarks>
    ''' InitEcoPath() must be called before this can be called
    ''' </remarks>
    Public Function RunEcoPath() As Boolean

        Dim bSucces As Boolean = False
        Dim msg As cMessage

        Try

            If Me.m_StateMonitor.HasEcopathLoaded() = False Then
                msg = CreateMessage(My.Resources.CoreMessages.ECOPATH_ERROR_NOMODEL, eCoreComponentType.EcoPath, eMessageType.ErrorEncountered)
                m_publisher.AddMessage(msg)

                cLog.Write(Me.ToString & ".RunEcoPath() Failed EcoPath Model has not been initialized. InitEcoPath(filename) must be called before .RunEcoPath().")
                Return False
            End If

            'make sure this is set correctly for this call
            'other things (Monte Carlo) could have changed this
            m_EcoPath.ParameterEstimationType = eEstimateParameterFor.ParameterEstimation

            ' Update core state
            Me.m_StateMonitor.SetEcopathRun()

            'copy all input data into the modeling arrays 
            m_EcoPathData.CopyInputToModelArrays()

            Me.ResetEcopathGroupOutputs()

            'call EcoPath to estimate the missing parameters
            If (m_EcoPath.Run()) Then

                're-populate the output list with the new outputs from Ecopath
                LoadEcopathOutputs()
                're-populate the Ecopath statistics
                LoadEcopathStats()

                If Me.PluginManager IsNot Nothing Then
                    Me.PluginManager.EcopathRunCompleted(m_EcoPathData)
                End If

                bSucces = True

            Else 'If mEcoPath.EstimateParameters() Then

                'I am assuming here that if EcoPath returned false it has already sent a message that explains the problem 
                'so I don't need to send another message

                cLog.Write(Me.ToString & ".RunEcoPath() Failed to Estimate Parameters.")

                bSucces = False

            End If

        Catch ex As Exception

            msg = CreateMessage(String.Format(My.Resources.CoreMessages.ECOPATH_RUN_ERROR_EXCEPTION, ex.Message), _
                    eCoreComponentType.EcoPath, eMessageType.ErrorEncountered)
            m_publisher.AddMessage(msg)

            cLog.Write(Me.ToString & ".RunEcoPath() Error. " & ex.Message)
            Debug.Assert(False)
            bSucces = False
        End Try

        ' Did Ecopath run succesful?
        If bSucces Then

            msg = New cMessage(My.Resources.CoreMessages.ECOPATH_RUN_SUCCESS, eMessageType.Any, eCoreComponentType.EcoPath, eMessageImportance.Information)
            m_publisher.AddMessage(msg)

            ' Update core state monitor
            Me.m_StateMonitor.SetEcopathCompleted()

            ' Now deal with PSD

            ' PSD not enabled?
            If (Me.m_PSDData.Enabled) Then

                'copy all PSD data into the modeling arrays 
                m_PSDData.CopyInputToModelArrays()

                ' Run PSD
                If m_psdModel.Run() Then

                    're-populate PSD parameters
                    LoadPSDParameters()

                    msg = New cMessage(My.Resources.CoreMessages.PSD_RUN_SUCCESS, eMessageType.Any, eCoreComponentType.EcoPath, eMessageImportance.Information)
                    m_publisher.AddMessage(msg)

                    Me.m_StateMonitor.SetPSDCompleted()
                Else
                    msg = New cMessage(My.Resources.CoreMessages.PSD_RUN_ERROR, eMessageType.Any, eCoreComponentType.EcoPath, eMessageImportance.Information)
                    m_publisher.AddMessage(msg)
                End If
            End If
        Else
            msg = New cMessage(My.Resources.CoreMessages.ECOPATH_RUN_ERROR, eMessageType.ErrorEncountered, eCoreComponentType.EcoPath, eMessageImportance.Warning)
            m_publisher.AddMessage(msg)

            ' Update core state monitor
            Me.m_StateMonitor.SetEcopathLoaded(True)
        End If

        ' Unleash all messages after core state monitor is up to date
        m_publisher.sendAllMessages()

        Return bSucces

    End Function

    ''' <summary>
    ''' Take a cMessage object generated by EcoPath and set any flags in the input or output 
    ''' that can be used by an interface to display the problem.
    ''' </summary>
    ''' <param name="msg">cMessage object created by EcoPath</param>
    ''' <remarks>
    ''' Called by EcoPathMessage_Handler(cMessage) when a message has been sent from EcoPath to the Core
    ''' </remarks>
    Private Sub processMessageFromEcopath(ByVal msg As cMessage)

        Dim var As cVariableStatus = Nothing
        Dim i As Integer = 0

        Try

            'for each variable in the EcoPath generated message set any status flags in the input or output objects
            'this is so that an interface can see the status of the messages
            For Each var In msg.Variables

                Select Case var.DataType

                    Case eDataTypes.EcoPathGroupInput

                        Dim inputGrp As cEcoPathGroupInput = Me.EcoPathGroupInputs(var.Index)

                        'DietComp needs to be handled differently
                        If var.VarName = eVarNameFlags.DietComp Then
                            For i = 1 To nGroups
                                inputGrp.SetStatus(var.VarName, var.Status, var.Index)
                            Next

                        Else
                            Dim tmpstatus As eStatusFlags = inputGrp.GetStatus(var.VarName)
                            inputGrp.SetStatus(var.VarName, tmpstatus, var.iArrayIndex)
                            tmpstatus = tmpstatus Or var.Status
                        End If

                        'set the reference to the parent object of this variable
                        'this could not be set by EcoPath because it has no idea what this is
                        var.CoreDataObject = inputGrp

                    Case eDataTypes.EcoPathGroupOutput

                        Dim outputGrp As cEcoPathGroupOutput = Me.EcoPathGroupOutputs(var.Index)
                        Dim tmpstatus As eStatusFlags = outputGrp.GetStatus(var.VarName)
                        tmpstatus = tmpstatus Or var.Status

                        outputGrp.SetStatus(var.VarName, tmpstatus, var.iArrayIndex)
                        'set the reference to the parent object of this variable
                        'this could not be set by EcoPath because it has no idea what this is
                        var.CoreDataObject = outputGrp

                    Case eDataTypes.FleetInput

                        Dim inputFleet As cFleetInput = DirectCast(Me.m_EcopathFleetsInput(var.Index), cFleetInput)
                        Dim tmpstatus As eStatusFlags = inputFleet.GetStatus(var.VarName)
                        tmpstatus = tmpstatus Or var.Status

                        inputFleet.SetStatus(var.VarName, tmpstatus, var.iArrayIndex)
                        'set the reference to the parent object of this variable
                        'this could not be set by EcoPath because it has no idea what this is
                        var.CoreDataObject = inputFleet

                    Case Else

                        'A message got sent by EcoPath that is not being handled here.
                        'This is probable wrong. 
                        'Any variable that had its status flag set by EcoPath should have its status flag set in the interface.
                        cLog.Write("Message sent to Core from EcopPath that is not handled by processMessageFromEcopath(cMessage). Message = " & msg.Message)
                        Debug.Assert(False, "Variable from EcoPath not handled by the Core.")

                End Select

            Next var

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' This is the message handler that got passed to EcoPath during the Initialization routine. See InitEcopath().
    ''' It will be called by EcoPath when ever it needs to tell the Core that something has happen.
    ''' </summary>
    ''' <param name="message">Message Object from EcoPath contains any information that is needed to process the message</param>
    ''' <remarks>
    ''' Take a messages that originated in EcoPath 
    ''' and run whatever processing has to happen to pass the message out to the Observers 
    '''  </remarks>
    Private Sub EcoPathMessage_Handler(ByRef message As cMessage)

        Try

            'this will take the variable status messages contained in the message object generated by EcoPath
            'and set flags in the input or output objects of the Core that can be used by an interface
            processMessageFromEcopath(message)

            If TypeOf message Is cFeedbackMessage Then
                m_publisher.SendMessage(message)
            Else
                m_publisher.AddMessage(message)
            End If


        Catch ex As Exception
            'OOPS
            'not much we can do at this point as there is no place to post the message too
            cLog.Write(Me.ToString & ".EcoPathMessage_Handler(...) Error:" & ex.Message)
            Debug.Assert(False)
        End Try

    End Sub

    Private Sub PSDMessage_Handler(ByRef message As cMessage)

        Try

            'this will take the variable status messages contained in the message object generated by EcoPath
            'and set flags in the input or output objects of the Core that can be used by an interface
            processMessageFromEcopath(message)

            If TypeOf message Is cFeedbackMessage Then
                m_publisher.SendMessage(message)
            Else
                m_publisher.AddMessage(message)
            End If


        Catch ex As Exception
            'OOPS
            'not much we can do at this point as there is no place to post the message too
            cLog.Write(Me.ToString & ".EcoPathMessage_Handler(...) Error:" & ex.Message)
            Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Normalize ecopath input values
    ''' </summary>
    Public Sub NormalizeDietInput()
        ' Sanity check
        Debug.Assert(Me.StateMonitor.HasEcopathLoaded())
        ' Normalize ecopath DCInput
        Me.m_EcoPathData.SumDCToOne(True)
        ' Refresh ecopath groups
        Me.LoadEcopathInputs()
        Me.m_StateMonitor.SetEcopathLoaded(True)
        ' Send out data changed message for ecopath
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.EcoPath, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag datasource as dirty
        Me.DataSource.SetChanged(eCoreComponentType.EcoPath)
        Me.m_StateMonitor.UpdateDataState(DataSource)

    End Sub

    ''' <summary>
    ''' Statistics from the last Ecopath model run
    ''' </summary>
    Public ReadOnly Property EcopathStats() As cEcoPathStats
        Get
            Return Me.m_EcopathStats
        End Get
    End Property

#Region " Status flags updating "

    Friend Function Set_PP_Flags(ByVal obj As cEcoSimGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        Dim sPP As Single = obj.PP

        If sPP = 1.0 Then
            obj.ClearStatusFlags(eVarNameFlags.MaxRelPB, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.SetStatusFlags(eVarNameFlags.MaxRelFeedingTime, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.SetStatusFlags(eVarNameFlags.FeedingTimeAdjRate, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.SetStatusFlags(eVarNameFlags.OtherMortFeedingTime, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.SetStatusFlags(eVarNameFlags.PredEffectFeedingTime, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.SetStatusFlags(eVarNameFlags.DenDepCatchability, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.SetStatusFlags(eVarNameFlags.QBMaxQBio, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.SetStatusFlags(eVarNameFlags.SwitchingPower, eStatusFlags.NotEditable Or eStatusFlags.Null)
        Else
            obj.SetStatusFlags(eVarNameFlags.MaxRelPB, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.ClearStatusFlags(eVarNameFlags.MaxRelFeedingTime, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.ClearStatusFlags(eVarNameFlags.FeedingTimeAdjRate, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.ClearStatusFlags(eVarNameFlags.OtherMortFeedingTime, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.ClearStatusFlags(eVarNameFlags.PredEffectFeedingTime, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.ClearStatusFlags(eVarNameFlags.DenDepCatchability, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.ClearStatusFlags(eVarNameFlags.QBMaxQBio, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.ClearStatusFlags(eVarNameFlags.SwitchingPower, eStatusFlags.NotEditable Or eStatusFlags.Null)
        End If

    End Function

    ''' <summary>
    ''' Set the NotEditable flags for BioAcum or BiomAccumRate based on data in only one of the two
    ''' </summary>
    ''' <param name="obj">The object to update.</param>
    ''' <param name="bSendMessage">True to send a message, False to suppress this.</param>
    ''' <returns>Always true.</returns>
    ''' <remarks>This is called by <see cref="PostVariableValidation">PostVariableValidation</see> 
    ''' to set status when a variable has been edited, as well as <see cref="cEcoPathGroupInput.ResetStatusFlags">ResetStatusFlags</see>
    ''' when the object is first created.</remarks>
    Friend Function set_BioAccumRate_Flags(ByVal obj As cEcoPathGroupInput, Optional ByVal VarName As eVarNameFlags = eVarNameFlags.NotSet, Optional ByVal bSendMessage As Boolean = True) As Boolean

        'If Multi Stanza then Not Editable
        'If VarName supplied then called for validation of variable
        'If No VarName then call for initialization
        obj.AllowValidation = False

        Dim varnameToDisable As eVarNameFlags
        Dim bDisable As Boolean = True
        Dim iGrp As Integer = obj.Index

        If obj.isMultiStanza Then

            'BioAccum and BioAccumRate are not editable if this is a multi stanza group
            obj.SetStatusFlags(eVarNameFlags.BioAccum, eStatusFlags.NotEditable)
            obj.SetStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.NotEditable)

        Else 'If obj.isMultiStanza Then
            'not a multi stanza group so set the status flag

            If VarName = eVarNameFlags.NotSet Then
                'caller has not supplied a varname
                'this is called for initialization

                If CSng(obj.GetVariable(eVarNameFlags.BioAccum)) = 0 And CSng(obj.GetVariable(eVarNameFlags.BioAccumRate)) = 0 Then
                    'if neither is set then both are OK to edit
                    bDisable = False
                    obj.SetStatusFlags(eVarNameFlags.BioAccum, eStatusFlags.OK)
                    obj.SetStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.OK)

                ElseIf CSng(obj.GetVariable(eVarNameFlags.BioAccum)) <> 0 And CSng(obj.GetVariable(eVarNameFlags.BioAccumRate)) <> 0 Then
                    'if both are set then make BioAccum blocked
                    varnameToDisable = eVarNameFlags.BioAccum
                    VarName = eVarNameFlags.BioAccumRate

                ElseIf CSng(obj.GetVariable(eVarNameFlags.BioAccum)) <> 0 Then
                    'if one is set then the other is blocked
                    varnameToDisable = eVarNameFlags.BioAccumRate
                    VarName = eVarNameFlags.BioAccum
                Else
                    varnameToDisable = eVarNameFlags.BioAccum
                    VarName = eVarNameFlags.BioAccumRate
                End If


            Else 'VarName <> eVarNameFlags.NotSet
                'caller supplied a VarName 
                'VarName has been edited

                'which var is being edited (block the other)
                If VarName = eVarNameFlags.BioAccum Then
                    varnameToDisable = eVarNameFlags.BioAccumRate
                Else
                    varnameToDisable = eVarNameFlags.BioAccum
                End If

            End If 'If varname = eVarNameFlags.NotSet Then

            If bDisable Then
                ' Make un-editable if the other var is populated
                If CSng(obj.GetVariable(VarName)) <> 0 Then
                    obj.SetStatusFlags(varnameToDisable, eStatusFlags.NotEditable) 'Or eStatusFlags.Null
                    obj.ClearStatusFlags(VarName, eStatusFlags.NotEditable) 'Or eStatusFlags.Null
                Else
                    obj.ClearStatusFlags(varnameToDisable, eStatusFlags.NotEditable) 'Or eStatusFlags.Null
                End If

            End If 'If block Then

        End If ' If obj.isMultiStanza Then

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True
        Return True

    End Function

    ''' <summary>
    ''' Set the NotEditable flags for PB QB and GE
    ''' </summary>
    ''' <param name="obj">The object to update.</param>
    ''' <param name="bSendMessage">False not to send a message</param>
    ''' <returns>Always true.</returns>
    ''' <remarks>This is called by <see cref="PostVariableValidation">PostVariableValidation</see> 
    ''' to set status when a variable has been edited, as well as <see cref="cEcoPathGroupInput.ResetStatusFlags">ResetStatusFlags</see>
    ''' when the object is first created.</remarks>
    Friend Function Set_PB_QB_GE_BA_Flags(ByVal obj As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        'Make the variable(s) un-editable under certain circumstances
        'see EwE5 frmInputData.LockInputFor_PB_QB_GE(...)

        Dim sQB As Single = CSng(obj.GetVariable(eVarNameFlags.QBInput))
        Dim sGE As Single = CSng(obj.GetVariable(eVarNameFlags.GEInput))
        Dim sPB As Single = CSng(obj.GetVariable(eVarNameFlags.PBInput))
        Dim bLockGE As Boolean = False
        Dim bLockQB As Boolean = False
        Dim bLockPB As Boolean = False
        Dim bLockBA As Boolean = False

        Dim bIsPartOfStanza As Boolean = obj.isMultiStanza()
        Dim bIsDetritus As Boolean = (obj.PP > 1.1)
        Dim bIsProducer As Boolean = (obj.PP = 1.0)

        obj.AllowValidation = False

        ' Stanza: block all
        If bIsPartOfStanza Then bLockGE = True : bLockQB = True : bLockPB = True : bLockBA = True

        If bIsDetritus Then
            ' Detritus: block all
            bLockGE = True
            bLockQB = True
            bLockPB = True
        ElseIf bIsProducer Then
            ' Producer: block all non-PB
            bLockGE = True
            bLockQB = True
        Else
            ' This logic comes from the original code
            bLockGE = bLockGE Or (sPB > 0.0 And sQB > 0.0)
            bLockQB = bLockQB Or (sPB > 0.0 And sGE > 0.0)
            bLockPB = bLockPB Or (sQB > 0.0 And sGE > 0.0)
        End If

        ' Update status flags
        If bLockGE Then
            obj.SetStatusFlags(eVarNameFlags.GEInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.GEInput, eStatusFlags.NotEditable)
        End If

        If bLockQB Then
            obj.SetStatusFlags(eVarNameFlags.QBInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.QBInput, eStatusFlags.NotEditable)
        End If

        If bLockPB Then
            obj.SetStatusFlags(eVarNameFlags.PBInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.PBInput, eStatusFlags.NotEditable)
        End If

        If bLockBA Then
            obj.SetStatusFlags(eVarNameFlags.BiomassAreaInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.BiomassAreaInput, eStatusFlags.NotEditable)
        End If

        obj.AllowValidation = True

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="bSendMessage"></param>
    ''' <returns>Always true.</returns>
    Friend Function Set_GS_Flags(ByVal obj As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        ' See EwE5 frmInputData.LockInputForProducers(..)
        obj.AllowValidation = False

        If (obj.PP >= 1.0) Then
            obj.SetStatusFlags(eVarNameFlags.GS, eStatusFlags.NotEditable Or eStatusFlags.Null)
            obj.GS = 0
        Else
            obj.ClearStatusFlags(eVarNameFlags.GS, eStatusFlags.NotEditable Or eStatusFlags.Null)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True

        Return True
    End Function


    Friend Function Set_EE_Flags(ByVal obj As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        ' See EwE5 frmInputData.DisplayBasicInput(..), detritus comment
        obj.AllowValidation = False

        If (obj.PP > 1.0) Then
            obj.SetStatusFlags(eVarNameFlags.EEInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.EEInput, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True

    End Function

    Friend Function Set_DetImp_Flags(ByVal obj As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        ' See EwE5 frmInputData.ForMatNewGroupBasicInput(..), case 10
        obj.AllowValidation = False

        If (obj.PP <= 1.0) Then
            obj.SetStatusFlags(eVarNameFlags.DetImp, eStatusFlags.NotEditable Or eStatusFlags.Null)
        Else
            obj.ClearStatusFlags(eVarNameFlags.DetImp, eStatusFlags.NotEditable Or eStatusFlags.Null)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True

    End Function

    ''' <summary>
    ''' Set the NotEditable flags for Emigration values when a group is part of a stanza configuration
    ''' </summary>
    ''' <param name="obj">The object to update.</param>
    ''' <param name="bSendMessage">False not to send a message.</param>
    ''' <returns>Always true.</returns>
    ''' <remarks>
    ''' <para>When a group is part of a Stanza configuration, its migration parameters
    ''' should be blocked for input. Original email message:</para>
    ''' <para>Thu, 30 Nov 2006 10:44:13 -0800 (PST)</para>
    ''' <para>Carl, I have blocked entry of migration parameters for stanza's (we need to do the same in EwE6 folks)</para>
    ''' <para>Villy</para>
    ''' <para> --------------------------------------------------------------------------------</para>
    ''' <para>From: Carl(Walters)</para>
    ''' <para>Sent: Thursday, November 30, 2006 07:14</para>
    ''' <para>To: Villy Christensen</para>
    ''' <para>Subject: problem with migration for multistanza groups</para>
    ''' <para>Villy,</para>
    ''' <para>That Cowan student uncovered a “bug” in the “other production” interface in ecopath. 
    ''' We do not include immigration accounting in Ecosim, so if a user sets nonzero 
    ''' immigration and emigration rates, only the emigration contribution to Z is included
    ''' in the multistanza dynamics. The problem with modeling immigration is how to specify
    ''' age-specific immigration rates for each age within any stanza specified to have
    ''' immigrating biomass; there is no obvious way to do the rates in a robustway,
    ''' especially considering that weights at age of immigrants may differ from those of
    ''' “resident” creatures. I think the best strategy is just to not allow rates to be set
    ''' to nonzero values in the other production interface.</para>
    ''' <para>Carl</para>
    ''' </remarks>
    Friend Function Set_Migration_Flags(ByVal obj As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        obj.AllowValidation = False

        ' JS061214: All Migration/Other production related variables are read-only for stanza groups
        If (obj.isMultiStanza) Then
            obj.SetStatusFlags(eVarNameFlags.Immig, eStatusFlags.NotEditable)
            obj.SetStatusFlags(eVarNameFlags.Emig, eStatusFlags.NotEditable)
            obj.SetStatusFlags(eVarNameFlags.EmigRate, eStatusFlags.NotEditable)
            'obj.SetStatusFlags(eVarNameFlags.BioAccum, eStatusFlags.NotEditable)
            'obj.SetStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.Immig, eStatusFlags.NotEditable)
            obj.ClearStatusFlags(eVarNameFlags.Emig, eStatusFlags.NotEditable)
            obj.ClearStatusFlags(eVarNameFlags.EmigRate, eStatusFlags.NotEditable)
            'obj.ClearStatusFlags(eVarNameFlags.BioAccum, eStatusFlags.NotEditable)
            'obj.ClearStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True

        Return True
    End Function

    Friend Function Set_IBM_Flags(ByVal obj As cEcospaceModelParameters, Optional ByVal bSendMessage As Boolean = True) As Boolean

        obj.AllowValidation = False

        If (obj.UseIBM) Then
            obj.ClearStatusFlags(eVarNameFlags.PacketsMultiplier, eStatusFlags.NotEditable)
        Else
            obj.SetStatusFlags(eVarNameFlags.PacketsMultiplier, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoSpace, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True
        Return True
    End Function

    Friend Function Set_MarketPrice_Flags(ByVal obj As cFleetInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        obj.AllowValidation = False

        For iGroup As Integer = 1 To Me.nGroups
            If obj.Landings(iGroup) = 0.0! Then
                obj.SetStatusFlags(eVarNameFlags.OffVesselPrice, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            Else
                obj.ClearStatusFlags(eVarNameFlags.OffVesselPrice, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            End If
        Next

        If bSendMessage Then
            Me.m_publisher.SendMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.FleetInput))
        End If

        obj.AllowValidation = True
        Return True
    End Function

    Friend Function Set_Quota_Flags(ByVal fleetMSE As cMSEFleetInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        If fleetMSE Is Nothing Then
            'If Ecosim has not been loaded then the cMSEFleetInput objects will be Nothing
            'boot out of here in that case
            Return False
        End If

        fleetMSE.AllowValidation = False

        Dim fleet As cFleetInput = Me.FleetInputs(fleetMSE.Index)
        For iGroup As Integer = 1 To Me.nGroups
            If (fleet.Landings(iGroup) + fleet.Discards(iGroup)) = 0.0! Then
                fleetMSE.SetStatusFlags(eVarNameFlags.QuotaShare, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            Else
                fleetMSE.ClearStatusFlags(eVarNameFlags.QuotaShare, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            End If
        Next

        If bSendMessage Then
            Me.m_publisher.SendMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.MSEFleetInput))
        End If

        fleetMSE.AllowValidation = True
        Return True
    End Function

    Friend Function Set_DiscardMort_Flags(ByVal fleet As cFleetInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        fleet.AllowValidation = False

        For iGroup As Integer = 1 To Me.nGroups
            'jb 7-Jan-2010 changed to allow setting of discard mort on groups with landing as well
            'MSE can discard excess catches so we need to be able to set DiscardMortality on landings
            If (fleet.Discards(iGroup) + fleet.Landings(iGroup)) <= 0.0! Then
                fleet.SetStatusFlags(eVarNameFlags.DiscardMortality, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            Else
                fleet.ClearStatusFlags(eVarNameFlags.DiscardMortality, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            End If
        Next

        If bSendMessage Then
            Me.m_publisher.SendMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.FleetInput))
        End If

        fleet.AllowValidation = True
        Return True
    End Function

    Friend Function Set_VBK_Flags(ByVal group As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        Dim sg As cStanzaGroup = Nothing
        Dim groupLeading As cEcoPathGroupInput = Nothing
        Dim bIsLeadingGroup As Boolean = False

        group.AllowValidation = False

        ' Is a multi-stanza group?
        If group.isMultiStanza Then
            ' #Yes: configure VBK editable mode
            sg = Me.StanzaGroups(group.iStanza)

            ' Get the leading group for this stanza config

            groupLeading = Me.EcoPathGroupInputs(sg.iGroups(sg.LeadingB))
            bIsLeadingGroup = Object.ReferenceEquals(groupLeading, group)

            ' Is leading stanza?
            If bIsLeadingGroup Then
                ' #Yes: make VBK editable to the user
                group.ClearStatusFlags(eVarNameFlags.VBK, eStatusFlags.NotEditable)
            Else
                ' #No: make VBK read-only to the user
                group.SetStatusFlags(eVarNameFlags.VBK, eStatusFlags.NotEditable)
            End If
        Else
            ' #No: Make VBK editable to the user
            group.ClearStatusFlags(eVarNameFlags.VBK, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        group.AllowValidation = True
    End Function

    Friend Function Set_Tcatch_Flags(ByVal group As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean

        Dim iGroup As Integer
        Dim bIsFished As Boolean = False

        group.AllowValidation = False

        'convert the Database ID into an iGroup
        iGroup = Array.IndexOf(m_EcoPathData.GroupDBID, group.DBID)
        ' haha
        Debug.Assert(group.Index = iGroup)

        ' Is multi-stanza?
        If group.isMultiStanza Then
            ' #Yes: determine if this is the first stanza group that is being fished (ouch)
            Dim sg As cStanzaGroup = Me.StanzaGroups(group.iStanza)
            Dim iAgeYoungest As Integer = 0
            Dim iYoungest As Integer = 0

            ' Determine lifestage index of youngest life stage that is being fished
            ' ..For all life stages
            For iLifestage As Integer = 1 To sg.NStanzas
                ' ..For all fleets
                For iFleet As Integer = 1 To Me.nFleets
                    ' Is this life stage being caught?
                    If (Me.m_EcoPathData.Landing(iFleet, sg.iGroups(iLifestage)) + _
                        Me.m_EcoPathData.Discard(iFleet, sg.iGroups(iLifestage))) > 0 Then

                        ' #Yes: remember youngest life stage index 
                        If (bIsFished = False) Or (sg.StartAge(iLifestage) < iAgeYoungest) Then
                            iAgeYoungest = sg.StartAge(iLifestage)
                            iYoungest = sg.iGroups(iLifestage)
                            bIsFished = True
                        End If
                    End If
                Next
            Next

            bIsFished = bIsFished And (iYoungest = iGroup)

        Else
            ' #No: is being fished?
            For iFleet As Integer = 1 To Me.nFleets
                If (Me.m_EcoPathData.Landing(iFleet, iGroup) + Me.m_EcoPathData.Discard(iFleet, iGroup)) > 0 Then
                    bIsFished = True
                    Exit For
                End If
            Next
        End If

        ' Is being fished?
        If bIsFished Then
            ' #Yes: make Tcatch editable to the user
            group.ClearStatusFlags(eVarNameFlags.TCatchInput, eStatusFlags.NotEditable)
        Else
            ' #No: make Tcatch read-only to the user
            group.SetStatusFlags(eVarNameFlags.TCatchInput, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        group.AllowValidation = True
    End Function

    Friend Function Set_Tmax_Flags(ByVal group As cEcoPathGroupInput, Optional ByVal bSendMessage As Boolean = True) As Boolean
        group.AllowValidation = False

        ' Is a multi-stanza group?
        If group.isMultiStanza Then
            ' #Yes: Make Tmax non-editable to the user
            group.SetStatusFlags(eVarNameFlags.TmaxInput, eStatusFlags.NotEditable)
        Else
            ' #No: Make Tmax editable to the user
            group.ClearStatusFlags(eVarNameFlags.TmaxInput, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                    eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        group.AllowValidation = True
    End Function

    Friend Function Set_EconomicAvailable_Flags(ByVal parms As cCoreInputOutputBase, ByVal varname As eVarNameFlags) As Boolean

        Dim bAllowValidationOrg As Boolean = parms.AllowValidation
        Dim bAvailable As Boolean = False
        Dim ds As cEconomicDataSource = cEconomicDataSource.getInstance()

        If (ds IsNot Nothing) Then
            bAvailable = ds.IsDataAvailable(New EwEPlugin.cEcosimRunType)
        End If

        parms.AllowValidation = False
        If bAvailable Then
            parms.ClearStatusFlags(varname, eStatusFlags.NotEditable)
        Else
            parms.SetStatusFlags(varname, eStatusFlags.NotEditable)
            ' JS 19Feb10: disabled flag reset; the plug-in is responsible for handling this
            'parms.SetVariable(varname, False)
        End If
        parms.AllowValidation = bAllowValidationOrg

    End Function

    Private Function Cascade_Name(ByVal strName As String, ByVal obj As cCoreInputOutputBase, ByVal msg As cMessage) As Boolean

        Dim objCascade As cCoreInputOutputBase = Nothing
        Dim bAllowValidationOrg As Boolean = False

        Select Case obj.DataType
            Case eDataTypes.EcoPathGroupInput, eDataTypes.EcoPathGroupOutput, _
                 eDataTypes.EcoSimGroupInput, eDataTypes.EcospaceGroup, eDataTypes.EcotracerGroupInput

                ' Cascase group name to all relevant core IO objects
                objCascade = Me.EcoPathGroupInputs(obj.Index)
                If Not Object.ReferenceEquals(objCascade, obj) Then
                    bAllowValidationOrg = objCascade.AllowValidation
                    objCascade.AllowValidation = False
                    objCascade.Name = strName
                    objCascade.AllowValidation = bAllowValidationOrg

                    msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                End If

                objCascade = Me.EcoPathGroupOutputs(obj.Index)
                If Not Object.ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                    bAllowValidationOrg = objCascade.AllowValidation
                    objCascade.AllowValidation = False
                    objCascade.Name = strName
                    objCascade.AllowValidation = bAllowValidationOrg

                    msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                End If

                If Me.m_StateMonitor.HasEcosimLoaded() Then
                    objCascade = Me.EcoSimGroupInputs(obj.Index)
                    If Not Object.ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg
                    End If
                End If

                If Me.m_StateMonitor.HasEcospaceLoaded() Then
                    objCascade = Me.EcospaceGroups(obj.Index)
                    If Not Object.ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg

                        msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                    End If
                End If

                If Me.m_StateMonitor.HasEcotracerLoaded() Then
                    objCascade = Me.EcotracerGroupInputs(obj.Index)
                    If Not Object.ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg

                        msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                    End If
                End If

            Case eDataTypes.FleetInput, _
                 eDataTypes.EcosimFleetInput, _
                 eDataTypes.EcosimFleetOutput, _
                 eDataTypes.EcospaceFleet, _
                 eDataTypes.MSEFleetInput

                ' Cascase fleet name to all relevant core IO objects
                objCascade = Me.FleetInputs(obj.Index)
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.AllowValidation = False
                objCascade.Name = strName
                objCascade.AllowValidation = bAllowValidationOrg

                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))

                If Me.m_StateMonitor.HasEcosimLoaded() Then
                    objCascade = Me.EcosimFleetInputs(obj.Index)
                    If Not Object.ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg
                    End If
                    objCascade = Me.EcosimFleetOutput(obj.Index)
                    If Not Object.ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg
                    End If
                End If

                If Me.m_StateMonitor.HasEcospaceLoaded() Then
                    objCascade = Me.EcospaceFleets(obj.Index)
                    bAllowValidationOrg = objCascade.AllowValidation
                    objCascade.AllowValidation = False
                    objCascade.Name = strName
                    objCascade.AllowValidation = bAllowValidationOrg

                    msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                End If

        End Select
    End Function

    Private Sub Cascade_PP(ByVal sPP As Single, ByVal obj As cCoreGroupBase, ByVal msg As cMessage)

        Dim objCascade As cCoreGroupBase = Nothing
        Dim bAllowValidationOrg As Boolean = False

        Debug.Assert(obj.DataType = eDataTypes.EcoPathGroupInput)

        If Me.m_StateMonitor.HasEcosimLoaded() Then
            objCascade = Me.EcoSimGroupInputs(obj.Index)
            If objCascade IsNot Nothing Then
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.AllowValidation = True
                objCascade.PP = sPP
                objCascade.ResetStatusFlags()
                objCascade.AllowValidation = bAllowValidationOrg
                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.PP))
            End If
        End If

        If Me.m_StateMonitor.HasEcospaceLoaded() Then
            objCascade = Me.EcospaceGroups(obj.Index)
            If objCascade IsNot Nothing Then
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.AllowValidation = True
                objCascade.PP = sPP
                objCascade.ResetStatusFlags()
                objCascade.AllowValidation = bAllowValidationOrg
                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.PP))
            End If
        End If

    End Sub

    Private Sub Cascade_VBK(ByVal sVBK As Single, ByVal group As cEcoPathGroupInput, ByVal msg As cMessage)

        Dim groupCascade As cEcoPathGroupInput = Nothing
        Dim bAllowValidationOrg As Boolean = False
        Dim iStanza As Integer = Me.getStanzaIndexForGroup(group.Index)

        Debug.Assert(iStanza = group.iStanza)

        ' Is not a stanza life stage?
        If (iStanza < 0) Then Return

        For iGroup As Integer = 1 To Me.nGroups
            groupCascade = Me.EcoPathGroupInputs(iGroup)

            Debug.Assert(Me.getStanzaIndexForGroup(iGroup) = groupCascade.iStanza)

            If (iGroup <> group.Index) And (Me.getStanzaIndexForGroup(iGroup) = iStanza) Then

                bAllowValidationOrg = groupCascade.AllowValidation
                groupCascade.AllowValidation = False
                groupCascade.VBK = sVBK
                groupCascade.ResetStatusFlags()
                groupCascade.AllowValidation = bAllowValidationOrg

                msg.AddVariable(GetAffectedVariableStatus(groupCascade, eVarNameFlags.VBK))
            End If
        Next

    End Sub

    Private Sub Update_Stanza_Catches()

        Dim group As cEcoPathGroupInput = Nothing

        For iGroup As Integer = 1 To Me.nGroups
            group = Me.EcoPathGroupInputs(iGroup)
            If (group.isMultiStanza) Then
                Me.Set_Tcatch_Flags(group, True)
            End If
        Next

    End Sub

    Private Sub Cascade_TCatchInput(ByVal sTCatchInput As Single, ByVal group As cEcoPathGroupInput, ByVal msg As cMessage)

        Dim groupCascade As cEcoPathGroupInput = Nothing
        Dim bAllowValidationOrg As Boolean = False
        Dim iStanza As Integer = Me.getStanzaIndexForGroup(group.Index)
        Dim bIsFished As Boolean

        Debug.Assert(iStanza = group.iStanza)

        ' Is not a stanza life stage?
        If (iStanza < 0) Then Return

        For iGroup As Integer = 1 To Me.nGroups
            bIsFished = False

            groupCascade = Me.EcoPathGroupInputs(iGroup)

            Debug.Assert(Me.getStanzaIndexForGroup(iGroup) = groupCascade.iStanza)

            For iFleet As Integer = 1 To Me.nFleets
                If (Me.m_EcoPathData.Landing(iFleet, iGroup) + _
                    Me.m_EcoPathData.Discard(iFleet, iGroup)) > 0 Then
                    bIsFished = True
                    Exit For
                End If
            Next

            If (iGroup <> group.Index) And (Me.getStanzaIndexForGroup(iGroup) = iStanza) And bIsFished Then

                bAllowValidationOrg = groupCascade.AllowValidation
                groupCascade.AllowValidation = False
                groupCascade.TcatchInput = sTCatchInput
                groupCascade.ResetStatusFlags()
                groupCascade.AllowValidation = bAllowValidationOrg

                msg.AddVariable(GetAffectedVariableStatus(groupCascade, eVarNameFlags.TCatchInput))
            End If
        Next

    End Sub

#End Region ' Status flags updating

#End Region 'EcoPath

#Region " EcoSim "

#Region " Variables "

    Friend m_EcoSim As Ecosim.cEcoSimModel 'the EcoSim Model itself
    'EcoSim parameters that are not meant for public consumption this is the underlying data structures of the EcoSim Model
    'this data is exposed so that it can be serialized.
    'for public access to these parameters see EcoSimGroupOutputs(...) and other access methods.
    Friend m_EcoSimData As cEcosimDatastructures = Nothing
    Friend m_SearchData As cSearchDatastructures

    Private m_EcoSimRun As cEcoSimModelParameters 'private copy of EcoSim model parameters. Public access will through a reference to this object
    Friend m_EcoSimGroups As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoSimGroupInput, 1)
    '   Friend m_EcoSimGroupOuputs As New cCoreInputOutputList(Of cEcosimGroupOutput)(eDataTypes.EcoSimGroupOutput, 1)
    Friend m_EcoSimGroupOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoSimGroupOutput, 1)
    Friend m_EcoSimScenarios As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoSimScenario, 1)
    'Friend m_EcoSimGroupSummaries As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 1)
    Friend m_EcosimFleetInputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcosimFleetInput, 1)
    Friend m_EcosimFleetOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 0)
    Private m_PPIManager As cPPIManager

    Private m_EcopathStats As cEcoPathStats
    Private m_EcosimStats As cEcosimStats
    Private m_EcosimOutputs As cEcosimOutput
    Private m_EcospaceStats As cEcospaceStats

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'MULTI THREADING VARIABLES FOR ECOSIM

    'Delegate for Time-Step notification from the interface 
    Private m_InterfaceDelegate As Ecosim.EcoSimTimeStepDelegate

    Friend m_MSEData As cMSEDataStructures

    ''thread that the EcoSim model is running on
    'Private m_EcoSimThread As System.Threading.Thread

    ''Semaphore object that is used to stop multiple instanse of EcoSim from running at one time
    ''the EcoSim model itself is not thread safe 
    'Private m_EcoSimSemaphor As System.Threading.Semaphore

#End Region ' Variables

    ''' <summary>
    ''' Start biomass of each group
    ''' </summary>
    ''' <remarks>Added by FG temporarily. For the map plotting
    ''' JJ: Please Update it to the correct core class 
    ''' </remarks>
    Public ReadOnly Property StartBiomass(ByVal iGroup As Integer) As Single
        Get
            Return m_EcoSimData.StartBiomass(iGroup)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets an <see cref="cEcoSimScenario">Ecosim scenario</see> from the list of available scenarios.
    ''' </summary>
    ''' <param name="iScenario">One based indexed property of EcoSim Scenarios objects</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcosimScenarios(ByVal iScenario As Integer) As cEcoSimScenario
        Get
            ' JS 06Jul07: list will take care of scenario index/item index offset
            Return DirectCast(m_EcoSimScenarios(iScenario), cEcoSimScenario)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of <see cref="cEcoSimScenario">Ecosim scenarios</see> in the currently loaded model
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcosimScenarioCount() As Integer
        Get
            Try
                Return Me.m_EcoPathData.NumEcosimScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the index of the active <see cref="cEcosimScenario">Ecosim scenario</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveEcosimScenarioIndex() As Integer
        Get
            Return Me.m_EcoPathData.ActiveEcosimScenario
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the start year for Ecosim
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function EcosimFirstYear() As Integer
        ' Has TS?
        If (Me.ActiveTimeSeriesDatasetIndex > 0) Then
            ' #Yes: Return first year of active TS dataset
            Return Me.TimeSeriesDataset(Me.ActiveTimeSeriesDatasetIndex).FirstYear
        Else
            ' #No: no time reference
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Statistics from the last Ecosim model run
    ''' </summary>
    Public ReadOnly Property EcosimStats() As cEcosimStats
        Get
            Try
                Return Me.m_EcosimStats
            Catch ex As Exception
                Debug.Assert(False, "EcosimStats")
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' Get outputs of the last Ecosim run
    ''' </summary>
    Public ReadOnly Property EcosimOutputs() As cEcosimOutput
        Get
            Return Me.m_EcosimOutputs
        End Get
    End Property

    ''' <summary>
    ''' Normalize MSE QuotaShare values
    ''' </summary>
    Public Sub NormalizeQuotaShare()
        ' Sanity check
        Debug.Assert(Me.StateMonitor.HasEcosimLoaded())
        ' Normalize MSE quota share values
        Me.MSEManager.SumQuotaShareToOne()
        Me.m_StateMonitor.SetEcoSimLoaded(True)
        ' Send out data changed message for MSE
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.EcoSim, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag datasource as dirty
        Me.DataSource.SetChanged(eCoreComponentType.MSE)
        Me.m_StateMonitor.UpdateDataState(DataSource)
    End Sub

    ''' <summary>
    ''' Set MSE QuotaShare values to default
    ''' </summary>
    Public Sub SetDefaultQuotaShare()
        ' Sanity check
        Debug.Assert(Me.StateMonitor.HasEcosimLoaded())
        ' Set default MSE quota share values
        Me.MSEManager.SetDefaultQuotaShare()
        Me.m_StateMonitor.SetEcoSimLoaded(True)
        ' Send out data changed message for MSE
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.EcoSim, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag datasource as dirty
        Me.DataSource.SetChanged(eCoreComponentType.MSE)
        Me.m_StateMonitor.UpdateDataState(DataSource)
    End Sub


    ''' <summary>
    ''' Set MSE QuotaShare values to default
    ''' </summary>
    Public Sub SetDefaultMSERecruitment()
        ' Sanity check
        Debug.Assert(Me.StateMonitor.HasEcosimLoaded())
        Try

            ' Set default MSE quota share values
            Me.MSEManager.SetDefaultRecruitment()
            Me.m_StateMonitor.SetEcoSimLoaded(True)
            ' Send out data changed message for MSE
            Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.EcoSim, eMessageType.DataModified))
            Me.m_publisher.sendAllMessages()
            ' Flag datasource as dirty
            Me.DataSource.SetChanged(eCoreComponentType.MSE)
            Me.m_StateMonitor.UpdateDataState(DataSource)

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            'ToDo send a message....
            'Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.EcoSim, eMessageType.DataModified))
            'Me.m_publisher.sendAllMessages()
        End Try

    End Sub

    Public Sub ResetMSEGroupRefLevels()
        ' Sanity check
        Debug.Assert(Me.StateMonitor.HasEcosimLoaded())
        ' Set default MSE quota share values
        Me.MSEManager.SetDefaultGroupRefLevels()
        Me.m_StateMonitor.SetEcoSimLoaded(True)
        ' Send out data changed message for MSE
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.EcoSim, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag datasource as dirty
        Me.DataSource.SetChanged(eCoreComponentType.MSE)
        Me.m_StateMonitor.UpdateDataState(DataSource)
    End Sub

    ''' <summary>
    ''' Initialize the EcoSim model
    ''' </summary>
    ''' <returns>True is successfull. False otherwise</returns>
    ''' <remarks></remarks>
    Private Function InitEcoSim() As Boolean

        Try

            m_bEcoSimIsInit = False

            'has the core been initialized
            'If Not m_bCoreIsInit Then
            '    'ToDo_jb InitEcoSim() failed send a message ??????
            '    Debug.Assert(False, "Core has not been initialized.")
            '    Return False
            'End If

            m_EcoSim = New Ecosim.cEcoSimModel(Me.EcoFunction)

            m_EcoSim.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.EcosimMessageHandler, eCoreComponentType.EcoSim, eMessageType.Any, Me.m_SyncObj))

            'set the output variables from EcoPath as the Input for EcoSim
            'this sets the baseline state for EcoSim as the last run EcoPath model
            m_EcoSim.EcopathData = Me.m_EcoPathData
            m_EcoSim.m_Data = Me.m_EcoSimData
            m_EcoSim.m_stanza = Me.m_Stanza
            m_EcoSim.TracerData = Me.m_tracerData
            m_EcoSim.TimeSeriesData = Me.m_TSData
            m_EcoSim.MSEData = Me.m_MSEData

            Me.m_EcosimOutputs = New cEcosimOutput(Me)

            'Build all the shape managers
            m_ShapeManagers = New Dictionary(Of eDataTypes, cBaseShapeManager)
            Dim manager As cBaseShapeManager

            manager = New cForcingFunctionManager(m_EcoSimData, Me, eDataTypes.Forcing)
            m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cMediationManager(m_EcoSimData, Me, eDataTypes.Mediation)
            m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cEggProductionManager(m_EcoSimData, Me, eDataTypes.EggProd)
            m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cFishingEffortManger(m_EcoSimData, Me, eDataTypes.FishingEffort)
            m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cFishingMortalityManger(m_EcoSimData, Me, eDataTypes.FishMort)
            m_ShapeManagers.Add(manager.DataType, manager)

            m_PPIManager = New cPPIManager(m_EcoPathData, m_EcoSimData, Me)
            m_FitToTimeSeriesData = New cF2TSDataStructures()
            ' m_FitToTimeSeries = New cF2TSManager(Me)

            m_bEcoSimIsInit = True

            ' Set core state
            Me.m_StateMonitor.SetEcoSimLoaded(False)

            Return True

        Catch ex As Exception
            cLog.Write(Me.ToString & ".InitEcoSim(...) Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return False
        End Try


    End Function

    Private Sub EcosimMessageHandler(ByRef Message As cMessage)
        m_publisher.AddMessage(Message)
    End Sub

    Private Function InitEcosimScenarios() As Boolean
        Me.m_EcoSimScenarios.Clear()
        For i As Integer = 1 To Me.m_EcoPathData.EcosimScenarioName.Length - 1
            Me.m_EcoSimScenarios.Add(Me.privateEcoSimScenario(i))
        Next
        Return True
    End Function

    ''' <summary>
    ''' Load an <see cref="cEcoSimScenario">Ecosim scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcoSimScenario">Scenario</see> to load.</param>
    ''' <returns>True if succesful.</returns>
    Public Function LoadEcosimScenario(ByVal scenario As cEcoSimScenario) As Boolean
        Return LoadEcosimScenario(scenario.Index)
    End Function

    Private Sub SendEcosimLoadStateMessage(ByVal strScenarioName As String, Optional ByVal strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = String.Format(My.Resources.CoreMessages.ECOSIM_LOAD_SUCCESS, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSim, eMessageImportance.Information)
        Else
            strText = String.Format(My.Resources.CoreMessages.ECOSIM_LOAD_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()
    End Sub

    Private Sub SendEcosimSaveStateMessage(ByVal strScenarioName As String, Optional ByVal bSucces As Boolean = True, _
            Optional ByVal strError As String = "")

        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If bSucces Then
            strText = String.Format(My.Resources.CoreMessages.ECOSIM_SAVE_SUCCESS, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Information)
        Else
            strText = String.Format(My.Resources.CoreMessages.ECOSIM_SAVE_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()
    End Sub

    ''' <summary>
    ''' Creates and loads a new Ecosim scenario.
    ''' </summary>
    ''' <param name="strName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <returns>True if succesful.</returns>
    Public Function NewEcosimScenario(ByVal strName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String) As Boolean

        Dim ds As IEcosimDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim iScenario As Integer = 0

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcosimDatasource Then Return False

        If Me.m_StateMonitor.HasEcopathLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        Try

            ds = DirectCast(DataSource, IEcosimDatasource)
            If (ds.AppendEcosimScenario(strName, strDescription, strAuthor, strContact, iScenarioID)) Then

                Me.StateMonitor.UpdateDataState(Me.m_DataSource)
                Me.InitEcosimScenarios()
                DataAddedOrRemovedMessage("Ecosim number of scenarios has changed.", eCoreComponentType.EcoSim, eDataTypes.EcoSimScenario)
                iScenario = Array.IndexOf(Me.m_EcoPathData.EcosimScenarioDBID, iScenarioID)
                Return Me.LoadEcosimScenario(iScenario)

            End If

            Return False
        Catch ex As Exception

        End Try
        Return False

    End Function

    ''' <summary>
    ''' Load an <see cref="cEcoSimScenario">Ecosim scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the <see cref="cEcoSimScenario">Scenario</see> in the <see cref="m_EcoSimScenarios">Scenario list</see>.</param>
    ''' <returns>True if succesful.</returns>
    Public Function LoadEcosimScenario(ByVal iScenario As Integer) As Boolean

        Dim ds As IEcosimDatasource = Nothing
        Dim strScenarioName As String = Me.m_EcoPathData.EcosimScenarioName(iScenario)

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcosimDatasource Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        Try

            ' Update core state
            Me.m_StateMonitor.SetEcoSimLoaded(False)
            Me.m_EcoPathData.ActiveEcosimScenario = -1
            Me.m_EcoPathData.ActiveEcospaceScenario = -1
            Me.m_EcoPathData.ActiveEcotracerScenario = -1

            If Not m_bEcoSimIsInit Then
                Debug.Assert(False, "Failed to LoadScenario(). EcoSim must be initialized first.")
                ' User cannot do anything about this error; the core should never be here. No need to send a message.
                'Me.SendEcosimLoadStateMessage(strScenarioName, "EcoSim is not initialized yet.")
                Return False
            End If

            'this could happen by calling LoadScenario() with an integer value without having loaded a model
            If Me.m_StateMonitor.HasEcopathLoaded() = False Then
                Debug.Assert(False, "Failed to LoadScenario(). A model must be loaded first.")
                ' User cannot do anything about this error; the core should never be here. No need to send a message.
                ' Me.SendEcosimLoadStateMessage(CStr(iScenario), "An Ecopath model must be loaded first.")
                Return False
            End If

            If Me.m_StateMonitor.HasEcopathRan() = False Then
                'EcoPath will handle it's own messages if it fails
                If Not RunEcoPath() Then
                    'Debug.Assert(False, Me.ToString & ".RunEcoSim() Failed to Run EcoPath.")
                    cLog.Write(Me.ToString & ".RunEcoSim() Failed to Run EcoPath.")
                    Me.SendEcosimLoadStateMessage(strScenarioName, My.Resources.CoreMessages.ECOPATH_RUN_ERROR)
                    Return False
                End If
            End If

            'things that need to happen before a scenario is loaded
            m_EcoSim.SearchData = m_SearchData
            m_EcoSim.SetCounters()
            m_EcoSim.InitStanza()
            m_EcoSim.SetDefaultParameters()

            m_TSData.ClearTimeSeries()


            'jb I still need to deal with how to handle these problems
            ds = DirectCast(DataSource, IEcosimDatasource)
            If Not ds.LoadEcosimScenario(Me.m_EcoPathData.EcosimScenarioDBID(iScenario)) Then
                Debug.Assert(False, "LoadEcosimScenario() Failed to load scenario from data source.")
                Me.SendEcosimLoadStateMessage(strScenarioName, "Failed to read the database")
                Return False
            End If

            m_SearchData.redimTime(Me.nEcosimYears)

            'set the default summary time periods
            m_EcoSimData.DefaultSummaryPeriods()

            'Init PropLandedTime() and Propdiscardtime() to Ecopath values so they can be used during the initialization of Ecosim
            Me.m_MSEData.InitToEcopath()
            m_EcoSim.Init(True)

            InitEcosimGroups()
            InitEcosimFleetInput()
            initEcoSimModelParameters()

            'rebuild all the shapes in the shape managers
            For Each manager As cBaseShapeManager In m_ShapeManagers.Values
                manager.Init() 'init will rebuild and load all the shapes in the manager
            Next

            m_PPIManager.Init()
            m_PPIManager.Load()

            InitEcosimGroupOutput()
            InitEcosimFleetOutput()

            InitEcosimTimeSeries()
            LoadEcosimTimeSeries()

            ' Reload stanzas
            Me.LoadStanzas()

            Me.m_EcosimStats = New cEcosimStats(Me)

            'init the monte carlo model with the newly loaded data
            Me.m_MonteCarlo.init(Me)

            'search manager Init and Load
            'SearchObjective base, Fishing policy, MSE and Ecoseed
            For Each search As ISearchObjective In Me.m_SearchManagers.Values
                search.Init(Me) 'init will rebuild all the interface objects
                search.Load() 'populate the interface objects
            Next

            ' Me.m_EcoSim.InitAssessment()

            ' Update economic data state for Ecosim objects
            Me.OnEconomicDataPluginEnabled()

            ' Let's send out at least one message
            Me.SendEcosimLoadStateMessage(strScenarioName)

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.LoadEcosimScenario(ds)
                Me.PluginManager.EcosimInitialized(m_EcoSimData)
            End If

            ' Update core state
            Me.m_StateMonitor.SetEcoSimLoaded(True)

            ' JS12May10: EcosimLoaded implies EcosimInitialized
            'Me.m_StateMonitor.SetEcoSimInitialized()

            Return True

        Catch ex As Exception
            cLog.Write(Me.ToString & ".LoadEcosimScenario(...) Error: " & ex.Message)
            Me.SendEcosimLoadStateMessage(strScenarioName, ex.Message)
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the current Ecosim scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcosimScenario() As Boolean

        Dim iScenarioID As Integer = 0
        Dim ds As IEcosimDatasource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcosimDatasource Then Return False

        ' Overwrite scenario?
        iScenarioID = Me.m_EcoPathData.EcosimScenarioDBID(Me.m_EcoPathData.ActiveEcosimScenario)
        Debug.Assert(iScenarioID > 0)

        ds = DirectCast(DataSource, IEcosimDatasource)
        ' No need to save? Yippee
        If Not ds.IsEcosimModified Then Return True
        ' Save ok?
        If (ds.SaveEcosimScenario(iScenarioID)) Then
            ' Reload ecosim scenarios
            Me.InitEcosimScenarios()
            ' Update active scenario ID
            Me.m_EcoPathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcoPathData.EcosimScenarioDBID, iScenarioID)
            ' #Yes: invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcosimScenario(Me)
            ' Force update
            Me.m_StateMonitor.SetEcoSimLoaded(True, TriState.True)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Report succes
            Me.SendEcosimSaveStateMessage(Me.m_EcoPathData.EcosimScenarioName(Me.m_EcoPathData.ActiveEcosimScenario))
            Return True
        End If

        ' Restore active scenario ID
        Me.m_EcoPathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcoPathData.EcosimScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcosimSaveStateMessage(Me.m_EcoPathData.EcosimScenarioName(Me.m_EcoPathData.ActiveEcosimScenario), False, _
                My.Resources.CoreMessages.GENERIC_SAVE_RESOLUTION)

        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the current ecosim scenario under a new name.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' This will adjust the active scenario index!
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcosimScenarioAs(ByVal strName As String, ByVal strDescription As String) As Boolean

        Dim epd As cEcopathDataStructures = Me.m_EcoPathData
        Dim iScenarioID As Integer = 0
        Dim ds As IEcosimDatasource = Nothing
        Dim bSucces As Boolean = False

        ' Sanity checks
        If (DataSource Is Nothing) Then Return bSucces
        If (Not TypeOf (DataSource) Is IEcosimDatasource) Then Return bSucces
        If (Me.ActiveEcosimScenarioIndex <= 0) Then Return bSucces

        ' Is this the current scenario?
        If String.Compare(Me.EcosimScenarios(Me.ActiveEcosimScenarioIndex).Name, strName, True) = 0 Then
            ' #Yes: save directly instead
            Return Me.SaveEcosimScenario()
        End If

        ds = DirectCast(DataSource, IEcosimDatasource)
        ' Save ok?
        If ds.SaveEcosimScenarioAs(strName, strDescription, _
                epd.EcosimScenarioAuthor(Me.m_EcoPathData.ActiveEcosimScenario), _
                epd.EcosimScenarioContact(Me.m_EcoPathData.ActiveEcosimScenario), _
                iScenarioID) Then

            ' #Yes: invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcosimScenario(Me)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)

            ' Reload scenarios
            Me.InitEcosimScenarios()

            ' Inform the world
            Me.SendEcosimSaveStateMessage(strName)

            ' Load new Ecosim scenario to refresh all IO objects
            bSucces = Me.LoadEcosimScenario(Array.IndexOf(Me.m_EcoPathData.EcosimScenarioDBID, iScenarioID))
            ' Changed no. of scenarios
            Me.DataAddedOrRemovedMessage("Ecosim number of scenarios has changed.", eCoreComponentType.EcoSim, eDataTypes.EcoSimScenario)
            ' Report succes
            Return bSucces
        End If

        ' Restore active scenario ID
        Me.m_EcoPathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcoPathData.EcosimScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcosimSaveStateMessage(strName, bSucces)
        Return bSucces

    End Function

    ''' <summary>
    ''' Remove a <see cref="cEcoSimScenario">Ecosim Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcoSimScenario">Scenario</see> to remove.</param>
    ''' <returns>True if succesful.</returns>
    Public Function RemoveEcosimScenario(ByVal scenario As cEcoSimScenario) As Boolean
        Return Me.RemoveEcosimScenario(scenario.Index)
    End Function

    ''' <summary>
    ''' Remove a <see cref="cEcoSimScenario">Ecosim Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario in the <see cref="m_EcoSimScenarios">Scenario list</see>.</param>
    ''' <returns>True if succesful.</returns>
    Public Function RemoveEcosimScenario(ByVal iScenario As Integer) As Boolean

        Dim ds As IEcosimDatasource = Nothing
        Dim iScenarioID As Integer = cCore.NULL_VALUE ' Scenario to restore
        Dim bSucces As Boolean = False

        ' Sanity check
        Debug.Assert(iScenario > 0 And iScenario < Me.m_EcoPathData.EcosimScenarioDBID.Length)

        If (iScenario = Me.m_EcoPathData.ActiveEcosimScenario) Then
            Me.m_publisher.SendMessage(New cMessage("Cannot delete a loaded scenario", eMessageType.NotSet, eCoreComponentType.DataSource, eMessageImportance.Warning))
            Return False
        End If

        ' Save pending changes
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcosimDatasource) Then Return False

        ' Remember scenario ID to restore
        If (Me.m_EcoPathData.ActiveEcosimScenario > 0) Then
            iScenarioID = Me.m_EcoPathData.EcosimScenarioDBID(Me.m_EcoPathData.ActiveEcosimScenario)
        End If

        ds = DirectCast(Me.DataSource, IEcosimDatasource)
        ' Scenario removed succesfully?
        If ds.RemoveEcosimScenario(Me.m_EcoPathData.EcosimScenarioDBID(iScenario)) Then
            ' #Yes: reload scenario list
            bSucces = Me.InitEcosimScenarios()
            ' Restore active scenario ID
            Me.m_EcoPathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcoPathData.EcosimScenarioDBID, iScenarioID)
            ' Broadcast change
            Me.DataAddedOrRemovedMessage("Ecosim number of scenarios has changed.", eCoreComponentType.EcoSim, eDataTypes.EcoSimScenario)
        End If

        ' Return succes
        Return bSucces

    End Function

    ''' <summary>
    ''' Update the list of available ecosim input groups
    ''' </summary>
    Private Function InitEcosimGroups() As Boolean

        m_EcoSimGroups.Clear()

        'populate the list of cEcoSimGroupInfo objects that the user will interact with 
        'to change group related parameters from the interface see getEcoSimGroupInfo(iGroup)
        For i As Integer = 1 To nGroups
            m_EcoSimGroups.Add(New cEcoSimGroupInput(Me, m_EcoSimData.GroupDBID(i)))
        Next i

        'now load the Ecosim data into the objects created above
        LoadEcosimGroups()

    End Function

    Friend Function LoadEcosimGroups() As Boolean
        Dim iGroup As Integer
        Dim iPred As Integer

        For Each group As cEcoSimGroupInput In m_EcoSimGroups

            'convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcoSimData.GroupDBID, group.DBID)

            'this will only resize the arrays if NumGroups is different then the existing array size
            group.Resize()

            group.AllowValidation = False

            group.Index = iGroup

            'get the group name from EcoPath not EcoSim
            group.Name = m_EcoPathData.GroupName(iGroup)
            'Primary Production also comes from EcoPath
            group.PP = m_EcoPathData.PP(iGroup)

            group.MaxRelPB = m_EcoSimData.PBmaxs(iGroup)
            group.MaxRelFeedingTime = m_EcoSimData.FtimeMax(iGroup)
            group.FeedingTimeAdjustRate = m_EcoSimData.FtimeAdjust(iGroup)
            group.OtherMortFeedingTime = m_EcoSimData.MoPred(iGroup)
            group.PredEffectFeedingTime = m_EcoSimData.RiskTime(iGroup)
            group.DenDepCatchability = m_EcoSimData.QmQo(iGroup)
            group.QBMaxQBio = m_EcoSimData.CmCo(iGroup)
            group.SwitchingPower = m_EcoSimData.SwitchPower(iGroup)
            group.SalinityOpt = m_EcoSimData.SalOpt(iGroup)
            group.SalinitySpreadLeft = m_EcoSimData.SdSalLeft(iGroup)
            group.SalinitySpreadRight = m_EcoSimData.SdSalRight(iGroup)
            group.TemperatureOpt = m_EcoSimData.TempOpt(iGroup)
            group.TemperatureSpreadLeft = m_EcoSimData.TempLeft(iGroup)
            group.TemperatureSpreadRight = m_EcoSimData.TempRight(iGroup)

            Try
                For iPred = 1 To nGroups

                    group.VulMult(iPred) = m_EcoSimData.VulMult(iGroup, iPred)

                    If m_EcoSimData.SimDC(iPred, iGroup) > 0 Or (iGroup = iPred And m_EcoPathData.PP(iPred) = 1) Then
                        group.VulMultiStatus(iPred) = eStatusFlags.OK
                        group.VulRateStatus(iPred) = eStatusFlags.OK
                    Else
                        group.VulMultiStatus(iPred) = eStatusFlags.NotEditable Or eStatusFlags.Null
                        group.VulRateStatus(iPred) = eStatusFlags.NotEditable Or eStatusFlags.Null
                    End If

                Next
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

            group.iStanza = getStanzaIndexForGroup(iGroup)

            group.ResetStatusFlags()

            group.AllowValidation = True

        Next

    End Function

    ''' <summary>
    ''' Get a <see cref="cEcoSimGroupInput">Ecosim input group</see> for a given group index.
    ''' </summary>
    ''' <param name="iGroup">The index to obtain the group for.</param>
    Public ReadOnly Property EcoSimGroupInputs(ByVal iGroup As Integer) As cEcoSimGroupInput
        Get
            'test that EcoSim has been initialized
            If Not m_bEcoSimIsInit Then
                Debug.Assert(False, "EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
                cLog.Write("EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
                Return Nothing
            End If
            ' JS 06Jul07: list will take care of scenario index/item index offset
            Return DirectCast(m_EcoSimGroups(iGroup), cEcoSimGroupInput)
        End Get
    End Property

    ''' <summary>
    ''' Get a <see cref="cEcoSimGroupOutput">Ecosim output group</see> for a given group index.
    ''' </summary>
    ''' <param name="iGroup">The index to obtain the group for.</param>
    Public ReadOnly Property EcoSimGroupOutputs(ByVal iGroup As Integer) As cEcosimGroupOutput
        Get

            Try
                If m_EcoSimGroupOutputs IsNot Nothing Then
                    If m_EcoSimGroupOutputs.Count > 0 Then
                        Return DirectCast(m_EcoSimGroupOutputs(iGroup), cEcosimGroupOutput)
                    End If
                End If
                Return Nothing
            Catch ex As Exception
                cLog.Write(ex)
                Return Nothing
            End Try
        End Get
    End Property

    Private Sub InitEcosimFleetInput()
        Try

            Me.m_EcosimFleetInputs.Clear()

            For iflt As Integer = 1 To nFleets
                Me.m_EcosimFleetInputs.Add(New cEcosimFleetInput(Me, iflt))
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcosimFleetInput() Error: " & ex.Message)
        End Try

        LoadEcosimFleetInputs()

    End Sub

    Private Sub InitEcosimFleetOutput()
        Try

            Me.m_EcosimFleetOutputs.Clear()

            'this includes zero index 'Combined Fleets' 
            For iflt As Integer = 0 To nFleets
                Me.m_EcosimFleetOutputs.Add(New cEcosimFleetOutput(Me, iflt))
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcosimFleetOutput() Error: " & ex.Message)
        End Try
    End Sub

    Private Function LoadEcosimFleetOutputs() As Boolean
        Dim iFlt As Integer
        Dim sCatch As Single, EndCatch As Single
        Dim sVal As Single, endVal As Single

        'if Ecosim has not run the results data will not be dimensioned so do not try to load it
        If m_EcoSimData.ResultsOverTime Is Nothing Then
            'HACK WARNING
            'this should use the state monitor however there is a problem with that
            'if the user edits cEcosimModelParameters.StartSummaryTime the statemonitor will flag ecosim as needing to run which it does not
            Exit Function
        End If

        Try
            For Each fleet As cEcosimFleetOutput In m_EcosimFleetOutputs
                fleet.Resize()

                fleet.AllowValidation = False

                'the group index was passed into the constructor
                iFlt = fleet.Index

                If iFlt = 0 Then
                    fleet.Name = My.Resources.CoreDefaults.CORE_ALL_FLEETS
                Else
                    fleet.Name = m_EcoPathData.FleetName(iFlt)
                End If

                m_EcoSimData.getSummaryBioOfCatch(iFlt, sCatch, EndCatch)
                fleet.CatchStart = sCatch
                fleet.CatchEnd = EndCatch

                m_EcoSimData.getSummaryValueOfCatch(iFlt, sVal, endVal)
                fleet.ValueStart = sVal
                fleet.ValueEnd = endVal

                'see EwE5 CalculateSimSpaceResults
                m_EcoSimData.getSummaryCostByCatch(iFlt, sVal, endVal)
                fleet.CostStart = sVal * (m_EcoPathData.cost(iFlt, eCostIndex.CUPE) + m_EcoPathData.cost(iFlt, eCostIndex.Sail)) + m_EcoPathData.cost(iFlt, eCostIndex.Fixed)
                fleet.CostEnd = endVal * (m_EcoPathData.cost(iFlt, eCostIndex.CUPE) + m_EcoPathData.cost(iFlt, eCostIndex.Sail)) + m_EcoPathData.cost(iFlt, eCostIndex.Fixed)

                'If there is forced catches for any group caught by this fleet then Cost is not valid.
                'Cost is calculated from Ecopath input values and Effort Not Catch. 
                'If catch is forced we have no way of knowing what effort created the catch so no cost.
                Dim bCatchTS As Boolean = False
                For igrp As Integer = 1 To Me.nGroups
                    If Me.m_EcoPathData.Landing(iFlt, igrp) > 0 Then
                        If Me.m_TSData.DataLoadedForTypeGroup(eTimeSeriesType.CatchesForcing, igrp) Then
                            'cost is never stored in the core so we can only set the values in the interface
                            fleet.CostStart = cCore.NULL_VALUE
                            fleet.CostEnd = cCore.NULL_VALUE
                            bCatchTS = True
                        End If
                    End If
                    If bCatchTS Then Exit For
                Next

                fleet.Effort = 0.0F
                If sVal <> 0 Then
                    fleet.Effort = endVal / sVal
                End If

                'get Economic data from the data adapter
                'this economic data could come Ecosim or any Plugin that supplies economic data e.g. ECost
                fleet.ProfitSummary = Me.m_EcoSimData.ProfitByFleet(iFlt)
                fleet.JobsSummary = Me.m_EcoSimData.EmploymentValueByFleet(iFlt)
                fleet.Init()

            Next

            Return True

        Catch ex As Exception
            cLog.Write(ex)
            m_publisher.AddMessage(New cMessage("Error loading Ecosim Summary data. " & ex.Message, eMessageType.ErrorEncountered, _
                                    eCoreComponentType.EcoSim, eMessageImportance.Critical))
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Friend Sub LoadEcosimStats()
        Try
            Me.m_EcosimStats.SS = m_EcoSimData.SS
            For igrp As Integer = 1 To Me.nGroups
                Me.m_EcosimStats.SSGroup(igrp) = m_EcoSimData.SSGroup(igrp)
            Next
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ArgumentException(Me.ToString & ".LoadEcosimStats() Error: " & ex.Message, ex)
        End Try
    End Sub

    Friend Sub LoadEcosimOutputs()
        Try
            ' Hah, nothing to do here
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ArgumentException(Me.ToString & ".LoadEcosimOutputs() Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Function InitEcosimGroupOutput() As Boolean

        m_EcoSimGroupOutputs.Clear()

        'populate the list of cEcoSimGroupInfo objects that the user will interact with 
        'to change group related parameters from the interface see EcosimGroupOutputs(iGroup)
        For i As Integer = 1 To nGroups
            m_EcoSimGroupOutputs.Add(New cEcosimGroupOutput(Me, Me.m_EcoSimData, i))
        Next i

    End Function

    ''' <summary>
    ''' Clear all status flags on Ecosim group outputs
    ''' </summary>
    Private Sub ResetEcosimGroupOutputs()
        For Each group As cEcosimGroupOutput In Me.m_EcoSimGroupOutputs
            group.ResetStatusFlags(True)
        Next
    End Sub

    Private Function LoadEcosimGroupOutputs() As Boolean
        Dim iGroup As Integer
        Dim sBio As Single, EndBio As Single, sCatch As Single, EndCatch As Single
        Dim sVal As Single, endVal As Single


        For Each group As cEcosimGroupOutput In m_EcoSimGroupOutputs

            'reset the reference to the sim results arrays
            group.Init()

            'this will only resize the arrays if NumGroups is different then the existing array size
            group.Resize()

            group.AllowValidation = False

            'the group index was passed into the constructor
            iGroup = group.Index

            'get the group name from EcoPath not EcoSim
            group.Name = m_EcoPathData.GroupName(iGroup)

            'stanza variables setting the stanza id will also set the isMultiStanza Flag
            group.iStanza = getStanzaIndexForGroup(iGroup)
            group.PP = m_EcoPathData.PP(iGroup)

            'Biomass
            m_EcoSimData.getSummaryBioForGroup(iGroup, sBio, EndBio)
            group.BiomassStart = sBio
            group.BiomassEnd = EndBio

            'catch by group
            For iFlt As Integer = 0 To nFleets 'Zero is the combined fleets 
                m_EcoSimData.getSummaryCatchByGroup(iGroup, iFlt, sCatch, EndCatch)
                group.CatchStart(iFlt) = sCatch
                group.CatchEnd(iFlt) = EndCatch

                m_EcoSimData.getSummaryValueByGroup(iGroup, iFlt, sVal, endVal)
                group.ValueStart(iFlt) = sVal
                group.ValueEnd(iFlt) = endVal
            Next

            For i As Integer = 1 To nGroups

                group.isPrey(i) = False
                'is this i prey for this output group
                If m_EcoPathData.DC(iGroup, i) > 0 Then
                    group.isPrey(i) = True
                End If

                group.isPred(i) = False
                'is this i a predator of this output group
                If m_EcoPathData.DC(i, iGroup) > 0 Then
                    group.isPred(i) = True
                End If

            Next

            group.ResetStatusFlags()

        Next

    End Function



    Public ReadOnly Property PPInteractionManager() As cPPIManager

        Get
            Return Me.m_PPIManager
        End Get

    End Property


    Public ReadOnly Property ForcingShapeManager() As cForcingFunctionManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.Forcing), cForcingFunctionManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find Shape Manager")
                cLog.Write(Me.ToString & ".ForcingShapeManager() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property EggProdShapeManager() As cEggProductionManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.EggProd), cEggProductionManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find Shape Manager")
                cLog.Write(Me.ToString & ".EggProdShapeManager() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property MediationShapeManager() As cMediationManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.Mediation), cMediationManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find Shape Manager")
                cLog.Write(Me.ToString & ".MediationShapeManager() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property FishingEffortShapeManager() As cFishingEffortManger

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.FishingEffort), cFishingEffortManger)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find effort shape manager")
                cLog.Write(Me.ToString & ".FishingEffortShapeManager() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property FishMortShapeManager() As cFishingMortalityManger

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.FishMort), cFishingMortalityManger)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find mortality shape manager")
                cLog.Write(Me.ToString & ".FishMortShapeManager() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    ''' <summary>
    ''' Update all the underlying data structures that contain EcoSim scenario data
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    Private Function UpdateEcoSimScenario(ByVal iDBID As Integer) As Boolean

        Dim iScenario As Integer = Array.IndexOf(Me.m_EcoPathData.EcosimScenarioDBID, iDBID)
        Dim scn As cEcoSimScenario = Me.EcosimScenarios(iScenario)

        Try
            Me.m_EcoPathData.EcosimScenarioName(iScenario) = scn.Name
            Me.m_EcoPathData.EcosimScenarioDescription(iScenario) = scn.Description
            Me.m_EcoPathData.EcosimScenarioAuthor(iScenario) = scn.Author
            Me.m_EcoPathData.EcosimScenarioContact(iScenario) = scn.Contact
            ' Do not update last saved date; this is exclusively set by the core when saving
            'Me.m_EcoPathData.EcosimScenarioLastSaved(iScenario) = scn.LastSaved

        Catch ex As Exception
            cLog.Write(Me.ToString & ".UpdateEcoSimScenario() Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Update all the underlying data structures that contain group info for EcoSim
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function UpdateEcoSimGroup(ByVal iDBID As Integer) As Boolean

        Dim iGroup As Integer = Array.IndexOf(m_EcoSimData.GroupDBID, iDBID)
        Dim group As cEcoSimGroupInput = Me.EcoSimGroupInputs(iGroup)

        Try
            m_EcoSimData.QmQo(iGroup) = group.DenDepCatchability
            m_EcoSimData.FtimeAdjust(iGroup) = group.FeedingTimeAdjustRate
            m_EcoSimData.FtimeMax(iGroup) = group.MaxRelFeedingTime
            m_EcoSimData.PBmaxs(iGroup) = group.MaxRelPB
            m_EcoSimData.MoPred(iGroup) = group.OtherMortFeedingTime
            m_EcoSimData.RiskTime(iGroup) = group.PredEffectFeedingTime
            m_EcoSimData.CmCo(iGroup) = group.QBMaxQBio
            m_EcoSimData.SwitchPower(iGroup) = group.SwitchingPower
            m_EcoSimData.SdSalLeft(iGroup) = group.SalinitySpreadLeft
            m_EcoSimData.SdSalRight(iGroup) = group.SalinitySpreadRight
            m_EcoSimData.SalOpt(iGroup) = group.SalinityOpt
            m_EcoSimData.TempOpt(iGroup) = group.TemperatureOpt
            m_EcoSimData.TempLeft(iGroup) = group.TemperatureSpreadLeft
            m_EcoSimData.TempRight(iGroup) = group.TemperatureSpreadRight

            For iPred As Integer = 1 To nGroups
                ' m_EcoSimData.vulrate(iGroup, i) = grp.VulRate(i)
                m_EcoSimData.VulMult(iGroup, iPred) = group.VulMult(iPred)
            Next

        Catch ex As Exception
            cLog.Write(Me.ToString & ".updateEcoSimGroupInfo() Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Update all the underlying data structures that contain fleet input info for EcoSim
    ''' </summary>
    Private Function UpdateEcoSimFleetInput(ByVal iDBID As Integer) As Boolean

        Dim iFleet As Integer = Array.IndexOf(m_EcoPathData.FleetDBID, iDBID)
        Dim fleet As cEcosimFleetInput = Me.EcosimFleetInputs(iFleet)

        Try
            Me.m_EcoSimData.Epower(iFleet) = fleet.EPower
            Me.m_EcoSimData.PcapBase(iFleet) = fleet.PcapBase
            Me.m_EcoSimData.CapBaseGrowth(iFleet) = fleet.CapBaseGrowth
            Me.m_EcoSimData.CapDepreciate(iFleet) = fleet.CapDepreciateRate

        Catch ex As Exception
            cLog.Write(Me.ToString & ".updateEcoSimGroupInfo() Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function LoadEcosimFleetInputs() As Boolean

        Dim iFleet As Integer

        Try

            For Each fleet As cEcosimFleetInput In m_EcosimFleetInputs

                fleet.AllowValidation = False

                iFleet = Array.IndexOf(m_EcoSimData.FleetDBID, fleet.DBID)
                Debug.Assert(iFleet > 0 And iFleet <= m_EcoPathData.NumFleet, "Failed to find Fleet index for database ID " & fleet.DBID.ToString)

                fleet.Name = Me.m_EcoPathData.FleetName(iFleet)
                fleet.EPower = m_EcoSimData.Epower(iFleet)
                fleet.PcapBase = m_EcoSimData.PcapBase(iFleet)
                fleet.CapDepreciateRate = m_EcoSimData.CapDepreciate(iFleet)
                fleet.CapBaseGrowth = m_EcoSimData.CapBaseGrowth(iFleet)

                fleet.ResetStatusFlags()
                fleet.AllowValidation = True
            Next

        Catch ex As Exception

            cLog.Write(Me.ToString() & ".LoadFleets() Error: " & ex.Message)
            Debug.Assert(False, Me.ToString & ".LoadFleets() Error: " & ex.Message)
            Return False

        End Try
        Return True

    End Function

    ''' <summary>
    ''' Stop a running EcoSim model
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopEcoSim()
        Try
            If Not m_EcoSim Is Nothing Then
                m_EcoSim.bStopRunning = True
            End If
        Catch ex As Exception
            cLog.Write(Me.ToString & ".StopEcoSim() Error: & " & ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Change the number of years the ecosim model runs for
    ''' </summary>
    ''' <param name="newNumberOfYears"></param>
    ''' <remarks>There are two events that can trigger this. User has set the Ecosim run length. User has loaded timeseries data which will set the Ecosim run length to the same as the timeseries data</remarks>
    Private Sub setEcosimRunLength(ByVal newNumberOfYears As Integer, Optional ByVal bOverwriteNewData As Boolean = True)
        'newNumberOfYears has already passed validation
        'set the number of years the model will run for and resize and reload all the data

        Try
            'number of years before setting to the new value
            Dim orgNYears As Integer = m_EcoSimData.NumYears

            If newNumberOfYears = 0 Then Exit Sub
            'sets NumYears and NTimes and resize the underlying data to the new number of years
            m_EcoSimData.RedimTime(newNumberOfYears, m_TSData.NdatYear, bOverwriteNewData)

            'redim the cv vars to the new timesteps
            Me.m_MSEData.redimTime(orgNYears)

            'Reload the forcing data PoolForceBB(), PoolForceZ(), PoolForceCatch() and FishRateGear(), FishRateNo
            'forcing data needs to be the max of Reference data years and Ecosim Years
            Me.m_TSData.LoadForcingData(m_EcoSimData, Math.Max(m_TSData.NdatYear, m_EcoSimData.NumYears))

            Me.m_SearchData.redimTime(m_EcoSimData.NumYears)

            Me.m_EcoSpaceData.TotalTime = m_EcoSimData.NumYears
            'changed the run length for ecospace reset the summary periods to defaults
            Me.m_EcoSpaceData.setDefaultSummaryPeriod()

            'Now Update the interface objects

            'tell the affected shape managers that there data has changed
            Dim manager As cBaseShapeManager
            manager = m_ShapeManagers.Item(eDataTypes.FishMort)
            manager.Load()
            manager = m_ShapeManagers.Item(eDataTypes.FishingEffort)
            manager.Load()

            Me.m_SearchManagers(eDataTypes.FishingPolicyManager).Load()
            Me.m_SearchManagers(eDataTypes.MSEManager).Load()

            'Parameters
            Me.LoadEcoSimModelParameters()

            Me.m_publisher.AddMessage(New cMessage("Ecosim number of years has changed.", eMessageType.EcosimNYearsChanged, _
                                                     eCoreComponentType.EcoSim, eMessageImportance.Maintenance))

            Me.m_publisher.AddMessage(New cMessage("Ecosim number of years has changed.", eMessageType.DataModified, _
                                                     eCoreComponentType.ShapesManager, eMessageImportance.Maintenance))

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("Error changing number of Ecosim years.", ex)
        End Try


    End Sub

#Region "EcoSim multi threading"

#If 0 Then
    ''' <summary>
    ''' Run the EcoSim model on a worker thread
    ''' </summary>
    ''' <param name="SynchronizingObject">Snychronization Object from the interface. This must be the same Windows Form as the Time-Step and Completed delegates belong to. </param>
    ''' <param name="ProgressDelegate">Delegate in the interface that will receive the time step notification.</param>
    ''' <param name="CompletedDelegate">Delegate in the interface that will receive the Completed notication.</param>
    ''' <returns>True if thread started successfuly. False if the thread fail to start</returns>
    ''' <remarks>
    ''' The SynchronizingObject will be used for both the Time-Step and the Completed notification 
    ''' this mean both these calls MUST be handled by the same Windows Form.
    ''' How it works:
    ''' The user interface calls RunEcoSimOnThread(...) passing in a Synchronization Object(its self) and Delegates for both the Progress and the Completed notification.
    ''' The ModelInterface keeps a reference to the Synchronization Object and creates its own delegate that EcoSim will call to handle the progress notification.
    ''' The ModelInterface then calls cEcoSim.InitMultiThreading(...)
    ''' Arg#1 The Synchronization Object from the user interface. 
    ''' Arg#2. Progress delegate from its self 
    ''' Arg#3. The completed delegate from the user interface.
    ''' This mean that the running instance of cEcoSim will call the ModelInterface progress delegate which will 
    ''' have a chance to modify the data before calling the user interface progress delegate using the Synchronization Object for the data.
    ''' This gives the ModelInterface a shot at the data before it's passed to the user interface.
    ''' The Completed delegate belongs to the user interface and the ModelInterface will have no chance to modify the call(it shouldn't need to)
    '''</remarks>
    Public Function RunEcoSimOnThread(ByVal SynchronizingObject As System.ComponentModel.ISynchronizeInvoke, _
                                        ByVal ProgressDelegate As EcoSim.EcoSimTimeStepDelegate, _
                                        ByVal CompletedDelegate As EcoSim.EcoSimCompletedDelegate) As Boolean


        Try

            m_publisher.bHoldMessages = True

            'don't start another thread if this one is running
            'done like this instead of a Semaphore 
            'to prevent a second thread from running instead of just blocking/waiting for the current thread to end then starting another
            'to use a Semaphore instead it would have to go inside the running thread RunEcoSim() to prevent the interface from blocking
            If Not m_EcoSimThread Is Nothing Then
                If m_EcoSimThread.IsAlive Then
                    'the thread is still running 
                    'for now don't let another thread start 
                    cLog.Write(Me.ToString & ".RunEcoSimOnThread(SyncObject,Delegate) EcoSim model is already running. Please wait to start another model.")
                    Return False
                End If 'If mEcoSimThread.IsAlive Then
            End If 'If Not mEcoSimThread Is Nothing Then


            'Progress delegate from the user interface this is what will get called by Me.EcoSimProgress_handler(...) from cEcoSim.ProcessTimeStep(...)
            m_InterfaceDelegate = ProgressDelegate

            'Synchronization Object for passing data to the user interface thread
            'Used here by Me.EcoSimProgress_handler(...) and in cEcoSim.runCompleted(...)
            mSynEcoSim = SynchronizingObject

            'set up the multi threading in EcoSim
            'SynchronizingObject from the user interface thread for the call to the Completed delegate
            'EcoSimProgress_handler for the progress handler from here (cModelInterface)
            'CompletedDelegate from the user interface thread
            m_EcoSim.InitMultiThreading(SynchronizingObject, AddressOf EcoSimProgressMultiThread_handler, CompletedDelegate)

            'Create the thread and start it running
            m_EcoSimThread = New System.Threading.Thread(AddressOf RunEcoSimOnThread)
            m_EcoSimThread.Name = "EcoSim Thread"

            m_EcoSimThread.Priority = System.Threading.ThreadPriority.Normal
            m_EcoSimThread.IsBackground = True

            m_EcoSimThread.Start()

        Catch ex As Exception
            cLog.Write(Me.ToString & ".RunEcoSimOnThread(SyncObject,Delegate)Error: " & ex.Message)
            Debug.Assert(False, "Error in RunEcoSimOnThread(...)")
            Return False
        End Try

        Return True

    End Function


       ''' <summary>
    ''' Run the EcoSim Model
    ''' </summary>
    ''' <remarks>
    ''' This is used be the ModelInterface to run the EcoSim model
    ''' </remarks>
    Private Sub RunEcoSimOnThread()

        ''Try
        ''Semaphore object is to protect against running two models at once
        m_EcoSimSemaphor.WaitOne()

        'get any changes the user may have made to parameters/variables
        updateEcoSimGroupInfo()

        'update the model run parameters
        UpdateEcoSimModelParameters()

        m_EcoSim.bStopRunning = False
        m_EcoSim.Run()
        m_EcoSimSemaphor.Release()

        'Catch ex As Exception

        '    cLog.Write(Me.ToString & ".RunEcoSim(...) Error: " & ex.Message)
        '    Debug.Assert(False, "Error trying to run EcoSim Model")

        '    'let EcoSim Start again
        '    mEcoSim.bStopRunning = False
        '    mEcoSimSemaphor.Release()

        'End Try



    End Sub


        ''' <summary>
    ''' Delegate handler for the current EcoSim time-step. Marshalls data from the EcoSim thread to the User Interface thread.
    ''' </summary>
    ''' <param name="iTime">Time Step of the Model</param>
    ''' <param name="results">EcoSim Results object that contains the results of this iTime time-step</param>
    ''' <remarks>
    ''' This handler gets passed to the current EcoSim thread in the call to EcoSim.InitMultiThreading(...)(see  Me.RunEcoSimOnThread(...))
    ''' When it gets call by the EcoSim Model thread it will still be in the EcoSim threads space.
    ''' It must then marshal the data out to the user interface via a reference to a Synchronization Object (mSynEcoSim) that was set in the call to RunEcoSimOnThread(...).
    ''' It is done like this because Windows GUI objects can not be assessed from a thread other then the one they are running on.
    ''' </remarks>
    Private Sub EcoSimProgressMultiThread_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)
        Dim args(1) As Object

        Try

            'set-up the arguments that get passed to the interface delegate
            'arguments get passed to the Syncronization.invoke(...) method as an array of Objects indexed from zero to n-1
            'they must be in the same order as they appear in the declaration of the delegate
            'EcoSimTimeStepDelegate(ByVal iTime As Long, ByVal data As cEcoSimResults)
            args(0) = iTime
            args(1) = results

            'call the sync object with the user interface delegate, this is where delegate will run
            'the Syncronization object handles the marshalling of the data between this thread and the GUI object i.e. a Windows form or textbox
            mSynEcoSim.Invoke(m_InterfaceDelegate, args)

        Catch ex As Exception
            cLog.Write(Me.ToString & ".EcoSimProgress_handler() Delegate has thrown an Error: " & ex.Message)

        End Try


    End Sub


#End If

#End Region

    Public Function RunEcoSim(Optional ByVal TimeStepDelegate As Ecosim.EcoSimTimeStepDelegate = Nothing) As Boolean
        Dim msg As cMessage

        If Me.m_StateMonitor.HasEcosimLoaded() = False Then
            'EcoSim has not been initialized
            Debug.Assert(False, "Ecosim has not been initialized.")
            'a message? this should not happen it is caused by a bug!!!
            Return False '?????? 
        End If

        If Not Me.m_StateMonitor.HasEcopathRan Then

            'BRUTE FORCE APPORACH TO ECOPATH EDIT
            'The Ecopath data has been edit. We have no idea what has changed so we need to re-intialize all the ecosim data
            'giv'er ehh!!!!

            System.Console.WriteLine("Ecosim: StateMonitor.HasEcoPathRan = False")
            'Ecopath has been modified by a user
            'We need to re-run it to make sure all the inputs to Ecosim are up to date with the new data
            'this could cause problem if Ecopath has a problem
            If Not RunEcoPath() Then

                cLog.Write(Me.ToString & ".RunEcoSim() Failed to Run EcoPath.")

                'EcoPath is supposed to have sent a message if it failed
                msg = New cMessage("Ecosim could not be run because Ecopath failed to balance the model.", eMessageType.ErrorEncountered, _
                                            eCoreComponentType.EcoSim, eMessageImportance.Critical, eDataTypes.NotSet)
                m_publisher.SendMessage(msg)
                Return False
            End If
        End If

        Debug.Assert(Me.m_StateMonitor.HasEcopathRan() = True)

        'the .HasEcosimRan flag will be false if ANY value in Ecosim has been changed
        If Not Me.m_StateMonitor.HasEcosimRan Then
            'Ecopath has been re-run to init data that is used by Ecosim
            'OR
            'Ecosim has been edited
            'Ecosim needs to be initialized

            're-initialize Ecosim data
            'this could be streamlined but it's good enough for now (EwE5 StartEcoSim())
            m_EcoSim.Init(Me.m_StateMonitor.RequiresEcosimFullInit)

            'now we need to load any changes to the ecosim data that was made by init
            'into the objects used by the interface
            LoadEcosimGroups()
            LoadEcoSimModelParameters()
            LoadStanzas()
            'Ecopath should have sent out its own message 
            'so we should only need to send a message for Ecosim
            msg = New cMessage("Ecosim has re-run Ecopath and initialized its data.", eMessageType.DataModified, _
                                        eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.NotSet)
            m_publisher.SendMessage(msg)
        End If

        ' Update core state monitor
        Me.m_StateMonitor.SetEcosimRun()

        Me.m_EcoSim.TimeStepDelegate = TimeStepDelegate

        'if Ecosim is being run on a thread then setup the RunCompletedDelegate
        'this will call  Me.EcoSimRunCompleted(Nothing) once Ecosim has completed the run
        Dim bMultiThread As Boolean
        Me.m_EcoSim.RunCompletedDelegate = Nothing
        If Me.m_EcoSimData.bMultiThreaded Then
            bMultiThread = True
            Me.m_EcoSim.RunCompletedDelegate = AddressOf Me.EcoSimRunCompleted
        End If

        'make sure all the searches are turned off
        m_EcoSim.setSearchOff()
        Me.ResetEcosimGroupOutputs()
        m_EcoSim.bStopRunning = False

        m_EcoSim.Run()

        'if not mulithreaded then the Ecosim run has completed 
        'do any processing to complete the run (populate objects, send any messages...)
        'if running in on a thread then Me.EcoSimRunCompleted(Nothing) will be called via the delegate set before the run
        If Not bMultiThread Then
            Me.EcoSimRunCompleted(Nothing)
        End If

        Return True

    End Function

    Private Sub EcoSimRunCompleted(ByVal obj As Object)
        Try

            Me.m_TSData.Update()

            LoadEcosimGroupOutputs()
            LoadEcosimFleetOutputs()
            LoadEcosimTimeSeries()

            LoadEcosimOutputs()
            LoadEcosimStats()
            loadEcoTracerResults()

            If m_EcoSimData.PredictSimEffort Or Me.m_StateMonitor.RequiresEcosimFullInit Then
                'if effort was predicted then reload the shapes
                m_ShapeManagers.Item(eDataTypes.FishMort).Load()
                m_ShapeManagers.Item(eDataTypes.FishingEffort).Load()

                'tell the interface that the shapes have changed
                Me.m_publisher.AddMessage(New cMessage("Fish rate shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishingEffort))
                Me.m_publisher.AddMessage(New cMessage("Fish mort shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishMort))

            End If

            'make sure ecosim can start again
            m_EcoSim.bStopRunning = False

            m_publisher.AddMessage(New cMessage("Ecosim run completed.", eMessageType.EcosimRunCompleted, _
                                            eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.NotSet))

            ' Update core state monitor
            Me.m_StateMonitor.SetEcosimCompleted()
            ' Send messages after
            m_publisher.sendAllMessages()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".EcoSimRunCompleted() Exception: " & ex.Message)
        End Try


    End Sub

    ''' <summary>
    ''' Creates a new cEcoSimScenario object for this nScenario from the uderlying parameters in EcoSim
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario to get or set the variables for.</param>
    ''' <value>
    ''' Returns a valid cEcoSimScenario object if nScenario (group index) is in bounds. 
    ''' Null cEcoSimGroupInfo object if iGroup (group index) is out of bounds or an error occurs.</value>
    Private Property privateEcoSimScenario(ByVal iScenario As Integer) As cEcoSimScenario

        Get
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcoPathData.EcosimScenarioName.Length Then
                    cLog.Write(Me.ToString + ".EcoSimScenario(nScenario) nScenario out of bounds.")
                    Return Nothing
                End If

                Dim infoOut As New cEcoSimScenario(Me)

                infoOut.AllowValidation = False

                infoOut.DBID = m_EcoPathData.EcosimScenarioDBID(iScenario)
                infoOut.Name = m_EcoPathData.EcosimScenarioName(iScenario)
                infoOut.Description = m_EcoPathData.EcosimScenarioDescription(iScenario)
                infoOut.Author = m_EcoPathData.EcosimScenarioAuthor(iScenario)
                infoOut.Contact = m_EcoPathData.EcosimScenarioContact(iScenario)
                infoOut.LastSaved = m_EcoPathData.EcosimScenarioLastSaved(iScenario)
                infoOut.Index = iScenario

                infoOut.ResetStatusFlags()

                infoOut.AllowValidation = True

                Return infoOut

            Catch ex As Exception
                cLog.Write(Me.ToString & ".cEcoSimScenario() Error: " & ex.Message)
                Debug.Assert(False, "Error Getting EcoSim Scenario Info: " & ex.Message)
                Return Nothing
            End Try

        End Get

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Set(ByVal ParametersIn As cEcoSimScenario)

            'Set the parameters in the underlying EcoSim data structures to user supplied values
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcoPathData.EcosimScenarioName.Length Then
                    cLog.Write(Me.ToString + ".EcoSimScenario(nScenario) nScenario out of bounds.")
                    Return
                End If

                m_EcoPathData.EcosimScenarioName(iScenario) = ParametersIn.Name
                m_EcoPathData.EcosimScenarioDescription(iScenario) = ParametersIn.Description
                m_EcoPathData.EcosimScenarioAuthor(iScenario) = ParametersIn.Author
                m_EcoPathData.EcosimScenarioContact(iScenario) = ParametersIn.Contact
                ' Do not update last saved date; this is exclusively set by the core when saving

            Catch ex As Exception
                cLog.Write(Me.ToString & ".cEcoSimScenario() EcoSim Parameters will not be set Error: " & ex.Message)
                Debug.Assert(False, "EcoSim Scenario Info will not be set Error: " & ex.Message)
            End Try

        End Set

    End Property

    ''' <summary>
    ''' Get the <see cref="cEcoSimModelParameters">Ecosim model parameters</see> for
    ''' the currently loaded Ecosim scenario.
    ''' </summary>
    Public ReadOnly Property EcoSimModelParameters() As cEcoSimModelParameters
        Get
            If Not m_bEcoSimIsInit Then
                Debug.Assert(False, "EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
                'MsgBox("EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first", MsgBoxStyle.Critical)
                cLog.Write("EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
                Return Nothing
            End If

            Return m_EcoSimRun
        End Get
    End Property

    ''' <summary>
    ''' Create a new cEcoSimModelParameters object. This is the parameter for the current model run.
    ''' </summary>
    ''' <returns>True if successfull</returns>
    ''' <remarks></remarks>
    Private Function initEcoSimModelParameters() As Boolean

        m_EcoSimRun = New cEcoSimModelParameters(Me)

        Return LoadEcoSimModelParameters()

    End Function

    ''' <summary>
    ''' Reload the Eocsim data into the existing EcoSim parameter object
    ''' </summary>
    ''' <returns>True if no error encountered.</returns>
    ''' <remarks>This can be used if a new scenario is loaded to populate the existing EcoSim parameter object (m_EcoSimRun) with the new scenario data. </remarks>
    Private Function LoadEcoSimModelParameters() As Boolean

        Try
            m_EcoSimRun.AllowValidation = False
            m_EcoSimRun.DBID = m_EcoPathData.EcosimScenarioDBID(m_EcoPathData.ActiveEcosimScenario)
            m_EcoSimRun.Name = m_EcoPathData.EcosimScenarioName(m_EcoPathData.ActiveEcosimScenario)
            m_EcoSimRun.BiomassOn = m_EcoSim.m_Data.BiomassOn
            m_EcoSimRun.Discount = m_EcoSim.m_Data.Discount
            m_EcoSimRun.EquilibriumStepSize = m_EcoSim.m_Data.EquilibriumStepSize
            m_EcoSimRun.EquilMaxFishingRate = m_EcoSim.m_Data.EquilScaleMax
            m_EcoSimRun.NudgeChecked = m_EcoSim.m_Data.NudgeChecked
            m_EcoSimRun.NumberYears = m_EcoSim.m_Data.NumYears
            m_EcoSimRun.NutBaseFreeProp = m_EcoSim.m_Data.NutBaseFreeProp
            m_EcoSimRun.NutForceFunctionNumber = m_EcoSim.m_Data.NutForceNumber
            m_EcoSimRun.NutPBMax = m_EcoSim.m_Data.NutPBmax
            m_EcoSimRun.Relaxation = m_EcoSim.m_Data.SorWt
            m_EcoSimRun.StepSize = m_EcoSim.m_Data.StepSize
            m_EcoSimRun.SystemRecovery = m_EcoSim.m_Data.SystemRecovery
            m_EcoSimRun.UseVarPQ = m_EcoSim.m_Data.UseVarPQ

            m_EcoSimRun.SalinityForceFunctionNumber = m_EcoSim.m_Data.SalinityForceNo
            m_EcoSimRun.TemperatureForceFunctionNumber = m_EcoSim.m_Data.TemperatureForceNo

            m_EcoSimRun.ContaminantTracing = Me.m_tracerData.EcoSimConSimOn
            m_EcoSimRun.PredictEffort = m_EcoSim.m_Data.PredictSimEffort
            m_EcoSimRun.NumberSummaryTimeSteps = m_EcoSim.m_Data.NumStep
            m_EcoSimRun.StartSummaryTime = m_EcoSim.m_Data.SumStart(0)
            m_EcoSimRun.EndSummaryTime = m_EcoSim.m_Data.SumStart(1)

            m_EcoSimRun.AllowValidation = True

            m_EcoSimRun.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function UpdateEcoSimModelParameters() As Boolean

        Try

            m_EcoSim.m_Data.BiomassOn = m_EcoSimRun.BiomassOn
            m_EcoSim.m_Data.Discount = m_EcoSimRun.Discount
            m_EcoSim.m_Data.EquilibriumStepSize = m_EcoSimRun.EquilibriumStepSize
            m_EcoSim.m_Data.EquilScaleMax = m_EcoSimRun.EquilMaxFishingRate
            m_EcoSim.m_Data.NudgeChecked = m_EcoSimRun.NudgeChecked
            m_EcoSim.m_Data.NumYears = m_EcoSimRun.NumberYears
            m_EcoSim.m_Data.NutBaseFreeProp = m_EcoSimRun.NutBaseFreeProp
            m_EcoSim.m_Data.NutForceNumber = m_EcoSimRun.NutForceFunctionNumber
            m_EcoSim.m_Data.NutPBmax = m_EcoSimRun.NutPBMax
            m_EcoSim.m_Data.SorWt = m_EcoSimRun.Relaxation
            m_EcoSim.m_Data.StepSize = m_EcoSimRun.StepSize
            m_EcoSim.m_Data.SystemRecovery = m_EcoSimRun.SystemRecovery
            m_EcoSim.m_Data.UseVarPQ = m_EcoSimRun.UseVarPQ

            Me.m_tracerData.EcoSimConSimOn = m_EcoSimRun.ContaminantTracing

            m_EcoSim.m_Data.SalinityForceNo = m_EcoSimRun.SalinityForceFunctionNumber
            m_EcoSim.m_Data.TemperatureForceNo = m_EcoSimRun.TemperatureForceFunctionNumber

            m_EcoSim.m_Data.PredictSimEffort = m_EcoSimRun.PredictEffort

            m_EcoSim.m_Data.NumStep = m_EcoSimRun.NumberSummaryTimeSteps
            m_EcoSim.m_Data.SumStart(0) = m_EcoSimRun.StartSummaryTime
            m_EcoSim.m_Data.SumStart(1) = m_EcoSimRun.EndSummaryTime

        Catch ex As Exception
            cLog.Write(Me.ToString & ".EcoSimModelRunParameters() EcoSim Parameters will not be set Error: " & ex.Message)
            Debug.Assert(False, "EcoSim Parameters will not be set Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    '''' <summary>
    '''' The user has changed the number of years that the model can run for.
    '''' So update the dimensions of all the time variables (NTimes)
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks>
    '''' This has to be called explicitly be an interface so that it is not reloading all the data on every edit.
    '''' </remarks>
    'Public Function UpdateTimeVariables() As Boolean

    '    Debug.Assert(False, "UpdateTimeVariables() not implemented yet.")
    '    Return False

    '    ' ToDo_jb UpdateTimeVariables every thing

    '    'save the parameters back to the Ecosim data
    '    UpdateEcoSimModelParameters()

    '    'ToDo_jb Core.UpdateTimeVariables needs to tell the datasource to update time variables
    '    'DataSource.UpdateTime()

    '    'jb Tell Ecosim to redim time variables 
    '    'I think this need to be handled by the core and not the datasource because only it knows about Ecosim's needs
    '    m_EcoSim.ReSetTime()

    '    LoadEcosimGroups() 'may not need to load the groups
    '    LoadEcoSimModelParameters()

    '    'ToDo_jb UpdateTimeVariables need to load all the Shapes that are dimmed by time
    '    'size of the time and eggproduction shapes has changed
    '    '   OnShapeEdited(eDataTypes.Forcing)

    '    m_publisher.SendMessage(New cMessage("Groups have been updated.", _
    '                    eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.EcoSimGroupInput))
    '    m_publisher.SendMessage(New cMessage("Model parameters have been updated.", _
    '            eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.EcoSimModelParameter))

    '    'the data changed message has to be sent be the core instead of the shapemanagers 
    '    'because the message refers to all the shape managers not just a single one so all the data has to be loaded before the message can be sent
    '    'm_publisher.SendMessage(New cMessage("Forcing Shapes have been updated.", _
    '    '                        eMessageType.DataChanged, eCoreComponentType.ShapesManager, eMessageImportance.Warning, eDataTypes.Shape))

    'End Function



#If 0 Then

    ''' <summary>
    ''' Dump the values from the last model run into a file that has the same format as used by EwE5 file dump from the Plot interface
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>The resulting file can be used to compare results with EwE5 </remarks>
    Public Function dumpEcosimModelResults(ByVal fileName As String) As Boolean
        Dim strm As System.IO.StreamWriter
        Dim igrp As Integer
        Dim delimiter As String = ", " ' this may have to change to a tab for international formatting

        Try

            If Not Me.m_StateMonitor.HasEcosimRan Then
                Return False
            End If

            strm = System.IO.File.CreateText(fileName)

            'header
            strm.WriteLine(DataSource.ToString() & delimiter & m_EwEModel.Name & delimiter & m_EcoPathData.EcosimScenarioName(m_EcoPathData.ActiveEcosimScenario))

            'group names
            For igrp = 1 To m_EcoPathData.NumGroups
                strm.Write(m_EcoPathData.GroupName(igrp))
                If igrp < m_EcoPathData.NumGroups Then strm.Write(delimiter)
            Next igrp
            strm.Write(vbNewLine)

            'data Groups in columns 
            'Time in rows
            For it As Integer = 1 To m_EcoSimData.NTimes
                For igrp = 1 To m_EcoPathData.NumGroups
                    strm.Write(Me.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, it).ToString)
                    If igrp < m_EcoPathData.NumGroups Then strm.Write(delimiter)
                Next igrp
                strm.Write(vbNewLine)
            Next it

            strm.Close()
            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Try
                strm.Close()
            Catch ex2 As Exception
                'no big deal error closing the stream
            End Try
            Return False
        End Try

    End Function

#End If

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check if Ecosim has non-default vulnerabilities, and if so, reset the
    ''' vulnerabilties. 
    ''' </summary>
    ''' <param name="bQuiet">Flag stating whether the user should be prompted
    ''' whether vulnerabilities should be reset.</param>
    ''' <param name="sDefaultValue">The value to test vulnerabilities for, and
    ''' to set vulnerabilties to.</param>
    ''' <returns>True if Ecosim has all default vulnerabilties.</returns>
    ''' -----------------------------------------------------------------------
    Public Function CheckResetDefaultVulnerabilities(Optional ByVal bQuiet As Boolean = False, _
                                                     Optional ByVal sDefaultValue As Single = 2.0!) As Boolean

        Dim groupSim As cEcoSimGroupInput = Nothing
        Dim fmsg As cFeedbackMessage = Nothing

        If Not Me.HasNonDefaultVulnerabilty(sDefaultValue) Then Return True

        If Not bQuiet Then
            fmsg = New cFeedbackMessage(String.Format(My.Resources.CoreMessages.VULNERABILITIES_PROMPT_RESET, sDefaultValue), _
                                        eCoreComponentType.EcoSim, eMessageType.Any, _
                                        eMessageImportance.Information, _
                                        cFeedbackMessage.eReplyStyle.YES_NO, eDataTypes.NotSet, cFeedbackMessage.eReply.YES)
            Me.m_publisher.SendMessage(fmsg)
            If fmsg.Reply = cFeedbackMessage.eReply.NO Then Return False
        End If

        For iPrey As Integer = 1 To Me.nGroups
            groupSim = Me.EcoSimGroupInputs(iPrey)
            For iPred As Integer = 1 To Me.nGroups
                groupSim.VulMult(iPred) = sDefaultValue
            Next iPred
        Next iPrey

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Test whether Ecosim has non-default vulnerabilities.
    ''' </summary>
    ''' <param name="sValue">The value to test for, 2.0 by default.</param>
    ''' <returns>True if Ecosim has non-default vulnerabilties.</returns>
    ''' <remarks>
    ''' A 0.01 margin of error is tolerated in this test.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function HasNonDefaultVulnerabilty(Optional ByVal sValue As Single = 2.0!) As Boolean

        Dim groupPath As cEcoPathGroupInput = Nothing
        Dim groupSim As cEcoSimGroupInput = Nothing

        For iPred As Integer = 1 To Me.nLivingGroups
            For iPrey As Integer = 1 To Me.nGroups
                groupPath = Me.EcoPathGroupInputs(iPred)
                groupSim = Me.EcoSimGroupInputs(iPred)
                If groupPath.DietComp(iPrey) > 0 Then
                    If (Math.Abs(groupSim.VulMult(iPrey)) - Math.Abs(sValue)) > 0.01 Then Return True
                End If
            Next iPrey
        Next iPred
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The vulnerabilities have changed
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub VulnerabilitiesChanged()

        Try
            Me.LoadEcosimGroups()

            Me.m_StateMonitor.SetEcoSimLoaded(True)
            DataSource.SetChanged(eCoreComponentType.EcoSim)
            Me.m_StateMonitor.UpdateDataState(DataSource)

            Me.Messages.SendMessage(New cMessage("Vulnerabilites changed.", eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Maintenance))

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Public Function CalcEcosimVulBo(ByVal BmaxBo As Single, _
                                    ByVal iGroup As Integer, _
                                    ByVal FtimeOn As Boolean) As Single
        Return Me.m_EcoSim.VulBo(BmaxBo, iGroup, FtimeOn)
    End Function

    Public Function CalcEcosimVulFMax(ByVal Fpo As Single, _
                                      ByVal iGroup As Integer, _
                                      ByVal FtimeOn As Boolean) As Single
        Return Me.m_EcoSim.VulFmax(Fpo, iGroup, FtimeOn)
    End Function

    Public Function EstimateVulnerabilities(ByVal iGroup As Integer, _
                                            ByRef PotGrowth As Single, ByRef FWMax As Single, _
                                            ByVal estimates As Single()) As Boolean
        Return Me.m_EcoSim.EstimateVulnerabilities(iGroup, PotGrowth, FWMax, estimates)
    End Function

#End Region 'EcoSim

#Region " Ecospace "

#Region " Variables "

    Friend m_Ecospace As cEcoSpace
    Friend m_EcoSpaceData As cEcospaceDataStructures
    Private m_EcoSpaceGroups As New cCoreInputOutputList(Of cEcospaceGroup)(eDataTypes.EcospaceGroup, 1)
    Private m_EcoSpaceFleets As New cCoreInputOutputList(Of cEcospaceFleet)(eDataTypes.EcospaceFleet, 1)
    Private m_EcoSpaceScenarios As New cCoreInputOutputList(Of cEcospaceScenario)(eDataTypes.EcoSpaceScenario, 1)
    Friend m_EcospaceHabitats As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcospaceHabitat, 0)
    Friend m_EcospaceRegions As New cCoreInputOutputList(Of cEcospaceRegion)(eDataTypes.EcospaceLayerRegion, 1)
    Friend m_EcospaceMPAs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcospaceMPA, 1)
    Private m_EcospaceModelParams As cEcospaceModelParameters
    Private m_EcospaceBasemap As cEcospaceBasemap

    Private m_spaceresults As cEcospaceTimestep
    Private m_SpaceInterfaceCallBack As EcoSpaceInterfaceDelegate

    'Ecospace output lists
    '  Friend m_EcospaceGroupSummaries As New cCoreInputOutputList(Of cEcospaceGroupSummary)(eDataTypes.NotSet, 1)
    ' Index 0 holds the combined fleet
    Friend m_EcospaceFleetOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 0)
    ' the zero index holds the data not include in one of the other regions
    Friend m_EcospaceRegionSummaries As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 0)
    Friend m_EcospaceGroupOuputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 1)

#End Region ' Variables

    Private Function InitEcoSpace() As Boolean

        m_Ecospace = New cEcoSpace()

        m_Ecospace.Messages.AddMessageHandler(New cMessageHandler(AddressOf EcospaceMessageHandler, eCoreComponentType.EcoSpace, eMessageType.Any, Me.m_SyncObj))

        'm_EcoSpaceData = New cEcospaceDataStructures
        m_SpaceTSData = New cEcospaceTimeSeriesDataStructures

        m_Ecospace.TimeSeriesData = m_SpaceTSData

        'data need to initialize
        m_EcoSpaceData.StanzaGroups = Me.m_Stanza
        m_EcoSpaceData.EcoPathData = Me.m_EcoPathData

        'counters needed 
        'this could change to get the counter from the above data structures
        m_EcoSpaceData.NGroups = Me.nGroups
        m_EcoSpaceData.nFleets = Me.nFleets
        m_EcoSpaceData.nLiving = Me.nLivingGroups

        m_EcoSpaceData.ReDimFleets()
        m_EcoSpaceData.SetDefaults()

        m_EcoSpaceData.DefaultBasemapDimensions()
        m_EcoSpaceData.RedimMigratoryVariables()

        m_Ecospace.EcoSpaceData = Me.m_EcoSpaceData
        m_Ecospace.StanzaData = Me.m_Stanza
        m_Ecospace.EcoPathData = Me.m_EcoPathData
        m_Ecospace.EcoSim = Me.m_EcoSim
        m_Ecospace.EcoSimData = Me.m_EcoSimData
        m_Ecospace.ContaiminantTracerData = m_tracerData

        'sub in core to call at each time step
        m_Ecospace.TimeStepDelegate = AddressOf onEcospaceTimeStep

        'this will initialize local Ecospace variables to default values as well as some dimensioning
        m_Ecospace.InitToDefaults()

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Recalculate the Ecospace distance sailing cost map for all fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub CalcEcospaceCostOfSailing()

        Me.m_Ecospace.CalculateCostOfSailing()
        Me.onChanged(Me.EcospaceBasemap.LayerSailingCost)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set all coastal cells to ports for a given fleet.
    ''' </summary>
    ''' <param name="iFleet">The fleet to set the ports for. If not provided,
    ''' all coastal cells for all fleets are set to ports.</param>
    ''' -----------------------------------------------------------------------
    Public Sub SetEcospaceAllCoastToPort(Optional ByVal iFleet As Integer = cCore.NULL_VALUE)

        Me.m_Ecospace.SetAllCoastsToPorts(iFleet)
        Me.onChanged(Me.EcospaceBasemap.LayerPort)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear port cells for a given fleet.
    ''' </summary>
    ''' <param name="iFleet">The fleet to clear the ports for. If not provided,
    ''' all ports for all fleets are cleared.</param>
    ''' -----------------------------------------------------------------------
    Public Sub ClearEcospacePort(Optional ByVal iFleet As Integer = cCore.NULL_VALUE)

        Me.m_Ecospace.ClearPorts(iFleet)
        Me.onChanged(Me.EcospaceBasemap.LayerPort)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run the Ecospace model with the currently loaded Ecosim and Ecospace scenario
    ''' </summary>
    ''' <param name="EcospaceTimeStepHandler">Optional handler to call with timestep data. 
    '''  If no handler is supplied then the user will not be called at each time step. </param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RunEcoSpace(Optional ByRef EcospaceTimeStepHandler As EcoSpaceInterfaceDelegate = Nothing) As Boolean
        Dim breturn As Boolean

        Debug.Assert(Me.m_StateMonitor.HasEcospaceLoaded, "RunEcospace() You must load an Ecospace scenario first.")

        If Me.m_StateMonitor.HasEcosimLoaded Then
            If Not Me.m_StateMonitor.HasEcosimInitialized Then
                'Ecosim is loaded but not initialized do a full initialization
                If Me.m_EcoSim.Init(True) Then
                    Me.StateMonitor.SetEcoSimInitialized()
                Else
                    'Failed to init Ecosim, post a message and return
                    Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_SIM_INIT_FAILED, _
                                              eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Warning))
                    Return False
                End If
            End If
        Else
            'No Ecosim scenario loaded, post a message and return
            Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_NO_SIM_SCENARIO, _
                                      eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Warning))
            Return False
        End If

        Try
            Dim t As Double = Timer
            System.Console.WriteLine("----------cCore.RunEcospace() Start------------")

            If Me.m_StateMonitor.HasEcospaceLoaded Then

                If checkHabitats() Then

                    'user supplied delegate to call at each time step
                    Me.m_SpaceInterfaceCallBack = EcospaceTimeStepHandler
                    'set the handler for the Ecospace time step 
                    'this makes sure Ecospace calls the core handler and not some other process that was running Ecospace
                    m_Ecospace.TimeStepDelegate = AddressOf onEcospaceTimeStep

                    Me.m_StateMonitor.SetEcospaceRun()

                    breturn = m_Ecospace.Run()
                    If breturn Then
                        'only load the results if the run was successful
                        LoadEcospaceResults()
                        loadEcoTracerResults()
                    End If

                    Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_RUN_COMPLETED, _
                                  eMessageType.EcospaceRunCompleted, eCoreComponentType.EcoSpace, eMessageImportance.Information))

                End If 'If GroupsMissingHabitat() Then

            Else 'If Me.m_StateMonitor.HasEcospaceLoaded Then
                Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_NO_SPACE_SCENARIO, _
                                          eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Warning))
            End If 'If Me.m_StateMonitor.HasEcospaceLoaded Then

            System.Console.WriteLine("cCore.RunEcospace() Run Time = " & CDbl(Timer - t))
            System.Console.WriteLine("----------cCore.RunEcospace() End------------")

            Me.m_StateMonitor.SetEcospaceCompleted()
            Me.m_publisher.sendAllMessages()

            If (m_pluginManager IsNot Nothing) Then m_pluginManager.EcospaceRunCompleted(Me.m_EcoSpaceData)

            Return breturn

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Me.m_publisher.SendMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_RUN_ERROR & ex.Message, _
                                      eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Critical))

            Return False
        End Try

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Does every group have area defined on the map for its habitat(s)
    ''' </summary>
    ''' <returns>True if all groups have habitat area. False otherwise</returns>
    ''' -----------------------------------------------------------------------
    Private Function checkHabitats() As Boolean

        Dim bHasArea As Boolean
        Dim groups As New List(Of Integer)
        Dim grpNames As String
        Dim igrp As Integer

        If Not Me.m_EcoSpaceData.NewMultiStanza Then
            'this only matters for the New Multi Stanza code
            Return True
        End If

        For isp As Integer = 1 To Me.m_Stanza.Nsplit
            For ist As Integer = 1 To m_Stanza.Nstanza(isp)

                igrp = m_Stanza.EcopathCode(isp, ist)
                bHasArea = False

                For ihab As Integer = 0 To Me.nHabitats

                    If Me.m_EcoSpaceData.PrefHab(igrp, ihab) Then

                        If Me.m_EcoSpaceData.HabAreaProportion(ihab) > 0 Then
                            bHasArea = True
                            Exit For
                        End If
                    End If

                Next ihab

                If Not bHasArea Then
                    'no area for this group
                    groups.Add(igrp)
                End If

            Next ist
        Next isp

        If groups.Count > 0 Then
            grpNames = "Group(s) "
            For Each grp As Integer In groups
                grpNames += Me.m_EcoPathData.GroupName(grp) & ", "
            Next grp
            'strip the last ',' of the end of the string
            grpNames = grpNames.Remove(grpNames.LastIndexOf(","), 2)
            grpNames += " do not have a map area defined for there habitat(s)."
            grpNames += " Ecospace cannot be run."
            grpNames += " Please edit either your Habitat Assignments or Basemap data."

            Me.Messages.AddMessage(New cMessage(grpNames, eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Critical))
            Return False
        End If

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stop a running EcoSpace model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub StopEcospace()
        Try
            If Not m_Ecospace Is Nothing Then
                'ToDo_jb: there needs to be some kind of a distinction between a model run that was stopped and one that completed on it's own
                'right now all the statemanager knows is that Ecospace has completed not why
                m_Ecospace.StopRun = True
            End If
        Catch ex As Exception
            cLog.Write(Me.ToString & ".StopEcospace() Error: & " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' This gets call by Ecospace at every time step
    ''' </summary>
    ''' <param name="iTime">Time index of this time step</param>
    ''' <remarks>processEcospaceTimeStep() will populate the cEcospaceTSResults object and send it to an interface</remarks>
    Private Sub onEcospaceTimeStep(ByVal iTime As Integer)
        Try
            'only populate the results object if there is somewhere to send it
            If m_SpaceInterfaceCallBack IsNot Nothing Then

                m_spaceresults.iTimeStep = iTime
                m_spaceresults.TimeStepinYears = m_EcoSpaceData.TimeNow + m_EcoSpaceData.TimeStep

                m_spaceresults.ComputeIBMMap()

                'the group time-step data was populated by Ecospace
                For igrp As Integer = 1 To nGroups
                    m_spaceresults.Biomass(igrp) = m_EcoSpaceData.ResultsByGroup(eSpaceResultsGroups.Biomass, igrp, iTime)
                    m_spaceresults.RelativeBiomass(igrp) = m_EcoSpaceData.ResultsByGroup(eSpaceResultsGroups.RelativeBiomass, igrp, iTime)
                    If m_Ecospace.ContaiminantTracerData.EcoSpaceConSimOn Then
                        m_spaceresults.ConcMax(igrp) = m_Ecospace.ContaiminantTracerData.ConcMax(igrp)
                    End If

                    For irgn As Integer = 1 To nRegions
                        m_spaceresults.BiomassByRegion(igrp, irgn) = m_EcoSpaceData.ResultsRegionGroup(irgn, igrp, iTime)
                    Next
                Next

                m_SpaceInterfaceCallBack(m_spaceresults)

            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".processEcospaceTimeStep() Error: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Sample code to loop over EcoSpace map of mortality by predation 
    ''' </summary>
    Private Sub dumpSpaceMortPred(ByVal Core As cCore, ByVal SpaceResults As cEcospaceTimestep)

        'loop over all the prey/pred linkages
        For iLink As Integer = 1 To SpaceResults.nPreyPredLinks
            Dim mort As Single
            'get group indexes for prey and pred
            Dim iPreyIndex As Integer = SpaceResults.iPreyIndex(iLink)
            Dim iPredIndex As Integer = SpaceResults.iPredIndex(iLink)

            'get prey and pred names(just for display)
            Dim PreyName As String = Core.EcoPathGroupInputs(iPreyIndex).Name
            Dim PredName As String = Core.EcoPathGroupInputs(iPredIndex).Name

            'loop over the map rows/cols and dump mortality values
            For iRow As Integer = 1 To Core.GetCoreCounter(eCoreCounterTypes.nRows)
                For iCol As Integer = 1 To Core.GetCoreCounter(eCoreCounterTypes.nCols)
                    'mortality in this map cell by Link
                    'the Prey/Pred for this Link are in iPreyIndex and iPredIndex
                    mort = SpaceResults.MortPredRate(iRow, iCol, iLink)
                    System.Console.WriteLine("mortality of " & PreyName + " by " & PredName & " = " & mort.ToString & " for row col " & iRow.ToString & ", " & iCol.ToString)
                Next iCol
            Next iRow

        Next

    End Sub

    ''' <summary>
    ''' Message handler for messages sent by Ecospace
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Private Sub EcospaceMessageHandler(ByRef message As cMessage)

        'at this moment this just passes the messages off the who ever is listening
        'the core does not have to do anything in response to Ecospace messages
        m_publisher.AddMessage(message)

    End Sub

#Region " Ecospace interface objects "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the number of available Ecospace scenarios for the loaded model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceScenarioCount() As Integer
        Get
            Try
                ' Return the official ecopath administration figure
                Return Me.m_EcoPathData.NumEcospaceScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets an <see cref="cEcospaceScenario">Ecospace scenario</see> from the
    ''' list of available scenarios.
    ''' </summary>
    ''' <param name="iScenario">One based indexed property of Ecospace Scenario 
    ''' objects</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceScenarios(ByVal iScenario As Integer) As cEcospaceScenario
        Get
            ' JS 06Jul07: list will handle scenario index / item index offsets
            Return Me.m_EcoSpaceScenarios(iScenario)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the index of the active <see cref="cEcospaceScenario">Ecospace scenario</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveEcospaceScenarioIndex() As Integer
        Get
            Return Me.m_EcoPathData.ActiveEcospaceScenario
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cEcospaceModelParameters">Ecospace model parameters</see>
    ''' for the currently loaded Ecospace scenario.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceModelParameters() As cEcospaceModelParameters
        Get
            Return m_EcospaceModelParams
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cEcospaceBasemap">Ecospace base map</see> for the 
    ''' currently loaded Ecospace scenario.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceBasemap() As cEcospaceBasemap
        Get
            Return Me.m_EcospaceBasemap
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceGroup">Ecospace group</see> for a given index.
    ''' </summary>
    ''' <param name="iGroup">The index to obtain the Ecospace group for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceGroups(ByVal iGroup As Integer) As cEcospaceGroup
        Get
            ' JS 06Jul07: list will handle group index / item index offsets
            Return m_EcoSpaceGroups.Item(iGroup)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceFleet">Ecospace fleet</see> for a given index.
    ''' </summary>
    ''' <param name="iFleet">The index to obtain the Ecospace fleet for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceFleets(ByVal iFleet As Integer) As cEcospaceFleet
        Get
            ' JS 06Jul07: list will handle fleet index / item index offsets
            Return m_EcoSpaceFleets.Item(iFleet)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceHabitat">Ecospace habitat</see> for a given index.
    ''' </summary>
    ''' <param name="iHabitat">The index to obtain the Ecospace habitat for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceHabitats(ByVal iHabitat As Integer) As cEcospaceHabitat
        Get
            ' JS 06Jul07: list will handle habitat index / item index offsets
            Return DirectCast(Me.m_EcospaceHabitats(iHabitat), cEcospaceHabitat)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceRegion">Ecospace region</see> for a given index.
    ''' </summary>
    ''' <param name="iRegion">The index to obtain the Ecospace region for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceRegions(ByVal iRegion As Integer) As cEcospaceRegion
        Get
            ' JS 06Jul07: list will handle region index / item index offsets
            Return Me.m_EcospaceRegions(iRegion)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceMPA">Ecospace MPA</see> for a given index.
    ''' </summary>
    ''' <param name="iMPA">The index to obtain the Ecospace marine protected area for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceMPAs(ByVal iMPA As Integer) As cEcospaceMPA
        Get
            ' JS 06Jul07: list will handle MPA index / item index offsets
            Return DirectCast(Me.m_EcospaceMPAs(iMPA), cEcospaceMPA)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecosim Fleet inputs.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcosimFleetInputs(ByVal iFleet As Integer) As cEcosimFleetInput
        Get
            ' JS 05Nov09: list will handle fleet index / item index offsets
            Return DirectCast(Me.m_EcosimFleetInputs(iFleet), cEcosimFleetInput)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecosim Fleet summary results from last Ecosim run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcosimFleetOutput(ByVal iFleet As Integer) As cEcosimFleetOutput
        Get
            ' JS 06Jul07: list will handle fleet index / item index offsets
            Return DirectCast(Me.m_EcosimFleetOutputs(iFleet), cEcosimFleetOutput)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Results from last Ecospace run by group
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceGroupOutput(ByVal iGroup As Integer) As cEcospaceGroupOutput
        Get
            ' JS 06Jul07: list will handle group index / item index offsets
            Return DirectCast(Me.m_EcospaceGroupOuputs(iGroup), cEcospaceGroupOutput)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace Fleet summary results from last Ecospace run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceFleetOutput(ByVal iFleet As Integer) As cEcospaceFleetOutput
        Get
            ' JS 06Jul07: list will handle fleet index / item index offsets
            Return DirectCast(Me.m_EcospaceFleetOutputs(iFleet), cEcospaceFleetOutput)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace region summary results from last Ecospace run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceRegionOutput(ByVal iRegion As Integer) As cEcospaceRegionOutput
        Get
            ' JS 06Jul07: list will handle region index / item index offsets
            Return DirectCast(Me.m_EcospaceRegionSummaries(iRegion), cEcospaceRegionOutput)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Statistics from the last Ecospace model run
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceStats() As cEcospaceStats
        Get
            Return Me.m_EcospaceStats
        End Get
    End Property

#End Region ' Ecospace interface objects

#Region " Scenarios "

    Private Function InitEcospaceScenarios() As Boolean
        Me.m_EcoSpaceScenarios.Clear()
        For i As Integer = 1 To Me.m_EcoPathData.EcospaceScenarioDBID.Length - 1
            Me.m_EcoSpaceScenarios.Add(Me.privateEcospaceScenario(i))
        Next
        Return True
    End Function

    ''' <summary>
    ''' Creates a new <see cref="cEcospaceScenario">cEcospaceScenario</see> object for this 
    ''' nScenario from the underlying parameters in Ecospace.
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario to get/set the variables for.</param>
    ''' <value>
    ''' Returns a valid <see cref="cEcospaceScenario">cEcospaceScenario</see> object if nScenario,
    ''' the scenario index, is in bounds, or  Null when the index is out of bounds or an error 
    ''' occured.</value>
    Private Property privateEcospaceScenario(ByVal iScenario As Integer) As cEcospaceScenario

        Get
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcoPathData.EcospaceScenarioDBID.Length Then
                    cLog.Write(Me.ToString + ".privateEcospaceScenario(iScenario) index out of bounds.")
                    Return Nothing
                End If

                Dim ess As New cEcospaceScenario(Me)

                ess.AllowValidation = False

                ess.DBID = m_EcoPathData.EcospaceScenarioDBID(iScenario)
                ess.Name = m_EcoPathData.EcospaceScenarioName(iScenario)
                ess.Author = m_EcoPathData.EcospaceScenarioAuthor(iScenario)
                ess.Contact = m_EcoPathData.EcospaceScenarioContact(iScenario)
                ess.LastSaved = m_EcoPathData.EcospaceScenarioLastSaved(iScenario)
                ess.Index = iScenario
                ess.ResetStatusFlags()

                ess.AllowValidation = True

                Return ess

            Catch ex As Exception
                cLog.Write(Me.ToString & ".cEcospaceScenario() Error: " & ex.Message)
                Debug.Assert(False, "Error Getting cEcospaceScenario Info: " & ex.Message)
                Return Nothing
            End Try

        End Get

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Set(ByVal ess As cEcospaceScenario)

            'Set the parameters in the underlying EcoSim data structures to user supplied values
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcoPathData.EcospaceScenarioDBID.Length Then
                    cLog.Write(Me.ToString + ".cEcospaceScenario(nScenario) nScenario out of bounds.")
                    Return
                End If

                m_EcoPathData.EcospaceScenarioName(iScenario) = ess.Name

            Catch ex As Exception
                cLog.Write(Me.ToString & ".privateEcospaceScenario() EcoSim parameters will not be set Error: " & ex.Message)
                Debug.Assert(False, "cEcospaceScenario Info will not be set Error: " & ex.Message)
            End Try

        End Set

    End Property

    Private Sub SendEcospaceLoadMessage(ByVal iScenario As Integer, Optional ByVal strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = String.Format(My.Resources.CoreMessages.ECOSPACE_LOAD_SUCCESS, Me.m_EcoPathData.EcospaceScenarioName(iScenario))
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Information)
        Else
            strText = String.Format(My.Resources.CoreMessages.ECOSPACE_LOAD_FAILED, Me.m_EcoPathData.EcospaceScenarioName(iScenario), strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()

    End Sub

    Private Sub SendEcospaceSaveStateMessage(ByVal strScenarioName As String, Optional ByVal bSucces As Boolean = True, _
            Optional ByVal strError As String = "")

        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If bSucces Then
            strText = String.Format(My.Resources.CoreMessages.ECOSPACE_SAVE_SUCCES, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataModified, eCoreComponentType.EcoSpace, eMessageImportance.Information)
        Else
            strText = String.Format(My.Resources.CoreMessages.ECOSPACE_SAVE_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()
    End Sub

    ''' <summary>
    ''' Creates and loads a new Ecospace scenario.
    ''' </summary>
    ''' <param name="strName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <param name="strAuthor">Author of new scenario.</param>
    ''' <param name="strContact">Contact of new scenario.</param>
    ''' <param name="iNumRows">Number of rows in basemap.</param>
    ''' <param name="iNumCols">Number of columns in basemap.</param>
    ''' <param name="sLat">Latitude of basemap (TL corner).</param>
    ''' <param name="sLon">Longitude of basemap (TL corner)></param>
    ''' <param name="sCellSize">Cell size, in degrees.</param>
    ''' <returns>True if succesful.</returns>
    Public Function NewEcospaceScenario(ByVal strName As String, ByVal strDescription As String, _
            ByVal strAuthor As String, ByVal strContact As String, _
            ByVal iNumRows As Integer, ByVal iNumCols As Integer, _
            ByVal sLat As Single, ByVal sLon As Single, ByVal sCellSize As Single) As Boolean

        Dim ds As IEcospaceDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim iScenario As Integer = 0

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        If Me.m_StateMonitor.HasEcopathLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecospace) Then Return False

        Try

            ds = DirectCast(DataSource, IEcospaceDatasource)

            ds.BeginTransaction()
            If (ds.AppendEcospaceScenario(strName, strDescription, _
                    strAuthor, strContact, _
                    iNumRows, iNumCols, _
                    sLat, sLon, sCellSize, iScenarioID)) Then
                ds.EndTransaction(True)

                Me.StateMonitor.UpdateDataState(Me.m_DataSource)
                Me.InitEcospaceScenarios()
                iScenario = Array.IndexOf(Me.m_EcoPathData.EcospaceScenarioDBID, iScenarioID)
                Return Me.LoadEcospaceScenario(iScenario)
            End If

            ds.EndTransaction(False)
            Return False
        Catch ex As Exception

        End Try
        Return False

    End Function

    ''' <summary>
    ''' Load an <see cref="cEcoSimScenario">Ecospace scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcoSpaceScenario">Scenario</see> to load.</param>
    ''' <returns>True if succesful.</returns>
    Public Function LoadEcospaceScenario(ByVal scenario As cEcospaceScenario) As Boolean
        Return LoadEcospaceScenario(scenario.Index)
    End Function

    ''' <summary>
    ''' Load an <see cref="cEcoSpaceScenario">Ecospace scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the <see cref="cEcoSpaceScenario">Scenario</see> in the <see cref="m_EcoSpaceScenarios">Scenario list</see>.</param>
    ''' <returns>True if succesful.</returns>
    Public Function LoadEcospaceScenario(ByVal iScenario As Integer) As Boolean

        Dim ds As IEcospaceDatasource = Nothing
        Dim bSuccess As Boolean = True

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecospace) Then Return False

        Try

            'For an Ecospace scenario to load there must be an Ecosim scenario loaded
            If Not Me.m_StateMonitor.HasEcosimLoaded() Then
                'No implicit running of Ecosim because we do not know which Ecosim scenario to run
                Debug.Assert(False, "LoadEcospaceScenario() Load  Ecosim first. This is temporary.")
                'SendEcospaceLoadMessage(iScenario, "Load Ecosim first. This is temporary.")
                Return False
            End If

            ' Update core state
            Me.m_StateMonitor.SetEcospaceLoaded(False)
            Me.m_EcoPathData.ActiveEcospaceScenario = -1

            ds = DirectCast(DataSource, IEcospaceDatasource)
            If Not ds.LoadEcospaceScenario(Me.m_EcoPathData.EcospaceScenarioDBID(iScenario)) Then
                Debug.Assert(False, "LoadEcospaceScenario() Failed to load scenario from data source.")
                SendEcospaceLoadMessage(iScenario, " ")
                Return False
            End If

            'set the time steps is Ecospace to be the same as ecosim
            If m_EcoSpaceData.TotalTime <> m_EcoSimData.NumYears Then m_EcoSpaceData.TotalTime = m_EcoSimData.NumYears

            m_Ecospace.SearchData = m_SearchData

            'sets the summary peroids to first and last year
            'at this time this data is not saved in the database
            m_EcoSpaceData.setDefaultSummaryPeriod()

            'This flag tells Ecospace to use the fishing rates set by Ecosim
            'in EwE5 it is set in the Ecosim database reading routine 
            'if it is set to false Ecospace will set all the FishGearRates() to one
            m_EcoSpaceData.IsFishRateSet = True

            m_EcoSpaceData.SetDefaultThreads()
            m_Ecospace.redimForRun()

            ' JS30oct09: Spatial Equilibrium is ONLY required when starting an Ecospace run
            'm_Ecospace.initSpatialEquilibrium()

            'Init MPA Optimization
            Dim MPAOptManager As ISearchObjective = Me.m_SearchManagers.Item(eDataTypes.MPAOptManager)
            MPAOptManager.Init(Me)
            MPAOptManager.Load()

            bSuccess = InitEcospaceBasemap()
            bSuccess = bSuccess And InitEcospaceModelParameters()
            bSuccess = bSuccess And InitEcospaceHabitats()
            bSuccess = bSuccess And InitEcospaceRegions()
            bSuccess = bSuccess And InitEcospaceMPAs()
            bSuccess = bSuccess And InitEcospaceGroups()
            bSuccess = bSuccess And InitEcospaceFleets()

            InitEcospaceOutputs()
            InitEcotracerOutputs()

            SendEcospaceLoadMessage(iScenario)

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.LoadEcospaceScenario(ds)
                Me.PluginManager.EcospaceInitialized(Me.m_EcoSpaceData)
            End If

            ' Update core state
            Me.m_StateMonitor.SetEcospaceLoaded(bSuccess)

        Catch ex As Exception
            cLog.Write(Me.ToString & ".LoadEcospaceScenario(...) Error: " & ex.Message)
            SendEcospaceLoadMessage(iScenario, ex.Message)
            Debug.Assert(False, ex.Message)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcospaceScenario() As Boolean

        Dim iScenarioID As Integer = 0
        Dim ds As IEcospaceDatasource = Nothing
        Dim bSucces As Boolean = False

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Overwrite scenario?
        iScenarioID = m_EcoPathData.EcospaceScenarioDBID(ActiveEcospaceScenarioIndex)
        Debug.Assert(iScenarioID > 0)

        ds = DirectCast(DataSource, IEcospaceDatasource)
        ' No need to save? Yippee
        If Not ds.IsEcospaceModified Then Return True
        ' Save ok?
        If (ds.SaveEcospaceScenario(iScenarioID)) Then

            ' #Yes: reload ecospace scenario defs
            Me.InitEcospaceScenarios()
            ' Update active scenario ID
            Me.m_EcoPathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcoPathData.EcospaceScenarioDBID, iScenarioID)

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcospaceScenario(Me)
            ' Force update
            Me.m_StateMonitor.SetEcospaceLoaded(True, TriState.True)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Report succes
            Me.SendEcospaceSaveStateMessage(Me.m_EcoPathData.EcospaceScenarioName(Me.ActiveEcospaceScenarioIndex))
            Return True
        End If

        ' Restore previous active scenario ID on save failure
        Me.m_EcoPathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcoPathData.EcospaceScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcospaceSaveStateMessage(Me.m_EcoPathData.EcospaceScenarioName(Me.ActiveEcospaceScenarioIndex), False, _
                My.Resources.CoreMessages.GENERIC_SAVE_RESOLUTION)

        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the current ecospace scenario under a new name.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcospaceScenarioAs(ByVal strName As String, _
                                           ByVal strDescription As String) As Boolean

        Dim epd As cEcopathDataStructures = Me.m_EcoPathData
        Dim esd As cEcospaceDataStructures = Me.m_EcoSpaceData
        Dim ds As IEcospaceDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcospaceDatasource) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False

        iScenarioID = Me.m_EcoPathData.EcospaceScenarioDBID(Me.ActiveEcospaceScenarioIndex)
        If (iScenarioID <= 0) Then Return bSucces

        ' Is this the current scenario?
        If String.Compare(Me.EcospaceScenarios(Me.ActiveEcospaceScenarioIndex).Name, strName, True) = 0 Then
            ' #Yes: save directly instead
            Return Me.SaveEcospaceScenario()
        End If

        ds = DirectCast(DataSource, IEcospaceDatasource)
        ' Save ok?
        If (ds.SaveEcospaceScenarioAs(strName, strDescription, _
                epd.EcospaceScenarioAuthor(Me.ActiveEcospaceScenarioIndex), _
                epd.EcospaceScenarioContact(Me.ActiveEcospaceScenarioIndex), _
                iScenarioID)) Then

            ' #Yes: invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcospaceScenario(Me)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)

            ' Reload scenarios
            Me.InitEcospaceScenarios()

            ' Inform the world
            Me.SendEcospaceSaveStateMessage(strName)
            ' Load Ecospace scenario
            bSucces = Me.LoadEcospaceScenario(Array.IndexOf(epd.EcospaceScenarioDBID, iScenarioID))
            Me.DataAddedOrRemovedMessage("Ecospace number of scenarios has changed.", eCoreComponentType.EcoSpace, eDataTypes.EcoSpaceScenario)
            Return bSucces
        End If

        ' Restore previous active scenario ID on save failure
        Me.m_EcoPathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcoPathData.EcospaceScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcospaceSaveStateMessage(strName, bSucces)
        Return bSucces
    End Function

    ''' <summary>
    ''' Remove a <see cref="cEcoSpaceScenario">Ecospace Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcoSpaceScenario">Scenario</see> to remove.</param>
    ''' <returns>True if succesful.</returns>
    Public Function RemoveEcospaceScenario(ByVal scenario As cEcospaceScenario) As Boolean
        Return Me.RemoveEcosimScenario(scenario.Index)
    End Function

    ''' <summary>
    ''' Remove a <see cref="cEcoSpaceScenario">Ecospace Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario in the <see cref="m_EcoSpaceScenarios">Ecospace Scenario list</see>.</param>
    ''' <returns>True if succesful.</returns>
    Public Function RemoveEcospaceScenario(ByVal iScenario As Integer) As Boolean

        Dim ds As IEcospaceDatasource = Nothing
        Dim iScenarioID As Integer = cCore.NULL_VALUE ' Scenario to restore
        Dim bSucces As Boolean = False

        ' Sanity check
        Debug.Assert(iScenario > 0 And iScenario < Me.m_EcoPathData.EcospaceScenarioDBID.Length)

        ' Cannot delete a loaded scenario
        If (iScenario = Me.ActiveEcospaceScenarioIndex) Then
            Me.m_publisher.SendMessage(New cMessage("Cannot delete a loaded scenario", eMessageType.NotSet, eCoreComponentType.DataSource, eMessageImportance.Warning))
            Return False
        End If

        ' Save pending relevant changes
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecospace) Then Return False

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcospaceDatasource) Then Return False

        ' Remember scenario ID to restore
        If (Me.ActiveEcospaceScenarioIndex > 0) Then
            iScenarioID = Me.m_EcoPathData.EcospaceScenarioDBID(Me.ActiveEcospaceScenarioIndex)
        End If

        ds = DirectCast(Me.DataSource, IEcospaceDatasource)
        ' Scenario removed succesfully?
        If ds.RemoveEcospaceScenario(Me.m_EcoPathData.EcospaceScenarioDBID(iScenario)) Then
            ' #Yes: reload scenario list
            bSucces = Me.InitEcospaceScenarios()
            ' Restore active scenario ID
            Me.m_EcoPathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcoPathData.EcospaceScenarioDBID, iScenarioID)
            ' Broadcast change
            Me.DataAddedOrRemovedMessage("Ecospace number of scenarios has changed.", eCoreComponentType.EcoSpace, eDataTypes.EcoSpaceScenario)
        End If
        ' Return succes
        Return bSucces
    End Function


    ''' <summary>
    ''' Update all the underlying data structures that contain Ecospace scenario data
    ''' </summary>
    ''' <returns>True if successful.</returns>
    Private Function UpdateEcospaceScenario(ByVal iDBID As Integer) As Boolean

        Dim iScenario As Integer = Array.IndexOf(Me.m_EcoPathData.EcospaceScenarioDBID, iDBID)
        Dim scn As cEcospaceScenario = Me.EcospaceScenarios(iScenario)

        Try
            Me.m_EcoPathData.EcospaceScenarioName(iScenario) = scn.Name
            Me.m_EcoPathData.EcospaceScenarioDescription(iScenario) = scn.Description
            Me.m_EcoPathData.EcospaceScenarioAuthor(iScenario) = scn.Author
            Me.m_EcoPathData.EcospaceScenarioContact(iScenario) = scn.Contact

        Catch ex As Exception
            cLog.Write(Me.ToString & ".UpdateEcoSpaceScenario() Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

#End Region ' Scenarios

#Region " Model parameters "

    Private Function InitEcospaceModelParameters() As Boolean
        'there is only one cEcospaceModelParameters object 
        Try
            Me.m_EcospaceModelParams = New cEcospaceModelParameters(Me, m_EcoPathData.EcospaceScenarioDBID(ActiveEcospaceScenarioIndex))
            '  Return True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcospaceModelParameters() Error: " & ex.Message)
            Return False
        End Try

        Return LoadEcospaceModelParameters()

    End Function

    Private Function LoadEcospaceModelParameters() As Boolean
        'there is only one cEcospaceModelParameters object 
        Dim bSucces As Boolean = True

        ' Debug.Assert(m_EcospaceModelParams IsNot Nothing, Me.ToString & ".LoadEcospaceModelParameters() m_EcospaceModelParams is null.")
        Try

            If m_EcospaceModelParams Is Nothing Then Return False
            m_EcospaceModelParams.AllowValidation = False
            m_EcospaceModelParams.PredictEffort = m_EcoSpaceData.PredictEffort
            m_EcospaceModelParams.NumberOfTimeStepsPerYear = CSng(1.0 / m_EcoSpaceData.TimeStep)

            m_EcospaceModelParams.AdjustSpace = m_EcoSpaceData.AdjustSpace

            ' JS 04jun08: Villy wants the summary times in the interface to be treated as integer values
            m_EcospaceModelParams.StartSummaryTime = CInt(m_EcoSpaceData.SumStart(0))
            m_EcospaceModelParams.EndSummaryTime = CInt(m_EcoSpaceData.SumStart(1))
            m_EcospaceModelParams.NumberSummaryTimeSteps = m_EcoSpaceData.NumStep

            m_EcospaceModelParams.nSolverThreads = m_EcoSpaceData.nGridSolverThreads
            m_EcospaceModelParams.nGroupsPerThread = m_EcoSpaceData.nGroupsPerThread

            m_EcospaceModelParams.nMapCellsPerThread = m_EcoSpaceData.nCellsPerThread
            m_EcospaceModelParams.nSpaceThreads = m_EcoSpaceData.nSpaceSolverThreads

            m_EcospaceModelParams.IFDPower = m_EcoSpaceData.IFDPower
            m_EcospaceModelParams.UseIBM = m_EcoSpaceData.UseIBM
            m_EcospaceModelParams.UseNewMultiStanza = m_EcoSpaceData.NewMultiStanza
            m_EcospaceModelParams.TotalTime = CInt(m_EcoSpaceData.TotalTime)

            m_EcospaceModelParams.Tolerance = m_EcoSpaceData.Tol
            m_EcospaceModelParams.SOR = m_EcoSpaceData.W
            m_EcospaceModelParams.MaxNumberOfIterations = m_EcoSpaceData.maxIter
            m_EcospaceModelParams.UseExact = m_EcoSpaceData.UseExact


            ' JS06jun07: There is no generic stanza object to expose the packets multiplier value. Since this
            '             value is used during Ecospace calculations, it makes sense to expose it from Ecospace.
            m_EcospaceModelParams.PacketsMultiplier = Me.m_Stanza.NPacketsMultiplier

            Me.Set_IBM_Flags(m_EcospaceModelParams)

            m_EcospaceModelParams.ResetStatusFlags()
            m_EcospaceModelParams.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcospaceModelParameters() Error: " & ex.Message)
            bSucces = False
        End Try
        Return bSucces

    End Function

    Private Function UpdateEcospaceModelParameters() As Boolean

        If m_EcospaceModelParams Is Nothing Then Return False

        m_EcoSpaceData.PredictEffort = m_EcospaceModelParams.PredictEffort

        'casting to single was resulting in the timestep being slightly larger (in some cases)this was causing the time loop in ecospace to exit early
        'rounding seems to have solved this
        'm_EcoSpaceData.TimeStep = CSng(1.0 / m_EcospaceModelParams.NumberOfTimeStepsPerYear)
        m_EcoSpaceData.TimeStep = CSng(Math.Round(1.0 / m_EcospaceModelParams.NumberOfTimeStepsPerYear, 6))

        m_EcoSpaceData.SumStart(0) = m_EcospaceModelParams.StartSummaryTime
        m_EcoSpaceData.SumStart(1) = m_EcospaceModelParams.EndSummaryTime
        m_EcoSpaceData.NumStep = m_EcospaceModelParams.NumberSummaryTimeSteps
        m_EcoSpaceData.AdjustSpace = m_EcospaceModelParams.AdjustSpace

        m_EcoSpaceData.nGroupsPerThread = m_EcospaceModelParams.nGroupsPerThread
        m_EcoSpaceData.nGridSolverThreads = m_EcospaceModelParams.nSolverThreads

        m_EcoSpaceData.nSpaceSolverThreads = m_EcospaceModelParams.nSpaceThreads
        m_EcoSpaceData.nCellsPerThread = m_EcospaceModelParams.nMapCellsPerThread

        m_EcoSpaceData.IFDPower = m_EcospaceModelParams.IFDPower
        m_EcoSpaceData.UseIBM = m_EcospaceModelParams.UseIBM
        m_EcoSpaceData.NewMultiStanza = m_EcospaceModelParams.UseNewMultiStanza
        m_EcoSpaceData.TotalTime = m_EcospaceModelParams.TotalTime

        m_EcoSpaceData.Tol = m_EcospaceModelParams.Tolerance
        m_EcoSpaceData.W = m_EcospaceModelParams.SOR
        m_EcoSpaceData.maxIter = m_EcospaceModelParams.MaxNumberOfIterations
        m_EcoSpaceData.UseExact = m_EcospaceModelParams.UseExact

        ' JS06jun07: There is no generic stanza object to expose the packets multiplier value. Since this
        '             value is used during Ecospace calculations, it makes sense to expose it from Ecospace.
        Me.m_Stanza.NPacketsMultiplier = m_EcospaceModelParams.PacketsMultiplier

        Me.m_tracerData.EcoSpaceConSimOn = m_EcospaceModelParams.ContaminantTracing

        Return True

    End Function

#End Region ' Model parameters

#Region " Basemap "

    Private Function InitEcospaceBasemap() As Boolean

        Try
            m_EcospaceBasemap = New cEcospaceBasemap(Me)
            Return LoadEcospaceBasemap()
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Public Function ResizeEcospaceBasemap(ByVal InRow As Integer, ByVal InCol As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim bSucces As Boolean = False

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.ResizeEcospaceBasemap(InRow, InCol) Then

            ' Reload the scenario
            If Me.LoadEcospaceScenario(Me.ActiveEcospaceScenarioIndex) Then

                ' Egg
                Dim r As New Random()
                If CInt(r.NextDouble * 42) = 13 Then
                    Me.m_publisher.AddMessage(New cMessage("Map has been resized; a tsunami warning has been issued.", _
                        eMessageType.NotSet, eCoreComponentType.EcoSpace, eMessageImportance.Warning))
                End If

                bSucces = True

            End If
        End If

        ' Decrease batch count, stating what has been changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize ecospace basemap from core data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcospaceBasemap() As Boolean

        Try
            Debug.Assert(m_EcospaceBasemap IsNot Nothing, Me.ToString & ".LoadEcospaceBasemap() basemap is null.")
            If m_EcospaceBasemap Is Nothing Then Return False

            With m_EcospaceBasemap
                .AllowValidation = False
                .InCol = m_EcoSpaceData.InCol
                .InRow = m_EcoSpaceData.InRow
                .CellLength = m_EcoSpaceData.CellLength
                .Latitude = m_EcoSpaceData.Lat1 'UDH_UL
                .Longitude = m_EcoSpaceData.Lon1
                .ResetStatusFlags()
                .AllowValidation = True
            End With

            LoadEcospaceImportanceLayer()
            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadEcospaceImportanceLayer() As Boolean
        Dim dest As cEcospaceLayerImportance = Nothing
        Dim src As cEcospaceDataStructures.cLayerImportanceData = Nothing

        For i As Integer = 0 To Me.m_EcoSpaceData.nImportanceLayers - 1
            src = Me.m_EcoSpaceData.ImportanceLayers(i)
            dest = Me.m_EcospaceBasemap.LayerImportance(i + 1)

            dest.AllowValidation = False
            dest.Index = i
            dest.Weight = src.sWeight
            dest.Name = src.strName
            dest.Description = src.strDescription
            dest.AllowValidation = True

        Next i
    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update core data from ecospace basemap.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function UpdateEcospaceBasemap() As Boolean

        Dim bSucces As Boolean = True

        ' JS070227: The layers operate directly onto the data arrays. This may need to change

        Try

            ' JS21sep07: Basemap row/col set via ResizeEcospaceBasemap
            'Me.m_EcoSpaceData.Inrow = m_EcospaceBasemap.InRow
            'Me.m_EcoSpaceData.InCol = m_EcospaceBasemap.InCol
            Me.m_EcoSpaceData.CellLength = m_EcospaceBasemap.CellLength
            Me.m_EcoSpaceData.Lat1 = m_EcospaceBasemap.Latitude
            Me.m_EcoSpaceData.Lon1 = m_EcospaceBasemap.Longitude

            UpdateEcospaceImportanceLayers()

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

    Private Sub UpdateEcospaceImportanceLayers()
        Dim src As cEcospaceLayerImportance = Nothing
        Dim dest As cEcospaceDataStructures.cLayerImportanceData = Nothing

        For i As Integer = 0 To Me.m_EcoSpaceData.nImportanceLayers - 1
            src = Me.m_EcospaceBasemap.LayerImportance(i + 1)
            dest = Me.m_EcoSpaceData.ImportanceLayers(i)

            dest.sWeight = src.Weight
            dest.strName = src.Name
            dest.strDescription = src.Description

        Next i
    End Sub

#End Region ' Basemap

#Region " Groups "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize <see cref="cEcospaceGroup">Ecospace group</see> objects to
    ''' expose to the interface layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function InitEcospaceGroups() As Boolean

        Dim grp As cEcospaceGroup = Nothing

        Try

            m_EcoSpaceGroups.Clear()

            'populate the list of cEcoSimGroupInfo objects that the user will interact with 
            'to change group related parameters from the interface see getEcoSimGroupInfo(iGroup)
            For i As Integer = 1 To nGroups
                ' Create group
                grp = New cEcospaceGroup(Me, Me.m_EcoSpaceData.GroupDBID(i))
                ' Add to list
                m_EcoSpaceGroups.Add(grp)
            Next i

            ' Load the Ecospace data into the objects created above
            Return LoadEcospaceGroups()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcospaceGroups() Error: " & ex.Message)
            Return False
        End Try


    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load Ecospace group data from the underlying data structures into the 
    ''' interface objects.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcospaceGroups() As Boolean

        Dim iGroup As Integer
        Dim i As Integer

        Try

            For Each grp As cEcospaceGroup In Me.m_EcoSpaceGroups

                'convert the Database ID into an iGroup
                iGroup = Array.IndexOf(m_EcoSpaceData.GroupDBID, grp.DBID)
                grp.Index = iGroup

                Debug.Assert(iGroup > 0 And iGroup <= Me.nGroups, "LoadEcospaceGroups() failed to find iGroup for Ecospace DBID.")

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                grp.Resize()

                grp.AllowValidation = False

                grp.Name = m_EcoPathData.GroupName(iGroup)
                'Mvel
                grp.SetVariable(eVarNameFlags.MVel, m_EcoSpaceData.Mvel(iGroup))

                grp.RelMoveBad = m_EcoSpaceData.RelMoveBad(iGroup)
                grp.RelVulBad = m_EcoSpaceData.RelVulBad(iGroup)
                grp.EatEffBad = m_EcoSpaceData.EatEffBad(iGroup)
                grp.IsMigratory = m_EcoSpaceData.IsMigratory(iGroup)
                grp.IsAdvected = m_EcoSpaceData.IsAdvected(iGroup)
                grp.MigrationNSCon = m_EcoSpaceData.MigConcCol(iGroup)
                grp.MigrationEWCon = m_EcoSpaceData.MigConcRow(iGroup)
                grp.BarrierAvoidanceWeight = m_EcoSpaceData.barrierAvoidanceWeight(iGroup)

                'jb test this out PreferedCell
                Dim pt As Drawing.Point
                For i = 1 To N_MONTHS
                    pt = New Drawing.Point
                    pt.X = m_EcoSpaceData.Prefcol(iGroup, i)
                    pt.Y = m_EcoSpaceData.PrefRow(iGroup, i)
                    grp.PreferredCell(i) = pt
                Next i

                For i = 0 To nHabitats - 1
                    grp.PreferredHabitat(i) = m_EcoSpaceData.PrefHab(iGroup, i)
                Next

                grp.ResetStatusFlags()
                grp.AllowValidation = True

            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcospaceGroups() Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function

    Private Function UpdateEcospaceGroup(ByVal iDBID As Integer) As Boolean

        Dim grp As cEcospaceGroup = Nothing
        Dim pt As Drawing.Point
        Dim iGroup As Integer
        Dim i As Integer

        Try

            ' Convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcoSpaceData.GroupDBID, iDBID)
            ' Get the group
            grp = Me.EcospaceGroups(iGroup)

            ' Suck it empty
            m_EcoSpaceData.Mvel(iGroup) = grp.MVel

            m_EcoSpaceData.RelMoveBad(iGroup) = grp.RelMoveBad
            m_EcoSpaceData.RelVulBad(iGroup) = grp.RelVulBad
            m_EcoSpaceData.EatEffBad(iGroup) = grp.EatEffBad
            m_EcoSpaceData.IsAdvected(iGroup) = grp.IsAdvected
            m_EcoSpaceData.IsMigratory(iGroup) = grp.IsMigratory
            m_EcoSpaceData.MigConcCol(iGroup) = grp.MigrationNSCon
            m_EcoSpaceData.MigConcRow(iGroup) = grp.MigrationEWCon
            m_EcoSpaceData.barrierAvoidanceWeight(iGroup) = grp.BarrierAvoidanceWeight

            For i = 1 To N_MONTHS
                pt = grp.PreferredCell(i)
                m_EcoSpaceData.Prefcol(iGroup, i) = pt.X
                m_EcoSpaceData.PrefRow(iGroup, i) = pt.Y
            Next i

            For i = 0 To nHabitats - 1
                m_EcoSpaceData.PrefHab(iGroup, i) = grp.PreferredHabitat(i)
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateEcospaceGroup() Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function

    Private Sub InitEcospaceOutputs()
        Try

            m_EcospaceFleetOutputs.Clear()
            m_EcospaceRegionSummaries.Clear()
            m_EcospaceGroupOuputs.Clear()

            For igrp As Integer = 1 To nGroups
                m_EcospaceGroupOuputs.Add(New cEcospaceGroupOutput(Me, Me.m_EcoSpaceData, igrp))
            Next

            'this includes zero index 'Combined Fleets' 
            For iflt As Integer = 0 To nFleets 'this includes the 'Combined Fleets' 
                Me.m_EcospaceFleetOutputs.Add(New cEcospaceFleetOutput(Me, Me.m_EcoSpaceData, iflt))
            Next

            'This will include the zero indexed region 
            'the zero index holds the data not include in one of the other regions (OR)
            'It is NOT like the Fleets where the zero index in the combined values (AND)
            For iRgn As Integer = 0 To nRegions
                Me.m_EcospaceRegionSummaries.Add(New cEcospaceRegionOutput(Me, Me.m_EcoSpaceData, iRgn))
            Next

            'load a new results object for the new scenario
            m_spaceresults = New cEcospaceTimestep(Me.m_EcoSimData, Me.m_EcoSpaceData, Me.m_Stanza)

            m_EcospaceStats = New cEcospaceStats(Me, cCore.NULL_VALUE)


            'in the other InitEcospacexxxx the data is loaded during the init
            'for the output LoadEcospaceResults() is not called until the model has successfully run 

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcospaceOutputs() Error: " & ex.Message)
        End Try
    End Sub


    Private Sub LoadEcospaceResults()
        'see cEcoSpace.ScaleAfterNumStep(), summarizeCatchData() and summarizeTimeStepData()
        Dim iflt As Integer
        Dim igrp As Integer
        Dim stVal As Single, endVal As Single

        Try

            'Spatial results are averaged over space by Ecospace in cEcoSpaceDataStructures.AverageSpatialResults()

            'Fleet summarized output
            For Each objFlt As cEcospaceFleetOutput In m_EcospaceFleetOutputs

                'loads results over time
                objFlt.Init()

                If objFlt.Index <> 0 Then
                    objFlt.Name = m_EcoPathData.FleetName(objFlt.Index)
                Else
                    objFlt.Name = My.Resources.CoreDefaults.CORE_DEFAULT_COMBINEDFLEETS
                End If

                m_EcoSpaceData.getSumCatchFleet(objFlt.Index, stVal, endVal)
                objFlt.CatchStart = stVal
                objFlt.CatchEnd = endVal

                m_EcoSpaceData.getSumCostFleet(Me.m_EcoPathData.cost, objFlt.Index, stVal, endVal)
                objFlt.CostStart = stVal
                objFlt.CostEnd = endVal

                m_EcoSpaceData.getSumValueFleet(objFlt.Index, stVal, endVal)
                objFlt.ValueStart = stVal
                objFlt.ValueEnd = endVal


                m_EcoSpaceData.getSumEffortES(objFlt.Index, stVal)
                objFlt.EffortES = stVal

            Next objFlt

            For Each objRgn As cEcospaceRegionOutput In m_EcospaceRegionSummaries
                objRgn.Resize()

                'init the core data arrays
                objRgn.Init()

                If objRgn.Index <> 0 Then
                    objRgn.Name = m_EcoSpaceData.RegionName(objRgn.Index)
                Else
                    objRgn.Name = "Undefined Area"
                End If

                'average the data over the number of cells in the region for output
                Dim nCellsInRegion As Integer = m_EcoSpaceData.nCellsInRegion(objRgn.Index)
                If nCellsInRegion = 0 Then nCellsInRegion = 1

                For igrp = 1 To nGroups

                    Dim sbio As Single, ebio As Single
                    m_EcoSpaceData.getSumBiomByRegion(objRgn.Index, igrp, sbio, ebio)
                    objRgn.BiomassStart(igrp) = sbio
                    objRgn.BiomassEnd(igrp) = ebio

                    For iflt = 0 To nFleets
                        Dim sCatch As Single, eCatch As Single
                        m_EcoSpaceData.getSumCatchRegionGearGroup(objRgn.Index, iflt, igrp, sCatch, eCatch)
                        '  Debug.Assert(sCatch = 0)
                        objRgn.CatchFleetGroupStart(iflt, igrp) = sCatch
                        objRgn.CatchFleetGroupEnd(iflt, igrp) = eCatch
                    Next iflt

                Next igrp


            Next objRgn

            For Each objGrpOutput As cEcospaceGroupOutput In m_EcospaceGroupOuputs
                'init the object to the underlying ecospace data
                objGrpOutput.Init()
                objGrpOutput.ResetStatusFlags()
                objGrpOutput.Name = m_EcoPathData.GroupName(objGrpOutput.Index)

                m_EcoSpaceData.getSumBiom(objGrpOutput.Index, stVal, endVal)
                objGrpOutput.BiomassStart = stVal
                objGrpOutput.BiomassEnd = endVal

                For iflt = 0 To nFleets
                    m_EcoSpaceData.getSumCatchFleetGroup(iflt, objGrpOutput.Index, stVal, endVal)
                    objGrpOutput.CatchStart(iflt) = stVal
                    objGrpOutput.CatchEnd(iflt) = endVal

                    m_EcoSpaceData.getSumValueFleetGroup(iflt, objGrpOutput.Index, stVal, endVal)
                    objGrpOutput.ValueStart(iflt) = stVal
                    objGrpOutput.ValueEnd(iflt) = endVal
                Next iflt

            Next

            Me.m_EcospaceStats.SS = m_EcoSpaceData.SS

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & "LoadEcospaceResults() Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Groups

#Region " Habitats "

    Private Function InitEcospaceHabitats() As Boolean

        Try
            Dim objHab As cEcospaceHabitat

            m_EcospaceHabitats.Clear()

            'populate the list of cEcospaceHabitat objects that the user will interact with 
            'to change habitat related parameters from the interface
            For i As Integer = 0 To nHabitats - 1
                ' Create habitat
                objHab = New cEcospaceHabitat(Me, Me.m_EcoSpaceData.HabitatDBID(i))
                ' Set index
                objHab.Index = i
                ' Add to list
                m_EcospaceHabitats.Add(objHab)
            Next i

            ' Load the Ecospace data into the objects created above
            Return LoadEcospaceHabitats()

        Catch ex As Exception
            Debug.Assert(False, "InitEcospaceHabitats Error: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadEcospaceHabitats() As Boolean
        Dim iHab As Integer = -1

        Try

            Me.m_Ecospace.CalcHabitatArea()

            For Each objHab As cEcospaceHabitat In Me.m_EcospaceHabitats
                ' Get index
                iHab = objHab.Index
                ' Validate
                Debug.Assert(iHab = Array.IndexOf(m_EcoSpaceData.HabitatDBID, objHab.DBID), "LoadEcospaceHabitats() detected Index inconsistency")

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                objHab.Resize()

                objHab.AllowValidation = False

                objHab.Name = m_EcoSpaceData.HabitatText(iHab)
                objHab.HabAreaProportion = m_EcoSpaceData.HabAreaProportion(iHab)

                objHab.AllowValidation = True

            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcospaceHabitats() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the ecospace data structures with the content of an 
    ''' <see cref="cEcospaceHabitat">Ecospace habitat</see>.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the Ecospace Habitat to update.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function UpdateEcospaceHabitat(ByVal iDBID As Integer) As Boolean
        Dim objHab As cEcospaceHabitat = Nothing
        Dim iHabitat As Integer = Array.IndexOf(Me.m_EcoSpaceData.HabitatDBID, iDBID)

        ' Sanity check
        Debug.Assert(iHabitat > 0)
        Debug.Assert(Me.nHabitats >= iHabitat)

        Try
            ' Get the object
            objHab = DirectCast(Me.m_EcospaceHabitats(iHabitat), cEcospaceHabitat)

            m_EcoSpaceData.HabitatText(iHabitat) = objHab.Name
            m_EcoSpaceData.HabAreaProportion(iHabitat) = objHab.HabAreaProportion

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcospaceHabitats() Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an <see cref="cEcospaceHabitat">Ecospace habitat</see> to the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="strHabitatName">Name of habitat to add.</param>
    ''' <param name="iHabitatID">DBID of the habitat.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceHabitat(ByVal strHabitatName As String, ByRef iHabitatID As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AddEcospaceHabitat(strHabitatName, iHabitatID) Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage(String.Format("Ecospace habitat {0} has been added", strHabitatName), _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        Else
            bSucces = False
        End If

        ' Decrease batch count, stating what has been changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an <see cref="cEcospaceHabitat">Ecospace habitat</see> from the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="objHabitat">The <see cref="cEcospaceHabitat">Ecospace habitat</see> to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceHabitat(ByVal objHabitat As cEcospaceHabitat) As Boolean
        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False
        ' Not allowed to remove 'All' habitat
        If objHabitat.Index = 0 Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceHabitat(objHabitat.DBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace habitat has been removed", _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function

#End Region ' Habitats

#Region " Regions "

    Private Function InitEcospaceRegions() As Boolean

        Try
            Dim objRegion As cEcospaceRegion = Nothing

            m_EcospaceRegions.Clear()

            'populate the list of cEcospaceRegion objects that the user will interact with 
            'to change region related parameters from the interface
            For i As Integer = 1 To Me.nRegions
                ' Create region
                objRegion = New cEcospaceRegion(Me, Me.m_EcoSpaceData.RegionDBID(i))
                ' Add to list
                m_EcospaceRegions.Add(objRegion)
            Next i

            ' Load the Ecospace data into the objects created above
            Return LoadEcospaceRegions()

        Catch ex As Exception
            Debug.Assert(False, "InitEcospaceRegions Error: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadEcospaceRegions() As Boolean
        Dim iReg As Integer

        Try

            For Each objReg As cEcospaceRegion In Me.m_EcospaceRegions

                'convert the Database ID into an iGroup
                iReg = Array.IndexOf(m_EcoSpaceData.RegionDBID, objReg.DBID)
                objReg.Index = iReg
                Debug.Assert(iReg > 0 And iReg <= Me.nRegions, "LoadEcospaceRegions() failed to find iRegion for Ecospace DBID.")

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                objReg.Resize()

                objReg.AllowValidation = False

                ' Just pass on the name (RegionName)
                objReg.Name = m_EcoSpaceData.RegionName(iReg)

                objReg.AllowValidation = True
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcospaceRegions() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

    Private Function UpdateEcospaceRegion(ByVal iDBID As Integer) As Boolean
        Dim objReg As cEcospaceRegion = Nothing
        Dim iReg As Integer

        Try
            ' convert the Database ID into an index
            iReg = Array.IndexOf(m_EcoSpaceData.RegionDBID, iDBID)
            ' get the object
            objReg = Me.m_EcospaceRegions(iReg)

            ' Just pass on the name (RegionName)
            m_EcoSpaceData.RegionName(iReg) = objReg.Name

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateEcospaceRegion() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an <see cref="cEcospaceRegion">Ecospace region</see> to the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="strRegionName">Name of region to add.</param>
    ''' <param name="iDBID">DB id of the new region.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceRegion(ByVal strRegionName As String, ByRef iDBID As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim obj As cCoreInputOutputBase = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AppendEcospaceRegion(strRegionName, iDBID) Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage(String.Format("Ecospace Region {0} has been added", strRegionName), _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        Else
            bSucces = False
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an <see cref="cEcospaceRegion">Ecospace region</see> from the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="objRegion">The <see cref="cEcospaceRegion">Ecospace region</see> to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceRegion(ByVal objRegion As cEcospaceRegion) As Boolean
        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Not allowed to delete 0 region (if any)
        If objRegion.Index = 0 Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceRegion(objRegion.DBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace region has been removed", _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function

#End Region ' Regions

#Region " MPAs "

    Private Function InitEcospaceMPAs() As Boolean

        Try
            Dim objMPA As cEcospaceMPA = Nothing

            m_EcospaceMPAs.Clear()

            'populate the list of cEcospaceMPA objects that the user will interact with 
            'to change MPA related parameters from the interface
            For i As Integer = 1 To Me.nMPAs
                ' Create MPA
                objMPA = New cEcospaceMPA(Me, Me.m_EcoSpaceData.MPADBID(i))
                ' Add to list
                m_EcospaceMPAs.Add(objMPA)
            Next i

            ' Load the Ecospace data into the objects created above
            Return LoadEcospaceMPAs()

        Catch ex As Exception
            Debug.Assert(False, "InitEcospaceMPAs Error: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadEcospaceMPAs() As Boolean
        Dim iMPA As Integer

        Try
            For Each objMPA As cEcospaceMPA In Me.m_EcospaceMPAs

                'convert the Database ID into an iGroup
                iMPA = Array.IndexOf(m_EcoSpaceData.MPADBID, objMPA.DBID)
                objMPA.Index = iMPA
                Debug.Assert(iMPA > 0 And iMPA <= Me.nMPAs, "LoadEcospaceMPAs() failed to find iMPA for Ecospace DBID.")

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                objMPA.Resize()

                objMPA.AllowValidation = False

                objMPA.Name = m_EcoSpaceData.MPAname(iMPA)
                For iMonth As Integer = 1 To N_MONTHS
                    objMPA.MPAMonth(iMonth) = m_EcoSpaceData.MPAmonth(iMonth, iMPA)
                Next iMonth

                objMPA.AllowValidation = True
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcospaceMPAs() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

    Private Function UpdateEcospaceMPA(ByVal iDBID As Integer) As Boolean
        Dim objMPA As cEcospaceMPA = Nothing
        Dim iMPA As Integer

        Try
            ' convert the Database ID into an index
            iMPA = Array.IndexOf(m_EcoSpaceData.MPADBID, iDBID)
            ' get the object
            objMPA = Me.EcospaceMPAs(iMPA)

            m_EcoSpaceData.MPAname(iMPA) = objMPA.Name
            For iMonth As Integer = 1 To N_MONTHS
                m_EcoSpaceData.MPAmonth(iMonth, iMPA) = objMPA.MPAMonth(iMonth)
            Next iMonth

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateEcospaceMPA() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an <see cref="cEcospaceMPA">Ecospace MPA</see> to the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="strMPAName">Name of MPA to add.</param>
    ''' <param name="iMPA">Index of the new MPA.</param>
    ''' <param name="abMPAMonths">Flags indicating when the MPA is OPEN for fishing.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceMPA(ByVal strMPAName As String, _
                                   ByVal abMPAMonths() As Boolean, _
                                   ByRef iMPA As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim obj As cCoreInputOutputBase = Nothing
        Dim iDBID As Integer = 0
        Dim bSucces As Boolean = True

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AppendEcospaceMPA(strMPAName, abMPAMonths, iDBID) Then
            ' JS 20sep07: Release batch lock will reload the scenario already
            '' This has effects throughout the Ecospace scenario - reload it
            'bSucces = Me.LoadEcospaceScenario(Me.ActiveEcospaceScenarioIndex)
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage(String.Format("Ecospace MPA {0} has been added", strMPAName), _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        Else
            bSucces = False
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an <see cref="cEcospaceMPA">Ecospace MPA</see> from the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="objMPA">The <see cref="cEcospaceMPA">Ecospace MPA</see> to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceMPA(ByVal objMPA As cEcospaceMPA) As Boolean
        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceMPA(objMPA.DBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace MPA has been removed", _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function

#End Region ' MPAs

#Region " Fleets "

    Private Function InitEcospaceFleets() As Boolean

        Try
            Dim objFleet As cEcospaceFleet

            m_EcoSpaceFleets.Clear()

            'populate the list of cEcospaceHabitat objects that the user will interact with 
            'to change habitat related parameters from the interface
            For i As Integer = 1 To nFleets
                ' Create fleet
                objFleet = New cEcospaceFleet(Me, Me.m_EcoSpaceData.FleetDBID(i))
                ' Add to list
                m_EcoSpaceFleets.Add(objFleet)
            Next i

            ' Load the Ecospace data into the objects created above
            Return LoadEcospaceFleets()

        Catch ex As Exception
            Debug.Assert(False, "InitEcospaceFleets Error: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadEcospaceFleets() As Boolean
        Dim iFleet As Integer

        Try

            For Each fleet As cEcospaceFleet In Me.m_EcoSpaceFleets

                'convert the Database ID into an iGroup
                iFleet = Array.IndexOf(m_EcoSpaceData.FleetDBID, fleet.DBID)
                fleet.Index = iFleet
                Debug.Assert(iFleet >= 0 And iFleet <= Me.nFleets, String.Format("LoadEcospaceFleets() failed to find iFleet for Ecospace DBID {0}.", fleet.DBID))

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                fleet.Resize()

                fleet.AllowValidation = False

                fleet.Name = m_EcoPathData.FleetName(iFleet)

                ' JS 04feb08: in sync with EwE5, this value is now read into space.EffPower
                'fleet.EffectivePower = m_EcoPathData.Epower(iFleet)
                fleet.EffectivePower = m_EcoSpaceData.EffPower(iFleet)
                fleet.TotalEffMultiplier = m_EcoSpaceData.SEmult(iFleet)

                For iHabitat As Integer = 0 To Me.nHabitats
                    fleet.HabitatFishery(iHabitat) = m_EcoSpaceData.GearHab(iFleet, iHabitat)
                Next
                For iMPA As Integer = 0 To Me.nMPAs
                    fleet.MPAFishery(iMPA) = m_EcoSpaceData.MPAfishery(iFleet, iMPA)
                Next

                fleet.ResetStatusFlags()

                fleet.AllowValidation = True

            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcospaceFleets() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

    Private Function UpdateEcospaceFleet(ByVal iDBID As Integer) As Boolean
        Dim fleet As cEcospaceFleet = Nothing
        Dim iFleet As Integer

        Try

            ' Convert the Database ID into an index
            iFleet = Array.IndexOf(m_EcoSpaceData.FleetDBID, iDBID)
            ' Get the object
            fleet = Me.EcospaceFleets(iFleet)

            m_EcoPathData.FleetName(iFleet) = fleet.Name
            ' JS 04feb08: in sync with EwE5, this value is now read into space.EffPower
            'm_EcoPathData.Epower(iFleet) = fleet.EffectivePower
            m_EcoSpaceData.EffPower(iFleet) = fleet.EffectivePower
            m_EcoSpaceData.SEmult(iFleet) = fleet.TotalEffMultiplier

            For iHabitat As Integer = 0 To Me.nHabitats
                m_EcoSpaceData.GearHab(iFleet, iHabitat) = fleet.HabitatFishery(iHabitat)
            Next
            For iMPA As Integer = 0 To Me.nMPAs
                m_EcoSpaceData.MPAfishery(iFleet, iMPA) = fleet.MPAFishery(iMPA)
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateEcospaceFleet() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

#End Region ' Fleets

#Region " Importance layers "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an <see cref="cEcospaceLayerImportance">Ecospace importance layer</see>
    ''' to the current <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="strName">Name of layer to add.</param>
    ''' <param name="strDescription">Description of layer to add.</param>
    ''' <param name="sWeight">Weight of layer to add.</param>
    ''' <param name="iID">DBID that the datasource has assigned to the new 
    ''' layer.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceImportanceLayer(ByVal strName As String, ByVal strDescription As String, ByVal sWeight As Single, ByRef iID As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AppendEcospaceImportanceLayer(strName, strDescription, sWeight, iID) Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage(String.Format("Ecospace importance layer {0} has been added", strName), _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        Else
            bSucces = False
        End If

        ' Decrease batch count, stating what has been changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an <see cref="cEcospaceHabitat">Ecospace habitat</see> from the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="objLayer">The <see cref="cEcospaceLayerImportance">
    ''' Ecospace importance layer</see> to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceImportanceLayer(ByVal objLayer As cEcospaceLayerImportance) As Boolean
        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Me.ActiveEcospaceScenarioIndex <= 0 Then Return False
        If Not TypeOf (DataSource) Is IEcospaceDatasource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceImportanceLayer(objLayer.DBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace importance has been removed", _
                eMessageType.DataAddedOrRemoved, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function

#End Region ' Importance layers

#End Region ' Ecospace

#Region " Stanza "

    ''' <summary>
    ''' Initialize and populate the Stanza interface between the core and an interface
    ''' </summary>
    Private Function InitStanzas() As Boolean

        ' Now (re)generate CoreInterface objects.
        Try
            'clear out any old data
            m_stanzaGroups.Clear()

            'build the cStanzaGroup object for each Nsplit (stanza group)
            Dim tmpstanzaGrp As cStanzaGroup
            For i As Integer = 1 To m_Stanza.Nsplit

                tmpstanzaGrp = New cStanzaGroup(Me, Me.m_Stanza.StanzaDBID(i), m_Stanza.Nstanza(i), i)
                tmpstanzaGrp.AllowValidation = False
                tmpstanzaGrp.Index = i
                m_stanzaGroups.Add(tmpstanzaGrp)

            Next i

            'populate the stanza groups list with data from EcoSim (m_stanzaGroups) 
            LoadStanzas()

        Catch ex As Exception
            'make sure the core can still run if this thing explodes
            Debug.Assert(False, Me.ToString & ".InitStanza() " & ex.Message)
            Return False

        End Try
        Return True

    End Function

    Private Function LoadStanzas() As Boolean

        For Each stanza As cStanzaGroup In m_stanzaGroups

            LoadStanza(stanza)

        Next stanza

    End Function

    ''' <summary>
    ''' Populate a cStanzaGroup object with the core data
    ''' </summary>
    ''' <param name="stanza">cStanzaGroup object to populate.</param>
    ''' <returns>True is successfull. False otherwise.</returns>
    ''' <remarks>Call to populate a single cStanzaGroup object with the core data from the Ecopath and Stanza data structures</remarks>
    Friend Function LoadStanza(ByVal stanza As cStanzaGroup) As Boolean
        Try
            Dim iStanza As Integer = 0
            'iStanza is the index in the Ecosim stanza arrays that this cStanzaGroup object belongs to
            'Nstanza is the number of groups in this stanza

            stanza.AllowValidation = False

            'convert the Database ID into an iStanza
            iStanza = Array.IndexOf(m_Stanza.StanzaDBID, stanza.DBID)

            'jb Set the stanza index. This is not like other input-output objects 
            'The cStanzaGroup object uses the iStanza (Index) to figure out how many life stages (stanzas) are in this stanzagroup (m_Stanza.Nstanza(iStanza))
            'which it needs to size its internal array structures iGroup, BiomassAtAge,WeightAtAge and NumberAtAge
            'as well and its MaxAge property
            stanza.Index = iStanza
            stanza.Resize()

            stanza.Name = m_Stanza.StanzaName(iStanza)
            stanza.LeadingB = m_Stanza.BaseStanza(iStanza)
            stanza.LeadingCB = m_Stanza.BaseStanzaCB(iStanza)
            stanza.RecruitmentPower = m_Stanza.RecPowerSplit(iStanza)
            stanza.WmatWinf = m_Stanza.WmatWinf(iStanza)
            stanza.BiomassAccumulationRate = m_Stanza.BABsplit(iStanza)
            stanza.HatchCode = m_Stanza.HatchCode(iStanza)
            stanza.FixedFecundity = m_Stanza.FixedFecundity(iStanza)

            'stanza.VBGF = m_EcoPathData.vbKInput(m_Stanza.EcopathCode(iStanza, m_Stanza.BaseStanza(iStanza)))

            ' Array variables
            For j As Integer = 1 To m_Stanza.Nstanza(iStanza)
                stanza.StartAge(j) = m_Stanza.Age1(iStanza, j)
            Next

            For j As Integer = 1 To m_Stanza.Nstanza(iStanza)
                stanza.iGroups(j) = m_Stanza.EcopathCode(iStanza, j)
                stanza.Biomass(j) = m_EcoPathData.Binput(m_Stanza.EcopathCode(iStanza, j))
                stanza.Mortality(j) = m_EcoPathData.PBinput(m_Stanza.EcopathCode(iStanza, j))
                stanza.CB(j) = m_EcoPathData.QBinput(m_Stanza.EcopathCode(iStanza, j))
            Next j

            For iage As Integer = 1 To stanza.MaxAge ' the MaxAge of a stanza group is not available until the Index has been set
                'biomass at age is not used by ecosim it is compute when it is needed
                stanza.BiomassAtAge(iage) = m_Stanza.SplitWage(iStanza, iage) * m_Stanza.SplitNo(iStanza, iage)
                stanza.WeightAtAge(iage) = m_Stanza.SplitWage(iStanza, iage)
                stanza.NumberAtAge(iage) = m_Stanza.SplitNo(iStanza, iage)
            Next

            stanza.ResetStatusFlags()
            stanza.isDirty = False 'this needs to change 

            ' JS 18may07: discuss with JB why this is. Disabled datavalidation prohibits stanza changes to reach the core.
            'jb so that the user can change the values and run the stanza calculation to get the type of curve they want without saving the to the core
            'if the data is validated by the core this is difficult validation and updating are handled by the manager explicitly from the interface
            'data validation is turned off for stanza groups
            'stanza.AllowValidation = True

            Return True

        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex)
            Throw New ApplicationException("LoadStanza()", ex)
        End Try

    End Function

    ''' <summary>
    ''' Re-calculate Stanza variables from the new parameters in the cStanzaGroup object
    ''' </summary>
    ''' <param name="stanza">cStanzaGroup object that contains the new parameters and will be populated with the new values</param>
    ''' <returns>True if successfull. False otherwise.</returns>
    ''' <remarks>Calculates Biomass for all non leading stanzas, CB for non leading stanzas, WeightAtAge, NumberAtAge and BiomassAtAge.
    '''  It does not save the values or update the Ecopath variables that were affected. That is done via cStanzaGroup.Apply() </remarks>
    Friend Function CalculateStanza(ByVal stanza As cStanzaGroup) As Boolean
        Dim FirstAge() As Integer, SecondAge() As Integer
        Dim Bio() As Single, Z() As Single, cb() As Single, Bat() As Single
        Dim bSuccess As Boolean = True

        Try
            Dim iStanza As Integer = stanza.Index
            Dim nStanzas As Integer = stanza.NStanzas

            If (nStanzas <= 0) Then
                ' Fail calculations without making any changes
                Return False
            End If

            Dim i As Integer
            Dim orgVBK As Single = Me.EcoPathGroupInputs(stanza.iGroups(1)).VBK
            Dim iHatchCode As Integer = stanza.HatchCode
            Dim bFixedFecundity As Boolean = stanza.FixedFecundity

            Dim wmatwinf As Single
            Dim rp As Single
            Dim ba As Single


            ReDim Bio(nStanzas)
            ReDim Bat(nStanzas) 'in this case the Bat() is ignored so no need to populate it
            ReDim Z(nStanzas)
            ReDim cb(nStanzas)
            ReDim FirstAge(nStanzas)
            ReDim SecondAge(nStanzas) 'last month of age by spp, stanza (set in ecopath)

            If Not stanza.OkToCalculate Then
                'this stanza group has not had it parameters set B CB and Mort
                'Stanza parameters can not be calculated until this has been done by the interface
                ' ToDo_JS: Add cVariableStatuses for missing vars
                Me.m_publisher.SendMessage(New cMessage(String.Format(My.Resources.CoreMessages.STANZA_CALCULATEPARMS_TOOMANYMISSING, stanza.Name), _
                                            eMessageType.TooManyMissingParameters, eCoreComponentType.EcoPath, eMessageImportance.Warning, eDataTypes.Stanza))
                'maybe not the correct messagetype but it seems to work
                Return False
            End If

            wmatwinf = stanza.WmatWinf
            rp = stanza.RecruitmentPower
            ba = stanza.BiomassAccumulationRate

            For i = 1 To nStanzas
                Bio(i) = stanza.Biomass(i)
                Z(i) = stanza.Mortality(i)
                cb(i) = stanza.CB(i)
                FirstAge(i) = stanza.StartAge(i)
            Next

            If SecondAge(nStanzas) = 0 Then
                For i = 2 To nStanzas
                    SecondAge(i - 1) = FirstAge(i) - 1
                Next
                SecondAge(nStanzas) = CInt(Math.Log(1 - 0.9 ^ (1 / 3)) / (-orgVBK / 12))
                If SecondAge(nStanzas) > cCore.MAX_AGE Then SecondAge(nStanzas) = cCore.MAX_AGE
            End If

            'CalculateStanzaParameters() will update cStanzaDatastructure.SplitWage() and SplitNo() for this iStanza (as well a a bunch of other variables)
            bSuccess = m_EcoSim.CalculateStanzaParameters(iStanza, nStanzas, stanza.LeadingB, FirstAge, SecondAge, Bio, orgVBK, Z, _
                                                stanza.LeadingCB, cb, stanza.BiomassAccumulationRate, Bat)

            'set Age2() for the last life stage of this stanza group to the value calculated here and CalculateStanzaParameters() (why not just once in CalculateStanzaParameters?) 
            'In EwE5 this only happens in InitStanza here we need the value from Age2() for the interface EwE5 uses SecondAge()
            m_Stanza.Age2(iStanza, nStanzas) = SecondAge(nStanzas)


            'LoadStanza() will update WeightAtAge (SplitWage), NumberAtAge (SplitNo), BiomassAtAge (SplitWage*SplitNo)
            'with the new values computed by CalculateStanzaParameters() above
            'It will also overwrite variables entered by the user with the values from Ecopath
            LoadStanza(stanza)

            're-populate the variables that the user entered as arguments to CalculateStanzaParameters() 
            'that were over written by loadStanza()

            ' JS 25feb09: vbk stored in groups, unaffected by stanza calculations
            'StanzaGrp.VBGF = orgVBK

            ' Restore group
            stanza.AllowValidation = False

            For i = 1 To nStanzas
                stanza.Biomass(i) = Bio(i)
                stanza.Mortality(i) = Z(i)
                stanza.CB(i) = cb(i)
                stanza.StartAge(i) = FirstAge(i)
            Next

            stanza.WmatWinf = wmatwinf
            stanza.RecruitmentPower = rp
            stanza.BiomassAccumulationRate = ba
            stanza.HatchCode = iHatchCode
            stanza.FixedFecundity = bFixedFecundity

            'stanza.AllowValidation = True

            'this does not update the Ecopath variables that were also changed 
            'this is handled by OnChanged()
            'Ecopath.BInput(ieco) = Bio(i)
            'Ecopath.QBInput(ieco) = cb(i)
            'Ecopath.PBInput(ieco) = Z(i)

            'tell the interface that the stanza object has changed
            m_publisher.AddMessage(New cMessage("New Stanza parameters calculated.", eMessageType.DataModified, _
                        eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.Stanza))

            m_publisher.sendAllMessages()
            Return bSuccess

        Catch ex As Exception
            cLog.Write(ex)
            m_publisher.AddMessage(New cMessage("Error Calculating Stanza variables. " & ex.Message, eMessageType.ErrorEncountered, _
                                    eCoreComponentType.EcoPath, eMessageImportance.Critical, eDataTypes.Stanza))
            m_publisher.sendAllMessages()
            Return False
        End Try

    End Function

    Private Function UpdateStanza(ByVal iDBID As Integer) As Boolean

        Dim stanza As cStanzaGroup = Nothing
        'core array index of stanza
        Dim iStanza As Integer = Array.IndexOf(m_Stanza.StanzaDBID, iDBID)
        Dim bSucces As Boolean = (iStanza <> -1)

        Debug.Assert(bSucces)

        stanza = Me.StanzaGroups(iStanza - 1) 'stanza groups are kept in a zero based list by the core
        m_Stanza.StanzaName(iStanza) = stanza.Name
        m_Stanza.BaseStanza(iStanza) = stanza.LeadingB
        m_Stanza.BaseStanzaCB(iStanza) = stanza.LeadingCB
        m_Stanza.RecPowerSplit(iStanza) = stanza.RecruitmentPower
        m_Stanza.WmatWinf(iStanza) = stanza.WmatWinf
        m_Stanza.BABsplit(iStanza) = stanza.BiomassAccumulationRate
        m_Stanza.HatchCode(iStanza) = stanza.HatchCode
        m_Stanza.FixedFecundity(iStanza) = stanza.FixedFecundity

        m_Stanza.Nstanza(iStanza) = stanza.NStanzas
        For iLifeStage As Integer = 1 To stanza.NStanzas
            m_Stanza.EcopathCode(iStanza, iLifeStage) = stanza.iGroups(iLifeStage)
            m_Stanza.Age1(iStanza, iLifeStage) = stanza.StartAge(iLifeStage)

            ''update all the lifestages with the single vbK value EwE5 see frmGrpStanza.UpdateGroups
            'm_EcoPathData.vbKInput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.VBGF

            'Ecopath data that may have been changed by the stanza parameter calculations
            m_EcoPathData.Binput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Biomass(iLifeStage)
            m_EcoPathData.BHinput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Biomass(iLifeStage) * m_EcoPathData.Area(m_Stanza.EcopathCode(iStanza, iLifeStage))
            m_EcoPathData.QBinput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.CB(iLifeStage)
            m_EcoPathData.PBinput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Mortality(iLifeStage)
            m_EcoPathData.BA(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Biomass(iLifeStage) * stanza.BiomassAccumulationRate

        Next iLifeStage

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Adds a stanza group to the datasource.
    ''' </summary>
    ''' <param name="strStanzaName">Name to assign to new stanza group.</param>
    ''' <param name="aiGroupID">ID of the first <see cref="cEcoPathGroupInput">Ecopath group</see>
    ''' to assign to this mutli-stanza configuration.</param>
    ''' <param name="iDBID">Database ID assigned to the new stanza group.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>The EwE core cannot handle a situation where a stanza configuration
    ''' is defined without having any groups. To avoid this situation, this method
    ''' requires a valid <paramref name="iGroupID">group ID</paramref>.</remarks>
    ''' -----------------------------------------------------------------------
    Public Function AppendStanza(ByVal strStanzaName As String, ByVal aiGroupID() As Integer, ByVal aiStartAge() As Integer, ByRef iDBID As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        ' Append the stanza
        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.AppendStanza(strStanzaName, aiGroupID, aiStartAge, iDBID) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of stanza has changed.", eCoreComponentType.EcoPath, eDataTypes.Stanza)
            bSucces = True
        End If
        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a stanza group from the datasource.
    ''' </summary>
    ''' <param name="iStanza">Index of the stanza group to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveStanza(ByRef iStanza As Integer) As Boolean

        Dim iDBID As Integer = Me.m_Stanza.StanzaDBID(iStanza)
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return False
        If Not TypeOf (Me.DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        ' Remove the stanza
        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.RemoveStanza(iDBID) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of stanza has changed.", eCoreComponentType.EcoPath, eDataTypes.Stanza)
            bSucces = True
        End If
        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a group to a stanza configuration as a life stage.
    ''' </summary>
    ''' <param name="iStanza">Index of the stanza group to modify.</param>
    ''' <param name="iGroupDBID">Database if of the Group to assign as life stage.</param>
    ''' <param name="iAge">The age to assign to this life stage.</param>
    ''' <param name="sMortality"></param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddStanzaLifestage(ByVal iStanza As Integer, ByVal iGroupDBID As Integer, _
                                       ByVal iAge As Integer, ByVal sMortality As Single) As Boolean

        Dim iStanzaDBID As Integer = Me.m_Stanza.StanzaDBID(iStanza)
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        ' Remove the stanza
        ds = DirectCast(DataSource, IEcopathDataSource)
        bSucces = ds.AddStanzaLifestage(iStanzaDBID, iGroupDBID, iAge, sMortality)
        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a life stage from a stanza configuration.
    ''' </summary>
    ''' <param name="iStanza">Index of the stanza configuration to adjust.</param>
    ''' <param name="iGroupDBID">Database ID of group to remove as a life stage.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveStanzaLifestage(ByVal iStanza As Integer, _
                                          ByVal iGroupDBID As Integer) As Boolean
        Dim iStanzaDBID As Integer = Me.m_Stanza.StanzaDBID(iStanza)
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcopathDataSource Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        ' Remove the stanza
        ds = DirectCast(DataSource, IEcopathDataSource)
        bSucces = ds.RemoveStanzaLifestage(iStanzaDBID, iGroupDBID)
        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces
    End Function

#End Region ' Stanza

#Region "Monte Carlo"

    ''' <summary>
    ''' Get the Ecosim<see cref="cMonteCarloManager">Monte Carlo manager</see>.
    ''' </summary>
    Public ReadOnly Property EcosimMonteCarlo() As cMonteCarloManager
        Get
            Return Me.m_MonteCarlo
        End Get
    End Property

#End Region ' Monte carlo

#Region "Fit to time series "

    ''' <summary>
    ''' Get the Ecosim <see cref="cF2TSManager">Fit to Time Series manager</see>.
    ''' </summary>
    Public ReadOnly Property EcosimFitToTimeSeries() As cF2TSManager
        Get
            Return DirectCast(Me.m_SearchManagers(eDataTypes.FitToTimeSeries), cF2TSManager)
        End Get
    End Property

#End Region ' Fit to time series

#Region " Ecotracer "

#Region " Variables "

    Private m_EcotracerScenarios As New cCoreInputOutputList(Of cEcotracerScenario)(eDataTypes.EcotracerScenario, 1)
    Private m_EcotracerGroupInputs As New cCoreInputOutputList(Of cEcotracerGroupInput)(eDataTypes.EcotracerGroupInput, 1)
    Private m_EcotracerModelParameters As cEcotracerModelParameters
    Private m_EcotracerGroupOutput As cEcotracerGroupOutput
    Private m_EcotracerRegionGroupOutput As cEcotracerRegionGroupOutput

#End Region ' Variables 

#Region " Scenarios "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of available Ecotracer scenarios in the current loaded
    ''' model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcotracerScenarioCount() As Integer
        Get
            Try
                ' Return Ecopath administration number here instead of counting UI items
                Return Me.m_EcoPathData.NumEcotracerScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get an Ecotracer scenario.
    ''' </summary>
    ''' <param name="iScenario">
    ''' One-based index of the scenario to obtain. This value cannot exceed 
    ''' <see cref="EcotracerScenarioCount">EcotracerScenarioCount</see>.
    ''' </param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcotracerScenarios(ByVal iScenario As Integer) As cEcotracerScenario
        Get
            Return Me.m_EcotracerScenarios(iScenario)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the index of the active <see cref="cEcotracerScenario">Ecotracer scenario</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveEcotracerScenarioIndex() As Integer
        Get
            Return Me.m_EcoPathData.ActiveEcotracerScenario
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates and loads a new Ecotracer scenario.
    ''' </summary>
    ''' <param name="strName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <param name="strAuthor">Author to assign to new scenario.</param>
    ''' <param name="strContact">Contact to assign to new scenario.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function NewEcotracerScenario(ByVal strName As String, _
                                         ByVal strDescription As String, _
                                         ByVal strAuthor As String, _
                                         ByVal strContact As String) As Boolean

        Dim ds As IEcotracerDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim iScenario As Integer = 0

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcotracerDatasource Then Return False

        If Me.m_StateMonitor.HasEcopathLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecotracer) Then Return False

        Try

            ds = DirectCast(DataSource, IEcotracerDatasource)
            If (ds.AppendEcotracerScenario(strName, strDescription, strAuthor, strContact, iScenarioID)) Then
                Me.StateMonitor.UpdateDataState(Me.m_DataSource)
                Me.InitEcotracerScenarios()
                Me.DataAddedOrRemovedMessage("Ecotracer number of scenarios has changed.", eCoreComponentType.Ecotracer, eDataTypes.EcotracerScenario)
                iScenario = Array.IndexOf(Me.m_EcoPathData.EcotracerScenarioDBID, iScenarioID)
                Return Me.LoadEcotracerScenario(iScenario)
            End If

            Return False
        Catch ex As Exception

        End Try
        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load an <see cref="cEcoSimScenario">Ecotracer scenario</see> from the 
    ''' current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcotracerScenario">Scenario</see> to load.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadEcotracerScenario(ByRef scenario As cEcotracerScenario) As Boolean
        Return LoadEcotracerScenario(scenario.Index)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load an <see cref="cEcotracerScenario">Ecotracer scenario</see> 
    ''' from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the 
    ''' <see cref="cEcotracerScenario">Scenario</see> in the 
    ''' <see cref="m_EcotracerScenarios">Scenario list</see>.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadEcotracerScenario(ByVal iScenario As Integer) As Boolean

        Dim ds As IEcotracerDatasource = Nothing
        Dim bSuccess As Boolean = True

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcotracerDatasource Then Return False

        Try

            'For an Ecotracer scenario to load there must be an Ecosim scenario loaded
            If Not Me.m_StateMonitor.HasEcosimLoaded() Then
                Debug.Assert(False, "LoadEcotracerScenario() Load  Ecosim first. This is temporary.")
                Return False
            End If

            ' Update core state
            Me.m_StateMonitor.SetEcotracerLoaded(False)

            ds = DirectCast(DataSource, IEcotracerDatasource)
            If Not ds.LoadEcotracerScenario(Me.m_EcoPathData.EcotracerScenarioDBID(iScenario)) Then
                Debug.Assert(False, "LoadEcotracerScenario() Failed to load scenario from data source.")
                SendEcospaceLoadMessage(iScenario, My.Resources.CoreMessages.ECOTRACER_LOAD_FAILED)
                Return False
            End If

            bSuccess = Me.InitEcotracerModelParamaters()
            bSuccess = bSuccess And InitEcotracerGroups()

            InitEcotracerOutputs()

            ' Reset ecosim model params for consimon flag
            Me.m_EcoSimRun.ResetStatusFlags()

            SendEcotracerLoadMessage(iScenario)

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.LoadEcotracerScenario(ds)
            ' Update core state
            Me.m_StateMonitor.SetEcotracerLoaded(bSuccess)

        Catch ex As Exception
            cLog.Write(Me.ToString & ".LoadEcotracerScenario(...) Error: " & ex.Message)
            SendEcotracerLoadMessage(iScenario, ex.Message)
            Debug.Assert(False, ex.Message)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the current Ecotracer scenario.
    ''' </summary>
    ''' <param name="scenario">A scenario to overwrite, if any.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcotracerScenario(Optional ByVal scenario As cEcotracerScenario = Nothing) As Boolean
        Dim iScenarioID As Integer = 0
        Dim ds As IEcotracerDatasource = Nothing

        ' Sanity checks
        If DataSource Is Nothing Then Return False
        If Not TypeOf (DataSource) Is IEcotracerDatasource Then Return False

        ' Overwrite scenario?
        If scenario IsNot Nothing Then
            iScenarioID = scenario.DBID
        Else
            iScenarioID = m_EcoPathData.EcotracerScenarioDBID(m_EcoPathData.ActiveEcotracerScenario)
        End If

        Debug.Assert(iScenarioID > 0)

        ' Save ok?
        ds = DirectCast(DataSource, IEcotracerDatasource)
        If (ds.SaveEcotracerScenario(iScenarioID)) Then
            ' #Yes: Reload scenarios
            Me.InitEcotracerScenarios()
            ' Update active scenario ID
            Me.m_EcoPathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcoPathData.EcotracerScenarioDBID, iScenarioID)
            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcotracerScenario(Me)
            ' Force update
            Me.m_StateMonitor.SetEcotracerLoaded(True, TriState.True)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Report succes
            SendEcotracerSaveStateMessage(Me.m_EcoPathData.EcotracerScenarioName(Me.ActiveEcotracerScenarioIndex))
            Return True
        End If

        ' Restore active scenario ID
        Me.m_EcoPathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcoPathData.EcotracerScenarioDBID, iScenarioID)

        ' Report failure
        SendEcotracerSaveStateMessage(Me.m_EcoPathData.EcotracerScenarioName(Me.ActiveEcotracerScenarioIndex), False, _
                My.Resources.CoreMessages.GENERIC_SAVE_RESOLUTION)

        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the current ecotracer scenario under a new name.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcotracerScenarioAs(ByVal strName As String, _
                                            ByVal strDescription As String) As Boolean

        Dim epd As cEcopathDataStructures = Me.m_EcoPathData
        Dim ds As IEcotracerDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcotracerDatasource) Then Return False
        If (Me.m_EcoPathData.ActiveEcotracerScenario <= 0) Then Return False

        iScenarioID = Me.m_EcoPathData.EcotracerScenarioDBID(Me.m_EcoPathData.ActiveEcotracerScenario)
        If (iScenarioID <= 0) Then Return False

        ' Save ok?
        ds = DirectCast(DataSource, IEcotracerDatasource)
        If (ds.AppendEcotracerScenario(strName, strDescription, _
                epd.EcotracerScenarioAuthor(Me.m_EcoPathData.ActiveEcotracerScenario), _
                epd.EcotracerScenarioContact(Me.m_EcoPathData.ActiveEcotracerScenario), _
                iScenarioID)) Then

            ' #Yes: Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcotracerScenario(Me)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)

            ' Reload scenarios
            Me.InitEcotracerScenarios()

            ' Inform the world
            Me.SendEcotracerSaveStateMessage(strName)
            ' Load Ecospace scenario
            bSucces = Me.LoadEcotracerScenario(Array.IndexOf(epd.EcotracerScenarioDBID, iScenarioID))
            Me.DataAddedOrRemovedMessage("Ecotracer number of scenarios has changed.", eCoreComponentType.Ecotracer, eDataTypes.EcotracerScenario)
            Return bSucces
        End If

        ' Restore active scenario ID
        Me.m_EcoPathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcoPathData.EcotracerScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcotracerSaveStateMessage(strName, False)
        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cEcotracerScenario">Ecotracer Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcotracerScenario">Scenario</see> to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcotracerScenario(ByVal scenario As cEcotracerScenario) As Boolean
        Return Me.RemoveEcosimScenario(scenario.Index)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cEcotracerScenario">Ecotracer Scenario</see> from 
    ''' the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario in the 
    ''' <see cref="m_EcotracerScenarios">Ecotracer Scenario list</see>.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcotracerScenario(ByVal iScenario As Integer) As Boolean

        Dim ds As IEcotracerDatasource = Nothing
        Dim iScenarioID As Integer = cCore.NULL_VALUE
        Dim bSucces As Boolean = False

        ' Sanity check
        Debug.Assert(iScenario > 0 And iScenario < Me.m_EcoPathData.EcotracerScenarioDBID.Length)

        ' Cannot delete a loaded scenario
        If (iScenario = Me.m_EcoPathData.ActiveEcotracerScenario) Then
            Me.m_publisher.SendMessage(New cMessage("Cannot delete a loaded scenario", eMessageType.NotSet, eCoreComponentType.DataSource, eMessageImportance.Warning))
            Return False
        End If

        ' Save pending relevant changes
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecotracer) Then Return False

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcotracerDatasource) Then Return False

        ' Remember scenario ID to restore
        If (Me.m_EcoPathData.ActiveEcotracerScenario > 0) Then
            iScenarioID = Me.m_EcoPathData.EcotracerScenarioDBID(Me.m_EcoPathData.ActiveEcotracerScenario)
        End If

        ds = DirectCast(Me.DataSource, IEcotracerDatasource)
        ' Scenario removed succesfully?
        If ds.RemoveEcotracerScenario(Me.m_EcoPathData.EcotracerScenarioDBID(iScenario)) Then
            ' #Yes: reload scenario list
            bSucces = Me.InitEcotracerScenarios()
            ' Restore active scenario ID
            Me.m_EcoPathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcoPathData.EcotracerScenarioDBID, iScenarioID)
            ' Broadcast change
            Me.DataAddedOrRemovedMessage("Ecotracer number of scenarios has changed.", eCoreComponentType.Ecotracer, eDataTypes.EcotracerScenario)
        End If
        ' Return succes
        Return bSucces
    End Function

#Region " Internals "

    Private Function InitEcotracerScenarios() As Boolean
        Me.m_EcotracerScenarios.Clear()
        For i As Integer = 1 To Me.m_EcoPathData.EcotracerScenarioDBID.Length - 1
            Me.m_EcotracerScenarios.Add(New cEcotracerScenario(Me))
            Me.InitEcotracerScenario(i)
        Next
        Return True
    End Function

    Private Function InitEcotracerScenario(ByVal iScenario As Integer) As Boolean

        Dim ets As cEcotracerScenario = Me.m_EcotracerScenarios(iScenario)
        Try
            ets.AllowValidation = False

            ets.DBID = m_EcoPathData.EcotracerScenarioDBID(iScenario)
            ets.Name = m_EcoPathData.EcotracerScenarioName(iScenario)
            ets.Author = m_EcoPathData.EcotracerScenarioAuthor(iScenario)
            ets.Contact = m_EcoPathData.EcotracerScenarioContact(iScenario)
            ets.LastSaved = m_EcoPathData.EcotracerScenarioLastSaved(iScenario)
            ets.Index = iScenario
            ets.ResetStatusFlags()

            ets.AllowValidation = True

        Catch ex As Exception
            cLog.Write(Me.ToString & ".cEcotracerScenario() Error: " & ex.Message)
            Debug.Assert(False, "Error Getting cEcotracerScenario Info: " & ex.Message)
            Return Nothing
        End Try
        Return True

    End Function

    Private Function UpdateEcotracerScenario(ByVal iDBID As Integer) As Boolean

        Dim iScenario As Integer = Array.IndexOf(Me.m_EcoPathData.EcotracerScenarioDBID, iDBID)
        Dim scn As cEcotracerScenario = Me.EcotracerScenarios(iScenario)

        Try
            Me.m_EcoPathData.EcotracerScenarioName(iScenario) = scn.Name
            Me.m_EcoPathData.EcotracerScenarioDescription(iScenario) = scn.Description
            Me.m_EcoPathData.EcotracerScenarioAuthor(iScenario) = scn.Author
            Me.m_EcoPathData.EcotracerScenarioContact(iScenario) = scn.Contact
            ' Do not update last saved date; this is exclusively set by the core when saving

        Catch ex As Exception
            cLog.Write(Me.ToString & ".privateEcotracerScenario() EcoSim parameters will not be set Error: " & ex.Message)
            Debug.Assert(False, "cEcotracerScenario Info will not be set Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function

    Private Sub InitEcotracerOutputs()
        Me.m_EcotracerGroupOutput = New cEcotracerGroupOutput(Me)
        Me.m_EcotracerRegionGroupOutput = New cEcotracerRegionGroupOutput(Me, Me.m_tracerData)
    End Sub

    Public ReadOnly Property EcotracerGroupResults() As cEcotracerGroupOutput
        Get
            Return Me.m_EcotracerGroupOutput
        End Get
    End Property

    Public ReadOnly Property EcotracerRegionGroupResults() As cEcotracerRegionGroupOutput
        Get
            Return Me.m_EcotracerRegionGroupOutput
        End Get
    End Property

    Private Sub SendEcotracerLoadMessage(ByVal iScenario As Integer, Optional ByVal strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = String.Format(My.Resources.CoreMessages.ECOTRACER_LOAD_SUCCESS, Me.m_EcoPathData.EcotracerScenarioName(iScenario))
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecotracer, eMessageImportance.Information)
        Else
            strText = String.Format(My.Resources.CoreMessages.ECOTRACER_LOAD_FAILED, Me.m_EcoPathData.EcotracerScenarioName(iScenario), strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecotracer, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()

    End Sub

    Private Sub SendEcotracerSaveStateMessage(ByVal strScenarioName As String, Optional ByVal bSucces As Boolean = True, _
            Optional ByVal strError As String = "")

        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If bSucces Then
            strText = String.Format(My.Resources.CoreMessages.ECOTRACER_SAVE_SUCCES, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataModified, eCoreComponentType.Ecotracer, eMessageImportance.Information)
        Else
            strText = String.Format(My.Resources.CoreMessages.ECOTRACER_SAVE_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecotracer, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()
    End Sub

#End Region ' Internals

#End Region ' Scenarios

#Region " ModelParameters "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the model parameters object for the current loaded Ecotracer scenario.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcotracerModelParameters() As cEcotracerModelParameters
        Get
            Return Me.m_EcotracerModelParameters
        End Get
    End Property

#Region " Internals "

    Private Function InitEcotracerModelParamaters() As Boolean
        Me.m_EcotracerModelParameters = New cEcotracerModelParameters(Me)
        Return Me.LoadEcotracerModelParameters()
    End Function

    Private Function LoadEcotracerModelParameters() As Boolean

        Try
            Me.m_EcotracerModelParameters.AllowValidation = False

            Me.m_EcotracerModelParameters.CZero = Me.m_tracerData.Czero(0)
            Me.m_EcotracerModelParameters.CInflow = Me.m_tracerData.Cinflow(0)
            Me.m_EcotracerModelParameters.COutflow = Me.m_tracerData.CoutFlow(0)
            Me.m_EcotracerModelParameters.CDecay = Me.m_tracerData.cdecay(0)
            Me.m_EcotracerModelParameters.ConForceNumber = Me.m_tracerData.ConForceNumber

            Me.m_EcotracerModelParameters.ResetStatusFlags()
            Me.m_EcotracerModelParameters.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function UpdateEcotracerModelParameters() As Boolean

        Try
            Me.m_tracerData.Czero(0) = Me.m_EcotracerModelParameters.CZero
            Me.m_tracerData.Cinflow(0) = Me.m_EcotracerModelParameters.CInflow
            Me.m_tracerData.CoutFlow(0) = Me.m_EcotracerModelParameters.COutflow
            Me.m_tracerData.cdecay(0) = Me.m_EcotracerModelParameters.CDecay
            Me.m_tracerData.ConForceNumber = Me.m_EcotracerModelParameters.ConForceNumber

        Catch ex As Exception
            cLog.Write(Me.ToString & ".EcoSimModelRunParameters() EcoSim Parameters will not be set Error: " & ex.Message)
            Debug.Assert(False, "EcoSim Parameters will not be set Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function

#End Region ' Internals

#End Region ' ModelParameters

#Region " Groups "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get an Ecotracer <see cref="cEcotracerGroupInput">group input</see>.
    ''' </summary>
    ''' <param name="iGroup">One-based index of the group to obtain. This value
    ''' cannot exceed <see cref="nGroups">nGroups</see>.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcotracerGroupInputs(ByVal iGroup As Integer) As cEcotracerGroupInput
        Get
            Return Me.m_EcotracerGroupInputs(iGroup)
        End Get
    End Property

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize <see cref="cEcotracerGroupInput">Ecotracer group</see> objects to
    ''' expose to the interface layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function InitEcotracerGroups() As Boolean

        Dim grp As cEcotracerGroupInput = Nothing

        Try

            m_EcotracerGroupInputs.Clear()

            'populate the list of cEcoSimGroupInfo objects that the user will interact with 
            'to change group related parameters from the interface see getEcoSimGroupInfo(iGroup)
            For i As Integer = 1 To nGroups
                ' Create group
                grp = New cEcotracerGroupInput(Me, Me.m_EcoPathData.GroupDBID(i))
                ' Add to list
                m_EcotracerGroupInputs.Add(grp)
            Next i

            ' Load the Ecotracer data into the objects created above
            Return LoadEcotracerGroups()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcotracerGroups() Error: " & ex.Message)
            Return False
        End Try


    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load Ecotracer group data from the underlying data structures into the 
    ''' interface objects.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcotracerGroups() As Boolean

        Dim iGroup As Integer

        Try

            For Each grp As cEcotracerGroupInput In Me.m_EcotracerGroupInputs

                'convert the Database ID into an iGroup
                iGroup = Array.IndexOf(Me.m_EcoPathData.GroupDBID, grp.DBID)

                Debug.Assert(iGroup > 0 And iGroup <= Me.nGroups, "LoadEcotracerGroups() failed to find iGroup for Ecotracer DBID.")

                grp.Resize()

                grp.AllowValidation = False

                grp.Index = iGroup
                grp.Name = m_EcoPathData.GroupName(iGroup)
                grp.CZero = Me.m_tracerData.Czero(iGroup)
                grp.CImmig = Me.m_tracerData.Cimmig(iGroup)
                grp.CEnvironment = Me.m_tracerData.Cenv(iGroup)
                grp.CDecay = Me.m_tracerData.cdecay(iGroup)
                grp.CExcretionRate = Me.m_tracerData.CexcretionRate(iGroup)

                grp.ResetStatusFlags()
                grp.AllowValidation = True

            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcotracerGroups() Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function

    Private Function UpdateEcotracerGroup(ByVal iDBID As Integer) As Boolean

        Dim grp As cEcotracerGroupInput = Nothing
        Dim iGroup As Integer

        Try

            ' Convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcoPathData.GroupDBID, iDBID)
            ' Get the group
            grp = Me.m_EcotracerGroupInputs(iGroup)
            ' Read it
            Me.m_tracerData.Czero(iGroup) = grp.CZero
            Me.m_tracerData.Cimmig(iGroup) = grp.CImmig
            Me.m_tracerData.Cenv(iGroup) = grp.CEnvironment
            Me.m_tracerData.cdecay(iGroup) = grp.CDecay
            Me.m_tracerData.CexcretionRate(iGroup) = grp.CExcretionRate

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateEcotracerGroup() Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function


    Private Sub loadEcoTracerResults()

        Try

            If m_tracerData.EcoSimConSimOn Then
                Debug.Assert(Me.m_EcotracerGroupOutput IsNot Nothing, "Ecotracer can not load results.")

                For igrp As Integer = 0 To nGroups
                    For it As Integer = 1 To nEcosimTimeSteps
                        m_EcotracerGroupOutput.Concentration(igrp, it) = m_tracerData.TracerConc(igrp, it)
                    Next
                Next

            End If ' If m_tracerData.EcoSimConSimOn Then

            If m_tracerData.EcoSpaceConSimOn Then
                Debug.Assert(Me.m_EcotracerRegionGroupOutput IsNot Nothing, "Ecotracer can not load results.")

                'For irgn As Integer = 0 To nRegions
                '    For it As Integer = 1 To nEcospaceTimeSteps

                '        'For igrp As Integer = 1 To nGroups
                '        '    m_EcotracerRegionGroupOutput.Concentration(irgn, igrp, it) = m_tracerData.TracerConcByRegion(irgn, igrp, it)
                '        '    m_EcotracerRegionGroupOutput.CB(irgn, igrp, it) = m_tracerData.TracerCBRegion(irgn, igrp, it)
                '        'Next igrp

                '        ''environment values are stored in the zero group element
                '        'm_EcotracerRegionGroupOutput.CEnvironment(irgn, it) = m_tracerData.TracerConcByRegion(irgn, 0, it)
                '        'm_EcotracerRegionGroupOutput.CBEnvironment(irgn, it) = m_tracerData.TracerCBRegion(irgn, 0, it)

                '    Next it
                'Next irgn
            End If 'If m_tracerData.EcoSpaceConSimOn Then

        Catch ex As Exception
            cLog.Write(ex)
            'for now just assert this should send a message that the tracer results could not be loaded
            Debug.Assert(False, ex.StackTrace)
        End Try

    End Sub

#End Region ' Internals

#End Region ' Groups

#End Region ' Ecotracer

#Region " Data adapters "

    'Public ReadOnly Property EconomicDataAdapter() As cEconomicDataAdapter
    '    Get
    '        Return Me.m_adapterEconomic
    '    End Get
    'End Property

#End Region ' Data adapters

#Region " Auxiliary data "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set an<see cref="cAuxiliaryData">AuxillaryData</see> instance 
    ''' for a given <see cref="cValueID">value ID</see>.
    ''' </summary>
    ''' <param name="key">The unique <see cref="cValueID">value ID</see>.</param>
    ''' <returns>An cAuxillaryData instance.</returns>
    ''' -------------------------------------------------------------------
    Public Property AuxillaryData(ByVal key As cValueID) As cAuxiliaryData
        Get
            Return Me.AuxillaryData(key.ToString)
        End Get
        Friend Set(ByVal value As cAuxiliaryData)
            Me.AuxillaryData(key.ToString) = value
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; looks up - or creates when non-existing - an 
    ''' <see cref="cAuxiliaryData">AuxillaryData</see> instance for a value.
    ''' </summary>
    ''' <param name="strValueID">The unique <see cref="cValueID">value ID</see>.</param>
    ''' <returns>An cAuxillaryData instance.</returns>
    ''' -------------------------------------------------------------------
    Public Property AuxillaryData(ByVal strValueID As String) As cAuxiliaryData

        Get
            If (String.IsNullOrEmpty(strValueID)) Then Return Nothing

            Dim ad As cAuxiliaryData = Nothing
            If (Not Me.m_dtAuxiliaryData.ContainsKey(strValueID)) Then
                Me.AuxillaryData(strValueID) = New cAuxiliaryData(Me, strValueID)
            End If
            Return Me.m_dtAuxiliaryData(strValueID)
        End Get

        Set(ByVal ad As cAuxiliaryData)
            If (String.IsNullOrEmpty(strValueID)) Then Return

            If (ad Is Nothing) Then
                If (Me.m_dtAuxiliaryData.ContainsKey(strValueID)) Then
                    Me.m_dtAuxiliaryData.Remove(strValueID)
                End If
            Else
                Me.m_dtAuxiliaryData(strValueID) = ad
            End If
        End Set

    End Property

#Region " Pedigree "

    Private Function InitPedigreeManagers() As Boolean

        Dim manager As cPedigreeManager = Nothing
        Dim level As cPedigreeLevel = Nothing

        ' Create managers
        Me.m_PedigreeManagers = New Dictionary(Of eVarNameFlags, cPedigreeManager)
        For Each vn As eVarNameFlags In cPedigreeManager.SupportVarNames
            manager = New cPedigreeManager(Me, vn)
            manager.Load()
            Me.m_PedigreeManagers(vn) = manager
        Next
        Return True

    End Function

    Public Function GetPedigreeManager(ByVal varName As eVarNameFlags) As cPedigreeManager
        If Me.m_PedigreeManagers.ContainsKey(varName) Then Return Me.m_PedigreeManagers(varName)
        Return Nothing
    End Function

#End Region ' Pedigree

#End Region ' Auxillary data

#Region " Variable validation "

    ''' <summary>
    ''' The one point where cCoreInputOutputBase objects report validated data.
    ''' </summary>
    ''' <param name="value">The value that passed or failed validation.</param>
    ''' <param name="objValidated">The object this value belongs to.</param>
    Friend Sub OnValidated(ByRef value As cValue, ByRef objValidated As cCoreInputOutputBase)

        Dim bValidatedOk As Boolean = ((value.ValidationStatus And eStatusFlags.FailedValidation) = 0)
        Dim dtAffected As eDataTypes = eDataTypes.NotSet
        Dim idAffected As Integer = 0
        Dim msAffected As eCoreComponentType = eCoreComponentType.NotSet
        Dim rsAffected As eCoreExecutionState = eCoreExecutionState.Idle
        Dim bBlock As Boolean = False

        'Dim objAffected As cCoreInputOutputBase = Nothing
        Dim msg As cMessage = Nothing

        ' Prepare main validation message
        msg = New cMessage(value.ValidationMessage, eMessageType.DataValidation, objValidated.CoreComponent, eMessageImportance.Information, objValidated.DataType)
        ' JS 27sep07: validation success messages are maintenance messages now; the user does not need to see these.
        If bValidatedOk Then msg.Importance = eMessageImportance.Maintenance

        msg.AddVariable(New cVariableStatus(objValidated.ValidationStatus))

        ' Give the core a chance to respond to successfull edits
        ' JS070306: added the option to flag any affected object as changed
        If bValidatedOk Then PostVariableValidation(value, objValidated, msg)

        ' Handle all affected objects
        For Each vs As cVariableStatus In msg.Variables

            dtAffected = DirectCast(vs.CoreDataObject, cCoreInputOutputBase).DataType
            idAffected = DirectCast(vs.CoreDataObject, cCoreInputOutputBase).DBID
            msAffected = DirectCast(vs.CoreDataObject, cCoreInputOutputBase).CoreComponent

            Select Case dtAffected

                Case eDataTypes.EwEModel
                    If bValidatedOk Then Me.UpdateEwEModel()

                Case eDataTypes.EcoPathGroupInput
                    If bValidatedOk Then Me.UpdateEcopathInput(idAffected)
                    ' Special cases: name and colour changes will not affect the Ecopath execution state!
                    ' Reroute these changes to the model itself
                    If vs.VarName = eVarNameFlags.Name Or vs.VarName = eVarNameFlags.PoolColor Then
                        msAffected = eCoreComponentType.DataSource
                    End If

                Case eDataTypes.Taxon
                    If bValidatedOk Then Me.UpdateTaxon(idAffected)

                Case eDataTypes.FleetInput
                    If bValidatedOk Then Me.UpdateFleetInput(idAffected)
                    ' Special cases: name and colour changes will not affect the Ecopath execution state!
                    ' Reroute these changes to the model itself
                    If vs.VarName = eVarNameFlags.Name Or vs.VarName = eVarNameFlags.PoolColor Then
                        msAffected = eCoreComponentType.DataSource
                    End If

                Case eDataTypes.Stanza
                    If bValidatedOk Then Me.UpdateStanza(idAffected)

                Case eDataTypes.ParticleSizeDistribution
                    If bValidatedOk Then Me.UpdatePSDParameters()

                Case eDataTypes.EcoSimGroupInput
                    If bValidatedOk Then Me.UpdateEcoSimGroup(idAffected)

                Case eDataTypes.EcosimFleetInput
                    If bValidatedOk Then Me.UpdateEcoSimFleetInput(idAffected)

                Case eDataTypes.EcoSimModelParameter
                    If bValidatedOk Then Me.UpdateEcoSimModelParameters()

                Case eDataTypes.EcoSimScenario
                    If bValidatedOk Then Me.UpdateEcoSimScenario(idAffected)

                Case eDataTypes.EcoSpaceScenario
                    If bValidatedOk Then Me.UpdateEcospaceScenario(idAffected)

                Case eDataTypes.Forcing, _
                     eDataTypes.EggProd, _
                     eDataTypes.Mediation, _
                     eDataTypes.FishingEffort, _
                     eDataTypes.FishMort
                    ' VERIFY_JS: This line of code is never hit?
                    msAffected = eCoreComponentType.ShapesManager

                Case eDataTypes.EcospaceBasemap
                    If bValidatedOk Then Me.UpdateEcospaceBasemap()

                Case eDataTypes.EcospaceLayerImportance
                    If bValidatedOk Then Me.UpdateEcospaceImportanceLayers()

                Case eDataTypes.EcospaceModelParameter
                    If bValidatedOk Then Me.UpdateEcospaceModelParameters()

                Case eDataTypes.EcospaceHabitat
                    If bValidatedOk Then Me.UpdateEcospaceHabitat(idAffected)

                Case eDataTypes.EcospaceRegion
                    If bValidatedOk Then Me.UpdateEcospaceRegion(idAffected)

                Case eDataTypes.EcospaceMPA
                    If bValidatedOk Then Me.UpdateEcospaceMPA(idAffected)

                Case eDataTypes.EcospaceGroup
                    If bValidatedOk Then Me.UpdateEcospaceGroup(idAffected)

                Case eDataTypes.EcospaceFleet
                    If bValidatedOk Then Me.UpdateEcospaceFleet(idAffected)

                Case eDataTypes.EcotracerScenario
                    If bValidatedOk Then Me.UpdateEcotracerScenario(idAffected)

                Case eDataTypes.EcotracerModelParameters
                    If bValidatedOk Then Me.UpdateEcotracerModelParameters()

                Case eDataTypes.EcotracerGroupInput
                    If bValidatedOk Then Me.UpdateEcotracerGroup(idAffected)

                Case eDataTypes.MSEFleetInput, eDataTypes.MSEGroupInput, eDataTypes.MSEParameters
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.MSEManager).Update(dtAffected)

                Case eDataTypes.FishingPolicyManager, eDataTypes.FishingPolicyParameters, _
                            eDataTypes.FishingPolicySearchBlocks
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.FishingPolicyManager).Update(dtAffected)

                Case eDataTypes.SearchObjectiveFleetInput, eDataTypes.SearchObjectiveGroupInput, _
                        eDataTypes.SearchObjectiveWeights, eDataTypes.SearchObjectiveParameters
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.SearchObjectiveManager).Update(dtAffected)

                Case eDataTypes.MPAOptManager, eDataTypes.MPAOptOuput, eDataTypes.MPAOptParameters
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.MPAOptManager).Update(dtAffected)

                Case eDataTypes.FitToTimeSeries
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.FitToTimeSeries).Update(dtAffected)

            End Select

            ' Data processed ok?
            If bValidatedOk Then

                ' Notify plug-ins
                Try
                    If Not Object.ReferenceEquals(Me.PluginManager, Nothing) Then
                        Me.PluginManager.DataValidated(vs.VarName, dtAffected)
                    End If
                Catch ex As Exception
                    ' NOP
                End Try

                ' Dirty datasource
                If Not Object.ReferenceEquals(DataSource, Nothing) Then

                    ' Block non-stored variables from dirtying the datasource
                    bBlock = (value.Stored = False)

                    ' Block cascaded name changes for groups and fleets
                    If (value.varName = eVarNameFlags.Name) Then
                        bBlock = bBlock Or _
                                 dtAffected = eDataTypes.EcoPathGroupOutput Or _
                                 dtAffected = eDataTypes.EcoSimGroupInput Or _
                                 dtAffected = eDataTypes.EcospaceGroup Or _
                                 dtAffected = eDataTypes.EcospaceFleet
                    End If

                    ' Data not blocked?
                    If Not bBlock Then
                        ' #Yes: dirty the data source
                        DataSource.SetChanged(msAffected)
                        ' Notify state monitor of data modification
                        Me.m_StateMonitor.RegisterModification(msAffected)
                    End If
                End If

                ' Update core run state:
                ' Block selected variables from affecting the core run state
                bBlock = (value.AffectsRunState = False)

                If (Not bBlock) Then
                    ' Update state monitor execution state
                    Me.m_StateMonitor.UpdateExecutionState(msAffected, _
                        DirectCast(IIf(Me.m_batchLockType = eBatchLockType.NotSet, TriState.UseDefault, TriState.False), TriState))
                End If

            End If
        Next

        If bValidatedOk Then
            PostVariableUpdated(value, objValidated)
        End If

        ' Send only notifications when NO lock active
        Me.m_StateMonitor.UpdateDataState(DataSource, _
            DirectCast(IIf(Me.m_batchLockType = eBatchLockType.NotSet, TriState.UseDefault, TriState.False), TriState))

        ' Send all messages
        Me.m_publisher.AddMessage(msg)
        Me.m_publisher.sendAllMessages()

    End Sub


    ''' <summary>
    ''' Have the core Validate a cValue object
    ''' </summary>
    ''' <param name="ValueObject">cValue Object to validate</param>
    ''' <param name="MetaData">Meta data associated with the cValue object</param>
    ''' <param name="iSecondaryIndex"></param>
    ''' <returns>True if the validation was run. False if the validation routine failed to run</returns>
    ''' <remarks>Ther results of the validation are in the cValue Object</remarks>
    Friend Function Validate(ByRef ValueObject As cValue, ByRef MetaData As cVariableMetaData, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean

        Dim cni As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

        'For now the validation is done right here (inline)
        'if this gets to bulky the core can call another routine to do the validation for different variables
        Select Case ValueObject.varName

            Case eVarNameFlags.MSEFleetWeight
                'Can not set FleetWeight if this is not a valid fleet
                Dim iflt As Integer = ValueObject.Index
                Dim igrp As Integer = iSecondaryIndex

                If Me.m_EcoSimData.relQ(iflt, igrp) > 0 Then
                    'passed validation
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                End If

            Case eVarNameFlags.SearchBlock
                'Fishing Policy Search
                'Cannot set the SearchBlock for anything less than or equal to the BaseYear 

                If iSecondaryIndex > Me.FishingPolicyManager.ObjectiveParameters.BaseYear Then
                    'passed validation
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                End If

            Case eVarNameFlags.EcospaceSummaryTimeEnd

                Dim value As Single = CSng(ValueObject.Value)
                'greater than zero 
                'less than the last time step
                'greater than start summary period
                If value > 0 And value + m_EcoSpaceData.TimeStep * m_EcoSpaceData.NumStep <= m_EcoSpaceData.TotalTime And _
                                value > m_EcoSpaceData.SumStart(0) Then
                    'passed validation
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If

            Case eVarNameFlags.EcospaceSummaryTimeStart

                Dim value As Single = CSng(ValueObject.Value)
                'greater than or equal to zero 
                'less than the last time step
                'less than end summary period
                If value >= 0 And value + m_EcoSpaceData.TimeStep * m_EcoSpaceData.NumStep <= m_EcoSpaceData.TotalTime And _
                                value < m_EcoSpaceData.SumStart(1) Then
                    'passed validation
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If

            Case eVarNameFlags.EcospaceNumberSummaryTimeSteps
                'EcospaceNumberSummaryTimeSteps is the number of time steps to summarize over
                ' not the actual time in years
                Dim value As Integer = CInt(ValueObject.Value)

                'greater than zero
                'end of the last summary period is still in bounds
                If value > 0 And m_EcoSpaceData.SumStart(1) + value * m_EcoSpaceData.TimeStep <= m_EcoSpaceData.TotalTime Then
                    'passed validation
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If


            Case eVarNameFlags.EcosimSumEnd, eVarNameFlags.EcosimSumStart, eVarNameFlags.EcosimSumNTimeSteps

                Me.validateEcosimSummaryTimes(ValueObject)


            Case eVarNameFlags.MPAOptEndYear
                'Last year of the MPA Optimization Search
                Dim value As Integer = CInt(ValueObject.Value)

                If value > 0 And value <= m_EcoSpaceData.TotalTime And value >= Me.m_MPAOptData.EcoSpaceStartYear + Me.m_MPAOptData.MinRunLength Then
                    'passed validation
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If


            Case eVarNameFlags.MPAOptStartYear
                'First year of the MPA Optimization Search
                Dim value As Integer = CInt(ValueObject.Value)

                If value > 0 And value < m_EcoSpaceData.TotalTime And value + Me.m_MPAOptData.MinRunLength <= Me.m_MPAOptData.EcoSpaceEndYear Then
                    'passed validation
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If


            Case eVarNameFlags.MSEFixedF, eVarNameFlags.MSEFixedEscapement

                'Fixed F and Fixed Escapement can not be set at the same time
                'get the other value
                Dim otherVal As Single = Me.m_MSEData.FixedF(ValueObject.Index)
                If ValueObject.varName = eVarNameFlags.MSEFixedF Then
                    otherVal = Me.m_MSEData.FixedEscapement(ValueObject.Index)
                End If

                If otherVal = 0 Then
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.MSE_FIXF_FIXESC_FAILEDVALIDATION)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                End If

        End Select

        Return True

    End Function


    Private Sub validateEcosimSummaryTimes(ByRef ValueObject As cValue)

        Dim val As Single = CSng(ValueObject.Value)
        Dim cni As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim endsummary As Single

        'get the end of the summary period in years
        If ValueObject.varName = eVarNameFlags.EcosimSumEnd Or ValueObject.varName = eVarNameFlags.EcosimSumStart Then

            endsummary = val + CSng(m_EcoSimData.NumStep / m_EcoSimData.NumStepsPerYear)

        ElseIf ValueObject.varName = eVarNameFlags.EcosimSumNTimeSteps Then

            'user has edited the number of time steps get the last summary period (should be SumStart(1))
            If m_EcoSimData.SumStart(1) > m_EcoSimData.SumStart(0) Then
                endsummary = m_EcoSimData.SumStart(1) + CSng(val / m_EcoSimData.NumStepsPerYear)
            Else
                endsummary = m_EcoSimData.SumStart(0) + CSng(val / m_EcoSimData.NumStepsPerYear)
            End If

        End If

        'is the end of the summary periods in bounds
        If endsummary <= m_EcoSimData.NumYears Then
            'passed validation
            ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.OK
            ValueObject.Status(cCore.NULL_VALUE) = eStatusFlags.OK
        Else
            'failed validation
            ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        End If


    End Sub

    Private Function GetAffectedVariableStatus(ByVal obj As cCoreInputOutputBase, ByVal varName As eVarNameFlags, Optional ByVal iSecIndex As Integer = cCore.NULL_VALUE) As cVariableStatus
        Dim cni As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Return New cVariableStatus(obj, eStatusFlags.OK, _
                String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_ADJUSTED, cni.GetVarName(varName)), _
                varName, obj.DataType, obj.CoreComponent, obj.Index, iSecIndex)
    End Function

    ''' <summary>
    ''' A Variable has been validated succesfully but has not yet been stored in the core. This
    ''' method allows other variables, or related variables in other core objects to be affected
    ''' before these values are stored in the core.
    ''' </summary>
    ''' <param name="value">The <see cref="cValue">Value</see> that validated succesfully.</param>
    ''' <param name="obj">The <see cref="cCoreInputOutputBase">Core I/O object</see> that this value belongs to.</param>
    ''' <param name="msg">The <see cref="cMessage">main validation message</see> that this logic can attach
    ''' variables to.</param>
    Private Sub PostVariableValidation(ByVal value As cValue, ByVal obj As cCoreInputOutputBase, ByVal msg As cMessage)

        Debug.Assert(value.ValidationStatus <> eStatusFlags.FailedValidation, "PostVariableValidation() should not be called if a variable failed validation.")

        ' First update core data from object
        Select Case obj.DataType

            Case eDataTypes.EcoPathGroupInput
                Debug.Assert(TypeOf obj Is cEcoPathGroupInput)
                Dim group As cEcoPathGroupInput = DirectCast(obj, cEcoPathGroupInput)

                Select Case value.varName

                    Case eVarNameFlags.Biomass
                        Debug.Assert(False, "Biomass is not editable from the UI")

                    Case eVarNameFlags.Area, eVarNameFlags.BiomassAreaInput
                        ' Area or BiomassAreaInput have changed: recalculate B (biomass)
                        m_EcoPathData.Binput(group.Index) = group.BiomassAreaInput * group.Area
                        ' Add to msg
                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.Biomass))

                    Case eVarNameFlags.VBK
                        'see vaSimGetPBMandFtimeMax() in EwE5 case 10. Solve this here or in PostVariableUpdated?
                        Me.Cascade_VBK(group.VBK, group, msg)

                    Case eVarNameFlags.TCatchInput
                        Me.Cascade_TCatchInput(group.TcatchInput, group, msg)

                    Case eVarNameFlags.PP
                        ' Cascade PP change to other Groups
                        Me.Cascade_PP(group.PP, group, msg)

                End Select

            Case eDataTypes.FleetInput
                Dim flt As cFleetInput = DirectCast(obj, cFleetInput)

                Select Case value.varName
                    Case eVarNameFlags.Landings, eVarNameFlags.OffVesselPrice
                        Set_MarketPrice_Flags(flt, True)

                        If Me.m_StateMonitor.HasEcosimLoaded Then
                            Set_Quota_Flags(Me.MSEManager.FleetInputs(flt.Index), True)
                            Dim qsMsg As New cMessage("QuotaShare has changed.", eMessageType.DataModified, _
                                                        eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.MSEFleetInput)
                            Me.m_publisher.AddMessage(qsMsg)
                        End If

                    Case eVarNameFlags.Discards
                        Set_DiscardMort_Flags(flt, True)

                        Dim qsMsg As New cMessage("QuotaShare has changed.", eMessageType.DataModified, _
                            eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.MSEFleetInput)
                        Me.m_publisher.AddMessage(qsMsg)


                End Select

            Case eDataTypes.EcoSimModelParameter
                Debug.Assert(TypeOf obj Is cEcoSimModelParameters)
                Dim params As cEcoSimModelParameters = DirectCast(obj, cEcoSimModelParameters)

                Select Case value.varName
                    Case eVarNameFlags.EcoSimNYears

                        setEcosimRunLength(CInt(value.Value))

                        'the length of the ecospace run will be changed as well
                        Me.LoadEcospaceModelParameters()
                        'jb 26/09/2008 EcoSimNYears is already in the variables list adding it again causes loops over variables to execute twice
                        ' msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.EcoSimNYears))

                    Case eVarNameFlags.ConSimOnEcoSim
                        'toggle contaminant tracing OFF is ecospace
                        If CBool(value.Value) = True Then
                            If Me.StateMonitor.HasEcospaceLoaded Then
                                Me.EcospaceModelParameters.ContaminantTracing = False
                            End If
                        End If


                End Select

            Case eDataTypes.EcoSimGroupInput
                Debug.Assert(TypeOf obj Is cEcoSimGroupInput)
                Dim esi As cEcoSimGroupInput = DirectCast(obj, cEcoSimGroupInput)

                Select Case value.varName
                    Case eVarNameFlags.MaxRelPB
                        'see vaSimGetPBMandFtimeMax() in EwE5. Solve this here or in PostVariableUpdated?

                    Case eVarNameFlags.VulMult
                        Try
                            m_EcoSim.setvulratecell(obj.ValidationStatus.Index, obj.ValidationStatus.iArrayIndex, CSng(value.Value(obj.ValidationStatus.iArrayIndex)))
                        Catch ex As Exception
                            cLog.Write(ex)
                            Debug.Assert(False, "PostVariableValidation() setvulratecell error. " & ex.StackTrace)
                        End Try


                End Select

            Case eDataTypes.EcospaceGroup

                Dim esg As cEcospaceGroup = DirectCast(obj, cEcospaceGroup)

                Select Case value.varName

                    Case eVarNameFlags.PreferredHabitat

                        ' If 'All' habitat is set, all other preferred habitat assignments must be cleared.
                        ' If any other habitat is set, the 'All' habitat assignment must be cleared.

                        esg.AllowValidation = False

                        ' Setting a value?
                        If Object.Equals(value.Value(esg.ValidationStatus.iArrayIndex), True) Then
                            ' 'All' habitat set? Clear all other preferred habitats
                            If esg.ValidationStatus.iArrayIndex = 0 Then
                                For iHabitat As Integer = 0 To Me.nHabitats - 1
                                    value.Value(iHabitat) = (iHabitat = 0)
                                    ' Add to msg
                                    msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.PreferredHabitat, iHabitat))
                                Next
                            Else
                                ' Clear 'All' habitat from preferred habitats
                                value.Value(0) = False
                                ' Add to msg
                                msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.PreferredHabitat, 0))
                            End If
                        End If
                        esg.AllowValidation = True

                End Select

            Case eDataTypes.EcospaceFleet

                Dim esf As cEcospaceFleet = DirectCast(obj, cEcospaceFleet)

                Select Case value.varName

                    Case eVarNameFlags.HabitatFishery

                        ' If 'All' habitat is set, all other preferred habitat assignments must be cleared.
                        ' If any other habitat is set, the 'All' habitat assignment must be cleared.

                        esf.AllowValidation = False

                        ' Setting a value?
                        If Object.Equals(value.Value(esf.ValidationStatus.iArrayIndex), True) Then
                            ' 'All' habitat set? Clear all other preferred habitats
                            If esf.ValidationStatus.iArrayIndex = 0 Then
                                For iHabitat As Integer = 0 To Me.nHabitats - 1
                                    value.Value(iHabitat) = (iHabitat = 0)
                                    ' Add to msg
                                    msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.HabitatFishery, iHabitat))
                                Next
                            Else
                                ' Clear 'All' habitat from preferred habitats
                                value.Value(0) = False
                                ' Add to msg
                                msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.HabitatFishery, 0))
                            End If
                        End If
                        esf.AllowValidation = True

                End Select

            Case eDataTypes.EcospaceModelParameter

                Dim spaceParams As cEcospaceModelParameters = DirectCast(obj, cEcospaceModelParameters)

                spaceParams.AllowValidation = False

                Select Case value.varName

                    Case eVarNameFlags.EcospaceSummaryTimeStart

                        spaceParams.StartSummaryTime = Math.Min(spaceParams.EndSummaryTime, spaceParams.StartSummaryTime)
                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.EcospaceSummaryTimeStart))

                    Case eVarNameFlags.EcospaceSummaryTimeEnd

                        spaceParams.EndSummaryTime = Math.Max(spaceParams.EndSummaryTime, spaceParams.StartSummaryTime)
                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.EcospaceSummaryTimeEnd))

                    Case eVarNameFlags.TotalTime

                        'setEcosimRunLength will set the model run length in both ecosim and ecospace
                        setEcosimRunLength(CInt(value.Value))

                        'change the summary periods to fit the new run length
                        Me.m_EcoSpaceData.setDefaultSummaryPeriod()

                        'load the new data into the parameters object
                        Me.LoadEcospaceModelParameters()

                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.TotalTime))

                    Case eVarNameFlags.ConSimOnEcoSpace
                        'toggle contaminant tracing OFF in ecosim
                        If CBool(value.Value) = True Then
                            Me.EcoSimModelParameters.ContaminantTracing = False

                            If Me.EcospaceModelParameters.NumberOfTimeStepsPerYear <> 12 Then
                                'Ecospace must have monthly timestep for Contaminant tracing
                                Me.EcospaceModelParameters.NumberOfTimeStepsPerYear = 12
                                msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.NumTimeStepsPerYear))
                            End If
                        End If

                End Select 'Select Case value.varName

                spaceParams.AllowValidation = True

            Case eDataTypes.SearchObjectiveParameters

                Select Case value.varName

                    Case eVarNameFlags.SearchBaseYear
                        'Change the search blocks in response to an baseyear edit
                        'All search blocks must be set to zero for the base year

                        'user edited interface object
                        Dim InputParams As SearchObjectives.cSearchObjectiveParameters = Me.SearchObjective.ObjectiveParameters
                        'get the new base year from the interface object that has been set by a user
                        Dim iNewBaseYear As Integer = InputParams.BaseYear
                        'get the current base year from the cores data this has not been changed yet
                        Dim iOrgBaseYear As Integer = Me.m_SearchData.BaseYear
                        Dim yearOffset As Integer = 1

                        'make sure this is a different base year
                        If (iNewBaseYear = iOrgBaseYear) Then Return

                        'figure out where to get the code to clear out the the current base year
                        If Me.m_SearchData.BaseYear = Me.nEcosimYears Then
                            yearOffset = -1
                        End If

                        For iflt As Integer = 1 To Me.nFleets
                            ''get the code from a neighbouring block
                            'Dim iClearCode As Integer = Me.m_SearchData.FblockCode(iflt, iOrgBaseYear + yearOffset)
                            ''set the current baseyear to its neighbours code
                            'Me.m_SearchData.FblockCode(iflt, iOrgBaseYear) = iClearCode
                            ''set the code for the new base year
                            'Me.m_SearchData.FblockCode(iflt, InputParams.BaseYear) = 0 'set the new base year

                            ' JS30may08: set the values of blocks between the old and the new base year
                            For iyear As Integer = Math.Min(iNewBaseYear, iOrgBaseYear) To Math.Max(iNewBaseYear, iOrgBaseYear)
                                Dim iCode As Integer = 0
                                If iNewBaseYear > iOrgBaseYear Then
                                    'set as base year
                                    iCode = 0
                                Else
                                    'get the code from a neighbouring block
                                    iCode = Me.m_SearchData.FblockCode(iflt, Math.Max(iNewBaseYear, iOrgBaseYear) + yearOffset)
                                End If
                                'set the code for the each year
                                Me.m_SearchData.FblockCode(iflt, iyear) = iCode
                            Next
                        Next iflt

                        Me.m_SearchData.BaseYear = InputParams.BaseYear

                        'Load the new search blocks into the fishing policy manager
                        Me.m_SearchManagers(eDataTypes.FishingPolicyManager).Load()

                        'tell the world that that the Fishing Policy search blocks have changed
                        Dim sbmsg As New cMessage("Fishing Policy search blocks have changed.", eMessageType.DataModified, _
                                        eCoreComponentType.FishingPolicySearch, eMessageImportance.Maintenance, eDataTypes.FishingPolicySearchBlocks)
                        Me.m_publisher.AddMessage(sbmsg)

                End Select 'Select Case value.varName

            Case eDataTypes.ParticleSizeDistribution

                Me.m_PSDParameters.AllowValidation = False
                Me.m_PSDParameters.PSDComputed = False
                Me.m_PSDParameters.AllowValidation = True

        End Select

                ' Cascade name changes across models
                If value.varName = eVarNameFlags.Name Then
                    Me.Cascade_Name(CStr(value.Value), obj, msg)
                End If


                ' Cascade PP changes across models

    End Sub


    ''' <summary>
    ''' A Variable has been validated succesfully and has been stored in the core. 
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="obj"></param>
    ''' <remarks>This gives the core a chance to update any of it internal data structures after a user has edited a variable. </remarks>
    Private Sub PostVariableUpdated(ByRef value As cValue, ByRef obj As cCoreInputOutputBase)

        Dim bRecalcStanza As Boolean = False

        Debug.Assert(value.ValidationStatus <> eStatusFlags.FailedValidation, "PostVariableUpdated() should not be called if a variable failed validation.")

        ' First update core data from object
        Select Case obj.DataType

            Case eDataTypes.EcoPathGroupInput

                Debug.Assert(TypeOf obj Is cEcoPathGroupInput)

                Dim egi As cEcoPathGroupInput = DirectCast(obj, cEcoPathGroupInput)

                Select Case value.varName

                    Case eVarNameFlags.Area, eVarNameFlags.BiomassAreaInput
                        ' Set biomass area status
                        Me.Set_PB_QB_GE_BA_Flags(egi)

                    Case eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.GEInput
                        'PB, QB or GE has been changed in the interface
                        Me.Set_PB_QB_GE_BA_Flags(egi)
                        ' Need to recalc stanza when this group is part of a multi-stanza configuration
                        bRecalcStanza = (egi.iStanza > 0)

                        If value.varName = eVarNameFlags.PBInput Then
                            'update bgoal from the new PB
                            Me.m_SearchData.setDefaultBGoal(Me.m_EcoPathData.PBinput) 'use PBInput because PB has not been update at this time
                            'load the values into the search manager
                            'if Ecosim has not been loaded SearchObjectiveManager.Load() will do nothing
                            Me.m_SearchManagers(eDataTypes.SearchObjectiveManager).Load()

                            Dim msg As New cMessage("Search Structure rel. weight changed.", eMessageType.DataModified, _
                                            eCoreComponentType.SearchObjective, eMessageImportance.Maintenance, eDataTypes.SearchObjectiveManager)
                            Me.m_publisher.AddMessage(msg)

                        End If

                    Case eVarNameFlags.GS
                        'GS has been changed in the interface
                        Me.Set_GS_Flags(egi)

                    Case eVarNameFlags.DietComp
                        'DietComp has been changed by the user
                        'this needs to update the Ecosim dietcomp and refresh the shape functions AppliesTo datastructures 
                        Me.m_StateManager.updateDietComp()
                        ' Sync the ecosim groups
                        LoadEcosimGroups()

                        m_PPIManager.Init()
                        m_PPIManager.Load()

                        Me.m_SearchManagers(eDataTypes.FitToTimeSeries).Load()
                        '  Me.m_FitToTimeSeries.Load()

                    Case eVarNameFlags.VBK
                        'see vaSimGetPBMandFtimeMax() in EwE5 case 10. Solve this here or in PostVariableValidation?

                        ' Need to recalc stanza when this group is part of a multi-stanza configuration
                        bRecalcStanza = (egi.iStanza > 0)

                    Case eVarNameFlags.BioAccum, eVarNameFlags.BioAccumRate
                        Me.set_BioAccumRate_Flags(egi, value.varName)
                        Me.LoadEcopathInput(egi)

                    Case eVarNameFlags.GS
                        'If GS is a primay producer then it can only be zero
                        Me.Set_GS_Flags(egi)

                    Case eVarNameFlags.PP
                        Me.Set_GS_Flags(egi)
                        Me.Set_PB_QB_GE_BA_Flags(egi)
                        Me.Set_EE_Flags(egi)
                        Me.Set_DetImp_Flags(egi)

                End Select

            Case eDataTypes.Stanza
                ' Need to recalc multi-stanza configuration
                bRecalcStanza = True

            Case eDataTypes.FleetInput

                Dim flt As cFleetInput = DirectCast(obj, cFleetInput)

                Select Case value.varName
                    ' For landings and discards, check the effect on stanza configurations
                    Case eVarNameFlags.Landings, eVarNameFlags.Discards
                        Update_Stanza_Catches()

                End Select

            Case eDataTypes.EcoSimModelParameter
                Select Case value.varName
                    Case eVarNameFlags.EcoSimNYears
                        ' Solve in PostVariableValidation

                    Case eVarNameFlags.EcosimSumEnd, eVarNameFlags.EcosimSumStart, eVarNameFlags.EcosimSumNTimeSteps
                        'the user has changed the Ecosim summary start or end time
                        'this is the red vertical lines on the Ecosim biomass graph

                        'reload the ecosim results object with the new summary data
                        ' LoadEcosimSummaries()
                        Me.LoadEcosimGroupOutputs()
                        Me.LoadEcosimFleetOutputs()

                        'tell the world that this has happened
                        Dim msg As New cMessage("Ecosim results time period has changed.", eMessageType.DataModified, _
                                        eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.EcoSimModelParameter)

                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.EcosimSumStart))
                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.EcosimSumEnd))

                        Me.m_publisher.AddMessage(msg)

                End Select

            Case eDataTypes.EcoSimGroupInput
                Select Case value.varName
                    Case eVarNameFlags.MaxRelPB
                        'see vaSimGetPBMandFtimeMax() in EwE5. Solve this here or in PostVariableValidation?
                End Select

            Case eDataTypes.EcospaceModelParameter

                Dim emp As cEcospaceModelParameters = DirectCast(obj, cEcospaceModelParameters)

                Select Case value.varName

                    Case eVarNameFlags.NumTimeStepsPerYear
                        'user has changed the number of ecospace time steps per year
                        'resize the output data Ecospace will take care of itself

                        Me.m_EcoSpaceData.setDefaultSummaryPeriod()

                        ' ToDo_JS: Test if core counter has been updated prior to calling this!
                        For Each objOutput As cEcospaceGroupOutput In m_EcospaceGroupOuputs
                            objOutput.Resize()
                        Next

                    Case eVarNameFlags.UseIBM
                        Me.Set_IBM_Flags(emp)


                    Case eVarNameFlags.EcospaceNumberSummaryTimeSteps, eVarNameFlags.EcospaceSummaryTimeEnd, eVarNameFlags.EcospaceSummaryTimeStart
                        Me.LoadEcospaceResults()

                End Select 'Select Case value.varName


            Case eDataTypes.MonteCarlo

                Select Case value.varName

                    Case eVarNameFlags.mcBAcv, eVarNameFlags.mcBcv, eVarNameFlags.mcEEcv, eVarNameFlags.mcPBcv, eVarNameFlags.mcVUcv

                        Me.m_MonteCarlo.CalculateUpperLowerLimits()
                        Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                                                     eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.MonteCarlo))


                End Select

            Case eDataTypes.MSEFleetInput

                'Something in the fisheries regulation data has changed 
                'update all the variables
                Me.MSEManager.UpdateAssesmentVars()

                'jb if the game client has edited the fisheries quotas make sure the status flags are reset 
                'the client may have edited values that are not editable
                obj.ResetStatusFlags()


            Case eDataTypes.MSEGroupInput

                obj.ResetStatusFlags()

                Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified, _
                             eCoreComponentType.MSE, eMessageImportance.Maintenance, eDataTypes.MSEGroupInput))


        End Select

        ' Update multi-stanza info
        If (bRecalcStanza) Then
            ' Recalc stanza parms
            Me.m_EcoSim.InitStanza()
            ' Update GUI objects
            Me.LoadStanzas()
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for Non cCoreInputOutputBase object to report changes to the core
    ''' </summary>
    ''' <param name="obj">reference to a ICoreInterface object that has changed its data.</param>
    ''' <param name="TypeOfChange">How the object was changed </param>
    ''' <remarks> <para>This provides a public generic interface for any core object to communicate with the core. 
    ''' The nature of the comunication can be defined by the ICoreInterface object.</para> 
    ''' <para>Not all core objects can be fit into a cCoreInputOutputBase interface. This 
    ''' provides a way for these object to comumicate changes with the core.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub onChanged(ByVal obj As ICoreInterface, Optional ByVal TypeOfChange As eMessageType = eMessageType.NotSet)
        Dim manager As cBaseShapeManager = Nothing

        Try
            Select Case obj.DataType

                Case eDataTypes.PredPreyInteraction
                    Me.m_publisher.AddMessage(New cMessage("Shape application changed.", eMessageType.DataModified, eCoreComponentType.PPIManager, eMessageImportance.Maintenance))

                Case eDataTypes.Forcing, eDataTypes.EggProd, eDataTypes.Mediation

                    If (obj.DataType = eDataTypes.Forcing Or obj.DataType = eDataTypes.EggProd) Then

                        If TypeOfChange = eMessageType.DataAddedOrRemoved Then
                            'If a Forcing or EggProd object was added/removed then both these managers need to reload their data 
                            'as they share the same array data
                            manager = m_ShapeManagers.Item(eDataTypes.EggProd)
                            manager.Load()

                            manager = m_ShapeManagers.Item(eDataTypes.Forcing)
                            manager.Load()
                        End If
                    End If

                    If TypeOfChange = eMessageType.DataAddedOrRemoved Then
                        ' Only send out ONE message
                        Me.m_publisher.AddMessage(New cMessage("Shape added or removed.", eMessageType.DataAddedOrRemoved, _
                                     eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, obj.DataType))
                    End If

                    If TypeOfChange = eMessageType.DataModified Then
                        ' Only send out ONE message
                        Me.m_publisher.AddMessage(New cMessage("Shape modified.", eMessageType.DataModified, _
                                     eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, obj.DataType))
                    End If

                    If (obj.DataType = eDataTypes.Forcing Or obj.DataType = eDataTypes.Mediation) Then

                        'if the mediation or forcing manager has added or removed a shape
                        'then the Pred/Prey interaction manager PPIManager needs to reload all its data
                        'this is brute force
                        If TypeOfChange = eMessageType.DataAddedOrRemoved Then
                            m_PPIManager.Init()
                            m_PPIManager.Load()

                            Me.m_publisher.AddMessage(New cMessage("PPI manager reloaded data.", eMessageType.DataModified, _
                                                eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.PredPreyInteraction))
                        End If

                    End If

                Case eDataTypes.FishMort

                    Me.m_publisher.AddMessage(New cMessage("Fish mort shape modified", TypeOfChange, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishMort))

                Case eDataTypes.FishingEffort, eDataTypes.FishingPolicyManager
                    Dim qyear() As Single
                    ReDim qyear(Me.nGroups)
                    For i As Integer = 1 To Me.nFleets : qyear(i) = 1 : Next

                    'reset the mortaility due to fishing to the new values
                    Me.m_EcoSim.SetFFromGear(qyear)

                    'now load the interface data
                    'if the FishRate shape manager has changed the data then fishmort was also changed
                    're-load the fishMort shapes
                    manager = m_ShapeManagers.Item(eDataTypes.FishMort)
                    manager.Load()

                    'Ok this is kind of brutal
                    'If it was the all fleets fishing rate shape that changed
                    'Then it made changes to all the fleets fishing rate shapes underlying data
                    'that means all the fishing rate shapes need to be re-loaded
                    'If this becomes an issue we will need a way to tell what shape was edited either here or in the mangers.Load method
                    'brute force is good enough for now
                    manager = m_ShapeManagers.Item(eDataTypes.FishingEffort)
                    manager.Load()

                    Me.m_publisher.AddMessage(New cMessage("Fish rate shape modified", TypeOfChange, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishingEffort))
                    Me.m_publisher.AddMessage(New cMessage("Fish mort shape modified", TypeOfChange, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishMort))

                Case eDataTypes.EcospaceLayerDepth, eDataTypes.EcospaceLayerHabitat

                    ' Recalc habitat area
                    Me.LoadEcospaceHabitats()
                    Me.m_publisher.AddMessage(New cMessage("Ecospace basemap changed.", eMessageType.DataModified, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerDepth))
                    Me.m_publisher.AddMessage(New cMessage("Ecospace habitats changed.", eMessageType.DataModified, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance, eDataTypes.EcospaceHabitat))

                Case eDataTypes.EcospaceLayerMPA, _
                     eDataTypes.EcospaceLayerImportance, _
                     eDataTypes.EcospaceLayerRegion, _
                     eDataTypes.EcospaceLayerRelCin, _
                     eDataTypes.EcospaceLayerRelPP, _
                     eDataTypes.EcospaceLayerPort, _
                     eDataTypes.EcospaceLayerSail, _
                     eDataTypes.EcospaceLayerMigration
                    DirectCast(obj, cEcospaceLayer).Invalidate()
                    Me.m_publisher.AddMessage(New cMessage("Ecospace layer changed.", eMessageType.DataModified, eCoreComponentType.EcoSpace, eMessageImportance.Maintenance, obj.DataType))

                Case eDataTypes.EcospaceHabitat

                Case eDataTypes.Stanza
                    'A user has called Apply() on a stanza group. Update the underlying data Stanza and Ecopath
                    Me.UpdateStanza(obj.DBID)

                    'UpdateStanza() updated the cores ecopath data with values computed by CalculateStanzaParameters()
                    'reload the input objects
                    Me.LoadEcopathInputs()

                    'The Stanza object knows that it has changed make sure anything else that is listening knows as well
                    Me.m_publisher.AddMessage(New cMessage("Stanza group changed.", eMessageType.DataModified, eCoreComponentType.EcoSim, _
                                                            eMessageImportance.Maintenance, eDataTypes.Stanza))

                    'Ecopath Message
                    Me.m_publisher.AddMessage(New cMessage("Stanza group changed Ecopath values.", eMessageType.DataModified, eCoreComponentType.EcoPath, _
                                       eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))

                    'Tell the datasource that both Ecopath and Stanza data need saving. May not need to do this it may be good enough that the stanza data is dirty
                    DataSource.SetChanged(eCoreComponentType.EcoPath)
                    DataSource.SetChanged(eCoreComponentType.EcoSim)
                    ' Ecopath needs to run again
                    Me.StateMonitor.SetEcopathLoaded(True)

                Case eDataTypes.GroupTimeSeries, eDataTypes.FleetTimeSeries
                    ' Reload
                    If Me.UpdateEcosimTimeSeries() Then Me.m_TSData.loadEnabled(obj.Index)
                    Me.m_SearchManagers(eDataTypes.FitToTimeSeries).Load()

                    Me.m_publisher.AddMessage(New cMessage("Time series have changed.", eMessageType.DataModified, _
                        eCoreComponentType.TimeSeries, eMessageImportance.Maintenance))

                Case eDataTypes.PedigreeLevel

                    ' Me.m_PedigreeManagers

                Case eDataTypes.MonteCarlo
                    Me.LoadEcopathInputs()
                    Me.LoadEcosimGroups()
            End Select

            ' JS 31aug07: DataAddedOrRemoved messages are initialized by the db, thus the db should not get flagged as dirty
            If TypeOfChange <> eMessageType.DataAddedOrRemoved Then
                ' Update data state
                DataSource.SetChanged(obj.CoreComponent)
                Me.m_StateMonitor.UpdateDataState(DataSource)
            End If

            Me.m_StateMonitor.UpdateExecutionState(obj.CoreComponent)

            Try
                If Not Object.ReferenceEquals(Me.PluginManager, Nothing) Then
                    Me.PluginManager.DataValidated(eVarNameFlags.NotSet, obj.DataType)
                End If
            Catch ex As Exception
                ' NOP
            End Try

            Me.m_publisher.sendAllMessages()

        Catch ex As Exception

            cLog.Write(ex)
            'maybe a better message than this
            Me.m_publisher.AddMessage(New cMessage("Error in " & Me.ToString & ".OnChanged(). " & ex.Message, _
                                        eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical))

        End Try

    End Sub

#End Region ' Variable validation

#Region " Plugins "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cPluginManager">Plug-in manager</see> that the core must use
    ''' for accessing plug-ins.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PluginManager() As cPluginManager
        Get
            Return Me.m_pluginManager
        End Get
        Set(ByVal pm As cPluginManager)
            ' Remember plugin manager
            Me.m_pluginManager = pm
            ' Hand plugin manager to ecopath
            Me.m_EcoPath.PluginManager = pm
            Me.m_EcoSim.PluginManager = pm
            Me.m_Ecospace.PluginManager = pm

            If (Me.m_pluginManager IsNot Nothing) Then
                ' Hand plugin manager a delegate to check core enabled state
                Me.m_pluginManager.CoreExecutionStateDelegate = New cPluginManager.CanExecutePlugin(AddressOf Me.CanExecutePlugin)
            End If
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in manager event handler, caught to provide message feedback about loaded plug-in assemblies.
    ''' </summary>
    ''' <param name="paAdded">A loaded <see cref="cPluginAssembly">plug-in assembly</see>.</param>
    ''' -----------------------------------------------------------------------
    Private Sub m_pluginManager_AssemblyAdded(ByVal paAdded As EwEPlugin.cPluginAssembly) _
        Handles m_pluginManager.AssemblyAdded

        Me.m_publisher.SendMessage(New cMessage(String.Format("Plug-in module '{0}' loaded", paAdded.Filename), eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information))
        'AddHandler paAdded.AssemblyEnabled, AddressOf OnPluginAssemblyStateChanged
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in manager event handler, caught to provide message feedback about removed plug-in assemblies.
    ''' </summary>
    ''' <param name="paRemoved">A removed <see cref="cPluginAssembly">plug-in assembly</see>.</param>
    ''' -----------------------------------------------------------------------
    Private Sub m_pluginManager_AssemblyRemoved(ByVal paRemoved As EwEPlugin.cPluginAssembly) _
        Handles m_pluginManager.AssemblyRemoved

        m_publisher.SendMessage(New cMessage(String.Format("Plugin module '{0}' unloaded", paRemoved.Filename), eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information))
        'RemoveHandler paRemoved.AssemblyEnabled, AddressOf OnPluginAssemblyStateChanged
    End Sub

    Private Sub OnPluginAssemblyStateChanged(ByVal pa As cPluginAssembly, ByVal bEnabled As Boolean)

        If (pa Is Nothing) Then Return
        If (pa.Plugins(GetType(IEconomicData)) IsNot Nothing) Then
            Me.OnEconomicDataPluginEnabled()
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The Plugin Manager has caught an exception thrown by a plugin
    ''' </summary>
    ''' <param name="PluginException"></param>
    ''' -----------------------------------------------------------------------
    Private Sub m_pluginManager_PluginException(ByVal PluginException As cPluginException) _
        Handles m_pluginManager.PluginException

        If PluginException.Assembly.AlwaysEnabled Then
            Dim msg As cMessage = New cMessage(PluginException.Message, eMessageType.ErrorEncountered, eCoreComponentType.External, eMessageImportance.Warning)
            m_publisher.SendMessage(msg)
        Else
            Dim fmsg As New cFeedbackMessage( _
                    String.Format(My.Resources.CoreMessages.PLUGIN_PROMPT_DISABLE, PluginException.Message, vbNewLine), _
                    eCoreComponentType.External, eMessageType.Any, _
                    eMessageImportance.Warning, _
                    cFeedbackMessage.eReplyStyle.YES_NO, eDataTypes.NotSet, cFeedbackMessage.eReply.YES)

            m_publisher.SendMessage(fmsg)
            PluginException.Assembly.Enabled = (fmsg.Reply = cFeedbackMessage.eReply.NO)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Callback for <see cref="cPluginManager.CanExecutePlugin">Plug-in manager CanExecutePlugin delegate</see>,
    ''' which the plug-in manager must invoke to test if a plug-in can be enabled by testing a given 
    ''' <see cref="EwEUtils.Core.eCoreExecutionState">Core execution state</see> against the
    ''' <see cref="cCoreStateMonitor.CoreExecutionState">current core execution state</see>.
    ''' </summary>
    ''' <param name="coreExecutionState">The <see cref="EwEUtils.Core.eCoreExecutionState">Core execution state</see> to test.</param>
    ''' <returns>True if the current core state enables to tested core execution state.</returns>
    ''' -----------------------------------------------------------------------
    Private Function CanExecutePlugin(ByVal coreExecutionState As eCoreExecutionState) As Boolean
        Return Me.m_StateMonitor.IsExecutionStateSuperceded(coreExecutionState)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, triggered when the <see cref="cCoreStateMonitor.CoreExecutionStateEvent">Core State Monitor</see>
    ''' execution state has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_StateMonitor_CoreExecutionStateEvent(ByVal csm As cCoreStateMonitor) _
        Handles m_StateMonitor.CoreExecutionStateEvent

        If Me.m_pluginManager IsNot Nothing Then
            ' Inform the plugin manager of the new core state.
            Me.m_pluginManager.UpdatePluginEnabledStates()
        End If

    End Sub

    ''' <summary>
    ''' The enabled state of an Economic plugin has changed (e.g. ValueChain)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnEconomicDataPluginEnabled()
        'ToDo_jb cCore.onEconomicPluginEnabled() we still need to find a way for the core to know when Plugin Enabled states have changed

        'This should only be called when a plugin that supports Economic data has changed
        'that decision will need to be made elsewhere

        'update all components that could be using the Economic data from a plugin

        Try
            'this will reset the isEconomicAvailable flag in the Parameters objects to the values 
            If Me.m_StateMonitor.HasEcosimLoaded Then
                'implementation limitation: managers are only initialized when Ecosim is initialized
                Me.MSEManager.Load()
                Me.FishingPolicyManager.Load()

                Me.m_publisher.SendMessage(New cMessage("", eMessageType.DataModified, eCoreComponentType.FishingPolicySearch, eMessageImportance.Maintenance))
                Me.m_publisher.SendMessage(New cMessage("", eMessageType.DataModified, eCoreComponentType.MSE, eMessageImportance.Maintenance))
            End If

        Catch ex As Exception
            Debug.Assert(False, "Core.onEconomicPluginEnabled() Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Plugins

#Region " Search Managers "

    Public ReadOnly Property SearchObjective() As cSearchObjective

        Get
            Try

                Return DirectCast(Me.m_SearchManagers(eDataTypes.SearchObjectiveManager), cSearchObjective)

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, "SearchObjective() not avalible...... Oppssssss ")
                Return Nothing
            End Try
        End Get

    End Property

    Private m_SearchManagers As New Dictionary(Of eDataTypes, ISearchObjective)
    '  Private m_SearchObjective As cSearchObjective

    ''' <summary>
    ''' Build and initialize the search managers
    ''' </summary>
    Private Sub InitSearchManagers()

        Dim SearchManager As ISearchObjective
        Me.m_SearchData = New cSearchDatastructures(Me.m_Functions, Me.m_EcoPathData)
        AddHandler Me.m_SearchData.OnSearchStateChanged, AddressOf OnSearchChanged
        ' Sanity check

        'Shared Objective manager
        If Not Me.m_SearchManagers.ContainsKey(eDataTypes.SearchObjectiveManager) Then
            SearchManager = New cSearchObjective
            Me.m_SearchManagers.Add(eDataTypes.SearchObjectiveManager, SearchManager)
        End If

        'MSE
        If Not Me.m_SearchManagers.ContainsKey(eDataTypes.MSEManager) Then
            SearchManager = New cMSEManager(Me, Me.m_MSEData)
            Me.m_SearchManagers.Add(eDataTypes.MSEManager, SearchManager)
        End If

        'Ecoseed
        If Not Me.m_SearchManagers.ContainsKey(eDataTypes.MPAOptManager) Then
            SearchManager = New cMPAOptManager
            Me.m_SearchManagers.Add(eDataTypes.MPAOptManager, SearchManager)
        End If

        'Fishing Policy
        If Not Me.m_SearchManagers.ContainsKey(eDataTypes.FishingPolicyManager) Then
            SearchManager = New cFishingPolicyManager
            Me.m_SearchManagers.Add(eDataTypes.FishingPolicyManager, SearchManager)
        End If


        'Fit to time series 
        If Not Me.m_SearchManagers.ContainsKey(eDataTypes.FitToTimeSeries) Then
            SearchManager = New cF2TSManager(Me)
            Me.m_SearchManagers.Add(eDataTypes.FitToTimeSeries, SearchManager)
        End If

    End Sub

    Private Sub OnSearchChanged(ByVal searchmode As eSearchModes)
        Me.m_StateMonitor.SetIsSearching(searchmode <> eSearchModes.NotInSearch)
    End Sub

#Region "Fishing Policy Search"

    Public ReadOnly Property FishingPolicyManager() As cFishingPolicyManager
        Get
            Try

                Return DirectCast(Me.m_SearchManagers(eDataTypes.FishingPolicyManager), cFishingPolicyManager)

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, "FishingPolicyManager() not avalible...... Oppssssss ")
                Return Nothing
            End Try
        End Get

    End Property

#End Region 'Fishing policy search

#Region "Ecoseed"

    Friend m_MPAOptData As cMPAOptDataStructures


    Friend ReadOnly Property MPAOptData() As cMPAOptDataStructures
        Get
            Return m_MPAOptData
        End Get
    End Property

    Public ReadOnly Property MPAOptimizationManager() As cMPAOptManager
        Get
            Try
                Return DirectCast(Me.m_SearchManagers.Item(eDataTypes.MPAOptManager), cMPAOptManager)
            Catch ex As Exception
                Debug.Assert(False, "Error getting EcoSeedManager(): " & ex.Message)
                cLog.Write(ex)
                Return Nothing
            End Try

        End Get
    End Property

#End Region

#Region "MSE"

    '  Dim m_MSEManager As cMSEManager

    Public ReadOnly Property MSEManager() As cMSEManager
        Get
            Try
                Return DirectCast(Me.m_SearchManagers.Item(eDataTypes.MSEManager), cMSEManager)
            Catch ex As Exception
                cLog.Write(ex)
                Return Nothing
            End Try

        End Get
    End Property

#End Region

#End Region ' Search managers

#Region " Pedigree "

    Public Function AddPedigreeLevel(ByVal varName As eVarNameFlags, ByVal iPosition As Integer, _
            ByVal sIndexValue As Single, ByVal sConfidence As Single, ByVal strDescription As String, ByRef iDBID As Integer) As Boolean

        Return False
    End Function

    Public Function RemovePedigreeLevel(ByVal iDBID As Integer) As Boolean
        Return False
    End Function

#End Region ' Pedigree

#Region "Game manager/interface"

    Public ReadOnly Property GameManager() As cGameServerInterface
        Get
            Return m_gameManager
        End Get
    End Property
#End Region

#Region "Eco Functions"

    Private Sub initEcoFunctions()
        Try
            Me.m_Functions.Init(Me)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
        End Try
    End Sub

    ''' <summary>
    ''' Public access to stand alone functions wrapped in cEcoFunctions
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EcoFunction() As cEcoFunctions
        Get
            Return Me.m_Functions
        End Get
    End Property

#End Region

End Class
