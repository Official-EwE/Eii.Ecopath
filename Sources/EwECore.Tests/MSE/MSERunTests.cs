// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSE;
using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Integration tests that exercise a full MSE <c>Run()</c> cycle against the
/// loaded Anchovy Bay Spatial model.  Each test launches the run and then calls
/// <c>Wait()</c> to block until the run thread has finished.
/// </summary>
[Collection("Model")]
public sealed class MSERunTests
{
    private readonly ModelFixture _fixture;

    public MSERunTests(ModelFixture fixture)
    {
        _fixture = fixture;
    }

    private cMSEManager MSE => _fixture.Core.MSEManager;

    // -----------------------------------------------------------------------
    // ValidateRun
    // -----------------------------------------------------------------------

    [Fact]
    public void ValidateRun_WithDefaultSettings_ReturnsTrue()
    {
        // Arrange
        // no setup required – the fixture already loaded the model

        // Act
        bool result = MSE.ValidateRun();

        // Assert
        // The default LP solution mode means ValidateRun only checks time series;
        // for a plain Anchovy Bay Spatial model this should pass.
        result.Should().BeTrue();
    }

    // -----------------------------------------------------------------------
    // Run / Wait
    // -----------------------------------------------------------------------

    [Fact]
    public void Run_WithOneTrialAndDefaultSettings_CompletesSuccessfully()
    {
        // Arrange
        int originalTrials = MSE.ModelParameters.NTrials;
        MSE.ModelParameters.NTrials = 1;

        try
        {
            // Act
            bool started = MSE.Run();
            bool finished = MSE.Wait();

            // Assert
            started.Should().BeTrue("Run() should return true when the background thread starts");
            finished.Should().BeTrue("Wait() should return true once the MSE run completes");
        }
        finally
        {
            // Restore so other tests in the collection are not affected.
            MSE.ModelParameters.NTrials = originalTrials;
        }
    }

    [Fact]
    public void Run_IsNotRunning_AfterWaitReturns()
    {
        // Arrange
        int originalTrials = MSE.ModelParameters.NTrials;
        MSE.ModelParameters.NTrials = 1;

        try
        {
            // Act
            MSE.Run();
            MSE.Wait();

            // Assert
            MSE.IsRunning.Should().BeFalse("the MSE thread should have finished");
        }
        finally
        {
            MSE.ModelParameters.NTrials = originalTrials;
        }
    }

    [Fact]
    public void Run_StopRun_IsFalse_AfterNormalCompletion()
    {
        // Arrange
        int originalTrials = MSE.ModelParameters.NTrials;
        MSE.ModelParameters.NTrials = 1;

        try
        {
            // Act
            MSE.Run();
            MSE.Wait();

            // Assert
            // StopRun should still be false — it is only set to true by an explicit
            // user/batch cancellation.
            _fixture.Core.MSEManager.MSEData.StopRun.Should().BeFalse();
        }
        finally
        {
            MSE.ModelParameters.NTrials = originalTrials;
        }
    }

    // -----------------------------------------------------------------------
    // Output availability after a run
    // -----------------------------------------------------------------------

    [Fact]
    public void BiomassStats_ArePopulated_AfterRun()
    {
        // Arrange
        int originalTrials = MSE.ModelParameters.NTrials;
        MSE.ModelParameters.NTrials = 1;

        try
        {
            // Act
            MSE.Run();
            MSE.Wait();

            // Assert
            for (int i = 1; i <= MSE.NumGroups; i++)
            {
                ((EwECore.MSE.cMSEStats)MSE.BiomassStats[i]).Should().NotBeNull();
            }
        }
        finally
        {
            MSE.ModelParameters.NTrials = originalTrials;
        }
    }

    [Fact]
    public void EffortStats_ArePopulated_AfterRun()
    {
        // Arrange
        int originalTrials = MSE.ModelParameters.NTrials;
        MSE.ModelParameters.NTrials = 1;

        try
        {
            // Act
            MSE.Run();
            MSE.Wait();

            // Assert
            for (int i = 1; i <= MSE.NumFleets; i++)
            {
                ((EwECore.MSE.cMSEStats)MSE.EffortStats[i]).Should().NotBeNull();
            }
        }
        finally
        {
            MSE.ModelParameters.NTrials = originalTrials;
        }
    }
}
