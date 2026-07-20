// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Parameterised tests that verify the MSE engine runs exactly the requested
/// number of trials and advances <c>CurrentIteration</c> accordingly.
/// </summary>
[Collection("Model")]
public sealed class MSETrialCountTests
{
    private readonly ModelFixture _fixture;

    public MSETrialCountTests(ModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Run_CompletesExpectedNumberOfTrials(int nTrials)
    {
        var mse = _fixture.Core.MSEManager;
        int originalTrials = mse.ModelParameters.NTrials;
        try
        {
            mse.ModelParameters.NTrials = nTrials;
            mse.Run();
            mse.Wait();

            // CurrentIteration is the 1-based index of the last completed trial;
            // after a clean run it should equal the number of trials requested.
            _fixture.Core.MSEManager.MSEData.CurrentIteration.Should().Be(nTrials);
        }
        finally
        {
            mse.ModelParameters.NTrials = originalTrials;
        }
    }

    [Fact]
    public void ModelParameters_NTrials_IsPersistedCorrectly()
    {
        var mse = _fixture.Core.MSEManager;
        int original = mse.ModelParameters.NTrials;
        try
        {
            foreach (int n in new[] { 1, 5, 10 })
            {
                mse.ModelParameters.NTrials = n;
                mse.ModelParameters.NTrials.Should().Be(n);
            }
        }
        finally
        {
            mse.ModelParameters.NTrials = original;
        }
    }
}
