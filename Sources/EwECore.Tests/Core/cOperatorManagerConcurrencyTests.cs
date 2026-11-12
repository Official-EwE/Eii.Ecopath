// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using System.Collections.Concurrent;
using FluentAssertions;

namespace EwECore.Tests.Core;

[Collection(ColdStartStateCollection.Name)]
public sealed class cOperatorManagerConcurrencyTests
{
    private const int Attempts = 200;
    private const int ThreadCount = 8;

    [Fact]
    public void GetOperator_ConcurrentColdStart_ReturnsSameInstanceToAllThreads()
    {
        // Arrange
        var field = ColdStartRace.GetStaticField(typeof(cOperatorManager), "m_operators");
        object? saved = field.GetValue(null);
        var failures = new ConcurrentBag<string>();

        try
        {
            for (int attempt = 0; attempt < Attempts; attempt++)
            {
                field.SetValue(null, null);
                var results = new cOperatorBase?[ThreadCount];

                // Act
                ColdStartRace.Run(ThreadCount,
                    i => results[i] = cOperatorManager.getOperator(eOperators.LessThan),
                    failures, $"Attempt {attempt}");

                if (results.Any(r => r is null) || results.Distinct().Count() != 1)
                {
                    failures.Add($"Attempt {attempt}: threads received missing or differing operator instances");
                }
            }
        }
        finally
        {
            field.SetValue(null, saved);
        }

        // Assert
        failures.Should().BeEmpty("the operator manager must hand out one shared, fully initialized set of operators");
    }
}
