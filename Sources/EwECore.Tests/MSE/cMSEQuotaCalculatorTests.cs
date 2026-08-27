// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSE;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Pure unit tests for <see cref="cMSEQuotaCalculator"/>.
/// <para>
/// The calculator is exercised in isolation through the <see cref="IMSEQuotaData"/> and
/// <see cref="IMSEStockRecruitment"/> interfaces, so no <c>cCore</c>, <c>cEcopathDataStructures</c>,
/// <c>cEcosimDatastructures</c> or <c>cSearchDatastructures</c> instances are needed.
/// </para>
/// <para>
/// Per living group <c>igrp</c> the calculator computes:
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
/// All quota tests set <c>CVbiomEst = 0</c> so the uncertainty factor is <c>Exp(0) = 1</c>
/// and the calculation is fully deterministic regardless of the random source.
/// </para>
/// </summary>
public sealed class cMSEQuotaCalculatorTests
{
    private const float Tolerance = 1e-4f;

    /// <summary>Deterministic random source; value is irrelevant while CVbiomEst == 0.</summary>
    private static float ZeroRandom() => 0f;

    /// <summary>Lightweight in-memory implementation of <see cref="IMSEQuotaData"/>.</summary>
    private sealed class FakeQuotaData : IMSEQuotaData
    {
        public FakeQuotaData(int numGroups, int numLiving, int nGear)
        {
            nGroups = numGroups;
            nLiving = numLiving;
            nFleets = nGear;
            TAC = new float[numGroups + 1];
            FixedEscapement = new float[numGroups + 1];
            FixedF = new float[numGroups + 1];
            Fopt = new float[numGroups + 1];
            Fmin = new float[numGroups + 1];
            Bbase = new float[numGroups + 1];
            Blim = new float[numGroups + 1];
            Bestimate = new float[numGroups + 1];
            CVbiomEst = new float[numGroups + 1];
            FTarget = new float[numGroups + 1];
            Quotashare = new float[nGear + 1, numGroups + 1];
            QuotaTime = new float[nGear + 1, numGroups + 1];
            CatchYearGroup = new float[numGroups + 1];
            BestimateLast = new float[numGroups + 1];
            Fish1 = new float[numGroups + 1];
            GstockPred = new float[numGroups + 1];
            RstockRatio = new float[numGroups + 1];
            KalmanGain = new float[numGroups + 1];
            BhalfT = new float[numGroups + 1];
            Rmax = new float[numGroups + 1];
            cvRec = new float[numGroups + 1];
        }

        public int nGroups { get; set; }
        public int nLiving { get; set; }
        public int nFleets { get; set; }
        public float[] TAC { get; set; }
        public float[] FixedEscapement { get; set; }
        public float[] FixedF { get; set; }
        public float[] Fopt { get; set; }
        public float[] Fmin { get; set; }
        public float[] Bbase { get; set; }
        public float[] Blim { get; set; }
        public float[] Bestimate { get; set; }
        public float[] CVbiomEst { get; set; }
        public float[] FTarget { get; set; }
        public float[,] Quotashare { get; set; }
        public float[,] QuotaTime { get; set; }
        public float[] CatchYearGroup { get; set; }
        public float[] BestimateLast { get; set; }
        public float[] Fish1 { get; set; }
        public float[] GstockPred { get; set; }
        public float[] RstockRatio { get; set; }
        public float[] KalmanGain { get; set; }
        public float[] BhalfT { get; set; }
        public float[] Rmax { get; set; }
        public float[] cvRec { get; set; }
        public IMSESummaryStats BioEstStats { get; set; } = null!;
    }

    /// <summary>Records every call and returns a configurable estimate.</summary>
    private sealed class StubStockRecruitment : IMSEStockRecruitment
    {
        public List<(int iGroup, float B, float BioEst, float Blast, int iCurYear)> Calls { get; } = new();

        public Func<int, float, float, float, int, float> Result { get; set; } =
            (iGroup, b, bioEst, blast, iCurYear) => bioEst;
        public IMSEQuotaData Data { set => throw new NotImplementedException(); }

        public float StockRecruitment(int iGroup, float B, float BioEst, float Blast, int iCurYear)
        {
            Calls.Add((iGroup, B, BioEst, Blast, iCurYear));
            return Result(iGroup, B, BioEst, Blast, iCurYear);
        }
    }

    /// <summary>
    /// Builds a hand-wired <see cref="cMSEQuotaCalculator"/> plus the fake data it mutates,
    /// sized for the given dimensions. Arrays are 1-based (index 0 unused) to match the VB engine.
    /// </summary>
    private static (cMSEQuotaCalculator updater, FakeQuotaData data, StubStockRecruitment recruiter) BuildUpdater(
        int numGroups, int numLiving, int nGear)
    {
        var data = new FakeQuotaData(numGroups, numLiving, nGear);
        var recruiter = new StubStockRecruitment();
        var updater = new cMSEQuotaCalculator(recruiter) { Data = data };
        return (updater, data, recruiter);
    }

    // -----------------------------------------------------------------------
    // Total Allowable Catch branch
    // -----------------------------------------------------------------------

    [Fact]
    public void TotalAllowableCatch_SetsQuotaToRoundedTac_AndSharesAcrossFleet()
    {
        // Arrange
        const int igrp = 1, iflt = 1;
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
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
        var (updater, data, _) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 2);
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
    // DoAssessment: delegation to the stock-recruitment model
    // -----------------------------------------------------------------------

    [Fact]
    public void DoAssessment_StoresRecruiterResult_AndPassesPreviousEstimateAsBlast()
    {
        // Arrange
        const int igrp = 1;
        var (updater, data, recruiter) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.Bestimate[igrp] = 2.0f;      // previous estimate, passed to the recruiter as Blast
        data.CVbiomEst[igrp] = 0f;        // ZeroRandom -> Bobs = Biomass
        recruiter.Result = (_, _, _, _, _) => 7.5f;
        var biomass = new float[] { 0f, 3.0f };

        // Act
        updater.DoAssessment(biomass, curYear: 4, randomNormal: ZeroRandom);

        // Assert
        data.Bestimate[igrp].Should().BeApproximately(7.5f, Tolerance);
        recruiter.Calls.Should().ContainSingle();
        recruiter.Calls[0].iGroup.Should().Be(igrp);
        recruiter.Calls[0].B.Should().BeApproximately(3.0f, Tolerance);
        recruiter.Calls[0].BioEst.Should().BeApproximately(3.0f, Tolerance);
        recruiter.Calls[0].Blast.Should().BeApproximately(2.0f, Tolerance);
        recruiter.Calls[0].iCurYear.Should().Be(4);
    }

    [Fact]
    public void DoAssessment_AppliesObservationErrorToObservedBiomass()
    {
        // Arrange
        const int igrp = 1;
        var (updater, data, recruiter) = BuildUpdater(numGroups: 1, numLiving: 1, nGear: 1);
        data.CVbiomEst[igrp] = 0.5f;
        var biomass = new float[] { 0f, 3.0f };
        float expectedBobs = 3.0f * (float)Math.Exp(0.5f * 1.0f); // Bobs = B * Exp(CV * rand)

        // Act
        updater.DoAssessment(biomass, curYear: 1, randomNormal: () => 1.0f);

        // Assert
        recruiter.Calls.Should().ContainSingle();
        recruiter.Calls[0].B.Should().BeApproximately(3.0f, Tolerance);
        recruiter.Calls[0].BioEst.Should().BeApproximately(expectedBobs, Tolerance);
    }

    [Fact]
    public void DoAssessment_OnlyProcessesLivingGroups()
    {
        // Arrange
        const int living = 1, nonLiving = 2;
        var (updater, data, recruiter) = BuildUpdater(numGroups: 2, numLiving: 1, nGear: 1);
        data.Bestimate[living] = 2.0f;
        data.Bestimate[nonLiving] = 42.0f; // must stay untouched (index > nLiving)
        recruiter.Result = (_, _, bioEst, _, _) => bioEst;
        var biomass = new float[] { 0f, 3.0f, 5.0f };

        // Act
        updater.DoAssessment(biomass, curYear: 1, randomNormal: ZeroRandom);

        // Assert
        recruiter.Calls.Should().ContainSingle();
        data.Bestimate[living].Should().BeApproximately(3.0f, Tolerance);
        data.Bestimate[nonLiving].Should().Be(42.0f);
    }
}
