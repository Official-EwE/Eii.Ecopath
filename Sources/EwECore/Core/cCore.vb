' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.IO
Imports System.Text
Imports System.Xml
Imports EwECore.Auxiliary
Imports EwECore.Common
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwECore.Ecosim
Imports EwECore.Ecospace.Advection
Imports EwECore.ExternalData
Imports EwECore.FishingPolicy
Imports EwECore.MSE
Imports EwECore.Samples
Imports EwECore.SearchObjectives
Imports EwECore.SpatialData
Imports EwECore.ValueWrapper
Imports EwELicense
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug





#Disable Warning IDE0017 ' Suppress "Object initialization can be simplified" 
#Disable Warning IDE0009 ' Suppress "Add Me qualification" 
''' ---------------------------------------------------------------------------
''' <summary>
''' Class to handle all interactions between a user interface layer, a 
''' <see cref="IEwEDataSource">data source</see> and the 
''' <see cref="Ecopath.cEcopathModel">Ecopath</see>, <see cref="EcoSim.cEcosimModel">EcoSim</see> 
''' and <see cref="cEcoSpace">EcoSpace</see> models.
''' </summary>
''' <remarks>
''' <para>This class provides a wrapper for the underlying EcoPath, EcoSim and
''' EcoSpace models.</para>
''' <para>The underlying model data structures have been converted into classes
''' that an interface can program against. For instance, cEcopathFleetInput is the
''' representation of a fishing fleet.</para>
''' <para>The <see cref="EcopathfleetInputs"/>(iFleet) property provides a way for 
''' a user interface to interact with the underlying data structures that represent 
''' a fishing fleet without having to understand the modeling array structures.</para>
''' <para>Most conversions from interface objects (cEcopathFleetInput or cEcoSimResults) into
''' model data structures are handled by the core.</para>
''' <para>Data structures for each model that need to be made public for setting
''' of parameters or storing to file are held in a wrapper class for each model
''' (<see cref="cEcopathDataStructures"/> or <see cref="cEcosimDatastructures"/>). 
''' These classes provide a thin wrapper, and a means to pass data back and forth between 
''' each other and a <see cref="IEwEDataSource">data source</see>.</para>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cCore
    Implements IDisposable

#Region " Shared consts "

    ''' <summary>The NULL or 'no data' value for values maintained in the EwE Core.</summary>
    Public Const NULL_VALUE As Integer = -9999
    ''' <summary>The maximum age of a stanza life stage.</summary>
    Public Const MAX_AGE As Integer = 400 '1200 '
    ''' <summary>The number of months in a year.</summary><remarks type="bs">A petition to change the number of months per year to 10 has been submitted to the international organization for standardization (ISO) dd Jun02, 2007. We sincerely hope that the next addendum to ISO 9000 will include this change to facilitate our computational models. Unfortunately, until this change has globally been implemented issued, Ecopath will be using the more conventional assumption of 12 months per year.</remarks>
    Public Const N_MONTHS As Integer = 12
    ''' <summary>Max number of years ecosim or ecospace can run for</summary>
    Public Const MAX_RUN_LENGTH As Integer = 500
    ''' <summary>Because the shared arenas logic still causes issues in Ecospace and the Monte Carlo diet searches,
    ''' this flag can be used to bypass shared arenas data loading, logic, and user interfaces</summary>
    Public Const USE_SHARED_ARENAS As Boolean = False

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

    ''' <summary>
    ''' Delegate that can be passed to the core to allow interruption of a run or search
    ''' </summary>
    Public Delegate Sub StopRunDelegate()

    ''' <summary>
    ''' Delegate used by <see cref="cThreadWaitBase">cThreadWaitBase</see> to run the interface message pump when waiting for a thread to complete 
    ''' </summary>
    ''' <remarks>Set <see cref="cCore.setMessagePumpDelegate">cCore.setMessagePumpDelegate(MessagePumpDelegate)</see> from the interface to allow the interface to process messages while waiting for a thread to complete.</remarks>
    Public Delegate Sub MessagePumpDelegate()

#End Region ' Public delegates

#Region " Generic variables "

    ''' <summary>Datasource used by the core for reading and writing model data.</summary>
    Private m_DataSource As IEwEDataSource = Nothing
    ''' <summary>Plug-in manager.</summary>
    Private WithEvents m_pluginManager As cPluginManager = Nothing

    ''' <summary>Core state monitor</summary>
    Private WithEvents m_StateMonitor As cCoreStateMonitor = Nothing
    ''' <summary>Core thread synchronization object for thread marshalling.</summary>
    Private m_SyncObj As System.Threading.SynchronizationContext = Nothing

    ''' <summary>Core state manager</summary>
    ''' <remarks>performs actions to bring core state up-to-date</remarks>
    Private m_StateManager As cCoreStateManager = Nothing

    Private m_ShapeManagers As New Dictionary(Of eDataTypes, cBaseShapeManager)
    Private m_PedigreeManagers As New Dictionary(Of eVarNameFlags, cPedigreeManager)
    Private m_MonteCarlo As cMonteCarloManager = Nothing
    Private m_ConTracer As cContaminantTracer = Nothing
    Private m_AdvectionManager As cAdvectionManager = Nothing
    'Private m_AdvectionParameters As cAdvectionParameters = Nothing

    ''' <summary>Class to wrap stand alone functions for internal and external access.</summary>
    Private m_Functions As cEcoFunctions = Nothing

    Private m_EwEModel As cEwEModel = Nothing
    Private m_stanzaGroups As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.Stanza, 0)
    Private m_timeSeriesDatasets As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.TimeSeriesDataset, 1)
    Private m_timeSeriesGroup As New cCoreInputOutputList(Of cTimeSeries)(eDataTypes.GroupTimeSeries, 1)
    Private m_timeSeriesFleet As New cCoreInputOutputList(Of cTimeSeries)(eDataTypes.FleetTimeSeries, 1)

    ''' <summary>Auxillary data not stored in a list, needs to be quickly accessible via hash keys</summary>
    ''' <remarks>Dictionary is 'friend' to be accessible to data source.</remarks>
    Friend m_dtAuxiliaryData As New Dictionary(Of String, cAuxiliaryData)

    ''' <summary>The central core message publisher.</summary>
    Friend m_publisher As New cMessagePublisher()
    Friend m_validators As cValidatorManager = Nothing

    Friend m_TSData As cTimeSeriesDataStructures = Nothing

    'jb 16-June-2016 Remove the cEcospaceTimeSeriesDataStructures when implementing Ecosim biomass forcing in EcoSpace
    'Ecospace can use the Cores cTimeSeriesDataStructures object until we need something better
    'Friend m_SpaceTSData As cEcospaceTimeSeriesDataStructures = Nothing

    Friend m_Stanza As cStanzaDatastructures = Nothing
    Friend m_FitToTimeSeriesData As cF2TSDataStructures = Nothing
    Friend m_tracerData As cContaminantTracerDataStructures = Nothing

    Private m_spatialdataconnectionManager As cSpatialDataConnectionManager = Nothing
    Friend m_SpatialData As cSpatialDataStructures = Nothing
    Private m_spatialOperationLog As cSpatialOperationLog = Nothing

    ''' <summary>
    ''' List of all multi threaded models/processes <see cref="IThreadedProcess">IThreadedProcess</see> that the core can run.
    ''' </summary>
    ''' <remarks>This list is used by the core to stop all running models when something major happens.</remarks>
    Private m_ThreadedProcesses As New List(Of IThreadedProcess)

    ''' <summary>Delegate to interrupt a run.</summary>
    Private m_dgtStop As StopRunDelegate = Nothing

    Private m_settings As New cCoreSettings()

    ''' <summary>Data for the Ecosim MSY search.</summary>
    Friend m_MSYData As MSY.cMSYDataStructures

    Private m_SampleManager As Samples.cEcopathSampleManager = Nothing
    Private m_ArenaManager As cEcosimArenaManager = Nothing
    Private Shared ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cCore)()


#If Not NET Then
    Private m_license As cLicense = Nothing
#End If

#End Region ' Generic variables

#Region " Private Initialization Flags "

    ''' <summary>Has the Core been initialized.</summary>
    ''' <remarks>True if a Core has been initialized.</remarks>
    Private m_bCoreIsInit As Boolean = False
    Private m_bEcoSimIsInit As Boolean = False

#End Region

#Region " Public Core Counters "

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
    ''' Dim iNumGroups As Integer = core.GetCoreCounter(eCoreCounterTypes.nGroups)
    ''' </code>
    ''' </remarks>
    Public Function GetCoreCounter(counterType As eCoreCounterTypes) As Integer
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
                Case eCoreCounterTypes.nEffortZones
                    Return Me.nEffortZones
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
                Case eCoreCounterTypes.nEnvironmentalDriverLayers
                    Return Me.nEnvironmentalDriverLayers
                    ' Case eCoreCounterTypes.nTrophicLevels
                    '     Return m_NetworkManager.nTrophicLevels
                Case eCoreCounterTypes.nRows
                    If (Me.m_EcospaceBasemap IsNot Nothing) Then
                        Return Me.m_EcospaceBasemap.InRow
                    Else
                        Return 0
                    End If
                Case eCoreCounterTypes.nCols
                    If (Me.m_EcospaceBasemap IsNot Nothing) Then
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
                Case eCoreCounterTypes.nPedigreeVariables
                    Return Me.nPedigreeVariables
                Case eCoreCounterTypes.nCapacityMaps
                    Return Me.CapacityMapInteractionManager.nEnviroData
                Case eCoreCounterTypes.nEcospaceResultWriters
                    If (Me.m_EcospaceModelParams IsNot Nothing) Then
                        Return Me.m_EcospaceModelParams.nResultWriters
                    Else
                        Return 0
                    End If
                Case eCoreCounterTypes.nMSEBATCHFixedF
                    Return Me.MSEBatchManager.BatchData.nFixedF
                Case eCoreCounterTypes.nMSEBATCHTAC
                    Return Me.MSEBatchManager.BatchData.nTAC
                Case eCoreCounterTypes.nMSEBatchTFM
                    Return Me.MSEBatchManager.BatchData.nTFM
                Case eCoreCounterTypes.nVectorFields
                    Return 2
                Case Else
                    'Debug.Assert(False, cStringUtils.Localize("{0}.GetCoreCounter() Invalid eCoreCounterTypes enumerator '{1}'.", Me.ToString(), counterType))
                    Return NULL_VALUE
            End Select

        Catch ex As Exception
            m_logger.LogError(ex, "GetCoreCounter {counterType}", counterType)
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
    Friend Function GetCoreCounter(SizeType As eCoreCounterTypes, iIndex As Integer) As Integer

        Select Case SizeType

            Case eCoreCounterTypes.nMaxStanzaAge
                Try
                    Return m_Stanza.Age2(iIndex, m_Stanza.Nstanza(iIndex))
                Catch ex As Exception
                    m_logger.LogError(ex, "GetCoreCounter nMaxStanzaAge for index {iIndex}", iIndex)
                    Return 0 '?
                End Try

            Case eCoreCounterTypes.nStanzasForStanzaGroup
                Try
                    Return m_Stanza.Nstanza(iIndex)
                Catch ex As Exception
                    m_logger.LogError(ex, "GetCoreCounter nStanzasForStanzaGroup for index {iIndex}", iIndex)
                    Return 0 '?
                End Try

            Case eCoreCounterTypes.nTaxonForGroup
                Try
                    Return Me.m_TaxonData.NumGroupTaxa(iIndex)
                Catch ex As Exception

                End Try
            Case Else
                Debug.Assert(False, "Invalid Counter passed to getCoreCounter(SizeType,iIndex)")
        End Select

    End Function

    ''' <summary>
    ''' Total number of groups across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nGroups"/>.
    ''' </remarks>
    Public ReadOnly Property nGroups() As Integer
        Get
            Return m_EcopathData.NumGroups
        End Get
    End Property

    ''' <summary>
    ''' Number of detritus groups across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nDetritus"/>.
    ''' </remarks>
    Public ReadOnly Property nDetritusGroups() As Integer
        Get
            Return m_EcopathData.NumDetrit
        End Get
    End Property

    ''' <summary>
    ''' Number of living groups across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nLivingGroups"/>.
    ''' </remarks>
    Public ReadOnly Property nLivingGroups() As Integer
        Get
            Return m_EcopathData.NumLiving
        End Get
    End Property

    ''' <summary>
    ''' Number of fishing fleets across all models.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nFleets"/>.
    ''' </remarks>
    Public ReadOnly Property nFleets() As Integer
        Get
            Return m_EcopathData.NumFleet
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace habitats.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nHabitats"/>.
    ''' </remarks>
    Public ReadOnly Property nHabitats() As Integer
        Get
            Return m_EcospaceData.NoHabitats
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace regions.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nRegions"/>.
    ''' </remarks>
    Public ReadOnly Property nRegions() As Integer
        Get
            Return m_EcospaceData.nRegions
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace effort zones.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEffortZones"/>.
    ''' </remarks>
    Public ReadOnly Property nEffortZones() As Integer
        Get
            Return m_EcospaceData.nEffZones
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace MPAs.
    ''' </summary>
    Public ReadOnly Property nMPAs() As Integer
        Get
            Return m_EcospaceData.MPAno
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace Importance layers.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nImportanceLayers"/>.
    ''' </remarks>
    Public ReadOnly Property nImportanceLayers() As Integer
        Get
            Return Me.m_EcospaceData.nImportanceLayers
        End Get
    End Property

    ''' <summary>
    ''' Number of Ecospace <see cref="cEcospaceLayerDriver">environmental driver layers</see>.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEnvironmentalDriverLayers"/>.
    ''' </remarks>
    Public ReadOnly Property nEnvironmentalDriverLayers() As Integer
        Get
            Return Me.m_EcospaceData.nEnvironmentalDriverLayers
        End Get
    End Property

    ''' <summary>
    ''' Number of years to run an Ecospace model.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEcospaceYears"/>.
    ''' </remarks>
    Public ReadOnly Property nEcospaceYears() As Integer
        Get
            Return CInt(m_EcospaceData.TotalTime)
        End Get
    End Property

    ''' <summary>
    ''' Number time steps in an Ecospace model.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEcospaceYears"/>.
    ''' </remarks>
    Public ReadOnly Property nEcospaceTimeSteps() As Integer
        Get
            Return m_EcospaceData.nTimeSteps
        End Get
    End Property

    ''' <summary>
    ''' Number of years to run an Ecosim model.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nEcosimYears"/>.
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
    ''' See <see cref="eCoreCounterTypes.nEcosimTimeSteps"/>.
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
    ''' <seealso cref="nStanzas"/>
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nMaxStanza"/>.
    ''' </remarks>
    Public ReadOnly Property nMaxStanza() As Integer
        Get
            Return m_Stanza.MaxStanza
        End Get
    End Property

    ''' <summary>
    ''' Number of stanza configurations. 
    ''' <seealso cref="nMaxStanza"/>
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nStanzas"/>.
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
    ''' See <see cref="eCoreCounterTypes.nTimeSeries"/>.
    ''' </remarks>
    Public ReadOnly Property nTimeSeries() As Integer
        Get
            Return m_TSData.nTimeSeries
        End Get
    End Property

    ''' <summary>
    ''' Number of applied time series.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nTimeSeriesApplied"/>.
    ''' </remarks>
    Public ReadOnly Property nTimeSeriesEnabled() As Integer
        Get
            Return m_TSData.AppliedNdatType
        End Get
    End Property

    ''' <summary>
    ''' Number of applied time series.
    ''' </summary>
    ''' <remarks>
    ''' See <see cref="eCoreCounterTypes.nTimeSeriesApplied"/>.
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
    ''' <seealso cref="eCoreCounterTypes.nTaxon"/>
    ''' </summary>
    Public ReadOnly Property nTaxon() As Integer
        Get
            Return Me.m_TaxonData.NumTaxon
        End Get
    End Property

    ''' <summary>
    ''' Get the number of pedigree variables.
    ''' <seealso cref="eCoreCounterTypes.nPedigreeVariables"/>
    ''' </summary>
    Public ReadOnly Property nPedigreeVariables() As Integer
        Get
            Return Me.m_EcopathData.NumPedigreeVariables
        End Get
    End Property

    ''' <summary>
    ''' Get the number of capacity maps
    ''' <seealso cref="eCoreCounterTypes.nCapacityMaps"/>.
    ''' </summary>
    Public ReadOnly Property nCapacityMaps() As Integer
        Get
            Return Me.CapacityMapInteractionManager.nEnviroData
        End Get
    End Property

    Public ReadOnly Property nEcospaceResultWriters As Integer
        Get
            Return Me.m_EcospaceModelParams.nResultWriters
        End Get
    End Property

#End Region ' Public Core Counters

#Region " Public Core Interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        Me.m_bCoreIsInit = False

        ' Create core data structures
        Me.m_EcopathData = New cEcopathDataStructures(Me.Messages)
        Me.m_EcoSimData = New cEcosimDatastructures
        Me.m_EcospaceData = New cEcospaceDataStructures(Me.Messages)
        Me.m_SpatialData = New cSpatialDataStructures(Me.m_EcopathData, Me.m_EcospaceData)
        Me.m_Stanza = New cStanzaDatastructures(Me.Messages)
        Me.m_tracerData = New cContaminantTracerDataStructures()
        Me.m_TSData = New cTimeSeriesDataStructures(Me.m_EcopathData, Me.m_EcoSimData)
        Me.m_MPAOptData = New cMPAOptDataStructures()
        Me.m_MSEData = New cMSEDataStructures(Me.m_EcopathData, Me.m_EcoSimData)
        Me.m_TaxonData = New cTaxonDataStructures(Me.m_EcopathData, Me.m_Stanza)
        Me.m_SampleData = New cEcopathSampleDatastructures(Me.m_EcopathData)

        Me.m_MSYData = New MSY.cMSYDataStructures(Me.m_EcopathData, Me.m_EcoSimData)

        ' Create core state monitor and manager
        Me.m_StateMonitor = New cCoreStateMonitor(Me)
        Me.m_StateManager = New cCoreStateManager(Me)

        Me.m_SyncObj = System.Threading.SynchronizationContext.Current
        'if there is no current context then create a new one on this thread. 
        If (Me.m_SyncObj Is Nothing) Then Me.m_SyncObj = New System.Threading.SynchronizationContext()

        Me.SaveWithFileHeader = True

        Me.InitCore()

    End Sub

    Private m_bDisposed As Boolean = False        ' To detect redundant calls

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not Me.m_bDisposed Then
            Try
                'Dispose of all the message handlers
                If Me.m_Ecopath.Messages IsNot Nothing Then
                    Me.m_Ecopath.Messages.Dispose()
                End If

                If Me.m_Ecosim.Messages IsNot Nothing Then
                    Me.m_Ecosim.Messages.Dispose()
                End If

                If Me.m_Ecospace.Messages IsNot Nothing Then
                    Me.m_Ecospace.Messages.Dispose()
                End If

                If Me.m_psdModel.Messages IsNot Nothing Then
                    Me.m_psdModel.Messages.Dispose()
                End If

                Me.Messages.Dispose()
                'cCoreEnumNamesIndex.GetInstance.Dispose()

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".Dispose() Exception: " & ex.Message)
            End Try
            Me.m_bDisposed = True
        End If
        GC.SuppressFinalize(Me)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name to identify a core instance with.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Core assembly version, formatted as a string.
    ''' </summary>
    ''' <param name="bIncludeBitness">Inlcude 32 or 64 bitness in version string.</param>
    ''' <param name="bIncludeCompilationDate">Inlcude compilation date in version string.</param>
    ''' -----------------------------------------------------------------------
    Public Shared ReadOnly Property Version(Optional bIncludeCompilationDate As Boolean = False, Optional bIncludeBitness As Boolean = False) As String
        Get
            Try
                Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.GetAssembly(GetType(cCore))
                Dim an As System.Reflection.AssemblyName = ass.GetName()
                Dim strVersion As String = cAssemblyUtils.GetVersion(an).ToString

                If bIncludeCompilationDate Then
                    strVersion = cStringUtils.Localize(My.Resources.CoreDefaults.VERSION_EXT_COMPILED, strVersion, cAssemblyUtils.GetCompileDate(ass).ToShortDateString)
                End If

                If bIncludeBitness Then
                    strVersion = cStringUtils.Localize("{0} - {1}", strVersion, If(cSystemUtils.Is64BitProcess, My.Resources.CoreDefaults.BITNESS_64, My.Resources.CoreDefaults.BITNESS_32))
                End If

                Return strVersion
            Catch ex As Exception
                m_logger.LogError(ex, "Version")
                Return ""
            End Try
        End Get
    End Property

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
    ''' <returns>True if successful.</returns>
    ''' <remarks>This will perform a full model save to the temporary data source
    ''' passed to this method.</remarks>
    ''' -----------------------------------------------------------------------
    Friend Function Export(ds As IEwEDataSource) As Boolean
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
        TimeSeries = 2
        Ecospace = 3
        Ecotracer = 4
        NotSet = 777 ' Just the highest number, and a random value at that :p
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
    Public Function SetBatchLock(batchLockType As eBatchLockType) As Boolean

        If (Me.m_DataSource Is Nothing) Then Return False

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

        If Me.m_iBatchLock = 1 Then
            Me.DataSource.BeginTransaction()
            Me.StateMonitor.SetIsBatchLocked(True)
        End If

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
    Public Function ReleaseBatchLock(batchChangeLevel As eBatchChangeLevelFlags,
            Optional bCommit As Boolean = True) As Boolean

        If (Me.m_DataSource Is Nothing) Then Return False

        ' Sanity checks: validate batch lock type
        Debug.Assert(Me.m_batchLockType <> eBatchLockType.NotSet, "Cannot release a batch lock; no current lock active")

        ' Decrease batch lock count
        Me.m_iBatchLock -= 1

        ' Keep track of most serious impact level
        Me.m_batchChangeLevel = DirectCast(Math.Min(CInt(batchChangeLevel), CInt(Me.m_batchChangeLevel)), eBatchChangeLevelFlags)

        ' Last batch lock released?
        If (Not Me.IsBatchLocked()) Then

            Dim iSim As Integer = Me.ActiveEcosimScenarioIndex
            Dim iSpace As Integer = Me.ActiveEcospaceScenarioIndex
            Dim iTracer As Integer = Me.ActiveEcotracerScenarioIndex

            Me.DataSource.EndTransaction(bCommit)
            Me.StateMonitor.SetIsBatchLocked(False)

            ' Need to reload?
            If (Me.m_batchLockType = eBatchLockType.Restructure) Then

                ' Determine level of reload
                ' JS 24 March 2010: Only reload a scenario when the batch release affected that particular level.
                '                   In other words, adding or removing groups (batch level Ecopath) will NOT
                '                   cause batch level Ecosim and higher to automatically reload because Ecopath 
                '                   will most likely not run. This addresses issue #512
                Dim iEcosimScenarioToLoad As Integer = If(Me.m_batchChangeLevel <= eBatchChangeLevelFlags.Ecosim, iSim, cCore.NULL_VALUE)
                Dim iEcospaceScenarioToLoad As Integer = If(Me.m_batchChangeLevel <= eBatchChangeLevelFlags.Ecospace, iSpace, cCore.NULL_VALUE)
                Dim iEcotracerScenarioToLoad As Integer = If(Me.m_batchChangeLevel <= eBatchChangeLevelFlags.Ecotracer, iTracer, cCore.NULL_VALUE)
                Dim iDatasetToReload As Integer = 0

                If (Me.m_batchChangeLevel <= eBatchChangeLevelFlags.TimeSeries) Then
                    For ids As Integer = 1 To Me.nTimeSeriesDatasets
                        Dim ds As cTimeSeriesDataset = Me.TimeSeriesDataset(ids)
                        If ds.IsLoaded() Then iDatasetToReload = ids : Exit For
                    Next
                End If

                ' Reload restructured data
                ' JS 17 May 2010: If a scenario does not need reloading then the scenario is explicitly discarded.
                If (Me.m_batchChangeLevel = eBatchChangeLevelFlags.Ecopath) Then Me.LoadModel(Me.DataSource.ToString())
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
            Me.m_StateMonitor.UpdateExecutionState(eCoreComponentType.NotSet, TriState.True)

        End If

        ' Decrease messages lock count
        Me.Messages.RemoveMessageLock()

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether a <see cref="SetBatchLock">batch lock</see> is active.
    ''' </summary>
    ''' <returns>True if a <see cref="SetBatchLock">batch lock</see> is active.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function IsBatchLocked() As Boolean
        Return (Me.m_iBatchLock > 0)
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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddGroup(strName As String, sPP As Single, sVBK As Single,
            ByRef iGroup As Integer, ByRef iGroupID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        If iGroup < 1 And iGroup <> NULL_VALUE Then iGroup = 1 'less than 1 insert the new group as one

        ' iGroup value does not really matter. This addition may be part of a batch run; the data source 
        ' will take care of proper iGroup value assignments
        'If iGroup > nGroups Then iGroup = nGroups + 1 'greater then ngroups append the new group to the end this means the new group is a detritus group?????

        ' Must specify an iGroup value
        Debug.Assert(iGroup <> NULL_VALUE)

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ' Start the actual work
        If (DirectCast(Me.DataSource, IEcopathDataSource).AddGroup(strName, sPP, sVBK, iGroup, iGroupID)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.Ecopath, eDataTypes.EcoPathGroupInput)
            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.Ecopath, eDataTypes.EcoPathGroupOutput)
            Me.DataAddedOrRemovedMessage("Fleet number of groups has changed.", eCoreComponentType.Ecopath, eDataTypes.FleetInput)
            Me.DataAddedOrRemovedMessage("Stanza number of groups has changed.", eCoreComponentType.Ecopath, eDataTypes.Stanza)

            If m_bEcoSimIsInit And (m_EcoSimData.GroupDBID IsNot Nothing) Then
                Me.DataAddedOrRemovedMessage("EcoSim number of groups has changed.", eCoreComponentType.Ecosim, eDataTypes.EcoSimGroupInput)
            End If

            bSucces = True

        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a group from the EwE model.
    ''' </summary>
    ''' <param name="iGroup"><see cref="cCoreGroupBase.Index">One-based index of 
    ''' the group</see> to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveGroup(iGroup As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(Me.DataSource, IEcopathDataSource)
        If ds.RemoveGroup(Me.m_EcopathData.GroupDBID(iGroup)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.Ecopath, eDataTypes.EcoPathGroupInput)
            Me.DataAddedOrRemovedMessage("Ecopath number of groups has changed.", eCoreComponentType.Ecopath, eDataTypes.EcoPathGroupOutput)
            Me.DataAddedOrRemovedMessage("Fleet number of groups has changed.", eCoreComponentType.Ecopath, eDataTypes.FleetInput)
            Me.DataAddedOrRemovedMessage("Stanza number of groups has changed.", eCoreComponentType.Ecosim, eDataTypes.Stanza)

            If m_bEcoSimIsInit And (m_EcoSimData.GroupDBID IsNot Nothing) Then
                'load the Ecosim Groups with the Ecosim data reloaded from the database above
                Me.DataAddedOrRemovedMessage("EcoSim number of groups has changed.", eCoreComponentType.Ecosim, eDataTypes.EcoSimGroupInput)
            End If

            bSucces = True
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Move a group to a new position in the EwE model.
    ''' </summary>
    ''' <param name="iGroup"><see cref="cCoreGroupBase.Index">One-based index of 
    ''' the group</see> to remove.</param>
    ''' <param name="iIndex">New, one-based position of the group in the group list.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function MoveGroup(iGroup As Integer, iIndex As Integer) As Boolean
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.MoveGroup(Me.m_EcopathData.GroupDBID(iGroup), iIndex) Then

            Me.DataAddedOrRemovedMessage("Ecopath group order has changed.", eCoreComponentType.Ecopath, eDataTypes.EcoPathGroupInput)
            Me.DataAddedOrRemovedMessage("Ecopath group order has changed.", eCoreComponentType.Ecopath, eDataTypes.EcoPathGroupOutput)

            If m_bEcoSimIsInit And (m_EcoSimData.GroupDBID IsNot Nothing) Then
                'load the Ecosim Groups with the Ecosim data reloaded from the database above
                Me.DataAddedOrRemovedMessage("EcoSim group order has changed.", eCoreComponentType.Ecosim, eDataTypes.EcoSimGroupInput)
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
    Friend Function AddShape(strName As String, DataType As eDataTypes, ByRef newDBID As Integer,
                             Optional asData As Single() = Nothing,
                             Optional shapeType As Long = eShapeFunctionType.NotSet,
                             Optional parms As Single() = Nothing) As Boolean

        'the data source will allocate space in the EcoSim data arrays
        Dim ds As IEcosimDatasource = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcosimDatasource) Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        ds = DirectCast(Me.DataSource, IEcosimDatasource)
        bSucces = ds.AppendShape(strName, DataType, newDBID, asData, shapeType, parms)

        If bSucces = False Then
            'oops.....
            'do something
            'ToDo_jb this could throw an error back to the shape manager
        End If

        'At this time the shape manager has not had time to add the shape to its list 
        'so sending a message or telling the other manager what has happend is premature.
        'The shape manager will handle telling the other shape managers that it has changed the underlying data
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a shape from the EwE model.
    ''' </summary>
    ''' <param name="iDBID"><see cref="cShapeData.DBID">database ID</see> of the 
    ''' shape to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function RemoveShape(iDBID As Integer) As Boolean

        Dim ds As IEcosimDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcosimDatasource) Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        ds = DirectCast(Me.DataSource, IEcosimDatasource)
        'the data source is responsible for 
        '1 removing the record from the database
        '2 resizing the Ecosim data arrays
        '3 reloading the Ecosim data arrays with the values from the database
        'The shape manager that asked for the remove will handle loading the Ecosim data back into the shape managers
        Return ds.RemoveShape(iDBID)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether Ecosim has unused shape data - shape data allocated by
    ''' earlier, longer Ecosim runs - that is currently not used. This data can
    ''' be trimmed via <see cref="TrimUnusedShapeData"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasUnusedShapeData() As Boolean

        If (Not Me.StateMonitor.HasEcosimLoaded) Then Return False
        Return (Me.m_EcoSimData.ForcePoints > Me.m_EcoSimData.NTimes)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Trim forcing functions to the Ecosim number of time steps. To determine
    ''' if there is unused shape data check <see cref="HasUnusedShapeData"/>.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function TrimUnusedShapeData() As Boolean

        If (Not Me.CanSave(True)) Then Return False
        If (Not Me.StateMonitor.HasEcosimLoaded) Then Return False

        Try
            Me.m_EcoSimData.ForcePoints = Me.m_EcoSimData.NTimes
            For Each manager As cBaseShapeManager In m_ShapeManagers.Values
                manager.Update()
            Next
        Catch ex As Exception
            Return False
        End Try
        Return True

    End Function

#End Region

#End Region ' Public Core Interfaces

#Region " Private and Friend Core Functions " 'private functionality used by the core

    ''' <summary>
    ''' Initialize all core objects
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This initializes all the object that the core need to run a basic model (EcoPath). This does not load the model that happens in LoadModel(DataSource)</remarks>
    Friend Function InitCore() As Boolean

        m_bCoreIsInit = False
        m_bEcoSimIsInit = False

        'Ecofunctions is needed by Ecopath so make sure it is created before Ecopath
        m_Functions = New cEcoFunctions()

        'initialize the models
        'each models initialization will handle its own messages and flags
        Dim bsuccess As Boolean
        bsuccess = InitEcopath()
        bsuccess = bsuccess And InitEcoSim()
        bsuccess = bsuccess And InitEcospace()
        bsuccess = bsuccess And Me.InitPSD()

        m_SampleManager = New cEcopathSampleManager(Me)
        m_MonteCarlo = New cMonteCarloManager()
        m_ConTracer = New cContaminantTracer()
        m_ArenaManager = New cEcosimArenaManager(Me)

        InitThreadedProcesses()

        m_bCoreIsInit = bsuccess
        Return bsuccess

    End Function


    Private Sub InitThreadedProcesses()
        Try

            Me.m_ThreadedProcesses.Clear()
            'add all the search managers that are IThreadedProcesses 
            'to the list of threaded core processes Me.m_ThreadedProcesses
            For Each manager As ISearchObjective In Me.m_SearchManagers.Values
                If TypeOf manager Is IThreadedProcess Then
                    Me.m_ThreadedProcesses.Add(DirectCast(manager, IThreadedProcess))
                End If
            Next

            'EcoSpace implements the IThreadedProcess 
            Me.m_ThreadedProcesses.Add(Me.m_Ecospace)

            'MonteCarlo does not implement ISearchObjective so it is not in the SearchManager list me.m_SearchManagers
            'but does implements the IThreadedProcess 
            Me.m_ThreadedProcesses.Add(Me.m_MonteCarlo)

            ' Datasets are indexed in separate threads
            Me.m_ThreadedProcesses.Add(Me.SpatialDataConnectionManager.DatasetManager)

        Catch ex As Exception
            m_logger.LogError(ex, "InitThreadedProcesses")
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Function InitEcopath() As Boolean

        Try
            Dim mh As New cMessageHandler(AddressOf Me.EcopathMessage_Handler, eCoreComponentType.Ecopath, eMessageType.Any, Me.m_SyncObj)
#If DEBUG Then
            mh.Name = "cCore::Ecopath"
#End If

            'build a new EcoPath Model object
            Me.m_Ecopath = New Ecopath.cEcopathModel(Me.m_Functions)
            Me.m_Ecopath.Messages.AddMessageHandler(mh)
            Me.m_Ecopath.m_stanza = Me.m_Stanza
            Me.m_Ecopath.m_psd = Me.m_PSDData

            'the Ecopath Data belongs to the core instead of Ecopath so that it can be shared by all the models
            Me.m_Ecopath.EcopathData = Me.m_EcopathData


            'protect against error loading the validators
            Try
                m_validators = New cValidatorManager(Me)
            Catch ex As Exception
                'the validation manager creates all the validators. Make sure we know if something went wrong
                Dim msg As cMessage = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.CORE_INIT_CRITICAL_VALIDATORS, ex.Message),
                        eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical)
                'the message publisher is declared with the new operator so it already exists 
                m_publisher.AddMessage(msg)
                m_publisher.sendAllMessages()
                Return False
            End Try

        Catch ex As Exception
            'Major Error ???????
            Dim msg As cMessage = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.CORE_INIT_CRITICAL_GENERIC, ex.Message),
                    eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical)
            'the message publisher is declared with the new operator so it already exists 
            m_publisher.AddMessage(msg)
            m_publisher.sendAllMessages()
            Return False
        End Try

        Return True

    End Function

    Private Function InitPSD() As Boolean
        Try
            Dim mh As New cMessageHandler(AddressOf Me.PSDMessage_Handler, eCoreComponentType.Ecopath, eMessageType.Any, Me.m_SyncObj)
#If DEBUG Then
            mh.Name = "cCore::PSD"
#End If

            Me.m_psdModel = New cPSDModel
            Me.m_PSDData = New cPSDDatastructures(Me.m_EcopathData)
            Me.m_psdModel.Messages.AddMessageHandler(mh)

            Me.m_psdModel.m_Data = m_EcopathData
            Me.m_psdModel.m_stanza = m_Stanza
            Me.m_psdModel.m_psd = m_PSDData
            Return True
        Catch ex As Exception
            m_logger.LogError(ex, "InitPSD")
            Return False
        End Try
    End Function

    Private Sub ClosePSD()

        Try
            If Me.m_psdModel IsNot Nothing Then
                'sets all array to nothing
                Me.m_PSDData.Clear()
            End If
        Catch ex As Exception
            m_logger.LogError(ex, "ClosePSD")
        End Try

    End Sub

    ''' <summary>
    ''' Send a Data <see cref="eMessageType.DataAddedOrRemoved">added or removed</see> message.
    ''' </summary>
    ''' <param name="message">Test of the message</param>
    ''' <param name="dataType">eDataTypes enumerator for the type of data</param>
    ''' <remarks>This is just to wrap the creation and sending of a data added or removed message to clean up the code a bit</remarks>
    Private Sub DataAddedOrRemovedMessage(ByRef message As String, messageSource As eCoreComponentType, dataType As eDataTypes, Optional vars() As cVariableStatus = Nothing)

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
    ''' Send a <see cref="eMessageType.DataModified">data modified</see> message.
    ''' </summary>
    ''' <param name="message">Test of the message</param>
    ''' <param name="dataType">eDataTypes enumerator for the type of data</param>
    ''' <remarks>This is just to wrap the creation and sending of a data modified message to clean up the code a bit</remarks>
    Private Sub DataModifiedMessage(ByRef message As String, messageSource As eCoreComponentType, dataType As eDataTypes, Optional vars() As cVariableStatus = Nothing)

        ' Create msg
        Dim msg As New cMessage(message, eMessageType.DataModified, messageSource, eMessageImportance.Maintenance, dataType)
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
        For i As Integer = m_EcopathData.NumLiving + 1 To m_EcopathData.NumGroups
            If m_EcopathData.BH(i) < 0 Then

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

    Private Function FindObjectByDBID(lItems As IList, iDBID As Integer) As cCoreInputOutputBase
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

    Private Function FindObjectByIndex(lItems As IList, iIndex As Integer) As cCoreInputOutputBase
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

    Private Function FindObjectByName(lItems As cCoreInputOutputList(Of cCoreInputOutputBase), strName As String) As cCoreInputOutputBase
        Dim obj As cCoreInputOutputBase = Nothing

        For Each objTest As Object In lItems
            If TypeOf (objTest) Is cCoreInputOutputBase Then
                If (String.Compare(strName, DirectCast(objTest, cCoreInputOutputBase).Name, True) = 0) Then
                    obj = DirectCast(objTest, cCoreInputOutputBase)
                    Exit For
                End If
            End If
        Next
        Return obj
    End Function

#End Region ' Private and Friend Core Functions

#Region " Time series "

#Region " Import "

    ''' <summary>
    ''' Import one time series into the database.
    ''' </summary>
    ''' <param name="ts">The <see cref="cTimeSeries">cTimeSeries-derived</see> object to import.</param>
    ''' <returns>True if successful.</returns>
    Public Function ImportEcosimTimeSeries(ts As cTimeSeriesImport, iDataset As Integer) As Boolean

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

            For Each ds As cTimeSeriesDataset In Me.m_timeSeriesDatasets
                ds.Clear()
            Next
            Me.m_timeSeriesDatasets.Clear()

            For iDS As Integer = 1 To Me.m_TSData.nDatasets
                tsd = New cTimeSeriesDataset(Me, Me.m_TSData.DatasetNumTimeSeries(iDS))
                tsd.AllowValidation = False
                tsd.DBID = Me.m_TSData.DatasetDBID(iDS)
                tsd.Index = iDS
                tsd.Name = Me.m_TSData.DatasetName(iDS)
                tsd.FirstYear = Me.m_TSData.DatasetFirstYear(iDS)
                tsd.NumPoints = Me.m_TSData.DatasetNumPoints(iDS)
                tsd.TimeSeriesInterval = Me.m_TSData.DataSetIntervals(iDS)
                tsd.AllowValidation = True

                Me.m_timeSeriesDatasets.Add(tsd)

            Next

            ' Reset number of loaded and applied time series
            Me.m_TSData.ClearTimeSeries()
            ' Set number of groups
            Me.m_TSData.nGroups = Me.nGroups
            Me.m_TSData.nFleets = Me.nFleets

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
            ts = cTimeSeriesFactory.CreateTimeSeries(Me.m_TSData.TimeSeriesType(iSeries), Me, Me.m_TSData.TimeSeriesDBID(iSeries))
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
                iNumYears = Me.m_TSData.DatasetNumPoints(Me.ActiveTimeSeriesDatasetIndex)
            End If

            For Each ts As cGroupTimeSeries In Me.m_timeSeriesGroup

                ts.LockUpdates()

                ts.Name = Me.m_TSData.TimeSeriesName(ts.Index)
                ts.Index = ts.Index
                ts.DBID = Me.m_TSData.TimeSeriesDBID(ts.Index)
                ts.TimeSeriesType = Me.m_TSData.TimeSeriesType(ts.Index)
                ts.GroupIndex = Me.m_TSData.TimeSeriesPool(ts.Index)
                ts.WtType = Me.m_TSData.TimeSeriesWeight(ts.Index)
                ts.CV = Me.m_TSData.TimeSeriesCV(ts.Index)

                ts.DataSS = Me.m_TSData.TimeSeriesSSPredErr(ts.Index)
                ts.DataQ = Me.m_TSData.TimeSeriesDatQ(ts.Index)
                ts.eDataQ = Me.m_TSData.TimeSeriesEDatQ(ts.Index)
                ts.Interval = Me.m_TSData.AppliedDataSetInterval

                ts.ResizeData(iNumYears)
                For iYear As Integer = 1 To iNumYears
                    ts.DatVal(iYear) = Me.m_TSData.TimeSeriesValues(iYear, ts.Index)
                Next iYear

                Me.ValidateTimeSeries(ts)

                ts.Enabled = Me.m_TSData.TimeSeriesEnabled(ts.Index)
                ts.UnlockUpdates(False)

            Next

            For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet

                ts.LockUpdates()

                ts.Name = Me.m_TSData.TimeSeriesName(ts.Index)
                ts.Index = ts.Index
                ts.DBID = Me.m_TSData.TimeSeriesDBID(ts.Index)
                ts.TimeSeriesType = Me.m_TSData.TimeSeriesType(ts.Index)
                ts.FleetIndex = Me.m_TSData.TimeSeriesPool(ts.Index)
                ts.GroupIndex = Me.m_TSData.TimeSeriesPoolSec(ts.Index)
                ts.WtType = Me.m_TSData.TimeSeriesWeight(ts.Index)
                ts.CV = Me.m_TSData.TimeSeriesCV(ts.Index)

                'DatSS and DatQ are not part of m_TSData yet
                ts.DataSS = Me.m_TSData.TimeSeriesSSPredErr(ts.Index)
                ts.DataQ = Me.m_TSData.TimeSeriesDatQ(ts.Index)
                ts.eDataQ = Me.m_TSData.TimeSeriesEDatQ(ts.Index)
                ts.Interval = Me.m_TSData.AppliedDataSetInterval

                ts.ResizeData(iNumYears)
                For iYear As Integer = 1 To iNumYears
                    ts.DatVal(iYear) = Me.m_TSData.TimeSeriesValues(iYear, ts.Index)
                Next iYear

                Me.ValidateTimeSeries(ts)

                ts.Enabled = Me.m_TSData.TimeSeriesEnabled(ts.Index)
                ts.UnlockUpdates(False)

            Next

        Catch ex As Exception
            bSucces = False
        End Try

        If (Me.PluginManager IsNot Nothing) Then
            Me.PluginManager.EcosimLoadedTimeSeries()
        End If

        Return bSucces

    End Function

    Private Function LoadEcosimTimeSeriesStats() As Boolean
        Dim tsd As cTimeSeriesDataset = Nothing

        Dim bSucces As Boolean = True

        Try
            If (Me.ActiveTimeSeriesDatasetIndex > 0) Then
                tsd = Me.TimeSeriesDataset(Me.ActiveTimeSeriesDatasetIndex)
            End If

            For Each ts As cGroupTimeSeries In Me.m_timeSeriesGroup

                ts.LockUpdates()

                ts.DataSS = Me.m_TSData.TimeSeriesSSPredErr(ts.Index)
                ts.DataQ = Me.m_TSData.TimeSeriesDatQ(ts.Index)
                ts.eDataQ = Me.m_TSData.TimeSeriesEDatQ(ts.Index)
                ts.Interval = Me.m_TSData.AppliedDataSetInterval

                ts.UnlockUpdates(False)

            Next

            For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet

                ts.LockUpdates()

                ts.DataSS = Me.m_TSData.TimeSeriesSSPredErr(ts.Index)
                ts.DataQ = Me.m_TSData.TimeSeriesDatQ(ts.Index)
                ts.eDataQ = Me.m_TSData.TimeSeriesEDatQ(ts.Index)

                ts.UnlockUpdates(False)

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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function UpdateEcosimTimeSeries() As Boolean
        Dim bSucces As Boolean = (Me.UpdateEcosimGroupTimeSeries() And Me.UpdateEcosimFleetTimeSeries())
        Return bSucces
    End Function

    Private Function UpdateEcosimGroupTimeSeries() As Boolean

        Dim cc As eCoreComponentType = eCoreComponentType.NotSet
        Dim bSucces As Boolean = True

        Try
            For Each ts As cGroupTimeSeries In Me.m_timeSeriesGroup

                ' Validate whether TS will remain in its category (group)
                Debug.Assert(cTimeSeries.Category(ts.TimeSeriesType) = eTimeSeriesCategoryType.Group, "Cannot change TS to a different category")
                Me.m_TSData.TimeSeriesType(ts.Index) = ts.TimeSeriesType
                Me.m_TSData.TimeSeriesName(ts.Index) = ts.Name
                Me.m_TSData.TimeSeriesPool(ts.Index) = ts.GroupIndex
                Me.m_TSData.TimeSeriesWeight(ts.Index) = ts.WtType
                Me.m_TSData.TimeSeriesCV(ts.Index) = ts.CV

                'DatSS and DatQ are computed so they are not updated from the interface
                'Me.m_TSData.datass(ts.Index) = ts.DataQ
                'Me.m_TSData.Datq(ts.Index) = ts.DataSS 

                ' Update core DatVal
                For iYear As Integer = 1 To ts.nPoints
                    Me.m_TSData.TimeSeriesValues(iYear, ts.Index) = ts.DatVal(iYear)
                Next iYear

                Me.m_TSData.TimeSeriesEnabled(ts.Index) = ts.Enabled
                cc = ts.CoreComponent
                Me.ValidateTimeSeries(ts)

            Next

            If (cc = eCoreComponentType.NotSet) Then Me.DataSource.SetChanged(cc)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function UpdateEcosimFleetTimeSeries() As Boolean

        Dim cc As eCoreComponentType = eCoreComponentType.NotSet
        Dim bSucces As Boolean = True

        Try
            For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet

                ' Validate whether TS will remain in its category
                Debug.Assert(cTimeSeries.Category(ts.TimeSeriesType) = eTimeSeriesCategoryType.Fleet Or
                             cTimeSeries.Category(ts.TimeSeriesType) = eTimeSeriesCategoryType.FleetGroup, "Cannot change TS to a different category")
                Me.m_TSData.TimeSeriesType(ts.Index) = ts.TimeSeriesType
                Me.m_TSData.TimeSeriesName(ts.Index) = ts.Name
                Me.m_TSData.TimeSeriesPool(ts.Index) = ts.FleetIndex
                Me.m_TSData.TimeSeriesPoolSec(ts.Index) = ts.GroupIndex
                Me.m_TSData.TimeSeriesWeight(ts.Index) = ts.WtType
                Me.m_TSData.TimeSeriesCV(ts.Index) = ts.CV

                'DatSS and DatQ are computed so they are not updated from the interface
                'Me.m_TSData.datass(ts.Index) = ts.DataQ
                'Me.m_TSData.Datq(ts.Index) = ts.DataSS 

                ' Update core DatVal
                For iYear As Integer = 1 To ts.nPoints
                    Me.m_TSData.TimeSeriesValues(iYear, ts.Index) = ts.DatVal(iYear)
                Next iYear

                Me.m_TSData.TimeSeriesEnabled(ts.Index) = ts.Enabled
                cc = ts.CoreComponent
                Me.ValidateTimeSeries(ts)

            Next

            If (cc = eCoreComponentType.NotSet) Then Me.DataSource.SetChanged(cc)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' Update

#Region " Validation "

    Private Sub ValidateTimeSeries(ts As cTimeSeries)

        Dim status As eStatusFlags = eStatusFlags.OK
        Dim strStatus As String = ""

        If TypeOf ts Is cGroupTimeSeries Then
            Dim gts As cGroupTimeSeries = DirectCast(ts, cGroupTimeSeries)
            status = gts.GroupIndexStatus
            If (status = eStatusFlags.ErrorEncountered) Then
                strStatus = cStringUtils.Localize(My.Resources.CoreMessages.TIMESERIES_ERROR_INVALIDGROUP, gts.GroupIndex)
            End If
        End If

        If TypeOf ts Is cFleetTimeSeries Then
            Dim fts As cFleetTimeSeries = DirectCast(ts, cFleetTimeSeries)
            status = fts.FleetIndexStatus
            If (status = eStatusFlags.ErrorEncountered) Then
                strStatus = cStringUtils.Localize(My.Resources.CoreMessages.TIMESERIES_ERROR_INVALIDFLEET, fts.FleetIndex)
            Else
                status = fts.GroupIndexStatus
                If (status = eStatusFlags.ErrorEncountered) Then
                    strStatus = cStringUtils.Localize(My.Resources.CoreMessages.TIMESERIES_ERROR_INVALIDGROUP, fts.GroupIndex)
                End If
            End If
        End If

        ts.ValidationStatus = status
        ts.ValidationMessage = strStatus

    End Sub

#End Region ' Validation

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the one-based index of the active <see cref="cTimeSeriesDataset">TimeSeries Dataset</see>.
    ''' If no time series are loaded, a value &lt; 1 is returned.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveTimeSeriesDatasetIndex() As Integer
        Get
            Return Me.m_TSData.ActiveDatasetIndex
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain a <see cref="cTimeSeriesDataset">time series dataset</see>.
    ''' </summary>
    ''' <param name="iDatasetIndex">One-based index of the dataset to retrieve [1, <see cref="ActiveTimeSeriesDatasetIndex">#sets</see>].</param>
    ''' <returns>A <see cref="cTimeSeriesDataset">time series dataset</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Function TimeSeriesDataset(iDatasetIndex As Integer) As cTimeSeriesDataset
        ' Sanity check
        Debug.Assert(iDatasetIndex > 0 And iDatasetIndex <= Me.m_TSData.nDatasets)
        Return DirectCast(Me.m_timeSeriesDatasets(iDatasetIndex), cTimeSeriesDataset)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strDataset">Name of dataset that was loaded</param>
    ''' <param name="strError"></param>
    ''' <remarks></remarks>
    Private Sub SendTimeSeriesLoadMessage(strDataset As String, Optional strError As String = "")

        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            If String.IsNullOrEmpty(strDataset) Then
                strText = My.Resources.CoreMessages.TIMESERIES_UNLOAD_SUCCESS
            Else
                strText = cStringUtils.Localize(My.Resources.CoreMessages.TIMESERIES_LOAD_SUCCESS, strDataset)
            End If
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.TimeSeries, eMessageImportance.Information, eDataTypes.TimeSeriesDataset)
        Else
            If String.IsNullOrEmpty(strDataset) Then
                strText = cStringUtils.Localize(My.Resources.CoreMessages.TIMESERIES_UNLOAD_FAILED, strError)
            Else
                strText = cStringUtils.Localize(My.Resources.CoreMessages.TIMESERIES_LOAD_FAILED, strDataset, strError)
            End If
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.TimeSeries, eMessageImportance.Warning, eDataTypes.TimeSeriesDataset)
        End If

        Me.m_publisher.AddMessage(msg)
        Me.m_publisher.sendAllMessages()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load (and optionally apply) a single time series dataset
    ''' </summary>
    ''' <param name="tsd">The dataset to load. Provide 'nothing' to unload any dataset.</param>
    ''' <param name="bEnable">Flag stating whether loaded time series should be enabled immediately. True by default</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadTimeSeries(tsd As cTimeSeriesDataset,
                                   Optional bEnable As Boolean = True) As Boolean
        Dim iIndex As Integer = 0
        If tsd IsNot Nothing Then iIndex = tsd.Index
        Return Me.LoadTimeSeries(iIndex, bEnable)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load (and optionally apply) a single time series dataset
    ''' </summary>
    ''' <param name="iDataset">One-based index of the dataset to load. Provide 0 to unload any dataset.</param>
    ''' <param name="bEnable">Flag stating whether loaded time series should be enabled immediately. True by default.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadTimeSeries(iDataset As Integer,
                                   Optional bEnable As Boolean = True) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity check
        If ((iDataset < 0) Or (iDataset > Me.nTimeSeriesDatasets)) Then Return bSucces

        ' Sanity checks
        If Me.DataSource Is Nothing Then Return bSucces

        ' Ask for saving
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return bSucces

        If (TypeOf Me.DataSource Is IEcosimDatasource) Then
            Dim sds As IEcosimDatasource = DirectCast(Me.DataSource, IEcosimDatasource)

            ' Can load dataset succesfully?
            If sds.LoadTimeSeriesDataset(iDataset) Then
                ' #Yes: Can init core interface objects succesfully?
                If Me.InitEcosimTimeSeries() Then
                    ' #Yes: Can populate core interface objects succesfully?
                    If (Me.LoadEcosimTimeSeries()) Then
                        ' Need to apply too?
                        If (bEnable = True) Then
                            ' #Yes: Apply
                            For Each ts As cTimeSeries In Me.m_timeSeriesGroup : ts.Enabled = True : Next
                            For Each ts As cTimeSeries In Me.m_timeSeriesFleet : ts.Enabled = True : Next
                        End If
                        ' Send messages
                        If iDataset > 0 Then
                            Me.SendTimeSeriesLoadMessage(Me.TimeSeriesDataset(iDataset).Name)
                        Else
                            Me.SendTimeSeriesLoadMessage("")
                        End If
                        ' Flag as successful
                        bSucces = True
                    End If
                End If
                Me.UpdateTimeSeries()

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
    Public Function EcosimTimeSeries(iIndex As Integer) As cTimeSeries
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

    Public Function EcosimGroupTimeseries() As cTimeSeries()
        Return Me.m_timeSeriesGroup.ToArray
    End Function

    Public Function EcosimFleetTimeseries() As cTimeSeries()
        Return Me.m_timeSeriesFleet.ToArray
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply all <see cref="cTimeSeries">Time Series</see> that are flagged as
    ''' <see cref="cTimeSeries.Enabled">Enabled</see> to the Ecosim model.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function UpdateTimeSeries(Optional bDirtyDatasource As Boolean = False) As Boolean

        ' ToDo: merge this functionality with TS handling in OnChanged. We now 
        '       have two separate pathways for processing TS changes which 
        '       should be merged

        Dim lstEffortToReset As New List(Of Integer)
        Dim bChanged As Boolean = False

        ' Update enable flags and weights
        For Each ts As cGroupTimeSeries In Me.m_timeSeriesGroup
            bChanged = bChanged Or (Me.m_TSData.TimeSeriesEnabled(ts.Index) <> ts.Enabled)
            Me.m_TSData.TimeSeriesEnabled(ts.Index) = ts.Enabled
            bChanged = bChanged Or (Me.m_TSData.TimeSeriesWeight(ts.Index) <> ts.WtType) Or (Me.m_TSData.TimeSeriesCV(ts.Index) <> ts.CV)
            Me.m_TSData.TimeSeriesWeight(ts.Index) = ts.WtType
            Me.m_TSData.TimeSeriesCV(ts.Index) = ts.CV
        Next
        For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet
            bChanged = bChanged Or (Me.m_TSData.TimeSeriesEnabled(ts.Index) <> ts.Enabled)
            Me.m_TSData.TimeSeriesEnabled(ts.Index) = ts.Enabled
            bChanged = bChanged Or (Me.m_TSData.TimeSeriesWeight(ts.Index) <> ts.WtType) Or (Me.m_TSData.TimeSeriesCV(ts.Index) <> ts.CV)
            Me.m_TSData.TimeSeriesWeight(ts.Index) = ts.WtType
            Me.m_TSData.TimeSeriesCV(ts.Index) = ts.CV
        Next

        For Each ts As cFleetTimeSeries In Me.m_timeSeriesFleet
            'build a list of all disabled Effort timeseries that need to be reset to default (one)
            If ts.TimeSeriesType = eTimeSeriesType.FishingEffort Then
                If Me.m_TSData.TimeSeriesEnabled(ts.Index) = True And ts.Enabled = False Then
                    'Timeseries is being disabled so keep the index to reset effort from
                    lstEffortToReset.Add(ts.DatPool)
                End If
            End If
            Me.m_TSData.TimeSeriesEnabled(ts.Index) = ts.Enabled
        Next

        ' Load enabled TS
        Me.m_TSData.loadEnabled()

        ' Ecosim needs to run again, but do not screw up the data state
        Me.StateMonitor.SetEcoSimLoaded(True, bResetDataState:=False)

        'setEcosimRunLength() will call DoDatValCalculations to re-load forcing data
        If Me.ActiveTimeSeriesDatasetIndex > 0 Then
            Me.setEcosimRunLength(Me.m_TSData.nYears, True)
        Else
            Me.setEcosimRunLength(Me.m_EcoSimData.NumYears, True)
        End If

        'Set Default Ecospace Biomass Forcing values
        'all groups that have Ecosim biomass forcing will be forced in Ecospace
        Me.UpdateEcospaceForcingByEcosim()

        'reset all efforts that were unloaded/disabled
        Me.m_EcoSimData.setEffortToDefault(lstEffortToReset)

        Me.m_Ecosim.SetBaseFFromGear()

        Me.m_SearchManagers(eDataTypes.FitToTimeSeries).Load()

        Me.m_ShapeManagers.Item(eDataTypes.FishingEffort).Load()
        Me.m_ShapeManagers.Item(eDataTypes.FishMort).Load()

        'tell the interface that the shapes have changed
        Me.m_publisher.AddMessage(New cMessage("Fish rate shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishingEffort))
        Me.m_publisher.AddMessage(New cMessage("Fish mort shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishMort))

        If (bChanged) Then
            If (bDirtyDatasource) Then
                Me.DataAddedOrRemovedMessage("Time Series have been updated", eCoreComponentType.TimeSeries, eDataTypes.NotSet)
                DataSource.SetChanged(eCoreComponentType.TimeSeries)
                Me.m_StateMonitor.UpdateDataState(DataSource)
            Else
                Me.DataModifiedMessage("Time Series have changed", eCoreComponentType.TimeSeries, eDataTypes.NotSet)
            End If
        End If

        Me.Messages.sendAllMessages()

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states whether the model has time series loaded.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function HasTimeSeries() As Boolean
        Return (Me.m_TSData.nTimeSeries > 0)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states whether the model has time series applied.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function HasAppliedTimeSeries() As Boolean
        Return (Me.m_TSData.AppliedNdatType > 0)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an Ecosim Time Series to the data source.
    ''' </summary>
    ''' <param name="strName">Name of the new Time Series to add.</param>
    ''' <param name="iPool">Index of item to assign this TS to.</param>
    ''' <param name="iPoolSec">Index of secundary item to assign this TS to.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of the time series.</param>
    ''' <param name="asValues">Initial values to set in the TS.</param>
    ''' <param name="iDBID">Database ID assigned to the new TS.</param>
    ''' -----------------------------------------------------------------------
    Public Function AddTimeSeries(strName As String,
                                  iPool As Integer, iPoolSec As Integer,
                                  timeSeriesType As eTimeSeriesType,
                                  sWeight As Single, asValues() As Single,
                                  ByRef iDBID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf DataSource Is IEcosimDatasource) Then Return False

        ' Set bach lock for adding and removing items
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        Try
            ' Try to add TS to the data source
            If DirectCast(DataSource, IEcosimDatasource).AppendTimeSeries(strName, iPool, iPoolSec, timeSeriesType, sWeight, asValues, iDBID) Then
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
    ''' Remove an Ecosim Time Series from the data source.
    ''' </summary>
    ''' <param name="TS"><see cref="cTimeSeries">Time Series instance</see> to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Function RemoveTimeSeries(TS As cTimeSeries) As Boolean
        Return Me.RemoveTimeSeries(TS.DBID)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an Ecosim Time Series from the data source.
    ''' </summary>
    ''' <param name="DBID">Database ID of the time series to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Function RemoveTimeSeries(DBID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Safety check
        If Not TypeOf DataSource Is IEcosimDatasource Then Return False

        ' Set bach lock for adding and removing items
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        Try
            ' Try to add TS to the data source
            If DirectCast(DataSource, IEcosimDatasource).RemoveTimeSeries(DBID) Then
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
    ''' <param name="iNumPoints">The number of data points in the dataset.</param>
    ''' <param name="interval">The <see cref="eTSDataSetInterval">interval</see> between two points in the dataset.</param>
    ''' <param name="iDataset">Index of the new time series dataset if the 
    ''' operation completed succesfully.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AppendTimeSeriesDataset(strName As String,
                                            strDescription As String,
                                            strAuthor As String,
                                            strContact As String,
                                            iFirstYear As Integer,
                                            iNumPoints As Integer,
                                            interval As eTSDataSetInterval,
                                            ByRef iDataset As Integer) As Boolean

        Dim ds As IEcosimDatasource = Nothing
        Dim iDatasetID As Integer = 0

        ' Safety check
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf DataSource Is IEcosimDatasource) Then Return False

        If Me.m_StateMonitor.HasEcosimLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        Try

            ds = DirectCast(DataSource, IEcosimDatasource)
            If ds.AppendTimeSeriesDataset(strName, strDescription, strAuthor, strContact, iFirstYear, iNumPoints, interval, iDatasetID) Then

                Me.InitAndLoadEcosimTimeSeriesDatasets()
                Me.DataAddedOrRemovedMessage("Number of time series datasets has changed.", eCoreComponentType.TimeSeries, eDataTypes.TimeSeriesDataset)
                iDataset = Array.IndexOf(Me.m_TSData.DatasetDBID, iDatasetID)
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

    Public Function RemoveTimeSeriesDataset(iDatasetIndex As Integer) As Boolean
        Return Me.RemoveTimeSeriesDataset(Me.TimeSeriesDataset(iDatasetIndex))
    End Function

    Public Function RemoveTimeSeriesDataset(dataset As cTimeSeriesDataset) As Boolean
        Dim bSucces As Boolean = False

        ' Safety check
        If Not TypeOf DataSource Is IEcosimDatasource Then Return False
        Try
            ' Try to add TS to the data source
            If DirectCast(DataSource, IEcosimDatasource).RemoveTimeSeriesDataset(dataset.Index) Then
                Me.DataAddedOrRemovedMessage("Ecosim number of time series has changed.", eCoreComponentType.TimeSeries, eDataTypes.TimeSeriesDataset)
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

#Region " Generic helper methods "

    ''' <summary>
    ''' Creates a new cMessage Object
    ''' </summary>
    ''' <param name="message">Message to send</param>
    ''' <param name="source">Source of the message</param>
    ''' <param name="MessageType">Type of message</param>
    ''' <returns>A new cMessage Object</returns>
    ''' <remarks>Used as a simple way to build a new message instance.</remarks>
    Private Function CreateMessage(message As String, source As eCoreComponentType, MessageType As eMessageType) As cMessage
        Dim msg As New cMessage
        msg.Message = message
        msg.Source = source
        msg.Type = MessageType
        Return msg
    End Function

    ''' -------------------------------------------------------------------------
    ''' <summary>
    ''' Public interface to save changes.
    ''' </summary>
    ''' <param name="bQuiet">Flag stating whether to suppress any user prompts.</param>
    ''' <param name="savelevel">The MINIMUM level of data to save. For instance,
    ''' when loading a new Ecospace scenario, any pending Ecospace changes have
    ''' to be stored but there is no need to save Sim or Path. A savelevel value 
    ''' of <see cref="eBatchChangeLevelFlags.Ecospace">Ecospace</see> would 
    ''' achieve this.</param>
    ''' <returns>True if successful.</returns>
    ''' -------------------------------------------------------------------------
    Public Function SaveChanges(Optional bQuiet As Boolean = False,
                                Optional savelevel As eBatchChangeLevelFlags = eBatchChangeLevelFlags.Ecopath) As Boolean

        Dim fm As cFeedbackMessage = Nothing
        Dim msg As cMessage = Nothing
        Dim strPrompt As String = ""
        Dim terms As New List(Of String)
        Dim bSuccess As Boolean = True

        ' Hang on, can we do this at all?
        If (Me.DataSource Is Nothing) Then Return True

        ' In a batch?
        If (Me.m_iBatchLock > 0) Then Return True

        ' Just to be sure
        Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)

        ' Assess tracer
        Dim bIsModified As Boolean = Me.m_StateMonitor.IsEcotracerModified()
        If (savelevel = eBatchChangeLevelFlags.Ecotracer) Then If Not bIsModified Then Return True
        If (savelevel <= eBatchChangeLevelFlags.Ecotracer And Me.m_StateMonitor.IsEcotracerModified()) Then terms.Insert(0, "Ecotracer")

        ' Assess ecospace
        bIsModified = bIsModified Or Me.m_StateMonitor.IsEcospaceModified()
        If (savelevel = eBatchChangeLevelFlags.Ecospace) Then If Not bIsModified Then Return True
        If (savelevel <= eBatchChangeLevelFlags.Ecospace And Me.m_StateMonitor.IsEcospaceModified()) Then terms.Insert(0, "Ecospace")

        ' Assess Ecosim
        bIsModified = bIsModified Or Me.m_StateMonitor.IsEcosimModified()
        If (savelevel = eBatchChangeLevelFlags.Ecosim) Then If Not bIsModified Then Return True
        If (savelevel <= eBatchChangeLevelFlags.Ecosim And Me.m_StateMonitor.IsEcosimModified()) Then terms.Insert(0, "Ecosim")

        ' Assess Ecopath
        If (Not Me.m_StateMonitor.IsModified) Then Return True
        If (savelevel <= eBatchChangeLevelFlags.Ecopath) And (Me.m_StateMonitor.IsEcopathModified()) Then terms.Insert(0, "Ecopath")

        If (Me.m_StateMonitor.IsPluginModified) Then terms.Add(My.Resources.CoreDefaults.SOURCE_PLUGINS)

        ' OK, changes are assessed. Now decide how to handle these changes, which may require user input.

        ' Read-only datasources require special prompt
        If (Me.DataSource.IsReadOnly = True) Then
            ' Prepare feedback message
            strPrompt = My.Resources.CoreMessages.PROMPT_DISCARD_CHANGES
            fm = New cFeedbackMessage(strPrompt,
                                      eCoreComponentType.Core, eMessageType.Any,
                                      eMessageImportance.Maintenance, eMessageReplyStyle.YES_NO)
            ' Auto-affirm
            fm.Reply = eMessageReply.YES

            ' Send and see what happens
            If (Not bQuiet) Then Me.m_publisher.SendMessage(fm)

            Select Case fm.Reply
                Case eMessageReply.YES
                    Me.DiscardChanges()
                    Return True
                Case Else
                    Return False
            End Select
        End If

        ' Prepare feedback message
        If (savelevel = eBatchChangeLevelFlags.NotSet) Then
            strPrompt = My.Resources.CoreMessages.PROMPT_SAVE_CHANGES
        Else
            strPrompt = cStringUtils.Localize(My.Resources.CoreMessages.PROMPT_SAVE_CHANGES_DETAILED, cStringUtils.FormatList(terms, True))
        End If
        fm = New cFeedbackMessage(strPrompt,
                                  eCoreComponentType.Core, eMessageType.Any,
                                  eMessageImportance.Maintenance, eMessageReplyStyle.YES_NO_CANCEL)

        ' Auto-affirm
        fm.Reply = eMessageReply.YES
        ' Send and see what happens
        If (Not bQuiet) Then Me.m_publisher.SendMessage(fm)

        ' Do not save
        If (fm.Reply = eMessageReply.CANCEL) Then Return False
        If (fm.Reply = eMessageReply.NO) Then Me.DiscardChanges() : Return True

        ' Check if save must be aborted
        If (Me.PluginManager IsNot Nothing) Then
            Dim bCancel As Boolean = False
            Me.PluginManager.SaveChanges(bCancel)
            If (bCancel) Then Return False
        End If

        ' Send progress message
        msg = New cProgressMessage(eProgressState.Start, 0, -1, My.Resources.CoreMessages.STATUS_SAVING_CHANGES, eMessageType.DataExport)
        Me.Messages.SendMessage(msg, True)

        ' Plug-ins
        If (Me.PluginManager IsNot Nothing) And (bSuccess = True) And (Me.m_StateMonitor.IsPluginModified) Then
            If (Not Me.PluginManager.SaveModel(Me.DataSource)) Then
                bSuccess = False
            Else
                Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
            End If
        End If

        ' Ecotracer
        If (bSuccess And (savelevel <= eBatchChangeLevelFlags.Ecotracer)) Then
            If (Me.m_StateMonitor.IsEcotracerModified) Then
                If (Not Me.SaveEcotracerScenario()) Then
                    bSuccess = False
                Else
                    Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                End If
            End If
        End If

        ' Ecospace
        If (bSuccess And (savelevel <= eBatchChangeLevelFlags.Ecospace)) Then
            If (Me.m_StateMonitor.IsEcospaceModified) Then
                If (Not Me.SaveEcospaceScenario()) Then
                    bSuccess = False
                Else
                    Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                End If
            End If
        End If

        ' Ecosim
        If (bSuccess And (savelevel <= eBatchChangeLevelFlags.Ecosim)) Then
            If (Me.m_StateMonitor.IsEcosimModified) Then
                If (Not Me.SaveEcosimScenario()) Then
                    bSuccess = False
                Else
                    Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                End If
            End If
        End If

        ' The bottom of it all
        If (bSuccess And (savelevel <= eBatchChangeLevelFlags.Ecopath)) Then
            If (Me.m_StateMonitor.IsEcopathModified Or Me.m_StateMonitor.IsDatasourceModified) Then
                If (Not Me.SaveModel()) Then
                    bSuccess = False
                Else
                    Me.m_StateMonitor.UpdateDataState(Me.DataSource, TriState.False)
                End If
            End If
        End If

        msg = New cProgressMessage(eProgressState.Finished, 0, 0, "", eMessageType.DataExport)
        Me.Messages.SendMessage(msg, True)

        ' Report success
        Return bSuccess

    End Function

    ''' -------------------------------------------------------------------------
    ''' <summary>
    ''' Discard any unsaved data change flags.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' <remarks>
    ''' Although data is not physically discarded, the core will no longer attempt 
    ''' to <see cref="SaveChanges">save changes</see> until a new data edit is made.
    ''' </remarks>
    ''' -------------------------------------------------------------------------
    Public Function DiscardChanges() As Boolean

        ' Hang on, can we do this at all?
        If (Me.DataSource Is Nothing) Then Return False

        Dim bCancel As Boolean = False
        If (Me.PluginManager IsNot Nothing) Then
            Me.PluginManager.DiscardChanges(bCancel)
        End If
        If (bCancel) Then Return False

        Me.DataSource.ClearChanged()
        Me.m_StateMonitor.UpdateDataState(Me.DataSource)
        Return True

    End Function

    ''' -------------------------------------------------------------------------
    ''' <summary>
    ''' Check whether the EwE data needs saving.
    ''' </summary>
    ''' <returns>True if the EwE data needs saving.</returns>
    ''' -------------------------------------------------------------------------
    Public Function HasChanges() As Boolean

        If (Me.DataSource Is Nothing) Then Return False
        Return Me.DataSource.IsModified

    End Function

    Private m_dtCustomAutosaveFolders As New Dictionary(Of eAutosaveTypes, String)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the custom folder to save scenario data under. This is very useful when running tools such as
    ''' MultiSim, where plug-ins responding to MultiSim perturbations need to save under a non-standard Ecosim folder.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CustomAutosaveFolder(savetype As eAutosaveTypes) As String
        Get
            If Not Me.m_dtCustomAutosaveFolders.ContainsKey(savetype) Then Return ""
            Return Me.m_dtCustomAutosaveFolders(savetype)
        End Get
        Set(value As String)
            If String.IsNullOrWhiteSpace(value) Then
                If (Me.m_dtCustomAutosaveFolders.ContainsKey(savetype)) Then Me.m_dtCustomAutosaveFolders.Remove(savetype)
            Else
                Me.m_dtCustomAutosaveFolders(savetype) = cFileUtils.ToValidFileName(value, True)
            End If
        End Set
    End Property

    ''' -------------------------------------------------------------------------
    ''' <summary>
    ''' Get the default output location for a given <see cref="eAutosaveTypes">autosaving component</see>.
    ''' </summary>
    ''' <param name="type">The <see cref="eAutosaveTypes">autosaving component</see>
    ''' to get return the default path for.</param>
    ''' <param name="strBasePath">The base directory to place the output folder under, 
    ''' if any. if left empty the current <see cref="OutputPath"> is assumed.</see></param>
    ''' -------------------------------------------------------------------------
    Public ReadOnly Property DefaultOutputPath(type As eAutosaveTypes,
                                               Optional strBasePath As String = "") As String
        Get
            Dim strModel As String = ""
            Dim strScenario As String = ""
            Dim strPath As String = ""

            If String.IsNullOrWhiteSpace(strBasePath) Then
                strBasePath = Me.OutputPath
            End If

            If (Me.DataSource IsNot Nothing) And (type <> eAutosaveTypes.NotSet) Then
                strModel = Path.GetFileNameWithoutExtension(Me.DataSource.FileName)
            Else
                strModel = "{model}"
            End If

            strScenario = Me.CustomAutosaveFolder(type)
            If (String.IsNullOrWhiteSpace(strScenario)) Then

                Select Case type
                    Case eAutosaveTypes.Ecopath
                    ' NOP

                    Case eAutosaveTypes.Ecosim, eAutosaveTypes.EcosimResults
                        strScenario = "ecosim_"
                        If (Me.ActiveEcosimScenarioIndex > 0) Then
                            strScenario = strScenario & Me.EcosimScenarios(Me.ActiveEcosimScenarioIndex).Name
                        Else
                            strScenario = strScenario & "{scenario}"
                        End If

                    Case eAutosaveTypes.MonteCarlo
                        strScenario = "mc_"
                        If (Me.ActiveEcosimScenarioIndex > 0) Then
                            strScenario = strScenario & Me.EcosimScenarios(Me.ActiveEcosimScenarioIndex).Name
                        Else
                            strScenario = strScenario & "{scenario}"
                        End If

                    Case eAutosaveTypes.MSE
                        strScenario = "mse_"
                        If (Me.ActiveEcosimScenarioIndex > 0) Then
                            strScenario = strScenario & Me.EcosimScenarios(Me.ActiveEcosimScenarioIndex).Name
                        Else
                            strScenario = strScenario & "{scenario}"
                        End If

                    Case eAutosaveTypes.MSY
                        strScenario = "msy_"
                        If (Me.ActiveEcosimScenarioIndex > 0) Then
                            strScenario = strScenario & Me.EcosimScenarios(Me.ActiveEcosimScenarioIndex).Name
                        Else
                            strScenario = strScenario & "{scenario}"
                        End If

                    Case eAutosaveTypes.Ecospace, eAutosaveTypes.EcospaceResults
                        If (Me.ActiveEcospaceScenarioIndex > 0) Then
                            strScenario = strScenario & Me.EcospaceScenarios(Me.ActiveEcospaceScenarioIndex).Name
                        Else
                            strScenario = strScenario & "{scenario}"
                        End If

                    Case eAutosaveTypes.MPAOpt
                        strScenario = "mpa_opt_"
                        If (Me.ActiveEcospaceScenarioIndex > 0) Then
                            strScenario = strScenario & Me.EcospaceScenarios(Me.ActiveEcospaceScenarioIndex).Name
                        Else
                            strScenario = strScenario & "{scenario}"
                        End If

                    Case eAutosaveTypes.Ecotracer
                        strScenario = "ecotracer_"
                        If (Me.ActiveEcotracerScenarioIndex > 0) Then
                            strScenario = strScenario & Me.EcotracerScenarios(Me.ActiveEcotracerScenarioIndex).Name
                        Else
                            strScenario = strScenario & "{scenario}"
                        End If

                End Select
                strScenario = cFileUtils.ToValidFileName(strScenario, False)
            End If

            cPathUtility.ResolvePath(strBasePath, Me, strPath)

            If Not String.IsNullOrWhiteSpace(strScenario) Then
                strPath = Path.Combine(strPath, strScenario)
            End If

            Return strPath

        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the path for core processes to write output information to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property OutputPath() As String
        Get
            Dim strPath As String = Me.m_settings.OutputPath

            If String.IsNullOrWhiteSpace(strPath) Then
                If Me.DataSource IsNot Nothing Then
                    Return Path.Combine(Me.DataSource.Directory, "EwE output")
                End If
                strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            End If
            Return strPath
        End Get
        Set(value As String)
            Try
                Dim strPath As String = cFileUtils.ToValidFileName(value, True)
                If (String.Compare(strPath, Me.m_settings.OutputPath) <> 0) Then
                    Me.m_settings.OutputPath = strPath
                    Me.Messages.SendMessage(New cMessage("Output path has changed", eMessageType.GlobalSettingsChanged, eCoreComponentType.Core, eMessageImportance.Maintenance))
                End If
            Catch ex As Exception
                m_logger.LogError(ex, "cCore::OutputPath({value})", value)
            End Try
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a <see cref="eAutosaveTypes">auto-save capable component</see>
    ''' is allowed to auto-save.
    ''' </summary>
    ''' <param name="savetype">The <see cref="eAutosaveTypes">auto-save capable component</see>
    ''' to access this setting for.</param>
    ''' -----------------------------------------------------------------------
    Public Property Autosave(savetype As eAutosaveTypes) As Boolean
        Get
            Try
                Return Me.m_settings.Autosave(savetype)
            Catch ex As Exception
                m_logger.LogError(ex, "cCore::Autosave({savetype})", savetype.ToString)
            End Try
            Return False
        End Get
        Set(value As Boolean)
            Try
                If (value <> Me.m_settings.Autosave(savetype)) Then
                    Me.m_settings.Autosave(savetype) = value
                    Me.OnSettingsChanged()
                End If
            Catch ex As Exception
                m_logger.LogError(ex, "cCore::Autosave({savetype})", savetype.ToString)
            End Try

        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Flag, stating whether results should be saved with default header information.
    ''' </summary>
    ''' <seealso cref="DefaultFileHeader(eAutosaveTypes, Integer, Dictionary(Of String, String))"/>
    ''' <seealso cref="DefaultFileHeader(XmlDocument, eAutosaveTypes, Integer, Dictionary(Of String, String))"/>
    ''' -----------------------------------------------------------------------
    Public Property SaveWithFileHeader As Boolean
        Get
            Try
                Return Me.m_settings.AutosaveHeaders
            Catch ex As Exception
                m_logger.LogError(ex, "cCore::SaveWithFileHeader")
            End Try
            Return False
        End Get
        Set(value As Boolean)
            Try
                If (value <> Me.m_settings.AutosaveHeaders) Then
                    Me.m_settings.AutosaveHeaders = value
                    Me.OnSettingsChanged()
                End If
            Catch ex As Exception
                m_logger.LogError(ex, "cCore::SaveWithFileHeader")
            End Try
        End Set
    End Property

    Private Function HeaderInfo(savetype As eAutosaveTypes,
                                Optional iStartYear As Integer = cCore.NULL_VALUE) As Dictionary(Of String, String)

        Dim fields As New Dictionary(Of String, String)

        Select Case savetype
            Case eAutosaveTypes.NotSet
                fields("EwEVersion") = cCore.Version(True)
                fields("Date") = Date.Now.ToString()

            Case eAutosaveTypes.Ecopath
                If Not Me.m_StateMonitor.HasEcopathLoaded Then Return fields
                fields("ModelName") = Me.EwEModel.Name
                fields("ModelSource") = Me.DataSource.ToString()

            Case eAutosaveTypes.Ecosim
                If Not Me.m_StateMonitor.HasEcosimLoaded Then Return fields
                fields("EcosimScenario") = Me.EcosimScenarios(Me.ActiveEcosimScenarioIndex).Name
                fields("TimeSeries") = If(Me.ActiveTimeSeriesDatasetIndex > 0, Me.TimeSeriesDataset(Me.ActiveTimeSeriesDatasetIndex).Name, "-")
                If (iStartYear = cCore.NULL_VALUE) Then iStartYear = Me.EcosimFirstYear
                fields("StartYear") = CStr(iStartYear)

            Case eAutosaveTypes.Ecospace
                If Not Me.m_StateMonitor.HasEcospaceLoaded Then Return fields
                Dim bm As cEcospaceBasemap = Me.m_EcospaceBasemap
                Dim ld As cEcospaceLayerDepth = bm.LayerDepth
                Dim man As cSpatialDataConnectionManager = Me.SpatialDataConnectionManager

                fields("EcospaceScenario") = Me.EcospaceScenarios(Me.ActiveEcospaceScenarioIndex).Name
                fields("MapRows") = cStringUtils.FormatNumber(Me.m_EcospaceData.InRow)
                fields("MapCols") = cStringUtils.FormatNumber(Me.m_EcospaceData.InCol)
                fields("MapCellLength") = cStringUtils.FormatNumber(Me.m_EcospaceData.CellLength)
                fields("MapCellSize") = cStringUtils.FormatNumber(Me.m_EcospaceBasemap.CellSize())
                fields("MapTopLeftLat") = cStringUtils.FormatNumber(Me.m_EcospaceData.Lat1)
                fields("MapTopLeftLon") = cStringUtils.FormatNumber(Me.m_EcospaceData.Lon1)
                fields("NoActiveCells") = cStringUtils.FormatNumber(ld.NumActiveCells)
                fields("EcoSpaceTimeStepLength") = cStringUtils.FormatNumber(Me.m_EcospaceData.TimeStep)
                fields("CoordinateSystemWKT") = Me.m_EcospaceData.ProjectionString.Replace("""", "'")
                fields("ExternalDataConfigFile") = Me.SpatialDatasetManager.CurrentConfigFile

                ' Gather spat temp connections
                Try
                    For Each adt As cSpatialDataAdapter In man.Adapters
                        ' Only map layers delivered by the Ecospace basemap are listed in the header. That may not be enough
                        For Each l As cEcospaceLayer In Me.EcospaceBasemap.Layers(adt.VarName)
                            If (l IsNot Nothing) Then
                                Dim conns() As cSpatialDataConnection = adt.Connections(l.Index, True)
                                For i As Integer = 1 To conns.Count
                                    Dim conn As cSpatialDataConnection = conns(i - 1)
                                    If (conn.IsConfigured) Then
                                        fields("Layer_" & adt.VarName.ToString() & "_" & i & "_""" & l.Name & """") = conn.Dataset.CustomName
                                    End If
                                Next
                            End If
                        Next
                    Next
                Catch ex As Exception
                    m_logger.LogError(ex, "cCore.HeaderInfo-spattemp")
                End Try
            Case eAutosaveTypes.Ecotracer
                fields("EcotracerScenario") = Me.EcotracerScenarios(Me.ActiveEcotracerScenarioIndex).Name

        End Select

        Return fields

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a default file header for text files for a given 
    ''' <see cref="eAutosaveTypes">auto-save type</see>, representing loaded
    ''' aspects of ecopath, ecosim, ecospace and ecotracer, where applicable.
    ''' This header block can be integrated in CSV files.
    ''' <seealso cref="DefaultFileHeader"/>
    ''' </summary>
    ''' <param name="savetype">The <see cref="eAutosaveTypes">auto-save type</see>
    ''' to obtain the generic file header for.</param>
    ''' <param name="iStartYear">Optional start year to include in the header. If 
    ''' omitted, the <see cref="cCore.EcosimFirstYear"/> will be used.</param>
    ''' <returns>A text block safe for integration in CSV files.</returns>
    ''' <seealso cref="SaveWithFileHeader"/>
    ''' <seealso cref="DefaultFileHeader(XmlDocument, eAutosaveTypes, Integer, Dictionary(Of String, String))"/>
    ''' -----------------------------------------------------------------------
    Public Function DefaultFileHeader(savetype As eAutosaveTypes,
                                      Optional iStartYear As Integer = cCore.NULL_VALUE,
                                      Optional extraFields As Dictionary(Of String, String) = Nothing) As String

        Dim sb As New StringBuilder()
        Dim sm As cCoreStateMonitor = Me.StateMonitor

        If (iStartYear <= 0) Then iStartYear = Me.EcosimFirstYear

        sb.AppendLine(cStringUtils.ToCSVField("<HEADER software/>"))
        Dim dt As Dictionary(Of String, String) = HeaderInfo(eAutosaveTypes.NotSet, iStartYear)
        For Each field As String In dt.Keys
            sb.AppendLine(cStringUtils.ToCSVField(field) & "," & cStringUtils.ToCSVField(dt(field)))
        Next

        If (savetype > eAutosaveTypes.NotSet) And (sm.HasEcopathLoaded) Then
            sb.AppendLine(cStringUtils.ToCSVField("<HEADER ecopath/>"))
            dt = HeaderInfo(eAutosaveTypes.Ecopath, iStartYear)
            For Each field As String In dt.Keys
                sb.AppendLine(cStringUtils.ToCSVField(field) & "," & cStringUtils.ToCSVField(dt(field)))
            Next
        End If

        If (savetype >= eAutosaveTypes.Ecosim) And (sm.HasEcosimLoaded) Then
            sb.AppendLine(cStringUtils.ToCSVField("<HEADER ecosim/>"))
            dt = HeaderInfo(eAutosaveTypes.Ecosim, iStartYear)
            For Each field As String In dt.Keys
                sb.AppendLine(cStringUtils.ToCSVField(field) & "," & cStringUtils.ToCSVField(dt(field)))
            Next
        End If

        If (savetype >= eAutosaveTypes.Ecospace) And (sm.HasEcospaceLoaded) Then
            sb.AppendLine(cStringUtils.ToCSVField("<HEADER ecospace/>"))
            dt = HeaderInfo(eAutosaveTypes.Ecospace, iStartYear)
            For Each field As String In dt.Keys
                sb.AppendLine(cStringUtils.ToCSVField(field) & "," & cStringUtils.ToCSVField(dt(field)))
            Next
        End If

        If (savetype >= eAutosaveTypes.Ecotracer) And (sm.HasEcotracerLoaded) Then
            sb.AppendLine(cStringUtils.ToCSVField("<HEADER ecotracer/>"))
            dt = HeaderInfo(eAutosaveTypes.Ecotracer, iStartYear)
            For Each field As String In dt.Keys
                sb.AppendLine(cStringUtils.ToCSVField(field) & "," & cStringUtils.ToCSVField(dt(field)))
            Next
        End If

        If (extraFields IsNot Nothing) Then
            If (extraFields.Count > 0) Then
                sb.AppendLine(cStringUtils.ToCSVField("<HEADER extra/>"))
                For Each field As String In extraFields.Keys
                    sb.AppendLine(cStringUtils.ToCSVField(field) & "," & cStringUtils.ToCSVField(extraFields(field)))
                Next
            End If
        End If

        sb.AppendLine(cStringUtils.ToCSVField("<HEADER end/>"))

        Return sb.ToString

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a default file header for XML files for a given 
    ''' <see cref="eAutosaveTypes">auto-save type</see>, representing loaded
    ''' aspects of ecopath, ecosim, ecospace and ecotracer, where applicable.
    ''' This header block can be integrated in XML files.
    ''' <seealso cref="DefaultFileHeader"/>
    ''' </summary>
    ''' <param name="savetype">The <see cref="eAutosaveTypes">auto-save type</see>
    ''' to obtain the generic file header for.</param>
    ''' <param name="iStartYear">Optional start year to include in the header. If 
    ''' omitted, the <see cref="cCore.EcosimFirstYear"/> will be used.</param>
    ''' <returns>A XML node structure describing the EwE run, safe for integration in XML files.</returns>
    ''' <seealso cref="SaveWithFileHeader"/>
    ''' <seealso cref="DefaultFileHeader(eAutosaveTypes, Integer, Dictionary(Of String, String))"/>
    ''' -----------------------------------------------------------------------
    Public Function DefaultFileHeader(doc As XmlDocument,
                                      savetype As eAutosaveTypes,
                                      Optional iStartYear As Integer = cCore.NULL_VALUE,
                                      Optional extraFields As Dictionary(Of String, String) = Nothing) As XmlNode

        Dim xnHeader As XmlNode = doc.CreateElement("Header")
        Dim xn As XmlNode = Nothing
        Dim xa As XmlAttribute = Nothing
        Dim sm As cCoreStateMonitor = Me.StateMonitor

        Dim dt As Dictionary(Of String, String) = HeaderInfo(eAutosaveTypes.NotSet, iStartYear)
        xn = doc.CreateElement("Software")
        For Each field As String In dt.Keys
            xa = doc.CreateAttribute(cXMLUtils.XMLNodeName(field))
            xa.Value = cXMLUtils.XMLNodeValue(dt(field))
            xn.Attributes.Append(xa)
        Next
        xnHeader.AppendChild(xn)

        If (sm.HasEcopathLoaded() And savetype > eAutosaveTypes.NotSet) Then
            xn = doc.CreateElement("Ecopath")
            dt = HeaderInfo(eAutosaveTypes.Ecopath, iStartYear)
            For Each field As String In dt.Keys
                xa = doc.CreateAttribute(cXMLUtils.XMLNodeName(field))
                xa.Value = cXMLUtils.XMLNodeValue(dt(field))
                xn.Attributes.Append(xa)
            Next
            xnHeader.AppendChild(xn)
        End If

        If (savetype >= eAutosaveTypes.Ecosim) And (sm.HasEcosimLoaded) Then
            xn = doc.CreateElement("Ecosim")
            dt = HeaderInfo(eAutosaveTypes.Ecosim, iStartYear)
            For Each field As String In dt.Keys
                xa = doc.CreateAttribute(cXMLUtils.XMLNodeName(field))
                xa.Value = cXMLUtils.XMLNodeValue(dt(field))
                xn.Attributes.Append(xa)
            Next
            xnHeader.AppendChild(xn)
        End If

        If (savetype >= eAutosaveTypes.Ecospace) And (sm.HasEcospaceLoaded) Then
            xn = doc.CreateElement("Ecospace")
            dt = HeaderInfo(eAutosaveTypes.Ecospace, iStartYear)
            For Each field As String In dt.Keys
                xa = doc.CreateAttribute(cXMLUtils.XMLNodeName(field))
                xa.Value = cXMLUtils.XMLNodeValue(dt(field))
                xn.Attributes.Append(xa)
            Next
            xnHeader.AppendChild(xn)
        End If

        If (savetype >= eAutosaveTypes.Ecotracer) And (sm.HasEcotracerLoaded) Then
            xn = doc.CreateElement("Ecotracer")
            dt = HeaderInfo(eAutosaveTypes.Ecotracer, iStartYear)
            For Each field As String In dt.Keys
                xa = doc.CreateAttribute(cStringUtils.ToCSVField(field))
                xa.Value = cXMLUtils.XMLNodeValue(dt(field))
                xn.Attributes.Append(xa)
            Next
            xnHeader.AppendChild(xn)
        End If

        If (extraFields IsNot Nothing) Then
            If (extraFields.Count > 0) Then
                xn = doc.CreateElement("Extra")
                For Each field As String In extraFields.Keys
                    xa = doc.CreateAttribute(cStringUtils.ToCSVField(field))
                    xa.Value = cXMLUtils.XMLNodeValue(dt(field))
                    xn.Attributes.Append(xa)
                Next
                xnHeader.AppendChild(xn)
            End If
        End If

        Return xnHeader

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Send out a notification that core settings have changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnSettingsChanged()
        Try
            ' ToDo: globalize this
            Me.Messages.SendMessage(New cMessage("Global settings have changed", eMessageType.GlobalSettingsChanged, eCoreComponentType.Core, eMessageImportance.Maintenance))
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default author name to use for EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DefaultAuthor As String
        Get
            Return Me.m_settings.Author
        End Get
        Set(value As String)
            Me.m_settings.Author = value
            Me.Messages.SendMessage(New cMessage("Default author has changed", eMessageType.GlobalSettingsChanged, eCoreComponentType.Core, eMessageImportance.Maintenance))
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default author contact information to use for EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DefaultContact As String
        Get
            Return Me.m_settings.Contact
        End Get
        Set(value As String)
            Me.m_settings.Contact = value
            Me.Messages.SendMessage(New cMessage("Default contact information has changed", eMessageType.GlobalSettingsChanged, eCoreComponentType.Core, eMessageImportance.Maintenance))
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default number of threads for the use of EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ThreatCount As Integer
        Get
            Return Me.m_settings.ThreatCount
        End Get
        Set(value As Integer)
            Me.m_settings.ThreatCount = value
            Me.Messages.SendMessage(New cMessage("Default threat count has changed", eMessageType.GlobalSettingsChanged, eCoreComponentType.Core, eMessageImportance.Maintenance))
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check wether a means to stop any running process is in place.
    ''' </summary>
    ''' <returns>True if any current running process can be stopped.</returns>
    ''' <remarks>
    ''' <para>To set a means to stop any running process see <see cref="SetStopRunDelegate"/>.</para>
    ''' <para>Call <see cref="StopRun"/> to invoke this delegate. The implementation that this delegate refers to is 
    ''' responsible for implementing the stopping of the process</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function CanStopRun() As Boolean
        If Not (Me.m_StateMonitor.IsBusy) Then
            Return False
        End If
        Return (Me.m_dgtStop IsNot Nothing)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stop any running process, if possible.
    ''' </summary>
    ''' <remarks>
    ''' <para>To set a means to stop any running process see <see cref="SetStopRunDelegate"/>.</para>
    ''' <para>Check <see cref="CanStopRun"/> to see if a stop delegate is in place.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub StopRun()
        If Me.CanStopRun Then
            Me.m_dgtStop.Invoke()
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide the delegate that the core can call to stop any running process.
    ''' </summary>
    ''' <param name="dgt">The <see cref="StopRunDelegate">delegate</see> that the core 
    ''' can call to stop a running process. if Nothing is provided the core loses
    ''' its ability to abort a running process.</param>
    ''' <remarks>
    ''' <para>Call <see cref="StopRun"/> to invoke this delegate. The implementation 
    ''' that this delegate refers to is responsible for implementing the stopping 
    ''' of the process</para>
    ''' <para>Check <see cref="CanStopRun"/> to see if a stop delegate is in place.</para>
    ''' <para>Note that this delegate is cleared any time the core detects the end 
    ''' of a running process.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub SetStopRunDelegate(dgt As StopRunDelegate)
        Me.m_dgtStop = dgt
    End Sub


    ''' <summary>
    ''' Sets a delegate that a Manager can use to pump messages in the interface waiting for a process/thread to complete.
    ''' </summary>
    ''' <param name="InterfaceMessagePumpDelegate">Delegate from an interface to run the interfaces message pump.</param>
    ''' <remarks>This is used to prevent a thread deadlock when a Manager is waiting for a process to complete. </remarks>
    Public Sub SetMessagePumpDelegate(InterfaceMessagePumpDelegate As MessagePumpDelegate)
        Try
            For Each threadProcess As IThreadedProcess In Me.m_ThreadedProcesses
                threadProcess.MessagePump = InterfaceMessagePumpDelegate
            Next
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Generic helper methods

#Region " Datasource "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The <see cref="IEwEDataSource">data source</see> that the core will use
    ''' for reading and writing model data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DataSource() As IEwEDataSource
        Get
            Return Me.m_DataSource
        End Get
        Private Set(value As IEwEDataSource)
            ' Assign new DS
            Me.m_DataSource = value
        End Set
    End Property

    Public Property BackupFileMask() As String
        Get
            Return Me.m_settings.BackupFileMask
        End Get
        Set(value As String)
            Me.m_settings.BackupFileMask = value
        End Set
    End Property

    Private Function UpdateDatasource(ds As IEwEDataSource) As Boolean

        ' Do not update a read-only database
        If (ds.IsReadOnly) Then Return True

        ' Run database updates
        If (TypeOf ds.Connection Is cEwEDatabase) Then
            Dim db As cEwEDatabase = DirectCast(ds.Connection, cEwEDatabase)
            Dim dbUpd As New cDatabaseUpdater(Me, 6.0!)
            Dim ver As Single = db.GetVersion()

            If dbUpd.HasDatabaseUpdates(db, ver) Then

                Dim fmsg As New cFeedbackMessage("To continue, your model database will be updated to a newer version. This means that you will not be able to open the model in older versions of EwE. Do you want to continue?", eCoreComponentType.DataSource,
                                           eMessageType.NotSet, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                fmsg.Reply = eMessageReply.YES
                Me.m_publisher.SendMessage(fmsg)
                If (fmsg.Reply <> eMessageReply.YES) Then Return False
            End If

            ' Run updates
            If Not dbUpd.UpdateDatabase(db) Then
                ' Database update failed
                Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_MODEL_UPDATE_FAILED, Version.ToString),
                                    eMessageType.DataImport,
                                    eCoreComponentType.DataSource,
                                    eMessageImportance.Critical)

                Me.m_publisher.SendMessage(msg)
                Return False
            Else
                ' Database update failed
                Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_MODEL_UPDATE_SUCCESS, Version.ToString),
                                    eMessageType.DataImport,
                                    eCoreComponentType.DataSource,
                                    eMessageImportance.Information)
                Me.m_publisher.SendMessage(msg)
            End If
        End If
        Return True

    End Function

#End Region ' Datasource

#Region " EwEModel "

    ''' <summary>
    ''' Returns the <see cref="cEwEModel">EwE model</see> for the current loaded data source.
    ''' </summary>
    Public ReadOnly Property EwEModel() As cEwEModel
        Get
            Return Me.m_EwEModel
        End Get
    End Property

    Private Function InitEwEModel() As Boolean
        Me.m_EwEModel = New cEwEModel(Me)
        Return LoadEwEModel()
    End Function

    Friend Function LoadEwEModel() As Boolean
        'Pre
        Debug.Assert(Me.m_EwEModel IsNot Nothing)
        Me.m_EwEModel.AllowValidation = False
        Me.m_EwEModel.DBID = Me.m_EcopathData.ModelDBID
        Me.m_EwEModel.Name = Me.m_EcopathData.ModelName
        Me.m_EwEModel.Description = Me.m_EcopathData.ModelDescription
        Me.m_EwEModel.Area = Me.m_EcopathData.ModelArea
        Me.m_EwEModel.Author = Me.m_EcopathData.ModelAuthor
        Me.m_EwEModel.Contact = Me.m_EcopathData.ModelContact
        Me.m_EwEModel.NumDigits = Me.m_EcopathData.ModelNumDigits
        Me.m_EwEModel.GroupDigits = Me.m_EcopathData.ModelGroupDigits
        Me.m_EwEModel.UnitCurrency = DirectCast(Me.m_EcopathData.ModelUnitCurrency, eUnitCurrencyType)
        Me.m_EwEModel.UnitCurrencyCustomText = Me.m_EcopathData.ModelUnitCurrencyCustom
        Me.m_EwEModel.UnitTime = Me.m_EcopathData.ModelUnitTime
        Me.m_EwEModel.UnitTimeCustomText = Me.m_EcopathData.ModelUnitTimeCustom
        Me.m_EwEModel.UnitMonetary = Me.m_EcopathData.ModelUnitMonetary
        Me.m_EwEModel.UnitArea = Me.m_EcopathData.ModelUnitArea
        Me.m_EwEModel.UnitAreaCustomText = Me.m_EcopathData.ModelUnitAreaCustom
        Me.m_EwEModel.FirstYear = Me.m_EcopathData.FirstYear
        Me.m_EwEModel.NumYears = Me.m_EcopathData.NumYears
        Me.m_EwEModel.South = Me.m_EcopathData.ModelSouth
        Me.m_EwEModel.North = Me.m_EcopathData.ModelNorth
        Me.m_EwEModel.West = Me.m_EcopathData.ModelWest
        Me.m_EwEModel.East = Me.m_EcopathData.ModelEast
        Me.m_EwEModel.Country = Me.m_EcopathData.ModelCountry
        Me.m_EwEModel.EcosystemType = Me.m_EcopathData.ModelEcosystemType
        Me.m_EwEModel.PublicationDOI = Me.m_EcopathData.ModelPublicationDOI
        Me.m_EwEModel.PublicationURI = Me.m_EcopathData.ModelPublicationURI
        Me.m_EwEModel.PublicationReference = Me.m_EcopathData.ModelPublicationRef
        Me.m_EwEModel.EcobaseCode = Me.m_EcopathData.ModelEcobaseCode
        Me.m_EwEModel.LastSaved = Me.m_EcopathData.ModelLastSaved
        Me.m_EwEModel.IsEcoSpaceModelCoupled = Me.m_EcopathData.isEcospaceModelCoupled
        Me.m_EwEModel.DiversityIndexType = Me.m_EcopathData.DiversityIndexType

        Me.m_EwEModel.AllowValidation = True

        Me.m_EwEModel.ResetStatusFlags()
        Return True
    End Function

    Friend Function UpdateEwEModel() As Boolean
        Me.m_EcopathData.ModelName = Me.m_EwEModel.Name
        Me.m_EcopathData.ModelDescription = Me.m_EwEModel.Description
        Me.m_EcopathData.ModelAuthor = Me.m_EwEModel.Author
        Me.m_EcopathData.ModelContact = Me.m_EwEModel.Contact
        Me.m_EcopathData.ModelArea = Me.m_EwEModel.Area
        Me.m_EcopathData.ModelNumDigits = Me.m_EwEModel.NumDigits
        Me.m_EcopathData.ModelGroupDigits = Me.m_EwEModel.GroupDigits
        Me.m_EcopathData.ModelUnitCurrency = Me.m_EwEModel.UnitCurrency
        Me.m_EcopathData.ModelUnitCurrencyCustom = Me.m_EwEModel.UnitCurrencyCustomText
        Me.m_EcopathData.ModelUnitTime = Me.m_EwEModel.UnitTime
        Me.m_EcopathData.ModelUnitTimeCustom = Me.m_EwEModel.UnitTimeCustomText
        Me.m_EcopathData.ModelUnitMonetary = Me.m_EwEModel.UnitMonetary
        Me.m_EcopathData.ModelUnitArea = Me.m_EwEModel.UnitArea
        Me.m_EcopathData.ModelUnitAreaCustom = Me.m_EwEModel.UnitAreaCustomText
        Me.m_EcopathData.FirstYear = Me.m_EwEModel.FirstYear
        Me.m_EcopathData.NumYears = Me.m_EwEModel.NumYears
        Me.m_EcopathData.ModelSouth = Me.m_EwEModel.South
        Me.m_EcopathData.ModelNorth = Me.m_EwEModel.North
        Me.m_EcopathData.ModelWest = Me.m_EwEModel.West
        Me.m_EcopathData.ModelEast = Me.m_EwEModel.East
        Me.m_EcopathData.ModelCountry = Me.m_EwEModel.Country
        Me.m_EcopathData.ModelEcosystemType = Me.m_EwEModel.EcosystemType
        Me.m_EcopathData.ModelPublicationDOI = Me.m_EwEModel.PublicationDOI
        Me.m_EcopathData.ModelPublicationURI = Me.m_EwEModel.PublicationURI
        Me.m_EcopathData.ModelPublicationRef = Me.m_EwEModel.PublicationReference
        Me.m_EcopathData.ModelEcobaseCode = Me.m_EwEModel.EcobaseCode
        Me.m_EcopathData.DiversityIndexType = Me.m_EwEModel.DiversityIndexType

        ' Do not update LastSaved; exclusively set by core

        ' Update relevant unit(s) in Ecopath
        'HACK BUG FIX 30-May-2012 Changing the units did not effect the respiration output in the interface
        'The interface and database use Me.m_EcoPathData.currUnitIndex the Ecopath Codes uses Me.m_EcoPathData.currUnitIndex
        'These where not in sync currUnitIndex was never updated
        'This should be fixed by removing currUnitIndex
        Me.m_EcopathData.ModelUnitCurrency = Me.m_EcopathData.ModelUnitCurrency

        Me.m_EcopathData.isEcospaceModelCoupled = Me.m_EwEModel.IsEcoSpaceModelCoupled

        Return True
    End Function

#End Region ' EwEModel

#Region " EcoPath "

#Region " Variables "

    'Private EcoPath Model Variables
    Friend m_Ecopath As Ecopath.cEcopathModel ' the EcoPath model
    Friend m_EcopathData As cEcopathDataStructures = Nothing 'Parameters read for data source for EcoPath

    Friend m_EcoPathInputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoPathGroupInput, 1)
    Friend m_EcopathOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoPathGroupOutput, 1)
    Friend m_EcopathFleetsInput As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.FleetInput, 1)
    Friend m_EcopathFleetsOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.FleetInput, 1)
    Friend m_EcopathTaxon As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.Taxon, 1)

    Private m_postEcoPathMessage As CoreMessageDelegate
    Friend m_PSDData As cPSDDatastructures
    Private m_PSDParameters As cPSDParameters
    Private m_psdModel As cPSDModel
    Friend m_TaxonData As cTaxonDataStructures = Nothing
    Friend m_SampleData As cEcopathSampleDatastructures = Nothing

#End Region ' Variables

#Region "Public Ecopath Variables"

    Public ReadOnly Property EcopathDataStructures As cEcopathDataStructures
        Get
            Return Me.m_EcopathData
        End Get
    End Property

    Public ReadOnly Property EcosimDataStructures As cEcosimDatastructures
        Get
            Return Me.m_EcoSimData
        End Get
    End Property

    Public ReadOnly Property EcospaceDataStructures As cEcospaceDataStructures
        Get
            Return Me.m_EcospaceData
        End Get
    End Property


    ''' <summary>Flag to affect which groups to auto-save data for.</summary>
    ''' <remarks>Values are set by the UI. This logic is currently only used by the Ecospace result writers.</remarks>
    Public Property SelectedGroups As Boolean() = Nothing
    ''' <summary>Flag to affect which fleets to auto-save data for</summary>
    ''' <remarks>Values are set by the UI. This logic is currently only used by the Ecospace result writers.</remarks>
    Public Property SelectedFleets As Boolean() = Nothing

#End Region

#Region " Model "

    Private Sub SendEcopathLoadMessage(ds As IEwEDataSource, Optional strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_LOAD_SUCCESS, ds.ToString())
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecopath, eMessageImportance.Information)
        Else
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_LOAD_FAILED, ds.ToString(), strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecopath, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the Ecopath model from a given file.
    ''' </summary>
    ''' <param name="strFile">The model file to load.</param>
    ''' <returns>True if the model was loaded successfully. False otherwise</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadModel(strFile As String) As Boolean

        m_logger.LogInformation("Loading Ecopath model from file: {0}", strFile)

        Dim fnNow As String = ""
        If (Me.DataSource IsNot Nothing) Then fnNow = Path.GetFullPath(Me.DataSource.ToString())
        Dim fnNew As String = Path.GetFullPath(strFile)
        Dim bNeedClosing As Boolean = (String.Compare(fnNow, fnNew, True) <> 0)
        Dim dsEcopath As IEcopathDataSource = Nothing
        Dim bsuccess As Boolean

        If (bNeedClosing) Then

            If Not Me.CloseModel() Then Return False

            Me.m_Ecopath.RunState = Ecopath.eEcopathRunState.NotRun
            Me.m_EcopathData.ActiveEcosimScenario = -1
            Me.m_EcopathData.ActiveEcospaceScenario = -1
            Me.m_EcopathData.ActiveEcotracerScenario = -1

            If (Me.DataSource Is Nothing) Then
                ' Remember the new data source
                Dim ds As IEwEDataSource = cDataSourceFactory.Create(strFile)
                If (ds Is Nothing) Then Return False

                If ds.Open(strFile, Me) <> eDatasourceAccessType.Opened Then Return False

                ' Sanity checks
                Debug.Assert(ds IsNot Nothing, Me.ToString & "LoadModel() Datasource can not be NULL.")
                Debug.Assert(TypeOf ds Is IEcopathDataSource, "Invalid data source type specified")

                'm_bCoreIsInit was set in InitCore()
                If Not m_bCoreIsInit Then
                    'core has not been initialized this can not be run
                    Debug.Assert(False, "The Core has not been initialized. Call InitCore() first.")
                    ' Flag data as gone
                    Me.m_StateMonitor.SetEcopathLoaded(False)
                    SendEcopathLoadMessage(ds, "Core not initialized")
                    Return False
                End If

                ' Run any available updates on the data source
                If Not Me.UpdateDatasource(ds) Then
                    Return False
                End If

                Me.DataSource = ds
            End If
        End If

        Try

            'Init the parameters from the data source
            dsEcopath = DirectCast(DataSource, IEcopathDataSource)
            If dsEcopath.LoadModel() Then

                If (Me.PluginManager IsNot Nothing) Then
                    Me.PluginManager.OpenDatabase(strFile)
                End If

                'build model
                bsuccess = InitEwEModel()

                'There needs to be a Maintenance message sent SendEcopathLoadMessage() does not really seem like it would work for this
                m_publisher.AddMessage(New cMessage("Loaded model '" & m_EwEModel.Name & "'", eMessageType.DataModified,
                                        eCoreComponentType.Core, eMessageImportance.Maintenance))

                'copy the input data into the output data this could wait for a model run but it may be safer to do it here
                m_EcopathData.CopyInputToModelArrays()
                m_PSDData.Enabled = False ' Fixes bug 683
                m_PSDData.CopyInputToModelArrays()

                'compute the stanza data from the parameters loaded from the model 
                'this has to come before initializing and loading the ecopath groups because 
                'InitStanza can modify the ecopath value: b, pb and qb
                m_Ecosim.InitStanza()

                Me.m_tracerData.RedimByNGroups(Me.nGroups)

                'build input and output objects
                bsuccess = bsuccess And InitEcopathGroups()

                'build the Stanza Groups for the interface
                bsuccess = bsuccess And InitStanzas()
                bsuccess = bsuccess And InitPedigreeManagers()

                'populate input objects
                bsuccess = bsuccess And LoadEcopathInputs()
                'populate output objects
                bsuccess = bsuccess And LoadEcopathOutputs()

                'build the fleets
                bsuccess = bsuccess And InitFleets()

                'buil taxa when groups and fishing are in place
                bsuccess = bsuccess And InitTaxa()

                ' Initialize scenarios
                bsuccess = bsuccess And InitEcosimScenarios()
                bsuccess = bsuccess And InitEcospaceScenarios()
                bsuccess = bsuccess And InitEcotracerScenarios()
                bsuccess = bsuccess And InitAndLoadEcosimTimeSeriesDatasets()

                bsuccess = bsuccess And InitPSDParameters()
                bsuccess = bsuccess And InitEcopathSamples()

                bsuccess = bsuccess And LoadPedigreeManagers()

                Me.m_EcopathStats = New cEcopathStats(Me, cCore.NULL_VALUE)

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
                m_publisher.sendAllMessages()
                Return False
            End If

        Catch ex As Exception
            ' Flag data as gone
            Me.m_StateMonitor.SetEcopathLoaded(False)
            ' Major Error
            Me.SendEcopathLoadMessage(DataSource, ex.Message)
            ' Release data source
            DataSource = Nothing
            ' Report error
            Return False
        End Try

        Me.SendEcopathLoadMessage(DataSource)

        ' Invoke plugin point
        If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.LoadModel(DataSource)

        ' Update core state
        Me.m_StateMonitor.SetEcopathLoaded(True)

        'Core initialized plugin point
        If (Me.PluginManager IsNot Nothing) Then
            Me.PluginManager.CoreInitialized(m_Ecopath, m_Ecosim, m_Ecospace)
            Me.PluginManager.CoreDataInitialized(m_EcopathData, m_Stanza, m_TaxonData, m_SampleData, m_PSDData, m_EcoSimData, m_TSData, m_SearchData, m_EcospaceData)
        End If

        m_publisher.sendAllMessages()

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Checks if the underlying datasource can be saved.
    ''' </summary>
    ''' <param name="bSendMessage"></param>
    ''' <returns>True if the database is editable.</returns>
    ''' -----------------------------------------------------------------------
    Public Function CanSave(Optional bSendMessage As Boolean = False) As Boolean

        If (Me.DataSource Is Nothing) Then Return False
        If (Not Me.DataSource.IsReadOnly) Then Return True

        If bSendMessage Then
            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.MODEL_READONLY, Me.EwEModel.Name),
                                    eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Warning)
            Me.Messages.SendMessage(msg)
        End If

        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the data of all models.
    ''' </summary>
    ''' <param name="strFileName">Optional file name to save to. If not provided
    ''' data is saved to the currently connected <see cref="DataSource"/>. If
    ''' a file name is provided a new datasource will be created, and the core
    ''' will switch to that datasource. The current datasource will then NOT be
    ''' modified.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Save(Optional strFileName As String = "") As Boolean

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcopathDataSource) Then Return False

        Dim bSucces As Boolean = True

        ' Saving to a new file name?
        If (Not String.IsNullOrEmpty(strFileName)) Then
            ' #Yes: First save current database to a new location
            If (DirectCast(DataSource, cDBDataSource).SaveAs(strFileName, Me.m_EwEModel.Name)) = eDatasourceAccessType.Success Then
                ' #Success! The data source has been changed this new location, now save data in memory to the new data source.
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
    ''' <returns>True if successful.</returns>
    ''' <remarks>Note that this logic will NOT sync the two datasources; this
    ''' responsibility is left to the calling process. Yes, this is a hack 
    ''' around a process that needs to be very well thought out!!!</remarks>
    ''' -----------------------------------------------------------------------
    Private Function SaveModel() As Boolean

        Me.m_EcopathData.ModelLastSaved = CInt(Date.Now().ToOADate())

        If (DirectCast(Me.DataSource, IEcopathDataSource).SaveModel()) Then
            ' #Yes: invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveModel(Me)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Oh we're happy now!
            Return True
        Else
            Me.m_publisher.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_SAVE_FAILED, DataSource.ToString), eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Warning))
            m_logger.LogInformation("cCore.SaveModel() Failed to save the current model")
            Return False
        End If

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Close the Ecopath model and terminate the core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function CloseModel() As Boolean

        'Stop any running search
        For Each tp As IThreadedProcess In Me.m_ThreadedProcesses
            'wait 10 seconds
            If Not tp.StopRun(10000) Then
                m_logger.LogInformation("{0} Failed to stop run when loading new model.", tp.ToString)
                'not really a lot of point in sending out an error message
                'this is more of a developement error
                'Debug.Assert(False, manager.ToString & " Failed to stop run when loading new model.")
            End If
        Next

        If Not Me.SaveChanges() Then Return False
#If PROFILE Then
        System.Console.WriteLine("CloseModel() memory before  " & GC.GetTotalMemory(True))
#End If

        Try
            'work from the top down Tracer to Path
            Me.CloseEcotracerScenario()
            Me.CloseEcospaceScenario()
            Me.CloseEcosimScenario()

            ' Has data source?
            If (Me.DataSource IsNot Nothing) Then
                ' #Yes: has open connection?
                If Me.DataSource.Connection IsNot Nothing Then
                    ' '#Yes: close plug-in data sources, close plug-in 
                    If (Me.PluginManager IsNot Nothing) Then
                        Me.PluginManager.CloseDatabase()
                        Me.PluginManager.CloseModel()
                    End If

                    Me.DataSource.Close()
                End If
                ' Release data source
                Me.DataSource = Nothing
            End If

        Catch ex As Exception
            'Debug.Assert(False, Me.ToString & ".CloseModel() Exception closing data source: " & ex.Message)
            m_logger.LogError(ex, "CloseModel() Exception closing data source: {1}", ex.Message)
        End Try

        Try
            ' Forget model
            Me.m_StateMonitor.SetEcopathLoaded(False)
            Me.m_StateMonitor.UpdateDataState(Nothing)

            ' Clear (not destroy) managers 
            For Each pdMng As cPedigreeManager In Me.m_PedigreeManagers.Values
                pdMng.Clear()
            Next
            Me.m_PedigreeManagers.Clear()
            Me.m_MediatedInteractionManager.Clear()

            ' Clear core data structures
            Me.m_EcopathData.Clear()
            Me.m_SampleManager.Clear()
            Me.m_Stanza.Clear()
            Me.m_TaxonData.Clear()
            Me.m_EcoSimData.Clear()
            Me.m_EcospaceData.Clear()
            Me.m_tracerData.Clear()
            Me.m_TSData.Clear()

            ' Destroy IO objects not in a list - may not have been created yet
            If (Me.m_EwEModel IsNot Nothing) Then Me.m_EwEModel.Clear() : Me.m_EwEModel = Nothing
            If (Me.m_EcopathStats IsNot Nothing) Then Me.m_EcopathStats.Clear() : Me.m_EcopathStats = Nothing
            If (Me.m_PSDParameters IsNot Nothing) Then Me.m_PSDParameters.Clear() : Me.m_PSDParameters = Nothing

            ' Clear the contents of core IO object lists
            Me.ClearIOList(Me.m_EcoPathInputs)
            Me.ClearIOList(Me.m_EcopathOutputs)
            Me.ClearIOList(Me.m_EcopathFleetsInput)
            Me.ClearIOList(m_EcopathFleetsOutputs)
            Me.ClearIOList(Me.m_stanzaGroups)
            Me.ClearIOList(Me.m_EcopathTaxon)
            Me.ClearIOList(Me.m_EcotracerGroupInputs)
            ' m_EcotracerGroupOutput is only allocated if Tracer has run
            If Me.m_EcotracerGroupOutput IsNot Nothing Then Me.m_EcotracerGroupOutput.Dispose() : Me.m_EcotracerGroupOutput = Nothing

            Me.ClearIOList(Me.m_timeSeriesDatasets)

            ' Clear scenarios
            Me.m_EcopathData.NumEcotracerScenarios = 0
            Me.m_EcopathData.NumEcospaceScenarios = 0
            Me.m_EcopathData.NumEcosimScenarios = 0

            Me.ClearIOList(Me.m_EcoSimScenarios)
            Me.ClearIOList(Me.m_EcospaceScenarios)
            Me.ClearIOList(Me.m_EcotracerScenarios)

            Me.SelectedGroups = Nothing
            Me.SelectedFleets = Nothing

            'ToDo: add SendEcopathClosedMessage()
            Me.m_publisher.SendMessage(New cMessage("Closed model", eMessageType.DataAddedOrRemoved,
                                       eCoreComponentType.Ecopath, eMessageImportance.Maintenance), True)

            Me.m_dtAuxiliaryData.Clear()

            Me.ClosePSD()

            Me.m_Ecopath.Clear()

            Me.m_EcospaceTimeSeriesManager.Clear()

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Can't do this the Datasource needs to be Initialized again 
            'which only happens when the plugin is loaded.
            'To do this we need to add an Initialize loop to the plugin manager, which could case problems with some plugins,
            'or Init the datasources by hand on model load.
            'Dim EcoDS As cEconomicDataSource = cEconomicDataSource.getInstance
            'If EcoDS IsNot Nothing Then
            '    EcoDS.Clear()
            'End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".CloseModel() Exception clearing memory: " & ex.Message)
            m_logger.LogError(ex, "CloseModel() Exception clearing memory: {1}", ex.Message)
        End Try

        Try
            Me.m_publisher.sendAllMessages()
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".CloseModel() Exception sending messages: " & ex.Message)
        End Try

        GC.Collect()
#If PROFILE Then
        System.Console.WriteLine("CloseModel() memory after  " & GC.GetTotalMemory(True))
#End If

        m_logger.LogInformation("Model closed")

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear the contents of a cCoreInputOutputList, disposing all of the items in the list.
    ''' </summary>
    ''' <param name="IOList">The list to clear, may be NULL.</param>
    ''' -----------------------------------------------------------------------
    Private Sub ClearIOList(IOList As cCoreInputOutputList(Of cCoreInputOutputBase))
        If (IOList Is Nothing) Then Return
        Try
            For Each ioOb As cCoreInputOutputBase In IOList
                ioOb.Dispose()
            Next
            IOList.Clear()
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

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
    Public ReadOnly Property EcopathGroupInputs(iGroup As Integer) As cEcoPathGroupInput
        Get
            ' JS 06Jul07: list takes care of group index / item index offset
            Return DirectCast(m_EcoPathInputs(iGroup), cEcoPathGroupInput)
        End Get

    End Property

    Private Function InitEcopathGroups() As Boolean
        Dim bsuccess As Boolean = True

        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.m_EcoPathInputs.AllowEvents = False
        'Me.m_EcoPathOutputs.AllowEvents = False

        Try

            m_EcoPathInputs.Clear()
            m_EcopathOutputs.Clear()

            'populate the list of Inputs and Outputs (cEcoPathGroupInputs and cEcoPathGroupOutput)
            For i As Integer = 1 To nGroups
                'creates an instance of both the input and output objects and adds it to the list
                'the Input and Output objects have only been created they are not Loaded with the Ecopath data at this time
                m_EcoPathInputs.Add(New cEcoPathGroupInput(Me, m_EcopathData.GroupDBID(i), i))
                m_EcopathOutputs.Add(New cEcopathGroupOutput(Me, m_EcopathData.GroupDBID(i)))
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

    Private Function LoadEcopathInput(group As cEcoPathGroupInput) As Boolean
        Dim iGroup As Integer
        Try

            'do not run the data validation when the object is populated
            group.AllowValidation = False

            'convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcopathData.GroupDBID, group.DBID)

            If iGroup >= 0 And iGroup <= m_EcopathData.NumGroups Then

                group.Resize()

                'get the public variables
                'jb June-7-2006 DatabaseID is now set in the constructor so that an object always knows it DatabaseID
                'Input.DBID = m_EcoPathData.GroupDBID(iGroup)
                group.Name = m_EcopathData.GroupName(iGroup)

                'input variables
                group.EEInput = CSng(m_EcopathData.EEinput(iGroup))
                group.OtherMortInput = CSng(m_EcopathData.OtherMortinput(iGroup))
                group.QBInput = CSng(m_EcopathData.QBinput(iGroup))
                group.PBInput = CSng(m_EcopathData.PBinput(iGroup))
                group.GEInput = CSng(m_EcopathData.GEinput(iGroup))
                group.BiomassAreaInput = CSng(m_EcopathData.BHinput(iGroup))

                group.Area = m_EcopathData.Area(iGroup)
                group.GS = m_EcopathData.GS(iGroup)
                group.DetImport = m_EcopathData.DtImp(iGroup)
                group.EmigRate = m_EcopathData.Emig(iGroup)
                group.BioAccumRate = CSng(m_EcopathData.BaBi(iGroup))
                group.Immigration = m_EcopathData.Immig(iGroup)
                group.PP = m_EcopathData.PP(iGroup)
                group.VBK = m_EcopathData.vbK(iGroup)
                group.PoolColor = m_EcopathData.GroupColor(iGroup)
                group.NonMarketValue = m_EcopathData.Shadow(iGroup)
                group.Energy = m_EcopathData.Energy(iGroup)

                For i As Integer = 1 To nGroups
                    group.IsPrey(i) = False
                    If m_EcopathData.DC(iGroup, i) > 0 Then group.IsPrey(i) = True
                    group.IsPred(i) = False
                    If m_EcopathData.DC(i, iGroup) > 0 Then group.IsPred(i) = True
                Next

                group.BioAccumInput = If(m_EcopathData.BaBi(iGroup) <> 0 And m_EcopathData.B(iGroup) > 0, m_EcopathData.BaBi(iGroup) * m_EcopathData.B(iGroup), m_EcopathData.BAInput(iGroup))

                'if  Emigration = 0 then compute Emigration as EmigRate * biomass for this group
                'from original code
                group.Emigration = If(m_EcopathData.Emig(iGroup) > 0 And m_EcopathData.B(iGroup) > 0 And m_EcopathData.Emigration(iGroup) = 0,
                                                m_EcopathData.Emig(iGroup) * m_EcopathData.B(iGroup), m_EcopathData.Emigration(iGroup))
                Dim j As Integer
                'Diet Comp (DO NOT INCLUDE IMPORT IN THE DC ARRAY - THIS IS SEPARATED IN ECOPATHGROUP!)
                For j = 1 To m_EcopathData.NumGroups
                    group.DietComp(j) = m_EcopathData.DC(iGroup, j)
                Next
                group.ImpDiet = m_EcopathData.DC(iGroup, 0)

                'detritus fate
                For j = 1 To nDetritusGroups
                    group.DetritusFate(j) = m_EcopathData.DF(iGroup, j)
                Next

                'stanza variables setting the stanza id will also set the isMultiStanza Flag
                group.iStanza = getStanzaIndexForGroup(iGroup)

                'PSD
                group.AinLWInput = m_PSDData.AinLWInput(iGroup)
                group.BinLWInput = m_PSDData.BinLWInput(iGroup)
                group.LooInput = m_PSDData.LooInput(iGroup)
                group.WinfInput = m_PSDData.WinfInput(iGroup)
                group.t0Input = m_PSDData.t0Input(iGroup)
                group.TcatchInput = m_PSDData.TcatchInput(iGroup)
                group.TmaxInput = m_PSDData.TmaxInput(iGroup)

                'Taxa
                For i As Integer = 1 To Me.m_TaxonData.NumGroupTaxa(iGroup)
                    group.iTaxon(i) = Me.m_TaxonData.GroupTaxa(iGroup, i)
                Next

                'set all the status flags to default value
                group.ResetStatusFlags()

                group.AllowValidation = True
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
    Private Function UpdateEcopathInput(iDBID As Integer) As Boolean

        Dim iGroup As Integer = Array.IndexOf(m_EcopathData.GroupDBID, iDBID)
        'jb List of inputs is indexed from zero iGroup is the array index which is indexed from one 
        'so subtract one from the array index to get the correct index in the list
        Dim Input As cEcoPathGroupInput = Me.EcopathGroupInputs(iGroup)
        Dim bSucces As Boolean = True

        Try

            If iGroup >= 1 And iGroup <= m_EcopathData.NumGroups Then

                m_EcopathData.GroupName(iGroup) = Input.Name
                m_EcopathData.Area(iGroup) = Input.Area
                m_EcopathData.DtImp(iGroup) = Input.DetImport
                'jb 17/mar/06 removed biomass from input
                'mEcoPathData.B(iGroup) = input.Biomass
                m_EcopathData.BaBi(iGroup) = Input.BioAccumRate
                m_EcopathData.Immig(iGroup) = Input.Immigration
                m_EcopathData.BAInput(iGroup) = Input.BioAccumInput
                m_EcopathData.Emig(iGroup) = Input.EmigRate
                m_EcopathData.PP(iGroup) = Input.PP

                m_EcopathData.vbK(iGroup) = Input.VBK
                m_PSDData.AinLWInput(iGroup) = Input.AinLWInput
                m_PSDData.BinLWInput(iGroup) = Input.BinLWInput
                m_PSDData.LooInput(iGroup) = Input.LooInput
                m_PSDData.WinfInput(iGroup) = Input.WinfInput
                m_PSDData.t0Input(iGroup) = Input.t0Input
                m_PSDData.TcatchInput(iGroup) = Input.TcatchInput
                m_PSDData.TmaxInput(iGroup) = Input.TmaxInput

                m_EcopathData.QBinput(iGroup) = Input.QBInput
                m_EcopathData.PBinput(iGroup) = Input.PBInput
                m_EcopathData.EEinput(iGroup) = Input.EEInput
                m_EcopathData.OtherMortinput(iGroup) = Input.OtherMortInput
                m_EcopathData.GEinput(iGroup) = Input.GEInput
                m_EcopathData.BHinput(iGroup) = Input.BiomassAreaInput

                m_EcopathData.GroupColor(iGroup) = Input.PoolColor
                m_EcopathData.Shadow(iGroup) = Input.NonMarketValue()
                m_EcopathData.Energy(iGroup) = Input.Energy

                'from the original code MakeUnknownUnknown
                m_EcopathData.BAInput(iGroup) = If(m_EcopathData.BaBi(iGroup) <> 0 And m_EcopathData.B(iGroup) > 0,
                                                m_EcopathData.BaBi(iGroup) * m_EcopathData.B(iGroup), m_EcopathData.BAInput(iGroup))

                'Emigi(igroup) = inputVars.EmigRate
                'if  Emigration = 0 then compute Emigration as EmigRate * biomass for this group
                'from original code
                m_EcopathData.Emigration(iGroup) = If(m_EcopathData.Emig(iGroup) > 0 And m_EcopathData.B(iGroup) > 0 And m_EcopathData.Emigration(iGroup) = 0,
                                                         m_EcopathData.Emig(iGroup) * m_EcopathData.B(iGroup), Input.Emigration)

                'GS Unassimilated Consumption changes with Model Currency Units
                m_EcopathData.GS(iGroup) = Input.GS
                If Not Me.m_EcopathData.areUnitCurrencyNutrients Then
                    'Model Currency Units are Energy (NOT Nutrient)
                    'keep a copy of the GS edits incase the user switches Currency types
                    'GS will change 
                    m_EcopathData.GSEng(iGroup) = m_EcopathData.GS(iGroup)
                End If

                For i As Integer = 1 To m_EcopathData.NumGroups
                    'Diet Comp is stored by Pred/Prey
                    'so this is the Prey for Predator iGroup
                    m_EcopathData.DC(iGroup, i) = Input.DietComp(i)
                Next i
                m_EcopathData.DC(iGroup, 0) = Input.ImpDiet()

                For i As Integer = 1 To nDetritusGroups
                    m_EcopathData.DF(iGroup, i) = Input.DetritusFate(i)
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
    Public ReadOnly Property EcopathGroupOutputs(iGroup As Integer) As cEcopathGroupOutput

        Get
            ' The list takes care of group index / item index differences
            Return DirectCast(m_EcopathOutputs(iGroup), cEcopathGroupOutput)
        End Get

    End Property

    ''' <summary>
    ''' Clear all status flags on Ecopath group outputs
    ''' </summary>
    Private Sub ResetEcopathGroupOutputs()
        For Each group As cEcopathGroupOutput In Me.m_EcopathOutputs
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

            For Each output As cEcopathGroupOutput In m_EcopathOutputs
                'convert the DBID into an iGroup
                iGroup = Array.IndexOf(m_EcopathData.GroupDBID, output.DBID)

                'set the size of any array data
                output.Resize()

                'iGroup out of bounds
                If (iGroup > nGroups Or iGroup < 0) And iGroup <> NULL_VALUE Then
                    m_logger.LogInformation("PopulateEcoPathOutput() iGroup out of bounds.")
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

                For iPred As Integer = 1 To m_EcopathData.NumLiving
                    If m_EcopathData.B(iGroup) > 0 Then
                        'predation mortality is not held by EcoPath; it is computed every time it's needed
                        predmort(iPred) = CSng(m_EcopathData.B(iPred) * m_EcopathData.QB(iPred) * m_EcopathData.DC(iPred, iGroup) / m_EcopathData.B(iGroup))
                        'search rate is not held by EcoPath; it is computed every time it's needed
                        searchrate(iPred) = CSng(m_EcopathData.B(iPred) * m_EcopathData.QB(iPred) * m_EcopathData.DC(iPred, iGroup) / (m_EcopathData.B(iGroup) * m_EcopathData.B(iPred)))
                    End If
                Next
                output.PredMort = predmort
                output.SearchRate = searchrate

                output.Index = iGroup
                output.DBID = m_EcopathData.GroupDBID(iGroup)
                output.Name = m_EcopathData.GroupName(iGroup)
                output.Area = m_EcopathData.Area(iGroup)
                output.Biomass = CSng(m_EcopathData.B(iGroup))
                output.BiomassArea = CSng(m_EcopathData.BH(iGroup))
                output.BioAccum = CSng(m_EcopathData.BA(iGroup))
                Try
                    output.BioAccumRatePerYear = CSng(m_EcopathData.BA(iGroup) / m_EcopathData.B(iGroup))
                Catch ex As Exception
                    output.BioAccumRatePerYear = 0.0!
                End Try
                output.GS = m_EcopathData.GS(iGroup)
                output.TTLX = m_EcopathData.TTLX(iGroup)

                output.PP = m_EcopathData.PP(iGroup)

                'output variables

                If output.IsDetritus Then
                    output.PBOutput = cCore.NULL_VALUE
                    output.QBOutput = cCore.NULL_VALUE
                Else
                    output.PBOutput = CSng(m_EcopathData.PB(iGroup))
                    output.QBOutput = CSng(m_EcopathData.QB(iGroup))
                End If

                output.PBOutput = CSng(m_EcopathData.PB(iGroup))
                output.QBOutput = CSng(m_EcopathData.QB(iGroup))
                output.EEOutput = CSng(m_EcopathData.EE(iGroup))
                output.GEOutput = CSng(m_EcopathData.GE(iGroup))


                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'mortality coefficients are computed when they are needed
                'see Ewe-5 code frmPasicParams.DisplayMortalityCoefficients() for original code
                output.MortCoBioAcumRate = CSng(m_EcopathData.BA(iGroup) / m_EcopathData.B(iGroup))
                output.MortCoFishRate = CSng(m_EcopathData.fCatch(iGroup) / m_EcopathData.B(iGroup))
                output.MortCoNetMig = CSng((m_EcopathData.Emigration(iGroup) - m_EcopathData.Immig(iGroup)) / m_EcopathData.B(iGroup))
                output.MortCoOtherMort = CSng((1 - m_EcopathData.EE(iGroup)) * m_EcopathData.PB(iGroup))
                output.MortCoPB = CSng(m_EcopathData.PB(iGroup))
                output.MortCoPredMort = m_EcopathData.M2(iGroup)
                'jb 28-Sept-2010 changed FishMortPerTotMort 
                'Dim m0 As Single = CSng((1 - m_EcoPathData.EE(iGroup)))
                'output.FishMortPerTotMort = output.MortCoFishRate / (m0 + m_EcoPathData.M2(iGroup) + output.MortCoFishRate) 'F/Z
                output.FishMortPerTotMort = output.MortCoFishRate / (m_EcopathData.PB(iGroup) - m_EcopathData.BA(iGroup) - output.MortCoNetMig)
                output.NatMortPerTotMort = CSng(1.0 - output.FishMortPerTotMort) 'M/Z

                'For iflt As Integer = 1 To nFleets
                '    output.CatchByFleet(iflt) = Me.m_EcoPathData.Landing(iflt, iGroup) + Me.m_EcoPathData.Discard(iflt, iGroup)
                '    output.LandingsByFleet(iflt) = Me.m_EcoPathData.Landing(iflt, iGroup)
                '    output.DiscardMortByFleet(iflt) = Me.m_EcoPathData.Discard(iflt, iGroup) * Me.m_EcoPathData.PropDiscardMort(iflt, iGroup)
                '    output.DiscardSurvivalByFleet(iflt) = Me.m_EcoPathData.Discard(iflt, iGroup) * (1 - Me.m_EcoPathData.PropDiscardMort(iflt, iGroup))
                'Next

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'consumption
                'see frmPasicParams.DisplayFoodIntake
                For i As Integer = 1 To m_EcopathData.NumGroups
                    If i <= m_EcopathData.NumLiving Then
                        convalue = CSng(m_EcopathData.B(i) * m_EcopathData.QB(i) * m_EcopathData.DC(i, iGroup))
                    Else
                        convalue = CSng(m_EcopathData.det(iGroup, i))
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
                impConsump = CSng(m_EcopathData.B(iGroup) * m_EcopathData.QB(iGroup) * m_EcopathData.DC(iGroup, 0))
                If impConsump > 0 Then
                    output.ImportedConsumption = impConsump
                Else
                    output.ImportedConsumption = 0
                End If

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'key indices
                output.NetMigration = CSng(m_EcopathData.Emigration(iGroup) - m_EcopathData.Immig(iGroup))
                output.FlowToDet = m_EcopathData.FlowToDet(iGroup)
                If (iGroup <= Me.m_EcopathData.NumLiving) Then
                    If (m_EcopathData.QB(iGroup) * (1 - m_EcopathData.GS(iGroup)) > 0) Then
                        output.NetEfficiency = m_EcopathData.PB(iGroup) / (m_EcopathData.QB(iGroup) * (1 - m_EcopathData.GS(iGroup)))
                    Else
                        output.NetEfficiency = cCore.NULL_VALUE
                    End If
                End If
                output.OmnivoryIndex = m_EcopathData.BQB(iGroup)

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'respiration
                output.Respiration = m_EcopathData.Resp(iGroup)
                output.Assimilation = cCore.NULL_VALUE
                output.RespAssim = cCore.NULL_VALUE
                output.ProdResp = cCore.NULL_VALUE
                output.RespBiom = cCore.NULL_VALUE
                If (iGroup <= Me.m_EcopathData.NumLiving) Then
                    If m_EcopathData.QB(iGroup) > 0 Then
                        Dim sAssim As Single = m_EcopathData.QB(iGroup) * m_EcopathData.B(iGroup) * (1 - m_EcopathData.GS(iGroup))
                        output.Assimilation = sAssim
                        output.RespAssim = CSng(m_EcopathData.Resp(iGroup) / sAssim)
                    End If

                    If (m_EcopathData.Resp(iGroup) > 0 And m_EcopathData.B(iGroup) > 0) Then
                        output.ProdResp = m_EcopathData.PB(iGroup) * m_EcopathData.B(iGroup) / m_EcopathData.Resp(iGroup)
                        output.RespBiom = m_EcopathData.Resp(iGroup) / m_EcopathData.B(iGroup)
                    End If
                End If

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Niche
                For i As Integer = 1 To m_EcopathData.NumLiving
                    Hlap(i) = m_EcopathData.Hlap(i, iGroup)
                    Plap(i) = m_EcopathData.Plap(i, iGroup)
                Next
                output.Hlap = Hlap
                output.Plap = Plap

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
                ' Electivity
                For i As Integer = 1 To m_EcopathData.NumGroups
                    Alpha(i) = m_EcopathData.Alpha(iGroup, i)
                Next
                output.Alpha = Alpha

                output.iStanza = getStanzaIndexForGroup(iGroup)

                ' === PSD ===

                ReDim EcopathWeight(nAgeSteps)
                ReDim EcopathNumber(nAgeSteps)
                ReDim EcopathBiomass(nAgeSteps)
                ReDim LorenzenMortality(nAgeSteps)
                ReDim PSD(nWeightClasses)

                output.VBK = CSng(m_EcopathData.vbK(iGroup))
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

            m_logger.LogError(ex, "LoadEcopathOutputs() Error: {1}", ex.Message)
            Return False

        End Try

    End Function

#End Region ' Groups

#Region " Taxon "

    Private Function InitTaxa() As Boolean

        Dim bSucces As Boolean = True
        Try

            Me.m_EcopathTaxon.Clear()
            For iTaxon As Integer = 1 To Me.m_TaxonData.NumTaxon
                Me.m_EcopathTaxon.Add(New cTaxon(Me, Me.m_TaxonData.TaxonDBID(iTaxon)))
            Next iTaxon
            Me.LoadEcopathTaxon()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitTaxa() Error: " & ex.Message)
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadEcopathTaxon() As Boolean

        Dim iTaxon As Integer = 0

        Try

            For Each taxon As cTaxon In Me.m_EcopathTaxon

                taxon.AllowValidation = False

                iTaxon = Array.IndexOf(Me.m_TaxonData.TaxonDBID, taxon.DBID)

                Debug.Assert(iTaxon > 0 And iTaxon <= Me.m_TaxonData.NumTaxon, "Failed to find Taxon index for database ID " & taxon.DBID)

                taxon.Resize()

                taxon.Index = iTaxon
                taxon.DBID = Me.m_TaxonData.TaxonDBID(iTaxon)
                If Me.m_TaxonData.IsTaxonStanza(iTaxon) Then
                    taxon.iStanza = Me.m_TaxonData.TaxonTarget(iTaxon)
                    taxon.iGroup = NULL_VALUE
                Else
                    taxon.iStanza = NULL_VALUE
                    taxon.iGroup = Me.m_TaxonData.TaxonTarget(iTaxon)
                End If
                taxon.PropB = Me.m_TaxonData.TaxonPropBiomass(iTaxon)
                taxon.Name = Me.m_TaxonData.TaxonName(iTaxon)
                taxon.Class = Me.m_TaxonData.TaxonClass(iTaxon)
                taxon.Order = Me.m_TaxonData.TaxonOrder(iTaxon)
                taxon.Family = Me.m_TaxonData.TaxonFamily(iTaxon)
                taxon.Genus = Me.m_TaxonData.TaxonGenus(iTaxon)
                taxon.Species = Me.m_TaxonData.TaxonSpecies(iTaxon)
                taxon.CodeSAUP = Me.m_TaxonData.TaxonCodeSAUP(iTaxon)
                taxon.CodeFishBase = Me.m_TaxonData.TaxonCodeFB(iTaxon)
                taxon.CodeSeaLifeBase = Me.m_TaxonData.TaxonCodeSLB(iTaxon)
                taxon.CodeFAO = Me.m_TaxonData.TaxonCodeFAO(iTaxon)
                taxon.CodeLSID = Me.m_TaxonData.TaxonCodeLSID(iTaxon)
                taxon.CodeAquaMaps = Me.m_TaxonData.TaxonCodeAquaMaps(iTaxon)
                taxon.CodeAphia = Me.m_TaxonData.TaxonCodeAphia(iTaxon)
                taxon.CodeOBIS = Me.m_TaxonData.TaxonCodeOBIS(iTaxon)
                taxon.Source = Me.m_TaxonData.TaxonSource(iTaxon)
                taxon.SourceKey = Me.m_TaxonData.TaxonSourceKey(iTaxon)
                taxon.North = Me.m_TaxonData.TaxonNorth(iTaxon)
                taxon.South = Me.m_TaxonData.TaxonSouth(iTaxon)
                taxon.East = Me.m_TaxonData.TaxonEast(iTaxon)
                taxon.West = Me.m_TaxonData.TaxonWest(iTaxon)
                taxon.EcologyType = Me.m_TaxonData.TaxonEcologyType(iTaxon)
                taxon.PropC = Me.m_TaxonData.TaxonPropCatch(iTaxon)
                taxon.IUCNConservationStatus = Me.m_TaxonData.TaxonIUCNConservationStatus(iTaxon)
                taxon.ExploitationStatus = Me.m_TaxonData.TaxonExploitationStatus(iTaxon)
                taxon.OrganismType = Me.m_TaxonData.TaxonOrganismType(iTaxon)
                taxon.OccurrenceStatus = Me.m_TaxonData.TaxonOccurrenceStatus(iTaxon)
                taxon.MeanLength = Me.m_TaxonData.TaxonMeanLength(iTaxon)
                taxon.MaxLength = Me.m_TaxonData.TaxonMaxLength(iTaxon)
                taxon.MeanWeight = Me.m_TaxonData.TaxonMeanWeight(iTaxon)
                taxon.MeanLifespan = Me.m_TaxonData.TaxonMeanLifeSpan(iTaxon)
                taxon.VulnerabilityIndex = Me.m_TaxonData.TaxonVulnerabilityIndex(iTaxon)
                taxon.Winf = Me.m_TaxonData.TaxonWinf(iTaxon)
                taxon.vbgfK = Me.m_TaxonData.TaxonK(iTaxon)

                taxon.LastUpdated = Me.m_TaxonData.TaxonLastUpdated(iTaxon)

                taxon.ResetStatusFlags()
                taxon.AllowValidation = True
            Next

            Return True

        Catch ex As Exception

            m_logger.LogError(ex, "LoadTaxon() Error: {1}", ex.Message)
            Debug.Assert(False, Me.ToString & ".LoadTaxon() Error: " & ex.Message)
            Return False

        End Try

    End Function

    Private Function UpdateEcopathGroupTaxon(iDBID As Integer) As Boolean

        Dim iTaxon As Integer = Array.IndexOf(Me.m_TaxonData.TaxonDBID, iDBID)
        Debug.Assert(iTaxon > 0 And iTaxon <= m_TaxonData.NumTaxon, "Failed to find Taxon index for database ID " & iDBID)

        Dim taxon As cTaxon = Me.Taxon(iTaxon)

        If taxon.iGroup > 0 Then
            Me.m_TaxonData.TaxonTarget(iTaxon) = taxon.iGroup
            Me.m_TaxonData.IsTaxonStanza(iTaxon) = False
        Else
            Me.m_TaxonData.TaxonTarget(iTaxon) = taxon.iStanza
            Me.m_TaxonData.IsTaxonStanza(iTaxon) = True
        End If
        Me.m_TaxonData.TaxonPropBiomass(iTaxon) = taxon.PropB
        Me.m_TaxonData.TaxonPropCatch(iTaxon) = taxon.PropC
        Me.m_TaxonData.TaxonName(iTaxon) = taxon.Name
        Me.m_TaxonData.TaxonClass(iTaxon) = taxon.Class
        Me.m_TaxonData.TaxonOrder(iTaxon) = taxon.Order
        Me.m_TaxonData.TaxonFamily(iTaxon) = taxon.Family
        Me.m_TaxonData.TaxonGenus(iTaxon) = taxon.Genus
        Me.m_TaxonData.TaxonSpecies(iTaxon) = taxon.Species
        Me.m_TaxonData.TaxonCodeSAUP(iTaxon) = taxon.CodeSAUP
        Me.m_TaxonData.TaxonCodeFB(iTaxon) = taxon.CodeFishBase
        Me.m_TaxonData.TaxonCodeSLB(iTaxon) = taxon.CodeSeaLifeBase
        Me.m_TaxonData.TaxonCodeFAO(iTaxon) = taxon.CodeFAO
        Me.m_TaxonData.TaxonCodeLSID(iTaxon) = taxon.CodeLSID
        Me.m_TaxonData.TaxonCodeAquaMaps(iTaxon) = taxon.CodeAquaMaps
        Me.m_TaxonData.TaxonCodeAphia(iTaxon) = taxon.CodeAphia
        Me.m_TaxonData.TaxonCodeOBIS(iTaxon) = taxon.CodeOBIS
        Me.m_TaxonData.TaxonSource(iTaxon) = taxon.Source
        Me.m_TaxonData.TaxonSourceKey(iTaxon) = taxon.SourceKey
        Me.m_TaxonData.TaxonEcologyType(iTaxon) = taxon.EcologyType
        Me.m_TaxonData.TaxonIUCNConservationStatus(iTaxon) = taxon.IUCNConservationStatus
        Me.m_TaxonData.TaxonExploitationStatus(iTaxon) = taxon.ExploitationStatus
        Me.m_TaxonData.TaxonOrganismType(iTaxon) = taxon.OrganismType
        Me.m_TaxonData.TaxonOccurrenceStatus(iTaxon) = taxon.OccurrenceStatus
        Me.m_TaxonData.TaxonMeanLength(iTaxon) = taxon.MeanLength
        Me.m_TaxonData.TaxonMaxLength(iTaxon) = taxon.MaxLength
        Me.m_TaxonData.TaxonMeanWeight(iTaxon) = taxon.MeanWeight
        Me.m_TaxonData.TaxonMeanLifeSpan(iTaxon) = taxon.MeanLifespan
        Me.m_TaxonData.TaxonLastUpdated(iTaxon) = taxon.LastUpdated
        Me.m_TaxonData.TaxonVulnerabilityIndex(iTaxon) = taxon.VulnerabilityIndex
        Me.m_TaxonData.TaxonWinf(iTaxon) = taxon.Winf
        Me.m_TaxonData.TaxonK(iTaxon) = taxon.vbgfK

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cTaxon">taxon</see> for a given index.
    ''' </summary>
    ''' <param name="iTaxon">The one-based index to obtain the Taxon definition for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Taxon(iTaxon As Integer) As cTaxon
        Get
            ' List will handle index / item index offsets
            Return DirectCast(Me.m_EcopathTaxon(iTaxon), cTaxon)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a taxonomy definition to an Ecopath group.
    ''' </summary>
    ''' <param name="iTarget">Target index to add the taxonomy definition to.</param>
    ''' <param name="bIsStanza">Flag stating whether the index represents a 
    ''' stanza (true) or a group (false).</param>
    ''' <param name="data">Taxonomy data to add.</param>
    ''' <param name="sPropBiomass">Proportion that this taxonomy contributes to the biomass of the group.</param>
    ''' <param name="sPropCatch">Proportion that this taxonomy contributes to the catch of the group.</param>
    ''' <param name="iDBID">Database ID for the new taxonomy definition.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddTaxon(iTarget As Integer,
                             bIsStanza As Boolean,
                             data As ITaxonSearchData,
                             sPropBiomass As Single,
                             sPropCatch As Single,
                             ByRef iDBID As Integer) As Boolean

        If (data Is Nothing) Then Return False

        Dim bSucces As Boolean = False
        Dim iTargetDBID As Integer = 0

        If bIsStanza Then
            iTargetDBID = (Me.m_Stanza.StanzaDBID(iTarget))
        Else
            iTargetDBID = (Me.m_EcopathData.GroupDBID(iTarget))
        End If

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ' Start the actual work
        If (DirectCast(DataSource, IEcopathDataSource).AddTaxon(iTargetDBID, bIsStanza, data, sPropBiomass, sPropCatch, iDBID)) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of taxa has changed.", eCoreComponentType.Ecopath, eDataTypes.Taxon)
            bSucces = True
            m_logger.LogInformation("Taxon {0} {1} added", data.Genus, data.Species)
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
    Public Function RemoveTaxon(iTaxon As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.RemoveTaxon(Me.m_TaxonData.TaxonDBID(iTaxon)) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of taxa has changed.", eCoreComponentType.Ecopath, eDataTypes.Taxon)
            bSucces = True
            m_logger.LogInformation("Taxon {0} deleted", iTaxon)
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    Public Function TaxonAnalysis() As cTaxonAnalysis
        If Not Me.m_StateMonitor.HasEcopathLoaded Then Return Nothing
        Return New cTaxonAnalysis(Me.m_TaxonData)
    End Function

    Public Function NormalizeTaxonProportions() As Boolean

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Update) Then Return False

        Me.m_TaxonData.NormalizeProportions()
        Me.LoadEcopathTaxon()
        Me.DataModifiedMessage("Ecopath taxa normalized.", eCoreComponentType.Ecopath, eDataTypes.Taxon)

        Return Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

    End Function

#End Region ' Taxon

#Region " Fleets "

    Private Function InitFleets() As Boolean
        Try
            Dim iFleet As Integer

            'clear out the old data
            m_EcopathFleetsInput.Clear()
            m_EcopathFleetsOutputs.Clear()

            'loop over the number of fleets 
            'adding a new fleet to the Fleets collection for each iFleet
            For iFleet = 1 To m_EcopathData.NumFleet
                m_EcopathFleetsInput.Add(New cEcopathFleetInput(Me, m_EcopathData.FleetDBID(iFleet), iFleet))
                m_EcopathFleetsOutputs.Add(New cEcopathFleetOutput(Me, m_EcopathData.FleetDBID(iFleet), iFleet))
            Next iFleet

            LoadEcopathFleetInputs()
            LoadEcopathFleetOutputs()
            Me.Update_IsFished(False)

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitFleets() Error: " & ex.Message)
            Return False
        End Try
    End Function

    Private Function UpdateFleetInput(iDBID As Integer) As Boolean

        Dim iFleet As Integer = Array.IndexOf(m_EcopathData.FleetDBID, iDBID)
        Dim fleet As cEcopathFleetInput = Me.EcopathFleetInputs(iFleet)

        Try

            Debug.Assert(iFleet > 0 And iFleet <= m_EcopathData.NumFleet, "Failed to find Fleet index for database ID " & fleet.DBID)

            Me.m_EcopathData.FleetName(iFleet) = fleet.Name
            Me.m_EcopathData.NominalEffort(iFleet) = fleet.NominalEffort
            Me.m_EcopathData.CostPct(iFleet, eCostIndex.Fixed) = fleet.FixedCost
            Me.m_EcopathData.CostPct(iFleet, eCostIndex.CUPE) = fleet.EffortCost
            Me.m_EcopathData.CostPct(iFleet, eCostIndex.Sail) = fleet.SailCost
            Me.m_EcopathData.FleetColor(iFleet) = fleet.PoolColor

            For iGroup As Integer = 1 To m_EcopathData.NumGroups
                Me.m_EcopathData.Landing(iFleet, iGroup) = fleet.Landings(iGroup)
                Me.m_EcopathData.Market(iFleet, iGroup) = fleet.OffVesselValue(iGroup)
                Me.m_EcopathData.Discard(iFleet, iGroup) = fleet.Discards(iGroup)
                Me.m_EcopathData.PropDiscardMort(iFleet, iGroup) = fleet.DiscardMortality(iGroup)
            Next

            For iGroup As Integer = 1 To nDetritusGroups
                Me.m_EcopathData.DiscardFate(iFleet, iGroup) = fleet.DiscardFate(iGroup)
            Next iGroup

        Catch ex As Exception
            m_logger.LogError(ex, "updateFleets() Error: {1}", ex.Message)
            'ok figure out what happened!!!!!!!!!!!!!
            Debug.Assert(False, Me.ToString & ".updateFleets() Error: " & ex.Message)
            Return False
        End Try

    End Function

    Friend Function LoadEcopathFleetInputs() As Boolean
        Dim iFleet As Integer
        Dim iGroup As Integer

        Try

            For Each fleet As cEcopathFleetInput In m_EcopathFleetsInput

                fleet.AllowValidation = False

                iFleet = Array.IndexOf(m_EcopathData.FleetDBID, fleet.DBID)

                Debug.Assert(iFleet > 0 And iFleet <= m_EcopathData.NumFleet, "Failed to find Fleet index for database ID " & fleet.DBID.ToString)

                fleet.Resize()

                fleet.Index = iFleet

                fleet.DBID = m_EcopathData.FleetDBID(iFleet)
                fleet.Name = m_EcopathData.FleetName(iFleet)
                fleet.NominalEffort = m_EcopathData.NominalEffort(iFleet)
                fleet.FixedCost = m_EcopathData.CostPct(iFleet, eCostIndex.Fixed)
                fleet.EffortCost = m_EcopathData.CostPct(iFleet, eCostIndex.CUPE)
                fleet.SailCost = m_EcopathData.CostPct(iFleet, eCostIndex.Sail)
                fleet.PoolColor = m_EcopathData.FleetColor(iFleet)

                For iGroup = 1 To m_EcopathData.NumGroups
                    fleet.Landings(iGroup) = CSng(m_EcopathData.Landing(iFleet, iGroup))
                    fleet.OffVesselValue(iGroup) = m_EcopathData.Market(iFleet, iGroup)
                    fleet.Discards(iGroup) = CSng(m_EcopathData.Discard(iFleet, iGroup))
                    fleet.DiscardMortality(iGroup) = m_EcopathData.PropDiscardMort(iFleet, iGroup)
                Next

                For iGroup = 1 To nDetritusGroups
                    fleet.DiscardFate(iGroup) = m_EcopathData.DiscardFate(iFleet, iGroup)
                Next iGroup

                fleet.ResetStatusFlags()
                fleet.AllowValidation = True
            Next

            Return True

        Catch ex As Exception

            m_logger.LogInformation(ex, "LoadFleets() Error: {1}", ex.Message)
            Debug.Assert(False, Me.ToString & ".LoadFleets() Error: " & ex.Message)
            Return False

        End Try

    End Function


    Friend Function LoadEcopathFleetOutputs() As Boolean
        Dim iFleet As Integer

        Try

            For Each fleet As cEcopathFleetOutput In m_EcopathFleetsOutputs

                fleet.AllowValidation = False

                iFleet = Array.IndexOf(m_EcopathData.FleetDBID, fleet.DBID)

                'Debug.Assert(iFleet > 0 And iFleet <= m_EcoPathData.NumFleet, "Failed to find Fleet index for database ID " & fleet.DBID.ToString)

                fleet.Resize()

                fleet.Index = iFleet

                fleet.DBID = m_EcopathData.FleetDBID(iFleet)
                fleet.Name = m_EcopathData.FleetName(iFleet)

                For igrp As Integer = 1 To Me.nGroups
                    'Debug.Assert(Me.m_EcoPathData.Landing(iFleet, igrp) = 0)
                    fleet.CatchTotalByGroup(igrp) = Me.m_EcopathData.Landing(iFleet, igrp) + Me.m_EcopathData.Discard(iFleet, igrp)
                    fleet.CatchMortByGroup(igrp) = Me.m_EcopathData.Landing(iFleet, igrp) + (Me.m_EcopathData.Discard(iFleet, igrp) * Me.m_EcopathData.PropDiscardMort(iFleet, igrp))
                    fleet.LandingsByGroup(igrp) = Me.m_EcopathData.Landing(iFleet, igrp)
                    fleet.DiscardMortByGroup(igrp) = Me.m_EcopathData.Discard(iFleet, igrp) * Me.m_EcopathData.PropDiscardMort(iFleet, igrp)
                    fleet.DiscardSurvivalByGroup(igrp) = Me.m_EcopathData.Discard(iFleet, igrp) * (1 - Me.m_EcopathData.PropDiscardMort(iFleet, igrp))
                    fleet.DiscardByGroup(igrp) = Me.m_EcopathData.Discard(iFleet, igrp)
                Next

                fleet.ResetStatusFlags()
                fleet.AllowValidation = True
            Next

            Return True

        Catch ex As Exception

            m_logger.LogError(ex, "LoadFleets() Error: {1}", ex.Message)
            Debug.Assert(False, Me.ToString & ".LoadFleets() Error: " & ex.Message)
            Return False

        End Try

    End Function

    Public ReadOnly Property EcopathFleetInputs(iFleet As Integer) As cEcopathFleetInput
        Get
            Try
                ' List handles item index offset
                Return DirectCast(m_EcopathFleetsInput(iFleet), cEcopathFleetInput)
            Catch ex As Exception
                Return Nothing
            End Try
        End Get

    End Property

    Public ReadOnly Property EcopathFleetOutputs(iFleet As Integer) As cEcopathFleetOutput

        Get
            ' The list takes care of group index / item index differences
            Return DirectCast(m_EcopathFleetsOutputs(iFleet), cEcopathFleetOutput)
        End Get

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a Fleet to the system.
    ''' </summary>
    ''' <param name="strName">Name of the fleet.</param>
    ''' <param name="iFleet">Position to insert fleet into the current fleet list. This position may be modified by this call.</param>
    ''' <param name="iFleetID">Database ID assigned to the new fleet.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddFleet(strName As String, ByRef iFleet As Integer, ByRef iFleetID As Integer) As Boolean

        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ' Start the actual work. The data source will ensure the new fleet will be added througout models and scenarios
        If (DirectCast(DataSource, IEcopathDataSource).AddFleet(strName, iFleet, iFleetID)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.Ecopath, eDataTypes.FleetInput)
            'DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetOutput)

            If Me.ActiveEcospaceScenarioIndex > 0 Then
                Me.DataAddedOrRemovedMessage("EcoSpace number of fleets has changed.", eCoreComponentType.Ecospace, eDataTypes.EcospaceFleet)
            End If

            bSucces = True

        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    Public Function RemoveFleet(iFleet As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.RemoveFleet(Me.m_EcopathData.FleetDBID(iFleet)) Then

            Me.DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.Ecopath, eDataTypes.FleetInput)
            'Me.DataAddedOrRemovedMessage("Ecopath number of fleets has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetOutput)

            If Me.ActiveEcospaceScenarioIndex > 0 Then
                Me.DataAddedOrRemovedMessage("EcoSpace number of fleets has changed.", eCoreComponentType.Ecospace, eDataTypes.EcospaceFleet)
            End If

            bSucces = True
        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    Public Function MoveFleet(iFleet As Integer, iIndex As Integer) As Boolean
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.MoveFleet(Me.m_EcopathData.FleetDBID(iFleet), iIndex) Then

            Me.DataAddedOrRemovedMessage("Ecopath fleet order has changed.", eCoreComponentType.Ecopath, eDataTypes.FleetInput)
            'Me.DataAddedOrRemovedMessage("Ecopath fleet order has changed.", eCoreComponentType.EcoPath, eDataTypes.FleetOutput)

            If Me.ActiveEcospaceScenarioIndex > 0 Then
                Me.DataAddedOrRemovedMessage("EcoSpace group order has changed.", eCoreComponentType.Ecospace, eDataTypes.EcospaceFleet)
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
    ''' Returns the <see cref="cEwEModel">EwE model</see> for the current loaded data source.
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

        For iGroup As Integer = 1 To m_EcopathData.NumGroups
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

        For iGroup As Integer = 1 To m_EcopathData.NumGroups
            Me.m_PSDData.Include(iGroup) = Me.m_PSDParameters.GroupIncluded(iGroup)
        Next

    End Function

#End Region ' Particle size distribution

#Region " Stats "

    Friend Sub LoadEcopathStats()
        Try

            Dim sTroughput As Single = Me.m_EcopathData.Consum + Me.m_EcopathData.SumEx + Me.m_EcopathData.Dt + Me.m_EcopathData.RTZ

            Me.m_EcopathStats.Name = Me.m_EcopathData.ModelName
            Me.m_EcopathStats.TotalConsumption = Me.m_EcopathData.Consum
            Me.m_EcopathStats.TotalExports = Me.m_EcopathData.SumEx
            Me.m_EcopathStats.TotalRespFlow = Me.m_EcopathData.RTZ
            Me.m_EcopathStats.TotalFlowDetritus = Me.m_EcopathData.Dt
            Me.m_EcopathStats.TotalThroughput = sTroughput
            Me.m_EcopathStats.TotalProduction = Me.m_EcopathData.SumP

            If (Me.m_EcopathData.GEff > 0) Then
                Me.m_EcopathStats.MeanTrophicLevelCatch = Me.m_EcopathData.TLcatch
                Me.m_EcopathStats.GrossEfficiency = Me.m_EcopathData.GEff
            Else
                Me.m_EcopathStats.MeanTrophicLevelCatch = cCore.NULL_VALUE
                Me.m_EcopathStats.GrossEfficiency = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.TotalNetPP = Me.m_EcopathData.PProd

            If (Me.m_EcopathData.Totpp > 0) Then
                If (Me.m_EcopathData.RTZ > 0) Then
                    Me.m_EcopathStats.TotalPResp = Me.m_EcopathData.Totpp / Me.m_EcopathData.RTZ
                Else
                    Me.m_EcopathStats.TotalPResp = cCore.NULL_VALUE
                End If

                Me.m_EcopathStats.NetSystemProduction = Me.m_EcopathData.Totpp - Me.m_EcopathData.RTZ
                Me.m_EcopathStats.TotalPB = Me.m_EcopathData.Totpp / Me.m_EcopathData.SumBio
            Else
                If (Me.m_EcopathData.RTZ > 0) Then
                    Me.m_EcopathStats.TotalPResp = Me.m_EcopathData.PProd / Me.m_EcopathData.RTZ
                Else
                    Me.m_EcopathStats.TotalPResp = cCore.NULL_VALUE
                End If

                Me.m_EcopathStats.NetSystemProduction = Me.m_EcopathData.PProd - Me.m_EcopathData.RTZ
                Me.m_EcopathStats.TotalPB = Me.m_EcopathData.PProd / Me.m_EcopathData.SumBio
            End If

            'No Respiration if the Ecopath units are nutrients 
            If Me.m_EcopathData.areUnitCurrencyNutrients() Then
                Me.m_EcopathStats.TotalPResp = cCore.NULL_VALUE
                Me.m_EcopathStats.NetSystemProduction = cCore.NULL_VALUE
            End If

            If (sTroughput > 0) Then
                Me.m_EcopathStats.TotalBT = Me.m_EcopathData.SumBio / sTroughput
            Else
                Me.m_EcopathStats.TotalBT = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.TotalBNonDet = Me.m_EcopathData.SumBio

            If Me.m_EcopathData.CatchSum > 0 Then
                Me.m_EcopathStats.TotalCatch = Me.m_EcopathData.CatchSum
            Else
                Me.m_EcopathStats.TotalCatch = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.ConnectanceIndex = Me.m_EcopathData.Conn

            If (Me.m_EcopathData.SysOm > 0) Then
                Me.m_EcopathStats.OmnivIndex = Me.m_EcopathData.SysOm
            Else
                Me.m_EcopathStats.OmnivIndex = cCore.NULL_VALUE
            End If

            Me.m_EcopathStats.TotalMarketValue = Me.m_EcopathData.LandingValue
            Me.m_EcopathStats.TotalShadowValue = Me.m_EcopathData.ShadowValue
            Me.m_EcopathStats.TotalValue = Me.m_EcopathData.LandingValue + Me.m_EcopathData.ShadowValue
            Me.m_EcopathStats.TotalFixedCost = Me.m_EcopathData.Fixed
            Me.m_EcopathStats.TotalVarCost = Me.m_EcopathData.Variab
            Me.m_EcopathStats.TotalCost = Me.m_EcopathData.Fixed + Me.m_EcopathData.Variab
            Me.m_EcopathStats.Profit = Me.m_EcopathData.LandingValue + Me.m_EcopathData.ShadowValue - (Me.m_EcopathData.Fixed + Me.m_EcopathData.Variab)
            Me.m_EcopathStats.Pedigree = Me.m_EcopathData.PedigreeStatsModel
            Me.m_EcopathStats.MeasureOfFit = Me.m_EcopathData.PedigreeStatsTStar
            Me.m_EcopathStats.DiversityIndex = Me.m_EcopathData.DiversityIndex

            Me.m_EcopathStats.ResetStatusFlags()

        Catch ex As Exception
            m_logger.LogError(ex, "LoadEcopathStats() Error: {1}", ex.Message)
            Throw New ArgumentException(Me.ToString & ".LoadEcopathStats() Error: " & ex.Message, ex)
        End Try
    End Sub

#End Region ' Stats

#Region " Stanza "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return a <see cref="cStanzaGroup">stanza group</see> from the core.
    ''' </summary>
    ''' <param name="iIndex">Zero-based index of the group.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property StanzaGroups(iIndex As Integer) As cStanzaGroup
        Get
            Return DirectCast(Me.m_stanzaGroups(iIndex), cStanzaGroup)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the zero-based stanza index for a group index, or <see cref="NULL_VALUE"/>
    ''' if the group does not belong to a stanza.
    ''' </summary>
    ''' <param name="iGroup">The one-based group index to get the stanza for.</param>
    ''' <returns>Gets Stanza index if this group is a stanza group, or
    ''' cCore.NULL_VALUE if this group does not belong to a stanza configuration.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Friend Function getStanzaIndexForGroup(iGroup As Integer) As Integer

        For i As Integer = 1 To m_Stanza.Nsplit

            For ii As Integer = 1 To m_Stanza.Nstanza(i)
                If iGroup = m_Stanza.EcopathCode(i, ii) Then
                    Return i - 1 'stanzas are indexed from zero
                End If
            Next ii

        Next i

        Return NULL_VALUE

    End Function

#End Region ' Stanza

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run the EcoPath model.
    ''' </summary>
    ''' <returns>
    ''' True if the EcoPath model ran successfully, or False if an error occurred.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function RunEcopath() As Boolean
        Dim bDummyFlag As Boolean
        Return RunEcopath(bDummyFlag)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run the EcoPath model.
    ''' </summary>
    ''' <param name="isModelBalanced">
    ''' Return flag that indicates whether the Ecopath model balanced.
    ''' </param>
    ''' <returns>
    ''' True if the EcoPath model ran successfully, or False if an error occurred.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function RunEcopath(ByRef isModelBalanced As Boolean) As Boolean

        Dim msg As cMessage
        Dim bSuccessEcopath As Boolean = False
        Dim bSuccessPSD As Boolean = False

        Try

            If Me.m_StateMonitor.HasEcopathLoaded() = False Then
                msg = CreateMessage(My.Resources.CoreMessages.ECOPATH_ERROR_NOMODEL, eCoreComponentType.Ecopath, eMessageType.ErrorEncountered)
                m_publisher.AddMessage(msg)

                m_logger.LogInformation("RunEcoPath() Failed EcoPath Model has not been initialized. InitEcoPath(filename) must be called before .RunEcoPath().")
                Return False
            End If

            'make sure this is set correctly for this call
            'other things (Monte Carlo) could have changed this
            m_Ecopath.ParameterEstimationType = eEstimateParameterFor.ParameterEstimation

            ' Update core state
            Me.m_StateMonitor.SetEcopathRun()

            'copy all input data into the modeling arrays 
            m_EcopathData.CopyInputToModelArrays()

            Me.ResetEcopathGroupOutputs()

            'Tell the plugins that Ecopath is about to run
            If Me.PluginManager IsNot Nothing Then Me.PluginManager.EcopathRunInitialized(m_EcopathData, m_TaxonData, m_Stanza)

            'call EcoPath to estimate the missing parameters
            If (m_Ecopath.Run()) Then

                'run PSD
                '  !PSD needs to run before Ecopath outputs are populated
                If (Me.m_PSDData.Enabled) Then
                    'copy all PSD data into the modeling arrays 
                    m_PSDData.CopyInputToModelArrays()
                    ' Run PSD
                    bSuccessPSD = m_psdModel.Run()
                End If

                ' Repopulate data
                're-populate the output list with the new outputs from Ecopath
                LoadEcopathOutputs()

                LoadEcopathFleetOutputs()

                're-populate the Ecopath statistics
                LoadEcopathStats()

                If bSuccessPSD Then
                    're-populate PSD parameters
                    LoadPSDParameters()
                End If

                If Me.PluginManager IsNot Nothing Then
                    Me.PluginManager.EcopathRunCompleted(m_EcopathData, m_TaxonData, m_Stanza)
                End If
                bSuccessEcopath = True
            Else
                'Assuming here that if EcoPath returned false it has already sent a message that explains the problem 
                'No need to send another message
                m_logger.LogInformation("RunEcoPath() Failed to Estimate Parameters.")

                bSuccessEcopath = True
                bSuccessPSD = False

                If m_Ecopath.RunState = Ecopath.eEcopathRunState.Error Or
                    m_Ecopath.RunState = Ecopath.eEcopathRunState.InValidInitialization Then
                    'Only return false if there was an error
                    bSuccessEcopath = False
                    bSuccessPSD = False
                End If
            End If

        Catch ex As Exception
            msg = CreateMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_RUN_ERROR_EXCEPTION, ex.Message),
                    eCoreComponentType.Ecopath, eMessageType.ErrorEncountered)
            m_publisher.AddMessage(msg)

            m_logger.LogError(ex, "RunEcoPath() Error: {1}", ex.Message)
            bSuccessEcopath = False
            bSuccessPSD = False
            'Set the run state to Error
            m_Ecopath.RunState = Ecopath.eEcopathRunState.Error
        End Try

        ' Did Ecopath run successful?
        If bSuccessEcopath Then
            msg = New cMessage(My.Resources.CoreMessages.ECOPATH_RUN_SUCCESS, eMessageType.Any, eCoreComponentType.Ecopath, eMessageImportance.Information)
            m_publisher.AddMessage(msg)

            'Is the model balanced
            isModelBalanced = False
            If m_Ecopath.RunState = Ecopath.eEcopathRunState.Balanced Then isModelBalanced = True

            ' Update core state monitor
            Me.m_StateMonitor.SetEcopathCompleted(isModelBalanced)

            ' Write results if needed
            If Me.Autosave(eAutosaveTypes.EcopathResults) Then
                Dim writer As New cEcopathResultWriter(Me)
                writer.WriteResults()
            End If
        Else
            msg = New cMessage(My.Resources.CoreMessages.ECOPATH_RUN_ERROR, eMessageType.ErrorEncountered, eCoreComponentType.Ecopath, eMessageImportance.Warning)
            m_publisher.AddMessage(msg)

            'Yo...the model can't be balanced if it didn't run
            isModelBalanced = False
            ' Update core state monitor
            Me.m_StateMonitor.SetEcopathLoaded(True)
        End If

        ' Did PSD run successful?
        If Me.m_PSDData.Enabled Then
            If (bSuccessPSD) Then
                msg = New cMessage(My.Resources.CoreMessages.PSD_RUN_SUCCESS, eMessageType.Any, eCoreComponentType.Ecopath, eMessageImportance.Information)
                m_publisher.AddMessage(msg)
                Me.m_StateMonitor.SetPSDCompleted()
            Else
                msg = New cMessage(My.Resources.CoreMessages.PSD_RUN_ERROR, eMessageType.Any, eCoreComponentType.Ecopath, eMessageImportance.Information)
                m_publisher.AddMessage(msg)
            End If
        End If

        ' Unleash all messages after core state monitor is up to date
        m_publisher.sendAllMessages()

        Return bSuccessEcopath

    End Function

    Public Function IsModelBalanced() As Boolean
        Return (Me.m_Ecopath.RunState = Ecopath.eEcopathRunState.Balanced)
    End Function

    ''' <summary>
    ''' Take a cMessage object generated by a model and set any flags in the input or output 
    ''' that can be used by an interface to display the problem.
    ''' </summary>
    ''' <param name="msg">cMessage object created by EcoPath</param>
    ''' <remarks>
    ''' Called by EcoPathMessage_Handler(cMessage) when a message has been sent from EcoPath to the Core
    ''' </remarks>
    Private Sub ProcessMessageFromModel(msg As cMessage)

        Dim var As cVariableStatus = Nothing
        Dim i As Integer = 0
        Dim msAffected As eCoreComponentType = eCoreComponentType.NotSet

        Try

            'for each variable in the EcoPath generated message set any status flags in the input or output objects
            'this is so that an interface can see the status of the messages
            For Each var In msg.Variables

                Select Case var.DataType

                    Case eDataTypes.EcoPathGroupInput

                        Dim inputGrp As cEcoPathGroupInput = Me.EcopathGroupInputs(var.Index)
                        Dim val As cValue = inputGrp.ValueDescriptor(var.VarName)

                        If val.IsArray Then
                            For i = 1 To val.Length
                                inputGrp.SetStatus(var.VarName, var.Status, i)
                            Next
                        Else
                            Dim tmpstatus As eStatusFlags = inputGrp.GetStatus(var.VarName)
                            inputGrp.SetStatus(var.VarName, tmpstatus, var.iArrayIndex)
                            tmpstatus = tmpstatus Or var.Status
                        End If

                        ' JS: Above block replaces case-specific logic
                        'Select Case var.VarName
                        '    Case eVarNameFlags.DietComp
                        '        For i = 1 To nGroups
                        '            inputGrp.SetStatus(var.VarName, var.Status, var.Index)
                        '        Next
                        '    Case eVarNameFlags.DetritusFate
                        '        For i = 1 To nDetritusGroups
                        '            inputGrp.SetStatus(var.VarName, var.Status, var.Index)
                        '        Next
                        '    Case Else
                        '        Dim tmpstatus As eStatusFlags = inputGrp.GetStatus(var.VarName)
                        '        inputGrp.SetStatus(var.VarName, tmpstatus, var.iArrayIndex)
                        '        tmpstatus = tmpstatus Or var.Status
                        'End Select

                        'set the reference to the parent object of this variable
                        'this could not be set by EcoPath because it has no idea what this is
                        var.CoreDataObject = inputGrp
                        msAffected = eCoreComponentType.Ecopath

                    Case eDataTypes.EcoPathGroupOutput

                        Dim outputGrp As cEcopathGroupOutput = Me.EcopathGroupOutputs(var.Index)
                        Dim tmpstatus As eStatusFlags = outputGrp.GetStatus(var.VarName)
                        tmpstatus = tmpstatus Or var.Status

                        outputGrp.SetStatus(var.VarName, tmpstatus, var.iArrayIndex)
                        'set the reference to the parent object of this variable
                        'this could not be set by EcoPath because it has no idea what this is
                        var.CoreDataObject = outputGrp

                    Case eDataTypes.FleetInput

                        Dim inputFleet As cEcopathFleetInput = DirectCast(Me.m_EcopathFleetsInput(var.Index), cEcopathFleetInput)
                        Dim tmpstatus As eStatusFlags = inputFleet.GetStatus(var.VarName)
                        tmpstatus = tmpstatus Or var.Status

                        inputFleet.SetStatus(var.VarName, tmpstatus, var.iArrayIndex)
                        'set the reference to the parent object of this variable
                        'this could not be set by EcoPath because it has no idea what this is
                        var.CoreDataObject = inputFleet
                        msAffected = eCoreComponentType.Ecopath

                    Case Else

                        'A message got sent by EcoPath that is not being handled here.
                        'This is probable wrong. 
                        'Any variable that had its status flag set by EcoPath should have its status flag set in the interface.
                        m_logger.LogInformation("Message sent to Core from EcopPath that is not handled by ProcessMessageFromModel(cMessage). Message = {1}", msg.Message)
                        Debug.Assert(False, "Variable from EcoPath not handled by the Core.")

                End Select

                ' Did this affect any significant core component?
                If (msAffected <> eCoreComponentType.NotSet) And
                   (msg.Importance = eMessageImportance.Maintenance) Then
                    ' Has data source?
                    If (Me.DataSource IsNot Nothing) Then
                        Dim state As TriState = TriState.False
                        If Me.m_batchLockType = eBatchLockType.NotSet Then state = TriState.UseDefault
                        ' #Yes: dirty the data source
                        Me.DataSource.SetChanged(msAffected)
                        ' Notify state monitor of data modification
                        Me.m_StateMonitor.RegisterModification(msAffected)
                        ' Send only notifications when NO lock active
                        Me.m_StateMonitor.UpdateDataState(DataSource, state)
                        'logic before Mono compatibility changes
                        ' If(Me.m_batchLockType = eBatchLockType.NotSet, TriState.UseDefault, TriState.False)
                    End If
                End If

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
    Private Sub EcopathMessage_Handler(ByRef message As cMessage)

        Try

            If (TypeOf message Is cFeedbackMessage) Then
                Me.m_publisher.SendMessage(message)
            Else
                'this will take the variable status messages contained in the message object generated by EcoPath
                'and set flags in the input or output objects of the Core that can be used by an interface
                ProcessMessageFromModel(message)
                ' Pass message on
                Me.m_publisher.AddMessage(message)
            End If

        Catch ex As Exception
            'OOPS
            'not much we can do at this point as there is no place to post the message too
            m_logger.LogError(ex, "EcopathMessage_Handler(...) Error: {1}", ex.Message)
            Debug.Assert(False)
        End Try

    End Sub

    Private Sub PSDMessage_Handler(ByRef message As cMessage)

        Try

            'this will take the variable status messages contained in the message object generated by EcoPath
            'and set flags in the input or output objects of the Core that can be used by an interface
            ProcessMessageFromModel(message)
            ' Relay message immediately
            Me.m_publisher.SendMessage(message)

        Catch ex As Exception
            'OOPS
            'not much we can do at this point as there is no place to post the message too
            m_logger.LogError(ex, "PSDMessage_Handler(...) Error: {1}", ex.Message)
            Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Normalize ecopath input values
    ''' </summary>
    Public Sub NormalizeDietInput()
        ' Sanity check
        Debug.Assert(Me.StateMonitor.HasEcopathLoaded())
        ' Normalize ecopath DC
        Me.m_EcopathData.SumDCToOne()
        ' Refresh ecopath groups
        Me.LoadEcopathInputs()
        Me.m_StateMonitor.SetEcopathLoaded(True)
        ' Send out data changed message for ecopath
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.Ecopath, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag data source as dirty
        Me.DataSource.SetChanged(eCoreComponentType.Ecopath)
        Me.m_StateMonitor.UpdateDataState(DataSource)

    End Sub

    ''' <summary>
    ''' Statistics from the last Ecopath model run
    ''' </summary>
    Public ReadOnly Property EcopathStats() As cEcopathStats
        Get
            Return Me.m_EcopathStats
        End Get
    End Property

#Region " Status flags updating "

    Friend Function Set_PP_Flags(obj As cEcosimGroupInput, Optional bSendMessage As Boolean = True) As Boolean

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
    ''' Set the NotEditable flags for PB QB and GE
    ''' </summary>
    ''' <param name="obj">The object to update.</param>
    ''' <param name="bSendMessage">False not to send a message</param>
    ''' <returns>Always true.</returns>
    ''' <remarks>This is called by <see cref="PostVariableValidation">PostVariableValidation</see> 
    ''' to set status when a variable has been edited, as well as <see cref="cEcoPathGroupInput.ResetStatusFlags">ResetStatusFlags</see>
    ''' when the object is first created.</remarks>
    Friend Function Set_PB_QB_GE_BA_Flags(obj As cEcoPathGroupInput,
                                          Optional bSendMessage As Boolean = True) As Boolean

        'Make the variable(s) un-editable under certain circumstances
        'see EwE5 frmInputData.LockInputFor_PB_QB_GE(...)

        Dim sB As Single = CSng(obj.GetVariable(eVarNameFlags.BiomassAreaInput))
        Dim sEE As Single = CSng(obj.GetVariable(eVarNameFlags.EEInput))
        Dim sOtherMort As Single = CSng(obj.GetVariable(eVarNameFlags.OtherMortInput))
        Dim sQB As Single = CSng(obj.GetVariable(eVarNameFlags.QBInput))
        Dim sGE As Single = CSng(obj.GetVariable(eVarNameFlags.GEInput))
        Dim sPB As Single = CSng(obj.GetVariable(eVarNameFlags.PBInput))
        Dim sBA As Single = CSng(obj.GetVariable(eVarNameFlags.BioAccumInput))
        Dim sBAr As Single = CSng(obj.GetVariable(eVarNameFlags.BioAccumRate))
        Dim bLockGE As Boolean = False
        Dim bLockQB As Boolean = False
        Dim bLockPB As Boolean = False
        Dim bLockB As Boolean = False
        Dim bLockBA As Boolean = False
        Dim bLockBARate As Boolean = False
        Dim bClearPB As Boolean = False
        Dim bClearQB As Boolean = False
        Dim bClearBA As Boolean = False
        Dim bClearBARate As Boolean = False

        Dim bIsPartOfStanza As Boolean = obj.IsMultiStanza()
        Dim bIsDetritus As Boolean = (obj.PP > 1.1)
        Dim bIsProducer As Boolean = (obj.PP = 1.0)

        obj.AllowValidation = False

        ' Stanza: block all
        If bIsPartOfStanza Then bLockGE = True : bLockQB = True : bLockPB = True : bLockB = True : bLockBA = True : bLockBARate = True

        If bIsDetritus Then
            ' Detritus: block all
            bLockGE = True
            bLockQB = True
            bClearQB = True
            bLockPB = True
            bClearPB = True
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

        bClearPB = bClearPB Or (sPB <= 0)
        bClearQB = bClearQB Or (sQB <= 0)

        ' BA and BA rate cleared when PB, QB, (EE or OtherMort) and Barea entered
        If (sPB > 0) And (sQB > 0) And ((sEE > 0) Or (sOtherMort > 0)) And (sB > 0) Then
            bClearBA = True : bLockBA = True
            bClearBARate = True : bLockBARate = True
        End If

        ' Asses BA / BAr changes
        '   BaBi is only non-zero when entered. Entering BaBi will update Ba. By
        '   checking BaBi for non-zero it can be deducted which value was
        '   originally entered: BaBi or Ba.
        If sBAr <> 0 Then
            bLockBA = True : bClearBA = True
        ElseIf sBA <> 0 Then
            bLockBARate = True : bClearBARate = True
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

        If bClearPB Then
            obj.SetStatusFlags(eVarNameFlags.PBInput, eStatusFlags.Null)
        Else
            obj.ClearStatusFlags(eVarNameFlags.PBInput, eStatusFlags.Null)
        End If

        If bClearQB Then
            obj.SetStatusFlags(eVarNameFlags.QBInput, eStatusFlags.Null)
        Else
            obj.ClearStatusFlags(eVarNameFlags.QBInput, eStatusFlags.Null)
        End If

        ' -- biomass area --
        If bLockB Then
            obj.SetStatusFlags(eVarNameFlags.BiomassAreaInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.BiomassAreaInput, eStatusFlags.NotEditable)
        End If

        ' -- biomass accumulation --
        If bLockBA Then
            obj.SetStatusFlags(eVarNameFlags.BioAccumInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.BioAccumInput, eStatusFlags.NotEditable)
        End If

        If bClearBA Then
            obj.SetStatusFlags(eVarNameFlags.BioAccumInput, eStatusFlags.Null)
        Else
            obj.ClearStatusFlags(eVarNameFlags.BioAccumInput, eStatusFlags.Null)
        End If

        If bLockBARate Then
            obj.SetStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.NotEditable)
        End If

        If bClearBARate Then
            obj.SetStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.Null)
        Else
            obj.ClearStatusFlags(eVarNameFlags.BioAccumRate, eStatusFlags.Null)
        End If

        obj.AllowValidation = True

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("PB+QB+GE+BA flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        ' Update pedigree accordingly
        Me.GetPedigreeManager(eVarNameFlags.PBInput).Set_Pedigree_Flags(obj)
        Me.GetPedigreeManager(eVarNameFlags.QBInput).Set_Pedigree_Flags(obj)

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="bSendMessage"></param>
    ''' <returns>Always true.</returns>
    Friend Function Set_GS_Flags(obj As cEcoPathGroupInput, Optional bSendMessage As Boolean = True) As Boolean

        ' See EwE5 frmInputData.LockInputForProducers(..)
        obj.AllowValidation = False

        If (obj.PP >= 1.0 Or Me.m_EcopathData.areUnitCurrencyNutrients()) Then
            ' JS 08Feb16: Do not show GS values for producers or detritus
            obj.SetStatusFlags(eVarNameFlags.GS, eStatusFlags.NotEditable Or eStatusFlags.Null)
            ' obj.SetStatusFlags(eVarNameFlags.GS, eStatusFlags.NotEditable)
            'obj.GS = 0
        Else
            obj.ClearStatusFlags(eVarNameFlags.GS, eStatusFlags.NotEditable Or eStatusFlags.Null)
            ' obj.ClearStatusFlags(eVarNameFlags.GS, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("GS flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True

        Return True
    End Function

    Friend Function Set_EE_OtherMort_Flags(obj As cEcoPathGroupInput, Optional bSendMessage As Boolean = True) As Boolean

        ' See EwE5 frmInputData.DisplayBasicInput(..), detritus comment
        obj.AllowValidation = False

        Dim sPP As Single = obj.PP
        Dim sEE As Single = obj.EEInput
        Dim sOM As Single = obj.OtherMortInput
        Dim bLockEE As Boolean = False
        Dim bLockOM As Boolean = False

        bLockEE = (sOM > 0.0!) Or (sPP > 1.0)
        bLockOM = (sEE > 0.0!) Or (sPP > 1.0)

        If (bLockEE) Then
            obj.SetStatusFlags(eVarNameFlags.EEInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.EEInput, eStatusFlags.NotEditable)
        End If

        If (bLockOM) Then
            obj.SetStatusFlags(eVarNameFlags.OtherMortInput, eStatusFlags.NotEditable)
        Else
            obj.ClearStatusFlags(eVarNameFlags.OtherMortInput, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("EE+OtherMort flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True

    End Function

    Friend Function Set_DetImp_Flags(obj As cEcoPathGroupInput, Optional bSendMessage As Boolean = True) As Boolean

        ' See EwE5 frmInputData.ForMatNewGroupBasicInput(..), case 10
        obj.AllowValidation = False

        If (obj.PP <= 1.0) Then
            obj.SetStatusFlags(eVarNameFlags.DetImp, eStatusFlags.NotEditable Or eStatusFlags.Null)
        Else
            obj.ClearStatusFlags(eVarNameFlags.DetImp, eStatusFlags.NotEditable Or eStatusFlags.Null)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("DetImp flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
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
    ''' <para>That Cowan student uncovered a �bug� in the �other production� interface in ecopath. 
    ''' We do not include immigration accounting in Ecosim, so if a user sets nonzero 
    ''' immigration and emigration rates, only the emigration contribution to Z is included
    ''' in the multistanza dynamics. The problem with modeling immigration is how to specify
    ''' age-specific immigration rates for each age within any stanza specified to have
    ''' immigrating biomass; there is no obvious way to do the rates in a robustway,
    ''' especially considering that weights at age of immigrants may differ from those of
    ''' �resident� creatures. I think the best strategy is just to not allow rates to be set
    ''' to nonzero values in the other production interface.</para>
    ''' <para>Carl</para>
    ''' </remarks>
    Friend Function Set_Migration_Flags(obj As cEcoPathGroupInput, Optional bSendMessage As Boolean = True) As Boolean

        obj.AllowValidation = False

        ' JS061214: All Migration/Other production related variables are read-only for stanza groups
        If (obj.IsMultiStanza) Then
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
            Me.m_publisher.AddMessage(New cMessage("Migration flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        obj.AllowValidation = True

        Return True
    End Function

    Friend Function Set_IBM_Flags(obj As cEcospaceModelParameters, Optional bSendMessage As Boolean = True) As Boolean

        obj.AllowValidation = False

        If (obj.UseIBM) Then
            obj.ClearStatusFlags(eVarNameFlags.PacketsMultiplier, eStatusFlags.NotEditable)
        Else
            obj.SetStatusFlags(eVarNameFlags.PacketsMultiplier, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.SendMessage(New cMessage("IBM flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceModelParameter))
        End If

        obj.AllowValidation = True
        Return True
    End Function

    Friend Function Set_OffVesselValue_Flags(obj As cEcopathFleetInput, Optional bSendMessage As Boolean = True) As Boolean

        obj.AllowValidation = False

        For iGroup As Integer = 1 To Me.nGroups
            If obj.Landings(iGroup) = 0.0! Then
                obj.SetStatusFlags(eVarNameFlags.OffVesselPrice, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            Else
                obj.ClearStatusFlags(eVarNameFlags.OffVesselPrice, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            End If
        Next

        If bSendMessage Then
            Me.m_publisher.SendMessage(New cMessage("Market price flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.FleetInput))
        End If

        obj.AllowValidation = True
        Return True
    End Function

    Friend Function Set_Quota_Flags(fleetMSE As cMSEFleetInput, Optional bSendMessage As Boolean = True) As Boolean

        If fleetMSE Is Nothing Then
            'If Ecosim has not been loaded then the cMSEFleetInput objects will be Nothing
            'boot out of here in that case
            Return False
        End If

        fleetMSE.AllowValidation = False

        Dim fleet As cEcopathFleetInput = Me.EcopathFleetInputs(fleetMSE.Index)
        For iGroup As Integer = 1 To Me.nGroups
            If (fleet.Landings(iGroup) + fleet.Discards(iGroup)) = 0.0! Then
                fleetMSE.SetStatusFlags(eVarNameFlags.QuotaShare, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            Else
                fleetMSE.ClearStatusFlags(eVarNameFlags.QuotaShare, eStatusFlags.Null Or eStatusFlags.NotEditable, iGroup)
            End If
        Next

        If bSendMessage Then
            Me.m_publisher.SendMessage(New cMessage("Quota flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.MSEFleetInput))
        End If

        fleetMSE.AllowValidation = True
        Return True
    End Function

    Friend Function Set_DiscardMort_Flags(fleet As cEcopathFleetInput, Optional bSendMessage As Boolean = True) As Boolean

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
            Me.m_publisher.SendMessage(New cMessage("Discard mort flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.FleetInput))
        End If

        fleet.AllowValidation = True
        Return True
    End Function

    Friend Function Set_VBK_Flags(group As cEcoPathGroupInput, Optional bSendMessage As Boolean = True) As Boolean

        Dim sg As cStanzaGroup = Nothing
        Dim groupLeading As cEcoPathGroupInput = Nothing
        Dim bIsLeadingGroup As Boolean = False

        group.AllowValidation = False

        ' Is a multi-stanza group?
        If group.IsMultiStanza Then
            ' #Yes: configure VBK editable mode
            sg = Me.StanzaGroups(group.iStanza)

            ' Get the leading group for this stanza config

            groupLeading = Me.EcopathGroupInputs(sg.iGroups(sg.LeadingB))
            bIsLeadingGroup = ReferenceEquals(groupLeading, group)

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
            Me.m_publisher.AddMessage(New cMessage("VBK flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        group.AllowValidation = True
    End Function

    Friend Function Set_Tcatch_Flags(group As cEcoPathGroupInput, Optional bSendMessage As Boolean = True) As Boolean

        Dim iGroup As Integer = group.Index
        Dim bIsFished As Boolean = False

        group.AllowValidation = False

        ' Is multi-stanza?
        If group.IsMultiStanza Then
            ' #Yes: determine if this is the first stanza group that is being fished (ouch)
            Dim sg As cStanzaGroup = Me.StanzaGroups(group.iStanza)
            Dim iAgeYoungest As Integer = 0
            Dim iYoungest As Integer = 0

            ' Determine lifestage index of youngest life stage that is being fished
            ' ..For all life stages
            For iLifestage As Integer = 1 To sg.nLifeStages
                ' ..For all fleets
                For iFleet As Integer = 1 To Me.nFleets
                    ' Is this life stage being caught?
                    If (Me.m_EcopathData.Landing(iFleet, sg.iGroups(iLifestage)) +
                        Me.m_EcopathData.Discard(iFleet, sg.iGroups(iLifestage))) > 0 Then

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
                If (Me.m_EcopathData.Landing(iFleet, iGroup) + Me.m_EcopathData.Discard(iFleet, iGroup)) > 0 Then
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
            Me.m_publisher.AddMessage(New cMessage("TCatch flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        group.AllowValidation = True

        ' Update pedigree accordingly
        Dim man As cPedigreeManager = Me.GetPedigreeManager(eVarNameFlags.TCatchInput)
        If (man IsNot Nothing) Then man.Set_Pedigree_Flags(group)

    End Function

    Friend Function Set_Tmax_Flags(group As cEcoPathGroupInput, Optional bSendMessage As Boolean = True) As Boolean
        group.AllowValidation = False

        ' Is a multi-stanza group?
        If group.IsMultiStanza Then
            ' #Yes: Make Tmax non-editable to the user
            group.SetStatusFlags(eVarNameFlags.TmaxInput, eStatusFlags.NotEditable)
        Else
            ' #No: Make Tmax editable to the user
            group.ClearStatusFlags(eVarNameFlags.TmaxInput, eStatusFlags.NotEditable)
        End If

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("TMax flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))
        End If

        group.AllowValidation = True
    End Function

    Friend Function Set_EconomicAvailable_Flags(parms As cCoreInputOutputBase, varname As eVarNameFlags) As Boolean

        Dim bAllowValidationOrg As Boolean = parms.AllowValidation
        Dim bAvailable As Boolean = False
        Dim ds As cEconomicDataSource = cEconomicDataSource.getInstance()

        If (ds IsNot Nothing) Then
            bAvailable = ds.IsDataAvailable(New EwECore.cEcosimRunType)
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

    Friend Function Set_Taxon_Flags(t As cTaxon, Optional bSendMessage As Boolean = True) As Boolean

        Dim bIsNotFish As Boolean = False
        Dim bIshigherOrganism As Boolean = False
        Dim bIsFished As Boolean = True
        Dim bIsStanza As Boolean = (t.iStanza >= 0)
        Dim bIsGroup As Boolean = (t.iGroup > 0)
        Dim bClearVI As Boolean = False

        ' Cheung Vulnerability index is only available for fish (per FishBase)
        bIsNotFish = (t.OrganismType <> eOrganismTypes.Fishes)
        bIshigherOrganism = (t.OrganismType <> eOrganismTypes.Bacteria And t.OrganismType <> eOrganismTypes.Fungi)

        t.AllowValidation = False

        Try

            If bIsStanza Then
                bIsFished = Me.StanzaGroups(t.iStanza - 1).IsFished
            ElseIf bIsGroup Then
                bIsFished = Me.EcopathGroupInputs(t.iGroup).IsFished
            Else
                ' Orphaned / unassigned record
                bIsFished = False
            End If

            If bIshigherOrganism Then
                t.ClearStatusFlags(eVarNameFlags.IUCNConservationStatus, eStatusFlags.NotEditable)
                t.ClearStatusFlags(eVarNameFlags.TaxonMaxLength, eStatusFlags.NotEditable)
                t.ClearStatusFlags(eVarNameFlags.TaxonMeanLength, eStatusFlags.NotEditable)
                t.ClearStatusFlags(eVarNameFlags.TaxonMeanLifespan, eStatusFlags.NotEditable)
                t.ClearStatusFlags(eVarNameFlags.TaxonMeanWeight, eStatusFlags.NotEditable)
            Else
                t.SetStatusFlags(eVarNameFlags.IUCNConservationStatus, eStatusFlags.NotEditable)
                t.SetStatusFlags(eVarNameFlags.TaxonMaxLength, eStatusFlags.NotEditable)
                t.SetStatusFlags(eVarNameFlags.TaxonMeanLength, eStatusFlags.NotEditable)
                t.SetStatusFlags(eVarNameFlags.TaxonMeanLifespan, eStatusFlags.NotEditable)
                t.SetStatusFlags(eVarNameFlags.TaxonMeanWeight, eStatusFlags.NotEditable)
            End If

            ' == Prop of biomass
            If bIsStanza Then
                t.SetStatusFlags(eVarNameFlags.TaxonPropBiomass, eStatusFlags.NotEditable)
            Else
                t.ClearStatusFlags(eVarNameFlags.TaxonPropBiomass, eStatusFlags.NotEditable)
            End If

            ' == Prop of catch (not editable when not fished or a stanza taxon)
            If bIsFished And Not bIsStanza Then
                t.ClearStatusFlags(eVarNameFlags.TaxonPropCatch, eStatusFlags.NotEditable)
            Else
                t.SetStatusFlags(eVarNameFlags.TaxonPropCatch, eStatusFlags.NotEditable)
            End If

            If (Not bIsFished) Or (t.PropC <= 0) Then
                t.SetStatusFlags(eVarNameFlags.TaxonPropCatch, eStatusFlags.Null)
            Else
                t.ClearStatusFlags(eVarNameFlags.TaxonPropCatch, eStatusFlags.Null)
            End If

            ' == VI ==
            If bIsNotFish Then
                t.SetStatusFlags(eVarNameFlags.TaxonVulnerabilityIndex, eStatusFlags.NotEditable)
            Else
                t.ClearStatusFlags(eVarNameFlags.TaxonVulnerabilityIndex, eStatusFlags.NotEditable)
            End If

            If (bIsNotFish Or (t.VulnerabilityIndex < 0)) Then
                t.SetStatusFlags(eVarNameFlags.TaxonVulnerabilityIndex, eStatusFlags.Null)
            Else
                t.ClearStatusFlags(eVarNameFlags.TaxonVulnerabilityIndex, eStatusFlags.Null)
            End If

        Catch ex As Exception
            m_logger.LogError("Set_Taxon_Flags. Error setting taxon flags: {message}", ex.Message)
        End Try

        t.AllowValidation = True

        If bSendMessage Then
            Me.m_publisher.AddMessage(New cMessage("Taxon flags updated", eMessageType.DataModified,
                    eCoreComponentType.Ecopath, eMessageImportance.Maintenance, t.DataType))
        End If

    End Function

    Friend Function Set_BadHab_Flags(grp As cEcospaceGroupInput) As Boolean

        'Dim b As Boolean = grp.AllowValidation()
        'Dim s As eStatusFlags = eStatusFlags.Null Or eStatusFlags.NotEditable
        'grp.AllowValidation = False

        'Select Case grp.CapacityCalculationType
        '    Case eEcospaceCapacityCalType.Habitat
        '        grp.ClearStatusFlags(eVarNameFlags.RelMoveBad, s)
        '        grp.ClearStatusFlags(eVarNameFlags.RelVulBad, s)
        '        grp.ClearStatusFlags(eVarNameFlags.EatEffBad, s)
        '    Case eEcospaceCapacityCalType.EnvResponses
        '        grp.SetStatusFlags(eVarNameFlags.RelMoveBad, s)
        '        grp.SetStatusFlags(eVarNameFlags.RelVulBad, s)
        '        grp.SetStatusFlags(eVarNameFlags.EatEffBad, s)
        '    Case Else
        '        Debug.Assert(False)
        'End Select
        'grp.AllowValidation = b

    End Function

    Friend Function Set_HabPref_Flags(grp As cEcospaceGroupInput) As Boolean

        Dim b As Boolean = grp.AllowValidation()
        Dim s As eStatusFlags = eStatusFlags.Null Or eStatusFlags.NotEditable
        grp.AllowValidation = False

        ' ToDo: CEFAS requested that users can turn off specific habitats for capacity calculations (just like can be done for env drivers)
        '       Habitat enabled / disabled state check needs to be included in the status flags
        For iHabitat As Integer = 0 To Me.nHabitats - 1
            If ((grp.CapacityCalculationType And eEcospaceCapacityCalType.Habitat) = eEcospaceCapacityCalType.Habitat) Then
                grp.ClearStatusFlags(eVarNameFlags.PreferredHabitat, s, iHabitat)
            Else
                grp.SetStatusFlags(eVarNameFlags.PreferredHabitat, s, iHabitat)
            End If
        Next
        grp.AllowValidation = b

    End Function

    Friend Function Set_Migratory_Flags(grp As cEcospaceGroupInput) As Boolean

        Dim b As Boolean = grp.AllowValidation()
        Dim s As eStatusFlags = eStatusFlags.Null Or eStatusFlags.NotEditable
        grp.AllowValidation = False

        If grp.IsMigratory Then
            grp.ClearStatusFlags(eVarNameFlags.BarrierAvoidanceWeight, s)
            grp.ClearStatusFlags(eVarNameFlags.InMigAreaMoveWeight, s)
        Else
            grp.SetStatusFlags(eVarNameFlags.BarrierAvoidanceWeight, s)
            grp.SetStatusFlags(eVarNameFlags.InMigAreaMoveWeight, s)
        End If
        grp.AllowValidation = b

    End Function

    Private Function Cascade_Name(strName As String, obj As cCoreInputOutputBase, msg As cMessage) As Boolean

        Dim objCascade As cCoreInputOutputBase = Nothing
        Dim bAllowValidationOrg As Boolean = False

        Select Case obj.DataType
            Case eDataTypes.EcoPathGroupInput, eDataTypes.EcoPathGroupOutput,
                 eDataTypes.EcoSimGroupInput, eDataTypes.EcoSimGroupOutput,
                 eDataTypes.EcospaceGroup, eDataTypes.EcotracerGroupInput

                ' Cascase group name to all relevant core IO objects
                objCascade = Me.EcopathGroupInputs(obj.Index)
                If Not ReferenceEquals(objCascade, obj) Then
                    bAllowValidationOrg = objCascade.AllowValidation
                    objCascade.AllowValidation = False
                    objCascade.Name = strName
                    objCascade.AllowValidation = bAllowValidationOrg

                    msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                End If

                objCascade = Me.EcopathGroupOutputs(obj.Index)
                If Not ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                    bAllowValidationOrg = objCascade.AllowValidation
                    objCascade.AllowValidation = False
                    objCascade.Name = strName
                    objCascade.AllowValidation = bAllowValidationOrg

                    msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                End If

                If Me.m_StateMonitor.HasEcosimLoaded() Then
                    objCascade = Me.EcosimGroupInputs(obj.Index)
                    If Not ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg
                    End If

                    ' Ugh
                    objCascade = Me.MSEBatchManager.TFMGroups(obj.Index)
                    If Not ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg
                    End If
                End If

                If Me.m_StateMonitor.HasEcospaceLoaded() Then
                    objCascade = Me.EcospaceGroupInputs(obj.Index)
                    If Not ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg

                        msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                    End If
                End If

                If Me.m_StateMonitor.HasEcotracerLoaded() Then
                    objCascade = Me.EcotracerGroupInputs(obj.Index)
                    If Not ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg

                        msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                    End If
                End If

            Case eDataTypes.FleetInput,
                 eDataTypes.EcosimFleetInput,
                 eDataTypes.EcosimFleetOutput,
                 eDataTypes.EcospaceFleet,
                 eDataTypes.MSEFleetInput

                ' Cascase fleet name to all relevant core IO objects
                objCascade = Me.EcopathFleetInputs(obj.Index)
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.AllowValidation = False
                objCascade.Name = strName
                objCascade.AllowValidation = bAllowValidationOrg

                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))

                If Me.m_StateMonitor.HasEcosimLoaded() Then
                    objCascade = Me.EcosimFleetInputs(obj.Index)
                    If Not ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg
                    End If
                    objCascade = Me.EcosimFleetOutput(obj.Index)
                    If Not ReferenceEquals(objCascade, obj) And objCascade IsNot Nothing Then
                        bAllowValidationOrg = objCascade.AllowValidation
                        objCascade.AllowValidation = False
                        objCascade.Name = strName
                        objCascade.AllowValidation = bAllowValidationOrg
                    End If
                End If

                If Me.m_StateMonitor.HasEcospaceLoaded() Then
                    objCascade = Me.EcospaceFleetInputs(obj.Index)
                    bAllowValidationOrg = objCascade.AllowValidation
                    objCascade.AllowValidation = False
                    objCascade.Name = strName
                    objCascade.AllowValidation = bAllowValidationOrg

                    msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.Name))
                End If

        End Select
    End Function

    Private Sub Cascade_PP(sPP As Single, obj As cCoreGroupBase, msg As cMessage)

        Dim objCascade As cCoreGroupBase = Nothing
        Dim bAllowValidationOrg As Boolean = False

        Debug.Assert(obj.DataType = eDataTypes.EcoPathGroupInput)

        If Me.m_StateMonitor.HasEcosimLoaded() Then
            objCascade = Me.EcosimGroupInputs(obj.Index)
            If objCascade IsNot Nothing Then
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.AllowValidation = True
                objCascade.PP = sPP
                objCascade.ResetStatusFlags()
                objCascade.AllowValidation = bAllowValidationOrg
                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.PP))
            End If

            objCascade = Me.EcosimGroupOutputs(obj.Index)
            If objCascade IsNot Nothing Then
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.PP = sPP
                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.PP))
            End If

        End If

        If Me.m_StateMonitor.HasEcospaceLoaded() Then
            objCascade = Me.EcospaceGroupInputs(obj.Index)
            If objCascade IsNot Nothing Then
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.AllowValidation = True
                objCascade.PP = sPP
                objCascade.ResetStatusFlags()
                objCascade.AllowValidation = bAllowValidationOrg
                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.PP))
            End If
            objCascade = Me.EcospaceGroupOutput(obj.Index)
            If objCascade IsNot Nothing Then
                bAllowValidationOrg = objCascade.AllowValidation
                objCascade.PP = sPP
                msg.AddVariable(GetAffectedVariableStatus(objCascade, eVarNameFlags.PP))
            End If
        End If

    End Sub

    Private Sub Cascade_VBK(sVBK As Single, group As cEcoPathGroupInput, msg As cMessage)

        Dim groupCascade As cEcoPathGroupInput = Nothing
        Dim bAllowValidationOrg As Boolean = False
        Dim iStanza As Integer = Me.getStanzaIndexForGroup(group.Index)

        Debug.Assert(iStanza = group.iStanza)

        ' Is not a stanza life stage?
        If (iStanza < 0) Then Return

        For iGroup As Integer = 1 To Me.nGroups
            groupCascade = Me.EcopathGroupInputs(iGroup)

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
            group = Me.EcopathGroupInputs(iGroup)
            If (group.IsMultiStanza) Then
                Me.Set_Tcatch_Flags(group, True)
            End If
        Next

    End Sub

    Private Sub Update_Taxon_Catches()

        Dim taxon As cTaxon = Nothing

        For iTaxon As Integer = 1 To Me.nTaxon
            taxon = Me.Taxon(iTaxon)
            Me.Set_Taxon_Flags(taxon)
        Next

    End Sub

    Private Sub Update_IsFished(bSendMessage As Boolean)

        Dim group As cEcoPathGroupInput = Nothing
        Dim fleet As cEcopathFleetInput = Nothing
        Dim stanza As cStanzaGroup = Nothing
        Dim bIsFished As Boolean = False
        Dim msg As cMessage = Nothing
        Dim vs As cVariableStatus = Nothing

        For iGroup As Integer = 1 To Me.nGroups
            group = Me.EcopathGroupInputs(iGroup)

            bIsFished = False
            For iFleet As Integer = 1 To Me.nFleets
                fleet = Me.EcopathFleetInputs(iFleet)
                If fleet.Landings(iGroup) > 0 Or fleet.Discards(iGroup) > 0 Then
                    bIsFished = True
                    Exit For
                End If
            Next

            If bIsFished <> group.IsFished Then
                group.AllowValidation = False
                group.IsFished = bIsFished
                If bSendMessage Then
                    If msg Is Nothing Then
                        msg = New cMessage("Group fished state has changed", eMessageType.DataModified, eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput)
                    End If
                    vs = New cVariableStatus(group, eStatusFlags.ValueComputed, "Group " & group.Name & " is " & If(bIsFished, "", "not ") & "fished", eVarNameFlags.IsFished, eDataTypes.EcoPathGroupInput, eCoreComponentType.Ecopath, group.Index)
                    msg.AddVariable(vs)
                End If
                group.AllowValidation = True
            End If
        Next

        For iStanza As Integer = 0 To Me.nStanzas - 1
            stanza = Me.StanzaGroups(iStanza)
            bIsFished = False
            For iIndex As Integer = 1 To stanza.nLifeStages
                Dim iGroup As Integer = stanza.iGroups(iIndex)
                group = Me.EcopathGroupInputs(iGroup)
                If group.IsFished Then
                    bIsFished = True
                    Exit For
                End If
            Next

            If (bIsFished <> stanza.IsFished) Then
                stanza.AllowValidation = False
                stanza.IsFished = bIsFished
                If bSendMessage Then
                    If msg Is Nothing Then
                        msg = New cMessage("Stanza fished state has changed", eMessageType.DataModified, eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.Stanza)
                    End If
                    vs = New cVariableStatus(group, eStatusFlags.ValueComputed, "Stanza " & stanza.Name & " is " & If(bIsFished, "", "not ") & "fished", eVarNameFlags.IsFished, eDataTypes.Stanza, eCoreComponentType.Ecopath, iStanza)
                    msg.AddVariable(vs)
                End If
                stanza.AllowValidation = True
            End If
        Next

        If msg IsNot Nothing Then
            Me.Messages.AddMessage(msg)
        End If

    End Sub

    Private Sub Cascade_TCatchInput(sTCatchInput As Single, group As cEcoPathGroupInput, msg As cMessage)

        Dim groupCascade As cEcoPathGroupInput = Nothing
        Dim bAllowValidationOrg As Boolean = False
        Dim iStanza As Integer = Me.getStanzaIndexForGroup(group.Index)

        Debug.Assert(iStanza = group.iStanza)

        ' Is not a stanza life stage?
        If (iStanza < 0) Then Return

        For iGroup As Integer = 1 To Me.nGroups

            groupCascade = Me.EcopathGroupInputs(iGroup)
            Debug.Assert(Me.getStanzaIndexForGroup(iGroup) = groupCascade.iStanza)

            If (iGroup <> group.Index) And (Me.getStanzaIndexForGroup(iGroup) = iStanza) And groupCascade.IsFished Then

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

    Friend m_Ecosim As Ecosim.cEcosimModel 'the EcoSim Model itself
    'EcoSim parameters that are not meant for public consumption this is the underlying data structures of the EcoSim Model
    'this data is exposed so that it can be serialized.
    'for public access to these parameters see EcoSimGroupOutputs(...) and other access methods.
    Friend m_EcoSimData As cEcosimDatastructures = Nothing
    Friend m_SearchData As cSearchDatastructures

    Private m_EcoSimRun As cEcoSimModelParameters 'private copy of EcoSim model parameters. Public access will through a reference to this object
    Friend m_EcoSimGroups As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoSimGroupInput, 1)
    Friend m_EcoSimGroupOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoSimGroupOutput, 1)
    Friend m_EcoSimScenarios As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoSimScenario, 1)
    Friend m_EcosimFleetInputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcosimFleetInput, 1)
    Friend m_EcosimFleetOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 0)
    Private m_MediatedInteractionManager As cMediatedInteractionManager

    Private m_EcopathStats As cEcopathStats
    Private m_EcosimStats As cEcosimStats
    Private m_EcosimOutputs As cEcosimOutput
    Private m_EcospaceStats As cEcospaceStats

    'Delegate for Time-Step notification from the interface 
    Private m_InterfaceDelegate As Ecosim.EcoSimTimeStepDelegate
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Friend m_MSEData As cMSEDataStructures

#End Region ' Variables

    ''' <summary>
    ''' Start biomass of each group
    ''' </summary>
    ''' <remarks>Added by FG temporarily. For the map plotting
    ''' JJ: Please Update it to the correct core class 
    ''' </remarks>
    Public ReadOnly Property StartBiomass(iGroup As Integer) As Single
        Get
            Return m_EcoSimData.StartBiomass(iGroup)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get an <see cref="cEcoSimScenario">Ecosim scenario</see> from the available scenarios.
    ''' </summary>
    ''' <param name="iScenario">One-based indexed of the scenario to load
    ''' [1, <see cref="nEcosimScenarios">#scenarios</see>].</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcosimScenarios(iScenario As Integer) As cEcoSimScenario
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
    Public ReadOnly Property nEcosimScenarios() As Integer
        Get
            Try
                Return Me.m_EcopathData.NumEcosimScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the index of the active <see cref="cEcosimScenario">Ecosim scenario</see>.
    ''' If no scenario is loaded, a value &lt; 1 will be returned.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveEcosimScenarioIndex() As Integer
        Get
            If Not Me.StateMonitor.HasEcosimLoaded Then Return -1
            Return Me.m_EcopathData.ActiveEcosimScenario
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
            ' #No: no time reference: return model year from Ecopath
            Return Me.m_EcopathData.FirstYear
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
    Public Function EcosimOutputs() As cEcosimOutput
        Return Me.m_EcosimOutputs
    End Function

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
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.Ecosim, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag data source as dirty
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
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.Ecosim, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag data source as dirty
        Me.DataSource.SetChanged(eCoreComponentType.MSE)
        Me.m_StateMonitor.UpdateDataState(DataSource)
    End Sub

    Public Sub SetDefaultTFM()

        ' Sanity check
        Debug.Assert(Me.StateMonitor.HasEcosimLoaded())
        ' Set default MSE quota share values
        Me.MSEManager.SetDefaultTFM()
        Me.m_StateMonitor.SetEcoSimLoaded(True)
        ' Send out data changed message for MSE
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.Ecosim, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag data source as dirty
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
            Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.Ecosim, eMessageType.DataModified))
            Me.m_publisher.sendAllMessages()
            ' Flag data source as dirty
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
        Me.m_publisher.AddMessage(Me.CreateMessage("", eCoreComponentType.Ecosim, eMessageType.DataModified))
        Me.m_publisher.sendAllMessages()
        ' Flag data source as dirty
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

            Me.m_bEcoSimIsInit = False

            'has the core been initialized
            'If Not m_bCoreIsInit Then
            '    'ToDo_jb InitEcoSim() failed send a message ??????
            '    Debug.Assert(False, "Core has not been initialized.")
            '    Return False
            'End If

            Me.m_Ecosim = New Ecosim.cEcosimModel(Me.EcoFunction)

            Dim mh As New cMessageHandler(AddressOf Me.EcosimMessageHandler, eCoreComponentType.Ecosim, eMessageType.Any, Me.m_SyncObj)
#If DEBUG Then
            mh.Name = "cCore::Ecosim"
#End If
            Me.m_Ecosim.Messages.AddMessageHandler(mh)

            'set the output variables from EcoPath as the Input for EcoSim
            'this sets the baseline state for EcoSim as the last run EcoPath model
            Me.m_Ecosim.EcopathData = Me.m_EcopathData
            Me.m_Ecosim.m_Data = Me.m_EcoSimData
            Me.m_Ecosim.m_stanza = Me.m_Stanza
            Me.m_Ecosim.TracerData = Me.m_tracerData
            Me.m_Ecosim.TimeSeriesData = Me.m_TSData
            Me.m_Ecosim.MSEData = Me.m_MSEData

            Me.m_EcosimOutputs = New cEcosimOutput(Me)

            Me.CreateSearchManagers()

            'Build all the shape managers
            Me.m_ShapeManagers.Clear()
            Dim manager As cBaseShapeManager

            manager = New cForcingFunctionShapeManager(m_EcoSimData, Me, eDataTypes.Forcing)
            Me.m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cMediationShapeManager(m_EcoSimData, Me, eDataTypes.Mediation)
            Me.m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cEggProductionShapeManager(m_EcoSimData, Me, eDataTypes.EggProd)
            Me.m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cFishingEffortShapeManger(m_EcoSimData, Me, eDataTypes.FishingEffort)
            Me.m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cFishingMortalityShapeManger(m_EcoSimData, Me, eDataTypes.FishMort)
            Me.m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cLandingsMediationShapeManager(m_EcoSimData, Me, eDataTypes.PriceMediation)
            Me.m_ShapeManagers.Add(manager.DataType, manager)

            manager = New cEnviroResponseShapeManager(m_EcoSimData, Me.m_EcospaceData, Me, eDataTypes.CapacityMediation)
            Me.m_ShapeManagers.Add(manager.DataType, manager)

            Me.m_MediatedInteractionManager = New cMediatedInteractionManager(m_EcopathData, m_EcoSimData, Me)
            Me.m_FitToTimeSeriesData = New cF2TSDataStructures()

            'Environmental response managers
            'Foraging capacity
            m_EcosimEnviroResponseManager = New cEcosimEnviroResponseManager(Me)
            m_EcosimEnviroResponseManager.Init(Me.m_EcoSimData, Me.m_EcoSimData.CapEnvResData)
            Me.m_Ecosim.EcosimEnviroResponseManager = m_EcosimEnviroResponseManager
            'Mortality MO
            m_EcosimMortalityResponseManager = New cEcosimMortalityResponseManager(Me)
            m_EcosimMortalityResponseManager.Init(Me.m_EcoSimData, Me.m_EcoSimData.CapEnvResData)
            Me.m_Ecosim.EcosimMortalityResponseManager = m_EcosimMortalityResponseManager

            'manager = New cEcosimResponseShapeManager(m_EcoSimData, Me, eDataTypes.EcosimEnviroResponseFunctionManager)
            'Me.m_ShapeManagers.Add(manager.DataType, manager)

            Me.m_bEcoSimIsInit = True

            ' Set core state
            Me.m_StateMonitor.SetEcoSimLoaded(False)

            Return True

        Catch ex As Exception
            m_logger.LogError("InitEcoSim. Error initializing EcoSim: {message}", ex.Message)
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Private Sub EcosimMessageHandler(ByRef Message As cMessage)
        m_publisher.AddMessage(Message)
    End Sub

    Private Function InitEcosimScenarios() As Boolean
        Me.m_EcoSimScenarios.Clear()
        For i As Integer = 1 To Me.m_EcopathData.EcosimScenarioName.Length - 1
            Me.m_EcoSimScenarios.Add(Me.privateEcoSimScenario(i))
        Next
        Return True
    End Function


    Friend Sub InitEcosimLinks()

        ' ToDo: protect this
        For i As Integer = 1 To Me.m_EcopathData.NumGroups
            Me.m_EcoSimData.StartBiomass(i) = Me.m_EcopathData.B(i)
        Next i
        Me.m_Ecosim.CalcEatenOfBy()

    End Sub

    ''' <summary>
    ''' Load an <see cref="cEcoSimScenario">Ecosim scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcoSimScenario">Scenario</see> to load.</param>
    ''' <returns>True if successful.</returns>
    Public Function LoadEcosimScenario(scenario As cEcoSimScenario) As Boolean
        Return LoadEcosimScenario(scenario.Index)
    End Function

    Private Sub SendEcosimLoadStateMessage(strScenarioName As String, Optional strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_LOAD_SUCCESS, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecosim, eMessageImportance.Information, eDataTypes.EcoSimScenario)
        Else
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_LOAD_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning, eDataTypes.EcoSimScenario)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()
    End Sub

    Private Sub SendEcosimSaveStateMessage(strScenarioName As String, Optional bSucces As Boolean = True,
            Optional strError As String = "")

        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If bSucces Then
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_SAVE_SUCCESS, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataModified, eCoreComponentType.Ecosim, eMessageImportance.Information, eDataTypes.EcoSimScenario)
        Else
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_SAVE_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning, eDataTypes.EcoSimScenario)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()
    End Sub

    ''' <summary>
    ''' Creates and loads a new Ecosim scenario.
    ''' </summary>
    ''' <param name="strName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <returns>True if successful.</returns>
    Public Function NewEcosimScenario(strName As String, strDescription As String, strAuthor As String, strContact As String) As Boolean

        Dim ds As IEcosimDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim iScenario As Integer = 0

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcosimDatasource) Then Return False

        If Me.m_StateMonitor.HasEcopathLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        Try

            ds = DirectCast(DataSource, IEcosimDatasource)
            If (ds.AppendEcosimScenario(strName, strDescription, strAuthor, strContact, iScenarioID)) Then

                Me.StateMonitor.UpdateDataState(Me.DataSource)
                Me.InitEcosimScenarios()
                DataAddedOrRemovedMessage("Ecosim number of scenarios has changed.", eCoreComponentType.Ecosim, eDataTypes.EcoSimScenario)
                iScenario = Array.IndexOf(Me.m_EcopathData.EcosimScenarioDBID, iScenarioID)

                If (Me.PluginManager IsNot Nothing) Then
                    Me.PluginManager.EcosimScenarioAdded(Me.DataSource, iScenarioID)
                End If

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
    ''' <param name="iScenario">One-based index of the <see cref="cEcoSimScenario">Scenario</see> in the <see cref="m_EcoSimScenarios">Scenario list</see>.</param>
    ''' <returns>True if successful.</returns>
    Public Function LoadEcosimScenario(iScenario As Integer) As Boolean

        If (iScenario < 1) Then Return False
        If (Me.nEcosimScenarios < iScenario) Then Return False
        If (Not Me.StateMonitor.HasEcopathLoaded) Then Return False

        Dim ds As IEcosimDatasource = Nothing
        Dim strScenarioName As String = Me.m_EcopathData.EcosimScenarioName(iScenario)

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcosimDatasource) Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        Try

            ' Update core state
            Me.CloseEcosimScenario()

            Me.m_EcopathData.ActiveEcospaceScenario = -1
            Me.m_EcopathData.ActiveEcotracerScenario = -1

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
                If Not RunEcopath() Then
                    'Debug.Assert(False, Me.ToString & ".RunEcoSim() Failed to Run EcoPath.")
                    m_logger.LogInformation("RunEcoSim(). Failed to run EcoPath scenario '{scenarioName}'", strScenarioName)
                    Me.SendEcosimLoadStateMessage(strScenarioName, My.Resources.CoreMessages.ECOPATH_RUN_ERROR)
                    Return False
                End If
            End If

            'things that need to happen before a scenario is loaded
            m_Ecosim.SearchData = m_SearchData
            m_Ecosim.SetCounters()
            m_Ecosim.InitStanza()
            m_Ecosim.SetDefaultParameters()

            m_TSData.ClearTimeSeries()

            'jb I still need to deal with how to handle these problems
            ds = DirectCast(DataSource, IEcosimDatasource)
            If Not ds.LoadEcosimScenario(Me.m_EcopathData.EcosimScenarioDBID(iScenario)) Then
                Debug.Assert(False, "LoadEcosimScenario() Failed to load scenario from data source.")
                Me.SendEcosimLoadStateMessage(strScenarioName, "Failed to read the database")
                Return False
            End If


            Me.m_SearchData.RedimToSimScenario(Me.nEcosimYears)

            '  Me.m_EcoSimData.Debug_InitEcosimResponse()

            'set the default summary time periods
            m_EcoSimData.DefaultSummaryPeriods()
            m_EcoSimData.SetDefaultCatchabilities(Me.m_EcopathData.Landing, Me.m_EcopathData.Discard, Me.m_EcopathData.B)
            m_Ecosim.Init(True)

            InitEcosimGroups()
            InitEcosimFleetInput()
            InitEcosimModelParameters()

            'rebuild all the shapes in the shape managers
            For Each manager As cBaseShapeManager In m_ShapeManagers.Values
                manager.Init() 'init will rebuild and load all the shapes in the manager
            Next

            m_MediatedInteractionManager.Init()
            m_MediatedInteractionManager.Load()

            m_ArenaManager.Init()
            m_ArenaManager.Load()

            InitEcosimGroupOutput()
            InitEcosimFleetOutput()

            InitEcosimTimeSeries()
            LoadEcosimTimeSeries()

            'CheckEcosimCatchability()

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

            Me.m_EcosimEnviroResponseManager.Load(Me.ForcingShapeManager)
            Me.m_EcosimMortalityResponseManager.Load(Me.ForcingShapeManager)

            ' Me.m_EcoSim.InitAssessment()

            ' Update economic data state for Ecosim objects
            Me.OnEconomicDataPluginEnabled()

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.EcosimLoadScenario(ds)
                Me.PluginManager.EcosimInitialized(m_EcoSimData)
            End If

            ' Update core state
            Me.m_StateMonitor.SetEcoSimLoaded(True)
            ' Let's send out at least one message
            Me.SendEcosimLoadStateMessage(strScenarioName)

            ' JS12May10: EcosimLoaded implies EcosimInitialized
            'Me.m_StateMonitor.SetEcoSimInitialized()

            Return True

        Catch ex As Exception
            m_logger.LogError("LoadEcosimScenario. Error loading Ecosim scenario '{scenarioName}': {message}", strScenarioName, ex.Message)
            Me.SendEcosimLoadStateMessage(strScenarioName, ex.Message)
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Public Sub CloseEcosimScenario()

        Me.CloseEcospaceScenario()
        Me.CloseEcotracerScenario()

        Me.m_EcopathData.ActiveEcosimScenario = -1
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Scenarios are now cleared in CloseModel()
        'this was causing the interface to thow an error when trying to load the list of Sim scenarios
        'possible in response to a StateMonitor Event, but I'm not sure what is causing this chain
        'Need to sort this out as this is where the scenarios should be cleared
        'Me.m_EcoSimScenarios.Clear()
        'Me.m_EcoPathData.NumEcosimScenarios = 0
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Me.m_TSData.ClearTimeSeries()

        ' JS 01Dec10: This method should be bullet-proof. Search data is for instance not 
        '             present if this method is called when no model has been loaded.
        If (Me.m_SearchData IsNot Nothing) Then
            Me.m_SearchData.RedimTime(0)
        End If

        For Each man As cBaseShapeManager In Me.m_ShapeManagers.Values
            man.Clear()
        Next

        Me.ClearSearchManagers()

        Me.m_Ecosim.Clear()
        Me.m_MonteCarlo.Clear()
        Me.m_ArenaManager.Clear()

        Me.ClearIOList(Me.m_EcoSimGroups)
        Me.ClearIOList(Me.m_EcoSimGroupOutputs)
        Me.ClearIOList(Me.m_EcosimFleetInputs)
        Me.ClearIOList(Me.m_EcosimFleetOutputs)

        Me.m_EcoSimGroups.Clear()
        Me.m_EcoSimGroupOutputs.Clear()
        Me.m_EcosimFleetInputs.Clear()
        Me.m_EcosimFleetOutputs.Clear()

        'Need to change m_timeSeriesFleet from (Of cTimeSeries) to (Of cCoreInputOutputBase)
        Me.m_timeSeriesFleet.Clear()
        For Each ts As cTimeSeries In Me.m_timeSeriesGroup
            ts.Clear()
        Next
        Me.m_timeSeriesGroup.Clear()
        ' Forget active time series dataset (fixes issue #863)
        Me.m_TSData.ActiveDatasetIndex = -1

        If Me.m_EcosimOutputs IsNot Nothing Then
            Me.m_EcosimOutputs.Clear()
        End If

        If Me.m_EcosimStats IsNot Nothing Then
            Me.m_EcosimStats.Clear()
            Me.m_EcosimStats = Nothing
        End If

        ' Set the state monitor can fire events that use the Ecosim and Ecospace data
        Me.m_StateMonitor.SetEcoSimLoaded(False)
        m_logger.LogInformation("Ecosim scenario closed")

        Me.m_EcosimEnviroResponseManager.Clear()
        Me.m_EcosimMortalityResponseManager.Clear()


        ' Invoke plugin point to allow plug-ins to clean up now Ecosim has gone
        If (Me.PluginManager IsNot Nothing) Then
            Me.PluginManager.EcosimRunInvalidated()
            Me.PluginManager.EcosimClosedTimeSeries()
            Me.PluginManager.EcosimCloseScenario()
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the current Ecosim scenario.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcosimScenario() As Boolean

        Dim iScenarioID As Integer = 0
        Dim ds As IEcosimDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcosimDatasource) Then Return False

        ' Overwrite scenario?
        iScenarioID = Me.m_EcopathData.EcosimScenarioDBID(Me.m_EcopathData.ActiveEcosimScenario)
        Debug.Assert(iScenarioID > 0)

        ds = DirectCast(DataSource, IEcosimDatasource)
        ' No need to save? Yippee
        If Not ds.IsEcosimModified Then Return True
        ' Save ok?
        If (ds.SaveEcosimScenario(iScenarioID)) Then
            ' Reload ecosim scenarios
            Me.InitEcosimScenarios()
            ' Update active scenario ID
            Me.m_EcopathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcopathData.EcosimScenarioDBID, iScenarioID)
            ' #Yes: invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcosimScenario(Me)
            ' Force update
            Me.m_StateMonitor.SetEcoSimLoaded(True, TriState.True)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Report succes
            Me.SendEcosimSaveStateMessage(Me.m_EcopathData.EcosimScenarioName(Me.m_EcopathData.ActiveEcosimScenario))
            Return True
        End If

        ' Restore active scenario ID
        Me.m_EcopathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcopathData.EcosimScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcosimSaveStateMessage(Me.m_EcopathData.EcosimScenarioName(Me.m_EcopathData.ActiveEcosimScenario), False,
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
    Public Function SaveEcosimScenarioAs(strName As String, strDescription As String) As Boolean

        Dim epd As cEcopathDataStructures = Me.m_EcopathData
        Dim iScenarioID As Integer = 0
        Dim ds As IEcosimDatasource = Nothing
        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcosimDatasource) Then Return bSucces
        If (Me.ActiveEcosimScenarioIndex <= 0) Then Return bSucces

        ' Clear duplicates
        Me.RemoveEcosimScenario(Me.FindObjectByName(Me.m_EcoSimScenarios, strName))

        ds = DirectCast(DataSource, IEcosimDatasource)
        ' Save ok?
        If ds.SaveEcosimScenarioAs(strName, strDescription,
                epd.EcosimScenarioAuthor(Me.m_EcopathData.ActiveEcosimScenario),
                epd.EcosimScenarioContact(Me.m_EcopathData.ActiveEcosimScenario),
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
            bSucces = Me.LoadEcosimScenario(Array.IndexOf(Me.m_EcopathData.EcosimScenarioDBID, iScenarioID))
            ' Changed no. of scenarios
            Me.DataAddedOrRemovedMessage("Ecosim number of scenarios has changed.", eCoreComponentType.Ecosim, eDataTypes.EcoSimScenario)
            ' Report succes
            Return bSucces
        End If

        ' Restore active scenario ID
        Me.m_EcopathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcopathData.EcosimScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcosimSaveStateMessage(strName, bSucces)
        Return bSucces

    End Function

    ''' <summary>
    ''' Remove a <see cref="cEcoSimScenario">Ecosim Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcoSimScenario">Scenario</see> to remove.</param>
    ''' <returns>True if successful.</returns>
    Public Function RemoveEcosimScenario(scenario As cCoreInputOutputBase) As Boolean
        If (scenario Is Nothing) Then Return True
        If (Not TypeOf (scenario) Is cEcoSimScenario) Then Return False
        Return Me.RemoveEcosimScenario(scenario.Index)
    End Function

    ''' <summary>
    ''' Remove a <see cref="cEcoSimScenario">Ecosim Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario in the <see cref="m_EcoSimScenarios">Scenario list</see>.</param>
    ''' <returns>True if successful.</returns>
    Public Function RemoveEcosimScenario(iScenario As Integer) As Boolean

        Dim ds As IEcosimDatasource = Nothing
        Dim iScenarioIDDeleted As Integer = Me.m_EcopathData.EcosimScenarioDBID(iScenario)
        Dim iScenarioID As Integer = cCore.NULL_VALUE ' Scenario to restore
        Dim bSucces As Boolean = False

        ' Sanity check
        Debug.Assert(iScenario > 0 And iScenario < Me.m_EcopathData.EcosimScenarioDBID.Length)

        If (iScenario = Me.m_EcopathData.ActiveEcosimScenario) Then
            Me.m_publisher.SendMessage(New cMessage(My.Resources.CoreMessages.SCENARIO_DELETE_LOADED,
                                                    eMessageType.NotSet, eCoreComponentType.DataSource,
                                                    eMessageImportance.Warning))
            Return False
        End If

        ' Save pending changes
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecosim) Then Return False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcosimDatasource) Then Return False

        ' Remember scenario ID to restore
        If (Me.m_EcopathData.ActiveEcosimScenario > 0) Then
            iScenarioID = Me.m_EcopathData.EcosimScenarioDBID(Me.m_EcopathData.ActiveEcosimScenario)
        End If

        ds = DirectCast(Me.DataSource, IEcosimDatasource)
        ' Scenario removed succesfully?
        If ds.RemoveEcosimScenario(iScenarioIDDeleted) Then
            ' #Yes: reload scenario list
            bSucces = Me.InitEcosimScenarios()
            ' Restore active scenario ID
            Me.m_EcopathData.ActiveEcosimScenario = Array.IndexOf(Me.m_EcopathData.EcosimScenarioDBID, iScenarioID)

            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.EcosimScenarioRemoved(Me.DataSource, iScenarioIDDeleted)
            End If

            ' Broadcast change
            Me.DataAddedOrRemovedMessage("Ecosim number of scenarios has changed.", eCoreComponentType.Ecosim, eDataTypes.EcoSimScenario)
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
            m_EcoSimGroups.Add(New cEcosimGroupInput(Me, m_EcoSimData.GroupDBID(i)))
        Next i

        'now load the Ecosim data into the objects created above
        LoadEcosimGroups()

    End Function

    Friend Function LoadEcosimGroups() As Boolean
        Dim iGroup As Integer
        Dim iPred As Integer

        For Each group As cEcosimGroupInput In m_EcoSimGroups

            'convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcoSimData.GroupDBID, group.DBID)

            'this will only resize the arrays if NumGroups is different then the existing array size
            group.Resize()

            group.AllowValidation = False

            group.Index = iGroup

            'get the group name from EcoPath not EcoSim
            group.Name = m_EcopathData.GroupName(iGroup)
            'Primary Production also comes from EcoPath
            group.PP = m_EcopathData.PP(iGroup)

            group.MaxRelPB = m_EcoSimData.PBmaxs(iGroup)
            group.MaxRelFeedingTime = m_EcoSimData.FtimeMax(iGroup)
            group.FeedingTimeAdjustRate = m_EcoSimData.FtimeAdjust(iGroup)
            group.OtherMortFeedingTime = m_EcoSimData.MoPred(iGroup)
            group.PredEffectFeedingTime = m_EcoSimData.RiskTime(iGroup)
            group.DenDepCatchability = m_EcoSimData.QmQo(iGroup)
            group.QBMaxQBio = m_EcoSimData.CmCo(iGroup)
            group.SwitchingPower = m_EcoSimData.SwitchPower(iGroup)
            group.PP = m_EcopathData.PP(iGroup)
            group.AdditivePredationMortality = m_EcoSimData.PaddP(iGroup)

            Try
                For iPred = 1 To nGroups

                    group.VulMult(iPred) = m_EcoSimData.VulMult(iGroup, iPred)

                    If m_EcoSimData.SimDC(iPred, iGroup) > 0 Or (iGroup = iPred And m_EcopathData.PP(iPred) = 1) Then
                        group.VulMultiStatus(iPred) = eStatusFlags.OK
                        'group.VulRateStatus(iPred) = eStatusFlags.OK
                    Else
                        group.VulMultiStatus(iPred) = eStatusFlags.NotEditable Or eStatusFlags.Null
                        'group.VulRateStatus(iPred) = eStatusFlags.NotEditable Or eStatusFlags.Null
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
    ''' Get a <see cref="cEcosimGroupInput">Ecosim input group</see> for a given group index.
    ''' </summary>
    ''' <param name="iGroup">The index to obtain the group for.</param>
    Public ReadOnly Property EcosimGroupInputs(iGroup As Integer) As cEcosimGroupInput
        Get
            'test that EcoSim has been initialized
            If Not m_bEcoSimIsInit Then
                Debug.Assert(False, "EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
                m_logger.LogError("EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
                Return Nothing
            End If
            ' JS 06Jul07: list will take care of scenario index/item index offset
            Return DirectCast(m_EcoSimGroups(iGroup), cEcosimGroupInput)
        End Get
    End Property

    ''' <summary>
    ''' Get a <see cref="cEcoSimGroupOutput">Ecosim output group</see> for a given group index.
    ''' </summary>
    ''' <param name="iGroup">The index to obtain the group for.</param>
    Public ReadOnly Property EcosimGroupOutputs(iGroup As Integer) As cEcosimGroupOutput
        Get

            Try
                If m_EcoSimGroupOutputs IsNot Nothing Then
                    If m_EcoSimGroupOutputs.Count > 0 Then
                        Return DirectCast(m_EcoSimGroupOutputs(iGroup), cEcosimGroupOutput)
                    End If
                End If
                Return Nothing
            Catch ex As Exception
                m_logger.LogError("EcosimGroupOutputs. Error getting Ecosim Group Output for group index {groupIndex}: {message}", iGroup, ex.Message)
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
                    fleet.Name = m_EcopathData.FleetName(iFlt)
                End If

                m_EcoSimData.getSummaryBioOfCatch(iFlt, sCatch, EndCatch)
                fleet.CatchStart = sCatch
                fleet.CatchEnd = EndCatch

                m_EcoSimData.getSummaryValueOfCatch(iFlt, sVal, endVal)
                fleet.ValueStart = sVal
                fleet.ValueEnd = endVal

                'see EwE5 CalculateSimSpaceResults
                m_EcoSimData.getSummaryCostByCatch(iFlt, sVal, endVal)
                fleet.CostStart = sVal * (m_EcopathData.cost(iFlt, eCostIndex.CUPE) + m_EcopathData.cost(iFlt, eCostIndex.Sail)) + m_EcopathData.cost(iFlt, eCostIndex.Fixed)
                fleet.CostEnd = endVal * (m_EcopathData.cost(iFlt, eCostIndex.CUPE) + m_EcopathData.cost(iFlt, eCostIndex.Sail)) + m_EcopathData.cost(iFlt, eCostIndex.Fixed)

                'If there is forced catches for any group caught by this fleet then Cost is not valid.
                'Cost is calculated from Ecopath input values and Effort Not Catch. 
                'If catch is forced we have no way of knowing what effort created the catch so no cost.
                Dim bCatchTS As Boolean = False
                For igrp As Integer = 1 To Me.nGroups
                    If Me.m_EcopathData.Landing(iFlt, igrp) > 0 Then
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
            m_logger.LogError("LoadEcosimFleetOutputs. Error loading Ecosim Fleet Summary data: {message}", ex.Message)
            m_publisher.AddMessage(New cMessage("Error loading Ecosim Summary data. " & ex.Message, eMessageType.ErrorEncountered,
                                    eCoreComponentType.Ecosim, eMessageImportance.Critical))
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
            m_logger.LogError("LoadEcosimStats. Error loading Ecosim Stats: {message}", ex.Message)
            Throw New ArgumentException(Me.ToString & ".LoadEcosimStats() Error: " & ex.Message, ex)
        End Try
    End Sub

    Friend Sub LoadEcosimOutputs()
        Try
            ' Hah, nothing to do here
        Catch ex As Exception
            m_logger.LogError("LoadEcosimOutputs. Error loading Ecosim Outputs: {message}", ex.Message)
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
            group.Name = m_EcopathData.GroupName(iGroup)

            'stanza variables setting the stanza id will also set the isMultiStanza Flag
            group.iStanza = getStanzaIndexForGroup(iGroup)
            group.PP = m_EcopathData.PP(iGroup)

            'Biomass
            m_EcoSimData.getSummaryBioForGroup(iGroup, sBio, EndBio)
            group.BiomassStart = sBio
            group.BiomassEnd = EndBio
            group.PP = m_EcopathData.PP(iGroup)
            group.isCatchAggregated = Me.m_EcoSimData.FisForced(iGroup)

            'catch by group
            For iFlt As Integer = 0 To nFleets 'Zero is the combined fleets 
                m_EcoSimData.getSummaryCatchByGroup(iGroup, iFlt, sCatch, EndCatch)
                group.CatchStart(iFlt) = sCatch
                group.CatchEnd(iFlt) = EndCatch

                m_EcoSimData.getSummaryValueByGroup(iGroup, iFlt, sVal, endVal)
                group.ValueStart(iFlt) = sVal
                group.ValueEnd(iFlt) = endVal
            Next

            group.ResetStatusFlags()

        Next

    End Function

    Public ReadOnly Property MediatedInteractionManager() As cMediatedInteractionManager
        Get
            Return Me.m_MediatedInteractionManager
        End Get
    End Property

    Public ReadOnly Property ForcingShapeManager() As cForcingFunctionShapeManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.Forcing), cForcingFunctionShapeManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find Shape Manager")
                m_logger.LogError("ForcingShapeManager. Failed to find Shape Manager: {message}", ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property EggProdShapeManager() As cEggProductionShapeManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.EggProd), cEggProductionShapeManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find Shape Manager")
                m_logger.LogError("EggProdShapeManager. Failed to find Shape Manager: {message}", ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property MediationShapeManager() As cMediationShapeManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.Mediation), cMediationShapeManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find Shape Manager")
                m_logger.LogError("MediationShapeManager. Failed to find Shape Manager: {message}", ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property FishingEffortShapeManager() As cFishingEffortShapeManger

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.FishingEffort), cFishingEffortShapeManger)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find effort shape manager")
                m_logger.LogError("FishingEffortShapeManager. Failed to find effort shape manager: {message}", ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property FishMortShapeManager() As cFishingMortalityShapeManger

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.FishMort), cFishingMortalityShapeManger)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find mortality shape manager")
                m_logger.LogError("FishMortShapeManager. Failed to find mortality shape manager: {message}", ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property LandingsShapeManager() As cLandingsMediationShapeManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.PriceMediation), cLandingsMediationShapeManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find price elasticity shape manager")
                m_logger.LogError("LandingsShapeManager. Failed to find price elasticity shape manager: {message}", ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    Public ReadOnly Property CapacityMapInteractionManager() As cEcospaceEnviroResponseManager
        Get
            Return Me.m_mapInteractionManager
        End Get
    End Property

    Public ReadOnly Property MortalityMapInteractionManager() As cEcospaceMortalityResponseManager
        Get
            Return Me.m_mapMortalityManager
        End Get
    End Property


    Public ReadOnly Property EcosimArenaManager() As cEcosimArenaManager
        Get
            Return Me.m_ArenaManager
        End Get
    End Property

    Public ReadOnly Property EcosimEnviroResponseManager() As cEcosimEnviroResponseManager
        Get
            Return Me.m_EcosimEnviroResponseManager
        End Get
    End Property

    Public ReadOnly Property EcosimMortalityResponseManager() As cEcosimMortalityResponseManager
        Get
            Return Me.m_EcosimMortalityResponseManager
        End Get
    End Property



    Public ReadOnly Property EnviroResponseShapeManager() As cEnviroResponseShapeManager

        Get
            Try
                Return DirectCast(m_ShapeManagers.Item(eDataTypes.CapacityMediation), cEnviroResponseShapeManager)
            Catch ex As Exception
                Debug.Assert(False, "Failed to find price elasticity shape manager")
                m_logger.LogError("EnviroResponseShapeManager. Failed to find price elasticity shape manager: {message}", ex.Message)
                Return Nothing
            End Try

        End Get

    End Property


    'Public ReadOnly Property EcosimEnviroResponseShapeManager() As cEcosimResponseShapeManager

    '    Get
    '        Try
    '            Return DirectCast(m_ShapeManagers.Item(eDataTypes.EcosimEnviroResponseFunctionManager), cEcosimResponseShapeManager)
    '        Catch ex As Exception
    '            Debug.Assert(False, "Failed to find Ecosim Environmental Response shape manager")
    '            m_logger.LogError(ex, ".EcosimEnviroResponseShapeManager() Error: " & ex.Message)
    '            Return Nothing
    '        End Try

    '    End Get

    'End Property



    ''' <summary>
    ''' Update all the underlying data structures that contain EcoSim scenario data
    ''' </summary>
    ''' <returns>True if successful.</returns>
    Private Function UpdateEcoSimScenario(iDBID As Integer) As Boolean

        Dim iScenario As Integer = Array.IndexOf(Me.m_EcopathData.EcosimScenarioDBID, iDBID)
        Dim scn As cEcoSimScenario = Me.EcosimScenarios(iScenario)

        Try
            Me.m_EcopathData.EcosimScenarioName(iScenario) = scn.Name
            Me.m_EcopathData.EcosimScenarioDescription(iScenario) = scn.Description
            Me.m_EcopathData.EcosimScenarioAuthor(iScenario) = scn.Author
            Me.m_EcopathData.EcosimScenarioContact(iScenario) = scn.Contact
            ' Do not update last saved date; this is exclusively set by the core when saving
            'Me.m_EcoPathData.EcosimScenarioLastSaved(iScenario) = scn.LastSaved

        Catch ex As Exception
            m_logger.LogError("UpdateEcoSimScenario. Error updating EcoSim scenario data: {message}", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Update all the underlying data structures that contain group info for EcoSim
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function UpdateEcoSimGroup(iDBID As Integer) As Boolean

        Dim iGroup As Integer = Array.IndexOf(m_EcoSimData.GroupDBID, iDBID)
        Dim group As cEcosimGroupInput = Me.EcosimGroupInputs(iGroup)

        Try
            m_EcoSimData.QmQo(iGroup) = group.DenDepCatchability
            m_EcoSimData.FtimeAdjust(iGroup) = group.FeedingTimeAdjustRate
            m_EcoSimData.FtimeMax(iGroup) = group.MaxRelFeedingTime
            m_EcoSimData.PBmaxs(iGroup) = group.MaxRelPB
            m_EcoSimData.MoPred(iGroup) = group.OtherMortFeedingTime
            m_EcoSimData.RiskTime(iGroup) = group.PredEffectFeedingTime
            m_EcoSimData.CmCo(iGroup) = group.QBMaxQBio
            m_EcoSimData.SwitchPower(iGroup) = group.SwitchingPower
            m_EcoSimData.PaddP(iGroup) = group.AdditivePredationMortality


            For iPred As Integer = 1 To nGroups
                ' m_EcoSimData.vulrate(iGroup, i) = grp.VulRate(i)
                m_EcoSimData.VulMult(iGroup, iPred) = group.VulMult(iPred)
            Next

        Catch ex As Exception
            m_logger.LogError("UpdateEcoSimGroup. Error updating EcoSim group data: {message}", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Update all the underlying data structures that contain fleet input info for EcoSim
    ''' </summary>
    ''' <param name="iDBID">Database ID of Ecosim fleet to update</param>
    Private Function UpdateEcoSimFleetInput(iDBID As Integer) As Boolean

        Dim iFleet As Integer = Array.IndexOf(m_EcoSimData.FleetDBID, iDBID)
        Dim fleet As cEcosimFleetInput = Me.EcosimFleetInputs(iFleet)

        Try
            Me.m_EcoSimData.Epower(iFleet) = fleet.EPower
            Me.m_EcoSimData.PcapBase(iFleet) = fleet.PcapBase
            Me.m_EcoSimData.CapBaseGrowth(iFleet) = fleet.CapBaseGrowth
            Me.m_EcoSimData.CapDepreciate(iFleet) = fleet.CapDepreciateRate
            Me.m_EcoSimData.EffortConversionFactor(iFleet) = fleet.EffortConversionFactor

            ' JS 06Apr22: Move to Catchabilities time series logic
            'For igrp As Integer = 1 To nGroups
            '    If (Me.m_EcopathData.Landing(iFleet, igrp) + Me.m_EcopathData.Discard(iFleet, igrp)) > 0 Then

            '        For it As Integer = 1 To nEcosimTimeSteps
            '            Me.m_EcoSimData.relQt(iFleet, igrp, it) = fleet.RelQt(igrp, it)
            '        Next
            '    End If
            'Next
            'Dim arrayValue As cValueArrayTripleIndex = DirectCast(value, cValueArrayTripleIndex)
            'Dim QYear() As Single = New Single(Me.nFleets) {}
            'For i As Integer = 1 To Me.m_EcopathData.NumFleet
            '    QYear(i) = 1
            'Next
            ''For it As Integer = 1 To Me.nEcosimTimeSteps
            'Me.m_Ecosim.SetFtimeFromGear(arrayValue.iThirdIndex, QYear, True)
            'Next


        Catch ex As Exception
            m_logger.LogError("UpdateEcoSimFleetInput. Error updating EcoSim fleet data: {message}", ex.Message)
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
                Debug.Assert(iFleet > 0 And iFleet <= m_EcopathData.NumFleet, "Failed to find Fleet index for database ID " & fleet.DBID.ToString)

                fleet.Name = Me.m_EcopathData.FleetName(iFleet)
                fleet.EPower = m_EcoSimData.Epower(iFleet)
                fleet.PcapBase = m_EcoSimData.PcapBase(iFleet)
                fleet.CapDepreciateRate = m_EcoSimData.CapDepreciate(iFleet)
                fleet.CapBaseGrowth = m_EcoSimData.CapBaseGrowth(iFleet)

                fleet.EffortConversionFactor = m_EcoSimData.EffortConversionFactor(iFleet)

                For iGroup As Integer = 1 To Me.nGroups
                    If (Me.EcopathDataStructures.Landing(iFleet, iGroup) + Me.EcopathDataStructures.Discard(iFleet, iGroup) > 0) Then
                        fleet.RelQ(iGroup) = m_EcoSimData.relQ(iFleet, iGroup)
                        fleet.RelQStatus(iGroup) = eStatusFlags.NotEditable Or eStatusFlags.OK
                    Else
                        fleet.RelQStatus(iGroup) = eStatusFlags.NotEditable Or eStatusFlags.Null
                    End If
                Next

                fleet.ResetStatusFlags()
                fleet.AllowValidation = True
            Next

        Catch ex As Exception

            m_logger.LogError("LoadEcosimFleetInputs. Error loading Ecosim fleet data: {message}", ex.Message)
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
            If Not m_Ecosim Is Nothing Then
                m_Ecosim.bStopRunning = True
            End If
        Catch ex As Exception
            m_logger.LogError("StopEcoSim. Error stopping EcoSim model: {message}", ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Change the number of years the ecosim model runs for
    ''' </summary>
    ''' <param name="newNumberOfYears"></param>
    ''' <remarks>There are two events that can trigger this. User has set the Ecosim run length. User has loaded timeseries data which will set the Ecosim run length to the same as the timeseries data</remarks>
    Private Sub setEcosimRunLength(newNumberOfYears As Integer, Optional bOverwriteNewData As Boolean = True)
        'newNumberOfYears has already passed validation
        'set the number of years the model will run for and resize and reload all the data

        Try
            'number of years before setting to the new value
            Dim orgNYears As Integer = m_EcoSimData.NumYears

            If newNumberOfYears = 0 Then Exit Sub
            'sets NumYears and NTimes and resize the underlying data to the new number of years
            m_EcoSimData.redimTime(newNumberOfYears, m_TSData.nYears, bOverwriteNewData)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'OK Super awkward 
            'ONLY resize relQt(fleet,group,time) for time dimensions great than the array already contains
            'this sort of has to happen in the core because only it knows all the information need to do this
            Dim nsteps As Integer = m_EcoSimData.relQt.GetUpperBound(2) 'time dimension
            If nsteps < m_EcoSimData.NTimes Then
                ReDim Preserve m_EcoSimData.relQt(Me.nFleets, Me.nGroups, m_EcoSimData.NTimes)
                Dim q As Single
                For iFlt As Integer = 0 To Me.nFleets
                    For iGrp As Integer = 0 To nGroups
                        If (Me.m_EcopathData.Landing(iFlt, iGrp) + Me.m_EcopathData.Discard(iFlt, iGrp)) > 0 Then
                            q = (Me.m_EcopathData.Landing(iFlt, iGrp) + Me.m_EcopathData.Discard(iFlt, iGrp)) / Me.m_EcopathData.B(iGrp)
                        Else
                            q = cCore.NULL_VALUE
                        End If
                        For t As Integer = nsteps + 1 To m_EcoSimData.NTimes
                            m_EcoSimData.relQt(iFlt, iGrp, t) = q
                        Next t
                    Next iGrp
                Next iFlt
            End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'redim the cv vars to the new timesteps
            'The MSE will use Ecosim.NumYears for the new run length
            'So pass in the original number of years so it can figure out what to do
            Me.m_MSEData.redimTime(orgNYears)

            'Reload the forcing data PoolForceBB(), PoolForceZ(), PoolForceCatch() and FishRateGear(), FishRateNo
            'forcing data needs to be the max of Reference data years and Ecosim Years
            Me.m_TSData.LoadForcingData()

            Me.m_SearchData.RedimTime(m_EcoSimData.NumYears)

            Me.m_EcospaceData.TotalTime = m_EcoSimData.NumYears
            'changed the run length for ecospace reset the summary periods to defaults
            Me.m_EcospaceData.setDefaultSummaryPeriod()

            If Me.m_FitToTimeSeriesData.LastYear = 1 Then Me.m_FitToTimeSeriesData.LastYear = Integer.MaxValue
            Me.m_FitToTimeSeriesData.LastYear = Math.Max(1, Math.Min(Me.m_FitToTimeSeriesData.LastYear, m_EcoSimData.NumYears))

            'Now Update the interface objects
            Me.InitEcosimFleetInput()

            'tell the affected shape managers that there data has changed
            Dim manager As cBaseShapeManager
            manager = m_ShapeManagers.Item(eDataTypes.FishMort)
            manager.Load()
            manager = m_ShapeManagers.Item(eDataTypes.FishingEffort)
            manager.Load()

            Me.m_SearchManagers(eDataTypes.FishingPolicyManager).Load()
            Me.m_SearchManagers(eDataTypes.MSEManager).Load()

            Me.m_ShapeManagers(eDataTypes.Forcing).Load()
            Me.m_ShapeManagers(eDataTypes.EggProd).Load()

            'Parameters
            Me.LoadEcosimModelParameters()

            Me.m_publisher.AddMessage(New cMessage("Ecosim number of years has changed.", eMessageType.EcosimNYearsChanged,
                                                     eCoreComponentType.Ecosim, eMessageImportance.Maintenance))

            Me.m_publisher.AddMessage(New cMessage("Ecosim number of years has changed.", eMessageType.DataModified,
                                                     eCoreComponentType.ShapesManager, eMessageImportance.Maintenance))

        Catch ex As Exception
            m_logger.LogError("setEcosimRunLength. Error changing number of Ecosim years: {message}", ex.Message)
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
    Public Function RunEcoSimOnThread(SynchronizingObject As System.ComponentModel.ISynchronizeInvoke,
                                        ProgressDelegate As EcoSim.EcoSimTimeStepDelegate,
                                        CompletedDelegate As EcoSim.EcoSimCompletedDelegate) As Boolean


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
                    m_logger.LogError(".RunEcoSimOnThread(SyncObject,Delegate) EcoSim model is already running. Please wait to start another model.")
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
            m_logger.LogError(ex, ".RunEcoSimOnThread(SyncObject,Delegate)Error: " & ex.Message)
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

        '    m_logger.LogError(ex, ".RunEcoSim(...) Error: " & ex.Message)
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
    Private Sub EcoSimProgressMultiThread_handler(iTime As Long, results As cEcoSimResults)
        Dim args(1) As Object

        Try

            'set-up the arguments that get passed to the interface delegate
            'arguments get passed to the Syncronization.invoke(...) method as an array of Objects indexed from zero to n-1
            'they must be in the same order as they appear in the declaration of the delegate
            'EcoSimTimeStepDelegate(iTime As Long, data As cEcoSimResults)
            args(0) = iTime
            args(1) = results

            'call the sync object with the user interface delegate, this is where delegate will run
            'the Syncronization object handles the marshalling of the data between this thread and the GUI object i.e. a Windows form or textbox
            mSynEcoSim.Invoke(m_InterfaceDelegate, args)

        Catch ex As Exception
            m_logger.LogError(ex, ".EcoSimProgress_handler() Delegate has thrown an Error: " & ex.Message)

        End Try


    End Sub


#End If

#End Region

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Launch an Ecosim run.
    ''' </summary>
    ''' <param name="TimeStepDelegate">Delegate for receiving time step notifications.</param>
    ''' <param name="bMultiThreaded">Multi-threaded run flag. Please use with care.</param>
    ''' <returns>True if a run started successfully.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RunEcosim(Optional TimeStepDelegate As Ecosim.EcoSimTimeStepDelegate = Nothing,
                              Optional bMultiThreaded As Boolean = False) As Boolean
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
            If Not RunEcopath() Then

                m_logger.LogInformation("RunEcoSim. Failed to run EcoPath before running EcoSim.")

                'EcoPath is supposed to have sent a message if it failed
                msg = New cMessage("Ecosim could not be run because Ecopath failed to balance the model.", eMessageType.ErrorEncountered,
                                            eCoreComponentType.Ecosim, eMessageImportance.Critical, eDataTypes.NotSet)
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
            m_Ecosim.Init(Me.m_StateMonitor.RequiresEcosimFullInit)

            'now we need to load any changes to the ecosim data that was made by init
            'into the objects used by the interface
            LoadEcosimGroups()
            LoadEcosimModelParameters()
            LoadStanzas()
            'Ecopath should have sent out its own message 
            'so we should only need to send a message for Ecosim
            msg = New cMessage("Ecosim has re-run Ecopath and initialized its data.", eMessageType.DataModified,
                                        eCoreComponentType.Ecosim, eMessageImportance.Maintenance, eDataTypes.NotSet)
            m_publisher.SendMessage(msg)
        End If

        ' Update core state monitor
        Me.m_StateMonitor.SetEcosimRun()

        Me.m_Ecosim.TimeStepDelegate = TimeStepDelegate

        'if Ecosim is being run on a thread then setup the RunCompletedDelegate
        'this will call  Me.EcoSimRunCompleted(Nothing) once Ecosim has completed the run
        Me.m_Ecosim.RunCompletedDelegate = Nothing
        Me.m_EcoSimData.bMultiThreaded = bMultiThreaded

        If Me.m_EcoSimData.bMultiThreaded Then
            Me.m_Ecosim.RunCompletedDelegate = AddressOf Me.onEcoSimRunCompleted
            Me.SetStopRunDelegate(New StopRunDelegate(AddressOf StopEcoSim))
        End If

        'make sure all the searches are turned off
        Me.m_Ecosim.setSearchOff()
        Me.ResetEcosimGroupOutputs()
        Me.m_Ecosim.bStopRunning = False

        m_Ecosim.Run()

        'if not mulithreaded then the Ecosim run has completed 
        'do any processing to complete the run (populate objects, send any messages...)
        'if running in on a thread then Me.EcoSimRunCompleted(Nothing) will be called via the delegate set before the run
        If Not Me.m_EcoSimData.bMultiThreaded Then
            Me.onEcoSimRunCompleted(Nothing)
        End If

        Return True

    End Function

    Private Sub onEcoSimRunCompleted(obj As Object)
        Try

            Me.m_TSData.Update()

            LoadEcosimGroupOutputs()
            LoadEcosimFleetOutputs()

            ' JS 03Feb2021: Changed statement below to only load time series statistics.
            ' The previous call to reload time series all the way messed up plug-ins such as Stepwise Fitting that lose their content in response
            LoadEcosimTimeSeriesStats()

            LoadEcosimOutputs()
            LoadEcosimStats()
            loadEcotracerResults()

            If m_EcoSimData.PredictSimEffort Or Me.m_StateMonitor.RequiresEcosimFullInit Then
                'if effort was predicted then reload the shapes
                m_ShapeManagers.Item(eDataTypes.FishMort).Load()
                m_ShapeManagers.Item(eDataTypes.FishingEffort).Load()

                'tell the interface that the shapes have changed
                Me.m_publisher.AddMessage(New cMessage("Fish rate shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishingEffort))
                Me.m_publisher.AddMessage(New cMessage("Fish mort shape modified", eMessageType.DataModified, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishMort))

            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".EcoSimRunCompleted() Exception: " & ex.Message)
        End Try

        Try

            'make sure ecosim can start again
            m_Ecosim.bStopRunning = False

            m_publisher.AddMessage(New cMessage("Ecosim run completed.", eMessageType.EcosimRunCompleted,
                                            eCoreComponentType.Ecosim, eMessageImportance.Maintenance, eDataTypes.NotSet))

            Me.m_Ecosim.TimeStepDelegate = Nothing
            Me.m_Ecosim.RunCompletedDelegate = Nothing

            ' Update core state monitor
            Me.m_StateMonitor.SetEcosimCompleted(Me.m_tracerData.EcoSimConSimOn)
            ' Send messages after
            m_publisher.sendAllMessages()

            ' -------
            ' Write results if needed
            If Me.Autosave(eAutosaveTypes.EcosimResults) Then
                Dim writer As New cEcosimResultWriter(Me)
                writer.WriteResults()
            End If

            If (Me.Autosave(eAutosaveTypes.Ecotracer) And Me.m_tracerData.EcoSimConSimOn) Then
                Dim writer As New cEcotracerResultWriter(Me)
                writer.WriteEcosimResults()
            End If
            ' -------

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
    Private Property privateEcoSimScenario(iScenario As Integer) As cEcoSimScenario

        Get
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcopathData.EcosimScenarioName.Length Then
                    m_logger.LogInformation("EcoSimScenario. nScenario out of bounds: {nScenario}", iScenario)
                    Return Nothing
                End If

                Dim infoOut As New cEcoSimScenario(Me)

                infoOut.AllowValidation = False

                infoOut.DBID = m_EcopathData.EcosimScenarioDBID(iScenario)
                infoOut.Name = m_EcopathData.EcosimScenarioName(iScenario)
                infoOut.Description = m_EcopathData.EcosimScenarioDescription(iScenario)
                infoOut.Author = m_EcopathData.EcosimScenarioAuthor(iScenario)
                infoOut.Contact = m_EcopathData.EcosimScenarioContact(iScenario)
                infoOut.LastSaved = m_EcopathData.EcosimScenarioLastSaved(iScenario)
                infoOut.Index = iScenario

                infoOut.ResetStatusFlags()

                infoOut.AllowValidation = True

                Return infoOut

            Catch ex As Exception
                m_logger.LogError("privateEcoSimScenario. Error getting EcoSim scenario info: {message}", ex.Message)
                Debug.Assert(False, "Error Getting EcoSim Scenario Info: " & ex.Message)
                Return Nothing
            End Try

        End Get

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Set(ParametersIn As cEcoSimScenario)

            'Set the parameters in the underlying EcoSim data structures to user supplied values
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcopathData.EcosimScenarioName.Length Then
                    m_logger.LogInformation("privateEcoSimScenario. nScenario out of bounds: {nScenario}", iScenario)
                    Return
                End If

                m_EcopathData.EcosimScenarioName(iScenario) = ParametersIn.Name
                m_EcopathData.EcosimScenarioDescription(iScenario) = ParametersIn.Description
                m_EcopathData.EcosimScenarioAuthor(iScenario) = ParametersIn.Author
                m_EcopathData.EcosimScenarioContact(iScenario) = ParametersIn.Contact
                ' Do not update last saved date; this is exclusively set by the core when saving

            Catch ex As Exception
                m_logger.LogError("privateEcoSimScenario. Error setting EcoSim scenario info: {message}", ex.Message)
                Debug.Assert(False, "EcoSim Scenario Info will not be set Error: " & ex.Message)
            End Try

        End Set

    End Property

    ''' <summary>
    ''' Get the <see cref="cEcoSimModelParameters">Ecosim model parameters</see> for
    ''' the currently loaded Ecosim scenario.
    ''' </summary>
    Public ReadOnly Property EcosimModelParameters() As cEcoSimModelParameters
        Get
            If Not m_bEcoSimIsInit Then
                Debug.Assert(False, "EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
                'MsgBox("EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first", MsgBoxStyle.Critical)
                m_logger.LogInformation("EcoSim must be initialized before you can get or set its Parameters. Call InitEcoSim(...) first")
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
    Private Function InitEcosimModelParameters() As Boolean

        m_EcoSimRun = New cEcoSimModelParameters(Me)

        Return LoadEcosimModelParameters()

    End Function

    ''' <summary>
    ''' Reload the Eocsim data into the existing EcoSim parameter object
    ''' </summary>
    ''' <returns>True if no error encountered.</returns>
    ''' <remarks>This can be used if a new scenario is loaded to populate the existing EcoSim parameter object (m_EcoSimRun) with the new scenario data. </remarks>
    Private Function LoadEcosimModelParameters() As Boolean

        Try
            m_EcoSimRun.AllowValidation = False
            m_EcoSimRun.DBID = m_EcopathData.EcosimScenarioDBID(m_EcopathData.ActiveEcosimScenario)
            m_EcoSimRun.Name = m_EcopathData.EcosimScenarioName(m_EcopathData.ActiveEcosimScenario)
            m_EcoSimRun.BiomassOn = m_Ecosim.m_Data.BiomassOn
            m_EcoSimRun.Discount = m_Ecosim.m_Data.Discount
            m_EcoSimRun.EquilibriumStepSize = m_Ecosim.m_Data.EquilibriumStepSize
            m_EcoSimRun.EquilMaxFishingRate = m_Ecosim.m_Data.EquilScaleMax
            m_EcoSimRun.NudgeChecked = m_Ecosim.m_Data.NudgeChecked
            m_EcoSimRun.NumberYears = m_Ecosim.m_Data.NumYears
            m_EcoSimRun.NutBaseFreeProp = m_Ecosim.m_Data.NutBaseFreeProp
            m_EcoSimRun.NutForceFunctionNumber = m_Ecosim.m_Data.NutForceNumber
            m_EcoSimRun.NutPBMax = m_Ecosim.m_Data.NutPBmax
            m_EcoSimRun.StepSize = m_Ecosim.m_Data.StepSize
            m_EcoSimRun.SystemRecovery = m_Ecosim.m_Data.SystemRecovery
            m_EcoSimRun.UseVarPQ = m_Ecosim.m_Data.UseVarPQ
            m_EcoSimRun.VulnerabilityCap = m_Ecosim.m_Data.VulnerabilityCap
            m_EcoSimRun.ForagingTimeLowerLimit = m_Ecosim.m_Data.ForagingTimeLowerLimit

            m_EcoSimRun.ContaminantTracing = Me.m_tracerData.EcoSimConSimOn
            m_EcoSimRun.PredictEffort = m_Ecosim.m_Data.PredictSimEffort
            m_EcoSimRun.NumberSummaryTimeSteps = m_Ecosim.m_Data.NumStep
            m_EcoSimRun.StartSummaryTime = m_Ecosim.m_Data.SumStart(0)
            m_EcoSimRun.EndSummaryTime = m_Ecosim.m_Data.SumStart(1)

            m_EcoSimRun.SORWt = m_Ecosim.m_Data.SorWt

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

            m_Ecosim.m_Data.BiomassOn = m_EcoSimRun.BiomassOn
            m_Ecosim.m_Data.Discount = m_EcoSimRun.Discount
            m_Ecosim.m_Data.EquilibriumStepSize = m_EcoSimRun.EquilibriumStepSize
            m_Ecosim.m_Data.EquilScaleMax = m_EcoSimRun.EquilMaxFishingRate
            m_Ecosim.m_Data.NudgeChecked = m_EcoSimRun.NudgeChecked
            m_Ecosim.m_Data.NumYears = m_EcoSimRun.NumberYears
            m_Ecosim.m_Data.NutBaseFreeProp = m_EcoSimRun.NutBaseFreeProp
            m_Ecosim.m_Data.NutForceNumber = m_EcoSimRun.NutForceFunctionNumber
            m_Ecosim.m_Data.NutPBmax = m_EcoSimRun.NutPBMax
            m_Ecosim.m_Data.StepSize = m_EcoSimRun.StepSize
            m_Ecosim.m_Data.SystemRecovery = m_EcoSimRun.SystemRecovery
            m_Ecosim.m_Data.UseVarPQ = m_EcoSimRun.UseVarPQ
            m_Ecosim.m_Data.VulnerabilityCap = m_EcoSimRun.VulnerabilityCap
            m_Ecosim.m_Data.ForagingTimeLowerLimit = m_EcoSimRun.ForagingTimeLowerLimit

            m_tracerData.EcoSimConSimOn = m_EcoSimRun.ContaminantTracing

            m_Ecosim.m_Data.PredictSimEffort = m_EcoSimRun.PredictEffort

            m_Ecosim.m_Data.NumStep = m_EcoSimRun.NumberSummaryTimeSteps
            m_Ecosim.m_Data.SumStart(0) = m_EcoSimRun.StartSummaryTime
            m_Ecosim.m_Data.SumStart(1) = m_EcoSimRun.EndSummaryTime

            m_Ecosim.m_Data.SorWt = m_EcoSimRun.SORWt

        Catch ex As Exception
            m_logger.LogError("UpdateEcoSimModelParameters. EcoSim Parameters will not be set: {message}", ex.Message)
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

    '    'ToDo_jb Core.UpdateTimeVariables needs to tell the data source to update time variables
    '    'DataSource.UpdateTime()

    '    'jb Tell Ecosim to redim time variables 
    '    'I think this need to be handled by the core and not the data source because only it knows about Ecosim's needs
    '    m_EcoSim.ReSetTime()

    '    LoadEcosimGroups() 'may not need to load the groups
    '    LoadEcoSimModelParameters()

    '    'ToDo_jb UpdateTimeVariables need to load all the Shapes that are dimmed by time
    '    'size of the time and eggproduction shapes has changed
    '    '   OnShapeEdited(eDataTypes.Forcing)

    '    m_publisher.SendMessage(New cMessage("Groups have been updated.",
    '                    eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.EcoSimGroupInput))
    '    m_publisher.SendMessage(New cMessage("Model parameters have been updated.",
    '            eMessageType.DataModified, eCoreComponentType.EcoSim, eMessageImportance.Maintenance, eDataTypes.EcoSimModelParameter))

    '    'the data changed message has to be sent be the core instead of the shapemanagers 
    '    'because the message refers to all the shape managers not just a single one so all the data has to be loaded before the message can be sent
    '    'm_publisher.SendMessage(New cMessage("Forcing Shapes have been updated.",
    '    '                        eMessageType.DataChanged, eCoreComponentType.ShapesManager, eMessageImportance.Warning, eDataTypes.Shape))

    'End Function



#If 0 Then

    ''' <summary>
    ''' Dump the values from the last model run into a file that has the same format as used by EwE5 file dump from the Plot interface
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>The resulting file can be used to compare results with EwE5 </remarks>
    Public Function dumpEcosimModelResults(fileName As String) As Boolean
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
            strm.Write(Environment.NewLine )

            'data Groups in columns 
            'Time in rows
            For it As Integer = 1 To m_EcoSimData.NTimes
                For igrp = 1 To m_EcoPathData.NumGroups
                    strm.Write(Me.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, it).ToString)
                    If igrp < m_EcoPathData.NumGroups Then strm.Write(delimiter)
                Next igrp
                strm.Write(Environment.NewLine )
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
    Public Function CheckResetDefaultVulnerabilities(Optional bQuiet As Boolean = False,
                                                     Optional sDefaultValue As Single = 2.0!) As Boolean
        Dim fmsg As cFeedbackMessage = Nothing

        Try

            If Not Me.HasNonDefaultVulnerabilty(sDefaultValue) Then Return True

            If Not bQuiet Then
                fmsg = New cFeedbackMessage(cStringUtils.Localize(My.Resources.CoreMessages.VULNERABILITIES_PROMPT_RESET, sDefaultValue),
                                            eCoreComponentType.Ecosim, eMessageType.Any,
                                            eMessageImportance.Information,
                                            eMessageReplyStyle.YES_NO, eDataTypes.NotSet, eMessageReply.YES)
                Me.m_publisher.SendMessage(fmsg)
                If fmsg.Reply = eMessageReply.NO Then
                    Me.EcosimFitToTimeSeries.UseDefaultV = False
                    Return False
                Else
                    Me.EcosimFitToTimeSeries.UseDefaultV = True
                End If
            End If

            Return Me.SetVToDefault(sDefaultValue)

        Catch ex As Exception
            m_logger.LogError("CheckResetDefaultVulnerabilities. Exception: {message}", ex.Message)
            Debug.Assert(False, ex.Message)
            Return False
        End Try

        Return True

    End Function

    Public Function SetVToDefault(Optional sDefaultValue As Single = 2.0F) As Boolean

        Dim bSuccess As Boolean = True
        Me.EcosimArenaManager.InUpdates = True
        Me.SetBatchLock(eBatchLockType.Update)

        Try
            Dim groupSim As cEcosimGroupInput = Nothing
            For iPrey As Integer = 1 To Me.nGroups
                groupSim = Me.EcosimGroupInputs(iPrey)
                For iPred As Integer = 1 To Me.nGroups
                    groupSim.VulMult(iPred) = sDefaultValue
                Next iPred
            Next iPrey
        Catch ex As Exception
            m_logger.LogError("SetVToDefault. Exception: {message}", ex.Message)
            Debug.Assert(False, ex.Message)
            bSuccess = False
        End Try

        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecosim)
        Me.EcosimArenaManager.InUpdates = False
        Me.EcosimArenaManager.ResetArenas(0)

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecosim scale vulnerabilities by trophic level.
    ''' </summary>
    ''' <param name="sVulLow"></param>
    ''' <param name="sVulHigh"></param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ScaleVulnerabilitiesToTL(sVulLow As Single, sVulHigh As Single) As Boolean

        Try
            If (Me.m_Ecosim.ScaleVulnerabilitiesToTL(sVulLow, sVulHigh)) Then
                Me.LoadEcosimGroups()
                Me.m_publisher.SendMessage(New cMessage("Ecosim groups have changed.", eMessageType.DataModified,
                                                        eCoreComponentType.Ecosim, eMessageImportance.Maintenance))
                Return True
            End If
        Catch ex As Exception
            m_logger.LogError("ScaleVulnerabilitiesToTL. Exception: {message}", ex.Message)
            Debug.Assert(False, ex.Message)
        End Try

        Return False

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
    Private Function HasNonDefaultVulnerabilty(Optional sValue As Single = 2.0!) As Boolean

        Dim groupPath As cEcoPathGroupInput = Nothing
        Dim groupSim As cEcosimGroupInput = Nothing

        For iPred As Integer = 1 To Me.nLivingGroups
            For iPrey As Integer = 1 To Me.nGroups
                groupPath = Me.EcopathGroupInputs(iPred)
                groupSim = Me.EcosimGroupInputs(iPred)
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
            DataSource.SetChanged(eCoreComponentType.Ecosim)
            Me.m_StateMonitor.UpdateDataState(DataSource)

            Me.Messages.SendMessage(New cMessage("Vulnerabilites changed.", eMessageType.DataModified, eCoreComponentType.Ecosim, eMessageImportance.Maintenance))

        Catch ex As Exception
            m_logger.LogError("VulnerabilitiesChanged. Exception: {message}", ex.Message)
        End Try

    End Sub

    Public Function CalcEcosimVulBo(BmaxBo As Single,
                                    iGroup As Integer,
                                    FtimeOn As Boolean) As Single
        Return Me.m_Ecosim.VulBo(BmaxBo, iGroup, FtimeOn)
    End Function

    Public Function CalcEcosimVulFMax(Fpo As Single,
                                      iGroup As Integer,
                                      FtimeOn As Boolean) As Single
        Return Me.m_Ecosim.VulFmax(Fpo, iGroup, FtimeOn)
    End Function

    Public Function EstimateVulnerabilities(iGroup As Integer,
                                            ByRef PotGrowth As Single, ByRef FWMax As Single,
                                            estimates As Single()) As Boolean
        Return Me.m_Ecosim.EstimateVulnerabilities(iGroup, PotGrowth, FWMax, estimates)
    End Function


    Public Sub SetFtimeFromGear(t As Integer, QYear() As Single, PredEffort As Boolean)
        Try
            Me.m_Ecosim.SetFtimeFromGear(t, QYear, PredEffort)
        Catch ex As Exception

        End Try

    End Sub

#End Region 'EcoSim

#Region " Ecospace "

#Region " Variables "

    Friend m_Ecospace As cEcoSpace
    Public m_EcospaceData As cEcospaceDataStructures
    Private m_EcospaceGroups As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcospaceGroup, 1)
    Private m_EcospaceFleets As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcospaceFleet, 1)
    Private m_EcospaceScenarios As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcoSpaceScenario, 1)
    Friend m_EcospaceHabitats As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcospaceHabitat, 0)
    Friend m_EcospaceRegions As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcospaceLayerRegion, 1)
    Friend m_EcospaceMPAs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcospaceMPA, 1)
    Private m_EcospaceModelParams As cEcospaceModelParameters
    Private m_EcospaceBasemap As cEcospaceBasemap
    Private m_spaceresults As cEcospaceTimestep
    Private m_SpaceInterfaceCallBacks As New List(Of EcoSpaceInterfaceDelegate)
    Private m_SpaceInterfaceCallBackUI As EcoSpaceInterfaceDelegate = Nothing

    Private m_EcospaceTimeSeriesManager As EcospaceTimeSeries.cEcospaceTimeSeriesManager

    'Ecospace output lists
    '  Friend m_EcospaceGroupSummaries As New cCoreInputOutputList(Of cEcospaceGroupSummary)(eDataTypes.NotSet, 1)
    ' Index 0 holds the combined fleet
    Friend m_EcospaceFleetOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 0)
    ' the zero index holds the data not include in one of the other regions
    Friend m_EcospaceRegionSummaries As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 0)
    Friend m_EcospaceGroupOuputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.NotSet, 1)

    Friend m_mapInteractionManager As cEcospaceEnviroResponseManager
    Friend m_mapMortalityManager As cEcospaceMortalityResponseManager

    Friend m_EcosimEnviroResponseManager As cEcosimEnviroResponseManager
    Friend m_EcosimMortalityResponseManager As cEcosimMortalityResponseManager

    Private m_stpwSpaceTimer As Stopwatch
    Private m_spaceSaveTime As Double

#End Region ' Variables

#Region " Ecospace Public Methods "

    Public ReadOnly Property SpatialDataConnectionManager As SpatialData.cSpatialDataConnectionManager
        Get
            Return Me.m_spatialdataconnectionManager
        End Get
    End Property

    Public ReadOnly Property SpatialDatasetManager As SpatialData.cSpatialDataSetManager
        Get
            Return Me.m_spatialdataconnectionManager.DatasetManager
        End Get
    End Property

    Public ReadOnly Property SpatialOperationLog As cSpatialOperationLog
        Get
            Return Me.m_spatialOperationLog
        End Get
    End Property

    Private Function InitEcospace() As Boolean

        m_Ecospace = New cEcoSpace()

        m_Ecospace.Messages.AddMessageHandler(New cMessageHandler(AddressOf EcospaceMessageHandler, eCoreComponentType.Ecospace, eMessageType.Any, Me.m_SyncObj))

        'jb 16-June-2016 Remove the cEcospaceTimeSeriesDataStructures when implementing Ecosim biomass forcing in EcoSpace
        'Ecospace can use the Cores cTimeSeriesDataStructures object
        m_Ecospace.TimeSeriesData = Me.m_TSData

        'data need to initialize
        m_EcospaceData.StanzaGroups = Me.m_Stanza
        m_EcospaceData.EcoPathData = Me.m_EcopathData
        m_AdvectionManager = New cAdvectionManager

        m_spatialdataconnectionManager = New SpatialData.cSpatialDataConnectionManager()
        m_spatialdataconnectionManager.Init(Me, Me.m_SpatialData)
        m_spatialOperationLog = New cSpatialOperationLog(Me)

        Me.m_EcospaceTimeSeriesManager = New EcospaceTimeSeries.cEcospaceTimeSeriesManager(Me, m_EcospaceData)

        'counters needed 
        'this could change to get the counter from the above data structures
        m_EcospaceData.NGroups = Me.nGroups
        m_EcospaceData.nFleets = Me.nFleets
        m_EcospaceData.nLiving = Me.nLivingGroups

        m_EcospaceData.DefaultBasemapDimensions()

        ' m_EcoSpaceData.ReDimFleets()
        m_EcospaceData.SetDefaults()

        m_EcospaceData.RedimMigratoryVariables()

        m_Ecospace.EcoSpaceData = Me.m_EcospaceData
        m_Ecospace.StanzaData = Me.m_Stanza
        m_Ecospace.EcoPathData = Me.m_EcopathData
        m_Ecospace.EcoSim = Me.m_Ecosim
        m_Ecospace.EcoSimData = Me.m_EcoSimData
        m_Ecospace.ContaiminantTracerData = m_tracerData
        m_Ecospace.SpatialData = m_SpatialData
        m_Ecospace.EcoFunctions = Me.EcoFunction
        m_Ecospace.AdvectionManager = Me.AdvectionManager
        m_Ecospace.TimeSeriesManager = Me.m_EcospaceTimeSeriesManager

        'sub in core to call at each time step
        m_Ecospace.TimeStepDelegate = AddressOf onEcospaceTimeStep

        'this will initialize local Ecospace variables to default values as well as some dimensioning
        m_Ecospace.InitToDefaults()

        m_mapInteractionManager = New cEcospaceEnviroResponseManager(Me)
        m_mapInteractionManager.Init(Me.m_EcospaceData, Me.m_EcoSimData.CapEnvResData)


        'jb Moved to InitEcosim
        'm_EcosimEnviroResponseManager = New cEcosimEnviroResponseManager(Me)
        'jb Use the Ecospace Environmental Response functions for now
        'this means Ecosim will use the same response functions as Ecospace
        'm_EcosimEnviroResponseManager.Init(Me.m_EcoSimData, Me.m_EcoSimData.EcosimEnvResFunctions)
        'm_EcosimEnviroResponseManager.Init(Me.m_EcoSimData, Me.m_EcoSimData.CapEnvResData)
        'Me.m_EcoSim.EcosimEnviroResponseManager = m_EcosimEnviroResponseManager


        'm_EcosimMortalityResponseManager = New cEcosimMortalityResponseManager(Me)
        'jb Use the Ecospace Environmental Response functions for now
        'this means Ecosim will use the same response functions as Ecospace
        'm_EcosimMortalityResponseManager.Init(Me.m_EcoSimData, Me.m_EcoSimData.CapEnvResData)
        'Me.m_EcoSim.EcosimMortalityResponseManager = m_EcosimMortalityResponseManager

        m_mapMortalityManager = New cEcospaceMortalityResponseManager(Me)
        m_mapMortalityManager.Init(Me.m_EcospaceData, Me.m_EcoSimData.CapEnvResData)


        'm_EcosimEnviroResponseManager = New cEcosimEnviroResponseManager(Me)
        ''jb Use the Ecospace Environmental Response functions for now
        ''this means Ecosim will use the same response functions as Ecospace
        ''m_EcosimEnviroResponseManager.Init(Me.m_EcoSimData, Me.m_EcoSimData.EcosimEnvResFunctions)
        'm_EcosimEnviroResponseManager.Init(Me.m_EcoSimData, Me.m_EcoSimData.CapEnvResData)

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Recalculate the Ecospace distance sailing cost map for all fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub CalcEcospaceCostOfSailing()

        Me.m_Ecospace.CalculateCostOfSailing()
        Me.onChanged(Me.EcospaceBasemap.LayerSailingCost(1))

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set all coastal cells to ports for a given fleet.
    ''' </summary>
    ''' <param name="iFleet">The fleet to set the ports for. If not provided,
    ''' all coastal cells for all fleets are set to ports.</param>
    ''' -----------------------------------------------------------------------
    Public Sub SetEcospaceAllCoastToPort(Optional iFleet As Integer = cCore.NULL_VALUE)

        Me.m_Ecospace.SetAllCoastsToPorts(iFleet)
        Me.onChanged(Me.EcospaceBasemap.LayerPort(iFleet))

        ' JS: do not automatically calculate sailing cost and overwrite data; that should be done explicitly
        ' Me.CalcEcospaceCostOfSailing

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear port cells for a given fleet.
    ''' </summary>
    ''' <param name="iFleet">The fleet to clear the ports for. If not provided,
    ''' all ports for all fleets are cleared.</param>
    ''' -----------------------------------------------------------------------
    Public Sub ClearEcospacePort(Optional iFleet As Integer = cCore.NULL_VALUE)

        Me.m_Ecospace.ClearPorts(iFleet)
        Me.onChanged(Me.EcospaceBasemap.LayerPort(iFleet))

        ' JS: do not automatically calculate sailing cost and overwrite data; that should be done explicitly
        ' Me.CalcEcospaceCostOfSailing

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set all coastal cells to ports for a given fleet.
    ''' </summary>
    ''' <param name="iDepth">The min depth to exclude from computations.</param>
    ''' -----------------------------------------------------------------------
    Public Sub SetExcludedDepth(iDepth As Integer)

        Me.m_Ecospace.SetExcludedDepth(iDepth)
        Me.onChanged(Me.EcospaceBasemap.LayerExclusion())

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear excluded cells.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub ClearExcludedCells()
        'Dim ExcBuffer(,) As Boolean = New Boolean(Me.m_EcoSpaceData.InRow + 1, Me.m_EcoSpaceData.InCol + 1) {}
        'Array.Copy(Me.m_EcoSpaceData.Excluded, ExcBuffer, Me.m_EcoSpaceData.Excluded.Length)

        Me.m_Ecospace.ClearExcludedCells()
        Me.onChanged(Me.EcospaceBasemap.LayerExclusion())

        'Array.Copy(ExcBuffer, Me.m_EcoSpaceData.Excluded, Me.m_EcoSpaceData.Excluded.Length)
        'Me.onChanged(Me.EcospaceBasemap.LayerExclusion())

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear excluded cells.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub InvertExcludedCells()

        Me.m_Ecospace.InvertExcludedCells()
        Me.onChanged(Me.EcospaceBasemap.LayerExclusion())

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Convert an Ecospace time step to absolute time.
    ''' </summary>
    ''' <param name="iTime">The Ecospace time step to convert.</param>
    ''' <returns>The absolute time represented by a time step.</returns>
    ''' <remarks>The absolute time is based on the <see cref="EcosimFirstYear"/>, to
    ''' which the time represented by a given time step is added. The resulting 
    ''' date is rounded to the first day of the month.</remarks>
    ''' -------------------------------------------------------------------
    Public Function EcosimTimestepToAbsoluteTime(iTime As Integer) As DateTime

        Dim iYear As Integer = Me.EcosimFirstYear + (iTime - 1) \ cCore.N_MONTHS
        Dim iMonth As Integer = ((iTime - 1) Mod cCore.N_MONTHS) + 1
        Return New DateTime(iYear, iMonth, 1)

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Convert an absolute time to an Ecospace time step.
    ''' </summary>
    ''' <param name="dt">The date to convert to a time step.</param>
    ''' <returns></returns>
    ''' <remarks>The resulting time step is calculated from difference in time steps,
    ''' rounded to months, between the given time and the <see cref="EcosimFirstYear"/>.</remarks>
    ''' -------------------------------------------------------------------
    Public Function AbsoluteTimeToEcosimTimestep(dt As DateTime) As Integer

        Dim dtStart As New Date(Math.Max(Me.EcosimFirstYear, 1), 1, 1)
        Dim sTime As Single = (dt.Year - dtStart.Year) + CSng((dt.Month - dtStart.Month) / cCore.N_MONTHS)
        Return CInt(sTime * cCore.N_MONTHS) + 1 ' Timesteps are one-based!

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Convert an Ecospace time step to absolute time.
    ''' </summary>
    ''' <param name="iTime">The Ecospace time step to convert.</param>
    ''' <returns>The absolute time represented by a time step.</returns>
    ''' <remarks>The absolute time is based on the <see cref="EcosimFirstYear"/>, to
    ''' which the time represented by a given time step is added. The resulting 
    ''' date is rounded to the first day of the month.</remarks>
    ''' -------------------------------------------------------------------
    Public Function EcospaceTimestepToAbsoluteTime(iTime As Integer) As DateTime

        ' Translate ecospace time step to year and month
        Dim sTimeStepYearFraction As Single = CSng((iTime - 1) * Me.m_EcospaceData.TimeStep)
        Dim iTimeStepYear As Integer = CInt(Math.Floor(sTimeStepYearFraction))
        Dim iTimeStepMonth As Integer = CInt(((sTimeStepYearFraction - iTimeStepYear) * cCore.N_MONTHS))

        ' Fix potential rounding oddness
        While (iTimeStepMonth >= cCore.N_MONTHS)
            iTimeStepYear += 1
            iTimeStepMonth -= cCore.N_MONTHS
        End While

        ' Return absolute date
        Return New DateTime(Math.Max(Me.EcosimFirstYear + iTimeStepYear, 1), Math.Max(iTimeStepMonth + 1, 1), 1)

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Convert an absolute time to an Ecospace time step.
    ''' </summary>
    ''' <param name="dt">The date to convert to a time step.</param>
    ''' <returns></returns>
    ''' <remarks>The resulting time step is calculated from difference in time steps,
    ''' rounded to months, between the given time and the <see cref="EcosimFirstYear"/>.</remarks>
    ''' -------------------------------------------------------------------
    Public Function AbsoluteTimeToEcospaceTimestep(dt As DateTime) As Integer

        Dim dtStart As New Date(Math.Max(Me.EcosimFirstYear, 1), 1, 1)
        Dim sTime As Single = (dt.Year - dtStart.Year) + CSng((dt.Month - dtStart.Month) / cCore.N_MONTHS)
        Return CInt(sTime / Me.m_EcospaceData.TimeStep) + 1 ' Timesteps are one-based!

    End Function

    Public Sub AddEcospaceTimeStepHandler(handler As EcoSpaceInterfaceDelegate)
        If (handler IsNot Nothing) Then Me.m_SpaceInterfaceCallBacks.Add(handler)
    End Sub

    Public Sub RemoveEcospaceTimeStepHandler(handler As EcoSpaceInterfaceDelegate)
        If (handler IsNot Nothing) Then Me.m_SpaceInterfaceCallBacks.Remove(handler)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run the Ecospace model with the currently loaded Ecosim and Ecospace scenario
    ''' </summary>
    ''' <param name="EcospaceTimeStepHandler">
    ''' Optional handler to call with timestep data. 
    '''  If no handler is supplied then the user will not be called at each time step. 
    ''' </param>
    ''' <param name="RunOnThread">
    ''' Optional parameter to run Ecospace on the same thread as the calling process.
    ''' By default Ecospace is run on a separate thread. If RunOnThread = False Ecospace will run on the same thread as the calling process.
    ''' </param>
    ''' <remarks>
    ''' If RunOnThread = True (default behaviour) then RunEcoSpace(...) is run asynchronously, it will return immediately after starting Ecospace on a separate thread.
    ''' Once the Ecospace run completes the <see cref="StateMonitor">cCore.StateMonitor()</see> will fire a CoreExecutionStateEvent().
    ''' </remarks>
    ''' <returns>
    ''' If RunOnThread = True then True if a new thread was started. False otherwise. 
    ''' If RunOnThread = False then True when the run has completed. False otherwise.
    '''  </returns>
    ''' -----------------------------------------------------------------------
    Public Function RunEcospace(Optional ByRef EcospaceTimeStepHandler As EcoSpaceInterfaceDelegate = Nothing, Optional RunOnThread As Boolean = True) As Boolean
        Dim breturn As Boolean

        Debug.Assert(Me.m_StateMonitor.HasEcospaceLoaded, "RunEcospace() You must load an Ecospace scenario first.")
        'ToDo_jb 7-July-2011 cCore.RunEcospace() When running Ecospace the OnEcospaceRunCompleted delegate must always be called
        'even if the run failed
        'This could be done by adding a CanEcoSpaceRun function that did all the checking 
        'if that failed then OnEcospaceRunCompleted() could be called with False...

        If Me.m_StateMonitor.IsEcospaceRunning Then
            'EcoSpace is already running
            'Send a message and return False
            Me.m_publisher.SendMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_RUNNING,
                                      eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Warning))
            Return False

        End If

        If Me.m_StateMonitor.HasEcosimLoaded Then
            If Not Me.m_StateMonitor.HasEcosimInitialized Then
                'Ecosim is loaded but not initialized. Do a partial initialization
                If Me.m_Ecosim.Init(False) Then
                    Me.StateMonitor.SetEcoSimInitialized()
                Else
                    'Failed to init Ecosim, post a message and return
                    Me.m_publisher.SendMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_SIM_INIT_FAILED,
                                              eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Warning))
                    Return False
                End If
            End If
        Else
            'No Ecosim scenario loaded, post a message and return
            Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_NO_SIM_SCENARIO,
                                      eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Warning))
            Return False
        End If
        Try
            ' Dim t As Double = Timer
            System.Console.WriteLine("----------cCore.RunEcospace() Start------------")

            If Me.m_StateMonitor.HasEcospaceLoaded Then

                ' Prepare outputs at the beginning of the run
                InitEcospaceOutputs()
                InitEcotracerOutputs()

                If CheckHabitats() And CheckVariousParameters() Then
                    If CheckMigrationMapsSet() Then
                        If CheckExternalSpatialTemporalData() Then

                            ' Write detailed info
                            m_logger.LogInformation("Started Ecospace run with {num} configured connection(s), start year {year}", Me.SpatialDataConnectionManager.NumConnectedAdapters, Me.EcosimFirstYear)

                            'Setup delegates for Ecospace to call 
                            Me.AddEcospaceTimeStepHandler(EcospaceTimeStepHandler)
                            m_SpaceInterfaceCallBackUI = EcospaceTimeStepHandler
                            m_Ecospace.TimeStepDelegate = AddressOf onEcospaceTimeStep
                            Me.m_Ecospace.RunCompletedDelegate = AddressOf Me.onEcoSpaceRunCompleted

                            'Tell the StateMonitor a run has started
                            Me.m_StateMonitor.SetEcospaceRun()
                            Me.SetStopRunDelegate(New StopRunDelegate(AddressOf StopEcospace))

                            Me.initEcospaceResultsWriters()

                            'make sure Ecospace is not paused
                            Me.m_Ecospace.isPaused = False

                            Me.m_spatialOperationLog.BeginRun()
                            Me.MortalityMapInteractionManager.InitRun()
                            Me.CapacityMapInteractionManager.InitRun()
                            Me.m_stpwSpaceTimer = Stopwatch.StartNew
                            Me.m_spaceSaveTime = 0

                            If RunOnThread Then
                                'Run Ecospace
                                breturn = Me.m_Ecospace.RunThreaded()
                            Else
                                breturn = Me.m_Ecospace.Run()
                                'jb 30-May-2014 onEcoSpaceRunCompleted() was called by Ecospace.Run()
                                'Don't call it again
                                'If EcospaceTimeStepHandler Is Nothing Then
                                '    'if no RunCompleted call back then makes sure the onEcoSpaceRunCompleted() is called
                                '    Me.onEcoSpaceRunCompleted(breturn)
                                'End If

                            End If
                        End If ' If CheckSpatialDataTimeSteps() Then
                    End If
                End If 'If checkHabitats() Then

            Else 'If Me.m_StateMonitor.HasEcospaceLoaded Then
                Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_NO_SPACE_SCENARIO,
                                          eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Warning))
            End If 'If Me.m_StateMonitor.HasEcospaceLoaded Then

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Me.m_publisher.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_RUN_ERROR, ex.Message),
                                      eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Critical))
            breturn = False
        End Try

        Return breturn

    End Function

    'Public Function LoadEcospaceTimeSeriesData(InputFileName As String, OutputFileName As String) As Boolean

    '    Try
    '        Return Me.m_Ecospace.TimeSeriesManager.Load(InputFileName, OutputFileName)
    '    Catch ex As Exception

    '    End Try
    '    Return False
    'End Function

    Public ReadOnly Property EcospaceTimeSeriesManager As EcospaceTimeSeries.cEcospaceTimeSeriesManager
        Get
            Return Me.m_EcospaceTimeSeriesManager
        End Get
    End Property


    Private Sub onEcoSpaceRunCompleted(Succeeded As Boolean)

        Try
            Try
                Me.m_stpwSpaceTimer.Stop()
                Dim totRT As Double = Me.m_stpwSpaceTimer.Elapsed.TotalSeconds
                ' System.Console.WriteLine("Ecospace Runtime(sec) = " + totRT.ToString + ", Save time = " + Me.m_spaceSaveTime.ToString + ", % " + (Me.m_spaceSaveTime / totRT * 100).ToString)
            Catch ex As Exception

            End Try

            If Succeeded Then
                LoadEcospaceResults()
                loadEcotracerResults()
            End If

            Try
                For n As Integer = 1 To Me.m_EcospaceModelParams.nResultWriters
                    Dim writer As IEcospaceResultsWriter = Me.m_EcospaceModelParams.ResultWriter(n)
                    If (writer.Enabled) Then
                        writer.EndWrite()
                    End If
                Next
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".cEcospaceResultsWriter.EndWrite() Exception: " & ex.Message)
                m_logger.LogError("onEcospaceRunCompleted SaveResults. Exception: {message}", ex.Message)
            End Try

            ' Did a spatial temporal error occur?
            If Not Me.m_spatialOperationLog.EndRun() Then
                Dim msg As New cMessage(My.Resources.CoreMessages.SPATIALTEMPORAL_RUN_ISSUES, eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Warning)
                msg.Hyperlink = Me.m_spatialOperationLog.LogFileName
                Me.m_publisher.AddMessage(msg)
            End If

            Me.RemoveEcospaceTimeStepHandler(Me.m_SpaceInterfaceCallBackUI)
            Me.m_SpaceInterfaceCallBackUI = Nothing

            Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.ECOSPACE_RUN_COMPLETED,
                          eMessageType.EcospaceRunCompleted, eCoreComponentType.Ecospace, eMessageImportance.Information))

            Me.m_StateMonitor.SetEcospaceCompleted(Me.m_tracerData.EcoSpaceConSimOn)
            Me.m_publisher.sendAllMessages()

            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.EcospaceRunCompleted(Me.m_EcospaceData)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onEcoSpaceRunCompleted() Exception: " & ex.Message)
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set Ecospace to pause
    ''' </summary>
    ''' <returns>True if Ecosapce is set to pause.</returns>
    ''' -----------------------------------------------------------------------
    Public Property EcospacePaused() As Boolean
        Get
            Return Me.m_Ecospace.isPaused
        End Get

        Set(value As Boolean)
            Me.m_Ecospace.isPaused = value
            Me.m_StateMonitor.SetIsPaused()
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Has the capacity been set to a reasonable level for all groups.
    ''' </summary>
    ''' <returns>True if all groups are above so min value. False otherwise</returns>
    ''' -----------------------------------------------------------------------
    Public Function CheckHabitats() As Boolean
        Dim igrp As Integer
        Dim msg As cFeedbackMessage = Nothing
        Dim vs As cVariableStatus = Nothing
        Dim limits() As Single = New Single(Me.nGroups) {}

        'set the lower limit based on the trophic level
        For i As Integer = 1 To Me.nGroups
            '0.1% average capacity
            limits(i) = 0.1
        Next

        Try

            'get the groups that are below the limit
            Dim FailedGroups() As Integer = Me.m_Ecospace.GetHabCapsLessThen(limits)

            'send a message if there are groups that failed the HabCap test
            If FailedGroups.Count > 0 Then
                Dim strMsg As String = My.Resources.CoreMessages.ECOSPACE_LOWHABITAT_CAP
                msg = New cFeedbackMessage(strMsg, eCoreComponentType.Ecospace, eMessageType.InvalidModel_HabCapLow, eMessageImportance.Warning, eMessageReplyStyle.YES_NO, , eMessageReply.YES)
                msg.Suppressable = True

                For Each igrp In FailedGroups
                    Dim avgCap As Single = Me.m_EcospaceData.TotHabCap(igrp) / Me.m_EcospaceData.nWaterCells * 100
                    strMsg = cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_LOWHABITAT_CAP_GROUP, Me.m_EcopathData.GroupName(igrp), avgCap)
                    vs = New cVariableStatus(eStatusFlags.MissingParameter, strMsg, eVarNameFlags.NotSet, eDataTypes.EcospaceLayerHabitatCapacity, eCoreComponentType.Ecospace, igrp)
                    msg.AddVariable(vs)
                Next

                Me.m_publisher.SendMessage(msg)

                If msg.Reply = eMessageReply.NO Then
                    Return False
                End If

            End If

        Catch ex As Exception

        End Try

        Return True

    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check if ecospace parameters may cause trouble.
    ''' </summary>
    ''' <returns>True if all ok, False otherwise</returns>
    ''' -----------------------------------------------------------------------
    Public Function CheckVariousParameters() As Boolean

        Dim msg As cFeedbackMessage = Nothing
        Dim vs As cVariableStatus = Nothing

        Try
            Dim FailedFleets() As Integer = Me.m_Ecospace.GetEffPowerLessThan(0.01!)
            If FailedFleets.Count > 0 Then
                Dim strMsg As String = My.Resources.CoreMessages.ECOSPACE_LOWEFFPOWER
                msg = New cFeedbackMessage(strMsg, eCoreComponentType.Ecospace, eMessageType.InvalidModel_EffPower0, eMessageImportance.Warning, eMessageReplyStyle.YES_NO, , eMessageReply.YES)
                msg.Suppressable = True

                For Each iflt As Integer In FailedFleets
                    Dim flt As cEcospaceFleetInput = Me.EcospaceFleetInputs(iflt)
                    strMsg = cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_LOWEFFPOWER_FLEET, Me.m_EcopathData.GroupName(iflt), Me.m_EcospaceData.EffPower(iflt))
                    vs = New cVariableStatus(flt, eStatusFlags.MissingParameter, strMsg, eVarNameFlags.EffectivePower)
                    msg.AddVariable(vs)
                Next

                Me.m_publisher.SendMessage(msg)

                If msg.Reply = eMessageReply.NO Then
                    Return False
                End If

            End If

        Catch ex As Exception

        End Try

        ' Various region checks
        Dim nRegions As Integer = Me.nRegions
        If (nRegions > 0) Then
            Dim sbWarnings As New StringBuilder()
            Dim bWoops As Boolean = False
            sbWarnings.AppendLine(My.Resources.CoreMessages.REGIONS_PRERUN_WARNING)

            If (nRegions > Me.EcospaceDataStructures.nWaterCells / 4) Then
                ' That's a pile of regions. Alert user
                sbWarnings.AppendLine(My.Resources.CoreMessages.REGIONS_WARNING_TOOMANY)
                bWoops = True
            End If

            Dim nMaxAllocatedRegions As Integer = 0
            Dim hsValid As New HashSet(Of Integer)
            Dim hsInvalid As New HashSet(Of Integer)
            For iRow As Integer = 1 To Me.EcospaceDataStructures.InRow
                For iCol As Integer = 1 To Me.EcospaceDataStructures.InCol
                    If Me.EcospaceDataStructures.Depth(iRow, iCol) > 0 Then
                        Dim iReg As Integer = Me.EcospaceDataStructures.Region(iRow, iCol)
                        If (iReg > 0) Then
                            If (iReg <= nRegions) Then
                                hsValid.Add(iReg)
                            Else
                                hsInvalid.Add(iReg)
                            End If
                        End If
                    End If
                Next
            Next

            If (hsInvalid.Count > 0) Then
                ' nRegions may be set too low
                sbWarnings.AppendLine(cStringUtils.Localize(My.Resources.CoreMessages.REGIONS_WARNING_UNUSED, hsInvalid.Count))
                bWoops = True
            End If

            If hsValid.Count < Me.nRegions Then
                ' Not all regions spoken for
                sbWarnings.AppendLine(cStringUtils.Localize(My.Resources.CoreMessages.REGIONS_WARNING_NOCELLS, Me.nRegions - hsValid.Count))
                bWoops = True
            End If

            If (bWoops) Then
                msg = New cFeedbackMessage(sbWarnings.ToString(), eCoreComponentType.Ecospace, eMessageType.InvalidModel_Regions, eMessageImportance.Warning, eMessageReplyStyle.YES_NO, , eMessageReply.YES)
                msg.Suppressable = True

                Me.m_publisher.SendMessage(msg)
                If msg.Reply = eMessageReply.NO Then
                    Return False
                End If
            End If

        End If
        Return True

    End Function

    Public Function CheckMigrationMapsSet() As Boolean
        Dim msg As cFeedbackMessage = Nothing
        Dim vs As cVariableStatus = Nothing
        Dim MigMapsSet() As Boolean
        Try

            If Me.m_Ecospace.getMissingMigrationMaps(MigMapsSet) > 0 Then
                Dim strMsg As String = My.Resources.CoreMessages.MIGRATION_MISSING_MAPS
                msg = New cFeedbackMessage(strMsg, eCoreComponentType.Ecospace, eMessageType.InvalidModel_MigMapsMissing, eMessageImportance.Warning, eMessageReplyStyle.YES_NO, , eMessageReply.YES)
                msg.Suppressable = True

                For igrp As Integer = 1 To Me.nGroups
                    If Not MigMapsSet(igrp) Then
                        strMsg = cStringUtils.Localize(My.Resources.CoreMessages.MIGRATION_MISSING_GROUPS, Me.m_EcopathData.GroupName(igrp))
                        vs = New cVariableStatus(eStatusFlags.MissingParameter, strMsg, eVarNameFlags.NotSet, eDataTypes.EcospaceLayerHabitatCapacity, eCoreComponentType.Ecospace, igrp)
                        msg.AddVariable(vs)
                    End If
                Next

                Me.m_publisher.SendMessage(msg)

                If msg.Reply = eMessageReply.NO Then
                    Return False
                End If

            End If

        Catch ex As Exception

        End Try

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check whether the spatial/temporal data set-up is valid.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function CheckExternalSpatialTemporalData() As Boolean

        If (Me.m_spatialdataconnectionManager.NumConnectedAdapters = 0) Then Return True

        ' Allow a bit of room for rounding errors
        If (Math.Round(1 / Me.m_EcospaceData.TimeStep, 3) > cCore.N_MONTHS) Then
            Dim fmsg As New cFeedbackMessage(My.Resources.CoreMessages.SPATIALTEMPORAL_TOOMANYTIMESTEPS,
                                             eCoreComponentType.External, eMessageType.Any, eMessageImportance.Warning)
            fmsg.ReplyStyle = eMessageReplyStyle.YES_NO
            fmsg.Suppressable = True
            fmsg.Reply = eMessageReply.YES

            Me.m_publisher.SendMessage(fmsg)

            If (fmsg.Reply = eMessageReply.NO) Then
                Return False
            End If

        End If

        ' Check if all connections are able to deliver data
        Dim problems As ISpatialDataSet() = Me.m_spatialdataconnectionManager.InvalidConnections(True)
        If (problems.Length > 0) Then

            Dim fmsg As New cFeedbackMessage(My.Resources.CoreMessages.SPATIALTEMPORAL_MISSINGDATA,
                                             eCoreComponentType.External, eMessageType.Any, eMessageImportance.Warning)
            For Each conn As ISpatialDataSet In problems
                Dim vs As New cVariableStatus(eStatusFlags.MissingParameter,
                                              cStringUtils.Localize(My.Resources.CoreMessages.SPATIALTEMPORAL_MISSINGDATA_DETAIL, conn.CustomName),
                                              eVarNameFlags.Name, eDataTypes.External, eCoreComponentType.External, 0)
                fmsg.AddVariable(vs)
            Next

            fmsg.ReplyStyle = eMessageReplyStyle.YES_NO
            fmsg.Suppressable = True
            fmsg.Reply = eMessageReply.YES

            Me.m_publisher.SendMessage(fmsg)

            If (fmsg.Reply = eMessageReply.NO) Then Return False

        End If
        Return True

    End Function


    'Private Sub CheckEcosimCatchability()

    '    Dim bUpdate As Boolean = False
    '    For iflt As Integer = 1 To Me.nFleets
    '        For igrp As Integer = 1 To Me.nGroups
    '            Dim baseRelQ As Single = CSng((m_EcopathData.Landing(iflt, igrp) + m_EcopathData.Discard(iflt, igrp)) / (m_EcopathData.B(igrp) + 1.0E-20))
    '            If baseRelQ > 0 Then

    '                For it As Integer = 1 To Me.nEcosimTimeSteps
    '                    'If baseRelQ <> Me.m_EcoSimData.relQt(iflt, igrp, it) Then
    '                    If Math.Abs(baseRelQ - Me.m_EcoSimData.relQt(iflt, igrp, it)) > 0.0001 Then
    '                        Dim msg As String = "Ecosim Catchability does not match the default catchabilities. Would you like to reset this to default values?"
    '                        Dim msgFB As New cFeedbackMessage(msg, eCoreComponentType.Ecosim, eMessageType.ErrorEncountered, eMessageImportance.Warning, eMessageReplyStyle.YES_NO)
    '                        Me.Messages.SendMessage(msgFB)
    '                        If msgFB.Reply = eMessageReply.YES Then
    '                            Me.SetDefaultCatchabilities()
    '                        End If
    '                        'Done just stop the search
    '                        Return
    '                    End If
    '                Next it

    '            End If 'If baseRelQ > 0 Then
    '        Next igrp
    '    Next iflt

    'End Sub

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
                m_Ecospace.StopRun() ' = True
            End If
        Catch ex As Exception
            m_logger.LogError("StopEcospace() Error: {message}", ex.Message)
        End Try
    End Sub

#End Region

#Region "Ecospace Private Methods"

    ''' <summary>
    ''' This gets called by Ecospace at every time step
    ''' </summary>
    ''' <param name="iTime">Time index of this time step</param>
    ''' <remarks>processEcospaceTimeStep() will populate the cEcospaceTSResults object and send it to an interface</remarks>
    Private Sub onEcospaceTimeStep(iTime As Integer)
        Try
            Dim f As Single
            m_spaceresults.InSpinUp = Me.m_EcospaceData.bInSpinUp
            If Me.m_EcospaceData.bInSpinUp Then
                m_spaceresults.RunProgress = CSng(Me.m_Ecospace.iSpinUp / Me.m_Ecospace.nSpinUp)
            Else
                m_spaceresults.RunProgress = CSng(Me.m_EcospaceData.TimeNow * Me.m_EcospaceData.nTimeStepsPerYear / Me.m_EcospaceData.nTimeSteps)
            End If

            If Not Me.m_EcospaceData.bInSpinUp Then

                m_spaceresults.iTimeStep = iTime
                m_spaceresults.TimeStepinYears = CSng(m_EcospaceData.TimeNow + m_EcospaceData.TimeStep)

                m_spaceresults.ComputeSumEffortMap()

                'the group time-step data was populated by Ecospace
                For igrp As Integer = 1 To nGroups
                    m_spaceresults.Biomass(igrp) = m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.Biomass, igrp, iTime)
                    m_spaceresults.RelativeBiomass(igrp) = m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.RelativeBiomass, igrp, iTime)

                    'Make these relative to base values
                    m_spaceresults.FishingMort(igrp) = m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.FishingMort, igrp, iTime) / Me.m_EcospaceData.BaseFishMort(igrp)
                    f = m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.CatchBio, igrp, iTime) / CSng((m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.Biomass, igrp, iTime) + 0.0000001))
                    f = f / (Me.m_EcospaceData.BaseFishMort(igrp) + 1.0E-20F)
                    'm_spaceresults.FishingMort(igrp) = f / Me.m_EcospaceData.BaseFishMort(igrp)
                    m_spaceresults.ConsumptRate(igrp) = m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.ConsumpRate, igrp, iTime) / Me.m_EcospaceData.BaseConsump(igrp)
                    m_spaceresults.PredMortRate(igrp) = m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.PredMortRate, igrp, iTime) / Me.m_EcospaceData.BasePredMort(igrp)
                    m_spaceresults.Catch(igrp) = m_EcospaceData.ResultsByGroup(eSpaceResultsGroups.CatchBio, igrp, iTime) / Me.m_EcospaceData.BaseCatch(igrp)

                    If m_Ecospace.ContaiminantTracerData.EcoSpaceConSimOn Then
                        m_spaceresults.ConcMax(igrp) = m_Ecospace.ContaiminantTracerData.ConcMax(igrp)
                    End If

                    For irgn As Integer = 1 To nRegions
                        m_spaceresults.BiomassByRegion(igrp, irgn) = m_EcospaceData.ResultsRegionGroup(irgn, igrp, iTime)
                    Next

                Next igrp

                Me.SaveEcospaceENA(m_spaceresults)

                'Save to the current writer always (saveannual = false) or once per year (saveannual=true) for the first time step
                'Default is to save every time step
                If (iTime >= EcospaceModelParameters.FirstOutputTimeStep) Then
                    If ((iTime - EcospaceModelParameters.FirstOutputTimeStep) Mod CInt(EcospaceModelParameters.NumberOfTimeStepsPerYear) = 0) Or (Me.m_EcospaceData.SaveAnnual = False) Then
                        Me.SaveEcospaceResults(Me.m_spaceresults)
                    End If
                End If
            End If

            'Call the interface delegate
            For Each callback As EcoSpaceInterfaceDelegate In m_SpaceInterfaceCallBacks
                Try
                    callback(m_spaceresults)
                Catch ex As Exception
                    System.Console.WriteLine("Core.onEcospaceTimeStep(" & iTime.ToString & ") Interface Delegate Exception: " & ex.Message)
                End Try
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".processEcospaceTimeStep() Error: " & ex.Message)
        End Try
    End Sub

    Private Sub SaveEcospaceENA(SpaceResults As cEcospaceTimestep)
        Try

            If Me.m_EcospaceData.bENA Then

                Dim SCORFileWriter As New cSCORFileWriter(Me.m_EcopathData)
                For Each enaData As cENAData In Me.m_EcospaceData.m_enaCellData.Values
                    'System.Console.WriteLine(enaData.Key)
                    SCORFileWriter.Write(enaData.OutputFileName(Me, eAutosaveTypes.Ecospace, SpaceResults.iTimeStep), enaData.ENARData)
                Next enaData

            End If 'Me.m_EcoSpaceData.bENA

        Catch ex As Exception
            m_logger.LogError("enaR Failed to save SCOR file. Exception: {message}", ex.Message)
        End Try
    End Sub

    Private Sub SaveEcospaceResults(SpaceResults As cEcospaceTimestep)

        Dim st As Double = Me.m_stpwSpaceTimer.Elapsed.TotalSeconds
        Try

            For n As Integer = 1 To Me.m_EcospaceModelParams.nResultWriters
                Dim writer As IEcospaceResultsWriter = Me.m_EcospaceModelParams.ResultWriter(n)
                Try
                    If (writer.Enabled) Then writer.WriteResults(Me.m_spaceresults)
                Catch ex As Exception
                    System.Console.WriteLine("Core.SaveEcospaceResults() m_EcospaceResultsWriter Exception: " & ex.Message)
                    m_logger.LogError("SaveEcospaceResults() m_EcospaceResultsWriter Exception: {message}", ex.Message)
                End Try
            Next
            Me.m_spaceSaveTime += Me.m_stpwSpaceTimer.Elapsed.TotalSeconds - st

        Catch ex As Exception
            m_logger.LogError("SaveEcospaceResults() Exception: {message}", ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Sample code to loop over EcoSpace map of mortality by predation 
    ''' </summary>
    Private Sub dumpSpaceMortPred(Core As cCore, SpaceResults As cEcospaceTimestep)

        'loop over all the prey/pred linkages
        For iLink As Integer = 1 To SpaceResults.nPreyPredLinks
            Dim mort As Single
            'get group indexes for prey and pred
            Dim iPreyIndex As Integer = SpaceResults.iPreyIndex(iLink)
            Dim iPredIndex As Integer = SpaceResults.iPredIndex(iLink)

            'get prey and pred names(just for display)
            Dim PreyName As String = Core.EcopathGroupInputs(iPreyIndex).Name
            Dim PredName As String = Core.EcopathGroupInputs(iPredIndex).Name

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

        'at this moment this just passes the messages off to whomever is listening
        'the core does not have to do anything in response to Ecospace messages
        m_publisher.AddMessage(message)

    End Sub

    Private Sub LoadEcospaceResultsWriters()
        ' NOP
    End Sub

    Private Sub UpdateEcospaceForcingByEcosim()
        Try

            Me.m_TSData.SetBiomassForcing(Me.m_EcospaceData.IsEcosimBioForcingGroup)
            Me.m_TSData.SetDiscardForcing(Me.m_EcospaceData.IsEcosimDiscardForcingGroup)

            If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return

            If Not Me.m_EcospaceData.isEcosimBiomassForcingLoaded Then
                Me.m_EcospaceData.UseEcosimBiomassForcing = False
                Me.m_EcospaceModelParams.UseEcosimBiomassForcing = Me.m_EcospaceData.UseEcosimBiomassForcing
            End If
            Me.m_EcospaceModelParams.IsEcosimBiomassForcingLoaded = Me.m_EcospaceData.isEcosimBiomassForcingLoaded

            If Not Me.m_EcospaceData.isEcosimDiscardForcingLoaded Then
                Me.m_EcospaceData.UseEcosimDiscardForcing = False
                Me.m_EcospaceModelParams.UseEcosimDiscardForcing = Me.m_EcospaceData.UseEcosimDiscardForcing
            End If
            Me.m_EcospaceModelParams.IsEcosimDiscardForcingLoaded = Me.m_EcospaceData.isEcosimDiscardForcingLoaded

            Me.m_publisher.SendMessage(New cMessage("Ecospace forcing from Ecosim", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            m_logger.LogError("UpdateEcospaceForcingByEcosim() Exception: {message}", ex.Message)
        End Try

    End Sub

#End Region

#Region " Ecospace interface objects "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the number of available Ecospace scenarios for the loaded model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nEcospaceScenarios() As Integer
        Get
            Try
                ' Return the official ecopath administration figure
                Return Me.m_EcopathData.NumEcospaceScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get an <see cref="cEcoSpacescenario">Ecospace scenario</see> from the available scenarios.
    ''' </summary>
    ''' <param name="iScenario">One-based indexed of the scenario to load
    ''' [1, <see cref="nEcospaceScenarios">#scenarios</see>].</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceScenarios(iScenario As Integer) As cEcospaceScenario
        Get
            ' JS 06Jul07: list will handle scenario index / item index offsets
            Return DirectCast(Me.m_EcospaceScenarios(iScenario), cEcospaceScenario)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the index of the active <see cref="cEcospaceScenario">Ecospace scenario</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveEcospaceScenarioIndex() As Integer
        Get
            Return Me.m_EcopathData.ActiveEcospaceScenario
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
    ''' Get a <see cref="cEcospaceGroupInput">Ecospace group</see> for a given index.
    ''' </summary>
    ''' <param name="iGroup">The index to obtain the Ecospace group for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceGroupInputs(iGroup As Integer) As cEcospaceGroupInput
        Get
            ' JS 06Jul07: list will handle group index / item index offsets
            Return DirectCast(Me.m_EcospaceGroups.Item(iGroup), cEcospaceGroupInput)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceFleetInput">Ecospace fleet</see> for a given index.
    ''' </summary>
    ''' <param name="iFleet">The index to obtain the Ecospace fleet for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceFleetInputs(iFleet As Integer) As cEcospaceFleetInput
        Get
            ' JS 06Jul07: list will handle fleet index / item index offsets
            Return DirectCast(Me.m_EcospaceFleets.Item(iFleet), cEcospaceFleetInput)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceHabitat">Ecospace habitat</see> for a given index.
    ''' </summary>
    ''' <param name="iHabitat">The zero-based index to obtain the Ecospace habitat for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceHabitats(iHabitat As Integer) As cEcospaceHabitat
        Get
            ' JS 06Jul07: list will handle habitat index / item index offsets
            Return DirectCast(Me.m_EcospaceHabitats(iHabitat), cEcospaceHabitat)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a <see cref="cEcospaceMPA">Ecospace MPA</see> for a given index.
    ''' </summary>
    ''' <param name="iMPA">The index to obtain the Ecospace marine protected area for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcospaceMPAs(iMPA As Integer) As cEcospaceMPA
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
    Public ReadOnly Property EcosimFleetInputs(iFleet As Integer) As cEcosimFleetInput
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
    Public ReadOnly Property EcosimFleetOutput(iFleet As Integer) As cEcosimFleetOutput
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
    Public ReadOnly Property EcospaceGroupOutput(iGroup As Integer) As cEcospaceGroupOutput
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
    Public ReadOnly Property EcospaceFleetOutput(iFleet As Integer) As cEcospaceFleetOutput
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
    Public ReadOnly Property EcospaceRegionOutput(iRegion As Integer) As cEcospaceRegionOutput
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
        Me.m_EcospaceScenarios.Clear()
        For i As Integer = 1 To Me.m_EcopathData.EcospaceScenarioDBID.Length - 1
            Me.m_EcospaceScenarios.Add(Me.privateEcospaceScenario(i))
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
    Private Property privateEcospaceScenario(iScenario As Integer) As cEcospaceScenario

        Get
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcopathData.EcospaceScenarioDBID.Length Then
                    m_logger.LogInformation("privateEcospaceScenario(iScenario) index out of bounds.")
                    Return Nothing
                End If

                Dim ess As New cEcospaceScenario(Me)

                ess.AllowValidation = False

                ess.DBID = m_EcopathData.EcospaceScenarioDBID(iScenario)
                ess.Name = m_EcopathData.EcospaceScenarioName(iScenario)
                ess.Description = m_EcopathData.EcospaceScenarioDescription(iScenario)
                ess.Author = m_EcopathData.EcospaceScenarioAuthor(iScenario)
                ess.Contact = m_EcopathData.EcospaceScenarioContact(iScenario)
                ess.LastSaved = m_EcopathData.EcospaceScenarioLastSaved(iScenario)
                ess.Index = iScenario
                ess.ResetStatusFlags()

                ess.AllowValidation = True

                Return ess

            Catch ex As Exception
                m_logger.LogError("cEcospaceScenario() Error: {message}", ex.Message)
                Debug.Assert(False, "Error Getting cEcospaceScenario Info: " & ex.Message)
                Return Nothing
            End Try

        End Get

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Set(ess As cEcospaceScenario)

            'Set the parameters in the underlying EcoSim data structures to user supplied values
            Try
                If iScenario < 0 Or iScenario >= Me.m_EcopathData.EcospaceScenarioDBID.Length Then
                    m_logger.LogInformation("cEcospaceScenario(nScenario) nScenario out of bounds.")
                    Return
                End If

                m_EcopathData.EcospaceScenarioName(iScenario) = ess.Name

            Catch ex As Exception
                m_logger.LogError("privateEcospaceScenario() EcoSim parameters will not be set Error: {message}", ex.Message)
                Debug.Assert(False, "cEcospaceScenario Info will not be set Error: " & ex.Message)
            End Try

        End Set

    End Property

    Private Sub SendEcospaceLoadMessage(strScenarioName As String, Optional strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_LOAD_SUCCESS, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Information)
        Else
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_LOAD_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()

    End Sub

    Private Sub SendEcospaceSaveStateMessage(strScenarioName As String, Optional bSucces As Boolean = True,
            Optional strError As String = "")

        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If bSucces Then
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_SAVE_SUCCES, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Information)
        Else
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_SAVE_FAILED, strScenarioName, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Warning)
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
    ''' <param name="sCellLength">Cell length, in km. A square grid is assumed.</param>
    ''' <returns>True if successful.</returns>
    Public Function NewEcospaceScenario(strName As String, strDescription As String,
            strAuthor As String, strContact As String,
            iNumRows As Integer, iNumCols As Integer,
            sLat As Single, sLon As Single, sCellLength As Single) As Boolean

        Dim ds As IEcospaceDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim iScenario As Integer = 0

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        If Me.m_StateMonitor.HasEcopathLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecospace) Then Return False

        Try

            ds = DirectCast(DataSource, IEcospaceDatasource)

            ds.BeginTransaction()

            ' Clear duplicates
            Me.RemoveEcospaceScenario(Me.FindObjectByName(Me.m_EcospaceScenarios, strName))

            ' Append
            If (ds.AppendEcospaceScenario(strName, strDescription,
                    strAuthor, strContact,
                    iNumRows, iNumCols,
                    sLat, sLon, sCellLength, iScenarioID)) Then
                ds.EndTransaction(True)

                Me.StateMonitor.UpdateDataState(Me.DataSource)
                Me.InitEcospaceScenarios()
                iScenario = Array.IndexOf(Me.m_EcopathData.EcospaceScenarioDBID, iScenarioID)

                If (Me.PluginManager IsNot Nothing) Then
                    Me.PluginManager.EcospaceScenarioAdded(Me.DataSource, iScenarioID)
                End If

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
    ''' <returns>True if successful.</returns>
    Public Function LoadEcospaceScenario(scenario As cEcospaceScenario) As Boolean
        Return LoadEcospaceScenario(scenario.Index)
    End Function

    ''' <summary>
    ''' Load an <see cref="cEcoSpaceScenario">Ecospace scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the <see cref="cEcoSpaceScenario">Scenario</see> in the <see cref="m_EcospaceScenarios">Scenario list</see>.</param>
    ''' <returns>True if successful.</returns>
    Public Function LoadEcospaceScenario(iScenario As Integer) As Boolean

        If (iScenario < 1) Then Return False
        If (Me.nEcospaceScenarios < iScenario) Then Return False
        If (Not Me.StateMonitor.HasEcopathLoaded) Then Return False

        Dim ds As IEcospaceDatasource = Nothing
        Dim strScenarioName As String = Me.m_EcopathData.EcospaceScenarioName(iScenario)
        Dim bSuccess As Boolean = True

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecospace) Then Return False

        Try

            'For an Ecospace scenario to load there must be an Ecosim scenario loaded
            If Not Me.m_StateMonitor.HasEcosimLoaded() Then
                'No implicit running of Ecosim because we do not know which Ecosim scenario to run
                Debug.Assert(False, "LoadEcospaceScenario() Load  Ecosim first. This is temporary.")
                'SendEcospaceLoadMessage(iScenario, "Load Ecosim first. This is temporary.")
                Return False
            End If

            'Clears out any memory
            'And updates core state
            Me.CloseEcospaceScenario()

            Me.m_EcopathData.ActiveEcospaceScenario = -1
            Me.SpatialDataConnectionManager.DatasetManager.Reload(True)

            ds = DirectCast(DataSource, IEcospaceDatasource)
            If Not ds.LoadEcospaceScenario(Me.m_EcopathData.EcospaceScenarioDBID(iScenario)) Then
                Debug.Assert(False, "LoadEcospaceScenario() Failed to load scenario from data source.")
                SendEcospaceLoadMessage("", "Failed to load scenario")
                Return False
            End If

            Me.m_Ecospace.UpdateDepthMap()
            Me.m_Ecospace.CalcHabitatArea()

            Me.m_TSData.SetBiomassForcing(Me.m_EcospaceData.IsEcosimBioForcingGroup)
            Me.m_TSData.SetDiscardForcing(Me.m_EcospaceData.IsEcosimDiscardForcingGroup)

            ' JB 12dec10: Space can not run longer than Sim
            If m_EcospaceData.TotalTime > m_EcoSimData.NumYears Then m_EcospaceData.TotalTime = m_EcoSimData.NumYears

            m_Ecospace.SearchData = m_SearchData

            'all the input maps have changed if a new scenario is loaded
            Me.m_EcospaceData.isCapacityChanged = True
            Me.m_EcospaceData.setHabCapGroupIsChanged(True)


            'hardwire some capacity maps for debugging
            Me.m_EcospaceData.setDebugCapMaps(Me.m_EcoSimData.CapEnvResData)
            Debug.Print("Ecospace Shapemanager Init")
            Dim EnvRespManager As cEnviroResponseShapeManager = DirectCast(Me.m_ShapeManagers.Item(eDataTypes.CapacityMediation), cEnviroResponseShapeManager)
            EnvRespManager.Init()
            ' EnvRespManager.UpdateNormalDistributions()

            'sets the summary peroids to first and last year
            'at this time this data is not saved in the database
            m_EcospaceData.setDefaultSummaryPeriod()

            'This flag tells Ecospace to use the fishing rates set by Ecosim
            'in EwE5 it is set in the Ecosim database reading routine 
            'if it is set to false Ecospace will set all the FishGearRates() to one
            m_EcospaceData.IsFishRateSet = True

            m_EcospaceData.SetDefaultThreads()
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
            bSuccess = bSuccess And InitEcospaceMPAs()
            bSuccess = bSuccess And InitEcospaceGroups()
            bSuccess = bSuccess And InitEcospaceFleets()
            '   bSuccess = bSuccess And InitEcospaceAdvection()

            'Init advection
            Me.m_AdvectionManager.Init(Me, Me.m_Ecospace)
            Me.m_AdvectionManager.Load()

            ' JS 15Jan13: moved this to RunEcospace. Region dimensions are off, and output objects should not be used at this point anyway
            'InitEcospaceOutputs()
            'InitEcotracerOutputs()

            SpatialDataConnectionManager.Load()

            bSuccess = bSuccess And Me.CapacityMapInteractionManager.Load()
            bSuccess = bSuccess And Me.MortalityMapInteractionManager.Load()


            Me.m_EcospaceStats = New cEcospaceStats(Me, cCore.NULL_VALUE)
            'For debugging add the RelCin Layer to the Capacity maps
            'you have to turn On the Contaminant tracer to edit the RelCin map
            'Me.m_mapInteractionManager.AddMap(Me.m_EcoSpaceData.RelCin, "Relative Contaminants")

            ' Make sure Ecospace is ready to deal with further parameterization based on loaded data.
            Me.m_Ecospace.Load()

            SendEcospaceLoadMessage(strScenarioName)

            If Not cCore.USE_SHARED_ARENAS Then
                ' JS 25oct'23: This bandaid solution compensates for Ecospace not getting the shared arenas right
                Me.m_ArenaManager.ResetArenas(0)
            End If

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.EcospaceLoadScenario(ds)
                Me.PluginManager.EcospaceInitialized(Me.m_EcospaceData)
            End If

            ' Update core state
            Me.m_StateMonitor.SetEcospaceLoaded(bSuccess)

        Catch ex As Exception
            m_logger.LogError("LoadEcospaceScenario(...) Error: {message}", ex.Message)
            SendEcospaceLoadMessage(strScenarioName, ex.Message)
            Debug.Assert(False, ex.Message)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    Public Sub CloseEcospaceScenario()

        'If (Not Me.StateMonitor.HasEcospaceLoaded) Then Return
        Try

            If (Me.m_AdvectionManager IsNot Nothing) Then
                Me.m_AdvectionManager.Clear()
            End If
            '' Discard advection IO object
            'If (Me.m_AdvectionParameters IsNot Nothing) Then
            '    Me.m_AdvectionParameters.Dispose()
            '    Me.m_AdvectionParameters = Nothing
            'End If

            'Me.m_EcoSpaceScenarios.Clear()
            'Me.m_EcoPathData.NumEcospaceScenarios = 0

            Me.m_EcospaceFleets.Clear()
            Me.m_EcospaceFleetOutputs.Clear()
            Me.m_EcospaceGroups.Clear()
            Me.m_EcospaceGroupOuputs.Clear()

            Me.m_EcospaceHabitats.Clear()
            Me.m_EcospaceMPAs.Clear()

            Me.m_EcospaceMPAs.Clear()
            Me.m_EcospaceRegions.Clear()
            Me.m_EcospaceRegionSummaries.Clear()
            Me.m_mapInteractionManager.Clear()

            Me.m_EcospaceModelParams = Nothing
            Me.m_Ecospace.Clear()

            'delegates
            Me.m_SpaceInterfaceCallBacks.Clear()
            Me.m_Ecospace.TimeStepDelegate = Nothing

            Me.m_EcopathData.ActiveEcospaceScenario = -1
            Me.m_StateMonitor.SetEcospaceLoaded(False)

            Me.m_EcospaceTimeSeriesManager.Clear()

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.EcospaceRunInvalidated()
                Me.PluginManager.EcospaceCloseScenario()
            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".CloseEcoSpaceScenario() Exception: " & ex.Message)
            m_logger.LogError("CloseEcoSpaceScenario() Exception: {message}", ex.Message)
        End Try

        m_logger.LogInformation("Ecospace scenario closed")

    End Sub

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
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Overwrite scenario?
        iScenarioID = m_EcopathData.EcospaceScenarioDBID(ActiveEcospaceScenarioIndex)
        Debug.Assert(iScenarioID > 0)

        ds = DirectCast(DataSource, IEcospaceDatasource)
        ' No need to save? Yippee
        If Not ds.IsEcospaceModified Then Return True
        ' Save ok?
        If (ds.SaveEcospaceScenario(iScenarioID)) Then

            ' #Yes: reload ecospace scenario defs
            Me.InitEcospaceScenarios()
            ' Update active scenario ID
            Me.m_EcopathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcopathData.EcospaceScenarioDBID, iScenarioID)

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcospaceScenario(Me.DataSource)
            ' Force update
            Me.m_StateMonitor.SetEcospaceLoaded(True, TriState.True)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Report succes
            Me.SendEcospaceSaveStateMessage(Me.m_EcopathData.EcospaceScenarioName(Me.ActiveEcospaceScenarioIndex))
            Return True
        End If

        ' Restore previous active scenario ID on save failure
        Me.m_EcopathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcopathData.EcospaceScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcospaceSaveStateMessage(Me.m_EcopathData.EcospaceScenarioName(Me.ActiveEcospaceScenarioIndex), False,
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
    Public Function SaveEcospaceScenarioAs(strName As String,
                                           strDescription As String) As Boolean

        Dim epd As cEcopathDataStructures = Me.m_EcopathData
        Dim esd As cEcospaceDataStructures = Me.m_EcospaceData
        Dim ds As IEcospaceDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcospaceDatasource) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False

        iScenarioID = Me.m_EcopathData.EcospaceScenarioDBID(Me.ActiveEcospaceScenarioIndex)
        If (iScenarioID <= 0) Then Return bSucces

        ' Clear duplicates
        Me.RemoveEcospaceScenario(Me.FindObjectByName(Me.m_EcospaceScenarios, strName))

        ds = DirectCast(DataSource, IEcospaceDatasource)
        ' Save ok?
        If (ds.SaveEcospaceScenarioAs(strName, strDescription,
                epd.EcospaceScenarioAuthor(Me.ActiveEcospaceScenarioIndex),
                epd.EcospaceScenarioContact(Me.ActiveEcospaceScenarioIndex),
                iScenarioID)) Then

            ' #Yes: invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcospaceScenario(ds)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)

            ' Reload scenarios
            Me.InitEcospaceScenarios()

            ' Inform the world
            Me.SendEcospaceSaveStateMessage(strName)
            ' Load Ecospace scenario
            bSucces = Me.LoadEcospaceScenario(Array.IndexOf(epd.EcospaceScenarioDBID, iScenarioID))
            Me.DataAddedOrRemovedMessage("Ecospace number of scenarios has changed.", eCoreComponentType.Ecospace, eDataTypes.EcoSpaceScenario)
            Return bSucces
        End If

        ' Restore previous active scenario ID on save failure
        Me.m_EcopathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcopathData.EcospaceScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcospaceSaveStateMessage(strName, bSucces)
        Return bSucces
    End Function

    ''' <summary>
    ''' Remove a <see cref="cEcoSpaceScenario">Ecospace Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcoSpaceScenario">Scenario</see> to remove.</param>
    ''' <returns>True if successful.</returns>
    Public Function RemoveEcospaceScenario(scenario As cCoreInputOutputBase) As Boolean
        If (scenario Is Nothing) Then Return True
        If (Not TypeOf (scenario) Is cEcospaceScenario) Then Return False
        Return Me.RemoveEcospaceScenario(scenario.Index)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cEcoSpaceScenario">Ecospace Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario in the <see cref="m_EcospaceScenarios">Ecospace Scenario list</see>.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceScenario(iScenario As Integer) As Boolean

        Dim ds As IEcospaceDatasource = Nothing
        Dim iScenarioIDDeleted As Integer = Me.m_EcopathData.EcospaceScenarioDBID(iScenario)
        Dim iScenarioID As Integer = cCore.NULL_VALUE ' Scenario to restore
        Dim bSucces As Boolean = False

        ' Sanity check
        Debug.Assert(iScenario > 0 And iScenario < Me.m_EcopathData.EcospaceScenarioDBID.Length)

        ' Cannot delete a loaded scenario
        If (iScenario = Me.ActiveEcospaceScenarioIndex) Then
            Me.m_publisher.SendMessage(New cMessage(My.Resources.CoreMessages.SCENARIO_DELETE_LOADED,
                                                    eMessageType.NotSet, eCoreComponentType.DataSource,
                                                    eMessageImportance.Warning))
            Return False
        End If

        ' Save pending relevant changes
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecospace) Then Return False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcospaceDatasource) Then Return False

        ' Remember scenario ID to restore
        If (Me.ActiveEcospaceScenarioIndex > 0) Then
            iScenarioID = Me.m_EcopathData.EcospaceScenarioDBID(Me.ActiveEcospaceScenarioIndex)
        End If

        ds = DirectCast(Me.DataSource, IEcospaceDatasource)
        ' Scenario removed succesfully?
        If ds.RemoveEcospaceScenario(iScenarioIDDeleted) Then
            ' #Yes: reload scenario list
            bSucces = Me.InitEcospaceScenarios()
            ' Restore active scenario ID
            Me.m_EcopathData.ActiveEcospaceScenario = Array.IndexOf(Me.m_EcopathData.EcospaceScenarioDBID, iScenarioID)

            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.EcospaceScenarioRemoved(Me.DataSource, iScenarioIDDeleted)
            End If

            ' Broadcast change
            Me.DataAddedOrRemovedMessage("Ecospace number of scenarios has changed.", eCoreComponentType.Ecospace, eDataTypes.EcoSpaceScenario)
        End If
        ' Return succes
        Return bSucces
    End Function


    ''' <summary>
    ''' Update all the underlying data structures that contain Ecospace scenario data
    ''' </summary>
    ''' <returns>True if successful.</returns>
    Private Function UpdateEcospaceScenario(iDBID As Integer) As Boolean

        Dim iScenario As Integer = Array.IndexOf(Me.m_EcopathData.EcospaceScenarioDBID, iDBID)
        Dim scn As cEcospaceScenario = Me.EcospaceScenarios(iScenario)

        Try
            Me.m_EcopathData.EcospaceScenarioName(iScenario) = scn.Name
            Me.m_EcopathData.EcospaceScenarioDescription(iScenario) = scn.Description
            Me.m_EcopathData.EcospaceScenarioAuthor(iScenario) = scn.Author
            Me.m_EcopathData.EcospaceScenarioContact(iScenario) = scn.Contact

        Catch ex As Exception
            m_logger.LogError("UpdateEcoSpaceScenario() Error: {message}", ex.Message)
            Return False
        End Try

        Return True

    End Function

#End Region ' Scenarios

#Region " Model parameters "

    Private Function InitEcospaceModelParameters() As Boolean
        'there is only one cEcospaceModelParameters object 
        Try
            Me.m_EcospaceModelParams = New cEcospaceModelParameters(Me, m_EcopathData.EcospaceScenarioDBID(ActiveEcospaceScenarioIndex))
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
            m_EcospaceModelParams.PredictEffort = m_EcospaceData.PredictEffort
            m_EcospaceModelParams.NumberOfTimeStepsPerYear = CSng(1.0 / m_EcospaceData.TimeStep)

            m_EcospaceModelParams.AdjustSpace = m_EcospaceData.AdjustSpace

            m_EcospaceModelParams.StartSummaryTime = CInt(m_EcospaceData.SumStart(0))
            m_EcospaceModelParams.EndSummaryTime = CInt(m_EcospaceData.SumStart(1))
            m_EcospaceModelParams.NumberSummaryTimeSteps = m_EcospaceData.NumStep
            m_EcospaceModelParams.nRegions = m_EcospaceData.nRegions
            m_EcospaceModelParams.nEffortZones = m_EcospaceData.nEffZones

            'Grid threads
            m_EcospaceModelParams.nGridSolverThreads = m_EcospaceData.nGridSolverThreads

            'Group Threads
            m_EcospaceModelParams.nSpaceThreads = m_EcospaceData.nSpaceSolverThreads

            'Effort dist threads
            m_EcospaceModelParams.nEffortDistThreads = m_EcospaceData.nEffortDistThreads

            m_EcospaceModelParams.IFDPower = m_EcospaceData.IFDPower
            m_EcospaceModelParams.UseIBM = m_EcospaceData.UseIBM
            m_EcospaceModelParams.UseNewMultiStanza = m_EcospaceData.NewMultiStanza
            m_EcospaceModelParams.TotalTime = CInt(m_EcospaceData.TotalTime)

            m_EcospaceModelParams.Tolerance = m_EcospaceData.Tol
            m_EcospaceModelParams.SOR = m_EcospaceData.W
            m_EcospaceModelParams.MaxNumberOfIterations = m_EcospaceData.maxIter
            m_EcospaceModelParams.UseExact = m_EcospaceData.UseExact

            m_EcospaceModelParams.UseAnnualOuput = m_EcospaceData.SaveAnnual
            m_EcospaceModelParams.UseCoreOutputDirectory = m_EcospaceData.UseCoreOutputDir
            m_EcospaceModelParams.AutosaveSelectedGroupsFleetsOnly = m_EcospaceData.SaveSelectedGroupsFleetsOnly

            m_EcospaceModelParams.IBMMovePacketOnStanza = m_EcospaceData.MovePacketsAtStanzaEntry

            'm_EcospaceModelParams.CapacityCalculationType = m_EcoSpaceData.CapCalType

            m_EcospaceModelParams.PacketsMultiplier = Me.m_Stanza.NPacketsMultiplier

            m_EcospaceModelParams.UseEffortDistThreshold = Me.m_EcospaceData.UseEffortDistThreshold
            m_EcospaceModelParams.EffortDistThreshold = Me.m_EcospaceData.EffortDistThreshold

            m_EcospaceModelParams.UseHabCapGradientCorrections = Me.m_EcospaceData.UseHabCapGradientCorrections
            m_EcospaceModelParams.MinForagingCapacity = Me.m_EcospaceData.MinHabCap

            m_EcospaceModelParams.SpinupEnabled = Me.m_EcospaceData.UseSpinUp
            m_EcospaceModelParams.SpinupYears = Me.m_EcospaceData.SpinUpYears

            m_EcospaceModelParams.EcospaceAreaOutputDir = Me.m_EcospaceData.EcospaceAreaOutputDir
            m_EcospaceModelParams.EcospaceMapOutputDir = Me.m_EcospaceData.EcospaceMapOutputDir

            m_EcospaceModelParams.FirstOutputTimeStep = Me.m_EcospaceData.FirstOutputTimeStep

            m_EcospaceModelParams.UseEcosimBiomassForcing = Me.m_EcospaceData.UseEcosimBiomassForcing
            m_EcospaceModelParams.UseEcosimDiscardForcing = Me.m_EcospaceData.UseEcosimDiscardForcing

            m_EcospaceModelParams.IsEcosimBiomassForcingLoaded = Me.m_EcospaceData.isEcosimBiomassForcingLoaded
            m_EcospaceModelParams.IsEcosimDiscardForcingLoaded = Me.m_EcospaceData.isEcosimDiscardForcingLoaded

            m_EcospaceModelParams.SaveThreadingLog = m_EcospaceData.bSaveThreadingLog
            m_EcospaceModelParams.nIBMMovementThreads = m_EcospaceData.nIBMMovementSolverThreads

            m_EcospaceModelParams.UseSpatialEffortPenalty = m_EcospaceData.DoPenaltysearch
            m_EcospaceModelParams.PenaltyPower = m_EcospaceData.PenPow
            m_EcospaceModelParams.AdjustEffortWeight = m_EcospaceData.NoFishWeight
            m_EcospaceModelParams.FirstPenaltyMonth = m_EcospaceData.FirstPenaltyMonth
            m_EcospaceModelParams.EffortRelaxationWeight = m_EcospaceData.EffortRelaxationWeight

            Me.LoadEcospaceResultsWriters()

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

        m_EcospaceData.PredictEffort = m_EcospaceModelParams.PredictEffort

        m_EcospaceData.TimeStep = CDbl(1.0 / m_EcospaceModelParams.NumberOfTimeStepsPerYear)

        m_EcospaceData.SumStart(0) = m_EcospaceModelParams.StartSummaryTime
        m_EcospaceData.SumStart(1) = m_EcospaceModelParams.EndSummaryTime
        m_EcospaceData.NumStep = m_EcospaceModelParams.NumberSummaryTimeSteps
        m_EcospaceData.AdjustSpace = m_EcospaceModelParams.AdjustSpace
        m_EcospaceData.nRegions = m_EcospaceModelParams.nRegions
        m_EcospaceData.nEffZones = m_EcospaceModelParams.nEffortZones

        m_EcospaceData.nGridSolverThreads = m_EcospaceModelParams.nGridSolverThreads
        m_EcospaceData.nEffortDistThreads = m_EcospaceModelParams.nEffortDistThreads
        m_EcospaceData.nSpaceSolverThreads = m_EcospaceModelParams.nSpaceThreads

        m_EcospaceData.nIBMMovementSolverThreads = m_EcospaceModelParams.nIBMMovementThreads

        m_EcospaceData.IFDPower = m_EcospaceModelParams.IFDPower
        m_EcospaceData.UseIBM = m_EcospaceModelParams.UseIBM
        m_EcospaceData.NewMultiStanza = m_EcospaceModelParams.UseNewMultiStanza
        m_EcospaceData.TotalTime = m_EcospaceModelParams.TotalTime

        m_EcospaceData.Tol = m_EcospaceModelParams.Tolerance
        m_EcospaceData.W = m_EcospaceModelParams.SOR
        m_EcospaceData.maxIter = m_EcospaceModelParams.MaxNumberOfIterations
        m_EcospaceData.UseExact = m_EcospaceModelParams.UseExact

        m_EcospaceData.SaveAnnual = m_EcospaceModelParams.UseAnnualOuput
        m_EcospaceData.UseCoreOutputDir = m_EcospaceModelParams.UseCoreOutputDirectory
        m_EcospaceData.SaveSelectedGroupsFleetsOnly = m_EcospaceModelParams.AutosaveSelectedGroupsFleetsOnly

        m_EcospaceData.MovePacketsAtStanzaEntry = m_EcospaceModelParams.IBMMovePacketOnStanza

        m_Stanza.NPacketsMultiplier = m_EcospaceModelParams.PacketsMultiplier

        m_tracerData.EcoSpaceConSimOn = m_EcospaceModelParams.ContaminantTracing
        m_EcospaceData.UseSpinUp = m_EcospaceModelParams.SpinupEnabled
        m_EcospaceData.SpinUpYears = m_EcospaceModelParams.SpinupYears

        m_EcospaceData.UseEffortDistThreshold = m_EcospaceModelParams.UseEffortDistThreshold
        m_EcospaceData.EffortDistThreshold = m_EcospaceModelParams.EffortDistThreshold

        m_EcospaceData.UseHabCapGradientCorrections = m_EcospaceModelParams.UseHabCapGradientCorrections
        m_EcospaceData.MinHabCap = m_EcospaceModelParams.MinForagingCapacity

        m_EcospaceData.EcospaceAreaOutputDir = m_EcospaceModelParams.EcospaceAreaOutputDir
        m_EcospaceData.EcospaceMapOutputDir = m_EcospaceModelParams.EcospaceMapOutputDir

        m_EcospaceData.FirstOutputTimeStep = m_EcospaceModelParams.FirstOutputTimeStep

        m_EcospaceData.UseEcosimBiomassForcing = m_EcospaceModelParams.UseEcosimBiomassForcing
        m_EcospaceData.UseEcosimDiscardForcing = m_EcospaceModelParams.UseEcosimDiscardForcing

        m_EcospaceData.bSaveThreadingLog = m_EcospaceModelParams.SaveThreadingLog

        m_EcospaceData.DoPenaltysearch = m_EcospaceModelParams.UseSpatialEffortPenalty
        m_EcospaceData.PenPow = m_EcospaceModelParams.PenaltyPower
        m_EcospaceData.NoFishWeight = m_EcospaceModelParams.AdjustEffortWeight
        m_EcospaceData.FirstPenaltyMonth = m_EcospaceModelParams.FirstPenaltyMonth
        m_EcospaceData.EffortRelaxationWeight = m_EcospaceModelParams.EffortRelaxationWeight

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Change the size of the Ecospace base map to a new number of rows and columns.
    ''' </summary>
    ''' <param name="InRow"></param>
    ''' <param name="InCol"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function ResizeEcospaceBasemap(InRow As Integer, InCol As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        If Not Me.SaveChanges(False) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.ResizeEcospaceBasemap(InRow, InCol) Then
            ' ToDo: Take decisive action on existing Ecospace data.
            ' - Ask users to erase?
            ' - Interpret the coordinates, cell sizes etc and resample?
            ' - etc?

            ' Save Ecospace
            Me.SaveChanges(True, eBatchChangeLevelFlags.Ecospace)
            ' Reload the scenario
            If Me.LoadEcospaceScenario(Me.ActiveEcospaceScenarioIndex) Then

                ' Egg
                Dim r As New Random()
                If CInt(r.NextDouble * 42) = 13 Then
                    Me.m_publisher.AddMessage(New cMessage("Map has been resized; a tsunami warning has been issued.",
                        eMessageType.NotSet, eCoreComponentType.Ecospace, eMessageImportance.Information))
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
                .InCol = m_EcospaceData.InCol
                .InRow = m_EcospaceData.InRow
                .CellLength = m_EcospaceData.CellLength
                .Latitude = m_EcospaceData.Lat1 'UDH_UL
                .Longitude = m_EcospaceData.Lon1
                .AssumeSquareCells = m_EcospaceData.AssumeSquareCells
                .ProjectionString = m_EcospaceData.ProjectionString

                .nCells = m_EcospaceData.ThabArea
                .ResetStatusFlags()
                .AllowValidation = True
            End With

            ' Load layers that do not get initialized by the basemap
            Me.LoadEcospaceDepthLayer()
            Me.LoadEcospaceImportanceLayers()
            Me.LoadEcospaceDriverLayers()
            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Private Sub LoadEcospaceDepthLayer()

        Dim dest As cEcospaceLayerDepth = Me.m_EcospaceBasemap.LayerDepth
        dest.AllowValidation = False
        dest.IsCapacityEnabled = Not Me.m_EcospaceData.EnvironmentalLayerCapacityDisabled(0)
        dest.AllowValidation = True

    End Sub

    Private Sub LoadEcospaceImportanceLayers()

        Dim dest As cEcospaceLayerImportance = Nothing
        For i As Integer = 1 To Me.m_EcospaceData.nImportanceLayers
            dest = Me.m_EcospaceBasemap.LayerImportance(i)
            dest.AllowValidation = False
            dest.Index = i
            dest.Weight = Me.m_EcospaceData.ImportanceLayerWeight(i)
            dest.Name = Me.m_EcospaceData.ImportanceLayerName(i)
            dest.Description = Me.m_EcospaceData.ImportanceLayerDescription(i)
            dest.AllowValidation = True
        Next i

    End Sub

    Private Sub LoadEcospaceDriverLayers()

        Dim dest As cEcospaceLayerDriver = Nothing
        For i As Integer = 1 To Me.m_EcospaceData.nEnvironmentalDriverLayers
            dest = Me.m_EcospaceBasemap.LayerDriver(i)
            dest.AllowValidation = False
            dest.Index = i
            dest.Name = Me.m_EcospaceData.EnvironmentalLayerName(i)
            dest.Description = Me.m_EcospaceData.EnvironmentalLayerDescription(i)
            dest.Units = Me.m_EcospaceData.EnvironmentalLayerUnits(i)
            dest.IsCapacityEnabled = Not Me.m_EcospaceData.EnvironmentalLayerCapacityDisabled(i)
            dest.AllowValidation = True
        Next i

    End Sub

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

            Me.m_EcospaceData.CellLength = m_EcospaceBasemap.CellLength
            Me.m_EcospaceData.AssumeSquareCells = Me.m_EcospaceBasemap.AssumeSquareCells
            Me.m_EcospaceData.ProjectionString = Me.m_EcospaceBasemap.ProjectionString
            Me.m_EcospaceData.Lat1 = m_EcospaceBasemap.Latitude
            Me.m_EcospaceData.Lon1 = m_EcospaceBasemap.Longitude

            Me.UpdateEcospaceDepthLayer()
            Me.UpdateEcospaceImportanceLayers()
            Me.UpdateEcospaceDriverLayers()

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

    Private Sub UpdateEcospaceDepthLayer()
        Me.m_EcospaceData.EnvironmentalLayerCapacityDisabled(0) = Not Me.m_EcospaceBasemap.LayerDepth.IsCapacityEnabled
    End Sub

    Private Sub UpdateEcospaceImportanceLayers()

        Dim src As cEcospaceLayerImportance = Nothing

        For i As Integer = 1 To Me.m_EcospaceData.nImportanceLayers
            src = Me.m_EcospaceBasemap.LayerImportance(i)
            Me.m_EcospaceData.ImportanceLayerName(i) = src.Name
            Me.m_EcospaceData.ImportanceLayerDescription(i) = src.Description
            Me.m_EcospaceData.ImportanceLayerWeight(i) = src.Weight
        Next i
    End Sub

    Private Sub UpdateEcospaceDriverLayers()

        Dim src As cEcospaceLayerDriver = Nothing

        For i As Integer = 1 To Me.m_EcospaceData.nEnvironmentalDriverLayers
            src = Me.m_EcospaceBasemap.LayerDriver(i)
            Me.m_EcospaceData.EnvironmentalLayerName(i) = src.Name
            Me.m_EcospaceData.EnvironmentalLayerDescription(i) = src.Description
            Me.m_EcospaceData.EnvironmentalLayerUnits(i) = src.Units
            Me.m_EcospaceData.EnvironmentalLayerCapacityDisabled(i) = Not src.IsCapacityEnabled
        Next i

    End Sub

#End Region ' Basemap

#Region " Groups "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize <see cref="cEcospaceGroupInput">Ecospace group</see> objects to
    ''' expose to the interface layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function InitEcospaceGroups() As Boolean

        Dim grp As cEcospaceGroupInput = Nothing

        Try

            m_EcospaceGroups.Clear()

            'populate the list of cEcoSimGroupInfo objects that the user will interact with 
            'to change group related parameters from the interface see getEcoSimGroupInfo(iGroup)
            For i As Integer = 1 To nGroups
                ' Create group
                grp = New cEcospaceGroupInput(Me, Me.m_EcospaceData.GroupDBID(i))
                ' Add to list
                m_EcospaceGroups.Add(grp)
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

            For Each grp As cEcospaceGroupInput In Me.m_EcospaceGroups

                'convert the Database ID into an iGroup
                iGroup = Array.IndexOf(m_EcospaceData.GroupDBID, grp.DBID)
                grp.Index = iGroup

                Debug.Assert(iGroup > 0 And iGroup <= Me.nGroups, "LoadEcospaceGroups() failed to find iGroup for Ecospace DBID.")

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                grp.Resize()

                grp.AllowValidation = False

                grp.Name = m_EcopathData.GroupName(iGroup)
                'Mvel
                grp.SetVariable(eVarNameFlags.MVel, m_EcospaceData.Mvel(iGroup))

                grp.RelMoveBad = m_EcospaceData.RelMoveBad(iGroup)
                grp.RelVulBad = m_EcospaceData.RelVulBad(iGroup)
                grp.KMoveFitness = m_EcospaceData.Kmovefit(iGroup)
                grp.IsMigratory = m_EcospaceData.IsMigratory(iGroup)
                grp.IsAdvected = m_EcospaceData.IsAdvected(iGroup)
                grp.BarrierAvoidanceWeight = m_EcospaceData.barrierAvoidanceWeight(iGroup)
                grp.PP = m_EcopathData.PP(iGroup)
                grp.CapacityCalculationType = m_EcospaceData.CapCalType(iGroup)

                grp.InMigrationAreaMovement = m_EcospaceData.InMigAreaMovement(iGroup)
                grp.FTarget = m_EcospaceData.Ftarget(iGroup)

                For i = 0 To nHabitats - 1
                    grp.PreferredHabitat(i) = m_EcospaceData.PrefHab(iGroup, i)
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

    Private Function UpdateEcospaceGroup(iDBID As Integer) As Boolean

        Dim grp As cEcospaceGroupInput = Nothing
        Dim iGroup As Integer
        Dim i As Integer

        Try

            ' Convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcospaceData.GroupDBID, iDBID)
            ' Get the group
            grp = Me.EcospaceGroupInputs(iGroup)

            ' Suck it empty
            m_EcospaceData.Mvel(iGroup) = grp.MVel

            m_EcospaceData.RelMoveBad(iGroup) = grp.RelMoveBad
            m_EcospaceData.RelVulBad(iGroup) = grp.RelVulBad
            m_EcospaceData.Kmovefit(iGroup) = grp.KMoveFitness
            m_EcospaceData.IsAdvected(iGroup) = grp.IsAdvected
            m_EcospaceData.IsMigratory(iGroup) = grp.IsMigratory
            m_EcospaceData.barrierAvoidanceWeight(iGroup) = grp.BarrierAvoidanceWeight
            m_EcospaceData.CapCalType(iGroup) = grp.CapacityCalculationType

            m_EcospaceData.InMigAreaMovement(iGroup) = grp.InMigrationAreaMovement

            m_EcospaceData.Ftarget(iGroup) = grp.FTarget

            For i = 0 To nHabitats - 1
                m_EcospaceData.PrefHab(iGroup, i) = grp.PreferredHabitat(i)
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
                m_EcospaceGroupOuputs.Add(New cEcospaceGroupOutput(Me, Me.m_EcospaceData, igrp))
            Next

            'this includes zero index 'Combined Fleets' 
            For iflt As Integer = 0 To nFleets 'this includes the 'Combined Fleets' 
                Me.m_EcospaceFleetOutputs.Add(New cEcospaceFleetOutput(Me, Me.m_EcospaceData, iflt))
            Next

            'This will include the zero indexed region 
            'the zero index holds the data not include in one of the other regions (OR)
            'It is NOT like the Fleets where the zero index in the combined values (AND)
            For iRgn As Integer = 0 To nRegions
                Me.m_EcospaceRegionSummaries.Add(New cEcospaceRegionOutput(Me, Me.m_EcospaceData, iRgn))
            Next

            'load a new results object for the new scenario
            m_spaceresults = New cEcospaceTimestep(Me, Me.m_EcoSimData, Me.m_EcospaceData, Me.m_Stanza)


            'in the other InitEcospacexxxx the data is loaded during the init
            'for the output LoadEcospaceResults() is not called until the model has successfully run 

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcospaceOutputs() Error: " & ex.Message)
        End Try
    End Sub


    Private Sub initEcospaceResultsWriters()
        'Output writing
        For iWriter As Integer = 1 To Me.m_EcospaceModelParams.nResultWriters
            Try
                Dim writer As IEcospaceResultsWriter = Me.m_EcospaceModelParams.ResultWriter(iWriter)
                If (writer.Enabled) Then
                    writer.Init(Me)
                    writer.StartWrite()
                End If
            Catch ex As Exception
                System.Console.WriteLine("Exception in " + Me.ToString + ".initEcospaceResultsWriters()")
            End Try
        Next
    End Sub


    Private Sub LoadEcospaceResults()
        'see cEcoSpace.ScaleAfterNumStep(), summarizeCatchData() and summarizeTimeStepData()
        Dim iflt As Integer
        Dim igrp As Integer
        Dim stVal As Single, endVal As Single

        Try

            'Spatial results are averaged over space by Ecospace in cEcoSpaceDataStructures.AverageSpatialResults()

            'Fleet summarized output
            For Each flt As cEcospaceFleetOutput In m_EcospaceFleetOutputs

                'loads results over time
                flt.Init()

                If flt.Index <> 0 Then
                    flt.Name = m_EcopathData.FleetName(flt.Index)
                Else
                    flt.Name = My.Resources.CoreDefaults.CORE_DEFAULT_COMBINEDFLEETS
                End If

                m_EcospaceData.getSumCatchFleet(flt.Index, stVal, endVal)
                flt.CatchStart = stVal
                flt.CatchEnd = endVal

                m_EcospaceData.getSumCostFleet(Me.m_EcopathData.cost, flt.Index, stVal, endVal)
                flt.CostStart = stVal
                flt.CostEnd = endVal

                m_EcospaceData.getSumValueFleet(flt.Index, stVal, endVal)
                flt.ValueStart = stVal
                flt.ValueEnd = endVal


                m_EcospaceData.getSumEffortES(flt.Index, stVal)
                flt.EffortES = stVal

            Next flt

            For Each rgn As cEcospaceRegionOutput In m_EcospaceRegionSummaries
                rgn.Resize()

                'init the core data arrays
                rgn.Init()

                If rgn.Index <> 0 Then
                    rgn.Name = "Region " & rgn.Index.ToString()
                Else
                    rgn.Name = "Undefined Area"
                End If

                'average the data over the number of cells in the region for output
                Dim nCellsInRegion As Integer = m_EcospaceData.RegionCells(rgn.Index)
                If nCellsInRegion = 0 Then nCellsInRegion = 1

                For igrp = 1 To nGroups

                    Dim sbio As Single, ebio As Single
                    m_EcospaceData.getSumBiomByRegion(rgn.Index, igrp, sbio, ebio)
                    rgn.BiomassStart(igrp) = sbio
                    rgn.BiomassEnd(igrp) = ebio

                    For iflt = 0 To nFleets
                        Dim sCatch As Single, eCatch As Single
                        m_EcospaceData.getSumCatchRegionGearGroup(rgn.Index, iflt, igrp, sCatch, eCatch)
                        '  Debug.Assert(sCatch = 0)
                        rgn.CatchFleetGroupStart(iflt, igrp) = sCatch
                        rgn.CatchFleetGroupEnd(iflt, igrp) = eCatch
                    Next iflt

                Next igrp


            Next rgn

            For Each grp As cEcospaceGroupOutput In m_EcospaceGroupOuputs
                'init the object to the underlying ecospace data
                grp.Init()
                grp.ResetStatusFlags()
                grp.Name = m_EcopathData.GroupName(grp.Index)
                grp.PP = m_EcopathData.PP(grp.Index)

                m_EcospaceData.getSumBiom(grp.Index, stVal, endVal)
                grp.BiomassStart = stVal
                grp.BiomassEnd = endVal
                For iflt = 0 To nFleets
                    m_EcospaceData.getSumCatchFleetGroup(iflt, grp.Index, stVal, endVal)
                    grp.CatchStart(iflt) = stVal
                    grp.CatchEnd(iflt) = endVal

                    m_EcospaceData.getSumValueFleetGroup(iflt, grp.Index, stVal, endVal)
                    grp.ValueStart(iflt) = stVal
                    grp.ValueEnd(iflt) = endVal
                Next iflt

            Next

            'Populate the Stats objects 
            Me.m_EcospaceStats.SS = Me.m_EcospaceTimeSeriesManager.SS
            'if the manager contains data then SS has been calculated
            'this doesn't mean it isn't zero!
            Me.m_EcospaceStats.isSSCalculated = Me.m_EcospaceTimeSeriesManager.ContainsData(eVarNameFlags.EcospaceMapBiomass)
            For igrp = 1 To Me.nGroups
                Me.m_EcospaceStats.SSGroup(igrp) = Me.m_EcospaceTimeSeriesManager.SSGroup(igrp)
            Next

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
                objHab = New cEcospaceHabitat(Me, Me.m_EcospaceData.HabitatDBID(i))
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
                Debug.Assert(iHab = Array.IndexOf(m_EcospaceData.HabitatDBID, objHab.DBID), "LoadEcospaceHabitats() detected Index inconsistency")

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                objHab.Resize()

                objHab.AllowValidation = False

                objHab.Name = m_EcospaceData.HabitatText(iHab)
                objHab.HabAreaProportion = m_EcospaceData.HabAreaProportion(iHab)

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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function UpdateEcospaceHabitat(iDBID As Integer) As Boolean
        Dim objHab As cEcospaceHabitat = Nothing
        Dim iHabitat As Integer = Array.IndexOf(Me.m_EcospaceData.HabitatDBID, iDBID)

        ' Sanity check
        Debug.Assert(iHabitat > 0)
        Debug.Assert(Me.nHabitats >= iHabitat)

        Try
            ' Get the object
            objHab = DirectCast(Me.m_EcospaceHabitats(iHabitat), cEcospaceHabitat)

            m_EcospaceData.HabitatText(iHabitat) = objHab.Name
            m_EcospaceData.HabAreaProportion(iHabitat) = objHab.HabAreaProportion

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
    ''' <param name="strName">Name of habitat to add.</param>
    ''' <param name="iIndex">Sequential index for the new habitat.</param>
    ''' <param name="iDBID">DBID of the habitat.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceHabitat(strName As String, iIndex As Integer, ByRef iDBID As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AddEcospaceHabitat(strName, iIndex, iDBID) Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage(cStringUtils.Localize("Ecospace habitat {0} has been added", strName),
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
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
    ''' <param name="iDBID">The <see cref="cEcospaceHabitat.DBID"/> of the habitat 
    ''' to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceHabitat(iDBID As Integer) As Boolean

        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Not allowed to remove 'All' habitat
        If iDBID <= 0 Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceHabitat(iDBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace habitat has been removed",
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function

    Public Function MoveEcospaceHabitat(iHabitatID As Integer, iIndex As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.MoveHabitat(iHabitatID, iIndex) Then

            Me.DataModifiedMessage("Ecospace habitat order has changed.", eCoreComponentType.Ecospace, eDataTypes.EcospaceHabitat)
            Me.DataModifiedMessage("Ecospace habitat order has changed.", eCoreComponentType.Ecospace, eDataTypes.EcospaceLayerHabitat)
            bSucces = True

        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces

    End Function

#End Region ' Habitats

#Region " MPAs "

    Private Function InitEcospaceMPAs() As Boolean

        Try
            Dim objMPA As cEcospaceMPA = Nothing

            m_EcospaceMPAs.Clear()

            'populate the list of cEcospaceMPA objects that the user will interact with 
            'to change MPA related parameters from the interface
            For i As Integer = 1 To Me.nMPAs
                ' Create MPA
                objMPA = New cEcospaceMPA(Me, Me.m_EcospaceData.MPADBID(i))
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
                iMPA = Array.IndexOf(m_EcospaceData.MPADBID, objMPA.DBID)
                objMPA.Index = iMPA
                Debug.Assert(iMPA > 0 And iMPA <= Me.nMPAs, "LoadEcospaceMPAs() failed to find iMPA for Ecospace DBID.")

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                objMPA.Resize()

                objMPA.AllowValidation = False

                objMPA.Name = m_EcospaceData.MPAname(iMPA)
                For iMonth As Integer = 1 To N_MONTHS
                    objMPA.MPAMonth(iMonth) = m_EcospaceData.MPAmonth(iMonth, iMPA)
                Next iMonth

                objMPA.AllowValidation = True
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcospaceMPAs() Error: " & ex.Message)
            Return False
        End Try
        Return True
    End Function

    Private Function UpdateEcospaceMPA(iDBID As Integer) As Boolean
        Dim objMPA As cEcospaceMPA = Nothing
        Dim iMPA As Integer

        Try
            ' convert the Database ID into an index
            iMPA = Array.IndexOf(m_EcospaceData.MPADBID, iDBID)
            ' get the object
            objMPA = Me.EcospaceMPAs(iMPA)

            m_EcospaceData.MPAname(iMPA) = objMPA.Name
            For iMonth As Integer = 1 To N_MONTHS
                m_EcospaceData.MPAmonth(iMonth, iMPA) = objMPA.MPAMonth(iMonth)
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
    ''' <param name="iDBID"><see cref="cCoreInputOutputBase.DBID"/> of the new MPA.</param>
    ''' <param name="MPAMonths">One-based series of flags that indicate when the 
    ''' MPA is OPEN for fishing.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceMPA(strMPAName As String,
                                   iIndex As Integer,
                                   MPAMonths() As Boolean,
                                   ByRef iDBID As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim obj As cCoreInputOutputBase = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AddEcospaceMPA(strMPAName, iIndex, MPAMonths, iDBID) Then
            Me.m_publisher.AddMessage(New cMessage(cStringUtils.Localize("Ecospace MPA {0} has been added", strMPAName),
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
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
    ''' <param name="iMPADBID">The <see cref="cEcospaceMPA.DBID"/> to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceMPA(iMPADBID As Integer) As Boolean
        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceMPA(iMPADBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace MPA has been removed",
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function

    Public Function MoveEcospaceMPA(iDBID As Integer, iIndex As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.MoveEcospaceMPA(iDBID, iIndex) Then

            Me.DataModifiedMessage("Ecospace MPA order has changed.", eCoreComponentType.Ecospace, eDataTypes.EcospaceMPA)
            Me.DataModifiedMessage("Ecospace MPA order has changed.", eCoreComponentType.Ecospace, eDataTypes.EcospaceLayerMPA)
            bSucces = True

        End If

        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces

    End Function

#End Region ' MPAs

#Region " Fleets "

    Private Function InitEcospaceFleets() As Boolean

        Try
            Dim objFleet As cEcospaceFleetInput

            m_EcospaceFleets.Clear()

            'populate the list of cEcospaceHabitat objects that the user will interact with 
            'to change habitat related parameters from the interface
            For i As Integer = 1 To nFleets
                ' Create fleet
                objFleet = New cEcospaceFleetInput(Me, Me.m_EcospaceData.FleetDBID(i))
                ' Add to list
                m_EcospaceFleets.Add(objFleet)
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

            For Each fleet As cEcospaceFleetInput In Me.m_EcospaceFleets

                'convert the Database ID into an iGroup
                iFleet = Array.IndexOf(m_EcospaceData.FleetDBID, fleet.DBID)
                fleet.Index = iFleet
                Debug.Assert(iFleet >= 0 And iFleet <= Me.nFleets, cStringUtils.Localize("LoadEcospaceFleets() failed to find iFleet for Ecospace DBID {0}.", fleet.DBID))

                'this will call the cCore.getCounter() with the counter type
                'and only resize the arrays if getCounter() is different from the existing size
                fleet.Resize()

                fleet.AllowValidation = False

                fleet.Name = m_EcopathData.FleetName(iFleet)

                ' JS 04feb08: in sync with EwE5, this value is now read into space.EffPower
                'fleet.EffectivePower = m_EcoPathData.Epower(iFleet)
                fleet.EffectivePower = m_EcospaceData.EffPower(iFleet)
                fleet.TotalEffMultiplier = m_EcospaceData.SEmult(iFleet)

                For iHabitat As Integer = 0 To Me.nHabitats
                    fleet.HabitatFishery(iHabitat) = m_EcospaceData.GearHab(iFleet, iHabitat)
                Next
                For iMPA As Integer = 0 To Me.nMPAs
                    fleet.MPAFishery(iMPA) = m_EcospaceData.MPAfishery(iFleet, iMPA)
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

    Private Function UpdateEcospaceFleet(iDBID As Integer) As Boolean
        Dim fleet As cEcospaceFleetInput = Nothing
        Dim iFleet As Integer

        Try

            ' Convert the Database ID into an index
            iFleet = Array.IndexOf(m_EcospaceData.FleetDBID, iDBID)
            ' Get the object
            fleet = Me.EcospaceFleetInputs(iFleet)

            m_EcopathData.FleetName(iFleet) = fleet.Name
            ' JS 04feb08: in sync with EwE5, this value is now read into space.EffPower
            'm_EcoPathData.Epower(iFleet) = fleet.EffectivePower
            m_EcospaceData.EffPower(iFleet) = fleet.EffectivePower
            m_EcospaceData.SEmult(iFleet) = fleet.TotalEffMultiplier

            For iHabitat As Integer = 0 To Me.nHabitats
                m_EcospaceData.GearHab(iFleet, iHabitat) = fleet.HabitatFishery(iHabitat)
            Next
            For iMPA As Integer = 0 To Me.nMPAs
                m_EcospaceData.MPAfishery(iFleet, iMPA) = fleet.MPAFishery(iMPA)
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
    ''' <param name="iID">DBID that the data source has assigned to the new 
    ''' layer.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceImportanceLayer(strName As String, strDescription As String, sWeight As Single, ByRef iID As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AppendEcospaceImportanceLayer(strName, strDescription, sWeight, iID) Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage(cStringUtils.Localize("Ecospace importance layer {0} has been added", strName),
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceImportanceLayer(objLayer As cEcospaceLayerImportance) As Boolean
        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceImportanceLayer(objLayer.DBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace importance has been removed",
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function

#End Region ' Importance layers

#Region " Driver layers "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a <see cref="cEcospaceLayerDriver">driver layer</see> to the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="iDBID">DB id of the new map.</param>
    ''' <param name="strName">Name of layer to add.</param>
    ''' <param name="strDescription">Description of layer to add.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddEcospaceDriverLayer(strName As String, strDescription As String, strUnits As String, ByRef iDBID As Integer) As Boolean
        Dim ds As IEcospaceDatasource = Nothing
        Dim obj As cCoreInputOutputBase = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.AddEcospaceDriverLayer(strName, strDescription, strUnits, iDBID) Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage(cStringUtils.Localize("Ecospace driver layer {0} has been added", strName),
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
        Else
            bSucces = False
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cEcospaceLayerDriver">driver layer</see> from the current
    ''' <see cref="DataSource">data source</see>.
    ''' </summary>
    ''' <param name="iDBID">The <see cref="cCoreInputOutputBase.DBID"/> of the map
    ''' to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcospaceDriverLayer(iDBID As Integer) As Boolean
        Dim bsucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return False
        If (Not TypeOf (DataSource) Is IEcospaceDatasource) Then Return False

        ' Not allowed to delete 0 region (if any)
        If iDBID <= 0 Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        bsucces = ds.RemoveEcospaceDriverLayer(iDBID)

        If bsucces Then
            ' Broadcast update
            Me.m_publisher.AddMessage(New cMessage("Ecospace driver layer has been removed",
                eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecospace, eMessageImportance.Maintenance))
        End If

        ' Decrease batch count, stating what has changed
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bsucces
    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Move a <see cref="cEcospaceLayerDriver"/> to a new position in the EwE model.
    ''' </summary>
    ''' <param name="iLayer"><see cref="cCoreInputOutputBase.Index">One-based index of 
    ''' the environmental driver layer</see> to move.</param>
    ''' <param name="iIndex">New, one-based position of the layer in the layer list.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function MoveDriverLayer(iLayer As Integer, iIndex As Integer) As Boolean
        Dim bSucces As Boolean = False
        Dim ds As IEcospaceDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcospaceDatasource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcospaceDatasource)
        If ds.MoveEcospaceDriverLayer(Me.m_EcospaceData.EnvironmentalLayerDBID(iLayer), iIndex) Then

            Me.DataAddedOrRemovedMessage("Ecospace driver layer order has changed.", eCoreComponentType.Ecopath, eDataTypes.EcospaceLayerDriver)
            bSucces = True
        End If

        ' Decrease batch count
        ReleaseBatchLock(eBatchChangeLevelFlags.Ecospace)

        Return bSucces

    End Function

#End Region ' Driver layers

#Region " Advection "

    'Friend Function InitEcospaceAdvection() As Boolean
    '    Me.m_AdvectionParameters = New cAdvectionParameters(Me, -1)
    '    Return Me.m_AdvectionManager.Init
    'End Function

    Public ReadOnly Property AdvectionManager() As cAdvectionManager
        Get
            Return Me.m_AdvectionManager
        End Get
    End Property

    'Public ReadOnly Property AdvectionParameters() As cAdvectionParameters
    '    Get
    '        Return Me.m_AdvectionParameters
    '    End Get
    'End Property

#End Region ' Advection

#Region " Foraging Capacity "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Recalculate Ecospace foraging capacity.
    ''' </summary>
    ''' <param name="iGroup">The group to calculate capacity for. Set this value to 0 or less to 
    ''' calculate capacity for all groups.</param>
    ''' -----------------------------------------------------------------------
    Public Sub RecomputeEcospaceForagingCapacity(Optional iGroup As Integer = -1)
        If (Me.ActiveEcospaceScenarioIndex <= 0) Then Return
        Try
            Dim iMin As Integer = If(iGroup <= 0, 1, Math.Min(1, Math.Max(Me.nLivingGroups, iGroup)))
            Dim iMax As Integer = If(iGroup <= 0, Me.nLivingGroups, Math.Min(1, Math.Max(Me.nLivingGroups, iGroup)))
            For iGroup = iMin To iMax
                Me.m_EcospaceData.isGroupHabCapChanged(iGroup) = True
            Next
            Me.m_EcospaceData.isCapacityChanged = True
            Me.m_Ecospace.SetHabCap()
            Me.onChanged(Me.EcospaceBasemap.LayerHabitatCapacity(1))
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Foraging Capacity

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
    Friend Function LoadStanza(stanza As cStanzaGroup) As Boolean
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
            stanza.RecruitmentStanza = m_Stanza.RecStanza(iStanza)
            stanza.WmatWinf = m_Stanza.WmatWinf(iStanza)
            stanza.BiomassAccumulationRate = m_Stanza.BABsplit(iStanza)
            stanza.HatchCode = m_Stanza.HatchCode(iStanza)
            stanza.FixedFecundity = m_Stanza.FixedFecundity(iStanza)
            stanza.EggAtSpawn = m_Stanza.EggAtSpawn(iStanza)

            stanza.Age0Numbers = m_Stanza.RzeroS(iStanza) * 12

            'stanza.VBGF = m_EcoPathData.vbKInput(m_Stanza.EcopathCode(iStanza, m_Stanza.BaseStanza(iStanza)))

            ' Array variables
            For j As Integer = 1 To m_Stanza.Nstanza(iStanza)
                stanza.StartAge(j) = m_Stanza.Age1(iStanza, j)
                stanza.SpawnProp(j) = m_Stanza.SpawnProp(iStanza, j)
            Next

            For j As Integer = 1 To m_Stanza.Nstanza(iStanza)
                stanza.iGroups(j) = m_Stanza.EcopathCode(iStanza, j)
                stanza.Biomass(j) = m_EcopathData.Binput(m_Stanza.EcopathCode(iStanza, j))
                stanza.Mortality(j) = m_EcopathData.PBinput(m_Stanza.EcopathCode(iStanza, j))
                stanza.CB(j) = m_EcopathData.QBinput(m_Stanza.EcopathCode(iStanza, j))
            Next j

            For iage As Integer = 0 To stanza.MaxAge ' the MaxAge of a stanza group is not available until the Index has been set
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

            Me.updateStanzaLeadingGroups(stanza)

            Return True

        Catch ex As Exception
            Debug.Assert(False)
            m_logger.LogError(ex, "LoadStanza. Exception:{message}", ex.Message)
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
    Friend Function CalculateStanza(stanza As cStanzaGroup) As Boolean
        Dim FirstAge() As Integer, SecondAge() As Integer
        Dim Bio() As Single, Z() As Single, cb() As Single, Bat() As Single
        Dim SpawnProp() As Single
        Dim bSuccess As Boolean = True

        Try
            Dim iStanza As Integer = stanza.Index
            Dim nLifeStages As Integer = stanza.nLifeStages

            If (nLifeStages <= 0) Then
                ' Fail calculations without making any changes
                Return False
            End If

            Dim i As Integer
            Dim orgVBK As Single = Me.EcopathGroupInputs(stanza.iGroups(1)).VBK
            Dim iHatchCode As Integer = stanza.HatchCode
            Dim bFixedFecundity As Boolean = stanza.FixedFecundity

            Dim wmatwinf As Single
            Dim rp As Single
            Dim ba As Single
            Dim leadingB As Integer
            Dim leadingCB As Integer
            Dim recruiting As Integer

            'maybe not the correct messagetype but it seems to work
            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STANZA_CALCULATEPARMS_TOOMANYMISSING, stanza.Name),
                                            eMessageType.TooManyMissingParameters, eCoreComponentType.Ecopath, eMessageImportance.Warning, eDataTypes.Stanza)
            ReDim Bio(nLifeStages)
            ReDim Bat(nLifeStages) 'in this case the Bat() is ignored so no need to populate it
            ReDim Z(nLifeStages)
            ReDim cb(nLifeStages)
            ReDim FirstAge(nLifeStages)
            ReDim SecondAge(nLifeStages) 'last month of age by spp, stanza (set in ecopath)
            ReDim SpawnProp(nLifeStages)

            If Not stanza.OkToCalculate(msg) Then
                'this stanza group has not had it parameters set B CB and Mort
                'Stanza parameters can not be calculated until this has been done by the interface
                Me.m_publisher.SendMessage(msg)
                Return False
            End If

            wmatwinf = stanza.WmatWinf
            rp = stanza.RecruitmentPower
            ba = stanza.BiomassAccumulationRate

            For i = 1 To nLifeStages
                Bio(i) = stanza.Biomass(i)
                Z(i) = stanza.Mortality(i)
                cb(i) = stanza.CB(i)
                FirstAge(i) = stanza.StartAge(i)
                SpawnProp(i) = stanza.SpawnProp(i)
            Next
            leadingB = stanza.LeadingB
            leadingCB = stanza.LeadingCB
            recruiting = stanza.RecruitmentStanza

            If SecondAge(nLifeStages) = 0 Then
                For i = 2 To nLifeStages
                    SecondAge(i - 1) = FirstAge(i) - 1
                Next
                SecondAge(nLifeStages) = CInt(Math.Log(1 - 0.9 ^ (1 / 3)) / (-orgVBK / 12))
                If SecondAge(nLifeStages) > cCore.MAX_AGE Then SecondAge(nLifeStages) = cCore.MAX_AGE
            End If

            'CalculateStanzaParameters() will update cStanzaDatastructure.SplitWage() and SplitNo() for this iStanza (as well a a bunch of other variables)
            bSuccess = m_Ecosim.CalculateStanzaParameters(iStanza, nLifeStages, stanza.LeadingB, FirstAge, SecondAge, Bio, orgVBK, Z,
                                                stanza.LeadingCB, cb, stanza.BiomassAccumulationRate, Bat)

            'set Age2() for the last life stage of this stanza group to the value calculated here and CalculateStanzaParameters()
            'In EwE5 this only happens in InitStanza here we need the value from Age2() for the interface EwE5 uses SecondAge()
            m_Stanza.Age2(iStanza, nLifeStages) = SecondAge(nLifeStages)

            'Keep the EggAtSpawn flag any edits by the user will be lost by LoadStanza()
            Dim EggAtSpawn As Boolean = stanza.EggAtSpawn

            'LoadStanza() will update WeightAtAge (SplitWage), NumberAtAge (SplitNo), BiomassAtAge (SplitWage*SplitNo)
            'with the new values computed by CalculateStanzaParameters() above
            'It will also overwrite variables entered by the user with the values from Ecopath
            LoadStanza(stanza)

            're-populate the variables that the user entered as arguments to CalculateStanzaParameters() 
            'that were over written by loadStanza()
            'Restore group
            stanza.AllowValidation = False
            For i = 1 To nLifeStages
                stanza.Biomass(i) = Bio(i)
                stanza.Mortality(i) = Z(i)
                stanza.CB(i) = cb(i)
                stanza.StartAge(i) = FirstAge(i)
                stanza.StartAge(i) = FirstAge(i)
                stanza.SpawnProp(i) = SpawnProp(i)
            Next

            stanza.WmatWinf = wmatwinf
            stanza.RecruitmentPower = rp
            stanza.BiomassAccumulationRate = ba
            stanza.HatchCode = iHatchCode
            stanza.FixedFecundity = bFixedFecundity
            stanza.LeadingB = leadingB
            stanza.LeadingCB = leadingCB
            stanza.RecruitmentStanza = recruiting

            stanza.EggAtSpawn = EggAtSpawn

            'stanza.AllowValidation = True

            'this does not update the Ecopath variables that were also changed 
            'this is handled by OnChanged()
            'Ecopath.BInput(ieco) = Bio(i)
            'Ecopath.QBInput(ieco) = cb(i)
            'Ecopath.PBInput(ieco) = Z(i)

            'tell the interface that the stanza object has changed
            m_publisher.AddMessage(New cMessage("New Stanza parameters calculated.", eMessageType.DataModified,
                        eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.Stanza))

            m_publisher.sendAllMessages()
            Return bSuccess

        Catch ex As Exception
            m_logger.LogError(ex, "CalculateStanza. Exception:{message}", ex.Message)
            m_publisher.AddMessage(New cMessage("Error Calculating Stanza variables. " & ex.Message, eMessageType.ErrorEncountered,
                                    eCoreComponentType.Ecopath, eMessageImportance.Critical, eDataTypes.Stanza))
            m_publisher.sendAllMessages()
            Return False
        End Try

    End Function

    Private Sub updateStanzaLeadingGroups(stanza As cStanzaGroup)

        For iLF As Integer = 1 To stanza.nLifeStages
            Dim iEcopath As Integer = stanza.iGroups(iLF)
            m_EcopathData.isGroupLeadingB(iEcopath) = False
            m_EcopathData.isGroupLeadingCB(iEcopath) = False
            'is this LifeStage index the leading B or QB for this MultiStanza Group
            If iLF = stanza.LeadingB Then
                m_EcopathData.isGroupLeadingB(iEcopath) = True
            End If

            If iLF = stanza.LeadingCB Then
                m_EcopathData.isGroupLeadingCB(iEcopath) = True
            End If
        Next

    End Sub

    Private Function UpdateStanza(iDBID As Integer) As Boolean

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
        m_Stanza.RecStanza(iStanza) = stanza.RecruitmentStanza
        m_Stanza.WmatWinf(iStanza) = stanza.WmatWinf
        m_Stanza.BABsplit(iStanza) = stanza.BiomassAccumulationRate
        m_Stanza.HatchCode(iStanza) = stanza.HatchCode
        m_Stanza.FixedFecundity(iStanza) = stanza.FixedFecundity
        m_Stanza.EggAtSpawn(iStanza) = stanza.EggAtSpawn

        m_Stanza.Nstanza(iStanza) = stanza.nLifeStages
        For iLifeStage As Integer = 1 To stanza.nLifeStages
            m_Stanza.EcopathCode(iStanza, iLifeStage) = stanza.iGroups(iLifeStage)
            m_Stanza.Age1(iStanza, iLifeStage) = stanza.StartAge(iLifeStage)
            m_Stanza.SpawnProp(iStanza, iLifeStage) = stanza.SpawnProp(iLifeStage)

            ''update all the lifestages with the single vbK value EwE5 see frmGrpStanza.UpdateGroups
            'm_EcoPathData.vbKInput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.VBGF

            'Ecopath data that may have been changed by the stanza parameter calculations
            m_EcopathData.Binput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Biomass(iLifeStage)
            m_EcopathData.BHinput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Biomass(iLifeStage) * m_EcopathData.Area(m_Stanza.EcopathCode(iStanza, iLifeStage))
            m_EcopathData.QBinput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.CB(iLifeStage)
            m_EcopathData.PBinput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Mortality(iLifeStage)
            m_EcopathData.BAInput(m_Stanza.EcopathCode(iStanza, iLifeStage)) = stanza.Biomass(iLifeStage) * stanza.BiomassAccumulationRate

        Next iLifeStage

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Adds a stanza group to the data source.
    ''' </summary>
    ''' <param name="strStanzaName">Name to assign to new stanza group.</param>
    ''' <param name="aiGroupID">Zero-based array of <see cref="cEcoPathGroupInput">Ecopath group IDs</see>
    ''' to assign to a multi-stanza configuration.</param>
    ''' <param name="aiStartAge">Zero-based array of start ages for <paramref name="aiGroupID">these groups</paramref>.</param>
    ''' <param name="iDBID">Database ID assigned to the new stanza group.</param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>The EwE core cannot handle a situation where a stanza configuration
    ''' is defined without having any groups. To avoid this situation, this method
    ''' requires a valid group ID.</remarks>
    ''' -----------------------------------------------------------------------
    Public Function AppendStanza(strStanzaName As String, aiGroupID() As Integer, aiStartAge() As Integer, ByRef iDBID As Integer) As Boolean

        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        ' Append the stanza
        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.AppendStanza(strStanzaName, aiGroupID, aiStartAge, iDBID) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of stanza has changed.", eCoreComponentType.Ecopath, eDataTypes.Stanza)
            bSucces = True
        End If
        ' Decrease batch count
        Me.ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a stanza group from the data source.
    ''' </summary>
    ''' <param name="iStanza">Index of the stanza group to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveStanza(ByRef iStanza As Integer) As Boolean

        Dim iDBID As Integer = Me.m_Stanza.StanzaDBID(iStanza)
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        ' Remove the stanza
        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.RemoveStanza(iDBID) Then
            Me.DataAddedOrRemovedMessage("Ecopath number of stanza has changed.", eCoreComponentType.Ecopath, eDataTypes.Stanza)
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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddStanzaLifestage(iStanza As Integer, iGroupDBID As Integer,
                                       iAge As Integer) As Boolean

        Dim iStanzaDBID As Integer = Me.m_Stanza.StanzaDBID(iStanza)
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False
        ' Remove the stanza
        ds = DirectCast(DataSource, IEcopathDataSource)
        bSucces = ds.AddStanzaLifestage(iStanzaDBID, iGroupDBID, iAge)
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
    Public Function RemoveStanzaLifestage(iStanza As Integer,
                                          iGroupDBID As Integer) As Boolean
        Dim iStanzaDBID As Integer = Me.m_Stanza.StanzaDBID(iStanza)
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcopathDataSource) Then Return False

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

#Region " Monte Carlo "

    ''' <summary>
    ''' Get the Ecosim<see cref="cMonteCarloManager">Monte Carlo manager</see>.
    ''' </summary>
    Public ReadOnly Property EcosimMonteCarlo() As cMonteCarloManager
        Get
            Return Me.m_MonteCarlo
        End Get
    End Property

#End Region ' Monte carlo

#Region " Fit to time series "

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

    Private m_EcotracerScenarios As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcotracerScenario, 1)
    Private m_EcotracerGroupInputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.EcotracerGroupInput, 1)
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
    Public ReadOnly Property nEcotracerScenarios() As Integer
        Get
            Try
                ' Return Ecopath administration number here instead of counting UI items
                Return Me.m_EcopathData.NumEcotracerScenarios
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
    ''' One-based index of the scenario to obtain [1, <see cref="EcotracerScenarioCount">#scenarios</see>].
    ''' </param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EcotracerScenarios(iScenario As Integer) As cEcotracerScenario
        Get
            Return DirectCast(Me.m_EcotracerScenarios(iScenario), cEcotracerScenario)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the index of the active <see cref="cEcotracerScenario">Ecotracer scenario</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ActiveEcotracerScenarioIndex() As Integer
        Get
            Return Me.m_EcopathData.ActiveEcotracerScenario
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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function NewEcotracerScenario(strName As String,
                                         strDescription As String,
                                         strAuthor As String,
                                         strContact As String) As Boolean

        Dim ds As IEcotracerDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim iScenario As Integer = 0

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcotracerDatasource) Then Return False

        If Me.m_StateMonitor.HasEcopathLoaded() = False Then
            Return False
        End If

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecotracer) Then Return False

        Try

            ds = DirectCast(DataSource, IEcotracerDatasource)
            If (ds.AppendEcotracerScenario(strName, strDescription, strAuthor, strContact, iScenarioID)) Then
                Me.StateMonitor.UpdateDataState(Me.DataSource)
                Me.InitEcotracerScenarios()
                Me.DataAddedOrRemovedMessage("Ecotracer number of scenarios has changed.", eCoreComponentType.Ecotracer, eDataTypes.EcotracerScenario)
                iScenario = Array.IndexOf(Me.m_EcopathData.EcotracerScenarioDBID, iScenarioID)

                If (Me.PluginManager IsNot Nothing) Then
                    Me.PluginManager.EcotracerScenarioAdded(Me.DataSource, iScenarioID)
                End If

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
    ''' <returns>True if successful.</returns>
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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadEcotracerScenario(iScenario As Integer) As Boolean

        If (iScenario < 1) Then Return False
        If (Me.nEcotracerScenarios < iScenario) Then Return False

        Dim ds As IEcotracerDatasource = Nothing
        Dim strScenarioName As String = Me.m_EcopathData.EcotracerScenarioName(iScenario)
        Dim bSuccess As Boolean = True

        ' Sanity checks
        If (Me.DataSource Is Nothing) Then Return False
        If (Not TypeOf (DataSource) Is IEcotracerDatasource) Then Return False

        Try

            'For an Ecotracer scenario to load there must be an Ecosim scenario loaded
            If Not Me.m_StateMonitor.HasEcosimLoaded() Then
                Debug.Assert(False, "LoadEcotracerScenario() Load  Ecosim first. This is temporary.")
                Return False
            End If

            ' Update core state
            Me.m_StateMonitor.SetEcotracerLoaded(False)

            ds = DirectCast(DataSource, IEcotracerDatasource)
            If Not ds.LoadEcotracerScenario(Me.m_EcopathData.EcotracerScenarioDBID(iScenario)) Then
                Debug.Assert(False, "LoadEcotracerScenario() Failed to load scenario from data source.")
                SendEcotracerLoadMessage(strScenarioName, My.Resources.CoreMessages.ECOTRACER_LOAD_FAILED)
                Return False
            End If

            bSuccess = Me.InitEcotracerModelParamaters()
            bSuccess = bSuccess And InitEcotracerGroups()

            InitEcotracerOutputs()

            ' Reset ecosim model params for consimon flag
            Me.m_EcoSimRun.ResetStatusFlags()

            SendEcotracerLoadMessage(strScenarioName)

            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.EcotracerLoadScenario(ds)
            ' Update core state
            Me.m_StateMonitor.SetEcotracerLoaded(bSuccess)

        Catch ex As Exception
            m_logger.LogError(ex, "LoadEcotracerScenario. Exception:{message}", ex.Message)
            SendEcotracerLoadMessage(strScenarioName, ex.Message)
            Debug.Assert(False, ex.Message)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    Public Sub CloseEcotracerScenario()
        Me.m_EcopathData.ActiveEcotracerScenario = -1

        Me.m_StateMonitor.SetEcotracerLoaded(False)
        m_logger.LogInformation("Ecotracer scenario closed")

        ' Invoke plugin point
        If (Me.PluginManager IsNot Nothing) Then
            Me.PluginManager.EcotracerCloseScenario()
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the current Ecotracer scenario.
    ''' </summary>
    ''' <param name="scenario">A scenario to overwrite, if any.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveEcotracerScenario(Optional scenario As cEcotracerScenario = Nothing) As Boolean
        Dim iScenarioID As Integer = 0
        Dim ds As IEcotracerDatasource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (DataSource) Is IEcotracerDatasource) Then Return False

        ' Overwrite scenario?
        If scenario IsNot Nothing Then
            iScenarioID = scenario.DBID
        Else
            iScenarioID = m_EcopathData.EcotracerScenarioDBID(m_EcopathData.ActiveEcotracerScenario)
        End If

        Debug.Assert(iScenarioID > 0)

        ' Save ok?
        ds = DirectCast(DataSource, IEcotracerDatasource)
        If (ds.SaveEcotracerScenario(iScenarioID)) Then
            ' #Yes: Reload scenarios
            Me.InitEcotracerScenarios()
            ' Update active scenario ID
            Me.m_EcopathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcopathData.EcotracerScenarioDBID, iScenarioID)
            ' Invoke plugin point
            If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.SaveEcotracerScenario(Me)
            ' Force update
            Me.m_StateMonitor.SetEcotracerLoaded(True, TriState.True)
            ' Update data state
            Me.m_StateMonitor.UpdateDataState(DataSource)
            ' Report succes
            SendEcotracerSaveStateMessage(Me.m_EcopathData.EcotracerScenarioName(Me.ActiveEcotracerScenarioIndex))
            Return True
        End If

        ' Restore active scenario ID
        Me.m_EcopathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcopathData.EcotracerScenarioDBID, iScenarioID)

        ' Report failure
        SendEcotracerSaveStateMessage(Me.m_EcopathData.EcotracerScenarioName(Me.ActiveEcotracerScenarioIndex), False,
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
    Public Function SaveEcotracerScenarioAs(strName As String,
                                            strDescription As String) As Boolean

        Dim epd As cEcopathDataStructures = Me.m_EcopathData
        Dim ds As IEcotracerDatasource = Nothing
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcotracerDatasource) Then Return False
        If (Me.m_EcopathData.ActiveEcotracerScenario <= 0) Then Return False

        ' Clear duplicates
        Me.RemoveEcotracerScenario(Me.FindObjectByName(Me.m_EcotracerScenarios, strName))

        iScenarioID = Me.m_EcopathData.EcotracerScenarioDBID(Me.m_EcopathData.ActiveEcotracerScenario)
        If (iScenarioID <= 0) Then Return False

        ' Save ok?
        ds = DirectCast(DataSource, IEcotracerDatasource)
        If (ds.AppendEcotracerScenario(strName, strDescription,
                epd.EcotracerScenarioAuthor(Me.m_EcopathData.ActiveEcotracerScenario),
                epd.EcotracerScenarioContact(Me.m_EcopathData.ActiveEcotracerScenario),
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
        Me.m_EcopathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcopathData.EcotracerScenarioDBID, iScenarioID)

        ' Report failure
        Me.SendEcotracerSaveStateMessage(strName, False)
        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cEcotracerScenario">Ecotracer Scenario</see> from the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="scenario">The <see cref="cEcotracerScenario">Scenario</see> to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcotracerScenario(scenario As cCoreInputOutputBase) As Boolean
        If (scenario Is Nothing) Then Return True
        If (Not TypeOf (scenario) Is cEcotracerScenario) Then Return False
        Return Me.RemoveEcotracerScenario(scenario.Index)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cEcotracerScenario">Ecotracer Scenario</see> from 
    ''' the current <see cref="IEwEDataSource">Data Source</see>.
    ''' </summary>
    ''' <param name="iScenario">Index of the scenario in the 
    ''' <see cref="m_EcotracerScenarios">Ecotracer Scenario list</see>.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveEcotracerScenario(iScenario As Integer) As Boolean

        Dim ds As IEcotracerDatasource = Nothing
        Dim iScenarioIDDeleted As Integer = Me.m_EcopathData.EcotracerScenarioDBID(iScenario)
        Dim iScenarioID As Integer = cCore.NULL_VALUE
        Dim bSucces As Boolean = False

        ' Sanity check
        Debug.Assert(iScenario > 0 And iScenario < Me.m_EcopathData.EcotracerScenarioDBID.Length)

        ' Cannot delete a loaded scenario
        If (iScenario = Me.m_EcopathData.ActiveEcotracerScenario) Then
            Me.m_publisher.SendMessage(New cMessage(My.Resources.CoreMessages.SCENARIO_DELETE_LOADED,
                                                    eMessageType.NotSet, eCoreComponentType.DataSource,
                                                    eMessageImportance.Warning))
            Return False
        End If

        ' Save pending relevant changes
        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecotracer) Then Return False

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcotracerDatasource) Then Return False

        ' Remember scenario ID to restore
        If (Me.m_EcopathData.ActiveEcotracerScenario > 0) Then
            iScenarioID = Me.m_EcopathData.EcotracerScenarioDBID(Me.m_EcopathData.ActiveEcotracerScenario)
        End If

        ds = DirectCast(Me.DataSource, IEcotracerDatasource)
        ' Scenario removed succesfully?
        If ds.RemoveEcotracerScenario(iScenarioIDDeleted) Then
            ' #Yes: reload scenario list
            bSucces = Me.InitEcotracerScenarios()
            ' Restore active scenario ID
            Me.m_EcopathData.ActiveEcotracerScenario = Array.IndexOf(Me.m_EcopathData.EcotracerScenarioDBID, iScenarioID)

            If (Me.PluginManager IsNot Nothing) Then
                Me.PluginManager.EcotracerScenarioRemoved(Me.DataSource, iScenarioIDDeleted)
            End If

            ' Broadcast change
            Me.DataAddedOrRemovedMessage("Ecotracer number of scenarios has changed.", eCoreComponentType.Ecotracer, eDataTypes.EcotracerScenario)
        End If
        ' Return succes
        Return bSucces
    End Function

#Region " Internals "

    Private Function InitEcotracerScenarios() As Boolean
        Me.m_EcotracerScenarios.Clear()
        For i As Integer = 1 To Me.m_EcopathData.EcotracerScenarioDBID.Length - 1
            Me.m_EcotracerScenarios.Add(New cEcotracerScenario(Me))
            Me.InitEcotracerScenario(i)
        Next
        Return True
    End Function

    Private Function InitEcotracerScenario(iScenario As Integer) As Boolean

        Dim ets As cEcotracerScenario = Me.EcotracerScenarios(iScenario)
        Try
            ets.AllowValidation = False

            ets.DBID = m_EcopathData.EcotracerScenarioDBID(iScenario)
            ets.Name = m_EcopathData.EcotracerScenarioName(iScenario)
            ets.Author = m_EcopathData.EcotracerScenarioAuthor(iScenario)
            ets.Contact = m_EcopathData.EcotracerScenarioContact(iScenario)
            ets.LastSaved = m_EcopathData.EcotracerScenarioLastSaved(iScenario)
            ets.Index = iScenario
            ets.ResetStatusFlags()

            ets.AllowValidation = True

        Catch ex As Exception
            m_logger.LogError(ex, "InitEcotracerScenario. Exception:{message}", ex.Message)
            Debug.Assert(False, "Error Getting cEcotracerScenario Info: " & ex.Message)
            Return Nothing
        End Try
        Return True

    End Function

    Private Function UpdateEcotracerScenario(iDBID As Integer) As Boolean

        Dim iScenario As Integer = Array.IndexOf(Me.m_EcopathData.EcotracerScenarioDBID, iDBID)
        Dim scn As cEcotracerScenario = Me.EcotracerScenarios(iScenario)

        Try
            Me.m_EcopathData.EcotracerScenarioName(iScenario) = scn.Name
            Me.m_EcopathData.EcotracerScenarioDescription(iScenario) = scn.Description
            Me.m_EcopathData.EcotracerScenarioAuthor(iScenario) = scn.Author
            Me.m_EcopathData.EcotracerScenarioContact(iScenario) = scn.Contact
            ' Do not update last saved date; this is exclusively set by the core when saving

        Catch ex As Exception
            m_logger.LogError(ex, "UpdateEcotracerScenario. Exception:{message}", ex.Message)
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

    Private Sub SendEcotracerLoadMessage(strScenario As String, Optional strError As String = "")
        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If String.IsNullOrEmpty(strError) Then
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOTRACER_LOAD_SUCCESS, strScenario)
            msg = New cMessage(strText, eMessageType.DataAddedOrRemoved, eCoreComponentType.Ecotracer, eMessageImportance.Information)
        Else
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOTRACER_LOAD_FAILED, strScenario, strError)
            msg = New cMessage(strText, eMessageType.ErrorEncountered, eCoreComponentType.Ecotracer, eMessageImportance.Warning)
        End If

        Me.m_publisher.AddMessage(msg)
        m_publisher.sendAllMessages()

    End Sub

    Private Sub SendEcotracerSaveStateMessage(strScenarioName As String, Optional bSucces As Boolean = True,
            Optional strError As String = "")

        Dim msg As cMessage = Nothing
        Dim strText As String = ""

        If bSucces Then
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOTRACER_SAVE_SUCCES, strScenarioName)
            msg = New cMessage(strText, eMessageType.DataModified, eCoreComponentType.Ecotracer, eMessageImportance.Information)
        Else
            strText = cStringUtils.Localize(My.Resources.CoreMessages.ECOTRACER_SAVE_FAILED, strScenarioName, strError)
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
            Me.m_EcotracerModelParameters.MaxTimeSteps = Me.m_tracerData.MaxTimeSteps

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

            Me.m_tracerData.MaxTimeSteps = Me.m_EcotracerModelParameters.MaxTimeSteps


        Catch ex As Exception
            m_logger.LogError(ex, "UpdateEcotracerModelParameters. Exception:{message}", ex.Message)
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
    Public ReadOnly Property EcotracerGroupInputs(iGroup As Integer) As cEcotracerGroupInput
        Get
            Return DirectCast(Me.m_EcotracerGroupInputs(iGroup), cEcotracerGroupInput)
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
                grp = New cEcotracerGroupInput(Me, Me.m_EcopathData.GroupDBID(i))
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
                iGroup = Array.IndexOf(Me.m_EcopathData.GroupDBID, grp.DBID)

                Debug.Assert(iGroup > 0 And iGroup <= Me.nGroups, "LoadEcotracerGroups() failed to find iGroup for Ecotracer DBID.")

                grp.Resize()

                grp.AllowValidation = False

                grp.Index = iGroup
                grp.Name = m_EcopathData.GroupName(iGroup)
                grp.CZero = Me.m_tracerData.Czero(iGroup)
                grp.CImmig = Me.m_tracerData.Cimmig(iGroup)
                grp.CEnvironment = Me.m_tracerData.Cenv(iGroup)
                grp.CDecay = Me.m_tracerData.cdecay(iGroup)
                grp.CAssimilationProp = Me.m_tracerData.CassimProp(iGroup)
                grp.CMetablismRate = Me.m_tracerData.CmetabolismRate(iGroup)
                grp.PP = m_EcopathData.PP(iGroup)

                grp.ResetStatusFlags()
                grp.AllowValidation = True

            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitEcotracerGroups() Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function

    Private Function UpdateEcotracerGroup(iDBID As Integer) As Boolean

        Dim grp As cEcotracerGroupInput = Nothing
        Dim iGroup As Integer

        Try

            ' Convert the Database ID into an iGroup
            iGroup = Array.IndexOf(m_EcopathData.GroupDBID, iDBID)
            ' Get the group
            grp = Me.EcotracerGroupInputs(iGroup)
            ' Read it
            Me.m_tracerData.Czero(iGroup) = grp.CZero
            Me.m_tracerData.Cimmig(iGroup) = grp.CImmig
            Me.m_tracerData.Cenv(iGroup) = grp.CEnvironment
            Me.m_tracerData.cdecay(iGroup) = grp.CDecay
            Me.m_tracerData.CassimProp(iGroup) = grp.CAssimilationProp
            Me.m_tracerData.CmetabolismRate(iGroup) = grp.CMetablismRate

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateEcotracerGroup() Error: " & ex.Message)
            Return False
        End Try
        Return True

    End Function


    Private Sub loadEcotracerResults()

        Try

            'jb 11-Jan-2011 All Contaminant Tracer output now uses core data directly.
            'So we don't need to update the IO objects.
            'This was left in place in case other data is added to the Tracer output objects.

        Catch ex As Exception
            m_logger.LogError(ex, "loadEcotracerResults. Exception:{message}", ex.Message)
            'for now just assert this should send a message that the tracer results could not be loaded
            Debug.Assert(False, ex.StackTrace)
        End Try

    End Sub

#End Region ' Internals

#End Region ' Groups

#End Region ' Ecotracer

#Region " Auxiliary data "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set an<see cref="cAuxiliaryData">AuxillaryData</see> instance 
    ''' for a given <see cref="cValueID">value ID</see>.
    ''' </summary>
    ''' <param name="key">The unique <see cref="cValueID">value ID</see>.</param>
    ''' <returns>An cAuxillaryData instance.</returns>
    ''' -------------------------------------------------------------------
    Public Property AuxillaryData(key As cValueID) As cAuxiliaryData
        Get
            Return Me.AuxillaryData(key.ToString)
        End Get
        Friend Set(value As cAuxiliaryData)
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
    Public Property AuxillaryData(strValueID As String) As cAuxiliaryData

        Get
            If (String.IsNullOrEmpty(strValueID)) Then Return Nothing

            Dim ad As cAuxiliaryData = Nothing
            If (Not Me.m_dtAuxiliaryData.ContainsKey(strValueID)) Then
                Me.AuxillaryData(strValueID) = New cAuxiliaryData(Me, strValueID)
            End If
            Return Me.m_dtAuxiliaryData(strValueID)
        End Get

        Set(ad As cAuxiliaryData)
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

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get all auxillary data that related to a given core object.
    ''' </summary>
    ''' <param name="source">The core object to retrieve <see cref="cAuxiliaryData">auxillary data</see> for.</param>
    ''' <param name="bIncludeReferrals"><para>Flag stating if also auxillary data 
    ''' needs to be found for other core objects that use this object as a
    ''' secundary index.</para>
    ''' <para>For instance, when searching for group input remarks, setting 
    ''' this flag to false will only return remarks allocated to the group.
    ''' Setting this flag to true will also return remarks for fleet landings
    ''' that refer to the group.</para></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property AuxillaryData(source As cCoreInputOutputBase,
                                           Optional bIncludeReferrals As Boolean = False) As Dictionary(Of String, cAuxiliaryData)
        Get
            Return Me.AuxillaryData(source.DataType, source.DBID, bIncludeReferrals)
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get all auxillary data that related to a given object.
    ''' </summary>
    ''' <param name="dt">The datatype of the core object.</param>
    ''' <param name="iDBID">The database ID of the core object.</param>
    ''' <param name="bIncludeReferrals"><para>Flag stating if also auxillary data 
    ''' needs to be found for other core objects that use this object as a
    ''' secundary index.</para>
    ''' <para>For instance, when searching for group input remarks, setting 
    ''' this flag to false will only return remarks allocated to the group.
    ''' Setting this flag to true will also return remarks for fleet landings
    ''' that refer to the group.</para></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property AuxillaryData(dt As eDataTypes, iDBID As Integer,
                                           Optional bIncludeReferrals As Boolean = False) As Dictionary(Of String, cAuxiliaryData)
        Get
            Dim dic As New Dictionary(Of String, cAuxiliaryData)
            For Each strKey As String In Me.m_dtAuxiliaryData.Keys
                Dim vid As cValueID = cValueID.FromString(strKey)
                If vid.DataTypePrim = dt And vid.DBIDPrim = iDBID Then
                    dic(strKey) = Me.m_dtAuxiliaryData(strKey)
                End If
                If bIncludeReferrals And vid.DataTypeSec = dt And vid.DBIDSec = iDBID Then
                    dic(strKey) = Me.m_dtAuxiliaryData(strKey)
                End If
            Next
            Return dic
        End Get
    End Property

#Region " Pedigree "

    Private Function InitPedigreeManagers() As Boolean

        Dim manager As cPedigreeManager = Nothing
        Dim level As cPedigreeLevel = Nothing
        Dim varname As eVarNameFlags = eVarNameFlags.NotSet

        ' Popluate managers
        Me.m_PedigreeManagers.Clear()
        For iVariable As Integer = 1 To Me.nPedigreeVariables
            ' Get variable
            varname = Me.PedigreeVariable(iVariable)
            ' Create manager
            manager = New cPedigreeManager(Me, varname, iVariable)
            ' Configure manager
            manager.AllowValidation = False
            manager.Index = iVariable
            manager.DBID = iVariable
            manager.AllowValidation = True
            ' Initialize manager
            manager.Init()
            ' Store manager
            Me.m_PedigreeManagers(varname) = manager
        Next
        Return True

    End Function

    Private Function LoadPedigreeManagers() As Boolean
        Return Me.LoadPedigreeLevels() And Me.LoadPedigree()
    End Function

    ''' <summary>
    ''' Load pedigree levels data.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    Private Function LoadPedigreeLevels() As Boolean
        Dim bSucces As Boolean = True
        For Each man As cPedigreeManager In Me.m_PedigreeManagers.Values
            bSucces = bSucces And Me.LoadPedigreeLevels(man)
        Next
        Return bSucces
    End Function

    ''' <summary>
    ''' Load pedigree levels data for a given manager.
    ''' </summary>
    ''' <param name="man">The <see cref="cPedigreeManager">manager</see> to load.</param>
    ''' <returns>True if successful.</returns>
    Private Function LoadPedigreeLevels(man As cPedigreeManager) As Boolean
        Return man.LoadPedigreeLevels()
    End Function

    ''' <summary>
    ''' Load pedigree assignments data.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    Private Function LoadPedigree() As Boolean
        Dim bSucces As Boolean = True
        For Each man As cPedigreeManager In Me.m_PedigreeManagers.Values
            bSucces = bSucces And Me.LoadPedigree(man)
        Next
        Return bSucces
    End Function

    ''' <summary>
    ''' Load pedigree assignments data for a given manager.
    ''' </summary>
    ''' <param name="man">The <see cref="cPedigreeManager">manager</see> to load.</param>
    ''' <returns>True if successful.</returns>
    Private Function LoadPedigree(man As cPedigreeManager) As Boolean
        Return man.LoadPedigree()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the pedigree manager for a given variable name.
    ''' </summary>
    ''' <param name="varName"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function GetPedigreeManager(varName As eVarNameFlags) As cPedigreeManager
        ' Mappings
        If (varName = eVarNameFlags.Discards) Or (varName = eVarNameFlags.Landings) Then
            varName = eVarNameFlags.TCatchInput
        End If
        If (Not Me.IsPedigreeVariableSupported(varName)) Then Return Nothing
        If (Not Me.m_PedigreeManagers.ContainsKey(varName)) Then Return Nothing
        Return Me.m_PedigreeManagers(varName)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether pedigree is supported for a given variable.
    ''' </summary>
    ''' <param name="varName">The variable to check.</param>
    ''' <returns>True if the <paramref name="varName">variable</paramref> is supported.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsPedigreeVariableSupported(varName As eVarNameFlags) As Boolean
        Return Me.PedigreeVariableIndex(varName) > -1 And (varName <> eVarNameFlags.NotSet)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the index of a pedigree variable, or -1 if pedigree is not 
    ''' supported for the variable.
    ''' </summary>
    ''' <param name="varName">The variable to obtain the index for.</param>
    ''' <returns>A one-based index, or -1 if pedigree is not 
    ''' supported for the variable.</returns>
    ''' -----------------------------------------------------------------------
    Public Function PedigreeVariableIndex(varName As eVarNameFlags) As Integer
        Return Array.IndexOf(cEcopathDataStructures.PedigreeVariables, varName)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the variable at a given <see cref="PedigreeVariableIndex">variable index</see>.
    ''' </summary>
    ''' <param name="iIndex">One-based of the variable to retrieve.</param>
    ''' <returns>The variable at the given <see cref="PedigreeVariableIndex">index</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Function PedigreeVariable(iIndex As Integer) As eVarNameFlags
        If (iIndex < 1 Or iIndex > Me.m_EcopathData.NumPedigreeVariables) Then Return eVarNameFlags.NotSet
        Try
            Return cEcopathDataStructures.PedigreeVariables(iIndex)
        Catch ex As Exception
            Return eVarNameFlags.NotSet
        End Try
    End Function

#End Region ' Pedigree

#Region " Meta data "

    Public Function GetDataDescription(dt As eDataTypes, iDBID As Integer) As String
        If (Me.DataSource IsNot Nothing) Then
            If (TypeOf Me.DataSource Is IEwEDatasourceMetadata) Then
                Return DirectCast(Me.DataSource, IEwEDatasourceMetadata).GetDescription(dt, iDBID)
            End If
        End If
        Return ""
    End Function

#End Region ' Meta data

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
        Dim updateState As TriState

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

                Case eDataTypes.Taxon
                    If bValidatedOk Then Me.UpdateEcopathGroupTaxon(idAffected)

                Case eDataTypes.FleetInput
                    If bValidatedOk Then Me.UpdateFleetInput(idAffected)

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

                    'Case eDataTypes.Forcing,
                    '     eDataTypes.EggProd,
                    '     eDataTypes.Mediation,
                    '     eDataTypes.FishingEffort,
                    '     eDataTypes.FishMort
                    '    msAffected = eCoreComponentType.ShapesManager

                Case eDataTypes.EcospaceBasemap
                    If bValidatedOk Then Me.UpdateEcospaceBasemap()

                Case eDataTypes.EcospaceLayerImportance
                    If bValidatedOk Then Me.UpdateEcospaceImportanceLayers()

                Case eDataTypes.EcospaceModelParameter
                    If bValidatedOk Then Me.UpdateEcospaceModelParameters()

                Case eDataTypes.EcospaceHabitat
                    If bValidatedOk Then Me.UpdateEcospaceHabitat(idAffected)

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

                Case eDataTypes.FishingPolicyManager, eDataTypes.FishingPolicyParameters,
                            eDataTypes.FishingPolicySearchBlocks
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.FishingPolicyManager).Update(dtAffected)

                Case eDataTypes.SearchObjectiveFleetInput, eDataTypes.SearchObjectiveGroupInput,
                        eDataTypes.SearchObjectiveWeights, eDataTypes.SearchObjectiveParameters
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.SearchObjectiveManager).Update(dtAffected)

                Case eDataTypes.MPAOptManager, eDataTypes.MPAOptOutput, eDataTypes.MPAOptParameters
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.MPAOptManager).Update(dtAffected)

                Case eDataTypes.FitToTimeSeries
                    If bValidatedOk Then Me.m_SearchManagers.Item(eDataTypes.FitToTimeSeries).Update(dtAffected)

                Case eDataTypes.PedigreeManager
                    If bValidatedOk Then DirectCast(objValidated, cPedigreeManager).UpdatePedigree()

                Case eDataTypes.MSEBatchParameters, eDataTypes.MSEBatchTFMInput, eDataTypes.MSEBatchFixedFInput
                    'Something in the MSE Batch interface has changed
                    'Update all the underlying core data
                    Me.MSEBatchManager.Update(dtAffected, value.varName)

                Case eDataTypes.EcospaceLayerDepth
                    If bValidatedOk Then Me.UpdateEcospaceDepthLayer()

                Case eDataTypes.EcospaceLayerDriver
                    If bValidatedOk Then Me.UpdateEcospaceDriverLayers()

                Case eDataTypes.EcospaceAdvectionParameters
                    If bValidatedOk Then Me.m_AdvectionManager.Update()

                Case eDataTypes.EcosimArenaShare
                    If bValidatedOk Then Me.m_ArenaManager.Update()

            End Select

            ' Data processed ok?
            If bValidatedOk Then

                ' Notify plug-ins
                Try
                    If Me.PluginManager IsNot Nothing Then
                        Me.PluginManager.DataValidated(vs.VarName, dtAffected)
                    End If
                Catch ex As Exception
                    ' NOP
                End Try

                ' Dirty data source
                If DataSource IsNot Nothing Then

                    ' Block non-stored variables from dirtying the data source
                    bBlock = (value.Stored = False)

                    ' Block cascaded name changes for groups and fleets
                    If (value.varName = eVarNameFlags.Name) Then
                        bBlock = bBlock Or
                                 dtAffected = eDataTypes.EcoPathGroupOutput Or
                                 dtAffected = eDataTypes.EcoSimGroupInput Or
                                 dtAffected = eDataTypes.EcospaceGroup Or
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
                If (value.AffectsRunState) Then
                    Me.m_StateMonitor.UpdateExecutionState(msAffected, updateState)
                End If

            End If
        Next

        If bValidatedOk Then
            PostVariableUpdated(value, objValidated)
        End If

        If Me.m_batchLockType = eBatchLockType.NotSet Then updateState = TriState.UseDefault Else updateState = TriState.False
        ' Send only notifications when NO lock active
        Me.m_StateMonitor.UpdateDataState(DataSource, updateState)

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
    Friend Function Validate(ByRef ValueObject As cValue, ByRef MetaData As cVariableMetaData, Optional iSecondaryIndex As Integer = cCore.NULL_VALUE, Optional iThirdIndex As Integer = cCore.NULL_VALUE) As Boolean

        Dim fmt As New Style.cVarnameTypeFormatter()

        'For now the validation is done right here (inline)
        'if this gets to bulky the core can call another routine to do the validation for different variables
        Select Case ValueObject.varName

            Case eVarNameFlags.RecruitmentStanza
                'Cannot chain these up
                Dim iStanza As Integer = ValueObject.Index
                Dim iTarget As Integer = CInt(ValueObject.Value)
                If (iTarget <= 0) Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    If Me.m_Stanza.RecStanza(iTarget) <= 0 Then
                        'passed validation
                        ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                        ValueObject.ValidationStatus = eStatusFlags.OK
                        ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                    Else
                        ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                        ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                    End If
                End If

            Case eVarNameFlags.MSEFleetWeight
                'Can not set FleetWeight if this is not a valid fleet
                Dim iflt As Integer = ValueObject.Index
                Dim igrp As Integer = iSecondaryIndex

                If Me.m_EcoSimData.relQ(iflt, igrp) > 0 Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                End If

            Case eVarNameFlags.SearchBlock
                'Fishing Policy Search
                'Cannot set the SearchBlock for anything less than or equal to the BaseYear 

                If iSecondaryIndex > Me.FishingPolicyManager.ObjectiveParameters.BaseYear Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                End If

            Case eVarNameFlags.EcospaceSummaryTimeEnd

                Dim value As Single = CSng(ValueObject.Value)
                'greater than zero 
                'less than the last time step
                'greater than start summary period
                If value > 0 And value + CSng(m_EcospaceData.TimeStep * m_EcospaceData.NumStep) <= m_EcospaceData.TotalTime And
                                value > m_EcospaceData.SumStart(0) Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If

            Case eVarNameFlags.EcospaceSummaryTimeStart

                Dim value As Single = CSng(ValueObject.Value)
                'greater than or equal to zero 
                'less than the last time step
                'less than end summary period
                If value >= 0 And value + CSng(m_EcospaceData.TimeStep * m_EcospaceData.NumStep) <= m_EcospaceData.TotalTime And
                                value < m_EcospaceData.SumStart(1) Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If

            Case eVarNameFlags.EcospaceNumberSummaryTimeSteps
                'EcospaceNumberSummaryTimeSteps is the number of time steps to summarize over
                ' not the actual time in years
                Dim value As Integer = CInt(ValueObject.Value)

                'greater than zero
                'end of the last summary period is still in bounds
                If value > 0 And m_EcospaceData.SumStart(1) + CSng(value * m_EcospaceData.TimeStep) <= m_EcospaceData.TotalTime Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If


            Case eVarNameFlags.EcosimSumEnd, eVarNameFlags.EcosimSumStart, eVarNameFlags.EcosimSumNTimeSteps

                Me.validateEcosimSummaryTimes(ValueObject)


            Case eVarNameFlags.MPAOptEndYear
                'Last year of the MPA Optimization Search
                Dim value As Integer = CInt(ValueObject.Value)

                If value > 0 And value <= m_EcospaceData.TotalTime And value >= Me.m_MPAOptData.EcoSpaceStartYear + Me.m_MPAOptData.MinRunLength Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation

                End If


            Case eVarNameFlags.MPAOptStartYear
                'First year of the MPA Optimization Search
                Dim value As Integer = CInt(ValueObject.Value)

                If value > 0 And value < m_EcospaceData.TotalTime And value + Me.m_MPAOptData.MinRunLength <= Me.m_MPAOptData.EcoSpaceEndYear Then
                    'passed validation
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
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
                    ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationMessage = My.Resources.CoreMessages.MSE_FIXF_FIXESC_FAILEDVALIDATION
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                End If

        End Select

        Return True

    End Function


    Private Sub validateEcosimSummaryTimes(ByRef ValueObject As cValue)

        Dim val As Single = CSng(ValueObject.Value)
        Dim fmt As New Style.cVarnameTypeFormatter()
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
            ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.OK
            ValueObject.Status(cCore.NULL_VALUE) = eStatusFlags.OK
        Else
            'failed validation
            ValueObject.ValidationMessage = cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        End If


    End Sub

    Private Function GetAffectedVariableStatus(obj As cCoreInputOutputBase, varName As eVarNameFlags, Optional iSecIndex As Integer = cCore.NULL_VALUE) As cVariableStatus
        Dim fmt As New Style.cVarnameTypeFormatter()
        Return New cVariableStatus(obj, eStatusFlags.OK,
                                   cStringUtils.Localize(My.Resources.CoreMessages.VARIABLE_VALIDATION_ADJUSTED, fmt.ToString(varName)),
                                   varName, iSecIndex)
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
    Private Sub PostVariableValidation(value As cValue, obj As cCoreInputOutputBase, msg As cMessage)

        Debug.Assert(value.ValidationStatus <> eStatusFlags.FailedValidation, "PostVariableValidation() should not be called if a variable failed validation.")

        ' First update core data from object
        Select Case obj.DataType

            Case eDataTypes.EwEModel
                'EwEModel
                Select Case value.varName

                    Case eVarNameFlags.IsEcospaceModelCoupled
                        If Me.m_StateMonitor.HasEcospaceLoaded Then
                            're-load Ecospace before the changes can take affect
                            Me.m_publisher.AddMessage(New cMessage(My.Resources.CoreMessages.RELOAD_ECOSPACE, eMessageType.Any, eCoreComponentType.Core, eMessageImportance.Warning))
                        End If

                End Select


            Case eDataTypes.EcoPathGroupInput
                Debug.Assert(TypeOf obj Is cEcoPathGroupInput)
                Dim group As cEcoPathGroupInput = DirectCast(obj, cEcoPathGroupInput)

                Select Case value.varName

                    Case eVarNameFlags.Biomass
                        Debug.Assert(False, "Biomass is not editable from the UI")

                    Case eVarNameFlags.HabitatArea, eVarNameFlags.BiomassAreaInput
                        ' Area or BiomassAreaInput have changed: recalculate B (biomass)
                        m_EcopathData.Binput(group.Index) = group.BiomassAreaInput * group.Area
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

                    Case eVarNameFlags.BioAccumOutput
                        group.AllowValidation = False
                        group.BioAccumRate = 0.0
                        group.AllowValidation = True

                    Case eVarNameFlags.BioAccumRate
                        group.AllowValidation = False
                        group.BioAccumInput = 0.0
                        group.AllowValidation = True

                End Select

            Case eDataTypes.FleetInput
                'all Fleet stuff is handled in PostVariableUpdated()
                'The Case statement was just left here for reference


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
                Debug.Assert(TypeOf obj Is cEcosimGroupInput)
                Dim esi As cEcosimGroupInput = DirectCast(obj, cEcosimGroupInput)

                Select Case value.varName
                    Case eVarNameFlags.MaxRelPB
                        'see vaSimGetPBMandFtimeMax() in EwE5. Solve this here or in PostVariableUpdated?

                    Case eVarNameFlags.VulMult
                        Try
                            m_Ecosim.setvulratecell(obj.ValidationStatus.Index, obj.ValidationStatus.iArrayIndex, CSng(value.Value(obj.ValidationStatus.iArrayIndex)))
                        Catch ex As Exception
                            m_logger.LogError("PostVariableValidation Exception:{message}", ex.Message)
                            Debug.Assert(False, "PostVariableValidation() setvulratecell error. " & ex.StackTrace)
                        End Try


                End Select

            'Case eDataTypes.EcosimFleetInput

            '    If value.varName = eVarNameFlags.RelQt Then
            '        Try
            '            Dim arrayValue As cValueArrayTripleIndex = DirectCast(value, cValueArrayTripleIndex)
            '            Dim QYear() As Single = New Single(Me.nFleets) {}
            '            For i As Integer = 1 To Me.m_EcoPathData.NumFleet
            '                QYear(i) = 1
            '            Next
            '            'For it As Integer = 1 To Me.nEcosimTimeSteps
            '            Me.m_EcoSim.SetFtimeFromGear(arrayValue.iThirdIndex, QYear, True)
            '            'Next


            '        Catch ex As Exception

            '        End Try
            '    End If


            Case eDataTypes.EcospaceGroup

                Dim grp As cEcospaceGroupInput = DirectCast(obj, cEcospaceGroupInput)

                Select Case value.varName

                    Case eVarNameFlags.PreferredHabitat

                        ' If 'All' habitat is set, all other preferred habitat assignments must be cleared.
                        ' If any other habitat is set, the 'All' habitat assignment must be cleared.

                        grp.AllowValidation = False

                        ' Setting a value?
                        If CSng(value.Value(grp.ValidationStatus.iArrayIndex)) > 0 Then
                            ' 'All' habitat set? Clear all other preferred habitats
                            If grp.ValidationStatus.iArrayIndex = 0 Then
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
                        grp.AllowValidation = True

                End Select

            Case eDataTypes.EcospaceFleet

                Dim esf As cEcospaceFleetInput = DirectCast(obj, cEcospaceFleetInput)

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
                        Me.m_EcospaceData.setDefaultSummaryPeriod()

                        'load the new data into the parameters object
                        Me.LoadEcospaceModelParameters()

                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.TotalTime))

                    Case eVarNameFlags.ConSimOnEcoSpace
                        'toggle contaminant tracing OFF in ecosim
                        If CBool(value.Value) = True Then
                            Me.EcosimModelParameters.ContaminantTracing = False

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
                        Dim sbmsg As New cMessage("Fishing Policy search blocks have changed.", eMessageType.DataModified,
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

            Case eDataTypes.EwEModel
                Select Case value.varName

                    Case eVarNameFlags.UnitCurrency
                        'Tell Ecopath that the Model Unit Currency has changed
                        'Ecopath will set GS to the correct values
                        Me.m_Ecopath.onModelUnitCurrencyChanged()

                        'Ecopath GS has changed populate the input objects
                        'this will set the Status flags for the interface which will have changed
                        Me.LoadEcopathInputs()

                        'Tell the interface
                        Dim gsMsg As New cMessage("", eMessageType.DataModified,
                                                       eCoreComponentType.Ecopath, eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput)
                        Me.m_publisher.AddMessage(gsMsg)

                End Select

            Case eDataTypes.EcoPathGroupInput

                Debug.Assert(TypeOf obj Is cEcoPathGroupInput)

                Dim grp As cEcoPathGroupInput = DirectCast(obj, cEcoPathGroupInput)

                Select Case value.varName

                    Case eVarNameFlags.HabitatArea, eVarNameFlags.BiomassAreaInput
                        ' Set biomass area status
                        Me.Set_PB_QB_GE_BA_Flags(grp)

                    Case eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.GEInput
                        'PB, QB or GE has been changed in the interface
                        Me.Set_PB_QB_GE_BA_Flags(grp)
                        ' Need to recalc stanza when this group is part of a multi-stanza configuration
                        bRecalcStanza = (grp.iStanza > 0)

                        If (value.varName = eVarNameFlags.PBInput) And (Me.m_StateMonitor.HasEcosimLoaded) Then
                            'update bgoal from the new PB
                            Me.m_SearchData.SetDefaultBGoal(Me.m_EcopathData.PBinput) 'use PBInput because PB has not been updated at this time
                            'load the values into the search manager
                            'if Ecosim has not been loaded SearchObjectiveManager.Load() will do nothing
                            Me.m_SearchManagers(eDataTypes.SearchObjectiveManager).Load()

                            Dim msg As New cMessage("Search Structure rel. weight changed.", eMessageType.DataModified,
                                            eCoreComponentType.SearchObjective, eMessageImportance.Maintenance, eDataTypes.SearchObjectiveManager)
                            Me.m_publisher.AddMessage(msg)
                        End If

                    Case eVarNameFlags.GS
                        'GS has been changed in the interface
                        Me.Set_GS_Flags(grp)

                    Case eVarNameFlags.DietComp
                        'DietComp has been changed by the user
                        'this needs to update the Ecosim dietcomp and refresh the shape functions AppliesTo datastructures 
                        Me.m_StateManager.updateDietComp()
                        If Me.m_StateMonitor.HasEcosimLoaded Then
                            ' Sync the ecosim groups
                            LoadEcosimGroups()

                            m_MediatedInteractionManager.Init()
                            m_MediatedInteractionManager.Load()

                            Me.m_SearchManagers(eDataTypes.FitToTimeSeries).Load()
                            '  Me.m_FitToTimeSeries.Load()
                        End If

                    Case eVarNameFlags.VBK
                        'see vaSimGetPBMandFtimeMax() in EwE5 case 10. Solve this here or in PostVariableValidation?

                        ' Need to recalc stanza when this group is part of a multi-stanza configuration
                        bRecalcStanza = (grp.iStanza > 0)

                    Case eVarNameFlags.BioAccumInput, eVarNameFlags.BioAccumRate
                        Me.Set_PB_QB_GE_BA_Flags(grp)
                        Me.LoadEcopathInput(grp)

                    Case eVarNameFlags.GS
                        'If GS is a primay producer then it can only be zero
                        Me.Set_GS_Flags(grp)

                    Case eVarNameFlags.PP
                        Me.Set_GS_Flags(grp)
                        Me.Set_PB_QB_GE_BA_Flags(grp)
                        Me.Set_EE_OtherMort_Flags(grp)
                        Me.Set_DetImp_Flags(grp)

                    Case eVarNameFlags.EEInput, eVarNameFlags.OtherMortInput
                        Me.Set_EE_OtherMort_Flags(grp)
                        Me.Set_PB_QB_GE_BA_Flags(grp)

                End Select

            Case eDataTypes.Stanza
                ' Need to recalc multi-stanza configuration
                bRecalcStanza = True

            Case eDataTypes.FleetInput

                Dim flt As cEcopathFleetInput = DirectCast(obj, cEcopathFleetInput)

                Select Case value.varName

                    Case eVarNameFlags.Landings, eVarNameFlags.Discards
                        Me.Update_IsFished(True)
                        Me.Update_Stanza_Catches()
                        Me.Update_Taxon_Catches()
                        Me.Set_DiscardMort_Flags(flt, True)
                        Me.Set_OffVesselValue_Flags(flt, True)

                        'Landing and/or discards has changed so Quota share has changed
                        If Me.m_StateMonitor.HasEcosimLoaded Then
                            'Ecosim is loaded so update the MSE Quota share
                            Me.SetDefaultQuotaShare()
                            Me.Set_Quota_Flags(Me.MSEManager.EcopathFleetInputs(flt.Index), True)
                            Dim qsMsg As New cMessage("QuotaShare has changed.", eMessageType.DataModified,
                                                        eCoreComponentType.Ecosim, eMessageImportance.Maintenance, eDataTypes.MSEFleetInput)
                            Me.m_publisher.AddMessage(qsMsg)
                        End If

                        'Dim iflt As Integer = value.Index
                        'Dim igrp As Integer = obj.ValidationStatus.iArrayIndex
                        'If Me.m_StateMonitor.HasEcosimLoaded Then
                        '    Me.SetDefaultCatchabilities(iflt, igrp)
                        'End If

                    Case eVarNameFlags.OffVesselPrice
                        Me.Set_OffVesselValue_Flags(flt, True)

                End Select

            Case eDataTypes.Taxon

                Select Case value.varName

                    Case eVarNameFlags.OrganismType, eVarNameFlags.TaxonVulnerabilityIndex
                        ' Update vul index status
                        Me.Set_Taxon_Flags(DirectCast(obj, cTaxon), True)

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
                        Dim msg As New cMessage("Ecosim results time period has changed.", eMessageType.DataModified,
                                        eCoreComponentType.Ecosim, eMessageImportance.Maintenance, eDataTypes.EcoSimModelParameter)

                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.EcosimSumStart))
                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.EcosimSumEnd))

                        Me.m_publisher.AddMessage(msg)

                End Select

            Case eDataTypes.EcoSimGroupInput
                Select Case value.varName
                    Case eVarNameFlags.MaxRelPB
                        'see vaSimGetPBMandFtimeMax() in EwE5. Solve this here or in PostVariableValidation?

                    Case eVarNameFlags.AdditivePredMortProp
                        Me.m_Ecosim.CalcBaseAdditiveMort()

                    Case eVarNameFlags.VulMult
                        If cCore.USE_SHARED_ARENAS Then
                            ' JS 01Jun23: Changing Vuls messes up shared arenas for IBM
                            Me.EcosimArenaManager.ResetArenas(0)
                        End If
                End Select

            Case eDataTypes.EcospaceModelParameter

                Dim emp As cEcospaceModelParameters = DirectCast(obj, cEcospaceModelParameters)

                Select Case value.varName

                    Case eVarNameFlags.NumTimeStepsPerYear
                        'user has changed the number of ecospace time steps per year
                        'resize the output data Ecospace will take care of itself

                        Me.m_EcospaceData.setDefaultSummaryPeriod()

                        ' ToDo_JS: Test if core counter has been updated prior to calling this!
                        For Each objOutput As cEcospaceGroupOutput In m_EcospaceGroupOuputs
                            objOutput.Resize()
                        Next

                    Case eVarNameFlags.UseIBM
                        Me.Set_IBM_Flags(emp)


                    Case eVarNameFlags.EcospaceNumberSummaryTimeSteps, eVarNameFlags.EcospaceSummaryTimeEnd, eVarNameFlags.EcospaceSummaryTimeStart
                        Me.LoadEcospaceResults()

                    Case eVarNameFlags.EcospaceRegionNumber
                        Me.EcospaceBasemap.LayerRegion.Invalidate()

                    Case eVarNameFlags.EcospaceAutosaveFirstTimeStep
                        Me.LoadEcospaceResultsWriters()

                    Case eVarNameFlags.EcospaceUseEcosimBiomassForcing, eVarNameFlags.EcospaceUseEcosimDiscardForcing
                        Me.m_publisher.AddMessage(New cMessage("Ecospace use Ecosim forcing.", eMessageType.DataModified,
                                                                  eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceModelParameter))

                    Case eVarNameFlags.EcospaceEffortZoneNumber
                        Me.m_EcospaceData.ReDimEffortZones()

                End Select 'Select Case value.varName


            Case eDataTypes.EcospaceGroup

                Dim grp As cEcospaceGroupInput = DirectCast(obj, cEcospaceGroupInput)

                Select Case value.varName
                    Case eVarNameFlags.PreferredHabitat
                        'Let Ecospace decide what to update in response
                        If Me.m_Ecospace.UpdateMaps(obj.DataType) Then
                            'Capacity layer has changed
                            'send out a message
                            Me.m_publisher.AddMessage(New cMessage("Ecospace capacity map may have changed.", eMessageType.DataModified,
                                                                   eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerHabitatCapacity))
                        End If

                    Case eVarNameFlags.EcospaceCapCalType

                        'Let Ecospace decide what to update in response
                        If Me.m_Ecospace.UpdateMaps(obj.DataType) Then
                            'Capacity layer has changed
                            'send out a message
                            Me.m_publisher.AddMessage(New cMessage("Ecospace capacity map may have changed.", eMessageType.DataModified,
                                                                   eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerHabitatCapacity))
                        End If
                        Me.Set_BadHab_Flags(grp)
                        Me.Set_HabPref_Flags(grp)

                    Case eVarNameFlags.IsMigratory

                        Me.Set_Migratory_Flags(grp)

                        Dim msg As New cMessage("Migration settings have changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceGroup)
                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.BarrierAvoidanceWeight))
                        msg.AddVariable(GetAffectedVariableStatus(obj, eVarNameFlags.InMigAreaMoveWeight))
                        Me.m_publisher.AddMessage(msg)

                        Me.m_EcospaceData.RedimMigrationMaps(bClearExisting:=False)

                End Select

            Case eDataTypes.MonteCarlo

                Select Case value.varName

                    Case eVarNameFlags.mcBAcv, eVarNameFlags.mcBcv, eVarNameFlags.mcEEcv, eVarNameFlags.mcPBcv, eVarNameFlags.mcVUcv, eVarNameFlags.mcQBcv, eVarNameFlags.mcDCcv, eVarNameFlags.mcDiscardscv, eVarNameFlags.mcLandingscv

                        Me.m_MonteCarlo.CalculateUpperLowerLimits()
                        Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified,
                                                     eCoreComponentType.Ecosim, eMessageImportance.Maintenance, eDataTypes.MonteCarlo))

                End Select

            Case eDataTypes.MSEFleetInput

                'Something in the fisheries regulation data has changed 
                'update all the variables
                Me.MSEManager.UpdateAssesmentVars()

                'jb if the fisheries quotas are changed, make sure the status flags are reset 
                'the client may have edited values that are not editable
                obj.ResetStatusFlags()


            Case eDataTypes.MSEGroupInput

                obj.ResetStatusFlags()

                Me.m_publisher.AddMessage(New cMessage("", eMessageType.DataModified,
                             eCoreComponentType.MSE, eMessageImportance.Maintenance, eDataTypes.MSEGroupInput))

            Case eDataTypes.EcospaceBasemap

                If value.varName = eVarNameFlags.Latitude Then
                    Me.m_EcospaceData.CalculateRelCellWidths()
                End If

        End Select

        ' Update multi-stanza info
        If (bRecalcStanza) Then
            ' Recalc stanza parms
            Me.m_Ecosim.InitStanza()
            ' Update GUI objects
            Me.LoadStanzas()
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for non-<see cref="cCoreInputOutputBase"/> object to report changes to the core
    ''' </summary>
    ''' <param name="obj">Reference to a <see cref="ICoreInterface"/> instance
    ''' that has changed its data.</param>
    ''' <param name="TypeOfChange">Flag stating how the object was changed</param>
    ''' <remarks> <para>This provides a public generic interface for any core object to communicate with the core. 
    ''' The nature of the comunication can be defined by the ICoreInterface object.</para> 
    ''' <para>Not all core objects can be fit into a cCoreInputOutputBase interface. This 
    ''' provides a way for these object to comumicate changes to the core.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub onChanged(obj As ICoreInterface,
                         Optional TypeOfChange As eMessageType = eMessageType.NotSet)
        Dim manager As cBaseShapeManager = Nothing

        Try
            Select Case obj.DataType

                Case eDataTypes.PriceMediation, eDataTypes.Mediation ', eDataTypes.CapacityMediation
                    'jb 18-Aug-2011 Capacity functions (cEnviroResponseFunction) don't have an init function
                    If obj.DataType = eDataTypes.PriceMediation Then
                        Me.m_Ecosim.InitializePriceFunctions()
                    ElseIf obj.DataType = eDataTypes.Mediation Then
                        Me.m_Ecosim.InitializeMedFunctions()
                    End If
                    Me.m_publisher.AddMessage(New cMessage("Mediation shape has changed", TypeOfChange, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, obj.DataType))

                Case eDataTypes.CapacityMediation

                    Me.m_publisher.AddMessage(New cMessage("Capacity shape has changed", TypeOfChange, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, obj.DataType))

                Case eDataTypes.PredPreyInteraction
                    Me.m_publisher.AddMessage(New cMessage("PredPrey interactions changed.", TypeOfChange, eCoreComponentType.MediatedInteractionManager, eMessageImportance.Maintenance))

                Case eDataTypes.LandingInteraction
                    Me.m_publisher.AddMessage(New cMessage("Landings interactions changed.", TypeOfChange, eCoreComponentType.MediatedInteractionManager, eMessageImportance.Maintenance))

                Case eDataTypes.Forcing, eDataTypes.EggProd

                    If (obj.DataType = eDataTypes.Forcing Or obj.DataType = eDataTypes.EggProd) Then

                        If (TypeOfChange = eMessageType.DataAddedOrRemoved) Then
                            ' Special case: If a Forcing or EggProd object was added/removed then both these 
                            '               managers need to reload their data as they share the same array data
                            manager = m_ShapeManagers.Item(eDataTypes.EggProd)
                            manager.Load()

                            manager = m_ShapeManagers.Item(eDataTypes.Forcing)
                            manager.Load()

                            Me.m_EcosimEnviroResponseManager.Load(Me.ForcingShapeManager)
                            Me.m_MediatedInteractionManager.Load()
                            Me.m_EcosimMortalityResponseManager.Load(Me.ForcingShapeManager)
                        End If
                    End If

                    If TypeOfChange = eMessageType.DataAddedOrRemoved Then
                        ' Only send out ONE message
                        Me.m_publisher.AddMessage(New cMessage("Shape added or removed.", eMessageType.DataAddedOrRemoved,
                                     eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, obj.DataType))
                    End If

                    If TypeOfChange = eMessageType.DataModified Then
                        ' Only send out ONE message
                        Me.m_publisher.AddMessage(New cMessage("Shape modified.", eMessageType.DataModified,
                                     eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, obj.DataType))
                    End If

                Case eDataTypes.FishMort
                    'Debug.Assert(False, "Core.OnChange() Fishing Mort Shape need to update Forced F")

                    Dim bForced As Boolean = False
                    For igrp As Integer = 1 To nGroups
                        For it As Integer = 1 To Me.m_EcoSimData.NTimes
                            If Me.m_TSData.ForcedFs(igrp, it) >= 0 Then
                                Me.m_EcoSimData.FishRateNo(igrp, it) = Me.m_TSData.ForcedFs(igrp, it)
                                bForced = True
                            End If
                        Next
                    Next

                    If bForced Then
                        manager = m_ShapeManagers.Item(eDataTypes.FishMort)
                        manager.Load()
                    End If

                    Me.m_publisher.AddMessage(New cMessage("Fish mort shape modified", TypeOfChange, eCoreComponentType.ShapesManager, eMessageImportance.Maintenance, eDataTypes.FishMort))

                Case eDataTypes.FishingEffort, eDataTypes.FishingPolicyManager
                    'Dim qyear() As Single
                    'ReDim qyear(Me.nGroups)
                    'For i As Integer = 1 To Me.nFleets : qyear(i) = 1 : Next

                    '25-Jan-2017 Back again???
                    'this will only update F if there is no F time series loaded
                    If obj.DataType = eDataTypes.FishingEffort Then
                        Me.m_Ecosim.SetBaseFFromGear()
                    End If

                    'JB 21-Feb-2011 No longer set F to base if Effort has been edited
                    'this allows the user the edit effort when F timeseries is loaded
                    'reset the mortaility due to fishing to the new values
                    '  Me.m_EcoSim.SetBaseFFromGear()

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

                Case eDataTypes.EcosimEnviroResponseFunctionManager
                    Me.m_publisher.AddMessage(New cMessage("Ecosim environmental responses modified", eMessageType.DataModified, obj.CoreComponent, eMessageImportance.Maintenance, eDataTypes.EcosimEnviroResponseFunctionManager))

                Case eDataTypes.EcosimMortalityResponseFunctionManager
                    Me.m_publisher.AddMessage(New cMessage("Ecosim environmental responses modified", eMessageType.DataModified, obj.CoreComponent, eMessageImportance.Maintenance, eDataTypes.EcosimMortalityResponseFunctionManager))

                Case eDataTypes.EcosimArenaShare
                    If cCore.USE_SHARED_ARENAS Then
                        Me.m_ArenaManager.Load()
                        Me.m_publisher.AddMessage(New cMessage("Arenas modified", TypeOfChange, eCoreComponentType.Ecosim, eMessageImportance.Maintenance, eDataTypes.EcosimArenaShare))
                    End If


                Case eDataTypes.EcospaceLayerDepth, eDataTypes.EcospaceLayerHabitat

                    Me.m_Ecospace.UpdateDepthMap()

                    ' Recalc habitat area
                    Me.LoadEcospaceHabitats()

                    'update the map/response interactions to the new data
                    Me.m_mapInteractionManager.Update()

                    ''Input map(s) may have change
                    ''Let Ecospace decide what to update in response
                    'If Me.m_Ecospace.UpdateMaps(obj.DataType) Then
                    '    'Capacity layer has changed
                    '    'send out a message
                    '    Me.m_publisher.AddMessage(New cMessage("Ecospace capacity map may have changed.", eMessageType.DataModified,
                    '                                           eCoreComponentType.EcoSpace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerHabitatCapacity))
                    'End If

                    Me.m_publisher.AddMessage(New cMessage("Ecospace basemap changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerDepth))
                    Me.m_publisher.AddMessage(New cMessage("Ecospace habitats changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceHabitat))

                    If (obj.DataType = eDataTypes.EcospaceLayerDepth) Then
                        ' Depth layer changes invalidate all layer stats
                        For Each l As cEcospaceLayer In Me.EcospaceBasemap.Layers
                            l.Invalidate()
                        Next
                    Else
                        For Each l As cEcospaceLayer In Me.EcospaceBasemap.Layers(eVarNameFlags.LayerHabitat)
                            l.Invalidate()
                        Next
                    End If

                Case eDataTypes.EcospaceLayerMPA,
                     eDataTypes.EcospaceLayerImportance,
                     eDataTypes.EcospaceLayerRegion,
                     eDataTypes.EcospaceLayerContaminantRelativeDistribution,
                     eDataTypes.EcospaceLayerRelPP,
                     eDataTypes.EcospaceLayerPort,
                     eDataTypes.EcospaceLayerSail,
                     eDataTypes.EcospaceLayerMigration,
                     eDataTypes.EcospaceLayerFlow,
                     eDataTypes.EcospaceLayerUpwelling,
                     eDataTypes.EcospaceLayerHabitatCapacityInput,
                     eDataTypes.EcospaceLayerExclusion,
                     eDataTypes.EcospaceLayerDriver

                    DirectCast(obj, cEcospaceLayer).Invalidate()

                    ' Exclusion layer changes invalidate all layer stats
                    If (obj.DataType = eDataTypes.EcospaceLayerExclusion) Then
                        Me.m_Ecospace.UpdateDepthMap()
                        Me.m_Ecospace.CalcHabitatArea()
                        Me.EcospaceBasemap.nCells = Me.m_EcospaceData.ThabArea
                        For Each l As cEcospaceLayer In Me.EcospaceBasemap.Layers
                            l.Invalidate()
                        Next
                    End If

                    'update the map/response interactions to the new data
                    Me.m_mapInteractionManager.Update()

                    Me.m_publisher.AddMessage(New cMessage("Ecospace layer changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, obj.DataType))

                Case eDataTypes.EcospaceHabitat
                    ' NOP

                Case eDataTypes.Stanza
                    'A user has called Apply() on a stanza group. Update the underlying data Stanza and Ecopath
                    Me.UpdateStanza(obj.DBID)

                    'UpdateStanza() updated the cores ecopath data with values computed by CalculateStanzaParameters()
                    'reload the input objects
                    Me.LoadEcopathInputs()

                    'The Stanza object knows that it has changed make sure anything else that is listening knows as well
                    Me.m_publisher.AddMessage(New cMessage("Stanza group changed.", eMessageType.DataModified, eCoreComponentType.Ecosim,
                                                            eMessageImportance.Maintenance, eDataTypes.Stanza))

                    'Ecopath Message
                    Me.m_publisher.AddMessage(New cMessage("Stanza group changed Ecopath values.", eMessageType.DataModified, eCoreComponentType.Ecopath,
                                       eMessageImportance.Maintenance, eDataTypes.EcoPathGroupInput))

                    'Tell the data source that both Ecopath and Stanza data need saving. May not need to do this it may be good enough that the stanza data is dirty
                    DataSource.SetChanged(eCoreComponentType.Ecopath)
                    ' JS 23Nov10: only flag sim as dirty when sim is loaded, hm?
                    If Me.m_StateMonitor.HasEcosimLoaded Then
                        DataSource.SetChanged(eCoreComponentType.Ecosim)
                    End If
                    ' Ecopath needs to run again
                    Me.StateMonitor.SetEcopathLoaded(True)

                Case eDataTypes.GroupTimeSeries, eDataTypes.FleetTimeSeries
                    ' Reload
                    If Me.UpdateEcosimTimeSeries() Then Me.m_TSData.loadEnabled(obj.Index)
                    Me.m_SearchManagers(eDataTypes.FitToTimeSeries).Load()

                    Me.m_publisher.AddMessage(New cMessage("Time series have changed.", eMessageType.DataModified,
                        eCoreComponentType.TimeSeries, eMessageImportance.Maintenance))

                Case eDataTypes.PedigreeLevel
                    Dim level As cPedigreeLevel = DirectCast(obj, cPedigreeLevel)
                    Dim man As cPedigreeManager = Me.GetPedigreeManager(level.VariableName)
                    Me.LoadPedigreeLevels(man)

                    Me.m_publisher.AddMessage(New cMessage("Pedigree levels have changed.", eMessageType.DataModified,
                                                           level.CoreComponent, eMessageImportance.Maintenance))

                Case eDataTypes.PedigreeManager
                    Dim man As cPedigreeManager = DirectCast(obj, cPedigreeManager)
                    man.LoadPedigree()

                    Me.m_publisher.AddMessage(New cMessage("Pedigree assignments have changed.", eMessageType.DataModified,
                                       man.CoreComponent, eMessageImportance.Maintenance))

                Case eDataTypes.MonteCarlo
                    Me.LoadEcopathInputs()
                    Me.LoadEcopathFleetInputs()
                    Me.LoadEcosimGroups()

                    Me.m_publisher.AddMessage(New cMessage("Monte carlo data has changed.", eMessageType.DataModified,
                                       eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Maintenance))

                Case eDataTypes.EcopathSample
                    Me.LoadEcopathInputs()
                    Me.LoadEcopathFleetInputs()

                    Me.m_publisher.AddMessage(New cMessage("Sample data is loaded.", eMessageType.DataModified,
                                       eCoreComponentType.Ecopath, eMessageImportance.Maintenance))

                Case eDataTypes.EcospaceEnviroCapacityResponse
                    If obj.CoreComponent = eCoreComponentType.EcospaceCapacityResponseInteractionManager Then
                        Me.m_publisher.AddMessage(New cMessage("Capacity map data has changed.", TypeOfChange,
                                      eCoreComponentType.EcospaceCapacityResponseInteractionManager, eMessageImportance.Maintenance))
                    End If

                Case eDataTypes.EcospaceEnviroMortalityResponse
                    If obj.CoreComponent = eCoreComponentType.EcospaceMortalityResponseInteractionManager Then

                        Me.m_publisher.AddMessage(New cMessage("Mortality map data has changed.", TypeOfChange,
                                      eCoreComponentType.EcospaceMortalityResponseInteractionManager, eMessageImportance.Maintenance))
                    End If


                Case eDataTypes.EcospaceSpatialDataConnection
                    Me.m_publisher.AddMessage(New cMessage("Spatial data configuration changed.", TypeOfChange, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceSpatialDataConnection))

                Case eDataTypes.EcospaceLayerExclusion
                    'Update the Depth map based on the Exlusion layer
                    Me.m_Ecospace.UpdateDepthMap()

                    Me.m_publisher.AddMessage(New cMessage("Depth map update to exclusion layer.", eMessageType.DataModified,
                                      eCoreComponentType.Ecospace, eMessageImportance.Maintenance))

                Case eDataTypes.EcospaceSpatialDataSource
                    Me.m_publisher.AddMessage(New cMessage("External data configuration changed.", eMessageType.DataModified,
                                      eCoreComponentType.Ecospace, eMessageImportance.Maintenance, obj.DataType))

                Case eDataTypes.EcospaceLayerUpwelling
                    Me.EcospaceBasemap.LayerUpwelling.Invalidate()
                    Me.m_publisher.AddMessage(New cMessage("Ecospace upwelling map changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerUpwelling))

                Case eDataTypes.EcospaceLayerAdvection
                    For Each l As cEcospaceLayer In Me.EcospaceBasemap.LayerAdvection
                        l.Invalidate()
                    Next
                    Me.m_publisher.AddMessage(New cMessage("Ecospace advection maps changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerAdvection))

                Case eDataTypes.EcospaceLayerWind
                    For Each l As cEcospaceLayer In Me.EcospaceBasemap.LayerWind
                        l.Invalidate()
                    Next
                    Me.m_publisher.AddMessage(New cMessage("Ecospace wind maps changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerWind))

                Case eDataTypes.EcospaceLayerHabitatCapacity
                    For Each l As cEcospaceLayer In Me.EcospaceBasemap.Layers(eVarNameFlags.LayerHabitatCapacity)
                        l.Invalidate()
                    Next
                    Me.m_publisher.AddMessage(New cMessage("Ecospace computed capacity changed.", eMessageType.DataModified, eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerHabitatCapacity))

            End Select

            'Ecospace map input map(s) may have change
            'Let Ecospace decide what to update in response


            If Me.m_Ecospace.UpdateMaps(obj.DataType) Then
                'Capacity layer has changed
                'send out a message
                Me.m_publisher.AddMessage(New cMessage("Ecospace capacity map may have changed.", eMessageType.DataModified,
                                                       eCoreComponentType.Ecospace, eMessageImportance.Maintenance, eDataTypes.EcospaceLayerHabitatCapacity))
            End If

            ' JS 31aug07: DataAddedOrRemoved messages are initialized by the db, thus the db should not get flagged as dirty
            If (TypeOfChange <> eMessageType.DataAddedOrRemoved) And (Me.DataSource IsNot Nothing) Then
                ' Update data state
                DataSource.SetChanged(obj.CoreComponent)
                Me.m_StateMonitor.UpdateDataState(DataSource)
            End If

            ' Do not interrupt executions
            If (Not Me.m_StateMonitor.IsBusy) Then
                Me.m_StateMonitor.UpdateExecutionState(obj.CoreComponent)
            End If

            Try
                If Me.PluginManager IsNot Nothing Then
                    Me.PluginManager.DataValidated(eVarNameFlags.NotSet, obj.DataType)
                End If
            Catch ex As Exception
                ' NOP
            End Try

            Me.m_publisher.sendAllMessages()

        Catch ex As Exception

            m_logger.LogError(ex, "onChanged Exception:{message}", ex.Message)
            'maybe a better message than this
            Me.m_publisher.AddMessage(New cMessage("Error in " & Me.ToString & ".OnChanged(). " & ex.Message,
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
        Set(pm As cPluginManager)
            ' Remember plugin manager
            Me.m_pluginManager = pm
            ' Hand plugin manager to components
            Me.m_Ecopath.PluginManager = pm
            Me.m_Ecosim.PluginManager = pm
            Me.m_Ecospace.PluginManager = pm
            Me.m_publisher.PluginManager = pm

            If (Me.m_pluginManager IsNot Nothing) Then
                ' Hand plugin manager a delegate to check core enabled state
                Me.m_pluginManager.CoreExecutionStateDelegate = New cPluginManager.CanExecutePlugin(AddressOf Me.CanExecutePlugin)

                If Not ReferenceEquals(Me.m_pluginManager.Core, Me) Then
                    Me.m_pluginManager.Core = Me
                End If

            End If
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in manager event handler, caught to provide message feedback about loaded plug-in assemblies.
    ''' </summary>
    ''' <param name="paAdded">A loaded <see cref="cPluginAssembly">plug-in assembly</see>.</param>
    ''' -----------------------------------------------------------------------
    Private Sub m_pluginManager_AssemblyAdded(paAdded As EwECore.cPluginAssembly) _
        Handles m_pluginManager.AssemblyAdded

        Me.m_publisher.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STATUS_PLUGIN_SANDBOXED, Path.GetFileNameWithoutExtension(paAdded.Filename)),
                                                eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information))
        'AddHandler paAdded.AssemblyEnabled, AddressOf OnPluginAssemblyStateChanged
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in manager event handler, caught to provide message feedback about removed plug-in assemblies.
    ''' </summary>
    ''' <param name="paRemoved">A removed <see cref="cPluginAssembly">plug-in assembly</see>.</param>
    ''' -----------------------------------------------------------------------
    Private Sub m_pluginManager_AssemblyRemoved(paRemoved As EwECore.cPluginAssembly) _
        Handles m_pluginManager.AssemblyRemoved

        m_publisher.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STATUS_PLUGIN_UNLOADED, Path.GetFileNameWithoutExtension(paRemoved.Filename)),
                                             eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information))
        'RemoveHandler paRemoved.AssemblyEnabled, AddressOf OnPluginAssemblyStateChanged
    End Sub

    Private Sub m_pluginManager_AssemblyUserDisabled(strPluginName As String) _
        Handles m_pluginManager.AssemblyUserDisabled

        m_publisher.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STATUS_PLUGIN_USERDISABLED, Path.GetFileNameWithoutExtension(strPluginName)),
                                     eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information))

    End Sub

    'Private Sub OnPluginAssemblyStateChanged(pa As cPluginAssembly, bEnabled As Boolean)

    '    If (pa Is Nothing) Then Return
    '    If (pa.Plugins(GetType(IEconomicData)) IsNot Nothing) Then
    '        Me.OnEconomicDataPluginEnabled()
    '    End If

    'End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The Plug-in Manager has caught an exception thrown by a plugin.
    ''' </summary>
    ''' <param name="ex">The <see cref="cPluginException"/> that was thrown.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnPluginException(ex As cPluginException) _
        Handles m_pluginManager.PluginException

        If (ex.Assembly Is Nothing) Then
            Debug.Assert(False)
            Return
        End If

        If ex.Assembly.AlwaysEnabled Then
            Dim msg As cMessage = New cMessage(ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.External, eMessageImportance.Warning)
            Me.m_publisher.SendMessage(msg)
            Return
        End If

        Dim fmsg As New cFeedbackMessage(
                cStringUtils.Localize(My.Resources.CoreMessages.PLUGIN_PROMPT_DISABLE, ex.Message, Environment.NewLine),
                eCoreComponentType.External, eMessageType.Any,
                eMessageImportance.Warning,
                eMessageReplyStyle.YES_NO, eDataTypes.NotSet, eMessageReply.YES)

        Me.m_publisher.SendMessage(fmsg)
        ex.Assembly.Enabled = (fmsg.Reply = eMessageReply.NO)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Callback for <see cref="cPluginManager.CanExecutePlugin">Plug-in manager CanExecutePlugin delegate</see>,
    ''' which the plug-in manager must invoke to test if a plug-in can be enabled by testing a given 
    ''' <see cref="eCoreExecutionState">Core execution state</see> against the
    ''' <see cref="cCoreStateMonitor.CoreExecutionState">current core execution state</see>.
    ''' </summary>
    ''' <param name="coreExecutionState">The <see cref="eCoreExecutionState">Core execution state</see> to test.</param>
    ''' <returns>True if the current core state enables to tested core execution state.</returns>
    ''' -----------------------------------------------------------------------
    Private Function CanExecutePlugin(coreExecutionState As eCoreExecutionState) As Boolean
        Return Me.m_StateMonitor.IsExecutionStateSuperceded(coreExecutionState)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, triggered when the <see cref="cCoreStateMonitor.CoreExecutionStateEvent">Core State Monitor</see>
    ''' execution state has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_StateMonitor_CoreExecutionStateEvent(csm As cCoreStateMonitor) _
        Handles m_StateMonitor.CoreExecutionStateEvent

        If (Me.PluginManager IsNot Nothing) Then
            ' Inform the plugin manager of the new core state.
            Me.PluginManager.UpdatePluginEnabledStates()
        End If

        If (Not Me.m_StateMonitor.IsSearching And Not Me.m_StateMonitor.IsBusy) Then
            ' Remove any pending handbrake
            Me.SetStopRunDelegate(Nothing)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The enabled state of an Economic plugin has changed (e.g. ValueChain)
    ''' </summary>
    ''' -----------------------------------------------------------------------
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
                m_logger.LogError(ex, "SearchObjective() not available: {message}", ex.Message)
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
    Private Sub CreateSearchManagers()

        Me.m_SearchData = New cSearchDatastructures(Me.m_Functions, Me.m_EcopathData)
        AddHandler Me.m_SearchData.OnSearchStateChanged, AddressOf OnSearchChanged

        Me.m_SearchManagers.Add(eDataTypes.SearchObjectiveManager, New cSearchObjective)
        Me.m_SearchManagers.Add(eDataTypes.MSEManager, New cMSEManager(Me, Me.m_MSEData))
        Me.m_SearchManagers.Add(eDataTypes.MPAOptManager, New cMPAOptManager)
        Me.m_SearchManagers.Add(eDataTypes.FishingPolicyManager, New cFishingPolicyManager)
        Me.m_SearchManagers.Add(eDataTypes.FitToTimeSeries, New cF2TSManager(Me))

        Me.m_SearchManagers.Add(eDataTypes.MSYManager, New MSY.cMSYManager(Me, Me.m_MSYData))

    End Sub

    Private Sub ClearSearchManagers()
        Try
            'Search manager are initialized each time a model is loaded
            For Each man As ISearchObjective In Me.m_SearchManagers.Values
                Try
                    man.Clear()
                Catch ex As Exception
                    'just for robustness 
                    'if one of the managers throws an exception the remaining managers will still be cleared
                    m_logger.LogError(ex, "ClearSearchManagers Exception:{message}", ex.Message)
                End Try

            Next
        Catch ex As Exception

        End Try
    End Sub

    'Private Sub DestroySearchManagers()
    '    Try
    '        'Search manager are initialized each time a model is loaded
    '        For Each man As ISearchObjective In Me.m_SearchManagers.Values
    '            man.Clear()
    '        Next
    '        Me.m_SearchManagers.Clear()
    '        If Me.m_SearchData IsNot Nothing Then
    '            Me.m_SearchData.Dispose()
    '            RemoveHandler Me.m_SearchData.OnSearchStateChanged, AddressOf Me.OnSearchChanged
    '        End If
    '    Catch ex As Exception

    '    End Try
    'End Sub

    Private Sub OnSearchChanged(searchmode As eSearchModes)
        Me.m_StateMonitor.SetIsSearching(searchmode)
    End Sub

#Region "Fishing Policy Search"

    Public ReadOnly Property FishingPolicyManager() As cFishingPolicyManager
        Get
            Try

                Return DirectCast(Me.m_SearchManagers(eDataTypes.FishingPolicyManager), cFishingPolicyManager)

            Catch ex As Exception
                m_logger.LogError(ex, "FishingPolicyManager() not available: {message}", ex.Message)
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
                m_logger.LogError(ex, "MPAOptimizationManager() not available: {message}", ex.Message)
                Return Nothing
            End Try

        End Get
    End Property

#End Region

#Region "MSE"


    Public ReadOnly Property MSEManager() As cMSEManager
        Get
            Try
                Return DirectCast(Me.m_SearchManagers.Item(eDataTypes.MSEManager), cMSEManager)
            Catch ex As Exception
                m_logger.LogError(ex, "MSEManager() not available: {message}", ex.Message)
                Return Nothing
            End Try

        End Get
    End Property

    Public ReadOnly Property MSEBatchManager() As MSEBatchManager.cMSEBatchManager
        Get
            Try
                Return DirectCast(Me.m_SearchManagers.Item(eDataTypes.MSEManager), cMSEManager).MSEBatchManager
            Catch ex As Exception
                m_logger.LogError(ex, "MSEBatchManager() not available: {message}", ex.Message)
                Return Nothing
            End Try
        End Get
    End Property

#End Region

#Region "MSY"

    Public ReadOnly Property MSYManager() As MSY.cMSYManager
        Get
            Try
                Return DirectCast(Me.m_SearchManagers.Item(eDataTypes.MSYManager), MSY.cMSYManager)
            Catch ex As Exception
                m_logger.LogError(ex, "MSYManager() not available: {message}", ex.Message)
                Return Nothing
            End Try

        End Get
    End Property

#End Region

#End Region ' Search managers

#Region " Pedigree "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a pedigree level to a loaded EwE model.
    ''' </summary>
    ''' <param name="varName"><see cref="eVarNameFlags">Variable to assign new level to.</see></param>
    ''' <param name="iPosition">One-based position in the list of pedigree levels for this particular <paramref name="varName"/></param>
    ''' <param name="strName">Name for the new level.</param>
    ''' <param name="iColor">Color for the new level.</param>
    ''' <param name="strDescription">Description for the new level.</param>
    ''' <param name="sIndexValue"></param>
    ''' <param name="sConfidence"></param>
    ''' <param name="iDBID"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function AddPedigreeLevel(varName As eVarNameFlags,
                                     iPosition As Integer,
                                     strName As String,
                                     iColor As Integer,
                                     strDescription As String,
                                     sIndexValue As Single,
                                     sConfidence As Single,
                                     ByRef iDBID As Integer) As Boolean

        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecopath) Then Return False

        ' Issue #796: in some daily build databases the description field cannot be empty
        If String.IsNullOrEmpty(strDescription) Then strDescription = " "

        ds = DirectCast(Me.DataSource, IEcopathDataSource)

        Return ds.AddPedigreeLevel(iPosition, strName, iColor, strDescription, varName, sIndexValue, sConfidence, iDBID)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an existing pedigree level.
    ''' </summary>
    ''' <param name="iLevelDBID">The <see cref="cPedigreeLevel.DBID"/> of the level to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemovePedigreeLevel(iLevelDBID As Integer) As Boolean

        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        If Not Me.SaveChanges(False, eBatchChangeLevelFlags.Ecopath) Then Return False

        ds = DirectCast(Me.DataSource, IEcopathDataSource)

        Return ds.RemovePedigreeLevel(iLevelDBID)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Move a pedigree level to a new location in the pedigree levels list.
    ''' </summary>
    ''' <param name="iLevelDBID">The <see cref="cPedigreeLevel.DBID"/> of the level to move.</param>
    ''' <param name="iIndex">The new posiition to move the level to.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function MovePedigreeLevel(iLevelDBID As Integer, iIndex As Integer) As Boolean
        Dim bSucces As Boolean = False
        Dim ds As IEcopathDataSource = Nothing

        ' Sanity checks
        If (Not Me.CanSave(True)) Then Return False
        If (Not TypeOf (Me.DataSource) Is IEcopathDataSource) Then Return False

        ' Increase batch count
        If Not Me.SetBatchLock(eBatchLockType.Restructure) Then Return False

        ds = DirectCast(DataSource, IEcopathDataSource)
        If ds.MovePedigreeLevel(iLevelDBID, iIndex) Then
            Me.DataModifiedMessage("Ecopath pedigree order has changed.", eCoreComponentType.Ecopath, eDataTypes.PedigreeLevel)
            bSucces = True
        End If

        ' Decrease batch count
        ReleaseBatchLock(eBatchChangeLevelFlags.Ecopath)

        Return bSucces

    End Function

#End Region ' Pedigree

#Region " Samples "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the only instance of the <see cref="cEcopathSampleManager"/>.
    ''' </summary>
    ''' <returns>
    ''' The only instance of the <see cref="cEcopathSampleManager"/>.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function SampleManager() As Samples.cEcopathSampleManager
        Return Me.m_SampleManager
    End Function

    Private Function InitEcopathSamples() As Boolean
        Me.SampleManager.Init()
        Return True
    End Function

#End Region ' Samples

#Region " Eco Functions "
    Private Sub initEcoFunctions()
        Try
            Me.m_Functions.Init(Me)
        Catch ex As Exception
            m_logger.LogError(ex, "initEcoFunctions Exception:{message}", ex.Message)
            Debug.Assert(False)
        End Try
    End Sub

    ''' <summary>
    ''' Get the single <see cref="cEcoFunctions"/> instance.
    ''' </summary>
    Public ReadOnly Property EcoFunction() As cEcoFunctions
        Get
            Return Me.m_Functions
        End Get
    End Property

#End Region

#If Not NET Then
#Region " License "

    
    Public ReadOnly Property License As cLicense
        Get
            If (Me.m_license Is Nothing) Then
                Me.m_license = New cLicense()
            End If
            Return Me.m_license
        End Get
    End Property

#End Region ' License
#End If

#Region " Deprecated "

    <Obsolete("Please use nEcosimScenarios instead")>
    Public ReadOnly Property EcosimScenarioCount() As Integer
        Get
            Try
                Return Me.nEcosimScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    <Obsolete("Use nEcospaceScenarios instead")>
    Public ReadOnly Property EcospaceScenarioCount() As Integer
        Get
            Try
                Return Me.nEcospaceScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    <Obsolete("Use nEcotracerScenarios instead")>
    Public ReadOnly Property EcotracerScenarioCount() As Integer
        Get
            Try
                Return Me.nEcotracerScenarios
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    <Obsolete("Use EcospaceGroupInputs instead")>
    Public ReadOnly Property EcospaceGroups(iGroup As Integer) As cEcospaceGroupInput
        Get
            Return Me.EcospaceGroupInputs(iGroup)
        End Get
    End Property

    <Obsolete("Use EcospaceFleetInputs instead")>
    Public ReadOnly Property EcospaceFleets(iFleet As Integer) As cEcospaceFleetInput
        Get
            Return Me.EcospaceFleetInputs(iFleet)
        End Get
    End Property

    ''' <inheritdocs cref="nEnvironmentalDriverLayers"/>
    <Obsolete("Use nEnvironmentalDriverLayers instead")>
    Public ReadOnly Property nEnvironmentalLayers() As Integer
        Get
            Return Me.nEnvironmentalDriverLayers
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an Ecosim Time Series to the data source.
    ''' </summary>
    ''' <param name="strName">Name of the new Time Series to add.</param>
    ''' <param name="iPool">Index of item to assign this TS to.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of the time series.</param>
    ''' <param name="asValues">Initial values to set in the TS.</param>
    ''' <param name="iDBID">Database ID assigned to the new TS.</param>
    ''' -----------------------------------------------------------------------
    <Obsolete("Use AddTimeSeries(String, Integer, Integer, eTimeSeriesType, Single, Single(), ByRef Integer) instead")>
    Public Function AddTimeSeries(strName As String,
                                  iPool As Integer,
                                  timeSeriesType As eTimeSeriesType,
                                  sWeight As Single, asValues() As Single,
                                  ByRef iDBID As Integer) As Boolean
        Return Me.AddTimeSeries(strName, iPool, 0, timeSeriesType, sWeight, asValues, iDBID)
    End Function

#End Region

End Class
