// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using System.Collections.Concurrent;
using System.Reflection;
using FluentAssertions;

namespace EwECore.Tests.Core;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ColdStartStateCollection
{
    public const string Name = "Process-wide lazily initialized state";
}

/// <summary>
/// The metadata registry is a lazily initialized process-wide singleton. These tests reset it to its
/// cold-start state so that concurrent first access, as happens when several cores are constructed at once, can be observed.
/// </summary>
[Collection(ColdStartStateCollection.Name)]
public sealed class cVariableMetaDataConcurrencyTests
{
    private const int Attempts = 25;
    private const int ThreadCount = 8;

    private static readonly FieldInfo InstanceField = GetStaticField("s_inst");
    private static readonly FieldInfo RegistryField = GetStaticField("s_metadata");
    private static readonly FieldInfo ReadyField = GetStaticField("s_ready");

    private static FieldInfo GetStaticField(string name) =>
        typeof(cVariableMetaData).GetField(name, BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new InvalidOperationException($"Field {name} not found on {nameof(cVariableMetaData)}");

    private static void ResetToColdStart()
    {
        ReadyField.SetValue(null, false);
        InstanceField.SetValue(null, null);
        RegistryField.SetValue(null, null);
    }

    private static eVarNameFlags[] RegisteredVariables()
    {
        ResetToColdStart();
        return Enum.GetValues<eVarNameFlags>()
            .Where(v => cVariableMetaData.Get(v) is not null)
            .ToArray();
    }

    [Fact]
    public void Get_ConcurrentColdStart_NeverObservesPartiallyInitializedRegistry()
    {
        // Arrange
        object? savedInstance = InstanceField.GetValue(null);
        object? savedRegistry = RegistryField.GetValue(null);
        object? savedReady = ReadyField.GetValue(null);
        var failures = new ConcurrentBag<string>();

        try
        {
            eVarNameFlags[] expected = RegisteredVariables();
            expected.Should().NotBeEmpty("a serial cold start must register metadata");

            for (int attempt = 0; attempt < Attempts; attempt++)
            {
                ResetToColdStart();
                using var barrier = new Barrier(ThreadCount);

                // Act
                var threads = Enumerable.Range(0, ThreadCount).Select(_ => new Thread(() =>
                {
                    try
                    {
                        barrier.SignalAndWait();
                        // Every thread asks for the full set; each answer must already be available.
                        foreach (eVarNameFlags v in expected)
                        {
                            if (cVariableMetaData.Get(v) is null)
                            {
                                failures.Add($"Attempt {attempt}: metadata for {v} missing during initialization");
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failures.Add($"Attempt {attempt}: {ex.GetType().Name}: {ex.Message}");
                    }
                })).ToArray();

                foreach (Thread t in threads) t.Start();
                foreach (Thread t in threads) t.Join();
            }

            // Assert
            failures.Should().BeEmpty("concurrent first access must wait for initialization to complete");
        }
        finally
        {
            InstanceField.SetValue(null, savedInstance);
            RegistryField.SetValue(null, savedRegistry);
            ReadyField.SetValue(null, savedReady);
        }
    }

    [Fact]
    public void Get_ConcurrentColdStart_ReturnsSameInstancePerVariable()
    {
        // Arrange
        object? savedInstance = InstanceField.GetValue(null);
        object? savedRegistry = RegistryField.GetValue(null);
        object? savedReady = ReadyField.GetValue(null);
        var mismatches = new ConcurrentBag<string>();

        try
        {
            for (int attempt = 0; attempt < Attempts; attempt++)
            {
                ResetToColdStart();
                using var barrier = new Barrier(ThreadCount);
                var results = new cVariableMetaData?[ThreadCount];

                // Act
                var threads = Enumerable.Range(0, ThreadCount).Select(i => new Thread(() =>
                {
                    try
                    {
                        barrier.SignalAndWait();
                        results[i] = cVariableMetaData.Get(eVarNameFlags.Biomass);
                    }
                    catch (Exception ex)
                    {
                        mismatches.Add($"Attempt {attempt}: {ex.GetType().Name}: {ex.Message}");
                    }
                })).ToArray();

                foreach (Thread t in threads) t.Start();
                foreach (Thread t in threads) t.Join();

                if (results.Any(r => r is null) || results.Distinct().Count() != 1)
                {
                    mismatches.Add($"Attempt {attempt}: threads received missing or differing metadata instances");
                }
            }

            // Assert
            mismatches.Should().BeEmpty("the registry must be a single shared, fully initialized instance");
        }
        finally
        {
            InstanceField.SetValue(null, savedInstance);
            RegistryField.SetValue(null, savedRegistry);
            ReadyField.SetValue(null, savedReady);
        }
    }
}
