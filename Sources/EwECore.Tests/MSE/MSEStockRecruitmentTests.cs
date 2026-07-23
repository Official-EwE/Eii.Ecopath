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
/// The estimator is exercised in isolation: minimal <c>cMSEDataStructures</c>,
/// <c>cEcosimDatastructures</c> and <c>cSearchDatastructures</c> instances are
/// built by hand (no <c>cCore</c>, no plugin manager, no loaded model), so the
/// delay-difference / Kalman-filter math can be verified deterministically.
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

    /// <summary>
    /// Builds a hand-wired <see cref="cMSEStockRecruitment"/> plus the data
    /// structures it reads and mutates, sized for the given dimensions.
    /// Arrays are 1-based (index 0 unused) to match the VB engine conventions.
    /// </summary>
    private static (cMSEStockRecruitment recruiter,
                    cMSEDataStructures data,
                    cEcosimDatastructures esData,
                    cSearchDatastructures search) BuildRecruiter(int numGroups, int numLiving)
    {
        var epData = new cEcopathDataStructures(null)
        {
            NumGroups = numGroups,
            NumLiving = numLiving,
            NumFleet = 1
        };

        var esData = new cEcosimDatastructures
        {
            nGear = 1,
            Fish1 = new float[numGroups + 1]
        };

        var data = new cMSEDataStructures(epData, esData)
        {
            BestimateLast = new float[numGroups + 1],
            CatchYearGroup = new float[numGroups + 1],
            Rmax = new float[numGroups + 1],
            BhalfT = new float[numGroups + 1],
            RstockRatio = new float[numGroups + 1],
            cvRec = new float[numGroups + 1],
            GstockPred = new float[numGroups + 1],
            KalmanGain = new float[numGroups + 1],
            CVbiomEst = new float[numGroups + 1]
        };

        // BioEstStats is written to as a side effect. Wire a real summary-stats
        // object with a fixed time-step count so AddValue succeeds.
        var stats = new cMSESummaryStats(
            data, null, numGroups + 1, 1, eCoreCounterTypes.nGroups, _ => 5);
        stats.Init();
        stats.AddIteration();
        data.BioEstStats = stats;

        var search = new cSearchDatastructures(null, epData)
        {
            CatchYearGroup = new float[numGroups + 1]
        };

        var recruiter = new cMSEStockRecruitment(data, esData, search);
        return (recruiter, data, esData, search);
    }

    // -----------------------------------------------------------------------
    // Full observation: KalmanGain == 1 collapses Best onto the observed biomass
    // -----------------------------------------------------------------------

    [Fact]
    public void KalmanGainOfOne_ReturnsObservedBiomass()
    {
        // Arrange
        const int igrp = 1;
        var (recruiter, data, _, search) = BuildRecruiter(numGroups: 1, numLiving: 1);
        search.CatchYearGroup[igrp] = 0f;   // no catch -> Exp(0) -> BestimateLast = Blast
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
        var (recruiter, data, _, search) = BuildRecruiter(numGroups: 1, numLiving: 1);
        search.CatchYearGroup[igrp] = 0f;
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
        var (recruiter, data, _, search) = BuildRecruiter(numGroups: 1, numLiving: 1);
        search.CatchYearGroup[igrp] = 2f;   // BestimateLast = 4 * Exp(-2/4 + 0) = 4 * Exp(-0.5)
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
    // The group catch accumulator is cleared after the estimate is produced
    // -----------------------------------------------------------------------

    [Fact]
    public void StockRecruitment_ResetsDataCatchYearGroupToZero()
    {
        // Arrange
        const int igrp = 1;
        var (recruiter, data, _, search) = BuildRecruiter(numGroups: 1, numLiving: 1);
        data.CatchYearGroup[igrp] = 99f;    // should be cleared by the estimator
        search.CatchYearGroup[igrp] = 0f;
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;
        data.GstockPred[igrp] = 0f;
        data.CVbiomEst[igrp] = 0f;

        // Act
        recruiter.StockRecruitment(igrp, B: 3f, BioEst: 3f, Blast: 2f, iCurYear: 1);

        // Assert
        data.CatchYearGroup[igrp].Should().Be(0f);
    }

    // -----------------------------------------------------------------------
    // Side effect: the predicted/actual ratio (Best / B) is recorded in BioEstStats
    // -----------------------------------------------------------------------

    [Fact]
    public void StockRecruitment_StoresPredictedActualRatioInBioEstStats()
    {
        // Arrange
        const int igrp = 1, iCurYear = 1;
        var (recruiter, data, _, search) = BuildRecruiter(numGroups: 1, numLiving: 1);
        search.CatchYearGroup[igrp] = 0f;
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;
        data.GstockPred[igrp] = 0f;
        data.CVbiomEst[igrp] = 0f;          // KalmanGain = 1 -> Best = BioEst = 3, val = Best / B = 1

        // Act
        recruiter.StockRecruitment(igrp, B: 3f, BioEst: 3f, Blast: 2f, iCurYear: iCurYear);

        // Assert
        data.BioEstStats.get_Values(igrp, 1)[iCurYear].Should().BeApproximately(1f, Tolerance);
    }
}
