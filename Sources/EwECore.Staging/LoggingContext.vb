' SPDX-License-Identifier: EUPL-1.2
'
' STUB - the real LoggingContext lives in EwEUtils.Utilities (not part of
' this isolated staging-only test project) and presumably wires up real
' logging sinks. This stub provides just enough for cEwEDatabase.vb and
' cEwEExclusivityLock.vb to compile and run: a real, standard no-op logger
' (Microsoft.Extensions.Logging.Abstractions.NullLogger) rather than an
' invented fake - log calls succeed and do nothing, which is fine for
' testing the staging logic itself.

Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Logging.Abstractions

Namespace Global.EwEUtils.Utilities

    Public Class LoggingContext
        Public Shared Function CreateLogger(Of T)() As ILogger
            Return NullLogger(Of T).Instance
        End Function
    End Class

End Namespace
