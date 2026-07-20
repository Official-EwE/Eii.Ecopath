// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using System.Text.Json;
using System.Text.Json.Serialization;
using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.Regression;

/// <summary>
/// Numerical regression test for the MSE engine.
/// <para>
/// On the first run (no baseline file) the test records the current output
/// into <c>Regression/Baselines/mse_baseline.json</c> and passes. On every
/// subsequent run it loads that file and asserts that all captured values are
/// within a small relative tolerance — catching any unintended numerical
/// drift introduced by code changes.
/// </para>
/// <para>
/// The run is made deterministic by setting <c>m_MSEData.bInBatch = true</c>
/// before calling <c>Run()</c>. That causes the engine to use seed 42 instead
/// of a time-based seed.
/// </para>
/// </summary>
[Collection("Model")]
public sealed class MSERegressionTests
{
    // Path is relative to the test output directory, which is the same folder
    // this assembly is deployed to at build time.
    private static readonly string BaselineFile =
        Path.Combine(AppContext.BaseDirectory, "Regression", "Baselines", "mse_baseline.json");

    private const int TrialCount = 5;
    private const double RelativeTolerance = 1e-4;

    private readonly ModelFixture _fixture;

    public MSERegressionTests(ModelFixture fixture)
    {
        _fixture = fixture;
    }

    // -----------------------------------------------------------------------
    // Snapshot model
    // -----------------------------------------------------------------------

    private sealed class MseBaseline
    {
        public float MeanEcologicalValue { get; set; }
        public float MeanEconomicValue { get; set; }
        public float MeanEmployValue { get; set; }
        public float MeanMandatedValue { get; set; }
        public int NTrials { get; set; }
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static bool IsEffectivelyZero(float v) => Math.Abs(v) < 1e-12f;

    private static void AssertClose(float expected, float actual, string label)
    {
        if (IsEffectivelyZero(expected))
        {
            // Absolute tolerance for near-zero values
            Math.Abs(actual - expected).Should().BeLessThan(1e-4f,
                $"{label}: absolute difference too large (expected ~0, got {actual})");
        }
        else
        {
            double relErr = Math.Abs((actual - expected) / expected);
            relErr.Should().BeLessThan(RelativeTolerance,
                $"{label}: relative error {relErr:P4} exceeds tolerance (expected {expected}, got {actual})");
        }
    }

    // -----------------------------------------------------------------------
    // Test
    // -----------------------------------------------------------------------

    [Fact]
    public void MSE_NumericalOutputs_MatchOrCreateBaseline()
    {
        var mse = _fixture.Core.MSEManager;
        int originalTrials = mse.ModelParameters.NTrials;
        bool originalBatch = _fixture.Core.MSEManager.MSEData.bInBatch;

        try
        {
            // Force deterministic seed (seed 42 via bInBatch == true).
            _fixture.Core.MSEManager.MSEData.bInBatch = true;
            mse.ModelParameters.NTrials = TrialCount;

            mse.Run();
            mse.Wait();

            var output = mse.Output();
            var snapshot = new MseBaseline
            {
                MeanEcologicalValue = output.MeanEcologicalValue,
                MeanEconomicValue = output.MeanEconomicValue,
                MeanEmployValue = output.MeanEmployValue,
                MeanMandatedValue = output.MeanMandatedValue,
                NTrials = TrialCount
            };

            if (!File.Exists(BaselineFile))
            {
                // First run — write baseline and pass.
                Directory.CreateDirectory(Path.GetDirectoryName(BaselineFile)!);
                var opts = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(BaselineFile, JsonSerializer.Serialize(snapshot, opts));
                return; // Baseline written; test passes unconditionally.
            }

            // Subsequent runs — compare against baseline.
            string json = File.ReadAllText(BaselineFile);
            var baseline = JsonSerializer.Deserialize<MseBaseline>(json)!;

            baseline.Should().NotBeNull("baseline file must contain valid JSON");
            baseline.NTrials.Should().Be(TrialCount, "baseline was recorded with a different trial count");

            AssertClose(baseline.MeanEcologicalValue, snapshot.MeanEcologicalValue, nameof(MseBaseline.MeanEcologicalValue));
            AssertClose(baseline.MeanEconomicValue, snapshot.MeanEconomicValue, nameof(MseBaseline.MeanEconomicValue));
            AssertClose(baseline.MeanEmployValue, snapshot.MeanEmployValue, nameof(MseBaseline.MeanEmployValue));
            AssertClose(baseline.MeanMandatedValue, snapshot.MeanMandatedValue, nameof(MseBaseline.MeanMandatedValue));
        }
        finally
        {
            _fixture.Core.MSEManager.MSEData.bInBatch = originalBatch;
            mse.ModelParameters.NTrials = originalTrials;
        }
    }
}
