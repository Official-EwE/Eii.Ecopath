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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


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

    Public BeforeRun As cRunPeriods
    Public AfterRun As cRunPeriods

    Public Sub New()
        BeforeRun = New cRunPeriods(1995, 10)
        AfterRun = New cRunPeriods(2015, 15)
    End Sub

End Class


Public Class cRunManager

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

    Private m_parNames() As String

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

        Dim msg As String = ""

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


    Public Sub Init(thePlugin As cEcospaceMonteCarloPluginPoint)
        Me.m_plugin = thePlugin
        core = Me.m_plugin.Core
        Me.m_RunSpace = New cRunEcospace

        m_plugin.MonteCarlo.maxEcopathTries = 1000000

        m_parNames = New String() {"Biomass", "P/B", "Q/B", "EE", "BA"}

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

        Me.SaveRun()

        ''Completed the second run
        ''let the MonteCarlo go
        'If Me.m_curSpaceRun = 2 Then
        '    Me.m_waitLock.Set()
        'End If

    End Sub


    Private Sub SaveRun()
        writeResults()
        writeEcopathPars()
    End Sub

    Private Sub writeResults()

        Try
            'Only save complete runs
            If Me.m_bStop Then Return

            Dim strm As StreamWriter
            strm = New StreamWriter(Me.RunParameters.OutputFileName, True)
            'filename for the output file
            'this allows a row of data to be recognised once all the data is merged into one file
            Dim filename As String = Path.GetFileName(Me.RunParameters.OutputFileName)

            Dim sumB As Single
            strm.Write(filename + ", " + Me.m_TrialNumber.ToString + ", " + RunType)
            For igrp As Integer = 1 To m_plugin.Core.nGroups
                sumB = 0
                'The Zero index in ResultsByGroup(type,group,year) is Biomass
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


    Public Function getEcopathParFile(Optional filename As String = "") As String
        'Create the Ecopath Parameters filename
        If String.IsNullOrWhiteSpace(filename) Then filename = Me.RunParameters.OutputFileName
        Dim parFile As String = Path.GetFileNameWithoutExtension(filename)
        parFile = String.Concat(parFile, "_Ecopath_Pars.csv")
        Return Path.Combine(Path.GetDirectoryName(filename), parFile)
    End Function


    Private Sub writeEcopathPars()

        Try
            'Only save complete runs
            If Me.m_bStop Then Return

            If String.Compare(RunType, "After", True) = 0 Then
                'Only do this for the before run
                'Parameters will be the same for both runs
                Return
            End If

            'Create the Ecopath Parameters filename
            Dim parFile As String = Me.getEcopathParFile

            Dim strm As StreamWriter
            strm = New StreamWriter(parFile, True)
            'filename for the output file
            'this allows a row of data to be recognised once all the data is merged into one file
            Dim filename As String = Path.GetFileName(Me.RunParameters.OutputFileName)
            Dim epdata As cEcopathDataStructures = Me.m_plugin.EcoPathData

            For ipar As Integer = 0 To 4
                strm.Write(filename + ", " + Me.m_TrialNumber.ToString + ", " + Me.m_parNames(ipar))
                For igrp As Integer = 1 To m_plugin.Core.nGroups
                    Dim value As Single
                    Select Case ipar
                        Case 0
                            value = epdata.B(igrp)
                        Case 1
                            value = epdata.PB(igrp)
                        Case 2
                            value = epdata.QB(igrp)
                        Case 3
                            value = epdata.EE(igrp)
                        Case 4
                            value = epdata.BA(igrp)
                    End Select

                    strm.Write(", " + value.ToString)

                Next igrp
                strm.WriteLine()
            Next ipar

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
