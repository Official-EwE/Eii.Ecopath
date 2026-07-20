// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSE;
using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Tests for MSE data-structure enums and for the <c>cMSEDataStructures</c>
/// properties accessed through the loaded model fixture.
/// </summary>
public sealed class MSEDataStructuresTests
{
    // -----------------------------------------------------------------------
    // Standalone enum tests – require no cCore
    // -----------------------------------------------------------------------

    [Fact]
    public void RegulationMode_HasExpectedValues()
    {
        var values = Enum.GetValues<eMSERegulationMode>();
        values.Should().Contain(eMSERegulationMode.UseRegulations);
        values.Should().Contain(eMSERegulationMode.NoRegulations);
    }

    [Fact]
    public void EffortSource_HasExpectedValues()
    {
        var values = Enum.GetValues<eMSEEffortSource>();
        values.Should().Contain(eMSEEffortSource.EcosimEffort);
        values.Should().Contain(eMSEEffortSource.NoCap);
        values.Should().Contain(eMSEEffortSource.Predicted);
    }

    [Fact]
    public void AssessmentMethods_HasExpectedValues()
    {
        var values = Enum.GetValues<eAssessmentMethods>();
        values.Should().Contain(eAssessmentMethods.Exact);
        values.Should().Contain(eAssessmentMethods.CatchEstmBio);
        values.Should().Contain(eAssessmentMethods.DirectExploitation);
    }

    [Fact]
    public void RunStates_HasExpectedValues()
    {
        var values = Enum.GetValues<eMSERunStates>();
        values.Should().Contain(eMSERunStates.Started);
        values.Should().Contain(eMSERunStates.RunCompleted);
        values.Should().Contain(eMSERunStates.IterationCompleted);
        values.Should().Contain(eMSERunStates.IterationStarted);
    }
}

/// <summary>
/// Tests that verify <c>cMSEDataStructures</c> default values and mutations
/// through the public <c>cMSEManager</c> / <c>cMSEParameters</c> surface once
/// a model has been loaded.
/// </summary>
[Collection("Model")]
public sealed class MSEDataStructuresIntegrationTests
{
    private readonly ModelFixture _fixture;

    public MSEDataStructuresIntegrationTests(ModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MSEManager_IsAvailable_AfterModelLoad()
    {
        _fixture.Core.MSEManager.Should().NotBeNull();
    }

    [Fact]
    public void ModelParameters_DefaultNTrials_IsPositive()
    {
        _fixture.Core.MSEManager.ModelParameters.NTrials.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ModelParameters_NTrials_CanBeSetAndRead()
    {
        var mse = _fixture.Core.MSEManager;
        int original = mse.ModelParameters.NTrials;
        try
        {
            mse.ModelParameters.NTrials = 7;
            mse.ModelParameters.NTrials.Should().Be(7);
        }
        finally
        {
            mse.ModelParameters.NTrials = original;
        }
    }

    [Fact]
    public void MSEManager_NumGroups_MatchesCoreNGroups()
    {
        _fixture.Core.MSEManager.NumGroups.Should().Be(_fixture.Core.nGroups);
    }

    [Fact]
    public void MSEManager_NumFleets_MatchesCoreNFleets()
    {
        _fixture.Core.MSEManager.NumFleets.Should().Be(_fixture.Core.nFleets);
    }

    [Fact]
    public void BiomassStats_AreAvailable_ForEachGroup()
    {
        var mse = _fixture.Core.MSEManager;
        for (int i = 1; i <= mse.NumGroups; i++)
        {
            ((EwECore.MSE.cMSEStats)mse.BiomassStats[i]).Should().NotBeNull();
        }
    }
}
