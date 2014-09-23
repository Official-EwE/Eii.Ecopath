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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports LumenWorks.Framework.IO.Csv
Imports ScientificInterfaceShared.Controls
Imports Troschuetz.Random

#End Region ' Imports

Public Class cMSE

#Region " Internal vars "

    Private m_core As cCore = Nothing
    Private m_strategies As Strategies = Nothing
    Private m_survivability As cSurvivability = Nothing
    Private m_regulations As cRegulations = Nothing
    Private m_currentStrategy As Strategy = Nothing
    Private m_monitor As cMSEStateMonitor = Nothing
    Private m_effortlimits As cEffortLimits = Nothing
    Private m_diets As cDiets = Nothing

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'For the Stock Assessment Model
    Private m_StockAssessment As cStockAssessmentModel

    'For now use the MSE data from the Core get CV's for distributions
    'Once we have an interface we can replace this
    Private m_CoreMSEData As MSE.cMSEDataStructures
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Private m_ecosim As EwECore.Ecosim.cEcoSimModel
    Private _simdata As cEcosimDatastructures
    Private _pathdata As cEcopathDataStructures
    Private m_ecopath As Ecopath.cEcoPathModel
    Private _EcosimTimeStepDelegate As EwECore.Ecosim.EcoSimTimeStepDelegate
    Private StrategyIndex As Integer
    Private OriginalNTimesteps As Integer
    Private MinEffortThisYear() As Single

    Private TargConsQuota(,) As Double 'Stores the target and conservation f's for each species
    Private nSuccessfullyProjectedModels As Integer

    Private TechnologyCreep() As Single 'an array where each element represents the percentage with which each fleet increases its catching efficiency each year
    Private m_plugin As cMSEPluginPoint = Nothing

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'All basic Ecopath and Ecosim parameters X(a,b) a = iteration b = the functional group
    Private B(,) As Double
    Private PB(,) As Double
    Private QB(,) As Double
    Private EE(,) As Double
    Private BA(,) As Double
    Private DenDepCatchability(,) As Double
    Private FeedingTimeAdjustRate(,) As Double
    Private MaxRelFeedingTime(,) As Double
    Private OtherMortFeedingTime(,) As Double
    Private PredEffectFeedingTime(,) As Double
    Private QBMaxxQBio(,) As Double
    Private SwitchingPower(,) As Double
    Private Vulnerabilities(,,) As Double
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Private FleetsThatFishHCRGrp As List(Of Integer) = New List(Of Integer)
    Public Property m_quotashares As cQuotaShares
    'Private m_Survivability As cSurvivability

    Private BTemp() As Double
    Private PBTemp() As Double
    Private QBTemp() As Double
    Private EETemp() As Double
    Private BATemp() As Double
    Private DenDepCatchabilityTemp() As Double
    Private FeedingTimeAdjustRateTemp() As Double
    Private MaxRelFeedingTimeTemp() As Double
    Private OtherMortFeedingTimeTemp() As Double
    Private PredEffectFeedingTimeTemp() As Double
    Private QBMaxxQBioTemp() As Double
    Private SwitchingPowerTemp() As Double
    Private VulnerabilitiesTemp(,) As Double
    Private DietMatrixTemp(,) As Double
    Private DietImpTemp() As Double
    Private ChangeEffortFlag As Boolean = False
    Private m_rand As New Random()

    Private m_StopRun As Boolean
    Private m_runstate As eRunStates = eRunStates.Idle

#Region "Threading variables"
    'Private m_WaitObject As System.Threading.ManualResetEvent

    Private m_SyncObj As System.Threading.SynchronizationContext

#End Region

    Public Enum DistributionType As Integer
        NotSet = 0
        Uniform
        Triangular
    End Enum

    Public Enum RegulateFleet As Integer
        Discards = 0
        NoDiscards_NonSelective
        NoDiscards_Selective
    End Enum

    Private m_mhSettings As cMessageHandler = Nothing
    Private m_mhEcosim As cMessageHandler = Nothing
    Private m_iNumModelsAvailable As Integer = cCore.NULL_VALUE
    Private m_iNumStrategiesAvailable As Integer = cCore.NULL_VALUE
    Private m_tsInputDataCompatibility As TriState = TriState.UseDefault
    Private m_tsRunDataCompatibility As TriState = TriState.UseDefault
    Private TrajectoryCsv As StreamWriter
    Private Trajectory2Csv As List(Of StreamWriter)             'Trajectories2 is similar to trajectories apart from it each file contains only 1 group
    Private swFleetEffort As StreamWriter

#End Region ' Internal vars

#Region " Public Properties "

    Public Enum eRunStates As Byte
        Idle = 0
        RunningModels
        RunningMSE
    End Enum

    Public Property RunState As eRunStates
        Get
            Return Me.m_runstate
        End Get
        Set(value As eRunStates)
            If (value <> Me.m_runstate) Then
                Me.m_runstate = value
                Me.fireChangeRunState()
            End If
        End Set
    End Property

    Public ReadOnly Property Core As cCore
        Get
            Return Me.m_core
        End Get
    End Property

    Public ReadOnly Property Survivability As cSurvivability
        Get
            Return Me.m_survivability
        End Get
    End Property

    Public ReadOnly Property QuotaShares As cQuotaShares
        Get
            Return Me.m_quotashares
        End Get
    End Property

    Public ReadOnly Property Strategies As Strategies
        Get
            Return m_strategies
        End Get
    End Property

    Public ReadOnly Property EffortLimits As cEffortLimits
        Get
            Return Me.m_effortlimits
        End Get
    End Property

    Public ReadOnly Property Diets As cDiets
        Get
            Return Me.m_diets
        End Get
    End Property

    Public ReadOnly Property StockAssessment As cStockAssessmentModel
        Get
            Return Me.m_StockAssessment
        End Get
    End Property

    Public ReadOnly Property EcosimData As cEcosimDatastructures
        Get
            Return Me._simdata
        End Get
    End Property

    Public ReadOnly Property EcopathData As cEcopathDataStructures
        Get
            Return Me._pathdata
        End Get
    End Property

    Public Property CoreMSEData As MSE.cMSEDataStructures
        Get
            Return Me.m_CoreMSEData
        End Get
        Set(value As MSE.cMSEDataStructures)
            Me.m_CoreMSEData = value
        End Set
    End Property

    Public Sub StopRun()
        If (Me.RunState <> eRunStates.Idle) Then
            Me.m_StopRun = True
        End If
    End Sub

#End Region

#Region " Construction "

    Public Sub New(ByVal Monitor As cMSEStateMonitor, pluginPoint As cMSEPluginPoint)
        Me.m_monitor = Monitor
        Me.m_plugin = pluginPoint

        Me.m_SyncObj = System.Threading.SynchronizationContext.Current
        'if there is no current context then create a new one on this thread. 
        'this happens if no interface has been created yet(I think...)
        If (Me.m_SyncObj Is Nothing) Then Me.m_SyncObj = New System.Threading.SynchronizationContext()

        'Create the wait object with the initial state signaled, the object can be used
        'Me.m_WaitObject = New System.Threading.ManualResetEvent(True)

        Me.InvalidateConfigurationState()
    End Sub

    Public Sub onCoreInitialized(EwECore As cCore, Ecopath As Ecopath.cEcoPathModel, Ecosim As Ecosim.cEcoSimModel)

        Me.m_core = EwECore
        Me.m_ecopath = Ecopath
        Me.m_ecosim = Ecosim

        Me.InvalidateConfigurationState()

    End Sub

#End Region ' Construction

#Region "Pubic methods"


    ''' <summary>
    ''' Interactively resolve MSE folder conflicts.
    ''' </summary>
    Public Function ResolveMSEPathConflicts(ByVal bInteractive As Boolean) As Boolean

        ' Forget all we know
        Me.InvalidateConfigurationState(False)

        ' Check if input structure is missing
        If Not Me.IsInputStructureAvailable() Then
            ' Ask user to create folder structure
            If (bInteractive) And Me.AskUser(String.Format(My.Resources.PROMPT_DATAPATH_MISSING, Me.DataPath), eMessageReplyStyle.YES_NO) <> eMessageReply.OK Then
                ' #User abort: abandon process
                Return False
            End If
        Else
            ' Check if input structure is compatible
            ' JS: this check is for ow ruled out by disabling the 'Review dist params' button in the UI
            If Not Me.IsInputDataCompatible() Then
                If (bInteractive) And Me.AskUser("The selected folder is not compatible with the currently loaded model. If you continue, previously saved MSE settings will be lost. Do you wish to continue, delete existing MSE settings, and start anew?", _
                               eMessageReplyStyle.YES_NO, eMessageImportance.Warning, eMessageReply.NO) = eMessageReply.NO Then
                    Return False
                End If
            Else
                Return True
            End If
        End If


        ' Make sure plug-in has all dirs
        Dim strPath As String = Me.DataPath
        For Each f As cMSEUtils.eMSEPaths In [Enum].GetValues(GetType(cMSEUtils.eMSEPaths))
            cFileUtils.IsDirectoryAvailable(cMSEUtils.MSEFolder(strPath, f), True)
            'bSuccess = bSuccess And cFileUtils.IsDirectoryAvailable(cMSEUtils.MSEFolder(strPath, f), True)
        Next

        ' --- BEGIN GENERATING ALL ESSENTIAL INPUT FILES FOR A NEW MSE FOLDER ---

        Me.GenerateEmptyDistributions()
        Me.GenerateDefaultSurviveDistributions()
        Me.GenerateDefaultDiets()
        Me.GenerateEmptyBiomassLimitsCSV()
        Me.GenerateEmptyEffortLimitsCSV()
        Me.GenerateEmptyQuotaSharesCSV()

        ' .. add more

        ' --- END GENERATING ALL ESSENTIAL INPUT FILES FOR A NEW MSE FOLDER ---


        ' Re-assess state
        Me.InvalidateConfigurationState(True)

#If DEBUG Then
        ' Panic in debug mode only
        'Stop
        Debug.Assert(Me.IsInputDataCompatible(), "Cefas MSE default data generation logic is not working")
#End If

        Return Me.IsInputDataCompatible()

    End Function

    Public Sub LoadSampledParams()

        Me.RunState = eRunStates.RunningModels
        Me.ChangeEffortFlag = True
        cApplicationStatusNotifier.StartProgress(Me.Core, "", -1)

        Try
            Me.Run()
        Catch ex As Exception
            ' Whoah!
            'This shouldn't happen, really....
            cLog.Write(ex, "CefasMSE:LoadSampledParameters")
        End Try

        cApplicationStatusNotifier.EndProgress(Me.Core)
        Me.ChangeEffortFlag = False
        Me.RunState = eRunStates.Idle

    End Sub


    Public Function CreateModels() As Boolean
        Dim bsuccess As Boolean = True

        If (Me.RunState <> eRunStates.Idle) Then
            Me.InformUser("Sorry already running.", eMessageImportance.Warning)
            Return False
        End If

        Me.m_StopRun = False
        Dim RunThread As Threading.Thread

        RunThread = New Threading.Thread(AddressOf Me.runCreateModelsThread)
        RunThread.Name = "CEFAS_MSE"

        RunThread.Start()

        Return bsuccess

    End Function

    Public Sub GenerateDefaultSurviveDistributions()

        Dim TSurvivability As cSurvivability = New cSurvivability(Me, m_core, _simdata, _pathdata)
        TSurvivability.Save()

    End Sub

    Public Sub GenerateSurvivabilities()

        Dim TSurvivability As cSurvivability = New cSurvivability(Me, m_core, _simdata, _pathdata)

        TSurvivability.Load()
        TSurvivability.SampleParams(Me.NModels)
        TSurvivability.SaveSampledToCSV()
        TSurvivability.Save()

        Me.Survivability.Load()
        'Me.InvalidateConfigurationState()

    End Sub

    Public Sub GenerateEmptyBiomassLimitsCSV()
        Dim TBiomassLimits As cBiomassLimits = New cBiomassLimits(m_plugin)
        TBiomassLimits.SaveLimitsToCSV()
    End Sub

    Public Sub GenerateEmptyEffortLimitsCSV()
        Dim TEffortLimits As cEffortLimits = New cEffortLimits(Me, m_core)
        TEffortLimits.Save()
    End Sub

    Public Sub GenerateEmptyQuotaSharesCSV()
        Dim TQuotaShares As cQuotaShares = New cQuotaShares(Me, Core)
        TQuotaShares.Save()
    End Sub

    Public Function GenerateDefaultDiets() As Boolean
        ' JS 20Jul14: Diet writing moved to cDiets class
        Dim d As New cDiets(Me, Me.Core)
        Return d.Save()
    End Function

    Public Function GenerateEmptyDistributions() As Boolean

        Dim distpath As New cEcopathDistributionParams(Me, Me.Core)
        Dim distsim As New cEcosimDistributionParams(Me, Me.Core)
        Return distpath.Save() And distsim.Save()

    End Function

#End Region

#Region "Threading"

    Private Sub runCreateModelsThread()

        Me.m_StopRun = False
        Me.m_core.StateMonitor.SetIsSearching(eSearchModes.External)
        Me.m_core.SetStopRunDelegate(New cCore.StopRunDelegate(AddressOf Me.StopRun))
        Me.RunState = eRunStates.RunningModels

        cApplicationStatusNotifier.StartProgress(Me.Core, "", -1)

        Try
            Me.GenerateEcosimParameters("MaxRelFeedingTime")
            Me.GenerateEcosimParameters("FeedingTimeAdjustRate")
            Me.GenerateEcosimParameters("OtherMortFeedingTime")
            Me.GenerateEcosimParameters("PredEffectFeedingTime")
            Me.GenerateEcosimParameters("DenDepCatchability")
            Me.GenerateEcosimParameters("QBMaxxQBio")
            Me.GenerateEcosimParameters("SwitchingPower")
            Me.GenerateSurvivabilities()
            Me.CreateVulnerabilities()
            Me.GenerateEcopathParamaters()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        cApplicationStatusNotifier.EndProgress(Me.Core)

        Me.m_core.StateMonitor.SetIsSearching(eSearchModes.NotInSearch)
        Me.m_core.SetStopRunDelegate(Nothing)
        Me.RunState = eRunStates.Idle

    End Sub

    Private Sub fireChangeRunState()
        'Make sure this gets called even if there is an exception
        m_SyncObj.Send(New System.Threading.SendOrPostCallback(AddressOf Me.SendEvent), Nothing)
    End Sub

    Private Sub SendEvent(obj As Object)
        Try
            ' Have the State monitor tell the world
            Me.m_monitor.Invalidate()
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".fireChangeRunState() Exception: " & ex.Message)
        End Try
    End Sub

#End Region 'Threading

#Region " Diagnostics and state management "

    ''' <summary>
    ''' Invalidate the known state of the MSE engine. Data may have been changed, directories
    ''' may have been swapped: MSE simply does not know its state any longer and will need to
    ''' reassess this at a next opportunity.
    ''' </summary>
    ''' <param name="bReloadData"></param>
    Friend Sub InvalidateConfigurationState(Optional bReloadData As Boolean = True)

        Me.m_iNumModelsAvailable = cCore.NULL_VALUE
        Me.m_iNumStrategiesAvailable = cCore.NULL_VALUE
        Me.m_tsInputDataCompatibility = TriState.UseDefault
        Me.m_tsRunDataCompatibility = TriState.UseDefault
        Me.m_monitor.Invalidate()

        ' Test whether core is up and running
        If (Me.m_core Is Nothing) Then Return
        ' Test whether a scenario is available
        If (Me.m_core.ActiveEcosimScenarioIndex < 1) Then Return
        ' Test whether MSE has been initialized
        If (Me.m_survivability Is Nothing) Then Return

        If (Not Me.IsInputStructureAvailable()) Then Return
        If (Not Me.IsInputDataCompatible()) Then Return
        If (Not bReloadData) Then Return

        cApplicationStatusNotifier.StartProgress(Me.m_core, My.Resources.STATUS_LOADING, -1)
        Try
            ' Reload possible data
            Me.EffortLimits.Load()
            Me.QuotaShares.Load()
            Me.Strategies.Load()
            Me.Diets.Load()
            Me.Survivability.LoadSampledParamsFromCSV()
            Me.Survivability.Load()
            Me.StockAssessment.Load()
        Catch ex As Exception

        End Try
        cApplicationStatusNotifier.EndProgress(Me.m_core)

    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the MSE plug-in has all directories that it needs in the
    ''' <see cref="DataPath"></see>.
    ''' </summary>
    ''' <returns>True if the directory structure exists in its entirety.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsInputStructureAvailable() As Boolean

        Return Me.Survivability.FileExists()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether all base data is available for building models
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function IsInputDataCompatible() As Boolean

        ' JS 21Jul14: data compatibility is assessed by individual data classes now

        If (Me.m_tsInputDataCompatibility = TriState.UseDefault) Then

            ' Hope for the best
            Dim msgError As New cMessage("", eMessageType.Any, eCoreComponentType.External, eMessageImportance.Maintenance, eDataTypes.External)
            Me.m_tsInputDataCompatibility = TriState.True

            ' Make sure plug-in has empty CSV
            If Not Me.Diets.Load(msgError) Or Not Me.Survivability.Load(msgError) Then
                Me.m_tsInputDataCompatibility = TriState.False
            End If

            If (Me.m_tsInputDataCompatibility <> TriState.False) Then
                ' Assess Ecopath distributions
                Dim dist As New cEcopathDistributionParams(Me, Me.Core)
                If Not dist.Load(msgError) Then
                    Me.m_tsInputDataCompatibility = TriState.False
                End If
            End If

            If (Me.m_tsInputDataCompatibility <> TriState.False) Then
                ' Assess Ecosim distributions
                Dim dist As New cEcosimDistributionParams(Me, Me.Core)
                If Not dist.Load(msgError) Then
                    Me.m_tsInputDataCompatibility = TriState.False
                End If
            End If

            ' Input structure not compatible?
            If (Me.m_tsInputDataCompatibility = TriState.False) Then
                ' Prepare and send message
                msgError.Message = String.Format(My.Resources.PROMPT_DATAPATH_INCOMPATIBLE, Me.DataPath)
                msgError.Importance = eMessageImportance.Critical
                Me.m_core.Messages.SendMessage(msgError)
            End If

        End If

        Return (Me.m_tsInputDataCompatibility = TriState.True)

    End Function


    Public Function IsRunDataCompatible() As Boolean

        ' Would it not be nice if these file names were represented by enums as well?

        'Dim aFilesFleet As String() = New String() {"ChangesInEffortLimits", "QuotaShares"}
        'Dim strRoot As String = cMSEUtils.MSEFolder(Me.DataPath, cMSEUtils.eMSEPaths.Fleet)
        Dim outParamFiles As String() = New String() {"b_out", "ba_out", "ee_out", "pb_out", "qb_out", "DenDepCatchability_out", _
                                                      "FeedingTimeAdjustRate_out", "MaxRelFeedingTime_out", "OtherMortFeedingTime_out", _
                                                      "PredEffectFeedingTime_out", "QBMaxxQBio_out", "SwitchingPower_out"}

        If (Me.m_tsRunDataCompatibility = TriState.UseDefault) Then

            ' Hope for the best
            Me.m_tsRunDataCompatibility = TriState.True

            ' Make sure folder has out CSV files
            If Not File.Exists(cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv")) Or _
                Not File.Exists(cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.Fleet, "QuotaShares.csv")) Or _
                Not File.Exists(cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.ParamsOut, "Survivabilities_out.csv")) Then
                Me.m_tsRunDataCompatibility = TriState.False
            End If

            ' Instead, test whether the data classes are populated with data:
            ' - Has fishing strategies?
            If (Me.NumStrategiesAvailable = 0) Then Me.m_tsRunDataCompatibility = TriState.False
            ' - Has quota shares? etc

            ' Assess Ecopath files
            For Each strFile As String In outParamFiles
                If Not File.Exists(cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.ParamsOut, strFile & ".csv")) Then
                    Me.m_tsRunDataCompatibility = TriState.False
                    Exit For
                End If
            Next strFile

        End If

        Return (Me.m_tsRunDataCompatibility = TriState.True)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of pre-generated models found in the current <see cref="DataPath"/>.
    ''' </summary>
    ''' <returns>
    ''' The number of pre-generated models found in the current <see cref="DataPath"/>.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function NumModelsAvailable() As Integer

        If (Me.m_iNumModelsAvailable = cCore.NULL_VALUE) Then

            SyncLock Me
                Me.m_iNumModelsAvailable = 0
                ' JS 07Oct13: Very simple test
                Dim strPath As String = cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.ParamsOut, "b_out.csv")
                If File.Exists(strPath) Then
                    Dim reader As StreamReader = cMSEUtils.GetReader(strPath)
                    If (reader IsNot Nothing) Then
                        reader.ReadLine()
                        While Not reader.EndOfStream
                            reader.ReadLine()
                            Me.m_iNumModelsAvailable += 1
                        End While
                    End If
                End If
            End SyncLock

        End If
        Return Me.m_iNumModelsAvailable

    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of fishing strategies found in the current <see cref="DataPath"/>.
    ''' </summary>
    ''' <returns>
    ''' The number of fishing strategies found in the current <see cref="DataPath"/>.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function NumStrategiesAvailable() As Integer

        If (Me.m_iNumStrategiesAvailable = cCore.NULL_VALUE) Then

            SyncLock Me
                Me.Strategies.Load()
                Me.m_iNumStrategiesAvailable = Me.Strategies.Count
            End SyncLock

        End If
        Return Me.m_iNumStrategiesAvailable

    End Function

    Private m_strModelCompatibility As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get any recent compatibility assessment.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ModelCompatibilityInfo As String
        Get
            Return Me.m_strModelCompatibility
        End Get
    End Property

#End Region ' Diagnostics and state management

#Region "File I/O and other file related 'stuff'"

    Public ReadOnly Property DataPath As String
        Get
            If Me.UseEwEPath Then
                Return Path.Combine(Me.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim), "CefasMSE")
            End If
            Return Me.CustomPath
        End Get
    End Property

    Private Function ExtractParamsCSV(ByRef param_name As String) As Double(,)

        ' JS 09Oct13: Used standard readers/writers, and made robust

        Dim Params(,) As Double = Nothing
        Dim iRecord As Integer = 0
        Dim csv As CsvReader = Nothing
        Dim nIterations As Integer = Me.NModels2Run
        Dim reader As StreamReader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, param_name & "_out.csv"))

        If (reader Is Nothing) Then Return Params

        csv = New CsvReader(reader, True)
        ReDim Params(nIterations - 1, csv.FieldCount - 1)
        Try
            While Not csv.EndOfStream And iRecord < nIterations
                If csv.ReadNextRecord() Then
                    For iField = 1 To csv.FieldCount()
                        Params(iRecord, iField - 1) = cStringUtils.ConvertToDouble(csv(iField - 1))
                    Next
                End If
                iRecord += 1
            End While
        Catch ex As Exception
            ' ToDo: decide what to do when CSV data is malformed
        End Try

        csv.Dispose()
        cMSEUtils.ReleaseReader(reader)

        Return Params

    End Function

    Private Function ExtractVulnerabilitiesCSV() As Double(,,)

        ' JS 09Oct13: Used standard readers/writers, and made robust

        Dim nIterations As Integer = Me.NModels2Run
        Dim csv As CsvReader
        Dim vulnerabilities(nIterations - 1, m_ecopath.EcopathData.NumGroups - 1, m_ecopath.EcopathData.NumGroups - 1) As Double

        For iIteration As Integer = 1 To nIterations
            Dim reader As StreamReader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "VulnerabilityIteration" & iIteration.ToString & "_out.csv"))
            If (reader IsNot Nothing) Then
                csv = New CsvReader(reader, False)
                Try
                    While Not csv.EndOfStream
                        If csv.ReadNextRecord() Then
                            For iPrey As Integer = 1 To m_ecopath.EcopathData.NumGroups
                                vulnerabilities(iIteration - 1, CInt(csv.CurrentRecordIndex), iPrey - 1) = cStringUtils.ConvertToDouble(csv(iPrey - 1))
                            Next
                        End If
                    End While
                Catch ex As Exception
                    ' ToDo: decide what to do when CSV data is malformed
                End Try
                csv.Dispose()
                cMSEUtils.ReleaseReader(reader)
            End If
        Next

        Return vulnerabilities

    End Function

    ''' <summary>
    ''' Append an Ecopath variable to a CSV out file.
    ''' </summary>
    ''' <param name="strFile"></param>
    ''' <param name="data"></param>
    ''' <returns>True if successful.</returns>
    Private Function WriteEcopathParms(strFile As String, data As Single()) As Boolean

        Dim strPath As String = cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.ParamsOut, strFile)
        Dim writer As StreamWriter = Nothing

        If Not File.Exists(strPath) Then
            writer = cMSEUtils.GetWriter(strPath)
            If (writer Is Nothing) Then Return False

            For igrp As Integer = 1 To Me.Core.nLivingGroups
                If (igrp > 1) Then writer.Write(",")
                writer.Write(cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(igrp).Name))
            Next
        Else
            writer = cMSEUtils.GetWriter(strPath, True)
            If (writer Is Nothing) Then Return False
        End If

        writer.WriteLine()
        For igrp As Integer = 1 To Me.Core.nLivingGroups
            If (igrp > 1) Then writer.Write(",")
            writer.Write(data(igrp))
        Next
        cMSEUtils.ReleaseWriter(writer)
        Return True

    End Function

    Private Function InitMonteCarloParamX(ByVal strPath As String, ByVal ParamName As eParamName) As Boolean
        Dim csvParamX As CsvReader
        Dim MonteCarlo As cMonteCarloManager = m_core.EcosimMonteCarlo
        Dim MCGroup As cMonteCarloGroup
        Dim xgrp As Integer

        ' ToDo: merge with self-load capabilities of cDistributionParameters

        Try

            'If Not CheckEcopathDistributionFilesOkay(strPath) Then Return False

            csvParamX = New CsvReader(New StreamReader(strPath), True) ' I think this is to restart the reading of the csv

            For igrp = 1 To m_core.nLivingGroups

                xgrp = 1
                If (Not csvParamX.EndOfStream) And (csvParamX.ReadNextRecord()) Then
                    'Make sure that .csv files are set up with group names in same order because xgrp is found only from the B file
                    'and then is assumed to be the same for all other files
                    While MonteCarlo.Groups(xgrp).Name <> csvParamX(1) 'And xgrp <= mCore.nLivingGroups
                        xgrp += 1
                    End While

                    MCGroup = MonteCarlo.Groups(xgrp)

                    'Setting a CV value will automatically set the Lower and Upper limits
                    'by Calling cEcosimMonteCarlo.CalculateUpperLowerLimits()
                    'If you want to manually set limits it must be done after the CV has been set

                    'CVs
                    If ParamName = eParamName.B Then
                        MCGroup.Bcv = cStringUtils.ConvertToSingle(csvParamX(2))
                        MCGroup.BLower = cStringUtils.ConvertToSingle(csvParamX(3))
                        MCGroup.BUpper = cStringUtils.ConvertToSingle(csvParamX(4))
                    End If

                    If ParamName = eParamName.PB Then
                        MCGroup.PBcv = cStringUtils.ConvertToSingle(csvParamX(2))
                        MCGroup.PBLower = cStringUtils.ConvertToSingle(csvParamX(3))
                        MCGroup.PBUpper = cStringUtils.ConvertToSingle(csvParamX(4))
                    End If

                    If ParamName = eParamName.QB Then
                        MCGroup.QBcv = cStringUtils.ConvertToSingle(csvParamX(2))
                        MCGroup.QBLower = cStringUtils.ConvertToSingle(csvParamX(3))
                        MCGroup.QBUpper = cStringUtils.ConvertToSingle(csvParamX(4))
                    End If

                    If ParamName = eParamName.EE Then
                        MCGroup.EEcv = cStringUtils.ConvertToSingle(csvParamX(2))
                        MCGroup.EELower = cStringUtils.ConvertToSingle(csvParamX(3))
                        MCGroup.EEUpper = cStringUtils.ConvertToSingle(csvParamX(4))
                    End If

                    If ParamName = eParamName.BA Then
                        MCGroup.BAcv = cStringUtils.ConvertToSingle(csvParamX(2))
                        MCGroup.BALower = cStringUtils.ConvertToSingle(csvParamX(3))
                        MCGroup.BAUpper = cStringUtils.ConvertToSingle(csvParamX(4))
                    End If
                Else
                    ' ToDo_JS: Error reading CSV content. How to respond?
                End If

            Next '========================================================================================================================================================

            'reset the connection to the csv files ready to be read from the beginning again
            csvParamX.Dispose()

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitMonteCarloParameters() Exception: " & ex.Message)
        End Try

        Return False

    End Function

    Enum eParamName As Integer
        B
        PB
        QB
        EE
        BA
    End Enum

    Private Function InitMonteCarloParameters() As Boolean

        'loads the distribution parameters for the Ecopath parameters from csvs
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "B_Dist.csv"), eParamName.B) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "PB_Dist.csv"), eParamName.PB) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "QB_Dist.csv"), eParamName.QB) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "EE_Dist.csv"), eParamName.EE) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "BA_Dist.csv"), eParamName.BA) Then Return False

        Return True

    End Function

    Private Function GenerateEcosimParameters(ByVal ParamName As String) As Boolean

        Dim strPath As String = cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, ParamName & ".csv")

        'If Not CheckEcoSimDistributionFilesOkay(strPath) Then Return False

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim ParameterArray(m_core.nLivingGroups - 1, 3) As Single

        ' JS 30Sep13: Use local properties
        Dim nModels As Integer = Me.NModels
        Dim eDistributionType As DistributionType
        Dim SampledParameters(nModels - 1, m_core.nLivingGroups - 1) As Double
        Dim GroupNames(m_core.nLivingGroups - 1) As String

        reader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, ParamName & ".csv"))
        If (reader IsNot Nothing) Then
            csv = New CsvReader(reader, True)
            'Read all the distribution information from the .csv file and into an array ParameterArray
            Try

                While Not csv.EndOfStream
                    If csv.ReadNextRecord() Then
                        GroupNames(CInt(csv.CurrentRecordIndex)) = cMSEUtils.FromCSVField(csv("GroupName"))
                        For iField = 2 To 5
                            ParameterArray(CInt(csv.CurrentRecordIndex), iField - 2) = cStringUtils.ConvertToSingle(csv(iField))
                        Next
                    End If
                End While
            Catch ex As Exception

            End Try
            csv.Dispose()
        End If

        cMSEUtils.ReleaseReader(reader)

        'Generate an array of sample parameters
        For iGroup = 1 To m_core.nLivingGroups

            If Not ParameterArray(iGroup - 1, 1) = cCore.NULL_VALUE Then

                eDistributionType = CType(ParameterArray(iGroup - 1, 0), DistributionType)

                For iModel = 1 To nModels

                    Select Case eDistributionType
                        Case DistributionType.Uniform
                            SampledParameters(iModel - 1, iGroup - 1) = UniformSample(ParameterArray(iGroup - 1, 1), ParameterArray(iGroup - 1, 2))
                        Case DistributionType.Triangular
                            SampledParameters(iModel - 1, iGroup - 1) = TriangularSample(ParameterArray(iGroup - 1, 1), ParameterArray(iGroup - 1, 2), ParameterArray(iGroup - 1, 3))
                    End Select

                Next
            Else
                For iModel = 1 To nModels
                    SampledParameters(iModel - 1, iGroup - 1) = cCore.NULL_VALUE
                Next
            End If

        Next

        'Output the sampled parameters to a csv
        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, ParamName & "_out.csv"))
        If (writer IsNot Nothing) Then
            Try
                For igrp As Integer = 1 To m_core.nLivingGroups
                    If (igrp > 1) Then writer.Write(",")
                    writer.Write(cStringUtils.ToCSVField(GroupNames(igrp - 1)))
                Next
                writer.WriteLine()

                For iIteration = 1 To nModels
                    For iGroup = 1 To m_core.nLivingGroups
                        If (iGroup > 1) Then writer.Write(",")
                        writer.Write(cStringUtils.ToCSVField(SampledParameters(iIteration - 1, iGroup - 1)))
                    Next
                    writer.WriteLine()
                Next
            Catch ex As Exception
                ' ToDo: respond to error, somehow
            End Try
        End If
        cMSEUtils.ReleaseWriter(writer)
        Return True

    End Function

    ''' <summary>
    ''' Generate csv with vulnerabilities.
    ''' </summary>
    Private Sub CreateVulnerabilities()

        ' JS 13Oct13: Fixed path usage
        ' JS 13Oct13: Used standard CSV field reading/writing
        ' JS 30Sep13: Used persistent properties

        Dim writer As StreamWriter = Nothing
        Dim nIterations As Integer = NModels

        For iIteration = 1 To nIterations

            writer = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "VulnerabilityIteration" & iIteration & "_out.csv"), False)
            If (writer IsNot Nothing) Then
                'Create random values for the vulnerabilities and store in a csv
                For igrppredator As Integer = 1 To m_ecopath.EcopathData().NumLiving
                    If m_core.EcoPathGroupInputs(igrppredator).IsProducer Then
                        For igrpprey As Integer = 1 To m_ecopath.EcopathData().NumGroups
                            If (igrpprey > 1) Then writer.Write(",")
                            writer.Write(Convert.ToSingle(cCore.NULL_VALUE))
                        Next igrpprey
                    Else
                        For igrpprey As Integer = 1 To m_ecopath.EcopathData().NumGroups
                            If (igrpprey > 1) Then writer.Write(",")
                            writer.Write(Convert.ToSingle(1 + Math.Exp(9 * (CSng(Me.m_rand.NextDouble()) - 0.5))))
                        Next igrpprey
                    End If
                    writer.WriteLine()
                Next igrppredator
            Else
                ' Hmm, writer could not be created?!
            End If
            cMSEUtils.ReleaseWriter(writer)
        Next

    End Sub


    'Private Function LoadCostFunctionsCSV()
    '    Dim CostFunctionReader As CsvReader
    '    Dim CostFunctionArray(,) As String
    '    Dim CostFunctionArrayIndex As Integer = 0
    '    Dim StratFileNames() As String
    '    Dim FileName As String
    '    Dim FoundElement As Boolean

    '    'Get the names of the files in the strategies folder
    '    StratFileNames = Directory.GetFiles(DataPath & "\Strategies")

    '    'Redim the cost function array so that there are enough rows for each strategy
    '    ReDim CostFunctionArray(StratFileNames.Count - 2, 1)

    '    'Setup csvreader object
    '    CostFunctionReader = New CsvReader(New StreamReader(DataPath & "\Strategies\CostFunctionType.csv"), True)

    '    'Read the data in CostFunctionType.csv into the CostFunctionArray
    '    While Not CostFunctionReader.EndOfStream
    '        CostFunctionReader.ReadNextRecord()
    '        CostFunctionArray(CostFunctionArrayIndex, 0) = CostFunctionReader(0)
    '        CostFunctionArray(CostFunctionArrayIndex, 1) = CostFunctionReader(1)
    '        CostFunctionArrayIndex += 1
    '    End While

    '    'Check that all files in the strategies folder are represented in the CostFunctionArray
    '    For Each iFile In StratFileNames
    '        FileName = Path.GetFileName(iFile)
    '        If FileName = "CostFunctionType.csv" Then Continue For 'skip over the file that holds the costfunctions

    '        FoundElement = False
    '        For iCostFunctionElement = 0 To CostFunctionArray.GetLength(0) - 1
    '            If FileName = CostFunctionArray(iCostFunctionElement, 0) & ".csv" Then FoundElement = True
    '        Next

    '        If FoundElement = False Then
    '            Err.Raise(1000, "LoadCostFunctionsCSV", "Strategy file not listed in the CostFunctionType.csv file")
    '        End If

    '    Next

    '    Return CostFunctionArray

    'End Function


#End Region 'File I/O

#Region "MonteCarlo state save and restore "

    ''' <summary>
    ''' Save any variable that will be changed so the model can be restore to it's original state 
    ''' </summary>
    ''' <remarks>This just stores a sub set of variable as an example</remarks>
    Private Sub SaveOriginalState()
        Try
            'Have the MonteCarloManager save the values it will alter
            m_core.EcosimMonteCarlo.SaveOriginalValues()

            'Now store the variables that this app will change so they can be restored in RestoreOriginalState()

            'The makes sure Ecopath does not make a fuss, popping up message boxes, when it fails to balance a model
            Me.m_ecopath.suppressMessages = True

            'Make sure nothing is listening to Ecosim when we run it
            Me._EcosimTimeStepDelegate = Me.m_ecosim.TimeStepDelegate
            Me.m_ecosim.TimeStepDelegate = Nothing

            'Save any parameters that we are going to change 
            'This has not been implemented here but...
            'For igrp = 1 To Core.nLivingGroups
            '    MCGroup = MonteCarlo.Groups(igrp)
            '   _orgB(igrp) =  MCGroup.Bcv 
            '    'PB, QB...               
            'Next

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Restore the currently loaded model back to its original state so that it can be run in the interface.
    ''' </summary>
    ''' <remarks>In some cases you may want to save changes you made to the model.</remarks>
    Private Sub RestoreOriginalState()
        Try

            Dim iscenario As Integer = Me.m_core.ActiveEcosimScenarioIndex

            'Have the MonteCarloManager restore it's variables to the original state
            Me.RestoreParameters()
            Me.m_ecosim.TimeStepDelegate = Me._EcosimTimeStepDelegate

            ' No database changes left, yippee
            Me.Core.DiscardChanges()

            ' Just reload Ecosim
            Core.CloseEcosimScenario()

            Me.Core.LoadEcosimScenario(iscenario)

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub



    Private Sub SaveOriginalParameters()

        Dim ecopathData As cEcopathDataStructures = Me.m_ecopath.EcopathData
        Dim ecosimData As cEcosimDatastructures = Me.m_ecosim.EcosimData

        ReDim BTemp(ecopathData.B.Length - 1)
        ReDim PBTemp(ecopathData.PB.Length - 1)
        ReDim QBTemp(ecopathData.QB.Length - 1)
        ReDim EETemp(ecopathData.EE.Length - 1)
        ReDim BATemp(ecopathData.BA.Length - 1)
        ReDim DenDepCatchabilityTemp(ecosimData.QmQo.Length - 1)
        ReDim FeedingTimeAdjustRateTemp(ecosimData.FtimeAdjust.Length - 1)
        ReDim MaxRelFeedingTimeTemp(ecosimData.FtimeMax.Length - 1)
        ReDim OtherMortFeedingTimeTemp(ecosimData.MoPred.Length - 1)
        ReDim PredEffectFeedingTimeTemp(ecosimData.RiskTime.Length - 1)
        ReDim QBMaxxQBioTemp(ecosimData.CmCo.Length - 1)
        ReDim SwitchingPowerTemp(ecosimData.SwitchPower.Length - 1)
        ReDim VulnerabilitiesTemp(ecosimData.VulMult.GetLength(0) - 1, ecosimData.VulMult.GetLength(1) - 1)
        ReDim DietMatrixTemp(ecopathData.DC.GetLength(0) - 1, ecopathData.DC.GetLength(1) - 1)
        ReDim DietImpTemp(m_core.nGroups - 1)

        For x = 0 To ecopathData.B.Length - 1
            BTemp(x) = ecopathData.B(x)
            PBTemp(x) = ecopathData.PB(x)
            QBTemp(x) = ecopathData.QB(x)
            EETemp(x) = ecopathData.EE(x)
            BATemp(x) = ecopathData.BA(x)
            DenDepCatchabilityTemp(x) = ecosimData.QmQo(x)
            FeedingTimeAdjustRateTemp(x) = ecosimData.FtimeAdjust(x)
            MaxRelFeedingTimeTemp(x) = ecosimData.FtimeMax(x)
            OtherMortFeedingTimeTemp(x) = ecosimData.MoPred(x)
            PredEffectFeedingTimeTemp(x) = ecosimData.RiskTime(x)
            QBMaxxQBioTemp(x) = ecosimData.CmCo(x)
            SwitchingPowerTemp(x) = ecosimData.SwitchPower(x)
        Next
        For x = 0 To ecopathData.DC.GetLength(0) - 1
            For y = 0 To ecopathData.DC.GetLength(1) - 1
                DietMatrixTemp(x, y) = ecopathData.DC(x, y)
            Next
            'DietImpTemp(x) = mCore.EcoPathGroupInputs(x + 1).ImpDiet
        Next
        For x = 1 To ecosimData.VulMult.GetLength(0) - 1
            For y = 0 To ecosimData.VulMult.GetLength(1) - 1
                VulnerabilitiesTemp(x, y) = ecosimData.VulMult(x, y)
            Next
        Next

        OriginalNTimesteps = m_ecosim.EcosimData.NTimes

    End Sub

    Private Sub RestoreParameters()

        Dim ecopathData As cEcopathDataStructures = Me.m_ecopath.EcopathData
        Dim ecosimData As cEcosimDatastructures = Me.m_ecosim.EcosimData

        For x = 0 To ecopathData.B.Length - 1
            ecopathData.B(x) = CSng(BTemp(x))
            ecopathData.PB(x) = CSng(PBTemp(x))
            ecopathData.QB(x) = CSng(QBTemp(x))
            ecopathData.EE(x) = CSng(EETemp(x))
            ecopathData.BA(x) = CSng(BATemp(x))
            ecosimData.QmQo(x) = CSng(DenDepCatchabilityTemp(x))
            ecosimData.FtimeAdjust(x) = CSng(FeedingTimeAdjustRateTemp(x))
            ecosimData.FtimeMax(x) = CSng(MaxRelFeedingTimeTemp(x))
            ecosimData.MoPred(x) = CSng(OtherMortFeedingTimeTemp(x))
            ecosimData.RiskTime(x) = CSng(PredEffectFeedingTimeTemp(x))
            ecosimData.CmCo(x) = CSng(QBMaxxQBioTemp(x))
            ecosimData.SwitchPower(x) = CSng(SwitchingPowerTemp(x))
        Next

        For x = 0 To ecopathData.DC.GetLength(0) - 1
            For y = 0 To ecopathData.DC.GetLength(1) - 1
                ecopathData.DC(x, y) = CSng(DietMatrixTemp(x, y))
            Next
        Next

        'I don't think we should do this 
        'We never changed the input/output dietmatrix
        'For x = 1 To mCore.nGroups
        '    For y = 1 To mCore.nLivingGroups
        '        mCore.EcoPathGroupInputs(x).DietComp(y) = CSng(DietMatrixTemp(x - 1, y - 1))
        '    Next
        'Next

        For x = 1 To ecosimData.VulMult.GetLength(0) - 1
            For y = 0 To ecosimData.VulMult.GetLength(1) - 1
                ecosimData.VulMult(x, y) = CSng(VulnerabilitiesTemp(x, y))
            Next
        Next

        Me.Core.DiscardChanges()

    End Sub

#End Region 'MonteCarlo state save and restore

#Region "Private modeling code"

    ''' <summary>
    ''' Resets the effort to the maximum specifed effort for the project time steps
    ''' </summary>
    ''' <remarks>The effort is used is the effort determined through regulation unless greater than this maximum effort</remarks>
    Private Sub ResetEffortToMax(StartT As Integer, EndT As Integer)
        Dim MSEMaxEffort As Single = 200

        Try

            For iflt As Integer = 1 To m_ecopath.EcopathData.NumFleet
                'Only if this fleet is regulated

                For it As Integer = StartT To EndT
                    m_ecosim.EcosimData.FishRateGear(iflt, it) = MSEMaxEffort
                Next it

            Next iflt

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub Run()

        ' JS 20Oct13: Fixed path usage
        ' JS 20Oct13: Used standard CSV field reading/writing; all names CSV protected and values written in US-en notation
        ' JS 20Oct13: Applied standard EwE headers. Mark, please don't kill me

        Debug.Assert(Me.IsInputDataCompatible)
        Dim nModels As Integer
        Dim GoodDynamics As Boolean
        ' Dim diet_matrix As CsvReader
        Dim nResultIters As Integer
        Dim nFleetIters As Integer
        Dim nFailedModels As Integer
        Dim BiomassLimits As cBiomassLimits
        Dim ModelValid As Boolean

        Dim swGroup As StreamWriter = Nothing
        Dim swFleet As StreamWriter = Nothing
        Dim swFleetEffort As StreamWriter = Nothing

        Try

            Me.StockAssessment.BeginRun()

            Dim BiomassProjected(Me.NYearsProject * m_ecosim.EcosimData.NumStepsPerYear - 1) As Double

            Dim msgReport As New cFeedbackMessage("?", eCoreComponentType.External, eMessageType.DataExport, eMessageImportance.Information, eMessageReplyStyle.OK)
            msgReport.Hyperlink = cMSEUtils.MSEFolder(Me.DataPath, cMSEUtils.eMSEPaths.Results)

            'Save the original Ecopath and Ecosim parameter values 
            'so the model can be restored at the end of the run
            Me.SaveOriginalParameters()

            'Set the TechnologyCreep(nfleets) to one for all fleets
            'No technology creep for us
            Me.initTechnologyCreep()

            'Open the "Results.csv" and "Fleet.cvs" file and write the header info
            Me.initResultFiles(msgReport, swGroup, swFleet)

            'Read all the parameters from the <parameter name>_out.csv files into memory
            Me.readEcopathEcosimParameters()

            'Prepare the trajectory csv with the column headings
            Trajectory2Csv = New List(Of StreamWriter)
            Me.initTrajectoryByGroupFiles(msgReport, Trajectory2Csv)

            'Prepare the effort trajectory csv with the column headings
            swFleetEffort = Me.initTrajectoryEffortFiles(msgReport)

            ReDim TargConsQuota(m_core.nGroups - 1, 1)
            ReDim MinEffortThisYear(m_core.nFleets - 1)

            'increase the number of years for the projection
            m_core.EcoSimModelParameters.NumberYears = CInt(OriginalNTimesteps / m_ecosim.EcosimData.NumStepsPerYear + NYearsProject)

            'Tell Ecopath not to send out messages
            Me.m_ecopath.suppressMessages = True

            'Initialise and load from CSV the biomass limits
            BiomassLimits = New cBiomassLimits(m_plugin)
            BiomassLimits.LoadLimitsFromCSV()

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Run The Trials 
            'load parameter values into ecopath and ecosim to be used
            nModels = Me.NModels2Run    '0 is the 1st dimension and 1' the second etc
            For iModel = 1 To nModels
                ModelValid = True

                Dim FleetEffortTable As New DataTable
                FleetEffortTable.Columns.Add("ModelID", GetType(Integer))
                FleetEffortTable.Columns.Add("StrategyName", GetType(String))
                FleetEffortTable.Columns.Add("FleetNumber", GetType(Integer))
                FleetEffortTable.Columns.Add("FleetName", GetType(String))
                FleetEffortTable.Columns.Add("Effort", GetType(Single()))

                Dim FleetCatchTable As New DataTable
                FleetCatchTable.Columns.Add("ModelID", GetType(Integer))
                FleetCatchTable.Columns.Add("StrategyName", GetType(String))
                FleetCatchTable.Columns.Add("FleetNumber", GetType(Integer))
                FleetCatchTable.Columns.Add("FleetName", GetType(String))
                FleetCatchTable.Columns.Add("GroupNumber", GetType(Integer))
                FleetCatchTable.Columns.Add("GroupName", GetType(String))
                FleetCatchTable.Columns.Add("CatchAtLastTimeStep", GetType(Single))

                Dim ResultsTable As New DataTable
                ResultsTable.Columns.Add("ModelID", GetType(Integer))
                ResultsTable.Columns.Add("StrategyName", GetType(String))
                ResultsTable.Columns.Add("ID", GetType(Integer))
                ResultsTable.Columns.Add("Name", GetType(String))
                ResultsTable.Columns.Add("ResultName", GetType(String))
                ResultsTable.Columns.Add("ResultValue", GetType(Double))

                Dim TrajectoryTable As New DataTable
                TrajectoryTable.Columns.Add("ModelID", GetType(Integer))
                TrajectoryTable.Columns.Add("StrategyName", GetType(String))
                TrajectoryTable.Columns.Add("GroupID", GetType(Integer))
                TrajectoryTable.Columns.Add("GroupName", GetType(String))
                TrajectoryTable.Columns.Add("Biomass", GetType(Single()))

                ResetEffortToMax(OriginalNTimesteps + 1, m_core.EcoSimModelParameters.NumberYears * m_ecosim.EcosimData.NumStepsPerYear)

                cApplicationStatusNotifier.UpdateProgress(Me.Core, String.Format(My.Resources.STATUS_RUN_PROGRESS, My.Resources.CAPTION, iModel), CSng(iModel / nModels))

                'Only run the Strategies if the parameters loaded
                If Me.updateEcopathEcosimParameters(iModel) Then
                    'Yep loaded all the parameters from file or memory

                    Try
                        'Run Ecopath with the parameters updated above
                        Dim bEcopathRan As Boolean
                        bEcopathRan = Me.m_ecopath.Run()
                        'this should not happen 
                        Debug.Assert(bEcopathRan, Me.ToString + ".Run() Ecopath failed to run from balanced parameter set.")

                        'This creates the files we will write the biomass trajectories to
                        TrajectoryCsv = Me.initTrialTrajectoryFile(msgReport, iModel)

                        Me.m_StockAssessment.TrialNumber = iModel

                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        'Loop over all the strategies for this trial
                        For Each curStrategy As Strategy In Strategies

                            'Set the CurrentStrategy used by onEcosimTimeStep()
                            m_currentStrategy = curStrategy

                            'Get a list of all fleets that fish the groups that have HCRs
                            'Populates FleetsTheFishHCRGroup() which is used by onEcosimTimeStep() to optimize the fleets it loops over
                            Me.initFishedByHCR(curStrategy)

                            If Me.RunEcosim() Then
                                'Save the Ecosim results
                                GoodDynamics = Me.SaveResults2RAM(iModel, nResultIters, nFleetIters, ResultsTable, FleetCatchTable, FleetEffortTable, TrajectoryTable, BiomassLimits)
                            Else
                                GoodDynamics = False
                                nFailedModels += 1
                                Exit For
                            End If

                            'If one of the groups colapsed during the Ecosim run 
                            'Reject this parameter set

                        Next curStrategy
                        'End of Strategy loop
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                        If GoodDynamics Then
                            SaveResults2CSV(FleetEffortTable, FleetCatchTable, ResultsTable, TrajectoryTable, _
                                            swFleetEffort, swFleet, swGroup, TrajectoryCsv, Trajectory2Csv)
                        End If

                        cMSEUtils.ReleaseWriter(TrajectoryCsv)

                    Catch ex As Exception
                        Debug.Assert(False, Me.ToString & ".Run() Exception: " & ex.Message)
                    End Try
                End If

                'BadDynamics:  ' This is so that if the dynamics of a parameterisation are bad that we can skip out of the loops and onto the the next Trial
                GoodDynamics = True

            Next iModel
            'End of trials loop
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'ecosimData.NTimes is the number of months so 17 years = 204 timesteps
            m_core.EcoSimModelParameters.NumberYears = CInt(OriginalNTimesteps / m_ecosim.EcosimData.NumStepsPerYear)

            'Provide user with a message stating how many of the Trials produced reasonable dynamics
            msgReport.Message = String.Format(My.Resources.PROMPT_TRIAL_REPORT, (nModels - nFailedModels), nModels, CInt((nModels - nFailedModels) * 100 / nModels))
            Me.Core.Messages.SendMessage(msgReport)

        Catch ex As Exception
            cLog.Write(ex)
            'Warn the user
            Me.Core.Messages.SendMessage(New cMessage("CEFAS MSE Failed to run MSE trials due to exception " + ex.Message, _
                                                      eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Information))
        End Try

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Cleanup even if there has been an exception
        If Trajectory2Csv IsNot Nothing Then
            For Each strmWriter As StreamWriter In Trajectory2Csv
                cMSEUtils.ReleaseWriter(strmWriter)
            Next
            Trajectory2Csv.Clear()
        End If

        cMSEUtils.ReleaseWriter(swGroup)
        cMSEUtils.ReleaseWriter(swFleet)
        cMSEUtils.ReleaseWriter(swFleetEffort)

        Me.StockAssessment.RunEnded()

        Me.RestoreOriginalState()
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    End Sub

    Private Sub SaveResults2CSV(ByRef FleetEffortTable As DataTable, ByRef FleetCatchTable As DataTable, ByRef GroupResultsTable As DataTable, _
                         ByRef TrajectoryTable As DataTable, _
                         ByRef swFleetEffort As StreamWriter, ByRef swFleet As StreamWriter, ByRef swGroup As StreamWriter, _
                         ByRef swTrajectory As StreamWriter, ByRef swTrajectory2 As List(Of StreamWriter))

        'Save FleetEffortTable to CSV
        'swFleetEffort.Write("Model,Strategy,FleetNumber,FleetName")
        For iTimeStep As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
            swFleetEffort.Write("," & iTimeStep)
        Next
        swFleetEffort.WriteLine()
        For iRow = 0 To FleetEffortTable.Rows.Count - 1
            Dim RowData As Data.DataRow
            Dim EffortVals() As Single
            RowData = FleetEffortTable.Rows(iRow)
            EffortVals = RowData.Field(Of Single())("Effort")
            swFleetEffort.Write("{0},{1},{2},{3}", _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("ModelID")), cStringUtils.ToCSVField(RowData.Field(Of String)("StrategyName")), _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("FleetNumber")), cStringUtils.ToCSVField(RowData.Field(Of String)("FleetName")))
            For iTimeStep As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                swFleetEffort.Write("," & cStringUtils.FormatNumber(EffortVals(iTimeStep - 1)))
            Next
            swFleetEffort.WriteLine()
        Next

        'Save the FleetCatchTable to CSV
        'swFleet.WriteLine("Model,Strategy,FleetNumber,FleetName,GroupNumber,GroupName,Value")
        For iRow = 0 To FleetCatchTable.Rows.Count - 1
            Dim RowData As Data.DataRow = FleetCatchTable.Rows(iRow)
            swFleet.WriteLine("{0},{1},{2},{3},{4},{5},{6}", _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("ModelID")), _
                                cStringUtils.ToCSVField(RowData.Field(Of String)("StrategyName")), _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("FleetNumber")), _
                                cStringUtils.ToCSVField(RowData.Field(Of String)("FleetName")), _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("GroupNumber")), _
                                cStringUtils.ToCSVField(RowData.Field(Of String)("GroupName")), _
                                cStringUtils.FormatNumber(RowData.Field(Of Single)("CatchAtLastTimeStep")))
        Next

        'Save the GroupResultsTable to CSV
        'swGroup.WriteLine("Model,Strategy,ID,Name,ResultName,ResultValue")
        For iRow = 0 To GroupResultsTable.Rows.Count - 1
            Dim RowData As Data.DataRow = GroupResultsTable.Rows(iRow)
            swGroup.WriteLine("{0},{1},{2},{3},{4},{5}", _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("ModelID")), _
                                cStringUtils.ToCSVField(RowData.Field(Of String)("StrategyName")), _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("ID")), _
                                cStringUtils.ToCSVField(RowData.Field(Of String)("Name")), _
                                cStringUtils.ToCSVField(RowData.Field(Of String)("ResultName")), _
                                cStringUtils.FormatNumber(RowData.Field(Of Double)("ResultValue")))
        Next

        'Save the trajectories by group
        For iRow = 0 To TrajectoryTable.Rows.Count - 1
            Dim RowData As Data.DataRow
            Dim Biomass() As Single
            RowData = TrajectoryTable.Rows(iRow)
            Biomass = RowData.Field(Of Single())("Biomass")
            swTrajectory.Write("{0},{1},{2}", _
                                cStringUtils.FormatNumber(RowData.Field(Of Integer)("GroupID")), cStringUtils.ToCSVField(RowData.Field(Of String)("GroupName")), _
                                cStringUtils.ToCSVField(RowData.Field(Of String)("StrategyName")))
            For iTimeStep As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                swTrajectory.Write("," & cStringUtils.FormatNumber(Biomass(iTimeStep - 1)))
            Next
            swTrajectory.WriteLine()
        Next

        For iRow = 0 To TrajectoryTable.Rows.Count - 1
            Dim RowData As Data.DataRow
            Dim Biomass() As Single
            RowData = TrajectoryTable.Rows(iRow)
            Biomass = RowData.Field(Of Single())("Biomass")
            Trajectory2Csv(RowData.Field(Of Integer)("GroupID") - 1).Write(cStringUtils.FormatNumber(RowData.Field(Of Integer)("ModelID")) & "," & cStringUtils.ToCSVField(RowData.Field(Of String)("StrategyName")))
            For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                Trajectory2Csv(RowData.Field(Of Integer)("GroupID") - 1).Write("," & cStringUtils.FormatNumber(Biomass(iTime - 1)))
            Next
            Trajectory2Csv(RowData.Field(Of Integer)("GroupID") - 1).WriteLine()
        Next

    End Sub


    Private Sub initFishedByHCR(curStrategy As Strategy)
        'Clear the data from the last Strategy
        FleetsThatFishHCRGrp.Clear()

        'Get a list of all fleets that fish the groups that have HCRs
        For iFleet As Integer = 1 To m_core.nFleets
            For Each HCRGroup In curStrategy
                If m_core.FleetInputs(iFleet).Landings(HCRGroup.GroupF.Index) + m_core.FleetInputs(iFleet).Discards(HCRGroup.GroupF.Index) > 0 Then
                    If Not FleetsThatFishHCRGrp.Contains(iFleet) Then
                        FleetsThatFishHCRGrp.Add(iFleet)
                    End If
                    'Exit For
                End If
            Next HCRGroup
        Next iFleet

    End Sub


    Private Sub initTechnologyCreep()

        'an array where each element represents the percentage with which each fleet increases its catching efficiency each year
        TechnologyCreep = New Single(m_core.nFleets) {}
        For iTechCreep As Integer = 1 To m_core.nFleets
            TechnologyCreep(iTechCreep) = 1
        Next

    End Sub

    ''' <summary>
    ''' Read the DietMatrix file for this trial and populate the Ecopath diet matrix
    ''' </summary>
    ''' <param name="iTrial"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function updateDietMatrixFromCSVFile(ByVal iTrial As Integer) As Boolean

        Dim GoodDynamics As Boolean = True
        Dim csvDietMatrix As CsvReader = Nothing
        Dim strmReader As StreamReader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "DietMatrixTrial" & iTrial & ".csv"))

        If (strmReader IsNot Nothing) Then
            csvDietMatrix = New CsvReader(strmReader, False)
            If csvDietMatrix.ReadNextRecord() Then
                For iPred As Integer = 1 To m_core.nLivingGroups
                    'NOT DCInput because we don't have the support that copies this into DC
                    _pathdata.DC(iPred, 0) = cStringUtils.ConvertToSingle(csvDietMatrix(iPred - 1))
                Next
            Else
                ' Unable to read predator header line! We have a problem
                GoodDynamics = False
            End If

            'Me.dumpDietMatrix()
            For iPrey As Integer = 1 To m_core.nGroups
                If (Not csvDietMatrix.EndOfStream) And (csvDietMatrix.ReadNextRecord()) Then
                    For iPred As Integer = 1 To m_core.nLivingGroups
                        'If m_ecopath.EcopathData.DC(iPred, iPrey) > 0 Then
                        'Debug.Assert(cStringUtils.ConvertToSingle(csvDietMatrix(iPred - 1)) > 0)
                        'NOT DCInput because we don't have the support that copies this into DC
                        _pathdata.DC(iPred, iPrey) = cStringUtils.ConvertToSingle(csvDietMatrix(iPred - 1))
                        'End If

                    Next
                Else
                    ' Unable to read prey line! We have a problem
                    GoodDynamics = False
                End If
            Next
        Else
            ' Could not read diet matrix for this trial
            GoodDynamics = False
        End If

        cMSEUtils.ReleaseReader(strmReader)
        If csvDietMatrix IsNot Nothing Then csvDietMatrix.Dispose()

        'Me.dumpDietMatrix()

        Return GoodDynamics

    End Function

    Private Sub DumpFishingEffort()

        Try
            Dim strm As New System.IO.StreamWriter("C:\Users\Mark\Desktop\GAP\Data\Results\fishingEffort.csv", True)
            strm.WriteLine("iter")

            strm.WriteLine("-----------------Start Fishing Effort Matrix-----------------------")
            For iFleet As Integer = 1 To m_core.nFleets
                strm.Write("Fleet = " & m_core.EcosimFleetInputs(iFleet).Name & ",")
                For iTimeStep As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                    strm.Write(Me._simdata.ResultsEffort(iFleet, iTimeStep).ToString() + ",")
                Next
                strm.WriteLine()
            Next
            strm.WriteLine("-----------------Start Fishing Effort Matrix------------------------")
            strm.Close()
        Catch ex As Exception
            ' Aargh
        End Try

    End Sub

    ''' <summary>
    ''' Used for debugging to test the diet matrix created here against the Ecopath diet matrix 
    ''' </summary>
    ''' <remarks>
    ''' There is a equivalent method in Ecopath that dumps the diet matrix use for the current iteration out to file. 
    ''' These file can then be compared.
    ''' </remarks>
    Private Sub dumpDietMatrix()

        Try
            Dim strm As New System.IO.StreamWriter("MSEDietMatrix.csv", True)
            strm.WriteLine("iter")

            strm.WriteLine("-----------------Start Diet Matrix-----------------------")
            For iprey As Integer = 1 To m_core.nGroups
                For ipred As Integer = 1 To m_core.nLivingGroups
                    strm.Write(Me._pathdata.DCInput(ipred, iprey).ToString() + ",")
                Next
                strm.WriteLine()
            Next
            strm.WriteLine("-----------------End Diet Matrix------------------------")
            strm.Close()
        Catch ex As Exception
            ' Aargh
        End Try

    End Sub

    Private Function SaveResults2RAM(ByVal iModel As Integer, ByRef NumberIterationsAlreadyInResults As Integer, _
                                        ByVal NumberIterationsAlreadyInFleets As Integer, ByRef ResultsTable As DataTable,
                                        ByRef FleetCatchTable As DataTable, ByRef FleetEffortTable As DataTable,
                                        ByRef TrajectoryTable As DataTable, ByRef BiomassLimits As cBiomassLimits) As Boolean

        Dim GoodDynamics As Boolean = True

        Dim BiomassProjected(NYearsProject * m_ecosim.EcosimData.NumStepsPerYear - 1) As Double

        nSuccessfullyProjectedModels = 0

        'This outputs information that can be used to resolve issues with the biomass limits are exceeded
        Dim DiagnosticOutput4BiomassLimits = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.Results, "BadDynamicsTrajectories.csv"), True)
        Dim MaxBiomass As Single
        Dim MinBiomass As Single
        For Each iGrp In BiomassLimits.lstBiomassLimits
            MaxBiomass = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, OriginalNTimesteps + 1)
            MinBiomass = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, OriginalNTimesteps + 1)
            For iTimeStep As Integer = OriginalNTimesteps + 2 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                If Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, iTimeStep) > MaxBiomass Then
                    MaxBiomass = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, iTimeStep)
                End If
                If Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, iTimeStep) < MinBiomass Then
                    MinBiomass = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, iTimeStep)
                End If
            Next
            DiagnosticOutput4BiomassLimits.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", _
                        cStringUtils.FormatNumber(iModel), _
                        cStringUtils.ToCSVField(m_currentStrategy.Name), _
                        cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(iGrp.mGroup.Index).Name), _
                        cStringUtils.FormatNumber(MaxBiomass), _
                        cStringUtils.FormatNumber(iGrp.mUpperLimit), _
                        cStringUtils.FormatNumber(MaxBiomass / iGrp.mUpperLimit), _
                        cStringUtils.FormatNumber(MinBiomass), _
                        cStringUtils.FormatNumber(iGrp.mLowerLimit), _
                        cStringUtils.FormatNumber(MinBiomass <= iGrp.mLowerLimit))
        Next
        cMSEUtils.ReleaseWriter(DiagnosticOutput4BiomassLimits)

        'Check whether the biomass for any species goes beneath or hits zero
        For iGrp As Integer = 1 To m_core.nLivingGroups
            For iTimeStepHistoric As Integer = 1 To OriginalNTimesteps
                'Console.Write(mCore.EcoSimGroupOutputs(iGrp).Biomass(iTimeStep).ToString & " ")
                If Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, iTimeStepHistoric) <= 1.0E-20 Then
                    GoodDynamics = False
                End If
                'If mCore.EcoSimGroupOutputs(iGrp).Biomass(iTimeStep) <= 0 Then GoodDynamics = False
                'Test
                If GoodDynamics = False Then
                    Exit For
                End If
            Next
            If GoodDynamics = False Then Exit For
        Next iGrp

        'Code to test whether the efforts are reasonable
        'DumpFishingEffort()

        'Only commented out for testing - uncomment when finished TODO_mp
        For Each iGrp In BiomassLimits.lstBiomassLimits
            For iTimeStep As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                'Check projection is above minimum biomass
                If Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, iTimeStep) <= iGrp.mLowerLimit Then
                    GoodDynamics = False
                    GoodDynamics = True ' test to make it output the biomass trajectories - todo comment out
                    Exit For
                End If
                'check the projection is above the max biomass
                If Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp.mGroup.Index, iTimeStep) > iGrp.mUpperLimit Then
                    GoodDynamics = False
                    GoodDynamics = True ' test to make it output the biomass trajectories - todo comment out
                    Exit For
                End If
            Next
            If GoodDynamics = False Then Exit For
        Next

        If GoodDynamics = False Then
            Console.WriteLine("This set of parameters is no good")
        Else

            'Output the trajectories of the efforts
            For iFleet As Integer = 1 To m_core.nFleets
                Dim EffortVals(OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear - 1) As Single
                For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                    EffortVals(iTime - 1) = Me._simdata.ResultsEffort(iFleet, iTime)
                Next
                FleetEffortTable.Rows.Add(iModel, m_currentStrategy.Name, iFleet, m_core.FleetInputs(iFleet).Name, EffortVals)
            Next

            For iFleet As Integer = 1 To m_core.nFleets
                For iGrp As Integer = 1 To m_core.nLivingGroups
                    FleetCatchTable.Rows.Add(iModel + NumberIterationsAlreadyInFleets, m_currentStrategy.Name, _
                                             iFleet, m_core.FleetInputs(iFleet).Name, iGrp, m_core.EcoPathGroupInputs(iGrp).Name, _
                                            Me._simdata.ResultsSumCatchByGroupGear(iGrp, iFleet, OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear))
                Next
            Next

            For iGrp As Integer = 1 To m_core.nLivingGroups
                'calculate what the minimum biomass was for each group
                For iTime As Integer = 1 To NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                    BiomassProjected(iTime - 1) = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, OriginalNTimesteps + iTime)
                Next

                Dim BiomassVals(OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear - 1) As Single
                For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                    BiomassVals(iTime - 1) = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, iTime)
                Next
                TrajectoryTable.Rows.Add(iModel, m_currentStrategy.Name, iGrp, m_core.EcoPathGroupInputs(iGrp).Name, BiomassVals)

                ResultsTable.Rows.Add(NumberIterationsAlreadyInResults + iModel, m_currentStrategy.Name, iGrp, _
                                           m_core.EcoPathGroupOutputs(iGrp).Name, "Biomass", BiomassProjected.Min)
                ResultsTable.Rows.Add(NumberIterationsAlreadyInResults + iModel, m_currentStrategy.Name, iGrp, _
                                           m_core.EcoPathGroupOutputs(iGrp).Name, "BiomassEnd", Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, Me._simdata.NTimes))
                ResultsTable.Rows.Add(NumberIterationsAlreadyInResults + iModel, m_currentStrategy.Name, iGrp, _
                                           m_core.EcoPathGroupOutputs(iGrp).Name, "Catch", Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, iGrp, Me._simdata.NTimes))
            Next

            For iFleet As Integer = 1 To m_core.nFleets
                ResultsTable.Rows.Add(NumberIterationsAlreadyInResults + iModel, m_currentStrategy.Name, iFleet, _
                                      m_core.FleetInputs(iFleet).Name, "TotalEndValue", Me._simdata.ResultsSumValueByGear(iFleet, m_ecosim.EcosimData.NTimes))
            Next

        End If

        Return GoodDynamics
    End Function

    Private Function updateEcopathEcosimParameters(iModel As Integer) As Boolean


        'Update the Ecopath and Ecosim parameters from the data read into memory by Me.readEcopathEcosimParameters()
        Me.updateParametersFromMemory(iModel)
        Survivability.ConfigCoreWithSurvivabilities(iModel)

        ' Me.updateDietRandom()
        'Return Me.readDietMatrix(iTrial)
        ' Return True
        'Diet matrix parameters are stored in file by iTrial
        'Read the file and update the dietmatrix parameters
        Return Me.updateDietMatrixFromCSVFile(iModel)
        Return True


    End Function


    ''' <summary>
    ''' Populte the Ecopath and Ecosim parameter with values read into memory by Me.readEcopathEcosimParameters()
    ''' </summary>
    ''' <param name="itrial"></param>
    ''' <remarks></remarks>
    Private Sub updateParametersFromMemory(itrial As Integer)

        For igrp = 1 To m_core.nLivingGroups
            Debug.Assert(Not B(itrial - 1, igrp - 1) = cCore.NULL_VALUE, "Oppss something is very wrong with the parameters read from file.")
            If Not B(itrial - 1, igrp - 1) = cCore.NULL_VALUE Then
                Me._pathdata.B(igrp) = CSng(B(itrial - 1, igrp - 1))
                Me._pathdata.PB(igrp) = CSng(PB(itrial - 1, igrp - 1))
                Me._pathdata.QB(igrp) = CSng(QB(itrial - 1, igrp - 1))
                Me._pathdata.EE(igrp) = CSng(EE(itrial - 1, igrp - 1))
                Me._pathdata.BA(igrp) = CSng(BA(itrial - 1, igrp - 1))
                If Not m_core.EcoPathGroupInputs(igrp).IsProducer Then
                    Me._simdata.QmQo(igrp) = CSng(DenDepCatchability(itrial - 1, igrp - 1))
                    Me._simdata.FtimeAdjust(igrp) = CSng(FeedingTimeAdjustRate(itrial - 1, igrp - 1))
                    Me._simdata.FtimeMax(igrp) = CSng(MaxRelFeedingTime(itrial - 1, igrp - 1))
                    Me._simdata.MoPred(igrp) = CSng(OtherMortFeedingTime(itrial - 1, igrp - 1))
                    Me._simdata.RiskTime(igrp) = CSng(PredEffectFeedingTime(itrial - 1, igrp - 1))
                    Me._simdata.CmCo(igrp) = CSng(QBMaxxQBio(itrial - 1, igrp - 1))
                    Me._simdata.SwitchPower(igrp) = CSng(SwitchingPower(itrial - 1, igrp - 1))
                    If m_core.EcoPathGroupInputs(igrp).IsProducer Then Stop
                End If
            End If 'Not B(iTrial - 1, igrp - 1) = cCore.NULL_VALUE 
        Next igrp


        'Vulnerabilities
        For iPrey As Integer = 1 To m_core.nGroups

            For iPred As Integer = 1 To m_core.nLivingGroups
                'For iPred As Integer = 1 To Vulnerabilities.GetLength(2)
                If Not Vulnerabilities(itrial - 1, iPred - 1, iPrey - 1) = cCore.NULL_VALUE Then
                    Me._simdata.VulMult(iPrey, iPred) = CSng(Vulnerabilities(itrial - 1, iPred - 1, iPrey - 1))
                Else
                    Me._simdata.VulMult(iPrey, iPred) = cCore.NULL_VALUE
                End If
            Next
        Next

    End Sub


    ''' <summary>
    ''' Initialize the biomass and fleet results files
    ''' </summary>
    ''' <param name="msgReport"></param>
    ''' <param name="strmGroup"></param>
    ''' <param name="strmFleet"></param>
    ''' <remarks></remarks>
    Private Sub initResultFiles(ByVal msgReport As cMessage, ByRef strmGroup As StreamWriter, ByRef strmFleet As StreamWriter)

        'Output the final results
        strmGroup = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.Results, "Results.csv"), False)
        If Me.m_core.SaveWithFileHeader Then strmGroup.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))
        strmGroup.WriteLine("Iteration,Strategy,GroupNumber,GroupName,ResultName,Value")
        msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "Results.csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

        'Create the csv writer for writing out individual fleets catches of each group
        strmFleet = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.Results, "Fleet.csv"), False)
        If Me.m_core.SaveWithFileHeader Then strmFleet.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))
        strmFleet.WriteLine("Iteration,Strategy,FleetNumber,FleetName,GroupNumber,GroupName,Value")
        msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "Fleet.csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

        ''Create the csv writer for writing out individual fleet efforts
        'strmFleetEffort = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.Results, "FleetEfforts.csv"), False)
        'If Me.m_core.SaveWithFileHeader Then strmFleetEffort.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))
        'strmFleetEffort.WriteLine("Iteration,Strategy,FleetNumber,FleetName,Effort")
        'msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "FleetEfforts.csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

    End Sub

    Private Function initTrialTrajectoryFile(msgReport As cMessage, iModel As Integer) As StreamWriter
        Dim strm As StreamWriter
        strm = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ResultsTrajectories, "Model" & iModel & ".csv"), False)
        If Me.m_core.SaveWithFileHeader Then strm.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))
        strm.Write("GroupNumber,Group,Strategy")
        msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "Model" & iModel & ".csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

        For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
            strm.Write("," & cStringUtils.FormatNumber(iTime))
        Next
        strm.WriteLine()
        Return strm
    End Function

    Private Function initTrajectoryEffortFiles(msgReport As cMessage) As StreamWriter
        Dim strm As StreamWriter
        strm = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.Results, "EffortTrajectories.csv"), False)
        If Me.m_core.SaveWithFileHeader Then strm.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))
        strm.Write("Model,Strategy,FleetNumber,FleetName")
        msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "EffortTrajectories.csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

        For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
            strm.Write("," & cStringUtils.FormatNumber(iTime))
        Next
        strm.WriteLine()
        Return strm
    End Function



    Private Sub initTrajectoryByGroupFiles(ByVal msgReport As cMessage, ByVal TrajectoryList As List(Of StreamWriter))

        For igrp = 1 To m_core.nLivingGroups

            Dim strFile As String = cFileUtils.ToValidFileName(m_core.EcoPathGroupInputs(igrp).Name & "_GroupNo" & igrp & ".csv", False)
            Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ResultsTraj2, strFile))
            msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, strFile), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

            Debug.Assert(writer IsNot Nothing)

            TrajectoryList.Add(writer)
            If Me.m_core.SaveWithFileHeader Then TrajectoryList(igrp - 1).WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))
            TrajectoryList(igrp - 1).Write("Model,Strategy")
            For iTime = 1 To OriginalNTimesteps + NYearsProject * m_ecosim.EcosimData.NumStepsPerYear
                TrajectoryList(igrp - 1).Write("," & cStringUtils.FormatNumber(iTime))
            Next
            TrajectoryList(igrp - 1).WriteLine()

        Next

    End Sub

    ''' <summary>
    ''' Load all the parameters from the [parameter name]_out.csv files
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub readEcopathEcosimParameters()
        'Load all the parameters from the <parameter name>_out.csv files
        B = ExtractParamsCSV("B")
        PB = ExtractParamsCSV("PB")
        QB = ExtractParamsCSV("QB")
        EE = ExtractParamsCSV("EE")
        BA = ExtractParamsCSV("BA")
        DenDepCatchability = ExtractParamsCSV("DenDepCatchability")
        FeedingTimeAdjustRate = ExtractParamsCSV("FeedingTimeAdjustRate")
        MaxRelFeedingTime = ExtractParamsCSV("MaxRelFeedingTime")
        OtherMortFeedingTime = ExtractParamsCSV("OtherMortFeedingTime")
        PredEffectFeedingTime = ExtractParamsCSV("PredEffectFeedingTime")
        QBMaxxQBio = ExtractParamsCSV("QBMaxxQBio")
        SwitchingPower = ExtractParamsCSV("SwitchingPower")
        Vulnerabilities = ExtractVulnerabilitiesCSV()
    End Sub

    Private Sub GenerateEcopathParamaters()

        Me.SaveOriginalParameters()
        Try
            Me.GenerateInputStructure()
        Catch ex As Exception
            ' Kaboom!
        End Try
        ' Re-assess configuration when next needed
        Me.InvalidateConfigurationState()
        Me.RestoreOriginalState()

    End Sub

    Private Sub GenerateInputStructure()

        ' JS 12Oct13: Fixed path usage
        ' JS 12Oct13: Used standard CSV field reading/writing
        ' JS 12Oct13: Used standard readers/writers, and made robust

        Dim nLiving As Integer = m_core.nLivingGroups
        Dim nGroups As Integer = m_core.nGroups
        Dim MonteCarlo As cMonteCarloManager = m_core.EcosimMonteCarlo
        Dim nTrials As Integer = Me.NModels
        Dim b(nTrials, nGroups) As Single
        Dim ba(nTrials, nLiving) As Single
        Dim pb(nTrials, nLiving) As Single
        Dim qb(nTrials, nLiving) As Single
        Dim ee(nTrials, nLiving) As Single
        Dim TimeFindingBalanced As New Stopwatch
        Dim csv As CsvReader
        'Dim nPPers As Integer 'number of primary producers
        'Dim nLivingMinusPPers As Integer 'number of living groups minus primary producers
        'Const PQThreshold As Double = 0.5
        'Const RespirThreshold As Double = 0
        Dim isbalanced As Boolean
        Dim iNumFound As Integer = 0

        'I am just altering the tolerance so that it can run faster; this needs deleting later
        'MessageBox.Show("the default tolerance = " & MonteCarlo.EcopathEETolerance)
        MonteCarlo.EcopathEETolerance = Me.MassBalanceTol
        'MonteCarlo.EcopathEETolerance = 0.05 'comment out and uncomment above line!!! this line just a test
        'Forces the same sequence of random numbers for each run. Used only for debugging runs
        'MonteCarlo.InitRandomSequence(666)

        'cMonteCarloManager.selectNewEcopathParameters() will alter the Ecopath Input parameters
        'We need to save the original state of Ecopath so it can be restored when we are done
        Me.SaveOriginalState()

        ' JS 20Jul14: Diet reading moved to cDiets class
#If 0 Then
        Dim reader As StreamReader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "DietComposition.csv"))
        If (reader Is Nothing) Then
            ' ToDo: report some kind of error
            Return
        End If

        'Read in the values from the DietComposition.csv into each array
        csv = New CsvReader(reader, True)
        If (reader IsNot Nothing) Then

            Try
                'For iPred As Integer = 1 To mCore.nLivingGroups
                '    For iPrey As Integer = 0 To mCore.nGroups
                '        If csv.ReadNextRecord() Then
                '            'Note about indices for interacts, lower and upper
                '            'The 1st index for predator runs from 0 and each element is equal to the same element+1 in mcore.ecopathgroupinputs
                '            'The 2nd index for prey runs from zero, where zero is the imports and then every other index is identical to mcore.ecopathgroupinputs
                '            Interacts(cStringUtils.ConvertToInteger(csv(2)) - 1, cStringUtils.ConvertToInteger(csv(3))) = cStringUtils.ConvertToInteger(csv(4))
                '            MeanProportions(cStringUtils.ConvertToInteger(csv(2)) - 1, cStringUtils.ConvertToInteger(csv(3))) = cStringUtils.ConvertToSingle(csv(5))
                '        Else
                '            ' ToDo_JS: handle error. Unexpected end in CSV file
                '        End If
                '    Next
                'Next
                While Not csv.EndOfStream
                    If csv.ReadNextRecord() Then
                        'Note about indices for interacts, lower and upper
                        'The 1st index for predator runs from 0 and each element is equal to the same element+1 in mcore.ecopathgroupinputs
                        'The 2nd index for prey runs from zero, where zero is the imports and then every other index is identical to mcore.ecopathgroupinputs
                        Interacts(cStringUtils.ConvertToInteger(csv(2)) - 1, cStringUtils.ConvertToInteger(csv(3))) = cStringUtils.ConvertToInteger(csv(4))
                        MeanProportions(cStringUtils.ConvertToInteger(csv(2)) - 1, cStringUtils.ConvertToInteger(csv(3))) = cStringUtils.ConvertToSingle(csv(5))
                    End If
                End While
            Catch ex As Exception
                ' ToDo_JS: handle error. Unexpected exception reading CSV file
            End Try
        Else
            ' ToDo_JS: Diets were not read; handle error
        End If
        csv.Dispose()
        cMSEUtils.ReleaseReader(reader)

        reader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "DietCompositionMultipliers.csv"))
        If (reader IsNot Nothing) Then
            'Read in the values from the DietCompositionMultipliers.csv
            csv = New CsvReader(reader, True)
            Try
                Do While Not csv.EndOfStream
                    If csv.ReadNextRecord() Then
                        DietPropMultipliers(cStringUtils.ConvertToInteger(csv(0)) - 1) = cStringUtils.ConvertToInteger(csv(2))
                    End If
                Loop
            Catch ex As Exception
                ' ToDo_JS: handle error. Unexpected exception reading CSV file
            End Try
            csv.Dispose()
            cMSEUtils.ReleaseReader(reader)
        Else
            ' ToDo_JS: Diets multipliers were not read; handle error
        End If
#End If

        'Calculate how many living groups that aren't primary producers
        'For i = 1 To mCore.nGroups
        '    If mCore.EcoPathGroupInputs(i).IsProducer Then nPPers += 1
        'Next i
        'nLivingMinusPPers = mCore.nLivingGroups - nPPers
        Try

            'Init some of the Monte Carlo parameters
            If Me.InitMonteCarloParameters() Then
                'Succeeded in intitializing Monte Carlo Parameters
                Dim iTrial As Integer = 1
                Dim bExpired As Boolean = False
                Dim sw As New Stopwatch()
                Dim lTimeout As Long = CLng(60 * 60 * 1000 * Me.NMaxTime)

                sw.Start()

                While (iTrial <= Me.NModels) And Not bExpired

                    'Set the Ecopath parameters using the Monte Carlo input parameters set above
                    TimeFindingBalanced.Start()
                    Dim i As Integer = 1
                    Dim bFound As Boolean = False

                    While (i <= Me.NMaxAttempts) And (Not bExpired) And (Not bFound)

                        isbalanced = True

                        'Has the user tried to stop the run
                        If Me.m_StopRun Then
                            'Yep
                            'HACK use the bExpired flag to stop the run
                            bExpired = True
                        End If

                        ' Provide occassional UI feedback
                        If (i = 1) Or ((i Mod 50) = 0) Then
                            cApplicationStatusNotifier.UpdateProgress(Me.Core, String.Format(My.Resources.STATUS_TRIAL_PROGRESS, My.Resources.CAPTION, iTrial, i), -1)
                        End If

                        'Write code here that generates a whole set of diet parameters to be used in combination with new ecopath parameters
                        'to be tested for the mass-balance criteria
                        Me.SampleDietMatrix(Me.m_diets.Interacts, Me.m_diets.InteractsImports, Me.m_diets.MeanProportions, _
                                            Me.m_diets.MeanProportionsImports, Me.m_diets.DietPropMultipliers)

                        'jb 23-Sept-2014 SampleDietMatrix(...) will truncate values less than a hardwired min (1.0E-6 inside SampleDietMatrix )
                        'If that happens we need to re-normalize the diet matrix
                        Me.NormalizeDiet(Me._pathdata.DCInput)
                        'Me.dumpDietMatrix()

                        Console.WriteLine("Iteration = " & i)
                        If MonteCarlo.selectNewEcopathParameters(1) Then

                            'For iGrp = 1 To mCore.nGroups
                            '    If mCore.EcoPathGroupInputs(iGrp).IsLiving Then
                            '        If _ecopath.EcopathData.GE(iGrp) > PQThreshold Or _ecopath.EcopathData.Resp(iGrp) < RespirThreshold Then
                            '            isbalanced = False
                            '        End If
                            '    End If
                            'Next

                            If isbalanced = True Then

                                'Output the diet matrix parameters to csv
                                Dim csv_dietout As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "DietMatrixTrial" & iTrial & ".csv"), False)
                                Try
                                    For iPrey = 0 To nGroups
                                        For iPred = 1 To m_core.nLivingGroups
                                            If iPred > 1 Then csv_dietout.Write(",")
                                            csv_dietout.Write(cStringUtils.FormatNumber(Me.m_ecopath.EcopathData.DC(iPred, iPrey)))
                                        Next
                                        csv_dietout.WriteLine()
                                    Next
                                Catch ex As Exception
                                    ' ToDo: respond to error
                                End Try
                                cMSEUtils.ReleaseWriter(csv_dietout)

                                ' JS 30Sep13: greatly simplified :)
                                WriteEcopathParms("b_out.csv", Me.m_ecopath.EcopathData.B)
                                WriteEcopathParms("ba_out.csv", Me.m_ecopath.EcopathData.BA)
                                WriteEcopathParms("pb_out.csv", Me.m_ecopath.EcopathData.PB)
                                WriteEcopathParms("qb_out.csv", Me.m_ecopath.EcopathData.QB)
                                WriteEcopathParms("ee_out.csv", Me.m_ecopath.EcopathData.EE)
                                ''This runs Ecosim without core support
                                'If Me.RunEcosim() Then
                                '    'dumps out some Ecosim results
                                '    Me.getEcosimResults()
                                'End If 'RunEcosim

                                'Me.InformUser(String.Format(My.Resources.STATUS_FOUND_MODEL, My.Resources.CAPTION, i), eMessageImportance.Information)
                                cLog.Write(String.Format(My.Resources.STATUS_FOUND_MODEL, My.Resources.CAPTION, i, sw.Elapsed.ToString()))

                                iNumFound += 1
                                bFound = True

                            End If
                        Else
                            System.Console.WriteLine("Failed to find balanced Ecopath model")
                        End If ' MonteCarlo.selectNewEcopathParameters()

                        i += 1

                        If (sw.ElapsedMilliseconds > lTimeout) Then
                            cLog.Write(String.Format("Cefas MSE time-out expired at iteration {0}, {1}", i, sw.Elapsed.ToString()))
                            bExpired = True
                        End If

                    End While

                    'Console.WriteLine("Number of seconds to run iteration: " & (TimeFindingBalanced.ElapsedMilliseconds / 1000).ToString)
                    TimeFindingBalanced.Reset()

                    iTrial += 1

                End While

                If bExpired And Not Me.m_StopRun Then
                    Me.InformUser("MSE expired after " & sw.Elapsed.ToString(), eMessageImportance.Information)
                End If

            End If 'Me.InitMonteCarloParameters()

            'Save the results to a .csv

        Catch ex As Exception

        End Try

        Me.RestoreOriginalState()

        ' Provide summary
        If iNumFound = 0 Then
            Me.InformUser(String.Format(My.Resources.STATUS_FINDMODELS_SUMMARY, My.Resources.CAPTION, iNumFound, nTrials), eMessageImportance.Warning)
        Else
            Me.InformUser(String.Format(My.Resources.STATUS_FINDMODELS_SUMMARY, My.Resources.CAPTION, iNumFound, nTrials), eMessageImportance.Information)
        End If

    End Sub


    Private Function getEcosimResults() As Boolean
        Try
            'Because we ran Ecosim directly from cEcosimModel.Run() instead of via the core cCore.RunEcosim()
            'the Core output objects cCore.EcoSimGroupOutputs() will not be populated
            'Instead get the Ecosim results directly from the underlying arrays
            Dim sumb() As Single
            ReDim sumb(m_core.nLivingGroups)
            For igrp As Integer = 1 To m_core.nLivingGroups
                'sum biomass over all the Ecosim timesteps
                For itime As Integer = 1 To m_core.nEcosimTimeSteps
                    'see cEcosimModel.PopulateResults() for how ResultsOverTime(var,group,time) are stored
                    sumb(igrp) += Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, itime)
                Next itime

                System.Console.WriteLine("Average Biomass for " & Me.m_ecopath.EcopathData.GroupName(igrp) & " = " & (sumb(igrp) / m_core.nEcosimTimeSteps).ToString)

            Next igrp

        Catch ex As Exception

        End Try

        Return Nothing

    End Function



    Private Function CalcFfromHCR(ByRef Biomass As Single, ByRef MinBiomass As Single, ByRef MaxBiomass As Single, ByRef FMax As Single) As Double

        If Biomass > MaxBiomass Then
            Return Convert.ToDouble(FMax)
        ElseIf Biomass < MinBiomass Then
            Return 0
        Else
            Return Convert.ToDouble(((Biomass - MinBiomass) / (MaxBiomass - MinBiomass)) * FMax)
        End If

    End Function

    Private Function DetermineZeroEffortFleets(ByRef FTargCons(,) As Double) As Integer()
        Dim ZeroEffortFleets As New List(Of Integer)

        For iGrp = 1 To m_core.nGroups
            If FTargCons(iGrp - 1, HCRType.Conservation) = 0 Then
                For iFleet = 1 To m_core.nFleets
                    If m_core.FleetInputs(iFleet).Landings(iGrp) + m_core.FleetInputs(iFleet).Discards(iGrp) > 0 And _
                        Not ZeroEffortFleets.Contains(iFleet) Then
                        ZeroEffortFleets.Add(iFleet)
                    End If
                Next
            End If
        Next
        Return ZeroEffortFleets.ToArray

    End Function

    Private Function DetermineQuotas(BiomassAtTimestep() As Single) As Double(,)
        '!!! At the moment we are only using the on column of TargConsQuota - the target one
        '!!! This is to simply the problem - it would have been difficult to include the regulation choices with both targ and cons
        '!!! at a later date we might want to change this to include conservation fs

        Dim TargConsQuota(m_core.nGroups - 1, 1) As Double

        'Calc the maximum decreases in the biomass


        'Initialise FTargetandConservation
        For i = 1 To m_core.nGroups
            TargConsQuota(i - 1, 0) = cEffortLimits.NoHCR_F
            TargConsQuota(i - 1, 1) = cEffortLimits.NoHCR_F
        Next

        For Each iHCRGroup In m_currentStrategy
            ' Determines the F for each group
            If TargConsQuota(iHCRGroup.GroupF.Index - 1, iHCRGroup.TypeOfHCR) = cEffortLimits.NoHCR_F Then
                TargConsQuota(iHCRGroup.GroupF.Index - 1, iHCRGroup.TypeOfHCR) = CalcFfromHCR(BiomassAtTimestep(iHCRGroup.GroupB.Index), 0, CSng(iHCRGroup.UpperLimit), CSng(iHCRGroup.MaxF)) * BiomassAtTimestep(iHCRGroup.GroupF.Index)
            Else
                Me.InformUser(String.Format(My.Resources.ERROR_HARVESTRUILE_DUPLICATE_F, iHCRGroup.GroupF.Name), eMessageImportance.Warning)
            End If
        Next

        Return TargConsQuota

    End Function



    'This is just a diagnostics routine that outputs to console the Biomass for each living group at a particular iteration
    Private Sub dumpEcopathParameters(ByVal iteration As Integer)
        Dim nliving As Integer = Me.m_core.nLivingGroups
        Dim MonteCarlo As cMonteCarloManager = Me.m_core.EcosimMonteCarlo

        System.Console.WriteLine("Iteration = " & iteration.ToString)
        For igrp = 1 To nliving
            Dim mcGrp As cMonteCarloGroup = MonteCarlo.Groups(igrp)
            System.Console.Write(mcGrp.Name & " = " & mcGrp.B & " , ")
            'Other parameters...  mcGrp.PB
        Next igrp
        System.Console.WriteLine()

    End Sub

    Private Function RunEcosim() As Boolean

        Try
            'increase the number of years for the projection
            ' mCore.EcoSimModelParameters.NumberYears = CInt(OriginalNTimesteps / _ecosim.EcosimData.NumStepsPerYear + NYearsProject)

            'make sure Ecosim computes the output data
            Me.m_ecosim.EcosimData.bTimestepOutput = True

            'No timestep call back
            Me.m_ecosim.TimeStepDelegate = Nothing

            'Run on the same thread 
            'this means Me._ecosim.Run() will block until Ecosim has finished running
            Me.m_ecosim.EcosimData.bMultiThreaded = False

            'Run Ecosim without Core support 
            'This means Core Input/ouput objects will not be populate 
            'So you can not use cCore.EcoSimGroupOutputs() to retrieve the results
            Me.m_ecosim.Init(True)
            Return Me.m_ecosim.Run()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RunEcosim() Exception: " & ex.Message)
        End Try

        Return False

    End Function


    Private Sub NormalizeDiet(ByRef DietMatrix(,) As Single)
        Dim dietsum As Single
        Dim tol As Single = 0.001
        Dim bwarning As Boolean = False

        For iPred = 1 To Me._pathdata.NumLiving
            bwarning = False
            If Me._pathdata.PP(iPred) < 1 Then
                dietsum = 0
                For iPrey = 0 To Me._pathdata.NumGroups
                    dietsum = dietsum + DietMatrix(iPred, iPrey)
                Next
                If dietsum <> 0 And Math.Abs(dietsum - 1) > tol Then
                    bwarning = True
                    For iPrey = 0 To Me._pathdata.NumGroups
                        DietMatrix(iPred, iPrey) = DietMatrix(iPred, iPrey) / dietsum
                    Next
                    'm_Data.DietsModified = True
                End If
            End If
        Next
        If bwarning Then
            System.Console.WriteLine("WARNING MSE Normalized Diet after sampling.")
        End If

    End Sub


    Public Sub CreateRCode()
        Dim writer As StreamWriter = New StreamWriter("C:\Users\Mark\Desktop\vbcreatedRcode.txt", False)
        Dim bSuccess As Boolean = False
        Dim firstpreyfound As Boolean
        Dim tempname As String

        For iPred = 1 To m_core.nLivingGroups
            tempname = m_core.EcoPathGroupInputs(iPred).Name
            tempname = tempname.Replace(" ", "")
            writer.Write("dietmeans[[" & iPred & "]] = c(")
            firstpreyfound = False
            For iPrey = 1 To m_core.nGroups
                If m_core.EcoPathGroupInputs(iPred).DietComp(iPrey) <> 0 Then
                    If firstpreyfound = False Then
                        writer.Write(m_core.EcoPathGroupInputs(iPred).DietComp(iPrey))
                        firstpreyfound = True
                    Else
                        writer.Write(", " & m_core.EcoPathGroupInputs(iPred).DietComp(iPrey))
                    End If
                End If
            Next
            writer.WriteLine(")")

            writer.Write("preynames[[" & iPred & "]] = c(")
            firstpreyfound = False
            For iPrey = 1 To m_core.nGroups
                If m_core.EcoPathGroupInputs(iPred).DietComp(iPrey) <> 0 Then
                    If firstpreyfound = False Then
                        writer.Write("""" & m_core.EcoPathGroupInputs(iPrey).Name & """")
                        firstpreyfound = True
                    Else
                        writer.Write(", """ & m_core.EcoPathGroupInputs(iPrey).Name & """")
                    End If
                End If
            Next
            writer.WriteLine(")")

        Next

        writer.Write("prednames = c(")
        For iPred = 1 To m_core.nLivingGroups
            If iPred = 1 Then
                writer.Write("""" & m_core.EcoPathGroupInputs(iPred).Name & """")
            Else
                writer.Write(", """ & m_core.EcoPathGroupInputs(iPred).Name & """")
            End If
        Next
        writer.WriteLine(")")

        cMSEUtils.ReleaseWriter(writer)

    End Sub



#End Region 'Private modeling code

#Region "Distributions and sampling code"

    Public Function DirichletSample2(ByVal nDimensions As Integer, ByVal alpha() As Single, ByRef DietMultiplier As Double) As Single()
        Dim gamma(nDimensions - 1) As Single
        Dim dirichlet(nDimensions - 1) As Single
        Dim sumofgamma As Single
        Dim GammaGenerator As New GammaDistribution

        For i = 0 To alpha.Length() - 1
            'alpha(i) = alpha(i) * TempDietMultiplier
            alpha(i) = CSng(alpha(i) * DietMultiplier)
        Next

        For i As Integer = 0 To nDimensions - 1
            GammaGenerator.Alpha = alpha(i)
            gamma(i) = CSng(GammaGenerator.NextDouble())
        Next

        sumofgamma = gamma.Sum()
        For i = 0 To nDimensions - 1
            dirichlet(i) = gamma(i) / sumofgamma
        Next

        Return (dirichlet)

    End Function

    '    Private Function DirichletSample(ByVal nDimensions As Integer, ByVal a() As Single, ByRef DietMultiplier As Double) As Single()
    '        Dim u1, u2, b, p, x As Single
    '        Dim gamma(nDimensions - 1) As Single
    '        Dim dirichlet(nDimensions - 1) As Single
    '        Dim sumofgamma As Single

    '        For i = 0 To a.Length() - 1
    '            a(i) = CSng(a(i) * DietMultiplier)
    '        Next

    '        For i As Integer = 0 To nDimensions - 1
    'step1:
    '            u1 = CSng(Me.m_rand.NextDouble())
    '            b = CSng((Math.E + a(i)) / Math.E)
    '            p = b * u1
    '            If p >= 1 Then GoTo step3
    'step2:
    '            x = CSng(p ^ (1 / a(i)))
    '            u2 = CSng(Me.m_rand.NextDouble())
    '            If u2 > Math.Exp(-x) Then
    '                GoTo step1
    '            Else
    '                gamma(i) = x
    '                GoTo stepend
    '            End If
    'step3:
    '            x = CSng(-Math.Log((b - p) / a(i)))
    '            u2 = CSng(Me.m_rand.NextDouble())
    '            If u2 > x ^ (a(i) - 1) Then
    '                GoTo step1
    '            Else
    '                gamma(i) = x
    '            End If
    'stepend:
    '        Next

    '        sumofgamma = gamma.Sum()
    '        For i = 0 To nDimensions - 1
    '            dirichlet(i) = gamma(i) / sumofgamma
    '        Next

    '        Return (dirichlet)

    '    End Function

    Private Function UniformSample(ByVal min_par As Single, ByVal max_par As Single) As Double

        Return (min_par + Me.m_rand.NextDouble() * (max_par - min_par))

    End Function

    Private Function TriangularSample(ByVal A_par As Single, ByVal B_par As Single, ByVal C_par As Single) As Double

        Dim U As Double = Me.m_rand.NextDouble()
        If U < ((C_par - A_par) / (B_par - A_par)) Then
            Return A_par + Math.Sqrt(U * (B_par - A_par) * (C_par - A_par))
        Else
            Return B_par - Math.Sqrt((1 - U) * (B_par - A_par) * (B_par - C_par))
        End If

    End Function

    Private Sub SampleDietMatrix(ByVal Interacts(,) As Integer, ByVal InteractsImports() As Integer, ByVal MeanProportions(,) As Single, ByVal MeanProportionsImports() As Single, ByVal DietPropMultipliers() As Double)
        Dim MeanPropMod() As Single
        Dim SumInteractions(m_core.nLivingGroups - 1) As Single
        Dim TempDirichlet() As Single
        Dim DirichStopWatch As New Stopwatch
        Dim NormaliseStopWatch As New Stopwatch
        Dim EcopathStopWatch As New Stopwatch
        Dim EcopathInternalStopWatch As New Stopwatch
        Dim ecopathData As cEcopathDataStructures = Me.m_ecopath.EcopathData
        Dim iPointer As Integer = 0
        Dim MinProp As Single = 0.000001

        'Dim DirichletArray(mCore.nLivingGroups - 1, mCore.nLivingGroups - 1) As Single

        'Array.Clear(DirichletArray, 0, DirichletArray.GetLength(1))

        'Generate a vector 'SumInteractions' that counts how many prey each predator has
        For iPred As Integer = 0 To m_core.nLivingGroups - 1
            SumInteractions(iPred) += InteractsImports(iPred)
            For iPrey As Integer = 0 To m_core.nGroups - 1
                SumInteractions(iPred) += Interacts(iPred, iPrey)
            Next
        Next

        For iPred As Integer = 0 To m_core.nLivingGroups - 1
            'mCore.EcoPathGroupInputs(iPred + 1).DietComp(0) = 0
            If (SumInteractions(iPred) = 0) Then    'No need to do any of this unless there is at least 1 prey for this parameter
                'Set all values to zero - if running slow might want to consider how this could be skipped - possibly setting whole array to zero at start
                For iPrey = 0 To m_core.nGroups
                    ecopathData.DCInput(iPred + 1, iPrey) = 0
                Next
            Else
                ' DirichStopWatch.Start()

                ReDim MeanPropMod(CInt(SumInteractions(iPred) - 1))
                iPointer = 0
                If InteractsImports(iPred) = 1 Then
                    MeanPropMod(iPointer) = MeanProportionsImports(iPred)
                    iPointer += 1
                End If
                For iPrey = 0 To m_core.nGroups - 1
                    If Interacts(iPred, iPrey) = 1 Then
                        MeanPropMod(iPointer) = MeanProportions(iPred, iPrey)
                        iPointer += 1
                    End If
                Next iPrey

                'Samples a set of Dirichlet distributed parameters
                TempDirichlet = DirichletSample2(CInt(SumInteractions(iPred)), MeanPropMod, DietPropMultipliers(iPred))


                Dim i As Integer = 0
                Dim dProp As Single
                If InteractsImports(iPred) = 1 Then
                    dProp = TempDirichlet(i)
                    If dProp < MinProp Then dProp = 0.0F
                    ecopathData.DCInput(iPred + 1, 0) = dProp
                    i += 1
                End If 'InteractsImports(iPred) = 1

                For iPrey = 1 To m_core.nGroups
                    If Interacts(iPred, iPrey - 1) = 1 Then
                        dProp = TempDirichlet(i)
                        If dProp < MinProp Then dProp = 0.0F
                        ecopathData.DCInput(iPred + 1, iPrey) = dProp
                        i += 1
                    End If 'Interacts(iPred, iPrey - 1) = 1
                Next iPrey

            End If '(SumInteractions(iPred) = 0)
        Next iPred

    End Sub



#End Region 'Distributions and sampling code

#Region "EwE Events onEcosimInitialized()..."

    Public Sub onEcosimInitialized(ByVal EcosimDatastructures As cEcosimDatastructures)

        Me._simdata = EcosimDatastructures

        Me.m_regulations = New cRegulations(Me, Me.m_core)
        Me.m_quotashares = New cQuotaShares(Me, Me.m_core)
        Me.m_survivability = New cSurvivability(Me, Me.m_core, EcosimDatastructures, _pathdata)
        Me.m_strategies = New Strategies(Me, Me.m_core)
        Me.m_effortlimits = New cEffortLimits(Me, Me.m_core)
        Me.m_StockAssessment = New cStockAssessmentModel(Me)
        Me.m_diets = New cDiets(Me, Me.m_core)

    End Sub

    Public Sub onEcosimRunBeginning(ByVal EcosimDatastructures As cEcosimDatastructures)

        Try
            Me.StockAssessment.InitForRun()
        Catch ex As Exception

        End Try

    End Sub


    Public Sub onEcopathInitialized(ByVal EcopathData As cEcopathDataStructures)
        Me._pathdata = EcopathData
    End Sub


    Public Sub onEcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal iTime As Integer)
        ' JS 13Oct13: Fixed CurDir vulnerability in lpsolve
        ' JS 13Oct13: Globalized this method
        ' JS 13Oct13: Fixed path usage
        ' JS 13Oct13: Removed MsgBox

        'Must have nfleets+1 elements so for 10 fleets needs elements 0-10
        'This is because of the way code works in EwE
        Dim TargetF(m_core.nGroups) As Double
        'Dim CostFunctionType(mCore.nGroups) As String
        'Dim mincost As Double = 1000000

        Dim QMult(m_ecosim.EcosimData.nGroups) As Double
        'Dim tempFConservation As Double
        'used so that we don't repeat same groups when cycling through HCRs
        'Dim LastYearsEffort(m_ecosim.EcopathData.NumFleet - 1) As Double
        'Dim variable_results() As Double

        'Dim TargConsQuota(mCore.nGroups - 1, 1) As Double 'Stores the target and conservation f's for each species
        Dim Elim As Single 'the maximum effort that can be exerted without causing discards
        Dim Emax As Single 'the effort that will catch the entire quota of the most valuable species
        Dim iCatch As Single


        If ChangeEffortFlag = True And iTime > OriginalNTimesteps Then 'Flag is only set to true when the button on the form is clicked
            'this is so that its only executed when ecosim is run from mseform

            If (iTime - 1) Mod 12 = 0 Then

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Stock Assessment
                'Get Biomass estimated by the stock assessment model
                Dim bioEst() As Single = Me.StockAssessment.DoAnnualStockAssessment(Me.m_currentStrategy, iTime, BiomassAtTimestep)

                'OK Hook the stock assessment model up to the Quota setting
                'Use the biomass estimated by the stock assessment model as the true biomass to set the Quota
                TargConsQuota = DetermineQuotas(bioEst)

                'DON'T use stock assessment model 
                'Compute the quota on the TRUE(Ecosim) biomass
                'TargConsQuota = DetermineQuotas(BiomassAtTimestep)
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                For iFleet = 1 To m_core.nFleets
                    MinEffortThisYear(iFleet - 1) = m_ecosim.EcosimData.FishRateGear(iFleet, iTime - 1) * (1 - m_effortlimits.Value(iFleet))
                    If m_effortlimits.Value(iFleet) = cCore.NULL_VALUE Then MinEffortThisYear(iFleet - 1) = 0
                Next

            End If

            'if there are no fleets to optimise for skip all this
            If FleetsThatFishHCRGrp.Count > 0 Then

                'jb QMult() is Density dependant catchability
                'Not quite sure what QMult is but it is needed to calculate what F is in the optimised routine
                For indexgrp As Integer = 1 To m_ecosim.EcosimData.nGroups
                    QMult(indexgrp - 1) = m_ecosim.EcosimData.QmQo(indexgrp) / (1 + (m_ecosim.EcosimData.QmQo(indexgrp) - 1) * BiomassAtTimestep(indexgrp) / m_ecosim.EcosimData.StartBiomass(indexgrp))
                Next

                For Each iFleet In FleetsThatFishHCRGrp
                    Select Case m_currentStrategy.Regulations.Method(iFleet)
                        Case cRegulations.eRegMethod.HighestValue, cRegulations.eRegMethod.SelectiveFishing
                            'Find out the highest value species
                            'Calculate the effort that would catch all quota of highest value species
                            'Set it for this fleet
                            'If selective
                            'Calculate what selectivity would prevent any other stock going over quota
                            'Set selectivity variables

                            'find the stock with the biggest economic value that the given fleet catches
                            Dim vmax As Single = 0
                            Dim imax As Integer = 0
                            Dim v As Single
                            For iGrp = 1 To m_ecopath.EcopathData.NumGroups
                                If (m_ecopath.EcopathData.Landing(iFleet, iGrp)) > 0 And TargConsQuota(iGrp - 1, 0) > 0 Then
                                    v = CSng(m_quotashares.ReadiFleetiGroupQuotaShare(iFleet, iGrp).mShare * TargConsQuota(iGrp - 1, 0) * m_ecopath.EcopathData.Market(iFleet, iGrp))
                                    If v > vmax Then
                                        vmax = v
                                        imax = iGrp
                                    End If
                                End If
                            Next iGrp

                            'get the effort limit for the stock with the biggest value
                            Emax = 0
                            Emax = CSng((m_quotashares.ReadiFleetiGroupQuotaShare(iFleet, imax).mShare * TargConsQuota(imax - 1, 0)) / (1.0E-20 + QMult(imax) * m_ecosim.EcosimData.FishMGear(iFleet, imax) * BiomassAtTimestep(imax)))

                            'Check whether the calculated effort is less than the max decrease and if it is set it to the max decrease
                            If Emax < MinEffortThisYear(iFleet - 1) Then
                                Emax = MinEffortThisYear(iFleet - 1)
                            End If

                            'Limit the effort if it is greater than the max allowable 
                            If Emax < m_ecosim.EcosimData.FishRateGear(iFleet, iTime) Then m_ecosim.EcosimData.FishRateGear(iFleet, iTime) = Emax

                            'Alters the discard parameters 
                            For iGrp = 1 To m_ecopath.EcopathData.NumGroups
                                If (m_ecopath.EcopathData.Landing(iFleet, iGrp)) > 0 And TargConsQuota(iGrp - 1, 0) <> cCore.NULL_VALUE Then
                                    'get the total catch at this effort
                                    iCatch = CSng(m_ecosim.EcosimData.FishRateGear(iFleet, iTime) * QMult(iGrp) * m_ecosim.EcosimData.FishMGear(iFleet, iGrp) * BiomassAtTimestep(iGrp))

                                    'if the total catch exceeds the quota figure out what do do with the discards
                                    If iCatch > m_quotashares.ReadiFleetiGroupQuotaShare(iFleet, iGrp).mShare * TargConsQuota(iGrp - 1, 0) Then
                                        'fishing mortality exceeds quota
                                        m_ecosim.EcosimData.PropLandedTime(iFleet, iGrp) = CSng((m_quotashares.ReadiFleetiGroupQuotaShare(iFleet, iGrp).mShare * TargConsQuota(iGrp - 1, 0)) / (iCatch + 1.0E-20))
                                        If m_regulations.Method(iFleet) = cRegulations.eRegMethod.HighestValue Then
                                            'QuotaType = Strongest
                                            'excess catch discarded and included in the fishing mortality()
                                            m_ecosim.EcosimData.Propdiscardtime(iFleet, iGrp) = (1 - m_ecosim.EcosimData.PropLandedTime(iFleet, iGrp)) * m_ecopath.EcopathData.PropDiscardMort(iFleet, iGrp)
                                        Else
                                            'QuotaType = Selective 
                                            'excess catch is NOT included in fishing mortality all discards survive
                                            m_ecosim.EcosimData.Propdiscardtime(iFleet, iGrp) = 0
                                        End If

                                    Else
                                        'iCatch < Quota
                                        m_ecosim.EcosimData.PropLandedTime(iFleet, iGrp) = m_ecopath.EcopathData.PropLanded(iFleet, iGrp)
                                        m_ecosim.EcosimData.Propdiscardtime(iFleet, iGrp) = m_ecopath.EcopathData.PropDiscard(iFleet, iGrp)
                                    End If

                                End If
                            Next iGrp
                        Case cRegulations.eRegMethod.WeakestStock
                            'Find the weakest stock
                            'Calculate effort that would catch all weakest stock quota
                            'Set it for this fleet
                            For iGrp = 1 To m_ecopath.EcopathData.NumGroups
                                If (m_ecopath.EcopathData.Landing(iFleet, iGrp) + m_ecopath.EcopathData.Discard(iFleet, iGrp)) > 0 And TargConsQuota(iGrp - 1, 0) <> cEffortLimits.NoHCR_F Then
                                    'Calculate the effort limitation, has quota been exceeded?
                                    'QYear is omitted from following equation because it is assumed that technological creep is zero
                                    Elim = CSng((m_quotashares.ReadiFleetiGroupQuotaShare(iFleet, iGrp).mShare * TargConsQuota(iGrp - 1, 0)) / (1.0E-20 + QMult(iGrp) * _simdata.FishMGear(iFleet, iGrp) * BiomassAtTimestep(iGrp)))
                                    'Check whether the calculated effort is less than the max decrease and if it is set it to the max decrease
                                    If Elim < MinEffortThisYear(iFleet - 1) Then
                                        Elim = MinEffortThisYear(iFleet - 1)
                                    End If
                                    Debug.Assert(Elim >= 0)
                                    If _simdata.FishRateGear(iFleet, iTime) > Elim Then
                                        _simdata.FishRateGear(iFleet, iTime) = Elim
                                    End If
                                End If
                            Next iGrp
                        Case cRegulations.eRegMethod.None
                            _simdata.FishRateGear(iFleet, iTime) = _simdata.FishRateGear(iFleet, iTime - 1)
                    End Select
                Next

            End If

            'This sets the effort for any fleet that does not have a HCR which affects it to the effort as it was in the previous timestep
            For iFleet = 1 To m_core.nFleets
                If FleetsThatFishHCRGrp.IndexOf(iFleet) = -1 Then
                    _simdata.FishRateGear(iFleet, iTime) = _simdata.FishRateGear(iFleet, iTime - 1)
                Else

                    'Make sure the fleet is regulated if we are going add error to Effort Implementation
                    If Me.m_currentStrategy.Regulations.Method(iFleet) <> cRegulations.eRegMethod.None Then
                        'Add uncertainty to the regulated Effort 
                        'This is implementation error. The Effort actually achieved by the Fleet.
                        'The implementation error is not really the business of the stock assessment model
                        'It is part of the stock assessment for practical reasons 
                        '   CV can be included in the interface
                        '   Random number generator needs to be seeded at the same time as the stock assessment model
                        _simdata.FishRateGear(iFleet, iTime) = _simdata.FishRateGear(iFleet, iTime) * Me.StockAssessment.getImplementationError(iFleet)

                    End If 'Me.m_currentStrategy.Regulations.Method(iFleet) <> cRegulations.eRegMethod.None

                End If
            Next

            'Calculates what the F's are for each species given the effort
            m_ecosim.SetFtimeFromGear(Nothing, iTime, TechnologyCreep, True)

        End If

    End Sub

#End Region 'EwE Events onEcosimInitialized()...

#Region " Helper methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a name and index to a <see cref="cFleetInput"/> instance.
    ''' </summary>
    ''' <param name="strName">The name to resolve.</param>
    ''' <param name="iIndex">The index to resolve.</param>
    ''' <returns>A <see cref="cFleetInput"/> instance, or Nothing if
    ''' the index or name did not match any of the present fleets.</returns>
    ''' <remarks>Note that name comparison is not case sensitive.</remarks>
    ''' -----------------------------------------------------------------------
    Private Function ResolveFleet(strName As String, iIndex As Integer) As cFleetInput
        If (iIndex < 1) Or (iIndex > Me.Core.nFleets) Then Return Nothing
        Dim flt As cFleetInput = Me.Core.FleetInputs(iIndex)
        If String.Compare(flt.Name, strName, True) <> 0 Then
            Return Nothing
        End If
        Return flt
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Notify the user of an event.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="importance"></param>
    ''' <param name="strHyperlink"></param>
    ''' -----------------------------------------------------------------------
    Friend Sub InformUser(strMessage As String, importance As eMessageImportance, _
                          Optional strHyperlink As String = "", _
                          Optional astrSubMessages As String() = Nothing)

        If (Me.Core Is Nothing) Then Return

        Dim msg As New cMessage(String.Format(ScientificInterfaceShared.My.Resources.GENERIC_LABEL_INDEXED, My.Resources.CAPTION, strMessage), _
                                eMessageType.Any, eCoreComponentType.External, importance)
        msg.Hyperlink = strHyperlink
        If (astrSubMessages IsNot Nothing) Then
            For Each strSubMessage As String In astrSubMessages
                msg.AddVariable(New cVariableStatus(eStatusFlags.OK, strSubMessage, eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.External, 0))
            Next
        End If
        Me.Core.Messages.SendMessage(msg)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ask the user a question.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="style"></param>
    ''' <param name="importance"></param>
    ''' <param name="replyDefault"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Friend Function AskUser(strMessage As String, _
                            style As eMessageReplyStyle, _
                            Optional importance As eMessageImportance = eMessageImportance.Question, _
                            Optional replyDefault As eMessageReply = eMessageReply.OK) As eMessageReply

        If (Me.Core Is Nothing) Then Return replyDefault

        Dim fmsg As New cFeedbackMessage(String.Format(ScientificInterfaceShared.My.Resources.GENERIC_LABEL_INDEXED, My.Resources.CAPTION, strMessage), _
                                         eCoreComponentType.External, eMessageType.Any, importance, style)
        fmsg.Reply = replyDefault
        Me.Core.Messages.SendMessage(fmsg)
        Return fmsg.Reply

    End Function

    Private Sub OnCoreMessage(ByRef msg As cMessage)

        Dim bRefresh As Boolean = False

        ' Refresh when Core settings have changed
        If (msg.Type = eMessageType.GlobalSettingsChanged) Then
            bRefresh = True
        End If

        ' Refresh upon ecosim scenario load
        If (msg.Type = eMessageType.DataAddedOrRemoved And msg.Source = eCoreComponentType.EcoSim) Then
            bRefresh = True
        End If

        If (bRefresh = True) Then
            Me.InvalidateConfigurationState()
        End If

    End Sub


#End Region ' Helper methods

#Region " Configurable settings "

    Public Property NModels2Run As Integer
        Get
            'Return Math.Max(1, Math.Min(My.Settings.NModels2Run, 100))
            Return My.Settings.NModels2Run
        End Get
        Set(value As Integer)
            If (value <> My.Settings.NModels2Run) Then
                My.Settings.NModels2Run = value
                My.Settings.Save()
            End If
        End Set
    End Property

    Public Property NModels As Integer
        Get
            'Return Math.Max(1, Math.Min(My.Settings.NTrials, 100))
            Return My.Settings.NTrials
        End Get
        Set(value As Integer)
            If (value <> My.Settings.NTrials) Then
                My.Settings.NTrials = value
                My.Settings.Save()
            End If
        End Set
    End Property

    Public Property NYearsProject As Integer
        Get
            Return Math.Max(1, Math.Min(My.Settings.NYearsProject, 1000))
        End Get
        Set(value As Integer)
            If (value <> My.Settings.NYearsProject) Then
                My.Settings.NYearsProject = value
                My.Settings.Save()
            End If
        End Set
    End Property

    Public Property MassBalanceTol As Single
        Get
            Return Math.Max(0.0!, Math.Min(My.Settings.MassBalanceTol, 0.1!))
        End Get
        Set(value As Single)
            If (value <> My.Settings.MassBalanceTol) Then
                My.Settings.MassBalanceTol = value
                My.Settings.Save()
            End If
        End Set
    End Property

    Public Property UseEwEPath As Boolean
        Get
            Return My.Settings.UseEwEPath
        End Get
        Set(value As Boolean)
            If (value <> My.Settings.UseEwEPath) Then
                My.Settings.UseEwEPath = value
                My.Settings.Save()
                Me.InvalidateConfigurationState(True)
            End If
        End Set
    End Property

    Public Property CustomPath As String
        Get
            Dim strPath As String = My.Settings.CustomPath
            If (String.IsNullOrWhiteSpace(strPath)) Then
                Return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            End If
            Return strPath
        End Get
        Set(value As String)
            If (value <> My.Settings.CustomPath) Then
                My.Settings.CustomPath = value
                My.Settings.Save()
                Me.InvalidateConfigurationState(True)
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the max # of trials for finding a balanced model
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NMaxAttempts As Integer
        Get
            Return My.Settings.NMaxAttempts
        End Get
        Set(value As Integer)
            If (value <> My.Settings.NMaxAttempts) Then
                My.Settings.NMaxAttempts = value
                My.Settings.Save()
                'Me.InvalidateConfigurationState()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the max time for finding a balanced model (in fractions of hours)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NMaxTime As Single
        Get
            Return My.Settings.NMaxTime
        End Get
        Set(value As Single)
            If (value <> My.Settings.NMaxTime) Then
                My.Settings.NMaxTime = value
                My.Settings.Save()
                'Me.InvalidateConfigurationState()
            End If
        End Set
    End Property

#End Region ' Configurable settings

#Region "Dead Code"

#If 0 Then

    'JB 3-April-2014 
    'Code used to create, save and reload a dietmatrix file for debugging

    ''' <summary>
    ''' Reads a known dietmatrix file
    ''' </summary>
    ''' <param name="iTrial"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function readDietMatrix(ByVal iTrial As Integer) As Boolean
        Dim buff As String
        Dim data() As String
        Dim strmReader As StreamReader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "DietMatrix.csv"))

        If (strmReader IsNot Nothing) Then

            For iprey As Integer = 1 To mCore.nGroups
                buff = strmReader.ReadLine
                data = buff.Split(","c)
                Debug.Assert(data.Length = mCore.nLivingGroups)
                For ipred As Integer = 1 To mCore.nLivingGroups
                    If _ecopath.EcopathData.DC(ipred, iprey) > 0 And cStringUtils.ConvertToSingle(data(ipred - 1)) > 0 Then
                        _ecopath.EcopathData.DC(ipred, iprey) = cStringUtils.ConvertToSingle(data(ipred - 1))
                    End If
                Next ipred

            Next iprey
        End If

        Me.NormalizeDiet()

        cMSEUtils.ReleaseReader(strmReader)
        Me.dumpDietMatrix()

        Return True

    End Function

    ''' <summary>
    ''' Sample the diet matrix
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub updateDietRandom()

        Dim rnd As New Random(666)
        Dim dist As New Troschuetz.Random.NormalDistribution

        For iPred = 1 To Me._pathdata.NumLiving
            If Me._pathdata.PP(iPred) < 1 Then
                For iPrey = 0 To Me._pathdata.NumGroups
                    If Me._pathdata.DC(iPred, iPrey) > 0 Then
                        dist.Mu = Me._pathdata.DC(iPred, iPrey)
                        dist.Sigma = dist.Mu * 0.1
                        Me._pathdata.DC(iPred, iPrey) = CSng(dist.NextDouble)
                    End If
                Next
            End If
        Next

        Me.NormalizeDiet()

        Me.dumpDietMatrix()

    End Sub

#End If

    'Commented out because redundant 3-9-13

    'Public Sub Create2DimParams(ByVal ParamName As String)
    '    Dim sPath As String = DataPath & "\DistributionParameters"
    '    Dim csv = New CsvReader(New StreamReader(sPath & "\" & ParamName & ".csv"), True)
    '    Dim ParameterArray(mCore.nLivingGroups * mCore.nLivingGroups, 5) As Single
    '    Dim nIterations As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)
    '    Dim SampledParameters(nIterations, mCore.nLivingGroups, mCore.nLivingGroups)
    '    Dim eDistributionType As DistributionType

    '    For iGroup = 1 To mCore.nLivingGroups * mCore.nLivingGroups
    '        csv.ReadNextRecord()
    '        For iField = 1 To 5
    '            ParameterArray(iGroup - 1, iField) = csv(iField)
    '        Next
    '    Next

    '    'Generate an array of sample parameters
    '    For iGroup = 1 To mCore.nLivingGroups
    '        For jGroup = 1 To mCore.nLivingGroups
    '            eDistributionType = ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 1)
    '            For iIteration = 1 To nIterations

    '                Select Case eDistributionType
    '                    Case DistributionType.Uniform
    '                        SampledParameters(iIteration - 1, iGroup, jGroup) = UniformSample(ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 2), ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 3))

    '                    Case DistributionType.Triangular
    '                        SampledParameters(iIteration - 1, iGroup, jGroup) = TriangularSample(ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 2), ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 3), ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 4))
    '                End Select

    '            Next
    '        Next
    '    Next

    '    For iIteration = 1 To nIterations
    '        'Output the sampled parameters to a csv
    '        sPath = DataPath & "\ParametersOut"
    '        Dim csvout As New StreamWriter(Path.Combine(sPath & "\" & ParamName & ToString(iIteration) & "out.csv"), True)

    '        For igrp As Integer = 1 To mCore.nLivingGroups
    '            csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
    '        Next
    '        csvout.WriteLine()

    '        For jGroup = 1 To mCore.nLivingGroups
    '            For iGroup = 1 To mCore.nLivingGroups
    '                csvout.Write("," & SampledParameters(iIteration - 1, iGroup - 1, jGroup - 1))
    '            Next
    '            csvout.WriteLine()
    '        Next

    '        csvout.Dispose()
    '    Next




    'End Sub


    'Private Sub CalculateFError(ByRef eps() As Double)
    '    Dim Fopt(mCore.nGroups - 1) As Double
    '    For iGrp As Integer = 1 To mCore.nLivingGroups
    '        Fopt(iGrp - 1) = 0
    '        For iFleet As Integer 1 to mCore.nFleets
    '            Fopt(iGrp - 1) = Fopt(iGrp - 1) + (mCore.FleetInputs(iFleet).Landings(iGrp) + mCore.FleetInputs(iFleet).Discards(iGrp)) * eps(iGrp - 1)
    '        Next
    '    Next
    'End Sub
#End Region

End Class
