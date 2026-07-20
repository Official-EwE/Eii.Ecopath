' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports Microsoft.Extensions.Logging
Namespace Logging

    Public Module LoggingContext
        Public Property LoggerFactory As ILoggerFactory

        ' Helper method to create a logger for a given type
        Public Function CreateLogger(Of T)() As ILogger
            Dim innerLogger As ILogger

            ' Return a logger from the factory if available, otherwise return a NullLogger that does nothing
            If LoggerFactory IsNot Nothing Then
                innerLogger = LoggerFactory.CreateLogger(GetType(T).FullName)
            Else
                innerLogger = New NullLogger()
            End If

            ' Wrap the logger with TraceErrorLogger to write to Trace on LogError calls
            Return New TraceErrorLogger(innerLogger)
        End Function

        Public Property LogFile As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ecopath with Ecosim", "Logs", $"log-{DateTime.Now:yyyyMMdd}.txt")
    End Module
End Namespace