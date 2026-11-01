' SPDX-License-Identifier: EUPL-1.2
'
' Tests the local-staging-copy mechanism directly, via TestDatabase's public
' wrappers around cEwEDatabase's Protected staging members. No real Access/
' SQLite connection is involved anywhere - staging is pure file I/O, and
' these tests exercise exactly that.
'
' NOTE: GetWorkingCopyFolder() is hardcoded (by design, in the real class)
' to %LocalAppData%\Ecopath with Ecosim\DBs - it cannot be redirected for
' isolation. Tests use uniquely-named (GUID) source files to avoid
' colliding with each other, and clean up after themselves, but they do
' touch that real folder on whatever machine runs them - the same folder
' the real application itself uses.

Imports System.IO
Imports EwECore.Database
Imports Xunit

Public Class StagingTests
    Implements IDisposable

    Private ReadOnly m_strSourcePath As String
    Private ReadOnly m_db As New TestDatabase()

    Public Sub New()
        ' Unique source file per test instance (xUnit creates a new test
        ' class instance per test method by default), in the system temp
        ' folder - never the staging folder itself.
        Me.m_strSourcePath = Path.Combine(Path.GetTempPath(), "StagingTestSource_" & Guid.NewGuid().ToString("N") & ".eweaccdb")
        File.WriteAllText(Me.m_strSourcePath, "original content")
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Me.m_db.Test_CleanUpLocalWorkingCopy()
        Try
            If File.Exists(Me.m_strSourcePath) Then File.Delete(Me.m_strSourcePath)
        Catch
        End Try
    End Sub

    <Fact>
    Public Sub StageLocalWorkingCopy_CreatesFileWithRealExtension_NotDotTmp()
        ' Regression test for a real bug: the staged copy originally always
        ' ended in ".tmp", which made cDataSourceFactory.GetSupportedType
        ' (which determines file type purely from Path.GetExtension)
        ' unable to recognize the staged copy at all when it was opened
        ' directly by path - breaking the Access migration flow silently.
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)

        Assert.True(File.Exists(strStagedPath))
        Assert.Equal(".eweaccdb", Path.GetExtension(strStagedPath))
        Assert.NotEqual(".tmp", Path.GetExtension(strStagedPath))
    End Sub

    <Fact>
    Public Sub StageLocalWorkingCopy_NameStartsWithTemp_AndContainsGuid()
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)
        Dim strFileName As String = Path.GetFileName(strStagedPath)

        Assert.StartsWith("Temp", strFileName)
        ' A GUID is 32 hex chars with 4 hyphens in its default ToString() format
        Assert.Matches("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}", strFileName)
    End Sub

    <Fact>
    Public Sub StageLocalWorkingCopy_OnlyUsesFileName_NotOriginalDirectory()
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)

        ' The staging folder must be the fixed %LocalAppData%\Ecopath with
        ' Ecosim\DBs location, never anything derived from the source
        ' file's own directory.
        Assert.Contains(Path.Combine("Ecopath with Ecosim", "DBs"), strStagedPath)
        Assert.DoesNotContain(Path.GetDirectoryName(Me.m_strSourcePath), Path.GetDirectoryName(strStagedPath))
    End Sub

    <Fact>
    Public Sub StageLocalWorkingCopy_CopiesRealContent()
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)

        Assert.Equal("original content", File.ReadAllText(strStagedPath))
    End Sub

    <Fact>
    Public Sub StageLocalWorkingCopy_CreatesMarkerFile()
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)

        Assert.True(File.Exists(strStagedPath & ".marker"))
    End Sub

    <Fact>
    Public Sub CleanUpLocalWorkingCopy_DeletesCopyAndMarker_AndResetsTrackedPaths()
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)
        Dim strMarkerPath As String = strStagedPath & ".marker"

        Me.m_db.Test_CleanUpLocalWorkingCopy()

        Assert.False(File.Exists(strStagedPath))
        Assert.False(File.Exists(strMarkerPath))
        Assert.Equal("", Me.m_db.Test_LocalWorkingCopyPath)
        Assert.Equal("", Me.m_db.Test_RealFilePath)
    End Sub

    <Fact>
    Public Sub CleanUpLocalWorkingCopy_IsSafeNoOp_WhenNothingWasStaged()
        ' Should not throw - e.g. a genuine read-only open (old DB version
        ' case) never stages anything at all, but Close() still
        ' unconditionally calls this.
        Dim ex As Exception = Record.Exception(Sub() Me.m_db.Test_CleanUpLocalWorkingCopy())
        Assert.Null(ex)
    End Sub

    <Fact>
    Public Sub PublishLocalCopyToSource_IsNoOp_ReturnsTrue_WhenNothingWasStaged()
        Dim bResult As Boolean = Me.m_db.PublishLocalCopyToSource()
        Assert.True(bResult)
    End Sub

    <Fact>
    Public Sub PublishLocalCopyToSource_CopiesCurrentWorkingCopyBytes_BackToOriginal()
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)

        ' Simulate an edit: change the working copy, not the original.
        File.WriteAllText(strStagedPath, "edited content")

        Dim bResult As Boolean = Me.m_db.PublishLocalCopyToSource()

        Assert.True(bResult)
        Assert.Equal("edited content", File.ReadAllText(Me.m_strSourcePath))
    End Sub

    <Fact>
    Public Sub CleanUpOrphanedWorkingCopies_RemovesMarkerlessLeftover_OnNextStage()
        ' Simulates a crashed session: a "Temp*" file sitting in the
        ' staging folder with NO marker (as if the process died before
        ' Close() ever ran). The sweep runs automatically as the first
        ' step of StageLocalWorkingCopy - triggered here by an unrelated
        ' second staging operation, exactly as happens in production when
        ' ANY database is opened elsewhere on the same machine.
        Dim strOrphanPath As String = Path.Combine(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ecopath with Ecosim", "DBs"),
            "TempOrphanTest." & Guid.NewGuid().ToString() & ".eweaccdb")
        File.WriteAllText(strOrphanPath, "leftover from a crash")

        Try
            Dim strUnrelatedSource As String = Path.Combine(Path.GetTempPath(), "StagingTestSource_" & Guid.NewGuid().ToString("N") & ".eweaccdb")
            File.WriteAllText(strUnrelatedSource, "unrelated")
            Try
                Me.m_db.Test_StageLocalWorkingCopy(strUnrelatedSource)
                Assert.False(File.Exists(strOrphanPath))
            Finally
                File.Delete(strUnrelatedSource)
            End Try
        Finally
            Try
                If File.Exists(strOrphanPath) Then File.Delete(strOrphanPath)
            Catch
            End Try
        End Try
    End Sub

    <Fact>
    Public Sub CleanUpOrphanedWorkingCopies_LeavesMarkerHeldFile_Alone()
        ' A "Temp*" file whose marker IS still held (simulating a session
        ' that's genuinely still open elsewhere on this machine) must
        ' survive the sweep.
        Dim strFolder As String = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ecopath with Ecosim", "DBs")
        Dim strStillInUsePath As String = Path.Combine(strFolder, "TempStillInUse." & Guid.NewGuid().ToString() & ".eweaccdb")
        File.WriteAllText(strStillInUsePath, "still being used")
        Dim markerStream As FileStream = New FileStream(strStillInUsePath & ".marker", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)

        Try
            Dim strUnrelatedSource As String = Path.Combine(Path.GetTempPath(), "StagingTestSource_" & Guid.NewGuid().ToString("N") & ".eweaccdb")
            File.WriteAllText(strUnrelatedSource, "unrelated")
            Try
                Me.m_db.Test_StageLocalWorkingCopy(strUnrelatedSource)
                Assert.True(File.Exists(strStillInUsePath))
            Finally
                File.Delete(strUnrelatedSource)
            End Try
        Finally
            markerStream.Dispose()
            Try
                If File.Exists(strStillInUsePath) Then File.Delete(strStillInUsePath)
                If File.Exists(strStillInUsePath & ".marker") Then File.Delete(strStillInUsePath & ".marker")
            Catch
            End Try
        End Try
    End Sub

    <Fact(Skip:="Windows-specific: depends on FileShare.None sharing-violation semantics and the exact Windows HResult (-2147024864) PublishLocalCopyToSource checks for. Advisory locking on Linux/macOS will not reproduce this reliably. Remove the Skip to run on an actual Windows machine.")>
    Public Sub PublishLocalCopyToSource_RetriesOnSharingViolation_ThenReportsFailure()
        Dim strStagedPath As String = Me.m_db.Test_StageLocalWorkingCopy(Me.m_strSourcePath)
        File.WriteAllText(strStagedPath, "edited content")

        Using lockStream As New FileStream(Me.m_strSourcePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            Dim bResult As Boolean = Me.m_db.PublishLocalCopyToSource()
            Assert.False(bResult)
        End Using

        ' Lock released - a subsequent publish should now succeed.
        Dim bResultAfterRelease As Boolean = Me.m_db.PublishLocalCopyToSource()
        Assert.True(bResultAfterRelease)
    End Sub

End Class
