// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwEMSEPlugin.HCR_GroupNS;
using FluentAssertions;
using Xunit;

namespace EwEMSEPlugin.Tests
{
    /// <summary>
    /// Unit tests for the Harvest Control Rule (HCR) math in <see cref="HCR_Group"/>.
    /// These verify the fishing mortality (F) produced by <c>CalcFfromHCR</c> and the
    /// resulting Total Allowable Catch (TAC = F * Biomass) that <c>cMSE.DetermineQuotas</c>
    /// derives from it.
    /// </summary>
    public sealed class HCR_GroupTests
    {
        private const float Tolerance = 1e-5f;

        private static float CalcF(HCR_Group hcr, float biomass)
        {
            // CalcFfromHCR takes its argument ByRef; use a local so we can pass it.
            float b = biomass;
            return hcr.CalcFfromHCR(ref b);
        }

        // -------------------------------------------------------------------
        // Traditional hockey-stick HCR
        // -------------------------------------------------------------------

        [Fact]
        public void Traditional_BiomassAboveUpperLimit_ReturnsMaxF_AndTac()
        {
            // Arrange
            var hcr = new HCR_Group
            {
                HCR_Type = eHCR_Type.Traditional,
                LowerLimit = 0.5f,
                UpperLimit = 1.0f,
                MaxF = 0.4f
            };
            float biomass = 1.5f;

            // Act
            float f = CalcF(hcr, biomass);
            float tac = f * biomass;

            // Assert
            f.Should().BeApproximately(0.4f, Tolerance);
            tac.Should().BeApproximately(0.6f, Tolerance);
        }

        [Fact]
        public void Traditional_BiomassBelowLowerLimit_ReturnsZeroF_AndZeroTac()
        {
            // Arrange
            var hcr = new HCR_Group
            {
                HCR_Type = eHCR_Type.Traditional,
                LowerLimit = 0.5f,
                UpperLimit = 1.0f,
                MaxF = 0.4f
            };
            float biomass = 0.3f;

            // Act
            float f = CalcF(hcr, biomass);
            float tac = f * biomass;

            // Assert
            f.Should().Be(0f);
            tac.Should().Be(0f);
        }

        [Fact]
        public void Traditional_BiomassOnRamp_InterpolatesF_AndTac()
        {
            // Arrange
            var hcr = new HCR_Group
            {
                HCR_Type = eHCR_Type.Traditional,
                LowerLimit = 0.5f,
                UpperLimit = 1.0f,
                MaxF = 0.4f
            };
            float biomass = 0.75f; // halfway up the ramp => 0.5 * MaxF

            // Act
            float f = CalcF(hcr, biomass);
            float tac = f * biomass;

            // Assert
            f.Should().BeApproximately(0.2f, Tolerance);
            tac.Should().BeApproximately(0.15f, Tolerance);
        }

        // -------------------------------------------------------------------
        // Multilevel HCR
        // -------------------------------------------------------------------

        [Fact]
        public void Multilevel_BiomassAboveUpperLimit_ReturnsMaxF()
        {
            // Arrange
            var hcr = new HCR_Group
            {
                HCR_Type = eHCR_Type.Multilevel,
                LowerLimit = 0.5f,
                UpperLimit = 1.0f,
                BStep = 0.5f,
                MinF = 0.05f,
                MaxF = 0.4f
            };
            float biomass = 1.5f;

            // Act
            float f = CalcF(hcr, biomass);

            // Assert
            f.Should().BeApproximately(0.4f, Tolerance);
        }

        [Fact]
        public void Multilevel_BiomassBelowBStep_ReturnsMinF()
        {
            // Arrange
            var hcr = new HCR_Group
            {
                HCR_Type = eHCR_Type.Multilevel,
                LowerLimit = 0.5f,
                UpperLimit = 1.0f,
                BStep = 0.5f,
                MinF = 0.05f,
                MaxF = 0.4f
            };
            float biomass = 0.3f;

            // Act
            float f = CalcF(hcr, biomass);

            // Assert
            f.Should().BeApproximately(0.05f, Tolerance);
        }

        [Fact]
        public void Multilevel_BiomassOnRamp_InterpolatesBetweenMinFandMaxF_AndTac()
        {
            // Arrange
            var hcr = new HCR_Group
            {
                HCR_Type = eHCR_Type.Multilevel,
                LowerLimit = 0.5f,
                UpperLimit = 1.0f,
                BStep = 0.5f,
                MinF = 0.05f,
                MaxF = 0.4f
            };
            float biomass = 0.75f;
            // MinF + (B - Lower) * ((MaxF - MinF) / (Upper - Lower))
            // = 0.05 + 0.25 * (0.35 / 0.5) = 0.225
            const float expectedF = 0.225f;

            // Act
            float f = CalcF(hcr, biomass);
            float tac = f * biomass;

            // Assert
            f.Should().BeApproximately(expectedF, Tolerance);
            tac.Should().BeApproximately(expectedF * biomass, Tolerance);
        }
    }
}
