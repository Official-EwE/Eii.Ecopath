Option Explicit On
Option Strict On


Imports EwECore
Imports System.IO
Imports System.Threading

Public Class cRunPeriods

    Public StartYear As Integer
    Public nYears As Integer

    Public Sub New(Start As Integer, NumberOfYears As Integer)
        StartYear = Start
        nYears = NumberOfYears
    End Sub

End Class


Public Class cRunParameters

    Public OutputFileName As String
    Public RunTimes As cRunPeriods
    Public lstLayers As List(Of IEnviroInputMap)
    Public Delta As Single

    Public ReadOnly Property LowerBound As Single
        Get
            Dim temp As Single = 1 - Delta
            If temp < 0 Then temp = 0
            Return temp
        End Get
    End Property

    Public ReadOnly Property UpperBound As Single
        Get
            Return 1 + Delta
        End Get
    End Property


    Private m_core As cCore

    Public Sub New(theCore As cCore)
        Me.OutputFileName = "C:\Users\Joe\Documents\Projects\EwE\Ecopath6\Sources Internal\Plugins\EcospaceSensitivityAnalysis\B_out.csv"
        Me.m_core = theCore
        Me.setDefaults()

    End Sub

    Private Sub setDefaults()
        RunTimes = New cRunPeriods(2015, 5)
        Delta = 0.2
        Me.setDefaultMapLayers()
    End Sub

    Private Sub setDefaultMapLayers()
        Dim mapManager As cMapResponseInteractionManager = Me.m_core.CapacityMapInteractionManager
        Dim map As IEnviroInputMap = Nothing

        Me.lstLayers = New List(Of IEnviroInputMap)
        For iMap As Integer = 1 To mapManager.nMaps
            'Not Depth or Hard sediment
            If Not mapManager.Map(iMap).Layer.Name.Trim.ToLower.Contains("hard sediment") Then
                Me.lstLayers.Add(mapManager.Map(iMap))
            End If

        Next iMap
    End Sub

End Class


Public Class cRunManager

    Private Class cResponseFunctionValuePair
        Public orgData() As Single
        Public orgShape As cForcingFunction

        Public Sub New(Shape As cForcingFunction)
            orgShape = Shape
            Me.store()
        End Sub

        Public Sub store()
            orgData = New Single(orgShape.ShapeData.Length - 1) {}
            Array.Copy(orgShape.ShapeData, orgData, orgShape.ShapeData.Length)
        End Sub

        Public Sub restore()
            orgShape.ShapeData = orgData
        End Sub
    End Class


    Private m_RunSpace As cRunEcospace
    Private m_plugin As cEcospaceSensitivityPluginPoint

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

    Public Sub setBiomass(biomass() As Single)
        m_curB = biomass
    End Sub

    Public Sub StopRun()
        Me.m_plugin.EcoSpace.m_StopRun = True
        m_bStop = True
    End Sub

    Public Sub isConfigured()
        Me.m_isConfig = True

        'Not for now
        Return

        Dim msg As String
        If Not Directory.Exists(Path.GetDirectoryName(Me.RunParameters.OutputFileName)) Then
            Me.m_isConfig = False
            msg = "No output file defined"
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

        If Not Me.m_isConfig Then
            MsgBox("Ecospace MonteCarlo is not properly configured. Please stop the search and fix the following issues." + vbCrLf + msg)
        End If
    End Sub


    Public Sub Init(thePlugin As cEcospaceSensitivityPluginPoint)
        Me.m_plugin = thePlugin
        core = Me.m_plugin.Core
        Me.m_parameters = New cRunParameters(core)
        Me.m_RunSpace = New cRunEcospace

    End Sub

    Public Function Run() As Boolean

        'If Not Me.m_isConfig Then
        '    Return False
        'End If

        Me.m_isRunning = True
        Dim runthread As New Thread(AddressOf RunOnThread)
        runthread.Start()

        Return True

    End Function


    Private Sub RunOnThread()

        Try

            Me.m_bStop = False
            Me.m_RunSpace.Init(Me.m_plugin.Core, Me.m_plugin.EcoSpace)
            Me.m_RunSpace.SetRunParameters(Me.RunParameters.RunTimes)

            For Each map As IEnviroInputMap In Me.RunParameters.lstLayers

                Dim AltPercent As Single = Me.RunParameters.UpperBound
                Me.StoreResponse(map)

                Me.AlterResponse(map, AltPercent)
                Me.m_RunSpace.Run()
                Me.writeResponseAlterationResults(map, AltPercent)

                Me.RestoreResponse()

                AltPercent = Me.RunParameters.LowerBound
                Me.AlterResponse(map, AltPercent)
                Me.m_RunSpace.Run()
                Me.writeResponseAlterationResults(map, AltPercent)

                Me.RestoreResponse()

            Next map


        Catch ex As Exception

        End Try

        Me.RunsCompleted()

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
            'Me.m_waitLock.Set()
        Catch ex As Exception

        End Try

    End Sub

    Public Sub OnEcospaceRunCompleted()

        If Not Me.m_isConfig Then
            Return
        End If

        Me.SaveRun()
    

    End Sub

    Private Sub writeResponseAlterationResults(Map As IEnviroInputMap, PercentageOfChange As Single)

        Dim strm As StreamWriter
        strm = New StreamWriter(Me.RunParameters.OutputFileName, True)

        strm.Write(Map.Layer.Name + "," + PercentageOfChange.ToString)
        For igrp As Integer = 1 To Me.core.nGroups
            strm.Write("," + Me.m_curB(igrp).ToString)
        Next
        strm.WriteLine()
        strm.Close()

    End Sub



    Private Sub StoreResponse(map As IEnviroInputMap)

        m_lstResponseFunctions = New List(Of cResponseFunctionValuePair)
        For igrp As Integer = 1 To Me.core.nGroups
            Dim iResponseIndex As Integer = map.ResponseIndexForGroup(igrp)
            If iResponseIndex > 0 Then
                Dim ResponFunct As cForcingFunction = Me.core.CapacityShapeManager.Item(iResponseIndex - 1)
                m_lstResponseFunctions.Add(New cResponseFunctionValuePair(ResponFunct))
            End If
        Next
    End Sub

    Private Sub RestoreResponse()

        For Each pair In m_lstResponseFunctions
            pair.restore()
        Next
    End Sub


    Private Sub AlterResponse(map As IEnviroInputMap, PercentToAlter As Single)

        For igrp As Integer = 1 To Me.core.nGroups
            Dim iResponseIndex As Integer = map.ResponseIndexForGroup(igrp)
            If iResponseIndex > 0 Then
                'System.Console.WriteLine("Response for map = " + map.Layer.Name + " group = " + igrp.ToString + " response = " + iResponseIndex.ToString)
                Me.setResponseFunction(iResponseIndex, PercentToAlter)
            End If
        Next
    End Sub

    Private Sub setResponseFunction(iResponseIndex As Integer, Percentage As Single)
        Dim ResponFunct As cForcingFunction = Me.core.CapacityShapeManager.Item(iResponseIndex - 1)

        ResponFunct.LockUpdates()
        For ipt As Integer = 1 To ResponFunct.nPoints
            ResponFunct.ShapeData(ipt) = ResponFunct.ShapeData(ipt) * Percentage
        Next
        ResponFunct.UnlockUpdates()

    End Sub

    Private Sub SaveRun()
        'writeResults()
        'writeEcopathPars()
    End Sub

End Class
