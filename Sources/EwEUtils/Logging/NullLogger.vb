Imports Microsoft.Extensions.Logging

Public Class NullLogger
    Implements ILogger

    Public Function BeginScope(Of TState)(state As TState) As IDisposable Implements ILogger.BeginScope
        Return Nothing
    End Function

    Public Function IsEnabled(logLevel As LogLevel) As Boolean Implements ILogger.IsEnabled
        Return False
    End Function

    Public Sub Log(Of TState)(logLevel As LogLevel,
                              eventId As EventId,
                              state As TState,
                              exception As Exception,
                              formatter As Func(Of TState, Exception, String)) Implements ILogger.Log
        ' Do nothing
    End Sub
End Class
