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
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwECore.Ecosim
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cSFPManager

#Region " Private vars "

    ' -- Admin --
    ''' <summary>Filename to be removed when the run completes</summary>
    Private m_strTempFileName As String
    ''' <summary>All available STF interations</summary>
    Private m_iterations As New List(Of ISFPIteration)

    ' -- Multi-threaded running
    Private m_queue As New Stack(Of ISFPIteration)
    Private m_containers As New List(Of cSFPContainer)
    Private m_statusmsg As cMessage = Nothing
    Private m_iQueueLength As Integer = 0
    Private m_iQueueDone As Integer = 0

    ' -- State flags --

    ''' <summary>Flag, stating whether a run is in progress.</summary>
    Private m_bIsRunning As Boolean = False
    ''' <summary>Flag, stating whether a run abortion has been requested.</summary>
    Private m_bStopRun As Boolean = False

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for stand-alone app
    ''' </summary>
    ''' <remarks>
    ''' In this modus, the SFP manager is in full control over its own core.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        'Create a new core
        Me.New(New cCore())
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor when used in a plug-in environment.
    ''' </summary>
    ''' <remarks>
    ''' In this modus, the SFP manager adheres to choices made in a core managed
    ''' by EwE.
    ''' </remarks>
    ''' <param name="core">The core instance to initialize to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore)
        Me.Parameters = New cSFPParameters(core)
    End Sub

#Region " Load user Inputs "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the model in the selected file and keep a reference of the file path
    ''' </summary>
    ''' <returns>True if load successful</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadModel(strFileName As String) As Boolean
        Me.Parameters.ModelFileName = ""
        If Me.Core.LoadModel(strFileName) Then
            Me.Parameters.ModelFileName = strFileName
        End If
        Return Not String.IsNullOrWhiteSpace(Me.Parameters.ModelFileName)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the Ecosim Scenario from selected index and keep a reference of the scenario
    ''' </summary>
    ''' <param name="iScenario">One-based Ecosim scenario index.</param>
    ''' <returns>True if load successful</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadEcoSimScenario(iScenario As Integer) As Boolean
        Return Me.Core.LoadEcosimScenario(iScenario)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets a list of names of all the Ecosim Scenarios from the core
    ''' </summary>
    ''' <returns>String List of Ecosim Scenario names </returns>
    ''' -----------------------------------------------------------------------
    Public Function GetAvailableScenarioNames() As List(Of String)
        Dim lscenarios As New List(Of String)
        Dim scenario As cEcoSimScenario

        For iScenario As Integer = 1 To Me.Core.nEcosimScenarios
            scenario = Me.Core.EcosimScenarios(iScenario)
            lscenarios.Add(scenario.Name)
        Next
        Return lscenarios
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the Time Series from selected index and keep a reference of the Time Series
    ''' </summary>
    ''' <param name="tsi">One-based time series dataset index, just as used in the
    ''' EwE core.</param>
    ''' <returns>True if load successful</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadTimeSeries(tsi As Integer) As Boolean

        Dim bSuccess As Boolean = False

        'Try to load time series
        If Me.Core.LoadTimeSeries(tsi) Then
            'Store a reference to time series index in SFPManager
            Me.Parameters.TimeSeriesDataset = tsi
            Console.WriteLine("Time Series : " & Me.Core.TimeSeriesDataset(tsi).Name & " Loaded successfully")
            bSuccess = True
        Else
            Console.WriteLine("Time Series could not Load")
            Me.Parameters.TimeSeriesDataset = -1
            bSuccess = False
        End If

        Me.Refresh(0)
        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets a list of names of all the Time Series from the core
    ''' </summary>
    ''' <returns>String List of Time Series names </returns>
    ''' -----------------------------------------------------------------------
    Public Function GetAvailableTimeSeriesNames() As List(Of String)
        Dim lTimeSeries As New List(Of String)
        Dim TimeSeries As cTimeSeriesDataset = Nothing

        For iTimeSeries As Integer = 1 To Me.Core.nTimeSeriesDatasets
            TimeSeries = Me.Core.TimeSeriesDataset(iTimeSeries)
            lTimeSeries.Add(TimeSeries.Name)
        Next
        Return lTimeSeries
    End Function

    Public Function GetAvailableAnomalyShapes() As cShapeData()

        Dim interactions As cMediatedInteractionManager = Me.Core.MediatedInteractionManager
        Dim shapes As New List(Of cShapeData)

        Dim lPP As New List(Of Integer)
        For iGroup As Integer = 1 To Me.Core.nGroups
            Dim grp As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iGroup)
            If (grp.IsProducer) Then
                lPP.Add(iGroup)
            End If
        Next

        For Each iGroup As Integer In lPP
            Dim interact As cPredPreyInteraction = interactions.PredPreyInteraction(iGroup, iGroup)
            If (interact IsNot Nothing) Then
                Dim shape As cForcingFunction = Nothing
                Dim ft As eForcingFunctionApplication = eForcingFunctionApplication.NotSet
                For i As Integer = 1 To interact.nAppliedShapes
                    If (interact.getShape(i, shape, ft)) Then
                        If (Not shapes.Contains(shape)) Then
                            shapes.Add(shape)
                        End If
                    End If
                Next i
            End If
        Next iGroup
        Return shapes.ToArray()

    End Function

    Public Sub Refresh(iPrefK As Integer)

        Me.m_iterations.Clear()

        If (Me.Parameters.EcosimScenario <= 0) Then Return

        ' Always do this
        Me.Parameters.CalculateParameters(iPrefK)
        ' Create list of ISFPIterations
        Me.LoadSFPIterationsList()
    End Sub

#End Region ' Load user inputs

#Region " Load to EwE state "

    Public Sub UpdateToCore()

        ' Sanity checks
        Debug.Assert(Me.Core IsNot Nothing)

        If (Me.Parameters.AnomalyShapeIndex = 0) Then
            Dim shapes As cShapeData() = Me.GetAvailableAnomalyShapes()
            If (shapes.Length > 0) Then Me.Parameters.AnomalyShapeIndex = shapes(0).Index
        End If

        If (Me.Core.StateMonitor.HasEcosimLoaded) Then
            Me.Parameters.EcosimScenario = Me.Core.ActiveEcosimScenarioIndex
            Me.Parameters.TimeSeriesDataset = Me.Core.ActiveTimeSeriesDatasetIndex
        Else
            Me.Parameters.EcosimScenario = -1
            Me.Parameters.TimeSeriesDataset = -1
            Me.m_iterations.Clear()
        End If

        Me.Refresh(0)

    End Sub

#End Region ' Load to EwE state

#Region " Run Iterations "

    Public Sub Run()
        Me.Parameters.PrepareForRun(Me.OutputFolder)
        Me.StartContainerRun()
    End Sub

    Private Sub StartContainerRun()

        If (Me.IsRunning) Then Return
        cLog.VerboseLevel = eVerboseLevel.Disabled
        Me.m_bIsRunning = True

#If NO_PARALLEL Then
        Dim iNumThreads As integer= 1
#Else
        Dim iNumThreads As Integer = Me.Parameters.NumThreads
#End If

        ' Add in reverse order (it's a stack)
        For i As Integer = Me.m_iterations.Count - 1 To 0 Step -1
            Dim it As ISFPIteration = Me.m_iterations(i)
            it.RunState = ISFPIteration.eRunState.Idle
            it.Elapsed = New TimeSpan(0)
            it.IsBestFit = False
            If (it.Enabled) Then Me.m_queue.Push(it)
        Next

        Me.m_statusmsg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_SAVE_SUCCESS, My.Resources.DISPLAYNAME),
                                      eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        Me.m_statusmsg.Hyperlink = Me.OutputFolder

        Me.Core.SetBatchLock(cCore.eBatchLockType.Update)
        Me.Core.StateMonitor.SetIsSearching(eSearchModes.External)
        Me.Core.SetStopRunDelegate(New cCore.StopRunDelegate(AddressOf Me.StopRun))

        Me.m_iQueueDone = 0
        Me.m_iQueueLength = Me.m_queue.Count + 1

        cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_INITIALIZING, My.Resources.DISPLAYNAME), (Me.m_iQueueDone + 0.5!) / Me.m_iQueueLength)

        ' Export model to .eiixml to prevent database clashes
        Dim strModelFile As String = Me.ExportModelToText()
        Me.m_iQueueDone += 1

        ' Do not create containers that aren't going to be doing anything, right?
        For i As Integer = 1 To Math.Min(iNumThreads, m_queue.Count)
            Me.AddContainer(i, strModelFile)
            Me.m_iQueueDone += 1
        Next

        ' Kick off
        Dim k As Integer = 0
        While k < Me.m_containers.Count And Me.m_queue.Count > 0
            Me.HandleIterationUpdate(Me.m_containers(k), Nothing, False)
            k += 1
        End While

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Exports the model to the EIIXML format to reduce database clashes while 
    ''' running iterations.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ExportModelToText() As String

        Dim strSource As String = Me.Core.DataSource.ToString()
        Dim strTempFile As String = cFileUtils.MakeTempFile(".eiixml")

        Dim ds As IEwEDataSource = Me.Core.DataSource
        If Not (TypeOf ds Is cDBDataSource) Then Return strSource
        Dim dbds As cDBDataSource = DirectCast(ds, cDBDataSource)
        If Not (TypeOf dbds.Connection Is cEwEAccessDatabase) Then Return strSource
        Dim db As cEwEAccessDatabase = DirectCast(dbds.Connection, cEwEAccessDatabase)
        ds = cDataSourceFactory.Create(eDataSourceTypes.EIIXML)
        If DirectCast(ds, cEIIXMLDataSource).SaveFromDB(db, strTempFile) Then
            Me.m_strTempFileName = strTempFile
            Return strTempFile
        End If

        Return strSource

    End Function

    Private Sub AddContainer(i As Integer, strModelFile As String)
        Dim cnt As New cSFPContainer("Container_" & i, strModelFile, Me.Parameters)
        AddHandler cnt.OnIterationUpdated, AddressOf Me.HandleIterationUpdate
        Me.m_containers.Add(cnt)
    End Sub

    Private Sub RemoveContainer(cnt As cSFPContainer)
        RemoveHandler cnt.OnIterationUpdated, AddressOf Me.HandleIterationUpdate
        Me.m_containers.Remove(cnt)
    End Sub

    Private Sub HandleIterationUpdate(cnt As cSFPContainer, iteration As ISFPIteration, bDone As Boolean)

        SyncLock Me.m_queue
            ' Process iteration
            If (iteration IsNot Nothing) Then

                Debug.WriteLine(cnt.ToString & ": " & iteration.Name & " = " & iteration.RunState.ToString() & " on " & cnt.Model)

                If (iteration.RunState = ISFPIteration.eRunState.Completed And bDone) Then

                    Debug.WriteLine(iteration.Name & " SS= " & iteration.SS & " AIC= " & iteration.AIC & " AICc= " & iteration.AICc & ", " & iteration.RunState)

                    For Each msg As String In iteration.RunStateMessages
                        Me.AppendStatus(Me.m_statusmsg, msg, If(iteration.RunState = ISFPIteration.eRunState.Completed, eStatusFlags.OK, eStatusFlags.ErrorEncountered))
                    Next
                    ' Determine the best fitting iteration
                    Me.DetermineBestFit()

                    Me.m_iQueueDone += 1

                    cApplicationStatusNotifier.UpdateProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_RUNNING, My.Resources.DISPLAYNAME),
                                                              (Me.m_iQueueDone + 0.5!) / Me.m_iQueueLength)

                End If
                Me.SendIterationUpdated(iteration)
            End If

            ' Container done?
            If (Not cnt.IsRunning) Then
                ' More to run?
                If (Me.m_queue.Count > 0) Then
                    ' #Yes: order next run
                    cnt.Run(Me.m_queue.Pop)
                Else
                    ' #No: thrash container
                    Me.RemoveContainer(cnt)

                    ' Terminate run if all containers are done
                    If (Me.m_containers.Count = 0) Then
                        Me.TerminateContainerRun()
                    End If
                End If
            End If
        End SyncLock

    End Sub

    ''' <summary>
    ''' Terminate the SFP iterations container.
    ''' </summary>
    Private Sub TerminateContainerRun()

        Debug.Assert(Me.m_containers.Count = 0)

        If (Me.Parameters.AutosaveMode <> cSFPParameters.eAutosaveMode.None) Then
            Me.SaveResultsToCSV(Me.m_statusmsg)
            Me.SaveAllAnomalyResultsToCSV(Me.m_statusmsg)
        End If

        Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.NotSet)
        Me.Core.StateMonitor.SetIsSearching(eSearchModes.NotInSearch)
        Me.Core.SetStopRunDelegate(Nothing)

        Me.m_bIsRunning = False
        Me.SendIterationUpdated(Nothing)

        If (Me.m_statusmsg IsNot Nothing) Then
            If Me.m_statusmsg.Importance = eMessageImportance.Critical Then
                Me.m_statusmsg.Message = cStringUtils.Localize(My.Resources.STATUS_SAVE_FAILED, My.Resources.DISPLAYNAME)
            End If
            Me.Core.Messages.SendMessage(Me.m_statusmsg)
        End If
        cApplicationStatusNotifier.EndProgress(Me.Core)

        ''Load best fitted iteration
        'For Each Iteration As ISFPIterations In m_iterations
        '    If Iteration.IsBestFit Then
        '        Iteration.Apply(Me.Core)
        '        'LoadIterationConfiguration(Iteration)
        '        Exit For
        '    End If
        'Next

        Try
            If Not String.IsNullOrWhiteSpace(Me.m_strTempFileName) Then
                File.Delete(Me.m_strTempFileName)
                Me.m_strTempFileName = ""
            End If
        Catch ex As Exception

        End Try

        Me.m_containers.Clear()
        Me.m_bIsRunning = False
        Me.SendIterationUpdated(Nothing)
    End Sub

    Public ReadOnly Property IsRunning As Boolean
        Get
            Return Me.m_bIsRunning
        End Get
    End Property

    Public Sub StopRun()

        If (Me.IsRunning) Then
            Me.m_bStopRun = True
            ' To account for new container run mode
            If (Me.m_containers.Count > 0) Then
                Dim cts As cSFPContainer() = Me.m_containers.ToArray
                SyncLock Me.m_queue
                    Me.m_queue.Clear()
                    For Each c As cSFPContainer In cts
                        c.StopRun()
                    Next
                End SyncLock
            Else
                Me.Core.EcosimFitToTimeSeries.StopRun()
            End If
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event to notify that an iteraton has been updated during a <see cref="IsRunning">run</see>.
    ''' <see cref="IsRunning"/>
    ''' <see cref="StopRun"/>
    ''' </summary>
    ''' <param name="sender">This class.</param>
    ''' <param name="iteration">The iteration that completed.</param>
    ''' -----------------------------------------------------------------------
    Friend Event OnIterationUpdated(sender As cSFPManager, iteration As ISFPIteration)

    Private Sub SendIterationUpdated(iteration As ISFPIteration)
        Try
            ' Notify the world that the run is over
            RaiseEvent OnIterationUpdated(Me, Nothing)
        Catch ex As Exception
            ' This should not happen
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Sub LoadSFPIterationsList()

        Me.m_iterations.Clear()

        'Only add iterations if time series is loaded
        If Me.TSIndex >= 1 Then

            'Load Fishing iteration
            Me.m_iterations.Add(New cSFPEcosimRun(ISFPIteration.eBaseSearchMode.Fishing))

            'Load Fishing Vunerability Search iterations
            For i = Me.Parameters.MinK To Me.Parameters.K
                Me.m_iterations.Add(New cSFPVulnerabilitySearch(ISFPIteration.eBaseSearchMode.Fishing, i))
            Next

            'If there is a current FF applied to PP
            If (Me.Parameters.AnomalyShapeIndex > 0) Then

                'Load Fishing Anomaly Search iterations
                For i = Me.Parameters.MinSplinePoints To Me.Parameters.MaxSplinePoints Step Me.Parameters.AnomalySearchSplineStepSize
                    Me.m_iterations.Add(New cSFPAnomalySearch(ISFPIteration.eBaseSearchMode.Fishing, i))
                Next

                'Load Fishing V and A Search iterations
                For i = Me.Parameters.MinK To Me.Parameters.K
                    For j = Me.Parameters.MinSplinePoints To Me.Parameters.MaxSplinePoints Step Me.Parameters.AnomalySearchSplineStepSize
                        Dim estParams As Integer = i + j
                        If estParams <= Me.Parameters.K Then
                            Me.m_iterations.Add(New cSFPVandASearch(ISFPIteration.eBaseSearchMode.Fishing, i, j))
                        End If
                    Next
                Next

            End If

            'Load Baseline iteration
            Me.m_iterations.Add(New cSFPEcosimRun(ISFPIteration.eBaseSearchMode.Baseline))

            'Load Baseline Vunerability Search iterations
            For i = Me.Parameters.MinK To Me.Parameters.K
                Me.m_iterations.Add(New cSFPVulnerabilitySearch(ISFPIteration.eBaseSearchMode.Baseline, i))
            Next

            'If there is a current FF applied to PP
            If (Me.Parameters.AnomalyShapeIndex > 0) Then

                'Load Baseline Anomaly Search iterations
                For i = Me.Parameters.MinSplinePoints To Me.Parameters.MaxSplinePoints Step Me.Parameters.AnomalySearchSplineStepSize
                    Me.m_iterations.Add(New cSFPAnomalySearch(ISFPIteration.eBaseSearchMode.Baseline, i))
                Next

                'Load Baseline V and A Search iterations
                For i = Me.Parameters.MinK To Me.Parameters.K
                    For j = Me.Parameters.MinSplinePoints To Me.Parameters.MaxSplinePoints Step Me.Parameters.AnomalySearchSplineStepSize
                        Dim estParams As Integer = i + j
                        If estParams <= Me.Parameters.K Then
                            Me.m_iterations.Add(New cSFPVandASearch(ISFPIteration.eBaseSearchMode.Baseline, i, j))
                        End If
                    Next
                Next
            End If

        End If

    End Sub

    Private Sub DetermineBestFit()

        Dim BestAICc As Single = Single.MaxValue
        Dim BestIteration As ISFPIteration = Nothing

        ' Clear all best fit flags, and determine the best fit
        For Each it As ISFPIteration In Me.Iterations
            it.IsBestFit = False
            If (it.RunState = ISFPIteration.eRunState.Completed) And (it.AICc < BestAICc) Then
                BestIteration = it
                BestAICc = it.AICc
            End If
        Next

        ' Set best fit
        If (BestIteration IsNot Nothing) Then
            BestIteration.IsBestFit = True
        End If

    End Sub

#End Region ' Run Iterations

#Region " Public access "

    Public ReadOnly Property Core As cCore
        Get
            Return Me.Parameters.Core
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get an array with all available <see cref="ISFPIteration"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend ReadOnly Property Iterations As ISFPIteration()
        Get
            Return Me.m_iterations.ToArray()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the one instance of run configuration <see cref="cSFPParameters"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Parameters As cSFPParameters

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the one-based index of the currently loaded <see cref="cTimeSeriesDataset"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property TSIndex As Integer
        Get
            Return Me.Core.ActiveTimeSeriesDatasetIndex
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the output folder for storing Stepwise Fitting results to.
    ''' <seealso cref="cSFPParameters.CustomOutputFolder"/>
    ''' </summary>
    ''' <seealso cref="IsDefaultOutputFolder"/>
    ''' <seealso cref="cCore.DefaultOutputPath(eAutosaveTypes, String)"/>
    ''' <seealso cref="cCore.OutputPath"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property OutputFolder As String
        Get
            If Me.IsDefaultOutputFolder Then
                Return Path.Combine(Me.Core.DefaultOutputPath(eAutosaveTypes.Ecosim), cFileUtils.ToValidFileName(My.Resources.DISPLAYNAME, False))
            End If
            Return Me.Parameters.CustomOutputFolder
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get if results will be saved to the default output folder.
    ''' </summary>
    ''' <seealso cref="OutputFolder"/>
    ''' <seealso cref="cCore.DefaultOutputPath(eAutosaveTypes, String)"/>
    ''' <seealso cref="cCore.OutputPath"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsDefaultOutputFolder As Boolean
        Get
            Return String.IsNullOrWhiteSpace(Me.Parameters.CustomOutputFolder)
        End Get
    End Property

    Public ReadOnly Property HasAbsoluteBiomassTimeSeries As Boolean

#End Region ' Public access

#Region " Save run results "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save iteration results to CSV.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveResultsToCSV(msg As cMessage) As Boolean

        ' Note on globalization: 
        '  - All messages presented to users should be localized, e.g., obtained from the resources;
        '  - All text written to CSV files is written in English, and cannot be localized in case EwE needs to parse this data one day.
        '  - File names are thus also not localized.

        Dim strPath As String = Me.OutputFolder
        Dim CSVfileSimple As String = Path.Combine(strPath, "Stepwise_Fitting_Procedure_Iteration_Results.csv")
        Dim writer As StreamWriter = Nothing
        Dim bSuccess As Boolean = True
        Dim TimeSeries As cTimeSeriesDataset = Me.Core.TimeSeriesDataset(Me.Core.ActiveTimeSeriesDatasetIndex)

        If cFileUtils.IsDirectoryAvailable(strPath, True) Then

            ' ToDo: clear the content of the directory?

            Try
                writer = New StreamWriter(CSVfileSimple)
            Catch ex As Exception
                Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_FAILED, My.Resources.DETAIL_SUMMARY, ex.Message), eStatusFlags.ErrorEncountered)
                bSuccess = False
            End Try

            If (writer IsNot Nothing) Then

                ' Include default header if needed
                If Me.Core.SaveWithFileHeader Then
                    writer.WriteLine(Me.Core.DefaultFileHeader(eAutosaveTypes.Ecosim))
                    writer.WriteLine(cStringUtils.ToCSVField("Number of Observations") & "," & cStringUtils.ToCSVField(Me.Parameters.NumberOfObservations))
                End If

                ' -- Write header --
                writer.WriteLine(",,,,,,,{0}", cStringUtils.ToCSVField("Time Series SS results"))
                writer.Write("Name,K,NVs,NSpline,SS,AIC,AICc")
                For i As Integer = 1 To TimeSeries.nTimeSeries
                    writer.Write("," & cStringUtils.ToCSVField(TimeSeries.TimeSeries(i).Name))
                Next
                writer.WriteLine()

                Try

                    'Go through each iteration_EC
                    For Each Iteration As ISFPIteration In Me.m_iterations
                        If (Iteration.RunState = ISFPIteration.eRunState.Completed) Then

                            ' Write iteration info line
                            writer.Write(cStringUtils.ToCSVField(Iteration.Name) & "," &
                                         cStringUtils.ToCSVField(Iteration.K) & "," &
                                         cStringUtils.ToCSVField(Iteration.EstimatedV) & "," &
                                         cStringUtils.ToCSVField(Iteration.SplinePoints) & "," &
                                         cStringUtils.ToCSVField(Iteration.SS) & "," &
                                         cStringUtils.ToCSVField(Iteration.AIC) & "," &
                                         cStringUtils.ToCSVField(Iteration.AICc))

                            For i As Integer = 1 To TimeSeries.nTimeSeries
                                writer.Write(",")
                                If (Iteration.TimeSeriesSS(i) > 0) Then
                                    writer.Write(cStringUtils.ToCSVField(Iteration.TimeSeriesSS(i)))
                                End If
                            Next
                            writer.WriteLine()
                        End If
                    Next
                    Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_SUCCESS, My.Resources.DETAIL_SUMMARY, CSVfileSimple), eStatusFlags.OK)
                Catch ex As Exception
                    Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_FAILED, My.Resources.DETAIL_SUMMARY, ex.Message), eStatusFlags.ErrorEncountered)
                End Try

                writer.Close()

            End If
        Else
            Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.FAILURE_DIRECTORY, strPath), eStatusFlags.ErrorEncountered)
        End If

        Return bSuccess

    End Function

    'Public Function LoadIterationsConfiguration() As Boolean

    '    Me.m_iterations.Clear()

    '    Dim strSimPath As String = Me.OutputFolder
    '    For Each dir As String In Directory.GetDirectories(strSimPath)

    '        Dim n As String = Path.GetFileName(dir)
    '        Dim iter As ISFPIteration = Nothing

    '        Try
    '            Using reader As New StreamReader(Path.Combine(dir, ".classname"))
    '                Dim strClassName As String = reader.ReadLine().Trim()
    '                reader.Close()
    '                Dim t As Type = Type.GetType(strClassName, False, True)
    '                iter = CType(Activator.CreateInstance(t), ISFPIteration)
    '            End Using
    '        Catch ex As Exception
    '            ' NOP
    '        End Try

    '        If iter.LoadIterationConfiguration(Me.Core) Then
    '            Me.m_iterations.Add(iter)
    '        End If
    '    Next
    '    Return (Me.m_iterations.Count > 0)

    'End Function

    Private Sub AppendStatus(msg As cMessage, strMessage As String, status As eStatusFlags)
        Dim vs As New cVariableStatus(status, strMessage, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
        msg.Variables.Add(vs)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save all iteration Anomaly shape results to CSV.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveAllAnomalyResultsToCSV(msg As cMessage) As Boolean

        ' Note on globalization: 
        '  - All messages presented to users should be localized, e.g., obtained from the resources;
        '  - All text written to CSV files is written in English, and cannot be localized in case EwE needs to parse this data one day.
        '  - File names are thus also not localized.

        Dim strPath As String = Me.OutputFolder
        Dim CSVfileSimple As String = Path.Combine(strPath, "Stepwise_Fitting_Procedure_Anomaly_Results.csv")
        Dim writer As StreamWriter = Nothing
        Dim bSuccess As Boolean = True
        Dim TimeSeries As cTimeSeriesDataset = Me.Core.TimeSeriesDataset(Me.Core.ActiveTimeSeriesDatasetIndex)

        If cFileUtils.IsDirectoryAvailable(strPath, True) Then

            ' ToDo: clear the content of the directory?

            Try
                writer = New StreamWriter(CSVfileSimple)
            Catch ex As Exception
                Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_FAILED, My.Resources.DETAIL_SUMMARY, ex.Message), eStatusFlags.ErrorEncountered)
                bSuccess = False
            End Try

            If (writer IsNot Nothing) Then

                ' Include default header if needed
                If Me.Core.SaveWithFileHeader Then
                    writer.WriteLine(Me.Core.DefaultFileHeader(eAutosaveTypes.Ecosim))
                    writer.WriteLine(cStringUtils.ToCSVField("Number of Observations") & "," & cStringUtils.ToCSVField(Me.Parameters.NumberOfObservations))
                End If

                ' -- Write header --

                writer.WriteLine("Anomaly Results")
                writer.WriteLine()
                writer.Write("Iteration Name")
                writer.WriteLine()



                Try

                    'Go through each iteration_EC
                    For Each Iteration As ISFPIteration In Me.m_iterations
                        If (Iteration.RunState = ISFPIteration.eRunState.Completed) Then

                            writer.Write(cStringUtils.ToCSVField(Iteration.Name) & ",")

                            ' Write iteration info line
                            For i As Integer = 0 To Iteration.AnomalyShape.Length - 1
                                If (i >= 1) Then writer.Write(",")
                                writer.Write(cStringUtils.ToCSVField(Iteration.AnomalyShape(i)))
                            Next

                            writer.WriteLine()
                        End If
                    Next
                    Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_SUCCESS, My.Resources.DETAIL_SUMMARY, CSVfileSimple), eStatusFlags.OK)
                Catch ex As Exception
                    Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_FAILED, My.Resources.DETAIL_SUMMARY, ex.Message), eStatusFlags.ErrorEncountered)
                End Try

                writer.Close()

            End If
        Else
            Me.AppendStatus(msg, cStringUtils.Localize(My.Resources.FAILURE_DIRECTORY, strPath), eStatusFlags.ErrorEncountered)
        End If

        Return bSuccess

    End Function

#End Region ' Save run results

End Class
