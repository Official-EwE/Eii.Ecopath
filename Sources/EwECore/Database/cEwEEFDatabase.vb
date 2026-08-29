' SPDX-License-Identifier: EUPL-1.2
' Entity Framework implementation of cEwEDatabase for SQLite/EF
Imports System.Data
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Eii.Ecopath.Storage
Imports EwECore.DataSources
Imports EwEUtils.Utilities
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging

Namespace Database
    Public Class cEwEEFDatabase
        Inherits cEwEDatabase

        Private m_dbContext As EwEDbContext = Nothing
        Private m_strFileName As String = ""
        ''' <summary>Tracks every command created via CreateDBCommand, so Close() can force-dispose all of them regardless of whether individual callers/readers ever did.</summary>
        Private ReadOnly m_openCommands As New List(Of IDbCommand)

        ''' <summary>Write-exclusivity lock on the real, possibly-shared file.</summary>
        Private ReadOnly m_exclusivityLock As New cEwEExclusivityLock()
        ''' <summary>True if this session is read-only specifically because another session holds m_exclusivityLock.</summary>
        Private m_bLockedByAnotherUser As Boolean = False

        ''' <summary>Path of this session's disposable local snapshot, when read-only. Empty when this session is the writer.</summary>
        Private m_strLocalReadOnlyCopyPath As String = ""
        ''' <summary>Liveness marker for m_strLocalReadOnlyCopyPath - lets other sessions/instances tell a live copy from a crash-orphaned one.</summary>
        Private ReadOnly m_readOnlyCopyMarker As New cEwEExclusivityLock()

        ''' <summary>Subfolder of Path.GetTempPath() where local read-only copies and their liveness markers live.</summary>
        Private Const cReadOnlyCopySubfolder As String = "EwEReadOnlyCopies"

        ''' <summary>Guards one-time registration of the ProcessExit cleanup hook.</summary>
        Private Shared ReadOnly s_processExitHookLock As New Object()
        Private Shared s_processExitHookRegistered As Boolean = False

        ' Private helper to ensure DbContext is initialized and migrated
        Private Sub EnsureDbContext(strDatabase As String)
            EwEDbContext.DefaultSQLiteFilePath = strDatabase
        #If NET48 Then
            ' net48 cannot run EF Core in-process (Eii.Ecopath.Storage's EwEDbContext
            ' is net10.0-only there) - shell out to a self-contained net10.0 tool to
            ' apply any pending migrations BEFORE this process's own EF6 context
            ' ever touches the file.
            cSqliteMigrator.MigrateDatabase(strDatabase)
        #End If
            If m_dbContext Is Nothing Then
                m_dbContext = New EwEDbContext()
            End If
        #If NET48 Then
            m_dbContext.Database.Initialize(True)
        #Else
            m_dbContext.Database.Migrate()
        #End If
        End Sub

        Public Function GetDbContext() As EwEDbContext
            Return m_dbContext
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' True if this instance is open read-only specifically because
        ''' another Ecopath session already holds the exclusivity lock on
        ''' this file - as opposed to being read-only for some other reason
        ''' (explicit caller request, read-only file attribute, etc). Flows
        ''' up through IEwEDataSource.IsLockedByAnotherSession() exactly like
        ''' IsReadOnly() already does, so cCore can surface a message via its
        ''' own Messages pipeline (cEwEEFDatabase has no reference to cCore
        ''' itself, so it cannot send that message directly).
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property IsLockedByAnotherSession As Boolean
            Get
                Return Me.m_bLockedByAnotherUser
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Path of this session's local, disposable read-only copy, if it
        ''' has one (see Open()) - empty when this session holds the write
        ''' lock instead.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property LocalReadOnlyCopyPath As String
            Get
                Return Me.m_strLocalReadOnlyCopyPath
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new SQLite database, seeded from an embedded starter
        ''' resource (mirrors cEwEAccessDatabase.Create - EnsureDbContext's
        ''' Migrate()/Initialize(True) call needs the file to already exist;
        ''' it does not create a schema from nothing).
        ''' </summary>
        ''' <param name="strDatabase">The file name of the .ewesqlite to create.</param>
        ''' <param name="strAuthor">Name of the author to assign.</param>
        ''' <param name="strModelName">Name of the model to use.</param>
        ''' <param name="bOverwrite">States whether an existing database may be overwritten.</param>
        ''' <param name="format">Database format type to use. If not set, the
        ''' database type is deducted from the <paramref name="strDatabase">database</paramref>.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">eDatasourceAccessType</see> value</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Create(strDatabase As String,
                strModelName As String,
                Optional bOverwrite As Boolean = False,
                Optional format As eDataSourceTypes = eDataSourceTypes.NotSet,
                Optional strAuthor As String = "") As eDatasourceAccessType

            Dim strSource As String = ""
            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Success

            If format = eDataSourceTypes.NotSet Then
                format = cDataSourceFactory.GetSupportedType(strDatabase)
            End If

            Select Case format
                Case eDataSourceTypes.Sqlite
                    strSource = "EwE6.sqlite"
                Case Else
                    datResult = eDatasourceAccessType.Failed_UnknownType
                    m_logger.LogInformation("Create DB: cannot determine format")
            End Select

            If (datResult = eDatasourceAccessType.Success) Then

                ' Copy the starter resource to strDatabase FIRST - EnsureDbContext's
                ' Migrate()/Initialize(True) call (via cSqliteMigrator on net48)
                ' needs the file to already physically exist; it brings an
                ' existing file up to date, it does not create one from nothing.
                If cResourceUtils.SaveResourceToFile(strSource, strDatabase, bOverwrite, Assembly.GetExecutingAssembly()) Then
                    Try
                        EnsureDbContext(strDatabase)
                        m_strFileName = strDatabase

                        Me.Execute("UPDATE EcopathModel SET Name = ?, Author = ? WHERE ModelID = 1",
                                   New Object() {strModelName, strAuthor})
                        Try
                            ' Egg - over-easy but slightly obfuscated ;)
                            If strModelName.ToLower().Contains(cStringUtils.Shift("Dbsm!Xbmufst").ToLower()) Then
                                Me.Execute("UPDATE EcopathGroup SET GroupName = ? WHERE GroupID = 1",
                                           New Object() {cStringUtils.Shift("Dijdlfo!tiju")})
                                Me.Execute("UPDATE EcopathFleet SET FleetName = ? WHERE FleetID = 1",
                                           New Object() {cStringUtils.Shift("Tfbm!cbtifst")})
                            End If
                        Catch ex As Exception
                            ' Do not let eggs make the pot explode
                            m_logger.LogError(ex, "Create DB: found a rotten egg: " & ex.Message)
                        End Try

                        Me.Close()
                        datResult = eDatasourceAccessType.Success

                    Catch ex As Exception
                        m_logger.LogError(ex, "Create DB: Exception when updating model name: " & ex.Message)
                        datResult = eDatasourceAccessType.Failed_Unknown
                    End Try
                Else
                    ' Unable to write to target location
                    m_logger.LogError("Create DB: Unable to save to target location " & strDatabase)
                    datResult = eDatasourceAccessType.Failed_CannotSave
                End If
            End If
            Return datResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the current database to a new destination, and continue
        ''' operating on that new destination.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function SaveAs(strDatabaseTo As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType
            Try
                If File.Exists(strDatabaseTo) AndAlso Not bOverwrite Then
                    Return eDatasourceAccessType.Failed_CannotSave
                End If
                File.Copy(m_strFileName, strDatabaseTo, True)

                ' Move exclusivity to the new file - SaveAs re-points this
                ' instance without going through Open(), so this has to be
                ' handled here too.
                Dim bWasExclusive As Boolean = Me.m_exclusivityLock.IsHeld
                Me.m_exclusivityLock.Release()
                If bWasExclusive AndAlso Not Me.m_exclusivityLock.TryAcquire(strDatabaseTo & ".lock") Then
                    ' Vanishingly unlikely immediately after our own File.Copy,
                    ' but degrade honestly rather than silently keeping stale
                    ' exclusivity we can no longer actually back up.
                    Me.IsReadOnly = True
                    Me.m_bLockedByAnotherUser = True
                End If

                EnsureDbContext(strDatabaseTo)
                m_strFileName = strDatabaseTo
                Return eDatasourceAccessType.Success
            Catch ex As Exception
                Return eDatasourceAccessType.Failed_Unknown
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a SQLite database. If another EwECore-based
        ''' session already holds the exclusivity lock on this file, opens
        ''' read-only instead of failing - see FILE_LOCK_HANDOFF.md.
        ''' </summary>
        ''' <remarks>
        ''' A read-only open never touches the real file with a live database
        ''' connection: it works off a disposable local snapshot in the OS
        ''' temp folder instead. This lets Migrate() run normally against the
        ''' snapshot (a genuinely read-only SQLite connection cannot apply
        ''' migrations), guarantees a read-only session always sees the
        ''' current schema rather than breaking on an older un-migrated one,
        ''' and makes "never write to the real file" true by construction -
        ''' independent of whether every write path in the app or its plugins
        ''' correctly checks IsReadOnly first.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Open(strDatabase As String, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet, Optional bReadOnly As Boolean = False) As eDatasourceAccessType
            Try
                If Not File.Exists(strDatabase) Then
                    Return eDatasourceAccessType.Failed_FileNotFound
                End If

                Me.m_bLockedByAnotherUser = False
                Dim strEffectivePath As String = strDatabase

                If Not bReadOnly Then
                    If Me.m_exclusivityLock.TryAcquire(strDatabase & ".lock") Then
                        bReadOnly = False
                    Else
                        ' Someone else already holds it - degrade to read-only
                        ' and still open successfully. This deliberately does
                        ' NOT return Failed_AlreadyInUse: that value is fatal
                        ' to the whole LoadModel() call further up the stack
                        ' (mirroring cEwEAccessDatabase's Access-specific
                        ' behavior), which is not what we want here.
                        bReadOnly = True
                        Me.m_bLockedByAnotherUser = True
                    End If
                End If

                If bReadOnly Then
                    cEwEEFDatabase.CleanUpOrphanedReadOnlyCopies()

                    Dim strFolder As String = cEwEEFDatabase.GetReadOnlyCopyFolder()
                    Me.m_strLocalReadOnlyCopyPath = Path.Combine(strFolder, Guid.NewGuid().ToString("N") & Path.GetExtension(strDatabase))

                    ' Claim the liveness marker BEFORE creating the copy - the
                    ' marker path is freshly GUID-named, so this always
                    ' succeeds trivially. Doing it in this order guarantees a
                    ' concurrent sweep on another session (CleanUpOrphanedReadOnlyCopies)
                    ' can never observe our copy file without its marker, which
                    ' would otherwise get it deleted out from under us as a
                    ' false-positive "orphan" mid-open.
                    Me.m_readOnlyCopyMarker.TryAcquire(Me.m_strLocalReadOnlyCopyPath & ".marker")
                    File.Copy(strDatabase, Me.m_strLocalReadOnlyCopyPath, True)
                    cEwEEFDatabase.EnsureProcessExitHookRegistered()

                    strEffectivePath = Me.m_strLocalReadOnlyCopyPath
                End If

                EnsureDbContext(strEffectivePath) ' Migrate()/Initialize(True) always run the same way, read-write, regardless of bReadOnly
                m_strFileName = strDatabase        ' Name/Directory/FileName/Extension keep reporting the REAL path
                Me.IsReadOnly = bReadOnly           ' base property - what CanSave()/UpdateModelControls() actually check
                GetOpenConnection()

                Return eDatasourceAccessType.Opened

            Catch ex As Exception
                Me.m_exclusivityLock.Release()
                Me.CleanUpLocalReadOnlyCopy()
                m_logger.LogError(ex, "cEwEEFDatabase.Open('{0}') failed: {1}", strDatabase, ex.Message)
                Return eDatasourceAccessType.Failed_Unknown
            End Try
        End Function

        Public Overrides Sub Close()
            Dim conn As IDbConnection = GetConnection()
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            ' Force-dispose every command this instance ever created via
            ' CreateDBCommand, regardless of whether the reader/caller that
            ' used it was ever properly disposed. Must happen BEFORE the
            ' context/connection is disposed below - see FILE_LOCK_HANDOFF.md
            ' for why this, not caller-side reader disposal alone, may be
            ' the actual cause of the connection never fully releasing.
            SyncLock Me.m_openCommands
                For Each cmd As IDbCommand In Me.m_openCommands
                    Try
                        cmd.Dispose()
                    Catch
                    End Try
                Next
                Me.m_openCommands.Clear()
            End SyncLock

            If m_dbContext IsNot Nothing Then
                m_dbContext.Dispose()
                m_dbContext = Nothing
            End If
            Me.m_exclusivityLock.Release()
            Me.m_bLockedByAnotherUser = False
            Me.CleanUpLocalReadOnlyCopy()
            m_strFileName = ""
        End Sub

#Region " Local read-only copy management "

        ''' <summary>Full path of the (auto-created) folder holding local read-only copies and their liveness markers.</summary>
        Private Shared Function GetReadOnlyCopyFolder() As String
            Dim strFolder As String = Path.Combine(Path.GetTempPath(), cReadOnlyCopySubfolder)
            System.IO.Directory.CreateDirectory(strFolder) ' no-op if it already exists - qualified: this class also declares a "Directory" property
            Return strFolder
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sweeps the local read-only-copy folder for copies left behind by
        ''' a crashed session. A copy is orphaned if nobody currently holds
        ''' its ".marker" lock - live sessions (this or any other Ecopath
        ''' instance on this machine) keep theirs held via FileShare.None for
        ''' as long as they're using the copy, exactly like the write-
        ''' exclusivity lock. Since Path.GetTempPath() is always local to the
        ''' machine, this only ever needs to reason about instances on the
        ''' same machine, never across the network.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Sub CleanUpOrphanedReadOnlyCopies()
            Try
                For Each strCopyFile As String In System.IO.Directory.GetFiles(cEwEEFDatabase.GetReadOnlyCopyFolder())
                    If strCopyFile.EndsWith(".marker", StringComparison.OrdinalIgnoreCase) Then Continue For

                    Dim strMarkerFile As String = strCopyFile & ".marker"
                    Using probe As New cEwEExclusivityLock()
                        If probe.TryAcquire(strMarkerFile) Then
                            ' Marker was free - nobody's using this copy; it's an orphan from a crash.
                            probe.Release()
                            cEwEEFDatabase.TryDeleteFile(strCopyFile)
                            cEwEEFDatabase.TryDeleteFile(strMarkerFile)
                        End If
                        ' TryAcquire returned False: a live session still holds
                        ' this copy open elsewhere on this machine - leave both files alone.
                    End Using
                Next
            Catch
                ' Folder access issue - non-fatal, just skip cleanup this pass
            End Try
        End Sub

        Private Shared Sub TryDeleteFile(strPath As String)
            Try
                If File.Exists(strPath) Then File.Delete(strPath)
            Catch
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' A single, immediate delete attempt - no retry loop. Confirmed
        ''' (see FILE_LOCK_HANDOFF.md) that the underlying connection stays
        ''' open for this app's entire process lifetime, not just briefly -
        ''' so a sleep-based retry here can never succeed and only adds
        ''' blocking delay for no benefit. Kept as a single cheap check in
        ''' case a future fix (the root leak, once found) makes this start
        ''' succeeding again naturally.
        ''' </summary>
        ''' <returns>True if the file no longer exists when this returns.</returns>
        ''' -------------------------------------------------------------------
        Private Shared Function TryDeleteFileOnce(strPath As String) As Boolean
            Try
                If Not File.Exists(strPath) Then Return True
                File.Delete(strPath)
                Return True
            Catch
                Return False
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Schedules one detached script that, once this process's own PID
        ''' disappears from the OS process list, deletes every file in the
        ''' read-only copy folder that has no adjacent ".marker" file.
        ''' Called exactly once per app session, from OnProcessExit.
        ''' </summary>
        ''' <remarks>
        ''' No specific paths are passed in deliberately: a file whose
        ''' marker has already been released (by CleanUpLocalReadOnlyCopy)
        ''' is safe to delete regardless of which session created it, so a
        ''' generic sweep needs no bookkeeping of what to look for. This is
        ''' a bare existence check, not the true FileShare.None liveness
        ''' test CleanUpOrphanedReadOnlyCopies() does - deliberately: a
        ''' genuinely crashed session's marker BYTES can remain on disk even
        ''' though nobody holds it, which this script would treat as "still
        ''' in use" and skip. That's fine - CleanUpOrphanedReadOnlyCopies(),
        ''' unaffected by this, still catches that case properly the next
        ''' time any session on this machine opens something read-only.
        ''' This script only needs to handle the common case (this app's
        ''' own normally-closed sessions) quickly.
        ''' </remarks>
        ''' <remarks>
        ''' Uses a short wait cap (a few minutes), not hours: this only ever
        ''' runs from OnProcessExit, at which point the process is already
        ''' in the process of shutting down, not at an arbitrary earlier
        ''' point in a session that could still run for hours.
        ''' </remarks>
        ''' <remarks>
        ''' A script FILE is used instead of an inline "cmd.exe /C ..."
        ''' one-liner specifically to avoid the nested-quoting fragility
        ''' that caused a real bug elsewhere in this project's tooling (see
        ''' SESSION_HANDOFF.md's PowerShell Invoke-Expression parsing
        ''' issue).
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Shared Sub ScheduleDetachedFolderSweep()
            Try
                Dim iPid As Integer = Process.GetCurrentProcess().Id
                Dim strFolder As String = cEwEEFDatabase.GetReadOnlyCopyFolder()
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
                ' Best-effort only - CleanUpOrphanedReadOnlyCopies() is the guaranteed eventual cleanup path regardless.
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases this session's copy marker (fast, synchronous - also
        ''' deletes the ".marker" file itself, see cEwEExclusivityLock.Release)
        ''' and tries once to delete the local read-only copy, if any.
        ''' </summary>
        ''' <remarks>
        ''' No retry, no queueing: once the marker is released, this file is
        ''' indistinguishable from any other marker-less file in the copy
        ''' folder, so ScheduleDetachedFolderSweep (run once at process
        ''' exit) picks it up generically - there is nothing further for
        ''' this method to track. Never blocks the caller - the one delete
        ''' attempt is instant, so this is safe even during app shutdown.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub CleanUpLocalReadOnlyCopy()
            Me.m_readOnlyCopyMarker.Release()
            If String.IsNullOrEmpty(Me.m_strLocalReadOnlyCopyPath) Then Return
            cEwEEFDatabase.TryDeleteFileOnce(Me.m_strLocalReadOnlyCopyPath)
            Me.m_strLocalReadOnlyCopyPath = ""
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Registers the AppDomain.ProcessExit cleanup hook, once, the first
        ''' time this process actually creates a local read-only copy -
        ''' sessions that never open anything read-only never pay any cost
        ''' at exit. ProcessExit is a base .NET runtime event, not a
        ''' WinForms-specific one - it fires identically regardless of which
        ''' host app (net48 WinForms, net10.0 console, etc.) is using
        ''' EwECore, so this works entirely from within the library with no
        ''' cooperation needed from the consuming app.
        ''' </summary>
        ''' <remarks>
        ''' Not guaranteed to fire on a hard kill or fatal crash - that's
        ''' fine, CleanUpOrphanedReadOnlyCopies() (the sweep on the next
        ''' read-only Open() anywhere on this machine) remains the real
        ''' guarantee for those cases, completely unaffected by this.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Shared Sub EnsureProcessExitHookRegistered()
            If s_processExitHookRegistered Then Return
            SyncLock s_processExitHookLock
                If s_processExitHookRegistered Then Return
                AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf cEwEEFDatabase.OnProcessExit
                s_processExitHookRegistered = True
            End SyncLock
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fires once, at graceful process shutdown. Schedules exactly one
        ''' detached script that, once this PID disappears, deletes every
        ''' marker-less file in the read-only copy folder - not just this
        ''' session's, but anything left behind over the app's whole
        ''' lifetime. No path tracking needed: a file with its marker
        ''' already released (by CleanUpLocalReadOnlyCopy, above) is
        ''' indistinguishable from any other abandoned copy, so a generic
        ''' folder sweep finds it without being told where to look.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Sub OnProcessExit(sender As Object, e As EventArgs)
            cEwEEFDatabase.ScheduleDetachedFolderSweep()
        End Sub

#End Region ' Local read-only copy management

        Public Overrides ReadOnly Property Name As String
            Get
                Return m_strFileName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains and configures a <see cref="System.Data.IDataAdapter"/> for
        ''' the current SQLite database via the underlying DbConnection.
        ''' </summary>
        ''' <param name="strSQL">The SQL query to obtain the adapter for.</param>
        ''' <returns>A <see cref="System.Data.IDataAdapter"/> if successful,
        ''' or Nothing when an error occurred.</returns>
        ''' <remarks>
        ''' <para>The returned adapter is initialized with default insert, update
        ''' and delete commands based on the provided query.</para>
        ''' <para>The obtained adapter should be released via
        ''' <see cref="ReleaseAdapter">ReleaseAdapter</see>.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetAdapter(strSQL As String) As IDataAdapter
            If m_dbContext Is Nothing Then Return Nothing

            Try
                Dim conn As IDbConnection = GetOpenConnection()
                If conn Is Nothing Then Return Nothing

                Dim dbConn As System.Data.Common.DbConnection = TryCast(conn, System.Data.Common.DbConnection)
                If dbConn Is Nothing Then Return Nothing

                Dim factory As System.Data.Common.DbProviderFactory = System.Data.Common.DbProviderFactories.GetFactory(dbConn)
                If factory Is Nothing Then
                    Throw New NotSupportedException("No DbProviderFactory found for connection type: " & dbConn.GetType().Name)
                End If

                Dim adapter As System.Data.Common.DbDataAdapter = TryCast(factory.CreateDataAdapter(), System.Data.Common.DbDataAdapter)
                If adapter Is Nothing Then
                    Throw New NotSupportedException("IDataAdapter is not supported for provider: " & factory.GetType().Name & ". Use GetDbContext() to access entities directly via Entity Framework.")
                End If

                Dim cmd As System.Data.Common.DbCommand = dbConn.CreateCommand()
                cmd.CommandText = strSQL
                adapter.SelectCommand = cmd
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey

                Dim cmdBuilder As System.Data.Common.DbCommandBuilder = factory.CreateCommandBuilder()
                cmdBuilder.DataAdapter = adapter
                adapter.InsertCommand = DirectCast(cmdBuilder.GetInsertCommand(True), System.Data.Common.DbCommand)
                adapter.UpdateCommand = DirectCast(cmdBuilder.GetUpdateCommand(True), System.Data.Common.DbCommand)
                adapter.DeleteCommand = DirectCast(cmdBuilder.GetDeleteCommand(True), System.Data.Common.DbCommand)

                Return adapter
            Catch ex As NotSupportedException
                Throw
            Catch ex As InvalidOperationException
                m_logger.LogError(ex, cStringUtils.Localize("Table in query '{0}' seems to be missing a primary key: {1}", strSQL, ex.Message))
                Return Nothing
            Catch ex As Exception
                m_logger.LogError(ex, cStringUtils.Localize("Error when opening adapter for query '{0}': {1}", strSQL, ex.Message))
                Return Nothing
            End Try
        End Function

        Public Overrides Function GetConnection() As IDbConnection
            If m_dbContext IsNot Nothing Then
#If NET48 Then
                Return m_dbContext.Database.Connection
#Else
                Return m_dbContext.Database.GetDbConnection()
#End If
            End If
            Return Nothing
        End Function

        Private Function GetOpenConnection() As IDbConnection
            Dim conn As IDbConnection = GetConnection()
            If conn Is Nothing Then Return Nothing
            If conn.State <> ConnectionState.Open Then
                conn.Open()
                ' Run PRAGMAs once on first open
                Using pragmasCmd As IDbCommand = conn.CreateCommand()
                    pragmasCmd.CommandText = "PRAGMA journal_mode=DELETE; PRAGMA synchronous=FULL; PRAGMA cache_size=-64000; PRAGMA temp_store=MEMORY;"
                    pragmasCmd.ExecuteNonQuery()
                End Using
            End If
            Return conn
        End Function

        Public Overrides Function CanConnect(dst As eDataSourceTypes) As Boolean
            Return dst = eDataSourceTypes.Sqlite
        End Function

        Public Overrides Function CanCompact(strConnectionFrom As String, strConnectionTo As String) As Boolean
            Return False ' Not supported for EF/SQLite
        End Function

        Public Overrides Function Compact(strFileFrom As String, strFileTo As String) As eDatasourceAccessType
            Return eDatasourceAccessType.Failed_DeprecatedOperation
        End Function

        Public Overrides Function MaxDBVersion() As Single
            Return cDatabaseUpdater.MaxSupportedVersion
        End Function

        Public Overrides ReadOnly Property Directory As String
            Get
                Return Path.GetDirectoryName(m_strFileName)
            End Get
        End Property

        Public Overrides ReadOnly Property FileName As String
            Get
                Return Path.GetFileNameWithoutExtension(m_strFileName)
            End Get
        End Property

        Public Overrides ReadOnly Property Extension As String
            Get
                Return Path.GetExtension(m_strFileName)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The legacy cDBUpdate update chain (see cDatabaseUpdater.RunAllUpdates)
        ''' was written for, and several of its historical updates rely on,
        ''' Access/OleDb-specific SQL that SQLite does not support at all
        ''' (ALTER COLUMN, ADD/DROP CONSTRAINT, ADD PRIMARY KEY/FOREIGN KEY via
        ''' ALTER TABLE). This database's schema is versioned separately, via
        ''' EF Core migrations, so the legacy chain should never run against it.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportsLegacyDatabaseUpdates() As Boolean
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Not supported - see <see cref="SupportsLegacyDatabaseUpdates"/>.
        ''' Throws rather than silently attempting SQL that SQLite cannot run,
        ''' in case this is ever reached by a path other than the (guarded)
        ''' legacy update chain.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function DropColumn(strTable As String, strColumn As String) As Boolean
            Throw New NotSupportedException(
                "cEwEEFDatabase.DropColumn() is not supported - SQLite schema changes go through EF Core migrations instead. See SupportsLegacyDatabaseUpdates().")
        End Function

        ' Additional EF-specific methods can be added here as needed
        Protected Overrides Function CreateDBCommand(strSQL As String) As IDbCommand
            ' For EF, create a command from the underlying DbConnection
            If m_dbContext Is Nothing Then Return Nothing
            Dim conn As IDbConnection = GetOpenConnection()
            If conn Is Nothing Then Return Nothing
            Dim cmd As IDbCommand = conn.CreateCommand()
            cmd.CommandText = strSQL
            SyncLock Me.m_openCommands
                Me.m_openCommands.Add(cmd)
            End SyncLock
            Return cmd
        End Function

        Protected Overrides Function HasTable(strTableName As String) As Boolean
            If m_dbContext Is Nothing Then Return False
            Return m_dbContext.GetEntityTypeByTableName(strTableName) IsNot Nothing
        End Function

        Public Overrides Function GetReader(strSQL As String) As IDataReader
            Dim command As IDbCommand = Nothing
            Dim reader As cCoercedDataReader = Nothing
            Try
                command = Me.CreateDBCommand(strSQL)
                reader = New cCoercedDataReader(command.ExecuteReader())
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("GetReader error: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "cEwEEFDatabase.GetReader(" & strSQL & ")")
                command?.Dispose()
                reader = Nothing
            End Try
            If reader IsNot Nothing Then
                reader.PropTypes = GetDbContext().GetPropTypes(DataReaderDiff.GetTableName(reader))
            End If
            Return reader
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns an EF-backed writer for the given table. Overridden because
        ''' the base implementation constructs a cEwEDbWriter, which relies on
        ''' GetAdapter() - and Microsoft.Data.Sqlite has no DbDataAdapter to give it.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetWriter(strTable As String) As IEwEDbWriter
            Dim writer As IEwEDbWriter = New cEwEEFDbWriter(Me.m_dbContext, strTable, Me.m_logger)
            writer.RefCount += 1
            Return writer
        End Function

    End Class
End Namespace
