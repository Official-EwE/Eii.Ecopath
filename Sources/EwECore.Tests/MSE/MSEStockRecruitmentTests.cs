// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;
using EwECore.MSE;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Pure unit tests for <see cref="cMSEStockRecruitment"/>.
/// <para>
/// The estimator is exercised in isolation through a hand-rolled <see cref="FakeQuotaData"/>
/// implementing <see cref="IMSEQuotaData"/> (no <c>cCore</c>, no core data structures, no
/// loaded model), so the delay-difference / Kalman-filter math can be verified deterministically.
/// </para>
/// <para>
/// For living group <c>iGroup</c> the estimator computes:
/// <code>
/// BestimateLast = Blast * Exp(-Search.CatchYearGroup / Blast + Fish1)
/// Data.CatchYearGroup = 0
/// RstockPred    = Rmax * BestimateLast / (BhalfT + BestimateLast)
/// vPred         = (RstockRatio * cvRec)^2 / (1 - GstockPred^2)
/// KalmanGain    = vPred / (vPred + CVbiomEst^2)
/// Best          = KalmanGain * BioEst + (1 - KalmanGain) * (GstockPred * BestimateLast + RstockPred)
/// BioEstStats.AddValue(iGroup, iCurYear, Best / B)
/// return Best
/// </code>
/// </para>
/// </summary>
public sealed class cMSEStockRecruitmentTests
{
    private const float Tolerance = 1e-4f;

    /// <summary>Records every <see cref="IMSESummaryStats.AddValue"/> call for verification.</summary>
    private sealed class SpyBioEstStats : IMSESummaryStats
    {
        public List<(int Index, int TimeIndex, float Value)> Recorded { get; } = new();

        public void AddValue(int index, int TimeIndex, float Value)
            => Recorded.Add((index, TimeIndex, Value));
    }

    /// <summary>
    /// Builds a hand-wired <see cref="cMSEStockRecruitment"/> plus the fake data it reads and
    /// mutates, sized for the given dimensions. Arrays are 1-based (index 0 unused) to match
    /// the VB engine conventions.
    /// </summary>
    private static (cMSEStockRecruitment recruiter,
                    FakeQuotaData data,
                    SpyBioEstStats stats) BuildRecruiter(int numGroups, int numLiving)
    {
        var data = new FakeQuotaData(numGroups, numLiving, nGear: 1);
        var stats = new SpyBioEstStats();
        data.BioEstStats = stats;

        var recruiter = new cMSEStockRecruitment() { Data = data };
        return (recruiter, data, stats);
    }

    // -----------------------------------------------------------------------
    // Full observation: KalmanGain == 1 collapses Best onto the observed biomass
    // -----------------------------------------------------------------------

    [Fact]
    public void KalmanGainOfOne_ReturnsObservedBiomass()
    {
        // Arrange
        const int igrp = 1;
        var (recruiter, data, _) = BuildRecruiter(numGroups: 1, numLiving: 1);
        data.CatchYearGroup[igrp] = 0f;     // no catch -> Exp(0) -> BestimateLast = Blast
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;
        data.GstockPred[igrp] = 0f;
        data.CVbiomEst[igrp] = 0f;          // -> KalmanGain = 1

        // Act
        float best = recruiter.StockRecruitment(igrp, B: 3f, BioEst: 3f, Blast: 2f, iCurYear: 1);

        // Assert
        best.Should().BeApproximately(3f, Tolerance);
        data.BestimateLast[igrp].Should().BeApproximately(2f, Tolerance);
        data.KalmanGain[igrp].Should().BeApproximately(1f, Tolerance);
    }

    // -----------------------------------------------------------------------
    // Partial observation: KalmanGain == 0.5 blends observation and prediction
    // -----------------------------------------------------------------------

    [Fact]
    public void PartialKalmanGain_BlendsObservationAndPrediction()
    {
        // Arrange
        const int igrp = 1;
        var (recruiter, data, _) = BuildRecruiter(numGroups: 1, numLiving: 1);
        data.CatchYearGroup[igrp] = 0f;
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;             // RstockPred = 1*2/(2+2) = 0.5
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;              // vPred = 1
        data.GstockPred[igrp] = 0f;
        data.CVbiomEst[igrp] = 1f;          // KalmanGain = 1/(1+1) = 0.5

        // Act
        // Best = 0.5*3 + 0.5*(0*2 + 0.5) = 1.5 + 0.25 = 1.75
        float best = recruiter.StockRecruitment(igrp, B: 3f, BioEst: 3f, Blast: 2f, iCurYear: 1);

        // Assert
        best.Should().BeApproximately(1.75f, Tolerance);
        data.KalmanGain[igrp].Should().BeApproximately(0.5f, Tolerance);
    }

    // -----------------------------------------------------------------------
    // Delay-difference update: catch reduces the carried-over biomass estimate
    // -----------------------------------------------------------------------

    [Fact]
    public void CatchYearGroup_ReducesBestimateLastExponentially()
    {
        // Arrange
        const int igrp = 1;
        var (recruiter, data, _) = BuildRecruiter(numGroups: 1, numLiving: 1);
        data.CatchYearGroup[igrp] = 2f;     // BestimateLast = 4 * Exp(-2/4 + 0) = 4 * Exp(-0.5)
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;
        data.GstockPred[igrp] = 0f;
        data.CVbiomEst[igrp] = 0f;

        // Act
        recruiter.StockRecruitment(igrp, B: 3f, BioEst: 3f, Blast: 4f, iCurYear: 1);

        // Assert
        data.BestimateLast[igrp].Should().BeApproximately(4f * (float)Math.Exp(-0.5), Tolerance);
    }

    // -----------------------------------------------------------------------
    // Side effect: the predicted/actual ratio (Best / B) is recorded in BioEstStats
    // -----------------------------------------------------------------------

    [Fact]
    public void StockRecruitment_StoresPredictedActualRatioInBioEstStats()
    {
        // Arrange
        const int igrp = 1, iCurYear = 1;
        var (recruiter, data, stats) = BuildRecruiter(numGroups: 1, numLiving: 1);
        data.CatchYearGroup[igrp] = 0f;
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;
        data.GstockPred[igrp] = 0f;
        data.CVbiomEst[igrp] = 0f;          // KalmanGain = 1 -> Best = BioEst = 3, val = Best / B = 1

        // Act
        recruiter.StockRecruitment(igrp, B: 3f, BioEst: 3f, Blast: 2f, iCurYear: iCurYear);

        // Assert
        stats.Recorded.Should().ContainSingle();
        stats.Recorded[0].Index.Should().Be(igrp);
        stats.Recorded[0].TimeIndex.Should().Be(iCurYear);
        stats.Recorded[0].Value.Should().BeApproximately(1f, Tolerance);
    }
}
