' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection
Imports EwECore.Common
Imports EwECore.Plugins.Ecospace
Imports Microsoft.Extensions.Logging



''' ---------------------------------------------------------------------------
''' <summary>
''' Factory class for creating an <see cref="IEcospaceResultsWriter"/>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceResultWriterFactory

    Private Shared ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceResultWriterFactory)()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all the Ecospace result writers provided by the EwE core and plug-ins
    ''' </summary>
    ''' <param name="pm">The plug-in manager instance to consult, if any.</param>
    ''' <returns>An array of all avaliable result writers.</returns>
    ''' -----------------------------------------------------------------------
    Friend Shared Function GetWriters(pm As cPluginManager) As IEcospaceResultsWriter()

        Dim writers As New List(Of IEcospaceResultsWriter)

        Try

            For Each t As Type In Assembly.GetAssembly(GetType(cCore)).GetTypes()

                If (GetType(IEcospaceResultsWriter).IsAssignableFrom(t) And Not t.IsAbstract()) Then
                    Try
                        writers.Add(CType(Activator.CreateInstance(t), IEcospaceResultsWriter))
                    Catch ex As Exception
                        m_logger.LogError(ex, "cEcospaceResultWriterFactory.GetWriters() Failed to create instance of IEcospaceResultsWriter")
                    End Try
                End If
            Next

            ' Plug-in manager provided?
            If (pm IsNot Nothing) Then
                Try
                    ' #Yes: see if a plug-in based writer supports the requested format
                    For Each ip As IEcospaceResultWriterPlugin In pm.GetPlugins(GetType(IEcospaceResultWriterPlugin))
                        writers.Add(ip)
                    Next
                Catch ex As Exception
                    m_logger.LogError(ex, "cEcospaceResultWriterFactory.GetWriters() Failed to create instance of IEcospaceResultWriterPlugin")
                End Try
            End If

        Catch ex As Exception
            m_logger.LogError(ex, "cEcospaceResultWriterFactory.GetWriters()")
        End Try

        Return writers.ToArray()

    End Function


End Class
