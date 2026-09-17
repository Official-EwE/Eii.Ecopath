// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore.MSE;
using FluentAssertions;

namespace EwECore.Tests.MSE;

/// <summary>
/// Unit tests for <see cref="cRandomService"/>. A fixed seed makes the generated
/// sequences reproducible, so determinism, ranges and loose statistical properties
/// can be asserted without flakiness.
/// </summary>
public sealed class cRandomServiceTests
{
    private const int Seed = 42;
    private const int SampleSize = 10000;

    // -----------------------------------------------------------------------
    // Construction and seeding
    // -----------------------------------------------------------------------

    [Fact]
    public void SameSeed_ProducesIdenticalSequences()
    {
        // Arrange
        var first = new cRandomService(Seed);
        var second = new cRandomService(Seed);

        // Act
        var firstSequence = Enumerable.Range(0, 100).Select(_ => first.RandomNormal()).ToArray();
        var secondSequence = Enumerable.Range(0, 100).Select(_ => second.RandomNormal()).ToArray();

        // Assert
        firstSequence.Should().Equal(secondSequence);
    }

    [Fact]
    public void DifferentSeeds_ProduceDifferentSequences()
    {
        // Arrange
        var first = new cRandomService(Seed);
        var second = new cRandomService(Seed + 1);

        // Act
        var firstSequence = Enumerable.Range(0, 100).Select(_ => first.RandomNormal()).ToArray();
        var secondSequence = Enumerable.Range(0, 100).Select(_ => second.RandomNormal()).ToArray();

        // Assert
        firstSequence.Should().NotEqual(secondSequence);
    }

    [Fact]
    public void ParameterlessConstruction_ProducesWorkingGenerator()
    {
        // Arrange
        var service = new cRandomService();

        // Act
        double value = service.NextDouble();

        // Assert
        value.Should().BeGreaterThanOrEqualTo(0.0).And.BeLessThan(1.0);
    }

    // -----------------------------------------------------------------------
    // NextDouble
    // -----------------------------------------------------------------------

    [Fact]
    public void NextDouble_ReturnsValuesInUnitInterval()
    {
        // Arrange
        var service = new cRandomService(Seed);

        // Act
        var values = Enumerable.Range(0, SampleSize).Select(_ => service.NextDouble()).ToArray();

        // Assert
        values.Should().OnlyContain(v => v >= 0.0 && v < 1.0);
    }

    // -----------------------------------------------------------------------
    // Normal (Box-Muller)
    // -----------------------------------------------------------------------

    [Fact]
    public void Normal_HasMeanZeroAndStdDevOne()
    {
        // Arrange
        var service = new cRandomService(Seed);

        // Act
        var values = Enumerable.Range(0, SampleSize).Select(_ => (double)service.Normal()).ToArray();

        // Assert
        double mean = values.Average();
        double stdDev = Math.Sqrt(values.Sum(v => (v - mean) * (v - mean)) / values.Length);
        mean.Should().BeApproximately(0.0, 0.05);
        stdDev.Should().BeApproximately(1.0, 0.05);
    }

    [Fact]
    public void Normal_ReturnsFiniteValues()
    {
        // Arrange
        var service = new cRandomService(Seed);

        // Act
        var values = Enumerable.Range(0, SampleSize).Select(_ => service.Normal()).ToArray();

        // Assert
        values.Should().OnlyContain(v => !float.IsNaN(v) && !float.IsInfinity(v));
    }

    // -----------------------------------------------------------------------
    // RandomNormal (sum of twelve)
    // -----------------------------------------------------------------------

    [Fact]
    public void RandomNormal_HasMeanZeroAndStdDevOne()
    {
        // Arrange
        var service = new cRandomService(Seed);

        // Act
        var values = Enumerable.Range(0, SampleSize).Select(_ => (double)service.RandomNormal()).ToArray();

        // Assert
        double mean = values.Average();
        double stdDev = Math.Sqrt(values.Sum(v => (v - mean) * (v - mean)) / values.Length);
        mean.Should().BeApproximately(0.0, 0.05);
        stdDev.Should().BeApproximately(1.0, 0.05);
    }

    [Fact]
    public void RandomNormal_StaysWithinSumOfTwelveBounds()
    {
        // Arrange
        var service = new cRandomService(Seed);

        // Act
        var values = Enumerable.Range(0, SampleSize).Select(_ => service.RandomNormal()).ToArray();

        // Assert
        values.Should().OnlyContain(v => v > -6f && v < 6f);
    }

    // -----------------------------------------------------------------------
    // Normal2 (inverse sigmoid)
    // -----------------------------------------------------------------------

    [Fact]
    public void Normal2_HasMeanApproximatelyZero()
    {
        // Arrange
        var service = new cRandomService(Seed);

        // Act
        var values = Enumerable.Range(0, SampleSize).Select(_ => (double)service.Normal2()).ToArray();

        // Assert
        values.Average().Should().BeApproximately(0.0, 0.1);
        values.Should().OnlyContain(v => !double.IsNaN(v) && !double.IsInfinity(v));
    }

    // -----------------------------------------------------------------------
    // RandNormDist
    // -----------------------------------------------------------------------

    [Fact]
    public void RandNormDist_EqualsNormalTimesStdevPlusMean()
    {
        // Arrange
        const float stdev = 2.5f, mean = 10f;
        var reference = new cRandomService(Seed);
        var service = new cRandomService(Seed);

        // Act
        float expected = reference.Normal() * stdev + mean;
        float actual = service.RandNormDist(stdev, mean);

        // Assert
        actual.Should().BeApproximately(expected, 1e-6f);
    }

    [Fact]
    public void RandNormDist_HasRequestedMeanAndStdDev()
    {
        // Arrange
        const float stdev = 2.0f, mean = 5.0f;
        var service = new cRandomService(Seed);

        // Act
        var values = Enumerable.Range(0, SampleSize).Select(_ => (double)service.RandNormDist(stdev, mean)).ToArray();

        // Assert
        double sampleMean = values.Average();
        double sampleStdDev = Math.Sqrt(values.Sum(v => (v - sampleMean) * (v - sampleMean)) / values.Length);
        sampleMean.Should().BeApproximately(mean, 0.1);
        sampleStdDev.Should().BeApproximately(stdev, 0.1);
    }
}
