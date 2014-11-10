Option Explicit On
Option Strict On


Imports EwECore
Imports System.IO
Imports System.Threading

'Public Class cRunPeriods

'    Public StartYear As Integer
'    Public nYears As Single

'    Public Sub New(Start As Integer, NumberOfYears As Integer)
'        StartYear = Start
'        nYears = NumberOfYears
'    End Sub

'    Public Sub New(theCore As cCore)
'        StartYear = theCore.EwEModel.FirstYear
'        nYears = theCore.EcospaceModelParameters.TotalTime
'    End Sub


'End Class


Public Class cRunParameters

    Public OutputFileName As String
    'Public RunTimes As cRunPeriods
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

        'Use the current Ecospace configuration
        'as the default years
        'RunTimes = New cRunPeriods(Me.m_core)
        Delta = 0.9 ' 0.2
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

    Public Enum eEcospaceSensitivityStates
        Stopped
        Running
    End Enum

    Private Class cResponseFunctionValuePair
        Public orgData() As Single
        Public orgResponseFunct As cEnviroResponseFunction

        Public Sub New(Shape As cEnviroResponseFunction)
            orgResponseFunct = Shape
            Me.Store()
        End Sub

        Public Sub Store()
            'Store the original value of the response function
            orgData = New Single(orgResponseFunct.ShapeData.Length - 1) {}
            Array.Copy(orgResponseFunct.ShapeData, orgData, orgResponseFunct.ShapeData.Length)
        End Sub

        Public Sub Restore()
            orgResponseFunct.ShapeData = orgData
        End Sub

        Public Sub Alter(PercentageToAlter As Single)

            For ipt As Integer = 1 To orgResponseFunct.nPoints
                orgResponseFunct.ShapeData(ipt) = orgResponseFunct.ShapeData(ipt) * PercentageToAlter
            Next

        End Sub

    End Class


    ''' <summary>Core thread synchronization object for thread marshalling.</summary>
    Private m_SyncObj As System.Threading.SynchronizationContext = Nothing

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

    Private m_curTotTime As Integer
    Private m_curTimeStep As Integer
    Private m_curMapName As String

    Private m_TotTimeSteps As Integer
    Private m_RunTimeSteps As Integer

    Private m_curState As eEcospaceSensitivityStates

    Public Event OnProgress(TotalPercentDone As Single, RunPercentDone As Single, MapName As String)

    Public Event OnStateChange(State As eEcospaceSensitivityStates)

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


    Private Sub RunOnThread()

        Try
            Dim AltPercent As Single

            Me.m_bStop = False

            Me.initForRun()

            'Run and Write Baseline run
            Me.m_RunSpace.Run()
            Me.m_curMapName = "BaseLine"
            Me.SaveRun(Me.m_curMapName, 0.0)

            For Each map As IEnviroInputMap In Me.RunParameters.lstLayers
                If Me.m_bStop Then Exit For

                Me.m_curMapName = map.Layer.Name

                AltPercent = Me.RunParameters.LowerBound
                Me.StoreOrginalResponse(map)

                Me.AlterResponse(AltPercent)
                Me.m_RunSpace.Run()
                If Me.m_bStop Then Exit For
                Me.SaveRun(map.Layer.Name, AltPercent)

                Me.RestoreResponse()

                AltPercent = Me.RunParameters.UpperBound
                Me.AlterResponse(AltPercent)
                Me.m_RunSpace.Run()
                If Me.m_bStop Then Exit For
                Me.SaveRun(map.Layer.Name, AltPercent)

                Me.RestoreResponse()

            Next map


        Catch ex As Exception

        End Try

        Me.RunsCompleted()

    End Sub

    Private Sub initForRun()


        Me.m_RunSpace.Init(Me.m_plugin.Core, Me.m_plugin.EcoSpace)
        Me.m_TotTimeSteps = Me.calcTotalTimeStep()
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



    Private Sub StoreOrginalResponse(map As IEnviroInputMap)

        m_lstResponseFunctions = New List(Of cResponseFunctionValuePair)
        For igrp As Integer = 1 To Me.core.nGroups
            Dim iResponseIndex As Integer = map.ResponseIndexForGroup(igrp)
            If iResponseIndex > 0 Then
                Dim ResponFunct As cEnviroResponseFunction = DirectCast(Me.core.CapacityShapeManager.Item(iResponseIndex - 1), cEnviroResponseFunction)
                m_lstResponseFunctions.Add(New cResponseFunctionValuePair(ResponFunct))
            End If
        Next

    End Sub

    Private Sub RestoreResponse()

        For Each pair As cResponseFunctionValuePair In m_lstResponseFunctions
            pair.Restore()
        Next

    End Sub


    Private Sub AlterResponse(PercentToAlter As Single)

        For Each pair As cResponseFunctionValuePair In m_lstResponseFunctions
            pair.Alter(PercentToAlter)
        Next

    End Sub


    Public Sub New()

        Me.m_SyncObj = System.Threading.SynchronizationContext.Current
        'if there is no current context then create a new one on this thread. 
        If (Me.m_SyncObj Is Nothing) Then Me.m_SyncObj = New System.Threading.SynchronizationContext()

        Me.m_curState = eEcospaceSensitivityStates.Stopped

    End Sub
End Class
