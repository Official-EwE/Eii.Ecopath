' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Liveness marker for a staged working copy: the "{copy}.marker" file, held open with
    ''' <see cref="FileShare.None"/> while the owner is alive, so the orphan sweep leaves the copy alone.
    ''' </summary>
    ''' <remarks>
    ''' Deliberately not SQLite's own locking, and it never guards the data file itself. A crashed holder needs no
    ''' cleanup: the OS releases the handle when the process dies, so the next TryAcquire simply succeeds.
    ''' </remarks>
    ''' =======================================================================
    Public Class cEwEExclusivityLock
        Implements IDisposable

        Private m_lockStream As FileStream = Nothing
        Private m_strLockPath As String = ""
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEExclusivityLock)()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attempt to become the exclusive holder of <paramref name="strLockFilePath"/>.
        ''' </summary>
        ''' <param name="strLockFilePath">Full path of the marker file itself,
        ''' i.e. "{staged copy}.marker".</param>
        ''' <returns>True if the lock was acquired (or was already held by
        ''' this instance); False if another session currently holds it.</returns>
        ''' -------------------------------------------------------------------
        Public Function TryAcquire(strLockFilePath As String) As Boolean

            If Me.IsHeld Then Return True

            Me.m_strLockPath = strLockFilePath
            Try
                Me.m_lockStream = New FileStream(Me.m_strLockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
                Try
                    File.SetAttributes(Me.m_strLockPath, FileAttributes.Hidden)
                Catch
                    ' Cosmetic only; must never fail the lock.
                End Try
                Return True

            Catch ex As IOException
                ' Sharing violation: someone else already holds it.
                m_logger.LogInformation("Marker '{0}' is currently held by another session.", strLockFilePath)
                Me.m_lockStream = Nothing
                Return False

            Catch ex As UnauthorizedAccessException
                ' e.g. read-only share: same as "cannot claim", not a hard error.
                m_logger.LogWarning(ex, "Unable to create marker file '{0}': {1}", strLockFilePath, ex.Message)
                Me.m_lockStream = Nothing
                Return False
            End Try

        End Function

        ''' <summary>True if this instance currently holds the lock.</summary>
        Public ReadOnly Property IsHeld As Boolean
            Get
                Return Me.m_lockStream IsNot Nothing
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Release the lock, if held. Safe to call when not held.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Release()
            If Me.m_lockStream Is Nothing Then Return
            Try
                Me.m_lockStream.Close()
            Catch
                ' NOP - releasing must never throw
            Finally
                Me.m_lockStream.Dispose()
                Me.m_lockStream = Nothing
            End Try
            Try
                ' Best-effort; a leftover marker is inert and reopened by the next TryAcquire.
                If Not String.IsNullOrEmpty(Me.m_strLockPath) AndAlso File.Exists(Me.m_strLockPath) Then
                    File.Delete(Me.m_strLockPath)
                End If
            Catch
            End Try
            Me.m_strLockPath = ""
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Release()
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace
