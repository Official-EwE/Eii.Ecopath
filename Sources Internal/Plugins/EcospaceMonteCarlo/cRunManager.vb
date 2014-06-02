
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

    Public BeforeRun As cRunPeriods
    Public AfterRun As cRunPeriods

    Public Sub New()
        BeforeRun = New cRunPeriods(2000, 10)
        AfterRun = New cRunPeriods(2018, 12)
    End Sub

End Class


Public Class cRunManager

    Public OutputFilename As String

    Private m_RunSpace As cRunEcospace
    Private m_plugin As cEcospaceMonteCarloPluginPoint

    Private core As cCore
    Private RunType As String
    Private m_isConfig As Boolean

    Private m_TrialNumber As Integer
    Private m_waitLock As ManualResetEvent

    'Private m_curSpaceRun As Integer
    Private m_bStop As Boolean

    Private m_parameters As New cRunParameters

    Public Property RunParameters As cRunParameters

        Get
            Return Me.m_parameters
        End Get
        Set(value As cRunParameters)
            Me.m_parameters = value
        End Set
    End Property



    Public Sub StopRun()
        Me.m_plugin.MonteCarlo.StopTrial = True
        Me.m_plugin.EcoSpace.m_StopRun = True
        m_bStop = True
    End Sub

    Public Sub isConfigured()
        Me.m_isConfig = True

        Dim msg As String
        If Not Directory.Exists(Path.GetDirectoryName(OutputFilename)) Then
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


    Public Sub Init(thePlugin As cEcospaceMonteCarloPluginPoint)
        Me.m_plugin = thePlugin
        core = Me.m_plugin.Core
        Me.m_RunSpace = New cRunEcospace

        m_plugin.MonteCarlo.maxEcopathTries = 1000000

    End Sub

    Public Function Run(ByVal WaitLock As ManualResetEvent, ByVal TrialNumber As Integer) As Boolean

        If Not Me.m_isConfig Then
            Return False
        End If

        m_TrialNumber = TrialNumber
        m_waitLock = WaitLock



        m_waitLock.Reset()
        Dim runthread As New Thread(AddressOf RunOnThread)
        runthread.Start()

        Return True

    End Function


    Private Sub RunOnThread()

        Try

            m_bStop = False
            m_RunSpace.Init(Me.m_plugin.Core, Me.m_plugin.MonteCarlo, Me.m_plugin.EcoSpace)
            ' m_curSpaceRun = 1
            RunType = "Before"
            m_RunSpace.SetRunParameters(Me.RunParameters.BeforeRun)
            Me.m_RunSpace.Run()

            ' m_curSpaceRun = 2

            If Not Me.m_bStop Then
                RunType = "After"
                m_RunSpace.SetRunParameters(Me.RunParameters.AfterRun)
                Me.m_RunSpace.Run()
            End If

        Catch ex As Exception

        End Try

        Me.RunsCompleted()

    End Sub

    Private Sub RunsCompleted()
        Try
            Me.m_waitLock.Set()
        Catch ex As Exception

        End Try

    End Sub

    Public Sub OnEcospaceRunCompleted()

        If Not Me.m_isConfig Then
            Return
        End If

        Me.getResults()

        ''Completed the second run
        ''let the MonteCarlo go
        'If Me.m_curSpaceRun = 2 Then
        '    Me.m_waitLock.Set()
        'End If

    End Sub

    Private Sub getResults()

        Try
            'Don't save the results when the run was stopped
            If Me.m_bStop Then Return

            Dim strm As StreamWriter
            strm = New StreamWriter(Me.OutputFilename, True)
            'filename for the output file
            'this allows a row of data to be recognised once all the data is merged into one file
            Dim filename As String = Path.GetFileName(Me.OutputFilename)

            Dim sumB As Single
            strm.Write(filename + ", " + Me.m_TrialNumber.ToString + ", " + RunType)
            For igrp As Integer = 1 To m_plugin.Core.nGroups
                sumB = 0

                For it As Integer = Me.m_RunSpace.StartOfLastYear To Me.m_RunSpace.StartOfLastYear + Me.m_RunSpace.nTimeStepPerYear
                    sumB += m_plugin.EcoSpaceData.ResultsByGroup(0, igrp, it)
                Next it

                'Average of the last year
                strm.Write(", " + (sumB / Me.m_RunSpace.nTimeStepPerYear).ToString)

            Next igrp
            strm.WriteLine()

            strm.Close()
            'System.Console.WriteLine()

        Catch ex As Exception

        End Try


    End Sub


    Public Sub configMonteCarlo()

        Dim MC As cMonteCarloManager = Me.core.EcosimMonteCarlo
        'For now set BA to 0 for all groups
        'until we sort out how to deal with the 
        'BA BA/B variation
        For igrp As Integer = 1 To core.nGroups
            MC.Groups(igrp).BAcv = 0.0
        Next


    End Sub


End Class
