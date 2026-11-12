// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using System.Collections.Concurrent;
using EwECore.SpatialData;
using EwECore.Tests.Core;
using FluentAssertions;

namespace EwECore.Tests.SpatialTemporal;

[Collection(ColdStartStateCollection.Name)]
public sealed class cSpatialDataCacheConcurrencyTests
{
    private const int Attempts = 200;
    private const int ThreadCount = 8;

    [Fact]
    public void DefaultDataCache_ConcurrentColdStart_ReturnsSameInstanceToAllThreads()
    {
        // Arrange
        var field = ColdStartRace.GetStaticField(typeof(cSpatialDataCache), "s_default");
        object? saved = field.GetValue(null);
        var failures = new ConcurrentBag<string>();

        try
        {
            for (int attempt = 0; attempt < Attempts; attempt++)
            {
                field.SetValue(null, null);
                var results = new cSpatialDataCache?[ThreadCount];

                // Act
                ColdStartRace.Run(ThreadCount,
                    i => results[i] = cSpatialDataCache.DefaultDataCache(),
                    failures, $"Attempt {attempt}");

                if (results.Any(r => r is null) || results.Distinct().Count() != 1)
                {
                    failures.Add($"Attempt {attempt}: threads received missing or differing cache instances");
                }
            }
        }
        finally
        {
            field.SetValue(null, saved);
        }

        // Assert
        failures.Should().BeEmpty("the default cache must be a single shared instance");
    }
}
