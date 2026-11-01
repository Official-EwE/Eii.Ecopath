' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
'
' ============================================================================
' TRIMMED COPY - for standalone compilation/testing of the local-staging-copy
' mechanism only, backported to the master (Access-only) branch. This
' contains the real staging code reproduced verbatim from master's own
' cEwEDatabase.vb (post-backport) - the "Private vars and constants" and
' "Open and close" regions, copied exactly. Everything else in the real
' class (the nested cEwEDbWriter, Compatibility/Maintenance/Connection/
' Transaction/DB helper methods/Internals/EwE versioning regions) is
' excluded - this subset is only large enough to compile and test staging
' in isolation.
'
' The only addition beyond the verbatim staging code is a short block of
' the REMAINING abstract (MustOverride) member declarations further down,
' reproduced exactly from the real file so this class can compile
' standalone - these are signature-only declarations with no body to
' differ in the first place.
' ============================================================================

Imports System.Data
Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.InteropServices
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace Database

    Public MustInherit Class cEwEDatabase

#Region " Private vars and constants "

        ''' <summary>Current database version.</summary>
        Private m_sVersion As Single = 0.0
        ''' <summary>EwE version that produces the current database version.</summary>
        Private m_strEwEversion As String = ""
        ''' <summary>Database read-only state.</summary>
        Private m_bIsReadonly As Boolean = False
        ''' <summary>Directory associated with the database.</summary>
        Private m_strDirectory As String = ""

        ''' <summary>Oldest EwE5 version number supported</summary>
        Private Const cDBVERSION_EWE5_MIN As Single = 1.6!
        ''' <summary>Newest EwE5 version number supported</summary>
        Private Const cDBVERSION_EWE5_MAX As Single = 1.73!
        Protected ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEDatabase)()

#End Region ' Private vars and constants

#Region " Open and close "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new database.
        ''' </summary>
        ''' <param name="strDatabase">The database to create.</param>
        ''' <param name="strModelName">Name of the new model.</param>
        ''' <param name="strAuthor">Name of the author to add. May be omitted.</param>
        ''' <param name="databaseType">Type of the database to create. May be omitted.</param>
        ''' <param name="bOverwrite">States whether an existing database may be overwritten.</param>
        ''' <returns>True of created succesfully.</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Create(strDatabase As String,
                strModelName As String,
                Optional bOverwrite As Boolean = False,
                Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet,
                Optional strAuthor As String = "") As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a database.
        ''' </summary>
        ''' <param name="strDatabase">The database to open.</param>
        ''' <param name="databaseType">Type to use to open the database. Set this
        ''' to 'NotSet' to auto-detect the database type.</param>
        ''' <returns>True if connected succesfully.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Open(strDatabase As String,
                                          Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet,
                                          Optional bReadOnly As Boolean = False) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close an open connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Close()
            Me.m_sVersion = 0.0!
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the connected database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Name() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a given database to a new destination, and open this new database.
        ''' </summary>
        ''' <param name="strDatabaseTo">Target database name.</param>
        ''' <param name="strModelName">New name to assign to the model.</param>
        ''' <param name="bOverwrite">States whether any model in the way will be obliterated.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function SaveAs(strDatabaseTo As String,
                strModelName As String,
                Optional bOverwrite As Boolean = False,
                Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the database is read-only.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsReadOnly() As Boolean
            Get
                Return Me.m_bIsReadonly
            End Get
            Protected Set(value As Boolean)
                Me.m_bIsReadonly = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Opt-in switch for the local working-copy staging mechanism below.
        ''' Defaults to False: EwECore.dll is used by more than just the
        ''' interactive ScientificInterface app (e.g. EwERunConsole, or any
        ''' other future console/API consumer, per cCoreService), and
        ''' staging is pure cost with no possible benefit for a caller that
        ''' never calls Save() - it pays for a File.Copy on every open to
        ''' guard against a corruption scenario (a live editing session
        ''' colliding with a cloud sync client) that can't occur if nothing
        ''' ever writes back to the source file at all.
        ''' </summary>
        ''' <remarks>
        ''' A single, application-level flag rather than a per-call
        ''' parameter threaded through Open() - set ONCE, at startup, by
        ''' the interactive host application (see
        ''' EwE6ApplicationFramework.Main()). A per-call parameter would
        ''' put the burden on every future Core.LoadModel() call site to
        ''' remember to opt in correctly; a single startup flag means every
        ''' call anywhere in that host, present or future, is automatically
        ''' covered with nothing to remember at each call site. Any other
        ''' consumer of EwECore.dll that never sets this gets the safe
        ''' default (no staging, no unnecessary cost) with no action needed
        ''' on their part either.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Property EnableLocalStaging As Boolean = False

        ''' =======================================================================
        ''' <summary>
        ''' Local working-copy staging, shared by every concrete database type.
        ''' </summary>
        ''' <remarks>
        ''' <para>Cloud-sync clients (OneDrive, Dropbox, etc.) can rewrite a
        ''' file on disk out from under an application that has it open,
        ''' which can corrupt an Access or SQLite database mid-save. To guard
        ''' against this, every successful Open() stages a private local
        ''' working copy under
        ''' "%LocalAppData%\Ecopath with Ecosim\DBs\" and the actual database
        ''' connection (OleDb or EF/SQLite) is pointed at that copy, never at
        ''' the original file, for the entire session - but only when
        ''' EnableLocalStaging (above) is True. When it's False (the
        ''' default), Open() connects directly to the real file, exactly as
        ''' if this whole mechanism did not exist.</para>
        ''' <para>On Save, PublishLocalCopyToSource() blind-overwrites the
        ''' real file with the working copy's current bytes - no fingerprint
        ''' check, no conflict prompt.</para>
        ''' <para><b>Multi-user/multi-session protection is explicitly out of
        ''' scope</b> - a deliberate product decision. There is no lock of
        ''' any kind (no companion sidecar, no native Access exclusive mode)
        ''' anywhere in this class or its subclasses. If two sessions -
        ''' anywhere, on any kind of storage, including a plain local network
        ''' share - open the same file at the same time, that is the user's
        ''' own responsibility to avoid; the last one to save simply wins,
        ''' with no detection and no warning. This mechanism protects a
        ''' single session's own save from cloud-sync interference; it does
        ''' not, and is not intended to, protect against another session
        ''' editing the same file concurrently.</para>
        ''' </remarks>
        ''' =======================================================================
        Protected m_strRealFilePath As String = ""
        Protected m_strLocalWorkingCopyPath As String = ""
        Private ReadOnly m_workingCopyMarker As New cEwEExclusivityLock()

        Private Const cWorkingCopySubfolder As String = "Ecopath with Ecosim\DBs"

        Private Shared ReadOnly s_processExitHookLock As New Object()
        Private Shared s_processExitHookRegistered As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Stages a local, disposable working copy of strRealPath. Only the
        ''' file NAME of strRealPath is used - its directory is discarded, so
        ''' the working folder stays flat regardless of where models are
        ''' actually stored, per this feature's specification.
        ''' </summary>
        ''' <param name="strRealPath">The real (possibly cloud-synced) file
        ''' to stage a working copy of.</param>
        ''' <returns>The local working copy's full path. Callers should open
        ''' their actual database connection against THIS path, not
        ''' strRealPath.</returns>
        ''' -------------------------------------------------------------------
        Protected Function StageLocalWorkingCopy(strRealPath As String) As String
            cEwEDatabase.CleanUpOrphanedWorkingCopies()

            Dim strFolder As String = cEwEDatabase.GetWorkingCopyFolder()
            ' The real file's own extension is kept at the end (not ".tmp")
            ' so cDataSourceFactory.GetSupportedType - which determines a
            ' file's data source type purely from Path.GetExtension - still
            ' correctly recognizes the staged copy if it's ever opened
            ' directly by path, e.g. via frmEwE6.LoadEcopathModel. A ".tmp"
            ' extension here made the staged copy invisible to that
            ' detection entirely.
            Dim strTempName As String = "Temp" & Path.GetFileNameWithoutExtension(strRealPath) & "." & Guid.NewGuid().ToString() & Path.GetExtension(strRealPath)
            Dim strTempPath As String = Path.Combine(strFolder, strTempName)

            ' No retry here, deliberately - unlike PublishLocalCopyToSource
            ' (Save), there is no unsaved work at risk on Open, and no
            ' user-facing Retry/Cancel dialog paired with this path. A
            ' retry loop would only add latency before the same eventual
            ' failure, with no better outcome to show for it.
            File.Copy(strRealPath, strTempPath, True)
            Me.m_workingCopyMarker.TryAcquire(strTempPath & ".marker")
            cEwEDatabase.EnsureProcessExitHookRegistered()

            Me.m_strRealFilePath = strRealPath
            Me.m_strLocalWorkingCopyPath = strTempPath
            Return strTempPath
        End Function

        ' A sharing violation (ERROR_SHARING_VIOLATION) or lock violation
        ' (ERROR_LOCK_VIOLATION) is the specific, transient condition worth
        ' retrying - e.g. a cloud-sync client (OneDrive etc.) briefly
        ' holding the real file open to hash or sync it. Other IOExceptions
        ' (disk full, path too long, media removed) are not transient in
        ' the same way and should fail immediately rather than retry
        ' pointlessly.
        ''' <summary>HRESULT for ERROR_SHARING_VIOLATION (0x80070020), as the signed Integer .NET exceptions actually report.</summary>
        Private Const HR_ERROR_SHARING_VIOLATION As Integer = -2147024864
        ''' <summary>HRESULT for ERROR_LOCK_VIOLATION (0x80070021), as the signed Integer .NET exceptions actually report.</summary>
        Private Const HR_ERROR_LOCK_VIOLATION As Integer = -2147024863
        Private Const cPublishRetryAttempts As Integer = 6
        Private Const cPublishRetryDelayMs As Integer = 500

        ''' <summary>
        ''' Sidecar suffix used to stage the new content next to the real
        ''' file before the atomic rename in PublishLocalCopyToSource -
        ''' never written to directly by anything else, so collisions with
        ''' unrelated files/processes aren't a concern.
        ''' </summary>
        Private Const cPublishSidecarSuffix As String = ".publishing"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copies the local working copy's current bytes back over the real
        ''' file, unconditionally - no fingerprint check, no conflict
        ''' detection, no prompt. Called once per explicit user Save action
        ''' (see cCore.Save()), never per internal transaction.
        ''' </summary>
        ''' <returns>
        ''' True on success (or if nothing was staged - a no-op still
        ''' counts as success). False if the real file remained locked
        ''' (e.g. by a cloud-sync client) after exhausting all retry
        ''' attempts, or if some other, non-retryable I/O error occurred.
        ''' </returns>
        ''' <remarks>
        ''' <para>Callers must ensure all pending writes are actually
        ''' committed to the local working copy (e.g. via
        ''' CommitTransaction()/SaveChanges()) BEFORE calling this - it
        ''' operates purely on the working copy's current bytes on disk, it
        ''' does not itself flush anything.</para>
        ''' <para>Publishes via copy-to-sidecar, delete-original,
        ''' rename-sidecar-to-original - NOT a direct overwrite. A crash
        ''' mid-copy leaves the real file completely untouched (the sidecar
        ''' is written next to it, never in place of it, until the rename).
        ''' A crash after the delete but before the rename leaves the real
        ''' file genuinely missing - but the fully-intact new content sits
        ''' right next to it as "&lt;name&gt;.publishing", trivially recoverable
        ''' by hand even though there is no automated recovery for this.
        ''' Either way, what the direct-overwrite approach this replaced
        ''' could do - leave the real file's bytes themselves partially
        ''' written and corrupted - is no longer possible: the rename
        ''' itself is a single, atomic filesystem metadata operation on the
        ''' same volume (confirmed for NTFS), not a byte-by-byte copy, so
        ''' it cannot itself be interrupted into a half-done state.</para>
        ''' <para>Retries the whole sequence from the top, mechanically,
        ''' with a short delay, purely to ride out a brief sharing
        ''' violation on the real path (delete or rename, most likely) -
        ''' no UI, no dialog, no awareness of who is holding the file. A
        ''' caller that wants to ask the user to retry after this returns
        ''' False (see cCore.Save()) is responsible for that itself.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function PublishLocalCopyToSource() As Boolean
            If String.IsNullOrEmpty(Me.m_strLocalWorkingCopyPath) OrElse String.IsNullOrEmpty(Me.m_strRealFilePath) Then Return True

            Dim strSidecar As String = Me.m_strRealFilePath & cPublishSidecarSuffix

            For iAttempt As Integer = 1 To cPublishRetryAttempts
                Try
                    ' 1. Write the new content next to the real file, never
                    ' in place of it - the real file is untouched by this
                    ' step regardless of outcome.
                    File.Copy(Me.m_strLocalWorkingCopyPath, strSidecar, True)

                    ' 2. Remove the real file - a safe no-op if it doesn't
                    ' exist yet (first-ever publish to a brand-new path).
                    ' .NET's File.Move has no cross-target-platform
                    ' (net48/net10) overwrite overload, so the delete has
                    ' to happen explicitly, as its own step, before the
                    ' rename below.
                    If File.Exists(Me.m_strRealFilePath) Then File.Delete(Me.m_strRealFilePath)

                    ' 3. Atomic rename, same volume - either this fully
                    ' completes or it doesn't happen at all; there is no
                    ' partially-renamed state for it to be interrupted into.
                    File.Move(strSidecar, Me.m_strRealFilePath)

                    Return True

                Catch ex As IOException When ex.HResult = HR_ERROR_SHARING_VIOLATION OrElse ex.HResult = HR_ERROR_LOCK_VIOLATION
                    If iAttempt = cPublishRetryAttempts Then
                        m_logger.LogWarning(ex, "cEwEDatabase.PublishLocalCopyToSource('{0}'): still locked after {1} attempts.", Me.m_strRealFilePath, cPublishRetryAttempts)
                        Return False
                    End If
                    System.Threading.Thread.Sleep(cPublishRetryDelayMs)

                Catch ex As Exception
                    ' Not a sharing/lock violation - not the transient
                    ' condition this retry loop exists for. Fail
                    ' immediately. Note: if this fires after the delete but
                    ' before the rename, strSidecar still holds the
                    ' complete, correct content - not cleaned up here
                    ' deliberately, since it may be the only remaining copy
                    ' of the user's latest changes.
                    m_logger.LogError(ex, "cEwEDatabase.PublishLocalCopyToSource('{0}'): {1}", Me.m_strRealFilePath, ex.Message)
                    Return False
                End Try
            Next

            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases the working-copy marker (always succeeds immediately)
        ''' and deletes the local working copy, if any. Call from each
        ''' subclass's Close() override, AFTER that subclass has finished
        ''' closing its own connection to the working copy.
        ''' </summary>
        ''' <remarks>
        ''' This delete does NOT retry. A long-standing, still-unresolved
        ''' issue (predating this staging feature entirely) holds a
        ''' database connection's underlying file handle open for the rest
        ''' of THIS process's own lifetime, regardless of Close() - not a
        ''' brief pooling delay a short retry could ride out. A sleep-based
        ''' retry here was tried once already and found to have no effect
        ''' beyond risking an application-shutdown hang, and was removed
        ''' for that reason. If the delete fails, the file is simply left
        ''' behind - safe to do, since the marker above is already released
        ''' regardless, so the orphan sweep will correctly identify and
        ''' clean it up on a later Open() anywhere on this machine, or the
        ''' exit-time sweep will on this process's own clean exit (the one
        ''' point this handle is actually known to release).
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Sub CleanUpLocalWorkingCopy()
            Me.m_workingCopyMarker.Release()
            If String.IsNullOrEmpty(Me.m_strLocalWorkingCopyPath) Then Return
            cEwEDatabase.TryDeleteFileOnce(Me.m_strLocalWorkingCopyPath)
            Me.m_strLocalWorkingCopyPath = ""
            Me.m_strRealFilePath = ""
        End Sub

        Private Shared Function GetWorkingCopyFolder() As String
            Dim strFolder As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                cWorkingCopySubfolder)
            System.IO.Directory.CreateDirectory(strFolder) ' no-op if it already exists
            Return strFolder
        End Function

        Private Shared Function TryDeleteFileOnce(strPath As String) As Boolean
            Try
                If Not File.Exists(strPath) Then Return True
                File.Delete(strPath)
                Return True
            Catch
                Return False
            End Try
        End Function

        Private Shared Sub TryDeleteFile(strPath As String)
            Try
                If File.Exists(strPath) Then File.Delete(strPath)
            Catch
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sweeps the working-copy folder for files left behind by a
        ''' crashed session (no live ".marker"). Runs at the start of every
        ''' staging operation. Only ever needs to reason about this same
        ''' machine, since %LocalAppData% is always local.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Sub CleanUpOrphanedWorkingCopies()
            Try
                For Each strFile As String In System.IO.Directory.GetFiles(cEwEDatabase.GetWorkingCopyFolder())
                    If strFile.EndsWith(".marker", StringComparison.OrdinalIgnoreCase) Then Continue For
                    If Not Path.GetFileName(strFile).StartsWith("Temp", StringComparison.OrdinalIgnoreCase) Then Continue For

                    Dim strMarkerFile As String = strFile & ".marker"
                    Using probe As New cEwEExclusivityLock()
                        If probe.TryAcquire(strMarkerFile) Then
                            ' Marker was free - nobody's using this copy; it's an orphan from a crash.
                            probe.Release()
                            cEwEDatabase.TryDeleteFile(strFile)
                            cEwEDatabase.TryDeleteFile(strMarkerFile)
                        End If
                        ' TryAcquire returned False: a live session still holds this copy - leave it alone.
                    End Using
                Next
            Catch
                ' Folder access issue - non-fatal, just skip cleanup this pass
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Registers the AppDomain.ProcessExit cleanup hook, once, the first
        ''' time this process actually stages a working copy - a session
        ''' that never opens anything never pays any cost at exit.
        ''' ProcessExit is a base .NET runtime event, not WinForms-specific,
        ''' so this works identically for the net48 ScientificInterface app
        ''' or any other host built on EwECore.
        ''' </summary>
        ''' <remarks>
        ''' Not guaranteed to fire on a hard kill or fatal crash - that's
        ''' fine, CleanUpOrphanedWorkingCopies() (the sweep on the next
        ''' staging operation anywhere on this machine) remains the real
        ''' guarantee for those cases.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Shared Sub EnsureProcessExitHookRegistered()
            If s_processExitHookRegistered Then Return
            SyncLock s_processExitHookLock
                If s_processExitHookRegistered Then Return
                AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf cEwEDatabase.OnProcessExit
                s_processExitHookRegistered = True
            End SyncLock
        End Sub

        Private Shared Sub OnProcessExit(sender As Object, e As EventArgs)
            cEwEDatabase.ScheduleDetachedFolderSweep()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Schedules one detached script that, once this process's own PID
        ''' disappears from the OS process list, deletes every file in the
        ''' working-copy folder that has no adjacent ".marker" file. Handles
        ''' the case
        ''' where a working copy's underlying connection does not release
        ''' its file handle until the whole process exits, regardless of how
        ''' thoroughly Close() disposes things in-process.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Sub ScheduleDetachedFolderSweep()
            Try
                Dim iPid As Integer = Process.GetCurrentProcess().Id
                Dim strFolder As String = cEwEDatabase.GetWorkingCopyFolder()
                Dim psi As New ProcessStartInfo() With {
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .WindowStyle = ProcessWindowStyle.Hidden
                }

                If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
                    Dim strScriptPath As String = Path.Combine(Path.GetTempPath(), "EwECleanup_" & Guid.NewGuid().ToString("N") & ".bat")
                    Dim strScript As String =
                        "@echo off" & vbCrLf &
                        "for /L %%i in (1,1,180) do (" & vbCrLf & ' ~1/sec, so ~3 minutes cap
                        "    tasklist /FI ""PID eq " & iPid & """ 2>NUL | find """ & iPid & """ >NUL" & vbCrLf &
                        "    if errorlevel 1 (" & vbCrLf & ' PID no longer found - process has exited
                        "        for %%f in (""" & strFolder & "\*"") do (" & vbCrLf &
                        "            echo %%~xf | findstr /I /X "".marker"" >NUL" & vbCrLf &
                        "            if errorlevel 1 if not exist ""%%f.marker"" del /F /Q ""%%f""" & vbCrLf &
                        "        )" & vbCrLf &
                        "        del /F /Q ""%~f0""" & vbCrLf &
                        "        exit /b 0" & vbCrLf &
                        "    )" & vbCrLf &
                        "    ping -n 2 127.0.0.1 >nul" & vbCrLf &
                        ")" & vbCrLf &
                        "del /F /Q ""%~f0""" & vbCrLf
                    File.WriteAllText(strScriptPath, strScript)
                    psi.FileName = strScriptPath

                Else
                    Dim strScriptPath As String = Path.Combine(Path.GetTempPath(), "ewe_cleanup_" & Guid.NewGuid().ToString("N") & ".sh")
                    Dim strScript As String =
                        "#!/bin/sh" & vbLf &
                        "for i in $(seq 1 180); do" & vbLf & ' ~1/sec, so ~3 minutes cap
                        "    if ! kill -0 " & iPid & " 2>/dev/null; then" & vbLf & ' PID no longer exists - process has exited
                        "        for f in """ & strFolder & """/*; do" & vbLf &
                        "            case ""$f"" in" & vbLf &
                        "                *.marker) continue ;;" & vbLf &
                        "            esac" & vbLf &
                        "            [ -f ""$f.marker"" ] || rm -f ""$f""" & vbLf &
                        "        done" & vbLf &
                        "        rm -f ""$0""" & vbLf &
                        "        exit 0" & vbLf &
                        "    fi" & vbLf &
                        "    sleep 1" & vbLf &
                        "done" & vbLf &
                        "rm -f ""$0""" & vbLf
                    File.WriteAllText(strScriptPath, strScript)
                    Try
                        Process.Start(New ProcessStartInfo("chmod", "+x """ & strScriptPath & """") With {.UseShellExecute = False})?.WaitForExit()
                    Catch
                    End Try
                    psi.FileName = strScriptPath
                End If

                Process.Start(psi)
            Catch
                ' Best-effort only - CleanUpOrphanedWorkingCopies() is the guaranteed eventual cleanup path regardless.
            End Try
        End Sub

#End Region ' Open and close


#Region " Remaining abstract members "
        ' Reproduced exactly from the real cEwEDatabase.vb (signature-only,
        ' no body to differ) - required for this class to compile standalone.

        Public MustOverride Function MaxDBVersion() As Single

        Public MustOverride Function Compact(strFileFrom As String,
                                             strFileTo As String) As eDatasourceAccessType

        Public MustOverride Function CanCompact(strConnectionFrom As String,
                                                strConnectionTo As String) As Boolean

        Public MustOverride Function GetConnection() As IDbConnection

        Public MustOverride Function CanConnect(dst As eDataSourceTypes) As Boolean

        Public MustOverride ReadOnly Property Directory() As String

        Public MustOverride ReadOnly Property FileName() As String

        Public MustOverride ReadOnly Property Extension() As String

#End Region ' Remaining abstract members

    End Class

End Namespace
