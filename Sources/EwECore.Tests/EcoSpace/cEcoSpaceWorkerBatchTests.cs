// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).

using System.Reflection;
using Microsoft.Extensions.Logging;

namespace EwECore.Tests.EcoSpace;

[Collection("Ecospace worker batches")]
public sealed class cEcoSpaceWorkerBatchTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(15);

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task OverlappingCoreBatches_CompleteIndependently(bool dedicatedThread, bool failFirstCore)
    {
        // Arrange
        using var coreA = new cCore();
        using var coreB = new cCore();
        var spaceA = GetSpace(coreA);
        var spaceB = GetSpace(coreB);
        ConfigureHabitat(spaceA);
        ConfigureHabitat(spaceB);
        if (failFirstCore)
            spaceA.EcoSpaceData.HabCap[1] = null!;

        using var batchA = new Batch(spaceA);
        using var batchB = new Batch(spaceB);
        using var started = new CountdownEvent(4);
        using var releaseA = new ManualResetEventSlim();
        using var releaseB = new ManualResetEventSlim();
        Task<Exception?>? waitA = null;
        Task<Exception?>? waitB = null;
        try
        {
            // Act
            for (int group = 1; group <= 2; group++)
            {
                int index = group;
                batchA.Queue(() =>
                {
                    started.Signal();
                    WaitForRelease(releaseA);
                    batchA.InvokeWorker(spaceA, "AdjustLowHabCapsThreaded", index, index);
                }, dedicatedThread);
                batchB.Queue(() =>
                {
                    started.Signal();
                    WaitForRelease(releaseB);
                    batchB.InvokeWorker(spaceB, "AdjustLowHabCapsThreaded", index, index);
                }, dedicatedThread);
            }
            waitA = Task.Run<Exception?>(() => Record.Exception(() => batchA.Wait()));
            waitB = Task.Run<Exception?>(() => Record.Exception(() => batchB.Wait()));
            Assert.True(started.Wait(TestTimeout));
            releaseA.Set();
            var failureA = await waitA.WaitAsync(TestTimeout);

            // Assert
            if (failFirstCore)
            {
                var aggregate = Assert.IsType<AggregateException>(failureA);
                Assert.IsType<NullReferenceException>(Assert.Single(aggregate.InnerExceptions));
            }
            else
            {
                Assert.Null(failureA);
                AssertHabitat(spaceA, 1);
            }
            AssertHabitat(spaceA, 2);
            Assert.False(waitB.IsCompleted);
            Assert.Equal(0.01f, spaceB.EcoSpaceData.HabCap[1][1, 2]);

            releaseB.Set();
            Assert.Null(await waitB.WaitAsync(TestTimeout));
            AssertHabitat(spaceB, 1);
            AssertHabitat(spaceB, 2);
        }
        finally
        {
            releaseA.Set();
            releaseB.Set();
            if (waitA is not null) await waitA.WaitAsync(TestTimeout);
            if (waitB is not null) await waitB.WaitAsync(TestTimeout);
        }
    }

    [Theory]
    [InlineData("AdjustLowHabCapsThreaded", true)]
    [InlineData("PredictEffortDistributionThreaded", false)]
    [InlineData("PredictEffortDistributionThreadedLoadShared", false)]
    [InlineData("setFishMortFromEffort", false)]
    public async Task WorkerFailure_IsReportedOnlyAfterOtherWorkersFinish(string workerName, bool dedicatedThread)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        space.EcoSpaceData = null!;
        using var batch = new Batch(space);
        using var failed = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        Task<Exception?>? waiting = null;
        try
        {
            // Act
            batch.Queue(() =>
            {
                try { batch.InvokeWorker(space, workerName, 1, 1); }
                finally { failed.Set(); }
            }, dedicatedThread);
            batch.Queue(() => WaitForRelease(release), dedicatedThread);
            waiting = Task.Run<Exception?>(() => Record.Exception(() => batch.Wait()));
            Assert.True(failed.Wait(TestTimeout));

            // Assert
            Assert.False(waiting.IsCompleted);
            release.Set();
            var failure = Assert.IsType<AggregateException>(await waiting.WaitAsync(TestTimeout));
            Assert.IsType<NullReferenceException>(Assert.Single(failure.InnerExceptions));
        }
        finally
        {
            release.Set();
            if (waiting is not null) await waiting.WaitAsync(TestTimeout);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Timeout_DrainsWorkersBeforeThrowingAndPreservesFailures(bool failWorker)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        using var timeoutLogged = new ManualResetEventSlim();
        var logger = new TimeoutLogger(timeoutLogged);
        typeof(cEcoSpace).GetField("m_logger", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(space, logger);
        using var batch = new Batch(space);
        using var release = new ManualResetEventSlim();
        int modelValue = 0;
        Task<Exception?>? waiting = null;
        try
        {
            batch.Queue(() =>
            {
                WaitForRelease(release);
                modelValue = 42;
                if (failWorker) throw new InvalidOperationException("worker failed");
            });

            // Act
            waiting = Task.Run<Exception?>(() => Record.Exception(() => batch.Wait(0)));
            Assert.True(timeoutLogged.Wait(TestTimeout));

            // Assert
            Assert.False(waiting.IsCompleted);
            Assert.Equal(0, modelValue);
            release.Set();
            var failure = Assert.IsType<TimeoutException>(await waiting.WaitAsync(TestTimeout));
            Assert.Equal(42, modelValue);
            if (failWorker)
            {
                var aggregate = Assert.IsType<AggregateException>(failure.InnerException);
                Assert.IsType<InvalidOperationException>(Assert.Single(aggregate.InnerExceptions));
            }
            else
            {
                Assert.Null(failure.InnerException);
            }
        }
        finally
        {
            release.Set();
            if (waiting is not null) await waiting.WaitAsync(TestTimeout);
        }
    }

    [Fact]
    public async Task Dispose_DrainsWorkersWhenCallerFailsBeforeWaiting()
    {
        // Arrange
        using var core = new cCore();
        var batch = new Batch(GetSpace(core));
        using var release = new ManualResetEventSlim();
        using var disposing = new ManualResetEventSlim();
        int modelValue = 0;
        Task? disposal = null;
        try
        {
            batch.Queue(() =>
            {
                WaitForRelease(release);
                modelValue = 42;
            });

            // Act
            disposal = Task.Run(() =>
            {
                disposing.Set();
                batch.Dispose();
            });
            Assert.True(disposing.Wait(TestTimeout));

            // Assert
            Assert.False(disposal.IsCompleted);
            release.Set();
            await disposal.WaitAsync(TestTimeout);
            Assert.Equal(42, modelValue);
        }
        finally
        {
            release.Set();
            if (disposal is not null) await disposal.WaitAsync(TestTimeout);
            else batch.Dispose();
        }
    }

    [Fact]
    public void EmptyBatch_CompletesImmediately()
    {
        // Arrange
        using var core = new cCore();
        using var batch = new Batch(GetSpace(core));

        // Act
        var failure = Record.Exception(() => batch.Wait(0));

        // Assert
        Assert.Null(failure);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void HabitatEntryPoint_PreservesCalculatedCapacities(int threadCount)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        ConfigureHabitat(space);
        space.EcoSpaceData.nGridSolverThreads = threadCount;
        var run = typeof(cEcoSpace).GetMethod("runAjustLowHabCapsThreaded", BindingFlags.Instance | BindingFlags.NonPublic)!
            .CreateDelegate<Action>(space);

        // Act
        run();

        // Assert
        AssertHabitat(space, 1);
        AssertHabitat(space, 2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EffortEntryPoint_PropagatesFailuresBeforeStartingMortality(bool loadShared)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        var data = space.EcoSpaceData;
        data.NGroups = 2;
        data.nFleets = 2;
        data.nEffortDistThreads = 2;
        data.UseEffortDistThreshold = loadShared;
        data.Ftot = new float[3, 2, 2];
        data.EffortSpace = new float[3, 2, 2];
        data.TotEffort = null!;
        string name = loadShared ? "runEffortDistributionLoadShared" : "runEffortDistributionNoLoadShare";
        var run = typeof(cEcoSpace).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!
            .CreateDelegate<Action<int, int, int>>(space);

        // Act
        var failure = await Task.Run<Exception?>(() => Record.Exception(() => run(1, 1, 1))).WaitAsync(TestTimeout);

        // Assert
        var aggregate = Assert.IsType<AggregateException>(failure);
        Assert.Equal(2, aggregate.InnerExceptions.Count);
        Assert.All(aggregate.InnerExceptions, error => Assert.IsType<NullReferenceException>(error));
        Assert.All(data.Ftot.Cast<float>(), value => Assert.Equal(0f, value));
    }

    [Fact]
    public async Task FleetClaims_AreUniqueAndIndependentAcrossCoreBatches()
    {
        // Arrange
        using var coreA = new cCore();
        using var coreB = new cCore();
        using var batchA = new Batch(GetSpace(coreA));
        using var batchB = new Batch(GetSpace(coreB));
        using var started = new CountdownEvent(4);
        using var release = new ManualResetEventSlim();
        var claimsA = new System.Collections.Concurrent.ConcurrentBag<int>();
        var claimsB = new System.Collections.Concurrent.ConcurrentBag<int>();
        try
        {
            // Act
            for (int i = 0; i < 2; i++)
            {
                batchA.Queue(() => ClaimFleets(batchA, 7, claimsA, started, release));
                batchB.Queue(() => ClaimFleets(batchB, 11, claimsB, started, release));
            }
            Assert.True(started.Wait(TestTimeout));
            release.Set();
            await Task.WhenAll(Task.Run(() => batchA.Wait()), Task.Run(() => batchB.Wait())).WaitAsync(TestTimeout);

            // Assert
            Assert.Equal(Enumerable.Range(1, 7), claimsA.Order());
            Assert.Equal(Enumerable.Range(1, 11), claimsB.Order());
        }
        finally
        {
            release.Set();
        }
    }

    private static void ClaimFleets(Batch batch, int count,
        System.Collections.Concurrent.ConcurrentBag<int> claims, CountdownEvent started, ManualResetEventSlim release)
    {
        started.Signal();
        WaitForRelease(release);
        int fleet = 0;
        while (batch.GetNextFleet(count, ref fleet)) claims.Add(fleet);
    }

    private static cEcoSpace GetSpace(cCore core) =>
        (cEcoSpace)typeof(cCore).GetField("m_Ecospace", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(core)!;

    private static void ConfigureHabitat(cEcoSpace space)
    {
        var data = space.EcoSpaceData;
        data.NGroups = 2;
        data.InRow = 1;
        data.InCol = 2;
        data.Depth = new float[3, 4];
        data.Depth[1, 1] = data.Depth[1, 2] = 1;
        data.MinHabCap = 0.1f;
        data.CellLength = 2;
        data.UseHabCapGradientCorrections = true;
        data.isGroupHabCapChanged = new[] { false, true, true };
        data.Mvel = new[] { 0f, 1f, 1f };
        space.EcoPathData.PB = new[] { 0f, 1f, 1f };
        data.HabCap = new float[3][,];
        for (int group = 1; group <= 2; group++)
        {
            data.HabCap[group] = new float[3, 4];
            data.HabCap[group][1, 1] = 0.5f;
            data.HabCap[group][1, 2] = 0.01f;
        }
    }

    private static void AssertHabitat(cEcoSpace space, int group)
    {
        Assert.Equal(0.5f, space.EcoSpaceData.HabCap[group][1, 1]);
        Assert.Equal(0.1f * MathF.Exp(-0.4f), space.EcoSpaceData.HabCap[group][1, 2], 6);
    }

    private static void WaitForRelease(ManualResetEventSlim release)
    {
        if (!release.Wait(TestTimeout)) throw new TimeoutException("Test worker was not released.");
    }

    // Keep synchronization implementation private rather than exposing test-only engine APIs.
    private sealed class Batch : IDisposable
    {
        private static readonly Type BatchType = typeof(cEcoSpace).GetNestedType("cWorkerBatch", BindingFlags.NonPublic)!;
        private static readonly Type ArgumentsType = typeof(cEcoSpace).GetNestedType("cThreadedCallArgs", BindingFlags.NonPublic)!;
        private readonly object _instance;
        private readonly Action<WaitCallback, object?, bool> _queue;
        private readonly Action<int, string> _wait;
        private readonly FleetClaim _getNextFleet;
        private delegate bool FleetClaim(int count, ref int fleet);

        public Batch(cEcoSpace owner)
        {
            _instance = Activator.CreateInstance(BatchType, owner)!;
            _queue = BatchType.GetMethod("Queue")!.CreateDelegate<Action<WaitCallback, object?, bool>>(_instance);
            _wait = BatchType.GetMethod("WaitForCompletion")!.CreateDelegate<Action<int, string>>(_instance);
            _getNextFleet = BatchType.GetMethod("GetNextFleet")!.CreateDelegate<FleetClaim>(_instance);
        }

        public void Queue(Action action, bool dedicatedThread = false) => _queue(_ => action(), null, dedicatedThread);
        public void Wait(int timeout = Timeout.Infinite) => _wait(timeout, "Test Ecospace batch");
        public bool GetNextFleet(int count, ref int fleet) => _getNextFleet(count, ref fleet);
        public void Dispose() => ((IDisposable)_instance).Dispose();

        public void InvokeWorker(cEcoSpace space, string name, int first, int last)
        {
            var arguments = Activator.CreateInstance(ArgumentsType, _instance, first, last)!;
            var worker = typeof(cEcoSpace).GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                .CreateDelegate<WaitCallback>(space);
            worker(arguments);
        }
    }

    private sealed class TimeoutLogger(ManualResetEventSlim timeoutLogged) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (formatter(state, exception).Contains("timed out")) timeoutLogged.Set();
        }
    }
}
