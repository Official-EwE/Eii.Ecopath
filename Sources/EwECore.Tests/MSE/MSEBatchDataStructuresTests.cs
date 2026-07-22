// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSEBatchManager;
using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Standalone tests for MSE batch enums and constants — no <c>cCore</c> required.
/// </summary>
public sealed class MSEBatchEnumTests
{
    [Fact]
    public void MSEBatchRunTypes_HasExpectedValues()
    {
        // Arrange
        // no setup required

        // Act
        var values = Enum.GetValues<eMSEBatchRunTypes>();

        // Assert
        values.Should().Contain(eMSEBatchRunTypes.Any);
        values.Should().Contain(eMSEBatchRunTypes.FixedF);
        values.Should().Contain(eMSEBatchRunTypes.TAC);
        values.Should().Contain(eMSEBatchRunTypes.TFM);
        values.Should().Contain(eMSEBatchRunTypes.NotManaged);
    }

    [Fact]
    public void MSEBatchOutputTypes_HasExpectedValues()
    {
        // Arrange
        // no setup required

        // Act
        var values = Enum.GetValues<eMSEBatchOuputTypes>();

        // Assert
        values.Should().Contain(eMSEBatchOuputTypes.NotSet);
        values.Should().Contain(eMSEBatchOuputTypes.Biomass);
        values.Should().Contain(eMSEBatchOuputTypes.FishingMortRate);
        values.Should().Contain(eMSEBatchOuputTypes.Effort);
        values.Should().Contain(eMSEBatchOuputTypes.CatchByGroup);
    }

    [Fact]
    public void MSEBatchProgress_HasExpectedValues()
    {
        // Arrange
        // no setup required

        // Act
        var values = Enum.GetValues<eMSEBatchProgress>();

        // Assert
        values.Should().Contain(eMSEBatchProgress.MSEIteration);
        values.Should().Contain(eMSEBatchProgress.RunStarted);
        values.Should().Contain(eMSEBatchProgress.RunCompleted);
    }
}

/// <summary>
/// Tests for <c>cMSEBatchDataStructures</c> initialisation and resize methods,
/// exercised through the loaded model fixture.
/// </summary>
[Collection("Model")]
public sealed class MSEBatchDataStructuresTests
{
    private readonly ModelFixture _fixture;

    public MSEBatchDataStructuresTests(ModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MSEBatchManager_IsAvailable_AfterModelLoad()
    {
        // Arrange
        // no setup required – fixture already loaded the model

        // Act & Assert
        _fixture.Core.MSEBatchManager.Should().NotBeNull();
    }

    [Fact]
    public void BatchData_DefaultStopRun_IsFalse()
    {
        // Arrange
        // no setup required

        // Act & Assert
        _fixture.Core.MSEBatchManager.BatchData.StopRun.Should().BeFalse();
    }

    [Fact]
    public void BatchData_DefaultRunType_IsAny()
    {
        // Arrange
        // no setup required

        // Act & Assert
        _fixture.Core.MSEBatchManager.BatchData.RunType.Should().Be(eMSEBatchRunTypes.Any);
    }

    [Fact]
    public void BatchData_DefaultNForcing_IsAtLeastOne()
    {
        // Arrange
        // no setup required

        // Act & Assert
        _fixture.Core.MSEBatchManager.BatchData.nForcing.Should().BeGreaterThan(0);
    }

    [Fact]
    public void BatchData_DefaultNControlTypes_IsAtLeastOne()
    {
        // Arrange
        // no setup required

        // Act & Assert
        _fixture.Core.MSEBatchManager.BatchData.nControlTypes.Should().BeGreaterThan(0);
    }

    [Fact]
    public void BatchData_nGroups_MatchesCoreNGroups()
    {
        // Arrange
        // no setup required

        // Act & Assert
        _fixture.Core.MSEBatchManager.BatchData.nGroups.Should().Be(_fixture.Core.nGroups);
    }

    [Fact]
    public void BatchData_nFleets_MatchesCoreNFleets()
    {
        // Arrange
        // no setup required

        // Act & Assert
        _fixture.Core.MSEBatchManager.BatchData.nFleets.Should().Be(_fixture.Core.nFleets);
    }

    [Fact]
    public void BatchData_RedimForcing_ResizesArraysCorrectly()
    {
        // Arrange
        var batch = _fixture.Core.MSEBatchManager.BatchData;
        const int n = 3;

        // Act
        batch.redimForcing(n);

        // Assert
        batch.nForcing.Should().Be(n);
        batch.ForcingNames.Should().HaveCount(n + 1);   // 1-based
        batch.ForcingIndexes.Should().HaveCount(n + 1);
        batch.ForcingGroup.Should().HaveCount(n + 1);
    }

    [Fact]
    public void BatchData_RedimControlTypes_ResizesArrayCorrectly()
    {
        // Arrange
        var batch = _fixture.Core.MSEBatchManager.BatchData;
        int nFleets = _fixture.Core.nFleets;
        const int nTypes = 2;

        // Act
        batch.redimControlTypes(nTypes, nFleets);

        // Assert
        batch.nControlTypes.Should().Be(nTypes);
        batch.ControlType.GetUpperBound(0).Should().BeGreaterThanOrEqualTo(nTypes);
        batch.ControlType.GetUpperBound(1).Should().BeGreaterThanOrEqualTo(nFleets);
    }
}
