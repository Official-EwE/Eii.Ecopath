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

Option Explicit On
Option Strict On

Imports EwECore
Imports System.IO
Imports System.Threading

Public Class cRunManager

#Region "Definitions"

#Region "Public Events"

    Public Event OnProgress(TotalPercentDone As Single, RunPercentDone As Single, MapName As String)

    Public Event OnStateChange(State As eEcospaceSensitivityStates)

#End Region

#Region "Public definitions"

    Public Enum eEcospaceSensitivityStates
        Stopped
        Running
    End Enum

#End Region

#Region "Internal class definitions"

    Private Class cResponseFunctionValuePair
        Public orgData() As Single
        Public orgResponseFunct As cEnviroResponseFunction
        Public iGroup As Integer

     
        Public Sub New(Shape As cEnviroResponseFunction, iGroupIndex As Integer)
            orgResponseFunct = Shape
            Me.iGroup = iGroupIndex
        End Sub

     
        Public Sub removeResponse(map As IEnviroInputMap)

            Debug.Assert(map.ResponseIndexForGroup(Me.iGroup) = Me.orgResponseFunct.Index)
            map.ResponseIndexForGroup(Me.iGroup) = 0

        End Sub

        Public Sub RestoreResponse(map As IEnviroInputMap)

            map.ResponseIndexForGroup(Me.iGroup) = Me.orgResponseFunct.Index

        End Sub

    End Class


    Private Class cMapLayer
        Public MapLayer As IEnviroInputMap

        Private m_orgLayerData(,) As Single
        Private manager As cRunManager

        Public Sub New(RunManager As cRunManager, map As IEnviroInputMap)
            Me.manager = RunManager
            Me.MapLayer = map
            Me.storeOrgData()
        End Sub

        Private Sub storeOrgData()
           
            Me.storeLayerData(Me.MapLayer.Layer.Index)

        End Sub

        Private Sub storeLayerData(iLayer As Integer)

            Dim data(,,) As Single = Me.manager.SpaceData.EnvironmentalLayerMap
            m_orgLayerData = New Single(Me.manager.SpaceData.InRow, Me.manager.SpaceData.InCol) {}
            For ir As Integer = 1 To Me.manager.SpaceData.InRow
                For ic As Integer = 1 To Me.manager.SpaceData.InCol
                    m_orgLayerData(ir, ic) = data(iLayer, ir, ic)
                Next ic
            Next ir

        End Sub





    End Class


#End Region

#End Region

#Region "Private variables"

    ''' <summary>Core thread synchronization object for thread marshalling.</summary>
    Private m_SyncObj As System.Threading.SynchronizationContext = Nothing

    Private m_RunSpace As cRunEcospace
    Private m_plugin As cEcospaceSensitivityPluginPoint
    Private m_FileManager As cSpatialTemporalFileManager

    Private core As cCore
    Private RunType As String
    Private m_isConfig As Boolean

    Private m_TrialNumber As Integer
    Private m_waitLock As ManualResetEvent

    Private m_bStop As Boolean

    Private m_parameters As cRunParameters

    Private m_curB() As Single

    Private m_isRunning As Boolean

    Private m_lstResponseFunctions As List(Of cResponseFunctionValuePair)

    Private m_curTotTime As Integer
    Private m_curTimeStep As Integer
    Private m_curMapName As String

    Private m_TotTimeSteps As Integer
    Private m_RunTimeSteps As Integer

    Private m_curState As eEcospaceSensitivityStates

    Private m_orgBackupFile As String
    Private m_upperFile As String
    Private m_lowerFile As String

#End Region

#Region "Public Properties and Methods"

    Public Sub New()

        Me.m_SyncObj = System.Threading.SynchronizationContext.Current
        'if there is no current context then create a new one on this thread. 
        If (Me.m_SyncObj Is Nothing) Then Me.m_SyncObj = New System.Threading.SynchronizationContext()

        Me.m_curState = eEcospaceSensitivityStates.Stopped

    End Sub


    Public Property RunParameters As cRunParameters

        Get
            Return Me.m_parameters
        End Get
        Set(value As cRunParameters)
            Me.m_parameters = value
        End Set
    End Property

    Public ReadOnly Property isRunning As Boolean
        Get
            Return Me.m_isRunning
        End Get
    End Property

    Public ReadOnly Property SpaceData As cEcospaceDataStructures
        Get
            Return Me.m_plugin.EcoSpaceData
        End Get
    End Property

    Public Sub setBiomass(biomass() As Single, itime As Integer)
        m_curB = biomass
        Me.m_curTotTime += 1
        Me.m_curTimeStep = itime
        ' If itime Mod 12 = 0 Then
        Me.MarshallOnProgress()
        '  End If
    End Sub

    Public Sub StopRun()
        Me.m_plugin.EcoSpace.m_StopRun = True
        Me.m_bStop = True
    End Sub

    Public Sub isConfigured()
        Me.m_isConfig = True

        Dim msg As String
        If Not Directory.Exists(Path.GetDirectoryName(Me.RunParameters.OutputFileName)) Then
            Me.m_isConfig = False
            msg = "No output file defined"
            MsgBox("Ecospace Sensitivity is not properly configured. Please stop the search and fix the following issues." + vbCrLf + msg)
            Me.StopRun()
        End If

        'If File.Exists(OutputFilename) Then
        '    If MsgBox("Selected output file already exists. Do you want to overwrite it?" + vbCrLf + "Yes to overwrite" + vbCrLf + "No to append new results.", _
        '              MsgBoxStyle.YesNo, "Ecospace MonteCarlo.") = MsgBoxResult.Yes Then
        '        Try
        '            File.Delete(OutputFilename)
        '        Catch ex As Exception

        '        End Try
        '    End If
        'End If

        'If Not Me.m_isConfig Then
        '    MsgBox("Ecospace Sensitivity is not properly configured. Please stop the search and fix the following issues." + vbCrLf + msg)
        'End If
    End Sub


    Public Sub Init(thePlugin As cEcospaceSensitivityPluginPoint)
        Me.m_plugin = thePlugin
        core = Me.m_plugin.Core
        Me.m_parameters = New cRunParameters(core)
        Me.m_RunSpace = New cRunEcospace
        Me.m_FileManager = New cSpatialTemporalFileManager(core)

    End Sub

    Public Function Run() As Boolean

        'If Not Me.m_isConfig Then
        '    Return False
        'End If


        Me.setState(eEcospaceSensitivityStates.Running)

        Me.m_isRunning = True
        Me.m_curTotTime = 0
        Me.m_TotTimeSteps = calcTotalTimeStep()

        Dim runthread As New Thread(AddressOf RunOnThread)
        runthread.Start()

        Return True

    End Function

    Public ReadOnly Property State As eEcospaceSensitivityStates
        Get
            Return Me.m_curState
        End Get
    End Property


#End Region

#Region "Private Methods"

    Private Sub RunOnThread()

        Try
            Dim AltPercent As Single

            Me.m_bStop = False

            Me.initForRun()

            'Run and Write Baseline run
            Me.m_RunSpace.Run()
            Me.m_curMapName = "BaseLine"
            Me.SaveRun(Me.m_curMapName, 1.0)

            For Each pair As cLayerFilePair In Me.RunParameters.lstFiles
                If Me.m_bStop Then Exit For

                Me.m_curMapName = pair.MapLayer.Layer.Name

                AltPercent = Me.RunParameters.LowerBound
                Me.CreateBoundsFiles(pair)

                Me.SwapFiles(Me.m_lowerFile, pair.File)

                Me.m_RunSpace.Run()
                If Me.m_bStop Then Exit For
                Me.SaveRun(pair.MapLayer.Layer.Name, AltPercent)

                AltPercent = Me.RunParameters.UpperBound
                Me.SwapFiles(Me.m_upperFile, pair.File)
                Me.m_RunSpace.Run()
                If Me.m_bStop Then Exit For
                Me.SaveRun(pair.MapLayer.Layer.Name, AltPercent)

                Me.CleanUpFiles(pair.File)

            Next pair


        Catch ex As Exception

        End Try

        Me.RunsCompleted()

    End Sub

    Private Function calcRemovalTimeSteps() As Integer

        Dim n As Integer
        Me.m_RunTimeSteps = Me.core.nEcospaceTimeSteps

        For Each map As IEnviroInputMap In Me.RunParameters.lstLayers
            For igrp As Integer = 1 To Me.core.nGroups
                If map.ResponseIndexForGroup(igrp) > 0 Then
                    n += 1
                End If
            Next
        Next

        Return Me.m_RunTimeSteps * n + 1

    End Function


    Public Function RunRemoval() As Boolean

        'If Not Me.m_isConfig Then
        '    Return False
        'End If


        Me.setState(eEcospaceSensitivityStates.Running)

        Me.m_isRunning = True
        Me.m_curTotTime = 0
        Me.m_TotTimeSteps = calcRemovalTimeSteps()

        Dim runthread As New Thread(AddressOf RunRemovalOnThread)
        runthread.Start()

        Return True

    End Function


    Private Sub RunRemovalOnThread()

        Try

            Me.m_bStop = False

            Me.initForRun()

            'Run and Write Baseline run
            Me.m_RunSpace.Run()
            Me.m_curMapName = "BaseLine"
            Me.SaveRun(Me.m_curMapName, 0.0)

            For Each map As IEnviroInputMap In Me.RunParameters.lstLayers
                If Me.m_bStop Then Exit For

                Me.StoreResponseGroup(map)

                For Each resFunction As cResponseFunctionValuePair In Me.m_lstResponseFunctions
                    If Me.m_bStop Then Exit For
                    resFunction.removeResponse(map)

                    Me.m_RunSpace.Run()
                    If Me.m_bStop Then Exit For
                    Me.SaveRun(map.Layer.Name, resFunction.iGroup)

                    resFunction.RestoreResponse(map)
                Next

            Next map

        Catch ex As Exception

        End Try

        Me.RunsCompleted()

    End Sub

    Private Sub setState(newState As eEcospaceSensitivityStates)
        Me.m_curState = newState
        Me.MarshallOnStateChanged(Me.m_curState)
    End Sub

    Private Sub MarshallOnProgress()
        Try
            Me.m_SyncObj.Send(New System.Threading.SendOrPostCallback(AddressOf Me.fireOnProgress), Nothing)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub fireOnStateChanged(arg As Object)
        RaiseEvent OnStateChange(Me.m_curState)
    End Sub

    Private Sub MarshallOnStateChanged(NewState As eEcospaceSensitivityStates)
        Try
            Me.m_SyncObj.Send(New System.Threading.SendOrPostCallback(AddressOf Me.fireOnStateChanged), NewState)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub fireOnProgress(arg As Object)
        Dim TotalPercentDone As Single = CSng(Me.m_curTotTime / Me.m_TotTimeSteps)
        Dim RunPercentDone As Single = CSng(Me.m_curTimeStep / Me.m_RunTimeSteps)
        RaiseEvent OnProgress(TotalPercentDone, RunPercentDone, Me.m_curMapName)
    End Sub



    Private Sub initForRun()

        Me.m_RunSpace.Init(Me.m_plugin.Core, Me.m_plugin.EcoSpace)
        ' Me.m_TotTimeSteps = Me.calcTotalTimeStep()
        Me.writeResponseAlterationHeader()


    End Sub




    Private Function calcTotalTimeStep() As Integer

        Dim nruns As Integer = Me.RunParameters.lstLayers.Count * 2 + 1
        Me.m_RunTimeSteps = Me.core.nEcospaceTimeSteps
        Return Me.m_RunTimeSteps * nruns

    End Function


    Private Sub dumpB(Percentage As Single)
        System.Console.Write("Ecospace B Percentage," + Percentage.ToString)
        For igrp As Integer = 1 To Me.core.nGroups
            System.Console.Write("," + Me.m_curB(igrp).ToString)
        Next
        System.Console.WriteLine()
    End Sub

    Private Sub RunsCompleted()
        Try
            Me.m_isRunning = False
            Me.setState(eEcospaceSensitivityStates.Stopped)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub SaveRun(RunName As String, PercentageOfChange As Single)

        Me.m_curTimeStep = 0
        Me.writeResponseAlterationResults(RunName, PercentageOfChange)
        Me.MarshallOnProgress()

    End Sub

    Private Sub writeResponseAlterationResults(RunName As String, PercentageOfChange As Single)

        Dim strm As StreamWriter
        strm = New StreamWriter(Me.RunParameters.OutputFileName, True)

        strm.Write(RunName + "," + PercentageOfChange.ToString)
        For igrp As Integer = 1 To Me.core.nGroups
            strm.Write("," + Me.m_curB(igrp).ToString)
        Next
        strm.WriteLine()
        strm.Close()

    End Sub

    Private Sub writeResponseAlterationHeader()

        Dim strm As StreamWriter
        'Delete the old file if it exists
        strm = New StreamWriter(Me.RunParameters.OutputFileName, False)

        strm.Write(Me.core.DefaultFileHeader(EwEUtils.Core.eAutosaveTypes.Ecospace))
        strm.WriteLine()
        strm.Write("Run_Type,Percentage_Alteration")
        For igrp As Integer = 1 To Me.core.nGroups
            strm.Write("," + EwEUtils.Utilities.cStringUtils.ToCSVField(Me.m_plugin.EcoPathData.GroupName(igrp)))
        Next
        strm.WriteLine()
        strm.Close()

    End Sub


    Private Sub CreateBoundsFiles(LayerFilePair As cLayerFilePair)

        Me.m_orgBackupFile = Path.Combine(Path.GetDirectoryName(LayerFilePair.File), Path.GetFileNameWithoutExtension(LayerFilePair.File) + ".org.asc")
        If File.Exists(Me.m_orgBackupFile) Then
            File.Delete(Me.m_orgBackupFile)
        End If
        File.Copy(LayerFilePair.File, Me.m_orgBackupFile)
        Debug.Assert(File.Exists(Me.m_orgBackupFile))

        Me.m_lowerFile = Path.Combine(Path.GetDirectoryName(LayerFilePair.File), Path.GetFileNameWithoutExtension(LayerFilePair.File) + ".lower.asc")
        Me.AlterBoundsFiles(LayerFilePair.MapLayer.Layer, LayerFilePair.File, Me.m_lowerFile, Me.RunParameters.LowerBound)

        Me.m_upperFile = Path.Combine(Path.GetDirectoryName(LayerFilePair.File), Path.GetFileNameWithoutExtension(LayerFilePair.File) + ".upper.asc")
        Me.AlterBoundsFiles(LayerFilePair.MapLayer.Layer, LayerFilePair.File, Me.m_upperFile, Me.RunParameters.UpperBound)

    End Sub

    Private Sub SwapFiles(sourceFile As String, destinationFile As String)

        Me.m_FileManager.SwapFiles(sourceFile)

        'If File.Exists(destinationFile) Then
        '    File.Delete(destinationFile)
        'End If

        'File.Copy(sourceFile, destinationFile)

    End Sub

    Private Sub CleanUpFiles(OriginalFile As String)

        Me.SwapFiles(Me.m_orgBackupFile, OriginalFile)

        If File.Exists(Me.m_orgBackupFile) Then
            File.Delete(Me.m_orgBackupFile)
        End If

        If File.Exists(Me.m_lowerFile) Then
            File.Delete(Me.m_lowerFile)
        End If

        If File.Exists(Me.m_upperFile) Then
            File.Delete(Me.m_upperFile)
        End If

    End Sub


    Private Sub StoreResponseGroup(map As IEnviroInputMap)

        m_lstResponseFunctions = New List(Of cResponseFunctionValuePair)
        For igrp As Integer = 1 To Me.core.nGroups
            Dim iResponseIndex As Integer = map.ResponseIndexForGroup(igrp)
            If iResponseIndex > 0 Then
                Dim ResponFunct As cEnviroResponseFunction = DirectCast(Me.core.CapacityShapeManager.Item(iResponseIndex - 1), cEnviroResponseFunction)
                m_lstResponseFunctions.Add(New cResponseFunctionValuePair(ResponFunct, igrp))
            End If
        Next

    End Sub

    Private Sub AlterBoundsFiles(OrgLayer As cEcospaceLayer, orgfile As String, NewFile As String, Percentage As Single)

        Dim after(,) As Single
        Dim strmOrg As New StreamReader(orgfile)
        Dim strmNew As New StreamWriter(NewFile)
        Dim ascFile As New cASCIIReaderWriter(Me.core)

        Dim x(,) As Single = New Single(Me.SpaceData.InRow, Me.SpaceData.InCol) {}
        Dim orgData(,) As Single = Me.getLayerData(OrgLayer)
        ascFile.ReadASCFile(strmOrg)
        after = ascFile.data
        strmOrg.Close()

        'x = b +(a-b)*delta
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                x(ir, ic) = orgData(ir, ic) + (after(ir, ic) - orgData(ir, ic)) * Percentage
            Next
        Next

        ascFile.data = x
        ascFile.SaveASCFile(strmNew)
        strmNew.Close()


    End Sub

    Private Function getLayerData(Layer As cEcospaceLayer) As Single(,)

        Dim CoreData(,,) As Single = Me.SpaceData.EnvironmentalLayerMap
        Dim temp(,) = New Single(Me.SpaceData.InRow, Me.SpaceData.InCol) {}
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                temp(ir, ic) = CoreData(Layer.Index, ir, ic)
            Next ic
        Next ir

        Return temp

    End Function

#End Region

End Class
