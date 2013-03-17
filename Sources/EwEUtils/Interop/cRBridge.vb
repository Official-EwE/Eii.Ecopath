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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System

#End Region ' 

Namespace Interop

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This class implements a simple connection to R. It facilitates <see cref="cRBridge.Execute">
    ''' execution of scripts from file or as a series of lines</see>.
    ''' </summary>
    ''' <remarks>
    ''' Note that the connection to R is established by rerouting the <see cref="Process.StandardInput"/>,
    ''' <see cref="Process.StandardOutput"/>, and <see cref="Process.StandardError"/> of the
    ''' R process. This code does not use COM to remain CRL-compliant.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cRBridge

#Region " Internals "

         ''' <summary>Output produced by R</summary>
        Private m_ROutput As New List(Of String)
        ''' <summary>Errors produced by R</summary>
        Private m_RErrors As New List(Of String)
        ''' <summary>Full path to R</summary>
        Private m_strPathToR As String = ""

#End Region ' Internals

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new R connection
        ''' </summary>
        ''' <param name="strPathToR">The path to the R executable.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(strPathToR As String)

            ' Sanity checks
            Debug.Assert(File.Exists(strPathToR), "Cannot find R at '" & strPathToR & "'")

            ' Store path
            Me.m_strPathToR = strPathToR

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Execute an R script.
        ''' </summary>
        ''' <param name="strScriptFile">The R script file to execute.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Execute(strScriptFile As String) As Boolean

            Dim RScriptReader As StreamReader = Nothing

            Try
                ' Try to open the file
                RScriptReader = New StreamReader(strScriptFile)
            Catch ex As Exception
                ' Kaboom
                Return False
            End Try

            Dim RScriptLines As New List(Of String)
            While (RScriptReader.Peek > 0)
                RScriptLines.Add(RScriptReader.ReadLine())
            End While

            ' Do not forget to clean up
            RScriptReader.Close()
            ' Done
            Return Me.Execute(RScriptLines.ToArray)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Execute an R script.
        ''' </summary>
        ''' <param name="RScriptLines">The script to execute.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Execute(RScriptLines As String()) As Boolean

            Dim RProcess As New Process()
            Dim RInputWriter As StreamWriter
            Dim Line As String
            Dim Success As Boolean

            ' Clean up results from previous R runs
            Me.m_RErrors.Clear()
            Me.m_ROutput.Clear()

            ' ----------
            ' Configure how RProcess will run
            ' ----------

            ' We want to execute the process as close to this application as possible
            RProcess.StartInfo.UseShellExecute = False

            ' We want to connect to the process input, output and error streams
            RProcess.StartInfo.RedirectStandardInput = True
            RProcess.StartInfo.RedirectStandardOutput = True
            RProcess.StartInfo.RedirectStandardError = True

            ' The process needs to know where find its R program
            RProcess.StartInfo.FileName = Me.m_strPathToR
            ' R needs some options too, which are described in https://projects.uabgrid.uab.edu/r-group/wiki/CommandLineProcessing
            RProcess.StartInfo.Arguments = "--slave"

            ' We want to hide the R interface
            RProcess.StartInfo.CreateNoWindow = True

            Try
                ' Ok, the process is ready to run. Let's try this
                RProcess.Start()
            Catch ex As Exception
                ' Oh shoot. Abandon ship, women and debuggers first.
                Return False
            End Try

            ' ----------
            ' The R program has been successfully launched. Now, start feeding it with lines of script
            ' ----------

            ' First, create a connection to the input stream, through which we'll feed the lines of script
            RInputWriter = RProcess.StandardInput
            ' For all lines of text:
            For i As Integer = 0 To RScriptLines.Length - 1
                ' Pass the line to R
                RInputWriter.WriteLine(RScriptLines(i))
            Next
            ' Done writing, close the input stream
            RInputWriter.Close()

            ' Wait for R to finish. Wait for a certain number of milliseconds (here hardcoded to 5 minutes)
            Success = RProcess.WaitForExit(5 * 60 * 1000)

            ' Read whatever error text is available
            Line = RProcess.StandardError.ReadLine
            While (Line IsNot Nothing)
                Me.m_RErrors.Add(Line)
                Line = RProcess.StandardError.ReadLine
            End While

            ' Read whatever output text is available
            Line = RProcess.StandardOutput.ReadLine
            While (Line IsNot Nothing)
                Me.m_ROutput.Add(Line)
                Line = RProcess.StandardOutput.ReadLine
            End While

            ' Clean up
            RProcess.Dispose()

            Return Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the error lines produced by the last R run.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Errors As String()
            Get
                Return Me.m_RErrors.ToArray
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the output produced by the last R run.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Output As String()
            Get
                Return Me.m_ROutput.ToArray
            End Get
        End Property

    End Class

End Namespace
