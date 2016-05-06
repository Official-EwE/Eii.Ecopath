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
' Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
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

#Region "Internal definitions"

    Public Enum eRunTypes
        Bounds
        Removal
    End Enum


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

    Private m_RunType As eRunTypes

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

    Public ReadOnly Property runType As eRunTypes
        Get
            Return Me.m_RunType
        End Get
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

    Public Sub onEcospaceTimeStep(EcospaceData As cEcospaceDataStructures, itime As Integer)
        Try

            Me.m_curTotTime += 1
            Me.m_curTimeStep += 1

            Me.MarshallOnProgress()

        Catch ex As Exception
            Me.ExceptionMessage("onEcospaceTimeStep()", ex)
        End Try

    End Sub

    Public Sub setBiomass(EcospaceData As cEcospaceDataStructures, itime As Integer)
        'Try

        '    If Me.m_curTimeStep > Me.m_RunTimeSteps - 12 Then
        '        'Me.m_nBTimeSteps += 1
        '        For igrp As Integer = 1 To Me.core.nGroups
        '            m_curB(igrp) += SpaceData.ResultsByGroup(eSpaceResultsGroups.Biomass, igrp, itime)
        '        Next igrp
        '    End If

        'Catch ex As Exception
        '    Me.ExceptionMessage("setBiomass()", ex)
        'End Try

    End Sub

    Private Sub ExceptionMessage(Source As String, ex As Exception, Optional msg As String = "")
        EwEUtils.Core.cLog.Write(ex, Source)
        System.Console.WriteLine("Exception: " + Source + ex.Message)
    End Sub


    Public Sub StopRun()
        Me.m_plugin.EcoSpace.m_StopRun = True
        Me.m_bStop = True
    End Sub

    Public Function isBoundsConfigured() As Boolean
        Dim lstMsgs As New List(Of String)
        Dim msg As String
        If Not Directory.Exists(Path.GetDirectoryName(Me.RunParameters.BoundsOutput)) Then
            lstMsgs.Add("No ouput file defined.")
        End If

        For Each pair In Me.RunParameters.lstBoundsFiles
            If Not File.Exists(pair.File) Then
                lstMsgs.Add("Invalid input file for Drive Layer '" + pair.MapLayer.Layer.Name + "'.")
            End If
        Next

        If lstMsgs.Count > 0 Then
            For Each mg As String In lstMsgs
                msg += vbCrLf + mg
            Next

            MsgBox("Ecospace sensitivity parameter uncertainty is not properly configured. Please fix the following issues." + msg)
            Me.StopRun()
            Return False

        End If

        Return True

    End Function

    Public Function isRemovalConfigured() As Boolean
        If Not Directory.Exists(Path.GetDirectoryName(Me.RunParameters.RemovalOutput)) Then
            MsgBox("Ecospace sensitivity  to removal is not properly configured. Please select a valid output file")
            Me.StopRun()
            Return False
        End If

        Return True

    End Function



    Public Sub Init(thePlugin As cEcospaceSensitivityPluginPoint)
        Me.m_plugin = thePlugin
        core = Me.m_plugin.Core
        Me.m_parameters = New cRunParameters(core)
        Me.m_RunSpace = New cRunEcospace
        Me.m_FileManager = New cSpatialTemporalFileManager(core)

    End Sub

    Public Function RunBounds() As Boolean

        If Me.isBoundsConfigured Then
            Me.m_RunType = eRunTypes.Bounds
            Me.setState(eEcospaceSensitivityStates.Running)

            Me.m_isRunning = True
            Me.m_curTotTime = 0
            Me.m_TotTimeSteps = calcTotalTimeSteps()

            Dim runthread As New Thread(AddressOf RunOnThread)
            runthread.Start()
            Return True

        End If

        Return False

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
            Dim pair As cLayerFilePair
            Me.m_bStop = False
            Me.initForRun()

            'Run and Write Baseline run
            Me.m_RunSpace.Run()
            'If the user stopped the run don't do any of the processing
            If Not Me.m_bStop Then

                Me.m_curMapName = "BaseLine"
                Me.IterationCompleted(Me.m_curMapName, 0.0)

                For Each pair In Me.RunParameters.lstBoundsFiles
                    If Me.m_bStop Then Exit For

                    Me.m_curMapName = pair.MapLayer.Layer.Name

                    Me.CreateBoundsFiles(pair)

                    Me.SwapFiles(pair.MapLayer.Layer, Me.m_lowerFile, pair.File)

                    Me.m_RunSpace.Run()
                    If Me.m_bStop Then Exit For
                    Me.IterationCompleted(pair.MapLayer.Layer.Name, Me.RunParameters.LowerBound)

                    Me.SwapFiles(pair.MapLayer.Layer, Me.m_upperFile, pair.File)
                    Me.m_RunSpace.Run()
                    If Me.m_bStop Then Exit For
                    Me.IterationCompleted(pair.MapLayer.Layer.Name, Me.RunParameters.UpperBound)

                    Me.SwapFiles(pair.MapLayer.Layer, pair.File, "")

                    Me.CleanUpFiles(pair.File)

                Next pair

            End If

            'If the user stopped the run while in the loop
            'then we need to clean up the input files
            If Me.m_bStop And pair IsNot Nothing Then
                Me.CleanUpFiles(pair.File)
            End If

        Catch ex As Exception
            Me.ExceptionMessage("RunOnThread()", ex)
        End Try

        Me.RunsCompleted()

    End Sub




    Private Function calcTotalTimeSteps() As Integer

        If Me.m_RunType = eRunTypes.Bounds Then
            Return Me.calcBoundsTimeSteps
        ElseIf Me.m_RunType = eRunTypes.Removal Then
            Return Me.calcRemovalTimeSteps
        End If

        Debug.Assert(False, "Oppss failed to set the total number of time steps.")
        Return 0

    End Function


    Private Function calcBoundsTimeSteps() As Integer

        Dim nSpinUpSteps As Integer = 0
        If Me.SpaceData.UseSpinUp Then
            nSpinUpSteps = CInt(SpaceData.SpinUpYears * CInt(1.0 / SpaceData.TimeStep))
        End If

        Me.m_RunTimeSteps = Me.core.nEcospaceTimeSteps + nSpinUpSteps
        Dim nruns As Integer = Me.RunParameters.lstRemovalLayers.Count * 2 + 1

        Return Me.m_RunTimeSteps * nruns

    End Function


    Private Function calcRemovalTimeSteps() As Integer
        Dim nRuns As Integer
        Dim nSpinUpSteps As Integer = 0

        If Me.SpaceData.UseSpinUp Then
            nSpinUpSteps = CInt(SpaceData.SpinUpYears * CInt(1.0 / SpaceData.TimeStep))
        End If
        Me.m_RunTimeSteps = Me.core.nEcospaceTimeSteps + nSpinUpSteps

        For Each map As IEnviroInputMap In Me.RunParameters.lstRemovalLayers
            For igrp As Integer = 1 To Me.core.nGroups
                If map.ResponseIndexForGroup(igrp) > 0 Then
                    nRuns += 1
                End If
            Next
        Next

        Return Me.m_RunTimeSteps * (nRuns + 1)

    End Function


    Public Function RunRemoval() As Boolean

        If isRemovalConfigured() Then
            Me.m_RunType = eRunTypes.Removal
            Me.setState(eEcospaceSensitivityStates.Running)

            Me.m_isRunning = True
            Me.m_curTotTime = 0
            Me.m_TotTimeSteps = calcRemovalTimeSteps()

            Dim runthread As New Thread(AddressOf RunRemovalOnThread)
            runthread.Start()
        End If

        Return True

    End Function


    Private Sub RunRemovalOnThread()

        Try

            Me.m_bStop = False

            Me.initForRun()

            'Run and Write Baseline run
            Me.m_RunSpace.Run()
            Me.m_curMapName = "BaseLine"
            Me.IterationCompleted(Me.m_curMapName, 0)
            'Me.SaveRun(Me.m_curMapName, 0.0)

            For Each map As IEnviroInputMap In Me.RunParameters.lstRemovalLayers
                If Me.m_bStop Then Exit For

                Me.StoreResponseGroup(map)

                For Each resFunction As cResponseFunctionValuePair In Me.m_lstResponseFunctions
                    If Me.m_bStop Then Exit For
                    resFunction.removeResponse(map)

                    Me.m_RunSpace.Run()
                    If Me.m_bStop Then Exit For
                    Me.IterationCompleted(map.Layer.Name, resFunction.iGroup)

                    resFunction.RestoreResponse(map)
                Next

            Next map

        Catch ex As Exception
            Me.ExceptionMessage("RunRemovalOnThread()", ex)
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
            Me.ExceptionMessage("MarshallOnProgress", ex)
        End Try
    End Sub

    Private Sub fireOnStateChanged(arg As Object)
        RaiseEvent OnStateChange(Me.m_curState)
    End Sub

    Private Sub MarshallOnStateChanged(NewState As eEcospaceSensitivityStates)
        Try
            Me.m_SyncObj.Send(New System.Threading.SendOrPostCallback(AddressOf Me.fireOnStateChanged), NewState)
        Catch ex As Exception
            Me.ExceptionMessage("MarshallOnStateChanged", ex)
        End Try
    End Sub

    Private Sub fireOnProgress(arg As Object)
        Dim TotalPercentDone As Single = CSng(Me.m_curTotTime / Me.m_TotTimeSteps)
        Dim RunPercentDone As Single = CSng(Me.m_curTimeStep / Me.m_RunTimeSteps)
        RaiseEvent OnProgress(TotalPercentDone, RunPercentDone, Me.m_curMapName)
    End Sub



    Private Sub initForRun()

        Me.m_curB = New Single(Me.core.nGroups) {}
        Me.m_RunSpace.Init(Me.m_plugin.Core, Me.m_plugin.EcoSpace)
        Me.writeResultsHeader()

    End Sub

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
            Me.ExceptionMessage("RunsCompleted()", ex)
        End Try

    End Sub

    Private Sub IterationCompleted(RunName As String, ColumnValue As Single)

        Me.SaveRun(RunName, ColumnValue)
        Me.m_curTimeStep = 0
        Me.MarshallOnProgress()

    End Sub


    Private Sub SaveRun(RunName As String, ColumnValue As Single)
        Dim startB As Single, EndB As Single
        'average the biomass over the last year
        For i As Integer = 1 To Me.core.nGroups
            Me.SpaceData.getSumBiom(i, startB, EndB)
            Me.m_curB(i) = EndB
        Next

        Me.writeResults(RunName, ColumnValue)

    End Sub

    Private Sub writeResults(RunName As String, ColumnValue As Single)

        Dim strm As StreamWriter
        strm = New StreamWriter(Me.getOuputFile(), True)

        strm.Write(RunName + "," + ColumnValue.ToString)
        For igrp As Integer = 1 To Me.core.nGroups
            strm.Write("," + Me.m_curB(igrp).ToString)
        Next
        strm.WriteLine()
        strm.Close()

    End Sub

    Private Sub writeResultsHeader()
        Try

            Dim strm As StreamWriter
            'Delete the old file if it exists
            strm = New StreamWriter(Me.getOuputFile(), False)

            strm.Write(Me.core.DefaultFileHeader(EwEUtils.Core.eAutosaveTypes.Ecospace))
            strm.WriteLine()
            strm.Write("Driver_Layer," + Me.getColHeader())
            For igrp As Integer = 1 To Me.core.nGroups
                strm.Write("," + EwEUtils.Utilities.cStringUtils.ToCSVField(Me.m_plugin.EcoPathData.GroupName(igrp)))
            Next
            strm.WriteLine()
            strm.Close()
        Catch ex As Exception
            Me.ExceptionMessage("writeResultsHeader", ex)
        End Try

    End Sub

    Private Function getOuputFile() As String
        If Me.m_RunType = eRunTypes.Bounds Then
            Return Me.RunParameters.BoundsOutput
        Else
            Return Me.RunParameters.RemovalOutput
        End If

        Return ""
    End Function


    Private Function getColHeader() As String

        If Me.m_RunType = eRunTypes.Bounds Then
            Return "Percentage_Alteration"
        Else
            Return "Group_Index"
        End If

        Return ""

    End Function


    Private Sub CreateBoundsFiles(LayerFilePair As cLayerFilePair)

        'Me.m_orgBackupFile = Path.Combine(Path.GetDirectoryName(LayerFilePair.File), Path.GetFileNameWithoutExtension(LayerFilePair.File) + ".org.asc")
        'If File.Exists(Me.m_orgBackupFile) Then
        '    File.Delete(Me.m_orgBackupFile)
        'End If
        'File.Copy(LayerFilePair.File, Me.m_orgBackupFile)
        'Debug.Assert(File.Exists(Me.m_orgBackupFile))

        Me.m_lowerFile = Path.Combine(Path.GetDirectoryName(LayerFilePair.File), Path.GetFileNameWithoutExtension(LayerFilePair.File) + ".lower.asc")
        Me.AlterBoundsFiles(LayerFilePair.MapLayer.Layer, LayerFilePair.File, Me.m_lowerFile, Me.RunParameters.LowerBound)

        Me.m_upperFile = Path.Combine(Path.GetDirectoryName(LayerFilePair.File), Path.GetFileNameWithoutExtension(LayerFilePair.File) + ".upper.asc")
        Me.AlterBoundsFiles(LayerFilePair.MapLayer.Layer, LayerFilePair.File, Me.m_upperFile, Me.RunParameters.UpperBound)

    End Sub

    Private Sub SwapFiles(layer As cEcospaceLayer, sourceFile As String, destinationFile As String)

        Me.m_FileManager.SwapFiles(layer, sourceFile)

        'If File.Exists(sourceFile) Then

        '    If File.Exists(destinationFile) Then
        '        File.Delete(destinationFile)
        '    End If

        '    File.Copy(sourceFile, destinationFile)
        'End If

    End Sub

    Private Sub SwapFiles(sourceFile As String, destinationFile As String)

        'Me.m_FileManager.SwapFiles(sourceFile)

        If File.Exists(sourceFile) Then

            If File.Exists(destinationFile) Then
                File.Delete(destinationFile)
            End If

            File.Copy(sourceFile, destinationFile)
        End If

    End Sub

    Private Sub CleanUpFiles(OriginalFile As String)

        'Me.SwapFiles(Me.m_orgBackupFile, OriginalFile)

        'If File.Exists(Me.m_orgBackupFile) Then
        '    File.Delete(Me.m_orgBackupFile)
        'End If

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
        Dim a As Single, b As Single

        Dim orgData(,) As Single = Me.getCoreLayerData(OrgLayer)
        ascFile.ReadASCFile(strmOrg)
        after = ascFile.data
        strmOrg.Close()

        Dim x(,) As Single = New Single(Me.SpaceData.InRow, Me.SpaceData.InCol) {}
        'a = new data, after the change
        'b = orginal data, before the change
        'x = b +(a-b)*delta
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                b = orgData(ir, ic)
                If b < 0 Then b = 0
                a = after(ir, ic)

                If after(ir, ic) > 0 Then
                    x(ir, ic) = b + (a - b) * Percentage
                Else
                    x(ir, ic) = cCore.NULL_VALUE
                End If
            Next
        Next

        ascFile.data = x
        ascFile.SaveASCFile(strmNew)
        strmNew.Close()

    End Sub

    Private Function getCoreLayerData(Layer As cEcospaceLayer) As Single(,)
        Dim temp(,) As Single

        If Layer.VarName = EwEUtils.Core.eVarNameFlags.LayerDepth Then

            temp = New Single(Me.SpaceData.InRow, Me.SpaceData.InCol) {}
            For ir As Integer = 1 To Me.SpaceData.InRow
                For ic As Integer = 1 To Me.SpaceData.InCol
                    temp(ir, ic) = Me.SpaceData.Depth(ir, ic)
                Next ic
            Next ir

        ElseIf Layer.VarName = EwEUtils.Core.eVarNameFlags.LayerDriver Then


            Dim CoreData(,,) As Single = Me.SpaceData.EnvironmentalLayerMap
            temp = New Single(Me.SpaceData.InRow, Me.SpaceData.InCol) {}
            For ir As Integer = 1 To Me.SpaceData.InRow
                For ic As Integer = 1 To Me.SpaceData.InCol
                    temp(ir, ic) = CoreData(Layer.Index, ir, ic)
                Next ic
            Next ir


        End If

        Debug.Assert(temp IsNot Nothing, "Failed to read original raster data from core.")
        Return temp

    End Function

#End Region

End Class
