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
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimBeginTimestepPlugin
    Implements EwEPlugin.IMessageFilterPlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcosimPlugin

#Region " Internal vars "

    Friend Strategies As New Strategies
    Private CurrentStrategy As Strategy

    Private MSEForm As frmMSE = Nothing
    Private mCore As cCore = Nothing
    Private m_uic As cUIContext = Nothing
    Private _ecosim As EwECore.Ecosim.cEcoSimModel = Nothing
    Private _simdata As cEcosimDatastructures
    Private _ecopath As Ecopath.cEcoPathModel
    Private _EcosimTimeStepDelegate As EwECore.Ecosim.EcoSimTimeStepDelegate
    Private StrategyIndex As Integer
    Private OriginalNTimesteps As Integer
    Private ChangeInEffortLimits() As Double
    Public Const NoHCR_F As Integer = cCore.NULL_VALUE

    Private m_Survivability As New cSurvivability

    Private m_monitor As New cMSEStateMonitor(Me)

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

    Public Enum DistributionType As Integer
        Uniform = 1
        Triangular
    End Enum

    Private m_mhSettings As cMessageHandler = Nothing
    Private m_mhEcosim As cMessageHandler = Nothing

#End Region ' Internal vars

#Region " Construction "

    Public Sub New()
        Me.InvalidateConfiguration()
    End Sub

#End Region ' Construction

#Region " Diagnostics and state management "

    Friend Sub InvalidateConfiguration()

        Me.m_iNumStrategiesAvailable = cCore.NULL_VALUE
        Me.m_iNumModelsAvailable = cCore.NULL_VALUE
        Me.m_tsInputDataCompatibility = TriState.UseDefault

        Me.m_monitor.Invalidate()

        If Me.HasUI Then
            MSEForm.UpdateState()
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cMSEStateMonitor">MSE state monitor</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Controller As cMSEStateMonitor
        Get
            Return Me.m_monitor
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the MSE plug-in has all directories that it needs in the
    ''' <see cref="DataPath"></see>.
    ''' </summary>
    ''' <param name="bCreate">Flag, indicating whether the directory structure
    ''' should be created if missing.</param>
    ''' <returns>True if the directory structure exists in its entirety.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsInputStructureAvailable(bCreate As Boolean) As Boolean

        ' Make sure plug-in has all dirs
        Dim strPath As String = Me.DataPath
        Dim bSuccess As Boolean = True

        For Each f As cMSEUtils.eMSEPaths In [Enum].GetValues(GetType(cMSEUtils.eMSEPaths))
            bSuccess = bSuccess And cFileUtils.IsDirectoryAvailable(cMSEUtils.MSEFolder(strPath, f), bCreate)
        Next
        Return bSuccess

    End Function

    Private m_tsInputDataCompatibility As TriState = TriState.UseDefault

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether all base data is available for building models
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function IsInputDataCompatible() As Boolean

        ' Would it not be nice if these file names were represented by enums as well?

        Dim aFilesEcopath As String() = New String() {"B_Dist", "BA_Dist", "PB_Dist", "QB_Dist", "EE_Dist"}
        Dim aFilesEcosim As String() = New String() {"DenDepCatchability", "SwitchingPower", "QBMaxxQBio", "PredEffectFeedingTime", "OtherMortFeedingTime", "MaxRelFeedingTime", "FeedingTimeAdjustRate"}
        Dim strRoot As String = cMSEUtils.MSEFolder(Me.DataPath, cMSEUtils.eMSEPaths.DistrParams)

        If (Me.m_tsInputDataCompatibility = TriState.UseDefault) Then

            ' Hope for the best
            Me.m_tsInputDataCompatibility = TriState.True

            ' Make sure plug-in has empty CSV
            If Not File.Exists(cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.DistrParams, "DietComposition.csv")) Then
                Me.m_tsInputDataCompatibility = TriState.False
            Else
                ' Assess Ecopath files
                For Each strFile As String In aFilesEcopath
                    If Not CheckEcopathDistributionFilesOkay(cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.DistrParams, strFile & ".csv")) Then
                        Me.m_tsInputDataCompatibility = TriState.False
                        Exit For
                    End If
                Next strFile

                ' Assess Ecopath files
                For Each strFile As String In aFilesEcosim
                    If Not CheckEcoSimDistributionFilesOkay(cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.DistrParams, strFile & ".csv")) Then
                        Me.m_tsInputDataCompatibility = TriState.False
                        Exit For
                    End If
                Next strFile

            End If
        End If

        Return (Me.m_tsInputDataCompatibility = TriState.True)

    End Function

    ''' <summary>
    ''' Checks whether each of the Ecopath (not diet matrix) distribution files is has the correct functional groups in it
    ''' They should only have living groups
    ''' It does this by saving a true in the position of an array at the index at which it exists in EwE
    ''' It then sums the values in this array and checks that they are equal to nlivinggroups (TRUE=1)
    ''' The reason I have done this is to prevent the problem where a file might have replicate groups
    ''' If a file has replicate groups and we check each group to see if it is in EwE and it happens that the number
    ''' of groups in the file are equal to the number of living groups, the file will be wrongly accepted.
    ''' </summary>
    ''' <param name="strPath"></param>
    ''' <returns>True if all ok.</returns>
    Public Function CheckEcopathDistributionFilesOkay(ByVal strPath As String) As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim correct(mCore.nGroups - 1) As Integer
        Dim TotalFound As Integer = 0
        Dim bOK As Boolean = True

        reader = cMSEUtils.GetReader(strPath)
        If (reader Is Nothing) Then Return False

        ' Initialise correct to all zeros
        For i = 1 To mCore.nGroups
            correct(i - 1) = 0
        Next

        csv = New CsvReader(reader, True)
        Try
            'cycle through each of the living functional groups each time checking if it exists in the file
            ' JS 13Oct13: Changed the looping structure here. If csvreader fails to load a record it will repeat the last record!
            '             This created double-counting when a CSV file did not contain enough records
            While Not csv.EndOfStream
                If csv.ReadNextRecord() Then
                    For xgrp = 1 To mCore.nGroups
                        If (cStringUtils.ConvertToInteger(csv(0)) = xgrp) And (String.Compare(cMSEUtils.FromCSVField(csv(1)), _ecopath.EcopathData.GroupName(xgrp), True) = 0) Then
                            correct(xgrp - 1) += 1
                            ' Exit For ' JS: keep on checking to find duplicates
                        End If
                    Next
                End If
            End While
        Catch ex As Exception
            bOK = False
        End Try

        csv.Dispose()
        cMSEUtils.ReleaseReader(reader)

        ' Report file read error
        If (bOK = False) Then
            Me.InformUser(String.Format(My.Resources.ERROR_CSV_MALFORMED, Path.GetFileName(strPath)), eMessageImportance.Warning)
            Return False
        End If

        'check that there are no replicates
        For igrp = 1 To mCore.nGroups
            If correct(igrp - 1) > 1 Then
                Me.InformUser(String.Format(My.Resources.ERROR_DISTRPARAM_GROUPS_REPLICATED, Path.GetFileName(strPath)), eMessageImportance.Warning)
                Return False
            End If
        Next

        'sum all the values in correct to be use to diagnose whether there are the correct number of groups in the file
        For Each i In correct
            TotalFound += i
        Next

        ' Check whether there are too few groups in the file
        If TotalFound < mCore.nLivingGroups Then
            Me.InformUser(String.Format(My.Resources.ERROR_DISTRFILE_GROUPS_LIVING_MISSING, Path.GetFileName(strPath)), eMessageImportance.Warning)
            Return False
        ElseIf TotalFound > mCore.nLivingGroups Then 'Check whether there are too many groups in the file
            Me.InformUser(String.Format(My.Resources.ERROR_DISTRFILE_GROUPS_HASNONLIVING, Path.GetFileName(strPath)), eMessageImportance.Warning)
            Return False
        End If

        ' Phew
        Return True

    End Function

    ''' <summary>
    ''' Checks whether each of the Ecosim distribution files (not vulnerabilities) is has the correct functional groups in it
    ''' It does this by saving a true in the position of an array at the index at which it exists in EwE
    ''' It then sums the values in this array and checks that they are equal to nlivinggroups (TRUE=1)
    ''' The reason I have done this is to prevent the problem where a file might have replicate groups
    ''' If a file has replicate groups and we check each group to see if it is in EwE and it happens that the number
    ''' of groups in the file are equal to the number of living groups, the file will be wrongly accepted
    ''' </summary>
    ''' <param name="strPath"></param>
    ''' <returns>True if all ok.</returns>
    Public Function CheckEcoSimDistributionFilesOkay(ByVal strPath As String) As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim correct(mCore.nGroups - 1) As Integer
        Dim TotalFound As Integer = 0
        Dim bOK As Boolean = True
        Dim nPrimaryProducers As Integer

        'initialise correct to all zeros
        For i = 1 To mCore.nGroups
            correct(i - 1) = 0
        Next

        'Count the number of primary producers
        For i = 1 To mCore.nGroups
            If mCore.EcoPathGroupInputs(i).IsProducer And mCore.EcoPathGroupInputs(i).IsLiving Then nPrimaryProducers += 1
        Next

        reader = cMSEUtils.GetReader(strPath)
        If (reader IsNot Nothing) Then
            csv = New CsvReader(reader, True)
            Try
                'cycle through each of the living functional groups each time checking if it exists in the file
                While Not csv.EndOfStream
                    If csv.ReadNextRecord() Then
                        For xgrp = 1 To mCore.nGroups
                            If String.Compare(cMSEUtils.FromCSVField(csv("GroupName")), _ecopath.EcopathData.GroupName(xgrp), True) = 0 Then
                                correct(xgrp - 1) += 1
                                Exit For
                            End If
                        Next
                    End If
                End While

            Catch ex As Exception
                bOK = False
            End Try
            csv.Dispose()
        End If

        cMSEUtils.ReleaseReader(reader)

        ' Report file read error
        If (bOK = False) Then
            Me.InformUser(String.Format(My.Resources.ERROR_CSV_MALFORMED, Path.GetFileName(strPath)), eMessageImportance.Warning)
            Return False
        End If

        'Check if any of the records are for groups which are primary producers
        'For igrp = 1 To mCore.nGroups
        '    If correct(igrp - 1) > 0 And mCore.EcoPathGroupOutputs(igrp).IsProducer Then
        '        Me.InformUser(String.Format(My.Resources.ERROR_DISTRFILE_GROUPS_INVALID_PRODUCER, _
        '                                     Path.GetFileNameWithoutExtension(strPath), _
        '                                     mCore.EcoPathGroupOutputs(igrp).Name, _
        '                                     cStringUtils.vbCrLf), _
        '                       eMessageImportance.Warning)
        '        Return False
        '    End If
        'Next

        'check that there are no replicates
        For igrp = 1 To mCore.nGroups
            If correct(igrp - 1) > 1 Then
                Me.InformUser(String.Format(My.Resources.ERROR_DISTRPARAM_GROUPS_REPLICATED, Path.GetFileNameWithoutExtension(strPath)), eMessageImportance.Warning)
                Return False
            End If
        Next

        'sum all the values in correct to be use to diagnose whether there are the correct number of groups in the file
        For Each i In correct
            TotalFound += i
        Next

        If TotalFound < mCore.nLivingGroups - nPrimaryProducers Then 'Check whether there are too few groups in the file
            Me.InformUser(String.Format(My.Resources.ERROR_DISTRPARAM_GROUPS_TOOFEW, Path.GetFileNameWithoutExtension(strPath)), eMessageImportance.Warning)
            Return False
        ElseIf TotalFound > mCore.nLivingGroups Then 'Check whether there are too many groups in the file
            Me.InformUser(String.Format(My.Resources.ERROR_DISTRPARAM_GROUPS_TOOMANY, Path.GetFileNameWithoutExtension(strPath)), eMessageImportance.Warning)
            Return False
        End If

        Return True

    End Function

    Private m_iNumModelsAvailable As Integer = cCore.NULL_VALUE

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

    Private m_iNumStrategiesAvailable As Integer = cCore.NULL_VALUE

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
                Me.ExtractHCR()
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

#Region " EwE app flow plugins "

    Public Function CloseModel() As Boolean Implements EwEPlugin.IEcopathPlugin.CloseModel
        ' NOP
        Return True
    End Function

    Public Function LoadModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.LoadModel
        Me.InvalidateConfiguration()
        Return True
    End Function

    Public Function SaveModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.SaveModel
        Return True
    End Function

    Public Sub CloseEcosimScenario() Implements EwEPlugin.IEcosimPlugin.CloseEcosimScenario
        ' NOP
    End Sub

    Public Sub LoadEcosimScenario(dataSource As Object) Implements EwEPlugin.IEcosimPlugin.LoadEcosimScenario
        Me.InvalidateConfiguration()
    End Sub

    Public Sub SaveEcosimScenario(dataSource As Object) Implements EwEPlugin.IEcosimPlugin.SaveEcosimScenario
        ' NOP
    End Sub

#End Region ' EwE app flow plugins

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the MSE is running. This flag is used to know when to 
    ''' suppress core messages in order not to disrupt the MSE run flow.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property IsRunning As Boolean = False

    Public ReadOnly Property DataPath As String
        Get
            If Me.UseEwEPath Then
                Return Path.Combine(Me.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim), "CefasMSE")
            End If
            Return Me.CustomPath
        End Get
    End Property

    Private Sub ExtractChangeInEffortLimits()

        ' JS 30Sep13: Standardized path access
        ' JS 02Oct13: Used standard CSV field reading/writing

        Dim strPath As String = cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv")
        Dim reader As StreamReader = cMSEUtils.GetReader(strPath)

        ReDim ChangeInEffortLimits(mCore.nFleets - 1)

        If (reader IsNot Nothing) Then

            Try
                Dim EffortLimitsCSV As New CsvReader(reader, True)
                For i = 1 To mCore.nFleets
                    ChangeInEffortLimits(i - 1) = NoHCR_F
                Next
                While Not EffortLimitsCSV.EndOfStream
                    If EffortLimitsCSV.ReadNextRecord() Then
                        ChangeInEffortLimits(cStringUtils.ConvertToInteger(EffortLimitsCSV(0)) - 1) = cStringUtils.ConvertToDouble(EffortLimitsCSV(2))
                    End If
                End While
                EffortLimitsCSV.Dispose()

            Catch ex As Exception
                ' CSV malformed, handle error?
            End Try

        End If

        cMSEUtils.ReleaseReader(reader)

    End Sub

    Public Function ExtractHCR() As Boolean

        Dim StrategiesFileNames As String()
        Dim csv As CsvReader
        Dim tempHCRGroup As HCR_Group
        Dim Strategy As Strategy = Nothing
        Dim datadir As String = cMSEUtils.MSEFolder(Me.DataPath, cMSEUtils.eMSEPaths.Strategies)
        Dim strVal As String = ""

        Strategies.DataDirectory = datadir

        'Get an array of strings giving the path to each HCR
        ' JS 30Sep13: Only read CSV files
        StrategiesFileNames = Directory.GetFiles(datadir, "*.csv")

        For Each HCRFileName As String In StrategiesFileNames 'loop through reading each HCR file

            ' ToDo_JS: make robust
            Dim reader As StreamReader = cMSEUtils.GetReader(HCRFileName)
            If (reader IsNot Nothing) Then
                csv = New CsvReader(reader, True)
                'Create the new Strategy with the Filename as the strategy name
                Strategy = New Strategy(Path.GetFileNameWithoutExtension(HCRFileName), HCRFileName)

                Try
                    Do Until csv.EndOfStream
                        If csv.ReadNextRecord() Then
                            'Read all fields from csv and then add to the list that makes up the whole strategy
                            'csv.ReadNextRecord()
                            'Each HCR Group needs to be a new object
                            tempHCRGroup = New HCR_Group(Me.m_uic.Core)

                            ' Resolve group
                            tempHCRGroup.GroupB = Me.ResolveGroup(csv(0), cStringUtils.ConvertToInteger(csv(1)))
                            tempHCRGroup.LowerLimit = cStringUtils.ConvertToDouble(csv(2))
                            tempHCRGroup.UpperLimit = cStringUtils.ConvertToDouble(csv(3))
                            tempHCRGroup.GroupF = Me.ResolveGroup(csv(4), cStringUtils.ConvertToInteger(csv(5)))
                            tempHCRGroup.MaxF = cStringUtils.ConvertToDouble(csv(6))
                            tempHCRGroup.CostFunction = HCR_Group.toCostFunctionEnum(csv(7))

                            'tempHCRGroup.GroupName4Biomass = csv(0)
                            'tempHCRGroup.GroupNumber4Biomass = csv(1)
                            'tempHCRGroup.GroupName4F = csv(4)
                            'tempHCRGroup.GroupNumber4F = csv(5)
                            'tempHCRGroup.CostFunctionOrg = csv(7)

                            ' Only add valid strategies!
                            If tempHCRGroup.isValid(strVal) Then
                                Strategy.Add(tempHCRGroup)
                            End If
                        End If
                    Loop
                    Strategies.Add(Strategy)

                Catch ex As Exception
                    ' ToDo: decide what to do when CSV data is malformed
                End Try
                csv.Dispose()
            End If

            'End While
            'csv.Dispose()
            cMSEUtils.ReleaseReader(reader)
        Next

        Return True

    End Function

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
        Dim vulnerabilities(nIterations - 1, _ecopath.EcopathData.NumGroups - 1, _ecopath.EcopathData.NumGroups - 1) As Double

        For iIteration As Integer = 1 To nIterations
            Dim reader As StreamReader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "VulnerabilityIteration" & iIteration.ToString & "_out.csv"))
            If (reader IsNot Nothing) Then
                csv = New CsvReader(reader, False)
                Try
                    While Not csv.EndOfStream
                        If csv.ReadNextRecord() Then
                            For iPrey As Integer = 1 To _ecopath.EcopathData.NumGroups
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

    Private Sub SaveOriginalParameters()

        Dim ecopathData As cEcopathDataStructures = Me._ecopath.EcopathData
        Dim ecosimData As cEcosimDatastructures = Me._ecosim.EcosimData

        ReDim BTemp(mCore.nGroups - 1)
        ReDim PBTemp(mCore.nGroups - 1)
        ReDim QBTemp(mCore.nGroups - 1)
        ReDim EETemp(mCore.nGroups - 1)
        ReDim BATemp(mCore.nGroups - 1)
        ReDim DenDepCatchabilityTemp(mCore.nGroups - 1)
        ReDim FeedingTimeAdjustRateTemp(mCore.nGroups - 1)
        ReDim MaxRelFeedingTimeTemp(mCore.nGroups - 1)
        ReDim OtherMortFeedingTimeTemp(mCore.nGroups - 1)
        ReDim PredEffectFeedingTimeTemp(mCore.nGroups - 1)
        ReDim QBMaxxQBioTemp(mCore.nGroups - 1)
        ReDim SwitchingPowerTemp(mCore.nGroups - 1)
        ReDim VulnerabilitiesTemp(mCore.nGroups - 1, mCore.nGroups - 1)
        ReDim DietMatrixTemp(mCore.nGroups - 1, mCore.nGroups - 1)
        ReDim DietImpTemp(mCore.nGroups - 1)

        For x = 0 To mCore.nGroups - 1
            BTemp(x) = ecopathData.B(x + 1)
            PBTemp(x) = ecopathData.PB(x + 1)
            QBTemp(x) = ecopathData.QB(x + 1)
            EETemp(x) = ecopathData.EE(x + 1)
            BATemp(x) = ecopathData.BA(x + 1)
            DenDepCatchabilityTemp(x) = ecosimData.QmQo(x + 1)
            FeedingTimeAdjustRateTemp(x) = ecosimData.FtimeAdjust(x + 1)
            MaxRelFeedingTimeTemp(x) = ecosimData.FtimeMax(x + 1)
            OtherMortFeedingTimeTemp(x) = ecosimData.MoPred(x + 1)
            PredEffectFeedingTimeTemp(x) = ecosimData.RiskTime(x + 1)
            QBMaxxQBioTemp(x) = ecosimData.CmCo(x + 1)
            SwitchingPowerTemp(x) = ecosimData.SwitchPower(x + 1)
            For y = 0 To mCore.nGroups - 1
                VulnerabilitiesTemp(x, y) = ecosimData.VulMult(x + 1, y + 1)
                DietMatrixTemp(x, y) = ecopathData.DC(x + 1, y + 1)
            Next
            DietImpTemp(x) = mCore.EcoPathGroupInputs(x + 1).ImpDiet
        Next

    End Sub

    Private Sub RestoreParameters()

        Dim ecopathData As cEcopathDataStructures = Me._ecopath.EcopathData
        Dim ecosimData As cEcosimDatastructures = Me._ecosim.EcosimData

        For x = 0 To mCore.nGroups - 1
            ecopathData.B(x + 1) = CSng(BTemp(x))
            ecopathData.PB(x + 1) = CSng(PBTemp(x))
            ecopathData.QB(x + 1) = CSng(QBTemp(x))
            ecopathData.EE(x + 1) = CSng(EETemp(x))
            ecopathData.BA(x + 1) = CSng(BATemp(x))
            ecosimData.QmQo(x + 1) = CSng(DenDepCatchabilityTemp(x))
            ecosimData.FtimeAdjust(x + 1) = CSng(FeedingTimeAdjustRateTemp(x))
            ecosimData.FtimeMax(x + 1) = CSng(MaxRelFeedingTimeTemp(x))
            ecosimData.MoPred(x + 1) = CSng(OtherMortFeedingTimeTemp(x))
            ecosimData.RiskTime(x + 1) = CSng(PredEffectFeedingTimeTemp(x))
            ecosimData.CmCo(x + 1) = CSng(QBMaxxQBioTemp(x))
            ecosimData.SwitchPower(x + 1) = CSng(SwitchingPowerTemp(x))
            For y = 0 To mCore.nGroups - 1
                ecosimData.VulMult(x + 1, y + 1) = CSng(VulnerabilitiesTemp(x, y))
                ecopathData.DC(x + 1, y + 1) = CSng(DietMatrixTemp(x, y))
            Next
            mCore.EcoPathGroupInputs(x + 1).ImpDiet = CSng(DietImpTemp(x))
        Next
        Me.Core.DiscardChanges()

    End Sub


    Public Sub LoadSampledParams()

        Me.IsRunning = True
        Me.ChangeEffortFlag = True
        cApplicationStatusNotifier.StartProgress(Me.Core, "", -1)

        Try
            Me.Run()
        Catch ex As Exception
            ' Whoah!
            cLog.Write(ex, "CefasMSE:LoadSampledParameters")
        End Try

        cApplicationStatusNotifier.EndProgress(Me.Core)
        Me.ChangeEffortFlag = False
        Me.IsRunning = False

    End Sub

    Private Sub Run()

        ' JS 20Oct13: Fixed path usage
        ' JS 20Oct13: Used standard CSV field reading/writing; all names CSV protected and values written in US-en notation
        ' JS 20Oct13: Applied standard EwE headers. Mark, please don't kill me

        Debug.Assert(Me.IsInputDataCompatible)

        Dim B(,) As Double 'All basic Ecopath and Ecosim parameters X(a,b) a = iteration b = the functional group
        Dim PB(,) As Double
        Dim QB(,) As Double
        Dim EE(,) As Double
        Dim BA(,) As Double
        Dim DenDepCatchability(,) As Double
        Dim FeedingTimeAdjustRate(,) As Double
        Dim MaxRelFeedingTime(,) As Double
        Dim OtherMortFeedingTime(,) As Double
        Dim PredEffectFeedingTime(,) As Double
        Dim QBMaxxQBio(,) As Double
        Dim SwitchingPower(,) As Double
        Dim Vulnerabilities(,,) As Double
        Dim nTrials As Integer
        Dim ecopathData As cEcopathDataStructures = Me._ecopath.EcopathData
        Dim ecosimData As cEcosimDatastructures = Me._ecosim.EcosimData
        Dim GoodDynamics As Boolean
        Dim Results As New DataTable

        Dim diet_matrix As CsvReader

        Dim NumberIterationsAlreadyInResults As Integer
        Dim NumberIterationsAlreadyInFleets As Integer
        Dim nFailedParameterisations As Integer = 0
        'Dim nLivingGroupsMinusPPers As Integer

        Dim swGroup As StreamWriter
        Dim swFleet As StreamWriter
        Dim TrajectoryCsv As StreamWriter
        Dim Trajectory2Csv As List(Of StreamWriter)             'Trajectories2 is similar to trajectories apart from it each file contains only 1 group

        ' JS 30Sep13: Use local properties
        Dim NYearsProject = Me.NYearsProject
        Dim BiomassProjected(NYearsProject * _ecosim.EcosimData.NumStepsPerYear - 1) As Double

        Dim msgReport As New cFeedbackMessage("?", eCoreComponentType.External, eMessageType.DataExport, eMessageImportance.Information, eMessageReplyStyle.OK)
        msgReport.Hyperlink = cMSEUtils.MSEFolder(Me.DataPath, cMSEUtils.eMSEPaths.Results)

        OriginalNTimesteps = _ecosim.EcosimData.NTimes

        SaveOriginalParameters()

        'Output the final results
        swGroup = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.Results, "Results.csv"), False)
        If Me.mCore.SaveWithFileHeader Then swGroup.WriteLine(Me.mCore.DefaultFileHeader(eAutosaveTypes.Ecosim))
        swGroup.WriteLine("Iteration,Strategy,GroupNumber,GroupName,ResultName,Value")
        msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "Results.csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

        'Create the csv writer for writing out individual fleets catches of each group
        swFleet = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.Results, "Fleet.csv"), False)
        If Me.mCore.SaveWithFileHeader Then swFleet.WriteLine(Me.mCore.DefaultFileHeader(eAutosaveTypes.Ecosim))
        swFleet.WriteLine("Iteration,Strategy,FleetNumber,FleetName,GroupNumber,GroupName,Value")
        msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "Fleet.csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

        'Count the number of live groups which aren't primary producers
        'nLivingGroupsMinusPPers = mCore.nLivingGroups
        'For i = 1 To mCore.nGroups
        '    If mCore.EcoPathGroupInputs(i).IsLiving And mCore.EcoPathGroupInputs(i).IsProducer Then
        '        nLivingGroupsMinusPPers -= 1
        '    End If
        'Next

        'Extract the maximum percentage change in effort for each fleet from csv and put into array ChangeInEffortLimits
        'to be used by the optim to determine effort is beyond maximum change in effort
        ExtractChangeInEffortLimits()

        ''Load up what type of cost function to use for each strategy
        'Try
        '    CostFunctionTypeArray = LoadCostFunctionsCSV()
        'Catch e As Exception
        '    MessageBox.Show(e.ToString)
        'End Try

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

        'Prepare the trajectory csv with the column headings
        Trajectory2Csv = New List(Of StreamWriter)
        For igrp = 1 To mCore.nLivingGroups

            Dim strFile As String = cFileUtils.ToValidFileName(mCore.EcoPathGroupInputs(igrp).Name & "_GroupNo" & igrp & ".csv", False)
            Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ResultsTraj2, strFile))
            msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, strFile), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

            Debug.Assert(writer IsNot Nothing)

            Trajectory2Csv.Add(writer)
            If Me.mCore.SaveWithFileHeader Then Trajectory2Csv(igrp - 1).WriteLine(Me.mCore.DefaultFileHeader(eAutosaveTypes.Ecosim))
            Trajectory2Csv(igrp - 1).Write("Trial,Strategy")
            For iTime = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                Trajectory2Csv(igrp - 1).Write("," & cStringUtils.FormatNumber(iTime))
            Next
            Trajectory2Csv(igrp - 1).WriteLine()

        Next

        'load parameter values into ecopath and ecosim to be used
        nTrials = Me.NModels2Run    '0 is the 1st dimension and 1' the second etc
        For iTrial = 1 To nTrials

            'Console.WriteLine("Trial = " & iTrial)
            cApplicationStatusNotifier.UpdateProgress(Me.Core, String.Format(My.Resources.STATUS_RUN_PROGRESS, My.Resources.CAPTION, iTrial), CSng(iTrial / nTrials))

            For igrp = 1 To mCore.nLivingGroups
                If Not B(iTrial - 1, igrp - 1) = cCore.NULL_VALUE Then
                    ecopathData.B(igrp) = CSng(B(iTrial - 1, igrp - 1))
                    ecopathData.PB(igrp) = CSng(PB(iTrial - 1, igrp - 1))
                    ecopathData.QB(igrp) = CSng(QB(iTrial - 1, igrp - 1))
                    ecopathData.EE(igrp) = CSng(EE(iTrial - 1, igrp - 1))
                    ecopathData.BA(igrp) = CSng(BA(iTrial - 1, igrp - 1))
                    If Not mCore.EcoPathGroupInputs(igrp).IsProducer Then
                        ecosimData.QmQo(igrp) = CSng(DenDepCatchability(iTrial - 1, igrp - 1))
                        ecosimData.FtimeAdjust(igrp) = CSng(FeedingTimeAdjustRate(iTrial - 1, igrp - 1))
                        ecosimData.FtimeMax(igrp) = CSng(MaxRelFeedingTime(iTrial - 1, igrp - 1))
                        ecosimData.MoPred(igrp) = CSng(OtherMortFeedingTime(iTrial - 1, igrp - 1))
                        ecosimData.RiskTime(igrp) = CSng(PredEffectFeedingTime(iTrial - 1, igrp - 1))
                        ecosimData.CmCo(igrp) = CSng(QBMaxxQBio(iTrial - 1, igrp - 1))
                        ecosimData.SwitchPower(igrp) = CSng(SwitchingPower(iTrial - 1, igrp - 1))
                        If mCore.EcoPathGroupInputs(igrp).IsProducer Then Stop
                    End If
                Else
                    Stop
                End If
            Next



            For iPrey As Integer = 1 To mCore.nGroups
                'For iPrey As Integer = 1 To Vulnerabilities.GetLength(1)
                For iPred As Integer = 1 To mCore.nLivingGroups
                    'For iPred As Integer = 1 To Vulnerabilities.GetLength(2)
                    If Not Vulnerabilities(iTrial - 1, iPred - 1, iPrey - 1) = cCore.NULL_VALUE Then
                        ecosimData.VulMult(iPrey, iPred) = CSng(Vulnerabilities(iTrial - 1, iPred - 1, iPrey - 1))
                    Else
                        ecosimData.VulMult(iPrey, iPred) = cCore.NULL_VALUE
                    End If
                Next
            Next

            ' Start optimistically. It's a beautiful day outside, etc. etc.
            GoodDynamics = True

            'Extract the diet matrix
            Dim reader As StreamReader = cMSEUtils.GetReader(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "DietMatrixTrial" & iTrial & ".csv"))
            If (reader IsNot Nothing) Then
                diet_matrix = New CsvReader(reader, False)
                If diet_matrix.ReadNextRecord() Then
                    For iPred As Integer = 1 To mCore.nLivingGroups
                        mCore.EcoPathGroupInputs(iPred).ImpDiet() = cStringUtils.ConvertToSingle(diet_matrix(iPred - 1))
                    Next
                Else
                    ' Unable to read predator header line! We have a problem
                    GoodDynamics = False
                End If
                For iPrey As Integer = 1 To mCore.nGroups
                    If (Not diet_matrix.EndOfStream) And (diet_matrix.ReadNextRecord()) Then
                        For iPred As Integer = 1 To mCore.nLivingGroups
                            mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) = cStringUtils.ConvertToSingle(diet_matrix(iPred - 1))
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

            If GoodDynamics Then

                Try

                    Me._ecopath.Run()

                    'make sure Ecosim computes the output data
                    Me._ecosim.EcosimData.bTimestepOutput = True

                    'No timestep call back
                    Me._ecosim.TimeStepDelegate = Nothing

                    'Run on the same thread 
                    'this means Me._ecosim.Run() will block until Ecosim has finished running
                    Me._ecosim.EcosimData.bMultiThreaded = False

                    'increase the number of years for the projection
                    mCore.EcoSimModelParameters.NumberYears = CInt(OriginalNTimesteps / _ecosim.EcosimData.NumStepsPerYear + NYearsProject)

                    'This creates the files we will write the biomass trajectories to
                    TrajectoryCsv = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ResultsTrajectories, "Trial" & iTrial & ".csv"), False)
                    If Me.mCore.SaveWithFileHeader Then TrajectoryCsv.WriteLine(Me.mCore.DefaultFileHeader(eAutosaveTypes.Ecosim))
                    TrajectoryCsv.Write("GroupNumber,Group,Strategy")
                    msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, "Trial" & iTrial & ".csv"), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

                    For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                        TrajectoryCsv.Write("," & cStringUtils.FormatNumber(iTime))
                    Next
                    TrajectoryCsv.WriteLine()

                    For Each iStrategy In Strategies

                        CurrentStrategy = iStrategy
                        'Run Ecosim
                        Me._ecosim.Init(True)   'causes it to reset the f's to the base
                        Me._ecosim.Run()

                        'Check whether the biomass for any species goes beneath or hits zero
                        For iGrp As Integer = 1 To mCore.nLivingGroups
                            For iTimeStep As Integer = 1 To OriginalNTimesteps
                                'Console.Write(mCore.EcoSimGroupOutputs(iGrp).Biomass(iTimeStep).ToString & " ")
                                If Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, iTimeStep) <= 0 Then GoodDynamics = False
                                'If mCore.EcoSimGroupOutputs(iGrp).Biomass(iTimeStep) <= 0 Then GoodDynamics = False
                                'Test
                                If GoodDynamics = False Then Exit For
                            Next
                            If GoodDynamics = False Then Exit For
                        Next iGrp

                        If GoodDynamics = False Then
                            Console.WriteLine("This set of parameters is no good")
                            nFailedParameterisations += 1
                        Else
                            For iFleet = 1 To mCore.nFleets
                                For iGrp = 1 To mCore.nLivingGroups
                                    swFleet.WriteLine("{0},{1},{2},{3},{4},{5},{6}", _
                                                      cStringUtils.FormatNumber(iTrial + NumberIterationsAlreadyInFleets), _
                                                      cStringUtils.ToCSVField(iStrategy.Name), _
                                                      cStringUtils.FormatNumber(iFleet), cStringUtils.ToCSVField(mCore.FleetInputs(iFleet).Name), _
                                                      cStringUtils.FormatNumber(iGrp), cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iGrp).Name), _
                                                      cStringUtils.FormatNumber(Me._simdata.ResultsSumCatchByGroupGear(iGrp, iFleet, OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear)))
                                Next
                            Next

                            For igrp = 1 To mCore.nLivingGroups
                                'calculate what the minimum biomass was for each group
                                For iTime As Integer = 1 To NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                                    BiomassProjected(iTime - 1) = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, OriginalNTimesteps + iTime)
                                Next

                                'Output to csv the biomass trajectories
                                TrajectoryCsv.Write("{0},{1},{2}", _
                                                    cStringUtils.FormatNumber(igrp), _
                                                    cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(igrp).Name), _
                                                    cStringUtils.ToCSVField(iStrategy.Name))

                                For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                                    TrajectoryCsv.Write("," & cStringUtils.FormatNumber(Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime)))
                                Next
                                TrajectoryCsv.WriteLine()

                                'Trajectory2Csv(igrp - 1).Write(iTrial & "," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))))
                                Trajectory2Csv(igrp - 1).Write(iTrial & "," & cStringUtils.ToCSVField(iStrategy.Name))
                                For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                                    Trajectory2Csv(igrp - 1).Write("," & cStringUtils.FormatNumber(Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime)))
                                Next
                                Trajectory2Csv(igrp - 1).WriteLine()

                                swGroup.WriteLine("{0},{1},{2},{3},Biomass,{4}", _
                                             cStringUtils.FormatNumber(NumberIterationsAlreadyInResults + iTrial), _
                                             cStringUtils.ToCSVField(iStrategy.Name), _
                                             cStringUtils.FormatNumber(igrp), _
                                             cStringUtils.ToCSVField(mCore.EcoPathGroupOutputs(igrp).Name), _
                                             cStringUtils.FormatNumber(BiomassProjected.Min))
                                swGroup.WriteLine("{0},{1},{2},{3},BiomassEnd,{4}", _
                                              cStringUtils.FormatNumber(NumberIterationsAlreadyInResults + iTrial), _
                                              cStringUtils.ToCSVField(iStrategy.Name), _
                                              cStringUtils.FormatNumber(igrp), _
                                              cStringUtils.ToCSVField(mCore.EcoPathGroupOutputs(igrp).Name), _
                                              cStringUtils.FormatNumber(Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, ecosimData.NTimes)))
                                'Results.Rows.Add(iIteration, HCRFiles(Strategies.IndexOf(iStrategy)), mCore.EcoPathGroupOutputs(igrp).Name, "Catch", Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, ecosimData.NTimes))
                                swGroup.WriteLine("{0},{1},{2},{3},Catch,{4}", _
                                              (NumberIterationsAlreadyInResults + iTrial), _
                                              cStringUtils.ToCSVField(iStrategy.Name), _
                                              cStringUtils.FormatNumber(igrp), cStringUtils.ToCSVField(mCore.EcoPathGroupOutputs(igrp).Name), _
                                              cStringUtils.FormatNumber(Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, ecosimData.NTimes)))

                            Next

                            For iFleet As Integer = 1 To mCore.nFleets
                                swGroup.WriteLine("{0},{1},{2},{3},TotalEndValue,{4}", _
                                             cStringUtils.FormatNumber(NumberIterationsAlreadyInResults + iTrial), _
                                             cStringUtils.ToCSVField(iStrategy.Name), _
                                             cStringUtils.FormatNumber(iFleet), _
                                             cStringUtils.ToCSVField(mCore.FleetInputs(iFleet).Name), _
                                             cStringUtils.FormatNumber(Me._simdata.ResultsSumValueByGear(iFleet, _ecosim.EcosimData.NTimes)))
                            Next
                        End If

                        If GoodDynamics = False Then Exit For
                    Next iStrategy

                    cMSEUtils.ReleaseWriter(TrajectoryCsv)

                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".Run() Exception: " & ex.Message)
                End Try
            End If

            'BadDynamics:  ' This is so that if the dynamics of a parameterisation are bad that we can skip out of the loops and onto the the next Trial
            GoodDynamics = True

        Next iTrial

        For igrp = 1 To mCore.nLivingGroups
            cMSEUtils.ReleaseWriter(Trajectory2Csv(igrp - 1))
        Next

        'ecosimData.NTimes is the number of months so 17 years = 204 timesteps
        mCore.EcoSimModelParameters.NumberYears = CInt(OriginalNTimesteps / _ecosim.EcosimData.NumStepsPerYear)

        'Provide user with a message stating how many of the Trials produced reasonable dynamics
        msgReport.Message = String.Format(My.Resources.PROMPT_TRIAL_REPORT, (nTrials - nFailedParameterisations), nTrials, CInt((nTrials - nFailedParameterisations) * 100 / nTrials))
        Me.Core.Messages.SendMessage(msgReport)

        cMSEUtils.ReleaseWriter(swGroup)
        cMSEUtils.ReleaseWriter(swFleet)

        'This should only be commented out for testing purposes!!!
        Me.RestoreOriginalState()

    End Sub

    Public Function DirichletSample2(ByVal nDimensions As Integer, ByVal alpha() As Single, ByRef DietMultiplier As Double) As Single()
        Dim gamma(nDimensions - 1) As Single
        Dim dirichlet(nDimensions - 1) As Single
        Dim sumofgamma As Single
        Dim GammaGenerator As New GammaDistribution

        For i = 0 To alpha.Length() - 1
            'alpha(i) = alpha(i) * TempDietMultiplier
            alpha(i) = CSng(alpha(i) * DietMultiplier)
        Next

        'ALGORITHM 1 From A Convenient Way of Generating Gamma
        'Random Variables Using Generalized
        'Exponential(Distribution)
        'Debasis Kundu1 & Rameshwar D. Gupta2
        'For i As Integer = 0 To nDimensions - 1
        '    Do
        '        U = Rnd()
        '        X = -2 * Math.Log(1 - U ^ (1 / alpha(i)))
        '        V = Rnd()
        '    Loop Until V <= (X ^ (alpha(i) - 1) * Math.E ^ (-X / 2)) / (2 ^ (alpha(i) - 1) * (1 - Math.E ^ (-X / 2)) ^ (alpha(i) - 1))
        '    gamma(i) = X
        'Next

        'ALGORITHM 2 FROM ABOVE
        'For i As Integer = 0 To nDimensions - 1
        '    FoundX = False
        '    Do
        '        a = ((1 - Math.Exp(-1 / 2)) ^ alpha(i)) / ((1 - Math.Exp(-1 / 2)) ^ alpha(i) + (alpha(i) * Math.Exp(-1)) / (2 ^ alpha(i)))
        '        b = (1 - Math.Exp(-1 / 2)) ^ alpha(i) + (alpha(i) * Math.Exp(-1)) / (2 ^ alpha(i))
        '        U = Rnd()
        '        If U <= a Then
        '            X = -2 * Math.Log(1 - (U * b) ^ (1 / alpha(i)))
        '        Else
        '            X = -Math.Log((2 ^ alpha(i) / alpha(i)) * b * (1 - U))
        '        End If
        '        V = Rnd()
        '        If X <= 1 And V <= (X ^ (alpha(i) - 1) * Math.Exp(-X / 2)) / (2 ^ (alpha(i) - 1) * (1 - Math.Exp(-X / 2)) ^ (alpha(i) - 1)) Then
        '            FoundX = True
        '        ElseIf X > 1 And V <= X ^ (alpha(i) - 1) Then
        '            FoundX = True
        '        End If
        '    Loop Until FoundX = True
        '    gamma(i) = X
        'Next

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

    Public Function DirichletSample(ByVal nDimensions As Integer, ByVal a() As Single, ByRef DietMultiplier As Double) As Single()
        Dim u1, u2, b, p, x As Single
        Dim gamma(nDimensions - 1) As Single
        Dim dirichlet(nDimensions - 1) As Single
        Dim sumofgamma As Single

        For i = 0 To a.Length() - 1
            a(i) = CSng(a(i) * DietMultiplier)
        Next

        For i As Integer = 0 To nDimensions - 1
step1:
            u1 = CSng(Me.m_rand.NextDouble())
            b = CSng((Math.E + a(i)) / Math.E)
            p = b * u1
            If p >= 1 Then GoTo step3
step2:
            x = CSng(p ^ (1 / a(i)))
            u2 = CSng(Me.m_rand.NextDouble())
            If u2 > Math.Exp(-x) Then
                GoTo step1
            Else
                gamma(i) = x
                GoTo stepend
            End If
step3:
            x = CSng(-Math.Log((b - p) / a(i)))
            u2 = CSng(Me.m_rand.NextDouble())
            If u2 > x ^ (a(i) - 1) Then
                GoTo step1
            Else
                gamma(i) = x
            End If
stepend:
        Next

        sumofgamma = gamma.Sum()
        For i = 0 To nDimensions - 1
            dirichlet(i) = gamma(i) / sumofgamma
        Next

        Return (dirichlet)

    End Function

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

    Private Sub SampleDietMatrix(ByRef Interacts(,) As Integer, ByRef MeanProportions(,) As Single, ByRef DietPropMultipliers() As Double)

        Dim MeanPropMod() As Single
        Dim SumInteractions(mCore.nLivingGroups - 1) As Single
        Dim TempDirichlet() As Single
        Dim PreyIndex As Integer
        Dim DirichStopWatch As New Stopwatch
        Dim NormaliseStopWatch As New Stopwatch
        Dim EcopathStopWatch As New Stopwatch
        Dim EcopathInternalStopWatch As New Stopwatch
        Dim ecopathData As cEcopathDataStructures = Me._ecopath.EcopathData
        Dim iPointer As Integer = 0

        'Dim DirichletArray(mCore.nLivingGroups - 1, mCore.nLivingGroups - 1) As Single

        'Array.Clear(DirichletArray, 0, DirichletArray.GetLength(1))

        'Generate a vector 'SumInteractions' that counts how many prey each predator has
        For iPred As Integer = 0 To mCore.nLivingGroups - 1
            For iPrey As Integer = 0 To mCore.nGroups
                SumInteractions(iPred) += Interacts(iPred, iPrey)
            Next
        Next

        For iPred As Integer = 0 To mCore.nLivingGroups - 1
            'mCore.EcoPathGroupInputs(iPred + 1).DietComp(0) = 0
            If (SumInteractions(iPred) = 0) Then    'No need to do any of this unless there is at least 1 prey for this parameter
                'Set all values to zero - if running slow might want to consider how this could be skipped - possibly setting whole array to zero at start
                For iPrey = 0 To mCore.nGroups
                    ecopathData.DCInput(iPred + 1, iPrey) = 0
                Next
            Else
                ' DirichStopWatch.Start()

                ReDim MeanPropMod(CInt(SumInteractions(iPred) - 1))
                iPointer = 0
                For iPrey = 0 To mCore.nGroups
                    If Interacts(iPred, iPrey) = 1 Then
                        MeanPropMod(iPointer) = MeanProportions(iPred, iPrey)
                        iPointer += 1
                    End If
                Next



                'Samples a set of Dirichlet distributed parameters
                TempDirichlet = DirichletSample2(CInt(SumInteractions(iPred)), MeanPropMod, DietPropMultipliers(iPred))

                'Set all the diet values in ecopath to those sampled and checked to be within correct intervals
                PreyIndex = 0
                For i = 0 To TempDirichlet.GetLength(0) - 1
                    While Interacts(iPred, PreyIndex) = 0
                        ecopathData.DCInput(iPred + 1, PreyIndex) = 0
                        PreyIndex += 1
                    End While
                    ecopathData.DCInput(iPred + 1, PreyIndex) = TempDirichlet(i)
                    PreyIndex += 1
                Next

                ''Changes the output of TempDirichlet so that it is a vector of length mcore.nlivinggroups
                'If Interacts(iPred, 0) = 1 Then
                '    'mCore.EcoPathGroupInputs(iPred + 1).ImpDiet() = TempDirichlet(0)
                '    ecopathData.DC(iPred + 1, 0) = TempDirichlet(0)
                'End If
                'PreyIndex = 1
                ''EcopathStopWatch.Start()
                'For i = 1 To TempDirichlet.GetLength(0) - 1
                '    While Interacts(iPred, PreyIndex) = 0
                '        'DirichletArray(iPred, PreyIndex) = 0
                '        'mCore.EcoPathGroupInputs(iPred + 1).DietComp(PreyIndex) = 0
                '        ecopathData.DC(iPred + 1, PreyIndex) = 0
                '        PreyIndex += 1
                '    End While
                '    'DirichletArray(iPred, PreyIndex) = TempDirichlet(i)

                '    'EcopathInternalStopWatch.Start()
                '    'mCore.EcoPathGroupInputs(iPred + 1).DietComp(PreyIndex) = TempDirichlet(i)
                '    Console.WriteLine(TempDirichlet(i))
                '    ecopathData.DC(iPred + 1, PreyIndex) = TempDirichlet(i)
                '    'EcopathInternalStopWatch.Stop()
                '    PreyIndex += 1
                'Next
                ''EcopathStopWatch.Stop()

            End If

        Next

        'Console.WriteLine("Dirich Time = " & DirichStopWatch.ElapsedMilliseconds.ToString)
        'Console.WriteLine("Normalise Time = " & NormaliseStopWatch.ElapsedMilliseconds.ToString)
        'Console.WriteLine("Ecopath Time = " & EcopathStopWatch.ElapsedMilliseconds.ToString)
        'Console.WriteLine("Ecopath Internal Time = " & EcopathInternalStopWatch.ElapsedMilliseconds.ToString)

    End Sub

    'Private Sub SampleFunctionalGrpParams(ByVal ParamName As String, ByVal nIterations As Integer)
    '    Dim sPath As String = "C:\Users\mick\Desktop\GAP\Data"
    '    Dim csv = New CsvReader(New StreamReader(sPath & "/DistributionParameters/" & ParamName & ".csv"), True)
    '    Dim SampledValues(mCore.nGroups - 1, nIterations - 1) As Single

    '    For iGrp = 1 To mCore.nGroups
    '        csv.ReadNextRecord()
    '        For iIteration = 1 To nIterations
    '            If csv(1) = 1 Then
    '                SampledValues(iGrp - 1, iIteration - 1) = UniformSample(csv(2), csv(3))
    '            ElseIf csv(1) = 2 Then
    '                SampledValues(iGrp - 1, iIteration - 1) = TriangularSample(csv(2), csv(3), csv(4))
    '            End If
    '        Next
    '    Next

    '    Dim csvout As New StreamWriter(Path.Combine(sPath & "/ParametersOut/" & ParamName & ".csv"), True)
    '    csvout.Write(mCore.EcoPathGroupInputs(1).Name)
    '    If mCore.nGroups > 1 Then
    '        For iGrp = 2 To mCore.nGroups
    '            csvout.Write("," & mCore.EcoPathGroupInputs(iGrp).Name)
    '        Next
    '    End If
    '    csvout.WriteLine()
    '    For iIteration = 1 To nIterations
    '        csvout.Write(SampledValues(0, iIteration - 1))
    '        If mCore.nGroups > 1 Then
    '            For iGrp = 2 To mCore.nGroups
    '                csvout.Write("," & SampledValues(iGrp - 1, iIteration - 1))
    '            Next
    '        End If
    '        csvout.WriteLine()
    '    Next

    'End Sub

    Public Function GenerateEmptyDietCSVs() As Boolean

        Dim strPath As String = ""
        Dim writer As StreamWriter = Nothing
        Dim bSuccess As Boolean = True

        strPath = cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.DistrParams, "DietComposition.csv")
        writer = cMSEUtils.GetWriter(strPath, False)

        If (writer IsNot Nothing) Then

            writer.Write("Predator,Prey,PredIndex,PreyIndex,Interacts,Mean")
            writer.WriteLine()

            For iPred As Integer = 1 To mCore.nLivingGroups
                If mCore.EcoPathGroupInputs(iPred).ImpDiet > 0 Then
                    Dim mean As Single = mCore.EcoPathGroupInputs(iPred).ImpDiet
                    writer.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & ",Imports," & iPred & ",0,1," & cStringUtils.ToCSVField(mean))
                Else
                    writer.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & ",Imports," & iPred & ",0,0,0")
                End If

                For iPrey As Integer = 1 To mCore.nGroups
                    If mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) > 0 Then
                        Dim mean As Single = mCore.EcoPathGroupInputs(iPred).DietComp(iPrey)
                        writer.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & "," & cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPrey).Name) & "," & iPred & "," & iPrey & ",1," & cStringUtils.ToCSVField(mean))
                    Else
                        writer.WriteLine(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name) & "," & cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPrey).Name) & "," & iPred & "," & iPrey & ",0,0")
                    End If
                Next
            Next
        Else
            bSuccess = False
        End If
        cMSEUtils.ReleaseWriter(writer)

        strPath = cMSEUtils.MSEFile(Me.DataPath, cMSEUtils.eMSEPaths.DistrParams, "DietCompositionMultipliers.csv")
        writer = cMSEUtils.GetWriter(strPath, False)
        If (writer IsNot Nothing) Then
            writer.WriteLine("PredatorIndexNumber,PredatorIndexName,Multiplier")
            For iPred As Integer = 1 To mCore.nLivingGroups
                writer.WriteLine("{0},{1},{2}", _
                                 cStringUtils.ToCSVField(iPred), _
                                 cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(iPred).Name), _
                                 1)
            Next
        Else
            bSuccess = False
        End If
        cMSEUtils.ReleaseWriter(writer)

        Me.InvalidateConfiguration()
        Return bSuccess

    End Function

    Public Sub GenerateEcopathParamaters()

        Me.SaveOriginalParameters()
        Try
            Me.GenerateInputStructure()
        Catch ex As Exception
            ' Kaboom!
        End Try
        ' Re-assess configuration when next needed
        Me.InvalidateConfiguration()
        Me.RestoreOriginalState()

    End Sub

    Private Sub GenerateInputStructure()

        ' JS 12Oct13: Fixed path usage
        ' JS 12Oct13: Used standard CSV field reading/writing
        ' JS 12Oct13: Used standard readers/writers, and made robust

        Dim nLiving As Integer = mCore.nLivingGroups
        Dim nGroups As Integer = mCore.nGroups
        Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
        Dim nTrials As Integer = Me.NModels
        Dim b(nTrials, nGroups) As Single
        Dim ba(nTrials, nLiving) As Single
        Dim pb(nTrials, nLiving) As Single
        Dim qb(nTrials, nLiving) As Single
        Dim ee(nTrials, nLiving) As Single
        Dim TimeFindingBalanced As New Stopwatch
        Dim csv As CsvReader
        Dim MeanProportions(mCore.nLivingGroups - 1, mCore.nGroups) As Single
        Dim DietPropMultipliers(mCore.nLivingGroups - 1) As Double
        Dim Interacts(mCore.nLivingGroups - 1, mCore.nGroups) As Integer
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

        'Calculate how many living groups that aren't primary producers
        'For i = 1 To mCore.nGroups
        '    If mCore.EcoPathGroupInputs(i).IsProducer Then nPPers += 1
        'Next i
        'nLivingMinusPPers = mCore.nLivingGroups - nPPers

        Me.IsRunning = True
        cApplicationStatusNotifier.StartProgress(Me.Core, "", -1)
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

                        ' Provide occassional UI feedback
                        If (i = 1) Or ((i Mod 50) = 0) Then
                            cApplicationStatusNotifier.UpdateProgress(Me.Core, String.Format(My.Resources.STATUS_TRIAL_PROGRESS, My.Resources.CAPTION, iTrial, i), -1)
                        End If

                        'Write code here that generates a whole set of diet parameters to be used in combination with new ecopath parameters
                        'to be tested for the mass-balance criteria
                        SampleDietMatrix(Interacts, MeanProportions, DietPropMultipliers)

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
                                        For iPred = 1 To mCore.nLivingGroups
                                            If iPred > 1 Then csv_dietout.Write(",")
                                            csv_dietout.Write(cStringUtils.FormatNumber(Me._ecopath.EcopathData.DC(iPred, iPrey)))
                                        Next
                                        csv_dietout.WriteLine()
                                    Next
                                Catch ex As Exception
                                    ' ToDo: respond to error
                                End Try
                                cMSEUtils.ReleaseWriter(csv_dietout)

                                ' JS 30Sep13: greatly simplified :)
                                WriteEcopathParms("b_out.csv", Me._ecopath.EcopathData.B)
                                WriteEcopathParms("ba_out.csv", Me._ecopath.EcopathData.BA)
                                WriteEcopathParms("pb_out.csv", Me._ecopath.EcopathData.PB)
                                WriteEcopathParms("qb_out.csv", Me._ecopath.EcopathData.QB)
                                WriteEcopathParms("ee_out.csv", Me._ecopath.EcopathData.EE)
                                ''This runs Ecosim without core support
                                'If Me.RunEcosim() Then
                                '    'dumps out some Ecosim results
                                '    Me.getEcosimResults()
                                'End If 'RunEcosim

                                Me.InformUser(String.Format(My.Resources.STATUS_FOUND_MODEL, My.Resources.CAPTION, i), eMessageImportance.Information)
                                iNumFound += 1
                                bFound = True

                            End If
                        Else
                            System.Console.WriteLine("Failed to find balanced Ecopath model")
                        End If ' MonteCarlo.selectNewEcopathParameters()

                        i += 1
                        bExpired = (sw.ElapsedMilliseconds > lTimeout)

                    End While

                    'Console.WriteLine("Number of seconds to run iteration: " & (TimeFindingBalanced.ElapsedMilliseconds / 1000).ToString)
                    TimeFindingBalanced.Reset()

                    iTrial += 1

                End While

                If bExpired Then
                    Me.InformUser("MSE expired after " & sw.ElapsedMilliseconds & "ms", eMessageImportance.Information)
                End If

            End If 'Me.InitMonteCarloParameters()

            'Save the results to a .csv

        Catch ex As Exception

        End Try

        cApplicationStatusNotifier.EndProgress(Me.Core)
        Me.IsRunning = False

        Me.RestoreOriginalState()

        ' Provide summary
        If iNumFound = 0 Then
            Me.InformUser(String.Format(My.Resources.STATUS_FINDMODELS_SUMMARY, My.Resources.CAPTION, iNumFound, nTrials), eMessageImportance.Warning)
        Else
            Me.InformUser(String.Format(My.Resources.STATUS_FINDMODELS_SUMMARY, My.Resources.CAPTION, iNumFound, nTrials), eMessageImportance.Information)
        End If

    End Sub

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
                writer.Write(cStringUtils.ToCSVField(mCore.EcoPathGroupInputs(igrp).Name))
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

    Private Function getEcosimResults() As Boolean
        Try
            'Because we ran Ecosim directly from cEcosimModel.Run() instead of via the core cCore.RunEcosim()
            'the Core output objects cCore.EcoSimGroupOutputs() will not be populated
            'Instead get the Ecosim results directly from the underlying arrays
            Dim sumb() As Single
            ReDim sumb(mCore.nLivingGroups)
            For igrp As Integer = 1 To mCore.nLivingGroups
                'sum biomass over all the Ecosim timesteps
                For itime As Integer = 1 To mCore.nEcosimTimeSteps
                    'see cEcosimModel.PopulateResults() for how ResultsOverTime(var,group,time) are stored
                    sumb(igrp) += Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, itime)
                Next itime

                System.Console.WriteLine("Average Biomass for " & Me._ecopath.EcopathData.GroupName(igrp) & " = " & (sumb(igrp) / mCore.nEcosimTimeSteps).ToString)

            Next igrp

        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    Private Function InitMonteCarloParamX(ByVal strPath As String, ByVal ParamName As eParamName) As Boolean
        Dim csvParamX As CsvReader
        Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
        Dim MCGroup As cMonteCarloGroup
        Dim xgrp As Integer

        Try

            If Not CheckEcopathDistributionFilesOkay(strPath) Then Return False

            csvParamX = New CsvReader(New StreamReader(strPath), True) ' I think this is to restart the reading of the csv

            For igrp = 1 To mCore.nLivingGroups

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

        'Dim csv_B, csv_PB, csv_QB, csv_EE, csv_BA As CsvReader
        Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
        'Dim MCGroup As cMonteCarloGroup
        'Dim xgrp As Integer
        'Initialize Monte Carlo parameters for B, PB, QB, EE and BA
        'These are the group parameters in the EwE Monte Carlo runs form
        'CV Lower and Upper Limit

        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "B_Dist.csv"), eParamName.B) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "PB_Dist.csv"), eParamName.PB) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "QB_Dist.csv"), eParamName.QB) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "EE_Dist.csv"), eParamName.EE) Then Return False
        If Not InitMonteCarloParamX(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, "BA_Dist.csv"), eParamName.BA) Then Return False

        'csv_B = New CsvReader(New StreamReader(Path & "/B_Dist.csv"), True)
        'csv_PB = New CsvReader(New StreamReader(Path & "/PB_Dist.csv"), True)
        'csv_QB = New CsvReader(New StreamReader(Path & "/QB_Dist.csv"), True)
        'csv_EE = New CsvReader(New StreamReader(Path & "/EE_Dist.csv"), True)
        'csv_BA = New CsvReader(New StreamReader(Path & "/BA_Dist.csv"), True)

        'If Not CheckEcopathDistributionFilesOkay(csv_B, "biomass") Then Return False
        'If Not CheckEcopathDistributionFilesOkay(csv_PB, "production/biomass") Then Return False
        'If Not CheckEcopathDistributionFilesOkay(csv_QB, "consumption/biomass") Then Return False
        'If Not CheckEcopathDistributionFilesOkay(csv_EE, "ecotrophic efficiency") Then Return False
        'If Not CheckEcopathDistributionFilesOkay(csv_BA, "biomass accumulation") Then Return False

        'csv_B = New CsvReader(New StreamReader(Path & "/B_Dist.csv"), True)
        'csv_PB = New CsvReader(New StreamReader(Path & "/PB_Dist.csv"), True)
        'csv_QB = New CsvReader(New StreamReader(Path & "/QB_Dist.csv"), True)
        'csv_EE = New CsvReader(New StreamReader(Path & "/EE_Dist.csv"), True)
        'csv_BA = New CsvReader(New StreamReader(Path & "/BA_Dist.csv"), True)

        'Set the cv values first ==================================================================================================================
        'For igrp = 1 To mCore.nLivingGroups

        '    xgrp = 1
        '    csv_B.ReadNextRecord()
        '    csv_BA.ReadNextRecord()
        '    csv_EE.ReadNextRecord()
        '    csv_PB.ReadNextRecord()
        '    csv_QB.ReadNextRecord()

        '    'Make sure that .csv files are set up with group names in same order because xgrp is found only from the B file
        '    'and then is assumed to be the same for all other files
        '    While MonteCarlo.Groups(xgrp).Name <> csv_B(1) 'And xgrp <= mCore.nLivingGroups
        '        xgrp += 1
        '    End While

        '    MCGroup = MonteCarlo.Groups(xgrp)

        '    'Setting a CV value will automatically set the Lower and Upper limits
        '    'by Calling cEcosimMonteCarlo.CalculateUpperLowerLimits()
        '    'If you want to manually set limits it must be done after the CV has been set

        '    'CVs
        '    MCGroup.Bcv = csv_B(2)
        '    MCGroup.PBcv = csv_PB(2)
        '    MCGroup.QBcv = csv_QB(2)
        '    MCGroup.EEcv = csv_EE(2)
        '    MCGroup.BAcv = csv_BA(2)

        '    'LowerBounds
        '    MCGroup.BLower = csv_B(3)
        '    MCGroup.PBLower = csv_PB(3)
        '    MCGroup.QBLower = csv_QB(3)
        '    MCGroup.EELower = csv_EE(3)
        '    MCGroup.BALower = csv_BA(3)

        '    'UpperBounds
        '    MCGroup.BUpper = csv_B(4)
        '    MCGroup.PBUpper = csv_PB(4)
        '    MCGroup.QBUpper = csv_QB(4)
        '    MCGroup.EEUpper = csv_EE(4)
        '    MCGroup.BAUpper = csv_BA(4)

        'Next '========================================================================================================================================================

        ''reset the connection to the csv files ready to be read from the beginning again
        'csv_B.Dispose()
        'csv_BA.Dispose()
        'csv_EE.Dispose()
        'csv_PB.Dispose()
        'csv_QB.Dispose()

        Return True

        'Catch ex As Exception
        '    Debug.Assert(False, Me.ToString & ".InitMonteCarloParameters() Exception: " & ex.Message)
        'End Try

    End Function

    ''' <summary>
    ''' Save any variable that will be changed so the model can be restore to it's original state 
    ''' </summary>
    ''' <remarks>This just stores a sub set of variable as an example</remarks>
    Private Sub SaveOriginalState()
        Try
            'Have the MonteCarloManager save the values it will alter
            mCore.EcosimMonteCarlo.SaveOriginalValues()

            'Now store the variables that this app will change so they can be restored in RestoreOriginalState()

            'The makes sure Ecopath does not make a fuss, popping up message boxes, when it fails to balance a model
            Me._ecopath.suppressMessages = True

            'Make sure nothing is listening to Ecosim when we run it
            Me._EcosimTimeStepDelegate = Me._ecosim.TimeStepDelegate
            Me._ecosim.TimeStepDelegate = Nothing

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

    'This is just a diagnostics routine that outputs to console the Biomass for each living group at a particular iteration
    Private Sub dumpEcopathParameters(ByVal iteration As Integer)
        Dim nliving As Integer = Me.mCore.nLivingGroups
        Dim MonteCarlo As cMonteCarloManager = Me.mCore.EcosimMonteCarlo

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

            'make sure Ecosim computes the output data
            Me._ecosim.EcosimData.bTimestepOutput = True

            'No timestep call back
            Me._ecosim.TimeStepDelegate = Nothing

            'Run on the same thread 
            'this means Me._ecosim.Run() will block until Ecosim has finished running
            Me._ecosim.EcosimData.bMultiThreaded = False

            'Run Ecosim without Core support 
            'This means Core Input/ouput objects will not be populate 
            'So you can not use cCore.EcoSimGroupOutputs() to retrieve the results
            Me._ecosim.Init(True)
            Return Me._ecosim.Run()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RunEcosim() Exception: " & ex.Message)
        End Try

        Return False

    End Function

    ''' <summary>
    ''' Restore the currently loaded model back to its original state so that it can be run in the interface.
    ''' </summary>
    ''' <remarks>In some cases you may want to save changes you made to the model.</remarks>
    Private Sub RestoreOriginalState()
        Try

            Dim iscenario As Integer = Me.mCore.ActiveEcosimScenarioIndex

            'Have the MonteCarloManager restore it's variables to the original state
            Me.RestoreParameters()
            Me._ecosim.TimeStepDelegate = Me._EcosimTimeStepDelegate

            ' No database changes left, yippee
            Me.Core.DiscardChanges()

            ' Just reload Ecosim
            Core.CloseEcosimScenario()

            Me.Core.LoadEcosimScenario(iscenario)

        Catch ex As Exception

        End Try

    End Sub

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return My.Resources.CAPTION_TOOLTIP
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            'Return EwEUtils.Core.eCoreExecutionState.EcosimCompleted
            Return EwEUtils.Core.eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Mark Platts CEFAS"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevlowestoft@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in to run CEFAS MSE"
        End Get
    End Property

    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Me.mCore = CType(core, cCore)
        Units.Init(mCore)
    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndCefasMSE"
        End Get
    End Property

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized

        _ecopath = CType(objEcoPath, Ecopath.cEcoPathModel)
        _ecosim = CType(objEcoSim, Ecosim.cEcoSimModel)

        Debug.Assert(Me.m_uic IsNot Nothing)

        ' Set message handlers

        Me.m_mhSettings = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.Core, eMessageType.GlobalSettingsChanged, Me.m_uic.SyncObject)
        Me.m_mhEcosim = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.EcoSim, eMessageType.DataAddedOrRemoved, Me.m_uic.SyncObject)

#If DEBUG Then
        Me.m_mhSettings.Name = "CefasMSE_mhSettings"
        Me.m_mhEcosim.Name = "CefasMSE_mhEcosim"
#End If

    End Sub

    Public Sub UIContext(ByVal uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Public Sub EcosimInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        Debug.Assert(TypeOf EcosimDatastructures Is cEcosimDatastructures, "EcosimInitialized() failed to pass in valid Ecosim Data!")
        If TypeOf EcosimDatastructures Is cEcosimDatastructures Then
            _simdata = DirectCast(EcosimDatastructures, cEcosimDatastructures)
        End If
    End Sub

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        If Not Me.HasUI Then
            MSEForm = New frmMSE(Me, Me.m_uic, Me.m_Survivability)
        End If

        ' Let EwE show the form
        frmPlugin = MSEForm

    End Sub

    Private Function HasUI() As Boolean
        If Me.MSEForm Is Nothing Then Return False
        Return Not Me.MSEForm.IsDisposed
    End Function

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

    Public Function GenerateEcosimParameters(ByVal ParamName As String) As Boolean

        Dim strPath As String = cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.DistrParams, ParamName & ".csv")

        If Not CheckEcoSimDistributionFilesOkay(strPath) Then Return False

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim ParameterArray(mCore.nLivingGroups - 1, 3) As Single

        ' JS 30Sep13: Use local properties
        Dim nModels As Integer = Me.NModels
        Dim eDistributionType As DistributionType
        Dim SampledParameters(nModels - 1, mCore.nLivingGroups - 1) As Double
        Dim GroupNames(mCore.nLivingGroups - 1) As String

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
        For iGroup = 1 To mCore.nLivingGroups

            If Not ParameterArray(iGroup - 1, 1) = cCore.NULL_VALUE Then

                eDistributionType = CType(ParameterArray(iGroup - 1, 0), DistributionType)

                For iIteration = 1 To nModels

                    Select Case eDistributionType
                        Case DistributionType.Uniform
                            SampledParameters(iIteration - 1, iGroup - 1) = UniformSample(ParameterArray(iGroup - 1, 1), ParameterArray(iGroup - 1, 2))
                        Case DistributionType.Triangular
                            SampledParameters(iIteration - 1, iGroup - 1) = TriangularSample(ParameterArray(iGroup - 1, 1), ParameterArray(iGroup - 1, 2), ParameterArray(iGroup - 1, 3))
                    End Select

                Next
            Else
                For iIteration = 1 To nModels
                    SampledParameters(iIteration - 1, iGroup - 1) = cCore.NULL_VALUE
                Next
            End If

        Next

        'Output the sampled parameters to a csv
        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, ParamName & "_out.csv"))
        If (writer IsNot Nothing) Then
            Try
                For igrp As Integer = 1 To mCore.nLivingGroups
                    If (igrp > 1) Then writer.Write(",")
                    writer.Write(cStringUtils.ToCSVField(GroupNames(igrp - 1)))
                Next
                writer.WriteLine()

                For iIteration = 1 To nModels
                    For iGroup = 1 To mCore.nLivingGroups
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
    Public Sub CreateVulnerabilities()

        ' JS 13Oct13: Fixed path usage
        ' JS 13Oct13: Used standard CSV field reading/writing
        ' JS 30Sep13: Used persistent properties

        Dim writer As StreamWriter = Nothing
        Dim nIterations As Integer = NModels

        For iIteration = 1 To nIterations

            writer = cMSEUtils.GetWriter(cMSEUtils.MSEFile(DataPath, cMSEUtils.eMSEPaths.ParamsOut, "VulnerabilityIteration" & iIteration & "_out.csv"), False)
            If (writer IsNot Nothing) Then
                'Create random values for the vulnerabilities and store in a csv
                For igrppredator As Integer = 1 To _ecopath.EcopathData().NumLiving
                    If mCore.EcoPathGroupInputs(igrppredator).IsProducer Then
                        For igrpprey As Integer = 1 To _ecopath.EcopathData().NumGroups
                            If (igrpprey > 1) Then writer.Write(",")
                            writer.Write(Convert.ToSingle(cCore.NULL_VALUE))
                        Next igrpprey
                    Else
                        For igrpprey As Integer = 1 To _ecopath.EcopathData().NumGroups
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

    Private Function CalcFfromHCR(ByRef Biomass As Single, ByRef MinBiomass As Single, ByRef MaxBiomass As Single, ByRef FMax As Single) As Double

        If Biomass > MaxBiomass Then
            Return Convert.ToDouble(FMax)
        ElseIf Biomass < MinBiomass Then
            Return 0
        Else
            Return Convert.ToDouble(((Biomass - MinBiomass) / (MaxBiomass - MinBiomass)) * FMax)
        End If

    End Function

    Public Function DetermineZeroEffortFleets(ByRef FTargCons(,) As Double) As Integer()
        Dim ZeroEffortFleets As New List(Of Integer)

        For iGrp = 1 To mCore.nGroups
            If FTargCons(iGrp - 1, HCRType.Conservation) = 0 Then
                For iFleet = 1 To mCore.nFleets
                    If mCore.FleetInputs(iFleet).Landings(iGrp) + mCore.FleetInputs(iFleet).Discards(iGrp) > 0 And _
                        Not ZeroEffortFleets.Contains(iFleet) Then
                        ZeroEffortFleets.Add(iFleet)
                    End If
                Next
            End If
        Next
        Return ZeroEffortFleets.ToArray

    End Function

    Public Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcosimBeginTimestepPlugin.EcosimBeginTimeStep

        ' JS 13Oct13: Fixed CurDir vulnerability in lpsolve
        ' JS 13Oct13: Globalized this method
        ' JS 13Oct13: Fixed path usage
        ' JS 13Oct13: Removed MsgBox

        Dim TechnologyCreep(mCore.nFleets) As Single 'an array where each element represents the percentage with which each fleet increases its catching efficiency each year
        'Must have nfleets+1 elements so for 10 fleets needs elements 0-10
        'This is because of the way code works in EwE
        Dim TargetF(mCore.nGroups) As Double
        Dim CostFunctionType(mCore.nGroups) As String
        Dim mincost As Double = 1000000
        Dim Fleets2Fit As List(Of Integer) = New List(Of Integer)
        Dim QMult(_ecosim.EcosimData.nGroups) As Double
        Dim tempFConservation As Double
        'used so that we don't repeat same groups when cycling through HCRs
        Dim LastYearsEffort(_ecosim.EcopathData.NumFleet - 1) As Double
        Dim variable_results() As Double


        If ChangeEffortFlag = True Then 'Flag is only set to true when the button on the form is clicked
            'this is so that its only executed when ecosim is run from mseform

            If iTime > OriginalNTimesteps And (iTime - 1) Mod 12 = 0 Then

                'Initialise the TechnologyCreep array. Change this variable into a class level variable and set it only once.
                For iTechCreep As Integer = 1 To TechnologyCreep.Length - 1
                    TechnologyCreep(iTechCreep) = 1
                Next

                'determine what the target F should be from the HCR's and current biomass levels for each group
                'count number of unique groups to calc F for in strategy
                'For iGrp = 1 To mCore.nLivingGroups
                '    For Each i In CurrentStrategy
                '        If i.GroupNumber4F = iGrpF Then
                '            countFGroups += 1
                '            Exit For
                '        End If
                '    Next
                'Nextt

                Dim FTargetandConservation(mCore.nGroups - 1, 1) As Double
                For i = 1 To mCore.nGroups
                    FTargetandConservation(i - 1, HCRType.Target) = NoHCR_F
                    FTargetandConservation(i - 1, HCRType.Conservation) = NoHCR_F
                Next

                For Each iHCRGroup In CurrentStrategy
                    Select Case iHCRGroup.CostFunction
                        Case HCRType.Target
                            If FTargetandConservation(iHCRGroup.GroupF.Index - 1, HCRType.Target) = NoHCR_F Then
                                FTargetandConservation(iHCRGroup.GroupF.Index - 1, HCRType.Target) = CalcFfromHCR(BiomassAtTimestep(iHCRGroup.GroupB.Index), 0, CSng(iHCRGroup.UpperLimit), CSng(iHCRGroup.MaxF))
                            Else
                                Me.InformUser(String.Format(My.Resources.ERROR_HARVESTRUILE_DUPLICATE_F, iHCRGroup.GroupF.Name), eMessageImportance.Warning)
                            End If
                        Case HCRType.Conservation
                            tempFConservation = CalcFfromHCR(BiomassAtTimestep(iHCRGroup.GroupB.Index), CSng(iHCRGroup.LowerLimit), CSng(iHCRGroup.UpperLimit), CSng(iHCRGroup.MaxF))
                            If tempFConservation < FTargetandConservation(iHCRGroup.GroupF.Index - 1, HCRType.Conservation) Or FTargetandConservation(iHCRGroup.GroupF.Index - 1, HCRType.Conservation) = NoHCR_F Then
                                FTargetandConservation(iHCRGroup.GroupF.Index - 1, HCRType.Conservation) = tempFConservation
                            End If
                        Case Else
                            ' This state is not possible, but good to add a safe catch anyway ;)
                            Debug.Assert(False)
                    End Select
                Next

                'Compiles a list of which fleets are affecting groups which have a zero conservation f
                'ZeroEffortFleetsList = DetermineZeroEffortFleets(FTargetandConservation)

                'Checks to see whether each fleet effects a group that has an HCR - if yes it adds it to fleets2fit so that we know what fleets to
                'change the effort for to try and achieve the target F's for each group
                For iFleet As Integer = 1 To mCore.nFleets
                    For Each iHCRGroup In CurrentStrategy
                        If mCore.FleetInputs(iFleet).Landings(iHCRGroup.GroupF.Index) + mCore.FleetInputs(iFleet).Discards(iHCRGroup.GroupF.Index) > 0 Then
                            'If Not Fleets2Fit.Contains(iFleet) And Not ZeroEffortFleetsList.Contains(iFleet) Then
                            If Not Fleets2Fit.Contains(iFleet) Then
                                Fleets2Fit.Add(iFleet)
                            End If
                            Exit For
                        End If
                    Next
                Next

                'if there are no fleets to optimise for skip all this
                If Fleets2Fit.Count > 0 Then

                    'Not quite sure what QMult is but it is needed to calculate what F is in the optimised routine
                    For indexgrp As Integer = 1 To _ecosim.EcosimData.nGroups
                        QMult(indexgrp - 1) = _ecosim.EcosimData.QmQo(indexgrp) / (1 + (_ecosim.EcosimData.QmQo(indexgrp) - 1) * BiomassAtTimestep(indexgrp) / _ecosim.EcosimData.StartBiomass(indexgrp))
                    Next

                    'All the variables that are used by the optimisation routine
                    'Dim x(Fleets2Fit.Count - 1) As Double
                    'Dim epsg As Double = 0.0000000001
                    'Dim epsf As Double = 0
                    'Dim epsx As Double = 0
                    'Dim diffstep As Double = 0.000001
                    'Dim maxits As Integer = 0
                    'Dim state As minlbfgsstate = New XAlglib.minlbfgsstate() ' initializer can be dropped, but compiler will issue warning
                    'Dim rep As minlbfgsreport = New XAlglib.minlbfgsreport() ' initializer can be dropped, but compiler will issue warning
                    'Dim parameters = New List(Of Object)
                    'Dim AllEffortsAboveZero As Boolean
                    'Dim BestCost As Double = 0 'this holds the cost value of best fit so far for comparison with current iteration
                    'Dim BestEfforts(Fleets2Fit.Count - 1) As Double
                    ' if cost improves on current iteration replace all the saved effort values with current effort values

                    'New Linear programming method - ideally needs placing in its own sub

                    Dim lpsolve As New cLPSolver.lpsolve55
                    Dim lp As Integer
                    Dim solution_return_value As Integer

                    'Count how many groups we are setting target F's for
                    Dim countGroupsFTarget As Integer = 0
                    Dim countGroupsFConservation As Integer = 0
                    For i = 1 To mCore.nGroups
                        If FTargetandConservation(i - 1, HCRType.Target) <> NoHCR_F Then
                            countGroupsFTarget += 1
                        End If
                        If FTargetandConservation(i - 1, HCRType.Conservation) <> NoHCR_F Then
                            countGroupsFConservation += 1
                        End If
                    Next

                    cLPSolver.lpsolve55.Init()

                    lp = cLPSolver.lpsolve55.make_lp(0, mCore.nFleets + countGroupsFTarget * 2 + countGroupsFConservation * 2)
                    cLPSolver.lpsolve55.set_outputfile(lp, Path.Combine(DataPath, "result_lin_prog_MSE" & iTime & ".txt"))

                    cLPSolver.lpsolve55.set_timeout(lp, 0)

                    'Set up the factors to go with the effort of each fleet to work out what the fishing mortality is
                    Dim Constraints(mCore.nFleets + countGroupsFTarget * 2 + countGroupsFConservation * 2) As Double
                    Dim indexGroupConstraint = 0
                    Dim ObjectiveFunction(mCore.nFleets + countGroupsFTarget * 2 + countGroupsFConservation * 2) As Double
                    'Create constraints for f_targets
                    For iGrp = 1 To mCore.nGroups
                        If FTargetandConservation(iGrp - 1, HCRType.Target) <> NoHCR_F Then
                            Array.Clear(Constraints, 0, Constraints.Length)
                            For iFleet = 1 To mCore.nFleets
                                Constraints(iFleet) = TechnologyCreep(iFleet) * _ecosim.EcosimData.FishMGear(iFleet, iGrp) * (_ecosim.EcosimData.PropLandedTime(iFleet, iGrp) + _ecosim.EcosimData.Propdiscardtime(iFleet, iGrp)) * QMult(iGrp - 1)
                            Next
                            Constraints(mCore.nFleets + indexGroupConstraint * 2 + 1) = 1
                            Constraints(mCore.nFleets + indexGroupConstraint * 2 + 2) = -1
                            indexGroupConstraint += 1
                            cLPSolver.lpsolve55.print_str(lp, "Contraint for group: " & iGrp)
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.EQ, FTargetandConservation(iGrp - 1, HCRType.Target))
                            'cLPSolver.lpsolve55.print_lp(lp)
                        End If

                    Next
                    'Create constriants for f_conservations
                    indexGroupConstraint = 0
                    For iGrp = 1 To mCore.nGroups
                        If FTargetandConservation(iGrp - 1, HCRType.Conservation) <> NoHCR_F Then
                            Array.Clear(Constraints, 0, Constraints.Length)
                            For iFleet = 1 To mCore.nFleets
                                Constraints(iFleet) = TechnologyCreep(iFleet) * _ecosim.EcosimData.FishMGear(iFleet, iGrp) * (_ecosim.EcosimData.PropLandedTime(iFleet, iGrp) + _ecosim.EcosimData.Propdiscardtime(iFleet, iGrp)) * QMult(iGrp - 1)
                            Next
                            'If iTime = 217 Then
                            '    Console.WriteLine("The group is : " & iGrp)
                            '    Stop
                            'End If
                            Constraints(mCore.nFleets + countGroupsFTarget * 2 + indexGroupConstraint * 2 + 1) = 1000
                            Constraints(mCore.nFleets + countGroupsFTarget * 2 + indexGroupConstraint * 2 + 2) = -1000
                            indexGroupConstraint += 1
                            cLPSolver.lpsolve55.print_str(lp, "Contraint for group: " & iGrp)
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.LE, FTargetandConservation(iGrp - 1, HCRType.Conservation))
                        End If
                    Next

                    'set up the constraints so that change in effort cannot be > some limit
                    For Each iFleet In Fleets2Fit
                        Array.Clear(Constraints, 0, Constraints.Length)
                        Constraints(iFleet) = 1
                        If _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1) = 0 Then
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.GE, 0)
                        Else
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.GE, _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1) - ChangeInEffortLimits(iFleet - 1) * _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1))
                        End If

                    Next

                    'These are all the constants that are added to the end of each simulataneous equation which allows for error between
                    'the actual and target f's - these are to be minimised
                    For i = 1 To countGroupsFTarget
                        ObjectiveFunction(mCore.nFleets + (i - 1) * 2 + 1) = 1
                        ObjectiveFunction(mCore.nFleets + (i - 1) * 2 + 2) = 1
                    Next
                    For i = 1 To countGroupsFConservation
                        ObjectiveFunction(mCore.nFleets + countGroupsFTarget * 2 + (i - 1) * 2 + 1) = 1
                        ObjectiveFunction(mCore.nFleets + countGroupsFTarget * 2 + (i - 1) * 2 + 2) = 1
                    Next

                    cLPSolver.lpsolve55.set_obj_fn(lp, ObjectiveFunction)
                    cLPSolver.lpsolve55.print_lp(lp)
                    cLPSolver.lpsolve55.set_minim(lp)

                    solution_return_value = cLPSolver.lpsolve55.solve(lp)
                    If solution_return_value <> 0 Then
                        Me.InformUser(String.Format(My.Resources.ERROR_LPSOLVE, solution_return_value), eMessageImportance.Warning)
                    End If

                    cLPSolver.lpsolve55.print_str(lp, solution_return_value & ": " & cLPSolver.lpsolve55.get_objective(lp) & cStringUtils.vbLf)

                    cLPSolver.lpsolve55.print_objective(lp)
                    cLPSolver.lpsolve55.print_solution(lp, 1)
                    cLPSolver.lpsolve55.print_constraints(lp, 1)

                    ReDim variable_results(cLPSolver.lpsolve55.get_Ncolumns(lp))
                    cLPSolver.lpsolve55.get_variables(lp, variable_results)

                    'Set the fishing effort according to what the optimised efforts were just calculated
                    For iFleet = 1 To mCore.nFleets

                        If Fleets2Fit.IndexOf(iFleet) <> -1 Then 'If fleet doesnt effect a group that has a HCR set effort to what it was end of last year
                            For iMonth = 1 To 12
                                _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = CSng(variable_results(iFleet - 1))
                            Next
                        End If
                    Next

                End If

                For iFleet = 1 To mCore.nFleets
                    If Fleets2Fit.IndexOf(iFleet) = -1 Then
                        For iMonth = 1 To 12
                            _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1)
                            '_ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = 2
                        Next
                    End If
                Next

                'Calculates what the F's are for each species given the effort
                For iMonth = 0 To 11
                    _ecosim.SetFtimeFromGear(Nothing, iTime + iMonth, TechnologyCreep, True)
                Next

            End If
        End If
    End Sub

    Friend ReadOnly Property Core As cCore
        Get
            Return Me.mCore
        End Get
    End Property

    'Private Sub CalculateFError(ByRef eps() As Double)
    '    Dim Fopt(mCore.nGroups - 1) As Double
    '    For iGrp As Integer = 1 To mCore.nLivingGroups
    '        Fopt(iGrp - 1) = 0
    '        For iFleet As Integer 1 to mCore.nFleets
    '            Fopt(iGrp - 1) = Fopt(iGrp - 1) + (mCore.FleetInputs(iFleet).Landings(iGrp) + mCore.FleetInputs(iFleet).Discards(iGrp)) * eps(iGrp - 1)
    '        Next
    '    Next
    'End Sub

#Region " Helper methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a name and index to a <see cref="cEcoPathGroupInput"/> instance.
    ''' </summary>
    ''' <param name="strName">The name to resolve.</param>
    ''' <param name="iIndex">The index to resolve.</param>
    ''' <returns>A <see cref="cEcoPathGroupInput"/> instance, or Nothing if
    ''' the index or name did not match any of the present groups.</returns>
    ''' <remarks>Note that name comparison is not case sensitive.</remarks>
    ''' -----------------------------------------------------------------------
    Private Function ResolveGroup(strName As String, iIndex As Integer) As cEcoPathGroupInput
        If (iIndex < 1) Or (iIndex > Me.Core.nGroups) Then Return Nothing
        Dim grp As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iIndex)
        If String.Compare(grp.Name, strName, True) <> 0 Then
            Return Nothing
        End If
        Return grp
    End Function

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

        Dim msg As New cMessage(strMessage, eMessageType.Any, eCoreComponentType.External, importance)
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

        Dim fmsg As New cFeedbackMessage(strMessage, eCoreComponentType.External, eMessageType.Any, importance, style)
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
            Me.InvalidateConfiguration()
        End If

    End Sub

    Private Sub onPreProcessMessage(ByVal msg As EwEUtils.Core.IMessage, ByRef bCancelMessage As Boolean) _
        Implements EwEPlugin.IMessageFilterPlugin.PreProcessMessage

        ' JS 03Oct13: ONLY SUPPRESS MESSAGES WHEN MSE IS RUNNING! 
        If Not Me.IsRunning Then Return

        'Plugin Point called to cancel a message
        Select Case msg.Type

            Case eMessageType.Estimate_BA, _
                 eMessageType.Estimate_Net_Migration, _
                 eMessageType.EE
                Console.WriteLine("! MSE suppressed message " & msg.Message)
                bCancelMessage = True

            Case Else
                bCancelMessage = False

        End Select

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
                Me.InvalidateConfiguration()
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
                Me.InvalidateConfiguration()
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
                Me.InvalidateConfiguration()
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
                Me.InvalidateConfiguration()
            End If
        End Set
    End Property

#End Region ' Configurable settings


    Public Sub CreateRCode()
        Dim writer As StreamWriter = New StreamWriter("C:\Users\Mark\Desktop\vbcreatedRcode.txt", False)
        Dim bSuccess As Boolean = False
        Dim firstpreyfound As Boolean
        Dim tempname As String

        For iPred = 1 To mCore.nLivingGroups
            tempname = mCore.EcoPathGroupInputs(iPred).Name
            tempname = tempname.Replace(" ", "")
            writer.Write("dietmeans[[" & iPred & "]] = c(")
            firstpreyfound = False
            For iPrey = 1 To mCore.nGroups
                If mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) <> 0 Then
                    If firstpreyfound = False Then
                        writer.Write(mCore.EcoPathGroupInputs(iPred).DietComp(iPrey))
                        firstpreyfound = True
                    Else
                        writer.Write(", " & mCore.EcoPathGroupInputs(iPred).DietComp(iPrey))
                    End If
                End If
            Next
            writer.WriteLine(")")

            writer.Write("preynames[[" & iPred & "]] = c(")
            firstpreyfound = False
            For iPrey = 1 To mCore.nGroups
                If mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) <> 0 Then
                    If firstpreyfound = False Then
                        writer.Write("""" & mCore.EcoPathGroupInputs(iPrey).Name & """")
                        firstpreyfound = True
                    Else
                        writer.Write(", """ & mCore.EcoPathGroupInputs(iPrey).Name & """")
                    End If
                End If
            Next
            writer.WriteLine(")")

        Next

        writer.Write("prednames = c(")
        For iPred = 1 To mCore.nLivingGroups
            If iPred = 1 Then
                writer.Write("""" & mCore.EcoPathGroupInputs(iPred).Name & """")
            Else
                writer.Write(", """ & mCore.EcoPathGroupInputs(iPred).Name & """")
            End If
        Next
        writer.WriteLine(")")

        cMSEUtils.ReleaseWriter(writer)

    End Sub

End Class
