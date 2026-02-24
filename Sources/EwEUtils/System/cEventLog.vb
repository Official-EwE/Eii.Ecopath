' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System
Imports System.Diagnostics

''' ---------------------------------------------------------------------------
''' <summary>
''' Utility class for writing entries to the Windows Event log.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEventLog

    Private m_strAppName As String = ""

    ''' <summary>
    ''' Create a n
    ''' </summary>
    ''' <param name="strAppName">Name of Client Application. Needed because before 
    ''' writing to event log, you must have a named EventLog source.</param>
    ''' <param name="strLogName">Name of Log (System, Application, Security is 
    ''' read-only) If you specify a non-existent log, the log will be created</param>
    ''' <remarks></remarks>
    Public Sub New(strAppName As String,
                   Optional strLogName As String = "Application")

        Me.m_strAppName = strAppName

        'Register the App as an Event Source
        If Not EventLog.SourceExists(strAppName) Then
            EventLog.CreateEventSource(strAppName, strLogName)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write entry to the Windows Event log.
    ''' </summary>
    ''' <param name="strEntry">Value to write</param>
    ''' <param name="eventlogentry"><see cref="EventLogEntryType">Entry Type</see>.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function WriteToEventLog(strEntry As String,
                                    Optional eventlogentry As EventLogEntryType = EventLogEntryType.Information) As Boolean

        Dim objEventLog As New EventLog()
        Dim bSucces As Boolean = True

        objEventLog.Source = Me.m_strAppName
        Try
            objEventLog.WriteEntry(strEntry, eventlogentry)
        Catch Ex As Exception
            bSucces = False
        End Try

        Try
            objEventLog.Close()
            objEventLog.Dispose()
        Catch ex As Exception

        End Try

        Return bSucces

    End Function

End Class
