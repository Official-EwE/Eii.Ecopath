' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.Extensions.Logging

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Database compact class for JRO databases.
    ''' </summary>
    ''' =======================================================================
    Public Class cCompactJRO
        Implements IDatabaseCompact

#Region " Private vars "

        ''' <summary>Global, one time flags.</summary>
        Private Shared s_bEngineSearched As Boolean = False
        ''' <summary>Global, one time flags.</summary>
        Private Shared s_bEngineFound As Boolean = False
        Private Shared ReadOnly s_logger As ILogger = LoggingContext.CreateLogger(Of cCompactJRO)()

#End Region ' Private vars 

#Region " IDatabaseCompact implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the OS supports compacting databases via JRO.
        ''' </summary>
        ''' <returns>True if the OS supports compacting databases via JRO.</returns>
        ''' -------------------------------------------------------------------
        Public Function CanCompact() As Boolean _
            Implements IDatabaseCompact.CanCompact

            If Not RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then Return False
            If cCompactJRO.s_bEngineSearched Then Return cCompactJRO.s_bEngineFound

            Dim engine As Object = Nothing
            Try
                engine = CreateEngine()
                cCompactJRO.s_bEngineFound = (engine IsNot Nothing)
            Catch ex As Exception When IsEngineUnavailable(ex)
                s_logger.LogError(ex, "JRO database compaction is unavailable.")
                cCompactJRO.s_bEngineFound = False
            Finally
                If engine IsNot Nothing Then Marshal.FinalReleaseComObject(engine)
            End Try

            cCompactJRO.s_bEngineSearched = True
            Return cCompactJRO.s_bEngineFound

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact a database via JRO.
        ''' </summary>
        ''' <param name="strFileFrom">Source database location.</param>
        ''' <param name="strConnectionFrom">Source database connection string.</param>
        ''' <param name="strFileTo">Target database location.</param>
        ''' <param name="strConnectionTo">Target database connection string.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">database access
        ''' result code</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Function Compact(strFileFrom As String,
                                strConnectionFrom As String,
                                strFileTo As String,
                                strConnectionTo As String) As eDatasourceAccessType _
            Implements IDatabaseCompact.Compact

            If Not Me.CanCompact() Then Return eDatasourceAccessType.Failed_OSUnsupported

            Dim engine As Object = Nothing
            Try
                engine = CreateEngine()
            Catch ex As Exception When IsEngineUnavailable(ex)
                s_logger.LogError(ex, "Unable to create the JRO database compaction engine.")
                Return eDatasourceAccessType.Failed_OSUnsupported
            End Try

            Try
                CompactDatabase(engine, strConnectionFrom, strConnectionTo)
                Return eDatasourceAccessType.Success
            Catch ex As COMException
                s_logger.LogError(ex, "JRO database compaction failed.")
                Return eDatasourceAccessType.Failed_Unknown
            Finally
                If engine IsNot Nothing Then Marshal.FinalReleaseComObject(engine)
            End Try

        End Function

#End Region ' IDatabaseCompact implementation

        ' Keep interop types outside the guarded callers: the JIT can load their assembly before entering a Try block.
        <MethodImpl(MethodImplOptions.NoInlining)>
        Private Shared Function CreateEngine() As Object
            Return New JRO.JetEngine()
        End Function

        <MethodImpl(MethodImplOptions.NoInlining)>
        Private Shared Sub CompactDatabase(engine As Object, connectionFrom As String, connectionTo As String)
            DirectCast(engine, JRO.JetEngine).CompactDatabase(connectionFrom, connectionTo)
        End Sub

        Private Shared Function IsEngineUnavailable(ex As Exception) As Boolean
            Return TypeOf ex Is COMException OrElse
                   TypeOf ex Is PlatformNotSupportedException OrElse
                   TypeOf ex Is FileNotFoundException OrElse
                   TypeOf ex Is FileLoadException OrElse
                   TypeOf ex Is BadImageFormatException
        End Function

    End Class

End Namespace ' Database
