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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
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

    Private m_core As EwECore.cCore
    Private m_EcopathData As EwECore.cEcopathDataStructures
    Private m_EcosimData As EwECore.cEcosimDatastructures
    Private m_EcospaceData As EwECore.cEcospaceDataStructures

    Private m_dctKeyRunValues As Dictionary(Of String, cHashValues)
    Private m_dctCurValues As Dictionary(Of String, cHashValues)

    Private m_lHashObjects As List(Of IHashSummarizer)
    Private m_strKeyRunFile As String = ""

    Private m_Results As cHashResults

    ''' <summary>List of errors that occurred during an operation.</summary>
    Private m_lErrors As List(Of String)

#End Region ' Private variables

#Region " Construction initialization "

    Public Sub New(ByVal Core As EwECore.cCore, _
                   ByVal PathData As EwECore.cEcopathDataStructures, _
                   ByVal SimData As EwECore.cEcosimDatastructures, _
                   ByVal SpaceData As EwECore.cEcospaceDataStructures)

        Me.m_core = Core
        Me.m_EcopathData = PathData
        Me.m_EcosimData = SimData
        Me.m_EcospaceData = SpaceData

        Me.m_dctCurValues = New Dictionary(Of String, cHashValues)
        Me.m_dctKeyRunValues = New Dictionary(Of String, cHashValues)
        Me.m_lHashObjects = New List(Of IHashSummarizer)
        Me.m_lErrors = New List(Of String)

    End Sub

#End Region ' Construction initialization

#Region " Public properties "

    Public ReadOnly Property Core As EwECore.cCore
        Get
            Return Me.m_core
        End Get
    End Property

    Public ReadOnly Property EcopathData As EwECore.cEcopathDataStructures
        Get
            Return Me.m_EcopathData
        End Get
    End Property

    Public ReadOnly Property EcosimData As EwECore.cEcosimDatastructures
        Get
            Return Me.m_EcosimData
        End Get
    End Property

    Public ReadOnly Property EcoSpaceData As EwECore.cEcospaceDataStructures
        Get
            Return Me.m_EcospaceData
        End Get
    End Property

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

    Public ReadOnly Property Messages As List(Of String)
        Get
            Return Me.m_lErrors
        End Get
    End Property

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
        sbFile.Append(Path.GetFileNameWithoutExtension(Me.m_core.DataSource.ToString()))

        '' Append author name
        'If (Not String.IsNullOrWhiteSpace(Me.m_core.DefaultAuthor)) Then
        '    sbFile.Append("^")
        '    sbFile.Append(Me.m_core.DefaultAuthor)
        'End If

        sbFile.Append(".ewekeyrun")

        Return cFileUtils.ToValidFileName(sbFile.ToString(), False)

    End Function

    Public Function DefaultKeyRunFileLocation() As String
        Return Path.GetDirectoryName(Me.m_core.DataSource.ToString())
    End Function

    Public Function LoadKeyRun(strFileName As String) As Boolean
        Dim bSuccess As Boolean = False

        Me.ResetErrors()

        If Me.RunStateOk Then
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
        End If 'Me.RunStateOk

        Me.NotifyUI()

        Return bSuccess

    End Function

    Public Function RunLoadedKeyRun() As Boolean
        Dim bSuccess As Boolean = False

        Me.ResetErrors()

        If Me.RunStateOk Then
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

        End If 'Me.RunStateOk 

        Me.NotifyUI()

        Return bSuccess

    End Function

    Public Function SaveKeyRunFile(strFileName As String) As Boolean

        Dim bSuccess As Boolean = False

        Me.ResetErrors()

        If Me.RunStateOk Then
            If Me.PopulateCurrentModel() Then
                bSuccess = Me.SaveCurModelToFile(strFileName)
            End If

            If (bSuccess) Then
                Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_SAVE_SUCCESS, strFileName), _
                               Path.GetDirectoryName(strFileName))
            Else
                Me.SendMessage(cStringUtils.Localize(My.Resources.STATUS_KEYRUN_SAVE_FAILED, strFileName))
            End If
        End If 'Me.RunStateOk

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

    Private Function RunStateOk() As Boolean
        Dim bStateOk As Boolean = Me.Core.StateMonitor.HasEcospaceLoaded
        If Not bStateOk Then
            Me.AddError("")
            Me.SendMessage("Sorry unable to run. You must reload an Ecospace scenario.")
        End If
        Return bStateOk
    End Function

    Private Function ReadKeyRunFile(FileName As String) As Boolean

        Dim bSuccess As Boolean = False

        Me.m_strKeyRunFile = String.Empty

        Try

            'Throw New Exception("Bogus exception for testing.")

            Me.m_dctKeyRunValues.Clear()

            Dim strm As New StreamReader(FileName)
            Dim hashVal As cHashValues
            Do While Not strm.EndOfStream
                Dim line As String
                line = strm.ReadLine()
                If cHashValues.isHashRecord(line) Then
                    hashVal = New cHashValues()
                    If hashVal.FromRecordString(line) Then
                        Debug.Assert(Not Me.m_dctKeyRunValues.ContainsKey(hashVal.Key), "Oh my! You're trying to add a duplicate key to the key run dictionary.")
                        Me.m_dctKeyRunValues.Add(hashVal.Key, hashVal)
                    End If
                End If
            Loop

            strm.Close()

            Me.m_strKeyRunFile = FileName
            bSuccess = True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Me.AddError(cStringUtils.Localize(My.Resources.DETAIL_KEYRUN_READ_FAILED, ex.Message))
        End Try

        Return bSuccess

    End Function

    Private Function CompareRuns() As Boolean

        Dim bSuccess As Boolean = False

        Try
            Me.m_Results = New cHashResults(Me.m_strKeyRunFile)

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

        ' Status stuff really belongs in the UI
        ' This should be either recoded using Progress messages, OR the UI should be notified about progress

        cApplicationStatusNotifier.StartProgress(Me.m_core)

        Dim n As Integer = Me.m_lHashObjects.Count
        For i As Integer = 0 To n - 1

            Dim wrapper As IHashSummarizer = Me.m_lHashObjects(i)

            cApplicationStatusNotifier.UpdateProgress(Me.m_core, _
                                                      cStringUtils.Localize(My.Resources.PROGRESS_HASHING, wrapper.Name), _
                                                      CSng((i + 1) / n))

            Try
                For Each hash As cHashValues In wrapper.HashValues()
                    'this is a coding issue you have dulicated one of the hash IDs
                    Debug.Assert(Not Me.m_dctCurValues.ContainsKey(hash.Key), "Oh my! You're trying to add a duplicate key to the current model dictionary.")
                    Me.m_dctCurValues.Add(hash.Key, hash)
                    ' System.Console.WriteLine(hash.SortOrder.ToString + ", " + hash.Component + ", " + hash.VariableID + ", " + hash.Hash + ", " + hash.Value)
                Next
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                'right now there is no way for IHashSummarizer to tell use what/who it is
                Me.AddError(cStringUtils.Localize(My.Resources.DETAIL_COMPUTE_HASH_FAILED, ex.Message))
            End Try
        Next

        cApplicationStatusNotifier.EndProgress(Me.m_core)

    End Sub

    Private Sub InitCurrentModel()

        'reset sort order to zero
        cHashValues.ClearSort()
        'Clear out any old results
        Me.m_dctCurValues.Clear()
        ' Clear out hash objects 
        Me.m_lHashObjects.Clear()

        '-- Core Scenarios --
        m_lHashObjects.Add(New cCoreScenariosWrapper(Me.Core))

        ' -- Ecopath --
        m_lHashObjects.Add(New cEcopathModelWrapper(Me.Core))
        m_lHashObjects.Add(New cEcopathInputWrapper(Me.Core))
        'Moved all Ecopath variables to inputs cEcopathInputWrapper 
        'm_lHashObjects.Add(New cEcopathOutputWrapper(Me.Core))
        m_lHashObjects.Add(New cDietCompWrapper(Me.Core))
        m_lHashObjects.Add(New cDetritusFateWrapper(Me.Core))
        m_lHashObjects.Add(New cEcopathFleetDefinitionWrapper(Me.Core))
        m_lHashObjects.Add(New cEcopathFleetWrapper(Me.Core))

        m_lHashObjects.Add(New cEcopathDiscardFateWrapper(Me.Core))

        ' -- Stanza --
        m_lHashObjects.Add(New cStanzaWrapper(Me.Core))
        m_lHashObjects.Add(New cStanzaLifestageWrapper(Me.Core))

        ' -- Ecosim --
        m_lHashObjects.Add(New cEcosimParamatersWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimEnvForcingWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimForcingFunctionWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimInputWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimEffortWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimMortalityWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimVulnerabilitiesWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimMediationWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimPriceElasticityWrapper(Me.Core))
        m_lHashObjects.Add(New cTimeSeriesWrapper(Me.Core))
        m_lHashObjects.Add(New cEcosimFleetSizeDynamicsWrapper(Me.Core))

        ' -- Ecospace --
        m_lHashObjects.Add(New cEcospaceParamatersWrapper(Me.Core))
        m_lHashObjects.Add(New cCapacityCalTypeWrapper(Me.Core))
        m_lHashObjects.Add(New cEcopaceCapacityWrapper(Me.Core))
        m_lHashObjects.Add(New cEcospaceHabitatWrapper(Me.Core))
        m_lHashObjects.Add(New cEcospaceDispersalWrapper(Me.Core))
        m_lHashObjects.Add(New cEcospaceFisheryWrapper(Me.Core))
        m_lHashObjects.Add(New cEcospaceFisheryHabitatsWrapper(Me.Core))
        m_lHashObjects.Add(New cEcospaceMapsWrapper(Me.Core))
        m_lHashObjects.Add(New cSpatialTemporalConfigurationWrapper(Me.Core))

        ' -- Give'r --
        For Each wrapper As IHashSummarizer In m_lHashObjects
            wrapper.Init()
        Next

    End Sub

    Private Function SaveCurModelToFile(filename As String) As Boolean

        Try
            Dim strm As New StreamWriter(filename)
            Dim an As AssemblyName = cAssemblyUtils.GetAssemblyName(Me.GetType())

            ' Write header info
            strm.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
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
        Me.m_lErrors.Clear()
    End Sub

    Private Sub AddError(message As String)
        Me.m_lErrors.Add(message)
    End Sub

    Private Sub SendMessage(ByVal strMessage As String, Optional strHyperlink As String = "")

        Dim msg As New cMessage(strMessage, _
                                eMessageType.DataExport, _
                                eCoreComponentType.External, _
                                CType(cSystemUtils.IIF(Me.m_lErrors.Count = 0, eMessageImportance.Information, eMessageImportance.Critical), eMessageImportance))
        msg.Hyperlink = strHyperlink

        For i As Integer = 0 To Me.m_lErrors.Count - 1
            Dim vs As New cVariableStatus(eStatusFlags.OK, Me.m_lErrors(i), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
            msg.Variables.Add(vs)
        Next

        Me.m_core.Messages.SendMessage(msg)
        Me.ResetErrors()

    End Sub

#End Region ' Messaging

#End Region ' Private Methods

End Class

