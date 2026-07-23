// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;
using EwECore.MSE;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Pure unit tests for <see cref="cMSEQuotaUpdater"/>.
/// <para>
/// The updater is exercised in isolation: minimal <c>cEcopathDataStructures</c>,
/// <c>cEcosimDatastructures</c> and <c>cMSEDataStructures</c> instances are built
/// by hand (no <c>cCore</c>, no plugin manager, no loaded model), so each quota
/// branch can be verified deterministically.
/// </para>
/// <para>
/// Per living group <c>igrp</c> the updater computes:
/// <code>
/// TAC &gt; 0             -&gt; tQuota = Round(TAC, 5)
/// FixedEscapement &gt; 0 -&gt; tQuota = max(Bestimate - FixedEscapement, 0)
/// FixedF &gt; 0          -&gt; FTarget = Round(FixedF, 5); tQuota = FTarget * Bestimate
/// otherwise           -&gt; hockey stick:
///                        brange  = max(Bbase - Blim, 1e-20)
///                        FTarget = clamp(Fopt * (Bestimate - Blim) / brange, Fmin, Fopt)
///                        tQuota  = FTarget * Bestimate
/// tQuota *= Exp(CVbiomEst * randomNormal() - 0.5 * CVbiomEst^2)
/// QuotaTime(iflt, igrp) = tQuota * Quotashare(iflt, igrp)
/// </code>
/// All tests set <c>CVbiomEst = 0</c> so the uncertainty factor is <c>Exp(0) = 1</c>
/// and the calculation is fully deterministic regardless of the random source.
/// </para>
/// </summary>
public sealed class cMSEQuotaUpdaterTests
{
    private const float Tolerance = 1e-4f;

    /// <summary>Deterministic random source; value is irrelevant while CVbiomEst == 0.</summary>
    private static float ZeroRandom() => 0f;

    /// <summary>
    /// Builds a minimal, hand-wired <see cref="cMSEQuotaUpdater"/> plus the shared
    /// <see cref="cMSEDataStructures"/> it mutates, sized for the given dimensions.
    /// Arrays are 1-based (index 0 unused) to match the VB engine conventions.
    /// </summary>
    private static (cMSEQuotaUpdater updater, cMSEDataStructures data) BuildUpdater(
        int numGroups, int numLiving, int nGear)
    {
        var epData = new cEcopathDataStructures(null)
        {
            NumGroups = numGroups,
            NumLiving = numLiving,
            NumFleet = nGear
        };

        var esData = new cEcosimDatastructures
        {
            nGear = nGear,
            Fish1 = new float[numGroups + 1]
        };

        var data = new cMSEDataStructures(epData, esData)
        {
            TAC = new float[numGroups + 1],
            FixedEscapement = new float[numGroups + 1],
            FixedF = new float[numGroups + 1],
            Fopt = new float[numGroups + 1],
            Fmin = new float[numGroups + 1],
            Bbase = new float[numGroups + 1],
            Blim = new float[numGroups + 1],
            Bestimate = new float[numGroups + 1],
            CVbiomEst = new float[numGroups + 1],
            FTarget = new float[numGroups + 1],
            Quotashare = new float[nGear + 1, numGroups + 1],
            QuotaTime = new float[nGear + 1, numGroups + 1],
            // Fields read/written by cMSEStockRecruitment during DoAssessment.
            BestimateLast = new float[numGroups + 1],
            CatchYearGroup = new float[numGroups + 1],
            Rmax = new float[numGroups + 1],
            BhalfT = new float[numGroups + 1],
            RstockRatio = new float[numGroups + 1],
            cvRec = new float[numGroups + 1],
            GstockPred = new float[numGroups + 1],
            KalmanGain = new float[numGroups + 1]
        };

        // BioEstStats is written to as a side effect of StockRecruitment. Wire a
        // real summary-stats object with a fixed time-step count so AddValue succeeds.
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
        var updater = new cMSEQuotaUpdater(data, epData, esData, recruiter);
        return (updater, data);
    }

    // -----------------------------------------------------------------------
    // Total Allowable Catch branch
    // -----------------------------------------------------------------------

    [Fact]
    public void TotalAllowableCatch_SetsQuotaToRoundedTac_AndSharesAcrossFleet()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.TAC[igrp] = 12.3456789f;   // rounded to 5 decimals -> 12.34568
        data.Quotashare[iflt, igrp] = 0.5f;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        tQuota[igrp].Should().BeApproximately(12.34568f, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(12.34568f * 0.5f, Tolerance);
    }

    // -----------------------------------------------------------------------
    // Fixed Escapement branch
    // -----------------------------------------------------------------------

    [Fact]
    public void FixedEscapement_SetsQuotaToBiomassMinusEscapement()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.FixedEscapement[igrp] = 0.4f;
        data.Bestimate[igrp] = 1.0f;
        data.Quotashare[iflt, igrp] = 1.0f;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        tQuota[igrp].Should().BeApproximately(0.6f, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(0.6f, Tolerance);
    }

    [Fact]
    public void FixedEscapement_AboveBiomass_ClampsQuotaToZero()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.FixedEscapement[igrp] = 1.5f; // greater than biomass -> negative -> clamped to 0
        data.Bestimate[igrp] = 1.0f;
        data.Quotashare[iflt, igrp] = 1.0f;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        tQuota[igrp].Should().BeApproximately(0f, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(0f, Tolerance);
    }

    // -----------------------------------------------------------------------
    // Fixed Fishing Mortality branch
    // -----------------------------------------------------------------------

    [Fact]
    public void FixedF_SetsFTargetAndQuotaFromBiomass()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.FixedF[igrp] = 0.2f;
        data.Bestimate[igrp] = 3.0f;
        data.Quotashare[iflt, igrp] = 1.0f;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        data.FTarget[igrp].Should().BeApproximately(0.2f, Tolerance);
        tQuota[igrp].Should().BeApproximately(0.6f, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(0.6f, Tolerance);
    }

    // -----------------------------------------------------------------------
    // Target Fishing Mortality (hockey stick) branch
    // -----------------------------------------------------------------------

    [Fact]
    public void TargetF_NominalBiomass_ComputesHockeyStickFTarget()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, bestimate = 1.25f, quotashare = 0.5f;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.Bbase[igrp] = bbase;
        data.Blim[igrp] = blim;
        data.Fopt[igrp] = fopt;
        data.Bestimate[igrp] = bestimate;
        data.Quotashare[iflt, igrp] = quotashare;
        float expectedFTarget = fopt * (bestimate - blim) / (bbase - blim);

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        data.FTarget[igrp].Should().BeApproximately(expectedFTarget, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(expectedFTarget * bestimate * quotashare, Tolerance);
    }

    [Fact]
    public void TargetF_BiomassAtOrAboveBbase_ClampsToFopt()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, bestimate = 3.0f, quotashare = 1.0f;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.Bbase[igrp] = bbase;
        data.Blim[igrp] = blim;
        data.Fopt[igrp] = fopt;
        data.Bestimate[igrp] = bestimate;
        data.Quotashare[iflt, igrp] = quotashare;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        data.FTarget[igrp].Should().BeApproximately(fopt, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(fopt * bestimate * quotashare, Tolerance);
    }

    [Fact]
    public void TargetF_LowBiomass_WithPositiveFmin_ClampsToFmin()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, fmin = 0.1f, bestimate = 0.4f, quotashare = 1.0f;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.Bbase[igrp] = bbase;
        data.Blim[igrp] = blim;
        data.Fopt[igrp] = fopt;
        data.Fmin[igrp] = fmin; // biomass below Blim -> raw FTarget < 0 -> clamp up to Fmin
        data.Bestimate[igrp] = bestimate;
        data.Quotashare[iflt, igrp] = quotashare;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        data.FTarget[igrp].Should().BeApproximately(fmin, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(fmin * bestimate * quotashare, Tolerance);
    }

    [Fact]
    public void TargetF_DegenerateBrange_UsesFallback_ClampsToFopt()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        // Bbase <= Blim forces brange to the 1e-20 fallback -> raw FTarget huge -> clamp to Fopt.
        const float bbase = 1.0f, blim = 1.0f, fopt = 0.3f, bestimate = 1.5f, quotashare = 1.0f;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.Bbase[igrp] = bbase;
        data.Blim[igrp] = blim;
        data.Fopt[igrp] = fopt;
        data.Bestimate[igrp] = bestimate;
        data.Quotashare[iflt, igrp] = quotashare;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        data.FTarget[igrp].Should().BeApproximately(fopt, Tolerance);
        data.QuotaTime[iflt, igrp].Should().BeApproximately(fopt * bestimate * quotashare, Tolerance);
    }

    // -----------------------------------------------------------------------
    // Fleet sharing
    // -----------------------------------------------------------------------

    [Fact]
    public void FleetShare_DistributesGroupQuotaAcrossFleetsByQuotashare()
    {
        // Arrange
        const int igrp = 1;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 2);
        data.TAC[igrp] = 10.0f;
        data.Quotashare[1, igrp] = 0.7f;
        data.Quotashare[2, igrp] = 0.3f;

        // Act
        float[] tQuota = updater.UpdateQuotas(ZeroRandom);

        // Assert
        tQuota[igrp].Should().BeApproximately(10.0f, Tolerance);
        data.QuotaTime[1, igrp].Should().BeApproximately(7.0f, Tolerance);
        data.QuotaTime[2, igrp].Should().BeApproximately(3.0f, Tolerance);
    }

    // -----------------------------------------------------------------------
    // DoAssessment: stock-recruitment biomass estimation (moved from cMSE)
    // -----------------------------------------------------------------------

    [Fact]
    public void DoAssessment_KalmanGainOfOne_SetsBestimateToObservedBiomass()
    {
        // Arrange
        const int igrp = 1;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.Bestimate[igrp] = 2.0f;      // Blast for the delay-difference model (> 0)
        data.CVbiomEst[igrp] = 0f;        // ZeroRandom -> Bobs = Biomass, KalmanGain = 1
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;
        data.GstockPred[igrp] = 0f;
        var biomass = new float[] { 0f, 3.0f };

        // Act
        updater.DoAssessment(biomass, curYear: 1, randomNormal: ZeroRandom);

        // Assert
        data.Bestimate[igrp].Should().BeApproximately(3.0f, Tolerance);
        data.KalmanGain[igrp].Should().BeApproximately(1f, Tolerance);
    }

    [Fact]
    public void DoAssessment_PartialKalmanGain_BlendsObservationAndPrediction()
    {
        // Arrange
        const int igrp = 1;
        var (updater, data) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.Bestimate[igrp] = 2.0f;      // Blast
        data.CVbiomEst[igrp] = 1f;        // KalmanGain = vPred/(vPred+1) = 0.5
        data.Rmax[igrp] = 1f;
        data.BhalfT[igrp] = 2f;           // RstockPred = 1*2/(2+2) = 0.5
        data.RstockRatio[igrp] = 1f;
        data.cvRec[igrp] = 1f;            // vPred = 1
        data.GstockPred[igrp] = 0f;
        var biomass = new float[] { 0f, 3.0f };

        // Act
        // Best = 0.5*3 + 0.5*(0*2 + 0.5) = 1.5 + 0.25 = 1.75
        updater.DoAssessment(biomass, curYear: 1, randomNormal: ZeroRandom);

        // Assert
        data.Bestimate[igrp].Should().BeApproximately(1.75f, Tolerance);
        data.KalmanGain[igrp].Should().BeApproximately(0.5f, Tolerance);
    }

    [Fact]
    public void DoAssessment_OnlyProcessesLivingGroups()
    {
        // Arrange
        const int living = 1, nonLiving = 2;
        var (updater, data) = BuildUpdater(numGroups: 2, numLiving: 1, nGear: 1);
        data.Bestimate[living] = 2.0f;
        data.Bestimate[nonLiving] = 42.0f; // must stay untouched (index > nLiving)
        data.CVbiomEst[living] = 0f;
        data.Rmax[living] = 1f;
        data.BhalfT[living] = 2f;
        data.RstockRatio[living] = 1f;
        data.cvRec[living] = 1f;
        data.GstockPred[living] = 0f;
        var biomass = new float[] { 0f, 3.0f, 5.0f };

        // Act
        updater.DoAssessment(biomass, curYear: 1, randomNormal: ZeroRandom);

        // Assert
        data.Bestimate[living].Should().BeApproximately(3.0f, Tolerance);
        data.Bestimate[nonLiving].Should().Be(42.0f);
    }
}
