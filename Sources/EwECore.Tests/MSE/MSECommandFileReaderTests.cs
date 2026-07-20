// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSECommandFile;
using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Standalone tests for <c>cMSECommandFileReader.CanRead()</c> — no <c>cCore</c>
/// required because the method is static (Shared in VB).
/// </summary>
public sealed class MSECommandFileReaderCanReadTests
{
    [Theory]
    [InlineData(cMSECommandFileReader.NSIMS_DATA_TAG, "Number_Sims, 10")]
    [InlineData(cMSECommandFileReader.CV_DATA_TAG, "Error_CV, 0.2")]
    [InlineData(cMSECommandFileReader.RUNTYPE_DATA_TAG, "Run_Type, 3")]
    [InlineData(cMSECommandFileReader.F_DATA_TAG, "Constant_F, 0.1, 0.2")]
    [InlineData(cMSECommandFileReader.Y_DATA_TAG, "Constant_Y, 100")]
    [InlineData(cMSECommandFileReader.TFM_DATA_TAG, "TFM, 0.5, 0.8, 0.3, 0.6")]
    [InlineData(cMSECommandFileReader.VERSION_DATA_TAG, "Control_File_Version, 1.0")]
    [InlineData(cMSECommandFileReader.ENDYEAR_DATA_TAG, "End_Year, 20")]
    [InlineData(cMSECommandFileReader.STARTYEAR_DATA_TAG, "Start_Year, 1")]
    [InlineData(cMSECommandFileReader.PP_DATA_TAG, "PP, 1")]
    [InlineData(cMSECommandFileReader.PPDEV_DATA_TAG, "PP_STDEV, 0.05")]
    public void CanRead_ReturnsTrue_ForMatchingTagAndLine(string tag, string line)
    {
        cMSECommandFileReader.CanRead(tag, line).Should().BeTrue();
    }

    [Theory]
    [InlineData(cMSECommandFileReader.NSIMS_DATA_TAG, "Run_Type, 3")]
    [InlineData(cMSECommandFileReader.CV_DATA_TAG, "Number_Sims, 10")]
    [InlineData(cMSECommandFileReader.RUNTYPE_DATA_TAG, "Constant_F, 0.1")]
    public void CanRead_ReturnsFalse_ForMismatchedTagAndLine(string tag, string line)
    {
        cMSECommandFileReader.CanRead(tag, line).Should().BeFalse();
    }

    [Fact]
    public void CanRead_ReturnsFalse_ForNullLine()
    {
        cMSECommandFileReader.CanRead(cMSECommandFileReader.NSIMS_DATA_TAG, null!).Should().BeFalse();
    }

    [Fact]
    public void CanRead_ReturnsFalse_ForEmptyLine()
    {
        cMSECommandFileReader.CanRead(cMSECommandFileReader.NSIMS_DATA_TAG, string.Empty).Should().BeFalse();
    }

    [Fact]
    public void TagConstants_HaveExpectedStringValues()
    {
        cMSECommandFileReader.NSIMS_DATA_TAG.Should().Be("Number_Sims");
        cMSECommandFileReader.CV_DATA_TAG.Should().Be("Error_CV");
        cMSECommandFileReader.RUNTYPE_DATA_TAG.Should().Be("Run_Type");
        cMSECommandFileReader.F_DATA_TAG.Should().Be("Constant_F");
        cMSECommandFileReader.Y_DATA_TAG.Should().Be("Constant_Y");
        cMSECommandFileReader.TFM_DATA_TAG.Should().Be("TFM");
        cMSECommandFileReader.VERSION_DATA_TAG.Should().Be("Control_File_Version");
        cMSECommandFileReader.ENDYEAR_DATA_TAG.Should().Be("End_Year");
        cMSECommandFileReader.STARTYEAR_DATA_TAG.Should().Be("Start_Year");
        cMSECommandFileReader.PP_DATA_TAG.Should().Be("PP");
        cMSECommandFileReader.PPDEV_DATA_TAG.Should().Be("PP_STDEV");
        cMSECommandFileReader.OUTPUT_DATA_TAG.Should().Be("Output_Directory");
    }
}

/// <summary>
/// Integration tests for <c>cMSECommandFileReader.Read()</c>: writes a minimal
/// command file to a temp directory, passes it to the reader through the batch
/// manager, and verifies the resulting data structures.
/// </summary>
[Collection("Model")]
public sealed class MSECommandFileReaderReadTests : IDisposable
{
    private readonly ModelFixture _fixture;
    private readonly string _tempDir;

    public MSECommandFileReaderReadTests(ModelFixture fixture)
    {
        _fixture = fixture;
        _tempDir = Path.Combine(Path.GetTempPath(), $"EwECore.Tests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private string WriteCommandFile(IEnumerable<string> lines)
    {
        string path = Path.Combine(_tempDir, "test_command.csv");
        File.WriteAllLines(path, lines);
        return path;
    }

    [Fact]
    public void Read_MinimalFile_ReturnsTrue()
    {
        string file = WriteCommandFile(new[]
        {
            $"{cMSECommandFileReader.VERSION_DATA_TAG}, 1.0",
            $"{cMSECommandFileReader.RUNTYPE_DATA_TAG}, 3",
            $"{cMSECommandFileReader.NSIMS_DATA_TAG}, 5",
            $"{cMSECommandFileReader.CV_DATA_TAG}, 0.2",
            $"{cMSECommandFileReader.TFM_DATA_TAG}, 0.5, 0.8, 0.3, 0.6"
        });

        bool result = _fixture.Core.MSEBatchManager.ReadCommandFile(file);
        result.Should().BeTrue();
    }

    [Fact]
    public void Read_NonExistentFile_ReturnsFalse()
    {
        string nonExistent = Path.Combine(_tempDir, "does_not_exist.csv");
        bool result = _fixture.Core.MSEBatchManager.ReadCommandFile(nonExistent);
        result.Should().BeFalse();
    }
}
