Imports Microsoft.Extensions.Logging
Namespace Logging

    Public Module LoggingContext
        Public Property LoggerFactory As ILoggerFactory

        ' Helper method to create a logger for a given type
        Public Function CreateLogger(Of T)() As ILogger

            ' Return a logger from the factory if available, otherwise return a NullLogger that does nothing
            If LoggerFactory IsNot Nothing Then
                Return LoggerFactory.CreateLogger(GetType(T).FullName)
            Else
                Return New NullLogger()
            End If
        End Function
    End Module
End Namespace