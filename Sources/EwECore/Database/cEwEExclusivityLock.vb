' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' OS-level exclusivity guard, based on a lock file opened with
    ''' <see cref="FileShare.None"/>. Used both as the write-exclusivity
    ''' lock for a shared data file, and as the liveness marker for a
    ''' session's local read-only copy of that file - see cEwEEFDatabase.
    ''' </summary>
    ''' <remarks>
    ''' <para>This deliberately does NOT use SQLite's own file locking
    ''' (<c>PRAGMA locking_mode</c>). Guarding against genuinely external
    ''' tools (e.g. DB Browser for SQLite) opening the file concurrently is
    ''' out of scope for this class - see FILE_LOCK_HANDOFF.md. This class
    ''' only arbitrates between multiple EwECore-based sessions.</para>
    ''' <para>Recovery from a crashed holder is automatic: the OS releases
    ''' the file handle the instant the holding process dies, so the next
    ''' <see cref="TryAcquire"/> call from anywhere just succeeds. No
    ''' PID/heartbeat bookkeeping is needed.</para>
    ''' </remarks>
    ''' =======================================================================
    Friend Class cEwEExclusivityLock
        Implements IDisposable

        Private m_lockStream As FileStream = Nothing
        Private m_strLockPath As String = ""
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEExclusivityLock)()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attempt to become the exclusive holder of <paramref name="strLockFilePath"/>.
        ''' </summary>
        ''' <param name="strLockFilePath">Full path of the lock file itself
        ''' (caller decides the name/suffix - e.g. "model.ewesqlite.lock" for
        ''' a write-exclusivity lock, or "{copy}.marker" for a read-only-copy
        ''' liveness marker).</param>
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
                    ' Cosmetic only (keeps the lock file out of the way on a
                    ' shared folder) - never let this fail the actual lock.
                End Try
                Return True

            Catch ex As IOException
                ' Sharing violation: someone else already holds it.
                m_logger.LogInformation("Exclusivity lock '{0}' is currently held by another session.", strLockFilePath)
                Me.m_lockStream = Nothing
                Return False

            Catch ex As UnauthorizedAccessException
                ' e.g. a read-only share/permissions issue - treat the same
                ' as "cannot claim exclusivity", not as a hard error.
                m_logger.LogWarning(ex, "Unable to create exclusivity lock file '{0}': {1}", strLockFilePath, ex.Message)
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
                ' Best-effort tidy-up. If this fails, an inert leftover lock
                ' file is harmless - the next TryAcquire on any machine just
                ' reopens and truncates it via OpenOrCreate.
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
