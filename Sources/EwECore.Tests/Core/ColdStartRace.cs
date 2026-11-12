// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using System.Collections.Concurrent;
using System.Reflection;

namespace EwECore.Tests.Core;

/// <summary>Helpers to observe races on lazily initialized, process-wide state.</summary>
internal static class ColdStartRace
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    public static FieldInfo GetStaticField(Type type, string name) =>
        type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new InvalidOperationException($"Static field {name} not found on {type.Name}");

    /// <summary>Releases all threads simultaneously and records any exception or hang as a failure.</summary>
    public static void Run(int threadCount, Action<int> body, ConcurrentBag<string> failures, string label)
    {
        using var barrier = new Barrier(threadCount);
        Thread[] threads = Enumerable.Range(0, threadCount).Select(i => new Thread(() =>
        {
            try
            {
                barrier.SignalAndWait();
                body(i);
            }
            catch (Exception ex)
            {
                failures.Add($"{label}: {ex.GetType().Name}: {ex.Message}");
            }
        })
        { IsBackground = true }).ToArray();

        foreach (Thread t in threads) t.Start();
        foreach (Thread t in threads)
        {
            if (!t.Join(Timeout)) failures.Add($"{label}: thread did not complete within {Timeout.TotalSeconds}s");
        }
    }
}
