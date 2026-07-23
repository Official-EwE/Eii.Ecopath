' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports Microsoft.Extensions.Logging

Namespace Logging

    ''' <summary>
    ''' A logger wrapper that writes all log messages to System.Diagnostics.Trace with appropriate severity levels.
    ''' This allows external applications using the EwECore library to capture log messages
    ''' through TraceListener (e.g., Trace.Listeners.Add(new CustomConsoleTraceListener())).
    ''' Maps LogLevel.Error/Critical to TraceError, Warning to TraceWarning, Information to TraceInformation, etc.
    ''' </summary>
    Public Class TraceErrorLogger
        Implements ILogger

        Private ReadOnly _innerLogger As ILogger

        Public Sub New(innerLogger As ILogger)
            _innerLogger = innerLogger
        End Sub

        Public Function BeginScope(Of TState)(state As TState) As IDisposable Implements ILogger.BeginScope
            Return _innerLogger.BeginScope(state)
        End Function

        Public Function IsEnabled(logLevel As LogLevel) As Boolean Implements ILogger.IsEnabled
            Return _innerLogger.IsEnabled(logLevel)
        End Function

        Public Sub Log(Of TState)(logLevel As LogLevel,
                                  eventId As EventId,
                                  state As TState,
                                  exception As Exception,
                                  formatter As Func(Of TState, Exception, String)) Implements ILogger.Log

            ' Format the message
            Dim message As String = If(formatter IsNot Nothing,
                                    formatter(state, exception),
                                    state?.ToString())

            ' Build comprehensive message including all inner exceptions if present
            Dim traceMessage As String
            If exception IsNot Nothing Then
                Dim sb As New System.Text.StringBuilder()
                sb.Append(exception.Message)

                Dim innerEx As Exception = exception.InnerException
                While innerEx IsNot Nothing
                    sb.Append(" --> ")
                    sb.Append(innerEx.Message)
                    innerEx = innerEx.InnerException
                End While

                traceMessage = sb.ToString()
            Else
                traceMessage = message
            End If

            ' Write to Trace with appropriate severity level
            Select Case logLevel
                Case LogLevel.Critical, LogLevel.Error
                    System.Diagnostics.Trace.TraceError(traceMessage)
                Case LogLevel.Warning
                    System.Diagnostics.Trace.TraceWarning(traceMessage)
                Case LogLevel.Information
                    System.Diagnostics.Trace.TraceInformation(traceMessage)
                Case LogLevel.Debug
                    System.Diagnostics.Trace.WriteLine(traceMessage, "Debug")
                Case LogLevel.Trace
                    System.Diagnostics.Trace.WriteLine(traceMessage, "Trace")
            End Select

            ' Call the underlying logger
            _innerLogger.Log(logLevel, eventId, state, exception, formatter)
        End Sub
    End Class

End Namespace
