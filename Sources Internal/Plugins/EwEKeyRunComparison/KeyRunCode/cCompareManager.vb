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
Imports System.IO
Imports System.Reflection
Imports EwEUtils.Utilities
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Controls
Imports System.Text

#End Region ' Imports

Public Class cCompareManager

#Region " Private variables "

    ''' <summary>Hashed values read from a keyrun file</summary>
    Private m_dctKeyRunValues As Dictionary(Of String, cHashValues) = Nothing
    Private m_dctCurValues As Dictionary(Of String, cHashValues) = Nothing

    Private m_lSummarizers As List(Of IHashSummarizer) = Nothing
    Private m_strKeyRunFile As String = ""

    Private m_Results As cHashResults = Nothing

#End Region ' Private variables

#Region " Construction initialization "

    Public Sub New(ByVal uic As cUIContext,
                   ByVal PathData As cEcopathDataStructures,
                   ByVal SimData As cEcosimDatastructures,
                   ByVal SpaceData As cEcospaceDataStructures)

        Me.UIContext = uic
        Me.Core = uic.Core
        Me.EcopathData = PathData
        Me.EcosimData = SimData
        Me.EcoSpaceData = SpaceData

        Me.m_dctCurValues = New Dictionary(Of String, cHashValues)
        Me.m_dctKeyRunValues = New Dictionary(Of String, cHashValues)
        Me.m_lSummarizers = New List(Of IHashSummarizer)
        Me.Messages = New List(Of String)

    End Sub

    Private ReadOnly Property UIContext As cUIContext

#End Region ' Construction initialization

#Region " Public properties "

    Public ReadOnly Property Core As cCore = Nothing

    Public ReadOnly Property EcopathData As cEcopathDataStructures = Nothing

    Public ReadOnly Property EcosimData As cEcosimDatastructures = Nothing

    Public ReadOnly Property EcoSpaceData As cEcospaceDataStructures = Nothing

    Public ReadOnly Property Results As cHashResults
        Get
            Return Me.m_Results
        End Get
    End Property

    Public ReadOnly Property KeyRunFile As String
        Get
            Return Me.m_strKeyRunFile
        End Get
    End Property

    Public ReadOnly Property Messages As New List(Of String)

#End Region ' Public properties

#Region " Public methods "

    ''' <summary>
    ''' Returns the default key run file name for a given model file.
    ''' </summary>
    ''' <returns>The default key run file name for a given model file.</returns>
    ''' <remarks>
    ''' It is safe to assume that there will be only one key run for a given model version.
    ''' For ease of use the key run file name should be identical to the model name.
    ''' </remarks>
    Public Function DefaultKeyRunFileName() As String

        Dim sbFile As New StringBuilder()
        sbFile.Append(Path.GetFileNameWithoutExtension(Me.Core.DataSource.ToString()))

        '' Append author name
        'If (Not String.IsNullOrWhiteSpace(Me.m_core.DefaultAuthor)) Then
        '    sbFile.Append("^")
        '    sbFile.Append(Me.m_core.DefaultAuthor)
        'End If

        sbFile.Append(".ewekeyrun")

        Return cFileUtils.ToValidFileName(sbFile.ToString(), False)

    End Function

    Public Function DefaultKeyRunFileLocation() As String
        Return Path.GetDirectoryName(Me.Core.DataSource.ToString())
    End Function

    Public Function LoadKeyRun(strFileName As String) As Boolean
        Dim bSuccess As Boolean = False

        Me.ResetErrors()

        If ReadKeyRunFile(strFileName) Then
            If PopulateCurrentModel() Then
                If Me.CompareRuns() Then
                    bSuccess = True
                End If
            End If
        End If

        If (bSuccess) Then
            Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_LOAD_SUCCESS, strFileName))
        Else
            Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_LOAD_FAILED, strFileName))
        End If

        Me.NotifyUI()

        Return bSuccess

    End Function

    Public Function RunLoadedKeyRun() As Boolean
        Dim bSuccess As Boolean = False

        ' ToDo: globalize this

        Me.ResetErrors()

        If File.Exists(Me.m_strKeyRunFile) Then
            If PopulateCurrentModel() Then
                If Me.CompareRuns() Then
                    bSuccess = True
                End If
            End If

            If (bSuccess) Then
                Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_LOAD_SUCCESS, Me.m_strKeyRunFile))
            Else
                Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_LOAD_FAILED, Me.m_strKeyRunFile))
            End If

        Else

            Me.AddError("Invalid key run file. You must have a valid key run file loaded first.")
            Me.SendMessage("Sorry unable to run current key run file.")

        End If 'File.Exists(Me.m_strKeyRunFile)

        Me.NotifyUI()

        Return bSuccess

    End Function

    Public Function SaveKeyRunFile(strFileName As String) As Boolean

        Dim bSuccess As Boolean = False

        Me.ResetErrors()

        If Me.PopulateCurrentModel() Then
            bSuccess = Me.SaveCurModelToFile(strFileName)
        End If

        If (bSuccess) Then
            Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_SAVE_SUCCESS, strFileName),
                           Path.GetDirectoryName(strFileName))
        Else
            Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_SAVE_FAILED, strFileName))
        End If

        Me.NotifyUI()

        Return bSuccess

    End Function

    Public Sub Invalidate()

        ' Do not kill the results because this seriously hampers troubleshooting
        ' Just invalidate any comparison
        If (Me.m_Results IsNot Nothing) Then Me.m_Results.Invalidate()

        Me.NotifyUI()

    End Sub

    Public Event OnChanged(man As cCompareManager)

#End Region ' Public methods

#Region " Private Methods "

    Private Function PopulateCurrentModel() As Boolean

        Dim bSuccess As Boolean = False

        Try

            Me.InitCurrentModel()
            Me.ComputeHashValues()
            bSuccess = True

        Catch ex As Exception
            Me.AddError(cStringUtils.Localize(My.Resources.DETAIL_COMPUTE_FAILED, ex.Message))
        End Try

        Return bSuccess

    End Function

    Private Function ReadKeyRunFile(strFileName As String) As Boolean

        Dim bSuccess As Boolean = False

        Me.m_strKeyRunFile = String.Empty

        Try

            Me.m_dctKeyRunValues.Clear()

            Dim strm As New StreamReader(strFileName)
            Dim hashVal As cHashValues
            Do While Not strm.EndOfStream
                Dim line As String
                line = strm.ReadLine()
                If cHashValues.IsHashRecord(line) Then
                    hashVal = New cHashValues()
                    If hashVal.FromRecordString(line) Then
                        Debug.Assert(Not Me.m_dctKeyRunValues.ContainsKey(hashVal.Key), "Oh my! You're trying to add a duplicate key to the key run dictionary.")
                        Me.m_dctKeyRunValues.Add(hashVal.Key, hashVal)
                    End If
                End If
            Loop

            strm.Close()

            Me.m_strKeyRunFile = strFileName
            bSuccess = True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Me.AddError(cStringUtils.Localize(My.Resources.DETAIL_KEYRUN_READ_FAILED, ex.Message))
        End Try

        Return bSuccess

    End Function

    Private Function CompareRuns() As Boolean

        ' This code works the wrong way around. We should:
        '  1. Load a key run file
        '  2. Read the number of digits from the key run file, and use this to generate the hash set for the current model
        '  3. Compare the two with that number of digits

        Dim bSuccess As Boolean = False
        Dim iNumDigits As Integer = Me.UIContext.StyleGuide.NumDigits

        Try
            Me.m_Results = New cHashResults(Me.m_strKeyRunFile, iNumDigits)

            Dim curHash As cHashValues
            Dim bMatch As Boolean
            For Each KeyRunHash As cHashValues In Me.m_dctKeyRunValues.Values
                bMatch = False
                If Me.m_dctCurValues.ContainsKey(KeyRunHash.Key) Then
                    curHash = Me.m_dctCurValues.Item(KeyRunHash.Key)
                    If curHash IsNot Nothing Then
                        If String.Compare(KeyRunHash.Hash, curHash.Hash) = 0 Then
                            bMatch = True
                        End If

                    End If
                Else
                    'Key Run HashValue not found in the current model
                    'Add a NULL current model hash value
                    curHash = Nothing
                End If 'Me.m_dctCurValues.ContainsKey(KeyRunHash.Key)

                Me.m_Results.Add(KeyRunHash, curHash, bMatch)

            Next KeyRunHash

            'Check for hash value in the current model not in the key run
            For Each CurRunHash As cHashValues In Me.m_dctCurValues.Values
                If Not Me.m_dctKeyRunValues.ContainsKey(CurRunHash.Key) Then
                    'Missing from the key run
                    'Add a NULL hash value for the key run
                    Me.m_Results.Add(Nothing, CurRunHash, False)
                End If 'Me.m_dctCurValues.ContainsKey(KeyRunHash.Key)
            Next CurRunHash

            bSuccess = True

        Catch ex As Exception
            Me.AddError(cStringUtils.Localize(My.Resources.DETAIL_COMPARE_FAILED, ex.Message))
        End Try

        Return bSuccess

    End Function

    Private Sub ComputeHashValues()

        Me.SendProgress("", eProgressState.Start, 0)

        Dim n As Integer = Me.m_lSummarizers.Count
        For i As Integer = 0 To n - 1

            Dim summarizer As IHashSummarizer = Me.m_lSummarizers(i)

            Try
                Me.SendProgress(cStringUtils.Localize(My.Resources.PROGRESS_HASHING, summarizer.Name), eProgressState.Running, CSng((i + 1) / n))
                For Each hash As cHashValues In summarizer.HashValues()
                    ' Catch possible coding issue of duplicate hash IDs
                    Debug.Assert(Not Me.m_dctCurValues.ContainsKey(hash.Key), "Oh my! You're trying to add a duplicate key to the current model dictionary.")
                    Me.m_dctCurValues.Add(hash.Key, hash)
                    ' System.Console.WriteLine(hash.SortOrder.ToString + ", " + hash.Component + ", " + hash.VariableID + ", " + hash.Hash + ", " + hash.Value)
                Next
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                'right now there is no way for IHashSummarizer to tell use what/who failed
                Me.AddError(cStringUtils.Localize(My.Resources.DETAIL_COMPUTE_HASH_FAILED, ex.Message))
            End Try
        Next

        Me.SendProgress("", eProgressState.Finished, 1)

    End Sub

    Private Sub SendProgress(strStatus As String, status As eProgressState, sProgress As Single)

        Dim pm As New cProgressMessage(status, 1, sProgress, strStatus)
        Me.Core.Messages.SendMessage(pm)

    End Sub

    Private Sub InitCurrentModel()

        Dim sm As cCoreStateMonitor = Me.Core.StateMonitor

        'reset sort order to zero
        cHashValues.ClearSort()
        'Clear out any old results
        Me.m_dctCurValues.Clear()
        ' Clear out hash objects 
        Me.m_lSummarizers.Clear()

        '-- Core Scenarios --
        m_lSummarizers.Add(New cCoreScenariosSummarizer(Me.Core))

        ' -- Ecopath --
        m_lSummarizers.Add(New cEcopathModelSummarizer(Me.Core))
        m_lSummarizers.Add(New cEcopathInputSummarizer(Me.Core))
        m_lSummarizers.Add(New cDietCompSummarizer(Me.Core))
        m_lSummarizers.Add(New cDetritusFateSummarizer(Me.Core))
        m_lSummarizers.Add(New cEcopathFleetDefinitionSummarizer(Me.Core))
        m_lSummarizers.Add(New cEcopathFleetSummarizer(Me.Core))

        m_lSummarizers.Add(New cEcopathDiscardFateSummarizer(Me.Core))

        ' -- Stanza --
        m_lSummarizers.Add(New cStanzaSummarizer(Me.Core))
        m_lSummarizers.Add(New cStanzaLifestageSummarizer(Me.Core))

        ' -- Ecosim --
        If (sm.HasEcosimLoaded) Then

            m_lSummarizers.Add(New cEcosimParametersSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimEnvForcingSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimForcingFunctionSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimInputSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimEffortSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimMortalitySummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimVulnerabilitiesSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimMediationSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimPriceElasticitySummarizer(Me.Core))
            m_lSummarizers.Add(New cTimeSeriesSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcosimFleetSizeDynamicsSummarizer(Me.Core))

        End If

        ' -- Ecospace --
        If (sm.HasEcospaceLoaded) Then

            m_lSummarizers.Add(New cEcospaceParametersSummarizer(Me.Core))
            m_lSummarizers.Add(New cCapacityCalTypeSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcospaceCapacitySummarizer(Me.Core))
            m_lSummarizers.Add(New cEcospaceHabitatSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcospaceDispersalSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcospaceFisherySummarizer(Me.Core))
            m_lSummarizers.Add(New cEcospaceFisheryHabitatSummarizer(Me.Core))
            m_lSummarizers.Add(New cEcospaceMapsSummarizer(Me.Core))
            m_lSummarizers.Add(New cSpatialTemporalConfigurationSummarizer(Me.Core))

        End If

        ' -- Give'r --
        For Each summarizer As IHashSummarizer In m_lSummarizers
            summarizer.Init()
        Next

    End Sub

    Private Function SaveCurModelToFile(strFileName As String) As Boolean

        Try
            Dim strm As New StreamWriter(strFileName)
            Dim an As AssemblyName = cAssemblyUtils.GetAssemblyName(Me.GetType())

            ' Write header info
            strm.WriteLine(Me.Core.DefaultFileHeader(eAutosaveTypes.Ecospace))
            ' Add plug-in version. 
            strm.WriteLine("KeyRunVersion," & cStringUtils.ToCSVField(cAssemblyUtils.GetVersion(an).ToString()))
            strm.WriteLine()

            For Each hash As cHashValues In Me.m_dctCurValues.Values
                strm.WriteLine(hash.ToRecordString())
            Next
            strm.Close()

            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Me.AddError(ex.Message)
        End Try

        Return False

    End Function

    Private Sub NotifyUI()
        Try
            RaiseEvent OnChanged(Me)
        Catch ex As Exception
            ' Whoah!
            Debug.Assert(False)
        End Try
    End Sub

#Region " Messaging "

    Private Sub ResetErrors()
        Me.Messages.Clear()
    End Sub

    Private Sub AddError(message As String)
        Me.Messages.Add(message)
    End Sub

    Private Sub SendMessage(ByVal strMessage As String, Optional strHyperlink As String = "")

        Dim msg As New cMessage(strMessage,
                                eMessageType.DataExport,
                                eCoreComponentType.External,
                                CType(cSystemUtils.IIF(Me.Messages.Count = 0, eMessageImportance.Information, eMessageImportance.Critical), eMessageImportance))
        msg.Hyperlink = strHyperlink

        For i As Integer = 0 To Me.Messages.Count - 1
            Dim vs As New cVariableStatus(eStatusFlags.OK, Me.Messages(i), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
            msg.Variables.Add(vs)
        Next

        Me.Core.Messages.SendMessage(msg)
        Me.ResetErrors()

    End Sub

#End Region ' Messaging

#End Region ' Private Methods

End Class

