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
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports EwEUtils.Utilities

#End Region ' Imports

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

#Region " Private vars "

        ''' <summary>Input sent to R</summary>
        Private m_RInput As New List(Of String)
        ''' <summary>Output produced by R</summary>
        Private m_ROutput As New List(Of String)
        ''' <summary>Errors produced by R</summary>
        Private m_RErrors As New List(Of String)
        ''' <summary>Full path to R</summary>
        Private m_strPathToR As String = ""
        ''' <summary>Disctionary of fields to replace in the script</summary>
        Private m_dtFields As New Dictionary(Of String, String)

#End Region ' Private vars

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
        Public Function ExecuteFile(strScriptFile As String) As Boolean

            Dim RScriptReader As StreamReader = Nothing

            Me.Clear()

            Try
                ' Try to open the file
                RScriptReader = New StreamReader(strScriptFile)
            Catch ex As Exception
                ' Kaboom
                Me.m_RErrors.Add(ex.Message)
                If (ex.InnerException IsNot Nothing) Then
                    Me.m_RErrors.Add(ex.InnerException.Message)
                End If
                Return False
            End Try

            ' Read script lines
            Dim RScriptLines As New List(Of String)
            While (RScriptReader.Peek > 0)
                RScriptLines.Add(RScriptReader.ReadLine())
            End While

            ' Do not forget to clean up
            RScriptReader.Close()

            ' Execute R on script lines
            Return Me.Execute(RScriptLines.ToArray)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clean up left-overs from previous R runs.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Clear()
            Me.m_RInput.Clear()
            Me.m_RErrors.Clear()
            Me.m_ROutput.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Execute an R script provided as a block of text
        ''' </summary>
        ''' <param name="Script">The script to execute.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Execute(Script As String) As Boolean
            Return Me.Execute(Script.Split(New String() {cStringUtils.vbCrLf, cStringUtils.vbCr, cStringUtils.vbLf}, _
                                           StringSplitOptions.RemoveEmptyEntries))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Execute an R script provided as a string collection.
        ''' </summary>
        ''' <param name="RScriptLines">The script to execute.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Execute(RScriptLines As ICollection(Of String)) As Boolean

            Dim Rwrapper As New Process()
            Dim bSuccess As Boolean

            Me.Clear()

            ' ----------
            ' Configure how RProcess will run
            ' ----------

            ' Execute R in the current memory space
            Rwrapper.StartInfo.UseShellExecute = False

            ' Connect to the R input, output and error streams
            Rwrapper.StartInfo.RedirectStandardInput = True
            Rwrapper.StartInfo.RedirectStandardOutput = True
            Rwrapper.StartInfo.RedirectStandardError = True

            ' Point out the R executable
            Rwrapper.StartInfo.FileName = Me.m_strPathToR
            ' Set R command line options (see https://projects.uabgrid.uab.edu/r-group/wiki/CommandLineProcessing)
            Rwrapper.StartInfo.Arguments = "--slave"

            ' Suppress R user interface
            Rwrapper.StartInfo.CreateNoWindow = Not Me.ShowRInterface

            ' The process is ready to run
            Try
                ' Launch R
                Rwrapper.Start()
            Catch ex As Exception
                ' Shoot! Something went wrong. Pass error information out
                Me.m_RErrors.Add(ex.Message)
                If (ex.InnerException IsNot Nothing) Then
                    Me.m_RErrors.Add(ex.InnerException.Message)
                End If
                ' Abandon ship, women and debuggers first.
                Return False
            End Try

            ' ----------
            ' The R program has been successfully launched. Now, start feeding it with lines of script
            ' ----------

            ' Process input lines
            For Each strLine As String In RScriptLines
                ' Write each individual script line to R
                For Each strKey As String In Me.m_dtFields.Keys
                    strLine = strLine.Replace(strKey, Me.m_dtFields(strKey))
                Next
                Me.m_RInput.Add(strLine)
                Rwrapper.StandardInput.WriteLine(strLine)
            Next

            ' Wait for R to finish. Wait for a certain number of milliseconds (here hard-coded to max. 30 seconds)
            bSuccess = Rwrapper.WaitForExit(30 * 1000)

            ' Read whatever output text is available
            While (Rwrapper.StandardOutput.Peek > 0)
                Me.m_ROutput.Add(Rwrapper.StandardOutput.ReadLine)
             End While

            ' Read whatever error text is available
            While (Rwrapper.StandardError.Peek > 0)
                Me.m_RErrors.Add(Rwrapper.StandardError.ReadLine)
                ' Script contained errors
                bSuccess = False
            End While

            ' Clean up
            Rwrapper.Close()
            Rwrapper.Dispose()

            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a field to replace in the script
        ''' </summary>
        ''' <param name="strFieldName"></param>
        ''' -------------------------------------------------------------------
        Public Property Field(strFieldName As String) As String
            Get
                If (Me.m_dtFields.ContainsKey(strFieldName)) Then
                    Return Me.m_dtFields(strFieldName)
                End If
                Return String.Empty
            End Get
            Set(value As String)

                If (String.IsNullOrWhiteSpace(strFieldName)) Then Return

                If String.IsNullOrWhiteSpace(value) Then
                    If (Me.m_dtFields.ContainsKey(strFieldName)) Then
                        Me.m_dtFields.Remove(strFieldName)
                    End If
                Else
                    Me.m_dtFields(strFieldName) = value
                End If

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the R interface should be shown while a script is
        ''' executed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShowRInterface As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the input lines sent to during the last R run.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Input As String()
            Get
                Return Me.m_RInput.ToArray
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
        ''' Get whether the last R script ran successfully, i.e. when the script
        ''' ran without producing errors.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property LastRunSuccess As Boolean
            Get
                Return (Me.m_RErrors.Count = 0)
            End Get
        End Property

    End Class

End Namespace
