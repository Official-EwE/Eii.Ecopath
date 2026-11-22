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
        ''' One application-level flag, set once at startup by the interactive host (EwE6ApplicationFramework.Main),
        ''' rather than a parameter on every Open() call. Other EwECore consumers keep the default: no staging.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Property EnableLocalStaging As Boolean = False

        ''' =======================================================================
        ''' <summary>
        ''' Local working-copy staging, shared by every concrete database type.
        ''' </summary>
        ''' <remarks>
        ''' <para>Cloud-sync clients (OneDrive, Dropbox) can rewrite an open file and corrupt a database mid-save. When
        ''' EnableLocalStaging is True, every Open() for writing connects to a private copy under
        ''' "%LocalAppData%\Ecopath with Ecosim\DBs\" instead of the real file. When False, Open() uses the real file.</para>
        ''' <para>Save publishes the copy back over the real file (PublishLocalCopyToSource).</para>
        ''' <para>There is no multi-session protection, by design: no lock on the real file, no conflict check.
        ''' If two sessions open the same file, the last save wins.</para>
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
            ' Keep the real extension (not ".tmp"): cDataSourceFactory.GetSupportedType detects the type from it.
            Dim strTempName As String = "Temp" & Path.GetFileNameWithoutExtension(strRealPath) & "." & Guid.NewGuid().ToString() & Path.GetExtension(strRealPath)
            Dim strTempPath As String = Path.Combine(strFolder, strTempName)

            ' No retry: unlike Save, no unsaved work is at risk on Open.
            File.Copy(strRealPath, strTempPath, True)
            Me.m_workingCopyMarker.TryAcquire(strTempPath & ".marker")
            cEwEDatabase.EnsureProcessExitHookRegistered()

            Me.m_strRealFilePath = strRealPath
            Me.m_strLocalWorkingCopyPath = strTempPath
            Return strTempPath
        End Function

        ' Only sharing/lock violations are retried (e.g. a sync client briefly holding the real file);
        ' other IO errors (disk full, path too long) fail at once.
        ''' <summary>HRESULT for ERROR_SHARING_VIOLATION (0x80070020), as the signed Integer .NET exceptions actually report.</summary>
        Private Const HR_ERROR_SHARING_VIOLATION As Integer = -2147024864
        ''' <summary>HRESULT for ERROR_LOCK_VIOLATION (0x80070021), as the signed Integer .NET exceptions actually report.</summary>
        Private Const HR_ERROR_LOCK_VIOLATION As Integer = -2147024863
        Private Const cPublishRetryAttempts As Integer = 6
        Private Const cPublishRetryDelayMs As Integer = 500

        ''' <summary>Suffix of the temporary file written next to the real file during publish.</summary>
        Private Const cPublishSidecarSuffix As String = ".publishing"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copies the working copy back over the real file, unconditionally. Called once per user Save (cCore.Save).
        ''' </summary>
        ''' <returns>True on success or when nothing was staged; False if the real file stayed locked after all
        ''' retries, or on a non-retryable IO error.</returns>
        ''' <remarks>
        ''' <para>Pending writes must be committed to the working copy first; this only copies bytes.</para>
        ''' <para>Sequence: copy to "&lt;name&gt;.publishing", delete the real file, rename the sidecar over it. A crash
        ''' before the rename never corrupts the real file; a crash between delete and rename leaves the complete new
        ''' content in the ".publishing" file for manual recovery.</para>
        ''' <para>Retries the whole sequence on a sharing violation. No UI: the caller decides on a user retry.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function PublishLocalCopyToSource() As Boolean
            If String.IsNullOrEmpty(Me.m_strLocalWorkingCopyPath) OrElse String.IsNullOrEmpty(Me.m_strRealFilePath) Then Return True

            Dim strSidecar As String = Me.m_strRealFilePath & cPublishSidecarSuffix

            For iAttempt As Integer = 1 To cPublishRetryAttempts
                Try
                    ' 1. Write the new content next to the real file; the real file stays untouched.
                    File.Copy(Me.m_strLocalWorkingCopyPath, strSidecar, True)

                    ' 2. Remove the real file (net48 File.Move has no overwrite overload). No-op on a first publish.
                    If File.Exists(Me.m_strRealFilePath) Then File.Delete(Me.m_strRealFilePath)

                    ' 3. Atomic rename on the same volume.
                    File.Move(strSidecar, Me.m_strRealFilePath)

                    Return True

                Catch ex As IOException When ex.HResult = HR_ERROR_SHARING_VIOLATION OrElse ex.HResult = HR_ERROR_LOCK_VIOLATION
                    If iAttempt = cPublishRetryAttempts Then
                        m_logger.LogWarning(ex, "cEwEDatabase.PublishLocalCopyToSource('{0}'): still locked after {1} attempts.", Me.m_strRealFilePath, cPublishRetryAttempts)
                        Return False
                    End If
                    System.Threading.Thread.Sleep(cPublishRetryDelayMs)

                Catch ex As Exception
                    ' Non-retryable: fail at once. If this fires after the delete, the sidecar may be the only copy
                    ' of the latest changes, so it is deliberately not cleaned up.
                    m_logger.LogError(ex, "cEwEDatabase.PublishLocalCopyToSource('{0}'): {1}", Me.m_strRealFilePath, ex.Message)
                    Return False
                End Try
            Next

            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases the marker and deletes the working copy. Call from Close() after the connection is closed.
        ''' </summary>
        ''' <remarks>
        ''' No delete retry: the connection's file handle can stay open until the process exits, so a retry would not
        ''' help. A leftover copy is harmless: its marker is released, so the orphan sweep or exit sweep removes it.
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
        ''' Deletes working copies left by crashed sessions (no live ".marker"). Runs before every staging operation.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Sub CleanUpOrphanedWorkingCopies()
            Try
                For Each strFile As String In System.IO.Directory.GetFiles(cEwEDatabase.GetWorkingCopyFolder())
                    If strFile.EndsWith(".marker", StringComparison.OrdinalIgnoreCase) Then Continue For
                    If Not Path.GetFileName(strFile).StartsWith("Temp", StringComparison.OrdinalIgnoreCase) Then Continue For

                    ' SQLite side files ("...ewesqlite-journal") have no marker: judge them by their base copy's marker.
                    Dim strBaseFile As String = strFile
                    For Each strSuffix As String In New String() {"-journal", "-wal", "-shm"}
                        If strFile.EndsWith(strSuffix, StringComparison.OrdinalIgnoreCase) Then
                            strBaseFile = strFile.Substring(0, strFile.Length - strSuffix.Length)
                            Exit For
                        End If
                    Next

                    Dim strMarkerFile As String = strBaseFile & ".marker"
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
        ''' Registers the ProcessExit cleanup hook, once, when this process first stages a copy.
        ''' </summary>
        ''' <remarks>
        ''' Does not fire on a hard kill; the orphan sweep on the next staging operation covers that.
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
        ''' Starts a detached script that, once this process has exited, deletes every working-copy file that has no
        ''' ".marker" (connection handles may stay open until process exit). SQLite side files are skipped: they are
        ''' cleaned by the orphan sweep, judged by their base copy's marker.
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
                        "            if errorlevel 1 echo %%f| findstr /I /R ""\-journal$ \-wal$ \-shm$"" >NUL" & vbCrLf &
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
                        "                *.marker|*-journal|*-wal|*-shm) continue ;;" & vbLf &
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
