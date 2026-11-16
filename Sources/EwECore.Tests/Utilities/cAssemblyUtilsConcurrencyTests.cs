// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using System.Collections.Concurrent;
using System.Reflection;
using EwECore.Tests.Core;
using EwEUtils.Utilities;
using FluentAssertions;

namespace EwECore.Tests.Utilities;

[Collection(ColdStartStateCollection.Name)]
public sealed class cAssemblyUtilsConcurrencyTests
{
    private const int ThreadCount = 8;
    private const int NamesPerThread = 150;

    [Fact]
    public void GetVersion_ConcurrentFirstLookups_PopulatesCacheWithoutErrors()
    {
        // Arrange
        string prefix = $"Fake.{Guid.NewGuid():N}";
        var failures = new ConcurrentBag<string>();
        var versions = new ConcurrentDictionary<string, Version?>();

        // Act
        ColdStartRace.Run(ThreadCount, t =>
        {
            for (int n = 0; n < NamesPerThread; n++)
            {
                // Every thread looks up every name, so first lookups overlap while the cache grows
                string name = $"{prefix}.{n}";
                var an = new AssemblyName($"{name}, Version=1.{n}.0.0");
                versions[name + "#" + t] = cAssemblyUtils.GetVersion(an);
            }
        }, failures, "GetVersion");

        // Assert
        failures.Should().BeEmpty("the assembly state cache must tolerate concurrent first lookups");
        versions.Should().HaveCount(ThreadCount * NamesPerThread);
        versions.Should().OnlyContain(kv => kv.Value != null);
    }
}
