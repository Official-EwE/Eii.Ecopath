// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSEBatchManager;
using EwECore.MSECommandFile;
using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Integration tests for <c>cMSEBatchManager</c>: loads a minimal command file
/// and verifies that a batch run completes without errors.
/// </summary>
[Collection("Model")]
public sealed class MSEBatchManagerTests : IDisposable
{
    private readonly ModelFixture _fixture;
    private readonly string _tempDir;

    public MSEBatchManagerTests(ModelFixture fixture)
    {
        _fixture = fixture;
        _tempDir = Path.Combine(Path.GetTempPath(), $"EwEBatch.Tests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private string WriteMinimalCommandFile(int nGroups)
    {
        // Build a simple TFM command file with one iteration.
        // All TFM values are set to 0.5 for every group so the run is valid.
        var tfmValues = string.Join(",", Enumerable.Repeat("0.5", nGroups));

        var lines = new List<string>
        {
            $"{cMSECommandFileReader.VERSION_DATA_TAG}, 1.0",
            $"{cMSECommandFileReader.RUNTYPE_DATA_TAG}, 3",          // 3 = TFM
            $"{cMSECommandFileReader.NSIMS_DATA_TAG}, 1",
            $"{cMSECommandFileReader.CV_DATA_TAG}, 0.0",
            $"{cMSECommandFileReader.STARTYEAR_DATA_TAG}, 1",
            $"{cMSECommandFileReader.ENDYEAR_DATA_TAG}, 5",
            $"{cMSECommandFileReader.OUTPUT_DATA_TAG}, {_tempDir}",
            // TFM_INDEX row lists column headers (ignored by reader)
            $"{cMSECommandFileReader.TFM_INDEX_TAG}, " + string.Join(",", Enumerable.Range(1, nGroups)),
            $"{cMSECommandFileReader.TFM_DATA_TAG}, {tfmValues}"
        };

        string path = Path.Combine(_tempDir, "batch_command.csv");
        File.WriteAllLines(path, lines);
        return path;
    }

    // -----------------------------------------------------------------------
    // ReadCommandFile
    // -----------------------------------------------------------------------

    [Fact]
    public void ReadCommandFile_WithMinimalTFMFile_ReturnsTrue()
    {
        int nGroups = _fixture.Core.nGroups;
        string file = WriteMinimalCommandFile(nGroups);

        bool result = _fixture.Core.MSEBatchManager.ReadCommandFile(file);
        result.Should().BeTrue();
    }

    [Fact]
    public void ReadCommandFile_SetsIsInit_ToTrue_OnSuccess()
    {
        int nGroups = _fixture.Core.nGroups;
        string file = WriteMinimalCommandFile(nGroups);

        _fixture.Core.MSEBatchManager.ReadCommandFile(file);
        _fixture.Core.MSEBatchManager.BatchData.isInit.Should().BeTrue();
    }

    [Fact]
    public void ReadCommandFile_SetsRunType_ToTFM_FromMinimalFile()
    {
        int nGroups = _fixture.Core.nGroups;
        string file = WriteMinimalCommandFile(nGroups);

        _fixture.Core.MSEBatchManager.ReadCommandFile(file);
        _fixture.Core.MSEBatchManager.BatchData.RunType.Should().Be(eMSEBatchRunTypes.TFM);
    }

    [Fact]
    public void ReadCommandFile_SetsNSims_Correctly()
    {
        int nGroups = _fixture.Core.nGroups;
        string file = WriteMinimalCommandFile(nGroups);

        _fixture.Core.MSEBatchManager.ReadCommandFile(file);
        // nParIters reflects the number of batch parameter iterations (TFM = 1)
        _fixture.Core.MSEBatchManager.BatchData.nParIters.Should().Be(1);
    }

    // -----------------------------------------------------------------------
    // Run
    // -----------------------------------------------------------------------

    [Fact]
    public void Run_WithMinimalCommandFile_CompletesWithoutError()
    {
        int nGroups = _fixture.Core.nGroups;
        string file = WriteMinimalCommandFile(nGroups);
        var batchMgr = _fixture.Core.MSEBatchManager;

        bool loaded = batchMgr.ReadCommandFile(file);
        loaded.Should().BeTrue();

        batchMgr.Run();
        batchMgr.Wait();

        batchMgr.BatchData.StopRun.Should().BeFalse();
    }

    [Fact]
    public void Run_IsNotRunning_AfterWaitReturns()
    {
        int nGroups = _fixture.Core.nGroups;
        string file = WriteMinimalCommandFile(nGroups);
        var batchMgr = _fixture.Core.MSEBatchManager;

        batchMgr.ReadCommandFile(file);
        batchMgr.Run();
        batchMgr.Wait();

        batchMgr.IsRunning.Should().BeFalse();
    }
}
