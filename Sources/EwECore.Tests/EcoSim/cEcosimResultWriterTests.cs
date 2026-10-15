// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.Ecosim;
using FluentAssertions;

namespace EwECore.Tests.EcoSim;

public sealed class cEcosimResultWriterTests
{
    [Fact]
    public void GetOutputFileName_PreservesDirectoryCase()
    {
        // Arrange
        string dir = Path.Combine(Path.GetTempPath(), "EwE_CaseTest_MixedCase");

        // Act
        string result = cEcosimResultWriter.GetOutputFileName(dir, true,
            cEcosimResultWriter.eResultTypes.Biomass);

        // Assert
        Path.GetDirectoryName(result).Should().Be(dir);
    }

    [Fact]
    public void GetOutputFileName_PreservesGroupNameCase()
    {
        // Arrange
        string dir = Path.Combine(Path.GetTempPath(), "EwE_CaseTest");

        // Act
        string result = cEcosimResultWriter.GetOutputFileName(dir, false,
            cEcosimResultWriter.eResultTypes.PredationMortality, "Cod");

        // Assert
        Path.GetFileName(result).Should().Be("predation_Cod_monthly.csv");
    }

    [Fact]
    public void GetOutputFileName_PreservesResultTypeCase()
    {
        // Arrange
        string dir = Path.Combine(Path.GetTempPath(), "EwE_CaseTest");

        // Act
        string result = cEcosimResultWriter.GetOutputFileName(dir, true,
            cEcosimResultWriter.eResultTypes.Biomass);

        // Assert
        Path.GetFileName(result).Should().Be("Biomass_annual.csv");
    }

    [Fact]
    public void GetOutputFileName_LowercasesFileNameOnly_PreservesDirectoryCase()
    {
        // Arrange
        string dir = Path.Combine(Path.GetTempPath(), "EwE_CaseTest_MixedCase");

        // Act
        string result = cEcosimResultWriter.GetOutputFileName(dir, false,
            cEcosimResultWriter.eResultTypes.Prey, "Cod");

        // Assert
        result.Should().Be(Path.Combine(dir, "prey_Cod_monthly.csv"));
    }
}
