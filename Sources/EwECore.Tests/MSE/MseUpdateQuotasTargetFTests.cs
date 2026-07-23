// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSE;
using EwECore.Tests.Fixtures;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// 
/// This is the old MSE quota update test class, which is now being refactored into smaller, more focused tests.
/// 
/// Unit tests for the <b>Target Fishing Mortality</b> (hockey-stick) branch of
/// <c>cMSE.UpdateQuotas</c>.
/// <para>
/// The branch computes, per living group <c>igrp</c>:
/// <code>
/// brange  = max(Bbase - Blim, 1e-20)
/// FTarget = Fopt * (Bestimate - Blim) / brange   (clamped to [Fmin, Fopt])
/// tQuota  = FTarget * Bestimate
/// tQuota *= Exp(CVbiomEst * RandomNormal() - 0.5 * CVbiomEst^2)
/// QuotaTime(iflt, igrp) = tQuota * Quotashare(iflt, igrp)
/// </code>
/// The branch is only reached when <c>TAC</c>, <c>FixedEscapement</c> and
/// <c>FixedF</c> are all zero for the group.  Setting <c>CVbiomEst = 0</c> makes
/// the uncertainty factor <c>Exp(0) = 1</c>, so the calculation becomes fully
/// deterministic.
/// </para>
/// <para>
/// Each test mutates <c>MSEData</c> for a single group/fleet and restores the
/// original values in a <c>finally</c> block so the shared model fixture is not
/// disturbed for other tests in the collection.
/// </para>
/// </summary>
[Collection("Model")]
public sealed class MseUpdateQuotasTargetFTests
{
    private const float Tolerance = 1e-4f;

    private readonly ModelFixture _fixture;

    public MseUpdateQuotasTargetFTests(ModelFixture fixture)
    {
        _fixture = fixture;
    }

    private cMSEManager MSEManager => _fixture.Core.MSEManager;

    /// <summary>
    /// Snapshot of the <c>MSEData</c> fields mutated by a single test for one
    /// group and one fleet, used to restore state afterwards.
    /// </summary>
    private sealed record GroupState(
        float Bbase, float Blim, float Fopt, float Fmin, float Bestimate,
        float FTarget, float CVbiomEst, float TAC, float FixedF,
        float FixedEscapement, float Quotashare, float QuotaTime);

    private GroupState Snapshot(int igrp, int iflt)
    {
        var d = MSEManager.MSEData;
        return new GroupState(
            d.Bbase[igrp], d.Blim[igrp], d.Fopt[igrp], d.Fmin[igrp], d.Bestimate[igrp],
            d.FTarget[igrp], d.CVbiomEst[igrp], d.TAC[igrp], d.FixedF[igrp],
            d.FixedEscapement[igrp], d.Quotashare[iflt, igrp], d.QuotaTime[iflt, igrp]);
    }

    private void Restore(int igrp, int iflt, GroupState s)
    {
        var d = MSEManager.MSEData;
        d.Bbase[igrp] = s.Bbase;
        d.Blim[igrp] = s.Blim;
        d.Fopt[igrp] = s.Fopt;
        d.Fmin[igrp] = s.Fmin;
        d.Bestimate[igrp] = s.Bestimate;
        d.FTarget[igrp] = s.FTarget;
        d.CVbiomEst[igrp] = s.CVbiomEst;
        d.TAC[igrp] = s.TAC;
        d.FixedF[igrp] = s.FixedF;
        d.FixedEscapement[igrp] = s.FixedEscapement;
        d.Quotashare[iflt, igrp] = s.Quotashare;
        d.QuotaTime[iflt, igrp] = s.QuotaTime;
    }

    /// <summary>
    /// Configures <paramref name="igrp"/> so the Target Fishing Mortality branch
    /// runs deterministically (no TAC / FixedF / FixedEscapement, zero CV).
    /// </summary>
    private void ArrangeTargetFBranch(
        int igrp, int iflt,
        float bbase, float blim, float fopt, float fmin, float bestimate, float quotashare)
    {
        var d = MSEManager.MSEData;
        d.TAC[igrp] = 0f;
        d.FixedEscapement[igrp] = 0f;
        d.FixedF[igrp] = 0f;
        d.CVbiomEst[igrp] = 0f; // Exp(0) == 1 -> deterministic quota
        d.Bbase[igrp] = bbase;
        d.Blim[igrp] = blim;
        d.Fopt[igrp] = fopt;
        d.Fmin[igrp] = fmin;
        d.Bestimate[igrp] = bestimate;
        d.Quotashare[iflt, igrp] = quotashare;

        // UpdateQuotas always calls RandomNormal(); seed the RNG so it is
        // initialized even though CVbiomEst == 0 neutralizes its effect.
        MSEManager.MSE.SeedRandomizer(42);
    }

    private float[] MakeBiomassArg()
    {
        // The argument is only forwarded to the plugin callback; size it safely.
        return new float[MSEManager.NumGroups + 1];
    }

    // -----------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------

    [Fact]
    public void TargetF_NominalBiomass_ComputesHockeyStickFTarget()
    {
        // Arrange
        const int igrp = 1;
        const int iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, fmin = 0.0f;
        const float bestimate = 1.25f, quotashare = 0.5f;
        var state = Snapshot(igrp, iflt);
        try
        {
            ArrangeTargetFBranch(igrp, iflt, bbase, blim, fopt, fmin, bestimate, quotashare);
            float expectedFTarget = fopt * (bestimate - blim) / (bbase - blim);
            float expectedQuota = expectedFTarget * bestimate * quotashare;

            // Act
            MSEManager.MSE.UpdateQuotas(MakeBiomassArg());

            // Assert
            MSEManager.MSEData.FTarget[igrp].Should().BeApproximately(expectedFTarget, Tolerance);
            MSEManager.MSEData.QuotaTime[iflt, igrp].Should().BeApproximately(expectedQuota, Tolerance);
        }
        finally
        {
            Restore(igrp, iflt, state);
        }
    }

    [Fact]
    public void TargetF_BiomassAtOrAboveBbase_ClampsToFopt()
    {
        // Arrange
        const int igrp = 1;
        const int iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, fmin = 0.0f;
        const float bestimate = 3.0f, quotashare = 1.0f; // above Bbase -> raw FTarget > Fopt
        var state = Snapshot(igrp, iflt);
        try
        {
            ArrangeTargetFBranch(igrp, iflt, bbase, blim, fopt, fmin, bestimate, quotashare);

            // Act
            MSEManager.MSE.UpdateQuotas(MakeBiomassArg());

            // Assert
            MSEManager.MSEData.FTarget[igrp].Should().BeApproximately(fopt, Tolerance);
            MSEManager.MSEData.QuotaTime[iflt, igrp].Should().BeApproximately(fopt * bestimate * quotashare, Tolerance);
        }
        finally
        {
            Restore(igrp, iflt, state);
        }
    }

    [Fact]
    public void TargetF_BiomassAtOrBelowBlim_WithZeroFmin_ProducesZeroQuota()
    {
        // Arrange
        const int igrp = 1;
        const int iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, fmin = 0.0f;
        const float bestimate = 0.5f, quotashare = 1.0f; // at Blim -> raw FTarget == 0
        var state = Snapshot(igrp, iflt);
        try
        {
            ArrangeTargetFBranch(igrp, iflt, bbase, blim, fopt, fmin, bestimate, quotashare);

            // Act
            MSEManager.MSE.UpdateQuotas(MakeBiomassArg());

            // Assert
            MSEManager.MSEData.FTarget[igrp].Should().BeApproximately(0f, Tolerance);
            MSEManager.MSEData.QuotaTime[iflt, igrp].Should().BeApproximately(0f, Tolerance);
        }
        finally
        {
            Restore(igrp, iflt, state);
        }
    }

    [Fact]
    public void TargetF_LowBiomass_WithPositiveFmin_ClampsToFmin()
    {
        // Arrange
        const int igrp = 1;
        const int iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, fmin = 0.1f;
        const float bestimate = 0.4f, quotashare = 1.0f; // below Blim -> raw FTarget < 0 -> clamp up to Fmin
        var state = Snapshot(igrp, iflt);
        try
        {
            ArrangeTargetFBranch(igrp, iflt, bbase, blim, fopt, fmin, bestimate, quotashare);

            // Act
            MSEManager.MSE.UpdateQuotas(MakeBiomassArg());

            // Assert
            MSEManager.MSEData.FTarget[igrp].Should().BeApproximately(fmin, Tolerance);
            MSEManager.MSEData.QuotaTime[iflt, igrp].Should().BeApproximately(fmin * bestimate * quotashare, Tolerance);
        }
        finally
        {
            Restore(igrp, iflt, state);
        }
    }

    [Fact]
    public void TargetF_DegenerateBrange_UsesFallback_ClampsToFopt()
    {
        // Arrange
        const int igrp = 1;
        const int iflt = 1;
        // Bbase <= Blim forces brange to the 1e-20 fallback, making the raw
        // FTarget enormous and therefore clamped to Fopt.
        const float bbase = 1.0f, blim = 1.0f, fopt = 0.3f, fmin = 0.0f;
        const float bestimate = 1.5f, quotashare = 1.0f;
        var state = Snapshot(igrp, iflt);
        try
        {
            ArrangeTargetFBranch(igrp, iflt, bbase, blim, fopt, fmin, bestimate, quotashare);

            // Act
            MSEManager.MSE.UpdateQuotas(MakeBiomassArg());

            // Assert
            MSEManager.MSEData.FTarget[igrp].Should().BeApproximately(fopt, Tolerance);
            MSEManager.MSEData.QuotaTime[iflt, igrp].Should().BeApproximately(fopt * bestimate * quotashare, Tolerance);
        }
        finally
        {
            Restore(igrp, iflt, state);
        }
    }

    [Fact]
    public void TargetF_FleetShare_DistributesQuotaByQuotashare()
    {
        // Arrange
        const int igrp = 1;
        const int iflt = 1;
        const float bbase = 2.0f, blim = 0.5f, fopt = 0.4f, fmin = 0.0f;
        const float bestimate = 1.25f, quotashare = 0.25f;
        var state = Snapshot(igrp, iflt);
        try
        {
            ArrangeTargetFBranch(igrp, iflt, bbase, blim, fopt, fmin, bestimate, quotashare);
            float expectedFTarget = fopt * (bestimate - blim) / (bbase - blim);
            float expectedGroupQuota = expectedFTarget * bestimate;

            // Act
            MSEManager.MSE.UpdateQuotas(MakeBiomassArg());

            // Assert
            MSEManager.MSEData.QuotaTime[iflt, igrp]
                .Should().BeApproximately(expectedGroupQuota * quotashare, Tolerance);
        }
        finally
        {
            Restore(igrp, iflt, state);
        }
    }

    [Fact]
    public void Test()
    {
        //// Arrange
        //cMSE m_MSE = new cMSE(null);

        //var m_MSEdata = new cMSEDataStructures();
        //m_MSE.Init(m_MSEdata, );
        //var biomassArg = new float[10 + 1];

        //// Act
        //m_MSE.UpdateQuotas(biomassArg);

        // Assert

    }

}
